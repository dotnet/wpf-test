// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data to be used for testing the DataTransfer extensibility

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Data/DataTransferExtensibilityData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Threading;
    using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Animation;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>Provides the extensibility points for data transfer.</summary>
    public enum DataTransferExtensibility
    {
        /// <summary>Extensibility at format-setting point.</summary>
        Setting,

        /// <summary>Extensibility at full DataObject copying point.</summary>
        Copying,

        /// <summary>Extensibility at DataObject pasting point.</summary>
        Pasting,
    }

    /// <summary>General callback used for any data transfer extensibility.</summary>
    /// <param name='sender'>Control firing event.</param>
    /// <param name='args'>Arguments in event (typically a DataObjectEventArgs subtype).</param>
    public delegate void DataExtensibilityCallback(object sender, DataObjectEventArgs args);

    /// <summary>Provides information about data transfer extensibility points.</summary>
    public class DataTransferExtensibilityData
    {

        #region Constructors.

        /// <summary>Hides the constructor.</summary>
        private DataTransferExtensibilityData() { }

        #endregion Constructors.


        #region Public methods.

        /// <summary>
        /// Adds an event handler for the extensibility point to the
        /// specified target object.
        /// </summary>
        /// <param name="target">Object to add handler to.</param>
        /// <param name="handler">Delegate (of the correct subtype) to add to target.</param>
        public void AddHandler(DependencyObject target, Delegate handler)
        {
            switch (_value)
            {
                case DataTransferExtensibility.Copying:
                    DataObject.AddCopyingHandler(target, (DataObjectCopyingEventHandler)handler);
                    break;
                case DataTransferExtensibility.Pasting:
                    DataObject.AddPastingHandler(target, (DataObjectPastingEventHandler)handler);
                    break;
                case DataTransferExtensibility.Setting:
                    DataObject.AddSettingDataHandler(target, (DataObjectSettingDataEventHandler)handler);
                    break;
            }
        }

        /// <summary>
        /// Creates a delegate of the correct type for the extensibility point,
        /// that calls the generic specified callback on invocation.
        /// </summary>
        /// <param name="callback">Callback to call on invocation.</param>
        /// <returns>A delegate that can be used with AddHandler / RemoveHandler.</returns>
        public Delegate CreateDelegate(DataExtensibilityCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            switch(_value)
            {
                case DataTransferExtensibility.Copying:
                    return new DataObjectCopyingEventHandler(delegate(object sender, DataObjectCopyingEventArgs e) {
                        callback(sender, e);
                    });
                case DataTransferExtensibility.Pasting:
                    return new DataObjectPastingEventHandler(delegate(object sender, DataObjectPastingEventArgs e) {
                        callback(sender, e);
                    });
                case DataTransferExtensibility.Setting:
                    return new DataObjectSettingDataEventHandler(delegate(object sender, DataObjectSettingDataEventArgs e) {
                        callback(sender, e);
                    });
                default:
                    throw new Exception("Unknown value: " + _value);
            }
        }

        /// <summary>
        /// Removes an event handler for the extensibility point from the
        /// specified target object.
        /// </summary>
        /// <param name="target">Object to remove handler from.</param>
        /// <param name="handler">Delegate (of the correct subtype) to remove from target.</param>
        public void RemoveHandler(DependencyObject target, Delegate handler)
        {
            switch (_value)
            {
                case DataTransferExtensibility.Copying:
                    DataObject.RemoveCopyingHandler(target, (DataObjectCopyingEventHandler)handler);
                    break;
                case DataTransferExtensibility.Pasting:
                    DataObject.RemovePastingHandler(target, (DataObjectPastingEventHandler)handler);
                    break;
                case DataTransferExtensibility.Setting:
                    DataObject.RemoveSettingDataHandler(target, (DataObjectSettingDataEventHandler)handler);
                    break;
            }
        }

        /// <summary>Provides a string representation of the object.</summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>The RoutedEvent associated with this extensibility point.</summary>
        public RoutedEvent ExtensibilityEvent
        {
            get { return _extensibilityEvent; }
        }

        /// <summary>The extensibility point this instance encapsulates.</summary>
        public DataTransferExtensibility Value
        {
            get { return this._value; }
        }

        /// <summary>Extensibility point instances.</summary>
        public static DataTransferExtensibilityData[] Values
        {
            get
            {
                if (s_values == null)
                {
                    s_values = new DataTransferExtensibilityData[] {
                        ForData(DataTransferExtensibility.Setting, DataObject.SettingDataEvent),
                        ForData(DataTransferExtensibility.Copying, DataObject.CopyingEvent),
                        ForData(DataTransferExtensibility.Pasting, DataObject.PastingEvent),
                    };
                }
                return s_values;
            }
        }

        #endregion Public properties.


        #region Private methods.

        /// <summary>Initializes a new DataTransferExtensibilityData instance.</summary>
        private static DataTransferExtensibilityData ForData(
            DataTransferExtensibility value, RoutedEvent routedEvent)
        {
            DataTransferExtensibilityData result;

            result = new DataTransferExtensibilityData();
            result._extensibilityEvent = routedEvent;
            result._value = value;

            return result;
        }

        #endregion Private methods.


        #region Private fields.

        /// <summary>The RoutedEvent associated with this extensibility point.</summary>
        private RoutedEvent _extensibilityEvent;

        /// <summary>The extensibility point this instance encapsulates.</summary>
        private DataTransferExtensibility _value;

        /// <summary>Extensibility point instances.</summary>
        private static DataTransferExtensibilityData[] s_values;

        #endregion Private fields.
    }
}