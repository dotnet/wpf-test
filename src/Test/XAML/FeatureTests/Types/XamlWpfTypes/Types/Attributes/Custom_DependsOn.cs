// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// Custom_DependsOn Test
    /// </summary>
    [RuntimeNameProperty("Name")]
    public class Custom_DependsOn : FrameworkElement
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for AttachedDependsOnAttachedProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AttachedDependsOnAttachedPropertyProperty =
            DependencyProperty.RegisterAttached("AttachedDependsOnAttachedProperty", typeof(string), typeof(Custom_DependsOn));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DependencyProp.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DependencyPropProperty =
            DependencyProperty.Register("DependencyProp", typeof(int), typeof(Custom_DependsOn));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DependencyDependsOnStringProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DependencyDependsOnStringPropertyProperty =
            DependencyProperty.Register("DependencyDependsOnStringProperty", typeof(string), typeof(Custom_DependsOn));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DependencyDependsOnDependencyProp.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DependencyDependsOnDependencyPropProperty =
            DependencyProperty.Register("DependencyDependsOnDependencyProp", typeof(int), typeof(Custom_DependsOn));

        /// <summary>
        /// Using a DependencyProperty as the backing store for AttachedProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AttachedPropertyProperty =
            DependencyProperty.RegisterAttached("AttachedProperty", typeof(int), typeof(Custom_DependsOn));

        /// <summary> String Property </summary>
        private string _depStrProp = string.Empty;

        /// <summary> Name Property</summary>
        private string _depNameProp = string.Empty;

        /// <summary> Dependency String Property</summary>
        private string _depDepStrProp = string.Empty;

        /// <summary> Multiple Props</summary>
        private string _depMulProps = string.Empty;

        /// <summary> Dependency Property</summary>
        private string _depDepProp = string.Empty;

        /// <summary>
        /// Gets or sets the string property.
        /// </summary>
        /// <value>The string property.</value>
        public string StringProperty { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value.</value>
        public new string Name { get; set; }

        /// <summary>
        /// Gets or sets the depends on string property.
        /// </summary>
        /// <value>The depends on string property.</value>
        [DependsOn("StringProperty")]
        public string DependsOnStringProperty
        {
            get
            {
                return _depStrProp;
            }

            set
            {
                _depStrProp = value + StringProperty;
            }
        }

        /// <summary>
        /// Gets or sets the depends on name property.
        /// </summary>
        /// <value>The depends on name property.</value>
        [DependsOn("Name")]
        public string DependsOnNameProperty
        {
            get
            {
                return _depNameProp;
            }

            set
            {
                _depNameProp = value + "_" + Name;
            }
        }

        /// <summary>
        /// Gets or sets the depends on depends on string property.
        /// </summary>
        /// <value>The depends on depends on string property.</value>
        [DependsOn("DependsOnStringProperty")]
        public string DependsOnDependsOnStringProperty
        {
            get
            {
                return _depDepStrProp;
            }

            set
            {
                _depDepStrProp = value + "_" + DependsOnStringProperty;
            }
        }

        /// <summary>
        /// Gets or sets the depends on multiple properties.
        /// </summary>
        /// <value>The depends on multiple properties.</value>
        [DependsOn("Name")]
        [DependsOn("StringProperty")]
        public string DependsOnMultipleProperties
        {
            get
            {
                return _depMulProps;
            }

            set
            {
                _depMulProps = value + "_" + Name + "_" + StringProperty;
            }
        }

        /// <summary>
        /// Gets or sets the depends on dependency prop.
        /// </summary>
        /// <value>The depends on dependency prop.</value>
        [DependsOn("DependencyProp")]
        public string DependsOnDependencyProp
        {
            get
            {
                return _depDepProp;
            }

            set
            {
                _depDepProp = value + "_" + DependencyProp.ToString();
            }
        }

        #region DependencyProperties

        /// <summary>
        /// Gets or sets the dependency prop.
        /// </summary>
        /// <value>The dependency prop.</value>
        public int DependencyProp
        {
            get
            {
                return (int) GetValue(DependencyPropProperty);
            }

            set
            {
                SetValue(DependencyPropProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the dependency depends on string property.
        /// </summary>
        /// <value>The dependency depends on string property.</value>
        [DependsOn("StringProperty")]
        public string DependencyDependsOnStringProperty
        {
            get
            {
                return (string) GetValue(DependencyDependsOnStringPropertyProperty);
            }

            set
            {
                string strVal = value + "_" + StringProperty;
                SetValue(DependencyDependsOnStringPropertyProperty, strVal);
            }
        }

        /// <summary>
        /// Gets or sets the dependency depends on dependency prop.
        /// </summary>
        /// <value>The dependency depends on dependency prop.</value>
        [DependsOn("DependencyProp")]
        public int DependencyDependsOnDependencyProp
        {
            get
            {
                return (int) GetValue(DependencyDependsOnDependencyPropProperty);
            }

            set
            {
                int intVal = value + DependencyProp;
                SetValue(DependencyDependsOnDependencyPropProperty, value);
            }
        }

        #endregion

        #region AttachedProperties

        /// <summary>
        /// Gets the attacheddependsonattached property.
        /// </summary>
        /// <param name="obj">The dependency obj.</param>
        /// <returns> GetAttachedDependsOnAttachedProperty value </returns>
        [DependsOn("AttachedProperty")]
        public static string GetAttachedDependsOnAttachedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(AttachedDependsOnAttachedPropertyProperty);
        }

        /// <summary>
        /// Sets the attacheddependsonattached property.
        /// </summary>
        /// <param name="obj">The dependency obj.</param>
        /// <param name="value">The value.</param>
        public static void SetAttachedDependsOnAttachedProperty(DependencyObject obj, string value)
        {
            value += "_" + GetAttachedProperty(obj).ToString();
            obj.SetValue(AttachedDependsOnAttachedPropertyProperty, value);
        }

        /// <summary>
        /// Gets the attached property.
        /// </summary>
        /// <param name="obj">The dependency obj.</param>
        /// <returns> int value. </returns>
        public static int GetAttachedProperty(DependencyObject obj)
        {
            return (int)obj.GetValue(AttachedPropertyProperty);
        }

        /// <summary>
        /// Sets the attached property.
        /// </summary>
        /// <param name="obj">The dependency obj.</param>
        /// <param name="value">The value.</param>
        public static void SetAttachedProperty(DependencyObject obj, int value)
        {
            obj.SetValue(AttachedPropertyProperty, value);
        }

        #endregion
    }

    /*
    /// <summary>
    /// Custom_DependsOn_DependentName class
    /// </summary>
    [RuntimeNameProperty("Name")]
    public class Custom_DependsOn_DependentName
    {
        /// <summary>
        /// Name variable
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value .</value>
        [DependsOn("StringProperty")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value + "_" + StringProperty;
            }
        }

        /// <summary>
        /// Gets or sets the string property.
        /// </summary>
        /// <value>The string property.</value>
        public string StringProperty { get; set; }
    }

    /// <summary>
    /// Custom_DependsOn_Inherited class
    /// </summary>
    public class Custom_DependsOn_Inherited : Custom_DependsOn
    {
        /// <summary>
        /// DependsOn inherited
        /// </summary>
        private string depInherit = string.Empty;

        /// <summary>
        /// Gets or sets the depends on inherited string property.
        /// </summary>
        /// <value>The depends on inherited string property.</value>
        [DependsOn("StringProperty")]
        public string DependsOnInheritedStringProperty
        {
            get
            {
                return depInherit;
            }

            set
            {
                depInherit = value + "_" + StringProperty;
            }
        }
    }

    /// <summary>
    /// Custom_DependsOn_NonExistent class
    /// </summary>
    public class Custom_DependsOn_NonExistent
    {
        /// <summary>
        /// Gets or sets the string property.
        /// </summary>
        /// <value>The string property.</value>
        [DependsOn("NonExistemProperty")]
        public string StringProperty { get; set; }
    }

    /// <summary>
    /// Custom_DependsOn_Circular class
    /// </summary>
    public class Custom_DependsOn_Circular
    {
        /// <summary> First private variable </summary>
        private string first = string.Empty;

        /// <summary> Second private variable </summary>
        private string second = string.Empty;

        /// <summary>
        /// Gets or sets the first property.
        /// </summary>
        /// <value>The first property.</value>
        [DependsOn("SecondProperty")]
        public string FirstProperty
        {
            get
            {
                return first;
            }

            set
            {
                first = value + "_" + second;
            }
        }

        /// <summary>
        /// Gets or sets the second property.
        /// </summary>
        /// <value>The second property.</value>
        [DependsOn("FirstProperty")]
        public string SecondProperty
        {
            get
            {
                return second;
            }

            set
            {
                second = value + "_" + first;
            }
        }
    }

    /// <summary>
    /// Custom_DependsOn_Self class
    /// </summary>
    public class Custom_DependsOn_Self
    {
        /// <summary>
        /// String property variable
        /// </summary>
        private string strProp = string.Empty;

        /// <summary>
        /// Gets or sets the string property.
        /// </summary>
        /// <value>The string property.</value>
        [DependsOn("StringProperty")]
        public string StringProperty
        {
            get
            {
                return strProp;
            }

            set
            {
                strProp += "_" + value;
            }
        }
    }
     * */
}
