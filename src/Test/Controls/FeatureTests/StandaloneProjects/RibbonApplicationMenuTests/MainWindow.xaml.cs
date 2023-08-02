using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Test.Input;
using System.Windows.Threading;
using System.Threading;
using Avalon.Test.ComponentModel;
using Microsoft.Test.VisualVerification;


namespace RibbonApplicationMenuTests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //run RAM tests
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            RunFunctionalTest("Click_RAM Click_Save CheckResult_Save");

            RunFunctionalTest("SetRTL_foo ClickRTL_RAM Wait_foo IsVisible_Save");

            RunFunctionalTest("SetFooterLength_0 Wait_foo Click_RAM Wait_foo VerifyFooterLengthIs_0");
            RunFunctionalTest("SetFooterLength_1 Wait_foo Click_RAM Wait_foo VerifyFooterLengthIs_1");
            RunFunctionalTest("SetFooterLength_100 Wait_foo Click_RAM Wait_foo VerifyFooterLengthIs_100");

            RunFunctionalTest("SetRecentDocsLength_0 Wait_foo Click_RAM Wait_foo VerifyRecentDocsLengthIs_0");
            RunFunctionalTest("SetRecentDocsLength_1 Wait_foo Click_RAM Wait_foo VerifyRecentDocsLengthIs_1");
            RunFunctionalTest("SetRecentDocsLength_100 Wait_foo Click_RAM Wait_foo VerifyRecentDocsLengthIs_100");


        }

        //run VIS tests against
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            RunFunctionalTest("Click_RAM Wait_foo Capture_SaveAs CompareTo_SaveAsNormalReference.png");
            RunFunctionalTest("Click_RAM Wait_foo HoverRightSide_SaveAs Wait_foo Capture_SaveAs CompareTo_SaveAsHoverReference.png");
            RunFunctionalTest("Click_RAM Wait_foo ClickRightSide_SaveAs Wait_foo Hover_SaveAs Wait_foo Capture_SaveAs CompareTo_SaveAsCheckedHoverReference.png");
            RunFunctionalTest("Click_RAM Wait_foo ClickRightSide_SaveAs Wait_foo Leave_SaveAs Capture_SaveAs CompareTo_SaveAsCheckedReference.png");
            RunFunctionalTest("Click_RAM Wait_foo PressRightSide_SaveAs Wait_foo Capture_SaveAs CompareTo_SaveAsPressedReference.png");
            RunFunctionalTest("Click_RAM Wait_foo Capture_New CompareTo_NewNormalReference.png");
            RunFunctionalTest("Click_RAM Wait_foo Press_New Wait_foo Capture_New CompareTo_NewPressedReference.png");
            //RunFunctionalTest("Click_RAM Wait_foo Capture_SaveAs SaveCaptureAs_SaveAsNormalReference.png");
            //RunFunctionalTest("Click_RAM Wait_foo HoverRightSide_SaveAs Wait_foo Capture_SaveAs SaveCaptureAs_SaveAsHoverReference.png");
            //RunFunctionalTest("Click_RAM Wait_foo ClickRightSide_SaveAs Wait_foo Hover_SaveAs Wait_foo Capture_SaveAs SaveCaptureAs_SaveAsCheckedHoverReference.png");
            //RunFunctionalTest("Click_RAM Wait_foo ClickRightSide_SaveAs Wait_foo Leave_SaveAs Capture_SaveAs SaveCaptureAs_SaveAsCheckedReference.png");
            //RunFunctionalTest("Click_RAM Wait_foo PressRightSide_SaveAs Wait_foo Capture_SaveAs SaveCaptureAs_SaveAsPressedReference.png");
            //RunFunctionalTest("Click_RAM Wait_foo Capture_New SaveCaptureAs_NewNormalReference.png");
            //RunFunctionalTest("Click_RAM Wait_foo Press_New Wait_foo Capture_New SaveCaptureAs_NewPressedReference.png");
        }

        private void PerformActions(string actions)
        {
            string[] actionList = actions.Split(' ');
            foreach (string action in actionList)
            {
                string[] tokens = action.Split('_');
                PerformAction(tokens[0], tokens[1]);
            }
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //make new window with RAM page in it.
            //display this window
            RAMWindow = new Window();
            RAMPage = new RAMPage1();
            RAMWindow.Content = RAMPage;
            RAMWindow.Show();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {            //make new window with RAM page in it.
            //display this window
            Window NoTextWindow = new Window();
            NoTextPage = new NoTextPage();
            NoTextWindow.Content = NoTextPage;
            NoTextWindow.Show();

        }

        private void RunFunctionalTest(string actions)
        {
            output.Text += "test: [" + actions + "]";
            PerformActions(actions);
            //recycle window
            RAMWindow.Close();
            RAMWindow = null;

            RAMWindow = new Window();
            RAMPage = null;
            RAMPage = new RAMPage1();
            RAMWindow.Content = RAMPage;
            RAMWindow.Show();            
        }

        private void PerformAction(string verb, string noun)
        {
            //first, locate the control
            FrameworkElement targetControl = (FrameworkElement)RAMPage.FindName(noun);
            
            output.Text += verb + "ing " + noun;

            //now, perform the desired action
            switch (verb)
            {
                case "Press":
                    Microsoft.Test.Input.Mouse.MoveTo(CenterOf(targetControl));
                    Microsoft.Test.Input.Mouse.Down(Microsoft.Test.Input.MouseButton.Left);
                    break;
                case "PressRightSide":
                    Microsoft.Test.Input.Mouse.MoveTo(RightSideOf(targetControl));
                    Microsoft.Test.Input.Mouse.Down(Microsoft.Test.Input.MouseButton.Left);
                    break;
                case "Click":
                    Microsoft.Test.Input.Mouse.MoveTo(CenterOf(targetControl));
                    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                    break;
                case "ClickRightSide":
                    Microsoft.Test.Input.Mouse.MoveTo(RightSideOf(targetControl));
                    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                    break;
                case "DoubleClick":
                    Microsoft.Test.Input.Mouse.MoveTo(CenterOf(targetControl));
                    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                    QueueHelper.WaitTillTimeout(TimeSpan.FromMilliseconds(100));
                    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                    break;
                case "ClickBlindly":
                    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                    break;
                case "Hover":
                    Microsoft.Test.Input.Mouse.MoveTo(CenterOf(targetControl));
                    break;
                case "HoverRightSide":
                    Microsoft.Test.Input.Mouse.MoveTo(RightSideOf(targetControl));
                    break;
                case "Leave":
                    Microsoft.Test.Input.Mouse.MoveTo(new System.Drawing.Point(2000,2000));
                    break;
                case "Capture":
                    capture = FromControl(targetControl);
                    break;
                case "CheckResult":
                    CheckResult(noun);
                    break;
                case "SaveCaptureAs":
                    capture.ToFile(noun, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case "CompareTo":
                    CompareTo(noun);
                    break;
                case "Wait":
                    QueueHelper.WaitTillTimeout(TimeSpan.FromMilliseconds(1000));
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;
                case "SetRecentDocsLength":
                    RAMPage.RecentDocsLength = Int32.Parse(noun);
                    break;
                case "VerifyRecentDocsLengthIs":
                    if(RAMPage.RecentDocsLength != Int32.Parse(noun))
                    {
                        throw new Exception("fail");
                    }
                    break;
                case "SetFooterLength":
                    RAMPage.FooterLength = Int32.Parse(noun);
                    break;
                case "VerifyFooterLengthIs":
                    if (RAMPage.FooterLength != Int32.Parse(noun))
                    {
                        throw new Exception("fail");
                    }
                    break;
                case "SetRTL":
                    RAMPage.FlowDirection = System.Windows.FlowDirection.RightToLeft;
                    break;
                case "SetLTR":
                    RAMPage.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                    break;
                case "ClickRTL":
                    ClickElementCenter(targetControl, Microsoft.Test.Input.MouseButton.Left);
                    break;
                case "IsVisible":
                    if (!targetControl.IsVisible)
                    {
                        throw new Exception("fail");
                    }
                    break;

            }
      
        }

        private void CompareTo(string referencePath)
        {
            //requires that we've specified an image to load, and that we've captured an image of a control.
            SnapshotHistogramVerifier verifier = new SnapshotHistogramVerifier(Histogram.FromFile(referenceHistogramPath));
            Snapshot reference = Snapshot.FromFile(referencePath);          
            Snapshot diff = capture.CompareTo(reference);
            VerificationResult result = verifier.Verify(diff);
            if (result != VerificationResult.Pass)
            {
                //this is a failure condition. 
                //first, save out the images and a graph of the histogram
                capture.ToFile("capturedImage.png", System.Drawing.Imaging.ImageFormat.Png);
                diff.ToFile("diffImage.png", System.Drawing.Imaging.ImageFormat.Png);
                Histogram.FromSnapshot(diff).ToGraph("DifferenceHistogram.png", System.Drawing.Imaging.ImageFormat.Png);

                //second, fail on this test.
                output.Text += "image comparison failed : (";
            }
        }

        private void CheckResult(string expectedResult)
        {
            if (RAMPage.LastCalledFunction != expectedResult)
            {
                output.Text += "Failure : ( ";
            }
            else
            {
                output.Text += "Success : ) ";
            }

            output.Text += "expected [" + expectedResult + "] got [" + RAMPage.LastCalledFunction + "]";
        }

        #region helpers

        /// <summary>
        /// Grabs a Snapshot of a control.
        /// </summary>
        /// <param name="target">The control to capture</param>
        /// <returns>a TestAPI Snapshot</returns>
        public static Snapshot FromControl(FrameworkElement target)
        {
            System.Drawing.Size controlSize = new System.Drawing.Size((int)target.ActualWidth, (int)target.ActualHeight);
            System.Windows.Point targetLocationWindowsPoint = (System.Windows.Point)target.PointToScreen(new System.Windows.Point(0, 0));
            System.Drawing.Point targetLocationDrawingPoint = new System.Drawing.Point((int)targetLocationWindowsPoint.X, (int)targetLocationWindowsPoint.Y);
            System.Drawing.Rectangle targetRectangle = new System.Drawing.Rectangle(targetLocationDrawingPoint, controlSize);
            Snapshot capture = Snapshot.FromRectangle(targetRectangle);
            return capture;
        }

        System.Drawing.Point CenterOf(FrameworkElement fe)
        {
            System.Windows.Point topleft = fe.PointToScreen(new System.Windows.Point());
            double wid = fe.ActualWidth;
            double hi = fe.ActualHeight;
            return new System.Drawing.Point((int)(topleft.X + wid / 2), (int)(topleft.Y + hi / 2));
        }
        
        System.Drawing.Point RightSideOf(FrameworkElement fe)
        {
            System.Windows.Point topleft = fe.PointToScreen(new System.Windows.Point());
            double wid = fe.ActualWidth;
            double hi = fe.ActualHeight;
            return new System.Drawing.Point((int)(topleft.X + wid - 3), (int)(topleft.Y + hi - 3));
        }
        #endregion

        //API tests
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            RAMPage.DoAPITests();
        }

        //empty recent docs list
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            RAMPage.RecentDocsLength = 0;
        }

        //put just one entry in the recent list
        private void button7_Click(object sender, RoutedEventArgs e)
        {
            RAMPage.RecentDocsLength = 1;
        }

        //put many entries in the recent docs list
        private void button8_Click(object sender, RoutedEventArgs e)
        {
            RAMPage.RecentDocsLength = 200;
        }

        //template page button
        private void button9_Click(object sender, RoutedEventArgs e)
        {
            Window win = new Window();
            myTemplatePage = new TemplatePage();
            win.Content = myTemplatePage;
            win.Show();
        }

        #region mouse helpers

        public static void ClickElementCenter(FrameworkElement element, Microsoft.Test.Input.MouseButton mouseButton)
        {
        }

        #endregion

        #region private members
        NoTextPage NoTextPage;
        Window RAMWindow;
        RAMPage1 RAMPage;
        Snapshot capture;
        string referenceHistogramPath = "TestAPIDefaultProfile.xml";
        private TemplatePage myTemplatePage;

        #endregion

    }
}
