# FrankJob.Log

FrankJob.Log is a library that provides a new log level for [log4net](https://logging.apache.org/log4net/), where it's possible to write the application execution stack (stacktrace), the HttpContext object, Exceptions, execution information as the Method that was called or the Controller/Action and still write entire objects of your application to check the values passed by the user.
Through additional options in the application configuration file, it is possible to determine which objects are written and which user will have the logged log.
With the flexibility of [log4net](https://logging.apache.org/log4net/), it is possible to persist logging information from text files to database.
This information is serialized using [Newtonsoft Json.NET](https://github.com/JamesNK/Newtonsoft.Json) in json format.

## Code Example
Configure your log4net environment and add a new log appender like this:

    <appender name="StackLogAppender" type="log4net.Appender.RollingFileAppender">
      <file value="C:\\Log\\Log4NetTrace.log" />
      <appendToFile value="true" />
      <rollingStyle value="Composite" />
      <datePattern value="yyyyMMdd" />
      <maxSizeRollBackups value="10" />
      <maximumFileSize value="1MB" />
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%-5p %d %5rms %-22.22c{1} %-18.18M - %m%n" />
      </layout>
    </appender>
    
And add a new logger like this:
    
    <logger name="StackLog">
      <level value="STACK"/>
      <appender-ref ref="StackLogAppender" />
    </logger>

Customize your log content with AppSettings on Web.Config:

      <appSettings>
        <!-- enable stack log -->
        <add key="FrankJob.Log.EnableStackLog" value="false" />
        <!-- show all properties fields of object -->
        <add key="FrankJob.Log.ShowDefaultPropertiesValue" value="false" />
        <!-- Indent json log -->
        <add key="FrankJob.Log.JsonIndented" value="true" />
        <!-- put domain\user1; domain\user2 format to monitor a user. No value monitor all users -->
        <add key="FrankJob.Log.UsersToMonitor" value="" />
        <!-- override the configuration when monitor a specific user and enable all log properties fields -->
        <add key="FrankJob.Log.EnableAllSettingsInUsersToMonitor" value="false" />
        <!-- enable log of HttpContext object -->
        <add key="FrankJob.Log.EnableHttpContext" value="false" />
        <!-- ignore parts of HttpContext object like Cache; Current -->
        <add key="FrankJob.Log.HttpContextPropertiesIgnore" value="" />
        <!-- enable stacktrace of log execution -->
        <add key="FrankJob.Log.EnableStackTrace" value="false" />
        <!-- log stacktrace with file columns and rows or all stack -->
        <add key="FrankJob.Log.StackTraceWithoutNumbers" value="false" />
        <!-- enable Server Variables log -->
        <add key="FrankJob.Log.EnableServerVariables" value="false" />
      </appSettings>

Using a custom log creator to return an ILog interface from log4net in your page or global.asax like this:

    public static readonly ILog LogStack = CustomLogManager.GetLogger("StackLog");

And consuming from any method as listed above:

    MvcApplication.LogStack.Stack("Stack!?");
    MvcApplication.LogStack.Stack("No Exception", new object[] { resultado });
    MvcApplication.LogStack.Stack("With Exception",new object[]{ resultado }, new NotImplementedException("New Error! LoL"));
    
The result is a Log with a json content of objects and enviroment variables:

    STACK 2017-04-11 20:13:06,138 - {
      "DateTime": {
        "Date": "2017-4-11",
        "Time": "20:13:4:192",
        "DST": false
      },
      "Message": "Trace!",
      "Method": "Main",
      "Namespace": "LogWithoutHttp.Program",
      "User": "lenovo-jr\\franc",
      "UserObjects": {
        "Object[]": [
          {
            "name": "Beth",
            "age": 41,
            "bornDate": "2017-04-11T20:13:04.1039911-03:00"
          }
        ]
      },
      "Exception": {
        "ClassName": "System.NotImplementedException",
        "Message": "erro exemplo",
        "Data": null,
        "InnerException": null,
        "HelpURL": null,
        "StackTraceString": null,
        "RemoteStackTraceString": null,
        "RemoteStackIndex": 0,
        "ExceptionMethod": null,
        "HResult": -2147467263,
        "Source": null,
        "WatsonBuckets": null
      }
    }

## Motivation

Several companies I've worked on have problems logging their logs efficiently into a bug situation with the user.
The motivation is to facilitate the adoption of logs by all in an efficient way.

## Installation

Well all you basically have to do is install the [FrankJob.Log](https://www.nuget.org/packages/FrankJob.Log) NuGet package:

`PM> Install-Package FrankJob.Log`

For more information about usage, please view Code Example part.

## Tests

I've to improve that part! Sorry! :(

## Contributors

Highly welcome! Just fork away and send a pull request.

This library would be NOTHING without its [contributors](https://github.com/FrankJob/FrankJob.Log/graphs/contributors) - thanks so much!!

## Copyright

Copyright © 2017 Francisco C. de S. Junior & [Contributors](https://github.com/FrankJob/FrankJob.Log/graphs/contributors)

## License

FrankJob.Log is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to [LICENSE.md](https://github.com/FrankJob/FrankJob.Log/blob/master/LICENSE.md) for more information.

