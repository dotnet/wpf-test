// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for the CommandLink class.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    
    using System.Windows;
    using System.Windows.Controls;

    using System.Windows.Media;
    using System.Windows.Input;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// A RoutedCommand type for testing purposes.
    /// </summary>
    public class TestCommand: RoutedCommand
    {
        #region Constructors.

        /// <summary>
        /// Creates a new TestCommand instance with default values.
        /// </summary>
        public TestCommand(Type ownerType): base("Test Command", new CommandLink[] { }, ownerType)
        {
        }

        #endregion Constructors.
    }

    /// <summary>Verifies that a CommandLink can be created.</summary>
    [TestOwner("Microsoft"),TestTactics("65")]
    public class CommandLinkCreation: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            VerifyInvalidCalls();
            VerifyValidCalls();

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalidCalls()
        {
            // There is currently no way to force an exception here.
        }

        private void VerifyValidCalls()
        {
            CommandLink link;

            Log("Creating a simple command link...");
            link = new CommandLink();
            Verifier.Verify(link.Command == null,
                "Command on link is unassigned", true);

            Log("Creating a command link with a command...");
            RoutedCommand command = new TestCommand(typeof(CommandLinkCreation));
            link = new CommandLink(command);
            Verifier.Verify(link.Command == command,
                "Command on link is assigned", true);
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that a CommandLink cannot be modified after being sealed.
    /// </summary>
    [TestOwner("Microsoft"),TestTactics("66"), TestBugs("391")]
    public class CommandLinkSeal: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            VerifySealing();

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifySealing()
        {
            object gesture = "gesture placeholder";

            Log("Setting up command link...");
            CommandLink link = new CommandLink();
            RoutedCommand command = new TestCommand(typeof(CommandLinkSeal));
            link.Command = command;
            link.Enabled = true;
            link.Checked = true;
            link.KeyBinding = new KeyBinding((ModifierKeys) 0, Key.A);
            link.MouseBinding = new MouseBinding(MouseAction.LeftClick);
            link.PenBinding = new PenBinding(gesture);

            Verifier.Verify(!link.IsSealed, "Link is not sealed", true);

            Log("Sealing link...");
            link.Seal();
            Verifier.Verify(link.IsSealed, "Link is sealed", true);

            Log("Verifying that properties cannot be modified " +
                "after sealing...");
            try {
                link.Command = null;
                ThrowModifiable("Command");
            } catch(SystemException) {
                LogUnmodifiable("Command");
            }
            try {
                link.Enabled = false;
                ThrowModifiable("Enabled");
            } catch(SystemException) {
                LogUnmodifiable("Enabled");
            }
            try {
                link.Checked = false;
                ThrowModifiable("Checked");
            } catch(SystemException) {
                LogUnmodifiable("Checked");
            }
            try {
                link.KeyBinding = null;
                ThrowModifiable("KeyBinding");
            } catch(SystemException) {
                LogUnmodifiable("KeyBinding");
            }
            try {
                link.MouseBinding = null;
                ThrowModifiable("MouseBinding");
            } catch(SystemException) {
                LogUnmodifiable("MouseBinding");
            }
            try {
                link.PenBinding = null;
                ThrowModifiable("PenBinding");
            } catch(SystemException) {
                LogUnmodifiable("PenBinding");
            }
        }

        private void ThrowModifiable(string property)
        {
            throw new ApplicationException(
                "Property is modifiable: " + property);
        }

        private void LogUnmodifiable(string property)
        {
            Log("Property cannot be modified: " + property);
        }

        #endregion Verifications.
    }
    
    /// <summary>
    /// Verifies that removing a CommandLink prevents it from firing.
    /// </summary>
    [TestOwner("Microsoft"),TestTactics("67"), TestBugs("392")]
    public class CommandLinkRemove: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Creating a new TextBox...");
            this._textbox = new TextBox();
            MainWindow.Content = _textbox;
            
            Log("Removing CutCommand from TextBox...");
            int linksFound = 0;
            IList links = (IList)_textbox.CommandLinks;
            for (int i = links.Count - 1; i >= 0; i--)
            {
                CommandLink link = (CommandLink)links[i];
                if (link.Command == StandardCommands.CutCommand)
                {
                    linksFound++;
                    links.RemoveAt(i);
                }
            }
            Log("Links with CutCommand found: " + linksFound);
            if (linksFound == 0)
            {
                throw new Exception("Unable to find CommandLink with CutCommand.");
            }

            QueueHelper.Current.QueueDelegate(ClickTypeAndCut);
        }

        private void ClickTypeAndCut()
        {
            Log("Clicking on TextBox, typing, selecting and cutting...");
            MouseInput.MouseClick(_textbox);
            KeyboardInput.TypeString(" +{LEFT}^x");
            QueueHelper.Current.QueueDelegate(CheckText);
        }
        
        private void CheckText()
        {
            Log("Expecting a single space (because cut will have been disabled).");
            Log("Text: [" + _textbox.Text + "]");
            Verifier.Verify(_textbox.Text == " ", "Text is a single space", true);
            Logger.Current.ReportSuccess();
        }
        
        #endregion Main flow.

        #region Private data.
        
        private TextBox _textbox;
        
        #endregion Private data.
    }
}
