using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azuro.ActiveDirectory;
using Azuro.Common.Security;
using System.Security;

namespace Unit.Test.EnterpriseLibrary
{
	[TestClass]
	public class ADHelperTests
	{
		//	Sample Extension Data Driven Test - http://blog.drorhelper.com/2011/09/enabling-parameterized-tests-in-mstest.html
		//[TestMethod, RowTest]
		//[Row(12, 3, 4)]
		//[Row(12, 2, 6)]
		//[Row(12, 4, 3)]
		//public void TestWithData(int one, int two, int three)
		//{
		//}

		//	Sample Data Driven Test - http://callumhibbert.blogspot.com/2009/07/data-driven-tests-with-mstest.html
		//[TestMethod]
		//[DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "MyWidgetTests.csv", "MyWidgetTests#csv", DataAccessMethod.Sequential)]
		//public void TestMyBusinessLogicWithCsv()
		//{
		//	int valueA = Convert.ToInt32(TestContext.DataRow["valueA"]);
		//	int valueB = Convert.ToInt32(TestContext.DataRow["valueB"]);
		//	int expectedResult = Convert.ToInt32(TestContext.DataRow["expectedResult"]);
		//	int actualResult = MyWidget.MyBusinessLogic(valueA, valueB);
		//	Assert.AreEqual(expectedResult, actualResult, "The result returned from the widget was not as expected.");
		//}

		[TestMethod]
		public void TestAuthenticate()
		{
			ADHelper adh = new ADHelper();

			//	Force password failure - Not a good test, but didn't want to leave pwd in TFS.
			Assert.AreEqual<bool>(false, adh.Authenticate("johannu", "failpwd"));
		}
	}
}
