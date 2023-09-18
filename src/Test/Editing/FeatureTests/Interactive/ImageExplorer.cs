// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides an application to view images

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 19 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/BVT/Interactive/DragDropExplorer.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.IO;
    using System.ComponentModel.Design;
    using Drawing = System.Drawing;
    using System.Text;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Input;

    using Test.Uis.Data;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// An interactive application to test imaging features.
    /// </summary>
    public class ImageExplorer: CustomTestCase
    {
        #region Public methods.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            CreateUserInterface();
        }

        #endregion Public methods.

        #region Private methods.

        private void CreateUserInterface()
        {
            _topPanel = new DockPanel();
            _leftPanel = new DockPanel();
            _pathBox = new TextBox();
            _imageList = new ListBox();
            _imageScrollViewer = new ScrollViewer();
            _image = new Image();

            _pathBox.Text = System.Environment.CurrentDirectory;

            _pathBox.KeyDown += delegate(object sender, KeyEventArgs e) {
                if (e.Key == Key.Enter)
                {
                    PopulateListBox();
                    e.Handled = true;
                }
            };
            _imageList.SelectionChanged += delegate {
                string path;
                if (_imageList.SelectedItem == null)
                {
                    return;
                }
                path = _imageList.SelectedItem.ToString();
                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    _image.Source = CreateBitmapSource(stream);
                }
            };

            MainWindow.FontSize = 16;

            DockPanel.SetDock(_leftPanel, Dock.Left);
            DockPanel.SetDock(_pathBox, Dock.Top);

            _leftPanel.Children.Add(_pathBox);
            _leftPanel.Children.Add(_imageList);
            _topPanel.Children.Add(_leftPanel);
            _imageScrollViewer.Content = _image;
            _topPanel.Children.Add(_imageScrollViewer);

            PopulateListBox();

            MainWindow.Content = _topPanel;
        }

        private BitmapSource CreateBitmapSource(Stream stream)
        {
            BitmapDecoder decoder;

            decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

        private void PopulateListBox()
        {
            string[] files;
            _imageList.Items.Clear();

            files = Directory.GetFiles(_pathBox.Text, "*.png");
            foreach(string s in files)
            {
                _imageList.Items.Add(s);
            }
        }

        #endregion Private methods.

        #region Private fields.

        private DockPanel _topPanel;
        private DockPanel _leftPanel;
        private TextBox _pathBox;
        private ListBox _imageList;
        private ScrollViewer _imageScrollViewer;
        private Image _image;

        #endregion Private fields.
    }
}
