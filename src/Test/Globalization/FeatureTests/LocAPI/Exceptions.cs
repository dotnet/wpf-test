// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#region Using
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Globalization;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup.Localizer;
using System.Windows.Markup;
using System.Windows.Threading;

using AutoData;
using Microsoft.Test.Logging;
#endregion

namespace Avalon.Test.Globalization
{
    public class LocMgrExceptions
    {
        private static ILogger s_log;

        static LocMgrExceptions()
        {
            PackUriHelper.RegisterPackScheme();
        }
        #region RunTest
        public void RunTest(ILogger logger, string param)
        {
            s_log = logger;
            s_log.Stage(TestStage.Initialize);
            _rm = new System.Resources.ResourceManager("ExceptionStringTable", typeof(Button).Assembly);
            string strParams = param;
            _passedInParams = param;
            s_log.LogResult("LocMgrExceptions Started ..." + "\n");
            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(CultureInfo.InvariantCulture);
            switch (strParams)
            {
                case "LOCMGREXCEPTION":
                    TestBamlLocMgrException();
                    break;
                case "TREEEXCEPTION":
                    TestTreeException();
                    break;
                case "GENRESEXCEPTION":
                    TestUpdateBaml();
                    break;
                case "DUPUID":
                    DuplicateUIDExceptionTest();
                    break;
                case "MISSINGUID":
                    MissingUIDExceptionTest();
                    break;
                case "NOUID":
                    NoUIDExceptionTest();
                    break;
                case "ADDDUPKEY":
                    AddingDupKeyTest();
                    break;
                case "INVALIDTRAN":
                    InvalidTranslationTest();
                    break;
                case "DIC":
                    TestExceptionDictionary();
                    break;
                case "LOCDIC":
                    BamlLocalizationDictionaryException();
                    break;
                case "COMMENT":
                    LocCommentExceptionTest();
                    break;
                case "COMMENTLOAD":
                    LocCommentLoadException();
                    break;
                case "INVALIDATTR":
                    InvalidLocAttribute();
                    break;
                case "ICCOPYTO":
                    TestICollectionCopyTo();
                    break;
                case "124322":
                    TestLocAttributeException();
                    break;
                case "124458":
                    StartElementException();
                    break;
                default:
                    s_log.LogResult("LocMgrExceptions.RunTest was called with an unsupported parameter.");
                    throw new ApplicationException("Parameter is not supported");
            }
            // _log.Dispose();
        }
        #endregion
        #region TestBamlLocMgrException
        void TestBamlLocMgrException()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("TestBamlLocMgrException: Start test...");
            try
            {
                BamlLocalizer locmgr = new BamlLocalizer(null, null);
		        locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
            }
            catch (ArgumentNullException exp)
            {
                s_log.LogResult(true, "BamlLocalizer(passing null for src): Catch Expected Exception.\n" + exp.Message);
            }
        }
        #endregion
        #region TestTreeException
        void TestTreeException()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("TestTreeException: Start test...");
            Stream stream = File.OpenRead("tree.baml");
            try
            {
                BamlLocalizer locmgr = new BamlLocalizer(stream, null);
		locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
                foreach (DictionaryEntry entry in locmgr.ExtractResources())
                {
                    BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
                    s_log.LogResult(resource.Category.ToString());
                    s_log.LogResult(resource.Readable.ToString());
                    s_log.LogResult("Content: " + resource.Content);
                    s_log.LogResult("Comments: " + resource.Comments);
                }
            }
            catch (ArgumentException exp)
            {
                s_log.LogResult(true, "Catch Expected Exception.\n" + exp.Message);
            }
        }
        #endregion
        #region TestUpdateBaml
        void TestUpdateBaml()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("TestUpdateBaml: Start test...");
            Stream stream = File.OpenRead("GlobTestCase.baml");
            BamlLocalizer locmgr = new BamlLocalizer(stream, null);
	    locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
            BamlLocalizationDictionary dictionary = new BamlLocalizationDictionary();
            s_log.LogResult("Generating new Resource BAML.");
            try
            {
                locmgr.UpdateBaml(null, dictionary);
            }
            catch (ArgumentNullException exp)
            {
                s_log.LogResult(true, "UpdateBaml(passing null value for stream): Catch Expected Exception.\n" + exp.Message);
            }
            string tgt = "GlobTempTarget.baml";
            Stream target = new FileStream(tgt, FileMode.Create);
            try
            {
                locmgr.UpdateBaml(target, null);
            }
            catch (ArgumentNullException exp)
            {
                s_log.LogResult(true, "UpdateBaml(passing null value for resourcedictionary): Catch Expected Exception.\n" + exp.Message);
            }
            finally
            {
                target.Close();
                File.Delete(tgt);
            }
        }
        #endregion
        #region DuplicateUIDExceptionTest
        void DuplicateUIDExceptionTest()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("DuplicateUIDExceptionTest: Start test...");
            Stream stream = File.OpenRead("DuplicateUID.baml");
            try
            {
                BamlLocalizer locmgr = new BamlLocalizer(stream);
		        locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
                BamlLocalizationDictionary dictionary = locmgr.ExtractResources();
                foreach (DictionaryEntry entry in dictionary)
                {
					s_log.LogResult(string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName));
                }
            }
            catch (InvalidOperationException exp)
            {
                string message = _rm.GetString("DuplicateId");
                message = string.Format(message, "1");
                if (String.Compare(exp.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    s_log.LogResult(true, "DuplicateUIDExceptionTest: Catch Expected Exception.\n" + exp.Message);
                }
                else
                {
                    s_log.LogResult(false, "Incorrect exception being thrown: " + exp.Message);
                }
                
            }
        }
        #endregion
        #region MissingUIDExceptionTest
        void MissingUIDExceptionTest()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("MissingUIDExceptionTest: Start test...");
            Stream stream = File.OpenRead("MissingUID.baml");
            try
            {
                BamlLocalizer locmgr = new BamlLocalizer(stream);
	 	        locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
                BamlLocalizationDictionary dictionary = locmgr.ExtractResources();
                foreach (DictionaryEntry entry in dictionary)
                {
					s_log.LogResult(string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName));
                }
            }
            catch (InvalidOperationException exp)
            {
                string message = _rm.GetString("ChildrenElementNoUid");
                if (String.Compare(exp.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    s_log.LogResult(true, "MissingUIDExceptionTest: Catch Expected Exception.\n" + exp.Message);
                }
                else
                {
                    s_log.LogResult(false, "MissingUIDExceptionTest: Incorrect exception being thrown: " + exp.Message);
                }

            }
        }
        #endregion
        #region NoUIDExceptionTest
        /// <summary>
        /// 

        void NoUIDExceptionTest()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("NoUIDExceptionTest: Start test...");
            Stream stream = File.OpenRead("NoUID.baml");
            try
            {
                BamlLocalizer locmgr = new BamlLocalizer(stream);
		        locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
                BamlLocalizationDictionary dictionary = locmgr.ExtractResources();
                foreach (DictionaryEntry entry in dictionary)
                {
					s_log.LogResult(string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName));
                }
            }
            catch (InvalidOperationException exp)
            {
                string message = _rm.GetString("ChildrenElementNoUid");
                if (String.Compare(exp.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    s_log.LogResult(true, "NoUIDExceptionTest: Catch Expected Exception.\n" + exp.Message);
                }
                else
                {
                    s_log.LogResult(false, "NoUIDExceptionTest: Incorrect exception being thrown: " + exp.Message);
                }

            }
        }
        #endregion
        #region AddingDupKeyTest
        void AddingDupKeyTest()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("AddingDupKeyTest: Start test...");
            string bamlName = "GlobTestCase.baml";
            Stream stream = File.OpenRead(bamlName);
            CreateContext();
            BamlLocalizer locmgr = new BamlLocalizer(stream);
	    locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
            BamlLocalizationDictionary dictionary = new BamlLocalizationDictionary();
            foreach (DictionaryEntry entry in locmgr.ExtractResources())
            {
				s_log.LogResult("Key: " + string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName));
                BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        dictionary.Add((BamlLocalizableResourceKey) entry.Key, resource);
                    }
                    catch (ArgumentException exp)
                    {
                        s_log.LogResult(true, "AddingDupKeyTest: Catch Expected Exception.\n" + exp.Message);
                        i++;
                        break;
                    }
                    if(i>0)
                        s_log.LogResult(false, "AddingDupKeyTest: Didn't Catch Expected Exception.\n");
                }
            }
            stream.Close();
            s_log.LogResult(true, "AddingDupKeyTest: Test PASS.......");
        }
        #endregion
        #region InvalidTranslationTest
        /// <summary>
        /// 
        /// </summary>
        void InvalidTranslationTest()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("InvalidTranslationTest: Start Test....");
            string key1 = "TextBlock_1:System.Windows.Controls.TextBlock.$Content";
            bool found = false;
            string tran = AutoData.Generate.GetInvalidString();
            string tgt = "GlobTempTarget.baml";
            string bamlName = "GlobTestCase.baml";
            Stream stream = File.OpenRead(bamlName);
            CreateContext();
            BamlLocalizer locmgr = new BamlLocalizer(stream);
	        locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
            BamlLocalizationDictionary dictionary = new BamlLocalizationDictionary();
            foreach (DictionaryEntry entry in locmgr.ExtractResources())
            {
                BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
				if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains(key1))
                {
                    resource.Category = System.Windows.LocalizationCategory.Text;
                    resource.Readable = true;
                    resource.Modifiable = true;
                    resource.Comments = AutoData.Generate.GetInvalidString();
                    s_log.LogResult("Trying to Set Content using AutoData in TextBlock_1.");
                    resource.Content = tran;
                    found = true;
                }
                dictionary.Add((BamlLocalizableResourceKey)entry.Key, resource);
            }
            if (!found)
            {
                s_log.LogResult(false, "Failed: Could not find correct Text controls.");
                return;
            }
            if (File.Exists(tgt))
            {
                try
                {
                    File.Delete(tgt);
                }
                catch (Exception e)
                {
                    s_log.LogResult("Failed to clean up file..." + e.ToString());
                }
            }
            Stream target = new FileStream(tgt, FileMode.Create);
            s_log.LogResult("Generating new Resource BAML with Invalid Strings");
            try
            {
                locmgr.UpdateBaml(target, dictionary);
            }
            catch (FormatException exp)
            {
                string message = _rm.GetString("InvalidTranslation");
                message = string.Format(message, tran);
                if (String.Compare(exp.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    s_log.LogResult(true, "InvalidTranslationTest: Catch Expected Exception.\n" + exp.Message);
                }
                else
                {
                    s_log.LogResult(false, "InvalidTranslationTest: Incorrect exception being thrown: " + exp.Message);
                }
            }
            finally
            {
                target.Close();
                s_log.LogResult("Clean up file...");
                if (File.Exists(tgt))
                {
                    try
                    {
                        File.Delete(tgt);
                    }
                    catch (Exception e)
                    {
                        s_log.LogResult("Failed to clean up file..." + e.ToString());
                    }
                }
            }
            s_log.LogResult("InvalidTranslationTest::Done...");
        }
        #endregion
        #region TestExceptionDictionary
        /// <summary>
        /// 
        /// </summary>
        void TestExceptionDictionary()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("TestExceptionDictionary Start...");
            BamlLocalizationDictionary d = new BamlLocalizationDictionary();
            try
            {
                BamlLocalizableResource r = d[null];
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ArgumentNullException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    s_log.LogResult(true, "CheckNonNullParam throw exception.\n" + e.Message);
                }
                return;
            }
            s_log.LogResult(false, "CheckNonNullParam did not throw exception.");
        }
        #endregion
        #region BamlLocalizationDictionaryException
        /// <summary>
        /// 
        /// </summary>
        void BamlLocalizationDictionaryException()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("BamlLocalizationDictionaryException Start...");
            DictionaryEntry[] en = new DictionaryEntry[1];
            BamlLocalizationDictionary d = new BamlLocalizationDictionary();
            BamlLocalizableResourceKey testKey1 = new BamlLocalizableResourceKey(AutoData.Extract.GetTestString(2),AutoData.Extract.GetTestString(2),AutoData.Extract.GetTestString(2));
            BamlLocalizableResourceKey testKey2 = new BamlLocalizableResourceKey(AutoData.Extract.GetTestString(1),AutoData.Extract.GetTestString(1),AutoData.Extract.GetTestString(1));

            d.Add(testKey1, null);
            d.Add(testKey2, null);
            try
            {
                s_log.LogResult("CopyTo(dicEntry, -1).");
                d.CopyTo(en, -1);
            }
            catch (Exception e)
            {
                string message = _rm.GetString("ParameterCannotBeNegative");
                if (!e.GetType().Equals(typeof(ArgumentOutOfRangeException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    if (e.Message.IndexOf(message) >= 0)
                    {
                        s_log.LogResult(true, "BamlLocalizationDictionaryException: Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "BamlLocalizationDictionaryException: Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
            try
            {
                s_log.LogResult("CopyTo(dicEntry, GreaterThan dicEntry (3)).");
                d.CopyTo(en, 3);
            }
            catch (Exception e)
            {
                string message = _rm.GetString("Collection_CopyTo_IndexGreaterThanOrEqualToArrayLength");
                message = String.Format(message, "arrayIndex", "array");
                if (!e.GetType().Equals(typeof(ArgumentException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    if (e.Message.IndexOf(message) >= 0)
                    {
                        s_log.LogResult(true, "BamlLocalizationDictionaryException: Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "BamlLocalizationDictionaryException: Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
            try
            {
                s_log.LogResult("CopyTo(dicEntry, d is GreaterThan dicEntry).");
                d.CopyTo(en, 0);
            }
            catch (Exception e)
            {
                string message = _rm.GetString("Collection_CopyTo_NumberOfElementsExceedsArrayLength");
                message = String.Format(message, "arrayIndex", "array");
                if (!e.GetType().Equals(typeof(ArgumentException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    if (e.Message.IndexOf(message) >= 0)
                    {
                        s_log.LogResult(true, "BamlLocalizationDictionaryException: Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "BamlLocalizationDictionaryException: Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
        }
        #endregion
        #region LocCommentExceptionTest
        void LocCommentExceptionTest()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("LocCommentExceptionTest Start...");
            try
            {
                s_log.LogResult("Localization.GetComments(null).");
                string a = Localization.GetComments(null);
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ArgumentNullException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    s_log.LogResult(true, "GetComments(null) return ArgumentNullException.");
                }
            }
            try
            {
                s_log.LogResult("Localization.GetAttributes(null).");
                string b = Localization.GetAttributes(null);
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ArgumentNullException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    s_log.LogResult(true, "Loc.GetAttributes(null) return ArgumentNullException.");
                }
            }
            try
            {
                s_log.LogResult("Localization.SetComments(null, abc).");
                Localization.SetComments(null, "abc");
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ArgumentNullException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    s_log.LogResult(true, "Localization.SetComments(null, abc) return ArgumentNullException.");
                }
            }
            try
            {
                s_log.LogResult("Localization.SetAttributes(null, abc).");
                Localization.SetAttributes(null, "abc");
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ArgumentNullException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    s_log.LogResult(true, "Localization.SetAttributes(null, abc) return ArgumentNullException.");
                }
            }
            s_log.LogResult("Parsing Loc.xaml....");
            
            UIElement root = LoadXamlFromSiteOfOrigin("Loc.xaml");

            Window win = (Window)root;
            if (win == null)
            {
                s_log.LogResult(false, "Can't get root Window.");
                return;
            }
            s_log.LogResult("Getting StackPanel.");
            System.Windows.Controls.StackPanel st = (System.Windows.Controls.StackPanel)(win.Content);
            if (st == null)
            {
                s_log.LogResult(false, "Can't get StackPanel.");
                return;
            }
            try
            {
                s_log.LogResult("Localizability.SetComments(d, abc).");
                Localization.SetComments(CommonLib.FindElementByID("MyButton", st), AutoData.Extract.GetTestString(0));
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(FormatException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    string message = _rm.GetString("UnmatchedLocComment");
                    message = string.Format(message, AutoData.Extract.GetTestString(0));
                    if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        s_log.LogResult(true, "Localization.SetComments(d, abc).. Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "Localization.SetComments(d, abc) Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
            try
            {
                s_log.LogResult("Localization.SetAttributes(d, abc).");
                Localization.SetAttributes(CommonLib.FindElementByID("MyButton", st), AutoData.Extract.GetTestString(0));
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(FormatException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    string message = _rm.GetString("UnmatchedLocComment");
                    message = string.Format(message, AutoData.Extract.GetTestString(0));
                    if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        s_log.LogResult(true, "Localization.SetAttributes(d, abc).. Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "Localization.SetAttributes(d, abc) Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
            try
            {
                s_log.LogResult("Localization.SetAttributes(d, null).");
                Localization.SetAttributes(CommonLib.FindElementByID("MyButton", st), null);
            }
            catch (Exception e)
            {
                // 
                if (!e.GetType().Equals(typeof(NullReferenceException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    s_log.LogResult(true, "Localization.SetAttributes(d, null) return ArgumentNullException.");
                }
            }
            s_log.LogResult(true, "LocCommentExceptionTest PASS!!.");
        }
        #endregion
        #region LocCommentLoadException
        /// <summary>
        /// Test Case # 124307
        /// </summary>
        void LocCommentLoadException()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("LocCommentLoadException Start: Parsing Loc.baml....");
            Stream stream = File.OpenRead("Loc.baml");
            s_log.LogResult("Creating Stream Loc Comment file.");
            UTF8Encoding u = new UTF8Encoding();
            TextReader s = new StreamReader (new MemoryStream(u.GetBytes("<root><Hello /></root>")));
            TextReader s1 = new StreamReader (new MemoryStream(u.GetBytes("test")));
            try
            {
                s_log.LogResult("BamlLoclaizer(input, null, loc), passing text as loc comment.");
                BamlLocalizer loc = new BamlLocalizer(stream, null, s1);
		        loc.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ArgumentException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    string message = _rm.GetString("InvalidXmlCommentStream");
                    if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        s_log.LogResult(true, "LocCommentLoadException: Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "LocCommentLoadException: Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
            stream.Close();
            stream = File.OpenRead("Loc.baml");
            try
            {
                s_log.LogResult("BamlLoclaizer(input, null, loc), passing invalid xml loc comment.");
                BamlLocalizer locmgr = new BamlLocalizer(stream, null, s);
		        locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(FormatException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    string message = _rm.GetString("InvalidCommentStreamRoot");
                    message = string.Format(message, "root", "LocalizableFile");
                    if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        s_log.LogResult(true, "LocCommentLoadException: Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "LocCommentLoadException: Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
        }
        #endregion
        #region InvalidLocAttribute
        /// <summary>
        /// Test Case # 124310
        /// </summary>
        void InvalidLocAttribute()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("InvalidLocAttribute Start: Parsing Loc.xaml....");
            UIElement root = LoadXamlFromSiteOfOrigin("Loc.xaml");
            Window win = (Window)root;
            if (win == null)
            {
                s_log.LogResult(false, "Can't get root Window.");
                return;
            }
            s_log.LogResult("Getting StackPanel.");
            System.Windows.Controls.StackPanel st = (System.Windows.Controls.StackPanel)(win.Content);
            if (st == null)
            {
                s_log.LogResult(false, "Can't get StackPanel.");
                return;
            }
            string AttrProp = "$Content(TestInherit Unreadable Text)";
            s_log.LogResult("Setting Localization.SetAttributes(button, " + AttrProp + ").");
            try
            {
                Localization.SetAttributes(CommonLib.FindElementByID("MyButton", st), AttrProp);
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(FormatException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    string message = _rm.GetString("InvalidLocalizabilityValue");
                    message = string.Format(message, "TestInherit");
                    if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        s_log.LogResult(true, "InvalidLocAttribute: Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "InvalidLocAttribute: Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
        }
        #endregion
        #region TestICollectionCopyTo
        /// <summary>
        /// 124312
        /// </summary>
        void TestICollectionCopyTo()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("124312: TestICollectionCopyTo Start...");
            BamlLocalizationDictionary d = new BamlLocalizationDictionary();
            ICollection ic = (ICollection)d;
            Array[,] a = new Array[2,3];
            try
            {
                ic.CopyTo(a, 1);
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ArgumentException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    string message = _rm.GetString("Collection_CopyTo_ArrayCannotBeMultidimensional");
                    if (e.Message.IndexOf(message) >= 0)
                    {
                        s_log.LogResult(true, "TestICollectionCopyTo: Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "TestICollectionCopyTo: Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
        }
        #endregion
        #region TestLocAttributeException
        /// <summary>
        /// 124322
        /// </summary>
        void TestLocAttributeException()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("124322: TestLocAttributeException Start...");
            LocalizabilityAttribute la;
            object LocEnum = Enum.Parse(typeof(LocalizationCategory), "19");
            object ModEnum = Enum.Parse(typeof(Modifiability), "19");
            object ReadEnum = Enum.Parse(typeof(Readability), "19");
            try
            {
                s_log.LogResult("Setting Invalid Enum for LocalizationCategory.");
                la = new LocalizabilityAttribute((LocalizationCategory)LocEnum);
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(InvalidEnumArgumentException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    s_log.LogResult(true, "TestLocAttributeException: Catch Expected Exception.");
                }
            }
            try
            {
                s_log.LogResult("Setting Invalid Enum for Readability.");
                la = new LocalizabilityAttribute(LocalizationCategory.None);
                la.Readability = (Readability)ReadEnum;
            }
            catch(Exception e)
            {
                if (!e.GetType().Equals(typeof(InvalidEnumArgumentException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    s_log.LogResult(true, "TestLocAttributeException: Catch Expected Exception.");
                }
            }
            try
            {
                s_log.LogResult("Setting Invalid Enum for Modifiability.");
                la = new LocalizabilityAttribute(LocalizationCategory.None);
                la.Modifiability = (Modifiability)ModEnum;
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(InvalidEnumArgumentException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    s_log.LogResult(true, "TestLocAttributeException: Catch Expected Exception.");
                }
            }
        }
        #endregion
        #region StartElementException
        /// <summary>
        /// Test Case # 124458
        /// LoadBaml
        /// </summary>
        void StartElementException()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("StartElementException Start: Parsing Loc.baml....");
            Stream stream = File.OpenRead("invalidlocattributesandcomments.baml");
            s_log.LogResult("Creating Stream Loc Comment file.");
            UTF8Encoding u = new UTF8Encoding();
            TextReader s = new StreamReader(new MemoryStream(u.GetBytes("<root><Hello /></root>")));
            TextReader s1 = new StreamReader(new MemoryStream(u.GetBytes("test")));
            try
            {
                s_log.LogResult("BamlLoclaizer(input, null, loc), passing text as loc comment.");
                BamlLocalizer loc = new BamlLocalizer(stream, null, s1);
                loc.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
                loc.ExtractResources();
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ArgumentException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    string message = _rm.GetString("InvalidXmlCommentStream");
                    if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        s_log.LogResult(true, "StartElementException: Catch Expected Exception.");
                    }
                    else
                    {
                        s_log.LogResult(false, "StartElementException: Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
            try
            {
                s_log.LogResult("BamlLoclaizer(input, null, loc), passing invalid xml loc comment.");
                stream.Seek(0, SeekOrigin.Begin);
                BamlLocalizer locmgr = new BamlLocalizer(stream, null, s);
                locmgr.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(CatchWarningEvents);
                locmgr.ExtractResources();
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(System.FormatException)))
                {
                    s_log.LogResult(false, "Exception Type is not expected. " + e.GetType().ToString());
                }
                else
                {
                    if ((e.Message).Contains("LocalizableFile"))
                    {
                        s_log.LogResult(true, "StartElementException: Catch Expected Exception." + e.Message);
                    }
                    else
                    {
                        s_log.LogResult(false, "StartElementException: Incorrect exception being thrown: " + e.Message);
                    }
                }
            }
        }
        #endregion
        #region Helper
        public UIElement LoadBAML(string filename)
        {
            s_log.LogResult("Loading BAML...");
            System.Uri resourceLocator = new System.Uri(filename, System.UriKind.RelativeOrAbsolute);
            UIElement root = System.Windows.Application.LoadComponent(resourceLocator) as UIElement;
            return root;
        }

	public UIElement LoadXamlFromSiteOfOrigin(string filename)
	{
            s_log.LogResult("Loading Xaml from site of origin - " + filename);
	    Uri resourceUri = new Uri("pack://siteoforigin:,,,/" + filename, UriKind.RelativeOrAbsolute);	
	    Stream s = Application.GetRemoteStream(resourceUri).Stream;
	    UIElement root = XamlReader.Load(s) as UIElement;
	    return root;
	}

        System.Resources.ResourceManager _rm;
        static Dispatcher s_dispatcher;
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;
        }
	
	 private void CatchWarningEvents(object sender, BamlLocalizerErrorNotifyEventArgs args)
        {
            s_log.LogResult("----An Event has been fired from BamlLocalizerError----");
            switch(_passedInParams)
             {
            case "MISSINGUID":
                if(args.Error == BamlLocalizerError.UidMissingOnChildElement)
                    s_log.LogResult(true, "Correct error found: " + args.Error);
                else
                    s_log.LogResult(false,"Wrong Error. Expected: UidMissingOnChildElement found: " + args.Error);
                break;
	        case "DUPUID":
                if(args.Error == BamlLocalizerError.DuplicateUid)
                    s_log.LogResult(true, "Correct error found: " + args.Error);
                else
                    s_log.LogResult(false,"Wrong Error. Expected: DuplicateUid found: " + args.Error);
                break;
	        case "INVALIDTRAN":
                if(args.Error == BamlLocalizerError.SubstitutionAsPlaintext)
                    s_log.LogResult(true, "Correct error found: " + args.Error);
                else
                    s_log.LogResult(false,"Wrong Error. Expected: SubstitutionAsPlaintext found: " + args.Error);
                break;
            case "124458":

                if (args.Error == BamlLocalizerError.InvalidLocalizationAttributes)
                {
                    s_log.LogResult(true, "Correct error found: " + args.Error);
                    break;
                }
                else if (args.Error == BamlLocalizerError.InvalidLocalizationComments)
                {
                    s_log.LogResult(true, "Correct error found: " + args.Error);
                    break;
                }
                else if (args.Error == BamlLocalizerError.InvalidCommentingXml)
                {
                    s_log.LogResult(true, "Correct error found: " + args.Error);
                    break;
                }
                else
                    s_log.LogResult(false, "Wrong Error. Found: " + args.Error);
                
                break;
            
            default:
              s_log.LogResult(false,"Unknown Error: " + args.Error);
		break;
            }
        }
        #endregion

	//Class variables
	string _passedInParams;
    }
}
