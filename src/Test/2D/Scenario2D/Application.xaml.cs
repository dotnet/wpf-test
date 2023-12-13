// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Threading;
using System.Reflection;
using System.Resources;
using System.Collections;
using Microsoft.Test.Graphics;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    public partial class ScenarioApplication : Application
    {
        /// <summary/>
        public static void Launch()
        {
            char[] tokens ={ ' ' };
            ScenarioApplication.s_driverInvokeArgs = DriverState.DriverParameters["Args"].Split(tokens);
            ScenarioApplication.Main();
        }

        string Help
        {
            get
            {
                return
                    "\n2D animation scenario framework" +
                    "\n==================================" +
                    "\n" +
                    "\nVerification parameters (case sensitive and mutually exclusive):" +
                    "\n" +
                    "\n   /scenario=          (String)  XAML file name of the scenario. (e.g. /scenario=Integrate2D.xaml)" +
                    "\n" +
                    "\n   /times=             (Int32[]) If set, enables animation verification. (e.g. /times=100,300,500)" +
                    "\n                       List of times at which verification takes place, in milliseconds ." +
                    "\n" +
                    "\n   /failOnIdential=    (bool)   If true, the test fails if two captures are identical. (e.g. /serialize_time=true)" +
                    "\n";
            }
        }

        // We need to override application behavior to use a styled window with no UI chrome.
        protected override void OnStartup(StartupEventArgs e)
        {
            NavWindow = new NavigationWindow();
            NavWindow.Source = new Uri("pack://application:,,,/scenario2d;component/Tests/launcher.xaml", UriKind.RelativeOrAbsolute);
            NavWindow.Width = 400;
            NavWindow.Height = 400;
            NavWindow.WindowStyle = WindowStyle.SingleBorderWindow;
            NavWindow.Topmost = true;
            NavWindow.Title = "2D Scenario";
            NavWindow.Style = GetPlainStyle();
            NavWindow.Navigated += new NavigatedEventHandler(OnNavigated);
            NavWindow.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            NavWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            NavWindow = null;
            base.OnExit(e);
        }

        private Style GetPlainStyle()
        {
            Style style = new Style(typeof(NavigationWindow));
            ControlTemplate template = new ControlTemplate(typeof(NavigationWindow));
            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter), "CP");
            contentPresenter.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(NavigationWindow.ContentProperty));
            template.VisualTree = contentPresenter;
            style.Setters.Add(new Setter(Control.TemplateProperty, template));
            return style;
        }

        void OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (!_firstNavigationDone)
            {
                _firstNavigationDone = true;

                string scenario = null;
                string[] cmdArgs = GetArgs();
                foreach (string arg in cmdArgs)
                {
                    if (arg.IndexOfAny(new char[] { '/', '-' }) != 0)
                    {
                        continue;
                    }

                    string[] s = arg.Substring(1, arg.Length - 1).Split(':', '=');

                    switch (s[0])
                    {
                        case "scenario":
                            scenario = s[1];
                            break;

                        default:
                            if (s.Length == 1)
                            {
                                if (s[0] == "?")
                                {
                                    s[0] = "Help";
                                }

                                // default flags-type switches to true if present
                                this.Properties.Add(s[0], "true");
                            }
                            else
                            {
                                this.Properties.Add(s[0], s[1]);
                            }
                            break;
                    }
                }

                // Parse tolerances from command lines
                ScenarioUtility.SetToleranceFromParameters(this.Properties);

                // Discover bit depth for rendering verification
                ColorOperations.DiscoverBitDepth(new Point(NavWindow.Left, NavWindow.Top));

                if (this.Properties["Help"] != null)
                {
                    // Print help for framework
                    Logger logger = Logger.Create();
                    logger.AddFailure(Help);
                    logger.Close();
                }

                if (scenario != null)
                {
                    ScenarioUtility.NavigateToPage(scenario);
                }
                else if (this.Properties["Help"] != null)
                {
                    // User was only asking for help; not to run a test.
                    this.Shutdown();
                }
            }
        }

        private string[] GetArgs()
        {
            if (s_driverInvokeArgs == null)
            {
                return ScenarioUtility.GetCommandLineArgs();
            }
            else
            {
                return s_driverInvokeArgs;
            }
        }

        void OnNavigated(object sender, NavigationEventArgs args)
        {
            Application application = Application.Current as Application;

            if (application.Properties["WindowWidth"] != null)
            {
                NavWindow.Width = double.Parse((string)application.Properties["WindowWidth"]);
            }
            if (application.Properties["WindowHeight"] != null)
            {
                NavWindow.Height = double.Parse((string)application.Properties["WindowHeight"]);
            }
            if (application.Properties["WindowBackgroundColor"] != null)
            {
                Color bg = StringConverter.ToColor((string)application.Properties["WindowBackgroundColor"]);
                application.MainWindow.Background = new SolidColorBrush(bg);
                _background = bg;
            }
            if (application.Properties["Help"] != null)
            {
                // Print help for individual tests
                if (application.MainWindow.Content is ScenarioUtility.IHelp)
                {
                    ScenarioUtility.PrintHelp((ScenarioUtility.IHelp)application.MainWindow.Content);
                }
            }

            if (verifier == null)
            {
                Panel panel = FindPanel((DependencyObject)Application.Current.MainWindow.Content);
                if (panel != null)
                {
                    bool failOnIdentical = false;
                    if (application.Properties["failOnIdentical"] != null)
                    {
                        failOnIdentical = bool.Parse(application.Properties["failOnIdentical"] as string);
                    }

                    if (application.Properties["times"] != null)
                    {
                        string[] times = ((string)application.Properties["times"]).Split(',');
                        int[] actualTimes = new int[times.Length];
                        for (int i = 0; i < times.Length; i++)
                        {
                            actualTimes[i] = int.Parse(times[i]);
                        }

                        verifier = new AnimationVerifier(panel, _background, failOnIdentical);
                        ((AnimationVerifier)verifier).EnterAnimationLoop(actualTimes);
                    }
                }
            }

        }

        public Panel FindPanel(DependencyObject root)
        {
            if (root is Page)
            {
                return FindPanel((DependencyObject)((Page)root).Content);
            }
            if (root is Panel)
            {
                Panel dp = root as Panel;
                Brush bgBrush = (Brush)dp.GetValue(Panel.BackgroundProperty);
                if (bgBrush != null && bgBrush is SolidColorBrush)
                {
                    _background = ((SolidColorBrush)bgBrush).Color;
                }
                return dp;
            }
            return null;
        }


        internal NavigationWindow NavWindow;
        internal Verifier verifier = null;

        bool _firstNavigationDone = false;
        Color _background = Colors.Black;
        private static String[] s_driverInvokeArgs = null;
    }
}