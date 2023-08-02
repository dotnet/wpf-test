// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Layout {
    using System;
    using System.Windows;
    using Microsoft.Test.Logging;
    
    ///<Summary>internal implementation of RelayoutListener do not use directly; Use RelayoutTestCase instead</Summary>
    public class UIRelayoutListener: RelayoutListener {
        public UIRelayoutListener(UIElement target, ExpectedFlags flags):base(flags) 
        {
            if(target == null) {
                throw new ArgumentNullException("target");
            }
            
            _target = target;
            _target.LayoutUpdated += OnLayoutUpdated;
        }

        public override bool HasLayoutOccured { get { return _hasLayoutOccured;} }

        ///<Summary>Target element being listened to</Summary>
        public UIElement Target { get { return _target; } }
        
        public override void OnCancel() {
            _target.LayoutUpdated -= OnLayoutUpdated;
            _target = null;
        }
        
        void OnLayoutUpdated(object sender, EventArgs eventArgs) {
            if (sender == _target)
            {                
                _hasLayoutOccured = true;
            }            
        }
        
        bool _hasLayoutOccured;
        UIElement _target;
    }
}