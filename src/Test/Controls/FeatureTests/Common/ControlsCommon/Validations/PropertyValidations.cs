using System;
using System.Windows;
using System.Windows.Input;

using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using System.Reflection;

namespace Avalon.Test.ComponentModel.Validations
{
    /// <summary>
    /// Validate the value of a control's property by Id.
    /// </summary>
    public class ControlChildPropertyValidationById : IValidation
    {
        /// <summary>
        /// Perform the validation.
        /// The XTC would look something like the following:
        /// <Validation Name="ControlChildPropertyValidationById">
        /// 	<Parameter Value="treeview1" />
        /// 	<Parameter Value="ScrollViewer" />
        /// 	<Parameter Value="1" />
        ///     <Parameter Value="BandIndex" />
        /// 	<Parameter Value="1" />
        /// </Validation>
        /// </summary>
        /// <param name="validateParams">First is the control, second is an array within which is the name of the property and the expected value.</param>
        /// <returns>True if validation passed, false otherwise.</returns>
        /// assumption is that the second element of validateParams is array with first element name and second value.
        public bool Validate(params object[] validateParams)
        {
            object control = validateParams[0];

            string elementId = (string)(validateParams[1] as Array).GetValue(0);
            string controlName = (string)(validateParams[1] as Array).GetValue(1);
            string indexStr = (string)(validateParams[1] as Array).GetValue(2);
            string propertyName = (string)(validateParams[1] as Array).GetValue(3);
            object expectedValue = (validateParams[1] as Array).GetValue(4);

            TestLog.Current.LogStatus("ControlId: [" + elementId + "]");
            TestLog.Current.LogStatus("ChildControlType: [" + controlName + "]");
            TestLog.Current.LogStatus("ControlIndex: [" + indexStr + "]");
            TestLog.Current.LogStatus("PropertyName: [" + propertyName + "]");
            TestLog.Current.LogStatus("ExpectedValue: [" + expectedValue.ToString() + "]");

            int index = 0;
            if (indexStr != "")
            {
                index = Convert.ToInt32(indexStr, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (elementId != "")
            {
                control = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)control, elementId) as FrameworkElement;
            }

            if (controlName != "")
            {
                object testControl = null;
                try
                {
                    testControl = ObjectFactory.CreateObjectFromTypeName(controlName);
                }
                catch (Exception e)
                {
                    TestLog.Current.LogEvidence("Failed creating a " + controlName + " control. Exception thrown: " + e.Message);
                }
                if (testControl == null)
                {
                    TestLog.Current.LogEvidence("Creating a " + controlName + " control returned null.");
                    return false;
                }
                else
                {
                    control = VisualTreeUtils.FindPartByType(control as System.Windows.Media.Visual, testControl.GetType(), index) as FrameworkElement;
                }

                if (control == null)
                {
                    TestLog.Current.LogEvidence("Failed finding a part of type " + controlName + ".");
                    return false;
                }
            }

            PropertyInfo pInfo = control.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Type valueType = pInfo.PropertyType;



            if (valueType == expectedValue.GetType())
            {
                return PMEUtils.Current.VerifyProperty(control, propertyName, expectedValue);
            }
            else if (expectedValue.GetType() == typeof(string))
            {
                return PMEUtils.Current.VerifyProperty(control, propertyName, PMEUtils.Current.StringToType((string)expectedValue, valueType));
            }
            else
            {
                throw new Exception("Invalid set of arguments");
            }
        }
    }

    /// <summary>
    /// Validate the value of a control's property by Id.
    /// </summary>
    public class ControlChildPropertyValidationById2 : IValidation
    {
        /// <summary>
        /// Perform the validation.
        /// The XTC would look something like the following:
        /// <Validation Name="ControlChildPropertyValidationById">
        /// 	<Parameter Value="treeview1" />
        /// 	<Parameter Value="ScrollViewer" />
        /// 	<Parameter Value="1" />
        ///     <Parameter Value="HorizontalOffset" />
        /// 	<Parameter Value="1" />
        ///  	<Parameter Value=">" />
        /// </Validation>
        /// </summary>
        /// <param name="validateParams">First is the control, second is an array within which is the name of the property and the expected value.</param>
        /// <returns>True if validation passed, false otherwise.</returns>
        /// assumption is that the second element of validateParams is array with first element name and second value.
        public bool Validate(params object[] validateParams)
        {
            object control = validateParams[0];

            string elementId = (string)(validateParams[1] as Array).GetValue(0);
            string controlName = (string)(validateParams[1] as Array).GetValue(1);
            string indexStr = (string)(validateParams[1] as Array).GetValue(2);
            string propertyName = (string)(validateParams[1] as Array).GetValue(3);
            string expectedValue = (string)(validateParams[1] as Array).GetValue(4);
            string compareType = (string)(validateParams[1] as Array).GetValue(5);

            TestLog.Current.LogStatus("ControlId: [" + elementId + "]");
            TestLog.Current.LogStatus("ChildControlType: [" + controlName + "]");
            TestLog.Current.LogStatus("ControlIndex: [" + indexStr + "]");
            TestLog.Current.LogStatus("PropertyName: [" + propertyName + "]");
            TestLog.Current.LogStatus("ExpectedValue: [" + expectedValue + "]");
            TestLog.Current.LogStatus("CompareType: [" + compareType + "]");
            
            int index = 0;
            if (indexStr != "")
            {
                index = Convert.ToInt32(indexStr, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (elementId != "")
            {
                control = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)control, elementId) as FrameworkElement;
            }

            if (controlName != "")
            {
                object testControl = null;
                try
                {
                    testControl = ObjectFactory.CreateObjectFromTypeName(controlName);
                }
                catch (Exception e)
                {
                    TestLog.Current.LogEvidence("Failed creating a " + controlName + " control. Exception thrown: " + e.Message);
                }
                if (testControl == null)
                {
                    TestLog.Current.LogEvidence("Creating a " + controlName + " control returned null.");
                    return false;
                }
                else
                {
                    control = VisualTreeUtils.FindPartByType(control as System.Windows.Media.Visual, testControl.GetType(), index) as FrameworkElement;
                }

                if (control == null)
                {
                    TestLog.Current.LogEvidence("Failed finding a part of type " + controlName + ".");
                    return false;
                }
            }

            PropertyInfo pInfo = control.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Type valueType = pInfo.PropertyType;




            return CompareNumbers(control, propertyName, expectedValue, compareType);
            
        }
        public bool CompareNumbers(object obj, string propertyName, object expectedValue, string compareType)
        {
            if (String.IsNullOrEmpty(propertyName) || obj == null)
                return false;

            PropertyInfo pInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (null == pInfo)
            {
                // Can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propertyName + " in the control");
            }

            object curValue = pInfo.GetValue(obj, new object[0]);

            TestLog.Current.LogStatus("ActualValue: [" + curValue.ToString() + "]");

            if (curValue == null)
                return expectedValue == null;

            double num1 = System.Convert.ToDouble(curValue);
            double num2 = System.Convert.ToDouble(expectedValue);
            TestLog.Current.LogStatus("curValue" + compareType + "expectedValue");
            if ((num1 > num2) && compareType==">")
                return true;
            if ((num1 == num2) && compareType == "=")
                return true;
            if ((num1 < num2) && compareType == "<")
                return true;
            if ((num1 != num2) && compareType == "!=")
                return true;
            return false;
        }
    }
}

