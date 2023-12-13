// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Test.Uis.TextEditing
{
    #region Namespaces.
    using System;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Microsoft.Test.Imaging;
    using Test.Uis.Management;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Markup;
    using System.Collections;
    #endregion Namespaces.

    /// <summary>
    /// this class contains code for testing the passwordbox.
    ///</summary>
     [TestOwner("Microsoft"), TestBugs("444, Regression_Bug112"), TestTactics("429, 429")]
    public class PasswordBoxTestCases : PasswordBoxBase
    {
        #region Regression case - Regression_Bug444: no excption should be thrown when apply bold on PasswordBox
        /// <summary>Regression_Bug444 no excption should be thrown when apply bold on PasswordBox</summary>
        public void Regression_Bug444_CaseStart()
        {
            EnterFuction("Regression_Bug444_CaseStart");
            Sleep();
            MouseInput.MouseClick(_wraper1.Element);
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Regression_Bug444_MakeSelection));
        }

        /// <summary>Regression_Bug444_End</summary>
        void Regression_Bug444_MakeSelection()
        {
            EnterFuction("Regression_Bug444_MakeSelection");
            Sleep();
            KeyboardInput.TypeString("abc+{LEFT 2}^b^i^u");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Regression_Bug444_End));
        }
        
        /// <summary>Regression_Bug444_End</summary>
        void Regression_Bug444_End()
        {
            EnterFuction("Regression_Bug444_End");
            Sleep();
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(EndTest));
        }

        #endregion 
        
        #region Regression case - Regression_Bug112
        /// <summary>we make sure that there is not scroll bar.</summary>
        public void Regression_Bug112_CaseStart()
        {
            EnterFuction("Regression_Bug112_CaseStart");
            //PasswordBox1 & passwordbox2 are the same size, 9 characters will make it full. 
            //if more than 9 character are in the box, we don't expected the scroll bar is shown.
            //So we compare that the images are still the same.
            ((PasswordBox)_wraper1.Element).Password = "cdefghimj";
            ((PasswordBox)_wraper2.Element).Password = "xyzdefkijedis";
            _wraper3.Element.Focus();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Regression_Bug112_CompareImg));
            EndFunction();
        }
        
         /// <summary> </summary>
        void Regression_Bug112_CompareImg()
        {
            EnterFuction("Regression_Bug112_CompareImg");
            Sleep();
            bool b1;
            System.Drawing.Bitmap Bmp1, Bmp2, Bmp3;
            Bmp1 = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(_wraper1.Element);
            Bmp2 = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(_wraper2.Element);

            b1 = Microsoft.Test.Imaging.ComparisonOperationUtils.AreBitmapsEqual(Bmp1, Bmp2, out Bmp3);

            if (!b1)
            {
                MyLogger.LogImage(Bmp1, "Passwordbox1Img");
                MyLogger.LogImage(Bmp2, "Passwordbox2Img");
                MyLogger.LogImage(Bmp3, "DiffP1AndP2Img");
            }
            Verifier.Verify(b1, CurrentFunction + "- Failed: image comparing failed!!! Please see: Passwordbox1Img, Passwordbox2Img, DiffP1AndP2Img");
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(EndTest));
            EndFunction();
        }
        #endregion
        
        #region Regression case - Regression_Bug111
        /// <summary>Regression_Bug111</summary>
        public void Regression_Bug111_CaseStart()
        {
            EnterFunction("Regression_Bug111_CaseStart");
            MouseInput.MouseClick(_wraper1.Element);
            EndFunction();
            QueueDelegate(Regression_Bug111_DoActions);
        }

        void Regression_Bug111_DoActions()
        {
            EnterFunction("Regression_Bug111_DoActions");
            KeyboardInput.TypeString("something is junck{home}+{Right 50}+{LEFT}^x^z");
            EndFunction();
            QueueDelegate(Regression_Bug111_Done);
        }

        void Regression_Bug111_Done()
        {
            //if no crash, the case pass.
            EnterFunction("Regression_Bug111_Done");
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion
        
        #region Regression case - Regression_Bug110
        /// <summary>
        /// Regression_Bug110
        /// </summary>
        public void Regression_Bug110_CaseStart()
        {
            EnterFunction("Regression_Bug110_CaseStart");
            _wraper1 = new Test.Uis.Wrappers.UIElementWrapper(new PasswordBox());
            ListBox listBox = new ListBox();
            Panel panel = new Canvas();

            panel.Background = Brushes.Violet;
            MainWindow.Content = panel;
            ((IAddChild)panel).AddChild(_wraper1.Element);
            ((IAddChild)panel).AddChild(listBox);
            ((PasswordBox)(_wraper1.Element)).Width = 250;
            ((PasswordBox)(_wraper1.Element)).Height = 70;
            listBox.Width = 150;
            listBox.Height = 100;
            listBox.Items.Add(new ListBoxItem());
            listBox.Items.Add(new ListBoxItem());

            Canvas.SetLeft(_wraper1.Element, 200);
            Canvas.SetTop(_wraper1.Element, 200);
            Canvas.SetLeft(listBox, 20);
            Canvas.SetTop(listBox, 180);
            ((PasswordBox)(_wraper1.Element)).FontSize = 50;
            _wraper1 = new Test.Uis.Wrappers.UIElementWrapper(_wraper1.Element);
            QueueDelegate(Regression_Bug110_MouseClick);
        }
        
        void Regression_Bug110_MouseClick()
        {
            MouseInput.MouseClick(_wraper1.Element);
            QueueDelegate(Regression_Bug110_Done);
        }
        
        void Regression_Bug110_Done()
        {
            System.Drawing.Bitmap elementBitmap;    // Colored image.
            System.Drawing.Bitmap bw;               // Black and white image.
            double caretWidth;                      // Expected width of caret.
            System.Drawing.Bitmap img;

            caretWidth = SystemParameters.CaretWidth;

            using (elementBitmap = BitmapCapture.CreateBitmapFromElement(_wraper1.Element))
            using (img = BitmapUtils.CreateBorderlessBitmap(elementBitmap, 2))
            using (bw = BitmapUtils.ColorToBlackWhite(img))
            {
                System.Drawing.Rectangle rectangle;
                pass = BitmapUtils.GetTextCaret(bw, out rectangle);
                if (!pass)
                    MyLogger.LogImage(bw, "Regression_Bug110_img");
            }
            base.Init();
            QueueDelegate(EndTest);
        }
    #endregion
    }
}
