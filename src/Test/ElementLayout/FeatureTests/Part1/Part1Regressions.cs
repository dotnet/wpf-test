// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using ElementLayout.TestLibrary;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Layout;
using Microsoft.Test.Layout.TestTypes;
using System.Globalization;

namespace ElementLayout.FeatureTests.Part1
{
   /// <summary>
   /// Tests to verify regression issue :: "LayoutInformation.GetExceptionElement should return the element in the tree which threw during a layout operation, not the root of the layout operation."
   /// Verify the UIElement that was being processed by the layout engine at the moment of an unhandled exception. 
   /// </summary>
   [Test(0, "Part1", "VerifyGetLayoutExceptionElement", Variables = "Area=ElementLayout")]
    public class VerifyGetLayoutExceptionElement : CodeTest 
    {
        CustomButton _myButton;
        Grid _myGrid;
        string _elementName;
        FrameworkElement _myFrameworkElement;

        public VerifyGetLayoutExceptionElement()
        { }

        public override void WindowSetup()
        {
            Helpers.Log("Setting up Test Window");
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
           
            Dispatcher.CurrentDispatcher.UnhandledException += this.CustomExceptionHandler;
            this._myButton.Click += this.CustomButtonClick;
         }

        public override FrameworkElement TestContent()
        {
            _myGrid = new Grid();
            _myGrid.Name = "DocumentRoot";
            _myButton = new CustomButton();          
            
            _myButton.Name = "CustomButton";
            _myButton.Width = 100;
            _myButton.Height = 50;
            _myButton.Content = "Click Me";

            _myGrid.Children.Add(_myButton);
            _myFrameworkElement = (FrameworkElement)_myGrid;            
            return _myFrameworkElement;
        }

        public override void TestActions()
        {
            Helpers.Log("Performing a click event on the Button");
            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(_myFrameworkElement, typeof(CustomButton)) as FrameworkElement;
            UserInput.MouseLeftClickCenter(clickElement);
        }
        
        private void CustomExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Helpers.Log("Handling Exception");

            // Verify if the exception thrown is a custom exception
            if (args.Exception.GetType().Name.Equals("MyCustomException"))
            {
                UIElement exceptionElement = LayoutInformation.GetLayoutExceptionElement(Dispatcher.CurrentDispatcher);
                _elementName = (string)exceptionElement.GetValue(FrameworkElement.NameProperty);                
            }
            args.Handled = true;
        }

        private void CustomButtonClick(object sender, RoutedEventArgs args)
        {
            this._myGrid.InvalidateMeasure();
            this._myButton.InvalidateMeasure();
        }

        public override void TestVerify()
        {
            if (_elementName.Equals("CustomButton"))
            {            
                Helpers.Log("Verified Element which caused the exception.");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Expected Element ExceptionElement");
                Helpers.Log(String.Format(CultureInfo.InvariantCulture,"Actual Element {0}", _elementName));
                this.Result = false;
            }
        }


        /// <summary>
        /// Custom Exception 
        /// </summary>
        private class MyCustomException : Exception
        {
            public MyCustomException()
            {
            }
            public MyCustomException(String message)
                : base(message)
            {                
            }
        }

       /// <summary>
       /// Custom Exception Button
       /// </summary>
        private class CustomButton : Button
        {
            private bool _shouldThrow = false;

            protected override Size MeasureOverride(Size availableSize)
            {
                if (_shouldThrow)
                {
                    _shouldThrow = false;
                    throw new MyCustomException("Test Exception!");                    
                }
                else
                {
                    return base.MeasureOverride(availableSize);
                }
            }

            protected override void OnClick()
            {
                this._shouldThrow = true;
                base.OnClick();
            }
        }
    }
       
}
