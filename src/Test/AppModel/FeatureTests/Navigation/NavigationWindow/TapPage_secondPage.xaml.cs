// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using Microsoft.Test.Loaders;           // ApplicationMonitor
using Microsoft.Test.Logging;           // TestLog

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class TapPage_SecondPage
    {
        Window _win = Application.Current.MainWindow;
        
        void OnLoaded(object sender, EventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("    OnLoaded method called");
            // should start at 6
            Log.Current.CurrentVariation.LogMessage("  ==> init testCount is: " 
                + NavigationHelper.ActualTestCount);

            // attach to other events for Window
            _win.Activated += new EventHandler(OnActivated);
            _win.Deactivated += new EventHandler(OnDeactivated);
            _win.StateChanged += new EventHandler(OnStateChanged);
            _win.SizeChanged += new SizeChangedEventHandler(OnWindowSizeChanged);
            _win.Closing += new CancelEventHandler(OnClosing);
            _win.Closed += new EventHandler(OnClosed);

            Log.Current.CurrentVariation.LogMessage("  Tapping properties of the page: ");
            bool didSucceed = this.TapPageProperties();
            Log.Current.CurrentVariation.LogMessage("  TapPageProperties passed: " 
                + didSucceed);
            // should end at 13
            Log.Current.CurrentVariation.LogMessage("  ==> testCount is: " 
                + NavigationHelper.ActualTestCount);

            // activate hyperlink to navigate out of this
            RequestNavigateEventArgs requestNavigateEventArgs = new RequestNavigateEventArgs(TapPageHyperlink1.NavigateUri, null);
            requestNavigateEventArgs.Source=TapPageHyperlink1;
            TapPageHyperlink1.RaiseEvent(requestNavigateEventArgs);
        }

        bool TapPageProperties()
        {
            bool didSucceed = true;
            string textValue = "Are you ready for some Kentucky Fried Chicken?";
            Double topValue = 200, leftValue=400;
            Double windowHeightValue = 800, windowWidthValue=100;
            WindowState windowStateValue = WindowState.Maximized;

            // Text
            Log.Current.CurrentVariation.LogMessage("    *Text is: " + this.WindowTitle);
            this.WindowTitle = textValue;
            didSucceed = didSucceed && this.WindowTitle.Equals(textValue);
            Log.Current.CurrentVariation.LogMessage("     **Text is now: " + this.WindowTitle);

            // Top
            Log.Current.CurrentVariation.LogMessage("    *Top is: " + _win.Top.ToString());
            _win.Top = topValue;
            didSucceed = didSucceed && _win.Top.Equals(topValue);
            Log.Current.CurrentVariation.LogMessage("     **Top is now: " + _win.Top.ToString());

            // Left
            Log.Current.CurrentVariation.LogMessage("    *Left is: " + _win.Left.ToString());
            _win.Left = leftValue;
            didSucceed = didSucceed && _win.Left.Equals(leftValue);
            Log.Current.CurrentVariation.LogMessage("     **Left is now: " + _win.Left.ToString());

            // WindowHeight
            Log.Current.CurrentVariation.LogMessage("    *WindowHeight is: " + this.WindowHeight.ToString());
            this.WindowHeight = windowHeightValue;
            didSucceed = didSucceed && this.WindowHeight.Equals(windowHeightValue);
            Log.Current.CurrentVariation.LogMessage("     **WindowHeight is now: " + this.WindowHeight.ToString());

            // WindowWidth
            Log.Current.CurrentVariation.LogMessage("    *WindowWidth is: " + this.WindowWidth.ToString());
            this.WindowWidth = windowWidthValue;
            didSucceed = didSucceed && this.WindowWidth.Equals(windowWidthValue);
            Log.Current.CurrentVariation.LogMessage("     **WindowWidth is now: " + this.WindowWidth.ToString());

            // WindowState
            Log.Current.CurrentVariation.LogMessage("    *WindowState is: " + _win.WindowState.ToString());
            _win.WindowState = windowStateValue;
            didSucceed = didSucceed && _win.WindowState.Equals(windowStateValue);
            Log.Current.CurrentVariation.LogMessage("     **WindowState is now: " + _win.WindowState.ToString());

            Log.Current.CurrentVariation.LogMessage("    *didSucceed is: " + didSucceed);

            // un-register all the eventhandlers at this point
            _win.Activated -= OnActivated;
            _win.Deactivated -= OnDeactivated;
            _win.StateChanged -= OnStateChanged;
            _win.SizeChanged -= OnWindowSizeChanged;
            _win.Closing -= OnClosing;
            _win.Closed -= OnClosed;

            return didSucceed;
        }

        // event handler code
        void OnActivated(object sender, EventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("    OnActivated method called");
            NavigationHelper.ActualTestCount++;
        }

        void OnDeactivated(object sender, EventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("    OnDeactivated method called");
            NavigationHelper.ActualTestCount++;
        }

        // should get tapped 1 time:
        // 1 time for WindowState 
        void OnStateChanged(object sender, EventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("    OnStateChanged method called");
            NavigationHelper.ActualTestCount++;
        }

        // should get tapped 3 times:
        // 1 time for Height, 
        // 1 time for Width, 
        // 1 time for WindowState 
        void OnWindowSizeChanged(object sender, EventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("    OnWindowSizeChanged method called");
            NavigationHelper.ActualTestCount++;
        }

        void OnClosing(object sender, EventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("    OnClosing method called");
            NavigationHelper.ActualTestCount++;
        }

        void OnClosed(object sender, EventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("    OnClosed method called");
            NavigationHelper.ActualTestCount++;
        }
    }
}

