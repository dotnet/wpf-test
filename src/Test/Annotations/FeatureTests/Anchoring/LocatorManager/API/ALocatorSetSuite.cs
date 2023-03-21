// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Superclass for the LocatorSetSuite test suite.  This provides
//  some common functions and builds the tree used by the LocatorSetSuite tests.
//  
//  The ContentLocatorGroup test suite consists of testing the following API and 
//  functionality:
//
//  - GenerateLocators
//  - ResolveLocator
//  - FindAttachedAnchor


using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Documents;
using System.Collections;
using System.Collections.Generic;

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
using Proxies.MS.Internal.Annotations;					// TestSuite.


namespace Avalon.Test.Annotations
{
    public abstract class ALocatorSetSuite : AAnchoringAPITests
    {
        #region globals
        protected Canvas a = null;
        protected FlowDocumentScrollViewer txtPanel = null;
        protected Paragraph p1 = null;
        protected Paragraph p2 = null;
        protected Paragraph p3 = null;
        protected Paragraph empty_Paragraph = null;
        protected Image empty_image = null;
        protected Paragraph p4 = null;

        protected AttachmentLevel anchorType;

        protected int moveStart = 0;
        protected int moveEnd = 0;
        protected int expectedLocators = -1;
        protected int expectedLocatorParts = 2;
        #endregion

        // ---------------------------------------------------------------------------------------------
        //                                  BUILD VARIATIONS OF TREE
        // ---------------------------------------------------------------------------------------------


        protected void BuildLocatorSetTree()
        {
            // Set up root
            a = new Canvas();
            EnableService(a, annotationStore);


            txtPanel = new FlowDocumentScrollViewer();
            txtPanel.Document = new FlowDocument(new Paragraph(new Run("Congratulations!  Today is your day.  You're off to Great Places!  You're off and away!\n")));

            txtPanel.Document.Blocks.Add(new Paragraph(new Run("You have brains in your head.  You have feet in your shoes.  You can steer yourself any direction you choose.\n")));

            txtPanel.Document.Blocks.Add(new Paragraph(new Run("You're on your own. And you know what you know.  And YOU are the guy who'll decide where to go.\n")));

            txtPanel.Document.Blocks.Add(new Paragraph(new Run()));

            txtPanel.Document.Blocks.Add(new Paragraph(new InlineUIContainer(new Image())));

            txtPanel.Document.Blocks.Add(new Paragraph(new Run("You'll look up and down the streets. Look 'em over with care.  About some you will say, \"I dont choose to go there.\"  With your head full of brains and your shoes full of feet, you're too smart to go down any not-so-good street.\n")));

            DataIdProcessor.SetDataId(txtPanel, "txtPanel");

            a.Children.Add(txtPanel);
            //LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
        }


        // ---------------------------------------------------------------------------------------------
        //                                  HELPER METHODS
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Calls the FindAttachedAnchor (internal) method, packs all the parameters and
        /// unpacks the result
        /// </summary>
        /// <param name="startNode">Node to start searching from</param>
        /// <param name="targetNode">Node to look for</param>
        /// <param name="anchorType">Level of resolution achieved</param>
        /// <returns>the resolved anchor and the attachment level (as an out parameter)</returns>
        protected Object FindAttachedAnchor(DependencyObject startNode, object targetNode,
            out AttachmentLevel anchorType)
        {
            Object[] paramsArray = new Object[4];
            paramsArray[0] = startNode;
            paramsArray[1] = GetStartNodePrefix(startNode);
            paramsArray[2] = GetTargetLocator(targetNode);

            Object result = AnchoringAPIHelpers.CallInternalMethod("FindAttachedAnchor", manager, paramsArray);
            anchorType = (AttachmentLevel)paramsArray[3];

            return result;
        }

        /// <summary>
        /// Generates Locators for the startNode and pass out the first ContentLocator
        /// from the generated ContentLocatorBase
        /// </summary>
        /// <param name="startNode">node we want the ContentLocatorBase prefix of</param>
        /// <returns>the first ContentLocator of the node's ContentLocatorBase</returns>
        private ContentLocator[] GetStartNodePrefix(DependencyObject startNode)
        {
            IList<ContentLocatorBase> startNodeLocators = manager.GenerateLocators(startNode);
            return startNodeLocators.Count == 0 ? new ContentLocator[0] : 
                new ContentLocator[1] { (ContentLocator)startNodeLocators[0] };
        }

        /// <summary>
        /// Generates Locators for the targetNode and returns the first ContentLocatorBase
        /// </summary>
        /// <param name="targetNode">node we want the ContentLocatorBase of</param>
        /// <returns>the first ContentLocatorBase in the list of generated Locators for the node</returns>
        private ContentLocatorBase GetTargetLocator(object targetNode)
        {
            IList<ContentLocatorBase> targetLocators = manager.GenerateLocators(targetNode);
            return targetLocators[0] as ContentLocatorBase;
        }


    }       // end of ALocatorSetSuite class

}           // end of namespace

