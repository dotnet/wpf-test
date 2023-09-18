// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Tests
{
   using Location;
   using Common;
   using Inifile;
   using Language;
   using CountTests;
   using Ask;
   using TestList;

   // Test class - Object that runs tests

   class CTests
   {
      // Global settings
      
      System.Windows.Forms.Form _formParent;
      Delegate _delegateFinished;
      Delegate _delegateTestProgress;
      CLocation _location;
      CTestLog _testLog;
      CTestList _testlistObject;
      bool _fRunning;		// if tests are running
      int _kTestStatus;	// Status of the last test
      rxCommands[] _rgTestToRun;

      // Variables for current run

      Process _processWord;
      bool _fRunningWordProcess;
      bool _fTerminating;	 // if tests are in the process is being terminated and
      // finish delegate is already called
      int _nErrors;		 // number of failed documents
      int _nDocumentsRun;   // number of tests run
      DateTime _dtStartTest;	 // when test started
      bool _fNoAssertDialog;
      bool _fRestartEveryDoc;
      rxCommands _currentTest;

      public CTests(CLocation Location,
                   System.Windows.Forms.Form formParent,
                  CTestList testlistObject,
                   Delegate delegateFinished,
                   Delegate delegateTestProgress)
      {
         this._location = Location;
         this._formParent = formParent;
         this._delegateFinished = delegateFinished;
         this._delegateTestProgress = delegateTestProgress;
         this._testlistObject = testlistObject;

         //Common.Assert(false);
         //how am I generating my log file names?
         //Location.CreateLogFileName();
         //TestLog = new CTestLog(Location.GetLogFile());
         _testLog = null;

         _fRunning = false;
         _fTerminating = false;

         _kTestStatus = KTestStatus.Running; /* Does not matter, just initialization */

         _processWord = null;
      }

      // Main function to compare only
      //public void RunCompareOnly ( bool fFuzzyDupCompare,
      //   int kCompare,
      //   bool fIgnoreNestedPositions )
      //{
      //   Thread threadRunning;

      //   nDocumentsRun = 0;

      //   Common.Assert (!fRunning);

      //   //this.fFuzzyDupCompare = fFuzzyDupCompare;
      //   //this.kCompare = kCompare;
      //   //this.fIgnoreNestedPositions = fIgnoreNestedPositions;

      //   fRunning = true;
      //   fTerminating = false;
      //   fRunningWordProcess = false; /* Not currently running Word process */
      //   nErrors = 0;
      //   nDocumentsRun = 0;
      //   dtStartTest = DateTime.Now; // Remember when test started for ETA

      //   // Start running thread and exit

      //   threadRunning = new Thread (new ThreadStart (RunCompareOnlyAsync));
      //   threadRunning.Start ();
      //}


      // RunCompareOnlyAsync
      public void RunCompareOnlyAsync()
      {
         //int nDocuments = 0;
         //int iTestFolder;
         //string spathDoc, spathLayout, spathLayoutTemp;
         //int kLanguage;
         //int kPage;
         //bool fSaveLayout;
         //string [] rgstrDocnames;
         //int [] rgiddoc;

         //kTestStatus = KTestStatus.Running; /* Does not matter, just initialization */
         //nErrors = 0;

         //fRunning = true;

         //try 
         //{
         //   /* Create collection of test tasks */
         //   nDocuments = testlistObject.GetNumberTestedDocument ();

         //   iTestFolder = -1; /* To start from the beginning */

         //   while (testlistObject.FGetNextTestFolder (iTestFolder, out spathDoc,
         //      /*out spathLayout, out spathLayoutTemp, 
         //      out kLanguage, out fSaveLayout, 
         //      out kPage,*/  out rgstrDocnames, 
         //      out rgiddoc, out iTestFolder ))
         //   {
         //      RunCompareOnlyTask ( iTestFolder, spathDoc, spathLayout, spathLayoutTemp, 
         //                      rgstrDocnames, rgiddoc, nDocuments );
         //   }

         //   if (nErrors > 0) kTestStatus = KTestStatus.Failed;
         //   else kTestStatus = KTestStatus.OK;
         //}
         //catch (W11Exception) 
         //{ 
         //   /* Message already displayed, just set unsuccessful result */
         //   kTestStatus = KTestStatus.Failed;
         //}
         //catch (Exception ex) 
         //{ 
         //   kTestStatus = KTestStatus.Failed;

         //   /* Display unhandled excepton message */
         //   W11Messages.ShowExceptionError (ex); 
         //}

         //lock (this)
         //{
         //   // Not running any more
         //   fRunning = false;

         //   // Notify main window that tests ended
         //   formParent.Invoke (delegateFinished);
         //}
      }

      // Runs folder task and comparison, if neccesary
      //void RunCompareOnlyTask (int iFolder, string spathDoc, string spathLayout, string spathLayoutTemp, 
      //   string [] rgstrDocnames, int [] rgiddoc, int nDocumentsTotal)
      //{
      //   int idoc = 0;

      //   while (idoc < rgstrDocnames.Length)
      //   {
      //      string strCompareError;
      //      string strCompareFlags;

      //      nDocumentsRun++;

      //      if ( FCompareLayoutFiles ( spathLayout, spathLayoutTemp,
      //         rgstrDocnames [idoc], fFuzzyDupCompare, kCompare,
      //         fIgnoreNestedPositions, 
      //         out strCompareError, out strCompareFlags))
      //      {
      //         testlistObject.SetDocumentTestResult (iFolder, rgiddoc [idoc],
      //            false, KRun.EQ, strCompareFlags, "");
      //      }
      //      else 
      //      {
      //         testlistObject.SetDocumentTestResult ( iFolder, rgiddoc [idoc],
      //            false, KRun.ErrorNeq, strCompareFlags, strCompareError);
      //         nErrors ++;
      //      }
      //      idoc ++;

      //      if (fTerminating) W11Messages.RaiseError ();

      //      if (nDocumentsRun % 10 == 0) UpdateProgressBar (nDocumentsTotal, spathDoc);
      //   }
      //}


      // Main function to start test runner
      public void StartTests(rxCommands[] rgTestTypes)
      {
         Thread threadRunning;

         // Assign test input variables
         //this.fRestartEveryDoc = fRestartEveryDoc;
         //this.fNoAssertDialog = fNoAssertDialog;
         _rgTestToRun = rgTestTypes;
         // Result of the last test

         //nDocumentsRun = 0;

         Common.Assert(!_fRunning);

         _fRunning = true;
         _fTerminating = false;
         _fRunningWordProcess = false; /* Not currently running Word process */
         _nErrors = 0;
         _nDocumentsRun = 0;
         _dtStartTest = DateTime.Now; // Remember when test started for ETA

         // Start running thread and exit
         threadRunning = new Thread(new ThreadStart(RunTestsAsync));
         threadRunning.Start();
      }

      // Get status of the last test
      public int GetLastTestStatus(out int _nTests, out int _nErrors)
      {
         Common.Assert(!_fRunning);

         _nTests = _nDocumentsRun;
         _nErrors = this._nErrors;

         return _kTestStatus;
      }

      // User clicks on close button
      public void CloseTests()
      {
         lock (this)
         {
            Common.Assert(false);
            _fTerminating = true;
         }
      }

      // Killing tests
      public void KillTests()
      {
         Common.Assert(false);
         _fTerminating = true;
      }

      // Test runner thread entry point
      void RunTestsAsync()
      {
         int nDocuments = 0;
         int iTestFolder;
         string spathDoc;
         string[] rgstrDocnames;
         int[] rgiddoc;

         for (int i = 0; i < _rgTestToRun.Length; i++)
         {
            _currentTest = _rgTestToRun[i];
            try /* Need to catch all! exception in the running thread */
            {
               // Create log file
               string szBVT = null;
               switch (_currentTest)
               {
                  case rxCommands.rxRTREAssert:
                     {
                        szBVT = "REAssert";
                        break;
                     }
                  case rxCommands.rxRTRtf:
                     {
                        szBVT = "RTRtf";
                        break;
                     }
                  case rxCommands.rxRTRTFXAML:
                     {
                        szBVT = "RTRtfXaml";
                        break;
                     }
                  case rxCommands.rxRTXaml:
                     {
                        szBVT = "RTXaml";
                        break;
                     }
                  default:
                     Common.Assert(false,
                        "CurrentTest not set to a known test in CreateTestRunnerInputFile.");
                     break;
               }
               _location.CreateLogFileName(szBVT);
               File.Delete(_location.GetLogFile());
               File.Delete(_location.GetSumaryFile());

               _testLog = new CTestLog(_location.GetLogFile());
               _testLog.Start(_location.GetLogDir());

               /* Create collection of test tasks */
               
               //get number of docs to test
               
               nDocuments = _testlistObject.GetNumberTestedDocument();

               iTestFolder = -1; /* To start from the beginning */

               while (_testlistObject.FGetNextTestFolder(iTestFolder, /*out spathDoc,*/
                                               out rgstrDocnames,
                                               out rgiddoc, out iTestFolder))
               {
                  RunFolderTask(iTestFolder, /*spathDoc,*/
                              rgstrDocnames, rgiddoc, nDocuments);
               }

               if (_nErrors > 0) _kTestStatus = KTestStatus.Failed;
               else _kTestStatus = KTestStatus.OK;
            }
            catch (W11Exception)
            {
               /* Message already displayed, just set unsuccessful result */
               if (_fTerminating) _kTestStatus = KTestStatus.Terminated;
               else _kTestStatus = KTestStatus.Failed;
            }
            catch (Exception ex)
            {
               _kTestStatus = KTestStatus.Failed;

               /* Display unhandled excepton message */
               W11Messages.ShowExceptionError(ex);
            }

            // Finish log file
            
            _testLog.Finish(_kTestStatus, _nDocumentsRun, nDocuments, _nErrors);
         }

         lock (this)
         {
            // Not running any more
            _fRunning = false;

            // Notify main window that tests ended
            _formParent.Invoke(_delegateFinished);
         }
      }

      // Make sure the main test folder has subfolders docs, layout, [layout.temp]
      //static void CheckTestStructure (string spathTestRoot, bool fMustHaveLayoutSubfolder)
      //{
      //   string [] rgspathFolders;
      //   string [] rgfnFiles;
      //   bool fDocs = false;

      //   if (!Directory.Exists (spathTestRoot))
      //   {
      //      W11Messages.RaiseError ("Root test folder '" + spathTestRoot + "' is not found");
      //   }

      //   rgspathFolders = Directory.GetDirectories (spathTestRoot, "*.*");

      //   foreach (string spathCurrent in rgspathFolders)
      //   {
      //      string spathname = Path.GetFileName (spathCurrent);

      //      if (Common.CompareStr (spathname, "Docs") == 0) fDocs = true;
      //   }

      //   if (!fDocs)
      //   {
      //      W11Messages.RaiseError ("Root test folder '" + spathTestRoot + "' must contain 'Docs' subfolder");
      //   };

      //   rgfnFiles = Directory.GetFiles (spathTestRoot, "*.doc");

      //   if (rgfnFiles.Length > 0)
      //   {
      //      W11Messages.RaiseError ("Root test folder '" + spathTestRoot + "' may not contain any documents. All documents must be under Docs subfolder");
      //   };
      //}

      // Create root folder, handle error
      //static void CreateFolder (string spath)
      //{
      //   try
      //   {
      //      if (!Directory.Exists (spath))
      //      {
      //         Directory.CreateDirectory (spath);
      //      };
      //   }
      //   catch (Exception ex)
      //   {
      //      W11Messages.RaiseError ("Unable to create folder " + spath +"\n\n" + ex.ToString () );
      //   }
      //}


      // Runs folder task and comparison, if neccesary
      void RunFolderTask(int iFolder, string[] rgstrDocnames,
            int[] rgiddoc, int nDocumentsTotal)
      {
         //string sfnTestRunnerDoc = "";// Location.GetFnTestRunnerDoc();
         bool fFinishedWord = false;
         string szCmdLine;
         int idoc = 0;//progress through this folders documents
         //DateTime dtLastPickup;
         //TimeSpan tsWordProcessorLastPickup;
         string[] rgstrDocnamesAscii;

         /* Add folder information in the log file */
         //TestLog.LogTestFolder(spathDoc);

         //Update progress bar 
         UpdateProgressBar(nDocumentsTotal, "NYI"/*spathDoc*/);

         #region //iterate through rgstrDocnames
         while (idoc < rgstrDocnames.Length)
         {
            int ndocsCur;//documents to process on this mini run
            //int ndocsProcessed;
            bool fAssertFound;
            bool fCrashFound;
            //   int iDocInterrupted = - 1;

            if (_location.options.fRestartDocs) ndocsCur = 1; /* Must run 1 document at a time */
            else
            {
               /* Not required to restart after every doc. Restart after 12 documents */
               ndocsCur = Math.Min(rgstrDocnames.Length, 12 * ((idoc + 12) / 12)) - idoc;
            };

            /* Create a file list for this mini run and command line string */
            CreateTestRunnerInputFile(rgstrDocnames,
               idoc, ndocsCur, out szCmdLine);

            /* Do not even start Word if we were terminated */
            //if (fTerminating) 
            //{
            //   if (FContinueAfterTermination ("")) fTerminating = false;
            //   else W11Messages.RaiseError ();
            //};

            Common.Assert(!_fRunningWordProcess);

            // Start word
            try
            {
               _processWord = Process.Start(_location.GetRtfXamlExe(), szCmdLine);
            }
            catch
            {
               _fTerminating = true;
               W11Messages.RaiseError("Can not start RTFXamlView "); //+ sfnTestRunnerDoc);
            }

            _fRunningWordProcess = true;
            fFinishedWord = false;
            fAssertFound = false;
            fCrashFound = false;
            //fTimeOut = false;
            //fTimeOutCauseByDialog = false;

            //ndocsProcessed = 0;

            //so wait for one minute per document
            _processWord.WaitForExit(92000);
            fFinishedWord = _processWord.HasExited;

            if (fFinishedWord)
            {
               /* Pick up processed files */
               StreamReader sr;
               string szResult;
               if (File.Exists(_location.GetTempBVTLogFile()))
               {
                  sr = new StreamReader(_location.GetTempBVTLogFile());
                  szResult = sr.ReadToEnd();
                  sr.Close();
               }
               else
               {
                  szResult = "Unable to retrieve temp log file for this batch.\r\n";
               }
               _testLog.LogString(szResult);
               _testLog.Flush();
               //now pick up results
               StreamReader srs;
               string szSumary;
               if (File.Exists(_location.GetTempSumaryFile()))
               {
                  srs = new StreamReader(_location.GetTempSumaryFile());
                  szSumary = srs.ReadToEnd();
                  srs.Close();
               }
               else
               {
                  szSumary = "Unable to retrieve temp sumary file for this batch.\r\n";
               }
               StreamWriter sw = new StreamWriter(_location.GetSumaryFile(), true);
               sw.Write(szSumary);
               sw.Flush();
               sw.Close();
            }
            else//something went wrong, skip this cluster of files and move on
            {
               for (int i = idoc; (idoc < idoc + ndocsCur) && (i < rgstrDocnames.Length); i++)
               {
                  _testLog.LogLine(rgstrDocnames[i] + ", Unknown Problem, app crash.");
               }
            }

            File.Delete(_location.GetTempBVTLogFile());
            File.Delete(_location.GetLogDir() + @"/FList.txt");
            File.Delete(_location.GetTempSumaryFile());

            idoc += ndocsCur;
            /* Update progress bar */
            UpdateProgressBar(nDocumentsTotal, "NYI");

            #region/* Make sure Word has finished */
            //if (!fFinishedWord)
            //{
            //   if (!fTimeLimit) processWord.WaitForExit ();
            //   else 
            //   {
            //      processWord.WaitForExit (tsecTimeLimit * 1000);
            //      if (!processWord.HasExited)
            //      {
            //         try 
            //         {
            //            processWord.Kill ();
            //         }
            //         catch { /* Ignore error in case if process already exited */ };
            //         processWord.WaitForExit ();
            //      }
            //   };
            //};
            #endregion
            
            // Make sure Word has finished, commented out for now
            _fRunningWordProcess = false;

            /* Check if all files were proccessed or Word exited prematurely (crashed/Assert) */
            #region //commented out for now
            //if (fAssertFound)
            //{
            //   /* Stopped on the last document, .run file should not be created */
            //   if (ndocsProcessed == ndocsCur)
            //   {
            //      /* Assert happened after last document, in the macro??? */
            //      string strMsg = "*** AF in the test macro: " + GetAssertMsg ();

            //      TestLog.LogLine ();
            //      TestLog.LogLine (strMsg);
            //      TestLog.LogLine ();
            //      ProtocolDocError (Protocol, rgstrDocnames [idoc + ndocsProcessed - 1], strMsg);
            //   }
            //   else
            //   {
            //      /* Assert on the last open document. Run file for such document was not created */
            //      string strMsg = "AF " + GetAssertMsg ();

            //      testlistObject.SetDocumentTestResult ( iFolder, rgiddoc [idoc + ndocsProcessed],
            //         ndocsProcessed == 0, 
            //         (fTimeOut ? KRun.ErrorTimeout : KRun.ErrorAF), "", strMsg);

            //      TestLog.LogEndDoc (true, false, "", strMsg, rgstrDocnames [idoc + ndocsProcessed]);
            //      ProtocolDocError (Protocol, rgstrDocnames [idoc + ndocsProcessed], strMsg);

            //      iDocInterrupted = idoc + ndocsProcessed; // In case we are being interrupted

            //      nDocumentsRun ++;
            //      ndocsProcessed++;
            //   }
            //}
            //else if (ndocsProcessed < ndocsCur)
            //{
            //   testlistObject.SetDocumentTestResult ( iFolder, rgiddoc [idoc + ndocsProcessed],
            //      ndocsProcessed == 0, 
            //      (fTimeOut? KRun.ErrorTimeout : KRun.ErrorUnfinished), "", (fCrashFound ? "Crash" : ""));

            //   TestLog.LogEndDoc (true, false, "", "Unfinished", rgstrDocnames [idoc + ndocsProcessed]);
            //   ProtocolDocError (Protocol, rgstrDocnames [idoc + ndocsProcessed], "Unfinished");

            //   iDocInterrupted = idoc + ndocsProcessed; // Remember which document was terminated
            //   nErrors ++;
            //   nDocumentsRun ++;
            //   ndocsProcessed++;
            //}
            //else
            //{
            //   /* Everything is ok */
            //};

            //Common.Assert (ndocsProcessed > 0); // Guard against infinite loop
            //idoc += ndocsProcessed;

            ///* Delete temporary output files */
            //DeleteRunnerOutputFiles ();

            /* Kill remaining recover dialog boxes */
            //FKillWordRecoveryDialogs ();

            /* Check maybe we were terminated */
            //if (fTerminating)
            //{
            //   if (FContinueAfterTermination ((iDocInterrupted == -1 ? "" :
            //      rgstrDocnames [iDocInterrupted] )))
            //   {
            //      fTerminating = false;
            //   }
            //   else
            //   {
            //      DeleteDocumentsWithUnicodeNames (spathDoc, rgstrDocnames, rgstrDocnamesAscii);
            //      W11Messages.RaiseError ();
            //   };
            //}

            //DeleteDocumentsWithUnicodeNames (spathDoc, rgstrDocnames, rgstrDocnamesAscii);
            //}
            #endregion //commented out for now

         }//while (idoc < rgstrDocnames.Length)
         #endregion //iterate through rgstrDocnames
      }

      // CopyDocumentsWithUnicodeNames
      // Copy documents with unicode names so that Word macro can open them
      //static void CopyDocumentsWithUnicodeNames (string spathDoc, string [] rgstrDocnames, out string [] rgstrDocnamesAscii)
      //{
      //   int i;

      //   rgstrDocnamesAscii = new string [rgstrDocnames.Length];

      //   for (i=0; i < rgstrDocnames.Length; i++)
      //   {
      //      if (Common.FContainsUnicodeCharacter (rgstrDocnames [i]))
      //      {
      //         string sfnAscii;
      //         rgstrDocnamesAscii [i] = Common.GetTemporaryDocumentName (spathDoc);

      //         sfnAscii = Path.Combine (spathDoc, rgstrDocnamesAscii [i]);
      //         File.Copy (Path.Combine (spathDoc, rgstrDocnames [i]), sfnAscii, true);

      //         // Clear read-only attribute
      //         File.SetAttributes (sfnAscii, (FileAttributes) (File.GetAttributes (sfnAscii) & ~FileAttributes.ReadOnly));
      //      }
      //      else
      //      {
      //         rgstrDocnamesAscii [i] = rgstrDocnames [i];
      //      }
      //   }
      //}

      // DeleteDocumentsWithUnicodeNames
      //static void DeleteDocumentsWithUnicodeNames (string spathDoc, string [] rgstrDocnames, string [] rgstrDocnamesAscii)
      //{
      //   int i;

      //   for (i=0; i < rgstrDocnames.Length; i++)
      //   {
      //      if (rgstrDocnames [i] != rgstrDocnamesAscii [i])
      //      {
      //         try
      //         {
      //            File.Delete (Path.Combine (spathDoc, rgstrDocnamesAscii [i]));
      //         }
      //         catch 
      //         {
      //         }; // Do not through an error if file can not be deleted
      //      }
      //   }
      //}

      // UpdateProgressBar
      // Calculate ETA and call parent form delegate to report progress
      void UpdateProgressBar(int nDocumentsTotal, string spathDoc)
      {
         bool fTestEta;
         DateTime dtTestEta;
         TimeSpan tspan = DateTime.Now.Subtract(_dtStartTest);

         if (_nDocumentsRun < 10 || tspan.TotalSeconds < 10)
         {
            fTestEta = false;
            dtTestEta = DateTime.Now; // Eta is invalid, assign something
         }
         else
         {
            fTestEta = true;
            dtTestEta = _dtStartTest.Add
                        (new TimeSpan
                           (0, 0, 0, 0, (int)(tspan.TotalMilliseconds * nDocumentsTotal / _nDocumentsRun)));
         }


         _formParent.Invoke(_delegateTestProgress,
            new object[] { _nDocumentsRun, nDocumentsTotal, _nErrors, 
							    _testlistObject.GetNumberOfTestErrorsFixed (),
							    _testlistObject.GetNumberOfTestErrorsNew (),
								spathDoc, fTestEta, _dtStartTest, dtTestEta });
      }

      // Check if Word already running
      //bool FFindWordProcess ()
      //{
      //string fnameWordDebug = Path.GetFileNameWithoutExtension (Location.GetFnWordExe (KTarget.Debug));
      //string fnameWordShip  = Path.GetFileNameWithoutExtension (Location.GetFnWordExe (KTarget.Ship));

      //return Process.GetProcessesByName (fnameWordDebug).Length > 0 || 
      //   Process.GetProcessesByName (fnameWordShip).Length > 0 ;
      //   return false;//while deving BVT TEST
      //}


      // Kill remaining recovery dialog boxes
      //bool FKillWordRecoveryDialogs ()
      //{
      //   bool fFoundRecoveryDialogs  = false;

      //   foreach (Process dw20 in Process.GetProcessesByName ("DW20"))
      //   {
      //      try
      //      {
      //         Protocol.WriteWarning ("Killing dw20.exe");
      //         dw20.Kill ();
      //         fFoundRecoveryDialogs = true;
      //      }
      //      catch { /* Do not care about exceptions... */ } 

      //      dw20.WaitForExit ();
      //   }

      //   return fFoundRecoveryDialogs;
      //}


      // Check if end user wants to stop entire test
      bool FContinueAfterTermination(string strdocnameTerminated)
      {
         string strMsg;

         if (strdocnameTerminated != "")
         {
            strMsg = "Interrupted:  " + strdocnameTerminated + "\n\n" +
                       "Do you want to stop the entire test?";
         }
         else
         {
            strMsg = "Do you want to interrupt the entire test?";
         }

         return !Ask.YesNoDialog(strMsg, "STOP", "Continue", true);
      }

      // Creates test runner input file
      //I'm changing this to create a file list and run string for RTFXAMLView
      void CreateTestRunnerInputFile(string[] rgstrDocnames,
                                       int iFrom, int nDocs, out string szCmdLine)
      {
         StreamWriter stream = null;
         string szFileList = _location.GetLogDir() + "\\FList.txt";//change to temp file later
         try
         {
            stream = File.CreateText(szFileList);
         }
         catch
         {
            W11Messages.RaiseError("Unable to create file list: " + szFileList);
            //return;
         }

         /* Write document names in file list.*/
         stream.Write(rgstrDocnames[iFrom]);
         for (int idoc = (iFrom + 1); idoc < iFrom + nDocs; idoc++)
         {
            stream.Write("," + rgstrDocnames[idoc]);
         }
         stream.Close();

         //create cmdline string
         string szBVT = null;
         switch (_currentTest)
         {
            case rxCommands.rxRTREAssert:
               {
                  szBVT = "-REAssert";
                  break;
               }
            case rxCommands.rxRTRtf:
               {
                  szBVT = "-RTRtf";
                  break;
               }
            case rxCommands.rxRTRTFXAML:
               {
                  szBVT = "-RTRtfXaml";
                  break;
               }
            case rxCommands.rxRTXaml:
               {
                  szBVT = "-RTXaml";
                  break;
               }
            default:
               Common.Assert(false,
                  "CurrentTest not set to a known test in CreateTestRunnerInputFile.");
               break;
         }

         szCmdLine = "-LogFile ";
         szCmdLine += @"""";
         szCmdLine += _location.GetTempBVTLogFile();
         szCmdLine += @"""";
         szCmdLine += " -FileList ";
         szCmdLine += @"""";
         szCmdLine += szFileList;
         szCmdLine += @"""";
         szCmdLine += " -Sumary ";
         szCmdLine += @"""";
         szCmdLine += _location.GetTempSumaryFile();
         szCmdLine += @"""";

         szCmdLine += " -NumFiles " + nDocs.ToString() + " " + szBVT;
      }

      // Get rid of runner input file
      //void DeleteTestRunnerInputFile ()
      //{
      //string sfnTestRunnerInput = Location.GetFnTestRunnerInput ();
      //try
      //{
      //   File.Delete (sfnTestRunnerInput);
      //}
      //catch { W11Messages.RaiseError ("Unable to delete W11TestRunner input file " + sfnTestRunnerInput); 
      //}
      //}

      // Get rid of Assert file
      //void DeleteAssertFile ()
      //{
      //string sfnAssertTxt = Location.GetFnAssertTxt ();
      //try
      //{
      //   if (File.Exists (sfnAssertTxt))
      //   {
      //      File.Delete (sfnAssertTxt);
      //   };
      //}
      //catch { 
      //         W11Messages.RaiseError ("Unable to delete Assert file" + sfnAssertTxt); }
      //}


      // Check if there was Assert
      //bool FCheckAssert ()
      //{
      //string sfnAssertTxt = Location.GetFnAssertTxt ();
      //return File.Exists (sfnAssertTxt);
      //return false;
      //}

      // Reads Assert message, providing that Assert was already detected
      //string GetAssertMsg ()
      //{
      //string sfnAssertTxt = Location.GetFnAssertTxt ();
      //string strResult;

      //try
      //{
      //   StreamReader stream = new StreamReader (sfnAssertTxt);

      //   if (stream.Peek () != -1)
      //   {
      //      strResult = stream.ReadLine ();
      //   }
      //   else strResult = "??? Assert file " + sfnAssertTxt + " empty";

      //   stream.Close ();
      //}
      //catch { W11Messages.RaiseError ("Unable to read Assert file" + sfnAssertTxt);
      //      strResult = ""; };

      //return strResult;
      //   return "";//while deving BVTTEST
      //}

      // Get rid of runner input file
      //void DeleteRunnerOutputFiles ()
      //{
      //try 
      //{
      //   string [] rgfnRunnerOutput = Directory.GetFiles (Location.GetPathTestRunnerOutput (), Location.GetMaskTestRunnerOutput ());
      //   foreach (string fn in rgfnRunnerOutput)
      //   {
      //      File.Delete (fn);
      //   }
      //}
      //catch
      //{
      //   W11Messages.RaiseError ("Unable to delete files .run from folder " + Location.GetPathTestRunnerOutput ());
      //};
      //}

      // Rename ascii=>uncidoe layout output file
      //static void RenameUnicodeLayoutFile (string spathLayoutTemp, string strDocname, string strDocnameAscii)
      //{
      //try
      //{
      //   if (strDocname != strDocnameAscii)
      //   {
      //      string fnxmlLayout = Path.Combine (spathLayoutTemp, Path.GetFileNameWithoutExtension (strDocname) + ".xml");
      //      string fnxmlLayoutAscii = Path.Combine (spathLayoutTemp, Path.GetFileNameWithoutExtension (strDocnameAscii) + ".xml");

      //      if (File.Exists (fnxmlLayoutAscii))
      //      {
      //         File.Move (fnxmlLayoutAscii, fnxmlLayout);
      //      }
      //   }
      //}
      //catch { /* Ignore all errors */ };
      //}

      // Compares saved xml files
      //static bool FCompareLayoutFiles ( string spathLayout,  string spathLayoutTemp, 
      //                          string strDocumentName, bool fFuzzyDupCompare,
      //                          int kCompare, bool fIgnoreNestedPositions, out string strError, 
      //                          out string strFlags)
      //{
      //string fnxmlLayout = Path.Combine (spathLayout, Path.GetFileNameWithoutExtension (strDocumentName) + ".xml");
      //string fnxmlLayoutTemp = Path.Combine (spathLayoutTemp, Path.GetFileNameWithoutExtension (strDocumentName) + ".xml");

      //strFlags = "";

      //if (!Directory.Exists (spathLayout)) 
      //{
      //   strError = "Can not find layout folder: " + spathLayout;
      //   return false;
      //}
      //else if (!Directory.Exists (spathLayoutTemp)) 
      //{
      //   strError = "Can not find temporary layout folder: " + spathLayoutTemp;
      //   return false;
      //}
      //else if (!File.Exists (fnxmlLayout))
      //{
      //   strError = "Can not find saved xml layout file: " + fnxmlLayout;
      //   return false;
      //}
      //else if (!File.Exists (fnxmlLayoutTemp))
      //{
      //   strError = "Can not find saved xml layout file: " + fnxmlLayoutTemp;
      //   return false;
      //}
      //else
      //{
      //   return Layout.FCompare ( fnxmlLayout, fnxmlLayoutTemp, fFuzzyDupCompare,
      //                      kCompare, fIgnoreNestedPositions, out strError, out strFlags );
      //}
      //		}

      // View test log
      public void ViewTestLog()
      {
         _testLog.View();
      }

      // View example test ini
      //public static void ViewExampleTestIni ( CLocation Location)
      //{
      //string sfnExampleTestIni = Location.GetFnSampleTestIni ();

      //try
      //{
      //   if (!File.Exists (sfnExampleTestIni))
      //   {
      //      W11Messages.ShowWarning ("Can not find " + sfnExampleTestIni + ")");
      //   }
      //   else
      //   {
      //      try 
      //      {
      //         /* Open notepad and do not wait till exit */
      //         Process processNotepad = Process.Start ("notepad.exe", sfnExampleTestIni);
      //      }
      //      catch { W11Messages.RaiseError ("Unable to start notepad.exe with " + sfnExampleTestIni);};
      //   };
      //}
      //catch (W11Exception)  {}
      //catch (Exception ex) { W11Messages.ShowExceptionError (ex); }

      //}

   } // End of CTests


   // Implemenation for test log file

   class CTestLog
   {
      string _sfnTestLog;		// Name of the log file
      string _strCache;	// Cached line that has to be written in the file

      DateTime _dtStartTest;

      // Constructor
      public CTestLog(string sfnTestLog)
      {
         this._sfnTestLog = sfnTestLog;
         _strCache = "";
      }

      // Start...
      //deletes the log file if one exists, prepares to create and log output
      public void Start(string spathTestRoot)
      {
         _dtStartTest = DateTime.Now;

         if (File.Exists(_sfnTestLog))
         {
            /* Delete existing log file */
            try { File.Delete(_sfnTestLog); }
            catch
            {
               W11Messages.RaiseError("Unable to delete test log file (" + _sfnTestLog + ")");
               return;
            }
         };
         //string szBVT = null;
         //switch (TestType)
         //{
         //   case rxCommands.rxRTREAssert:
         //      {
         //         szBVT = "REAssert";
         //         break;
         //      }
         //   case rxCommands.rxRTRtf:
         //      {
         //         szBVT = "RTRtf";
         //         break;
         //      }
         //   case rxCommands.rxRTRTFXAML:
         //      {
         //         szBVT = "RTRtfXaml";
         //         break;
         //      }
         //   case rxCommands.rxRTXaml:
         //      {
         //         szBVT = "RTXaml";
         //         break;
         //      }
         //   default:
         //      Common.Assert(false,
         //         "CurrentTest not set to a known test in CreateTestRunnerInputFile.");
         //      break;
         //}
         //LogLine("Starting " + szBVT + " Test.");//+ dtStartTest.ToString ().PadLeft (64-"Starting test log".Length));
         //LogLine (spathTestRoot.PadLeft (64));
         //LogLine ();
      }


      // Finish
      public void Finish(int kTestStatus, int nTested, int nTotal, int nErrors)
      {
         TimeSpan timespan = DateTime.Now - _dtStartTest;

         //LogLine ();
         //LogLine ();
         //LogLine ("*****************************");
         //LogLine ("*                           *");
         //LogLine ("* Explanation of test flags *");
         //LogLine ("*                           *");
         //LogLine ("*****************************");
         //LogLine ();
         //LogLine ();
         //LogLine ("*******************");
         //LogLine ("*                 *");
         //LogLine ("* " + KTestStatus.ToPresentationString (kTestStatus).PadRight (16) + "*");
         //LogLine ("*                 *");
         //LogLine ("*******************");
         //LogLine ();
         //LogLine ("Tested documents = " + nTested.ToString () + (nTested != nTotal ? " out of " + nTotal.ToString () : ""));
         //LogLine ("Errors = " + nErrors.ToString ());
         //LogLine ();
         //LogLine ("Execution time = " + ((int) timespan.TotalSeconds).ToString () + " seconds");

         Flush();
      }

      // View test log
      public void View()
      {
         lock (this)
         {
            Flush();

            if (!File.Exists(_sfnTestLog))
            {
               W11Messages.ShowWarning("Test log file not found (" + _sfnTestLog + ")");
            }
            else
            {
               try
               {
                  /* Open notepad and do not wait till exit*/
                  Process processNotepad = Process.Start("notepad.exe", _sfnTestLog);
               }
               catch { W11Messages.RaiseError("Unable to start notepad.exe with " + _sfnTestLog); };
            };
         };
      }

      // Trim document name for logging
      private string PadAndTrimDocumentName(string strDocName, out bool fTrimmed)
      {
         if (strDocName.Length <= 59)
         {
            fTrimmed = false;
            return strDocName.PadRight(59);
         }
         else
         {
            fTrimmed = true;
            return strDocName.Substring(0, 59 - 3) + "...";
         }
      }

      // Log when document starts
      public void LogBeginDoc(string strDocName, bool fRestartWord, int n)
      {
         bool fTrimmedUnused;

         LogString("  " + (fRestartWord ? "r" : " ") + n.ToString().PadLeft(5) + ". " +
                  PadAndTrimDocumentName(strDocName, out fTrimmedUnused));
         Flush();
      }

      // Log when finished document
      public void LogEndDoc(bool fError, bool fComparison, string strCompareFlags, string strErrorMessage, string strDocName)
      {
         bool fTrimmed;
         string strFullName = "";
         string strResult;
         string strComment = "";

         PadAndTrimDocumentName(strDocName, out fTrimmed);

         // Disable full name output
         // if (fTrimmed) strFullName = "~ " + strDocName;

         if (!fError)
         {
            if (fComparison) strResult = "  EQ";
            else strResult = "  OK";
         }
         else strResult = "* ERROR *";

         if (strCompareFlags != "") strComment = "(" + strCompareFlags + ")";

         LogLine(" " + strResult.PadRight(9) + "  " + strComment.PadRight(8) + " " + strErrorMessage +
            (strErrorMessage != "" && strFullName != "" ? "     " : "") + strFullName);
      }

      // Log test folder header
      //public void LogTestFolder ( string spathDoc, string spathLayoutTemp)
      //{
      //   LogLine ();
      //   LogLine ("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      //   LogLine ("  Directory : " + spathDoc);
      //   LogLine ("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      //   LogLine ();
      //}
      
      // Log new line
      public void LogLine(string strMessage)
      {
         _strCache += strMessage + (char)0xD + (char)0xA;
      }

      // Log empty line
      public void LogLine()
      {
         LogLine("");
      }

      // Log new line
      public void LogString(string strMessage)
      {
         _strCache += strMessage;
      }

      // Flush caches string
      public void Flush()
      {
         StreamWriter stream;

         lock (this)
         {
            if (_strCache != "")
            {
               /* Open for "append" */
               try { stream = new StreamWriter(_sfnTestLog, true); }
               catch
               {
                  W11Messages.RaiseError("Unable to write in test log file (" + _sfnTestLog + ")");
                  return;
               };

               stream.Write(this._strCache);
               stream.Close();
            };

            _strCache = "";
         }
      }
   } // End of class CTestLog
} // End of namespace