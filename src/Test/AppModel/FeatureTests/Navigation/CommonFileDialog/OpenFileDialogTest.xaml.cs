// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Windows.Interop;
using Microsoft.Test;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    partial class OpenFileDialogTest
    {
        OpenFileDialog _ofd = null;
        Log _log = null;

        private void OnLoadedShowOpenFileDlg(object sender, RoutedEventArgs e)
        {
            _log = Log.Current;
            if (_log == null)
            {
                statusText.Text = "Could not find a currentTestLog.  Exiting.";
                return;
            }

            _log.CurrentVariation.LogMessage("Creating new OpenFileDialog");
            _ofd = new OpenFileDialog();
            if (_ofd == null)
            {
                statusText.Text = "Could not create a new OpenFileDialog. Exiting.";
                ExitWithError(statusText.Text);
                return;
            }

            _log.CurrentVariation.LogMessage("Registering OpenFileDialog eventhandlers");
            _ofd.FileOk += new CancelEventHandler(OnOpenFileDialogOk);

            _log.CurrentVariation.LogMessage("Displaying OpenFileDialog with ShowDialog()");
            _ofd.ShowDialog();
        }

        private void OnOpenFileDialogOk(object sender, CancelEventArgs e)
        {
            bool testResult = true;
            Stream ofdStream = _ofd.OpenFile();

            // [1] check Stream properties
            testResult = CheckStreamProperties(ofdStream);
            if (!testResult)
                ExitWithError(statusText.Text);

            //            // [2] check that file matches the one we want (standalone-test only)
            //            if (!BrowserInteropHelper.IsBrowserHosted)
            //            {
            //                testResult = CheckFileProperties(ofdStream, ofd.FileName);
            //                if (!testResult)
            //                    ExitWithError(statusText.Text);
            //            }

            // [3] try writing to Stream
            testResult = WriteToStream(ofdStream);
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
            else if (s.CanWrite)
                statusText.Text = "Stream returned is writeable";
            else
            {
                _log.CurrentVariation.LogMessage("Stream is non-null, readable and non-writeable");
                return true;
            }

            return false;
        }

        private bool CheckFileProperties(Stream s, String fileName)
        {
            try
            {
                int knownFileSize = -1;
                byte[] knownFileContents = null;

                // Store all the bytes in a byte array
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
                statusText.Text = "FileNotFoundException thrown\n" + fnfe.ToString();
            }
            catch (Exception exp)
            {
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
            // try writing to readonly Stream anyway, this should throw an Exception
            try
            {
                _log.CurrentVariation.LogMessage("Try to write to readonly stream");
                StreamWriter ofdWriter = new StreamWriter(s);
                ofdWriter.WriteLine("Hello, my name is Simon and I like to do drawing");
                ofdWriter.Close();

                // Should have thrown an Exception here; pop through and return false
                statusText.Text = "Expecting ArgumentException to be thrown; no actual exceptions thrown";
            }
            catch (ArgumentException ae)
            {
                _log.CurrentVariation.LogMessage("ArgumentException correctly thrown when trying to write to readonly Stream\n" + ae.ToString());
                return true;
            }
            catch (Exception exc)
            {
                statusText.Text = "Expecting ArgumentException to be thrown, instead of " + exc.ToString();
            }

            return false;
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
