// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: eventually replace WaitFor() with something that waits until n frames have rendered,
   using ETW or some other events
 ********************************************************************/

using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    public static class RenderSynchronization
    {
        #region helper functions
        /// <summary>
        /// Eventually, wait for the number of rendered frames, but timeout if this doesnt happen within the specified time limit.
        /// </summary>
        /// <param name="renderedFrames">The number of frames to wait for</param>
        /// <param name="timeout">Maximum time in ms to wait for these rendered frames</param>
        /// <returns>whether the frames all rendered in this time</returns>
        static public bool WaitForRenderedFrames(WindowTest wt, int renderedFrames, int timeout)
        {       
            wt.WaitFor(timeout);
            return true;
        }        
        #endregion
    }
}

