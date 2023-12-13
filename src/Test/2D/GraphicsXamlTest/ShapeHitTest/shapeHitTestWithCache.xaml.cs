// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Same as the existing shape hit test, but this one has a cache on the topmost element.
    /// This test case requires version 4.0
    /// </summary>

    public partial class ShapeHitTestWithCache : Window
    {

        
        public ShapeHitTestWithCache()
        {
            InitializeComponent();
            DpiScalingHelper.ScaleWindowToFixedDpi(this, 96.0f, 96.0f);
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            ArrayList alTestData = PrepareTestData();
            
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "ShapeHitTest.bmp");
            XamlTestHelper.AddStep(Verify);

            for (int i = 0; i < alTestData.Count; i++)
            {
                //Setting the Timeout between each action
                //Longer timeout is needed because this tests involve a lot of Mouse movement
                XamlTestHelper.AddStep(HitTest, alTestData[i], 500);
                XamlTestHelper.AddStep(VerifyHitTest, alTestData[i]);
            }

            XamlTestHelper.AddStep(XamlTestHelper.Quit);

            XamlTestHelper.Run();
        }

        public object Verify(object arg)
        {
            if (XamlTestHelper.CompareWithoutDPI("ShapeHitTest.bmp"))
            {
                XamlTestHelper.LogStatus(" *** Good:  all shapes were rendered correctly");
                XamlTestHelper.LogStatus(" ***   Start the HitTest test");
            }
            else
            {
                XamlTestHelper.LogFail(" *** Bad: The Shapes rendered different than expected");
                XamlTestHelper.LogFail(" ***   HitTest would mostly likely failed due to this!");
            }
            return null;
        }

        private Point getHitPoint(object arg)
        {
            //Bounding box of the element relative to the screen coordinate.
            Rect relativeBound = Input.GetScreenRelativeRect(((TestData)arg).Element);
            //window topleft coord
            Rect winrect = Input.GetScreenRelativeRect(this);
            //need to adjust moveto locations to take into account non DPI aware window location reporting.from above call.
            float s1 = (Microsoft.Test.Display.Monitor.Dpi.x-96.0f)/96.0f;
            float xAdjust = (float)winrect.Left*-s1;
            float yAdjust = (float)winrect.Top*-s1;
            
            //HitPoint position relative to the screen coordinate.
            //transform position of bounding rect to match DPI scaling and adjust for window position
            float s=Microsoft.Test.Display.Monitor.Dpi.x/96.0f;
            Point hitpoint = new Point( s*relativeBound.TopLeft.X + 
                                        ((TestData)arg).OffSet.X +
                                        xAdjust,
                                        s*relativeBound.TopLeft.Y + 
                                        ((TestData)arg).OffSet.Y +
                                        yAdjust);
            return hitpoint;
        }

        public object HitTest(object arg)
        {
            //Clean up the state of the test first
            _isHit = false;
            _elementID = string.Empty;

            //The argument should be TestData type
            if (! (arg is TestData))
            {
                throw new System.ApplicationException(" *** Real Bad:  incorrect argument was passed into HitTest call");
            }

            string result = (((TestData)arg).ExpectedResult)? "Hit" : "No Hit";
            XamlTestHelper.LogStatus("Variation " + ++_varCount + 
                ": OffSet[ " + ((TestData)arg).OffSet + " ] and Element=" + 
                ((Shape)((TestData)arg).Element).Name + 
                " Expected Result = " + result);

            Point hitpoint =  getHitPoint( arg);

            Input.MouseMove((int)(hitpoint.X), (int)(hitpoint.Y));
            return null;
        }

        public object VerifyHitTest(object arg)
        {
            //The argument should be TestData type
            if (!(arg is TestData))
            {
                throw new System.ApplicationException(" *** Real Bad:  incorrect argument was passed into HitTest call");
            }

            if (((TestData)arg).ExpectedResult)
            {
                //Expect a hit
                if (!_isHit)
                {
                    XamlTestHelper.LogFail("  OffSet[ " + ((TestData)arg).OffSet + " ] should have hit the element " + ((Shape)((TestData)arg).Element).Name);
                    return null;
                }

                if (string.IsNullOrEmpty(_elementID))
                {
                    XamlTestHelper.LogFail("  OffSet[ " + ((TestData)arg).OffSet + " ] should have hit the element " + ((Shape)((TestData)arg).Element).Name);
                    return null;
                }

                if (string.Compare(_elementID, ((Shape)((TestData)arg).Element).Name, false, System.Globalization.CultureInfo.InvariantCulture) != 0)
                {
                    XamlTestHelper.LogFail(" Hit the wrong element");
                    XamlTestHelper.LogFail(" ***    Expect:  " + ((Shape)((TestData)arg).Element).Name);
                    XamlTestHelper.LogFail(" ***    Actual:  " + _elementID);
                    return null;
                }
            }
            else
            {
                if (_isHit)
                {
                    XamlTestHelper.LogFail("  expected result = NO HIT, but it hits");
                    return null;
                }
            }

            XamlTestHelper.LogStatus(" *** This variation passed!");
            return null;
        }

        void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //XamlTestHelper.LogStatus(((Shape)sender).Name + " is hit");
            _isHit = true;
            _elementID = ((Shape)sender).Name;
        }

        private ArrayList PrepareTestData()
        {
            
            ArrayList testDatas = new ArrayList();
            
            testDatas.Add(new TestData(new Point(20, 15),  Line1,      true  ));
            testDatas.Add(new TestData(new Point(-10, -10),Line1,      false ));
            testDatas.Add(new TestData(new Point(0, 0),    Line2,      false ));
            testDatas.Add(new TestData(new Point(13, 10),  Line2,      true  ));
            testDatas.Add(new TestData(new Point(3, 3),    Rectangle1, true  ));
            testDatas.Add(new TestData(new Point(-10, 10), Rectangle1, false ));
            testDatas.Add(new TestData(new Point(30, 35),  Rectangle2, true  ));
            testDatas.Add(new TestData(new Point(5, 5),    Rectangle2, false ));
            testDatas.Add(new TestData(new Point(38, 2),   Ellipse1,   true  ));
            testDatas.Add(new TestData(new Point(40, 14),  Ellipse1,   false ));
            testDatas.Add(new TestData(new Point(30, 30),  Ellipse2,   true  ));
            testDatas.Add(new TestData(new Point(75, 0),   Ellipse2,   false ));
            testDatas.Add(new TestData(new Point(166, 77), Path1,      true  ));
            testDatas.Add(new TestData(new Point(0, 0),    Path1,      false ));
            testDatas.Add(new TestData(new Point(131, 76), Path1,      false ));
            return testDatas;
        }
        private string _elementID = string.Empty;
        private bool       _isHit = false;
        private int     _varCount = 0;
    }

}
