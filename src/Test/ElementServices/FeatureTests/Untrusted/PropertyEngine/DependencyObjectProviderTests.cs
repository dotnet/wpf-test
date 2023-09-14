// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
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
    * CLASS:          DependencyObjectProviderTests
    ******************************************************************************/
    /// <summary>
    /// Hard-coded tests for DependencyObjectProvider which makes
    /// Avalon work well with TypeDecriptor/PropertyDecriptor.
    /// </summary>
    [Test(1, "PropertyEngine.TypeDescriptor", TestCaseSecurityLevel.PartialTrust, "DependencyObjectProviderTests")]
    public class DependencyObjectProviderTests : AvalonTest
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("AccessDP")]
        [Variation("PropertyChangeNotification")]
        [Variation("LocateAttachedProperty")]
        [Variation("PullCLRAttributes")]
        [Variation("PropertyFilterAttribute")]
        // [Variation("AttachedPropertyTargets", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there.  // [DISABLE WHILE PORTING]
        [Variation("DependencyObjectDescriptor")]
        // [Variation("PropertyDescriptorIsReadOnly", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there.  // [DISABLE WHILE PORTING]

        /******************************************************************************
        * Function:          DependencyObjectProviderTests Constructor
        ******************************************************************************/
        public DependencyObjectProviderTests(string arg)
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
            GlobalLog.LogStatus("Test Case: " + _testName);

            switch (_testName)
            {
                case "AccessDP":
                    AccessDP();
                    break;
                case "PropertyChangeNotification":
                    PropertyChangeNotification();
                    break;
                case "LocateAttachedProperty":
                    LocateAttachedProperty();
                    break;
                case "PullCLRAttributes":
                    PullCLRAttributes();
                    break;
                case "PropertyFilterAttribute":
                    PropertyFilterAttribute();
                    break;
                case "AttachedPropertyTargets":
                    AttachedPropertyTargets();
                    break;
                case "DependencyObjectDescriptor":
                    DependencyObjectDescriptor();
                    break;
                case "PropertyDescriptorIsReadOnly":
                    PropertyDescriptorIsReadOnly();
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
        * Function:          AccessDP
        ******************************************************************************/
        /// <summary>
        /// Ensure we can access a DP and do basic manipulations.
        /// </summary>
        public void AccessDP() 
        {
            SimplePropertyObject o = new SimplePropertyObject();
            
            // Ensure that we can find the Test property
            GlobalLog.LogStatus("Locate Test Property");
            PropertyDescriptor testProp = TypeDescriptor.GetProperties(o)["Test"];
            if (testProp == null) 
            { 
                throw new Microsoft.Test.TestValidationException("FAILED");
            }

            // Ensure that the value returned from testProp
            // equals the real value returned.
            GlobalLog.LogStatus("Compare PropertyDescriptor value with real property value.");
            if (!object.Equals(o.Test, testProp.GetValue(o))) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            // Ensure that the DefaultValueAttribute returns a value
            // that is equal to the current value, since
            // we haven't changed it.
            GlobalLog.LogStatus("Compare DefaultValueAttribute with current value.");
            DefaultValueAttribute def = testProp.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
            if (def == null || !object.Equals(def.Value, o.Test)) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            // Ensure that ShouldSerializeValue is false, since we haven't
            // changed the property value
            GlobalLog.LogStatus("Validate PropertyDescriptor.ShouldSerializeValue == false.");
            if (testProp.ShouldSerializeValue(o)) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            // Ensure that setting the property via a PropertyDescriptor
            // changes the real property value.
            GlobalLog.LogStatus("Validate PropertyDescriptor.SetValue.");
            string setValue = "NewValue";
            testProp.SetValue(o, setValue);
            if (!object.Equals(o.Test, setValue)) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            // Ensure that ShouldSerializeValue is now true
            GlobalLog.LogStatus("Validate PropertyDescriptor.ShouldSerializeValue == true.");
            if (!testProp.ShouldSerializeValue(o)) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            // Ensure that ResetValue works, and resets the value back to the default
            GlobalLog.LogStatus("Validate ResetValue.");
            testProp.ResetValue(o);
            if (!object.Equals(def.Value, o.Test)) { throw new Microsoft.Test.TestValidationException("FAILED");  }
            if (testProp.ShouldSerializeValue(o)) { throw new Microsoft.Test.TestValidationException("FAILED");  }
        }


        /******************************************************************************
        * Function:          PropertyChangeNotification
        ******************************************************************************/
        /// <summary>
        /// Use the property change notification API and ensure it works.
        /// </summary>
        public void PropertyChangeNotification() 
        {
            SimplePropertyObject o = new SimplePropertyObject();
            bool detectedChange = false;

            // Get the property
            GlobalLog.LogStatus("Locate Test Property");
            PropertyDescriptor testProp = TypeDescriptor.GetProperties(o)["Test"];
            if (testProp == null) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            // Listen to change events and verify they work.
            GlobalLog.LogStatus("Listen to Change Events");
            EventHandler changeHandler = delegate {
                detectedChange = true;
            };

            testProp.AddValueChanged(o, changeHandler);

            o.Test = "NewValue";
            if (!detectedChange) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            // Unhook change events and verify they unhook
            GlobalLog.LogStatus("Ignore Change Events");
            testProp.RemoveValueChanged(o, changeHandler);
            detectedChange = false;

            o.Test = "NewValue2";
            if (detectedChange) { throw new Microsoft.Test.TestValidationException("FAILED"); }
        }

        /******************************************************************************
        * Function:          LocateAttachedProperty
        ******************************************************************************/
        /// <summary>
        /// Verify that we can locate an attached property on an object.
        /// </summary>
        public void LocateAttachedProperty() 
        {
            // Attached properties are not "present" until they become
            // registered with the property engine.  
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(AttachedPropertyObject).TypeHandle);

            SimplePropertyObject o = new SimplePropertyObject();

            //
            // Get the attached property on the object.
            //
            GlobalLog.LogStatus("Locate Attached Property");
            PropertyDescriptor prop = TypeDescriptor.GetProperties(o)["AttachedPropertyObject.Attached"];
            if (prop == null) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            //
            // Now verify that the attached property is NOT found
            // on a stock DependencyObject, as the property's CanAttach
            // rules do not match.
            //
            GlobalLog.LogStatus("Verify CanAttach");
            DependencyObject dob = new DependencyObject();
            prop = TypeDescriptor.GetProperties(dob)["AttachedPropertyObject.Attached"];
            if (prop != null) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            
        }

        /******************************************************************************
        * Function:          PullCLRAttributes
        ******************************************************************************/
        /// <summary>
        /// Verifies that we correctly pull CLR attributes of of properties
        /// for both normal and attached properties.
        /// </summary>
        public void PullCLRAttributes() 
        {
            // Attached properties are not "present" until they become
            // registered with the property engine.  
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(AttachedPropertyObject).TypeHandle);

            SimplePropertyObject o = new SimplePropertyObject();

            //
            // Get the property on the object.
            //
            GlobalLog.LogStatus("Locate Property");
            PropertyDescriptor prop = TypeDescriptor.GetProperties(o)["Test"];
            if (prop == null) { throw new Microsoft.Test.TestValidationException("FAILED: PropertyDescriptor returned null");  }

            //
            // Verify it has the description attribute
            //
            GlobalLog.LogStatus("Verify attributes");
            DescriptionAttribute attr = prop.Attributes[typeof(DescriptionAttribute)] as DescriptionAttribute;
            if (attr == null) { throw new Microsoft.Test.TestValidationException("FAILED: DescriptionAttribute returned null");  }

            //
            // Get the attached property on the object.
            //
            GlobalLog.LogStatus("Locate Attached Property");
            prop = TypeDescriptor.GetProperties(o)["AttachedPropertyObject.Attached"];
            if (prop == null) { throw new Microsoft.Test.TestValidationException("FAILED: AttachedPropertyObject.Attached returned null"); }

            //
            // Verify it has the description attribute
            //
            GlobalLog.LogStatus("Verify attached attributes");
            attr = prop.Attributes[typeof(DescriptionAttribute)] as DescriptionAttribute;
            if (attr == null) { throw new Microsoft.Test.TestValidationException("FAILED: Attributes[typeof(DescriptionAttribute) returned null");  }

            //
            // Verify AttachedPropertyBrowsable attributes.
            //
//TO-DO: Determine why bft returns null sometimes in Lab test runs only.  (This code was only recently enabled, so it may
//not have been successul in V1.
/*
            // Verify it has the 'AttachedPropertyBrowsableForTypeAttribute' attribute
            AttachedPropertyBrowsableForTypeAttribute bft = prop.Attributes[typeof(AttachedPropertyBrowsableForTypeAttribute)] as AttachedPropertyBrowsableForTypeAttribute;
            if (bft == null) { throw new Microsoft.Test.TestValidationException("FAILED: AttachedPropertyBrowsableForTypeAttribute returned null");  }

            // Verify API of 'AttachedPropertyBrowsableForTypeAttribute'.
            if (bft.TargetType != typeof(SimplePropertyObject)) { throw new Microsoft.Test.TestValidationException("FAILED: AttachedPropertyBrowsableForTypeAttribute.TargetType returned null"); }

            if (bft.TypeId == null) { throw new Microsoft.Test.TestValidationException("FAILED:  AttachedPropertyBrowsableForTypeAttribute.TypeId returned null"); }

            // Compare to new equivalent instance.
            AttachedPropertyBrowsableForTypeAttribute bft2 =
                new AttachedPropertyBrowsableForTypeAttribute(typeof(SimplePropertyObject));
            if (!bft.Equals(bft2)) { throw new Microsoft.Test.TestValidationException("FAILED:  AttachedPropertyBrowsableForTypeAttribute failed equivalence");  }

            if (bft.GetHashCode() != bft2.GetHashCode()) { throw new Microsoft.Test.TestValidationException("FAILED:  AttachedPropertyBrowsableForTypeAttribute.GetHashCode failed equivalence"); }

            // Compare to new non-equivalent instance.
            AttachedPropertyBrowsableForTypeAttribute bft3 =
                new AttachedPropertyBrowsableForTypeAttribute(typeof(DependencyObject));
            if (bft.Equals(bft3)) { throw new Microsoft.Test.TestValidationException("FAILED:  AttachedPropertyBrowsableForTypeAttribute failed non-equivalence"); }

            if (bft.GetHashCode() == bft3.GetHashCode()) { throw new Microsoft.Test.TestValidationException("FAILED:  AttachedPropertyBrowsableForTypeAttribute.GetHashCode failed non-equivalence");  }
*/
            // Verify it has the 'AttachedPropertyBrowsableWhenAttributePresent' attribute
            AttachedPropertyBrowsableWhenAttributePresentAttribute bwap2 = prop.Attributes[typeof(AttachedPropertyBrowsableWhenAttributePresentAttribute)] as AttachedPropertyBrowsableWhenAttributePresentAttribute;
            if (bwap2 == null) { throw new Microsoft.Test.TestValidationException("FAILED: AttachedPropertyBrowsableWhenAttributePresentAttribute returned null"); }

            // Verify API of 'AttachedPropertyBrowsableWhenAttributePresent'.
            if (bwap2.AttributeType != typeof(DescriptionAttribute)) { throw new Microsoft.Test.TestValidationException("FAILED: AttachedPropertyBrowsableWhenAttributePresentAttribute.AttributedType not a DescriptionAttribute type"); }

            // Compare to new equivalent instance.
            AttachedPropertyBrowsableWhenAttributePresentAttribute bwap3 = 
                new AttachedPropertyBrowsableWhenAttributePresentAttribute(typeof(DescriptionAttribute));
            if (!bwap2.Equals(bwap3)) { throw new Microsoft.Test.TestValidationException("FAILED: AttachedPropertyBrowsableWhenAttributePresentAttribute failed equivlance");  }

            if (bwap2.GetHashCode() != bwap3.GetHashCode()) { throw new Microsoft.Test.TestValidationException("FAILED: AttachedPropertyBrowsableWhenAttributePresentAttribute.HashCode failed equilalence"); }

            // Compare to new non-equivalent instance.
            bwap3 = 
                new AttachedPropertyBrowsableWhenAttributePresentAttribute(typeof(SerializableAttribute));

            if (bwap2.Equals(bwap3)) { throw new Microsoft.Test.TestValidationException("FAILED: AttachedPropertyBrowsableWhenAttributePresentAttribute failed non-equivlance");  }

            if (bwap2.GetHashCode() == bwap3.GetHashCode()) { throw new Microsoft.Test.TestValidationException("FAILED:  AttachedPropertyBrowsableWhenAttributePresentAttribute.GetHashCode failed non-equivlance"); }
        }

        /******************************************************************************
        * Function:          PropertyFilterAttribute
        ******************************************************************************/
        /// <summary>
        /// Validates that filters can be applied to the scope of properties
        /// to restrict what is returned.
        /// </summary>
        public void PropertyFilterAttribute() 
        {
            // Attached properties are not "present" until they become
            // registered with the property engine.  
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(AttachedPropertyObject).TypeHandle);

            SimplePropertyObject o = new SimplePropertyObject();
            PropertyDescriptor normal, hidden, attached;
            Attribute[] attrs = new Attribute[1];

            // Verify misc PropertyFilterAttribute API.
            GlobalLog.LogStatus("Verify misc PropertyFilterAttribute");
            PropertyFilterAttribute attr1 = new PropertyFilterAttribute(PropertyFilterOptions.SetValues);
            PropertyFilterAttribute attr2 = new PropertyFilterAttribute(PropertyFilterOptions.SetValues);
            PropertyFilterAttribute attr3 = new PropertyFilterAttribute(PropertyFilterOptions.All);
            PropertyFilterAttribute attr4 = new PropertyFilterAttribute(PropertyFilterOptions.UnsetValues);
            if (!attr1.Equals(attr2)) { throw new Microsoft.Test.TestValidationException("FAILED"); }
            if (attr1.Equals(attr3)) { throw new Microsoft.Test.TestValidationException("FAILED"); }
            if (!attr1.Match(attr2)) { throw new Microsoft.Test.TestValidationException("FAILED"); }
            if (!attr1.Match(attr3)) { throw new Microsoft.Test.TestValidationException("FAILED"); }
            if (attr1.Match(attr4)) { throw new Microsoft.Test.TestValidationException("FAILED"); }
            if (attr1.GetHashCode() != attr2.GetHashCode()) { throw new Microsoft.Test.TestValidationException("FAILED"); }
            if (attr1.GetHashCode() == attr3.GetHashCode()) { throw new Microsoft.Test.TestValidationException("FAILED"); }
            if (attr1.Filter != PropertyFilterOptions.SetValues || attr3.Filter != PropertyFilterOptions.All) { throw new Microsoft.Test.TestValidationException("FAILED"); }


            // Verify that we can ask for only the "set" properties and that
            // is all we will get back.
            GlobalLog.LogStatus("Verify Set Property Filter");
            attrs[0] = new PropertyFilterAttribute(PropertyFilterOptions.SetValues);
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(o, attrs);

            // We haven't set any of these yet, so we should get back none.
            normal = props["Test"];
            hidden = props["AttachedPropertyObject.HiddenAttached"];
            attached = props["AttachedPropertyObject.Attached"];

            if (normal != null || hidden != null || attached != null) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            AttachedPropertyObject.SetAttached(o, "Attached");

            // Cannot directly set hidden attached because the API prevents it
            o.SetValue(AttachedPropertyObject.HiddenAttachedProperty, "HiddenAttached");
            o.Test = "Normal";

            props = TypeDescriptor.GetProperties(o, attrs);

            // We've set all of these so they should all come back now.
            normal = props["Test"];
            hidden = props["AttachedPropertyObject.HiddenAttached"];
            attached = props["AttachedPropertyObject.Attached"];

            if (normal == null || hidden == null || attached == null) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            // Verify we can ask for only the "valid" properties and that
            // is all we will get back
            GlobalLog.LogStatus("Verify Valid Property Filter");
            attrs[0] = new PropertyFilterAttribute(PropertyFilterOptions.Valid);
            props = TypeDescriptor.GetProperties(o, attrs);

            // Of these, only hidden should be null
            normal = props["Test"];
            hidden = props["AttachedPropertyObject.HiddenAttached"];
            attached = props["AttachedPropertyObject.Attached"];

            if (normal == null || hidden != null || attached == null) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            // Verify we can ask for all properties and that we get
            // back everything
            GlobalLog.LogStatus("Verify All Property Filter");
            attrs[0] = new PropertyFilterAttribute(PropertyFilterOptions.All);
            props = TypeDescriptor.GetProperties(o, attrs);

            // All of these should be available.
            normal = props["Test"];
            hidden = props["AttachedPropertyObject.HiddenAttached"];
            attached = props["AttachedPropertyObject.Attached"];

            if (normal == null || hidden == null || attached == null) { throw new Microsoft.Test.TestValidationException("FAILED");  }
        }

        /******************************************************************************
        * Function:          AttachedPropertyTargets
        ******************************************************************************/
        /// <summary>
        /// Runs through all the different attached property targets provided in 
        /// the framework and verifies their function.
        /// </summary>
        public void AttachedPropertyTargets() 
        {
            // First, setup a tree of objects
            CallbackObject root = new CallbackObject();
            Button button = new Button();
            CheckBox checkBox = new CheckBox();
            TextBox textBox = new TextBox();
            Canvas nested = new Canvas();
            Button nestedButton = new Button();
            TextBox nestedTextBox = new TextBox();
            Button enabledButton = new EnabledButton();
            Button disabledButton = new DisabledButton();

            root.Children.Add(button);
            root.Children.Add(textBox);
            root.Children.Add(checkBox);
            root.Children.Add(nested);
            root.Children.Add(enabledButton);
            root.Children.Add(disabledButton);

            nested.Children.Add(nestedButton);
            nested.Children.Add(nestedTextBox);

            // All the attached properties we will be looking at
            string[] propertyNames = new string[] {
                "CallbackObject.Children",
                "CallbackObject.Descendants",
                "CallbackObject.SingleSubtype",
                "CallbackObject.MultipleSubtype",
                "CallbackObject.Attribute"
            };

            bool[] rootValues = new bool[] {
                false, false, false, false, false
            };

            bool[] buttonValues = new bool[] {
                true, true, true, false, false
            };

            bool[] textBoxValues = new bool[] {
                true, true, false, true, false
            };

            bool[] nestedButtonValues = new bool[] {
                false, true, true, false, false
            };

            bool[] nestedTextBoxValues = new bool[] {
                false, true, false, true, false
            };

            bool[] checkBoxValues = new bool[] {
                true, true, false, true, false
            };

            bool[] enabledButtonValues = new bool[] {
                true, true, true, false, true
            };

            bool[] disabledButtonValues = new bool[] {
                true, true, true, false, false
            };

            // Verify that controls at the first level work as expected
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(button);
            for (int idx = 0; idx < propertyNames.Length; idx++)
            {
                bool contains = props[propertyNames[idx]] != null;
                ValidatePropertyTarget(button, propertyNames[idx], buttonValues[idx], contains);
            }

            props = TypeDescriptor.GetProperties(textBox);
            for (int idx = 0; idx < propertyNames.Length; idx++)
            {
                bool contains = props[propertyNames[idx]] != null;
                ValidatePropertyTarget(textBox, propertyNames[idx], textBoxValues[idx], contains);
            }

            props = TypeDescriptor.GetProperties(checkBox);
            for (int idx = 0; idx < propertyNames.Length; idx++)
            {
                bool contains = props[propertyNames[idx]] != null;
                ValidatePropertyTarget(checkBox, propertyNames[idx], checkBoxValues[idx], contains);
            }

            // Verify that controls that have a particular attribute
            // light up the right properties.
            props = TypeDescriptor.GetProperties(enabledButton);
            for (int idx = 0; idx < propertyNames.Length; idx++)
            {
                bool contains = props[propertyNames[idx]] != null;
                ValidatePropertyTarget(enabledButton, propertyNames[idx], enabledButtonValues[idx], contains);
            }

            props = TypeDescriptor.GetProperties(disabledButton);
            for (int idx = 0; idx < propertyNames.Length; idx++)
            {
                bool contains = props[propertyNames[idx]] != null;
                ValidatePropertyTarget(disabledButton, propertyNames[idx], disabledButtonValues[idx], contains);
            }

            // Verify that controls at the second level work as expected
            GlobalLog.LogStatus("Verify Descendants Controls");
            props = TypeDescriptor.GetProperties(nestedButton);
            for (int idx = 0; idx < propertyNames.Length; idx++)
            {
                bool contains = props[propertyNames[idx]] != null;
                ValidatePropertyTarget(nestedButton, propertyNames[idx], nestedButtonValues[idx], contains);
            }

            props = TypeDescriptor.GetProperties(nestedTextBox);
            for (int idx = 0; idx < propertyNames.Length; idx++)
            {
                bool contains = props[propertyNames[idx]] != null;
                ValidatePropertyTarget(nestedTextBox, propertyNames[idx], nestedTextBoxValues[idx], contains);
            }

            // Verify that the root works as expected
            GlobalLog.LogStatus("Verify Root control");
            props = TypeDescriptor.GetProperties(root);
            for (int idx = 0; idx < propertyNames.Length; idx++)
            {
                bool contains = props[propertyNames[idx]] != null;
                ValidatePropertyTarget(root, propertyNames[idx], rootValues[idx], contains);
            }
        }

        private static void ValidatePropertyTarget(object testValue, string propertyName, bool expected, bool actual)
        {
            if(expected != actual)
                throw new Microsoft.Test.TestValidationException(string.Format("Validation failure for object {0}, property {1}.  Expected: {2}, Actual: {3}",
                testValue, propertyName, expected, actual));
        }

        /******************************************************************************
        * Function:          DependencyObjectDescriptor
        ******************************************************************************/
        /// <summary>
        /// Validates that the public DependencyObjectDescriptor
        /// works as advertised.
        /// </summary>
        public void DependencyObjectDescriptor() 
        {
            GlobalLog.LogStatus("Verify CLR Property");
            DependencyPropertyDescriptor pd = DependencyPropertyDescriptor.FromProperty(TypeDescriptor.GetProperties(typeof(SimplePropertyObject))["ClrTest"]);
            if (pd != null) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            GlobalLog.LogStatus("Verify Dependency Property 1");
            pd = DependencyPropertyDescriptor.FromProperty(TypeDescriptor.GetProperties(typeof(SimplePropertyObject))["Test"]);
            if (pd == null || pd.DependencyProperty != SimplePropertyObject.TestProperty) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            GlobalLog.LogStatus("Verify Dependency Property 2");
            pd = DependencyPropertyDescriptor.FromProperty(SimplePropertyObject.TestProperty, typeof(SimplePropertyObject));
            if (pd == null || pd.DependencyProperty != SimplePropertyObject.TestProperty) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            // Note, owner type here is purposely different from AttachedPropertyObject.
            GlobalLog.LogStatus("Verify Attached Property");
            pd = DependencyPropertyDescriptor.FromProperty(AttachedPropertyObject.AttachedProperty, typeof(DependencyObject));
            if (pd == null || pd.DependencyProperty != AttachedPropertyObject.AttachedProperty) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            GlobalLog.LogStatus("Verify FromName Direct");
            pd = DependencyPropertyDescriptor.FromName(SimplePropertyObject.TestProperty.Name, typeof(SimplePropertyObject), typeof(SimplePropertyObject));
            if (pd == null || pd.DependencyProperty != SimplePropertyObject.TestProperty) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            GlobalLog.LogStatus("Verify FromName Attached");
            pd = DependencyPropertyDescriptor.FromName(AttachedPropertyObject.AttachedProperty.Name, typeof(AttachedPropertyObject), typeof(SimplePropertyObject));
            if (pd == null || pd.DependencyProperty != AttachedPropertyObject.AttachedProperty) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            GlobalLog.LogStatus("Verify FromName Private");
            pd = DependencyPropertyDescriptor.FromName("HiddenPrivate", typeof(SimplePropertyObject), typeof(SimplePropertyObject));
            if (pd != null) { throw new Microsoft.Test.TestValidationException("FAILED");  }

        }

        /******************************************************************************
        * Function:          PropertyDescriptorIsReadOnly
        ******************************************************************************/
        /// <summary>
        /// Verifies PropertyDescriptor.IsReadOnly is correct for various kinds of properties.
        /// </summary>
        public void PropertyDescriptorIsReadOnly() 
        {
            SimplePropertyObject o = new SimplePropertyObject();
            AttachedPropertyObject ao = new AttachedPropertyObject();
            PropertyDescriptor prop;

            GlobalLog.LogStatus("Verify read only for CLR accessors");
            prop = TypeDescriptor.GetProperties(o)["ReadOnlyCLR"];
            if (prop == null || !prop.IsReadOnly) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            GlobalLog.LogStatus("Verify read only for Attached CLR methods");
            prop = TypeDescriptor.GetProperties(o)["AttachedPropertyObject.ReadOnlyAttachedCLR"];
            if (prop == null || !prop.IsReadOnly) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            GlobalLog.LogStatus("Verify read only for CLR metadata");
            prop = TypeDescriptor.GetProperties(o)["ReadOnlyCLRMetadata"];
            if (prop == null || !prop.IsReadOnly) { throw new Microsoft.Test.TestValidationException("FAILED"); }

            GlobalLog.LogStatus("Verify read only for Attached CLR metadata");
            prop = TypeDescriptor.GetProperties(o)["AttachedPropertyObject.ReadOnlyAttachedCLRMetadata"];
            if (prop == null || !prop.IsReadOnly) { throw new Microsoft.Test.TestValidationException("FAILED");  }

            GlobalLog.LogStatus("Verify read only for DP metadata");
            prop = TypeDescriptor.GetProperties(o)["ReadOnlyDPMetadata"];
            if (prop == null || !prop.IsReadOnly) { throw new Microsoft.Test.TestValidationException("FAILED");  }
        }
        #endregion
    }

    [EnableProperty(true)]
    class EnabledButton : Button
    {
    }

    [EnableProperty(false)]
    class DisabledButton : Button
    {
    }

}
