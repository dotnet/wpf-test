// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/********************************************************************
*
*this file contains code for teseting LollipopAdorner functionalitis.
*
*Note: the LollipopAdorner will be cut. Don't waste time on this
*
*********************************************************************/

namespace Test.Uis.TextEditing
{
    #region Namespaces.
    
    using System;
    using System.ComponentModel;
    
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Documents;
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Windows.Markup;
    using Test.Uis.Utils;
    using Test.Uis.Loggers;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Test.Uis.Management;
    
    #endregion Namespaces.
    
    /// <summary>
    /// Verifies Lollipop adorner functionality.
    /// </summary>
    [Test(0, "Adorner", "LollipopAdornerTest", MethodParameters = "/TestCaseType=LollipopAdornerTest /Case=RunAllCases")]
    [TestOwner("Microsoft"), TestBugs("277, 276"), TestTactics("24"), TestLastUpdatedOn("Jan 25, 2007")]
    public class LollipopAdornerTest : CommonTestCase
    {
        private Pen _pen;

        private Brush _brush;

        private Size anchorSize;

        private int _stem = 5,_diameter = 10,_elementWidth = 60,_elementHeight = 60,_anchorPositionX = 30,_anchorPositionY = 30,AnchorSize = 10,_elementCount = 8;

        private Adorner[] _adorners;

        private FrameworkElement[] _testElements;

        private int[] _mouseEventCount; 
        private Point _centerPoint;

        private AdornerLayer _adornerLayer = null;
        private Canvas _designRoot;
        string _controlType;

        /// <summary>Initializes the instance.</summary>
        public override void Init()
        {
            MyLogger.Log("Enter function - LollipopAdornerTest.init().");
            MainWindow.Content = new AdornerDecorator();
            _designRoot = new Canvas();
            ((AdornerDecorator)MainWindow.Content).Child = _designRoot;

            int i = 0;
            string FlowDir = ConfigurationSettings.Current.GetArgument("FlowDir");

            if (FlowDir != null && FlowDir != string.Empty && FlowDir.ToLower() == "rlTb")
                _designRoot.FlowDirection = FlowDirection.RightToLeft;

            if (_controlType == null || _controlType == string.Empty)
                _controlType = "Button";

            _testElements = new FrameworkElement[_elementCount];
            _mouseEventCount = new int[_elementCount];
            for (int j = 0; j < _elementCount; j++)
                _mouseEventCount[j] = 0;
            for (LollipopPosition position = LollipopPosition.TopLeft; position <= LollipopPosition.BottomRight; position++, i++)
            {
                _testElements[i] = CreateElementWithSize(_controlType, i * 80 + 50, i * 60 + 50, _elementWidth, _elementHeight);
                _designRoot.Children.Add(_testElements[i]);
            }

            MyLogger.Log("Enter function - LollipopAdornerTest.init().");
        }

        /// <summary>LollipopAdorner_FormTestElement.</summary>
        [TestCase(LocalCaseStatus.Ready, "Test LollipopAdorner")]
        public void LollipopAdorner()
        {
            _pen = new Pen(Brushes.Black, 1.0f);
            _brush = Brushes.Blue;
            _centerPoint = new Point(_anchorPositionX, _anchorPositionY);
            anchorSize = new Size(AnchorSize, AnchorSize);
            _adorners = new Adorner[_elementCount];
            _adornerLayer = AdornerLayer.GetAdornerLayer(_designRoot);

            int i = 0;

            for (LollipopPosition position = LollipopPosition.TopLeft; position <= LollipopPosition.BottomRight; position++)
            {
                //use different constructor
                if (position == LollipopPosition.TopLeft)
                {
                    _adorners[i] = new RotationAdorners(_testElements[i], position);
                    ((RotationAdorners)_adorners[i]).CenterPoint = _centerPoint;
                    ((RotationAdorners)_adorners[i]).AnchorSize = anchorSize;
                    ((RotationAdorners)_adorners[i]).AnchorPen = _pen;
                    ((RotationAdorners)_adorners[i]).AnchorBrush = _brush;
                    ((RotationAdorners)_adorners[i]).LollipopStemLength = _stem;
                    ((RotationAdorners)_adorners[i]).LollipopHeadDiameter = _diameter;
                    ((RotationAdorners)_adorners[i]).LollipopPen = _pen;
                    ((RotationAdorners)_adorners[i]).LollipopBrush = _brush;

                }
                else if (position == LollipopPosition.BottomRight)
                {
                    _adorners[i] = new RotationAdorners(_testElements[i], position, _stem, _diameter, _pen, _brush);
                    ((RotationAdorners)_adorners[i]).CenterPoint = _centerPoint;
                    ((RotationAdorners)_adorners[i]).AnchorSize = anchorSize;
                    ((RotationAdorners)_adorners[i]).AnchorPen = _pen;
                    ((RotationAdorners)_adorners[i]).AnchorBrush = _brush;
                }
                else
                {
                    _adorners[i] = new RotationAdorners(_testElements[i], position, _centerPoint, anchorSize, _pen, _brush, _stem, _diameter, _pen, _brush);
                }
                _adorners[i].MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnMouseDown);
                _adornerLayer.Add(_adorners[i++]);
            }

            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CheckResult));
        }
        void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Adorner ad = sender as Adorner;
            for (int i = 0; i < _elementCount; i++)
            {
                if (_testElements[i] == ad.AdornedElement)
                {
                    _mouseEventCount[i] = _mouseEventCount[i] + 1;
                }
            }
        }
        void ClearAdorners()
        {
            if (_adornerLayer == null)
                _adornerLayer = AdornerLayer.GetAdornerLayer(_designRoot);
            for (int i = 0; i < _elementCount; i++)
            {
                Adorner[] tAds = _adornerLayer.GetAdorners(_testElements[i]);
                if (tAds == null)
                    return;
                for (int t = 0; t < tAds.Length; t++)
                {
                    _adornerLayer.Remove(tAds[t]);
                }
            }
        }


        /// <summary>LollipopAdorner_FormTestElement.</summary>
        public void CheckResult()
        {
            EnterFunction("CheckResult");

            string str = null;

            for (int i = 0; i < 8; i++)
            {
                str = ((RotationAdorners)_adorners[i]).verifyAdorners(true, _adornerLayer) + ((RotationAdorners)_adorners[i]).VerifyProperties(_testElements[i], (LollipopPosition)i, _centerPoint, anchorSize, _pen, _brush, _stem, _diameter, _pen, _brush);
                if (!(null == str || String.Empty == str))
                {
                    MyLogger.Log("Failed - " + str);
                    QueueDelegate(EndTest);
                    return;
                }
            }
            QueueDelegate(EndTest);
            EndFunction();
        }

        void Regression_Bug276_CheckMouseEvent()
        {
            for (int i = 0; i < _elementCount; i++)
            {
                if (_mouseEventCount[i] != 2)
                {
                    MyLogger.Log(CurrentFunction + " - Failed: Expected event fires[2]. Actual fires[" + _mouseEventCount[i].ToString() + "] for TestEelement[" + i.ToString() + "]");
                    pass = false;
                }   
            }
            QueueDelegate(EndTest);
        }
        
        #region Regresion test - Regression_Bug277
        /// <summary>
        /// regression test for Regression_Bug277
        /// </summary>
        [TestCase(LocalCaseStatus.Broken, "Regression Test Regression_Bug277")]
        public void Regression_Bug277()
        {
            MyLogger.Log("Enter Function - AdornerDrawn");
            ClearAdorners();

            Size anchorSize  = new Size(3,3);
            _pen = new Pen(Brushes.Black, 1.0f);
            _brush = Brushes.Blue;
            _centerPoint = new Point(30, 30);

            Adorner adorner;

            _adornerLayer = AdornerLayer.GetAdornerLayer(_designRoot);
            adorner = new RotationAdorners(_testElements[0], LollipopPosition.RightCenter, _centerPoint, anchorSize, _pen, _brush, _stem, _diameter, _pen, _brush);
            _adornerLayer.Add(adorner);
            MyLogger.Log("Enter Function - AdornerDrawn");
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Regression_Bug277_AdornerDrawn));
        }

        void Regression_Bug277_AdornerDrawn()
        {
            EnterFunction("Regression_Bug277_AdornerDrawn");
            MouseInput.MouseMove( AdornerUtils.GetLollipopAnchorLocation(AdornerUtils.AdornerRelatives.RelativeToScreen, _testElements[0]));
            Point p1 = AdornerUtils.GetLollipopAnchorLocation(AdornerUtils.AdornerRelatives.RelativeToClientArea, _testElements[0]);
            //The center of the anchor is (80, 80), the size is of the anchor is 3, 3), if I use (70, 70), I should get null for the Hittestresult.
            AdornerHitTestResult a1 = _adornerLayer.AdornerHitTest(new Point(70, 70));
            if (null != a1)
            {
                MyLogger.Log(CurrentFunction + " - failed: AdornerHitTestResult should be null at point (70, 70) relative to client area!!!");
                pass = false;
            }

            AdornerHitTestResult a2 = _adornerLayer.AdornerHitTest(p1);

            if (null == a2)
            {
                MyLogger.Log(CurrentFunction + " - Failed: AdornerHitTestResult should be not null at p1 (80, 80) relative to client area!!!");
                pass = false;
            }

            //change the size;
            Adorner[] ads = _adornerLayer.GetAdorners(_testElements[0]);
            RotationAdorners ra = ads[0] as RotationAdorners;
            
            if(ra == null)
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: Can't get the adorner back!!!");
            }
            ra.AnchorSize = new Size(30, 30);
            System.Threading.Thread.Sleep(1500);
            if (pass)
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Regression_Bug277_done));

            }
            else
            {
                QueueDelegate(EndTest);
            }
            EndFunction();
        }

        void Regression_Bug277_done()
        {
            EnterFunction("Regression_Bug277_done");
            Sleep();

            AdornerHitTestResult a1 = _adornerLayer.AdornerHitTest(new Point(70, 70));
            //after I re-specifed a anchor size. Hit test at point (70, 70) will return non-null value;
            if (null == a1)
            {
                pass = false;
                MyLogger.Log(CurrentFunction + "- Failed: AdornerHitTestResult should not be null at point (70, 70) after achor size is enlarged!!!");
            }
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion 
    }

    /// <summary>A customlollipop adorner for testing purposes.</summary>
    public class RotationAdorners : LollipopAdorner
    {
        /// <summary>Creates a new RotationAdorners instance.</summary>
        public RotationAdorners(UIElement element, LollipopPosition lollipopPosition) : base(element,lollipopPosition )
        {
        }

        /// <summary>Creates a new RotationAdorners instance.</summary>
        public RotationAdorners(UIElement element, LollipopPosition lollipopPosition, int lollipopStemLength, int lollipopHeadDiameter, Pen lollipopPen, Brush lollipopBrush) : base(element, lollipopPosition, lollipopStemLength, lollipopHeadDiameter, lollipopPen,  lollipopBrush)
        {
        }

        /// <summary>Creates a new RotationAdorners instance.</summary>
        public RotationAdorners(UIElement element, LollipopPosition lollipopPosition, Point centerPoint, Size anchorSize, Pen anchorPen, Brush anchorBrush, int lollipopStemLength, int lollipopHeadDiameter, Pen lollipopPen, Brush lollipopBrush) : base(element, lollipopPosition,centerPoint , anchorSize,anchorPen ,anchorBrush ,lollipopStemLength, lollipopHeadDiameter , lollipopPen,lollipopBrush )
        {
        }

        /// <summary>verifyAdorners.</summary>
        public string verifyAdorners(bool drawn, AdornerLayer adornerLayer)
        {
            Point point;

            //no we expect no overlaps
            for (LollipopPosition position = LollipopPosition.TopLeft; position <= LollipopPosition.BottomRight; position++)
            {
                //the mouse move used here is to help the visual verification and debug purpose
               // Test.Uis.Utils.MouseInput.MouseClick(AdornerUtils.GetLollipopHandleLocation(AdornerUtils.AdornerRelatives.RelativeToScreen, base.AdornedElement, position));
                //System.Threading.Thread.Sleep(100);

                //the point should be relative to the adornerLayer. since the adorner Layer is at the same location of the clientArea, we use AdornerRelatives.RelativeToClientArea to find the adorner location.
                point = AdornerUtils.GetLollipopHandleLocation(AdornerUtils.AdornerRelatives.RelativeToClientArea, base.AdornedElement, position);

                AdornerHitTestResult adResult = adornerLayer.AdornerHitTest(point);

                if (position == base.LollipopPosition)
                {
                    //note: we assume that no adorner overlap.
                    if (adResult == null )
                    {
                        System.Threading.Thread.Sleep(3000);
                        return "Can't find a adorner at mouse pointed location!!!";
                    }
                }
                else
                {
                    //note: we assume that no adorner overlap.
                    if (adResult != null)
                    {
                        System.Threading.Thread.Sleep(3000);
                        return "Should not find an adorner at mouse pointed location!!!";
                    }
                }
            }

            //check the anchor
            //Test.Uis.Utils.MouseInput.MouseClick(AdornerUtils.GetLollipopAnchorLocation(AdornerUtils.AdornerRelatives.RelativeToScreen, base.AdornedElement));
            //System.Threading.Thread.Sleep(100);

            //the point should be relative to the adornerLayer. since the adorner Layer is at the same location of the clientArea, we use AdornerRelatives.RelativeToClientArea to find the adorner location.
            point = AdornerUtils.GetLollipopAnchorLocation(AdornerUtils.AdornerRelatives.RelativeToClientArea, base.AdornedElement);

            AdornerHitTestResult anchor = adornerLayer.AdornerHitTest(point);

            if (!(anchor != null && drawn))
            {
                return "Can't find the anchor at moust pointed position!!!";
            }

            if (anchor != null && !drawn)
            {
                return "We don't expect the anchor at mouse pointed position!!!";
            }

            return null;
        }

        /// <summary>VerifyProperties.</summary>
        public string VerifyProperties(UIElement element, LollipopPosition lollipopPosition, Point centerPoint, Size anchorSize, Pen anchorPen, Brush anchorBrush, int lollipopStemLength, int lollipopHeadDiameter, Pen lollipopPen, Brush lollipopBrush)
        {
            string str = null;

            if (element != this.AdornedElement)
                str = "AdornedElement won't match!!!";

            if (lollipopPosition != this.LollipopPosition)
                str = str + "\nLollipopPosition won't match!!!";

            if (centerPoint != this.CenterPoint)
                str = str + "\nCenterPoint won't match!!!";

            if (anchorSize.Equals(this.AnchorSize))
                str = str + "\nAnchor size is not correct";

            if (anchorPen != this.AnchorPen)
                str = str + "\nAnchor pen won't match!!!";

            if (anchorBrush != this.AnchorBrush)
                str = str + "\nAnchor brush won't match!!!";

            if (lollipopStemLength != this.LollipopStemLength)
                str = str + "\nlollipopStemLength won't match!!!";

            if (lollipopHeadDiameter != this.LollipopHeadDiameter)
                str = str + "\nlollipopHeadDiameter won't match";

            if (lollipopPen != this.LollipopPen)
                str = str + "\nlollipopPen won't match!!!";

            if (lollipopBrush != this.LollipopBrush)
                str = str + "\nlollipopBrush!!!";

            return str;
        }
    }
}