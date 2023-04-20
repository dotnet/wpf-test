// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Collections.Generic;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test : Verify that Indexer is able to accept integer parameters when expecting object types.    
    /// </description>

    /// </summary>            
    [Test(1, "Binding", "RegressionIntegerIndexerExpectsObject")]
    public class RegressionIntegerIndexerExpectsObject : AvalonTest
    {
        #region Private Data

        TextBox _textBox;

        #endregion


        #region Public Members

        public RegressionIntegerIndexerExpectsObject()
        {
            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(Verify);
        }

        public TestResult SetUp()
        {
            Status("SetUp");

            MySource source = new MySource();
            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath("[(0)]", 0);
            binding.Mode = BindingMode.OneWay;

            _textBox = new TextBox();
            _textBox.SetBinding(TextBox.TextProperty, binding);

            LogComment("SetUp Successful");
            return TestResult.Pass;
        }

        public TestResult Verify()
        {
            Status("Verifying Test");

            if (_textBox.Text != "Aaron")
            {
                LogComment("Were not able to pass integer as a parameter when indexer was expecting an object.");
                return TestResult.Fail;
            }

            LogComment("Indexer successfully accepted integer as a parameter when the expected type was onject.");
            return TestResult.Pass;
        }

        #endregion


        #region Customized source to perform test on

        public class MySource
        {
            List<string> _list = new List<string>();

            public MySource()
            {
                this._list.Add("Aaron");
                this._list.Add("Beatriz");
                this._list.Add("Carol");
                this._list.Add("Dennis");
                this._list.Add("Erin");
            }

            public object this[object index]
            {
                get
                {
                    int intIndex = Int32.Parse(index.ToString());
                    return _list[intIndex];
                }
            }
        }

        #endregion
    }    
}
