using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;
using Avalon.Test.ComponentModel.Actions;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace Avalon.Test.ComponentModel.UnitTests
{
    [Flags]
    enum DependencyPropertyEvaluation
    {
        None = 0x0,
        Expression = 0x1,
        Animation = 0x2,
        Coerce = 0x3,
        DesignerCoerce = 0x4
    }

    /// <summary>
    /// <description>
    /// TestAnimationAndCoerceAndDesignerCoerceProperty    
    /// </description>
    /// </summary>
    // Disabling for .NET Core 3, Fix and re-enable.
    //[Test(0, "DependencyProperty", "TestAnimationAndCoerceAndDesignerCoerceProperty")]
    public class TestAnimationAndCoerceAndDesignerCoerceProperty : XamlTest
    {
        #region Private Members
        // Test Button.TestCoerce property

        // Not testing Coerce value
        TestCoerceButton defaultValue;
        TestCoercePanel inheritedPanel;
        TestCoerceButton inheritedValue;
        TestCoerceButton styleValue;
        TestCoerceButton styleTriggerValue;
        TestCoerceButton localValue;
        TestCoerceButton expressionValue;

        // Testing Coerce value
        TestCoerceButton testCoerceInheritedValue;
        TestCoerceButton testCoerceStyleValue;
        TestCoerceButton testCoerceStyleTriggerValue;
        TestCoerceButton testCoerceLocalValue;

        int expectedPropertyValue;
        #endregion

        #region Public Members

        public TestAnimationAndCoerceAndDesignerCoerceProperty()
            : base(@"TestAnimationAndCoerceAndDesignerCoerceProperty.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestAnimationAndCoerceAndDesignerCoerce);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.Render);

            // initial values.
            defaultValue = (TestCoerceButton)RootElement.FindName("defaultValue");
            inheritedPanel = (TestCoercePanel)RootElement.FindName("inheritedPanel");
            inheritedValue = (TestCoerceButton)RootElement.FindName("inheritedValue");
            styleValue = (TestCoerceButton)RootElement.FindName("styleValue");
            styleTriggerValue = (TestCoerceButton)RootElement.FindName("styleTriggerValue");
            localValue = (TestCoerceButton)RootElement.FindName("localValue");
            expressionValue = (TestCoerceButton)RootElement.FindName("expressionValue");

            testCoerceInheritedValue = (TestCoerceButton)RootElement.FindName("testCoerceInheritedValue");
            testCoerceStyleValue = (TestCoerceButton)RootElement.FindName("testCoerceStyleValue");
            testCoerceStyleTriggerValue = (TestCoerceButton)RootElement.FindName("testCoerceStyleTriggerValue");
            testCoerceLocalValue = (TestCoerceButton)RootElement.FindName("testCoerceLocalValue");

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            defaultValue = null;
            inheritedPanel = null;
            inheritedValue = null;
            styleValue = null;
            styleTriggerValue = null;
            localValue = null;
            expressionValue = null;
            testCoerceInheritedValue = null;
            testCoerceStyleValue = null;
            testCoerceStyleTriggerValue = null;
            testCoerceLocalValue = null;
            return TestResult.Pass;
        }

        public TestResult TestAnimationAndCoerceAndDesignerCoerce()
        {
            TestAnimationAndCoerceAndDesignerCoerce(DependencyPropertyEvaluation.None);
            TestAnimationAndCoerceAndDesignerCoerce(DependencyPropertyEvaluation.Animation);
            TestAnimationAndCoerceAndDesignerCoerce(DependencyPropertyEvaluation.Coerce);
            TestAnimationAndCoerceAndDesignerCoerce(DependencyPropertyEvaluation.Animation | DependencyPropertyEvaluation.Coerce);
            TestAnimationAndCoerceAndDesignerCoerce(DependencyPropertyEvaluation.DesignerCoerce);
            TestAnimationAndCoerceAndDesignerCoerce(DependencyPropertyEvaluation.DesignerCoerce | DependencyPropertyEvaluation.Animation);
            TestAnimationAndCoerceAndDesignerCoerce(DependencyPropertyEvaluation.DesignerCoerce | DependencyPropertyEvaluation.Coerce);
            TestAnimationAndCoerceAndDesignerCoerce(DependencyPropertyEvaluation.DesignerCoerce | DependencyPropertyEvaluation.Animation | DependencyPropertyEvaluation.Coerce);

            return TestResult.Pass;
        }

        #endregion

        #region Private Members

        private object OnCoerceTestCoerce(DependencyObject sender, object value)
        {
            return 100;
        }

        /// <summary>
        /// Test AnimationAndCoerceAndDesignerCoerce 
        /// </summary>
        /// <param name="isTestAnimation">Testing attached animation</param>
        /// <param name="isTestCoerce">Testing CoerceValueCallback.</param>
        /// <param name="isTestDesignerCoerce">Testing DesignerCoerceValueCallback.</param>
        private void TestAnimationAndCoerceAndDesignerCoerce(DependencyPropertyEvaluation dependencyPropertyEvaluation)
        {

            if ((dependencyPropertyEvaluation & DependencyPropertyEvaluation.DesignerCoerce) == DependencyPropertyEvaluation.DesignerCoerce)
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(TestCoerceButton.TestCoerceProperty, typeof(TestCoerceButton));
                dpd.DesignerCoerceValueCallback = new CoerceValueCallback(OnCoerceTestCoerce);

                if ((dependencyPropertyEvaluation & DependencyPropertyEvaluation.Animation) == DependencyPropertyEvaluation.None
                    && (dependencyPropertyEvaluation & DependencyPropertyEvaluation.Coerce) == DependencyPropertyEvaluation.None)
                {
                    // modify property value to trigger DesignerCoerceValueCallback.
                    ModifyPropertiesValue(1);

                    UserInput.MouseMove(styleTriggerValue, Convert.ToInt32(styleTriggerValue.ActualWidth / 2), Convert.ToInt32(styleTriggerValue.ActualHeight / 2));
                    QueueHelper.WaitTillQueueItemsProcessed();

                    //verify end property value is 100;
                    expectedPropertyValue = 100;
                    VerifyPropertiesValue();
                }
                else if ((dependencyPropertyEvaluation & DependencyPropertyEvaluation.Animation) == DependencyPropertyEvaluation.Animation
                && (dependencyPropertyEvaluation & DependencyPropertyEvaluation.Coerce) == DependencyPropertyEvaluation.None)
                {
                    Int32Animation animation = new Int32Animation();
                    animation.From = 3;
                    animation.To = 25;

                    expectedPropertyValue = 100;

                    RunAnimation(animation);
                }
                else if ((dependencyPropertyEvaluation & DependencyPropertyEvaluation.Animation) == DependencyPropertyEvaluation.None
                && (dependencyPropertyEvaluation & DependencyPropertyEvaluation.Coerce) == DependencyPropertyEvaluation.Coerce)
                {
                    // make local value bigger than Coerce value 50
                    ModifyPropertiesValue(60);

                    UserInput.MouseMove(styleTriggerValue, Convert.ToInt32(styleTriggerValue.ActualWidth / 2), Convert.ToInt32(styleTriggerValue.ActualHeight / 2));
                    QueueHelper.WaitTillQueueItemsProcessed();

                    //verify end property value is 100;
                    expectedPropertyValue = 100;
                    VerifyPropertiesValue();
                }
                else if ((dependencyPropertyEvaluation & DependencyPropertyEvaluation.Animation) == DependencyPropertyEvaluation.Animation
                && (dependencyPropertyEvaluation & DependencyPropertyEvaluation.Coerce) == DependencyPropertyEvaluation.Coerce)
                {
                    Int32Animation animation = new Int32Animation();
                    animation.From = 3;
                    animation.To = 60;

                    expectedPropertyValue = 100;

                    RunAnimation(animation);
                }
            }
            else
            {
                TestAnimationAndCoerce(dependencyPropertyEvaluation);
            }
        }

        private void TestAnimationAndCoerce(DependencyPropertyEvaluation dependencyPropertyEvaluation)
        {
            if ((dependencyPropertyEvaluation & DependencyPropertyEvaluation.Animation) == DependencyPropertyEvaluation.None
                && (dependencyPropertyEvaluation & DependencyPropertyEvaluation.Coerce) == DependencyPropertyEvaluation.None)
            {
                UserInput.MouseMove(styleTriggerValue, Convert.ToInt32(styleTriggerValue.ActualWidth / 2), Convert.ToInt32(styleTriggerValue.ActualHeight / 2));
                QueueHelper.WaitTillQueueItemsProcessed();

                if (defaultValue.TestCoerce != 0)
                {
                    throw new TestValidationException("Default value is not 0. It is " + defaultValue.TestCoerce.ToString());
                }
                expectedPropertyValue = 5;
                VerifyPropertiesValue();
            }
            else if ((dependencyPropertyEvaluation & DependencyPropertyEvaluation.Animation) == DependencyPropertyEvaluation.Animation 
                && (dependencyPropertyEvaluation & DependencyPropertyEvaluation.Coerce) == DependencyPropertyEvaluation.None)
            {
                Int32Animation animation = new Int32Animation();
                animation.From = 3;
                animation.To = 25;
                expectedPropertyValue = 25;
                RunAnimation(animation);
            }
            else if ((dependencyPropertyEvaluation & DependencyPropertyEvaluation.Animation) == DependencyPropertyEvaluation.None
                && (dependencyPropertyEvaluation & DependencyPropertyEvaluation.Coerce) == DependencyPropertyEvaluation.Coerce)
            {
                // make local value bigger than Coerce value 50
                // verify end property value is 50
                UserInput.MouseMove(testCoerceStyleTriggerValue, Convert.ToInt32(testCoerceStyleTriggerValue.ActualWidth / 2), Convert.ToInt32(testCoerceStyleTriggerValue.ActualHeight / 2));
                QueueHelper.WaitTillQueueItemsProcessed();

                if (testCoerceInheritedValue.TestCoerce != 50)
                {
                    throw new TestValidationException("Inherited value is not 50. It is " + testCoerceInheritedValue.TestCoerce.ToString());
                }
                if (testCoerceStyleValue.TestCoerce != 50)
                {
                    throw new TestValidationException("Style value is not 50. It is " + testCoerceStyleValue.TestCoerce.ToString());
                }
                if (testCoerceStyleTriggerValue.TestCoerce != 50)
                {
                    throw new TestValidationException("Style Trigger value is not 50. It is " + testCoerceStyleTriggerValue.TestCoerce.ToString());
                }
                if (testCoerceLocalValue.TestCoerce != 50)
                {
                    throw new TestValidationException("Local value is not 50. It is " + testCoerceLocalValue.TestCoerce.ToString());
                }
            }
            else if ((dependencyPropertyEvaluation & DependencyPropertyEvaluation.Animation) == DependencyPropertyEvaluation.Animation
                && (dependencyPropertyEvaluation & DependencyPropertyEvaluation.Coerce) == DependencyPropertyEvaluation.Coerce)
            {
                Int32Animation animation = new Int32Animation();
                animation.From = 3;
                animation.To = 60;
                expectedPropertyValue = 50;
                RunAnimation(animation);
            }
        }

        private void ModifyPropertiesValue(int newValue)
        {
            inheritedPanel.TestCoerce = newValue;
            styleValue.TestCoerce = newValue;

            Int32Animation animation = new Int32Animation();
            animation.From = newValue;
            animation.To = newValue;

            AttachStyleTrigger(animation);

            localValue.TestCoerce = newValue;
            expressionValue.TestCoerce = newValue;
        }

        private void AttachStyleTrigger(Int32Animation animation)
        {
            Trigger trigger = new Trigger();
            trigger.Property = TestCoerceButton.IsMouseOverProperty;
            trigger.Value = true;

            Storyboard storyboard = new Storyboard();
            storyboard.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(TestCoerceButton.TestCoerceProperty));
            storyboard.Children.Add(animation);

            BeginStoryboard beginStoryboard = new BeginStoryboard();
            beginStoryboard.Storyboard = storyboard;

            trigger.EnterActions.Add(beginStoryboard);

            Style style = new Style();
            style.TargetType = typeof(TestCoerceButton);
            style.Triggers.Add(trigger);

            styleTriggerValue.Style = style;
        }

        private void RunAnimation(Int32Animation animation)
        {
            defaultValue.BeginAnimation(TestCoerceButton.TestCoerceProperty, animation);
            inheritedPanel.BeginAnimation(TestCoerceButton.TestCoerceProperty, animation);
            styleValue.BeginAnimation(TestCoerceButton.TestCoerceProperty, animation);
            localValue.BeginAnimation(TestCoerceButton.TestCoerceProperty, animation);
            expressionValue.BeginAnimation(TestCoerceButton.TestCoerceProperty, animation);

            // Attach the completed event for style trigger button for the animation for that.
            animation.Completed += new EventHandler(animation_Completed);

            DispatcherFrame frame = QueueHelper.BlockDispatcherFrameForAnimation(animation);

            AttachStyleTrigger(animation);

            UserInput.MouseMove(styleTriggerValue, Convert.ToInt32(styleTriggerValue.ActualWidth / 2), Convert.ToInt32(styleTriggerValue.ActualHeight / 2));
            QueueHelper.WaitForAnimationCompleted(frame);
        }

        void animation_Completed(object sender, EventArgs e)
        {
            if (defaultValue.TestCoerce != expectedPropertyValue)
            {
                throw new TestValidationException("Default value is not " + expectedPropertyValue.ToString() + ". It is " + defaultValue.TestCoerce.ToString());
            }
            VerifyPropertiesValue();
        }

        private void VerifyPropertiesValue()
        {
            if (inheritedValue.TestCoerce != expectedPropertyValue)
            {
                throw new TestValidationException("Inherited value is not " + expectedPropertyValue.ToString() + ". It is " + inheritedValue.TestCoerce.ToString());
            }
            if (styleValue.TestCoerce != expectedPropertyValue)
            {
                throw new TestValidationException("Style value is not " + expectedPropertyValue.ToString() + ". It is " + styleValue.TestCoerce.ToString());
            }
            if (styleTriggerValue.TestCoerce != expectedPropertyValue)
            {
                throw new TestValidationException("Style Trigger value is not " + expectedPropertyValue.ToString() + ". It is " + styleTriggerValue.TestCoerce.ToString());
            }
            if (localValue.TestCoerce != expectedPropertyValue)
            {
                throw new TestValidationException("Local value is not " + expectedPropertyValue.ToString() + ". It is " + localValue.TestCoerce.ToString());
            }
            if (expressionValue.TestCoerce != expectedPropertyValue)
            {
                throw new TestValidationException("Local value is not " + expectedPropertyValue.ToString() + ". It is " + localValue.TestCoerce.ToString());
            }
        }

        #endregion
        
    } 
}
