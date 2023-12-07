// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
