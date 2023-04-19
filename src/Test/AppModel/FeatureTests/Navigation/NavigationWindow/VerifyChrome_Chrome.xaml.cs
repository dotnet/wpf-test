// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

#define TRACE

using System;
using System.Collections;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Xml;
using System.Net;
using System.Windows.Interop;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Diagnostics;
using Microsoft.Windows.Test.Client.AppSec.Navigation;
#endregion

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// BVT that verifies visual trees of NavigationWindow with styles 
    /// set programatically
    /// Initial style is the default style (ie with the NavigationChrome)
    /// New style is with the chrome removed
    /// On Standalone apps - this behavior is the same on LH and XP
    /// For browser hosted apps - Inline Chrome is present in XP whereas
    /// not in LH
    /// Page1.xaml is a simple xaml file with a DockPanel 
    /// Page2.xaml is contains a DockPanel and also a style definition
    /// for the NavigationWindow Type. It sets the style in its code behind
    /// (Page2.xaml.cs) in the OnLoaded method
    /// </summary>
    public partial class NavigationTests : Application
    {
        Application      _navApp         = null;
        NavigationWindow navWin         = null;
        //Frame            placeFrame     = null;
        //Button           navigateButton = null;

        // doing this test the lazy way
        // when verify mode is false - we just write the visual trees
        // to xml files and utilize them when verify mode is true
        bool _inVerifyMode_VerifyChrome = true;
        
        // app running in IE7?
        bool _inIE7 = false;
                
        XmlDocument _doc = null;

        String _standaloneVisualTreeNoChrome = "VerifyChrome_stdAloneVisualTreeNoChrome.xml";
        String _standaloneVisualTreeWithChrome = "VerifyChrome_stdAloneVisualTreeChrome.xml";
        String _browserVisualTreeNoChrome = "VerifyChrome_browVisualTreeNoChrome.xml";
        String _browserVisualTreeWithChrome = "VerifyChrome_browVisualTreeChrome.xml";
        String _ie7WithoutChrome = "VerifyChrome_ie7Page1.xml";
        String _ie7WithoutChromeRestyled = "VerifyChrome_ie7Page2.xml";
        String _visualTreeToUse = String.Empty;

        enum VerifyChrome_State
        {
            Inited,
            Page1,
            Page2
        }

        VerifyChrome_State _verifyChrome_currentState;

        private void VerifyChrome_Startup(object sender, StartupEventArgs e)
        {                        
            /*
            TraceListener tr = new TextWriterTraceListener("xamlAD.txt");
            Trace.Listeners.Add(tr);
            Trace.AutoFlush = true;
            */

            if (Log.Current == null)
                new TestLog("Chrome BVT");

            if (Log.Current == null)
                ApplicationMonitor.NotifyStopMonitoring();

            Log.Current.CurrentVariation.LogMessage("Starting up Chrome BVT...");
            // Log.Current.Stage = TestStage.Initialize;

            VerifyChrome_CheckIfIE7();
            VerifyChrome_CheckIfServer2003();

            _navApp = Application.Current;

            //Log.Current.CurrentVariation.LogMessage("Registering Application-level eventhandlers.");           
            //navApp.Navigated += new NavigatedEventHandler(OnNavigated);

            _verifyChrome_currentState = VerifyChrome_State.Inited;
            // Log.Current.Stage = TestStage.Run;
            this.StartupUri = new Uri("VerifyChrome_Page1.xaml", UriKind.RelativeOrAbsolute);
        }

        void VerifyChrome_CheckIfIE7()
        {
            int ieVersion = ApplicationDeploymentHelper.GetIEVersion();
            Log.Current.CurrentVariation.LogMessage("Finding InternetExplorer version: " + ieVersion);
            if (ieVersion > 6)
            {
                _inIE7 = true;
                Log.Current.CurrentVariation.LogMessage("On IE7");
            }
        }

        /// <summary>
        /// Windows Server 2003 uses a different UI than XP does.  We need to detect this and
        /// use a different comparison xml file.
        /// </summary>
        void VerifyChrome_CheckIfServer2003()
        {
            if (SystemInformation.Current.Product == Product.WindowsServer2003)
            {
                _browserVisualTreeWithChrome = "VerifyChrome_browVisualTreeChrome_Srv03.xml";
                Log.Current.CurrentVariation.LogMessage("Detected Server03, will use comparison file " + _browserVisualTreeWithChrome);
            }
        }

        public void VerifyChrome_Navigated(object sender, NavigationEventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("OnNavigated sender = " + sender);
            Log.Current.CurrentVariation.LogMessage("navigated to uri = " + e.Uri
                            + " content = " + e.Content
                            + " object type = " + e.Content.GetType());

            NavigationService ns = NavigationService.GetNavigationService(e.Content as DependencyObject);
            Log.Current.CurrentVariation.LogMessage("NavigationService = " + ns);

            if (_verifyChrome_currentState == VerifyChrome_State.Inited)
            {
                navWin = _navApp.MainWindow as NavigationWindow;
                if (navWin == null)
                    VerifyChrome_ExitWithError("navWin is null");
		
                if (BrowserInteropHelper.IsBrowserHosted)
                {
                    Log.Current.CurrentVariation.LogMessage("Running test in browser.");
                    navWin.Title = "RootBrowserWindow: Browser hosted app";
                }
                else
                {
                    Log.Current.CurrentVariation.LogMessage("Running test as standalone.");
                    navWin.Title = "NavigationWindow: Standalone app";
                }

                Log.Current.CurrentVariation.LogMessage("Changing to Page1");
                _verifyChrome_currentState = VerifyChrome_State.Page1;

                Log.Current.CurrentVariation.LogMessage("Registering NavigationWindow-level eventhandlers.");
                navWin.ContentRendered += new EventHandler(VerifyChrome_ContentRendered);
            }
            else if (_verifyChrome_currentState == VerifyChrome_State.Page1)
            {
                Log.Current.CurrentVariation.LogMessage("Changing to Page2");
                _verifyChrome_currentState = VerifyChrome_State.Page2;
            }
        }

        void VerifyChrome_TraverseLogicalTree(DependencyObject node)
        {
            IEnumerator logicalChildren = LogicalTreeHelper.GetChildren(node).GetEnumerator();
            logicalChildren.Reset();
            while (logicalChildren.MoveNext())
            {
                Log.Current.CurrentVariation.LogMessage("logical child type = " + logicalChildren.Current.GetType());
                DependencyObject child = logicalChildren.Current as DependencyObject;
                // Trace.Indent();
                if (child != null)
                {
                    Log.Current.CurrentVariation.LogMessage("-" + child);
                    VerifyChrome_TraverseLogicalTree(child);
                }
                else
                {
                    Log.Current.CurrentVariation.LogMessage("logical child is not a DependencyObject | child = " + logicalChildren.Current);                    
                    Log.Current.CurrentVariation.LogMessage("-" + logicalChildren.Current);
                }
                // Trace.Unindent();
            }
        }

        void VerifyChrome_TraverseVisualTree(DependencyObject visual, XmlNode root)
        {
            if (visual == null)
            {
                Log.Current.CurrentVariation.LogMessage("visual is null");
                return;
            }

            int count = VisualTreeHelper.GetChildrenCount(visual);
            if (count != 0)
            {
                int i = 0;
                XmlNode child = null;
                XmlAttribute id = null;
                FrameworkElement fe = null;

                for(int j = 0; j < count; j++)
                {
                    DependencyObject v = VisualTreeHelper.GetChild(visual, j);
                    if (!_inVerifyMode_VerifyChrome)
                    {
                        child = _doc.CreateElement(v.GetType().ToString());
                        id = _doc.CreateAttribute("Name");
                        fe = v as FrameworkElement;
                        if (fe != null)
                        {
                            id.Value = (fe).Name;
                            if (!String.IsNullOrEmpty(id.Value))
                                child.Attributes.Append(id);
                        }
                        root.AppendChild(child);
                    }
                    else
                    {
                        if (child == null)
                            child = root.FirstChild;
                        else
                            child = child.NextSibling;

                        if (child == null)
                            VerifyChrome_ExitWithError("Test failed. Visual = " + v + " matched null xml element");
                        else if (!child.Name.Equals(v.GetType().ToString()))
                            VerifyChrome_ExitWithError("Test failed. Visual = " + v + " matched xml element is " + child.Name);
                    }
                    // Trace.Indent();
                    //Log.Current.Status("-" + v.GetType().ToString());
                    VerifyChrome_TraverseVisualTree(v, child);
                    // Trace.Unindent();
                    i++;
                }
            }
            else
            {
                if (root != null)
                {
                    if (root.ChildNodes.Count != 0)
                        VerifyChrome_ExitWithError("Xml Vs Visual tree - child nodes count mismatch");
                }
            }
        }

        void VerifyChrome_TraverseChildHierarchy(Visual visual)
        {
            // Log.Current.CurrentVariation.LogMessage("\n\n\nLogical Tree = ");
            // Log.Current.CurrentVariation.LogMessage(visual);
            // TraverseLogicalTree(visual);
            if (!_inVerifyMode_VerifyChrome)
            {
                _doc = new XmlDocument();                
            }
            else
            {
                _doc = new XmlDocument();
                Uri baseUri = new Uri("pack://application:,,,/");
                Uri resolvedUri = new Uri(baseUri, _visualTreeToUse);
                IWebRequestCreate factory = (IWebRequestCreate) new PackWebRequestFactory();
                WebRequest request = factory.Create(resolvedUri);
                StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
                
                _doc.LoadXml(sr.ReadToEnd());
            }

            // Log.Current.CurrentVariation.LogMessage("\n\n\nVisual Tree = ");
            // Log.Current.CurrentVariation.LogMessage(visual);
            if (!_inVerifyMode_VerifyChrome)
            {
                XmlNode root = _doc.CreateElement(navWin.GetType().ToString());
                _doc.AppendChild(root);
                VerifyChrome_TraverseVisualTree(visual, root);
                // Log.Current.CurrentVariation.LogMessage(doc.ChildNodes.Count);
                XmlWriter xw = new XmlTextWriter(_visualTreeToUse, Encoding.ASCII);
                _doc.WriteTo(xw);
                xw.Flush();                
            }
            else
            {
                VerifyChrome_TraverseVisualTree(visual, _doc.FirstChild);
            }
        }

        void VerifyChrome_ContentRendered(object sender, EventArgs e)
        {
            if (_verifyChrome_currentState == VerifyChrome_State.Page1)
            {
                if (BrowserInteropHelper.IsBrowserHosted)
                {
                    Log.Current.CurrentVariation.LogMessage("Page 1 --> Determining which visual tree to use for browser-hosted case");
                    if (_inIE7)
                    {
                        Log.Current.CurrentVariation.LogMessage("Using IE7 visual tree without chrome");
                        _visualTreeToUse = _ie7WithoutChrome;
                    }
                    else
                    {
                        Log.Current.CurrentVariation.LogMessage("Using IE6 visual tree with chrome");
                        _visualTreeToUse = _browserVisualTreeWithChrome;
                    }
                }
                else
                {
                    Log.Current.CurrentVariation.LogMessage("Using standalone visual tree with chrome");
                    _visualTreeToUse = _standaloneVisualTreeWithChrome;
                }

                VerifyChrome_TraverseChildHierarchy(navWin);
                navWin.Navigate(new Uri("VerifyChrome_Page2.xaml", UriKind.RelativeOrAbsolute));
            }
            else if (_verifyChrome_currentState == VerifyChrome_State.Page2)
            {
                if (BrowserInteropHelper.IsBrowserHosted)
                {
                    Log.Current.CurrentVariation.LogMessage("Page 2 --> Determining which visual tree to use for browser-hosted case");
                    if (_inIE7)
                    {
                        Log.Current.CurrentVariation.LogMessage("Using IE7 visual tree with a restyled chrome");
                        _visualTreeToUse = _ie7WithoutChromeRestyled;
                    }
                    else
                    {
                        Log.Current.CurrentVariation.LogMessage("Using IE6 visual tree with no chrome");
                        _visualTreeToUse = _browserVisualTreeNoChrome;
                    }
                }
                else
                {
                    Log.Current.CurrentVariation.LogMessage("Using standalone visual tree with no chrome");
                    _visualTreeToUse = _standaloneVisualTreeNoChrome;
                }

                VerifyChrome_TraverseChildHierarchy(navWin);

                // Pass the test!
                Log.Current.CurrentVariation.LogMessage("Application shutdown. Test passed");
                NavigationHelper.CacheTestResult(Result.Pass);
                VerifyChrome_Shutdown();
            }
        }

        private void VerifyChrome_ExitWithError(String failMessage)
        {
            Log.Current.CurrentVariation.LogMessage(failMessage);
            NavigationHelper.CacheTestResult(Result.Fail);
            VerifyChrome_Shutdown();
        }

        private void VerifyChrome_Shutdown()
        {
            NavigationHelper.Shutdown();
        }
    }
}

