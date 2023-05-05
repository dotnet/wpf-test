// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;   //UserInput
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.GetSet</area>
    /// <priority>2</priority>
    /// <description>
    /// This test tests the constructors for standard animation types, as well as get/set for properties & methods
    /// check FrameworkElements, etc.
    /// </description>
    /// </summary>
    [Test(2, "Animation.PropertyMethodEvent.GetSet", "AnimationGetSet", Keywords = "Localization_Suite")]
    public class AnimationGetSet : WindowTest
    {
        #region Private Members

        private object _animation1;
        private AnimationClock _animClock1;
        private object _animation2;
        private AnimationClock _animClock2;
        private object _animation3;
        private AnimationClock _animClock3;
        private object _animation4;
        private AnimationClock _animClock4;
        private object _animation5;
        private AnimationClock _animClock5;
        private object _animation6;
        private AnimationClock _animClock6;
        private object _animation7;
        private AnimationClock _animClock7;

        private string _animationType;
        private object _fromValue;
        private object _toValue;
        private DispatcherTimer _aTimer = null;

        private Type _targetType = null;
        private PropertyInfo _propertyInfo = null;

        #endregion


        #region Constructors

        [Variation("Byte")]
        [Variation("Color")]
        [Variation("Decimal")]
        [Variation("Double")]
        [Variation("Int16")]
        [Variation("Int32")]
        [Variation("Int64")]
        [Variation("Point")]
        [Variation("Point3D")]
        [Variation("Quaternion")]
        [Variation("Rect")]
        [Variation("Rotation3D")]
        [Variation("Single")]
        [Variation("Size")]
        [Variation("Vector")]
        [Variation("Vector3D")]
        [Variation("Thickness")]


        /******************************************************************************
        * Function:          GetAnimationBaseValue1Test Constructor
        ******************************************************************************/
        public AnimationGetSet(string testValue)
        {
            _animationType = testValue;

            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Private Members

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        private TestResult CreateTree()
        {
            Window.Width = 100d;
            Window.Height = 100d;
            Window.Left = 0d;
            Window.Top = 0d;

            // creating a window ensures the animation system is up and running

            Canvas body = new Canvas();
            body.Background = Brushes.Black;

            Window.Content = body;

            switch (_animationType)
            {
                case "Byte":
                    _fromValue = (byte)2;
                    _toValue = (byte)4;
                    AnimationGetSetTests(_animationType);
                    break;
                case "Color":
                    _fromValue = Colors.Red;
                    _toValue = Colors.Blue;
                    AnimationGetSetTests(_animationType);
                    break;
                case "Decimal":
                    _fromValue = 4.1M;
                    _toValue = 0M;
                    AnimationGetSetTests(_animationType);
                    break;
                case "Double":
                    _fromValue = (double)10.3;
                    _toValue = (double)0;
                    AnimationGetSetTests(_animationType);
                    break;
                case "Int16":
                    _fromValue = (Int16)2;
                    _toValue = (Int16)0;
                    AnimationGetSetTests(_animationType);
                    break;
                case "Int32":
                    _fromValue = 400;
                    _toValue = 0;
                    AnimationGetSetTests(_animationType);
                    break;
                case "Int64":
                    _fromValue = (Int64)5000;
                    _toValue = (Int64)0;
                    AnimationGetSetTests(_animationType);
                    break;
                case "Point":
                    _fromValue = new Point(50, 50);
                    _toValue = new Point(0, 0);
                    AnimationGetSetTests(_animationType);
                    break;
                case "Point3D":
                    _fromValue = new Point3D(50, 50, 50);
                    _toValue = new Point3D(0, 0, 0);
                    AnimationGetSetTests(_animationType);
                    break;
                case "Quaternion":
                    _fromValue = new Quaternion(5, 2, 7, 4);
                    _toValue = new Quaternion(1, 2, 3, 4);
                    AnimationGetSetTests(_animationType);
                    break;
                case "Rect":
                    _fromValue = new Rect(0, 0, 50, 50);
                    _toValue = new Rect(0, 0, 0, 0);
                    AnimationGetSetTests(_animationType);
                    break;
                case "Rotation3D":
                    _fromValue = new QuaternionRotation3D(new Quaternion(5, 2, 7, 4));
                    _toValue = new QuaternionRotation3D(new Quaternion(1, 2, 3, 4));
                    AnimationGetSetTests(_animationType);
                    break;
                case "Single":
                    _fromValue = (Single)5;
                    _toValue = (Single)0;
                    AnimationGetSetTests(_animationType);
                    break;
                case "Size":
                    _fromValue = new Size(50, 50);
                    _toValue = new Size(0, 0);
                    AnimationGetSetTests(_animationType);
                    break;
                case "Vector":
                    _fromValue = new Vector(50, 50);
                    _toValue = new Vector(0, 0);
                    AnimationGetSetTests(_animationType);
                    break;
                case "Vector3D":
                    _fromValue = new Vector3D(0, 50, 50);
                    _toValue = new Vector3D(0, 0, 0);
                    AnimationGetSetTests(_animationType);
                    break;
                case "Thickness":
                    _fromValue = new Thickness(5, 10, 5, 10);
                    _toValue = new Thickness(0, 0, 0, 0);
                    AnimationGetSetTests(_animationType);
                    break;
                default:
                    // Error No Such animation type exists
                    GlobalLog.LogStatus("Invalid animation type: " + _animationType);
                    return TestResult.Fail;

            }


            return TestResult.Pass;
        }

        private void AnimationGetSetTests(string baseType)
        {
            // assume it is in PresentationCore
            Assembly asm = typeof(System.Windows.Media.Animation.AnimationTimeline).Assembly;
            _targetType = asm.GetType("System.Windows.Media.Animation." + baseType + "Animation");

            // if it was not in PresentationCore, lets try PresentationFramework
            if (_targetType == null)
            {
                asm = typeof(System.Windows.Media.Animation.ThicknessAnimation).Assembly;
                _targetType = asm.GetType("System.Windows.Media.Animation." + baseType + "Animation");

            }

            // if not there, throw an exception
            if (_targetType == null)
            {
                throw new ArgumentException("Can't load type " + baseType + "Animation");
            }

            // We need 5 types for full code coverage, plus one with IsCummulative set
            //
            // 1) From animation
            // 2) To animation
            // 3) By animation
            // 4) From-To animation with IsAdditive set
            // 5) From-By animation with IsAdditive set
            //
            //
            // There are 5 different constructors:
            //
            // 1) animation()
            // 2) animation(toValue, Duration)
            // 3) animation(toValue, Duration, FillBehavior)
            // 4) animation(fromValue, toValue, Duration)
            // 5) animation(fromValue, toValue, Duration, FillBehavior)
            //


            _animation1 = Activator.CreateInstance(_targetType);   //default constructor (From)
            _animation2 = Activator.CreateInstance(_targetType);   //default constructor (By)
            _animation3 = Activator.CreateInstance(_targetType);   //default constructor (FromBy)


            foreach (ConstructorInfo constructor in _targetType.GetConstructors())
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                if (parameters.Length == 2)
                {
                    _animation4 = constructor.Invoke(new Object[2] { _toValue, new Duration(TimeSpan.FromMilliseconds(10000)) });   // 2nd constructor
                }
                else if ((parameters.Length == 3) && (parameters[1].Name == "duration"))
                {
                    _animation5 = constructor.Invoke(new Object[3] { _toValue, new Duration(TimeSpan.FromMilliseconds(2000)), FillBehavior.HoldEnd });
                }
                else if ((parameters.Length == 3) && (parameters[2].Name == "duration"))
                {
                    _animation6 = constructor.Invoke(new Object[3] { _fromValue, _toValue, new Duration(TimeSpan.FromMilliseconds(10000)) });
                }
                else if (parameters.Length == 4)
                {
                    _animation7 = constructor.Invoke(new Object[4] { _fromValue, _toValue, new Duration(TimeSpan.FromMilliseconds(10000)), FillBehavior.HoldEnd });
                }
            }

            // animation1 - From
            // animation2 - By
            // animation3 - FromBy, IsAdditive
            // animation4 - To  (quick constructor 1)
            // animation5 - To  (qc2), IsCumulative w/ repeat
            // animation6 - FromTo (qc3), IsAdditive
            // animation7 - FromTo (qc4)

            _propertyInfo = _targetType.GetProperty("From");
            _propertyInfo.SetValue(_animation1, _fromValue, null);
            _propertyInfo.SetValue(_animation3, _fromValue, null);

            _propertyInfo = _targetType.GetProperty("By");       // use the To value for By animations
            _propertyInfo.SetValue(_animation2, _toValue, null);
            _propertyInfo.SetValue(_animation3, _toValue, null);

            _propertyInfo = _targetType.GetProperty("Duration");
            _propertyInfo.SetValue(_animation1, new Duration(TimeSpan.FromMilliseconds(10000)), null);
            _propertyInfo.SetValue(_animation2, new Duration(TimeSpan.FromMilliseconds(10000)), null);
            _propertyInfo.SetValue(_animation3, new Duration(TimeSpan.FromMilliseconds(10000)), null);

            _propertyInfo = _targetType.GetProperty("IsAdditive");
            _propertyInfo.SetValue(_animation3, true, null);
            _propertyInfo.SetValue(_animation6, true, null);

            _propertyInfo = _targetType.GetProperty("IsCumulative");
            _propertyInfo.SetValue(_animation5, true, null);

            _propertyInfo = _targetType.GetProperty("RepeatBehavior");
            _propertyInfo.SetValue(_animation5, RepeatBehavior.Forever, null);



            // create clocks which activates the animations

            _animClock1 = ((AnimationTimeline)_animation1).CreateClock();
            _animClock2 = ((AnimationTimeline)_animation2).CreateClock();
            _animClock3 = ((AnimationTimeline)_animation3).CreateClock();
            _animClock4 = ((AnimationTimeline)_animation4).CreateClock();
            _animClock5 = ((AnimationTimeline)_animation5).CreateClock();
            _animClock6 = ((AnimationTimeline)_animation6).CreateClock();
            _animClock7 = ((AnimationTimeline)_animation7).CreateClock();

            _animClock7.Controller.Stop();


        }



        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0, 0, 0, 0, 5000);
            _aTimer.Start();

            GlobalLog.LogStatus("----DispatcherTimer Started----");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)
        {
            Signal("AnimationDone", TestResult.Pass);
            _aTimer.Stop();
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        private TestResult Verify()
        {
            bool overallPass = true;

            WaitForSignal("AnimationDone");

            // animation1 - From
            // animation2 - By
            // animation3 - FromBy, IsAdditive
            // animation4 - To  (quick constructor 1)
            // animation5 - To  (qc2), IsCumulative w/ repeat
            // animation6 - FromTo (qc3), IsAdditive
            // animation7 - FromTo (qc4)

            _propertyInfo = _targetType.GetProperty("From");
            if (!(_propertyInfo.GetValue(_animation1, null).Equals(_fromValue)))
            {
                GlobalLog.LogStatus("From value Get does not match. Expected " + _fromValue + "  Actual: " + _propertyInfo.GetValue(_animation1, null));
                overallPass = false;
            }

            _propertyInfo = _targetType.GetProperty("By");
            if (!(_propertyInfo.GetValue(_animation2, null).Equals(_toValue)))
            {
                GlobalLog.LogStatus("By value Get does not match. Expected " + _toValue + "  Actual: " + _propertyInfo.GetValue(_animation2, null));
                overallPass = false;
            }

            _propertyInfo = _targetType.GetProperty("To");
            if (!(_propertyInfo.GetValue(_animation4, null).Equals(_toValue)))
            {
                GlobalLog.LogStatus("To value Get does not match. Expected " + _toValue + "  Actual: " + _propertyInfo.GetValue(_animation4, null));
                overallPass = false;
            }

            _propertyInfo = _targetType.GetProperty("IsAdditive");
            if ((bool)_propertyInfo.GetValue(_animation3, null) != true)
            {
                GlobalLog.LogStatus("IsAdditive value Get does not return true on " + _animation3.ToString());
                overallPass = false;
            }

            _propertyInfo = _targetType.GetProperty("IsCumulative");
            if ((bool)_propertyInfo.GetValue(_animation5, null) != true)
            {
                GlobalLog.LogStatus("IsCumulative value Get does not return true on" + _animation5.ToString());
                overallPass = false;
            }


            // invalid input checks - Calling SetExpected SetExpectedError  immediately results in a TestResult.Pass - so, doing it manually


            // call getCurrentValue with a null clock
            GlobalLog.LogStatus("call getCurrentValue with a null clock");
            ExceptionHelper.ExpectException(delegate() { ((AnimationTimeline)_animation6).GetCurrentValue(_fromValue, _fromValue, null); }, new ArgumentNullException("animationClock"));


            if (_animationType != "Rotation3D")
            {

                // call with null default/origin values
                GlobalLog.LogStatus("call with null origin values");
                ExceptionHelper.ExpectException(delegate() { (_animClock1).GetCurrentValue(null, _fromValue); }, new ArgumentNullException("defaultOriginValue"));

                GlobalLog.LogStatus("call with null destination values");
                ExceptionHelper.ExpectException(delegate() { (_animClock2).GetCurrentValue(_fromValue, null); }, new ArgumentNullException("defaultDestinationValue"));
            }

            // call with non null default/origin values, but of the wrong type. 
            GlobalLog.LogStatus("call with non null default values, but of the wrong type");
            ExceptionHelper.ExpectException(delegate() { (_animClock3).GetCurrentValue(new Button(), _fromValue); }, new InvalidCastException());

            GlobalLog.LogStatus("call with non null origin values, but of the wrong type");
            ExceptionHelper.ExpectException(delegate() { (_animClock4).GetCurrentValue(_fromValue, new Button()); }, new InvalidCastException());

            // call with stopped clock should return DestinationValue, not Origin

            if (((object)_animClock4.GetCurrentValue(_fromValue, _toValue)).Equals(_fromValue))
            {
                GlobalLog.LogStatus("GetCurrentValue returned the OriginValue instead of the DestinationValue for a stopped clock");
                overallPass = false;
            }


            return overallPass ? TestResult.Pass : TestResult.Fail;
        }

        #endregion
    }
}
