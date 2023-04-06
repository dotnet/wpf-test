// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.AddIn.Pipeline;
using System.AddIn.Contract;

namespace Microsoft.Test.AddIn
{
    [AddInAdapter]
    public class AddInSequenceFocusViewToContractAdapter : ContractBase, ISequenceFocusTestAddIn
    {
        private AddInSequenceFocusView _view;
        private INativeHandleContract _contract;

        public AddInSequenceFocusViewToContractAdapter(AddInSequenceFocusView addInView) 
        {
            _view = addInView;
            _contract = FrameworkElementAdapters.ViewToContractAdapter(_view.GetAddInUserInterface());   
        }

        public override IContract QueryContract(string contractIdentifier)
        {           
            if (contractIdentifier.Equals(typeof(INativeHandleContract).AssemblyQualifiedName))
            {
                return _contract;
            }
            else
            {
                return base.QueryContract(contractIdentifier);
            }
        }

        #region ISequenceFocusTestAddIn Members

        //public Dictionary<string, object>[] GetFocusSequence()
        //{
        //    FocusItem[] items = view.GetFocusSequence();
        //    if (items == null)
        //    {
        //        return null;
        //    }
        //    Dictionary<string, object>[] dictionaries = new Dictionary<string, object>[items.Length];
        //    for (int i = 0; i < dictionaries.Length; i++)
        //    {
        //        dictionaries[i] = FocusItem.ConvertToDictionary(items[i]);
        //    }
        //    return dictionaries;
        //}

        public FocusItem[] GetFocusSequence()
        {
            return _view.GetFocusSequence();
        }

        public void ClearSequence()
        {
            _view.ClearSequence();
        }

        public void Initialize(string addInParameters)
        {
            _view.Initialize(addInParameters);
        }

        internal AddInSequenceFocusView GetSourceView()
        {
            return _view;
        }

        #endregion

        #region INativeHandleContract Members

        public IntPtr GetHandle()
        {
            return _contract.GetHandle();
        }

        #endregion
    }
}
