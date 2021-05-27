// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.IO;
using Microsoft.Test.Security.Wrappers;

namespace Microsoft.Test.MSBuildEngine
{
    /// <summary>
    /// Constants defined for Error Parsing.
    /// </summary>
    struct Constants
    {
        public const string ErrorRootElement = "MSBuildErrors";
        public const string ErrorElement = "Error";
        public const string WarningElement = "Warning";
        public const string DescriptionElement = "Description";
        public const string SourceElement = "Source";
        public const string StartingMessageElement = "StartingMessage";
        public const string LineNumber = "LineNumber";
        public const string ColumnNumber = "ColumnNumber";

        public const string PartialAttribute = "Partial";
        public const string IDAttribute = "ID";
        public const string ReferredIDAttribute = "ReferredID";
        public const string IgnoreAttribute = "Ignore";

        public const string AssemblyResourceAssembly = "Assembly";
        public const string AssemblyResoruceResourceName = "ResourceName";
        public const string AssemblyResourceErrorIdentifier = "ErrorIdentifier";
    }

    /// <summary>
    /// Summary description for CommonHelper.
    /// </summary>
    static class MSBuildEngineCommonHelper
    {
        #region Member Variables

        private static DebugMode bDebug;
        static MemoryStream ms = null;

        internal static string urtpath = null;
        internal static string urtversion = null;
        static StreamWriterSW sw = null;
        static string logfilename = null;
        static bool bconsolelog = true;

        static string presentationframeworkfullname = null;
        static string pfversioninformation = null;

        #endregion Member Variables

        #region Public Methods

        // Todo: Need to improve this to check absolute path, relative path, server and 
        // other paths are not supported.
        /// <summary>
        /// Helper method to check if an existing file exists.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string VerifyFileExists(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                return null;
            }

            // Todo: Can cause an Arguement exception need to handle that.
            string filepath = PathSW.GetFullPath(filename.ToLowerInvariant());

            if (!FileSW.Exists(filepath.ToLowerInvariant()))
            {
                return null;
            }

            // Todo: First try didn't get anywhere.
            // Next

            return filepath;
        }

        /// <summary>
        /// Display exception information on to the Console.
        /// </summary>
        /// <param name="ex">Exception type.</param>		
        public static void DisplayExceptionInformation(Exception ex)
        {
            LogError = "";
            LogError = "\t" + ex.GetType().Name + " Occured";
            LogError = "\t" + "Message - \n\t   " + ex.Message.ToString();

            //if (bDebug != DebugMode.Quiet)
            //{
            LogError = "\t" + ex.Source.ToString();
            LogError = "\t" + ex.StackTrace.ToString();
            if (ex.InnerException != null)
            {
                LogError = "\t" + ex.InnerException.ToString();
            }
            //}
        }

        /// <summary>
        /// Log to Console dependent on running debug mode.
        /// </summary>
        /// <value></value>
        public static string Log
        {
            set
            {
                if (bDebug != DebugMode.Quiet)
                {
                    Console.WriteLine("MSBuildLog - {0}", value);
                }

                SaveToMemory(value, ref ms, logfilename);
            }
        }

        /// <summary>
        /// Logging enabled only when running in any mode other than Quiet.
        /// </summary>
        public static string LogDiagnostic
        {
            set
            {
                if (bDebug != DebugMode.Quiet)
                {
                    Log = value;
                }
            }
        }

        /// <summary>
        /// Log Errors to console.
        /// </summary>
        /// <value></value>
        public static string LogError
        {
            set
            {
                CheckConsole();
                if (bconsolelog)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.WriteLine(value);

                if (bconsolelog)
                {
                    Console.ResetColor();
                }

                SaveToMemory(value, ref ms, logfilename);
            }
        }

        /// <summary>
        /// Log Warnings to console.
        /// </summary>
        /// <value></value>
        public static string LogWarning
        {
            set
            {
                CheckConsole();
                if (bconsolelog)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.WriteLine(value);

                if (bconsolelog)
                {
                    Console.ResetColor();
                }

                SaveToMemory(value, ref ms, logfilename);
            }
        }

        /// <summary>
        /// Enables logging LHCompiler debug info to file.
        /// </summary>
        /// <value></value>
        public static void LogToFile(string write)
        {
            if (String.IsNullOrEmpty(logfilename))
            {
                logfilename = @"LHCompiler.log";
            }

            logfilename = PathSW.GetFullPath(logfilename);

            switch (write)
            {
                case "Overwrite":
                    sw = new StreamWriterSW(logfilename);
                    sw.Flush();
                    break;

                case "Append":
                    if (FileSW.Exists(logfilename))
                    {
                        sw = new StreamWriterSW(logfilename, true);
                    }
                    else
                    {
                        sw = new StreamWriterSW(logfilename);
                        sw.Flush();
                    }
                    break;

                default:
                    break;
            }

            if (sw != null)
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Flag indicating if current compilation needs to be run in debug mode.
        /// </summary>
        /// <value></value>
        public static DebugMode Debug
        {
            set
            {
                bDebug = value;
            }
            get
            {
                return bDebug;
            }
        }

        /// <summary>
        /// Full name of PresentationFramework compiled against.
        /// Helper to find PF version info.
        /// </summary>
        public static string PresentationFrameworkFullName
        {
            get
            {
                if (String.IsNullOrEmpty(presentationframeworkfullname))
                {
                    presentationframeworkfullname = typeof(System.Windows.FrameworkElement).Assembly.FullName;
                }

                return presentationframeworkfullname;
            }
        }


        /// <summary>
        /// The Version infromation, PublicKeyToken and Culture infromation for Avalon assemblies.
        /// </summary>
        public static string PresentationVersionInformation
        {
            get
            {
                if (string.IsNullOrEmpty(pfversioninformation))
                {
                    int index = presentationframeworkfullname.IndexOf(',');
                    if (index < 0)
                    {
                        return null;
                    }

                    pfversioninformation = presentationframeworkfullname.Substring(index);
                }

                return pfversioninformation;
            }
        }

        /// <summary>
        /// Save a ---- to memory for writing to filestream later.
        /// </summary>
        public static void SaveToMemory(string message, ref MemoryStream filememstream, string logfilename)
        {
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            byte[] byteArray = null;
            if (filememstream == null)
            {
                filememstream = new MemoryStream();
                byteArray = unicodeEncoding.GetBytes(message);
            }
            else
            {
                if (filememstream.Length > Int16.MaxValue / 4)
                {
                    WritetoFilefromMemory(ref filememstream, logfilename);
                    return;
                }

                byteArray = filememstream.GetBuffer();
                //char[] charArray = new char[uniEncoding.GetCharCount(byteArray, 0, count)];
                //uniEncoding.GetDecoder().GetChars(byteArray, 0, count, charArray, 0);

                string previousstring = unicodeEncoding.GetString(byteArray, 0, (int)filememstream.Length);
                message = previousstring + "\n" + message;
                filememstream.Flush();
                filememstream.Close();
                filememstream = null;

                filememstream = new MemoryStream();
                byteArray = unicodeEncoding.GetBytes(message);
            }

            filememstream.Write(byteArray, 0, byteArray.Length);
            if (filememstream.Length > Int16.MaxValue / 4)
            {
                WritetoFilefromMemory(ref filememstream, logfilename);
            }

            byteArray = null;
        }

        /// <summary>
        /// Using a memory stream object write data in the buffer to filestream.
        /// </summary>
        /// <param name="filememstream"></param>
        /// <param name="logfile"></param>
        public static void WritetoFilefromMemory(ref MemoryStream filememstream, string logfile)
        {
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            if (filememstream == null)
            {
                return;
            }

            byte[] byteArray = filememstream.GetBuffer();
            string message = unicodeEncoding.GetString(byteArray, 0, (int)filememstream.Length);
            filememstream.Flush();
            filememstream.Close();
            filememstream = null;

            WriteToLogFile(message, logfile);
        }

        #endregion Public Methods

        #region Internal Methods
        /// <summary>
        /// Close the 
        /// </summary>
        public static void WritetoStreamandClose()
        {
            if (ms != null)
            {
                WritetoFilefromMemory(ref ms, logfilename);
            }
        }

        #endregion Internal Methods

        #region Private Methods
        /// <summary>
        /// Write to log file.
        /// </summary>
        private static void WriteToLogFile(string message, string logfilename)
        {
            if (String.IsNullOrEmpty(logfilename) == false)
            {
                sw = new StreamWriterSW(logfilename, true);
                if (String.IsNullOrEmpty(message))
                {
                    sw.WriteLine();
                }
                else
                {
                    sw.WriteLine(message);
                }

                sw.Close();
                sw = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CheckConsole()
        {
            if (bconsolelog == false)
            {
                return;
            }

            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            catch
            {
                bconsolelog = false;
            }

            if (bconsolelog)
            {
                Console.ResetColor();
            }
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Debug Mode settings.
    /// </summary>
    enum DebugMode
    {
        /// <summary>
        /// Debug with Minimal output
        /// </summary>
        Quiet,
        /// <summary>
        /// Debug with Verbose output
        /// </summary>
        Verbose,
        /// <summary>
        /// Debug with Detailed output
        /// </summary>
        Diagnoistic
    }

    /// <summary>
    /// Structure that holds ProjFile information including 
    /// FileName, FilePath and FileExtension.
    /// 
    /// This is mainly to help the generation of the References section
    /// converting proj to new proj.
    /// </summary>
    struct ProjRefFileInfo
    {
        internal string includefilename;

        internal string includefilepath;

        internal string includefileextension;
    }
}
