using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IvanZ.Mvc.LocalizableViews;

namespace Demo
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes (RouteCollection routes)
		{
			routes.IgnoreRoute ("{resource}.axd/{*pathInfo}");

			routes.MapRoute (
			    "RoutingDemo",                                              // Route name
			    "{culture}/Home/RoutingDemo",                           // URL with parameters
			    new { controller = "Home", 
				  action = "RoutingDemo", 
				  culture = LocalizationConfig.FallbackCulture },  // Parameter defaults
			    new { culture = new SupportedCultureRouteConstraint () }
			);

			routes.MapRoute (
			    "Default",                                              // Route name
			    "{controller}/{action}/{id}",                           // URL with parameters
			    new { controller = "Home", action = "LocalizationDemo", id = "" }  // Parameter defaults
			);

		}

		protected void Application_Start ()
		{
			SetupLocalization ();
			RegisterRoutes (RouteTable.Routes);
		}

		private void SetupLocalization ()
		{
			LocalizationConfig.EnableLocalization ();
			LocalizationConfig.SupportedCultures = new string[] { "bg", "en-US" };
			LocalizationConfig.FallbackCulture = "en";
			// Search for View.en.aspx instead of View.en-US.aspx
			LocalizationConfig.UseTwoLetterISOCultureNameForViews = true;
		}

		public override string GetVaryByCustomString (HttpContext context, string custom)
		{
			if (String.Compare (custom, LocalizationConfig.VaryByCustomKey, StringComparison.InvariantCultureIgnoreCase) == 0)
				return LocalizationConfig.GetVaryByCustomString (context, custom);

			return base.GetVaryByCustomString (context, custom);
		}
	}
}