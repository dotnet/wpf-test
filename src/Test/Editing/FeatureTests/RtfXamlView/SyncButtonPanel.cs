// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//              Creates Sync button.

// avalon
using System.Windows.Controls;
using System.Windows.Documents;

namespace RtfXamlView
{
    class SyncButtonPanel : Grid
    {
        public SyncButtonPanel()
        {
            // Add column for Sync button
            ColumnDefinitions.Add(new ColumnDefinition());

            // Set Sync Button
            Span span = new Span(new Run("Sync"));
            _btnSync = new Button();
            _btnSync.MaxWidth = 40;
            _btnSync.Content = span;

            Grid.SetColumn(_btnSync, 0);
            Children.Add(_btnSync);
        }

        #region Public Properties
        
        // Get Properties
        public Button SyncButton
        {
            get
            {
                return _btnSync;
            }
        }
        #endregion

        private Button _btnSync;
    }
}