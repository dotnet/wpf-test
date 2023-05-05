// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Microsoft.Test.DataServices.Regressions
{
    /// <summary>
    /// Interaction logic for RegressionTest1.xaml
    /// </summary>
    public partial class RegressionTest1 : System.Windows.Controls.UserControl
    {
        public RegressionTest1()
        {
            InitializeComponent();
            LoadRandomData();
        }

        private void TheGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = string.Format("{0:00}", e.Row.GetIndex() + 1);
        }

        private void LoadRandomData()
        {
            Random rand = new Random();
            int numberOfColumns = rand.Next(10, 20);
            int numberOfRows = rand.Next(100, 300);

            // Add the necessary columns
            for (int i = 0; i < numberOfColumns; i++)
            {
                TheGrid.Columns.Add(
                            new DataGridTextColumn
                            {
                                Header = string.Format("{0:00}", i),
                                Binding = new Binding(string.Format("[{0}]", i)),
                                Width = DataGridLength.Auto
                            }
                            );
            }


            // Create empty rows
            List<object[]> randomData = new List<object[]>();
            for (int i = 0; i < numberOfRows; i++)
            {
                randomData.Add(new object[numberOfColumns]);
            }

            for (int i = 0; i < (numberOfColumns * numberOfRows); i++)
            {
                int row = i / numberOfColumns;
                int column = i % numberOfColumns;
                randomData[row][column] = rand.NextDouble(); // This one is BROKE
                //randomData[row][column] = rand.Next(); // This one works
            }

            // Bind
            TheGrid.DataContext = randomData;
        }
    }
}
