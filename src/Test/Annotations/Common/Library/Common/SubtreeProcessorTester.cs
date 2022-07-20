// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Basic test functionality for implementator of MS.Internal.Annotations.Anchoring.SubtreeProcessor.

using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Reflection;

using Annotations.Test.Framework;


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

using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;
using System.Windows.Media;
using System.Xml;
using Annotations.Test.Reflection;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Module that provides default support for testing SubtreeProcessor apis.
	/// May be extended to provide implementation specific verification.
	/// </summary>
	public class SubtreeProcessorTester
	{
        public SubtreeProcessorTester(TestSuite testSuite, SubTreeProcessor subTreeProcessor)
        {
            suite = testSuite;
            processor = subTreeProcessor;
        }

        #region PreProcessNode

        /// <summary>
        /// Verify that expected number of attached annoations are returned and that the out parameter is correct.
        /// </summary>
        /// <param name="expectedNumAttachedAnnotations">-1 if return value should be null.</param>
        public virtual void VerifyPreProcessNode(DependencyObject node, bool expectedCalledProcessAnnotations, int expectedNumAttachedAnnotations) 
        {
            bool calledProcessAnnotations;
            IList<IAttachedAnnotation> result = processor.PreProcessNode(node, out calledProcessAnnotations);

            if (expectedNumAttachedAnnotations == -1)
                suite.AssertNull("Result should be null.", result);
            else
                suite.AssertEquals("Verify number of AttachedAnnotations.", expectedNumAttachedAnnotations, result.Count);

            suite.AssertEquals("Verify calledProcessAnnotations.", expectedCalledProcessAnnotations, calledProcessAnnotations);
        }
        public virtual void VerifyPreProcessNodeFails(DependencyObject node, Type expectedExceptionType) 
        {
            bool exception = false;
            try
            {
                bool calledProcessAnnotations;
                processor.PreProcessNode(node, out calledProcessAnnotations);
            }
            catch (Exception e)
            {
                exception = true;
                suite.AssertEquals("Verify exception type.", expectedExceptionType, e.GetType());
            }
            suite.Assert("Expected exception to have occurred.", exception);
        }

        #endregion

        #region PostProcessNode

        /// <summary>
        /// Verify that expected number of attached annoations are returned and that the out parameter is correct.
        /// </summary>        
        /// <param name="expectedNumAttachedAnnotations">-1 if return value should be null.</param>
        public virtual void VerifyPostProcessNode(DependencyObject node, bool childrenCalledProcessAnnotations, bool expectedCalledProcessAnnotations, int expectedNumAttachedAnnotations) 
        {
            bool calledProcessAnnotations;
            IList<IAttachedAnnotation> result = processor.PostProcessNode(node, childrenCalledProcessAnnotations, out calledProcessAnnotations);
            
            if (expectedNumAttachedAnnotations == -1)
                suite.AssertNull("Result should be null.", result);
            else
                suite.AssertEquals("Verify number of AttachedAnnotations.", expectedNumAttachedAnnotations, result.Count);

            suite.AssertEquals("Verify calledProcessAnnotations.", expectedCalledProcessAnnotations, calledProcessAnnotations);
        }
        public virtual void VerifyPostProcessNodeFails(DependencyObject node, bool childrenCalledProcessAnnotations, Type expectedExceptionType) 
        {
            bool exception = false;
            try
            {
                bool calledProcessAnnotations;
                processor.PostProcessNode(node, childrenCalledProcessAnnotations, out calledProcessAnnotations);
            }
            catch (Exception e)
            {
                exception = true;
                suite.AssertEquals("Verify exception type.", expectedExceptionType, e.GetType());
            }
            suite.Assert("Expected exception to have occurred.", exception);
        }

        #endregion

        #region GenerateLocator

        /// <summary>
        /// Verify that:
        /// -no exception
        /// -out parameter is correct
        /// -return value IS null.
        /// </summary>
        public virtual void VerifyGenerateLocator(PathNode node, bool expectedContinueGenerating) 
        {
            bool continueGenerating;
            ContentLocator lps = processor.GenerateLocator(node, out continueGenerating);
            suite.AssertEquals("Verify value of continueGenerating.", expectedContinueGenerating, continueGenerating);
            suite.AssertNull("Verify return value is not null.", lps);            
        }
        public virtual void VerifyGenerateLocatorFails(PathNode node, Type expectedExceptionType) 
        {
            bool exception = false;
            try
            {
                bool continueGenerating;
                processor.GenerateLocator(node, out continueGenerating);
            }
            catch (Exception e)
            {
                exception = true;
                suite.AssertEquals("Verify exception type.", expectedExceptionType, e.GetType());
            }
            suite.Assert("Expected exception to have occurred.", exception);
        }

        #endregion

        #region ResolveLocatorPart

        /// <summary>
        /// Verify return value and value of out parameter.
        /// </summary>
        public virtual void VerifyResolveLocatorPart(ContentLocatorPart locatorPart, DependencyObject startNode, bool expectedContinueResolving, object expectedResult) 
        {
            bool continueResolving;
            object result = processor.ResolveLocatorPart(locatorPart, startNode, out continueResolving);

            suite.AssertEquals("Verify result.", expectedResult, result);
            suite.AssertEquals("Verify continueResolving.", expectedContinueResolving, continueResolving);
        }
        public virtual void VerifyResolveLocatorPartFails(ContentLocatorPart locatorPart, DependencyObject startNode, Type expectedExceptionType) 
        {
            bool exception = false;
            try
            {
                bool continueResolving;
                processor.ResolveLocatorPart(locatorPart, startNode, out continueResolving);
            }
            catch (Exception e)
            {
                exception = true;
                suite.AssertEquals("Verify exception type.", expectedExceptionType, e.GetType());
            }
            suite.Assert("Expected exception to have occurred.", exception);
        }

        #endregion

        #region GetLocatorPartTypes

        public void VerifyGetLocatorPartTypes(XmlQualifiedName[] names)
        {
            XmlQualifiedName[] locatorPartTypes = processor.GetLocatorPartTypes();
            suite.AssertEquals("Verify number of LocatorPartTypes.", names.Length, locatorPartTypes.Length);
            for (int i = 0; i < names.Length; i++)
                suite.AssertEquals("Verify type '" + i + "'.", names[i], locatorPartTypes[i]);
        }

        #endregion

        
        protected SubTreeProcessor processor;
        
        protected TestSuite suite;
	}
}
