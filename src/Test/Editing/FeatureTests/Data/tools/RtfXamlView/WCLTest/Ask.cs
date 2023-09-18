// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

namespace Ask
{
	using Common;

	class CAskForm : System.Windows.Forms.Form
	{
		// Class variables:

		int _kAskResult;
		bool _fAlways;

		private System.Windows.Forms.CheckBox _checkBox1;
		private System.Windows.Forms.Label _label1;
		private System.Windows.Forms.Button _buttonYes;
		private System.Windows.Forms.Button _buttonNo;
		private System.Windows.Forms.RichTextBox _richtextMessage;
		private System.Windows.Forms.GroupBox _groupBox1;
		private System.Windows.Forms.Button _buttonCancel;
		private System.ComponentModel.Container _components = null;

	
		public CAskForm (string strMsg, string strYes, string strNo, bool fAlwaysCheckbox)
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			_richtextMessage.Text = strMsg;

			if (!fAlwaysCheckbox) 
			{
				_checkBox1.Visible = false;
				Size = new Size (Size.Width, Size.Height - _checkBox1.Size.Height);
			};

			// HACK: How to autosize message box?
			if (strMsg.Length > 100)
			{
				int dy = _richtextMessage.Size.Height * 3 / 2;

				_richtextMessage.Size = new Size (_richtextMessage.Size.Width, _richtextMessage.Size.Height + dy);

				_buttonYes.Location = new Point (_buttonYes.Location.X, _buttonYes.Location.Y + dy);
				_buttonNo.Location = new Point (_buttonNo.Location.X, _buttonNo.Location.Y + dy);
				_buttonCancel.Location = new Point (_buttonCancel.Location.X, _buttonCancel.Location.Y + dy);
				_checkBox1.Location = new Point (_checkBox1.Location.X, _checkBox1.Location.Y + dy);

				_groupBox1.Size = new Size (_groupBox1.Size.Width, _groupBox1.Size.Height + dy);
				Size = new Size (Size.Width, Size.Height + dy);
			};

			_buttonYes.Text = strYes;
			_buttonNo.Text = strNo;

			_kAskResult = KAskResult.Undefined;

			_fAlways = false;
		}

		// Get dialog result
		public int GetAskResult ()
		{
			return _kAskResult;
		}

		public void OpenInNewThread ()
		{
			Thread threadAsk = new Thread (new ThreadStart (RunNewThreadCore));
		}

		private void RunNewThreadCore ()
		{
			ShowDialog ();
		}

		// Get Always result
		public bool GetFAlways ()
		{
			return _fAlways;
		}


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
			this._buttonYes = new System.Windows.Forms.Button();
			this._buttonNo = new System.Windows.Forms.Button();
			this._checkBox1 = new System.Windows.Forms.CheckBox();
			this._label1 = new System.Windows.Forms.Label();
			this._richtextMessage = new System.Windows.Forms.RichTextBox();
			this._groupBox1 = new System.Windows.Forms.GroupBox();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._groupBox1.SuspendLayout();
			this.SuspendLayout();

			// buttonYes
			this._buttonYes.Location = new System.Drawing.Point(88, 120);
			this._buttonYes.Name = "buttonYes";
			this._buttonYes.Size = new System.Drawing.Size(88, 24);
			this._buttonYes.TabIndex = 1;
			this._buttonYes.Text = "Yes";
			this._buttonYes.Click += new System.EventHandler(this.buttonYes_Click);

			// buttonNo
			this._buttonNo.Location = new System.Drawing.Point(207, 120);
			this._buttonNo.Name = "buttonNo";
			this._buttonNo.Size = new System.Drawing.Size(88, 24);
			this._buttonNo.TabIndex = 3;
			this._buttonNo.Text = "No";
			this._buttonNo.Click += new System.EventHandler(this.buttonNo_Click);
			// 
			// checkBox1
			// 
			this._checkBox1.Location = new System.Drawing.Point(88, 152);
			this._checkBox1.Name = "checkBox1";
			this._checkBox1.Size = new System.Drawing.Size(232, 24);
			this._checkBox1.TabIndex = 4;
			this._checkBox1.Text = "Do not ask me again";

			// label1
			this._label1.Font = new System.Drawing.Font("Algerian", 60F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label1.ForeColor = System.Drawing.Color.Orange;
			this._label1.Location = new System.Drawing.Point(8, 0);
			this._label1.Name = "label1";
			this._label1.Size = new System.Drawing.Size(64, 120);
			this._label1.TabIndex = 6;
			this._label1.Text = "?";

			// richtextMessage
			this._richtextMessage.BackColor = System.Drawing.SystemColors.Control;
			this._richtextMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._richtextMessage.Font = new System.Drawing.Font("Verdana Ref", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._richtextMessage.Location = new System.Drawing.Point(8, 16);
			this._richtextMessage.Name = "richtextMessage";
			this._richtextMessage.ReadOnly = true;
			this._richtextMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this._richtextMessage.Size = new System.Drawing.Size(304, 77);
			this._richtextMessage.TabIndex = 0;
			this._richtextMessage.Text = "";

			// groupBox1
			this._groupBox1.Controls.Add(this._richtextMessage);
			this._groupBox1.Location = new System.Drawing.Point(88, 8);
			this._groupBox1.Name = "groupBox1";
			this._groupBox1.Size = new System.Drawing.Size(320, 104);
			this._groupBox1.TabIndex = 7;
			this._groupBox1.TabStop = false;

			// buttonCancel
			this._buttonCancel.Location = new System.Drawing.Point(317, 120);
			this._buttonCancel.Name = "buttonCancel";
			this._buttonCancel.Size = new System.Drawing.Size(88, 24);
			this._buttonCancel.TabIndex = 2;
			this._buttonCancel.Text = "Cancel";
			this._buttonCancel.Visible = false;
			this._buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);

			// CAskForm
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(426, 184);
			this.Controls.Add(this._groupBox1);
			this.Controls.Add(this._buttonNo);
			this.Controls.Add(this._label1);
			this.Controls.Add(this._checkBox1);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._buttonYes);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CAskForm";
			this.Text = "W11 Builder Question";
			this._groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		// User clicks on YES
		private void buttonYes_Click(object sender, System.EventArgs e)
		{
			_kAskResult = KAskResult.Yes;
			_fAlways = _checkBox1.Checked;
			Close ();
		}

		// User clicks on NO
		private void buttonNo_Click(object sender, System.EventArgs e)
		{
			_kAskResult = KAskResult.No;
			_fAlways = _checkBox1.Checked;
			Close ();
		}

		// User clicks on CANCEL
		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			_kAskResult = KAskResult.Cancel;
			Close ();
		}
	}


	class AskAsync
	{
		private CAskForm _form;
		string _strMsg;
		string _strYes;
		string _strNo;
		bool _fAlwaysCheckbox;

		public AskAsync (string strMsg, string strYes, string strNo, bool fAlwaysCheckbox)
		{
			_strMsg = strMsg;
			_strYes = strYes;
			_strNo = strNo;
			_fAlwaysCheckbox = fAlwaysCheckbox;

			_form = null;

			Thread thread = new Thread (new ThreadStart (Run));
			thread.Start ();
		}

		void Run ()
		{
			lock (this)
			{
				_form = new CAskForm (_strMsg, _strYes, _strNo, _fAlwaysCheckbox);
			}
			_form.ShowDialog ();
		}

		public int /* KAskResult */ GetResult ()
		{
			if (_form == null) return KAskResult.Undefined;
			else return _form.GetAskResult ();
		}

		public void Close ()
		{
			lock (this)
			{
				_form.Close ();
				_form = null;				
			}
		}
	}


	// Class CAsk: yes, no, always dialog

	class Ask
	{
		public static bool fSilent = false;

		static bool [] s_rgfAlways = null;
		static bool [] s_rgfAlwaysYes = null;

		
		static public void Initialize ()
		{
			int kquest;

			s_rgfAlways = new bool [KQuestion.Range];
			s_rgfAlwaysYes = new bool [KQuestion.Range];

			for (kquest =0; kquest < KQuestion.Range; kquest++)
			{
				s_rgfAlways [kquest] = false;
				s_rgfAlwaysYes [kquest] = false;
			}
		}

		static public void ResetQuestion (int kQuestion)
		{
			s_rgfAlways [kQuestion] = false;
			s_rgfAlwaysYes [kQuestion] = false;
		}

		// Ask dialog with specified kAsk
		// Supports "always" checkbox
		static public bool YesNoDialog (int kQuestion, string strMsg, bool fDefault)
		{
			return YesNoDialog (kQuestion, strMsg, "Yes", "No", fDefault);
		}

		// Ask dialog with specified kAsk with yes/no modifiers
		// Supports "always" checkbox
		static public bool YesNoDialog (int kQuestion, string strMsg, string strYes, string strNo, bool fDefault)
		{
			CAskForm dlgAsk;
			int kaskResult;
			bool fAlways;

			Common.Assert (kQuestion >= 0 && kQuestion <= KQuestion.Range);

			if (s_rgfAlways [kQuestion])
			{
				// Already have always result, just return it
				return s_rgfAlwaysYes [kQuestion];
			}
			else
			{
				if (fSilent) return fDefault;

				// Do not have always result, ask
				dlgAsk = new CAskForm  (strMsg, strYes, strNo, true /* display always */);

				// Show dialog, get result
				dlgAsk.ShowDialog ();
				kaskResult = dlgAsk.GetAskResult ();
				fAlways = dlgAsk.GetFAlways ();
				dlgAsk.Dispose ();

				if (kaskResult == KAskResult.Cancel) W11Messages.RaiseError ();

				// Save always, if selected, but only 
				if ( fAlways && ( kaskResult == KAskResult.Yes || 
											   kaskResult == KAskResult.No ))
				{
					s_rgfAlways    [kQuestion] = true;
					s_rgfAlwaysYes [kQuestion] = kaskResult == KAskResult.Yes;
				}

				return kaskResult == KAskResult.Yes;
			};
		}

		// Ask dialog with specified string message
		// Note that this function does not support "always" checkbox
		static public bool YesNoDialog (string strMsg, bool fDefault)
		{
			DialogResult res;

			if (Ask.fSilent) return fDefault;

			do 
			{
				res = MessageBox.Show (strMsg, "W11Builder", MessageBoxButtons.YesNo, 
					MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			}
			while (res != DialogResult.Yes && res != DialogResult.No);

			return res == DialogResult.Yes;
		}

		// Ask dialog with specified string message & yes/no modifiers
		// Note that this function does not support "always" checkbox
		static public bool YesNoDialog (string strMsg, string strYes, string strNo, bool fDefault)
		{
			CAskForm dlgAsk;
			int kaskResult;

			if (fSilent) return fDefault;

			dlgAsk = new CAskForm  ( strMsg, strYes, strNo, false /* do not display always */);

			// Show dialog, get result
			dlgAsk.ShowDialog ();
			kaskResult = dlgAsk.GetAskResult ();

			dlgAsk.Dispose ();

			if (kaskResult == KAskResult.Cancel) W11Messages.RaiseError ();

			return kaskResult == KAskResult.Yes;
		}


	}


	// Enumerated questions for "always" checkbox

	class KQuestion
	{
		public static int WordTest = 0;
		public static int DeleteTemporaryDoc = 1;
		public static int DeleteRecursiveIni = 2;
		public static int DifferentLanguage = 3;
		public static int DeleteBogusDocumentIniFile = 3;
		public static int BogusResultEntry = 4;
		public static int OverwriteCopyConfirmation = 5;
		public static int DeleteOriginalConfirmation = 6;
		public static int RenameReadOnlyConfirmation = 7;
		public static int UnicodeFolderConfirmation = 8;
		public static int Range = 9;
	}


	class KAskResult
	{
		public static int Yes = 0;
		public static int No = 2;
		public static int Cancel = 4;
		public static int Undefined = 5;
	}
}
