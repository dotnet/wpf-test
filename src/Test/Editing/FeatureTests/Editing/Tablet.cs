// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Integration test cases for the Tablet environment. 

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Editing/Tablet.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;

    using System.Windows;
    using System.Windows.Annotations;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Ink;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Performs a smoke test of the Tablet components integrated with
    /// the text editable controls.
    /// </summary>
    [Test(2, "Editor", "TabletSmokeTest", MethodParameters = "/TestCaseType:TabletSmokeTest", Timeout = 120)]
    [TestOwner("Microsoft"), TestTactics("298"), TestWorkItem("25"), TestBugs("477")]
    public class TabletSmokeTest: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            return result;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            // Create an instance to test.
            FrameworkElement element = _editableType.CreateInstance();
            _wrapper = new UIElementWrapper(element);

            // Create an InkCanvas around the element.
            _canvas = new InkCanvas();
            _canvas.Width = 200;
            _canvas.Height = 200;
            _canvas.Children.Add(element);
            _canvas.EditingMode = _editingMode;
            TestElement = _canvas;

            QueueDelegate(TestInteraction);
        }

        private void TestInteraction()
        {
            MouseInput.MouseClick(_wrapper.Element);

            QueueDelegate(TestTyping);
        }

        private void TestTyping()
        {
            KeyboardInput.TypeString("   {ENTER}");
            QueueDelegate(VerifyResults);
        }

        private void VerifyResults()
        {
            Log("Element content: [" + _wrapper.Text + "]");
            Log("Element content length: " + _wrapper.Text.Length);
            if (_editingMode == InkCanvasEditingMode.None)
            {
                Verifier.Verify(_wrapper.Text.Length > 2,
                    "text was modified in None editing mode", true);
            }
            else
            {
                Verifier.Verify(_wrapper.Text.Length <= 2,
                    "text was not modified in editing mode", true);
            }
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        /// <summary>Canvas around control being tested.</summary>
        private InkCanvas _canvas  = null;

        /// <summary>Wrapper for control being tested.</summary>
        private UIElementWrapper _wrapper = null;

        /// <summary>EditableType of control tested.</summary>
        private TextEditableType _editableType = null;

        /// <summary>Editing mode for ink canvas.</summary>
        private InkCanvasEditingMode _editingMode =0;

        #endregion Private fields.
    }

    /// <summary>
    /// Performs a smoke test on the Text StickyNote controls in
    /// annotated flow documents.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("123"), TestBugs("477")]
    public class StickyNoteEditingTest: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs the current combination.</summary>
        protected override void DoRunCombination()
        {
            CreateDocument();
            QueueDelegate(MakeSelection);
        }

        /// <summary>Create the document to annotate.</summary>
        private void CreateDocument()
        {
            _document = new FlowDocument();

            for (int i = 0; i < 35; i++)
            {
                _document.Blocks.Add(new Paragraph(new Run(TextScript.Latin.Sample)));
            }

            _viewer = new FlowDocumentPageViewer();
            _viewer.Document = _document;

            _annotationService = new AnnotationService(_viewer);
            _annotationService.Enable(new System.Windows.Annotations.Storage.XmlStreamStore(new System.IO.MemoryStream()));
            _annotationService.Store.AutoFlush = true;

            TestElement = _viewer;
        }

        /// <summary>Makes a selection on the document to annotate.</summary>
        private void MakeSelection()
        {
            if (_isSelectionUnderneath)
            {
                MouseInput.MouseClick(_viewer);
                KeyboardInput.TypeString("^{HOME}+{DOWN 10}");
            }
            else
            {
                MouseInput.MouseClick(_viewer);
                KeyboardInput.TypeString("^{HOME}");
                KeyboardInput.TypeString("+{RIGHT}");
            }
            QueueDelegate(CreateStickyNote);
        }

        /// <summary>Creates a StickyNote on the document.</summary>
        private void CreateStickyNote()
        {
            Log("Creating a StickyNote...");
            AnnotationService.CreateTextStickyNoteCommand.Execute(null, _viewer);

            QueueDelegate(PerformEditingAction);
        }

        /// <summary>Performs an editing action on the StickyNote.</summary>
        private void PerformEditingAction()
        {
            UIElementWrapper wrapper;

            Log("Focused element: " + Keyboard.FocusedElement + " (expecting RichTextBox)");

            wrapper = new UIElementWrapper((UIElement)Keyboard.FocusedElement);
            _state = _editingData.CaptureBeforeEditing(wrapper);
            _editingData.PerformAction(wrapper, VerifyEditing);
        }

        private void VerifyEditing()
        {
            _editingData.VerifyEditing(_state);
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        private FlowDocumentPageViewer _viewer;
        private FlowDocument _document;
        private AnnotationService _annotationService;
        private KeyboardEditingState _state;

        /// <summary>Whether the selection is fully underneath the StickyNote.</summary>
        private bool _isSelectionUnderneath = false;
        
        /// <summary>Editing action to perform.</summary>
        private KeyboardEditingData _editingData = null;

        #endregion Private fields.
    }
}
