// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test ContentElement derived Hyperlink inner Document navigation. Content Hosted in FlowDocumentScrollViewer. 
//               
//               Loads a page with a Hyperlink into a NavigationWindow and invokes the Hyperlink.
//			     The Hyperlink then navigates to various positions of the same xaml file.  
//                                   
// Verification: ScrollViewer vertical offset comparison or page number (if paginated viewer host)
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing inner document navigation.   
    /// </summary>
    [Test(3, "Hyperlink", "InnerDocumentNavigationTest", Keywords = "Localization_Suite")]
    public class InnerDocumentNavigationTest : AvalonTest
    {
        #region Test case members

        private NavigationWindow _navWin;        
        private string _loadFile;
        private string _inputString;
        private int _expectedValue;

        #endregion
        
        #region Constructor
        
        [Variation("Hyperlink_Bottomless_Inner_Block.xaml", "InnerBlock", 977)]
        [Variation("Hyperlink_Bottomless_Inner_Floater.xaml", "InnerFloater", 988)]
        [Variation("Hyperlink_Bottomless_Inner_Inline.xaml", "InnerInline", 465)]
        [Variation("Hyperlink_Bottomless_Inner_ListItem.xaml", "InnerListItem", 1023)]
        [Variation("Hyperlink_Bottomless_Inner_TableCell.xaml", "InnerTableCell", 1532)]
        [Variation("Hyperlink_Bottomless_Inner_UIElement.xaml", "InnerUIElement", 458)]
        [Variation("Hyperlink_Bottomless_InterDocInnerNav.xaml", "InterDocInnerNav", 977)]
        [Variation("Hyperlink_InnerDocNav_NestedFlowDocument.xaml", "InnerDocNav_NestedFlowDocument", 327)]
        [Variation("Hyperlink_FlowDocument_Inner_Block.xaml", "FlowDocument_Inner_Block", 5)]
        public InnerDocumentNavigationTest(string loadFile, string inputString, int expectedValue)
            : base()
        {
            this._inputString = inputString;
            this._loadFile = loadFile;
            this._expectedValue = expectedValue;

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        #endregion

        #region Test Steps
      
        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            Status("Initialize");
            _navWin = new NavigationWindow();
            _navWin.Navigated += new NavigatedEventHandler(navWin_Navigated);
            _navWin.Navigate(new Uri(System.IO.Path.Combine(System.Environment.CurrentDirectory, _loadFile)));
            _navWin.Top = 0;
            _navWin.Left = 0;            
            _navWin.ShowsNavigationUI = false;
            _navWin.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());
            _navWin.SizeToContent = SizeToContent.WidthAndHeight;
            _navWin.Show();
           
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _navWin.Close();
            return TestResult.Pass;
        }

        private void navWin_Navigated(object sender, NavigationEventArgs e)
        {
            //Every time the window navigates, set the content size
            if (_navWin.Content is FrameworkElement)
            {
                // Setting Window.Content size to ensure same size of root element over all themes.  
                // Different themes have different sized window chrome which will cause property dump 
                // and vscan failures even though the rest of the content is the same.
                // 784x564 is the content size of a 800x600 window in Aero theme.
                ((FrameworkElement)_navWin.Content).Height = 564;
                ((FrameworkElement)_navWin.Content).Width = 784;
                WaitFor(500); //Wait for a split second to make sure content has rendered.
            }
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return InvokeHyperlink();
        }

        #endregion

        #region Methods
        
        private TestResult InvokeHyperlink()
        {
            Hyperlink hl = LogicalTreeHelper.FindLogicalNode(_navWin, "HyperlinkObject") as Hyperlink;
            if (hl == null)
            {
                //Try to find the Hyperlink again
                WaitFor(1500);
                hl = LogicalTreeHelper.FindLogicalNode(_navWin, "HyperlinkObject") as Hyperlink;
            }

            if (hl != null)
            {
                hl.DoClick();                
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                return NavigationVerification();
            }
            else
            {
                TestLog.Current.LogEvidence("Could not find the Hyperlink in the document to click!");
                return TestResult.Fail;
            }
        }

        private TestResult NavigationVerification()
        {
            ScrollViewer sv =(ScrollViewer)LayoutUtility.GetChildFromVisualTree(_navWin, typeof(ScrollViewer));
            if (sv != null)
            {               
                if ((sv.VerticalOffset > _expectedValue - 2) && (sv.VerticalOffset < _expectedValue + 2))
                {
                    return TestResult.Pass;
                }
                else
                {
                    TestLog.Current.LogEvidence(string.Format("Expected a ScrollViewer vertical offset of {0}, actual value: {1}", _expectedValue, sv.VerticalOffset));
                    return TestResult.Fail;                    
                }
            }
            else //Verify FlowDocumentPageViewer page number
            {
                FlowDocumentPageViewer fdpv = (FlowDocumentPageViewer)LayoutUtility.GetChildFromVisualTree(_navWin, typeof(FlowDocumentPageViewer));
                if (fdpv != null)
                {
                    if (fdpv.MasterPageNumber != _expectedValue)
                    {
                        TestLog.Current.LogEvidence(string.Format("Expected FlowDocumentPageViewer.MasterPageNumber of {0}, actual value: {1}", _expectedValue, fdpv.MasterPageNumber));
                        return TestResult.Fail;
                    }
                    else
                    {
                        return TestResult.Pass;
                    }
                }
            }
            TestLog.Current.LogEvidence("Could not verify test b/c could not find the correct navigation container");
            return TestResult.Fail;
        }        
        #endregion
    }
}
