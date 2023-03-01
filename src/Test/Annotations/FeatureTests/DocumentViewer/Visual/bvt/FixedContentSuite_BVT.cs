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

namespace Avalon.Test.Annotations.BVTs
{
	public class FixedContentSuite_BVT : AFixedContentSuite
	{
		#region Tests

        /// <summary>
        /// Create anchor that includes an empty page.
        /// </summary>
        protected void fixedcontent1()
        {
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Middle, 0, 2, PagePosition.Middle, 0));
            VerifyAnnotation(GetText(new MultiPageSelectionData(0, PagePosition.Middle, 0, 0, PagePosition.End, 0)));
            PageDown();
            VerifyNumberOfAttachedAnnotations(0);
            PageDown();
            VerifyAnnotation(GetText(new MultiPageSelectionData(2, PagePosition.Beginning, 0, 2, PagePosition.Middle, 0)));
            passTest("Verified creating annotation that includes an empty page.");
        }
        /// <summary>
        /// Create annotation that starts on empty page.
        /// </summary>
        protected void fixedcontent3()
        {            
            CreateAnnotation(new MultiPageSelectionData(1, PagePosition.Beginning, 0, 2, PagePosition.Middle, 0));
            VerifyNumberOfAttachedAnnotations(0);
            PageDown();
            VerifyNumberOfAttachedAnnotations(0);
            PageDown();
            VerifyAnnotation(GetText(new MultiPageSelectionData(2, PagePosition.Beginning, 0, 2, PagePosition.Middle, 0)));
            passTest("Verified creating annotation starting on empty page.");
        }

        /// <summary>
        /// Create annotation that ends on empty page.
        /// </summary>
        protected void fixedcontent4()
        {
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Middle, 0, 1, PagePosition.End, 0));
            VerifyAnnotation(GetText(new MultiPageSelectionData(0, PagePosition.Middle, 0, 0, PagePosition.End, 0)));
            PageDown();
            VerifyNumberOfAttachedAnnotations(0);
            PageDown();
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Verified creating annotation ending on empty page.");
        }

		#endregion Tests
	}
}	

