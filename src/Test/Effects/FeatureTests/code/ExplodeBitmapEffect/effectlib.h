// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for effectlib.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__


#ifndef __effectlib_h__
#define __effectlib_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __Explode_FWD_DEFINED__
#define __Explode_FWD_DEFINED__

#ifdef __cplusplus
typedef class Explode Explode;
#else
typedef struct Explode Explode;
#endif /* __cplusplus */

#endif 	/* __Explode_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "mileffects.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_effectlib_0000_0000 */
/* [local] */ 




extern RPC_IF_HANDLE __MIDL_itf_effectlib_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_effectlib_0000_0000_v0_0_s_ifspec;


#ifndef __effectlib_LIBRARY_DEFINED__
#define __effectlib_LIBRARY_DEFINED__

/* library effectlib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_effectlib;

EXTERN_C const CLSID CLSID_Explode;

#ifdef __cplusplus

class DECLSPEC_UUID("C9308B90-2011-47E6-BC3E-872B1AEF7258")
Explode;
#endif
#endif /* __effectlib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


