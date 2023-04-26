// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 * 
 *
 *   Program:           Boundary (and error) testing of Animation classes
 *   Dependencies:      TestRuntime.dll
 
 *
 ************************************************************/
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Animation.ObjectPatterns;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

//----------------------------------------------------------------------------------------------------

namespace                       Microsoft.Test.Animation
{
    //--------------------------------------------------------------------------------------------
    /// <summary>
    /// <area>Animation.Boundary</area>
    /// <priority>2</priority>
    /// <description>
    /// Verify Constructors for ByteAnimation.
    /// </description>
    /// </summary>

    [Test(2, "Animation.Boundary", "BoundaryTest")]
    public class BoundaryTest : AvalonTest
    {
        #region Test case members

        private string              _className           = null;
        private string              _requestedProperty   = null;
        private string              _requestedMethod     = null;
        private ListDictionary      _goodValues          = null;
        private ListDictionary      _badValues           = null;
        private bool                _testPassed          = true;
        private bool                _finalResult         = true;
        
        #endregion


        #region Constructor

        [Variation("ClassTest", "BooleanAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "BooleanKeyFrameCollection", "None")]
        [Variation("ClassTest", "ByteAnimation", "None")]
        [Variation("ClassTest", "ByteAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "ByteKeyFrameCollection", "None")]
        [Variation("ClassTest", "CharAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "CharKeyFrameCollection", "None")]
        [Variation("ClassTest", "ColorAnimation", "None")]
        [Variation("ClassTest", "ColorAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "ColorKeyFrameCollection", "None")]
        [Variation("ClassTest", "DecimalAnimation", "None")]
        [Variation("ClassTest", "DecimalAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "DecimalKeyFrameCollection", "None")]

        [Variation("ClassTest", "DiscreteBooleanKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteByteKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteCharKeyFrame", "None")]
        // [DISABLED WHILE PORTING]
        // [Variation("ClassTest", "DiscreteColorKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteDecimalKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteDoubleKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteInt16KeyFrame", "None")]
        [Variation("ClassTest", "DiscreteInt32KeyFrame", "None")]
        [Variation("ClassTest", "DiscreteInt64KeyFrame", "None")]
        [Variation("ClassTest", "DiscreteMatrixKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteObjectKeyFrame", "None")]
        [Variation("ClassTest", "DiscretePoint3DKeyFrame", "None")]
        [Variation("ClassTest", "DiscretePointKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteQuaternionKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteRectKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteRotation3DKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteSingleKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteSizeKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteStringKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteVector3DKeyFrame", "None")]
        [Variation("ClassTest", "DiscreteVectorKeyFrame", "None")]

        [Variation("ClassTest", "DoubleAnimation", "None")]
        [Variation("ClassTest", "DoubleAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "DoubleAnimationUsingPath", "None")]
        [Variation("ClassTest", "DoubleKeyFrameCollection", "None")]
        [Variation("ClassTest", "Int16Animation", "None")]
        [Variation("ClassTest", "Int16AnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "Int16KeyFrameCollection", "None")]
        [Variation("ClassTest", "Int32Animation", "None")]
        [Variation("ClassTest", "Int32AnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "Int32KeyFrameCollection", "None")]
        [Variation("ClassTest", "Int64Animation", "None")]
        [Variation("ClassTest", "Int64AnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "Int64KeyFrameCollection", "None")]
        [Variation("ClassTest", "KeySpline", "None")]
        [Variation("ClassTest", "KeyTime", "None")]

        [Variation("ClassTest", "LinearByteKeyFrame", "None")]
        // [DISABLED WHILE PORTING]
        // [Variation("ClassTest", "LinearColorKeyFrame", "None")]
        [Variation("ClassTest", "LinearDecimalKeyFrame", "None")]
        [Variation("ClassTest", "LinearDoubleKeyFrame", "None")]
        [Variation("ClassTest", "LinearInt16KeyFrame", "None")]
        [Variation("ClassTest", "LinearInt32KeyFrame", "None")]
        [Variation("ClassTest", "LinearInt64KeyFrame", "None")]
        [Variation("ClassTest", "LinearPoint3DKeyFrame", "None")]
        [Variation("ClassTest", "LinearPointKeyFrame", "None")]
        [Variation("ClassTest", "LinearQuaternionKeyFrame", "None")]
        [Variation("ClassTest", "LinearRectKeyFrame", "None")]
        [Variation("ClassTest", "LinearRotation3DKeyFrame", "None")]
        [Variation("ClassTest", "LinearSingleKeyFrame", "None")]
        [Variation("ClassTest", "LinearSizeKeyFrame", "None")]
        [Variation("ClassTest", "LinearVector3DKeyFrame", "None")]
        [Variation("ClassTest", "LinearVectorKeyFrame", "None")]

        [Variation("ClassTest", "MatrixAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "MatrixAnimationUsingPath", "None")]
        [Variation("ClassTest", "MatrixKeyFrameCollection", "None")]
        [Variation("ClassTest", "ObjectAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "ObjectKeyFrameCollection", "None")]
        [Variation("ClassTest", "Point3DAnimation", "None")]
        [Variation("ClassTest", "Point3DAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "Point3DKeyFrameCollection", "None")]
        [Variation("ClassTest", "QuaternionAnimation", "None")]
        [Variation("ClassTest", "QuaternionAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "QuaternionKeyFrameCollection", "None")]
        [Variation("ClassTest", "RectAnimation", "None")]
        [Variation("ClassTest", "RectAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "RectKeyFrameCollection", "None")]
        [Variation("ClassTest", "Rotation3DAnimation", "None", Disabled = true)]
        [Variation("ClassTest", "Rotation3DAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "Rotation3DKeyFrameCollection", "None")]
        [Variation("ClassTest", "SingleAnimation", "None")]
        [Variation("ClassTest", "SingleAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "SingleKeyFrameCollection", "None")]
        [Variation("ClassTest", "SizeAnimation", "None")]
        [Variation("ClassTest", "SizeAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "SizeKeyFrameCollection", "None")]

        [Variation("ClassTest", "SplineByteKeyFrame", "None")]
        // [DISABLED WHILE PORTING]
        // [Variation("ClassTest", "SplineColorKeyFrame", "None")]
        [Variation("ClassTest", "SplineDecimalKeyFrame", "None")]
        [Variation("ClassTest", "SplineDoubleKeyFrame", "None")]
        [Variation("ClassTest", "SplineInt16KeyFrame", "None")]
        [Variation("ClassTest", "SplineInt32KeyFrame", "None")]
        [Variation("ClassTest", "SplineInt64KeyFrame", "None")]
        [Variation("ClassTest", "SplinePoint3DKeyFrame", "None")]
        [Variation("ClassTest", "SplinePointKeyFrame", "None")]
        [Variation("ClassTest", "SplineQuaternionKeyFrame", "None")]
        [Variation("ClassTest", "SplineRectKeyFrame", "None")]
        [Variation("ClassTest", "SplineRotation3DKeyFrame", "None")]
        [Variation("ClassTest", "SplineSingleKeyFrame", "None")]
        [Variation("ClassTest", "SplineSizeKeyFrame", "None")]
        [Variation("ClassTest", "SplineVector3DKeyFrame", "None")]
        [Variation("ClassTest", "SplineVectorKeyFrame", "None")]

        [Variation("ClassTest", "StringAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "StringKeyFrameCollection", "None")]
        // [DISABLED WHILE PORTING]
        // [Variation("ClassTest", "ClockController", "None")]
        [Variation("ClassTest", "Vector3DAnimation", "None")]
        [Variation("ClassTest", "Vector3DAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "Vector3DKeyFrameCollection", "None")]
        [Variation("ClassTest", "VectorAnimation", "None")]
        [Variation("ClassTest", "VectorAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "VectorKeyFrameCollection", "None")]
        [Variation("ClassTest", "BeginStoryboard", "None")]
        [Variation("ClassTest", "DiscreteThicknessKeyFrame", "None")]
        [Variation("ClassTest", "LinearThicknessKeyFrame", "None")]
        [Variation("ClassTest", "PauseStoryboard", "None")]
        [Variation("ClassTest", "RemoveStoryboard", "None")]
        [Variation("ClassTest", "ResumeStoryboard", "None")]
        [Variation("ClassTest", "SeekStoryboard", "None")]
        [Variation("ClassTest", "SetStoryboardSpeedRatio", "None")]
        [Variation("ClassTest", "SkipStoryboardToFill", "None")]
        [Variation("ClassTest", "SplineThicknessKeyFrame", "None")]
        [Variation("ClassTest", "StopStoryboard", "None")]
        [Variation("ClassTest", "Storyboard", "None")]
        [Variation("ClassTest", "ThicknessAnimation", "None")]
        [Variation("ClassTest", "ThicknessAnimationUsingKeyFrames", "None")]
        [Variation("ClassTest", "ThicknessKeyFrameCollection", "None")]
        [Variation("ClassTest", "ParallelTimeline", "None")]
        [Variation("ClassTest", "TimelineCollection", "None")]

        /******************************************************************************
        * Function:          BoundaryTest Constructor
        ******************************************************************************/
        public BoundaryTest(string testType, string animClassName, string itemName)
        {
            GlobalLog.LogStatus("-------------------------------------------");
            GlobalLog.LogStatus("Parameter 1 (testType):      " + testType);
            GlobalLog.LogStatus("Parameter 2 (animClassName): " + animClassName);
            GlobalLog.LogStatus("Parameter 3 (itemName):      " + itemName);
            GlobalLog.LogStatus("-------------------------------------------");

            _className = animClassName;
            
            if (testType == "MethodTest")
            {
                _requestedMethod = itemName;
            }
            else if (testType == "PropertyTest")
            {
                _requestedProperty = itemName;
            }

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        //--------------------------------------------------------------------------------------------

        TestResult             Initialize ()
        {
            //Use known type to obtain the assembly (PresentaionCore).
            Assembly dll = typeof(ByteAnimation).Assembly;
            Type t = dll.GetType ( "System.Windows.Media.Animation." + _className );

            if (t == null)
            {
                // Could be in presentationFramework.dll
                //dll = Assembly.Load("PresentationFramework");
                dll = typeof(Storyboard).Assembly;
                t = dll.GetType("System.Windows.Media.Animation." + _className);
                if (t == null)
                {
                    GlobalLog.LogEvidence("!!!FAIL: ClassName not found: " + _className);
                    _testPassed = false;
                }
            }
            if (t.IsAbstract)
            {
                GlobalLog.LogEvidence("Cannot create an instance of an abstract class " + t.ToString());
                _testPassed = false;
            }
            GlobalLog.LogStatus("Testing Class: " + t.ToString() + "----------");

            object o = null;
            try
            {
                o = (object)Activator.CreateInstance(t);
            }
            catch (System.MissingMethodException e)
            {
                if (e.ToString().Contains("No parameterless constructor defined for type"))
                {
                    // Cannot create an instance for this object,
                    // So try another method
                    if (t.ToString().Contains("Collection"))
                    {
                        o =  PredefinedObjects.MakeCollection(t.FullName.ToString());
                    }
                    else
                    {
                        o = PredefinedObjects.MakeValue(t);
                    }

                }
            }
            if (o == null)
            {
                GlobalLog.LogEvidence("Cannot make an instance for " + t.ToString());
                _testPassed = false;
            }
            if (_requestedMethod == null && _requestedProperty == null)
            {
                _finalResult &= TestAllProperties ( t, o );
                _finalResult &= TestAllMethods(t, o);
            }
            else if (_requestedMethod != null)
            {
                _finalResult &= TestRequestedMethod(t, o, _requestedMethod);
            }
            else
            {
                _finalResult &= TestRequestedProperty( t, o, _requestedProperty );
            }
            GlobalLog.LogEvidence("**********Final result**********");

            Signal("AnimationDone", TestResult.Pass);

            return TestResult.Pass;

        }
        
        //--------------------------------------------------------------------------------------------
        
        public bool TestAllProperties ( Type t, object o )
        {
            GlobalLog.LogStatus ( "----------TestAllProperties----------" );
            try
            {
                //Specifying BindingFlags.DeclaredOnly, which means only those props directly
                //specified for the requested class will be tested, e.g., properties from
                //Timeline are skipped when testing DoubleAnimation.
                foreach ( PropertyInfo p in t.GetProperties (  BindingFlags.Public | BindingFlags.Instance ) )
                {
                    if (p.CanWrite)
                    {
                        GlobalLog.LogStatus ( "*********" + p.Name + "*********" );
                        //GlobalLog.LogStatus ( "********************" + p.PropertyType.ToString() + "*" );
                        //SET PROPERTY VALUES, THEN GET THEM AND COMPARE.
                        DataTypeFactory.GetData(p, out _goodValues, out _badValues);
                        bool propPassed = AnimationTests.TestProperty( o, p, _goodValues, _badValues);
                        if (propPassed == false)
                        {
                            _testPassed = false;  //One failure will fail the entire test.
                        }
                    }
                }
            }
            catch ( Exception e2 )
            {
                GlobalLog.LogEvidence ( "*** EXCEPTION ***: " + e2.ToString () );
                _testPassed = false;
            }
            return _testPassed;
    
        }

        //--------------------------------------------------------------------------------------------

        public bool TestAllMethods(Type t, object o)
        {
            GlobalLog.LogStatus("----------TestAllMethods----------");
            try
            {
                MethodInfo[] methodInfo = t.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                foreach (MethodInfo method in methodInfo)
                {
                    if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")
                        || method.Name.StartsWith("add_") || method.Name.StartsWith("remove_"))
                    {
                        // set/get are tested in TestAllProperties()
                        // add/remove are tested in EventTesting
                        continue;
                    }
                    ParameterInfo[] paramInfo = method.GetParameters();
                    object [] args = new object[paramInfo.Length];
                    for (int i = 0; i < paramInfo.Length; i++)
                    {
                        if (paramInfo[i].ParameterType.FullName.ToString() == "System.Collections.ICollection"
                            || paramInfo[i].ParameterType.FullName.StartsWith("System.Collections.Generic.IEnumerable"))
                        {
                            args[i] = PredefinedObjects.MakeCollection(t.FullName.ToString());
                        }
                        else
                        {
                            args[i] = PredefinedObjects.MakeValue(paramInfo[i].ParameterType);
                        }
                    }

                    if (   method.Name == "GetCurrentGlobalSpeed"
                        || method.Name == "GetCurrentIteration" 
                        || method.Name == "GetCurrentTime"
                        || method.Name == "GetCurrentState"
                        || method.Name == "GetCurrentProgress"
                        || method.Name == "GetIsPaused")
                        continue;
                    try
                    {
                        GlobalLog.LogStatus("Testing " + method.Name + " ...");
                        if (args.Length == 0)
                        {
                            method.Invoke(o, null);
                            _testPassed &= true;
     
                        }
                        else
                        {
                            // GetCurrentValueCore requires AnimationClock.CurrentProgres != null
                            // Ingnore this method due to current test limitation.
                            if (method.Name != "GetCurrentValueCore")
                            {
                                method.Invoke(o, args);
                                _testPassed &= true;
                            }
                        }
                        GlobalLog.LogEvidence(" -RESULT: " + _testPassed);
      
                    }
                    catch (Exception e)
                    {
                        GlobalLog.LogEvidence("MethodName : " + method.Name + " *** EXCEPTION ***:  " + e.ToString());
                        _testPassed &= false;
     
                    }
                }
            }
            catch (Exception e2)
            {
                GlobalLog.LogEvidence("Exception: " + e2.ToString());
                _testPassed = false;
            }
            return _testPassed;
        }

        //--------------------------------------------------------------------------------------------
        public bool TestRequestedProperty ( Type t, object o, string requestedProperty)
        {
            
            PropertyInfo p = t.GetProperty(requestedProperty);
            if (p == null)
            {
                GlobalLog.LogEvidence ( "!!!FAIL --- Property not found: " + requestedProperty );
                return false;
            }
            else if (!p.CanWrite)
            {
                GlobalLog.LogEvidence("!!!FAIL --- Property is readonly " + requestedProperty);
                return false;
            }
            else
            {
                GlobalLog.LogEvidence("--- Property found: " + p.Name);
                DataTypeFactory.GetData(p, out _goodValues, out _badValues);
                return AnimationTests.TestProperty(o, p, _goodValues, _badValues);
            }
        }

        //--------------------------------------------------------------------------------------------
        public bool TestRequestedMethod(Type t, object o, string requestedMethod)
        {
            MethodInfo methodInfo = t.GetMethod(requestedMethod, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            if (methodInfo == null)
            {
                GlobalLog.LogEvidence("!!!FAIL --- Method not found: " + requestedMethod);
                return false;
            }
            else
            {
                ParameterInfo[] paramInfo = methodInfo.GetParameters();
                object[] args = new object[paramInfo.Length];
                for (int i = 0; i < paramInfo.Length; i++)
                {
                    if (paramInfo[i].ParameterType.FullName.ToString() == "System.Collections.ICollection"
                        || paramInfo[i].ParameterType.FullName.StartsWith("System.Collections.Generic.IEnumerable"))
                    {
                        args[i] = PredefinedObjects.MakeCollection(t.FullName.ToString());
                    }
                    else
                    {
                        args[i] = PredefinedObjects.MakeValue(paramInfo[i].ParameterType);
                    }
                }
                try
                {
                    GlobalLog.LogStatus("Testing " + methodInfo.Name + " ...");
                    if (args.Length == 0)
                    {
                        methodInfo.Invoke(o, null);
                    }
                    else
                    {
                        // GetCurrentValueCore requires AnimationClock.CurrentProgres != null
                        // Ingnore this method due to current test limitation.
                        if (methodInfo.Name != "GetCurrentValueCore")
                        {
                            methodInfo.Invoke(o, args);
                        }
                    }
                    GlobalLog.LogEvidence(" -RESULT: " + true);
                    return true;
                }
                catch (Exception e)
                {
                    GlobalLog.LogEvidence("MethodName : " + methodInfo.Name + " *** EXCEPTION ***:  " + e.ToString());
                    return false;
                }
            }
 
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult Verify()
        {
            WaitForSignal("AnimationDone");

            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion
    }
}
