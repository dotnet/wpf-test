// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where code re-entrancy causes invariant assert.
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ValueConversion")]
    public class ValueConversion : XamlTest
    {

        #region Constructors

        public ValueConversion()
            : base(@"ValueConversion.xaml")
        {
            InitializeSteps += new TestStep(Validate);            
        }

        #endregion

        #region Private Members
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);                    

            // An invariant assert will cause the test to fail here if thee is a regression.
            return TestResult.Pass;
        }

        #endregion
        
    }   

    public class MyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(String));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new Exception();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // here we do something to cause data-binding activity that will lead to
            // a re-entrant call to DefaultValueConverter.ConvertHelper
            MyTestData myData = MyTestData.StaticInstance;
            if (myData != null)
            {
                myData.MyString = new MyString("changed");
            }


            if (value == null)
            {
                return "null";
            }
            else if (destinationType == typeof(String))
            {
                return value.ToString();
            }
            else
            {
                throw new Exception();
            }
        }
    }

    [TypeConverter(typeof(MyTypeConverter))]
    public class MyType
    {
    }


    public class MyStringConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(String));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new Exception();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return "null";

            MyString myString = value as MyString;
            return (myString != null) ? myString.TheString : "unknown";
        }
    }

    [TypeConverter(typeof(MyStringConverter))]
    public class MyString
    {
        public MyString(string s)
        {
            TheString = s;
        }

        public string TheString { get; set; }
    }

    public class MyTestData : INotifyPropertyChanged
    {
        public MyTestData()
        {
            StaticInstance = this;
        }

        public MyType MyType
        {
            get { return _myType; }
            set
            {
                _myType = value;
                NotifyPropertyChanged("MyType");
            }
        }

        public MyString MyString
        {
            get { return _myString; }
            set
            {
                _myString = value;
                NotifyPropertyChanged("MyString");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        internal static MyTestData StaticInstance;
        private MyType _myType = new MyType();
        private MyString _myString = new MyString("initial");
    }
}
