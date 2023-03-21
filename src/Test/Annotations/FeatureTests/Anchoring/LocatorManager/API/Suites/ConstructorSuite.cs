// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVT test case for the LocatorManager constructor.


using System;

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
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;


namespace Avalon.Test.Annotations.Suites
{
    public class Constructor_BVT : AAnchoringAPITests
    {
        /// <summary>
        /// BVT test for the LocatorManager constructor.  Tests that no exceptions get
        /// thrown while trying to create a new LocatorManager
        /// </summary>
        [Keywords("Setup_SanitySuite")]
        [Priority(0)]
        protected void constructor()
        {
            LocatorManager manager = null;

            try
            {
                manager = new LocatorManager();
            }
            catch (Exception exp)
            {
                failTest("Exception thrown in constructor: " + exp.ToString());
            }
            finally
            {
                if (manager != null)
                    passTest("Constructor test");
            }
        }
    }
}

