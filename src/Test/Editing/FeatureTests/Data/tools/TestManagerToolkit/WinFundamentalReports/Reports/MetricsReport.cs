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

    /// <summary>Provides a report with specific performance metrics.</summary>
    public class MetricsReport: ReportBase
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new MetricsReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public MetricsReport(XmlWriter excelWriter, XmlWriter wordWriter,
            FundamentalReportOptions options): base(excelWriter, wordWriter, options)
        {
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Writes the report to the configured outputs.</summary>
        public override void WriteReport()
        {
            base.WriteReport();

            RetrieveData();
            _copiedReportPath = CopyExistingReport();

            WriteWordReport();
        }

        #endregion Public methods.

        #region Private methods.

        private string GetExpectedExistingReportPath()
        {
            string folder;
            
            folder = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return System.IO.Path.Combine(folder, "BugStatsv2.5.xls");
        }

        private string CopyExistingReport()
        {
            string copyPath;
            DateTime date;
            string existingReportPath;

            existingReportPath = GetExpectedExistingReportPath();

            // This report is copied just once a week, with Monday's date.
            if (!File.Exists(existingReportPath))
            {
                return null;
            }
            
            // "2005-05-03-trends.xls"
            date = DateTime.Today;
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);
            }
            copyPath = ComprehensiveReport.GetPathForReport("trends.xls", date);
            
            // Don't overwrite the file.
            if (!File.Exists(copyPath))
            {
                File.Copy(existingReportPath, copyPath, false);
            }
            return copyPath;
        }

        /// <summary>Gets the field value for the given item.</summary>
        /// <param name="item">PS item to query.</param>
        /// <param name="fieldName">Field to return value for.</param>
        /// <returns>The string value of the value, an empty string if it's null.</returns>
        private static string FieldValue(PS.DatastoreItem item, string fieldName)
        {
            return ProductStudioQuery.FieldValue(item, fieldName);
        }

        /// <summary>
        /// Retrieves all necessary data from other stores and assigns it
        /// to instance fields, with all necessary 'massaging' for processing,
        /// like sorting or pivoting.
        /// </summary>
        private void RetrieveData()
        {
            string queryXml;
            string[] fieldNames;
            PS.DatastoreItemList items;

            // Notes (press Ctrl while running query in PS to paste to clipboard)

            queryXml = "<Query>" +
                "<Group GroupOperator='And'>" +
                @" <Group GroupOperator='Or'>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" <Expression Column='Assigned To' FieldType='16' Operator='equals'><String>Microsoft</String></Expression>" +
                @" </Group>" +
                @" <Group GroupOperator='Or'>" +
                @"  <Expression Column='Status' Operator='equals'><String>Active</String></Expression>" +
                @"  <Expression Column='Status' Operator='equals'><String>Resolved</String></Expression>" +
                @" </Group>" +
                @"<Expression Column='Issue type' Operator='equals'><String>Code Bug</String></Expression>" +
                "</Group></Query>";
            _bugs = new List<BugRecord>();
            fieldNames = new string[] {
                "ID", "Assigned To", "Title", "Opened Date", "Resolved Date", "Status",
            };

            items = ProductStudioQuery.ExecuteQuery(BugsProductName, queryXml, fieldNames);

            for (int i = 0; i < items.DatastoreItems.Count; i++)
            {
                PS.DatastoreItem item;
                BugRecord bug;

                item = items.DatastoreItems[i];
                bug = new BugRecord();
                bug.BugId = long.Parse(FieldValue(item, "ID"));
                bug.AssignedTo = FieldValue(item, "Assigned To");
                bug.Title = FieldValue(item, "Title");
                bug.OpenedDate = DateTime.Parse(FieldValue(item, "Opened Date"));
                bug.ResolvedDate = FieldValue(item, "Resolved Date");
                bug.Status = FieldValue(item, "Status");
                _bugs.Add(bug);
            }
        }

        private void WriteWordReport()
        {
            BugRecord oldestBug;

            WriteStartSection("Metrics");

            oldestBug = null;
            foreach (BugRecord bug in _bugs)
            {
                if (bug.Status != "Active") continue;
                if (oldestBug == null)
                {
                    oldestBug = bug;
                }
                else
                {
                    if (bug.OpenedDate < oldestBug.OpenedDate)
                    {
                        oldestBug = bug;
                    }
                }
            }
            WriteStyledParagraph("Oldest active bug", true, false, null);
            if (oldestBug == null)
            {
                WriteParagraph("There are no active bugs.");
            }
            else
            {
                WriteParagraph("Bug " + oldestBug.BugId + ", assigned to " +
                    oldestBug.AssignedTo + ", opened on " + oldestBug.OpenedDate);
            }

            oldestBug = null;
            foreach (BugRecord bug in _bugs)
            {
                if (!bug.IsResolved) continue;
                if (oldestBug == null)
                {
                    oldestBug = bug;
                }
                else
                {
                    if (DateTime.Parse(bug.ResolvedDate) < DateTime.Parse(oldestBug.ResolvedDate))
                    {
                        oldestBug = bug;
                    }
                }
            }
            WriteStyledParagraph("Oldest resolved bug", true, false, null);
            if (oldestBug == null)
            {
                WriteParagraph("There are no resolved bugs.");
            }
            else
            {
                WriteParagraph("Bug " + oldestBug.BugId + ", assigned to " +
                    oldestBug.AssignedTo + ", resolved on " + oldestBug.ResolvedDate);
            }
            
            WriteEmptyParagraph();
            if (_copiedReportPath == null)
            {
                WriteParagraph("This report does not have an associated trend file.");
                WriteParagraph("This trend file was expected at this location:" + GetExpectedExistingReportPath());
                WriteParagraph("You can find more information about this report at http://toolbox/sites/BugStats/default.aspx.");
            }
            else
            {
                WriteParagraph("See the following report file for trend data:");
                WriteParagraph(System.IO.Path.GetFileName(_copiedReportPath));
            }
        }

        #endregion Private methods.

        #region Private fields.

        private List<BugRecord> _bugs;
        private string _copiedReportPath;

        #endregion Private fields.
    }
}