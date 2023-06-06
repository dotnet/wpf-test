// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;
using System.Windows.Data;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>TextBlock</area>
    /// <owner>Microsoft</owner>
    /// <priority>2</priority>
    /// <description>
    /// Testing Textblock data binding.
    /// </description>
    /// </summary>
    [Test(0, "TextBlock", "TextBlockDataBinding")]
    public class TextBlockDataBinding : AvalonTest
    {
        #region Test case members
         
        private TextBlock _tb1,_tb2;
        private MyDataSource _myDS;
        private Binding _oStringProp;
        private Window _w1;

        #endregion

        #region Constructor

        public TextBlockDataBinding()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        #endregion

        #region Test Steps
      
        /// <summary>
        /// Initialize: Setup test
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {     
            _tb1 = new TextBlock();
            _tb2 = new TextBlock();
            _tb1.Name = "tb1";
            _tb2.Name = "tb2";
            _myDS             = new MyDataSource();
            _oStringProp      = new Binding("StringProperty");
            _oStringProp.Source = _myDS;
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w1.Close();
            return TestResult.Pass;
        }
    
        /// <summary>
        /// RunTests: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            // go with defaults
            _w1 = new Window();
            DockPanel dp = new DockPanel();
            _w1.Content= dp;

            Binding b = null;
            TextBlock tb = _tb1;
            BindingOperations.ClearAllBindings(tb);
            tb.SetBinding(TextBlock.TextProperty, _oStringProp);
            Validate(_tb1);

            BindingOperations.ClearAllBindings(tb);

            b = new Binding("StringProperty");
            b.Source = _myDS;
            b.Mode = BindingMode.OneTime;
            _myDS.StringProperty = "One Time BindingExpression String";
            tb.SetBinding(TextBlock.TextProperty, b);
            Validate(_tb1);

            BindingOperations.ClearAllBindings(tb);

            b = new Binding("StringProperty");
            b.Source = _myDS;
            b.Mode = BindingMode.TwoWay;
            _myDS.StringProperty = "Two Way BindingExpression String";
            tb.SetBinding(TextBlock.TextProperty, b);
            Validate(_tb1);

            tb.Text = "Two Way BindingExpression Reverse Test";
            Validate(_tb1);

            BindingOperations.ClearAllBindings(_tb1);

            _tb2.Text = "Binding With Another TextBlock";
            b = new Binding();
            b.ElementName = "tb2";
            b.Path = new PropertyPath("Text");
            _tb1.SetBinding(TextBlock.TextProperty, b);
            dp.Children.Add(_tb1);
            dp.Children.Add(_tb2);
            _w1.Show();
            CommonFunctionality.FlushDispatcher();
            _myDS.StringProperty = _tb1.Text;
            Validate(_tb1);

            BindingOperations.ClearAllBindings(_tb1);

            _tb2.Text = "Two Way BindingExpression With Another TextBlock";
            b = new Binding();
            b.ElementName = "tb2";
            b.Path = new PropertyPath("Text");
            b.Mode = BindingMode.TwoWay;
            _tb1.SetBinding(TextBlock.TextProperty, b);
            CommonFunctionality.FlushDispatcher();
            _myDS.StringProperty = _tb1.Text;
            Validate(_tb1);

            _tb1.Text = "Two Way BindingExpression Reverse Test With Another TextBlock";
            CommonFunctionality.FlushDispatcher();
            _myDS.StringProperty = _tb2.Text;
            Validate(_tb2);


            if (Log.Result == TestResult.Fail)
            {
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }

        private void Validate(TextBlock tb)
        {
            BindingExpression b = null;
            if (tb == null)
            {
                tb = _tb1;
            }
            if (tb == null)
            {
                Log.Result = TestResult.Fail;
                LogComment("TextBlock Expected, not found ");
            }

            b = BindingOperations.GetBindingExpression(tb, TextBlock.TextProperty);
            if (tb.Text != _myDS.StringProperty)
            {
                Log.Result = TestResult.Fail;
                LogComment("Text Content BindingExpression Failed. Values Do Not Match: " + tb.Text
                    + ", " + ((MyDataSource)b.ParentBinding.Source).StringProperty);
            }
        }
        #endregion
    }
}
