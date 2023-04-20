// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Tests binding to the DataContext property, both inside and outside a Template.
    /// Scenario 1 - Test binding to DataContext outside template.
    /// Scenario 2 - Test binding to DataContext in DataTemplate.
    /// Scenario 3 - Remove source DataContext, make sure binding in target DataContext gets updated using the correct DataContext.
    /// Scenario 4 - ContentPresenter in template with Content explicitly data bound.
	/// </description>
    /// <relatedTasks>

    /// </relatedTasks>
    /// <relatedBugs>

    /// </relatedBugs>
	/// </summary>
    [Test(1, "Binding", "BindToDataContext")]
	public class BindToDataContext : XamlTest
	{
        private ListBox _lb1;
        private ListBox _lb2;
        private ListBox _lb3;
        private ListBox _lb4;
        private TextBlock _tb;
        private Button _btn;
        private StackPanel _sp1;
        private StackPanel _sp2;

        public BindToDataContext()
            : base(@"BindToDataContext.xaml")
		{
			InitializeSteps += new TestStep(Setup);
            // Scenario 1 - Test binding to DataContext outside template
			RunSteps += new TestStep(OutsideTemplate);
            // Scenario 2 - Test binding to DataContext in DataTemplate.
            RunSteps += new TestStep(InsideTemplate);
            //Scenario 3 - Remove source DataContext, make sure binding in target DataContext gets updated using the correct DataContext.
            RunSteps += new TestStep(RemoveDataContext);
            //Scenario 4 - ContentPresenter in template with Content explicitly data bound.
            RunSteps += new TestStep(ContentPresenterInTemplate);
        }

		private TestResult Setup()
		{
			Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _lb1 = (ListBox)LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb1");
            _lb2 = (ListBox)LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb2");
            _lb3 = (ListBox)LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb3");
            _lb4 = (ListBox)LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb4");
            _tb = (TextBlock)LogicalTreeHelper.FindLogicalNode(this.RootElement, "tb");
            _btn = (Button)LogicalTreeHelper.FindLogicalNode(this.RootElement, "btn");
            _sp1 = (StackPanel)LogicalTreeHelper.FindLogicalNode(this.RootElement, "sp1");
            _sp2 = (StackPanel)LogicalTreeHelper.FindLogicalNode(this.RootElement, "sp2");

			return TestResult.Pass;
        }

        #region OutsideTemplate
        private TestResult OutsideTemplate()
        {
            Status("OutsideTemplate");
            WaitForPriority(DispatcherPriority.SystemIdle);

            string[] expectedItems = new string[2] { "Western Hemisphere", "Eastern Hemisphere" };
            if (!TestItemsControlHemisphere(_lb1, expectedItems)) { return TestResult.Fail; }
            if (!TestItemsControlHemisphere(_lb2, expectedItems)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        private bool TestItemsControlHemisphere(ItemsControl ic, string[] expectedItemContent)
        {
            if (ic.Items.Count != expectedItemContent.Length)
            {
                LogComment("Fail - Actual number of items: " + ic.Items.Count + ". Expected: " + expectedItemContent.Length);
                return false;
            }
            for(int i=0; i<ic.Items.Count; i++)
            {
                if (((Hemisphere)ic.Items[i]).HemisphereName != expectedItemContent[i])
                {
                    LogComment("Fail - Item not as expected. Actual: " + ((Hemisphere)ic.Items[i]).HemisphereName + ". Expected: " + expectedItemContent[i]);
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region InsideTemplate
        private TestResult InsideTemplate()
        {
            Status("InsideTemplate");
            string[][] expectedItems = new string[2][];
            string[] westernHemisphere = new string[4] { "The Caribbean", "Central America", "South America", "North America" };
            string[] easternHemisphere = new string[6] { "Europe", "Asia", "Oceania", "Middle East", "Africa", "Southeast Asia" };
            expectedItems[0] = westernHemisphere;
            expectedItems[1] = easternHemisphere;
            if (!TestItemsControlHemispheresRegions(_lb3, expectedItems)) { return TestResult.Fail; }
            if (!TestItemsControlHemispheresRegions(_lb4, expectedItems)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        private bool TestItemsControlHemispheresRegions(ItemsControl ic, string[][] expectedItemContent)
        {
            // 2 Hemispheres
            if (ic.Items.Count != expectedItemContent.Length)
            {
                LogComment("Fail - Actual number of items: " + ic.Items.Count + ". Expected: " + expectedItemContent.Length);
                return false;
            }            
            FrameworkElement[] lbsInTemplate = Util.FindElements(ic, "lbTemplate");
            for (int i = 0; i < ic.Items.Count; i++) 
            {
                Hemisphere hem = (Hemisphere)ic.Items[i];
                ListBox currentLbInTemplate = (ListBox)lbsInTemplate[i];
                // 4 or 6 Regions
                if (currentLbInTemplate.Items.Count != hem.Regions.Count)
                {
                    LogComment("Fail - Actual number of items: " + currentLbInTemplate.Items.Count + ". Expected: " + hem.Regions.Count);
                    return false;
                }
                for (int j = 0; j < hem.Regions.Count; j++)
                {
                    if (hem.Regions[j].RegionName != ((Region)currentLbInTemplate.Items[j]).RegionName)
                    {
                        LogComment("Fail - Actual region: " + ((Region)currentLbInTemplate.Items[j]).RegionName + ". Expected: " + hem.Regions[j].RegionName);
                        return false;
                    }
                }
            }
            
            return true;
        }
        #endregion

        private TestResult RemoveDataContext()
        {
            Status("RemoveDataContext");

            if (_tb.Text != "Eastern Hemisphere")
            {
                LogComment("Fail - Expected value for TextBlock: Eastern Hemisphere. Actual: " + _tb.Text);
                return TestResult.Fail;
            }

            _sp2.Children.Clear();
            _sp1.Children.Clear();
            _sp1.Children.Add(_tb);

            WaitForPriority(DispatcherPriority.SystemIdle);

            // Won't Fix - binding does not update when DataContext is removed
            if (_tb.Text != "Eastern Hemisphere")
            {
                LogComment("Fail - Expected value for TextBlock: Eastern Hemisphere. Actual: " + _tb.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult ContentPresenterInTemplate()
        {
            Status("ContentPresenterInTemplate");

            ContentPresenter cp = (ContentPresenter)Util.FindElement(_btn, "cp");
            if (cp.Content.ToString() != "Hello")
            {
                LogComment("Fail - Expected ContentPresenter's Content: Hello. Actual: " + cp.Content);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}

