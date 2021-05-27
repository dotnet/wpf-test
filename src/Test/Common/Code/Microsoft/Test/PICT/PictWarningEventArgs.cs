// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Xml.Serialization;

    sealed class PictWarningEventArgs : EventArgs
    {
        readonly string warning;

        public string Warning { get { return this.warning; } }

        public PictWarningEventArgs(string warning)
        {
            this.warning = warning;
        }
    }
}
