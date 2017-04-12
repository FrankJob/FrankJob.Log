using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FrankJob.Log
{
    public static class UserConfiguration
    {
        //"FrankJob.Log.EnableStackLog"
        public static bool EnableStackLog
        {
            get
            {
                try{
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["FrankJob.Log.EnableStackLog"]);
                }
                catch(Exception ex) {
                    return false;
                }
            }
        }

        //"FrankJob.Log.ShowDefaultPropertiesValue"
        public static DefaultValueHandling ShowDefaultPropertiesValue
        {
            get
            {
                try
                {
                    var show = Convert.ToBoolean(ConfigurationManager.AppSettings["FrankJob.Log.ShowDefaultPropertiesValue"]);
                    return show ? DefaultValueHandling.Include : DefaultValueHandling.Ignore;
                }
                catch (Exception ex)
                {
                    return DefaultValueHandling.Ignore;
                }
            }
        }

        public static NullValueHandling ShowNullPropertiesValue
        {
            get
            {
                try
                {
                    var show = Convert.ToBoolean(ConfigurationManager.AppSettings["FrankJob.Log.ShowDefaultPropertiesValue"]);
                    return show ? NullValueHandling.Include : NullValueHandling.Ignore;
                }
                catch (Exception ex)
                {
                    return NullValueHandling.Ignore;
                }
            }
        }

        //"FrankJob.Log.JsonIndented"
        public static Formatting JsonIndented
        {
            get
            {
                try
                {
                    var indented = Convert.ToBoolean(ConfigurationManager.AppSettings["FrankJob.Log.JsonIndented"]);
                    return indented ? Formatting.Indented : Formatting.None;
                }
                catch (Exception ex)
                {
                    return Formatting.Indented;
                }
            }
        }

        //"FrankJob.Log.UsersToMonitor"
        public static List<string> UsersToMonitor
        {
            get
            {
                try{
                    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["FrankJob.Log.UsersToMonitor"]))
                        return new List<string>();
                    var config = ConfigurationManager.AppSettings["FrankJob.Log.UsersToMonitor"].ToLower().Split(';');                    
                    var users = new List<string>();
                    foreach (var item in config)
                        users.Add(item.Trim());
                    return users;
                }
                catch (Exception ex) {
                    return new List<string>();
                }
            }
        }

        //"FrankJob.Log.EnableAllSettingsInUsersToMonitor"
        public static bool EnableAllSettingsInUsersToMonitor
        {
            get
            {
                try {                    
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["FrankJob.Log.EnableAllSettingsInUsersToMonitor"]);
                }
                catch (Exception ex) {
                    return false;
                }
            }
        }

        //"FrankJob.Log.EnableHttpContext"
        public static bool EnableHttpContext
        {
            get
            {
                if (UsersToMonitor.Count > 0 && EnableAllSettingsInUsersToMonitor) return true;

                try {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["FrankJob.Log.EnableHttpContext"]);
                }
                catch (Exception ex) {
                    return false;
                }                
            }
        }

        //"FrankJob.Log.HttpContextIgnore"
        public static List<string> HttpContextPropertiesIgnore
        {
            get
            {
                try {
                    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["FrankJob.Log.HttpContextPropertiesIgnore"]))
                        return new List<string>();
                    var config = ConfigurationManager.AppSettings["FrankJob.Log.HttpContextPropertiesIgnore"].Split(';');
                    var ignoredProps = new List<string>();
                    foreach (var item in config)
                        ignoredProps.Add(item.Trim());
                    return ignoredProps.OrderBy(p => p).ToList();
                }
                catch (Exception ex) {
                    return new List<string>();
                }
            }
        }

        //"FrankJob.Log.EnableStackTrace"
        public static bool EnableStackTrace
        {
            get
            {
                if (UsersToMonitor.Count > 0 && EnableAllSettingsInUsersToMonitor) return true;

                try
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["FrankJob.Log.EnableStackTrace"]);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        //"FrankJob.Log.StackTraceWithoutNumbers"
        public static bool StackTraceWithoutNumbers
        {
            get
            {
                try {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["FrankJob.Log.StackTraceWithoutNumbers"]);
                }
                catch (Exception ex) {
                    return false;
                }
            }
        }

        //"FrankJob.Log.EnableServerVariables"
        public static bool EnableServerVariables
        {
            get
            {
                if (UsersToMonitor.Count > 0 && EnableAllSettingsInUsersToMonitor) return true;

                try
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["FrankJob.Log.EnableServerVariables"]);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

    }
}
