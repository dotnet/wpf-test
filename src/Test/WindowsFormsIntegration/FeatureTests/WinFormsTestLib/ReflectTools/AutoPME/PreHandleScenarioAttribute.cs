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
        private bool isPreHandleScenario;

        public PreHandleScenarioAttribute(bool isPreHandleScenario) {
            this.isPreHandleScenario = isPreHandleScenario;
        }

        public bool IsPreHandleScenario {
            get { return isPreHandleScenario; }
        }
    }
}
