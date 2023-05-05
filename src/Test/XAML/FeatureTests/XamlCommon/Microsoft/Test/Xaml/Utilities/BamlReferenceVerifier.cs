// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Xaml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Utilities
{
    /******************************************************************************
    * CLASS:          BamlReferenceVerifier
    ******************************************************************************/

    /// <summary>
    /// Class for verifying References in Baml.
    /// </summary>
    public class BamlReferenceVerifier
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          CheckReferenceVersion
        ******************************************************************************/

        /// <summary>
        /// Verifies that the wpf version is correct for the given .baml file.
        /// </summary>
        /// <param name="bamlFileName">The name of the .baml file being checked.</param>
        /// <param name="exeFilePath">The location of the .baml file.</param>
        /// <param name="unexpectedVersion">The version of wpf that should not be present in the assembly.</param>
        /// <returns>A boolean indicating whether or not the verification succeeded</returns>
        public static bool CheckReferenceVersion(string bamlFileName, string exeFilePath, string unexpectedVersion)
        {
            bool referenceVersionCorrect = true;

            string grabbedBamlPath = bamlFileName.Replace(".baml", "_grabbed.baml");
            BamlHelper.ExtractBamlResource(exeFilePath, grabbedBamlPath);

            GlobalLog.LogEvidence("GrabbedBaml = " + grabbedBamlPath);

            List<string> references = BamlHelper.GetReferencesInBaml(grabbedBamlPath);
            if (references.Count == 0)
            {
                throw new TestSetupException("No assembly references were found in the baml");
            }

            foreach (string str in references)
            {
                GlobalLog.LogStatus("reference: " + str);

                // Make sure that 'unexpectedVersion' is -not- present in the compiled assemblies.
                // mscorlib is a special case where the correct version is loaded as required.
                if (str.Contains(unexpectedVersion) && !str.Contains("mscorlib"))
                {
                    referenceVersionCorrect = false;
                    GlobalLog.LogEvidence("FAIL: Found " + unexpectedVersion + " version reference in baml.");
                }
            }

            return referenceVersionCorrect;
        }

        #endregion
    }
}
