// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
//  Desc: Interface for external logging
//
using System;

namespace System.Windows.Test.CommonSource
{
    public interface ILog
    {
        void Log( string s );
    }
    
    public class LogToConsole : ILog
    {
        void ILog.Log( string s )
        {
            //Console.WriteLine( s );
        }
    }
}
