// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.AddIn;
using System.AddIn.Pipeline;

namespace Microsoft.Test.AddIn
{
    /// <summary>
    /// Addin that contains a WebBrowser control
    /// </summary>
    [AddIn("WebOC", Version = "1.0.0.0")]
    public class WebOCAddInCode : AddInWebOCView
    {
        private Canvas _canvas;
        private WebBrowser _browser;

        public WebOCAddInCode()
        {
            _canvas = new Canvas();
            _canvas.Name = "RootCanvas";
            _canvas.Height = 300;
            _canvas.Width = 300;
            _canvas.Background = new SolidColorBrush(Colors.CornflowerBlue);

            _browser = new WebBrowser();
            _browser.Height = 300;
            _browser.Width = 300;
            _canvas.Children.Add(_browser);

            _browser.NavigateToString("<html><head><title>test page</title></head><body>test page<input type='text' name='text here'/></body></html>");
        }

        public override string Uri
        {
            get
            {
                return (null == _browser.Source) ? "about:blank" : _browser.Source.ToString();
            }
            set
            {
                //
            }
        }

        public override void Initialize(string addInParameters)
        {
            //
        }

        public override FrameworkElement GetAddInUserInterface()
        {
            return _canvas;
        }

    }
}
