using Microsoft.Test;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Theming;
using Microsoft.Test.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Diagnostics;
using System.Windows.Markup;
using System.Windows.Resources;

namespace ResourceDictionaryDiagnosticsTests.FeatureTests
{
    // Priority = 1, SubArea = ResourceDictionaryDiagnostics
    // [Microsoft.Test.Discovery.Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Enable")]
    public class ResourceDictionaryDiagnosticsTests : WindowTest
    {
        ResourceDictionaryTestEnabler _enabler = new ResourceDictionaryTestEnabler(DriverState.DriverParameters["Mode"]);

        public ResourceDictionaryDiagnosticsTests()
        {
            if (EnsureCurrentThemeIsAeroOrAero2())
            {
                InitializeSteps += InitializeTest;

                RunSteps += VerifyInitalResourceDictionaryLoadedEvents;
                RunSteps += VerifyInitialResourceDictionaryEnumeration;

                RunSteps += ResetCollections;

                if (OSVersion.IsWindows7OrGreater())
                {
                    RunSteps += ChangeThemeToHighContrast;
                }
                else
                {
                    RunSteps += ChangeThemeToWindowsClassic;
                }

                RunSteps += VerifyHC2ResourceDictionaryUnloadedEvents;
                RunSteps += VerifyHC2ResourceDictionaryLoadedEvents;
                RunSteps += VerifyHC2ResourceDictionaryEnumeration;

                CleanUpSteps += RevertToOriginalTheme;
                CleanUpSteps += RemoveListeners;
                CleanUpSteps += DisposeEnabler;
            }
            else
            {
                RunSteps += DoNothing;
            }
        }

        private TestResult DoNothing()
        {
            Log.LogStatus(string.Empty);
            Log.LogStatus("The current theme is not Aero or Aero2 - likely on a server build");
            Log.LogStatus("Skipping tests..");
            Log.LogStatus(string.Empty);

            return TestResult.Ignore;
        }

        private bool EnsureCurrentThemeIsAeroOrAero2(bool take2 = false)
        {
            bool result = false;
            try
            {
                var theme = Theme.GetCurrent();
                if (string.Equals(theme.Name, "Aero", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(theme.Name, "Aero2", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = true;
                }
            }
            catch
            {
                // do nothing
            }

            if (take2)
            {
                return result;
            }

            if (!result)
            {
                List<Theme> themes = new List<Theme>(Theme.GetAvailableSystemThemes());
                Theme aeroTheme = themes.Find((Theme theme) => { return string.Equals(theme.Name, "Aero", StringComparison.InvariantCultureIgnoreCase); });
                Theme aero2Theme = themes.Find((Theme theme) => { return string.Equals(theme.Name, "Aero2", StringComparison.InvariantCultureIgnoreCase); });

                try
                {
                    // Try to set the theme to Aero2
                    if (OSVersion.IsWindows8OrGreater() && (aero2Theme != null))
                    {
                        Theme.SetCurrent(aero2Theme);
                    }
                    else if (aeroTheme != null)
                    {
                        Theme.SetCurrent(aeroTheme);
                    }

                    result = EnsureCurrentThemeIsAeroOrAero2(take2: true);
                }
                catch
                {
                    // do nothing
                }
            }

            return result;
        }

        private TestResult InitializeTest()
        {
            InitializeListeners();

            if (!InitializeControls())
            {
                return TestResult.Fail;
            }

            InitializeTestData();

            this.Log.LogStatus("Successfully initialized test window");
            return TestResult.Pass;
        }

        private bool InitializeDiagnostics()
        {
            Log.LogStatus(string.Empty);
            Log.LogStatus($"Starting: {nameof(InitializeDiagnostics)}");
            bool result = true;

            try
            {
                Type rddType = typeof(System.Windows.Diagnostics.ResourceDictionaryDiagnostics);
                var field = rddType.GetField("s_EnableForTestPurposes", BindingFlags.NonPublic | BindingFlags.Static);
                field.SetValue(null, true);
            }
            catch (Exception e)
            {
                Log.LogStatus(e.ToString());
                Log.LogStatus("Failed to set System.Windows.Diagnostics.ResourceDictionaryDiagnostics.s_EnableForTestPurposes = true using private reflection");
                Log.LogStatus("Can not continue tests...");

                result = false;
            }

            Log.LogStatus($"Done: {nameof(InitializeDiagnostics)}");
            Log.LogStatus(string.Empty);

            return result;
        }

        private bool InitializeControls()
        {
            Log.LogStatus(string.Empty);
            Log.LogStatus($"Starting: {nameof(InitializeControls)}");

            bool result = true;

            try
            {
                Uri uri = new Uri("pack://application:,,,/ResourceDictionaryDiagnosticsTests;component/TestGrid.xaml");
                StreamResourceInfo sri = Application.GetResourceStream(uri);
                if (sri == null)
                {
                    throw new Exception("Could not read TestGrid.xaml");
                }

                using (Stream stream = sri.Stream)
                {
                    this.Window.Content = (Grid)XamlReader.Load(stream);
                }

                // Width, Height identified by trial and error - these dimensions seem to work well.
                this.Window.Width = 635;
                this.Window.Height = 455;

                this.Window.Title = nameof(ResourceDictionaryDiagnosticsTests);
            }
            catch (Exception e)
            {
                Log.LogStatus(e.ToString());
                Log.LogStatus("Failed to initialize Grid with themed buttons - can not continue tests...");

                result = false;
            }

            Log.LogStatus($"Done: {nameof(InitializeControls)}");
            Log.LogStatus(string.Empty);

            return result;
        }

        private void InitializeTestData()
        {
            if (OSVersion.IsWindows8OrGreater())
            {
                _testData = Aero2Data.Data;
            }
            else
            {
                _testData = AeroData.Data;
            }
        }

        private void InitializeListeners()
        {
            _genericDictionariesLoaded = new List<ResourceDictionaryInfo>();
            _themedDicionariesLoaded = new List<ResourceDictionaryInfo>();
            _themedDictionariesUnloaded = new List<ResourceDictionaryInfo>();

            ResourceDictionaryDiagnostics.GenericResourceDictionaryLoaded += GenericResourceDictionaryLoaded;
            ResourceDictionaryDiagnostics.ThemedResourceDictionaryLoaded += ThemedResourceDictionaryLoaded;
            ResourceDictionaryDiagnostics.ThemedResourceDictionaryUnloaded += ThemedResourceDictionaryUnloaded;
        }

        private TestResult ResetCollections()
        {
            lock (_mutex)
            {
                _genericDictionariesLoaded.Clear();
                _themedDicionariesLoaded.Clear();
                _themedDictionariesUnloaded.Clear();
            }

            return TestResult.Pass;
        }

        private TestResult ChangeThemeToHighContrast()
        {
            TestResult result = TestResult.Pass;

            try
            {
                BeginThemeChange();
                _themeSwitcher = Theme.SetTheme(Theme.HighContrastTheme.hc2);
            }
            catch (Exception e)
            {
                Log.LogStatus(e.ToString());
                Log.LogStatus("Failed to change OS theme to High Contrast #2");
                result = TestResult.Fail;
            }

            // Wait 10 seconds for theme set
            // There is no good mechanism to detect when WPF has fully processed WM_THEMECHANGED
            // so we just have to guess and hope that 10 seconds would be sufficient.
            WaitFor(10 * 1000);

            return result;
        }

        private TestResult ChangeThemeToWindowsClassic()
        {
            var systemThemes = new List<Theme>(Theme.GetAvailableSystemThemes());

            var windowsClassicTheme
                = systemThemes.Find((theme) =>
                {
                    return string.Equals(theme.Name, "Windows Classic", StringComparison.InvariantCultureIgnoreCase);
                });

            if (windowsClassicTheme == null)
            {
                return TestResult.Ignore;
            }

            TestResult result = TestResult.Pass;

            try
            {
                BeginThemeChange();
                _themeSwitcher = Theme.SetTheme(windowsClassicTheme);
            }
            catch (Exception e)
            {
                Log.LogStatus(e.ToString());
                Log.LogStatus("Failed to change OS theme to Windows Classic");
                result = TestResult.Fail;
            }

            // Wait 10 seconds for theme set
            // There is no good mechanism to detect when WPF has fully processed WM_THEMECHANGED
            // so we just have to guess and hope that 10 seconds would be sufficient.
            Log.LogStatus("Waiting 10 seconds for theme change to take effect...");
            WaitFor(10 * 1000);

            return result;
        }

        private TestResult RevertToOriginalTheme()
        {
            TestResult result = TestResult.Pass;

            try
            {
                _themeSwitcher?.Dispose();
            }
            catch (Exception e)
            {
                Log.LogStatus(e.ToString());
                result = TestResult.Fail;
            }

            Log.LogStatus("Reverted the OS to original theme");
            return result;
        }

        private TestResult RemoveListeners()
        {
            ResourceDictionaryDiagnostics.GenericResourceDictionaryLoaded -= GenericResourceDictionaryLoaded;
            ResourceDictionaryDiagnostics.ThemedResourceDictionaryLoaded -= ThemedResourceDictionaryLoaded;
            ResourceDictionaryDiagnostics.ThemedResourceDictionaryUnloaded -= ThemedResourceDictionaryUnloaded;

            return TestResult.Pass;
        }

        private TestResult DisposeEnabler()
        {
            _enabler.Dispose();
            return TestResult.Pass;
        }

        private TestResult VerifyInitalResourceDictionaryLoadedEvents()
        {
            var loadedDictionaries = new List<ResourceDictionaryInfo>();

            lock (_mutex)
            {
                loadedDictionaries.AddRange(_themedDicionariesLoaded);
                loadedDictionaries.AddRange(_genericDictionariesLoaded);
            }

            return VerifyDictionariesNoLock(_testData.StartupLoadedEvents(), loadedDictionaries);
        }

        private TestResult VerifyInitialResourceDictionaryEnumeration()
        {
            var dictionaries = new List<ResourceDictionaryInfo>();
            dictionaries.AddRange(ResourceDictionaryDiagnostics.GenericResourceDictionaries);
            dictionaries.AddRange(ResourceDictionaryDiagnostics.ThemedResourceDictionaries);

            return VerifyDictionariesNoLock(_testData.StartupEnumeration(), dictionaries);
        }

        private TestResult VerifyHC2ResourceDictionaryUnloadedEvents()
        {
            return VerifyDictionaries(_testData.ThemeChangedUnloadedEvents(), _themedDictionariesUnloaded);
        }

        private TestResult VerifyHC2ResourceDictionaryLoadedEvents()
        {
            var loadedDictionaries = new List<ResourceDictionaryInfo>();

            lock (_mutex)
            {
                loadedDictionaries.AddRange(_genericDictionariesLoaded);
                loadedDictionaries.AddRange(_themedDicionariesLoaded);
            }

            return VerifyDictionariesNoLock(_testData.ThemeChangedLoadedEvents(), loadedDictionaries);
        }

        private TestResult VerifyHC2ResourceDictionaryEnumeration()
        {
            var dictionaries = new List<ResourceDictionaryInfo>();

            dictionaries.AddRange(ResourceDictionaryDiagnostics.GenericResourceDictionaries);
            dictionaries.AddRange(ResourceDictionaryDiagnostics.ThemedResourceDictionaries);

            return VerifyDictionariesNoLock(_testData.ThemeChangedEnumeration(), dictionaries);
        }

        private TestResult VerifyDictionaries(List<Tuple<string, string, string>> expectedDictionaries, List<ResourceDictionaryInfo> collection)
        {
            TestResult result = TestResult.Pass;

            lock (_mutex)
            {
                result = VerifyDictionariesNoLock(expectedDictionaries, collection);
            }

            return result;
        }

        private TestResult VerifyDictionariesNoLock(List<Tuple<string, string, string>> expectedDictionaries, List<ResourceDictionaryInfo> collection)
        {
            TestResult result = TestResult.Pass;

            if (expectedDictionaries.Count != collection.Count)
            {
                Log.LogStatus($"\tExpected RD Count({expectedDictionaries.Count}) <> Observed RD Count({collection.Count})");
                result = TestResult.Fail;
            }

            foreach (var expectedRdInfo in expectedDictionaries)
            {

                var rdInfo = collection.Find(ResourceDictionaryInfoPredicate.Comparer(expectedRdInfo.Item1, expectedRdInfo.Item2, expectedRdInfo.Item3, Log));

                if (rdInfo == null)
                {
                    Log.LogStatus("\tDictionary info not found:");
                    Log.LogStatus($"\t\tAssembly: {expectedRdInfo.Item1}; Dictionary Assembly: {expectedRdInfo.Item2}; SourceUri = {expectedRdInfo.Item3}");

                    result = TestResult.Fail;
                }
            }

            if (result == TestResult.Fail)
            {
                Log.LogStatus("\tThe items in the input collection being tested are:");
                foreach (var dict in collection)
                {
                    Log.LogStatus($"\t\tAssembly: {dict?.Assembly?.GetName()?.Name}; Dictionary Assembly: {dict?.ResourceDictionaryAssembly?.GetName()?.Name}; SourceUri = {dict?.SourceUri?.AbsoluteUri}");
                }

            }

            return result;
        }

        // The test started failing because the OS sometimes sends a second WM_THEMECHANGED
        // message, which causes a second round of dictionary unload/load events
        // that adds unexpected entries to the lists.  To work around this, ignore
        // the events once the second round has started, i.e. at the first Unload
        // event that follows a Load event.
        void BeginThemeChange()
        {
            _ignoreResourceDictionaryEvents = false;
        }

        void GenericResourceDictionaryLoaded(object sender, ResourceDictionaryLoadedEventArgs args)
        {
            lock (_mutex)
            {
                if (!_ignoreResourceDictionaryEvents)
                {
                    _genericDictionariesLoaded.Add(args.ResourceDictionaryInfo);
                }
            }
        }

        void ThemedResourceDictionaryLoaded(object sender, ResourceDictionaryLoadedEventArgs args)
        {
            lock (_mutex)
            {
                if (!_ignoreResourceDictionaryEvents)
                {
                    _themedDicionariesLoaded.Add(args.ResourceDictionaryInfo);
                }
            }
        }

        private void ThemedResourceDictionaryUnloaded(object sender, ResourceDictionaryUnloadedEventArgs args)
        {
            lock (_mutex)
            {
                if (!_ignoreResourceDictionaryEvents && (_themedDicionariesLoaded.Count > 0))
                {
                    _ignoreResourceDictionaryEvents = true;
                    Log.LogStatus("Ignoring events after a second WM_THEMECHANGED message");
                }

                if (!_ignoreResourceDictionaryEvents)
                {
                    _themedDictionariesUnloaded.Add(args.ResourceDictionaryInfo);
                }
            }
        }

        private List<ResourceDictionaryInfo> _genericDictionariesLoaded;
        private List<ResourceDictionaryInfo> _themedDicionariesLoaded;
        private List<ResourceDictionaryInfo> _themedDictionariesUnloaded;

        private readonly object _mutex = new object();
        private bool _ignoreResourceDictionaryEvents;

        private Theme.ThemeSwitcher _themeSwitcher;

        private IResourceDictionaryTestsData _testData;

#pragma warning disable 414  // unused variables warning - remove after completing this code.

        #region TestData

        #region AssemblyNames

        private static readonly string PresentationFramework = "PresentationFramework";
        private static readonly string PresentationFramework_Aero = "PresentationFramework.Aero";
        private static readonly string PresentationFramework_Aero2 = "PresentationFramework.Aero2";
        private static readonly string PresentationFramework_Classic = "PresentationFramework.Classic";

        private static readonly string MyButton = "MyButton";
        private static readonly string MyButton_Aero2 = "MyButton.Aero2";
        private static readonly string MyButton_Classic = "MyButton.Classic";
        private static readonly string MyButton_Generic = "MyButton.Generic";

        private static readonly string YourButton = "YourButton";

        private static readonly string ThisAssembly = Assembly.GetExecutingAssembly().GetName().Name;

        #endregion // AssemblyNames

        #region BamlUris

        private static readonly string PresentationFramework_Aero_AeroNormalColorBaml
            = "pack://application:,,,/PresentationFramework.Aero;v4.0.0.0;component/themes/aero.normalcolor.baml";
        private static readonly string PresentationFramework_Aero2_Aero2NormalcolorBaml
            = "pack://application:,,,/PresentationFramework.Aero2;v4.0.0.0;component/themes/aero2.normalcolor.baml";
        private static readonly string PresentationFramework_Classic_ClassicBaml
            = "pack://application:,,,/PresentationFramework.classic;v4.0.0.0;component/themes/classic.baml";

        private static readonly string MyButton_Aero2_Aero2NormalColorBaml
            = "pack://application:,,,/MyButton.Aero2;v4.0.0.0;component/themes/aero2.normalcolor.baml";
        private static readonly string MyButton_Classic_ClassicBaml
            = "pack://application:,,,/MyButton.classic;v4.0.0.0;component/themes/classic.baml";
        private static readonly string MyButton_Generic_GenericBaml
            = "pack://application:,,,/MyButton.generic;v4.0.0.0;component/themes/generic.baml";

        private static readonly string YourButton_Aero2NormalColorBaml
            = "pack://application:,,,/YourButton;v4.0.0.0;component/themes/aero2.normalcolor.baml";
        private static readonly string YourButton_GenericBaml
            = "pack://application:,,,/YourButton;v4.0.0.0;component/themes/generic.baml";

        #endregion //BamlUris


        private interface IResourceDictionaryTestsData
        {
            List<Tuple<string, string, string>> StartupLoadedEvents();
            List<Tuple<string, string, string>> StartupEnumeration();
            List<Tuple<string, string, string>> ThemeChangedUnloadedEvents();
            List<Tuple<string, string, string>> ThemeChangedLoadedEvents();
            List<Tuple<string, string, string>> ThemeChangedEnumeration();
        }

        private class AeroData : IResourceDictionaryTestsData
        {
            private static AeroData _instance;
            private AeroData() { }

            public static IResourceDictionaryTestsData Data
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new AeroData();
                    }

                    return _instance;
                }
            }
            public List<Tuple<string, string, string>> StartupEnumeration()
            {
                return _startupEnumeration;
            }

            public List<Tuple<string, string, string>> StartupLoadedEvents()
            {
                return _startupLoadedEvents;
            }

            public List<Tuple<string, string, string>> ThemeChangedEnumeration()
            {
                return _themeChangedEnumeration_HC2;
            }

            public List<Tuple<string, string, string>> ThemeChangedLoadedEvents()
            {
                return _themeChangedLoadedEvents_HC2;
            }

            public List<Tuple<string, string, string>> ThemeChangedUnloadedEvents()
            {
                return _themeChangedUnloadedEvents_HC2;
            }

            private readonly List<Tuple<string, string, string>> _startupLoadedEvents = new List<Tuple<string, string, string>>()
            {
                // The base class of this test, WindowTest, would pre-load PresentationFramework, and therefore it
                // will not show up in the Loaded events.
                // Tuple.Create(PresentationFramework, PresentationFramework_Aero, PresentationFramework_Aero_AeroNormalColorBaml),
                Tuple.Create(MyButton, MyButton_Classic, MyButton_Classic_ClassicBaml),
                Tuple.Create(MyButton, MyButton_Generic, MyButton_Generic_GenericBaml),
                Tuple.Create(YourButton, YourButton, YourButton_GenericBaml)
            };

            private readonly List<Tuple<string, string, string>> _startupEnumeration = new List<Tuple<string, string, string>>()
            {
                Tuple.Create(PresentationFramework, PresentationFramework_Aero, PresentationFramework_Aero_AeroNormalColorBaml),
                Tuple.Create(MyButton, MyButton_Classic, MyButton_Classic_ClassicBaml),
                Tuple.Create(MyButton, MyButton_Generic, MyButton_Generic_GenericBaml),
                Tuple.Create(YourButton, YourButton, YourButton_GenericBaml)
            };

            private readonly List<Tuple<string, string, string>> _themeChangedUnloadedEvents_HC2 = new List<Tuple<string, string, string>>()
            {
                Tuple.Create(PresentationFramework, PresentationFramework_Aero, PresentationFramework_Aero_AeroNormalColorBaml),
                Tuple.Create(MyButton, MyButton_Classic, MyButton_Classic_ClassicBaml)
            };

            private readonly List<Tuple<string, string, string>> _themeChangedLoadedEvents_HC2 = new List<Tuple<string, string, string>>()
            {
                Tuple.Create(PresentationFramework, PresentationFramework_Classic, PresentationFramework_Classic_ClassicBaml),
                Tuple.Create(MyButton, MyButton_Classic, MyButton_Classic_ClassicBaml)
            };

            private readonly List<Tuple<string, string, string>> _themeChangedEnumeration_HC2 = new List<Tuple<string, string, string>>()
            {
                Tuple.Create(PresentationFramework, PresentationFramework_Classic, PresentationFramework_Classic_ClassicBaml),
                Tuple.Create(MyButton, MyButton_Generic, MyButton_Generic_GenericBaml),
                Tuple.Create(MyButton, MyButton_Classic, MyButton_Classic_ClassicBaml),
                Tuple.Create(YourButton, YourButton, YourButton_GenericBaml)
            };
        }

        private class Aero2Data : IResourceDictionaryTestsData
        {
            private static Aero2Data _instance;

            private Aero2Data() { }

            public static IResourceDictionaryTestsData Data
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new Aero2Data();
                    }

                    return _instance;
                }
            }

            public List<Tuple<string, string, string>> StartupEnumeration()
            {
                return _startupEnumeration;
            }

            public List<Tuple<string, string, string>> StartupLoadedEvents()
            {
                return _startupLoadedEvents;
            }

            public List<Tuple<string, string, string>> ThemeChangedEnumeration()
            {
                return _themeChangedEnumeration_HC2;
            }

            public List<Tuple<string, string, string>> ThemeChangedLoadedEvents()
            {
                return _themeChangedLoadedEvents_HC2;
            }

            public List<Tuple<string, string, string>> ThemeChangedUnloadedEvents()
            {
                return _themeChangedUnloadedEvents_HC2;
            }

            private readonly List<Tuple<string, string, string>> _startupLoadedEvents = new List<Tuple<string, string, string>>()
            {
                // The base class of this test, WindowTest, would pre-load PresentationFramework, and therefore it
                // will not show up in the Loaded events.
                // Tuple.Create(PresentationFramework, PresentationFramework_Aero2, PresentationFramework_Aero2_Aero2NormalcolorBaml),
                Tuple.Create(MyButton, MyButton_Aero2, MyButton_Aero2_Aero2NormalColorBaml),
                Tuple.Create(MyButton, MyButton_Generic, MyButton_Generic_GenericBaml),
                Tuple.Create(YourButton, YourButton, YourButton_Aero2NormalColorBaml)
            };

            private readonly List<Tuple<string, string, string>> _startupEnumeration = new List<Tuple<string, string, string>>()
            {
                Tuple.Create(PresentationFramework, PresentationFramework_Aero2, PresentationFramework_Aero2_Aero2NormalcolorBaml),
                Tuple.Create(MyButton, MyButton_Aero2, MyButton_Aero2_Aero2NormalColorBaml),
                Tuple.Create(MyButton, MyButton_Generic, MyButton_Generic_GenericBaml),
                Tuple.Create(YourButton, YourButton, YourButton_Aero2NormalColorBaml)
            };

            private readonly List<Tuple<string, string, string>> _themeChangedUnloadedEvents_HC2 = new List<Tuple<string, string, string>>()
            {
                Tuple.Create(PresentationFramework, PresentationFramework_Aero2, PresentationFramework_Aero2_Aero2NormalcolorBaml),
                Tuple.Create(MyButton, MyButton_Aero2, MyButton_Aero2_Aero2NormalColorBaml),
                Tuple.Create(YourButton, YourButton, YourButton_Aero2NormalColorBaml)
            };

            private readonly List<Tuple<string, string, string>> _themeChangedLoadedEvents_HC2 = new List<Tuple<string, string, string>>()
            {
                Tuple.Create(PresentationFramework, PresentationFramework_Classic, PresentationFramework_Classic_ClassicBaml),
                Tuple.Create(MyButton, MyButton_Classic, MyButton_Classic_ClassicBaml),
                Tuple.Create(YourButton, YourButton, YourButton_GenericBaml)
            };

            private readonly List<Tuple<string, string, string>> _themeChangedEnumeration_HC2 = new List<Tuple<string, string, string>>()
            {
                Tuple.Create(PresentationFramework, PresentationFramework_Classic, PresentationFramework_Classic_ClassicBaml),
                Tuple.Create(MyButton, MyButton_Generic, MyButton_Generic_GenericBaml),
                Tuple.Create(MyButton, MyButton_Classic, MyButton_Classic_ClassicBaml),
                Tuple.Create(YourButton, YourButton, YourButton_GenericBaml)
            };
        }

        #endregion // TestData
#pragma warning restore 414
    }

    internal static class ResourceDictionaryInfoPredicate
    {
        public static Predicate<ResourceDictionaryInfo> Comparer(string assemblyName, string dictionaryAssemblyName, string sourceUri, TestLog log)
        {
            var predicate = new Predicate<ResourceDictionaryInfo>(
                (ResourceDictionaryInfo rdInfo) =>
                {
                    if ((rdInfo == null) || (rdInfo.Assembly == null) || (rdInfo.ResourceDictionaryAssembly == null) || (rdInfo.SourceUri == null))
                    {
                        // log.LogStatus("\tResult: false");
                        return false;
                    }

                    bool result = true;

                    if (!(string.Equals(rdInfo.Assembly.GetName().Name, assemblyName, StringComparison.InvariantCultureIgnoreCase) &&
                        string.Equals(rdInfo.ResourceDictionaryAssembly.GetName().Name, dictionaryAssemblyName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        result = false;
                    }

                    if (!ResourceDictionaryInfoPredicate.ArePackUrisEquivalent(rdInfo.SourceUri.AbsoluteUri, sourceUri))
                    {
                        result = false;
                    }

                    return result;
                });

            return predicate;
        }

        /// <summary>
        /// ResourceDictionaryInfo.SourceUri is of the form:
        ///   pack://application:,,,/MyButton.Aero2;v1.0.0.0;component/themes/aero2.normalcolor.baml
        ///     These are separated by semicolons (;) into 3 parts
        ///      i.   Base Uri to assembly
        ///      ii.  version
        ///      iii. Relative path to baml
        ///  We will compare (i) and (iii), but ignore the version (ii)
        /// </summary>
        /// <param name="uri1"></param>
        /// <param name="uri2"></param>
        /// <returns></returns>
        private static bool ArePackUrisEquivalent(string uri1, string uri2)
        {
            var uri1Parts = uri1.Split(';');
            var uri2Parts = uri2.Split(';');

            if ((uri1Parts.Length != 3) || (uri2Parts.Length != 3))
            {
                return false;
            }

            if (!string.Equals(uri1Parts[0], uri2Parts[0], StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(uri1Parts[2], uri2Parts[2], StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }

}
