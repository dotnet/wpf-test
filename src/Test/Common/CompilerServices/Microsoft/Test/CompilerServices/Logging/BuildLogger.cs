using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;

namespace Microsoft.Test.CompilerServices.Logging
{
    /// <summary>
    /// Wrapper class for the BuildLoggerInternal which is of type Microsoft.Build.Framework.ILogger
    /// This hides ILogger, which cannot be used on an Arrowhead machine
    /// </summary>
    public class BuildLogger
    {
        private BuildLoggerInternal internalLog;

        public BuildLogger()
        {
            internalLog = new BuildLoggerInternal();
        }

        #region Public Properties

        /// <summary>
        /// This object is gauranteed to alway be of type Microsoft.Build.Framework.ILogger
        /// Boxing is required to allow this type to be loaded on an Arrowhead machine
        /// </summary>
        public object InnerObject
        {
            get { return internalLog as object; }
        }

        /// <summary>
        /// File to save the build log to
        /// </summary>
        public string BuildLogPath
        {
            get { return internalLog.BuildLogPath; }
            set { internalLog.BuildLogPath = value; }
        }

        /// <summary>
        /// Collection of errors encountered during the build process
        /// </summary>
        public BuildStatusCollection BuildErrors
        {
            get { return internalLog.BuildErrors; }
        }

        /// <summary>
        /// Collection of warnings encountered during the build process
        /// </summary>
        public BuildStatusCollection BuildWarnings
        {
            get { return internalLog.BuildWarnings; }
        }   

        /// <summary>
        /// Build log detail level 
        /// Defaults to Normal.
        /// </summary>
        public LogVerbosity Verbosity
        {
            get { return (LogVerbosity)Enum.Parse(typeof(LogVerbosity), internalLog.Verbosity.ToString()); }
            set { internalLog.Verbosity = (LoggerVerbosity)Enum.Parse(typeof(LoggerVerbosity), value.ToString()); }
        }
        #endregion
    }


}
