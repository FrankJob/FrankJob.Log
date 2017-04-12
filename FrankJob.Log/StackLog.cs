using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace FrankJob.Log
{
    public class StackLog
    {
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DateTime { get; set; }

        public string Message { get; set; }
        public string Method { get; set; }
        public string Namespace { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string HttpAction { get; set; }
        public string User { get; set; }

        [JsonConverter(typeof(HttpContextConverter))]
        public HttpContext HttpContext { get; set; }

        [JsonConverter(typeof(ServerVariablesConverter))]
        public IDictionary<string, string> ServerVariables { get; set; }

        [JsonConverter(typeof(StackTraceConverter))]
        public StackTrace Stacktrace { get; set; }

        [JsonConverter(typeof(UserObjectConverter))]
        public object[] UserObjects { get; set; }

        public Exception Exception { get; set; }

        public override string ToString()
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.DefaultValueHandling = UserConfiguration.ShowDefaultPropertiesValue;
            jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonSettings.Formatting = UserConfiguration.JsonIndented;
            jsonSettings.NullValueHandling = UserConfiguration.ShowNullPropertiesValue;
            //jsonSettings.MaxDepth = 5;

            var json = JsonConvert.SerializeObject(this, jsonSettings);
            if (UserConfiguration.ShowDefaultPropertiesValue == DefaultValueHandling.Ignore && UserConfiguration.ShowNullPropertiesValue == NullValueHandling.Ignore)
                return RemoveNull(json);
            else
                return json;

            //return new JavaScriptSerializer().Serialize(this);
        }

        private string RemoveNull(string json)
        {
            const string HTTP = "\"HttpContext\": null,";
            const string SERVVAR = "\"ServerVariables\": null,";
            const string TRACE = "\"Stacktrace\": null,";
            const string CONTROLLER = "\"Controller\": \"\",";
            const string ACTION = "\"Action\": \"\",";
            const string HTTPACTION = "\"HttpAction\": \"\",";

            const int PRE = 2;
            const int POS = 2;

            var httpPos = json.IndexOf(HTTP);
            if(httpPos != -1)
                json = json.Remove(httpPos - PRE, HTTP.Length + PRE + POS);
            var svPos = json.IndexOf(SERVVAR);
            if (svPos != -1)
                json = json.Remove(svPos - PRE, SERVVAR.Length + PRE + POS);
            var tracePos = json.IndexOf(TRACE);
            if (tracePos != -1)
                json = json.Remove(tracePos - PRE, TRACE.Length + PRE + POS);
            var controllerPos = json.IndexOf(CONTROLLER);
            if (controllerPos != -1)
                json = json.Remove(controllerPos - PRE, CONTROLLER.Length + PRE + POS);
            var actionPos = json.IndexOf(ACTION);
            if (actionPos != -1)
                json = json.Remove(actionPos - PRE, ACTION.Length + PRE + POS);
            var httpActionPos = json.IndexOf(HTTPACTION);
            if (httpActionPos != -1)
                json = json.Remove(httpActionPos - PRE, HTTPACTION.Length + PRE + POS);

            return json;
        }
    }
}
