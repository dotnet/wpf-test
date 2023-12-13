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
	using Query;
	using QueryBuilder;
	using DocumentFile;

	delegate void CDelegateFinishedWord ();

	class CTestList : System.Windows.Forms.Form
	{
		// Class variables
      #region
      System.Drawing.Size			_sizePrevious;
		System.Windows.Forms.Form	_formParent;
		Delegate					_delegateFinished;
		CLocation					_ourLocation;
		CProtocol					_protocol;
		CRunword					_runwordObject;

		bool						_fRunning;
		bool						_fTerminating;
		bool						_fRunningWord;
		int							_kTarget;

		CDelegateFinishedWord	    _delegateFinishedWord;
      
      /* Array of folder tasks */
		CTFolderTask []				_rgFolderTasks;
		int							_nTasks;
		bool						_fAllDocumentPropertiesSaved;      
      #endregion
	  
	  //Variables for current test run
      #region
      bool						_fPreparedForTests;
		bool						_fRunTestsAfterClosing;
		int							_nTestErrorsFixed;	// for progres bar
		int							_nTestErrorsNew;		// for progress bar
		int							_nTestErrors;		// for progress bar

		bool						_fTestListValid;

		/* Mapping from current view to tasks and documents */
		int []						_rgimapView2Task;
		int []						_rgimapView2Doc;
      #endregion

      // Variables corresonding to current dialog control status
      #region
      bool						_fAllowDataExchange; // If exchange is allowed?

		int							_iviewSelection; // Selection: read-only!
		bool						_fDocEnabled;	// If current document enabled (props)
		string						_strDocComment;	// Comments for current document (props)
		string						_sfnCurrentRun;	// Current run
		string						_sfnSecondRun;	// Second run
		string						_strQuery;		// Current query string
		string						_strQuerySet;	// Set query
		bool						_fRecalcStatistics;
													// Statistics may have changed - recalculate

		/* Test statistics */
		int							_nDocuments_stat;
		int							_nRun_stat;
		int							_nErrors_stat;
		int							_nDocumentsVisible_stat;
		int							_nErrorsVisible_stat;
		int							_nRunVisible_stat;
		int							_nDocumentsMarked_stat;
		int							_nErrorsMarked_stat;
		int							_nRunMarked_stat;
      #endregion

      #region //dialog variables
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

		private System.ComponentModel.Container _components = null;
		private System.Windows.Forms.Label _label3;
		private System.Windows.Forms.ColumnHeader _columnRestart;
		private System.Windows.Forms.MenuItem _menuItem5;
		private System.Windows.Forms.MenuItem _menuItem6;
		private System.Windows.Forms.MenuItem _menuItem7;
		private System.Windows.Forms.MenuItem _menuItem8;
		private System.Windows.Forms.MenuItem _menuItemFileClose;
		private System.Windows.Forms.MenuItem _menuItemTestClear;
		private System.Windows.Forms.MainMenu _mainMenu1;
		private System.Windows.Forms.Label _label13;
		private System.Windows.Forms.MenuItem _menuItemFileUpdateList;
		private System.Windows.Forms.GroupBox _groupBoxTestStat;
		private System.Windows.Forms.Label _labelNErrors;
		private System.Windows.Forms.Label _labelNDocuments;
		private System.Windows.Forms.Label _labelNRun;
		private System.Windows.Forms.Button _buttonFindNext;
		private System.Windows.Forms.Button _buttonFindPrev;
		private System.Windows.Forms.Button _buttonFindFirst;
		private System.Windows.Forms.Button _buttonCountAll;
		private System.Windows.Forms.GroupBox _groupBoxUpdatingBackground;
		private System.Windows.Forms.Label _label4;
		private System.Windows.Forms.ContextMenu _contextMenu1;
		private System.Windows.Forms.MenuItem _menuItem15;
		private System.Windows.Forms.MenuItem _menuItem16;
		private System.Windows.Forms.MenuItem _menuItem17;
		private System.Windows.Forms.RichTextBox _textBoxComment;
		private System.Windows.Forms.Label _label5;
		private System.Windows.Forms.Label _labelStatAllDocs;
		private System.Windows.Forms.Label _labelStatCurDocs;
		private System.Windows.Forms.Label _labelStatCurRun;
		private System.Windows.Forms.Label _labelStatAllRun;
		private System.Windows.Forms.Label _labelStatCurErrors;
		private System.Windows.Forms.Label _labelStatAllErrors;
		private System.Windows.Forms.Label _labelCurrent;
		private System.Windows.Forms.MenuItem _menuItemFileSave;
		private System.Windows.Forms.MenuItem _menuItem9;
		private System.Windows.Forms.MenuItem _menuItemFileLoad;
		private System.Windows.Forms.MenuItem _menuItem10;
		private System.Windows.Forms.ColumnHeader _columnRun2;
		private System.Windows.Forms.MenuItem _menuItemFileOpenSecond;
		private System.Windows.Forms.MenuItem _menuItemFileClearSecond;

      #region
      Color _colorButtonBackgroundDefault;
      int _nColumns;
      string[] _rgstrColumnNames;
      private System.Windows.Forms.MenuItem _menuItem3;
      private System.Windows.Forms.GroupBox _groupBoxTop;
      private System.Windows.Forms.Label _labelExistingWordWarning;
      private System.Windows.Forms.Label _label7;
      private System.Windows.Forms.Button _buttonClearQuery;
      private System.Windows.Forms.Button _buttonLoadQuery;
      private System.Windows.Forms.ColumnHeader _columnError2;
      private System.Windows.Forms.Label _labelCurrentQuery;
      private System.Windows.Forms.Label _label6;
      private System.Windows.Forms.Label _labelStatSelErrors;
      private System.Windows.Forms.Label _labelStatSelRun;
      private System.Windows.Forms.Label _labelStatSelDocs;
      private System.Windows.Forms.MenuItem _menuItem4;
      private System.Windows.Forms.ComboBox _comboBoxQuery;
      private System.Windows.Forms.MenuItem _menuItem13;
      private System.Windows.Forms.MenuItem _menuItem19;
      private System.Windows.Forms.MenuItem _menuItemEditSelectAll;
      private System.Windows.Forms.MenuItem _menuItemRunSelection;
      private System.Windows.Forms.MenuItem _menuItemOpenWord;
      private System.Windows.Forms.MenuItem _menuItemCompareXml;
      private System.Windows.Forms.MenuItem _menuItemCopyDocument;
      private System.Windows.Forms.MenuItem _menuItemMoveDocument;
      private System.Windows.Forms.MenuItem _menuItemDeleteDocument;
      private System.Windows.Forms.ColumnHeader _columnFlagged;
      private System.Windows.Forms.MenuItem _menuItemEditFlagAllCurrent;
      private System.Windows.Forms.MenuItem _menuItemClearFlags;
      private System.Windows.Forms.MenuItem _menuItemRenameDocument;
      private System.Windows.Forms.MenuItem _menuItem1;
      private System.Windows.Forms.MenuItem _menuItemExportExcelAll;
      private System.Windows.Forms.MenuItem _menuItemExportExcelSelection;
      #endregion

      #endregion//dialog variables

      // Temporaries - caches

		string [] _rgstrSearchRecordTemp;

		public CTestList (CLocation OurLocation,
						  /*CRunword runwordObject,*/
						  System.Windows.Forms.Form formParent, 
			 			  Delegate delegateFinished )
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			this._ourLocation = OurLocation;
			//this.runwordObject = runwordObject;
			this._fRunning = false;
			this._delegateFinished = delegateFinished;
			this._formParent = formParent;
			this._fTerminating = false;
			this._fRunningWord = false;
			this._fAllDocumentPropertiesSaved = true;
			this._fPreparedForTests = false;
			this._fTestListValid = false;
			this._sfnCurrentRun = "";
			this._sfnSecondRun = "";

			this._fAllowDataExchange = true;

			this._colorButtonBackgroundDefault = _buttonWord.BackColor;

			_delegateFinishedWord = new CDelegateFinishedWord (FinishedWordNotification);

			// Pre-allocated arays of documents and folders
			_rgFolderTasks = new CTFolderTask [1024];
			_nTasks = 0;

			_sizePrevious = Size;
			MinimumSize = Size;

			// Fill out column name information

			Common.Assert (_listView1.Columns.Count == 10);

			_nColumns = _listView1.Columns.Count;

			/* Allocate arrays */
			_rgstrColumnNames = new string [_nColumns];			// Column names
			_rgstrSearchRecordTemp = new string [_nColumns + 1];	// Search record temp array

			_rgstrColumnNames [_columnN.Index] = _columnN.Text;
			_rgstrColumnNames [_columnRestart.Index] = _columnRestart.Text;
			_rgstrColumnNames [_columnFlagged.Index] = _columnFlagged.Text;
			_rgstrColumnNames [_columnDoc.Index] = _columnDoc.Text;
			_rgstrColumnNames [_columnRun.Index] = _columnRun.Text;
			_rgstrColumnNames [_columnRun2.Index] = _columnRun2.Text;
			_rgstrColumnNames [_columnC.Index] = _columnC.Text;
			_rgstrColumnNames [_columnFlags.Index] = _columnFlags.Text;
			_rgstrColumnNames [_columnError.Index] = _columnError.Text;
			_rgstrColumnNames [_columnError2.Index] = _columnError2.Text;
		}

		// Run - start dialog in a new thread
        public void Run ()
		{
			Thread threadRunning;

			Common.Assert (!_fRunning);
			Common.Assert (_fAllDocumentPropertiesSaved);

			this._fPreparedForTests = false;
			this._fRunTestsAfterClosing = false;

			if (!_fTestListValid)
			{
				try
				{
					EnumerateTests ();
				}
				catch {};

				LoadResultsCore ("", 0);
				InitializeDialog ();
				InitializeQuery ();

				_fTestListValid = true;
			};

			threadRunning = new Thread (new ThreadStart (RunDialogAsync));
            threadRunning.SetApartmentState(ApartmentState.STA);
			threadRunning.Start ();

			_fRunning = true;
		}

		// ChangeFolder
		public void ChangeFolder ()
		{
			Common.Assert (!_fRunning);

			_fTestListValid = false;
		}	

		public void DeleteRun (string strRun)
		{
			string sfnRun = Path.Combine (_ourLocation.GetBVTDir(), strRun);

			if (File.Exists (sfnRun)) File.Delete (sfnRun);
		}

		public void CopyTemporaryRun (string strNewRunName)
		{
			string sfnTemporaryRun = GetDefaultRunFn ();
         	string sfnNewRun = Path.Combine(_ourLocation.GetBVTDir(), strNewRunName);

			if (File.Exists (sfnTemporaryRun))
			{
				File.Copy (sfnTemporaryRun, sfnNewRun, true);
			}
		}

		// FPrepareForTests
		// Mark documents for test, asks user if needed, etc.
		// Returns true if testing is ok
		// //not sure what fCompare only does so for now leave it out.
      public bool FPrepareForTests(KTest kTest/*, bool fCompareOnly*/)
      {
         int iTask, idoc;
         int nDocumentForTest;
         int nEnabledDocuments = 0;
         bool fFullTest;

         _fPreparedForTests = false;

         if (!_fTestListValid)
         {
            try
            {
               EnumerateTests();
            }
            catch { return false; };

            // Review: In principle it is not neccesary to read old results if we are going to run ALL tests
            LoadResultsCore("", 0);
            _sfnSecondRun = "";
            InitializeDialog();
            InitializeQuery();

            _fTestListValid = true;
         };

         /* First, clear all and count number of enabled documents */
         for (iTask = 0; iTask < _nTasks; iTask++)
         {
            CTFolderTask Task = _rgFolderTasks[iTask];
            for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
            {
               Task.rgDocuments[idoc].fSelectedForTest = false;
               if (Task.rgDocuments[idoc].fEnable) nEnabledDocuments++;
            }
         }

         //for now all we do is all docs in szBVTDir
         if (kTest == KTest.All)
         {
            /* Test all documents */
            for (iTask = 0; iTask < _nTasks; iTask++)
            {
               CTFolderTask Task = _rgFolderTasks[iTask];
               for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
               {
                  Task.rgDocuments[idoc].fSelectedForTest = Task.rgDocuments[idoc].fEnable;
               };
            }
         }
         //else if (kTest == KTest.AllCurrent)
         //{
         //   /* Test all current documents */
         //   for (iTask = 0; iTask < nTasks; iTask++)
         //   {
         //      CTFolderTask Task = rgFolderTasks[iTask];
         //      for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
         //      {
         //         Task.rgDocuments[idoc].fSelectedForTest = Task.rgDocuments[idoc].fEnable &&
         //                                          Task.rgDocuments[idoc].fVisible;
         //      };
         //   }
         //}
         //else if (kTest == KTest.Selected)
         //{
         //   /* Test selection */
         //   ListView.SelectedListViewItemCollection SelectedItems = listView1.SelectedItems;

         //   foreach (ListViewItem itemCur in SelectedItems)
         //   {
         //      int iviewCur = itemCur.Index;

         //      iTask = rgimapView2Task[iviewCur];
         //      idoc = rgimapView2Doc[iviewCur];

         //      CTFolderTask Task = rgFolderTasks[iTask];

         //      if (idoc != -1 && Task.rgDocuments[idoc].fEnable)
         //      {
         //         Task.rgDocuments[idoc].fSelectedForTest = true;
         //      };
         //   }
         //}
         else
         {
            Common.Assert(false);
         };

         nDocumentForTest = GetNumberTestedDocumentCore();
         fFullTest = nEnabledDocuments == nDocumentForTest;

         /* Various warning messages */
         if (nDocumentForTest == 0)
         {
            W11Messages.ShowWarning("No documents selected for test");
         }
         else
         {
            string strConfirmation = "Note that previous test results will be overwritten. Are you sure you want to continue?";
            if (fFullTest)
            {
               if (Common.CompareStr(_ourLocation.GetLogDir(), _ourLocation.options.szLogDir) == 0)
               {
                  /* Do not ask confirmation for BVT */
                  _fPreparedForTests = true;
               }
               else
               {
                  _fPreparedForTests = Ask.YesNoDialog("Full run:  " + _ourLocation.GetLogDir() + "\n\n" +
                                               "Documents: " + nDocumentForTest.ToString() + "\n\n" +
                                              strConfirmation, true);
               }
            }
            else
            {
               /* Always ask confirmation for selection */
               _fPreparedForTests = Ask.YesNoDialog("Selected run:  " + _ourLocation.GetLogFile() + "\n\n" +
                                           "Documents: " + nDocumentForTest.ToString() + " *** SELECTION ***" + "\n\n" +
                                            strConfirmation, true);
            }
         };

         /* Do neccesary cleanups */
         if (_fPreparedForTests)
         {
            /* Save temporary run file */

            //if (File.Exists(GetDefaultRunFn()))
            //{
            //   try
            //   {
            //      File.Copy(GetDefaultRunFn(), Path.Combine(OurLocation.GetLogFile(), "temporary.backup.run"), true);
            //   }
            //   catch { /* do not care */ };
            //}
         }

         //if (fPreparedForTests && !fCompareOnly)
         //{
            /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
            /* Delete layout.temporary files */
            /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
            //no layout files to delete, remove before final checkin
            //   if (fFullTest)
            //   {
            //      string spathBackupLayout = Path.Combine (OurLocation.GetBVTDir, "Layout.~backup~");
            //      string spathOutputLayout = Path.Combine (OurLocation.GetBVTDir, OurLocation.StrOutputFolderName);

            //      if (Directory.Exists (spathOutputLayout))
            //      {
            //         if (Directory.Exists (spathBackupLayout))
            //         {
            //            Common.DeleteFolder (spathBackupLayout);
            //         };

            //         Common.RenameFolder (spathOutputLayout, spathBackupLayout);
            //      };
            //   }
            //   else
            //   {
            //      /* Delete only temporary files of those documents, marked for test */
            //      for (iTask = 0; iTask < nTasks; iTask++)
            //      {
            //         CTFolderTask Task = rgFolderTasks [iTask];
            //         for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
            //         {
            //            if (Task.rgDocuments [idoc].fSelectedForTest)
            //            {
            //               string sfnLayoutTempXml = Path.Combine (Task.GetPathLayoutOutput (OurLocation), 
            //                  Path.GetFileNameWithoutExtension  (Task.rgDocuments [idoc].strDocname) + ".xml");

            //               if (File.Exists (sfnLayoutTempXml))
            //               {
            //                  try 
            //                  {
            //                     File.Delete (sfnLayoutTempXml);
            //                  }
            //                  catch { W11Messages.ShowError ("Unable to delete " + sfnLayoutTempXml);};
            //               };
            //            }
            //         }
            //      }
            //   }
            //}

            if (_fPreparedForTests)
            {
               // Clear results

               for (iTask = 0; iTask < _nTasks; iTask++)
               {
                  CTFolderTask Task = _rgFolderTasks[iTask];
                  for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
                  {
                     if (!Task.rgDocuments[idoc].fEnable || Task.rgDocuments[idoc].fSelectedForTest)
                     {
                        Task.rgDocuments[idoc].kRunOld = Task.rgDocuments[idoc].rgRun[1].kRun;

                        if (Task.rgDocuments[idoc].rgRun[0].kRun != KRun.NotRun)
                        {
                           Task.rgDocuments[idoc].rgRun[0].Initialize();
                           UpdateDocumentView(iTask, idoc);
                        }
                     }
                  };
               };

               /* Save current temporary results */
               SaveResultsCore("");
            }

            _nTestErrorsFixed = 0;	// for progres bar
            _nTestErrorsNew = 0;		// for progress bar
            _nTestErrors = 0;		// for progress bar

         return _fPreparedForTests;
      }

		public bool FGetNextTestFolder ( int iFolderPrev /* -1 for the first one */,
			/*out string spathDoc,*/
			out string [] rgstrDocnames,
			out int [] rgiddoc,
			out int iFolderNew )
		{
			int iTask;

			Common.Assert (iFolderPrev >= -1 && iFolderPrev < _nTasks);
			Common.Assert (_fPreparedForTests);
			Common.Assert (!_fRunning);

			iTask = iFolderPrev + 1;

			/* Search for task with documents for testing */
			while (iTask < _nTasks)
			{
				CTFolderTask Task = _rgFolderTasks [iTask];
				int idoc;
				int nTestDocuments = 0;

				for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
				{
					if (Task.rgDocuments [idoc].fSelectedForTest) nTestDocuments ++;
				}

				if (nTestDocuments > 0)
				{
					int itestdoc;
					/* Found new folder with nonzero number of test documents, return it */
					rgstrDocnames = new string [nTestDocuments];
					rgiddoc = new int [nTestDocuments];

					itestdoc = 0;
					for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
					{
						if (Task.rgDocuments [idoc].fSelectedForTest) 
						{
                     		rgstrDocnames[itestdoc] = Task.GetPathDoc(_ourLocation) + @"\" + Task.rgDocuments[idoc].strDocname;
							rgiddoc [itestdoc] = idoc;
							itestdoc++;
						};						
					}

					Common.Assert (itestdoc == nTestDocuments);

					/* Prepare other outputs and return */
					//spathDoc = Task.GetPathDoc (OurLocation);
					iFolderNew = iTask;

					return true;
					}

				iTask ++;
         }//while (iTask < nTasks)

			//spathDoc = "";
         //spathLayout = "";
         //spathLayoutOutput = "";
         //kLanguage = 0;
         //fSaveLayout = false;
         //kPage = 0;
			iFolderNew = 0;
			rgstrDocnames = null;
			rgiddoc = null;

			return false; /* Not found */
	   }

		// SetDocumentTestResult
		public void SetDocumentTestResult (int iTask, int idoc, bool fRestart,
			int kRun, string strFlags, string strError)
		{
			CTFolderTask Task = _rgFolderTasks [iTask];
			bool fError = KRun.IsError (kRun);

			Common.Assert (_fPreparedForTests);

			Task.rgDocuments [idoc].rgRun [0].fRestart = fRestart;
			Task.rgDocuments [idoc].rgRun [0].kRun = kRun;
			Task.rgDocuments [idoc].rgRun [0].strFlags = strFlags;
			Task.rgDocuments [idoc].rgRun [0].strError = strError;

			if (Task.rgDocuments [idoc].kRunOld != KRun.NotRun)
			{
				bool fErrorOld = KRun.IsError (Task.rgDocuments [idoc].kRunOld);

				if (fError && !fErrorOld) _nTestErrorsNew++;
				if (!fError && fErrorOld) _nTestErrorsFixed++;
			}

			if (fError) _nTestErrors++;

			UpdateDocumentView (iTask, idoc);

			AppendResultCore (iTask, idoc);
		}

		// Functions that return number of current errors
		public int GetNumberOfTestErrorsNew ()
		{
			Common.Assert (_fPreparedForTests);
			return this._nTestErrorsNew;
		}

		public int GetNumberOfTestErrorsFixed ()
		{
			Common.Assert (_fPreparedForTests);
			return this._nTestErrorsFixed;
		}

		public int GetNumberOfTestErrors ()
		{
			Common.Assert (_fPreparedForTests);
			return this._nTestErrors;
		}

		// GetFRunTestsAfterClosing
		public bool GetFRunTestsAfterClosing ()
		{
			return _fRunTestsAfterClosing;
		}

		// GetNumberTestedDocument
		public int GetNumberTestedDocument ()
		{
			Common.Assert (_fPreparedForTests);
			return GetNumberTestedDocumentCore ();
		}

		// Terminate : called when main window is closing\
		public void Terminate ()
		{
			lock (this)
			{
				Common.Assert (_fRunning);
				_fTerminating = true;

				this.Close ();
			};
		}

		// RunDialogAsync - thread for running dialog
        [STAThreadAttribute]
        void RunDialogAsync ()
		{
			_labelExistingWordWarning.Visible = false;

			_fRecalcStatistics = true; // Statistics could change after test
			UpdateDialog (false);

			ShowDialog (Parent);

			SaveUnsavedDocumentProperties ();
			_fRunning = false;

			// Send notification that dialog was closed
			if (!_fTerminating) _formParent.Invoke (_delegateFinished);
		}

		// RebuildViewList - populates view list, creates map
		void RebuildViewList ()
		{
			bool fAllowDataExchange_Saved = _fAllowDataExchange;
			int nDocumentsVisible = 0;
			int nTasksVisible = 0;
			int idocGlobal;
			int iTask;
			int iView;
			System.Drawing.Font FontSelectionItem = new System.Drawing.Font
						("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, 
						 System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

			/* Disallow data exchange during initialization */
			_fAllowDataExchange = false;

			/* Count number of documents */
			for (iTask = 0; iTask < _nTasks; iTask++) 
			{
				CTFolderTask task = _rgFolderTasks [iTask];
				bool fTaskVisible = false;

				for (int idoc = 0; idoc < task.Length; idoc++)
				{
					if (task.rgDocuments [idoc].fVisible)
					{
						nDocumentsVisible += 1;
						fTaskVisible = true;
					}
				}
				if (fTaskVisible) nTasksVisible += 1;
			}

			/* Allocate map arrays */
			_rgimapView2Task = new int [nTasksVisible + nDocumentsVisible];
			_rgimapView2Doc = new int [nTasksVisible + nDocumentsVisible];

			/* Disable list during population */
			if (nDocumentsVisible > 300)
			{
				_listView1.Hide ();
				_groupBoxUpdatingBackground.Update ();
			};

			/* Erase current view list */
			_listView1.Items.Clear ();

			idocGlobal = 0; // Current document
			iView = 0; // Current list item

			/* Loop through tasks */
			for (iTask = 0; iTask < _nTasks; iTask++)
			{
				CTFolderTask task = _rgFolderTasks [iTask];
				bool fTaskVisible = false;

				for (int idoc = 0; idoc < task.Length; idoc++)
				{
					// task.rgDocuments [idoc].Item = null;
					if (task.rgDocuments [idoc].fVisible) fTaskVisible = true;
				};

				if (fTaskVisible)
				{
					// Create folder entry in the list view
					ListViewItem itemFolder = new ListViewItem (new string [_listView1.Columns.Count]);

					itemFolder.SubItems [_columnN.Index].Text = "-----";
					//itemFolder.SubItems [ColumnDoc.Index].Text = task.GetPathDoc (OurLocation);
					itemFolder.ForeColor = Color.Blue;
					_listView1.Items.Add (itemFolder);

					// Mapping for this view item
					_rgimapView2Task [iView] = iTask;
					_rgimapView2Doc [iView] = -1; // No document for task entry

					task.iview = iView; // Mapping from task to iview

					iView++;

					// Loop through document in the task
					for (int idoc = 0; idoc < task.Length; idoc++)
					{
						if (task.rgDocuments [idoc].fVisible)
						{
							// Create document entry in the list view
							ListViewItem itemDocument = new ListViewItem (new string [_listView1.Columns.Count]);

							itemDocument.UseItemStyleForSubItems = false;
							itemDocument.SubItems [_columnN.Index].Text = task.rgDocuments [idoc].iglobal.ToString ();
							itemDocument.SubItems [_columnDoc.Index].Text = task.rgDocuments [idoc].strDocname;
							itemDocument.SubItems [_columnFlagged.Index].ForeColor = Color.DarkOrange;
							itemDocument.SubItems [_columnFlagged.Index].Font = FontSelectionItem;

							_listView1.Items.Add (itemDocument);

							// Mapping for this view item
							_rgimapView2Task [iView] = iTask;
							_rgimapView2Doc [iView] = idoc;

							task.rgDocuments [idoc].Item = itemDocument;

							UpdateDocumentViewCore (task.rgDocuments [idoc]);

							iView++;
							idocGlobal ++;
						};
					}
				}
				else
				{
					/* Task is not visible */
					task.iview = -1;
				}
			}

			Common.Assert (iView == nTasksVisible + nDocumentsVisible);

			/* Enable list after population */
			_listView1.Show ();

			InitializeSelection ();

			/* Restore data exchange */
			_fAllowDataExchange = fAllowDataExchange_Saved;
		}

		// InitializeDialog - initializes all dialog controls,
		// populates view list, creates map
		void InitializeDialog ()
		{
			bool fAllowDataExchange_Saved = _fAllowDataExchange;

			/* Disallow data exchange during initialization */
			_fAllowDataExchange = false;

			/* 1. Populate the view list & map */
			RebuildViewList ();

			/* 2. Initialize other dialog fields, selection */
			_fDocEnabled = true;

			InitializeQuery ();

			InitializeSelection ();

			_fRecalcStatistics = true;

			/* Restore data exchange */
			_fAllowDataExchange = fAllowDataExchange_Saved;

			UpdateDialog (false);
		}

		// UpdateAllVisibleDocumentsView - updates view for all
		// visible documents
		void UpdateAllVisibleDocumentsView ()
		{
			int iTask;
			int idoc;
			int nVisibleDocuments = 0;

			for (iTask = 0; iTask < _nTasks; iTask++)
			{
				CTFolderTask Task = _rgFolderTasks [iTask];

				for (idoc = 0; idoc < Task .rgDocuments.Length; idoc++)
					if (Task.rgDocuments [idoc].fVisible)
					{
						nVisibleDocuments ++;
					}
			};

			/* Disable list during population */
			if (nVisibleDocuments > 300)
			{
				_listView1.Hide ();
				_groupBoxUpdatingBackground.Update ();
			};

			for (iTask = 0; iTask < _nTasks; iTask++)
			{
				CTFolderTask Task = _rgFolderTasks [iTask];

				for (idoc = 0; idoc < Task .rgDocuments.Length; idoc++)
					if (Task.rgDocuments [idoc].fVisible)
					{
						UpdateDocumentViewCore (Task.rgDocuments [idoc]);
					}
			};

			_listView1.Show ();
		}

		// UpdateDocumentViewCore - updates view for given document
		void UpdateDocumentViewCore (SDocument document)
		{
			ListViewItem itemDocument = document.Item;

			/* Set comment field */
			itemDocument.SubItems [_columnC.Index].Text = document.strComment;

			// itemDocument.SubItems [ColumnC.Index].Text = document.strComment;

			int irun;

			itemDocument.SubItems [_columnRestart.Index].Text = (document.rgRun [0].fRestart ? "r" : "");
			itemDocument.SubItems [_columnFlags.Index].Text = document.rgRun [0].strFlags;

			for (irun=0; irun < 2; irun++)
			{
				string strError;
				string strRun;
				Color colorError;
				Color colorRun;

				int kRun = document.rgRun [irun].kRun;
				int iruncol = (irun == 0 ? _columnRun.Index : _columnRun2.Index);
				int ierrcol = (irun == 0 ? _columnError.Index : _columnError2.Index);

				GetRunDisplayText (kRun, document.fEnable, out strRun, out colorRun);
				GetErrorDisplayText (kRun, document.rgRun [irun].strError, out strError, out colorError);

				itemDocument.SubItems [iruncol].ForeColor = colorRun;
				itemDocument.SubItems [iruncol].Text = strRun;

				itemDocument.SubItems [ierrcol].ForeColor = colorError;
				itemDocument.SubItems [ierrcol].Text = strError;
			}

			if (document.fSelected) 
			{
				/* Selected document */
				itemDocument.SubItems [_columnFlagged.Index].Text = "X";
			}
			else 
			{
				/* Document is not selected */
				itemDocument.SubItems [_columnFlagged.Index].Text = "";
			}
		}

		// GetRunAndErrorText
		static void GetRunDisplayText (int kRun, bool fEnabled, out string strText, out Color color)
		{
			if (fEnabled)
			{
				strText = KRun.ToDisplay (kRun);

				if (KRun.IsError (kRun)) color = Color.Red;
				else color = Color.Green;
			}
			else
			{
				strText = "SKIP";
				color = Color.Black;
			};
		}

		// GetErrorDisplayText
		static void GetErrorDisplayText (int kRun, string strError, out string strText, out Color color)
		{
			color = Color.Black;

			if (KRun.IsError (kRun))
			{
				strText = KRun.ToErrorString (kRun) + ": " + strError;
			}
			else
			{
				strText = strError;
			};
		}

		// UpdateDocumentView  - updates view for given document		
		void UpdateDocumentView (int iTask, int idoc)
		{
			CTFolderTask task = _rgFolderTasks [iTask];
			UpdateDocumentViewCore (task.rgDocuments [idoc]);
			_listView1.Update ();
		}

		// This method updates dialog controls to reflect new status
		void UpdateDialog (bool fEditingChange)
		{
			string strHelp;

			if (!_fAllowDataExchange) return; /* Data exchange not allowed => return */
			_fAllowDataExchange = false; // Prohibit data exchange during update

			if (fEditingChange) 
			{
				// Update current state based on user change

				int iviewNew = -1;
				ListView.SelectedListViewItemCollection SelectedItems;

				/* Update document properties first */
				if (_textBoxComment.Enabled)
				{
					int iTask;
					int idoc;

					/* Document properties may be changed */
					Common.Assert (_checkBoxDocumentDisabled.Enabled);
					Common.Assert (_iviewSelection != -1);

					_fDocEnabled = !_checkBoxDocumentDisabled.Checked;
					_strDocComment = _textBoxComment.Text;

					iTask  = _rgimapView2Task [_iviewSelection];
					idoc = _rgimapView2Doc [_iviewSelection];

					Common.Assert (idoc != -1);

					if (FModifyDocumentProperties (iTask, idoc, _strDocComment, _fDocEnabled))
					{
						UpdateDocumentView (iTask, idoc);
					};
				}

				/* Take care of selection change, if any */

				SelectedItems = _listView1.SelectedItems;

				if (SelectedItems.Count == 1)
				{
					foreach (ListViewItem itemSel in SelectedItems) iviewNew = itemSel.Index;
				};

				if (iviewNew != _iviewSelection)
				{
					// Save previous changes in the current document ini
					SaveUnsavedDocumentProperties ();

					/* Set new selection and change property state fields */
					_iviewSelection = iviewNew;
					ModifyPropertyStateFieldsAfterSelectionChange ();
				};

				// if (textBoxQuery.Enabled) strQuery = textBoxQuery.Text;
				if (_comboBoxQuery.Enabled) _strQuery  = _comboBoxQuery.Text;
			}

			// Now modify dialog based on current state 
			if (_buttonStop.Enabled != _fRunningWord)
			{
				/* fRunning Word changed */

				_buttonStop.Enabled = _fRunningWord;

				if (_fRunningWord) _buttonStop.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
				else _buttonStop.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			}
		
			if (_fRunningWord) _labelExistingWordWarning.Visible = false;

			_checkBoxDocumentDisabled.Checked = !_fDocEnabled;
			if (_textBoxComment.Text != _strDocComment) _textBoxComment.Text = _strDocComment;

			if (_iviewSelection == -1)
			{
				/* No selection or multiple selection */
				_checkBoxDocumentDisabled.Enabled = false;
				_textBoxComment.Enabled = false;
				_buttonXmlDiff.Enabled = false;
				_menuItemCompareXml.Enabled = false;
				_buttonWord.Enabled = false;
				_menuItemOpenWord.Enabled = false;
				_labelFile.Text = "";
				_labelLanguage.Text = "";
				_labelPage.Text = "";
			}
			else
			{
				CTFolderTask Task;
				int idoc;
				int iTask = _rgimapView2Task [_iviewSelection];

				Task = _rgFolderTasks [iTask];
				idoc = _rgimapView2Doc [_iviewSelection];

				//labelLanguage.Text = KLanguage.ToPresentationString (Task.kLanguage);
				//labelPage.Text = KPage.ToPresentationString (Task.kPage);

				if (idoc == -1)
				{
					/* Folder selection */
					_labelFile.Text = "";
					_checkBoxDocumentDisabled.Enabled = false;
					_textBoxComment.Enabled = false;

					_buttonXmlDiff.Enabled = true;
					_menuItemCompareXml.Enabled = true;

					_buttonWord.Enabled = false;
					_menuItemOpenWord.Enabled = false;
				}
				else
				{
					/* Document selection */
					//labelFile.Text = Path.Combine (Task.GetPathDoc (OurLocation), Task.rgDocuments [idoc].strDocname);

					_buttonXmlDiff.Enabled = true;
					_menuItemCompareXml.Enabled = true;

					_buttonWord.Enabled = true;
					_menuItemOpenWord.Enabled = true;

					_textBoxComment.Enabled = true;
					_checkBoxDocumentDisabled.Enabled = true;

					if (FModifyDocumentProperties (iTask, idoc, _strDocComment, _fDocEnabled))
					{
						UpdateDocumentView (iTask, idoc);
					};
				};
			}

			/* Search control */

			// if (textBoxQuery.Text != strQuery) textBoxQuery.Text = strQuery;
			if (_comboBoxQuery.Text != _strQuery) _comboBoxQuery.Text = _strQuery;

			string strQuerySetLabelText = (_strQuerySet == "" ? "No query currently set" : _strQuerySet);

			if (_labelCurrentQuery.Text != strQuerySetLabelText) _labelCurrentQuery.Text = strQuerySetLabelText;

			if (_strQuerySet != "")
			{
				_buttonClearQuery.Enabled = true;
				_buttonClearQuery.BackColor = Color.Khaki;
			}
			else
			{
				_buttonClearQuery.Enabled = false;
				_buttonClearQuery.BackColor = _colorButtonBackgroundDefault;
			}

			_buttonLoadQuery.Enabled = _strQuery != "";
			_buttonFindNext.Enabled = _iviewSelection != -1 && _strQuery != "";
			_buttonFindPrev.Enabled = _iviewSelection != -1 && _strQuery != "";
			_buttonFindFirst.Enabled = _strQuery != "";
			_buttonCountAll.Enabled = _strQuery != "";

			/* Test statistics */

			if (_fRecalcStatistics)
			{
				RecalcTestStatistics ();
				_fRecalcStatistics = false;
			};

			_labelStatAllDocs.Text = _nDocuments_stat.ToString ();
			_labelStatAllRun.Text = _nRun_stat.ToString ();
			_labelStatAllErrors.Text = _nErrors_stat.ToString ();

			_labelStatCurDocs.Text = _nDocumentsVisible_stat.ToString ();
			_labelStatCurRun.Text = _nRunVisible_stat.ToString ();
			_labelStatCurErrors.Text = _nErrorsVisible_stat.ToString ();

			_labelStatSelDocs.Text = _nDocumentsMarked_stat.ToString ();
			_labelStatSelRun.Text = _nRunMarked_stat.ToString ();
			_labelStatSelErrors.Text = _nErrorsMarked_stat.ToString ();

			/* Set current run information */
			if (string.Compare (_sfnCurrentRun, GetDefaultRunFn (), true) == 0)
			{
				strHelp = "Test List: [ " + "Temporary" + " ]";
				}
			else 
			{
				strHelp = "Test List: [ " + _sfnCurrentRun + " ]";
			}

			if (_sfnSecondRun != "") strHelp += " vs [" + _sfnSecondRun + " ]";

			this.Text = strHelp;

			_fAllowDataExchange = true; // Restore data exchange after update
		}

		// UpdateSelection
		void UpdateSelection (int iviewNew)
		{
			bool fAllowDataExchange_saved = _fAllowDataExchange;

			_fAllowDataExchange = false;

			if (_iviewSelection != -1)
			{
				/* Undo single old selection */
				_listView1.Items [_iviewSelection].Selected = false;
			}
			else
			{
				/* Undo multiple old selection */
				foreach (ListViewItem itemCur in _listView1.Items)
				{
					if (itemCur.Selected) itemCur.Selected = false;
				};
			};

			_iviewSelection = iviewNew;

			if (_iviewSelection != -1)
				{
				_listView1.Items [_iviewSelection].Selected = true;
				_listView1.Items [_iviewSelection].Focused = true;
				}

			ModifyPropertyStateFieldsAfterSelectionChange ();

			_fAllowDataExchange = fAllowDataExchange_saved;

			UpdateDialog (false);
		}

		void InitializeSelection ()
		{
			Common.Assert (!_fAllowDataExchange);

			foreach (ListViewItem itemCur in _listView1.Items)
			{
				itemCur.Selected = false;
				itemCur.Focused = false;
			};

			if (_listView1.Items.Count > 1)
			{
				_iviewSelection = 1;

				_listView1.Items [_iviewSelection].Selected = true;
				_listView1.Items [_iviewSelection].Focused = true;
			}
			else
			{
				_iviewSelection = -1;
			};

			ModifyPropertyStateFieldsAfterSelectionChange ();
		}

		// MakeSureSelectionIsVisible 
		void MakeSureSelectionIsVisible ()
		{
			ListView.SelectedListViewItemCollection SelectedItems;

			SelectedItems = _listView1.SelectedItems;

			foreach (ListViewItem item in SelectedItems)
			{
				_listView1.EnsureVisible (item.Index);
				return;
			}
		}

		// ModifyPropertyStateFieldsAfterSelectionChange
		void ModifyPropertyStateFieldsAfterSelectionChange ()
		{
			int idoc;
			CTFolderTask Task;

			if (_iviewSelection != -1)
			{
				Task = _rgFolderTasks [_rgimapView2Task [_iviewSelection]];
				idoc = _rgimapView2Doc [_iviewSelection];

				if (idoc == -1)
				{
					// Document is not selected
					_fDocEnabled = true;
					_strDocComment = "";
				}
				else
				{
					_fDocEnabled = Task.rgDocuments [idoc].fEnable;
					_strDocComment = Task.rgDocuments [idoc].strComment;
				};
			}
			else
			{
				// Document is not selected
				_fDocEnabled = true;
				_strDocComment = "";
			}
		}

		// EditingChange: notification about any kind of editing change
		private void EditingChange(object sender, System.EventArgs e)
		{
			/* Just call update dialog to take care of all changes */
			UpdateDialog (true);		
		}

		// listView1_SelectedIndexChanged: notificaton about selection change
		private void listView1_SelectedIndexChanged (object sender, System.EventArgs e)
		{
			UpdateDialog (true);
		}

		// GetNumberTestedDocumentCore
		//retrieve the number of documents selected for test
		int GetNumberTestedDocumentCore ()
		{
			int nDocs = 0;

			for (int iTask = 0; iTask < _nTasks; iTask++)
			{
				CTFolderTask Task = _rgFolderTasks [iTask];
				for (int idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
				{
					if (Task.rgDocuments [idoc].fSelectedForTest) nDocs++;

				}
			}

			return nDocs;
		}

		// RecalcTestStatistics
		void RecalcTestStatistics ()
		{
			_nDocuments_stat = 0;
			_nErrors_stat = 0;
			_nRun_stat = 0;
			_nDocumentsVisible_stat = 0;
			_nErrorsVisible_stat = 0;
			_nRunVisible_stat = 0;
			_nDocumentsMarked_stat = 0;
			_nErrorsMarked_stat = 0;
			_nRunMarked_stat = 0;

			for (int iTask = 0; iTask < _nTasks; iTask++)
			{
				CTFolderTask Task = _rgFolderTasks [iTask];
				for (int idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
				{
					bool fVisible = Task.rgDocuments [idoc].fVisible;

					_nDocuments_stat ++;
					if (fVisible) _nDocumentsVisible_stat++;

					if (Task.rgDocuments [idoc].rgRun [0].kRun != KRun.NotRun)
					{
						_nRun_stat++;
						if (fVisible) _nRunVisible_stat++;

						if (KRun.IsError (Task.rgDocuments [idoc].rgRun [0].kRun))
						{
							_nErrors_stat++;
							if (fVisible) _nErrorsVisible_stat++;
						}
					}

					if (Task.rgDocuments [idoc].fSelected)
					{
						_nDocumentsMarked_stat++;

						if (KRun.IsError (Task.rgDocuments [idoc].rgRun [0].kRun)) _nErrorsMarked_stat++;
						if (Task.rgDocuments [idoc].rgRun [0].kRun != KRun.NotRun) _nRunMarked_stat++;
					}

				}
			};
		}

		// GetDefaultRunFn 
		string GetDefaultRunFn ()
		{
			return Path.Combine (_ourLocation.GetLogDir(), "Temporary.run");
		}

		// LoadResultsCore
		void LoadResultsCore (string sfnRun, int irun)
		{
			if (sfnRun == "") sfnRun = GetDefaultRunFn ();

			if (irun == 0) _sfnCurrentRun = sfnRun;
			else _sfnSecondRun = sfnRun;

			TResFile.GetResults (_ourLocation, _ourLocation.GetLogDir(), _rgFolderTasks, _nTasks, sfnRun, irun);
		}

		// SaveResultsCore
		void SaveResultsCore (string sfnRun)
		{
			if (sfnRun == "") sfnRun = GetDefaultRunFn ();

			_sfnCurrentRun = sfnRun;

			TResFile.SaveResults ( _ourLocation, _ourLocation.GetLogDir(), _rgFolderTasks, _nTasks, 
								   Path.Combine (_ourLocation.GetLogDir(), sfnRun));
		}

		// AppendResultsCore
		void AppendResultCore (int iTask, int idoc)
		{
			// We can only append to the teporary results
			Common.Assert (_sfnCurrentRun == GetDefaultRunFn ());

        }

		// Add new task to the global array
		void AddNewTask ( 
			string srelpathDoc,
			string [] rgstrDocnames, 
			bool [] rgfEnableDoc, 
			string [] rgstrComments )
		{
			long nDocumentsAdded = rgstrDocnames.Length;

			/* Check realloc arrays */
			if (_nTasks >= _rgFolderTasks.Length)
			{
				CTFolderTask [] rgFolderTasksRealloc = new CTFolderTask [(_nTasks + 1)*2];

				for (int it=0; it < _nTasks; it++) rgFolderTasksRealloc [it] = _rgFolderTasks [it];
				_rgFolderTasks = rgFolderTasksRealloc;
			};

			_rgFolderTasks [_nTasks] = new CTFolderTask (srelpathDoc, rgstrDocnames, rgfEnableDoc, rgstrComments );

			_nTasks++;
		}

		// Notification that Word process has exited
		void FinishedWordNotification ()
		{
			Common.Assert (_fRunningWord);

			_fRunningWord = false;

			UpdateDialog (false);
      }

      #region //dialog Functions
	  
		// Designers dispose
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

		// Resize notification
		private void CTestList_Resize(object sender, System.EventArgs e)
		{
			/* Review (AntonS): This is hack! ListView size is wrong after minimize/maximize,
			 * so to fix it, I do not make adjustments if size is smaller then the minimum */

			if (Size.Width >= MinimumSize.Width && Size.Height >= MinimumSize.Height)
			{
				int dx = Size.Width - _sizePrevious.Width;
				int dy = Size.Height - _sizePrevious.Height;

				_listView1.Size = new Size (dx + _listView1.Size.Width, dy + _listView1.Size.Height);
				_groupBoxUpdatingBackground.Size = new Size (dx + _groupBoxUpdatingBackground.Size.Width,
					dy + _groupBoxUpdatingBackground.Size.Height );

				_labelExistingWordWarning.Location = new Point (_labelExistingWordWarning.Location.X + dx,
					_labelExistingWordWarning.Location.Y + dy);

				_groupBoxDocument.Location = new Point (_groupBoxDocument.Location.X, _groupBoxDocument.Location.Y + dy);
				_groupBoxTestStat.Location = new Point (_groupBoxTestStat.Location.X, _groupBoxTestStat.Location.Y + dy);
				_groupBoxTop.Size = new Size (_groupBoxTop.Size.Width + dx, _groupBoxTop.Size.Height);

				_labelFile.Location = new Point (_labelFile.Location.X, _labelFile.Location.Y + dy);
				_label13.Location = new Point (_label13.Location.X, _label13.Location.Y + dy);

				// textBoxComment.Size = new Size (textBoxComment.Size.Width + dx, textBoxComment.Size.Height);

				_sizePrevious = Size;
			}
		}

		// Close
		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			this.Hide ();
		}

		// Double click on document - selection
		void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			int iTask;
			int idoc;
			CTFolderTask Task;
			
			SaveUnsavedDocumentProperties ();

			if (_iviewSelection != -1)
			{
				iTask = _rgimapView2Task [_iviewSelection];
				idoc = _rgimapView2Doc [_iviewSelection];

				Task = _rgFolderTasks [iTask];

				if (idoc == -1)
				{
					int ndocsSelected = 0;
					int ndocsVisible = 0;
					bool fFolderSelection;
					// Trigger selection for all document in a folder
					// If number of selected documents < 1/2 => select, otherwise deselect

					for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
					{
						if (Task.rgDocuments [idoc].fSelected && Task.rgDocuments [idoc].fVisible) ndocsSelected++;
						if (Task.rgDocuments [idoc].fVisible) ndocsVisible++;
					}

					fFolderSelection = ndocsSelected * 2 < ndocsVisible;

					for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
					{
						if (Task.rgDocuments [idoc].fVisible)
						{
							FlagDocument (iTask, idoc, fFolderSelection);
						}
					}
				}
				else
				{
					/* Trigger selection for one document */
					FlagDocument (iTask, idoc, !Task.rgDocuments [idoc].fSelected);
				}
			}

			UpdateDialog (false);
		}

		// OpenSelectedDocument - action on user command
		void OpenSelectedDocument ()
		{

			// Check if we have only one selection and if selected folder or document
			ListView.SelectedListViewItemCollection SelectedItems = _listView1.SelectedItems;

			if (SelectedItems.Count == 1)
			{
				foreach (ListViewItem item in SelectedItems)
				{
					long iSelection = _listView1.Items.IndexOf (item);

					if (_rgimapView2Doc [iSelection] != -1)
					{
						CTFolderTask task = _rgFolderTasks [_rgimapView2Task [iSelection]];
						long idocTask = _rgimapView2Doc [iSelection];
            			string sfnDocument = "NYI";//Path.Combine (task.GetPathDoc (OurLocation), task.rgDocuments [idocTask].strDocname);

						_fRunningWord = _runwordObject.FStartWordOpenDoc ( _delegateFinishedWord, 
											_kTarget, sfnDocument, "", true /* Allow use existing Word */ )
									   || _fRunningWord;

						if (!_fRunningWord) _labelExistingWordWarning.Visible = true;
					}
				}
			}
		}

		// Run specified file in Word
		private void buttonWord_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();

			try 
			{
				OpenSelectedDocument ();
				UpdateDialog (false);
			}
			catch (W11Exception) {}
			catch (Exception ex) { W11Messages.ShowExceptionError (ex); };
		}

		// Handle keyboard shortcuts
		private void listView1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F4)
			{
				buttonWord_Click (sender, e);
			}
			else if (e.KeyCode == Keys.F1)
			{
				//buttonXmlDiff_Click (sender, e);
			}
			else if (e.KeyCode == Keys.Escape)
			{
				this.Hide ();
			}
		}

		// User clicks on STOP button
		private void buttonStop_Click(object sender, System.EventArgs e)
		{
			Common.Assert (_fRunningWord);

			_runwordObject.KillWord ();
		}

		// User clicks on Clear button
		private void buttonClear_Click(object sender, System.EventArgs e)
		{
			_strDocComment = "";
			_fDocEnabled = true;

			UpdateDialog (false);		
		}

		// See xml diff
      //private void buttonXmlDiff_Click(object sender, System.EventArgs e)
      //{
      //   int iTask;
      //   int idoc;
      //   CTFolderTask Task;
			
      //   SaveUnsavedDocumentProperties ();

      //   if (iviewSelection != -1)
      //   {
      //      iTask = rgimapView2Task [iviewSelection];
      //      idoc = rgimapView2Doc [iviewSelection];

      //      Task = rgFolderTasks [iTask];

            //if (idoc != -1)
            //{
					/* Windiff for selected document */
               
               //not used, delete

					//string sfnLayoutXml;
					//string sfnLayoutTempXml;

					//sfnLayoutXml = Path.Combine (Task.GetPathLayoutCompare (OurLocation), 
						//Path.GetFileNameWithoutExtension (Task.rgDocuments [idoc].strDocname) + ".xml");
					//sfnLayoutTempXml = Path.Combine (Task.GetPathLayoutOutput (OurLocation), 
						//Path.GetFileNameWithoutExtension  (Task.rgDocuments [idoc].strDocname) + ".xml");

					/* Check if files exist */
               //if (!File.Exists (sfnLayoutXml))
               //{
               //   W11Messages.ShowError ("Can not find " + sfnLayoutXml);
               //   return;
               //};

               //if (!File.Exists (sfnLayoutTempXml))
               //{
               //   W11Messages.ShowError ("Can not find " + sfnLayoutTempXml);
               //   return;
               //};

					/* Check if file name contains unicode characters => raname for windiff */
               //if (Common.FContainsUnicodeCharacter (Task.rgDocuments [idoc].strDocname))
               //{
               //   string sfnLayoutXmlAscii;
               //   string sfnLayoutTempXmlAscii;

               //   try
               //   {
               //      sfnLayoutXmlAscii = Path.GetTempFileName ();
               //      sfnLayoutTempXmlAscii = Path.GetTempFileName ();

               //      /* Copy to temporaty files for Windiff */

               //      File.Copy (sfnLayoutXml, sfnLayoutXmlAscii, true);
               //      File.Copy (sfnLayoutTempXml, sfnLayoutTempXmlAscii, true);

               //      try 
               //      {
               //         Process processWindiff = Process.Start (OurLocation.GetFnWindiffExe (),
               //            "\"" + sfnLayoutXmlAscii + "\" " +
               //            "\"" + sfnLayoutTempXmlAscii + "\"");

               //         processWindiff.WaitForExit ();
               //      }
               //      catch { W11Messages.ShowError ("Unable to start windiff");}

                  //   /* Delete temporary files */

                  //   File.Delete (sfnLayoutXmlAscii);
                  //   File.Delete (sfnLayoutTempXmlAscii);
                  //}
      //            catch { W11Messages.ShowError ("Problems creating / deleting temporary files for windiff");}
      //         }
      //         else
      //         {
      //            try 
      //            {
      //               Process processWindiff = Process.Start (OurLocation.GetFnWindiffExe (),
      //                  "\"" + sfnLayoutXml + "\" " +
      //                  "\"" + sfnLayoutTempXml + "\"");
      //            }
      //            catch { W11Messages.ShowError ("Unable to start windiff");}

      //            /* Do not wait for exit */
      //         }
      //      }
      //      else
      //      {
      //         /* Windiff for selected folder */

      //         /* Check if paths exist */
      //         if (!Directory.Exists (Task.GetPathLayoutCompare (OurLocation)))
      //         {
      //            W11Messages.ShowError ("Can not find folder " + Task.GetPathLayoutCompare (OurLocation));
      //            return;
      //         };

      //         if (!Directory.Exists (Task.GetPathLayoutOutput (OurLocation)))
      //         {
      //            W11Messages.ShowError ("Can not find folder" + Task.GetPathLayoutOutput (OurLocation));
      //            return;
      //         };


      //         /* Run windiff for two folders */
      //         try 
      //         {
      //            Process processWindiff = Process.Start (OurLocation.GetFnWindiffExe (),
      //               "\"" + Task.GetPathLayoutCompare (OurLocation) + "\\\" " +
      //               "\"" + Task.GetPathLayoutOutput (OurLocation) + "\\\"");
      //         }
      //         catch { W11Messages.ShowError ("Unable to start windiff");}


      //      }

      //   };
      //}

		// Menu / File::Close
		private void menuItemFileClose_Click(object sender, System.EventArgs e)
		{
			this.Hide ();		
		}

		// Menu / Edit::SelectAll
		private void menuItemEditSelectAll_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();

			for (int item = 0; item < _listView1.Items.Count; item++)
			{
				_listView1.Items [item].Selected = true;
			};

			UpdateDialog (false);
		}

		// Menu / Edit::FlagAllCurrent
		private void menuItemEditFlagAllCurrent_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();
			FlagAllVisibleDocuments ();
			UpdateDialog (false);
		}

		// Menu / Edit::Clear flags
		private void menuItemClearFlags_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();
			ClearFlags ();
			UpdateDialog (false);
		}

		// Menu / Tests::Clear
		private void menuItemTestClear_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();

			for (int iTask = 0; iTask < _nTasks; iTask++)
			{
				CTFolderTask Task = _rgFolderTasks [iTask];
				for (int idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
				{
					if (Task.rgDocuments [idoc].rgRun [0].kRun != KRun.NotRun)
					{
						Task.rgDocuments [idoc].rgRun [0].Initialize ();
					}
				};
			};

			SaveResultsCore ("");
			UpdateAllVisibleDocumentsView ();
		}

		// Menu / File::UpdateList
		private void menuItemFileUpdateList_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();

			try
			{
				EnumerateTests ();
			}
			catch {};

			_sfnSecondRun = "";
			LoadResultsCore ("", 0);
			InitializeDialog ();
			InitializeQuery ();
		}

		// Menu / File::Save
		private void menuItemFileSave_Click(object sender, System.EventArgs e)
		{
			try 
			{
				SaveUnsavedDocumentProperties ();

				SaveFileDialog dlgSaveFile = new SaveFileDialog ();

				SaveUnsavedDocumentProperties ();

				dlgSaveFile.InitialDirectory = _ourLocation.GetLogDir();
				dlgSaveFile.Filter = "Run files (*.run) | *.run";
				dlgSaveFile.FilterIndex = 0;

				if (dlgSaveFile.ShowDialog () == DialogResult.OK)
				{
					SaveResultsCore (Path.Combine (_ourLocation.GetLogDir(), dlgSaveFile.FileName));
					UpdateDialog (false);
				}
			}
			catch (W11Exception) {}
			catch (Exception ex) { W11Messages.ShowExceptionError (ex); };
		}

		// Menu / File::Load
		private void menuItemFileLoad_Click(object sender, System.EventArgs e)
		{
			try 
			{
				OpenFileDialog dlgOpenFile = new OpenFileDialog ();

				SaveUnsavedDocumentProperties ();

				dlgOpenFile.InitialDirectory = _ourLocation.GetLogDir();
				dlgOpenFile.Filter = "Run files (*.run) | *.run";
				dlgOpenFile.FilterIndex = 0;

				if (dlgOpenFile.ShowDialog () == DialogResult.OK)
				{
					LoadResultsCore (dlgOpenFile.FileName, 0);
					UpdateAllVisibleDocumentsView ();
					_fRecalcStatistics = true;
					UpdateDialog (false);
				}
			}
			catch (W11Exception) {}
			catch (Exception ex) { W11Messages.ShowExceptionError (ex); };
		}

		// Menu / File::Open 2nd
		private void menuItemFileOpenSecond_Click(object sender, System.EventArgs e)
		{
			try
			{
				OpenFileDialog dlgOpenFile = new OpenFileDialog ();

				SaveUnsavedDocumentProperties ();

            	dlgOpenFile.InitialDirectory = _ourLocation.GetLogDir();
				dlgOpenFile.Filter = "Run files (*.run) | *.run";
				dlgOpenFile.FilterIndex = 0;

				if (dlgOpenFile.ShowDialog () == DialogResult.OK)
				{
					if (string.Compare (dlgOpenFile.FileName, GetDefaultRunFn ()) == 0)
					{
						W11Messages.ShowWarning ("Can not load default result in the second run");
					}
					else
					{
						LoadResultsCore (dlgOpenFile.FileName, 1 /* Results for second run */);
						UpdateAllVisibleDocumentsView ();
						_fRecalcStatistics = true;
						UpdateDialog (false);
					}
				}
			}
			catch (W11Exception) {}
			catch (Exception ex) { W11Messages.ShowExceptionError (ex); };
		}

		// Menu / File::Clear 2nd
		private void menuItemFileClearSecond_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();

			for (int iTask = 0; iTask < _nTasks; iTask++)
			{
				CTFolderTask Task = _rgFolderTasks [iTask];
				for (int idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
				{
					if (Task.rgDocuments [idoc].rgRun [1].kRun != KRun.NotRun)
					{
						Task.rgDocuments [idoc].rgRun [1].Initialize ();
					}
				};
			};

			_sfnSecondRun = "";

			UpdateAllVisibleDocumentsView ();
			_fRecalcStatistics = true;
			UpdateDialog (false);
		}

		// Menu / Main ::Export Excel All
		private void menuItemExportExcelAll_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();
			ExportExcelCore (false);
		}

		// Menu / Main ::Export Excel All
		private void menuItemExportExcelSelection_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();
			ExportExcelCore (true);
		}

		//	Export excel - core function
		void ExportExcelCore (bool fSelection)
		{
			string sfnExcelOutput;
			int iTask, idoc;
			StreamWriter stream;
			SaveFileDialog dlgSaveFile = new SaveFileDialog ();

         dlgSaveFile.InitialDirectory = _ourLocation.GetLogDir();
			dlgSaveFile.Filter = "Excel report (*.xls) | *.xls";
			dlgSaveFile.FilterIndex = 0;

			if (dlgSaveFile.ShowDialog () != DialogResult.OK)
			{
				return;
			}

			sfnExcelOutput = dlgSaveFile.FileName;

			try
			{
				try { stream = new StreamWriter (sfnExcelOutput, false); }
				catch
				{
					W11Messages.RaiseError ("Unable to open file " + sfnExcelOutput + ")");
					return; 
				};

				/* Mark documents for outpit */

				if (fSelection)
				{
					ListView.SelectedListViewItemCollection SelectedItems = _listView1.SelectedItems;

					for (iTask = 0; iTask < _nTasks; iTask++)
						for (idoc = 0; idoc < _rgFolderTasks [iTask].rgDocuments.Length; idoc++)
							_rgFolderTasks [iTask].rgDocuments [idoc].fExcelOutputTemp = false;

					foreach (ListViewItem itemCur in SelectedItems) 
					{
						int iviewCur  = itemCur.Index;

						iTask = _rgimapView2Task [iviewCur];
						idoc = _rgimapView2Doc [iviewCur];

						CTFolderTask Task = _rgFolderTasks [iTask];

						if (idoc != -1 && Task.rgDocuments [idoc].fEnable)
						{
							Task.rgDocuments [idoc].fExcelOutputTemp = true;
						};
					}
				}
				else
				{
					for (iTask = 0; iTask < _nTasks; iTask++)
						for (idoc = 0; idoc < _rgFolderTasks [iTask].rgDocuments.Length; idoc++)
							_rgFolderTasks [iTask].rgDocuments [idoc].fExcelOutputTemp = true;
				}

				// Header line 
				stream.WriteLine (GetExportExcelHeader ());

				// Table
				for (iTask = 0; iTask < _nTasks; iTask++)
					for (idoc = 0; idoc < _rgFolderTasks [iTask].rgDocuments.Length; idoc++)
					{
						if (_rgFolderTasks [iTask].rgDocuments [idoc].fExcelOutputTemp)
						{
							stream.WriteLine ( GetExportExcelLine (iTask, idoc));
						}
					}

				stream.Close ();
			}
			catch (W11Exception) {}
			catch (Exception)
			{
				W11Messages.ShowError ("Error writing " + sfnExcelOutput);
			};
		}


		// Get header line for excel output
		static string GetExportExcelHeader ()
		{
			return "N" + KCharConst.tab + 
				"Document" + KCharConst.tab +
				"Run" + KCharConst.tab +
				"Run2" + KCharConst.tab +
				"Error" + KCharConst.tab +
				"Error2";
		}

		// Get line for excel output
		string GetExportExcelLine (int iTask, int idoc)
		{
			CTFolderTask Task = _rgFolderTasks [iTask];

			Color colorUnused;
			string strRun;
			string strError;
			string strRun2;
			string strError2;

			GetRunDisplayText (Task.rgDocuments [idoc].rgRun [0].kRun, Task.rgDocuments [idoc].fEnable,
				out strRun, out colorUnused );

			GetErrorDisplayText ( Task.rgDocuments [idoc].rgRun [0].kRun, 
				Task.rgDocuments [idoc].rgRun [0].strError,
				out strError, out colorUnused );

			GetRunDisplayText (Task.rgDocuments [idoc].rgRun [1].kRun, Task.rgDocuments [idoc].fEnable,
				out strRun2, out colorUnused );

			GetErrorDisplayText ( Task.rgDocuments [idoc].rgRun [1].kRun, 
				Task.rgDocuments [idoc].rgRun [1].strError,
				out strError2, out colorUnused );

			return Task.rgDocuments [idoc].iglobal.ToString () + KCharConst.tab + 
				   Path.Combine (Task.GetPathDoc (_ourLocation), Task.rgDocuments [idoc].strDocname) + KCharConst.tab +
				   strRun + KCharConst.tab +
				   strRun2 + KCharConst.tab +
				   strError + KCharConst.tab +
				   strError2;
		}

		// Menu / Test::Run current documents
		private void menuItemRunCurrentDocuments_Click(object sender, System.EventArgs e)
		{
			if (FPrepareForTests (KTest.AllCurrent/*, false*/))
			{
				/* Ready for the tests: close and run tests afterwards */
				_fRunTestsAfterClosing = true;
				this.Hide ();
			};
		}

		// Menu / Test::Run selection
		private void menuItemRunSelection_Click(object sender, System.EventArgs e)
		{
			if (FPrepareForTests (KTest.Selected/*, false*/))
			{
				/* Ready for the tests: close and run tests afterwards */
				_fRunTestsAfterClosing = true;
				this.Hide ();
			};
		}

      #endregion

		// Enumerates tests, including subfolders
		void EnumerateTests ()
		{
			int iTask;
			int idoc;
			int iglobal;
			try
			{
				// Clear current task list
				for (iTask = 0; iTask < _nTasks; iTask++) _rgFolderTasks [iTask] = null;
				_nTasks = 0;

				Common.Assert (_fAllDocumentPropertiesSaved);

				// Make sure the main test folder has just subfolders: docs, layout, [layout.temp]
            //CheckTestStructure(OurLocation.GetLogDir());

				/* Create collection of test tasks */
            
				EnumerateTestsCore ( _ourLocation.GetBVTDir());

				/* Set global document numbers */
				iglobal = 0;
				for (iTask = 0; iTask < _nTasks; iTask++)
				{
					for (idoc = 0; idoc < _rgFolderTasks [iTask].rgDocuments.Length; idoc++)
					{
						iglobal ++;
						_rgFolderTasks [iTask].rgDocuments [idoc].iglobal = iglobal;
					}
				};
			}
			catch (W11Exception)  { W11Messages.RaiseError ();}
			catch (Exception ex) { W11Messages.RaiseError ("Unhandled exception during enumeration " + ex.ToString ()); };

		} // End of EnumerateTests

		// Recursive funtion to enumerates tests, including subfolders
		void EnumerateTestsCore ( string spathrelDoc
					  			  /*bool fRecursiveSettingsImposed,*/  // Recursive setting imposed. //ignored for now
								  /*string sfnIniRecursive*/)			 // iff fRecursiveSettingImposed

		{
         string spathDoc = spathrelDoc;

         //string sfnIniCurrent = Path.Combine (spathDoc, OurLocation.GetNameTestIni ());
         //string sfnIni; // The one which are actually going to read
			string [] rgstrSubfolderNames;


         #region //not used currently, maybe when we add display stuff
         //bool fEnable;
         //bool fRecursive;


			// Check if there is ini file at all
         //if (!File.Exists (sfnIni))
         //{
         //   // Ccan not find ini file; must be not recursive
         //   if ( Ask.YesNoDialog ("Can not find setting " + OurLocation.GetNameTestIni () + " in test folder " + spathDoc + "\n\n" +
         //      "Would you like to create create ini file with the following default settings?\n\n" +
         //      "Page = Word              ; // page option not supported in Word 12 \n" +
         //      "Language = English\n" +
         //      "Save layout = On\n" +
         //      "Enable = On\n" +
         //      "Recursive = On\n" +
         //      "\n" +
         //      "If you do not want to continue, you can create ini manually. See example in the help menu", false))
         //   {
         //      try
         //      {
         //         //File.Copy (OurLocation.GetFnSampleTestIni (), sfnIni, false);
         //      }
         //      catch
         //      {
         //         //W11Messages.RaiseError ("Can not copy " + OurLocation.GetFnSampleTestIni () + " => " + sfnIni);
         //      }                    
         //   }
         //   else
         //   {
         //      W11Messages.RaiseError ();
         //      fRecursive = fRecursiveSettingsImposed;
         //   }
         //}

			// Process current folder
			// Read from ini file, it is either recursive or current
         //ReadTestIniFile (sfnIni, out fEnable, out fRecursive);

         //Common.Assert (!fRecursiveSettingsImposed || fRecursive);


         //For now I'll worry about just rtf files, but need to add 
         //possiblity of xaml as well.
		 
         //if (fEnable)
         //{
         #endregion
         //is there any way to combine the to file extension types?
         //string[] rgstrDocnames = Common.ReadFileNames(spathDoc, "*.rtf"); + 
            //   Common.ReadFileNames(spathDoc, "*.xaml");
         string[] rgRtfDocs = Common.ReadFileNames(spathDoc, "*.rtf");
         string[] rgXamlDocs = Common.ReadFileNames(spathDoc, "*.xaml");
         string[] rgstrDocnames = new string[rgRtfDocs.Length + rgXamlDocs.Length];

         for (int i = 0; i < rgRtfDocs.Length ; i++)
         {
            rgstrDocnames[i] = rgRtfDocs[i];
         }

         for (int i = 0; i < (rgXamlDocs.Length); i++)
         {
            rgstrDocnames[i + rgRtfDocs.Length] = rgXamlDocs[i];
         }
         
				// Remove temporary documents from the array, ask end-user to delete from disk
				//rgstrDocnames = RemoveTemporaryDocuments (Protocol, spathDoc, rgstrDocnames);
				Common.SortStringArray (rgstrDocnames);

				if (rgstrDocnames.Length != 0)
				{
					bool [] rgfEnableDoc;
					string [] rgstrComments;

					ReadDocumentIniFiles (spathDoc, rgstrDocnames, out rgfEnableDoc, out rgstrComments);

					// Add new task to the global array and view
					AddNewTask ( spathrelDoc, rgstrDocnames, rgfEnableDoc, rgstrComments );
				};
         //}

			/* Process subfolders recursively */
			rgstrSubfolderNames = Common.ReadDirectotyNames (spathDoc, "*.*");

			foreach (string strFolderCur in rgstrSubfolderNames)
			{
				EnumerateTestsCore ( Path.Combine (spathrelDoc, strFolderCur));
			}
		} //  End of EnumerateTestsCore


		// Reads test ini file
		static void ReadTestIniFile ( string sfnIni, out int kPage,  out int kLanguage, 
			out bool fSaveLayout,  out bool fEnable,
			out bool fRecursive )
		{
			CInifile inifile = new CInifile (sfnIni);
			string strPage;
			string strLanguage;
			string strSaveLayout;
			string strEnable;
			string strRecursive;

			try 
			{
				/* Kind of page layout */
				strPage = inifile.GetNext ("Page");
				kPage = KPage.FromPresentationString (strPage);
				if (kPage == -1) 
				{
					W11Messages.RaiseError ("Invalid page type in configuration file " + sfnIni);
				};

				/* Kind of language */
				strLanguage = inifile.GetNext ("Language");
				kLanguage = KLanguage.FromPresentationString (strLanguage);
				if (kLanguage == -1 || kLanguage == KLanguage.Other)
				{
					W11Messages.RaiseError ("Invalid language in configuration file " + sfnIni);
				};

				/* Saving layout? */
				strSaveLayout = inifile.GetNext ("Save layout").ToUpper ();
				if (strSaveLayout == "ON") fSaveLayout = true;
				else if (strSaveLayout == "OFF") fSaveLayout = false;
				else
				{
					W11Messages.RaiseError ("Invalid value of <Save Layout> in configuration file " + sfnIni);
					fSaveLayout = false;
				};

				/* Test enabled? */
				strEnable = inifile.GetNext ("Enable").ToUpper ();
				if (strEnable == "ON") fEnable = true;
				else if (strEnable == "OFF") fEnable = false;
				else
				{
					W11Messages.RaiseError ("Invalid value of <Enable> in configuration file " + sfnIni);
					fEnable = false;
				};

				/* Recursive? */
				strRecursive = inifile.GetNext ("Recursive").ToUpper ();
				if (strRecursive == "ON") fRecursive = true;
				else if (strRecursive == "OFF") fRecursive = false;
				else
				{
					W11Messages.RaiseError ("Invalid value of <Recursive> in configuration file " + sfnIni);
					fRecursive = false;
				};

				inifile.Close ();
			}
			finally
			{
				// Cleanup
				inifile.Close ();				
			}
		}

		// D O C U M E N T   F I L E   C O M M A N D S
		
		// MenuItem Open Word
		private void menuItemOpenWord_Click(object sender, System.EventArgs e)
		{
			buttonWord_Click (sender, e);
		}

		private void menuItemCompareXml_Click(object sender, System.EventArgs e)
		{
			//buttonXmlDiff_Click (sender, e);
		
		}

		private void menuItemCopyDocument_Click(object sender, System.EventArgs e)
		{
			CommandCopyMoveDelete (KFileAction.Copy);
		}

		private void menuItemRenameDocument_Click(object sender, System.EventArgs e)
		{
			CommandCopyMoveDelete (KFileAction.Rename);
		}


		private void menuItemMoveDocument_Click(object sender, System.EventArgs e)
		{
			CommandCopyMoveDelete (KFileAction.Move);
		}

		private void menuItemDeleteDocument_Click(object sender, System.EventArgs e)
		{
			CommandCopyMoveDelete (KFileAction.Delete);
		}

		private void CommandCopyMoveDelete (KFileAction kfaction)
		{
			string [] rgfnDocuments = GetSelectedDocumentFileList ();
			bool fDeletedFiles = false;

			if (rgfnDocuments.Length == 0)
			{
				W11Messages.ShowWarning ("Please select some documents for this operation");
				return;
			}

         //commented out till I am certain what this does. Don't want to delete our test files.
			//DocumentFile.CopyMoveDelete (OurLocation.PathTestFolder, kfaction, rgfnDocuments, out fDeletedFiles);
         if (fDeletedFiles)
         {
            /* Have to initialize list if some files were deleted */
            EnumerateTests();
            _sfnSecondRun = ""; // Can we preserve second run?
            LoadResultsCore("", 0);
            InitializeDialog();
            InitializeQuery();
         }
		}


		// GetSelectedDocumentFileList - returns array of selected documents files
		private string [] GetSelectedDocumentFileList ()
		{
			ListView.SelectedListViewItemCollection SelectedItems;
			int nSelectedDocuments;
			string [] rgfnDocuments;
			int ifn;

			SelectedItems = _listView1.SelectedItems;

			/* Count number of selected documents */
			nSelectedDocuments = 0;
			foreach (ListViewItem itemCur in SelectedItems) 
			{
				if (_rgimapView2Doc [itemCur.Index] != -1) nSelectedDocuments++;
			}

			if (nSelectedDocuments == 0)
			{
				return new string [0];
			}
			else
				{
				rgfnDocuments = new string [nSelectedDocuments];

				/* Fill out array of file names */
				ifn = 0;
				foreach (ListViewItem itemCur in SelectedItems) 
				{
					int iTask = _rgimapView2Task [itemCur.Index];
					int idoc = _rgimapView2Doc [itemCur.Index];

					if (idoc != -1) 
					{
						Common.Assert (ifn < nSelectedDocuments);

						rgfnDocuments [ifn] = Path.Combine ( _rgFolderTasks [iTask].GetPathDoc (_ourLocation),
							_rgFolderTasks [iTask].rgDocuments [idoc].strDocname );												
						ifn = ifn + 1;					
					}
				}

				Common.Assert (ifn == nSelectedDocuments);
				return rgfnDocuments;
			}
		}

		//F L A G G I N G
		 // Flag Document
		void FlagDocument (int iTask, int idoc, bool fSelect)
		{
			CTFolderTask Task = this._rgFolderTasks [iTask];

			if (Task.rgDocuments [idoc].fSelected != fSelect)
			{
				_fRecalcStatistics = true;
				Task.rgDocuments [idoc].fSelected = fSelect;
				UpdateDocumentView (iTask, idoc);
			}
		}

		// FlagAllVisibleDocuments
		void FlagAllVisibleDocuments ()
		{
			int idoc;
			int iTask;
			CTFolderTask Task;

			for (iTask = 0; iTask < _nTasks; iTask++)
			{
				Task = _rgFolderTasks [iTask];

				for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
				{
					if (Task.rgDocuments [idoc].fVisible)
					{
						FlagDocument (iTask, idoc, true);
					}
				}
			}
		}


		// FlagAllDocuments 
		void FlagAllDocuments ()
		{
			int idoc;
			int iTask;
			CTFolderTask Task;

			for (iTask = 0; iTask < _nTasks; iTask++)
			{
				Task = _rgFolderTasks [iTask];

				for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
				{
					FlagDocument (iTask, idoc, true);
				}
			}
		}


		// ClearFlags
		void ClearFlags ()
		{
			int idoc;
			int iTask;
			CTFolderTask Task;

			for (iTask = 0; iTask < _nTasks; iTask++)
			{
				Task = _rgFolderTasks [iTask];

				for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
				{
					FlagDocument (iTask, idoc, false);
				}
			}
		}

		//D O C U M E N T  P R O P E R T I E S
		// FModifyDocumentProperties 
		bool FModifyDocumentProperties (int iTask, int idoc, string strComment, bool fEnable)
		{
			CTFolderTask Task = _rgFolderTasks [iTask];

			strComment = strComment.Trim ();

			if (strComment != Task.rgDocuments [idoc].strComment || 
				fEnable != Task.rgDocuments [idoc].fEnable )
			{
				Task.rgDocuments [idoc].fEnable = fEnable;
				Task.rgDocuments [idoc].strComment = strComment;
				Task.rgDocuments [idoc].fSaved = false;

				_fAllDocumentPropertiesSaved = false;

				return true;
			}
			else
			{
				/* No changes */
				return false;
			}
		}

		void SaveUnsavedDocumentProperties ()
		{
			int iTask, idoc;

			if (!_fAllDocumentPropertiesSaved)
			{
				for (iTask = 0; iTask < _nTasks; iTask++)
				{
					CTFolderTask Task = _rgFolderTasks [iTask];

					for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
					{
						if (!Task.rgDocuments [idoc].fSaved)
						{
							SDocument Document = Task.rgDocuments [idoc];
							StreamWriter stream;
							string sfnDin;
							string strCommentTr = Document.strComment.Trim ();

							sfnDin = Path.Combine (Task.GetPathDoc (_ourLocation), Path.GetFileNameWithoutExtension (Document.strDocname ) + ".din");

							if (Document.fEnable && strCommentTr == "")
							{
								/* Defaults => delete din file */
								if (File.Exists (sfnDin))
								{
									try 
									{
										File.Delete (sfnDin);
									} 
									catch {W11Messages.ShowError ("Unable to delete document ini file " + sfnDin);};
								}
							}
							else
							{
								/* Values are not default => save file */
								try 
								{
									stream = File.CreateText (sfnDin);

									stream.WriteLine ("Run = " + (Document.fEnable ? "On" : "Off"));
									stream.WriteLine ("Comment = " + Common.PackMultiLineString (strCommentTr));
									stream.Close ();
								}
								catch {W11Messages.ShowError ("Can not save document ini file " + sfnDin);};
							};

							Task.rgDocuments [idoc].fSaved = true;
						}
					};
				};

				_fAllDocumentPropertiesSaved = true;
			};
		}

		// ReadDocumentIniFiles
		// Process document ini files for the specified folder
		static void ReadDocumentIniFiles ( string spathDoc, 
											string [] rgstrDocnames,
										    out bool [] rgfEnableDocument, 
											out string [] rgstrDocComment )
		{
			int idoc;
         //int idin;
         //string [] rgstrDinnames = Common.ReadFileNames (spathDoc, "*.din");

         //Common.SortStringArray (rgstrDinnames);

			// Allocate output arrays
			rgfEnableDocument = new bool [rgstrDocnames.Length];
			rgstrDocComment = new string [rgstrDocnames.Length];

         //idin = 0; // current ini-document

			// Initialize init information for every document
         #region
			for (idoc = 0; idoc < rgstrDocnames.Length; idoc++)
			{
				// Set defaults first
				rgfEnableDocument [idoc] = true;
				rgstrDocComment [idoc] = "";

            //if (idin < rgstrDinnames.Length)
            //{
            //   string strDocName_noex = Path.GetFileNameWithoutExtension (rgstrDocnames [idoc]).ToUpper ();
            //   string strDinName_noex = Path.GetFileNameWithoutExtension (rgstrDinnames [idin]).ToUpper ();

            //   while (idin < rgstrDinnames.Length && Common.CompareStr (strDocName_noex, strDinName_noex) > 0)
            //   {
            //      /* Document has greater name => din name corresponds to non-existing document */
            //      /* Prompt if user wants to delete it */
            //      DeleteBogusDin (Path.Combine (spathDoc, rgstrDinnames [idin]));
            //      idin ++;
            //      if (idin < rgstrDinnames.Length) strDinName_noex = Path.GetFileNameWithoutExtension (rgstrDinnames [idin]).ToUpper ();
            //   }

            //   if (idin < rgstrDinnames.Length && strDocName_noex == strDinName_noex)
            //   {
            //      /* Found document for given din-file */
            //      CInifile inifile = new CInifile (Path.Combine (spathDoc, rgstrDinnames [idin]));

            //      string strFRun = inifile.GetNext ("Run").ToUpper ();
            //      string strComment = inifile.GetNext ("Comment");

            //      inifile.Close ();

            //      rgfEnableDocument [idoc] = strFRun == "ON";
            //      rgstrDocComment [idoc] = Common.UnpackMultiLineString (strComment);

            //      // Advance to next document ini
            //      idin++;
            //   };

            //}

			}
         #endregion
			// Delete remaining din files
         //while (idin < rgstrDinnames.Length)
         //{
         //   /* Document has greater name => din name corresponds to non-existing document */
         //   /* Prompt if user wants to delete it */
         //   DeleteBogusDin (Path.Combine (spathDoc, rgstrDinnames [idin]));
         //   idin ++;
         //}

		} // end of ReadDocumentIniFiles

		// DeleteBogusDin - deletes bugus document ini file
		static void DeleteBogusDin (string sfnDin)
		{
			if (Ask.YesNoDialog ( KQuestion.DeleteBogusDocumentIniFile, 
				"Found ini file for non-existing document: " + sfnDin  + "\n\nDo you want to delete it?", false))
			{
				try 
				{
					File.Delete (sfnDin);
				}
				catch {W11Messages.ShowWarning ("Unable to delete " + sfnDin); }
			};
		}

		// Removes temporary document names form the list
		static string [] RemoveTemporaryDocuments ( CProtocol Protocol,  string spath, 
			string [] rgstrDocnames)
		{
			int idoc;
			int nTemps = 0;

			for (idoc=0; idoc < rgstrDocnames.Length; idoc++)
			{
				if (Common.FTemporaryDocument (rgstrDocnames [idoc]))
				{
					/* Temporary document: ask about deletion & count */
					string sfnDoc = Path.Combine (spath, rgstrDocnames [idoc]);

					if (Ask.YesNoDialog ( KQuestion.DeleteTemporaryDoc, 
						"Found temporary document: " + sfnDoc + "\n\nDo you want to delete it?", true))
					{
						// Delete temporary document
						try 
						{
							// Clear read-only attribute
							File.SetAttributes (sfnDoc, (FileAttributes) (File.GetAttributes (sfnDoc) & ~FileAttributes.ReadOnly));

							File.Delete (sfnDoc);
							Protocol.Write ("Deleted " + sfnDoc);
						}
						catch { Protocol.WriteWarning ("Unable to delete " + sfnDoc); }
					}

					// Count
					nTemps++;
				}
			};

			if (nTemps > 0)
			{
				string [] rgstrDocnamesNew = new string [rgstrDocnames.Length - nTemps];
				int idocNew = 0;

				for (idoc=0; idoc < rgstrDocnames.Length; idoc++)
				{
					if (Common.FTemporaryDocument (rgstrDocnames [idoc]))
					{
						/* Temporary document, skip */
					}
					else
					{
						/* Normal document */
						rgstrDocnamesNew [idocNew] = rgstrDocnames [idoc];
						idocNew++;
					};
				};

				Common.Assert (idocNew == rgstrDocnamesNew.Length);

				return rgstrDocnamesNew;
			}
			else
			{
				/* No temps, return original array */
				return rgstrDocnames;
			};
		}

		//S E A R C H  &  F I L T E R
		// InitializeQuery
		void InitializeQuery ()
		{
			_strQuery = "";
			_strQuerySet = "";
		}

		// AddQueryToComboBoxHistory
		static void AddQueryToComboBoxHistory (System.Windows.Forms.ComboBox comboBox, string strquery)
		{
			int ii;

			strquery = strquery.Trim ();

			// Check if query string already there
			for (ii=0; ii < comboBox.Items.Count; ii++)
			{
				if (strquery == (string) comboBox.Items [ii])
				{
					return; // Found, no need to add
				}
			}

			// Insert in the beginning of the list
			comboBox.Items.Insert (0, strquery);
		}

		// buttonFindFirst_Click - User clicks on Find (First) button
		private void buttonFindFirst_Click(object sender, System.EventArgs e)
		{
			SaveUnsavedDocumentProperties ();
			SearchAndSelect  (0, true /* forward */);
		}

		// buttonPrev_Click - User clicks on Find Prev button
		private void buttonPrev_Click(object sender, System.EventArgs e)
		{
			/* Should be disable with multiple selection */
			SaveUnsavedDocumentProperties ();
			if (_iviewSelection != -1)
			{
				SearchAndSelect (_iviewSelection-1, false /* backwards */);
			};
		}

		// buttonNext_Click - User clicks on Find Next button
		private void buttonNext_Click(object sender, System.EventArgs e)
		{
			/* Should be disable with multiple selection */
			SaveUnsavedDocumentProperties ();
			if (_iviewSelection != -1)
			{
				SearchAndSelect (_iviewSelection + 1, true /* forward */);
			};
		}

		// buttonCountAll_Click - User clicks on Count All
		private void buttonCountAll_Click(object sender, System.EventArgs e)
		{
			CQuery query; 
			
			SaveUnsavedDocumentProperties ();

			if (FCreateSearchQuery (out query, _strQuery))
			{
				int iview = SearchCore (query, 0, true);
				int nCount = 0;
				int nCountSelected = 0;

				while (iview != -1)
				{
					nCount ++;

					if (_listView1.Items [iview].Selected) nCountSelected ++;

					iview = SearchCore (query, iview + 1, true);
				};

				AddQueryToComboBoxHistory (_comboBoxQuery, _strQuery);

				W11Messages.ShowMessage ("TOTAL: " + nCount.ToString () + "\n\nSELECTED: " + nCountSelected.ToString ());
			}
		}

		// User clicks on Load filter
		private void buttonLoadQuery_Click(object sender, System.EventArgs e)
		{
			LoadFilterCore ();
			UpdateDialog (false);
		}

		// User clicks on query builder
		private void QueryBuilderClick(object sender, System.EventArgs e)
		{
			string strQueryNew;

			if (QueryBuilder.FShowDialog (out strQueryNew))
			{
				this._strQuery = strQueryNew;
				LoadFilterCore ();
				UpdateDialog (false);
			}
		}

		// LoadFilterCore
		private void LoadFilterCore ()
		{
			int iTask;
			int idoc;
			CQuery query;

			SaveUnsavedDocumentProperties ();

			_strQuery = _strQuery.Trim ();

			if (_strQuery == "") ClearFilterCore ();
			else
			{
				if (FCreateSearchQuery (out query, _strQuery))
				{
					for (iTask = 0; iTask < _nTasks; iTask++)
					{
						CTFolderTask Task = _rgFolderTasks [iTask];
						for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
						{
							if (!FItemMatchesQuery (query, iTask, idoc, out Task.rgDocuments [idoc].fVisibleTemp))
							{
								/* Query error => return */
								return;
							}
						}
					};

					/* Copy temp visible to real visible and rebuild the list */
					for (iTask = 0; iTask < _nTasks; iTask++)
					{
						CTFolderTask Task = _rgFolderTasks [iTask];
						for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++) 
							Task.rgDocuments [idoc].fVisible = Task.rgDocuments [idoc].fVisibleTemp;
					};

					RebuildViewList ();
					_fRecalcStatistics = true;
					AddQueryToComboBoxHistory (_comboBoxQuery, _strQuery);
					_strQuerySet = _strQuery;
 				}
			}
		}

		// buttonUnfilter_Click - Undo filtering
		void ClearFilterCore ()
		{
			int iTask;
			int idoc;

			SaveUnsavedDocumentProperties ();

			for (iTask = 0; iTask < _nTasks; iTask++)
			{
				for (idoc = 0; idoc < _rgFolderTasks [iTask].rgDocuments.Length; idoc++)
				{
					_rgFolderTasks [iTask].rgDocuments [idoc].fVisible = true;
				}
			};

			RebuildViewList ();
			_strQuerySet = "";
			_fRecalcStatistics = true;
			InitializeQuery ();
		}

		// buttonUnfilter_Click - Undo filtering
		private void buttonUnfilter_Click(object sender, System.EventArgs e)
		{
			ClearFilterCore ();
			UpdateDialog (false);
		}

		private void textBoxSearch_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				/* Enter inside search textbox is equivalent to FindFirst */
				buttonFindFirst_Click (sender, e);
			};		
		}

		// SearchAndSelect: search from given location and selects result
		void SearchAndSelect (int iview, bool fForward)
		{
			CQuery query;
			int iviewFound;

			if (FCreateSearchQuery (out query, _strQuery))
			{
				iviewFound = SearchCore (query, iview, fForward);

				if (iviewFound != -1)
				{
					UpdateSelection (iviewFound);
					_listView1.EnsureVisible (iviewFound);
					_listView1.Focus ();
				}
				else
				{
					// 
				}
				AddQueryToComboBoxHistory (_comboBoxQuery, _strQuery);
			};
		}

		// CreateSearchQuery
		bool FCreateSearchQuery (out CQuery query, string str)
		{
			bool fSuccessful;
			string [] rgSearchColumns = new string [_nColumns + 1];
			int icol;

			query = new CQuery ();

			// All column names first
			for (icol = 0; icol < _nColumns; icol++)
			{
				rgSearchColumns [icol] = _rgstrColumnNames [icol];
			}

            // Extra fields			
			rgSearchColumns [_nColumns] = "folder";
			
			fSuccessful = query.CreateQuery (rgSearchColumns, str);

			if (!fSuccessful)
			{
				W11Messages.ShowError ("Error in search string: \n\n" + query.ErrorMessage);
				return false;
			}
			else
			{
				return true;
			}
		}


		// SearchCore: Implementation of the search
		int SearchCore ( CQuery cquery, int iview, bool fForward)
		{
			bool fSuccessful;
			bool fMatch;

			if (iview < 0 || iview >= _listView1.Items.Count)
			{
				Common.Assert (fForward || iview < _listView1.Items.Count);
				Common.Assert (!fForward || iview >= 0);

				return -1;
			};

			Common.Assert (iview < _listView1.Items.Count);
			Common.Assert (iview >= 0);

			if (fForward)
			{
				// Forward search
				while (iview < _listView1.Items.Count)
				{
					if (_rgimapView2Doc [iview] != -1)
					{
						fSuccessful = FItemMatchesQuery (cquery, _rgimapView2Task [iview], _rgimapView2Doc [iview], out fMatch);
						if (!fSuccessful) return -1; /* Error during query */
						else if (fMatch) return iview;
					}
					iview ++;
				}

				return -1; /* Not found */
			}
			else
			{
				// Backward search
				while ( iview >= 0)
				{
					if (_rgimapView2Doc [iview] != -1)
					{
						fSuccessful = FItemMatchesQuery (cquery, _rgimapView2Task [iview], _rgimapView2Doc [iview], out fMatch);
						if (!fSuccessful) return -1; /* Error during query */
						else if (fMatch) return iview;
					}
					iview --; 
				}

				return -1; /* Not found */
			}
		}


		// FQueryItem: service function to query view item
		bool FItemMatchesQuery ( CQuery query, int iTask, int idoc, out bool fResult )
		{
			System.Windows.Forms.ListViewItem item;
			int ic;
			bool fSuccessful;

			Common.Assert (idoc != -1);
			Common.Assert (_rgstrSearchRecordTemp.Length == _nColumns + 1);

			item = _rgFolderTasks [iTask].rgDocuments [idoc].Item;

			Common.Assert (item.SubItems.Count == _nColumns);

			// Convert list item into array
			// First, take all columns
			for (ic = 0; ic < _nColumns; ic++)
			{
				_rgstrSearchRecordTemp [ic] = item.SubItems [ic].Text;
			};

			// Extra fields
			_rgstrSearchRecordTemp [_nColumns] = _rgFolderTasks [iTask].GetPathDoc (_ourLocation);

			fSuccessful = query.ExecuteQuery (_rgstrSearchRecordTemp, out fResult);

			if (!fSuccessful)
			{
				W11Messages.ShowError ("Search error: \n\n" + query.ErrorMessage);
				return false;
			}
			else
			{
				return true;
			}
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
			this._columnFlagged = new System.Windows.Forms.ColumnHeader();
			this._columnRestart = new System.Windows.Forms.ColumnHeader();
			this._columnDoc = new System.Windows.Forms.ColumnHeader();
			this._columnRun = new System.Windows.Forms.ColumnHeader();
			this._columnRun2 = new System.Windows.Forms.ColumnHeader();
			this._columnC = new System.Windows.Forms.ColumnHeader();
			this._columnFlags = new System.Windows.Forms.ColumnHeader();
			this._columnError = new System.Windows.Forms.ColumnHeader();
			this._columnError2 = new System.Windows.Forms.ColumnHeader();
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
			this._label13 = new System.Windows.Forms.Label();
			this._buttonClearQuery = new System.Windows.Forms.Button();
			this._buttonCountAll = new System.Windows.Forms.Button();
			this._buttonFindNext = new System.Windows.Forms.Button();
			this._buttonFindPrev = new System.Windows.Forms.Button();
			this._buttonFindFirst = new System.Windows.Forms.Button();
			this._labelNErrors = new System.Windows.Forms.Label();
			this._labelNDocuments = new System.Windows.Forms.Label();
			this._mainMenu1 = new System.Windows.Forms.MainMenu();
			this._menuItem5 = new System.Windows.Forms.MenuItem();
			this._menuItemFileLoad = new System.Windows.Forms.MenuItem();
			this._menuItemFileSave = new System.Windows.Forms.MenuItem();
			this._menuItem1 = new System.Windows.Forms.MenuItem();
			this._menuItemExportExcelAll = new System.Windows.Forms.MenuItem();
			this._menuItemExportExcelSelection = new System.Windows.Forms.MenuItem();
			this._menuItem9 = new System.Windows.Forms.MenuItem();
			this._menuItemFileOpenSecond = new System.Windows.Forms.MenuItem();
			this._menuItemFileClearSecond = new System.Windows.Forms.MenuItem();
			this._menuItem10 = new System.Windows.Forms.MenuItem();
			this._menuItemFileUpdateList = new System.Windows.Forms.MenuItem();
			this._menuItemFileClose = new System.Windows.Forms.MenuItem();
			this._menuItem13 = new System.Windows.Forms.MenuItem();
			this._menuItemOpenWord = new System.Windows.Forms.MenuItem();
			this._menuItemCompareXml = new System.Windows.Forms.MenuItem();
			this._menuItem19 = new System.Windows.Forms.MenuItem();
			this._menuItemCopyDocument = new System.Windows.Forms.MenuItem();
			this._menuItemRenameDocument = new System.Windows.Forms.MenuItem();
			this._menuItemMoveDocument = new System.Windows.Forms.MenuItem();
			this._menuItemDeleteDocument = new System.Windows.Forms.MenuItem();
			this._menuItem6 = new System.Windows.Forms.MenuItem();
			this._menuItemEditSelectAll = new System.Windows.Forms.MenuItem();
			this._menuItem4 = new System.Windows.Forms.MenuItem();
			this._menuItemEditFlagAllCurrent = new System.Windows.Forms.MenuItem();
			this._menuItemClearFlags = new System.Windows.Forms.MenuItem();
			this._menuItem7 = new System.Windows.Forms.MenuItem();
			this._menuItemRunSelection = new System.Windows.Forms.MenuItem();
			this._menuItem3 = new System.Windows.Forms.MenuItem();
			this._menuItemTestClear = new System.Windows.Forms.MenuItem();
			this._menuItem8 = new System.Windows.Forms.MenuItem();
			this._labelNRun = new System.Windows.Forms.Label();
			this._groupBoxTestStat = new System.Windows.Forms.GroupBox();
			this._labelStatSelErrors = new System.Windows.Forms.Label();
			this._labelStatSelRun = new System.Windows.Forms.Label();
			this._labelStatSelDocs = new System.Windows.Forms.Label();
			this._label6 = new System.Windows.Forms.Label();
			this._labelStatCurErrors = new System.Windows.Forms.Label();
			this._labelStatAllErrors = new System.Windows.Forms.Label();
			this._labelStatCurRun = new System.Windows.Forms.Label();
			this._labelStatAllRun = new System.Windows.Forms.Label();
			this._labelStatCurDocs = new System.Windows.Forms.Label();
			this._labelStatAllDocs = new System.Windows.Forms.Label();
			this._labelCurrent = new System.Windows.Forms.Label();
			this._label5 = new System.Windows.Forms.Label();
			this._groupBoxUpdatingBackground = new System.Windows.Forms.GroupBox();
			this._label4 = new System.Windows.Forms.Label();
			this._contextMenu1 = new System.Windows.Forms.ContextMenu();
			this._menuItem15 = new System.Windows.Forms.MenuItem();
			this._menuItem16 = new System.Windows.Forms.MenuItem();
			this._menuItem17 = new System.Windows.Forms.MenuItem();
			this._groupBoxTop = new System.Windows.Forms.GroupBox();
			this._comboBoxQuery = new System.Windows.Forms.ComboBox();
			this._labelCurrentQuery = new System.Windows.Forms.Label();
			this._label7 = new System.Windows.Forms.Label();
			this._buttonLoadQuery = new System.Windows.Forms.Button();
			this._labelExistingWordWarning = new System.Windows.Forms.Label();
			this._groupBoxDocument.SuspendLayout();
			this._groupBoxTestStat.SuspendLayout();
			this._groupBoxUpdatingBackground.SuspendLayout();
			this._groupBoxTop.SuspendLayout();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this._listView1.AllowColumnReorder = true;
			this._listView1.BackColor = System.Drawing.SystemColors.Window;
			this._listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this._columnN,
																						this._columnFlagged,
																						this._columnRestart,
																						this._columnDoc,
																						this._columnRun,
																						this._columnRun2,
																						this._columnC,
																						this._columnFlags,
																						this._columnError,
																						this._columnError2});
			this._listView1.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._listView1.FullRowSelect = true;
			this._listView1.GridLines = true;
			this._listView1.HideSelection = false;
			this._listView1.Location = new System.Drawing.Point(2, 64);
			this._listView1.Name = "listView1";
			this._listView1.Size = new System.Drawing.Size(854, 432);
			this._listView1.TabIndex = 0;
			this._listView1.View = System.Windows.Forms.View.Details;
			this._listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
			this._listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
			this._listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// ColumnN
			// 
			this._columnN.Text = "N";
			this._columnN.Width = 39;
			// 
			// columnFlagged
			// 
			this._columnFlagged.Text = "x";
			this._columnFlagged.Width = 22;
			// 
			// columnRestart
			// 
			this._columnRestart.Text = "r";
			this._columnRestart.Width = 20;
			// 
			// ColumnDoc
			// 
			this._columnDoc.Text = "Document";
			this._columnDoc.Width = 233;
			// 
			// ColumnRun
			// 
			this._columnRun.Text = "Run";
			this._columnRun.Width = 45;
			// 
			// ColumnRun2
			// 
			this._columnRun2.Text = "Run2";
			this._columnRun2.Width = 44;
			// 
			// ColumnC
			// 
			this._columnC.Text = "C";
			this._columnC.Width = 97;
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
			// ColumnError2
			// 
			this._columnError2.Text = "Error2";
			this._columnError2.Width = 182;
			// 
			// buttonWord
			// 
			this._buttonWord.BackColor = System.Drawing.SystemColors.Control;
			this._buttonWord.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonWord.Location = new System.Drawing.Point(12, 16);
			this._buttonWord.Name = "buttonWord";
			this._buttonWord.Size = new System.Drawing.Size(42, 25);
			this._buttonWord.TabIndex = 3;
			this._buttonWord.Text = "Word";
			this._buttonWord.Click += new System.EventHandler(this.buttonWord_Click);
			// 
			// labelFile
			// 
			this._labelFile.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelFile.ForeColor = System.Drawing.SystemColors.GrayText;
			this._labelFile.Location = new System.Drawing.Point(52, 613);
			this._labelFile.Name = "labelFile";
			this._labelFile.Size = new System.Drawing.Size(404, 20);
			this._labelFile.TabIndex = 4;
			this._labelFile.Text = "FILE:";
			// 
			// buttonXmlDiff
			// 
			this._buttonXmlDiff.BackColor = System.Drawing.SystemColors.Control;
			this._buttonXmlDiff.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonXmlDiff.Location = new System.Drawing.Point(61, 16);
			this._buttonXmlDiff.Name = "buttonXmlDiff";
			this._buttonXmlDiff.Size = new System.Drawing.Size(42, 25);
			this._buttonXmlDiff.TabIndex = 5;
			this._buttonXmlDiff.Text = "Diff";
			//this.buttonXmlDiff.Click += new System.EventHandler(this.buttonXmlDiff_Click);
			// 
			// buttonStop
			// 
			this._buttonStop.BackColor = System.Drawing.SystemColors.Control;
			this._buttonStop.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonStop.Location = new System.Drawing.Point(109, 16);
			this._buttonStop.Name = "buttonStop";
			this._buttonStop.Size = new System.Drawing.Size(42, 25);
			this._buttonStop.TabIndex = 33;
			this._buttonStop.Text = "Stop";
			this._buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// labelLanguage
			// 
			this._labelLanguage.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelLanguage.Location = new System.Drawing.Point(402, 25);
			this._labelLanguage.Name = "labelLanguage";
			this._labelLanguage.Size = new System.Drawing.Size(74, 24);
			this._labelLanguage.TabIndex = 35;
			this._labelLanguage.Text = "English";
			// 
			// labelPage
			// 
			this._labelPage.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelPage.Location = new System.Drawing.Point(370, 49);
			this._labelPage.Name = "labelPage";
			this._labelPage.Size = new System.Drawing.Size(110, 24);
			this._labelPage.TabIndex = 36;
			this._labelPage.Text = "Word";
			// 
			// label1
			// 
			this._label1.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label1.Location = new System.Drawing.Point(330, 49);
			this._label1.Name = "label1";
			this._label1.Size = new System.Drawing.Size(48, 24);
			this._label1.TabIndex = 40;
			this._label1.Text = "Page:";
			// 
			// label2
			// 
			this._label2.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label2.Location = new System.Drawing.Point(330, 25);
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
			this._groupBoxDocument.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._groupBoxDocument.Location = new System.Drawing.Point(2, 504);
			this._groupBoxDocument.Name = "groupBoxDocument";
			this._groupBoxDocument.Size = new System.Drawing.Size(486, 106);
			this._groupBoxDocument.TabIndex = 41;
			this._groupBoxDocument.TabStop = false;
			this._groupBoxDocument.Text = "Document properties";
			// 
			// textBoxComment
			// 
			this._textBoxComment.BackColor = System.Drawing.SystemColors.Info;
			this._textBoxComment.Location = new System.Drawing.Point(8, 43);
			this._textBoxComment.Name = "textBoxComment";
			this._textBoxComment.Size = new System.Drawing.Size(312, 53);
			this._textBoxComment.TabIndex = 48;
			this._textBoxComment.Text = "richTextBox1";
			this._textBoxComment.TextChanged += new System.EventHandler(this.EditingChange);
			// 
			// checkBoxDocumentDisabled
			// 
			this._checkBoxDocumentDisabled.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._checkBoxDocumentDisabled.Location = new System.Drawing.Point(328, 75);
			this._checkBoxDocumentDisabled.Name = "checkBoxDocumentDisabled";
			this._checkBoxDocumentDisabled.Size = new System.Drawing.Size(96, 24);
			this._checkBoxDocumentDisabled.TabIndex = 0;
			this._checkBoxDocumentDisabled.Text = "Do not run";
			this._checkBoxDocumentDisabled.CheckedChanged += new System.EventHandler(this.EditingChange);
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
			// label13
			// 
			this._label13.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label13.ForeColor = System.Drawing.SystemColors.GrayText;
			this._label13.Location = new System.Drawing.Point(8, 613);
			this._label13.Name = "label13";
			this._label13.Size = new System.Drawing.Size(46, 24);
			this._label13.TabIndex = 46;
			this._label13.Text = "FILE = ";
			// 
			// buttonClearQuery
			// 
			this._buttonClearQuery.BackColor = System.Drawing.SystemColors.Control;
			this._buttonClearQuery.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonClearQuery.Location = new System.Drawing.Point(592, 16);
			this._buttonClearQuery.Name = "buttonClearQuery";
			this._buttonClearQuery.Size = new System.Drawing.Size(42, 25);
			this._buttonClearQuery.TabIndex = 56;
			this._buttonClearQuery.Text = "Clear";
			this._buttonClearQuery.Click += new System.EventHandler(this.buttonUnfilter_Click);
			// 
			// buttonCountAll
			// 
			this._buttonCountAll.BackColor = System.Drawing.SystemColors.Control;
			this._buttonCountAll.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonCountAll.Location = new System.Drawing.Point(789, 16);
			this._buttonCountAll.Name = "buttonCountAll";
			this._buttonCountAll.Size = new System.Drawing.Size(42, 25);
			this._buttonCountAll.TabIndex = 54;
			this._buttonCountAll.Text = "Cnt";
			this._buttonCountAll.Click += new System.EventHandler(this.buttonCountAll_Click);
			// 
			// buttonFindNext
			// 
			this._buttonFindNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonFindNext.Location = new System.Drawing.Point(691, 16);
			this._buttonFindNext.Name = "buttonFindNext";
			this._buttonFindNext.Size = new System.Drawing.Size(42, 25);
			this._buttonFindNext.TabIndex = 50;
			this._buttonFindNext.Text = "=>";
			this._buttonFindNext.Click += new System.EventHandler(this.buttonNext_Click);
			// 
			// buttonFindPrev
			// 
			this._buttonFindPrev.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonFindPrev.Location = new System.Drawing.Point(739, 16);
			this._buttonFindPrev.Name = "buttonFindPrev";
			this._buttonFindPrev.Size = new System.Drawing.Size(42, 25);
			this._buttonFindPrev.TabIndex = 49;
			this._buttonFindPrev.Text = "<=";
			this._buttonFindPrev.Click += new System.EventHandler(this.buttonPrev_Click);
			// 
			// buttonFindFirst
			// 
			this._buttonFindFirst.BackColor = System.Drawing.SystemColors.Control;
			this._buttonFindFirst.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonFindFirst.Location = new System.Drawing.Point(640, 16);
			this._buttonFindFirst.Name = "buttonFindFirst";
			this._buttonFindFirst.Size = new System.Drawing.Size(42, 25);
			this._buttonFindFirst.TabIndex = 49;
			this._buttonFindFirst.Text = "Find";
			this._buttonFindFirst.Click += new System.EventHandler(this.buttonFindFirst_Click);
			// 
			// labelNErrors
			// 
			this._labelNErrors.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelNErrors.Location = new System.Drawing.Point(16, 77);
			this._labelNErrors.Name = "labelNErrors";
			this._labelNErrors.Size = new System.Drawing.Size(48, 17);
			this._labelNErrors.TabIndex = 7;
			this._labelNErrors.Text = "Errors:";
			// 
			// labelNDocuments
			// 
			this._labelNDocuments.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelNDocuments.Location = new System.Drawing.Point(16, 41);
			this._labelNDocuments.Name = "labelNDocuments";
			this._labelNDocuments.Size = new System.Drawing.Size(80, 24);
			this._labelNDocuments.TabIndex = 1;
			this._labelNDocuments.Text = "Documents:";
			// 
			// mainMenu1
			// 
			this._mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItem5,
																					  this._menuItem13,
																					  this._menuItem6,
																					  this._menuItem7,
																					  this._menuItem8});
			// 
			// menuItem5
			// 
			this._menuItem5.Index = 0;
			this._menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItemFileLoad,
																					  this._menuItemFileSave,
																					  this._menuItem1,
																					  this._menuItem9,
																					  this._menuItemFileOpenSecond,
																					  this._menuItemFileClearSecond,
																					  this._menuItem10,
																					  this._menuItemFileUpdateList,
																					  this._menuItemFileClose});
			this._menuItem5.Text = "&Results";
			// 
			// menuItemFileLoad
			// 
			this._menuItemFileLoad.Index = 0;
			this._menuItemFileLoad.Text = "&Load run";
			this._menuItemFileLoad.Click += new System.EventHandler(this.menuItemFileLoad_Click);
			// 
			// menuItemFileSave
			// 
			this._menuItemFileSave.Index = 1;
			this._menuItemFileSave.Text = "&Save run As ...";
			this._menuItemFileSave.Click += new System.EventHandler(this.menuItemFileSave_Click);
			// 
			// menuItem1
			// 
			this._menuItem1.Index = 2;
			this._menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItemExportExcelAll,
																					  this._menuItemExportExcelSelection});
			this._menuItem1.Text = "&Export run for Excel";
			// 
			// menuItemExportExcelAll
			// 
			this._menuItemExportExcelAll.Index = 0;
			this._menuItemExportExcelAll.Text = "&All";
			this._menuItemExportExcelAll.Click += new System.EventHandler(this.menuItemExportExcelAll_Click);
			// 
			// menuItemExportExcelSelection
			// 
			this._menuItemExportExcelSelection.Index = 1;
			this._menuItemExportExcelSelection.Text = "&Selection";
			this._menuItemExportExcelSelection.Click += new System.EventHandler(this.menuItemExportExcelSelection_Click);
			// 
			// menuItem9
			// 
			this._menuItem9.Index = 3;
			this._menuItem9.Text = "-";
			// 
			// menuItemFileOpenSecond
			// 
			this._menuItemFileOpenSecond.Index = 4;
			this._menuItemFileOpenSecond.Text = "Load second run";
			this._menuItemFileOpenSecond.Click += new System.EventHandler(this.menuItemFileOpenSecond_Click);
			// 
			// menuItemFileClearSecond
			// 
			this._menuItemFileClearSecond.Index = 5;
			this._menuItemFileClearSecond.Text = "Clear second run";
			this._menuItemFileClearSecond.Click += new System.EventHandler(this.menuItemFileClearSecond_Click);
			// 
			// menuItem10
			// 
			this._menuItem10.Index = 6;
			this._menuItem10.Text = "-";
			// 
			// menuItemFileUpdateList
			// 
			this._menuItemFileUpdateList.Index = 7;
			this._menuItemFileUpdateList.Text = "&Reload ( temporary run )";
			this._menuItemFileUpdateList.Click += new System.EventHandler(this.menuItemFileUpdateList_Click);
			// 
			// menuItemFileClose
			// 
			this._menuItemFileClose.Index = 8;
			this._menuItemFileClose.Text = "&Close";
			this._menuItemFileClose.Click += new System.EventHandler(this.menuItemFileClose_Click);
			// 
			// menuItem13
			// 
			this._menuItem13.Index = 1;
			this._menuItem13.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this._menuItemOpenWord,
																					   this._menuItemCompareXml,
																					   this._menuItem19,
																					   this._menuItemCopyDocument,
																					   this._menuItemRenameDocument,
																					   this._menuItemMoveDocument,
																					   this._menuItemDeleteDocument});
			this._menuItem13.Text = "&Document";
			// 
			// menuItemOpenWord
			// 
			this._menuItemOpenWord.Index = 0;
			this._menuItemOpenWord.Text = "Open in &Word - F4";
			this._menuItemOpenWord.Click += new System.EventHandler(this.menuItemOpenWord_Click);
			// 
			// menuItemCompareXml
			// 
			this._menuItemCompareXml.Index = 1;
			this._menuItemCompareXml.Text = "Compare &Lauout XML - F1";
			this._menuItemCompareXml.Click += new System.EventHandler(this.menuItemCompareXml_Click);
			// 
			// menuItem19
			// 
			this._menuItem19.Index = 2;
			this._menuItem19.Text = "-";
			// 
			// menuItemCopyDocument
			// 
			this._menuItemCopyDocument.Index = 3;
			this._menuItemCopyDocument.Text = "&Copy";
			this._menuItemCopyDocument.Click += new System.EventHandler(this.menuItemCopyDocument_Click);
			// 
			// menuItemRenameDocument
			// 
			this._menuItemRenameDocument.Index = 4;
			this._menuItemRenameDocument.Text = "&Rename";
			this._menuItemRenameDocument.Click += new System.EventHandler(this.menuItemRenameDocument_Click);
			// 
			// menuItemMoveDocument
			// 
			this._menuItemMoveDocument.Index = 5;
			this._menuItemMoveDocument.Text = "&Move";
			this._menuItemMoveDocument.Click += new System.EventHandler(this.menuItemMoveDocument_Click);
			// 
			// menuItemDeleteDocument
			// 
			this._menuItemDeleteDocument.Index = 6;
			this._menuItemDeleteDocument.Text = "&Delete";
			this._menuItemDeleteDocument.Click += new System.EventHandler(this.menuItemDeleteDocument_Click);
			// 
			// menuItem6
			// 
			this._menuItem6.Index = 2;
			this._menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItemEditSelectAll,
																					  this._menuItem4,
																					  this._menuItemEditFlagAllCurrent,
																					  this._menuItemClearFlags});
			this._menuItem6.Text = "&Edit";
			// 
			// menuItemEditSelectAll
			// 
			this._menuItemEditSelectAll.Index = 0;
			this._menuItemEditSelectAll.Text = "&Select All";
			this._menuItemEditSelectAll.Click += new System.EventHandler(this.menuItemEditSelectAll_Click);
			// 
			// menuItem4
			// 
			this._menuItem4.Index = 1;
			this._menuItem4.Text = "-";
			// 
			// menuItemEditFlagAllCurrent
			// 
			this._menuItemEditFlagAllCurrent.Index = 2;
			this._menuItemEditFlagAllCurrent.Text = "Flag All";
			this._menuItemEditFlagAllCurrent.Click += new System.EventHandler(this.menuItemEditFlagAllCurrent_Click);
			// 
			// menuItemClearFlags
			// 
			this._menuItemClearFlags.Index = 3;
			this._menuItemClearFlags.Text = "Clear Flags";
			this._menuItemClearFlags.Click += new System.EventHandler(this.menuItemClearFlags_Click);
			// 
			// menuItem7
			// 
			this._menuItem7.Index = 3;
			this._menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._menuItemRunSelection,
																					  this._menuItem3,
																					  this._menuItemTestClear});
			this._menuItem7.Text = "&Test";
			// 
			// menuItemRunSelection
			// 
			this._menuItemRunSelection.Index = 0;
			this._menuItemRunSelection.Text = "Run Selection";
			this._menuItemRunSelection.Click += new System.EventHandler(this.menuItemRunSelection_Click);
			// 
			// menuItem3
			// 
			this._menuItem3.Index = 1;
			this._menuItem3.Text = "-";
			// 
			// menuItemTestClear
			// 
			this._menuItemTestClear.Index = 2;
			this._menuItemTestClear.Text = "&Clear Results";
			this._menuItemTestClear.Click += new System.EventHandler(this.menuItemTestClear_Click);
			// 
			// menuItem8
			// 
			this._menuItem8.Index = 4;
			this._menuItem8.Text = "&Help";
			// 
			// labelNRun
			// 
			this._labelNRun.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelNRun.Location = new System.Drawing.Point(16, 59);
			this._labelNRun.Name = "labelNRun";
			this._labelNRun.Size = new System.Drawing.Size(56, 19);
			this._labelNRun.TabIndex = 51;
			this._labelNRun.Text = "Run:";
			// 
			// groupBoxTestStat
			// 
			this._groupBoxTestStat.Controls.Add(this._labelStatSelErrors);
			this._groupBoxTestStat.Controls.Add(this._labelStatSelRun);
			this._groupBoxTestStat.Controls.Add(this._labelStatSelDocs);
			this._groupBoxTestStat.Controls.Add(this._label6);
			this._groupBoxTestStat.Controls.Add(this._labelStatCurErrors);
			this._groupBoxTestStat.Controls.Add(this._labelStatAllErrors);
			this._groupBoxTestStat.Controls.Add(this._labelStatCurRun);
			this._groupBoxTestStat.Controls.Add(this._labelStatAllRun);
			this._groupBoxTestStat.Controls.Add(this._labelStatCurDocs);
			this._groupBoxTestStat.Controls.Add(this._labelStatAllDocs);
			this._groupBoxTestStat.Controls.Add(this._labelCurrent);
			this._groupBoxTestStat.Controls.Add(this._label5);
			this._groupBoxTestStat.Controls.Add(this._labelNRun);
			this._groupBoxTestStat.Controls.Add(this._labelNErrors);
			this._groupBoxTestStat.Controls.Add(this._labelNDocuments);
			this._groupBoxTestStat.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._groupBoxTestStat.Location = new System.Drawing.Point(512, 504);
			this._groupBoxTestStat.Name = "groupBoxTestStat";
			this._groupBoxTestStat.Size = new System.Drawing.Size(296, 106);
			this._groupBoxTestStat.TabIndex = 50;
			this._groupBoxTestStat.TabStop = false;
			this._groupBoxTestStat.Text = "Counters";
			// 
			// labelStatSelErrors
			// 
			this._labelStatSelErrors.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelStatSelErrors.ForeColor = System.Drawing.Color.Red;
			this._labelStatSelErrors.Location = new System.Drawing.Point(212, 77);
			this._labelStatSelErrors.Name = "labelStatSelErrors";
			this._labelStatSelErrors.Size = new System.Drawing.Size(45, 19);
			this._labelStatSelErrors.TabIndex = 66;
			this._labelStatSelErrors.Text = "13900";
			this._labelStatSelErrors.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelStatSelRun
			// 
			this._labelStatSelRun.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelStatSelRun.Location = new System.Drawing.Point(212, 59);
			this._labelStatSelRun.Name = "labelStatSelRun";
			this._labelStatSelRun.Size = new System.Drawing.Size(45, 24);
			this._labelStatSelRun.TabIndex = 65;
			this._labelStatSelRun.Text = "13900";
			this._labelStatSelRun.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelStatSelDocs
			// 
			this._labelStatSelDocs.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelStatSelDocs.Location = new System.Drawing.Point(212, 41);
			this._labelStatSelDocs.Name = "labelStatSelDocs";
			this._labelStatSelDocs.Size = new System.Drawing.Size(45, 24);
			this._labelStatSelDocs.TabIndex = 64;
			this._labelStatSelDocs.Text = "13900";
			this._labelStatSelDocs.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this._label6.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label6.Location = new System.Drawing.Point(216, 19);
			this._label6.Name = "label6";
			this._label6.Size = new System.Drawing.Size(64, 24);
			this._label6.TabIndex = 63;
			this._label6.Text = "Flagged";
			// 
			// labelStatCurErrors
			// 
			this._labelStatCurErrors.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelStatCurErrors.ForeColor = System.Drawing.Color.Red;
			this._labelStatCurErrors.Location = new System.Drawing.Point(155, 77);
			this._labelStatCurErrors.Name = "labelStatCurErrors";
			this._labelStatCurErrors.Size = new System.Drawing.Size(45, 19);
			this._labelStatCurErrors.TabIndex = 62;
			this._labelStatCurErrors.Text = "13900";
			this._labelStatCurErrors.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelStatAllErrors
			// 
			this._labelStatAllErrors.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelStatAllErrors.ForeColor = System.Drawing.Color.Red;
			this._labelStatAllErrors.Location = new System.Drawing.Point(92, 77);
			this._labelStatAllErrors.Name = "labelStatAllErrors";
			this._labelStatAllErrors.Size = new System.Drawing.Size(45, 19);
			this._labelStatAllErrors.TabIndex = 61;
			this._labelStatAllErrors.Text = "13900";
			this._labelStatAllErrors.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelStatCurRun
			// 
			this._labelStatCurRun.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelStatCurRun.Location = new System.Drawing.Point(155, 59);
			this._labelStatCurRun.Name = "labelStatCurRun";
			this._labelStatCurRun.Size = new System.Drawing.Size(45, 24);
			this._labelStatCurRun.TabIndex = 59;
			this._labelStatCurRun.Text = "13900";
			this._labelStatCurRun.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelStatAllRun
			// 
			this._labelStatAllRun.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelStatAllRun.Location = new System.Drawing.Point(92, 59);
			this._labelStatAllRun.Name = "labelStatAllRun";
			this._labelStatAllRun.Size = new System.Drawing.Size(45, 24);
			this._labelStatAllRun.TabIndex = 58;
			this._labelStatAllRun.Text = "13900";
			this._labelStatAllRun.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelStatCurDocs
			// 
			this._labelStatCurDocs.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelStatCurDocs.Location = new System.Drawing.Point(155, 41);
			this._labelStatCurDocs.Name = "labelStatCurDocs";
			this._labelStatCurDocs.Size = new System.Drawing.Size(45, 24);
			this._labelStatCurDocs.TabIndex = 56;
			this._labelStatCurDocs.Text = "13900";
			this._labelStatCurDocs.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelStatAllDocs
			// 
			this._labelStatAllDocs.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelStatAllDocs.Location = new System.Drawing.Point(92, 41);
			this._labelStatAllDocs.Name = "labelStatAllDocs";
			this._labelStatAllDocs.Size = new System.Drawing.Size(45, 24);
			this._labelStatAllDocs.TabIndex = 55;
			this._labelStatAllDocs.Text = "13900";
			this._labelStatAllDocs.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelCurrent
			// 
			this._labelCurrent.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelCurrent.Location = new System.Drawing.Point(159, 19);
			this._labelCurrent.Name = "labelCurrent";
			this._labelCurrent.Size = new System.Drawing.Size(48, 24);
			this._labelCurrent.TabIndex = 53;
			this._labelCurrent.Text = "Current";
			// 
			// label5
			// 
			this._label5.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label5.Location = new System.Drawing.Point(110, 19);
			this._label5.Name = "label5";
			this._label5.Size = new System.Drawing.Size(32, 24);
			this._label5.TabIndex = 52;
			this._label5.Text = "Total";
			// 
			// groupBoxUpdatingBackground
			// 
			this._groupBoxUpdatingBackground.BackColor = System.Drawing.Color.Silver;
			this._groupBoxUpdatingBackground.Controls.Add(this._label4);
			this._groupBoxUpdatingBackground.Location = new System.Drawing.Point(2, 64);
			this._groupBoxUpdatingBackground.Name = "groupBoxUpdatingBackground";
			this._groupBoxUpdatingBackground.Size = new System.Drawing.Size(848, 432);
			this._groupBoxUpdatingBackground.TabIndex = 51;
			this._groupBoxUpdatingBackground.TabStop = false;
			// 
			// label4
			// 
			this._label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 66.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label4.ForeColor = System.Drawing.SystemColors.Highlight;
			this._label4.Location = new System.Drawing.Point(32, 80);
			this._label4.Name = "label4";
			this._label4.Size = new System.Drawing.Size(712, 120);
			this._label4.TabIndex = 0;
			this._label4.Text = "Updating view...";
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
			// groupBoxTop
			// 
			this._groupBoxTop.Controls.Add(this._comboBoxQuery);
			this._groupBoxTop.Controls.Add(this._labelCurrentQuery);
			this._groupBoxTop.Controls.Add(this._label7);
			this._groupBoxTop.Controls.Add(this._buttonClearQuery);
			this._groupBoxTop.Controls.Add(this._buttonLoadQuery);
			this._groupBoxTop.Controls.Add(this._buttonFindFirst);
			this._groupBoxTop.Controls.Add(this._buttonFindPrev);
			this._groupBoxTop.Controls.Add(this._buttonFindNext);
			this._groupBoxTop.Controls.Add(this._buttonCountAll);
			this._groupBoxTop.Controls.Add(this._buttonWord);
			this._groupBoxTop.Controls.Add(this._buttonXmlDiff);
			this._groupBoxTop.Controls.Add(this._buttonStop);
			this._groupBoxTop.Controls.Add(this._labelExistingWordWarning);
			this._groupBoxTop.Location = new System.Drawing.Point(3, -3);
			this._groupBoxTop.Name = "groupBoxTop";
			this._groupBoxTop.Size = new System.Drawing.Size(853, 75);
			this._groupBoxTop.TabIndex = 52;
			this._groupBoxTop.TabStop = false;
			// 
			// comboBoxQuery
			// 
			this._comboBoxQuery.BackColor = System.Drawing.SystemColors.Info;
			this._comboBoxQuery.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._comboBoxQuery.Location = new System.Drawing.Point(208, 18);
			this._comboBoxQuery.Name = "comboBoxQuery";
			this._comboBoxQuery.Size = new System.Drawing.Size(328, 22);
			this._comboBoxQuery.TabIndex = 65;
			this._comboBoxQuery.Text = "query";
			this._comboBoxQuery.TextChanged += new System.EventHandler(this.EditingChange);
			// 
			// labelCurrentQuery
			// 
			this._labelCurrentQuery.Cursor = System.Windows.Forms.Cursors.Default;
			this._labelCurrentQuery.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelCurrentQuery.ForeColor = System.Drawing.SystemColors.GrayText;
			this._labelCurrentQuery.Location = new System.Drawing.Point(211, 46);
			this._labelCurrentQuery.Name = "labelCurrentQuery";
			this._labelCurrentQuery.Size = new System.Drawing.Size(608, 13);
			this._labelCurrentQuery.TabIndex = 64;
			this._labelCurrentQuery.Text = "No query currently set";
			this._labelCurrentQuery.UseMnemonic = false;
			// 
			// label7
			// 
			this._label7.Cursor = System.Windows.Forms.Cursors.Hand;
			this._label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._label7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this._label7.Location = new System.Drawing.Point(167, 21);
			this._label7.Name = "label7";
			this._label7.Size = new System.Drawing.Size(48, 16);
			this._label7.TabIndex = 62;
			this._label7.Text = "Query:";
			this._label7.Click += new System.EventHandler(this.QueryBuilderClick);
			// 
			// buttonLoadQuery
			// 
			this._buttonLoadQuery.BackColor = System.Drawing.SystemColors.Control;
			this._buttonLoadQuery.Font = new System.Drawing.Font("Verdana Ref", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buttonLoadQuery.Location = new System.Drawing.Point(544, 16);
			this._buttonLoadQuery.Name = "buttonLoadQuery";
			this._buttonLoadQuery.Size = new System.Drawing.Size(42, 25);
			this._buttonLoadQuery.TabIndex = 63;
			this._buttonLoadQuery.Text = "Set";
			this._buttonLoadQuery.Click += new System.EventHandler(this.buttonLoadQuery_Click);
			// 
			// labelExistingWordWarning
			// 
			this._labelExistingWordWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelExistingWordWarning.ForeColor = System.Drawing.Color.Red;
			this._labelExistingWordWarning.Location = new System.Drawing.Point(13, 45);
			this._labelExistingWordWarning.Name = "labelExistingWordWarning";
			this._labelExistingWordWarning.Size = new System.Drawing.Size(171, 24);
			this._labelExistingWordWarning.TabIndex = 60;
			this._labelExistingWordWarning.Text = "Using existing Word process";
			// 
			// CTestList
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(856, 633);
			this.Controls.Add(this._groupBoxTestStat);
			this.Controls.Add(this._groupBoxDocument);
			this.Controls.Add(this._label13);
			this.Controls.Add(this._labelFile);
			this.Controls.Add(this._listView1);
			this.Controls.Add(this._groupBoxUpdatingBackground);
			this.Controls.Add(this._groupBoxTop);
			this.Menu = this._mainMenu1;
			this.Name = "CTestList";
			this.Text = "Test List";
			this.Resize += new System.EventHandler(this.CTestList_Resize);
			this.VisibleChanged += new System.EventHandler(this.VisibleChangedNotification);
			this._groupBoxDocument.ResumeLayout(false);
			this._groupBoxTestStat.ResumeLayout(false);
			this._groupBoxUpdatingBackground.ResumeLayout(false);
			this._groupBoxTop.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void VisibleChangedNotification (object sender, System.EventArgs e)
		{
			if (Visible)
			{
				MakeSureSelectionIsVisible ();	
			};
		}
	}

   // Contains SDocument structures that hold all documents in folder
	class CTFolderTask
	{
		string _srelpath;
      
      //The things that call this aren't being used right now. 
      //when doing display work, then I will need to make sure this is getting used 
      //and works properly.	  
	  public string GetPathDoc(CLocation loc)
      {
		//return Path.Combine(loc.GetBVTDir(), srelpath);
		return _srelpath;
      }
	
		public SDocument [] rgDocuments;
		public int iview;

		public int Length
		{
			get { return rgDocuments.Length; }
		}

		public CTFolderTask (			
			string srelpath, string [] rgstrDocnames, 
			bool [] rgfEnableDoc, string [] rgstrComments )
		{
			long nDocs = rgstrDocnames.Length;
			long idoc;

			this._srelpath = srelpath;

			this.iview = 0; // Will be set later

			Common.Assert (nDocs == rgfEnableDoc.Length && nDocs == rgstrComments.Length);

			this.rgDocuments = new SDocument [nDocs];

			for (idoc=0; idoc < nDocs; idoc++)
			{
				rgDocuments [idoc].strDocname = rgstrDocnames [idoc];
				rgDocuments [idoc].fEnable = rgfEnableDoc [idoc];
				rgDocuments [idoc].strComment = rgstrComments [idoc];

				rgDocuments [idoc].rgRun = new SRUN [2];

				rgDocuments [idoc].rgRun [0].Initialize ();
				rgDocuments [idoc].rgRun [1].Initialize ();

				rgDocuments [idoc].fSaved = true;
				rgDocuments [idoc].fSelectedForTest = false;
				rgDocuments [idoc].fSelected = false;
				rgDocuments [idoc].fVisible = true;
				rgDocuments [idoc].iglobal = 0;
				rgDocuments [idoc].Item = null;
			}
		}
	}
	
	//Holds individual document information
	struct SDocument
	{
		public string strDocname;
		public bool fEnable;
		public string strComment;
		public bool fSaved;
		public bool fSelectedForTest;
		public bool fSelected;
		public bool fVisible;
		public bool fVisibleTemp;
		public bool fExcelOutputTemp;
		public int iglobal;
		public ListViewItem Item;

		/* Results of run */
		public SRUN [] rgRun;

		public int kRunOld;	// Previous krun - used to count number of new / fixed document during test run.
	}

	// Result of document run so a single failure or success line.
	struct SRUN
	{
		public int kRun;
		public bool fRestart;
		public string strError;
		public string strFlags;

		public void Initialize ()
		{
			kRun = KRun.NotRun;
			fRestart = false;
			strError = "";
			strFlags = "";
		}
	}


	// Managing file with results
	class CDocumentFinder // Helper class for quick document finding
	{
		CTFolderTask [] _rgTasks;
		int _nTasks;
		int _iTaskCur;
		int _idocCur;

		public CDocumentFinder (CTFolderTask [] rgTasks, int nTasks)
		{
			this._rgTasks = rgTasks;
			this._nTasks = nTasks;
			this._iTaskCur = -1;
			this._idocCur = -1;
		}

		public bool Find (CLocation loc, string spathDoc, string strDocname, out int iTask, out int idoc)
		{
			iTask = 0;
			idoc = 0;

			if (_nTasks == 0) return false;

			if (_iTaskCur == -1 || _iTaskCur >= _nTasks)
			{
				_iTaskCur = 0;
				_idocCur = 0;
			}
			else
			{
				if (_idocCur + 1 >= _rgTasks [_iTaskCur].rgDocuments.Length)
				{
					if (_iTaskCur + 1 >= _nTasks) _iTaskCur = 0;
					else _iTaskCur = _iTaskCur + 1;

					_idocCur = 0;
				}
				else
				{
					_idocCur = _idocCur + 1;
				};
			}

			if (Common.CompareStr (spathDoc, _rgTasks [_iTaskCur].GetPathDoc (loc)) == 0 && 
				Common.CompareStr (strDocname, _rgTasks [_iTaskCur].rgDocuments [_idocCur].strDocname) == 0)
			{
				iTask = _iTaskCur;
				idoc = _idocCur;
				return true;
			}
			else
			{
				_iTaskCur = 0;

				while (_iTaskCur < _nTasks)
				{
					_idocCur = 0;
					while (_idocCur < _rgTasks [_iTaskCur].rgDocuments.Length)
					{
						if (Common.CompareStr (spathDoc, _rgTasks [_iTaskCur].GetPathDoc (loc)) == 0 && 
							Common.CompareStr (strDocname, _rgTasks [_iTaskCur].rgDocuments [_idocCur].strDocname) == 0)
						{
							iTask = _iTaskCur;
							idoc = _idocCur;
							return true;
						}
						_idocCur ++;
					}
					_iTaskCur ++;
				}
			}

			return false;
		}
	}

   //not using below until runs generation is going well!

	class TResFile
	{
		static string s_strVersion = "1";
		
		// GetResults 
		public static void GetResults (CLocation loc, string spathRoot, CTFolderTask [] rgTasks, int nTasks, string sfnRun, int irun)
		{
			CInifile inifile;
			int iTask;
			int idoc;
			bool fBadDocumentsFound = false;
			string strVersionCur;
			string strFolderCur;
			string strDocnameCur;
			string strRestartCur;
			string strKRunCur;
			string strFlagsCur;
			string strErrorCur;
			CDocumentFinder DocFinder = new CDocumentFinder (rgTasks, nTasks);

			/* Initialize everything with defaults */
			for (iTask = 0; iTask < nTasks; iTask++)
				for (idoc = 0; idoc < rgTasks [iTask].rgDocuments.Length; idoc++)
					rgTasks [iTask].rgDocuments [idoc].rgRun [irun].kRun = KRun.NotRun;

			if (!File.Exists (sfnRun)) 
			{
				/* No results file => return with all defaults */
				return;
			};

			inifile = new CInifile (sfnRun);

			strVersionCur = inifile.GetNext ("version");

			if (strVersionCur != s_strVersion)
			{
				W11Messages.ShowWarning ("We have changed format of test results. Old results will be unavailble");
				inifile.Close ();
				return; /* return with defaults */
			}

			while (inifile.FGetNext ("Folder", out strFolderCur))
			{
				strFolderCur = Path.Combine (spathRoot, strFolderCur);

				strDocnameCur = inifile.GetNext ("Doc");
				strRestartCur = inifile.GetNext ("Restart");
				strKRunCur = inifile.GetNext ("KRun");
				strFlagsCur = inifile.GetNext ("Flags");
				strErrorCur = inifile.GetNext ("Error");

				if (DocFinder.Find (loc, strFolderCur, strDocnameCur, out iTask, out idoc))
				{
					/* Found document corresponding to this entry */
					CTFolderTask Task = rgTasks [iTask];

					Task.rgDocuments [idoc].rgRun [irun].fRestart = Common.CompareStr (strRestartCur, "On") == 0;
					Task.rgDocuments [idoc].rgRun [irun].kRun = KRun.FromPresentation (strKRunCur);
					Task.rgDocuments [idoc].rgRun [irun].strFlags = strFlagsCur;
					Task.rgDocuments [idoc].rgRun [irun].strError = strErrorCur;
				}
				else
				{
					fBadDocumentsFound = true;
				}
			}

			inifile.Close ();

			if (fBadDocumentsFound)
			{
				/*  Save without bad documents */
				// TResFile.SaveResults (spathRoot, rgTasks, nTasks, sfnRun);
			}
		}

		// SaveResults
		public static void SaveResults (CLocation loc, string spathRoot, CTFolderTask [] rgTasks, int nTasks, string sfnRun)
		{
			StreamWriter stream;
			int iTask, idoc;

			try { stream = new StreamWriter (sfnRun, false); }
			catch  
			{
				W11Messages.RaiseError ("Unable to open file " + sfnRun + ")");
				return; 
			};

			stream.WriteLine ("version = " + s_strVersion);

			for (iTask = 0; iTask < nTasks; iTask++)
			{
				CTFolderTask Task = rgTasks [iTask];

				for (idoc = 0; idoc < Task.rgDocuments.Length; idoc++)
				{
					if (Task.rgDocuments [idoc].rgRun [0].kRun != KRun.NotRun)
					{
						stream.WriteLine (
							GetDocumentRunString ( spathRoot, 
							Task.GetPathDoc (loc), 
							Task.rgDocuments [idoc] ));
					}
				}
			};

			stream.Close ();
		}

		// AppendRun
		public static void AppendRun (string spathRoot, string spathDoc, SDocument document, string sfnRun)
		{
			StreamWriter stream = null;
			AskAsync ask = null;

			if (document.rgRun [0].kRun != KRun.NotRun)
			{
				bool fRetry = true;

				while (fRetry)
				{
					fRetry = false;

					try { stream = new StreamWriter (sfnRun, true); }
					catch  { fRetry = true; };

					if (!fRetry)
					{
						try 
						{
							stream.WriteLine (GetDocumentRunString (spathRoot, spathDoc, document));
						}
						catch { fRetry = true; }
	
						stream.Close ();
					}

					if (fRetry)
					{
						if (ask == null)
						{
							ask = new AskAsync ("Unable to append result to run file " + sfnRun, "Retry", "Cancel", false);
						}

						/* Just wait and try again */
						for (int tsec = 0; tsec < 10 && ask.GetResult () == KAskResult.Undefined; tsec++)
						{
							Thread.Sleep (1000);
						}

						if (ask.GetResult () == KAskResult.No) 
						{
							W11Messages.RaiseError ();
						}
						else if (ask.GetResult () == KAskResult.Yes)
						{
							/* retry */
							ask.Close ();
							ask = null;
						}
					}
				};

				if (ask != null) ask.Close ();
			}
		}

		// GetDocumentRunString
		static string GetDocumentRunString (string spathRoot, string spathDoc, SDocument document)
		{
			string spathDocTail;

			Common.Assert (Common.CompareStr (spathRoot, spathDoc.Substring (0, spathRoot.Length)) == 0);

			if (spathDoc.Length > spathRoot.Length && spathDoc [spathRoot.Length] == '\\')
			{
				spathDocTail = spathDoc.Substring (spathRoot.Length + 1);
			}
			else
			{
				spathDocTail = spathDoc.Substring (spathRoot.Length);
			}

			return "Folder = " + spathDocTail  + "\n" +
				   "Doc = " + document.strDocname + "\n" +
				   "Restart = " + (document.rgRun [0].fRestart ? "On" : "Off") + "\n" +
				   "KRun = " + KRun.ToPresentation (document.rgRun [0].kRun) + "\n" +
				   "Flags = " + document.rgRun [0].strFlags + "\n" +
				   "Error = " + document.rgRun [0].strError;
		}

	}

	/********/
	/*		*/
	/* KRun	*/		// Document run result
	/*		*/
	/********/

	class KRun
	{

		public static int NotRun = 0;
		public static int OK = 1;
		public static int EQ = 2;

		public static int ErrorAF = 3;
		public static int ErrorUnfinished = 4;
		public static int ErrorTimeout = 5;
		public static int ErrorNeq = 6;
		public static int ErrorUnknown = 7;

		public static bool IsError (int kRun)
		{
			return (kRun != KRun.NotRun && kRun != KRun.OK && kRun != KRun.EQ);
		}

		public static string ToDisplay (int kRun)
		{
			switch (kRun)
			{
			case 0: return "";
			case 1: return "OK";
			case 2: return "EQ";
			case 3: return "Error";
			case 4: return "Error";
			case 5: return "Error";
			case 6: return "Error";
			case 7: return "Error";

			default: Common.Assert (false, "Invalid krun"); return "";
			}
		}

		public static string ToErrorString (int kRun)
		{
			switch (kRun)
			{
			case 0: return "";
			case 1: return "";
			case 2: return "";
			case 3: return "Assert";
			case 4: return "Stopped";
			case 5: return "Timeout";
			case 6: return "Compare";
			case 7: return "Error";

			default: Common.Assert (false, "Invalid krun"); return "";
			}
		}

		public static string ToPresentation (int kRun)
		{
			switch (kRun)
			{
			case 0: return "";
			case 1: return "OK";
			case 2: return "EQ";
			case 3: return "ERROR.AF";
			case 4: return "ERROR.UNFINISHED";
			case 5: return "ERROR.TIMEOUT";
			case 6: return "ERROR.NEQ";

			case 7: return "ERROR";

			default: Common.Assert (false, "Invalid krun"); return "";
			}
		}

		public static int FromPresentation (string str)
		{
			str = str.ToUpper ();

			if (str == "") return KRun.NotRun;
			else if (str == "OK") return KRun.OK;
			else if (str == "EQ") return KRun.EQ;
			else if (str == "ERROR.AF") return KRun.ErrorAF;
			else if (str == "ERROR.UNFINISHED") return KRun.ErrorUnfinished;
			else if (str == "TIMEOUT") return KRun.ErrorTimeout;
			else if (str == "ERROR.TIMEOUT") return KRun.ErrorTimeout;
			else if (str == "ERROR.NEQ") return KRun.ErrorNeq;

			/* legacy values */
			else if (str == "ERROR") return KRun.ErrorUnknown;
			else if (str == "UNFINISHED") return KRun.ErrorUnfinished;

			else Common.Assert (false, "Invalid krun"); return 0;
		}
	}

	enum KTest
	{
		All,
		AllCurrent,
		Selected
	}
}