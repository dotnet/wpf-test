//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;

using Avalon.Test.ComponentModel.Utilities;

#endregion

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Finding an object from a source
    /// </summary>
    public class Finding : InnerFinderBase
    {
        #region Constructor

        public Finding(string path)
        {
            _path = path;
        }

        public Finding()
            : this(string.Empty)
        {
        }

        #endregion

        #region Find

        /// <summary>
        /// find the object from the source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected override object Find(object source)
        {
            object target = source;
            if (!string.IsNullOrEmpty(NamePath))
            {
                string[] path = NamePath.Split('.');
                foreach (string node in path)
                {
                    target = VisualTreeUtils.FindPartByName(target as Visual, node);
                }
            }
            foreach (TypeNode node in TypePath)
            {
                target = VisualTreeUtils.FindPartByType(target as Visual, node.Type, node.Index);
            }

            if (!string.IsNullOrEmpty(this.Path))
            {
                target = ControlTestHelper.GetObjectByPath(target, this.Path, true);
            }

            return target;
        }

        #endregion

        #region Path

        /// <summary>
        /// property path of the object
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private string _path;

        #endregion

        #region TypePath

        private TypePath _typePath = new TypePath();

        /// <summary>
        /// type path of the object
        /// </summary>
        [TypeConverter(typeof(TypePathConverter))]
        public TypePath TypePath
        {
            get { return _typePath; }
            set { _typePath = value; }
        }

        #endregion

        #region NamePath

        /// <summary>
        /// property path of the object
        /// </summary>
        public string NamePath
        {
            get { return _namePath; }
            set { _namePath = value; }
        }

        private string _namePath;

        #endregion
    }

    /// <summary>
    /// the type path of an object
    /// </summary>
    [TypeConverter(typeof(TypePathConverter))]
    public class TypePath : List<TypeNode>
    {
    }

    /// <summary>
    /// the converter for TypePath
    /// </summary>
    public class TypePathConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (!(source is string))
            {
                throw new ArgumentException("source is not a string");
            }
            string sourceString = (string)source;
            string[] types = sourceString.Split('.');

            TypePath typePath = new TypePath();

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TypeNode));

            foreach (string nodeString in types)
            {
                typePath.Add((TypeNode)converter.ConvertFrom(typeDescriptorContext, cultureInfo, nodeString));
            }

            return typePath;
        }
    }

    /// <summary>
    /// type node description including the type and the index
    /// </summary>
    [TypeConverter(typeof(TypeNodeConverter))]
    public struct TypeNode
    {
        public TypeNode(Type type, int index)
        {
            _type = type;
            _index = index;
        }

        public Type Type
        {
            get { return _type; }
        }

        private Type _type;

        public int Index
        {
            get { return _index; }
        }

        private int _index;
    }

    /// <summary>
    /// converter for TypeNode
    /// </summary>
    public class TypeNodeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (!(source is string))
            {
                throw new ArgumentException("source is not a string");
            }
            string sourceString = (string)source;

            Match match = Regex.Match(sourceString, @"^(?<name>[^\[]+)(\[(?<index>\d+)\])?$");
            if (!match.Success)
                throw new ArgumentException("Invalid format");

            string typeName = match.Groups["name"].Value;
            string indexString = match.Groups["index"].Value;

            IXamlTypeResolver resolver = typeDescriptorContext.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
            if (resolver == null)
            {
                throw new InvalidOperationException("No IXamlTypeResolver service found");
            }
            Type type = resolver.Resolve(typeName);
            if (type == null)
            {
                throw new InvalidOperationException("Cannot resolve type " + typeName);
            }

            int index = 0;
            if (!String.IsNullOrEmpty(indexString))
                index = XmlHelper.Convert<int, string>(indexString);

            return new TypeNode(type, index);

        }
    }
}
