// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinFundamentalReports.Reports
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Sql;
    using System.Data.SqlClient;
    using System.Text;
    using System.Xml;

    using WinFundamentalReports;

    #endregion Namespaces.

    /// <summary>
    /// Provides an aggregate of all available reports.
    /// </summary>
    public class ComprehensiveReport
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new ComprehensiveReport instance.
        /// </summary>
        public ComprehensiveReport()
        {

        }

        #endregion Constructors.

        #region Public properties.

        public FundamentalReportOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        #endregion Public properties.

        #region Public methods.

        /// <summary>Gets the full path to a report file.</summary>
        /// <param name="reportSuffix">Report suffix for file name, including extension.</param>
        /// <returns>The full path to a report file, including directory and timestamp.</returns>
        public static string GetPathForReport(string reportSuffix)
        {
            return GetPathForReport(reportSuffix, DateTime.Today);
        }

        /// <summary>Gets the full path to a report file.</summary>
        /// <param name="reportSuffix">Report suffix for file name, including extension.</param>
        /// <param name="date">Date to use as timestamp.</param>
        /// <returns>The full path to a report file, including directory and timestamp.</returns>
        public static string GetPathForReport(string reportSuffix, DateTime date)
        {
            string basePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return System.IO.Path.Combine(basePath,
                date.ToString("yyyy-MM-dd") + "-" + reportSuffix);
        }

        /// <summary>Writes the report files to disk.</summary>
        public void WriteReport()
        {
            // Use the new infrastructure for this.
            XmlTextWriter excelWriter;
            XmlTextWriter wordWriter;
            string excelFileName;
            string wordFileName;

            excelFileName = GetPathForReport("fund-report-numbers.xml");
            wordFileName = GetPathForReport("fund-report-description.xml");

            using (excelWriter = new XmlTextWriter(excelFileName, null))
            using (wordWriter = new XmlTextWriter(wordFileName, null))
            {
                ReportKinds[] reportKinds;
                Type[] reportTypes;

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    // Set indented formatting to help debug the output.
                    excelWriter.Formatting = Formatting.Indented;
                    wordWriter.Formatting = Formatting.Indented;
                }

                // Write the start of the reports.
                WriteStartExcelReport(excelWriter);
                WriteStartWordReport(wordWriter);

                // Write each report. reportTypes is the list of reports in
                // generation order, and reportKinds has the flag for each
                // report.
                reportTypes = new Type[] {
                    typeof(MetricsReport),
                    typeof(BugJailReport), typeof(ScheduleReport),
                    typeof(CodeCleanupReport),
                    typeof(CoverageReport),
                    typeof(SustainabilityReport), typeof(PlatformReport),
                    typeof(AvalonCoverageReport),
                    typeof(ToolOptionsReport),
                };
                reportKinds = new ReportKinds[] {
                    ReportKinds.Metrics,
                    ReportKinds.BugJail, ReportKinds.TaskList,
                    ReportKinds.CodeCleanup,
                    ReportKinds.Coverage,
                    ReportKinds.Sustainability, ReportKinds.Platform,
                    ReportKinds.AvalonCoverage,
                    ReportKinds.ToolOptions,
                };
                for (int i = 0; i < reportTypes.Length; i++)
                {
                    if ((reportKinds[i] & this.Options.ReportKinds) != 0)
                    {
                        ReportBase report;

                        report = (ReportBase)Activator.CreateInstance(reportTypes[i],
                            excelWriter, wordWriter, this.Options);
                        report.WriteReport();
                    }
                }

                // Close the end of the Word and Excel reports.
                WriteEndExcelReport(excelWriter);
                WriteEndWordReport(wordWriter);
            }
        }

        #endregion Public methods.

        #region Private methods.

        /// <summary>Writes a Word element with the specified value in the w:val attribute.</summary>
        /// <param name="writer">XmlWriter for WordML document.</param>
        /// <param name="elementName">Name of element to write.</param>
        /// <param name="value">Value for the w:val attribute.</param>
        private static void WriteElementWithValue(XmlWriter writer,
            string elementName, string value)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("w:val", value);
            writer.WriteEndElement();
        }

        /// <summary>Writes the end of an Excel report.</summary>
        /// <param name="excelWriter">XML writer for SpreadsheetML document.</param>
        /// <remarks>This method balances a call to WriteStartExcelReport.</remarks>
        private static void WriteEndExcelReport(XmlTextWriter excelWriter)
        {
            excelWriter.WriteEndElement();
        }

        /// <summary>Writes the end of a Word report.</summary>
        /// <param name="excelWriter">XML writer for WordML document.</param>
        /// <remarks>This method balances a call to WriteStartWordReport.</remarks>
        private static void WriteEndWordReport(XmlTextWriter wordWriter)
        {
            wordWriter.WriteEndElement();   // Close the w:body element.
            wordWriter.WriteEndElement();   // Close the w:wordDocument elemetn.
        }

        private static void WriteExcelStyle(XmlTextWriter excelWriter,
            string styleId, string styleName, string fontFamily,
            string fontName, int fontSize, bool isBold,
            string fontColor, string interiorColor, string interiorPattern,
            string numberFormat)
        {
            excelWriter.WriteStartElement("ss:Style");
            excelWriter.WriteAttributeString("ss:ID", styleId);
            excelWriter.WriteAttributeString("ss:Name", styleName);

            if (fontFamily != null || fontName != null || fontColor != null)
            {
                excelWriter.WriteStartElement("Font");
                if (fontFamily != null)
                {
                    excelWriter.WriteAttributeString("x:Family", fontFamily);
                }
                if (fontName != null)
                {
                    excelWriter.WriteAttributeString("ss:FontName", fontName);
                }
                if (fontSize != 0)
                {
                    excelWriter.WriteAttributeString("ss:Size", fontSize.ToString());
                }
                if (isBold)
                {
                    excelWriter.WriteAttributeString("ss:Bold", "1");
                }
                if (fontColor != null)
                {
                    excelWriter.WriteAttributeString("ss:Color", fontColor);
                }
                excelWriter.WriteEndElement();  // Close the Font element.
            }

            if (interiorColor != null || interiorPattern != null)
            {
                excelWriter.WriteStartElement("Interior");
                excelWriter.WriteAttributeString("ss:Color", interiorColor);
                excelWriter.WriteAttributeString("ss:Pattern", interiorPattern);
                excelWriter.WriteEndElement();  // Close the Interior element.
            }

            if (numberFormat != null)
            {
                excelWriter.WriteStartElement("ss:NumberFormat");
                excelWriter.WriteAttributeString("ss:Format", numberFormat);
                excelWriter.WriteEndElement();  // Close the ss:NumberFormat element.
            }
            excelWriter.WriteEndElement();  // Close the ss:Style element.
        }

        /// <summary>
        /// Writes the starting elements of the report in SpreadsheetML.
        /// </summary>
        /// <param name="excelWriter">XmlWriter to Excel report.</param>
        private static void WriteStartExcelReport(XmlTextWriter excelWriter)
        {
            excelWriter.WriteProcessingInstruction("xml", "version='1.0'");
            excelWriter.WriteProcessingInstruction("mso-application", "progid='Excel.Sheet'");
            excelWriter.WriteStartElement("ss:Workbook");
            excelWriter.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:spreadsheet");
            excelWriter.WriteAttributeString("xmlns:ss", "urn:schemas-microsoft-com:office:spreadsheet");
            excelWriter.WriteAttributeString("xmlns:x", "urn:schemas-microsoft-com:office:excel");
            excelWriter.WriteAttributeString("xmlns:x2", "urn:schemas-microsoft-com:office:excel2");
            excelWriter.WriteAttributeString("xmlns:html", "http://www.w3.org/TR/REC-html40");

            excelWriter.WriteStartElement("ss:Styles");

            WriteExcelStyle(excelWriter, ReportBase.NormalCellStyleId, "Normal",
                "Swiss", "Tahoma", 8,
                false,  // isBold
                null,   // fontColor
                null,   // interiorColor
                null,   // interiorPattern
                null);  // numberFormat

            WriteExcelStyle(excelWriter, ReportBase.HeaderCellStyleId, "Header",
                "Swiss", "Tahoma", 8,
                true,       // isBold
                "#FFFFFF",  // fontColor
                "#000000",  // interiorColor
                "Solid",    // interiorPattern
                null);      // numberFormat

            WriteExcelStyle(excelWriter, ReportBase.ConfidentCellStyleId, "Confident",
                "Swiss", "Tahoma", 8, false, null, "#AAFFAA", "Solid", null);

            WriteExcelStyle(excelWriter, ReportBase.ProblemStyleId, "Problem",
                "Swiss", "Tahoma", 8, true, "#FF0000", "#333333", "Solid", null);

            WriteExcelStyle(excelWriter, ReportBase.PercentProblemStyleId, "PercentProblem",
                "Swiss", "Tahoma", 8, true, "#FF0000", "#333333", "Solid", "0%");

            WriteExcelStyle(excelWriter, ReportBase.PercentCellStyleId, "Percent",
                null, null, 0, false, null, null, null, "0%");

            WriteExcelStyle(excelWriter, ReportBase.WeekendStyleId, "Weekend",
                null, null, 0, false, null, "#333333", "Solid", null);
            WriteExcelStyle(excelWriter, ReportBase.WeekendDateStyleId, "WeekendDate",
                null, null, 0, false, null, "#333333", "Solid", @"d\-mmm");

            WriteExcelStyle(excelWriter, ReportBase.PastStyleId, "Past",
                null, null, 0, false, null, "#333333", "Solid", null);
            WriteExcelStyle(excelWriter, ReportBase.PastDateStyleId, "PastDate",
                null, null, 0, false, null, "#333333", "Solid", @"d\-mmm");

            WriteExcelStyle(excelWriter, ReportBase.FutureStyleId, "Future",
                null, null, 0, false, null, "#CC99FF", "Solid", null);
            WriteExcelStyle(excelWriter, ReportBase.FutureDateStyleId, "FutureDate",
                null, null, 0, false, null, "#CC99FF", "Solid", @"d\-mmm");

            WriteExcelStyle(excelWriter, ReportBase.OtherWorkStyleId, "OtherWork",
                null, null, 0, false, null, "#999999", "Solid", null);

            WriteExcelStyle(excelWriter, ReportBase.FixedWorkStyleId, "FixedWork",
                null, null, 0, false, null, "#99CCFF", "Solid", null);

            excelWriter.WriteEndElement();  // Close the ss:Styles element.
        }

        /// <summary>
        /// Writes the starting elements of the report in WordML.
        /// </summary>
        /// <param name="wordWriter">XmlWriter to Word report.</param>
        private static void WriteStartWordReport(XmlTextWriter wordWriter)
        {
            wordWriter.WriteProcessingInstruction("xml", "version='1.0'");
            wordWriter.WriteProcessingInstruction("mso-application", "progid='Word.Document'");
            wordWriter.WriteStartElement("w:wordDocument");
            wordWriter.WriteAttributeString("xmlns:w", "http://schemas.microsoft.com/office/word/2003/wordml");

            wordWriter.WriteStartElement("w:styles");
            WriteWordStyle(wordWriter, "SectionName", true, "Arial", 16, true);
            WriteWordStyle(wordWriter, ReportBase.NormalStyleId, false, "Verdana", 10, false);
            wordWriter.WriteEndElement();   // Close the w:styles element.

            wordWriter.WriteStartElement("w:docPr");
            WriteElementWithValue(wordWriter, "w:view", "print");
            wordWriter.WriteEndElement();   // Close the w:docPr element.

            wordWriter.WriteStartElement("w:body");
        }

        private static void WriteWordStyle(XmlTextWriter wordWriter,
            string styleId, bool pageBreak, string fontName, int fontSize,
            bool bold)
        {
            wordWriter.WriteStartElement("w:style");
            wordWriter.WriteAttributeString("w:type", "paragraph");
            wordWriter.WriteAttributeString("w:styleId", styleId);
            WriteElementWithValue(wordWriter, "w:name", styleId);
            wordWriter.WriteStartElement("w:pPr");
            if (pageBreak)
            {
                WriteElementWithValue(wordWriter, "w:pageBreakBefore", "on");
            }
            wordWriter.WriteEndElement();   // Close pPr
            wordWriter.WriteStartElement("w:rPr");
            wordWriter.WriteStartElement("w:rFonts");
            wordWriter.WriteAttributeString("w:ascii", fontName);
            wordWriter.WriteAttributeString("w:h-ascii", fontName);
            wordWriter.WriteAttributeString("w:cs", fontName);
            wordWriter.WriteEndElement();   // Close rFonts
            if (bold)
            {
                WriteElementWithValue(wordWriter, "w:b", "on");
            }
            WriteElementWithValue(wordWriter, "w:sz", (fontSize * 2).ToString());
            wordWriter.WriteEndElement();   // Close rPr
            wordWriter.WriteEndElement();   // Close style (SectionName)
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>Options for reports.</summary>
        private FundamentalReportOptions _options;

        #endregion Private fields.
    }
}