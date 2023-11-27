// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReflectTools.AutoPME {
    using System;

    //
    // This Attribute is used to mark AutoPME non-property scenarios which
    // should be run in ReflectBase pre-handle mode.  Pre-handle mode runs
    // the test scenarios before the form's handle has been created.
    // Properties tests are run by default.  If you want to run a method
    // test in pre-handle mode, put this attribute on the method.
    //
    [AttributeUsage(AttributeTargets.Method)]
    public class PreHandleScenarioAttribute : Attribute {
        private bool _isPreHandleScenario;

        public PreHandleScenarioAttribute(bool isPreHandleScenario) {
            this._isPreHandleScenario = isPreHandleScenario;
        }

        public bool IsPreHandleScenario {
            get { return _isPreHandleScenario; }
        }
    }
}
