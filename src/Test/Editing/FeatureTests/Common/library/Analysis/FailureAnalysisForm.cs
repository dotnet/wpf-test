// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides the user interface for the failure analysis workflow.


[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 2 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/BVT/KeyNavigation/KeyNavigationTest.cs $")]

namespace Test.Uis.Analysis
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.TestTypes;

    #endregion Namespaces.

    /// <summary>Provides a Form to assist testers in failure analysis.</summary>
    class FailureAnalysisForm : System.Windows.Forms.Form
    {
        #region Constructors.

        /// <summary>Creates a new Test.Uis.Analysis.FailureAnalysisForm instance.</summary>
        internal FailureAnalysisForm()
        {
            InitializeComponent();
            PopulateTestCaseDataListBox();
        }

        #endregion Constructors.

        #region Internal methods.

        /// <summary>
        /// Runs any analysis requests passed to this process.
        /// </summary>
        /// <param name="settings">Settings used to modify process behavior.</param>
        /// <returns>true if the analyzer has taken care of any requests; false otherwise.</returns>
        internal static bool DoAnalysis(ConfigurationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            // Special flag for running the analyzer tool.
            if (settings.GetArgumentAsBool("FaRunAnalyzer"))
            {
                new FailureAnalysisForm().ShowDialog();
                return true;
            }

            return false;
        }

        #endregion Internal methods.

       #region Private methods.

        /// <summary>
        /// Analyzes the test execution log supplied by the reader,
        /// and outputs analysis results to the specified
        /// RichTextBox.
        /// </summary>
        /// <param name="reader">Reader for log file.</param>
        /// <param name="logBox">Control to log analysis to.</param>
        /// <param name="familyName">Family name for fonts.</param>
        private static void AnalyzeLog(TextReader reader, RichTextBox logBox, string familyName)
        {
            using (Font normalFont = new Font(familyName, logBox.Font.Size))
            using (Font boldFont = new Font(familyName, logBox.Font.Size, FontStyle.Bold))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Combination passed"))
                    {
                        logBox.SelectionColor = Color.Green;
                        logBox.SelectionFont = normalFont;
                    }
                    else if (line.Contains("[process") && line.Contains(";thread"))
                    {
                        int index;

                        index = line.IndexOf("]");
                        logBox.SelectionFont = normalFont;
                        logBox.SelectedText = line.Substring(0, index) + "\r\n";

                        line = line.Substring(index + 2);
                        logBox.SelectionFont = normalFont;
                    }
                    else if (line.Contains("Test passed successfully"))
                    {
                        logBox.SelectionColor = Color.Green;
                        logBox.SelectionFont = boldFont;
                    }
                    else if (line.Contains("Moving mouse to"))
                    {
                        logBox.SelectionColor = Color.DarkGray;
                        logBox.SelectionFont = normalFont;
                    }
                    else if (line.Contains("Combination"))
                    {
                        logBox.SelectionColor = Color.Blue;
                        logBox.SelectionFont = normalFont;
                    }
                    else if (line.Contains("FAILURE"))
                    {
                        logBox.SelectionColor = Color.Red;
                        logBox.SelectionFont = boldFont;
                    }
                    else
                    {
                        logBox.SelectionFont = normalFont;
                    }
                    logBox.SelectedText = line + "\r\n";
                }
            }
        }

        /// <summary>Initializes all controls in the form.</summary>
        private void InitializeComponent()
        {
            _fontBold = new Font("Tahoma", 10, FontStyle.Bold);
            _fontConsole = new Font("Lucida Console", 10);
            _fontRegular = new Font("Tahoma", 10, FontStyle.Regular);

            _commandLineBox = new TextBox();
            _commandLineBox.Dock = DockStyle.Top;

            _killTestButton = new Button();
            _killTestButton.Dock = DockStyle.Bottom;
            _killTestButton.Enabled = false;
            _killTestButton.Text = "Kill Test";
            _killTestButton.Click += new EventHandler(KillTestButtonClick);

            _logBox = new RichTextBox();
            _logBox.Dock = DockStyle.Fill;
            _logBox.Font = _fontRegular;
            _logBox.ScrollBars = RichTextBoxScrollBars.Both;
            _logBox.WordWrap = false;

            _pollProcessTimer = new Timer();
            _pollProcessTimer.Enabled = false;
            _pollProcessTimer.Interval = 2 * 1000;
            _pollProcessTimer.Tick += new EventHandler(PollProcessTimerTick);

            _runTestButton = new Button();
            _runTestButton.Dock = DockStyle.Top;
            _runTestButton.Text = "Run Test";
            _runTestButton.Click += new EventHandler(RunTestButtonClick);

            _saveLogButton = new Button();
            _saveLogButton.Dock = DockStyle.Bottom;
            _saveLogButton.Text = "Save Log To AnalysisLog.rtf";
            _saveLogButton.Click += new EventHandler(SaveLogButtonClick);

            _testCaseDataListBox = new ListBox();
            _testCaseDataListBox.Dock = DockStyle.Top;
            _testCaseDataListBox.SelectedIndexChanged += new EventHandler(TestCaseDataListBoxSelectedIndexChanged);

            _toolTip = new ToolTip();
            _toolTip.AutomaticDelay = 200;
            _toolTip.SetToolTip(_commandLineBox, "Other useful switches:\r\n/" +
                "NoExit=True\r\n/[DimensionName]=[value1,value2,...]");
            _toolTip.SetToolTip(_testCaseDataListBox, "Select data to prepare for analysis.");

            this.Text = "Failure Analysis";
            this.StartPosition = FormStartPosition.Manual;
            this.Left = 8;
            this.Top = 8;
            this.Height = Screen.GetWorkingArea(this).Height - 16;
            this.Width = Screen.GetWorkingArea(this).Width - 16;

            this.Controls.AddRange(new Control[] {
                _logBox, _killTestButton, _saveLogButton, _runTestButton,
                _commandLineBox, _testCaseDataListBox
            });
        }

        /// <summary>
        /// Cleans up and log on test completion when the test polling timer ticks.
        /// </summary>
        private void PollProcessTimerTick(object sender, EventArgs e)
        {
            if (_testProcess == null)
            {
                return;
            }

            if (_testProcess.HasExited)
            {
                int scrollTargetIndex;

                // Close and kill the process.
                _testProcess.Close();
                _testProcess = null;

                // Remember where the analysis log starts.
                scrollTargetIndex = _logBox.TextLength;

                // Write out the analysis for the test run into the log.
                _logBox.SelectionStart = scrollTargetIndex;
                _logBox.SelectedText = "Output log:\r\n\r\n";
                using (StreamReader reader = new StreamReader(OutputFileName))
                {
                    AnalyzeLog(reader, _logBox, "Lucida Console");
                }

                // Scroll to the start of the log output.
                _logBox.SelectionStart = scrollTargetIndex;
                _logBox.ScrollToCaret();

                // Restore controls for a new run.
                _runTestButton.Enabled = true;
                _killTestButton.Enabled = false;
                _pollProcessTimer.Enabled = false;
            }
        }

        /// <summary>Kills the test process if it's running.</summary>
        private void KillTestButtonClick(object sender, EventArgs e)
        {
            if (_testProcess == null)
            {
                return;
            }
            _logBox.SelectionStart = _logBox.TextLength;
            _logBox.SelectionFont = _fontBold;
            _logBox.SelectedText = "Process killed at " + DateTime.Now + ".\r\n";
            _testProcess.Kill();
        }

        /// <summary>Populates the TestCaseDataListBox with test case data.</summary>
        private void PopulateTestCaseDataListBox()
        {
            _testCaseDataListBox.Items.AddRange(TestCaseData.GetTestCaseDataTable());
        }

        /// <summary>Runs the test selected in the command line.</summary>
        private void RunTestButtonClick(object sender, EventArgs e)
        {
            string commandLine;
            ProcessStartInfo startInfo;

            commandLine = _commandLineBox.Text;
            commandLine += " /OutputFile=" + OutputFileName;

            startInfo = new ProcessStartInfo();
            startInfo.Arguments = commandLine;
            startInfo.FileName = "EditingTest.exe";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            _logBox.SelectionStart = _logBox.TextLength;
            _logBox.SelectionFont = _fontBold;
            _logBox.SelectedText = "Test case run: " + startInfo.FileName +
                " " + startInfo.Arguments + "\r\n";

            _testProcess = Process.Start(startInfo);
            Test.Uis.IO.LogStreamWorkItem item = new Test.Uis.IO.LogStreamWorkItem(
                _testProcess.StandardOutput, "Case StdOut Log: ", true);

            _runTestButton.Enabled = false;
            _killTestButton.Enabled = true;
            _pollProcessTimer.Enabled = true;
        }

        /// <summary>Saves the current log.</summary>
        private void SaveLogButtonClick(object sender, EventArgs e)
        {
            _logBox.SaveFile("AnalysisLog.rtf");
        }

        /// <summary>Refreshes the log when the data for a test case is selected.</summary>
        private void TestCaseDataListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            TestCaseData data;      // Selected data.
            int scrollTargetIndex;  // Index of caret to scroll to.

            data = _testCaseDataListBox.SelectedItem as TestCaseData;
            if (data == null)
            {
                return;
            }

            _logBox.SelectionStart = _logBox.TextLength;
            scrollTargetIndex = _logBox.SelectionStart;

            _logBox.SelectionFont = _fontBold;
            _logBox.SelectedText = data.TestCaseType.FullName + "\r\n";

            _logBox.SelectionFont = _fontRegular;
            _logBox.SelectedText = "Log started at " + DateTime.Now + ".\r\n";

            _logBox.SelectionFont = _fontBold;
            _logBox.SelectedText = "\r\nDimensions:\r\n";

            _logBox.SelectionFont = _fontRegular;
            foreach(Dimension dimension in data.Dimensions)
            {
                _logBox.SelectedText = dimension.Name + "\r\n";
                _logBox.SelectionBullet = true;
                foreach(object value in dimension.Values)
                {
                    _logBox.SelectedText = Dimension.ValueToString(value) + "\r\n";
                }
                _logBox.SelectionBullet = false;
                _logBox.SelectedText = "\r\n";
            }

            _logBox.SelectionStart = scrollTargetIndex;
            _logBox.ScrollToCaret();

            _commandLineBox.Text = "/TestCaseType=" + data.TestCaseType.Name;
            if (data.Filter.Length > 0)
            {
                _commandLineBox.AppendText(" /");
                _commandLineBox.AppendText(data.Filter);
            }
        }

        #endregion Private methods.


        #region Private properties.

        /// <summary>File name for test runs.</summary>
        private const string OutputFileName = "AnalysisOutputLog.txt";

        #endregion Private properties.

        #region Private fields.

        /// <summary>Control with command-line arguments for test to run.</summary>
        private TextBox _commandLineBox;

        /// <summary>Font with bold style for logging.</summary>
        private Font _fontBold;

        /// <summary>Font with console style for logging.</summary>
        private Font _fontConsole;

        /// <summary>Font with regular style for logging.</summary>
        private Font _fontRegular;

        /// <summary>Control to kill test process.</summary>
        private Button _killTestButton;

        /// <summary>Control for failure analysis notes.</summary>
        private RichTextBox _logBox;

        /// <summary>Control to poll a running test process.</summary>
        private Timer _pollProcessTimer;

        /// <summary>Control to run a test.</summary>
        private Button _runTestButton;

        /// <summary>Control to save the current failure analysis log.</summary>
        private Button _saveLogButton;

        /// <summary>Control to review and select data for test cases.</summary>
        private ListBox _testCaseDataListBox;

        /// <summary>Process being executed.</summary>
        private Process _testProcess;

        /// <summary>ToolTip for the form.</summary>
        private ToolTip _toolTip;

        #endregion Private fields.
    }
}
