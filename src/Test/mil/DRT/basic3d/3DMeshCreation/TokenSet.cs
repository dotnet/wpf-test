// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Tests
{
    //--------------------------------------------------------------

    // TokenTypes are bit-exclusive so that we can easily group them
    //  together in sets using a bit-vector
    public enum TokenType
    {
        None = 0x0,
        Constant = 0x1,
        Variable = 0x2,
        Plus = 0x4,
        Minus = 0x8,
        Multiply = 0x10,
        Divide = 0x20,
        Exponent = 0x40,
        Sine = 0x80,
        Cosine = 0x100,
        Tangent = 0x200,
        OpenParen = 0x400,
        CloseParen = 0x800,
        EOF = 0x1000,
    }

    //--------------------------------------------------------------

    public class TokenSet
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public TokenSet( TokenType type )
        {
            this.tokens = (uint)type;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public TokenSet( TokenSet t )
        {
            this.tokens = t.tokens;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private TokenSet( uint tokens )
        {
            this.tokens = tokens;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public static TokenSet operator +( TokenSet t1, TokenSet t2 )
        {
            return new TokenSet( (uint)t1.tokens | (uint)t2.tokens );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public static TokenSet operator +( TokenSet t1, TokenType t2 )
        {
            return new TokenSet( t1.tokens | (uint)t2 );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public bool Contains( TokenType type )
        {
            return ( tokens & (uint)type ) != 0;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private uint tokens;
    }
}
