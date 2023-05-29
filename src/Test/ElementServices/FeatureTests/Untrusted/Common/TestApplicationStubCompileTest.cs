// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:   
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// Compiles using a TestApplicationStub as the ApplicationDefinition. This will be the entry
    /// point of for the test case.
    /// </summary>
    public static class TestApplicationStubCompileTest
    {

        /// <summary>
        /// Save the predefine ApplicationDefinition and compile.
        /// It will compile using MSBuild, the proj file will be created using the specified
        /// TestApplicationStubCompileTestParams.
        /// </summary>
        static public void Compile(TestApplicationStubCompileTestParams param)
        {
            param.VerifyHostType();
            Dictionary<string,string> xmlnsDictionary = new Dictionary<string,string>();
            string xamlApplicationStubString = string.Empty;
            xmlnsDictionary.Add("harness", "clr-namespace:Avalon.Test.CoreUI.Common;assembly=CoreTestsUntrusted");
            xmlnsDictionary.Add("hostTypeNS", "clr-namespace:Avalon.Test.CoreUI.Common;assembly=CoreTestsUntrustedBase");

            if (param.TestCaseAssemblyName != CoreTests.CoreTestsDefaultAssemblies)
            {
                xmlnsDictionary.Add("testspace", "clr-namespace:" + param.ClrNameSpace + ";assembly=" + param.TestCaseAssemblyName);
            }
            else
            {
                Type classType = Utility.FindType(param.ClrNameSpace + "." + param.TestCaseClassName, true);

                if (classType == null)
                {
                    throw new InvalidOperationException(param.ClrNameSpace + "." + param.TestCaseClassName + " was not found.");
                }

                string assemblyName = Path.GetFileNameWithoutExtension(classType.Assembly.GetName().Name);
                xmlnsDictionary.Add("testspace", "clr-namespace:" + param.ClrNameSpace + ";assembly=" + assemblyName);

            }


            xamlApplicationStubString += 
                "\n<xh:TestApplicationStub\n xmlns:xh=\"" + xmlnsDictionary["harness"] + "\"\n xmlns:xhost=\""+xmlnsDictionary["hostTypeNS"]+"\"\n xmlns:" + param.XmlPrefix + "=\"" + xmlnsDictionary["testspace"] + "\"\n xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"" +
                "\n HostType=\"" + param.HostType.ToString() + "\"" +
                "\n TestEntryPointMethodName=\"" + param.EntryMethodName + "\"" + ">" +
                "\n   <xh:TestApplicationStub.CurrentHostedTest>" +
                "\n       <" + param.XmlPrefix + ":" + param.TestCaseClassName + " /> " +
                "\n   </xh:TestApplicationStub.CurrentHostedTest>" +
                "\n</xh:TestApplicationStub>";

            IOHelper.SaveTextToFile(xamlApplicationStubString, param.ApplicationDefinition);

            Microsoft.Test.MSBuildEngine.Compiler compiler = new Microsoft.Test.MSBuildEngine.Compiler(param);
            compiler.Compile(true);
        }

    }



    /// <summary>
    /// Specilized usage for creating the Proj File. This class is for the special use of
    /// TestApplicationStubCompileTest class
    /// </summary>
    [Serializable()]
    public class TestApplicationStubCompileTestParams : CoreCompilerParams
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestApplicationStubCompileTestParams(string clrNameSpace,
            string assembly,
            string testCaseClassName,
            string entryMethodName,
            HostType hostType)
            : this(true, "coretest", clrNameSpace, assembly, testCaseClassName, entryMethodName, hostType) { }


        /// <summary>
        /// Constructor.        
        /// </summary>
        public TestApplicationStubCompileTestParams(bool useDefaultCoreDlls, string xmlPrefix,
            string clrNameSpace,
            string assembly,
            string testCaseClassName,
            string entryMethodName,
            HostType hostType)
            : base(useDefaultCoreDlls)
        {
            EntryMethodName = entryMethodName;
            XmlPrefix = xmlPrefix;
            ClrNameSpace = clrNameSpace;
            TestCaseAssemblyName = assembly;
            TestCaseClassName = testCaseClassName;
            base.ApplicationDefinition = "XamlApplicationStub.xaml";
            this.HostType = hostType;

            VerifyHostType();
        }

        /// <summary>
        /// Make sure that if you specified Browser the Proj file sets HostInBrowser
        /// </summary>
        public void VerifyHostType()
        {
            if (this.HostType == Avalon.Test.CoreUI.Common.HostType.Browser)
            {
                base.HostInBrowser = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Avalon.Test.CoreUI.Common.HostType HostType = HostType.Browser;

        /// <summary>
        /// Xml prefix used on the ApplicationDefinition file for the IHostedTest
        /// </summary>
        public string XmlPrefix;

        /// <summary>
        /// This is the ClrNameSpace for the IHostedTest test live in.       
        /// </summary>
        public string ClrNameSpace;

        /// <summary>
        /// This is the name of the assembly where the IHostedTest test live in.
        /// </summary>
        public string TestCaseAssemblyName;

        /// <summary>
        /// This is the IHostedTest Class Name.
        /// </summary>
        public string TestCaseClassName;

        /// <summary>
        /// This is the name of the method that will start the test on the IHostedTest
        /// </summary>
        public string EntryMethodName;

    }
}
