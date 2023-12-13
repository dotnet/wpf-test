// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/********************************************************************
*
*this file contains code for teseting GrabHandleAdorner functionalitis.
*
**Note: the GrabHandleAdorner will be cut. Don't waste time on this
*********************************************************************/

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Input;
    using Microsoft.Test.Threading;
    using Test.Uis.Management;
    using Test.Uis.Utils;    

    #endregion Namespaces.

    /// <summary>
    /// Verifies GrabHandleAdorner functionality.
    /// </summary>
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "GrabHandleAdornerBVTTest2", MethodParameters = "/TestCaseType=GrabHandleAdornerBVTTest /Case=RunAllCases /XbapName=EditingTestDeploy", Timeout = 120)]
    [Test(0, "Adorner", "GrabHandleAdornerBVTTest1", MethodParameters = "/TestCaseType=GrabHandleAdornerBVTTest /Case=RunAllCases")]
    [Test(2, "Adorner", "GrabHandleAdornerBVTTest", MethodParameters = "/TestCaseType=GrabHandleAdornerBVTTest /Case=Regression_Bug273")]
    [TestOwner("Microsoft"), TestBugs("274, 273, 382, 383, 384"), TestTactics("23"), TestLastUpdatedOn("Jan 25, 2007")]
    public class GrabHandleAdornerBVTTest : CommonTestCase
    {
        private int _elementCount = 48,_elementWidth=40,_elementHeight=80;
        private FrameworkElement[] _testElements; 
        private Pen _pen;
        private Brush _brush;
        private Size _adornerSize;
        private Adorner[] _adorners;
        private Canvas _designRoot;
        private string _controlType;
        int[] _mouseEventCount ;


        System.Drawing.Color _testColor;

        private AdornerLayer _adornerLayer = null;

        public override void Init()
        {
            MyLogger.Log("Enter Function - GrabHandleAdornerTest.init().");
            string FlowDir = ConfigurationSettings.Current.GetArgument("FlowDir");
            AdornerDecorator decorator = new AdornerDecorator();
            MainWindow.Content = decorator;
            _designRoot = new Canvas();
            _designRoot.Background = Brushes.Red;
            decorator.Child = _designRoot;
            if (MainWindow.Content is AdornerDecorator == false)
            {
                DispatcherHelper.DoEvents();
                MainWindow.Content = decorator;
            }

            if (FlowDir != null && FlowDir != string.Empty && FlowDir.ToLower() == "rltb")
                _designRoot.FlowDirection = FlowDirection.RightToLeft;

            _pen = new Pen(Brushes.Black, 1.0f);
            _brush = Brushes.Blue;
            if (_controlType == null || _controlType == string.Empty)
                _controlType = "Button";

            //ServiceManager.SetService(typeof(DesignerLookupService), DesingRoot, new DesignerLookupService());
            _testElements = new FrameworkElement[_elementCount];
            _adorners = new Adorner[_elementCount];
            _mouseEventCount = new int[_elementCount];
            for (int w = 0; w < _elementCount; w++)
                _mouseEventCount[w] = 0;

            for (int i = 0, k = 1, j = 1; i < _testElements.Length; i++, j++)
            {
                if (j == 13)
                {
                    k++;
                    j = 1;
                }

                _testElements[i] = CreateElementWithSize(_controlType, j * 60, k * 100, _elementWidth, _elementHeight);
                _designRoot.Children.Add(_testElements[i]);
            }
            MyLogger.Log("End Function - GrabHandleAdornerTest.init(). Init");
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

        void SetUpAdorners()
        {
            int x = 0;

            _adornerSize = new Size(5, 5);
            for (int j = 0; j < 4; j++)
            {
                GrabHandleAnchor g;
                if(j<3)
                    g = (GrabHandleAnchor)j;
                else 
                    g = default(GrabHandleAnchor);
            
                for (GrabHandles handle = GrabHandles.TopLeft; handle <= GrabHandles.BottomRight; handle = ((GrabHandles)(((int)handle) << 1)))
                {
                    //use constructor with 6 parameters
                    _adorners[x] = new SquareAdorners(_testElements[x], g, handle, _adornerSize, _pen, _brush);
                    ((UIElement)_adorners[x]).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnMouseDown);            
                    _adornerLayer.Add(_adorners[x++]);
                }
                //use constructor with 6 parameters
                _adorners[x] = new SquareAdorners(_testElements[x], g, GrabHandles.Corners, _adornerSize, _pen, _brush);
                ((UIElement)_adorners[x]).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnMouseDown);
                _adornerLayer.Add(_adorners[x++]);
                _adorners[x] = new SquareAdorners(_testElements[x], g, GrabHandles.Centers, _adornerSize, _pen, _brush);
                ((UIElement)_adorners[x]).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnMouseDown);
                _adornerLayer.Add(_adorners[x++]);
                _adorners[x] = new SquareAdorners(_testElements[x], g, GrabHandles.All, _adornerSize, _pen, _brush);
                ((UIElement)_adorners[x]).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnMouseDown);
                _adornerLayer.Add(_adorners[x++]);
                _adorners[x] = new SquareAdorners(_testElements[x], g, GrabHandles.None, _adornerSize, _pen, _brush);
                ((UIElement)_adorners[x]).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnMouseDown);
                _adornerLayer.Add(_adorners[x++]);
            }

           _adornerLayer.Update();
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

        #region cases - BVT cases
       /// <summary>GrabHandleAdorner_FormTestElement.</summary>
        [TestCase(LocalCaseStatus.Ready, "Test GrabHandleAdorner")]
        public void GrabHandleAdorner()
        {
            EnterFunction("GrabHandleAdorner_FormTestElement");
            ClearAdorners();
            SetUpAdorners();

            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CheckResult));
            EndFunction();
        }

        void CheckResult()
        {
            string str = null;
            int i;

            //evaluate the adorners.
            for (i = 0; i < _elementCount; i++)
            {
                str = ((SquareAdorners)_adorners[i]).VerifyAdorners(_adornerLayer);
                if (!(str == null || string.Empty == str))
                {
                    MyLogger.Log("Failed - " + str);
                    pass = false;
                    QueueDelegate(EndTest);
                    return;
                }
            }

            //evaluate properties.
            i = 0;
            for (GrabHandles handle = GrabHandles.TopLeft; handle <= GrabHandles.BottomRight; handle = ((GrabHandles)(((int)handle) << 1)))
            {
                str = str + ((SquareAdorners)_adorners[i]).VerifyProperties(_testElements[i++], GrabHandleAnchor.Outside, handle, _adornerSize, _pen, _brush);
            }

            if (!(str == null || string.Empty == str))
            {
                MyLogger.Log("Failed - " + str);
                pass = false;
            
            }
            QueueDelegate(EndTest);
        }
        
        void Regression_Bug274_VerifyMouseEvents()
        {
            int j;
            for (int i = 0; i < _elementCount; i++)
            {
                j = i % 12;
                if ((j <= 7 && _mouseEventCount[i] != 1))
                {
                    pass = false;
                    MyLogger.Log(CurrentFunction + " - Failed: Expected mouse event fires: [1], Actual mouse event firs: [" + _mouseEventCount[i].ToString() + "]" + "for TestElement[" + i.ToString() + "]");
                }
                else if ((j == 8 || j == 9) && _mouseEventCount[i] != 4)
                {
                    pass = false;
                    MyLogger.Log(CurrentFunction + " - Failed: Expected mouse event fires: [4], Actual mouse event firs: [" + _mouseEventCount[i].ToString() + "]" + "for TestElement[" + i.ToString() + "]");
                }
                else if (j == 10 && _mouseEventCount[i] != 8)
                {
                    pass = false;
                    MyLogger.Log(CurrentFunction + " - Failed: Expected mouse event fires: [i], Actual mouse event firs: [" + _mouseEventCount[i].ToString() + "]" + "for TestElement[" + i.ToString() + "]");
                }
                else if (j == 11 && _mouseEventCount[i] != 0)
                {
                    pass = false;
                    MyLogger.Log(CurrentFunction + " - Failed: Expected mouse event fires: [0], Actual mouse event firs: [" + _mouseEventCount[i].ToString() + "]" + "for TestElement[" + i.ToString() + "]");
                }
            }
            QueueDelegate(EndTest);
        }
        # endregion 

        #region -Regression case - Regression_Bug275
        /// <summary>Regression_Bug275 - change properties will cause the adorner(s) to be redrawn</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test Regression_Bug275")]
        public void Regression_Bug275()
        {
            EnterFunction("Regression_Bug275_CaseStart");
            _adornerLayer = AdornerLayer.GetAdornerLayer(_designRoot);
            ClearAdorners();
            GrabHandleAdorner ga = new GrabHandleAdorner(_testElements[0], GrabHandleAnchor.Inside, GrabHandles.TopLeft);
            _adornerLayer.Add(ga);
            //change size
            ga.Size = new Size(40, 40);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(WhenAdornerSizeChanged));
        }

        void WhenAdornerSizeChanged()
        {
            EnterFunction("WhenAdornerSizeChanged");
            Adorner[] arry = _adornerLayer.GetAdorners(_testElements[0]);
            GrabHandleAdorner ga = arry[0] as GrabHandleAdorner;

            Sleep();
            Test.Uis.Utils.MouseInput.MouseMove(AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToScreen, _testElements[0],GrabHandles.TopLeft));

            Point point = AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToClientArea, _testElements[0], GrabHandles.TopLeft);
            
			AdornerHitTestResult adResult = _adornerLayer.AdornerHitTest(point);

            if (adResult == null)
            {
                MyLogger.Log(CurrentFunction + " - Failed: to hittest the redrawn adorner for size change!!!");
                pass = false;
            }

            System.Drawing.Bitmap adb = BitmapCapture.CreateBitmapFromElement(ga);

            ga.Brush = Brushes.Red;
            _testColor = adb.GetPixel(20, 20);
            EndFunction();
			if (pass)
				Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(WhenAdornerColorChanged));
			else
				QueueDelegate(EndTest);
		}
        
        void WhenAdornerColorChanged()
        {
            EnterFunction("WhenAdornerColorChanged");
            Adorner[] arry = _adornerLayer.GetAdorners(_testElements[0]);
            GrabHandleAdorner ga = arry[0] as GrabHandleAdorner;

            System.Threading.Thread.Sleep(1000);
            Test.Uis.Utils.MouseInput.MouseMove(AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToScreen, _testElements[0],  GrabHandles.TopLeft));

            Point point = AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToClientArea, _testElements[0], GrabHandles.TopLeft);
            AdornerHitTestResult adResult = _adornerLayer.AdornerHitTest(point);

            if(adResult == null)
            {
                MyLogger.Log(CurrentFunction + " - Failed: to hittest the redrawn adorner for size change!!!");
                pass = false;
            }

            System.Drawing.Bitmap adb =Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(ga);
            System.Drawing.Color CurrentColor = adb.GetPixel(20, 20);
            if(_testColor.Equals(CurrentColor))
            {
                MyLogger.Log(CurrentFunction +  "- Failed: Adorner color should change to Red!!!");
                pass = false;
            }
            //change anchor
            ga.GrabHandleAnchor = GrabHandleAnchor.Outside;
            EndFunction();
            if (pass)
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(WhenAnchorChanged));
            else
                QueueDelegate(EndTest);
        }

        void WhenAnchorChanged()
        {
            EnterFunction("WhenAnchorChanged");
            Adorner[] arry = _adornerLayer.GetAdorners(_testElements[0]);
            GrabHandleAdorner ga = arry[0] as GrabHandleAdorner;

            System.Threading.Thread.Sleep(1000);
            Test.Uis.Utils.MouseInput.MouseMove(AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToScreen, _testElements[0], GrabHandles.TopLeft));

            Point point = AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToClientArea, _testElements[0], GrabHandles.TopLeft);
            AdornerHitTestResult adResult = _adornerLayer.AdornerHitTest(point);

            if(adResult == null)
            {
                MyLogger.Log(CurrentFunction + "- Failed: to hittest the redrawn adorner for anchor change!!!");
                pass = false;
            }
            EndFunction();
            QueueDelegate(EndTest);
        }
        #endregion 
    
        #region Regression case - Regression_Bug273
        /// <summary>Regression_Bug273, No null value  of pen and brush for adorners </summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test Regression_Bug273")]
        public void Regression_Bug273()
        {
            EnterFunction("Regression_Bug273_CaseStart");

            GrabHandleAdorner ga = new GrabHandleAdorner(new FrameworkElement());
            LollipopAdorner la = new LollipopAdorner(new FrameworkElement(), LollipopPosition.TopCenter);

			try
			{
				ga.Brush = null;
				throw new Exception("GrabHandleAdorner.Brush should not accept null value!!!");
			}
			catch (ArgumentNullException e)
			{
				MyLogger.Log(e.Message);
			}

			try
            {
                ga.Pen = null;
                throw new Exception("GrabHandleAdorner.Pen should not accept null value!!!");
            }
            catch (ArgumentNullException e)
            {
				MyLogger.Log(e.Message);
			}
			try
            {
                la.AnchorPen = null;
                throw new Exception("LollipopAdorner.AnchorPen should not accept null value!!!");
            }
            catch (ArgumentNullException e)
            {
				MyLogger.Log(e.Message);
			}
			try
            {
                la.AnchorBrush = null;
                throw new Exception("LollipopAdorner.AnchorBrush should not accept null value!!!");
            }
            catch (ArgumentNullException e)
			{ 
				MyLogger.Log(e.Message);
			}
			try
            {
                la.LollipopBrush = null;
                throw new Exception("LollipopAdorner.LollipopBrush should not accept null value!!!");
            }
            catch (ArgumentNullException e)
            {
				MyLogger.Log(e.Message);
			}
			try
            {
                la.LollipopPen = null;
                throw new Exception("LollipopAdorner.LollipopPen should not accept null value!!!");
            }
            catch (ArgumentNullException e)
            {
				MyLogger.Log(e.Message);
			}
			QueueDelegate(EndTest);
            MyLogger.Log("End function - Regression_Bug273()");
        }
        #endregion 
    }

    /// <summary>A custom GrabHandleAdorner for testing purposes.</summary>
    public class SquareAdorners : GrabHandleAdorner
    {

        /// <summary>Creates a new SquareAdorners instance.</summary>
        public SquareAdorners(UIElement element) : base(element)
        {
        }

        /// <summary>Creates a new SquareAdorners instance.</summary>
        public SquareAdorners(UIElement element, GrabHandleAnchor anchor, GrabHandles grabHandles) : base(element, anchor, grabHandles)
        {
        }

        /// <summary>Creates a new SquareAdorners instance.</summary>
        public SquareAdorners(UIElement element, GrabHandleAnchor anchor, GrabHandles grabHandles, Size size, Pen pen, Brush brush) 
            : base(element, anchor, grabHandles, size, pen, brush)
        {
        }

        /// <summary>VerifyProperties.</summary>
        public string VerifyProperties(UIElement element, GrabHandleAnchor anchor, GrabHandles grabHandles, Size size, Pen pen, Brush brush)
        {
            string str = null;

            if (element != this.AdornedElement)
                str = "AdornerElement won't match!!!";
            else if (anchor != this.GrabHandleAnchor)
                str = str + "\nAdornerElement won't match!!!";
            else if (grabHandles != this.GrabHandles)
                str = str + "\nGrabHandle won't match!!!";
            else if (!size.Equals(this.Size))
                str = str + "\nSize won't match!!!";
            else if (pen != this.Pen)
                str = str + "\n pen won't match!!!";
            else if (brush != this.Brush)
                str = str + "\n brush won't match!!!";

            return str;
        }

        /// <summary>VerifyAdorners.</summary>
        public string VerifyAdorners( AdornerLayer adornerLayer)
        {
            Point point;

            for (GrabHandles handle = GrabHandles.TopLeft; handle <= GrabHandles.BottomRight; handle = ((GrabHandles)(((int)handle) << 1)))
            {
                // the mouse move used here is to help the visual verification and debug purpose
                // Test.Uis.Utils.MouseInput.MouseClick(AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToScreen, AdornedElement, handle));
                // System.Threading.Thread.Sleep(15);
                point = AdornerUtils.GetGrabHandleLocation(AdornerUtils.AdornerRelatives.RelativeToClientArea, AdornedElement, handle);

                AdornerHitTestResult adResult = adornerLayer.AdornerHitTest(point);

                if ((handle & this.GrabHandles) != 0)
                {
                    //note: we assume that no adorner overlap.
                    if (adResult == null)
                    {
                        System.Threading.Thread.Sleep(3000);
                        return "Can't find adorner at mouse pointed location!!!";
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
            return null;
        }
    }
}