// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Test.Input.MultiTouch
{
    #region public abstract class Property

    /// <summary>
    /// Base class for testing getters/setters
    /// </summary>
    public abstract class Property
    {
        #region private fields

        private readonly string _name;

        #endregion 

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public Property(string name)
        {
            Utils.Assert(name != null, "The parameter name should not be null");
            this._name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// the name of the property
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks that the properties enforce valid and invalid values
        /// </summary>
        /// <param name="properties"></param>
        public static void CheckValues(IEnumerable<Property> properties)
        {
            Utils.Assert(properties != null, "The parameter 'properties' should not be null in CheckValues");

            foreach (Property property in properties)
            {
                property.CheckValues();
            }
        }

        /// <summary>
        /// Checks that the properties have the expected default value
        /// </summary>
        /// <param name="properties"></param>
        public static void CheckDefault(IEnumerable<Property> properties)
        {
            Utils.Assert(properties != null, "The parameter properties should not be null in CheckDefault");

            foreach (Property property in properties)
            {
                property.CheckDefault();
            }
        }

        /// <summary>
        /// Checks that you get what you set
        /// </summary>
        /// <param name="properties"></param>
        public static void CheckSetGet(IEnumerable<Property> properties)
        {
            Utils.Assert(properties != null, "The parameter properties should not be null in CheckSetGet");
            
            foreach (Property property in properties)
            {
                property.CheckSetGet();
            }
        }

        /// <summary>
        /// Checks that the property enforces valie and invalid values
        /// </summary>
        public abstract void CheckValues();

        /// <summary>
        /// Checks that the property has the expected default value
        /// </summary>
        public abstract void CheckDefault();

        /// <summary>
        /// Checks that you get what you set
        /// </summary>
        public abstract void CheckSetGet();

        #endregion
    }

    #endregion
    
    #region public abstract class Property<>

    /// <summary>
    /// class for testing the behavior of a get/set property
    /// </summary>
    /// <typeparam name="TOwningType">the type of the object that has the property on it.</typeparam>
    /// <typeparam name="TPropertyType">the return type of the property.</typeparam>
    public abstract class Property<TOwningType, TPropertyType> : Property
    {
        #region Private Fields

        private readonly System.Func<bool, IEnumerable<TPropertyType>> _values;
        private readonly System.Func<TOwningType> _constructor;
        private readonly System.Func<TOwningType, TPropertyType> _getter;
        private readonly System.Action<TOwningType, TPropertyType> _setter;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="values">A function for enumerating valid and invalid values.</param>
        /// <param name="constructor">A function for constructing the owning type.</param>
        /// <param name="getter">A function to get the property value.</param>
        /// <param name="setter">An action that sets the property value.</param>
        public Property(
            string name,
            System.Func<bool, IEnumerable<TPropertyType>> values,
            System.Func<TOwningType> constructor,
            System.Func<TOwningType, TPropertyType> getter,
            System.Action<TOwningType, TPropertyType> setter)
            : base(name)
        {
            Utils.Assert(values != null, "The parameter values should not be null");
            Utils.Assert(constructor != null, "The parameter constructor should not be null");
            Utils.Assert(getter != null, "The parameter getter should not be null");
            Utils.Assert(setter != null, "The parameter setter should not be null");

            this._values = values;
            this._constructor = constructor;
            this._getter = getter;
            this._setter = setter;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the constructor function for the owning type.
        /// </summary>
        public System.Func<TOwningType> Constructor
        {
            get { return this._constructor; }
        }

        /// <summary>
        /// Gets the function for getting a property value from the owning type.
        /// </summary>
        public System.Func<TOwningType, TPropertyType> Getter
        {
            get { return this._getter; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks that the property enforces valid and invalid values
        /// </summary>
        public override void CheckValues()
        {
            TOwningType owner = _constructor();

            ValueTests.Try<TPropertyType>(
                (value) => this._setter(owner, value),
                this._values,
                Name);
        }

        /// <summary>
        /// Checks that if you set the value and then get it, you get 
        /// the value that you put in.
        /// </summary>
        public override void CheckSetGet()
        {
            TOwningType owner = Constructor();

            foreach (TPropertyType value in this._values(true))
            {
                this._setter(owner, value);
                TPropertyType result = this._getter(owner);
                Utils.Assert(CompareValues(result, value), 
                    string.Format("Property {0} returned different {1} after being set to {2}",Name,result,value));
            }
        }

        /// <summary>
        /// Picks a valid value and sets it
        /// </summary>
        public void SetValidValue(TOwningType owner)
        {
            IEnumerator<TPropertyType> enumerator = this._values(true).GetEnumerator();
            enumerator.MoveNext();
            this._setter(owner, enumerator.Current);
        }

        /// <summary>
        /// Compares two property values.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        protected virtual bool CompareValues(TPropertyType value1, TPropertyType value2)
        {
            return value1.Equals(value2);
        }

        #endregion
    }

    #endregion

}
