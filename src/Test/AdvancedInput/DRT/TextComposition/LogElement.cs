// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace DRT
{

    public class LogElement : FrameworkElement //UIElement
    {
        public LogElement()
        {
        }
    
        protected override void OnRender(DrawingContext ctx)
        {
            ctx.DrawRectangle(
                Brushes.LightSkyBlue,
                null,
                new Rect(new Point(), RenderSize));
    
            if (_messages == null)
                return;
    
            Typeface typeface = new Typeface("Verdana");
            Brush pen = Brushes.Black;
    
            string s = null;
            double y = 0;
    
            for(int i = 0; i < _maxmessage; i++)
            {
                s = _messages[i];
    
                if(s != null)
                {
                    FormattedText text = new FormattedText(s, typeface, LINE_HEIGHT, pen);
                    ctx.DrawText(text, new Point(5, y));
                    y += LINE_HEIGHT;
                }
            }
        }
    
        public void ClearMessage()
        {
            for(int i = 0; i < _maxmessage; i++)
            {
                _messages[i] = null;
            }
            InvalidateVisual();
        }
    
    
        public void PushMessage(string msg)
        {
            if (_messages == null)
            {
                _maxmessage = (int)(RenderSize.Height / LINE_HEIGHT) - 1;
                _messages = new string[_maxmessage + 1];
            }
    
            for(int i = 0; i < (_maxmessage -1 ); i++)
            {
                _messages[i] = _messages[i+1];
            }
            _messages[_maxmessage - 1] = msg;
            InvalidateVisual();
    
        }
    
        private const int LINE_HEIGHT = 10;
        private int _maxmessage;
        private string[] _messages;
    }
}
