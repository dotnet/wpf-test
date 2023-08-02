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
    /// TestObject enable its DP to support IFinder transparently to its users.
    /// </summary>
    public abstract class TestObject : DependencyObject, ITestStep
    {
        #region Register

        private class TestPropertyValidator
        {
            private TestPropertyValidator(Type type)
            {
                _type = type;
            }

            private Type _type;

            public bool Validate(object value)
            {
                if (value == null)
                    return true;

                if (typeof(IFinder).IsInstanceOfType(value))
                    return true;
                return _type.IsInstanceOfType(value);
            }

            public static TestPropertyValidator GetValidator(Type type)
            {
                return new TestPropertyValidator(type);
            }
        }

        protected static DependencyProperty Register(string name, Type propertyType, Type ownerType)
        {
            return Register(name, propertyType, ownerType, new PropertyMetadata());
        }

        protected static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata propertyMetadata)
        {
            return DependencyProperty.Register(name, typeof(object), ownerType, propertyMetadata, new ValidateValueCallback(TestPropertyValidator.GetValidator(propertyType).Validate));
        }

        #endregion

        #region Property Setter/Getter

        protected object GetProperty(DependencyProperty dp)
        {
            object res = GetValue(dp);
            if (res is IFinder)
            {
                res = ((IFinder)res).Find(_source);
            }
            return res;
        }

        protected void SetProperty(DependencyProperty dp, object value)
        {
            SetValue(dp, value);
        }

        private object _source;

        #endregion

        #region Target

        public static readonly DependencyProperty TargetProperty = Register(
            "Target",
            typeof(object),
            typeof(TestObject),
            new PropertyMetadata(new Finding()));

        public object Target
        {
            get { return GetProperty(TargetProperty); }
            set { SetProperty(TargetProperty, value); }
        }

        #endregion

        #region ITestStep Members

        void ITestStep.Do(object testObject)
        {
            _source = testObject;

            try
            {
                DoCore();
            }
            finally
            {
                _source = null;
            }
        }

        protected abstract void DoCore();

        #endregion
    }
}
