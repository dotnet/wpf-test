// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
	/// <description>
    /// Tests Custom Transformer
	/// </description>
	/// </summary>
    [Test(0, "Binding", "CustomTransformerBvt")]
    public class CustomTransformerBvt : WindowTest
    {
        HappyMan _happy;
        Dwarf _dwarf;
        BoolToColor _converter;

        public CustomTransformerBvt()
        {
            InitializeSteps += new TestStep(CreateTree);

            RunSteps += new TestStep(BoolToColor);
            RunSteps += new TestStep(ColorToBool);
        }

        TestResult CreateTree()
        {
            Status("Creating the Element Tree");

            _happy = new HappyMan();

            _happy.Name = "George";
            _happy.Position = new Point(100,100);
            _happy.Width = 200;
            _happy.Height = 200;

            Window.Content = _happy;

            _dwarf = new Dwarf("Sleepy", "Yellow", 30, 500, Colors.Purple, new Point(2, 5), true);
            _happy.DataContext = _dwarf;

            return TestResult.Pass;
        }

        /// <summary>
        /// Validate conversion from Bool to Color type
        /// </summary>
        private TestResult BoolToColor()
        {
            Status("Calling default converter.");
            _converter = new BoolToColor();
            Binding bind = new Binding("Angry");
            bind.Mode = BindingMode.TwoWay;
            bind.Converter = _converter;
            bind.ConverterCulture = CultureInfo.InvariantCulture;
            _happy.SetBinding(HappyMan.SkinColorProperty, bind);

            if(_happy.SkinColor != Colors.Green)
            {
                LogComment("Conversion from source to target failed.");
                return TestResult.Fail;
            }

            return _converter.ValidateParameters(CultureInfo.InvariantCulture, 1, 0);
        }

        /// <summary>
        /// Validate conversion from Color type to Bool
        /// </summary>
        private TestResult ColorToBool()
        {
            Status("Calling inverse custom converter.");
            _happy.SkinColor = Colors.Blue;

            if(_dwarf.Angry != false)
            {
                LogComment("Conversion from Color to Bool failed.");
                return TestResult.Fail;
            }
            Status("Functional Verification: We expect to see two conversions");
            return _converter.ValidateParameters(CultureInfo.InvariantCulture, 2, 1);

        }
    }


    public class BoolToColor : IValueConverter
    {
        object _actualValue;
        CultureInfo _actualCulture;
        int _actualConvertCount = 0;
        int _actualConvertBackCount = 0;

        /// <summary>
        /// Convert from bool to Color
        /// </summary>
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            _actualValue = o;
            _actualCulture = culture;
            _actualConvertCount++;

            if((bool)o)
                return Colors.Green;
            return null;
        }

        /// <summary>
        /// Convert from Color to bool
        /// </summary>
        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            _actualValue = o;
            _actualCulture = culture;
            _actualConvertBackCount++;

            if ((Color)o == Colors.Green)
                return true;
            else
                return false;
        }

        public TestResult ValidateParameters(CultureInfo culture, int convertCount, int convertBackCount)
        {

            if (_actualCulture != culture)
            {
                GlobalLog.LogEvidence("Culture was unexpected");
                GlobalLog.LogEvidence(string.Format("Expected: {0}", culture));
                GlobalLog.LogEvidence(string.Format("Actual: {0}", _actualCulture));
                return TestResult.Fail;
            }

            if (_actualConvertCount != convertCount) {
                GlobalLog.LogEvidence("Convert happened an unexpected number of times");
                GlobalLog.LogEvidence(string.Format("Expected: {0}", convertCount));
                GlobalLog.LogEvidence(string.Format("Actual: {0}", _actualConvertCount));
                return TestResult.Fail;
            }

            if (_actualConvertBackCount != convertBackCount)
            {
                GlobalLog.LogEvidence("ConvertBack happened an unexpected number of times");
                GlobalLog.LogEvidence(string.Format("Expected: {0}", convertBackCount));
                GlobalLog.LogEvidence(string.Format("Actual: {0}", _actualConvertBackCount));
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

    }
}
