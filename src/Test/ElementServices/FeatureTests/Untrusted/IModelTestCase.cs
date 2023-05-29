// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Represents model-based CoreUI test cases.
 * Contributor: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
 * Filename:         $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/Core/common/CoreTestsTrusted/ModelTestCaseInfo.cs $
********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// </summary>
    public interface IModelTestCase
    {
        /// <summary>
        /// </summary>
        string XtcFileName
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        string ModelClassAssemblyName
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        string ModelClass
        {
            get;
            set;
        }
    }
}

