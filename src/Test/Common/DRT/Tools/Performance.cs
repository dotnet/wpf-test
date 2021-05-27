// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
* File: Preformance.cs
*
* Description:
* Implements Performance Tracking
*
* 
\***************************************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using System.IO;

using System.Reflection;


namespace MS.Utility
{
    #region PerformanceUtilities

    /// <summary>
    /// </summary>
    /// <ExternalAPI/>     
    public class PerformanceUtilities
    {
        static private long _frequency       = 0;
        static private long _frequency1000th = 0;

        /// <summary>
        ///     Initializes static data used by the Utilities class.
        /// </summary>
        
        static PerformanceUtilities()
        {
            QueryPerformanceFrequency(out _frequency);
            _frequency1000th = _frequency/1000;
            //Debug.Assert((_frequency != 0),"PerformanceCounter is not supported on this machine");
        }

        /// <summary>
        ///     An optimized wrapper for QueryPerformanceCounter().
        /// </summary>
        /// <param name="lpPerformanceCount">
        ///     The value of <paramref name="lpPerformanceCount" /> is set to the current high-resolution counter value.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.  If the function fails, the return value is zero.
        /// </returns>
        /// <ExternalAPI/> 
        [System.Security.SuppressUnmanagedCodeSecurity, System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        /// <summary>
        ///     An optimized wrapper for QueryPerformanceFrequency().
        /// </summary>
        /// <param name="lpFrequency">
        ///     The value of <paramref name="lpFrequency" /> is set to the high-resolution counter frequency.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.  If the function fails, the return value is zero.
        /// </returns>
        /// <ExternalAPI/>      
        [System.Security.SuppressUnmanagedCodeSecurity, System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        /// <summary>
        ///     Calculates the elapsed time in QueryPerformanceCounter ticks between two given QueryPerformanceCounter values.
        /// </summary>
        /// <param name="startCount">
        ///     The starting counter value.
        /// </param>
        /// <param name="endCount">
        ///     The ending counter value.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time between the two counter values in QueryPerformanceCounter ticks.  
        ///     Wrapping is taken into account.
        /// </returns>
        /// <ExternalAPI/> 
        public static long Cost(long startCount, long endCount)
        {
            if (endCount >= startCount)
            {
                return (endCount - startCount);
            }
            else
            {
                return endCount - startCount + long.MaxValue;
            }
        }

        /// <summary>
        ///     Calculates the elapsed time in seconds between two given QueryPerformanceCounter values.
        /// </summary>
        /// <param name="startCount">
        ///     The starting counter value.
        /// </param>
        /// <param name="endCount">
        ///     The ending counter value.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time between the two counter values in seconds.  
        ///     Wrapping is taken into account.
        /// </returns>
        /// <ExternalAPI/>      
        public static double CostInSeconds(long startCount, long endCount)
        {
            return ((double) Cost(startCount, endCount))/((double) _frequency);
        }

        /// <summary>
        ///     Calculates the elapsed time in seconds given an elapsed time expressed in QueryPerformanceCounter ticks.
        /// </summary>
        /// <param name="cost">
        ///     The elapsed time in QueryPerformanceCounter ticks.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time in seconds.  
        /// </returns>
        /// <ExternalAPI/> 
        public static double CostInSeconds(long cost)
        {
            return ((double) cost)/((double) _frequency);
        }

        /// <summary>
        ///     Calculates the elapsed time in milliseconds between two given QueryPerformanceCounter values.
        /// </summary>
        /// <param name="startCount">
        ///     The starting counter value.
        /// </param>
        /// <param name="endCount">
        ///     The ending counter value.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time between the two counter values in milliseconds.  
        ///     Wrapping is taken into account.
        /// </returns>
        /// <ExternalAPI/> 
        public static double CostInMilliseconds(long startCount, long endCount)
        {
            return ((double) Cost(startCount, endCount))/((double) _frequency1000th);
        }

        /// <summary>
        ///     Calculates the elapsed time in milliseconds given an elapsed time expressed in QueryPerformanceCounter ticks.
        /// </summary>
        /// <param name="cost">
        ///     The elapsed time in QueryPerformanceCounter ticks.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time in milliseconds.  
        /// </returns>
        /// <ExternalAPI/> 
        public static double CostInMilliseconds(long cost)
        {
            return ((double) cost)/((double) _frequency1000th);
        }

        /// <summary>
        ///     Returns the total size of the managed (GC) heap.
        /// </summary>
        /// <remarks>
        ///     GetManagedHeapSize() utilizes System.GC.GetTotalMemory() to get the managed heap size.
        ///     Before the size is calculated, it performs a full garbage collection and waits for all 
        ///     pending finalizers.
        /// </remarks>
        /// <returns>
        ///     The function returns the total size of the managed (GC) heap expressed in bytes.
        /// </returns>
        /// <ExternalAPI/> 
        public static double GetManagedHeapSize()
        {
            return (double)GC.GetTotalMemory(true);
        }
    }

    #endregion PerformanceUtilities

    #region PerformanceMarker

    /// <summary>
    /// </summary>
    /// <ExternalAPI/>     
    public class PerformanceMarker
    {
        //----------------------------------------------------------------------------------------
        // HELPER CLASSES/STRUCTS
        //----------------------------------------------------------------------------------------

        internal struct Mark
        {
            internal double value;
            internal int groupIndex;
            internal bool isTime;
            internal bool isComparator;
            internal string name;
            
            internal Mark(string newName, int newGroupIndex, double newValue, bool newIsTime, bool newIsComparator)
            {
                this.name = newName;
                this.groupIndex = newGroupIndex;
                this.value = newValue;
                this.isTime = newIsTime;
                this.isComparator = newIsComparator;
            }
        }

        internal class MarkCollection 
        {
            private const int INITIAL_MARK_COLLECTION_SIZE = 8;
            private const int MARK_COLLECTION_GROW_INCREMENT = 8;

            private Mark[] _marks;
            internal int Count;

            internal MarkCollection() 
            {
                _marks = new Mark[INITIAL_MARK_COLLECTION_SIZE];
                Count = 0;
            }

            internal int Add(Mark newMark)
            {
                if (Count >= _marks.Length)
                {
                    // Grow array by MARK_COLLECTION_GROW_INCREMENT if it's not big enough
                    Mark[] tempMarks = new Mark[_marks.Length + MARK_COLLECTION_GROW_INCREMENT];
                    int i;

                    for (i = 0; i < _marks.Length; i++)
                    {
                        tempMarks[i] = _marks[i];
                    }

                    _marks = tempMarks;
                }
                _marks[Count] = newMark;
                return Count++;
            }
            
            internal Mark this[int nIndex]
            {
                get
                {
                    if (nIndex < 0)
                    {
                        // assert?
                        return _marks[0];
                    }
                    else if (nIndex >= Count)
                    {
                        // assert?
                        return _marks[Count - 1];
                    }
                    else
                    {
                        return _marks[nIndex];
                    }
                }

                set
                {
                    if (nIndex > -1 && nIndex < Count)
                    {
                        _marks[nIndex] = value;
                    }
                    else if (nIndex == Count)
                    {
                        Add(value);
                    }
                    else
                    {
                        // assert?
                    }
                }
            }
        }

        internal struct Iteration
        {
            internal long iterations;
            internal long min;
            internal long max;
            internal long total;
            internal string name;
            internal string description;
            private long previousTime;      
            
            internal void Reset()
            {
                this.iterations = 0;
                this.min = long.MaxValue;
                this.max = long.MinValue;
                this.total = 0;
                this.name = null;
                this.description = null;
            }
            
            internal void Start(string newName, string newDescription)
            {
                long currentTime;

                this.name = newName;
                this.description = newDescription;

                PerformanceUtilities.QueryPerformanceCounter(out currentTime);
                this.previousTime = currentTime;
            }

            internal void Accumulate()
            {
                long interval;
                long currentTime;

                PerformanceUtilities.QueryPerformanceCounter(out currentTime);

                interval = PerformanceUtilities.Cost(previousTime, currentTime);

                if (interval < min)
                {
                    min = interval;
                }

                if (interval > max)
                {
                    max = interval;
                }

                total += interval;
                iterations++;
                previousTime = currentTime;
            }
        }

        internal struct Group
        {
            internal long startTime;
            internal string name;
            internal string description;
            internal long nameHash;
            
            internal Group(string newName, string newDescription, long newStartTime)
            {
                this.name = newName;
                this.description = newDescription;
                this.startTime = newStartTime;
                this.nameHash = (newName == null) ? -1 : name.GetHashCode();
            }
        }
        
        internal class GroupCollection
        {
            private const int INITIAL_GROUP_COLLECTION_SIZE = 4;
            private const int GROUP_COLLECTION_GROW_INCREMENT = 2;

            private Group[] _groups;
            internal int Count;

            internal GroupCollection() 
            {
                _groups = new Group[INITIAL_GROUP_COLLECTION_SIZE];
                Count = 0;
            }
            
            internal int IndexOf(string groupNameToFind)
            {
                long nameToFindHash = groupNameToFind.GetHashCode();
                int i;
                
                for (i = 0; i < Count; i++)
                {
                    if (_groups[i].nameHash == nameToFindHash && _groups[i].name == groupNameToFind)
                    {
                        return i;
                    }
                }
                
                return -1;
            }
            
            internal int Add(Group newGroup)
            {
                // Add this new group
                if (Count >= _groups.Length)
                {
                    // Grow array by GROUP_COLLECTION_GROW_INCREMENT if it's not big enough
                    Group[] tempGroups = new Group[_groups.Length + GROUP_COLLECTION_GROW_INCREMENT];
                    int i;

                    for (i = 0; i < _groups.Length; i++)
                    {
                        tempGroups[i] = _groups[i];
                    }

                    _groups = tempGroups;
                }

                _groups[Count] = newGroup;
                return Count++;
            }
            
            internal Group this[int nIndex]
            {
                get
                {
                    if (nIndex < 0)
                    {
                        // assert?
                        return _groups[0];
                    }
                    else if (nIndex >= Count)
                    {
                        // assert?
                        return _groups[Count - 1];
                    }
                    else
                    {
                        return _groups[nIndex];
                    }
                }

                set
                {
                    if (nIndex > -1 && nIndex < Count)
                    {
                        _groups[nIndex] = value;
                    }
                    else if (nIndex == Count)
                    {
                        Add(value);
                    }
                    else
                    {
                        // assert?
                    }
                }
            }
        }
        
        private MarkCollection Marks;
        private GroupCollection Groups;
        private Iteration _currentIteration;
        private long _overhead;

        /// <summary>
        /// 
        /// </summary>
        /// <ExternalAPI/> 
        public PerformanceMarker()
        {
            long startTime;
            long endTime;
            long currentTime;

            Marks = new MarkCollection();
            Groups = new GroupCollection();
            _currentIteration = new Iteration();
            _currentIteration.Reset();
            
            PerformanceUtilities.QueryPerformanceCounter(out startTime);
            PerformanceUtilities.QueryPerformanceCounter(out endTime);
            PerformanceUtilities.QueryPerformanceCounter(out endTime);
            
            _overhead = PerformanceUtilities.Cost(startTime, endTime);
            
            PerformanceUtilities.QueryPerformanceCounter(out currentTime);

            Groups[0] = new Group(null, "Default group", currentTime);          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public bool StartGroup(string groupName, string description)
        {
            long currentTime;
            
            if (groupName == null || Groups.IndexOf(groupName) != -1)
            {
                // Only the default group may have a null name and we
                // can't duplicate group names
                return false;
            }
            
            // Add a new group
            PerformanceUtilities.QueryPerformanceCounter(out currentTime);
            Groups.Add(new Group(groupName, description, currentTime));

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="groupName"></param>
        /// <param name="isComparator"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public bool MarkTime(string name, string groupName, bool isComparator)
        {
            int groupIndex = (groupName == null) ? 0 : Groups.IndexOf(groupName);
            long currentTime;
            
            if (groupIndex < 0)
            {
                // Couldn't find the specified group
                return false;
            }
            else if (name == null)
            {
                return false;
            }
            
            PerformanceUtilities.QueryPerformanceCounter(out currentTime);
            
            Marks.Add(new Mark(name, groupIndex, (double)currentTime, true, isComparator));
            
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="groupName"></param>
        /// <param name="value"></param>
        /// <param name="isComparator"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public bool MarkValue(string name, string groupName, double value, bool isComparator)
        {
            int groupIndex = (groupName == null) ? 0 : Groups.IndexOf(groupName);

            if (groupIndex < 0)
            {
                // Couldn't find the specified group
                return false;
            }
            else if (name == null)
            {
                return false;
            }

            Marks.Add(new Mark(name, groupIndex, value, false, isComparator));
            
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public bool StartIteration(string name, string description)
        {
            if (_currentIteration.iterations != 0 || Groups.IndexOf(name) != -1)
            {
                // Can't start a new iteration without ending the current one and
                // we can't have iterations that overlap names with the groups
                return false;
            }

            _currentIteration.Start(name, description);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <ExternalAPI/> 
        public void MarkIteration()
        {
            _currentIteration.Accumulate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <ExternalAPI/> 
        public void EndIteration()
        {
            int groupIndex = 0;
            
            if (_currentIteration.iterations != 0)
            {
                // Calculate average, add group and add marks
                groupIndex = Groups.Add(new Group(_currentIteration.name, _currentIteration.description, -1));
                
                Marks.Add(new Mark("Minimum Time", groupIndex, PerformanceUtilities.CostInMilliseconds(_currentIteration.min), false, false));
                Marks.Add(new Mark("Maximum Time", groupIndex, PerformanceUtilities.CostInMilliseconds(_currentIteration.max), false, false));
                Marks.Add(new Mark("Number of Iterations", groupIndex, _currentIteration.iterations, false, false));
                Marks.Add(new Mark("Total Time", groupIndex, PerformanceUtilities.CostInMilliseconds(_currentIteration.total), false, false));
                Marks.Add(new Mark("Average Value", groupIndex, PerformanceUtilities.CostInMilliseconds(_currentIteration.total / _currentIteration.iterations), false, true));
            }
            _currentIteration.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <ExternalAPI/> 
        public void PrintXml()
        {
            System.Text.StringBuilder outputString = new System.Text.StringBuilder();
            int i, j;
            
            outputString.Append("<RESULT>\n");
            
            for (i = 0; i < Groups.Count; i++)
            {
                if (i != 0)
                {
                    outputString.Append("<SUBRESULT NAME=\"");
                    outputString.Append(Groups[i].name);
                    outputString.Append("\" DESCRIPTION=\"");
                    outputString.Append((Groups[i].description == null) ? "" : Groups[i].description);
                    outputString.Append("\" >\n");
                }
                
                for (j = 0; j < Marks.Count; j++)
                {
                    if (Marks[j].groupIndex == i)
                    {
                        outputString.Append("<DATUM NAME=\"");
                        outputString.Append(Marks[j].name);
                        outputString.Append("\" VALUE=\"");
                        outputString.Append((Marks[j].isTime == true) ? (PerformanceUtilities.CostInMilliseconds(Groups[i].startTime, (long)Marks[j].value)) : Marks[j].value);
                        outputString.Append("\" ");
                        outputString.Append((Marks[j].isComparator == false) ? "/>\n" : "COMPARATOR=\"yes\" />\n");
                    }       
                }

                if (i != 0)
                {
                    outputString.Append("</SUBRESULT>\n");
                }
            }
            
            outputString.Append("</RESULT>\n");
            
            Console.WriteLine(outputString.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <ExternalAPI/> 
        public void PrintText()
        {
            System.Text.StringBuilder outputString = new System.Text.StringBuilder();
            int i, j;
                
            for (i = 0; i < Groups.Count; i++)
            {
                if (i == 0)
                {
                    outputString.Append("Performance Marker Log\n");
                    outputString.Append("------------------------------------------------------------\n");
                }
                else
                {
                    outputString.Append(Groups[i].name);
                    outputString.Append(" (");
                    outputString.Append((Groups[i].description == null) ? "" : Groups[i].description);
                    outputString.Append(")\n");
                    outputString.Append("------------------------------------------------------------\n");
                }
                
                for (j = 0; j < Marks.Count; j++)
                {
                    if (Marks[j].groupIndex == i)
                    {
                        outputString.Append((Marks[j].isComparator == false) ? "  " : "* ");
                        outputString.Append(Marks[j].name);
                        outputString.Append(":  ");
                        outputString.Append((Marks[j].isTime == true) ? (PerformanceUtilities.CostInMilliseconds(Groups[i].startTime, (long)Marks[j].value)) : Marks[j].value);
                        outputString.Append("\n");
                    }
                }

                outputString.Append("\n");
            }
            
            Console.WriteLine(outputString.ToString());
        }
    }
    
    #endregion PerformanceMarker
    

    /// <summary>
    /// A enumeration of the types of performance marks
    /// </summary>
    /// <ExternalAPI Inherit="true"/>
    public enum MarkType
    {
        /// <summary> Start  </summary>
        Start,
        /// <summary> Accrued </summary>
        Accrued,
        /// <summary> Iterative </summary>
        Iterative,
        /// <summary> End </summary>
        End,
    };

    /// <summary>
    /// An enumeration of PerformanceCounters avaliable with PerformanceTracker
    /// </summary>
    /// <ExternalAPI Inherit="true"/>
    public enum CounterType
    {
        /// <summary> Default Counter Type  </summary>
        Timer,
        /// <summary> PercentTimeLoading </summary>
        PercentTimeLoading,
        /// <summary> NumberOfAssemblies </summary>
        NumberOfAssemblies,
        /// <summary> TotalNumberOfAssemblies </summary>
        TotalNumberOfAssemblies,
        /// <summary> FinalizationSurvivors </summary>
        FinalizationSurvivors,
        /// <summary> WorkingSet </summary>
        WorkingSet,
        /// <summary> PeakWorkingSet </summary>
        PeakWorkingSet
    };

    /// <summary>
    /// A performance marks have description and a type
    /// </summary>
    internal class PerformanceMark
    {
        protected long     _value       = 0;
        protected string   _description = null;

        protected CounterType _myCounterType  = CounterType.Timer;
        protected MarkType    _myType         = MarkType.Start;
        protected Type        _myValueType    = null;

        internal PerformanceMark(CounterType myCounterType, MarkType myType, Type myValueType)
        {
            _myCounterType = myCounterType;
            _myType        = myType;
            _myValueType   = myValueType;
        }

        internal bool IsTimer
        {
            get {return _myCounterType == CounterType.Timer;}
        }

        /// <summary> Mark Type</summary>
        internal CounterType PerformanceCounterType 
        {
            get {return _myCounterType;}
        }

        /// <summary> Mark Type</summary>
        internal MarkType Type 
        {
            get {return _myType;}
        }

        internal Type ValueType
        {
            get {return _myValueType;}
            set {_myValueType = value;}
        }

        /// <summary> Mark Count</summary>
        internal long RawValue 
        {
            get {return _value;}
            set {_value = value;}
        }

        /// <summary> Mark Description</summary>
        internal string Description 
        {
            get {return _description;}
            set {_description = value;}
        }
    }

    /// <summary>
    /// Base Iterative Mark
    /// </summary>
    internal abstract class IterativeMark
    {
        protected PerformanceMark _mark = null;

        protected long _iterativeFirstRawValue    = 0;
        protected long _iterativeSecondRawValue   = 0;
        protected long _iterativePreviousRawValue = 0;

        protected long _iterativeCount = 0;

        protected virtual void Accumulate(PerformanceMark mark)
        {
            _iterativeCount++;

            if (_iterativeCount == 2)
            {
                _iterativeFirstRawValue = _iterativePreviousRawValue;
            }
            else if (_iterativeCount == 3)
            {
                _iterativeSecondRawValue = _iterativePreviousRawValue;
            }
        }
        
        internal Type MarkValueType
        {
            get {return _mark.ValueType;}
        }

        /// <summary> A genaric performance mark to hold the working value </summary>
        internal PerformanceMark Mark
        {
            get 
            {
                return _mark;
            }

            set
            {
                Accumulate(value);

                if (_mark != null)
                {
                    _iterativePreviousRawValue = _mark.RawValue;
                }

                _mark = value;
            }
        }

        internal abstract long FirstValue();
        internal abstract long LastValue();
        internal abstract long MinimumValue();
        internal abstract long MaximumValue();
        internal abstract long TotalValue();
        internal abstract long AverageValue();

        /// <summary> Sequence first value in the sequence </summary>
        protected long IterativeFirst
        {
            get 
            {
                if (_iterativeCount == 2)
                {
                    return _mark.RawValue;
                }
                else
                {
                    return _iterativeFirstRawValue;
                }
            }
        }

        /// <summary> Sequence second value in the sequence </summary>
        protected long IterativeSecond
        {
            get 
            {
                if (_iterativeCount == 3)
                {
                    return _iterativePreviousRawValue;
                }
                else if (_iterativeCount == 2)
                {
                    return _mark.RawValue;
                }
                else
                {
                    return _iterativeFirstRawValue;
                }
            }
        }

        /// <summary> Sequence average of the interative marks</summary>
        protected internal long IterativeLast
        {
            get 
            {
                return _mark.RawValue;
            }
        }

        /// <summary> Sequence average of the interative marks</summary>
        internal long IterativeCount
        {
            get 
            {
                return _iterativeCount;
            }
        }
    }

    /// <summary>
    /// Subclass of iterative mark that is concerned about the difference of pairs of values
    /// </summary>
    internal abstract class IterativeMarkDifferences : IterativeMark
    {
        protected long _iterativeRawMinimumDifference = 0;
        protected long _iterativeRawMaximumDifference = 0;
        protected long _iterativeRawTotalDifference = 0;

        protected abstract long Difference(long rawValue1,long rawValue2);
        protected abstract long UpdateMinimumDifference(long rawValue);
        protected abstract long UpdateMaximumDifference(long rawValue);
        protected abstract long UpdateTotalDifference(long rawValue);

        /// <summary> The value of the first mark in the sequence</summary>
        internal override long FirstValue()
        {
            if (_iterativeCount > 2)
            {
                return Difference(IterativeFirst,IterativeSecond);
            }
            else
            {
                return 0;
            }
        }

        /// <summary> The value of the first mark in the sequence</summary>
        internal override long LastValue()
        {
            if (_iterativeCount > 2)
            {
                return Difference(_iterativePreviousRawValue,_mark.RawValue);
            }
            else
            {
                return 0;
            }
        }

        protected override void Accumulate(PerformanceMark mark)
        {
            base.Accumulate(mark);

            // The Count of the new mark has not been set
            // so we process the previous one now.
            if (_iterativeCount == 3)
            {
                long difference = Difference(_iterativePreviousRawValue,_mark.RawValue);

                _iterativeRawMinimumDifference = difference;
                _iterativeRawMaximumDifference = difference;
                _iterativeRawTotalDifference   = difference;
            }
            else if (_iterativeCount > 3)
            {
                long difference = Difference(_iterativePreviousRawValue,_mark.RawValue);

                _iterativeRawMinimumDifference = UpdateMinimumDifference(difference);
                _iterativeRawMaximumDifference = UpdateMaximumDifference(difference);
                _iterativeRawTotalDifference   = UpdateTotalDifference(difference);
            }
        }

        /// <summary> The minimum difference in the sequence </summary>
        protected long IterativeRawMinimumDifference
        {
            get 
            {
                if (_iterativeCount >= 2)
                {
                    return UpdateMinimumDifference(Difference(_iterativePreviousRawValue,_mark.RawValue));
                }

                return _iterativeRawMinimumDifference;
            }
        }

        /// <summary> The maximum difference in the sequence </summary>
        protected long IterativeRawMaximumDifference
        {
            get 
            {
                if (_iterativeCount >= 2)
                {
                    return UpdateMaximumDifference(Difference(_iterativePreviousRawValue,_mark.RawValue));
                }

                return _iterativeRawMaximumDifference;
            }
        }

        /// <summary> Sequence first value in the sequence </summary>
        protected long IterativeRawTotalDifference
        {
            get 
            {
                if (_iterativeCount >= 2)
                {
                    return UpdateTotalDifference(Difference(_iterativePreviousRawValue,_mark.RawValue));
                }

                return _iterativeRawTotalDifference;
            }
        }
    }

    /// <summary>
    /// Sub class for iterative marks that are concerned about absolute values
    /// </summary>
    internal abstract class IterativeMarkAbsolute : IterativeMark
    {
        protected long _iterativeRawMinimum = 0;
        protected long _iterativeRawMaximum = 0;
        protected long _iterativeRawTotal   = 0;

        protected abstract long UpdateMinimum(long rawValue);
        protected abstract long UpdateMaximum(long rawValue);
        protected abstract long UpdateTotal(long rawValue);

        protected override void Accumulate(PerformanceMark mark)
        {
            // The Count of the new mark has not been set
            // so we process the previous one now.
            if (_iterativeCount > 2)
            {
                _iterativeRawMinimum = UpdateMinimum(_iterativePreviousRawValue);
                _iterativeRawMaximum = UpdateMaximum(_iterativePreviousRawValue);
                _iterativeRawTotal   = UpdateTotal(_iterativePreviousRawValue);
            }
            else if (_iterativeCount == 2)
            {
                _iterativeFirstRawValue = _iterativePreviousRawValue;
                _iterativeRawMinimum    = _iterativePreviousRawValue;
                _iterativeRawMaximum    = _iterativePreviousRawValue;
                _iterativeRawTotal      = _iterativePreviousRawValue;
            }
        }

        /// <summary> The value of the first mark in the sequence</summary>
        internal override long FirstValue()
        {
            return IterativeFirst;
        }

        /// <summary> The value of the first mark in the sequence</summary>
        internal override long LastValue()
        {
            return IterativeLast;
        }

        /// <summary> The minimum value in the sequence </summary>
        protected long IterativeRawMinimum
        {
            get
            {
                // Updating the minimum multiple times is okay
                UpdateMinimum(_mark.RawValue);

                return _iterativeRawMinimum;
            }
        }

        /// <summary> The maximum value in the sequence </summary>
        protected long IterativeRawMaximum
        {
            get 
            {
                return UpdateMaximum(_mark.RawValue);
            }
        }

        /// <summary> The total value of the sequence </summary>
        protected long IterativeRawTotal
        {
            get 
            {
                // Updating the total multiple times is okay

                return UpdateTotal(_mark.RawValue);
            }
        }
    }

    /// <summary>
    /// Iterative Timer Mark which is concerned about the differences between values
    /// </summary>
    internal class IterativeMarkTimer : IterativeMarkDifferences
    {
        public IterativeMarkTimer()
        {
            _mark = new PerformanceMark(CounterType.Timer,MarkType.Iterative,typeof(long));
        }

        protected override long Difference(long rawValue1, long rawValue2) 
        {
            return PerformanceUtilities.Cost(rawValue1, rawValue2);
        }

        protected override long UpdateMinimumDifference(long rawValue) 
        {
            if (rawValue < _iterativeRawMinimumDifference)
            {
                return rawValue;
            }
            else
            {
                return (long) _iterativeRawMinimumDifference;
            }
        }

        protected override long UpdateMaximumDifference(long rawValue) 
        {
            if (rawValue > _iterativeRawMaximumDifference)
            {
                return rawValue;
            }
            else
            {
                return _iterativeRawMaximumDifference;
            }
        }

        protected override long UpdateTotalDifference(long rawValue)   
        {
            return (long) (_iterativeRawTotalDifference + rawValue);
        }

        /// <summary> The value of the first mark in the sequence</summary>
        internal override long MinimumValue ()
        {
            return _iterativeRawMinimumDifference;
        }

        /// <summary> The value of the first mark in the sequence</summary>
        internal override long MaximumValue()
        {
            return _iterativeRawMaximumDifference;
        }

        /// <summary> The total of all values in the sequence</summary>
        internal override long TotalValue()
        {
            return _iterativeRawTotalDifference;
        }

        /// <summary> Sequence average of the interative marks</summary>
        internal override long AverageValue()
        {
            if (IterativeCount <= 1)
            {
                return 0;
            }

            // Because this is a set of differences we need to subtrack one
            return TotalValue()/(IterativeCount-1);
        }
    }

    // If the user of this object should not be 
    // 
    // 1. Tracking marks from different threads.
    // 2. Interpret to marks that does not contain
    //    both the start of a fork and the end of fork.
    //
    // We can assert against 1.
    // 

    /// <summary>
    /// A PerformanceMarkSequence consists of
    /// a start PerformanceMark
    /// 0 or more Accrued PerformanceMarks
    /// an end PerformanceMark
    /// </summary>
    internal class PerformanceMarkSequence
    {
        private string _key = null;

        private PerformanceMark _startMark         = null;
        private PerformanceMark _startCounterMark  = null;
        private PerformanceMark _endMark          = null;
        private PerformanceMark _endCounterMark    = null;

        private ArrayList       _accruedMarks  = null;

        private IterativeMarkTimer _iterativeMarkTimer = null;

        private Thread _thread = null;

        /// <summary> Sequence key</summary>
        internal string Key 
        {
            get {return _key;}
            set {_key = value;}
        }

        /// <summary> Sequence start mark</summary>
        internal PerformanceMark StartMark 
        {
            get 
            {
                return _startMark;
            }
            set
            {
                _thread = Thread.CurrentThread;
                _startMark = value;
            }
        }

        /// <summary> Sequence start mark</summary>
        internal PerformanceMark StartCounterMark 
        {
            get 
            {
                return _startCounterMark;
            }
            set
            {
                if (_thread != Thread.CurrentThread)
                {
                    Debug.Assert(false,"This performance mark is being added to a sequence that started in another thread."); 
                    Debug.Assert(false,"But Start Counter is always preceeeded by a Start Timer"); 
                }

                _startCounterMark = value;
            }
        }

        /// <summary> Sequence end mark</summary>
        internal PerformanceMark EndMark 
        {
            get 
            {
                return _endMark;
            }
            set 
            {
                if (_thread != Thread.CurrentThread)
                {
                    Debug.Assert(false,"This performance mark is being added to a sequence that started in another thread."); 
                }

                _endMark = value;
            }
        }

        /// <summary> Sequence end mark</summary>
        internal PerformanceMark EndCounterMark 
        {
            get 
            {
                return _endCounterMark;
            }
            set 
            {
                if (_thread != Thread.CurrentThread)
                {
                    Debug.Assert(false,"This performance mark is being added to a sequence that started in another thread."); 
                }

                _endCounterMark = value;
            }
        }

        /// <summary> Sequence accrued marks</summary>
        internal int AccruedCount 
        {
            get {return _accruedMarks.Count;}
        }

        /// <summary> Sequence average of the interative marks</summary>
        internal long IterativeFirst
        {
            get 
            {
                return ((IterativeMarkTimer) _iterativeMarkTimer).FirstValue(); 
            }
        }

        /// <summary> Sequence average of the interative marks</summary>
        internal long IterativeLast
        {
            get 
            {
                return ((IterativeMarkTimer) _iterativeMarkTimer).LastValue();
            }
        }

        /// <summary> Sequence average of the interative marks</summary>
        internal long IterativeAverage
        {
            get 
            {
                return ((IterativeMarkTimer) _iterativeMarkTimer).AverageValue();
            }
        }

        /// <summary> Sequence maximum of the interative marks</summary>
        internal long IterativeMaximum
        {
            get 
            {
                return _iterativeMarkTimer.MaximumValue();            
            }
        }

        /// <summary> Sequence average of the interative marks</summary>
        internal long IterativeMinimum
        {
            get 
            {
                return _iterativeMarkTimer.MinimumValue();
            }
        }

        /// <summary> Sequence average of the interative marks</summary>
        internal long IterativeCount
        {
            get {return _iterativeMarkTimer.IterativeCount;}
        }

        /// <summary> Sequence average of the interative marks</summary>
        internal long IterativeTotal
        {
            get 
            {
                if (_iterativeMarkTimer.IterativeCount <= 1)
                {
                    return 0;
                }
                else
                {
                    // We have not yet taken into account the latest mark yet.
                    return _iterativeMarkTimer.TotalValue();
                }
            }
        }

        /// <summary>
        /// A PerformanceMarkSequence consists of
        /// a start PerformanceMark
        /// 0 or more Accrued PerformanceMarks
        /// an end PerformanceMark
        /// </summary>
        internal PerformanceMarkSequence()
        {
            Init();
        }

        /// <summary>
        /// Initializes the sequence
        /// </summary>
        private void Init()
        {
            _thread = Thread.CurrentThread;
            _accruedMarks = new ArrayList();
        }

        /// <summary>
        /// Initializes sequence and reinitializes member variables that are initialize by default. 
        /// </summary>
        internal void Reset()
        {
            _key        = null;
            _startMark  = null;
            _endMark    = null;
            _iterativeMarkTimer = null;

            Init();
        }

        /// <summary>
        /// Add a preformance mark to the accrued list
        /// </summary>
        /// <param name="accruedMark"></param>
        internal void AddAccrued(PerformanceMark accruedMark)
        {
            if (_thread != Thread.CurrentThread)
            {
                Debug.Assert(false,"This performance is being added to a sequence that started in another thread."); 
            }

            _accruedMarks.Add(accruedMark);
        }

        /// <summary>
        /// Add a preformance mark to the accrued list
        /// </summary>
        /// <param name="iterativeMark"></param>
        internal void AccumulateIterativeTimer(PerformanceMark iterativeMark)
        {
            if (_thread != Thread.CurrentThread)
            {
                Debug.Assert(false,"This performance is being added to a sequence that started in another thread."); 
            }

            if (_iterativeMarkTimer == null)
            {
                _iterativeMarkTimer = new IterativeMarkTimer();
            }

            _iterativeMarkTimer.Mark = iterativeMark; 

        }

        #region Elapsed

        /// <summary>
        /// Calculates the elpased time, from start to end
        /// </summary>
        /// <returns></returns>
        internal long Elapsed()
        {
            long fromTime;
            long toTime;

            if (StartMark == null)
            {
                Debug.Assert(false,Key + ": THIS SEQUENCE HAS NO START MARK"); 
                return -1;
            }

            if (EndMark == null)
            {
                Debug.Assert(false,Key + ": THIS SEQUENCE HAS NO END MARK");
                return -1;
            }

            fromTime = StartMark.RawValue;

            toTime = EndMark.RawValue;
  
            long elapsed = PerformanceUtilities.Cost(fromTime,toTime);

            return elapsed;
        }

        /// <summary>
        /// Calculates the elpased time, between two marks in a sequence
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        internal long Elapsed(int from, int to)
        {
            long fromTime;
            long toTime;

            if (StartMark != null)
            {
                Debug.Assert(false,Key + ": THIS SEQUENCE HAS NO START MARK"); 
                return -1;
            }

            long numPoints = 1 + _accruedMarks.Count;

            if (EndMark != null)
            {
                numPoints += 1;
            }

            if ((to >= from) || 
                ((to-from) > numPoints) || 
                (to < 1) ||
                (from < 0))
            {
                Debug.Assert(false,Key + ": Bad Arguments"); 
                return -1;
            }

            if (from == 0)
            {
                fromTime = StartMark.RawValue;
            }
            else
            {
                fromTime= ((PerformanceMark) _accruedMarks[from-1]).RawValue;
            }

            if (to == numPoints)
            {
                toTime = EndMark.RawValue;
            }
            else
            {
                toTime = ((PerformanceMark) _accruedMarks[to-1]).RawValue;
            }
 
            long elapsed = PerformanceUtilities.Cost(fromTime,toTime);

            return elapsed;
        }

        #endregion Elapsed

        #region Reporting

        /// <summary>
        /// Write a report on an mark sequence to a file
        /// </summary>
        /// <param name="log"></param>
        internal void Report(StreamWriter log)
        {
            long   relativeValue  = 0;

            if (StartMark != null)
            {
                relativeValue = PerformanceUtilities.Cost(PerformanceTracker.StartOfPerformanceTracking,StartMark.RawValue);
                log.WriteLine("   " + Key + " Sequence started at " + PerformanceUtilities.CostInSeconds(relativeValue).ToString()); 
                log.WriteLine("       s " + StartMark.Description);

                if (StartCounterMark != null)
                {
                    log.WriteLine("       s ..... " + 
                                  PerformanceTracker.GetCounterName(StartCounterMark.PerformanceCounterType) + 
                                  ": " +
                                  (Convert.ChangeType(StartCounterMark.RawValue,StartCounterMark.ValueType)).ToString() +
                                  " " + 
                                  StartCounterMark.Description);
                }
            }
            else
            {
                log.WriteLine("      " + Key + ": THIS SEQUENCE HAS NO START MARK"); 
            }

            foreach (PerformanceMark accruedMark in _accruedMarks)
            {
                if (accruedMark.IsTimer)
                {
                    relativeValue = PerformanceUtilities.Cost(StartMark.RawValue,accruedMark.RawValue);

                    log.WriteLine("       a ..... Time: " +  PerformanceUtilities.CostInSeconds(relativeValue).ToString() + " " + accruedMark.Description);
                }
                else
                {
                    log.WriteLine("       a ..... " + 
                                  PerformanceTracker.GetCounterName(accruedMark.PerformanceCounterType) + 
                                  ": " +
                                  (Convert.ChangeType(accruedMark.RawValue,accruedMark.ValueType)).ToString() +
                                  " " + 
                                  accruedMark.Description);
                }
            }

            if (_iterativeMarkTimer != null)
            {
                log.WriteLine("       i ..... Average " +  PerformanceUtilities.CostInSeconds(_iterativeMarkTimer.AverageValue()).ToString() + " " + _iterativeMarkTimer.Mark.Description);
                log.WriteLine("       i ..... Minimum " +  PerformanceUtilities.CostInSeconds(_iterativeMarkTimer.MinimumValue()).ToString());
                log.WriteLine("       i ..... Maximum " +  PerformanceUtilities.CostInSeconds(_iterativeMarkTimer.MaximumValue()).ToString());
                log.WriteLine("       i ..... Number  " +  _iterativeMarkTimer.IterativeCount.ToString());
                log.WriteLine("       i ..... Total   " +  PerformanceUtilities.CostInSeconds(_iterativeMarkTimer.TotalValue()).ToString());
            }

            if (EndCounterMark != null)
            {
                log.WriteLine("       e ..... " + 
                    PerformanceTracker.GetCounterName(EndCounterMark.PerformanceCounterType) + 
                    ": " +
                    (Convert.ChangeType(EndCounterMark.RawValue,EndCounterMark.ValueType)).ToString() +
                    " " + 
                    EndCounterMark.Description);
            }

            if (EndMark != null)
            {
                relativeValue = PerformanceUtilities.Cost(StartMark.RawValue,EndMark.RawValue);

                log.WriteLine("       e Time: " +  PerformanceUtilities.CostInSeconds(relativeValue).ToString() + " " + EndMark.Description);
            }
            else
            {
                log.WriteLine("      " + Key + ": THIS SEQUENCE HAS NO END MARK"); 
            }
        }

        #endregion Reporting
    }
 
    /// <summary>
    /// Preformance tracker is used to track points of time in execution sequences
    /// </summary>
    /// <ExternalAPI/>     
//    [StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey=Microsoft.Internal.BuildInfo.WCP_PUBLIC_KEY_STRING)]
    public class PerformanceTracker
    {
        private ArrayList _keysToReport = null;

        static private bool _disabled = true;
        static private bool _reportToConsole = false;

        static private PerformanceTracker _self = null;

        private string _processName  = null;
        private string _scenarioName = null;

        /// <summary>
        /// Preformance tracker is used to track points of time in execution sequences
        /// </summary>
        private PerformanceTracker()
        {
            _quickLock = new Object();
            _markSequences = new Hashtable();
            _performanceCounters = new Hashtable();
        }

        /// <summary>
        /// Initialize static data
        /// </summary>
        static PerformanceTracker()
        {
            PerformanceUtilities.QueryPerformanceCounter(out _startOfPerformanceTracking);
        }

        /// <summary>
        /// Singleton
        /// </summary>
        /// <ExternalAPI Inherit="true"/>
        static public PerformanceTracker Singleton
        {
            get
            {
                if (_self == null)
                {
                    _self = new PerformanceTracker();
                }

                return _self;
            }
        }

        /// <summary> Enable state of PerformanceTracker</summary>
        /// <ExternalAPI Inherit="true"/>
        static public bool Enabled
        {
            get {return !_disabled;}
            set {_disabled = !value;}
        }

        
        private string ProcessName
        {
            get
            {
                if (_processName == null)
                {
                    _processName  = Process.GetCurrentProcess().ProcessName;
                }
                return _processName;
            }
        }

        /// <summary> Enable state of PerformanceTracker</summary>
        /// <ExternalAPI Inherit="true"/>
        public string ScenarioName
        {
            get 
            {
                if (_scenarioName == null)
                {
                    _scenarioName  = "Not Set";
                }
                
                return _scenarioName;
            }
            set 
            {
                _scenarioName = value;
            }
        }

        #region Garbage Collection

        private Thread _threadWaitingForPendingFinalizers = null;
        private int _waitTime = 0;

        private void awakenWaitingThread()
        {
            Thread.Sleep(_waitTime);

            if ((_threadWaitingForPendingFinalizers.ThreadState == System.Threading.ThreadState.Suspended) ||
                (_threadWaitingForPendingFinalizers.ThreadState == System.Threading.ThreadState.SuspendRequested))
            {
                _threadWaitingForPendingFinalizers.Resume();
            }
        }

        /// <summary>
        /// Force garbage collection
        /// </summary>
        /// <ExternalAPI/> 
        public void ForceGarbageCollection()
        {
            if (_disabled)
            {
                return;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Force garbage collection
        /// </summary>
        /// <ExternalAPI/> 
        public void ForceGarbageCollection(int waitTime)
        {
            if (_disabled)
            {
                return;
            }

            Thread myAlarm = new Thread(new ThreadStart(awakenWaitingThread));

            _threadWaitingForPendingFinalizers = Thread.CurrentThread;

            _waitTime = waitTime;

            myAlarm.Start();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion Garbage Collection

        #region Priority Boosting

        private bool _threadFocusEnabled  = false;
        private bool _threadFocusOn       = false;

        private bool _processBoostEnabled = false;

        private ThreadPriorityLevel _focusThreadPriorityLevel    = ThreadPriorityLevel.Highest;
        private ThreadPriorityLevel _nonfocusThreadPriorityLevel = ThreadPriorityLevel.Idle;
        
        private Hashtable _originaThreadPriorities;

        private Process _process;
        private ProcessPriorityClass _boostedPriorityLevel = ProcessPriorityClass.AboveNormal;
        private ProcessPriorityClass _originalPriorityLevel;

        [System.Security.SuppressUnmanagedCodeSecurity, System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        /// <summary>
        /// Set or get process boost priority level
        /// </summary>
        /// <ExternalAPI Inherit="true"/>
        public ProcessPriorityClass BoostedPriorityLevel
        {
            get 
            {
                return _boostedPriorityLevel;
            }
            set 
            {
                _boostedPriorityLevel = value;
                
                if (_processBoostEnabled)
                {
                    _process.PriorityClass = value;
                }
            }
        }

        /// <summary>
        /// Set or get focus thread priority level
        /// </summary>
        /// <ExternalAPI Inherit="true"/>
        public ThreadPriorityLevel FocusThreadPriorityLevel
        {
            get {return _focusThreadPriorityLevel;}
            set {_focusThreadPriorityLevel = value;}
        }

        /// <summary>
        /// Set or get nonfocus thread priority level
        /// </summary>
        /// <ExternalAPI Inherit="true"/>
        public ThreadPriorityLevel NonfocusThreadPriorityLevel
        {
            get {return _nonfocusThreadPriorityLevel;}
            set {_nonfocusThreadPriorityLevel = value;}
        }

        /// <summary>
        /// Set or get the enabling or disabling of thread focus 
        /// </summary>
        /// <ExternalAPI Inherit="true"/>
        public bool ThreadFocusEnabled
        {
            get 
            {
                return _threadFocusEnabled;
            }
            set 
            {
                if (_disabled)
                {
                    return;
                }

                StartOverheadTime();

                bool killThreadFocus = false;

                if ((_threadFocusEnabled) && !value)
                {
                    if (_threadFocusOn)
                    {
                        killThreadFocus = true;    
                    }
                }

                _threadFocusEnabled = value;

                // So we don't nest Start and EndOverHeadTime
                EndOverheadTime();

                if (killThreadFocus)
                {
                    ThreadFocus(false);
                }
            }
        }

        /// <summary>
        /// Start or stop process level priority boost
        /// </summary>
        /// <param name="enable"></param>
        /// <ExternalAPI/> 
        public void BoostProcess(bool enable)
        {
            if (_disabled)
            {
                return;
            }

            StartOverheadTime();

            if (enable)
            {
                if (_processBoostEnabled == enable)
                {
                    Debug.Assert(false,"Process boost is already enabled."); 
                }
                else
                {
                    _process = Process.GetCurrentProcess();
                    _originalPriorityLevel = _process.PriorityClass;
                    _process.PriorityClass = _boostedPriorityLevel;
                }
            }
            else
            {
                if (_processBoostEnabled == enable)
                {
                    Debug.Assert(false,"Process boost is already disabled."); 
                }
                else
                {
                    _process.PriorityClass = _originalPriorityLevel;
                    _process = null;
                }
            }

            _processBoostEnabled = enable;

            EndOverheadTime();
        }

        /// <summary>
        /// Start or stop thread level priority boost
        /// </summary>
        /// <param name="enable"></param>
        /// <ExternalAPI/> 
        public void ThreadFocus(bool enable)
        {
            if ((_disabled) && ((!_threadFocusEnabled) || (!enable)))
            {
                return;
            }

            StartOverheadTime();

            if (enable)
            {
                if (_threadFocusOn)
                {
                    Debug.Assert(false,"Thread focus is already on."); 
                    return;
                }

                ProcessThreadCollection processThreads = Process.GetCurrentProcess().Threads;

                int currentThreadID = GetCurrentThreadId();

                _originaThreadPriorities = new Hashtable();

                foreach(ProcessThread thread in processThreads)
                {
                    _originaThreadPriorities.Add(thread,thread.PriorityLevel);

                    if (thread.Id == (int) currentThreadID)
                    {
                        thread.PriorityLevel = _focusThreadPriorityLevel;
                    }
                    else
                    {
                        thread.PriorityLevel = _nonfocusThreadPriorityLevel;
                    }
                }

                _threadFocusOn = true;
            }
            else
            {
                if (!_threadFocusOn)
                {
                    Debug.Assert(false,"Thread focus is already off."); 
                    return;
                }

                if (_originaThreadPriorities != null)
                {
                    ProcessThreadCollection processThreads = Process.GetCurrentProcess().Threads;

                    foreach(ProcessThread thread in processThreads)
                    {
                        if (_originaThreadPriorities.ContainsKey(thread))
                        {
                            thread.PriorityLevel = ((ThreadPriorityLevel) _originaThreadPriorities[thread]);
                        }
                    }
                }

                _originaThreadPriorities = null;
                _threadFocusOn = false;
            }

            EndOverheadTime();
        }

        #endregion Priority Boosting

        #region Tracking

        private Hashtable _markSequences        = null;
        private long      _accruedWhileTracking = 0;
        private Object    _quickLock            = null;
        
        private static long _startOfPerformanceTracking = 0;

        internal static long StartOfPerformanceTracking
        {
            get {return _startOfPerformanceTracking;}
        }

        private long _startOfOverheadCount = 0;
        private long _endOfOverheadCount   = 0;

        //private bool _trackingOverhead = false;

        private void StartOverheadTime()
        {
            bool locked;

            // On one hand we want to block as soon as possible for thread safety
            // On the other hand we don't want timing waiting not to be part of 
            // the accrued instrumentation time.
            //
            // Solution to try to block, but not wait before
            // starting the accrued instrumentation time.
            //
            // This is safe, because we do the quick try inside a lock block.
            //

            lock(_quickLock)
            {
                //Debug.Assert(_trackingOverhead == false,"Recursion in PerformanceTracker is really bad because of monitors."); 

                //_trackingOverhead = true;

                locked = Monitor.TryEnter(this);

                PerformanceUtilities.QueryPerformanceCounter(out _startOfOverheadCount);
            }

            if (!locked)
            {
                Monitor.Enter(this);
            }   
        }
        
        private void EndOverheadTime()
        {
            PerformanceUtilities.QueryPerformanceCounter(out _endOfOverheadCount);

            _accruedWhileTracking += PerformanceUtilities.Cost(_startOfOverheadCount,_endOfOverheadCount);
          
            //_trackingOverhead = false;

            Monitor.Exit(this);
        }


        private void NewStartMark(ref PerformanceMarkSequence applicableSequence,
                                  PerformanceMark newStartMark,
                                  string key)
        {
            if (applicableSequence == null)
            {
                PerformanceMarkSequence newMarkSequence  = new PerformanceMarkSequence();
                _markSequences[key] = newMarkSequence;
                applicableSequence  = newMarkSequence;
            }
            else
            {
                applicableSequence.Reset();
            }

            applicableSequence.Key = key;
            applicableSequence.StartMark = newStartMark;
        }

        /// <summary>
        /// Mark is used to map a point in time
        /// to a point in the execution sequence.
        /// </summary>
        /// <ExternalAPI/> 
        public void Mark(string key, 
                         MarkType markType, 
                         string description)
        {
            if (_disabled)
            {
                return;
            }

            StartOverheadTime();

            lock(_quickLock)
            {
                PerformanceMark newMark   = new PerformanceMark(CounterType.Timer,markType,typeof(long));
                newMark.Description  = description;

                bool isSequenceStart = false;

                PerformanceMarkSequence applicableSequence = (PerformanceMarkSequence) _markSequences[key];

                if (markType != MarkType.Start)
                {
#pragma warning disable 1691 // Pragma on the following line disables a PRESharp warning, which 
#pragma warning disable 56506 
                    if (applicableSequence == null)
                    {
                        // Instead of asserting auto-start the sequence
                        // But don't recurse to avoid multiple blocks
                    
                        NewStartMark(ref applicableSequence,
                            newMark,
                            key);

                        isSequenceStart = true;
                    }

                    if (markType == MarkType.End)
                    {
                        Debug.Assert(applicableSequence.EndMark == null,"A sequence should not have two end marks");
                        applicableSequence.EndMark = newMark;
                    }
                    else
                    {
                        if (markType == MarkType.Accrued)
                        {
                            applicableSequence.AddAccrued(newMark);
                        }
                        else
                        {
                            applicableSequence.AccumulateIterativeTimer(newMark);
                        }
                    }
#pragma warning restore 56506
#pragma warning restore 1691
                }
                else
                {
                    NewStartMark(ref applicableSequence,
                        newMark,
                        key);

                    isSequenceStart = true;
                } 
   
                EndOverheadTime();

                long currentCount;
                long snapTime;

                PerformanceUtilities.QueryPerformanceCounter(out currentCount);

                snapTime = currentCount - _accruedWhileTracking; 

                if (isSequenceStart)
                {
                    applicableSequence.StartMark.RawValue = snapTime;
                }

                newMark.RawValue = snapTime;

                if ((markType == MarkType.End) && (_reportToConsole))
                {
                    WriteReportToConsole(key);
                }
            }
        }

        /// <summary>
        /// Mark is used to map a counter value
        /// to a point in the execution sequence.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="counterType"></param>
        /// <param name="markType"></param>
        /// <param name="description"></param>
        /// <ExternalAPI/> 
        public void CounterMark(string key,
                                CounterType counterType,
                                MarkType markType,
                                string description)
        {
            if (_disabled)
            {
                return;
            }

            StartOverheadTime();

            lock(_quickLock)
            {
                PerformanceMark newCounterMark = null;
                PerformanceMark newTimerMark   = null;
                bool isSequenceStart = false;

                newCounterMark = new PerformanceMark(counterType,markType,typeof(long));

                SetMarkValueType(counterType,newCounterMark);

                newCounterMark.RawValue = CounterMarkValue(counterType);

                newCounterMark.Description  = description;

                if ((markType == MarkType.Start) || (markType == MarkType.End))
                {
                    newTimerMark = new PerformanceMark(CounterType.Timer,markType,typeof(long));
                    newTimerMark.Description = description;
                }

                PerformanceMarkSequence applicableSequence = (PerformanceMarkSequence) _markSequences[key];

#pragma warning disable 1691 // Pragma on the following line disables a PRESharp warning, which 
#pragma warning disable 56506
                if (markType != MarkType.Start)
                {
                    if (applicableSequence == null)
                    {
                        // Instead of asserting auto-start the sequence
                        // But don't recurse to avoid multiple blocks
                    
                        NewStartMark(ref applicableSequence,
                            newTimerMark,
                            key);

                        isSequenceStart = true;
                    }

                    if (markType == MarkType.End)
                    {
                        Debug.Assert(applicableSequence.EndMark == null,"A sequence should not have two end marks");
                        applicableSequence.EndMark = newTimerMark;
                        applicableSequence.EndCounterMark = newCounterMark;
                    }
                    else
                    {                    
                        if (markType == MarkType.Accrued)
                        {
                            applicableSequence.AddAccrued(newCounterMark);
                        }
                        else
                        {
                            Debug.Assert(false,"Not available yet");
                        }
                    }
                }
                else
                {
                    NewStartMark(ref applicableSequence,
                        newTimerMark,
                        key);

                    applicableSequence.StartCounterMark = newCounterMark;

                    isSequenceStart = true;
                }
#pragma warning restore 56506
#pragma warning restore 1691

                EndOverheadTime();

                if (newTimerMark != null)
                {
                    long currentCount;
                    long snapTime;

                    PerformanceUtilities.QueryPerformanceCounter(out currentCount);

                    snapTime = currentCount - _accruedWhileTracking; 

                    if (isSequenceStart)
                    {
                        applicableSequence.StartMark.RawValue = snapTime;
                    }

                    newTimerMark.RawValue = snapTime;

                    if ((markType == MarkType.End) && (_reportToConsole))
                    {
                        WriteReportToConsole(key);
                    }
                }
            }
        }

        /// <summary>
        /// True if there is a sequence for the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public bool KeyInUse(string key)
        {
            bool containsKey = _markSequences.ContainsKey(key);
            return (containsKey);
        }

        /// <summary>
        /// ReserveKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Returns false if the key is already in use.</returns>
        private bool ReserveKey(string key)
        {
            bool containsKey = _markSequences.ContainsKey(key);

            if (!containsKey)
            {
                PerformanceMarkSequence newMarkSequence  = new PerformanceMarkSequence();
                _markSequences[key] = newMarkSequence; 
            }

            return (!containsKey);
        }

        /// <summary>
        /// Safest to generate a new key instead of assuming
        /// that a key is unique.
        /// </summary>
        /// <param name="seedKey"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public string GenerateSequenceKey(string seedKey)
        {
            string uniqueKey = seedKey;

            StartOverheadTime();

            bool  containsKey = ReserveKey(seedKey);
            long  count = 0;

            while (containsKey)
            {
                uniqueKey = seedKey + count.ToString();
                count ++;
            }

            EndOverheadTime();

            return uniqueKey;
        }

        #endregion Tracking

        #region Performance Counters

        private Hashtable _performanceCounters = null;

        //
        // Performance Object Names
        //
        //private static string _netClrExceptionsObject      = ".NET CLR Exceptions";
        //private static string _netClrInteropObject         = ".NET CLR Interop";
        private static string _netClrLoadingObject         = ".NET CLR Loading";
        //private static string _netClrLocksAndThreadsObject = ".NET CLR LocksAndThreads";
        private static string _netClrMemoryObject          = ".NET CLR Memory";
        //private static string _memoryObject                = "Memory";
        //private static string _objectsObject               = "Objects";
        private static string _processObject               = "Process";
        //private static string _threadObject                = "Thread";

        //
        // Performance Counter Names
        //

        private static string _timer                         = "Timer";
        //private static string _exceptionsThrown              = "# of Exceps Thrown";
        //private static string _numberOfMarshallings          = "# of marshallings";
        private static string _percentTimeLoading            = "% Time Loading";
        private static string _currentNumberOfAssemblies     = "Current Assemblies";
        private static string _totalNumberOfAssemblies       = "Total Assemblies";
        //private static string _currentNumberOfClassesLoaded  = "Current Classes Loaded";
        //private static string _totalNumberOfClassesLoade     = "Total Classes Loaded";
        //private static string _currentNumberOfThreads        = "# of current recognized threads";
        //private static string _totalNumberOfThreads          = "# of total recognized threads";
        //private static string _numberOfGCHandles             = "# GC Handles";
        //private static string _numberOfInducedGCHandles      = "# Induced GC";
        //private static string _numberOfPinnedObjects         = "# of Pinned Objects";
        private static string _numberOfFinalizationSurvivors    = "Finalization Survivors";
        //private static string _availableBytes                = "Available Bytes";
        //private static string _numberOfMutexes               = "Mutexes";
        //private static string _percentagePriviledgeTime      = "% Privileged Time";
        //private static string _percentageProcessorTime       = "% Processor Time";
        //private static string _threadPercentageProcessorTime = "% Processor Time";
        //private static string _handleCount                   = "Handle Count";
        //private static string _pageFaultsSec                 = "Page Faults/sec";
        private static string _workingSet                    = "Working Set";
        private static string _peakWorkingSetCounter         = "Working Set Peak";
        //private static string _percentProcessorTime          = "% Processor Time";
        private static string _invalidCounter                = "Invalid Counter";

        /// <summary>
        /// Maps a counter type to a counter name
        /// </summary>
        /// <param name="counterType"></param>
        /// <returns></returns>
        internal static string GetCounterName(CounterType counterType)
        {
            switch(counterType)
            {
                case CounterType.Timer:
                    return _timer;
                case CounterType.PercentTimeLoading:
                    return _percentTimeLoading;
                case CounterType.NumberOfAssemblies:
                    return _currentNumberOfAssemblies;
                case CounterType.TotalNumberOfAssemblies:
                    return _totalNumberOfAssemblies;
                case CounterType.FinalizationSurvivors:
                    return _numberOfFinalizationSurvivors;
                case CounterType.WorkingSet:
                    return _workingSet;
                case CounterType.PeakWorkingSet:
                    return _peakWorkingSetCounter;
            }

            Debug.Assert(false,"Invalid Counter -- How did we get here switching on an enum? Did you forget a case?");

            return _invalidCounter;
        }

        
        ///<summary>Timer</summary>
        private long Timer   
        {
            get 
            {
                long time = 0;

                PerformanceUtilities.QueryPerformanceCounter(out time);

                return time;
            }
        }

        ///<summary>PercentTimeLoading</summary>
        private long PercentTimeLoading   
        {
            get 
            {
                long rawValue = 0;

                PerformanceCounter pwsPerformanceCounter = GetPerformanceCounter(_netClrLoadingObject,_percentTimeLoading,ProcessName);
                rawValue = pwsPerformanceCounter.RawValue;
                return rawValue;
            }
        }

        ///<summary>NumberOfAssemblies</summary>
        private long NumberOfAssemblies   
        {
            get 
            {
                long rawValue = 0;

                PerformanceCounter pwsPerformanceCounter = GetPerformanceCounter(_netClrLoadingObject,_currentNumberOfAssemblies,ProcessName);
                rawValue = pwsPerformanceCounter.RawValue;
                return rawValue;
            }
        }

        ///<summary>TotalNumberOfAssemblies</summary>
        private long TotalNumberOfAssemblies   
        {
            get 
            {
                long rawValue = 0;

                PerformanceCounter pwsPerformanceCounter = GetPerformanceCounter(_netClrLoadingObject,_totalNumberOfAssemblies,ProcessName);
                rawValue = pwsPerformanceCounter.RawValue;
                return rawValue;
            }
        }

        ///<summary>FinalizationSurvivors</summary>
        private long FinalizationSurvivors  
        {
            get 
            {
                long rawValue = 0;

                PerformanceCounter pwsPerformanceCounter = GetPerformanceCounter(_netClrMemoryObject,_numberOfFinalizationSurvivors,ProcessName);
                rawValue = pwsPerformanceCounter.RawValue;
                return rawValue;
            }
        }

        ///<summary>WorkingSet</summary>
        private long WorkingSet   
        {
            get 
            {
                long rawValue = 0;

                PerformanceCounter pwsPerformanceCounter = GetPerformanceCounter(_processObject,_workingSet,ProcessName);
                rawValue = pwsPerformanceCounter.RawValue;
                return rawValue;
            }
        }

        ///<summary>PeakWorkingSet</summary>
        private long PeakWorkingSet   
        {
            get 
            {
                long rawValue = 0;

                PerformanceCounter pwsPerformanceCounter = GetPerformanceCounter(_processObject,_peakWorkingSetCounter,ProcessName);
                rawValue = pwsPerformanceCounter.RawValue;
                return rawValue;
            }
        }

        private long CounterMarkValue(CounterType counterType)
        {
            switch(counterType)
            {
                case CounterType.Timer:
                    return Timer;
                case CounterType.PercentTimeLoading:
                    return PercentTimeLoading;
                case CounterType.NumberOfAssemblies:
                    return NumberOfAssemblies;
                case CounterType.TotalNumberOfAssemblies:
                    return TotalNumberOfAssemblies;
                case CounterType.FinalizationSurvivors:
                    return FinalizationSurvivors;
                case CounterType.WorkingSet:
                    return WorkingSet;
                case CounterType.PeakWorkingSet:
                    return PeakWorkingSet;
            }

            Debug.Assert(false,"Invalid Counter -- How did we get here switching on an enum? Did you forget a case?");

            return 0;
        }

        private void SetMarkValueType(CounterType counterType, PerformanceMark mark)
        {
            switch(counterType)
            {
                case CounterType.Timer:
                    mark.ValueType = typeof(long);
                    return;
                case CounterType.PercentTimeLoading:
                    mark.ValueType = typeof(double);
                    return;
                case CounterType.NumberOfAssemblies:
                case CounterType.TotalNumberOfAssemblies:
                case CounterType.FinalizationSurvivors:
                case CounterType.WorkingSet:
                case CounterType.PeakWorkingSet:
                    mark.ValueType = typeof(long);
                    return;
            }

            Debug.Assert(false,"Invalid Counter -- How did we get here switching on an enum? Did you forget a case?");
        }

        /// <summary>
        /// GetPerformanceCounter
        /// If a counter has not been started start it.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="counterName"></param>
        /// <returns></returns>
        private PerformanceCounter GetPerformanceCounter(string categoryName, string counterName)
        {
            string key = categoryName + counterName;

            PerformanceCounter applicableCounter = (PerformanceCounter) _performanceCounters[key];

            if (applicableCounter == null)
            {
                applicableCounter = new PerformanceCounter(categoryName,counterName);

                _performanceCounters.Add(key,applicableCounter);

                //Useful debug line for figuring out PerformanceCounter types
                //Console.WriteLine(applicableCounter.CounterType.ToString());
            }

            return applicableCounter;
        }

        /// <summary>
        /// If a counter has not been started start it.
        /// This is the instance version of GetPerformanceCounter
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="counterName"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        private PerformanceCounter GetPerformanceCounter(string categoryName, string counterName, string instanceName)
        {
            string key = categoryName + counterName + instanceName;

            PerformanceCounter applicableCounter = (PerformanceCounter) _performanceCounters[key];

            if (applicableCounter == null)
            {
                applicableCounter = new PerformanceCounter(categoryName,counterName,instanceName);
                
                _performanceCounters.Add(key,applicableCounter);

                //Useful debug line for figuring out PerformanceCounter types
                //Console.WriteLine(applicableCounter.CounterType.ToString());
            }

            return applicableCounter;
        }

        #endregion Performance Counters

        #region Process Tracking Object Model
   
        /// <summary>
        /// Calculates the elpased time, 
        /// from the start to the end 
        /// of the sequence specified by the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public double Elapsed(string key)
        {
            if (_disabled)
            {
                return 0.0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long elapsed = ((PerformanceMarkSequence) _markSequences[key]).Elapsed();
                EndOverheadTime();
                return PerformanceUtilities.CostInSeconds(elapsed);
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1.0;
            }
        }

        /// <summary>
        /// Calculates the elpased time, between two marks 
        /// in the sequence specified by the key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public double Elapsed(string key, int from, int to)
        {
            if (_disabled)
            {
                return 0.0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long elapsed = ((PerformanceMarkSequence) _markSequences[key]).Elapsed(from,to);
                EndOverheadTime();
                return PerformanceUtilities.CostInSeconds(elapsed);
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1.0;
            }
        }

        /// <summary>
        /// IterativeFirst
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public double IterativeFirst(string key)
        {
            if (_disabled)
            {
                return 0.0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long minimum = ((PerformanceMarkSequence) _markSequences[key]).IterativeFirst;
                EndOverheadTime();
                return PerformanceUtilities.CostInSeconds(minimum);
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1;
            }
        }

        /// <summary>
        /// IterativeLast
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public double IterativeLast(string key)
        {
            if (_disabled)
            {
                return 0.0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long minimum = ((PerformanceMarkSequence) _markSequences[key]).IterativeLast;
                EndOverheadTime();
                return PerformanceUtilities.CostInSeconds(minimum);
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1;
            }
        }

        /// <summary>
        /// IterativeMax
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public double IterativeMaximum(string key)
        {
            if (_disabled)
            {
                return 0.0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long maximum = ((PerformanceMarkSequence) _markSequences[key]).IterativeMaximum;
                EndOverheadTime();
                return PerformanceUtilities.CostInSeconds(maximum);
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1;
            }
        }

        /// <summary>
        /// IterativeMinimum
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public double IterativeMinimum(string key)
        {
            if (_disabled)
            {
                return 0.0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long minimum = ((PerformanceMarkSequence) _markSequences[key]).IterativeMinimum;
                EndOverheadTime();
                return PerformanceUtilities.CostInSeconds(minimum);
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1;
            }
        }

        /// <summary>
        /// IterativeAverage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public double IterativeAverage(string key)
        {
            if (_disabled)
            {
                return 0.0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long average = ((PerformanceMarkSequence) _markSequences[key]).IterativeAverage;
                EndOverheadTime();
                return PerformanceUtilities.CostInSeconds(average);
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1;
            }
        }

        /// <summary>
        /// IterativeCount
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public long IterativeCount(string key)
        {
            if (_disabled)
            {
                return 0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long average = ((PerformanceMarkSequence) _markSequences[key]).IterativeCount;
                EndOverheadTime();
                return average;
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1;
            }
        }

        /// <summary>
        /// IterativeTotal
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public double IterativeTotal(string key)
        {
            if (_disabled)
            {
                return 0.0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long total = ((PerformanceMarkSequence) _markSequences[key]).IterativeTotal;
                EndOverheadTime();
                return PerformanceUtilities.CostInSeconds(total);
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1;
            }
        }

        /// <summary>
        /// AccruedCount
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public long AccruedCount(string key) 
        {
            if (_disabled)
            {
                return 0;
            }

            StartOverheadTime();

            bool containsKey = _markSequences.ContainsKey(key);

            if (containsKey)
            {
                long average = ((PerformanceMarkSequence) _markSequences[key]).AccruedCount;
                EndOverheadTime();
                return average;
            }
            else
            {
                Debug.Assert(false,"Sequence for key \"" + key + "\" not found.");
                EndOverheadTime();
                return -1;
            }
        }

        #endregion Process Tracking Object Model

        #region Reporting

        /// <summary> Enable or disable reporting to the Console</summary>
        /// <ExternalAPI Inherit="true"/>
        static public bool ReportToConsole
        {
            get {return _reportToConsole;}
            set {_reportToConsole = value;}
        }

        /// <summary>
        /// GetSequenceEnumerator
        /// </summary>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public IDictionaryEnumerator GetSequenceEnumerator()
        {
            return _markSequences.GetEnumerator();
        }

        /// <summary>
        /// By default we report on all keys
        /// use this method to define a key on which to report.
        /// </summary>
        /// <ExternalAPI/> 
        public void ReportOnThisKey(string key)
        {
            if (_disabled)
            {
                return;
            }

            StartOverheadTime();

            if (_keysToReport == null)
            {
                _keysToReport = new ArrayList();
            }

            _keysToReport.Add(key);

            EndOverheadTime();
        }

        /// <summary>
        /// By default we report on all keys
        /// use this method to reset the reporting to all
        /// </summary>
        /// <ExternalAPI/> 
        public void ReportOnAll()
        {
            if (_disabled)
            {
                return;
            }

            StartOverheadTime();

            _keysToReport = null;

            EndOverheadTime();
        }

        /// <summary>
        /// WriteReportToConsole
        /// </summary>
        /// <param name="key"></param>
        private void WriteReportToConsole(string key)
        {
            if (_disabled)
            {
                return;
            }

            StartOverheadTime();

            StreamWriter log = new StreamWriter(Console.OpenStandardOutput());

            if (_markSequences.ContainsKey(key))
            {
                ((PerformanceMarkSequence) _markSequences[key]).Report(log);
                Console.WriteLine();
            }

            log.Flush();
            log.Close();

            EndOverheadTime();
        }

        // Print out report header
        private void ReportHeader(StreamWriter log)
        {
            long relativeValue = 0;
            long currentCount  = 0;

            PerformanceUtilities.QueryPerformanceCounter(out currentCount);

            relativeValue = PerformanceUtilities.Cost(_startOfPerformanceTracking,currentCount);
            
            log.WriteLine("Scenario: " + ScenarioName);
            log.WriteLine("Process:  " + ProcessName);
            log.WriteLine();
            log.WriteLine("PerformanceTracking for " + PerformanceUtilities.CostInSeconds(relativeValue) + " seconds");
            log.WriteLine();
        }

        // Get the last value for all PerformanceCounters quiered
        private void ReportPerformanceCounters(StreamWriter log)
        {
        }

        /// <summary>
        /// Write a preformance log to the specified file
        /// </summary>
        /// <ExternalAPI/> 
        public void Log(string outputFile)
        {
            if (_disabled)
            {
                return;
            }

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            StartOverheadTime();

            StreamWriter log = new StreamWriter(File.OpenWrite(outputFile));

            log.Flush();
            log.Close();

            EndOverheadTime();
        }


        /// <summary>
        /// Write a preformance report to the specified file
        /// </summary>
        /// <ExternalAPI/> 
        public void Report(string outputFile)
        {
            if (_disabled)
            {
                return;
            }

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            StartOverheadTime();

            bool reportAll = _keysToReport == null;
            
            StreamWriter log = new StreamWriter(File.OpenWrite(outputFile));

            ReportHeader(log);

            if (reportAll)
            {
                ReportPerformanceCounters(log);

                IDictionaryEnumerator _markSequencesEnumerator = _markSequences.GetEnumerator();

                while (_markSequencesEnumerator.MoveNext())
                {
                    ((PerformanceMarkSequence)_markSequencesEnumerator.Value).Report(log);
                    log.WriteLine();
                }
            }
            else
            {
                foreach (string currentKey in _keysToReport)
                {
                    if (_markSequences.ContainsKey(currentKey))
                    {
                        ((PerformanceMarkSequence) _markSequences[currentKey]).Report(log);
                        log.WriteLine();
                    }
                }
            }

            log.Flush();
            log.Close();

            EndOverheadTime();
        }

        #endregion Reporting
    }
}
