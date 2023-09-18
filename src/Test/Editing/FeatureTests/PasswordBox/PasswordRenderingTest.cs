// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    using System;
    using System.Drawing;
    using System.Text;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    
    using Test.Uis.Management;
    using Test.Uis.Wrappers;

    [Test(0, "PasswordBox", "PasswordRenderingTest", MethodParameters="/TestCaseType=PasswordRenderingTest")]
    [TestOwner("Microsoft"), TestBugs("683"), TestTactics("440"), TestWorkItem("70")]
    class PasswordRenderingTest : PasswordBoxBase
    {
        public override void Init()
        {
            StackPanel panel;
            TextBox tBox;
            PasswordBox pBox; 
            
            //Create controls
            panel = new StackPanel();
            tBox = new TextBox();
            pBox = new PasswordBox();
           
            //Create wrappers
            _wraper1 = new UIElementWrapper(pBox);
            _wraper2 = new UIElementWrapper(tBox);
            
            //Set size and font            
            pBox.Width = tBox.Width = 300;
            pBox.Height = tBox.Height = 300;
            pBox.FontSize = tBox.FontSize = 200;

            //Add control to panel
            panel.Children.Add(pBox);
            panel.Children.Add(tBox);

            //hook up to the window
            MainWindow.Content = panel; 
        }

        /// <summary>Check the Password does not render the true password.</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "Default Password character test")]
        public void DefaultPasswordCharTest()
        {
            EnterFunction("DefaultPasswordCharTest");
          
            //Set PasswordBox and TextBox values.
            TextBox tbox = _wraper2.Element as TextBox;
            PasswordBox pbox = _wraper1.Element as PasswordBox;
            tbox.FontFamily = pbox.FontFamily ; 
            _wraper1.Text ="0O0";
            _wraper2.Text = new string(new char[] { pbox.PasswordChar, pbox.PasswordChar, pbox.PasswordChar });
            _imageWidth = 40;
            _imageHeight = 40;
            
           

            //Do verification
            QueueDelegate(CompareBigMap);
            
            EndFunction();
        }

        /// <summary>Verify that Changing the passwordChar will trigger the rendering update </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "Change passwordChar test")]
        public void CustomPasswordCharTest()
        {
            EnterFunction("CustomPasswordCharTest");

            //Set PasswordBox and TextBox values.
            PasswordBox pbox = _wraper1.Element as PasswordBox;
            TextBox tbox = _wraper2.Element as TextBox;
           tbox.FontFamily = pbox.FontFamily = new System.Windows.Media.FontFamily("Tohoma") ;
            
            pbox.PasswordChar = 'I';
            _imageWidth = 12;
            _imageHeight = 60;
            _wraper2.Text = "III";

            QueueDelegate(CompareBigMap);

            EndFunction();
        }

        void CompareBigMap()
        {
            Bitmap bpTextBox, bpPasswordBox, bpDifference;
            Rect rect;
            int x, y; 

            rect = _wraper2.GetElementRelativeCharacterRect(_wraper2.SelectionInstance.Start,0, LogicalDirection.Forward);
            
            x = (int)(rect.Right + rect.Left) / 2 - _imageWidth/2; 
            y = (int)(rect.Bottom + rect.Top)/2 -_imageHeight/2; 

            //Get the bitmap from Textbox
            bpTextBox = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(_wraper2.Element);
            bpTextBox = Microsoft.Test.Imaging.BitmapUtils.ColorToBlackWhite(bpTextBox);
            bpTextBox = Microsoft.Test.Imaging.BitmapUtils.CreateSubBitmap(bpTextBox, new Rectangle((int)x, (int)y, _imageWidth, _imageHeight));

            //Get the bitmap from the Passwordbox
            bpPasswordBox = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(_wraper1.Element);
            bpPasswordBox = Microsoft.Test.Imaging.BitmapUtils.ColorToBlackWhite(bpPasswordBox);
            bpPasswordBox = Microsoft.Test.Imaging.BitmapUtils.CreateSubBitmap(bpPasswordBox, new Rectangle((int)x, (int)y, _imageWidth, _imageHeight));
            
            //comparing the bitmap
            if (!Microsoft.Test.Imaging.ComparisonOperationUtils.AreBitmapsEqual(bpPasswordBox, bpTextBox, out bpDifference))
            {
                MyLogger.LogImage(bpPasswordBox, "PasswordBoxImage");
                MyLogger.LogImage(bpTextBox, "TextBoxImage");
                MyLogger.LogImage(bpDifference, "ImageDiffence");
                MyLogger.Log(CurrentFunction + " - Failed: please compare PasswordBoxImage_.png, TextBoxImage_.png!");
                pass = false;
            }

            QueueDelegate(EndTest);
        }
        
        /// <summary>Image width for comparasion</summary>
        int _imageWidth;
        
        /// <summary>Image Height for comparasion</summary>
        int _imageHeight;
    }
}
