// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Common test code for verifying errors, either in
 *          invalid xaml or invalid trees or operations.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.Threading;
using System.Windows.Threading;

using System.Reflection;
using System.Collections;
using System.Xml;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Runtime.InteropServices;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Source;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// Parser error testing.
    /// </summary>
    /// <remarks>
    /// This class parses a set of XAML files with one error in each of them.
    /// It verifies the error message from the exception thrown in each case, 
    /// including line number and position if it's a XamlParseException, 
    /// against the expected values stored in a pre-populated XML file.
    /// </remarks>
    public class ErrorTest
    {
        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void RunTest()
        {
            string strParams =  DriverState.DriverParameters["Params"];
            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            _TestErrors(strParams);
        }
        #endregion RunTest

        /// <summary>
        /// Main method that loads the error xml file, and
        /// for each file entry, calls helper routines
        /// to get the error info and verify the error occurs
        /// as expected.
        /// </summary>
        /// <param name="errFileName">Error file name</param>
        private void _TestErrors(string errFileName)
        {
            ErrorData data = null;
            ArrayList failedFiles = new ArrayList();
            XmlDocument errDoc = new XmlDocument();

            // Load the error file, and get the list of file nodes.
            errDoc.Load(errFileName);
            XmlNodeList fileNodes = errDoc.SelectNodes("./Errors/File");

            // For each file, get the expected error info, and verify
            // the error actually occurs.
            for (int i = 0; i < fileNodes.Count; i++)
            {
                data = _GetErrorData(fileNodes[i]);

                _VerifyError(data, failedFiles);
            }

            // Finished reading the error file and parsing individual files.
            // Print the results.
            CoreLogger.LogStatus("Parsed " + fileNodes.Count + " files.");
            if (failedFiles.Count > 0)
            {
                CoreLogger.LogTestResult(false, "Test failed.");
                CoreLogger.LogStatus("The following files didn't throw expected exceptions:");

                IEnumerator iterator = failedFiles.GetEnumerator();
                while (iterator.MoveNext())
                {
                    CoreLogger.LogStatus((iterator.Current as string) + "\n\n");
                }
            }
            else
            {
                CoreLogger.LogStatus("The correct errors occurred in all the files.");
            }
        }
        /// <summary>
        /// Populates a new ErrorData object with error information 
        /// from an error xml node.
        /// </summary>
        private ErrorData _GetErrorData(XmlNode fileNode)
        {
            ErrorData data = new ErrorData();

            // Filename, Description, ErrorMessage, Line, and Position.
            data.Filename = fileNode.Attributes["Name"].Value;
            data.Description = fileNode.SelectSingleNode("./Description").InnerText;
            data.ErrorMessage = fileNode.SelectSingleNode("./ErrorMessage").InnerText;

            // Check for Line and set that if necessary.
            XmlNode subNode = fileNode.SelectSingleNode("./Line");
            if (subNode != null)
            {
                data.Line = Convert.ToInt32(subNode.InnerText);
            }

            // Check for Position and set that if necessary.
            subNode = fileNode.SelectSingleNode("./Position");
            if (subNode != null)
            {
                data.Position = Convert.ToInt32(subNode.InnerText);
            }

            // Check for ExceptionType and set that if necessary.
            subNode = fileNode.SelectSingleNode("./ExceptionType");
            if (subNode != null)
            {
                data.ExceptionType = subNode.InnerText;
            }

            // Check for InnerException and set that if necessary.
            subNode = fileNode.SelectSingleNode("./InnerException");
            if (subNode != null)
            {
                data.InnerException = subNode.InnerText;
            }

            return data;
        }
        /// <summary>
        /// Attempts to parse a xaml file and display the tree, 
        /// catches exceptions, and verifies the exception matches 
        /// what is expected.  If no exception occurs or the exception 
        /// doesn't match what is expected, the file is added to the 
        /// list of failures.
        /// </summary>
        private void _VerifyError(ErrorData data, ArrayList failedFiles)
        {
            // Open the xaml file.
            Stream xamlFileStream = File.OpenRead(data.Filename);

            object root = null;
            bool loadXmlFailed = false;
            try
            {
                // Attempt to parse the xaml file.
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                root = System.Windows.Markup.XamlReader.Load(xamlFileStream, pc);
            }
            catch (Exception e)
            {
                loadXmlFailed = true;

                if (!(e is XamlParseException))
                {
                    failedFiles.Add(data.Filename + "\n"
                        + " Expected Exception: XamlParseException" + "\n"
                        + " Thrown Exception: " + e.GetType().Name);
                }
                else
                {
                    String error = _CompareError(e, data);
                    if (String.Empty != error)
                        failedFiles.Add(error);
                }
            }
            finally
            {
                xamlFileStream.Close();
            }

            if (loadXmlFailed) return;

            try
            {
                // Attempt to display the tree if we made it
                // this far.
                if (root is UIElement)
                {
                    SerializationHelper helper = new SerializationHelper();
                    helper.DisplayTree((UIElement)root, "CoreUI Error Verification Test");
                }

                // Exception expected, but not thrown. Fail the test.
                failedFiles.Add(data.Filename + " didn't throw any exception.");
            }
            catch (Exception e)
            {
                if (!e.GetType().Name.Equals(data.ExceptionType))
                {
                    failedFiles.Add(data.Filename + "\n"
                        + " Expected Exception: " + data.ExceptionType + "\n"
                        + " Thrown Exception: " + e.GetType().Name);
                }
                else
                {
                    String error = _CompareError(e, data);
                    if (String.Empty != error)
                        failedFiles.Add(error);
                }
            }            
        }

        /// <summary>
        /// Compares various fields of actual exception with the expected error data.
        /// </summary>
        /// <param name="e">Actual exception</param>
        /// <param name="data">Expected exception</param>
        /// <returns>Any difference between expected and actual exceptions</returns>
        private String _CompareError(Exception e, ErrorData data)
        {
            String error = String.Empty;

            if (!e.Message.Trim().Equals(data.ErrorMessage.Trim()))
            {
                error = data.Filename + "\n"
                    + " Expected Message: " + data.ErrorMessage + "\n"
                    + " Thrown Message: " + e.Message;
            }
            else if ((data.InnerException != null) && ((e.InnerException == null) || !e.InnerException.Message.Trim().Equals(data.InnerException.Trim())))
            {
                if (e.InnerException == null)
                    error = (data.Filename + "\n" + " Expected InnerException: " + data.InnerException + "\n" + " Actual InnerException is null ");
                else
                    error = (data.Filename + "\n" + " Expected InnerException: " + data.InnerException + "\n" + " Actual InnerException: " + e.InnerException.Message);
            }
            else
            {
                // Additional checks if this is a parse exception.
                if (e is XamlParseException)
                {
                    XamlParseException parseEx = (XamlParseException)e;
                    if (parseEx.LineNumber != data.Line)
                    {
                        error = (data.Filename + "\n" + " Expected line number: " + data.Line + "\n" + " Actual line number: " + parseEx.LineNumber);
                    }
                    else if (parseEx.LinePosition != data.Position)
                    {
                        error = (data.Filename + "\n" + " Expected position: " + data.Position + "\n" + " Actual position: " + parseEx.LinePosition);
                    }
                }
            }
            return error;
        }
    }

    /// <summary>
    /// Error record.
    /// </summary>
    internal class ErrorData
    {
        internal string Filename;
        internal string Description;
        internal string ErrorMessage;
        internal string ExceptionType = "XamlParseException";
        internal string InnerException;
        internal int Line;
        internal int Position;
    }
}
