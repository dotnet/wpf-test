using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Test.Controls.UIADataSources;

namespace Microsoft.Test.Controls
{
    public partial class DataGridRegressionTest5 : Window
    {
        public DataGridRegressionTest5()
        {
            InitializeComponent();

            dg.ItemsSource = new UIAPeople();
        }      
    }  
}
