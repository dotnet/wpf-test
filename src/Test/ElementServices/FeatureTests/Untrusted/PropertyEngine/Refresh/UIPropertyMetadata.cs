// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshUIPropertyMetadataTest
{
    /******************************************************************************
    * CLASS:          UIPropertyMetadata
    ******************************************************************************/
    [Test(0, "PropertyEngine.UIPropertyMetadata", TestCaseSecurityLevel.FullTrust, "UIPropertyMetadataTest")]
    public class UIPropertyMetadataTest : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("TestConstructors")]
        [Variation("TestAnimation")]
        [Variation("NegativeTestCase")]

        /******************************************************************************
        * Function:          UIPropertyMetadataTest Constructor
        ******************************************************************************/
        public UIPropertyMetadataTest(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            TestUIPropertyMetadata test = new TestUIPropertyMetadata();

            Utilities.StartRunAllTests("UIPropertyMetadata");

            switch (_testName)
            {
                case "TestConstructors":
                    test.TestConstructors();
                    break;
                case "TestAnimation":
                    test.TestAnimation();
                    break;
                case "NegativeTestCase":
                    test.NegativeTestCase();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            Utilities.StopRunAllTests();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestUIPropertyMetadata
    ******************************************************************************/
    public class TestUIPropertyMetadata
    {
        /// <summary>
        /// Test 12 Constrcutors
        /// </summary>
        public void TestConstructors()
        {
            Utilities.PrintTitle("Test UIPropertyMetadata Constructors (14 Overloads)");

            UIPropertyMetadata uiPropertyMetadata = null;
            DependencyProperty TestProperty = null;
            PropertyChangedCallback propertyChangedCallback = new PropertyChangedCallback(PropertyChanged);

            Utilities.PrintStatus("(1) No Argument");
            uiPropertyMetadata = new UIPropertyMetadata();
            TestProperty = DependencyProperty.RegisterAttached("Test1", typeof(object), typeof(TestUIPropertyMetadata), uiPropertyMetadata);
            Utilities.Assert(TestProperty.DefaultMetadata.DefaultValue == null, "TestProperty.DefaultMetadata.DefaultValue == null");
            Utilities.Assert(TestProperty.DefaultMetadata.PropertyChangedCallback == null, "TestProperty.DefaultMetadata.PropertyChangedCallback == null");
            Utilities.Assert(TestProperty.ReadOnly == false, "TestProperty.ReadOnly == false");
            Utilities.Assert(uiPropertyMetadata.IsAnimationProhibited == false, "IsAnimationProhibited defaults to False");
            
            Utilities.PrintStatus("(2) defaultValue");
            uiPropertyMetadata = new UIPropertyMetadata(Matrix.Identity);
            TestProperty = DependencyProperty.RegisterAttached("Test2", typeof(object), typeof(TestUIPropertyMetadata), uiPropertyMetadata);
            Utilities.Assert((Matrix)TestProperty.DefaultMetadata.DefaultValue == Matrix.Identity, "(Matrix)TestProperty.DefaultMetadata.DefaultValue == Matrix.Identity");
            Utilities.Assert(TestProperty.DefaultMetadata.PropertyChangedCallback == null, "TestProperty.DefaultMetadata.PropertyChangedCallback == null");
            Utilities.Assert(TestProperty.ReadOnly == false, "TestProperty.ReadOnly == false");
            Utilities.Assert(uiPropertyMetadata.IsAnimationProhibited == false, "IsAnimationProhibited defaults to False");

            Utilities.PrintStatus("(3) PropertyChangedCallback");
            uiPropertyMetadata = new UIPropertyMetadata(propertyChangedCallback);
            TestProperty = DependencyProperty.RegisterAttached("Test3", typeof(object), typeof(TestUIPropertyMetadata), uiPropertyMetadata);
            Utilities.Assert(TestProperty.DefaultMetadata.DefaultValue == null, "TestProperty.DefaultMetadata.DefaultValue == null");
            Utilities.Assert(TestProperty.DefaultMetadata.PropertyChangedCallback == propertyChangedCallback, "TestProperty.DefaultMetadata.PropertyChangedCallback == propertyInvalidatedCallback");
            Utilities.Assert(TestProperty.ReadOnly == false, "TestProperty.ReadOnly == false");
            Utilities.Assert(uiPropertyMetadata.IsAnimationProhibited == false, "IsAnimationProhibited defaults to False");

            Utilities.PrintStatus("(9) defaultValue, PropertyChangedCallback");
            uiPropertyMetadata = new UIPropertyMetadata(System.Windows.Media.Colors.AliceBlue, propertyChangedCallback);
            TestProperty = DependencyProperty.RegisterAttached("Test9", typeof(System.Windows.Media.Color), typeof(TestUIPropertyMetadata), uiPropertyMetadata);
            Utilities.Assert((Color)TestProperty.DefaultMetadata.DefaultValue == System.Windows.Media.Colors.AliceBlue, "Default value verified");
            Utilities.Assert(TestProperty.DefaultMetadata.PropertyChangedCallback == propertyChangedCallback, "propertyInvalidatedCallback Verified");
            Utilities.Assert(uiPropertyMetadata.IsAnimationProhibited == false, "IsAnimationProhibited defaults to False");

            //TOADD: 2 more constructors
        }

        /// <summary>
        /// Test Animation
        /// </summary>
        public void TestAnimation()
        {
            Utilities.PrintTitle("Test Animation Peer DP and Owner Peer indirectly");

            System.Windows.Shapes.Rectangle rectangle1;
            DoubleAnimation opacityAnimation;

            rectangle1 = new System.Windows.Shapes.Rectangle();
            opacityAnimation = new DoubleAnimation(1.0, 0, new Duration(new TimeSpan(0, 0, 0, 0, 5000)));
            opacityAnimation.RepeatBehavior = RepeatBehavior.Forever;
            opacityAnimation.BeginTime = TimeSpan.FromMilliseconds(5000);
            rectangle1.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

            //Add One more test here:
            Button testButton = new Button();
            testButton.Width = 200;
            DoubleAnimation da = new DoubleAnimation(200, 400, new Duration(new TimeSpan(0, 0, 0, 0, 4000)));

            da.BeginTime = null;
            da.Name = "Length";
            da.Freeze();
            
            AnimationClock clock = da.CreateClock();
            Utilities.Assert(clock != null, "Get the clock");
            testButton.ApplyAnimationClock(Button.WidthProperty, clock, HandoffBehavior.SnapshotAndReplace);
            clock.Controller.Begin();
            testButton.ApplyTemplate();
        }

        internal void NegativeTestCase()
        {
            Utilities.PrintTitle("Negative: IsAnimationProhibited cannot be set after being sealed");
            UIPropertyMetadata uip = new UIPropertyMetadata();
            uip.IsAnimationProhibited = false;
            Utilities.Assert(uip.IsAnimationProhibited == false, "Set and get IsAnimationProhibited value (Value is false)");
            uip.IsAnimationProhibited = true;
            Utilities.Assert(uip.IsAnimationProhibited == true, "Set and get IsAnimationProhibited value (Value is true)");

            //Property metadata cannot be changed after it has been associated with a property.
            try
            {
                AvalonObject.MyMetadata.IsAnimationProhibited = true;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            //BeginAnimation with incorrect HandoffBehavior
            Button testButton = new Button();
            testButton.Width = 200;
            DoubleAnimation da = new DoubleAnimation(200, 400, new Duration(new TimeSpan(0, 0, 0, 0, 4000)));
            da.BeginTime = new TimeSpan(0);
            da.Name = "Length";
            da.Freeze();
            try
            {
                testButton.BeginAnimation(Button.WidthProperty, da, (HandoffBehavior)99);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

        }

        private void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    /******************************************************************************
    * CLASS:          AvalonObject
    ******************************************************************************/
    /// <summary>
    /// DependencyObject used in test
    /// </summary>
    public class AvalonObject : FrameworkElement
    {
        static internal FrameworkPropertyMetadata MyMetadata = new FrameworkPropertyMetadata();
        static AvalonObject()
        {
            StyleProperty.OverrideMetadata(typeof(AvalonObject), MyMetadata);
        }
   }
}
