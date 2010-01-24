using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Web;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections;

namespace IvanZ.Mvc.LocalizableViews.Tests
{
	public static class MockHelper
	{
		public static HttpContextBase CreateHttpContext ()
		{
			var context = new Mock<HttpContextBase> ();
			var request = new Mock<HttpRequestBase> ();
			var response = new Mock<HttpResponseBase> ();
			var session = new Mock<HttpSessionStateBase> ();
			var server = new Mock<HttpServerUtilityBase> ();

			context.Setup (ctx => ctx.Request).Returns (request.Object);
			context.Setup (ctx => ctx.Response).Returns (response.Object);
			context.Setup (ctx => ctx.Session).Returns (session.Object);
			context.Setup (ctx => ctx.Server).Returns (server.Object);

			var querystring = new NameValueCollection ();
			var user = new GenericPrincipal (new GenericIdentity ("testuser"),
					    new string[] { "Administrator" });

			request.Setup (r => r.Cookies).Returns (new HttpCookieCollection ());
			request.Setup (r => r.Form).Returns (new NameValueCollection ());
			request.Setup (r => r.Headers).Returns (new NameValueCollection ());
			request.Setup (q => q.QueryString).Returns (querystring);
			response.Setup (r => r.Cookies).Returns (new HttpCookieCollection ());
			context.Setup (u => u.User).Returns (user);

			return context.Object;
		}

		public static ControllerContext CreateControllerContext (ControllerBase controller, Dictionary<string, string> formValues, 
									 HttpCookieCollection requestCookies, IDictionary<string, string> routeData)
		{
			HttpContextBase httpContext = CreateHttpContext ();

			if (formValues != null) {
				foreach (string key in formValues.Keys)
					httpContext.Request.Form.Add (key, formValues[key]);
			}

			if (requestCookies != null) {
				foreach (string key in requestCookies.Keys)
					httpContext.Request.Cookies.Add (requestCookies[key]);
			}

			RouteData route = new RouteData ();
			route.Values.Add ("controller", controller.GetType ().Name);

			if (routeData != null) {
				foreach (var valuePair in routeData)
					route.Values.Add (valuePair.Key, valuePair.Value);
			}

			return new ControllerContext (new RequestContext (httpContext, route), controller);
		}
	}
}
