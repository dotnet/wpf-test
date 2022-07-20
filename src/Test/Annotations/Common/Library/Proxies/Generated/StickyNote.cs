// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  *** FILE IS AUTOMATICALLY GENERATED, DO NOT EDIT BY HAND ***
//
//        Generated: 10/6/2005 4:11:54 PM

// Required proxy imports.
using Annotations.Test.Reflection;
using System.Reflection;
using System.Windows.Controls;

// Following are used to access public types that don't have proxies
using StoreContentAction = System.Windows.Annotations.Storage.StoreContentAction;
using StoreContentChangedEventArgs = System.Windows.Annotations.Storage.StoreContentChangedEventArgs;
using StoreContentChangedEventHandler = System.Windows.Annotations.Storage.StoreContentChangedEventHandler;
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


// Delegate specific imports.
using Proxies.System.Windows.Controls;
using System.Collections;
using System.Windows;
using Proxies.MS.Internal.Annotations.Component;
using System;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Proxies.MS.Internal.Annotations;
using Proxies.System.Windows.Annotations;
using System.Windows.Input;
using System.Windows.Ink;
using System.Windows.Data;
using System.Xml;

namespace Proxies.System.Windows.Controls
{
	public class StickyNoteControl : AReflectiveProxy, IAnnotationComponent
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public StickyNoteControl()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}
		public StickyNoteControl(StickyNoteType type)
		: base (new Type[] { typeof(StickyNoteType) }, new object[] { type })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static StickyNoteControl() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected StickyNoteControl(Type[] types, object[] values) : base (types, values) { }
		protected StickyNoteControl(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		virtual public void OnApplyTemplate()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public void AddAttachedAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public void RemoveAttachedAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public void ModifyAttachedAnnotation(IAttachedAnnotation attachedAnnotation, Object previousAttachedAnchor, AttachmentLevel previousAttachmentLevel)
		{
			object [] parameters = new object[3];
			parameters[0] = attachedAnnotation;
			parameters[1] = previousAttachedAnchor;
			parameters[2] = previousAttachmentLevel;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			object [] parameters = new object[1];
			parameters[0] = transform;
			GeneralTransform routedResult = (GeneralTransform) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void OnAuthorUpdated(Object obj, AnnotationAuthorChangedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = obj;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnAnnotationUpdated(Object obj, AnnotationResourceChangedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = obj;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void SetAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void ClearAnnotation()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void UpdateOffsets()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = d;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
		}
		virtual public void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
		{
			object [] parameters = new object[2];
			parameters[0] = oldTemplate;
			parameters[1] = newTemplate;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs args)
		{
			object [] parameters = new object[1];
			parameters[0] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs args)
		{
			object [] parameters = new object[1];
			parameters[0] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void EnsureStickyNoteType()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void DisconnectContent()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void ConnectContent()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public void OnInkEditingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = d;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
		}
		static public void UpdateInkDrawingAttributes(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = d;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
		}
		public void OnTextChanged(Object obj, TextChangedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = obj;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public void OnContextMenuOpening(Object sender, RoutedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = args;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
		}
		public void OnInkCanvasStrokesReplacedEventHandler(Object sender, InkCanvasStrokesReplacedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnInkStrokesChanged(Object sender, StrokeCollectionChangedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void InitStickyNoteControl()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void InitializeEventHandlers()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void ShowBubble(Boolean show)
		{
			object [] parameters = new object[1];
			parameters[0] = show;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public Point GetInitialPosition()
		{
			object [] parameters = new object[0];
			Point routedResult = (Point) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void OnButtonClick(Object sender, RoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void DeleteStickyNote()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnDragCompleted(Object sender, DragCompletedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnDragDelta(Object sender, DragDeltaEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnTitleDragDelta(Double horizontalChange, Double verticalChange)
		{
			object [] parameters = new object[2];
			parameters[0] = horizontalChange;
			parameters[1] = verticalChange;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnResizeDragDelta(Double horizontalChange, Double verticalChange)
		{
			object [] parameters = new object[2];
			parameters[0] = horizontalChange;
			parameters[1] = verticalChange;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnPreviewDeviceDown(Object dc, InputEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = dc;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnLoadedEventHandler(Object sender, RoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void RemoveStickyNoteControlFromAnnotationStore()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void ClearCachedControls()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnInkEditingModeChanged()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnIsExpandedChanged()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public Object TakeFocus(Object notUsed)
		{
			object [] parameters = new object[1];
			parameters[0] = notUsed;
			Object routedResult = (Object) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void GiveUpFocus()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void BringToFront()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void SendToBack()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void InvalidateTransform()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public Object AsyncUpdateAnnotation(Object arg)
		{
			object [] parameters = new object[1];
			parameters[0] = arg;
			Object routedResult = (Object) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void BindContentControlProperties()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void UnbindContentControlProperties()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void StartListenToContentControlEvent()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void StopListenToContentControlEvent()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void StartListenToStrokesEvent(StrokeCollection strokes)
		{
			object [] parameters = new object[1];
			parameters[0] = strokes;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void StopListenToStrokesEvent(StrokeCollection strokes)
		{
			object [] parameters = new object[1];
			parameters[0] = strokes;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void StartListenToStrokeEvent(StrokeCollection strokes)
		{
			object [] parameters = new object[1];
			parameters[0] = strokes;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void StopListenToStrokeEvent(StrokeCollection strokes)
		{
			object [] parameters = new object[1];
			parameters[0] = strokes;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void SetupMenu()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void SetupMenuSeparators(ItemCollection itemCollection, MenuItem item, Binding bind)
		{
			object [] parameters = new object[3];
			parameters[0] = itemCollection;
			parameters[1] = item;
			parameters[2] = bind;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public void OnCommandExecuted(Object sender, ExecutedRoutedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = args;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
		}
		static public void OnQueryCommandEnabled(Object sender, CanExecuteRoutedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = args;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
		}
		public void UpdateInkDrawingAttributes()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "System.Windows.Controls.StickyNoteControl";
		}
		protected override string DelegateAssemblyName()
		{
			return "PresentationFramework";
		}

		//------------------------------------------------------
		//
		//  Properties
		//
		//------------------------------------------------------

		virtual public IList AttachedAnnotations
		{
			get
			{
				return (IList)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public UIElement AnnotatedElement
		{
			get
			{
				return (UIElement)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public PresentationContext PresentationContext
		{
			get
			{
				return (PresentationContext)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		virtual public Int32 ZOrder
		{
			get
			{
				return (Int32)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public TranslateTransform PositionTransform
		{
			get
			{
				return (TranslateTransform)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public Double XOffset
		{
			get
			{
				return (Double)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public Double YOffset
		{
			get
			{
				return (Double)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public Rect StickyNoteBounds
		{
			get
			{
				return (Rect)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Rect PageBounds
		{
			get
			{
				return (Rect)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Boolean IsExpanded
		{
			get
			{
				return (Boolean)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public Boolean IsActive
		{
			get
			{
				return (Boolean)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Boolean IsMouseOverAnchor
		{
			get
			{
				return (Boolean)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public FontFamily CaptionFontFamily
		{
			get
			{
				return (FontFamily)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public Double CaptionFontSize
		{
			get
			{
				return (Double)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public FontStretch CaptionFontStretch
		{
			get
			{
				return (FontStretch)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public FontStyle CaptionFontStyle
		{
			get
			{
				return (FontStyle)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public FontWeight CaptionFontWeight
		{
			get
			{
				return (FontWeight)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public Double PenWidth
		{
			get
			{
				return (Double)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public StickyNoteType StickyNoteType
		{
			get
			{
				return (StickyNoteType)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Button CloseButton
		{
			get
			{
				return (Button)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Button IconButton
		{
			get
			{
				return (Button)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Thumb TitleThumb
		{
			get
			{
				return (Thumb)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public TextBlock TitleLabel
		{
			get
			{
				return (TextBlock)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public ScrollViewer ScrollViewer
		{
			get
			{
				return (ScrollViewer)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Thumb ResizeThumb
		{
			get
			{
				return (Thumb)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public MenuItem EditMenu
		{
			get
			{
				return (MenuItem)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Boolean DefaultIsExpanded
		{
			get
			{
				return (Boolean)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Boolean IsDirty
		{
			get
			{
				return (Boolean)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public Menu Menu
		{
			get
			{
				return (Menu)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public MenuItem DeleteMenuItem
		{
			get
			{
				return (MenuItem)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public MenuItem InkMenuItem
		{
			get
			{
				return (MenuItem)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public MenuItem SelectMenuItem
		{
			get
			{
				return (MenuItem)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public MenuItem EraseMenuItem
		{
			get
			{
				return (MenuItem)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}

		//------------------------------------------------------
		//
		//  Delegate Static Fields
		//
		//------------------------------------------------------

		public static XmlQualifiedName TextSchemaName;
		public static XmlQualifiedName InkSchemaName;
		public static DependencyPropertyKey AuthorPropertyKey;
		public static DependencyProperty AuthorProperty;
		public static DependencyProperty IsExpandedProperty;
		public static DependencyProperty IsActiveProperty;
		public static DependencyProperty IsMouseOverAnchorProperty;
		public static DependencyProperty CaptionFontFamilyProperty;
		public static DependencyProperty CaptionFontSizeProperty;
		public static DependencyProperty CaptionFontStretchProperty;
		public static DependencyProperty CaptionFontStyleProperty;
		public static DependencyProperty CaptionFontWeightProperty;
		public static DependencyProperty PenWidthProperty;
		public static DependencyPropertyKey StickyNoteTypePropertyKey;
		public static DependencyProperty StickyNoteTypeProperty;
		public static RoutedCommand DeleteNoteCommand;
		public static RoutedCommand InkCommand;
		public static DependencyPropertyKey InkEditingModePropertyKey;
		public static DependencyProperty InkEditingModeProperty;
		public static String SchemaNamespace = "http://schemas.microsoft.com/stickynote";

		//------------------------------------------------------
		//
		//  Proxy Static Fields
		//
		//------------------------------------------------------

		//So that static methods can load the correct assembly.
		protected static string staticstatic_DelegateAssembly = "PresentationFramework";

		//------------------------------------------------------
		//
		//  Events
		//
		//------------------------------------------------------


		//------------------------------------------------------
		//
		//  Delegate Non-Static Fields (as properties)
		//
		//------------------------------------------------------

		private PresentationContext _presentationContext
		{
			get { return (PresentationContext) GetField("_presentationContext"); }
			set { SetField("_presentationContext", value); }
		}
		private TranslateTransform _positionTransform
		{
			get { return (TranslateTransform) GetField("_positionTransform"); }
			set { SetField("_positionTransform", value); }
		}
		private IAttachedAnnotation _attachedAnnotation
		{
			get { return (IAttachedAnnotation) GetField("_attachedAnnotation"); }
			set { SetField("_attachedAnnotation", value); }
		}
		private Double _offsetX
		{
			get { return (Double) GetField("_offsetX"); }
			set { SetField("_offsetX", value); }
		}
		private Double _offsetY
		{
			get { return (Double) GetField("_offsetY"); }
			set { SetField("_offsetY", value); }
		}
		private Int32 _zOrder
		{
			get { return (Int32) GetField("_zOrder"); }
			set { SetField("_zOrder", value); }
		}
		private Boolean _isPositionChanged
		{
			get { return (Boolean) GetField("_isPositionChanged"); }
			set { SetField("_isPositionChanged", value); }
		}
		private Boolean _isTextChanged
		{
			get { return (Boolean) GetField("_isTextChanged"); }
			set { SetField("_isTextChanged", value); }
		}
		private Boolean _isInkChanged
		{
			get { return (Boolean) GetField("_isInkChanged"); }
			set { SetField("_isInkChanged", value); }
		}
		private StickyNoteType _stickyNoteType
		{
			get { return (StickyNoteType) GetField("_stickyNoteType"); }
			set { SetField("_stickyNoteType", value); }
		}
		private Boolean _commandReentrancyFlag
		{
			get { return (Boolean) GetField("_commandReentrancyFlag"); }
			set { SetField("_commandReentrancyFlag", value); }
		}
		private Boolean _executeReentrancyFlag
		{
			get { return (Boolean) GetField("_executeReentrancyFlag"); }
			set { SetField("_executeReentrancyFlag", value); }
		}
	}
}
