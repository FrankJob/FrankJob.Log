using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net;

namespace FrankJob.Log.Test
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BornDate { get; set; }
    }

    [TestClass]
    public class StackLogTest
    {
        private static readonly ILog StackLog = CustomLogManager.GetLogger("StackLog");
        private static readonly ILog AppLog = LogManager.GetLogger("RootLog");
        private static readonly string StackLogFile = "C:\\Users\\franc\\Documents\\visual studio 2015\\Projects\\FrankJob.Log\\FrankJob.Log.Test\\Log\\Log4NetStack.log";

        [TestMethod]
        public void Create()
        {
            log4net.Config.XmlConfigurator.Configure();
            var p = new Person { Name = "Beth", Age = 41, BornDate = DateTime.Now };
            StackLog.Stack("Stack!", new object[] { p }, new NotImplementedException("exception message"));
            Assert.IsTrue(System.IO.File.Exists(StackLogFile));
            Assert.IsTrue(new System.IO.FileInfo(StackLogFile).Length > 0);
        }
    }
}
