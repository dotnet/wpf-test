// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace Configuration
{
   using Common;
   using Location;

   class COptionsDlg : System.Windows.Forms.Form
   {
      #region class variables
      // Dialog variables
      public bool FSetOptions; // If end user requested to change options
      public SOptions Options;
      bool _fAllowDataExchange;
      //Designer variables
      private System.Windows.Forms.Button _buttonSave;
      private System.Windows.Forms.Label _labelMessage;
      private System.Windows.Forms.GroupBox _groupBoxSetup;
      private System.Windows.Forms.Button _btnGetRTFXamlDir;
      private System.Windows.Forms.Button _btnGetWordDir;
      private System.Windows.Forms.TextBox _tbRTFXamlDir;
      private System.Windows.Forms.TextBox _tbWordDir;
      private System.Windows.Forms.Label _label1;
      private System.Windows.Forms.Label _label2;
      private System.Windows.Forms.Panel _panel1;
      private Label _label3;
      private Label _label4;
      private Label _label6;
      private TextBox _tbLogDir;
      private Button _btnGetLogDir;
      private Label _label5;
      private TextBox _tbBVTDir;
      private Button _btnGetBVTDir;
      private TextBox _tbLogFileName;
      private Label _label7;
      private System.ComponentModel.Container _components = null;
      #endregion//class variables

      public COptionsDlg(bool fMustSet, string strMessage, SOptions optionsCur)
      {
         _fAllowDataExchange = false; ;

         // Required for Windows Form Designer support
         InitializeComponent();

         FSetOptions = false;
         Options = optionsCur;

         _labelMessage.Text = strMessage;

         if (fMustSet)
         {
            _labelMessage.ForeColor = Color.IndianRed;
         }

         _fAllowDataExchange = true;
         UpdateDialogAfterStatusChange();
      }

      //Comments: Updates the config dialog with path info.
      void UpdateDialogAfterStatusChange()
      {
         bool fAllowDataExchage_saved = _fAllowDataExchange;
         _fAllowDataExchange = false;

         if (_tbRTFXamlDir.Text != Options.szRTFXamlDir) _tbRTFXamlDir.Text = Options.szRTFXamlDir;
         if (_tbWordDir.Text != Options.szWordDir) _tbWordDir.Text = Options.szWordDir;
         if (_tbBVTDir.Text != Options.szBVTDir) _tbBVTDir.Text = Options.szBVTDir;
         if (_tbLogDir.Text != Options.szLogDir) _tbLogDir.Text = Options.szLogDir;
         if (_tbLogFileName.Text != Options.szLogFile) _tbLogFileName.Text = Options.szLogFile;

         _fAllowDataExchange = fAllowDataExchage_saved;
      }

      /*		void EditingChange (object sender, System.EventArgs e)
            {
               if (fAllowDataExchange)
               {
                  if (radioButtonWord11.Enabled) Options.kword = (radioButtonWord11.Checked ? KWord.w11 : KWord.w12);
                  if (radioButtonInstallOnly.Enabled) Options.fInstallOnly = radioButtonInstallOnly.Checked;
                  if (tbRTFXamlDir.Enabled) Options.spathOffice = tbRTFXamlDir.Text;
                  if (tbWordDir.Enabled) Options.spathWordExeInstall = tbWordDir.Text;
                  if (checkBoxUseDefaultTestFolder.Enabled) Options.fUseDefaultTestFolder = checkBoxUseDefaultTestFolder.Checked;
                  if (checkBoxKillWordIfDW20.Enabled) Options.fKillWordIfDw20Present = checkBoxKillWordIfDW20.Checked;
                  if (textBoxDefaultTestFolder.Enabled) Options.spathDefaultTestFolder = textBoxDefaultTestFolder.Text;

                  UpdateDialogAfterStatusChange ();
               };
            }
      */

      private void btnGetWordDir_Click(object sender, System.EventArgs e)
      {
         FolderBrowserDialog folderDialog = new FolderBrowserDialog();

         if (Options.szWordDir == "") folderDialog.SelectedPath = "";
         else folderDialog.SelectedPath = Options.szWordDir;

         if (folderDialog.ShowDialog() == DialogResult.OK)
         {
            Options.szWordDir = folderDialog.SelectedPath;
            UpdateDialogAfterStatusChange();
         }
      }

      private void btnGetRTFXamlDir_Click(object sender, System.EventArgs e)
      {
         FolderBrowserDialog folderDialog = new FolderBrowserDialog();

         if (Options.szRTFXamlDir == "") folderDialog.SelectedPath = "";
         else folderDialog.SelectedPath = Options.szRTFXamlDir;

         if (folderDialog.ShowDialog() == DialogResult.OK)
         {
            Options.szRTFXamlDir = folderDialog.SelectedPath;
            UpdateDialogAfterStatusChange();
         }
      }

      private void btnGetBVTDir_Click(object sender, EventArgs e)
      {
         FolderBrowserDialog folderDialog = new FolderBrowserDialog();

         if (Options.szBVTDir == "") folderDialog.SelectedPath = "";
         else folderDialog.SelectedPath = Options.szBVTDir;

         if (folderDialog.ShowDialog() == DialogResult.OK)
         {
            Options.szBVTDir = folderDialog.SelectedPath;
            UpdateDialogAfterStatusChange();
         }
      }

      private void btnGetLogDir_Click(object sender, EventArgs e)
      {
         FolderBrowserDialog folderDialog = new FolderBrowserDialog();

         if (Options.szLogDir == "") folderDialog.SelectedPath = "";
         else folderDialog.SelectedPath = Options.szLogDir;

         if (folderDialog.ShowDialog() == DialogResult.OK)
         {
            Options.szLogDir = folderDialog.SelectedPath;
            UpdateDialogAfterStatusChange();
         }
      }

      private void buttonSave_Click(object sender, System.EventArgs e)
      {
         string strErrorMessage;

         if (Configuration.FCheckOptions(this.Options, out strErrorMessage))
         {
            FSetOptions = true;
            Close();
         }
         else
         {
            W11Messages.ShowWarning(strErrorMessage);
         };
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
         this._buttonSave = new System.Windows.Forms.Button();
         this._labelMessage = new System.Windows.Forms.Label();
         this._btnGetRTFXamlDir = new System.Windows.Forms.Button();
         this._btnGetWordDir = new System.Windows.Forms.Button();
         this._tbRTFXamlDir = new System.Windows.Forms.TextBox();
         this._tbWordDir = new System.Windows.Forms.TextBox();
         this._groupBoxSetup = new System.Windows.Forms.GroupBox();
         this._label6 = new System.Windows.Forms.Label();
         this._tbLogDir = new System.Windows.Forms.TextBox();
         this._btnGetLogDir = new System.Windows.Forms.Button();
         this._label5 = new System.Windows.Forms.Label();
         this._tbBVTDir = new System.Windows.Forms.TextBox();
         this._btnGetBVTDir = new System.Windows.Forms.Button();
         this._label4 = new System.Windows.Forms.Label();
         this._label3 = new System.Windows.Forms.Label();
         this._label2 = new System.Windows.Forms.Label();
         this._label1 = new System.Windows.Forms.Label();
         this._panel1 = new System.Windows.Forms.Panel();
         this._label7 = new System.Windows.Forms.Label();
         this._tbLogFileName = new System.Windows.Forms.TextBox();
         this._groupBoxSetup.SuspendLayout();
         this._panel1.SuspendLayout();
         this.SuspendLayout();
         
         // buttonSave
         this._buttonSave.Location = new System.Drawing.Point(344, 340);
         this._buttonSave.Name = "buttonSave";
         this._buttonSave.Size = new System.Drawing.Size(88, 28);
         this._buttonSave.TabIndex = 1;
         this._buttonSave.Text = "Save";
         this._buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
 
         // labelMessage
         this._labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._labelMessage.Location = new System.Drawing.Point(8, 9);
         this._labelMessage.Name = "labelMessage";
         this._labelMessage.Size = new System.Drawing.Size(411, 17);
         this._labelMessage.TabIndex = 3;
         this._labelMessage.Text = "Message";
         this._labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

         // btnGetRTFXamlDir
         this._btnGetRTFXamlDir.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._btnGetRTFXamlDir.Location = new System.Drawing.Point(112, 82);
         this._btnGetRTFXamlDir.Name = "btnGetRTFXamlDir";
         this._btnGetRTFXamlDir.Size = new System.Drawing.Size(26, 20);
         this._btnGetRTFXamlDir.TabIndex = 8;
         this._btnGetRTFXamlDir.Text = "...";
         this._btnGetRTFXamlDir.Click += new System.EventHandler(this.btnGetRTFXamlDir_Click);

         // btnGetWordDir
         this._btnGetWordDir.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._btnGetWordDir.Location = new System.Drawing.Point(112, 22);
         this._btnGetWordDir.Name = "btnGetWordDir";
         this._btnGetWordDir.Size = new System.Drawing.Size(26, 20);
         this._btnGetWordDir.TabIndex = 9;
         this._btnGetWordDir.Text = "...";
         this._btnGetWordDir.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         this._btnGetWordDir.Click += new System.EventHandler(this.btnGetWordDir_Click);

         // tbRTFXamlDir
         this._tbRTFXamlDir.Location = new System.Drawing.Point(152, 82);
         this._tbRTFXamlDir.Name = "tbRTFXamlDir";
         this._tbRTFXamlDir.Size = new System.Drawing.Size(256, 20);
         this._tbRTFXamlDir.TabIndex = 10;

         // tbWordDir
         this._tbWordDir.Location = new System.Drawing.Point(152, 22);
         this._tbWordDir.Name = "tbWordDir";
         this._tbWordDir.Size = new System.Drawing.Size(256, 20);
         this._tbWordDir.TabIndex = 0;

         // groupBoxSetup
         this._groupBoxSetup.Controls.Add(this._tbLogFileName);
         this._groupBoxSetup.Controls.Add(this._label7);
         this._groupBoxSetup.Controls.Add(this._label6);
         this._groupBoxSetup.Controls.Add(this._tbLogDir);
         this._groupBoxSetup.Controls.Add(this._btnGetLogDir);
         this._groupBoxSetup.Controls.Add(this._label5);
         this._groupBoxSetup.Controls.Add(this._tbBVTDir);
         this._groupBoxSetup.Controls.Add(this._btnGetBVTDir);
         this._groupBoxSetup.Controls.Add(this._label4);
         this._groupBoxSetup.Controls.Add(this._label3);
         this._groupBoxSetup.Controls.Add(this._label2);
         this._groupBoxSetup.Controls.Add(this._label1);
         this._groupBoxSetup.Controls.Add(this._tbWordDir);
         this._groupBoxSetup.Controls.Add(this._tbRTFXamlDir);
         this._groupBoxSetup.Controls.Add(this._btnGetWordDir);
         this._groupBoxSetup.Controls.Add(this._btnGetRTFXamlDir);
         this._groupBoxSetup.Location = new System.Drawing.Point(11, 38);
         this._groupBoxSetup.Name = "groupBoxSetup";
         this._groupBoxSetup.Size = new System.Drawing.Size(421, 296);
         this._groupBoxSetup.TabIndex = 15;
         this._groupBoxSetup.TabStop = false;
         this._groupBoxSetup.Text = "Locations";

         // label6
         this._label6.AutoSize = true;
         this._label6.Location = new System.Drawing.Point(7, 192);
         this._label6.Name = "label6";
         this._label6.Size = new System.Drawing.Size(69, 13);
         this._label6.TabIndex = 25;
         this._label6.Text = "log file folder:";

         // tbLogDir
         this._tbLogDir.Location = new System.Drawing.Point(153, 185);
         this._tbLogDir.Name = "tbLogDir";
         this._tbLogDir.Size = new System.Drawing.Size(256, 20);
         this._tbLogDir.TabIndex = 24;

         // btnGetLogDir
         this._btnGetLogDir.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._btnGetLogDir.Location = new System.Drawing.Point(113, 185);
         this._btnGetLogDir.Name = "btnGetLogDir";
         this._btnGetLogDir.Size = new System.Drawing.Size(26, 20);
         this._btnGetLogDir.TabIndex = 23;
         this._btnGetLogDir.Text = "...";
         this._btnGetLogDir.Click += new System.EventHandler(this.btnGetLogDir_Click);

         // label5
         this._label5.AutoSize = true;
         this._label5.Location = new System.Drawing.Point(6, 147);
         this._label5.Name = "label5";
         this._label5.Size = new System.Drawing.Size(100, 13);
         this._label5.TabIndex = 22;
         this._label5.Text = "BVT Test file folder:";

         // tbBVTDir
         this._tbBVTDir.Location = new System.Drawing.Point(152, 140);
         this._tbBVTDir.Name = "tbBVTDir";
         this._tbBVTDir.Size = new System.Drawing.Size(256, 20);
         this._tbBVTDir.TabIndex = 21;

         // btnGetBVTDir
         this._btnGetBVTDir.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._btnGetBVTDir.Location = new System.Drawing.Point(112, 140);
         this._btnGetBVTDir.Name = "btnGetBVTDir";
         this._btnGetBVTDir.Size = new System.Drawing.Size(26, 20);
         this._btnGetBVTDir.TabIndex = 20;
         this._btnGetBVTDir.Text = "...";
         this._btnGetBVTDir.Click += new System.EventHandler(this.btnGetBVTDir_Click);

         // label4
         this._label4.AutoSize = true;
         this._label4.Location = new System.Drawing.Point(6, 89);
         this._label4.Name = "label4";
         this._label4.Size = new System.Drawing.Size(70, 13);
         this._label4.TabIndex = 19;
         this._label4.Text = "RtfXamlView:";

         // label3
         this._label3.AutoSize = true;
         this._label3.Location = new System.Drawing.Point(6, 29);
         this._label3.Name = "label3";
         this._label3.Size = new System.Drawing.Size(36, 13);
         this._label3.TabIndex = 18;
         this._label3.Text = "Word:";

         // label2
         this._label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._label2.ForeColor = System.Drawing.SystemColors.GrayText;
         this._label2.Location = new System.Drawing.Point(150, 106);
         this._label2.Name = "label2";
         this._label2.Size = new System.Drawing.Size(258, 31);
         this._label2.TabIndex = 12;
         this._label2.Text = "Location of rtfXamlView App. Default is BVT Test Directory.";

         // label1
         this._label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._label1.ForeColor = System.Drawing.SystemColors.GrayText;
         this._label1.Location = new System.Drawing.Point(151, 45);
         this._label1.Name = "label1";
         this._label1.Size = new System.Drawing.Size(258, 27);
         this._label1.TabIndex = 11;
         this._label1.Text = "Office binary folder, containing winword exe. For example C:\\Program Files\\Micros" +
             "oft Office\\OFFICE11";

         // panel1
         this._panel1.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
         this._panel1.Controls.Add(this._labelMessage);
         this._panel1.Location = new System.Drawing.Point(0, 0);
         this._panel1.Name = "panel1";
         this._panel1.Size = new System.Drawing.Size(432, 32);
         this._panel1.TabIndex = 17;

         // label7
         this._label7.AutoSize = true;
         this._label7.Location = new System.Drawing.Point(7, 242);
         this._label7.Name = "label7";
         this._label7.Size = new System.Drawing.Size(78, 13);
         this._label7.TabIndex = 26;
         this._label7.Text = "Log File Name:";

         // tbLogFileName
         this._tbLogFileName.Location = new System.Drawing.Point(152, 237);
         this._tbLogFileName.Name = "tbLogFileName";
         this._tbLogFileName.Size = new System.Drawing.Size(256, 20);
         this._tbLogFileName.TabIndex = 27;

         // COptionsDlg
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(442, 380);
         this.Controls.Add(this._panel1);
         this.Controls.Add(this._groupBoxSetup);
         this.Controls.Add(this._buttonSave);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "COptionsDlg";
         this.Text = "BVT Test Options";
         this._groupBoxSetup.ResumeLayout(false);
         this._groupBoxSetup.PerformLayout();
         this._panel1.ResumeLayout(false);
         this.ResumeLayout(false);
      }
      #endregion
   }


   /************************
   *						 
   * Class Configuration   
   * used registry for storing these values
   *  so in the interest of speed I'll stick to that but 
   * I'd rather move it to an ini file and will when time permits.
   *************************/
   class Configuration
   {

      public static bool FCheckOptions(SOptions options, out string strError)
      {
         bool fSuccessful = true;
         strError = "";

         //Verify that these folders exist
         if ((options.szBVTDir != null) && !Directory.Exists(options.szBVTDir))
         {
            strError += "Can not find the BVT folder " + options.szBVTDir + "\n";
            fSuccessful = false;
         }

         if ((options.szLogDir != null) && !Directory.Exists(options.szLogDir))
         {
            strError += "Can not find the log folder " + options.szLogDir + "\n";
            fSuccessful = false;
         };

         if ((options.szWordDir != null) && !Directory.Exists(options.szWordDir))
         {
            strError += "Can not find the Word folder " + options.szWordDir + "\n";
            fSuccessful = false;
         };

         if ((options.szRTFXamlDir != null) && !Directory.Exists(options.szRTFXamlDir))
         {
            strError += "Can not find the RTFXAMLView folder " + options.szRTFXamlDir + "\n";
            fSuccessful = false;
         };
         //if ((options.szLogFile != null)&& !File.Exists(options.szLogFile))
         //{
         //   strError += "Can not find the log file " + options.szLogFile + "\n";
         //   fSuccessful = false;
         //}
         return fSuccessful;
      }


      // Dialog to change application settings
      public static bool FChangeOptionsDialog(SOptions optionsOld, out SOptions optionsNew)
      {
         return FChangeOptionsDialogCore(false, "Please select options", optionsOld, out optionsNew);
      }

      // Dialog to change application settings -- core function
      static bool FChangeOptionsDialogCore(bool fMustSave, string strMessage, SOptions optionsCur,
                                    out SOptions optionsNew)
      {
         COptionsDlg dlg = new COptionsDlg(fMustSave, strMessage, optionsCur);

         dlg.ShowDialog();

         if (dlg.FSetOptions)
         {
            optionsNew = dlg.Options;
            SaveOptions(optionsNew);
            return true;
         }
         else
         {
            optionsNew = optionsCur;
            return false;
         }
      }

      // Check if first time run or something else that requires
      // to start app with options dialog
      public static void GetOptionsOnLoad(out SOptions options, out bool fExit)
      {
         bool fSuccessful;
         bool fFirstTime;
         bool fShowDialog = false;
         string msg = "";

         fSuccessful = FGetOptionsCore(out fFirstTime, out options);

         if (GetFOptionsDialogAfterRestart())
         {
            fShowDialog = true;
            msg = "Application failed during last run, please confirm options";
         }
         else if (fFirstTime)
         {
            fShowDialog = true;
            msg = "You are running application for the first time. Please set options";
         }
         else if (!fSuccessful)
         {
            fShowDialog = true;
            msg = "There was a change in option settings. Please confirm new options";
         };

         if (fShowDialog)
         {
            fExit = !FChangeOptionsDialogCore(true /* fMustSave */, msg, options, out options);
         }
         else fExit = false;
      }


      // Save current configuration in the registry
      //public static void SaveConfiguration (string spathTestFolder, string strConfigName)
      //{
      //   RegistryKey regwcltest = GetWCLTestRegistry ();

      //   Common.Assert (regwcltest != null);

      //   regwcltest.SetValue ("Configuration", strConfigName);
      //   regwcltest.SetValue ("CurrentTestFolder", spathTestFolder);

      //   regwcltest.Close ();
      //}

      // Save current configuration in the registry
      public static void SaveOptions(SOptions options)
      {
         RegistryKey regwcltest = GetWCLTestRegistry();

         Common.Assert(regwcltest != null);
         
         //paths         
         regwcltest.SetValue("WordDir", options.szWordDir);
         regwcltest.SetValue("RTFXAMLDir", options.szRTFXamlDir);
         regwcltest.SetValue("BVTDir", options.szBVTDir);
         regwcltest.SetValue("LogDir", options.szLogDir);
         regwcltest.SetValue("LogFile", options.szLogFile);
         
         //BVT test flags         
         regwcltest.SetValue("RTRtf", options.fRTRtf);
         regwcltest.SetValue("RTXaml", options.fRTXaml);
         regwcltest.SetValue("RtfXaml", options.fRtfXaml);
         regwcltest.SetValue("REAssert", options.fREAssert);
         regwcltest.SetValue("WordAssert", options.fWordAssert);
         
         //misc options         
         regwcltest.SetValue("RestartDocs", options.fRestartDocs);//Restart App for each test.
         regwcltest.SetValue("NoAssert", options.fNoAssert);      //not currently implemented.
         regwcltest.SetValue("UseXCVT", options.fUseXCVT);        //Send Text directly to the converted or use copy paste
         regwcltest.SetValue("Pri0", options.fPri0);              //log only pri0 fails

         regwcltest.Close();
      }

      // Read options from the registry -- core function
      // Output options are set to defaults in case of errors or first time load.
      static bool FGetOptionsCore(out bool fFirstTime, out SOptions options)
      {
         bool fSuccess = true;
         RegistryKey regwcltest = GetWCLTestRegistry();
         object help;

         /* Set defaults first */
         //misc app settings
         options.fUseXCVT = true;
         options.fNoAssert = false;//not used right now
         options.fPri0 = true;
         options.fRestartDocs = false;
         //tests
         options.fREAssert = false;
         options.fRtfXaml = true;
         options.fRTRtf = false;
         options.fRTXaml = false;
         options.fWordAssert = false;//not implemented in rtfxamlview yet
         options.szBVTDir = "c:\\";
         options.szLogDir = "c:\\";
         options.szLogFile = "Results.log";
         options.szRTFXamlDir = "";
         options.szWordDir = "";
         options.szSumaryFile = "";


         //Get paths         
         help = regwcltest.GetValue("WordDir");
         if (help == null)
         {
            fSuccess = false;
            fFirstTime = true;
         }
         else
         {
            fFirstTime = false;
            options.szWordDir = (string)help;
         }

         help = regwcltest.GetValue("RTFXAMLDir");
         if (help == null) fSuccess = false;
         else
         {
            options.szRTFXamlDir = (string)help;
         }

         help = regwcltest.GetValue("BVTDir");
         if (help == null) fSuccess = false;
         else
         {
            options.szBVTDir = (string)help;
         }

         help = regwcltest.GetValue("LogDir");
         if (help == null) fSuccess = false;
         else
         {
            options.szLogDir = (string)help;
         }

         help = regwcltest.GetValue("LogFile");
         if (help == null) fSuccess = false;
         else
         {
            options.szLogFile = (string)help;
         }

         
         //bvt test flags         
         help = regwcltest.GetValue("RTRtf");
         if (help == null) fSuccess = false;
         else
         {
            options.fRTRtf = Common.CompareStr((string)help, "True") == 0;
         }

         help = regwcltest.GetValue("RTXaml");
         if (help == null) fSuccess = false;
         else
         {
            options.fRTXaml = Common.CompareStr((string)help, "True") == 0;
         }

         help = regwcltest.GetValue("RtfXaml");
         if (help == null) fSuccess = false;
         else
         {
            options.fRtfXaml = Common.CompareStr((string)help, "True") == 0;
         }

         help = regwcltest.GetValue("REAssert");
         if (help == null) fSuccess = false;
         else
         {
            options.fREAssert = Common.CompareStr((string)help, "True") == 0;
         }

         help = regwcltest.GetValue("WordAssert");
         if (help == null) fSuccess = false;
         else
         {
            options.fWordAssert = Common.CompareStr((string)help, "True") == 0;
         }

         
         //Misc flags         
         help = regwcltest.GetValue("RestartDocs");
         if (help == null) fSuccess = false;
         else
         {
            options.fRestartDocs = Common.CompareStr((string)help, "True") == 0;
         }

         help = regwcltest.GetValue("UseXCVT");
         if (help == null) fSuccess = false;
         else
         {
            options.fUseXCVT = Common.CompareStr((string)help, "True") == 0;
         }

         help = regwcltest.GetValue("Pri0");
         if (help == null) fSuccess = false;
         else
         {
            options.fPri0 = Common.CompareStr((string)help, "True") == 0;
         }

         regwcltest.Close();
         return fSuccess;
      }

      // Returns WCLTest registry top key
      static RegistryKey GetWCLTestRegistry()
      {
         return Registry.CurrentUser.CreateSubKey("Software\\BVTTest\\1.0");
      }

      // Request options dialog after restart.
      // Used in case of fatal error
      public static void SetOptionsDialogRequestAfterRetart(bool fRequest)
      {
         RegistryKey regwcltest = GetWCLTestRegistry();
         regwcltest.SetValue("OptionsDialogOnBoot", fRequest);
         regwcltest.Close();
      }

      // GetFOptionsDialogAfterRestart
      static bool GetFOptionsDialogAfterRestart()
      {
         bool fShowDialog;

         RegistryKey regwcltest = GetWCLTestRegistry();

         try
         {
            fShowDialog = Common.CompareStr((string)regwcltest.GetValue("OptionsDialogOnBoot", "false"), "true") == 0;
            regwcltest.Close();
         }
         catch
         {
            fShowDialog = true;
            regwcltest.Close();
         };

         return fShowDialog;
      }
   }
}