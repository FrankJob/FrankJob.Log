using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace FrankJob.Log
{
    //http://stackoverflow.com/questions/8926409/log4net-hierarchy-and-logging-levels
    //https://stackify.com/log4net-guide-dotnet-logging/
    //http://stackoverflow.com/questions/650694/changing-the-log-level-programmaticaly-in-log4net

    public static class CustomLogManager
    {
        static readonly Level traceLevel = new Level(200000, "STACK");
        static private ILog log { get; set; }
        static private bool IsWebApplication { get; set; }

        public static ILog GetLogger(string name)
        {
            //Configure Log Params
            Configure();
            log = LogManager.GetLogger(name);
            return log;
        }

        private static void Configure()
        {
            LogManager.GetRepository().LevelMap.Add(traceLevel);
            IsWebApplication = HttpContextValid();
        }

        private static bool HttpContextValid()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request != null)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private static string GetUser()
        {
            var user = string.Empty;
            if (IsWebApplication)
                user = HttpContext.Current.Request.ServerVariables["LOGON_USER"].ToLower(); //.Replace(@"\", @"\\")
            else
                user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower();
            return user;
        }

        private static bool UserNameValid()
        {
            //HttpContext.Current.Request.LogonUserIdentity.Name "LENOVO-JR\\franc"
            //HttpContext.Current.Request.ServerVariables["LOGON_USER"] "MicrosoftAccount\\francisco.mcse@gmail.com"

            //UsersToMonitor return lowercase users
            var appSettingsUsers = UserConfiguration.UsersToMonitor;
            if (appSettingsUsers.Count == 0) return true;

            var localUser = GetUser();
            return appSettingsUsers.Any(t => t.Equals(localUser));
        }

        private static bool IsStackEnabled()
        {
            return UserConfiguration.EnableStackLog;
        }

        private static Dictionary<string, string> LogServerVariables()
        {
            if (!IsWebApplication) return null;

            var serverVariables = new Dictionary<string, string>();
            foreach (var item in HttpContext.Current.Request.ServerVariables)
                serverVariables.Add(item.ToString(), HttpContext.Current.Request.ServerVariables[item.ToString()]);
            return serverVariables;
        }

        private static dynamic Check<T>(Expression<Func<T>> expr)
        {
            var body = ((MemberExpression)expr.Body);
            var name = body.Member.Name;
            var value = ((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value);
            return new { Name = name, Value = Convert.ChangeType(value,value.GetType()) };
        }

        public static void Stack(this ILog log, string message)
        {
            if (IsStackEnabled() && UserNameValid())
                log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, traceLevel, message, null);
        }

        public static void Stack(this ILog log, string message, Exception exception)
        {
            if (IsStackEnabled() && UserNameValid())
                log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, traceLevel, message, exception);
        }

        public static void StackFormat(this ILog log, string message, params object[] args)
        {
            if (IsStackEnabled() && UserNameValid())
            {
                string formattedMessage = string.Format(message, args);
                log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, traceLevel, formattedMessage, null);
            }
        }

        private static void GetCoreParams(ref StackLog stackLog)
        {
            //pilha de execucao ate chamar o log
            //http://stackoverflow.com/questions/628565/display-lines-number-in-stack-trace-for-net-assembly-in-release-mode
            StackTrace stackTrace = new StackTrace(true);
            stackLog.Stacktrace = stackTrace;
            //nome do metodo que chamou o log
            stackLog.Method = stackTrace.GetFrame(2).GetMethod().Name;
            //nome do namespace e da classe que chamou o log
            stackLog.Namespace = stackTrace.GetFrame(2).GetMethod().DeclaringType.FullName;
            //controller que chamou o log
            stackLog.Controller = IsWebApplication ? HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString() : string.Empty;
            //action que chamou o log
            stackLog.Action = IsWebApplication ? HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString() : string.Empty;
            //acao http chamada no log (GET, PUT, POST, DELETE, etc)
            stackLog.HttpAction = IsWebApplication ? HttpContext.Current.Request.HttpMethod : string.Empty;
            //httpcontext root
            stackLog.HttpContext = IsWebApplication ? HttpContext.Current : null;
        }

        public static void Stack(this ILog log, string message, object[] objectsToLog, Exception exception = null)
        {
            if (IsStackEnabled() && UserNameValid())
            {
                var stackLog = new StackLog();
                stackLog.DateTime = DateTime.Now;
                stackLog.Message = message;
                stackLog.Exception = exception;
                //Get Stack, Method, Namespace, Controller, Action, HttpAction and HttpContext
                GetCoreParams(ref stackLog);
                //usuario autenticado no site
                stackLog.User = GetUser();
                //server variables
                stackLog.ServerVariables = LogServerVariables();
                //dados enviados para o log
                stackLog.UserObjects = objectsToLog;

                //poderia ser usado
                //var auth_user = HttpContext.Current.User.Identity.IsAuthenticated ? HttpContext.Current.User.Identity.Name : "anonymous";
                //var x = System.Reflection.MethodBase.GetCurrentMethod().Name;
                //var h = new JavaScriptSerializer().Serialize(context);

                string formattedMessage = string.Format("{0}", stackLog);
                log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, traceLevel, formattedMessage, null);
            }
        }
    }
}
