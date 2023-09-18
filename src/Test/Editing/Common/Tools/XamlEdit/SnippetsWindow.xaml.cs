// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XamlPadEdit
{
    public partial class SnippetsWindow : Window
    {
        #region Constructors

        private SnippetsWindow()
        {
        }

        public SnippetsWindow(SnippetManager manager)
        {
            this._manager = manager;
            InitializeComponent();
        }

        #endregion Constructors

        #region Private Fields

        private SnippetManager _manager;

        #endregion Private Fields

        #region Event handlers

        private void AddButtonClicked(object sender, RoutedEventArgs e)
        {
            Snippet newItem = new Snippet("New Snippet", "Enter XAML; use | to insert selection.");
            _manager.Snippets.Add(newItem);
            snippetList.SelectedItem = newItem;
        }

        private void CloseButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RemoveButtonClicked(object sender, RoutedEventArgs e)
        {
            if (snippetList.SelectedItem != null)
            {
                _manager.Snippets.Remove((Snippet)snippetList.SelectedItem);
            }
        }

        private void RestoreDefaultsButtonClicked(object sender, RoutedEventArgs e)
        {
            _manager.SetDefaultSnippets();
        }

        private void SwapDownButtonClicked(object sender, RoutedEventArgs e)
        {
            int selectedIndex = snippetList.SelectedIndex;
            if ((selectedIndex > -1) && (selectedIndex < _manager.Snippets.Count - 1))
            {
                Snippet item = (Snippet)snippetList.SelectedItem;
                _manager.Snippets.RemoveAt(selectedIndex);
                _manager.Snippets.Insert(selectedIndex + 1, item);
                snippetList.SelectedIndex = selectedIndex + 1;
            }
        }

        private void SwapUpButtonClicked(object sender, RoutedEventArgs e)
        {
            int selectedIndex = snippetList.SelectedIndex;
            if (selectedIndex > 0)
            {
                Snippet item = (Snippet)snippetList.SelectedItem;
                _manager.Snippets.RemoveAt(selectedIndex);
                _manager.Snippets.Insert(selectedIndex - 1, item);
                snippetList.SelectedIndex = selectedIndex - 1;
            }
        }

        private void WindowLoaded(object sender, EventArgs e)
        {
            mainGrid.DataContext = _manager.Snippets;
        }

        #endregion Event handlers

    }
}
