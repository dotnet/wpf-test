// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using System.Windows.Interop;
using Microsoft.Test;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    partial class SaveFileDialogTest
    {
        SaveFileDialog _sfd = null;
        Log _log = null;

        private void OnLoadedShowSaveFileDlg(object sender, RoutedEventArgs e)
        {
            _log = Log.Current;
            if (_log == null)
            {
                statusText.Text = "Could not find a currentTestLog.  Exiting.";
                return;
            }

            _log.CurrentVariation.LogMessage("Creating a new SaveFileDialog.");
            _sfd = new SaveFileDialog();
            if (_sfd == null)
            {
                statusText.Text = "Could not create a new SaveFileDialog. Exiting.";
                ExitWithError(statusText.Text);
                return;
            }

            _log.CurrentVariation.LogMessage("Registering SaveFileDialog eventhandlers");
            _sfd.FileOk += new CancelEventHandler(OnSaveFileDialogOk);

            _log.CurrentVariation.LogMessage("Displaying SaveFileDialog with ShowDialog()");
            _sfd.ShowDialog();
        }

        private void OnSaveFileDialogOk(object sender, CancelEventArgs e)
        {
            bool testResult = true;
            Stream sfdStream = _sfd.OpenFile();

            // [1] Check Stream properties
            testResult = CheckStreamProperties(sfdStream);
            if (!testResult)
                ExitWithError(statusText.Text);

            // [2] Check that the Stream opened matches the contents of the known file
            // (if file previously existed) or that it is clean (if file is new)
            //            testResult = CheckFileProperties(sfdStream, sfd.FileName);
            //            if (!testResult)
            //                ExitWithError(statusText.Text);

            // [3] Attempt writing to stream and log when the new data was written
            testResult = WriteToStream(sfdStream);
            if (!testResult)
                ExitWithError(statusText.Text);
            else
            {
                _log.CurrentVariation.LogMessage("TEST PASSED!");
                _log.CurrentVariation.LogResult(Result.Pass);
                ShutdownTest();
            }
        }

        private bool CheckStreamProperties(Stream s)
        {
            if (s == null)
                statusText.Text = "Stream returned was null";
            else if (!s.CanRead)
                statusText.Text = "Stream returned isn't readable";
            else if (!s.CanWrite)
                statusText.Text = "Stream returned isn't writeable";
            else
            {
                _log.CurrentVariation.LogMessage("Stream is non-null, readable and writeable");
                return true;
            }

            return false;
        }

        private bool CheckFileProperties(Stream s, String fileName)
        {
            int knownFileSize = -1;
            byte[] knownFileContents = null;

            try
            {
                // existing file: check that the contents of the stream match
                // the contents of the known file
                FileStream knownFile = new FileStream(fileName, FileMode.Open);
                knownFileSize = (int)knownFile.Length;
                knownFileContents = new byte[knownFileSize];
#pragma warning disable CA2022 // Avoid inexact read
                knownFile.Read(knownFileContents, 0, knownFileSize);
#pragma warning restore CA2022
                knownFile.Close();

                // Compare stream opened to that of a known file (check length, contents)
                _log.CurrentVariation.LogMessage("Compare stream to a new one created with the same file");
                return CompareStreams(s, knownFileSize, knownFileContents);
            }
            catch (FileNotFoundException fnfe)
            {
                // we have a new file: check that stream returned by OpenFile is empty
                _log.CurrentVariation.LogMessage("FileNotFoundException caught" + fnfe.ToString());
                if (s.Length == 0)
                {
                    _log.CurrentVariation.LogMessage("Saving to new file, stream opened is empty");
                    return true;
                }
                else
                {
                    statusText.Text = "Saving to a new file, stream opened is non-empty";
                }
            }
            catch (Exception exp)
            {
                // unexpected exception, fail test 
                statusText.Text = "Unexpected exception thrown: " + exp.ToString();
            }

            return false;
        }

        private bool CompareStreams(Stream s1, int streamLength, byte[] streamContents)
        {
            _log.CurrentVariation.LogMessage("Compare stream to known stream length");

            if (streamLength < 0 || streamContents == null)
                statusText.Text = "Cannot compare to a NULL stream";
            else if (s1.Length != streamLength)
                statusText.Text = "Stream length for known file does NOT equal stream length returned by OpenFile";
            else
            {
                // Compare file contents
                _log.CurrentVariation.LogMessage("Compare files byte by byte");
                int s1Length = (int)s1.Length;
                byte[] s1Content = new byte[s1Length];
                int currByte = 0;
                int s1BytesRead = s1.Read(s1Content, currByte, s1Length);

                // Both must be empty streams.  Don't compare them in this case.
                if (s1BytesRead != 0)
                {
                    for (currByte = 0; currByte < streamLength; currByte++)
                    {
                        // if stream contents at corresponding positions don't match,
                        // comparison fails
                        if (streamContents[currByte] != s1Content[currByte])
                        {
                            statusText.Text = "Bytes at position " + currByte + " don't match";
                            return false;
                        }
                    }
                }

                _log.CurrentVariation.LogMessage("Compared each byte in stream against known file.  All match.");
                return true;
            }

            return false;
        }

        private bool WriteToStream(Stream s)
        {
            // Try writing to read/write-enabled Stream
            try
            {
                _log.CurrentVariation.LogMessage("Trying to write to the stream.");
                StreamWriter sfdWriter = new StreamWriter(s);
                sfdWriter.WriteLine("This line written on: " + DateTime.Now);
                sfdWriter.Close();
                return true;
            }
            catch (Exception exp)
            {
                ExitWithError("Unexpected exception encountered: " + exp.ToString());
                return false;
            }
        }

        private void ExitWithError(String failMsg)
        {
            _log.CurrentVariation.LogMessage("TEST FAILED: " + failMsg);
            _log.CurrentVariation.LogResult(Result.Fail);
            ShutdownTest();
        }

        private void ShutdownTest()
        {
            ApplicationMonitor.NotifyStopMonitoring();
        }       
    }
}
