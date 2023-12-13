// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing System.Windows.Media.GuidelineSet class
//  Author:   Microsoft
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{ 
    
    /// <summary>
    /// Summary description for System.Windows.Media.GuidelineSet.
    /// </summary>
    internal class GuidelineSet : ApiTest
    {
        public GuidelineSet( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("GuidelineSet Class");
            string objectType = "System.Windows.Media.GuidelineSet";

            #region Section I: Constructors
            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default constructor");
            System.Windows.Media.GuidelineSet glc1 = new System.Windows.Media.GuidelineSet();
            CommonLib.TypeVerifier(glc1, objectType);
            #endregion

            //#region Test #2: System.Windows.Media.GuidelineSet(GuidelinesX, GuidelinesY, Boolean)
            //CommonLib.LogStatus("Test #2: System.Windows.Media.GuidelineSet(GuidelinesX, GuidelinesY, Boolean)");
            //GuidelineSet glc2 = new System.Windows.Media.GuidelineSet(new Double[] { }, new Double[] { }, false);
            //CommonLib.TypeVerifier(glc2, objectType);
            //#endregion
            #endregion

            #region Section II: public methods
            #region Test #3: Copy() method
            CommonLib.LogStatus("Test #3: Copy() method");
            System.Windows.Media.GuidelineSet glc3 = new System.Windows.Media.GuidelineSet();
            glc3.GuidelinesX = new DoubleCollection(new Double[] { 32, 2 });
            glc3.GuidelinesY = null;
            System.Windows.Media.GuidelineSet glc3_copy = glc3.Clone();
            if (CommonLib.DeepEqual(glc3_copy, glc3))
            {
                CommonLib.LogStatus("Copy() passed!");
            }
            else
            {
                CommonLib.LogFail("Some properties in these two objects don't match");
            }
            #endregion

            #region Test #4: CloneCurrentValue() method
            CommonLib.LogStatus("Test #4: CloneCurrentValue() method");
            System.Windows.Media.GuidelineSet glc4 = new System.Windows.Media.GuidelineSet();
            glc4.GuidelinesX = new DoubleCollection(new Double[] { 1 });
            glc4.GuidelinesY = new DoubleCollection(new Double[] { 23.3, 232 });
            System.Windows.Media.GuidelineSet glc4_Current = glc4.CloneCurrentValue() as System.Windows.Media.GuidelineSet;
            if (glc4_Current != null && CommonLib.DeepEqual(glc4_Current, glc4))
            {
                CommonLib.LogStatus("CloneCurrentValue() passed!");
            }
            else
            {
                CommonLib.LogFail("CloneCurrentValue() failed");
            }
            #endregion
            #endregion

            #region Section III: public properties
            #region Test #8: GuidelinesX property
            CommonLib.LogStatus("Test #8: GuidelinesX property");
            System.Windows.Media.GuidelineSet glc8 = new System.Windows.Media.GuidelineSet();
            glc8.GuidelinesX = new DoubleCollection(new Double[] { 32 });
            CommonLib.GenericVerifier(CommonLib.DeepEqual(glc8.GuidelinesX, new DoubleCollection(new Double[] { 32 })), "GuidelinesX property");
            #endregion

            #region Test #8a: GuidelinesX property with basevalue
            CommonLib.LogStatus("Test #8a: GuidelinesX property with basevalue");
            System.Windows.Media.GuidelineSet glc8a = new System.Windows.Media.GuidelineSet();
            CommonLib.GenericVerifier(CommonLib.DeepEqual(glc8a.GuidelinesX, new DoubleCollection()), "GuidelinesX property with basevalue");
            #endregion

            #region Test #8b: GuidelinesX property in Invalid state
            CommonLib.LogStatus("Test #8b: GuidelinesX property in Invalid state");
            System.Windows.Media.GuidelineSet glc8b = new System.Windows.Media.GuidelineSet();
            glc8b.GuidelinesX = new DoubleCollection(new Double[] { 90, 23 });
            glc8b.InvalidateProperty(System.Windows.Media.GuidelineSet.GuidelinesXProperty);
            CommonLib.GenericVerifier(CommonLib.DeepEqual(glc8b.GuidelinesX, new DoubleCollection(new Double[] { 90, 23 })), "GuidelinesX property in Invalid state");
            #endregion

            #region Test #9: GuidelinesY property
            CommonLib.LogStatus("Test #9: GuidelinesY property");
            System.Windows.Media.GuidelineSet glc9 = new System.Windows.Media.GuidelineSet();
            glc9.GuidelinesX = new DoubleCollection(new Double[] { 323 });
            glc9.GuidelinesY = new DoubleCollection(new Double[] { 323 });
            CommonLib.GenericVerifier(CommonLib.DeepEqual(glc9.GuidelinesY, new DoubleCollection(new Double[] { 323 })), "GuidelinesY property");
            #endregion

            #region Test #9a: GuidelinesY property with basevalue
            CommonLib.LogStatus("Test #9a: GuidelinesX property with basevalue");
            System.Windows.Media.GuidelineSet glc9a = new System.Windows.Media.GuidelineSet();
            CommonLib.GenericVerifier(CommonLib.DeepEqual(glc9a.GuidelinesY, new DoubleCollection()), "GuidelinesY property with basevalue");
            #endregion

            #region Test #9b: GuidelinesY property in Invalid state
            CommonLib.LogStatus("Test #9b: GuidelinesY property in Invalid state");
            System.Windows.Media.GuidelineSet glc9b = new System.Windows.Media.GuidelineSet();
            glc9b.GuidelinesY = new DoubleCollection(new Double[] { -23 });
            glc9b.InvalidateProperty(System.Windows.Media.GuidelineSet.GuidelinesXProperty);
            CommonLib.GenericVerifier(CommonLib.DeepEqual(glc9b.GuidelinesY, new DoubleCollection(new Double[] { -23 })), "GuidelinesY property in Invalid state");
            #endregion
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;
            CommonLib.LogTest("Results for type: " + objectType);
            #endregion
        }
    }
}