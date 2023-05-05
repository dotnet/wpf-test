// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;


namespace MS.Internal
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("35355DA7-3EEA-452e-89F3-68344278F806")]
    internal interface IPersistFileCheckSum
    {
        void CalculateCheckSum( [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidCheckSumAlgorithm,
                                [In, MarshalAs(UnmanagedType.U4)]       int cbBufferSize,
                                [Out, MarshalAs(UnmanagedType.LPArray,
                                                     SizeParamIndex=1)] byte[] Hash,
                                [Out, MarshalAs(UnmanagedType.U4)]      out int ActualSize);
    }
}
