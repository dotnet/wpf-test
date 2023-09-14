// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;
using System.Windows.Markup;

namespace Avalon.Test.CoreUI.Parser.Common
{
    /// <summary>
    /// TypeDescriptorContext class used for describing Types.
    /// It contains a ParserContext that others can retrieve via GetService.
    /// </summary>
    /// <remarks>
    /// Based on implementation of internal class TypeConvertContext.
    /// </remarks>
    public class TestTypeDescriptorContext: ITypeDescriptorContext
    {
        /// <summary>
        /// Construct a TestTypeDescriptorContext.
        /// </summary>
        public TestTypeDescriptorContext()
        {
        }

        /// <summary>
        /// Construct a TestTypeDescriptorContext based on a supplied parser context.
        /// </summary>
        /// <param name="parserContext">Valid parser context.</param>
        public TestTypeDescriptorContext(ParserContext parserContext)
            : this()
        {
            _parserContext = parserContext;
        }

        /// <summary>
        /// Construct a TestTypeDescriptorContext based on a supplied parser context and with a returnable instance.
        /// </summary>
        /// <param name="parserContext">Valid parser context.</param>
        /// <param name="initialInstance">An object to return when this object is queried for an instance.</param>
        public TestTypeDescriptorContext(ParserContext parserContext, object initialInstance): this(parserContext)
        {
            _instance = initialInstance;
        }


        /// <summary>
        /// ITypeDescriptorContext OnComponentChange
        /// </summary>
        /// <remarks>
        /// member is public only because base class has
        /// this public member declared
        /// </remarks>
        public void OnComponentChanged()
        {
        }

        /// <summary>
        /// ITypeDescriptorContext OnComponentChanging
        /// </summary>
        /// <remarks>
        /// member is public only because base class has
        /// this public member declared
        /// </remarks>
        /// <returns>
        /// false
        /// </returns>
        public bool OnComponentChanging()
        {
            return false;
        }

        /// <summary>
        /// IServiceProvider GetService implementation
        /// </summary>
        /// <param name="serviceType">
        /// Type of Service to be returned
        /// </param>
        /// <remarks>
        /// Currently returns the ParserContext itself or as a UriContext.
        /// </remarks>
        /// <returns>
        /// Service object, or null if service is not found
        /// </returns>
        virtual public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ParserContext))
            {
                return _parserContext;
            }
            else if (serviceType == typeof(IUriContext))
            {
                return _parserContext as IUriContext;
            }

            return null;
        }

        /// <summary>
        /// ITypeDescriptorContext Container property
        /// </summary> 
        /// <remarks>
        /// property is public only because base class has
        /// this public property declared
        /// </remarks>
        /// <returns>
        /// null
        /// </returns>
        public IContainer Container
        {
            get { return null; }
        }

        /// <summary>ITypeDescriptorContext Instance property</summary> 
        /// <remarks>
        /// property is public only because base class has
        /// this public property declared
        /// </remarks>
        /// <returns>
        /// Instance value (can be null).
        /// </returns>
        public object Instance
        {
            get { return _instance; }
        }
        private object _instance = null;

        /// <summary>ITypeDescriptorContext PropertyDescriptor</summary> 
        /// <remarks>
        /// property is public only because base class has
        /// this public property declared
        /// </remarks>
        /// <returns>
        /// null
        /// </returns>
        public PropertyDescriptor PropertyDescriptor
        {
            get { return null; }
        }

        /// <summary>
        /// Parser context object, for descendants who need one.
        /// </summary>
        protected ParserContext _parserContext;
    }
}
