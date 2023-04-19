// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Method that the step will use to change the deployment.
    /// </summary>
    public enum InvalidationMethod
    {
        /// <summary>
        /// Change all version numbers for Avalon Assemblies to 7.0.5070
        /// </summary>
        ChangeVersion,
        /// <summary>
        /// Replace the deployment manifest with an empty (0 byte) file
        /// </summary>
        EmptyDeployManifest,
        /// <summary>
        /// Replace the application manifest with an empty (0 byte) file
        /// </summary>
        EmptyAppManifest,
        /// <summary>
        /// Open the deployment manifest's corresponding .exe, write random bytes several times.
        /// </summary>
        CorruptExe,
        /// <summary>
        /// Write random bytes into the deployment Manifest
        /// </summary>
        CorruptDeployManifest,
        /// <summary>
        ///  Write Random bytes into the Application manifest
        /// </summary>
        CorruptAppManifest
    }

  /// <summary>
  /// Used to corrupt Deployment Manifest files (.xbap and .application) to exercise 
  /// Deployment error UI.  
  /// </summary>
  public class InvalidateDeploymentStep : LoaderStep
  {
    #region Public members
      /// <summary>
      /// Full path to the Deployment manifest we'll be 
      /// </summary>
      public string DeploymentManifest = "";
      /// <summary>
      /// Method to be used to invalidate the deployment
      /// </summary>
      public InvalidationMethod Method;

    #endregion

    #region Step Implementation
    /// <summary>
    /// Modifies Deployment manifest based on "method" property 
    /// </summary>
    public override bool DoStep()
    {
        GlobalLog.LogDebug("Invalidate Deployment step starting for " + DeploymentManifest);
        string AppManifestPath = "";
        string ExePath = "";

        if (DeploymentManifest.EndsWith(ApplicationDeploymentHelper.BROWSER_APPLICATION_EXTENSION))
        {
            AppManifestPath = DeploymentManifest.Replace(ApplicationDeploymentHelper.BROWSER_APPLICATION_EXTENSION, ".exe.manifest");
            ExePath = DeploymentManifest.Replace(ApplicationDeploymentHelper.BROWSER_APPLICATION_EXTENSION, ".exe");
        }
        else if (DeploymentManifest.EndsWith(ApplicationDeploymentHelper.STANDALONE_APPLICATION_EXTENSION))
        {
            AppManifestPath = DeploymentManifest.Replace(ApplicationDeploymentHelper.STANDALONE_APPLICATION_EXTENSION, ".exe.manifest");
            ExePath = DeploymentManifest.Replace(ApplicationDeploymentHelper.STANDALONE_APPLICATION_EXTENSION, ".exe");
        }
        
        switch (this.Method)
        {
            case InvalidationMethod.CorruptExe:
                {
                    if (!File.Exists(ExePath))
                    {
                        GlobalLog.LogEvidence("Could not find " + ExePath + ", failing...");
                        return false;
                    }
                    CorruptFile(ExePath);
                    break;
                }
            case InvalidationMethod.ChangeVersion:
                {
                    // This has to live in ClientTestRuntime because it already works around Microsoft.Build.Tasks.dll not being available in the Razzle CLR.
                    GlobalLog.LogEvidence("Changing version of all Avalon Assemblies in " + DeploymentManifest + " to 7.0.5070");
                    ApplicationDeploymentHelper.ChangeAppVersion("7.0.5070.0", DeploymentManifest);
                }
                break;
            case InvalidationMethod.EmptyDeployManifest:
                {
                    EmptyFile(DeploymentManifest);
                    GlobalLog.LogEvidence("Overwrote " + DeploymentManifest + " with a 0-byte file");
                }
                break;
            case InvalidationMethod.EmptyAppManifest:
                {

                    EmptyFile(AppManifestPath);
                    GlobalLog.LogEvidence("Overwrote " + AppManifestPath + " with a 0-byte file");
                }
                break;
            case InvalidationMethod.CorruptDeployManifest:
                {
                    CorruptFile(DeploymentManifest);
                    GlobalLog.LogEvidence("Wrote random bytes to " + DeploymentManifest);
                }
                break;
            case InvalidationMethod.CorruptAppManifest:
                {
                    CorruptFile(AppManifestPath);
                    GlobalLog.LogEvidence("Wrote random bytes to " + AppManifestPath);
                }
                break;
        }
        return true;
    }

    
    #endregion

    #region Private Methods

    private static void CorruptFile(string filePath)
    {
    FileStream fs = null; ;
    try
    {
      byte[] randomStuff = new byte[50];
      fs = File.OpenWrite(filePath);
      Random r = new Random(DateTime.Now.Millisecond);
      for (int j = 0; j < 10; j++)
      {
          for (int i = 0; i < 20; i++)
          {
              fs.Seek(r.Next(1, (int)(fs.Length - randomStuff.Length)), SeekOrigin.Begin);
          }
          r.NextBytes(randomStuff);
          fs.Write(randomStuff, 0, randomStuff.Length);
      }
    }
    finally
    {
      if (fs != null)
      {
          fs.Flush();
          fs.Close();
      }
    }
    }

    private static bool isAvalonAssembly(string asmName)
    {
        return (asmName.ToLowerInvariant().Equals("presentationframework") ||
                asmName.ToLowerInvariant().Equals("uiautomationprovider") ||
                asmName.ToLowerInvariant().Equals("uiautomationtypes") ||
                asmName.ToLowerInvariant().Equals("windowsbase") ||
                asmName.ToLowerInvariant().Equals("milcore") ||
                asmName.ToLowerInvariant().Equals("presentationcore") ||
                asmName.ToLowerInvariant().Equals("presentationframework") ||
                asmName.ToLowerInvariant().Equals("presentationui") ||
                asmName.ToLowerInvariant().Equals("uiautomationclient") ||
                asmName.ToLowerInvariant().Equals("uiautomationclientsideproviders"));
    }

    private static void EmptyFile(string fileName)
    {
        // Don't have to do anything more complicated than this because it will overwrite any existing file.
        FileStream fs = File.Create(fileName);
        fs.Flush();
        fs.Close();
    }

    #endregion

  }

}
