// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Test constants common to all Integration TestHarness test cases.
//
// $Id:$ $Change:$
using System;


namespace Microsoft.Test.Animation
{

    /// <summary>
    /// Provides a way to filter results by categories and
    /// compare regressions of multiple part tests.
    /// </summary>
    public class CommonConstants
    {

        /// <summary>
        /// System.Windows related namespaces
        /// </summary>
        public const string NamespaceSystemWindows = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        /// <summary>
        /// Test pass message
        /// </summary>
        public const string TestPass = "Test PASS" ;
        /// <summary>
        /// Test fail message
        /// </summary>
        public const string TestFail = "Test FAIL" ;
        /// <summary>
        /// Test ignore message
        /// </summary>
        public const string TestIgnore = "Test IGNORE" ;
        /// <summary>
        /// Test ignore message
        /// </summary>
        public const string TestInternalFailure = "Test INTERNAL FAILURE" ;



        /// <summary>
        /// Key used to mark the repetitions parameter from the current application
        /// </summary>
        public const string flagRepetitions = "__TEST_REPETITIONS__" ;
        /// <summary>
        /// Key used to mark the target parameter from the current application
        /// </summary>
        public const string flagTarget = "__TEST_TARGET__" ;
        /// <summary>
        /// Key used to mark the loop parameter from the current application
        /// </summary>
        public const string flagLoop = "__TEST_LOOP_URL__" ;
        /// <summary>
        /// Key used to mark the proxy target parameter from the current application
        /// </summary>
        public const string flagProxyTarget = "__TEST_PROXY_TARGET_URL__" ;
        /// <summary>
        /// Key used to mark the test element parameter from the current application
        /// </summary>
        public const string flagTestElement = "testElement" ;
        /// <summary>
        /// Key used to mark the test variation parameter from the current application. Used in the HLS
        /// tests to select a subtest.
        /// </summary>
        public const string flagTestVariation = "TestVariation" ;
        /// <summary>
        /// Key used to force the test to end
        /// </summary>
        public const string flagKill = "__TEST_KILL__" ;
        /// <summary>
        /// Key used to pass in parameters to the filtered driver cases
        /// </summary>
        public const string flagFilterString = "__TEST_FILTER_STRING__" ;


        /// <summary>
        /// Internal Proxy Page
        /// </summary>
        public const string proxyPage = "IntegrationProxy.xaml" ;

        /// <summary>
        /// Animation test inter time value
        /// </summary>
        public const int AnimationWaitTime = 400 ;

        /// <summary>
        /// Animation test inter time value
        /// </summary>
        public const int UIBindingWaitTime = 200 ;

        /// <summary>
        /// File that contains Markup for Elements
        /// </summary>
        public const string ElementFile = @"MarkupElements.xaml";
    }
}
