// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.AddIn.Pipeline;
using System.AddIn.Contract;
using System.Windows;

namespace Microsoft.Test.AddIn
{
    [HostAdapterAttribute]
    public class HostDragDropContractToViewAdapter : HostDragDropView
    {

        private IDragDropTestAddIn _contract;
        private ContractHandle _handle;

        public HostDragDropContractToViewAdapter(IDragDropTestAddIn addInContract)
        {
            _contract = addInContract;
            _handle = new ContractHandle(_contract);
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

        public override string GetTextBoxText()
        {
            return _contract.GetTextBoxText();
        }
    }
}
