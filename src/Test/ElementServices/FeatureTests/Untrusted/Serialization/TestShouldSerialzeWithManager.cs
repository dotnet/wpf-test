// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//   Validate parameters for some function in Parser
//   Author Microsoft

//
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Windows.Markup;
using System.Collections;
using System.Xml;
using Avalon.Test.CoreUI.Common;
using System.Windows;
using System.IO.Packaging;
using Microsoft.Test.Serialization;
using Microsoft.Test;
using Microsoft.Test.Discovery;


namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    ///    Validate parameters for SerializationHelper.SerializeObjectTree and LoadXml
    /// </summary>
    public class TestShouldSerialzeWithManager : SerializationBaseCase
    {


        /// <summary>
        /// Called by SerializationBaseCase as entry point
        /// </summary>
        /// <param name="fileName"></param>
        protected override void DoTheTest(String fileName)
        {
            fileName="UIElementWithShouldSerialize.xaml";
            object root = SerializationHelper.ParseXamlFile(fileName);
            string serialized = SerializationHelper.SerializeObjectTree(root);
            
            CoreLogger.LogStatus(serialized);
            VerifyXaml(serialized);
        }
        void VerifyXaml(string xaml)
        {
            //verify Hidden property didn't get serialized.
            if (-1 != xaml.IndexOf("Hidden"))
            {
                CoreLogger.LogStatus(xaml);
                throw new Microsoft.Test.TestValidationException("Should not serialize Hidden properties.");
            }
            //Verify properties should not be serialized have not been.
            if (-1 != xaml.IndexOf("ShouldNotBeSerialized"))
            {
                CoreLogger.LogStatus(xaml);
                throw new Microsoft.Test.TestValidationException("Should not serialize any property whose value is ShouldNotBeSerialized.");
            }
            for (int i = 0; i < 6; i++)
            {
                int index = xaml.IndexOf("ShouldBeSerialized");
                if (-1 == index)
                {
                    throw new Microsoft.Test.TestValidationException("There are only " + i.ToString() + " Property with value of ShouldBeSerialized, should 6.");
                }
                xaml = xaml.Substring(index + 10);
            }
            if (-1 != xaml.IndexOf("ShouldBeSerialized"))

            {
                CoreLogger.LogStatus(xaml);
                throw new Microsoft.Test.TestValidationException("Should not serialize properties with value ShouldNotBeSerialized more than 6 times.");
            }
        }

    }
}
