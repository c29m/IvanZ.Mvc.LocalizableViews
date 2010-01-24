using IvanZ.Mvc.LocalizableViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;

namespace IvanZ.Mvc.LocalizableViews.Tests
{
	[TestClass]
	public class LocalizableWebFormViewEngineTest
	{
		public class TestController : LocalizableController
		{
		}

		public class TestView : IView
		{
			
			public TestView(string viewPath)
			{
				Path = viewPath;
			}

			public string Path { get; private set; }

			#region IView Members

			public void Render (ViewContext viewContext, System.IO.TextWriter writer)
			{
				throw new System.NotImplementedException ();
			}

			#endregion
		}

		public class TestLocalizableWebFormViewEngine : LocalizableWebFormViewEngine
		{
			public string[] AvailableViews { get; set; }


			protected override bool FileExists (ControllerContext controllerContext, string virtualPath)
			{
				if (AvailableViews != null)
					return AvailableViews.Contains (virtualPath);
				return false;
			}

			protected override IView CreateView (ControllerContext controllerContext, string viewPath, string masterPath)
			{
				return new TestView (viewPath);
			}

			protected override IView CreatePartialView (ControllerContext controllerContext, string partialPath)
			{
				return new TestView (partialPath);
			}
		}

		[ClassInitialize]
		public static void TestFixtureSetUp (TestContext testContext)
		{
			LocalizationConfig.SupportedCultures = new string[] { "bg", "en-US", "de" };
			LocalizationConfig.FallbackCulture = "en";
		}

		public TestLocalizableWebFormViewEngine ViewEngine { get; private set; }
		public ControllerContext ControllerContext { get; private set; }

		[TestInitialize]
		public void SetUp ()
		{
			LocalizableWebFormViewEngine.UseTwoLetterISOCultureName = false;
                        ViewEngine = new TestLocalizableWebFormViewEngine ();
			ControllerContext = MockHelper.CreateControllerContext (new TestController (), null, null, null);
		}

		[TestMethod]
		public void FindView_Not_Localized_Fallback ()
		{
			LocalizationConfig.SwitchCulture (ControllerContext.HttpContext, "de");

			ViewEngine.AvailableViews = new string[] {
				"~/Views/TestController/Test.aspx"
			};

			TestView view = (TestView)ViewEngine.FindView (ControllerContext, "Test", null, false).View;

			Assert.AreEqual ("~/Views/TestController/Test.aspx", view.Path);
		}

		[TestMethod]
		public void FindView_Localized_TwoLetter_Not_In_Subdirectory ()
		{
			LocalizationConfig.SwitchCulture (ControllerContext.HttpContext, "de");

			LocalizableWebFormViewEngine.UseTwoLetterISOCultureName = true;
			ViewEngine.AvailableViews = new string[] {
				"~/Views/TestController/Test.aspx",
				"~/Views/TestController/Test.de.aspx",
				"~/Views/TestController/Test.bg.aspx"
			};

			TestView view = (TestView)ViewEngine.FindView (ControllerContext, "Test", null, false).View;

			Assert.AreEqual ("~/Views/TestController/Test.de.aspx", view.Path);
		}

		[TestMethod]
		public void FindView_Localized_TwoLetter_In_Subdirectory ()
		{
			LocalizationConfig.SwitchCulture (ControllerContext.HttpContext, "de");

			LocalizableWebFormViewEngine.UseTwoLetterISOCultureName = true;
			ViewEngine.AvailableViews = new string[] {
				"~/Views/TestController/Test.aspx/Test.aspx",
				"~/Views/TestController/Test.aspx/Test.de.aspx",
				"~/Views/TestController/Test.aspx/Test.bg.aspx"
			};

			TestView view = (TestView)ViewEngine.FindView (ControllerContext, "Test", null, false).View;

			Assert.AreEqual ("~/Views/TestController/Test.aspx/Test.de.aspx", view.Path);
		}

		[TestMethod]
		public void FindView_Localized_Not_In_Subdirectory ()
		{
			LocalizationConfig.SwitchCulture (ControllerContext.HttpContext, "de");

			ViewEngine.AvailableViews = new string[] {
				"~/Views/TestController/Test.aspx",
				"~/Views/TestController/Test.de-DE.aspx",
				"~/Views/TestController/Test.de.aspx",
				"~/Views/TestController/Test.bg-BG.aspx"
			};

			TestView view = (TestView)ViewEngine.FindView (ControllerContext, "Test", null, false).View;

			Assert.AreEqual ("~/Views/TestController/Test.de-DE.aspx", view.Path);
		}


		[TestMethod]
		public void FindView_Localized_In_Subdirectory ()
		{
			LocalizationConfig.SwitchCulture (ControllerContext.HttpContext, "de");

			ViewEngine.AvailableViews = new string[] {
				"~/Views/TestController/Test.aspx/Test.aspx",
				"~/Views/TestController/Test.aspx/Test.de-DE.aspx",
				"~/Views/TestController/Test.aspx/Test.de.aspx",
				"~/Views/TestController/Test.aspx/Test.bg-BG.aspx",
				"~/Views/TestController/Test.aspx",
				"~/Views/TestController/Test.de-DE.aspx",
				"~/Views/TestController/Test.de.aspx",
				"~/Views/TestController/Test.bg-BG.aspx"
			};

			TestView view = (TestView)ViewEngine.FindView (ControllerContext, "Test", null, false).View;

			Assert.AreEqual ("~/Views/TestController/Test.aspx/Test.de-DE.aspx", view.Path);
		}

		[TestMethod]
		public void FindView_Localized_In_Subdirectory_Fallback ()
		{
			LocalizationConfig.SwitchCulture (ControllerContext.HttpContext, "en");

			ViewEngine.AvailableViews = new string[] {
				"~/Views/TestController/Test.aspx/Test.aspx",
				"~/Views/TestController/Test.aspx/Test.de-DE.aspx",
				"~/Views/TestController/Test.aspx/Test.de.aspx",
				"~/Views/TestController/Test.aspx/Test.bg-BG.aspx",
			};

			TestView view = (TestView)ViewEngine.FindView (ControllerContext, "Test", null, false).View;

			Assert.AreEqual ("~/Views/TestController/Test.aspx/Test.aspx", view.Path);
		}
	}
}
