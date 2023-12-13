// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a TraceListener that hooks up to a logger.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 5 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Common/Library/Loggers/Loggers.cs $")]

namespace Test.Uis.Loggers
{
    #region Namespaces.

    using System;
    using System.Diagnostics;
    using System.Collections;
    using System.Text;

    #endregion Namespaces.

    /// <summary>
    /// Provides a listener that monitors trace and outputs
    /// to a logger.
    /// </summary>
    class LoggerTraceListener: TraceListener
    {

        #region Constructors.

        /// <summary>
        /// Initializes a new LoggerTraceListener instance.
        /// </summary>
        /// <param name="logger">Logger to trace to.</param>
        internal LoggerTraceListener(Logger logger)
            : base("LoggerTraceListener")
        {
            System.Diagnostics.Debug.Assert(logger != null);
            _logger = logger;
        }

        #endregion Constructors.


        #region Public methods.

        /// <summary>Flushes the output buffer.</summary>
        public override void Flush()
        {
            if (_line != null)
            {
                _logger.Log(_line);
                _line = null;
            }
        }

        /// <summary>Writes the specified message to the logger.</summary>
        /// <param name="message">Message to write to logger.</param>
        public override void Write(string message)
        {
            if (NeedIndent)
            {
                WriteIndent();
            }

            if (_line == null)
            {
                _line = message;
            }
            else
            {
                _line += message;
            }
        }

        /// <summary>Writes the specified message line to the logger.</summary>
        /// <param name="message">Message to write to logger.</param>
        public override void WriteLine(string message)
        {
            Write(message);
            Flush();
            NeedIndent = true;
        }

        #endregion Public methods.


        #region Private fields.

        /// <summary>Line being buffered.</summary>
        private string _line;

        /// <summary>Logger being used to output messages.</summary>
        private Logger _logger;

        #endregion Private fields.
    }
}
