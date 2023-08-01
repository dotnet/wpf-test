// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Animation;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.ElementServices.Resources.RegressionTests
{
    /// <summary>
    /// Regression tests for Resources-related bugs (Xaml tests).
    /// </summary>
    [Test(1, @"Resources.RegressionTests", TestCaseSecurityLevel.FullTrust, "ResourceBugsXaml", SupportFiles=@"FeatureTests\ElementServices\RegressionTests\*.xaml")]
    public class ResourceBugsXaml : XamlTest
    {
        #region Private Data

        private string          _inputString = "";
        private bool            _testPassed  = false;

        #endregion


        #region Constructor

        [Variation("RegressionResourcesVisualBrush.xaml")]
        [Variation("RegressionResourcesSource.xaml")]
        [Variation("RegressionFreezeResource.xaml")]
        [Variation("RegressionMergedDictionariesAdd.xaml")]
        [Variation("RegressionMergedDictionariesAddCycle.xaml")]
        [Variation("RegressionSharedFalse.xaml", Disabled = true)] // Broken for both 4.0 and 3.X, disabled.  
        [Variation("RegressionDynamicRerender.xaml")]

        /******************************************************************************
        * Function:          ResourceBugsXaml Constructor
        ******************************************************************************/
        public ResourceBugsXaml(string testValue): base(testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: Set Window properties.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("---Test Case Variation: " + _inputString);

            Window.Left                 = 0d;
            Window.Top                  = 0d;
            Window.Height               = 250d;
            Window.Width                = 250d;
            Window.Topmost              = true;
            Window.WindowStyle          = WindowStyle.None;

            return TestResult.Pass;
        }

        
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// StartTest:  begin the test case, depending on the variation passed in.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult StartTest()
        {
            string              fileName = _inputString + ".xaml";
            Grid                grid;
            Page                page;
            Panel               panel;
            Button              button;
            StackPanel          stackPanel;
            ResourceDictionary  pageResources;
            ResourceDictionary  dict1;
            ResourceDictionary  dict2;
            ResourceDictionary  dict3;
            Color               expectedColor;
            Color               actualColor;
            _testPassed  = false;

            switch (_inputString)
            {
                // Regression test for 

                case "RegressionResourcesVisualBrush.xaml":
                    grid = (Grid)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"grid1");
                    if (grid == null)
                    {
                        throw new TestValidationException("ERROR!!! Grid is null");
                    }

                    ResourceDictionary rd = grid.Resources;
                    expectedColor = (Color)rd["BaseColor"];
                    GlobalLog.LogEvidence("Expected Color: " + expectedColor);

                    VisualBrush visualBrush = (VisualBrush)grid.Background;
                    Canvas canvas = (Canvas)visualBrush.Visual;

                    RadialGradientBrush rgb = (RadialGradientBrush)canvas.Background;
                    GradientStopCollection gsc = rgb.GradientStops;
                    actualColor = gsc[1].Color;
                    GlobalLog.LogEvidence("Actual Color:   " + actualColor);

                    if (Color.Equals(actualColor, expectedColor))
                    {
                        _testPassed = true;
                    }
                    break;

                // Regression test for 

                case "RegressionResourcesSource.xaml":
                    grid = (Grid)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"documentRoot");
                    if (grid == null)
                    {
                        throw new TestValidationException("ERROR!!! Grid is null");
                    }

                    string strUri2 = "pack://siteoforigin:,,,/RegressionResourcesSource2.xaml";
                    grid.Resources.Source = new Uri(strUri2, System.UriKind.RelativeOrAbsolute);

                    expectedColor = Colors.Red;
                    GlobalLog.LogEvidence("Expected Color: " + expectedColor);

                    button = (Button)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"button1");
                    if (button == null)
                    {
                        throw new TestValidationException("ERROR!!! Button is null");
                    }

                    SolidColorBrush scb = (SolidColorBrush)button.Background;
                    actualColor = scb.Color;
                    GlobalLog.LogEvidence("Actual Color:   " + actualColor);

                    if (Color.Equals(actualColor, expectedColor))
                    {
                        _testPassed = true;
                    }
                    break;

                // Regression test for 


                case "RegressionFreezeResource.xaml":
                    grid = (Grid)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"LayoutRoot");
                    if (grid == null)
                    {
                        throw new TestValidationException("ERROR!!! Grid is null");
                    }

                    Rectangle rect = (Rectangle)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"Source1");
                    if (rect == null)
                    {
                        throw new TestValidationException("ERROR!!! Rectangle is null");
                    }

                    string str142680a = "pack://siteoforigin:,,,/RegressionFreezeResource2.xaml";
                    string str142680b = "pack://siteoforigin:,,,/RegressionFreezeResource3.xaml";

                    //Step 1: apply the first Brush.
                    grid.Resources.Source = new Uri(str142680a, System.UriKind.RelativeOrAbsolute);

                    //Step 2: freeze the applied Brush.
                    Brush brush = rect.Fill;
                    brush.Freeze();

                    //Step 3: apply the first Brush.
                    grid.Resources = null;
                    grid.Resources.Source = new Uri(str142680b, System.UriKind.RelativeOrAbsolute);

                    expectedColor = ((SolidColorBrush)grid.Resources["Brush1"]).Color;
                    GlobalLog.LogEvidence("Expected Color: " + expectedColor);

                    actualColor = ((SolidColorBrush)rect.Fill).Color;
                    GlobalLog.LogEvidence("Actual Color:   " + actualColor);

                    if (Color.Equals(actualColor, expectedColor))
                    {
                        _testPassed = true;
                    }
                    break;

                // Regression test for 

                case "RegressionMergedDictionariesAdd.xaml":
                    page = (Page)Window.Content;
                    if (page == null)
                    {
                        throw new TestValidationException("ERROR!!! Page is null");
                    }

                    pageResources = page.Resources;
                    dict1 = pageResources.MergedDictionaries[0]; //Contains the SCB applied to the Button.
                    dict2 = pageResources.MergedDictionaries[1];
                    dict3 = pageResources.MergedDictionaries[2];
                    dict1.MergedDictionaries.Add(dict2);

                    expectedColor = ((SolidColorBrush)dict1["MyBrush"]).Color;
                    GlobalLog.LogEvidence("Expected Color: " + expectedColor);

                    button = (Button)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"button1");
                    if (button == null)
                    {
                        throw new TestValidationException("ERROR!!! Button is null");
                    }

                    actualColor = ((SolidColorBrush)button.Background).Color;
                    GlobalLog.LogEvidence("Actual Color:   " + actualColor);

                    if (Color.Equals(actualColor, expectedColor))
                    {
                        _testPassed = true;
                    }
                    break;

                // Regression test for 

                case "RegressionMergedDictionariesAddCycle.xaml":
                    page = (Page)Window.Content;
                    if (page == null)
                    {
                        throw new TestValidationException("ERROR!!! Page is null");
                    }

                    pageResources = page.Resources;
                    dict1 = pageResources.MergedDictionaries[0]; //Contains the SCB applied to the Button.
                    dict2 = pageResources.MergedDictionaries[1];
                    dict3 = pageResources.MergedDictionaries[2];
                    dict1.MergedDictionaries.Add(dict2);
                    dict2.MergedDictionaries.Add(dict3);

                    // (1) Verify the correct Exception is thrown.
                    GlobalLog.LogStatus("Add to MergedDictionaries, creating a cycle");
                    ExceptionHelper.ExpectException<InvalidOperationException>
                    (
                        delegate() { dict3.MergedDictionaries.Add(dict1); },
                        delegate(InvalidOperationException e) { ;}
                    );

                    GlobalLog.LogEvidence("Correct Exception thrown: InvalidOperationException");

                    // (2) Also verify that the correct brush from the ResourceDictionary is applied.
                    expectedColor = ((SolidColorBrush)dict1["MyBrush"]).Color;
                    GlobalLog.LogEvidence("Expected Color: " + expectedColor);

                    button = (Button)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"button1");
                    if (button == null)
                    {
                        throw new TestValidationException("ERROR!!! Button is null");
                    }

                    actualColor = ((SolidColorBrush)button.Background).Color;
                    GlobalLog.LogEvidence("Actual Color:   " + actualColor);

                    if (Color.Equals(actualColor, expectedColor))
                    {
                        _testPassed = true;
                    }
                    break;

                // Regression test for 











                case "RegressionDynamicRerender.xaml":
                    stackPanel = (StackPanel)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"root");

                    panel = (Panel)stackPanel.FindName("panel");
                    if (panel == null)
                    {
                        throw new TestValidationException("ERROR!!! Panel is null");
                    }

                    button = (Button)stackPanel.FindName("button");
                    if (button == null)
                    {
                        throw new TestValidationException("ERROR!!! Button is null");
                    }

                    // Validate the initial button background color is red.
                    if (!button.Background.ToString().Equals(Brushes.Red.ToString()))
                    {
                        throw new TestValidationException("ERROR!!! The button background is not Red.");
                    }

                    //Change the Style applied to the button.
                    panel.Resources.Add("TestStyle", panel.Resources["WaitingInTheWings"]);

                    expectedColor = Colors.Green;
                    actualColor = ((SolidColorBrush)button.Background).Color;

                    GlobalLog.LogEvidence("Expected Color: " + expectedColor);
                    GlobalLog.LogEvidence("Actual Color:   " + actualColor);

                    if (Color.Equals(actualColor, expectedColor))
                    {
                        _testPassed = true;
                    }
                    break;

                // Regression test for 

                case "RegressionSharedFalse.xaml":

                    Window.Title = "Regression Test ";
                    VisualVerifier verifier = new VisualVerifier();
                    verifier.InitRender(Window);

                    actualColor = verifier.getColorAtPoint(100,100);
                    expectedColor = Colors.Green;

                    GlobalLog.LogEvidence("Actual Color:   " + actualColor);
                    GlobalLog.LogEvidence("Expected Color: " + expectedColor);

                    if (Color.Equals(actualColor, expectedColor))
                    {
                        _testPassed = true;
                    }
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR!!! StartTest: Unexpected failure to match argument.");
                    _testPassed = false;
                    break;
            }
            
            // Return the final result for a given test case.
            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion
    }
}

