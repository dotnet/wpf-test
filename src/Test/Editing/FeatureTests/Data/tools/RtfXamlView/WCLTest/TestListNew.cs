// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;


namespace TestList
{
	using Location;
	using Protocol;
	using Common;
	using Ask;
	using Inifile;
	using Language;
	using Runword;
	using TestListDialog;

	class CTestList : IUIExchangeProc
	{
		public CTestList (CLocation OurLocation,
			CProtocol Protocol,
			CRunword runwordObject,
			string spathTestFolder,
			System.Windows.Forms.Form formParent, 
			Delegate delegateFinished )
		{
			// Required for Windows Form Designer support
		}

		void IUIExchangeProc.DoExchange (CUIExchangeData uiexdat, KUIExchange kux)
		{
			uiexdat.a = 0;
		}


		// FGetNextTestFolder
		public bool FGetNextTestFolder ( int iFolderPrev /* -1 for the first one */,
			out string spathDoc,
			out string spathLayout,
			out string spathLayoutTemp,
			out int kLanguage,
			out bool fSaveLayout,
			out int kPage,
			out string [] rgstrDocnames,
			out int [] rgiddoc,
			out int iFolderNew )
		{
			spathDoc = "";
			spathLayout= "";
			spathLayoutTemp = "";
			kLanguage = 0;
			fSaveLayout = false;
			kPage = 0;
			rgstrDocnames = null;
			rgiddoc = null;
			iFolderNew = 0;
			return false;
		}

		public bool FPrepareForTests (bool fTestAll)
		{
			return false;
		}

		public bool GetFRunTestsAfterClosing ()
		{
			return false;
		}

		public int GetNumberTestedDocument ()
		{
			return 0;
		}

		public void Run (int kTarget)
		{
			CTestListDialog testlistDialog = new CTestListDialog (this);

			testlistDialog.Show ();
		}

		public void SetDocumentTestResult (int iTask, int idoc, bool fRestart,
			int kRun, string strFlags, string strError)
		{
		}

		public void Terminate ()
		{
		}

		public void ChangeFolder (string str)
		{
		}
	}

	class KRun
	{
		public static int NotRun = 0;
		public static int OK = 1;
		public static int EQ = 2;
		public static int Error = 3;
		public static int Unfinished = 4;
		public static int Timeout = 5;

		public static bool IsError (int kRun)
		{
			return (kRun == KRun.Error || kRun == KRun.Timeout || kRun == KRun.Unfinished);
		}

		public static string ToPresentation (int kRun)
		{
			switch (kRun)
			{
			case 0: return "";
			case 1: return "OK";
			case 2: return "EQ";
			case 3: return "ERROR";
			case 4: return "Unfinished";
			case 5: return "Timeout";
			default: Common.Assert (false, "Invalid krun"); return "";
			}
		}

		public static int FromPresentation (string str)
		{
			str = str.ToUpper ();

			if (str == "") return KRun.NotRun;
			else if (str == "OK") return KRun.OK;
			else if (str == "EQ") return KRun.EQ;
			else if (str == "ERROR") return KRun.Error;
			else if (str == "UNFINISHED") return KRun.Unfinished;
			else if (str == "TIMEOUT") return KRun.Timeout;
			else Common.Assert (false, "Invalid krun"); return 0;
		}
	}

}


namespace TestListDialog
{
	class CTestListDialog : System.Windows.Forms.Form
	{
		IUIExchangeProc _exchangeObject;
		CUIExchangeData _exchangeData;

		private System.Windows.Forms.ListView _listView1;
		private System.Windows.Forms.Button _buttonWord;
		private System.Windows.Forms.Button _buttonXmlDiff;
		private System.Windows.Forms.Button _buttonStop;
		private System.Windows.Forms.ColumnHeader _columnN;
		private System.Windows.Forms.ColumnHeader _columnDoc;
		private System.Windows.Forms.ColumnHeader _columnC;
		private System.Windows.Forms.ColumnHeader _columnFlags;
		private System.Windows.Forms.ColumnHeader _columnError;
		private System.Windows.Forms.ColumnHeader _columnRun;
		private System.Windows.Forms.Label _labelLanguage;
		private System.Windows.Forms.Label _labelPage;
		private System.Windows.Forms.Label _label1;
		private System.Windows.Forms.Label _label2;
		private System.Windows.Forms.GroupBox _groupBoxDocument;
		private System.Windows.Forms.CheckBox _checkBoxDocumentDisabled;
		private System.Windows.Forms.Label _labelFile;

		private System.Windows.Forms.Label _label3;
		private System.Windows.Forms.ColumnHeader _columnRestart;
		private System.Windows.Forms.MenuItem _menuItem5;
		private System.Windows.Forms.MenuItem _menuItem6;
		private System.Windows.Forms.MenuItem _menuItem7;
		private System.Windows.Forms.MenuItem _menuItem8;
		private System.Windows.Forms.MenuItem _menuItemFileClose;
		private System.Windows.Forms.MenuItem _menuItemEditSelectAll;
		private System.Windows.Forms.MenuItem _menuItemTestClear;
		private System.Windows.Forms.MainMenu _mainMenu1;
		private System.Windows.Forms.Label _label13;
		private System.Windows.Forms.MenuItem _menuItemFileUpdateList;
		private System.Windows.Forms.Button _buttonClear;
		private System.Windows.Forms.GroupBox _groupBoxTestStat;
		private System.Windows.Forms.Label _labelNErrors;
		private System.Windows.Forms.Label _labelNDocuments;
		private System.Windows.Forms.Label _labelNRun;
		private System.Windows.Forms.MenuItem _menuItem1;
		private System.Windows.Forms.MenuItem _menuItem2;
		private System.Windows.Forms.MenuItem _menuItem3;
		private System.Windows.Forms.MenuItem _menuItem4;
		private System.Windows.Forms.ComboBox _comboBox1;
		private System.Windows.Forms.MenuItem _menuItem9;
		private System.Windows.Forms.MenuItem _menuItem10;
		private System.Windows.Forms.MenuItem _menuItem11;
		private System.Windows.Forms.MenuItem _menuItem12;
		private System.Windows.Forms.MenuItem _menuItem14;
		private System.Windows.Forms.MenuItem _menuItem13;
		private System.Windows.Forms.ComboBox _comboBox3;
		private System.Windows.Forms.Button _button3;
		private System.Windows.Forms.Button _button4;
		private System.Windows.Forms.Label _label9;
		private System.Windows.Forms.ContextMenu _contextMenu1;
		private System.Windows.Forms.MenuItem _menuItem15;
		private System.Windows.Forms.MenuItem _menuItem16;
		private System.Windows.Forms.MenuItem _menuItem17;
		private System.Windows.Forms.Button _buttonFirst;
		private System.Windows.Forms.Button _buttonFindPrev;
		private System.Windows.Forms.Button _buttonFindNext;
		private System.Windows.Forms.TextBox _textBoxSearch;
		private System.Windows.Forms.ListView _listView2;
		private System.Windows.Forms.ColumnHeader _columnHeader1;
		private System.Windows.Forms.GroupBox _groupBox1;
		private System.Windows.Forms.Label _label5;
		private System.Windows.Forms.RichTextBox _textBoxComment;

		// no constants :-)

		public CTestListDialog (IUIExchangeProc exchageObject)
		{
			this._exchangeObject = _exchangeObject;

			// Required for Windows Form Designer support
			InitializeComponent();

			// Initialize the exchange data
			_exchangeData = new CUIExchangeData ();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._listView1 = new System.Windows.Forms.ListView();
			this._columnN = new System.Windows.Forms.ColumnHeader();
			this._columnRestart = new System.Windows.Forms.ColumnHeader();
			this._columnDoc = new System.Windows.Forms.ColumnHeader();
			this._columnRun = new System.Windows.Forms.ColumnHeader();
			this._columnC = new System.Windows.Forms.ColumnHeader();
			this._columnFlags = new System.Windows.Forms.ColumnHeader();
			this._columnError = new System.Windows.Forms.ColumnHeader();
			this._buttonWord = new System.Windows.Forms.Button();
			this._labelFile = new System.Windows.Forms.Label();
			this._buttonXmlDiff = new System.Windows.Forms.Button();
			this._buttonStop = new System.Windows.Forms.Button();
			this._labelLanguage = new System.Windows.Forms.Label();
			this._labelPage = new System.Windows.Forms.Label();
			this._label1 = new System.Windows.Forms.Label();
			this._label2 = new System.Windows.Forms.Label();
			this._groupBoxDocument = new System.Windows.Forms.GroupBox();
			this._textBoxComment = new System.Windows.Forms.RichTextBox();
			this._checkBoxDocumentDisabled = new System.Windows.Forms.CheckBox();
			this._label3 = new System.Windows.Forms.Label();
			this._buttonClear = new System.Windows.Forms.Button();
			this._label13 = new System.Windows.Forms.Label();
			this._labelNErrors = new System.Windows.Forms.Label();
			this._labelNDocuments = new System.Windows.Forms.Label();
			this._mainMenu1 = new System.Windows.Forms.MainMenu();
			this._menuItem5 = new System.Windows.Forms.MenuItem();
			this._menuItem9 = new System.Windows.Forms.MenuItem();
			this._menuItem13 = new System.Windows.Forms.MenuItem();
			this._menuItem10 = new System.Windows.Forms.MenuItem();
			this._menuItem12 = new System.Windows.Forms.MenuItem();
			this._menuItemFileUpdateList = new System.Windows.Forms.MenuItem();
			this._menuItem14 = new System.Windows.Forms.MenuItem();
			this._menuItemFileClose = new System.Windows.Forms.MenuItem();
			this._menuItem6 = new System.Windows.Forms.MenuItem();
			this._menuItemEditSelectAll = new System.Windows.Forms.MenuItem();
			this._menuItem7 = new System.Windows.Forms.MenuItem();
			this._menuItem1 = new System.Windows.Forms.MenuItem();
			this._menuItem2 = new System.Windows.Forms.MenuItem();
			this._menuItem3 = new System.Windows.Forms.MenuItem();
			this._menuItemTestClear = new System.Windows.Forms.MenuItem();
			this._menuItem11 = new System.Windows.Forms.MenuItem();
			this._menuItem8 = new System.Windows.Forms.MenuItem();
			this._menuItem4 = new System.Windows.Forms.MenuItem();
			this._labelNRun = new System.Windows.Forms.Label();
			this._groupBoxTestStat = new System.Windows.Forms.GroupBox();
			this._comboBox1 = new System.Windows.Forms.ComboBox();
			this._textBoxSearch = new System.Windows.Forms.TextBox();
			this._contextMenu1 = new System.Windows.Forms.ContextMenu();
			this._menuItem15 = new System.Windows.Forms.MenuItem();
			this._menuItem16 = new System.Windows.Forms.MenuItem();
			this._menuItem17 = new System.Windows.Forms.MenuItem();
			this._buttonFindPrev = new System.Windows.Forms.Button();
			this._buttonFindNext = new System.Windows.Forms.Button();
			this._comboBox3 = new System.Windows.Forms.ComboBox();
			this._button3 = new System.Windows.Forms.Button();
			this._button4 = new System.Windows.Forms.Button();
			this._buttonFirst = new System.Windows.Forms.Button();
			this._label9 = new System.Windows.Forms.Label();
			this._listView2 = new System.Windows.Forms.ListView();
			this._columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this._groupBox1 = new System.Windows.Forms.GroupBox();
			this._label5 = new System.Windows.Forms.Label();
			this._groupBoxDocument.SuspendLayout();
			this._groupBoxTestStat.SuspendLayout();
			this._groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this._listView1.AllowColumnReorder = true;
			this._listView1.BackColor = System.Drawing.SystemColors.Window;
			this._listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this._columnN,
																						this._columnRestart,
																						this._columnDoc,
																						this._columnRun,
																						this._columnC,
																						this._columnFlags,
																						this._columnError});
			this._listView1.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._listView1.FullRowSelect = true;
			this._listView1.GridLines = true;
			this._listView1.HideSelection = false;
			this._listView1.Location = new System.Drawing.Point(136, 88);
			this._listView1.Name = "listView1";
			this._listView1.Size = new System.Drawing.Size(688, 440);
			this._listView1.TabIndex = 0;
			this._listView1.View = System.Windows.Forms.View.Details;
			// 
			// ColumnN
			// 
			this._columnN.Text = "N";
			this._columnN.Width = 39;
			// 
			// columnRestart
			// 
			this._columnRestart.Text = "r";
			this._columnRestart.Width = 20;
			// 
			// ColumnDoc
			// 
			this._columnDoc.Text = "Document";
			this._columnDoc.Width = 169;
			// 
			// ColumnRun
			// 
			this._columnRun.Text = "Run";
			this._columnRun.Width = 76;
			// 
			// ColumnC
			// 
			this._columnC.Text = "C";
			this._columnC.Width = 147;
			// 
			// ColumnFlags
			// 
			this._columnFlags.Text = "Flags";
			this._columnFlags.Width = 49;
			// 
			// ColumnError
			// 
			this._columnError.Text = "Error";
			this._columnError.Width = 300;
			// 
			// buttonWord
			// 
			this._buttonWord.BackColor = System.Drawing.SystemColors.Control;
			this._buttonWord.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonWord.Location = new System.Drawing.Point(8, 12);
			this._buttonWord.Name = "buttonWord";
			this._buttonWord.Size = new System.Drawing.Size(25, 20);
			this._buttonWord.TabIndex = 3;
			this._buttonWord.Text = "W";
			// 
			// labelFile
			// 
			this._labelFile.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelFile.ForeColor = System.Drawing.SystemColors.GrayText;
			this._labelFile.Location = new System.Drawing.Point(186, 672);
			this._labelFile.Name = "labelFile";
			this._labelFile.Size = new System.Drawing.Size(484, 20);
			this._labelFile.TabIndex = 4;
			this._labelFile.Text = "FILE:";
			// 
			// buttonXmlDiff
			// 
			this._buttonXmlDiff.BackColor = System.Drawing.SystemColors.Control;
			this._buttonXmlDiff.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonXmlDiff.Location = new System.Drawing.Point(40, 12);
			this._buttonXmlDiff.Name = "buttonXmlDiff";
			this._buttonXmlDiff.Size = new System.Drawing.Size(25, 20);
			this._buttonXmlDiff.TabIndex = 5;
			this._buttonXmlDiff.Text = "X";
			// 
			// buttonStop
			// 
			this._buttonStop.BackColor = System.Drawing.SystemColors.Control;
			this._buttonStop.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonStop.Location = new System.Drawing.Point(72, 12);
			this._buttonStop.Name = "buttonStop";
			this._buttonStop.Size = new System.Drawing.Size(50, 20);
			this._buttonStop.TabIndex = 33;
			this._buttonStop.Text = "STOP";
			// 
			// labelLanguage
			// 
			this._labelLanguage.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelLanguage.Location = new System.Drawing.Point(330, 43);
			this._labelLanguage.Name = "labelLanguage";
			this._labelLanguage.Size = new System.Drawing.Size(74, 24);
			this._labelLanguage.TabIndex = 35;
			this._labelLanguage.Text = "English";
			// 
			// labelPage
			// 
			this._labelPage.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelPage.Location = new System.Drawing.Point(298, 67);
			this._labelPage.Name = "labelPage";
			this._labelPage.Size = new System.Drawing.Size(112, 24);
			this._labelPage.TabIndex = 36;
			this._labelPage.Text = "Word";
			// 
			// label1
			// 
			this._label1.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label1.Location = new System.Drawing.Point(258, 67);
			this._label1.Name = "label1";
			this._label1.Size = new System.Drawing.Size(48, 24);
			this._label1.TabIndex = 40;
			this._label1.Text = "Page:";
			// 
			// label2
			// 
			this._label2.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label2.Location = new System.Drawing.Point(258, 43);
			this._label2.Name = "label2";
			this._label2.Size = new System.Drawing.Size(82, 24);
			this._label2.TabIndex = 39;
			this._label2.Text = "Language:";
			// 
			// groupBoxDocument
			// 
			this._groupBoxDocument.Controls.Add(this._textBoxComment);
			this._groupBoxDocument.Controls.Add(this._checkBoxDocumentDisabled);
			this._groupBoxDocument.Controls.Add(this._labelPage);
			this._groupBoxDocument.Controls.Add(this._labelLanguage);
			this._groupBoxDocument.Controls.Add(this._label2);
			this._groupBoxDocument.Controls.Add(this._label1);
			this._groupBoxDocument.Controls.Add(this._label3);
			this._groupBoxDocument.Controls.Add(this._buttonClear);
			this._groupBoxDocument.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._groupBoxDocument.Location = new System.Drawing.Point(144, 536);
			this._groupBoxDocument.Name = "groupBoxDocument";
			this._groupBoxDocument.Size = new System.Drawing.Size(438, 132);
			this._groupBoxDocument.TabIndex = 41;
			this._groupBoxDocument.TabStop = false;
			this._groupBoxDocument.Text = "Document properties";
			// 
			// textBoxComment
			// 
			this._textBoxComment.BackColor = System.Drawing.SystemColors.Info;
			this._textBoxComment.Location = new System.Drawing.Point(8, 43);
			this._textBoxComment.Name = "textBoxComment";
			this._textBoxComment.Size = new System.Drawing.Size(238, 69);
			this._textBoxComment.TabIndex = 48;
			this._textBoxComment.Text = "richTextBox1";
			// 
			// checkBoxDocumentDisabled
			// 
			this._checkBoxDocumentDisabled.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._checkBoxDocumentDisabled.Location = new System.Drawing.Point(259, 93);
			this._checkBoxDocumentDisabled.Name = "checkBoxDocumentDisabled";
			this._checkBoxDocumentDisabled.Size = new System.Drawing.Size(96, 24);
			this._checkBoxDocumentDisabled.TabIndex = 0;
			this._checkBoxDocumentDisabled.Text = "Do not run";
			// 
			// label3
			// 
			this._label3.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label3.Location = new System.Drawing.Point(6, 20);
			this._label3.Name = "label3";
			this._label3.Size = new System.Drawing.Size(184, 24);
			this._label3.TabIndex = 45;
			this._label3.Text = "Enter your comments here:";
			// 
			// buttonClear
			// 
			this._buttonClear.BackColor = System.Drawing.SystemColors.Control;
			this._buttonClear.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonClear.Location = new System.Drawing.Point(352, 93);
			this._buttonClear.Name = "buttonClear";
			this._buttonClear.Size = new System.Drawing.Size(75, 24);
			this._buttonClear.TabIndex = 49;
			this._buttonClear.Text = "Clear";
			// 
			// label13
			// 
			this._label13.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label13.ForeColor = System.Drawing.SystemColors.GrayText;
			this._label13.Location = new System.Drawing.Point(146, 672);
			this._label13.Name = "label13";
			this._label13.Size = new System.Drawing.Size(46, 24);
			this._label13.TabIndex = 46;
			this._label13.Text = "FILE = ";
			// 
			// labelNErrors
			// 
			this._labelNErrors.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelNErrors.Location = new System.Drawing.Point(16, 80);
			this._labelNErrors.Name = "labelNErrors";
			this._labelNErrors.Size = new System.Drawing.Size(112, 24);
			this._labelNErrors.TabIndex = 7;
			this._labelNErrors.Text = "Errors:";
			// 
			// labelNDocuments
			// 
			this._labelNDocuments.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelNDocuments.Location = new System.Drawing.Point(16, 32);
			this._labelNDocuments.Name = "labelNDocuments";
			this._labelNDocuments.Size = new System.Drawing.Size(136, 24);
			this._labelNDocuments.TabIndex = 1;
			this._labelNDocuments.Text = "Documents:";
			// 
			// mainMenu1
			// 
			this._mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItem5,
																					  this._menuItem6,
																					  this._menuItem7,
																					  this._menuItem11,
																					  this._menuItem8});
			// 
			// menuItem5
			// 
			this._menuItem5.Index = 0;
			this._menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItem9,
																					  this._menuItem13,
																					  this._menuItem10,
																					  this._menuItem12,
																					  this._menuItemFileUpdateList,
																					  this._menuItem14,
																					  this._menuItemFileClose});
			this._menuItem5.Text = "&File";
			// 
			// menuItem9
			// 
			this._menuItem9.Index = 0;
			this._menuItem9.Text = "&Save run";
			// 
			// menuItem13
			// 
			this._menuItem13.Index = 1;
			this._menuItem13.Text = "&New run";
			// 
			// menuItem10
			// 
			this._menuItem10.Index = 2;
			this._menuItem10.Text = "-";
			// 
			// menuItem12
			// 
			this._menuItem12.Index = 3;
			this._menuItem12.Text = "&Delete run";
			// 
			// menuItemFileUpdateList
			// 
			this._menuItemFileUpdateList.Index = 4;
			this._menuItemFileUpdateList.Text = "&Reload";
			// 
			// menuItem14
			// 
			this._menuItem14.Index = 5;
			this._menuItem14.Text = "-";
			// 
			// menuItemFileClose
			// 
			this._menuItemFileClose.Index = 6;
			this._menuItemFileClose.Text = "&Close";
			// 
			// menuItem6
			// 
			this._menuItem6.Index = 1;
			this._menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItemEditSelectAll});
			this._menuItem6.Text = "&Edit";
			// 
			// menuItemEditSelectAll
			// 
			this._menuItemEditSelectAll.Index = 0;
			this._menuItemEditSelectAll.Text = "&Select all";
			// 
			// menuItem7
			// 
			this._menuItem7.Index = 2;
			this._menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItem1,
																					  this._menuItem2,
																					  this._menuItem3,
																					  this._menuItemTestClear});
			this._menuItem7.Text = "&Test";
			// 
			// menuItem1
			// 
			this._menuItem1.Index = 0;
			this._menuItem1.Text = "&Run all";
			// 
			// menuItem2
			// 
			this._menuItem2.Index = 1;
			this._menuItem2.Text = "Run &selection";
			// 
			// menuItem3
			// 
			this._menuItem3.Index = 2;
			this._menuItem3.Text = "-";
			// 
			// menuItemTestClear
			// 
			this._menuItemTestClear.Index = 3;
			this._menuItemTestClear.Text = "&Clear";
			// 
			// menuItem11
			// 
			this._menuItem11.Index = 3;
			this._menuItem11.Text = "&Compare";
			// 
			// menuItem8
			// 
			this._menuItem8.Index = 4;
			this._menuItem8.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItem4});
			this._menuItem8.Text = "&Help";
			// 
			// menuItem4
			// 
			this._menuItem4.Index = 0;
			this._menuItem4.Text = "&About";
			// 
			// labelNRun
			// 
			this._labelNRun.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelNRun.Location = new System.Drawing.Point(16, 56);
			this._labelNRun.Name = "labelNRun";
			this._labelNRun.Size = new System.Drawing.Size(120, 24);
			this._labelNRun.TabIndex = 51;
			this._labelNRun.Text = "Run:";
			// 
			// groupBoxTestStat
			// 
			this._groupBoxTestStat.Controls.Add(this._labelNRun);
			this._groupBoxTestStat.Controls.Add(this._labelNErrors);
			this._groupBoxTestStat.Controls.Add(this._labelNDocuments);
			this._groupBoxTestStat.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._groupBoxTestStat.Location = new System.Drawing.Point(600, 536);
			this._groupBoxTestStat.Name = "groupBoxTestStat";
			this._groupBoxTestStat.Size = new System.Drawing.Size(184, 132);
			this._groupBoxTestStat.TabIndex = 50;
			this._groupBoxTestStat.TabStop = false;
			this._groupBoxTestStat.Text = "Test statistics";
			// 
			// comboBox1
			// 
			this._comboBox1.BackColor = System.Drawing.SystemColors.Info;
			this._comboBox1.Location = new System.Drawing.Point(10, 49);
			this._comboBox1.Name = "comboBox1";
			this._comboBox1.Size = new System.Drawing.Size(129, 21);
			this._comboBox1.TabIndex = 53;
			this._comboBox1.Text = "<temporary>";
			// 
			// textBoxSearch
			// 
			this._textBoxSearch.BackColor = System.Drawing.SystemColors.Info;
			this._textBoxSearch.ContextMenu = this._contextMenu1;
			this._textBoxSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._textBoxSearch.Location = new System.Drawing.Point(192, 11);
			this._textBoxSearch.Name = "textBoxSearch";
			this._textBoxSearch.Size = new System.Drawing.Size(168, 20);
			this._textBoxSearch.TabIndex = 56;
			this._textBoxSearch.Text = "<any error>";
			// 
			// contextMenu1
			// 
			this._contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this._menuItem15,
																						 this._menuItem16,
																						 this._menuItem17});
			// 
			// menuItem15
			// 
			this._menuItem15.Index = 0;
			this._menuItem15.Text = "Error";
			// 
			// menuItem16
			// 
			this._menuItem16.Index = 1;
			this._menuItem16.Text = "Comment";
			// 
			// menuItem17
			// 
			this._menuItem17.Index = 2;
			this._menuItem17.Text = "Run <> Run2";
			// 
			// buttonFindPrev
			// 
			this._buttonFindPrev.Location = new System.Drawing.Point(423, 12);
			this._buttonFindPrev.Name = "buttonFindPrev";
			this._buttonFindPrev.Size = new System.Drawing.Size(25, 20);
			this._buttonFindPrev.TabIndex = 55;
			this._buttonFindPrev.Text = "<=";
			// 
			// buttonFindNext
			// 
			this._buttonFindNext.Location = new System.Drawing.Point(456, 13);
			this._buttonFindNext.Name = "buttonFindNext";
			this._buttonFindNext.Size = new System.Drawing.Size(25, 20);
			this._buttonFindNext.TabIndex = 55;
			this._buttonFindNext.Text = "=>";
			// 
			// comboBox3
			// 
			this._comboBox3.BackColor = System.Drawing.SystemColors.Info;
			this._comboBox3.Location = new System.Drawing.Point(568, 11);
			this._comboBox3.Name = "comboBox3";
			this._comboBox3.Size = new System.Drawing.Size(184, 21);
			this._comboBox3.TabIndex = 59;
			this._comboBox3.Text = "<none>";
			// 
			// button3
			// 
			this._button3.Location = new System.Drawing.Point(486, 12);
			this._button3.Name = "button3";
			this._button3.Size = new System.Drawing.Size(50, 20);
			this._button3.TabIndex = 60;
			this._button3.Text = "add";
			// 
			// button4
			// 
			this._button4.Location = new System.Drawing.Point(763, 11);
			this._button4.Name = "button4";
			this._button4.Size = new System.Drawing.Size(50, 20);
			this._button4.TabIndex = 61;
			this._button4.Text = "clear";
			// 
			// buttonFirst
			// 
			this._buttonFirst.Location = new System.Drawing.Point(367, 12);
			this._buttonFirst.Name = "buttonFirst";
			this._buttonFirst.Size = new System.Drawing.Size(50, 20);
			this._buttonFirst.TabIndex = 62;
			this._buttonFirst.Text = "first";
			// 
			// label9
			// 
			this._label9.Location = new System.Drawing.Point(152, 13);
			this._label9.Name = "label9";
			this._label9.Size = new System.Drawing.Size(34, 24);
			this._label9.TabIndex = 63;
			this._label9.Text = "Find:";
			// 
			// listView2
			// 
			this._listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this._columnHeader1});
			this._listView2.GridLines = true;
			this._listView2.Location = new System.Drawing.Point(8, 88);
			this._listView2.MultiSelect = false;
			this._listView2.Name = "listView2";
			this._listView2.Size = new System.Drawing.Size(112, 440);
			this._listView2.TabIndex = 66;
			this._listView2.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this._columnHeader1.Text = "Folder";
			this._columnHeader1.Width = 106;
			// 
			// groupBox1
			// 
			this._groupBox1.Controls.Add(this._buttonWord);
			this._groupBox1.Controls.Add(this._buttonXmlDiff);
			this._groupBox1.Controls.Add(this._buttonStop);
			this._groupBox1.Controls.Add(this._comboBox3);
			this._groupBox1.Controls.Add(this._button4);
			this._groupBox1.Controls.Add(this._label9);
			this._groupBox1.Controls.Add(this._textBoxSearch);
			this._groupBox1.Controls.Add(this._buttonFirst);
			this._groupBox1.Controls.Add(this._buttonFindPrev);
			this._groupBox1.Controls.Add(this._buttonFindNext);
			this._groupBox1.Controls.Add(this._button3);
			this._groupBox1.Location = new System.Drawing.Point(2, 0);
			this._groupBox1.Name = "groupBox1";
			this._groupBox1.Size = new System.Drawing.Size(822, 40);
			this._groupBox1.TabIndex = 67;
			this._groupBox1.TabStop = false;
			// 
			// label5
			// 
			this._label5.Location = new System.Drawing.Point(152, 52);
			this._label5.Name = "label5";
			this._label5.Size = new System.Drawing.Size(88, 16);
			this._label5.TabIndex = 68;
			this._label5.Text = "Current run";
			// 
			// CTestListDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(832, 689);
			this.Controls.Add(this._label5);
			this.Controls.Add(this._groupBox1);
			this.Controls.Add(this._listView2);
			this.Controls.Add(this._comboBox1);
			this.Controls.Add(this._groupBoxTestStat);
			this.Controls.Add(this._groupBoxDocument);
			this.Controls.Add(this._label13);
			this.Controls.Add(this._labelFile);
			this.Controls.Add(this._listView1);
			this.Menu = this._mainMenu1;
			this.Name = "CTestListDialog";
			this.Text = "Test List";
			this._groupBoxDocument.ResumeLayout(false);
			this._groupBoxTestStat.ResumeLayout(false);
			this._groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}


	interface CUIExchangeData
	{
		// Run data 
		public int irun
		{
			get { return irun;}
		}

		public void SetRunList (string [] rgstrMainRun);
		private string [] _rgstrRun; // can not be changed outside
		private bool _fRebuildRunList;

		//Folder list

		public int iselFolder;
		public void SetFolderList (string [] rgstrFolders);
		public void SetFolderSelection (int iselFolder);
		private string [] _rgstrFolders;
		private bool _fRebuildFolderList;
		private bool _fFolderSelectionModified;

		// Document list
		public int GetNumberSelectedDocuments ();
		public int GetSingleDocumentSelection ();
		public CDocumentView [] rgDocumentViews;
		
		public void CreateDocumentList (string [] rgstrDocumentNames);
		public void SetDocumentProperties (int idoc, bool fRestart, string strRun, bool fShowAsError,
										   string strComment, string strFlags, string strError );

		private bool _fRebuildDocumentList;
		private bool _fDocumentPropertiesModified;
		private bool _fDocumentSelectionModified;

		// Search and boxes
		public string strSearch;
		public string strFilter;
		public void SelectSearchText ();
		private bool fSelectSearchText ();

		//Document properties 
		public string strComment;
		public bool fDontRun;
		public bool fDocumentPropertiesEnabled;

		// Shortcut buttons
		public bool fWordEnabled;
		public bool fDiffEnabled;
		public bool fStopEnabled;

		// Constructor
		public CUIExchangeData ();
   	}


	class CDocumentView 
	{
		public void Change (bool fRestart, string strRun, bool fShowAsError, string strComment,
			string strFlags, string strError )
		{
			this._fRestart = fRestart;
			this._strRun = strRun;
			this._fShowAsError = fShowAsError;
			this._strComment = strComment;
			this._strFlags = strFlags;
			this._strError = strError;

			_fModified = true;
		}

		public bool fSelected
		{
			get { return fSelected;}
			set 
			{
				if (fSelected != value) 
				{
					_fSelectionModified = true; 
					_exchangeDataParent._fDocumentSelectionModified = true;
				};
				fSelected = value;}
			}

		public bool fVisible
		{
			get { return fVisible;}
			set 
			{
				if (fVisible != value) 
				{
					_exchangeDataParent._fRebuildDocumentList = true;
				}
				fVisible = value;
			}
		}

		private CUIExchangeData _exchangeDataParent;
		private string _strName;
		private bool _fRestart;
		private string _strRun;
		private bool _fShowAsError;
		private string _strComment;
		private string _strFlags;
		private string _strError;

		private bool _fModified;
		private bool _fSelectionModified;
	}


	interface IUIExchangeProc
	{
		void DoExchange (CUIExchangeData uiexdat, KUIExchange kux);
	}

	// Reason for exchange

	enum KUIExchange
	{
		kuxInitialize = 0,
		kuxChangeFolderSelection,
		kuxChangeDocumentSelection,
		kuxChangeRun,
		kuxChangeSearchText,
		kuxCommandFindFirst,
		kuxCommandFindNext,
		kuxCommandFindPrev,
		kuxCommandAddFilter,
		kuxChangeFilterText,
		kuxCommandClearFilter,
		kuxChangeCommentsText,
		kuxChangeFRun,
		kuxClearPropertiesCommand,
		kuxCommandFileSaveRun,
		kuxCommandFileDeleteRun,
		kuxCommandFileNewRun,
		kuxCommandFileReloadRun,
		kuxCommandFileClose,
		kuxCommandEditSelectAll,
		kuxCommandTestRunALl,
		kuxCommandTestRunSelection,
		kuxCommandHelpAbout
	}
}