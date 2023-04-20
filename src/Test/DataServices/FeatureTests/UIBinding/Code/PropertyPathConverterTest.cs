// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Media;
using System.ComponentModel;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Test PropertyPathConverter unit test to improve codecoverage.
	/// </description>
	/// </summary>
    [Test(2, "Binding", "PropertyPathConverterTest")]
    public class PropertyPathConverterTest : XamlTest
    {

        PropertyPathConverter _converter;
        HappyMan _happyPerson;
        Button _specialButton;
        TextBlock _txtTest;

        public PropertyPathConverterTest()
            : base("happymanMarkup.xaml")
        {
            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(CanConvertFromInt);
            RunSteps += new TestStep(CanConvertToInt);
            RunSteps += new TestStep(ConvertFromNullException);
            RunSteps += new TestStep(ConvertFromIntException);
            RunSteps += new TestStep(ConvertToNullValueException);
            RunSteps += new TestStep(ConvertToNullTypeException);
            RunSteps += new TestStep(ConvertToIntException);
            RunSteps += new TestStep(ConvertToNotPathException);

            RunSteps += new TestStep(TestToStringHappy);
            RunSteps += new TestStep(SettingPathBinding);
            RunSteps += new TestStep(TestToStringText);
        }

        private TestResult SetUp()
        {
            _converter = new PropertyPathConverter();
            if (_converter == null)
            {
                LogComment("Could not create a PropertyPathConverter");
                return TestResult.Fail;
            }

            _happyPerson = (HappyMan)Util.FindElement(RootElement, "HappyPerson");

            if (_happyPerson == null)
            {
                LogComment("Could not refernce the HappyMan control");
                return TestResult.Fail;
            }

            _specialButton = (Button)Util.FindElement(RootElement, "SpecialButton");
            if (_specialButton == null)
            {
                LogComment("Could not refernce the SpecialButton control");
                return TestResult.Fail;
            }

            _txtTest = (TextBlock)Util.FindElement(RootElement, "txtTest");
            if (_txtTest == null)
            {
                LogComment("Could not refernce the txtTest control");
                return TestResult.Fail;
            }

            LogComment("PropertyPathConverter was created");

            return TestResult.Pass;
        }

        TestResult CanConvertFromInt()
        {
            if (_converter.CanConvertFrom(typeof(int)))
            {
                LogComment("Converter can convert from int, expected that it could not.");
                return TestResult.Fail;
            }
            LogComment("Correct value for convert from");
            return TestResult.Pass;
        }

        TestResult CanConvertToInt()
        {
            if (_converter.CanConvertTo(typeof(int)))
            {
                LogComment("Converter can convert to int, expected that it could not.");
                return TestResult.Fail;
            }
            LogComment("Correct value for convert to");
            return TestResult.Pass;
        }

        TestResult ConvertFromNullException()
        {
            Status("Attempting to convert from null");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            object o = _converter.ConvertFrom(null);

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult ConvertFromIntException()
        {
            Status("Attempting to convert from int");

            int i = 25;

            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            object o = _converter.ConvertFrom(i);

            LogComment("Expected ArgumentException did not occur");
            return TestResult.Fail;
        }

        TestResult ConvertToNullValueException()
        {
            Status("Attempting to convert from null");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            object o = _converter.ConvertTo(null, typeof(string));

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult ConvertToNullTypeException()
        {
            Status("Attempting to convert from null");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            object o = _converter.ConvertTo(new object(), null);

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult ConvertToIntException()
        {
            Status("Attempting to convert from null");

            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            object o = _converter.ConvertTo(new object(), typeof(int));

            LogComment("Expected ArgumentException did not occur");
            return TestResult.Fail;
        }

        TestResult ConvertToNotPathException()
        {
            Status("Attempting to convert from null");

            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            object o = _converter.ConvertTo(new object(), typeof(string));

            LogComment("Expected ArgumentException did not occur");
            return TestResult.Fail;
        }

        TestResult TestToStringHappy()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            WaitFor(100);

            if (_happyPerson.HappyName != "Marvin")
            {
                LogComment("Expected Marvin for the HappyName property, actual " + _happyPerson.HappyName);
                return TestResult.Fail;
            }

            Binding binding = BindingOperations.GetBinding(_happyPerson, HappyMan.HappyNameProperty);
            if (binding == null)
            {
                LogComment("Could not retrieve HappyMan.HappyNameProperty binding");
                return TestResult.Fail;
            }

            PropertyPath pp = binding.Path;
            string str = (string)_converter.ConvertTo(pp, typeof(string));
            string exp = "Name";
            if (!str.Equals(exp))
            {
                LogComment("Expected: " + exp + "  actual: " + str);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }


        TestResult SettingPathBinding()
        {
            Binding b = new Binding();
            b.Source = _specialButton;
            b.Path = new PropertyPath("(1).(0)");
            b.Path.PathParameters.Add(HappyMan.HappyNameProperty);
            b.Path.PathParameters.Add(Button.ContentProperty);
            _txtTest.SetBinding(TextBlock.TextProperty, b);

         
            return TestResult.Pass;
        }


        TestResult TestToStringText()
        {
            WaitForPriority(DispatcherPriority.Background);
            WaitFor(100);
            Binding binding = BindingOperations.GetBinding(_txtTest, TextBlock.TextProperty);
            if (binding == null)
            {
                LogComment("Could not retrieve Text binding");
                return TestResult.Fail;
            }

            PropertyPath pp = binding.Path;
            string str = (string)_converter.ConvertTo(pp, typeof(string));
            string exp = "(ContentControl.Content).(HappyMan.HappyName)";
            if (!str.Equals(exp))
            {
                LogComment("Expected: " + exp + "  actual: " + str);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}

