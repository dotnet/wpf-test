// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace LineInfoServiceProviderTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MainWindowBase
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (AttributeSyntaxLineInfo != "True 5 9" || PropertyElementSyntaxLineInfo != "True 9 10")
            {
                Console.WriteLine("Wrong line info in DEBUG build. AttributeSyntaxLineInfo:" + AttributeSyntaxLineInfo + " PropertyElementSyntaxLineInfo:" + PropertyElementSyntaxLineInfo);
                Console.Out.Flush();
                Application.Current.Shutdown(1);
            }
#else
            if (AttributeSyntaxLineInfo != "False 0 0" || PropertyElementSyntaxLineInfo != "False 0 0")
            {
                Console.WriteLine("Wrong line info in RELEASE build. AttributeSyntaxLineInfo:" + AttributeSyntaxLineInfo + " PropertyElementSyntaxLineInfo:" + PropertyElementSyntaxLineInfo);
                Console.Out.Flush();
                Application.Current.Shutdown(1);
            }
#endif

            Console.Out.Flush();
            Application.Current.Shutdown(0);
        }
    }

    public class MainWindowBase : Window
    {
        public string AttributeSyntaxLineInfo { get; set; }

        public string PropertyElementSyntaxLineInfo { get; set; }
    }

    public class GetLineInfo : MarkupExtension
    {
        public GetLineInfo()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IXamlLineInfo lineInfo = (IXamlLineInfo)serviceProvider.GetService(typeof(IXamlLineInfo));
            return lineInfo.HasLineInfo.ToString(CultureInfo.InvariantCulture) 
                + " " + lineInfo.LineNumber.ToString(CultureInfo.InvariantCulture) 
                + " " + lineInfo.LinePosition.ToString(CultureInfo.InvariantCulture);
        }
    }
}
