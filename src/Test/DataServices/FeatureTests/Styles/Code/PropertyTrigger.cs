// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Test.DataServices;
using System.Windows.Media;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	///	PropertyTriggers
	/// </description>
    /// </summary>
    [Test(1, "Styles", "PropertyTrigger")]
    public class PropertyTrigger : XamlTest
    {
        private string _original;
        private string _changed;
        private FrameworkElement _element;
        private DependencyProperty _dp;
        private string _dpName;

        [Variation("PropertyTrigger_Both.xaml", "Text", "Some String", "String Two")]
        [Variation("PropertyTrigger_OrigValue.xaml", "Text", "Some String", "Something")]
        [Variation("PropertyTrigger_SetValue.xaml", "Text", "Something", "Some String")]
        [Variation("PropertyTrigger_Compound.xaml", "Text", "Some String", "String Two")]
        public PropertyTrigger(string fileName, string dpName, string original, string changed)
            : base(fileName)
        {
            _original = original;
            _changed = changed;
            _dpName = dpName;

            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(InitialVerify);
            RunSteps += new TestStep(ChangeValue);
            //RunSteps += new TestStep(ChangeVerify);
            RunSteps += new TestStep(ResetValue);
            //RunSteps += new TestStep(ResetVerify);
        }

        private TestResult SetUp()
        {
            _element = Util.FindElement(RootElement, "TestMe");
            if (_element == null)
            {
                LogComment("Could not locate TestMe element");
                return TestResult.Fail;
            }

            _dp = DependencyPropertyFromName(_dpName, _element.GetType());
            if (_dp == null)
            {
                LogComment("_dp is null");
                return TestResult.Fail;
            }
            
            
            return TestResult.Pass;
        }

        private DependencyProperty DependencyPropertyFromName(string propertyName, Type propertyType)
        {
            FieldInfo fi = propertyType.GetField(propertyName + "Property", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return (fi != null)?fi.GetValue(null) as DependencyProperty:null;
        }


        private TestResult InitialVerify()
        {
            return Verify("Initial Verify", _original);
        }

        private TestResult ChangeValue()
        {
            _element.Tag = "TempValue";
            return TestResult.Pass;
        }

        private TestResult ChangeVerify()
        {
            return Verify("Change Verify", _changed);
        }

        private TestResult ResetValue()
        {
            _element.Tag = null;
            return TestResult.Pass;
        }

        private TestResult ResetVerify()
        {
            return Verify("Reset Verify", _original);
        }


        private TestResult Verify(string StepName, string expectedString)
        {
            WaitForPriority(DispatcherPriority.Background);

            TypeConverter converter = TypeDescriptor.GetConverter(_dp.PropertyType);
            if (converter == null)
            {
                LogComment("Type converter was null");
                return TestResult.Fail;
            }

            object expected = converter.ConvertFromString(expectedString);
            object actual = _element.GetValue(_dp);

            if (Util.CompareObjects(expected, actual))
            {
                LogComment("Values were the same for " + StepName);
                return TestResult.Pass;
            }
            else
            {
                LogComment("Values were different for " + StepName + ", expected: " + expected.ToString() + " actual: " + actual.ToString());
                return TestResult.Fail;
            }
        }
    }

    public class TriggerItem : INotifyPropertyChanged
    {

        public TriggerItem()
        {
            _brush1 = new SolidColorBrush(Colors.Magenta);
            _brush2 = new SolidColorBrush(Colors.MediumPurple);
            _string1 = "Some String";
            _string2 = "String Two";
            _int = 10;
            _double = 30.5;
            _bool = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private SolidColorBrush _brush1;
        private SolidColorBrush _brush2;
        private string _string1;
        private string _string2;
        private int _int;
        private double _double;
        private bool _bool;

        public SolidColorBrush BrushValue1
        {
            get { return _brush1; }
            set
            {
                _brush1 = value;
                RaisePropertyChangedEvent("BrushValue1");
            }
        }

        public SolidColorBrush BrushValue2
        {
            get { return _brush2; }
            set
            {
                _brush2 = value;
                RaisePropertyChangedEvent("BrushValue2");
            }
        }

        public string StringValue1
        {
            get { return _string1; }
            set
            {
                _string1 = value;
                RaisePropertyChangedEvent("StringValue1");
            }
        }

        public string StringValue2
        {
            get { return _string2; }
            set
            {
                _string2 = value;
                RaisePropertyChangedEvent("StringValue2");
            }
        }

        public int IntValue
        {
            get { return _int; }
            set
            {
                _int = value;
                RaisePropertyChangedEvent("IntValue");
            }
        }
        public double DoubleValue
        {
            get { return _double; }
            set
            {
                _double = value;
                RaisePropertyChangedEvent("DoubleValue");
            }
        }
        public bool BoolValue
        {
            get { return _bool; }
            set
            {
                _bool = value;
                RaisePropertyChangedEvent("BoolValue");
            }
        }
    }
}

