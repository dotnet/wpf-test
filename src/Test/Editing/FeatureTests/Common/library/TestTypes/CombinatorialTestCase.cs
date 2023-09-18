// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a base type to implement combinatorial engine-based test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/TestTypes/CombinatorialTestCase.cs $")]

namespace Test.Uis.TestTypes
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Microsoft.Test;
    using Microsoft.Test.Logging;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// Provides a base class for test cases based on a combinatorial engine.
    /// </summary>
    /// <remarks>
    /// To use this class, create a subclass and override the following
    /// members: DoGetDimensions (to return dimensions to combine),
    /// DoReadCombination (to get combination values), and DoRunCombination
    /// (to run on a given case). Call NextCombination when the test for
    /// a given combinatino is finished, and override DoTestCaseFinished
    /// for final clean-up as required.
    /// </remarks>
    [TestArgument("SkipCount", "Number of combinations to skip.")]
    public abstract class CustomCombinatorialTestCase : CustomTestCase
    {
        #region Public methods.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            this._combinationNumber = 0;
            this._startTime = DateTime.Now;
            this._results = new List<CombinationResult>();
            this._dimensions = DoGetDimensions();
            _engine = CombinatorialEngine.FromDimensions(Dimensions);
            try
            {
                string val = Settings.GetArgument("StopOnFailure");
                if (val != String.Empty)
                {
                    _stopOnFailure = bool.Parse(Settings.GetArgument("StopOnFailure"));
                }
            }
            catch (FormatException) //caused if improper string is passed
            {
                GlobalLog.LogStatus("StopOnFailure command switch has improper format");
            }
            PrepareCombinationLog();
            SkipInitialCombinations();
            NextCombination();
        }

        #endregion Public methods.

        
        #region Public properties.

        /// <summary>Dimensions being combined.</summary>
        /// <remarks>
        /// This value is set to the result of DoGetDimensions on startup.
        /// </remarks>
        public Dimension[] Dimensions
        {
            get { return this._dimensions; }
        }

        /// <summary>Whether the UI for the combination log is hidden.</summary>
        public bool HideCombinationLog
        {
            get { return this._hideCombinationLog; }
            set { this._hideCombinationLog = value; }
        }

        /// <summary>Moment the test case starts running.</summary>
        public DateTime StartTime
        {
            get { return this._startTime; }
        }

        /// <summary>Test element in window.</summary>
        public FrameworkElement TestElement
        {
            get
            {
                if (HideCombinationLog)
                {
                    return (FrameworkElement)MainWindow.Content;
                }
                else
                {
                    return this._testElement;
                }
            }
            set
            {
                if (HideCombinationLog)
                {
                    MainWindow.Content = value;
                }
                else
                {
                    if (_logControl != null)
                    {
                        if (this._testElement != null && _topPanel.Children.Count == 2)
                        {
                            _topPanel.Children.RemoveAt(1);
                        }
                        if (value != null)
                        {
                            _topPanel.Children.Add(value);
                        }
                    }
                    this._testElement = value;
                }
            }
        }

        #endregion Public properties.

        delegate void ExceptionDelegate(Exception e);
        delegate void DumpExceptionDelegate(Exception e, ExceptionDumpKinds kinds);

        #region Protected methods.

        /// <summary>Called to get dimensions for combinatorial engine.</summary>
        /// <returns>An array of dimensions to combine.</returns>
        protected abstract Dimension[] DoGetDimensions();

        /// <summary>Called when a combination of values should be read.</summary>
        /// <param name='values'>Values for combination, keyed by dimension name.</param>
        /// <returns>true to process this combination.</returns>
        protected virtual bool DoReadCombination(Hashtable values)
        {
            return true;
        }

        /// <summary>
        /// Called when a combination is ready to be run, after
        /// DoReadCombination return true.
        /// </summary>
        protected abstract void DoRunCombination();

        /// <summary>Called after the last combination is processed.</summary>
        protected virtual void DoTestCaseFinished()
        {
        }

        /// <summary>Starts testing the next combination.</summary>
        protected void NextCombination()
        {
            // If we were running a combination, toggle the flag and
            // report a successful pass if we haven't handled this yet.
            if (_isCombinationRunning)
            {
                if (!_hasCombinationFailed)
                {
                    _results.Add(CombinationResult.Passed);
                    _passCount++;
                    Logger.Current.TestLog.Result = TestResult.Pass;
                    Logger.Current.TestLog.Close();
                    Logger.Current.TestLog = null;
                }
                else
                {
                    _results.Add(CombinationResult.Failed);
                    _failCount++;
                    Logger.Current.TestLog.Result = TestResult.Fail;
                    Logger.Current.TestLog.Close();
                    Logger.Current.TestLog = null;
                    if (_stopOnFailure)
                    {
                        DoTestCaseFinished();
                        Logger.Current.ReportResults(_passCount, _failCount, "Failed test count: " + _failCount, false);
                    }
                }
                _isCombinationRunning = false;
            }

            // We could have closed the previous TestLog now from the if statement above...leaving us with no current TestLog opened.
            // So we open a new one here. We do that before actually reading the next combination so that we always have a log to 
            // write to before/between/after combinations
            if (Microsoft.Test.Logging.Log.Current.CurrentVariation == null)
            {
                TestLog combinationResultLog = new TestLog(DriverState.TestName + " - Variation " + (CombinationNumber + 1).ToString());
                Logger.Current.TestLog = combinationResultLog;
            }

            if (!ReadNextCombination())
            {
                DoTestCaseFinished();
                Logger.Current.ReportResults(_passCount, _failCount, "Failed test count: " + _failCount, false);
            }
            else
            {                
                _hasCombinationFailed = false;
                _isCombinationRunning = true;
                DoRunCombination();
            }
        }

        /// <summary>
        /// Sets up the exception handler for the current Dispatcher to avoid
        /// terminating the case when the application fails in product code.
        /// </summary>
        protected override void SetupTestCaseExceptionHandler()
        {
            Application application;

            application = Application.Current;
            if (application == null)
            {
                throw new InvalidOperationException(
                    "There is no UI context on which to set exception handler");
            }

            application.DispatcherUnhandledException +=
                new DispatcherUnhandledExceptionEventHandler(DispatcherException);
        }

        /// <summary>
        /// read only property of the current cobmination number. 
        /// a case can use this to skip verfication for a case that contains a non-fixed bug.
        /// </summary>
        protected int CombinationNumber
        {
            get
            {
                return _combinationNumber;
            }
        }
        #endregion Protected methods.


        #region Internal properties.

        /// <summary>Text to indicate the end of the test case summary.</summary>
        internal const string CombinationSummaryEndText = "ManagedCombinatorialTestCase - Summary End";

        /// <summary>Text to indicate the start of the test case summary.</summary>
        internal const string CombinationSummaryStartText = "ManagedCombinatorialTestCase - Summary Start";

        #endregion Internal properties.


        #region Private methods.


        /// <summary>
        /// A delegate calls this method when an exception reaches the context
        /// dispatcher.
        /// </summary>
        private void DispatcherException(object sender,
            DispatcherUnhandledExceptionEventArgs args)
        {
            Exception exception;
            exception = args.Exception;
            if (_isCombinationRunning)
            {
                //Logger.Current.DumpException(exception, ExceptionDumpKinds.Default);
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new DumpExceptionDelegate(Logger.Current.DumpException), exception, ExceptionDumpKinds.Default);
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ExceptionDelegate(ContinueWithCombination), args.Exception);
                //ContinueWithCombination(exception);
            }
            else
            {
                TestRunner.HandleOutermostException(exception,
                    "Combinatorial case - outside combination");
            }
            args.Handled = true;
        }

        private void ContinueWithCombination(Exception exception)
        {
            const bool continueExecution = true;
            Logger.Current.ReportResult(false,
                "Exception [" + exception +
                "] caught at combinatorial case - within combination",
                continueExecution);
            _hasCombinationFailed = true;
            Test.Uis.Threading.TestQueue.ThreadQueue.Clear();
            Logger.Current.Log("Continuing tests...");

            QueueDelegate(NextCombination);
        }

        /// <summary>Gets the length of the longest key string from the given entries.</summary>
        /// <param name='entries'>Entries to inspect.</param>
        /// <returns>
        /// The length of the longest key string from the given entries; 0 
        /// if entries is empty.
        /// </returns>
        private static int GetLongestKeyString(DictionaryEntry[] entries)
        {
            int result;

            if (entries == null)
            {
                throw new ArgumentNullException("entries");
            }

            result = 0;
            foreach (DictionaryEntry entry in entries)
            {
                if (entry.Key != null && entry.Key.ToString().Length > result)
                {
                    result = entry.Key.ToString().Length;
                }
            }

            return result;
        }

        private int GetTotalCombinationCount()
        {
            int totalCount = 0;
            int totalDimensions = Dimensions.Length;

            totalCount = Dimensions[0].Values.Length;
            for (int i = 1; i < Dimensions.Length; i++)
            {
                totalCount = totalCount * Dimensions[i].Values.Length;
            }

            return totalCount;
        }

        /// <summary>Logs a summary of combination resutls.</summary>
        private void LogSummary()
        {
            System.Text.StringBuilder builder;

            Log(CombinationSummaryStartText);

            builder = new System.Text.StringBuilder(3 * _results.Count);

            LogSummaryLine(CombinationResult.Passed, builder);
            Log(builder.ToString());
            builder.Remove(0, builder.Length);

            LogSummaryLine(CombinationResult.Skipped, builder);
            Log(builder.ToString());
            builder.Remove(0, builder.Length);

            LogSummaryLine(CombinationResult.Failed, builder);
            Log(builder.ToString());

            Log(CombinationSummaryEndText);
        }

        /// <summary>
        /// Logs a summary line to the specified StringBuilder.
        /// </summary>
        /// <param name="result">Type of result to log to line.</param>
        /// <param name="builder">StringBuilder in which to build line.</param>
        private void LogSummaryLine(CombinationResult result, System.Text.StringBuilder builder)
        {
            int runEnd;     // End index of a run of combinations with the specified result.
            int runStart;   // Start index of a run of combinations with the specified result.
            System.Globalization.CultureInfo cultureInfo;   // Culture to format lines.

            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
            runStart = -1;
            runEnd = -1;

            for (int combinationIndex = 0; combinationIndex < _results.Count; combinationIndex++)
            {
                if (_results[combinationIndex] == result)
                {
                    if (runStart == -1)
                    {
                        runStart = combinationIndex;
                    }
                    runEnd = combinationIndex;
                }
                else
                {
                    if (runStart != -1)
                    {
                        //LogSummaryLineRun(result, builder, cultureInfo, runStart, runEnd);
                        LogSummaryLineRun(result, builder, cultureInfo, runStart + 1, runEnd + 1);
                    }
                    runStart = -1;
                }
            }

            if (runStart != -1)
            {
                //LogSummaryLineRun(result, builder, cultureInfo, runStart, runEnd);
                LogSummaryLineRun(result, builder, cultureInfo, runStart + 1, runEnd + 1);
            }
        }

        /// <summary>
        /// Logs a run of combinations with a given result for a summary line 
        /// to the specified StringBuilder.
        /// </summary>
        /// <param name="result">Type of result to log to line.</param>
        /// <param name="builder">StringBuilder in which to build line.</param>
        /// <param name="cultureInfo">Culture used to format numbers.</param>
        /// <param name="runStart">Start index of a run of combinations with the specified result.</param>
        /// <param name="runEnd">End index of a run of combinations with the specified result.</param>
        private void LogSummaryLineRun(CombinationResult result, System.Text.StringBuilder builder,
            System.Globalization.CultureInfo cultureInfo, int runStart, int runEnd)
        {
            if (builder.Length == 0)
            {
                builder.Append(result.ToString());
                builder.Append(": ");
            }
            else
            {
                builder.Append(',');
            }
            builder.Append(runStart.ToString(cultureInfo));
            if (runEnd > runStart)
            {
                builder.Append('-');
                builder.Append(runEnd.ToString(cultureInfo));
            }
        }

        /// <summary>Prepares UI to display progress on combination processing.</summary>
        private void PrepareCombinationLog()
        {
            if (HideCombinationLog)
            {
                return;
            }

            _topPanel = new DockPanel();
            _logControl = new TextBox();

            _logControl.IsUndoEnabled = false;
            _logControl.MinLines = Dimensions.Length + 1;
            _logControl.FontSize = 10;
            _logControl.FontFamily = new System.Windows.Media.FontFamily("Lucida Console");
            DockPanel.SetDock(_logControl, Dock.Top);

            _topPanel.Children.Add(_logControl);
            if (_testElement != null)
            {
                _topPanel.Children.Add(_testElement);
            }
            MainWindow.Content = _topPanel;
        }

        /// <summary>Reads the next combination off the combinatorial engine.</summary>
        private bool ReadNextCombination()
        {
            Hashtable values;

            values = new Hashtable();
        ReadACombination:
            if (_engine.Next(values))
            {
                DictionaryEntry[] entries;
                int keyLength;
                string log;
                TimeSpan span;                

                span = DateTime.Now - _startTime;
                _combinationNumber++;
                Logger.Current.CombinationIndex = _combinationNumber;
                log = "Combination " + _combinationNumber;
                log += " (of " + GetTotalCombinationCount() + ")";
                log += " - time elapsed: " +
                    span.Hours.ToString("d2") + ":" +
                    span.Minutes.ToString("d2") + ":" +
                    span.Seconds.ToString("d2");

                // Put the entries in alphabetical order for predictable output.
                entries = new DictionaryEntry[values.Count];
                values.CopyTo(entries, 0);
                Array.Sort(entries, new EntryComparer());
                keyLength = GetLongestKeyString(entries) + 2;

                foreach (DictionaryEntry entry in entries)
                {
                    log += "\r\n";
                    log += (((entry.Key == null) ? "[null]" : entry.Key.ToString()) + ": ").PadRight(keyLength);
                    log += Dimension.ValueToString(entry.Value);
                }
                Log("\r\n" + log + "\r\n\r\n");
                if (_logControl != null)
                {
                    _logControl.Text = log;
                }

                if (DoReadCombination(values))
                {
                    return true;
                }
                else
                {
                    Log("Combination rejected.");                    
                    _results.Add(CombinationResult.Skipped);
                    goto ReadACombination;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>Skips combinations.</summary>
        private void SkipInitialCombinations()
        {
            int skipCountRequested;

            skipCountRequested = ConfigurationSettings.Current.GetArgumentAsInt("SkipCount");
            if (skipCountRequested > 0)
            {
                Hashtable values;
                int skipCount;

                values = new Hashtable();
                skipCount = 0;
                do
                {
                    if (!_engine.Next(values))
                    {
                        throw new Exception("Test case required to skip " +
                            skipCountRequested + " combinations, but only " +
                            skipCount + " were found.");
                    }
                    _combinationNumber++;
                    skipCount++;
                    _results.Add(CombinationResult.Skipped);
                } while (skipCountRequested > skipCount);
            }
        }

        #endregion Private methods.


        #region Private fields.

        /// <summary>Combination number.</summary>
        private int _combinationNumber;

        /// <summary>Dimensions being combined.</summary>
        private Dimension[] _dimensions;

        /// <summary>Combinatorial engine driving the test case.</summary>
        private CombinatorialEngine _engine;

        /// <summary>Number of combinations that have failed.</summary>
        private int _failCount;

        /// <summary>Whether the UI for the combination log should be removed.</summary>
        private bool _hideCombinationLog;

        /// <summary>Control displaying current combination.</summary>
        private TextBox _logControl;

        /// <summary>Number of combinations that have passed.</summary>
        private int _passCount;

        /// <summary>List of combination results.</summary>
        private List<CombinationResult> _results;

        /// <summary>Time at which the test case started to run.</summary>
        private DateTime _startTime;

        /// <summary>Root element of test display.</summary>
        private FrameworkElement _testElement;

        /// <summary>Top panel in main window.</summary>
        private DockPanel _topPanel;

        /// <summary>Whether a tested combination is in progress.</summary>
        private bool _isCombinationRunning;

        /// <summary>Whether the combination being ran failed.</summary>
        private bool _hasCombinationFailed;

        private bool _stopOnFailure = false;

        #endregion Private fields.


        #region Inner types.

        /// <summary>Result for a single combination.</summary>
        enum CombinationResult
        {
            /// <summary>The combination was not run.</summary>
            Skipped,
            /// <summary>The combination passed.</summary>
            Passed,
            /// <summary>The combination failed.</summary>
            Failed
        }

        /// <summary>Compares to DictionaryEntry objects by key.</summary>
        class EntryComparer : IComparer
        {
            /// <summary>
            /// Compares DictionaryEntry objects and returns a value indicating 
            /// whether one is less than, equal to or greater than the other.
            /// </summary>
            public int Compare(object a, object b)
            {
                if (a == null && b == null) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

                DictionaryEntry entryA = (DictionaryEntry)a;
                DictionaryEntry entryB = (DictionaryEntry)b;

                return String.CompareOrdinal((string)entryA.Key.ToString(), (string)entryB.Key.ToString());
            }
        }

        #endregion Inner types.
    }
}
