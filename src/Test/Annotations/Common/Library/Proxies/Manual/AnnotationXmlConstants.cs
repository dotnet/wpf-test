// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//        Generated: 6/13/2005 3:38:21 PM

// Required proxy imports.
using Annotations.Test.Reflection;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

// Delegate specific imports.
using System;


namespace Proxies.MS.Internal.Annotations
{
	public struct AnnotationXmlConstants 
	{
		public struct Namespaces
		{
			public const string CoreSchemaNamespace = "http://schemas.microsoft.com/windows/annotations/2003/11/core";
			public const string BaseSchemaNamespace = "http://schemas.microsoft.com/windows/annotations/2003/11/base";
		}

        public struct Prefixes
        {
            public const string XmlnsPrefix = "xmlns";
            public const string CoreSchemaPrefix = "anc";
            public const string BaseSchemaPrefix = "anb";
        }

        public struct Elements
        {
            public const string Annotation = "Annotation";
            public const string Resource = "Resource";
            public const string ContentLocator = "ContentLocator";
            public const string ContentLocatorGroup = "ContentLocatorGroup";
            public const string AuthorCollection = "Authors";
            public const string AnchorCollection = "Anchors";
            public const string CargoCollection = "Cargos";
            public const string Item = "Item";  // Individual name/value pair within a ContentLocatorPart

            public const string StringAuthor = "StringAuthor";
        }

        public struct Attributes
        {
            public const string Id = "Id";
            public const string CreationTime = "CreationTime";
            public const string LastModificationTime = "LastModificationTime";
            public const string TypeName = "Type";
            public const string ResourceName = "Name";
            public const string ItemName = "Name";
            public const string ItemValue = "Value";
        }
	}
}
