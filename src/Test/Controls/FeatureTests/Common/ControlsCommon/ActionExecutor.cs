using System;
using System.Windows;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Avalon.Test.ComponentModel.Utilities
{
    public static class ActionExecutor 
    {
        private class TestMethod
        {
            private string name;
            private Dictionary<string, ParameterInfo> parameters = new Dictionary<string, ParameterInfo>(StringComparer.InvariantCultureIgnoreCase);

            // Properties
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public Dictionary<string, ParameterInfo> Parameters
            {
                get { return parameters; }
            }
        }

        /// <summary>
        /// Parse xml generically  to create objects that match parameters in my own function using reflection.
        /// If namespace!="" then use the xaml parser to create the object.
        /// Some special syntax for looking up element in the logical tree of the scene {SceneTreeSearch: name}  {Scene}.
        /// Use reflection to call my function.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="actionElement"></param>
        /// <returns>Returns true if we successfully invoke the method. Otherwise, returns false.</returns>
        public static bool Execute(FrameworkElement scene, XmlElement actionElement)
        {
            if (scene == null)
            {
                throw new ArgumentNullException("scene");
            }
            if (actionElement == null)
            {
                throw new ArgumentNullException("actionElement");
            }

            XtcActionMethodParser xtcObjectMethodParser = new XtcActionMethodParser(actionElement);

            Type actionType = ObjectFactory.FindType(xtcObjectMethodParser.ObjectName);
            if (actionType == null)
            {
                throw new ArgumentException("Could not find the action type " + xtcObjectMethodParser.ObjectName, "actionElement");
            }

            // Get the test method.
            MethodInfo methodInfo = actionType.GetMethod(xtcObjectMethodParser.MethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (methodInfo == null)
            {
                throw new ArgumentException("The Action class " + actionType.Name + " does not contain a Public Instance method named " + xtcObjectMethodParser.MethodName, "actionElement");
            }

            // Create a TestMethod holder.
            TestMethod testMethod = new TestMethod();
            testMethod.Name = xtcObjectMethodParser.MethodName;

            // Collect all the parameters of the function.
            foreach (ParameterInfo methodParameter in methodInfo.GetParameters())
            {
                if (testMethod.Parameters.ContainsKey(methodParameter.Name))
                {
                    throw new ArgumentException("The method " + testMethod.Name + " contains a more then one parameter with the same name", "actionElement");
                }
                testMethod.Parameters.Add(methodParameter.Name, methodParameter);
            }

            object[] methodParameters = new object[methodInfo.GetParameters().Length];
            ParameterInfo parameter;

            // Loop through action object attributes to collect parameters that are attributes.
            foreach (XmlAttribute xmlAttribute in actionElement.Attributes)
            {
                // Get the parameter that matches the attribute name.
                parameter = FindParameter(testMethod, xmlAttribute.LocalName);

                // Convert the attribute value to the parameter value.
                object value = xtcObjectMethodParser.ConvertAttributeToParameter(xmlAttribute.Value, parameter.ParameterType, scene);

                AddMethodParameter(testMethod, parameter, value, methodParameters);
            }

            // Parameters may also be specified by child xml elements, so enumerate the xml elements to set remaining parameters.
            foreach (XmlElement childElement in actionElement.SelectNodes("*"))
            {
                // Get the parameter that matches the element name.
                parameter = FindParameter(testMethod, childElement.LocalName);

                // Convert the element value to the parameter value.
                object value = xtcObjectMethodParser.ConvertElementToParameter(childElement, parameter, scene);

                AddMethodParameter(testMethod, parameter, value, methodParameters);
            }

            // Make sure that all parameters where specified.
            if (testMethod.Parameters.Count > 0)
            {
                string[] missedParams = new string[testMethod.Parameters.Count];
                testMethod.Parameters.Keys.CopyTo(missedParams, 0);
                throw new ArgumentException("The method " + testMethod.Name + " does not specify values for the parameters: " + string.Join(",", missedParams), "actionElement");
            }

            // If the method is not static, we need to create an instance of the object.
            object testObject = null;
            if (!methodInfo.IsStatic)
            {
                testObject = Activator.CreateInstance(actionType, new object[0]);
            }
            
            // Invoke the method.
            return (bool)methodInfo.Invoke(testObject, methodParameters);
        }

        private static ParameterInfo FindParameter(TestMethod testMethod, string parameterName)
        {
            // Get the parameter that matches the attribute name.
            if (!testMethod.Parameters.ContainsKey(parameterName))
            {
                throw new ArgumentException("The method " + testMethod.Name + " does not contain a parameter named: " + parameterName, "actionElement");
            }
            return testMethod.Parameters[parameterName];
        }

        private static void AddMethodParameter(TestMethod testMethod, ParameterInfo parameter, object value, object[] methodParameters)
        {
            // Make sure that the value type is compatible.
            if (!parameter.ParameterType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException("The value for parameter " + parameter.Name + " is of type " + value.GetType().Name + " which is not compatible with the parameter type " + parameter.ParameterType.Name, "actionElement");
            }

            // Add the value into the right place in the method parameters.
            methodParameters[parameter.Position] = value;

            // Remove the parameter from the dictionary to indicate we have assiged a value to it.
            testMethod.Parameters.Remove(parameter.Name);
        }
    }
}


