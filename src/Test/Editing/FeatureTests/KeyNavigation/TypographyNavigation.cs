// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Text navigation with different typography features.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/KeyNavigation/TypographyNavigation.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Runs navigation test cases with different typography features.
    /// </summary>
    /// <remarks>
    /// The following execution modes are expected:
    /// Pri-0: EditingTest.exe /TestCaseType=BasicTypographyNavigationTest (~10 seconds)
    /// </remarks>
    [Test(2, "Editor", "BasicTypographyNavigationTest", MethodParameters = "/TestCaseType=BasicTypographyNavigationTest", Timeout=250)]
    [TestOwner("Microsoft"), TestWorkItem("123"), TestTactics("359"), TestBugs("510")]
    public class BasicTypographyNavigationTest: CustomCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            string[] fontFamilyNames;
            object[] fontSizes;

            fontFamilyNames = new string[] {
                "Tahoma", "Courier New", "Arial", "Microsoft Sans Serif", "Times New Roman",
                "Lucida Console", "Lucida Sans", "Verdana", "Comic Sans MS"
            };
            fontSizes = new object[] {
                (double)8, (double)9, (double)9.5, (double)10, (double)12, (double)32, (double)76
            };

            return new Dimension[] {
                new Dimension("FontFamilyName", fontFamilyNames),
                new Dimension("FontSize", fontSizes),
            };
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _fontFamilyName = (string)values["FontFamilyName"];
            _fontSize = (double)values["FontSize"];

            return true;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            TextBox control;

            control = new TextBox();
            _wrapper = new UIElementWrapper(control);
            TestElement = control;
            control.FontSize = _fontSize;
            control.FontFamily = new FontFamily(_fontFamilyName);
            QueueDelegate(PrepareControl);
        }

        private void PrepareControl()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString(" ");
            QueueDelegate(DoBackspace);
        }

        private void DoBackspace()
        {
            KeyboardInput.TypeString("{BACKSPACE}");
            QueueDelegate(TypeContent);
        }

        private void TypeContent()
        {
            KeyboardInput.TypeString("sample 1+1");
            QueueDelegate(Done);
        }

        private void Done()
        {            
            QueueDelegate(this.NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        private UIElementWrapper _wrapper;

        private string _fontFamilyName;
        private double _fontSize;

        #endregion Private fields.
    }
}
