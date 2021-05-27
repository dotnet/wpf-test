// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Microsoft.Test.Security.Wrappers;
//using Microsoft.Test.VariationEngine;

namespace Microsoft.Test.MSBuildEngine
{
    /// <summary>
    /// Logger class that attachs it self to all MSBuild events.
    /// If Build log files are specified the build logs get written to respective files and Console.
    /// </summary>
    class MSBuildProjLogger : IMSBuildLogger
    {
        #region Local variables
        internal bool _bbuiltwitherrors = false;
        internal bool _bbuiltwithwarnings = false;

        Hashtable expectederrors;

        //StreamWriter buildlog, builderror, buildwarning;

        string buildlogfile, builderrorfile, buildwaringfile;

        int tabcount = 0;

        string globalcurrentprojfile = null;

        LoggerVerbosity verbosity;

        Stack projectfilestack = null;
        ArrayList _directoriescreated = null;

        internal bool _berrorhandling = false;

        internal List<ErrorWarningCode> _listofhandlederrors = null;
        internal List<string> listoferrorwarningswithoutid = null;

        ErrorWarningCode currentduplicateerrorwarningid = null;

        MemoryStream errormemorystream;
        MemoryStream warningmemorystream;
        MemoryStream logmemorystream;

        #endregion Local variables

        #region Public Methods

        /// <summary>
        /// Initialize Logger and set LoggerVerbosity to Normal.
        /// </summary>
        public MSBuildProjLogger()
        {
            verbosity = LoggerVerbosity.Normal;
            _directoriescreated = new ArrayList();
        }

        /// <summary>
        /// Attach all MSBuild events to designated event handlers.
        /// </summary>
        /// <param name="eventSource"></param>
        public void Initialize(IEventSource eventSource)
        {
            // Method that needs to be implemented with ILogger implementation.
            // Attach to all events provided.
            eventSource.BuildStarted += new BuildStartedEventHandler(eventSource_BuildStartedEvent);
            eventSource.BuildFinished += new BuildFinishedEventHandler(eventSource_BuildFinishedEvent);
            eventSource.ProjectStarted += new ProjectStartedEventHandler(eventSource_ProjectStartedEvent);
            eventSource.ProjectFinished += new ProjectFinishedEventHandler(eventSource_ProjectFinishedEvent);
            eventSource.ErrorRaised += new BuildErrorEventHandler(eventSource_ErrorEvent);
            eventSource.CustomEventRaised += new CustomBuildEventHandler(eventSource_CommentEvent);
            eventSource.TargetStarted += new TargetStartedEventHandler(eventSource_TargetStartedEvent);
            eventSource.TargetFinished += new TargetFinishedEventHandler(eventSource_TargetFinishedEvent);
            eventSource.TaskStarted += new TaskStartedEventHandler(eventSource_TaskStartedEvent);
            eventSource.TaskFinished += new TaskFinishedEventHandler(eventSource_TaskFinishedEvent);
            eventSource.WarningRaised += new BuildWarningEventHandler(eventSource_WarningEvent);
            eventSource.MessageRaised += new BuildMessageEventHandler(eventSource_MessageRaised);
            eventSource.StatusEventRaised += new BuildStatusEventHandler(eventSource_StatusEventRaised);

            projectfilestack = new Stack();
        }

        /// <summary>
        /// ILogger interface implementation.
        /// </summary>
        public void Shutdown()
        {
            MSBuildEngineCommonHelper.Log = "Closing MSBuild";
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Set Build Log file name
        /// </summary>
        /// <value>Log File name</value>
        public string BuildLogFileName
        {
            // Open file and clear out contents.
            set
            {
                if (value == null)
                {
                    return;
                }

                buildlogfile = value;
                StreamWriterSW buildlog = new StreamWriterSW(value);
                buildlog.Flush();
                buildlog.Close();
            }
            get
            {
                return buildlogfile;
            }
        }

        /// <summary>
        /// Set Build Error Log file name
        /// </summary>
        /// <value>Error Log file name</value>
        public string BuildLogErrorFileName
        {
            set
            {
                if (value == null)
                {
                    return;
                }

                builderrorfile = value;
                StreamWriterSW builderror = new StreamWriterSW(value);
                builderror.Flush();
                builderror.Close();
            }
            get
            {
                return builderrorfile;
            }
        }

        /// <summary>
        /// Set Build Warning Log file name, open file and clear out contents.
        /// </summary>
        /// <value>Warning log file name</value>
        public string BuildLogWarningFileName
        {
            set
            {
                if (value == null)
                {
                    return;
                }

                buildwaringfile = value;
                StreamWriterSW buildwarning = new StreamWriterSW(value);
                buildwarning.Flush();
                buildwarning.Close();
            }
            get
            {
                return buildwaringfile;
            }
        }

        /// <summary>
        /// MSBuild build log level to Verbose, Diagnostic or Quiet.
        /// Defaults to Normal.
        /// </summary>
        /// <value></value>
        public Microsoft.Build.Framework.LoggerVerbosity Verbosity
        {
            get
            {
                return verbosity;
            }
            set
            {
                this.verbosity = value;
            }
        }

        /// <summary>
        /// MSBuild Parameters property
        /// Not Impelemented
        /// </summary>
        /// <value></value>
        public String Parameters
        {
            get
            {
                throw new NotImplementedException("Parameters property in not impelemented");
            }
            set
            {
                throw new NotImplementedException("Parameters property in not impelemented");
            }
        }

        #endregion Public Properties

        #region Private Event Handlers
        /// <summary>
        /// Build Started Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_BuildStartedEvent(object sender, BuildStartedEventArgs e)
        {
            //MSBuildEngineCommonHelper.Log = "************Starting Build " + e.Code;
            LogToBuildLog(e.Message + "\t" + e.Timestamp.ToString() + ".");
            LogToBuildLog(@"________________________________________________");
        }

        /// <summary>
        /// Project Started Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_ProjectStartedEvent(object sender, ProjectStartedEventArgs e)
        {
            // Print the project file currently building
            if (e.ProjectFile != null)
            {
                string strblanks = "";
                for (int i = 0; i < projectfilestack.Count; i++)
                {
                    strblanks += " ";
                }

                if (projectfilestack.Count > 0)
                {
                    Console.WriteLine();
                }

                Console.Write("{0}Compiling - {1}  ", strblanks, e.ProjectFile);

                string currentprojfile = e.ProjectFile;
                if (String.IsNullOrEmpty(currentprojfile) == false)
                {
                    currentprojfile = PathSW.GetFullPath(currentprojfile);

                    //Console.WriteLine(Directory.GetCurrentDirectory());
                    //Console.WriteLine(currentprojfile);
                    // Pushing current project file into stack.
                    projectfilestack.Push(currentprojfile);

                    LogToBuildLog("Current project file - " + currentprojfile);
                }

            }

            tabcount = 0;
            this.LogToBuildLog(e, true);
        }

        /// <summary>
        /// Project Finished Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_ProjectFinishedEvent(object sender, ProjectFinishedEventArgs e)
        {
            LogToBuildLog("");
            // Print the project file currently building
            if (e.ProjectFile != null)
            {
                string strblanks = "";
                for (int i = 0; i < projectfilestack.Count; i++)
                {
                    if (i - 1 > 0)
                    {
                        strblanks += " ";
                    }
                }

                Console.WriteLine("{0}Done Compiling - {1}", strblanks, e.ProjectFile);
                globalcurrentprojfile = e.ProjectFile;
                if (String.IsNullOrEmpty(globalcurrentprojfile) == false)
                {
                    globalcurrentprojfile = PathSW.GetFullPath(globalcurrentprojfile);

                    // Removing current project file that finished from stack.
                    projectfilestack.Pop();

                    LogToBuildLog("Compiling project file - " + globalcurrentprojfile + " finished.");
                }

            }

            tabcount = 0;
            this.LogToBuildLog(e, true);
        }

        /// <summary>
        /// Error build event handler.
        /// If the error is marked as ignoreable from error file or ignore from commandline,
        /// find the error, check if it is ignoreable and based on returned value
        /// log to Build error log file.
        /// If error was an unexpected error set buildresult flag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_ErrorEvent(object sender, BuildErrorEventArgs e)
        {
            tabcount = 2;

            bool returnvalue = IsErrorMessageIgnorable(e, "error");
            if (returnvalue == false)
            //if (IsErrorMessageIgnorable(e, "error") == false)
            {
                this.LogToBuildError(e);
                _bbuiltwitherrors = true;
            }
            else
            {
                LogToBuildLog(e, false);
            }
        }

        /// <summary>
        /// Comment Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_CommentEvent(object sender, BuildEventArgs e)
        {
            tabcount = 2;

            // Do not show comments when doing perf analysis.
            this.LogToBuildLog(e, false);
        }

        /// <summary>
        /// Comment Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void eventSource_StatusEventRaised(object sender, BuildStatusEventArgs e)
        {
            //this.LogToBuildLog(e, false);
        }

        /// <summary>
        /// Message Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void eventSource_MessageRaised(object sender, BuildMessageEventArgs e)
        {
            //tabcount = 3;            
            this.LogToBuildLog(e, false);
        }

        /// <summary>
        /// Target Started Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_TargetStartedEvent(object sender, TargetStartedEventArgs e)
        {
            tabcount = 0;
            this.LogToBuildLog(e.Timestamp.ToString() + " - " + "Target " + e.TargetName + ":");
        }

        /// <summary>
        /// Target Finished Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_TargetFinishedEvent(object sender, TargetFinishedEventArgs e)
        {
            tabcount = 0;
            this.LogToBuildLog(e, false);
        }

        /// <summary>
        /// Task Started Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_TaskStartedEvent(object sender, TaskStartedEventArgs e)
        {
            tabcount = 1;
            this.LogToBuildLog((BuildEventArgs)e, false);
        }

        /// <summary>
        /// Task Finished Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_TaskFinishedEvent(object sender, TaskFinishedEventArgs e)
        {
            tabcount = 1;
            this.LogToBuildLog((BuildEventArgs)e, false);

        }

        /// <summary>
        /// Warning build event handler.
        /// If the warning is marked as ignoreable from error file or ignore from commandline,
        /// find the warning, check if it is ignoreable and based on returned value
        /// log to Build warning log file.
        /// If warning was an unexpected warning set buildresult flag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_WarningEvent(object sender, BuildWarningEventArgs e)
        {
            tabcount = 2;

            bool returnvalue = IsErrorMessageIgnorable(e, "warning");
            if (returnvalue == false)
            {
                LogToBuildWarning(e);
                _bbuiltwithwarnings = true;
            }
            else
            {
                LogToBuildLog(e, false);
            }
        }

        /// <summary>
        /// Build Finished Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_BuildFinishedEvent(object sender, BuildFinishedEventArgs e)
        {
            LogToBuildLog(e.Message);

            if (logmemorystream != null)
            {
                MSBuildEngineCommonHelper.WritetoFilefromMemory(ref logmemorystream, this.buildlogfile);
            }
            if (errormemorystream != null)
            {
                MSBuildEngineCommonHelper.WritetoFilefromMemory(ref errormemorystream, this.builderrorfile);
            }
            if (warningmemorystream != null)
            {
                MSBuildEngineCommonHelper.WritetoFilefromMemory(ref warningmemorystream, this.buildwaringfile);
            }

            projectfilestack = null;
            currentduplicateerrorwarningid = null;
        }

        #endregion Event Handlers

        #region Private Methods
        /// <summary>
        /// Method checks if the error/warning exists in the ErrorWarnings list using error id.
        /// If the error is found verify description depending on IsPartial attribute value.
        /// Also if error is found check if IsIgnoreable attribute set to true
        /// </summary>
        /// <param name="e">BuildEventArgs object from MSBuild</param>
        /// <param name="errortype">Error or Warning</param>
        /// <returns></returns>
        private bool IsErrorMessageIgnorable(BuildEventArgs e, string errortype)
        {
            MSBuildEngineCommonHelper.LogDiagnostic = "Executing Find Error message";
            if (expectederrors == null && _berrorhandling == true)
            {
                // This is when there is no error file specified.
                return false;
            }

            if (String.IsNullOrEmpty(e.Message))
            {
                return false;
            }

            string errorid = null;
            BuildWarningEventArgs warningeventargs = null;
            BuildErrorEventArgs erroreventargs = e as BuildErrorEventArgs;
            if (erroreventargs == null)
            {
                warningeventargs = e as BuildWarningEventArgs;
                if (warningeventargs == null)
                {
                    throw new ApplicationException("Type of Build event could not be determined.");
                }

                errorid = warningeventargs.Code;
            }
            else
            {
                errorid = erroreventargs.Code;
            }

            ErrorWarningCode actualerrorwarning = new ErrorWarningCode(e);
            if (_listofhandlederrors == null)
            {
                _listofhandlederrors = new List<ErrorWarningCode>();
            }

            _listofhandlederrors.Add(actualerrorwarning);

            if (!_berrorhandling)
            {
                return false;
            }

            if (String.IsNullOrEmpty(errorid))
            {
                errorid = actualerrorwarning.ID;
            }

            // Retired.
            //if (String.IsNullOrEmpty(errorid))
            //{
            //    // Currently in Avalon the implementation is not in line with reporting 
            //    // Code ID, Line #, Position and so on.
            //    // Adding work around here. 
            //    //MSBuildEngineCommonHelper.Log = "Could not find Error/Warning with ID - " + errorid;

            //    return IsAvalonErrorMessageIgnorable(e, errortype);
            //    //return false;
            //}

            if (String.IsNullOrEmpty(errorid))
            {
                if (CheckUnassignedDescriptionExists(e.Message) == false)
                {
                    //if (listofhandlederrors == null)
                    //{
                    //    listofhandlederrors = new List<string>();
                    //}

                    //if (listofhandlederrors.Contains(currentduplicateerrorwarningid) == false)
                    //{
                    //    listofhandlederrors.Add(e.Message);
                    //}
                    MSBuildEngineCommonHelper.LogError = "Build error/warning should contain an error ID.";
                    return false;
                }
                else
                {
                    if (listoferrorwarningswithoutid != null)
                    {
                        for (int i = 0; i < listoferrorwarningswithoutid.Count; i++)
                        {
                            string erroridwithoutdescription = listoferrorwarningswithoutid[i];
                            if (erroridwithoutdescription.ToLowerInvariant().Contains("_Dup".ToLowerInvariant()))
                            {
                                int index = erroridwithoutdescription.ToLowerInvariant().IndexOf("_Dup".ToLowerInvariant());
                                erroridwithoutdescription = erroridwithoutdescription.Substring(0, index);
                            }

                            if (listoferrorwarningswithoutid.Contains(erroridwithoutdescription))
                            {
                                listoferrorwarningswithoutid.RemoveAt(i);

                                if (listoferrorwarningswithoutid.Count > 0 && i > 0)
                                {
                                    i--;
                                }

                                _listofhandlederrors.Remove(actualerrorwarning);
                                //break;
                            }
                        }

                        // Todo:                        

                    }
                    return true;
                }
            }

            if (IsDuplicated(errorid.ToUpper()))
            {
                if (IsDuplicatedErrorMessageIgnorable(ref actualerrorwarning))
                {
                    if (_listofhandlederrors != null && _listofhandlederrors.Contains(currentduplicateerrorwarningid))
                    //if (_listofhandlederrors != null)
                    {
                        //for (int i = 0; i < _listofhandlederrors.Count; i++)
                        //{
                        //    if (_listofhandlederrors[i].Equals(currentduplicateerrorwarningid))
                        //    {
                        _listofhandlederrors.Remove(currentduplicateerrorwarningid);
                        //    }
                        //}                        
                    }
                    currentduplicateerrorwarningid = null;
                    return true;
                }
                else
                {
                    currentduplicateerrorwarningid = null;
                    return false;
                }
            }

            if (expectederrors[errorid.ToUpper()] == null)
            {
                return false;
            }

            // Get object based on error id.
            ErrorWarningCode desirederrorwarning = (ErrorWarningCode)expectederrors[errorid.ToUpper()];
            if (desirederrorwarning == null)
            {
                MSBuildEngineCommonHelper.Log = "Could not find Error/Warning with ID - " + errorid;
                return false;
            }

            if (actualerrorwarning.Equals(desirederrorwarning) == false)
            {
                return false;
            }

            if (_listofhandlederrors != null && _listofhandlederrors.Count > 0 &&
                _listofhandlederrors.Contains(actualerrorwarning))
            {
                _listofhandlederrors.Remove(actualerrorwarning);
            }

            actualerrorwarning = null;
            return desirederrorwarning.IsIgnoreable;
        }

        /// <summary>
        /// This is specific handling for duplicated Error/Warning ID's.
        /// </summary>
        /// <param name="actualerrorwarning"></param>
        /// <returns></returns>
        private bool IsDuplicatedErrorMessageIgnorable(ref ErrorWarningCode actualerrorwarning)
        {
            if (actualerrorwarning == null)
            {
                return false;
            }

            ErrorWarningCode tempewc = null;
            IDictionaryEnumerator enumerator = expectederrors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().StartsWith(actualerrorwarning.ID))
                {
                    tempewc = (ErrorWarningCode)enumerator.Value;
                    if (tempewc == null)
                    {
                        tempewc = null;
                        continue;
                    }

                    if (String.IsNullOrEmpty(tempewc.Description))
                    {
                        tempewc = null;
                        continue;
                    }

                    if (actualerrorwarning.Description.Contains(tempewc.Description))
                    {
                        break;
                    }

                    if (ErrorWarningCode.Compare(tempewc.description, actualerrorwarning.Description))
                    {
                        currentduplicateerrorwarningid = actualerrorwarning;
                        tempewc = null;
                        enumerator = null;
                        return true;
                    }

                    tempewc = null;
                }
            }

            // Should never happen!
            if (tempewc == null)
            {
                return false;
            }

            if (actualerrorwarning.Equals(tempewc) == false)
            {
                return false;
            }

            currentduplicateerrorwarningid = tempewc;
            tempewc = null;
            enumerator = null;

            return currentduplicateerrorwarningid.IsIgnoreable;
        }

        /// <summary>
        /// Check if more than one Error/Warning was specified with the same ID.
        /// </summary>
        /// <param name="errorid"></param>
        /// <returns></returns>
        private bool IsDuplicated(string errorid)
        {
            if (String.IsNullOrEmpty(errorid))
            {
                return false;
            }

            int numberofduplicates = 0;

            IDictionaryEnumerator enumerator = expectederrors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().StartsWith(errorid))
                {
                    numberofduplicates++;
                }
            }

            if (numberofduplicates <= 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Overloaded Build log method, logs to buildlogfile.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="binsertnewline"></param>
        private void LogToBuildLog(BuildEventArgs e, bool binsertnewline)
        {
            BuildWarningEventArgs warningeventargs = null;
            BuildErrorEventArgs erroreventargs = e as BuildErrorEventArgs;

            string currentprojectfile = null;
            StringBuilder sb = new StringBuilder();
            string filename = null, errorid = null;
            int linenumber = -1, columnnumber = -1;

            if (erroreventargs == null)
            {
                warningeventargs = e as BuildWarningEventArgs;
                if (warningeventargs == null)
                {
                    goto NonWarningErrorEvent;
                }

                filename = currentprojectfile;
                if (warningeventargs.LineNumber >= 0 && warningeventargs.ColumnNumber >= 0)
                {
                    linenumber = warningeventargs.LineNumber;
                    columnnumber = warningeventargs.ColumnNumber;
                }

                if (String.IsNullOrEmpty(warningeventargs.Code) == false)
                {
                    errorid = warningeventargs.Code;
                }
            }
            else
            {
                filename = erroreventargs.File;
                if (erroreventargs.LineNumber >= 0 && erroreventargs.ColumnNumber >= 0)
                {
                    linenumber = erroreventargs.LineNumber;
                    columnnumber = erroreventargs.ColumnNumber;
                }

                if (String.IsNullOrEmpty(erroreventargs.Code) == false)
                {
                    errorid = erroreventargs.Code;
                }
            }

            if (projectfilestack.Count == 0)
            {
                if (String.IsNullOrEmpty(globalcurrentprojfile) == false)
                {
                    currentprojectfile = globalcurrentprojfile;
                }
            }
            else
            {
                currentprojectfile = projectfilestack.Peek().ToString();
            }

            if (filename != null && String.IsNullOrEmpty(currentprojectfile) == false &&
                filename.ToLowerInvariant() != PathSW.GetFileName(currentprojectfile).ToLowerInvariant())
            {
                sb.Append(filename.ToString() + " ");
                if (linenumber >= 0 && columnnumber >= 0)
                {
                    sb.Append("(" + linenumber + "," + columnnumber + ") " + " :");
                }
            }

            if (errorid != null)
            {
                sb.Append(" warning " + errorid + ": ");
            }

        NonWarningErrorEvent:

            //if (_btimestamp && baddtimestamp)
            //{
            //    //sb.AppendFormat("{0} ms: {1} - ", e.Timestamp.ToString(), e.Timestamp.Millisecond.ToString()); ;
            //    sb.AppendFormat("{0}:{1}:{2}:{3} - ", e.Timestamp.Hour.ToString(), e.Timestamp.Minute.ToString(), e.Timestamp.Second.ToString(), e.Timestamp.Millisecond.ToString());
            //}

            sb.Append(e.Message);

            StringBuilder tab = new StringBuilder();
            for (int i = 0; i < tabcount; i++)
            {
                tab.Append("\t");
            }

            LogToBuildLog(tab.ToString() + sb.ToString());
            if (binsertnewline)
            {
                LogToBuildLog("");
            }

            sb = null;
            tab = null;
        }

        /// <summary>
        /// Overloaded method does the actual logging to file.
        /// </summary>
        /// <param name="message"></param>
        private void LogToBuildLog(string message)
        {
            MSBuildEngineCommonHelper.Log = message;
            if (String.IsNullOrEmpty(buildlogfile))
            {
                return;
            }

            MSBuildEngineCommonHelper.SaveToMemory(message, ref logmemorystream, buildlogfile);
            //buildlog = new StreamWriter(buildlogfile, true);
            //if (String.IsNullOrEmpty(message))
            //{
            //    buildlog.WriteLine();
            //}
            //else
            //{
            //    buildlog.WriteLine(message);
            //}

            //buildlog.Close();
            //buildlog = null;
        }

        /// <summary>
        /// Log to Build error file and to Build Log file.
        /// </summary>
        /// <param name="e"></param>
        private void LogToBuildError(BuildErrorEventArgs e)
        {
            if (e != null)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder tab = new StringBuilder();

                StringBuilder errorstring = new StringBuilder();
                //if (String.IsNullOrEmpty(builderrorfile) == false)
                //{
                //    builderror = new StreamWriter(builderrorfile, true);
                //}

                for (int i = 0; i < tabcount; i++)
                {
                    tab.Append("\t");
                }

                string currentprojfile = null;
                if (projectfilestack.Count > 0)
                {
                    currentprojfile = projectfilestack.Peek().ToString();
                }

                if (e.File != null && e.File.ToLowerInvariant() != currentprojfile)
                {
                    sb.Append(e.File.ToString() + " ");
                    //if (builderror != null)
                    //{
                    //						builderror.Write(PathSW.GetFullPath(e.File.ToString()) + " ");
                    errorstring.Append(currentprojfile + " : " + e.File.ToString() + " ");
                    //MSBuildEngineCommonHelper.SaveToMemory(currentprojfile + " : " + e.File.ToString() + " ", ref errormemorystream, builderrorfile);
                    //builderror.Write();
                    //}

                    if (e.LineNumber >= 0 && e.ColumnNumber >= 0)
                    {
                        sb.Append("(" + e.LineNumber + "," + e.ColumnNumber + ")" + " :");
                        errorstring.Append("(" + e.LineNumber + "," + e.ColumnNumber + ")" + " :");
                        //MSBuildEngineCommonHelper.SaveToMemory("(" + e.LineNumber + "," + e.ColumnNumber + ")" + " :", ref errormemorystream, builderrorfile);
                        //if (builderror != null)
                        //{
                        //    builderror.Write("(" + e.LineNumber + "," + e.ColumnNumber + ")" + " :");
                        //}
                    }
                }
                else
                {
                    sb.AppendFormat("{0} - ", PathSW.GetFileName(currentprojfile));
                    //if (builderror != null)
                    //{
                    //    builderror.Write("{0} - ", currentprojfile);
                    //}
                    errorstring.AppendFormat("{0} - " + currentprojfile);
                    //MSBuildEngineCommonHelper.SaveToMemory("{0} - " + currentprojfile, ref errormemorystream, builderrorfile);
                }

                if (e.Code != null)
                {
                    sb.Append(" error " + e.Code + ": ");
                    //if (builderror != null)
                    //{
                    //    builderror.Write(" error " + e.Code + ": ");
                    //}
                    errorstring.Append(" error " + e.Code + ": ");
                    //MSBuildEngineCommonHelper.SaveToMemory(" error " + e.Code + ": ", ref errormemorystream, builderrorfile);
                }

                sb.Append(e.Message);
                //if (builderror != null)
                //{
                //    //builderror.Write(e.Message);
                //    //builderror.WriteLine();
                //}
                errorstring.Append(e.Message);

                if (String.IsNullOrEmpty(builderrorfile) == false)
                {
                    MSBuildEngineCommonHelper.SaveToMemory(errorstring.ToString(), ref errormemorystream, builderrorfile);

                    //if (builderror != null)
                    //{
                    //    builderror.Close();
                    //    builderror = null;
                    //}

                    MSBuildEngineCommonHelper.WritetoFilefromMemory(ref errormemorystream, builderrorfile);
                }

                MSBuildEngineCommonHelper.LogError = sb.ToString();
                LogToBuildLog(tab.ToString() + sb.ToString());

                sb = null;
                tab = null;
            }
        }

        /// <summary>
        /// Log to Build warning file and to Build Log file.
        /// </summary>
        /// <param name="e"></param>
        private void LogToBuildWarning(BuildWarningEventArgs e)
        {
            if (e != null)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder tab = new StringBuilder();
                StringBuilder warningstring = new StringBuilder();

                //if (String.IsNullOrEmpty(buildwaringfile) == false)
                //{
                //    buildwarning = new StreamWriter(buildwaringfile, true);
                //}

                for (int i = 0; i < tabcount; i++)
                {
                    tab.Append("\t");
                }

                //sb.AppendFormat("{0} - ",e.Timestamp.ToString());

                string currentprojfile = null;
                if (projectfilestack.Count > 0)
                {
                    currentprojfile = projectfilestack.Peek().ToString();
                }

                if (e.File != null && e.File.ToLowerInvariant() != currentprojfile)
                {
                    sb.Append(e.File.ToString() + " ");
                    //if (buildwarning != null)
                    //{
                    //						buildwarning.Write(PathSW.GetFullPath(e.File.ToString()) + " ");
                    //buildwarning.Write(currentprojfile + " : " + e.File.ToString() + " ");
                    warningstring.Append(currentprojfile + " : " + e.File.ToString() + " ");
                    //}

                    if (e.LineNumber >= 0 && e.ColumnNumber >= 0)
                    {
                        sb.Append("(" + e.LineNumber + "," + e.ColumnNumber + ") " + " :");
                        //if (buildwarning != null)
                        //{
                        //    buildwarning.Write("(" + e.LineNumber + "," + e.ColumnNumber + ") " + " :");
                        //}
                        warningstring.Append("(" + e.LineNumber + "," + e.ColumnNumber + ") " + " :");
                    }
                }
                else
                {
                    sb.AppendFormat("{0} - :", PathSW.GetFileName(currentprojfile));
                    //if (buildwarning != null)
                    //{
                    //    buildwarning.Write("{0} - :", currentprojfile);
                    //}
                    warningstring.AppendFormat("{0} - :", currentprojfile);
                }

                if (e.Code != null)
                {
                    sb.Append(" warning " + e.Code + ": ");
                    //if (buildwarning != null)
                    //{
                    //    buildwarning.Write(" warning " + e.Code + ": ");
                    //}
                    warningstring.Append(" warning " + e.Code + ": ");
                }

                sb.Append(e.Message);
                //if (buildwarning != null)
                //{
                //    buildwarning.Write(e.Message);
                //    buildwarning.WriteLine();
                //}
                warningstring.Append(e.Message + "\n");

                //if (buildwarning != null)
                //{
                //    //buildwarning.WriteLine(sb.ToString());
                //    buildwarning.Close();
                //    buildwarning = null;
                //}

                if (String.IsNullOrEmpty(buildwaringfile) == false)
                {
                    MSBuildEngineCommonHelper.SaveToMemory(warningstring.ToString(), ref warningmemorystream, buildwaringfile);
                    MSBuildEngineCommonHelper.WritetoFilefromMemory(ref warningmemorystream, buildwaringfile);
                }

                MSBuildEngineCommonHelper.LogWarning = sb.ToString();
                LogToBuildLog(tab.ToString() + sb.ToString());

                sb = null;
                tab = null;
            }
        }

        #endregion Private Methods

        #region Internal Properties

        /// <summary>
        /// List of Expected Errors/Warnings kept internal to the Logger.
        /// </summary>
        /// <value></value>
        public Hashtable ExpectedErrors
        {
            set
            {
                if (value != null)
                {
                    expectederrors = (Hashtable)value.Clone();
                }
            }
        }

        #endregion Internal Properties

        #region Internal Methods

        /// <summary>
        /// Some errors do not come with Error ID's or Warning ID's. 
        /// Example MSBuild &lt;Error&gt;&lt;/Error&gt; &amp; &lt;Warning&gt;&lt;/Warning&gt; elements.
        /// To support this I am creating this hack to check only the description based 
        /// on Partial flag set to false.
        /// </summary>
        /// <param name="currentdescription"></param>
        /// <returns></returns>
        internal bool CheckUnassignedDescriptionExists(string currentdescription)
        {
            if (expectederrors == null || expectederrors.Count == 0)
            {
                return false;
            }

            IDictionaryEnumerator enumerator = expectederrors.GetEnumerator();

            while (enumerator.MoveNext())
            {
                ErrorWarningCode ewc = (ErrorWarningCode)enumerator.Value;
                if (String.IsNullOrEmpty(ewc.Description) == false)
                {
                    if (ewc.Description.ToLowerInvariant() == currentdescription.ToLowerInvariant())
                    {
                        if (ewc.IsIgnoreable)
                        {
                            ewc = null;
                            goto SuccessFullExit;
                        }
                    }
                    else if (currentdescription.ToLowerInvariant().Contains(ewc.Description.ToLowerInvariant()))
                    {
                        if (ewc.IsIgnoreable && ewc.IsPartial)
                        {
                            ewc = null;
                            goto SuccessFullExit;
                        }
                    }
                    else if (ErrorWarningCode.Compare(ewc.description, currentdescription))
                    {
                        if (ewc.IsIgnoreable)
                        {
                            ewc = null;
                            goto SuccessFullExit;
                        }
                    }
                }
            }

            enumerator = null;
            return false;

        SuccessFullExit:
            enumerator = null;
            return true;
        }

        /// <summary>
        /// Set a warning or error in the Error XML file as ignoreable dynamically.
        /// This method is used to set existing parsed errors + warnings from the error file
        /// as ignoreable.
        /// This enables the testing where certain error's or warnings can be ignored.
        /// </summary>
        /// <param name="errorid">Error/Warning ID that exists in Error file</param>
        /// <param name="ignoreable">Boolean value to set Ignoreable property on Error/Warning</param>
        internal void SetErrorWarningAsIgnoreable(string errorid, bool ignoreable)
        {
            if (String.IsNullOrEmpty(errorid))
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "SetErrorWarningAsIgnoreable - Input paramter errorid is null";
                return;
            }

            if (listoferrorwarningswithoutid == null)
            {
                listoferrorwarningswithoutid = new List<string>();
            }

            if (errorid.StartsWith("LH") || errorid.StartsWith("UN"))
            {
                listoferrorwarningswithoutid.Add(errorid.ToUpper());
            }

            ErrorWarningCode desiredewc = (ErrorWarningCode)expectederrors[errorid.ToUpper()];
            if (desiredewc == null)
            {
                MSBuildEngineCommonHelper.Log = "Could not find error/warning for corresponding error id - " + errorid;
                return;
            }

            // If the error/warning is already set to ignore don't do anything.
            if (desiredewc.IsIgnoreable)
            {
                MSBuildEngineCommonHelper.Log = "SetErrorWarningAsIgnoreable - Current error/warning is set to Ignoreable already.";
                return;
            }

            expectederrors.Remove(errorid.ToUpper());
            desiredewc.IsIgnoreable = ignoreable;
            expectederrors.Add(errorid.ToUpper(), desiredewc);
        }

        #endregion Internal Methods

    }
}
