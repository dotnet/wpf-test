// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.ComponentModel;
using System.IO;
using Microsoft.Test.Logging;
using System.Threading;

namespace Microsoft.Test.Integration
{
    ///<summary>
    ///</summary>    
    [ContentProperty("TestCases")]
    public class TestExtenderOutput
    {
        ///<summary>
        ///</summary>
        static public TestExtenderOutput Load(Stream txro)
        {
            object o = XamlReader.Load(txro);

            return o as TestExtenderOutput;
        }

        ///<summary>
        ///</summary>
        static public TestExtenderOutput Load(string txro)
        {
            object o = null;

            using (FileStream fs = new FileStream(txro, FileMode.Open, FileAccess.Read))
            {
                o = XamlReader.Load(fs);
            }

            return o as TestExtenderOutput;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        static internal void MergeTEO(TestExtenderOutput destination, TestExtenderOutput source)
        {
            destination.TestCases.AddRange(source.TestCases);
        }


        ///<summary>
        ///</summary>
        public void Save(string file)
        {
            using (FileStream fs = new FileStream(file,FileMode.Create,FileAccess.Write))
            {
                Save(fs);    
            }            
        }

        ///<summary>
        ///</summary>
        public void Save(Stream stream)
        {
            XamlWriter.Save(this, stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public string Save(int testCase)
        {
            if (_testCases.Count == 0)
            {
                return "";
            }

            if (testCase >= _testCases.Count)
            {
                throw new IndexOutOfRangeException();
            }

            // Copy single case to new empty TXRO.
            // Mark single case as NOT root so the TestContract
            // values will not be serialized.
            TestExtenderOutput txro = new TestExtenderOutput();
            VariationItem item = this.TestCases[testCase];
            item.IsRoot = false;
            txro.TestCases.Add(item);

            return XamlWriter.Save(txro);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExecuteTests()
        {
            ExecuteTests(0, TestCases.Count - 1);
        }

        ///<summary>
        ///</summary>
        public void ExecuteTests(int start, int end)
        {
            if (_testCases.Count == 0)
            {
                return;
            }

            if (start < -1 || end < -1 || end >= _testCases.Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (start == -1)
            {
                start = 0;
            }

            if (end == -1)
            {
                end = _testCases.Count - 1;
            }


            for (int index = start; index <= end; index++)
            {
                GlobalLog.LogStatus("*************** Starting Test Case # " + index.ToString() + "***************");

                CommonStorage.CleanAll();
                TestCases[index].Execute();

                GlobalLog.LogStatus("*************** End Test Case # " + index.ToString() + "***************");
                GlobalLog.LogStatus("");
                GlobalLog.LogStatus("");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">Based in 0 index</param>
        public void ExecuteTest(int index)
        {
            ExecuteTests(index, index);
        }

        ///<summary>
        ///</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public VariationItemCollection TestCases
        {
            get
            {
                return _testCases;
            }
        }

        VariationItemCollection _testCases = new VariationItemCollection();
    }
}
