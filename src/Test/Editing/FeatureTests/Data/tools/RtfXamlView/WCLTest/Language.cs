// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Language
{
	using Common;

	class CLanguageForm : System.Windows.Forms.Form
	{
		bool _fLanguageSelected;	/* If end-user has choosen to set new language */
		int _kLanguageSelected;	/* Selected language */
		int _kLanguageInitial;

		CLanguageSettings _lngsetInitial;

		System.Windows.Forms.Button [] _rgButtons;
		Color _colorButtonBackgroundDefault;

		bool _fAllowDialogDataExchange;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.Windows.Forms.Button _buttonOK;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.Label _labelEdit;
		private System.Windows.Forms.Label _labelInstall;
		private System.Windows.Forms.Button _buttonOther;
		private System.Windows.Forms.GroupBox _groupBox1;
		private System.Windows.Forms.Button _buttonEnglish;
		private System.Windows.Forms.Button _buttonJapanese;
		private System.Windows.Forms.Button _buttonArabic;
		private System.ComponentModel.Container _components = null;

		public CLanguageForm (CLanguageSettings lngsetInitial)
		{
			// Call designer-initialization code
			InitializeComponent();

			_fAllowDialogDataExchange = true;
			_fLanguageSelected = false;
			this._lngsetInitial = lngsetInitial;
			_kLanguageInitial = lngsetInitial.GetKLanguage ();
			_kLanguageSelected = _kLanguageInitial;

			Common.Assert (KLanguage.Range == 4);
			_rgButtons = new System.Windows.Forms.Button [KLanguage.Range];

			_rgButtons [KLanguage.Arabic] = _buttonArabic;
			_rgButtons [KLanguage.Japanese] = _buttonJapanese;
			_rgButtons [KLanguage.English] = _buttonEnglish;
			_rgButtons [KLanguage.Other] = _buttonOther;

			_colorButtonBackgroundDefault = _buttonArabic.BackColor;

			if (_kLanguageInitial != KLanguage.Other)
			{
				Size = new Size (Size.Width, Size.Height - _buttonOther.Height);
				_groupBox1.Height -= _buttonOther.Height;
				_labelEdit.Location = new Point (_labelEdit.Location.X, _labelEdit.Location.Y - _buttonOther.Height);
				_labelInstall.Location = new Point (_labelInstall.Location.X, _labelInstall.Location.Y - _buttonOther.Height);

				_buttonOther.Visible = false;
			}

			UpdateControlsAfterStatusChange ();
		}

		// This method updates dialog controls to reflect new status
		void UpdateControlsAfterStatusChange ()
		{
			bool fAllowDialogDataExchange_Saved = _fAllowDialogDataExchange;
			int kLanguage;
			string strEdit;
			CLanguageSettings lngsetCur;
			_fAllowDialogDataExchange = false;

			for (kLanguage = 0; kLanguage < KLanguage.Range; kLanguage++)
			{
				if (kLanguage == _kLanguageSelected)
				{
					_rgButtons [kLanguage].BackColor = Color.Lavender;
					_rgButtons [kLanguage].Enabled = false;
				}
				else
				{
					_rgButtons [kLanguage].BackColor = _colorButtonBackgroundDefault;
					_rgButtons [kLanguage].Enabled = (kLanguage != KLanguage.Other) || 
													_kLanguageInitial == KLanguage.Other;
				}
			}

			lngsetCur = (_kLanguageSelected == KLanguage.Other ? _lngsetInitial :
																new CLanguageSettings (_kLanguageSelected));

			strEdit = "Edit = ";
			for (int i=0; i < Math.Min (lngsetCur.rgidEdit.Length, 3); i++)
			{
				if (i>0) strEdit += ", ";
				strEdit += lngsetCur.rgidEdit [i].ToString ();
			};

			if (lngsetCur.rgidEdit.Length > 3) strEdit += ", etc.";

			_labelEdit.Text = strEdit;
			_labelInstall.Text = "Install = " + lngsetCur.idInstall.ToString ();

			_buttonOK.Focus ();

			Update ();

			_fAllowDialogDataExchange = fAllowDialogDataExchange_Saved;
		}

		// This method is called each time there is end-user change
		// anywhere on any of the dialog box controls. We update
		// status and redisplay controls
		private void EditingChange(object sender, System.EventArgs e)
		{
			if (_fAllowDialogDataExchange)
			{
			}
		}

		// Get selected language
		public bool FGetSelectedLanguage (out int kLanguage)
		{
			kLanguage = _kLanguageSelected;
			return _fLanguageSelected;
		}

		// User clicks on OK (SET) button
		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			_fLanguageSelected = true; /* End-user selected language */
			Close ();
		}

		// User clicks on Cancel button
		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Common.Assert (!_fLanguageSelected);
			Close ();
		}

		// User clicks on Arabic
		private void buttonArabic_Click(object sender, System.EventArgs e)
		{
			_kLanguageSelected = KLanguage.Arabic;
			UpdateControlsAfterStatusChange ();
		}

		// User clicks on English
		private void buttonEnglish_Click(object sender, System.EventArgs e)
		{
			_kLanguageSelected = KLanguage.English;
			UpdateControlsAfterStatusChange ();
		}

		// User clicks on Japanese
		private void buttonJapanese_Click(object sender, System.EventArgs e)
		{
			_kLanguageSelected = KLanguage.Japanese;
			UpdateControlsAfterStatusChange ();
		}

		// User clicks on Other
		private void buttonOther_Click(object sender, System.EventArgs e)
		{
			_kLanguageSelected = KLanguage.Other;
			UpdateControlsAfterStatusChange ();
		}


		// Destructor (designer code)
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

		// Designer-initialization code
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._buttonOK = new System.Windows.Forms.Button();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._labelEdit = new System.Windows.Forms.Label();
			this._labelInstall = new System.Windows.Forms.Label();
			this._buttonEnglish = new System.Windows.Forms.Button();
			this._buttonJapanese = new System.Windows.Forms.Button();
			this._buttonArabic = new System.Windows.Forms.Button();
			this._buttonOther = new System.Windows.Forms.Button();
			this._groupBox1 = new System.Windows.Forms.GroupBox();
			this._groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this._buttonOK.Location = new System.Drawing.Point(136, 16);
			this._buttonOK.Name = "buttonOK";
			this._buttonOK.Size = new System.Drawing.Size(88, 28);
			this._buttonOK.TabIndex = 0;
			this._buttonOK.Text = "OK";
			this._buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this._buttonCancel.Location = new System.Drawing.Point(136, 56);
			this._buttonCancel.Name = "buttonCancel";
			this._buttonCancel.Size = new System.Drawing.Size(88, 28);
			this._buttonCancel.TabIndex = 1;
			this._buttonCancel.Text = "Cancel";
			this._buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelEdit
			// 
			this._labelEdit.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelEdit.Location = new System.Drawing.Point(18, 181);
			this._labelEdit.Name = "labelEdit";
			this._labelEdit.Size = new System.Drawing.Size(192, 16);
			this._labelEdit.TabIndex = 14;
			this._labelEdit.Text = "Edit = ";
			// 
			// labelInstall
			// 
			this._labelInstall.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelInstall.Location = new System.Drawing.Point(18, 205);
			this._labelInstall.Name = "labelInstall";
			this._labelInstall.Size = new System.Drawing.Size(192, 16);
			this._labelInstall.TabIndex = 15;
			this._labelInstall.Text = "Install = ";
			// 
			// buttonEnglish
			// 
			this._buttonEnglish.Location = new System.Drawing.Point(13, 16);
			this._buttonEnglish.Name = "buttonEnglish";
			this._buttonEnglish.Size = new System.Drawing.Size(88, 28);
			this._buttonEnglish.TabIndex = 17;
			this._buttonEnglish.Text = "English";
			this._buttonEnglish.Click += new System.EventHandler(this.buttonEnglish_Click);
			// 
			// buttonJapanese
			// 
			this._buttonJapanese.Location = new System.Drawing.Point(13, 56);
			this._buttonJapanese.Name = "buttonJapanese";
			this._buttonJapanese.Size = new System.Drawing.Size(88, 28);
			this._buttonJapanese.TabIndex = 18;
			this._buttonJapanese.Text = "Japanese";
			this._buttonJapanese.Click += new System.EventHandler(this.buttonJapanese_Click);
			// 
			// buttonArabic
			// 
			this._buttonArabic.Location = new System.Drawing.Point(13, 96);
			this._buttonArabic.Name = "buttonArabic";
			this._buttonArabic.Size = new System.Drawing.Size(88, 28);
			this._buttonArabic.TabIndex = 0;
			this._buttonArabic.Text = "Arabic";
			this._buttonArabic.Click += new System.EventHandler(this.buttonArabic_Click);
			// 
			// buttonOther
			// 
			this._buttonOther.BackColor = System.Drawing.SystemColors.Control;
			this._buttonOther.Location = new System.Drawing.Point(13, 136);
			this._buttonOther.Name = "buttonOther";
			this._buttonOther.Size = new System.Drawing.Size(88, 28);
			this._buttonOther.TabIndex = 20;
			this._buttonOther.Text = "Other";
			this._buttonOther.Click += new System.EventHandler(this.buttonOther_Click);
			// 
			// groupBox1
			// 
			this._groupBox1.Controls.Add(this._buttonOK);
			this._groupBox1.Controls.Add(this._buttonArabic);
			this._groupBox1.Controls.Add(this._buttonOther);
			this._groupBox1.Controls.Add(this._buttonJapanese);
			this._groupBox1.Controls.Add(this._buttonEnglish);
			this._groupBox1.Controls.Add(this._labelInstall);
			this._groupBox1.Controls.Add(this._labelEdit);
			this._groupBox1.Controls.Add(this._buttonCancel);
			this._groupBox1.Location = new System.Drawing.Point(2, -2);
			this._groupBox1.Name = "groupBox1";
			this._groupBox1.Size = new System.Drawing.Size(244, 234);
			this._groupBox1.TabIndex = 21;
			this._groupBox1.TabStop = false;
			// 
			// CLanguageForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(248, 236);
			this.Controls.Add(this._groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "CLanguageForm";
			this.Text = " Choose office language";
			this.Load += new System.EventHandler(this.CLanguageForm_Load);
			this._groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void CLanguageForm_Load(object sender, System.EventArgs e)
		{
		
		}
	}

	class Language
	{
		// Choose language command
		public static void ChooseLanguageDialog (int kWord, out bool fChanged, out int kLanguageNew)
		{
			CLanguageSettings lsngsetCurrent;

			lsngsetCurrent = CLanguageSettings.GetOfficeLanguageFromRegistry (kWord);

			CLanguageForm form = new CLanguageForm ( lsngsetCurrent );

			/* Show language dialog box */
			form.ShowDialog ();

			fChanged = false;

			if (form.FGetSelectedLanguage (out kLanguageNew))
			{
				if (kLanguageNew != KLanguage.Other)
				{
					CLanguageSettings lsngsetNew = new CLanguageSettings (kLanguageNew);
					lsngsetNew.SetOfficeLanguageInRegistry (kWord);

					fChanged = true;
				}
			}
		}

	} // end of language services


	class CLanguageSettings
	{
		public int idInstall;
		public int [] rgidEdit;

		static string GetLanguageRegistryPath (int kWord)
		{
			return "Software\\Microsoft\\Office\\" + kWord.ToString () + ".0" + "\\Common\\LanguageResources";
		}

		// Constructor from KLanguage
		public CLanguageSettings (int kLanguage)
		{
			idInstall = 1033; /* For normal settings, idInstall is always English */

			if (kLanguage == KLanguage.English)
			{
				rgidEdit = new int [1] {1033};
			}
			else if (kLanguage == KLanguage.Japanese)
			{
				rgidEdit = new int [2] {1033, 1041};
			}
			else if (kLanguage == KLanguage.Arabic)
			{
				rgidEdit = new int [2] {1033, 1025};
			}
			else Common.Assert (false, "CLanguageSettings main constructor can not be used with <Other>");
		}

		// Constructor from raw data (private)
		CLanguageSettings (int idInstall, int [] rgidEdit)
		{
			this.idInstall = idInstall;
			this.rgidEdit = rgidEdit;
		}

		// What kind of language?
		public int GetKLanguage ()
		{
			if (idInstall == 1033)
			{
				if (rgidEdit.Length == 1 && rgidEdit [0] == 1033) return KLanguage.English;
				else if (rgidEdit.Length == 2)
				{
					int id1 = rgidEdit [0];
					int id2 = rgidEdit [1];

					if (id1 == 1033 && id2 == 1041 || id2 == 1033 && id1 == 1041)
					{
						return KLanguage.Japanese;
					}
					else if (id1 == 1033 && id2 == 1025 || id2 == 1033 && id1 == 1025)
					{
						return KLanguage.Arabic;
					}
				}
			};

			/* In all other case return Other */

			return KLanguage.Other;
		}

		// GetOfficeLanguageFromRegistry
		public static CLanguageSettings GetOfficeLanguageFromRegistry (int kWord)
		{
			string [] rgstrOLangSubkeys;
			int [] rgidEdit;
			int nEditOn;
			int iln;
			int idInstall;

			System.Collections.SortedList a = new SortedList (1);

			RegistryKey registryOLang = Registry.CurrentUser.OpenSubKey 
											( GetLanguageRegistryPath (kWord), false /* read-only access */ );

			if (registryOLang == null)
			{
				W11Messages.RaiseError ("Could not find office language registry: " + GetLanguageRegistryPath (kWord));
			}

			rgstrOLangSubkeys = registryOLang.GetValueNames ();

			/* Count number of edit languages */

			nEditOn = 0;

			foreach (string strSubkey in rgstrOLangSubkeys)
			{
				int idLang;
				if ( Common.ParseStrToInt (strSubkey, out idLang))
				{
					if (registryOLang.GetValue (strSubkey).ToString ().ToUpper () == "ON") nEditOn++;
				}
			}

			/* Allocated array */
			rgidEdit = new int [nEditOn];
			iln = 0;

			foreach (string strSubkey in rgstrOLangSubkeys)
			{
				int idLang;
				if ( Common.ParseStrToInt (strSubkey, out idLang))
				{
					if (registryOLang.GetValue (strSubkey).ToString ().ToUpper () == "ON")
					{
						rgidEdit [iln] = idLang;
						iln++;
					}
				}
			};

			/* Read install language */
			try
			{
				idInstall = (int) registryOLang.GetValue ("InstallLanguage", 1033);
			}
			catch
			{
				W11Messages.RaiseError ("Registry key <InstallLanguage> is not valid");
				idInstall = 0;
			};

			registryOLang.Close ();

			return new CLanguageSettings (idInstall, rgidEdit);
		}

		// Save language settings in the registry
		public void SetOfficeLanguageInRegistry (int kWord)
		{
			string [] rgstrOLangSubkeys;

			RegistryKey registryOLang = Registry.CurrentUser.OpenSubKey 
												( GetLanguageRegistryPath (kWord), true /* Request write access */ );

			if (registryOLang == null)
			{
				W11Messages.RaiseError ("Could not find office language registry: " + GetLanguageRegistryPath (kWord));
			}

			rgstrOLangSubkeys = registryOLang.GetValueNames ();

			/* Set all languages to off */
			foreach (string strSubkey in rgstrOLangSubkeys)
			{
				int idLang;

				if ( Common.ParseStrToInt (strSubkey, out idLang))
				{
					if (registryOLang.GetValue (strSubkey).ToString ().ToUpper () == "ON")
					{
						registryOLang.SetValue (strSubkey, "Off");
					}
				}
			}

			/* Set selected languages On */
			foreach (int idLang in rgidEdit)
			{
				registryOLang.SetValue (idLang.ToString (), "On");
			};

			/* Set install language */
			registryOLang.SetValue ("InstallLanguage", idInstall);

			registryOLang.Close ();
		}

		// Check language tune-up setting
		public static void CheckLanguageTuneUp (int kWord)
		{
			string strLangTuneUp;

			RegistryKey registryOLang = Registry.CurrentUser.OpenSubKey 
					( GetLanguageRegistryPath (kWord), true /* Request write access */ );

			if (registryOLang == null) /* Office language registry not found */
				return;

			/* Correct language tuneup option */
			strLangTuneUp = (string) registryOLang.GetValue ("LangTuneUp", "");

			if (strLangTuneUp.ToUpper () == "OFFICECOMPLETED")
			{
				registryOLang.SetValue ("LangTuneUp", "Prohibited");

				W11Messages.ShowWarning ( "W11Builder changed your Office language tune-up setting from " + strLangTuneUp + " to Prohibited. There is some information about it below.\n\n" +
					"With your old settings, when you start Word 11, it will look up installed input languages in Windows and it will turn on corresponding Edit languages. For example, if you have Japanese IME, Word will automatically set 1041 = ON. The new settings prevents Word from using smart language tune up.\n\n" +
					"You will not see this warning again, unless the value needs to be modified again\n\n" +
					"Registry key: " + GetLanguageRegistryPath (kWord) + "\\LangTuneUp");
			};

			registryOLang.Close ();
		}

	} // End of CLanguageSettings class
}