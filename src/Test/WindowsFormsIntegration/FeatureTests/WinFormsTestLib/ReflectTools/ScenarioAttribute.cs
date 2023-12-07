// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        private bool _isScenario;
        private string _description;
		private int _order = Int32.MaxValue;		// Default value is Int32.MaxValue--this means the scenario is NOT ordered.

		public ScenarioAttribute(bool isScenario) {
            this._isScenario = isScenario;
        }

		public ScenarioAttribute(int order)
		{
			this._isScenario = true;
			this._order = order;
		}

		public ScenarioAttribute(string description)
		{
			this._isScenario = true;
            this._description = description;
        }

		public ScenarioAttribute(string description, int order)
		{
			this._isScenario = true;
			this._description = description;
			this._order = order;
		}

		public bool IsScenario
		{
			get { return _isScenario; }
        }

        public string Description {
            get { return _description; }
        }

		// Default value is Int32.MaxValue--this means the scenario is NOT ordered.
		public int Order {
			get { return _order; }
		}
    }
}
