// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using DRT;
using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;

[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/DrtAnimation/angels.jpg")]

namespace DRTAnimation
{
    public class DRTAnimation : DrtBase
    {
        private static bool s_isInteractive;

        // Shared objects for tests.

        private static Pen s_borderPen = null;
        private static Pen s_testPen = null;

        internal static Pen BorderPen
        {
            get
            {
                if (s_borderPen == null)
                {
                    s_borderPen = new Pen(Brushes.Yellow, 2.0);
                }
                return s_borderPen;
            }
        }
        internal static Pen TestPen
        {
            get
            {
                if (s_testPen == null)
                {
                    s_testPen = new Pen(Brushes.Red, 2.0);
                }
                return s_testPen;
            }
        }


        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DRTAnimation();

            return drt.Run(args);
        }

        private DRTAnimation()
        {
            WindowTitle = "DRTAnimation";
            DrtName = "DRTAnimation";
            WindowSize = new Size(800, 750);
            TeamContact = "WPF";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[] {
                        new NonVisualTestSuite(),
                        new DRTCSharpAnimationTestSuite(),
                        new DRTMarkupAnimationTestSuite(),
#if TESTBUILD_CLR40
                        new DRTEasingTestSuite(),
#endif
            };
        }

        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
            {
                return true;
            }

            if ("hold".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
            {
                TopMost = false;
                return true;
            }
            else if ("interactive".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
            {
                s_isInteractive = true;
                return true;
            }
            else if ("suite".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
            {
                string suiteName = args[k + 1];
                if (suiteName.IndexOf('*') != -1)
                {
                    suiteName = Regex.Escape(suiteName);
                    suiteName = suiteName.Replace("\\*", ".*?");
                    DrtTestSuite[] suites = Suites;
                    Regex regex = new Regex("(^" + suiteName + "$)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    for (int i = 0; i < suites.Length; i++)
                    {
                        DrtTestSuite suite = suites[i];
                        Match match = regex.Match(suite.Name);
                        if (match.Success)
                        {
                            SelectSuite(suite.Name);
                        }
                    }
                }
                return true;
            }

            return false;
        }

        protected override void PrintOptions()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("  -interactive  don't stop tests after a set time period");
            base.PrintOptions();
        }

        public void SetDefaultPage()
        {
            TextBlock text = new TextBlock();
            text.TextAlignment = TextAlignment.Center;
            text.Text = "DrtAnimation";

            Canvas canvas = new Canvas();
            canvas.Background = Brushes.White;
            canvas.Children.Add(text);

            RootElement = canvas;
            ShowRoot();
        }

        #region Static

        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public static void Check(double expected, double actual, string message)
        {
            string checkString = "    " + message + " expected: " + expected.ToString() + " actual: " + actual.ToString();

            WriteLine(checkString);
            if (actual != expected)
            {
                throw new InvalidOperationException("DRT Failure: " + checkString);
            }
        }

        public static void Check(int expected, int actual, string message)
        {
            string checkString = "    " + message + " expected: " + expected.ToString() + " actual: " + actual.ToString();

            WriteLine(checkString);
            if (actual != expected)
            {
                throw new InvalidOperationException("DRT Failure: " + checkString);
            }
        }

        public static void Check(string expected, string actual, string message)
        {
            string checkString = "    " + message + " expected: \"" + expected + "\" actual: \"" + actual + "\"";

            WriteLine(checkString);

            if (actual != expected)
            {
                throw new InvalidOperationException("DRT Failure: " + checkString);
            }
        }

        public static void Check(bool hasPassed, string message)
        {
            if (!hasPassed)
            {
                throw new InvalidOperationException("DRT Failure: " + message);
            }
        }

        public static bool IsInteractive
        {
            get
            {
                return s_isInteractive;
            }
        }

        #endregion
    }

    public sealed partial class NonVisualTestSuite : DrtTestSuite
    {
        private Clock _clock;

        public NonVisualTestSuite() : base("NonVisualTests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            ((DRTAnimation)DRT).SetDefaultPage();

            return new DrtTest[]
            {
                // Make sure that CompositionTargetRendering test is first because
                // we rely on being the only thing active for the TimeManager.
                new DrtTest(CompositionTargetTest),
                new DrtTest(FreezablesTest),
                new DrtTest(BeginAnimationTest),
                new DrtTest(SimpleTimingTest),
                new DrtTest(ApplyAnimationClockTest1),
                new DrtTest(ApplyAnimationClockTest2),
                new DrtTest(ApplyAnimationClockTest3),
                new DrtTest(InvalidValuesTest),
                new DrtTest(KeyFrameAnimationTest),
            };
        }

        void SimpleTimingTest()
        {
            ParallelTimeline timeline = new ParallelTimeline();
            timeline.Duration = new TimeSpan(0, 0, 0, 0, 100);
            timeline.CurrentStateInvalidated += new EventHandler(SimpleTimingTest_OnCurrentStateInvalidated);

            _clock = timeline.CreateClock();
            _clock.Controller.Begin();

            DRT.Suspend();
        }

        private void SimpleTimingTest_OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            Clock clock = (Clock)sender;

            if (clock.CurrentState != ClockState.Active)
            {
                DRT.Resume();

                Console.WriteLine("    NonVisualTestSuite.SimpleTimingTest() completed.");
            }
        }
    }
}

