// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinFundamentalReports.Reports
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data;
    using System.Data.SqlClient;
    using System.Xml;

    using WinFundamentalReports;

    #endregion Namespaces.

    /// <summary>Base class for report generators.</summary>
    public abstract class ReportBase
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new ReportBase instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public ReportBase(XmlWriter excelWriter, XmlWriter wordWriter,
            FundamentalReportOptions options)
        {
            this._excelWriter = excelWriter;
            this._wordWriter = wordWriter;
            this._options = options;
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>StyleId for normal text in Word.</summary>
        public const string NormalStyleId = "Normal";

        /// <summary>StyleId for high-confidence work item cells.</summary>
        public const string ConfidentCellStyleId = "confident-style-id";

        /// <summary>StyleId for header cells in Excel.</summary>
        public const string HeaderCellStyleId = "header-style-id";

        /// <summary>StyleId for normal cells in Excel.</summary>
        public const string NormalCellStyleId = "Default";

        /// <summary>StyleId for percent cells in Excel.</summary>
        public const string PercentCellStyleId = "percent-style-id";

        /// <summary>StyleId for weekend date data in Excel.</summary>
        public const string WeekendDateStyleId = "wend-date-style";

        /// <summary>StyleId for weekend data in Excel.</summary>
        public const string WeekendStyleId = "wend-style";

        /// <summary>StyleId for past date data in Excel.</summary>
        public const string PastDateStyleId = "past-date-style";

        /// <summary>StyleId for past data in Excel.</summary>
        public const string PastStyleId = "past-style";

        /// <summary>StyleId for future or current data  in Excel.</summary>
        public const string FutureStyleId = "future-style";

        /// <summary>StyleId for future or current date data in Excel.</summary>
        public const string FutureDateStyleId = "future-date-style";

        /// <summary>StyleId for "other" task descriptions in Excel.</summary>
        public const string OtherWorkStyleId = "other-work-style";

        /// <summary>StyleId for schedule-fixed work items in Excel (eg vacations).</summary>
        public const string FixedWorkStyleId = "fixed-work-style";

        /// <summary>StyleId to call out problem cells in Excel.</summary>
        public const string ProblemStyleId = "problem-style";

        /// <summary>StyleId to call out problem cells in Excel.</summary>
        public const string PercentProblemStyleId = "percent-problem-style";

        #endregion Public properties.

        #region Public methods.

        /// <summary>Writes the report to the configured outputs.</summary>
        public virtual void WriteReport()
        {
            ValidateReportProperties();
        }

        #endregion Public methods.

        #region Protected methods.

        /// <summary>
        /// Runs a SQL query, and returns the value of the first field
        /// as a long.
        /// </summary>
        /// <param name='connectionString'>Full connection string to connect to.</param>
        /// <param name='queryText'>SQL statement to execute.</param>
        /// <returns>
        /// The value of the first field of the first record, as a long.
        /// </returns>
        protected long RunReportAsLong(string connectionString, string queryText)
        {
            object result;

            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (queryText == null)
            {
                throw new ArgumentNullException("queryText");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand query;

                connection.Open();
                query = new SqlCommand();
                query.Connection = connection;
                query.CommandText = queryText;
                result = query.ExecuteScalar();
            }
            if (result is int)
            {
                return (int)result;
            }
            else if (result is long)
            {
                return (long)result;
            }
            return Convert.ToInt64(result);
        }

        /// <summary>Writes the start of a new worksheet in the Excel document.</summary>
        /// <param name='name'>Name of worksheet to start.</param>
        /// <remarks>Balance this call with WriteEndWorksheet.</remarks>
        protected void WriteStartWorksheet(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            _excelWriter.WriteStartElement("ss:Worksheet");
            _excelWriter.WriteAttributeString("ss:Name", name);
            _excelWriter.WriteStartElement("ss:Table");
        }

        /// <summary>
        /// Writes the given value with the specified type in an Excel cell.
        /// </summary>
        /// <param name='dataType'>Name of Excel data type.</param>
        /// <param name='content'>Value to write.</param>
        protected void WriteCellAsData(string dataType, string content)
        {
            WriteCellAsData(dataType, content, null);
        }

        /// <summary>
        /// Writes the given value with the specified type in an Excel cell,
        /// with the specified style id.
        /// </summary>
        /// <param name='dataType'>Name of Excel data type.</param>
        /// <param name='content'>Value to write.</param>
        /// <param name='styleId'>ID of style to reference (null for none).</param>
        protected void WriteCellAsData(string dataType,
            string content, string styleId)
        {
            WriteCellAsDataWithComment(dataType, content, styleId, null);
        }

        /// <summary>
        /// Writes the given value with the specified type in an Excel cell,
        /// with the specified style id and a comment.
        /// </summary>
        /// <param name='dataType'>Name of Excel data type.</param>
        /// <param name='content'>Value to write.</param>
        /// <param name='styleId'>ID of style to reference (null for none).</param>
        /// <param name='htmlComment'>HTML string with comment (null for none).</param>
        protected void WriteCellAsDataWithComment(string dataType,
            string content, string styleId, string htmlComment)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (dataType == null)
            {
                throw new ArgumentNullException("dataType");
            }

            _excelWriter.WriteStartElement("ss:Cell");
            if (styleId != null)
            {
                _excelWriter.WriteAttributeString("ss:StyleID", styleId);
            }
            _excelWriter.WriteStartElement("ss:Data");
            _excelWriter.WriteAttributeString("ss:Type", dataType);
            _excelWriter.WriteString(content);
            _excelWriter.WriteEndElement();
            if (htmlComment != null)
            {
                _excelWriter.WriteStartElement("ss:Comment");
                _excelWriter.WriteStartElement("ss:Data");
                _excelWriter.WriteAttributeString("xmlns", "http://www.w3.org/TR/REC-html40");
                _excelWriter.WriteRaw(htmlComment);
                _excelWriter.WriteEndElement();
                _excelWriter.WriteEndElement();
            }
            _excelWriter.WriteEndElement();
        }

        /// <summary>Writes the given number in an Excel cell.</summary>
        /// <param name='content'>Value to write.</param>
        protected void WriteCellAsNumber(long content)
        {
            WriteCellAsData("Number", content.ToString());
        }

        /// <summary>Writes the given number in an Excel cell.</summary>
        /// <param name='content'>Value to write.</param>
        protected void WriteCellAsNumber(double content)
        {
            WriteCellAsData("Number", content.ToString());
        }

        /// <summary>Writes the given text in an percent Excel cell.</summary>
        /// <param name='content'>Value to write.</param>
        protected void WriteCellAsPercent(double content)
        {
            WriteCellAsData("Number", content.ToString(), PercentCellStyleId);
        }

        /// <summary>Writes the given text in an Excel cell.</summary>
        /// <param name='content'>Value to write.</param>
        protected void WriteCellAsString(string content)
        {
            WriteCellAsData("String", content);
        }

        /// <summary>Writes the specified column headers to Excel.</summary>
        /// <param name='columnHeaders'>Column titles to write.</param>
        protected void WriteHeaderRow(string[] columnHeaders)
        {
            WriteHeaderRow(columnHeaders, null);
        }

        /// <summary>Writes the specified column headers to Excel.</summary>
        /// <param name='columnHeaders'>Column titles to write.</param>
        /// <param name='widths'>Width of columns.</param>
        protected void WriteHeaderRow(string[] columnHeaders, int[] widths)
        {
            if (columnHeaders == null)
            {
                throw new ArgumentNullException("columnHeaders");
            }

            if (widths != null)
            {
                foreach(int width in widths)
                {
                    _excelWriter.WriteStartElement("ss:Column");
                    _excelWriter.WriteAttributeString("ss:Width", width.ToString());
                    // excelWriter.WriteAttributeString("ss:AutoFitWidth", "0");
                    _excelWriter.WriteEndElement();
                }
            }

            WriteStartRow();
            foreach(string columnHeader in columnHeaders)
            {
                WriteCellAsData("String", columnHeader, HeaderCellStyleId);
            }
            WriteEndRow();
        }

        /// <summary>Writes the start of a new row in the Excel document.</summary>
        /// <remarks>Balance this call with WriteEndRow.</remarks>
        protected void WriteStartRow()
        {
            _excelWriter.WriteStartElement("ss:Row");
        }

        /// <summary>Starts a new section in Word.</summary>
        /// <param name="name">Name of section to start.</param>
        protected void WriteStartSection(string name)
        {
            WriteStyledParagraph(name, false, false, "SectionName");
        }

        /// <summary>Writes an empty paragraph in Word.</summary>
        protected void WriteEmptyParagraph()
        {
            _wordWriter.WriteStartElement("w:p");
            _wordWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes a paragraph of text to the Word document.
        /// </summary>
        /// <param name="paragraphContent">Paragraph content.</param>
        protected void WriteParagraph(string paragraphContent)
        {
            WriteStyledParagraph(paragraphContent, false, false, null);
        }

        /// <summary>
        /// Writes a paragraph of text to the Word document.
        /// </summary>
        /// <param name="paragraphContent">Paragraph content.</param>
        /// <param name="bold">Whether paragraph should be bold.</param>
        /// <param name="italic">Whether paragraph should be italic.</param>
        /// <param name="styleName">Name of paragraph style; null for normal.</param>
        protected void WriteStyledParagraph(string paragraphContent,
            bool bold, bool italic, string styleName)
        {
            if (styleName == null)
            {
                styleName = NormalStyleId;
            }

            _wordWriter.WriteStartElement("w:p");
            _wordWriter.WriteStartElement("w:pPr");
            _wordWriter.WriteStartElement("w:pStyle");
            _wordWriter.WriteAttributeString("w:val", styleName);
            _wordWriter.WriteEndElement();
            _wordWriter.WriteEndElement();
            _wordWriter.WriteStartElement("w:r");
            _wordWriter.WriteStartElement("w:rPr");
            if (bold)
            {
                _wordWriter.WriteStartElement("w:b");
                _wordWriter.WriteAttributeString("w:val", "on");
                _wordWriter.WriteEndElement();
            }
            if (italic)
            {
                _wordWriter.WriteStartElement("w:i");
                _wordWriter.WriteAttributeString("w:val", "on");
                _wordWriter.WriteEndElement();
            }
            _wordWriter.WriteEndElement();
            _wordWriter.WriteStartElement("w:t");
            _wordWriter.WriteString(paragraphContent);
            _wordWriter.WriteEndElement();
            _wordWriter.WriteEndElement();
            _wordWriter.WriteEndElement();
        }

        /// <summary>Ends the current section in Word.</summary>
        protected void WriteEndSection()
        {
        }

        /// <summary>Ends the current row in Excel.</summary>
        protected void WriteEndRow()
        {
            _excelWriter.WriteEndElement();
        }

        /// <summary>Ends the current worksheet in Excel.</summary>
        protected void WriteEndWorksheet()
        {
            _excelWriter.WriteEndElement();
            _excelWriter.WriteEndElement();
        }

        /// <summary>
        /// Validates that all properties used by ReportBase are valid.
        /// </summary>
        protected void ValidateReportProperties()
        {
            if (_excelWriter == null)
            {
                throw new InvalidOperationException("excelWriter should not be null");
            }
            if (_wordWriter == null)
            {
                throw new InvalidOperationException("wordWriter should not be null");
            }
            if (_options == null)
            {
                throw new InvalidOperationException("options should not be null");
            }
        }

        #endregion Protected methods.

        #region Protected properties.

        /// <summary>XmlWriter for SpreadsheetML document.</summary>
        protected XmlWriter ExcelWriter
        {
            get { return this._excelWriter; }
        }

        /// <summary>Report options.</summary>
        protected FundamentalReportOptions Options
        {
            get { return this._options; }
        }

        /// <summary>XmlWriter for WordML document.</summary>
        protected XmlWriter WordWriter
        {
            get { return this._wordWriter; }
        }

        protected const string TasksProductName = "Windows Client Task List";
        protected const string BugsProductName = "Windows OS Bugs";

        #endregion Protected properties.

        #region Private fields.

        /// <summary>XmlWriter for SpreadsheetML document.</summary>
        private XmlWriter _excelWriter;

        /// <summary>Report options.</summary>
        private FundamentalReportOptions _options;

        /// <summary>XmlWriter for WordML document.</summary>
        private XmlWriter _wordWriter;

        #endregion Private fields.
    }
}