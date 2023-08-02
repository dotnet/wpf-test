// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;



namespace Avalon.Test.CoreUI
{

    /// <summary>
    /// </summary>
    [Serializable]
    public class ApplicationDesription : IApplicationDescription, ICloneable
        {
            private string _ApplicationManifest;
            private string _ApplicationCodeBase;

    /// <summary>
    /// </summary>
            public ApplicationDesription(string manifest, string appCodeBase)
            {
                _ApplicationManifest = manifest;
                _ApplicationCodeBase = appCodeBase;}

    /// <summary>
    /// </summary>
            public string DeploymentManifestPath
            {
                get
                {
                    return null; //_DeploymentManifestPath;
                }
            }

    /// <summary>
    /// </summary>
            public string DeploymentManifest
            {
                get
                {
                    //CLR M2 hack - use app manifest as delpoy manifest
                    return _ApplicationManifest; //_DeploymentManifest;
                }
            }

    /// <summary>
    /// </summary>
            public string DeploymentCodeBase
            {
                get
                {
                    return _ApplicationCodeBase; //_DeploymentCodeBase;
                }
            }


    /// <summary>
    /// </summary>
            public string ApplicationManifestPath
            {
                get
                {
                    return null; //_ApplicationManifestPath;
                }
            }


    /// <summary>
    /// </summary>
            public string ApplicationManifest
            {
                get
                {
                    return _ApplicationManifest;
                }
            }


    /// <summary>
    /// </summary>
            public string ApplicationCodeBase
            {
                get
                {
                    return _ApplicationCodeBase;
                }
            }


    /// <summary>
    /// </summary>
            public object Clone()
            {
                // perform a deep copy
                return new ApplicationDesription(string.Copy(_ApplicationManifest), string.Copy(_ApplicationCodeBase));
            }
        }

}
