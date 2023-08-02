// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;


namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Helper class for Event cases 
    /// <para />
    /// </summary>
    /// <remarks>
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  EventHelper.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Register two events: RoutedEvent1 and  PreviewRoutedEvent1</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    public class EventHelper : TestCase
    {
        /// <summary>
        /// Static Constructor. Registers two events (RoutedEvent1 and PreviewRoutedEvent1)
        /// </summary>    
        static EventHelper()
        {
            // Register a RoutedEvent1            
            RoutedEvent1 = EventManager.RegisterRoutedEvent("RoutedEvent1", 
                RoutingStrategy.Bubble, 
                typeof(CustomRoutedEventHandler), 
                typeof(EventHelper));

            // Register a PreviewRoutedEvent1            
            PreviewRoutedEvent1 = EventManager.RegisterRoutedEvent("PreviewRoutedEvent1", 
                RoutingStrategy.Tunnel, 
                typeof(CustomRoutedEventHandler), 
                typeof(EventHelper));
        }

        ///<summary/>
        public static RoutedEvent RoutedEvent1;
        ///<summary/>
        public static RoutedEvent PreviewRoutedEvent1;
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public EventHelper() :base(TestCaseType.ContextSupport){}
        
        /// <summary>
        /// Verifies the sender argument and RoutedEventArgs.Source passed
        /// to a handler.
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void VerifyRoutedEvent(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("Verifying routed event handler's sender and args.Source...");
            if (args.Source != args.Targets[args.HandlersCalledCount].Source)
            {
                throw new Microsoft.Test.TestValidationException("Incorrect Source");
            }

            if (sender != args.Targets[args.HandlersCalledCount].Sender)
            {
                throw new Microsoft.Test.TestValidationException("Incorrect Sender");
            }
            args.HandlersCalledCount++;
        }

        /// <summary>
        /// Verifies the order of the called handler, as well as the sender argument 
        /// and RoutedEventArgs.Source passed to it.
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        /// <param name="handlerCount"></param>
        public void VerifyRoutedEvent(object sender, CustomRoutedEventArgs args, int handlerCount)
        {
            CoreLogger.LogStatus("Verifying the order of the called handler...");

            // Verify order of the called handler.
            if (args.HandlersCalledCount != handlerCount)
                throw new Microsoft.Test.TestValidationException("Event fired in incorrect order. Expected: " + handlerCount + ", Actual: " + args.HandlersCalledCount);
        
            // Verify sender and Source.
            this.VerifyRoutedEvent(sender, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="other"></param>
        public void OnCustomClrEvent(object sender, EventArgs args, object other)
        {
            _numCalls++;
        }

        private int _numCalls;
        #region Attached Event
        /// <summary>
        ///     Bubble
        /// </summary>
        public static readonly RoutedEvent BubbleEvent = 
            EventManager.RegisterRoutedEvent("Bubble",
                RoutingStrategy.Bubble,
                typeof(CustomRoutedEventHandler),
                typeof(EventHelper));
        /// <summary>
        ///     Adds a handler for the Bubble attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to this event</param>
        /// <param name="handler">Event Handler to be added</param>
        public static void AddBubbleHandler(DependencyObject element, CustomRoutedEventHandler handler)
        {
            AddHandlerForAttachedEvent(element, BubbleEvent, handler);
        }

        /// <summary>
        ///     Removes a handler for the Bubble attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to this event</param>
        /// <param name="handler">Event Handler to be removed</param>
        public static void RemoveBubbleHandler(DependencyObject element, CustomRoutedEventHandler handler)
        {
            RemoveHandlerForAttachedEvent(element, BubbleEvent, handler);
        }

        /// <summary>
        ///     Tunnel
        /// </summary>
        public static readonly RoutedEvent TunnelEvent =
            EventManager.RegisterRoutedEvent("Tunnel",
                RoutingStrategy.Tunnel,
                typeof(CustomRoutedEventHandler),
                typeof(EventHelper));
        /// <summary>
        ///     Adds a handler for the Tunnel attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to this event</param>
        /// <param name="handler">Event Handler to be added</param>
        public static void AddTunnelHandler(DependencyObject element, CustomRoutedEventHandler handler)
        {
            AddHandlerForAttachedEvent(element, TunnelEvent, handler);
        }

        /// <summary>
        ///     Removes a handler for the Tunnel attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to this event</param>
        /// <param name="handler">Event Handler to be removed</param>
        public static void RemoveTunnelHandler(DependencyObject element, CustomRoutedEventHandler handler)
        {
            RemoveHandlerForAttachedEvent(element, TunnelEvent, handler);
        }
        static void AddHandlerForAttachedEvent(DependencyObject element, RoutedEvent id, CustomRoutedEventHandler handler)
        {
            UIElement uiElement = element as UIElement;
            if (uiElement != null)
            {
                uiElement.AddHandler(id, handler);
            }
            else
            {
                ContentElement contentElement = element as ContentElement;
                if (contentElement != null)
                {
                    contentElement.AddHandler(id, handler);
                }
                else
                {
                    throw new Microsoft.Test.TestValidationException("Event can only be attached on UIElement or ContentElement.");
                }
            }
        }
        static void RemoveHandlerForAttachedEvent(DependencyObject element, RoutedEvent id, CustomRoutedEventHandler handler)
        {
            UIElement uiElement = element as UIElement;
            if (uiElement != null)
            {
                uiElement.RemoveHandler(id, handler);
            }
            else
            {
                ContentElement contentElement = element as ContentElement;
                if (contentElement != null)
                {
                    contentElement.RemoveHandler(id, handler);
                }
                else
                {
                    throw new Microsoft.Test.TestValidationException("Event can only be attached on UIElement or ContentElement.");
                }
            }
        }
        #endregion Attached Event
    }

    /// <summary>
    ///     CustomRoutedEventArgs
    /// </summary>
    /// <ExternalAPI/>
    public class CustomRoutedEventArgs : RoutedEventArgs
    {
        #region ExternalAPI

        /// <summary>
        ///     Gettor and Settor for HandlersCalledCount
        /// </summary>
        /// <ExternalAPI/>
        public int HandlersCalledCount
        {
            get { return _numEventsFired; }
            set { _numEventsFired = value; }
        }

        /// <summary>
        ///     Gettor and Settor for NumClassEventsFired
        /// </summary>
        /// <ExternalAPI/>
        public int NumClassEventsFired
        {
            get { return _numClassEventsFired; }
            set { _numClassEventsFired = value; }
        }

        /// <summary>
        ///     Set Shouldmath flag to the value given
        /// </summary>
        /// <ExternalAPI/>
        public bool ShouldMath
        {
            get { return _shouldMath; }
            set { _shouldMath = value; }
        }

        /// <summary>
        ///     Returns targets array
        /// </summary>
        /// <ExternalAPI/>
        public RouteTarget[] Targets
        {
            get { return _targets; }
        }

        /// <summary>
        ///     Invokes the event handler with the 
        ///     appropriate arguments
        /// </summary>
        /// <param name="genericHandler">
        ///     Generic Handler to be invoked
        /// </param>
        /// <param name="genericTarget">
        ///     Target on whom the Handler will be invoked
        /// </param>
        /// <ExternalAPI/>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            CustomRoutedEventHandler handler = (CustomRoutedEventHandler)genericHandler;

            handler(genericTarget, this);
        }

        #endregion ExternalAPI
        
        #region Construction

        /// <summary>
        ///     Constructor for CustomRoutedEventArgs
        /// </summary>
        /// <ExbetranlAPI/>
        public CustomRoutedEventArgs(RoutedEvent routedEvent, RouteTarget[] targets) : base()
        {
            base.RoutedEvent=routedEvent;
            _targets = targets;
            _numEventsFired = 0;
            _numClassEventsFired = 0;
            _shouldMath = true;
        }

        /// <summary>
        ///     Constructor for CustomRoutedEventArgs
        /// </summary>
        /// <ExbetranlAPI/>
        public CustomRoutedEventArgs(RoutedEvent routedEvent, RouteTarget[] targets, bool handled) : this(
                routedEvent,
                targets)
        {
            base.Handled = handled;
        }
        
        #endregion Construction
        
        #region Data

        private RouteTarget[] _targets;

        private int _numEventsFired;

        private int _numClassEventsFired;

        bool _shouldMath;
        
        #endregion Data
        
    }

    /// <summary>
    /// 
    /// </summary>
    public struct RouteTarget
    {
        /// <summary>
        /// 
        /// </summary>
        public object Sender;
        /// <summary>
        /// 
        /// </summary>
        public object Source;
    }

    /// <summary>
    ///     CustomRoutedEventHandler Definition
    /// </summary>
    /// <ExternalAPI/>
    public delegate void CustomRoutedEventHandler(object sender, CustomRoutedEventArgs args);

}
