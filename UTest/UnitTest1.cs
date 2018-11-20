using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace UTest
{
	[TestClass]
	public class TaskManager
	{
		ChromeDriver _driver = new ChromeDriver();
		~TaskManager()
		{
			_driver.Dispose();
		}
		public TaskManager()
		{
			_driver.Manage().Window.Maximize();
			Login();
		}
		public void Login()
		{
			_driver.Navigate().GoToUrl("http://localhost:8311/login.aspx");
			_driver.FindElement(By.Id("ctl00_MainContent_usr")).SendKeys("utest");
			_driver.FindElement(By.Id("ctl00_MainContent_pwd")).SendKeys("utest");
			_driver.FindElement(By.XPath("//input[@value='Login']")).Click();
		}
		public void Wait()
		{
			int maxtotalwaits = 20;
			int totalwaits = 0;
			while (true)
			{
				totalwaits++;
				Thread.Sleep(1000);
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
		[TestMethod]
		public void TestDailySearch()
		{
			_driver.Navigate().GoToUrl("http://localhost:8311/dailysearch.aspx?filter={%22startdate%22:%222017-12-31T21:00:00.000Z%22,%22enddate%22:%222018-01-04T21:00:00.000Z%22,%22userid%22:%221%22}");
			Wait();
			int countexp = 3;
			int count = _driver.FindElements(By.CssSelector(".report-day")).Count;
			if (count != countexp)
			{
				Assert.Fail("Daily search results: Expected - {0}, Actual - {1}", countexp, count);
			}
		}
	}
}
