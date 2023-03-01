// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Base class for FlowDocumentScrollViewer tests.

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Controls;

namespace Avalon.Test.Annotations
{
    public class AFlowDocumentScrollViewerSuite : ATextControlTestSuite
    {
        #region TestSuite Overrides

        protected override ATextControlTestSuite.AnnotatableTextControlTypes DetermineTargetControlType(string[] args)
        {
            return AnnotatableTextControlTypes.FlowDocumentScrollViewer;
        }

        /// <summary>
        /// Store the caseNum and initialize the test.
        /// </summary>
        [TestCase_Setup()]
        protected virtual void DoSetup()
        {
            SetupTestWindow();
            SetContent(LoadContent(Simple));

            TabHelper.Root = MainWindow;
        }

        protected override object CreateWindowContents()
        {
            ContentControl toolbar = new AnnotationToolbar();
            DockPanel.SetDock(toolbar, Dock.Top);

            DockPanel mainPanel = new DockPanel();
            mainPanel.Children.Add(toolbar);
            mainPanel.Children.Add(TextControl);

            return mainPanel;
        }

        protected override void CleanupVariation()
        {
            TabHelper.Root = null;
            base.CleanupVariation();
        }

        #endregion

        #region Protected Methods

        protected void TestAddDelete(ISelectionData selection, bool pageUp, bool visible)
        {
            if (!visible) { if (pageUp) PageUp(); else PageDown(); }
            CreateAnnotation(selection);
            if (!visible) { if (pageUp) PageDown(); else PageUp(); }
            VerifyAnnotation(GetText(selection));
            if (!visible) { if (pageUp) PageDown(); else PageUp(); }
            DeleteAnnotation(selection);
            if (!visible) { if (pageUp) PageUp(); else PageDown(); }
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Verified creating and deleting annotation.");
        }

        protected void TestCanTabTo(string author)
        {
            TextControlWrapper.Target.Focus();
            TabHelper.TabToAnnotationGroup();
            Assert("Verify Note is focused.", TabHelper.IsCurrentElementAStickyNote);
            VerifyNoteViewportVisibility(author, true);
            passTest("Verified note scrolled into view.");
        }

        #endregion

        #region Properties
       

        #endregion

        #region Fields

        protected TabHelper TabHelper = new TabHelper();

        protected static string Simple = "Simple.xaml";
        protected static string Article1 = "Article1.xaml";

        #endregion
    }
}	

