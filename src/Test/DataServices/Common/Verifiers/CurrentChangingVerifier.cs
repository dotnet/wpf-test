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
    public class CurrentChangingVerifier : IVerifier
    {
        private int _counter;
        private object _sender;
        private CurrentChangingEventArgs _args;
        private object _current;
        private bool _cancelEvent;

        public bool CancelEvent
        {
            get { return _cancelEvent; }
            set { _cancelEvent = value; }
        }

        public CurrentChangingVerifier(ICollectionView currentitem)
        {
            _counter = 0;
            currentitem.CurrentChanging += new CurrentChangingEventHandler(onCurrentChanging);
        }

        public void onCurrentChanging(object sender, CurrentChangingEventArgs args)
        {
            this._counter++;
            this._sender = sender;
            this._args = args;
            this._current = ((ICollectionView)sender).CurrentItem;
            args.Cancel = _cancelEvent;
        }

		// params: view, counter, current
        public IVerifyResult Verify(params object[] expectedState)
        {
            object expectedSender = expectedState[0];
            int expectedCounter = (int)expectedState[1];
            object expectedCurrent = expectedState[2];

            if (_sender != expectedSender)
            {
                return new VerifyResult(TestResult.Fail, "Sender object isn't the same as expected");
            }

            if (_counter != expectedCounter)
            {
                return new VerifyResult(TestResult.Fail, "Fail - CurrentChanging Event was called an unexpected amount of times!  Actual: " + _counter + " - Expected: " + expectedCounter);
            }
            else
            {
                _counter = 0;
            }

			if (_current != expectedCurrent)
			{
                return new VerifyResult(TestResult.Fail, "Fail - Current Item doesn't match expected");
            }

            return new VerifyResult(TestResult.Pass, "CurrentChangingVerifier was successful");
        }

        public void RemoveEventHandler(ICollectionView currentitem)
        {
            currentitem.CurrentChanging -= new CurrentChangingEventHandler(onCurrentChanging);
        }

    }   
}
