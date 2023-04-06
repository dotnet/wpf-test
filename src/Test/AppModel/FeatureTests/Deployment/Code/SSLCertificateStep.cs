// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Deployment
{
  /// <summary>
  /// Used to add a SSL X509Certificate to the Local machine or User store.
  /// Be very careful adding certificates to the store!  
  /// </summary>
    public class SSLCertificateStep : LoaderStep
    {
        #region Public members
        /// <summary>
        /// Path to the certificate to use
        /// </summary>
        public string PathToCert = "";
        /// <summary>
        /// StoreName to use. Enum from System.Security.Cryptography.X509Certificates.
        /// Default: TrustedPublisher
        /// </summary>
        public StoreName StoreName = StoreName.TrustedPublisher;
        
        /// <summary>
        /// StoreLocation to use. (Local Machine or User).  
        /// Default: Local Machine
        /// </summary>
        public StoreLocation StoreLocation = StoreLocation.LocalMachine;
        #endregion

        #region Step Implementation
        /// <summary>
        /// Modifies Deployment manifest based on "method" property 
        /// </summary>
        public override bool DoStep()
        {
            GlobalLog.LogDebug("Adding certificate " + PathToCert + " to " + this.StoreName.ToString() + " store for " + this.StoreLocation.ToString());
            if (!File.Exists(this.PathToCert))
            {
                throw new FileNotFoundException("Could not find X509 Certificate file " + this.PathToCert);
            }
            X509Certificate2 certToBeAdded = new X509Certificate2(PathToCert);
            StoreHelper myStoreHelper = new StoreHelper(this.StoreName, this.StoreLocation);
            myStoreHelper.OpenStore(OpenFlags.ReadWrite);
            GlobalLog.LogDebug("Opened the certificate store...");
            myStoreHelper.AddCertificate(certToBeAdded);
            myStoreHelper.CloseStore();
            GlobalLog.LogDebug("Successfully added certificate to store...");
            return true;
        }


        #endregion


        private class StoreHelper
        {
            private X509Store _store;
            private class StoreCertPair
            {
                public X509Store store;
                public X509Certificate2 cert;

                public StoreCertPair(X509Store _store, X509Certificate2 _cert)
                {
                    store = _store;
                    cert = _cert;
                }
            }

            private static ArrayList s_listOfCertsAdded = new ArrayList();

            public StoreHelper(X509Store inputStore)
            {
                try
                {
                    _store = inputStore;
                }
                catch (Exception ex)
                {
                    GlobalLog.LogDebug(ex.ToString());
                    throw new Exception("Failed to create Certificate Store object");
                }
            }

            public StoreHelper(string storeName, StoreLocation storeLocation)
            {
                try
                {
                    if (storeName == null)
                    {
                        _store = new X509Store(StoreName.My, storeLocation);
                    }
                    else
                    {
                        _store = new X509Store(storeName, storeLocation);
                    }
                }
                catch (Exception ex)
                {
                    GlobalLog.LogDebug(ex.ToString());
                    throw new Exception("Failed to create Certificate Store object");
                }
            }

            public StoreHelper(StoreName storeName, StoreLocation storeLocation)
            {
                try
                {
                    _store = new X509Store(storeName, storeLocation);
                }
                catch (Exception ex)
                {
                    GlobalLog.LogDebug(ex.ToString());
                    throw new Exception("Failed to create Certificate Store object");
                }
            }

            // Open a store 
            public void OpenStore()
            {
                try
                {
                    _store.Open(OpenFlags.OpenExistingOnly | OpenFlags.MaxAllowed);
                }
                catch (Exception ex)
                {
                    GlobalLog.LogDebug(ex.ToString());
                    throw new Exception("Failed to open Certificate Store");
                }
            }

            public void OpenStore(OpenFlags flags)
            {
                try
                {
                    _store.Open(flags);
                }
                catch (Exception ex)
                {
                    GlobalLog.LogDebug(ex.ToString());
                    throw new Exception("Failed to open Certificate Store");
                }
            }

            public void CloseStore()
            {
                _store.Close();
            }

            public void AddCertificate(X509Certificate2 cert)
            {
                try
                {
                    if (Exists(cert))
                        return;

                    try
                    {
                        _store.Add(cert);
                        s_listOfCertsAdded.Add(new StoreCertPair(_store, cert));
                    }
                    catch (Exception ex)
                    {
                        GlobalLog.LogDebug(ex.ToString());
                        throw;
                    }
                    GlobalLog.LogDebug("Store.Add Returned");
                }
                catch (Exception ex)
                {
                    GlobalLog.LogDebug("Failed to Add Certificate to Store");
                    GlobalLog.LogDebug(ex.ToString());
                }
            }

            public void RemoveCertificate(X509Certificate2 cert)
            {
                try
                {
                    if (Exists(cert))
                    {
                        this._store.Remove(cert);
                        s_listOfCertsAdded.Remove(new StoreCertPair(_store, cert));
                        GlobalLog.LogDebug("Store.Remove Returned");
                    }
                }
                catch (Exception ex)
                {
                    GlobalLog.LogDebug("Failed to Add Certificate to Store");
                    GlobalLog.LogDebug(ex.ToString());
                    throw;  // Throw the exception ret by the store
                }
            }

            public static void CleanUpAllCertsAdded()
            {
                GlobalLog.LogDebug("In CleanUpAllCertsAdded()");

                foreach (StoreCertPair storeCertPair in s_listOfCertsAdded)
                {
                    GlobalLog.LogDebug("Cert - " + storeCertPair.cert.IssuerName);
                    GlobalLog.LogDebug("Cert - " + storeCertPair.store);
                    new StoreHelper(storeCertPair.store).RemoveCertificate(storeCertPair.cert);
                }
                s_listOfCertsAdded = new ArrayList();
            }

            public bool Exists(X509Certificate2 cert)
            {
                bool exists = false;
                try
                {
                    exists = this._store.Certificates.Contains(cert);
                }
                catch (Exception ex)
                {
                    GlobalLog.LogDebug(ex.ToString());
                    throw;
                }
                return exists;
            }
        }
    }
}
