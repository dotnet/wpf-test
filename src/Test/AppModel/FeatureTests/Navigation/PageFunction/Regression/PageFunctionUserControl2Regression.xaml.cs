// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Interaction logic for PageFunctioningUserControl.xaml
    /// </summary>
    public partial class PageFunctioningUserControl : UserControl
    {
        public PageFunctioningUserControl()
        {
            InitializeComponent();
        }

        void StringPF_Return(object sender, ReturnEventArgs<string> e)
        {
            MessageBox.Show("Got return event!");
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            PageFunction1 stringPF = new PageFunction1();
            stringPF.Return += new ReturnEventHandler<string>(StringPF_Return);
            NavigationService.GetNavigationService(this).Navigate(stringPF);   
        }
    }
}
