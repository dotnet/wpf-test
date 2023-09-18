// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


//                                                                          //
//                                                                          //
//  Tests the fix for "insert before hyperlink" bug                         //
//                                                                          //


namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.IO;
    using System.Reflection;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Logging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.


    [Test(1, "Speller", "SpellerProgress", MethodParameters = "/TestCaseType:SpellerProgress", Versions = "4.5+")]
    public class SpellerProgress : CustomTestCase
    {
        RichTextBox _rtb;

        // Delegate for QueueHelper callbacks.
        private delegate void QueueHelperCallback();

        /// <summary>
        /// Runs the test case.
        /// </summary>
        public override void RunTestCase()
        {
            Log("SpellerProgress::RunTestCase");

            // see if the fix is present - it added a property StartPosition to
            // a nested class within Speller
            Assembly pf = typeof(RichTextBox).Assembly;
            Type scanStatusType = pf.GetType("System.Windows.Documents.Speller+ScanStatus");
            PropertyInfo piStartPosition = (scanStatusType == null) ? null :
                scanStatusType.GetProperty("StartPosition", BindingFlags.Instance | BindingFlags.NonPublic);
            bool isFixPresent = (piStartPosition != null);

            if (isFixPresent)
            {
                // do the actual test
                InitializeWindow();
            }
            else
            {
                // ignore the test, rather than get a fatal execution exception
                Logger.Current.ReportResult(TestResult.Ignore, "**** Ignoring test because the fix is not present.", false);;
            }
        }

        void InitializeWindow()
        {
            // create the RichTextBox
            _rtb = new RichTextBox();
            SpellCheck.SetIsEnabled(_rtb, true);
            _rtb.Height = 30;
            _rtb.Width = 200;

            // display it in the window
            Panel panel = new StackPanel();
            panel.Children.Add(_rtb);
            MainWindow.Content = panel;

            // add a hyperlink into the RTB
            string content =
                "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                    "<Paragraph>" +
                        "<Hyperlink Foreground=\"#FF0066CC\" NavigateUri=\"http://www.microsoft.com\" TextDecorations=\"Underline\">" +
                            "<Run>wordd</Run>" +
                        "</Hyperlink>" +
                    "</Paragraph>" +
                "</Section>";

            TextRange allText = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            using (MemoryStream contentMemStream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(contentMemStream))
                {
                    sw.Write(content);
                    sw.Flush();
                    contentMemStream.Seek(0, SeekOrigin.Begin);
                    allText.Load(contentMemStream, DataFormats.Xaml);
                }
            }

            // give UI time to settle
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(1.5), new QueueHelperCallback(InsertText));
        }

        void InsertText()
        {
            // add some text before the hyperlink
            _rtb.CaretPosition.InsertTextInRun("abcdef");

            // give the spell-checker time to process the text.
            // Prior to the fix, the speller would sometimes get stuck and
            // throw a fatal exception due to lack of progress.
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(1.5), new QueueHelperCallback(FinishTestCase));
        }

        void FinishTestCase()
        {
            // if we made it this far without a fatal exception, pass
            Logger.Current.ReportSuccess();
        }
    }
}
