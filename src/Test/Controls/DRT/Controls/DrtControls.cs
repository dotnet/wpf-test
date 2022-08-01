// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT for Component Model.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Interop;
using DRT;

// Loose resources should be declared in the .proj file.  Since this project does
// not have a .proj file the required attributes can be added manually.
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtexpander.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtframeinner.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtframeouter.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtgridsplitter.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtgroupbox.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtlistbox.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtlogicaltreeverify.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtmenu.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtprogressbar.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtfocusvisual.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtscrollbar.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/drtcombobox.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/page1.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/page2.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/page3.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/rose.jpg")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/smallenglish.txt")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/textpanelwithmargin.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/controls/tulip.jpg")]


namespace DRT
{
    public sealed class DrtControls : DrtControlsBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtControls drt = new DrtControls();
            int returnValue = 0;

            try
            {
                returnValue = drt.Run(args);
            }
            finally
            {
                drt.ResetMenuShowDelay();
            }

            return returnValue;
        }

        private DrtControls()
        {
            WindowTitle = "Component Model DRT";
            Contact = "Microsoft";
            TeamContact = "WPF";
            DrtName = "DrtControls";
            WindowPosition = new Point(50, 50);
            WindowSize = Size.Empty;
            BaseDirectory = @".\DrtFiles\Controls\";
            BlockInput = true;
            TopMost = true;
            Verbose = true;

            List<DrtTestSuite> suites = new List<DrtTestSuite>();


            //
            // --- SPECIAL AT THE START ---
            //

            // DrtThemes
            suites.Add(new DrtThemesSuite());

            //
            // --- STANDARD SET ---
            //

            // DrtAccessibility
            suites.Add(new DrtAccessibilitySuite());

            // DrtActiveElement
            suites.Add(new DrtActiveElementSuite());

            // DrtMenu
            foreach (MenuTest test in Enum.GetValues(typeof(MenuTest)))
            {
                suites.Add(new MenuSuite(test));
            }
            DisableSuite("Menu.TextBox");

            // DrtButtonBase
            foreach (ButtonTest test in Enum.GetValues(typeof(ButtonTest)))
            {
                suites.Add(new ButtonBaseSuite(test));
            }

            // DrtComboBox
            Append(suites, new DrtTestSuite[] {
                new ComboBoxSuite(),
                new ComboBoxTestSuite(),
                new EditableComboBoxSuite(),
            });
            DisableSuite("ComboBox.Test");

            //DrtExpander
            suites.Add(new DrtExpanderSuite());

            //DrtStatusBar
            suites.Add(new DrtStatusBarSuite());

            // DrtDelayLoad
            suites.Add(new DefSharedSuite());

            // DrtFocusVisual
            suites.Add(new DrtFocusVisualSuite());

            // DrtGroupBox
            suites.Add(new GroupBoxSuite());

            // DrtKeyboardNavigation
            Append(suites, new DrtTestSuite[] {
                new KeyboardNavigationTestSuite(),
                new TabAndDirectionalNavigationTestSuite(),
                //new WinFormsAccessKeySuite(),

                new DefaultButtonsSuite(),
                new PrimaryTextSuite(),
                new AccessKeyManagerTestSuite(),
                });
            DisableSuite("WinForms");

            // DrtListBox
            Append(suites, new DrtTestSuite[] {
                new ListBoxSuite(SelectionMode.Single, false),
                new ListBoxSuite(SelectionMode.Multiple, false),
                new ListBoxSuite(SelectionMode.Extended, false),
                new ListBoxSuite(ScrollingTest.SingleScrolling, false),
                new ListBoxSuite(ScrollingTest.ListBoxNoSV, false),
                new ListBoxSuite(ScrollingTest.HorizontalScrolling, false),
                new ListBoxSuite(ScrollingTest.LargeItems, false),
                new ListBoxSuite(ScrollingTest.ChangeItemsPanel, false),

                new ListBoxSuite(SelectionMode.Single, true),
                new ListBoxSuite(SelectionMode.Multiple, true),
                new ListBoxSuite(SelectionMode.Extended, true),
                new ListBoxSuite(ScrollingTest.SingleScrolling, true),
                new ListBoxSuite(ScrollingTest.ListBoxNoSV, true),
                new ListBoxSuite(ScrollingTest.HorizontalScrolling, true),
                new ListBoxSuite(ScrollingTest.LargeItems, true),
                new ListBoxSuite(ScrollingTest.ChangeItemsPanel, true),

                new TypeSearchSuite(),
            });

            // DrtListView
            suites.Add(new DrtListViewSuite());

            // DrtDataGrid
            suites.Add(new DrtDataGridSuite());

            // DrtVSMGoToState
            suites.Add(new DrtVSMGoToStateSuite());

            // DrtDatePicker
            suites.Add(new DrtDatePickerSuite());

            // DrtCalendar
            suites.Add(new DrtCalendarSuite());

            // DrtContainerVirtualization
            suites.Add(new DrtContainerVirtualizationSuite());

            // This suite causes the Drt to exceed its allocated time limit. 
            // Hence disable it normally and only allow it to be run manually.
            DisableSuite(suites.Count - 1);

            // DrtPopup
            suites.Add(new DrtPopupSuite());

            // DrtRangeBase
            suites.Add(new DrtRangeBaseSuite());

            // DrtRadioButton
            suites.Add(new DrtRadioButtonListSuite());

            // DrtSelector
            suites.Add(new DrtSelectorSuite());

            //// DrtSlider
            //foreach (SliderTest test in Enum.GetValues(typeof(SliderTest)))
            //{
            //    
            //    //suites.Add(new SliderSuite(test));
            //}

            // DrtTabControl
            suites.Add(new TabControlSuite());

        
            // DrtToolBar
            suites.Add(new DrtToolBarSuite());

            // DrtToolTip
            suites.Add(new ToolTipSuite());

            // DrtGridSplitter
            suites.Add(new GridSplitterSuite());

            // DrtProgressBar
            suites.Add(new ProgressBarSuite());

            // DrtTreeView
            suites.Add(new DrtTreeViewSuite());

            // DrtVirtualizingTreeView
            suites.Add(new DrtVirtualizingTreeViewSuite());

            // DrtThemeDictionaryExtension
            suites.Add(new DrtThemeDictionaryExtension());

            // DrtMetadata
            suites.Add(new DrtMetadataSuite());

            // DrtScrollBar
            suites.Add(new ScrollBarSuite());

            // DrtLayeredWindows
            suites.Add(new DrtLayeredWindowsSuite());

            //
            // --- SPECIAL AT THE END ---
            //

            // DrtLogicalTreeVerify
            suites.Add(new DrtLogicalTreeVerifySuite());

            // DrtFrame
            suites.Add(new DrtFrameTestSuite());


#if MULTICONTEXTCONTROL_SUITE_FIXED
            // DrtMultiContextControl
            foreach (MultiContextTest test in Enum.GetValues(typeof(MultiContextTest)))
            {
                suites.Add(new MultiContextSuite(test));
            }
#endif

            Suites = suites.ToArray();
        }

        protected override void OnStartingUp()
        {
            base.OnStartingUp();

            // If no one set menu show delay, it should be 10 ms
            if (_menuShowDelay == -1 && !KeepAlive)
            {
                _menuShowDelay = 10;
            }

            ShortenMenuShowDelay();

            if (DontStartSuites)
            {
                SetupMainMenu();
            }
        }

        protected override void OnShuttingDown()
        {
            base.OnShuttingDown();
        }

        private void SetupMainMenu()
        {
            System.Uri resourceLocator = new System.Uri(@"mainmenu.xaml", System.UriKind.RelativeOrAbsolute);
            DRT.RootElement = System.Windows.Application.LoadComponent(resourceLocator) as Visual;

            ListBox list = (ListBox)FindElementByID("SuiteList");
            list.ItemsSource = Suites;

            Button btn = (Button)FindElementByID("Run");
            btn.Click += new RoutedEventHandler(OnRun);

            DRT.ShowRoot();
        }

        private void OnRun(object source, RoutedEventArgs e)
        {
            ListBox list = (ListBox)FindElementByID("SuiteList");
            DrtTestSuite suite = (DrtTestSuite)list.SelectedItem;

            if (suite != null)
            {
                CheckBox keepAlive = (CheckBox)FindElementByID("KeepAlive");
                KeepAlive = keepAlive.IsChecked != true;
                SelectSuite(suite.Name);
                StartSuites();
            }
        }

        protected override void PrintOptions()
        {
            base.PrintOptions();

            Console.WriteLine("  -menushowdelay                                 Specify a different menu show delay value");
            Console.WriteLine("  -perf <ControlName> <VirtualizationMode>       Specify the parameters to measure perf under the ContainerVirtualization suite");
            Console.WriteLine("  -TraceUpdateOffsets                            Trace calls to UpdateOffsets in the ContainerVirtualization suite");
        }

        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            if ("hold".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
            {
                TopMost = false;
                MismatchedInputWarningLevel = WarningLevel.Ignore;
                WindowDeactivatedWarningLevel = WarningLevel.Ignore;
                if (!_suiteSelected)
                {
                    DontStartSuites = true;
                }
            }
            else if ("menushowdelay".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
            {
                _menuShowDelay = int.Parse(args[++k]);
                return true;
            }
            else if ("suite".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
            {
                _suiteSelected = true;
                DontStartSuites = false;

                string suiteName = args[k+1];
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
            }
            else if ("perf".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
            {
                InPerformanceMode = true;
                if (!_suiteSelected)
                {
                    DontStartSuites = true;
                }

                ControlNameForPerformanceTesting = args[++k];
                VirtualizationMode = args[++k];
                return true;
            }
            else if ("traceupdateoffsets".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
            {
                TraceUpdateOffsets = true;
                return true;
            }

            return base.HandleCommandLineArgument(arg, option, args, ref k);
        }

        internal static void NotUsingDrtWindow(DrtBase drt)
        {
            Border b = new Border();
            b.IsEnabled = false;
            DRT.Show(b);
            b.Background = Brushes.AntiqueWhite;

            TextBlock t = new TextBlock();
            t.Text = "This window is not used in these tests";
            b.Child = t;
        }

        public new HwndSource MainWindow
        {
            get { return base.MainWindow; }
            set { base.MainWindow = value; }
        }

        public bool InPerformanceMode
        {
            get; set;
        }

        public string ControlNameForPerformanceTesting
        {
            get; set;
        }

        public string VirtualizationMode
        {
            get; set;
        }

        public bool TraceUpdateOffsets
        {
            get; set;
        }

        private static void Append<T>(List<T> list, T[] array)
        {
            foreach (T t in array)
            {
                list.Add(t);
            }
        }

        private void ShortenMenuShowDelay()
        {
            if (_menuShowDelay == -1) return;

            SystemParametersInfo(106 /*SPI_GETMENUSHOWDELAY*/, 0, ref _startingMenuShowDelay, 0);

            SetMenuShowDelay(_menuShowDelay);
        }

        private void ResetMenuShowDelay()
        {
            if (_startingMenuShowDelay < 0) return;

            SetMenuShowDelay(_startingMenuShowDelay);
        }

        private void SetMenuShowDelay(int delay)
        {
            DRT.Assert(delay >= 0, "MenuShowDelay cannot be set to a negative value");

            int val = 0;
            if (!SystemParametersInfo(107 /*SPI_SETMENUSHOWDELAY*/, delay, ref val, 0))
            {
                Console.WriteLine("Unable to set MenuShowDelay.");
            }
        }

        private int _menuShowDelay = -1;
        private int _startingMenuShowDelay = -1;

        private bool _suiteSelected = false;

        internal const int PopupAnimationDelay = 300; // Time to wait for a popup animation to complete. It's really 150 in the product, but being paranoid here.


    }
}

