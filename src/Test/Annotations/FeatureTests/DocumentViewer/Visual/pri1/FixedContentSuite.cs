// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Collections.Generic;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations.Pri1s
{
	public class FixedContentSuite : AFixedContentSuite
	{
		#region Tests

        /// <summary>
        /// Create annotation on empty page.
        /// </summary>
        protected void fixedcontent2()
        {            
            VerifyCreateAnnotationFails(new MultiPageSelectionData(1, PagePosition.Beginning, 0, 1, PagePosition.End, 0), typeof(InvalidOperationException));
            passTest("Verified can't create annotation on empty page.");
        }

		#endregion Tests
	}
}	

