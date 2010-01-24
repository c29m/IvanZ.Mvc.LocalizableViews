using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using System.Threading;
using System.Globalization;

namespace IvanZ.Mvc.LocalizableViews.Tests
{

	[TestClass]
	public class LocalizationTests
	{
		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[ClassInitialize]
		public static void TestFixtureSetUp (TestContext testContext)
		{
			LocalizationConfig.EnableLocalization ();
			LocalizationConfig.SupportedCultures = new string[] { "bg", "en-US", "de" };
			LocalizationConfig.FallbackCulture = "en";
		}

		[TestMethod]
		public void Detect_Culture_From_Cookie ()
		{
			HttpContextBase httpContext = MockHelper.CreateHttpContext ();
			httpContext.Request.Cookies.Set (new HttpCookie (LocalizationConfig.CookieName, "bg"));
			Assert.AreEqual (LocalizationConfig.GetCultureFromName ("bg"), LocalizationConfig.GetRequestCulture (httpContext.Request, null));
		}

		[TestMethod]
		public void Detect_Culture_Cookie_Has_Highest_Priority ()
		{
			HttpContextBase httpContext = MockHelper.CreateHttpContext ();
			httpContext.Request.Cookies.Set (new HttpCookie (LocalizationConfig.CookieName, "bg"));

			RouteData routeData = new RouteData ();
			routeData.Values.Add (LocalizationConfig.RouteParameterName, "en");

			httpContext.Request.Headers.Add ("Accept-Language", "de");

			Assert.AreEqual (LocalizationConfig.GetCultureFromName ("bg"), LocalizationConfig.GetRequestCulture (httpContext.Request, null));
		}

		[TestMethod]
		public void Detect_Culture_From_RouteData ()
		{
			HttpContextBase httpContext = MockHelper.CreateHttpContext ();
			RouteData routeData = new RouteData ();
			routeData.Values.Add (LocalizationConfig.RouteParameterName, "bg");
			Assert.AreEqual (LocalizationConfig.GetCultureFromName ("bg"), LocalizationConfig.GetRequestCulture (httpContext.Request, routeData));
		}

		[TestMethod]
		public void Detect_Culture_RouteData_Has_Higher_Priority_Than_HttpHeader ()
		{
			HttpContextBase httpContext = MockHelper.CreateHttpContext ();

			RouteData routeData = new RouteData ();
			routeData.Values.Add (LocalizationConfig.RouteParameterName, "en");

			httpContext.Request.Headers.Add ("Accept-Language", "de");

			Assert.AreEqual (LocalizationConfig.GetCultureFromName ("en"), LocalizationConfig.GetRequestCulture (httpContext.Request, routeData));
		}

		[TestMethod]
		public void Detect_Culture_From_HttpHeader ()
		{
			HttpContextBase httpContext = MockHelper.CreateHttpContext ();
			httpContext.Request.Headers.Add ("Accept-Language", " bg, en-gb;q=0.8, en;q=0.7");
			Assert.AreEqual (LocalizationConfig.GetCultureFromName ("bg"), LocalizationConfig.GetRequestCulture (httpContext.Request, null));
		}

		[TestMethod]
		public void Detect_Fallback()
		{
			HttpContextBase httpContext = MockHelper.CreateHttpContext ();
			httpContext.Request.Cookies.Set (new HttpCookie (LocalizationConfig.CookieName, "not-supported"));
			Assert.AreEqual (LocalizationConfig.GetCultureFromName (LocalizationConfig.FallbackCulture), 
                                         LocalizationConfig.GetRequestCulture (httpContext.Request, null));
		}

		[TestMethod]
		public void SwitchCulture()
		{
			HttpContextBase httpContext = MockHelper.CreateHttpContext ();

			LocalizationConfig.SwitchCulture (httpContext, "bg");
			HttpCookie cookie = httpContext.Response.Cookies.Get (LocalizationConfig.CookieName);
			Assert.IsNotNull (cookie);
			Assert.AreEqual ("bg", cookie.Value);

			LocalizationConfig.SwitchCulture (httpContext, "en");
			cookie = httpContext.Response.Cookies.Get (LocalizationConfig.CookieName);
			Assert.IsNotNull (cookie);
			Assert.AreEqual ("en", cookie.Value);
		}
	}
}
