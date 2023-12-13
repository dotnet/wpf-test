using System;
using System.IO;
using Common;


namespace RtfXamlView
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
           this.groupBox1 = new System.Windows.Forms.GroupBox();
           this.chkPri0 = new System.Windows.Forms.CheckBox();
           this.chkSubFolders = new System.Windows.Forms.CheckBox();
           this.btnLogBrowse = new System.Windows.Forms.Button();
           this.tbLogFilePath = new System.Windows.Forms.TextBox();
           this.label2 = new System.Windows.Forms.Label();
           this.tbBVTFolder = new System.Windows.Forms.TextBox();
           this.btnBVTBrowse = new System.Windows.Forms.Button();
           this.label1 = new System.Windows.Forms.Label();
           this.groupBox2 = new System.Windows.Forms.GroupBox();
           this.chkFXCVT = new System.Windows.Forms.CheckBox();
           this.btnOK = new System.Windows.Forms.Button();
           this.btnCancel = new System.Windows.Forms.Button();
           this.groupBox1.SuspendLayout();
           this.groupBox2.SuspendLayout();
           this.SuspendLayout();
           // 
           // groupBox1
           // 
           this.groupBox1.Controls.Add(this.chkPri0);
           this.groupBox1.Controls.Add(this.chkSubFolders);
           this.groupBox1.Controls.Add(this.btnLogBrowse);
           this.groupBox1.Controls.Add(this.tbLogFilePath);
           this.groupBox1.Controls.Add(this.label2);
           this.groupBox1.Controls.Add(this.tbBVTFolder);
           this.groupBox1.Controls.Add(this.btnBVTBrowse);
           this.groupBox1.Controls.Add(this.label1);
           this.groupBox1.Location = new System.Drawing.Point(12, 2);
           this.groupBox1.Name = "groupBox1";
           this.groupBox1.Size = new System.Drawing.Size(424, 128);
           this.groupBox1.TabIndex = 0;
           this.groupBox1.TabStop = false;
           this.groupBox1.Text = "BVT Options";
           // 
           // chkPri0
           // 
           this.chkPri0.AutoSize = true;
           this.chkPri0.Location = new System.Drawing.Point(10, 105);
           this.chkPri0.Name = "chkPri0";
           this.chkPri0.Size = new System.Drawing.Size(116, 17);
           this.chkPri0.TabIndex = 1;
           this.chkPri0.Text = "Only log pri 0 errors";
           this.chkPri0.UseVisualStyleBackColor = true;
           this.chkPri0.CheckedChanged += new System.EventHandler(this.chkPri0_CheckedChanged);
           // 
           // chkSubFolders
           // 
           this.chkSubFolders.AutoSize = true;
           this.chkSubFolders.Location = new System.Drawing.Point(10, 77);
           this.chkSubFolders.Name = "chkSubFolders";
           this.chkSubFolders.Size = new System.Drawing.Size(120, 17);
           this.chkSubFolders.TabIndex = 6;
           this.chkSubFolders.Text = "Include Sub Folders";
           this.chkSubFolders.UseVisualStyleBackColor = true;
           this.chkSubFolders.CheckedChanged += new System.EventHandler(this.chkSubFolders_CheckedChanged);
           // 
           // btnLogBrowse
           // 
           this.btnLogBrowse.Location = new System.Drawing.Point(96, 42);
           this.btnLogBrowse.Name = "btnLogBrowse";
           this.btnLogBrowse.Size = new System.Drawing.Size(44, 19);
           this.btnLogBrowse.TabIndex = 5;
           this.btnLogBrowse.Text = "...";
           this.btnLogBrowse.UseVisualStyleBackColor = true;
           this.btnLogBrowse.Click += new System.EventHandler(this.btnLogBrowse_Click);
           // 
           // tbLogFilePath
           // 
           this.tbLogFilePath.Location = new System.Drawing.Point(146, 42);
           this.tbLogFilePath.Name = "tbLogFilePath";
           this.tbLogFilePath.Size = new System.Drawing.Size(272, 20);
           this.tbLogFilePath.TabIndex = 4;
           this.tbLogFilePath.TextChanged += new System.EventHandler(this.tbLogFilePath_TextChanged);
           // 
           // label2
           // 
           this.label2.AutoSize = true;
           this.label2.Location = new System.Drawing.Point(6, 42);
           this.label2.Name = "label2";
           this.label2.Size = new System.Drawing.Size(84, 13);
           this.label2.TabIndex = 3;
           this.label2.Text = "Log file location:";
           // 
           // tbBVTFolder
           // 
           this.tbBVTFolder.Location = new System.Drawing.Point(146, 12);
           this.tbBVTFolder.Name = "tbBVTFolder";
           this.tbBVTFolder.Size = new System.Drawing.Size(272, 20);
           this.tbBVTFolder.TabIndex = 2;
           this.tbBVTFolder.TextChanged += new System.EventHandler(this.tbBVTFolder_TextChanged);
           // 
           // btnBVTBrowse
           // 
           this.btnBVTBrowse.Location = new System.Drawing.Point(96, 13);
           this.btnBVTBrowse.Name = "btnBVTBrowse";
           this.btnBVTBrowse.Size = new System.Drawing.Size(44, 19);
           this.btnBVTBrowse.TabIndex = 1;
           this.btnBVTBrowse.Text = "...";
           this.btnBVTBrowse.UseVisualStyleBackColor = true;
           this.btnBVTBrowse.Click += new System.EventHandler(this.btnBVTBrowse_Click);
           // 
           // label1
           // 
           this.label1.AutoSize = true;
           this.label1.Location = new System.Drawing.Point(6, 16);
           this.label1.Name = "label1";
           this.label1.Size = new System.Drawing.Size(84, 13);
           this.label1.TabIndex = 0;
           this.label1.Text = "BVT Test folder:";
           // 
           // groupBox2
           // 
           this.groupBox2.Controls.Add(this.chkFXCVT);
           this.groupBox2.Location = new System.Drawing.Point(12, 136);
           this.groupBox2.Name = "groupBox2";
           this.groupBox2.Size = new System.Drawing.Size(424, 83);
           this.groupBox2.TabIndex = 1;
           this.groupBox2.TabStop = false;
           this.groupBox2.Text = "Startup options";
           // 
           // chkFXCVT
           // 
           this.chkFXCVT.AutoSize = true;
           this.chkFXCVT.Location = new System.Drawing.Point(6, 19);
           this.chkFXCVT.Name = "chkFXCVT";
           this.chkFXCVT.Size = new System.Drawing.Size(161, 17);
           this.chkFXCVT.TabIndex = 0;
           this.chkFXCVT.Text = "Convert directly through xcvt";
           this.chkFXCVT.UseVisualStyleBackColor = true;
           this.chkFXCVT.CheckedChanged += new System.EventHandler(this.chkFXCVT_CheckedChanged);
           // 
           // btnOK
           // 
           this.btnOK.Location = new System.Drawing.Point(361, 230);
           this.btnOK.Name = "btnOK";
           this.btnOK.Size = new System.Drawing.Size(75, 23);
           this.btnOK.TabIndex = 2;
           this.btnOK.Text = "OK";
           this.btnOK.UseVisualStyleBackColor = true;
           this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
           // 
           // btnCancel
           // 
           this.btnCancel.Location = new System.Drawing.Point(280, 230);
           this.btnCancel.Name = "btnCancel";
           this.btnCancel.Size = new System.Drawing.Size(75, 23);
           this.btnCancel.TabIndex = 3;
           this.btnCancel.Text = "Cancel";
           this.btnCancel.UseVisualStyleBackColor = true;
           this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
           // 
           // Options
           // 
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.ClientSize = new System.Drawing.Size(448, 265);
           this.Controls.Add(this.btnCancel);
           this.Controls.Add(this.btnOK);
           this.Controls.Add(this.groupBox2);
           this.Controls.Add(this.groupBox1);
           this.Name = "Options";
           this.Text = "Options";
           this.Load += new System.EventHandler(this.Options_Load);
           this.groupBox1.ResumeLayout(false);
           this.groupBox1.PerformLayout();
           this.groupBox2.ResumeLayout(false);
           this.groupBox2.PerformLayout();
           this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbBVTFolder;
        private System.Windows.Forms.Button btnBVTBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkFXCVT;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnLogBrowse;
        private System.Windows.Forms.TextBox tbLogFilePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkSubFolders;
       private System.Windows.Forms.CheckBox chkPri0;
    }

}


