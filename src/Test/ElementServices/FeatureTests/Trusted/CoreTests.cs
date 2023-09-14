// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Abstract base class for all types of drivers.  The various
 *          driver implementations control the initialization of our micro-TCM,
 *          loop thru the cases (could be a single case or many), and 
 *          produce a final report of failures.
********************************************************************/
using System;

namespace Avalon.Test.CoreUI.Trusted
{
    ///<summary/>    
    public abstract class CoreTests : MarshalByRefObject
    {        
        /// <summary>
        /// Comma-separated list of assemblies to look for case metadata in.
        /// </summary>
        /// <remarks>
        /// This is used in looking for case metadata. No file extensions are necessary.
        /// </remarks>
        internal static readonly string DefaultAssemblyList = "ElementServicesTest.dll,CoreTestsUntrusted.dll";

        /// <summary>
        /// </summary>
        public const string CoreTestsDefaultAssemblies = "coretestsdefaultassemblies";

        /// <summary>
        /// </summary>
        public const string AvalonDefaultAssemblies = "avalondefaultassemblies";
    }


}



