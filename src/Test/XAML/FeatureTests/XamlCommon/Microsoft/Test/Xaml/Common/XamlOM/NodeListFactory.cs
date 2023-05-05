// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    /// <summary>
    /// Factory that creates baked set of commonly used NodeLists
    /// </summary>
    public static class NodeListFactory
    {
        /// <summary>
        /// Create a x2:FactoryMethod member
        /// </summary>
        /// <param name="value">string value to add</param>
        /// <param name="writeAsAttribute">should write as attribute</param>
        /// <returns>Node list describing a factory method</returns>
        public static NodeList CreateFactoryMethod(string value, bool writeAsAttribute)
        {
            return new NodeList()
            {
                new StartMember(XamlLanguage.FactoryMethod)
                { 
                    TestMetadata = 
                    {
                        { NodeListXmlWriter.WriteAsAttributeProperty, writeAsAttribute }
                    } 
                },
                    new ValueNode(value),
                new EndMember(),
            };
        }

        /// <summary>
        ///  Create an x:Name member
        /// </summary>
        /// <param name="name">name of the member</param>
        /// <param name="writeAsAttribute">should write as attribute</param>
        /// <returns>Nodelist describing an x:Name</returns>
        public static NodeList CreateXName(string name, bool writeAsAttribute)
        {
            return new NodeList()
            {
                new StartMember(XamlLanguage.Name)
                { 
                    TestMetadata = 
                    {
                        { NodeListXmlWriter.WriteAsAttributeProperty, writeAsAttribute }
                    } 
                },
                    new ValueNode(name),
                new EndMember(),
            };
        }

        /// <summary>
        /// Create an x2:Reference member
        /// </summary>
        /// <param name="refName">reference name</param>
        /// <param name="writeAsAttribute">should write as attribute</param>
        /// <returns>node list describing an x2:reference</returns>
        public static NodeList CreateReference(string refName, bool writeAsAttribute)
        {
            return new NodeList()
            {
                new StartObject(XamlLanguage.Reference),
                    new StartMember(XamlLanguage.Reference.GetMember("Name"))
                    { 
                        TestMetadata = 
                        {
                            { NodeListXmlWriter.WriteAsAttributeProperty, writeAsAttribute }
                        } 
                    },
                        new ValueNode(refName),
                    new EndMember(),
                new EndObject(),
            };
        }

        /// <summary>
        /// Create a member
        /// </summary>
        /// <param name="type">type of object</param>
        /// <param name="property">property name</param>
        /// <param name="value">value node to write</param>
        /// <param name="writeAsAttribute">should write as attribute</param>
        /// <returns>Node list descriging a member</returns>
        public static NodeList CreateMember(Type type, string property, object value, bool writeAsAttribute)
        {
            return new NodeList()
            {
                new StartMember(type, property)
                { 
                    TestMetadata = 
                    {
                        { NodeListXmlWriter.WriteAsAttributeProperty, writeAsAttribute }
                    } 
                },
                    new ValueNode(value),
                new EndMember(),
            };
        }
    }
}
