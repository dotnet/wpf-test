// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#region Using
using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;

using System.Windows.Markup.Localizer;

using Microsoft.Test.Logging;
#endregion

namespace Microsoft.Test.Globalization
{
    enum eCategories{COMPLETE, DUPLICATE, EMPTY, MISSING};
    public class UIDTest
    {
        #region members
        private string _workDir;
        private static ILogger s_log;
        private static string s_msbuildPath;
        private static string[] s_strXamlNames = new string[6];
        private static bool[] s_overAllResults = new bool[4];
        #endregion

        #region RunTest
        /// <summary>
        /// Check UID Functionality
        /// </summary>
        public void RunTest(ILogger logger, string param)
        {
            s_log = logger;
            string strParams = param;
            
            //set the file names
            s_strXamlNames[0] = @"\FindDialog.xaml";
            s_strXamlNames[1] = @"\FileAssociations.xaml";
            s_strXamlNames[2] = @"\NotePadEditor.xaml";
            s_strXamlNames[3] = @"\NotePadApp.xaml";
            s_strXamlNames[4] = @"\GotoDialog.xaml";
            s_strXamlNames[5] = @"\FontChooser\FontChooser.xaml";

            //Initialize the logger
            s_log.Stage(TestStage.Initialize);

            //Get the Working directory
            _workDir = Environment.GetEnvironmentVariable("%WORKDIR%");
            if (_workDir != null && _workDir.Length != 0)
            {
                _workDir = Environment.ExpandEnvironmentVariables(_workDir);
                Directory.SetCurrentDirectory(_workDir);
            }
            else
                _workDir = Directory.GetCurrentDirectory();

            //Get the MSBuild path
            s_msbuildPath = Environment.GetEnvironmentVariable("WINDIR") + @"\Microsoft.NET\Framework\v2.0.50727\";
          
            s_log.LogResult("Test Started ..." + "\n");

            // Determine which test case to run based on the given in parameters
            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(CultureInfo.InvariantCulture);
            switch (strParams)
            {
                case "CHECKUID":
                    CheckUID();
                    break;
                case "UPDATEUID":
                    UpdateUID();
                    break;
                case "REMOVEUID":
                    RemoveUID();
                    break;
                default:
                    s_log.LogResult("UIDTest.RunTest was called with an unsupported parameter.");
                    throw new ApplicationException("Parameter is not supported.");
            }
        }
        #endregion
        
        #region Check UID
        void CheckUID()
        {
            //Starting Test Run
            s_log.Stage(TestStage.Run);

            string empty = _RunProcessAndReturnResults(_workDir + @"\checkuid\empty\avalonnotepad.proj /t:checkuid");
            string complete = _RunProcessAndReturnResults(_workDir + @"\checkuid\complete\avalonnotepad.proj /t:checkuid");
            string missing = _RunProcessAndReturnResults(_workDir + @"\checkuid\missing\avalonnotepad.proj /t:checkuid");
            string duplicate = _RunProcessAndReturnResults(_workDir + @"\checkuid\duplicate\avalonnotepad.proj /t:checkuid");

            if (_CountNumberOfLines(empty) == 1896)
                s_log.LogResult(true,"CheckUID has processed empty xamls properly");
            else
                s_log.LogResult(false,"CheckUID has not processed empty xamls properly");
            
            if (_CountNumberOfLines(complete) == 23)
                s_log.LogResult(true, "CheckUID has processed complete xamls properly");
            else
                s_log.LogResult(false, "CheckUID has not processed complete xamls properly");
            
            if (_CountNumberOfLines(missing) == 50)
                s_log.LogResult(true, "CheckUID has processed missing xamls properly");
            else
                s_log.LogResult(false, "CheckUID has not processed missing xamls properly");
            
            if (_CountNumberOfLines(duplicate) == 76)
                s_log.LogResult("CheckUID has processed duplicate xamls properly");
            else
                s_log.LogResult(false, "CheckUID has not processed duplicate xamls properly");
        }
        #endregion
        
        #region Update UID
        void UpdateUID()
        {
            //Starting Test Run
            s_log.Stage(TestStage.Run);

            //Reset the overall results
            for(int i = 0; i < 4; i++)
                 s_overAllResults[i] = false;

            //Run the test cases
            _RunUpdateUIDOrRemoveUID("updateuid", "empty", "complete", 0);
            _RunUpdateUIDOrRemoveUID("updateuid", "missing", "complete", 1);
            _RunUpdateUIDOrRemoveUID("updateuid", "duplicate", "complete", 2);
            _RunUpdateUIDOrRemoveUIDOnSelf("updateuid", "complete", 3);
            
            //Check if the results all turn in positive
            if (s_overAllResults[0] && s_overAllResults[1] && s_overAllResults[2] && s_overAllResults[3])
                s_log.LogResult(true, "UpdateUID has processed the XAMLs correctly");
            else
                s_log.LogResult(false, "UpdateUID has NOT processed the XAMLs correctly");
        }
        #endregion

        #region Remove UID
        void RemoveUID()
        {
            //Starting Test Run
            s_log.Stage(TestStage.Run);

            //Run the test cases
            _RunUpdateUIDOrRemoveUID("removeuid", "complete", "empty", 0);
            _RunUpdateUIDOrRemoveUID("removeuid", "missing", "empty", 1);
            _RunUpdateUIDOrRemoveUID("removeuid", "duplicate", "empty", 2);
            _RunUpdateUIDOrRemoveUIDOnSelf("removeuid", "empty", 3);
            
            //Check if the results all turn in positive
            if (s_overAllResults[0] && s_overAllResults[1] && s_overAllResults[2] && s_overAllResults[3])
                s_log.LogResult(true, "RemoveUID has processed the XAMLs correctly");
            else
                s_log.LogResult(false, "RemoveUID has NOT processed the XAMLs correctly");

        }
        #endregion

        #region Support functions
        bool _CompareLineNumbersOnTwoFiles(string strResultFile, string strCorrectFile)
        {
            int linecountResultFile = 0;
            int linecountCorrectfile = 0;

            //initialize the stream readers
            TextReader streamReaderResultFile = new StreamReader(strResultFile);
            TextReader streamReaderCorrectfile = new StreamReader(strCorrectFile);

            //count the lines
            while (streamReaderResultFile.ReadLine() != null)
                linecountResultFile++;
            while (streamReaderCorrectfile.ReadLine() != null)
                linecountCorrectfile++;

            //return the difference on the line numbers of each file
            if (Math.Abs(linecountCorrectfile - linecountResultFile) == 0)
                return true;
            else
                return false;
        }

        bool _CompareTwoFiles(string strResultFile, string strCorrectFile)
        {
            //initialize the stream readers
            TextReader streamReaderResultFile = new StreamReader(strResultFile);
            TextReader streamReaderCorrectfile = new StreamReader(strCorrectFile);

            //Dump to a string
            string string1 = streamReaderResultFile.ReadToEnd();
            string string2 = streamReaderCorrectfile.ReadToEnd();

            //compare if the strings are the same
            if (string1.CompareTo(string2) == 0)
                return true;
            else
                return false;
        }

        string _RunProcessAndReturnResults(string strArguments)
        {
            //Setup the process
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = s_msbuildPath + "msbuild.exe";
            p.StartInfo.Arguments = strArguments;

            s_log.LogResult("Starting MSBuild...");

            //Parse the the file and output into output directory
            p.Start();

            string output = null;

            //Read the output
            output += p.StandardOutput.ReadToEnd() + "\r\n";

            //Print out the output
            s_log.LogResult(output);

            return output;
        }

        void _RunUpdateUIDOrRemoveUID(string argument, string directoryname, string compareddirectoryname, int boolIndex)
        {
            bool[] processResults = new bool[6];

            
            //set the bools to false
            for(int i = 0; i < 6; i++)
                processResults[i] = false;
            
            s_log.LogResult("Processing" + directoryname  + " XAML");
            
            //run MSbuild
            _RunProcessAndReturnResults(_workDir + @"\" + argument + @"\"+directoryname+@"\avalonnotepad.proj /t:" + argument);

            //check if the results are the same
            for (int i = 0; i < 6; i++)
            {
                s_log.LogResult("Comparing " + _workDir + @"\" + argument + @"\" + directoryname + s_strXamlNames[i] + " and " + _workDir + @"\"+argument+@"\" + compareddirectoryname + s_strXamlNames[i]);
                processResults[i] = _CompareTwoFiles(_workDir + @"\"+ argument +@"\" + directoryname + s_strXamlNames[i], _workDir + @"\"+argument+@"\" + compareddirectoryname + s_strXamlNames[i]);

                //if there is a difference in the strings, then update failed
                if (!processResults[i])
                    s_log.LogResult(false, "Failed to " +  argument + " on " + @"\" + argument +@"\" + directoryname + s_strXamlNames[i]);
                else
                    s_log.LogResult("Files are similar");
            }

            //check if all the results passed
            if (processResults[0] && processResults[1] && processResults[2] && processResults[3] && processResults[4] && processResults[5])
            {
                s_log.LogResult(argument + " Passed on " + directoryname + " XAML");
                s_overAllResults[boolIndex] = true;
            }
        }

        void _RunUpdateUIDOrRemoveUIDOnSelf(string argument, string directoryname, int boolIndex)
        {
            bool[] processResults = new bool[6];
            string[] OriginalStringArray = new string[6];
            string[] ProcessedStringArray = new string[6];

            //set the bools to false
            for (int i = 0; i < 6; i++)
                processResults[i] = false;

            //Copy original to a string before processing
            for (int i = 0; i < 6; i++)
            {
                TextReader rOriginal = new StreamReader(_workDir + @"\" + argument + @"\" + directoryname + s_strXamlNames[i]);
                OriginalStringArray[i] = rOriginal.ReadToEnd();
            }

            //run MSbuild
            _RunProcessAndReturnResults(_workDir + @"\" + argument + @"\" + directoryname + @"\avalonnotepad.proj /t:" + argument);

            //check if the results are the same
            for (int i = 0; i < 6; i++)
            {
                //Get the processed result
                TextReader processed = new StreamReader(_workDir + @"\" + argument + @"\" + directoryname + s_strXamlNames[i]);
                ProcessedStringArray[i] = processed.ReadToEnd();

                s_log.LogResult("Comparing " + _workDir + @"\" + argument + @"\" + directoryname + s_strXamlNames[i] + " and " + _workDir + @"\" + argument + @"\" + directoryname + s_strXamlNames[i]);

                //compare the processed from before to after
                if (OriginalStringArray[i].CompareTo(ProcessedStringArray[i]) == 0)
                    processResults[i] = true;
                else
                    processResults[i] = false;

                //if there is a difference in the strings, then update failed
                if (!processResults[i])
                    s_log.LogResult(false, "Failed to " + argument + " on " + @"\" + argument + @"\" + directoryname + s_strXamlNames[i]);
                else
                    s_log.LogResult("Files are similar");
            }

            //check if all the results passed
            if (processResults[0] && processResults[1] && processResults[2] && processResults[3] && processResults[4] && processResults[5])
            {
                s_log.LogResult(argument + " Passed on " + directoryname + " XAML");
                s_overAllResults[boolIndex] = true;
            }
        }
        int _CountNumberOfLines(string str)
        {
            int size = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if(str[i] == '\n')
                    size++;
            }
            return size;
        }
        #endregion
    }
}
