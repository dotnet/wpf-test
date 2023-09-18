// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for TextBox with embedded UIElements. 

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.
    
    /// <summary>
    /// Verifies that UIElements do not have their layout permanently
    /// modified when they are clicked.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("520"), TestBugs("725,726")]
    [Test(3, "TextBox", "TextBoxUIElementRender", MethodParameters = "/TestCaseType=TextBoxUIElementRender")]
    public class TextBoxUIElementRender: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _richTextBox = new RichTextBox();
            new UIElementWrapper(_richTextBox).XamlText = "Some text <Button>button</Button> more text";
            MainWindow.Content = _richTextBox;
            QueueDelegate(AfterLayout);
        }
        
        private void AfterLayout()
        {
            Log("Looking for the first embedded element...");
            _embeddedElement = TextUtils.FindElementInText(_richTextBox.Document.ContentStart, null);
            if (_embeddedElement == null)
            {
                throw new InvalidOperationException(
                    "There are no objects in the tested control.");
            }

            Log("Capturing element before being clicked...");
            _beforeClick = BitmapCapture.CreateBitmapFromElement(_embeddedElement);

            Log("Clicking on the element...");
            MouseInput.MouseClick(_embeddedElement);

            Log("Clicking outside the element...");
            Rect r = ElementUtils.GetScreenRelativeRect(_embeddedElement);
            Log("Screen pos:" + r);
            MouseInput.MouseMove(
                (int)Math.Round(r.Left + _embeddedElement.RenderSize.Width + 32),
                (int)Math.Round(r.Top + _embeddedElement.RenderSize.Height / 2));

            QueueHelper.Current.QueueDelayedDelegate(
                TimeSpan.FromMilliseconds(2000),
                new SimpleHandler(CheckBitmap), new object[0]);
        }

        private void CheckBitmap()
        {
            Log("Capturing element after being clicked...");
            Bitmap afterClick =
                BitmapCapture.CreateBitmapFromElement(_embeddedElement);
            Rect r = ElementUtils.GetScreenRelativeRect(_embeddedElement);
            Log("Screen pos:" + r);

            ComparisonOperation op = new ComparisonOperation();
            op.MasterImage = _beforeClick;
            op.SampleImage = afterClick;

            ComparisonCriteria criteria = new ComparisonCriteria();
            criteria.MaxColorDistance = 0.1F;
            criteria.MaxErrorProportion = 0.01F;
            criteria.MaxPixelDistance = 1;
            criteria.MismatchDescription = "Images are same";
            //after clicking an embedded disabled element there is a selection highlight
            //so the resultant image will differ from what was at the beginning
            ComparisonResult result = op.Execute();
            if (result.CriteriaMet)
            {
                Logger.Current.LogImage(_beforeClick, "before");
                Logger.Current.LogImage(afterClick, "after");
                result.HighlightDifferences(afterClick);
                Logger.Current.LogImage(afterClick, "after-differences");
                throw new Exception("Bitmaps before and after clicking are same.");
            }

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private RichTextBox _richTextBox;
        private UIElement _embeddedElement;
        private Bitmap _beforeClick;

        #endregion Private fields.
    }

    /// <summary>Unimplemented test case.</summary>
    [TestOwner("Microsoft"), TestTactics("123")]
    public class TextBoxUIElementCaret: TextBoxTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
        }
    }

    /// <summary>Unimplemented test case.</summary>
    [TestOwner("Microsoft"), TestTactics("123")]
    public class TextBoxUIElementMouse: TextBoxTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
        }
    }
}
