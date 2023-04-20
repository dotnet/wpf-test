// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{

    /// <summary>
	/// <description>
    /// Tests Default Transformer
    /// </description>
    /// </summary>
    [Test(1, "Binding", "TransformerBvt")]
    public class TransformerBvt : WindowTest
    {
        HappyMan _happy;
        Dwarf _dwarf;

        public TransformerBvt()
        {
            InitializeSteps += new TestStep(CreateTree);

            RunSteps += new TestStep(ConvertFromSource);
            RunSteps += new TestStep(ConvertFromTarget);
        }

        private TestResult CreateTree()
        {
            Status("Creating the Element Tree");

            _happy = new HappyMan();

            _happy.Name = "George";
            _happy.Position = new Point(100,100);
            _happy.Width = 200;
            _happy.Height = 200;

            Window.Content = _happy;

            _dwarf = new Dwarf("Sleepy", "Yellow", 30, 500, Colors.Purple, new Point(2, 5), true);
            _happy.DataContext = _dwarf;

            return TestResult.Pass;
        }

        // Validate conversion from string to Color type
        private TestResult ConvertFromSource()
        {
            Status("Setting binding on property with target type converter.");
            Binding bind = new Binding("EyeColor");
            bind.Mode = BindingMode.TwoWay;
            _happy.SetBinding(HappyMan.EyeColorProperty, bind);

            if(_happy.EyeColor != Colors.Yellow)
            {
                LogComment("Failed to transform from Color to string.");
            }
            return TestResult.Pass;
        }

        // Validate conversion from Color to string type
        private TestResult ConvertFromTarget()
        {
            Status("Setting binding on property with source type converter.");
            Binding bind = new Binding("SkinColor");
            bind.Mode = BindingMode.TwoWay;
            _happy.SetBinding(HappyMan.NameProperty, bind);
            _happy.Name = "Green";

            if(_dwarf.SkinColor != Colors.Green)
            {
                LogComment("Failed to transform from string to Color.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
