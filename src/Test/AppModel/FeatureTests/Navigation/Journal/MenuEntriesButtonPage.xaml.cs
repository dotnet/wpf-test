// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class MenuEntriesButtonPage : Page, IProvideCustomContentState
    {
        public struct CurrentButton
        {
            public Brush ButtonColor;
            public String ButtonName;
        }

        private CurrentButton _mySpecialButton;

        public CurrentButton MyButton
        {
            get
            {
                return _mySpecialButton;
            }
            set
            {
                _mySpecialButton = value;
            }
        }

        public bool ChangeColor(Color newColor)
        {
            try
            {
                Brush b = new SolidColorBrush(newColor);
                _mySpecialButton.ButtonColor = b;
                _mySpecialButton.ButtonName = "R=" + newColor.R + ", G=" + newColor.G + ", B=" + newColor.B;

                // Add CurrentUser to the back stack
                ((NavigationWindow)this.Parent).AddBackEntry(null);
                // Change the colour of the button on the page
                mybutton.Background = b;

                return true;
            }
            catch (Exception exp)
            {
                // Print out exception and return false
                mybutton.Content = exp.ToString();
                return false;
            }
        }

        CustomContentState IProvideCustomContentState.GetContentState()
        {
            return new ButtonJournalEntry(this.MyButton.ButtonColor, this.MyButton.ButtonName);
        }
    }

    /// <summary>
    /// Simple JournalEntry class
    /// </summary>
    /// <remarks> This is not a real JournalEntry anymore, just a state object attached to
    /// journal entries created by the framework.
    /// </remarks>
    [Serializable]
    public class ButtonJournalEntry : CustomContentState
    {
        private Brush _color;
        private String _displayName;

        public ButtonJournalEntry(Brush mycolor, String entry)
        {
            _color = mycolor;
            _displayName = entry;
        }

        public override void Replay(NavigationService nav, NavigationMode navMode)
        {
            UIElement root = (UIElement)nav.Content;
            Button button = LogicalTreeHelper.FindLogicalNode(root, "mybutton") as Button;
            button.Background = _color;
        }

        public override string JournalEntryName
        {
            get 
            { 
                return _displayName; 
            }
        }
    }

}
