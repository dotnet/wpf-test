// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using WinFundamentalReports.Reports;

#endregion

namespace WinFundamentalReports.Forms
{
    /// <summary>Main window for the application.</summary>
    partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private bool RunReport(string connectionString, string queryText,
            System.Text.StringBuilder builder)
        {
            SqlConnection connection;

            System.Diagnostics.Debug.Assert(builder != null);
            System.Diagnostics.Debug.Assert(queryText != null);

            using (connection = new SqlConnection(connectionString))
            {
                SqlCommand query;
                SqlDataReader reader;

                connection.Open();
                query = new SqlCommand();
                query.Connection = connection;
                query.CommandText = queryText;
                try
                {
                    reader = query.ExecuteReader();
                }
                catch(Exception exception)
                {
                    builder.Append(exception.ToString());
                    return false;
                }
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    builder.Append(reader.GetName(i));
                    if (i < reader.FieldCount - 1)
                    {
                        builder.Append(" - ");
                    }
                }
                builder.AppendLine();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        try
                        {
                            builder.Append(reader.GetValue(i));
                        }
                        catch(Exception exception)
                        {
                            builder.Append(exception.ToString());
                            return false;
                        }
                        if (i < reader.FieldCount - 1)
                        {
                            builder.Append(" - ");
                        }
                    }
                    builder.AppendLine();
                }
                return true;
            }
        }

        private bool RunCoverageReport(string queryText, System.Text.StringBuilder builder)
        {
            return RunReport(CoverageConnectionString, queryText, builder);
        }

        private bool RunTacticsReport(string queryText, System.Text.StringBuilder builder)
        {
            return RunReport(this.Options.TacticsConnectionString, queryText, builder);
        }

        private void ShowReport(string connectionString, string queryText)
        {
            DataSet dataset;
            string errorMessage;
            Form form;

            dataset = new DataSet("Query");
            errorMessage = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter;

                    connection.Open();
                    adapter = new SqlDataAdapter(queryText, connection);
                    adapter.Fill(dataset);
                }
            }
            catch (SqlException exception)
            {
                errorMessage = exception.ToString();
            }

            form = new Form();
            if (errorMessage == null)
            {
                DataGridView grid;

                grid = new DataGridView();
                grid.Dock = DockStyle.Fill;
                grid.DataSource = dataset;
                grid.DataMember = dataset.Tables[0].TableName;
                form.Controls.Add(grid);
            }
            else
            {
                Label errorLabel;

                errorLabel = new Label();
                errorLabel.Dock = DockStyle.Fill;
                errorLabel.Text = errorMessage;
                form.Controls.Add(errorLabel);
            }
            form.Show(this);
        }

        private string CoverageConnectionString
        {
            get
            {
                // return = @"Data Source=tamales\scratch;Initial Catalog=Editing10062004;User ID=user;Password=user";
                return this.Options.AvalonCoverageConnection;
            }
        }

        private void RunQueryButton_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder builder;

            builder = new System.Text.StringBuilder();
            if (sender == QueryCoverageButton)
            {
                ShowReport(CoverageConnectionString, QueryBox.Text);
            }
            else if (sender == QueryTacticsButton)
            {
                RunReport(this.Options.TacticsConnectionString, QueryBox.Text, builder);
            }
            else
            {
                throw new ArgumentException("Unknown sender", "sender");
            }
            this.ReportBox.Text = builder.ToString();
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            using (OptionsForm form = new OptionsForm())
            {
                form.LoadFromOptions(options);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    form.SaveToOptions(options);
                    Options.SaveToFile(OptionsPath);
                }
            }
        }

        public FundamentalReportOptions Options
        {
            get { return this.options; }
        }

        private FundamentalReportOptions options;

        public string OptionsPath
        {
            get
            {
                return System.IO.Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "test-manager-toolkit.options");
            }
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(OptionsPath))
            {
                try
                {
                    options = FundamentalReportOptions.LoadFromFile(OptionsPath);
                }
                catch(NotSupportedException)
                {
                    options = FundamentalReportOptions.CreateNewOptions();
                }
            }
            else
            {
                options = FundamentalReportOptions.CreateNewOptions();
            }
            
            Array reportKinds;
            reportKinds = Enum.GetValues(typeof(ReportKinds));
            foreach(ReportKinds kind in reportKinds)
            {
                ReportsListBox.Items.Add(kind, (kind & options.ReportKinds) != 0);
            }
        }

        private void UpdateOptionsFromControls()
        {
            ReportKinds reportKinds;

            reportKinds = (ReportKinds)0;
            foreach(ReportKinds kind in ReportsListBox.CheckedItems)
            {
                reportKinds |= kind;
            }
            this.Options.ReportKinds = reportKinds;
        }

        private void ComprehensiveReportButtonClick(object sender, EventArgs e)
        {
            ComprehensiveReport report;

            UpdateOptionsFromControls();
            
            report = new ComprehensiveReport();
            report.Options = this.Options;
            try
            {
                report.WriteReport();
            }
            catch (System.IO.IOException exception)
            {
                MessageBox.Show(
                    "Unable to write report. Please ensure that Excel is not accessing it.\r\n" +
                    exception.ToString(), "Error writing report", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            this.ReportBox.Text = "Report available in My Documents.";
        }

        private void MainFormFormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateOptionsFromControls();
            Options.SaveToFile(OptionsPath);
        }
    }
}
