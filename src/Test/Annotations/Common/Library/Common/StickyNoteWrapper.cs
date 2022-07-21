// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Wrapper class for the internal Type StickyNoteControl.
//

using System;
using System.Windows;
using System.IO;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Resources;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Automation;
using System.Diagnostics;
using System.Windows.Threading;
using Proxies.MS.Internal.Annotations.Component;
using Annotations.Test.Reflection;
using System.Windows.Media;
using Annotations.Test.Framework;
using System.Windows.Ink;
using System.Windows.Controls.Primitives;

namespace Avalon.Test.Annotations
{
    /// <summary>
    /// Valid states for a StickyNote to be in.
    /// </summary>
    public enum StickyNoteState
    {
        // Currently these two states of activation are equivalent.  In the future they may be 
        // different in which case test case definitions will not need to be updated.
        //
        Active_Focused = 0x01,			// Active because SN is focused.
        Active_Selected = 0x02,			// Active because anchor is selected.

        Inactive = 0x00					// Not active. 
    }

    public class StickyNoteWrapper
    {
        public static Size IconHoverSize = new Size(16, 16);
        public static Size IconDefaultSize = new Size(10, 5);

        #region Constructors

        static StickyNoteWrapper()
        {
            PresentationFramework = typeof(FrameworkElement).Assembly;
            StickyNote_Type = PresentationFramework.GetType("System.Windows.Controls.StickyNoteControl");
            StickyNote_Anchor = StickyNote_Type.GetField("_anchor", BindingFlags.NonPublic | BindingFlags.Instance);
            StickyNote_Content = StickyNote_Type.GetProperty("Content", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Create a new wrapper for the given StickyNote and assign a unique ID to it.
        /// </summary>
        /// <param name="stickynote">StickyNoteControl to wrap.</param>
        /// <param name="uniqueId">will be set as the Name property of the StickyNote control.</param>
        public StickyNoteWrapper(Control stickynote, string uniqueId)
        {
            Initialize(stickynote, uniqueId);
        }

        public StickyNoteWrapper(Proxies.System.Windows.Controls.StickyNoteControl stickynoteProxy, string uniqueId)
        {
            Initialize((Control)stickynoteProxy.Delegate, uniqueId);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Programmatically set the focus of the Note.
        /// </summary>
        public void Focus()
        {
            Target.Focus();
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Use the mouse to click on the caption part of the StickyNote.
        /// </summary>
        public void ClickIn()
        {
            if (Expanded)
                UIAutomationModule.MoveToAndClickElement(TitleThumb);
            else
                UIAutomationModule.MoveToAndClickElement(Icon);
        }

        public void MoveTo()
        {
            UIAutomationModule.MoveToCenter(Target);
        }

        public void MinimizeWithMouse()
        {
            UIAutomationModule.MoveToCenterAndClickElement(CloseButton);
        }

        /// <summary>
        /// Resize note using the mouse by the given Vector.
        /// </summary>
        public void ResizeWithMouse(Vector delta)
        {
            UIAutomationModule.MoveToCenter(ResizeThumb);
            UIAutomationModule.LeftMouseDown();
            UIAutomationModule.Move(delta);
            UIAutomationModule.LeftMouseUp();
            DispatcherHelper.DoEvents();
        }

        public void Drag(Point dest) { _dragModule.Drag(dest); }
        public void Drag(Vector delta) { _dragModule.Drag(delta); }

        /// <summary>
        /// Move this StickyNote by the given offset.
        /// </summary>
        /// <remarks>
        /// Movement is programmatic (e.g. it does not use the mouse).  Updates StickyNote's internal
        /// PositionTransform property to change its position.
        /// </remarks>
        /// <param name="delta">Offset to move StickyNote.</param>
        public void Move(Vector delta)
        {
            TranslateTransform transform = PositionTranform;
            transform.X += delta.X;
            transform.Y += delta.Y;
            PositionTranform = transform;

            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Select all the contents of the Note.
        /// </summary>
        public void SelectAll()
        {
            if (Type == StickyNoteType.Text)
            {
                this.RichTextBox.SelectAll();
            }
            else
            {
                InkCanvas canvas = this.InkCanvas;
                canvas.Select(canvas.Strokes);
            }
        }

        #endregion

        #region Public Properties

        public RichTextBox RichTextBox
        {
            get
            {
                return InnerControl as RichTextBox;
            }
        }
        public InkCanvas InkCanvas
        {
            get
            {
                return InnerControl as InkCanvas;
            }
        }

        /// <summary>
        /// If InkNote: return InkStrokeCollection.
        /// If TextNote: return the Text of a FlowDocument.
        /// </summary>
        public object Content
        {
            get
            {
                if (Type == StickyNoteType.Text)
                    return GetFlowDocumentText(this.RichTextBox.Document);
                else
                    return this.InkCanvas.Strokes;
            }
            set
            {
                if (Type == StickyNoteType.Text)
                    this.RichTextBox.Document = value as FlowDocument;
                else
                    this.InkCanvas.Strokes = value as StrokeCollection;
                DispatcherHelper.DoEvents();
            }
        }

        /// <summary>
        /// Return true if RTB or InkCanvas has Text or InkStrokes.
        /// </summary>
        public bool HasContent
        {
            get
            {
                if (Content != null)
                {
                    if (Type == StickyNoteType.Text)
                    {
                        string text = GetFlowDocumentText(this.RichTextBox.Document);
                        return (!string.IsNullOrEmpty(text) && !text.Equals("\r\n"));
                    }
                    else
                    {
                        return this.InkCanvas.Strokes.Count > 0;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Return the inner control of the note (e.g. either an InkCanvas or a RichTextBox).
        /// </summary>
        public FrameworkElement InnerControl
        {
            get
            {
                if (Expanded)
                {
                    object content = ReflectionHelper.GetProperty(Target, "Content");
                    return ReflectionHelper.GetProperty(content, "InnerControl") as FrameworkElement;
                }
                else
                    return new VisualTreeWalker<Button>().FindChildren(Target)[0];
            }
        }

        /// <summary>
        /// Returns Z order of this SN.
        /// </summary>
        /// <remarks>
        /// Higher z-order indicates it is "on top".
        /// </remarks>
        public int ZOrder
        {
            get
            {
                Type IAnnotationComponentType = PresentationFramework.GetType("MS.Internal.Annotations.Component.IAnnotationComponent");
                return (int)IAnnotationComponentType.GetProperty("ZOrder").GetValue(Target, null);
            }
        }

        public string Author
        {
            get
            {
                return ((StickyNoteControl)_stickyNote.Target).GetValue(StickyNoteControl.AuthorProperty) as String;
            }
        }

        /// <summary>
        /// Property controls the minimized/maximized state of a StickyNote.
        /// </summary>
        public bool Expanded
        {
            get
            {
                return Target.IsExpanded;
            }
            set
            {
                Target.IsExpanded = value;
                DispatcherHelper.DoEvents();
            }
        }

        /// <summary>
        /// Get the control that this instance wraps.
        /// </summary>
        public StickyNoteControl Target
        {
            get
            {
                return (StickyNoteControl)_stickyNote.Target;
            }
        }

        /// <summary>
        /// Location of StickyNote in absolute coordinates.
        /// </summary>
        public Point Location
        {
            get
            {
                Rect boundingRect = BoundingRect;
                return new Point(boundingRect.Left, boundingRect.Top);
            }
        }

        /// <summary>
        /// Looks at the various ways that StickyNote represents its state internally, then converts these
        /// into one of the valid StickyNoteState values used in the tests.  
        /// </summary>
        /// <remarks>
        /// Accessing this property will perform a number of tests to try and verify that the state
        /// of the StickyNote is a valid one.  You should be confident that the state that is returned
        /// is an actual representation of the physical/visual state of the StickyNote.
        /// </remarks>
        /// <exception cref="Exception">If something about the internal state is amiss.</exception>
        public StickyNoteState State
        {
            get
            {
                StickyNoteState state;
                bool isStickyNoteSelected = _internalState.IsStickyNoteSelected;
                bool isStickyNoteFocused = _internalState.IsStickyNoteFocused;
                bool isAnchorActive = _internalState.IsAnchorActive;

                if (isStickyNoteFocused)
                {
                    if (isStickyNoteSelected)
                        throw new Exception("StickyNote is both 'selected' and 'focused' at the same time.");
                    if (!isAnchorActive)
                        throw new Exception("StickyNote is focused but anchor is not active.");
                }
                else if (isStickyNoteSelected != isAnchorActive)
                    throw new Exception("StickyNote is Selected='" + isStickyNoteSelected + "' but anchor active = '" + isAnchorActive + "'.");

                if (isStickyNoteFocused)
                    state = StickyNoteState.Active_Focused;
                else if (isStickyNoteSelected || isAnchorActive)
                    state = StickyNoteState.Active_Selected;
                else
                    state = StickyNoteState.Inactive;

                EnsureState(state);

                return state;
            }
        }

        /// <summary>
        /// Controls the size of the StickyNote body.
        /// </summary>
        public Size Size
        {
            set
            {
                Target.Width = value.Width;
                Target.Height = value.Height;
                DispatcherHelper.DoEvents();
            }
        }

        public Rect BoundingRect
        {
            get
            {
                return UIAutomationModule.BoundingRectangle(Target);
            }
        }

        public MenuWrapper Menu
        {
            get
            {
                return _menuWrapper;
            }
        }

        public ContextMenuWrapper ContextMenu
        {
            get
            {
                return _contextMenuWrapper;
            }
        }

        public Thumb TitleThumb
        {
            get
            {
                return FindChildControl(TitleThumbName) as Thumb;
            }
        }

        public Thumb ResizeThumb
        {
            get
            {
                return FindChildControl(ResizeThumbName) as Thumb;
            }
        }

        public Button CloseButton
        {
            get
            {
                return FindChildControl(CloseButtonName) as Button;
            }
        }

        public Button Icon
        {
            get
            {
                return FindChildControl(IconButtonName) as Button;
            }
        }

        public StickyNoteType Type
        {
            get
            {
                return (StickyNoteType)ReflectionHelper.GetField(Target, "_stickyNoteType");
            }
        }

        #endregion

        #region Private Methods

        private string GetFlowDocumentText(FlowDocument doc)
        {
            TextRange range = new TextRange(doc.ContentStart, doc.ContentEnd);
            return range.Text;
        }

        /// <summary>
        /// Try to ensure that the StickyNote and its anchor are actually in the given state.
        /// This invloves Programmatically checking that the colors of the StickyNote caption, anchor
        /// and brackets are correct.
        /// </summary>
        private void EnsureState(StickyNoteState state)
        {
            Color expectedHighlightColor;
            Color expectedMarkerColor;
            switch (state)
            {
                case StickyNoteState.Active_Focused:
                    // Check that StickyNote actually *has* focus.
                    if (!HasFocus)
                        throw new Exception("StickyNoteState is '" + state + "' but control doesn't actually have Focus.");
                    expectedHighlightColor = _internalState.DefaultActiveAnchorColor;
                    expectedMarkerColor = _internalState.DefaultActiveMarkerColor;
                    break;

                case StickyNoteState.Active_Selected:
                    expectedHighlightColor = _internalState.DefaultActiveAnchorColor;
                    expectedMarkerColor = _internalState.DefaultActiveMarkerColor;
                    break;

                case StickyNoteState.Inactive:
                    expectedHighlightColor = _internalState.DefaultAnchorColor;
                    expectedMarkerColor = _internalState.DefaultMarkerColor;
                    break;

                default:
                    throw new InvalidOperationException("Unknown StickyNoteState '" + state + "'.");
            }

            if (!_internalState.AnchorBrush.Equals(expectedHighlightColor))
                throw new Exception("Unexpected anchor color for '" + state + "' state.");
            if (!_internalState.MarkerBrush.Equals(expectedMarkerColor))
                throw new Exception("Unexpected Marker color for '" + state + " state.");
        }

        private void Initialize(Control stickynote, string uniqueId)
        {
            if (!stickynote.GetType().Equals(StickyNote_Type))
                throw new ArgumentException("Control must be of type '" + StickyNote_Type.FullName + "' but was '" + stickynote.GetType().FullName + "'.");

            stickynote.Name = uniqueId;
            _stickyNote = new WeakReference(stickynote);
            _dragModule = new DragModule(this);
            _internalState = new MarkedHighlightWrapper(Target);
            _menuWrapper = new MenuWrapper(this);
            _contextMenuWrapper = new ContextMenuWrapper(this);
        }

        private Control FindChildControl(string name)
        {
            VisualTreeWalker<Control> treeWalker = new VisualTreeWalker<Control>();
            IList<Control> controls = treeWalker.FindChildren(Target);
            foreach (Control control in controls)
            {
                if (control.Name.Equals(name))
                    return control;
            }
            return null;
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// StickyNote has focus if its content has focus, so look at its content.
        /// </summary>
        private bool HasFocus
        {
            get
            {
                return InnerControl.IsFocused;
            }
        }

        private TranslateTransform PositionTranform
        {
            get
            {
                return (TranslateTransform)ReflectionHelper.GetProperty(Target, _positionProperty);
            }
            set
            {
                ReflectionHelper.SetProperty(Target, _positionProperty, value);
            }
        }

        #endregion

        #region Private Fields

        WeakReference _stickyNote;
        MarkedHighlightWrapper _internalState;
        DragModule _dragModule;
        MenuWrapper _menuWrapper;
        ContextMenuWrapper _contextMenuWrapper;

        static Assembly PresentationFramework;
        static Type StickyNote_Type;
        static PropertyInfo StickyNote_Content;
        static FieldInfo StickyNote_Anchor;

        private static string _positionProperty = "PositionTransform";

        private static string TitleThumbName = "PART_TitleThumb";
        private static string ResizeThumbName = "PART_ResizeBottomRightThumb";
        private static string CloseButtonName = "PART_CloseButton";
        private static string IconButtonName = "PART_IconButton";


        #endregion

        /// <summary>
        /// Module that knows how to interact with the menu of a StickyNote.  All
        /// operations are performed using mouse input.
        /// </summary>
        public class MenuWrapper
        {
            #region Constructors

            internal MenuWrapper(StickyNoteWrapper note)
            {
                _noteWrapper = note;
            }

            #endregion

            #region Public Methods

            public MenuItem Target
            {
                get
                {
                    return GetMenuItem(_noteWrapper.Target, ToolsItem);
                }
            }

            /// <summary>
            /// Use mouse to click on Menu and open it.
            /// </summary>
            public void Open()
            {
                UIAutomationModule.MoveToAndClickElement(Target);
                DispatcherHelper.DoEvents(500);
            }

            public MenuItemWrapper Delete
            {
                get { return GetMenuItem(DeleteItem); }
            }
            public MenuItemWrapper Copy
            {
                get { return GetMenuItem(CopyItem); }
            }
            public MenuItemWrapper Paste
            {
                get { return GetMenuItem(PasteItem); }
            }
            public MenuItemWrapper Ink
            {
                get { return GetMenuItem(InkItem); }
            }
            public MenuItemWrapper Select
            {
                get { return GetMenuItem(SelectItem); }
            }
            public MenuItemWrapper Erase
            {
                get { return GetMenuItem(EraseItem); }
            }

            #endregion

            #region Private Methods

            private MenuItemWrapper GetMenuItem(string name)
            {
                Popup menu = MenuPopup;
                // Only open menu if it is not already opened.
                if (menu == null || !menu.IsOpen)
                {
                    Open();
                    menu = MenuPopup;
                }
                return new MenuItemWrapper(GetMenuItem(menu.Child, name));
            }

            /// <summary>
            /// Get a MenuItem with the given header.  Note: menu item must be visible.
            /// </summary>
            private MenuItem GetMenuItem(Visual parent, string name)
            {
                IList<MenuItem> menuItems = new VisualTreeWalker<MenuItem>().FindChildren(parent);
                foreach (MenuItem menuItem in menuItems)
                {
                    if (menuItem.HasHeader && menuItem.Name.Equals(name))
                        return menuItem;
                }
                throw new ArgumentException("Could not find a visible MenuItem with name '" + name + "'.");
            }

            /// <summary>
            /// Returns the Popup control that is the expanded form of the menu.  This popup
            /// contains all the MenuItems of the menu.
            /// </summary>
            private Popup MenuPopup
            {
                get
                {
                    MenuItem toolsMenu = GetMenuItem(_noteWrapper.Target, ToolsItem);
                    IList<Popup> popups = new VisualTreeWalker<Popup>().FindChildren(toolsMenu);
                    if (popups.Count > 0)
                        return popups[0];
                    return null;
                }
            }

            #endregion

            #region Private Fields

            StickyNoteWrapper _noteWrapper;

            // Header names of all the StickyNote menu items.
            //
            public const string ToolsItem = "EditMenuItem";
            public const string DeleteItem = "PART_DeleteMenuItem";
            public const string CopyItem = "PART_CopyMenuItem";
            public const string PasteItem = "PART_PasteMenuItem";
            public const string InkItem = "PART_InkMenuItem";
            public const string SelectItem = "PART_SelectMenuItem";
            public const string EraseItem = "PART_EraseMenuItem";

            #endregion

            public class MenuItemWrapper
            {
                internal MenuItemWrapper(MenuItem item)
                {
                    _item = item;
                }

                /// <summary>
                /// Timing related to when a MenuItem is clicked on and when it is actually
                /// processed is inconsistent, therefore we will wait for a designated period
                /// of time which is hopefully sufficiently long for all necessiary events to
                /// propagate.
                /// </summary>
                public void Execute()
                {
                    UIAutomationModule.MoveToAndClickElement(_item);
                    DispatcherHelper.DoEvents(2000); /* 2 seconds */
                }

                public bool IsEnabled
                {
                    get
                    {
                        return _item.IsEnabled;
                    }
                }

                MenuItem _item;
            }
        }

        public class ContextMenuWrapper
        {
            #region Constructor

            internal ContextMenuWrapper(StickyNoteWrapper note)
            {
                _noteWrapper = note;
            }

            #endregion

            #region Public Methods

            public void Cut(Position menuPosition)
            {
                OpenMenu(menuPosition);
                if (UseAccessKeys)
                    UIAutomationModule.PressKey(System.Windows.Input.Key.T);
                else
                    ClickOnMenuItem(0);
            }

            public void Copy(Position menuPosition)
            {
                OpenMenu(menuPosition);
                if (UseAccessKeys)
                    UIAutomationModule.PressKey(System.Windows.Input.Key.C);
                else
                    ClickOnMenuItem(1);
            }

            public void Paste(Position menuPosition)
            {
                OpenMenu(menuPosition);
                if (UseAccessKeys)
                    UIAutomationModule.PressKey(System.Windows.Input.Key.P);
                else
                    ClickOnMenuItem(2);
            }

            /// <summary>
            /// Open menu at the center of the note.
            /// </summary>
            public void OpenMenu(Position menuPosition)
            {
                Point target;
                Rect targetRect = UIAutomationModule.BoundingRectangle(_noteWrapper.RichTextBox);
                switch (menuPosition)
                {
                    case Position.UpperLeft:
                        target = new Point(targetRect.Left + 15, targetRect.Top + 15);
                        break;
                    case Position.Middle:
                        target = new Point(targetRect.Left + targetRect.Width / 2, targetRect.Top + targetRect.Height / 2);
                        break;
                    default:
                        throw new NotSupportedException();
                }

                UIAutomationModule.MoveTo(target);
                UIAutomationModule.RightMouseClick();
                // Context menu is slow, make sure it has time to open.
                DispatcherHelper.DoEvents(500);
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// If true then use HotKeys to execute the context menu its, otherwise use the mouse.
            /// </summary>
            public bool UseAccessKeys
            {
                set
                {
                    _useAccessKeys = value;
                }
                get
                {
                    return _useAccessKeys;
                }
            }

            #endregion

            #region Private Methods

            private void ClickOnMenuItem(int idx)
            {
                throw new NotImplementedException("Clicking the context menu doesn't work...");
                //IList<Popup> allPopups = new VisualTreeWalker<Popup>().FindChildren(_noteWrapper.Target);
                //if (allPopups.Count != 1)
                //    throw new Exception("Cannot determine which popup is the ContextMenu, there are '" + allPopups.Count + "' popups to choose from.");
                //Popup contextMenu = allPopups[0];
                //while (!contextMenu.IsOpen)
                //    DispatcherHelper.DoEvents();

                //IList<MenuItem> menuItems = new VisualTreeWalker<MenuItem>().FindChildren(contextMenu.Child);
                //if (menuItems.Count != 3)
                //    throw new Exception("Expected 3 menu items but was " + menuItems.Count + ".");
                //if (idx < 0 || idx >= menuItems.Count)
                //    throw new ArgumentException("MenuItem idx must be positive and less than number of menu items.");
                //UIAutomationModule.MoveToAndClickElement(menuItems[idx]);
            }

            #endregion

            #region Fields

            StickyNoteWrapper _noteWrapper;
            bool _useAccessKeys = true;

            #endregion

            /// <summary>
            /// Position of context menu may affect the enablement of its items.  This
            /// enum helps control the location of the ContextMenu without explicitly
            /// re-computing the necessary coordinates each time.
            /// </summary>
            public enum Position
            {
                UpperLeft,
                Middle
            }
        }

        /// <summary>
        /// Wrapper for MarkedHighlightComponent object, which is the object that contains all the state information
        /// for the StickyNote.
        /// </summary>
        class MarkedHighlightWrapper
        {
            #region Constructors

            public MarkedHighlightWrapper(Control stickynote)
            {
                object target = ReflectionHelper.GetField(stickynote, "_anchor");
                Type MarkedHighlightComponentType = typeof(FrameworkElement).Assembly.GetType("MS.Internal.Annotations.Component.MarkedHighlightComponent");
                if (!target.GetType().Equals(MarkedHighlightComponentType))
                    throw new ArgumentException("Target must be of type '" + MarkedHighlightComponentType.FullName + "' but was '" + target.GetType().FullName + "'.");
                _target = new WeakReference(target);
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Return true is StickyNote state variable is "Selected".
            /// </summary>
            public bool IsStickyNoteSelected
            {
                get
                {
                    byte SelectedFlag = (byte)ReflectionHelper.GetField(Target, "SelectedFlag");
                    return (StickyNoteState & SelectedFlag) > 0;
                }
            }
            /// <summary>
            /// Return true is StickyNote state variable is "Focused".
            /// </summary>
            public bool IsStickyNoteFocused
            {
                get
                {
                    byte FocusFlag = (byte)ReflectionHelper.GetField(Target, "FocusFlag");
                    return (StickyNoteState & FocusFlag) > 0;
                }
            }
            /// <summary>
            /// Return true if StickyNote's Anchor's state is "Active".
            /// </summary>
            public bool IsAnchorActive
            {
                get
                {
                    return (bool)ReflectionHelper.GetField(HighlightComponent, "_active");
                }
            }

            /// <summary>
            /// The current color used to draw the highlight part of the anchor.
            /// </summary>
            public Color AnchorBrush
            {
                get
                {
                    DependencyProperty HighlightBrushProperty = (DependencyProperty)ReflectionHelper.GetField(HighlightComponent, "HighlightBrushProperty");
                    return ((SolidColorBrush)HighlightComponent.GetValue(HighlightBrushProperty)).Color;
                }
            }
            public Color DefaultAnchorColor
            {
                get
                {
                    return (Color)ReflectionHelper.GetProperty(HighlightComponent, "DefaultBackground");
                }
            }
            public Color DefaultActiveAnchorColor
            {
                get
                {
                    return (Color)ReflectionHelper.GetProperty(HighlightComponent, "DefaultActiveBackground");
                }
            }
            /// <summary>
            /// The current color used to draw the Marker part of the anchor.
            /// </summary>
            public Color MarkerBrush
            {
                get
                {
                    DependencyProperty MarkerBrushProperty = (DependencyProperty)ReflectionHelper.GetField(Target, "MarkerBrushProperty");
                    return ((SolidColorBrush)Target.GetValue(MarkerBrushProperty)).Color;
                }
            }
            public Color DefaultMarkerColor
            {
                get
                {
                    return (Color)ReflectionHelper.GetField(Target, "DefaultMarkerColor");
                }
            }
            public Color DefaultActiveMarkerColor
            {
                get
                {
                    return (Color)ReflectionHelper.GetField(Target, "DefaultActiveMarkerColor");
                }
            }

            #endregion

            #region Private Properties

            /// <summary>
            /// The target of the wrapper.
            /// </summary>
            private DependencyObject Target
            {
                get
                {
                    return (DependencyObject)_target.Target;
                }
            }

            /// <summary>
            /// The anchor of the SN (represented by a HighlightComponent).
            /// </summary>
            private DependencyObject HighlightComponent
            {
                get
                {
                    return (DependencyObject)ReflectionHelper.GetField(Target, "_highlightAnchor");
                }
            }

            /// <summary>
            /// The actual byte state of the StickyNote as it is represented internally.
            /// </summary>
            private byte StickyNoteState
            {
                get
                {
                    return (byte)ReflectionHelper.GetField(Target, "_state");
                }
            }


            #endregion

            #region Private Variables

            WeakReference _target;

            #endregion
        }
    }

    /// <summary>
    /// Code for dragging a StickyNote around using Mouse input in a way that appears synchrous to the caller.
    /// </summary>
    class DragModule
    {
        #region Constructors

        public DragModule(StickyNoteWrapper snWrapper)
        {
            _sn = snWrapper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Use the mouse to drag this stickynote by grabbing it from the center of its toolbar.
        /// </summary>
        /// <remarks>Accuracy of placement will depend upon whether point is inside page or not.  If
        /// point is outside the page, the stickynote may not get placed perfectly flush with the side.
        /// For best results use points that are inside or as close to page boundry as possible.</remarks>
        /// <param name="dest">Point to move StickyNote to.</param>
        public void Drag(Point dest)
        {
            // Since we drag relative to the center of the toolbar, but want to define our position relative
            // to the top-left corner of the note, we convert the destination to be relative to the drag point
            // to simplify the calculations.
            //
            Vector relativeOffset = Point.Subtract(DragPoint, NotePosition);
            Point relativeDestination = Point.Add(dest, relativeOffset);

            UIAutomationModule.MoveToCenter(_sn.TitleThumb);
            StartMove(new DragParameters(relativeDestination));
        }

        /// <summary>
        /// Use the mouse to drag this stickynote by grabbing it from the center of its toolbar.
        /// </summary>
        /// <param name="delta">Relative distance from current position to drag SN..</param>
        public void Drag(Vector delta)
        {
            Point destination = Point.Add(NotePosition, delta);
            Drag(destination);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Position of the note is relative to its top-left corner.
        /// </summary>
        private Point NotePosition
        {
            get
            {
                return _sn.Location;
            }
        }

        /// <summary>
        /// Position is based on the Center point of the Note's TitleThumb.
        /// </summary>
        private Point DragPoint
        {
            get
            {
                Rect rect = UIAutomationModule.BoundingRectangle(_sn.TitleThumb);
                return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            }
        }

        private Vector ComputeDirection(Point current, Point dest)
        {
            Vector delta = Point.Subtract(dest, current);
            Vector normalizedDelta = Normalize(delta);
            LogStatus("[Drag]: Direction = (" + normalizedDelta.X + ", " + normalizedDelta.Y + ")");
            return normalizedDelta;
        }

        private void DoClick(Point dest)
        {
            UIAutomationModule.MoveToAndClick(dest);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Called if the drag operation is taking too long.  Kills the drag frame throws and exception.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnDragTimeout(object sender, EventArgs args)
        {
            _dragFrame.Continue = false;
            _dragTimer.Stop();
            throw new Exception("Drag operation timed out because it took too long.");
        }

        /// <summary>
        /// Begin dragging SN, do mouse down event, push a DispatcherFrame to cause synchronous 
        /// calling experience, post a task to do the actual move.
        /// </summary>
        private void StartMove(DragParameters parameters)
        {
            Point currentPosition = DragPoint;
            LogStatus("[Drag]: Start, initial position = (" + currentPosition.X + ", " + currentPosition.Y + ") target position = (" + parameters.Target.X + ", " + parameters.Target.Y + ").");

            UIAutomationModule.MoveTo(currentPosition);
            UIAutomationModule.LeftMouseDown();

            QueueIteration(parameters);

            // Timer to timeout if the drag events take too long and mouse is completely controlled.
            _dragTimer = new DispatcherTimer(_dragTimeout, DispatcherPriority.Normal, OnDragTimeout, Application.Current.Dispatcher);
            _dragTimer.Start();

            // Frame to execute drag events in so that it appears synchronous to the caller.
            _dragFrame = new DispatcherFrame();
            _dragFrame.Continue = true;
            Dispatcher.PushFrame(_dragFrame);
        }

        /// <summary>
        /// Do the mouse up event, kill the Frame, and the timeout timer.
        /// </summary>
        private void EndMove()
        {
            LogStatus("[Drag]: End.");
            UIAutomationModule.LeftMouseUp();

            _dragFrame.Continue = false;
            _dragTimer.Stop();
        }

        /// <summary>
        /// Post a dispatcher operation to move the mouse.
        /// </summary>
        private void QueueIteration(DragParameters parameters)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new DispatcherOperationCallback(DoMove),
                parameters);
        }

        /// <summary>
        /// Compute where to move the mouse to or whether we are done.
        /// </summary>
        /// <param name="input">DragParameters of the drag being performed.</param>
        /// <returns>Always null.</returns>
        private object DoMove(object input)
        {
            DragParameters parameters = (DragParameters)input;

            Point currentLocation = DragPoint;

            // Recompute direction at each iteration to prevent skew.
            Vector direction = ComputeDirection(currentLocation, parameters.Target);

            Vector distToTarget = Point.Subtract(parameters.Target, currentLocation);
            double magnitude = DotProduct(distToTarget, direction);
            if (magnitude <= MaximumPrecision)
            {
                EndMove();
            }
            else
            {
                // Move larger distances when we are far away from the target
                // when we get close, move constant small increment.
                double stepSize = (.75 * magnitude);
                if (magnitude < 1)
                    stepSize = .1; // 1 is smallest size supported.
                Vector step = Vector.Multiply(stepSize, direction);
                Point nextLocation = Point.Add(currentLocation, step);

                // Stop if the new location would be past the destination...
                if (VerifyLocation(nextLocation, parameters, direction))
                {
                    LogStatus("[Drag]: to (" + nextLocation.X + ", " + nextLocation.Y + ").");
                    UIAutomationModule.MoveTo(nextLocation);
                    QueueIteration(parameters);
                }
                else
                {
                    EndMove();
                }
            }

            return null;
        }

        /// <summary>
        /// Return true if Location is before the Target location.
        /// </summary>
        /// <param name="location">Position to verify.</param>
        /// <param name="parameters">Drag that is being performed.</param>
        private bool VerifyLocation(Point location, DragParameters parameters, Vector direction)
        {
            // use a buffered compare on high dpi.
            if (Microsoft.Test.Display.Monitor.Dpi.x == 120)
            {
                if (DpiBufferCompare(location, parameters.LastPosition))
                {
                    return false;
                }
            }
            else
            {
                if (Point.Equals(location, parameters.LastPosition))
                {
                    return false;
                }
            }

            parameters.LastPosition = location;
            return DotProduct(Point.Subtract(parameters.Target, location), direction) > 0;
        }

        // Allow at least one pixel buffer on high dpi.
        private bool DpiBufferCompare(Point point1, Point point2)
        {
            bool xDiff = 1 > (point1.X - point2.X) && (point1.X - point2.X) >= 0;
            bool yDiff = 1 > (point1.Y - point2.Y) && (point1.Y - point2.Y) >= 0;

            return (xDiff && yDiff);
        }

        private double DotProduct(Vector a, Vector b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        private Vector Normalize(Vector vect)
        {
            Vector result = Vector.Divide(vect, Math.Sqrt(Math.Pow(vect.X, 2) + Math.Pow(vect.Y, 2)));
            if (result.Y == 1)
                result.X = 0;
            if (result.X == 1)
                result.Y = 0;
            return result;
        }

        private void LogStatus(string msg)
        {
            if (TestSuite.Current != null)
                TestSuite.Current.printStatus(msg);
            else
                Console.WriteLine(msg);
        }

        #endregion

        #region Private Variables

        StickyNoteWrapper _sn;

        DispatcherFrame _dragFrame;
        DispatcherTimer _dragTimer;
        TimeSpan _dragTimeout = new TimeSpan(0, 0, 10);

        double MaximumPrecision = 1e-3;

        class DragParameters
        {
            public DragParameters(Point target)
            {
                Target = target;
            }

            public Point Target;

            /// <summary>
            /// Record of where the last mouse movement was too, if this converges than we know
            /// we should stop.
            /// </summary>
            public Point LastPosition = new Point(double.NaN, double.NaN);
        }

        #endregion
    }
}
