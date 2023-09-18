// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides an interactive application to generate combinatorial matrixes.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 17 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/Interactive/TreeNavigator.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;

    using System.ComponentModel.Design;
    using Drawing = System.Drawing;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// An interactive application to generate combinatorial
    /// matrixes.
    /// </summary>
    public class CombinatorialExplorer: CustomTestCase
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
            _topPanel.LastChildFill = true;
            _instructions = new FlowDocumentScrollViewer();
            _instructions.Document = new FlowDocument();
            _dimensionsBox = new TextBox();
            _generateButton = new Button();
            _resultBox = new TextBox();

            new UIElementWrapper(_instructions).XamlText =
                "<Paragraph>Use the following syntax to specify the " +
                "dimensions you want to have combined.</Paragraph><List>" +
                "<ListBoxItem>Blank lines are ignored.</ListBoxItem>" +
                "<ListBoxItem>Lines starting with # are ignored.</ListBoxItem>" +
                "<ListBoxItem>[dimension-name]: [value0] (, [value1] (,[value2])...)</ListBoxItem>" +
                "</List>";
            DockPanel.SetDock(_instructions, Dock.Top);

            _dimensionsBox.AcceptsReturn = true;
            _dimensionsBox.FontFamily = new FontFamily("Courier New");
            _dimensionsBox.FontSize = 12;
            _dimensionsBox.MinLines = 6;
            _dimensionsBox.MaxLines = 10;
            _dimensionsBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _dimensionsBox.Text = "sample-dimension: my-value, my-other-value";
            _dimensionsBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _dimensionsBox.TextWrapping = TextWrapping.NoWrap;
            DockPanel.SetDock(_dimensionsBox, Dock.Top);

            _generateButton.Content = "Generate Combinations";
            _generateButton.Click += GenerateButtonClick;
            DockPanel.SetDock(_generateButton, Dock.Top);

            _resultBox.AcceptsReturn = true;
            _resultBox.FontFamily = _dimensionsBox.FontFamily;
            _resultBox.FontSize = _dimensionsBox.FontSize;
            _resultBox.IsReadOnly = true;
            _resultBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            //DockPanel.SetDock(_resultBox, Dock.Fill);

            _topPanel.Children.Add(_instructions);
            _topPanel.Children.Add(_dimensionsBox);
            _topPanel.Children.Add(_generateButton);
            _topPanel.Children.Add(_resultBox);

            MainWindow.Content = _topPanel;
        }

        /// <summary>
        /// Creates the CombinatorialEngine for the given description.
        /// </summary>
        /// <param name="description">Description of dimensions and values.</param>
        /// <param name="engine">On return, engine created.</param>
        /// <param name="dimensionNames">
        /// On return, dimension names, in the order specified by the user.
        /// </param>
        private void CreateCombinatorialEngine(string description,
            out CombinatorialEngine engine, out string[] dimensionNames)
        {
            ArrayList dimensions;
            string[] lines;

            dimensions = new ArrayList();
            lines = _dimensionsBox.Text.Trim().Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string line;            // Line to analyze.
                string[] lineParts;     // Parts of line.
                string dimensionName;   // Name of dimension.
                string[] values;        // Values for dimension.

                line = lines[i].Trim();
                if (line.Length == 0) continue;
                if (line[0] == '#') continue;
                lineParts = line.Split(':');
                if (lineParts.Length != 2)
                {
                    throw new Exception("Cannot find name/values in " + line);
                }

                dimensionName = lineParts[0].Trim();
                values = lineParts[1].Split(',');

                // Remove whitespace from values.
                for (int j = 0; j < values.Length; j++)
                {
                    values[j] = values[j].Trim();
                }

                dimensions.Add(new Dimension(dimensionName, values));
            }

            engine = CombinatorialEngine.FromDimensions(
                (Dimension[])dimensions.ToArray(typeof(Dimension)));

            dimensionNames = new string[dimensions.Count];
            for (int i = 0; i < dimensions.Count; i++)
            {
                dimensionNames[i] = ((Dimension)dimensions[i]).Name;
            }
        }

        /// <summary>
        /// Generates all combinations for the user specification and
        /// puts them in the result text box.
        /// </summary>
        private void GenerateButtonClick(object sender, RoutedEventArgs e)
        {
            StringBuilder builder;      // Buffer for building resulting text.
            Hashtable combination;      // Combination values.
            int combinationCount;       // Number of combinations generated.
            string[] dimensionNames;    // Names of dimensions, in user order.
            CombinatorialEngine engine; // Engine to generate combinations.
            string resultText;          // Resulting text of operation.

            try
            {
                combination = new Hashtable();
                combinationCount = 0;
                builder = new StringBuilder("Combinations:\r\n\r\n");
                CreateCombinatorialEngine(_dimensionsBox.Text.Trim(),
                    out engine, out dimensionNames);

                while (engine.Next(combination))
                {
                    for (int i = 0; i < dimensionNames.Length; i++)
                    {
                        if (i > 0) builder.Append(',');
                        builder.Append(combination[dimensionNames[i]]);
                    }
                    builder.Append("\r\n");
                    combinationCount++;
                }

                builder.Append("\r\nCombination count: " + combinationCount);
                resultText = builder.ToString();
            }
            catch(Exception exception)
            {
                resultText = exception.ToString();
            }
            _resultBox.Text = resultText;
        }

        #endregion Private methods.

        #region Private fields.

        private DockPanel _topPanel;
        private FlowDocumentScrollViewer _instructions;
        private TextBox _dimensionsBox;
        private Button _generateButton;
        private TextBox _resultBox;

        #endregion Private fields.
    }
}
