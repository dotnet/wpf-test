// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace DocumentFile
{
	using Common;
	using Ask;

	public class CDocumentFileDialog : System.Windows.Forms.Form
	{
		/* Display variables */
		bool _fAllowDataExchange;
		bool _fCopyDirectoryStructure;
		bool _fRenameDocuments;
		string _strRenamePrefix;
		string _strFolder;

		/* Current parameters */
		KFileAction _kfaction;
		string [] _rgfnDocuments;
		string _spathTestRoot;

		/* Result of the dialog */
		bool _fDeletedFiles;

		/* Designer variables */
		private System.Windows.Forms.Label _label1;
		private System.Windows.Forms.Label _label3;
		private System.Windows.Forms.Panel _panel1;
		private System.Windows.Forms.Label _label4;
		private System.Windows.Forms.CheckBox _checkBoxRename;
		private System.Windows.Forms.Button _buttonSelectFolder;
		private System.Windows.Forms.ComboBox _comboBoxFolder;
		private System.Windows.Forms.Button _buttonReviewList;
		private System.Windows.Forms.CheckBox _checkBoxDirectoryStructure;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.TextBox _textBoxRenamePrefix;
		private System.Windows.Forms.Label _labelNumberOfDocuments;
		private System.Windows.Forms.Button _buttonCopyMoveDelete;

		private System.ComponentModel.Container _components = null;

		public CDocumentFileDialog()
		{
			_fAllowDataExchange = true;
			InitializeComponent(); // Required for Windows Form Designer support
		}

		public void FRunDialog (string spathTestRoot, KFileAction kfaction, string [] rgfnDocuments, out bool fDeletedFiles)
		{
			/* Unitialize display variables */
			_fCopyDirectoryStructure = false;
			_fRenameDocuments = false;
			_strRenamePrefix = "Doc";
			_strFolder = GetComboBoxHistoryMostRecent (_comboBoxFolder);
			this._kfaction = kfaction;
			this._rgfnDocuments = rgfnDocuments;
			this._fDeletedFiles = false;
			this._spathTestRoot = spathTestRoot;

			Update (false);

			_labelNumberOfDocuments.Text = "You selected: " + rgfnDocuments.Length.ToString () + " documents";
			_buttonCopyMoveDelete.Text = FileActionToString (kfaction);

			ShowDialog ();

			fDeletedFiles = this._fDeletedFiles;
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
			this._checkBoxRename = new System.Windows.Forms.CheckBox();
			this._textBoxRenamePrefix = new System.Windows.Forms.TextBox();
			this._label1 = new System.Windows.Forms.Label();
			this._buttonSelectFolder = new System.Windows.Forms.Button();
			this._comboBoxFolder = new System.Windows.Forms.ComboBox();
			this._buttonCopyMoveDelete = new System.Windows.Forms.Button();
			this._labelNumberOfDocuments = new System.Windows.Forms.Label();
			this._label3 = new System.Windows.Forms.Label();
			this._buttonReviewList = new System.Windows.Forms.Button();
			this._checkBoxDirectoryStructure = new System.Windows.Forms.CheckBox();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._panel1 = new System.Windows.Forms.Panel();
			this._label4 = new System.Windows.Forms.Label();
			this._panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBoxRename
			// 
			this._checkBoxRename.Location = new System.Drawing.Point(16, 148);
			this._checkBoxRename.Name = "checkBoxRename";
			this._checkBoxRename.Size = new System.Drawing.Size(136, 24);
			this._checkBoxRename.TabIndex = 0;
			this._checkBoxRename.Text = "Rename documents:";
			this._checkBoxRename.CheckedChanged += new System.EventHandler(this.EditingChange);
			// 
			// textBoxRenamePrefix
			// 
			this._textBoxRenamePrefix.Location = new System.Drawing.Point(144, 149);
			this._textBoxRenamePrefix.Name = "textBoxRenamePrefix";
			this._textBoxRenamePrefix.Size = new System.Drawing.Size(112, 20);
			this._textBoxRenamePrefix.TabIndex = 1;
			this._textBoxRenamePrefix.Text = "Doc";
			this._textBoxRenamePrefix.TextChanged += new System.EventHandler(this.EditingChange);
			// 
			// label1
			// 
			this._label1.Location = new System.Drawing.Point(264, 152);
			this._label1.Name = "label1";
			this._label1.Size = new System.Drawing.Size(104, 24);
			this._label1.TabIndex = 2;
			this._label1.Text = "+ number ( 1, 2, ... )";
			// 
			// buttonSelectFolder
			// 
			this._buttonSelectFolder.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonSelectFolder.Location = new System.Drawing.Point(12, 80);
			this._buttonSelectFolder.Name = "buttonSelectFolder";
			this._buttonSelectFolder.Size = new System.Drawing.Size(26, 20);
			this._buttonSelectFolder.TabIndex = 3;
			this._buttonSelectFolder.Text = "...";
			this._buttonSelectFolder.Click += new System.EventHandler(this.buttonSelectFolder_Click);
			// 
			// comboBoxFolder
			// 
			this._comboBoxFolder.Location = new System.Drawing.Point(48, 80);
			this._comboBoxFolder.Name = "comboBoxFolder";
			this._comboBoxFolder.Size = new System.Drawing.Size(312, 21);
			this._comboBoxFolder.TabIndex = 4;
			this._comboBoxFolder.Text = "C:\\E\\PTS\\Documents";
			this._comboBoxFolder.TextChanged += new System.EventHandler(this.EditingChange);
			// 
			// buttonCopyMoveDelete
			// 
			this._buttonCopyMoveDelete.Location = new System.Drawing.Point(376, 16);
			this._buttonCopyMoveDelete.Name = "buttonCopyMoveDelete";
			this._buttonCopyMoveDelete.Size = new System.Drawing.Size(88, 28);
			this._buttonCopyMoveDelete.TabIndex = 5;
			this._buttonCopyMoveDelete.Text = "Copy";
			this._buttonCopyMoveDelete.Click += new System.EventHandler(this.buttonCopyMoveDelete_Click);
			// 
			// labelNumberOfDocuments
			// 
			this._labelNumberOfDocuments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelNumberOfDocuments.Location = new System.Drawing.Point(8, 16);
			this._labelNumberOfDocuments.Name = "labelNumberOfDocuments";
			this._labelNumberOfDocuments.Size = new System.Drawing.Size(184, 20);
			this._labelNumberOfDocuments.TabIndex = 8;
			this._labelNumberOfDocuments.Text = "You selected: 22 documents";
			// 
			// label3
			// 
			this._label3.Location = new System.Drawing.Point(8, 48);
			this._label3.Name = "label3";
			this._label3.Size = new System.Drawing.Size(216, 16);
			this._label3.TabIndex = 9;
			this._label3.Text = "Destination folder:";
			// 
			// buttonReviewList
			// 
			this._buttonReviewList.Location = new System.Drawing.Point(272, 16);
			this._buttonReviewList.Name = "buttonReviewList";
			this._buttonReviewList.Size = new System.Drawing.Size(88, 28);
			this._buttonReviewList.TabIndex = 10;
			this._buttonReviewList.Text = "Review list";
			this._buttonReviewList.Click += new System.EventHandler(this.buttonReviewList_Click);
			// 
			// checkBoxDirectoryStructure
			// 
			this._checkBoxDirectoryStructure.Location = new System.Drawing.Point(16, 120);
			this._checkBoxDirectoryStructure.Name = "checkBoxDirectoryStructure";
			this._checkBoxDirectoryStructure.Size = new System.Drawing.Size(184, 24);
			this._checkBoxDirectoryStructure.TabIndex = 11;
			this._checkBoxDirectoryStructure.Text = "Preserve directory structure";
			this._checkBoxDirectoryStructure.CheckedChanged += new System.EventHandler(this.EditingChange);
			// 
			// buttonCancel
			// 
			this._buttonCancel.Location = new System.Drawing.Point(376, 56);
			this._buttonCancel.Name = "buttonCancel";
			this._buttonCancel.Size = new System.Drawing.Size(88, 28);
			this._buttonCancel.TabIndex = 12;
			this._buttonCancel.Text = "Cancel";
			this._buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// panel1
			// 
			this._panel1.BackColor = System.Drawing.Color.Silver;
			this._panel1.Controls.Add(this._label4);
			this._panel1.Location = new System.Drawing.Point(0, 192);
			this._panel1.Name = "panel1";
			this._panel1.Size = new System.Drawing.Size(480, 24);
			this._panel1.TabIndex = 13;
			// 
			// label4
			// 
			this._label4.Location = new System.Drawing.Point(12, 6);
			this._label4.Name = "label4";
			this._label4.Size = new System.Drawing.Size(204, 16);
			this._label4.TabIndex = 0;
			this._label4.Text = "When ready, click on Copy";
			// 
			// CDocumentFileDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(474, 216);
			this.Controls.Add(this._panel1);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._checkBoxDirectoryStructure);
			this.Controls.Add(this._buttonReviewList);
			this.Controls.Add(this._label3);
			this.Controls.Add(this._labelNumberOfDocuments);
			this.Controls.Add(this._buttonCopyMoveDelete);
			this.Controls.Add(this._comboBoxFolder);
			this.Controls.Add(this._buttonSelectFolder);
			this.Controls.Add(this._label1);
			this.Controls.Add(this._textBoxRenamePrefix);
			this.Controls.Add(this._checkBoxRename);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "CDocumentFileDialog";
			this.Text = "Document Mover Dialog";
			this._panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void Update (bool fEditingChange)
		{
			if (_fAllowDataExchange)
			{
				_fAllowDataExchange = false;

				if (fEditingChange)
				{
					if (_checkBoxDirectoryStructure.Enabled) _fCopyDirectoryStructure = _checkBoxDirectoryStructure.Checked;
					if (_checkBoxRename.Enabled) _fRenameDocuments = _checkBoxRename.Checked;
					if (_comboBoxFolder.Enabled) _strFolder = _comboBoxFolder.Text;
					if (_textBoxRenamePrefix.Enabled) _strRenamePrefix = _textBoxRenamePrefix.Text;
				}

				_checkBoxDirectoryStructure.Checked = _fCopyDirectoryStructure;
				_checkBoxDirectoryStructure.Enabled = _kfaction != KFileAction.Delete && _kfaction != KFileAction.Rename;
				_checkBoxRename.Checked = _fRenameDocuments || _kfaction == KFileAction.Rename;
				_checkBoxRename.Enabled = _kfaction != KFileAction.Delete /* && kfaction != KFileAction.Rename */;
				_comboBoxFolder.Enabled = _kfaction != KFileAction.Delete && _kfaction != KFileAction.Rename;

				if (_comboBoxFolder.Text != _strFolder) 
				{
					_comboBoxFolder.Text = _strFolder;
				};

				_textBoxRenamePrefix.Enabled = _kfaction != KFileAction.Delete;

				if (_textBoxRenamePrefix.Text != _strRenamePrefix) 
				{
					_textBoxRenamePrefix.Text = _strRenamePrefix;
				};

				_textBoxRenamePrefix.Enabled = _kfaction == KFileAction.Rename || (_fRenameDocuments && _kfaction != KFileAction.Delete);

				_buttonSelectFolder.Enabled = _kfaction != KFileAction.Delete && _kfaction != KFileAction.Rename;

				_fAllowDataExchange = true;
			}
		}

		private void EditingChange(object sender, System.EventArgs e)
		{
			Update (true);		
		}

		private void buttonReviewList_Click(object sender, System.EventArgs e)
		{
			string fnFileList = Path.GetTempFileName ();

			try
			{
				StreamWriter writer = new StreamWriter (fnFileList, false);
				int i;

				writer.WriteLine ("*******************************************************************");
				writer.WriteLine ("*                                                                 *");
				writer.WriteLine ("* If you need to change set of files for this operation,	make     *");
				writer.WriteLine ("* different selection in the test list, but do not edit this file *");
				writer.WriteLine ("*                                                                 *");
				writer.WriteLine ("*******************************************************************");
				writer.WriteLine ("");

				for (i=0; i < _rgfnDocuments.Length; i++)
				{
					writer.WriteLine (_rgfnDocuments [i]);
				};

				writer.Close ();
			}
			catch { W11Messages.ShowError ("Can not create temporary file with document list"); return; };

			try
			{
				/* Open notepad and do not wait till exit */
				Process processNotepad = Process.Start ("notepad.exe", fnFileList);
				processNotepad.WaitForExit ();
			}
			catch { W11Messages.ShowError ("Can not start Notepad.exe"); return; };

			try 
			{
				File.Delete (fnFileList);
			}
			catch { W11Messages.ShowError ("Can not delete temporary file " + fnFileList); return; };
		}


		private void buttonCopyMoveDelete_Click(object sender, System.EventArgs e)
		{
			if (_kfaction == KFileAction.Delete)
			{
				CommandDeleteCore ();
			}
			else if (_kfaction == KFileAction.Rename)
			{
				CommandRenameCore ();
			}
			else
			{
				CommandCopyMoveCore ();
			};

			AddFolderToComboBoxHistory (_comboBoxFolder, _strFolder);
			Close ();
		}

		private void CommandDeleteCore ()
		{
			int nFiles = _rgfnDocuments.Length;
			int nFilesProcessed = 0;
			int i;
			bool fCompleted = false;

			Ask.ResetQuestion (KQuestion.DeleteOriginalConfirmation );

			try
			{
				for (i=0; i < nFiles; i++)
				{
					string sfnSource = _rgfnDocuments [i];
					int attr = (int) File.GetAttributes (sfnSource);

					if ((attr & (int) FileAttributes.ReadOnly) != 0)
					{
						W11Messages.RaiseError ("File " + sfnSource + " is ReadOnly, can not delete");
					};

					if (Ask.YesNoDialog (KQuestion.DeleteOriginalConfirmation, "Do you want to delete " + sfnSource, true))
					{
						_fDeletedFiles = true;
						try { File.Delete (sfnSource);}
						catch { W11Messages.RaiseError ("Can not delete " + sfnSource);};
					}																												   

					nFilesProcessed++;
				}
				fCompleted = true;

			}
			catch (W11Exception) { /* already handled */ }
			catch (Exception ex) { W11Messages.ShowError ("Unknown error" + ex.ToString ());};

			if (!fCompleted)
			{
				W11Messages.ShowWarning ("Delete interrupted\n\nProcessed: " + nFilesProcessed.ToString () + " documents");
			}
		}

		private void CommandRenameCore ()
		{
			int nFiles = _rgfnDocuments.Length;
			int nFilesProcessed = 0;
			int i;
			bool fCompleted = false;
			string [] rgfnDocumentsNew = new string [nFiles];

			Ask.ResetQuestion (KQuestion.RenameReadOnlyConfirmation);

			for (i=0; i < nFiles; i++)
			{
				string sfnSource = _rgfnDocuments [i];
				rgfnDocumentsNew [i] = Path.Combine (Path.GetDirectoryName (sfnSource), _strRenamePrefix + "_" + (i+1).ToString () + Path.GetExtension (sfnSource));

				if (File.Exists (rgfnDocumentsNew [i]))
				{
					W11Messages.ShowError ("File " + rgfnDocumentsNew [i] + " already exists, can not rename. No other files were renamed");
					return;
				}
			}

			try
			{
				for (i=0; i < nFiles; i++)
				{
					string sfnSource = _rgfnDocuments [i];
					string sfnSourceNew = rgfnDocumentsNew [i];
					int attr = (int) File.GetAttributes (sfnSource);

					if ((attr & (int) FileAttributes.ReadOnly) == 0 ||
						Ask.YesNoDialog (KQuestion.RenameReadOnlyConfirmation, "File " + sfnSource + " is read-only. Do you want to reaname?", true))
					{
						_fDeletedFiles = true;
						try { File.Move (sfnSource, sfnSourceNew);}
						catch { W11Messages.RaiseError ("Can not rename " + sfnSource + " => " + sfnSourceNew);};

						nFilesProcessed++;
					}
				}
				fCompleted = true;
			}
			catch (W11Exception) { /* already handled */ }
			catch (Exception ex) { W11Messages.ShowError ("Unknown error" + ex.ToString ());};

			if (!fCompleted)
			{
				W11Messages.ShowWarning ("Rename interrupted\n\nProcessed: " + nFilesProcessed.ToString () + " documents");
			}
		}

		private void CommandCopyMoveCore ()
		{
			int nFiles = _rgfnDocuments.Length;
			string [] rgfnDestinations = new string [nFiles];
			int i;
			bool fCompleted = false;
			int nFilesProcessed = 0;

			Ask.ResetQuestion (KQuestion.OverwriteCopyConfirmation);
			Ask.ResetQuestion (KQuestion.DeleteOriginalConfirmation );

			try
			{
				/* Check existence of destination folder */
				if (!Directory.Exists (_strFolder))
				{
					W11Messages.ShowError ("Can not find folder " + _strFolder);
					return;
				}

				/* Create destination names for every file, check overwite */
				for (i=0; i < nFiles; i++)
				{
					string sfnSource = _rgfnDocuments [i];
					string sfnDestination;
					string strDestinationName;

					if (_fRenameDocuments)
						strDestinationName = _strRenamePrefix + "_" + (i+1).ToString () + Path.GetExtension (sfnSource);
					else
						strDestinationName = Path.GetFileName (sfnSource);

					if (_fCopyDirectoryStructure)
					{
						string spathSource = Path.GetDirectoryName (sfnSource);
						string spathLocalPath;
   
						Common.Assert (Common.CompareStr (_spathTestRoot, spathSource .Substring (0, _spathTestRoot.Length)) == 0);

						if (spathSource .Length > _spathTestRoot.Length && spathSource [_spathTestRoot.Length] == '\\')
						{
							spathLocalPath = spathSource .Substring (_spathTestRoot.Length + 1);
						}
						else
						{
							spathLocalPath = spathSource.Substring (_spathTestRoot.Length);
						}

						sfnDestination = Path.Combine (Path.Combine (_strFolder, spathLocalPath), strDestinationName);
					}
					else 
					{
						sfnDestination = Path.Combine (_strFolder, strDestinationName);
					};

					rgfnDestinations [i] = sfnDestination;
				}

				/* Process files */
				for (i=0; i < nFiles; i++)
				{
					string sfnSource = _rgfnDocuments [i];
					string sfnDestination = rgfnDestinations [i];
					bool fIgnore = false;

					if (_kfaction == KFileAction.Move)
					{
						int attr = (int) File.GetAttributes (sfnSource);
						if ((attr & (int) FileAttributes.ReadOnly) != 0)
						{
							W11Messages.RaiseError ("File " + sfnSource + " is ReadOnly, can not move");
						};
					}

					if (File.Exists (sfnDestination))
					{
						if (Ask.YesNoDialog (KQuestion.OverwriteCopyConfirmation, "Destination file exists " + sfnDestination + "\n\nOverwrite?", true))
						{
							int attr = (int) File.GetAttributes (sfnDestination);

							if ((attr & (int) FileAttributes.ReadOnly) != 0)
							{
								W11Messages.RaiseError ("File " + sfnDestination + " is ReadOnly, can not delete");
							};

							try { File.Delete (sfnDestination);}
							catch { W11Messages.RaiseError ("Can not delete " + sfnDestination);};
						}
						else fIgnore = true;
					};

					if (!fIgnore)
					{
						if (_fCopyDirectoryStructure)
						{
							string spathDestination = Path.GetDirectoryName (sfnDestination);
							if (!Directory.Exists (spathDestination))
							{
								try
								{
									Directory.CreateDirectory (spathDestination);
								}
								catch { W11Messages.RaiseError ("Can not create folder " + spathDestination);};
							}
						}

						try 
						{ 
							if (_kfaction == KFileAction.Copy) 
							{
								int attr;
								File.Copy (sfnSource, sfnDestination, false);

								/* Cancel read-only attribute */
								attr = (int) File.GetAttributes (sfnDestination);
								if ((attr & (int) FileAttributes.ReadOnly) != 0)
								{
									File.SetAttributes (sfnDestination, (FileAttributes) (File.GetAttributes (sfnSource) & ~FileAttributes.ReadOnly));
								};
							}
							else if (_kfaction == KFileAction.Move) 
							{
								_fDeletedFiles = true;
								File.Move (sfnSource, sfnDestination);
							}
							else Common.Assert (false);
						}
						catch { W11Messages.RaiseError ("Can not " + FileActionToString (_kfaction) + sfnSource + " => " + sfnDestination);};
					};

					nFilesProcessed ++;
				}

				fCompleted = true;
			} 
			catch (W11Exception) { /* already handled */ }
			catch (Exception ex) { W11Messages.ShowError ("Unknown error" + ex.ToString ());};

			if (!fCompleted)
			{
				W11Messages.ShowWarning (FileActionToString (_kfaction) + " interrupted\n\nProcessed: " + nFilesProcessed.ToString () + " documents");
			}
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			_fDeletedFiles = false;
			AddFolderToComboBoxHistory (_comboBoxFolder, _strFolder);
			Close ();		
		}

		private void buttonSelectFolder_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog folderDialog = new FolderBrowserDialog ();

			if (_strFolder == "") folderDialog.SelectedPath = _spathTestRoot;
			else 
				folderDialog.SelectedPath = _strFolder;

			if (folderDialog.ShowDialog() == DialogResult.OK)
			{
				_strFolder = folderDialog.SelectedPath;
				Update (false);
			}
		}

		static void AddFolderToComboBoxHistory (System.Windows.Forms.ComboBox comboBox, string strfolder)
		{
			int ii;

			strfolder = strfolder.Trim ();

			// Check if query string already there
			for (ii=0; ii < comboBox.Items.Count; ii++)
			{
				if (strfolder == (string) comboBox.Items [ii])
				{
					/* Delete and insert in the beginning */
					comboBox.Items.Remove (comboBox.Items [ii]);
					comboBox.Items.Insert (0, strfolder);
					return;
				}
			}

			// Insert in the beginning of the list
			comboBox.Items.Insert (0, strfolder);
		}

		static string GetComboBoxHistoryMostRecent (System.Windows.Forms.ComboBox comboBox)
		{
			if (comboBox.Items.Count == 0) return "";
			else return (string) comboBox.Items [0];						   
		}

		private static string FileActionToString (KFileAction kf)
		{
			if (kf == KFileAction.Copy) return "Copy";
			else if (kf == KFileAction.Move) return "Move";
			else if (kf == KFileAction.Delete) return "Delete";
			else if (kf == KFileAction.Rename) return "Rename";
		{
				Common.Assert (false);
				return "";
			};
		}		 
	}

	public class DocumentFile
	{
		static CDocumentFileDialog s_dialog = null; /* Dialog window */

		static public void CopyMoveDelete (string spathTestRoot, KFileAction kfaction, string [] rgfnDocuments, out bool fDeletedFiles)
		{
			if (s_dialog == null)
			{
				s_dialog = new CDocumentFileDialog ();
			};

			s_dialog.FRunDialog (spathTestRoot, kfaction, rgfnDocuments, out fDeletedFiles);
		}
	}


	public enum KFileAction
	{
		Copy,
		Move,
		Rename,
		Delete
	}
}