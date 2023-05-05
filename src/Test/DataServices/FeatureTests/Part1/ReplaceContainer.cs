// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Replacing an item in a collection that is an ItemsSource does not raise the ItemsChanged of the ItemContainerGenerator.
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ReplaceContainer")]
    public class ReplaceContainer : XamlTest
    {
        #region Private Data

        private ItemsControl _itemsControl;
        private ObservableCollection<UIElement> _elements;

        
        #endregion

        #region Constructors

        public ReplaceContainer()
            : base(@"ReplaceContainer.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            WaitForPriority(DispatcherPriority.Loaded);

            _itemsControl = (ItemsControl)RootElement.FindName("itemsControl");

            if (_itemsControl == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }            

            // Create a collection with a button in it.
            _elements = new ObservableCollection<UIElement>();
            Button myButton = new Button();
            myButton.Content = "Initial Item";
            _elements.Add(myButton);

            // Use collection as the Item Source for the items control.
            _itemsControl.ItemsSource = _elements;

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Replace the existing button with another item that is its own container.
            Button newButton = new Button();
            newButton.Content = "Replacement Item";
            _elements[0] = newButton;

            // Grab the visuals for the itemsControl.
            Border border = (Border)VisualTreeHelper.GetChild(_itemsControl, 0);
            ItemsPresenter itemsPresenter = (ItemsPresenter)VisualTreeHelper.GetChild(border, 0);
            StackPanel stackPanel  = (StackPanel)VisualTreeHelper.GetChild(itemsPresenter, 0);
            Button actualButton = (Button)VisualTreeHelper.GetChild(stackPanel, 0);

            // Verify 
            if (actualButton.Content.ToString() != "Replacement Item")
            {
                LogComment("Content of Button not updated. Expected: \"Replacement Item\". Actual: " + actualButton.Content.ToString());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    #endregion
}
