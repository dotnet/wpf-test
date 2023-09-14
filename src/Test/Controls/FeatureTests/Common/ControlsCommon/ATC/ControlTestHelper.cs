//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// All classes in this file are some helper ones, which do some common work 
// for testing
//---------------------------------------------------------------------------

#region Using directives

using System;
using System.Text;
using System.Windows.Controls;
using System.Xml;
using Microsoft.Test.Logging;
using System.Collections;
using System.Windows;
using Avalon.Test.ComponentModel.Utilities;
using System.Windows.Media;
using Avalon.Test.ComponentModel.UnitTests;
using System.Reflection;
using System.Windows.Data;
using Avalon.Test.ComponentModel.Validations;
using System.Windows.Markup;
using System.Drawing;
using Microsoft.Test.RenderingVerification;
using Avalon.Test.ComponentModel.Actions;
using System.Collections.Generic;
using System.ComponentModel;
//using AutoData;
using System.Text.RegularExpressions;
using Microsoft.Test.Input;

#endregion

namespace Avalon.Test.ComponentModel.Utilities
{
    /// <summary>
    /// Helper class to Run Actions and Validations
    /// </summary>
    public static class ControlTestHelper
    {
        /// <summary>
        /// Execute Actions. All the Actions and their parameters can be parsed and executed one by one.
        /// and it assume the first parameter is the control Name
        /// </summary>
        /// <param name="elelment"></param>
        /// <param name="frmElement"></param> 
        /// Parse and Execute actions one by one.
        public static void ExecuteActions(FrameworkElement frmElement, XmlElement elelment)
        {
            if (frmElement == null)
                throw new ArgumentNullException("frmElement");
            if (elelment == null)
                throw new ArgumentNullException("elelment");

            XtcTestHelper.DoActions(frmElement, elelment["Actions"]);
            QueueHelper.WaitTillQueueItemsProcessed();
        }



        /// <summary>
        /// Run all the Validations described in elelment,frmElement is the Control that will be tested.
        /// Sometime it can be the searching root in the logical tree,what it shoud be depends on conditions.
        /// in this function,Validations and their parameters described in xtc can be parsed  
        /// and excuted one by one.
        /// </summary>
        /// <param name="elelment"></param>
        /// <param name="frmElement"></param>
        /// <returns></returns>
        public static bool ExecuteValidations(FrameworkElement frmElement, XmlElement elelment)
        {
            string failedValidation = XtcTestHelper.DoValidations(frmElement, elelment);
            if (string.IsNullOrEmpty(failedValidation))
            {
                return true;
            }
            else
            {
                GlobalLog.LogEvidence(" Fail in : " + failedValidation);
                return false;
            }
        }



        /// <summary>
        /// Get DependencyProperty its name and owner type by reflection.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static DependencyProperty DependencyPropertyFromName(string propertyName, Type propertyType)
        {
            FieldInfo fi = propertyType.GetField(propertyName + "Property", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return (fi != null) ? fi.GetValue(null) as DependencyProperty : null;
        }



        /// <summary>
        /// Find visual part in a Visual by its type.
        /// </summary>
        /// <param name="logicalParent"></param>
        /// <param name="controlName"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static FrameworkElement FindIndexPartByType(Visual logicalParent, string controlName, Type type, int index)
        {
            Visual tempResult = (Visual)LogicalTreeHelper.FindLogicalNode(logicalParent, controlName);
            Assert.AssertTrue("Can not find Viual by its name " + controlName, tempResult != null);
            return VisualTreeUtils.FindPartByType(tempResult, type, index) as FrameworkElement;
        }



        /// <summary>
        /// Find visual part in a Visual by its type.
        /// </summary>
        /// <param name="logicalParent"></param>
        /// <param name="controlName"></param>
        /// <param name="xmlNamespace"></param>
        /// <param name="typeName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static FrameworkElement FindIndexPartByNamespace(Visual logicalParent, string controlName, string xmlNamespace, string typeName, int index)
        {
            GlobalLog.LogStatus("FindIndexPartByNamespace");
            Type type = XamlTypeMapper.DefaultMapper.GetType(xmlNamespace, typeName);

            if (type == null)
            {
                if (xmlNamespace != null)
                {
                    string[] strs = xmlNamespace.Split(';');
                    string clr_namespace, assembly;
                    if (strs != null && strs.Length == 2)
                    {
                        clr_namespace = strs[0].Split(':')[1];
                        //GlobalLog.LogStatus("clr-namespace : " + clr_namespace);
                        assembly = strs[1].Split('=')[1];
                        //GlobalLog.LogStatus("assembly : " + assembly);
                        Assembly asm = Assembly.Load(assembly);
                        //GlobalLog.LogStatus("assembly : " + asm);
                        if (asm != null)
                        {
                            //GlobalLog.LogStatus(clr_namespace + "." + typeName);
                            type = asm.GetType(clr_namespace + "." + typeName);
                            //GlobalLog.LogStatus("type name : " + type);
                        }
                    }
                }
            }
            Assert.AssertTrue("Can not find type " + typeName + " in namespace " + xmlNamespace, type != null);
            return FindIndexPartByType(logicalParent, controlName, type, index);
        }



        /// <summary>
        /// Find visual parts in a Visual by its type.
        /// </summary>
        /// <param name="logicalParent"></param>
        /// <param name="controlName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ArrayList FindPartsByType(Visual logicalParent, string controlName, Type type)
        {
            Visual tempResult = (Visual)LogicalTreeHelper.FindLogicalNode(logicalParent, controlName);
            Assert.AssertTrue("Can not find Viual by its name " + controlName, tempResult != null);
            return VisualTreeUtils.FindPartByType(tempResult, type);

        }



        /// <summary>
        /// Find visual parts in a Visual by its type.
        /// </summary>
        /// <param name="logicalParent"></param>
        /// <param name="controlName"></param>
        /// <param name="xmlNamespace"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static ArrayList FindPartsByNamespace(Visual logicalParent, string controlName, string xmlNamespace, string typeName)
        {
            Type type = XamlTypeMapper.DefaultMapper.GetType(xmlNamespace, typeName);
            Assert.AssertTrue("Can not find type " + typeName + " in namespace " + xmlNamespace, type != null);
            return FindPartsByType(logicalParent, controlName, type);
        }



        /// <summary>
        /// Check whether one FrameworkElement is contained in another one visually
        /// </summary>
        /// <param name="container"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static bool ContainFramworkElement(FrameworkElement container, FrameworkElement child)
        {
            Assert.AssertTrue("null FrameworkElement reference!", container != null && child != null);
            Rectangle rec = ImageUtility.GetScreenBoundingRectangle(child);
            return ImageUtility.GetScreenBoundingRectangle(container).Contains(rec);
        }



        /// <!--summary>
        /// Set property on DependencyObject.  
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param-->
        public static void SetPropertyOntoDependencyObject(DependencyObject dependencyObject, FrameworkElement resourceOwner, string propertyName, string value)
        {
            Type type = dependencyObject.GetType();
            DependencyProperty dp = ControlTestHelper.DependencyPropertyFromName(propertyName, type);
            PropertyInfo pInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Assert.AssertTrue("Property " + propertyName + " does not exist  !", pInfo != null);

            object obj = null;
            switch (value)
            {
                case "null":
                    pInfo.SetValue(dependencyObject, null, new object[0]);
                    //dependencyObject.SetValue(ControlTestHelper.DependencyPropertyFromName(propertyName, type), null);
                    break;
                case "Clear":
                    dependencyObject.ClearValue(ControlTestHelper.DependencyPropertyFromName(propertyName, type));
                    break;
                default:
                    if (propertyName.EndsWith("TemplateSelector"))
                    {
                        DataTemplate v = resourceOwner.FindResource(value) as DataTemplate;
                        obj = new ControlSetTemplateSelector.TemplateSelectorFactory(v);
                    }
                    else if (propertyName.EndsWith("Template"))
                    {
                        obj = resourceOwner.FindResource(value);
                    }
                    else if (propertyName.EndsWith("Style"))
                    {
                        obj = resourceOwner.FindResource(value);
                    }
                    else if (propertyName.EndsWith("StyleSelector"))
                    {
                        Style v = resourceOwner.FindResource(value) as Style;
                        obj = new ControlSetStyleSelector.StyleSelectorFactory(v);
                    }
                    else if (propertyName.EndsWith("Binding"))
                    {
                        obj = ListViewHelper.CreateBinding(value);
                    }

                    if (obj != null) //if obj != null, the propertyName must be one of the five metioned above.
                    {
                        //dependencyObject.SetValue(ControlTestHelper.DependencyPropertyFromName(propertyName, type), obj);
                        pInfo.SetValue(dependencyObject, obj, new object[0]);
                    }
                    else
                    {
                        if (dp.PropertyType == typeof(object))
                            pInfo.SetValue(dependencyObject, value, new object[0]);
                        //dependencyObject.SetValue(dp, value);
                        else
                            //dependencyObject.SetValue(dp, PMEUtils.Current.StringToType(value, dp.PropertyType));    
                            pInfo.SetValue(dependencyObject, PMEUtils.Current.StringToType(value, dp.PropertyType), new object[0]);
                    }
                    break;
            }
            QueueHelper.WaitTillQueueItemsProcessed();
        }



        /// <summary>
        /// Set property on an object
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="resourceOwner"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetPropertyOntoObject(object targetObj, FrameworkElement resourceOwner, string propertyName, string value)
        {
            string[] path = propertyName.Split('.');
            object obj = ControlTestHelper.GetObjectByDotPath(targetObj, path);
            string name = path[path.Length - 1];

            Type type = obj.GetType();
            bool isDp = (obj is DependencyObject) && (ControlTestHelper.DependencyPropertyFromName(name, type) != null);
            if (isDp)
            {
                SetPropertyOntoDependencyObject(obj as DependencyObject, resourceOwner, name, value);
            }
            else
            {
                PropertyInfo pInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
                Assert.AssertTrue("Property " + name + " does not exist  !", pInfo != null);
                switch (value)
                {
                    case "null":
                        pInfo.SetValue(obj, null, new object[0]);
                        break;
                    case "Clear":
                        pInfo.SetValue(obj, null, new object[0]);
                        break;
                    default:
                        if (name.EndsWith("Binding"))
                        {
                            pInfo.SetValue(obj, ListViewHelper.CreateBinding(value), new object[0]);
                        }
                        else
                        {
                            if (pInfo.PropertyType == typeof(object))
                                pInfo.SetValue(obj, value, new object[0]);
                            else
                                pInfo.SetValue(obj, PMEUtils.Current.StringToType(value, pInfo.PropertyType), new object[0]);
                        }
                        break;
                }
            }
            QueueHelper.WaitTillQueueItemsProcessed();
        }



        /// <summary>
        /// Get object according to a path by reflection. For example, we can get GridViewColumn from ListView
        /// by use {View.Columns.[0]}
        /// </summary>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object GetObjectByDotPath(object source, string[] path)
        {
            object obj = source;
            int length = path.Length;
            int i = 0;
            while (i < length - 1)
            {
                obj = GetObjectByPropertyName(obj, path[i]);
                Assert.AssertTrue(" Null reference", obj != null);
                i++;
            }
            return obj;
        }


        /// <summary>
        /// Get object by propertyName.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        public static object GetObjectByPropertyName(object source, string propertyPath)
        {
            object result = null;

            if (propertyPath.StartsWith("[") && propertyPath.EndsWith("]"))
            {
                string tempPath = propertyPath.Substring(1, propertyPath.Length - 2);
                PropertyInfo pi = source.GetType().GetProperty("Item");
                Assert.AssertTrue("No indexer found!", pi != null);

                //only single parameter is supported.
                ParameterInfo[] paraInfo = pi.GetIndexParameters();
                Assert.AssertTrue("Unexpected ParameterInfo[]!", paraInfo != null && paraInfo.Length > 0);

                object paraObj = XmlHelper.ConvertToType(paraInfo[0].ParameterType, tempPath);
                result = pi.GetValue(source, new object[] { paraObj });
            }
            else
            {
                PropertyInfo pInfo = source.GetType().GetProperty(propertyPath, BindingFlags.Instance | BindingFlags.Public);
                Assert.AssertTrue(" Can not find property " + propertyPath + " in type " + source.GetType().ToString(), pInfo != null);
                result = pInfo.GetValue(source, new object[0]);
            }
            return result;
        }



        /// <!--summary>
        /// Get object according to a path by reflection. For example, we can get GridViewColumn from ListView
        /// by use {View.Columns[0]}
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyPath"></param>
        /// <param name="notIncludeLast"></param>
        /// <returns></returns-->
        public static object GetObjectByPath(object source, string propertyPath, bool includeLast)
        {
            string[] path = FromPathToDotPath(propertyPath);
            object obj = ControlTestHelper.GetObjectByDotPath(source, path);

            if (includeLast)
            {
                string lastProperty = path[path.Length - 1];
                obj = GetObjectByPropertyName(obj, lastProperty);
            }
            return obj;
        }


        /// <summary>
        /// Parse a string to a array of string
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        public static string[] FromPathToDotPath(string propertyPath)
        {
            return Regex.Replace(propertyPath, @"([^\.])\[", "${1}.[").Split('.');
        }



        /// <summary>
        /// Find a logical child by its name. If not found, an exception will be thrown.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static FrameworkElement GetLogicalChild(FrameworkElement frmElement, string name)
        {
            FrameworkElement frameworkElement = LogicalTreeHelper.FindLogicalNode(frmElement, name) as FrameworkElement;
            Assert.AssertTrue(" Can not find control " + name, frameworkElement != null);
            return frameworkElement;
        }



        /// <summary>
        /// Find a visual child by its name. If not found, an exception will be thrown.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static FrameworkElement GetVisualChild(FrameworkElement frmElement, string name)
        {
            FrameworkElement result = VisualTreeUtils.FindPartByName(frmElement, name) as FrameworkElement;
            Assert.AssertTrue(" Can not find control " + name, frmElement != null);
            return result;
        }


        /// <summary>
        /// Verify value of property, dotted path is supported.
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="propertyPath"></param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public static bool VerifyObjectProperty(object targetObj, string propertyPath, object expectedValue)
        {
            string[] path = propertyPath.Split('.');
            object obj = ControlTestHelper.GetObjectByPath(targetObj, propertyPath, true);
            GlobalLog.LogStatus(propertyPath + ", ActualValue: [" + obj + "]");
            GlobalLog.LogStatus(propertyPath + ", ExpectedValue: [" + expectedValue + "]");
            if (obj == null)
                return expectedValue == null;

            if (ObjectFactory.CompareObjects(obj, expectedValue))
                return true;

            Type objType = obj.GetType();
            object valueToCompare = XmlHelper.ConvertToType(objType, expectedValue);
            if (ObjectFactory.CompareObjects(obj, valueToCompare))
                return true;

            //the last resort to verify they are equivalent.
            if (valueToCompare != null)
                return obj.ToString() == valueToCompare.ToString();
            else if (expectedValue != null)
                return obj.ToString() == expectedValue.ToString();

            return false;
        }
    }
}


namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// Run UnitTest with Exception handling.
    /// </summary>
    public class ControlGenericUnitTest : IUnitTest
    {
        /// <summary>
        /// Invoke ControlTestActionValidationUnitTest to run UnitTest.
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            return UnitTestHelper.RunTestVariation(testElement as FrameworkElement, variation);
        }
    }



    /// <summary>
    /// Those cases that test event should implement EventTestable to realize test logic.
    /// For event test logic is diverse, no uniform logic is suitable. so, a template is provided to 
    /// simplify event test.
    /// </summary>
    public interface EventTestable
    {
        //Set up enviroment to test event
        void Setup(object testElement, XmlElement variation);
        //Return test result.
        TestResult Validate();
        //clean up enviroment.
        void Clear();
    }



    /// <!--summary>
    /// Class to test events for Controls.
    /// This class provides a template for event test. Those test event should implement EventTestable to realize test logic.
    /// to make this work, a EventTestable node should be put in xtc.
    /// a xtc example:
    /// <INIT Class="ControlGenericUnitTest" SuppressGtoLogger="true"/>
    /// <VARIATION ID="1">
    /// <CONTROL>
    /// </CONTROL>
    /// <EventTestable Value="ListViewColumnListChangeArgs" SourceControlName="listView"/>
    /// <Actions>
    /// </Validations>
    /// </summary-->
    class ControlEventArgsUnitTest : IUnitTest
    {
        public TestResult Perform(object testElement, XmlElement variation)
        {
            FrameworkElement element = testElement as FrameworkElement;

            if (element == null)
                throw new NullReferenceException(" no instance of FrameworkElement was found ");

            string typeName = variation["EventTestable"].GetAttribute("Value");

            // Create EventTestable.
            EventTestable testable = ObjectFactory.CreateObjectFromTypeName(typeName) as EventTestable;
            testable.Setup(element, variation);
            IDataDrivenTest helper = new DataDirvenTestWithExceptionHandling() as IDataDrivenTest;

            //To prevent exception from one test case stopping all cases after it. Catch all Exception. it is a bad way. Since test Framework does not 
            //provide solution, this workaround is used here.

            if (helper.RunActions(element, variation) == TestResult.Fail)
                return TestResult.Fail;
            testable.Clear();

            QueueHelper.WaitTillQueueItemsProcessed();
            if (testable.Validate() == TestResult.Fail)
                return TestResult.Fail;

            return helper.RunValidations(element, variation);
        }
    }



    /// <summary>
    /// An interface, any class implements it indicate a manner to run all actions and validations.
    /// </summary>
    internal interface IDataDrivenTest
    {
        TestResult RunActions(FrameworkElement frmElement, XmlElement elelment);
        TestResult RunValidations(FrameworkElement frmElement, XmlElement elelment);
    }



    /// <summary>
    ///  Run all Actions with Exception handling
    /// </summary>
    internal class DataDirvenTestWithExceptionHandling : IDataDrivenTest
    {
        public TestResult RunActions(FrameworkElement frmElement, XmlElement elelment)
        {
            try
            {
                ControlTestHelper.ExecuteActions(frmElement as FrameworkElement, elelment);
            }
            catch (Exception e)
            {
                GlobalLog.LogEvidence(e.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }


        /// <summary>
        /// Run all Validations with Exception handling
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="elelment"></param>
        /// <returns></returns>
        public TestResult RunValidations(FrameworkElement frmElement, XmlElement elelment)
        {
            try
            {
                if (!ControlTestHelper.ExecuteValidations(frmElement as FrameworkElement, elelment["Validations"]))
                {
                    return TestResult.Fail;
                }
            }
            catch (Exception e)
            {
                GlobalLog.LogEvidence(e.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
    }



    /// <summary>
    /// A helper class to run all actions and validations
    /// </summary>
    internal class UnitTestHelper
    {
        internal static TestResult RunTestVariation(FrameworkElement testElement, XmlElement variation)
        {
            IDataDrivenTest helper = new DataDirvenTestWithExceptionHandling() as IDataDrivenTest;
            if (helper.RunActions(testElement, variation) == TestResult.Fail)
                return TestResult.Fail;

            return helper.RunValidations(testElement, variation);
        }
    }
}



namespace Avalon.Test.ComponentModel.IntegrationTests
{
    /// <summary>
    ///  class to Run IntegrationTests test cases;
    /// </summary>
    public class ControlIntegrationTest : IIntegrationTest
    {
        /// <summary>
        /// Run IntegrationTests cases
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>br
        public TestResult Perform(object testElement, XmlElement variation)
        {
            return UnitTestHelper.RunTestVariation(testElement as FrameworkElement, variation);
        }
    }
}



namespace Avalon.Test.ComponentModel.Validations
{
    /// <summary>
    /// Class to Verify a ItemsControl is showing sorted items. A simple example is like:
    /// <Validation Name="ItemsControlSortingValidation" >
    /// <Parameter Value="listBox" />
    /// <Parameter Value="http://schemas.microsoft.com/winfx/2006/xaml/presentation" />
    /// <Parameter Value="ListBoxItem"/>
    /// <Parameter Value="Content" />
    /// <Parameter Value="1" />
    /// </Validation>
    /// </summary>
    public class CustomListViewSortingValidation : IValidation
    {
        /// <summary>
        /// Verify a ItemsControl is showing sorted items.
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            string[] args = validationParams[1] as string[];
            Assert.AssertTrue("Not enough parameters in ListViewSortingValidation", args != null && args.Length != 2);

            string controlName = args[0];
            string xmlNamespace = args[1];
            string typeName = args[2];
            string propertyName = args[3];
            int direction = XmlHelper.Convert<int, string>(args[4]);

            FrameworkElement element = ControlTestHelper.GetLogicalChild((FrameworkElement)validationParams[0], controlName);
            Type type = XamlTypeMapper.DefaultMapper.GetType(xmlNamespace, typeName);

            IList list = GetValueList(element, type, propertyName);
            return VerifyListSorting(list, direction);
        }

        /// <summary>
        /// //Get values from a set of visual children. For example, get every ListBoxItem.Content from all the ListBoxItem in a ListBox.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private IList GetValueList(FrameworkElement element, Type type, string propertyName)
        {
            ArrayList results = new ArrayList();
            ArrayList list = VisualTreeUtils.FindPartByType(element, type);

            foreach (FrameworkElement targetElement in list)
            {
                results.Add(ControlTestHelper.GetObjectByPath(targetElement, propertyName, true));
            }

            return results;
        }


        /// <summary>
        /// Verify all the items in an IList are sorted ----ending or Descending.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="expectedResult"></param>
        /// <returns></returns>
        private bool VerifyListSorting(IList list, int expectedResult)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (!CompareValue(list[i], list[i + 1], expectedResult))
                    return false;
            }
            return true;
        }


        /// <!--summary>
        /// Compare two object, if firstObj less than secondObj, return -1, if equal, return 0, if greater than, return 1.
        /// </summary>
        /// <param name="firstObj"></param>
        /// <param name="secondObj"></param>
        /// <param name="comparer"></param>
        /// <param name="expectedResult"></param>
        /// <returns></returns-->
        private bool CompareValue(object firstObj, object secondObj, int expectedResult)
        {
            //ensure two object are of same type.
            if (firstObj.GetType() != secondObj.GetType())
                return false;

            IComparable comparable1 = firstObj as IComparable;
            if (firstObj != null)
                return (comparable1.CompareTo(secondObj) == expectedResult);

            return false;
        }
    }



    /// <summary>
    /// Class to validate a special property of some Visial part in Control.
    /// </summary>
    ///          An XTC Example
    /// <Validation Name="ControlVisualPartPropertyValidation" >
    ///   <Parameter Value="FrameworkElementName" />
    ///   <Parameter Value="PropertyName" />
    ///   <Parameter Value="VisualPartName" />
    ///   <Parameter Value="ExpectedValue" />
    /// </Validation>
    public class ControlVisualPartPropertyValidation : IValidation
    {

        /// <summary>
        /// validate the visibility 
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            string[] args = (string[])validationParams[1];
            FrameworkElement logicalChild = ControlTestHelper.GetLogicalChild((FrameworkElement)validationParams[0], (string)args[0]);

            object obj = ControlTestHelper.GetVisualChild(logicalChild, (string)args[1]);
            string propertyName = args[2];
            string expectedValue = args[3];

            return ControlTestHelper.VerifyObjectProperty(obj, propertyName, expectedValue);
        }
    }



    /// <summary>
    /// Validate one control is contained in another control. If the child is covered by the other control, return false.
    /// </summary>
    public class ControlContainedValidation : IValidation
    {
        /// <summary>
        /// validate the relationship 
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            string[] args = validationParams[1] as string[];
            Assert.AssertTrue("Not enough parameters", args != null && args.Length == 4);

            string controlName = args[0];
            string xmlNamespace = args[1];
            string typeName = args[2];
            int index = XmlHelper.Convert<int, string>(args[3]);

            Visual tempResult = (Visual)ControlTestHelper.GetLogicalChild((FrameworkElement)validationParams[0], controlName); // Search Control
            FrameworkElement child = ControlTestHelper.FindIndexPartByNamespace(tempResult, controlName, xmlNamespace, typeName, index) as FrameworkElement;
            Assert.AssertTrue("Null child", child != null);
            GlobalLog.LogStatus("Child : " + child.ToString());

            return ControlTestHelper.ContainFramworkElement((FrameworkElement)tempResult, child);
        }
    }



    /// <summary>
    /// A base that provide some common funciton to verify exception.
    /// </summary>
    public abstract class ControlExceptionValidationBase
    {
        /// <summary>
        /// Verify exception is thrown.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="name"></param>
        /// <param name="paras"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected bool ValidateException(FrameworkElement frmElement, string name, object paras, string typeName)
        {
            try
            {
                DoAction(frmElement, name, paras);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                {
                    e = ((TargetInvocationException)e).InnerException;
                }
                if (e.GetType().Equals(Type.GetType(typeName)))
                {
                    return true;
                }
                else
                {
                    GlobalLog.LogStatus("Expected Exception: " + typeName);
                    GlobalLog.LogStatus("Actual Exception: " + e.GetType().ToString());
                    GlobalLog.LogEvidence("Exception was thrown: " + e.GetType().Name + "\r\n " + e.Message + e.StackTrace);
                    return false;
                }
            }
            GlobalLog.LogEvidence("ControlPropertyExceptionValidation: : not throw exception!");
            return false;
        }

        protected abstract void DoAction(FrameworkElement frmElement, string name, object paras);
    }



    /// <!--summary>
    /// P1 class to validate that an Exception will be thrown by run a special Action
    /// </summary>
    /// the xtc descritpion should be 
    /// <Validation Name="ControlActionExceptionValidation" >
    ///      <Parameter Value="ActionName" />
    ///      <Paremeter Value="mcc" />
    ///      <Parameter Value="other....." />
    /// <Validation-->
    public class ControlActionExceptionValidation : ControlExceptionValidationBase, IValidation
    {
        /// <summary>
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            string[] args = (string[])validationParams[1];
            string[] paras = new string[args.Length - 2];
            string actionName = args[0];
            string typeName = args[1];
            for (int i = 2; i < args.Length; i++)
            {
                paras[i - 2] = args[i];
            }

            return base.ValidateException((FrameworkElement)validationParams[0], actionName, paras, typeName);
        }

        /// <summary>
        /// a callback function to do something.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="name"></param>
        /// <param name="paras"></param>
        protected override void DoAction(FrameworkElement frmElement, string name, object paras)
        {
            IAction action = ObjectFactory.CreateObjectFromTypeName(name) as IAction;
            action.Do(frmElement, (string[])paras);
        }
    }



    /// <summary>
    /// Class to verify the number of items in a Container. A xtc example:
    /// <Validation Name="ControlItemsCountValidation" >
    /// <Parameter Value="ControlName" />
    ///  <Parameter Value="XmlNamespace" />
    /// <Parameter Value="TypeName" />
    /// <Parameter Value="ExpectedValue" />
    /// </Validation>
    /// </summary>
    public class ControlItemsCountValidation : IValidation
    {
        /// <summary>
        /// Search Visual Tree to find out how many items according to type name.This tpye must be in the same Assembly with FrameworkElement.
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            string[] args = validationParams[1] as string[];
            if (args == null || args.Length != 4)
            {
                throw new ArgumentException("Not enough parameters");
            }
            string controlName = args[0];
            string xmlNampspace = args[1];
            string typeName = args[2];
            int expectedValue = XmlHelper.Convert<int, string>(args[3]);

            ArrayList list = ControlTestHelper.FindPartsByNamespace((Visual)validationParams[0], controlName, xmlNampspace, typeName);

            GlobalLog.LogStatus("Exepcted Count: " + expectedValue);
            GlobalLog.LogStatus("Actual Count: " + list.Count);
            return list.Count == expectedValue;
        }
    }



    /// <summary>
    /// Class to verify property of a index visual part in a Control. A xtc sample:
    ///<Validation Name="ControlIndexPartPropertyValidation" >
    ///<Parameter Value="listView" />
    ///<Parameter Value="http://schemas.microsoft.com/winfx/2006/xaml/presentation" />
    ///<Parameter Value="ListViewItem" />
    ///<Parameter Value="3" />
    ///<Parameter Value="Background" />
    ///<Parameter Value="#FF316AC5" />
    ///</Validation >
    /// </summary>
    public class ControlIndexPartPropertyValidation : IValidation
    {
        /// <summary>
        /// Search the index Visual part and verify its property
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            string[] args = validationParams[1] as string[];
            if (args == null || args.Length != 6)
            {
                throw new ArgumentException("Not enough parameters");
            }
            string controlName = args[0];
            string xmlNamespace = args[1];
            string typeName = args[2];
            int index = XmlHelper.Convert<int, string>(args[3]);
            string propertyName = args[4];
            string expectedValue = args[5];

            Visual tempResult = (Visual)ControlTestHelper.GetLogicalChild((FrameworkElement)validationParams[0], controlName); // Search Control
            FrameworkElement indexPart = ControlTestHelper.FindIndexPartByNamespace(tempResult, controlName, xmlNamespace, typeName, index); //Search visual part by its index.
            Assert.AssertTrue("No visual part was found!", indexPart != null);

            return ControlTestHelper.VerifyObjectProperty(indexPart, propertyName, expectedValue);
        }


    }



    /// <summary>
    /// Class to verify property on Control. Complalicated path is supported. for example. View.Columns.[0].Header.
    /// A simple xtc example is like below:
    /// <Validation Name="ControlPropertyValueValidation2" >
    ///   <ControlName Value="listView" />
    ///   <PropertyPath Value="View.Columns.[0].HeaderTemplate" />
    ///   <IsReverse Value="System.Windows.DataTemplate" />
    ///   </Validation>
    /// </summary>
    public class ControlPropertyValueValidation : IValidation
    {
        /// <summary>
        /// Parse the Path to get the object, then verify its value.
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            string[] args = (string[])validationParams[1];
            Assert.AssertTrue("Not enough parameters in ControlSetPropertyAction  ", args != null && args.Length == 3);
            string propertyPath = args[1];
            string value = args[2];

            FrameworkElement frameworkElement = ControlTestHelper.GetLogicalChild((FrameworkElement)validationParams[0], args[0]) as FrameworkElement;
            return ControlTestHelper.VerifyObjectProperty(frameworkElement, propertyPath, value);
        }
    }



    /// <summary>
    /// Interface to enable test event.
    /// </summary>
    public interface IEventHandler : IValidation
    {
        void OnEvent(object sender, EventArgs args);
    }



    /// <summary>
    /// A generic class to test event, user can do some actions during event.
    /// </summary>
    public class ControlChangeStatusDuringEvent : ControlEventTestBase, IEventHandler
    {
        public void OnEvent(object sender, EventArgs args)
        {
            ControlTestHelper.ExecuteActions(TestFrameworkElemet, EventTestableXmlElement["Actions"]);
        }


        public bool Validate(params object[] validationParams)
        {
            return true;
        }


        public sealed override IEventHandler ValidationObject
        {
            get { return this; }
        }
    }



    /// <summary>
    /// A generic class to test event, during event handling, user can verify argument and control itself. Continous event can be verified.
    /// </summary>
    public class ControlEventArgsValidation : ControlEventTestBase, IEventHandler
    {
        private List<bool> results = new List<bool>();
        private int index = 0;

        public virtual void OnEvent(object sender, EventArgs args)
        {

            Assert.AssertTrue("More event than expected!", EventTestableXmlElement["Validations"].ChildNodes.Count >= index + 1);
            XmlElement validationElement = EventTestableXmlElement["Validations"].ChildNodes[index++] as XmlElement;

            string targetType = validationElement.GetAttribute("TargetType");
            string validationsResult = string.Empty;
            if ("FrameworkElement".Equals(targetType))
                validationsResult = XtcTestHelper.DoValidations(TestFrameworkElemet, validationElement);
            else
                validationsResult = XtcTestHelper.DoValidations(args, validationElement);
            results.Add(string.IsNullOrEmpty(validationsResult));
        }


        public bool Validate(params object[] validationParams)
        {
            foreach (bool result in results)
            {
                if (!result)
                    return false;
            }

            //Make sure event count is correct. 
            if (results.Count != EventTestableXmlElement["Validations"].ChildNodes.Count)
            {
                GlobalLog.LogEvidence("Event count is wrong!");
                GlobalLog.LogEvidence("Exptected count is: " + EventTestableXmlElement["Validations"].ChildNodes.Count.ToString());
                GlobalLog.LogEvidence("Actual count is: " + results.Count.ToString());
                return false;
            }
            return true;
        }


        public sealed override IEventHandler ValidationObject
        {
            get { return this; }
        }
    }



    /// <summary>
    /// class to test fired event count.
    /// </summary>
    public class ControlEventCountTest : ControlEventTestBase, IEventHandler
    {
        private int _eventCount = 0;
        public void OnEvent(object sender, EventArgs args)
        {
            ++_eventCount;
        }


        public bool Validate(params object[] validationParams)
        {
            int expectedCount = XmlHelper.Convert<int, string>(EventTestableXmlElement["Validations"]["Count"].GetAttribute("Value"));
            GlobalLog.LogEvidence("Expected count is: " + expectedCount);
            GlobalLog.LogEvidence("Actual count is: " + _eventCount);
            return _eventCount == expectedCount;
        }


        public sealed override IEventHandler ValidationObject
        {
            get { return this; }
        }
    }



    /// <summary>
    /// skeleton class to support event test.
    /// </summary>
    public abstract class ControlEventTestBase : EventTestable
    {
        EventInfo eventInfo = null;
        UIElement uiElement = null;
        RoutedEvent routedEvent = null;
        Delegate eventDel = null;
        XmlElement eventTestableXmlElement = null;
        FrameworkElement testObject = null;

        public abstract IEventHandler ValidationObject
        {
            get;
        }

        public XmlElement EventTestableXmlElement
        {
            set { eventTestableXmlElement = value; }
            get { return eventTestableXmlElement; }
        }

        public FrameworkElement TestFrameworkElemet
        {
            get { return testObject; }
        }

        //clear enviroment
        public void Clear()
        {
            if (uiElement != null)
                uiElement.RemoveHandler(routedEvent, eventDel);
            else
            {
                if (eventInfo != null)
                    eventInfo.RemoveEventHandler(ValidationObject, eventDel);
            }
        }


        /// <summary>
        /// set up enviroment,RoutedEvent are Clr event are differently treated, 
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        public void Setup(object testElement, XmlElement variation)
        {
            FrameworkElement frameworkElement = testElement as FrameworkElement;
            Assert.AssertTrue("No FrameworkElement instance found", frameworkElement != null);

            EventTestableXmlElement = variation["EventTestable"];
            testObject = LogicalTreeHelper.FindLogicalNode(frameworkElement, EventTestableXmlElement.GetAttribute("SourceControlName")) as FrameworkElement;
            string objectPath = EventTestableXmlElement.GetAttribute("ObjectPath");

            object obj = testObject;
            if (!string.IsNullOrEmpty(objectPath))
                obj = ControlTestHelper.GetObjectByPath(testObject, objectPath, true);

            string eventName = EventTestableXmlElement.GetAttribute("EventName");
            MethodInfo methodInfo = ValidationObject.GetType().GetMethod("OnEvent");

            //hook Routed Event 
            string eventType = EventTestableXmlElement.GetAttribute("EventType");
            if ("RoutedEvent".Equals(eventType))
            {
                object typeObj = ObjectFactory.CreateObjectFromXaml(EventTestableXmlElement.GetAttribute("EventFromType"));
                uiElement = obj as UIElement;
                FieldInfo fInfo = typeObj.GetType().GetField(eventName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                routedEvent = fInfo.GetValue(uiElement) as RoutedEvent;
                eventDel = Delegate.CreateDelegate(routedEvent.HandlerType, ValidationObject, methodInfo);
                uiElement.AddHandler(routedEvent, eventDel);
            }
            else  //hook Clr Event.
            {
                eventInfo = obj.GetType().GetEvent(eventName);
                eventInfo.AddEventHandler(ValidationObject, eventDel);
            }
        }



        /// <!--summary>
        /// Only all the result in the List<bool> is true,is the final result true.
        /// </summary>
        /// <returns></returns-->
        public TestResult Validate()
        {
            if (ValidationObject.Validate(null))
                return TestResult.Pass;
            return TestResult.Fail;
        }
    }
}



namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// Helper class as object.
    /// </summary>
    public class RamdonDataSource
    {
        private string readOnlyStringProperty1;
        private string readOnlyStringProperty2;

        private string stringProperty1;
        private string stringProperty2;

        private DateTime dateTime1 = DateTime.MinValue;
        private DateTime dateTime2 = DateTime.MinValue;
        private AutoDataConfigurator dc;

        private IList dateList = new List<DateTime>();
        private IList stringList = new List<string>();
        private ArrayList objectList = new ArrayList();

        private Type type;

        internal RamdonDataSource(AutoDataConfigurator configurator)
        {
            dc = configurator;
            type = dc.ObjectType;
        }


        public Type ObjectType
        {
            set { type = value; }
            get { return type; }
        }



        public object ObjectWithType
        {
            get
            {
                return GetObjectWithType();
            }
        }



        public IList ObjectListWithType
        {
            get
            {
                Assert.AssertTrue("configuration file is needed!", dc != null);
                Assert.AssertTrue("Type information is required!", type != null);
                int length = dc.ObjectListLength;
                if (objectList.Count < length)
                {
                    for (int i = 0; i < length; i++)
                    {
                        objectList.Add(XmlHelper.ConvertToType(type, dc.GetTestString()));
                    }
                }
                return objectList;
            }
        }



        public string ConfigurationFileName
        {
            set
            {
                dc = new AutoDataConfigurator(value);
            }
        }



        public string ReadOnlyStringProperty1
        {
            get
            {
                if (string.IsNullOrEmpty(readOnlyStringProperty1))
                {
                    readOnlyStringProperty1 = GetString();
                }
                return readOnlyStringProperty1;
            }
        }



        public string ReadOnlyStringProperty2
        {
            get
            {
                if (string.IsNullOrEmpty(readOnlyStringProperty2))
                    readOnlyStringProperty2 = GetString();
                return readOnlyStringProperty2;
            }
        }



        public string StringProperty1
        {
            set { stringProperty1 = value; }
            get
            {
                if (string.IsNullOrEmpty(stringProperty1))
                    stringProperty1 = GetString();
                return stringProperty1;
            }
        }



        public string StringProperty2
        {
            set { stringProperty2 = value; }
            get
            {
                if (string.IsNullOrEmpty(StringProperty2))
                    stringProperty2 = GetString();
                return stringProperty2;
            }
        }



        public DateTime DateTimeProperty1
        {
            get
            {
                if (dateTime1 == DateTime.MinValue)
                {
                    dateTime1 = GetDateTime();
                }
                return dateTime1;
            }
            set
            {
                dateTime1 = value;
            }
        }



        public DateTime DateTimeProperty2
        {
            get
            {
                if (dateTime2 == DateTime.MinValue)
                {
                    dateTime2 = GetDateTime();
                }
                return dateTime2;
            }
            set
            {
                dateTime2 = value;
            }
        }



        public IList DateList
        {
            get
            {
                Assert.AssertTrue("configuration file is needed!", dc != null);
                int length = dc.DateListLength;
                if (dateList.Count < length)
                {
                    for (int i = 0; i < length; i++)
                    {
                        dateList.Add(dc.GetDate());
                    }
                }
                return dateList;
            }
        }



        public IList StringList
        {
            get
            {
                Assert.AssertTrue("configuration file is needed!", dc != null);
                int length = dc.StringListLength;
                if (stringList.Count < length)
                {
                    for (int i = 0; i < length; i++)
                    {
                        stringList.Add(dc.GetTestString());
                    }
                }
                return stringList;
            }
        }



        private string GetString()
        {
            Assert.AssertTrue("configuration file is needed!", dc != null);
            return dc.GetTestString();
        }

        private DateTime GetDateTime()
        {
            Assert.AssertTrue("configuration file is needed!", dc != null);
            return dc.GetDate();
        }

        private object GetObjectWithType()
        {
            Assert.AssertTrue("configuration file is needed!", dc != null);
            Assert.AssertTrue("Type information is required!", type != null);
            return XmlHelper.ConvertToType(type, dc.GetTestString());
        }
    }


    /// <summary>
    /// A helper class that provide values of type String or DateTime. Configuration files can be provided to provide data.
    /// </summary>
    internal class AutoDataConfigurator
    {
        //private static int globalStringIndex = 0;
        //private static int globalDateIndex = 0;

        private int maxDateCount = 1;
        private int maxStringCount = 1;
        private string stringTagName = string.Empty;
        private string dataTagName = string.Empty;
        private string fileName = string.Empty;
        //private string[] strs = null;
        //private string[] dataStrs = null;
        private bool useFile = false;
        private int stringLength = 50;

        private int dateListLength = 10;
        private int stringListLength = 10;
        private int objectListLength = 10;

        private Type type = null;

        private Random random = new Random();

        internal AutoDataConfigurator(string configFileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlElement root = doc["Configuration"];
            maxDateCount = GetValueFromXmlElement<int>(root, "MaxDateCount", maxDateCount);
            maxStringCount = GetValueFromXmlElement<int>(root, "MaxStringCount", maxStringCount);
            stringTagName = GetValueFromXmlElement<string>(root, "StringTagName", stringTagName);
            dataTagName = GetValueFromXmlElement<string>(root, "DataTagName", dataTagName);
            fileName = GetValueFromXmlElement<string>(root, "FileName", fileName);
            useFile = GetValueFromXmlElement<bool>(root, "UseFile", useFile);
            stringLength = GetValueFromXmlElement<int>(root, "MaxStringLength", stringLength);
            dateListLength = GetValueFromXmlElement<int>(root, "DateListLength", dateListLength);
            stringListLength = GetValueFromXmlElement<int>(root, "StringListLength", stringListLength);
            objectListLength = GetValueFromXmlElement<int>(root, "ObjectListLength", objectListLength);

            if (root.GetElementsByTagName("Type").Count > 0)
            {
                type = Type.GetType(root["Type"].InnerText);
            }
        }



        private TargetType GetValueFromXmlElement<TargetType>(XmlElement xmlElement, string tagName, TargetType defaultValue)
        {
            string text = string.Empty;
            if (xmlElement.GetElementsByTagName(tagName).Count > 0)
            {
                text = xmlElement[tagName].InnerText;
            }
            return XmlHelper.Convert<TargetType, string>(text, defaultValue);
        }


        internal Type ObjectType
        {
            get { return type; }
        }

        internal int DateListLength
        {
            get { return dateListLength; }
        }

        internal int ObjectListLength
        {
            get { return objectListLength; }
        }

        internal int StringListLength
        {
            get { return stringListLength; }
        }

        internal string GetTestString()
        {
            return string.Empty;
            //string result = string.Empty;
            //if (useFile)
            //{
            //    if (strs == null)
            //        strs = Extract.GetStringsFromXML(fileName, stringTagName);
            //
            //    result = strs[((globalStringIndex++) % maxStringCount)];
            //}
            //else
            //{
            //    result = Extract.GetTestString(((globalStringIndex++) % maxStringCount), stringLength);
            //}
            //
            //if (result.Length > stringLength)
            //    result = result.Substring(0, stringLength);
            //return result;
        }

        internal DateTime GetDate()
        {
            return DateTime.Now;
            //string dataString = string.Empty;
            //if (useFile)
            //{
            //    if (dataStrs == null)
            //        dataStrs = Extract.GetStringsFromXML(fileName, dataTagName);
            //
            //    dataString = dataStrs[((globalDateIndex++) % maxDateCount)];
            //}
            //else
            //{
            //    dataString = Extract.GetDate(((globalDateIndex++) % maxDateCount), true);
            //}
            //
            //DateTime result;
            //lf (!string.IsNullOrEmpty(dataString))
            //    result = DateTime.Parse(dataString);
            //else
            //    result = DateTime.Now;
            //
            //return result;
        }
    }


    /// <!--summary>
    /// Action that populate data into property of type IEnumerable . An example is lile:
    /// <Action Name="ControlPopulateDataAction" >
    ///<Parameter Value="listView" />
    ///<PropertyName Value="ItemsSource" />
    ///<Parameter Value="100" />
    ///<ConfigFile Value="DataConfiguration.xml" />
    ///</Action>
    /// </summary-->
    public class ControlPopulateDataAction : IAction
    {
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string[] args = actionParams as string[];
            if (args == null && args.Length != 4)
            {
                throw new ArgumentException("Wrong parameters");
            }

            string controlName = (string)actionParams[0];
            string propertyName = (args[1]);
            int count = XmlHelper.Convert<int, string>(args[2]);
            string configFileName = args[3];

            Control control = LogicalTreeHelper.FindLogicalNode((Visual)frmElement, controlName) as Control;
            Assert.AssertTrue("No control found!", control != null);

            AutoDataConfigurator configurator = new AutoDataConfigurator(configFileName);
            List<RamdonDataSource> source = new List<RamdonDataSource>();

            for (int i = 0; i < count; i++)
            {
                source.Add(new RamdonDataSource(configurator));
            }

            string[] path = propertyName.Split('.');
            object obj = ControlTestHelper.GetObjectByDotPath(control, path);
            string name = path[path.Length - 1];

            PropertyInfo pInfo = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            Assert.AssertTrue("Property " + name + " does not exist  !", pInfo != null);
            pInfo.SetValue(obj, source, new object[0]);
        }
    }


    /// <summary>
    /// Class to change sorting for a ItemsControl.
    /// </summary>
    public class CustomListViewSortor : ControlEventTestBase, IEventHandler
    {
        public void OnEvent(object sender, EventArgs args)
        {
            ItemsControl itemsControl = TestFrameworkElemet as ItemsControl;
            ICollectionView dataView = CollectionViewSource.GetDefaultView(itemsControl.ItemsSource);

            string propertyName = EventTestableXmlElement.GetAttribute("PropertyName");
            SortDescription sd = new SortDescription(propertyName, GetListSortDirection(args));
            SortDescriptionCollection sorts = dataView.SortDescriptions;

            sorts.Clear();
            sorts.Add(sd);
        }

        private ListSortDirection GetListSortDirection(object args)
        {
            GridViewColumnHeader header = (args as RoutedEventArgs).OriginalSource as GridViewColumnHeader;
            Assert.AssertTrue("No GridViewColumnHeader", header != null);

            ListSortDirection result = ListSortDirection.Ascending;
            if (header.Tag == null || ((ListSortDirection)header.Tag) == ListSortDirection.Descending)
            {
                header.Tag = ListSortDirection.Ascending;
                result = ListSortDirection.Ascending;
            }
            else
            {
                header.Tag = ListSortDirection.Descending;
                result = ListSortDirection.Descending;
            }
            return result;
        }


        public bool Validate(params object[] validationParams)
        {
            return true;
        }


        public sealed override IEventHandler ValidationObject
        {
            get { return this; }
        }
    }



    /// <summary>
    /// Click on a Visual Part of a control. This class first searches out the Visual part and click on it.
    /// </summary>
    /// <Action Name="ControlVisualPartClickAction" >
    ///   <Parameter Value="FrameworkElementName" />
    ///   <Parameter Value="VisualPartNameName" />
    ///   <Parameter Value="Times" />
    /// </Action>
    public class ControlVisualPartClickAction : IAction
    {
        /// <summary>
        /// Search out a special part and Click some times.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        /// An XTC Example:

        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            // Get Next or Previous button by searching Visual tree.
            string[] args = (string[])actionParams;
            Assert.AssertTrue(" Wrong parameters!", args != null && args.Length == 3);
            FrameworkElement mcc = ControlTestHelper.GetLogicalChild(frmElement, args[0]);

            FrameworkElement button = ControlTestHelper.GetVisualChild(mcc, args[0]);
            //Click the button the change month.
            int times = XmlHelper.Convert<int, string>(args[2]);
            if (button != null)
            {
                for (int i = 0; i < times; i++)
                {
                    UserInput.MouseLeftDownCenter(button);
                    UserInput.MouseLeftUpCenter(button);
                }
                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }
    }



    /// <summary>
    /// P1 Action Class to Press keys .
    /// </summary>
    /// An XTC Example 
    ///   <Action Name="ControlKeyInputAction" >
    ///   <Parameter Value="FrameworkElementName" />
    ///   <Parameter Value="InputKeyName" />
    ///   <Parameter Value="CountToInput" />
    ///   <Parameter Value="FunctionKeyName" />
    /// </Action>
    public class ControlKeyInputAction : IAction
    {
        /// <summary>
        /// Keyboard input clss. Function keys are supported.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string[] args = (string[])actionParams;
            Assert.AssertTrue("Not enough parameters in ControlKeyInputAction ", args != null && args.Length >= 3);
            FrameworkElement mcc = ControlTestHelper.GetLogicalChild(frmElement, args[0]);

            if (args.Length > 3 && args[3].Equals("True"))
            {
                mcc.Focus();
            }
            string key = args[1];
            int times = XmlHelper.Convert<int, string>(args[2]);

            if (args.Length > 4)
            {
                UserInput.KeyDown(args[4]);
            }

            for (int i = 0; i < times; i++)
            {
                UserInput.KeyPress(key);
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            if (args.Length > 4)
            {
                UserInput.KeyUp(args[4]);
            }
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }


    /// <summary>
    /// P1 action class to set binding between Controls.
    /// </summary>
    /// An XTC Example 
    ///   <Action Name="ControlSetBindingAction" >
    ///   <Parameter Value="SourceElementName" /> 
    ///   <Parameter Value="DestinationElementName" />
    ///   <Parameter Value="SourcePropertyname" />
    ///   <Parameter Value="DestinationPropertyname" />
    /// </Action>
    public class ControlSetBindingAction : IAction
    {
        /// <summary>
        /// Set bining between two Controls
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            // Get FrameworkElements from Logical tree.
            string[] args = (string[])actionParams;

            FrameworkElement source = ControlTestHelper.GetLogicalChild(frmElement, args[0]);
            FrameworkElement dest = ControlTestHelper.GetLogicalChild(frmElement, args[1]);

            Binding bind = new Binding(args[2]);
            bind.Source = source;
            DependencyProperty property = ControlTestHelper.DependencyPropertyFromName(args[3], dest.GetType());
            Assert.AssertTrue("Can not find DependencyProperty " + args[3] + "  in type " + dest.GetType(), property != null);

            dest.SetBinding(property, bind);
            QueueHelper.WaitTillQueueItemsProcessed();
        }



    }



    /// <summary>
    /// Class To click a element in a VisualTree. This element is found by its Type and index.
    /// a xtc Example is:
    /// <Action Name="ControlVisualIndexPartClickAction" >
    /// <Parameter Value="ControlName" />
    /// <Parameter Value="Index" />
    ///  <Parameter Value="XmlNamespace" />
    ///  <Parameter Value="TypeName" />
    /// <Parameter Value="ClickTimes" />
    /// <Parameter Value="FunctionKey" />
    /// </Action>
    /// </summary>
    public class ControlVisualIndexPartClickAction : IAction
    {
        /// <summary>
        /// Class To click a element in a VisualTree. 
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string[] args = actionParams as string[];
            if (args == null || args.Length < 5)
            {
                throw new ArgumentException("Not enough parameters");
            }
            string controlName = args[0];
            int pos = XmlHelper.Convert<int, string>(args[1]);
            string xmlNamespace = args[2];
            string typeName = args[3];
            int times = XmlHelper.Convert<int, string>(args[4]);

            //Get type by XmlNamespace and Type.
            ArrayList list = ControlTestHelper.FindPartsByNamespace(frmElement, controlName, xmlNamespace, typeName);
            Assert.AssertTrue("Parameter error: Item position out of range.", pos < list.Count);

            //Execute operation on the Visual Part.      
            FrameworkElement element = list[pos] as FrameworkElement;
            if (element != null)
            {
                string functionKey = null;
                if (actionParams.Length >= 6)
                {
                    functionKey = (string)actionParams[5];
                    UserInput.KeyDown(functionKey);
                }
                for (int i = 0; i < times; i++)
                {
                    UserInput.MouseLeftClickCenter(element);
                }
                if (actionParams.Length >= 6)
                {
                    UserInput.KeyUp(functionKey);
                }
                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }
    }



    /// <summary>
    /// Class to test Selector such as StyleSelector, TemplateSelector and so on. To do this, a subclass of this shoud be implemented.
    /// Its subclass must implements method GetSourceValue.
    /// </summary>
    public abstract class ControlSetSelector : IAction
    {
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string[] args = actionParams as string[];
            if (args == null || (args.Length != 3))
            {
                throw new ArgumentException("Wrong input parameters in ControlSetSelector !");
            }
            Assert.AssertTrue("Wrong input parameters in ControlSetSelector !", args != null && args.Length == 3);

            string controlName = args[0];
            string propertyName = args[1];
            string resourceID = args[2];

            FrameworkElement element = ControlTestHelper.GetLogicalChild(frmElement, controlName);
            DependencyProperty property1 = ControlTestHelper.DependencyPropertyFromName(propertyName, element.GetType());

            Assert.AssertTrue("Can not find DependencyProperty " + propertyName + "  in type " + element.GetType(), property1 != null);
            element.SetValue(property1, GetSourceValue(element, resourceID));
        }

        public virtual object GetSourceValue(FrameworkElement element, string resourceID)
        {
            return null;
        }
    }



    /// <summary>
    /// Action class to Set StyleSelector on a class.
    /// <Action Name="ControlSetStyleSelector" >
    ///    <ControlName Value="listView" />
    ///    <PropertyName Value="ItemTemplateSelector" />
    ///    <ReourceID Value="template" />
    /// </Action>
    /// </summary>
    public class ControlSetTemplateSelector : ControlSetSelector
    {
        public override object GetSourceValue(FrameworkElement element, string resourceID)
        {
            DataTemplate template = element.Resources[resourceID] as DataTemplate;
            return new TemplateSelectorFactory(template);
        }


        /// <summary>
        /// private class acts as DataTemplateSelector.
        /// </summary>
        public class TemplateSelectorFactory : DataTemplateSelector
        {
            private DataTemplate _template = null;

            public TemplateSelectorFactory(DataTemplate template)
            {
                _template = template;
            }


            /// <summary>
            /// return a DataTemplator
            /// </summary>
            /// <param name="item"></param>
            /// <param name="container"></param>
            /// <returns></returns>
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                return _template;
            }
        }
    }



    /// <summary>
    /// Action class to Set StyleSelector on a class.
    /// <Action Name="ControlSetStyleSelector" >
    ///    <ControlName Value="listView" />
    ///    <PropertyName Value="ItemContainerStyleSelector" />
    ///    <ReourceID Value="style" />
    /// </Action>
    /// </summary>
    public class ControlSetStyleSelector : ControlSetSelector
    {
        public override object GetSourceValue(FrameworkElement element, string resourceID)
        {
            Style style = element.Resources[resourceID] as Style;
            return new StyleSelectorFactory(style);
        }


        /// <summary>
        /// private class acts as DataTemplateSelector.
        /// </summary>
        public class StyleSelectorFactory : StyleSelector
        {
            private Style _style = null;

            public StyleSelectorFactory(Style style)
            {
                _style = style;
            }


            /// <summary>
            /// return a style
            /// </summary>
            /// <param name="item"></param>
            /// <param name="container"></param>
            /// <returns></returns>
            public override Style SelectStyle(object item, DependencyObject container)
            {
                return _style;
            }
        }
    }



    /// <!--summary>
    /// Class move from one IList to another. Complicated path dotted is supported.a xtc example is like below:
    /// <Action Name="ControlMoveListPartAction">
    /// <Parameter Value="listView" />
    /// <Parameter Value="View.Columns.[0]" />
    /// <Parameter Value="listView" />
    /// <Parameter Value="View.Columns" />
    /// <Parameter Value="2" />
    /// <DeleteSource Value="false" />
    /// </Action>
    /// if the args.Lenght <=2, just delete the item. If args.Length > 2, it will move an item from the source to destination, at this time, if the 
    /// DeleteSource is true, delete the item in the source.
    /// </summary-->
    class ControlMoveListPartAction : IAction
    {
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string[] args = actionParams as string[];
            Assert.AssertTrue("Not enough parameters in ListViewMoveColumnAction", args != null && args.Length >= 2);

            string sourceControlName = args[0];
            string sourceObjectPath = args[1];
            Visual sourceVisual = (Visual)LogicalTreeHelper.FindLogicalNode((Visual)frmElement, sourceControlName);
            Assert.AssertTrue("Null reference in ListViewMoveObjectAction", sourceVisual != null);
            object sourceObj = ControlTestHelper.GetObjectByPath(sourceVisual, sourceObjectPath, true);

            if (args.Length > 2)
            {
                Assert.AssertTrue("Not enough parameters in ListViewMoveColumnAction", args.Length == 6);
                string destinationControlName = args[2];
                string destinationObjectPath = args[3];
                int insertPos = XmlHelper.Convert<int, string>(args[4]);
                string removeSource = args[5];

                Visual destinationVisual = (Visual)LogicalTreeHelper.FindLogicalNode((Visual)frmElement, destinationControlName);
                Assert.AssertTrue("Null reference in ListViewMoveObjectAction", destinationVisual != null);

                if (("True").Equals(removeSource))
                {
                    RemoveItem(sourceVisual, sourceObjectPath, sourceObj);
                }

                object destinationParent = ControlTestHelper.GetObjectByPath(destinationVisual, destinationObjectPath, true);
                IList destinationList = destinationParent as IList;
                Assert.AssertTrue("Only IList interface is supported", destinationList != null);
                destinationList.Insert(insertPos, sourceObj);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            else
            {
                RemoveItem(sourceVisual, sourceObjectPath, sourceObj);
            }
        }

        /// <summary>
        /// private method to remove an item from IList.
        /// </summary>
        /// <param name="sourceVisual"></param>
        /// <param name="sourceObjectPath"></param>
        /// <param name="sourceObj"></param>
        private void RemoveItem(Visual sourceVisual, string sourceObjectPath, object sourceObj)
        {
            object parentObj = ControlTestHelper.GetObjectByPath(sourceVisual, sourceObjectPath, false);
            IList list = parentObj as IList;
            Assert.AssertTrue("Only IList interface is supported", list != null);
            list.Remove(sourceObj);
        }
    }



    /// <summary>
    /// Class to set value on Property. dot format and index format are supported. 
    /// a example:
    /// <Action Name="ControlSetPropertyAction2" >
    /// <Parameter Value="listView" />
    /// <Parameter Value="View.Columns.[0].Width" />
    /// <Parameter Value="500" />
    /// </Action>
    /// </summary>
    public class ControlSetPropertyAction : ColtrolSetPropertyActionBase, IAction
    {
        /// <summary>
        /// Set Value on Property
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string[] args = (string[])actionParams;
            Assert.AssertTrue("Not enough parameters in ControlSetPropertyAction  ", args != null && args.Length == 3);
            string propertyPath = args[1];
            string value = args[2];

            FrameworkElement frameworkElement = LogicalTreeHelper.FindLogicalNode(frmElement, args[0]) as FrameworkElement;
            Assert.AssertTrue(" Can not find control " + args[0], frameworkElement != null);

            SetPropertyOntoObject(frameworkElement, frameworkElement, propertyPath, value);
        }
    }



    /// <summary>
    /// A base class to enable property verification.
    /// </summary>
    public class ColtrolSetPropertyActionBase
    {
        protected void SetPropertyOntoObject(object obj, FrameworkElement frameworkElement, string propertyPath, string valueToSet)
        {
            string[] path = ControlTestHelper.FromPathToDotPath(propertyPath);
            object targetObj = ControlTestHelper.GetObjectByDotPath(obj, path);

            //Assert.AssertTrue(" Only support DependencyObject", ((DependencyObject)obj)!= null);
            ControlTestHelper.SetPropertyOntoObject(targetObj, frameworkElement, path[path.Length - 1], valueToSet);
        }
    }



    /// <summary>
    /// Class to invoke a method in a object. Dot path is supported. a xtc example is like:
    /// <Action Name="ControlMethodInvokeAction2">
    ///     <ControlName Value="listView" />
    ///     <PropertyPath Vlaue="View.Columns" />
    ///     <InterfaceName Value="IList" />
    ///     <MethodName Value="Contains" />
    ///     <Parameter1 Value="&lt;s:GridViewColumn DisplayMemberPath='@Level' xmlns:s='http://schemas.microsoft.com/winfx/2006/xaml/presentation' /&gt;"/>
    ///   </Action>
    /// </summary>
    public class ControlMethodInvokeAction2 : IAction
    {        
        /// <summary>
        /// Search the object and invoke method on it.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string[] args = actionParams as string[];
            Assert.AssertTrue("Wrong argument in ControlMethodInvokeAction2!", args != null && args.Length >= 4);

            string controlName = args[0];
            string propertyPath = args[1];
            string interfaceName = args[2];
            string methodName = args[3];


            Visual tempResult = LogicalTreeHelper.FindLogicalNode(frmElement, controlName) as Visual;
            Assert.AssertTrue("No Control " + controlName + " Found in ControlMethodInvokeAction2 !", tempResult != null);
            string[] path = propertyPath.Split('.');

            object obj = ControlTestHelper.GetObjectByDotPath(tempResult, path);
            Assert.AssertTrue("Null reference in ControlMethodInvokeAction2!", obj != null);
            PropertyInfo pInfo = obj.GetType().GetProperty(path[path.Length - 1], BindingFlags.Instance | BindingFlags.Public);

            obj = pInfo.GetValue(obj, new object[0]);
            Type type = null;
            if (!"".Equals(interfaceName))
                type = obj.GetType().GetInterface(interfaceName);
            Assert.AssertTrue("Null object in ControlMethodInvokeAction2!", obj != null);

            MethodInfo mInfo = type.GetMethod(methodName);
            Assert.AssertTrue("Null MethodInfo in ControlMethodInvokeAction2!", mInfo != null);

            int length = args.Length;
            object[] param = null;
            if (length > 4)
            {
                param = new object[length - 4];
                for (int i = 4; i < length; i++)
                {
                    param[i - 4] = ObjectFactory.CreateObjectFromXaml(args[i]);
                }
            }
            mInfo.Invoke(obj, param);
        }
    }



    /// <summary>
    /// Action to verify property on an index visual part.
    /// </summary>
    public class ControlSetIndexPartPropertyAction : ColtrolSetPropertyActionBase, IAction
    {
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string[] args = actionParams as string[];
            Assert.AssertTrue("Not enough parameters", args != null && args.Length == 6);

            string controlName = args[0];
            string xmlNamespace = args[1];
            string typeName = args[2];
            int index = XmlHelper.Convert<int, string>(args[3]);
            string propertyPath = args[4];
            string valueToSet = args[5];

            FrameworkElement frameworkElement = ControlTestHelper.GetLogicalChild(frmElement, controlName);
            FrameworkElement target = ControlTestHelper.FindIndexPartByNamespace(frameworkElement, controlName, xmlNamespace, typeName, index);

            SetPropertyOntoObject(target, frameworkElement, propertyPath, valueToSet);
        }
    }
}

