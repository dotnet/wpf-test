// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.IO;
using System.Windows;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Test.Security.Wrappers;
using System.Windows.Markup;

namespace Avalon.Test.ComponentModel
{
    // it's for creating avalon control only.
    public class ControlObjectFactory
    {
        public ControlObjectFactory()
        {
        }

        public ArrayList objectNamespaces;

        public object CreateObjectFromXml(XmlElement root)
        {
            if (root == null)
                return null;

            return CreateObjectFromXml(root.OuterXml);
        }

        public object CreateObjectFromXml(string str)
        {
            if (str == null || str == string.Empty)
                return null;

            MemoryStream ms = new MemoryStream(str.Length);
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(str);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return XamlReader.Load(ms);
        }

        public Object CreateObject(string objectName)
        {
            if (String.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be null", "objectName");

            Type type = null;

            objectNamespaces = new ArrayList();
            string controlsNamespace = "System.Windows.Controls";
            string controlsPrimitivesNamespace = "System.Windows.Controls.Primitives";
            string shapesPrimitivesNamespace = "System.Windows.Shapes";
            objectNamespaces.Add(controlsNamespace);
            objectNamespaces.Add(controlsPrimitivesNamespace);
            objectNamespaces.Add(shapesPrimitivesNamespace);

            foreach (string objectNamespace in objectNamespaces)
            {
                type = typeof(FrameworkElement).Assembly.GetType(objectNamespace + "." + objectName, false);
                if (type != null)
                    break;
            }

            if (type == null)
                throw new Exception("Could not find the type " + objectName + " in the object namespaces");

            return (Object)Activator.CreateInstance(type, new object[0]);
        }

        public void SetObjectProperty(object obj, string propName, string propVal)
        {
            if (String.IsNullOrEmpty(propName) || String.IsNullOrEmpty(propVal) || obj == null)
                throw new ArgumentException("arguments cannot be null", "null argument");

            //find the property to be set from the resource object
            PropertyInfoSW pi = TypeSW.Wrap(obj.GetType()).GetProperty(propName);

            if (null == pi)
            {
                //can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propName + " in the control");
            }

            //get type info of the property
            Type propType = pi.PropertyType.InnerObject;

            //get the type converter of the property type, which can be used to parse
            //the string value into the property type.
            TypeConverter propTypeConverter = TypeDescriptor.GetConverter(propType);
            object propValObj = null;

            try
            {
                propValObj = propTypeConverter.ConvertFromInvariantString(propVal);
            }
            catch (NotSupportedException)
            {
                throw new Exception("Can't convert string value property value to type property type");
            }

            //if we succeed in parsing the string into the property type
            //assign the resulting object to the resource's property
            //through reflection
            pi.SetValue(obj, propValObj, null);
        }

        /// <summary>
        /// Set the specified object as the value of the property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <param name="propVal"></param>
        public void SetObjectProperty(object obj, string propName, object propVal)
        {
            if (String.IsNullOrEmpty(propName) || obj == null)
                throw new ArgumentException("arguments cannot be null", "null argument");

            //find the property to be set from the resource object
            PropertyInfoSW pi = TypeSW.Wrap(obj.GetType()).GetProperty(propName);

            if (null == pi)
            {
                //can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propName + " in the control");
            }

            pi.SetValue(obj, propVal, null);
        }

        public void SetMultipleObjectProperties(FrameworkElement frmElement, params object[] actionParams)
        {
            if (frmElement == null)
            {
                throw new ArgumentException("First parameter must be a FrameworkElement.");
            }

            string ctlPropertyDescriptor = null;
            string propertyName, propertyValue = null;

            if (actionParams != null && actionParams.Length > 0)
            {
                for (int i = 0; i < actionParams.Length; ++i)
                {
                    ctlPropertyDescriptor = actionParams[i] as string;
                    if (String.IsNullOrEmpty(ctlPropertyDescriptor))
                        throw new ApplicationException("ctlPropertyDescriptor is null");

                    //Find the index to place the property name 
                    int idx_equal_sign = ctlPropertyDescriptor.IndexOf('=');

                    if (idx_equal_sign < 0)
                    {
                        throw new ArgumentException("Param number " + i + " is of incorrect format. Should be <propname>=<propvalue (type convertable string)>", "actionParams[" + i + "]");
                    }

                    //Set the propertyName before the equal sign index.
                    propertyName = ctlPropertyDescriptor.Substring(0, idx_equal_sign);

                    //Set the property value after the first index
                    propertyValue = ctlPropertyDescriptor.Substring(idx_equal_sign + 1);
                    if (String.IsNullOrEmpty(propertyValue) || String.IsNullOrEmpty(propertyName))
                        throw new ApplicationException("propertyValue or propertyName is null");

                    //calling the ControlObjectFactory SetObjectProperty to set properties on the control.
                    //controlObject = new ControlObjectFactory();
                    SetObjectProperty(frmElement, propertyName, propertyValue);
                }
            }
            else
            {
                throw new ArgumentException("Param is invalid.");
            }
        }

        /// <summary>
        /// Sets the objects property to the object passed in.
        /// </summary>
        /// <param name="obj">The object that has the property you want to change.</param>
        /// <param name="propName">The name of the property you want to change.</param>
        /// <param name="propValObj">The object value you want to set.</param>
        public void SetObjectPropertyToObject(object obj, string propName, object propValObj)
        {
            if (String.IsNullOrEmpty(propName) || propValObj == null || obj == null)
                throw new ArgumentException("arguments cannot be null", "null argument");

            //find the property to be set from the resource object
            PropertyInfoSW pi = TypeSW.Wrap(obj.GetType()).GetProperty(propName);

            if (null == pi)
            {
                //can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propName + " in the control");
            }

            pi.SetValue(obj, propValObj, null);
        }

        /// <summary>
        /// Used to obtain the property value of a given property on the specified object.
        /// </summary>
        /// <param name="obj">Object which you want to get the property value from.</param>
        /// <param name="propertyName">The property you want to get the property value from.</param>
        /// <returns>The property value.</returns>
        public object GetObjectProperty(object obj, string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName) || obj == null)
                throw new ArgumentException("arguments cannot be null", "null argument");

            PropertyInfo pInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (null == pInfo)
            {
                //can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propertyName + " in the control");
            }

            object curValue = pInfo.GetValue(obj, new object[0]);

            return curValue;
        }

        public bool VerifyProperty(object obj, string propertyName, string propertyValue)
        {
            if (String.IsNullOrEmpty(propertyName) || String.IsNullOrEmpty(propertyValue) || obj == null)
                throw new ArgumentException("arguments cannot be null", "null argument");

            PropertyInfo pInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (null == pInfo)
            {
                //can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propertyName + " in the control");
            }

            object curValue = pInfo.GetValue(obj, new object[0]);

            if (curValue == null)
                return propertyValue == null;

            object expValue;
            TypeConverter typeConverter = TypeDescriptor.GetConverter(curValue.GetType());

            if (typeConverter.CanConvertFrom(typeof(string)))
                expValue = typeConverter.ConvertFromInvariantString(propertyValue);
            else
                throw new InvalidOperationException("The property Type does not have a type converter that can convert from a string");

            return CompareObjects(curValue, expValue);
        }

        public static bool CompareObjects(object a, object b)
        {
            // Are they the same object?
            if (a == b)
                return true;

            // Are either null?
            if (a == null || b == null)
                return false;

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


