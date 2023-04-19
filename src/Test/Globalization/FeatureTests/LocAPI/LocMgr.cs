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

using AutoData;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
#endregion

namespace Avalon.Test.Globalization
{
    public class BamlLocMgr
    {
        private static ILogger s_log;
        
        static BamlLocMgr()
        {
            PackUriHelper.RegisterPackScheme();
        }
        #region RunTest
        /// <summary>
        /// 
        /// </summary>
        public void RunTest(ILogger logger, string param)
        {
            s_log = logger;
            s_log.Stage(TestStage.Initialize);
            string strParams = param;
            s_log.LogResult("BamlLocMgr Started ..." + "\n");
            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(CultureInfo.InvariantCulture);
            switch (strParams)
            {
                case "READ":
                    TestRead();
                    break;
                case "WRITE":
                    TestWrite();
                    break;
                case "DICT":
                    TestDictionary();
                    break;
                case "123007":
                    TestBamlLocDic();
                    break;
                default:
                    s_log.LogResult("BamlLocMgr.RunTest was called with an unsupported parameter.");
                    throw new ApplicationException("Parameter is not supported");
            }
            // _log.Dispose();
        }
        #endregion
        #region TestRead
        /// <summary>
        /// 
        /// </summary>
        void TestRead()
        {
            s_log.LogResult("Start TestRead....");
            TestReader();
            s_log.LogResult(true, "BamlLocMgr.TestRead: Pass....");
            s_log.Stage(TestStage.Cleanup);
        }
        /// <summary>
        /// 
        /// </summary>
        void TestReader()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("Parsing tree.baml....");
            string tree = CommonLib.DumpBamlToString("tree.baml");
            Console.WriteLine(tree);
            _table = new Hashtable();
            Stream stream = File.OpenRead("tree.baml");
            CreateContext();
            BamlLocalizer locmgr = new BamlLocalizer(stream);
            BamlLocalizationDictionary dic = locmgr.ExtractResources();
            s_log.LogResult("Checking Resource count...");
            // check to see if we miss any elements.
            // Regression Test for 
            if (dic.Count != 84)
            {
                s_log.LogResult(false, "Failed: Dic.Count (84): Actual result is " + dic.Count.ToString());
            }
            foreach (DictionaryEntry entry in dic)
            {
                string bamlName = "tree.baml";
				string key = string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName); ;
                s_log.LogResult("Key: " + key);
                BamlLocalizationDictionary dictionary = this[bamlName];
                if (dictionary == null)
                {
                    dictionary = new BamlLocalizationDictionary();
                    this[bamlName] = dictionary;
                }
                BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
                s_log.LogResult("Category: " + resource.Category.ToString());
                s_log.LogResult("Readable: " + resource.Readable.ToString());
                s_log.LogResult("Modifiable: " + resource.Modifiable.ToString());
                s_log.LogResult("Comments: " + resource.Comments);
                s_log.LogResult("Content: " + resource.Content);
                if (dictionary.IsFixedSize)
                    s_log.LogResult(false, "BamlLocalizationDictionary.IsFixedSize" + dictionary.IsFixedSize.ToString());
                if (dictionary.IsReadOnly)
                    s_log.LogResult(false, "BamlLocalizationDictionary.IsReadOnly" + dictionary.IsReadOnly.ToString());
                s_log.LogResult("BamlLocalizationDictionary.Add");
				dictionary.Add((BamlLocalizableResourceKey)entry.Key, resource);
                s_log.LogResult("BamlLocalizationDictionary.Contains");
                if (!dictionary.Contains((BamlLocalizableResourceKey)entry.Key))
                    s_log.LogResult(false, "BamlLocalizationDictionary.Add: Failed..");
                s_log.LogResult("BamlLocalizationDictionary.GetEnumerator");
                BamlLocalizationDictionaryEnumerator Enum = dictionary.GetEnumerator();
                Enum.MoveNext();
				if (Enum.Current.Key != (BamlLocalizableResourceKey)entry.Key)
                    s_log.LogResult(false, "BamlLocalizationDictionary.GetEnumerator: Failed..");
                s_log.LogResult(Enum.Entry.ToString() + " " + Enum.Key.ToString() + " " + Enum.Value.ToString());
                s_log.LogResult("Count: " + dictionary.Count.ToString());
                if (dictionary.Count > 0)
                {
                    s_log.LogResult("Enum.Reset");
                    Enum.Reset();
                    s_log.LogResult("Enum.MoveNext");
                    Enum.MoveNext();
                    if (Enum.Current.Key == null)
                    {
                        s_log.LogResult(false, "BamlLocalizationDictionary.GetEnumerator: Failed..");
                        s_log.LogResult(Enum.Current.ToString() + "\n" + Enum.Entry.ToString() + "\n" + Enum.Key.ToString() + "\n" + Enum.Value.ToString());
                    }
                    s_log.LogResult(Enum.Current.ToString() + "\n" + Enum.Entry.ToString() + "\n" + Enum.Key.ToString() + "\n" + Enum.Value.ToString());
                }
                s_log.LogResult("BamlLocalizationDictionary.Remove");
				dictionary.Remove((BamlLocalizableResourceKey)entry.Key);
				if (dictionary.Contains((BamlLocalizableResourceKey)entry.Key))
                    s_log.LogResult(false, "BamlLocalizationDictionary.Remove: Failed..");
                s_log.LogResult("BamlLocalizationDictionary.Clear");
                dictionary.Clear();
                if (dictionary.Count != 0)
                    s_log.LogResult(false, "BamlLocalizationDictionary.Clear: Failed...." + dictionary.Count.ToString());
            }
        }
        #endregion
        #region TestWrite
        /// <summary>
        /// 
        /// </summary>
        void TestWrite()
        {
            s_log.Stage(TestStage.Run);
            string key1 = "TextBlock_1:System.Windows.Controls.TextBlock.$Content";
            string key2 = "TextBlock_2:System.Windows.Controls.TextBlock.$Content";
            bool found = false;
            string tgt = "GlobTempTarget.baml";
            _table = new Hashtable();
            string bamlName = "GlobTestCase.baml";
            s_log.LogResult("Parsing " + bamlName);
            Stream stream = File.OpenRead(bamlName);
            CreateContext();
            BamlLocalizer locmgr = new BamlLocalizer(stream);
            BamlLocalizationDictionary dictionary = this[bamlName];
            if (dictionary == null)
            {
                dictionary = new BamlLocalizationDictionary();
                this[bamlName] = dictionary;
            }
            foreach (DictionaryEntry entry in locmgr.ExtractResources())
            {
                BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
				if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains(key1))
                {
                    resource.Category = System.Windows.LocalizationCategory.Text;
                    resource.Readable = true;
                    resource.Modifiable = true;
                    resource.Comments = AutoData.Extract.GetTestString(0);
                    s_log.LogResult("Trying to Set Content using AutoData in TextBlock_1.");
                    resource.Content = AutoData.Extract.GetTestString(0);
                    found = true;
                }
				if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains(key2))
                {
                    resource.Category = System.Windows.LocalizationCategory.Text;
                    resource.Readable = false;
                    resource.Modifiable = false;
                    resource.Comments = AutoData.Extract.GetTestString(0);
                    s_log.LogResult("Trying to Set Content using AutoData in TextBlock_2.");
                    resource.Content = AutoData.Extract.GetTestString(0);
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
            s_log.LogResult("Generating new Resource BAML.");
            locmgr.UpdateBaml(target, dictionary);
            target.Close();
            UIElement root = LoadBAML(tgt);
            DockPanel fp = root as DockPanel;
            if (fp == null)
                s_log.LogResult(false, "GenerateResource Failed....Root element is not DockPanel.");
            stream = File.OpenRead(tgt);
            locmgr = new BamlLocalizer(stream, null);
            foreach (DictionaryEntry entry in locmgr.ExtractResources())
            {
                BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;
				if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains(key1) == true)
                {
                    s_log.LogResult("Verifying Content TextBlock_1 Value.");
                    if (resource.Content != AutoData.Extract.GetTestString(0))
                        s_log.LogResult(false, "Content: " + resource.Content);
                }
				if ((string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)entry.Key).Uid, ((BamlLocalizableResourceKey)entry.Key).ClassName, ((BamlLocalizableResourceKey)entry.Key).PropertyName)).Contains(key2) == true)
                {
                    s_log.LogResult("Verifying Content TextBlock_2 Value.");
                    if (resource.Content != "page?")
                        s_log.LogResult(false, "Content: " + resource.Content);
                }
            }
            stream.Close();
            if (BamlHelper.CompareBamlFiles(bamlName, tgt).Count == 0)
                s_log.LogResult(false, "Same Output...");
            s_log.LogResult(true, "BamlLocMgr.TestWrite: Pass....");
            s_log.Stage(TestStage.Cleanup);
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
        #endregion
        #region TestDictionary
        /// <summary>
        /// 
        /// </summary>
        void TestDictionary()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("Parsing GlobTestCase.baml....");
            string tree = CommonLib.DumpBamlToString("GlobTestCase.baml");
            Console.WriteLine(tree);
            Stream stream = File.OpenRead("GlobTestCase.baml");
            CreateContext();
            BamlLocalizer locmgr = new BamlLocalizer(stream);
            BamlLocalizationDictionary dic = locmgr.ExtractResources();
            // ICollection
            ICollection ic = (ICollection)dic;
            int ic_count = ic.Count;
            s_log.LogResult("Checking Resource count...");
            if (ic_count != 16)
                s_log.LogResult(false, "ICollection.Count is not equal 16. " + ic_count.ToString());
            bool ic_IsSync = ic.IsSynchronized;
            s_log.LogResult("Checking Is Sync..." + ic_IsSync.ToString());
            object ic_SyncRoot = (object)(ic.SyncRoot);
            s_log.LogResult("Checking Sync Root..." + ic_SyncRoot);
            DictionaryEntry[] al = new DictionaryEntry[16];
            ic.CopyTo(al, 0);
            for (int i = 0; i < al.Length; i++)
            {
                if (al[i].Key == null)
                {
                    s_log.LogResult(false, "DictionaryEntry is null. ICollection.CopyTo failed...");
                    break;
                }
            }
            // IDictionary
            IDictionary idic = (IDictionary)dic;
            int idic_count = idic.Count;
            s_log.LogResult("Checking IDic count.");
            if (ic_count != idic_count)
                s_log.LogResult(false, "ICollection.Count is not equal to IDictionary. " + idic_count.ToString());
            bool idic_IsFix = idic.IsFixedSize;
            s_log.LogResult("Checking IDic IsFixedSize.");
            if (idic_IsFix)
                s_log.LogResult(false, "IDictionary.IsFixedSize is " + idic_IsFix.ToString());
            bool idic_IsRead = idic.IsReadOnly;
            s_log.LogResult("Checking IDic IsReadOnly.");
            if (idic_IsRead)
                s_log.LogResult(false, "IDictionary.IsReadOnly is " + idic_IsRead.ToString());
            bool idic_IsSync = idic.IsSynchronized;
            s_log.LogResult("Checking IDic IsSynchronized.");
            if (ic_IsSync != idic_IsSync)
                s_log.LogResult(false, "ICollection.IsSynchronized is not equal to IDictionary. " + idic_IsSync.ToString());
            ICollection idic_Keys = (ICollection)(idic.Keys);
            if (dic.Keys != idic_Keys)
                s_log.LogResult(false, "ICollection.SyncRoot.Keys is not equal to IDictionary. " + idic_Keys.ToString());
            object idic_sync = (object)(idic.SyncRoot);
            s_log.LogResult("Checking IDic SyncRoot.");
            if (ic_SyncRoot != idic_sync)
                s_log.LogResult(false, "ICollection.SyncRoot is not equal to IDictionary. " + idic_sync.ToString());
            ICollection idic_Values = (ICollection)(idic.Values);
            if (dic.Values != idic_Values)
            {
                s_log.LogResult(false, "ICollection.SyncRoot.Values is not equal to IDictionary. " + idic_Values.ToString());
            }
            // IDictionaryEnumerator
            s_log.LogResult("Checking IDictionaryEnumerator.");
            IDictionaryEnumerator idEnum = idic.GetEnumerator();
            if (idEnum == null)
            {
                s_log.LogResult(false, "IDictionaryEnumerator is null.");
            }
            else
            {
                foreach (DictionaryEntry en in idic)
                {
                    idEnum.MoveNext();
                    s_log.LogResult("Checking IDictionaryEnumerator.Key.");
                    if (idEnum.Key == null)
                    {
                        s_log.LogResult(false, "IDictionaryEnumerator.Key is null.");
                    }
                    s_log.LogResult("Checking IDictionaryEnumerator.Value.");
                    if (idEnum.Value == null)
                    {
                        s_log.LogResult(false, "IDictionaryEnumerator.Value is null.");
                    }
                    break;
                }
            }
            // IEnumerable
            s_log.LogResult("Checking IEnumerable.");
            IEnumerable ie = (IEnumerable)dic;
            if (ie == null)
            {
                s_log.LogResult(false, "IEnumerable is null.");
            }
            else
            {
                s_log.LogResult("Checking IEnumerator in IEnumerable.");
                IEnumerator it = ie.GetEnumerator();
                if (it == null)
                {
                    s_log.LogResult(false, "IEnumerator in IEnumerable is null.");
                }
                else
                {
                    foreach (DictionaryEntry en in ie)
                    {
                        if (en.Key == null)
                            s_log.LogResult(false, "DictionaryEntry is null.");
						s_log.LogResult("Entry : " + string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", ((BamlLocalizableResourceKey)en.Key).Uid, ((BamlLocalizableResourceKey)en.Key).ClassName, ((BamlLocalizableResourceKey)en.Key).PropertyName));
                        it.MoveNext();
                        s_log.LogResult("Check IEnumerator.Current.");
                        if (it.Current == null)
                            s_log.LogResult(false, "IEnumerator.Current is null.");
                        break;
                    }
                }
            }

            // BamlLocalizableResource
            s_log.LogResult("Creating BamlLocalizableResource.");
            BamlLocalizableResource r = new BamlLocalizableResource();
            r.Category = LocalizationCategory.Button;
            r.Comments = AutoData.Extract.GetTestString(1);
            r.Content = AutoData.Extract.GetTestString(0);
            r.Modifiable = true;
            r.Readable = true;

            BamlLocalizableResource r2 = new BamlLocalizableResource();
            r2.Category = LocalizationCategory.Button;
            r2.Comments = AutoData.Extract.GetTestString(1);
            r2.Content = AutoData.Extract.GetTestString(0);
            r2.Modifiable = true;
            r2.Readable = true;
            s_log.LogResult("Check to see BamlLocalizableResource.Equals.");
            if (!r2.Equals(r))
                s_log.LogResult(false, "R2 is not equal to R.");
            s_log.LogResult("Check to see BamlLocalizableResource.GetHashCode().");
            
			BamlLocalizableResourceKey key = new BamlLocalizableResourceKey(AutoData.Extract.GetTestString(2), AutoData.Extract.GetTestString(2), AutoData.Extract.GetTestString(2));
 			BamlLocalizableResourceKey key1 = new BamlLocalizableResourceKey(AutoData.Extract.GetTestString(3), AutoData.Extract.GetTestString(3), AutoData.Extract.GetTestString(3));

            s_log.LogResult("Testing IDictionary.Add()");
            idic.Add(key, r);
            s_log.LogResult("Check to see if it contains Add item.");
            if (!idic.Contains(key))
                s_log.LogResult(false, "IDictionary.Contains return false.");
            s_log.LogResult("Checking idic[key].");
            if (idic[key] == null)
            {
                s_log.LogResult(false, "idic[key] is null.");
            }

            // BamlLocalizationDictionaryEnumerator
            s_log.LogResult("Check BamlLocalizationDictionaryEnumerator.");
            BamlLocalizationDictionaryEnumerator dicenum = dic.GetEnumerator();
            if (dicenum == null)
                s_log.LogResult(false, "BamlLocalizationDictionaryEnumerator is null.");
            s_log.LogResult("Check IDictionaryEnumerator.");
            IDictionaryEnumerator idicenum = (IDictionaryEnumerator)dicenum;
            if (idicenum == null)
                s_log.LogResult(false, "IDictionaryEnumerator is null.");
            foreach (DictionaryEntry en in dic)
            {
                s_log.LogResult("Check BamlLocalizationDictionaryEnumerator.MoveNext()");
                if (!dicenum.MoveNext())
                    s_log.LogResult(false, "BamlLocalizationDictionaryEnumerator is null.");
                s_log.LogResult("Check BamlLocalizationDictionaryEnumerator.Entry");
                if (!en.Equals(dicenum.Entry))
                    s_log.LogResult(false, "BamlLocalizationDictionaryEnumerator is not the same as Current DictionaryEntry.");
                s_log.LogResult("Check BamlLocalizationDictionaryEnumerator.Current.Key");
                if (dicenum.Current.Key == null)
                    s_log.LogResult(false, "BamlLocalizationDictionaryEnumerator.Current.Key is null.");
                s_log.LogResult("Check BamlLocalizationDictionaryEnumerator.Current.Value");
                if (dicenum.Current.Value == null)
                    s_log.LogResult(false, "BamlLocalizationDictionaryEnumerator.Current.Value is null.");
                s_log.LogResult("Check BamlLocalizationDictionaryEnumerator.Key");
                if (dicenum.Key == null)
                    s_log.LogResult(false, "BamlLocalizationDictionaryEnumerator.Key is null.");
                s_log.LogResult("Check BamlLocalizationDictionaryEnumerator.Value");
                if (dicenum.Value == null)
                    s_log.LogResult(false, "BamlLocalizationDictionaryEnumerator.Value is null.");
                s_log.LogResult("Check IDictionaryEnumerator.Entry");
                if (!en.Equals(idicenum.Entry))
                    s_log.LogResult(false, "IDictionaryEnumerator.Entry is not the same as Current DictionaryEntry.");
                s_log.LogResult("Check IDictionaryEnumerator.Key");
                if (idicenum.Key == null)
                    s_log.LogResult(false, "IDictionaryEnumerator.Key is null.");
                s_log.LogResult("Check IDictionaryEnumerator.Value");
                if (idicenum.Value == null)
                    s_log.LogResult(false, "IDictionaryEnumerator.Value is null.");
                s_log.LogResult("Check BamlLocalizationDictionaryEnumerator.Reset()");
                dicenum.Reset();
                dicenum.MoveNext();
                if (!en.Equals(dicenum.Entry))
                    s_log.LogResult(false, "BamlLocalizationDictionaryEnumerator.Reset() Failed.");
                break;
            }
            s_log.LogResult("Clear IDictionary.");
            idic.Clear();
            if (idic.Count > 0)
            {
                s_log.LogResult(false, "Clear IDictionary Failed.");
            }
            s_log.LogResult(true, "TestDictionary::: PASS.");
            s_log.Stage(TestStage.Cleanup);
        }
        #endregion
        #region TestBamlLocDic
        /// <summary>
        /// 
        /// </summary>
        void TestBamlLocDic()
        {
            s_log.Stage(TestStage.Run);
            s_log.LogResult("Parsing Complex.baml...."); 
            string tree = CommonLib.DumpBamlToString("Complex.baml");
            Console.WriteLine(tree);
            Stream stream = File.OpenRead("Complex.baml");
            CreateContext();
            BamlLocalizer locmgr = new BamlLocalizer(stream, new BamlLocalizabilityByReflection());
            BamlLocalizationDictionary dic = locmgr.ExtractResources();
            s_log.LogResult("BamlLocalizationDictionary.Count");
            int count = dic.Count;
            if (count != 292)
            {
                s_log.LogResult(false, "BamlLocalizationDictionary.Count (292): Actual is " + count.ToString());
            }
            s_log.LogResult("BamlLocalizationDictionary.IsFixedSize");
            if (dic.IsFixedSize)
                s_log.LogResult(false, "BamlLocalizationDictionary.IsFixedSize is True.");
            s_log.LogResult("BamlLocalizationDictionary.IsReadOnly");
            if (dic.IsReadOnly)
                s_log.LogResult(false, "BamlLocalizationDictionary.IsReadOnly is True.");
            BamlLocalizableResourceKey ExpectedRoot = new BamlLocalizableResourceKey(
		"DockPanel_1",
		"System.Windows.Controls.DockPanel",
		"$Content"
		);
            BamlLocalizationDictionary d = new BamlLocalizationDictionary();
            DictionaryEntry[] dicen = new DictionaryEntry[292];
            BamlLocalizableResource r = new BamlLocalizableResource();
            r.Category = LocalizationCategory.NeverLocalize;
            r.Comments = AutoData.Extract.GetTestString(2);
            r.Content = AutoData.Extract.GetTestString(1);
            r.Modifiable = true;
            r.Readable = false;
            BamlLocalizableResourceKey key = new BamlLocalizableResourceKey(AutoData.Extract.GetTestString(0), AutoData.Extract.GetTestString(0), AutoData.Extract.GetTestString(0));
            s_log.LogResult("BamlLocalizationDictionary.Add(key, resource).");
            dic.Add(key, r);
            s_log.LogResult("Get BamlLocalizationDictionary.this[key].");
            BamlLocalizableResource getResource = dic[key];
            if (!r.Equals(getResource))
                s_log.LogResult(false, "BamlLocalizationDictionary.Add(key, resource) Failed.");
            s_log.LogResult("Set BamlLocalizationDictionary.this[key].");
            d[key] = dic[key];
            s_log.LogResult("BamlLocalizationDictionary.Contains(key).");
            if (!d.Contains(key))
                s_log.LogResult(false, "Set BamlLocalizationDictionary.this[key] Failed.");
            s_log.LogResult("BamlLocalizationDictionary.Remove(key).");
            dic.Remove(key);
            s_log.LogResult("BamlLocalizationDictionary.Contains(key).");
            if (dic.Contains(key))
                s_log.LogResult(false, "BamlLocalizationDictionary.Remove(key) Failed.");
            s_log.LogResult("BamlLocalizationDictionary.Clear().");
            d.Clear();
            s_log.LogResult("BamlLocalizationDictionary.Contains(key).");
            if (d.Count != 0)
                s_log.LogResult(false, "BamlLocalizationDictionary.Clear() Failed.");
            s_log.LogResult("BamlLocalizationDictionary.Keys.");
            ICollection iKeys = dic.Keys;
            if (iKeys == null)
                s_log.LogResult(false, "BamlLocalizationDictionary.Keys Failed.");
            s_log.LogResult("BamlLocalizationDictionary.Values.");
            ICollection iValues = dic.Values;
            if (iValues == null)
                s_log.LogResult(false, "BamlLocalizationDictionary.Values Failed.");
            s_log.LogResult("BamlLocalizationDictionary.GetEnumerator().");
            BamlLocalizationDictionaryEnumerator dicEnum = dic.GetEnumerator();
            if (dicEnum == null)
                s_log.LogResult(false, "BamlLocalizationDictionary.GetEnumerator() Failed.");
            s_log.LogResult("BamlLocalizationDictionary.CopyTo(dicEntry[], 0).");
            dic.CopyTo(dicen, 0);
            for (int i = 0; i < dicen.Length; i++)
            {
                if(dicen[i].Key == null)
                    s_log.LogResult(false, "BamlLocalizationDictionary.CopyTo(dicEntry[], 0) Failed.");
            }
            s_log.LogResult(true, "BamlLocalizationDictionary Test PASS!!.");
            s_log.Stage(TestStage.Cleanup);
        }
        #endregion
        #region Helper
        public BamlLocalizationDictionary this[string key]
        {
            get
            {
                return (BamlLocalizationDictionary)_table[key];
            }
            set
            {
                _table[key] = value;
            }
        }
        private Hashtable _table;
        public UIElement LoadBAML(string filename)
        {
            s_log.LogResult("Loading BAML...");
            Uri resourceUri = new Uri("pack://siteoforigin:,,,/" + filename, UriKind.RelativeOrAbsolute);
            Stream s = Application.GetRemoteStream(resourceUri).Stream;
            Type xamlReaderType = typeof(XamlReader);
            MethodInfo m = xamlReaderType.GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static);
            ParserContext p = new ParserContext();
            UIElement root = m.Invoke(null, new object[] { s, p, null, false }) as UIElement;
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
        static Dispatcher s_dispatcher;
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;
        }
        #endregion
    }
}
