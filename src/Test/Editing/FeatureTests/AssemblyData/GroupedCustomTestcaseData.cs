// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Assembly-level test case metadata.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;

    using System.Windows;
    using System.Windows.Controls;    
    using System.Windows.Documents;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Test case to run a group of customTestCases.
    /// </summary>
    [Test(1, "Editor", "KeyboardActionCases", MethodParameters = "/TestCaseType=GroupedCustomTestCase /GroupName=KeyboardActionCases")]
    [Test(1, "Selection", "SelectionCases", MethodParameters = "/TestCaseType=GroupedCustomTestCase /GroupName=SelectionCases", Timeout=120)]
    [Test(1, "RichEditing", "BidiEditingTestCases", MethodParameters = "/TestCaseType=GroupedCustomTestCase /GroupName:BidiEditingTestCases")]
    [Test(2, "RichEditing", "AnimationRegressionCases", MethodParameters = "/TestCaseType:GroupedCustomTestCase /GroupName:AnimationRegressionCases")]
    [Test(1, "DataObject", "DataTransferCases", MethodParameters = "/TestCaseType=GroupedCustomTestCase /GroupName=DataTransferCases")]
    [TestOwner("Microsoft"), TestTactics("25,26")]
    public class GroupedCustomTestCase : CombinedTestCaseManager
    {
        /// <summary>
        /// Get the group of test cases.
        /// </summary>
        /// <returns></returns>
        public override CombinedTestCase[] GetTestCases()
        {
            string groupName; 
            groupName = Settings.GetArgument("GroupName");
            return GroupedCustomTestcaseData.GetCombinedCustomCases(groupName);
        }
    }
   
    /// <summary>Test case data for test cases in this assembly.</summary>
    public static class GroupedCustomTestcaseData
    {
        /// <summary>
        /// this method helps to find a group of cases. 
        /// </summary>
        /// <param name="groupName">the groupName is used to search the static filed in this class
        /// each field contains an array of test class name.</param>
        /// <returns></returns>
        public static CombinedTestCase[] GetCombinedCustomCases(string groupName)
        {
            CombinedTestCase[] results;
            string[] caseNames;
            caseNames = Test.Uis.Utils.ReflectionUtils.GetStaticField(typeof(Test.Uis.TextEditing.GroupedCustomTestcaseData), groupName) as string[];

            results = new CombinedTestCase[caseNames.Length];
           
            //each case is specify by its class name, we will create the classes here.
            for (int i = 0; i < caseNames.Length; i++)
            {
                Type type = ReflectionUtils.FindType(caseNames[i]);
                results[i] = ReflectionUtils.CreateInstance(type) as CombinedTestCase;
                
                //make sure that the test case is created.
                if (results[i] == null)
                {
                    throw new Exception("Can't create the instance of " + type.FullName);
                }
            }

            return results; 
        }

        //Add groups here alphabetically...

        /// <summary>
        /// This group contains animation related regression cases.
        /// TestCase#: 26
        /// </summary>
        public static string[] AnimationRegressionCases = new string[]        
        {
            "TBBAnimationTest",             
        };

        /// <summary>
        /// This group contains Bidi editing related test cases.
        /// TestCase#: 27
        /// </summary>
        public static string[] BidiEditingTestCases = new string[]        
        {
            "DeleteRTLRunBtwnLTRRuns",
            "FDSwitchWithBidiContent", 
            "RegressionTest_Regression_Bug307",
        };

        /// <summary>This group DataTransfer cases. Test case #28</summary>
        public static string[] DataTransferCases = new string[]        
        {
            "DataObjectAPI",
            "DataObjectPastingEventArgsAPITest",
            "RegressionTest_Regression_Bug387",
        };

        /// <summary>This group contains keyboard editing cases, list editing cases. Tactics test case #25</summary>
        public static string[] KeyboardActionCases = new string[]        
        {
            "RegressionTest_Regression_Bug196",
            "RegressionTest_Regression_Bug197",
            "RegressionTest_Regression_Bug447",
            "RegressionTest_Regression_Bug198",
            "RegressionTest_Regression_Bug448",
            "RegressionTest_Regression_Bug522",
            "RegressionTest_Regression_Bug449",
            "RegressionTest_Regression_Bug86",
            "RegressionTest_Regression_Bug450",
            "RegressionTest_Regression_Bug199",
        };

        /// <summary>This group contains selection regression cases. Test case # in tactics: 29</summary>
        public static string[] SelectionCases = new string[]        
        {
            "RegressionTest_Regression_Bug671",
            "EmptyTableSelection",
            "RegressionTest_Regression_Bug684",
            "RegressionTest_Regression_Bug583",
 
            //this case is not selection scenario, put it here temperarily. 
            "BlockUIContainerAPITest",

            //will move to other test group.
            "RegressionTest_Regression_Bug386",
        }; 
    }
}