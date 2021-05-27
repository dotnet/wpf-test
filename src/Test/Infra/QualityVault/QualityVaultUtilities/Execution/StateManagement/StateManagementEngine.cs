// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Microsoft.Test.Execution.StateManagement
{
    /// <summary>
    /// This is the API for Managing Test Execution State
    /// </summary>
    internal static class StateManagementEngine
    {
        static Stack<StateModule>[] StatePools = new Stack<StateModule>[4];

        static StateManagementEngine()
        {
            for (int i = 0; i <= (int)StatePool.Last; i++)
            {
                StatePools[i] = new Stack<StateModule>();
            }
        }

        //












        /// <summary>
        /// Sets the state on the specified pool.
        /// This method is only meant for consumption by the test infrastructure.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="stateModule"></param>
        internal static void PushStateOnPool(StatePool pool, StateModule stateModule)
        {
            PushStateImplementation(pool, stateModule);
        }        

        /// <summary>
        /// Rolls back the state of the specified pool.
        /// This method is only meant for consumption by the test infrastructure.
        /// </summary>
        /// <param name="pool"></param>           
        internal static void PopAllStatesFromPool(StatePool pool)
        {
            //Work back from most specific to the selected pool
            for (int i = (int)StatePool.Last; i >= (int)pool; i--)
            {
                PopAllStatesFromPoolImplementation(i);
            }

        }

        private static void PopAllStatesFromPoolImplementation(int poolId)
        {            
            int height = StatePools[poolId].Count;
            for (int i = 0; i < height; i++)
            {
                StateModule stateModule = StatePools[poolId].Pop();
                ExecutionEventLog.RecordStatus("Rolling Back State: " + stateModule); 
                stateModule.StateImplementation.RollbackState(stateModule);                    
            }
        }

        private static void PushStateImplementation(StatePool pool, StateModule stateModule)
        {
            ExecutionEventLog.RecordStatus("Recording Previous State: " + stateModule); 
            stateModule.StateImplementation.RecordPreviousState(stateModule);
            ExecutionEventLog.RecordStatus("Applying State: " + stateModule); 
            stateModule.StateImplementation.ApplyState(stateModule);
            StatePools[(int)pool].Push(stateModule);
        }
    }
}      
