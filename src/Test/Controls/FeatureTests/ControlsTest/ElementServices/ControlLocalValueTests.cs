using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
#if TESTBUILD_CLR40
    public enum BaseValueSourceCombination
    {
        Local,
        LocalAndBinding,
        Style,
        StyleWithBinding,
        DefaultWithStyleTrigger,
        DefaultWithStyleTriggerAndBinding,
        StyleWithParentTemplateTrigger,
        ParentTemplate,
        Inherited
    }

    /// <summary>
    /// <description>
    /// Control Local Value tests.
    /// </description>
    /// <relatedBugs>
    /// <bug></bug>    
    /// <


    [Test(0, "ElementServices", "ControlLocalValueTests", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class ControlLocalValueTests : RegressionXamlTest
    {
    #region Fields

        // mapping of the different variations to the UIElements in the xaml file
        private static readonly Dictionary<BaseValueSourceCombination, string> BVSCombinationMapping = new Dictionary<BaseValueSourceCombination, string>
        {
            {BaseValueSourceCombination.Local, "CustomControl_Local"},
            {BaseValueSourceCombination.LocalAndBinding, "CustomControl_Local_Binding"},
            {BaseValueSourceCombination.Style, "CustomControl_Style"},
            {BaseValueSourceCombination.StyleWithBinding, "CustomControl_Style_Binding"},
            {BaseValueSourceCombination.DefaultWithStyleTrigger, "CustomControl_StyleTrigger"},
            {BaseValueSourceCombination.DefaultWithStyleTriggerAndBinding, "CustomControl_StyleTrigger_Binding"},
            {BaseValueSourceCombination.StyleWithParentTemplateTrigger, "ParentCustomControl_ParentTemplateTrigger"},
            {BaseValueSourceCombination.ParentTemplate, "ParentCustomControl_ParentTemplate"},
            {BaseValueSourceCombination.Inherited, "CustomControl_Inherits"}
        };

        private readonly string expectedCurrentValue = "Changed Title from SetCurrentValue";
        private readonly string expectedDefaultValue = "default test text";
        private readonly string expectedLocalValue = "Changed Title";
        private readonly string expectedCustomCoercedValue = "custom coerced value";

        private BaseValueSourceCombination currentBvsCombination;
        private CustomControlLocalValueControl customControl;
        private Control parentControl;
        private ListBox listBox;        
        private BaseValueSource previousBaseValueSource;
        private string previousValue;

        private ICommand setLocalValue;
        private ICommand setCurrentValue;
        private ICommand clearValue;

        #endregion Fields

    #region Constructor

        public ControlLocalValueTests()
            : this(BaseValueSourceCombination.StyleWithParentTemplateTrigger)
        {
        }

        [Variation(BaseValueSourceCombination.Local)]
        [Variation(BaseValueSourceCombination.LocalAndBinding)]
        [Variation(BaseValueSourceCombination.Style)]
        [Variation(BaseValueSourceCombination.StyleWithBinding)]
        [Variation(BaseValueSourceCombination.DefaultWithStyleTrigger)]
        [Variation(BaseValueSourceCombination.DefaultWithStyleTriggerAndBinding)]
        [Variation(BaseValueSourceCombination.StyleWithParentTemplateTrigger)]
        [Variation(BaseValueSourceCombination.ParentTemplate)]
        //[Variation(BaseValueSourceCombination.Inherited)]
        public ControlLocalValueTests(BaseValueSourceCombination bvsCombination)
            : base(@"ControlLocalValueTests.xaml")
        {
            currentBvsCombination = bvsCombination;

            InitializeSteps += new TestStep(Setup);
            RunSteps += TestInitialState;
            RunSteps += TestApplyingCurrentValue;
            RunSteps += TestClearValueAfterApplyingCurrentValue;
            RunSteps += TestTriggerAfterApplyingCurrentValue;
            RunSteps += TestSetLocalAfterApplyingCurrentValue;
            RunSteps += TestCustomCoercion;
        }

        #endregion

    #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            Status("Setup specific for ControlLocalValueTests");

            LogComment(string.Format("Looking for: {0}, given combo: {1} ", BVSCombinationMapping[currentBvsCombination], currentBvsCombination));

            if (currentBvsCombination == BaseValueSourceCombination.StyleWithParentTemplateTrigger ||
                currentBvsCombination == BaseValueSourceCombination.ParentTemplate)
            {
                parentControl = (Control)RootElement.FindName(BVSCombinationMapping[currentBvsCombination]);
                Assert.AssertTrue("parentControl cannot be null for test to continue.", parentControl != null);

                customControl = DataGridHelper.FindVisualChild<CustomControlLocalValueControl>(parentControl);
            }
            else
            {
                customControl = (CustomControlLocalValueControl)RootElement.FindName(BVSCombinationMapping[currentBvsCombination]);
            }
            Assert.AssertTrue("customControl cannot be null for test to continue.", customControl != null);

            listBox = (ListBox)RootElement.FindName("ListBox");
            Assert.AssertTrue("unable to find ListBox from xaml file", listBox != null);

            RootElement.DataContext = this;

            QueueHelper.WaitTillQueueItemsProcessed();

            InitBaseValueSource();

            // uncomment for ad-hoc debugging
            //base.Setup();

            LogComment("Setup for ControlLocalValueTests was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            parentControl = null;
            customControl = null;
            listBox = null;

            return base.CleanUp();
        }

        /// <summary>
        /// Assumption:  Initial State for each combination does not have CurrentValue set initially
        /// 
        /// 1. Verify that ValueSource.IsCoerced == false
        /// 2. Verify that ValueSource.IsCurrent == false
        /// </summary>
        private TestResult TestInitialState()
        {
            Status("TestInitialState");

            LogComment("Verify ValueSource.IsCoerced == false");
            Assert.AssertTrue("initial ValueSource.IsCoered should be false but isn't", !customControl.TitleValueSource.IsCoerced);

            LogComment("Verify ValueSource.IsCurrent == false");
            Assert.AssertTrue("initial ValueSource.IsCurrent should be false but isn't", !customControl.TitleValueSource.IsCurrent);

            LogComment("TestInitialState was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. Apply a CurrentValue on the Control
        /// 
        /// Verify:
        /// 1. CurrentValue is correctly passed to the CoercionCallback
        /// 2. CurrentValue is correctly passed to the PropertyChangedCallback
        /// 3. The value of CurrentValue is now the effective value of the DP
        /// 4. ValueSource.BaseValueSource has no change
        /// 5. ValueSource.IsCoerced == true
        /// 6. ValueSource.IsCurrent == true
        /// </summary>
        private TestResult TestApplyingCurrentValue()
        {
            Status("TestApplyingCurrentValue");

            SetControlCurrentValue();

            LogComment("TestApplyingCurrentValue was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// With CurrentValue already set, Verify clearing the local value:
        /// 
        /// 1. removes the current value
        /// 2. ValueSource.IsCoerced == false
        /// 3. ValueSource.IsCurrent == false
        /// 4a. for BaseValueSource initially set to Local, sets to the default value
        /// 4b. for BaseValueSource initially not set to Local, sets the BaseValueSource to it's previous value
        /// </summary>
        private TestResult TestClearValueAfterApplyingCurrentValue()
        {
            Status("TestClearValueAfterApplyingCurrentValue");

            ClearCurrentValue();

            LogComment("reset customControl back to set state");
            if (previousBaseValueSource == BaseValueSource.Local)
            {
                // have to set local value again as it was cleared
                SetLocalValue.Execute(customControl);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            SetControlCurrentValue();

            LogComment("TestClearValueAfterApplyingCurrentValue was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. With CurrentValue already set, mousover the control
        /// 
        /// Verify:
        /// 1. removes the current value
        /// 2. ValueSource.IsCoerced == false
        /// 3. ValueSource.IsCurrent == false
        /// 4. BaseValueSource is set to trigger
        /// 
        /// 2. mouse away from the control
        /// 
        /// Verify:
        /// 1. Value is set back to the default
        /// 2. ValueSource.IsCoerced == false
        /// 3. ValueSource.IsCurrent == false
        /// 4. BaseValueSource is set to the default
        /// 
        /// </summary>
        private TestResult TestTriggerAfterApplyingCurrentValue()
        {
            Status("TestTriggerAfterApplyingCurrentValue");

            if (currentBvsCombination == BaseValueSourceCombination.DefaultWithStyleTrigger ||
                currentBvsCombination == BaseValueSourceCombination.DefaultWithStyleTriggerAndBinding ||
                currentBvsCombination == BaseValueSourceCombination.StyleWithParentTemplateTrigger)
            {
                // assumption: in current value state upon entering
                Assert.AssertTrue("ValueSource.IsCoered did not change to true", customControl.TitleValueSource.IsCoerced);
                Assert.AssertTrue("ValueSource.IsCurrent did not change to true", customControl.TitleValueSource.IsCurrent);

                // set expected values
                string expectedEffectiveValueMouseOver = "trigger fired";
                string expectedEffectiveValueMouseAway = null;
                BaseValueSource expectedBvsMouseOver = BaseValueSource.Unknown;
                BaseValueSource expectedBvsMouseAway = BaseValueSource.Unknown;
                if (currentBvsCombination == BaseValueSourceCombination.DefaultWithStyleTrigger ||
                    currentBvsCombination == BaseValueSourceCombination.DefaultWithStyleTriggerAndBinding)
                {
                    expectedBvsMouseOver = BaseValueSource.StyleTrigger;
                    expectedBvsMouseAway = BaseValueSource.Default;
                }
                else if (currentBvsCombination == BaseValueSourceCombination.StyleWithParentTemplateTrigger)
                {
                    expectedBvsMouseOver = BaseValueSource.ParentTemplateTrigger;
                    expectedBvsMouseAway = BaseValueSource.Style;
                    expectedEffectiveValueMouseAway = expectedDefaultValue;
                }
                
                LogComment("mouse over the control");
                if (parentControl != null)
                {
                    // slightly different behavior for a composite control
                    MouseOverHelper(parentControl);
                }
                else
                {
                    MouseOverHelper(customControl);
                }
                QueueHelper.WaitTillQueueItemsProcessed();

                VerifyDP(false, false, expectedEffectiveValueMouseOver, expectedBvsMouseOver);

                LogComment("mouse away from control");
                UserInput.MouseMove(RootElement, 0, 0);
                QueueHelper.WaitTillQueueItemsProcessed();
                this.WaitFor(250);

                VerifyDP(false, false, expectedEffectiveValueMouseAway, expectedBvsMouseAway);

                LogComment("reset customControl back to set state");                
                SetControlCurrentValue();
            }
            else
            {
                LogComment("Skipping non-trigger test");
            }

            LogComment("TestTriggerAfterApplyingCurrentValue was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// With CurrentValue already set, Verify setting explicit local value:
        /// 
        /// 1. removes the current value
        /// 2. ValueSource.IsCoerced == false
        /// 3. ValueSource.IsCurrent == false
        /// 4. BaseValueSource is set to the Local
        /// </summary>
        private TestResult TestSetLocalAfterApplyingCurrentValue()
        {
            Status("TestSetLocalAfterApplyingCurrentValue");

            // assumption: in current value state upon entering
            Assert.AssertTrue("ValueSource.IsCoered did not change to true", customControl.TitleValueSource.IsCoerced);
            Assert.AssertTrue("ValueSource.IsCurrent did not change to true", customControl.TitleValueSource.IsCurrent);

            LogComment("set the local value");
            SetLocalValue.Execute(customControl);
            QueueHelper.WaitTillQueueItemsProcessed();

            VerifyDP(false, false, expectedLocalValue, BaseValueSource.Local);

            LogComment("TestSetLocalAfterApplyingCurrentValue was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// With CurrentValue not set, Verify an explicit coercion takes precedence over CurrrentValue        
        /// </summary>
        private TestResult TestCustomCoercion()
        {
            Status("TestCustomCoercion");

            // assumption: not in current value state upon entering
            Assert.AssertTrue("ValueSource.IsCoered should be false", !customControl.TitleValueSource.IsCoerced);
            Assert.AssertTrue("ValueSource.IsCurrent should be false", !customControl.TitleValueSource.IsCurrent);

            // flag so custom coercion is used
            customControl.UseCustomCoercion = true;

            SetControlCurrentValue();

            // clean up
            customControl.UseCustomCoercion = false;

            LogComment("TestCustomCoercion was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

    #region Command Helpers

        public ICommand SetLocalValue
        {
            get
            {
                if (setLocalValue == null)
                    setLocalValue = new RelayCommand<object>(
                        (param) =>
                        {
                            CustomControlLocalValueControl c = param as CustomControlLocalValueControl;
                            c.Title = "Changed Title";
                        });

                return setLocalValue;
            }
        }

        public ICommand SetCurrentLocalValue
        {
            get
            {
                if (setCurrentValue == null)
                    setCurrentValue = new RelayCommand<object>(
                        (param) =>
                        {
                            CustomControlLocalValueControl c = param as CustomControlLocalValueControl;
                            c.SetCurrentValue(CustomControlLocalValueControl.TitleProperty, expectedCurrentValue);
                        });

                return setCurrentValue;
            }
        }

        public ICommand ClearLocalValue
        {
            get
            {
                if (clearValue == null)
                    clearValue = new RelayCommand<object>(
                        (param) =>
                        {
                            CustomControlLocalValueControl c = param as CustomControlLocalValueControl;
                            c.ClearValue(CustomControlLocalValueControl.TitleProperty);
                        });

                return clearValue;
            }
        }

        #endregion Command Helpers

    #region Helpers

        private void InitBaseValueSource()
        {
            ListBoxItem lbi = FindContainerForCustomControl();
            listBox.ScrollIntoView(lbi);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (currentBvsCombination == BaseValueSourceCombination.DefaultWithStyleTrigger ||
                currentBvsCombination == BaseValueSourceCombination.DefaultWithStyleTriggerAndBinding)
            {
                // need to hover over custom control to set the default BaseValueSource
                bool result = MouseOverHelper(customControl);
                Assert.AssertTrue("unable to mouseover the control", result);

                UserInput.MouseMove(RootElement, 0, 0);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }

        private ListBoxItem FindContainerForCustomControl()
        {
            foreach (ListBoxItem lbi in listBox.Items)
            {
                CustomControlLocalValueControl control = DataGridHelper.FindVisualChild<CustomControlLocalValueControl>(lbi);
                if (control == customControl)
                {
                    return lbi;
                }
            }

            return null;
        }

        private bool MouseOverHelper(FrameworkElement fe)
        {
            int counter = 0;
            int x = 0;
            int y = 0;

            // to deal with triggers that expand and collapse the controls
            // this provides some padding to make sure that MouseOver works
            while (!fe.IsMouseOver && counter < 10)
            {
                UserInput.MouseMove(fe, x, y);
                QueueHelper.WaitTillQueueItemsProcessed();
                this.WaitFor(200);

                y++;
                x++;
                counter++;
            }

            if (!customControl.IsMouseOver)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void SetControlCurrentValue()
        {
            LogComment("Begin SetControlCurrentValue");

            previousBaseValueSource = customControl.TitleValueSource.BaseValueSource;
            previousValue = customControl.Title;

            LogComment("apply the current value on the control");
            SetCurrentLocalValue.Execute(customControl);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Verify CurrentValue is correctly passed to the CoercionCallback");
            Assert.AssertTrue(
                string.Format("baseValue passed to CoercionCallback is incorrect.  Expected: {0}, Actual: {1}", expectedCurrentValue, customControl.LastBaseValueFromCoercionCallback),
                customControl.LastBaseValueFromCoercionCallback.ToString() == expectedCurrentValue);

            string expectedNewPropertyValue = customControl.UseCustomCoercion ? expectedCustomCoercedValue : expectedCurrentValue;
            LogComment("Verify CurrentValue is correctly passed to the PropertyChangedCallback");
            Assert.AssertTrue(
                string.Format("NewValue passed to PropertyChangedCallback is incorrect.  Expected: {0}, Actual: {1}", expectedNewPropertyValue, customControl.LastNewValueFromPropertyChangedCallback),
                customControl.LastNewValueFromPropertyChangedCallback.ToString() == expectedNewPropertyValue);

            if (customControl.LastOldValueFromPropertyChangedCallback == null)
            {
                Assert.AssertTrue(
                    string.Format(
                        "OldValue passed to PropertyChangedCallback is incorrect.  Expected: {0}, Actual: {1}",
                        previousValue,
                        customControl.LastOldValueFromPropertyChangedCallback),
                        previousValue == null);
            }
            else
            {
                Assert.AssertTrue(
                    string.Format(
                        "OldValue passed to PropertyChangedCallback is incorrect.  Expected: {0}, Actual: {1}",
                        previousValue,
                        customControl.LastOldValueFromPropertyChangedCallback),
                        customControl.LastOldValueFromPropertyChangedCallback.ToString() == previousValue);
            }

            VerifyDP(true, customControl.UseCustomCoercion ? false : true, expectedNewPropertyValue, previousBaseValueSource);            

            LogComment("End SetControlCurrentValue");
        }

        private void ClearCurrentValue()
        {
            // assumption: in current value state upon entering
            Assert.AssertTrue("ValueSource.IsCoered did not change to true", customControl.TitleValueSource.IsCoerced);
            Assert.AssertTrue("ValueSource.IsCurrent did not change to true", customControl.TitleValueSource.IsCurrent);

            // set expected data
            string expectedEffectiveValue = null;
            BaseValueSource expectedBvs;
            if (previousBaseValueSource == BaseValueSource.Local)
            {
                expectedEffectiveValue = expectedDefaultValue;
                expectedBvs = BaseValueSource.Style;
            }
            else
            {
                expectedEffectiveValue = previousValue;
                expectedBvs = previousBaseValueSource;
            }

            LogComment("clear the local value");
            ClearLocalValue.Execute(customControl);
            QueueHelper.WaitTillQueueItemsProcessed();            

            VerifyDP(false, false, expectedEffectiveValue, expectedBvs);         
        }

        private void VerifyDP(bool expectedIsCoerced, bool expectedIsCurrent, string expectedEffectiveValue, BaseValueSource expectedBvs)
        {
            LogComment("Verify ValueSource.IsCoerced");
            Assert.AssertTrue(
                string.Format("ValueSource.IsCoered is incorrect. Expected: {0}, Actual: {1}", expectedIsCoerced, customControl.TitleValueSource.IsCoerced),
                expectedIsCoerced == customControl.TitleValueSource.IsCoerced);

            LogComment("Verify ValueSource.IsCurrent");
            Assert.AssertTrue(
               string.Format("ValueSource.IsCurrent is incorrect. Expected: {0}, Actual: {1}", expectedIsCurrent, customControl.TitleValueSource.IsCurrent),
               expectedIsCurrent == customControl.TitleValueSource.IsCurrent);


            LogComment("Verify the value of CurrentValue");
            Assert.AssertTrue(
                string.Format("DP's effective value is incorrect.  Expected: {0}, Actual: {1}", expectedEffectiveValue, customControl.Title),
                expectedEffectiveValue == customControl.Title || customControl.Title.Contains(expectedEffectiveValue));

            LogComment("Verify ValueSource.BaseValueSource");
            Assert.AssertTrue(
                string.Format("BaseValueSource is incorrect. Expected: {0}, Actual: {1}", expectedBvs, customControl.TitleValueSource.BaseValueSource),
                expectedBvs == customControl.TitleValueSource.BaseValueSource);
        }

        #endregion Helpers
    }
#endif
}
