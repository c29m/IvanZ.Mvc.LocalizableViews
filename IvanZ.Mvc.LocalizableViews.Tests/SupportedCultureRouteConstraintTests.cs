using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Routing;

namespace IvanZ.Mvc.LocalizableViews.Tests
{
	/// <summary>
	/// Summary description for SupportedCultureRouteConstraintTests
	/// </summary>
	[TestClass]
	public class SupportedCultureRouteConstraintTests
	{

		[ClassInitialize]
		public static void TestFixtureSetUp (TestContext testContext)
		{
			LocalizationConfig.EnableLocalization ();
			LocalizationConfig.SupportedCultures = new string[] { "bg", "en-US", "de" };
			LocalizationConfig.FallbackCulture = "en";
		}

		[TestMethod]
		public void Match_Success ()
		{
			SupportedCultureRouteConstraint constraint = new SupportedCultureRouteConstraint ();

			HttpContextBase httpContext = MockHelper.CreateHttpContext ();

			RouteValueDictionary routeValues = new RouteValueDictionary ();
			routeValues.Add (LocalizationConfig.RouteParameterName, "de");
			Assert.IsTrue (constraint.Match (httpContext, 
                                                         null, 
                                                         LocalizationConfig.RouteParameterName, 
                                                         routeValues, 
                                                         RouteDirection.IncomingRequest));
		}


		[TestMethod]
		public void Match_Fail_Culture_Not_Supported ()
		{
			SupportedCultureRouteConstraint constraint = new SupportedCultureRouteConstraint ();

			HttpContextBase httpContext = MockHelper.CreateHttpContext ();

			RouteValueDictionary routeValues = new RouteValueDictionary ();
			routeValues.Add (LocalizationConfig.RouteParameterName, "not-supported");
			Assert.IsFalse (constraint.Match (httpContext,
							 null,
							 LocalizationConfig.RouteParameterName,
							 routeValues,
							 RouteDirection.IncomingRequest));
		}

		[TestMethod]
		public void Match_Fail_No_Route_Parameter ()
		{
			SupportedCultureRouteConstraint constraint = new SupportedCultureRouteConstraint ();

			HttpContextBase httpContext = MockHelper.CreateHttpContext ();

			Assert.IsFalse (constraint.Match (httpContext,
							 null,
							 LocalizationConfig.RouteParameterName,
							 new RouteValueDictionary (),
							 RouteDirection.IncomingRequest));
		}
	}
}
