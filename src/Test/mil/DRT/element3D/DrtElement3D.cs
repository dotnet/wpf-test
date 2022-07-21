// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test of basic element 3D functionality
//
//
//

using System;
using System.Xml;
using System.Reflection;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using System.Windows.Input;

using System.Windows.Data;
using System.ComponentModel;
using CultureInfo = System.Globalization.CultureInfo;

using Tests;

namespace Microsoft.Samples
{
    public class DrtElement3D : Window
    {
        public DrtElement3D()
        {
            _suite = new Element3DTestSuite();

            //
            // Set up Window
            //
            Width = 800;
            Height = 600;
            Title = "DrtElement3D";

            // Build controls.
            _viewport = new Viewport3D();

            Grid viewportGrid = new Grid();
            viewportGrid.Background = Brushes.DarkBlue;
            viewportGrid.ColumnDefinitions.Add( new ColumnDefinition() );           
            Grid.SetColumn(_viewport,0);
            viewportGrid.Margin = new Thickness(4);
            viewportGrid.Children.Add(_viewport);
            
            _viewportPanel = viewportGrid;
            
            Content = _viewportPanel;
        }
    
        void RunAllTests()
        {
            // Set up state for tick handler which helps RunAllTests
            _count=0;
            
            // Push a work item onto the queue to move to the next test.
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(MoveToNextTest), null);
        }
        
        private object MoveToNextTest(object arg)
        {
            // build the scene
            Element3DTestCase testcase = _suite[_count++];
            testcase.BuildScene(_viewport);

            // Push a work item onto the queue to execute our next test.  This is the
            // same queue that processes paint messages so we are gauranteed to
            // be rendered.
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(DoCurrentTestBefore), null);
        
            return null;
        }

        private object DoCurrentTestBefore(object arg)
        {
            Element3DTestCase testcase = _suite[_count - 1];
            testcase.DoBefore();
            
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(DoCurrentTestAfter), null);            

            return null;
        }

        private object DoCurrentTestAfter(object arg)
        {
            Element3DTestCase testcase = _suite[_count - 1];
            if (testcase.Success)
            {
                testcase.DoAfter();
            }

            if (!testcase.Success)
            {
                _failedCount++;
                if (_failedCount > 1)
                {
                    _failedCaseNames += ", " + testcase;
                }
                else
                {
                    _failedCaseNames += " " + testcase;
                }
            }
            
            if (_count < _suite.Count)
            {                
                // Push a work item onto the queue to move to the next test.  Wait 2 seconds so we can see what is happening.
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(0.5);
                timer.Tick += 
                    delegate 
                    { 
                        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(MoveToNextTest), null); 
                        timer.IsEnabled = false;
                    };
                
                timer.IsEnabled = true;                
            }
            else
            {
                if (_failedCount == 0) 
                {
                    Console.WriteLine("DrtElement3D successfully completed " + (_count - _failedCount) + " of " + _suite.Count + " tests.");
                }
                else
                {
                    Console.WriteLine("DrtElement3D failed " + _failedCount + " of " + _suite.Count + " tests.  FAILED TESTS:" + _failedCaseNames);
                }

                Application.Current.Shutdown(-_failedCount);
            }

            return null;
        }

        [STAThread]
        public static int Main(string [] args)
        {
            Application application = new Application();

            DrtElement3D basic = new DrtElement3D();

            if (((IList)args).Contains("-help") || ((IList)args).Contains("-h"))
            {
                Console.Write("DrtElement3D [-auto] [-h(elp)]\nInteractive program to test element 3D features.\n-help : Show this usage info.\n");
                return 0;
            }
                       
            StreamWriter logOutput = new StreamWriter("DrtElement3D.log", false /* do not append - re-write if the file exists */);
            logOutput.AutoFlush = true;
            Console.SetOut(logOutput);
            Console.WriteLine("DrtElement3D contact is kurtb.  Use  -h for help.  ");
            basic.RunAllTests();
           
            basic.Show();            
            int result = application.Run();
            
            Console.Out.Close();

            // Recover the standard output stream so that a small note can be displayed.
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            Console.WriteLine("DrtElement3D " + (result == 0 ? "SUCCEEDED" : "FAILED (check DrtElement3D.log)"));

            return result;
        }

        private Element3DTestSuite _suite;
        private Viewport3D _viewport;
        private Panel _viewportPanel;

        private int _count;
        private int _failedCount = 0;
        private string _failedCaseNames = "";        
    }
}

