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
    public class AddInCountClicksContractToViewAdapter : ContractBase, ICountClicksTestAddIn
    {

        private AddInCountClicksView _view;

        private INativeHandleContract _contract;

        public AddInCountClicksContractToViewAdapter(AddInCountClicksView addInView) 
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

        #region ICountClicksTestAddIn Members

        public int GetClicks()
        {
            return _view.Clicks;
        }

        public void SetClicks(int clickCount)
        {
            _view.Clicks = clickCount;
        }

        public int Clicks
        {
            get
            {
                return _view.Clicks;
            }
            set
            {
                _view.Clicks = value;
            }
        }

        /// <summary>
        /// Try returning the ContractToViewAdapter from null (should return null)
        /// </summary>
        public object GetAdapterNull()
        {
            return FrameworkElementAdapters.ViewToContractAdapter(null);   
        }

        /// <summary>
        /// Try returning the ContractToViewAdapter from a child (should throw InvalidOperationException)
        /// </summary>
        public object GetAdapterChild()
        {
            return FrameworkElementAdapters.ViewToContractAdapter(_view.GetAddInUserInterfaceChild());
        }      

        internal AddInCountClicksView GetSourceView()
        {
            return _view;
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
