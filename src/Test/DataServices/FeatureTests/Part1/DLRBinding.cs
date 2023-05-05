// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Test scenarios to cover binding to DLR objects
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Binding", "DLRBinding", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
    public class DLRBinding : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TextBox _simpleBindingTextBox;
        private TextBox _validationsTextBox;        
        private TextBox _multiBindingsTextBox;
        private TextBox _priorityBindingsTextBox;
        private TextBox _indexersTextBoxA;
        private TextBox _indexersTextBoxB;
        private TextBox _removePropertyTextBox;
        private TextBox _twoWayBindingTextBoxA;
        private TextBox _twoWayBindingTextBoxB;
        private TextBox _nestedPropertiesTextBox;
        private TextBox _sharesProposedTextBox;

        #endregion

        #region Constructors

        public DLRBinding()
            : base(@"DLRBinding.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(SimpleBinding);            
            RunSteps += new TestStep(MultiBindings);
            RunSteps += new TestStep(PriorityBindings);            
            RunSteps += new TestStep(NestedProperties);
            RunSteps += new TestStep(SharesProposed);
            RunSteps += new TestStep(Validations);
            RunSteps += new TestStep(TwoWayBinding);
            RunSteps += new TestStep(RemoveProperty);            
            RunSteps += new TestStep(Indexers);
            
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");
            _simpleBindingTextBox = (TextBox)RootElement.FindName("simpleBindingTextBox");
            _validationsTextBox = (TextBox)RootElement.FindName("validationsTextBox");
            _multiBindingsTextBox = (TextBox)RootElement.FindName("multiBindingsTextBox");
            _priorityBindingsTextBox = (TextBox)RootElement.FindName("priorityBindingsTextBox");
            _indexersTextBoxA = (TextBox)RootElement.FindName("indexersTextBoxA");
            _indexersTextBoxB = (TextBox)RootElement.FindName("indexersTextBoxB");
            _removePropertyTextBox = (TextBox)RootElement.FindName("removePropertyTextBox");
            _twoWayBindingTextBoxA = (TextBox)RootElement.FindName("twoWayBindingTextBoxA");
            _twoWayBindingTextBoxB = (TextBox)RootElement.FindName("twoWayBindingTextBoxB");
            _nestedPropertiesTextBox = (TextBox)RootElement.FindName("nestedPropertiesTextBox");
            _sharesProposedTextBox = (TextBox)RootElement.FindName("sharesProposedTextBox");                        
            
            return TestResult.Pass;
        }

        // Verify if a simple binding works with DLR objects.
        private TestResult SimpleBinding()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic simpleBindingDo = new MyDynamicObject();
            simpleBindingDo.TempProperty = "SimpleBinding Value";
            _myStackPanel.DataContext = simpleBindingDo;

            WaitForPriority(DispatcherPriority.DataBind);            

            // Verify Binding.
            if (_simpleBindingTextBox.Text != "SimpleBinding Value" || simpleBindingDo.TempProperty != "SimpleBinding Value")
            {
                LogComment("Simple Binding Test case failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Verify if a property with Validations on it reports errors correctly.
        private TestResult Validations()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic validationsDo = new MyDynamicObject();
            validationsDo.TempProperty = "Validations Value";
            _myStackPanel.DataContext = validationsDo;

            WaitForPriority(DispatcherPriority.DataBind);

            // Verify Binding.
            if (_validationsTextBox.Text != "Validations Value" || validationsDo.TempProperty != "Validations Value")
            {
                LogComment("Validations Test case failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            // Attempt to update the source with an invalid value.
            _validationsTextBox.Text = "Too Long!";
            BindingOperations.GetBindingExpression(_validationsTextBox, TextBox.TextProperty).UpdateSource();
            _multiBindingsTextBox.Focus();

            WaitForPriority(DispatcherPriority.Render);

            // Verify that values or source were not updated.
            if (_validationsTextBox.Text != "Too Long!" || validationsDo.TempProperty != "Validations Value")
            {
                LogComment("Validations Test case failed. Invalid values caused update!");
                return TestResult.Fail;
            }

            // Verify that validation errors exist.
            if (BindingOperations.GetBindingExpression(_validationsTextBox, TextBox.TextProperty).HasError != true)
            {
                LogComment("Validations Test case failed. No validation errors reported!");
                return TestResult.Fail;
            }

            // Update with legal value and verify update.
            _validationsTextBox.Text = "Legal!";
            BindingOperations.GetBindingExpression(_validationsTextBox, TextBox.TextProperty).UpdateSource();
                        
            if (_validationsTextBox.Text != "Legal!" || validationsDo.TempProperty != "Legal!")
            {
                LogComment("Validations Test case failed. Valid values weren't updated!");
                return TestResult.Fail;
            }

            // Verify that validation errors exist.
            if (BindingOperations.GetBindingExpression(_validationsTextBox, TextBox.TextProperty).HasError != false)
            {
                LogComment("Validations Test case failed. Validation errors reported even with legal values!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Verify that Indexers work with DLR objects.
        private TestResult Indexers()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic indexersDo = new MyDynamicObject();
            indexersDo.AddItem("item 0");
            indexersDo[0] = "Indexers Value";
            _myStackPanel.DataContext = indexersDo;
                      
            WaitForPriority(DispatcherPriority.DataBind);

            // Verify Binding.
            if (_indexersTextBoxA.Text != "Indexers Value" || indexersDo[0] != "Indexers Value")
            {
                LogComment("Indexers Test case failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            // Test for multiple indexers.
            _indexersTextBoxB.Text = "Test MultipleIndexers";
            BindingOperations.GetBindingExpression(_indexersTextBoxB, TextBox.TextProperty).UpdateSource();

            WaitForPriority(DispatcherPriority.DataBind);
            
            // Verify Binding.
            if (indexersDo[0, 1, 2] != "Test MultipleIndexers")
            {
                LogComment("MultipleIndexers Test case failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Verify that removing a property works with DLR objects.
        private TestResult RemoveProperty()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic removePropertyDo = new MyDynamicObject();
            removePropertyDo.TempProperty = "RemoveProperty Value";
            _myStackPanel.DataContext = removePropertyDo;

            WaitForPriority(DispatcherPriority.DataBind);

            // Verify Binding.
            if (_removePropertyTextBox.Text != "RemoveProperty Value" || removePropertyDo.TempProperty != "RemoveProperty Value")
            {
                LogComment("RemoveProperty Binding Test case initial attach to property failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            // Try removing the new property.            
            removePropertyDo.DeleteProperty("TempProperty");

            // Verify that bindings are not reporting values now.
            if (_removePropertyTextBox.Text != "")
            {                
                LogComment("RemoveProperty Binding Test case final attach to property failed. Values should have been empty.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Verify that MultiBindings work with DLR objects.
        private TestResult MultiBindings()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic multiBindingsDo = new MyDynamicObject();
            multiBindingsDo.PropertyA = "MultiBindings";
            multiBindingsDo.PropertyB = "Value";
            _myStackPanel.DataContext = multiBindingsDo;

            WaitForPriority(DispatcherPriority.DataBind);
            
            // Verify Binding.
            if (_multiBindingsTextBox.Text != "MultiBindings Value")
            {
                LogComment("MultiBindings Test case failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Verify that PriorityBindings work with DLR objects.
        private TestResult PriorityBindings()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic priorityBindingsDo = new MyDynamicObject();
            priorityBindingsDo.RealProperty = "PriorityBindings Value";
            _myStackPanel.DataContext = priorityBindingsDo;

            WaitForPriority(DispatcherPriority.DataBind);

            // Verify Binding.
            if (_priorityBindingsTextBox.Text != "PriorityBindings Value" || priorityBindingsDo.RealProperty != "PriorityBindings Value")
            {
                LogComment("PriorityBindings Test case failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Verify that TwoWay Bindings work with DLR objects.
        private TestResult TwoWayBinding()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic twoWayBindingDo = new MyDynamicObject();
            twoWayBindingDo.TempProperty = "TwoWayBinding Value";
            _myStackPanel.DataContext = twoWayBindingDo;

            WaitForPriority(DispatcherPriority.DataBind);

            // Verify Binding.
            if (_twoWayBindingTextBoxA.Text != "TwoWayBinding Value" || _twoWayBindingTextBoxB.Text != "TwoWayBinding Value")
            {
                LogComment("TwoWayBinding Test case failed. Initial values were not set correctly on textboxes.");
                return TestResult.Fail;
            }

            // Update A to see if B's source is updated.
            _twoWayBindingTextBoxA.Text = "New Value";            
            BindingOperations.GetBindingExpression(_twoWayBindingTextBoxA, TextBox.TextProperty).UpdateSource();
            BindingOperations.GetBindingExpression(_twoWayBindingTextBoxB, TextBox.TextProperty).UpdateSource();

            WaitForPriority(DispatcherPriority.Render);

            // Verify that B and B's source are updated.
            if (_twoWayBindingTextBoxB.Text != "New Value" || twoWayBindingDo.TempProperty != "New Value")
            {
                LogComment("TwoWayBinding Test case failed. Source was not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Verify that SharesPropsedValues work with DLR objects.
        private TestResult SharesProposed()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic sharesProposedDo = new MyDynamicObject();
            sharesProposedDo.TempProperty = "SharesProposed Value";
            _myStackPanel.DataContext = sharesProposedDo;

            WaitForPriority(DispatcherPriority.DataBind);

            // Verify Binding.
            if (_sharesProposedTextBox.Text != "SharesProposed Value" || sharesProposedDo.TempProperty != "SharesProposed Value")
            {
                LogComment("SharesProposed Test case failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            // Change Text
            string proposedName = _sharesProposedTextBox.Text = "Toronto";
            BindingBase binding = BindingOperations.GetBindingBase(_sharesProposedTextBox, TextBox.TextProperty);

            // Replace with non-editable template.
            int index = _myStackPanel.Children.IndexOf(_sharesProposedTextBox);
            _myStackPanel.Children.RemoveAt(index);
            TextBlock sharesProposedTextBlock = new TextBlock();
            _myStackPanel.Children.Insert(index, sharesProposedTextBlock);
            sharesProposedTextBlock.SetBinding(TextBlock.TextProperty, binding);

            // Check if proposed values were kept.            
            if (sharesProposedTextBlock.Text != proposedName)
            {
                LogComment("Proposed values were not kept intact through template change.");
                LogComment("Actual: " + sharesProposedTextBlock.Text);
                LogComment("Expected: " + proposedName);
                return TestResult.Fail;
            }

            // Verify that we can commit good data
            if (_myStackPanel.BindingGroup.CommitEdit() != true)
            {
                LogComment("We were unable to commit good data.");
                return TestResult.Fail;
            }

            // Verify that data was updated correctly
            if (sharesProposedDo.TempProperty != proposedName)
            {
                LogComment("Source was not updated correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Verify that NestedProperties work with DLR objects.
        private TestResult NestedProperties()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic nestedPropertiesDo = new MyDynamicObject();
            nestedPropertiesDo.LevelOne = new MyDynamicObject();
            nestedPropertiesDo.LevelOne.LevelTwo = new MyDynamicObject();
            nestedPropertiesDo.LevelOne.LevelTwo.LevelThree = "NestedProperties Value";
            _myStackPanel.DataContext = nestedPropertiesDo;

            WaitForPriority(DispatcherPriority.DataBind);

            // Verify Binding.
            if (_nestedPropertiesTextBox.Text != "NestedProperties Value")
            {
                LogComment("NestedProperties Test case failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion

    }

    #region Helper Classes

    public class MultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return DependencyProperty.UnsetValue;

            if (targetType != typeof(String))
                return DependencyProperty.UnsetValue;

            return (values[0] as String) + " " + (values[1] as String);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string str = value as string;
            if (str == null)
                return null;

            if (str == "FailConvert")
            {
                return new object[] { DependencyProperty.UnsetValue, DependencyProperty.UnsetValue };
            }

            String[] values = str.Split(null);

            object[] returnValues = new object[2];
            returnValues[0] = values[0];
            returnValues[1] = values[1];

            return returnValues;
        }
    }

    #endregion
}
