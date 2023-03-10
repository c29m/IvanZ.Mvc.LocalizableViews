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
 * ***************************************************************************/

namespace IvanZ.Mvc.LocalizableViews {
    using System.Collections;
    using System.Web.Compilation;
	using System;

    internal sealed class BuildManagerWrapper : IBuildManager {
        #region IBuildManager Members
        object IBuildManager.CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType) {
            return BuildManager.CreateInstanceFromVirtualPath(virtualPath, requiredBaseType);
        }

        ICollection IBuildManager.GetReferencedAssemblies() {
            return BuildManager.GetReferencedAssemblies();
        }
        #endregion
    }
}
