// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/forms/BVT/DataTransfer/DragDrop/DragDropAPITest.cs $")]
/*************************************************
 *  
 *  This file test:
 *  1. RevokeDropTarget()
 *  2. DoDragDrop(text string)
 *  3. DragDropEffects() - copy, move, Link, None, Scroll
 *  4. AddHandler()
 *  5. RemoveHandler()
 *  Command Line: exe.exe /TestCaseType=DragDropAPITest /DragDrop=Valid, Invalid
 *
 * ************************************************/

namespace DragDropAPI
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using System.Windows.Interop;
    using Test.Uis.Data;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    #endregion Namespaces

    /// <summary>Test for DragDrop API</summary>
    [Test(0, "DragDrop", "DragDropAPITest1", MethodParameters = "/TestCaseType=DragDropAPITest /DragDrop=Valid")]
    [Test(2, "DragDrop", "DragDropAPITest2", MethodParameters = "/TestCaseType=DragDropAPITest /DragDrop=Invalid")]
    [TestOwner("Microsoft"), TestTactics("188,189"), TestBugs("")]
    public class DragDropAPITest : CustomTestCase
    {
        private Canvas _root;
        private TextBox _tb;
        private IWin32Window _win32Window;       //for RegisterDragDrop
        private DragDropEffects _dropEffectNone;
        private IDataObject _dataObject;
        private Object _data;
        //private bool registerDrop;              //to verify register droptarget
        //private bool revokeDrop;                //to verify revokedroptarget
        private Rect _rc;                        //find rectangle of a control
        private System.Windows.Point _pStart;    //point for mouse action

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            _root = new Canvas();
            _root.Width = 500;
            _root.Height = 500;
            this.MainWindow.Content = _root;
            _tb = new TextBox();
            _tb.Width = 500;
            _tb.Height = 500;
            _root.Children.Add(_tb);
            this.MainWindow.Show();

            _dataObject = (IDataObject)new DataObject();
            _dataObject.SetData("abc 123");  //DataObject to be dragdrop

            _data = new Object();
            _data = "abc 123";               //Object to be dragdrop

            _dropEffectNone = DragDropEffects.None;

            //attach mousedown event to textbox to Do DoDragDrop
            _tb.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
            // DragDrop.AddHandler(tb, DragDrop.DropEvent, new DragEventHandler(MyOnDrop)); //
            _tb.AddHandler(DragDrop.DropEvent, new DragEventHandler(MyOnDrop), true);
            InputMonitorManager.Current.IsEnabled = false;
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(startTest));
        }

        void startTest()
        {
            _rc = ElementUtils.GetScreenRelativeRect(_tb);
            _pStart = new Point(_rc.Left + 450, _rc.Top + 100);
            PresentationSource source = PresentationSource.FromVisual(_root);

            _win32Window = source as IWin32Window;
            switch (ConfigurationSettings.Current.GetArgument("DragDrop", true))
            {
                case "Valid":
                    DoRegisterDropTarget();
                    break;
                case "Invalid":
                    DoDoDragDropNull();
                    break;
            }
        }

        private void DoRegisterDropTarget()
        {
            //No need to RegisterDropTarget() due to AllowDrag, AllowDrop is enbled on TextBox by default.
            //See 


            MouseInput.MouseDown(_pStart);
            MouseInput.MouseUp();
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragDropEffects dragDropEffects = DragDrop.DoDragDrop((DependencyObject)_tb, _data, DragDropEffects.Copy | DragDropEffects.Move);
            Verifier.Verify(dragDropEffects == DragDropEffects.Move, "DragDropEffects is Move." + dragDropEffects, true);

            _dropEffectNone = DragDrop.DoDragDrop((DependencyObject)_tb, _data, DragDropEffects.None);
            Verifier.Verify(DragDropEffects.None == _dropEffectNone, "DragDropEffects.None works." + _dropEffectNone, true);
        }
        private void MyOnDrop(object sender, DragEventArgs e)
        {
            Verifier.Verify(_tb.Text == "abc 123", "Text in TextBox is correct." + _tb.Text, true);
            Log("DropEvent fired!");
            Logger.Current.ReportSuccess();
        }
        private void DoDoDragDropNull()
        {
            try
            {
                // Set the default drop effects with Copy and none
                DragDrop.DoDragDrop(null, "abc", DragDropEffects.Copy | DragDropEffects.Move);
                throw new ArgumentNullException("DoDragDrop accepted null values.");
            }
            catch (ArgumentNullException)
            {
                Log("DoDragDrop rejects null values.");
            }
            try
            {
                DragDrop.DoDragDrop(_tb, null, DragDropEffects.Copy | DragDropEffects.Move);
                throw new ArgumentNullException("DoDragDrop accepted null data.");
            }
            catch (ArgumentNullException)
            {
                Log("DoDragDrop rejects null data.");
            }
            DoHandlerNull();
        }
        private void DoHandlerNull()
        {
            try
            {
                _tb.AddHandler(null, null);
                throw new ArgumentNullException("AddHandler accepted null RoutedEvent.");
            }
            catch (ArgumentNullException)
            {
                Log("AddHandler rejects null RoutedEvent.");
            }
            try
            {
                _tb.AddHandler(DragDrop.QueryContinueDragEvent, null);
                throw new ArgumentNullException("AddHandler accepted null Delegate handler.");
            }
            catch (ArgumentNullException)
            {
                Log("AddHandler rejects null Delegate handler.");
            }
            try
            {
                _tb.RemoveHandler(null, null);
                throw new ArgumentNullException("RemoveHandler accepted null RoutedEvent.");
            }
            catch (ArgumentNullException)
            {
                Log("RemoveHandler rejects null RoutedEvent.");
            }
            try
            {
                _tb.RemoveHandler(DragDrop.QueryContinueDragEvent, null);
                throw new ArgumentNullException("RemoveHandler accepted null Delegate handler.");
            }
            catch (ArgumentNullException)
            {
                Log("RemoveHandler rejects null Delegate handler.");
            }
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Verifies that events can be added and removed, and that they
    /// fire correctly.
    /// </summary>
    /// <remarks>
    /// Test matrix:
    /// - Target element
    ///   - UIElement with internal handling (TextBox, RichTextBox)
    ///   - UIElement with no internal handling (Rectangle)
    ///   - ContentElement
    /// - Event
    ///   - DragOver
    ///   - Drop
    ///   - GiveFeedback
    ///   - QueryContinueDrag
    ///   - DragEnter
    ///   - DragLeave
    /// - Event stage
    ///   - Normal event
    ///   - Preview event
    /// - How the event is added
    ///   - CLR accessor
    ///   - Attached event syntax
    /// - How the event is removed
    ///   - CLR accessor
    ///   - Attached event syntax
    /// - Removed delegate
    ///   - Whether the delegate was removed
    /// Things to verify:
    /// - Whether an event was fired.
    /// - What the source of the event is.
    /// Runtime is ~7min on a middle-range machine.
    /// </remarks>
    // DISABLEDUNSTABLETEST:
    // TestName:DragDropEvents1
    // Area: Editing�� SubArea: DragDrop
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
    [Test(2, "DragDrop", "DragDropEvents1", MethodParameters = "/TestCaseType:DragDropEvents /Pri:0", Timeout = 240, Disabled = true)]
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "DragDropEvents2", MethodParameters = "/TestCaseType:DragDropEvents /Pri:0 /XbapName=EditingTestDeploy", Timeout = 240)]
    [Test(3, "DragDrop", "DragDropEvents3", MethodParameters = "/TestCaseType:DragDropEvents /Pri:3", Timeout = 600, Disabled=true)]
    [Test(3, "DragDrop", "DragDropEvents4", MethodParameters = "/TestCaseType:DragDropEvents /Pri:4", Timeout = 600, Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("190, 191"), TestWorkItem("20,21")]
    public class DragDropEvents : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads a specific combination and determines whether it should be run.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            // If we are not going to remove the handler, it doesn't
            // matter which one we're picking - filter for a single
            // value then (arbitrarly chose Accessor).
            // Condition: we are either removing the event, or we have the Accessor value.
            result = result &&
                (this._isEventRemoved || this._removeMethod == EventSettingMethod.Accessor);

            return result;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            StackPanel topPanel;

            // Reset state from previous combinations.
            _fireCount = 0;

            // Create the target and source elements.
            CreateElement(_elementType, out _sourceForEvent, out _sourceElement);
            CreateElement(_elementType, out _targetForEvent, out _targetElement);

            // Attach the event handlers.
            AttachEvent();

            // Remove the event handlers if needed.
            if (_isEventRemoved)
            {
                RemoveEvent();
            }

            // Add the elements to the tree.
            topPanel = new StackPanel();
            topPanel.Children.Add(_sourceElement);
            topPanel.Children.Add(_targetElement);
            TestElement = topPanel;

            QueueDelegate(AfterLayout);
        }

        private void AfterLayout()
        {
            // Get the coordinates for the drag/drop operation.
            _startPoint = GetPointForObject(_sourceForEvent);
            _endPoint = GetPointForObject(_targetForEvent);

            _sourceElement.Focus();

            // Prepare to start a drag/drop operation.
            MouseInput.MouseMove(_startPoint);
            MouseInput.MouseDown();

            QueueDelegate(PerformDragDrop);
        }

        private void PerformDragDrop()
        {
            // Perform a drag/drop operation.
            MouseInput.MouseDragInOtherThread(_startPoint, _endPoint,
                true, TimeSpan.FromMilliseconds(200), AfterDragDrop, Dispatcher.CurrentDispatcher);
            if (!IsDragDropImplemented)
            {
                try
                {
                    DragDrop.DoDragDrop(_sourceForEvent, "data", DragDropEffects.All);
                }
                catch (System.Security.SecurityException )
                {
                    // If it is running in partial trust, the expcetion is caught. 
                    // Since no event should be fired, the caught event is ignored.
                    _runningInPartialTrust = true;
                }
            }
        }

        private void AfterDragDrop()
        {
            // When running in partial trust, we cannot initiate drag-drop ourselves.
            // For some reason !SecurityHelper.GetIsStackFullyTrusted() and
            // _runningInPartialTrust are not both true sometimes.
            // We need to do a little investigation for this.
            if (!SecurityHelper.GetIsStackFullyTrusted() || _runningInPartialTrust)
            {
                // Drag-drop is disabled by design.
                Verifier.Verify(_fireCount == 0, "Drag-drop disabled in partial trust.");
            }
            else if (_isEventRemoved)
            {
                Verifier.Verify(_fireCount == 0, "No events fire when event is removed.");
            }
            else if (_isEventStagePreview)
            {
                Verifier.Verify(_fireCount > 0, "Preview events are always fired.");
            }
            else
            {
                if (!IsDragDropImplemented)
                {
                    Verifier.Verify(_fireCount > 0, "Regular events always fire.");
                }
                else
                {
                    Verifier.Verify(_fireCount == 0, "TextEditor handles non-preview drag/drop events.");
                }
            }

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Helper members.

        private void CreateElement(Type type, out DependencyObject objectForEvent, out UIElement element)
        {
            string text;    // Text for content and editable controls.

            text = StringData.MixedScripts.Value.Substring(0, 20);

            objectForEvent = (DependencyObject) Activator.CreateInstance(type);

            objectForEvent.SetValue(Control.AllowDropProperty, true);

            // In the simplest case, the element is a UIElement itself.
            element = objectForEvent as UIElement;
            if (element != null)
            {
                element.SetValue(Control.WidthProperty, 30d);
                element.SetValue(Control.HeightProperty, 20d);
                if (element is System.Windows.Shapes.Shape)
                {
                    ((System.Windows.Shapes.Shape)element).Fill = _brushes[_brushIndex++ % _brushes.Length];
                }
                else if (element is TextBoxBase)
                {
                    new UIElementWrapper(element).Text = text;
                    ((TextBoxBase)element).SelectAll();
                    ((TextBoxBase)element).FontFamily = new FontFamily("Arial");
                    ((TextBoxBase)element).FontSize = 12d;
                }
                if (element is Control)
                {
                    ((Control)element).Background = _brushes[_brushIndex++ % _brushes.Length];
                }
            }
            else
            {
                // This is some kind of Content Element. Put it into a container in this case.
                Type textFlowType = Assembly.GetAssembly(typeof(FrameworkElement)).GetType("MS.Internal.Documents.FlowDocumentView");
                RichTextBox rtb = new RichTextBox();
                FlowDocument flowDocumentView = rtb.Document;
                BlockCollection textFlowBlocks = (BlockCollection)ReflectionUtils.GetProperty(flowDocumentView, "Blocks");
                element = rtb;

                if (objectForEvent is Block)
                {
                    if (objectForEvent is Paragraph)
                    {
                        ((Paragraph)objectForEvent).Inlines.Add(text);
                    }
                    textFlowBlocks.Add((Block)objectForEvent);
                }
                else if (objectForEvent is Inline)
                {
                    if (objectForEvent is Span)
                    {
                        ((Span)objectForEvent).Inlines.Add(text);
                    }
                    textFlowBlocks.Add(new Paragraph((Inline)objectForEvent));
                }
                else
                {
                    throw new Exception("Test case does not support elements of type " + type);
                }
            }
        }

        /// <summary>Object to which events should be attached.</summary>
        private DependencyObject ObjectForEvent
        {
            get
            {
                // DragLeave is actually fired on the target, but
                // we connect it to the source object to make
                // a single drag-drop operation.
                if (_eventToTest == DragDropEvent.GiveFeedback ||
                    _eventToTest == DragDropEvent.QueryContinueDrag ||
                    _eventToTest == DragDropEvent.DragLeave)
                {
                    return _sourceForEvent;
                }
                else
                {
                    return _targetForEvent;
                }
            }
        }

        private bool IsDragDropImplemented
        {
            get { return ObjectForEvent is TextBoxBase; }
        }

        private void QueryContinueDragHandler(object sender, QueryContinueDragEventArgs e)
        {
            _fireCount++;
        }

        private void GiveFeedbackHandler(object sender, GiveFeedbackEventArgs e)
        {
            _fireCount++;
        }

        private void DragHandler(object sender, DragEventArgs e)
        {
            _fireCount++;
        }

        /// <summary>Creates a delegate that can handle the specified event name.</summary>
        private Delegate CreateHandler(string eventName)
        {
            switch (eventName)
            {
                case "QueryContinueDrag":
                case "PreviewQueryContinueDrag":
                    return new QueryContinueDragEventHandler(QueryContinueDragHandler);

                case "GiveFeedback":
                case "PreviewGiveFeedback":
                    return new GiveFeedbackEventHandler(GiveFeedbackHandler);

                case "DragEnter":
                case "DragLeave":
                case "DragOver":
                case "Drop":
                case "PreviewDragEnter":
                case "PreviewDragLeave":
                case "PreviewDragOver":
                case "PreviewDrop":
                    return new DragEventHandler(DragHandler);

                default:
                    throw new ArgumentException("Cannot create handler for event " + eventName);
            }
        }

        private string GetNameForEvent()
        {
            string result;

            result = _eventToTest.ToString();
            if (_isEventStagePreview)
            {
                result = "Preview" + result;
            }
            return result;
        }

        private void AttachEvent()
        {
            string eventName;
            Delegate handler;

            eventName = GetNameForEvent();
            handler = CreateHandler(eventName);

            // We could use a big switch or a table, but there
            // is a strict pattern we can rely on to access
            // through reflection.
            switch (_addMethod)
            {
                case EventSettingMethod.Accessor:
                    ReflectionUtils.AddInstanceEventHandler(ObjectForEvent,
                        eventName, handler);
                    break;
                case EventSettingMethod.HandlerMethod:
                    RoutedEvent routedEvent;

                    routedEvent = (RoutedEvent)
                        ReflectionUtils.GetStaticField(typeof(DragDrop), eventName + "Event");
                    if (ObjectForEvent is UIElement)
                    {
                        ((UIElement)ObjectForEvent).AddHandler(routedEvent, handler);
                    }
                    else if (ObjectForEvent is ContentElement)
                    {
                        ((ContentElement)ObjectForEvent).AddHandler(routedEvent, handler);
                    }
                    else
                    {
                        throw new Exception("Unable to use AddHandler on " + ObjectForEvent);
                    }
                    break;
                case EventSettingMethod.StaticMethod:
                    ReflectionUtils.InvokeStaticMethod(typeof(DragDrop),
                        "Add" + eventName + "Handler", new object[] { ObjectForEvent, handler });
                    break;
            }
        }

        private void RemoveEvent()
        {
            string eventName;
            Delegate handler;

            eventName = GetNameForEvent();
            handler = CreateHandler(eventName);

            switch (_removeMethod)
            {
                case EventSettingMethod.Accessor:
                    ReflectionUtils.RemoveInstanceEventHandler(ObjectForEvent,
                        eventName, handler);
                    break;
                case EventSettingMethod.HandlerMethod:
                    RoutedEvent routedEvent;

                    routedEvent = (RoutedEvent)
                        ReflectionUtils.GetStaticField(typeof(DragDrop), eventName + "Event");
                    if (ObjectForEvent is UIElement)
                    {
                        ((UIElement)ObjectForEvent).RemoveHandler(routedEvent, handler);
                    }
                    else if (ObjectForEvent is ContentElement)
                    {
                        ((ContentElement)ObjectForEvent).RemoveHandler(routedEvent, handler);
                    }
                    else
                    {
                        throw new Exception("Unable to use RemoveHandler on " + ObjectForEvent);
                    }
                    break;
                case EventSettingMethod.StaticMethod:
                    ReflectionUtils.InvokeStaticMethod(typeof(DragDrop),
                        "Remove" + eventName + "Handler", new object[] { ObjectForEvent, handler });
                    break;
            }
        }

        private Point GetPointForObject(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            if (dependencyObject is UIElement)
            {
                return ElementUtils.GetScreenRelativeCenter((UIElement)dependencyObject);
            }
            else if (dependencyObject is TextElement)
            {
                TextElement textElement = (TextElement)dependencyObject;
                DependencyObject documentParent;
                Rect characterRect;

                characterRect = textElement.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                documentParent = textElement.ContentStart.DocumentStart.Parent;
                if (documentParent is FlowDocument)
                {
                    documentParent = ((FlowDocument)documentParent).Parent;
                }
                if (documentParent is UIElement)
                {
                    Point result;

                    result = ElementUtils.GetScreenRelativePoint((UIElement)documentParent, characterRect.TopLeft);
                    result.Offset(characterRect.Width / 2, characterRect.Height / 2);
                    return result;
                }
                else
                {
                    throw new InvalidOperationException("Cannot find a UIElement parenting " +
                        dependencyObject + ", only found " + documentParent);
                }
            }
            else
            {
                throw new Exception("Cannot get point for object " + dependencyObject);
            }
        }

        #endregion Helper members.

        #region Private fields.

        private Type _elementType=null;
        private DragDropEvent _eventToTest=0;
        private bool _isEventStagePreview=false;
        private EventSettingMethod _addMethod=0;
        private EventSettingMethod _removeMethod=0;
        private bool _isEventRemoved=false;

        /// <summary>Check to see if the cases is running in partial trust</summary>
        private bool _runningInPartialTrust=false;

        /// <summary>UIElement that is or contains the source object for the drag/drop event.</summary>
        private UIElement _sourceElement;

        /// <summary>UIElement that is or contains the target object for the drag/drop event.</summary>
        private UIElement _targetElement;

        /// <summary>Object that is the event source for the drag/drop source.</summary>
        private DependencyObject _sourceForEvent;

        /// <summary>Object that is the event source for the drag/drop target.</summary>
        private DependencyObject _targetForEvent;

        /// <summary>Start point for drag/drop operation in screen coordinates.</summary>
        private Point _startPoint;

        /// <summary>End point for drag/drop operation in screen coordinates.</summary>
        private Point _endPoint;

        private Brush[] _brushes = new Brush[] { Brushes.Tan, Brushes.SlateBlue, Brushes.OrangeRed, Brushes.Gainsboro };

        private int _brushIndex;

        private int _fireCount;

        #endregion Private fields.

        #region Inner types.

        internal enum DragDropEvent
        {
            DragOver,
            Drop,
            GiveFeedback,
            QueryContinueDrag,
            DragEnter,
            DragLeave
        }

        internal enum EventSettingMethod
        {
            Accessor,
            HandlerMethod,
            StaticMethod,
        }

        #endregion Inner types.
    }
}
