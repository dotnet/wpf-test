// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Provides exception coverage for DataTrigger.cs and MultiDataTrigger.cs.
    /// </description>
    /// <activeBugs>

    /// </activeBugs>
    /// </summary>
    [Test(2, "Styles", "DataTriggersCoverage")]
    public class DataTriggersCoverage : XamlTest
	{
        private Style _listBoxItemStyle;
        private ListBox _lb;
        private ObjectDataProvider _writers;
        private Style _listBoxItemStyleMulti;

        public DataTriggersCoverage()
            : base(@"DataTriggersCoverage.xaml")
        {
			InitializeSteps += new TestStep(Setup);
            // DataTriggers
            //RunSteps += new TestStep(TestGettersAndSetters);
            RunSteps += new TestStep(SetBindingAfterSealed);
            RunSteps += new TestStep(SetValueAfterSealed);
            RunSteps += new TestStep(SetValueToMarkupExtension);
            RunSteps += new TestStep(SetValueToExpression);
            RunSteps += new TestStep(TestAddChildNull);
            RunSteps += new TestStep(TestAddChildNotSetter);
            RunSteps += new TestStep(TestAddTextNotWhiteSpace);
            RunSteps += new TestStep(TestAddTextWhiteSpace);
            // MultiDataTriggers
            RunSteps += new TestStep(TestSettersDpMulti);
            RunSteps += new TestStep(TestConditionsDpMulti);
            RunSteps += new TestStep(TestAddChildNullMulti);
            RunSteps += new TestStep(TestAddChildNotSetterMulti);
            RunSteps += new TestStep(TestAddChildEmptySetterMulti);
            RunSteps += new TestStep(TestAddChildSetterMulti);
            RunSteps += new TestStep(TestAddTextNotWhiteSpaceMulti);
            RunSteps += new TestStep(TestAddTextWhiteSpaceMulti);
        }

        private TestResult Setup()
        {
            Status("Setup");

            _listBoxItemStyle = RootElement.Resources["listBoxItemStyle"] as Style;
            if (_listBoxItemStyle == null)
            {
                LogComment("Fail - Unable to reference Style listBoxItemStyle");
                return TestResult.Fail;
            }
            _listBoxItemStyleMulti = RootElement.Resources["listBoxItemStyleMulti"] as Style;
            if (_listBoxItemStyleMulti == null)
            {
                LogComment("Fail - Unable to reference Style listBoxItemStyleMulti");
                return TestResult.Fail;
            }
            _lb = LogicalTreeHelper.FindLogicalNode(RootElement, "lb") as ListBox;
            if (_lb == null)
            {
                LogComment("Fail - Unable to reference ListBox lb");
                return TestResult.Fail;
            }
            _writers = RootElement.Resources["writers"] as ObjectDataProvider;
            if (_writers == null)
            {
                LogComment("Fail - Not able to reference ObjectDataProvider writers");
                return TestResult.Fail;
            }

            _lb.ItemContainerStyle = _listBoxItemStyle;

            Status("Setup was successful");
            return TestResult.Pass;
        }

        #region DataTrigger
        private TestResult TestGettersAndSetters()
        {
            Status("TestGettersAndSetters");

            DataTrigger dataTrigger = new DataTrigger();
            Binding b = new Binding("FirstName");
            dataTrigger.Binding = b;
            if (dataTrigger.Binding != b)
            {
                LogComment("Fail - Binding Getter/Setter did not work correctly");
                return TestResult.Fail;
            }
            dataTrigger.Value = "Carl";
            if (dataTrigger.Value.ToString() != "Carl")
            {
                LogComment("Fail - Value Getter/Setter did not work correctly");
                return TestResult.Fail;
            }
            SetterBaseCollection setters = dataTrigger.Setters;
            if (setters == null)
            {
                LogComment("Fail - Setters should return an empty collection, not null");
                return TestResult.Fail;
            }
            if (setters.Count != 0)
            {
                LogComment("Fail - Setters should return an empty collection, not a collection with count > 0");
                return TestResult.Fail;
            }

            Status("TestGettersAndSetters was successful");
            return TestResult.Pass;
        }

        private TestResult SetBindingAfterSealed()
        {
            Status("SetBindingAfterSealed");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));

            TriggerCollection triggers = _listBoxItemStyle.Triggers;
            DataTrigger dataTrigger = triggers[0] as DataTrigger;
            dataTrigger.Binding = new Binding("FirstName");

            return TestResult.Pass;
        }

        private TestResult SetValueAfterSealed()
        {
            Status("SetValueAfterSealed");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));

            TriggerCollection triggers = _listBoxItemStyle.Triggers;
            DataTrigger dataTrigger = triggers[0] as DataTrigger;
            dataTrigger.Value = "Carl";

            return TestResult.Pass;
        }

        private TestResult SetValueToMarkupExtension()
        {
            Status("SetValueToMarkupExtension");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(ArgumentException));

            DataTrigger dataTrigger = new DataTrigger();
            dataTrigger.Value = new StaticResourceExtension();

            return TestResult.Pass;
        }

        private TestResult SetValueToExpression()
        {
            Status("SetValueToExpression");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(ArgumentException));

            Binding binding = new Binding();
            binding.Source = _writers;
            binding.Path = new PropertyPath("[0].FirstName");
            Button btn = new Button();
            btn.SetBinding(Button.ContentProperty, binding);
            BindingExpression exp = btn.GetBindingExpression(Button.ContentProperty);

            DataTrigger dataTrigger = new DataTrigger();
            dataTrigger.Value = exp;

            return TestResult.Pass;
        }

        private TestResult TestAddChildNull()
        {
            Status("TestAddChildNull");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));

            DataTrigger dataTrigger = new DataTrigger();
            ((IAddChild)dataTrigger).AddChild(null);

            return TestResult.Pass;
        }

        private TestResult TestAddChildNotSetter()
        {
            Status("TestAddChildNotSetter");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(ArgumentException));

            DataTrigger dataTrigger = new DataTrigger();
            ((IAddChild)dataTrigger).AddChild(new TextBox());

            return TestResult.Pass;
        }

        private TestResult TestAddTextNotWhiteSpace()
        {
            Status("TestAddTextNotWhiteSpace");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(ArgumentException));

            DataTrigger dataTrigger = new DataTrigger();
            ((IAddChild)dataTrigger).AddText("hello");

            return TestResult.Pass;
        }

        private TestResult TestAddTextWhiteSpace()
        {
            Status("TestAddTextWhiteSpace");

            // no exception is thrown when adding white space

            DataTrigger dataTrigger = new DataTrigger();
            ((IAddChild)dataTrigger).AddText(" ");

            Status("TestAddTextWhiteSpace was successful");
            return TestResult.Pass;
        }

        #endregion

        #region MultiDataTriggers
        private TestResult TestSettersDpMulti()
        {
            Status("TestSettersDpMulti");

            TriggerCollection triggers = _listBoxItemStyleMulti.Triggers;
            MultiDataTrigger multiDataTrigger = triggers[0] as MultiDataTrigger;
            SetterBaseCollection setters = multiDataTrigger.Setters;

            if (setters.Count != 1)
            {
                LogComment("Fail - Should have 1 setter, instead it has " + setters.Count);
                return TestResult.Fail;
            }
            Setter setter = setters[0] as Setter;
            if (setter.Property != ListBoxItem.BackgroundProperty)
            {
                LogComment("Fail - Property in setter is not correct");
                return TestResult.Fail;
            }
            if (setter.Value != Brushes.Brown)
            {
                LogComment("Fail - Value in setter is not correct");
                return TestResult.Fail;
            }

            MultiDataTrigger mdtNoSetters = new MultiDataTrigger();
            SetterBaseCollection emptySbc = mdtNoSetters.Setters;
            if (emptySbc.Count != 0)
            {
                LogComment("Fail - Setters should return an empty collection");
                return TestResult.Fail;
            }

            Status("TestSettersDpMulti was successful");
            return TestResult.Pass;
        }

        private TestResult TestConditionsDpMulti()
        {
            Status("TestConditionsDpMulti");

            TriggerCollection triggers = _listBoxItemStyleMulti.Triggers;
            MultiDataTrigger multiDataTrigger = triggers[0] as MultiDataTrigger;
            ConditionCollection conditions = multiDataTrigger.Conditions;

            if (conditions.Count != 2)
            {
                LogComment("Fail - There should be 2 conditions");
                return TestResult.Fail;
            }
            Condition firstCondition = conditions[0] as Condition;
            if (((Binding)firstCondition.Binding).Path.Path != "FirstName")
            {
                LogComment("Fail - The first condition's path is incorrect");
                return TestResult.Fail;
            }
            if (firstCondition.Value.ToString() != "Carl")
            {
                LogComment("Fail - The first condition's value is incorrect");
                return TestResult.Fail;
            }
            Condition secondCondition = conditions[1] as Condition;
            if (((Binding)secondCondition.Binding).Path.Path != "LastName")
            {
                LogComment("Fail - The second condition's path is incorrect");
                return TestResult.Fail;
            }
            if (secondCondition.Value.ToString() != "Sagan")
            {
                LogComment("Fail - The second condition's value is incorrect");
                return TestResult.Fail;
            }

            Status("TestConditionsDpMulti was successful");
            return TestResult.Pass;
        }

        private TestResult TestAddChildNullMulti()
        {
            Status("TestAddChildNullMulti");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));

            MultiDataTrigger multiDataTrigger = new MultiDataTrigger();
            ((IAddChild)multiDataTrigger).AddChild(null);

            Status("TestAddChildNullMulti was successful");
            return TestResult.Pass;
        }

        private TestResult TestAddChildNotSetterMulti()
        {
            Status("TestAddChildNotSetterMulti");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(ArgumentException));

            MultiDataTrigger multiDataTrigger = new MultiDataTrigger();
            ((IAddChild)multiDataTrigger).AddChild(new TextBox());

            Status("TestAddChildNotSetterMulti was successful");
            return TestResult.Pass;
        }

        private TestResult TestAddChildEmptySetterMulti()
        {
            Status("TestAddChildEmptySetterMulti");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(ArgumentException));

            MultiDataTrigger multiDataTrigger = new MultiDataTrigger();
            ((IAddChild)multiDataTrigger).AddChild(new Setter());

            // The check for empty Setter now only happens when the Setter is sealed, which we can make happen
            // by using it in a Style. - see 
            Style style = new Style();
            style.Triggers.Add(multiDataTrigger);
            TextBlock tb = new TextBlock();
            tb.Style = style;

            Status("TestAddChildEmptySetterMulti was successful");
            return TestResult.Pass;
        }

        private TestResult TestAddChildSetterMulti()
        {
            Status("TestAddChildSetterMulti");

            MultiDataTrigger multiDataTrigger = new MultiDataTrigger();
            ((IAddChild)multiDataTrigger).AddChild(new Setter(ListBoxItem.ForegroundProperty, Brushes.Pink));

            if (((Setter)multiDataTrigger.Setters[0]).Property != ListBoxItem.ForegroundProperty)
            {
                LogComment("Fail - Setter's property should be ListBoxItem.ForegroundProperty");
                return TestResult.Fail;
            }
            if (((Setter)multiDataTrigger.Setters[0]).Value != Brushes.Pink)
            {
                LogComment("Fail - Setter's value should be Brushes.Pink");
                return TestResult.Fail;
            }

            Status("TestAddChildSetterMulti was successful");
            return TestResult.Pass;
        }

        private TestResult TestAddTextNotWhiteSpaceMulti()
        {
            Status("TestAddTextNotWhiteSpaceMulti");
            WaitForPriority(DispatcherPriority.Render);

            SetExpectedErrorTypeInStep(typeof(ArgumentException));

            MultiDataTrigger multiDataTrigger = new MultiDataTrigger();
            ((IAddChild)multiDataTrigger).AddText("hello");

            Status("TestAddTextNotWhiteSpaceMulti was successful");
            return TestResult.Pass;
        }

        private TestResult TestAddTextWhiteSpaceMulti()
        {
            Status("TestAddTextWhiteSpaceMulti");

            // this doesn't throw
            MultiDataTrigger multiDataTrigger = new MultiDataTrigger();
            ((IAddChild)multiDataTrigger).AddText(" ");

            Status("TestAddTextWhiteSpaceMulti was successful");
            return TestResult.Pass;
        }

        #endregion

    }
}
