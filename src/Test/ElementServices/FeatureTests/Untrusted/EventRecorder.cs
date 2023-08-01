// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Implements the EventRecorder and its supporting structures
 *          for filtering and recording events and properties.
 * 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;


using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Records events by attaching handlers to specified events.  Also, records value changes
    /// of specified properties at the time of each event.
    /// </summary>
    public class EventRecorder
    {
        /// <summary>
        /// Construct an EventRecorder.
        /// </summary>
        /// <param name="obj">Root of the tree to search.</param>
        /// <param name="eventFilters">Events to listen to.</param>
        /// <param name="propertyFilters">Properties to inspect.</param>
        public EventRecorder(DependencyObject obj, List<EventFilter> eventFilters, List<PropertyFilter> propertyFilters)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            _treeRoot = obj;

            _eventFilters = new List<EventFilter>(eventFilters);
            _propertyFilters = new List<PropertyFilter>(propertyFilters);

            _eventHandlers[typeof(EventHandler)] = new EventHandler(OnGenericEvent);
            _eventHandlers[typeof(RoutedEventHandler)] = new RoutedEventHandler(OnRoutedEvent);
            _eventHandlers[typeof(KeyEventHandler)] = new KeyEventHandler(OnKeyEvent);
            _eventHandlers[typeof(KeyboardFocusChangedEventHandler)] = new KeyboardFocusChangedEventHandler(OnFocusEvent);
            _eventHandlers[typeof(TextCompositionEventHandler)] = new TextCompositionEventHandler(OnTextCompositionEvent);
            _eventHandlers[typeof(MouseEventHandler)] = new MouseEventHandler(OnMouseEvent);
            _eventHandlers[typeof(MouseButtonEventHandler)] = new MouseButtonEventHandler(OnMouseButtonEvent);
            _eventHandlers[typeof(MouseWheelEventHandler)] = new MouseWheelEventHandler(OnMouseWheelEvent);
            _eventHandlers[typeof(DragEventHandler)] = new DragEventHandler(OnDragEvent);
            _eventHandlers[typeof(GiveFeedbackEventHandler)] = new GiveFeedbackEventHandler(OnFeedbackEvent);
            _eventHandlers[typeof(QueryCursorEventHandler)] = new QueryCursorEventHandler(OnQueryCursorEvent);
            _eventHandlers[typeof(DependencyPropertyChangedEventHandler)] = new DependencyPropertyChangedEventHandler(OnPropertyChangedEvent);
            _eventHandlers[typeof(QueryContinueDragEventHandler)] = new QueryContinueDragEventHandler(OnQueryContinueDragEvent);
            _eventHandlers[typeof(CanExecuteRoutedEventHandler)] = new CanExecuteRoutedEventHandler(OnCanExecuteRoutedEvent);
            _eventHandlers[typeof(ExecutedRoutedEventHandler)] = new ExecutedRoutedEventHandler(OnExecutedRoutedEvent);
            _eventHandlers[typeof(ContextMenuEventHandler)] = new ContextMenuEventHandler(OnContextMenuEvent);
            _eventHandlers[typeof(ToolTipEventHandler)] = new ToolTipEventHandler(OnToolTipEvent);

            // Attach event handlers.
            this._AttachHandlers();
        }

        /// <summary>
        /// Detaches all event handlers from all nodes in the tree.  It is illegal to
        /// call this method when the handlers are not currently attached.
        /// </summary>
        public void DetachHandlers()
        {
            if (!_isAttached)
            {
                throw new InvalidOperationException("Cannot detach the EventRecorder handlers because they are not currently attached.");
            }

            // Attach event handlers.
            _WalkTreeInitialize(_treeRoot, _eventFilters, false);

            _isAttached = false;
        }

        /// <summary>
        /// List of EventRecord items that have been recorded.
        /// </summary>
        public List<EventRecord> RecordedEvents
        {
            get
            {
                return _recordedEvents;
            }
        }

        /// <summary>
        /// Gets and sets whether the recorder is running.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        /// <summary>
        /// Gets and sets whether the recorder should ignore input state when recording events.
        /// </summary>
        public bool IgnoreInputState
        {
            get { return _ignoreInputState; }
            set { _ignoreInputState = value; }
        }

        /// <summary>
        /// Gets and sets whether the recorder should ignore EventArgs when recording events.
        /// </summary>
        public bool IgnoreEventArgs
        {
            get { return _ignoreEventArgs; }
            set { _ignoreEventArgs = value; }
        }

        /// <summary>
        /// Attaches event handlers to all nodes in the tree based on the EventFilters.
        /// It is illegal to call this method when the handlers are currently attached.
        /// </summary>
        private void _AttachHandlers()
        {
            if (_isAttached)
            {
                throw new InvalidOperationException("Cannot attach the EventRecorder handlers because they are currently attached.");
            }

            // Attach event handlers.
            _WalkTreeInitialize(_treeRoot, _eventFilters, true);

            _isAttached = true;
        }

        /// <summary>
        /// Attaches handlers for the specified events on the given tree.
        /// </summary>
        /// <param name="obj">Current root of tree to attach to.</param>
        /// <param name="filters">Events to filter on.</param>
        /// <param name="addHandler">OK to add event handler to this item?</param>
        /// <remarks>
        /// Uses recursion.
        /// </remarks>
        private void _WalkTreeInitialize(DependencyObject obj, List<EventFilter> filters, bool addHandler)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            // Attach event handlers to current node.
            _AttachEventHandlersToNode(obj, filters, addHandler);

#if TARGET_NET3_5
            // Find Viewport2DVisual3D children and UIElement3Ds.
            if (obj is Viewport3D)
            {
                foreach (Visual3D v3d in ((Viewport3D)obj).Children)
                {
                    _Walk3DTreeInitialize(v3d, filters, addHandler);
                }
            }
#endif
            // Walk to logical children.
            List<DependencyObject> children = TreeHelper.GetChildren(obj);

            for (int i = 0; i < children.Count; i++)
            {
                _WalkTreeInitialize(children[i], filters, addHandler);
            }
        }

#if TARGET_NET3_5
        /// <summary>
        /// Helper for _WalkTreeInitialize to find UIElement3Ds and ViewportVisual3D Children.
        /// </summary>
        /// <param name="v3d"></param>
        /// <param name="filters">Events to filter on.</param>
        /// <param name="addHandler">OK to add event handler to this item?</param>
        /// <remarks>
        /// Uses recursion.
        /// </remarks>
        private void _Walk3DTreeInitialize(Visual3D v3d, List<EventFilter> filters, bool addHandler)
        {
            if (v3d == null)
            {
                throw new ArgumentNullException("obj");
            }

            // Attach event handlers to ModelUIElement3D and check its Visual3D children.
            ModelUIElement3D modelElement = v3d as ModelUIElement3D;
            if (modelElement != null)
            {
                _AttachEventHandlersToNode(modelElement, filters, addHandler);

                return;
            }

            // Attach event handlers to ContainerUIElement3D and check its Visual3D children.
            ContainerUIElement3D containerElement = v3d as ContainerUIElement3D;
            if (containerElement != null)
            {
                _AttachEventHandlersToNode(containerElement, filters, addHandler);

                foreach (Visual3D childVisual in containerElement.Children)
                {
                    _Walk3DTreeInitialize(childVisual, filters, addHandler);
                }

                return;
            }

            // Check 2D children of ViewportVisual3D
            Viewport2DVisual3D viewportVisual3D = v3d as Viewport2DVisual3D;
            if (viewportVisual3D != null && viewportVisual3D.Visual != null)
            {
                _WalkTreeInitialize((DependencyObject)viewportVisual3D.Visual, filters, addHandler);

                return;
            }

            // Check children of ModelVisual3D.
            ModelVisual3D modelVisual = v3d as ModelVisual3D;
            if (modelVisual != null)
            {
                foreach (Visual3D childVisual in modelVisual.Children)
                {
                    _Walk3DTreeInitialize(childVisual, filters, addHandler);
                }
            }
        }
#endif

        /// <summary>
        /// Attaches handlers for the specified events to the given node.
        /// </summary>
        /// <param name="obj">Current node of tree to attach to.</param>
        /// <param name="filters">Events to filter on.</param>
        /// <param name="addHandler">OK to add event handler to this item?</param>
        private void _AttachEventHandlersToNode(DependencyObject obj, List<EventFilter> filters, bool addHandler)
        {
            string name = TreeHelper.GetNodeId(obj);

            foreach (EventFilter eventFilter in filters)
            {
                // Skip event if it is element-specific and the current object (obj)
                // is not the one.
                if (!String.IsNullOrEmpty(eventFilter.ElementName) && !eventFilter.ElementName.Equals(name))
                    continue;

                // 

                // Look for matching CLR event first if we don't need to listen for the event when handled.
                bool isAttached = false;
                if (!eventFilter.HandledEventsToo)
                {
                    if (eventFilter.UseStaticMethod)
                    {
                        isAttached = _AttachWithStaticMethod(eventFilter, obj, addHandler);
                    }
                    else
                    {
                        isAttached = _AttachToClrEvent(eventFilter, obj, addHandler);
                    }
                }
                else
                {
                    // Look for matching RoutedEvent.
                    isAttached = _AttachToRoutedEvent(eventFilter, obj, addHandler);
                }

                // Throw exception if we didn't successfully add a handler.
                if (!isAttached)
                {
                    throw new ArgumentException("Cannot handle event '" + eventFilter.Name + "'. The event was not found.");
                }
            }
        }

        private string _ConvertEventNameToStaticMethod(string eventName, bool addHandler)
        {
            string methodName = eventName;

            if (addHandler)
            {
                methodName = "Add" + methodName + "Handler";
            }
            else
            {
                methodName = "Remove" + methodName + "Handler";
            }

            return methodName;
        }

        private bool _AttachWithStaticMethod(EventFilter eventFilter, object obj, bool addHandler)
        {
            string eventName = eventFilter.Name;
            string methodName = _ConvertEventNameToStaticMethod(eventName, addHandler);

            MethodInfo methodInfo = _FindRoutedEventStaticMethod(methodName);

            if (methodInfo == null)
            {
                return false;
            }

            ParameterInfo[] parameterInfo = methodInfo.GetParameters();
            Delegate eventHandler = _eventHandlers[parameterInfo[1].ParameterType];

            _ValidateFilter(eventFilter, eventHandler);

            methodInfo.Invoke(obj, new object[] { (DependencyObject)obj, eventHandler });

            return true;
        }

        // Validate that the event is a RoutedEvent if MarkHandled is set to true.
        private void _ValidateFilter(EventFilter eventFilter, Delegate eventHandler)
        {
            if (eventFilter.MarkHandled)
            {
                ParameterInfo[] parameters = eventHandler.Method.GetParameters();
                if (parameters.Length >= 2 && !typeof(RoutedEventArgs).IsAssignableFrom(parameters[1].ParameterType))
                {
                    throw new ArgumentException("Cannot mark Handled property for event '" + eventFilter.Name + "' because it is not a RoutedEvent.");
                }

                _eventsToHandle.Add(eventFilter.Name);
            }
        }

        /// <summary>
        /// Attach or detach event handlers for an element, using the CLR eventing API.
        /// </summary>
        /// <param name="eventFilter">Event filter object.</param>
        /// <param name="obj">Object/node to attach event handlers to.</param>
        /// <param name="addHandler">True to attach, false to detach.</param>
        /// <returns>Was this event successfully attached/detached?</returns>
        /// <remarks>
        /// If the event is a commanding event (i.e., it belongs to the CommandBinding class),
        /// we attach a handler to all command bindings on the node.
        /// </remarks>
        private bool _AttachToClrEvent(EventFilter eventFilter, object obj, bool addHandler)
        {
            string eventName = eventFilter.Name;
            Type nodeType = obj.GetType();
            Type commandBindingType = typeof(CommandBinding);

            // Look for matching CLR event on element.
            EventInfo eventInfo = nodeType.GetEvent(eventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (eventInfo == null)
            {
                // Not found on event ... check the CommandBinding class.
                eventInfo = commandBindingType.GetEvent(eventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (eventInfo == null)
                {
                    return false;
                }
            }

            // Look up usable handler on element from table.
            Type handlerType = eventInfo.EventHandlerType;
            if (!_eventHandlers.ContainsKey(handlerType))
            {
                throw new ArgumentException("Cannot handle event '" + eventName + "'. A matching handler has not been created.");
            }

            // Retrieve event handler from table.
            Delegate eventHandler = _eventHandlers[handlerType];
            if (eventHandler == null)
                return false;

            _ValidateFilter(eventFilter, eventHandler);

            // Attach event handler to our element.
            if (eventInfo.DeclaringType == commandBindingType)
            {
                // This is a commanding event ... attach to the command bindings on our element, not the element.

                PropertyInfo propInfo = nodeType.GetProperty("CommandBindings", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
                CommandBindingCollection objCommandBindingCollection = (propInfo.GetValue(obj, null)) as CommandBindingCollection;

                if (objCommandBindingCollection != null)
                {
                    // Element has command bindings ... attach to all of them.
                    foreach (CommandBinding existingCommandBinding in objCommandBindingCollection)
                    {
                        if (addHandler)
                            eventInfo.AddEventHandler(existingCommandBinding, eventHandler);
                        else
                            eventInfo.RemoveEventHandler(existingCommandBinding, eventHandler);
                    }
                }
            }
            else
            {
                // This is a non-commanding event ... just attach to the element

                if (addHandler)
                    eventInfo.AddEventHandler(obj, eventHandler);
                else
                    eventInfo.RemoveEventHandler(obj, eventHandler);
            }

            // Handlers have been attached/detached.
            return true;
        }

        /// <summary>
        /// Attach or detach event handlers for an element, using the WPF eventing API.
        /// </summary>
        /// <param name="eventFilter">Event filter object.</param>
        /// <param name="obj">Object/node to attach event handlers to.</param>
        /// <param name="addHandler">True to attach, false to detach.</param>
        /// <returns>Was this event successfully attached/detached?</returns>
        private bool _AttachToRoutedEvent(EventFilter eventFilter, object obj, bool addHandler)
        {
            // Look for matching RoutedEvent.
            Type nodeType = obj.GetType();
#if TARGET_NET3_5
            if (!nodeType.IsSubclassOf(typeof(UIElement)) && !nodeType.IsSubclassOf(typeof(UIElement3D)) && !nodeType.IsSubclassOf(typeof(ContentElement)))
                return false;
#else 
            if (!nodeType.IsSubclassOf(typeof(UIElement)) && !nodeType.IsSubclassOf(typeof(ContentElement)))
                return false;
#endif
            string eventName = eventFilter.Name;
            FieldInfo fieldInfo = _FindRoutedEventField(eventName, obj);
            if (fieldInfo == null)
                return false;

            Delegate routedEventHandler = _eventHandlers[typeof(RoutedEventHandler)];

            _ValidateFilter(eventFilter, routedEventHandler);

            if (addHandler)
            {
                MethodInfo methodInfo = nodeType.GetMethod(("AddHandler"), new Type[] { fieldInfo.FieldType, typeof(RoutedEventHandler), typeof(bool) });
                methodInfo.Invoke(obj, new object[] { fieldInfo.GetValue(obj), routedEventHandler, true });
            }
            else
            {
                MethodInfo methodInfo = nodeType.GetMethod(("RemoveHandler"), new Type[] { fieldInfo.FieldType, typeof(RoutedEventHandler) });
                methodInfo.Invoke(obj, new object[] { fieldInfo.GetValue(obj), routedEventHandler });
            }

            // Handlers have been attached/detached.
            return true;
        }

        /// <summary>
        /// Finds a routed event on a given node or on certain predefined types. 
        /// </summary>
        /// <param name="eventName">Name of event to find.</param>
        /// <param name="obj">Node to search on.</param>
        /// <returns>Field information on the routed event.</returns>
        /// <remarks>
        /// The node is searched first for the event.
        /// If not found there, we search the following classes in the following order: Mouse, Keyboard, CommandManager.
        /// </remarks>
        private FieldInfo _FindRoutedEventField(string eventName, object obj)
        {
            Type mouseType = typeof(Mouse);
            Type keyboardType = typeof(Keyboard);
            Type commandManagerType = typeof(CommandManager);
            Type nodeType = obj.GetType();

            string routedEventName = eventName + "Event";

            FieldInfo fieldInfo = nodeType.GetField(routedEventName, BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null)
                return fieldInfo;

            fieldInfo = mouseType.GetField(routedEventName, BindingFlags.Static | BindingFlags.Public);
            if (fieldInfo != null)
                return fieldInfo;

            fieldInfo = keyboardType.GetField(routedEventName, BindingFlags.Static | BindingFlags.Public);
            if (fieldInfo != null)
                return fieldInfo;

            fieldInfo = commandManagerType.GetField(routedEventName, BindingFlags.Static | BindingFlags.Public);
            if (fieldInfo != null)
                return fieldInfo;

            return null;
        }

        /// <summary>
        /// Finds a static method on certain predefined types for adding a RoutedEvent to a node. 
        /// </summary>
        /// <param name="methodName">Name of method.</param>
        /// <returns>Method information.</returns>
        /// <remarks>
        /// We search the following classes in the following order: Mouse, Keyboard, CommandManager.
        /// </remarks>
        private MethodInfo _FindRoutedEventStaticMethod(string methodName)
        {
            Type mouseType = typeof(Mouse);
            Type keyboardType = typeof(Keyboard);
            Type commandManagerType = typeof(CommandManager);

            MethodInfo methodInfo = mouseType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            if (methodInfo != null)
                return methodInfo;

            methodInfo = keyboardType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            if (methodInfo != null)
                return methodInfo;

            methodInfo = commandManagerType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            if (methodInfo != null)
                return methodInfo;

            return null;
        }

        #region Event Handlers

        private void OnPropertyChangedEvent(object sender, DependencyPropertyChangedEventArgs args)
        {
            OnEvent(sender, new DependencyPropertyChangedEventArgsWrapper(args), "PropertyChangedEvent");
        }
        private void OnKeyEvent(object sender, KeyEventArgs args)
        {
            OnEvent(sender, args, "KeyEvent");
        }
        private void OnFocusEvent(object sender, KeyboardFocusChangedEventArgs args)
        {
            OnEvent(sender, args, "FocusEvent");
        }
        private void OnTextCompositionEvent(object sender, TextCompositionEventArgs args)
        {
            OnEvent(sender, args, "TextCompositionEvent");
        }
        private void OnMouseEvent(object sender, MouseEventArgs args)
        {
            OnEvent(sender, args, "MouseEvent");
        }
        private void OnMouseButtonEvent(object sender, MouseButtonEventArgs args)
        {
            OnEvent(sender, args, "MouseButtonEvent");
        }
        private void OnMouseWheelEvent(object sender, MouseWheelEventArgs args)
        {
            OnEvent(sender, args, "MouseWheelEvent");
        }
        private void OnQueryCursorEvent(object sender, QueryCursorEventArgs args)
        {
            OnEvent(sender, args, "QueryCursorEvent");
        }
        private void OnDragEvent(object sender, DragEventArgs args)
        {
            OnEvent(sender, args, "DragEvent");
        }
        private void OnQueryContinueDragEvent(object sender, QueryContinueDragEventArgs args)
        {
            OnEvent(sender, args, "QueryContinueDragEvent");
        }
        private void OnFeedbackEvent(object sender, GiveFeedbackEventArgs args)
        {
            OnEvent(sender, args, "FeedbackEvent");
        }
        private void OnRoutedEvent(object sender, RoutedEventArgs args)
        {
            OnEvent(sender, args, "RoutedEvent");
        }
        private void OnCanExecuteRoutedEvent(object sender, CanExecuteRoutedEventArgs args)
        {
            OnEvent(sender, args, "CanExecuteRoutedEvent");

            // Allow Executed event to be handled.
            args.CanExecute = true;
        }
        private void OnExecutedRoutedEvent(object sender, ExecutedRoutedEventArgs args)
        {
            OnEvent(sender, args, "ExecutedRoutedEvent");
        }
        private void OnContextMenuEvent(object sender, ContextMenuEventArgs args)
        {
            OnEvent(sender, args, "ContextMenuEvent");
        }
        private void OnToolTipEvent(object sender, ToolTipEventArgs args)
        {
            OnEvent(sender, args, "ToolTipEvent");
        }
        private void OnGenericEvent(object sender, EventArgs args)
        {
            OnEvent(sender, args, "GenericEvent");
        }

        // Common handler for events. Records the event for future reference.
        private void OnEvent(object sender, EventArgs args, string handlerName)
        {
            // If currently disabled, ignore event.
            if (!_isEnabled)
            {
                return;
            }

            //
            // Mark event as Handled.
            //
            if (args is RoutedEventArgs)
            {
                RoutedEventArgs routedArgs = (RoutedEventArgs)args;
                if (_eventsToHandle.Contains(routedArgs.RoutedEvent.Name))
                {
                    routedArgs.Handled = true;
                }
            }

            //
            // Construct a record of the event.
            //
            string senderName = _GetElementName(sender);

            string eventName = "";
            if (args is RoutedEventArgs)
                eventName = ((RoutedEventArgs)args).RoutedEvent.Name;
            else if (args is DependencyPropertyChangedEventArgsWrapper)
                eventName = ((DependencyPropertyChangedEventArgsWrapper)args).Property.Name + "Changed";
            else
                eventName = handlerName;

            EventRecord eventRecord = new EventRecord();
            eventRecord.Name = eventName;
            eventRecord.SenderName = senderName;

            // Store key state.
            if (!this.IgnoreInputState)
            {
                for (int i = 1; i < 256; i++)
                {
                    Key key = (Key)i;
                    if (Keyboard.IsKeyDown(key))
                    {
                        eventRecord.DownKeys.Add(key);
                    }
                }
            }

            // Store event args.
            if (!this.IgnoreEventArgs && args != null)
            {
                Type type = args.GetType();

                EventArgsRecord argsRecord = new EventArgsRecord();
                argsRecord.Type = type;
                PropertyInfo[] props = type.GetProperties();
                foreach (PropertyInfo pi in props)
                {
                    if (pi.CanRead)
                    {
                        switch (pi.Name)
                        {
                            case "Timestamp":
                                // do nothing - exclude this property
                                break;
                            case "Source":
                            case "OriginalSource":
                                string elementName = _GetElementName(pi.GetValue(args, null));
                                argsRecord.SetArg(pi.Name, elementName);
                                break;
                            default:
                                object val = pi.GetValue(args, null);
                                val = val == null ? "null" : val.ToString();
                                argsRecord.SetArg(pi.Name, val);
                                break;
                        }
                    }
                }

                eventRecord.EventArgs = argsRecord;
            }

            foreach (PropertyFilter propertyFilter in _propertyFilters)
            {
                // Adds a new PropertyRecord to the EventRecord's property list
                // if the property value has changed from the previous event.
                _AddPropertyRecordToEventRecord(sender, senderName, eventRecord, propertyFilter);
            }

            //
            // Store the event record.
            //
            _recordedEvents.Add(eventRecord);
        }

        // Returns the name of an element.
        // If the element isn't named, returns the element's type name.
        private string _GetElementName(object element)
        {
            string elementName = "";

            if (element is DependencyObject)
                elementName = TreeHelper.GetNodeId((DependencyObject)element);

            if (String.IsNullOrEmpty(elementName))
            {
                if (element != null)
                    elementName = element.GetType().Name;
                else
                    elementName = "null";
            }

            return elementName;
        }

        // Adds a new PropertyRecord to the EventRecord's property list
        // if the property value has changed from the previous event.
        private void _AddPropertyRecordToEventRecord(object sender, string senderName, EventRecord eventRecord, PropertyFilter propertyFilter)
        {
            object obj = null;
            string elementName = propertyFilter.ElementName;

            //
            // Get current value of property on element.
            // The element is specified by the property filter 
            // or is the sender by default.

            if (String.IsNullOrEmpty(elementName))
            {
                obj = sender;
                elementName = senderName;
            }
            else
            {
                obj = TreeHelper.FindNodeById(_treeRoot, propertyFilter.ElementName);
            }

            if (obj == null)
            {
                throw new Microsoft.Test.TestSetupException("Could not find element '" + elementName + "'.");
            }

            Type type = obj.GetType();

            PropertyInfo propertyInfo = type.GetProperty(propertyFilter.Name, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo == null)
            {
                throw new Microsoft.Test.TestSetupException("Could not get property '" + propertyFilter.Name + "' on sender '" + propertyFilter.ElementName + "'.");
            }

            string currentValue = propertyInfo.GetValue(obj, null).ToString();

            //
            // Add a new PropertyRecord to the EventRecord's property list
            // if the property value has changed from the previous event.
            //

            // Get a list of PropertyRecords for the element.
            Dictionary<string, PropertyRecord> cache = _GetPropertyRecords(elementName);

            // If the current value is different than the old value,
            // add it to the EventRecord's property list, and reset
            // the value in our value tracker.
            PropertyRecord oldRecord = null;

            if (cache.ContainsKey(propertyFilter.Name))
                oldRecord = cache[propertyFilter.Name];

            if (oldRecord == null || oldRecord.Value != currentValue)
            {
                if (oldRecord != null)
                    cache.Remove(propertyFilter.Name);

                PropertyRecord record = new PropertyRecord();
                record.Element = elementName;
                record.Name = propertyFilter.Name;
                record.Value = currentValue;

                // Add the new PropertyRecord to the EventRecord's property list.
                eventRecord.PropertyRecords.Add(record);

                // Add the PropertyRecord to the list of recorded properties
                // so we can detect when the property changes in the future.
                cache.Add(propertyFilter.Name, record);
            }
        }

        private Dictionary<string, PropertyRecord> _GetPropertyRecords(string elementName)
        {
            Dictionary<string, PropertyRecord> dictionary = null;

            if (_propertyCache.ContainsKey(elementName))
                dictionary = _propertyCache[elementName];

            if (dictionary == null)
            {
                dictionary = new Dictionary<string, PropertyRecord>();
                _propertyCache.Add(elementName, dictionary);
            }

            return dictionary;
        }

        #endregion

        private DependencyObject _treeRoot;
        private List<EventFilter> _eventFilters;
        private List<PropertyFilter> _propertyFilters;
        private IDictionary<Type, Delegate> _eventHandlers = new Dictionary<Type, Delegate>();
        private List<string> _eventsToHandle = new List<string>();
        private List<EventRecord> _recordedEvents = new List<EventRecord>();
        private bool _isAttached = false;
        private bool _isEnabled = false;
        private bool _ignoreInputState = true;
        private bool _ignoreEventArgs = true;
        private Dictionary<string, Dictionary<string, PropertyRecord>> _propertyCache = new Dictionary<string, Dictionary<string, PropertyRecord>>();
    }
}

