using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Provides access to parts defined in Calendar control template
    /// </summary>
    public class TemplatedCalendar
    {
        private const string PartCalendarItem = "PART_CalendarItem";
        private const string PartMonthView = "PART_MonthView";
        private const string PartYearView = "PART_YearView";
        private const string PartHeaderButton = "PART_HeaderButton";
        private const string PartPreviousButton = "PART_PreviousButton";
        private const string PartNextButton = "PART_NextButton";

        private readonly Calendar calendar = null;

        public TemplatedCalendar(Calendar calendar)
        {
            this.calendar = calendar;
        }

        public static T FindPartByName<T>(FrameworkElement fe, string partName)
        {
            object part = VisualTreeUtils.FindPartByName(fe, partName);
            if(part == null)
            {
                throw new TestFailedException(string.Format("Template part '{0}' not found", partName));
            }

            if(!(part is T))
            {
                throw new TestFailedException(string.Format("Template part '{0}' is not of type {1}", partName, typeof(T)));
            }

            return (T)part;
        }

        public CalendarItem CalendarItem
        {
            get
            {
                return FindPartByName<CalendarItem>(this.calendar, PartCalendarItem);
            }
        }

        public TemplatedMonthView Month
        {
            get
            {
                return new TemplatedMonthView(FindPartByName<Grid>(this.CalendarItem, PartMonthView));
            }
        }

        public TemplatedYearView Year
        {
            get
            {
                return new TemplatedYearView(FindPartByName<Grid>(this.CalendarItem, PartYearView));
            }
        }

        public Button HeaderButton
        {
            get
            {
                return FindPartByName<Button>(this.CalendarItem, PartHeaderButton);
            }
        }

        public Button PreviousButton
        {
            get
            {
                return FindPartByName<Button>(this.CalendarItem, PartPreviousButton);
            }
        }

        public Button NextButton
        {
            get
            {
                return FindPartByName<Button>(this.CalendarItem, PartNextButton);
            }
        }

        public CalendarDayButton CalendarDayButtonByDate(DateTime date)
        {
            if (date.Year != calendar.DisplayDate.Year)
            {
                return null;
            }

            if (date.Month != calendar.DisplayDate.Month)
            {
                return null;
            }

            bool inMonth = false;

            foreach (CalendarDayButton daybtn in Month.Days)
            {
                int current = ((DateTime)daybtn.DataContext).Day;

                if (current == 1)
                    // is true enter display month days (first time we hit 1)
                    // is false when we leave display month days (hit 1 again)
                    inMonth = InvertBool(inMonth);

                if (!inMonth)
                {
                    continue;
                }
                else
                {
                    if (date.Day == current)
                        return daybtn;
                    else
                        continue;
                }
            }

            return null;
        }

        public CalendarDayButton CalendarDayButtonByDay(int day)
        {
            bool inMonth = false;

            foreach (CalendarDayButton daybtn in Month.Days)
            {
                int current = ((DateTime)daybtn.DataContext).Day;//Convert.ToInt32(daybtn.Content);

                if (current == 1)
                    inMonth = InvertBool(inMonth);

                if (!inMonth)
                {
                    continue;
                }
                else
                {
                    if (day == current)
                        return daybtn;
                    else
                        continue;
                }
            }

            return null;
        }

        public CalendarButton CalendarButtonWithSelectedDays()
        {
            foreach (CalendarButton cbtn in Year.Items)
            {
                if (cbtn.HasSelectedDays)
                    return cbtn;
            }

            return null;
        }

        /// <summary>
        /// Get the max year in view in decade view
        /// </summary>
        public int MaxYear
        {
            get
            {
                return ((DateTime)Year.Items[10].DataContext).Year;
            }
        }

        /// <summary>
        /// Get the min year in view in decade mode
        /// </summary>
        public int MinYear
        {
            get
            {
                return ((DateTime)Year.Items[1].DataContext).Year;
            }
        }

        /// <summary>
        /// Get the current year in view in Year mode
        /// </summary>
        public int CurrentYear
        {
            get
            {
                int year = 0;
                Int32.TryParse(HeaderButton.Content.ToString(), out year);
                return year;
            }
        }

        private bool InvertBool(bool value)
        {
            return !value;
        }
    }

    /// <summary>
    /// Provides access to parts defined in Month view control template
    /// </summary>
    public class TemplatedMonthView
    {
        private readonly Grid month = null;

        public TemplatedMonthView(Grid month)
        {
            this.month = month;
        }

        public List<TextBlock> WeekDays
        {
            get
            {
                ArrayList list = VisualTreeUtils.FindPartByType(month, typeof(TextBlock));
                if (list.Count > 7)
                    throw new TestFailedException("Should not have found more that 7 textblocks in MonthView");
                // need to figure out something better for perf here.
                List<TextBlock> weekdays = new List<TextBlock>();
                foreach (TextBlock item in list)
                {
                    weekdays.Add(item);
                }
                return weekdays;
            }
        }

        public List<CalendarDayButton> Days
        {
            get
            {
                ArrayList list = VisualTreeUtils.FindPartByType(month, typeof(CalendarDayButton));
                if (list.Count > 42)
                    throw new TestFailedException("Should not have found more that 42 CalendarDayButtons in MonthView");
                // need to figure out something better for perf here.
                List<CalendarDayButton> days = new List<CalendarDayButton>();
                foreach (CalendarDayButton item in list)
                {
                    days.Add(item);
                }
                return days;
            }
        }

        public Grid UI
        {
            get { return month; }
        }
    }

    /// <summary>
    /// Provides access to parts defined in Year/Decade view control template
    /// </summary>
    public class TemplatedYearView
    {
        private readonly Grid year = null;

        public TemplatedYearView(Grid year)
        {
            this.year = year;
        }

        public List<CalendarButton> Items
        {
            get
            {
                ArrayList list = VisualTreeUtils.FindPartByType(year, typeof(CalendarButton));
                if (list.Count > 12)
                    throw new TestFailedException("Should not have found more that 12 CalendarDayButtons in MonthView");
                List<CalendarButton> years = new List<CalendarButton>();
                foreach (CalendarButton item in list)
                {
                    years.Add(item);
                }
                return years;
            }
        }

        public Grid UI
        {
            get { return year; }
        }

    }
}
