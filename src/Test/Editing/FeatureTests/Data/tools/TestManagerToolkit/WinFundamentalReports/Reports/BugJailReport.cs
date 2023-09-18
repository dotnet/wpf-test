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

    using PS = ProductStudio;
    using WinFundamentalReports;

    #endregion Namespaces.

    public class BugRecord
    {
        /// <summary>Bug number.</summary>
        public long BugId;
        /// <summary>Alias that bug is assigned to, or Closed.</summary>
        public string AssignedTo;
        /// <summary>Bug title.</summary>
        public string Title;
        /// <summary>Bug status description.</summary>
        public string Status;
        public int Priority;
        public string TestFollowupAssignedTo;

        /// <summary>Date the bug was initially opened.</summary>
        public DateTime OpenedDate;

        /// <summary>Date the bug was last resolved, null if never resolved.</summary>
        public string ResolvedDate;

        /// <summary>Whether the bug has any linked test work items.</summary>
        public bool HasLinkedTestWorkItem;

        /// <summary>Whether the bug has all linked test work items closed.</summary>
        public bool HasClosedLinkedTestWorkItem;

        public List<long> OpenTestWorkItems = new List<long>(0);

        public bool IsClosed { get { return Status == "Closed"; } }
        public bool IsResolved { get { return Status == "Resolved"; } }
    }

    public class BugJailReportRecord
    {
        /// <summary>Tester whose bugs are being reported.</summary>
        public string Tester;
        /// <summary>Count of pri-0 bugs.</summary>
        public long Pri0BugCount { get { return JailBugs[0].Count; } }
        /// <summary>Count of pri-1 bugs.</summary>
        public long Pri1BugCount { get { return JailBugs[1].Count; } }
        /// <summary>Count of pri-2 bugs.</summary>
        public long Pri2BugCount { get { return JailBugs[2].Count; } }
        /// <summary>Count of pri-3 bugs.</summary>
        public long Pri3BugCount { get { return JailBugs[3].Count; } }
        /// <summary>List of bugs with work items closed.</summary>
        public List<BugRecord> WorkItemFinishedBugs;
        /// <summary>List of bugs with work items pending.</summary>
        public List<BugRecord> WorkItemPendingBugs;

        /// <summary>
        /// Array of bug lists that count for bug jail, indexed
        /// by priority.
        /// </summary>
        public List<long>[] JailBugs;

        /// <summary>List of resolved bugs.</summary>
        public List<long> ResolvedBugs;

        public BugJailReportRecord()
        {
            this.WorkItemFinishedBugs = new List<BugRecord>();
            this.WorkItemPendingBugs = new List<BugRecord>();
            this.ResolvedBugs = new List<long>();
            this.JailBugs  = new List<long>[4];
            for (int i = 0; i < this.JailBugs.Length; i++)
            {
                this.JailBugs[i] = new List<long>();
            }
        }

        public string GetJailBugsList(int priority)
        {
            return ListToString(JailBugs[priority]);
        }

        public string GetResolvedBugsList()
        {
            return ListToString(ResolvedBugs);
        }

        public int TotalResolvedBugCount
        {
            get { return ResolvedBugs.Count; }
        }

        public string WorkItemPendingBugsAsString
        {
            get
            {
                return BugListToString(this.WorkItemPendingBugs);
            }
        }

        public string WorkItemFinishedBugsAsString
        {
            get
            {
                return BugListToString(this.WorkItemFinishedBugs);
            }
        }

        public string BugJailDescription
        {
            get
            {
                if (TotalResolvedBugCount > 5)
                {
                    return "More than 5 resolved bugs.";
                }
                else if (Pri0BugCount > 0)
                {
                    return "More than 0 pri-0 bugs.";
                }
                else if (Pri1BugCount > 3)
                {
                    return "More than 3 pri-1 bugs.";
                }
                else if (Pri2BugCount > 5)
                {
                    return "More than 5 pri-2 bugs.";
                }
                else if (Pri3BugCount > 7)
                {
                    return "More than 7 pri-3 bugs.";
                }
                else
                {
                    return "Not in bug jail.";
                }
            }
        }

        public bool IsInBugJail
        {
            get
            {
                return
                    (TotalResolvedBugCount > 5) ||
                    (Pri0BugCount > 0) ||
                    (Pri1BugCount > 3) ||
                    (Pri2BugCount > 5) ||
                    (Pri3BugCount > 6);
            }
        }

        #region Private methods.

        private string BugListToString(List<BugRecord> bugList)
        {
            StringBuilder builder;

            if (bugList == null)
            {
                throw new ArgumentNullException("bugList");
            }

            if (bugList.Count == 0)
            {
                return "(none)";
            }

            builder = new StringBuilder(bugList.Count * 12);
            foreach(BugRecord record in bugList)
            {
                if (builder.Length > 0) builder.Append(" - ");
                builder.Append(record.BugId);
            }

            return builder.ToString();
        }

        private static string ListToString(List<long> list)
        {
            StringBuilder builder;

            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            builder = new StringBuilder(list.Count * 12);
            foreach(long bug in list)
            {
                if (builder.Length > 0) builder.Append(" - ");
                builder.Append(bug);
            }
            return builder.ToString();
        }

        #endregion Private methods.
    }

    /// <summary>
    /// Provides information about bug jail status for testers.
    /// </summary>
    public class BugJailReport: ReportBase
    {
        #region Constructors.

        public BugJailReport(): base(null, null, null) { }

        /// <summary>
        /// Initializes a new BugJailReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public BugJailReport(XmlWriter excelWriter, XmlWriter wordWriter,
            FundamentalReportOptions options): base(excelWriter, wordWriter, options)
        {

        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Writes the report to the configured outputs.</summary>
        public override void WriteReport()
        {
            List<BugJailReportRecord> records;

            base.WriteReport();
            records = Execute();

            // Write to Excel.
            WriteStartWorksheet("BugJail");
            WriteHeaderRow(new string[] {
                "Tester" , "Pri0", "Pri1", "Pri2", "Pri3",
                "Total Resolved", "In Bug Jail Because", "Work Item Finished Bugs" },
                new int[] { 40, 22, 22, 22, 22, 70, 92, 216 });
            foreach(BugJailReportRecord record in records)
            {
                WriteStartRow();
                WriteCellAsString(record.Tester);
                WriteCellAsNumber(record.Pri0BugCount);
                WriteCellAsNumber(record.Pri1BugCount);
                WriteCellAsNumber(record.Pri2BugCount);
                WriteCellAsNumber(record.Pri3BugCount);
                WriteCellAsNumber(record.TotalResolvedBugCount);
                WriteCellAsString(record.BugJailDescription);
                WriteCellAsString(record.WorkItemFinishedBugsAsString);
                WriteEndRow();
            }
            WriteEndWorksheet();

            // Write to Word.
            WriteStartSection("Bug Jail");
            foreach(BugJailReportRecord record in records)
            {
                WriteStyledParagraph("Bugs for " + record.Tester, true, false, null);
                WriteParagraph("Resolved Bugs: " + record.GetResolvedBugsList());
                WriteParagraph("Pri 0: " + record.GetJailBugsList(0));
                WriteParagraph("Pri 1: " + record.GetJailBugsList(1));
                WriteParagraph("Pri 2: " + record.GetJailBugsList(2));
                WriteParagraph("Pri 3: " + record.GetJailBugsList(3));
                if (record.IsInBugJail)
                {
                    WriteStyledParagraph(record.Tester + " is in bug jail. " +
                        record.BugJailDescription, true, false, null);
                }
                else
                {
                    WriteParagraph(record.Tester +
                        " is not in bug jail - happy coding!");
                }
                if (record.WorkItemFinishedBugs.Count > 0)
                {
                    WriteParagraph("Bugs that are still pending regression coverage " +
                        "but have already had their work items closed: " +
                        record.WorkItemFinishedBugsAsString);
                }
                WriteEmptyParagraph();

                WriteParagraph("The following bugs need to have regression " +
                    "coverage but they have work items assigned: " +
                    record.WorkItemPendingBugsAsString);
                WriteEmptyParagraph();
            }
            WriteEndSection();
        }

        public List<BugJailReportRecord> Execute()
        {
            return CreateBugReportRecords(ListBugRecords());
        }

        #endregion Public methods.

        #region Private methods.

        private static string FieldValue(PS.DatastoreItem item, string fieldName)
        {
            return ProductStudioQuery.FieldValue(item, fieldName);
        }

        private List<BugRecord> ListBugRecords()
        {
            PS.Directory directory;
            PS.Product product;
            PS.Product tasksProduct;
            PS.Datastore tasksStore;
            PS.Query query;
            PS.DatastoreItemList items;
            PS.Datastore store;
            List<BugRecord> result;
            string queryXml;
            string[] fieldNames = new string[] { "ID", "Priority", "Status", "Assigned To", "Test Followup Assigned To", "Status", "Title", "RelatedLinkCount" };

            // Notes (press Ctrl while running query in PS to paste to clipboard)
            // TreeID 12541 is \Desktop Technologies\Client Platform\User Interaction Services (UIS)\
            // TreeID 12543 is \Desktop Technologies\Client Platform\User Interaction Services (UIS)\Unified Input Manager

            queryXml = "<Query>" +
                "<Group GroupOperator='And'>" +
                @"<Expression Column='TreeID' Operator='under'><Number>12541</Number></Expression>" +
                @"<Expression Column='TreeID' Operator='notUnder'><Number>12543</Number></Expression>" +
                @"<Expression Column='Status' Operator='notEquals'><String>Active</String></Expression>" +
                @"<Group GroupOperator='Or'>" +
                @" <Expression Column='Status' Operator='equals'><String>Resolved</String></Expression>" +
                @" <Group GroupOperator='And'>" +
                @"  <Expression Column='Closed Date' Operator='equalsGreater'><DateTime>2004-06-14</DateTime></Expression>" +
                @"  <Expression Column='Test Followup Status' Operator='notEquals'><String>Closed</String></Expression>" +
                @"  <Expression Column='Test Followup Status' Operator='notEquals'><String>Not Applicable</String></Expression>" +
                @" </Group>" +
                @"</Group>" +
                "</Group></Query>";

            result = new List<BugRecord>();
            directory = new PS.DirectoryClass();
            directory.Connect("", "", "");
            try
            {
                product = directory.GetProductByName(BugsProductName);
                store = product.Connect("", "", "");

                tasksProduct = directory.GetProductByName(TasksProductName);
                tasksStore = tasksProduct.Connect("", "", "");

                query = new PS.QueryClass();
                query.CountOnly = false;
                query.SelectionCriteria = queryXml;
                query.DatastoreItemType = ProductStudio.PsDatastoreItemTypeEnum.psDatastoreItemTypeBugs;

                items = new PS.DatastoreItemListClass();
                items.Datastore = store;
                items.Query = query;

                query.QueryFields.Clear();
                foreach(string s in fieldNames)
                {
                    query.QueryFields.Add(store.FieldDefinitions[s]);
                }
                items.Execute();

                for (int i = 0; i < items.DatastoreItems.Count; i++)
                {
                    PS.DatastoreItem item;
                    PS.IBug bug;
                    BugRecord record;

                    item = items.DatastoreItems[i];
                    record = new BugRecord();
                    record.AssignedTo = FieldValue(item, "Assigned To");
                    record.Status = FieldValue(item, "Status");
                    record.BugId = long.Parse(FieldValue(item, "ID"));
                    record.Priority = int.Parse(FieldValue(item, "Priority"));
                    record.TestFollowupAssignedTo = FieldValue(item, "Test Followup Assigned To");
                    if (Int32.Parse(FieldValue(item, "RelatedLinkCount")) > 0)
                    {
                        item.Edit(PS.PsItemEditActionEnum.psDatastoreItemEditActionEdit, "",
                            ProductStudio.PsApplyRulesMask.psApplyRulesDefaults);
                        bug = (PS.IBug) item;
                        if (bug.RelatedLinks.Count > 0)
                        {
                            foreach(PS.IRelatedLink link in bug.RelatedLinks)
                            {
                                if (link.ProductName == TasksProductName)
                                {
                                    bool isTestWorkItem;
                                    bool isClosed;
                                    CheckWorkItem(tasksStore, link.RelatedBugID, out isTestWorkItem, out isClosed);
                                    if (isTestWorkItem)
                                    {
                                        record.HasLinkedTestWorkItem = true;
                                        if (isClosed && record.OpenTestWorkItems.Count == 0)
                                        {
                                            record.HasClosedLinkedTestWorkItem = true;
                                        }
                                        else
                                        {
                                            record.HasClosedLinkedTestWorkItem = false;
                                            record.OpenTestWorkItems.Add(link.RelatedBugID);
                                        }
                                    }
                                }
                            }
                        }
                        item.Reset(false);
                    }
                    result.Add(record);
                }
            }
            finally
            {
                directory.Disconnect();
            }
            return result;
        }

        private static void CheckWorkItem(PS.Datastore store, int itemId,
            out bool isTestWorkItem, out bool isClosed)
        {
            PS.Query query;
            PS.DatastoreItemList items;
            string queryXml;
            PS.DatastoreItem item;
            string status;
            string[] fieldNames = new string[] { "Status", "Issue type" };

            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

            queryXml = "<Query><Expression Column='ID' Operator='equals'><Number>" +
                itemId.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                "</Number></Expression></Query>";

            query = new PS.QueryClass();
            query.CountOnly = false;
            query.SelectionCriteria = queryXml;

            items = new PS.DatastoreItemListClass();
            items.Datastore = store;
            items.Query = query;

            query.QueryFields.Clear();
            foreach(string s in fieldNames)
            {
                query.QueryFields.Add(store.FieldDefinitions[s]);
            }
            items.Execute();

            if (items.DatastoreItems.Count == 0)
            {
                throw new InvalidOperationException("Cannot find work item " + itemId);
            }
            item = items.DatastoreItems[0];
            if (item.Fields["Issue type"].Value.ToString() == "Test Work Item")
            {
                isTestWorkItem = true;
                status = item.Fields["Status"].Value.ToString();
                isClosed = status == "Resolved" || status == "Closed";
            }
            else
            {
                isTestWorkItem = false;
                isClosed = false;
            }
        }

        private List<BugJailReportRecord> CreateBugReportRecords(List<BugRecord> records)
        {
            List<BugJailReportRecord> result;
            Dictionary<string,BugJailReportRecord> bugJailReports;

            bugJailReports = new Dictionary<string,BugJailReportRecord>();
            foreach(BugRecord bugRecord in records)
            {
                BugJailReportRecord reportRecord;
                bool countsForBugJail;

                if (bugJailReports.ContainsKey(bugRecord.TestFollowupAssignedTo))
                {
                    reportRecord = bugJailReports[bugRecord.TestFollowupAssignedTo];
                }
                else
                {
                    reportRecord = new BugJailReportRecord();
                    reportRecord.Tester = bugRecord.TestFollowupAssignedTo;
                    bugJailReports[bugRecord.TestFollowupAssignedTo] = reportRecord;
                }

                if (bugRecord.IsResolved)
                {
                    reportRecord.ResolvedBugs.Add(bugRecord.BugId);
                }
                countsForBugJail = !bugRecord.HasLinkedTestWorkItem ||
                    bugRecord.HasClosedLinkedTestWorkItem;
                if (countsForBugJail)
                {
                    reportRecord.JailBugs[bugRecord.Priority]
                        .Add(bugRecord.BugId);
                }
                if (bugRecord.HasClosedLinkedTestWorkItem)
                {
                    reportRecord.WorkItemFinishedBugs.Add(bugRecord);
                }
                else if (bugRecord.HasLinkedTestWorkItem)
                {
                    reportRecord.WorkItemPendingBugs.Add(bugRecord);
                }
            }

            result = new List<BugJailReportRecord>();
            result.AddRange(bugJailReports.Values);
            return result;
        }

        #endregion Private methods.

        const string PsqFileName = @"\\Microsoft\public\Product Studio Files\Editing Resolved Bugs.psq";
    }
}
