// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.AddIn.Pipeline;
using System.AddIn.Contract;

namespace Microsoft.Test.AddIn
{
    [AddInAdapter]
    public class AddInWebOCContractToViewAdapter : ContractBase, IWebOCTestAddIn
    {

        private AddInWebOCView _view;

        private INativeHandleContract _contract;

        public AddInWebOCContractToViewAdapter(AddInWebOCView addInView) 
        {
            _view = addInView;
            _contract = FrameworkElementAdapters.ViewToContractAdapter(_view.GetAddInUserInterface());   
        }

        public override System.AddIn.Contract.IContract QueryContract(string contractIdentifier)
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

        #region IWebOCTestAddIn Members

        public string GetUri()
        {
            return _view.Uri;
        }

        public void SetUri(string Uri)
        {
            _view.Uri = Uri;
        }

        public string Uri
        {
            get
            {
                return _view.Uri;
            }
            set
            {
                _view.Uri = value;
            }
        }


        #endregion

        #region ITestAddIn Members

        public void Initialize(string addInParameters)
        {
            _view.Initialize(addInParameters);
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
