// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;
using System.Xaml.Schema;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Reader Context
    /// </summary>
    /// <typeparam name="T">param of Type T</typeparam>
    public class ReaderContext<T> : XamlSchemaContext where T : XamlBaseFrame, new()
    {
        /// <summary>
        /// Stack of T
        /// </summary>
        private readonly Stack<T> _stack;

        /// <summary>
        /// Initializes a new instance of the ReaderContext class.
        /// </summary>
        public ReaderContext()
        {
            _stack = new Stack<T>();
        }

        /// <summary>
        /// Gets the schema context.
        /// </summary>
        /// <value>The schema context.</value>
        public XamlSchemaContext SchemaContext
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets the type of the current.
        /// </summary>
        /// <value>The type of the current.</value>
        public XamlType CurrentType
        {
            get
            {
                return _stack.Peek().CurrentType;
            }
        }

        /// <summary>
        /// Gets the current property.
        /// </summary>
        /// <value>The current property.</value>
        public XamlMember CurrentProperty
        {
            get
            {
                return _stack.Peek().CurrentProperty;
            }
        }

        /// <summary>
        /// Pushes the frame.
        /// </summary>
        /// <param name="type">The type value.</param>
        public void PushFrame(XamlType type)
        {
            T frame;
            if (_stack.Peek().CurrentType == null)
            {
                frame = _stack.Peek();
            }
            else
            {
                frame = new T();
                frame.Previous = _stack.Peek();
            }

            frame.CurrentType = type;
            _stack.Push(frame);
        }

        /// <summary>
        /// Pushes the frame.
        /// </summary>
        /// <param name="xamlNS">The xaml NS.</param>
        /// <param name="prefix">The prefix.</param>
        public void PushFrame(string xamlNS, string prefix)
        {
            T frame;
            if (_stack.Peek().CurrentType == null)
            {
                frame = _stack.Peek();
            }
            else
            {
                frame = new T();
                frame.Previous = _stack.Peek();
            }

            frame.LocalPrefixes.Add(prefix, xamlNS);
            _stack.Push(frame);
        }

        /// <summary>
        /// Pushes the frame.
        /// </summary>
        /// <param name="property">The property.</param>
        public void PushFrame(XamlMember property)
        {
            _stack.Peek().CurrentProperty = property;
        }

        /// <summary>
        /// Pops the frame.
        /// </summary>
        /// <returns>XamlBase Frame</returns>
        public XamlBaseFrame PopFrame()
        {
            return _stack.Pop();
        }

        #region XamlContext Members

        /// <summary>
        /// Finds the namespace by prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>string value</returns>
        public string FindNamespaceByPrefix(string prefix)
        {
            XamlBaseFrame frame = _stack.Peek();
            while (frame != null)
            {
                if (frame.LocalPrefixes.ContainsKey(prefix))
                {
                    return frame.LocalPrefixes[prefix];
                }
                else
                {
                    frame = frame.Previous;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the type of the xaml.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="name">The name .</param>
        /// <returns>XamlType value</returns>
        public XamlType FindXamlType(string prefix, string name)
        {
            string xamlNamespace = FindNamespaceByPrefix(prefix);
            XamlTypeName typeName = new XamlTypeName(xamlNamespace, name);
            return GetXamlType(typeName);
        }

        #endregion
    }
}
