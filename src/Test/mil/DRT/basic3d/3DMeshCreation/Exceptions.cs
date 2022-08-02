// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Tests
{
    //--------------------------------------------------------------

    public class FunctionParserException : Exception
    {
        public FunctionParserException( string msg ) : base( msg )
        {
        }
    }

    //--------------------------------------------------------------

    public class InvalidExpressionException : FunctionParserException
    {
        public InvalidExpressionException( string msg ) : base( msg )
        {
        }
    }

    //--------------------------------------------------------------

    public class OutOfTokensException : FunctionParserException
    {
        public OutOfTokensException( string msg ) : base( msg )
        {
        }
    }

    //--------------------------------------------------------------

    public class InvalidSyntaxException : FunctionParserException
    {
        public InvalidSyntaxException( string msg ) : base( msg )
        {
        }
    }

    //--------------------------------------------------------------

    public class UnexpectedBehaviorException : FunctionParserException
    {
        public UnexpectedBehaviorException( string msg ) : base( msg )
        {
        }
    }

    //--------------------------------------------------------------

    public class TrailingTokensException : FunctionParserException
    {
        public TrailingTokensException( string msg ) : base( msg )
        {
        }
    }

    //--------------------------------------------------------------

    public class UndefinedVariableException : FunctionParserException
    {
        public UndefinedVariableException( string msg ) : base( msg )
        {
        }
    }

    //--------------------------------------------------------------

    public class CannotDifferentiateException : FunctionParserException
    {
        public CannotDifferentiateException( string msg ) : base( msg )
        {
        }
    }
}
