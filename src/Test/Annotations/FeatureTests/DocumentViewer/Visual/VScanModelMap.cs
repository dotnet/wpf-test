// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Map that contains all the vscan models used by the DocumentViewer visual tests.

using System;
using System.Windows;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Collections.Generic;
using System.Collections;				

namespace Avalon.Test.Annotations
{
	public class VScanModelMap 
	{
        static VScanModelMap()
        {
            SetupMap();
        }

        /// <summary>
        /// Get the absolute path the the VScan model for the given test and mode or return String.empty if
        /// none is set.
        /// </summary>
        /// <param name="caseNumber">Name of test case to get Model for.</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GetModel(AScenarioSuite suite)
        {
            string model = string.Empty;
            if (suite.VerifyVisuals)
            {
                ModelKey key = new ModelKey(suite.CaseNumber, suite.ContentMode, suite.AnnotationType);
                if (_modelMap.ContainsKey(key))
                    model = _modelMap[key];
            }
            return model;
        }

        private static void SetupMap() 
        {
            _modelMap.Add(new ModelKey("scenario1_1", TestMode.Flow, AnnotationMode.StickyNote), FLOW_DIRECTORY + "startofdocument_flow_sn_model.Analytical.vscan");
        }

        static string MODEL_BASE_PATH = @"%SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\annotations\TestCases\DocumentViewer\Visual\bvt\";
        static string FLOW_DIRECTORY = MODEL_BASE_PATH + @"Flow\";
        static string FIXED_DIRECTORY = MODEL_BASE_PATH + @"Fixed\";

        /// <summary>
        /// Map from KeyValuePair of CaseNumber and TestMode to absolute path of VScan model.
        /// </summary>
        static IDictionary<ModelKey, string> _modelMap = new Dictionary<ModelKey, string>();

        /// <summary>
        /// Inner class used for hashing into a dictionary to find the correct VScan model.
        /// </summary>
        class ModelKey
        {
            public ModelKey(string testname, TestMode testMode, AnnotationMode annotationMode)
            {
                _testname = testname;
                _testmode = testMode;
                _annotationMode = annotationMode;
            }

            public override int GetHashCode()
            {
                string code = _testname + _testmode.ToString() + _annotationMode.ToString();
                return code.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                ModelKey key = obj as ModelKey;
                if (key == null)
                    return false;
                return string.Equals(key._testname, _testname) && key._testmode == _testmode && key._annotationMode == _annotationMode;
            }

            public string _testname;
            public TestMode _testmode;
            public AnnotationMode _annotationMode;
        }
    }
}	

