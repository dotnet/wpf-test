using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test : Verify that ValidationSteps are correctly applied to
    /// the DataGrid during validation.
    /// 
    /// Variations:
    /// 
    /// 1. Create a ValidationRule with ValidationStep.RawProposedValue, do a valid edit and commit the row.
    /// 
    ///     Verify that the item returned from the BindingGroup in the Validate method has not been updated yet.    
    ///
    /// 2. Create a ValidationRule with ValidationStep.ConvertedProposedValue, do a valid edit and commit the row.
    /// 
    ///     Verify that the item returned from the BindingGroup in the Validate method has not been updated yet.
    ///     
    /// 3. Create a ValidationRule with ValidationStep.UpdatedValue, do a valid edit and commit the row.
    /// 
    ///     Verify that the item returned from the BindingGroup in the Validate method has been updated yet.
    ///     
    /// 4. Create a ValidationRule with ValidationStep.CommittedValue, do a valid edit and commit the row.
    /// 
    ///     Verify that the item returned from the BindingGroup in the Validate method has been updated yet.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest21", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest21 : DataGridTest
    {
        private Button _debugButton;
        private Button _button1;
        private Button _button2;
        private Button _button3;
        private Button _button4;

        #region Constructor

        public DataGridRegressionTest21()
            : base(@"DataGridRegressionTest21.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestValidationSteps);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridRegressionTest22");

            _debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", _debugButton != null);

            _button1 = (Button)RootElement.FindName("btn_RawProposedValue");
            Assert.AssertTrue("Unable to find btn_RawProposedValue from the resources", _button1 != null);

            _button2 = (Button)RootElement.FindName("btn_ConvertedValue");
            Assert.AssertTrue("Unable to find btn_ConvertedValue from the resources", _button2 != null);

            _button3 = (Button)RootElement.FindName("btn_UpdatedValue");
            Assert.AssertTrue("Unable to find btn_UpdatedValue from the resources", _button3 != null);

            _button4 = (Button)RootElement.FindName("btn_CommittedValue");
            Assert.AssertTrue("Unable to find btn_CommittedValue from the resources", _button4 != null);

            _button1.Click += (s, e) => { SetValidationStep(ValidationStep.RawProposedValue); };
            _button2.Click += (s, e) => { SetValidationStep(ValidationStep.ConvertedProposedValue); };
            _button3.Click += (s, e) => { SetValidationStep(ValidationStep.UpdatedValue); };
            _button4.Click += (s, e) => { SetValidationStep(ValidationStep.CommittedValue); };

            this.SetupDataSource();

            //Debug();

            LogComment("Setup for DataGridRegressionTest22 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            _debugButton = null;
            _button1 = null;
            _button2 = null;
            _button3 = null;
            _button4 = null;
            return base.CleanUp();
        }
        private TestResult TestValidationSteps()
        {
            Status("TestValidationSteps");

            LogComment("1. Create a ValidationRule with ValidationStep.RawProposedValue, do a valid edit and commit the row.");
            SetValidationStep(ValidationStep.RawProposedValue);

            DataGridCommandHelper.BeginEdit(MyDataGrid, 0, 0);
            string expectedData;
            DataGridActionHelper.EditCellGenericInput(MyDataGrid, 0, 0, out expectedData);
            DataGridCommandHelper.CommitEdit(MyDataGrid, 0, 0);

            // getting to this point without an exception signals correct verification

            LogComment("2. Create a ValidationRule with ValidationStep.ConvertedProposedValue, do a valid edit and commit the row.");
            SetValidationStep(ValidationStep.ConvertedProposedValue);

            DataGridCommandHelper.BeginEdit(MyDataGrid, 0, 0);
            DataGridActionHelper.EditCellGenericInput(MyDataGrid, 0, 0, out expectedData);
            DataGridCommandHelper.CommitEdit(MyDataGrid, 0, 0);

            // getting to this point without an exception signals correct verification

            LogComment("3. Create a ValidationRule with ValidationStep.UpdatedValue, do a valid edit and commit the row.");
            SetValidationStep(ValidationStep.UpdatedValue);

            DataGridCommandHelper.BeginEdit(MyDataGrid, 0, 0);
            DataGridActionHelper.EditCellGenericInput(MyDataGrid, 0, 0, out expectedData);
            DataGridCommandHelper.CommitEdit(MyDataGrid, 0, 0);

            // getting to this point without an exception signals correct verification

            LogComment("4. Create a ValidationRule with ValidationStep.CommittedValue, do a valid edit and commit the row.");
            SetValidationStep(ValidationStep.CommittedValue);

            DataGridCommandHelper.BeginEdit(MyDataGrid, 0, 0);
            DataGridActionHelper.EditCellGenericInput(MyDataGrid, 0, 0, out expectedData);
            DataGridCommandHelper.CommitEdit(MyDataGrid, 0, 0);

            // getting to this point without an exception signals correct verification

            LogComment("TestValidationSteps was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps    
  
        private void SetValidationStep(ValidationStep step)
        {
            ValidationRule validationRule = null;

            if (step == ValidationStep.RawProposedValue)
            {
                validationRule = new ValidationRuleRawProposedScenario1();
            }
            else if (step == ValidationStep.ConvertedProposedValue)
            {
                validationRule = new ValidationRuleConvertedProposedScenario1();
            }
            else if (step == ValidationStep.UpdatedValue)
            {
                validationRule = new ValidationRuleUpdatedValueScenario1();
            }
            else if (step == ValidationStep.CommittedValue)
            {
                validationRule = new ValidationRuleUpdatedValueScenario1();
            }
            validationRule.ValidationStep = step;

            MyDataGrid.RowValidationRules.Clear();
            MyDataGrid.RowValidationRules.Add(validationRule);
        }

        private void Debug()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            _debugButton.MouseLeftButtonDown += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }

        #region Validation Classes

        private class ValidationRuleRawProposedScenario1 : ValidationRule
        {
            public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
            {
                ValidationResult result = ValidationResult.ValidResult;

                BindingGroup bindingGroup = (BindingGroup)value;

                IList items = bindingGroup.Items;
                Person person = (Person)items[0];

                object firstNameObj;
                if (bindingGroup.TryGetValue(person, "FirstName", out firstNameObj))
                {
                    if (person.FirstName == firstNameObj as string)
                    {
                        throw new TestValidationException("For RawProposedValue scenario, data should not have been commited to the source yet.");
                    }
                }

                if (firstNameObj.ToString().ToUpper() == "XX")
                {
                    return new ValidationResult(false, string.Format("FirstName must exist"));
                }

                return result;
            }
        }

        private class ValidationRuleConvertedProposedScenario1 : ValidationRule
        {
            public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
            {
                ValidationResult result = ValidationResult.ValidResult;

                BindingGroup bindingGroup = (BindingGroup)value;

                IList items = bindingGroup.Items;
                Person person = (Person)items[0];

                object firstNameObj;
                if (bindingGroup.TryGetValue(person, "FirstName", out firstNameObj))
                {
                    if (person.FirstName == firstNameObj as string)
                    {
                        throw new TestValidationException("For RawProposedValue scenario, data should not have been commited to the source yet.");
                    }
                }

                if (firstNameObj.ToString().ToUpper() == "XX")
                {
                    return new ValidationResult(false, string.Format("FirstName must exist"));
                }

                return result;
            }
        }

        private class ValidationRuleUpdatedValueScenario1 : ValidationRule
        {
            public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
            {
                ValidationResult result = ValidationResult.ValidResult;

                BindingGroup bindingGroup = (BindingGroup)value;

                IList items = bindingGroup.Items;
                Person person = (Person)items[0];

                object firstNameObj;
                if (bindingGroup.TryGetValue(person, "FirstName", out firstNameObj))
                {
                    if (person.FirstName != firstNameObj as string)
                    {
                        throw new TestValidationException("For RawProposedValue scenario, data should not have been commited to the source yet.");
                    }
                }

                if (firstNameObj.ToString().ToUpper() == "XX")
                {
                    return new ValidationResult(false, string.Format("FirstName must exist"));
                }

                return result;
            }
        }    

        #endregion Validation Classes
    }
}
