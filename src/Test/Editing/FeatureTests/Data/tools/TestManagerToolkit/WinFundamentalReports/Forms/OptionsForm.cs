// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinFundamentalReports.Forms
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using WinFundamentalReports.Reports;

    #endregion Namespaces.

    /// <summary>
    /// Use this class to display a window that enables users
    /// to modify options.
    /// </summary>
    /// <remarks>
    /// Use the LoadFromOptions and SaveToOptions methods to initialize
    /// the controls and get the modified values.
    /// </remarks>
    partial class OptionsForm : Form
    {
        #region Constructors.

        public OptionsForm()
        {
            InitializeComponent();
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Sets the values on the form controls based on the specified
        /// options.
        /// </summary>
        /// <param name="options">Options to load values from.</param>
        public void LoadFromOptions(FundamentalReportOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.TacticsConnectionString.Text = options.TacticsConnectionString;
            this.TacticsNodeBox.Text = options.TacticsNode;
            this.CleanupPathsBox.Lines = options.CodeCleanupPaths;
            this.CoverageDatabaseBox.Text = options.CoverageDatabase;
            this.CoveragePasswordBox.Text = options.CoveragePassword;
            this.CoverageServerName.Text = options.CoverageServerName;
            this.CoverageUserNameBox.Text = options.CoverageUserName;
            this.TypeNameFiltersPathBox.Text = options.TypeNameFiltersPath;
            this.MemberFiltersFileBox.Text = options.MembersVisibilityPath;
            this.AvalonConnectionBox.Text = options.AvalonCoverageConnection;
            this.AvalonCoverageBuildId.Text = options.AvalonCoverageBuildId.ToString();
            this.TaskListQueryTextBox.Lines = options.TaskListQuery;
        }

        /// <summary>
        /// Sets the values on the specified options based on the form controls.
        /// </summary>
        /// <param name="options">Options to save values to.</param>
        public void SaveToOptions(FundamentalReportOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            options.TacticsConnectionString = this.TacticsConnectionString.Text;
            options.TacticsNode = this.TacticsNodeBox.Text;
            options.CoverageDatabase = this.CoverageDatabaseBox.Text;
            options.CoveragePassword = this.CoveragePasswordBox.Text;
            options.CoverageServerName = this.CoverageServerName.Text;
            options.CoverageUserName = this.CoverageUserNameBox.Text;
            options.TypeNameFiltersPath = this.TypeNameFiltersPathBox.Text;
            options.MembersVisibilityPath = this.MemberFiltersFileBox.Text;
            options.CodeCleanupPaths = this.CleanupPathsBox.Lines;
            options.AvalonCoverageConnection = this.AvalonConnectionBox.Text;
            options.AvalonCoverageBuildId = Int32.Parse(this.AvalonCoverageBuildId.Text);
            options.TaskListQuery = this.TaskListQueryTextBox.Lines;
        }

        #endregion Public methods.

        #region Event handlers.

        private void DisplaySelectionDialog(string title, string filter, TextBox box)
        {
            OpenFileDialog dialog;

            using (dialog = new OpenFileDialog())
            {
                dialog.Title = title;
                dialog.Filter = filter;
                dialog.FileName = box.Text;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    box.Text = dialog.FileName;
                }
            }
        }

        private void ListCoverageBuildsButtonClick(object sender, EventArgs e)
        {
            List<CoverageBuildRecord> builds;
            StringBuilder buildList;

            builds = AvalonCoverageReport.ListBuilds(this.AvalonConnectionBox.Text);
            buildList = new StringBuilder();
            foreach (CoverageBuildRecord record in builds)
            {
                buildList.AppendLine(record.ToString());
            }

            using (Form form = new Form())
            {
                TextBox textBox;

                textBox = new TextBox();
                textBox.Multiline = true;
                textBox.Text = buildList.ToString();
                textBox.ScrollBars = ScrollBars.Vertical;
                textBox.Dock = DockStyle.Fill;
                form.Controls.Add(textBox);
                form.Text = "Avalon Coverage Builds";
                form.ShowDialog(this);
            }
        }

        private void MemberFiltersFileButtonClick(object sender, EventArgs e)
        {
            DisplaySelectionDialog("Select Filter File",
                "Comma-Separated Value Files (*.csv)|*.csv|All Files (*.*)|*.*",
                MemberFiltersFileBox);
        }

        private void OpenTypeFiltersButtonClick(object sender, EventArgs e)
        {
            DisplaySelectionDialog("Select Filter File",
                "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                TypeNameFiltersPathBox);
        }

        private void ViewDatabasesButtonClick(object sender, EventArgs e)
        {
            List<string> databases;
            string databaseList;

            databases = CoverageReport.ListBuilds(
                this.CoverageServerName.Text,
                this.CoverageUserNameBox.Text,
                this.CoveragePasswordBox.Text);
            databaseList = string.Join("\r\n", databases.ToArray());

            using (Form form = new Form())
            {
                TextBox textBox;

                textBox = new TextBox();
                textBox.Multiline = true;
                textBox.Text = databaseList;
                textBox.ScrollBars = ScrollBars.Vertical;
                textBox.Dock = DockStyle.Fill;
                form.Controls.Add(textBox);
                form.Text = "Databases (builds)";
                form.ShowDialog(this);
            }
        }

        #endregion Event handlers.
    }
}
