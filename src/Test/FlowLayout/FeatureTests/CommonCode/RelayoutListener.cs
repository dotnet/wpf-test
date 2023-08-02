// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Layout {
    using System;
    using System.Windows;
    using Microsoft.Test.Logging;

    ///<Summary>Enum used to determine whether an action was expected or not expected</Summary>
    public enum ExpectedFlags {
        Expected,
        NotExpected
    }
        
    public abstract class RelayoutListener : IDisposable {
        ///<Summary>Constructor</Summary>
        ///<Parameter Name="flag">Enum describing whether layout was expected or not</Parameter>
        protected RelayoutListener(ExpectedFlags flag) {
            _canceled = false;
            switch (flag)
            {
                case ExpectedFlags.Expected:
                    {
                        _layoutExpected = true; break;
                    }
                case ExpectedFlags.NotExpected:
                    {
                        _layoutExpected = false; break;
                    }
                default:
                    {
                        throw new System.ComponentModel.InvalidEnumArgumentException("flag", (int)flag, typeof(ExpectedFlags));
                    }
            }
        }
        
        ///<returns>True is layout is expected before LogResult is called</returns>
        public bool IsLayoutExpected { get { return _layoutExpected; } }
        
        ///<returns>True if layout has occured since this object was created</returns>
        public abstract bool HasLayoutOccured { get; }
        
        ///<Summary>Gets the result of the test (pass / fail)</Summary>
        ///<Returns>
        ///     LogResult.Ignore if the test was canceled
        ///     LogResult.Pass if the test was not canceled and
        ///             Layout has occured as expected; Logs Pass
        ///             Layout has not occured as expected; Logs Pass
        ///     LogResult.Fail if 
        ///             Layout is expected but has not occured; Logs Fail
        ///             Layout has occured but is not expected; Logs Fail
        ///</Returns>
        public virtual TestResult GetResult() {            
            if(IsCanceled == true) {
                return TestResult.Ignore;
            }

            TestResult result;
            if(IsLayoutExpected == HasLayoutOccured) {
                string message = IsLayoutExpected ? "Layout was expected and occured" : "Layout was not expected and did not occur";
                GlobalLog.LogDebug(message);
                result = TestResult.Pass;
            }
            else {
                string message = IsLayoutExpected ? "Layout was expected but did not occur" : "Layout was not expected but occured";
                GlobalLog.LogStatus(message);
                result = TestResult.Fail;
            }
            return result;
        }
        
        public virtual void Cancel() {
            if(!IsCanceled) {
                OnCancel();
                _canceled = true;
            }
        }
        
        public bool IsCanceled { get { return _canceled; } }
        
        public virtual void OnCancel() { }
        
        void IDisposable.Dispose() { 
            Cancel();
        }
        
        bool _layoutExpected;
        bool _canceled;
    }
}