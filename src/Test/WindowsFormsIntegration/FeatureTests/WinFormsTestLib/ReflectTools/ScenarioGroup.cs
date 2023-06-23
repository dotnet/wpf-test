using System;
using System.Reflection;

namespace ReflectTools
{
    /**
     * Encapsulates a group of scenarios. A ScenarioGroup has a name
     * and a MethodInfo[]
     */
    public class ScenarioGroup
    {
        /**
         * The name of the ScenarioGroup
         */
        public String Name;
        
        /**
         * The group of scenarios associated with this group
         */
        public MethodInfo[] Scenarios;

    }
}
