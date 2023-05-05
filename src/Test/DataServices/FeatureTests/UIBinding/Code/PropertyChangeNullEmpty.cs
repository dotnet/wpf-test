// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This test confirms that raising a property change with Null or empty string invalidates 
    /// all properties in the source class.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(2, "Binding", "PropertyChangeNullEmpty")]
    public class PropertyChangeNullEmpty : XamlTest
    {
        TextBlock _tb1;
        TextBlock _tb2;
        TextBlock _tb3;
        PropertyChangeInvalidatesAll _source;

        public PropertyChangeNullEmpty()
            : base(@"PropertyChangeNullEmpty.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(ChangeProp1);
            RunSteps += new TestStep(ChangeProp2);
            RunSteps += new TestStep(ChangeProp3);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _tb1 = (TextBlock)(LogicalTreeHelper.FindLogicalNode(RootElement, "tb1"));
            _tb2 = (TextBlock)(LogicalTreeHelper.FindLogicalNode(RootElement, "tb2"));
            _tb3 = (TextBlock)(LogicalTreeHelper.FindLogicalNode(RootElement, "tb3"));
            _source = (PropertyChangeInvalidatesAll)(RootElement.Resources["source"]);

            Util.AssertEquals(_tb1.Text, Convert.ToString(_source.Prop1, CultureInfo.InvariantCulture));
            Util.AssertEquals(_tb2.Text, Convert.ToString(_source.Prop2, CultureInfo.InvariantCulture));
            Util.AssertEquals(_tb3.Text, Convert.ToString(_source.Prop3, CultureInfo.InvariantCulture));

            return TestResult.Pass;
        }

        private TestResult ChangeProp1()
        {
            Status("ChangeProp1");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _source.Prop1 = 20;
            Util.AssertEquals(_tb1.Text, "10");
            Util.AssertEquals(_tb2.Text, Convert.ToString(_source.Prop2, CultureInfo.InvariantCulture));
            Util.AssertEquals(_tb3.Text, Convert.ToString(_source.Prop3, CultureInfo.InvariantCulture));

            return TestResult.Pass;
        }

        private TestResult ChangeProp2()
        {
            Status("ChangeProp2");
            _source.Prop2 = "Hello again";
            Util.AssertEquals(_tb1.Text, Convert.ToString(_source.Prop1, CultureInfo.InvariantCulture));
            Util.AssertEquals(_tb2.Text, Convert.ToString(_source.Prop2, CultureInfo.InvariantCulture));
            Util.AssertEquals(_tb3.Text, Convert.ToString(_source.Prop3, CultureInfo.InvariantCulture));

            return TestResult.Pass;
        }

        private TestResult ChangeProp3()
        {
            Status("ChangeProp3");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _source.Prop1 = 30;
            Util.AssertEquals(_tb1.Text, "20");
            Util.AssertEquals(_tb2.Text, Convert.ToString(_source.Prop2, CultureInfo.InvariantCulture));
            Util.AssertEquals(_tb3.Text, Convert.ToString(_source.Prop3, CultureInfo.InvariantCulture));
            _source.Prop3 = 3.3;
            Util.AssertEquals(_tb1.Text, Convert.ToString(_source.Prop1, CultureInfo.InvariantCulture));
            Util.AssertEquals(_tb2.Text, Convert.ToString(_source.Prop2, CultureInfo.InvariantCulture));
            Util.AssertEquals(_tb3.Text, Convert.ToString(_source.Prop3, CultureInfo.InvariantCulture));

            return TestResult.Pass;
        }

    }
}
