// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Microsoft.Test.Execution.StateManagement.GacUtilities;
using Microsoft.Test.Execution.Logging;

namespace Microsoft.Test.Execution.StateManagement
{
    /// <summary>
    /// Implements GAC DLL registration service
    /// Requires StateModule.Path to be set to the location of dll within the TestBinariesDirectory
    /// </summary>
    internal class GacStateImplementation : IStateImplementation
    {
        #region IStateImplementation Members

        /// <summary>
        /// No-op - we simply remove the assembly afterwards
        /// </summary>
        /// <param name="settings"></param>
        public void RecordPreviousState(StateModule settings)
        {
            //



            //string name = new FileInfo(settings.Path).Name;
            //previousPath = AssemblyCache.QueryAssembly(name);
        }

        /// <summary>
        /// Installs specified dll to the GAC. Nothing more is promised.
        /// </summary>        
        public void ApplyState(StateModule settings)
        {
            LoggingMediator.LogEvent(string.Format("QualityVault: GAC: {0}", settings.Path));
            //if (previousPath != null)
            //{
            //    AssemblyCache.UninstallAssembly(new FileInfo(settings.Path).Name);
            //}
            //filePath = Path.Combine(settings.TestBinariesDirectory.FullName, settings.Path);
            //AssemblyCache.InstallAssembly(filePath, AssemblyCommitFlags.Refresh);
        }

        /// <summary>
        /// Removes specified dll from the GAC.
        /// </summary>
        /// <param name="settings"></param>
        public void RollbackState(StateModule settings)
        {
            LoggingMediator.LogEvent(string.Format("QualityVault: UN-GAC: {0}", settings.Path)); 
            //AssemblyCache.UninstallAssembly(filePath);
            //if (previousPath != null)
            //{
            //    AssemblyCache.InstallAssembly(previousPath, AssemblyCommitFlags.Refresh);
            //}
        }

        #endregion

        private string filePath;
        private string previousPath;
    }
}


