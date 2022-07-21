// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Ink;

namespace DRT
{

    /// <summary>
    /// Summary description for BaseTest.
    /// </summary>
    [TestedSecurityLevel(SecurityLevel.Default)]
    abstract public class DrtInkTestcase
    {
        abstract public void Run ();
        public bool Success = false;
        public System.Collections.Hashtable Options;
        public DrtBase DRT = null;
        protected StrokeCollection TestStrokes = new StrokeCollection();
    }
}
