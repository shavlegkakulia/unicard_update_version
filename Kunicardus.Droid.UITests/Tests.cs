using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Utils;

namespace Kunicardus.Droid.UITests
{
    [TestFixture]
    public class Tests
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            string currentFile = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            FileInfo fi = new FileInfo(currentFile);
            string dir = fi.Directory.Parent.Parent.Parent.FullName;
            
            string PathToAPK = Path.Combine(dir, "Kunicardus.Droid", "bin", "Debug", "ge.unicard.unicardmobileapp.APK");
            
            app = ConfigureApp.Android.ApkFile(PathToAPK).DeviceSerial("192.168.190.101:5555").StartApp();
            //app = ConfigureApp.Android.StartApp();
        }

        [Test]
        public void Repl()
        {

            //app.WaitForElement(c => c.Marked("button1"));
            //app.Tap(x => x.Marked("button1"));
            //
            //app.WaitForElement(x => x.Marked("authorization"));

            app.Repl();
        }

        [Test]
        public void AuthorizationValidationWrong()
        {
            app.WaitForElement(c => c.Marked("button1"));
            app.Tap(x => x.Marked("button1"));

            app.WaitForElement(x => x.Marked("authorization"));

            app.EnterText(c => c.Marked("login_user_name"), "Username");
            app.EnterText(c => c.Marked("txtPassword"), "Password");

            app.Tap(x => x.Marked("authorization"));
        }
    }

    public class WaitTimes : IWaitTimes
    {
        public TimeSpan GestureWaitTimeout
        {
            get { return TimeSpan.FromMinutes(4); }
        }
        public TimeSpan WaitForTimeout
        {
            get { return TimeSpan.FromMinutes(4); }
        }
    }
}
