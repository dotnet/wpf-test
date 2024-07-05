// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;


namespace Microsoft.Wpf.AppModel.CommonDialogs
{
    internal static class SaveDialogVerifyHelper
    {        
        internal static bool CheckStreamProperties(Stream s)
        {
            if (s == null)
            {   
                GlobalLog.LogEvidence("Stream returned was null");
            }
            else if (!s.CanRead)
            {   
                GlobalLog.LogEvidence("Stream returned isn't readable");
            }
            else if (!s.CanWrite)
            {   
                GlobalLog.LogEvidence("Stream returned isn't writeable");
            }
            else
            {   
                GlobalLog.LogEvidence("Stream is non-null, readable and writeable");
                return true;
            }
            
            return false;
        }
        
        internal static bool CheckFileProperties(Stream s, String fileName)
        {
            int    knownFileSize     = -1;
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
                GlobalLog.LogEvidence("Compare stream to a new one created with the same file");
                return CompareStreams(s, knownFileSize, knownFileContents);            
            }
            catch (FileNotFoundException)
            {
                // we have a new file: check that stream returned by OpenFile is empty
                if (s.Length == 0)
                {
                    GlobalLog.LogEvidence("Saving to new file, stream opened is empty");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Saving to a new file, stream opened is non-empty");
                }
            }
            catch (Exception exp)
            {
                // unexpected exception, fail test 
                GlobalLog.LogEvidence("Unexpected exception thrown: " + exp.ToString());
            }
            
            return false;
        }
        
        internal static bool CompareStreams(Stream s1, int streamLength, byte[] streamContents)
        {
            GlobalLog.LogEvidence("Compare stream to known stream length");

            if (streamLength < 0 || streamContents == null)
            {
                GlobalLog.LogEvidence("Cannot compare to a NULL stream");
            }
            else if (s1.Length != streamLength)
            {
                GlobalLog.LogEvidence("Stream length for known file does NOT equal stream length returned by OpenFile");
            }
            else
            {
                // Compare file contents
                GlobalLog.LogEvidence("Compare files byte by byte");
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
                            GlobalLog.LogEvidence("Bytes at position " + currByte + " don't match");
                            return false;
                        }
                    }
                }
                
                GlobalLog.LogEvidence("Compared each byte in stream against known file.  All match.");
                return true;
            }

            return false;
        }
        
        internal static bool WriteToStream(Stream s)
        {
            // Try writing to read/write-enabled Stream
            try
            {
                GlobalLog.LogEvidence("Trying to write to the stream.");
                StreamWriter sfdWriter = new StreamWriter(s);
                sfdWriter.WriteLine("This line written on: " + DateTime.Now);
                sfdWriter.Close();
                return true;
            }
            catch (Exception exp)
            {
                Common.ExitWithError("Unexpected exception encountered: " + exp.ToString());
                return false;
            }        
        }      
    }
}
