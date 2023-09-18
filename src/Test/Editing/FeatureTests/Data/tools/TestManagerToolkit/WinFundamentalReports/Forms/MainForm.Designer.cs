namespace WinFundamentalReports.Forms
{
    partial class MainForm
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
            this.ReportBox = new System.Windows.Forms.TextBox();
            this.QueryBox = new System.Windows.Forms.TextBox();
            this.QueryTacticsButton = new System.Windows.Forms.Button();
            this.OptionsButton = new System.Windows.Forms.Button();
            this.ComprehensiveReportButton = new System.Windows.Forms.Button();
            this.ReportsListBox = new System.Windows.Forms.CheckedListBox();
            this.QueryCoverageButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ReportBox
            // 
            this.ReportBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ReportBox.AutoSize = false;
            this.ReportBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReportBox.Location = new System.Drawing.Point(13, 145);
            this.ReportBox.Multiline = true;
            this.ReportBox.Name = "ReportBox";
            this.ReportBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ReportBox.Size = new System.Drawing.Size(478, 188);
            this.ReportBox.TabIndex = 1;
            this.ReportBox.WordWrap = false;
            // 
            // QueryBox
            // 
            this.QueryBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.QueryBox.AutoSize = false;
            this.QueryBox.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QueryBox.Location = new System.Drawing.Point(13, 88);
            this.QueryBox.Multiline = true;
            this.QueryBox.Name = "QueryBox";
            this.QueryBox.Size = new System.Drawing.Size(365, 50);
            this.QueryBox.TabIndex = 0;
            // 
            // QueryTacticsButton
            // 
            this.QueryTacticsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.QueryTacticsButton.Location = new System.Drawing.Point(384, 86);
            this.QueryTacticsButton.Name = "QueryTacticsButton";
            this.QueryTacticsButton.Size = new System.Drawing.Size(107, 23);
            this.QueryTacticsButton.TabIndex = 4;
            this.QueryTacticsButton.Text = "Query &Tactics";
            this.QueryTacticsButton.Click += new System.EventHandler(this.RunQueryButton_Click);
            // 
            // OptionsButton
            // 
            this.OptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OptionsButton.Location = new System.Drawing.Point(413, 10);
            this.OptionsButton.Name = "OptionsButton";
            this.OptionsButton.Size = new System.Drawing.Size(78, 23);
            this.OptionsButton.TabIndex = 5;
            this.OptionsButton.Text = "&Options...";
            this.OptionsButton.Click += new System.EventHandler(this.OptionsButton_Click);
            // 
            // ComprehensiveReportButton
            // 
            this.ComprehensiveReportButton.Location = new System.Drawing.Point(13, 10);
            this.ComprehensiveReportButton.Name = "ComprehensiveReportButton";
            this.ComprehensiveReportButton.Size = new System.Drawing.Size(157, 23);
            this.ComprehensiveReportButton.TabIndex = 8;
            this.ComprehensiveReportButton.Text = "&Comprehensive Report";
            this.ComprehensiveReportButton.Click += new System.EventHandler(this.ComprehensiveReportButtonClick);
            // 
            // ReportsListBox
            // 
            this.ReportsListBox.CheckOnClick = true;
            this.ReportsListBox.FormattingEnabled = true;
            this.ReportsListBox.Location = new System.Drawing.Point(176, 10);
            this.ReportsListBox.Name = "ReportsListBox";
            this.ReportsListBox.Size = new System.Drawing.Size(202, 72);
            this.ReportsListBox.TabIndex = 9;
            // 
            // QueryCoverageButton
            // 
            this.QueryCoverageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.QueryCoverageButton.Location = new System.Drawing.Point(384, 115);
            this.QueryCoverageButton.Name = "QueryCoverageButton";
            this.QueryCoverageButton.Size = new System.Drawing.Size(107, 23);
            this.QueryCoverageButton.TabIndex = 10;
            this.QueryCoverageButton.Text = "Query C&overage";
            this.QueryCoverageButton.Click += new System.EventHandler(this.RunQueryButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(503, 361);
            this.Controls.Add(this.QueryCoverageButton);
            this.Controls.Add(this.ReportsListBox);
            this.Controls.Add(this.ComprehensiveReportButton);
            this.Controls.Add(this.OptionsButton);
            this.Controls.Add(this.QueryTacticsButton);
            this.Controls.Add(this.QueryBox);
            this.Controls.Add(this.ReportBox);
            this.Name = "MainForm";
            this.Text = "Fundamental Reports";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ReportBox;
        private System.Windows.Forms.TextBox QueryBox;
        private System.Windows.Forms.Button QueryTacticsButton;
        private System.Windows.Forms.Button OptionsButton;
        private System.Windows.Forms.Button ComprehensiveReportButton;
        private System.Windows.Forms.CheckedListBox ReportsListBox;
        private System.Windows.Forms.Button QueryCoverageButton;
    }
}

