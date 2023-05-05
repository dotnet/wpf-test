// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Test BindingOperations Class
	/// </description>
	/// </summary>
    [Test(1, "Binding", "BindingOperationsTest")]
	public class BindingOperationsTest : XamlTest
    {
        private TextBlock _testText;
        private Book _book;
        private double _defaultFontSize;

        public BindingOperationsTest()
            : base(@"blank.xaml")
        {
            InitializeSteps += new TestStep(SetUp);

            RunSteps += new TestStep(BindFourProps);
            RunSteps += new TestStep(VerifyBind);
            RunSteps += new TestStep(GetBindingExpression);
            RunSteps += new TestStep(IsDataBoundException);
            RunSteps += new TestStep(SetBindingNullObjectException);
            RunSteps += new TestStep(SetBindingNullPropertyException);
            RunSteps += new TestStep(SetBindingNullBindingException);
            RunSteps += new TestStep(GetBindingExpressionNullObjectException);
            RunSteps += new TestStep(GetBindingExpressionNullPropertyException);
            RunSteps += new TestStep(ClearBindingNullObjectException);
            RunSteps += new TestStep(ClearBindingNullPropertyException);
            RunSteps += new TestStep(ClearAllBindingsNullObjectException);
            RunSteps += new TestStep(ClearAllBindings);
            RunSteps += new TestStep(ClearAllBindingsVerfiy);
            RunSteps += new TestStep(IsDataBoundNullRef);
        }

        private TestResult SetUp()
        {
            _testText = (TextBlock)Util.FindElement(((Panel)RootElement), "testText");
            if (_testText == null)
            {
                LogComment("Unable to find TextBlock element");
                return TestResult.Fail;
            }
            
            _book = new Book("BookTitle", "BookISBN", "BookAuthor", "BookPublisher", 40.0, Book.BookGenre.Reference);
            _defaultFontSize = _testText.FontSize;

            LogComment("Setup completed successfully");
            return TestResult.Pass;
        }

        private TestResult BindFourProps()
        {
            Binding b = new Binding("Title");
            b.Source = _book;
            BindingOperations.SetBinding(_testText, TextBlock.TextProperty, b);

            b = new Binding("Price");
            b.Source = _book;
            BindingOperations.SetBinding(_testText, TextBlock.HeightProperty, b);

            b = new Binding("Price");
            b.Source = _book;
            BindingOperations.SetBinding(_testText, TextBlock.FontSizeProperty, b);

            b = new Binding("Author");
            b.Source = _book;
            BindingOperations.SetBinding(_testText, TextBlock.TagProperty, b);

            LogComment("Properties are bound");
            return TestResult.Pass;
        }

        private TestResult VerifyBind()
        {
            bool isCorrect = true;

            if (_testText.Text != "BookTitle")
            {
                LogComment("Text was " + _testText.Text + " expected BookTitle");
                isCorrect = false;
            }
            if (_testText.Height != 40.0)
            {
                LogComment("Height was " + _testText.Height.ToString() + " expected 40");
                isCorrect = false;
            }
            if (_testText.FontSize != 40.0)
            {
                LogComment("FontSize was " + _testText.FontSize.ToString() + " expected 40");
                isCorrect = false;
            }
            if (_testText.Tag.ToString() != "BookAuthor")
            {
                LogComment("Text was " + _testText.Tag.ToString() + " expected BookAuthor");
                isCorrect = false;
            }

            if (isCorrect)
            {
                LogComment("Values were correct for VerifyBind");
                return TestResult.Pass;
            }
            else
                return TestResult.Fail;
        }

        TestResult GetBindingExpression()
        {
            BindingExpression be = BindingOperations.GetBindingExpression(_testText, TextBlock.TextProperty);
            if (be == null)
            {
                LogComment("BindingExpression was null, it should not be null");
                return TestResult.Fail;
            }

            Binding b = BindingOperations.GetBinding(_testText, TextBlock.TextProperty);
            if (b == null)
            {
                LogComment("Binding was null, it should not be null");
                return TestResult.Fail;
            }

            Binding nb = BindingOperations.GetBinding(_testText, TextBlock.TextAlignmentProperty);
            if (nb != null)
            {
                LogComment("Binding was not null, it should be null");
                return TestResult.Fail;
            }

            LogComment("GetBinding and GetBinding expressions were correct");

            return TestResult.Pass;
        }

        TestResult IsDataBoundException()
        {
            Status("Attempting to find out if null is data bound");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            bool isBound = BindingOperations.IsDataBound(null, TextBlock.TextAlignmentProperty);

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult SetBindingNullObjectException()
        {
            Status("Attempting to set binding on null");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            BindingOperations.SetBinding(null, TextBlock.TextAlignmentProperty, new Binding("Title"));

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult SetBindingNullPropertyException()
        {
            Status("Attempting to set binding on null property");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            BindingOperations.SetBinding(_testText, null, new Binding("Title"));

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult SetBindingNullBindingException()
        {
            Status("Attempting to set binding on null binding");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            BindingOperations.SetBinding(_testText, TextBlock.TextAlignmentProperty, null);

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult GetBindingExpressionNullObjectException()
        {
            Status("Attempting to get binding expression for null");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            BindingExpression be = BindingOperations.GetBindingExpression(null, TextBlock.TextProperty);
            
            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }


        TestResult GetBindingExpressionNullPropertyException()
        {
            Status("Attempting to get binding expression for null property");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            BindingExpression be = BindingOperations.GetBindingExpression(_testText, null);

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult ClearBindingNullObjectException()
        {
            Status("Attempting to clear binding for null");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            BindingOperations.ClearBinding(null, TextBlock.TextProperty);

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult ClearBindingNullPropertyException()
        {
            Status("Attempting to clear binding for null property");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            BindingOperations.ClearBinding(_testText, null);

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult ClearAllBindingsNullObjectException()
        {
            Status("Attempting to clear all bindings for null");

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            BindingOperations.ClearAllBindings(null);

            LogComment("Expected ArgumentNullException did not occur");
            return TestResult.Fail;
        }

        TestResult ClearAllBindings()
        {
            Status("Attempting to clear all bindings");

            BindingOperations.ClearAllBindings(_testText);
            
            return TestResult.Pass;
        }

        TestResult ClearAllBindingsVerfiy()
        {
            bool isCorrect = true;

            if (_testText.Text != "")
            {
                LogComment("Text was " + _testText.Text + " expected ''");
                isCorrect = false;
            }
            if (!Double.IsNaN(_testText.Height))
            {
                LogComment("Height was " + _testText.Height.ToString() + " expected Double.NaN");
                isCorrect = false;
            }
            if (_testText.FontSize != _defaultFontSize)
            {
                LogComment("FontSize was " + _testText.FontSize.ToString() + " expected " + _defaultFontSize);
                isCorrect = false;
            }
            if (_testText.Tag != null)
            {
                LogComment("Tag was not null, expected null");
                isCorrect = false;
            }

            if (isCorrect)
            {
                LogComment("Values were correct for ClearAllBindingsVerfiy");
                return TestResult.Pass;
            }
            else
                return TestResult.Fail;
        }

        // NullReferenceException calling BindingOperations.IsDataBound
        private TestResult IsDataBoundNullRef()
        {
            // BinidngOperations.IsDataBound with null for the DependencyProperty should throw an ArgumentNullException, not a NullRef
            ExceptionHelper.ExpectException(delegate() { BindingOperations.IsDataBound(RootElement, null); }, new ArgumentNullException("dp"));
            return TestResult.Pass;
        }
    }
}

