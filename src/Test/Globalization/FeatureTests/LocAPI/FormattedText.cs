// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#region Using
using System;
using System.Resources;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Threading;
using System.Globalization;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup.Localizer;
using System.Windows.Markup;

using Microsoft.Test.Serialization;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

#endregion

namespace Microsoft.Test.Globalization
{

    [Test(0, "Markup.Localizer.FormattedText", "SimpleFormattedText", SupportFiles = @"FeatureTests\Globalization\Data\FormattedText.baml", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords="Localization_Suite")]
    public class SimpleFormattedText : StepsTest
    {


        public SimpleFormattedText()
        {
            InitializeSteps += new TestStep(RunSimpleFormattedText);

        }

        TestResult RunSimpleFormattedText()
        {

            string key2 = "TextBlock_5:System.Windows.Controls.TextBlock.$Content";
            string key3 = "Block_1:System.Windows.Documents.Block.$Content";

            LogComment("SimpleFormatTest Start.....");
            string tree = CommonLib.DumpBamlToString("FormattedText.baml");
            Console.WriteLine(tree);
            Stream stream = File.OpenRead("FormattedText.baml");
            int startT = 0;
            int endT = 0;
            int dicCount = 0;
            CreateContext();
            LogComment("Parsing Baml: FormattedText.baml");

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
		LogComment("Key: " + string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName));
                LogComment("Category: " + resource.Category.ToString());
                LogComment("Modifiable: " + resource.Modifiable.ToString());
                LogComment("Readable: " + resource.Readable.ToString());
                LogComment("Comments: " + resource.Comments);
                LogComment("Content: " + resource.Content);
            	if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains(key2))

                {
                    string expected = "Basic Text <b Uid=\"Bold_1\">D</b>ecorations with &quot;XAML&quot;";
                    if (!resource.Content.Equals(expected, StringComparison.InvariantCulture))
                    {
                        LogComment("Actual Content: " + resource.Content + "\nExpected: " + expected);
                        return TestResult.Fail;
                    }
                }
                if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains(key3))
                {
                    string expected = "Normal <in Uid=\"Inline_1\">superscript</in> <in Uid=\"Inline_2\">subscript</in> 1<in Uid=\"Inline_3\">st</in> H<in Uid=\"Inline_4\">2</in>SO<in Uid=\"Inline_5\">4</in>";
                    //String expected = "Normal <Run x:Uid="Inline_1" Typography.Variants="Superscript">superscript</Run>"
		    if (!resource.Content.Equals(expected, StringComparison.InvariantCulture))
                    {
                        LogComment("Actual Content: " + resource.Content + "\nExpected: " + expected);
                        return TestResult.Fail;
                    }
                }
            }
            LogComment("SimpleFormatTest Test Pass!...");
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


    [Test(0, "Markup.Localizer.FormattedText", "WriteFormattedText", SupportFiles = @"FeatureTests\Globalization\Data\FormattedText.baml", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords="Localization_Suite")]
    public class WriteFormattedText : StepsTest
    {
        private string _tgt;
        private Stream _stream;
        private BamlLocalizer _locmgr;
        private BamlLocalizationDictionary _dic;
        private int _dicCount;
        private BamlLocalizationDictionary _dictionary;

        public WriteFormattedText()
        {
            InitializeSteps += new TestStep(RunWriteFormattedText1);

        }

        TestResult RunWriteFormattedText1()
        {
   
            _tgt = "GlobTempTarget.baml";
            string key2 = "TextBlock_5:System.Windows.Controls.TextBlock.$Content";
	    string key3 = "Block_1:System.Windows.Documents.Block.$Content";
            LogComment("WriteFormatTest Start.....");
            LogComment("Parsing Baml: FormattedText.baml");

            _stream = File.OpenRead("FormattedText.baml");
            int startT = 0;
            int endT = 0;
            _dicCount = 0;
            CreateContext();
            startT = Environment.TickCount;
            _locmgr = new BamlLocalizer(_stream, new BamlLocalizabilityByReflection());
            endT = Environment.TickCount;
            LogComment("Parse time = " + (endT - startT) + " ms");

            _dic = _locmgr.ExtractResources();
            _dicCount = _dic.Count;
            LogComment("Dictionary Count: " + _dicCount.ToString());
            _dictionary = new BamlLocalizationDictionary();
            foreach (DictionaryEntry entry in _dic)
            {
                BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
		if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains(key2))
                {
                    string expected = "Basic Text <b Uid=\"Bold_1\">D</b>ecorations with &quot;XAML&quot;";
                    if (resource.Content.Equals(expected, StringComparison.InvariantCulture))
                    {
                        LogComment("Change Formatted Text content.");
                        resource.Content = "&lt;&gt;&quot;&apos;&amp; <b Uid=\"Bold_1\">" + Extract.GetTestString(0,1) + "</b>" + Extract.GetTestString(0);
                    }
                    else
                    {
                        LogComment("Actual Content: " + resource.Content + "\nExpected: " + expected);
                        return TestResult.Fail;
                    }
                }
		if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains(key3))
                {
                    string expected = "Normal <in Uid=\"Inline_1\">superscript</in> <in Uid=\"Inline_2\">subscript</in> 1<in Uid=\"Inline_3\">st</in> H<in Uid=\"Inline_4\">2</in>SO<in Uid=\"Inline_5\">4</in>";
                    // string expected = "Normal <Run x:Uid="Inline_1" Typography.Variants="Superscript">superscript</Run>";
		    if (resource.Content.Equals(expected, StringComparison.InvariantCulture))
                    {
                        LogComment("Change Formatted Text content.");
                        resource.Content = Extract.GetTestString(0) + "<in Uid=\"Inline_1\">" + Extract.GetTestString(0) + "</in> <in Uid=\"Inline_2\">subscript</in> 1<in Uid=\"Inline_3\">st</in> H<in Uid=\"Inline_4\">2</in>SO<in Uid=\"Inline_5\">4</in>";
                    }
                    else
                    {
                        LogComment("Actual Content: " + resource.Content + "\nExpected: " + expected);
                        return TestResult.Fail;
                    }
                }

		LogComment("Adding Key " + string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName) + " and Value to new Dictionary.");
                _dictionary.Add((BamlLocalizableResourceKey)entry.Key, resource);
            }
            _stream.Close();
            return TestResult.Pass;
        }
        TestResult RunWriteFormattedText2()
        {
           
            LogComment("Creating New Baml. " + _tgt);
            Stream target = new FileStream(_tgt, FileMode.Create);

            LogComment("Generating new Resource BAML. " + _tgt);
            _locmgr.UpdateBaml(target, _dictionary);
            target.Close();
            _stream = File.OpenRead(_tgt);
            _locmgr = new BamlLocalizer(_stream, new BamlLocalizabilityByReflection());
            BamlLocalizationDictionary _dic = _locmgr.ExtractResources();
            LogComment("Checking key count...");
            if (_dicCount != _dic.Count)
            {
                LogComment("Dictionary count between Original Baml and newly created are different. Original Value: " + _dicCount.ToString() + " , New BAML Value: " + _dic.Count.ToString());
                return TestResult.Fail;
            }
            _stream.Close();

            LogComment("Comparing Baml...FormattedText.baml and " + _tgt);
            if (BamlHelper.CompareBamlFiles("FormattedText.baml", _tgt).Count == 0)
            {
                LogComment("Same...");
                return TestResult.Fail;
            }
            else
            {
            LogComment("WriteFormattedText test: Pass....");
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

    [Test(0, "Markup.Localizer.FormattedText", "ElementLocalizabilityTest", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords="Localization_Suite")]
    public class ElementLocalizabilityTest : StepsTest
    {
        private LocalizabilityAttribute _la;
        private LocalizabilityAttribute _la2;
        private ElementLocalizability _el;
        private ElementLocalizability _el2;

        public ElementLocalizabilityTest()
        {
            InitializeSteps += new TestStep(SetupAndAttributes1);
            RunSteps += new TestStep(FormattedTextCheck1);
            RunSteps += new TestStep(AttributesCheck2);
            RunSteps += new TestStep(FormattedTextCheck2);

        }

        TestResult SetupAndAttributes1()
        {

            LogComment("ElementLocalizabilityTest Start.....");
            LogComment("ElementLocalizability().....");
            _el = new ElementLocalizability();

            LogComment("LocalizabilityAttribute(Category)....");
            _la = new LocalizabilityAttribute(LocalizationCategory.NeverLocalize);
            _la2 = new LocalizabilityAttribute(LocalizationCategory.Inherit);

            LogComment("LocalizabilityAttribute.Modifiability....");
            _la.Modifiability = Modifiability.Modifiable;
            _la2.Modifiability = Modifiability.Inherit;

            LogComment("LocalizabilityAttribute.Readability....");
            _la.Readability = Readability.Unreadable;
            _la2.Readability = Readability.Inherit;

            LogComment("ElementLocalizability.Attribute....");
            _el.Attribute = _la;

            LogComment("ElementLocalizability.FormattingTag....");
            _el.FormattingTag = Extract.GetTestString(1);

            LogComment("Compare Attr...");
            if (_la != _el.Attribute)
            {
                LogComment("Attributes are different");
                return TestResult.Fail;
            }
            else
            {
            LogComment("Attributes are the same.");
            return TestResult.Pass;
            }
         }
        TestResult FormattedTextCheck1()
        {
            LogComment("Compare Formatted Text.");
            if (Extract.GetTestString(1) != _el.FormattingTag)
            {
                LogComment("Formatted text is not the same");
                return TestResult.Fail;
            }     
            else
            {
            LogComment("Formatted Text matches");
            return TestResult.Pass;
            }
        }
        TestResult AttributesCheck2()
        {
            LogComment("ElementLocalizability(format, attr).....");
            _el2 = new ElementLocalizability(Extract.GetTestString(0), _la2);
            LogComment("Compare Attr...");
            if (_la2 != _el2.Attribute)
            {
                LogComment("Attributes are not the same");
                return TestResult.Fail;
            }
            else
            {
            LogComment("Attributes are the same.");
            return TestResult.Pass;
            }
        }
        TestResult FormattedTextCheck2()
        {
            LogComment("Compare Formatted Text.");
            if (Extract.GetTestString(0) != _el2.FormattingTag)
	    { 
                LogComment("Formatted Text is not the same");
                return TestResult.Fail;
            }
            else
            {
            LogComment("ElementLocalizabilityTest: Pass....");
            return TestResult.Pass;
            }
        }
        
    }
}
