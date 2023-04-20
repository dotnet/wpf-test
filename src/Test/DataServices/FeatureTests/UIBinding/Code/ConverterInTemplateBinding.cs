// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using System.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests using a Converter in a TemplateBinding.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>



    [Test(2, "Binding", "ConverterInTemplateBinding")]
    public class ConverterInTemplateBinding : XamlTest
    {
        private Button _isTabStopTrue;
        private Button _isTabStopFalse;
        private ControlTemplate _ctButton;

        public ConverterInTemplateBinding()
            : base(@"ConverterInTemplateBinding.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestParameterConverter);
            RunSteps += new TestStep(TestConverter);
            RunSteps += new TestStep(TestSetNullConverter);
            RunSteps += new TestStep(TestSetNullProperty);
            RunSteps += new TestStep(TestPassNullConstructor);
        }

        public TestResult Setup()
        {
            Status("Setup");

            _isTabStopTrue = LogicalTreeHelper.FindLogicalNode(RootElement, "IsTabStopTrue") as Button;
            if (_isTabStopTrue == null)
            {
                LogComment("Fail - Unable to reference Button IsTabStopTrue");
                return TestResult.Fail;
            }
            _isTabStopFalse = LogicalTreeHelper.FindLogicalNode(RootElement, "IsTabStopFalse") as Button;
            if (_isTabStopFalse == null)
            {
                LogComment("Fail - Unable to reference Button IsTabStopFalse");
                return TestResult.Fail;
            }
            _ctButton = RootElement.Resources["ctButton"] as ControlTemplate;
            if (_ctButton == null)
            {
                LogComment("Fail - Unable to reference ControlTemplate ctButton");
                return TestResult.Fail;
            }

            Status("Setup was successful");
            return TestResult.Pass;
        }

        public TestResult TestParameterConverter()
        {
            Status("TestParameterConverter");

            BooleanToColorConverterWithParam converter = RootElement.Resources["converter"] as BooleanToColorConverterWithParam;
            if (converter == null)
            {
                LogComment("Fail - Converter is null");
                return TestResult.Fail;
            }

            if (converter.Failed == true)
            {
                LogComment("Fail - Parameter passed to constructor not as expected");
                return TestResult.Fail;
            }

            Status("TestParameterConverter was successful");
            return TestResult.Pass;
        }

        public TestResult TestConverter()
        {
            Status("TestConverter");

            Border borderIsTabStopTrue = VisualTreeHelper.GetChild(_isTabStopTrue, 0) as Border;
            if (borderIsTabStopTrue.BorderBrush != Brushes.Firebrick)
            {
                LogComment("Fail - BorderBrush should be Firebrick. Converter did not work as expected.");
                return TestResult.Fail;
            }

            Border borderIsTabStopFalse = VisualTreeHelper.GetChild(_isTabStopFalse, 0) as Border;
            if (borderIsTabStopFalse.BorderBrush != Brushes.LightCoral)
            {
                LogComment("Fail - BorderBrush should be LightCoral. Converter did not work as expected.");
                return TestResult.Fail;
            }

            Status("TestConverter was successful");
            return TestResult.Pass;
        }

        public TestResult TestSetNullConverter()
        {
            Status("TestSetNullConverter");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));

            TemplateBindingExtension tbe = new TemplateBindingExtension();
            tbe.Property = Button.IsTabStopProperty;
            tbe.ConverterParameter = 0;
            tbe.Converter = null; // this should throw ArgumentNullException

            Status("TestSetNullConverter was successful");
            return TestResult.Pass;
        }

        public TestResult TestSetNullProperty()
        {
            Status("TestSetNullProperty");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));

            TemplateBindingExtension tbe = new TemplateBindingExtension();
            tbe.Converter = RootElement.Resources["converter"] as IValueConverter;
            tbe.ConverterParameter = 0;
            tbe.Property = null; // this should throw ArgumentNullException

            Status("TestSetNullProperty was successful");
            return TestResult.Pass;
        }

        public TestResult TestPassNullConstructor()
        {
            Status("TestPassNullConstructor");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            TemplateBindingExtension tbe = new TemplateBindingExtension(null);

            Status("TestPassNullConstructor was successful");
            return TestResult.Pass;
        }
    }

    public class BooleanToColorConverterWithParam : IValueConverter
    {
        private bool _failed;

        public bool Failed
        {
            get { return _failed; }
            set { _failed = value; }
        }

        public BooleanToColorConverterWithParam()
        {
            Failed = false;
        }

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            int param = Int32.Parse(parameter.ToString());
            if (param != 0)
            {
                GlobalLog.LogStatus("Fail - Parameter passed is incorrect");
                Failed = true;
            }

            if ((bool)o)
            {
                return Brushes.Firebrick;
            }
            else
            {
                return Brushes.LightCoral;
            }
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

