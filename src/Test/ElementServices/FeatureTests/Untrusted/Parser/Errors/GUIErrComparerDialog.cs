// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Avalon.Test.CoreUI.Parser.Error
{
    /// <summary>
    /// UI dialog used by GUIErrComparer.
    /// This class contains mostly UI-related code.
    /// </summary>
    public partial class GUIErrComparerDialog : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorData"></param>
        /// <param name="resultData"></param>
        /// <param name="filepath">Path of file, whose contents are to be shown in the UI</param>
        public GUIErrComparerDialog(Hashtable errorData, Hashtable resultData, string filepath)
        {
            InitializeComponent();
            this.label5.Text = Path.GetFileName(filepath);
            this.richTextBox1.Text = File.ReadAllText(filepath);
            PopulateDataGridViews(dataGridView1, dataGridView2, errorData, resultData);
            
        }

        private void PopulateDataGridViewHoriz(DataGridView dataGridView, Hashtable data)
        {
            dataGridView.ColumnCount = data.Count;

            // Create the string array for each row of data.
            string[] row0 = new string[data.Count];
            string[] row1 = new string[data.Count];

            int i = 0;
            foreach (string key in data.Keys)
            {
                row0[i++] = key;
            }
            i = 0;
            foreach (string value in data.Values)
            {
                row1[i++] = value;
            }

            // Add a row for each string array.
            {
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows.Add(row0);
                rows.Add(row1);
            }
            DataGridViewCellStyle boldCell = new DataGridViewCellStyle();
            boldCell.Font = new Font(new FontFamily("Arial"), 12, FontStyle.Bold);
            dataGridView.Rows[0].DefaultCellStyle = boldCell;
        }

        private void PopulateDataGridViewVert(DataGridView dataGridView, Hashtable data)
        {
            dataGridView.ColumnCount = 2;
            DataGridViewRowCollection rows = dataGridView.Rows;
            foreach (string key in data.Keys)
            {
                // Create the string array for each row of data.
                string[] row = new string[2];
                row[0] = key;
                row[1] = data[key] as string;
                rows.Add(row);
            }

            DataGridViewCellStyle boldCell = new DataGridViewCellStyle();
            boldCell.Font = new Font(new FontFamily("Arial"), 12, FontStyle.Bold);
            dataGridView.Columns[0].DefaultCellStyle = boldCell;

            dataGridView.ClearSelection();
        }

        private void PopulateDataGridViews(DataGridView dataGridView1, DataGridView dataGridView2,
            Hashtable errorData, Hashtable rsltData)
        {
            Hashtable resultData = rsltData.Clone() as Hashtable;
            PopulateDataGridViewVert(dataGridView1, errorData);

            DataGridViewCellStyle boldCell = new DataGridViewCellStyle();
            boldCell.Font = new Font(new FontFamily("Arial"), 12, FontStyle.Bold);

            DataGridViewCellStyle redCell = new DataGridViewCellStyle();
            redCell.BackColor = Color.Red;

            DataGridViewCellStyle yellowCell = new DataGridViewCellStyle();
            yellowCell.BackColor = Color.Yellow;

            DataGridViewCellStyle pinkCell = new DataGridViewCellStyle();
            pinkCell.BackColor = Color.Pink;

            dataGridView2.ColumnCount = 2;
            dataGridView2.Columns[0].DefaultCellStyle = boldCell;

            DataGridViewRowCollection rows = dataGridView2.Rows;
            foreach (string key in errorData.Keys)
            {
                string[] rowValues = new string[2];
                if (resultData.Contains(key))
                {
                    rowValues[0] = key;
                    rowValues[1] = resultData[key] as string;
                    rows.Add(rowValues);
                    if (!errorData[key].Equals(resultData[key]))
                    {
                        dataGridView1.Rows[rows.Count - 2].DefaultCellStyle = pinkCell;
                        rows[rows.Count - 2].DefaultCellStyle = pinkCell;
                    }
                    resultData.Remove(key);
                }
                else
                {
                    rowValues[0] = "";
                    rowValues[1] = "";
                    rows.Add(rowValues);
                    dataGridView1.Rows[rows.Count - 2].DefaultCellStyle = redCell;
                }
            }

            // These keys are in resultData, but not in errorData.
            foreach (string remainingKey in resultData.Keys)
            {
                string[] rowValues = new string[2];
                rowValues[0] = remainingKey;                
                rowValues[1] = resultData[remainingKey] as string;
                rows.Add(rowValues);
                int rowNumber = rows.Count - 2;
                DataGridViewRow row = rows[rowNumber];
                row.DefaultCellStyle = yellowCell;
            }
            dataGridView2.ClearSelection();

           // AdjustHeightOfDataGridViews(dataGridView1, dataGridView2);
           // dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
           // dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView2.Columns[0].Width = 200;
            dataGridView1.Columns[0].Width = 200;
            button1.Height = Math.Max(100, dataGridView1.Height);
            button2.Height = Math.Max(100, dataGridView2.Height);
            pnlExistingMaster.Height = dataGridView1.Height;
            pnlChangeTo.Height = dataGridView2.Height;

        }

        private void AdjustHeightOfDataGridViews(DataGridView dataGridView1, DataGridView dataGridView2)
        {
            int commonRowsCount = Math.Min(dataGridView1.RowCount, dataGridView2.RowCount);

            for (int i = 0; i < commonRowsCount; i++)
            {
                int maxHeight = Math.Max(dataGridView1.Rows[i].Height, dataGridView2.Rows[i].Height);
                dataGridView1.Rows[i].Height = maxHeight;
                dataGridView2.Rows[i].Height = maxHeight;

            }

            DataGridView dataGridView;
            if (dataGridView1.RowCount > dataGridView2.RowCount)
            {
                dataGridView = dataGridView1;
            }
            else if (dataGridView2.RowCount > dataGridView1.RowCount)
            {
                dataGridView = dataGridView2;
            }
            else
            {
                return;
            }
            
            for (int i = commonRowsCount; i < dataGridView.RowCount; i++)
            {
                dataGridView.Rows[i].Height = dataGridView.Rows[i].Height;
            }
        }

        private void OnTextBoxSelectionChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            int line = this.richTextBox1.GetLineFromCharIndex(this.richTextBox1.SelectionStart);
            int position = this.richTextBox1.SelectionStart - this.richTextBox1.GetFirstCharIndexFromLine(line) + 1;
            this.toolStripStatusLabel1.Text = "Ln " + (line + 1) + " , Col " + position;
        }

        private void OnStatusBarPaint(object sender, PaintEventArgs e)
        {
            UpdateStatusBar();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
