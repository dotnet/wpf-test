// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Collections;
using System.Windows.Media;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This tests the MultiBinding + Validation scenario. It tests: 1.Setting NotifyOnValidationError
    /// flag on a MultiBinding causes an event to fire when validity changes.
    /// 2.Validation.MarkInvalid(multibinding, ...) method works as expected. 3.Same for
    /// Validation.ClearInvalid(multibinding...)
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(2, "Validation", "MultiBindingValidation")]
    public class MultiBindingValidation : XamlTest
    {
        private ListBox _listBox;
        private ObservableCollection<Star> _starCollection;
        private ValidationStaticVerifier _staticVerifierNotify;
        private ValidationNonStaticVerifier _nonStaticVerifier;
        private MultiBindingExpression _multiBindingExpressionNotify;

        public MultiBindingValidation()
            : base(@"MultiBindingValidation.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(NotifyValidationTest);
            RunSteps += new TestStep(MarkTextInvalid);
            RunSteps += new TestStep(ClearTextInvalid);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);
            _listBox = Util.FindElement(RootElement, "listBox") as ListBox;

            _starCollection = new ObservableCollection<Star>();
            _starCollection.Add(new Star("Mike", "Piazza", 24));
            _starCollection.Add(new Star("Mark", "McGwire", 26));
            _starCollection.Add(new Star("Jay", "Bell", 30));
            _starCollection.Add(new Star("Matt", "Williams", 21));
            _starCollection.Add(new Star("Barry", "Larkin", 29));
            _starCollection.Add(new Star("Sammy", "Sosa", 26));
            _starCollection.Add(new Star("Larry", "Walker", 28));
            _starCollection.Add(new Star("Tony", "Gwynn", 32));

            _listBox.ItemsSource = _starCollection;

            _nonStaticVerifier = new ValidationNonStaticVerifier();

            return TestResult.Pass;
        }

        #region NotifyValidationTest
        private TestResult NotifyValidationTest()
        {
            Status("NotifyValidationTest");

            TextBox tb = GetTextBoxVisual(0);

            _staticVerifierNotify = new ValidationStaticVerifier(tb);

            // add handler for changes in validity
            tb.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationErrorIsClearedFalse));

            // update with invalid value
            // Causes Validation to happen. Because it's not valid, it doesn't call the MultiBinding
            // Converter's ConvertBack. Validation happens BEFORE the Converter.
            tb.Text = "cool"; // not enough letters, validation will fail
            _multiBindingExpressionNotify = BindingOperations.GetMultiBindingExpression(tb, TextBox.TextProperty);
            _multiBindingExpressionNotify.UpdateSource();

            // wait for event and make sure it returns success
            TestResult res = WaitForSignal("NotifyIsClearedFalse");
            if (res != TestResult.Pass) { return res; }

            tb.RemoveHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationErrorIsClearedFalse));

            return TestResult.Pass;
        }

        public void OnValidationErrorIsClearedFalse(Object sender, ValidationErrorEventArgs args)
        {
            Status("OnValidationErrorIsClearedFalse");
            // get the Validation error and Action from args
            ValidationError veIsClearedFalse = args.Error;

            // element level
            _staticVerifierNotify.Step = "OnValidationErrorIsClearedFalse - element";
            if (!_staticVerifierNotify.CheckNumErrors(1))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }
             
            if (!_staticVerifierNotify.CheckHasError(true))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }
            if (!_staticVerifierNotify.CheckValidationError(0, veIsClearedFalse))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }
            // property level - TextProperty
            _nonStaticVerifier.Step = "OnValidationErrorIsClearedFalse - property";
             
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpressionNotify, true))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpressionNotify, veIsClearedFalse))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }

            ValidationErrorEventAction action = args.Action;
            if (action == ValidationErrorEventAction.Removed)
            {
                LogComment("Fail - ValidationErrorEventArgs.Action is == Removed, it should be == Added");
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }

            Signal("NotifyIsClearedFalse", TestResult.Pass);
            return;
        }
        #endregion

        #region MarkTextInvalid
        private TestResult MarkTextInvalid()
        {
            Status("MarkTextInvalid");

            // mark TextBox in index 7 invalid
            TextBox tb = GetTextBoxVisual(7);

            ValidationStaticVerifier vsv = new ValidationStaticVerifier(tb);

            MultiBindingExpression multiBindingExpression = BindingOperations.GetMultiBindingExpression(tb, TextBox.TextProperty);
            if (multiBindingExpression == null)
            {
                LogComment("Fail - MultiBindingExpression is null");
                return TestResult.Fail;
            }
            MultiBinding multiBinding = multiBindingExpression.ParentMultiBinding;
            if (multiBinding == null)
            {
                LogComment("Fail - MultiBinding is null");
                return TestResult.Fail;
            }
            MinCharsRule rule = multiBinding.ValidationRules[0] as MinCharsRule;
            if (rule == null)
            {
                LogComment("Fail - MinCharsRule is null");
                return TestResult.Fail;
            }
            ValidationError error = new ValidationError(rule, multiBindingExpression, rule.ErrorContent, null);
            Validation.MarkInvalid(multiBindingExpression, error);

            // element level
            vsv.Step = "MarkClearInvalid - element";
            if (!vsv.CheckHasError(true)) { return TestResult.Fail; }
            if (!vsv.CheckNumErrors(1)) { return TestResult.Fail; }
            if (!vsv.CheckValidationError(0, error)) { return TestResult.Fail; }

            // property level
            _nonStaticVerifier.Step = "MarkClearInvalid - property";
            if (!_nonStaticVerifier.CheckHasError(multiBindingExpression, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(multiBindingExpression, error)) { return TestResult.Fail; }

            return TestResult.Pass;
        }
        #endregion

        #region ClearTextInvalid
        private TestResult ClearTextInvalid()
        {
            Status("ClearTextInvalid");

            // clear invalid - TextBox in index 7
            TextBox tb = GetTextBoxVisual(7);

            ValidationStaticVerifier vsv = new ValidationStaticVerifier(tb);

            MultiBindingExpression multiBindingExpression = BindingOperations.GetMultiBindingExpression(tb, TextBox.TextProperty);
            if (multiBindingExpression == null)
            {
                LogComment("Fail - MultiBindingExpression is null");
                return TestResult.Fail;
            }
            MultiBinding multiBinding = multiBindingExpression.ParentMultiBinding;
            if (multiBinding == null)
            {
                LogComment("Fail - MultiBinding is null");
                return TestResult.Fail;
            }
            MinCharsRule rule = multiBinding.ValidationRules[0] as MinCharsRule;
            if (rule == null)
            {
                LogComment("Fail - MinCharsRule is null");
                return TestResult.Fail;
            }
            ValidationError error = new ValidationError(rule, multiBindingExpression, rule.ErrorContent, null);
            Validation.ClearInvalid(multiBindingExpression);

            // element level
            vsv.Step = "MarkClearInvalid - element";
            if (!vsv.CheckHasError(false)) { return TestResult.Fail; }
            if (!vsv.CheckNumErrors(0)) { return TestResult.Fail; }

            // property level
            _nonStaticVerifier.Step = "MarkClearInvalid - property";
            if (!_nonStaticVerifier.CheckHasError(multiBindingExpression, false)) { return TestResult.Fail; }

            return TestResult.Pass;
        }
        #endregion

        private TextBox GetTextBoxVisual(int index)
        {
            Status("GetTextBoxVisual");
            ObservableCollection<Star> col = _listBox.ItemsSource as ObservableCollection<Star>;
            Star star = col[index] as Star;
            // FindDataVisual passing the listBox doesn't work anymore
            // this was probably introduced by changes in the visual tree due to virtualization
            WaitForPriority(DispatcherPriority.Render);
            ListBoxItem lbi = (ListBoxItem)(_listBox.ItemContainerGenerator.ContainerFromIndex(index));
            TextBox tb = (TextBox)(Util.FindDataVisual(lbi, star));
            return tb;
        }
    }
}
