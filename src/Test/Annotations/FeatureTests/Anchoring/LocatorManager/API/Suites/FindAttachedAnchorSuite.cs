// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1 test cases for the FindAttachedAnchor test suite.


using System;
using System.Windows;
using System.Windows.Controls;

using System.Collections;

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
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    public class FindAttachedAnchorSuite_Pri1 : AFindAttachedAnchorSuite
    {
        #region globals
        // All tests use these variables to check the resolved anchor and attachmentLevel
        AttachmentLevel attachmentLevel;
        Object result = null;
        #endregion

        /// <summary>
        /// Null startNode (prefixes should not be generated)
        /// </summary>
        [TestId("findattachedanchor2.49")]
        [Priority(1)]
        protected void findattachedanchor2_49()
        {
            try
            {
                result = FindAttachedAnchor(null, d, out attachmentLevel);
                failTest("ArgumentNullException should have been thrown");
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception e)
            {
                failTest("ArgumentNullException should have been thrown, not " + e.ToString());
            }
        }

        /// <summary>
        /// Null locator
        /// </summary>
        [TestId("findattachedanchor2.50")]
        [Priority(1)]
        protected void findattachedanchor2_50()
        {
            try
            {
                result = FindAttachedAnchor_NullLocatorArg(d, out attachmentLevel);
                failTest("ArgumentNullException should have been thrown");
            }
            //catch (ArgumentNullException tie)
            catch (ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception e)
            {
                failTest("ArgumentNullException should have been thrown, not " + e.ToString());
            }
        }

        /// <summary>
        /// Start node is a descendant of the target node
        /// </summary>
        [Priority(1)]
        protected void findattachedanchor1()
        {
            result = FindAttachedAnchor(txt_h, d, out attachmentLevel);
            VerifyAnchorIsUnresolved(result, attachmentLevel);
        }

        /// <summary>
        /// Start node is a sibling of the target node  
        /// </summary>
        [Priority(1)]
        protected void findattachedanchor2()
        {
            result = FindAttachedAnchor(e, d, out attachmentLevel);
            VerifyAnchorIsUnresolved(result, attachmentLevel);
        }

        /// <summary>
        /// Start node is the immediate parent of the target node
        /// </summary>
        [Priority(1)]
        protected void findattachedanchor3()
        {
            result = FindAttachedAnchor(b, d, out attachmentLevel);
            VerifyAnchorIsFullyResolved(result, d, attachmentLevel);
        }

        /// <summary>
        /// Start node is an ancestor of the target node
        /// </summary>
        [Priority(1)]
        protected void findattachedanchor4()
        {
            result = FindAttachedAnchor(a, d, out attachmentLevel);
            VerifyAnchorIsFullyResolved(result, d, attachmentLevel);
        }

        /// <summary>
        /// Start node has no relationship to the target node
        /// </summary>
        [Priority(1)]
        protected void findattachedanchor5()
        {
            result = FindAttachedAnchor(f, d, out attachmentLevel);
            VerifyAnchorIsUnresolved(result, attachmentLevel);
        }

        /// <summary>
        /// Start node and target node are the same object
        /// </summary>
        [Priority(1)]
        protected void findattachedanchor6()
        {
            result = FindAttachedAnchor(d, d, out attachmentLevel);
            VerifyAnchorIsFullyResolved(result, d, attachmentLevel);
        }
    }
}

