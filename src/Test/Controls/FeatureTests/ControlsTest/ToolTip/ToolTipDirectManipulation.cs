using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Documents;
using SWI = System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Display;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel.Actions;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// <description>
    /// Test coverage for direct manipulation of tooltips.
    /// </summary>
    [Test(1, "ToolTip", "ToolTipDirectManipulation")]
    public class ToolTipDirectManipulation : XamlTest
    {
        #region Public Members

        public ToolTipDirectManipulation()
            : base(@"ToolTipDirectManipulation.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Scenario00);
            RunSteps += new TestStep(Scenario01);
            RunSteps += new TestStep(Scenario10);
            RunSteps += new TestStep(Scenario11);
        }

        public TestResult Setup()
        {
            Status("Setup");

            // find elements used by the tests
            _label00 = RootElement.FindName("_label00") as Label;           if (_label00 == null) throw new TestValidationException("Cannot find _label00");
            _label01 = RootElement.FindName("_label01") as Label;           if (_label01 == null) throw new TestValidationException("Cannot find _label01");
            _button10 = RootElement.FindName("_button10") as Button;        if (_button10 == null) throw new TestValidationException("Cannot find _button10");
            _textbox11 = RootElement.FindName("_textbox11") as TextBox;     if (_textbox11 == null) throw new TestValidationException("Cannot find _textbox11");

            // setup scenario 0,0
            _toolTip00 = new ToolTip
            {
                Placement = PlacementMode.MousePoint
            };

            ToolTipService.SetToolTip(_label00, _toolTip00);

            _label00.MouseMove += OnMouseMove00;
            _label00.MouseLeftButtonDown += OnMouseLeftButtonDown00;
            _label00.MouseLeftButtonUp += OnMouseLeftButtonUp00;

            // setup scenario 0,1
            _toolTip01 = new ToolTip
            {
                Placement = PlacementMode.MousePoint,
                Content = "Tooltip for Label01",
            };

            ToolTipService.SetToolTip(_label01, _toolTip01);
            _toolTip01.Opened += OnToolTipOpened01;

            _label01.MouseLeftButtonDown += OnMouseLeftButtonDown01;
            _label01.MouseLeftButtonUp += OnMouseLeftButtonUp01;

            // setup scenario 1,0
            _toolTip10 = (ToolTip)_button10.ToolTip;
            _button10.Click += OnButtonClick10;

            // setup scenario 1,1
            _toolTip11 = new ToolTip
            {
                Content = "This is the tooltip!",
            };
            ToolTipService.SetToolTip(_textbox11, _toolTip11);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            return TestResult.Pass;
        }

        #endregion

        #region Tests

        #region Scenario 0,0

        Label _label00;
        ToolTip _toolTip00;
        DispatcherTimer _toolTipClosingTimer00;

        public TestResult Scenario00()
        {
            UserInput.MouseMove(_label00, 5,5);     // move mouse into the label

            WaitFor(1000);                          // tooltip appears
            Assert.AssertTrue("Tooltip did not open", _toolTip00.IsOpen);

            WaitFor(5000);                          // wait for 5 seconds, tooltip disappears
            Assert.AssertFalse("Tooltip did not close", _toolTip00.IsOpen);

            UserInput.MouseMove(_label00, 10, 10);  // move mouse a short distance
            WaitForPriority(DispatcherPriority.Background);

            UserInput.MouseLeftDown(_label00, 10, 10);  // left-click
            WaitForPriority(DispatcherPriority.Background);
            UserInput.MouseLeftUp(_label00, 10, 10);
            WaitForPriority(DispatcherPriority.Background);

            return TestResult.Pass;                 // expect: no crash
        }

        private void OnMouseMove00(object sender, SWI.MouseEventArgs e)
        {
            if (_label00.IsMouseCaptured)
            {
                return;
            }

            var position = e.GetPosition(_label00);
            var positionText = position.ToString();

            _toolTip00.Content = positionText;

            if (_toolTip00.IsOpen)
            {
                return;
            }

            _toolTip00.IsOpen = true;

            _toolTipClosingTimer00?.Stop();

            _toolTipClosingTimer00 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _toolTipClosingTimer00.Tick += OnTick00;
            _toolTipClosingTimer00.Start();
        }

        private void OnMouseLeftButtonDown00(object sender, SWI.MouseButtonEventArgs e)
        {
            _label00.CaptureMouse();
        }

        private void OnMouseLeftButtonUp00(object sender, SWI.MouseButtonEventArgs e)
        {
            _label00.ReleaseMouseCapture();
        }

        private void OnTick00(object sender, EventArgs e)
        {
            _toolTipClosingTimer00?.Stop();
            _toolTipClosingTimer00 = null;

            if (_toolTip00.IsOpen)
            {
                _toolTip00.IsOpen = false;
            }
        }

        #endregion Scenario 0,0

        #region Scenario 0,1

        Label _label01;
        ToolTip _toolTip01;
        DispatcherTimer _toolTipClosingTimer01;

        public TestResult Scenario01()
        {
            UserInput.MouseMove(_label01, 5,5);     // move mouse into the label

            WaitFor(1500);                          // tooltip appears
            Assert.AssertTrue("Tooltip did not open", _toolTip01.IsOpen);

            WaitFor(5000);                          // wait for 5 seconds, tooltip disappears
            Assert.AssertFalse("Tooltip did not close", _toolTip01.IsOpen);

            UserInput.MouseMove(_label01, 10, 10);  // move mouse a short distance
            WaitForPriority(DispatcherPriority.Background);

            UserInput.MouseLeftDown(_label01, 10, 10);  // left-click
            WaitForPriority(DispatcherPriority.Background);
            UserInput.MouseLeftUp(_label01, 10, 10);
            WaitForPriority(DispatcherPriority.Background);

            return TestResult.Pass;                 // expect: no crash
        }

        private void OnToolTipOpened01(object sender, EventArgs e)
        {
            _toolTipClosingTimer01?.Stop();

            _toolTipClosingTimer01 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _toolTipClosingTimer01.Tick += OnTick01;
            _toolTipClosingTimer01.Start();
        }

        private void OnMouseLeftButtonDown01(object sender, SWI.MouseButtonEventArgs e)
        {
            _label01.CaptureMouse();
        }

        private void OnMouseLeftButtonUp01(object sender, SWI.MouseButtonEventArgs e)
        {
            _label01.ReleaseMouseCapture();
        }

        private void OnTick01(object sender, EventArgs e)
        {
            _toolTipClosingTimer01?.Stop();
            _toolTipClosingTimer01 = null;

            if (_toolTip01.IsOpen)
            {
                _toolTip01.IsOpen = false;
            }
        }

        #endregion Scenario 0,1

        #region Scenario 1,0

        Button _button10;
        ToolTip _toolTip10;
        DispatcherTimer _toggleTimer10;
        int _nOpened, _nClosed;

        public TestResult Scenario10()
        {
            UserInput.MouseLeftClickCenter(_button10);  // left-click the button (starts toggle timer)

            UserInput.MouseMove(_button10, -10, -10);   // move the mouse off the button
            WaitForPriority(DispatcherPriority.Background);

            UserInput.MouseMove(_button10, 10, 10);     // move the mouse back on the button

            WaitFor(1500);                      // tooltip appears
            Assert.AssertTrue("Tooltip did not open", _toolTip10.IsOpen);

            _nOpened = _nClosed = 0;            // count events
            _toolTip10.Opened += CountOpened10;
            _toolTip10.Closed += CountClosed10;

            WaitFor(3000);                      // wait for app to toggle the tooltip
            Assert.AssertTrue("App did not toggle the tooltip", _nOpened == 1 && _nClosed == 1);

            return TestResult.Pass;             // expect: no crash
        }

        void CountOpened10(object sender, EventArgs e)
        {
            ++ _nOpened;
        }

        void CountClosed10(object sender, EventArgs e)
        {
            ++ _nClosed;
        }

        private void OnButtonClick10(object sender, EventArgs e)
        {
            _toggleTimer10?.Stop();

            _toggleTimer10 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _toggleTimer10.Tick += OnTick10;
            _toggleTimer10.Start();
        }

        private void OnTick10(object sender, EventArgs e)
        {
            _toggleTimer10?.Stop();
            _toggleTimer10 = null;

            if (_toolTip10.IsOpen)
            {
                _toolTip10.Closed += OnToolTipClosed10;
                _toolTip10.IsOpen = false;
            }
        }

        private void OnToolTipClosed10(object sender, EventArgs e)
        {
            ToolTip tooltip = sender as ToolTip;
            tooltip.Closed -= OnToolTipClosed10;

            tooltip.Dispatcher.BeginInvoke(new Action(() => tooltip.IsOpen = true));
        }

        #endregion Scenario 1,0

        #region Scenario 1,1

        TextBox _textbox11;
        ToolTip _toolTip11;

        public TestResult Scenario11()
        {
            UserInput.MouseMove(_textbox11, 10, 10);    // move mouse inside the TextBox

            WaitFor(1500);                      // tooltip appears
            Assert.AssertTrue("tooltip did not appear", _toolTip11.IsOpen);

            UserInput.MouseMove(_textbox11, -10, -10);  // move mouse outside the TextBox

            WaitFor(1500);                      // tooltip disappears
            Assert.AssertFalse("tooltip did not disappear", _toolTip11.IsOpen);

            UserInput.MouseMove(_textbox11, 10, 10);    // move mouse inside the TextBox

            WaitFor(1500);                      // tooltip appears
            Assert.AssertTrue("tooltip did not appear", _toolTip11.IsOpen);

            return TestResult.Pass;             // expect: no crash
        }

        #endregion Scenario 1,1


        #endregion
    }
}

