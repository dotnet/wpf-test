// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Markup.Localizer;
using System.Windows.Markup;
using System.Windows.Threading;

using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Globalization
{
   /* [Test(0, "Markup.Localizer", "LocalizabilityAttributeTest", SupportFiles = @"FeatureTests\Globalization\Data\tree.xaml", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class LocalizabilityAttribute : StepsTest
    {
        
        
        static Localizabilityattributes()
        {
            PackUriHelper.RegisterPackScheme();
        }

       
 
            //Create Context here.
            LogComment("TestLocAttribute Started ..." + "\n");
            CreateContext();
            UIElement root = CommonLib.LoadXamlFromSiteOfOrigin("tree.xaml");
            GetElementAttribute(root);
            LogComment(true, "TestLocAttribute Test Pass!...");
            _log.Stage(TestStage.Cleanup);
        }
        private static void GetElementAttribute(UIElement root)
        {
            if (root == null)
                return;
            FrameworkElement fe = root as FrameworkElement;
            Type t = fe.GetType();
//
            if (t.Equals(typeof(Hyperlink)) == true)
            {
                LogComment("Checking Type " + t.ToString());
                foreach (Attribute attr in t.GetCustomAttributes(typeof(LocalizabilityAttribute), true))
                {
                    System.Windows.LocalizabilityAttribute loc = attr as System.Windows.LocalizabilityAttribute;
                    if (loc != null)
                    {
                        LogComment("Checking Loc Attribute.");
                        LogComment("Category: " + loc.Category.ToString());
                        if (loc.Category != System.Windows.LocalizationCategory.Hyperlink)
                            LogComment(false, "Category " + loc.Category.ToString());
                        LogComment("Readability: " + loc.Readability.ToString());
                        if (loc.Readability != Readability.Inherit)
                            LogComment(false, "Readability " + loc.Readability.ToString());
                        LogComment("Modifiability: " + loc.Modifiability.ToString());
                        if (loc.Modifiability != Modifiability.Inherit)
                            LogComment(false, "Modifiability " + loc.Modifiability.ToString());
                    }
                }

                foreach (PropertyInfo pi in t.GetProperties())
                {
                    //                    LogComment("Property " + pi.ToString());
                    if (String.Compare(pi.ToString(), "System.String FontFamily", false, CultureInfo.InvariantCulture) == 0)
                    {
                        LogComment("Checking Property " + pi.ToString());
                        foreach (Attribute a in pi.GetCustomAttributes(typeof(LocalizabilityAttribute), true))
                        {
                            System.Windows.LocalizabilityAttribute loc = a as System.Windows.LocalizabilityAttribute;
                            if (loc != null)
                            {
                                LogComment("Checking Font Category.");
                                LogComment("Category: " + loc.Category.ToString());
                                if (loc.Category != System.Windows.LocalizationCategory.Font)
                                    LogComment(false, "Category " + loc.Category.ToString());
                                LogComment("Readability: " + loc.Readability.ToString());
                                if (loc.Readability != Readability.Inherit)
                                    LogComment(false, "loc.Readability " + loc.Readability.ToString());
                                LogComment("Modifiability: " + loc.Modifiability.ToString());
                                if (loc.Modifiability != Modifiability.Unmodifiable)
                                    LogComment(false, "Modifiability " + loc.Modifiability.ToString());
                            }
                        }
                    }
                }
            }
//
            int count = VisualTreeHelper.GetChildrenCount(root);
            for(int i = 0; i < count; i++)
            {
                GetElementAttribute(VisualTreeHelper.GetChild(root,i) as UIElement);
            }
        } 


*/


    [Test(0, "Markup.Localizer.Locbaml", "LocalizabilityAttributeTest", SupportFiles = @"FeatureTests\Globalization\Data\tree.baml", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class TestLocBaml : StepsTest
    {
        
        private BamlLocalizationDictionary _dic;
        private Stream _stream;
	private bool _instanceexists;

        public TestLocBaml()      
 
        {
            InitializeSteps += new TestStep(LocBamlDicCountCheck);
            RunSteps += new TestStep(LocBamlAttrCheck);
        }

        TestResult LocBamlDicCountCheck()
        {

            //Create Context here.
            LogComment("TestLocBaml Started ..." + "\n");
            CreateContext();
            LogComment("Parsing tree.baml....");
            string tree = CommonLib.DumpBamlToString("tree.baml");
	    Console.WriteLine(tree);
            _stream = File.OpenRead("tree.baml");
            BamlLocalizer locmgr = new BamlLocalizer(_stream, new BamlLocalizabilityByReflection());
            _dic = locmgr.ExtractResources();
            LogComment("Checking Resource count...");
            if (_dic.Count != 56)
            {
                LogComment("Resources Count should be 56 but for some reason is: " + _dic.Count.ToString());
                _stream.Close();
                return TestResult.Fail;
            }
            else
            {
              LogComment("IsFixedSize " + _dic.IsFixedSize.ToString());
              LogComment("IsReadOnly " + _dic.IsReadOnly.ToString());
              return TestResult.Pass;
            }
         }
        TestResult LocBamlAttrCheck()
        {
              _instanceexists=false;
	      foreach (DictionaryEntry entry in _dic)
              {
                BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
                // checking Hyperlink
		LogComment("Key: " + string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName));
                LogComment(resource.Category.ToString());
                LogComment(resource.Readable.ToString());
                LogComment(resource.Modifiable.ToString());
                LogComment("Comments: " + resource.Comments);
                LogComment("Content: " + resource.Content);
		if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains("System.Windows.Documents.Hyperlink.NavigateUri"))
                {
                    _instanceexists=true;
                    LogComment("check to verify if Category is set correctly.");
                    if (resource.Category != System.Windows.LocalizationCategory.Hyperlink)
                    {
                        LogComment("Category " + resource.Category.ToString());
                        return TestResult.Fail;
                    }
                    if (!resource.Readable)
                    {
                        LogComment("Readable " + resource.Readable.ToString());
                        return TestResult.Fail;
                    }
                    if (!resource.Modifiable)
                    {
                        LogComment("Modifiable " + resource.Modifiable.ToString());
                        return TestResult.Fail;      
                    } 
                    if (!resource.Content.Equals("ControlsPage2.xaml", StringComparison.InvariantCulture))
                    {
                        LogComment("Content: " + resource.Content);
                        return TestResult.Fail;
                    }
		
                }
              }

              _stream.Close();
	      if (_instanceexists==true)
              {
              	LogComment("TestLocBaml Test Pass...");
              	return TestResult.Pass;
              }
              else
	      {
		LogComment("TestLocBaml Test Fail. Hyperlink was not found among dictionary entries...");
              	return TestResult.Fail;
              }
	      
            }
            
        



	

        static Dispatcher Dispatcher;
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

    }
}

