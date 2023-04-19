using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.CompilerServices.Logging
{
    public abstract class BuildStatus
    {
        protected string code;
        protected int columnNumber;
        protected int endColumnNumber;
        protected int endLineNumber;
        protected string file;
        protected int lineNumber;
        protected string subcategory;
        protected string message;


        /// <summary>
        /// Compares two BuildStatuses
        /// </summary>
        /// <param name="firstStatus"></param>
        /// <param name="secondStatus"></param>
        /// <param name="deepComparison">Determines whether to compare all properties or just Code and LineNumber</param>
        /// <returns></returns>
        public static bool Compare(BuildStatus firstStatus, BuildStatus secondStatus, bool deepComparison)
        {
            if (deepComparison)
            {
                return (String.Equals(firstStatus.Code, secondStatus.Code, StringComparison.InvariantCultureIgnoreCase)
                    && (firstStatus.ColumnNumber == secondStatus.ColumnNumber)
                    && (firstStatus.EndColumnNumber == secondStatus.EndColumnNumber)
                    && (firstStatus.EndLineNumber == secondStatus.EndLineNumber)
                    && String.Equals(firstStatus.File, secondStatus.File, StringComparison.InvariantCultureIgnoreCase)
                    && (firstStatus.LineNumber == secondStatus.LineNumber)
                    && String.Equals(firstStatus.Subcategory, secondStatus.Subcategory, StringComparison.InvariantCultureIgnoreCase));
            }
            return String.Equals(firstStatus.Code, secondStatus.Code, StringComparison.InvariantCultureIgnoreCase)
                // Below checks are a workaround for the issue with pseudo-localized builds. 
                // When expecting MCXXXX, but we are getting MC1000 with “MCXXXX” present inside the error message. 
                // This is because markup compiler expects the error message strings to begin with a valid error number. 
                // But this assumption is broken by the pseudoloc process that appends random characters before and 
                // after all resource strings. So we the markup compiler gets confused and throws “MC1000: unknown build error”
                || (firstStatus.Code == "MC1000" && firstStatus.Message.Contains(secondStatus.Code))    
                || (secondStatus.Code == "MC1000" && secondStatus.Message.Contains(firstStatus.Code));
        } 

        #region Public Properties
        /// <summary/>
        public string Code
        {
            get { return code; }
        }

        public int ColumnNumber
        {
            get { return columnNumber; }
        }

        public int EndColumnNumber
        {
            get { return endColumnNumber; }
        }

        public int EndLineNumber
        {
            get { return endLineNumber; }
        }

        public string File
        {
            get { return file; }
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public string Subcategory
        {
            get { return subcategory; }
        }

        public string Message
        {
            get { return message; }
        }
        #endregion
    }
}
