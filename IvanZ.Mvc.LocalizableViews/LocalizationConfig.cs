using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using System.Globalization;
using System.Web.Mvc;
using System.Threading;

namespace IvanZ.Mvc.LocalizableViews
{
	public static class LocalizationConfig
	{
		static LocalizationConfig ()
		{
			SupportedCultures = new string[0];
			FallbackCulture = CultureInfo.InvariantCulture.Name;
			CookieName = "culture";
			RouteParameterName = "culture";
		}

		public const string VaryByCustomKey = "culture";
		/// <summary>
		/// An array of the supported cultures. For example: "fr", "bg", "en-US", etc.
		/// The default is an empty array.
		/// </summary>
		public static string[] SupportedCultures { get; set; }

		/// <summary>
		/// The default fallback culture. The default is the Invariant culture.
		/// </summary>
		public static string FallbackCulture { get; set; }

		/// <summary>
		/// The name of the cookie file to store the current culture name. The default to "language".
		/// </summary>
		public static string CookieName { get; set; }

		/// <summary>
		/// The route parameter name which contains the culture name. The default is "culture"
		/// </summary>
		public static string RouteParameterName { get; set; }

		public static bool UseTwoLetterISOCultureNameForViews {
			get { return LocalizableWebFormViewEngine.UseTwoLetterISOCultureName; }
			set { LocalizableWebFormViewEngine.UseTwoLetterISOCultureName = value; }
		}

		private static Regex _acceptLanuageStripQExpression = new Regex (@";q=[0-9.\s]*", RegexOptions.CultureInvariant |
												RegexOptions.Compiled |
												RegexOptions.IgnoreCase);

		/// <summary>
		/// Tries to detect the current culture by checking in the following order: cookie, RouteData, Accept-Language HTTP Header.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="routeData"></param>
		/// <returns>Returns the detected culture if supported else the fallback culture.
		/// If the fallback culture isn't supported either this method will return the invariant culture.</returns>
		public static CultureInfo GetRequestCulture (HttpRequestBase request, RouteData routeData)
		{
			// Try 1: Cookie
			HttpCookie languageCookie = request.Cookies[CookieName];
			if (languageCookie != null) {
				string cultureName = languageCookie.Value;
				if (SupportsCulture (cultureName))
					return GetCultureFromName (cultureName);
			}

			// Try 2: Route Parameter
			if (routeData != null && routeData.Values.ContainsKey (RouteParameterName)) {
				string cultureName = (string)routeData.Values[RouteParameterName];
				if (SupportsCulture (cultureName))
					return GetCultureFromName (cultureName);
			}

			// Try 3: "Accept-Language" HTTP Header
			if (!String.IsNullOrEmpty (request.Headers["Accept-Language"])) {
				string languagesHeader = request.Headers["Accept-Language"];
				// does not support q=x.xx ranking
				languagesHeader = _acceptLanuageStripQExpression.Replace (languagesHeader, String.Empty);
				string[] supportedLanguages = languagesHeader.Split (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string supportedLanguage in supportedLanguages) {
					string trimmedSupportedLanguage = supportedLanguage.Trim ();
					if (SupportsCulture (trimmedSupportedLanguage)) {
						string cultureName = trimmedSupportedLanguage;
						return GetCultureFromName (cultureName);
					}
				}
			}

			if (SupportsCulture (FallbackCulture)) // it could be that we don't support the specified Fallback culture
				return GetCultureFromName (FallbackCulture);
			else
				return CultureInfo.InvariantCulture;
		}

		/// <summary>
		/// Returns a non-neutral <see cref="CultureInfo"/> for the specified culture name. For example if you pass
		/// "en" it will return a CultureInfo for "en-US".
		/// </summary>
		/// <param name="cultureName"></param>
		/// <returns></returns>
		public static CultureInfo GetCultureFromName (string cultureName)
		{
			CultureInfo culture = CultureInfo.GetCultureInfo (cultureName);
			if (culture.IsNeutralCulture)
				culture = CultureInfo.CreateSpecificCulture (cultureName);
			return culture;
		}

		public static bool SupportsCulture (string cultureName)
		{
			try {
				// "normalize"
				cultureName = GetCultureFromName (cultureName).Name;
			} catch {
				return false;
			}

			foreach (string supportedCulture in SupportedCultures) {
				if (String.Compare (cultureName, 
                                                    GetCultureFromName (supportedCulture).Name, 
                                                    StringComparison.InvariantCultureIgnoreCase) == 0)
					return true;
			}

			return false;
		}

		/// <summary>
		/// This method registers the localization view engine with ASP.NET MVC.
		/// </summary>
		public static void EnableLocalization ()
		{
			ViewEngines.Engines.Clear ();
			ViewEngines.Engines.Add (new LocalizableWebFormViewEngine ());
		}

		public static string GetVaryByCustomString (HttpContext context, string arg)
		{
			if (context == null)
				throw new ArgumentNullException ("context", "context is null.");

			return GetRequestCulture (new HttpRequestWrapper (context.Request),
						  RouteTable.Routes.GetRouteData (new HttpContextWrapper (context))).Name;
		}

		/// <summary>
		/// Switches the culture for the next request by adding/settings a culture cookie.
		/// </summary>
		/// <param name="context">The current HttpContext</param>
		/// <param name="culture">The culture to switch to. This must be one of the <see cref="SupportedCultures"/>.</param>
		public static void SwitchCulture (HttpContextBase context, string culture)
		{
			if (context == null)
				throw new ArgumentNullException ("context", "context is null.");
			if (String.IsNullOrEmpty (culture))
				throw new ArgumentException ("culture is null or empty.", "culture");
			if (!LocalizationConfig.SupportsCulture (culture))
				throw new ArgumentException ("This culture is not part of the specified SupportedCultures.");

			context.Response.Cookies.Set (new HttpCookie (CookieName, culture));
			CultureInfo cultureInfo = GetCultureFromName (culture);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			Thread.CurrentThread.CurrentCulture = cultureInfo;
		}
	}
}
