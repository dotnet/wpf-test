// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVT test cases for the ModifyDataId suite.  Checks that
//  attached annotations cannot be found if the DataId of any node in the
//  locator is changed.


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

namespace Avalon.Test.Annotations.Suites
{
    public class ModifyDataIdSuite_BVT : AModifyDataIdSuite
    {
        /// <summary>
        /// Modify DataID property on element at leaf of the tree
        /// After b2's Name was changed, we shouldn't find Annotation for b2 since locators won't resolve
        /// </summary>
        [Priority(1)]
        private void modifydataid2()
        {
            queueTask(ChangeLeafId, null);
        }

        /// <summary>
        /// Checks the number of attached annotations on the tree, then changes the DataId
        /// of a leaf node.
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private object ChangeLeafId(object obj)
        {
            base.AttachedAnnotationsBefore(obj);
            DataIdProcessor.SetDataId(b2, "b2_modify");
            queueTask(VerifyAnnotationsLoaded, new object[] { 2, 1 });
            return null;
        }

        /// <summary>
        /// Modify DataID property on element at root of the tree
        /// After root's Name was changed, we shouldn't find Annotations for b1, b2 since locators won't resolve
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void modifydataid1()
        {
            queueTask(ChangeRootId_DiffName, null);
        }

        /// <summary>
        /// Modify DataID property on element at root of the tree, but the new value equals the old one
        /// After root's Name was changed, we should find Annotations for b1, b2
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void modifydataid3()
        {
            queueTask(ChangeRootId_SameName, null);
        }

        /// <summary>
        /// Checks the number of attached annotations on the tree, then changes the DataId
        /// of a root node to the same name it had before.
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private object ChangeRootId_SameName(object obj)
        {
            base.AttachedAnnotationsBefore(obj);
            DataIdProcessor.SetDataId(a1, "a1");
            queueTask(VerifyAnnotationsLoaded, new object[] { 2, 2 });
            return null;
        }

        /// <summary>
        /// Checks the number of attached annotations on the tree, then changes the DataId
        /// of a root node to the same name it had before.
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private object ChangeRootId_DiffName(object obj)
        {
            base.AttachedAnnotationsBefore(obj);
            DataIdProcessor.SetDataId(a1, "a1_modify");
            queueTask(VerifyAnnotationsLoaded, new object[] { 2, 0 });
            return null;
        }
    }
}

