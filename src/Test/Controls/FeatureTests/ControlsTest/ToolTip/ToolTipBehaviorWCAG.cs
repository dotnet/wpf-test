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

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// <description>
    /// Test coverage for the WCAG 2.1 ToolTip behavior.
    /// This includes many individual tests, all collected here so that they can share the common setup.
    /// For debugging purposes, you can run individual tests by supplying command-line arguments:
    ///     /include name1 name2 /exclude name3 name4 ...
    /// specifying a list of the tests you want to include or exclude.  This will run test steps whose name
    /// matches an "included" name but doesn't match a subsequent "excluded" name.  (The first /include is optional.)
    /// </description>
    /// </summary>
    [Test(0, "ToolTip", "ToolTipBehaviorWCAG", Timeout = 240,
        SupportFiles = @"FeatureTests\Controls\Images\smallIcon.png,FeatureTests\Controls\Images\bigIcon.png,FeatureTests\Controls\Images\TooltipsmallIcon.png,FeatureTests\Controls\Images\TooltipbigIcon.png,FeatureTests\Controls\Images\avalon.png")]
    public class ToolTipBehaviorWCAG : XamlTest
    {
        #region Public Members

        public ToolTipBehaviorWCAG()
            : base(@"ToolTipBehaviorWCAG.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            AddRunSteps();
        }

        private void AddRunSteps()
        {
            // alternating include/exclude lists, starting with include.  A null list means "all".
            List<List<string>> nameLists = new List<List<string>>();

            // build the include/exclude lists
            bool including = true;
            List<string> list = new List<string>();
            string[] args = Environment.GetCommandLineArgs();
            for (int i=1, N=args.Length; i<N; ++i)
            {
                string arg = args[i].ToLower();
                if (arg.StartsWith('/') || arg.StartsWith('-'))
                {
                    switch (arg.Substring(1))
                    {
                        case "include":
                            if (!including)
                            {
                                nameLists.Add(list);
                                list = new List<string>();
                                including = true;
                            }
                            break;
                        case "exclude":
                            if (including)
                            {
                                if (list.Count == 0 && nameLists.Count == 0)
                                {
                                    list = null;
                                }
                                nameLists.Add(list);
                                list = new List<string>();
                                including = false;
                            }
                            break;
                        default:
                            throw new ArgumentException("Unrecognized argument.", arg);
                    }
                }
                else
                {
                    list.Add(arg);
                }
            }

            if (list.Count > 0)
            {
                nameLists.Add(list);
            }

            // add the tests
            AddRunStep(new TestStep(CheckReset));
            AddRunStep(new TestStep(MouseOverOpensToolTip), nameLists);
            AddRunStep(new TestStep(MouseAwayClosesToolTip), nameLists);
            AddRunStep(new TestStep(MouseHoverKeepsToolTipOpen), nameLists);
            AddRunStep(new TestStep(MouseMoveRespectsBetweenShowDelay), nameLists);
            AddRunStep(new TestStep(KeyNavOpensToolTip), nameLists);
            AddRunStep(new TestStep(KeyNavClosesToolTip), nameLists);
            AddRunStep(new TestStep(KeyNavRespectsBetweenShowDelay), nameLists);
            AddRunStep(new TestStep(CtrlClosesToolTip), nameLists);
            AddRunStep(new TestStep(CtrlShiftF10TogglesToolTip), nameLists);
            AddRunStep(new TestStep(KeyNavOptOut), nameLists);
            AddRunStep(new TestStep(SafeArea), nameLists);
            AddRunStep(new TestStep(SafeAreaOnHyperlink), nameLists);
            AddRunStep(new TestStep(RibbonMouseOpensToolTip), nameLists);
            AddRunStep(new TestStep(RibbonKeyNavOpensToolTip), nameLists);
            
            // bugfixes from 4.8
            AddRunStep(new TestStep(FocusDoesNotOpenToolTip), nameLists);
            AddRunStep(new TestStep(KeyNavDoesNotOpenAnotherToolTip), nameLists);
            AddRunStep(new TestStep(MouseMoveDoesNotCloseKBToolTip), nameLists);
            AddRunStep(new TestStep(BetweenShowDelayZero), nameLists);
       }

        private void AddRunStep(TestStep step, List<List<string>> nameLists=null)
        {
            bool add;
            if (nameLists == null || nameLists.Count == 0)
            {
                add = true;
            }
            else
            {
                add = false;
                bool including = true;
                foreach (List<string> list in nameLists)
                {
                    if (list == null)
                    {
                        add = including;
                    }
                    else
                    {
                        string stepName = step.GetMethodInfo().Name.ToLower();
                        foreach (string name in list)
                        {
                            if (stepName.IndexOf(name) >= 0)
                            {
                                add = including;
                                break;
                            }
                        }
                    }
                    including = !including;
                }
            }

            if (add)
            {
                RunSteps += step;
            }
        }

        public TestResult Setup()
        {
            Status("Setup");

            RootElement.DataContext = new Model(20);

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // prepare the ToolTipTracker
            _tracker.Initialize(RootElement);

            // set the InitialShowDelay to a small value, just to speed up the tests
            foreach (DependencyObject owner in _tracker.ToolTipOwners)
            {
                ValueSource vs = DependencyPropertyHelper.GetValueSource(owner, ToolTipService.InitialShowDelayProperty);
                if (vs.BaseValueSource == BaseValueSource.Default)
                {
                    owner.SetValue(ToolTipService.InitialShowDelayProperty, 10);
                }
            }

            // find elements used by the tests
            _ribbon = RootElement.FindName("_ribbon") as Ribbon;                            if (_ribbon == null) throw new TestValidationException("Cannot find _ribbon");
            _reset = RootElement.FindName("_reset") as FrameworkElement;                    if (_reset == null) throw new TestValidationException("Cannot find _reset");
            _listbox = RootElement.FindName("_listbox") as ListBox;                         if (_listbox == null) throw new TestValidationException("Cannot find _listbox");
            _button = RootElement.FindName("_button") as Button;                            if (_button == null) throw new TestValidationException("Cannot find _button");
            _textbox = RootElement.FindName("_textbox") as TextBox;                         if (_textbox == null) throw new TestValidationException("Cannot find _textbox");
            _image = RootElement.FindName("_image") as Image;                               if (_image == null) throw new TestValidationException("Cannot find _image");
            _focusableImage = RootElement.FindName("_focusableImage") as Image;             if (_focusableImage == null) throw new TestValidationException("Cannot find _focusableImage");
            _groupbox = RootElement.FindName("_groupbox") as GroupBox;                      if (_groupbox == null) throw new TestValidationException("Cannot find _groupbox");
            _hyperlinkTB = RootElement.FindName("_hyperlinkTB") as Hyperlink;               if (_hyperlinkTB == null) throw new TestValidationException("Cannot find _hyperlinkTB");
            _textBoxNoTT = RootElement.FindName("_textBoxNoTT") as TextBox;                 if (_textBoxNoTT == null) throw new TestValidationException("Cannot find _textBoxNoTT");
            _textBoxBSD0 = RootElement.FindName("_textBoxBSD0") as TextBox;                 if (_textBoxBSD0 == null) throw new TestValidationException("Cannot find _textBoxBSD0");
            _textBoxTT = RootElement.FindName("_textBoxTT") as TextBox;                     if (_textBoxTT == null) throw new TestValidationException("Cannot find _textBoxTT");
            _textBoxISD = RootElement.FindName("_textBoxISD") as TextBox;                   if (_textBoxISD == null) throw new TestValidationException("Cannot find _textBoxISD");
            _rectangle = RootElement.FindName("_rectangle") as FrameworkElement;            if (_rectangle == null) throw new TestValidationException("Cannot find _rectangle");
            _flowdocSV = RootElement.FindName("_flowdocSV") as FlowDocumentScrollViewer;    if (_flowdocSV == null) throw new TestValidationException("Cannot find _flowdocSV");
            _hyperlinkFD = RootElement.FindName("_hyperlinkFD") as Hyperlink;               if (_hyperlinkFD == null) throw new TestValidationException("Cannot find _hyperlinkFD");

            // find IContentHost for _hyperlinkTB
            TextBlock _textblockHL = RootElement.FindName("_textblockHL") as TextBlock;
            _textblockICH = _textblockHL as IContentHost;                                   if (_textblockICH == null) throw new TestValidationException("Cannot find IContentHost for TextBlock");

            // find IContentHost for _hyperlinkFD
            ScrollViewer sv = _flowdocSV.Template.FindName("PART_ContentHost", _flowdocSV) as ScrollViewer;
            DependencyObject fdView = sv?.Content as DependencyObject;
            if (fdView != null && VisualTreeHelper.GetChildrenCount(fdView) > 0)
            {
                _fdsvICH = VisualTreeHelper.GetChild(fdView, 0) as IContentHost;
            }
            if (_fdsvICH == null) throw new TestValidationException("Cannot find IContentHost for FlowDocumentScrollViewer");

            // get the opt-out DP via reflection (so that the test can run on older versions of WPF)
            Type toolTipServiceType = typeof(ToolTipService);
            FieldInfo fi = toolTipServiceType.GetField("ShowsToolTipOnKeyboardFocusProperty", BindingFlags.Static | BindingFlags.NonPublic);
            ShowsToolTipOnKeyboardFocusProperty = (DependencyProperty)fi?.GetValue(null);

            // collapse the ribbon to start with
            _ribbon.Visibility = Visibility.Collapsed;
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            if (_errors.Count == 0)
                return TestResult.Pass;

            foreach (string error in _errors)
            {
                LogComment(error);
            }
            return TestResult.Fail;
        }

        #endregion

        #region Tests

        // make sure the reset element works
        public TestResult CheckReset()
        {
            _tracker.BeginLogging();

            MoveMouseOver(_reset);
            _tracker.LogEvent(_reset, "MouseOver _reset");
            _tracker.WaitForToolTipClosed(_reset, 0, checkCurrent: false);

            var resetLog = _tracker.LogFor(_reset);
            if (VerifyExpectedEvents(_reset, resetLog, 
                                     new object[] { 0,"MouseOver", 0,TTOpening, 0,TTOpened, 0,TTClosing, 0,TTClosed }))
            {
                // if this test fails, there's no sense continuing.  Other tests rely on
                // the reset element working correctly
                return TestResult.Fail;
            }

            // there is some overhead for input simulation, dispatching events, etc.
            // We should ignore it when checking the lag between input and response.
            // It's not constant (or even deterministic), but as a best guess use the
            // lag observed in this test.
            _inputOH = (resetLog[2].Timestamp - resetLog[1].Timestamp).TotalMilliseconds - ToolTipService.GetInitialShowDelay(_reset);
            _shortWait = (int)_inputOH + Tolerance;

            return TestResult.Pass;
        }

        // moving the mouse over an element opens its tooltip, after the InitialShowDelay
        public TestResult MouseOverOpensToolTip()
        {
            MouseOverOpensToolTipCommon(_button);       // UIElement
            MouseOverOpensToolTipCommon(_hyperlinkTB);  // Hyperlink in TextBlock
            MouseOverOpensToolTipCommon(_hyperlinkFD);  // Hyperlink in FlowDocument
            return TestResult.Pass;
        }

        void MouseOverOpensToolTipCommon(DependencyObject d)
        {
            int delay = ToolTipService.GetInitialShowDelay(d);

            Reset();
            MoveMouseOver(d);
            _tracker.LogEvent(d, "MouseOver");
            _tracker.WaitForToolTipOpened(d, delay + Tolerance);
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                    new object[] {0,"MouseOver", delay,TTOpening, 0,TTOpened});
        }

        // moving the mouse away from an element closes its tooltip
        public TestResult MouseAwayClosesToolTip()
        {
            MouseAwayClosesToolTipCommon(_button);       // UIElement
            MouseAwayClosesToolTipCommon(_hyperlinkTB);  // Hyperlink in TextBlock
            MouseAwayClosesToolTipCommon(_hyperlinkFD);  // Hyperlink in FlowDocument
            return TestResult.Pass;
        }

        void MouseAwayClosesToolTipCommon(DependencyObject d)
        {
            int delay = ToolTipService.GetInitialShowDelay(d);

            Reset();
            MoveMouseOver(d);
            _tracker.LogEvent(d, "MouseOver");
            _tracker.WaitForToolTipOpened(d, delay + Tolerance);

            MoveMouseOver(d, 5, -5);
            _tracker.LogEvent(d, "MouseAway");
            _tracker.WaitForToolTipClosed(d, 0);
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                    new object[] {0,"MouseOver", delay,TTOpening, 0,TTOpened, 0,"MouseAway", 0,TTClosing, 0,TTClosed});
        }

        // tooltip does not close due to timeout
        public TestResult MouseHoverKeepsToolTipOpen()
        {
            Reset();
            MoveMouseOver(_image);
            _tracker.LogEvent(_image, "MouseOver");
            _tracker.WaitForToolTipOpened(_image, 0);

            _tracker.LogEvent(_image, "Hover");
            _tracker.WaitForToolTipClosed(_image, 6000);    // 6 seconds approximates "forever"
            VerifyExpectedEvents(_image, _tracker.LogFor(_image),
                    new object[] {0,"MouseOver", 0,TTOpening, 0,TTOpened, 0,"Hover"});

            return TestResult.Pass;
        }

        // opening a new tooltip soon after an old one closes bypasses the show-delay
        public TestResult MouseMoveRespectsBetweenShowDelay()
        {
            Reset();
            MoveMouseOver(_textbox);
            _tracker.LogEvent(_textbox, "MouseOver");
            _tracker.WaitForToolTipOpened(_textbox, 0);

            MoveMouseOver(_textBoxISD);
            _tracker.LogEvent(_textBoxISD, "MouseOver");
            _tracker.WaitForToolTipOpened(_textBoxISD, _shortWait);      // shorter than InitialShowDelay
            VerifyExpectedEvents(_textBoxISD, _tracker.LogFor(_textBoxISD),
                    new object[] {0,"MouseOver", 0,TTOpening, 0,TTOpened});

            return TestResult.Pass;
        }
        
        // opening a new tooltip soon after an old one closes does apply the show-delay
        // when the old tooltip's BetweenShowDelay is 0
        public TestResult BetweenShowDelayZero()
        {
            Reset();
            MoveMouseOver(_textBoxBSD0);
            _tracker.LogEvent(_textBoxBSD0, "MouseOver");
            _tracker.WaitForToolTipOpened(_textBoxBSD0, 0);

            MoveMouseOver(_textBoxISD);
            _tracker.LogEvent(_textBoxISD, "MouseOver");
            _tracker.WaitForToolTipOpened(_textBoxISD, _shortWait);      // shorter than InitialShowDelay
            VerifyExpectedEvents(_textBoxISD, _tracker.LogFor(_textBoxISD),
                    new object[] {0,"MouseOver"});

            _tracker.WaitForToolTipOpened(_textBoxISD, 3000);
            VerifyExpectedEvents(_textBoxISD, _tracker.LogFor(_textBoxISD),
                    new object[] {0,"MouseOver", 0,TTOpening, 0,TTOpened});

            return TestResult.Pass;
        }

        // keyboard navigation to an element opens its tooltip
        public TestResult KeyNavOpensToolTip()
        {
            KeyNavOpensToolTipCommon(_focusableImage);  // UIElement
            KeyNavOpensToolTipCommon(_hyperlinkTB);     // Hyperlink in TextBlock
            KeyNavOpensToolTipCommon(_hyperlinkFD);     // Hyperlink in FlowDocument
            return TestResult.Pass;
        }

        void KeyNavOpensToolTipCommon(DependencyObject d)
        {
            Reset(d);
            _tracker.LogEvent(d, "TabTo");
            TabStep();
            _tracker.WaitForToolTipOpened(d, 0);
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                    new object[] {0,"TabTo", 0,KBGotFocus, 0,TTOpening, 0,TTOpened});
        }

        // losing focus (for any reason) closes an element's tooltip
        public TestResult KeyNavClosesToolTip()
        {
            KeyNavClosesToolTipCommon(_focusableImage); // UIElement
            KeyNavClosesToolTipCommon(_hyperlinkTB);    // Hyperlink in TextBlock
            KeyNavClosesToolTipCommon(_hyperlinkFD);    // Hyperlink in FlowDocument
            return TestResult.Pass;
        }

        void KeyNavClosesToolTipCommon(DependencyObject d)
        {
            Reset(d);
            _tracker.LogEvent(d, "TabTo");
            TabStep();
            _tracker.WaitForToolTipOpened(d, 0);

            _tracker.LogEvent(d, "LostFocus");
            _reset.Focus();
            _tracker.WaitForToolTipClosed(d, 0);
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                    new object[] {0,"TabTo", 0,KBGotFocus, 0,TTOpening, 0,TTOpened, 0,"LostFocus", 0,KBLostFocus, 0,TTClosing, 0,TTClosed});
        }

        // opening a new tooltip (via keyboard) soon after an old one closes bypasses the show-delay
        public TestResult KeyNavRespectsBetweenShowDelay()
        {
            Reset(_textBoxTT);

            _tracker.LogEvent(_textBoxTT, "Tab to _textBoxTT");
            TabStep();
            _tracker.WaitForToolTipOpened(_textBoxTT, 0);

            _tracker.LogEvent(_textBoxISD, "Tab to _textBoxISD");
            TabStep();
            _tracker.WaitForToolTipOpened(_textBoxISD, _shortWait);      // shorter than InitialShowDelay
            VerifyExpectedEvents(_textBoxISD, _tracker.LogFor(_textBoxISD),
                    new object[] {0,"Tab", 0,KBGotFocus, 0,TTOpening, 0,TTOpened});

            return TestResult.Pass;
        }

        // typing Ctrl closes a tooltip opened from the keyboard
        public TestResult CtrlClosesToolTip()
        {
            DependencyObject d = _focusableImage;
            Reset(d);
            _tracker.LogEvent(d, "TabTo");
            TabStep();
            _tracker.WaitForToolTipOpened(d, 0);

            // type variants, to verify they don't close the tooltip
            _tracker.LogEvent(d, "Typing");
            Type(Ctrl, Shift);
            Type(Shift, Ctrl);
            Type(LeftCtrl, RightCtrl);
            Type(Alt);
            Type(Shift);
            _tracker.WaitForToolTipClosed(d, _shortWait);

            // now type Ctrl
            _tracker.LogEvent(d, "Ctrl");
            Type(Ctrl);
            _tracker.WaitForToolTipClosed(d, _shortWait);
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                new object[] {0,"TabTo", 0,KBGotFocus, 0,TTOpening, 0,TTOpened, 0,"Typing", 0,"Ctrl", 0,TTClosing, 0,TTClosed});

            return TestResult.Pass;
        }

        // Ctrl+Shift+F10 opens or closes the tooltip immediately
        public TestResult CtrlShiftF10TogglesToolTip()
        {
            // to test this fairly, use an element where
            //  a) no TextEditor (TextEditor closes tooltip on any keystroke)
            //  b) large ISD (to check that open happens immediately)
            DependencyObject d = _focusableImage;
            int oldISD = ToolTipService.GetInitialShowDelay(d);
            ToolTipService.SetInitialShowDelay(d, 2000);

            Reset(d);

            // open the tooltip
            _tracker.LogEvent(d, "Tab to _focusableImage");
            TabStep();
            _tracker.WaitForToolTipOpened(d, 0);

            // Ctrl+Shift+F10 closes it (immediately)
            _tracker.LogEvent(d, "CtrlShiftF10 - Close");
            Type(Ctrl, Shift, F10);
            _tracker.WaitForToolTipClosed(d, _shortWait);
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                new object[] {0,"Tab", 0,KBGotFocus, 0,TTOpening, 0,TTOpened,
                    0,"CtrlShiftF10", 0,TTClosing, 0,TTClosed});

            // Ctrl+Shift+F10 opens it (immediately)
            _tracker.LogEvent(d, "CtrlShiftF10 - Open");
            Type(Shift, Ctrl, F10);
            _tracker.WaitForToolTipOpened(d, _shortWait);
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                new object[] {0,"Tab", 0,KBGotFocus, 0,TTOpening, 0,TTOpened,
                    0,"CtrlShiftF10", 0,TTClosing, 0,TTClosed,
                    0,"CtrlShiftF10", 0,TTOpening, 0,TTOpened});

            // Ctrl+Shift+F10 closes it
            _tracker.LogEvent(d, "CtrlShiftF10 - Close");
            Type(Ctrl, Shift, F10);
            _tracker.WaitForToolTipClosed(d, _shortWait);
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                new object[] {0,"Tab", 0,KBGotFocus, 0,TTOpening, 0,TTOpened,
                    0,"CtrlShiftF10", 0,TTClosing, 0,TTClosed,
                    0,"CtrlShiftF10", 0,TTOpening, 0,TTOpened,
                    0,"CtrlShiftF10", 0,TTClosing, 0,TTClosed});

            // other combinations do nothing
            _tracker.LogEvent(d, "CtrlShiftF10Variants - Noop");
            Type(Ctrl, Shift, Alt, F10);
            Type(Alt, Ctrl, Shift, F10);
            _tracker.WaitForToolTipClosed(d, _shortWait);
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                new object[] {0,"Tab", 0,KBGotFocus, 0,TTOpening, 0,TTOpened,
                    0,"CtrlShiftF10", 0,TTClosing, 0,TTClosed,
                    0,"CtrlShiftF10", 0,TTOpening, 0,TTOpened,
                    0,"CtrlShiftF10", 0,TTClosing, 0,TTClosed,
                    0,"CtrlShiftF10Variants"});

            ToolTipService.SetInitialShowDelay(d, oldISD);
            return TestResult.Pass;
        }

        // the opt-out properties work as spec'd.  Default is to show the toooltip.
        public TestResult KeyNavOptOut()
        {
            if (ShowsToolTipOnKeyboardFocusProperty != null)
            {
                DependencyObject d = _focusableImage;
                ToolTip tooltip = d.GetValue(ToolTipService.ToolTipProperty) as ToolTip;

                KeyNavOptOutCommon(d, tooltip, false, false, false);
                KeyNavOptOutCommon(d, tooltip, false, true,  false);
                KeyNavOptOutCommon(d, tooltip, false, null,  false);
                KeyNavOptOutCommon(d, tooltip, true,  false, true);
                KeyNavOptOutCommon(d, tooltip, true,  true,  true);
                KeyNavOptOutCommon(d, tooltip, true,  null,  true);
                KeyNavOptOutCommon(d, tooltip, null,  false, false);
                KeyNavOptOutCommon(d, tooltip, null,  true,  true);
                KeyNavOptOutCommon(d, tooltip, null,  null,  true);
            }
            return TestResult.Pass;
        }

        private void KeyNavOptOutCommon(DependencyObject d, ToolTip tooltip, bool? ownerValue, bool? tooltipValue, bool showsToolTip)
        {
            // configure the owner and its tooltip
            d.SetValue(ShowsToolTipOnKeyboardFocusProperty, ownerValue);
            tooltip.SetValue(ShowsToolTipOnKeyboardFocusProperty, tooltipValue);
            string label = String.Format("Tab {0} {1} -> {2}", ownerValue, tooltipValue, showsToolTip);

            //  tab onto the target DO
            Reset();
            ((UIElement)d).Focus();
            TabStep(backward:true);
            _tracker.BeginLogging();
            TabStep();
            _tracker.LogEvent(d, label);
            _tracker.WaitForToolTipOpened(d, 1000);

            // verify the result
            object[] expected;
            if (showsToolTip)   expected = new object[] {0,"Tab", 0,TTOpening, 0,TTOpened};
            else                expected = new object[] {0,"Tab"};
            VerifyExpectedEvents(d, _tracker.LogFor(d), expected);
        }

        // moving the mouse in a straight line from the owner to the tooltip doesn't close the tooltip
        public TestResult SafeArea()
        {
            List<Vector> ownerPoints = new List<Vector>();
            System.Drawing.Rectangle ownerRect = Microsoft.Test.RenderingVerification.ImageUtility.GetScreenBoundingRectangle(_rectangle);
            AddPoints(ownerPoints, ownerRect);

            SafeAreaCommon(_rectangle, ownerPoints);
            return TestResult.Pass;
        }

        // moving the mouse in a straight line from the owner to the tooltip doesn't close the tooltip
        public TestResult SafeAreaOnHyperlink()
        {
            List<Vector> ownerPoints = new List<Vector>();
            FrameworkElement fe;
            IReadOnlyCollection<Rect> rects = GetRectangles(_hyperlinkFD, out fe);

            if (fe != null && rects != null)
            {
                System.Drawing.Rectangle parentRC = Microsoft.Test.RenderingVerification.ImageUtility.GetScreenBoundingRectangle(fe);
                foreach (Rect rect in rects)
                {
                    System.Drawing.Rectangle sdRect = new System.Drawing.Rectangle(
                        parentRC.Left + (int)Monitor.ConvertLogicalToScreen(Dimension.Width, rect.Left),
                        parentRC.Top + (int)Monitor.ConvertLogicalToScreen(Dimension.Height, rect.Top),
                        (int)Monitor.ConvertLogicalToScreen(Dimension.Width, rect.Width),
                        (int)Monitor.ConvertLogicalToScreen(Dimension.Height, rect.Height));

                    AddPoints(ownerPoints, sdRect);
                }
            }

            // moving the mouse between the hyperlink and its tooltip will cross over 
            // some nearby elements.  If they show their own tooltips, that will ruin
            // our test.  To prevent this, bump their ISD.
            int fdsvISD = ToolTipService.GetInitialShowDelay(_flowdocSV);
            int gbISD = ToolTipService.GetInitialShowDelay(_groupbox);
            ToolTipService.SetInitialShowDelay(_flowdocSV, 5000);
            ToolTipService.SetInitialShowDelay(_groupbox, 5000);

            SafeAreaCommon(_hyperlinkFD, ownerPoints);

            ToolTipService.SetInitialShowDelay(_flowdocSV, fdsvISD);
            ToolTipService.SetInitialShowDelay(_groupbox, gbISD);
            return TestResult.Pass;
        }

        private void AddPoints(List<Vector> points, in System.Drawing.Rectangle rect)
        {
            points.Add(new Vector(rect.Left+5, rect.Top+5));
            points.Add(new Vector(rect.Left+5, rect.Bottom-5));
            points.Add(new Vector(rect.Right-5, rect.Top+5));
            points.Add(new Vector(rect.Right-5, rect.Bottom-5));
        }

        private void SafeAreaCommon(DependencyObject d, List<Vector> ownerPoints)
        {
            SafeAreaPlacement(d, ownerPoints, PlacementMode.Bottom, -20, 20);
            SafeAreaPlacement(d, ownerPoints, PlacementMode.Top, -20, -20);
            SafeAreaPlacement(d, ownerPoints, PlacementMode.Left, -20, -20);
            SafeAreaPlacement(d, ownerPoints, PlacementMode.Right, 20, -20);
        }

        private void SafeAreaPlacement(DependencyObject d, List<Vector> ownerPoints, PlacementMode placement, double hoffset, double voffset)
        {
            ToolTipService.SetPlacement(d, placement);
            ToolTipService.SetHorizontalOffset(d, hoffset);
            ToolTipService.SetVerticalOffset(d, voffset);

            // open the tooltip (via mouse)
            Reset();
            _tracker.LogEvent(d, "OpenTooltip");
            MoveMouseOver(d);
            _tracker.WaitForToolTipOpened(d, 1000);

            // move the mouse through the safe area
            List<Vector> tooltipPoints = GetToolTipPoints(_tracker.CurrentToolTip);
            foreach (Vector ownerPt in ownerPoints)
            {
                foreach (Vector tooltipPt in tooltipPoints)
                {
                    // move the mouse in a straight line from the owner to the tooltip
                    double distance = (tooltipPt - ownerPt).Length;
                    int steps = (int)Math.Ceiling(distance / 5.0);      // each step is roughly 5 pixels
                    for (int i=0; i<= steps; ++i)
                    {
                        double t = ((double)i)/steps;
                        Vector v = tooltipPt * t + ownerPt * (1.0 - t);
                        Input.MoveTo((Point)v);
                        WaitForPriority(DispatcherPriority.ApplicationIdle);
                    }
                }
            }

            // verify that the tooltip stayed open
            VerifyExpectedEvents(d, _tracker.LogFor(d),
                    new object[] { 0,"OpenTooltip", 0,TTOpening, 0,TTOpened});

            // restore everything we changed
            d.ClearValue(ToolTipService.PlacementProperty);
            d.ClearValue(ToolTipService.HorizontalOffsetProperty);
            d.ClearValue(ToolTipService.VerticalOffsetProperty);
        }

        private List<Vector> GetToolTipPoints(ToolTip tooltip)
        {
            List<Vector> points = new List<Vector>();
            System.Drawing.Rectangle tooltipRectangle = Microsoft.Test.RenderingVerification.ImageUtility.GetScreenBoundingRectangle(tooltip);
            AddPoints(points, tooltipRectangle);
            return points;
        }

        // mouse opens tooltips in the Ribbon
        public TestResult RibbonMouseOpensToolTip()
        {
            _ribbon.Visibility = Visibility.Visible;
            Reset();

            object[] expectedEvents = new object[] {0,TTOpening, 0,TTOpened};
            foreach (DependencyObject d in _tracker.ToolTipOwners)
            {
                UIElement uie = d as UIElement;
                if (uie != null && _ribbon.IsAncestorOf(uie) && uie.IsVisible && uie.IsEnabled)
                {
                    MoveMouseOver(d);
                    _tracker.WaitForToolTipOpened(d, 0);
                    VerifyExpectedEvents(d, _tracker.LogFor(d), expectedEvents);
                }
            }

            _ribbon.Visibility = Visibility.Collapsed;
            return TestResult.Pass;
        }

        // keyboard opens tooltips in the Ribbon
        public TestResult RibbonKeyNavOpensToolTip()
        {
            _ribbon.Visibility = Visibility.Visible;
            Reset(_ribbon);
            TabStep();      // so that first iteration of the loop tabs to something within Ribbon

            IInputElement initialFocus = null;
            object[] expectedEvents = new object[] {0,KBGotFocus, 0,TTOpening, 0,TTOpened};
            for (IInputElement ie = TabStep(); ie != initialFocus; ie = TabStep())
            {
                if (initialFocus == null)
                    initialFocus = ie;

                FrameworkElement fe = ie as FrameworkElement;
                if (fe != null && _ribbon.IsAncestorOf(fe) && fe.ToolTip != null)
                {
                    _tracker.WaitForToolTipOpened(fe, 0);
                    VerifyExpectedEvents(fe, _tracker.LogFor(fe), expectedEvents);
                }
            }

            _ribbon.Visibility = Visibility.Collapsed;
            return TestResult.Pass;
        }

        // Setting focus in ways other than keyboard navigation does not show the tooltip
        public TestResult FocusDoesNotOpenToolTip()
        {
            Reset();
            MoveMouseOver(_textBoxNoTT);    // ensure last input is from mouse
            _tracker.LogEvent(_textbox, "Focus");
            _textbox.Focus();               // one way to get focus, representative of many others
            _tracker.WaitForToolTipOpened(_textbox, 1000);
            VerifyExpectedEvents(_textbox, _tracker.LogFor(_textbox),
                    new object[] {0,"Focus", 0,KBGotFocus});

            return TestResult.Pass;
        }

        // moving focus to an element doesn't open a different element's tooltip
        public TestResult KeyNavDoesNotOpenAnotherToolTip()
        {
            Reset(_textBoxNoTT);
            _tracker.LogEvent(_textBoxNoTT, "TabTo _textBoxNoTT");
            TabStep();
            _tracker.WaitForToolTipOpened(_textBoxNoTT, 1000);
            VerifyExpectedEvents(_textBoxNoTT, _tracker.LogFor(_textBoxNoTT),
                    new object[] {0,"TabTo", 0,KBGotFocus});
            VerifyExpectedEvents(_groupbox, _tracker.LogFor(_groupbox),
                    new object[] {});

            return TestResult.Pass;
        }

        // moving the mouse doesn't close a tooltip opened by keyboard
        public TestResult MouseMoveDoesNotCloseKBToolTip()
        {
            Reset(_focusableImage);
            MoveMouseOver(_textbox);
            _tracker.LogEvent(_focusableImage, "TabTo _focusableImage");
            TabStep();
            _tracker.WaitForToolTipOpened(_focusableImage, 0);

            MoveMouseOver(_textbox, 5, -5);
            _tracker.LogEvent(_focusableImage, "MouseAway _textbox");
            _tracker.WaitForToolTipClosed(_focusableImage, 5000);
            VerifyExpectedEvents(_focusableImage, _tracker.LogFor(_focusableImage),
                    new object[] {0,"TabTo", 0,KBGotFocus, 0,TTOpening, 0,TTOpened, 0,"MouseAway"});

            return TestResult.Pass;
        }

        #endregion

        #region Private Members

        // return true if actual and expected don't match.
        // 'actual' is extracted from the ToolTipTracker's log, reporting what events were actually 
        // seen and when they happened.
        // 'expected' is an array with 2n entries describing n events, in the format
        // { d1, s1,  d2, s2,  d3, s3, ... } where the d's give the expected time since
        // the previous event, and the s's give the name of the event (or a substring thereof).
        // The d's are further encoded as follows:
        //      > 0     delta (in ms) from previous event, ignoring the input overhead
        //        0     free pass - time is ignored
        //      < 0     -delta (in ms) from previous event.  No input overhead.
        private bool VerifyExpectedEvents(DependencyObject d, 
                                          List<ToolTipTracker.LogEntry> actual, 
                                          object[] expected, 
                                          [CallerMemberName] string caller = null)
        {
            StringBuilder builder = new StringBuilder();
            int actualIndex = 1, actualCount = actual.Count, expectedIndex = 0, expectedCount = expected.Length;

            // match actual to expected
            for (; actualIndex < actualCount && expectedIndex < expectedCount; ++actualIndex, expectedIndex+=2)
            {
                ToolTipTracker.LogEntry entry = actual[actualIndex], previousEntry = actual[actualIndex-1];
                double actualDelta = (entry.Timestamp - previousEntry.Timestamp).TotalMilliseconds;
                string actualEvent = entry.Description;
                int expectedDelta = (int)expected[expectedIndex];
                string expectedEvent = (string)expected[expectedIndex + 1];

                // the sign of expectedDelta indicates whether to ignore input overhead
                if (expectedDelta > 0)
                {
                    actualDelta = Math.Max(actualDelta - _inputOH, 0.0);
                }
                else
                {
                    expectedDelta = -expectedDelta;
                }

                if (actualEvent.IndexOf(expectedEvent, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    builder.Append(" " + actualIndex + " Mismatched event. ");
                    builder.Append(" Expected: ");
                    builder.Append(expectedEvent);
                    builder.Append("  Actual: ");
                    builder.AppendLine(actualEvent);
                }
                else if (expectedDelta > 0 && Math.Abs(actualDelta - expectedDelta) > Tolerance)
                {
                    builder.Append(" " + actualIndex + " Mistimed event " + actualEvent + ". ");
                    builder.Append(" Expected: ");
                    builder.Append(expectedDelta);
                    builder.Append(" Actual: ");
                    builder.Append(actualDelta);
                    builder.AppendLine();
                }
            }

            // extra events - in actual but not expected
            for (; actualIndex < actualCount; ++actualIndex)
            {
                var entry = actual[actualIndex];
                builder.Append(" " + actualIndex + " Unexpected event ");
                builder.Append(entry.Description);
                builder.Append(" at time ");
                builder.Append((int)Math.Round(entry.Timestamp.TotalMilliseconds));
                builder.AppendLine();
            }

            // missing events - in expected but not actual
            for (; expectedIndex < expectedCount; expectedIndex+=2)
            {
                builder.Append(" " + (int)(expectedIndex/2 + 1) + " Missing event ");
                builder.AppendLine((string)expected[expectedIndex + 1]);
            }

            bool foundErrors = (builder.Length > 0);
            if (foundErrors)
            {
                string name = (string)d.GetValue(FrameworkElement.NameProperty);
                if (String.IsNullOrEmpty(name))
                    name = String.Format("{0}", d);

                string error = String.Format("Errors from {0} for {1}:\n{2}", caller, name, builder.ToString());
                _errors.Add(error);
                LogComment(error);
            }

            return foundErrors;
        }

        // reset to a known initial state
        private void Reset(DependencyObject d=null)
        {
            // reset keyboard focus
            IInputElement ie = d as IInputElement;
            if (ie != null)
            {
                PrepareTab(ie);
            }
            else
            {
                _reset.Focus();
            }

            // reset mouse position and clear tooltips
            if (_reset.IsMouseDirectlyOver)
            {
                UserInput.MouseMove(_reset, 5, -5);
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }
            UserInput.MouseMove(_reset, 5, 5);
            _tracker.WaitForToolTipClosed(_reset, 0, checkCurrent: false);

            _tracker.BeginLogging();
        }

        private void MoveMouseOver(DependencyObject d, int x=5, int y=5)
        {
            FrameworkElement fe;
            ContentElement ce;

            if ((fe = d as FrameworkElement) != null)
            {
                UserInput.MouseMove(fe, x, y);
            }
            else if ((ce = d as ContentElement) != null)
            {
                IReadOnlyCollection<Rect> rects = GetRectangles(ce, out fe);
                if (rects != null && fe != null)
                {
                    foreach (Rect rc in rects)
                    {
                        UserInput.MouseMove(fe, (int)(rc.Left + x), (int)(rc.Top + y));
                        break;  // only need the first rect
                    }
                }
            }
        }

        private IReadOnlyCollection<Rect> GetRectangles(ContentElement ce, out FrameworkElement fe)
        {
            fe = null;
            IReadOnlyCollection<Rect> rects = null;
            IContentHost ich = GetContentHost(ce);

            if (ich != null)
            {
                rects = ich.GetRectangles(ce);

                // need an FE for the call to MouseMove, so find first FE ancestor of ich
                // (this assumes ich and the FE have the same coordinate system;
                // that's true for this test, but not in general)
                DependencyObject v = ich as DependencyObject;
                for (fe = v as FrameworkElement; 
                    fe == null && v != null; 
                    v = VisualTreeHelper.GetParent(v), fe = v as FrameworkElement)
                { /* empty */ }
            }

            return rects;
        }

        private void Type(params string[] keys)
        {
            int N = keys.Length;
            for (int i=0; i<N; ++i)
            {
                UserInput.KeyDown(keys[i]);
            }
            for (int i=N-1; i>=0; --i)
            {
                UserInput.KeyUp(keys[i]);
            }
        }
        
        // Move focus to the tab stop just before the given element,
        // so that a subsequent TAB moves focus onto the element.
        private void PrepareTab(IInputElement element)
        {
            if (element.Focusable)
            {
                element.Focus();
                TabStep(backward: true);
            }
        }

        // tab until focus reaches a given element (or we give up trying)
        private bool TabTo(DependencyObject d, IInputElement initialFocus=null)
        {
            if (initialFocus == null)
                initialFocus = _reset as IInputElement;

            if ((d as IInputElement).Focusable)
            {
                SWI.Keyboard.Focus(initialFocus);
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                for (int maxTabs = 100;  maxTabs > 0; --maxTabs)
                {
                    UserInput.KeyPress(Tab);
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    if (SWI.Keyboard.FocusedElement == d)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // tab forward or backward one step
        private IInputElement TabStep(bool backward=false)
        {
            if (backward)   UserInput.KeyDown(Shift);
                            UserInput.KeyPress(Tab);
            if (backward)   UserInput.KeyUp(Shift);

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return SWI.Keyboard.FocusedElement;
        }

        private IContentHost GetContentHost(ContentElement ce)
        {
            // there's no public way of doing this in general, so just support the
            // special cases used in this test
            if (ce == _hyperlinkTB)     return _textblockICH;
            if (ce == _hyperlinkFD)     return _fdsvICH;
            return null;
        }

        const int Tolerance = 50;       // tolerance (in ms) for all time comparisons

        static readonly string TTOpening = ToolTipService.ToolTipOpeningEvent.Name;
        static readonly string TTClosing = ToolTipService.ToolTipClosingEvent.Name;
        static readonly string TTOpened = ToolTip.OpenedEvent.Name;
        static readonly string TTClosed = ToolTip.ClosedEvent.Name;
        static readonly string KBGotFocus = SWI.Keyboard.GotKeyboardFocusEvent.Name;
        static readonly string KBLostFocus = SWI.Keyboard.LostKeyboardFocusEvent.Name;

        static readonly string LeftCtrl = SWI.Key.LeftCtrl.ToString();
        static readonly string RightCtrl = SWI.Key.RightCtrl.ToString();
        static readonly string Ctrl = LeftCtrl;
        static readonly string LeftShift = SWI.Key.LeftShift.ToString();
        static readonly string RightShift = SWI.Key.RightShift.ToString();
        static readonly string Shift = LeftShift;
        static readonly string LeftAlt = SWI.Key.LeftAlt.ToString();
        static readonly string RightAlt = SWI.Key.RightAlt.ToString();
        static readonly string Alt = LeftAlt;
        static readonly string F10 = SWI.Key.F10.ToString();
        static readonly string Tab = SWI.Key.Tab.ToString();

        DependencyProperty ShowsToolTipOnKeyboardFocusProperty;
        ToolTipTracker _tracker = new ToolTipTracker();
        List<string> _errors = new List<string>();
        double _inputOH;    // estimate of overhead (in ms) for input simulation, dispatching events, etc.
        int _shortWait;     // wait for overhead and tolerance (only)

        Ribbon _ribbon;
        FrameworkElement _reset, _rectangle;
        ListBox _listbox;
        Button _button;
        TextBox _textbox, _textBoxNoTT, _textBoxBSD0, _textBoxTT, _textBoxISD;
        Image _image, _focusableImage;
        GroupBox _groupbox;
        Hyperlink _hyperlinkTB, _hyperlinkFD;
        FlowDocumentScrollViewer _flowdocSV;
        IContentHost _textblockICH, _fdsvICH;

        #endregion

        #region Nested Types

        public class INPCBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string name)
            {
                var handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public class Model : INPCBase
        {
            public Model(int n)
            {
                Data = DataList.Create(n);
            }

            DataList _data;
            public DataList Data
            {
                get { return _data; }
                private set { _data = value; OnPropertyChanged(nameof(Data)); }
            }
        }

        public class DataItem : INPCBase
        {
            string _title;
            public string Title
            {
                get { return _title; }
                set { _title = value; OnPropertyChanged(nameof(Title)); }
            }

            public override string ToString()
            {
                return Title;
            }
        }

        public class DataList : ObservableCollection<DataItem>
        {
            public static DataList Create(int n)
            {
                DataList list = new DataList();
                for (int i = 0; i < n; ++i)
                {
                    list.Add(new DataItem { Title = "Item " + i });
                }
                return list;
            }
        }
        #endregion
    }
}
