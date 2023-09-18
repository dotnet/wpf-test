namespace WinFundamentalReports.Forms
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TacticsPage = new System.Windows.Forms.TabPage();
            this.TacticsConnectionString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CoveragePage = new System.Windows.Forms.TabPage();
            this.CoveragePasswordBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.CoverageUserNameBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ViewDatabasesButton = new System.Windows.Forms.Button();
            this.CoverageDatabaseBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CoverageServerName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FilteringPage = new System.Windows.Forms.TabPage();
            this.MemberFiltersFileButton = new System.Windows.Forms.Button();
            this.MemberFiltersFileBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.OpenTypeFiltersButton = new System.Windows.Forms.Button();
            this.TypeNameFiltersPathBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TacticsNodeBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.CleanupTab = new System.Windows.Forms.TabPage();
            this.CleanupPathsBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.AvalonCoveragePage = new System.Windows.Forms.TabPage();
            this.ListCoverageBuildsButton = new System.Windows.Forms.Button();
            this.AvalonCoverageBuildId = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.AvalonConnectionBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.AbortButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.TaskListTabPage = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.TaskListQueryTextBox = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.TacticsPage.SuspendLayout();
            this.CoveragePage.SuspendLayout();
            this.FilteringPage.SuspendLayout();
            this.CleanupTab.SuspendLayout();
            this.AvalonCoveragePage.SuspendLayout();
            this.TaskListTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.TacticsPage);
            this.tabControl1.Controls.Add(this.CoveragePage);
            this.tabControl1.Controls.Add(this.FilteringPage);
            this.tabControl1.Controls.Add(this.CleanupTab);
            this.tabControl1.Controls.Add(this.AvalonCoveragePage);
            this.tabControl1.Controls.Add(this.TaskListTabPage);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(377, 302);
            this.tabControl1.TabIndex = 0;
            // 
            // TacticsPage
            // 
            this.TacticsPage.Controls.Add(this.TacticsConnectionString);
            this.TacticsPage.Controls.Add(this.label1);
            this.TacticsPage.Location = new System.Drawing.Point(4, 22);
            this.TacticsPage.Name = "TacticsPage";
            this.TacticsPage.Padding = new System.Windows.Forms.Padding(3);
            this.TacticsPage.Size = new System.Drawing.Size(369, 276);
            this.TacticsPage.TabIndex = 0;
            this.TacticsPage.Text = "Tactics";
            // 
            // TacticsConnectionString
            // 
            this.TacticsConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TacticsConnectionString.Location = new System.Drawing.Point(7, 28);
            this.TacticsConnectionString.Multiline = true;
            this.TacticsConnectionString.Name = "TacticsConnectionString";
            this.TacticsConnectionString.Size = new System.Drawing.Size(356, 142);
            this.TacticsConnectionString.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Connection string:";
            // 
            // CoveragePage
            // 
            this.CoveragePage.Controls.Add(this.CoveragePasswordBox);
            this.CoveragePage.Controls.Add(this.label5);
            this.CoveragePage.Controls.Add(this.CoverageUserNameBox);
            this.CoveragePage.Controls.Add(this.label4);
            this.CoveragePage.Controls.Add(this.ViewDatabasesButton);
            this.CoveragePage.Controls.Add(this.CoverageDatabaseBox);
            this.CoveragePage.Controls.Add(this.label3);
            this.CoveragePage.Controls.Add(this.CoverageServerName);
            this.CoveragePage.Controls.Add(this.label2);
            this.CoveragePage.Location = new System.Drawing.Point(4, 22);
            this.CoveragePage.Name = "CoveragePage";
            this.CoveragePage.Padding = new System.Windows.Forms.Padding(3);
            this.CoveragePage.Size = new System.Drawing.Size(369, 276);
            this.CoveragePage.TabIndex = 1;
            this.CoveragePage.Text = "Coverage";
            // 
            // CoveragePasswordBox
            // 
            this.CoveragePasswordBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CoveragePasswordBox.Location = new System.Drawing.Point(7, 172);
            this.CoveragePasswordBox.Name = "CoveragePasswordBox";
            this.CoveragePasswordBox.Size = new System.Drawing.Size(355, 20);
            this.CoveragePasswordBox.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "&Password:";
            // 
            // CoverageUserNameBox
            // 
            this.CoverageUserNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CoverageUserNameBox.Location = new System.Drawing.Point(7, 124);
            this.CoverageUserNameBox.Name = "CoverageUserNameBox";
            this.CoverageUserNameBox.Size = new System.Drawing.Size(355, 20);
            this.CoverageUserNameBox.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "&User name:";
            // 
            // ViewDatabasesButton
            // 
            this.ViewDatabasesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ViewDatabasesButton.Location = new System.Drawing.Point(287, 76);
            this.ViewDatabasesButton.Name = "ViewDatabasesButton";
            this.ViewDatabasesButton.Size = new System.Drawing.Size(75, 23);
            this.ViewDatabasesButton.TabIndex = 4;
            this.ViewDatabasesButton.Text = "&View...";
            this.ViewDatabasesButton.Click += new System.EventHandler(this.ViewDatabasesButtonClick);
            // 
            // CoverageDatabaseBox
            // 
            this.CoverageDatabaseBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CoverageDatabaseBox.Location = new System.Drawing.Point(7, 76);
            this.CoverageDatabaseBox.Name = "CoverageDatabaseBox";
            this.CoverageDatabaseBox.Size = new System.Drawing.Size(273, 20);
            this.CoverageDatabaseBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "&Database:";
            // 
            // CoverageServerName
            // 
            this.CoverageServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CoverageServerName.Location = new System.Drawing.Point(7, 28);
            this.CoverageServerName.Name = "CoverageServerName";
            this.CoverageServerName.Size = new System.Drawing.Size(356, 20);
            this.CoverageServerName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Server:";
            // 
            // FilteringPage
            // 
            this.FilteringPage.Controls.Add(this.MemberFiltersFileButton);
            this.FilteringPage.Controls.Add(this.MemberFiltersFileBox);
            this.FilteringPage.Controls.Add(this.label8);
            this.FilteringPage.Controls.Add(this.OpenTypeFiltersButton);
            this.FilteringPage.Controls.Add(this.TypeNameFiltersPathBox);
            this.FilteringPage.Controls.Add(this.label7);
            this.FilteringPage.Controls.Add(this.TacticsNodeBox);
            this.FilteringPage.Controls.Add(this.label6);
            this.FilteringPage.Location = new System.Drawing.Point(4, 22);
            this.FilteringPage.Name = "FilteringPage";
            this.FilteringPage.Size = new System.Drawing.Size(369, 276);
            this.FilteringPage.TabIndex = 2;
            this.FilteringPage.Text = "Filtering";
            // 
            // MemberFiltersFileButton
            // 
            this.MemberFiltersFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MemberFiltersFileButton.Location = new System.Drawing.Point(335, 124);
            this.MemberFiltersFileButton.Name = "MemberFiltersFileButton";
            this.MemberFiltersFileButton.Size = new System.Drawing.Size(26, 23);
            this.MemberFiltersFileButton.TabIndex = 7;
            this.MemberFiltersFileButton.Text = "...";
            this.MemberFiltersFileButton.Click += new System.EventHandler(this.MemberFiltersFileButtonClick);
            // 
            // MemberFiltersFileBox
            // 
            this.MemberFiltersFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MemberFiltersFileBox.Location = new System.Drawing.Point(7, 124);
            this.MemberFiltersFileBox.Name = "MemberFiltersFileBox";
            this.MemberFiltersFileBox.Size = new System.Drawing.Size(321, 20);
            this.MemberFiltersFileBox.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 103);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "&Member filters file:";
            // 
            // OpenTypeFiltersButton
            // 
            this.OpenTypeFiltersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenTypeFiltersButton.Location = new System.Drawing.Point(335, 76);
            this.OpenTypeFiltersButton.Name = "OpenTypeFiltersButton";
            this.OpenTypeFiltersButton.Size = new System.Drawing.Size(26, 23);
            this.OpenTypeFiltersButton.TabIndex = 4;
            this.OpenTypeFiltersButton.Text = "...";
            this.OpenTypeFiltersButton.Click += new System.EventHandler(this.OpenTypeFiltersButtonClick);
            // 
            // TypeNameFiltersPathBox
            // 
            this.TypeNameFiltersPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TypeNameFiltersPathBox.Location = new System.Drawing.Point(7, 76);
            this.TypeNameFiltersPathBox.Name = "TypeNameFiltersPathBox";
            this.TypeNameFiltersPathBox.Size = new System.Drawing.Size(321, 20);
            this.TypeNameFiltersPathBox.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "T&ype filters file:";
            // 
            // TacticsNodeBox
            // 
            this.TacticsNodeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TacticsNodeBox.Location = new System.Drawing.Point(7, 28);
            this.TacticsNodeBox.Name = "TacticsNodeBox";
            this.TacticsNodeBox.Size = new System.Drawing.Size(355, 20);
            this.TacticsNodeBox.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "&Tactics Node:";
            // 
            // CleanupTab
            // 
            this.CleanupTab.Controls.Add(this.CleanupPathsBox);
            this.CleanupTab.Controls.Add(this.label9);
            this.CleanupTab.Location = new System.Drawing.Point(4, 22);
            this.CleanupTab.Name = "CleanupTab";
            this.CleanupTab.Padding = new System.Windows.Forms.Padding(3);
            this.CleanupTab.Size = new System.Drawing.Size(369, 276);
            this.CleanupTab.TabIndex = 3;
            this.CleanupTab.Text = "Cleanup";
            // 
            // CleanupPathsBox
            // 
            this.CleanupPathsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CleanupPathsBox.Location = new System.Drawing.Point(7, 46);
            this.CleanupPathsBox.Multiline = true;
            this.CleanupPathsBox.Name = "CleanupPathsBox";
            this.CleanupPathsBox.Size = new System.Drawing.Size(356, 224);
            this.CleanupPathsBox.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.Location = new System.Drawing.Point(7, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(356, 33);
            this.label9.TabIndex = 0;
            this.label9.Text = "Enter the paths to search for code cleanup, one per line:";
            // 
            // AvalonCoveragePage
            // 
            this.AvalonCoveragePage.Controls.Add(this.ListCoverageBuildsButton);
            this.AvalonCoveragePage.Controls.Add(this.AvalonCoverageBuildId);
            this.AvalonCoveragePage.Controls.Add(this.label11);
            this.AvalonCoveragePage.Controls.Add(this.AvalonConnectionBox);
            this.AvalonCoveragePage.Controls.Add(this.label10);
            this.AvalonCoveragePage.Location = new System.Drawing.Point(4, 22);
            this.AvalonCoveragePage.Name = "AvalonCoveragePage";
            this.AvalonCoveragePage.Padding = new System.Windows.Forms.Padding(3);
            this.AvalonCoveragePage.Size = new System.Drawing.Size(369, 276);
            this.AvalonCoveragePage.TabIndex = 4;
            this.AvalonCoveragePage.Text = "Avalon Coverage";
            // 
            // ListCoverageBuildsButton
            // 
            this.ListCoverageBuildsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ListCoverageBuildsButton.Location = new System.Drawing.Point(288, 74);
            this.ListCoverageBuildsButton.Name = "ListCoverageBuildsButton";
            this.ListCoverageBuildsButton.Size = new System.Drawing.Size(75, 23);
            this.ListCoverageBuildsButton.TabIndex = 4;
            this.ListCoverageBuildsButton.Text = "&List...";
            this.ListCoverageBuildsButton.Click += new System.EventHandler(this.ListCoverageBuildsButtonClick);
            // 
            // AvalonCoverageBuildId
            // 
            this.AvalonCoverageBuildId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AvalonCoverageBuildId.Location = new System.Drawing.Point(7, 76);
            this.AvalonCoverageBuildId.Name = "AvalonCoverageBuildId";
            this.AvalonCoverageBuildId.Size = new System.Drawing.Size(275, 20);
            this.AvalonCoverageBuildId.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 55);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "&Build Id:";
            // 
            // AvalonConnectionBox
            // 
            this.AvalonConnectionBox.Location = new System.Drawing.Point(7, 28);
            this.AvalonConnectionBox.Name = "AvalonConnectionBox";
            this.AvalonConnectionBox.Size = new System.Drawing.Size(246, 20);
            this.AvalonConnectionBox.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 7);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "&Connection String:";
            // 
            // AbortButton
            // 
            this.AbortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AbortButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AbortButton.Location = new System.Drawing.Point(315, 322);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 1;
            this.AbortButton.Text = "Cancel";
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(233, 322);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 0;
            this.OKButton.Text = "OK";
            // 
            // TaskListTabPage
            // 
            this.TaskListTabPage.Controls.Add(this.TaskListQueryTextBox);
            this.TaskListTabPage.Controls.Add(this.label12);
            this.TaskListTabPage.Location = new System.Drawing.Point(4, 22);
            this.TaskListTabPage.Name = "TaskListTabPage";
            this.TaskListTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.TaskListTabPage.Size = new System.Drawing.Size(369, 276);
            this.TaskListTabPage.TabIndex = 5;
            this.TaskListTabPage.Text = "Task List";
            this.TaskListTabPage.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 7);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(351, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "&Task list query: (press Ctrl while running query in PS to paste to clipboard)";
            // 
            // TaskListQueryTextBox
            // 
            this.TaskListQueryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TaskListQueryTextBox.Location = new System.Drawing.Point(10, 24);
            this.TaskListQueryTextBox.Multiline = true;
            this.TaskListQueryTextBox.Name = "TaskListQueryTextBox";
            this.TaskListQueryTextBox.Size = new System.Drawing.Size(348, 246);
            this.TaskListQueryTextBox.TabIndex = 1;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.AbortButton;
            this.ClientSize = new System.Drawing.Size(402, 357);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.AbortButton);
            this.Controls.Add(this.tabControl1);
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.tabControl1.ResumeLayout(false);
            this.TacticsPage.ResumeLayout(false);
            this.TacticsPage.PerformLayout();
            this.CoveragePage.ResumeLayout(false);
            this.CoveragePage.PerformLayout();
            this.FilteringPage.ResumeLayout(false);
            this.FilteringPage.PerformLayout();
            this.CleanupTab.ResumeLayout(false);
            this.AvalonCoveragePage.ResumeLayout(false);
            this.AvalonCoveragePage.PerformLayout();
            this.TaskListTabPage.ResumeLayout(false);
            this.TaskListTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TacticsPage;
        private System.Windows.Forms.TabPage CoveragePage;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.TabPage FilteringPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TacticsConnectionString;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox CoverageServerName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox CoverageDatabaseBox;
        private System.Windows.Forms.Button ViewDatabasesButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox CoverageUserNameBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox CoveragePasswordBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TacticsNodeBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TypeNameFiltersPathBox;
        private System.Windows.Forms.Button OpenTypeFiltersButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox MemberFiltersFileBox;
        private System.Windows.Forms.Button MemberFiltersFileButton;
        private System.Windows.Forms.TabPage CleanupTab;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox CleanupPathsBox;
        private System.Windows.Forms.TabPage AvalonCoveragePage;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox AvalonConnectionBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox AvalonCoverageBuildId;
        private System.Windows.Forms.Button ListCoverageBuildsButton;
        private System.Windows.Forms.TabPage TaskListTabPage;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox TaskListQueryTextBox;
    }
}