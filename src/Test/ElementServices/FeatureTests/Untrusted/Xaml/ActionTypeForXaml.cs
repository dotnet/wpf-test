// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Avalon.Test.Xaml.Markup
{
    /// <summary>
    /// 
    /// </summary>
    [TypeConverter(typeof(EnumConverter))]
    public enum ActionTypeForXaml
    {
        /// <summary>
        /// Compile as Avalon application using C# codegen.
        /// </summary>
        CompileCSharp,
        /// <summary>
        /// Compile as Avalon application using VB codegen.
        /// </summary>
        CompileVisualBasic,
        /// <summary>
        /// Do serialization round trip
        /// </summary>
        Serialization,
        /// <summary>
        /// Compile the XAML to BAML and test the BAML reader/writer API
        /// </summary>
        TestBamlReaderWriter,
        /// <summary>
        /// Stick an x:SynchronousMode="Async" attribute on root of XAML 
        /// and then load the XAML using XamlReader.Load()
        /// </summary>
        AsyncLoad,
        /// <summary>
        /// Stick an x:SynchronousMode="Async" attribute on root of XAML 
        /// and then load the XAML using XamlReader.Load()
        /// </summary>
        AsyncLoadXmlReader,
        /// <summary>
        /// Stick an x:SynchronousMode="Async" attribute on root of XAML 
        /// and then load the XAML using XamlReader.Load()
        /// </summary>
        AsyncLoadStream,
        /// <summary>
        /// </summary>
        Load

    };
}
