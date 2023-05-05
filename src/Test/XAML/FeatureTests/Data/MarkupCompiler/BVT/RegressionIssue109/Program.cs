// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Markup;
using System.Workflow.ComponentModel;
//using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;

namespace RegressionIssue109
{
    /// <summary>
    /// Verify that property element initialization doesn't get changed to attribute initilaization by XamlReader/XamlWriter
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ParserContext context = new ParserContext();
            context.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
            context.XamlTypeMapper.AddMappingProcessingInstruction("http://my", "RegressionIssue109", "RegressionIssue109");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            object xomlObjectI = null;
            object xomlObjectII = null;

            try
            {
                //
                using (Stream readerI = new FileStream(@"Markup.xoml", FileMode.Open))
                {
                    xomlObjectI = XamlReader.Load(readerI, context);
                }

                using (XmlWriter writerI = XmlTextWriter.Create(@"OutI_Markup.xoml", settings))
                {
                    XamlWriter.Save(xomlObjectI, writerI);
                }

                using (Stream readerII = new FileStream(@"OutI_Markup.xoml", FileMode.Open))
                {
                    xomlObjectII = XamlReader.Load(readerII, context);
                }

                using (XmlWriter writerII = XmlTextWriter.Create(@"OutII_Markup.xoml", settings))
                {
                    XamlWriter.Save(xomlObjectII, writerII);
                }
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Exception from Program.Main: " + exception.ToString());
                Microsoft.Test.Logging.TestLog log = TestLog.Current;
                log.Result = TestResult.Fail;
                Environment.Exit(-1);
            }
        }
    }

    public class ClassA
    {
        private ActivityCondition _property1;
        public ActivityCondition Property1
        {
            get
            {
                return _property1;
            }
            set
            {
                _property1 = value;
            }
        }
    
    }

    public class MyDerived : ActivityCondition 
    {
        public MyDerived()
        {
        }

        public override bool Evaluate(Activity ownerActivity, IServiceProvider provider)
        {
            return true;
        }
    }
}
