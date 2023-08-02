// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// A factory class for test windows, verifiers, etc. 
    /// </summary>
    public class MultiTouchFactory
    {
        #region Properties

        public Type Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public MultiTouchTestModes Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        #endregion

        #region Constructors

        static MultiTouchFactory() {}

        public MultiTouchFactory() : this(null, MultiTouchTestModes.Touch, null) { }

        public MultiTouchFactory(Type type) : this(type, MultiTouchTestModes.Touch, null) { }

        public MultiTouchFactory(MultiTouchTestModes mode) : this(null, mode, null) { }

        public MultiTouchFactory(Type type, MultiTouchTestModes mode, string title)
        {
            this.Type = type;
            this.Mode = mode;
            this.Title = title;
        }

        #endregion

        #region Mothods

        //public static void AddHandler(DependencyObject d, RoutedEvent routedEvent, Delegate handler)
        //{
        //    if (routedEvent == null)
        //    {
        //        throw new ArgumentNullException("routedEvent");
        //    }

        //    if (handler == null)
        //    {
        //        throw new ArgumentNullException("handler");
        //    }

        //    if (handler.GetType() != routedEvent.HandlerType)
        //    {
        //        throw new ArgumentException("routedEvent and handler");
        //    }

        //    UIElement uie = d as UIElement;  // 






        //public static void RemoveHandler(DependencyObject d, RoutedEvent routedEvent, Delegate handler)
        //{
        //    if (routedEvent == null)
        //    {
        //        throw new ArgumentNullException("routedEvent");
        //    }

        //    if (handler == null)
        //    {
        //        throw new ArgumentNullException("handler");
        //    }

        //    if (handler.GetType() != routedEvent.HandlerType)
        //    {
        //        throw new ArgumentException("routedEvent and handler");
        //    }

        //    UIElement uie = d as UIElement;  // 







        /// <summary>
        /// create the test window by test mode w/ or w/o given test win title
        /// the test window would be the generic type with major hookups
        /// </summary>
        /// <param name="testMode"></param>
        /// <returns></returns>
        public Window CreateTestWindow(string title)
        { 
            switch(Mode.ToString())
            {
                case "Manipulations":
                    _testwin = new TestManipulations(title);
                    break;

                case "Touch":
                    _testwin = new TestTouch(title);
                    break;

                case "Gestures":
                    _testwin = new MT35();
                    break;

                case "Events":
                    _testwin = new RoutedEvents();
                    break;

                default:
                    _testwin = new Window();  // manual build
                    break;
            }

            return _testwin;
        }

        /// <summary>
        /// create the test window by window type
        /// where you would like to have more control over
        /// the instantiated test window
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Window CreateTestWindow(Type type)
        {
            switch (type.ToString())
            {
                case "MT35":
                    _testwin = new MT35();
                    break;

                case "DrawCircles":
                    _testwin = new DrawCircles();
                    break;

                default:
                    _testwin = new Window(); 
                    break;
            }

            return _testwin;
        }

        /// <summary>
        /// create related verifier by test mode
        /// </summary>
        /// <returns></returns>
        public MultiTouchVerifier CreateVerifier()
        {
            switch (Mode.ToString())
            {
                case "Manipulations":
                    _verifier = new ManipulationVerifier(_testwin);
                    break;

                case "Touch":
                    _verifier = new TouchVerifier(_testwin);  
                    break;

                case "Events":  // 
                    break;

                default: // gestures
                    _verifier = new GestureVerifier(_testwin);
                    break;
            }

            return _verifier;        
        }

        #endregion

        #region Local Fields

        Type _type;
        MultiTouchTestModes _mode;
        string _title;
        Window _testwin;
        MultiTouchVerifier _verifier;
        
        #endregion
    }
}
