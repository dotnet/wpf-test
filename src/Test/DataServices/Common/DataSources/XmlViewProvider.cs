// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using System.Xml;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// Provides a view over an XML data source.
    /// </summary>
    public class XmlViewProvider : DataSourceViewProvider
    {
        #region Private Data

        private PropertyDescriptorCollection _rawDataPropertyDescriptors;

        #endregion

        #region Constructors

        public XmlViewProvider(IEnumerable data)
            : base(data, true, true)
        {
            _rawDataPropertyDescriptors = TypeDescriptor.GetProperties(data);
        }

        #endregion

        #region Override Members

        public override PropertyDescriptorCollection ConvertToCanonical(object item)
        {
            // 

            throw new System.NotImplementedException("The method or operation is not implemented.");
        }

        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            throw new System.NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        private class XmlPropertyDescriptor : PropertyDescriptor
        {
            private Type _propertyType;

            public XmlPropertyDescriptor(string propertyName, Type propertyType)
                : base(propertyName, null)
            {
                this._propertyType = propertyType;
            }

            public override object GetValue(object component)
            {
                if (component == null)
                {
                    throw new ArgumentNullException("component");
                }

                XmlNode xmlNode = component as XmlNode;

                if (xmlNode == null)
                {
                    throw new ArgumentException("Object must be an XmlNode.", "component");
                }

                TypeConverter typeConverter = TypeDescriptor.GetConverter(_propertyType);
                return typeConverter.ConvertFromInvariantString(xmlNode[this.Name].InnerText);
            }

            public override bool CanResetValue(object component)
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }

            public override Type ComponentType
            {
                get { throw new NotImplementedException("The method or operation is not implemented."); }
            }

            public override bool IsReadOnly
            {
                get { throw new NotImplementedException("The method or operation is not implemented."); }
            }

            public override Type PropertyType
            {
                get { throw new NotImplementedException("The method or operation is not implemented."); }
            }

            public override void ResetValue(object component)
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }

            public override void SetValue(object component, object value)
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }

            public override bool ShouldSerializeValue(object component)
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }
    }
}
