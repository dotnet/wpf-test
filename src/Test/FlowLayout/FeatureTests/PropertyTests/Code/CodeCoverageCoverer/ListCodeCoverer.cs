// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Controls;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "Code Coverage - List", MethodName = "Run")]
    public class ListCodeCoverer : AvalonTest
    {
        private List _list1;
        private ListItem _li;
        private Window _w1;

        public ListCodeCoverer()
        {
            CreateLog = false;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        private TestResult Initialize()
        {
            _w1 = new Window();
            UISetup(_w1, out _list1, out _li);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w1.Close();           
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            PassMethods(_list1, _li);
            FailMethods(_list1);
            SetProperties(_list1, _li);
            return TestResult.Pass;
        }

        private void UISetup(Window w1, out List list1, out ListItem li)
        {
            li = new ListItem();
            list1 = new List(li);
            FlowDocumentReader reader = new FlowDocumentReader();
            FlowDocument fd = new FlowDocument(list1);
            reader.Document = fd;
            w1.Content= reader;
            w1.Show();
        }

        private void PassMethods(List list1, ListItem li)
        {
            TestLog listPassMethods = new TestLog("List Pass Methods");
            
            ReflectionHelper rh = ReflectionHelper.WrapObject(list1);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo mInfo = list1.GetType().GetMethod("GetListItemIndex", flags, null, new Type[] { typeof(ListItem) }, null);
            try
            {
                rh.CallMethod(mInfo, li);
            }
            catch (TargetInvocationException) { }

            listPassMethods.Result = TestResult.Pass;
            listPassMethods.Close();
        }

        private void FailMethods(List list1)
        {
            TestLog listFailMethods = new TestLog("List Fail Methods");

            ReflectionHelper rh = ReflectionHelper.WrapObject(list1);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo mInfo = list1.GetType().GetMethod("GetListItemIndex", flags, null, new Type[] { typeof(ListItem) }, null);
            try
            {
                rh.CallMethod(mInfo, null);
            }
            catch (TargetParameterCountException) { }
            try
            {
                rh.CallMethod(mInfo, new ListItem());
            }
            catch (TargetInvocationException) { }

            try
            {
                List lis = new List(null);
            }
            catch (ArgumentNullException) { }

            listFailMethods.Result = TestResult.Pass;
            listFailMethods.Close();
        }

        private void SetProperties(List list1, ListItem li)
        {
            list1.MarkerOffset = 5.0;
            list1.MarkerStyle = TextMarkerStyle.None;
            list1.StartIndex = 2;

            li.BorderBrush = Brushes.AliceBlue;
            li.BorderThickness = new Thickness(10);
            li.FlowDirection = FlowDirection.LeftToRight;
            li.LineHeight = 10;
            li.Margin = new Thickness(3);
            li.Padding = new Thickness(3);
            li.TextAlignment = TextAlignment.Left;
        }
    }
}
