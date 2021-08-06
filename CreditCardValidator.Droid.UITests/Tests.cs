using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;
using Newtonsoft.Json;

namespace CreditCardValidator.Droid.UITests
{
    [TestFixture]
    public class Tests
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp.Android.StartApp();
        }

        [Test]
        public void CreditCardNumber_TooLong_DisplayErrorMessage()
        {
            // Invoke the REPL so that we can explore the user interface
            //app.Repl();
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Enter Credit Card Number"));
            app.EnterText(c => c.Marked("creditCardNumberText"), new string('9', 17));
            app.Tap(c => c.Marked("validateButton"));

            app.WaitForElement(c => c.Marked("errorMessagesText").Text("Credit card number is too long."));
        }

        [Test]
        public void CreditCardNumber_TooShort_DisplayErrorMessage()
        {
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Enter Credit Card Number"));
            app.EnterText(c => c.Marked("creditCardNumberText"), "123456");
            app.Tap(c => c.Marked("validateButton"));

            app.WaitForElement(c => c.Marked("errorMessagesText").Text("Credit card number is too short."));
        }

        [Test]
        public void CreditCardNumber_Success()
        {
            // Invoke the REPL so that we can explore the user interface
            //app.Repl();
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Enter Credit Card Number"));
            app.EnterText(c => c.Marked("creditCardNumberText"), new string('7', 16));
            app.Tap(c => c.Marked("validateButton"));

            app.WaitForElement(c => c.Marked("validationSuccessMessage"));

            string data = (string)app.Invoke("MyBackdoorMethod");
            string[] results = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            TestResult result = JsonConvert.DeserializeObject<TestResult>(results[2]);

            bool isTrue;
            bool parsed = bool.TryParse(result.Result, out isTrue);
            Assert.DoesNotThrow(delegate { Assert.True(isTrue); });

            app.ScrollDown();
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
        public void ClickingButtonTwiceShouldChangeItsLabel()
        {
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Enter Credit Card Number"));

            Func<AppQuery, AppQuery> MyButton = c => c.Button("validateButton");

            app.Tap(MyButton);
            app.Tap(MyButton);
            AppResult[] results = app.Query(MyButton);
            app.Screenshot("Button clicked twice.");

            Assert.AreEqual("2 clicks", results[0].Text);
        }

        [Test]
        public void InvokeTest()
        {
            // Wait for the Activity to load
            app.WaitForElement(c => c.Marked("action_bar_title").Text("Enter Credit Card Number"));

            // Invoke the backdoor method MainActivity.MyBackDoorMethod
            app.Invoke("MyBackdoorMethod");
        }
    }

    public class TestResult
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("outcome")]
        public string Outcome { get; set; }
    }
}

