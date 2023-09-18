// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinFundamentalReports.Reports
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;
    using System.Xml;

    using MAGELLANLib;
    using MAGREPORTSLib;
    using WinFundamentalReports;

    #endregion Namespaces.

    public class CoverageReportRecord
    {
        public string ComponentName;
        public string Namespace;
        public string ClassName;
        public string FunctionName;
        public bool ClassIsPublic;
        public bool FunctionIsPublic;
        public int CoveredArcCount;
        public int CoveredBlockCount;
        public int TotalArcCount;
        public int TotalBlockCount;

        public double CoveredArcRatio
        {
            get
            {
                return (TotalArcCount == 0)? 1 : (double)CoveredArcCount / (double)TotalArcCount;
            }
        }

        public double CoveredBlockRatio
        {
            get
            {
                return (TotalBlockCount == 0)? 1 : (double)CoveredBlockCount / (double)TotalBlockCount;
            }
        }
    }

    internal class MemberVisibility
    {
        public string TypeFullName;
        public bool TypeIsPublic;
        public string MemberName;
        public bool MemberIsPublic;

        public MemberVisibility(string line)
        {
            int start;
            int end;

            if (line == null)
            {
                throw new ArgumentNullException(line);
            }
            start = 0;
            end = line.IndexOf(',', start);
            TypeFullName = line.Substring(start, end - start);
            start = end + 1;
            end = line.IndexOf(',', start);
            TypeIsPublic = line.Substring(start, end - start) == "True";
            start = end + 1;
            end = line.IndexOf(',', start);
            MemberName = line.Substring(start, end - start);
            start = end + 1;
            end = line.IndexOf(',', start);
            MemberIsPublic = line.Substring(start) == "True";
        }
    }

    /// <summary>
    /// Provides information about code coverage.
    /// </summary>
    public class CoverageReport: ReportBase
    {
        /// <summary>
        /// Initializes a new CoverageReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public CoverageReport(XmlWriter excelWriter, XmlWriter wordWriter,
            FundamentalReportOptions options): base(excelWriter, wordWriter, options)
        {

        }

        private CSDocFormatter.TypeNameFilters _filters;
        private List<MemberVisibility> _memberVisibilityList;

        #region Public properties.

        public CSDocFormatter.TypeNameFilters Filters
        {
            get { return this._filters; }
            set { this._filters = value; }
        }

        public string MembersVisibilityPath
        {
            get { return Options.MembersVisibilityPath; }
        }

        #endregion Public properties.

        /// <summary>
        /// Provides a list of build names in the specified server,
        /// connecting with the given username/password.
        /// </summary>
        public static List<string> ListBuilds(string serverName,
            string userName, string password)
        {
            List<string> result;
            SqlConnection connection;
            string connectionString;

            connectionString = @"Data Source=" + serverName +
                ";User ID=" + userName + ";Password=" + password;

            result = new List<string>();
            using (connection = new SqlConnection(connectionString))
            {
                SqlDataReader buildReader;
                SqlCommand command;
                string commandText;

                commandText = "SELECT CATALOG_NAME FROM INFORMATION_SCHEMA.SCHEMATA ORDER BY CATALOG_NAME";

                connection.Open();
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = commandText;
                using (buildReader = command.ExecuteReader())
                {
                    while (buildReader.Read())
                    {
                        result.Add(buildReader.GetString(0));
                    }
                }
            }
            return result;
        }

        /// <summary>Writes the report to the configured outputs.</summary>
        public override void WriteReport()
        {
            List<CoverageReportRecord> records;
            List<CoverageReportRecord> missedPublicRecords;
            StringBuilder builder;
            string lastClassName;
            double currentCoverage;
            const double targetCoverage = 0.75;

            base.WriteReport();

            if (!String.IsNullOrEmpty(Options.TypeNameFiltersPath))
            {
                Filters = new CSDocFormatter.TypeNameFilters(Options.TypeNameFiltersPath);
            }
            records = CreateReport();

            // Write to Excel.
            WriteStartWorksheet("Coverage");
            WriteHeaderRow(new string[] {
                "Namespace", "Class Name", "Function Name",
                "Class Public", "Function Public", "Total Arcs",
                "Covered Arcs", "Arc Ratio", "Total Blocks",
                "Covered Blocks", "Block Ratio", "Component Name" },
                new int[] { 112, 126, 135, 54, 72, 48, 63, 47, 57, 71, 54, 102 });
            foreach(CoverageReportRecord record in records)
            {
                WriteStartRow();
                WriteCellAsString(record.Namespace);
                WriteCellAsString(record.ClassName);
                WriteCellAsString(record.FunctionName);
                WriteCellAsString(record.ClassIsPublic.ToString());
                WriteCellAsString(record.FunctionIsPublic.ToString());
                WriteCellAsNumber(record.TotalArcCount);
                WriteCellAsNumber(record.CoveredArcCount);
                WriteCellAsPercent(record.CoveredArcRatio);
                WriteCellAsNumber(record.TotalBlockCount);
                WriteCellAsNumber(record.CoveredBlockCount);
                WriteCellAsPercent(record.CoveredBlockRatio);
                WriteCellAsString(record.ComponentName);
                WriteEndRow();
            }
            WriteEndWorksheet();

            // For Word, write only the high-priority (untouched) APIs.
            missedPublicRecords = CoverageReport.ListNonHitMembers(records);

            // Write to Word.
            WriteStartSection("Coverage");
            WriteParagraph("Information retrieved from database " +
                Options.CoverageDatabase + " in server " +
                Options.CoverageServerName + ".");
            WriteEmptyParagraph();

            WriteStyledParagraph("Public API Coverage", true, false, null);
            if (missedPublicRecords.Count == 0)
            {
                WriteParagraph("All APIs have been invoked.");
                WriteStyledParagraph("The public coverage fundamental goal is being met.", true, false, null);
            }
            else
            {
                WriteStyledParagraph("The public coverage fundamental goal is not being met.", true, false, null);
                WriteEmptyParagraph();
                WriteParagraph("The following APIs were never invoked at all.");
                WriteEmptyParagraph();

                builder = null;
                lastClassName = null;
                foreach(CoverageReportRecord record in missedPublicRecords)
                {
                    if (record.ClassName != lastClassName)
                    {
                        if (builder != null)
                        {
                            WriteParagraph(builder.ToString());
                            WriteEmptyParagraph();
                        }
                        builder = new StringBuilder();
                        builder.Append(record.Namespace);
                        builder.Append(".");
                        builder.Append(record.ClassName);
                        builder.Append(": ");
                        builder.Append(record.FunctionName);

                        lastClassName = record.ClassName;
                    }
                    else
                    {
                        builder.Append(", ");
                        builder.Append(record.FunctionName);
                    }
                }
                WriteParagraph(builder.ToString());
            }

            WriteEmptyParagraph();
            WriteEmptyParagraph();
            WriteStyledParagraph("Total Code Coverage", true, false, null);
            currentCoverage = CalculateTotalCoverage(records);
            if (currentCoverage < targetCoverage)
            {
                WriteParagraph("The current coverage of " +
                    currentCoverage.ToString("p") +
                    " falls below the target coverage of " +
                    targetCoverage.ToString("p") + ".");
                WriteStyledParagraph("The total code coverage fundamental goal is not being met.",
                    true, false, null);
            }
            else
            {
                WriteParagraph("The current coverage of " +
                    currentCoverage.ToString("p") +
                    " meets the target coverage of " +
                    targetCoverage.ToString("p") + ".");
                WriteStyledParagraph("The total code coverage fundamental goal is not being met.",
                    true, false, null);
            }

            WriteEndSection();
        }

        private double CalculateTotalCoverage(List<CoverageReportRecord> records)
        {
            double totalBlockCount;
            double coveredBlockCount;

            if (records == null)
            {
                throw new ArgumentNullException("records");
            }

            totalBlockCount = 0;
            coveredBlockCount = 0;
            foreach(CoverageReportRecord record in records)
            {
                totalBlockCount += record.TotalBlockCount;
                coveredBlockCount += record.CoveredBlockCount;
            }

            return coveredBlockCount / totalBlockCount;
        }

        private List<CoverageReportRecord> CreateReport()
        {
            string componentName;
            string namespaceName;
            string className;
            List<CoverageReportRecord> result;

            MagApplicationClass magellanApplication;
            IMagCoverageDatabase coverageDatabase;

            LoadMemberVisibilityList();

            result = new List<CoverageReportRecord>();
            magellanApplication = new MagApplicationClass();
            coverageDatabase = magellanApplication.OpenCoverageDatabase(
                MAGELLANLib.MagDatabaseOpenType.magDatabaseOpen,
                Options.CoverageServerName, Options.CoverageDatabase, false,
                Options.CoverageUserName, Options.CoveragePassword);

            foreach(IMagCovLogicalBuild build in coverageDatabase.LogicalBuilds)
            {
                foreach(MAGELLANLib.IMagCovClass coverageClass in build.Classes)
                {
                    namespaceName = coverageClass.NameSpace;
                    className = coverageClass.Name;
                    if (!ShouldProcessClass(namespaceName + "." + className))
                    {
                        continue;
                    }
                    componentName = coverageClass.Component.Name;
                    foreach(MAGELLANLib.IMagCovFunction function in coverageClass.Functions)
                    {
                        MemberVisibility visibility;
                        CoverageReportRecord record;

                        visibility = FindMemberVisibility(namespaceName + "." + className, function.Name);
                        record = new CoverageReportRecord();
                        record.ComponentName = componentName;
                        record.Namespace = namespaceName;
                        record.ClassName = className;
                        record.FunctionName = function.Name;
                        record.ClassIsPublic = (visibility == null) || visibility.TypeIsPublic;
                        record.FunctionIsPublic = (visibility == null) || visibility.MemberIsPublic;
                        record.CoveredArcCount = function.CoveredArcsTotal;
                        record.CoveredBlockCount = function.CoveredBlocksTotal;
                        record.TotalArcCount = function.CountArcs;
                        record.TotalBlockCount = function.CountBlocks;
                        result.Add(record);
                    }
                }
            }
            return result;
        }

        public static List<CoverageReportRecord> ListNonHitMembers(List<CoverageReportRecord> coverageRecords)
        {
            List<CoverageReportRecord> result;

            result = new List<CoverageReportRecord>(coverageRecords.Count / 2);
            foreach(CoverageReportRecord record in coverageRecords)
            {
                if (record.ClassIsPublic && record.FunctionIsPublic &&
                    record.CoveredBlockCount == 0)
                {
                    result.Add(record);
                }
            }

            return result;
        }

        private bool ShouldProcessClass(string className)
        {
            if (Filters == null || !Filters.HasFilters)
            {
                return true;
            }
            else
            {
                return Filters.MatchesFilters(className);
            }
        }

        private MemberVisibility FindMemberVisibility(string className, string memberName)
        {
            foreach(MemberVisibility v in this._memberVisibilityList)
            {
                if (v.TypeFullName == className && v.MemberName == memberName)
                {
                    return v;
                }
            }
            return null;
        }

        private void LoadMemberVisibilityList()
        {
            this._memberVisibilityList = new List<MemberVisibility>();

            if (MembersVisibilityPath != null)
            {
                using (StreamReader reader = new StreamReader(MembersVisibilityPath))
                {
                    string line;
                    reader.ReadLine();
                    while ((line = reader.ReadLine()) != null )
                    {
                        if (line.Length > 1)
                        {
                            _memberVisibilityList.Add(new MemberVisibility(line));
                        }
                    }
                }
            }
        }
    }
}