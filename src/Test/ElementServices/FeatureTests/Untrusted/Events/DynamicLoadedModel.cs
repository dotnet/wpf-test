// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 4 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/parser/ContentModel.cs $
********************************************************************/

using System;
using System.Collections;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;

using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// DynamicLoadedModel class. This is to test Loaded/Unloaded events on 
    /// FrameworkElement and FrameworkContentElement.
    /// 
    /// Here is a brief description of what this model is all about:
    /// 
    /// We create three objects: a surface (we use Window), a parent node, and a child 
    /// node (the nodes could be FE or FCE).
    ///
    /// Then, we connect them in different ways: link parent to surface first; 
    /// link child to parent first, etc.
    ///
    /// We attach a Loaded or Unloaded listener: attach to parent; attach to child.
    ///
    /// We detach a Loaded or Unloaded listener:  detach from parent, detach from child.
    ///
    /// We disconnect the objects in different ways: unlink parent from surface first; 
    /// unlink child from parent first, etc.
    ///
    /// Finally, all those things can be mixed in any order.
    /// There are actions in our state-based model to achieve the above mix:  
    /// e.g AttachLoadedListener(parent/child), AttachUnloadedListener(parent/child),
    /// LinkChildToParent, LinkParentToSurface, UnlinkChildFromParent, etc. etc.
    /// </summary>
    /// 
    [Model(@"FeatureTests\ElementServices\DynamicLoadedModel_AllTransitions.xtc", 0, @"Events\Loaded\DynamicModel", TestCaseSecurityLevel.FullTrust, "DynamicLoadedModel", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\DynamicLoadedModel_AllTransitions.xtc", 1, @"Events\Loaded\DynamicModel", TestCaseSecurityLevel.PartialTrust, "DynamicLoadedModel", Timeout=180)]
    public class DynamicLoadedModel : CoreModel 
    {

        #region Private Data
        private static EventFireFlags s_flags;
        private static bool s_parentInitialized;
        private static bool s_childInitialized;
        private Window _surface;
        private Custom_FrameworkElement _parent;
        private DependencyObject _child; // Could be an FE or an FCE
        private Custom_FrameworkElement _childAsFE;
        private Custom_FrameworkContentElement _childAsFCE;
        private static RoutedEventHandler s_parentLoadedEventHandler;
        private static RoutedEventHandler s_childLoadedEventHandler;
        private static RoutedEventHandler s_parentUnloadedEventHandler;
        private static RoutedEventHandler s_childUnloadedEventHandler;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public DynamicLoadedModel()
            : base()
        {
            CoreLogger.LogStatus( "Constructor" );

            Name = "DynamicLoadedModel";

            //Attach Event Handlers
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);

            //Add Action Handlers
            AddAction("CreateElements", new ActionHandler(CreateElements));
            AddAction("AttachLoadedListener", new ActionHandler(AttachLoadedListener));
            AddAction("DetachLoadedListener", new ActionHandler(DetachLoadedListener));
            AddAction("AttachUnloadedListener", new ActionHandler(AttachUnloadedListener));
            AddAction("DetachUnloadedListener", new ActionHandler(DetachUnloadedListener));
            AddAction("LinkChildToParent", new ActionHandler(LinkChildToParent));
            AddAction("LinkParentToSurface", new ActionHandler(LinkParentToSurface));
            AddAction("UnlinkChildFromParent", new ActionHandler(UnlinkChildFromParent));
            AddAction("UnlinkParentFromSurface", new ActionHandler(UnlinkParentFromSurface)); 
        }

        private void ClearFlags()
        {
            s_flags = 0;

            // We don't clear the (Parent/Child)Initialized flags, since once an
            // element is initialized, it stays like that.
        }

        private static void CheckIfSet(EventFireFlags value)
        {
            if ((s_flags & value) == 0)
            {
                throw new Microsoft.Test.TestValidationException(value.ToString() + " was supposed to be true, but it's not.");
            }
            else
            {
                CoreLogger.LogStatus(value.ToString() + " is true, as expected.");
            }
        }

        private static void CheckNotSet(EventFireFlags value)
        {
            if ((s_flags & value) != 0)
            {
                throw new Microsoft.Test.TestValidationException(value.ToString() + " was supposed to be false, but it's true.");
            }
            else
            {
                CoreLogger.LogStatus(value.ToString() + " is false, as expected.");
            }
        }

        /// <summary>
        /// Sets the Model as necessary to begin the case with the given State
        /// </summary>
        /// <remarks>
        /// Attached to OnBeginCase event which is fired by the Traversal
        /// before a new case begins
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The Initial State in a StateEventArgs</param>
        private void OnBeginCase_Handler(object sender, StateEventArgs e)
        {
            // Prepare the event handlers.
            s_parentLoadedEventHandler = new RoutedEventHandler(ParentLoadedEventHandlerMethod);
            s_childLoadedEventHandler = new RoutedEventHandler(ChildLoadedEventHandlerMethod);
            s_parentUnloadedEventHandler = new RoutedEventHandler(ParentUnloadedEventHandlerMethod);
            s_childUnloadedEventHandler = new RoutedEventHandler(ChildUnloadedEventHandlerMethod);

            // Set the various flags to their initial values.
            ClearFlags();

            // ClearFlags() doesn't touch (Parent/Child)Initialized flags, so we set
            // them separately.
            s_parentInitialized = false;
            s_childInitialized = false;
        }

        /// <summary>
        /// Finishes the Model as necessary to end the case.
        /// </summary>
        /// <remarks>
        /// Attached to OnEndCase event which is fired by the Traversal
        /// after a case ends
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The End State in a StateEventArgs</param>
        private void OnEndCase_Handler(object sender, StateEventArgs e)
        {
            // Detach event handlers for LayoutUpdated and Initialized events.
            if(_childAsFE != null)
            {
                _childAsFE.Initialized -= new EventHandler(ChildInitializedEventHandlerMethod);
                _childAsFE.LayoutUpdated -= new EventHandler(ChildLayoutUpdatedEventHandlerMethod);
            }
            else
            {
                _childAsFCE.Initialized -= new EventHandler(ChildInitializedEventHandlerMethod);
            }   
        }

        #region CreateElements
        /// <summary>
        /// Handler for CreateElements.
        /// Here we create surface, parent and child.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool CreateElements(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Parameters: " + inParameters.ToString());
            CoreLogger.LogStatus("Expected end state: " + GetStateString(endState));

            // Create the surface.
            _surface = new Window();
            _surface.Show();

            // Create parent and child
            // Also attach event handlers for LayoutUpdated and Initialized events.
            switch(inParameters["Child_type"])
            {
                case "FrameworkElement":
                    _parent = new Custom_FrameworkElement();
                    _child = new Custom_FrameworkElement();
                    (_child as Custom_FrameworkElement).Initialized += new EventHandler(ChildInitializedEventHandlerMethod);
                    (_child as Custom_FrameworkElement).LayoutUpdated += new EventHandler(ChildLayoutUpdatedEventHandlerMethod);
                    break;
        
                case "FrameworkContentElement":
                    _parent = new Custom_FrameworkElement_With_IContentHost();
                    _child = new Custom_FrameworkContentElement();
                    (_child as Custom_FrameworkContentElement).Initialized += new EventHandler(ChildInitializedEventHandlerMethod);
                    break;
            }        
            _childAsFE = _child as Custom_FrameworkElement;        
            _childAsFCE = _child as Custom_FrameworkContentElement;
            
            _parent.Initialized += new EventHandler(ParentInitializedEventHandlerMethod);
            _parent.LayoutUpdated += new EventHandler(ParentLayoutUpdatedEventHandlerMethod);

            CoreLogger.LogStatus("");
            return true;
        }
        #endregion CreateElements

        #region LinkParentToSurface
        /// <summary>
        /// Handler for LinkParentToSurface
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool LinkParentToSurface(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Expected end state: " + GetStateString(endState));

            ClearFlags();

            CoreLogger.LogStatus("Linking parent to surface.");
            _surface.Content = _parent;

            // Wait till events are done firing.
            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);

            // Verification.
            // Loaded event should have been fired on parent, as long as 
            //   it has an event handler
            if (endState["LoadedListenerOnParent"] == "True")
            {
                CheckIfSet(EventFireFlags.LoadedFiredOnParent);
            }
            // Loaded event should have been fired on child, if and only if
            //   - child is linked to parent
            //   - child has an event handler attached
            if ((endState["ChildLinkedToParent"] == "True") &&
                (endState["LoadedListenerOnChild"] == "True"))
            {
                CheckIfSet(EventFireFlags.LoadedFiredOnChild);
            }
            else
            {
                CheckNotSet(EventFireFlags.LoadedFiredOnChild);
            }

            // Unloaded events should not be fired.
            CheckNotSet(EventFireFlags.UnloadedFiredOnParent);
            CheckNotSet(EventFireFlags.UnloadedFiredOnChild);

            CoreLogger.LogStatus("");
            return true;
        }
        #endregion LinkParentToSurface

        #region UnlinkParentFromSurface
        /// <summary>
        /// Handler for UnlinkParentFromSurface
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool UnlinkParentFromSurface(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Expected end state: " + GetStateString(endState));

            ClearFlags();

            CoreLogger.LogStatus("Unlinking parent from surface.");
            _surface.Content = null;

            // Wait till events are done firing.
            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);

            // Verification.
            // Unloaded event should have been fired on parent, as long as 
            //   it has an event handler
            if (endState["UnloadedListenerOnParent"] == "True")
            {
                CheckIfSet(EventFireFlags.UnloadedFiredOnParent);
            }
            // Unloaded event should have been fired on child, if and only if
            //   - child is linked to parent
            //   - child has an event handler attached
            if ((endState["ChildLinkedToParent"] == "True") &&
                (endState["UnloadedListenerOnChild"] == "True"))
            {
                CheckIfSet(EventFireFlags.UnloadedFiredOnChild);
            }
            else
            {
                CheckNotSet(EventFireFlags.UnloadedFiredOnChild);
            }

            // Loaded events should not be fired.
            CheckNotSet(EventFireFlags.LoadedFiredOnParent);
            CheckNotSet(EventFireFlags.LoadedFiredOnChild);

            CoreLogger.LogStatus("");
            return true;
        }
        #endregion UnlinkParentFromSurface

        #region AttachLoadedListener
        /// <summary>
        /// Handler for AttachLoadedListener.
        /// 
        /// Here we attach the Loaded event handler to either parent or child,
        /// depending on the input params.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool AttachLoadedListener(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Parameters: " + inParameters.ToString());
            CoreLogger.LogStatus("Expected end state: " + GetStateString(endState));

            ClearFlags();

            // Check the input parameter to decide whether we attach the event handler
            // to the parent or to the child, and attach the handler accordingly.
            switch (inParameters["LoadedListenerTarget"])
            {
                case "Parent":
                    _parent.Loaded += s_parentLoadedEventHandler;
                    break;

                case "Child":
                    if (_childAsFE != null)
                    {
                        _childAsFE.Loaded += s_childLoadedEventHandler;
                    }
                    else
                    {
                        _childAsFCE.Loaded += s_childLoadedEventHandler;
                    }
                    break;
            }

            // Wait till events are done firing.
            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);

            // We have just attached the handlers. That should not cause Loaded events 
            // to fire.
            CheckNotSet(EventFireFlags.LoadedFiredOnParent);
            CheckNotSet(EventFireFlags.LoadedFiredOnChild);

            CoreLogger.LogStatus("");
            return true;
        }
        #endregion AttachLoadedListener

        #region DetachLoadedListener
        /// <summary>
        /// Handler for DetachLoadedListener.
        /// 
        /// Here we detach the Loaded event handler from either parent or child,
        /// depending on the input params.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool DetachLoadedListener(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Parameters: " + inParameters.ToString());
            CoreLogger.LogStatus("Expected end state: " + GetStateString(endState));

            // Check the input parameter to decide whether we detach the event handler
            // from the parent or from the child, and detach the handler accordingly.
            switch (inParameters["LoadedListenerTarget"])
            {
                case "Parent":
                    _parent.Loaded -= s_parentLoadedEventHandler;
                    break;

                case "Child":
                    if (_childAsFE != null)
                    {
                        _childAsFE.Loaded -= s_childLoadedEventHandler;
                    }
                    else
                    {
                        _childAsFCE.Loaded -= s_childLoadedEventHandler;
                    }
                    break;
            }

            CoreLogger.LogStatus("");
            return true;
        }
        #endregion DetachLoadedListener

        #region AttachUnloadedListener
        /// <summary>
        /// Handler for AttachUnloadedListener.
        /// 
        /// Here we attach the Unloaded event handler either to parent or child,
        /// depending on the input params.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool AttachUnloadedListener(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Parameters: " + inParameters.ToString());
            CoreLogger.LogStatus("Expected end state: " + GetStateString(endState));

            ClearFlags();

            // Check the input parameter to decide whether we attach the event handler
            // to the parent or to the child, and attach the handler accordingly.
            switch (inParameters["UnloadedListenerTarget"])
            {
                case "Parent":
                    _parent.Unloaded += s_parentUnloadedEventHandler;
                    break;

                case "Child":
                    if (_childAsFE != null)
                    {
                        _childAsFE.Unloaded += s_childUnloadedEventHandler;
                    }
                    else
                    {
                        _childAsFCE.Unloaded += s_childUnloadedEventHandler;
                    }
                    break;
            }

            // Wait till events are done firing.
            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);

            // We have just attached the handlers. That should not cause Unloaded events 
            // to fire.
            CheckNotSet(EventFireFlags.UnloadedFiredOnParent);
            CheckNotSet(EventFireFlags.UnloadedFiredOnChild);

            CoreLogger.LogStatus("");
            return true;
        }
        #endregion AttachUnloadedListener

        #region DetachUnloadedListener
        /// <summary>
        /// Handler for DetachUnloadedListener.
        /// 
        /// Here we detach the Unloaded event handler from either parent or child,
        /// depending on the input params.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool DetachUnloadedListener(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Parameters: " + inParameters.ToString());
            CoreLogger.LogStatus("Expected end state: " + GetStateString(endState));

            // Check the input parameter to decide whether we detach the event handler
            // from the parent or from the child, and detach the handler accordingly.
            switch (inParameters["UnloadedListenerTarget"])
            {
                case "Parent":
                    _parent.Unloaded -= s_parentUnloadedEventHandler;
                    break;

                case "Child":
                    if (_childAsFE != null)
                    {
                        _childAsFE.Unloaded -= s_childUnloadedEventHandler;
                    }
                    else
                    {
                        _childAsFCE.Unloaded -= s_childUnloadedEventHandler;
                    }
                    break;
            }

            CoreLogger.LogStatus("");
            return true;
        }
        #endregion DetachUnloadedListener

        #region LinkChildToParent
        /// <summary>
        /// Handler for LinkChildToParent
        /// 
        /// Here we link child to parent. 
        /// If child is an FE, then parent is a Custom_FrameworkElement. In this case
        ///    we establish a visual-only link (that's needed for Loaded event to fire,
        ///    logical links are not supposed to work.)
        /// If child is an FCE, then parent is a Custom_FrameworkElement_With_IContentHost,
        ///    so we establish a logical link (logical link will work here for causing the
        ///    Loaded event. We cannot have a visual link anyway, since FCE is not a visual).
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool LinkChildToParent(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Expected end state: " + GetStateString(endState));

            ClearFlags();

            if (_childAsFE != null)
            {
                // Link visually
                _parent.AddChild(_child, false /*logical */, true /*visual*/);
            }
            else
            {
                // Link logically
                _parent.AddChild(_child, true /*logical */, false /*visual*/);
            }

            // Wait till events are done firing.
            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);

            // Verification
            // Loaded event should have been fired on child, if and only if 
            //   - parent is linked to the surface
            //   - Loaded listener is attached to the child           
            if ((endState["ParentLinkedToSurface"] == "True") && 
                (endState["LoadedListenerOnChild"] == "True"))
            {
                CheckIfSet(EventFireFlags.LoadedFiredOnChild);
            }
            else
            {
                CheckNotSet(EventFireFlags.LoadedFiredOnChild);                
            }

            // Loaded event should not have fired on parent
            CheckNotSet(EventFireFlags.LoadedFiredOnParent);

            CoreLogger.LogStatus("");
            return true;
        }
        #endregion LinkChildToParent

        #region UnlinkChildFromParent
        /// <summary>
        /// Handler for UnlinkChildFromParent
        /// 
        /// Here we remove the existing link between child and parent. The link could be
        /// logical or visual, depending on the child.
        /// </summary>
        /// 
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool UnlinkChildFromParent(State endState, State inParameters, State outParameters)
        {
            CoreLogger.LogStatus("Expected end state: " + GetStateString(endState));

            ClearFlags();

            if (_childAsFE != null)
            {
                // Unlink visually
                _parent.RemoveChild(_child, false /*logical */, true /*visual*/);
            }
            else
            {
                // Unlink logically
                _parent.RemoveChild(_child, true /*logical */, false /*visual*/);
            }

            // Wait till events are done firing.
            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);

            // Verification
            // Unloaded event should have been fired on child, if and only if 
            //   - parent is linked to the surface
            //   - Unloaded listener is attached to the child
            if ((endState["ParentLinkedToSurface"] == "True") && 
                (endState["UnloadedListenerOnChild"] == "True"))
            {
                CheckIfSet(EventFireFlags.UnloadedFiredOnChild);
            }
            else
            {
                CheckNotSet(EventFireFlags.UnloadedFiredOnChild);
            }

            // No other events should be fired on parent/child
            CheckNotSet(EventFireFlags.LoadedFiredOnParent);
            CheckNotSet(EventFireFlags.LoadedFiredOnChild);
            CheckNotSet(EventFireFlags.UnloadedFiredOnParent);

            CoreLogger.LogStatus("");
            return true;
        }
        #endregion UnlinkChildFromParent

        #region Event handlers
        private static void ParentLoadedEventHandlerMethod(object sender, RoutedEventArgs e)
        {
            // Make sure that Initialized and LayoutUpdated events have already
            // fired on parent, before Loaded is being fired.
            if (!s_parentInitialized)
            {
                throw new Microsoft.Test.TestValidationException("Loaded event fired on parent before Initialized event");
            }

            s_flags |= EventFireFlags.LoadedFiredOnParent;       
        }

        private static void ChildLoadedEventHandlerMethod(object sender, RoutedEventArgs e)
        {
            // Make sure that Initialized and LayoutUpdated events have already
            // fired on child, before Loaded is being fired.
            if (!s_childInitialized)
            {
                throw new Microsoft.Test.TestValidationException("Loaded event fired on child before Initialized event");
            }

            s_flags |= EventFireFlags.LoadedFiredOnChild;
        }

        private static void ParentUnloadedEventHandlerMethod(object sender, RoutedEventArgs e)
        {
            s_flags |= EventFireFlags.UnloadedFiredOnParent;
        }

        private static void ChildUnloadedEventHandlerMethod(object sender, RoutedEventArgs e)
        {
            s_flags |= EventFireFlags.UnloadedFiredOnChild;
        }

        private static void ParentInitializedEventHandlerMethod(object sender, EventArgs e)
        {
            s_parentInitialized = true;
        }

        private static void ChildInitializedEventHandlerMethod(object sender, EventArgs e)
        {
            s_childInitialized = true;
        }

        private static void ParentLayoutUpdatedEventHandlerMethod(object sender, EventArgs e)
        {
            CheckNotSet(EventFireFlags.LoadedFiredOnParent);
            CheckNotSet(EventFireFlags.UnloadedFiredOnParent);
        }

        private static void ChildLayoutUpdatedEventHandlerMethod(object sender, EventArgs e)
        {
            CheckNotSet(EventFireFlags.LoadedFiredOnChild);
            CheckNotSet(EventFireFlags.UnloadedFiredOnChild);
        }
#endregion Event handlers

        private string GetStateString(State state)
        {
            string newLine = "\n";
            string tab = "\t";
            string stateString = "";

            foreach (string key in state.Keys)
            {
                stateString += newLine + tab + key + " : " + state[key];
            }

            return stateString;
        }
    }

    //
    // Flags
    //
    [FlagsAttribute]
    enum EventFireFlags : short
    {
        LoadedFiredOnParent = 1,
        LoadedFiredOnChild = 2,
        UnloadedFiredOnParent = 4,
        UnloadedFiredOnChild = 8
    };
}

// This file was generated using MDE on: Monday, November 21, 2005 6:04:10 PM
