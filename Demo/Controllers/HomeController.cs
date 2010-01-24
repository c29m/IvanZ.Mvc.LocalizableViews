using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IvanZ.Mvc.LocalizableViews;
using System.Threading;

namespace Demo.Controllers
{
	[HandleError]
	[Localizable] // Either an attribute or
	public class HomeController : Controller /* : LocalizableController - or this*/
	{
		public ActionResult LocalizationDemo ()
		{
			return View ();
		}

		public ActionResult About ()
		{
			return View ();
		}

		[OutputCache (VaryByCustom = LocalizationConfig.VaryByCustomKey, VaryByParam = "none", Duration = 9999999)]
		public ActionResult CachingDemo ()
		{
			return View ();
		}

		[AcceptVerbs (HttpVerbs.Post)]
		public ActionResult SwitchCulture (string culture, string returnAction)
		{
			LocalizationConfig.SwitchCulture (this.HttpContext, culture);

			// NOTE: Note the use of Redirect here instead of a View.
			// Because the culture is detected *before* action execution
			// a refresh is needed *after* this action is executed in order
			// for the page to render in the selected culture.
			//
			// Using a Redirect instead of a View basically achieves that, because
			// it ends up executing the specified action after this one.

			// Do not ever do this in real code! The url has to be sanitized!!!
			return Redirect (returnAction);
		}

		public ActionResult RoutingDemo ()
		{
			return View ();
		}

		public ActionResult ClearCookiesAndGoToRoutingDemo ()
		{
			// We need to remove the culture cookie because they have higher precedence 
			// than routing parameters.
			this.HttpContext.Response.Cookies.Set (new HttpCookie (LocalizationConfig.CookieName, String.Empty) { Expires = DateTime.Now.AddDays (-7) });
			return Redirect("RoutingDemo");
		}
	}
}
