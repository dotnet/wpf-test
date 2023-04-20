// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Graphics;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests Binding Groups
    /// </description>
    /// </summary>
    [Test(0, "Binding", "BindingGroupTest")]
    public class BindingGroupTest : XamlTest
    {
        // TODO:
        // - GetValue/TryGetValue call during each of the ValidationSteps (raw vs converted value, for example)
        // - ValidationSteps on ValidationRules on Multi/PriorityBinding within a BindingGroup
        // - Have ItemBindingGroup on ItemsControl contain expressions and items
        // - ValidationAdorner appears when ValidationRule is on a binding instead of the BindingGroup

        #region Private Data

        BindingGroup _bindingGroup;

        GenericValidationRule _rawProposedValueStep = new GenericValidationRule(ValidationStep.RawProposedValue, true);
        GenericValidationRule _convertedProposedValueStep = new GenericValidationRule(ValidationStep.ConvertedProposedValue, true);
        GenericValidationRule _updatedValueStep = new GenericValidationRule(ValidationStep.UpdatedValue, true);
        GenericValidationRule _commitedValueStep = new GenericValidationRule(ValidationStep.CommittedValue, true);

        private enum ValidationAmount
        {
            ValidationWithoutUpdate, UpdateSources, CommitEdit
        }

        private enum JoinMethod
        {
            Implicit, Explicit, BindingExpressions, None
        }

        #endregion

        #region Constructors

        public BindingGroupTest()
            : base(@"BindingGroups.xaml")
        {
            InitializeSteps += new TestStep(Setup);

            // Editing Scenarios
            RunSteps += new TestStep(CanRestoreValues);
            RunSteps += new TestStep(EditingIEditableObject);
            RunSteps += new TestStep(CancelEditBindingRefetch);

            // GetValue Scenarios
            RunSteps += new TestStep(BindingGroupGetValue);
            RunSteps += new TestStep(BindingGroupTryGetValue);

            // Other BindingGroup API
            RunSteps += new TestStep(BindingGroupBindingExpressionsAdd);
            RunSteps += new TestStep(BindingGroupBindingExpressionsRemove);
            RunSteps += new TestStep(BindingGroupBindingExpressionsClear);
            RunSteps += new TestStep(BindingGroupItems);
            RunSteps += new TestStep(BindingGroupNotifyOnValidationError);

            // BindingGroup Membership
            RunSteps += new TestStep(BindingGroupImplicitMembership);
            RunSteps += new TestStep(BindingGroupExplicitMembership);
            RunSteps += new TestStep(BindingGroupMultiPriorityBindingMembership);

            // ValidationSteps
            RunSteps += new TestStep(BindingGroupValidationRulesSteps);
            RunSteps += new TestStep(BindingGroupValidationStepMisc);

            RunSteps += new TestStep(BindingGroupForwarding);

            RunSteps += new TestStep(BindingGroupUpdateSourceTriggerDefaultValue);

            RunSteps += new TestStep(BindingGroupAdorner);
        }

        #endregion Constructors

        #region Private Members

        private TestResult Setup()
        {
            UniformGrid uniformGrid = (UniformGrid)RootElement;
            _bindingGroup = new BindingGroup();
            _bindingGroup.Name = "groupToJoin";
            uniformGrid.BindingGroup = _bindingGroup;

            // some BindingGroup functionality doesn't kick in until layout has run,
            // so do some layout now
            uniformGrid.InvalidateMeasure();
            WaitForPriority(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        /// <summary>
        /// Validate BindingGroup.CanRestoreValues API
        /// </summary>
        /// <returns></returns>
        private TestResult CanRestoreValues()
        {
            _bindingGroup.BindingExpressions.Clear();

            // Right now there are no binding expressions in the BindingGroup, so CanRestoreValues should be true
            if (!_bindingGroup.CanRestoreValues) return TestResult.Fail;

            // Add a binding expression to the group whose data item is IEditableObject, so CanRestoreValues should remain true
            GenerateTextBox(new Place("Brea", "CA"));

            // Right now there is one binding expression whose source is IEO, so CanRestoreValues should be true
            if (!_bindingGroup.CanRestoreValues) return TestResult.Fail;

            // Add a binding expression to the group whose data item is not IEditableObject, so CanRestoreValues should become false
            GenerateTextBox(new Country("Australia", "Sydney", new DateTime(), 1000000, GovernmentType.Democracy));

            // Now one source is IEO and the other is not, so CanRestoreValues should be false
            if (_bindingGroup.CanRestoreValues) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Validate BindingGroup Editing APIs call corresponding IEditableObject APIs:
        /// - BindingGroup.BeginEdit
        /// - BindingGroup.CancelEdit
        /// - BindingGroup.CommitEdit
        /// </summary>
        /// <returns></returns>
        private TestResult EditingIEditableObject()
        {
            _bindingGroup.BindingExpressions.Clear();
            Place place = new Place("Brea", "CA");
            TextBox placeTextBox = GenerateTextBox(place);

            // Call BeginEdit and verify IEO.BeginEdit on the place object was called
            _bindingGroup.BeginEdit();
            if (place.CalledInterfaceAPI.Count != 1 || place.CalledInterfaceAPI[0] != Place.InterfaceAPI.BeginEdit) return TestResult.Fail;

            // Call CancelEdit and verify IEO.CancelEdit on the place object was called
            place.CalledInterfaceAPI.Clear();
            _bindingGroup.CancelEdit();
            if (place.CalledInterfaceAPI.Count != 1 || place.CalledInterfaceAPI[0] != Place.InterfaceAPI.CancelEdit) return TestResult.Fail;

            // Call CommitEdit and verify IEO.CommitEdit on the place object was called
            _bindingGroup.BeginEdit();
            place.CalledInterfaceAPI.Clear();
            _bindingGroup.CommitEdit();
            if (place.CalledInterfaceAPI.Count != 1 || place.CalledInterfaceAPI[0] != Place.InterfaceAPI.EndEdit) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Validate BindingGroup.CancelEdit causes bindings to refetch their values
        /// </summary>
        /// <returns></returns>
        private TestResult CancelEditBindingRefetch()
        {
            _bindingGroup.BindingExpressions.Clear();

            Place place = new Place("Brea", "CA");
            TextBox placeTextBox = GenerateTextBox(place);

            Country country = new Country("Australia", "Sydney", new DateTime(), 1000000, GovernmentType.Democracy);
            TextBox countryTextBox = GenerateTextBox(country);

            // Dirty the UI values so we can validate that CancelEdit causes bindings to refetch their values
            placeTextBox.Text = "New Value for Brea";
            countryTextBox.Text = "New Value for Australia";

            // Call CancelEdit and verify that both bindings refetched their values
            _bindingGroup.CancelEdit();
            if (!placeTextBox.Text.Equals("Brea") || !countryTextBox.Text.Equals("Australia")) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Validate BindingGroup.GetValue API
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupGetValue()
        {
            _bindingGroup.BindingExpressions.Clear();

            Place p = new Place("Brea", "CA");
            GenerateTextBox(p);

            // Negative cases
            ExceptionHelper.ExpectException(delegate() { _bindingGroup.GetValue(new Place("Seattle", "WA"), "Name"); }, new ValueUnavailableException());
            ExceptionHelper.ExpectException(delegate() { _bindingGroup.GetValue(p, "State"); }, new ValueUnavailableException());
            ExceptionHelper.ExpectException(delegate() { _bindingGroup.GetValue(p, "Foo"); }, new ValueUnavailableException());
            ExceptionHelper.ExpectException(delegate() { _bindingGroup.GetValue(null, "Name"); }, new ValueUnavailableException());
            ExceptionHelper.ExpectException(delegate() { _bindingGroup.GetValue(p, null); }, new ValueUnavailableException());

            // Positive case
            if (!Object.Equals(_bindingGroup.GetValue(p, "Name"), "Brea")) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Validate BindingGroup.TryGetValue API
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupTryGetValue()
        {
            _bindingGroup.BindingExpressions.Clear();

            Place p = new Place("Brea", "CA");
            GenerateTextBox(p);

            object outObject;

            // Negative cases
            if (_bindingGroup.TryGetValue(new Place("Seattle", "WA"), "Name", out outObject)) return TestResult.Fail;
            if (_bindingGroup.TryGetValue(p, "State", out outObject)) return TestResult.Fail;
            if (_bindingGroup.TryGetValue(p, "Foo", out outObject)) return TestResult.Fail;
            if (_bindingGroup.TryGetValue(null, "Name", out outObject)) return TestResult.Fail;
            if (_bindingGroup.TryGetValue(p, null, out outObject)) return TestResult.Fail;

            // Positive case
            if (!_bindingGroup.TryGetValue(p, "Name", out outObject)) return TestResult.Fail;
            if (!Object.Equals(outObject, "Brea")) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify a binding expression can be added to a BindingGroup via the BindingExpressions API, and that it behaves correctly once done so.
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupBindingExpressionsAdd()
        {
            _bindingGroup.BindingExpressions.Clear();
            GenericValidationRule.Results.Clear();
            RootElement.DataContext = null;

            // Verify Add - a binding expression added to a BindingGroup via BindingExpression should be called during validation
            Place p = new Place("Brea", "CA");
            TextBox tb = GenerateTextBox(p, JoinMethod.None, true);

            // Since we didn't use the DataContext or BindingGroup, we shouldn't see any BindingExpressions/Items yet
            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
            if (_bindingGroup.Items.Count != 0) return TestResult.Fail;

            _bindingGroup.BindingExpressions.Add(tb.GetBindingExpression(TextBox.TextProperty));

            // But now we should see the BindingExpression, and Items should pick it up also.
            if (_bindingGroup.BindingExpressions.Count != 1) return TestResult.Fail;
            if (_bindingGroup.Items.Count != 1) return TestResult.Fail;

            // Verify that when we run validation we are running validation rules on the binding expression.
            if (GenericValidationRule.Results.Count != 0) return TestResult.Fail;

            // We need to dirty the value so validation is run
            tb.Text = "Foo";
            if (!_bindingGroup.ValidateWithoutUpdate()) return TestResult.Fail;

            if (GenericValidationRule.Results.Count != 1) return TestResult.Fail;

            // Once a BindingExpression is added to a BindingGroup via BindingExpressions property, it should go into Explicit update mode
            // (changing focus should no longer push the changed value back)
            TextBox textBoxSwitchFocus = new TextBox();
            ((UniformGrid)RootElement).Children.Add(textBoxSwitchFocus);

            tb.Focus();
            tb.Text = "Bar";
            textBoxSwitchFocus.Focus();

            if (p.Name != "Brea") return TestResult.Fail;

            RootElement.BindingGroup.UpdateSources();

            if (p.Name != "Bar") return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify BindingExpressions can be removed from the BindingGroup via the BindingExpressions property, and behave as expected once done so
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupBindingExpressionsRemove()
        {
            _bindingGroup.BindingExpressions.Clear();
            GenericValidationRule.Results.Clear();

            Place p = new Place("Brea", "CA");
            Place p2 = new Place("Brea", "CA");

            // We can remove BindingExpressions that were added via DataContext/BindingGroupName or via BindingExpressions
            TextBox bindingGroupNameTextBox = GenerateTextBox(p, JoinMethod.Explicit, true);
            TextBox bindingExpressionsTextBox = GenerateTextBox(p2, JoinMethod.BindingExpressions, true);

            if (_bindingGroup.BindingExpressions.Count != 2) return TestResult.Fail;
            if (_bindingGroup.Items.Count != 2) return TestResult.Fail;

            _bindingGroup.BindingExpressions.Remove(bindingGroupNameTextBox.GetBindingExpression(TextBox.TextProperty));
            _bindingGroup.BindingExpressions.Remove(bindingExpressionsTextBox.GetBindingExpression(TextBox.TextProperty));

            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
            if (_bindingGroup.Items.Count != 0) return TestResult.Fail;

            if (GenericValidationRule.Results.Count != 0) return TestResult.Fail;

            // Dirty values
            p.Name = "Foo";
            p2.Name = "Bar";

            if (!_bindingGroup.ValidateWithoutUpdate()) return TestResult.Fail;

            if (GenericValidationRule.Results.Count != 0) return TestResult.Fail;

            // Removing the TextBox's binding from the BindingGroup means that UpdateSourceTrigger should revert from Explicit to when the Property Changes.
            bindingGroupNameTextBox.Text = "Foo";
            if (p.Name != "Foo") return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify BindingGroup BindingExpressions property can be cleared and contained BindingExpressions behave as expected
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupBindingExpressionsClear()
        {
            _bindingGroup.BindingExpressions.Clear();
            GenericValidationRule.Results.Clear();

            Place p = new Place("Brea", "CA");
            Place p2 = new Place("Brea", "CA");

            // We can remove BindingExpressions that were added via DataContext/BindingGroupName or via BindingExpressions
            TextBox bindingGroupNameTextBox = GenerateTextBox(p, JoinMethod.Explicit, true);
            TextBox bindingExpressionsTextBox = GenerateTextBox(p2, JoinMethod.BindingExpressions, true);

            if (_bindingGroup.BindingExpressions.Count != 2) return TestResult.Fail;
            if (_bindingGroup.Items.Count != 2) return TestResult.Fail;

            _bindingGroup.BindingExpressions.Clear();

            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
            if (_bindingGroup.Items.Count != 0) return TestResult.Fail;

            if (GenericValidationRule.Results.Count != 0) return TestResult.Fail;

            // Dirty values
            p.Name = "Foo";
            p2.Name = "Bar";

            if (!_bindingGroup.ValidateWithoutUpdate()) return TestResult.Fail;

            if (GenericValidationRule.Results.Count != 0) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Test BindingGroup.Items API
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupItems()
        {
            _bindingGroup.BindingExpressions.Clear();

            Place p = new Place("Brea", "CA");
            Place p2 = new Place("Seattle", "WA");

            GenerateTextBox(p);
            GenerateTextBox(p2);
            GenerateTextBox(p2);

            if (_bindingGroup.Items.Count != 2 || !_bindingGroup.Items.Contains(p) || !_bindingGroup.Items.Contains(p2)) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Validate BindingGroup.NotifyOnValidationError API
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupNotifyOnValidationError()
        {
            _bindingGroup.BindingExpressions.Clear();

            Place p = new Place("Brea", "CA");
            GenerateTextBox(p);

            GenericValidationRule gvr = new GenericValidationRule(ValidationStep.RawProposedValue, false);
            RootElement.BindingGroup.ValidationRules.Add(gvr);
            RootElement.BindingGroup.NotifyOnValidationError = true;

            // Not using EventHelper since ValidationErrorEventArgs doesn't have a public constructor
            bool result = false;
            RootElement.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(delegate(object o, RoutedEventArgs e) { result = true; }));

            RootElement.BindingGroup.CommitEdit();

            if (!result) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Test implicit membership of a BindingGroup. When a binding uses the DataContext of
        /// a Framework[Content]Element that has a BindingGroup defined, that binding is automatically
        /// added to the BindingGroup.
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupImplicitMembership()
        {
            UniformGrid uniformGrid = (UniformGrid)RootElement;
            _bindingGroup = new BindingGroup();
            Place dataContextPlace = new Place("Brea", "CA");
            Place nonDataContextPlace = new Place("Seattle", "WA");
            TextBox textBox = new TextBox();

            uniformGrid.BindingGroup = _bindingGroup;
            uniformGrid.DataContext = dataContextPlace;
            uniformGrid.Children.Add(textBox);

            // Default name for a BindingGroup is null
            if (_bindingGroup.Name != null) return TestResult.Fail;

            // Bindings using DataContext gets added to the BindingGroup implicitly. This should work for both OneWayToSource and TwoWay modes.
            // Since it is a TextBox, the default binding mode is TwoWay, so default will also work. All other modes should not be added implicitly
            Binding binding;
            foreach (BindingMode bindingMode in Enum.GetValues(typeof(BindingMode)))
            {
                binding = new Binding("Name");
                binding.Mode = bindingMode;
                textBox.SetBinding(TextBox.TextProperty, binding);

                if (bindingMode == BindingMode.OneWayToSource || bindingMode == BindingMode.TwoWay || bindingMode == BindingMode.Default)
                {
                    if (_bindingGroup.BindingExpressions.Count != 1 || _bindingGroup.BindingExpressions[0] != textBox.GetBindingExpression(TextBox.TextProperty)) return TestResult.Fail;
                }
                else
                {
                    if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
                }
            }

            // Make sure implicit joining also works when the BindingGroup has a name
            _bindingGroup.Name = "BindingGroupName";
            binding = new Binding("Name");
            textBox.SetBinding(TextBox.TextProperty, binding);
            if (_bindingGroup.BindingExpressions.Count != 1 || _bindingGroup.BindingExpressions[0] != textBox.GetBindingExpression(TextBox.TextProperty)) return TestResult.Fail;

            // Setting BindingGroupName to null means the binding shouldn't be added implicitly.
            binding = new Binding("Name");
            binding.BindingGroupName = null;
            textBox.SetBinding(TextBox.TextProperty, binding);
            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;

            // Bindings not using the DataContext are not added to the BindingGroup implicitly
            binding = new Binding("Name");
            binding.Source = nonDataContextPlace;
            textBox.SetBinding(TextBox.TextProperty, binding);
            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Test Explicit Membership of a BindingGroup by specifying the BindingGroupName
        /// property on Binding.
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupExplicitMembership()
        {
            UniformGrid uniformGrid = (UniformGrid)RootElement;
            _bindingGroup = new BindingGroup();
            Place p = new Place("Brea", "CA");
            TextBox textBox = new TextBox();

            uniformGrid.BindingGroup = _bindingGroup;
            uniformGrid.DataContext = null;
            uniformGrid.Children.Add(textBox);

            // Setting BindingGroupName to null means the binding shouldn't be a member of any group. For matching purposes null != null,
            // so even though the BindingGroup's name is null, it doesn't get added
            Binding binding = new Binding("Name");
            binding.Source = p;
            binding.BindingGroupName = null;
            textBox.SetBinding(TextBlock.TextProperty, binding);
            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;

            // Give the BindingGroup a name so it can be joined explicitly
            _bindingGroup.Name = "groupToJoin";

            // Bindings using explicit BindingGroup membership can be added to the BindingGroup regardless of their mode
            foreach (BindingMode bindingMode in System.Enum.GetValues(typeof(BindingMode)))
            {
                binding = new Binding("Name");
                binding.Source = p;
                binding.BindingGroupName = "groupToJoin";
                binding.Mode = bindingMode;
                textBox.SetBinding(TextBox.TextProperty, binding);
                if (_bindingGroup.BindingExpressions.Count != 1 || _bindingGroup.BindingExpressions[0] != textBox.GetBindingExpression(TextBox.TextProperty)) return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Test BindingGroup membership scenarios unique to Multibinding and PriorityBinding
        /// The rule is that explicit membership is determined via the BindingGroup,
        /// and implicit membership is determined via the child bindings.
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupMultiPriorityBindingMembership()
        {
            UniformGrid uniformGrid = (UniformGrid)RootElement;
            _bindingGroup = new BindingGroup();
            _bindingGroup.Name = "groupToJoin";
            BindingGroup otherBindingGroup = new BindingGroup();
            otherBindingGroup.Name = "OtherGroup";
            Place dataContextPlace = new Place("Brea", "CA");
            Place nonDataContextPlace = new Place("Seattle", "WA");
            uniformGrid.DataContext = dataContextPlace;
            uniformGrid.BindingGroup = _bindingGroup;
            Window.BindingGroup = otherBindingGroup;
            TextBox textBox = new TextBox();
            uniformGrid.Children.Add(textBox);

            // Explicit membership on child bindings should be ignored
            // For Multibinding:
            Binding bindToNonDataContext = new Binding("Name");
            bindToNonDataContext.BindingGroupName = "OtherGroup";
            bindToNonDataContext.Source = nonDataContextPlace;
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Bindings.Add(bindToNonDataContext);
            multiBinding.Converter = new IdentityConverter();
            textBox.SetBinding(TextBox.TextProperty, multiBinding);
            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
            if (otherBindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
            // For Prioritybinding:
            bindToNonDataContext = new Binding("Name");
            bindToNonDataContext.BindingGroupName = "OtherGroup";
            bindToNonDataContext.Source = nonDataContextPlace;
            System.Windows.Data.PriorityBinding priorityBinding = new System.Windows.Data.PriorityBinding();
            priorityBinding.Bindings.Add(bindToNonDataContext);
            textBox.SetBinding(TextBox.TextProperty, priorityBinding);
            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
            if (otherBindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;

            // Implicit membership is determined via child bindings
            // For Multibinding:
            Binding bindToDataContext = new Binding("Name");
            multiBinding = new MultiBinding();
            multiBinding.Bindings.Add(bindToDataContext);
            multiBinding.Converter = new IdentityConverter();
            textBox.SetBinding(TextBox.TextProperty, multiBinding);
            if (_bindingGroup.BindingExpressions.Count != 1 || _bindingGroup.BindingExpressions[0] != BindingOperations.GetMultiBindingExpression(textBox, TextBox.TextProperty)) return TestResult.Fail;
            if (otherBindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
            // For Prioritybinding:
            bindToDataContext = new Binding("Name");
            priorityBinding = new System.Windows.Data.PriorityBinding();
            priorityBinding.Bindings.Add(bindToDataContext);
            textBox.SetBinding(TextBox.TextProperty, priorityBinding);
            if (_bindingGroup.BindingExpressions.Count != 1 || _bindingGroup.BindingExpressions[0] != BindingOperations.GetPriorityBindingExpression(textBox, TextBox.TextProperty)) return TestResult.Fail;
            if (otherBindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;

            // Explicit membership is determined via parent binding and overrides implicit membership of child bindings
            bindToDataContext = new Binding("Name");
            multiBinding = new MultiBinding();
            multiBinding.Bindings.Add(bindToDataContext);
            multiBinding.Converter = new IdentityConverter();
            multiBinding.BindingGroupName = "OtherGroup";
            textBox.SetBinding(TextBox.TextProperty, multiBinding);
            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
            if (otherBindingGroup.BindingExpressions.Count != 1 || otherBindingGroup.BindingExpressions[0] != BindingOperations.GetMultiBindingExpression(textBox, TextBox.TextProperty)) return TestResult.Fail;
            // For Prioritybinding:
            bindToDataContext = new Binding("Name");
            priorityBinding = new System.Windows.Data.PriorityBinding();
            priorityBinding.Bindings.Add(bindToDataContext);
            priorityBinding.BindingGroupName = "OtherGroup";
            textBox.SetBinding(TextBox.TextProperty, priorityBinding);
            if (_bindingGroup.BindingExpressions.Count != 0) return TestResult.Fail;
            if (otherBindingGroup.BindingExpressions.Count != 1 || otherBindingGroup.BindingExpressions[0] != BindingOperations.GetPriorityBindingExpression(textBox, TextBox.TextProperty)) return TestResult.Fail;

            // clear it out
            Window.BindingGroup = null;

            return TestResult.Pass;
        }

        /// <summary>
        /// Test Validation APIs on BindingGroup run the appropriate Validation rules based on their ValidationStep
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupValidationRulesSteps()
        {
            _bindingGroup.BindingExpressions.Clear();

            UniformGrid uniformGrid = (UniformGrid)RootElement;

            Place p = new Place("Brea", "CA");
            TextBox textBox = new TextBox();

            uniformGrid.BindingGroup = _bindingGroup;
            uniformGrid.DataContext = p;
            uniformGrid.Children.Add(textBox);

            // The default value for ValidationStep should be RawProposedValue
            GenericValidationRule gvr = new GenericValidationRule();
            if (gvr.ValidationStep != ValidationStep.RawProposedValue) return TestResult.Fail;

            // ValidateWithoutUpdate runs validation rules marked RawProposedValue, and if there are no errors, then runs rules marked ConvertedProposedValue.

            // Make a RawProposedValue rule fail and make sure it returns false and doesn't continue on.
            InitValidationBinding(textBox, false, true, true, true);
            GenericValidationRule.Results.Clear();
            if (_bindingGroup.ValidateWithoutUpdate()) return TestResult.Fail;
            if (GenericValidationRule.Results.Count != 1 || !GenericValidationRule.Results.Contains(_rawProposedValueStep)) return TestResult.Fail;

            // Make a ConvertedProposedValue rule fail and make sure it returns false and doesn't continue on.
            InitValidationBinding(textBox, true, false, true, true);
            GenericValidationRule.Results.Clear();
            if (_bindingGroup.ValidateWithoutUpdate()) return TestResult.Fail;
            if (GenericValidationRule.Results.Count != 2 || !GenericValidationRule.Results.Contains(_rawProposedValueStep) || !GenericValidationRule.Results.Contains(_convertedProposedValueStep)) return TestResult.Fail;

            // Make all rules pass and make sure it returns true and doesn't continue on.
            InitValidationBinding(textBox, true, true, true, true);
            GenericValidationRule.Results.Clear();
            if (!_bindingGroup.ValidateWithoutUpdate()) return TestResult.Fail;
            if (GenericValidationRule.Results.Count != 2 || !GenericValidationRule.Results.Contains(_rawProposedValueStep) || !GenericValidationRule.Results.Contains(_convertedProposedValueStep)) return TestResult.Fail;
            GenericValidationRule.Results.Clear();

            // UpdateSources calls ValidateWithoutUpdate, and if it passes, runs rules marked UpdatedValue

            // Make a UpdatedValue rule fail and make sure it returns false and doesn't continue on.
            InitValidationBinding(textBox, true, true, false, true);
            GenericValidationRule.Results.Clear();
            if (_bindingGroup.UpdateSources()) return TestResult.Fail;
            if (GenericValidationRule.Results.Count != 3 || !GenericValidationRule.Results.Contains(_rawProposedValueStep) || !GenericValidationRule.Results.Contains(_convertedProposedValueStep) || !GenericValidationRule.Results.Contains(_updatedValueStep)) return TestResult.Fail;

            // Make all rules pass and make sure it returns true and doesn't continue on.
            InitValidationBinding(textBox, true, true, true, true);
            GenericValidationRule.Results.Clear();
            if (!_bindingGroup.UpdateSources()) return TestResult.Fail;
            if (GenericValidationRule.Results.Count != 3 || !GenericValidationRule.Results.Contains(_rawProposedValueStep) || !GenericValidationRule.Results.Contains(_convertedProposedValueStep) || !GenericValidationRule.Results.Contains(_updatedValueStep)) return TestResult.Fail;


            // CommitEdit calls UpdateSources, and if it passes, runs rules marked CommitedValue

            // Make a CommitedValue rule fail and make sure it returns false.
            InitValidationBinding(textBox, true, true, true, false);
            GenericValidationRule.Results.Clear();
            if (_bindingGroup.CommitEdit()) return TestResult.Fail;
            if (GenericValidationRule.Results.Count != 4 || !GenericValidationRule.Results.Contains(_rawProposedValueStep) || !GenericValidationRule.Results.Contains(_convertedProposedValueStep) || !GenericValidationRule.Results.Contains(_updatedValueStep) || !GenericValidationRule.Results.Contains(_commitedValueStep)) return TestResult.Fail;

            // Make all rules pass and make sure it returns true.
            InitValidationBinding(textBox, true, true, true, true);
            GenericValidationRule.Results.Clear();
            if (!_bindingGroup.CommitEdit()) return TestResult.Fail;
            if (GenericValidationRule.Results.Count != 4 || !GenericValidationRule.Results.Contains(_rawProposedValueStep) || !GenericValidationRule.Results.Contains(_convertedProposedValueStep) || !GenericValidationRule.Results.Contains(_updatedValueStep) || !GenericValidationRule.Results.Contains(_commitedValueStep)) return TestResult.Fail;

            // Clean up after itself
            GenericValidationRule.Results.Clear();

            return TestResult.Pass;
        }

        /// <summary>
        /// Test miscellaneous ValidationStep functionality
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupValidationStepMisc()
        {
            _bindingGroup.BindingExpressions.Clear();

            // ValidationRules on a BindingGroup are passed the BindingGroup as the value parameter
            GenerateTextBox(new Place("Brea", "CA"));

            GenericValidationRule genericValidationRule = new GenericValidationRule();
            _bindingGroup.ValidationRules.Add(genericValidationRule);

            _bindingGroup.CommitEdit();

            if (genericValidationRule.Value != _bindingGroup) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Validations ItemsControl.ItemBindingGroup API
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupForwarding()
        {
            UniformGrid uniformGrid = (UniformGrid)RootElement;
            EarthDataSource earthDataSource = new EarthDataSource();
            TreeView treeView = new TreeView();
            uniformGrid.Children.Add(treeView);
            uniformGrid.DataContext = earthDataSource;
            treeView.ItemBindingGroup = new BindingGroup();
            treeView.ItemBindingGroup.Name = "TreeViewBindingGroup";
            Binding HemisphereBinding = new Binding("Hemispheres");
            treeView.SetBinding(TreeView.ItemsSourceProperty, HemisphereBinding);
            WaitForPriority(DispatcherPriority.SystemIdle);
            TreeViewItem tvi = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(treeView.Items[0]);

            // Verify BindingGroup on tv and tvi matches. Using Graphics's ObjectUtils
            if (!ObjectUtils.DeepEquals(treeView.ItemBindingGroup, tvi.ItemBindingGroup)) return TestResult.Fail;
            //if (!ObjectUtils.DeepEquals(treeView.ItemBindingGroup, tvi.BindingGroup)) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify the default UpdateSourceTrigger value is Explicit
        /// change text of TextBox whose binding is part of a BindingGroup and
        /// verify the changed value is not pushed back to the datasource until
        /// it is explicitly done by calling UpdateSources.
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupUpdateSourceTriggerDefaultValue()
        {
            _bindingGroup.BindingExpressions.Clear();

            Place p = new Place("Brea", "CA");
            TextBox textBox = GenerateTextBox(p);
            TextBox textBoxSwitchFocus = new TextBox();
            ((UniformGrid)RootElement).Children.Add(textBoxSwitchFocus);

            textBox.Focus();
            textBox.Text = "Foo";
            textBoxSwitchFocus.Focus();

            if (p.Name != "Brea") return TestResult.Fail;

            RootElement.BindingGroup.UpdateSources();

            if (p.Name != "Foo") return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Test BindingGroup AdornerSite APIs
        /// </summary>
        /// <returns></returns>
        private TestResult BindingGroupAdorner()
        {
            _bindingGroup.BindingExpressions.Clear();

            UniformGrid uniformGrid = (UniformGrid)RootElement;
            Place p = new Place("Brea", "CA");
            TextBox tb = new TextBox();
            TextBox tb2 = new TextBox();
            uniformGrid.BindingGroup = _bindingGroup;
            uniformGrid.DataContext = p;
            uniformGrid.Children.Add(tb);
            uniformGrid.Children.Add(tb2);
            Binding b = new Binding("Name");
            tb.SetBinding(TextBox.TextProperty, b);
            GenericValidationRule gvr = new GenericValidationRule(ValidationStep.RawProposedValue, false);
            uniformGrid.BindingGroup.ValidationRules.Add(gvr);
            tb.SetBinding(TextBox.TextProperty, b);
            tb.Text = "Dirty";
            _bindingGroup.CommitEdit();

            // The adorner is already on the UniformGrid, move it to tb2.
            // ErrorTemplate Style not initially applied to CellTemplate when binding to IDataErrorInfo
            // which is about rebaking the adorner when an ingredient changes
            Validation.SetValidationAdornerSite(uniformGrid, tb2);

            AdornerLayer al = AdornerLayer.GetAdornerLayer(tb2);
            Adorner[] adorners = al.GetAdorners(tb2);
            if ((adorners == null) || (adorners.Length != 1)) return TestResult.Fail;

            Validation.SetValidationAdornerSiteFor(tb, uniformGrid);

            al = AdornerLayer.GetAdornerLayer(tb);
            adorners = al.GetAdorners(tb);
            if ((adorners == null) || (adorners.Length != 1)) return TestResult.Fail;

            return TestResult.Pass;
        }

        /// <summary>
        /// Given a data source, generate a basic TextBox that has it's Text property wired up.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        private TextBox GenerateTextBox(object dataSource)
        {
            return GenerateTextBox(dataSource, JoinMethod.Explicit, false);
        }

        /// <summary>
        /// Given a data source, generates a basic TextBox that has it's Text property wired up,
        /// and is joined to a BindingGroup by the specified method. AddValidationRule allows you
        /// an easy way to trace when validation rules on a binding expression are run.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        private TextBox GenerateTextBox(object dataSource, JoinMethod joinMethod, bool addValidationRule)
        {
            // Expected state, throw if not satisfied.
            if (RootElement.BindingGroup == null || RootElement.BindingGroup.Name != "groupToJoin")
            {
                throw new InvalidOperationException();
            }

            UniformGrid uniformGrid = (UniformGrid)RootElement;
            TextBox textBox = new TextBox();
            uniformGrid.Children.Add(textBox);

            Binding objectBinding;
            if (dataSource as Place != null)
            {
                objectBinding = new Binding("Name");
            }
            else if (dataSource as Country != null)
            {
                objectBinding = new Binding("CountryName");
            }
            else
            {
                throw new NotImplementedException();
            }

            switch (joinMethod)
            {
                case JoinMethod.Implicit:
                    RootElement.DataContext = dataSource;
                    break;
                case JoinMethod.Explicit:
                    objectBinding.Source = dataSource;
                    objectBinding.BindingGroupName = "groupToJoin";
                    break;
                case JoinMethod.BindingExpressions:
                    objectBinding.Source = dataSource;
                    break;
                case JoinMethod.None:
                    objectBinding.Source = dataSource;
                    break;
                default:
                    break;
            }

            if (addValidationRule)
            {
                objectBinding.ValidationRules.Add(new GenericValidationRule(ValidationStep.RawProposedValue, true));
            }

            textBox.SetBinding(TextBox.TextProperty, objectBinding);

            if (joinMethod == JoinMethod.BindingExpressions)
            {
                _bindingGroup.BindingExpressions.Add(textBox.GetBindingExpression(TextBox.TextProperty));
            }

            return textBox;
        }

        /// <summary>
        /// Basic converter that returns the first item
        /// </summary>
        private class IdentityConverter : IMultiValueConverter
        {
            #region IMultiValueConverter Members

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                return values[0];
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        /// Create a TextBox wired up with ValidationRules
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="rawPasses"></param>
        /// <param name="convertedPasses"></param>
        /// <param name="updatedPasses"></param>
        /// <param name="committedPasses"></param>
        private void InitValidationBinding(TextBox tb, bool rawPasses, bool convertedPasses, bool updatedPasses, bool committedPasses)
        {
            Binding b = new Binding("Name");
            b.ValidationRules.Add(_rawProposedValueStep);
            b.ValidationRules.Add(_convertedProposedValueStep);
            b.ValidationRules.Add(_updatedValueStep);
            b.ValidationRules.Add(_commitedValueStep);
            _rawProposedValueStep.Passes = rawPasses;
            _convertedProposedValueStep.Passes = convertedPasses;
            _updatedValueStep.Passes = updatedPasses;
            _commitedValueStep.Passes = committedPasses;

            tb.SetBinding(TextBox.TextProperty, b);

            tb.Text = "NewValue";
        }

        /// <summary>
        /// A GenericValidationRule to easily define the ValidationStep and presents a 'flight recorder' of if it was called and with what params
        /// </summary>
        private class GenericValidationRule : ValidationRule
        {
            public struct GenericValidationResult
            {
                public ValidationStep ValidationStep;
                public bool Passes;
                public object Value;

                public GenericValidationResult(ValidationStep validationStep, bool passes, object value)
                {
                    ValidationStep = validationStep;
                    Passes = passes;
                    Value = value;
                }
            }

            public static List<GenericValidationRule> Results = new List<GenericValidationRule>();

            private ValidationStep _step;
            private bool _passes;
            private object _passedValue;

            public GenericValidationRule()
            {
                Step = this.ValidationStep;
                Passes = true;
            }

            public GenericValidationRule(ValidationStep step, bool passes)
            {
                Step = step;
                Passes = passes;

                ValidationStep = step;
            }

            public ValidationStep Step
            {
                get
                {
                    return _step;
                }
                set
                {
                    _step = value;
                }
            }

            public bool Passes
            {
                get
                {
                    return _passes;
                }
                set
                {
                    _passes = value;
                }
            }

            public object Value
            {
                get
                {
                    return _passedValue;
                }
                set
                {
                    _passedValue = value;
                }
            }

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                Value = value;
                Results.Add(this);
                return new ValidationResult(Passes, null);
            }
        }

        #endregion
    }
}
