// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose: Used for CoreUI's compilation test variations.
*             
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Test.MSBuildEngine;

using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class SecurityHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="testDefinition"></param>
        /// <returns></returns>
        public static bool IsFullTrust(TestDefinition testDefinition)
        {
            return String.Equals(testDefinition.Parameters["SecurityLevel"], "FullTrust", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    /// <summary>
    /// Common options used in generated proj file for CoreUI's compiler test variations. 
    /// By default, adds proj references to CoreTests components, ClientTestRuntime, and ClientTestLibrary.
    /// Also, sets the proj TargetZone according to the current TestCaseInfo.SecurityLevel, and adds
    /// the current TestCaseInfo support files as loose file references.
    /// </summary> 
    [Serializable()]
    public class CoreCompilerParams : CompilerParams
    {
        /// <summary>
        /// </summary> 
        public CoreCompilerParams(bool useDefaultCoreDlls) : base(useDefaultCoreDlls)
        {
            if (useDefaultCoreDlls)
            {
                string currentDirectory = Environment.CurrentDirectory;
                References.Add(new Reference("CoreTestsUntrusted", currentDirectory + "\\CoreTestsUntrusted.dll"));
                References.Add(new Reference("CoreTestsUntrustedBase", currentDirectory + "\\CoreTestsUntrustedBase.dll"));
                References.Add(new Reference("CoreTestsUntrustedCore", currentDirectory + "\\CoreTestsUntrustedCore.dll"));                
                References.Add(new Reference("CoreTestsTrusted", currentDirectory + "\\CoreTestsTrusted.dll"));
                References.Add(new Reference("TestRuntime", currentDirectory + "\\TestRuntime.dll"));

                // Here we set the security zone for the compile test case.

                TestDefinition test = TestDefinition.Current;

                if (test == null)
                {
                    CoreLogger.LogStatus("TestDefinition cannot be found.");
                }

                TargetZone = "Internet";

                if (test != null && SecurityHelper.IsFullTrust(test))
                {
                    CoreLogger.LogStatus("Setting proj file to be Full Trust.");
                    TargetZone = ""; // FullTrust
                }
                else
                {
                    CoreLogger.LogStatus("Setting proj file to be Partial Trust.");
                }
            }

            AssemblyName = "CoreTestGeneric";
        }
    }
}

