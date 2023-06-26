// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.ComponentModel;
    using System.Collections;
    using SysDrawing = System.Drawing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Documents;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>AdornerLayerBVTTest .</summary>
    [Test(1, "Adorner", "AdornerManagmentTest2", MethodParameters = "/TestCaseType=AdornerManagmentTest /Case=Regression_Bug272")]
    [Test(0, "Adorner", "AdornerManagmentTest1", MethodParameters = "/TestCaseType=AdornerManagmentTest /Case=Test_AdornerDecorator")]
    [Test(0, "Adorner", "AdornerManagmentTest", MethodParameters = "/TestCaseType=AdornerManagmentTest /Case=Test_AdornerLayer_Methods")]
    [TestOwner("Microsoft"), TestBugs("378,379"), TestTactics("21"), TestWorkItem(""), TestLastUpdatedOn("Jan 25, 2007")]
    public class AdornerManagmentTest : CommonTestCase
    {
        private FrameworkElement _testElement1;
        private FrameworkElement _testElement2;
        private Pen _pen;
        private Brush _brush;
        private Adorner[] _adorners;
        private AdornerLayer _adornerLayer;
        private Canvas _designRoot;        

        /// <summary>Initializes the instance.</summary>
        public override void Init()
        {
            MyLogger.Log("Enter function - AdornerLayerBVTTest.Init()");
            MainWindow.Content = new AdornerDecorator();
            _designRoot = new Canvas();
            ((AdornerDecorator)MainWindow.Content).Child = _designRoot;
            _pen = new Pen(new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00)), 1.0f);
            _brush = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xff));
            _testElement1 = CreateElementWithSize("Button", 0, 0, 100, 100);
            _testElement2 = CreateElementWithSize("Button", 100, 100, 200, 200);
            _designRoot.Children.Add(_testElement1);
            _designRoot.Children.Add(_testElement2);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(SetAdorners));
            MyLogger.Log("End function - AdornerLayerBVTTest.Init()");
        }

        /// <summary> SetAdorners</summary>
        void SetAdorners()
        {
            _adorners = new Adorner[3];
            _adorners[0] = new GrabHandleAdorner(_testElement1, GrabHandleAnchor.Inside, GrabHandles.TopLeft, new Size(50, 50), _pen, Brushes.Red);
            _adorners[1] = new GrabHandleAdorner(_testElement1, GrabHandleAnchor.Outside, GrabHandles.RightCenter, new Size(50, 50), _pen, Brushes.Blue);
            _adorners[2] = new GrabHandleAdorner(_testElement2, GrabHandleAnchor.Inside, GrabHandles.All, new Size(50, 50), _pen, Brushes.Yellow);

            //designSurface.AdornerLayer.Add(adorner);
            _adornerLayer = AdornerLayer.GetAdornerLayer(_designRoot);
            _adornerLayer.Add(_adorners[0]);
            _adornerLayer.Add(_adorners[1]);
            _adornerLayer.Add(_adorners[2]);
        }

        /// <summary>Add_Remove_clear_GetAdornerFromPoint .</summary>
        [TestCase(LocalCaseStatus.Ready, "Test AdornerLayer Methods")]
        public void Test_AdornerLayer_Methods()
        {
            EnterFunction("Test_AdornerLayer_Methods");

            Point point = AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToTheElement, _adorners[1] as GrabHandleAdorner, GrabHandles.RightCenter);
            AdornerHitTestResult adResult = _adornerLayer.AdornerHitTest(point);

            Verifier.Verify(adResult != null && adResult.Adorner == _adorners[1] && adResult.VisualHit != null && adResult.PointHit.X >= 25 && adResult.PointHit.Y >= 25, "AdornerHitTest failed in AdornerLayer!!!");

            Adorner[] ac = _adornerLayer.GetAdorners(_testElement1);

            Verifier.Verify(ac != null && ac.Length == 2 && ac[0] != null && ac[1] != null, "adornerCollection is not correct!!!");
            ac = null;
            ac = _adornerLayer.GetAdorners(_testElement2);
            Verifier.Verify(ac != null && ac.Length == 1 && ac[0] == _adorners[2], "adornerCollection is not correct!!!");
            _adornerLayer.Update(_testElement1);
            _adornerLayer.Update();
            _adornerLayer.Remove(_adorners[1]);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Test_RemoveAdorner));
            EndFunction();
        }

        /// <summary>Test_RemoveAdorner </summary>
        void Test_RemoveAdorner()
        {
            EnterFunction("Test_RemoveAdorner");

            Point point = AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToTheElement, _testElement1, GrabHandles.RightCenter);
            AdornerHitTestResult adResult = _adornerLayer.AdornerHitTest(point);

            Verifier.Verify(adResult == null, "Adorner[1] should be removed!!!");
            point = AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToTheElement, _testElement1, GrabHandles.TopLeft);
            adResult = _adornerLayer.AdornerHitTest(point);
            Verifier.Verify(adResult.Adorner == _adorners[0], "Adorner[0] should exist!!!");

            _adornerLayer.Remove(_adorners[0]);
            
            Logger.Current.Log("Verifying that removing an adorner twice is a no-op...");
            _adornerLayer.Remove(_adorners[0]);
            
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Test_NullParameters));

            EndFunction();
        }

        /// <summary> Test_ClearAdornerForElement</summary>
        void Test_NullParameters()
        {
            MyLogger.Log("Enter function - Test_NullParameters.");

            Point point = AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToTheElement, _adorners[0] as GrabHandleAdorner, GrabHandles.LeftCenter);
            AdornerHitTestResult adResult = _adornerLayer.AdornerHitTest(point);

            Verifier.Verify(adResult == null, "Adorner[0] should be removed!!!");

            //test null parameter for adornerLayer.Update()
            try
            {
                pass = false;
                _adornerLayer.Update(null);
            }
            catch (Exception e)
            {
                Verifier.Verify(e.Message.Contains("element"), "We got wrong exception from calling adornerLayer.Update(null)!!!");
                pass = true;
            }
            Verifier.Verify(pass, "We did not get an exception from calling adornerLayer.Update(null)!!!");

            //test null parameter for AdornerLayer.GetAdorners()
            try
            {
                pass = false;
                _adornerLayer.GetAdorners(null);
            }
            catch (Exception e)
            {
                Verifier.Verify(e.Message.Contains("element"), " we get wrong exception from calling adornerLayer.GetAdorners(null)!!!");
                pass = true;
            }
            Verifier.Verify(pass, "We did not get an exception from calling adornerLayer.GetAdorners(null)!!!");

            //test null parameter for AdornerLayer.Remove()
            try
            {
                pass = false;
                _adornerLayer.Remove(null);
            }
            catch (Exception e)
            {
                Verifier.Verify(e.Message.Contains("adorner"), " we get wrong exception from calling adornerLayer.Remove(null)!!!");
                pass = true;
            }
            Verifier.Verify(pass, "We did not get an exception from calling adornerLayer.Remove(null)!!!");

            //test null parameter for AdornerLayer.Add()
            try
            {
                pass = false;
                _adornerLayer.Add(null);
            }
            catch (Exception e)
            {
                Verifier.Verify(e.Message.Contains("adorner"), " we get wrong exception from calling adornerLayer.Add(null)!!!");
                pass = true;
            }
            Verifier.Verify(pass, "We did not get an exception from calling adornerLayer.Add(null)!!!");
            QueueDelegate(EndTest);

            MyLogger.Log("End Function - Test_NullParameters()");
        }

        /// <summary>case to test AdornerDecorator</summary>
        [TestCase(LocalCaseStatus.Ready, "Test AdornerDecorator")]
        public void Test_AdornerDecorator()
        {
            EnterFunction("Test_AdornerDecorator");

            try
            {
                pass = false;
                AdornerLayer.GetAdornerLayer(null);
            }
            catch (ArgumentNullException e)
            {
               MyLogger.Log(e.Message);
               pass = true;
            }
            Verifier.Verify(pass, "No exception is caught for null paramegter of GetAdornerLayer()!!!");

            _adornerLayer = AdornerLayer.GetAdornerLayer(new Button());
            Verifier.Verify(null == _adornerLayer, "There should be no adornerLayer since button is on the tree!!!");
            _adornerLayer = AdornerLayer.GetAdornerLayer(_testElement1);
            Verifier.Verify(null != _adornerLayer, "adornerLayer must not be null!!!");

            AdornerDecorator ad = FindAdornerDecorder();
            Verifier.Verify(ad != null, "AdornerDecorder is not found!!!");
            Verifier.Verify(ad.AdornerLayer == _adornerLayer, "adornerLayer does not match!!!");

            object o = ad.Child;

            Verifier.Verify(o != null, "AdornerDecorator should not be null!!!");
            ad.Child = null;
            Verifier.Verify(ad.Child == null, "Afer null is set, we should get the null back!!!");
            ad.Child = o as UIElement;
            Verifier.Verify(ad.Child == o, "After o is set, we should get o back!!!");

            MyLogger.Log("test the constructors");
            AdornerDecorator myDecorator;

            myDecorator = new AdornerDecorator();
            Verifier.Verify(myDecorator.AdornerLayer != null, "adornerLayer in AdornerDecorator should not be null!!!");
            myDecorator.Child = new Button();
            Verifier.Verify(myDecorator.Child is Button, "the Child must be a button");

            myDecorator = new AdornerDecorator();
            Verifier.Verify(myDecorator.AdornerLayer != null, "adornerLayer in AdornerDecorator should not be null!!!");
            myDecorator.Child = new TextBox();
            Verifier.Verify(myDecorator.Child is TextBox, "the Child must be a TextBox");

            QueueDelegate(EndTest);
            EndFunction();
        }

        /// <summary>repgression for Regression_Bug272 - No excpetion should be thrown when adornering a zero sized element </summary>
        [TestCase(LocalCaseStatus.Ready, "Regression for Regression_Bug272")]
        public void Regression_Bug272()
        {
            DockPanel dp = new DockPanel();
            dp.Width = 0;
            dp.Height = 0;
            _designRoot.Children.Clear();
            _designRoot.Children.Add(dp);
            _adornerLayer.Add(new GrabHandleAdorner(dp));
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(EndTest));
        }                        

        /// <summary>FindAdornerDecorder </summary>
        /// <returns>return AdornerDecorator</returns>
        AdornerDecorator FindAdornerDecorder()
        {
            Visual parent = (Visual)VisualTreeHelper.GetParent(_designRoot);

            while (parent != null)
            {
                if (parent is AdornerDecorator)
                    return parent as AdornerDecorator;

                parent = (Visual)VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }

    /// <summary>
    /// Test for IsClipEnabled property on Adorner
    /// </summary>
    [Test(2, "Adorner", "AdornerIsClipEnabledTest", MethodParameters = "/TestCaseType=AdornerIsClipEnabledTest", Disabled = true)]
    [TestOwner("Microsoft"), TestBugs("380,381"), TestTactics("22"), TestWorkItem(""), TestLastUpdatedOn("Jan 25, 2007")]
    public class AdornerIsClipEnabledTest : CustomTestCase
    {
        private AdornerDecorator _adrDcr;
        private AdornerLayer _adrLyr;
        private Adorner _adorner1;
        private Canvas _panel;
        private FrameworkElement _fe1,_fe2;
        private ComparisonCriteria _criteria;

        private SysDrawing.Bitmap _bmpClipNotEnabled,_bmpClipEnabled,_bmpClipDisabled;

        /// <summary>Starts the test case</summary>
        public override void RunTestCase()
        {
            _adrDcr = new AdornerDecorator();
            _panel = new Canvas();
            _panel.Height = 200;
            _panel.Width = 200;            
            _adrDcr.Child = _panel;

            _fe1 = new TextBox();
            _fe1.Height = 100;
            _fe1.Width = 200;
            _fe1.SetValue(Canvas.LeftProperty, 0d);
            _fe1.SetValue(Canvas.TopProperty, 0d);
            _fe1.ClipToBounds = true;

            _fe2 = new TextBox();
            _fe2.Height = 100;
            _fe2.Width = 200;
            _fe2.SetValue(Canvas.LeftProperty, 0d);
            _fe2.SetValue(Canvas.TopProperty, 100d);

            _panel.Children.Add(_fe1);
            _panel.Children.Add(_fe2);

            _adrLyr = AdornerLayer.GetAdornerLayer(_panel);
            _adorner1 = new GrabHandleAdorner(_fe1, GrabHandleAnchor.Centered, GrabHandles.BottomCenter,
                new Size(100, 100), new Pen(Brushes.Blue, 2d), Brushes.Yellow);
            _adrLyr.Add(_adorner1);

            MainWindow.Content = _adrDcr;

            QueueDelegate(CaptureClipNotEnabled); 
        }

        private void CaptureClipNotEnabled()
        {
            _bmpClipNotEnabled = BitmapCapture.CreateBitmapFromElement(_panel);
            _adorner1.IsClipEnabled = true;

            QueueDelegate(CaptureClipEnabled);
        }

        private void CaptureClipEnabled()
        {
            _bmpClipEnabled = BitmapCapture.CreateBitmapFromElement(_panel);            

            //compare the images     
            Log("Comparing the bitmap captures before and after setting IsClipEnabled");
            _criteria = new ComparisonCriteria();
            _criteria.MaxColorDistance = 0.12f;

            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_bmpClipNotEnabled, _bmpClipEnabled, _criteria, false))
            {
                Logger.Current.LogImage(_bmpClipNotEnabled, "BmpClipNotEnabled");
                Logger.Current.LogImage(_bmpClipEnabled, "BmpClipEnabled");
                Verifier.Verify(false, "Failed, Comparing bitmap captures BmpClipNotEnabled.png and BmpClipEnabled.png!");
            }
            
            _adorner1.IsClipEnabled = false;
            QueueDelegate(CaptureClipDisabled);
        }

        private void CaptureClipDisabled()
        {
            _bmpClipDisabled = BitmapCapture.CreateBitmapFromElement(_panel);  

            //compare the images     
            Log("Comparing the bitmap captures after setting IsClipEnabled back to false");            
            _criteria.MaxErrorProportion = 0.01f;

            if (!ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_bmpClipNotEnabled, _bmpClipDisabled, _criteria, false))
            {
                Logger.Current.LogImage(_bmpClipNotEnabled, "BmpClipNotEnabled");
                Logger.Current.LogImage(_bmpClipDisabled, "BmpClipDisabled");
                Verifier.Verify(false, "Failed, Comparing bitmap captures BmpClipNotEnabled.png and BmpClipDisabled.png!");
            }

            Logger.Current.ReportSuccess();
        }
    }
}