// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Creates two buttons for converting to and from xaml.

// avalon
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RtfXamlView
{
    class ConvertButtonsPanel : Grid
    {
        public ConvertButtonsPanel()
        {
            #region Add 2 rows to the grid (for 2 buttons)          
            // add row for Rtf to Xaml button
            RowDefinitions.Add(new RowDefinition());
            // add row for Xaml to Rtf button
            RowDefinitions.Add(new RowDefinition());
            #endregion

            // Set RtfToXaml Button
            #region RtfToXaml Button

            Span span = new Span(new Run("> RTF To XAML >"));
            span.FontSize = 10.0;

            _btnRtfToXaml = new Button();
            _btnRtfToXaml.Height = 25.0;
            _btnRtfToXaml.Content = span;

            Grid.SetRow(_btnRtfToXaml, 0);
            Children.Add(_btnRtfToXaml);
            #endregion

            // Set XamlToRtf Button
            #region XamlToRtf Button
            span = new Span(new Run("< XAML to RTF <"));
            span.FontSize = 10.0;

            _btnXamlToRtf = new Button();
            _btnXamlToRtf.Height = 25.0;
            _btnXamlToRtf.Content = span;

            Grid.SetRow(_btnXamlToRtf, 1);
            Children.Add(_btnXamlToRtf);
            #endregion
        }

        #region Public Properties

        // Get Properties
        public Button RtfToXamlButton
        {
            get
            {
                return _btnRtfToXaml;
            }
        }

        public Button XamlToRtfButton
        {
            get
            {
                return _btnXamlToRtf;
            }
        }
        #endregion

        private Button _btnRtfToXaml;
        private Button _btnXamlToRtf;
    }
}
