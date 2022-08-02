// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace DRT
{

    public class InputElement : FrameworkElement, ITestElement
    {
        public InputElement()
        {
            _textComposition = null;
            Focusable = true;
    
            AddHandler(TextCompositionManager.TextInputStartEvent, new TextCompositionEventHandler(OnTextInputStart), true);
    
            AddHandler(TextCompositionManager.TextInputUpdateEvent, new TextCompositionEventHandler(OnTextInputUpdate), true);
            AddHandler(TextCompositionManager.TextInputEvent, new TextCompositionEventHandler(OnTextInput), true);
        }
    
        protected override void OnRender(DrawingContext ctx)
        {
            ctx.DrawRectangle(
                Brushes.LightSkyBlue,
                null,
                new Rect(new Point(), RenderSize));
        }
    
    
        protected void OnTextInputStart(object sender, TextCompositionEventArgs e)
        {
            _textComposition = e.TextComposition;
        }

        protected void OnTextInputUpdate(object sender, TextCompositionEventArgs e)
        {
            _textComposition = e.TextComposition;
        }
    
        protected void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            _textComposition = null;
        }
    
    
        public TextComposition TextComposition
        {
            get {return _textComposition;}
        }
    
        private TextComposition _textComposition;
    }
}
