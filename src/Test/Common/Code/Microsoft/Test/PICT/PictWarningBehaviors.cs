// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Xml.Serialization;
    [Flags]
    enum PictWarningBehaviors
    {
        Ignore = 0,
        FireWarningBehaviorEvent = 1,
        WriteToConsoleError = 2,
        ThrowException = 4,
        DefaultBehavior = WriteToConsoleError | FireWarningBehaviorEvent
    }
}
