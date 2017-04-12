using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace FrankJob.Log
{
    public class StackTraceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //if not enable, exit
            if (!UserConfiguration.EnableStackTrace)
            {
                writer.WriteNull();
                return;
            }

            var logTraceWithoutNumbers = UserConfiguration.StackTraceWithoutNumbers;

            var stack = value as StackTrace;
            var frames = stack.GetFrames();
            var aStack = new List<string>();
            for (int i = 0; i < frames.Length; i++)
            {
                if(logTraceWithoutNumbers == false)
                {
                    if (!string.IsNullOrEmpty(frames[i].GetFileName()))
                        aStack.Add(frames[i].ToString().Replace("\r\n", string.Empty));
                }
                else
                    aStack.Add(frames[i].ToString().Replace("\r\n", string.Empty));
            }
            
            serializer.Serialize(writer, aStack);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //Unnecessary because CanRead is false.
            //The type will skip the converter.
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StackTrace);
        }
    }

    public class HttpContextConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //if not enable, exit
            if (!UserConfiguration.EnableHttpContext)
            {
                writer.WriteNull();
                return;
            }

            var jsonObj = new JObject();
            var type = value.GetType();
            var ignoredHttpProperties = UserConfiguration.HttpContextPropertiesIgnore;

            //jsonObj.Add("Type", type.Name);
            //var obj = Convert.ChangeType(value, value.GetType());

            var allProps = type.GetProperties().OrderBy(p => p.Name);
            foreach (var prop in allProps)
            {
                if (ignoredHttpProperties.Any(p => p.Equals(prop.Name)))
                    continue;

                if (prop.CanRead)
                {
                    object propVal = prop.GetValue(value, null);
                    if (propVal != null)
                    {
                        serializer.ContractResolver = new HttpContextContractResolver();
                        jsonObj.Add(prop.Name, JToken.FromObject(propVal, serializer));
                    }
                }
            }

            serializer.Serialize(writer, jsonObj);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //Unnecessary because CanRead is false.
            //The type will skip the converter.
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HttpContext);
        }
    }

    public class ServerVariablesConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //if not enable, exit
            if (!UserConfiguration.EnableServerVariables)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //Unnecessary because CanRead is false.
            //The type will skip the converter.
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IDictionary<string, string>);
        }
    }

    public class UserObjectConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsonObj = new JObject();
            serializer.ContractResolver = new AllPropertiesContractResolver();
            jsonObj.Add(value.GetType().Name, JToken.FromObject(value, serializer));
            serializer.Serialize(writer, jsonObj);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //Unnecessary because CanRead is false.
            //The type will skip the converter.
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object[]);
        }
    }

    public class DateTimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dt = (DateTime) value;

            var jsonObj = new JObject();
            var date = string.Format("{0}-{1}-{2}", dt.Year, dt.Month, dt.Day);
            var time = string.Format("{0}:{1}:{2}:{3}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
            
            jsonObj.Add("Date", JToken.FromObject(date));
            jsonObj.Add("Time", JToken.FromObject(time));
            jsonObj.Add("DST", JToken.FromObject(dt.IsDaylightSavingTime()));

            serializer.Serialize(writer, jsonObj);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //Unnecessary because CanRead is false.
            //The type will skip the converter.
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }
}
