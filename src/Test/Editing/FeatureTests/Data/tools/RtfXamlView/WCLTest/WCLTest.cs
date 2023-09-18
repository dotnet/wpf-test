// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using Microsoft.Win32;
using System.Runtime.InteropServices;


namespace W11Builder
{
   using Common;
   using Location;
   using Configuration;
   using Runword;
   using Build;
   using Tests;
   using Language;
   using Protocol;
   using Ask;
   using TestList;
   using Layout;

   delegate void CDelegateFinishedTask();
   delegate void CDelegateTestProgress(int ndocsCompleted, int ndocsTotal, int nErrors,
                               int nErrorsFixed, int nErrorsNew,

                               string spathTestFolder, bool fTestETA,
                               DateTime dtTestStarted, DateTime dtTestETA);

   public class W11Builder : System.Windows.Forms.Form
   {
      new CLocation _location;					// All file locations

      //string[] rgstrConfigurationName;		// Configuration names
      //SBuildInfo[] rgBuildInfo;				// Build information for last attempted debug & ship builds

      /* Test information */
      bool _fNoAssertDialog;			// Do not show Assert dialog in Word

      //int tsecTimeLimit;				// Time limit
      bool _fRestartEveryDoc;			// Restart word on every doc
      int _kTestStatus;				// Status of the last test
      int _nTests;						// Number of tests on the last run
      int _nTestsError;				// Number of errors on the last run

      bool _fAllowDialogDataExchange = false;
      // If data exchange is allowed

      /* Runner-objects objects */

      bool _fRunningTask;
      int _kRunningTaskCur;
      int _nStopCounter;				// How many times end-user hit "stop"

      CRunword _runwordObject;
      CTests _testsObject;
      CTestList _testlistObject;

      Color _colorStopButtonNormalBackground;

      CDelegateFinishedTask _delegateFinishedTask;
      CDelegateTestProgress _delegateTestProgress;

      /* Saved configuration from previous run */
      //string spathTestFolderSaved;
      //string strConfgurationNameSaved;
      //int targetSaved;
      private System.Windows.Forms.Label _labelTestFolder;
      private System.Windows.Forms.MenuItem _miRtfXamlBVT;
      private System.Windows.Forms.MenuItem _menuItemTest;
      private System.Windows.Forms.MenuItem _miOptions;
      private System.Windows.Forms.MenuItem _menuItem10;
      private System.Windows.Forms.MenuItem _miRTXamlBVT;
      private MenuItem _miLaunchRTFXAMLVIEW;
      private MenuItem _miLaunchWord;
      private MenuItem _miRTRtfBVT;
      private GroupBox _groupBox3;
      private CheckBox _chkWordAssert;
      private CheckBox _chkREAssert;
      private CheckBox _chkRTFXAML;
      private CheckBox _chkRTXaml;
      private CheckBox _chkRTRTF;
      private Label _label5;
      private Label _label7;
      private Button _btnTestLog;
      private Button _btnRunTests;
      private Label _label3;
      private GroupBox _groupBox2;
      private CheckBox _chkPri0;
      private CheckBox _chkUseXCVT;
      private CheckBox _chkNoAssertDialog;
      private CheckBox _chkRestartEveryDoc;
      private Button _btnStop;
      private LinkLabel _linkBVTDir;
      private Panel _panel2;
      private Label _labelTestProgressInfo;
      private Label _labelNewOldErrors;
      private ProgressBar _progressBarTest;
      private Label _labelTestErrorWarning;
      private Label _label4;
      private Label _labelTestInformation;
      private LinkLabel _linkLogFile;
      private LinkLabel _linkLogDir;
      private GroupBox _groupBoxBackground;
      private MenuItem _miOldBVT;

      // Import system Beep API
      [DllImport("kernel32.dll")]
      public static extern void Beep(int freq, int duration);
      private System.Windows.Forms.MainMenu _mainMenu1;
      private System.Windows.Forms.MenuItem _menuItem7;
      private System.Windows.Forms.MenuItem _menuItem8;
      private System.Windows.Forms.MenuItem _menuItemFile;
      private System.Windows.Forms.MenuItem _menuItemHelp;
      private IContainer _components;

      // MainForm consructor - calls Designer's initializaton 
      // and initializes other object variables.
      public W11Builder()
      {
         try
         {
            // Designer initializatoion first
            InitializeComponent();

            // Our initializaton
            InitializeMainForm();

            _fAllowDialogDataExchange = true;
            UpdateControlsAfterStatusChange();
         }
         catch (System.Security.SecurityException)
         {
            W11Messages.RaiseError("This is .NET security exception. If you are runing W11Builder from shared drive, you have to increase trust level.\n\n" +
               "Please follow these steps: \n\n" +
               "1. Control Panel => Administrative Tools => \n" +
               "2. Microsoft .NET Framework 1.1 Configuration  =>\n" +
               "3. Runtime Security Policy ( left ) =>\n" +
               "4. Adjust Zone Security ( right ) =>\n" +
               "5. Make changes to this computer, next =>\n" +
               "6. Choose Local Intranet\n" +
               "7. Set trust bar to Full Trust");
         }
      }

      // Core function for our initialization
      void InitializeMainForm()
      {
         string[] rgstrProgramArgs = Environment.GetCommandLineArgs();
         string spathExe;
         SOptions options;
         bool fExitApp;
         //string pathTestFolder;

         // Initialize messages
         W11Messages.Initialize(this);

         ProcessArguments(rgstrProgramArgs, out spathExe);

         // Delete function: notification that running object (test, build, word) has finished
         _delegateFinishedTask = new CDelegateFinishedTask(FinishedTaskNotification);
         _delegateTestProgress = new CDelegateTestProgress(TestProgressNotification);

         _fRunningTask = false;

         // Ask
         Ask.Initialize();

         // Build and General data
         // Load configuration from the registry
         Configuration.GetOptionsOnLoad(out options, out fExitApp);

         if (fExitApp)
         {
            W11Messages.RaiseError();
         }

         // In case of error -- will clean it up if return is successful
         Configuration.SetOptionsDialogRequestAfterRetart(true);

         //Configuration.ReadConfiguration(out spathTestFolderSaved, out strConfgurationNameSaved, out targetSaved);

         //if (options.fUseDefaultTestFolder)
         //{
         // pathTestFolder = Path.Combine (Path.GetDirectoryName(Path.GetDirectoryName (spathExe)), "Tests\\BVT");
         //pathTestFolder = options.szBVTDir;
         //}
         //else pathTestFolder = spathTestFolderSaved;

         /* Initialize location object */
         _location = new CLocation(options);

         //runwordObject = new CRunword(this, Location);

         InitializeMainDialog(true);
      }


      void InitializeMainDialog(bool fFirstTime)
      {
         _fAllowDialogDataExchange = false;

         // Read compileable wal configurations
         //if (!Location.options.fInstallOnly)
         //{
         //   rgstrConfigurationName = Common.ReadDirectotyNames(Location.GetPathDev(), "Word.*");
         //   if (rgstrConfigurationName.Length == 0)
         //   {
         //      W11Messages.ShowError("Word is not found in " + Location.GetPathDev());
         //      rgstrConfigurationName = new string[1];
         //      rgstrConfigurationName[0] = "word";
         //   };
         //}
         //else
         //{
         //   rgstrConfigurationName = new string[1];
         //   rgstrConfigurationName[0] = "word";
         //};

         // Add configuration names to drop-down combobox
         //comboBox1.Items.Clear();
         //for (int ic = 0; ic < rgstrConfigurationName.Length; ic++)
         //{
         //   comboBox1.Items.Add(rgstrConfigurationName[ic]);
         //};

         if (fFirstTime)
         {
            //int iconfigSaved;

            /* Set current state defaults */
            //iConfig = 0;
            //kTarget = KTarget.Debug;
            //SetFCleanWal(false);
            //SetFCleanWord(false);
            //SetFFullLink(false);

            //iconfigSaved = Common.FindStringInArray(rgstrConfigurationName, strConfgurationNameSaved);
            //if (iconfigSaved != -1)
            //{
            //iConfig = iconfigSaved;
            //kTarget = targetSaved;
            //}
         }

         /* Set build information */
         //rgBuildInfo = new SBuildInfo[KTarget.Range];

         //for (int itarget = 0; itarget < KTarget.Range; itarget++)
         //{
         //   rgBuildInfo[itarget].Success = KBuildSuccess.NotAvailable;
         //   rgBuildInfo[itarget].StrConfigName = "";
         //   rgBuildInfo[itarget].FInCurrentSession = false;
         //};

         /* ---------------------------------------------- */
         /* Run object data								  */
         /* ---------------------------------------------- */

         _nStopCounter = 0;
         _colorStopButtonNormalBackground = _btnStop.BackColor;

         /* ---------------------------------------------- */
         /* Test data									  */
         /* ---------------------------------------------- */

         //kLayoutOut = KLayoutOut.Compare;
         //fWordOld = false;
         _fRestartEveryDoc = false;
         //fFixOldCode = true;
         //fTimeLimit = true;
         //fLayoutFolders = false;

         //Location.StrCompareFolderName = GetDefaultCompareFolderName();
         //Location.StrOutputFolderName = GetDefaultOutputFolderName();

         //kCompare = KCompare.All;
         //fIgnoreNestedPositions = false;
         //tsecTimeLimit = 30;
         _fNoAssertDialog = false;
         //SetFFuzzyDupCompare(true);
         _kTestStatus = KTestStatus.Unknown;

         //if (Location.options.kword == KWord.w12) fPtsOnWord12 = GetFPtsOnWord12(Location);
         //else fPtsOnWord12 = false;

         _testlistObject = new CTestList(_location, this, _delegateFinishedTask);

         _testsObject = new CTests(_location, this, _testlistObject,
            _delegateFinishedTask, _delegateTestProgress);

         _labelTestErrorWarning.Visible = false;
         _labelNewOldErrors.Visible = false;
         _labelTestProgressInfo.Visible = false;
         //labelTestETA.Visible = false;
         //labelTestSta.Visible = false;
         _progressBarTest.Visible = false;

         //InitMacros();

         // Language

         //  Check language tune up
         //CLanguageSettings.CheckLanguageTuneUp(Location.options.kword);

         // Update display controls
         UpdateControlsAfterStatusChange();

         _fAllowDialogDataExchange = true;
      }


      static void ArgumentError(string strmsg)
      {
         W11Messages.RaiseError("Wrong parameter string" + (strmsg == "" ? "" : ": " + strmsg) + "\n\n" +
                           "Please use this syntax:\n" +
                           "	  W11Builder [<w11 | w12> [Office path]]\n\n" +
                           "Defaults: w11; office path is relative to w11builder.exe, assuming that it is checked in O11WAL");
      }

      static void ProcessArguments(string[] rgstrArgs, out string spathExe)
      {
         Common.Assert(rgstrArgs.Length > 0);
         spathExe = Path.GetDirectoryName(Path.GetFullPath(rgstrArgs[0]));
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (_components != null)
            {
               _components.Dispose();
            }
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
         this._components = new System.ComponentModel.Container();
         this._mainMenu1 = new System.Windows.Forms.MainMenu(this._components);
         this._menuItemFile = new System.Windows.Forms.MenuItem();
         this._miOptions = new System.Windows.Forms.MenuItem();
         this._miLaunchRTFXAMLVIEW = new System.Windows.Forms.MenuItem();
         this._miLaunchWord = new System.Windows.Forms.MenuItem();
         this._menuItem10 = new System.Windows.Forms.MenuItem();
         this._menuItem7 = new System.Windows.Forms.MenuItem();
         this._menuItemTest = new System.Windows.Forms.MenuItem();
         this._miRtfXamlBVT = new System.Windows.Forms.MenuItem();
         this._miRTXamlBVT = new System.Windows.Forms.MenuItem();
         this._miRTRtfBVT = new System.Windows.Forms.MenuItem();
         this._miOldBVT = new System.Windows.Forms.MenuItem();
         this._menuItemHelp = new System.Windows.Forms.MenuItem();
         this._menuItem8 = new System.Windows.Forms.MenuItem();
         this._labelTestFolder = new System.Windows.Forms.Label();
         this._groupBox3 = new System.Windows.Forms.GroupBox();
         this._chkWordAssert = new System.Windows.Forms.CheckBox();
         this._chkREAssert = new System.Windows.Forms.CheckBox();
         this._chkRTFXAML = new System.Windows.Forms.CheckBox();
         this._chkRTXaml = new System.Windows.Forms.CheckBox();
         this._chkRTRTF = new System.Windows.Forms.CheckBox();
         this._label5 = new System.Windows.Forms.Label();
         this._label7 = new System.Windows.Forms.Label();
         this._btnTestLog = new System.Windows.Forms.Button();
         this._btnRunTests = new System.Windows.Forms.Button();
         this._label3 = new System.Windows.Forms.Label();
         this._groupBox2 = new System.Windows.Forms.GroupBox();
         this._chkPri0 = new System.Windows.Forms.CheckBox();
         this._chkUseXCVT = new System.Windows.Forms.CheckBox();
         this._chkNoAssertDialog = new System.Windows.Forms.CheckBox();
         this._chkRestartEveryDoc = new System.Windows.Forms.CheckBox();
         this._btnStop = new System.Windows.Forms.Button();
         this._linkBVTDir = new System.Windows.Forms.LinkLabel();
         this._panel2 = new System.Windows.Forms.Panel();
         this._labelTestProgressInfo = new System.Windows.Forms.Label();
         this._labelNewOldErrors = new System.Windows.Forms.Label();
         this._progressBarTest = new System.Windows.Forms.ProgressBar();
         this._labelTestErrorWarning = new System.Windows.Forms.Label();
         this._label4 = new System.Windows.Forms.Label();
         this._labelTestInformation = new System.Windows.Forms.Label();
         this._linkLogFile = new System.Windows.Forms.LinkLabel();
         this._linkLogDir = new System.Windows.Forms.LinkLabel();
         this._groupBoxBackground = new System.Windows.Forms.GroupBox();
         this._groupBox3.SuspendLayout();
         this._groupBox2.SuspendLayout();
         this._panel2.SuspendLayout();
         this._groupBoxBackground.SuspendLayout();
         this.SuspendLayout();
         // 
         // mainMenu1
         // 
         this._mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuItemFile,
            this._menuItemTest,
            this._menuItemHelp});
         // 
         // menuItemFile
         // 
         this._menuItemFile.Index = 0;
         this._menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._miOptions,
            this._miLaunchRTFXAMLVIEW,
            this._miLaunchWord,
            this._menuItem10,
            this._menuItem7});
         this._menuItemFile.Text = "&File";
         // 
         // miOptions
         // 
         this._miOptions.Index = 0;
         this._miOptions.Text = "&Options";
         this._miOptions.Click += new System.EventHandler(this.menuItem2_Click);
         // 
         // miLaunchRTFXAMLVIEW
         // 
         this._miLaunchRTFXAMLVIEW.Index = 1;
         this._miLaunchRTFXAMLVIEW.Text = "Launch RtfXamlView";
         // 
         // miLaunchWord
         // 
         this._miLaunchWord.Index = 2;
         this._miLaunchWord.Text = "Launch Word";
         // 
         // menuItem10
         // 
         this._menuItem10.Index = 3;
         this._menuItem10.Text = "-";
         // 
         // menuItem7
         // 
         this._menuItem7.Index = 4;
         this._menuItem7.Text = "&Close";
         this._menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
         // 
         // menuItemTest
         // 
         this._menuItemTest.Index = 1;
         this._menuItemTest.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._miRtfXamlBVT,
            this._miRTXamlBVT,
            this._miRTRtfBVT,
            this._miOldBVT});
         this._menuItemTest.Text = "&Test";
         // 
         // miRtfXamlBVT
         // 
         this._miRtfXamlBVT.Index = 0;
         this._miRtfXamlBVT.Text = "Rtf to Xaml bvt";
         this._miRtfXamlBVT.Click += new System.EventHandler(this.menuItem9_Click);
         // 
         // miRTXamlBVT
         // 
         this._miRTXamlBVT.Index = 1;
         this._miRTXamlBVT.Text = "Xaml to Xaml BVT";
         this._miRTXamlBVT.Click += new System.EventHandler(this.menuItem11_Click);
         // 
         // miRTRtfBVT
         // 
         this._miRTRtfBVT.Index = 2;
         this._miRTRtfBVT.Text = "Rtf To Rtf BVT";
         // 
         // miOldBVT
         // 
         this._miOldBVT.Index = 3;
         this._miOldBVT.Text = "Old BVT";
         // 
         // menuItemHelp
         // 
         this._menuItemHelp.Index = 2;
         this._menuItemHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuItem8});
         this._menuItemHelp.Text = "&Help";
         // 
         // menuItem8
         // 
         this._menuItem8.Index = 0;
         this._menuItem8.Text = "&About";
         this._menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
         // 
         // labelTestFolder
         // 
         this._labelTestFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._labelTestFolder.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
         this._labelTestFolder.Location = new System.Drawing.Point(2, 337);
         this._labelTestFolder.Name = "labelTestFolder";
         this._labelTestFolder.Size = new System.Drawing.Size(336, 24);
         this._labelTestFolder.TabIndex = 38;
         this._labelTestFolder.Text = "C:\\E\\O11WAL\\Tests\\BVT\\Docs";
         this._labelTestFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this._labelTestFolder.Visible = false;
         // 
         // groupBox3
         // 
         this._groupBox3.Controls.Add(this._chkWordAssert);
         this._groupBox3.Controls.Add(this._chkREAssert);
         this._groupBox3.Controls.Add(this._chkRTFXAML);
         this._groupBox3.Controls.Add(this._chkRTXaml);
         this._groupBox3.Controls.Add(this._chkRTRTF);
         this._groupBox3.Location = new System.Drawing.Point(13, 200);
         this._groupBox3.Name = "groupBox3";
         this._groupBox3.Size = new System.Drawing.Size(280, 88);
         this._groupBox3.TabIndex = 45;
         this._groupBox3.TabStop = false;
         this._groupBox3.Text = "BVT Tests to Run";
         // 
         // chkWordAssert
         // 
         this._chkWordAssert.Location = new System.Drawing.Point(132, 41);
         this._chkWordAssert.Name = "chkWordAssert";
         this._chkWordAssert.Size = new System.Drawing.Size(142, 16);
         this._chkWordAssert.TabIndex = 48;
         this._chkWordAssert.Text = "Word Assert Check";
         // 
         // chkREAssert
         // 
         this._chkREAssert.Location = new System.Drawing.Point(132, 19);
         this._chkREAssert.Name = "chkREAssert";
         this._chkREAssert.Size = new System.Drawing.Size(142, 16);
         this._chkREAssert.TabIndex = 47;
         this._chkREAssert.Text = "RichEdit Assert Check";
         this._chkREAssert.CheckedChanged += new System.EventHandler(this.EdiitingChange);
         // 
         // chkRTFXAML
         // 
         this._chkRTFXAML.Location = new System.Drawing.Point(6, 63);
         this._chkRTFXAML.Name = "chkRTFXAML";
         this._chkRTFXAML.Size = new System.Drawing.Size(110, 16);
         this._chkRTFXAML.TabIndex = 46;
         this._chkRTFXAML.Text = "RTF to Xaml";
         this._chkRTFXAML.CheckedChanged += new System.EventHandler(this.EdiitingChange);
         // 
         // chkRTXaml
         // 
         this._chkRTXaml.Location = new System.Drawing.Point(5, 41);
         this._chkRTXaml.Name = "chkRTXaml";
         this._chkRTXaml.Size = new System.Drawing.Size(110, 16);
         this._chkRTXaml.TabIndex = 37;
         this._chkRTXaml.Text = "Xaml Round Trip";
         this._chkRTXaml.CheckedChanged += new System.EventHandler(this.EdiitingChange);
         // 
         // chkRTRTF
         // 
         this._chkRTRTF.Location = new System.Drawing.Point(6, 19);
         this._chkRTRTF.Name = "chkRTRTF";
         this._chkRTRTF.Size = new System.Drawing.Size(120, 16);
         this._chkRTRTF.TabIndex = 45;
         this._chkRTRTF.Text = "Rtf Round Trip";
         this._chkRTRTF.CheckedChanged += new System.EventHandler(this.EdiitingChange);
         // 
         // label5
         // 
         this._label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._label5.Location = new System.Drawing.Point(5, 40);
         this._label5.Name = "label5";
         this._label5.Size = new System.Drawing.Size(80, 16);
         this._label5.TabIndex = 49;
         this._label5.Text = "Current log file:";
         // 
         // label7
         // 
         this._label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._label7.Location = new System.Drawing.Point(5, 66);
         this._label7.Name = "label7";
         this._label7.Size = new System.Drawing.Size(107, 16);
         this._label7.TabIndex = 50;
         this._label7.Text = "Current Log Folder:";
         // 
         // BtnTestLog
         // 
         this._btnTestLog.Location = new System.Drawing.Point(306, 105);
         this._btnTestLog.Name = "BtnTestLog";
         this._btnTestLog.Size = new System.Drawing.Size(88, 28);
         this._btnTestLog.TabIndex = 24;
         this._btnTestLog.Text = "VIEW";
         this._btnTestLog.Click += new System.EventHandler(this.BtnTestLog_Click);
         // 
         // BtnRunTests
         // 
         this._btnRunTests.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._btnRunTests.Location = new System.Drawing.Point(304, 207);
         this._btnRunTests.Name = "BtnRunTests";
         this._btnRunTests.Size = new System.Drawing.Size(88, 28);
         this._btnRunTests.TabIndex = 39;
         this._btnRunTests.Text = "Run Tests";
         this._btnRunTests.Click += new System.EventHandler(this.BtnRunTests_Click);
         // 
         // label3
         // 
         this._label3.Location = new System.Drawing.Point(6, 16);
         this._label3.Name = "label3";
         this._label3.Size = new System.Drawing.Size(82, 24);
         this._label3.TabIndex = 34;
         this._label3.Text = "BVT Location:";
         // 
         // groupBox2
         // 
         this._groupBox2.Controls.Add(this._chkPri0);
         this._groupBox2.Controls.Add(this._chkUseXCVT);
         this._groupBox2.Controls.Add(this._chkNoAssertDialog);
         this._groupBox2.Controls.Add(this._chkRestartEveryDoc);
         this._groupBox2.Location = new System.Drawing.Point(9, 99);
         this._groupBox2.Name = "groupBox2";
         this._groupBox2.Size = new System.Drawing.Size(280, 95);
         this._groupBox2.TabIndex = 30;
         this._groupBox2.TabStop = false;
         this._groupBox2.Text = "RTFXAMLVIEW Options";
         // 
         // chkPri0
         // 
         this._chkPri0.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._chkPri0.Location = new System.Drawing.Point(7, 19);
         this._chkPri0.Name = "chkPri0";
         this._chkPri0.Size = new System.Drawing.Size(118, 16);
         this._chkPri0.TabIndex = 45;
         this._chkPri0.Text = "Log Pri 0 only";
         this._chkPri0.CheckedChanged += new System.EventHandler(this.EdiitingChange);
         // 
         // chkUseXCVT
         // 
         this._chkUseXCVT.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._chkUseXCVT.Location = new System.Drawing.Point(154, 18);
         this._chkUseXCVT.Name = "chkUseXCVT";
         this._chkUseXCVT.Size = new System.Drawing.Size(118, 16);
         this._chkUseXCVT.TabIndex = 44;
         this._chkUseXCVT.Text = "Use XCVT";
         this._chkUseXCVT.CheckedChanged += new System.EventHandler(this.EdiitingChange);
         // 
         // chkNoAssertDialog
         // 
         this._chkNoAssertDialog.Enabled = false;
         this._chkNoAssertDialog.Location = new System.Drawing.Point(154, 42);
         this._chkNoAssertDialog.Name = "chkNoAssertDialog";
         this._chkNoAssertDialog.Size = new System.Drawing.Size(112, 16);
         this._chkNoAssertDialog.TabIndex = 36;
         this._chkNoAssertDialog.Text = "No Assert dialog";
         this._chkNoAssertDialog.CheckedChanged += new System.EventHandler(this.EdiitingChange);
         // 
         // chkRestartEveryDoc
         // 
         this._chkRestartEveryDoc.Location = new System.Drawing.Point(154, 66);
         this._chkRestartEveryDoc.Name = "chkRestartEveryDoc";
         this._chkRestartEveryDoc.Size = new System.Drawing.Size(112, 16);
         this._chkRestartEveryDoc.TabIndex = 35;
         this._chkRestartEveryDoc.Text = "Restart every doc";
         this._chkRestartEveryDoc.CheckedChanged += new System.EventHandler(this.EdiitingChange);
         // 
         // btnStop
         // 
         this._btnStop.BackColor = System.Drawing.SystemColors.Control;
         this._btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._btnStop.Location = new System.Drawing.Point(305, 260);
         this._btnStop.Name = "btnStop";
         this._btnStop.Size = new System.Drawing.Size(88, 28);
         this._btnStop.TabIndex = 32;
         this._btnStop.Text = "STOP";
         this._btnStop.UseVisualStyleBackColor = false;
         this._btnStop.Click += new System.EventHandler(this.buttonStop_Click);
         // 
         // linkBVTDir
         // 
         this._linkBVTDir.Cursor = System.Windows.Forms.Cursors.Default;
         this._linkBVTDir.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._linkBVTDir.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(255)))));
         this._linkBVTDir.Location = new System.Drawing.Point(84, 16);
         this._linkBVTDir.Name = "linkBVTDir";
         this._linkBVTDir.Size = new System.Drawing.Size(303, 25);
         this._linkBVTDir.TabIndex = 0;
         this._linkBVTDir.TabStop = true;
         this._linkBVTDir.Text = "C:\\TEST FOLDER";
         this._linkBVTDir.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkBCTDir_LinkClicked);
         // 
         // panel2
         // 
         this._panel2.BackColor = System.Drawing.Color.Silver;
         this._panel2.Controls.Add(this._labelTestProgressInfo);
         this._panel2.Controls.Add(this._labelNewOldErrors);
         this._panel2.Controls.Add(this._progressBarTest);
         this._panel2.Controls.Add(this._labelTestErrorWarning);
         this._panel2.Controls.Add(this._label4);
         this._panel2.Controls.Add(this._labelTestInformation);
         this._panel2.Location = new System.Drawing.Point(0, 307);
         this._panel2.Name = "panel2";
         this._panel2.Size = new System.Drawing.Size(395, 27);
         this._panel2.TabIndex = 21;
         // 
         // labelTestProgressInfo
         // 
         this._labelTestProgressInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._labelTestProgressInfo.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
         this._labelTestProgressInfo.Location = new System.Drawing.Point(184, 8);
         this._labelTestProgressInfo.Name = "labelTestProgressInfo";
         this._labelTestProgressInfo.Size = new System.Drawing.Size(88, 16);
         this._labelTestProgressInfo.TabIndex = 31;
         this._labelTestProgressInfo.Text = "11222 * 13300";
         this._labelTestProgressInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // labelNewOldErrors
         // 
         this._labelNewOldErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._labelNewOldErrors.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
         this._labelNewOldErrors.Location = new System.Drawing.Point(125, 8);
         this._labelNewOldErrors.Name = "labelNewOldErrors";
         this._labelNewOldErrors.Size = new System.Drawing.Size(75, 16);
         this._labelNewOldErrors.TabIndex = 32;
         this._labelNewOldErrors.Text = "+ 5  - 10";
         // 
         // progressBarTest
         // 
         this._progressBarTest.Location = new System.Drawing.Point(280, 6);
         this._progressBarTest.Name = "progressBarTest";
         this._progressBarTest.Size = new System.Drawing.Size(96, 16);
         this._progressBarTest.TabIndex = 29;
         // 
         // labelTestErrorWarning
         // 
         this._labelTestErrorWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._labelTestErrorWarning.ForeColor = System.Drawing.Color.Red;
         this._labelTestErrorWarning.Location = new System.Drawing.Point(48, 7);
         this._labelTestErrorWarning.Name = "labelTestErrorWarning";
         this._labelTestErrorWarning.Size = new System.Drawing.Size(72, 16);
         this._labelTestErrorWarning.TabIndex = 30;
         this._labelTestErrorWarning.Text = "ERR = 3211";
         // 
         // label4
         // 
         this._label4.BackColor = System.Drawing.Color.Transparent;
         this._label4.Location = new System.Drawing.Point(8, 8);
         this._label4.Name = "label4";
         this._label4.Size = new System.Drawing.Size(40, 16);
         this._label4.TabIndex = 28;
         this._label4.Text = "TEST:";
         // 
         // labelTestInformation
         // 
         this._labelTestInformation.BackColor = System.Drawing.Color.Transparent;
         this._labelTestInformation.Location = new System.Drawing.Point(48, 8);
         this._labelTestInformation.Name = "labelTestInformation";
         this._labelTestInformation.Size = new System.Drawing.Size(312, 16);
         this._labelTestInformation.TabIndex = 28;
         this._labelTestInformation.Text = "RUNNING";
         // 
         // linkLogFile
         // 
         this._linkLogFile.Cursor = System.Windows.Forms.Cursors.Default;
         this._linkLogFile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._linkLogFile.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(255)))));
         this._linkLogFile.Location = new System.Drawing.Point(87, 42);
         this._linkLogFile.Name = "linkLogFile";
         this._linkLogFile.Size = new System.Drawing.Size(291, 16);
         this._linkLogFile.TabIndex = 51;
         this._linkLogFile.TabStop = true;
         this._linkLogFile.Text = "Layout.temp";
         this._linkLogFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLogFile_LinkClicked);
         // 
         // linkLogDir
         // 
         this._linkLogDir.Cursor = System.Windows.Forms.Cursors.Default;
         this._linkLogDir.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._linkLogDir.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(255)))));
         this._linkLogDir.Location = new System.Drawing.Point(101, 66);
         this._linkLogDir.Name = "linkLogDir";
         this._linkLogDir.Size = new System.Drawing.Size(287, 16);
         this._linkLogDir.TabIndex = 52;
         this._linkLogDir.TabStop = true;
         this._linkLogDir.Text = "C:\\TestLogs";
         this._linkLogDir.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLogDir_LinkClicked);
         // 
         // groupBoxBackground
         // 
         this._groupBoxBackground.BackColor = System.Drawing.SystemColors.Control;
         this._groupBoxBackground.Controls.Add(this._linkLogDir);
         this._groupBoxBackground.Controls.Add(this._linkLogFile);
         this._groupBoxBackground.Controls.Add(this._panel2);
         this._groupBoxBackground.Controls.Add(this._labelTestFolder);
         this._groupBoxBackground.Controls.Add(this._linkBVTDir);
         this._groupBoxBackground.Controls.Add(this._btnStop);
         this._groupBoxBackground.Controls.Add(this._groupBox2);
         this._groupBoxBackground.Controls.Add(this._label3);
         this._groupBoxBackground.Controls.Add(this._btnRunTests);
         this._groupBoxBackground.Controls.Add(this._btnTestLog);
         this._groupBoxBackground.Controls.Add(this._label7);
         this._groupBoxBackground.Controls.Add(this._label5);
         this._groupBoxBackground.Controls.Add(this._groupBox3);
         this._groupBoxBackground.Location = new System.Drawing.Point(4, -9);
         this._groupBoxBackground.Name = "groupBoxBackground";
         this._groupBoxBackground.Size = new System.Drawing.Size(400, 367);
         this._groupBoxBackground.TabIndex = 33;
         this._groupBoxBackground.TabStop = false;
         // 
         // W11Builder
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.BackColor = System.Drawing.SystemColors.Control;
         this.ClientSize = new System.Drawing.Size(414, 359);
         this.Controls.Add(this._groupBoxBackground);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Menu = this._mainMenu1;
         this.Name = "W11Builder";
         this.Text = "WCLTest 4.0";
         this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
         this._groupBox3.ResumeLayout(false);
         this._groupBox2.ResumeLayout(false);
         this._panel2.ResumeLayout(false);
         this._groupBoxBackground.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      // Main function (entry point)
      [STAThreadAttribute]
      static void Main()
      {
         try
         {
            Application.Run(new W11Builder());
            Configuration.SetOptionsDialogRequestAfterRetart(false);
         }
         catch (W11Exception)
         {
            // This is our internal error
            // Since error message was already displayed, just return
         }
      }

      // This method updates dialog controls to reflect new status
      void UpdateControlsAfterStatusChange()
      {
         bool fAllowDialogDataExchange_Saved = _fAllowDialogDataExchange;
         // To avoid bouncing back & forth between this method & EditingChange
         _fAllowDialogDataExchange = false;

         #region
         _menuItemFile.Enabled = !_fRunningTask;
         _menuItemHelp.Enabled = !_fRunningTask;
         _menuItemTest.Enabled = !_fRunningTask;
         // Enable launch of tests from menu.
         _miRtfXamlBVT.Enabled = !_fRunningTask;
         _miRTXamlBVT.Enabled = !_fRunningTask;
         _miRTRtfBVT.Enabled = !_fRunningTask;
         _miOldBVT.Enabled = !_fRunningTask;
         _miLaunchRTFXAMLVIEW.Enabled = !_fRunningTask;
         _miLaunchWord.Enabled = !_fRunningTask;
         #endregion

         #region
         //locations
         _linkBVTDir.Text = (_location.GetBVTDir() == "" ? "Select test folder" : _location.GetBVTDir());
         _linkBVTDir.Enabled = !_fRunningTask;
         _linkLogFile.Enabled = !_fRunningTask;
         _linkLogFile.Text = _location.GetLogFile();
         _linkLogDir.Text = _location.GetLogDir();
         _linkLogDir.Enabled = !_fRunningTask;
         //misc
         _chkRestartEveryDoc.Checked = _location.options.fRestartDocs;
         _chkRestartEveryDoc.Enabled = !_fRunningTask;
         _chkUseXCVT.Checked = _location.options.fUseXCVT;
         _chkUseXCVT.Enabled = !_fRunningTask;
         _chkPri0.Checked = _location.options.fPri0;
         _chkPri0.Enabled = !_fRunningTask;
         //tests
         _chkRTRTF.Checked = _location.options.fRTRtf;
         _chkRTRTF.Enabled = !_fRunningTask;
         _chkRTXaml.Checked = _location.options.fRTXaml;
         _chkRTXaml.Enabled = !_fRunningTask;
         _chkRTFXAML.Checked = _location.options.fRtfXaml;
         _chkRTFXAML.Enabled = !_fRunningTask;
         _chkREAssert.Checked = _location.options.fREAssert;
         _chkREAssert.Enabled = !_fRunningTask;
         _chkWordAssert.Checked = _location.options.fWordAssert;//not implemented yet
         _chkWordAssert.Enabled = false;//!fRunningTask;

         //not used currently
         //chkNoAssertDialog.Checked = fNoAssertDialog;
         //chkNoAssertDialog.Enabled = !fRunningTask && Location.options.kword == KWord.w11;
         #endregion

         if ((_fRunningTask && _kRunningTaskCur != KRunningTask.Tests) ||
            _kTestStatus == KTestStatus.Unknown)
         {
            _panel2.BackColor = Color.Silver;
         }
         else if (_kTestStatus == KTestStatus.OK) _panel2.BackColor = Color.FromArgb(100, 200, 100);
         else if (_kTestStatus == KTestStatus.Running) _panel2.BackColor = Color.LightSteelBlue;
         else _panel2.BackColor = Color.FromArgb(200, 130, 130);

         _labelTestInformation.Text = KTestStatus.ToPresentationString(_kTestStatus);

         if (_kTestStatus == KTestStatus.OK || _kTestStatus == KTestStatus.Failed || _kTestStatus == KTestStatus.Terminated)
         {
            _labelTestInformation.Text += "       " + _nTests.ToString() + "  Documents" +
               (_nTestsError > 0 ? "     " + _nTestsError.ToString() + "  Errors" : "");
         };

         _btnRunTests.Enabled = !_fRunningTask;
         _btnTestLog.Enabled = !_fRunningTask;


         _btnStop.Enabled = _fRunningTask && (_kRunningTaskCur != KRunningTask.TestList);

         /* ---------------------------------------------- */
         /* Enable / Disable other text labels					  */
         /* ---------------------------------------------- */
         //label3.Enabled = !fRunningTask;
         //label5.Enabled = !fRunningTask;
         //label7.Enabled = !fRunningTask;

         /* Restore possibility to have dialog data exchange */
         _fAllowDialogDataExchange = fAllowDialogDataExchange_Saved;
      }

      // This method is called each time there is end-user change
      // anywhere on any of the dialog box controls. We update
      // status and redisplay controls
      private void EdiitingChange(object sender, System.EventArgs e)
      {
         if (_fAllowDialogDataExchange)
         {
            //Test Options
            
            _location.options.fRTRtf = _chkRTRTF.Checked;
            _location.options.fRTXaml = _chkRTXaml.Checked;
            _location.options.fRtfXaml = _chkRTFXAML.Checked;
            _location.options.fREAssert = _chkREAssert.Checked;
            _location.options.fWordAssert= _chkWordAssert.Checked;//not implemented yet
            //RTFXamlView behavior options
            _location.options.fUseXCVT = _chkUseXCVT.Checked;
            _location.options.fPri0 = _chkPri0.Checked;            
            //Retart rtfXamlView after each doc or run in batches of 12
            
            _location.options.fRestartDocs = _chkRestartEveryDoc.Checked;
            /* Update controls to reflect new status change */
            UpdateControlsAfterStatusChange();
         };

      }

      // User clicks on View Cmd button
      private void BtnTestLog_Click(object sender, System.EventArgs e)
      {
         //try
         //{
         //   CBuild.ViewCommandFile(Location, kTarget, rgstrConfigurationName[iConfig],
         //                      GetFCleanWal(), GetFCleanWord(), GetFFullLink());
         //}
         //catch (W11Exception) { }
         //catch (Exception ex) { W11Messages.ShowExceptionError(ex); };
      }

      // User clicks on Cancel button
      private void buttonCancel_Click(object sender, System.EventArgs e)
      {
         if (_fRunningTask)
         {
            W11Messages.ShowWarning("Can not exit while Word is still open.\n\nExit Word or use STOP button to terminate it");
         }
         else
         {
            Close();
         }
      }

      // User clicks on RunTests button
      private void BtnRunTests_Click(object sender, System.EventArgs e)
      {
         //run all of the tests and post status messages.
         FRunTestsCore(false);
      }

      // Set & Get methods for booleans CleanWAL/CleanWord/FullLink

      //void SetFCleanWal(bool fCleanWal) { fCleanWal_Calc = fCleanWal; }
      //void SetFCleanWord(bool fCleanWord) { fCleanWord_Calc = fCleanWord; }
      //void SetFFullLink(bool fFullLink) { fFullLink_Calc = fFullLink; }

      //bool GetFBuildDifferentConfiguration()
      //{
      //   return rgBuildInfo[kTarget].Success != KBuildSuccess.NotAvailable &&
      //         Common.CompareStr(rgBuildInfo[kTarget].StrConfigName, rgstrConfigurationName[iConfig]) != 0;
      //}

      //bool GetFCleanWal() { return fCleanWal_Calc || GetFCleanWord(); }
      //bool GetFCleanWalChangeable() { return !GetFCleanWord(); }

      //bool GetFFullLink() { return fFullLink_Calc || GetFCleanWord() || GetFBuildDifferentConfiguration(); }
      //bool GetFFullLinkChangeable() { return !GetFCleanWord() && !GetFBuildDifferentConfiguration(); }

      //bool GetFCleanWord() { return fCleanWord_Calc; }
      //bool GetFCleanWordChangeable() { return true; }

      //void SetFFuzzyDupCompare(bool fFuzzyDupCompare) { fFuzzyDupCompare_Calc = fFuzzyDupCompare; }
      //bool GetFFuzzyDupCompare() { return kLayoutOut == KLayoutOut.Compare && fFuzzyDupCompare_Calc; }
      //bool GetFFuzzyDupCompareChangeable() { return kLayoutOut == KLayoutOut.Compare; }

      // Save INI file
      void SaveConfigurationFile()
      {
         try
         {
            Configuration.SaveOptions(_location.options);
         }
         catch (W11Exception) { }

      }

      // User clicks on Build button
      //private void buttonBuild_Click(object sender, System.EventArgs e)
      //{
      //   FStartBuild();
      //}

      // FStartBuild - core function to start build
      //bool FStartBuild()
      //{
      //   bool fStarted = false;

      //   Common.Assert(!fRunningTask);

      //   fRunningTask = true;
      //   kRunningTaskCur = KRunningTask.Build;

      //   try
      //   {
      //      /* Get booleans before changing configuration */
      //      bool fCleanWal = GetFCleanWal();
      //      bool fCleanWord = GetFCleanWord();
      //      bool fFullLink = GetFFullLink();

      //      Protocol.Write("Starting build, " + rgstrConfigurationName[iConfig] + ", " + KTarget.ToDebugShipString(kTarget));

      //      /* Intiialize build info before build */
      //      rgBuildInfo[kTarget].FInCurrentSession = true;
      //      rgBuildInfo[kTarget].Success = KBuildSuccess.Running;
      //      rgBuildInfo[kTarget].StrConfigName = rgstrConfigurationName[iConfig];
      //      rgBuildInfo[kTarget].DTime = DateTime.Now;

      //      /* Show infinished build status on the screen */
      //      UpdateControlsAfterStatusChange();

      //      /* Start async build process */
      //      buildObject.StartBuild(kTarget,
      //                         rgstrConfigurationName[iConfig],
      //                         fCleanWal,
      //                         fCleanWord,
      //                         fFullLink);

      //      fStarted = true;

      //   }
      //   catch (W11Exception) { }
      //   catch (Exception ex) { W11Messages.ShowExceptionError(ex); };

      //   return fStarted;
      //}

      // User clicks on Run Word button
      private void buttonRunWord_Click(object sender, System.EventArgs e)
      {
         //bool fSuccessful = false;

         //Common.Assert(!fRunningTask);

         //fRunningTask = true;
         //kRunningTaskCur = KRunningTask.Word;

         //UpdateControlsAfterStatusChange();
         //Protocol.Write("Starting Word");


         //try
         //{
         //   bool fNewProcess = runwordObject.FStartWordOpenDoc(delegateFinishedTask, kTarget, "", "", false);

         //   Common.Assert(fNewProcess);

         //   fSuccessful = true;
         //}
         //catch (W11Exception) { }
         //catch (Exception ex) { W11Messages.ShowExceptionError(ex); };

         //if (!fSuccessful)
         //{
         //   fRunningTask = false;
         //   UpdateControlsAfterStatusChange();
         //   //Protocol.Write("Could not start Word");
         //}

      }

      // User clicks on RunTests button
      private bool FRunTestsCore(bool fTestObjectPreparedForTests)
      {
         bool fStarted = false;

         Common.Assert(!_fRunningTask);

         try
         {
            if (fTestObjectPreparedForTests || _testlistObject.FPrepareForTests(KTest.All/*, false*/))
            {
               _fRunningTask = true;
               _kRunningTaskCur = KRunningTask.Tests;//we only have one type of task, ie run selected BVT tests

               // Test status: running
               _kTestStatus = KTestStatus.Running;

               // Initialize progress bar min/max (percentage)
               _progressBarTest.Minimum = 0;
               _progressBarTest.Maximum = 100;
               _progressBarTest.Value = 0;
               _progressBarTest.Visible = true;

               _labelTestFolder.Text = "";
               _labelTestFolder.Visible = true;

               // Update controls
               UpdateControlsAfterStatusChange();

               //Build array of test types to run
               int cRTFTests = 0;
               if (_location.options.fRtfXaml) { cRTFTests++; }
               if (_location.options.fRTRtf) { cRTFTests++; }
               if (_location.options.fRTXaml) { cRTFTests++; }
               if (_location.options.fREAssert) { cRTFTests++; }
               rxCommands[] rgRTFTests = new rxCommands[cRTFTests];
               cRTFTests = 0;
               if (_location.options.fRtfXaml) 
               {
                  rgRTFTests[cRTFTests] = rxCommands.rxRTRTFXAML;
                  cRTFTests++; 
               }
               if (_location.options.fRTRtf) 
               {
                  rgRTFTests[cRTFTests] = rxCommands.rxRTRtf;
                  cRTFTests++; 
               }
               if (_location.options.fRTXaml) 
               {
                  rgRTFTests[cRTFTests] = rxCommands.rxRTXaml;
                  cRTFTests++; 
               }
               if (_location.options.fREAssert) 
               {
                  rgRTFTests[cRTFTests] = rxCommands.rxRTREAssert;
                  cRTFTests++; 
               }


               // Start test run (in will create new thread and return)
               //runs each selected test as a seperate run
               _testsObject.StartTests(rgRTFTests);

               fStarted = true;
            };
         }
         catch (W11Exception) { }
         catch (Exception ex)
         {
            W11Messages.ShowExceptionError(ex);
            _kTestStatus = KTestStatus.Failed;
         }

         return fStarted;
      }


      // Notification that current running task is finished
      // This method is called through delegate from the running thread
      private void FinishedTaskNotification()
      {
         Common.Assert(_fRunningTask);

         _fRunningTask = false;
         _nStopCounter = 0;

         if (_kRunningTaskCur == KRunningTask.Tests)
         {
            /* Finished tests */
            _kTestStatus = _testsObject.GetLastTestStatus(out _nTests, out _nTestsError);

            _btnRunTests.Enabled = true;
            _btnRunTests.Focus();

            _labelTestFolder.Visible = false;

            _labelTestErrorWarning.Visible = false;
            _labelTestProgressInfo.Visible = false;
            _labelNewOldErrors.Visible = false;
            _progressBarTest.Visible = false;
         }
         else if (_kRunningTaskCur == KRunningTask.TestList)
         {
            /* Finished test list */
            if (_testlistObject.GetFRunTestsAfterClosing())
            {
               FRunTestsCore(true);
            }
         }

         else
         {
            /* Wrong kind of task */
            Common.Assert(false);
         }

         // Need to enable controls that were disabled during run and update status
         UpdateControlsAfterStatusChange();
      }

      // Notification about progress of the test
      private void TestProgressNotification(int ndocsCompleted, int ndocsTotal, int nErrors,
                                    int nErrorsFixed, int nErrorsNew,
                                     string spathTestFolder, bool fTestETA,
                                    DateTime dtTestStarted,
                                    DateTime dtTestETA)
      {
         int nDaysToETA;
         Common.Assert(ndocsCompleted <= ndocsTotal, "Number of completed documents can not exceed total number of documents");

         _progressBarTest.Value = 100 * ndocsCompleted / ndocsTotal;
         _progressBarTest.Update();

         _labelTestFolder.Text = spathTestFolder;

         if (nErrors > 0)
         {
            if (!_labelTestErrorWarning.Visible)
            {
               /* First time error: enable error text and beep */

               _labelTestErrorWarning.Visible = true;
               Beep(1000, 1000);
            };

            _labelTestErrorWarning.Text = "ERR = " + nErrors.ToString();
            _labelTestErrorWarning.Update();
         };

         _labelTestProgressInfo.Visible = true;
         _labelTestProgressInfo.Text = ndocsCompleted.ToString() + "  of  " + ndocsTotal.ToString();
         _labelTestProgressInfo.Update();

         if (nErrorsFixed != 0 || nErrorsNew != 0)
         {
            _labelNewOldErrors.Visible = true;
            _labelNewOldErrors.Text = "+ " + nErrorsNew.ToString() + " - " + nErrorsFixed.ToString();
            _labelTestProgressInfo.Update();
         };

         /* Calculate ETA */

         //if (fTestETA)
         //{
         //   /* Round up to the nearest minute */
         //   if (dtTestETA.Second > 30)
         //   {
         //      dtTestETA = dtTestETA.Add(new TimeSpan(0, 0, 1, 0, 0));
         //   };

         //   nDaysToETA = (int)dtTestETA.Subtract(DateTime.Now).TotalDays;
         //   labelTestETA.Visible = true;
         //   labelTestETA.Text = "ETA = " + dtTestETA.ToShortTimeString().ToLower() +
         //      (nDaysToETA == 0 ? "" : "+" + nDaysToETA.ToString() + "d");

         //   labelTestSta.Visible = true;
         //   labelTestSta.Text = "Started " + dtTestStarted.ToShortTimeString().ToLower();
         //}
         //else
         //{
         //   labelTestETA.Visible = false;
         //};
      }


      // User clicks on stop button
      private void buttonStop_Click(object sender, System.EventArgs e)
      {
         Common.Assert(_fRunningTask);

         _nStopCounter++;
         UpdateControlsAfterStatusChange();

         if (_kRunningTaskCur == KRunningTask.Tests)
         {
            /* Killing tests */
            _testsObject.KillTests();
         }
         else
         {
            /* Wrong kind of task */
            Common.Assert(false);
         }
      }

      // User clicks on test log
      //private void ButtonTestLog_Click(object sender, System.EventArgs e)
      //{
      //   Common.Assert(!fRunningTask);

      //   try
      //   {

      //      testlistObject.Run();

      //      kRunningTaskCur = KRunningTask.TestList;
      //      fRunningTask = true;

      //      CTests.ViewTestList(Location);
      //   }
      //   catch (W11Exception) { }
      //   catch (Exception ex) { W11Messages.ShowExceptionError(ex); };

      //   UpdateControlsAfterStatusChange();
      //}


      // Main form is closing. Must save configuration file
      private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         if (_fRunningTask)
         {
            if (this._kRunningTaskCur == KRunningTask.TestList)
            {
               _testlistObject.Terminate();
            }
            else
            {
               W11Messages.ShowWarning("Still running " + KRunningTask.ToPresentationString(_kRunningTaskCur) +
                  "\n\nThe task will be shut down");
               buttonStop_Click(sender, e);
            };
         };

         SaveConfigurationFile();
      }


      // File :: Close
      private void menuItem7_Click(object sender, System.EventArgs e)
      {
         Close();
      }

      // Help :: About
      private void menuItem8_Click(object sender, System.EventArgs e)
      {
         MessageBox.Show(this, "WCLTest - Word Compatibility Tool, Version 4.0\n\n" +
                           "Copyright (c) Microsoft Corporation\n\n" +
                           "Developed by Anton Sukhanov for PTS and LineService team",
                           "About");
      }

      // Help :: Example of WCLTest ini
      private void menuItem1_Click(object sender, System.EventArgs e)
      {
         //CTests.ViewExampleTestIni(Location);
      }


      // File:: Options
      private void menuItem2_Click(object sender, System.EventArgs e)
      {
         SOptions optionsNew;

         if (Configuration.FChangeOptionsDialog(_location.options, out optionsNew)
            && !SOptions.FEqual(_location.options, optionsNew))
         {
            _location.ChangeOptions(optionsNew);
            InitializeMainDialog(false);
         }
      }

      // Test :: Word 11 suite 
      private void menuItem9_Click(object sender, System.EventArgs e)
      {
         //StartMacro11();
      }

      // Test :: Word 12 suite
      private void menuItem11_Click(object sender, System.EventArgs e)
      {
         //StartMacro12();
      }

      //Deal with output folder names
      // User clicks on the test folder button
      private void linkBCTDir_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
      {
         if (_fRunningTask && _kRunningTaskCur == KRunningTask.TestList)
         {
            W11Messages.ShowWarning("You shoud close Test List dialog before chaning test folder");
         }
         else
         {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            folderDialog.SelectedPath = _location.GetBVTDir();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
               if (Common.CompareStr(_location.GetBVTDir(), folderDialog.SelectedPath) != 0)
               {
                  _location.options.szBVTDir = folderDialog.SelectedPath; // 
                  _testlistObject.ChangeFolder();
                  UpdateControlsAfterStatusChange();
               }
            }
         }

         _btnRunTests.Focus();
      }

      //user clicked LogFile link
      private void linkLogFile_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
      {
         //Location.StrOutputFolderName = ChooseLayoutFolderCore(Location.PathTestFolder, Location.StrOutputFolderName);
         //UpdateControlsAfterStatusChange();
         //ButtonRunTests.Focus();
      }

      //user clicked logdir link
      private void linkLogDir_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
      {
         //Location.StrCompareFolderName = ChooseLayoutFolderCore(Location.PathTestFolder,
         //                                          Location.StrCompareFolderName);
         //add in call to choose log dir 
         UpdateControlsAfterStatusChange();
         _btnRunTests.Focus();
      }
   }
}