// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using System.Globalization;
using System.Collections;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{


    [Test(0, "Binding", "MultiBindTest")]
    public class MultiBindTest : XamlTest
    {
        TextBox _textbox1;
        TextBox _textbox2;
        TextBox _textbox3;
        TextBlock _text;

        public MultiBindTest() : base(@"converter.xaml")
        {
            RunSteps += new TestStep(Init);
            RunSteps += new TestStep(InitValidation);
            RunSteps += new TestStep(EditTextBox2);
            RunSteps += new TestStep(ValidateConvertBack);
            RunSteps += new TestStep(EditTextBox1);
            RunSteps += new TestStep(ValidateWriteBack);
            RunSteps += new TestStep(FixBadBind);
            RunSteps += new TestStep(ChangeFontSize);
            RunSteps += new TestStep(ValidateDoNothing);
            RunSteps += new TestStep(EditTextBox3);
            RunSteps += new TestStep(ValidateUnsetValue);
        }

        private TestResult Init()
        {
            Status("Init");
            WaitForPriority(DispatcherPriority.Render);
            _textbox1 = LogicalTreeHelper.FindLogicalNode(RootElement, "_textbox1") as TextBox;
            _textbox2 = LogicalTreeHelper.FindLogicalNode(RootElement, "_textbox2") as TextBox;
            _textbox3 = LogicalTreeHelper.FindLogicalNode(RootElement, "_textbox3") as TextBox;
            _text = LogicalTreeHelper.FindLogicalNode(RootElement, "_text") as TextBlock;
            if (_text == null || _textbox1 == null || _textbox2 == null || _textbox3 == null)
            {
                LogComment("One or more Elements was null!");
                return TestResult.Fail;
            }


            return TestResult.Pass;
        }

        private TestResult InitValidation()
        {
            Status("InitialValidation");
            if (_textbox2.Text != "al:tinda FooBob")
            {
                LogComment("TextBox.Text has unexpected value: " + _textbox2.Text);
                return TestResult.Fail;
            }
            if (_textbox2.FontSize != 100)
            {
                LogComment("TextBox.FontSize has unexpected value: " + _textbox2.FontSize.ToString());
                return TestResult.Fail;
            }
            if (_textbox3.Text != "al:tinda 100")
            {
                LogComment("TextBox.Text has unexpected value: " + _textbox3.Text);
                return TestResult.Fail;
            }
            if (_textbox3.FontSize != 4)
            {
                LogComment("TextBox.FontSize has unexpected value: " + _textbox3.FontSize.ToString());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult EditTextBox2()
        {
            Status("Modifiying text value");

//          assuring focus is on textbox2
            _textbox2.Focus();
            _textbox2.Text = "foo 15";
//          moving focus to force write back of to data source.
            _textbox1.Focus();
            return TestResult.Pass;
        }

        private TestResult ValidateConvertBack()
        {
            Status("Validating ConvertBack");

            if (_textbox2.Text != "foo 15")
            {
                LogComment("TextBox2.Text has unexpected value: " + _textbox2.Text);
                return TestResult.Fail;
            }

            if (_textbox3.Text != "foo 100")
            {
                LogComment("TextBox3.Text has unexpected value: " + _textbox3.Text);
                return TestResult.Fail;
            }


            return TestResult.Pass;
        }

        private TestResult EditTextBox1()
        {
            _textbox1.Focus();
            _textbox1.Text = "8";
            return TestResult.Pass;
        }

        private TestResult ValidateWriteBack()
        {

            if (_textbox2.Text != "foo 8")
            {
                LogComment("TextBox.Text has unexpected value: " + _textbox2.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult FixBadBind()
        {
            Status("Fixing Bad Binding");

            MultiBinding mbOld = BindingOperations.GetMultiBindingExpression(_textbox3, TextBox.FontSizeProperty).ParentMultiBinding;
            MultiBinding mb = new MultiBinding();
            mb.Converter = mbOld.Converter;
            mb.Mode = mbOld.Mode;
            mb.FallbackValue = mbOld.FallbackValue;

            Binding bOld = (Binding)mbOld.Bindings[0];
            Binding b = new Binding("Top");
            b.Mode = bOld.Mode;
            b.FallbackValue = bOld.FallbackValue;
            mb.Bindings.Add(b);

            mb.Bindings.Add(mbOld.Bindings[1]);

            _textbox3.SetBinding(TextBox.FontSizeProperty, mb);

            return TestResult.Pass;
        }
        private TestResult ChangeFontSize()
        {
            Status("Changing Fontsize on TextBox3");
            _textbox3.FontSize = 18.0;
            return TestResult.Pass;
        }

        private TestResult ValidateDoNothing()
        {
            if (_text.FontSize != 10)
            {
                LogComment("Text.FontSize has unexpected value: " + _text.FontSize.ToString());
                return TestResult.Fail;
            }

            if (_textbox2.FontSize != 18)
            {
                LogComment("TextBox2.FontSize has unexpected value: " + _textbox2.FontSize.ToString());
                return TestResult.Fail;
            }

			// This is now expected to be the fallback value, i.e. 4
			//The Multibinding update succeeds, in the sense that none of its child bindings have an UpdateSourceError, so we re-fetch the source values.  
			//This causes the fallback value to be used, just as it was during the initial transfer.
            if (_textbox3.FontSize != 4)
            {
                LogComment("TextBox3.FontSize has unexpected value: " + _textbox3.FontSize.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult EditTextBox3()
        {
            Status("Editing TextBox 3");
            _textbox3.Text = "newvalue 13";
            WaitForPriority(DispatcherPriority.Render);
            MultiBindingExpression mb = BindingOperations.GetMultiBindingExpression(_textbox3, TextBox.TextProperty);
            mb.UpdateSource();

            return TestResult.Pass;
        }

        private TestResult ValidateUnsetValue()
        {
            Status("Validating DependencyProperty.UnsetValue");

            ICollectionView cv = CollectionViewSource.GetDefaultView((IEnumerable)_textbox2.DataContext);
            SortItem si = new SortItem("newvalue", 18.0);
            ObservableCollection<SortItem> aldc = cv.SourceCollection as ObservableCollection<SortItem>;
            if ( ((SortItem)aldc[0]).Name == "newvalue" || ((SortItem)aldc[0]).Top != 18.0d)
            {
                LogComment("Unexpected value in the collection - Expected Name: !newvalue  Actual: " + ((SortItem)aldc[0]).Name + "  Expected Top: 18  Actual: " + ((SortItem)aldc[0]).Top.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

    }

    public class MyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(double))
            {
                return (double)values[0];
            }

            return (string)values[0] + " " + (string)values[1].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {

            object[] o = new object[2];
            string[] _splitValues = ((string)value).Split(' ');

//          o[0] = _splitValues[0];
            for (int i = 0; i < 2; i++)
            {
                string _targetType = targetTypes[i].ToString();

                switch (_targetType)
                {
                    case "System.String":
                        o[i] = _splitValues[i];
                        break;

                    case "System.Double":
                        if (_splitValues[i].ToString() == "13")
                            o[i] = DependencyProperty.UnsetValue;
                        else
                            o[i] = double.Parse(_splitValues[i].ToString());
                        break;
                }
            }

            return o;
        }

    }

    public class MyConverter1 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                //Making sure we can trap errors
                int _fontsize = (int)values[0];
                return (object)_fontsize;
            }
            catch (Exception e)
            {
                // Forcing Fallback value
                GlobalLog.LogStatus(e.Message);
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] o = new object[2];
            double f = (double)value;
            double d = f;

            o[0] = d;
            o[1] = Binding.DoNothing;
//          o[1] = value;
            return o;
        }

    }


}




