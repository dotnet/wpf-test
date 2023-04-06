// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*      This is a class that allows you to quickly debug by supplying command line   *
*      parameters                                                                   *
*                                                                                   *
*  Description:                                                                     *
*      Quick Entry point that accepts command line parameters                       *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   * 
*************************************************************************************
*/

using System;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class SelfRunner {
        //public static void Main(string [] args) {
        //    LaunchPageFunctions.Run(args);        
        //}

        public void Run(string[] args)
        {
            LaunchPageFunctions.Run(args);
        }
    }
}
