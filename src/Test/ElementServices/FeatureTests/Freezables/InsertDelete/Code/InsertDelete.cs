// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2006
 *
 *   Program:   TestLogicalTree 
 
 *  This test does the following:
 *   - Load xaml, then Insert or delete tree node base on user input defined in action file
 *   - Verify that if the parent node in the tree is frozen, then the children must be frozen.
 * Note: xaml should have the following attributes:
 *  xmlns:PresentationOptions="http://schemas.microsoft.com/winfx/2006/xaml/presentation/options" 
 *  xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
 *  mc:Ignorable="PresentationOptions"
 * PresentationOptions:Freeze="True or False"

 ************************************************************/

using System;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.ElementServices.Freezables.Objects;
using Microsoft.Test.ElementServices.Freezables.Utils;

namespace Microsoft.Test.ElementServices.Freezables
{
    ////////////////////////////////////////////////////////////////////////////////////////////
    // DISABLEDUNSTABLETEST:
    // TestName:InsertDelete
    // Area: ElementServices   SubArea: Freezables.InsertDelete
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
    ////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// <area>ElementServices\Freezables\InsertDelete</area>
 
    /// <priority>3</priority>
    /// <description>
    /// InsertDelete tests
    /// </description>
    /// </summary>
    [Test(3, "Freezables.InsertDelete", "InsertDelete", SupportFiles = @"FeatureTests\ElementServices\pathrect.xaml,FeatureTests\ElementServices\child.xaml,FeatureTests\ElementServices\insertrect.xml", Disabled = true)]

    public class InsertDelete : AvalonTest
    {
        #region Private Data
        private string              _xamlFile        = "";
        private string              _controlFile     = "";
        private bool                _found;
        private List<int>           _objectsInTree;
        private Microsoft.Test.ElementServices.Freezables.Utils.Result _result;
        private string              _action          = "";
        private string              _parentName      = "";
        private string              _childName       = "";
        private string              _actionFile      = "";
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          InsertDelete Constructor
        ******************************************************************************/
        public InsertDelete()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(Verify);
        }
        #endregion


        #region Private Members

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Sets global variables.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult Initialize()
        {
            _xamlFile        = "pathrect.xaml";
            _controlFile     = "insertrect.xml";

            _result = new Microsoft.Test.ElementServices.Freezables.Utils.Result();

            // Create hashtables contain objects in the current trees and later on add the 
            // objects into it. This is used to break loop
            _objectsInTree = new List<int>();

            return TestResult.Pass;
        }


        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        /// <summary>
        /// Carries out the requested InsertDelete tests.  A global variable tracks pass/fail.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult RunTest()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(_controlFile);
            }
            catch (Exception)
            {
                throw new ApplicationException("Unable to load " + _controlFile);
            }

            XmlElement freezableTest = (XmlElement)doc["FreezableTest"];
            for (XmlNode actionNode = freezableTest["Action"]; actionNode != null; actionNode = actionNode.NextSibling)
            {
                XmlAttribute a = actionNode.Attributes["Type"];
                XmlAttribute p = actionNode.Attributes["ParentNode"];
                XmlAttribute c = actionNode.Attributes["ChildNode"];
                XmlAttribute f = actionNode.Attributes["FileName"];
                if (a == null || p == null || c == null || f == null)
                {
                    throw new ApplicationException("Action Element must have attributes Type=, ChildNode=, ParentNode=, FileName=");
                }
                _action = Convert.ToString(a.Value);
                _parentName = Convert.ToString(p.Value);
                _childName = Convert.ToString(c.Value);
                _actionFile = Convert.ToString(f.Value);

                PerformTest();
            }

            return TestResult.Pass;
        }


        /******************************************************************************
        * Function:          PerformTest
        ******************************************************************************/
        /// <summary>
        /// Carries out the requested InsertDelete tests.  A global variable tracks pass/fail.
        /// </summary>
        /// <returns></returns>
        private void PerformTest()
        {
            Stream xamlStream = File.OpenRead(_xamlFile);
            object xaml = XamlReader.Load(xamlStream);
            _found = false;
            object parentNode = null;
            GetNodeFromXaml(out parentNode, xaml, _parentName);
            if (parentNode == null)
            {
                _result.passed = false;
                _result.failures.Add("Cannot find " + _parentName + " in file: " + _actionFile);
                GlobalLog.LogStatus("Cannot find " + _parentName + " in file: " + _actionFile);
                return;
            }
     
            Stream xmlStream = File.OpenRead(_actionFile);
            object xml = XamlReader.Load(xmlStream);
            _found = false;
            object childNode = null; 
            GetNodeFromXaml(out childNode, xml, _childName);
            if (childNode == null)
            {
                _result.passed = false;
                _result.failures.Add("Cannot find " + _childName + " in file: " + _actionFile);
                GlobalLog.LogStatus("Cannot find " + _childName + " in file: " + _actionFile);
                return;
            }
            
            switch (_action)
            {
                case "Delete":
                    {
                        TestDelete(parentNode, childNode);
                        break;
                    }
                case "Insert":
                {
                    TestInsert(parentNode, childNode);
                    break;
                }
                default:
                {
                    throw new ApplicationException("Unknown action: " + _action);   
                }
            }
        }


        /******************************************************************************
        * Function:          TestInsert
        ******************************************************************************/
        /// <summary>
        /// Tests inserting a node.
        /// </summary>
        /// <param name='parentNode'>The parent of the node to be added</param>
        /// <param name='childNode'>The child to be inserted into the tree</param>
        /// <returns></returns>
        private void TestInsert(object parentNode, object childNode)
        {
            //Console.WriteLine("{0} {1}", parentNode, childNode);
            IAddChild parent = parentNode as IAddChild;
            DependencyObject child = childNode as DependencyObject;
            parent.AddChild((UIElement)child);
            _objectsInTree.Clear();

            VerifyFreezable((object)parent);
        }


        /******************************************************************************
        * Function:          TestDelete
        ******************************************************************************/
        /// <summary>
        /// Tests deleting a node.
        /// </summary>
        /// <param name='parentNode'>The parent of the child to be deleted</param>
        /// <param name='childNode'>The child to be deleted from the tree</param>
        /// <returns></returns>
        private void TestDelete(object parentNode, object childNode)
        {
            //Console.WriteLine("{0} {1}", parentNode, childNode);
            if (parentNode is Panel)
            {
                ((Panel)parentNode).Children.Remove((UIElement)childNode);
            }
            else if (parentNode is ContentControl)
            {
                ((ContentControl)parentNode).Content = null;
            }
            else if (parentNode is ItemsControl)
            {
                ((ItemsControl)parentNode).Items.Remove(childNode);
            }
            else if (parentNode is Decorator)
            {
                ((Decorator)parentNode).Child = null;
            }
            else 
            {
                _result.passed = false;
                _result.failures.Add("Don't know how to remove " + childNode + " from " + parentNode);
                GlobalLog.LogStatus("Don't know how to remove " + childNode + " from " + parentNode);
            }
            _objectsInTree.Clear();
            VerifyFreezable((object)parentNode);
        }


        /******************************************************************************
        * Function:          GetNodeFromXaml
        ******************************************************************************/
        /// <summary>
        /// Retrieve a node.
        /// </summary>
        /// <param name='node'>The node returned from Xaml</param>
        /// <param name='obj'>object from xaml or xml</param>
        /// <param name='nodeName'>The name of the node to be retrieved</param>
        /// <returns></returns>
        private void GetNodeFromXaml(out object node, object obj, string nodeName)
        {
            node = null;
            if (_objectsInTree.Contains(obj.GetHashCode()))
            {
                return;
            }
            
            _objectsInTree.Add(obj.GetHashCode());
            Type t = obj.GetType();
            if (t.ToString() == nodeName)
            {
                //Console.WriteLine("FOUND" + obj);
                _found = true;
                node = obj;
                return;
            }
            else
            {
                PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in pi)
                {
                    if (_found)
                    {
                        break;
                    }
                    if (IsBadProperty(property.Name, obj))
                    {
                        continue;
                    }
                    if (IsGenericTypeMember(property.GetType(), property.Name))
                    {
                        continue;
                    }
                    //Console.WriteLine("Property: " + property);
                    if (property.PropertyType.ToString().Contains("Collection"))
                    {
                        object children = property.GetValue(obj, null);
                        //Console.WriteLine(children);
                        foreach (object child in (IEnumerable)children)
                        {
                            if (_found)
                            {
                                break;
                            }
                            GetNodeFromXaml(out node, child, nodeName);
                        }
                    }
                    else
                    {
                        object propObj = GetPropertyValue(obj, property);
                        if (propObj != null && !propObj.GetType().IsValueType)
                        {
                            GetNodeFromXaml(out node, propObj, nodeName);
                        }
                    }
                }
            }
        }


        /******************************************************************************
        * Function:          GetPropertyValue
        ******************************************************************************/
        /// <summary>
        /// Retrieve a property value.
        /// </summary>
        /// <param name='obj'>The object possessing the requested property</param>
        /// <param name='property'>The property whose value is returned</param>
        /// <returns>The value of the property requested</returns>
        private object GetPropertyValue(object obj, PropertyInfo property)
        {
            object propObj = null;
            ParameterInfo[] parms = property.GetIndexParameters();
            if (parms.Length > 0)
            {
                propObj = property.GetValue(obj, new object[1] { 0 });
            }
            else
            {
                propObj = property.GetValue(obj, null);
            }
            return propObj;
        }


        /******************************************************************************
        * Function:          VerifyFreezable
        ******************************************************************************/
        /// <summary>
        /// Retrieve a property value.
        /// </summary>
        /// <param name='obj'>The object possessing the requested property</param>
        /// <returns></returns>
        private void VerifyFreezable(object obj)
        {
            if (_objectsInTree.Contains(obj.GetHashCode()))
            {
                return;
            }
            if (TypeHelper.IsFreezable(obj.GetType()))
            {
                Freezable freezable = obj as Freezable;
                //Console.WriteLine("Freezable Object: " + obj.ToString());
                // If a freezable is frozen
                // freezable children must also be frozen 
                if (freezable.IsFrozen)
                {
                    VerifyFreezableIsFrozen(freezable);
                }
            }

            _objectsInTree.Add(obj.GetHashCode());

            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in pi)
            {
            //  Console.WriteLine("PropertyName: " + property.Name);
                if (IsBadProperty(property.Name, obj))
                {
                    continue;
                }
                if (IsGenericTypeMember(property.GetType(), property.Name))
                {
                    continue;
                }

                if (property.PropertyType.ToString().Contains("Collection"))
                {
                    object children = property.GetValue(obj, null);
                    foreach (object child in (IEnumerable)children)
                    {
                        VerifyFreezable(child);
                    }
                }
                else
                {
                    object propObj = GetPropertyValue(obj, property);
                    if (propObj != null)
                    {
                        VerifyFreezable(propObj);
                    }
                }
            }
        }


        /******************************************************************************
        * Function:          VerifyFreezableIsFrozen
        ******************************************************************************/
        /// <summary>
        /// Retrieve a property value.
        /// </summary>
        /// <param name='freezable'>The freezable object to be verified</param>
        /// <returns></returns>
        private void VerifyFreezableIsFrozen(Freezable freezable)
        {
            //Console.WriteLine("VerifyFreezableIsFrozen for : " + freezable);
            // 
            if (!freezable.IsFrozen && freezable.GetType().ToString() != "System.Windows.Media.MatrixTransform")
            {
                _result.passed = false;
                _result.failures.Add("Test action: " + _action + " - " + freezable.GetType().ToString() + " Expected IsFrozen=True");
                GlobalLog.LogStatus("Test action: " + _action + " - " + freezable.GetType().ToString() + " Expected IsFrozen=True");

            }
            Type t = freezable.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < pi.Length; i++)
            {
                if (IsBadProperty(pi[i].Name, freezable))
                {
                    continue;
                }
                if (!TypeHelper.IsFreezable(pi[i].PropertyType))
                {
                    // no need to verify
                    continue;
                }
                if (pi[i].Name == "Item" || pi[i].Name.ToString().EndsWith(".Item"))
                {
                    if (freezable is IEnumerable)
                    {
                        // This means that the current Freezable is a collection
                        foreach (System.Windows.Freezable f in (IEnumerable)freezable)
                        {
                            VerifyFreezableIsFrozen(f);
                        }
                    }
                }
                else
                {
                    System.Windows.Freezable f = null;
                    f = (System.Windows.Freezable)pi[i].GetValue(freezable, null);
                    VerifyFreezableIsFrozen(f);
                }
            }
        }


        /******************************************************************************
        * Function:          IsGenericTypeMember
        ******************************************************************************/
        /// <summary>
        /// Checks if the given type member is a generic-only member on a non-generic type.
        /// </summary>
        /// <param name='type'>The freezable object to be verified</param>
        /// <param name='memberName'>Member name</param>
        /// <returns>A boolean indicating whether or not the type is generic </returns>
        private bool IsGenericTypeMember(Type type, string memberName)
        {
            return !type.IsGenericType
                    && (memberName == "GenericParameterPosition"
                    || memberName == "GenericParameterAttributes"
                    || memberName == "GetGenericArguments"
                    || memberName == "GetGenericParameterConstraints"
                    || memberName == "GetGenericTypeDefinition"
                    || memberName == "IsGenericTypeDefinition"
                    || memberName == "DeclaringMethod");
        }


        /******************************************************************************
        * Function:          IsBadProperty
        ******************************************************************************/
        /// <summary>
        /// Checks for to-be-skipped properties / objects.
        /// </summary>
        /// <param name='propertyName'>The property to be checked</param>
        /// <param name='owner'>The object associated with the to-be-checked property</param>
        /// <returns>A boolean indicating whether or not the property is acceptable</returns>
        private bool IsBadProperty(string propertyName, object owner)
        {
            //GeneralTransform.Inverse property returns a GeneralTranform again, stuck in the loop
            if (propertyName == "Inverse" && owner.GetType().ToString() == "GeneralTransform")
            {
                return true;
            }
            // stuck in the loop
            else if (owner.GetType().ToString() == "System.Windows.Media.MatrixTransform")
            {
                return true;
            }
            // Get TargetInvocationException for some reason.
            else if (owner.GetType().ToString() == "System.String" && propertyName == "Chars")
            {
                return true;
            }
            else if (owner.GetType().ToString() == "System.Globalization.CultureInfo")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Returns a Pass/Fail result for the test case.
        /// </summary>
        /// <returns>A TestResult, indicating whether or not the test passed.</returns>
        private TestResult Verify()
        {
            // report the failures all together
            if (!_result.passed)
            {
                GlobalLog.LogEvidence("------------------------------------------");
                GlobalLog.LogEvidence("FAILURE REPORT");
                GlobalLog.LogEvidence("------------------------------------------");
                for (int i = 0; i < _result.failures.Count; i++)
                {
                    GlobalLog.LogStatus(_result.failures[i]);
                }
            }

            if (_result.passed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion
    }
}
