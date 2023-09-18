// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Image Copy/Paste tests

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.IO;
    using System.Threading;

    using SysDrawing = System.Drawing;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>This case tests that (only) image copy/paste.</summary>
    [Test(0, "Clipboard", "ImageCopyPasteTest", MethodParameters = "/TestCaseType:ImageCopyPasteTest", Keywords = "MicroSuite")]
    [TestOwner("Microsoft"), TestTactics("61"), TestBugs("388")]
    public class ImageCopyPasteTest : CustomTestCase
    {
        #region Main flow.

        /// <summary>Start the test case</summary>
        public override void RunTestCase()
        {
            _rtb1 = new RichTextBox();
            _rtb2 = new RichTextBox();
            _rtb1.Height = _rtb2.Height = 200d;
            _rtb1.Width = _rtb2.Width = 400d;

            _button1 = new Button();
            _button1.Height = 25d;
            _button1.Width = 400d;

            _panel = new StackPanel();
            _panel.Children.Add(_rtb1);
            _panel.Children.Add(_rtb2);
            _panel.Children.Add(_button1);

            SetUpSourceImageContent();

            //This action is just to make Paste command enabled in the ContextMenu.
            //That way doing {UP 2}{ENTER} is guaranteed to select copy from ContextMenu.
            Clipboard.SetText("abc");

            MainWindow.Content = _panel;
            QueueDelegate(RightClickImage1);
        }

        private void RightClickImage1()
        {
            MouseInput.RightMouseClick(_image1);
            //DelayDelegate since sometimes, contextmenus take a while to come up.
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(2), new SimpleHandler(CopyImage1), null);
        }

        private void CopyImage1()
        {
            //Select Copy from ContextMenu and press Enter
            KeyboardInput.TypeString("{DOWN 2}{ENTER}");
            QueueDelegate(PasteImage1);
        }

        private void PasteImage1()
        {
            _rtb2.Selection.Select(_rtb2.Document.ContentEnd, _rtb2.Document.ContentEnd);
            _rtb2.Paste();
            QueueDelegate(RightClickImage2);
        }

        private void RightClickImage2()
        {
            MouseInput.RightMouseClick(_image2);
            //DelayDelegate since sometimes, contextmenus take a while to come up.
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(2), new SimpleHandler(CopyImage2), null);
        }

        private void CopyImage2()
        {
            //Select Copy from ContextMenu and press Enter
            KeyboardInput.TypeString("{DOWN 2}{ENTER}");
            QueueDelegate(PasteImage2);
        }

        private void PasteImage2()
        {
            _rtb2.Selection.Select(_rtb2.Document.ContentEnd, _rtb2.Document.ContentEnd);
            _rtb2.Paste();
            _button1.Focus();

            //Required for Aero so that you dont get the blue highlight
            //while taking image capture
            MouseInput.MouseMove(0,0);

            // DelayDelegate, to give the context menu time to go away
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(2), new SimpleHandler(VerifyPastedImages), null);
        }

        private void VerifyPastedImages()
        {
            Paragraph para1 = (Paragraph)_rtb2.Document.Blocks.FirstBlock;

            int inlineUIContainerCount = 0;
            InlineUIContainer[] imageContainers = new InlineUIContainer[2];
            Inline[] inlines = new Inline[para1.Inlines.Count];
            para1.Inlines.CopyTo(inlines, 0);

            for (int i = 0; i < inlines.Length; i++)
            {
                if (inlines[i] is InlineUIContainer)
                {
                    if (inlineUIContainerCount < 2)
                    {
                        imageContainers[inlineUIContainerCount] = (InlineUIContainer)inlines[i];
                        inlineUIContainerCount++;
                    }
                    else
                    {
                        Verifier.Verify(false, "Paragraph should not have more than two InlineUIContainers one for each image pasted", true);
                    }
                }
            }

            SysDrawing.Bitmap sourceRTBBmp = BitmapCapture.CreateBitmapFromElement(_rtb1);
            SysDrawing.Bitmap destRTBBmp = BitmapCapture.CreateBitmapFromElement(_rtb2);

            ComparisonCriteria criteria= new ComparisonCriteria();
            criteria.MaxErrorProportion = 0.01f;

            Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(sourceRTBBmp, destRTBBmp, criteria),
                "Comparing Bitmaps after Image Copy/Paste", true);

            Logger.Current.ReportSuccess();
        }

        private void SetUpSourceImageContent()
        {
            _rtb1.Document.Blocks.Clear();

            Paragraph para1 = new Paragraph();
            para1.Inlines.Add(new Run("Test "));

            if ((!File.Exists("colors.png")) || (!File.Exists("XamlpackageImage.PNG")))
            {
                Verifier.Verify(false, "colors.png or XamlpackageImage.png file is missing", false);
            }

            _image1 = new Image();
            _image1.Height = 50;
            _image1.Width = 50;
            BitmapImage bitmapImage1 = new BitmapImage();
            bitmapImage1.BeginInit();
            bitmapImage1.StreamSource = new FileStream("colors.png", FileMode.Open);
            bitmapImage1.EndInit();
            _image1.Source = bitmapImage1;

            _image2 = new Image();
            _image2.Height = 50;
            _image2.Width = 50;
            BitmapImage bitmapImage2 = new BitmapImage();
            bitmapImage2.BeginInit();
            bitmapImage2.StreamSource = new FileStream("XamlpackageImage.png", FileMode.Open);
            bitmapImage2.EndInit();
            _image2.Source = bitmapImage2;

            para1.Inlines.Add(new InlineUIContainer(_image1));
            para1.Inlines.Add(new InlineUIContainer(_image2));

            _rtb1.Document.Blocks.Add(para1);

            _rtb2.Document.Blocks.Clear();
            _rtb2.Document.Blocks.Add(new Paragraph(new Run("Test ")));
        }

        #endregion Main flow

        #region Private fields

        RichTextBox _rtb1,_rtb2;
        Button _button1;
        StackPanel _panel;
        Image _image1,_image2;

        #endregion Private fields
    }
}
