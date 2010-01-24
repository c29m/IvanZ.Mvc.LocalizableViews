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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc.Resources;
using IvanZ.Mvc.LocalizableViews.Resources;

namespace IvanZ.Mvc.LocalizableViews
{
	public abstract class VirtualPathProviderViewEngine : IViewEngine
	{
		// format is ":ViewCacheEntry:{cacheType}:{CurrentUICulture}:{prefix}:{name}:{controllerName}:"
		private const string _cacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:";
		private const string _cacheKeyPrefix_Master = "Master";
		private const string _cacheKeyPrefix_Partial = "Partial";
		private const string _cacheKeyPrefix_View = "View";
		private static readonly string[] _emptyLocations = new string[0];

		private VirtualPathProvider _vpp;

		[SuppressMessage ("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public string[] MasterLocationFormats
		{
			get;
			set;
		}

		[SuppressMessage ("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public string[] LocalizedMasterLocationFormats
		{
			get;
			set;
		}

		[SuppressMessage ("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public string[] PartialViewLocationFormats
		{
			get;
			set;
		}

		[SuppressMessage ("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public string[] LocalizedPartialViewLocationFormats
		{
			get;
			set;
		}

		public IViewLocationCache ViewLocationCache
		{
			get;
			set;
		}

		[SuppressMessage ("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public string[] ViewLocationFormats
		{
			get;
			set;
		}

		[SuppressMessage ("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public string[] LocalizedViewLocationFormats
		{
			get;
			set;
		}

		protected VirtualPathProvider VirtualPathProvider
		{
			get
			{
				if (_vpp == null) {
					_vpp = HostingEnvironment.VirtualPathProvider;
				}
				return _vpp;
			}
			set
			{
				_vpp = value;
			}
		}

		protected VirtualPathProviderViewEngine ()
		{
			if (HttpContext.Current == null || HttpContext.Current.IsDebuggingEnabled) {
				ViewLocationCache = DefaultViewLocationCache.Null;
			} else {
				ViewLocationCache = new DefaultViewLocationCache ();
			}
		}

		private string CreateCacheKey (string prefix, string name, string culture, string controllerName)
		{
			return String.Format (CultureInfo.InvariantCulture, _cacheKeyFormat,
			    GetType ().AssemblyQualifiedName, culture, prefix, name, controllerName);
		}

		protected abstract IView CreatePartialView (ControllerContext controllerContext, string partialPath);

		protected abstract IView CreateView (ControllerContext controllerContext, string viewPath, string masterPath);

		protected virtual bool FileExists (ControllerContext controllerContext, string virtualPath)
		{
			return VirtualPathProvider.FileExists (virtualPath);
		}

		public virtual ViewEngineResult FindPartialView (ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			if (controllerContext == null) {
				throw new ArgumentNullException ("controllerContext");
			}
			if (String.IsNullOrEmpty (partialViewName)) {
				throw new ArgumentException (MvcResources.Common_NullOrEmpty, "partialViewName");
			}

			string[] localizedSearched = new string[0];
			string[] searched = new string[0];
			string controllerName = controllerContext.RouteData.GetRequiredString ("controller");
			string currentCulture = GetCurrentCulture (controllerContext);

			string partialPath = GetPath (controllerContext, LocalizedPartialViewLocationFormats, "LocalizedPartialViewLocationFormats", 
                                                      partialViewName, currentCulture, controllerName, _cacheKeyPrefix_Partial, 
                                                      useCache, out localizedSearched);
			if (String.IsNullOrEmpty (partialPath))
				partialPath = GetPath (controllerContext, PartialViewLocationFormats, "PartialViewLocationFormats",
						      partialViewName, String.Empty, controllerName, _cacheKeyPrefix_Partial,
						      useCache, out searched);

			if (String.IsNullOrEmpty (partialPath)) {
				return new ViewEngineResult (localizedSearched.Union (searched));
			}

			return new ViewEngineResult (CreatePartialView (controllerContext, partialPath), this);
		}

		public virtual ViewEngineResult FindView (ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			if (controllerContext == null) {
				throw new ArgumentNullException ("controllerContext");
			}
			if (String.IsNullOrEmpty (viewName)) {
				throw new ArgumentException (MvcResources.Common_NullOrEmpty, "viewName");
			}

			string[] localizedViewLocationsSearched = new string[0];
			string[] localizedMasterLocationsSearched = new string[0];
			string[] masterLocationsSearched = new string[0];
			string[] viewLocationsSearched = new string[0];

			string controllerName = controllerContext.RouteData.GetRequiredString ("controller");
			string currentCulture = GetCurrentCulture (controllerContext);

			string viewPath = GetPath (controllerContext, LocalizedViewLocationFormats, "LocalizedViewLocationFormats",
						   viewName, currentCulture, controllerName, _cacheKeyPrefix_View, 
                                                   useCache, out localizedViewLocationsSearched);
			if (String.IsNullOrEmpty (viewPath))
				viewPath = GetPath (controllerContext, ViewLocationFormats, "ViewLocationFormats", 
                                                   viewName, String.Empty, controllerName, _cacheKeyPrefix_View, 
                                                   useCache, out viewLocationsSearched);

			string masterPath = GetPath (controllerContext, LocalizedMasterLocationFormats, "LocalizedMasterLocationFormats",
						     masterName, currentCulture, controllerName, _cacheKeyPrefix_Master,
						     useCache, out localizedMasterLocationsSearched);
			if (String.IsNullOrEmpty (masterPath))
				masterPath = GetPath (controllerContext, MasterLocationFormats, "MasterLocationFormats", 
						     masterName, String.Empty, controllerName, _cacheKeyPrefix_Master, 
						     useCache, out masterLocationsSearched);

			if (String.IsNullOrEmpty (viewPath) || (String.IsNullOrEmpty (masterPath) && !String.IsNullOrEmpty (masterName))) {
				return new ViewEngineResult (localizedViewLocationsSearched
								.Union (viewLocationsSearched)
								.Union (masterLocationsSearched)
								.Union (localizedMasterLocationsSearched));
			}

			return new ViewEngineResult (CreateView (controllerContext, viewPath, masterPath), this);
		}

		private string GetPath (ControllerContext controllerContext, string[] locations, string locationsPropertyName, string name, string culture, string controllerName, string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
		{
			searchedLocations = _emptyLocations;

			if (String.IsNullOrEmpty (name)) {
				return String.Empty;
			}

			if (locations == null || locations.Length == 0) {
				throw new InvalidOperationException (String.Format (CultureInfo.CurrentUICulture,
				    MvcResources.Common_PropertyCannotBeNullOrEmpty, locationsPropertyName));
			}

			bool nameRepresentsPath = IsSpecificPath (name);
			string cacheKey = CreateCacheKey (cacheKeyPrefix, name, culture, (nameRepresentsPath) ? String.Empty : controllerName);

			if (useCache) {
				string result = ViewLocationCache.GetViewLocation (controllerContext.HttpContext, cacheKey);
				if (result != null) {
					return result;
				}
			}

			return (nameRepresentsPath) ?
			    GetPathFromSpecificName (controllerContext, name, cacheKey, ref searchedLocations) :
			    GetPathFromGeneralName (controllerContext, locations, name, culture, controllerName, cacheKey, ref searchedLocations);
		}

		private string GetPathFromGeneralName (ControllerContext controllerContext, string[] locations, string name, string culture, 
							string controllerName, string cacheKey, ref string[] searchedLocations)
		{
			string result = String.Empty;
			searchedLocations = new string[locations.Length];

			for (int i = 0; i < locations.Length; i++) {
				string virtualPath = String.Format (CultureInfo.InvariantCulture, locations[i], name, controllerName, culture);

				if (FileExists (controllerContext, virtualPath)) {
					result = virtualPath;
					searchedLocations = _emptyLocations;
					ViewLocationCache.InsertViewLocation (controllerContext.HttpContext, cacheKey, result);
					break;
				}

				searchedLocations[i] = virtualPath;
			}

			return result;
		}

		private string GetPathFromSpecificName (ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
		{
			string result = name;

			if (!FileExists (controllerContext, name)) {
				result = String.Empty;
				searchedLocations = new[] { name };
			}

			ViewLocationCache.InsertViewLocation (controllerContext.HttpContext, cacheKey, result);
			return result;
		}

		private static bool IsSpecificPath (string name)
		{
			char c = name[0];
			return (c == '~' || c == '/');
		}

		public virtual void ReleaseView (ControllerContext controllerContext, IView view)
		{
			IDisposable disposable = view as IDisposable;
			if (disposable != null) {
				disposable.Dispose ();
			}
		}

		protected abstract string GetCurrentCulture (ControllerContext controllerContext);
	}
}