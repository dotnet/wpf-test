// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CountTests
{
	using Common;

	public class CCountTests : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label _labelFolder;
		private System.Windows.Forms.Button _button1;
		private System.Windows.Forms.Label _labelNumberTests;
		private System.Windows.Forms.Label _labelNumberFolders;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container _components = null;

		public CCountTests (string strFolder, int nDocuments, int nSubfolders, int [,] rgCount )
		{
			int kPage;
			int kLanguage;
			int MaxX = 0, MaxY = 0;
			int dyTable;

			// Required for Windows Form Designer support
			InitializeComponent();

			_labelFolder.Text += strFolder;
			_labelNumberTests.Text += nDocuments.ToString ();
			_labelNumberFolders.Text += nSubfolders.ToString ();

			dyTable = 110;

			// Initialize language labels
			for (kLanguage = 0; kLanguage < KLanguage.Other; kLanguage++)
			{
				System.Windows.Forms.Label label = new System.Windows.Forms.Label();

				label.Location = new System.Drawing.Point(16, dyTable + 40 + kLanguage * 30);
				label.Name = "";
				label.Size = new System.Drawing.Size(72, 24);
				label.TabIndex = 1;
				label.Text = KLanguage.ToPresentationString (kLanguage);
				label.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

				MaxY = Math.Max (MaxY, label.Location.Y + label.Size.Height);

				this.Controls.Add(label);
			}

			// Initialize page labels
			for (kPage = 0; kPage < KPage.Range; kPage++)
			{
				string strLabel = KPage.ToPresentationString (kPage);
				string s1, s2;
				System.Windows.Forms.Label label = new System.Windows.Forms.Label();

				Common.FSplitString (strLabel, '.', out s1, out s2);

				if (s1!=null)
				{
					strLabel = s1 + "\n" + s2;
				}

				label.Location = new System.Drawing.Point(90 + kPage * 80, dyTable);
				label.Name = "";
				label.Size = new System.Drawing.Size(70, 35);
				label.TabIndex = 1;
				label.Text = strLabel;
				label.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

				// this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

				MaxX = Math.Max (MaxX, label.Location.X + label.Size.Width );

				this.Controls.Add(label);
			}

			// Numbers
			for (kLanguage = 0; kLanguage < KLanguage.Other; kLanguage++)
			{
				for (kPage = 0; kPage < KPage.Range; kPage++)
				{
					System.Windows.Forms.Label label = new System.Windows.Forms.Label();

					label.Location = new System.Drawing.Point(90 + kPage * 80, dyTable + 40 + kLanguage * 30);
					label.Name = "";
					label.Size = new System.Drawing.Size(72, 24);
					label.TabIndex = 1;
					label.Text = rgCount [kLanguage, kPage].ToString ();
					label.Font = new System.Drawing.Font("Courier", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

					this.Controls.Add(label);
				}
			}

			_button1.Location = new Point (MaxX - _button1.Width, _button1.Location.Y);
			this.Width = MaxX + 25;
			this.Height = MaxY + 45;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(_components != null)
				{
					_components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._labelFolder = new System.Windows.Forms.Label();
			this._button1 = new System.Windows.Forms.Button();
			this._labelNumberTests = new System.Windows.Forms.Label();
			this._labelNumberFolders = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelFolder
			// 
			this._labelFolder.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelFolder.Location = new System.Drawing.Point(16, 24);
			this._labelFolder.Name = "labelFolder";
			this._labelFolder.Size = new System.Drawing.Size(432, 24);
			this._labelFolder.TabIndex = 0;
			this._labelFolder.Text = "Folder: ";
			// 
			// button1
			// 
			this._button1.Location = new System.Drawing.Point(552, 24);
			this._button1.Name = "button1";
			this._button1.Size = new System.Drawing.Size(88, 28);
			this._button1.TabIndex = 2;
			this._button1.Text = "OK";
			this._button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// labelNumberTests
			// 
			this._labelNumberTests.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelNumberTests.Location = new System.Drawing.Point(16, 47);
			this._labelNumberTests.Name = "labelNumberTests";
			this._labelNumberTests.Size = new System.Drawing.Size(432, 24);
			this._labelNumberTests.TabIndex = 3;
			this._labelNumberTests.Text = "Number of documents: ";
			// 
			// labelNumberFolders
			// 
			this._labelNumberFolders.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelNumberFolders.Location = new System.Drawing.Point(16, 71);
			this._labelNumberFolders.Name = "labelNumberFolders";
			this._labelNumberFolders.Size = new System.Drawing.Size(432, 24);
			this._labelNumberFolders.TabIndex = 4;
			this._labelNumberFolders.Text = "Number of test subfolders: ";
			// 
			// CCountTests
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(730, 328);
			this.Controls.Add(this._labelNumberFolders);
			this.Controls.Add(this._labelNumberTests);
			this.Controls.Add(this._button1);
			this.Controls.Add(this._labelFolder);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CCountTests";
			this.Text = "Test count";
			this.ResumeLayout(false);

		}
		#endregion

		// User clicks OK button
		private void button1_Click(object sender, System.EventArgs e)
		{
			Close ();		
		}
	}
}