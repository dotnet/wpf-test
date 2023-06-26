// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



using System;
using System.Globalization;
using System.IO;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.IO;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Microsoft.Test.Threading;
using Microsoft.Test.Imaging;
using System.Windows.Threading;
using System.Windows.Markup;

namespace Test.Uis.TextEditing
{
    /// <summary>Tests Regression_Bug101 ... Loading image with corrupt data throws exception </summary>
    [Test(0, "Serialization", "Regression_Bug101", MethodParameters = "/TestCaseType=Regression_Bug101")]
    public class Regression_Bug101 : CustomTestCase
    {
        #region Public Members

        /// <summary>
        /// Entry point to test case
        /// </summary>
        public override void RunTestCase()
        {
            _richtextbox = new RichTextBox();
            TestWindow.Content = _richtextbox;
            DispatcherHelper.DoEvents(1000);
            LoadImage();
        }

        #endregion

        #region Private Members

        private void LoadImage()
        {
            //Path needs to be full path.. if its relative bug doesnt repro
            string imagePath = Environment.CurrentDirectory.ToString() + "\\corruptimage.gif";
            Dispatcher.CurrentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(CurrentDispatcher_UnhandledException);
            _richtextbox.Document = XamlUtils.ParseToObject(s_xamlString.Replace("PATH",imagePath)) as FlowDocument;
            //wait for the document to be loaded... On loading COM exception is thrown in v3.0
            DispatcherHelper.DoEvents(5000);
            Logger.Current.ReportSuccess();
        }

        void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Current.ReportResult(false, e.Exception.ToString());
        }

        #endregion

        #region private Data

        private RichTextBox _richtextbox = null;
        static string s_xamlString =
            "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" >" +
                "<Paragraph>"+
                    "<InlineUIContainer>"+
                        "<Image>"+
	                        "<Image.Source>"+
		                        "<BitmapImage UriSource=\"PATH\"/>"+
	                        "</Image.Source>"+
                        "</Image>"+
                    "</InlineUIContainer>"+
                "</Paragraph>"+
            "</FlowDocument>";

        #endregion
    }
}