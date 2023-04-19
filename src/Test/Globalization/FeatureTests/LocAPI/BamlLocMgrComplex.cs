// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections;
using System.Resources;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using System.Globalization;

using System.Windows;
using System.Windows.Baml2006;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup.Localizer;
using System.Windows.Markup;
using System.Xaml;

using Microsoft.Test;
using Microsoft.Test.Serialization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;



namespace Microsoft.Test.Globalization
{
    /// <summary>
    /// <description>
    /// Test BAML Localization Manager
    /// </description>
    /// </summary>

    [Test(0, "Markup.Localizer", "BamlLocMgrRead", SupportFiles = @"FeatureTests\Globalization\Data\SimpleOne.baml", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite,MicroSuite")]
    public class BamlLocMgrRead : StepsTest
    {

        public BamlLocMgrRead()
        {
            InitializeSteps += new TestStep(ReadBaml);

        }


        TestResult ReadBaml()
        {
            Stream stream = File.OpenRead("SimpleOne.baml");
            string tgt = "GlobTempTarget.baml";
            int startT = 0;
            int endT = 0;
            int dicCount = 0;
            CreateContext();
            LogComment("Parsing Baml: SimpleOne.baml");
            startT = Environment.TickCount;
            BamlLocalizer locmgr = new BamlLocalizer(stream, null);
            endT = Environment.TickCount;
            LogComment("Parse time = " + (endT - startT) + " ms");
            BamlLocalizationDictionary dic = locmgr.ExtractResources();
            dicCount = dic.Count;
            LogComment("Dictionary Count: " + dicCount.ToString());
            BamlLocalizationDictionary dictionary = new BamlLocalizationDictionary();
            foreach (DictionaryEntry entry in dic)
            {
                BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
                LogComment("Adding Key " + string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName) + " and Value to new Dictionary.");
                dictionary.Add((BamlLocalizableResourceKey)entry.Key, resource);
            }
            stream.Close();
            LogComment("Deleting Temp File if it existed.");
            if (File.Exists(tgt))
            {
                try
                {
                    File.Delete(tgt);
                }
                catch (Exception e)
                {
                    LogComment("Failed to clean up file..." + e.ToString());
                }
            }
            LogComment("Creating New Baml. " + tgt);
            Stream target = new FileStream(tgt, FileMode.Create);
            LogComment("Generating new Resource BAML. " + tgt);
            locmgr.UpdateBaml(target, dictionary);
            target.Close();
            stream = File.OpenRead(tgt);
            locmgr = new BamlLocalizer(stream, null);
            BamlLocalizationDictionary _dic = locmgr.ExtractResources();
            LogComment("Checking key count...");
            if (dicCount != _dic.Count)
            {
                LogComment("Dictionary count between Original Baml and newly created are different. Original Value: " + dicCount.ToString() + " , New BAML Value: " + _dic.Count.ToString());
                stream.Close();
                return TestResult.Fail;
            }
            stream.Close();
            LogComment("Comparing Baml...");
            if (BamlHelper.CompareBamlFiles("SimpleOne.baml", tgt).Count > 0)
            {
                LogComment("Not the same...");
                stream.Close();
                return TestResult.Fail;
            }

            LogComment("TestReadComplex: Pass....");
            stream.Close();
            return TestResult.Pass;



        }

        static Dispatcher s_dispatcher;
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;

        }

    }

    [Test(0, "Markup.Localizer", "BamlLocMgrReadComplex", SupportFiles = @"FeatureTests\Globalization\Data\Complex.baml", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite", Disabled = true)]
    public class BamlLocMgrReadComplex : StepsTest
    {

        public BamlLocMgrReadComplex()
        {
            InitializeSteps += new TestStep(ReadComplex);

        }


        TestResult ReadComplex()
        {

            string tgt = "GlobTempTarget.baml";
            int startT = 0;
            int endT = 0;
            int dicCount = 0;
            LogComment("Parsing Baml: Complex.baml");
            Stream stream = File.OpenRead("Complex.baml");
            CreateContext();
            startT = Environment.TickCount;
            BamlLocalizer locmgr = new BamlLocalizer(stream, new BamlLocalizabilityByReflection());
            endT = Environment.TickCount;
            LogComment("Parse time = " + (endT - startT) + " ms");
            BamlLocalizationDictionary dic = locmgr.ExtractResources();
            dicCount = dic.Count;
            LogComment("Dictionary Count: " + dicCount.ToString());
            BamlLocalizationDictionary dictionary = new BamlLocalizationDictionary();
            foreach (DictionaryEntry entry in dic)
            {
                BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
                LogComment("Adding Key " + string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName) + " and Value to new Dictionary.");
                dictionary.Add((BamlLocalizableResourceKey)entry.Key, resource);
            }
            stream.Close();

            LogComment("Creating New Baml. " + tgt);
            Stream target = new FileStream(tgt, FileMode.Create);
            LogComment("Generating new Resource BAML. " + tgt);
            locmgr.UpdateBaml(target, dictionary);
            target.Close();
            stream = File.OpenRead(tgt);
            locmgr = new BamlLocalizer(stream, new BamlLocalizabilityByReflection());
            BamlLocalizationDictionary _dic = locmgr.ExtractResources();
            LogComment("Checking key count..." + _dic.Count.ToString());

            if (dicCount != _dic.Count)
            {
                LogComment("Dictionary count between Original Baml and newly created are different. Original Value: " + dicCount.ToString() + " , New BAML Value: " + _dic.Count.ToString());
                stream.Close();
                return TestResult.Fail;
            }

            LogComment("Comparing Baml...");
            if (BamlHelper.CompareBamlFiles("Complex.baml", tgt).Count > 0)
            {
                LogComment("Not the same...");
                stream.Close();
                return TestResult.Fail;
            }
            else
            {
                string tree = CommonLib.DumpBamlToString(tgt);
                Console.WriteLine(tree);
                LogComment("TestReadComplex: Pass....");
                stream.Close();
                return TestResult.Pass;
            }
        }

        static Dispatcher s_dispatcher;
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;

        }

    }

    [Test(0, "Markup.Localizer", "BamlLocCustomControl", SupportFiles = @"FeatureTests\Globalization\Data\MultilangControl.baml", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class BamlLocCustomControl : StepsTest
    {
        private string _src;
        private string _tgt;
        private BamlLocalizer _locmgr;
        private Stream _stream;
        private Assembly[] _assemblies;
        private int _dicCount;
        private BamlLocalizationDictionary _dictionary;

        public BamlLocCustomControl()
        {
            InitializeSteps += new TestStep(SetupFirstDicCompare);
            RunSteps += new TestStep(SecondDicCompare);
            RunSteps += new TestStep(BamlCompare);
        }


        TestResult SetupFirstDicCompare()
        {
            string WorkDir = Environment.GetEnvironmentVariable("%WORKDIR%");
            if (WorkDir != null && WorkDir.Length != 0)
            {
                WorkDir = Environment.ExpandEnvironmentVariables(WorkDir);
                Directory.SetCurrentDirectory(WorkDir);
            }
            else
            {
                WorkDir = Directory.GetCurrentDirectory();
            }

            LogComment("TestCustomControl Start.....");
            _src = "MultiLangControl.baml";
            _tgt = "GlobTempTarget.baml";
            int startT = 0;
            int endT = 0;
            _dicCount = 0;

            LogComment("Parsing Baml: " + _src);
            _stream = File.OpenRead(_src);
            CreateContext();
            startT = Environment.TickCount;
            _assemblies = new Assembly[1];
            _assemblies[0] = Assembly.LoadFrom(WorkDir + "\\GlobalizationTestCommon.dll");
            _locmgr = new BamlLocalizer(_stream, new BamlLocalizabilityByReflection(_assemblies));
            endT = Environment.TickCount;

            LogComment("Parse time = " + (endT - startT) + " ms");
            BamlLocalizationDictionary dic = _locmgr.ExtractResources();
            _dicCount = dic.Count;

            LogComment("Compare Dic.Count.");
            if (_dicCount != 65)
            {
                LogComment("Dic Count is not equal 65. Actual is " + _dicCount.ToString());
                _stream.Close();
                return TestResult.Fail;
            }
            else
            {
                _dictionary = new BamlLocalizationDictionary();
                foreach (DictionaryEntry en in dic)
                {
                    BamlLocalizableResource res = (BamlLocalizableResource)en.Value;
                    LogComment("Key: " + string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)en.Key).Uid, ((BamlLocalizableResourceKey)en.Key).ClassName, ((BamlLocalizableResourceKey)en.Key).PropertyName));
                    LogComment("Category: " + res.Category.ToString());
                    LogComment("Comments: " + res.Comments);
                    LogComment("Modifiable: " + res.Modifiable.ToString());
                    LogComment("Readable: " + res.Readable.ToString());
                    LogComment("Content: " + res.Content + "\n");
                    LogComment("Adding Key and Value to new Dictionary.");
                    _dictionary.Add((BamlLocalizableResourceKey)en.Key, res);
                }
                _stream.Close();
                return TestResult.Pass;
            }
        }

        TestResult SecondDicCompare()
        {

            LogComment("Creating New Baml. " + _tgt);
            Stream target = new FileStream(_tgt, FileMode.Create);

            LogComment("Generating new Resource BAML. " + _tgt);
            _locmgr.UpdateBaml(target, _dictionary);
            target.Close();
            _stream = File.OpenRead(_tgt);
            _locmgr = new BamlLocalizer(_stream, new BamlLocalizabilityByReflection(_assemblies));
            BamlLocalizationDictionary _dic = _locmgr.ExtractResources();
            LogComment("Checking key count...");
            if (_dicCount != _dic.Count)
            {
                LogComment("Dictionary count between Original Baml and newly created are different. Original Value: " + _dicCount.ToString() + " , New BAML Value: " + _dic.Count.ToString());
                _stream.Close();
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }
        TestResult BamlCompare()
        {

            LogComment("Comparing Baml..." + _src + " and " + _tgt);
            if (BamlHelper.CompareBamlFiles(_src, _tgt).Count > 0)
            {
                LogComment("Not same...");
                return TestResult.Fail;
            }
            else
            {
                LogComment("TestComplexBaml: Pass....");
                _stream.Close();
                return TestResult.Pass;
            }

        }
        static Dispatcher s_dispatcher;
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;

        }

    }

    [Test(0, "Markup.Localizer", "SolidColorBrushSerialization", SupportFiles = @"FeatureTests\Globalization\Data\SolidColorBrushSerializationTest.baml", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite,MicroSuite", Versions = "4.6+")]
    public class SolidColorBrushSerialization : StepsTest
    {
        private const string bamlFileName = "SolidColorBrushSerializationTest.baml";
        private const string newBamlFileName = "SolidColorBrushSerializationTest_new.baml";

        private Brush _originalBackgroundBrush;
        private Brush _originalForegroundBrush;

        private Brush _localizedBackgroundBrush;
        private Brush _localizedForegroundBrush;

        public SolidColorBrushSerialization()
        {
            InitializeSteps += new TestStep(ExtractSCBsFromOriginalBaml);
            RunSteps += new TestStep(LocalizeAndSaveBaml);
            RunSteps += new TestStep(ExtractSCBsFromNewBaml);
            RunSteps += new TestStep(ValidateBrushes);
        }

        TestResult ExtractSCBsFromOriginalBaml()
        {
            try
            {
                LogComment("Reading original BAML from " + bamlFileName);
                FileStream bamlStream = new FileStream(bamlFileName, FileMode.Open);

                LogComment("Extracting Window Background property");
                Window window = SolidColorBrushSerialization.CreateWindowFromBamlStream(bamlStream);

                _originalBackgroundBrush = window.Background;
                LogComment("Original Background Brush color value is " + _originalBackgroundBrush);

                _originalForegroundBrush = window.Foreground;
                LogComment("Original Foreground Brush color value is " + _originalForegroundBrush);

                bamlStream.Close();

                return TestResult.Pass;

            }
            catch (Exception e)
            {
                LogComment("FAIL: Exception encountered: " + e.ToString());
                return TestResult.Fail;
            }
        }

        TestResult LocalizeAndSaveBaml()
        {
            // No real localization is done in this method.
            // We simply call BamlLocalizer.UpdateBaml which will in turn write the SCB back into the BAML
            // using SolidColorBrush.SerializeOn.

            try
            {

                LogComment("Localizing and re-saving the BAML as " + newBamlFileName);

                FileStream bamlStream = new FileStream(bamlFileName, FileMode.Open);
                BamlLocalizer localizer = new BamlLocalizer(bamlStream);

                FileStream newBamlStream = new FileStream(newBamlFileName, FileMode.Create, FileAccess.ReadWrite);

                // Nothing to localize here - we just want to roundtrip the BAML to ensure that the SolidColorBrush comes back the same
                localizer.UpdateBaml(newBamlStream, new BamlLocalizationDictionary());

                bamlStream.Close();
                newBamlStream.Close();

                return TestResult.Pass;
            }
            catch (Exception e)
            {
                LogComment("FAIL: Exception encountered: " + e.ToString());
                return TestResult.Fail;
            }
        }

        TestResult ExtractSCBsFromNewBaml()
        {
            try
            {
                LogComment("Reading new BAML from " + newBamlFileName);
                FileStream newBamlStream = new FileStream(newBamlFileName, FileMode.Open);

                LogComment("Extracting Window Background property");
                Window window = SolidColorBrushSerialization.CreateWindowFromBamlStream(newBamlStream);

                _localizedBackgroundBrush = window.Background;
                LogComment("Localized Background Brush color value is " + _localizedBackgroundBrush);

                _localizedForegroundBrush = window.Foreground;
                LogComment("Localized Foreground Brush color value is " + _localizedForegroundBrush);

                newBamlStream.Close();

                return TestResult.Pass;
            }
            catch (Exception e)
            {
                LogComment("FAIL: Exception encountered: " + e.ToString());
                return TestResult.Fail;
            }
        }

        TestResult ValidateBrushes()
        {
            TestResult result = TestResult.Pass;

            LogComment("Validating Brushes");

            if ((_originalBackgroundBrush.ToString().ToUpper() != "#00FFFFFF") || (_localizedBackgroundBrush.ToString().ToUpper() != "#00FFFFFF"))
            {
                LogComment("FAIL: Foreground Brushes do not have color value of #00FFFFFF. Was the text XAML modified out of sync with this code?");
                result = TestResult.Fail;
            }
            else
            {
                LogComment("PASS: Localized Foreground Brush has color value #00FFFFFF");
            }

            if ((_originalForegroundBrush.ToString().ToUpper() != "#FF000000") || (_localizedForegroundBrush.ToString().ToUpper() != "#FF000000"))
            {
                LogComment("FAIL: Background Brushes do not have color value of #FF000000. Was the text XAML modified out of sync with this code?");
                result = TestResult.Fail;
            }
            else
            {
                LogComment("PASS: Localized Background Brush has color value #FF000000");
            }

            if ((_originalBackgroundBrush != Brushes.Transparent) || (_localizedBackgroundBrush != Brushes.Transparent))
            {
                LogComment("FAIL: Foreground Brushes are not identical (by ref) to Brushes.Transparent");
                result = TestResult.Fail;
            }
            else
            {
                LogComment("PASS: Foreground Brushes are identical (by ref) to Brushes.Transparent");
            }

            if ((_originalBackgroundBrush == Brushes.Black) || (_localizedBackgroundBrush == Brushes.Black))
            {
                if (_originalBackgroundBrush == Brushes.Black)
                    LogComment("FAIL: Original Background Brush is identical (by ref) to Brushes.Black");
                if (_localizedBackgroundBrush == Brushes.Black)
                    LogComment("FAIL: Localized Background Brush is identical (by ref) to Brushes.Black");

                result = TestResult.Fail;
            }
            else
            {
                LogComment("PASS: Localized Background Brush is not identical (by ref) to Brushes.Black");
            }

            return result;
        }
        private static Window CreateWindowFromBamlStream(Stream bamlStream)
        {
            Baml2006Reader reader = new Baml2006Reader(bamlStream);

            XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);
            XamlServices.Transform(reader, writer);

            Window window = (Window)writer.Result;

            return window;
        }

    }
}
