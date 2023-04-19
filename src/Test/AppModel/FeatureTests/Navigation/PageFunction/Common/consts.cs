// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*                                                                                   *
*  Description:                                                                     *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   * 
*************************************************************************************
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class PageFunctionTestApp 
    {
        
        internal const bool DEFAULT_REMOVE_FROM_JOURNAL = true;
        
        internal const int DEFAULT_CHILDPF_FRAME_INT_RETVAL = 7;
        
        internal const string VPFSAVECALLED = "SaveCalledOnObjPageFunction";
        internal const string VPFLOADCALLED = "LoadCalledOnObjPageFunction";
        
        internal const string SAPPSEQ = "SequenceOfLoadSave";        
        internal const string APPSEQ_VPFSAVE = "_SaveCalledOnObjPageFunction" ;
        internal const string APPSEQ_VPFLOAD = "_LoadCalledOnObjPageFunction" ;
        internal const string APPSEQ_VPFDEFCTOR = "_DefaultCtorObjPageFunction" ;
        internal const string APPSEQ_VPFCTOR = "_DateTimeCtorObjPageFunction" ;
        internal const string APPSEQ_VPFSTART = "_ObjPageFunctionStart" ;

    }
}
