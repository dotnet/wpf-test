// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Test.Logging;

namespace MarkupCompiler.RegressionIssue107
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify that locally defined MarkupExtension supports a markup extension setting a property value
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            
            Microsoft.Test.Logging.LogManager.BeginTest(Microsoft.Test.DriverState.TestName);
            Microsoft.Test.Logging.TestLog log = TestLog.Current;
            if (((TestExtension)button1.Tag).TypeValue == typeof(Panel) && 
                ((TestExtension)button2.Tag).TypeValue == typeof(Panel) && 
                ((TestExtension)button3.Tag).TypeValue == typeof(Panel))
            {
                log.Result = TestResult.Pass;
                Application.Current.Shutdown(0);
            }
            else
            {
                log.Result = TestResult.Fail;
                Application.Current.Shutdown(-1);
            }
        }
    }

    [MarkupExtensionReturnType(typeof(TestExtension))]
    public class TestExtension : MarkupExtension
    {
        public TestExtension()
        {
        }

        public TestExtension(Type typeValue)
        {
            _typeValue = typeValue;
        }

        private Type _typeValue;

        public Type TypeValue
        {
            get
            {
                return _typeValue;
            }
            set
            {
                _typeValue = value;
            }
        }

        public override string ToString()
        {
            return (_typeValue == null ? "(null)" : _typeValue.ToString());
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
