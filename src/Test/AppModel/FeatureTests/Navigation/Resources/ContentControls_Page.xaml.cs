// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Interaction logic for ContentControls.xaml
    /// </summary>

    public partial class ContentControls : System.Windows.Controls.Page
    {
        private const String FLOWDOCURI = @"FlowDocument_Loose.xaml";


        public ContentControls()
        {
            InitializeComponent();
        }

        public void OnNavigateButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.GetNavigationService((DependencyObject)navigateButton).Navigate(new Uri(FLOWDOCURI, UriKind.RelativeOrAbsolute));
        }

    }
}
