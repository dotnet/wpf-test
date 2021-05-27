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
    /// 
    /// </summary>
    internal class MSBuildPerfLogger : Microsoft.Build.Framework.ILogger
    {
        #region Member Variables
        //internal Stack projectfilestack = null;
        Stack projectfileperfstack = null;
        MSBuildProjectPerf projectperf = null;
        LoggerVerbosity verbosity;
        internal List<MSBuildProjectPerf> projectfilesperflist = null;
        #endregion Member Variables

        #region ILogger Overrides
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
            //eventSource.CustomEventRaised += new CustomBuildEventHandler(eventSource_CommentEvent);
            eventSource.TargetStarted += new TargetStartedEventHandler(eventSource_TargetStartedEvent);
            eventSource.TargetFinished += new TargetFinishedEventHandler(eventSource_TargetFinishedEvent);
            eventSource.TaskStarted += new TaskStartedEventHandler(eventSource_TaskStartedEvent);
            eventSource.TaskFinished += new TaskFinishedEventHandler(eventSource_TaskFinishedEvent);
            eventSource.WarningRaised += new BuildWarningEventHandler(eventSource_WarningEvent);
            //eventSource.MessageRaised += new BuildMessageEventHandler(eventSource_MessageRaised);
            //eventSource.StatusEventRaised += new BuildStatusEventHandler(eventSource_StatusEventRaised);

            //projectfilestack = new Stack();
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
        /// ILogger interface implementation.
        /// </summary>
        public void Shutdown()
        {
            MSBuildEngineCommonHelper.Log = "Closing MSBuild";
        }

        #endregion ILogger Overrides

        #region Private Event Handlers


        /// <summary>
        /// Project Started Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_BuildStartedEvent(object sender, BuildStartedEventArgs e)
        {
            //if (projectfileperfstack == null)
            //{
            //    projectfileperfstack = new Stack();
            //}
        }

        /// <summary>
        /// Project Started Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_ProjectStartedEvent(object sender, ProjectStartedEventArgs e)
        {
            if (projectfileperfstack == null)
            {
                projectfileperfstack = new Stack();
            }

            if (projectperf != null)
            {
                projectfileperfstack.Push(projectperf);
                projectperf = null;
            }

            //timestampsstack.Push(e.Timestamp);
            if (projectperf == null)
            {
                projectperf = new MSBuildProjectPerf();
            }

            projectperf.ProjectName = e.ProjectFile;
            projectperf.StartTime = e.Timestamp;
        }

        /// <summary>
        /// Project Finished Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_ProjectFinishedEvent(object sender, ProjectFinishedEventArgs e)
        {
            projectperf.EndTime = e.Timestamp;

            if (projectfilesperflist == null)
            {
                projectfilesperflist = new List<MSBuildProjectPerf>();
            }

            if (projectfileperfstack.Count > 0)
            {
                projectfilesperflist.Add(projectperf);
                //projectfilestack.Push(projectperf);
                projectperf = (MSBuildProjectPerf)projectfileperfstack.Pop();
            }
            //else
            //{
            //    projectfilestack.Push(projectperf);
            //}            
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
            Console.WriteLine(e.Message);
        }


        /// <summary>
        /// Target Started Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_TargetStartedEvent(object sender, TargetStartedEventArgs e)
        {
            projectperf.SetProjectTargetData(e.TargetName, false, e.Timestamp);
        }

        /// <summary>
        /// Target Finished Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_TargetFinishedEvent(object sender, TargetFinishedEventArgs e)
        {
            projectperf.SetProjectTargetData(e.TargetName, true, e.Timestamp);
        }

        /// <summary>
        /// Task Started Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_TaskStartedEvent(object sender, TaskStartedEventArgs e)
        {
            projectperf.SetProjectTargetTaskData(e.TaskName, false, e.Timestamp);
        }

        /// <summary>
        /// Task Finished Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_TaskFinishedEvent(object sender, TaskFinishedEventArgs e)
        {
            projectperf.SetProjectTargetTaskData(e.TaskName, true, e.Timestamp);
        }

        /// <summary>
        /// Warning build event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_WarningEvent(object sender, BuildWarningEventArgs e)
        {
            MSBuildEngineCommonHelper.LogWarning = e.Message;
        }

        /// <summary>
        /// Build Finished Event handler, logs to build log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventSource_BuildFinishedEvent(object sender, BuildFinishedEventArgs e)
        {
            if (projectfilesperflist != null)
            {
                projectfilesperflist.Add(projectperf);
            }

            this.projectfileperfstack = null;
        }

        #region Unused
        ///// <summary>
        ///// Build Started Event handler, logs to build log file.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void eventSource_BuildStartedEvent(object sender, BuildStartedEventArgs e)
        //{
        //}

        ///// <summary>
        ///// Comment Event handler, logs to build log file.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void eventSource_CommentEvent(object sender, BuildEventArgs e)
        //{
        //}

        ///// <summary>
        ///// Comment Event handler, logs to build log file.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void eventSource_StatusEventRaised(object sender, BuildStatusEventArgs e)
        //{
        //    //this.LogToBuildLog(e, false);
        //}

        ///// <summary>
        ///// Message Event handler, logs to build log file.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void eventSource_MessageRaised(object sender, BuildMessageEventArgs e)
        //{

        //}

        #endregion Unused

        #endregion Event Handlers

    }
}
