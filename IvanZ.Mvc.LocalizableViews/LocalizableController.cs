/* ****************************************************************************
 *
 * Copyright (c) Ivan Zlatev <ivan@ivanz.com>. All rights reserved.
 *
 * This software is subject to the Microsoft Public License (Ms-PL). 
 * A copy of the license can be found in the license.htm file included 
 * in this distribution.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Linq;
using System.Text.RegularExpressions;

namespace IvanZ.Mvc.LocalizableViews
{

	/// <summary>
	/// Derive to this controller 
	/// </summary>
	public class LocalizableController : Controller
	{

		protected LocalizableController ()
		{
		}

		protected override void OnActionExecuting (ActionExecutingContext filterContext)
		{
			CultureInfo culture = LocalizationConfig.GetRequestCulture (filterContext.HttpContext.Request, filterContext.RouteData);
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;

			base.OnActionExecuting (filterContext);
		}
	}
}
