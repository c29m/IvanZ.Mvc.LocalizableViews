/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 *
 * This software is subject to the Microsoft Public License (Ms-PL). 
 * A copy of the license can be found in the license.htm file included 
 * in this distribution.
 *
 * You must not remove this notice, or any other, from this software.
 * 
 * Support for view-per-language localization by: Ivan Zlatev <ivan@ivanz.com>
 *
 * ***************************************************************************/

using System.Web.Mvc;
using System.Web.Compilation;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Net;
using System.Threading;
using System.Globalization;
using System;

namespace IvanZ.Mvc.LocalizableViews
{

	public class LocalizableWebFormViewEngine : VirtualPathProviderViewEngine
	{

		private IBuildManager _buildManager;

		public LocalizableWebFormViewEngine ()
		{
			MasterLocationFormats = new[] {
				"~/Views/{1}/{0}.master",
				"~/Views/{1}/{0}.master/{0}.master",
				"~/Views/Shared/{0}.master",
				"~/Views/Shared/{0}.master/{0}.master",
			};

			LocalizedMasterLocationFormats = new[] {
				"~/Views/{1}/{0}.master/{0}.{2}.master",
				"~/Views/{1}/{0}.{2}.master",
				"~/Views/Shared/{0}.master/{0}.{2}.master",
				"~/Views/Shared/{0}.{2}.master"
			};

			ViewLocationFormats = new[] {
				"~/Views/{1}/{0}.aspx",
				"~/Views/{1}/{0}.aspx/{0}.aspx",
				"~/Views/{1}/{0}.ascx",
				"~/Views/{1}/{0}.ascx/{0}.ascx",
				"~/Views/Shared/{0}.aspx",
				"~/Views/Shared/{0}.aspx/{0}.aspx",
				"~/Views/Shared/{0}.ascx",
				"~/Views/Shared/{0}.ascx/{0}.ascx",
			};

			LocalizedViewLocationFormats = new[] {
				"~/Views/{1}/{0}.aspx/{0}.{2}.aspx",
				"~/Views/{1}/{0}.{2}.aspx",
				"~/Views/{1}/{0}.ascx/{0}.{2}.ascx",
				"~/Views/{1}/{0}.{2}.ascx",
				"~/Views/Shared/{0}.aspx/{0}.{2}.aspx",
				"~/Views/Shared/{0}.{2}.aspx",
				"~/Views/Shared/{0}.ascx/{0}.{2}.ascx",
				"~/Views/Shared/{0}.{2}.ascx",
			};

			PartialViewLocationFormats = ViewLocationFormats;
			LocalizedPartialViewLocationFormats = LocalizedViewLocationFormats;
		}

		private IBuildManager BuildManager
		{
			get
			{
				if (_buildManager == null) {
					_buildManager = new BuildManagerWrapper ();
				}
				return _buildManager;
			}
			set
			{
				_buildManager = value;
			}
		}

		protected override IView CreatePartialView (ControllerContext controllerContext, string partialPath)
		{
			return new WebFormView (partialPath, null);
		}

		protected override IView CreateView (ControllerContext controllerContext, string viewPath, string masterPath)
		{
			return new WebFormView (viewPath, masterPath);
		}

		[SuppressMessage ("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
		    Justification = "Exceptions are interpreted as indicating that the file does not exist.")]
		protected override bool FileExists (ControllerContext controllerContext, string virtualPath)
		{
			try {
				object viewInstance = BuildManager.CreateInstanceFromVirtualPath (virtualPath, typeof (object));

				return viewInstance != null;
			} catch (HttpException he) {
				if (he.GetHttpCode () == (int)HttpStatusCode.NotFound) {
					// If BuildManager returns a 404 (Not Found) that means the file did not exist
					return false;
				} else {
					// All other error codes imply other errors such as compilation or parsing errors
					throw;
				}
			} catch {
				return false;
			}
		}

		protected override string GetCurrentCulture (ControllerContext controllerContext)
		{
			if (UseTwoLetterISOCultureName)
				return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			return Thread.CurrentThread.CurrentUICulture.Name;
		}

		public static bool UseTwoLetterISOCultureName { get; set; }
	}
}
