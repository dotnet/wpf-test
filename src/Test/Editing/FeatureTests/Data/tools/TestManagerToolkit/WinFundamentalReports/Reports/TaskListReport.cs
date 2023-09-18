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

    /// <summary>
    /// Provides information about the schedule for a specific
    /// milestone.
    /// </summary>
    public class MilestoneSchedule
    {
        #region Constructors.

        /// <summary>Initialize all known milestone schedules.</summary>
        static MilestoneSchedule()
        {
            #region Milestone 10 schedule.

            s_milestone10 = new MilestoneSchedule();
            s_milestone10.Name = "M10";
            s_milestone10.TestCodeCompleteDate = new DateTime(2005, 2, 25);
            s_milestone10.DateRange = new DateRange(
                new DateTime(2004, 10, 11), new DateTime(2005, 4, 3));
            s_milestone10.Checkpoints = new MilestoneCheckpoint[] {
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2004, 10, 11), new DateTime(2004, 11, 5)),
                    "M10-A"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2004, 11, 8), new DateTime(2005, 1, 7)),
                    "M10-B"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 1, 10), new DateTime(2005, 1, 28)),
                    "M10-C"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 1, 31), new DateTime(2005, 2, 18)),
                    "stabilize"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 1, 28), new DateTime(2005, 3, 4)),
                    "zbb drive"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 3,  7), new DateTime(2005, 4, 1)),
                    "zbr drive"),
            };
            s_milestone10.EventDays = new EventDay[] {
                new EventDay(new DateTime(2005,  1, 17), "Begin Checkpoint C coding"),
                new EventDay(new DateTime(2005,  2,  4), "Checkpoint C/Beta 1 CC"),
                new EventDay(new DateTime(2005,  2, 25), "Beta 1 TCC"),
            };
            s_milestone10.ScheduleBlocks = new ScheduleBlock[] {
                ScheduleBlock.ForWork (new DateTime(2004, 10, 11), 5),
                ScheduleBlock.ForWork (new DateTime(2004, 10, 18), 5),
                ScheduleBlock.ForWork (new DateTime(2004, 10, 25), 3),
                ScheduleBlock.ForOther(new DateTime(2004, 10, 28), 2, "Federal ad-hoc"),
                ScheduleBlock.ForOther(new DateTime(2004, 11,  1), 1, "Federal ad-hoc"),
                ScheduleBlock.ForWork (new DateTime(2004, 11,  2), 4),
                ScheduleBlock.ForWork (new DateTime(2004, 11,  8), 5),
                ScheduleBlock.ForWork (new DateTime(2004, 11, 15), 5),
                ScheduleBlock.ForOther(new DateTime(2004, 11, 22), 5, "Thanksgiving"),
                ScheduleBlock.ForOther(new DateTime(2004, 11, 29), 5, "Federal ad-hoc"),
                ScheduleBlock.ForWork (new DateTime(2004, 12,  6), 5),
                ScheduleBlock.ForOther(new DateTime(2004, 12, 13), 5, "App Building"),
                ScheduleBlock.ForOther(new DateTime(2004, 12, 20), 5, "Holiday"),
                ScheduleBlock.ForOther(new DateTime(2004, 12, 27), 5, "App Building"),
                ScheduleBlock.ForOther(new DateTime(2005,  1,  3), 5, "App Building"),
                ScheduleBlock.ForWork (new DateTime(2005,  1, 10), 5),
                ScheduleBlock.ForWork (new DateTime(2005,  1, 17), 5),
                ScheduleBlock.ForWork (new DateTime(2005,  1, 24), 5),
                ScheduleBlock.ForWork (new DateTime(2005,  1, 31), 5),
                ScheduleBlock.ForOther(new DateTime(2005,  2,  7), 5, "Federal ad-hoc"),
                ScheduleBlock.ForWork (new DateTime(2005,  2, 14), 5),
                ScheduleBlock.ForWork (new DateTime(2005,  2, 21), 5),
                ScheduleBlock.ForWork (new DateTime(2005,  2, 28), 5),
                ScheduleBlock.ForOther(new DateTime(2005,  3,  7), 5, "Federal ad-hoc"),
                ScheduleBlock.ForOther(new DateTime(2005,  3, 14), 5, "Resolve Bugs"),
                ScheduleBlock.ForOther(new DateTime(2005,  3, 21), 5, "Resolve Bugs"),
                ScheduleBlock.ForOther(new DateTime(2005,  3, 28), 5, "Resolve Bugs"),
            };
            #endregion Milestone 10 schedule.

            #region Milestone 11 schedule.

            s_milestone11 = new MilestoneSchedule();
            s_milestone11.Name = "M11";
            s_milestone11.TestCodeCompleteDate = new DateTime(2005, 7, 8);
            s_milestone11.DateRange = new DateRange(
                new DateTime(2004, 4, 4), new DateTime(2005, 8, 22));
            s_milestone11.Checkpoints = new MilestoneCheckpoint[] {
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 4,  1), new DateTime(2005, 4, 29)),
                    "sec/perf"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 5,  2), new DateTime(2005, 5, 20)),
                    "M11-A"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 5, 23), new DateTime(2005, 6,  3)),
                    "M11-As"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 6,  6), new DateTime(2005, 6, 24)),
                    "M11-B"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 6, 27), new DateTime(2005, 7,  8)),
                    "M11-Bs"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 7, 11), new DateTime(2005, 7, 29)),
                    "ZBR drive"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 8,  1), new DateTime(2005, 8, 26)),
                    "DCRs"),
            };
            s_milestone11.EventDays = new EventDay[] {
                new EventDay(new DateTime(2005,  6,  6), "Fork for CTP/RI"),
                new EventDay(new DateTime(2005,  6, 24), "Dev Code Complete"),
                new EventDay(new DateTime(2005,  7,  8), "Test Code Complete"),
                new EventDay(new DateTime(2005,  7, 22), "ZBB"),
                new EventDay(new DateTime(2005,  8,  1), "ZBR/Final Test Pass"),
                new EventDay(new DateTime(2005,  8, 15), "Final PDC build"),
                new EventDay(new DateTime(2005,  8, 26), "LH Beta2 Feature Complete"),
                new EventDay(new DateTime(2005, 11,  2), "LH Beta2 Released"),
                new EventDay(new DateTime(2006,  3, 22), "LH RC0"),
                new EventDay(new DateTime(2006,  5, 10), "LH RC1"),
                new EventDay(new DateTime(2006,  6, 21), "LH RTM"),
            };
            s_milestone11.ScheduleBlocks = new ScheduleBlock[] {
                ScheduleBlock.ForOther(new DateTime(2005, 4,  4), 5, "M11 Refresh"),
                ScheduleBlock.ForOther(new DateTime(2005, 4, 11), 5, "Security"),
                ScheduleBlock.ForOther(new DateTime(2005, 4, 18), 5, "Security"),
                ScheduleBlock.ForOther(new DateTime(2005, 4, 25), 5, "Security"),
                ScheduleBlock.ForWork (new DateTime(2005, 5,  2), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 5,  9), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 5, 16), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 5, 23), 5),
                ScheduleBlock.ForOther(new DateTime(2005, 5, 30), 1, "Memorial Day"),
                ScheduleBlock.ForWork (new DateTime(2005, 5, 31), 4),
                ScheduleBlock.ForOther(new DateTime(2005, 6,  6), 5, "Federal ad-hoc"),
                ScheduleBlock.ForWork (new DateTime(2005, 6, 13), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 6, 20), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 6, 27), 5),
                ScheduleBlock.ForOther(new DateTime(2005, 7,  4), 1, "4th of July"),
                ScheduleBlock.ForWork (new DateTime(2005, 7,  5), 4),
                ScheduleBlock.ForOther(new DateTime(2005, 7, 11), 5, "ZBR drive"),
                ScheduleBlock.ForOther(new DateTime(2005, 7, 18), 5, "ZBR drive"),
                ScheduleBlock.ForOther(new DateTime(2005, 7, 25), 5, "ZBR drive"),
                ScheduleBlock.ForWork (new DateTime(2005, 8,  1), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 8,  8), 5),
                ScheduleBlock.ForOther(new DateTime(2005, 8, 15), 5, "Test pass"),
                ScheduleBlock.ForWork (new DateTime(2005, 8, 22), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 8, 22), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 8, 29), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 9,  5), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 9, 12), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 9, 19), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 9, 26), 5),
            };

            #endregion Milestone 11 schedule.

            #region Milestone RC0 schedule.

            s_milestoneRC0 = new MilestoneSchedule();
            s_milestoneRC0.Name = "RC0";
            s_milestoneRC0.TestCodeCompleteDate = new DateTime(2006, 5, 1);
            s_milestoneRC0.DateRange = new DateRange(
                new DateTime(2005, 9, 29), new DateTime(2006, 5, 25));
            // These aren't very accurate.
            s_milestoneRC0.Checkpoints = new MilestoneCheckpoint[] {
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2005, 9, 29), new DateTime(2005, 12, 31)),
                    "DCRs"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2006, 1,  1), new DateTime(2006,  1, 15)),
                    "NewYear"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2006, 1, 16), new DateTime(2006, 2, 20)),
                    "Quality"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2006, 2, 21), new DateTime(2006, 4,  8)),
                    "Sec"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2006, 4,  9), new DateTime(2006, 5,  8)),
                    "Balance"),
                new MilestoneCheckpoint(
                    new DateRange(new DateTime(2006, 5,  9), new DateTime(2006, 5, 25)),
                    "Lockdown"),
            };
            s_milestoneRC0.EventDays = new EventDay[] {
                new EventDay(new DateTime(2005, 10, 28), "Halloween"),
            };
            s_milestoneRC0.ScheduleBlocks = new ScheduleBlock[] {
                ScheduleBlock.ForWork (new DateTime(2005, 10,  3), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 10, 10), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 10, 17), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 10, 24), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 10, 31), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 11,  7), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 11, 14), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 11, 21), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 11, 28), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 12,  5), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 12, 12), 5),
                ScheduleBlock.ForWork (new DateTime(2005, 12, 19), 5),
                ScheduleBlock.ForOther(new DateTime(2005, 12, 26), 5, "Holidays"),
                ScheduleBlock.ForWork (new DateTime(2006,  1,  2), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  1,  2), 5),
                ScheduleBlock.ForOther(new DateTime(2006,  1,  9), 5, "Federal ad-hoc"),
                ScheduleBlock.ForWork (new DateTime(2006,  1, 16), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  1, 23), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  1, 30), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  2,  6), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  2, 13), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  2, 20), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  2, 27), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  3,  6), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  3, 13), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  3, 20), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  3, 27), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  4,  3), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  4, 10), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  4, 17), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  4, 24), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  5,  1), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  5,  8), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  5, 15), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  5, 22), 5),
                ScheduleBlock.ForWork (new DateTime(2006,  5, 29), 5),
            };

            #endregion Milestone RC0 schedule.

            // Initialize the overflow schedule.
            if (s_overflowMilestone == null)
            {
                s_overflowMilestone = new MilestoneSchedule();
                s_overflowMilestone.Name = "Overflow";
                s_overflowMilestone.TestCodeCompleteDate = Current.TestCodeCompleteDate;
                s_overflowMilestone.DateRange = new DateRange(
                    new DateTime(2002, 1, 1), new DateTime(2006, 12, 31));
                s_overflowMilestone.Checkpoints = new MilestoneCheckpoint[] {
                    new MilestoneCheckpoint(
                        new DateRange(new DateTime(2002, 1,  1), new DateTime(2006, 12, 31)),
                        "overflow"),
                };
                s_overflowMilestone.EventDays = new EventDay[0];
                s_overflowMilestone.ScheduleBlocks = new ScheduleBlock[] {
                    ScheduleBlock.ForWork (new DateTime(2002, 1,  1), 365 * 6),
                };
            }
        }

        /// <summary>Hides the constructor.</summary>
        private MilestoneSchedule() { }

        #endregion Constructors.

        #region Public properties.

        /// <summary>Checkpoints in the milestone.</summary>
        public MilestoneCheckpoint[] Checkpoints
        {
            get { return this._checkpoints; }
            set { this._checkpoints = value; }
        }

        /// <summary>Static accessor for the current milestone.</summary>
        public static MilestoneSchedule Current
        {
            get { return MilestoneRC0; }
        }

        /// <summary>Inclusive range of milestone dates.</summary>
        public DateRange DateRange
        {
            get { return this._dateRange; }
            set { this._dateRange = value; }
        }

        /// <summary>Special event days.</summary>
        public EventDay[] EventDays
        {
            get { return this._eventDays; }
            set { this._eventDays = value; }
        }

        /// <summary>Whether this milestone is an overflow dummy at the end of the schedule.</summary>
        public bool IsOverflow
        {
            get { return this == s_overflowMilestone; }
        }

        /// <summary>Static accessor for M10.</summary>
        public static MilestoneSchedule Milestone10
        {
            get { return s_milestone10; }
        }

        /// <summary>Static accessor for M11.</summary>
        public static MilestoneSchedule Milestone11
        {
            get { return s_milestone11; }
        }

        /// <summary>Static accessor for RC0.</summary>
        public static MilestoneSchedule MilestoneRC0
        {
            get { return s_milestoneRC0; }
        }

        /// <summary>Milestone name.</summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>Static accessor for the overflow milestone schedule.</summary>
        public static MilestoneSchedule OverflowMilestone
        {
            get { return s_overflowMilestone; }
        }

        /// <summary>Blocks of time in schedule.</summary>
        public ScheduleBlock[] ScheduleBlocks
        {
            get { return this._scheduleBlocks; }
            set { this._scheduleBlocks = value; }
        }

        /// <summary>Date of Test Code Complete for milestone.</summary>
        public DateTime TestCodeCompleteDate
        {
            get { return this._testCodeCompleteDate; }
            set { this._testCodeCompleteDate = value; }
        }

        #endregion Public properties.

        #region Public methods.

        public MilestoneCheckpoint GetCheckpointForDate(DateTime date)
        {
            if (!_dateRange.IsDateWithin(date))
            {
                throw ArgumentOutOfDateRange(date, "date");
            }

            foreach (MilestoneCheckpoint result in Checkpoints)
            {
                if (result.DateRange.IsDateWithin(date))
                {
                    return result;
                }
            }

            throw new InvalidOperationException(
                "There is no checkpoint in milestone " + Name +
                " for date " + date);
        }

        /// <summary>Gets the description for any events on the given date.</summary>
        /// <param name="date">Date to get event descriptions for.</param>
        /// <returns>The event description, null if there is no event for the date.</returns>
        public string GetEventDescription(DateTime date)
        {
            foreach (EventDay eventDay in this.EventDays)
            {
                if (eventDay.Date == date)
                {
                    return eventDay.Name;
                }
            }

            return null;
        }

        /// <summary>Gets the milestone schedule that includes the specified date.</summary>
        /// <param name="date">Date to look for.</param>
        /// <returns>The milestone schedule that includes the specified date.</returns>
        /// <remarks>There is an overflow milestone used for out-of-bound dates.</remarks>
        public static MilestoneSchedule GetForDate(DateTime date)
        {
            if (Milestone10.DateRange.IsDateWithin(date)) return Milestone10;
            if (Milestone11.DateRange.IsDateWithin(date)) return Milestone11;
            if (MilestoneRC0.DateRange.IsDateWithin(date)) return MilestoneRC0;
            return OverflowMilestone;
        }

        public ScheduleBlock GetScheduleBlockForDate(DateTime date)
        {
            foreach(ScheduleBlock block in ScheduleBlocks)
            {
                if (block.DateRange.IsDateWithin(date))
                {
                    return block;
                }
            }

            return null;
        }

        public int GetScheduledDaysBetween(DateTime startDate,
            DateTime targetDate)
        {
            int result;
            bool flipDays;

            if (!_dateRange.IsDateWithin(startDate))
            {
                throw ArgumentOutOfDateRange(startDate, "startDate");
            }
            if (!_dateRange.IsDateWithin(targetDate))
            {
                throw ArgumentOutOfDateRange(targetDate, "targetDate");
            }

            result = 0;
            flipDays = startDate > targetDate;
            if (flipDays)
            {
                DateTime temp;
                temp = startDate;
                startDate = targetDate;
                targetDate = temp;
            }

            System.Diagnostics.Debug.Assert(startDate <= targetDate);
            while (startDate < targetDate)
            {
                if (IsScheduledWorkDay(startDate))
                {
                    result++;
                }
                startDate = startDate.AddDays(1);
            }

            if (flipDays)
            {
                result = -result;
            }
            return result;
        }

        /// <summary>
        /// Returns whether the specified milestone date can be
        /// scheduled for work.
        /// </summary>
        public bool IsScheduledWorkDay(DateTime date)
        {
            ScheduleBlock block;

            if (!_dateRange.IsDateWithin(date))
            {
                throw ArgumentOutOfDateRange(date, "date");
            }

            if (DateRange.IsDateWeekend(date))
            {
                return false;
            }

            block = GetScheduleBlockForDate(date);
            if (block == null)
            {
                throw new InvalidOperationException(
                    "Cannot find schedule block for date " + date +
                    " in milestone " + Name);
            }
            else
            {
                return block.CanScheduleWork;
            }
        }

        #endregion Public methods.

        #region Private methods.

        private Exception ArgumentOutOfDateRange(DateTime date,
            string argumentName)
        {
            return new ArgumentException("date is not within milestone " +
                Name + ": " + date, argumentName);
        }

        #endregion Private methods.

        #region Private fields.

        private string _name;
        private DateRange _dateRange;
        private MilestoneCheckpoint[] _checkpoints;
        private EventDay[] _eventDays;
        private ScheduleBlock[] _scheduleBlocks;
        private DateTime _testCodeCompleteDate;
        private static MilestoneSchedule s_milestone10;
        private static MilestoneSchedule s_milestone11;
        private static MilestoneSchedule s_milestoneRC0;
        private static MilestoneSchedule s_overflowMilestone;

        #endregion Private fields.
    }

    public struct MilestoneCheckpoint
    {
        #region Constructors.

        public MilestoneCheckpoint(DateRange dateRange, string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            this._dateRange = dateRange;
            this._name = name;
        }

        #endregion Constructors.

        #region Public properties.

        public DateRange DateRange { get { return this._dateRange; } }
        public string Name { get { return this._name; } }

        #endregion Public properties.

        #region Private fields.

        private readonly string _name;
        private readonly DateRange _dateRange;

        #endregion Private fields.
    }

    public struct EventDay
    {
        #region Constructors.

        public EventDay(DateTime date, string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            this._date = date;
            this._name = name;
        }

        #endregion Constructors.

        #region Public properties.

        public DateTime Date { get { return this._date; } }
        public string Name { get { return this._name; } }

        #endregion Public properties.

        #region Private fields.

        private readonly DateTime _date;
        private readonly string _name;

        #endregion Private fields.
    }

    public struct DateRange
    {
        #region Constructors.

        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                throw new ArgumentException(
                    "endDate cannot be before startDate", "endDate");
            }

            this._startDate = startDate;
            this._endDate = endDate;
        }

        #endregion Constructors.

        #region Public properties.

        public DateTime StartDate { get { return this._startDate; } }
        public DateTime EndDate { get { return this._endDate; } }

        #endregion Public properties.

        #region Public methods.

        public bool IsDateWithin(DateTime date)
        {
            return (_startDate <= date && date <= _endDate);
        }

        /// <summary>
        /// Checks whether the specified date is a Saturday or Sunday.
        /// </summary>
        /// <param name="date">Date to check.</param>
        /// <returns>
        /// true if <paramref name="date"/> is a Saturday or Sunday;
        /// false otherwise.
        /// </returns>
        public static bool IsDateWeekend(DateTime date)
        {
            return (date.DayOfWeek == DayOfWeek.Sunday) ||
                (date.DayOfWeek == DayOfWeek.Saturday);
        }

        #endregion Public methods.

        #region Private fields.

        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        #endregion Private fields.
    }

    public class ScheduleBlock
    {
        #region Constructors.

        private ScheduleBlock() { }

        public static ScheduleBlock ForWork(DateTime startDate, int dayCount)
        {
            ScheduleBlock result;

            result = new ScheduleBlock();

            result._dateRange = new DateRange(startDate, startDate.AddDays(dayCount - 1));
            result._canScheduleWork = true;
            result._description = "Scheduled Work";

            return result;
        }

        public static ScheduleBlock ForOther(DateTime startDate, int dayCount,
            string description)
        {
            ScheduleBlock result;

            result = new ScheduleBlock();

            result._dateRange = new DateRange(startDate, startDate.AddDays(dayCount - 1));
            result._canScheduleWork = false;
            result._description = description;

            return result;
        }

        #endregion Constructors.

        #region Public properties.

        public DateRange DateRange { get { return this._dateRange; } }
        public bool CanScheduleWork { get { return this._canScheduleWork; } }
        public string Description { get { return this._description; } }

        #endregion Public properties.

        #region Private fields.

        private DateRange _dateRange;
        private bool _canScheduleWork;
        private string _description;

        #endregion Private fields.
    }

    public class ScheduleReport: ReportBase
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new ScheduleReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public ScheduleReport(XmlWriter excelWriter, XmlWriter wordWriter,
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

            WriteWordReport();
            WriteExcelReport();
            WriteWordScheduleReport();
        }

        #endregion Public methods.

        #region Private methods.

        /// <summary>Gets the field value for the given item.</summary>
        /// <param name="item">PS item to query.</param>
        /// <param name="fieldName">Field to return value for.</param>
        /// <returns>The string value of the value, an empty string if it's null.</returns>
        private static string FieldValue(PS.DatastoreItem item, string fieldName)
        {
            return ProductStudioQuery.FieldValue(item, fieldName);
        }

        /// <summary>
        /// Retrieves the styles for the week day of the specified date.
        /// </summary>
        /// <param name="date">Date to get styles for.</param>
        /// <param name="cellStyle">On return, style name for cells.</param>
        /// <param name="cellDateStyle">On return, style name for cells with dates.</param>
        /// <returns>true if the date falls on a weekend, false otherwise.</returns>
        private static bool GetWeekDayStyles(DateTime date,
            out string cellStyle, out string cellDateStyle)
        {
            if (DateRange.IsDateWeekend(date))
            {
                cellStyle = WeekendStyleId;
                cellDateStyle = WeekendDateStyleId;
                return true;
            }
            else
            {
                cellStyle = FutureStyleId;
                cellDateStyle = FutureDateStyleId;
                return false;
            }
        }

        /// <summary>
        /// Retrieves all necessary data from other stores and assigns it
        /// to instance fields, with all necessary 'massaging' for processing,
        /// like sorting or pivoting.
        /// </summary>
        private void RetrieveData()
        {
            PS.Directory directory;
            PS.Product tasksProduct;
            PS.Datastore tasksStore;
            string queryXml;
            string[] fieldNames;

            // Notes (press Ctrl while running query in PS to paste to clipboard)
            // TreeID 408 is \Editing\
            // queryXml = "<Query>" +
            //     "<Group GroupOperator='And'>" +
            //     @"<Expression Column='TreeID' Operator='under'><Number>408</Number></Expression>" +
            //     @"<Expression Column='Status' Operator='equals'><String>Active</String></Expression>" +
            //     @"<Expression Column='Milestone' Operator='equals'><String>" + MilestoneSchedule.Current.Name + @"</String></Expression>" +
            //     @"<Expression Column='Issue type' Operator='equals'><String>Test Work Item</String></Expression>" +
            //     "</Group></Query>";

            queryXml = string.Join("\r\n", Options.TaskListQuery);
            _workItems = new List<WorkItem>();
            directory = new PS.DirectoryClass();
            fieldNames = new string[] {
                "ID", "Assigned To", "Title", "Priority", "Estimated Calendar Days",
                "Remaining Calendar Days", "Actual Calendar Days", "TestOwner",
                "Checkpoint", "Milestone", "Spec URL", "Sub TaskType1",
                "Confidence", "Implementation Order", "Changed Date", "Due Date"
            };

            directory.Connect("", "", "");
            try
            {
                PS.Query query;
                PS.DatastoreItemList items;

                tasksProduct = directory.GetProductByName(TasksProductName);
                tasksStore = tasksProduct.Connect("", "", "");

                query = new PS.QueryClass();
                query.CountOnly = false;
                query.SelectionCriteria = queryXml;
                query.DatastoreItemType = ProductStudio.PsDatastoreItemTypeEnum.psDatastoreItemTypeBugs;

                items = new PS.DatastoreItemListClass();
                items.Datastore = tasksStore;
                items.Query = query;

                query.QueryFields.Clear();
                foreach(string s in fieldNames)
                {
                    try
                    {
                        query.QueryFields.Add(tasksStore.FieldDefinitions[s]);
                    }
                    catch(Exception exception)
                    {
                        string message;
                        message = "Error: " + exception.Message +
                            "\r\n\r\nAccessing field: " + s + "\r\n\r\n" +
                            "Available fields:\r\n";
                        foreach(PS.FieldDefinition d in tasksStore.FieldDefinitions)
                        {
                            message += "[" + d.Name + "] ";
                        }
                        System.Windows.Forms.MessageBox.Show(message);
                        throw;
                    }
                }
                items.Execute();

                for (int i = 0; i < items.DatastoreItems.Count; i++)
                {
                    PS.DatastoreItem item;
                    WorkItem workItem;

                    item = items.DatastoreItems[i];
                    workItem = new WorkItem();
                    workItem.Id = long.Parse(FieldValue(item, "ID"));
                    workItem.AssignedTo = FieldValue(item, "Assigned To");
                    workItem.Title = FieldValue(item, "Title");
                    workItem.Priority = int.Parse(FieldValue(item, "Priority"));
                    workItem.EstimatedCalendarDays = int.Parse(FieldValue(item, "Estimated Calendar Days"));
                    workItem.RemainingCalendarDays =
                        (FieldValue(item, "Remaining Calendar Days") == "")? -1 :
                        int.Parse(FieldValue(item, "Remaining Calendar Days"));
                    workItem.ActualCalendarDays =
                        (FieldValue(item, "Actual Calendar Days") == "")? 0 :
                        int.Parse(FieldValue(item, "Actual Calendar Days"));
                    workItem.TestOwner = FieldValue(item, "TestOwner");
                    workItem.Checkpoint = FieldValue(item, "Checkpoint");
                    workItem.Milestone = FieldValue(item, "Milestone");
                    workItem.SpecUrl = FieldValue(item, "Spec URL");
                    workItem.SubTaskType1 = FieldValue(item, "Sub TaskType1");
                    workItem.Confidence =
                        (FieldValue(item, "Confidence") == "")? -1 :
                        int.Parse(FieldValue(item, "Confidence"));
                    workItem.ChangedDate = DateTime.Parse(FieldValue(item, "Changed Date"));
                    workItem.ImplementationOrder =
                        (FieldValue(item, "Implementation Order") == "")? -1 :
                        int.Parse(FieldValue(item, "Implementation Order"));
                    workItem.DueDate = FieldValue(item, "Due Date");
                    _workItems.Add(workItem);
                }
            }
            finally
            {
                directory.Disconnect();
            }

            _testerLists = new Dictionary<string, TesterWorkItemList>();
            _testers = new List<string>();
            foreach(WorkItem workItem in _workItems)
            {
                TesterWorkItemList list;

                if (_testerLists.ContainsKey(workItem.TestOwner))
                {
                    list = _testerLists[workItem.TestOwner];
                }
                else
                {
                    list = new TesterWorkItemList();
                    list.Tester = workItem.TestOwner;
                    _testerLists[workItem.TestOwner] = list;
                    _testers.Add(workItem.TestOwner);
                }

                if (workItem.DueDate.Length > 0)
                {
                    list.FixedItems.Add(workItem);
                }
                else
                {
                    list.Add(workItem);
                }
            }

            _testers.Sort();
            foreach(TesterWorkItemList itemList in _testerLists.Values)
            {
                itemList.Sort(WorkItemComparison);
                itemList.AssignConfidenceValues();
            }

            _overscheduledWorkItems = new List<WorkItem>();
        }

        private void WriteWordReport()
        {
            StringBuilder builder;
            DateTime lastAcceptableChangeDate;

            WriteStartSection("Schedule");

            WriteStyledParagraph("Vacation work items without Due Date", true, false, null);
            builder = null;
            foreach(WorkItem item in _workItems)
            {
                if (item.IsVacation && item.DueDate.Length == 0)
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                        builder.Append("The following work items are vacation items " +
                            "with no Due Date. Please fix: ");
                    }
                    builder.Append(item.Id.ToString() + "(" + item.TestOwner + ")  ");
                }
            }
            if (builder == null)
            {
                WriteParagraph("All vacation work items have their Due Date assigned.");
            }
            else
            {
                WriteParagraph(builder.ToString());
            }
            WriteEmptyParagraph();

            WriteStyledParagraph("Work items without Remaining Calendar Days", true, false, null);
            builder = null;
            foreach(WorkItem item in _workItems)
            {
                if (item.RemainingCalendarDays == -1)
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                        builder.Append("The following work items have no Remaining " +
                            "Calendar Days. Please fix: ");
                    }
                    builder.Append(item.Id.ToString() + "(" + item.TestOwner + ")  ");
                }
            }
            if (builder == null)
            {
                WriteParagraph("All work items have their Remaining Calendar Days assigned.");
            }
            else
            {
                WriteParagraph(builder.ToString());
            }
            WriteEmptyParagraph();

            WriteStyledParagraph("Work item updates", true, false, null);
            lastAcceptableChangeDate = DateTime.Today.AddDays(-5);
            foreach(string tester in _testers)
            {
                DateTime lastChange;

                // Note: this may be inaccurate if the last modified
                // work item is now Resolved.
                lastChange = DateTime.MinValue;
                foreach(WorkItem item in _testerLists[tester])
                {
                    if (item.ChangedDate > lastChange)
                    {
                        lastChange = item.ChangedDate;
                    }
                }

                if (lastChange > lastAcceptableChangeDate)
                {
                    WriteParagraph("Last change from " + tester +
                        " is considered up-to-date: " + lastChange + ".");
                }
                else
                {
                    WriteStyledParagraph("Last change from " + tester +
                        " is considered outdated: " + lastChange + ".",
                        true, false, null);
                }
            }
            WriteEmptyParagraph();

            WriteStyledParagraph("Work item without implementation order", true, false, null);
            builder = null;
            foreach(WorkItem item in _workItems)
            {
                if (item.ImplementationOrder == -1)
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                        builder.Append("The following work items have no " +
                            "Implementation Order. Please fix: ");
                    }
                    builder.Append(item.Id.ToString() + "(" + item.TestOwner + ")  ");
                }
            }
            if (builder == null)
            {
                WriteParagraph("All work items have their Implementation Order assigned.");
            }
            else
            {
                WriteParagraph(builder.ToString());
            }
            WriteEmptyParagraph();
        }

        /// <summary>
        /// Writes the Word report based on scheduled data produced by the
        /// Excel report generation and closes the Word section.
        /// </summary>
        private void WriteWordScheduleReport()
        {
            if (_overscheduledWorkItems.Count == 0)
            {
                WriteParagraph("No overdue work items.");
            }
            else
            {
                WriteStyledParagraph("Warning: there are overscheduled work items.", true, false, null);
                WriteParagraph("Overscheduled items:");
                _overscheduledWorkItems.Sort(delegate (WorkItem x, WorkItem y) {
                    int result;

                    result = StringComparer.InvariantCulture.Compare(x.TestOwner, y.TestOwner);
                    if (result == 0)
                    {
                        result = x.ImplementationOrder - y.ImplementationOrder;
                    }
                    return result;
                });
                foreach (WorkItem item in _overscheduledWorkItems)
                {
                    WriteParagraph(item.Title + " - " + item.TestOwner + " (" +
                        item.Id + ")");
                    WriteEmptyParagraph();
                }
            }
            WriteEmptyParagraph();

            WriteEndSection();
        }

        private bool WriteWeekDay(MilestoneSchedule schedule, DateTime date)
        {
            ScheduleBlock block;

            block = schedule.GetScheduleBlockForDate(date);
            if (block != null)
            {
                if (block.CanScheduleWork)
                {
                    return WriteWorkCells(schedule, date);
                }
                else
                {
                    for (int i = 0; i < _testers.Count; i++)
                    {
                        TesterWorkItemList itemList;
                        WorkItem item;

                        itemList = _testerLists[_testers[i]];
                        item = itemList.ScheduleFixedItem(schedule, date);
                        if (item == null)
                        {
                            WriteCellAsData("String", block.Description, OtherWorkStyleId);
                        }
                        else
                        {
                            WriteWorkItem(item);
                        }
                    }
                    return true;
                }
            }
            else
            {
                // Off-schedule work items remain; continue writing these.
                return WriteWorkCells(schedule, date);
            }
        }

        private bool WriteScheduleRow(MilestoneSchedule schedule, DateTime date)
        {
            string cellStyle;
            string cellDateStyle;
            bool shouldContinueWriting;

            // Stop writing unless this gets turned to on somewhere.
            shouldContinueWriting = false;

            GetWeekDayStyles(date, out cellStyle, out cellDateStyle);

            WriteStartRow();
            if (DateRange.IsDateWeekend(date))
            {
                WriteCellAsData("String", "w/end", cellStyle);
            }
            else
            {
                if (schedule.DateRange.IsDateWithin(date))
                {
                    WriteCellAsData("String",
                        schedule.GetCheckpointForDate(date).Name, cellStyle);
                }
                else
                {
                    WriteCellAsData("String", "slip", cellStyle);
                }
            }

            // Write the day of the week.
            WriteCellAsDataWithComment("String",
                date.DayOfWeek.ToString().Substring(0, 3), cellStyle,
                schedule.GetEventDescription(date));

            // Write the date.
            WriteCellAsData("DateTime", date.ToString("s"), cellDateStyle);
            if (schedule.DateRange.IsDateWithin(date))
            {
                WriteCellAsData("Number",
                    (schedule.GetScheduledDaysBetween(date, schedule.TestCodeCompleteDate) + 1)
                    .ToString(), cellStyle);
            }
            else
            {
                WriteCellAsData("String", "-", cellStyle);
            }

            // Write the weekends.
            if (DateRange.IsDateWeekend(date))
            {
                for (int i = 0; i < _testers.Count; i++)
                {
                    WriteCellAsData("String", "", cellStyle);
                }
                shouldContinueWriting = true;
            }
            else
            {
                shouldContinueWriting = WriteWeekDay(schedule, date);
            }
            WriteEndRow();

            return shouldContinueWriting;
        }

        /// <summary>Writes the Excel report (modifies retrieved data).</summary>
        private void WriteExcelReport()
        {
            DateTime date;              // Date being written out.
            MilestoneSchedule schedule; // Schedule being written out.
            bool shouldContinueWriting; // Whether there is still data to be written out.

            // Write the report to Excel.
            WriteStartWorksheet("Schedule");
            WriteHeader();

            date = DateTime.Today;
            shouldContinueWriting = true;
            while (shouldContinueWriting)
            {
                schedule = MilestoneSchedule.GetForDate(date);
                shouldContinueWriting = WriteScheduleRow(schedule, date);

                date = date.AddDays(1);
                shouldContinueWriting =
                    shouldContinueWriting || (date <= schedule.DateRange.EndDate && !schedule.IsOverflow);
            }
            WriteEndWorksheet();
        }

        private bool WriteWorkCells(MilestoneSchedule schedule, DateTime date)
        {
            bool shouldContinueWriting;

            shouldContinueWriting = false;
            for (int i = 0; i < _testers.Count; i++)
            {
                WorkItem dateWork;
                TesterWorkItemList itemList;

                itemList = _testerLists[_testers[i]];

                // Vacation is scheduled before work.
                dateWork = itemList.ScheduleFixedItem(schedule, date);
                if (dateWork == null)
                {
                    dateWork = itemList.ScheduleWork(schedule, date);
                }

                if (dateWork == null)
                {
                    WriteCellAsString("[no work]");
                }
                else
                {
                    WriteWorkItem(dateWork);
                    if (date > schedule.DateRange.EndDate &&
                        _overscheduledWorkItems.IndexOf(dateWork) == -1)
                    {
                        _overscheduledWorkItems.Add(dateWork);
                    }
                    shouldContinueWriting = true;
                }
            }

            return shouldContinueWriting;
        }

        /// <summary>Writes the specified work item to a cell.</summary>
        /// <param name="item">Item to write.</param>
        private void WriteWorkItem(WorkItem item)
        {
            string htmlComment;
            string styleId;

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (item.PlacedOnSchedule)
            {
                htmlComment = null;
            }
            else
            {
                item.PlacedOnSchedule = true;
                htmlComment = "<B>Task Item:</B> " + item.Id + "&#10;" +
                    "<B>Changed:</B> " + item.ChangedDate.ToString("yyyy-MM-dd");
            }

            if (item.IsFixed)
            {
                styleId = FixedWorkStyleId;
            }
            else if (item.IsHighConfidence)
            {
                styleId = ConfidentCellStyleId;
            }
            else
            {
                styleId = NormalCellStyleId;
            }
            WriteCellAsDataWithComment("String", item.Title, styleId, htmlComment);
        }

        private int WorkItemComparison(WorkItem x, WorkItem y)
        {
            int result;

            result = x.Milestone.CompareTo(y.Milestone);
            if (result != 0) return result;
            if (x.ImplementationOrder == -1) return 1;
            if (y.ImplementationOrder == -1) return -1;

            return x.ImplementationOrder - y.ImplementationOrder;
        }

        private void WriteHeader()
        {
            const int TesterColumnHeight = 256;
            List<string> columnTitles;
            int[] columnHeights;

            columnTitles = new List<string>();
            columnTitles.AddRange(new string[] { "Milestone", "Weekday", "Date", "T-CC" });
            columnTitles.AddRange(_testers);

            columnHeights = new int[4 + _testers.Count];
            columnHeights[0] = 47;
            columnHeights[1] = 45;
            columnHeights[2] = 32;
            columnHeights[3] = 25;
            for (int i = 4; i < columnHeights.Length; i++)
            {
                columnHeights[i] = TesterColumnHeight;
            }
            WriteHeaderRow(columnTitles.ToArray(), columnHeights);
        }

        #endregion Private methods.

        #region Private fields.

        private List<WorkItem> _workItems;
        private Dictionary<string, TesterWorkItemList> _testerLists;
        private List<string> _testers;
        private List<WorkItem> _overscheduledWorkItems;

        #endregion Private fields.
    }

    /// <summary>List of work items for a specific tester.</summary>
    public class TesterWorkItemList: List<WorkItem>
    {
        #region Constructors.

        /// <summary>Initializes a new TesterWorkItemList instance.</summary>
        public TesterWorkItemList(): base()
        {
            _fixedItems = new List<WorkItem>();
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>Tester alias.</summary>
        public string Tester
        {
            get { return this._tester; }
            set { this._tester = value; }
        }

        /// <summary>Fixed-schedule work items.</summary>
        public List<WorkItem> FixedItems
        {
            get { return this._fixedItems; }
        }

        #endregion Public properties.

        #region Public methods.

        /// <summary>Assigns confidence values to each work item.</summary>
        /// <remarks>
        /// Confidence is a boolean flag that determines whether a work
        /// day falls within 65% of the remaining floating work days.
        /// </remarks>
        public void AssignConfidenceValues()
        {
            MilestoneSchedule schedule;
            int remainingConfidenceDayCount;
            int schedulableDayCount;
            DateTime cursorDate;

            // Calculate the total number of schedulable days.
            schedule = MilestoneSchedule.Current;
            schedulableDayCount = 0;
            cursorDate = DateTime.Today;
            while (cursorDate < schedule.TestCodeCompleteDate)
            {
                if (schedule.IsScheduledWorkDay(cursorDate) &&
                    !HasFixedItemForDate(cursorDate))
                {
                    schedulableDayCount++;
                }
                cursorDate = cursorDate.AddDays(1);
            }

            // Calculate the number of days with high confidence.
            remainingConfidenceDayCount = (int)
                ((double)schedulableDayCount * 0.65);

            // Assign confidence values to tasks within confidence days.
            foreach (WorkItem item in this)
            {
                if (item.RemainingCalendarDays > 0 && !item.IsFixed)
                {
                    if (remainingConfidenceDayCount >= item.RemainingCalendarDays)
                    {
                        item.IsHighConfidence = true;
                        remainingConfidenceDayCount -= item.RemainingCalendarDays;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public WorkItem ScheduleWork(MilestoneSchedule schedule, DateTime date)
        {
            // Work items must have been scheduled beforehand, so
            // the first non-zero remaining item should be scheduled first.
            foreach(WorkItem item in this)
            {
                if (item.RemainingCalendarDays > 0)
                {
                    item.RemainingCalendarDays--;
                    return item;
                }
            }
            return null;
        }

        public bool HasFixedItemForDate(DateTime date)
        {
            MilestoneSchedule schedule;

            schedule = MilestoneSchedule.GetForDate(date);

            return ScheduleFixedItem(schedule, date) != null;
        }

        public WorkItem ScheduleFixedItem(MilestoneSchedule schedule, DateTime date)
        {
            if (schedule == null)
            {
                throw new ArgumentNullException("schedule");
            }

            // For regular fixed work items, remaining days must be the number
            // of remaining work days available for scheduling.
            // For vacation work items (always fixed), remaining days must
            // be the number of remaining non-weekend days.
            foreach(WorkItem item in _fixedItems)
            {
                int remainingDays;
                DateTime fixedDate;

                if (item.RemainingCalendarDays <= 0 || !item.IsFixed)
                {
                    continue;
                }

                remainingDays = item.RemainingCalendarDays;
                fixedDate = DateTime.Parse(item.DueDate).Date;
                while (remainingDays > 0)
                {
                    if (date == fixedDate)
                    {
                        return item;
                    }
                    // If the date is outside of this schedule, there is no work item to consider.
                    if (!schedule.DateRange.IsDateWithin(fixedDate))
                    {
                        return null;
                    }
                    if (schedule.IsScheduledWorkDay(fixedDate) ||
                        (item.IsVacation && !DateRange.IsDateWeekend(fixedDate)))
                    {
                        remainingDays--;
                    }
                    fixedDate = fixedDate.AddDays(-1);
                }
            }
            return null;
        }

        #endregion Public methods.

        #region Private fields.

        private string _tester;
        private List<WorkItem> _fixedItems;

        #endregion Private fields.
    }

    public class WorkItem
    {
        #region Public properties and fields.

        /// <summary>PS identifier for work item.</summary>
        public long Id;

        /// <summary>Alias of person with the task assigned.</summary>
        public string AssignedTo;

        /// <summary>Task title.</summary>
        public string Title;

        /// <summary>Task priority.</summary>
        public int Priority;

        /// <summary>Estimated calendar days, -1 if undefined.</summary>
        public int EstimatedCalendarDays;

        /// <summary>Remaining calendar days, -1 if undefined.</summary>
        public int RemainingCalendarDays;

        /// <summary>Actual calendar days, 0 if undefined.</summary>
        public int ActualCalendarDays;

        /// <summary>Test owner for test task.</summary>
        public string TestOwner;

        /// <summary>Checkpoint for task, empty if undefined.</summary>
        public string Checkpoint;

        /// <summary>Milestone name for the task, empty if undefined.</summary>
        public string Milestone;

        /// <summary>Test spec URL, empty if undefined.</summary>
        public string SpecUrl;

        /// <summary>Sub task type string, empty if undefined.</summary>
        public string SubTaskType1;

        /// <summary>Confidence in task, -1 if undefined.</summary>
        public int Confidence;

        /// <summary>Implementation order, -1 if undefined.</summary>
        public int ImplementationOrder;

        /// <summary>Last changed date.</summary>
        public DateTime ChangedDate;

        /// <summary>Due date for work item, null if undefined.</summary>
        public string DueDate;

        /// <summary>Whether this work item is fixed in time.</summary>
        public bool IsFixed { get { return this.DueDate.Length > 0; } }

        /// <summary>Whether this is a high-confidence work item.</summary>
        public bool IsHighConfidence;

        /// <summary>Whether this work item is a vacation item.</summary>
        public bool IsVacation { get { return this.SubTaskType1 == "Vacation Days"; } }

        /// <summary>Whether the work item has been written to the schedule before.</summary>
        public bool PlacedOnSchedule;

        #endregion Public properties and fields.
    }

    public class ActualDaysPerTester
    {
        #region Public properties and fields.

        public string Tester;
        public int ActualDaySum;

        #endregion Public properties and fields.
    }

    /// <summary>
    /// Provides a report of work based on actual days worked.
    /// </summary>
    public class ActualDaysReport: ReportBase
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new ActualDaysReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public ActualDaysReport(XmlWriter excelWriter, XmlWriter wordWriter,
            FundamentalReportOptions options): base(excelWriter, wordWriter, options)
        {
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Writes the report to the configured outputs.</summary>
        public override void WriteReport()
        {
            List<ActualDaysPerTester> report;

            base.WriteReport();
            report = this.Execute();

            // Write the report to Excel.
            WriteStartWorksheet("ActualDays");
            WriteHeaderRow(new string[] { "Tester", "Actual Day Sum" },
                new int[] { 40, 74});
            foreach(ActualDaysPerTester t in report)
            {
                WriteStartRow();
                WriteCellAsString(t.Tester);
                WriteCellAsNumber(t.ActualDaySum);
                WriteEndRow();
            }
            WriteEndWorksheet();

            // Write the report to Word.
            WriteStartSection("Actual Tester Days");
            WriteParagraph("This section provides the sum of all Actual Calendar Days.");
            WriteParagraph("Use this information to track whether all work days " +
                "have been accounted for.");
            foreach(ActualDaysPerTester t in report)
            {
                WriteEmptyParagraph();
                WriteParagraph(t.Tester + " has " + t.ActualDaySum + " actual days.");
            }
            WriteEndSection();
        }

        #endregion Public methods.

        #region Private methods.

        private List<WorkItem> GetWorkItems()
        {
            PS.DatastoreItemList items;
            string[] fieldNames;
            string queryXml;
            List<WorkItem> result;

            queryXml = "<Query Product='Windows Client Task List'>" +
                "<Group GroupOperator='And'>" +
                @"<Expression Column='TreeID' Operator='under'><Number>408</Number></Expression>" +
                @"<Expression Column='Milestone' Operator='equals'><String>" + MilestoneSchedule.Current.Name + @"</String></Expression>" +
                @"<Expression Column='Issue type' Operator='equals'><String>Test Work Item</String></Expression>" +
                "</Group></Query>";
            fieldNames = new string[] {
                "ID", "Assigned To", "Estimated Calendar Days",
                "Remaining Calendar Days", "Actual Calendar Days"
            };

            items = ProductStudioQuery.ExecuteQuery(TasksProductName, queryXml, fieldNames);
            result = new List<WorkItem>(items.DatastoreItems.Count);
            for (int i = 0; i < items.DatastoreItems.Count; i++)
            {
                PS.DatastoreItem item;
                WorkItem workItem;

                item = items.DatastoreItems[i];
                workItem = new WorkItem();
                workItem.Id = long.Parse(ProductStudioQuery.FieldValue(item, "ID"));
                workItem.AssignedTo = ProductStudioQuery.FieldValue(item, "Assigned To");
                workItem.EstimatedCalendarDays = int.Parse(ProductStudioQuery.FieldValue(item, "Estimated Calendar Days"));
                workItem.RemainingCalendarDays =
                    (ProductStudioQuery.FieldValue(item, "Remaining Calendar Days") == "") ? -1 :
                    int.Parse(ProductStudioQuery.FieldValue(item, "Remaining Calendar Days"));
                workItem.ActualCalendarDays =
                    (ProductStudioQuery.FieldValue(item, "Actual Calendar Days") == "") ? 0 :
                    int.Parse(ProductStudioQuery.FieldValue(item, "Actual Calendar Days"));
                result.Add(workItem);
            }

            return result;
        }

        private List<ActualDaysPerTester> Execute()
        {
            Dictionary<string, ActualDaysPerTester> actualDaysPerTesterDictionary;
            List<WorkItem> workItemList;

            workItemList = GetWorkItems();
            actualDaysPerTesterDictionary = new Dictionary<string,ActualDaysPerTester>();
            foreach(WorkItem workItem in workItemList)
            {
                ActualDaysPerTester days;

                if (actualDaysPerTesterDictionary.ContainsKey(workItem.TestOwner))
                {
                    days = actualDaysPerTesterDictionary[workItem.TestOwner];
                }
                else
                {
                    days = new ActualDaysPerTester();
                    days.Tester = workItem.TestOwner;
                    actualDaysPerTesterDictionary[workItem.TestOwner] = days;
                }
                days.ActualDaySum += workItem.ActualCalendarDays;
            }

            return new List<ActualDaysPerTester>(actualDaysPerTesterDictionary.Values);
        }

        #endregion Private methods.
    }
}
