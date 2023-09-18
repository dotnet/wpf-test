// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QueryBuilder
{
	public class CQueryBuilderDialog : System.Windows.Forms.Form
	{
		/* Display variables */
		bool _fNot;
		KFilter _kfilter;

		/* Dialog result */
		bool _fSetQuery;

		private System.Windows.Forms.GroupBox _groupBox1;
		private System.Windows.Forms.Button _buttonSet;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.RadioButton _radioButtonErrors;
		private System.Windows.Forms.RadioButton _radioButtonMarked;
		private System.Windows.Forms.RadioButton _radioButtonTimeouts;
		private System.Windows.Forms.Label _labelQueryString;
		private System.Windows.Forms.RadioButton _radioButtonNone;
		private System.Windows.Forms.CheckBox _checkBoxNot;
		private System.Windows.Forms.RadioButton _radioButtonPageBreaks;
		private System.Windows.Forms.RadioButton _radioButtonUnicodeDocument;
		private System.Windows.Forms.RadioButton _radioButtonPageLineBreaks;
		/// Required designer variable.
		private System.ComponentModel.Container _components = null;

		public CQueryBuilderDialog()
		{
			InitializeComponent(); // Required for Windows Form Designer support

			this._fNot = false;
			this._kfilter = KFilter.None;
			this._fSetQuery = false;

			Update (false);
		}

		private void Update (bool fEditingChange)
		{
			string strQuery;

			if (fEditingChange)
			{
				if (_checkBoxNot.Enabled) this._fNot = _checkBoxNot.Checked;

				if (this._radioButtonErrors.Checked) this._kfilter = KFilter.Errors;
				else if (this._radioButtonMarked.Checked) this._kfilter = KFilter.Flagged;
				else if (this._radioButtonTimeouts.Checked) this._kfilter = KFilter.Timeouts;
				else if (this._radioButtonNone.Checked) this._kfilter = KFilter.None;
				else if (this._radioButtonPageBreaks.Checked) this._kfilter = KFilter.PageBreaks;
				else if (this._radioButtonPageLineBreaks.Checked) this._kfilter = KFilter.PageLineBreaks;
				else if (this._radioButtonUnicodeDocument.Checked) this._kfilter = KFilter.Unicode;
			}

			this._radioButtonErrors.Checked = this._kfilter == KFilter.Errors;
			this._radioButtonTimeouts.Checked = this._kfilter == KFilter.Timeouts;
			this._radioButtonMarked.Checked = this._kfilter == KFilter.Flagged;
			this._radioButtonNone.Checked = this._kfilter == KFilter.None;
			this._radioButtonPageBreaks.Checked = this._kfilter == KFilter.PageBreaks;
			this._radioButtonPageLineBreaks.Checked = this._kfilter == KFilter.PageLineBreaks;
			this._radioButtonUnicodeDocument.Checked = this._kfilter == KFilter.Unicode;

			this._checkBoxNot.Enabled = this._kfilter != KFilter.None;
			this._checkBoxNot.Checked = this._kfilter != KFilter.None && this._fNot;

			strQuery = GetQueryString (this._fNot, this._kfilter);
			this._labelQueryString.Text = " Query:  " + strQuery;
		}

		private void EditingChange(object sender, System.EventArgs e)
		{
			Update (true);		
		}

		private void buttonNot_Click(object sender, System.EventArgs e)
		{
			this._fNot = !this._fNot;
			Update (false);		
		}

		private void buttonSet_Click(object sender, System.EventArgs e)
		{
			this._fSetQuery = true;
			Close ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Close ();		
		}

		public bool FGetDialogResult (out string strQuery)
		{
			if (this._fSetQuery)
			{
				strQuery = GetQueryString (this._fNot, this._kfilter);
				return true;
			}
			else
			{
				strQuery = "";
				return false;
			}
		}

		private static string GetQueryString (bool fNot, KFilter kf)
		{
			string str;

			if (kf == KFilter.Errors) 
			{
				str = "Run " + (fNot ? "!=" : "=") + " \"Error\"";
			}
			else if (kf == KFilter.Flagged) 
			{
				str = (fNot ? "!X" : "X");
			}
			else if (kf == KFilter.Timeouts)
			{
				str = "Error " + (fNot ? "!~" : "~") + " \"Timeout\"";
			}
			else if (kf == KFilter.PageBreaks)
			{
				str = "Error " + (fNot ? "!~" : "~") + " \"Compare: Pri 0\"";
			}
			else if (kf == KFilter.PageLineBreaks)
			{
				str = "Error " + "~" + " \"Compare: Pri 0\"" + " || " + "Error " + "~" + " \"Compare: Pri 1\"";
				if (fNot) str = "!(" + str + ")";

			}
			else if (kf == KFilter.Unicode)
			{
				str = (fNot ? "!" : "") + "FUnicode (Document)";
			}
			else str = "";

			return str;
		}

		/// Clean up any resources being used.
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
			this._buttonSet = new System.Windows.Forms.Button();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._radioButtonErrors = new System.Windows.Forms.RadioButton();
			this._radioButtonMarked = new System.Windows.Forms.RadioButton();
			this._groupBox1 = new System.Windows.Forms.GroupBox();
			this._radioButtonPageLineBreaks = new System.Windows.Forms.RadioButton();
			this._radioButtonUnicodeDocument = new System.Windows.Forms.RadioButton();
			this._radioButtonPageBreaks = new System.Windows.Forms.RadioButton();
			this._radioButtonNone = new System.Windows.Forms.RadioButton();
			this._radioButtonTimeouts = new System.Windows.Forms.RadioButton();
			this._labelQueryString = new System.Windows.Forms.Label();
			this._checkBoxNot = new System.Windows.Forms.CheckBox();
			this._groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonSet
			// 
			this._buttonSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonSet.Location = new System.Drawing.Point(376, 21);
			this._buttonSet.Name = "buttonSet";
			this._buttonSet.Size = new System.Drawing.Size(50, 25);
			this._buttonSet.TabIndex = 6;
			this._buttonSet.Text = "Set";
			this._buttonSet.Click += new System.EventHandler(this.buttonSet_Click);
			// 
			// buttonCancel
			// 
			this._buttonCancel.Location = new System.Drawing.Point(376, 57);
			this._buttonCancel.Name = "buttonCancel";
			this._buttonCancel.Size = new System.Drawing.Size(50, 25);
			this._buttonCancel.TabIndex = 8;
			this._buttonCancel.Text = "Cancel";
			this._buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// radioButtonErrors
			// 
			this._radioButtonErrors.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._radioButtonErrors.Location = new System.Drawing.Point(12, 40);
			this._radioButtonErrors.Name = "radioButtonErrors";
			this._radioButtonErrors.Size = new System.Drawing.Size(80, 24);
			this._radioButtonErrors.TabIndex = 9;
			this._radioButtonErrors.Text = "Errors";
			this._radioButtonErrors.CheckedChanged += new System.EventHandler(this.EditingChange);
			// 
			// radioButtonMarked
			// 
			this._radioButtonMarked.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._radioButtonMarked.Location = new System.Drawing.Point(12, 133);
			this._radioButtonMarked.Name = "radioButtonMarked";
			this._radioButtonMarked.Size = new System.Drawing.Size(156, 24);
			this._radioButtonMarked.TabIndex = 10;
			this._radioButtonMarked.Text = "Flagged";
			this._radioButtonMarked.CheckedChanged += new System.EventHandler(this.EditingChange);
			// 
			// groupBox1
			// 
			this._groupBox1.Controls.Add(this._radioButtonPageLineBreaks);
			this._groupBox1.Controls.Add(this._radioButtonUnicodeDocument);
			this._groupBox1.Controls.Add(this._radioButtonPageBreaks);
			this._groupBox1.Controls.Add(this._radioButtonNone);
			this._groupBox1.Controls.Add(this._radioButtonTimeouts);
			this._groupBox1.Controls.Add(this._radioButtonMarked);
			this._groupBox1.Controls.Add(this._radioButtonErrors);
			this._groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._groupBox1.Location = new System.Drawing.Point(64, 14);
			this._groupBox1.Name = "groupBox1";
			this._groupBox1.Size = new System.Drawing.Size(296, 194);
			this._groupBox1.TabIndex = 11;
			this._groupBox1.TabStop = false;
			this._groupBox1.Text = "Choose query";
			// 
			// radioButtonPageLineBreaks
			// 
			this._radioButtonPageLineBreaks.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._radioButtonPageLineBreaks.Location = new System.Drawing.Point(12, 109);
			this._radioButtonPageLineBreaks.Name = "radioButtonPageLineBreaks";
			this._radioButtonPageLineBreaks.Size = new System.Drawing.Size(244, 24);
			this._radioButtonPageLineBreaks.TabIndex = 15;
			this._radioButtonPageLineBreaks.Text = "Pri 1 compare errors - page, line";
			// 
			// radioButtonUnicodeDocument
			// 
			this._radioButtonUnicodeDocument.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._radioButtonUnicodeDocument.Location = new System.Drawing.Point(12, 154);
			this._radioButtonUnicodeDocument.Name = "radioButtonUnicodeDocument";
			this._radioButtonUnicodeDocument.Size = new System.Drawing.Size(204, 27);
			this._radioButtonUnicodeDocument.TabIndex = 14;
			this._radioButtonUnicodeDocument.Text = "Documents with unicode names";
			this._radioButtonUnicodeDocument.CheckedChanged += new System.EventHandler(this.EditingChange);
			// 
			// radioButtonPageBreaks
			// 
			this._radioButtonPageBreaks.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._radioButtonPageBreaks.Location = new System.Drawing.Point(12, 86);
			this._radioButtonPageBreaks.Name = "radioButtonPageBreaks";
			this._radioButtonPageBreaks.Size = new System.Drawing.Size(220, 24);
			this._radioButtonPageBreaks.TabIndex = 13;
			this._radioButtonPageBreaks.Text = "Pri 0 compare errors - page";
			this._radioButtonPageBreaks.CheckedChanged += new System.EventHandler(this.EditingChange);
			// 
			// radioButtonNone
			// 
			this._radioButtonNone.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._radioButtonNone.Location = new System.Drawing.Point(12, 18);
			this._radioButtonNone.Name = "radioButtonNone";
			this._radioButtonNone.Size = new System.Drawing.Size(148, 24);
			this._radioButtonNone.TabIndex = 12;
			this._radioButtonNone.Text = "None - clear query";
			this._radioButtonNone.Click += new System.EventHandler(this.radioButtonNone_Click);
			this._radioButtonNone.CheckedChanged += new System.EventHandler(this.EditingChange);
			// 
			// radioButtonTimeouts
			// 
			this._radioButtonTimeouts.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._radioButtonTimeouts.Location = new System.Drawing.Point(12, 62);
			this._radioButtonTimeouts.Name = "radioButtonTimeouts";
			this._radioButtonTimeouts.Size = new System.Drawing.Size(112, 24);
			this._radioButtonTimeouts.TabIndex = 11;
			this._radioButtonTimeouts.Text = "Timeout errors";
			this._radioButtonTimeouts.CheckedChanged += new System.EventHandler(this.EditingChange);
			// 
			// labelQueryString
			// 
			this._labelQueryString.BackColor = System.Drawing.Color.Silver;
			this._labelQueryString.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelQueryString.Location = new System.Drawing.Point(0, 216);
			this._labelQueryString.Name = "labelQueryString";
			this._labelQueryString.Size = new System.Drawing.Size(456, 24);
			this._labelQueryString.TabIndex = 13;
			this._labelQueryString.Text = "Query = ";
			this._labelQueryString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBoxNot
			// 
			this._checkBoxNot.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._checkBoxNot.Location = new System.Drawing.Point(13, 15);
			this._checkBoxNot.Name = "checkBoxNot";
			this._checkBoxNot.Size = new System.Drawing.Size(56, 24);
			this._checkBoxNot.TabIndex = 14;
			this._checkBoxNot.Text = "NOT:";
			this._checkBoxNot.CheckedChanged += new System.EventHandler(this.EditingChange);
			// 
			// CQueryBuilderDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(442, 240);
			this.Controls.Add(this._labelQueryString);
			this.Controls.Add(this._groupBox1);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._buttonSet);
			this.Controls.Add(this._checkBoxNot);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "CQueryBuilderDialog";
			this.Text = "Query Builder";
			this._groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion

		private void radioButtonNone_Click(object sender, System.EventArgs e)
		{

		}
	}

	public class QueryBuilder
	{
		static public bool FShowDialog (out string strQuery)
		{
			CQueryBuilderDialog dialog = new CQueryBuilderDialog ();
			dialog.ShowDialog ();
			return dialog.FGetDialogResult (out strQuery);
		}
	}

	enum KFilter
	{
		None,
		Errors,
		Timeouts,
		Flagged,
		PageBreaks,
		PageLineBreaks,
		Unicode
	}
}