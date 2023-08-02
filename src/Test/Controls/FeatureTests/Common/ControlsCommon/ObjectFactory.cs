using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace Avalon.Test.ComponentModel
{
    public static class ObjectFactory
    {
        private static string[] assemblyNames = new string[] {
                "ControlsCommon",
                "ControlsTest"
            };

        private static string[] namespaces = new string[] { 
                "System.Windows.Controls",
                "System.Windows.Controls.Primitives",
                "System.Windows.Shapes",
                "Microsoft.Test.Controls",
                "Microsoft.Test.Controls.Methods",
                "Microsoft.Test.Controls.Properties",
                "Avalon.Test.ComponentModel", 
                "Avalon.Test.ComponentModel.Actions", 
                "Avalon.Test.ComponentModel.Validations", 
                "Avalon.Test.ComponentModel.Utilities",
                "Avalon.Test.ComponentModel.UnitTests"
            };

        public static object CreateObjectFromXaml(string xamlString)
        {
            if (string.IsNullOrEmpty(xamlString))
            {
                return null;
            }

            if (xamlString.ToString().Contains("CodeplexControls"))
            {
                Assembly assm = Assembly.LoadFrom("CodeplexControls.dll");
            }

            MemoryStream memoryStream = new MemoryStream(xamlString.Length);
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(xamlString);
            streamWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return XamlReader.Load(memoryStream);
        }

        public static object CreateObjectFromTypeName(string objectName)
        {
            return CreateObjectFromTypeName(objectName, assemblyNames, namespaces);
        }

        public static object CreateObjectFromTypeName(string objectName, string[] assemblyNames, string[] namespaces)
        {
            if (String.IsNullOrEmpty(objectName))
            {
                throw new ArgumentException("objectName cannot be null", "objectName");
            }

            Type type = FindType(objectName, assemblyNames, namespaces);

            if (type == null)
            {
                throw new Exception("Could not find the type " + objectName);
            }

            return Activator.CreateInstance(type, new object[0]);
        }

        public static Type FindType(string objectName)
        {
            return FindType(objectName, assemblyNames, namespaces);
        }

        private static Type FindType(string objectName, string[] assemblies, string[] nameSpaces)
        {
            Type type = null;
            foreach (string assembly in assemblies)
            {
                foreach (string nameSpace in nameSpaces)
                {
                    // We use Assembly.Load method to load test assemblies to find test type
                    // because the test dlls are not in the gac.
                    type = Assembly.Load(assembly).GetType(nameSpace + "." + objectName, false);
                    if (type != null)
                    {
                        return type;
                    }

                    // We use FrameworkElement.Assembly to find WPF control type 
                    // because the PresentationFramework.dll is in the gac.
                    type = typeof(FrameworkElement).Assembly.GetType(nameSpace + "." + objectName, false);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            return type;
        }

        /// <summary>
        /// Sets the objects property to the object passed in.
        /// </summary>
        /// <param name="obj">The object that has the property you want to change.</param>
        /// <param name="propName">The name of the property you want to change.</param>
        /// <param name="propValObj">The object value you want to set.</param>
        public static void SetObjectPropertyToObject(object obj, string propName, object propValObj)
        {
            if (String.IsNullOrEmpty(propName) || obj == null)
            {
                throw new ArgumentException("arguments cannot be null", "null argument");
            }

            //find the property to be set from the resource object
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propName);

            if (null == propertyInfo)
            {
                //can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propName + " in the control");
            }

            propertyInfo.SetValue(obj, propValObj, null);
        }

        /// <summary>
        /// Used to obtain the property value of a given property on the specified object.
        /// </summary>
        /// <param name="obj">Object which you want to get the property value from.</param>
        /// <param name="propertyName">The property you want to get the property value from.</param>
        /// <returns>The property value.</returns>
        public static object GetObjectProperty(object obj, string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName) || obj == null)
            {
                throw new ArgumentException("arguments cannot be null", "null argument");
            }

            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (null == propertyInfo)
            {
                //can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propertyName + " in the control");
            }

            return propertyInfo.GetValue(obj, new object[0]);
        }

        public static bool VerifyProperty(object obj, string propertyName, string propertyValue)
        {
            if (String.IsNullOrEmpty(propertyName) || String.IsNullOrEmpty(propertyValue) || obj == null)
            {
                throw new ArgumentException("arguments cannot be null", "null argument");
            }

            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (null == propertyInfo)
            {
                //can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propertyName + " in the control");
            }

            object currentValue = propertyInfo.GetValue(obj, new object[0]);

            if (currentValue == null)
            {
                return propertyValue == null;
            }

            object expectedValue;
            TypeConverter typeConverter = TypeDescriptor.GetConverter(currentValue.GetType());

            if (typeConverter.CanConvertFrom(typeof(string)))
            {
                expectedValue = typeConverter.ConvertFromInvariantString(propertyValue);
            }
            else
            {
                throw new InvalidOperationException("The property Type does not have a type converter that can convert from a string");
            }

            return CompareObjects(currentValue, expectedValue);
        }


        public static bool CompareObjects(object a, object b)
        {
            // Are they the same object?
            if (a == b)
            {
                return true;
            }

            // Are either null?
            if (a == null || b == null)
            {
                return false;
            }

            // Are they of the same type?
            if (a.GetType() == b.GetType())
            {
                // Check the comparable interface to determine a match
                if (a is IComparable)
                {
                    return Comparer.Default.Compare(a, b) == 0;
                }
                else
                {
                    // Check the object equals to determine a match
                    return a.Equals(b);
                }
            }

            // No match, default to false
            return false;
        }
    }
}

