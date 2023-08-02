// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description:		Testing interface
using System;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using Microsoft.Test.WindowsUIAutomation.Logging;

namespace Microsoft.Test.WindowsUIAutomation.Logging
{
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    static public class LogTypes
    {
        public static string ConsoleLogger = "WUIALoggingConsole.dll";
        public static string PiperLogger = "WUIALoggerPiper.dll";
        public static string WTTLogger = "WUIALoggerWTT.dll";
        public static string XmlLogger = "WUIALoggerXml.dll";
        public static string DefaultLogger = "DEFAULT";
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class Logger
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Connection to the logging interface
        /// </summary>
        /// -------------------------------------------------------------------
        static internal IWUIALogger _IWUIALogger;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static string s_currentLogger;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static public void CloseLog()
        {
            _IWUIALogger.CloseLog();
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Call to set the specific logger.  There are some default loggers
        /// already defined.  Use the LogTypes class for a list of them.
        /// </summary>
        /// <param name="logFile">Ful path and name of the log file</param>
        /// ---------------------------------------------------------------
        static public void SetLogger(string logFile)
        {
            _IWUIALogger = SetLoggerType(logFile);
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Returns a results object of the run
        /// </summary>
        /// ---------------------------------------------------------------
        static public IWUIALogger BackEndLogger
        {
            get
            {
                return _IWUIALogger;
            }
            set
            {
                _IWUIALogger = value;
            }
        }

        #region IWUIALogger wrapper

        /// ---------------------------------------------------------------
        /// <summary>
        /// Some frameworks such as Piper will kill a process if it
        /// is closed incorrectly and hangs around.  If the framework
        /// does not support this, it will NOP out there
        /// </summary>
        /// <param name="process"></param>
        /// ---------------------------------------------------------------
        public static void MonitorProcess(Process process)
        {
            ValidateLogger();
            _IWUIALogger.MonitorProcess(process);
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Specifies that a test is starting
        /// </summary>
        /// ---------------------------------------------------------------
        public static void StartTest(string testName)
        {
            ValidateLogger();
            _IWUIALogger.StartTest(testName);
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Specifies that a test is ending
        /// </summary>
        /// ---------------------------------------------------------------
        public static void EndTest()
        {
            ValidateLogger();
            _IWUIALogger.EndTest();
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Report that the test had an error
        /// </summary>
        /// ---------------------------------------------------------------
        public static void LogError(string errorMessage)
        {
            ValidateLogger();
            _IWUIALogger.LogError(errorMessage);
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Report that the test had an error
        /// </summary>
        /// ---------------------------------------------------------------
        public static void LogError(Exception exception, bool displayTrace)
        {
            ValidateLogger();
            _IWUIALogger.LogError(exception, displayTrace);
        }

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        public static void LogPass()
        {
            ValidateLogger();
            _IWUIALogger.LogPass();
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Returns a report summary
        /// </summary>
        /// ---------------------------------------------------------------
        public static void ReportResults()
        {
            ValidateLogger();
            _IWUIALogger.ReportResults();
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Add a comment to the test summary
        /// </summary>
        /// ---------------------------------------------------------------
        public static void LogComment(object comment)
        {
            ValidateLogger();
            _IWUIALogger.LogComment(comment);
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Add a comment to the test summary
        /// </summary>
        /// <param name="format">The format string</param>
        /// <param name="args">An array of objects to write using format</param>
        /// ---------------------------------------------------------------
        public static void LogComment(string format, params object[] args)
        {
            ValidateLogger();

            // format may have formating characters '{' & '}', only call
            // String.Format if there is a valid args arrray. Calling it
            // without and arg will throw an exception if you have formating 
            // chars and no args array.
            if (args.Length == 0)
            {
                _IWUIALogger.LogComment(format);
            }
            else
            {
                _IWUIALogger.LogComment(String.Format(format, args));
            }
        }

        #endregion

        /// -------------------------------------------------------------------
        /// <summary>
        /// Loads a logger DLL binary that impements WUIALogging interface.
        /// If you specify the LogTypes.DefaultLogger, then WttLogger
        /// will be loaded if the executable has INTERNAL in the name, else
        /// PIper logger will be loaded
        /// </summary>
        /// <param name="filename">Filename with path</param>
        /// -------------------------------------------------------------------
        static private IWUIALogger SetLoggerType(string filename)
        {
            filename = (filename == null) ? LogTypes.DefaultLogger : filename;
            filename = (filename == string.Empty) ? LogTypes.DefaultLogger : filename;
            filename = (filename == "") ? LogTypes.DefaultLogger : filename;
            s_currentLogger = filename;

            if (filename == LogTypes.DefaultLogger)
            {
                // 
                if (true == IsRunningUnderWTT())
                {
                    Logger.SetLogger(LogTypes.WTTLogger);
                }
                else
                {
                    Logger.SetLogger(LogTypes.PiperLogger);
                }
            }
            else if (filename == LogTypes.ConsoleLogger)
            {
                _IWUIALogger = (IWUIALogger)new ConsoleLogger();
            }
#if BUILD_LOGGER_INTOAPP
// If this is built into the UI Visual Test Manager, then include the specific Xml 
// logger into the exe for simplicity.  No need to seperate dll to tag onto the 
// deployment of the application.  If this is part of the BVT solution, then we
// will need to delay load it.  Most likely, we will never use an XML logger in the lab.
            else if (filename == LogTypes.XmlLogger)
            {
                _IWUIALogger = new XmlLogger();
            }
#endif
            else
            {
                // If the user has implicitly or explicity loaded the logger, make 
                // sure that instance of the logger is closed before we load another
                // logger.
                if (_IWUIALogger != null)
                {
                    _IWUIALogger.CloseLog();
                    _IWUIALogger = null;
                }

                string pathFile = System.IO.Directory.GetCurrentDirectory() + @"\" + filename;
                Console.WriteLine("Loading: " + pathFile);
                string className = "Microsoft.Test.WindowsUIAutomation.Logging.Logger";
                string interfaceName = "Microsoft.Test.WindowsUIAutomation.Logging.IWUIALogger";

                // Load the binary dll
                Assembly assembly = Assembly.LoadFile(pathFile);

                // Get the logger class specified
                Type assemblyType = assembly.GetType(className);
                if (assemblyType == null)
                    throw new Exception("Could not load type \"" + className + "\" from file \"" + pathFile + "\"");

                Type[] s = assemblyType.GetInterfaces();

                //Make sure assembly supports the correct interface
                if (assemblyType.GetInterface(interfaceName) == null)
                    throw new Exception("File \"" + filename + "\" with class object \"" + className + "\" does not support the interface \"" + interfaceName + "\"");

                _IWUIALogger = (IWUIALogger)Activator.CreateInstance(assemblyType);
                if (_IWUIALogger == null)
                    throw new Exception("Could not create an instance of the logger");
            }

            return _IWUIALogger;
        }

        /// -------------------------------------------------------------------
        /// <summary>TODO$: ClientTestRuntime should be able to do this in the future</summary>
        /// -------------------------------------------------------------------
        static bool IsRunningUnderWTT()
        {
            return false;
/*  we always want to log through TestRuntime.  It will deal with whatever's above it.
            //For debugging
            if (Environment.GetEnvironmentVariable("LOG_TO_WTT") != null)
                return true;

            //The WTT Client sets this variable when it runs the process
            return Environment.GetEnvironmentVariable("WTTTASKGUID") != null;
*/
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal static void ValidateLogger()
        {
            if (_IWUIALogger == null)
                SetLoggerType(LogTypes.DefaultLogger);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static void LogUnexpectedError(Exception exception, bool displayTrace)
        {
            _IWUIALogger.LogUnexpectedError(exception, displayTrace);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static void SetCodeExample(string elementPath, string exampleCall)
        {
            _IWUIALogger.CacheCodeExampleVariables(elementPath, exampleCall);
        }
    }
}
