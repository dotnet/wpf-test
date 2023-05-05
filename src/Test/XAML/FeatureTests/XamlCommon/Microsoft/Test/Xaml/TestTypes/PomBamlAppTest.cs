// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Test.Baml.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;
using Microsoft.Win32;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// PomBamlAppTest localizes a pre-compiled app to the specified target
    /// languages.  Run the app each time to verify valid localization
    /// </summary>
    public class PomBamlAppTest : XamlTestType
    {
        /// <summary>
        /// Run the PomBaml test
        /// </summary>
        public override void Run()
        {
            TestLog log = new TestLog(DriverState.TestName);
            string appName = DriverState.DriverParameters["AppName"];
            string targetCulture = "ja-JP";
            string currentUICulture = CultureInfo.CurrentUICulture.ToString();
            string srcResourcesDll = string.Format("{0}.resources.dll", appName);
            string destResourcesDll = string.Format("{0}\\{1}.resources.dll", targetCulture, appName);
            string currentUICultureResourcesDll = string.Format("{0}\\{1}", currentUICulture, srcResourcesDll);            
            List<BamlString> originalStrings = new List<BamlString>();
            List<BamlString> localizedStrings = new List<BamlString>();
            int applicationTimeout = 30000;

            // Copy original resource dll to current UI culture folder
            if (!Directory.Exists(currentUICulture))
            {
                Directory.CreateDirectory(currentUICulture);
            }

            File.Copy(srcResourcesDll, currentUICultureResourcesDll);

            // Run app with original resources
            if (!LocHelper.RunApplication(appName, string.Empty, applicationTimeout))
            {
                log.LogEvidence("App failed with original resources");
                log.Result = TestResult.Fail;
                goto END;
            }

            // Extract strings from non-localized resource dll
            try
            {
               originalStrings = LocHelper.ExtractStringsFromResourceBinary(srcResourcesDll, "OriginalRes.dasm");
            }
            catch (Exception ex2)
            {
                log.LogStatus("Extraction Failed fo original resources dll");
                log.LogStatus(ex2.ToString());
                log.Result = TestResult.Fail;
                goto END;
            }

            // localize (psuedo) the resource dll
            try
            {
                if (!LocHelper.Localize(srcResourcesDll, destResourcesDll, targetCulture))
                {
                    log.LogEvidence("LSBuild failed");
                    log.Result = TestResult.Fail;
                    goto END;
                }
            }
            catch (Exception ex)
            {
                log.LogStatus("Localization Failed");
                log.LogStatus(ex.ToString());
                log.Result = TestResult.Fail;
                goto END;
            }        

            // Exctract the localized strings
            try
            {
                localizedStrings = LocHelper.ExtractStringsFromResourceBinary(destResourcesDll, "LocalizedRes.dasm");
            }
            catch (Exception ex2)
            {
                log.LogStatus("Extraction Failed for localized dll");
                log.LogStatus(ex2.ToString());
                log.Result = TestResult.Fail;
                goto END;
            }

            // Run app with new resources
            if (!LocHelper.RunApplication(appName, "ja-JP", applicationTimeout))
            {
                log.LogEvidence("App failed with localized resources");
                log.Result = TestResult.Fail;
                goto END;
            }

            // Compare original vs. localized strings
            if (!LocHelper.CompareExtractedStrings(originalStrings, localizedStrings))
            {
                log.LogEvidence("String comparison failed");
                log.Result = TestResult.Fail;
            }

            log.Result = TestResult.Pass;
            END:
            log.Close();
        }
    }
}
