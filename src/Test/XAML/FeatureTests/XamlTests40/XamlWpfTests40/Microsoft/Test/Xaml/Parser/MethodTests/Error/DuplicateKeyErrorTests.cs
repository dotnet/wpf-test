// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xaml;
using System.Xaml.Schema;
using System.Xml;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Error
{
    /******************************************************************************
    * Class:          DuplicateKeyErrorTests
    ******************************************************************************/

    /// <summary>
    /// Duplicate x:Key tests (Regression test)
    /// </summary>
    public class DuplicateKeyErrorTests
    {
        /// <summary>
        /// The line number expected from the Exception.
        /// </summary>
        private int _expLineNumber = 0;

        /// <summary>
        /// The line position expected from the Exception.
        /// </summary>
        private int _expLinePosition = 0;
        
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Verifies Xaml Exceptions.
        /// </summary>
        public void Run()
        {
            GlobalLog.LogEvidence("TEST #1: Verify duplicate keys as property elements.");
            VerifyDuplicatePropertyElement();

            GlobalLog.LogEvidence("TEST #2: Verify duplicate keys, both property and property element.");
            VerifyDuplicateMixed();

            GlobalLog.LogEvidence("TEST #3: Verify duplicate keys as property elements in a ResourceDictionary.");
            VerifyDuplicatePropertyElementInRD();

            GlobalLog.LogEvidence("TEST #4: Verify duplicate keys, property and property element, in a ResourceDictionary.");
            VerifyMixedInRD();

            GlobalLog.LogEvidence("TEST #5: Verify duplicate keys when nested.");
            VerifyNested();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          TEST 1: VerifyDuplicatePropertyElement
        ******************************************************************************/

        /// <summary>
        ///  Verifies duplicate keys, each set as a property element.
        /// </summary>
        private void VerifyDuplicatePropertyElement()
        {
            string xamlString = @"
            <Page xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                <Page.Resources>
                    <ComboBox>
                        <x:Key>Value1</x:Key>
                        <x:Key>Value2</x:Key>
                    </ComboBox>
                </Page.Resources>
            </Page>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            _expLineNumber = 7;
            _expLinePosition = 26;
            ExceptionHelper.ExpectException<System.Windows.Markup.XamlParseException>(delegate { System.Windows.Markup.XamlReader.Load(xmlReader); }, ValidateException);
        }

        /******************************************************************************
        * Function:          TEST 2: VerifyDuplicateMixed
        ******************************************************************************/

        /// <summary>
        ///  Verifies duplicate keys, one set as a property, another as a property element.
        /// </summary>
        private void VerifyDuplicateMixed()
        {
            string xamlString = @"
            <Page xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                <Page.Resources>
                    <TextBox x:Key='Value1'>
                        <x:Key>Value2</x:Key>
                    </TextBox>
                </Page.Resources>
            </Page>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            _expLineNumber = 6;
            _expLinePosition = 26;
            ExceptionHelper.ExpectException<System.Windows.Markup.XamlParseException>(delegate { System.Windows.Markup.XamlReader.Load(xmlReader); }, ValidateException);
        }

        /******************************************************************************
        * Function:          TEST 3: VerifyDuplicatePropertyElementInRD
        ******************************************************************************/

        /// <summary>
        ///  Verifies duplicate keys as property elements in a ResourceDictionary.
        /// </summary>
        private void VerifyDuplicatePropertyElementInRD()
        {
            string xamlString = @"
            <StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                <StackPanel.Resources>
                    <ResourceDictionary>
                        <Color>
                            <x:Key>Value1</x:Key>
                            <x:Key>Value2</x:Key>
                            Blue
                        </Color>
                    </ResourceDictionary>
                </StackPanel.Resources>
            </StackPanel>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            _expLineNumber = 8;
            _expLinePosition = 30;
            ExceptionHelper.ExpectException<System.Windows.Markup.XamlParseException>(delegate { System.Windows.Markup.XamlReader.Load(xmlReader); }, ValidateException);
        }

        /******************************************************************************
        * Function:          TEST 4: VerifyMixedInRD
        ******************************************************************************/

        /// <summary>
        ///  Verify duplicate keys, property and property element, in a ResourceDictionary.
        /// </summary>
        private void VerifyMixedInRD()
        {
            string xamlString = @"
            <StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                <StackPanel.Resources>
                    <ResourceDictionary>
                        <Color x:Key='Value1'>
                            Blue
                            <x:Key>Value2</x:Key>
                        </Color>
                    </ResourceDictionary>
                </StackPanel.Resources>
            </StackPanel>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            _expLineNumber = 8;
            _expLinePosition = 30;
            ExceptionHelper.ExpectException<System.Windows.Markup.XamlParseException>(delegate { System.Windows.Markup.XamlReader.Load(xmlReader); }, ValidateException);
        }

        /******************************************************************************
        * Function:          TEST 5: VerifyNested
        ******************************************************************************/

        /// <summary>
        ///  Verify duplicate keys when nested.
        /// </summary>
        private void VerifyNested()
        {
            string xamlString = @"
            <Window xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                <Window.Resources>
                    <SolidColorBrush x:Key='Value1'>
                        <x:Key><x:Int32><x:Key>1</x:Key><x:Key>2</x:Key></x:Int32></x:Key>
                    </SolidColorBrush>
                </Window.Resources>
            </Window>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            _expLineNumber = 6;
            _expLinePosition = 26;
            ExceptionHelper.ExpectException<System.Windows.Markup.XamlParseException>(delegate { System.Windows.Markup.XamlReader.Load(xmlReader); }, ValidateException);
        }

        /// <summary>
        /// Validates the exception thrown
        /// </summary>
        /// <param name="exception">exception to validate</param>
        private void ValidateException(System.Windows.Markup.XamlParseException exception)
        {
            string expected = string.Format(Exceptions.GetMessage("LineNumberAndPosition", WpfBinaries.SystemXaml), Exceptions.GetMessage("DuplicateMemberSet", WpfBinaries.SystemXaml), _expLineNumber, _expLinePosition);
            if (!Exceptions.CompareMessage(exception.Message, expected))
            {
                throw new TestValidationException("Expected Message: " + expected + ", Actual Message: " + exception.Message);
            }
        }
    }
}
