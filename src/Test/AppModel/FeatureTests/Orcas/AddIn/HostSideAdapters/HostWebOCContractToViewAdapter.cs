// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.AddIn.Pipeline;
using System.AddIn.Contract;
using System.Windows;

namespace Microsoft.Test.AddIn
{

    [System.AddIn.Pipeline.HostAdapterAttribute()]
    public class HostWebOCContractToViewAdapter : HostWebOCView
    {

        private IWebOCTestAddIn _contract;
        private ContractHandle _handle;

        public HostWebOCContractToViewAdapter(IWebOCTestAddIn addInContract)
        {
            _contract = addInContract;
            _handle = new ContractHandle(_contract);
        }

        public override string Uri
        {
            get
            {
                return _contract.GetUri();
            }
            set
            {
                _contract.SetUri(value);
            }
        }

        public override FrameworkElement GetAddInUserInterface()
        {
            INativeHandleContract uiContract = (INativeHandleContract)_contract.QueryContract(typeof(INativeHandleContract).AssemblyQualifiedName);
            FrameworkElement element = FrameworkElementAdapters.ContractToViewAdapter(uiContract);

            return element;
        }

        public override void Initialize(string addInParameters)
        {
            _contract.Initialize(addInParameters);
        }

    }
}
