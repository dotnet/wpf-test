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
    internal static class OpenDialogVerifyHelper
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
            else if (s.CanWrite)
            {   
                GlobalLog.LogEvidence("Stream returned is writeable");
            }
            else
            {   
                GlobalLog.LogEvidence("Stream is non-null, readable and non-writeable");
                return true;
            }
            
            return false;
        }
        
        internal static bool CheckFileProperties(Stream s, String fileName)
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
                GlobalLog.LogEvidence("Compare stream to a new one created with the same file");
                return CompareStreams(s, knownFileSize, knownFileContents);            
            }
            catch (FileNotFoundException)
            {
                GlobalLog.LogEvidence("FileNotFoundException thrown");
            }
            catch (Exception exp)
            {
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
            // try writing to readonly Stream anyway, this should throw an Exception
            try
            {
                GlobalLog.LogEvidence("Try to write to readonly stream");
                StreamWriter ofdWriter = new StreamWriter(s);
                ofdWriter.WriteLine("Hello, my name is Simon and I like to do drawing");
                ofdWriter.Close();
                
                // Should have thrown an Exception here; pop through and return false
                GlobalLog.LogEvidence("Expecting ArgumentException to be thrown; no actual exceptions thrown");
            }
            catch (ArgumentException)
            {
                GlobalLog.LogEvidence("ArgumentException correctly thrown when trying to write to readonly Stream");
                return true;
            }
            catch (Exception exc)
            {
                GlobalLog.LogEvidence("Expecting ArgumentException to be thrown, instead of " + exc.ToString());
            }
            
            return false;
        }
    }
}

