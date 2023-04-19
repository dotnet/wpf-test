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
    public class HostSequenceFocusContractToViewAdapter : HostSequenceFocusView
    {

        private ISequenceFocusTestAddIn _contract;
        private ContractHandle _handle;

        public HostSequenceFocusContractToViewAdapter(ISequenceFocusTestAddIn addInContract)
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

        public override FocusItem[] GetFocusSequence()
        {
            return _contract.GetFocusSequence();
            //Dictionary<string, object>[] dictionaries = contract.GetFocusSequence();
            //if (dictionaries == null)
            //{
            //    return null;
            //}
            //FocusItem[] items = new FocusItem[dictionaries.Length];
            //for (int i = 0; i < items.Length; i++)
            //{
            //    items[i] = FocusItem.ConvertFromDictionary(dictionaries[i]);
            //}
            //return items;
        }

        public override void ClearSequence()
        {
            _contract.ClearSequence();
        }

    }
}
