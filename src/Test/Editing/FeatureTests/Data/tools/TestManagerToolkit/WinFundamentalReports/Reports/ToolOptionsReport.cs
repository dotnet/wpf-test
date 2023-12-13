// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinFundamentalReports.Reports
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Xml;

    using PS = ProductStudio;

    #endregion Namespaces.

    /// <summary>Provides a report with report generation options.</summary>
    public class ToolOptionsReport: ReportBase
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new ToolOptionsReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public ToolOptionsReport(XmlWriter excelWriter, XmlWriter wordWriter,
            FundamentalReportOptions options): base(excelWriter, wordWriter, options)
        {
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Writes the report to the configured outputs.</summary>
        public override void WriteReport()
        {
            base.WriteReport();

            WriteWordReport();
        }

        #endregion Public methods.

        #region Private methods.

        private void WriteWordReport()
        {
            WriteStartSection("Tool Options");
            WriteParagraph("AvalonCoverageBuildId: " + Options.AvalonCoverageBuildId.ToString());
            WriteParagraph("AvalonCoverageConnection: " + Options.AvalonCoverageConnection);
            WriteParagraph("BinaryDropPath: " + Options.BinaryDropPath);
            WriteParagraph("CoverageDatabase: " + Options.CoverageDatabase);
            WriteParagraph("CodeCleanupPaths: " + String.Join(", ", Options.CodeCleanupPaths));
            WriteParagraph("CoverageUserName: " + Options.CoverageUserName);
            WriteParagraph("CoverageServerName: " + Options.CoverageServerName);
            WriteParagraph("MembersVisibilityPath: " + Options.MembersVisibilityPath);
            WriteParagraph("ReportKinds: " + Options.ReportKinds.ToString());
            WriteParagraph("TacticsNode: " + Options.TacticsNode);
            WriteParagraph("TacticsConnectionString: " + Options.TacticsConnectionString);
            WriteParagraph("TypeNameFiltersPath: " + Options.TypeNameFiltersPath);
        }

        #endregion Private methods.
    }
}
