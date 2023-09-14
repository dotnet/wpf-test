// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Events;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Create coverage for Clr event using the generic storage
    /// 
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Clr
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  ClrEventUsingGenericStore.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>For Button test DataContextChanged, 
    /// IsStylusDirectlyOverChanged, IsFocussWithinChanged, and Loaded</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.CLR", "ClrEventUsingGenericStore", Disabled=true)]
    public class ClrEventUsingGenericStore : EventHelper
    {
        #region Private Data
        int _totalEvent = 0;
        int _numCalled = 0;
        Window _win = null;
        Button _element = null;
        int _expectedCalled = 0;
        EventType _eventType;
        Dispatcher _dispatcher = null;
        Bold _fce = null;
        SurfaceFramework _surface = null;
        #endregion


        #region Constructor
        public ClrEventUsingGenericStore()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            CoreLogger.LogStatus ("This is a BVT scenario for a Clr events on framework element that use EventHandlersStore");

            using (CoreLogger.AutoStatus ("Creating a test events "))
            {
                _totalEvent = Enum.GetValues(typeof(EventType)).Length;
                

                _eventType = (EventType)0;
                TestEvent();
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        private void TestEvent()
        {

            CoreLogger.LogStatus("Test event : " + _eventType.ToString());
            SingleHandler();
        }

        private void SingleHandler()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            // Enter Dispatcher
            CreateWindow();
            _numCalled = 0;

            _expectedCalled = 1;
            DispatcherHelper.RunDispatcher();
        }

        private void CreateWindow()
        {
            _element = new Button ();
            _fce = new Bold();
            _element.Content = _fce;
            _surface = new SurfaceFramework("Window",0,0,300,300);
            AddEventHandler();
            _surface.DisplayObject(_element);
            _element.Loaded += new RoutedEventHandler(OnLoaded);
            _win = (Window)_surface.SurfaceObject;
        }
            
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            CoreLogger.LogStatus("In OnLoaded ...");
            RaiseEvent();
        }

        private void RaiseEvent()
        {
            CoreLogger.LogStatus("In RaiseEvent ...");

            switch (_eventType)
            {
                case EventType.DataContextChanged:
                    _element.DataContext = new object();
                    break;
                case EventType.IsStylusDirectlyOverChanged:
                    MoveMouse();
                    break;
                case EventType.IsKeyboardFocusWithinChanged:
                    if (_element.IsKeyboardFocusWithin)
                    {
                        CoreLogger.LogStatus("Moving focus away...");
                        _element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    }
                    else
                    {
                        CoreLogger.LogStatus("Getting focus ...");
                        _element.Focus();
                    }
                    break;
                case EventType.Loaded:
                    break;
                default: break;
            }
            VerifyCount();
            return;
        }

        private void VerifyCount()
        {
            if (_eventType != EventType.IsStylusDirectlyOverChanged)
            {
                CoreLogger.LogStatus("Verifying cound for : " + _eventType.ToString() + " Actaul: " + _numCalled.ToString() + " expected: " + _expectedCalled.ToString());
                if (_expectedCalled != _numCalled)
                    throw new Microsoft.Test.TestValidationException("Expected times called: " + _expectedCalled.ToString() + " Actually: " + _numCalled.ToString());
            }
            CleanWindow();
            _eventType++;
            if ((int)_eventType < (int)_totalEvent)
            {
                TestEvent();
            }
            else
            {
                CleanWindow();
            }
        }
        private void CleanWindow()
        {
            DispatcherHelper.ShutDown();
            CoreLogger.LogStatus("Quitted dispatcher .");
        }

        private void AddEventHandler()
        {
            switch(_eventType)
            {
                case EventType.DataContextChanged:
                    _element.DataContextChanged += new DependencyPropertyChangedEventHandler(OnDependencyPropertyChanged);
                    break;
                case EventType.IsStylusDirectlyOverChanged:
                    _element.IsStylusDirectlyOverChanged += new DependencyPropertyChangedEventHandler(OnDependencyPropertyChanged);
                    break;

                case EventType.IsKeyboardFocusWithinChanged:
                    _element.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(OnDependencyPropertyChanged);
                    break;
                case EventType.Loaded:
                    _element.Loaded += new RoutedEventHandler(OnClrEvent);
                    break;
                default: break;
            }
            return;
        }

        private void MoveMouse()
        {
            Rect boundary = VisualTreeHelper.GetContentBounds(_element);

            if (_element.IsMouseOver)
            {
                CoreLogger.LogStatus("Moving out mouse");
                MouseHelper.MoveOnVirtualScreenMonitor();
            }
            else
            {
                CoreLogger.LogStatus("Moving in mouse");
                CoreLogger.LogStatus("Moving in mouse: " + boundary.Left.ToString() + " " + boundary.Right.ToString() + "  " + boundary.Top.ToString() + " " + boundary.Bottom.ToString());
                MouseHelper.Move(_element);
            }
        }
        private void RemoveEventHandler()
        {
            switch(_eventType)
            {
                case EventType.DataContextChanged:
                    _element.DataContextChanged -= new DependencyPropertyChangedEventHandler(OnDependencyPropertyChanged);
                    break;
                case EventType.IsStylusDirectlyOverChanged:
                    _element.IsStylusDirectlyOverChanged -= new DependencyPropertyChangedEventHandler(OnDependencyPropertyChanged);
                    break;

                case EventType.IsKeyboardFocusWithinChanged:
                    _element.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(OnDependencyPropertyChanged);
                    break;
                case EventType.Loaded:
                    _element.Loaded -= new RoutedEventHandler(OnClrEvent);
                    break;
                default: break;
            }
            return;
        }
        
        private void OnClrEvent(object sender, RoutedEventArgs args)
        {
            _numCalled++;
        }
        private void OnDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            _numCalled++;
        }
    
        private enum EventType
        {
            DataContextChanged = 0,
            IsStylusDirectlyOverChanged = 1,
            IsKeyboardFocusWithinChanged = 2,
            Loaded = 3
        };
        #endregion
    }
}

