using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;

namespace IvanZ.Mvc.LocalizableViews
{
	public class SupportedCultureRouteConstraint : IRouteConstraint
	{
		private string _routeParameterName;

		/// <summary>
		/// Uses LocalizableController.RouteParameterName for the route parameter name
		/// </summary>
		public SupportedCultureRouteConstraint () : this (null)
		{
		}

                /// <summary>
		/// 
		/// </summary>
		/// <param name="routeParameterName">The name of the route parameter to check.</param>
		public SupportedCultureRouteConstraint (string routeParameterName)
		{
			_routeParameterName = routeParameterName;
		}

		public string RouteParameterName {
			get { return _routeParameterName ?? LocalizationConfig.RouteParameterName; }
		}

		public bool Match (HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			if (String.Compare (parameterName, RouteParameterName, StringComparison.InvariantCultureIgnoreCase) == 0) {
				object culture = null;
				if (values.TryGetValue (parameterName, out culture) && culture is string)
					return LocalizationConfig.SupportsCulture ((string)culture);
			}

			return false;
		}
	}
}
