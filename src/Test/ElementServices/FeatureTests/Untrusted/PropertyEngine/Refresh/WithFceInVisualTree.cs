// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshWithFceInVisualTreeTest
{
    /******************************************************************************
    * CLASS:          WithFceInVisualTree
    ******************************************************************************/
    [Test(0, "PropertyEngine.WithFceInVisualTree", TestCaseSecurityLevel.FullTrust, "WithFceInVisualTree")]
    public class WithFceInVisualTree : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("PositiveTestWithTestTypes")]
        [Variation("NegativeVisualTreeNotDeriveFromFE")]

        /******************************************************************************
        * Function:          WithFceInVisualTree Constructor
        ******************************************************************************/
        public WithFceInVisualTree(string arg)
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
            TestWithFceInVisualTree test = new TestWithFceInVisualTree();

            Utilities.StartRunAllTests("WithFceInVisualTree");

            switch (_testName)
            {
                case "PositiveTestWithTestTypes":
                    test.PositiveTestWithTestTypes();
                    break;
                case "NegativeVisualTreeNotDeriveFromFE":
                    test.NegativeVisualTreeNotDeriveFromFE();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            Utilities.StopRunAllTests();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }

    /******************************************************************************
    * CLASS:          TestWithFceInVisualTree
    ******************************************************************************/
    /// <summary>
    /// Enable FrameowrkContentElement in the VisualTree of a Style
    /// Description: Previously only FraeworkElement can be used in Style
    /// Now both FrameworkElement and FramentContentElement can be used in Style
    /// 
    /// </summary>
    public class TestWithFceInVisualTree
    {
        /// <summary>
        /// Exercising positive tests with test types defined in this file
        /// </summary>
        public void PositiveTestWithTestTypes()
        {
            Utilities.PrintTitle("Exercising positive tests with test types");

            TestFrameworkElement test = new TestFrameworkElement();
            Style myStyle = new Style(typeof(TestFrameworkElement));
            FrameworkElementFactory fefRoot = new FrameworkElementFactory(typeof(TestFrameworkElement));
            FrameworkElementFactory fefChild1 = new FrameworkElementFactory(typeof(TestFrameworkContentElement));
            FrameworkElementFactory fefChild11 = new FrameworkElementFactory(typeof(TestFrameworkContentElement));
            FrameworkElementFactory fefChild12 = new FrameworkElementFactory(typeof(TestFrameworkContentElement));

            fefRoot.AppendChild(fefChild1);
            fefChild1.AppendChild(fefChild11);
            fefChild1.AppendChild(fefChild12);
            ControlTemplate template = new ControlTemplate(typeof(TestFrameworkElement));
            template.VisualTree = fefRoot;
            myStyle.Setters.Add(new Setter(TestFrameworkElement.TemplateProperty, template));
            test.Style = myStyle;
            test.ApplyTemplate();

            TestFrameworkElement t1 = (TestFrameworkElement)VisualTreeHelper.GetChild(test,0);
            TestFrameworkContentElement t2 = (TestFrameworkContentElement)(t1.Children[0]);
            TestFrameworkContentElement t3 = (TestFrameworkContentElement)t2.Children[0];
            TestFrameworkContentElement t4 = (TestFrameworkContentElement)t2.Children[1];

           UtilityHelper.Utilities.Assert(t1.MagicNumber == 11, "t1.MagicNumber == 11");
           UtilityHelper.Utilities.Assert(t1.Children.Count == 1, "t1.Children.Count == 1");
           UtilityHelper.Utilities.Assert(t2.MagicNumber == 99, "t2.MagicNumber == 99");
           UtilityHelper.Utilities.Assert(t2.Children.Count == 2, "t2.Children.Count == 2");
           UtilityHelper.Utilities.Assert(t3.MagicNumber == t4.MagicNumber, "t3.MagicNumber == t4.MagicNumber");
           UtilityHelper.Utilities.Assert(t4.Children.Count == 0, "t4.Children.Count == 0");

        }

        /// <summary>
        /// The root of a VisualTree must derive from FrameworkElement.
        /// Otherwise we get ArgumentException: The root of a VisualTree must derive 
        /// from FrameworkElement.
        /// </summary>
        public void NegativeVisualTreeNotDeriveFromFE()
        {
            Utilities.PrintTitle("Negative Test Case:  The root of a VisualTree must derive from FrameworkElement.");

            TestFrameworkElement test = new TestFrameworkElement();
            Style myStyle = new Style(typeof(TestFrameworkElement));
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TestFrameworkContentElement));


            try
            {
                ControlTemplate template = new ControlTemplate(typeof(TestFrameworkElement));
                template.VisualTree = fef;
               UtilityHelper.Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
               UtilityHelper.Utilities.ExpectedExceptionReceived(ex);
            }
            test.Style = myStyle;

        }
    }

    /******************************************************************************
    * CLASS:          TestFrameworkElement
    ******************************************************************************/
    /// <summary>
    /// Test FrameworkElement-derived class 
    /// </summary>
    public class TestFrameworkElement : Control, IAddChild
    {
        /// <summary>
        /// Default Ctor
        /// </summary>
        public TestFrameworkElement()
        {
            _children = new VisualCollection(this);        
            this._createdInfo = System.DateTime.Now.ToLongTimeString() + " FE";
        }

        /// <summary>
        /// Dp
        /// </summary>
        public static DependencyProperty MagicNumberProperty = DependencyProperty.Register("MagicNumber", typeof(int), typeof(TestFrameworkElement), new PropertyMetadata(11));

        /// <summary>
        /// CLR Accessor
        /// </summary>
        /// <value></value>
        public int MagicNumber
        {
            set
            {
                SetValue(MagicNumberProperty, value);
            }
            get
            {
                if (!_magicNumberCacheValid)
                {
                    _magicNumber = (int)GetValue(MagicNumberProperty);
                    _magicNumberCacheValid = true;
                }

                return _magicNumber;
            }
        }

        private bool _magicNumberCacheValid = false;
        private int _magicNumber;
        
        void IAddChild.AddChild(object o)
        {
            AppendModelChild(o);
        }

        void IAddChild.AddText(string s)
        {
        }

        /// <summary>
        /// Append to logical tree
        /// </summary>
        /// <param name="modelChild">child to add</param>
        public void AppendModelChild(object modelChild)
        {
            _modelChildren.Add(modelChild);
            AddLogicalChild(modelChild);
        }

        /// <summary>
        /// Remove from logical tree
        /// </summary>
        /// <param name="modelChild">child to remove</param>
        public void RemoveModelChild(object modelChild)
        {
            _modelChildren.Remove(modelChild);
            RemoveLogicalChild(modelChild);
        }

        /// <summary>
        /// Children
        /// </summary>
        /// <value>Of type IEnumerator</value>
        protected override IEnumerator LogicalChildren
        {
            get { return _modelChildren.GetEnumerator(); }
        }

        /// <summary>
        /// Children
        /// </summary>
        /// <value>Of ype ArrayList</value>
        public ArrayList Children
        {
            get { return _modelChildren; }
        }

        /// <summary>
        /// Append child
        /// </summary>
        /// <param name="child">child to append</param>
        public void AppendChild(Visual child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            // if you have a template
            if(base.VisualChildrenCount != 0 && index == 0)
            {
                return base.GetVisualChild(0);
            }            
            // otherwise you can have your own children
            if(_children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if(index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }

        /// <summary>
        /// Returns the Visual children count.
        /// </summary>            
        protected override int VisualChildrenCount
        {           
            get 
            {
                //you can either have a Template or your own children
                if(base.VisualChildrenCount > 0) return 1;
                else return  _children.Count; 
            }            
        }


        private VisualCollection _children;
        private ArrayList _modelChildren = new ArrayList();

        /// <summary>
        /// Override toString
        /// </summary>
        /// <returns>Creation Information</returns>
        public override string ToString()
        {
            return _createdInfo;
        }

        private string _createdInfo = "NoInfo FE";
    }

    /******************************************************************************
    * CLASS:          TestFrameworkContentElement
    ******************************************************************************/
    /// <summary>
    /// Class to use into Style
    /// </summary>
    public class TestFrameworkContentElement : FrameworkContentElement, IAddChild
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public TestFrameworkContentElement()
        {
            this._createdInfo = System.DateTime.Now.ToLongTimeString() + " FCE";
        }

        /// <summary>
        /// DP: MagicNumber Property is of type int with default value 99
        /// </summary>
        public static DependencyProperty MagicNumberProperty =
                                         DependencyProperty.Register("MagicNumber", typeof(int), typeof(TestFrameworkContentElement), new PropertyMetadata(99));

        /// <summary>
        /// CLR Accessor
        /// </summary>
        /// <value>value</value>
        public int MagicNumber
        {
            set
            {
                SetValue(MagicNumberProperty, value);
            }
            get
            {
                if (!_magicNumberCacheValid)
                {
                    _magicNumber = (int)GetValue(MagicNumberProperty);
                    _magicNumberCacheValid = true;
                }

                return _magicNumber;
            }
        }

        private bool _magicNumberCacheValid = false;
        private int _magicNumber;

        void IAddChild.AddChild(object o)
        {
            AppendModelChild(o);
        }

        /// <summary>
        /// Do Nothing
        /// </summary>
        /// <param name="s">String to Add</param>
        void IAddChild.AddText(string s)
        {
        }

        /// <summary>
        /// Append model child 
        /// </summary>
        /// <param name="modelChild">model Child</param>
        public void AppendModelChild(object modelChild)
        {
            _modelChildren.Add(modelChild);
            AddLogicalChild(modelChild);
        }

        /// <summary>
        /// Remove from model tree
        /// </summary>
        /// <param name="modelChild"></param>
        public void RemoveModelChild(object modelChild)
        {
            _modelChildren.Remove(modelChild);
            RemoveLogicalChild(modelChild);
        }

        /// <summary>
        /// Children
        /// </summary>
        /// <value>Of type IEnumerator</value>
        protected override IEnumerator LogicalChildren
        {
            get { return _modelChildren.GetEnumerator(); }
        }

        /// <summary>
        /// Children
        /// </summary>
        /// <value>of Type ArrayList</value>
        public ArrayList Children
        {
            get { return _modelChildren; }
        }

        private ArrayList _modelChildren = new ArrayList();

        /// <summary>
        /// Override ToString()
        /// </summary>
        /// <returns>Creation Information</returns>
        public override string ToString()
        {
            return _createdInfo;
        }

        private string _createdInfo = "NoInfo FCE";
    }
}

