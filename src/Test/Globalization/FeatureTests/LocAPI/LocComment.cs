// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Resources;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Globalization;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup.Localizer;
using System.Windows.Markup;
using System.Windows.Threading;

using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Globalization
{
 //   public class LocComment
//    {

        
 //       static LocComment()
  //      {
   //         CommonLib.PackUriHelper.RegisterPackScheme();
  //      }
 

    [Test(0, "Markup.Localizer", "LocAttributeTest", SupportFiles = @"FeatureTests\Globalization\Data\loc.xaml", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class LocAttributeTest : StepsTest
    {

        private Window _win;
	private UIElement _root;
        private System.Windows.Controls.StackPanel _st;
        private string _attrProp;

        public LocAttributeTest()
        {
            PackUriHelper.RegisterPackScheme();
            InitializeSteps += new TestStep(SetUpWindow);
            RunSteps += new TestStep(GetStackPanel);
            RunSteps += new TestStep(SetAttributeButtonAndCompare);
            RunSteps += new TestStep(GetAttributeTextBlockAndCompare);
            RunSteps += new TestStep(SetAttributeTextBlockAndCompare);
        }

        TestResult SetUpWindow()
        {
        
            LogComment("LocAttributeTest Start: Parsing Loc.xaml....");
	    _root = CommonLib.LoadXamlFromSiteOfOrigin("Loc.xaml");
            _win = (Window)_root;
            if (_win == null)
            {
                LogComment("Can't get root Window.");
                return TestResult.Fail;
            }
	    else return TestResult.Pass;
        }

	TestResult GetStackPanel()
        {
            LogComment("Getting StackPanel.");
            _st = (System.Windows.Controls.StackPanel)(_win.Content);
            if (_st == null)
            {
                LogComment("Can't get StackPanel.");
                return TestResult.Fail;
            }
	    else return TestResult.Pass;
        }
	
	TestResult SetAttributeButtonAndCompare()
	{
            _attrProp = "$Content(Modifiable Unreadable Text) FontFamily(Unmodifiable Readable Font)";
            LogComment("Setting Localization.SetAttributes(button, " + _attrProp + ").");
            Localization.SetAttributes(CommonLib.FindElementByID("MyButton", _st), _attrProp);
            
	    LogComment("Getting Localization.GetAttributes(button).");
            string result = Localization.GetAttributes(CommonLib.FindElementByID("MyButton", _st));

            LogComment("Compare Get & Set Localizability.");
            if (String.Compare(result, _attrProp, false, CultureInfo.InvariantCulture) != 0)
	    {
		LogComment("LocAttributeTest Failed: Get Localizability is " + result);
		 return TestResult.Fail;
            }
	    else return TestResult.Pass;
         }
	 
         TestResult GetAttributeTextBlockAndCompare()
         {
            LogComment("Getting LocalizabilityAttribute from TextBlock. $Content    ( Unmodifiable Readable Text )");
            string result1 = Localization.GetAttributes(CommonLib.FindElementByID("MyTextBlock", _st));
            string expect = "$Content    ( Unmodifiable Readable Text )";
            
            LogComment("Compare Get Localizability. Expected: " + expect);
            if (expect.IndexOf(result1) >= 0)
            {
                LogComment("LocAttributeTest Failed: Get Localizability is " + result1);
                return TestResult.Fail;
            }
            else return TestResult.Pass;
         }
          TestResult SetAttributeTextBlockAndCompare()
          {  
            LogComment("Setting LocalizabilityAttribute to TextBlock.");
            Localization.SetAttributes(CommonLib.FindElementByID("MyTextBlock", _st), _attrProp);
            System.Windows.Controls.TextBlock tb = (System.Windows.Controls.TextBlock)(CommonLib.FindElementByID("MyTextBlock", _st));
            object o = tb.GetValue(Localization.AttributesProperty);
            LogComment("Compare Get Localizability.");
            if (String.Compare(o.ToString(), _attrProp, false, CultureInfo.InvariantCulture) != 0)
            {
                LogComment("LocAttributeTest Failed: Get Localizability is " + o.ToString());
		return TestResult.Fail;
            }
            else
            {
		LogComment("LocAttributeTest Passed");
		return TestResult.Pass;
            }
        }

    [Test(0, "Markup.Localizer", "LocCommentTest", SupportFiles = @"FeatureTests\Globalization\Data\loc.xaml", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class LocCommentTest : StepsTest
    {   
        private Window _win;
	private UIElement _root;
        private System.Windows.Controls.StackPanel _st;
        private string _attrProp;

        public LocCommentTest()
        {
            PackUriHelper.RegisterPackScheme();
	    InitializeSteps += new TestStep(SetUpWindow);
            RunSteps += new TestStep(GetStackPanel);
            RunSteps += new TestStep(SetButtonCommentAndCompare);
            RunSteps += new TestStep(GetTextBlockCommentAndCompare);
            RunSteps += new TestStep(SetTextBlockCommentAndCompare);

        }

        TestResult SetUpWindow()
        {

            LogComment("LocCommentTest Start: Parsing Loc.xaml....");            
            _root = CommonLib.LoadXamlFromSiteOfOrigin("Loc.xaml");
            _win = (Window)_root;
            if (_win == null)
            {
                LogComment("Can't get root Window.");
                return TestResult.Fail;
            }
            else return TestResult.Pass;
        }

	TestResult GetStackPanel()
        {	    
            LogComment("Getting StackPanel.");
            _st = (System.Windows.Controls.StackPanel)(_win.Content);
            if (_st == null)
            {
                LogComment("Can't get StackPanel.");
                return TestResult.Fail;
            }
	    else return TestResult.Pass;
         }
	TestResult SetButtonCommentAndCompare()
	{
            _attrProp = "$Content(" + Extract.GetTestString(0) + ") FontSize(Trademark font size " + Extract.GetTestString(3) + ")";
            
	    LogComment("Setting Localization.SetComments(button, " + _attrProp + ").");
            Localization.SetComments(CommonLib.FindElementByID("MyButton", _st), _attrProp);
            
            LogComment("Getting Localization.GetComments(button).");
            string result = Localization.GetComments(CommonLib.FindElementByID("MyButton", _st));
            LogComment("Compare Get & Set Comments.");
            if (String.Compare(result, _attrProp, false, CultureInfo.InvariantCulture) != 0)
            {
                LogComment("LocCommentTest Failed: Get Comments is " + result);
                return TestResult.Fail;
            }
            else return TestResult.Pass;
        }

	TestResult GetTextBlockCommentAndCompare()
	{
            LogComment("Getting Comments from TextBlock.");
            string result1 = Localization.GetComments(CommonLib.FindElementByID("MyTextBlock", _st));
            string expect = "FontSize ( Trademark font size ) ";
            
            LogComment("Compare Get Comments. Expected: " + expect);
            if (expect.IndexOf(result1) >= 0)
            {
                LogComment("LocCommentTest Failed: Get Comments are " + result1);
                return TestResult.Fail;
            }
            else return TestResult.Pass;
        }
	
        TestResult SetTextBlockCommentAndCompare()
        {
            LogComment("Setting Comments to TextBlock.");
            Localization.SetComments(CommonLib.FindElementByID("MyTextBlock", _st), _attrProp);
            System.Windows.Controls.TextBlock tb = (System.Windows.Controls.TextBlock)(CommonLib.FindElementByID("MyTextBlock", _st));
            object o = tb.GetValue(Localization.CommentsProperty);
            
            LogComment("Compare Get Comments.");
            if (String.Compare(o.ToString(), _attrProp, false, CultureInfo.InvariantCulture) != 0)
            {
                LogComment("LocCommentTest Failed: Get Localizability is " + o.ToString());
                return TestResult.Fail;
            }
            else
            {
               LogComment("LocCommentTest Passed.");
               return TestResult.Pass;
            }
         }
     }

    [Test(0, "Markup.Localizer", "BamlLocalizerTest", SupportFiles = @"FeatureTests\Globalization\Data\loc.baml", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class BamlLocalizerTest : StepsTest
    {

        public BamlLocalizerTest()
        {
            InitializeSteps += new TestStep(CheckDirectives);

        }

        TestResult CheckDirectives()
        {

            LogComment("BamlLocalizerTest Start: Parsing Loc.baml....");
            Stream stream = File.OpenRead("Loc.baml");
            LogComment("Creating Stream Loc Comment file.");
            UTF8Encoding u = new UTF8Encoding();

           TextReader s = new StreamReader(new MemoryStream(u.GetBytes("<LocalizableFile><LocalizationDirectives Uid=\"Button_1\" Comments=\"$Content (" + Extract.GetTestString(0) + ")\" Attributes= \"$Content (Unmodifiable Unreadable Ignore)\" /><LocalizationDirectives Uid=\"TextBlock_1\" Attributes= \"$Content(NeverLocalize)\" /></LocalizableFile>")));

            LogComment("BamlLoclaizer(input, null, loc).");
            BamlLocalizer locmgr = new BamlLocalizer(stream, new BamlLocalizabilityByReflection(), s);
            BamlLocalizationDictionary dic = locmgr.ExtractResources();
            foreach (DictionaryEntry en in dic)
            {
                BamlLocalizableResource resource = (BamlLocalizableResource)en.Value;
		if (String.Compare(string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)en.Key).Uid, ((BamlLocalizableResourceKey)en.Key).ClassName, ((BamlLocalizableResourceKey)en.Key).PropertyName), "Button_1:System.Windows.Controls.Button.$Content", false, CultureInfo.InvariantCulture) == 0)
                {
                    LogComment("Check comments.");
                    if (String.Compare(resource.Comments, Extract.GetTestString(0), false, CultureInfo.InvariantCulture) != 0)
                    {
                        LogComment("Comments are wrong. " + resource.Comments);
                        return TestResult.Fail;
                    }
                    LogComment("Check Modifiable.");
                    if (resource.Modifiable)
                    {
                        LogComment("Modifiable is true.");
                        return TestResult.Fail;
                    }
                    LogComment("Check Readable.");
                    if (resource.Readable)
                    {
                        LogComment("Readable is true.");
                        return TestResult.Fail;
                    }
                    LogComment("Check Category.");
                    if (resource.Category != LocalizationCategory.Ignore)
                    {
                        LogComment("Category is not Ignore. " + resource.Category.ToString());
                        return TestResult.Fail;
                    }
                }

		if (String.Compare(string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)en.Key).Uid, ((BamlLocalizableResourceKey)en.Key).ClassName, ((BamlLocalizableResourceKey)en.Key).PropertyName), "TextBlock_1:System.Windows.Controls.TextBlock.$Content", false, CultureInfo.InvariantCulture) == 0)
                {
                    LogComment("TextBlock_1 should not be localized.");
                    return TestResult.Fail;
                }
            }
            LogComment("BamlLocalizerTest PASS!!!");
            return TestResult.Pass;
        }
   }


    [Test(0, "Markup.Localizer", "LocAttributeValueTest", SupportFiles = @"FeatureTests\Globalization\Data\loc.xaml", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class LocAttributeValueTest : StepsTest
    {
        private Window _win;
	private UIElement _root;
        private System.Windows.Controls.StackPanel _st;
        private string _attrProp;
	
        public LocAttributeValueTest()
        {
            PackUriHelper.RegisterPackScheme();
            InitializeSteps += new TestStep(SetUpWindow);
        }

        TestResult SetUpWindow()
        {
            LogComment("LocAttributeValueTest Start: Parsing Loc.xaml....");
            _root = CommonLib.LoadXamlFromSiteOfOrigin("Loc.xaml");
            _win = (Window)_root;
            if (_win == null)
            {
                LogComment("Can't get root Window.");
                return TestResult.Fail;
            }
            else return TestResult.Pass;
        }
	
        TestResult GetStackPanel()
        {
            LogComment("Getting StackPanel.");
            _st = (System.Windows.Controls.StackPanel)(_win.Content);
            if (_st == null)
            {
                LogComment("Can't get StackPanel.");
                return TestResult.Fail;
            }
            else return TestResult.Pass;
        }

        TestResult SetButtonValueAttribute()
        {
            _attrProp = "$Content(Modifiability.Inherit Readability.Inherit LocalizationCategory.Inherit) FontFamily(Modifiable Readable)";
            LogComment("Setting Localization.SetAttributes(button, " + _attrProp + ").");
            Localization.SetAttributes(CommonLib.FindElementByID("MyButton", _st), _attrProp);

            LogComment("Getting Localization.GetAttributes(button).");
            string result = Localization.GetAttributes(CommonLib.FindElementByID("MyButton", _st));

            LogComment("Compare Get & Set Attributes.");
            if (String.Compare(result, _attrProp, false, CultureInfo.InvariantCulture) != 0)
            {
                LogComment("LocAttributeValueTest Failed: Get Attributes is " + result);
                return TestResult.Fail;
            }
            else
            {
            LogComment("LocAttributeValueTest Passed");
            return TestResult.Pass;
            }
        }
    }
        
  //      static Dispatcher _dispatcher;
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
   //     private void CreateContext()
   //     {
   //         _dispatcher = Dispatcher.CurrentDispatcher;

  //      }

	

    }
}
