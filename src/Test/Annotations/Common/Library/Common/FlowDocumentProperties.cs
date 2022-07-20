// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Class for converting commandline property arguments into
//               objects and then applying them to a FlowDocument.

using System;
using System.Windows;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations
{
    public class FlowDocumentProperties
    {
        #region Methods

        public void ProcessArgs(string[] args)
        {
            if (args != null)
            {
                Match match;
                foreach (string arg in args)
                {
                    if ((match = _propertyExpression.Match(arg)).Success)
                    {
                        string propertyName = match.Groups[1].Value;
                        string propertyValue = match.Groups[2].Value;
                        AFlowDocumentProperty property = CreateProperty(propertyName, propertyValue);
                        _documentProperties.Add(property);
                    }
                }
            }
        }

        public void ApplyProperties(FlowDocument document)
        {
            foreach (AFlowDocumentProperty property in _documentProperties)
            {
                property.ApplyProperty(document);
            }
        }

        private AFlowDocumentProperty CreateProperty(string name, string value)
        {
            Type propertyType = typeof(FlowDocumentProperties).GetNestedType(name + "Property");
            if (propertyType == null)
                throw new ArgumentException("No matching class found for FlowDocument property with name '" + name + "'.");
            AFlowDocumentProperty property = (AFlowDocumentProperty)ReflectionHelper.GetInstance(propertyType, new Type[] { typeof(string) }, new object[] { value });
            return property;
        }

        #endregion

        #region Fields

        Regex _propertyExpression = new Regex(@"/property=\((.*)=(.*)\)");
        IList<AFlowDocumentProperty> _documentProperties = new List<AFlowDocumentProperty>();

        #endregion

        #region Property Definitions

        public abstract class AFlowDocumentProperty
        {
            public AFlowDocumentProperty(string value)
            {
                // Nothing.
            }

            abstract public void ApplyProperty(FlowDocument document);
        }

        public class FlowDirectionProperty : AFlowDocumentProperty
        {
            public FlowDirectionProperty(string value)
                : base(value)
            {
                _direction = (FlowDirection)Enum.Parse(typeof(FlowDirection), value);
            }

            public override void ApplyProperty(FlowDocument document)
            {
                document.FlowDirection = _direction;
            }

            FlowDirection _direction;
        }

        public class ColumnWidthProperty : AFlowDocumentProperty
        {
            public ColumnWidthProperty(string value)
                : base(value)
            {
                _columnWidth = double.Parse(value);
            }

            public override void ApplyProperty(FlowDocument document)
            {
                document.ColumnWidth = _columnWidth;
            }

            double _columnWidth;
        }

        public class TextAlignmentProperty : AFlowDocumentProperty
        {
            public TextAlignmentProperty(string value)
                : base(value)
            {
                _alignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), value);
            }

            public override void ApplyProperty(FlowDocument document)
            {
                document.TextAlignment = _alignment;
            }

            TextAlignment _alignment;
        }

        #endregion
    }
}
