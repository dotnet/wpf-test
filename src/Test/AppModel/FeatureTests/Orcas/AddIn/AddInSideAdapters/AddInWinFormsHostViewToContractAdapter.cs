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
    public class AddInWinFormsHostViewToContractAdapter : ContractBase, IWinFormsHostTestAddIn
    {
        private AddInWinFormsHostView _view;
        private INativeHandleContract _contract;

        public AddInWinFormsHostViewToContractAdapter(AddInWinFormsHostView addInView) 
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

        #region IWinFormsHostTestAddIn Members

        public string GetTextBoxText()
        {
            return _view.GetTextBoxText();
        }

        public void Initialize(string addInParameters)
        {
            _view.Initialize(addInParameters);
        }

        internal AddInWinFormsHostView GetSourceView()
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
