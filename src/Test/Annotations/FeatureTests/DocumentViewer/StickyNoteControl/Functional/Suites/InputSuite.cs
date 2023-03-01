// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Diagnostics;
using System.Windows.Input;
using System.Globalization;

namespace Avalon.Test.Annotations.Suites
{
    [OverrideClassTestDimensions]
    [TestDimension("fixed,flow")]
    public class InputSuite : AStickyNoteControlFunctionalSuite
    {
        #region BVT TESTS

        /// <summary>
        /// Enter text/ink into a note.	Content.
        /// </summary>
        [TestDimension("stickynote,inkstickynote")]
        [Priority(0)]
        protected void input1()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.ClickIn();
            if (AnnotationType == AnnotationMode.StickyNote)
            {
                UIAutomationModule.PressKey(Key.H);
                UIAutomationModule.PressKey(Key.Enter);
                UIAutomationModule.PressKey(Key.Space);
                UIAutomationModule.PressKey(Key.A);
                // UIAutomationModule.PressKey(Key.D1);
                // temporary fix.  
                // need to map input to localized keyboards.
                //AssertEquals("Verify text.", "h\r\n a\r\n", CurrentlyAttachedStickyNote.Content);
                AssertStringNotEmptyOrNull("Verify text.", (string)CurrentlyAttachedStickyNote.Content); 
            }
            else
            {
                UIAutomationModule.MoveToCenter(Note);
                UIAutomationModule.LeftMouseDown();
                UIAutomationModule.Move(new Vector(-20, 0));
                UIAutomationModule.Move(new Vector(0, 20));
                UIAutomationModule.Move(new Vector(20, 0));
                UIAutomationModule.Move(new Vector(0, -20));
                UIAutomationModule.LeftMouseUp();
                Assert("Verify ink exists.", CurrentlyAttachedStickyNote.HasContent);
            }

            passTest("Verified simple input.");
        }

        /// <summary>
        /// Note with large amount of content.	
        /// Verify: 
        ///     Text - Vertical scrollbar is visible.
        ///     Ink - Both scrollbars are visible.
        /// </summary>
        [TestDimension("stickynote,inkstickynote")]
        [Priority(0)]
        [DisabledTestCase()]
        protected void input2()
        {
            CreateDefaultNote();

            if (AnnotationType == AnnotationMode.StickyNote)
            {

                RichTextBox parent = (RichTextBox)CurrentlyAttachedStickyNote.InnerControl;
                double viewportWidthBeforeSetting = parent.ViewportWidth;
                double viewportHeightBeforeSetting = parent.ViewportHeight;
                AssertEquals("Initial VerticalScrollbarVisibility.", ScrollBarVisibility.Auto, parent.VerticalScrollBarVisibility);
                AssertEquals("Initial HorizontalScrollbarVisibility.", ScrollBarVisibility.Disabled, parent.HorizontalScrollBarVisibility);

                SetContent(ContentKind.Standard_Large);
                double viewportWidthAfterSetting = parent.ViewportWidth;
                double viewportHeightAfterSetting = parent.ViewportHeight;

                AssertEquals("Final VerticalScrollbarVisibility.", ScrollBarVisibility.Auto, parent.VerticalScrollBarVisibility);
                AssertEquals("Final HorizontalScrollbarVisibility.", ScrollBarVisibility.Disabled, parent.HorizontalScrollBarVisibility);
                Assert("Final ViewportWidth.", viewportWidthBeforeSetting > viewportWidthAfterSetting);
                Assert("Final ViewportHeight.", viewportHeightBeforeSetting == viewportHeightAfterSetting);
            }
            else
            {
                ScrollViewer parent = (ScrollViewer)CurrentlyAttachedStickyNote.InnerControl.Parent;

                AssertEquals("Initial VerticalScrollbarVisibility.", Visibility.Collapsed, parent.ComputedVerticalScrollBarVisibility);
                AssertEquals("Initial HorizontalScrollbarVisibility.", Visibility.Collapsed, parent.ComputedHorizontalScrollBarVisibility);
                SetContent(ContentKind.Standard_Large);
                AssertEquals("Final VerticalScrollbarVisibility.", Visibility.Visible, parent.ComputedVerticalScrollBarVisibility);

                if (AnnotationType == AnnotationMode.StickyNote)
                    AssertEquals("Final HorizontalScrollbarVisibility.", Visibility.Collapsed, parent.ComputedHorizontalScrollBarVisibility);
                else
                    AssertEquals("Final HorizontalScrollbarVisibility.", Visibility.Visible, parent.ComputedHorizontalScrollBarVisibility);
            }
            passTest("Scrollbar visibility verified.");
        }

        /// <summary>
        /// Load note with large image.	
        /// Verify: Scrollbars are visible.
        /// </summary>
        [TestDimension("stickynote")]
        [Priority(0)]
        [DisabledTestCase()]
        protected void input3()
        {
            CreateDefaultNote();
            RichTextBox parent = (RichTextBox)CurrentlyAttachedStickyNote.InnerControl;
            double viewportWidthBeforeSetting = parent.ViewportWidth;
            double viewportHeightBeforeSetting = parent.ViewportHeight;
            SetContent(ContentKind.Image);
            double viewportWidthAfterSetting = parent.ViewportWidth;
            double viewportHeightAfterSetting = parent.ViewportHeight;

            AssertEquals("Final VerticalScrollbarVisibility.", ScrollBarVisibility.Auto, parent.VerticalScrollBarVisibility);


            AssertEquals("Final HorizontalScrollbarVisibility.", ScrollBarVisibility.Disabled, parent.HorizontalScrollBarVisibility);
            Assert("Final ViewportWidth.", viewportWidthBeforeSetting > viewportWidthAfterSetting);
            Assert("Final ViewportHeight.", viewportHeightBeforeSetting == viewportHeightAfterSetting);
            passTest("Scrollbars visibile for large image.");
        }

        /// <summary>
        /// Input content. 
        /// Iconify/restore.
        /// Delete content.
        /// Iconify/restore	Verify content after 2nd restore.
        /// </summary>
        //[TestDimension("stickynote")]
        //[Priority(0)]
        //protected void input21_1()
        //{
        //    input21(true);
        //}

        /// <summary>
        /// Input content. 
        /// Iconify/restore.
        /// Modify content.
        /// Iconify/restore	Verify content after 2nd restore.
        /// </summary>
        //[TestDimension("stickynote")]
        //[Priority(0)]
        //protected void input21_2()
        //{
        //    input21(false);
        //}
        [TestCase_Helper()]
        private void input21(bool deleteContent)
        {
            CreateDefaultNote();
            InsertContent(ContentKind.Standard_Small);
            printStatus("Iconify/Restore 1...");
            IconifyAndRestore();
            VerifyContent(ContentKind.Standard_Small);

            CurrentlyAttachedStickyNote.SelectAll();
            if (deleteContent)
            {
                UIAutomationModule.PressKey(System.Windows.Input.Key.Delete);
                Assert("Verify content was deleted.", !CurrentlyAttachedStickyNote.HasContent);
            }
            else
            {
                InsertContent(ContentKind.Standard_Brief);
                VerifyContent(ContentKind.Standard_Brief);
            }

            object modifiedContent = CurrentlyAttachedStickyNote.Content;

            printStatus("Iconify/Restore 2...");
            IconifyAndRestore();
            if (deleteContent)
                Assert("Verify content still gone.", !CurrentlyAttachedStickyNote.HasContent);
            else
                VerifyContent(ContentKind.Standard_Brief);

            passTest("Verified content properly persisted through iconify/restore.");
        }
        [TestCase_Helper()]
        private void IconifyAndRestore()
        {
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            Assert("Verify note is iconified.", !CurrentlyAttachedStickyNote.Expanded);
            CurrentlyAttachedStickyNote.ClickIn();
            Assert("Verify note is restored.", CurrentlyAttachedStickyNote.Expanded);
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        [TestCase_Cleanup]
        protected override void CleanupVariation()
        {
            if (_stream != null)
                _stream.Flush();

            base.CleanupVariation();

            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
        }

        protected override Stream AnnotationStream
        {
            get
            {
                if (_stream == null)
                {
                    if (CaseNumber.Contains("input20"))
                    {
                        string filename = "input20.annotations.xml";
                        if (CaseNumber.Contains("input20a"))
                            _stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite); // Clobber
                        else
                            _stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite); // Read existing.
                    }
                    else
                        _stream = base.AnnotationStream;
                }
                return _stream;
            }
        }
        private Stream _stream;

        /// <summary>
        /// Select and image and text.
        /// </summary>        
        [Priority(1)]
        protected void input20a_1()
        {
            input20a(new MultiPageSelectionData(0, PagePosition.Beginning, 0, 0, PagePosition.End, 0));
        }
      
        /// <summary>
        /// Select just an image.
        /// </summary>
        [Priority(1)]
        /////////////////////////////////////////////////////////////////////////////////////////
        //DISABLEDUNSTABLETEST: InputSuite.input20a_2/Annotations
        //Input testing in Annotations 
        //to find all disabled tests in the test tree, use:"findstr /snip DISABLEDUNSTABLETEST"
        /////////////////////////////////////////////////////////////////////////////////////////
        [DisabledTestCase()]
        protected void input20a_2()
        {
            if (ContentMode == TestMode.Fixed)
                input20a(new MultiPageSelectionData(0, PagePosition.End, -5, 0, PagePosition.End, -4));
            else
                input20a(new MultiPageSelectionData(0, PagePosition.End, -1, 0, PagePosition.End, 0));
        }

        /// <summary>
        /// Paste an image into StickyNote.
        /// Flush, then close app.
        /// </summary>
        [Priority(1)]
        protected void input20a(ISelectionData selection)
        {                                    
            ViewerBase.Focus();
            SetSelection(selection);
            UIAutomationModule.Ctrl(System.Windows.Input.Key.C);
            DispatcherHelper.DoEvents(20);
            GoToStart(); // Ensure note is in view.

            CreateAnnotation(new SimpleSelectionData(0, 50, 120));
            UIAutomationModule.Ctrl(System.Windows.Input.Key.V);
            DispatcherHelper.DoEvents(20);

            Assert("Verify note has content.", CurrentlyAttachedStickyNote.HasContent);
            AssertEquals("Verify image state.", false, ContentContainsImage());
            passTest("Verified.");
        }

        /// <summary>        
        /// Restart with same store as part a.	Verify image is restored properly.
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        protected void input20b()
        {
            printStatus("Verify that we loaded the store as expected.");
            VerifyNumberOfAttachedAnnotations(1);
            Assert("Verify note has content.", CurrentlyAttachedStickyNote.HasContent);
            AssertEquals("Verify image state.", false, ContentContainsImage());
            passTest("Verified note restored.");
        }

        [TestCase_Helper()]
        [Priority(1)]
        private bool ContentContainsImage()
        {
            RichTextBox rtb = CurrentlyAttachedStickyNote.RichTextBox;
            TextRange content = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            MemoryStream buffer = new MemoryStream();
            content.Save(buffer, DataFormats.Xaml);
            buffer.Seek(0, SeekOrigin.Begin);
            string xaml = new StreamReader(buffer).ReadToEnd();
            buffer.Close();
            return (xaml.Contains("<Image"));
        }

        #endregion PRIORITY TESTS
    }
}	

