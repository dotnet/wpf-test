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
    using System.Text;
    using System.Xml;

    using WinFundamentalReports;

    #endregion Namespaces.

    /// <summary>Provides information about sustainability goals.</summary>
    public class SustainabilityReport: ReportBase
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new SustainabilityReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public SustainabilityReport(XmlWriter excelWriter, XmlWriter wordWriter,
            FundamentalReportOptions options): base(excelWriter, wordWriter, options)
        {

        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Writes the report to the configured outputs.</summary>
        public override void WriteReport()
        {
            DataSet testCaseSet;

            base.WriteReport();

            testCaseSet = CreateTestCaseDataSet();
            
            // Write to Excel.
            WriteStartWorksheet("Sustainability");
            WriteHeaderRow(new string[] {
                "Tester", "Priority", "TestCase", "Title" },
                new int[] { 39, 37, 45, 423 });
            foreach(DataRow row in testCaseSet.Tables[NotRunningTableName].Rows)
            {
                WriteStartRow();
                WriteCellAsString(row["Tester"].ToString());
                WriteCellAsNumber(Convert.ToInt64(row["Priority"]));
                WriteCellAsNumber(Convert.ToInt64(row["pkTestCase"]));
                WriteCellAsString(row["Title"].ToString());
                WriteEndRow();
            }
            WriteEndWorksheet();

            // Write to Word.
            WriteToWord(testCaseSet);
        }

        #endregion Public methods.

        #region Private methods.

        private DataSet CreateTestCaseDataSet()
        {
            using (SqlConnection connection = new SqlConnection(Options.TacticsConnectionString))
            {
                SqlDataAdapter adapter;
                SqlCommand command;
                DataSet testCaseSet;

                command = connection.CreateCommand();
                command.CommandText = String.Format(NotRunningCasesQueryFmt, Options.TacticsNode);
                command.CommandType = CommandType.Text;

                adapter = new SqlDataAdapter(command);
                adapter.TableMappings.Add(NotRunningTableName, NotRunningTableName);
                
                testCaseSet = new DataSet();
                adapter.Fill(testCaseSet, NotRunningTableName);

                return testCaseSet;
            }
        }

        private static double CountDataRows(DataRowView[] rows)
        {
            return rows.Length;
        }

        private static object[] DistinctSortedValues(DataRowCollection rows, string columnName)
        {
            List<object> values;

            values = new List<object>();
            foreach(DataRow row in rows)
            {
                if (values.IndexOf(row[columnName]) == -1)
                {
                    values.Add(row[columnName]);
                }
            }
            return values.ToArray();
        }

        private static double SumDataRows(DataRowCollection rows, string columnName)
        {
            double result;

            result = 0;
            foreach(DataRow row in rows)
            {
                result += Convert.ToDouble(row[columnName]);
            }
            return result;
        }

        private void WriteToWord(DataSet testCaseSet)
        {
            object[] testers;
            DataTable table;

            table = testCaseSet.Tables[NotRunningTableName];
            table.DefaultView.Sort = "Tester";
            testers = DistinctSortedValues(table.Rows, "Tester");

            WriteStartSection("Sustainability");
            foreach(object tester in testers)
            {
                double testCaseCount;

                testCaseCount = CountDataRows(table.DefaultView.FindRows(tester));
                if (testCaseCount > 10)
                {
                    WriteStyledParagraph(tester.ToString() + " has more than 10 not-running " +
                        "test cases: " + testCaseCount + ".", true, false, null);
                }
                else
                {
                    WriteParagraph(tester.ToString() + " has less than 10 not-running " +
                        "test cases: " + testCaseCount + ".");
                }
            }
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>
        /// Query to be formatted with path, to list cases that are not 
        /// currently running.
        /// </summary>
        private const string NotRunningCasesQueryFmt =
@"SELECT U.Tester, TC.Priority, TC.pkTestCase, TC.Title
FROM SysTbl_TestCase TC, SysTbl_NodePath NP, ListTbl_MethodList ML, SysTbl_User U
WHERE TC.idNode = NP.idNode
AND NP.Path LIKE '{0}'
AND ML.idTestCase = TC.pkTestCase
AND (ML.idMethod = 11 OR ML.idMethod = 8 OR ML.idMethod = 7)
AND TC.idAssigned = U.pkUser
AND TC.IsDelete = 0
ORDER BY U.Tester, TC.Priority, TC.Title
";

        private const string NotRunningTableName = "NotRunningTable";

        #endregion Private fields.
    }
}
