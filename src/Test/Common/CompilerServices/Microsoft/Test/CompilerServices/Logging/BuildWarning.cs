using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;

namespace Microsoft.Test.CompilerServices.Logging
{
    /// <summary>
    /// Class to store build warning info
    /// </summary>
    public class BuildWarning : BuildStatus
    {
        /// <summary>
        /// Create from an BuildWarningEventArgs
        /// this is used by BuildLoggerInternal
        /// </summary>
        /// <param name="e"></param>
        internal BuildWarning(BuildWarningEventArgs e)
            :this(e.Code, e.ColumnNumber, e.EndColumnNumber, e.EndLineNumber, e.File, e.LineNumber, e.Subcategory, e.Message)
        { }

        public BuildWarning(string code, int lineNumber)
            : this(code, 0, 0, 0, String.Empty, lineNumber, String.Empty, String.Empty)
        { }

        public BuildWarning(string code, int columnNumber, int endColumnNumber, int endLineNumber, string file, int lineNumber, string subcategory, string message)
        {
            this.code = code;
            this.columnNumber = columnNumber;
            this.endColumnNumber = endColumnNumber;
            this.endLineNumber = endLineNumber;
            this.file = file;
            this.lineNumber = lineNumber;
            this.subcategory = subcategory;
            this.message = message;
        }

        
    }
}
