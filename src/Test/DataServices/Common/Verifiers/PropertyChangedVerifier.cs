// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// Summary description for PropertyChangedVerifier.
    /// </summary>
    public class PropertyChangedVerifier : IVerifier
    {

        #region Private members

        private ArrayList _proplist = new ArrayList();

        #endregion

        #region Constructor

        public PropertyChangedVerifier(object o)
        {
            if (!(o is INotifyPropertyChanged))
            {
                throw new Exception(o.GetType().FullName + " does not implement INotifyPropertyChanged");
            }

            INotifyPropertyChanged ipc = o as INotifyPropertyChanged;

            ipc.PropertyChanged += new PropertyChangedEventHandler(ipc_PropertyChanged);
        }

        #endregion

        #region Private methods

        public void ipc_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (!_proplist.Contains(args.PropertyName))
                _proplist.Add(args.PropertyName);
        }

        #endregion

        #region IVerifier implementation

        public IVerifyResult Verify(params object[] ExpectedState)
        {
            if ((int)ExpectedState[0] == _proplist.Count)
            {
                return new VerifyResult(TestResult.Pass, _proplist.Count.ToString());
            }
            else
            {
                return new VerifyResult(TestResult.Fail, "Expected: " + ExpectedState[0].ToString() + "  Actual: " + _proplist.Count.ToString());
            }
        }

        #endregion

    }
}
