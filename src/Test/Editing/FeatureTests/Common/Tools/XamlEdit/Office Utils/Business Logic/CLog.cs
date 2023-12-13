// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace O12FFVC
{
    public class Log
    {
        private static string LogFullName
        {
            get
            {
                string sLogFullName ="";
                //if (CRunHarness.IsRunning)
                //{
                //  //  sLogFullName = CRunHarness.ResultsFolderPath + "\\_Log.txt";
                //   // Debug.Assert(CUtils.CreateOrUseExistingFile(sLogFullName), "Could not create or use log file " + sLogFullName);
                //}
                //else
                //{
                //    //sLogFullName = CSettings.WorkingDirectory + "\\GlobalLog.txt";
                //    //Debug.Assert(CUtils.CreateOrUseExistingFile(sLogFullName), "Could not create or use log file " + sLogFullName);
                //    sLogFullName = null; // we are haven't started a run yet. No log to write to because we dont know the folder.
                //}

                return sLogFullName;
            }
        }

        public static void WriteLine(string msg)
        {
            Debug.WriteLine(msg);
            WriteLineToLog(msg);
        }

        public static void Assert(bool condition, string msg)
        {
            Debug.Assert(condition, msg);
            if (!condition)
            {
                WriteLineToLog("DEBUG ASSERT: " + msg);
            }
        }

        public static void Assert(bool condition, string msg, string detailedMessage)
        {
            Debug.Assert(condition, msg, detailedMessage);
            if (!condition)
            {
                WriteLineToLog("DEBUG ASSERT: " + msg, detailedMessage);
            }
        }

        public static void Fail(string msg, string detailedMsg)
        {
            Debug.Fail(msg, detailedMsg);
            WriteLineToLog("DEBUG FAIL: " + msg, detailedMsg);
        }

        public static void Fail(string msg)
        {
            Debug.Fail(msg);
            WriteLineToLog("DEBUG FAIL: " + msg);
        }

        private static void WriteLineToLog(string logMessage, string detailedMessage)
        {
            //if (LogFullName == null)
            //    return;

            //using (StreamWriter w = File.AppendText(LogFullName))
            //{
            //    w.Write(DateTime.Now + ": " + logMessage);

            //    if (!string.IsNullOrEmpty(detailedMessage))
            //    {
            //        w.Write(" (Details: " + detailedMessage + ")");
            //    }

            //    w.WriteLine(string.Empty);
            //    w.Flush();
            //    w.Close();
            //}
            Console.WriteLine(logMessage);
        }

        private static void WriteLineToLog(String logMessage)
        {
            WriteLineToLog(logMessage, null);
        }
    }

}
