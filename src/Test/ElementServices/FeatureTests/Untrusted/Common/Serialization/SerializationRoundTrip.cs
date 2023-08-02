// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: A class used for serialization round trip.
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Collections.Generic;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Integration;
using Microsoft.Test.Markup;
using Avalon.Test.Xaml.Markup;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A class just for Serialization Round Trip
    /// </summary>
    public class SerializationRoundTrip : SerializationBaseCase
    {
        /// <summary>
        /// Entry point for serialization round trip.
        /// </summary>
        public void RunTestCase(ActionForXamlInfo info)
        {
            InitializeCase();

            XamlTestRunner.LoadAssemblies(info.SupportingAssemblies);
            string fileName = Helper.PreprocessSupportFile(info.XamlFile);
            DoTheTest(fileName);
        }

    }
}
