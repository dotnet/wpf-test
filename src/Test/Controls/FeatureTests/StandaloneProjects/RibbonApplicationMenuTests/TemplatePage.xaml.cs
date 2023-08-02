using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Windows.Controls.Ribbon;
using System.ComponentModel;

namespace RibbonApplicationMenuTests
{
    /// <summary>
    /// Interaction logic for TemplatePage.xaml
    /// </summary>
    public partial class TemplatePage : Page
    {
        public TemplatePage()
        {
            InitializeComponent();
            RAM.DataContext = new List<myTinyViewModel>() {     new myTinyViewModel() { name = "blah", val = 5 },
                                                                new myTinyViewModel() { name = "meh", val = 6 } ,
                                                                new myTinyViewModel() { name = "foo", val = 7 } ,
                                                                new myTinyViewModel() { name = "var", val = 8 } ,
                                                                new myTinyViewModel() { name = "og", val = 9 } ,
                                                                new myTinyViewModel() { name = "smurf", val = 10 } 
                                                                };
        }

        #region Public Properties
        

        #endregion

        #region Fields
        private static Type OwnerType = typeof(TemplatePage);
        #endregion

        /// <summary>
        /// Perform API Tests against the ribbon application menu
        /// </summary>
        public bool DoAPITests()
        {            

            return true;
        }

    }


    public class myTinyViewModel
    {
        public string name
        {
            get;
            set;
        }
        public int val
        {
            get;
            set;
        }
    }

    public class myDataTemplateSelector
        : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return Application.Current.MainWindow.FindResource("firstTemplate") as DataTemplate;
        }
    }

}
