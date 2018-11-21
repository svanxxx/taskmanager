using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace UTest
{
	[TestClass]
	public class TaskManager
	{
		ChromeDriver _driver;
		[TestCleanup]
		public void Cleanup()
		{
			_driver.Dispose();
			_driver = null;
		}
		[TestInitialize]
		public void Initialize()
		{
			_driver = new ChromeDriver();
			_driver.Manage().Window.Maximize();
			Login();
		}
		public void Login()
		{
			GoToUrl("login.aspx");
			_driver.FindElement(By.Id("ctl00_MainContent_usr")).SendKeys("utest");
			_driver.FindElement(By.Id("ctl00_MainContent_pwd")).SendKeys("utest");
			_driver.FindElement(By.XPath("//input[@value='Login']")).Click();
		}
		public void GoToUrl(string page)
		{
			_driver.Navigate().GoToUrl("http://localhost:8311/" + page);
		}
		public void Wait()
		{
			int maxtotalwaits = 20;
			int totalwaits = 0;
			while (true)
			{
				totalwaits++;
				Thread.Sleep(500);
				if (_driver.FindElements(By.CssSelector(".loadingprogress")).Count == 0)
				{
					break;
				}
				if (totalwaits > maxtotalwaits)
				{
					Assert.Fail("Failed to load data in {0} seconds", maxtotalwaits);
				}
			}
		}
		public void TestElements(string selector, int expected)
		{
			Wait();
			int count = _driver.FindElements(By.CssSelector(selector)).Count;
			if (count != expected)
			{
				Assert.Fail("Failed to find elements: Expected - {0}, Actual - {1}", expected, count);
			}
		}
		public void TestElementVisible(string selector, bool expected)
		{
			Wait();
			bool vis = _driver.FindElement(By.CssSelector(selector)).Displayed;
			if (vis != expected)
			{
				Assert.Fail("Failed to test element ({2}) visibility: Expected - {0}, Actual - {1}", expected, vis, selector);
			}
		}
		public void TestTextValue(string selector, string contains)
		{
			Wait();
			var val = _driver.FindElement(By.CssSelector(selector)).GetAttribute("value");
			if (!val.Contains(contains))
			{
				Assert.Fail("Failed to test element ({2}) text: Expected - {0}, Actual - {1}", contains, val, selector);
			}
		}
		public void Click(string selector)
		{
			Wait();
			_driver.FindElement(By.CssSelector(selector)).Click();
		}
		[TestMethod]
		public void TestDailySearch()
		{
			GoToUrl("dailysearch.aspx?filter={%22startdate%22:%222017-12-31T21:00:00.000Z%22,%22enddate%22:%222018-01-04T21:00:00.000Z%22,%22userid%22:%221%22}");
			TestElements(".report-day", 3);
		}
		[TestMethod]
		public void TestBranches()
		{
			GoToUrl("branches.aspx");
			TestElements(".item-branch", 15);
		}
		[TestMethod]
		public void TestTask()
		{
			GoToUrl("showtask.aspx?ttid=560");
			TestElementVisible(".savebutton", false);

			Click("a[href='#specification']");
			TestTextValue("#spec", "#specloaded");

			Click("a[href='#detail']");
			TestTextValue("#Description", "#dataloaded");

			Click("a[href='#bst']");
			TestElements("#batches0>div>a", 14);

			Click("a[href='#batches']");
			TestTextValue("#bstbatches", "#batchloaded");

			Click("a[href='#commands']");
			TestTextValue("#bstcommands", "#commandloaded");
		}
	}
}