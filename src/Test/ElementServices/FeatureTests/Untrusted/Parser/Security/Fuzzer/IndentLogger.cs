// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Used to log with indenting.
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.IO;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// Logs with indenting.  Maintains indent state.
    /// </summary>
    public class IndentLogger
    {
        /// <summary>
        /// Logger that indent
        /// </summary>
        /// <param name="fileName">File that will receive logging output</param>
        /// <param name="indentIncrement">Number of spaces to increment each time</param>
        public IndentLogger(String fileName, int indentIncrement)
        {
            this._fileName = fileName;
            if (indentIncrement < 0)
            {
                throw new ApplicationException("Indent increment for IndentLogger must be nonnegative.");
            }
            this._indentIncrement = indentIncrement;
            this._indentString = "";
            this._fileInfo = new FileInfo(fileName);
            this._fileStream = _fileInfo.Open(FileMode.OpenOrCreate);
            this._streamWriter = new StreamWriter(_fileStream);
            this._isOpen = true;
            
        }

        /// <summary>
        /// Move indent inward by indentIncrement.  Everything logged after this call will be impacted.
        /// </summary>
        public void Indent()
        {
            _indent += _indentIncrement;
            if (_indent < 0)
            {
                _indent = 0;
            }
            _indentString = "".PadLeft(_indent); 

        }

        /// <summary>
        /// Reverse indent by indentIncrement.  Everything logged after this call will be impacted.
        /// </summary>
        public void Outdent()
        {
            _indent -= _indentIncrement;
            if (_indent < 0)
            {
                _indent = 0;
            }
            _indentString = "".PadLeft(_indent);
        }

        /// <summary>
        /// Write a line to the log.  
        /// </summary>
        /// <param name="outputString"></param>
        public void Log(String outputString)
        {
            _streamWriter.WriteLine(_indentString + outputString);
            _streamWriter.Flush();
            _fileStream.Flush();
        }

        /// <summary>
        /// Get the name of the file that is written by this logger.  
        /// </summary>
        public String GetFilename()
        {
            return _fileName;
        }

        /// <summary>
        /// Close the logger and its files.  Allows repeated close attempts.
        /// </summary>
        public void Close()
        {
            // some redundant flushing...
            // also closes fileStream?...
            if (_isOpen)
            {
                _fileStream.Flush();
                _streamWriter.Flush();
                _streamWriter.Close();
                _isOpen = false;
            }
        }

        private StreamWriter _streamWriter;
        private FileStream _fileStream;
        private String _indentString;
        private int _indentIncrement = 0;
        private int _indent = 0;
        private FileInfo _fileInfo;
        private String _fileName;
        private Boolean _isOpen;
    }
}

