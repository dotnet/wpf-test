// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Input;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Event
{ /// <summary>
    /// Parser Test
    /// </summary>
    /// <remarks>
    /// This is parser BVT test that parse XAML for Event testing.
    /// Test cases are:
    ///         - LoadXml with Event
    /// </remarks>
    public class AttachedRoutedEventTest
    {
        /// <summary>
        /// Check the exceptions thrown during illegel operations.
        /// </summary>
        public void RunTest()
        {
            BasicFunction();
        }

        void BasicFunction()
        {
            //Create element and EventArgs
            UIElement uie = new UIElement(); 
            MouseEventArgs args = new MouseEventArgs(InputManager.Current.PrimaryMouseDevice, 0);
            args.RoutedEvent= Mouse.MouseMoveEvent;

            //add a handler.
            _mouseMoveInvokedCount = 0;
            Mouse.AddMouseMoveHandler(uie, new MouseEventHandler(OnMouseMove));
            uie.RaiseEvent(args);
            if (1 != _mouseMoveInvokedCount)
                throw new Microsoft.Test.TestValidationException("EventHandler should be called 1 time, actual: " + _mouseMoveInvokedCount.ToString() + ".");
            
            //add another hander.
            _mouseMoveInvokedCount = 0;
            Mouse.AddMouseMoveHandler(uie, new MouseEventHandler(OnMouseMove));
            uie.RaiseEvent(args);
            if (2 != _mouseMoveInvokedCount)
                throw new Microsoft.Test.TestValidationException("EventHandler should be called 2 time, actual: " + _mouseMoveInvokedCount.ToString() + ".");

            //remove on handler
            _mouseMoveInvokedCount = 0;
            Mouse.RemoveMouseMoveHandler(uie, new MouseEventHandler(OnMouseMove));
            uie.RaiseEvent(args);
            if (1 != _mouseMoveInvokedCount)
                throw new Microsoft.Test.TestValidationException("EventHandler should be called 1 time, actual: " + _mouseMoveInvokedCount.ToString() + ".");

            //remove the last handler.
            _mouseMoveInvokedCount = 0;
            Mouse.RemoveMouseMoveHandler(uie, new MouseEventHandler(OnMouseMove));
            uie.RaiseEvent(args);
            if (0 != _mouseMoveInvokedCount)
                throw new Microsoft.Test.TestValidationException("EventHandler should not be called, actual times: " + _mouseMoveInvokedCount.ToString() + ".");

            //remove a handler that is not there.
            _mouseMoveInvokedCount = 0;
            Mouse.RemoveMouseMoveHandler(uie, new MouseEventHandler(OnMouseMove));
            uie.RaiseEvent(args);
            if (0 != _mouseMoveInvokedCount)
                throw new Microsoft.Test.TestValidationException("EventHandler should not be called, actual times: " + _mouseMoveInvokedCount.ToString() + ".");
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            _mouseMoveInvokedCount++;
        }

        int _mouseMoveInvokedCount = 0;
    }
}
