// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinFundamentalReports.Reports
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    using WinFundamentalReports;

    #endregion Namespaces.

    public class PlatformReport: ReportBase
    {
        /// <summary>
        /// Initializes a new PlatformReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public PlatformReport(XmlWriter excelWriter, XmlWriter wordWriter, 
            FundamentalReportOptions options): base(excelWriter, wordWriter, options)
        {
        }

        /// <summary>Writes the report to the configured outputs.</summary>
        public override void WriteReport()
        {
            string queryText;       // Text for querying.
            long activeTestCount;   // Number of active test cases.
            long activeTestCount64; // Number of active test cases enabled for 64-bit.

            base.WriteReport();

            // Nothing interesting to write to Excel.

            // Write status report to Word.
            WriteStartSection("Platforms");

            WriteStyledParagraph("64-bit Platform Testing", true, false, null);

            queryText = String.Format(ActiveEditingTestCountQueryTextFmt, Options.TacticsNode);
            activeTestCount = RunReportAsLong(Options.TacticsConnectionString, queryText);

            queryText = String.Format(ActiveEditingTestCount64QueryTextFmt, Options.TacticsNode);
            activeTestCount64 = RunReportAsLong(Options.TacticsConnectionString, queryText);

            WriteParagraph("There are " + activeTestCount64 +
                " test cases, of which " + activeTestCount +
                " are 64-bit enabled.");
            if (activeTestCount64 < activeTestCount)
            {
                WriteStyledParagraph("The fundamental goal is not being met.", true, false, null);
            }
            else
            {
                WriteStyledParagraph("The fundamental goal is being met.", true, false, null);
            }

            WriteEndSection();
        }

        private const string ActiveEditingTestCountQueryTextFmt = @"
            SELECT COUNT(*) AS ActiveEditingTestCount
            FROM SysTbl_TestCase TC
            WHERE TC.idNode IN (
	            SELECT NP.idNode
	            FROM SysTbl_NodePath NP
	            WHERE NP.Path like '{0}'
            )
            AND TC.IsDelete = 0
            AND TC.IsInactive = 0";

        private const string ActiveEditingTestCount64QueryTextFmt = @"
            SELECT COUNT(*) AS ActiveEditingTestCount64
            FROM SysTbl_TestCase TC
            WHERE TC.idNode IN (
	            SELECT NP.idNode
	            FROM SysTbl_NodePath NP
	            WHERE NP.Path like '{0}'
            )
            AND TC.IsDelete = 0
            AND TC.IsInactive = 0";
    }
}
