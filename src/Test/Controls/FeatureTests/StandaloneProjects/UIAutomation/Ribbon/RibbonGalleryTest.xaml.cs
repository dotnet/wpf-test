using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Microsoft.Test.Controls
{
    public partial class RibbonGalleryTest : Window
    {
        public RibbonGalleryTest()
        {
            InitializeComponent();
        }

        public ObservableCollection<TestSimpleGalleryItem> TestCategories 
        {
            get
            {
                if (testItems == null)
                {
                    testItems = new ObservableCollection<TestSimpleGalleryItem>();

                    AddItems("A");
                    AddItems("B");
                    AddItems("C");
                    AddItems("D");                   
                }

                return testItems;
            }           
        }
        private ObservableCollection<TestSimpleGalleryItem> testItems;

        private void AddItems(string categoryName)
        {
            for (int i = 0; i < 8; i++)
            {
                testItems.Add(new TestSimpleGalleryItem() { TestItemName = i.ToString(), TestCategoryName = categoryName });
            }
        }

        private void TestGallery_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectionChangedCheckBox != null)
            {
                SelectionChangedCheckBox.IsChecked = true;
            }
        }
    }

    public class TestSimpleGalleryItem
    {       
        public string TestItemName { get; set; }
        public string TestCategoryName { get; set; }
    }       
}
