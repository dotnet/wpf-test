// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test : Binary Back Compat:DataTemplates in Tiles not being applied correctly on UMPC Now App
    /// The issue is about template resolution when a ContentPresenter's
    /// ContentPresenter property is itself databound.
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>            
    [Test(1, "Styles", "RegressionContentPresenterContentTemplateBound")]
    public class RegressionContentPresenterContentTemplateBound : XamlTest
    {
        #region Private Data

        private static DockPanel s_instance;

        #endregion

        #region Constructors

        public RegressionContentPresenterContentTemplateBound()
            : base(@"Markup\RegressionContentPresenterContentTemplateBound.xaml")
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Private Members

        private TestResult Initialize()
        {
            s_instance = (DockPanel)RootElement;
            return TestResult.Pass;
        }

        private TestResult Verify()
        {
            Label MyLabel = (Label)Instance.FindName("MyLabel");
            MyLabel.Content = Instance.FindResource("Tile");

            // Wait for template for MyLabel's content to be expanded and rendered.
            WaitForPriority(System.Windows.Threading.DispatcherPriority.SystemIdle);

            // Spelunk visual tree to get to the textblock in the SummaryViewTemplate DataTemplate.
            Border labelBorder = (Border)VisualTreeHelper.GetChild(MyLabel, 0);
            ContentPresenter labelContentPresenter = (ContentPresenter)labelBorder.Child;
            ContentPresenter dataTemplateContentPresenter = (ContentPresenter)VisualTreeHelper.GetChild(labelContentPresenter, 0);
            TextBlock dataTemplateTextBlock = (TextBlock)VisualTreeHelper.GetChild(dataTemplateContentPresenter, 0);

            if (dataTemplateTextBlock.Text == "1")
            {
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogEvidence("TextBlock's text was not expected value of 1, actual value was " + dataTemplateTextBlock.Text + ".", dataTemplateTextBlock.Text);
                return TestResult.Fail;
            }
        }

        #endregion

        #region Static Members

        public static DockPanel Instance { get { return s_instance; } }

        #endregion
    }

    // A container class derived from the bug repro. It's purpose is provide a
    // property that returns a DataTemplate. This allows us to bind a
    // ContentPresenter's ContentTemplate property. (Since the content of the
    // ContentPresenter is the Tile, the DataContext for the ContentTemplate
    // binding is the Tile, and therefore the binding will look for a property
    // on Tile.
    public class Tile
    {
        public int ID { get { return 1; } }

        public DataTemplate SummaryViewTemplate
        {
            get
            {
                return RegressionContentPresenterContentTemplateBound.Instance.FindResource("SummaryViewTemplate") as DataTemplate;
            }
        }
    }
}
