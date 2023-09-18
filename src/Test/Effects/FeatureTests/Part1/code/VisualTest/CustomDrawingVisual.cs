// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Test.Effects
{
    public abstract class CustomDrawingVisual : DrawingVisual
    {
        internal abstract void Draw(System.Windows.Rect rect);

    }
}
