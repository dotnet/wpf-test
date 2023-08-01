// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test freezable object
 
 *
 ************************************************************/

using System;
using System.Reflection;
using System.Collections.Specialized;
using System.Windows;
using System.Xml;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.ElementServices.Freezables.Objects;


namespace Microsoft.Test.ElementServices.Freezables
{
    /// <summary>
    /// <area>ElementServices\Freezables\BVT</area>
 
    /// <priority>0</priority>
    /// <description>
    /// BVT tests for Freezables
    /// </description>
    /// </summary>

    [Test(0, "Freezables.BVT", "FreezablesBVT", SupportFiles=@"FeatureTests\ElementServices\FreezablesBVT.xtc")]

    /**********************************************************************************
    * CLASS:          FreezablesBVT
    **********************************************************************************/
    public class FreezablesBVT : AvalonTest
    {
        #region Test case members

        private bool                _passed;
        private StringCollection    _failures;
        private string              _testName        = "";
        private string              _xtcFileName     = "";

        #endregion


        #region Constructor

        [Variation("TestCopy", "FreezablesBVT.xtc", Keywords = "MicroSuite")]
        [Variation("TestIsFreezable", "FreezablesBVT.xtc")]
        [Variation("TestMakeUnfreezable", "FreezablesBVT.xtc", Keywords = "MicroSuite")]


        /******************************************************************************
        * Function:          FreezablesBVT Constructor
        ******************************************************************************/
        // Input Parameters:
        //      FreezablesBVT.exe assembly TestName ScriptName
        // (note: don't specify .dll in assembly)
        // e.g.,: FreezablesBVT.exe PresentationCore TestCopy FreezablesBVT.xtc
        public FreezablesBVT(string inputValue0, string inputValue1)
        {
            _testName = inputValue0;
            _xtcFileName = inputValue1;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTestCase);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Private Members

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Sets global variables.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult Initialize()
        {
            _failures = new StringCollection();
            _passed = true;

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTestCase
        ******************************************************************************/
        /// <summary>
        /// Carries out a series of basic Freezables tests.  A global variable tracks pass/fail.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult StartTestCase()
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(_xtcFileName);
            }
            catch (Exception)
            {
                throw new ApplicationException("Unable to load script file:" + _xtcFileName);
            }

            XmlElement changeableTest = (XmlElement)doc["FreezablesTest"];
            if (changeableTest == null)
            {
                throw new ApplicationException("The script needs FreezablesTest element");
            }
            for (XmlNode classNode = changeableTest["ClassName"]; classNode != null; classNode = classNode.NextSibling)
            {
                Assembly dll = typeof(UIElement).Assembly;
                Type t = dll.GetType(classNode.InnerText);

                if (t == null)
                {
                      _passed &= false;
                      _failures.Add(classNode.InnerText + ": invalid class in FreezablesBVT");
                      GlobalLog.LogEvidence("FAIL:" + classNode.InnerText + "invalid class in Freezable.xtc");
                  }
                else
                {
                    Test(t, _testName);
                }
            }

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          Test
        ******************************************************************************/
        /// <summary>
        /// Invoke the specific test case requested.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <param name="TestName">Name of the test to be executed.</param>
        /// <returns></returns>
        private void Test(Type t, string TestName)
        {
            if (TypeHelper.IsMediaTimeline (t))
            { // skip media data objects testing for now
                // 
                return;
            }
            if (TypeHelper.IsMedia3D(t))
            { // skip Media3D objects testing for now
                // 
                return;
            }

            GlobalLog.LogEvidence(t.ToString ());

            switch (TestName)
            {
                case "TestCopy":
                    TestCopy(t);
                    break;

                case "TestIsFreezable":
                    TestIsFreezable(t);
                    break;

                case "TestMakeUnfreezable":
                    TestMakeUnfreezable(t);
                    break;

                default:
                    throw new ApplicationException("Unknown TestName: " + TestName);
            }
        }

        /******************************************************************************
        * Function:          CreateNewFreezable
        ******************************************************************************/
        /// <summary>
        /// Obtain a Freezable.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <returns>an instance of the specified type</returns>
        private System.Windows.Freezable CreateNewFreezable(Type t)
        {
            if (t.IsAbstract)
            {
                return null;
            }

            System.Windows.Freezable retval = null;
            ConstructorInfo[] ci = t.GetConstructors();
        
            // for each constructor
            for (int i = 0; i < ci.Length; i++)
            {
                try
                {
                    ParameterInfo[] pi = ci[i].GetParameters();
                    Object[] objects = new Object[pi.Length]; // the constructor arguments

                    // for each parameter
                    for (int j = 0; j < pi.Length; j++)
                    {
                        if (j == pi[j].Position) // verify the position of the arguments
                        {
                            if (pi[j].ParameterType.FullName.ToString () == "System.Collections.ICollection"
                                || pi[j].ParameterType.FullName.StartsWith("System.Collections.Generic.IEnumerable"))
                            {
                                objects[j] = PredefinedObjects.MakeCollection (t.FullName.ToString ());
                                if (objects[j] == null)
                                {
                                    return null;
                                }

                            }
                            else
                            {
                                objects[j] = PredefinedObjects.MakeValue (pi[j].ParameterType);
                            }
                        }
                    }

                    MakeObjectIfSpecialCase(t, pi, objects);
                    System.Windows.Freezable obj = (System.Windows.Freezable)t.InvokeMember (null, BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, objects);

                    if (obj != null)
                    {
                        retval = obj;
                    }
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    if (!e.InnerException.ToString ().StartsWith ("System.ArgumentNullException"))
                    {
                        GlobalLog.LogEvidence("        !!! NewObject: Exception: {0}", e.InnerException.ToString ());
                    }

                    return null;
                }
            }
            return retval;
        }


        /******************************************************************************
        * Function:          MakeObjectIfSpecialCase
        ******************************************************************************/
        /// <summary>
        /// Create a Freezable object for special-case types.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <param name="pi">Parameter values.</param>
        /// <param name="objects">Array of objects.</param>
        /// <returns></returns>
        private void MakeObjectIfSpecialCase(Type t, ParameterInfo[] pi, Object[] objects)
        {
            if (t.ToString() == "System.Windows.Media.Imaging.BitmapImage" && pi.Length == 10)
            {
                // The 8th parameter (stride) of BitmapImage constructor needs value stride = Width * BytePerPixel
                // Ctor forImageData:
                // (Int32 pixelWidth, Int32 pixelHeight, Double dpiX, Double dpiY, PixelFormat pixelFormat, BitmapPalette imagePalette, Byte[] pixels, Int32 stride, Int32Rect sourceRect, BitmapSizeOptions sizeOptions)
                System.Windows.Media.PixelFormat p = (System.Windows.Media.PixelFormat)objects[4];
                objects[6] = new byte[32];
                objects[7] = (Int32)objects[0] * p.BitsPerPixel / 8;
            }
            else if (t.ToString() == "System.Windows.Media.Animation.KeySpline" && pi.Length == 2 && pi[0].ParameterType.FullName.ToString() == "System.Windows.Point" && pi[1].ParameterType.FullName.ToString() == "System.Windows.Point")
            {
                // For Keyspline, points must be betwen 0.0 and 1.0
                objects[0] = new Point(0.1, 0.2);
                objects[1] = new Point(0.2, 0.7);

            }
            else if (t.ToString() == "System.Windows.Media.Animation.Setter" && pi.Length == 3)
            {
                // For animation Setter, generic objects from PredefinesObject will not work.
                objects[0] = new System.Windows.Media.LineGeometry(new Point(10, 10), new Point(20, 20)); //DependencyObject();
                objects[1] = System.Windows.Media.LineGeometry.StartPointProperty;
                objects[2] = new System.Windows.Point(1.0, 2.0);
            }
        }


        /******************************************************************************
        * Function:          TestIsFreezable
        ******************************************************************************/
        /// <summary>
        /// Verify a Freezable object's IsFrozen property.
        /// </summary>
        /// <param name="t">The Type of the Freezable object tested.</param>
        /// <returns></returns>
        private void TestIsFreezable(Type t)
        {
            Freezable obj = CreateNewFreezable(t);

            if (obj != null)
            {
                // Test !IsFrozen property
                if (obj.IsFrozen)
                {
                    _passed &= false;
                    _failures.Add(t.ToString() + ": !IsFrozen is false");
                    GlobalLog.LogEvidence("FAIL:" + t.ToString() + " !IsFrozen is false, expected true");
                }
            }
        }


        /******************************************************************************
        * Function:          TestMakeUnfreezable
        ******************************************************************************/
        /// <summary>
        /// Verify a Freezable object's IsFrozen property, after freezing it.
        /// </summary>
        /// <param name="t">The Type of the Freezable object tested.</param>
        /// <returns></returns>
        private void TestMakeUnfreezable(Type t)
        {
            // create an instance of this type
            Freezable obj = CreateNewFreezable(t);

            if (obj != null && obj.CanFreeze)
            {
                obj.Freeze();
                
                // Verify that !IsFrozen is false
                if (!obj.IsFrozen)
                {
                    _passed &= false;
                    _failures.Add(t.ToString() + ": !IsFrozen is true, expected false");
                    GlobalLog.LogEvidence("FAIL:" + t.ToString() + " !IsFrozen true, expected false");
                }
            }
        }


        /******************************************************************************
        * Function:          TestCopy
        ******************************************************************************/
        /// <summary>
        /// Verify a Freezable object's IsFrozen property, after cloning it.
        /// </summary>
        /// <param name="t">The Type of the Freezable object tested.</param>
        /// <returns></returns>
        private void TestCopy(System.Type t)
        {
            // 
            if (t.ToString() == "System.Windows.Media.Imaging.BitmapSource+BitmapSourceNull")
            {
                // 
                return;
            }

            // create an instance of this type
            Freezable obj = CreateNewFreezable(t);

            if (obj!= null)
            {
                Freezable objCopy = obj.Clone() ;
                if (objCopy.IsFrozen)
                {
                    _passed &= false;
                    _failures.Add(t.ToString() + ": !IsFrozen is true, expeced false");
                    GlobalLog.LogEvidence("FAIL:" + t.ToString() + " !IsFrozen true, expected false");
                }
                // Skip Animation classes since
                if (TypeHelper.IsAnimatable(t) || TypeHelper.IsAnimationCollection(t))
                {
                    return;
                }
            }
        }


        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Returns a Pass/Fail result for the test case.
        /// </summary>
        /// <returns>A TestResult, indicating whether the test passes or fails.</returns>
        private TestResult Verify()
        {
            // report the failures all together
            if (!_passed)
            {
                GlobalLog.LogEvidence("-------------------------------------------------");
                GlobalLog.LogEvidence("FAILURE  REPORT");
                for (int i = 0; i < _failures.Count; i++)
                {
                    GlobalLog.LogEvidence (_failures[i]);
                }
            }

            if (_passed)
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
