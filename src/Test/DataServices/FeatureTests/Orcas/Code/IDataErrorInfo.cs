// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Validate Binding and MultiBinding query IDataErrorInfo for errors.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(0, "Validation", "IDataErrorInfo", SecurityLevel=TestCaseSecurityLevel.FullTrust)]
    public class IDataErrorInfoTest : WindowTest
    {
        Type _bindingType;
        Type _dataSourceType;
        object _dataSource;
        TextBox _tb;

        public IDataErrorInfoTest()
        {
            RunSteps += new TestStep(PlacesBinding);
            RunSteps += new TestStep(PlacesMultiBinding);
            RunSteps += new TestStep(PlacesDataTableBinding);
            RunSteps += new TestStep(PlacesDataTableMultiBinding);
            RunSteps += new TestStep(BindingValidatesOnExceptions);
            RunSteps += new TestStep(MultiBindingValidatesOnExceptions);
        }

        private TestResult PlacesBinding()
        {
            _dataSourceType = typeof(Places);
            _bindingType = typeof(Binding);
            return ValidateDataError();
        }

        private TestResult PlacesMultiBinding()
        {
            _dataSourceType = typeof(Places);
            _bindingType = typeof(Binding);
            return ValidateDataError();
        }

        private TestResult PlacesDataTableBinding()
        {
            _dataSourceType = typeof(Places);
            _bindingType = typeof(Binding);
            return ValidateDataError();
        }

        private TestResult PlacesDataTableMultiBinding()
        {
            _dataSourceType = typeof(Places);
            _bindingType = typeof(Binding);
            return ValidateDataError();
        }

        private TestResult BindingValidatesOnExceptions()
        {
            _dataSourceType = typeof(Places);
            _bindingType = typeof(Binding);
            return ValidateDataException();
        }

        private TestResult MultiBindingValidatesOnExceptions()
        {
            _dataSourceType = typeof(Places);
            _bindingType = typeof(MultiBinding);
            return ValidateDataException();
        }

        private TestResult ValidateDataError()
        {
            _dataSource = Activator.CreateInstance(_dataSourceType);
            _tb = new TextBox();
            CreateDataError();
            _tb.SetBinding(TextBox.TextProperty, CreateBinding());

            ValidationError ve = Validation.GetErrors(_tb)[0];
            if ((string)ve.ErrorContent == "Name cannot be blank!")
            {

                return TestResult.Pass;
            }
            else
            {
                LogComment("Validate Error failed.");
                return TestResult.Fail;
            }
        }

        private TestResult ValidateDataException()
        {
            string expectedMessage = "Name must be alpha only.";

            _dataSource = Activator.CreateInstance(_dataSourceType);
            _tb = new TextBox();
            _tb.SetBinding(TextBox.TextProperty, CreateBinding());
            CreateDataException();

            ValidationError ve = Validation.GetErrors(_tb)[0];
            LogComment("Seeing exception type = " + ve.Exception.GetType());
            if ((string)ve.Exception.Message == expectedMessage)
            {
                return TestResult.Pass;
            }
            else
            {
                LogComment("Validate Exception failed.  Expected: " + expectedMessage + " but saw " + (string)ve.Exception.Message);
                return TestResult.Fail;
            }
        }

        private void CreateDataError()
        {
            if (_dataSourceType == typeof(Places))
            {
                Places p = _dataSource as Places;
                p[0].Name = "";
            }
            else if (_dataSourceType == typeof(PlacesDataTable))
            {
                PlacesDataTable pdt = _dataSource as PlacesDataTable;
                pdt.Rows[0]["Name"] = "";
                pdt.Rows[0].SetColumnError("Name", "Name cannot be blank!");
            }
        }

        private void CreateDataException()
        {
            if (_dataSourceType == typeof(Places))
            {
                _tb.Text = "!,!";
                if (_bindingType == typeof(Binding))
                {
                    BindingOperations.GetBindingExpression(_tb, TextBox.TextProperty).UpdateSource();
                }
                else if (_bindingType == typeof(MultiBinding))
                {
                    BindingOperations.GetMultiBindingExpression(_tb, TextBox.TextProperty).UpdateSource();
                }
            }
        }

        private BindingBase CreateBinding()
        {
            if (_bindingType == typeof(Binding))
            {
                Binding bind = new Binding("Name");
                bind.ValidatesOnDataErrors = true;
                bind.ValidatesOnExceptions = true;
                bind.Source = _dataSource;
                return bind;
            }
            else if (_bindingType == typeof(MultiBinding))
            {
                MultiBinding bind = new MultiBinding();
                bind.Converter = new PlaceConverter();
                bind.ValidatesOnDataErrors = true;
                bind.ValidatesOnExceptions = true;
                Binding b1 = new Binding("Name");
                Binding b2 = new Binding("State");
                b1.Source = _dataSource;
                b2.Source = _dataSource;
                bind.Bindings.Add(b1);
                bind.Bindings.Add(b2);
                return bind;
            }
            return null;
        }
    }
}

