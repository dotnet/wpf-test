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
    using System.Reflection;
    using System.Text;
    using System.Xml;

    using WinFundamentalReports;

    #endregion Namespaces.

    /// <summary>
    /// Provides information about Avalon code coverage.
    /// </summary>
    public class AvalonCoverageReport : ReportBase
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new AvalonCoverageReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public AvalonCoverageReport(XmlWriter excelWriter, XmlWriter wordWriter,
            FundamentalReportOptions options): base(excelWriter, wordWriter, options)
        {

        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Writes the report to the configured outputs.</summary>
        public override void WriteReport()
        {
            base.WriteReport();

            // WriteComponentCoverage();
            // WriteFileCoverage();
            WritePublicCoverage();
        }

        /// <summary>Lists available coverage builds.</summary>
        /// <returns>The list of available coverage builds.</returns>
        public static List<CoverageBuildRecord> ListBuilds(string coverageConnection)
        {
            List<CoverageBuildRecord> result;

            result = new List<CoverageBuildRecord>(10);
            using (SqlConnection connection = new SqlConnection(coverageConnection))
            using (SqlCommand command = new SqlCommand(ListBuildsSql, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new CoverageBuildRecord(
                            reader.GetInt32(0), reader.GetString(1), 
                            reader.GetDateTime(2)));
                    }
                }
            }
            return result;
        }

        #endregion Public methods.

        #region Private methods.

        /// <summary>
        /// Gets the known visibility to other assemblies of the specified member.
        /// </summary>
        /// <param name="classNamespace">Namespace for containing class.</param>
        /// <param name="className">Name for class.</param>
        /// <param name="memberName">Member name for which to get visibility.</param>
        /// <param name="isPublic">After invocation, whether the member is known to be visible.</param>
        /// <param name="isPrivate">After invocation, whether the member is known to be invisible.</param>
        /// <remarks>
        /// Note that after invocation, it may be the case that isPublic is true, 
        /// isPrivate is true, or both are false (but never both true). The last case
        /// occurs when the member cannot be found through reflection.
        /// </remarks>
        private void GetMemberVisibility(string classNamespace, string className, string memberName,
            out bool isPublic, out bool isPrivate)
        {
            string fullName;

            if (_privateMembers == null)
            {
                List<string> privateMemberList;
                List<string> publicMemberList;

                privateMemberList = new List<string>(1024 * 24);
                publicMemberList = new List<string>(1024 * 24);

                // Initialize all known private members to ignore.
                foreach (string assemblyName in s_assemblyNamesForPublicCoverage)
                {
                    Assembly assembly;
                    string assemblyFile;
                    Type[] types;

                    assemblyFile = Path.Combine(Options.BinaryDropPath, assemblyName + ".dll");
                    assembly = Assembly.LoadFrom(assemblyFile);
                    types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        MethodInfo[] methods;

                        methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.DeclaredOnly);
                        foreach (MethodInfo methodInfo in methods)
                        {
                            string fullMethodName;

                            fullMethodName = type.Namespace + '.' + type.Name + '.' + methodInfo.Name;
                            if (type.IsNotPublic || methodInfo.IsPrivate || methodInfo.IsAssembly)
                            {
                                privateMemberList.Add(fullMethodName);
                            }
                            else
                            {
                                publicMemberList.Add(fullMethodName);
                            }
                        }
                    }
                }
                _privateMembers = privateMemberList.ToArray();
                _publicMembers = publicMemberList.ToArray();
                Array.Sort(_privateMembers, StringComparer.Ordinal);
                Array.Sort(_publicMembers, StringComparer.Ordinal);
            }

            fullName = classNamespace + '.' + className + '.' + memberName;
            // Test for public first. If there are public/private overloads, this code cannot
            // disambiguate, and we want to err on the side of flagging it. Also, we cannot
            // have members that are known to be public and private at the same time, but we
            // may have members about which we don't know either.
            isPublic = Array.BinarySearch(_publicMembers, fullName, StringComparer.Ordinal) >= 0;
            if (isPublic)
            {
                isPrivate = false;
            }
            else
            {
                isPrivate = Array.BinarySearch(_privateMembers, fullName, StringComparer.Ordinal) >= 0;
            }
            System.Diagnostics.Debug.Assert(!(isPrivate && isPublic));
        }

        /// <summary>Trims the specified path to the windows base directory.</summary>
        private static string TrimToWindows(string path)
        {
            int index;

            if (path == null) return null;
            index = path.ToLowerInvariant().IndexOf("windows");
            if (index == -1) return path;
            return path.Substring(index);
        }

        /// <summary>Write report with per-team, per-component coverage data.</summary>
        private void WriteComponentCoverage()
        {
            string sql;

            WriteStartWorksheet("ComponentCoverage");
            WriteHeaderRow(new string[] {
                "Team", "Component", "Blocks %", "Arcs %", "CountBlocks", "CountArcs",
                "BlocksHit", "ArcsHit", "BlocksNotHit", "ArcsNotHit" },
                new int[] { 109, 146, 57, 49, 44, 36, 58, 50, 36, 58, 50 });

            sql = String.Format(ComponentCoverageSqlFmt, BuildId, traceType);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string team;
                        string component;
                        int blocksTotal;
                        int arcsTotal;
                        int blocksHit;
                        int arcsHit;
                        int blocksNotHit;
                        int arcsNotHit;
                        double blocksRatio;
                        double arcsRatio;
                        bool meetsGoals;
                        string textStyle;

                        WriteStartRow();

                        team = reader.GetString(0);
                        component = reader.GetString(1);
                        blocksTotal = reader.GetInt32(2);
                        arcsTotal = reader.GetInt32(3);
                        blocksHit = reader.GetInt32(4);
                        arcsHit = reader.GetInt32(5);
                        blocksNotHit = reader.GetInt32(6);
                        arcsNotHit = reader.GetInt32(7);

                        blocksRatio = (blocksTotal == 0) ? 1 : (double)blocksHit / blocksTotal;
                        arcsRatio = (arcsTotal == 0) ? 1 : (double)arcsHit / arcsTotal;

                        meetsGoals = blocksRatio >= 0.7 && arcsRatio >= 0.6;
                        textStyle = (meetsGoals) ? null : ProblemStyleId;

                        WriteCellAsData("String", team, textStyle);
                        WriteCellAsData("String", component, textStyle);
                        WriteCellAsDataWithComment(
                            "Number", blocksRatio.ToString(),
                            (blocksRatio >= 0.7) ? PercentCellStyleId : PercentProblemStyleId,
                            (blocksRatio >= 0.7)? null :  "Does not meet 70% goal.");
                        WriteCellAsDataWithComment(
                            "Number", arcsRatio.ToString(), 
                            (arcsRatio >= 0.6) ? PercentCellStyleId : PercentProblemStyleId,
                            (arcsRatio >= 0.6) ? null : "Does not meet 60% goal.");
                        WriteCellAsNumber(blocksTotal);
                        WriteCellAsNumber(arcsTotal);
                        WriteCellAsNumber(blocksHit);
                        WriteCellAsNumber(arcsHit);
                        WriteCellAsNumber(blocksNotHit);
                        WriteCellAsNumber(arcsNotHit);

                        WriteEndRow();
                    }
                }
            }

            WriteEndWorksheet();
        }

        /// <summary>Write report with per-file coverage data.</summary>
        private void WriteFileCoverage()
        {
            string sql;

            WriteStartWorksheet("FileCoverage");
            WriteHeaderRow(new string[] {
                "Team", "FileName", "Component", "CountBlocks", "CountArcs",
                "BlocksHit", "ArcsHit", "Blocks %", "Arcs %", "BlocksNotHit", "ArcsNotHit" },
                new int[] { 90, 200, 122, 57, 49, 44, 36, 44, 36, 58, 50 });

            sql = String.Format(FileCoverageSqlFmt, BuildId, traceType);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WriteStartRow();

                        WriteCellAsString(reader.GetString(0)); // Team
                        WriteCellAsDataWithComment("String",
                            reader.GetString(1), null,
                            "Found in " + TrimToWindows(reader.GetString(7))); // FileName
                        WriteCellAsString(reader.GetString(2)); // Component
                        WriteCellAsNumber(reader.GetInt32(3));  // CountBlocks
                        WriteCellAsNumber(reader.GetInt32(4));  // CountArcs
                        WriteCellAsNumber(reader.GetInt32(5));  // BlocksHit
                        WriteCellAsNumber(reader.GetInt32(6));  // ArcsHit

                        WriteRatio(reader.GetInt32(5), reader.GetInt32(3)); // Blocks %
                        WriteRatio(reader.GetInt32(6), reader.GetInt32(4)); // Arcs %

                        WriteDelta(reader.GetInt32(5), reader.GetInt32(3)); // BlocksNotHit
                        WriteDelta(reader.GetInt32(6), reader.GetInt32(4)); // ArcsNotHit

                        WriteEndRow();
                    }
                }
            }

            WriteEndWorksheet();
        }

        private void WriteDelta(double deltaFrom, double deltaTo)
        {
            WriteCellAsNumber(deltaTo - deltaFrom);
        }

        /// <summary>Write report with data about uncovered members.</summary>
        private void WritePublicCoverage()
        {
            string sql;

            WriteStartWorksheet("PublicCoverage");
            WriteHeaderRow(new string[] {
                "Team", "Namespace", "ClassName", "MemberName" },
                new int[] { 90, 190, 370, 570 });

            sql = String.Format(PublicCoverageSqlFmt, BuildId);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string classNamespace;
                        string className;
                        string memberName;
                        bool isPublic;
                        bool isPrivate;

                        classNamespace = reader.GetString(1);
                        className = reader.GetString(2);
                        memberName = reader.GetString(3);

                        GetMemberVisibility(classNamespace, className, memberName, 
                            out isPublic, out isPrivate);
                        if (!isPrivate)
                        {
                            WriteStartRow();

                            WriteCellAsString(reader.GetString(0)); // Team name.
                            WriteCellAsString(classNamespace);
                            WriteCellAsString(className);
                            if (isPublic)
                            {
                                WriteCellAsString(memberName);
                            }
                            else
                            {
                                WriteCellAsDataWithComment("String", memberName,
                                    OtherWorkStyleId, "Unknown visibility.");
                            }

                            WriteEndRow();
                        }
                    }
                }
            }

            WriteEndWorksheet();
        }

        private void WriteRatio(double ratioPart, double total)
        {
            if (total == 0)
            {
                WriteCellAsPercent(0);
            }
            else
            {
                WriteCellAsPercent(ratioPart / total);
            }
        }

        #endregion Private methods.

        #region Private properties.

        private int BuildId
        {
            get { return this.Options.AvalonCoverageBuildId; }
        }

        private string ConnectionString
        {
            get { return this.Options.AvalonCoverageConnection; }
        }

        #endregion Private properties.

        #region Private fields.

        private string[] _privateMembers;
        private string[] _publicMembers;

        #endregion Private fields.

        #region Private constants.

        /// <summary>Trace for BVT and non-BVT tests.</summary>
        private const int traceType = 1;

        /// <summary>List of files to get types and member information from.</summary>
        private static readonly string[] s_assemblyNamesForPublicCoverage = new string[] {
            "PresentationCore", "PresentationFramework", "PresentationBuildTasks", 
            "PresentationFramework.Classic", "PresentationFramework.Luna",
            "WindowsBase", "UIAutomationClient", "UIAutomationTypes", "UIAutomationProvider", 
            "UIAutomationClientsideProviders.dll", "WindowsFormsIntegration"
            // "dwmapi.dll"
        };
    
        #endregion Private constants.

        #region SQL Statements.

        private const string FileCoverageSqlFmt =
@"SELECT FT.Name, DF.NameFile, DF_Component.NameFile, 
  FD.CountBlocks, FD.CountArcs, FD.BlocksHit, FD.ArcsHit, DF.NameDirectory
FROM CCAnalysis.dbo.tblFileData FD,
  CodeCoverage.dbo.tblDirectoryFile DF,
  CCAnalysis.dbo.tblFeatureTeam FT,
  CCAnalysis.dbo.tblComponentData CD,
  CodeCoverage.dbo.tblDirectoryFile DF_Component
WHERE FD.IDDirectoryFile = DF.IDDirectoryFile
AND FD.IDFeatureTeam = FT.IDFeatureTeam
AND FD.IDBuild = {0}
AND CD.IDComponentData = FD.IDComponentData
AND CD.IDTraceType = FD.IDTraceType
AND FD.IDTraceType = {1}
AND FT.Name <> 'All'
AND DF_Component.IDDirectoryFile = CD.IDDirectoryFile
ORDER BY FT.Name, DF.NameFile";

        private const string ComponentCoverageSqlFmt =
@"SELECT FT.Name, DF_Component.NameFile, 
  SUM(FD.CountBlocks) AS ComponentCountBlocks, 
  SUM(FD.CountArcs) AS ComponentCountArcs, 
  SUM(FD.BlocksHit) AS ComponentBlocksHit,
  SUM(FD.ArcsHit) AS ComponentArcsHit,
  SUM(FD.CountBlocks) - SUM(FD.BlocksHit) AS BlocksNotHit,
  SUM(FD.CountArcs) - SUM(FD.ArcsHit) AS ArcsNotHit
FROM CCAnalysis.dbo.tblFileData FD,
  CCAnalysis.dbo.tblFeatureTeam FT,
  CCAnalysis.dbo.tblComponentData CD,
  CodeCoverage.dbo.tblDirectoryFile DF_Component
WHERE FD.IDFeatureTeam = FT.IDFeatureTeam
AND FD.IDBuild = {0}
AND CD.IDComponentData = FD.IDComponentData
AND CD.IDTraceType = FD.IDTraceType
AND FD.IDTraceType = {1}
AND FT.Name <> 'All'
AND DF_Component.IDDirectoryFile = CD.IDDirectoryFile
GROUP BY FT.Name, DF_Component.NameFile
ORDER BY FT.Name, DF_Component.NameFile";

        private const string ListBuildsSql =
@"SELECT TOP 10 B.IDBuild, B.CustomVersion, B.Created
FROM CCAnalysis.dbo.tblBuild B
ORDER BY B.Created DESC";

        /// <summary>19 and 35 are the feature team codes for All and Libraries; 46-48 are externals.</summary>
        private const string PublicCoverageSqlFmt =
@"SELECT FT.Name, C.Namespace, C.Name, F.Name
FROM CCAnalysis.dbo.tblClass C,
  CCAnalysis.dbo.tblFunction F,
  CCAnalysis.dbo.tblClassData CD, 
  CCAnalysis.dbo.tblFeatureTeam FT,
  CCAnalysis.dbo.tblFunctionData FD
WHERE FD.CountBlocks > 0 AND FD.BlocksHit = 0
AND C.IDClass = CD.IDClass
AND F.IDFunction = FD.IDFunction
AND CD.IDClassData = FD.IDClassData
AND CD.IDTraceType = FD.IDTraceType
AND FT.IDFeatureTeam = FD.IDFeatureTeam
AND FD.IDBuild = {0}
AND FD.IDTraceType = 1
AND FT.IDFeatureTeam <> 19
AND FT.IDFeatureTeam <> 35
AND FT.IDFeatureTeam <> 46
AND FT.IDFeatureTeam <> 47
AND FT.IDFeatureTeam <> 48
ORDER BY FT.Name, C.Namespace, C.Name, F.Name";

        #endregion SQL Statements.
    }

    /// <summary>Represents a build record from the coverage database.</summary>
    public class CoverageBuildRecord
    {
        #region Constructors.

        public CoverageBuildRecord(int idBuild, string customVersion,
            DateTime created)
        {
            this._idBuild = idBuild;
            this._customVersion = (customVersion == null) ? "" : customVersion;
            this._created = created;
        }

        #endregion Constructors.

        #region Public methods.

        public override string ToString()
        {
            return "Build " + IdBuild + ": " + CustomVersion;
        }

        #endregion Public methods.

        #region Public properties.

        public int IdBuild { get { return this._idBuild; } }
        public string CustomVersion { get { return this._customVersion; } }
        public DateTime Created { get { return this._created; } }

        #endregion Public properties.

        #region Private fields.

        private int _idBuild;
        private string _customVersion;
        private DateTime _created;

        #endregion Private fields.
    }
}