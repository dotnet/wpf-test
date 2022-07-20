// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Reflection;

namespace DrtJournal.CustomJournaling
{
    /// <summary>
    /// This page uses custom journaling. Each custom journal entry takes a snapshot of 
    /// colorBlock's color. This is the "page state" stored with each journal entry and replayed
    /// later on. The class encapsulating the page state is CustomPageState.
    /// </summary>
    public partial class PageA : Page, System.Windows.Navigation.IProvideCustomContentState
	{
		//NavigationApplication myApp; -- The DRT doesn't have that.
		NavigationWindow _navWindow;
    
		void Init(object sender, EventArgs args)
		{
			//myApp = (NavigationApplication) System.Windows.Application.Current;
            //navWindow = (NavigationWindow) myApp.MainWindow;
            _navWindow = (NavigationWindow)this.Parent;
		}

        void OnChangeColor(object sender, RoutedEventArgs args)
        {
            Color newColor;
            try
            {
                newColor = (Color)ColorConverter.ConvertFromString(this.colorTextBox.Text);
            }
            catch
            {
                return;
            }
            finally
            {
                // Select and focus the text, to prepare for the next entry
                colorTextBox.Select(0, colorTextBox.Text.Length);
                colorTextBox.Focus();
            }
            JournalAndChangeBlockColor(newColor);
        }

        public Color BlockColor
        {
            get { return ((SolidColorBrush)this.colorBlock.Fill).Color; }
            set { colorBlock.Fill = new SolidColorBrush(value); }
        }

        /// <summary>
        /// Pushes a custom journal entry to remember the previous color and then sets the new color.
        /// </summary>
        /// <param name="newColor"></param>
        public void JournalAndChangeBlockColor(Color newColor)
        {
            // Take a snapshot of the current page state. Passing null for CustomContentState
            // requests the GetContentState() callback to be used.
            this._navWindow.AddBackEntry(null);

            // Change the "page state"
            this.BlockColor = newColor;
        }

        CustomContentState IProvideCustomContentState.GetContentState()
        {
            return new CustomPageState(this);
        }

	};

    [Serializable]
    class CustomPageState : System.Windows.Navigation.CustomContentState
    {
        Color _color;

        public CustomPageState(PageA page)
        {
            _color = page.BlockColor;
        }

        public override string JournalEntryName
        {
            get { return LookupColorName(_color); }
        }

        public override void Replay(NavigationService ns, NavigationMode mode)
        {
            ((PageA)ns.Content).BlockColor = _color;
        }

        string LookupColorName(Color color)
        {
            PropertyInfo[] props = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (PropertyInfo prop in props)
                if (prop.PropertyType == typeof(Color))
                {
                    Color curColor = (Color)typeof(Colors).InvokeMember(
                        prop.Name, BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public,
                        null, null, null);
                    if (curColor == color)
                        return prop.Name;
                }
            return color.ToString(); // "#RGB"
        }
    };
}
