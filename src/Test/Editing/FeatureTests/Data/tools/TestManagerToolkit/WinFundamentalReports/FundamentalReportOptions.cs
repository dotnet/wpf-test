// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace WinFundamentalReports
{
    /// <summary>Options used by fundamental reports.</summary>
    public class FundamentalReportOptions
    {
        #region Constructors.

        /// <summaryHidden constructor.</summary>
        private FundamentalReportOptions()
        {
            tacticsConnectionString = @"Data Source=AVALONTACTICS;Initial Catalog=tactics2;User Id=Tactics2_Log;Password=Tactics2_Log";
            tacticsNode = @"$\\WCP\Editing%";
            codeCleanupPaths = new string[] {
                @"C:\dev\nt\testsrc\windowstest\client\wcptests\uis\Common",
                @"C:\dev\nt\testsrc\windowstest\client\wcptests\uis\Forms",
                @"C:\dev\nt\testsrc\windowstest\client\wcptests\uis\Text",
                };
            coverageDatabase = "Editing10062004";
            coverageServerName = @"tamales\scratch";
            coveragePassword = "user";
            coverageUserName = "user";
            membersVisibilityPath = @"E:\dev\nt.binaries.x86chk\NTTEST\WINDOWSTEST\Client\Tools\Utils\CSDocFormatter\members_visibility.csv";
            reportKinds = ReportKinds.BugJail | ReportKinds.Coverage |
                ReportKinds.Platform | ReportKinds.TaskList | ReportKinds.Sustainability |
                ReportKinds.CodeCleanup | ReportKinds.Metrics;
            typeNameFiltersPath = @"E:\dev\nt\testsrc\windowstest\client\wcptests\Common\Utilities\CSDocFormatter\res\uis_type_filters.txt";
            avalonCoverageBuildId = 16;
            avalonCoverageConnection = "Server=AvalonCC1;User ID=Coverage_Log;Password=Coverage_Log";
            binaryDropPath = @"C:\dev\nt\windows\DevTest\WCP\objchk\i386";
            taskListQuery = new string[] {
                "<Query>",
                "<Group GroupOperator='And'>",
                @"<Expression Column='TreeID' Operator='under'><Number>408</Number></Expression>",
                @"<Expression Column='Status' Operator='equals'><String>Active</String></Expression>",
                @"<Expression Column='Milestone' Operator='equals'><String>" + WinFundamentalReports.Reports.MilestoneSchedule.Current.Name + @"</String></Expression>",
                @"<Expression Column='Issue type' Operator='equals'><String>Test Work Item</String></Expression>",
                "</Group></Query>"
            };
        }

        /// <summary>
        /// Initializes a new FundamentalReportOptions instance.
        /// </summary>
        /// <returns>A new FundamentalReportOptions instance.</returns>
        public static FundamentalReportOptions CreateNewOptions()
        {
            return new FundamentalReportOptions();
        }

        /// <summary>
        /// Initializes a new FundamentalReportOptions instance from
        /// a saved file.
        /// </summary>
        /// <returns>A new FundamentalReportOptions instance.</returns>
        public static FundamentalReportOptions LoadFromFile(string path)
        {
            FundamentalReportOptions result;
            int versionNumber;

            result = new FundamentalReportOptions();
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                versionNumber = reader.ReadInt32();
                if (versionNumber > VersionNumber)
                {
                    throw new NotSupportedException("Version " + versionNumber + " is not supported.");
                }

                result.tacticsConnectionString = reader.ReadString();
                result.tacticsNode = reader.ReadString();
                result.coverageDatabase = reader.ReadString();
                result.coverageUserName = reader.ReadString();
                result.coverageServerName = reader.ReadString();
                result.coveragePassword = reader.ReadString();
                result.membersVisibilityPath = reader.ReadString();
                result.typeNameFiltersPath = reader.ReadString();

                if (versionNumber > VersionNumber1)
                {
                    result.reportKinds = (ReportKinds) reader.ReadInt32();
                }

                if (versionNumber > VersionNumber2)
                {
                    result.codeCleanupPaths = ReadStringArray(reader);
                }

                if (versionNumber > VersionNumber3)
                {
                    result.avalonCoverageBuildId = reader.ReadInt32();
                    result.avalonCoverageConnection = reader.ReadString();
                }

                if (versionNumber > VersionNumber4)
                {
                    result.binaryDropPath = reader.ReadString();
                }

                if (versionNumber > VersionNumber5)
                {
                    result.taskListQuery = ReadStringArray(reader);
                }
            }
            return result;
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Saves the current options to the given file path.
        /// </summary>
        /// <param name="path">Full file path to save to.</param>
        public void SaveToFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(path);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(VersionNumber);
                writer.Write(tacticsConnectionString);
                writer.Write(tacticsNode);
                writer.Write(coverageDatabase);
                writer.Write(coverageUserName);
                writer.Write(coverageServerName);
                writer.Write(coveragePassword);
                writer.Write(membersVisibilityPath);
                writer.Write(typeNameFiltersPath);
                writer.Write((Int32)reportKinds);
                WriteStringArray(writer, this.codeCleanupPaths);
                writer.Write(avalonCoverageBuildId);
                writer.Write(avalonCoverageConnection);
                writer.Write(binaryDropPath);
                WriteStringArray(writer, this.taskListQuery);
            }
            File.SetAttributes(path, FileAttributes.NotContentIndexed | FileAttributes.Hidden);
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Build ID to use in Avalon coverage reports.</summary>
        public int AvalonCoverageBuildId
        {
            get { return avalonCoverageBuildId; }
            set { avalonCoverageBuildId = value; }
        }

        /// <summary>Connection string to coverage database for Avalon-wide reports.</summary>
        public string AvalonCoverageConnection
        {
            get { return avalonCoverageConnection; }
            set { avalonCoverageConnection = value; }
        }

        /// <summary>Path to directory with all Avalon binaries.</summary>
        public string BinaryDropPath
        {
            get { return binaryDropPath; }
            set { binaryDropPath = value; }
        }

        /// <summary>Database name for code coverage database.</summary>
        public string CoverageDatabase
        {
            get { return coverageDatabase; }
            set { coverageDatabase = value; }
        }

        /// <summary>Paths used when looking for code cleanup items.</summary>
        public string[] CodeCleanupPaths
        {
            get { return (string[]) this.codeCleanupPaths.Clone(); }
            set { this.codeCleanupPaths = (string[]) value.Clone(); }
        }

        /// <summary>User name for code coverage database.</summary>
        public string CoverageUserName
        {
            get { return coverageUserName; }
            set { coverageUserName = value; }
        }

        /// <summary>Server name for code coverage database.</summary>
        public string CoverageServerName
        {
            get { return coverageServerName; }
            set { coverageServerName = value; }
        }

        /// <summary>Password for code coverage database.</summary>
        public string CoveragePassword
        {
            get { return coveragePassword; }
            set { coveragePassword = value; }
        }

        /// <summary>File with visibility of members.</summary>
        public string MembersVisibilityPath
        {
            get { return this.membersVisibilityPath; }
            set { this.membersVisibilityPath = value; }
        }

        /// <summary>Report kinds to generate.</summary>
        public ReportKinds ReportKinds
        {
            get { return this.reportKinds; }
            set { this.reportKinds = value; }
        }

        /// <summary>
        /// Prefix node in the Tactics database for
        /// tests to be filtered.
        /// </summary>
        public string TacticsNode
        {
            get { return tacticsNode; }
            set { tacticsNode = value; }
        }

        /// <summary>
        /// String used to connect to the Tactics test
        /// case database.
        /// </summary>
        public string TacticsConnectionString
        {
            get { return tacticsConnectionString; }
            set { tacticsConnectionString = value; }
        }

        /// <summary>
        /// Product Studio query to generate task list information.
        /// </summary>
        public string[] TaskListQuery
        {
            get { return (string[])this.taskListQuery.Clone(); }
            set { this.taskListQuery = (string[])value.Clone(); }
        }

        /// <summary>
        /// Full path to file with type name filters.
        /// </summary>
        public string TypeNameFiltersPath
        {
            get { return this.typeNameFiltersPath; }
            set { this.typeNameFiltersPath = (value == null)? "" : value; }
        }

        #endregion Public properties.

        #region Private methods.

        private static string[] ReadStringArray(BinaryReader reader)
        {
            int itemCount;
            string[] result;

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            itemCount = reader.ReadInt32();
            result = new string[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                result[i] = reader.ReadString();
            }

            return result;
        }

        private static void WriteStringArray(BinaryWriter writer, string[] values)
        {
            int itemCount;

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            itemCount = values.Length;
            writer.Write(itemCount);
            foreach (string s in values)
            {
                writer.Write(s);
            }
        }

        #endregion Private methods.

        #region Private fields.

        private const int VersionNumber = 6;
        private const int VersionNumber5 = 5;
        private const int VersionNumber4 = 4;
        private const int VersionNumber3 = 3;
        private const int VersionNumber2 = 2;
        private const int VersionNumber1 = 1;

        private string binaryDropPath;
        private string tacticsConnectionString;
        private string tacticsNode;
        private string coverageDatabase;
        private string coverageUserName;
        private string coverageServerName;
        private string coveragePassword;
        private string membersVisibilityPath;
        private string typeNameFiltersPath;
        private string[] codeCleanupPaths;
        private int avalonCoverageBuildId;
        private string avalonCoverageConnection;
        private string[] taskListQuery;
        private ReportKinds reportKinds;

        #endregion Private fields.
    }

    /// <summary>Enumeration of available reports.</summary>
    [Flags]
    public enum ReportKinds
    {
        /// <summary>Reports bug jail status for testers.</summary>
        BugJail  = 0x01,
        /// <summary>Reports code coverage in tests.</summary>
        Coverage = 0x02,
        /// <summary>Reports platform support in tests.</summary>
        Platform = 0x04,
        /// <summary>Reports task list status testers.</summary>
        TaskList = 0x08,
        /// <summary>Reports sustainability goal status.</summary>
        Sustainability = 0x10,
        /// <summary>Reports pending code cleanup items.</summary>
        CodeCleanup = 0x20,
        /// <summary>Reports overall Avalon Code Coverage progress.</summary>
        AvalonCoverage = 0x40,
        /// <summary>Reports team-specific metrics.</summary>
        Metrics = 0x80,
        /// <summary>Reports options used in generation.</summary>
        ToolOptions = 0x100,
    }
}