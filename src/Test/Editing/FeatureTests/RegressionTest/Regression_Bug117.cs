// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression test Regression_Bug117

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    #endregion Namespaces.

    [TestOwner("Microsoft"), TestTitle("Regression_Bug117 Regression Test")]
    public class Regression_Bug117 : TestTextContainerChangedEventHandlerBase
    {
        protected override void Initialize()
        {
            string strInTextBox = ConfigurationSettings.Current.GetArgument("StringInTextBox");

            base.TextBox.Text = strInTextBox;
            
            base.TextBox.Selection.MoveToPositions(base.TextBox.Selection.TextContainer.Start, base.TextBox.Selection.TextContainer.End);
        }
 
        protected override void DoOperations()
        {   
            int selectionLength = base.TextBox.SelectionLength;

            int selectionStart = base.TextBox.SelectionStart;
        }

        private string _strInTextBox = String.Empty;
    }   
}