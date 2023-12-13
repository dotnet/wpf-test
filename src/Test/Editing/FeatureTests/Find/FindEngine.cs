// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for the FindEngine class.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Documents;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the FindEngine constructor can be created correctly.
    /// appropriate times.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("342")]
    public class FindEngineCreation: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            VerifyInvalidCalls();
            VerifyValidCalls();

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void ThrowAccepts(string description)
        {
            throw new ApplicationException(
                "FindEngine accepts " + description + " in constructor.");
        }

        private void LogRejects(string description)
        {
            Log("FindEngine rejects " + description + " in constructor.");
        }

        private void VerifyInvalidCalls()
        {
            Log("Verifying invalid calls to the FindEngine constructor...");
            string pattern = "pattern";

            try
            {
                new FindEngine(null, (FindOptions) 0);
                ThrowAccepts("null pattern");
            }
            catch(SystemException)
            {
                LogRejects("null pattern");
            }
            try
            {
                new FindEngine(pattern, (FindOptions) 0, null);
                ThrowAccepts("null culture");
            }
            catch(SystemException)
            {
                LogRejects("null culture");
            }
            try
            {
                new FindEngine(pattern, unchecked((FindOptions) 0xFFFFFFFF));
                ThrowAccepts("invalid options");
            }
            catch(SystemException)
            {
                LogRejects("invalid options");
            }
        }

        private void VerifyValidCalls()
        {
            Log("Verifying valid calls to the FindEngine constructor...");
            string pattern = "pattern";

            FindOptions[] validOptions = new FindOptions[] {
                FindOptions.None,
                FindOptions.None | FindOptions.Backward,
                FindOptions.MatchWholeWords,
                FindOptions.IgnoreCase,
            };
            for (int i = 0; i < validOptions.Length; i++)
            {
                FindEngine engine;
                FindOptions options = validOptions[i];

                Log("Attempting construction with options: " + options.ToString());
                engine = new FindEngine(pattern, options);
                Log("Construction succeeded.");
            }

            Log("Attempting construction with current culture...");
            new FindEngine(pattern, FindOptions.None, CultureInfo.CurrentCulture);
        }

        #endregion Verifications.
    }
    
    // FindEngineReproRegression_Bug898
    // This test case existed to verify that the FindEngine constructor 
    // rejects conflicting culture option arguments. The current
    // FindOptions enumeration does not allow for this conflict anymore,
    // therefore the test case has been removed.
}
