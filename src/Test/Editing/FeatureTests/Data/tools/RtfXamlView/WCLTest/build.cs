// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Build
{
	using Common;
	using Location;
	using Protocol;

   // This object is responsible building Word

	class CBuild
	{
		System.Windows.Forms.Form	_formParent;
		Delegate					_delegateFinished;
		CLocation					_location;
		CProtocol					_protocol;
		bool						_fRunning;		// if build is running
		bool						_fTerminating;	// if build is in the process is being terminated and
													// finish delegate is already called
		int							_kBuildSuccess;	// result of the last build

		/* Current build variables  */

		bool						_fRunningBuildProcess;
		Process						_processBuild;
		int							_kTarget;
		string						_strConfiguration;
		bool						_fCleanWAL;
		bool						_fCleanWord;
		bool						_fFullLink;


		// Contructor
		public CBuild ( CLocation Location,
						CProtocol Protocol,
						System.Windows.Forms.Form formParent, 
						Delegate delegateFinished )
		{
			this._location = Location;
			this._protocol = Protocol;
			this._formParent = formParent;
			this._delegateFinished = delegateFinished;

			_fRunning = false;
			_fTerminating = false;
			_processBuild = null;
		}

		// Main entry to start test runner (user clicks on build)
      //public void StartBuild ( int kTarget, 
      //   string strConfiguration,
      //   bool fCleanWAL,
      //   bool fCleanWord,
      //   bool fFullLink )
      //{
      //   Thread threadRunning;

      //   Common.Assert (!fRunning);
			
      //   /* Assign current build input variants */

      //   this.kTarget = kTarget;
      //   this.strConfiguration = strConfiguration;
      //   this.fCleanWAL = fCleanWAL;
      //   this.fCleanWord = fCleanWord;
      //   this.fFullLink = fFullLink;

      //   fRunning = true;
      //   fRunningBuildProcess = false;
      //   fTerminating = false;

      //   // Start running thread and exit

      //   threadRunning = new Thread (new ThreadStart (RunBuildAsync));
      //   threadRunning.Start ();
      //}

		// Get status of the last test
		public int /* KBuildSuccess */ GetResultOfLastBuild ()
		{
			Common.Assert (!_fRunning);

			return _kBuildSuccess;
		}

		// User clicks on close button.
		// Note there is no Kill for build
		public void CloseBuild ()
		{	
			lock (this)
			{
				if (!_fTerminating)
				{
					if (_fRunningBuildProcess)
					{
						// Kill the running Word process
						try
						{
							_processBuild.CloseMainWindow ();
						}
						catch (System.InvalidOperationException) { /* Process has already finished */ }
					}

					_fTerminating = true;
				}
			}

		}

		// Entry point for async build process thread
		// Build input variables must be initialized
      //void RunBuildAsync ()
      //{
      //   /* Need to catch all exceptions in this thread */
      //   try
      //   {
      //      kBuildSuccess = RunBuildAsyncCore ();
      //   }
      //   catch (W11Exception) { kBuildSuccess = KBuildSuccess.Errors; }
      //   catch (Exception ex) 
      //   { 
      //      kBuildSuccess = KBuildSuccess.Errors;
      //      /* Display unhandled excepton message */
      //      W11Messages.ShowExceptionError (ex); 
      //   }

      //   lock (this)
      //   {
      //      // Not running any more
      //      fRunning = false;

      //      // Notify main window that tests ended
      //      formParent.Invoke (delegateFinished);
      //   }
      //}

		// Core function for async build 
		// Does not catch unhandled exceptions and does not call main form delegate
      //int /* KBuildSuccess */ RunBuildAsyncCore ()
      //{
      //   //string sfnErrorLog = "";//Location.GetFnBuildErrorLog (kTarget, strConfiguration);
      //   //string sfnBuildCommandFile = Location.GetFnBuildTempCommandFile ();

      //   //CreateBuildCommandFile ( Location, kTarget, strConfiguration, 
      //   //                   fCleanWAL, fCleanWord, fFullLink, 
      //   //                   true /* check for run files */);

      //   ///* Delete error log if present */
      //   //if (File.Exists (sfnErrorLog))
      //   //{
      //   //   try { File.Delete (sfnErrorLog); }
      //   //   catch 
      //   //   { W11Messages.ShowWarning ("Unable to delete error log " + sfnErrorLog); }
      //   //};


      //   ///* Run build command file */
      //   //try 
      //   //{
      //   //   lock (this)
      //   //   {
      //   //      processBuild = Process.Start (sfnBuildCommandFile, "");
      //   //      fRunningBuildProcess = true;
      //   //   }

      //   //   processBuild.WaitForExit ();


      //   ///* Delete temporary build command file */
      //   //File.Delete (sfnBuildCommandFile);

      //   //if (fTerminating) 
      //   //{
      //   //   /* Do not continue if in the process of terminating */
      //   //   return KBuildSuccess.Terminated;
      //   //}
      //   //else
      //   //{
      //   //   /* Check if build was successful? */

      //   //   if (!File.Exists (sfnErrorLog))
      //   //   {
      //   //      /* Error log not found. Why? */
      //   //      W11Messages.ShowError ("Log not found (" + sfnErrorLog + ")");
      //   //      return KBuildSuccess.Errors;
      //   //   }
      //   //   else
      //   //   {
      //   //      bool fErrors;
      //   //      bool fWarnings;

      //   //      CheckIfErrorLogHasErrors (sfnErrorLog, out fErrors, out fWarnings);

      //   //      if (fErrors) 
      //   //      {
      //   //         ShowErrorLogCore (Location, kTarget, strConfiguration);

      //   //         /* Errors */
      //   //         return KBuildSuccess.Errors;
      //   //      }
      //   //      else if (fWarnings) 
      //   //      {
      //   //         /* Warning */
      //   //         return KBuildSuccess.Warnings;
      //   //      }
      //   //      else 
      //   //      {
      //   //         /* Successful */
      //   //         return KBuildSuccess.Successful;
      //   //      };
      //   //   };
      //   //}
      //}

		// Creates temporary build command file
      //static void CreateBuildCommandFile ( CLocation Location, 
      //                            int kTarget, 
      //                            string strConfiguration,
      //                            bool fCleanWAL,
      //                            bool fCleanWord,
      //                            bool fFullLink,
      //                            bool fCheckRunFilesPresent)
      //{
      //   string strEnvSystemRoot = Environment.GetEnvironmentVariable ("SYSTEMROOT");
      //   string sfnCommandFile = Location.GetFnBuildTempCommandFile ();
      //   string sfnOenvTest = Path.Combine (Location.GetPathOtools (), "bin\\oenvtest.bat");
      //   string sfnWordMakeCmd = Path.Combine(Location.GetPathWord (strConfiguration), "tools\\bin\\m.cmd");

      //   string strMakeOptions;
      //   StreamWriter stream;

      //   if (fCheckRunFilesPresent && !File.Exists (sfnOenvTest))
      //   {
      //      W11Messages.RaiseError ("Can not find Oenvtest.bat (" + sfnOenvTest + ")" + KCharConst.eoln1 + 
      //         "Make sure you have otools on your machine");
      //      return;
      //   };

      //   if (fCheckRunFilesPresent && !File.Exists (sfnWordMakeCmd))
      //   {
      //      W11Messages.RaiseError ("Can not find word make file (" + sfnWordMakeCmd + ")" + KCharConst.eoln1);
      //      return;
      //   };

      //   if (File.Exists (sfnCommandFile))
      //   {
      //      try { File.Delete (sfnCommandFile); }
      //      catch { W11Messages.RaiseError ("Can not delete temporary command file " + sfnCommandFile); };
      //   };

      //   stream = File.CreateText (sfnCommandFile);

      //   stream.WriteLine ("@echo off");
      //   stream.WriteLine ();
      //   stream.WriteLine ("Rem **********************");
      //   stream.WriteLine ("Rem Set office environment");
      //   stream.WriteLine ("Rem **********************");
      //   stream.WriteLine ();
      //   stream.WriteLine ("SET WORD=" + Location.GetPathWord (strConfiguration));
      //   if (strEnvSystemRoot != null && strEnvSystemRoot != "")
      //   {
      //      stream.WriteLine ("SET PATH=" + strEnvSystemRoot + ";" + strEnvSystemRoot + "\\system32");
      //   };

      //   stream.WriteLine ();
      //   stream.WriteLine ("SET OTOOLS=");
      //   stream.WriteLine ("SET INCLUDE=");
      //   stream.WriteLine ("SET LIB=");
      //   stream.WriteLine ();
      //   stream.WriteLine ("CALL " + sfnOenvTest);
      //   stream.WriteLine ();

      //   if (fFullLink && ! fCleanWord)
      //   {
      //      /* Force full link */
      //      stream.WriteLine ("Rem *************************************");
      //      stream.WriteLine ("Rem Delete Word executables for full link");
      //      stream.WriteLine ("Rem *************************************");
      //      stream.WriteLine ();
      //      stream.WriteLine ("del " + Location.GetFnWordExe (kTarget) + " /Q");
      //      if (Location.options.kword == KWord.w11)
      //      {
      //         stream.WriteLine ("del " + Location.GetFnWwintlDll_1 (kTarget) + " /Q");
      //         stream.WriteLine ("del " + Location.GetFnWwintlDll_2 (kTarget) + " /Q");
      //      }
      //      stream.WriteLine ();
      //   }

      //   if (fCleanWAL && ! fCleanWord)
      //   {
      //      /* Force recompilation of all WAL files */
      //      string [] rgsfnSourceFiles;
				
      //      try 
      //      {
      //         rgsfnSourceFiles = Directory.GetFiles ( Path.Combine (Location.GetPathWord (strConfiguration),
      //            "WAL\\src"), "*.c");
      //      } 
      //      catch (Exception ex) 
      //      { 
      //         W11Messages.RaiseError ("Can not get access WAL sources" + KCharConst.eoln1 + ex.ToString ());
      //         return;
      //      };

      //      stream.WriteLine ("Rem ***********************");
      //      stream.WriteLine ("Rem Delete WAL object files");
      //      stream.WriteLine ("Rem ***********************");
      //      stream.WriteLine ();

      //      foreach (string sfnSourceFile in rgsfnSourceFiles)
      //      {
      //         string sfnObjectFile = Path.Combine ( Location.GetPathBuildObjs (kTarget, strConfiguration),
      //            Path.GetFileNameWithoutExtension (sfnSourceFile) + ".obj");

      //         stream.WriteLine ("del " + sfnObjectFile + " /Q");
      //      }
      //      stream.WriteLine ();
      //   };

      //   stream.WriteLine ("Rem *********************");
      //   stream.WriteLine ("Rem Run Word make (m.cmd)");
      //   stream.WriteLine ("Rem *********************");
      //   stream.WriteLine ();

      //   strMakeOptions = (kTarget == KTarget.Debug ? " dw " : " sw ") + 
      //               (fCleanWord ? "+f " : "");

      //   stream.WriteLine ( sfnWordMakeCmd + strMakeOptions );

      //   stream.Close ();
      //}

		// Checks if build error log has errors
		static void CheckIfErrorLogHasErrors (string sfnErrorLog, out bool fErrors, out bool fWarnings)
		{
			StreamReader stream = File.OpenText (sfnErrorLog);
			string [] arrLines = Common.ReadLinesFromStream (stream, 12);
			stream.Close ();

			fErrors =   arrLines.Length != 12 ||
				arrLines [1]  != "" ||
				arrLines [2]  != "------" ||
				arrLines [3]  != "Errors" ||
				arrLines [4]  != "------" &&
				arrLines [5]  != "Scanning mkword.log for Errors and Warnings...Finished" ||
				arrLines [6]  != "  No errors found in mkword.log" ||
				(
				arrLines [7]  != "  No warnings found in mkword.log" &&
				arrLines [7]  != "  warnerr.log written."
				);

			fWarnings = arrLines.Length != 12 ||
				arrLines [7]  != "  No warnings found in mkword.log" ||
				arrLines [8]  != "  warnerr.log empty." ||
				arrLines [9]  != "--------" ||
				arrLines [10] != "Warnings" ||
				arrLines [11] != "--------";
		}

		// User clicks on View Cmd button
		public static void ViewCommandFile ( CLocation Location, 
											 int kTarget, 
											 string strConfiguration,
											 bool fCleanWAL,
											 bool fCleanWord,
											 bool fFullLink )
		{
         //string sfnCommandFile = Location.GetFnBuildTempCommandFile ();

         //CreateBuildCommandFile ( Location, kTarget, strConfiguration, fCleanWAL,
         //                   fCleanWord, fFullLink,
         //                   false /* do not check for run files */ );

         //if (!File.Exists (sfnCommandFile)) 
         //{
         //   W11Messages.RaiseError ("Unable to create command file");
         //}
         //else
         //{
         //   try 
         //   {
         //      /* Open notepad and do not wait till exit */
         //      Process ProcessNotepad = Process.Start ("notepad.exe", sfnCommandFile);
         //   }
         //   catch { W11Messages.RaiseError ("Unable to start notepad.exe with " + sfnCommandFile);};
         //};
         Common.Assert(false, "Tried to view Command file.");
		}

		// User clicks on Error Log button
		public static void ViewErrorLog (CLocation Location, int kTarget, string strConfiguration)
		{
			//ShowErrorLogCore (Location, kTarget, strConfiguration);
         Common.Assert(false, "Tried to view error log.");
		}

		// Core function to show build error log
      //static void ShowErrorLogCore (CLocation Location, int kTarget, string strConfiguration)
      //{
      //   string sfnErrorLog = Location.GetFnBuildErrorLog (kTarget, strConfiguration);

      //   if (!File.Exists (sfnErrorLog))
      //   {
      //      W11Messages.ShowWarning ("Error log file not found (" + sfnErrorLog + ")");
      //   }
      //   else
      //   {
      //      try 
      //      {
      //         /* Open notepad and do not wait till exit */
      //         Process processNotepad = Process.Start ("notepad.exe", sfnErrorLog);
      //      }
      //      catch { W11Messages.RaiseError ("Unable to start notepad.exe with " + sfnErrorLog);};
      //   };
      //}

	}; // End of class Build 

} // End of namespace Build
