// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.Windows.Controls;

using Annotation = System.Windows.Annotations.Annotation;
using ContentLocatorBase = System.Windows.Annotations.ContentLocatorBase;
using ContentLocatorPart = System.Windows.Annotations.ContentLocatorPart;
using ContentLocator = System.Windows.Annotations.ContentLocator;
using ContentLocatorGroup = System.Windows.Annotations.ContentLocatorGroup;
using AnnotationResource = System.Windows.Annotations.AnnotationResource;
using AnnotationResourceChangedEventArgs = System.Windows.Annotations.AnnotationResourceChangedEventArgs;
using AnnotationAuthorChangedEventArgs = System.Windows.Annotations.AnnotationAuthorChangedEventArgs;
using AnnotationResourceChangedEventHandler = System.Windows.Annotations.AnnotationResourceChangedEventHandler;
using AnnotationAuthorChangedEventHandler = System.Windows.Annotations.AnnotationAuthorChangedEventHandler;
using AnnotationAction = System.Windows.Annotations.AnnotationAction;

using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;					// TestSuite.


/// <summary>
/// Provides the implementation for BVTs for the FixedPageProxy object
/// </summary>
namespace Avalon.Test.Annotations.Suites
{
	public class FixedPageProxySuite : AFixedPageProcessorSuite
    {
        #region BVT TESTS

        #region Constructor Tests

        /// <summary>
        /// Paramters: manager = valid, non-null LocatorManager 	
        /// Verify: No exceptions thrown.
        /// </summary>
        [Priority(0)]
        private void fixedpageproxy_ctor3()
        {
            try
            {
                FixedPageProxy proxy = new FixedPageProxy(ViewerBase, 0);
                AssertNotNull("Verify object was initialized.", proxy);
                AssertEquals("Verify page number.", 0, proxy.Page);
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception:  " + exp.ToString());
            }
            passTest("Proxy created, Page 0.");
        }

        #endregion Constructor Tests

        #endregion BVT TESTS

        #region PRIORITY TESTS

        #region Constructor Tests

        [Priority(1)]
        private void VerifyFixedPageProxy(FixedPageProxy fpp, int page)
        {
            AssertNotNull("FixedPageProxy should not be null.", fpp);
            AssertEquals("Check page.", page, fpp.Page);
        }

        /// <summary>
        /// Parameters: parent = null
        ///				page = valid page number 	
        /// Verify: No exceptions thrown. 
        ///			FixedPageProxy.Page = page
        /// </summary>
        [Priority(1)]
        private void fixedpageproxy_ctor1()
        {
            FixedPageProxy fpp = new FixedPageProxy(null, 0);
            VerifyFixedPageProxy(fpp, 0);
            passTest("Verified: page = valid page number.");
        }


        /// <summary>
        /// Parameters: parent = valid DependencyObject
        ///				page = -1	
        /// Verify: No exceptions thrown
        ///			FixedPageProxy.Page = -1
        /// </summary>
        [Priority(1)]
        private void fixedpageproxy_ctor2()
        {
            FixedPageProxy fpp = new FixedPageProxy(null, -1);
            VerifyFixedPageProxy(fpp, -1);
            passTest("Verified: page = -1.");
        }


        /// <summary>
        /// Parameters: parent = valid DependencyObject
        ///				0 < page <= (total pageCount � 1)	
        /// Verify: No exceptions thrown
        ///			FixedPageProxy.Page = page
        /// </summary>
        [Priority(1)]
        private void fixedpageproxy_ctor4()
        {
            FixedPageProxy fpp = new FixedPageProxy(ViewerBase, 3);
            VerifyFixedPageProxy(fpp, 3);
            passTest("Verified: 0 < page <= (total pageCount � 1).");
        }


        /// <summary>
        /// Parameters: parent = valid DependencyObject
        ///				page > totalPageCount	
        /// Verify: No exceptions thrown
        ///			FixedPageProxy.Page = page
        /// </summary>
        [Priority(1)]
        private void fixedpageproxy_ctor5()
        {
            FixedPageProxy fpp = new FixedPageProxy(null, 400);
            VerifyFixedPageProxy(fpp, 400);
            passTest("Verified: page > totalPageCount.");
        }

        #endregion Constructor Tests

        #endregion PRIORITY TESTS
    }
}

