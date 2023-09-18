// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 21 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/ExeTarget/EntryPoint.cs $")]

namespace Test.Uis.TextEditing
{

    #region Namespaces.

    using System;
    using System.Collections;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows;
    using System.Windows.Documents;
    using Test.Uis.Wrappers;
    using Test.Uis.Data;
    using System.Windows.Controls.Primitives;

    #endregion Namespaces.

    /// <summary>This class test the Page up down in TextEditable Control.</summary>
    [TestOwner("Microsoft"), TestBugs(""), TestTactics("358"), TestWorkItem("38")]
    public class PageUpDownNavigation : ManagedCombinatorialTestCase
    {
        /// <summary>
        /// Override the base class
        /// </summary>
        protected override void DoRunCombination()
        {
            TestElement = _control.CreateInstance();
            _textControlWrapper = new UIElementWrapper(TestElement);
            TestElement.Height = 70;
            if (!(TestElement is TextBoxBase) || TestElement == null)
            {
                QueueDelegate(NextCombination);
                return;
            }
            if (_caretAtLine > _totalLines)
            {
                QueueDelegate(NextCombination);
                return;
            }

            if (TestElement is TextBox)
            {
                ((TextBox)TestElement).AcceptsReturn = true;
            }

            ((TextBoxBase)TestElement).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            ((TextBoxBase)TestElement).Height = 70;

            //Set the flowDirection
            ((Control)TestElement).FlowDirection = this._flowDirection;
            QueueDelegate(PrepareForPageAction);
        }

        void PrepareForPageAction()
        {
            string typedString=string.Empty ;
            for (int i = 1; i < _totalLines; i++)
            {
                typedString += "Test" + i + "{ENTER}";
            }
            typedString += "Test^{HOME}{Down " + (_caretAtLine -1) + "}";

            MouseInput.MouseClick(TestElement);
            KeyboardInput.TypeString(typedString);
            QueueDelegate(PerfromPageAction);
        }

        void PerfromPageAction()
        {
            _state = _pageAction.CaptureBeforeEditing(_textControlWrapper);
            _pageAction.PerformAction(_textControlWrapper, AfterPerformAction);
        }

        private void AfterPerformAction()
        {
            _pageAction.VerifyEditing(_state);
            QueueDelegate(NextCombination);

        }

        private KeyboardEditingState _state;

        /// <summary>Wrapper for text Controls</summary>
        private UIElementWrapper _textControlWrapper;

        /// <summary>Text Control created from TextEditableType</summary>
        private TextEditableType _control=null;

        /// <summary>page actions including pageUp, Down (with or without ctrl key pressed)</summary>
        private KeyboardEditingData _pageAction=null;

        /// <summary>Total Lines in the Test control</summary>
        private int _totalLines=0;

        /// <summary>line number on which the caret is at before the page navigation</summary>
        private int _caretAtLine=0;

        /// <summary>Flow direction for the text control</summary>
        private FlowDirection _flowDirection=System.Windows.FlowDirection.LeftToRight;
    }
}
