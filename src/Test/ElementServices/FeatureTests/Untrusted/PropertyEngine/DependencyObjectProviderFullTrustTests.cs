// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /******************************************************************************
    * CLASS:          DependencyObjectProviderFullTrustTests
    ******************************************************************************/
    /// <summary>
    /// Hard-coded tests for DependencyObjectProvider which makes
    /// Avalon work well with TypeDecriptor/PropertyDecriptor.
    /// </summary>

    // [DISABLE WHILE PORTING]
    // [Test(1, "PropertyEngine.TypeDescriptor", TestCaseSecurityLevel.FullTrust, "DependencyObjectProviderFullTrustTests")]
    public class DependencyObjectProviderFullTrustTests : AvalonTest
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("TestDependencyPropertyDescriptor")]
        [Variation("ShouldSerializeAndReset")]

        /******************************************************************************
        * Function:          DependencyObjectProviderFullTrustTests Constructor
        ******************************************************************************/
        public DependencyObjectProviderFullTrustTests(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "TestDependencyPropertyDescriptor":
                    TestDependencyPropertyDescriptor();
                    break;
                case "ShouldSerializeAndReset":
                    ShouldSerializeAndReset();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public and Private Members
        /******************************************************************************
        * Function:          TestDependencyPropertyDescriptor
        ******************************************************************************/
        /// <summary>
        /// Validates that the public DependencyPropertyDescriptor works as advertised.
        /// </summary>
        public void TestDependencyPropertyDescriptor()
        {
            DependencyObject spo = new SimplePropertyObject();
            DependencyObject apo = new AttachedPropertyObject();

            // Verify all properties on SimplePropertyObject.
            GlobalLog.LogStatus("\r\n\r\nVerifying SimplePropertyObject object...");

            foreach (PropertyDescriptor testProp in TypeDescriptor.GetProperties(spo))
            {
                GlobalLog.LogStatus("\r\nVerifying " + testProp.Name + " property...");
                _VerifyPublicDescriptor(testProp, spo);
            }

            // Verify all properties on AttachedPropertyObject.
            GlobalLog.LogStatus("\r\n\r\nVerifying AttachedPropertyObject object...");

            foreach (PropertyDescriptor testProp in TypeDescriptor.GetProperties(apo))
            {
                GlobalLog.LogStatus("\r\nVerifying " + testProp.Name + " property...");
                _VerifyPublicDescriptor(testProp, apo);
            }
        }

        // Generic verification of DependencyPropertyDescriptor.
        private void _VerifyPublicDescriptor(PropertyDescriptor pd, DependencyObject o)
        {
            // 
            // FromProperty
            //
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(pd);

            if (dpd == null)
            {
                GlobalLog.LogStatus("No DependencyPropertyDescriptor.");
                return;
            }

            DependencyProperty dp = dpd.DependencyProperty;
            string propertyName = dp.Name;

            // 
            // FromName
            // Equals
            // GetHashCode
            //
            GlobalLog.LogStatus("Verifying FromName...");

            DependencyPropertyDescriptor dpd2 = DependencyPropertyDescriptor.FromName(dp.Name, dp.OwnerType, dpd.ComponentType);

            _Assert(dpd2 != null, "FromName()");
            _Assert(dpd.Equals(dpd2), "Equals()");
            _Assert(dpd.GetHashCode() == dpd2.GetHashCode(), "GetHashCode()");

            //
            // IsReadOnly
            // ComponentType
            // IsLocalizable
            // GetValue
            // ShouldSerializeValue
            //
            _Assert(pd.IsReadOnly == dpd.IsReadOnly, "IsReadOnly");
            _Assert(pd.ComponentType == dpd.ComponentType, "ComponentType");
            _Assert(pd.IsLocalizable == dpd.IsLocalizable, "IsLocalizable");
            _Assert(pd.ShouldSerializeValue(o) == dpd.ShouldSerializeValue(o), "ShouldSerializeValue");

            object value = pd.GetValue(o);
            _Assert(value == dpd.GetValue(o), "GetValue()");

            _ComparePropertyCollections(pd.GetChildProperties(), dpd.GetChildProperties(), value);

            //
            // Change events.
            // AddValueChanged
            // RemoveValueChanged
            // SetValue
            // CanResetValue
            // ResetValue
            //
            if (dpd.IsReadOnly)
            {
                GlobalLog.LogStatus("Not verifying change events since property is ready-only.");
            }
            else
            {
                GlobalLog.LogStatus("Verifying change events...");

                bool detectedChange = false;

                // Listen to change events and verify they work.
                EventHandler changeHandler = delegate
                {
                    detectedChange = true;
                };

                dpd.AddValueChanged(o, changeHandler);

                dpd.SetValue(o, "NewValue");
                _Assert(detectedChange, "AddValueChanged");

                detectedChange = false;

                if (dpd.CanResetValue(o))
                {
                    dpd.ResetValue(o);
                }

                _Assert(detectedChange, "AddValueChanged");

                // Unhook change events and verify they unhook
                dpd.RemoveValueChanged(o, changeHandler);
                detectedChange = false;

                dpd.SetValue(o, "NewValue2");
                _Assert(!detectedChange, "RemoveValueChanged");
            }

            //
            // IsReadOnly
            // Category
            // ComponentType
            // Description
            // DisplayName
            // DesignTimeOnly
            // IsBrowsable
            // IsLocalizable
            // Metadata
            // GetValue
            //

            GlobalLog.LogStatus("Verifying misc properties...");

            // Use a ResourceManager to locate the resource for PropertyCategoryDefault in System resources.
            Assembly assembly = typeof(System.Timers.Timer).Assembly;
            ResourceManager rm = new ResourceManager("System", assembly);
            string expectedCategoryValue = rm.GetString("PropertyCategoryDefault");
            _CompareStrings(dpd.Category, expectedCategoryValue, "Category");

            _CompareStrings(dpd.Description, propertyName + "Property", "Description");

            string displayName = dpd.IsAttached ? dp.OwnerType.Name + "." + propertyName : propertyName;
            _CompareStrings(dpd.DisplayName, displayName, "DisplayName");

            _Assert(!dpd.DesignTimeOnly, "DesignTimeOnly");

            _Assert(dpd.GetEditor(typeof(object)) == null, "GetEditor()");

            _Assert(dpd.IsBrowsable, "IsBrowsable");

            _Assert(dpd.Metadata.DefaultValue == dp.DefaultMetadata.DefaultValue, "Metadata");

            _Assert(dpd.SupportsChangeEvents, "SupportsChangeEvents");

            _CompareStrings(dpd.ToString(), displayName, "ToString()");
        }

        // Generic comparison of PropertyDescriptor instances.
        private void _ComparePropertyDescriptors(PropertyDescriptor property1, PropertyDescriptor property2, object o)
        {
            // Verify common PropertyDescriptor values.
            _Assert(property1.IsReadOnly == property2.IsReadOnly, "IsReadOnly");
            _Assert(property1.ComponentType == property2.ComponentType, "ComponentType");
            _Assert(property1.IsLocalizable == property2.IsLocalizable, "IsLocalizable");

            object value1 = property1.GetValue(o);
            object value2 = property2.GetValue(o);

            _Assert(_IsEqual(value1, value2), "GetValue()");

            if (o is DependencyObject)
            {
                // Verify PropertyDescriptor values of property1 to 
                // DependencyPropertyDescriptor values or property2.
                _VerifyPublicDescriptor(property1, (DependencyObject)o);
            }
        }

        // Generic comparison of PropertyDescriptorCollection instances.
        private void _ComparePropertyCollections(PropertyDescriptorCollection properties1, PropertyDescriptorCollection properties2, object o)
        {
            for (int i = 0; i < properties1.Count; i++)
            {
                _ComparePropertyDescriptors(properties1[i], properties2[i], o);
            }
        }

        // Generic comparison of String instances.
        private void _CompareStrings(string string1, string string2, string msg)
        {
            if(msg != null)
                msg += " is not the expected value.";
            else
                msg = "Strings do not match.";

            msg += " Actual: '" + string1 + "', Expected: '" + string2 + "'.";

            _Assert(String.Equals(string1, string2, StringComparison.InvariantCulture), msg);
        }

        private void _Assert(bool test, string msg)
        {
            if (!test)
            {
                if(msg == null)
                    throw new Microsoft.Test.TestValidationException("FAILED");
                else
                    throw new Microsoft.Test.TestValidationException(msg);
            }
        }

        // Checks if two values are the same.
        private bool _IsEqual(object expectedValue, object actualValue)
        {
            bool same = false;

            if (actualValue == null)
            {
                same = expectedValue == null;
            }
            else
            {
                same = actualValue.Equals(expectedValue);
            }

            return same;
        }

        /******************************************************************************
        * Function:          ShouldSerializeAndReset
        ******************************************************************************/
        /// <summary>
        /// Validates that defining ShouldSerialize and Reset for dependency 
        /// and attached properties works correctly.
        /// </summary>
        public void ShouldSerializeAndReset() 
        {
            SimplePropertyObject o = new SimplePropertyObject();
            PropertyDescriptor prop = TypeDescriptor.GetProperties(o)["SpecialMethodTest"];

            GlobalLog.LogStatus("Verify Dependency Property ShouldSerialize");
            if (prop.ShouldSerializeValue(o)) { throw new Microsoft.Test.TestValidationException("FAILED"); }
            prop.SetValue(o, "NoSerialize");
            if (prop.ShouldSerializeValue(o)) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            GlobalLog.LogStatus("Verify Dependency Property Reset");
            prop.ResetValue(o);
            if (o.SpecialMethodTest != "Reset") { throw new Microsoft.Test.TestValidationException("FAILED"); }

            AttachedPropertyObject a = new AttachedPropertyObject();
            prop = DependencyPropertyDescriptor.FromProperty(AttachedPropertyObject.AttachedSpecialMethodProperty, typeof(SimplePropertyObject));

            GlobalLog.LogStatus("Verify Attached Property ShouldSerialize");
            if (prop.ShouldSerializeValue(o)) { throw new Microsoft.Test.TestValidationException("FAILED");  }
            
            prop.SetValue(o, "NoSerialize");
            if (prop.ShouldSerializeValue(o)) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            GlobalLog.LogStatus("Verify Attached Property Reset");
            prop.ResetValue(o);
            if (AttachedPropertyObject.GetAttachedSpecialMethod(o) != "Reset") { throw new Microsoft.Test.TestValidationException("FAILED");  }
        }
        #endregion
    }
}
