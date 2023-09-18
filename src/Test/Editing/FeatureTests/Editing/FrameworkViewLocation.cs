// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for the FrameworkViewLocation class. 

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;

    using System.ComponentModel.Design;
    using Drawing = System.Drawing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

//    /// <summary>
//    /// Verifies that end-of-line information stored in
//    /// the FrameworkViewLocation object is stored correctly.
//    /// </summary>
//    /// <remarks>
//    /// Because FrameworkViewLocation is internal, verification
//    /// is made by calling APIs that rely on the type working
//    /// as expected.
//    /// </remarks>
//    [TestOwner("Microsoft"), TestTactics("276")]
//    public class FrameworkViewLocationEndOfLine: CustomTestCase
//    {
//        #region Private data.
//
//        private Text text;
//        private UIElementWrapper _elementWrapper;
//
//        #endregion Private data.
//
//        #region Main flow.
//
//        /// <summary>Runs the test case.</summary>
//        public override void RunTestCase()
//        {
//            Log("Creating text element...");
//            text = new Text();
//            text.Text = "content";            
//            this._elementWrapper = new UIElementWrapper(text);
//            MainWindow.Content = text;
//            QueueDelegate(TestView);
//        }
//
//        private void TestView()
//        {
//            const int LargeX = 800;     // large enough to go beyond content
//            const int SmallX = 8;       // small enough to lie within content
//            const int SmallY = SmallX;  // small enough to lie within content
//
//            Rect rect;              // Rectangle with results.
//            Point pointWithin;      // A point within the text.
//            Point pointWithout;     // A point outside the text.
//            TextView textView;      // TextView being queried.
//
//            // A position with orientation information
//            TextPointer orientedPosition;
//
//            textView = this._elementWrapper.GetTextView();
//
//            Log("Testing the EndOfLine field for false...");
//            pointWithin = new Point(SmallX, SmallY);            
//            orientedPosition = textView.GetTextPointerFromPoint(pointWithin, false);
//            rect = textView.GetRectangleFromTextPointer(orientedPosition);
//            Log("Rectangle: " + rect.ToString());
//            Verifier.Verify(rect.Height > 0, "Height greater than zero");
//
//            Log("Testing the EndOfLine field for true...");
//            pointWithout = new Point(LargeX, SmallY);
//            orientedPosition = textView.GetTextPointerFromPoint( pointWithout, true);
//            rect = textView.GetRectangleFromTextPointer(orientedPosition);
//            Log("Rectangle: " + rect.ToString());
//            Verifier.Verify(rect.Width == 0, "Width equals zero.");
//            Verifier.Verify(rect.Height > 0, "Height less than zero.");
//
//            Logger.Current.ReportSuccess();
//        }
//
//        #endregion Main flow.
//    }

//    /// <summary>
//    /// Verifies that x-offset information stored in
//    /// the FrameworkViewLocation object is stored correctly.
//    /// </summary>
//    /// <remarks>
//    /// Because FrameworkViewLocation is internal, verification
//    /// is made by calling APIs that rely on the type working
//    /// as expected.
//    /// </remarks>
//    [TestOwner("Microsoft"), TestTactics("277")]
//    public class FrameworkViewLocationXOffset: CustomTestCase
//    {
//        #region Private data.
//
//        private Text text;
//        private UIElementWrapper _elementWrapper;
//
//        #endregion Private data.
//
//        #region Main flow.
//
//        /// <summary>Runs the test case.</summary>
//        public override void RunTestCase()
//        {
//            // Test the LineNavigationXOffset field.
//            Log("Creating text element...");
//
//            text = new Text();
//            this._elementWrapper = new UIElementWrapper(text);
//            text.Text = "Line One\nLine Two\nLine Three";            
//            MainWindow.Content = text;
//            QueueDelegate(TestView);
//        }
//
//        private void TestView()
//        {
//            TextView textView;
//            OrientedTextPointer orientedPosition;
//            OrientedTextPointer movedPosition;
//
//            Log("Testing the EndOfLine field for false...");
//            textView = this._elementWrapper.GetTextView();
//
//            orientedPosition = textView.GetTextPointerFromPoint(new Point(20, 0), true);
//            movedPosition = textView.GetPositionAtNextLine(orientedPosition, 0.0f, 1);
//            Verifier.Verify(!movedPosition.Equals(orientedPosition), "One line moved", true);
//
//            orientedPosition = movedPosition;
//            movedPosition = textView.GetPositionAtNextLine(orientedPosition, 0.0f, 1);
//            Verifier.Verify(!movedPosition.Equals(orientedPosition), "One line moved", true);
//
//            Logger.Current.ReportSuccess();
//        }
//
//        #endregion Main flow.
//    }
}
