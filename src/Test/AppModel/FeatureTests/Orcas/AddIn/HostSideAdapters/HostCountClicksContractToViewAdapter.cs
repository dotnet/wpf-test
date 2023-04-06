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
    public class HostCountClicksContractToViewAdapter : HostCountClicksAddInView
    {

        private ICountClicksTestAddIn _contract;
        private ContractHandle _handle;

        public HostCountClicksContractToViewAdapter(ICountClicksTestAddIn addInContract)
        {
            _contract = addInContract;
            _handle = new ContractHandle(_contract);
        }

        public override int Clicks
        {
            get
            {
                return _contract.GetClicks();
            }
            set
            {
                _contract.SetClicks(value);
            }
        }

        /// <summary>
        /// Try returning the ContractToViewAdapter from null (should return null)
        /// </summary>
        public override object AdapterNull
        {
            get
            {
                return _contract.GetAdapterNull();
            }
        }

        /// <summary>
        /// Try returning the ContractToViewAdapter from a child (should throw InvalidOperationException)
        /// </summary>
        public override object AdapterChild
        {
            get
            {
                return _contract.GetAdapterChild();
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
