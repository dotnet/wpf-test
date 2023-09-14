// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Microsoft.Test;
using Microsoft.Test.Discovery;


using System.Windows.Markup;

//using System.Windows.Data.

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A class to handle serialization and its verification for ICollection
    /// <para />
    /// </summary>
    /// <remarks>
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  SerializationICollection.cs
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    public class SerializationICollection : SerializationBaseCase
    {
        /// <summary>
        /// Override this method of Serialization by specify NodesShouldIgnoreChildrenSequence
        /// </summary>
        /// <param name="filename"></param>
        override protected void DoTheTest(String filename)
        {
            CoreLogger.BeginVariation();
            _serhelper.NodesShouldIgnoreChildrenSequence = new string[] { "CustomItem1.DictionaryProp", "CustomItem1.DictionaryDP1", "CustomItem1.DictionaryDP2" };
            _serhelper.RoundTripTestFile("ICollectionSerialization.xaml");
            CoreLogger.EndVariation();
        }
    }
}
