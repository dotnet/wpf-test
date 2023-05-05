// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Containers should be cleaned up after items are removed
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ContainerCleanup")]
    public class ContainerCleanup : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        
        #endregion

        #region Constructors

        public ContainerCleanup()
            : base(@"ContainerCleanup.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            
            if (_myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // If this bug Regresses, the following code will cause a resource not found exception
            MainItem mainItem0 = Items.Collection[0];
            MainItem mainItem1 = Items.Collection[1];
            Items.Collection.RemoveAt(1);
            Items.Collection.RemoveAt(0);
            Items.Collection.Insert(0, mainItem1);
            Items.Collection.Insert(1, mainItem0);

            SubItem subItem0 = mainItem1.Items[0];
            SubItem subItem1 = mainItem1.Items[1];
            mainItem1.Items.RemoveAt(1);
            mainItem1.Items.RemoveAt(0);
            mainItem1.Items.Insert(0, subItem1);
            mainItem1.Items.Insert(1, subItem0);

            
            LogComment("No unexpected exceptions thrown.");
            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class SubItem
    {
        private string _itemName;

        public string Name
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        public SubItem(string name)
        {
            _itemName = name;
        }
    }

    public static class Items
    {

        private static readonly ObservableCollection<MainItem> s_itemValue = Init();

        public static ObservableCollection<MainItem> Collection
        {
            get { return Items.s_itemValue; }
        }

        private static ObservableCollection<MainItem> Init()
        {
            ObservableCollection<MainItem> collec = new ObservableCollection<MainItem>();
            MainItem item = new MainItem("MainItem1");
            item.Items.Add(new SubItem("SubItem1"));
            item.Items.Add(new SubItem("SubItem2"));
            collec.Add(item);
            item = new MainItem("MainItem2");
            item.Items.Add(new SubItem("SubItem1"));
            item.Items.Add(new SubItem("SubItem2"));
            collec.Add(item);
            return collec;
        }

    }


    public class MainItem
    {
        private string _itemName;
        private readonly ObservableCollection<SubItem> _items = new ObservableCollection<SubItem>();

        public MainItem(string name)
        {
            _itemName = name;
        }

        public string Name
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        public ObservableCollection<SubItem> Items
        {
            get { return _items; }
        }
    }

    public class ItemStyleSelector : StyleSelector
    {

        public override Style SelectStyle(object item, DependencyObject container)
        {
            ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
            return itemsControl.FindResource("ItemStyle") as Style;
        }

    }
    
    #endregion
}
