// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Framework;
using Microsoft.Test.Xaml.Parser;
using Microsoft.Test.Xaml.Serialization;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// XamlTestType for XamlRoundTripTest
    /// </summary>
    public class XamlRoundTripTest : XamlTestType
    {
        #region XamlTestType Members

        /// <summary>
        /// Runs a XamlRoundTripTest, which consists of:
        ///     Loading original xaml
        ///     serialize to new file
        ///     compare new file with pre-serialized file
        ///     load serialized file
        ///     compare object trees
        /// NOTE: This test requires that both the original xaml file as well as the pre-serialized file
        /// be present in the working directory.  The pre-serialized file must have the name format of:
        /// originalName.SERIALIZED.xaml
        /// </summary>
        public override void Run()
        {
            string originalFile = DriverState.DriverParameters["File"];
            bool verifyObjectCreated = false;
            string preSerializedFile = originalFile.Substring(0, originalFile.Length - 5) + ".SERIALIZED.xaml";
            string newSerializedFile = "_XamlRoundTrip_TempFile.xaml";
            object originalRoot = null;
            object reloadRoot = null;

            // This causes the WPF assemblies to load, this is a hack that will be removed later
            FrameworkElement frameworkElement = new FrameworkElement();
            frameworkElement = null;

            IXamlTestParser parser = XamlTestParserFactory.Create();
            IXamlTestSerializer serializer = XamlTestSerializerFactory.Create();

            if (String.IsNullOrEmpty(originalFile))
            {
                throw new Exception("originalFile cannot be null");
            }

            if (!String.IsNullOrEmpty(DriverState.DriverParameters["VerifyObjectCreated"]))
            {
                verifyObjectCreated = bool.Parse(DriverState.DriverParameters["VerifyObjectCreated"]);
            }

            TestLog log = new TestLog(DriverState.TestName);

            // Load any supporting assemblies
            if (!String.IsNullOrEmpty(DriverState.DriverParameters["SupportingAssemblies"]))
            {
                string assemblies = DriverState.DriverParameters["SupportingAssemblies"];
                log.LogStatus("Loading Assemblies: " + assemblies);
                FrameworkHelper.LoadSupportingAssemblies(assemblies);
            }
            else
            {
                log.LogStatus("No Supporting assemblies loaded.");
            }

            try
            {
                // Load original xaml file            
                log.LogStatus("Loading original xaml file...");
                originalRoot = parser.LoadXaml(originalFile, null); // load with default settings

                // Serialize object tree
                log.LogStatus("Serializing object tree...");
                serializer.SerializeObjectTree(newSerializedFile, originalRoot);

                // Compare newly serialized xaml file with preserialized file
                log.LogStatus("Comparing xaml files...");
                if (!ComparisonServices.CompareXamlFiles(preSerializedFile, newSerializedFile))
                {
                    log.Result = TestResult.Fail;
                    log.LogEvidence("Xaml file comparison failed");
                    log.LogFile(preSerializedFile);
                    log.LogFile(newSerializedFile);
                    log.LogFile(originalFile);
                }

                // Reload serialized xaml and compare object trees
                log.LogStatus("Loading serialized xaml file...");
                reloadRoot = parser.LoadXaml(newSerializedFile, null);

                if (verifyObjectCreated)
                {
                    log.LogStatus("Ensuring object tree is created ...");
                    if ((reloadRoot == null) || (!reloadRoot.GetType().Equals(originalRoot.GetType())))
                    {
                        log.Result = TestResult.Fail;
                        log.LogEvidence("Object tree creation failed");
                        log.LogFile(originalFile);
                        log.LogFile(newSerializedFile);
                    }
                }
                else
                {
                    log.LogStatus("Comparing object trees...");
                    if (!ComparisonServices.CompareObjectTrees(originalRoot, reloadRoot))
                    {
                        log.Result = TestResult.Fail;
                        log.LogEvidence("Object tree comparison failed");
                        log.LogFile(originalFile);
                        log.LogFile(newSerializedFile);
                    }
                }

                log.Result = TestResult.Pass;
                log.Close();
            }
            catch (Exception e)
            {
                log.LogEvidence(e);
                log.Result = TestResult.Fail;
                log.Close();
            }
        }

        #endregion
    }
}
