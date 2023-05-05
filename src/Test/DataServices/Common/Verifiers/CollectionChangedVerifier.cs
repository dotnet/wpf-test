// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    public class CollectionChangedVerifier : IVerifier
    {
        private int _count;
        private object _sender;
        private NotifyCollectionChangedEventArgs _args;

        public CollectionChangedVerifier(INotifyCollectionChanged collection)
        {
            _count = 0;
            collection.CollectionChanged +=new NotifyCollectionChangedEventHandler(onCollectionChanged);
        }
        public CollectionChangedVerifier(ICollectionView collection)
        {
            _count = 0;
            collection.CollectionChanged +=new NotifyCollectionChangedEventHandler(onCollectionChanged);
        }

        void onCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            _count++;
            this._sender=sender;
            this._args=args;
        }

        //params: object sender, NotifyCollectionChangedEventArgs args, int count
        public IVerifyResult Verify(params object[] expectedState)
        {
            object expectedSender = expectedState[0];
            NotifyCollectionChangedEventArgs expectedArgs = expectedState[1] as NotifyCollectionChangedEventArgs;
            int expectedCount = (int)expectedState[2];

            if (_sender != expectedSender)
            {
                return new VerifyResult(TestResult.Fail, "Sender was not the same");
            }
            if (_args.Action != expectedArgs.Action)
            {
                return new VerifyResult(TestResult.Fail, "Expected args.Action '" + expectedArgs.Action + "' doesn't match Actual args.Action '" + _args.Action + "'!");
            }
            if (_args.Action != NotifyCollectionChangedAction.Reset)
            {
                object expectedItem = null;
                object actualItem = null;
                if (_args.Action == NotifyCollectionChangedAction.Add)
                {
                    expectedItem = expectedArgs.NewItems[0];
                    actualItem = _args.NewItems[0];
                }
                else if (_args.Action == NotifyCollectionChangedAction.Remove)
                {
                    expectedItem = expectedArgs.OldItems[0];
                    actualItem = _args.OldItems[0];
                }
                
                if (actualItem != expectedItem)
                {
                    return new VerifyResult(TestResult.Fail, "Expected item '" + expectedItem + "' doesn't match Actual item '" + actualItem + "'!");
                }
            }
            if (_count != expectedCount)
            {
                return new VerifyResult(TestResult.Fail, "CurrentChanging Event was called an unexpected amount of times!  Actual: " + _count + " - Expected: " + expectedCount);
            }
            else
            {
                _count = 0;
            }

            return new VerifyResult(TestResult.Pass, "Verify was successful");
        }
    }
}


