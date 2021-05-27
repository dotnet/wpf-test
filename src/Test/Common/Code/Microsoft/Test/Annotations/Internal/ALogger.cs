// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//  Description: Abstract class that TestSuite uses to log test results.
//  Creator: derekme
//  Date Created: 9/8/05
//---------------------------------------------------------------------
using System;
using System.IO;

namespace Annotations.Test.Framework.Internal
{
    internal abstract class ALogger
    {
        abstract public void LogStatus(string msg);
        abstract public void LogComment(string msg);
        abstract public void LogToFile(string filename, Stream stream);
        abstract public void LogToFile(string filename);
        abstract public void LogResult(bool passed, string msg);
        abstract public void SetStage(VariationStage stage);
        abstract public void Close();
    }
}
