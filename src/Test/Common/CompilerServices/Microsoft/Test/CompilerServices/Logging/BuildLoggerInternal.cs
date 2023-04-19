using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;
using System.IO;

namespace Microsoft.Test.CompilerServices.Logging
{
    /// <summary>
    /// Logger for MSBuild compilation. This is kept internal to prevent
    /// users from accessing MSBuild types directly
    /// </summary>
    internal class BuildLoggerInternal : Microsoft.Build.Logging.FileLogger
    {
        #region Private Fields

        private string buildLogPath = "_BuildLog.txt";
        private BuildStatusCollection buildErrors;
        private BuildStatusCollection buildWarnings;

        #endregion
                
        internal BuildLoggerInternal()
        {
            Verbosity = LoggerVerbosity.Diagnostic;
            Parameters = "logfile=" + buildLogPath;
            buildErrors = new BuildStatusCollection();
            buildWarnings = new BuildStatusCollection();
        }

        #region Public Properties

        /// <summary>
        /// File to save the build log to
        /// </summary>
        public string BuildLogPath
        {
            get { return buildLogPath; }
            set
            {
                buildLogPath = value;
                Parameters = "logfile=" + buildLogPath;
            }
        }

        /// <summary>
        /// Collection of errors encountered during the build process
        /// </summary>
        public BuildStatusCollection BuildErrors
        {
            get { return buildErrors; }
        }

        /// <summary>
        /// Collection of warnings encountered during the build process
        /// </summary>
        public BuildStatusCollection BuildWarnings
        {
            get { return buildWarnings; }
        }

        #endregion

        #region FileLogger overrides

        /// <summary>
        /// Sets up the event handlers for MSBuild logging
        /// hooks up different sets of handles based on the verbosity 
        /// desired
        /// </summary>
        /// <param name="eventSource"></param>
        public override void Initialize(IEventSource eventSource)
        {
            base.Initialize(eventSource);
            eventSource.ErrorRaised += new BuildErrorEventHandler(SaveErrorAndCallHandler);
            eventSource.WarningRaised += new BuildWarningEventHandler(SaveWarningAndCallHandler);
        }

        /// <summary>
        /// Sets up the event handlers for MSBuild logging
        /// hooks up different sets of handles based on the verbosity 
        /// desired
        /// </summary>
        /// <param name="eventSource"></param>
        public override void Initialize(IEventSource eventSource, int nodeCount)
        {
            base.Initialize(eventSource, nodeCount);
            eventSource.ErrorRaised += new BuildErrorEventHandler(SaveErrorAndCallHandler);
            eventSource.WarningRaised += new BuildWarningEventHandler(SaveWarningAndCallHandler);
        }

        #endregion

        private void SaveErrorAndCallHandler(object sender, BuildErrorEventArgs e)
        {
            BuildError error = new BuildError(e);
            buildErrors.Add(error);
            ErrorHandler(sender, e);
        }

        private void SaveWarningAndCallHandler(object sender, BuildWarningEventArgs e)
        {
            BuildWarning warning = new BuildWarning(e);
            buildWarnings.Add(warning);
            WarningHandler(sender, e);
        }
    }
}
