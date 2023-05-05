// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Reflection;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Globalization;

namespace ElementLayout.FeatureTests.LayoutSystem
{
    //////////////////////////////////////////////////////////////////
    /// This Layout System Priority Test cases.
    /// 
    //////////////////////////////////////////////////////////////////
    [Test(1, "LayoutSystem", "InfinityArrangeExceptionTest", Variables = "Area=ElementLayout")]
    public class InfinityArrangeExceptionTest : CodeTest
    {
        private InfinityArrangePanel _rootElement = null;
        private List<TempResult> _failures = new List<TempResult>();

        public InfinityArrangeExceptionTest() 
        {
            CreateLog = false;
        }

        public override void WindowSetup()
        {            
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
        }

        public override void TestActions()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            if (assembly == null)
            {
                throw new Exception("Could not load Framework Element assembly.");
            }
            else
            {
                foreach (string element in Helpers.ElementList)
                {
                    TestLog newlog = new TestLog(element);

                    Helpers.Log(String.Format(" Element : {0}", element));
                    _rootElement = null;
                    this.window.Content = null;
                    CommonFunctionality.FlushDispatcher();

                    _rootElement = new InfinityArrangePanel();
                    FrameworkElement fe = Instantiate(assembly, element);
                    _rootElement.Children.Add(fe as UIElement);
                    this.window.Content = _rootElement;
                    CommonFunctionality.FlushDispatcher();
                    Helpers.Log(String.Format(" Result : {0}", _rootElement.result));

                    if (!_rootElement.result)
                    {
                        TempResult r = new TempResult();
                        r.element = element;
                        r.result = _rootElement.result;
                        _failures.Add(r);
                        newlog.Result = TestResult.Fail;
                        newlog.Close();
                    }
                    else
                    {
                        newlog.Result = TestResult.Pass;
                        newlog.Close();
                    }
                }
            }
        }

        public override void TestVerify()
        {
            if (_failures.Count > 0)
            {
                Helpers.Log("failures :");
                this.Result = false;
                foreach (TempResult r in _failures)
                {
                    Helpers.Log(r.element);
                }
            }
            else
            {
                Helpers.Log("No failures.");
                this.Result = true;
            }
        }

        public struct TempResult
        {
            public bool result;
            public string element;
        }

        private FrameworkElement Instantiate(Assembly assembly, string feType)
        {
            if (feType == null || feType == "")
            {
                throw new ArgumentException("The framework element type was not found");
            }

            FrameworkElement fe = null;

            if (assembly == null)
            {
                throw new ArgumentNullException("Assembly not loaded.");
            }

            fe = assembly.CreateInstance(feType) as FrameworkElement;

            if (fe == null)
            {
                throw new ApplicationException("The type [" + feType + "] could not be instantiated from the assembly.");
            }

            return fe;
        }
    }

    [Test(1, "LayoutSystem", "NaNArrangeExceptionTest", Variables = "Area=ElementLayout")]
    public class NaNArrangeExceptionTest : CodeTest
    {
        private NaNArrangePanel _rootElement = null;
        private List<TempResult> _failures = new List<TempResult>();

        public NaNArrangeExceptionTest() 
        {
            CreateLog = false;
        }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
        }

        public override void TestActions()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            if (assembly == null)
            {
                throw new Exception("Could not load Framework Element assembly.");
            }
            else
            {
                foreach (string element in Helpers.ElementList)
                {
                    TestLog newlog = new TestLog(element);

                    Helpers.Log(String.Format(" Element : {0}", element));
                    _rootElement = null;
                    this.window.Content = null;
                    CommonFunctionality.FlushDispatcher();

                    _rootElement = new NaNArrangePanel();
                    FrameworkElement fe = Instantiate(assembly, element);
                    _rootElement.Children.Add(fe as UIElement);
                    this.window.Content = _rootElement;
                    CommonFunctionality.FlushDispatcher();
                    Helpers.Log(String.Format(" Result : {0}", _rootElement.result));
         
                    if (!_rootElement.result)
                    {
                        TempResult r = new TempResult();
                        r.element = element;
                        r.result = _rootElement.result;
                        _failures.Add(r);
                        newlog.Result = TestResult.Fail;
                        newlog.Close();
                    }
                    else
                    {
                        newlog.Result = TestResult.Pass;
                        newlog.Close();
                    }
                }
            }
        }
       
        public override void TestVerify()
        {
            if (_failures.Count > 0)
            {
                Helpers.Log("failures :");
                this.Result = false;
                foreach (TempResult r in _failures)
                {
                    Helpers.Log(r.element);
                }
            }
            else
            {
                Helpers.Log("No failures.");
                this.Result = true;
            }
        }

        public struct TempResult
        {
            public bool result;
            public string element;
        }

        private FrameworkElement Instantiate(Assembly assembly, string feType)
        {
            if (feType == null || feType == "")
            {
                throw new ArgumentException("The framework element type was not found");
            }

            FrameworkElement fe = null;

            if (assembly == null)
            {
                throw new ArgumentNullException("Assembly not loaded.");
            }

            fe = assembly.CreateInstance(feType) as FrameworkElement;

            if (fe == null)
            {
                throw new ApplicationException("The type [" + feType + "] could not be instantiated from the assembly.");
            }

            return fe;
        }
    }

    [Test(1, "LayoutSystem", "NaNMeasureExceptionTest", Variables = "Area=ElementLayout")]
    public class NaNMeasureExceptionTest : CodeTest
    {
        private NaNMeasurePanel _rootElement = null;
        private List<TempResult> _failures = new List<TempResult>();

        public NaNMeasureExceptionTest() 
        {
            CreateLog = false;
        }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
        }

        public override void TestActions()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            if (assembly == null)
            {
                throw new Exception("Could not load Framework Element assembly.");
            }
            else
            {
                foreach (string element in Helpers.ElementList)
                {
                    TestLog newlog = new TestLog(element);
                    
                    Helpers.Log(String.Format(" Element : {0}", element));
                    _rootElement = null;
                    this.window.Content = null;
                    CommonFunctionality.FlushDispatcher();

                    _rootElement = new NaNMeasurePanel();
                    FrameworkElement fe = Instantiate(assembly, element);
                    _rootElement.Children.Add(fe as UIElement);
                    this.window.Content = _rootElement;
                    CommonFunctionality.FlushDispatcher();
                    Helpers.Log(String.Format(" Result : {0}", _rootElement.result));
                  
                    if (!_rootElement.result)
                    {
                        TempResult r = new TempResult();
                        r.element = element;
                        r.result = _rootElement.result;
                        _failures.Add(r);
                        newlog.Result = TestResult.Fail;
                        newlog.Close();
                    }
                    else
                    {
                        newlog.Result = TestResult.Pass;
                        newlog.Close();
                    }
                }
            }
        }

        public override void TestVerify()
        {
            if (_failures.Count > 0)
            {
                Helpers.Log("failures :");
                this.Result = false;
                foreach (TempResult r in _failures)
                {
                    Helpers.Log(r.element);
                }
            }
            else
            {
                Helpers.Log("No failures.");
                this.Result = true;
            }
        }

        public struct TempResult
        {
            public bool result;
            public string element;
        }
        
        private FrameworkElement Instantiate(Assembly assembly, string feType)
        {
            if (feType == null || feType == "")
            {
                throw new ArgumentException("The framework element type was not found");
            }

            FrameworkElement fe = null;

            if (assembly == null)
            {
                throw new ArgumentNullException("Assembly not loaded.");
            }

            fe = assembly.CreateInstance(feType) as FrameworkElement;

            if (fe == null)
            {
                throw new ApplicationException("The type [" + feType + "] could not be instantiated from the assembly.");
            }

            return fe;
        }
    }

    [Test(1, "LayoutSystem", "MeasureReturnsNaNExceptionTest", Variables = "Area=ElementLayout")]
    public class MeasureReturnsNaNExceptionTest : CodeTest
    {
        public MeasureReturnsNaNExceptionTest()
        { }

        public override void TestActions()
        {
            Window testWindow = new Window();
            testWindow.Content = new MeasureReturnsNaNPanel();

            try
            {
                testWindow.Show();
            }
            catch (Exception e)
            {
                // Exception being tested
                // UIElement_Layout_NaNReturned=Layout measurement override of element '{0}' should not return NaN values as its DesiredSize.
                this.Result = Exceptions.CompareMessage(e.Message, "UIElement_Layout_NaNReturned", WpfBinaries.PresentationCore);
            }
        }

        public override void TestVerify()
        {
            if (this.Result)
            {
                Helpers.Log("Test Passed.");
            }
            else
            {
                Helpers.Log("Test Failed.");
            }
        }
    }
}
