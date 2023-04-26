// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;   //UserInput
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.GetSet for KeyFrames</area>
    /// <priority>2</priority>
    /// <description>
    /// This test tests the constructors for standard animation types, as well as get/set for properties & methods
    /// check FrameworkElements, etc.
    /// </description>
    /// </summary>
    [Test(2, "Animation.PropertyMethodEvent.GetSet", "KeyFrameAnimationGetSet", Keywords = "Localization_Suite")]
    public class KeyFrameAnimationGetSet : WindowTest
    {
        #region Private Data

        private object _animation1;
        private AnimationClock _animClock1;

        private object _keyFrameCollection;

        private object _linearKeyFrame1;
        private object _linearKeyFrame2;
        private object _linearKeyFrame3;

        private object _discreteKeyFrame1;
        private object _discreteKeyFrame2;
        private object _discreteKeyFrame3;

        private object _splineKeyFrame1;
        private object _splineKeyFrame2;
        private object _splineKeyFrame3;
        private object _splineKeyFrame4;


        private object _value1;
        private object _value2;
        private DispatcherTimer _aTimer = null;
        private bool _isDiscreteOnly = false;

        private string _animationType = null;

        private Type _keyFrameAnimationType = null;
        private Type _discreteType = null;
        private Type _linearType = null;
        private Type _splineType = null;
        private Type _collectionType = null;

        KeyTime _keyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 4000));
        KeySpline _keySpline = new KeySpline(0, 0, 1, 1);

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
        [Variation("Boolean")]
        [Variation("String")]
        [Variation("Object")]
        [Variation("Matrix")]
        [Variation("Char")]


        /******************************************************************************
        * Function:          AnimationGetSetTest Constructor
        ******************************************************************************/
        public KeyFrameAnimationGetSet(string testValue)
        {
            _animationType = testValue;

            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(GetAndSetValues);
            RunSteps += new TestStep(CollectionGetAndSet);
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
                    _value1 = (byte)2;
                    _value2 = (byte)4;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Color":
                    _value1 = Colors.Red;
                    _value2 = Colors.Blue;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Decimal":
                    _value1 = 4.1M;
                    _value2 = 0M;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Double":
                    _value1 = (double)10.3;
                    _value2 = (double)0;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Int16":
                    _value1 = (Int16)2;
                    _value2 = (Int16)0;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Int32":
                    _value1 = 400;
                    _value2 = 0;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Int64":
                    _value1 = (Int64)5000;
                    _value2 = (Int64)0;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Point":
                    _value1 = new Point(50, 50);
                    _value2 = new Point(0, 0);
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Point3D":
                    _value1 = new Point3D(50, 50, 50);
                    _value2 = new Point3D(0, 0, 0);
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Quaternion":
                    _value1 = new Quaternion(5, 2, 7, 4);
                    _value2 = new Quaternion(1, 2, 3, 4);
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Rect":
                    _value1 = new Rect(0, 0, 50, 50);
                    _value2 = new Rect(0, 0, 0, 0);
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Rotation3D":
                    _value1 = new QuaternionRotation3D(new Quaternion(5, 2, 7, 4));
                    _value2 = new QuaternionRotation3D(new Quaternion(1, 2, 3, 4));
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Single":
                    _value1 = (Single)5;
                    _value2 = (Single)10;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Size":
                    _value1 = new Size(50, 50);
                    _value2 = new Size(150, 150);
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Vector":
                    _value1 = new Vector(50, 50);
                    _value2 = new Vector(150, 150);
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Vector3D":
                    _value1 = new Vector3D(0, 50, 50);
                    _value2 = new Vector3D(10, 150, 50);
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Thickness":
                    _value1 = new Thickness(5, 10, 5, 10);
                    _value2 = new Thickness(25, 10, 25, 10);
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Boolean":
                    _value1 = true;
                    _value2 = false;
                    _isDiscreteOnly = true;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Char":
                    _value1 = 'a';
                    _value2 = 'b';
                    _isDiscreteOnly = true;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Matrix":
                    _value1 = new Matrix(0, 1, 1, 0, 2, 2);
                    _value2 = new Matrix(2, 1, 1, 0, 5, 2);
                    _isDiscreteOnly = true;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "Object":
                    _value1 = 500;
                    _value2 = 50;
                    _isDiscreteOnly = true;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                case "String":
                    _value1 = "String1";
                    _value2 = "String2";
                    _isDiscreteOnly = true;
                    KeyFrameAnimationGetSetTests(_animationType);
                    break;
                default:
                    // Error No Such animation type exists
                    GlobalLog.LogStatus("Invalid animation type:" + _animationType);
                    return TestResult.Fail;

            }


            return TestResult.Pass;
        }

        private void KeyFrameAnimationGetSetTests(string baseType)
        {

            // assume it is in PresentationCore
            Assembly asm = typeof(System.Windows.Media.Animation.AnimationTimeline).Assembly;
            _keyFrameAnimationType = asm.GetType("System.Windows.Media.Animation." + baseType + "AnimationUsingKeyFrames");

            // if it was not in PresentationCore, lets try PresentationFramework
            if (_keyFrameAnimationType == null)
            {
                asm = typeof(System.Windows.Media.Animation.ThicknessAnimation).Assembly;
                _keyFrameAnimationType = asm.GetType("System.Windows.Media.Animation." + baseType + "AnimationUsingKeyFrames");

            }

            // if not there, throw an exception
            if (_keyFrameAnimationType == null)
            {
                throw new ArgumentException("Can't load type " + baseType + "AnimationUsingKeyFrames");
            }

            // We need the following for full code coverage
            //
            // 1) Create the AnimationUsingKeyFrame
            // 2) Create a keyFrameCollection 
            // 3) Create key frames for each Linear (3) , Discrete (3) , and Spline (4)
            //
            //


            _collectionType = asm.GetType("System.Windows.Media.Animation." + baseType + "KeyFrameCollection");
            _keyFrameCollection = Activator.CreateInstance(_collectionType);

            Object holder = new Object();

            _discreteType = asm.GetType("System.Windows.Media.Animation.Discrete" + baseType + "KeyFrame");
            createKeyFrameObjects(_discreteType, ref _discreteKeyFrame1, ref _discreteKeyFrame2, ref _discreteKeyFrame3, ref holder);

            ((IList)(_keyFrameCollection)).Add(_discreteKeyFrame1);
            ((IList)(_keyFrameCollection)).Add(_discreteKeyFrame2);
            ((IList)(_keyFrameCollection)).Add(_discreteKeyFrame3);


            if (!_isDiscreteOnly)
            {
                _linearType = asm.GetType("System.Windows.Media.Animation.Linear" + baseType + "KeyFrame");
                createKeyFrameObjects(_linearType, ref _linearKeyFrame1, ref _linearKeyFrame2, ref _linearKeyFrame3, ref holder);

                ((IList)(_keyFrameCollection)).Add(_linearKeyFrame1);
                ((IList)(_keyFrameCollection)).Add(_linearKeyFrame2);
                ((IList)(_keyFrameCollection)).Add(_linearKeyFrame3);


                _splineType = asm.GetType("System.Windows.Media.Animation.Spline" + baseType + "KeyFrame");
                createKeyFrameObjects(_splineType, ref _splineKeyFrame1, ref _splineKeyFrame2, ref _splineKeyFrame3, ref _splineKeyFrame4);

                ((IList)(_keyFrameCollection)).Add(_splineKeyFrame1);
                ((IList)(_keyFrameCollection)).Add(_splineKeyFrame2);
                ((IList)(_keyFrameCollection)).Add(_splineKeyFrame3);
                ((IList)(_keyFrameCollection)).Add(_splineKeyFrame4);

            }


            _animation1 = Activator.CreateInstance(_keyFrameAnimationType);   //default constructor ()

            PropertyInfo keyFrames = _keyFrameAnimationType.GetProperty("KeyFrames");

            // get the value, which returns an empty collection
            holder = keyFrames.GetValue(_animation1, null);

            // now set the value
            keyFrames.SetValue(_animation1, _keyFrameCollection, null);

            _animClock1 = ((AnimationTimeline)_animation1).CreateClock();

        }

        /******************************************************************************
        * Function:          createKeyFrameObjects
        ******************************************************************************/
        /// <summary>
        /// create all instances of constructors for a given keyFrameType, and does a get/set for each
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private void createKeyFrameObjects(Type keyFrameType, ref object defaultConstructor, ref object constructor1, ref object constructor2, ref object constructor3)
        {

            defaultConstructor = Activator.CreateInstance(keyFrameType); // default constructor

            foreach (ConstructorInfo constructor in keyFrameType.GetConstructors())
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                if (parameters.Length == 1)
                {
                    constructor1 = constructor.Invoke(new Object[1] { _value1 });   // 2nd constructor
                }
                else if (parameters.Length == 2)
                {
                    constructor2 = constructor.Invoke(new Object[2] { _value1, _keyTime });   // 2nd constructor
                }
                else if (parameters.Length == 3)
                {
                    constructor3 = constructor.Invoke(new Object[3] { _value1, _keyTime, _keySpline });   // 3nd constructor
                }
            }

        }


        /******************************************************************************
        * Function:          GetAndSetValues
        ******************************************************************************/
        /// <summary>
        /// Get and set values for the 3 different types of KeyFrames
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult GetAndSetValues()
        {

            bool result = true;
            Object holder = new Object();

            result = result && getSetKeyFrameObjects(_discreteType, ref _discreteKeyFrame1, ref _discreteKeyFrame2, ref _discreteKeyFrame3, ref holder);

            if (!_isDiscreteOnly)
            {

                result = result && getSetKeyFrameObjects(_linearType, ref _linearKeyFrame1, ref _linearKeyFrame2, ref _linearKeyFrame3, ref holder);

                result = result && getSetKeyFrameObjects(_splineType, ref _splineKeyFrame1, ref _splineKeyFrame2, ref _splineKeyFrame3, ref _splineKeyFrame4);
            }

            return result ? TestResult.Pass : TestResult.Fail;


        }


        /******************************************************************************
        * Function:          CollectionGetAndSet
        ******************************************************************************/
        /// <summary>
        /// Get and set values for the KeyFrameCollection
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult CollectionGetAndSet()
        {

            bool result = true;
            MethodInfo methodInfo;
            PropertyInfo propertyInfo;

            // null checks:

            ExceptionHelper.ExpectException(delegate() { ((IList)(_keyFrameCollection)).Add(null); }, new ArgumentNullException("keyFrame"));
            ExceptionHelper.ExpectException(delegate() { ((IList)(_keyFrameCollection)).Insert(0,null); }, new ArgumentNullException("keyFrame"));
            ExceptionHelper.ExpectException(delegate() { ((IList)(_keyFrameCollection))[0] = null; }, new ArgumentNullException(_animationType + "KeyFrameCollection[0]"));


            if ( !((IList)(_keyFrameCollection)).Contains(((IList)(_keyFrameCollection))[0]) )
            {
                GlobalLog.LogStatus("Contains did not work for the Collection");
                result = false;
            }


            if ( ((IList)(_keyFrameCollection)).IndexOf(((IList)(_keyFrameCollection))[0]) != 0 )
            {
                GlobalLog.LogStatus("Incorrect Index");
                result = false;
            }

            ((IList)(_keyFrameCollection)).Insert(0,_discreteKeyFrame1);

            ((IList)(_keyFrameCollection)).Remove(_discreteKeyFrame1);

            ((IList)(_keyFrameCollection))[0] = _discreteKeyFrame1;

            methodInfo = _collectionType.GetMethod("RemoveAt");
            methodInfo.Invoke(_keyFrameCollection, new Object[1]{0});

            propertyInfo = _collectionType.GetProperty("Count");
            if ( ((int)propertyInfo.GetValue(_keyFrameCollection, null)) <= 0 )
            {
                GlobalLog.LogStatus("Count not greater than 0");
                result = false;
            }

            propertyInfo = _collectionType.GetProperty("IsSynchronized");
            if ( ((bool)propertyInfo.GetValue(_keyFrameCollection, null)) == false )
            {
                GlobalLog.LogStatus("IsSynchronized Failed");
                result = false;
            }

            propertyInfo = _collectionType.GetProperty("SyncRoot");
            if ( ((Object)propertyInfo.GetValue(_keyFrameCollection, null)).Equals(null) )
            {
                GlobalLog.LogStatus("Sync root is null");
                result = false;
            }

            Object[] tempArray = new Object[10];

            ((ICollection)(_keyFrameCollection)).CopyTo(tempArray,0);


            methodInfo = _collectionType.GetMethod("Clear");
            methodInfo.Invoke(_keyFrameCollection, null);


            return result ? TestResult.Pass : TestResult.Fail;


        }


        /******************************************************************************
        * Function:          createKeyFrameObjects
        ******************************************************************************/
        /// <summary>
        /// create all instances of constructors for a given keyFrameType, and does a get/set for each
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private bool getSetKeyFrameObjects(Type keyFrameType, ref object defaultConstructor, ref object constructor1, ref object constructor2, ref object constructor3)
        {

            bool overallResult = true;
            PropertyInfo propertyInfo;
            object actualValue;


            // *KeyFrame props & method

            propertyInfo = keyFrameType.GetProperty("KeyTime");

            propertyInfo.SetValue(defaultConstructor, KeyTime.Uniform, null);

            if ((KeyTime)propertyInfo.GetValue(defaultConstructor, null) != KeyTime.Uniform)
            {
                GlobalLog.LogStatus("Get set for KeyTime failed. Expected " + KeyTime.Uniform + " Actual: " + ((KeyTime)propertyInfo.GetValue(defaultConstructor, null)).ToString());
                overallResult = false;
            }

            propertyInfo = keyFrameType.GetProperty("Value");

            propertyInfo.SetValue(defaultConstructor, _value1, null);


            actualValue = (object)propertyInfo.GetValue(defaultConstructor, null);

            if (!actualValue.Equals(_value1))
            {
                GlobalLog.LogStatus("GetValue did not return the value we set. Expected " + _value1 + " Actual " +actualValue);
                overallResult = false;
            }


            ((IKeyFrame)constructor1).Value = _value2;

            actualValue = (object)((IKeyFrame)constructor1).Value;

            if (!actualValue.Equals(_value2))
            {
                GlobalLog.LogStatus("Get Set for IKeyFrame Value failed. Expected " + _value2 + " Actual " + actualValue);
                overallResult = false;
            }



            // InterpolateValue  < 1 and @ 1.0
            // InterpolateValue with null - ArgumentOutOfRangeException


            MethodInfo interpolateMethod = keyFrameType.GetMethod("InterpolateValue");

            actualValue = interpolateMethod.Invoke(constructor2, new Object[2] { _value2, 0.5 });
            if (actualValue.Equals(_value1))
            {
                GlobalLog.LogStatus("InterpolateValue at time 0.5 returned the BaseValue - expected some interpolated value");
                overallResult = false;
            }

            actualValue = interpolateMethod.Invoke(constructor2, new Object[2] { _value2, 1.0 });
            if (actualValue.Equals(_value2))
            {
                GlobalLog.LogStatus("InterpolateValue at time 1.0 should have returned anything other than the final value");
                overallResult = false;
            }

            // The new exception handler will not work for ref parameters
            //ExceptionHelper.ExpectException(delegate() { interpolateMethod.Invoke(constructor2, new Object[2] { value2, -1.0 }); }, new TargetInvocationException(new ArgumentOutOfRangeException("keyFrameProgress")));

            try
            {
                actualValue = interpolateMethod.Invoke(constructor2, new Object[2] { _value2, -1.0 });
            }
            catch (TargetInvocationException)
            {
                GlobalLog.LogStatus("Expected Exception for invalid keyTime caught for " + keyFrameType);
            }

            return overallResult;

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
            _aTimer.Interval = new TimeSpan(0, 0, 0, 0, 3000);
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
            PropertyInfo propertyInfo;

            WaitForSignal("AnimationDone");

            propertyInfo = _keyFrameAnimationType.GetProperty("KeyFrames");

            if (!(propertyInfo.GetValue(_animation1, null).Equals(_keyFrameCollection)))
            {
                GlobalLog.LogStatus("KeyFrame Collection Get is not the one we Set");
                overallPass = false;
            }

            if (((object)_animClock1.GetCurrentValue(_value1, _value2)).Equals(_value2))
            {
                GlobalLog.LogStatus("GetCurrentValue returned final value instead of an interpolated value");
                overallPass = false;
            }

            // null checks

            ExceptionHelper.ExpectException(delegate() { ((IAddChild)(_animation1)).AddChild(null); }, new ArgumentNullException("child"));
            ExceptionHelper.ExpectException(delegate() { ((IAddChild)(_animation1)).AddText(null); }, new ArgumentNullException("childText"));
            ExceptionHelper.ExpectException(delegate() { ((IAddChild)(_animation1)).AddText("text"); }, new InvalidOperationException());


            if ( ((IKeyFrameAnimation)_animation1).KeyFrames == null )
            {
                GlobalLog.LogStatus("IKeyFrameAnimation Get returned no KeyFrames, expected at least one");
                overallPass = false;
            }

            // try setting the key Frames

            ((IKeyFrameAnimation)_animation1).KeyFrames = (IList)_keyFrameCollection;


            propertyInfo = _collectionType.GetProperty("Empty");
            _keyFrameCollection = propertyInfo.GetValue(_keyFrameCollection, null);

            return overallPass ? TestResult.Pass : TestResult.Fail;
        }

        #endregion
    }
}
