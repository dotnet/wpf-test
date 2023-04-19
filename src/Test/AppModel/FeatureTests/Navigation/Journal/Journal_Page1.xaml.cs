// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class Page1 : Page, IProvideCustomContentState
    {
        #region navigation and journaling when switching users
        public struct User
        {
            public int Index;
            public String Name;
        }

        private User _currentUser;
        internal bool _isUserInitiatedLbxChange = false;

        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
            }
        }

        CustomContentState IProvideCustomContentState.GetContentState()
        {
            return new UserPageState(this.CurrentUser.Index, this.CurrentUser.Name);
        }

        public bool SelectUser(String selectedUser)
        {
            try
            {
                ListBoxItem lbiSelectedUser = userList.FindName(selectedUser) as ListBoxItem;
                userList.SelectedIndex = userList.Items.IndexOf(lbiSelectedUser);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        void userChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lbx = (ListBox)sender;
            if (lbx.SelectedItem != null && ((ListBoxItem)lbx.SelectedItem).Content != null)
            {
                #region journal last user visited
                if (_isUserInitiatedLbxChange)
                    // Add CurrentUser to the back stack
                    ((NavigationWindow)this.Parent).AddBackEntry(null);

                // update CurrentUser to be the currently selected ListBoxItem
                _currentUser.Index = lbx.SelectedIndex;
                _currentUser.Name = ((ListBoxItem)lbx.SelectedItem).Content.ToString();
                #endregion
            }
        }

        void OnPage1Loaded(object sender, RoutedEventArgs e)
        {
            _isUserInitiatedLbxChange = true;
        }

    }


    [Serializable]
    // This class encapsulates the current user data displayed in the Rolodex main panel
    class UserPageState : CustomContentState
    {
        private int _index;
        private String _name;

        public UserPageState(int myIndex, String myName)
        {
            _index = myIndex;
            _name = myName;
        }

        public override string JournalEntryName
        {
            get
            {
                return _name;
            }
        }

        public override void Replay(NavigationService navigationService, NavigationMode mode)
        {
            bool saved = ((Page1)navigationService.Content)._isUserInitiatedLbxChange;

            ((Page1)navigationService.Content)._isUserInitiatedLbxChange = false;
            ((Page1)navigationService.Content).userList.SelectedIndex = _index;

            ((Page1)navigationService.Content)._isUserInitiatedLbxChange = saved;
        }
    }
}
