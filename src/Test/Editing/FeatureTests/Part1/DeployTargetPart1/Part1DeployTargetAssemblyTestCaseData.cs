// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Assembly-level test case metadata for EditingTestPart1
//  partial trust tests.

using System;

using Test.Uis.Data;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.Utils;

namespace Microsoft.Test.Editing
{    
    /// <summary>Test case data for test cases in this assembly.</summary>
    [TestCaseDataTableClass]
    public static class Part1DeployTargetAssemblyTestCaseData
    {
        /// <summary>Test case data for test cases in this assembly.</summary>
        [TestCaseDataTable]
        public static TestCaseData[] Data = new TestCaseData[] {                      
                           
            new TestCaseData(typeof(CustomSpellerDictionaryTest), "XbapName=EditingTestDeployPart1",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("testContent", new object[]{"xmadeupstringx", "Ymadeupstringy" }),
                new Dimension("testCustomDictionaryUris", new object[]
                {
                    new Uri[] {
                        new Uri("pack://application:,,,/EditingTestDeployPart1;component/CustomSpellerDictionaryResourceLocal.lex")                     
                    },                    
                    new Uri[] {
                       new Uri("pack://application:,,,/EditingTestLib;component/CustomSpellerDictionaryResourceReferenced.lex")                   
                    }                   
                }),
                new Dimension("testClearCustomDictionary", BooleanValues)),                                                           

            new TestCaseData(typeof(CustomSpellerDictionaryInvalidInPTTest), "XbapName=EditingTestDeployPart1",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),                
                new Dimension("testCustomDictionaryUriType", new object[] {"uncfile", "uncfile_invalid", "siteoforgin", "siteoforgin_invalid", "localfile", "localfile_invalid"})),

            new TestCaseData(typeof(ProgrammaticAccessToClipBoardInPTTest), "XbapName=EditingTestDeployPart1",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")})),                
                
        };

        #region Private properties.

        /// <summary>Array with static boolean values.</summary>
        private static object[] BooleanValues
        {
            get
            {
                if (s_booleanValues == null)
                {
                    s_booleanValues = new object[] { true, false };
                }
                return s_booleanValues;
            }
        }

        #endregion

        #region Private fields.

        /// <summary>Array with static boolean values.</summary>
        private static object[] s_booleanValues;

        #endregion     
    }
}
