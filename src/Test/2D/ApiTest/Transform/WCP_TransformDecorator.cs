// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  TransformDecorator regression Tests - for Regression_Bug72

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class TransformDecoratorTest : ApiTest
    {
        public TransformDecoratorTest( double left, double top,
            double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Controls.Decorator);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            #region Initialization stage
            CommonLib.Stage = TestStage.Initialize;
            #endregion

            #region Running stage
            CommonLib.Stage = TestStage.Run;


            #region SECTION I - REGRESSION

            Decorator decorator = new Decorator();
            TransformCollection transformCollection = new TransformCollection();

            transformCollection.Add(new TranslateTransform(0.0, 0.0));
            transformCollection.Add(new RotateTransform(45));
            transformCollection.Add(new SkewTransform(0.0, 0.0));
            transformCollection.Add(new ScaleTransform(1.0, 1.0));
            transformCollection.Add(new MatrixTransform(0.0, 0.0, 0.0, 0.0, 0.0, 0.0));
            CommonLib.LogStatus("TransformCollection :" + transformCollection.ToString());

            TransformGroup group = new TransformGroup();
            group.Children = transformCollection;
            decorator.LayoutTransform = group;
            CommonLib.LogStatus("logter assigned by transformCollection, decorator.LayoutTransform.Children:" + ((TransformGroup)decorator.LayoutTransform).Children.ToString());
            _class_testresult &= _helper.CompareProp("decorator.LayoutTransform", ((TransformGroup)decorator.LayoutTransform).Children.ToString(), transformCollection.ToString());

            group = ((TransformGroup)decorator.LayoutTransform).Clone(); // make freezable clone

            #region Test1 - Modify the transformCollection[0] and reassign it back to decorator.LayoutTransform
            TranslateTransform translateTransform = (TranslateTransform)transformCollection[0];

            translateTransform.X = 0.1;
            translateTransform.Y = -0.1;
            transformCollection[0] = translateTransform;
            CommonLib.LogStatus("modified transformCollection:" + transformCollection.ToString());

            group.Children = transformCollection;
            decorator.LayoutTransform = group;
            CommonLib.LogStatus("expected decorator.LayoutTransform:" + transformCollection.ToString());
            CommonLib.LogStatus("result decorator.LayoutTransform:" + ((TransformGroup)decorator.LayoutTransform).Children.ToString());
            _class_testresult &= _helper.CompareProp("decorator.LayoutTransform", ((TransformGroup)decorator.LayoutTransform).Children.ToString(), transformCollection.ToString());

            #endregion

            #region Test2 - Modify the transformCollection[1] and reassign it to decorator.LayoutTransform
            //Regression_Bug72
            RotateTransform rotateTransform = (RotateTransform)transformCollection[1];

            rotateTransform.Angle = 180;
            transformCollection[1] = rotateTransform;
            CommonLib.LogStatus("modified transformCollection:" + transformCollection.ToString());

            group.Children = transformCollection;
            decorator.LayoutTransform = group;
            CommonLib.LogStatus("expected decorator.LayoutTransform:" + transformCollection.ToString());
            CommonLib.LogStatus("result decorator.LayoutTransform:" + ((TransformGroup)decorator.LayoutTransform).Children.ToString());
            _class_testresult &= _helper.CompareProp("decorator.LayoutTransform", ((TransformGroup)decorator.LayoutTransform).Children.ToString(), transformCollection.ToString()); //Regression_Bug72

            #endregion

            #region Test3 - Modify the transformCollection[2] and reassign it to decorator.LayoutTransform
            SkewTransform skewTransform = (SkewTransform)transformCollection[2];

            skewTransform.AngleX = 0.0;
            skewTransform.AngleY = 0.1;
            transformCollection[2] = skewTransform;
            CommonLib.LogStatus("modified transformCollection:" + transformCollection.ToString());

            group.Children = transformCollection;
            decorator.LayoutTransform = group;
            CommonLib.LogStatus("expected decorator.LayoutTransform:" + transformCollection.ToString());
            CommonLib.LogStatus("result decorator.LayoutTransform:" + ((TransformGroup)decorator.LayoutTransform).Children.ToString());
            _class_testresult &= _helper.CompareProp("decorator.LayoutTransform", ((TransformGroup)decorator.LayoutTransform).Children.ToString(), transformCollection.ToString());

            #endregion

            #region Test4 - Modify the transformCollection[3] and reassign it to decorator.LayoutTransform
            ScaleTransform scaleTransform = (ScaleTransform)transformCollection[3];

            scaleTransform.ScaleX = 0.1;
            scaleTransform.ScaleY = -0.1;
            transformCollection[3] = scaleTransform;
            CommonLib.LogStatus("modified transformCollection:" + transformCollection.ToString());

            group.Children = transformCollection;
            decorator.LayoutTransform = group;
            CommonLib.LogStatus("expected decorator.LayoutTransform:" + transformCollection.ToString());
            CommonLib.LogStatus("result decorator.LayoutTransform:" + ((TransformGroup)decorator.LayoutTransform).Children.ToString());
            _class_testresult &= _helper.CompareProp("decorator.LayoutTransform", ((TransformGroup)decorator.LayoutTransform).Children.ToString(), transformCollection.ToString());

            #endregion

            #region Test5 - Modify the transformCollection[4] and reassign it to decorator.LayoutTransform
            MatrixTransform matrixTransform = (MatrixTransform)transformCollection[4];

            matrixTransform.Matrix = new Matrix(-0.1, -0.1, -0.1, 0.1, 0.1, 0.1);

            transformCollection[4] = matrixTransform;
            CommonLib.LogStatus("modified transformCollection:" + transformCollection.ToString());

            group.Children = transformCollection;
            decorator.LayoutTransform = group;
            CommonLib.LogStatus("expected decorator.LayoutTransform:" + transformCollection.ToString());
            CommonLib.LogStatus("result decorator.LayoutTransform:" + ((TransformGroup)decorator.LayoutTransform).Children.ToString());
            _class_testresult &= _helper.CompareProp("decorator.LayoutTransform", ((TransformGroup)decorator.LayoutTransform).Children.ToString(), transformCollection.ToString());

            #endregion

            #endregion
            #endregion // Running stage

            CommonLib.LogTest("Result for :" + _class_testresult);

        }
        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}

