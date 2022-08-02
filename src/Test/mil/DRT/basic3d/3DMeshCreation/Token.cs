// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Tests
{
    //--------------------------------------------------------------

    public struct Token
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public Token( string value, TokenType type )
        {
            this.value = value;
            this.type = type;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public readonly string value;
        public readonly TokenType type;
    }
}
