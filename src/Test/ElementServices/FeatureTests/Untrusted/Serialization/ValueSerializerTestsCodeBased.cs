// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model ValueSerializer.
 *          Construct trees, serialize them and verify.
 *
 
  
 * Revision:         $Revision: 1 $
 
 *********************************************************************/
using System;
using System.Windows;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Markup;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Reflection;

namespace Avalon.Test.CoreUI.Serialization.Converter
{


    /// <summary>
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]   
    public class ArrayExtensionTest 
    {

        /// <summary>
        /// </summary>
        [TestCase("2",@"Parser",TestCaseSecurityLevel.FullTrust, "Simple creating a ArrayExtension with type ctor and call addtext.")]
        public void ConstructorTest()
        {

            ArrayExtension typeExt = new ArrayExtension(typeof(object));
    
            
            if(typeExt.Type != typeof(object))
            {
                CoreLogger.LogTestResult(false, "ArrayExtension.Type  didn't match ctor.");
            }

            typeExt.AddText("1");
            typeExt.AddText("2");


            if (typeExt.Items.Count != 2)
            {
                CoreLogger.LogTestResult(false, "ArrayExtension.Items  didn't match the AddText api.");
            }

        }


    }


    /// <summary>
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]   
    public class TypeExtensionTest 
    {

        /// <summary>
        /// </summary>
        [TestCase("2",@"Parser",TestCaseSecurityLevel.FullTrust, "Simple creating a type extension with string ctor.")]
        public void ConstructorTest()
        {

            TypeExtension typeExt = new TypeExtension(typeof(object).Name);
            bool passed = false;
            string comment = "The name didn't match.";
    
            
            if(String.Compare(typeExt.TypeName, typeof(object).Name, true) == 0)
            {
                passed = true;
                comment = "";
            }

            CoreLogger.LogTestResult(passed, comment);
        }


    }


    /// <summary>
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]       
    public class ValueSerializerTest 
    {

        /// <summary>
        /// </summary>
        [TestCase("2", @"Serialization\ValueSerializer",TestCaseSecurityLevel.FullTrust, "ValueSerializer really simple CanConvertFromString call")]
        public void CanConvertFromStringTest()
        {
            MyValueSerializer v = new MyValueSerializer();
            bool passed = false;
            string comment = "ValueSerializer.CanConvertFromString() return true.";

            if(!v.CanConvertFromString(String.Empty, null))
            {
                passed = true;
                comment = "";
            }

            CoreLogger.LogTestResult(passed, comment);
        }

        private class MyValueSerializer : ValueSerializer
        {

        }

    }

    /// <summary>
    /// ValueSerializer code-based test suit with a Model. 
    /// </summary>
    /// 
    [TestCaseModel("ValueSerializerCases.xtc", "0", @"Serialization\ValueSerializer", TestCaseSecurityLevel.FullTrust, "ValueSerializer model pairwise")]
    public class ValueSerializerModel : Model 
    {

        /// <summary>
        /// Construct new instance of the model.
        /// </summary>
        public ValueSerializerModel() : base()
        {
            Name = "ValueSerializerModel";
            Description = "ValueSerializer Model";

            //Add Action Handlers
            AddAction("RunTest", new ActionHandler(RunTest));
        }

        /// <summary>
        /// Single action for this model.  Constructs a tree based on
        /// the parameter combination; loads the tree, serialize the tree,
        /// and verify.
        /// </summary>
        /// <remarks>Handler for RunTest</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RunTest(State endState, State inParams, State outParams)
        {
            ModelState = new ValueSerializerModelState(inParams);
            ModelState.LogState();
            CreateObjectTree();
            SerializeTree();
            VerifySerialized();
            return true;
        }

        /// <summary>
        /// Create the object tree according to the parameter read from xtc.
        /// </summary>
        private void CreateObjectTree()
        {
            Style style = new Style(typeof(CustomElementWithCustomTypeProperties));
            Trigger trigger = new Trigger();

            //Get the type name
            string elementTypeName =  ModelState.Declaration + ModelState.DeclarationLocation + ModelState.TypeConverter + ModelState.LocationOfTypeConverter;
            //For element in VT, FrameworkElement is required. 
            if(String.Equals(ModelState.Location, "ControlTemplateVisualTree", StringComparison.InvariantCulture))
            {
                elementTypeName = "CustomFrameworkElement" + elementTypeName;
            }
            //FrameworkElement type is not allowed in Setter. 
            else
            {
               elementTypeName = "CustomElement" + elementTypeName;
            }
             
            Type innerObjectType = Type.GetType(ValueSerializerNameSpacePrefix + "." + elementTypeName + ", " + UntrustedAssemblyName);
            if (null == innerObjectType)
            {
                throw new Microsoft.Test.TestValidationException("Not found type: " + elementTypeName);
            }

            //Create an instance of the inner object.
            object innerObject = GetInnerObject(innerObjectType);
            DependencyProperty dp = null;

            //Craate the object tree.
            ObjectTree = new CustomElementWithCustomTypeProperties();
            switch (ModelState.Location)
            {
                case "Normal":
                    ObjectTree.Content = innerObject;
                    break;
                case "StyleSetterValue":
                    dp = GetDependencyProperty(elementTypeName + ModelState.PropertyType);
                    style.Setters.Add(new Setter(dp, innerObject));
                    break;
                case "StyleTriggerSetterValue":
                    dp = GetDependencyProperty(elementTypeName + ModelState.PropertyType);
                    trigger.Property = CustomElementWithCustomTypeProperties.StringPropertyProperty;
                    trigger.Value = string.Empty;
                    //Get DependencyProperty.
                    trigger.Setters.Add(new Setter(dp, innerObject));
                    style.Triggers.Add(trigger);
                    ObjectTree.Style = style;
                    break;
                case "StyleTriggerConditionProperty":
                    dp = GetDependencyProperty(elementTypeName + ModelState.PropertyType);
                    trigger.Property = dp;
                    trigger.Value = innerObject;
                    style.Triggers.Add(trigger);
                    ObjectTree.Style = style;
                    break;

                    //style.Setters.Add(new Setter(Control.TemplateProperty, template));
                case "NormalProperty":

                    if (string.Equals(ModelState.PropertyType, "DP", StringComparison.InvariantCulture)
                        || string.Equals(ModelState.PropertyType, "Attached", StringComparison.InvariantCulture)
                        )
                    {
                        dp = GetDependencyProperty(elementTypeName + ModelState.PropertyType);
                        ObjectTree.SetValue(dp, innerObject);
                    }
                    else if (string.Equals(ModelState.PropertyType, "Clr", StringComparison.InvariantCulture))
                    {
                        PropertyInfo property = typeof(CustomElementWithCustomTypeProperties).GetProperty(elementTypeName + ModelState.PropertyType);
                        if (null == property)
                        {
                            throw new Microsoft.Test.TestSetupException("Property: " + elementTypeName + ModelState.PropertyType + " not found.");
                        }

                        property.SetValue(ObjectTree, innerObject, null);
                    }
                    else 
                    {
                        throw new Microsoft.Test.TestValidationException("Not valid property type.");
                    }
                    break;


                case "ControlTemplateVisualTree":
         
                    ControlTemplate template = new ControlTemplate(typeof(CustomElementWithCustomTypeProperties));
                    FrameworkElementFactory fef = new FrameworkElementFactory(innerObject.GetType());
                    //Only FrameworkElement is allowed in VT and our type is not FE, we put it in a property. 
                    dp = GetValueStringDP(typeof(CustomFrameworkElementBase));
                    fef.SetValue(dp, ValueString);

                    template.VisualTree = fef;
                    ((Control)ObjectTree).Template = template;
                    break;
                
                default:
                    ObjectTree = null;
                    break;
            }

            //In the case it is supposed to be in the ResourceDictionary
            //add the object tree to the ResourceDictionary of another 
            //instance and use the new instance as object tree. 
            if (string.Equals(ModelState.IsResource, "Yes", StringComparison.InvariantCulture))
            {
                CustomElementWithCustomTypeProperties outLayer = new CustomElementWithCustomTypeProperties();
                outLayer.Resources.Add("resourceKey", ObjectTree);
                ObjectTree = outLayer;
            }
            return;
        }

        /// <summary>
        /// Given property name, get the DependencyProperty of CustomElementWithCustomTypeProperties.
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        DependencyProperty GetDependencyProperty(string PropertyName)
        {
            FieldInfo field = typeof(CustomElementWithCustomTypeProperties).GetField(PropertyName + "Property");
            DependencyProperty dp = field.GetValue(ObjectTree) as DependencyProperty;
            if (null == dp)
            {
                throw new Microsoft.Test.TestValidationException("Dependency Field not found on type CustomElementWithCustomTypeProperties.");
            }
            return dp;
        }


        /// <summary>
        /// Get the value of ValueProperty.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        DependencyProperty GetValueStringDP(Type type)
        {
            FieldInfo field = null;
            if (type.IsAssignableFrom(typeof(CustomElementBase)))
            {
                return CustomElementBase.ValueProperty;
            }
            else
            {
                return CustomFrameworkElementBase.ValueProperty;
            }
        }

        /// <summary>
        /// With the type, create an instance. 
        /// Throw if the new instance is null.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

        Object GetInnerObject(Type type)
        {
            Object[] parameters = {ValueString};
            Object innerObject = Activator.CreateInstance(type, parameters);
            
            if (null == innerObject)
            {
                throw new Microsoft.Test.TestSetupException("cannot create an instance of type : " + type.Name);
            }
            return innerObject;
        }

        /// <summary>
        /// Serialize the object tree.
        /// </summary>
        private void SerializeTree()
        {
            if (null == ObjectTree)
            {
                throw new NotImplementedException("Test case Notimplemented.");
            }

            Serialized = SerializationHelper.SerializeObjectTree(ObjectTree, XamlWriterMode.Expression);
        }

        /// <summary>
        /// verify that the Value has been serialized as attribute if it is supposed to be so.
        /// </summary>
        private void VerifySerialized()
        {
            string prefixOfValue = "ValueSerializer1";

            //using type converter. 
            if (String.Equals(ModelState.DeclarationLocation, "None", StringComparison.InvariantCulture)
               || String.Equals(ModelState.Location, "ControlTemplateVisualTree", StringComparison.InvariantCulture))
            {
                prefixOfValue = string.Empty;
            }

            bool shouldSerializeAsAttribute = true;
            if ((//No ValueSerializer
                String.Equals(ModelState.Declaration, "None", StringComparison.InvariantCulture)
                //No Valid TypeConverter
                && (!String.Equals(ModelState.TypeConverter, "Valid", StringComparison.InvariantCulture) || String.Equals(ModelState.LocationOfTypeConverter, "Type", StringComparison.InvariantCulture)))
                || String.Equals(ModelState.Location, "ControlTemplateVisualTree", StringComparison.InvariantCulture))
            {
                shouldSerializeAsAttribute = false;
            }

            if (0 == Serialized.Length)
            {
                throw new NotImplementedException("Test case Notimplemented.");
            }
            //Verify that the ValueString has been serialized. 
            if (-1 == Serialized.IndexOf(prefixOfValue + ValueString))
                throw new Microsoft.Test.TestValidationException("ValueString has not been serialized.");
            
            //Verify that the type has been serialized as string. 
            if ((-1 != Serialized.IndexOf("Value=\"" + prefixOfValue + ValueString))
                == shouldSerializeAsAttribute)
                throw new Microsoft.Test.TestValidationException("Type has not been serialized with correct converter.");

        }

        /// <summary>
        /// Serialized string.
        /// </summary>
        string Serialized
        {
            get
            {
                return _serialized;
            }
            set
            {
                _serialized = value;
            }
        }

        /// <summary>
        /// ModelState.
        /// </summary>
        ValueSerializerModelState ModelState
        {
            get
            {
                return _modelState;
            }
            set
            {
                _modelState = value;
            }
        }
        /// <summary>
        /// Root of the object tree to be serialized.
        /// </summary>
        CustomElementWithCustomTypeProperties ObjectTree
        {
            get
            {
                return _objectTree;
            }
            set
            {
                _objectTree = value;
            }
        }
    
        /// <summary>
        /// Value string of Value property for the instance to be serialized. 
        /// </summary>
        string ValueString
        {
            get
            {
                return _valueString;
            }
        }

        /// <summary>
        /// Name of CoretestsUntrusted assembly, where the custom elements are located.
        /// </summary>
        string UntrustedAssemblyName
        {
            get
            {
                return _untrustedAssemblyName;
            }
        }

        /// <summary>
        /// Namespace for custom elements.
        /// </summary>
        string ValueSerializerNameSpacePrefix
        {
            get
            {
                return _valueSerializerNameSpacePrefix;
            }
        }

        string _serialized = String.Empty;
        ValueSerializerModelState _modelState = null;
        CustomElementWithCustomTypeProperties _objectTree = null;
        string _valueString = "ValueString";
        string _untrustedAssemblyName = "CoretestsUntrusted";
        string _valueSerializerNameSpacePrefix = "Avalon.Test.CoreUI.Serialization.Converter";
    }

    /// <summary>
    /// ValueSerializerModelState inherits CoreModelState and 
    /// holds the parameters from the Model, as well as a LogState function 
    /// which print out the information about the correct state. 
    /// </summary>
    [Serializable()]
    class ValueSerializerModelState : CoreModelState
    {

        public ValueSerializerModelState(State state)
        {
            Location = state["Location"];
            PropertyType = state["PropertyType"];
            Declaration = state["Declaration"];
            DeclarationLocation = state["DeclarationLocation"];
            TypeConverter = state["TypeConverter"];
            LocationOfTypeConverter = state["LocationOfTypeConverter"];
            IsResource = state["IsResource"];
        }

        public override void LogState()
        {

            CoreLogger.LogStatus("  Location: " + Location +
                           "\r\n  PropertyType: " + PropertyType +
                           "\r\n  Declaration: " + Declaration +
                           "\r\n  DeclarationLocation: " + DeclarationLocation +
                           "\r\n  TypeConverter: " + TypeConverter +
                           "\r\n  LocationOfTypeConverter: " + LocationOfTypeConverter +
                           "\r\n  IsResource: " + IsResource);            
        }

        public string Location;
        public string PropertyType;
        public string Declaration;
        public string DeclarationLocation;
        public string TypeConverter;
        public string LocationOfTypeConverter;
        public string IsResource;
    }

}
