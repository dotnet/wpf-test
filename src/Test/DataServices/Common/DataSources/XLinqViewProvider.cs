// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// Provides a view over an XLinq data source.
    /// </summary>
    public class XLinqViewProvider : DataSourceViewProvider
    {
        #region Private Data

        private PropertyDescriptorCollection _rawDataPropertyDescriptors;
        private XElement _underlyingXElement;
        private CollectionPropertyHolder _collectionPropertyHolder;

        #endregion

        #region Constructors

        public XLinqViewProvider(IEnumerable data)
            : base(data, true, true)
        {
        }

        #endregion

        #region Override Members

        protected override void AddCore(object item)
        {
            PropertyDescriptor[] propertyDescriptors = GetByValueComparableProperties(item);

            XElement dataItem = new XElement(item.GetType().Name);
            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptors)
            {
                dataItem.SetAttributeValue(propertyDescriptor.Name, propertyDescriptor.GetValue(item));
            }

            _underlyingXElement.Add(dataItem);
        }

        /// <summary>
        /// Remove can take an item either in native (XElement) or canonical
        /// form. It searches through our underlying XElement's nodes using
        /// the appropriate technique and removes the first instance it sees.
        /// </summary>
        /// <param name="item">Data item to remove.</param>
        protected override void RemoveCore(object item)
        {
            foreach (XElement xe in _underlyingXElement.Nodes())
            {
                if (item.GetType() == typeof(XElement))
                {
                    if (xe.Equals(item))
                    {
                        xe.Remove();
                        return;
                    }
                }
                else
                {
                    if (CompareByValue(item, xe))
                    {
                        xe.Remove();
                        return;
                    }
                }
            }
        }

        protected override void SortCore(SortDescription sortDescription)
        {
            sortDescription.PropertyName = "Attribute[" + sortDescription.PropertyName + "].Value";
            base.SortCore(sortDescription);
        }

        /// <summary>
        /// Canonical form for retrieving property values on a data item type
        /// specific to a particular DataSourceViewProvider is via
        /// PropertyDescriptors. The native form is storing the properties as
        /// attributes, so we use the XLinqPropertyDescriptor which does that
        /// lookup.
        /// </summary>
        /// <param name="item">XElement native data form.</param>
        /// <returns>PropertyDescriptor canonical data form.</returns>
        public override PropertyDescriptorCollection ConvertToCanonical(object item)
        {
            PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);

            foreach (XAttribute attribute in ((XElement)item).Attributes())
            {
                propertyDescriptorCollection.Add(new XLinqPropertyDescriptor(attribute.Name.LocalName, _rawDataPropertyDescriptors[attribute.Name.LocalName].PropertyType));
            }

            return propertyDescriptorCollection;
        }

        /// <summary>
        /// The data source we want to create is a collection bound to a
        /// synthetic property on an XElement. We take the data in canonical
        /// construction form and create an XElement representation, then bind
        /// to a DependencyProperty on a FrameworkElement container. The
        /// binding path is the synthetic property, which for now is Elements.
        /// </summary>
        /// <param name="data">Data in canonical form.</param>
        /// <returns>An IEnumerable databound to an underlying XElement.</returns>
        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            IEnumerator dataEnumerator = data.GetEnumerator();
            dataEnumerator.MoveNext();

            _underlyingXElement = new XElement("Collection");

            PropertyDescriptor[] propertyDescriptors = GetByValueComparableProperties(dataEnumerator.Current);

            _rawDataPropertyDescriptors = new PropertyDescriptorCollection(propertyDescriptors);

            // For each data item we create an XElement, and for each property
            // we represent that as an attribute.
            do
            {
                XElement dataItem = new XElement(dataEnumerator.Current.GetType().Name);
                foreach (PropertyDescriptor propertyDescriptor in propertyDescriptors)
                {
                    dataItem.SetAttributeValue(propertyDescriptor.Name, propertyDescriptor.GetValue(dataEnumerator.Current));
                }
                _underlyingXElement.Add(dataItem);
            } while (dataEnumerator.MoveNext());

            // Bind the Collection property on our FrameworkElement container
            // to the Elements synthetic property on our underlying XElement.
            Binding binding = new Binding("Elements");
            binding.Source = _underlyingXElement;
            _collectionPropertyHolder = new CollectionPropertyHolder();
            _collectionPropertyHolder.SetBinding(CollectionPropertyHolder.CollectionProperty, binding);
            
            return _collectionPropertyHolder.Collection;
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// A FrameworkElement with an IEnumerable DependencyProperty that is
        /// needed for binding. See CreateDataSource method comments for
        /// details.
        /// </summary>
        private class CollectionPropertyHolder : FrameworkElement
        {
            public IEnumerable Collection
            {
                get { return (IEnumerable)GetValue(CollectionProperty); }
                set { SetValue(CollectionProperty, value); }
            }

            // Using a DependencyProperty as the backing store for collection.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty CollectionProperty =
                DependencyProperty.Register("Collection", typeof(IEnumerable), typeof(CollectionPropertyHolder), new UIPropertyMetadata(null));
        }

        /// <summary>
        /// A custom PropertyDescriptor that presents a formalized
        /// representation of a property on an XElement. For example, we
        /// represent a Place with a Name and State property as
        /// <Place Name="Seattle" State="WA" /> in XML, so we need to
        /// create a custom PropertyDescriptor that looks at the
        /// attribute to determine the value.
        /// </summary>
        private class XLinqPropertyDescriptor : PropertyDescriptor
        {
            private Type _propertyType;

            public XLinqPropertyDescriptor(string propertyName, Type propertyType)
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

                XElement xElement = component as XElement;

                if (xElement == null)
                {
                    throw new ArgumentException("Object must be an XElement.", "component");
                }

                TypeConverter typeConverter = TypeDescriptor.GetConverter(_propertyType);
                return typeConverter.ConvertFromInvariantString(xElement.Attribute(this.Name).Value);
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

        #endregion
    }
}
