// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//   Creates Text Box for logging

// avalon
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RtfXamlView
{
    class LogPanel : TextBox
    {
        public LogPanel()
        {
            // Set the log panel (text box)
            this.Background = Brushes.LightGray;
            this.Margin = new Thickness(2);
            this.FontFamily = new FontFamily("Tahoma");
            this.FontSize = 11.0;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            //this.TextWrapping = TextWrapping.Wrap;
            this.MaxHeight = 20.0;
            this.MaxWidth = 895.0;
            this.IsEnabled = true;
            _fNoMsgs = false;
        }
       public void DisableMsgs(bool fOff)
       {
          _fNoMsgs = fOff;
       }
        public void LogInfo(string logString)
        {
           if (!_fNoMsgs)
           {
              this.Foreground = Brushes.DarkBlue;
              this.Text = logString;
           }
        }

        public void LogError(string logString)
        {
           if (!_fNoMsgs)
           {
              this.Foreground = Brushes.DarkRed;
              this.Text = "Error: " + logString;
           }
        }

        public void Reset()
        {
            this.Clear();
        }
       bool _fNoMsgs;
    }
}
