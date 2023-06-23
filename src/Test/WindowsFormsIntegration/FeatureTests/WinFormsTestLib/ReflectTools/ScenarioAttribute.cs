namespace ReflectTools {
    using System;

    //
    // This Attribute is used to denote a ReflectBase method which returns
    // a ScenarioResult but is not considered a scenario, e.g. a utility
    // method.  Define the Scenario(false) attribute above your method to
    // make ReflectBase ignore your method.
    //
    // Can also be used to give a scenario a description that will be
    // logged in the result log and picked up by MadDog.
    //
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioAttribute : Attribute {
        private bool isScenario;
        private string description;
		private int order = Int32.MaxValue;		// Default value is Int32.MaxValue--this means the scenario is NOT ordered.

		public ScenarioAttribute(bool isScenario) {
            this.isScenario = isScenario;
        }

		public ScenarioAttribute(int order)
		{
			this.isScenario = true;
			this.order = order;
		}

		public ScenarioAttribute(string description)
		{
			this.isScenario = true;
            this.description = description;
        }

		public ScenarioAttribute(string description, int order)
		{
			this.isScenario = true;
			this.description = description;
			this.order = order;
		}

		public bool IsScenario
		{
			get { return isScenario; }
        }

        public string Description {
            get { return description; }
        }

		// Default value is Int32.MaxValue--this means the scenario is NOT ordered.
		public int Order {
			get { return order; }
		}
    }
}
