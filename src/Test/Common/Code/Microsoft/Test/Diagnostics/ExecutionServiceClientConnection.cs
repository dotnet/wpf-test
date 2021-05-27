// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Collections;

namespace Microsoft.Test.Diagnostics
{
    internal class ExecutionServiceClientConnection : IDisposable
    {
        #region Private Data

        private string name;
        private string id;

        #endregion

        #region Constructor

        internal ExecutionServiceClientConnection(string name, string id)
        {
            this.name = name;
            this.id = id;
        }

        #endregion

        #region ClientConnection Members

        public string Name
        {
            get { return name; }
        }

        public string Id
        {
            get { return id; }
        }

        public void Dispose()
        {
            ExecutionServiceClient.Proxy.EndConnection(name, id);
        }

        public override string ToString()
        {
            //

            return ExecutionServiceClient.Proxy.GetConnectionToString(name);
        }

        #endregion

        #region Registry Members

        public void SetRegistryValue(string keyName, string valueName, object valueData)
        {
            ExecutionServiceClient.Proxy.ClientConnectionSetRegistryValue(name, id, WindowsIdentity.GetCurrent().User.Value, keyName, valueName, valueData);
        }

        #endregion

        #region Registration Members

        public void RegisterComServer(string filename, string clsid)
        {
            ExecutionServiceClient.Proxy.RegisterComServer(name, id, filename, clsid);
        }

        public void UnregisterComServer(string filename, string clsid)
        {
            ExecutionServiceClient.Proxy.UnregisterComServer(name, id, filename, clsid);
        }

        #endregion

        #region User Session Members

        public bool UserSessionConnect(int sessionId, string targetSessionName)
        {
            return ExecutionServiceClient.Proxy.UserSessionConnect(name, id, sessionId, targetSessionName);
        }

        public bool UserSessionDisconnect(int sessionId)
        {
            return ExecutionServiceClient.Proxy.UserSessionDisconnect(name, id, sessionId);
        }

        public bool UserSessionUnlockConsole()
        {
            return ExecutionServiceClient.Proxy.UserSessionUnlockConsole();
        }

        public int UserSessionGetConsoleSessionId()
        {
            return ExecutionServiceClient.Proxy.UserSessionGetConsoleSessionId();
        }

        #endregion

        #region Firewall Members

        public void DisableFirewallForExecutingApplication()
        {
            ExecutionServiceClient.Proxy.ClientConnectionSetFirewallExceptionState(name, id, FirewallHelper.GetCurrentProcessExePath(), (int)FirewallRule.Enabled);
        }

        public void EnableFirewallForExecutingApplication()
        {
            ExecutionServiceClient.Proxy.ClientConnectionSetFirewallExceptionState(name, id, FirewallHelper.GetCurrentProcessExePath(), (int)FirewallRule.None);
        }

        #endregion

    }
}
