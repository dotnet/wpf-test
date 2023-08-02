// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Threading;
using System.Collections.Generic;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Base class helper for all the test case that target only
    /// WindowsBase.dll
    /// This class should not contain any references to any other dll
    /// on Avalon. Only WindowsBase.dll
    /// </summary>
    abstract public class TestCaseBase : TestCaseCommon
    {

    }

}
