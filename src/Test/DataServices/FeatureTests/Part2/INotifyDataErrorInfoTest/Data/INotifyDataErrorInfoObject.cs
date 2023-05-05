// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;

    /// <summary>
    /// Base class for data object used by INofifyDataErrorInfo tests.
    /// Class implements INotifyDataErrorInfo and INotifyPropertyChanged and provides validation 
    /// methods for string and int propties that will be used for binding.
    /// </summary>
    /// <typeparam name="T">Type of error object used in error list</typeparam>
    public abstract class INotifyDataErrorInfoObject<T> : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        #region Constructor

        public INotifyDataErrorInfoObject(bool async)
        {
            this._async = async;
            this._errors = new Dictionary<string, List<T>>();
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// String Property used for binding.  Property validation will check
        /// 1) If string is longer than 10 characters
        /// 2) If string contains '#'
        /// </summary>
        public string StringProperty
        {
            get { return _stringProperty; }
            set
            {
                ValidateStringProperty("StringProperty", value);
                _stringProperty = value;
                NotifyPropertyChanged("StringProperty");
            }
        }

        /// <summary>
        /// Int Property used for binding.  Property validation will check
        /// 1) If int is > 999
        /// 2) If int is odd
        /// 3) If int is negative, show error for StringProperty
        /// 4) If not an int, default convert will throw an exception
        /// </summary>
        public int IntProperty
        {
            get { return _intProperty; }
            set
            {
                ValidateIntProperty("IntProperty", value);
                _intProperty = value;
                NotifyPropertyChanged("IntProperty");
            }
        }

        /// <summary>
        /// If set this will simulate a long working time on working thread, 
        /// allowing updates to queue up.
        /// </summary>
        public int ThreadWait { get; set; }

        /// <summary>
        /// If true, INotifyDataErrorInfo.GetErrors will always return null
        /// </summary>
        public bool GetErrorsAlwaysNull { get; set; }

        /// <summary>
        /// If true, INotifyDataErrorInfo.GetErrors will throw an exception
        /// </summary>
        public bool GetErrorsThrows { get; set; }

        /// <summary>
        /// If true and GetErrorsThrows is true, INotifyDataErrorInfo.GetErrors will throw a critical application exception
        /// </summary>
        public bool ThrowCritical { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Used by subclasses to update errors during property validation
        /// </summary>
        /// <param name="method">UpdateErrorMethod to use</param>
        /// <param name="key">name of property</param>
        /// <param name="value">error object</param>
        public void UpdateErrors(UpdatErrorMethod method, string key, T value)
        {
            if (_async)
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    if (ThreadWait > 0)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(ThreadWait));
                    }

                    method.Invoke(key, value);
                });
            }
            else
            {
                method.Invoke(key, value);
            }
        }

        /// <summary>
        /// Override in subclass for specific IntProperty validation
        /// </summary>
        /// <param name="key">name of property</param>
        /// <param name="value">error object</param>
        public abstract void ValidateIntProperty(string key, int value);
        
        /// <summary>
        /// Override in subclass for specific StringProperty validation
        /// </summary>
        /// <param name="key">name of property</param>
        /// <param name="value">error object</param>
        public abstract void ValidateStringProperty(string key, string value);

        /// <summary>
        /// Add error to error dictionary
        /// </summary>
        /// <param name="key">name of key to look up</param>
        /// <param name="value">object to add</param>
        public void AddError(string key, T value)
        {
            lock (_lockObject)
            {
                if (!_errors.ContainsKey(key))
                {
                    _errors.Add(key, new List<T>());
                }

                if (!_errors[key].Contains(value))
                {
                    _errors[key].Add(value);
                    NotifyErrorsChanged(key);
                }
            }
        }

        /// <summary>
        /// Remove error from error dictionary
        /// </summary>
        /// <param name="key">name of key to look up</param>
        /// <param name="value">object to remove</param>
        public void RemoveError(string key, T value)
        {
            lock (_lockObject)
            {
                if (_errors.ContainsKey(key))
                {
                    if (_errors[key].Contains(value))
                    {
                        _errors[key].Remove(value);

                        NotifyErrorsChanged(key);
                    }
                }
            }
        }

        /// <summary>
        /// Delegate definition for update error methods.
        /// </summary>
        public delegate void UpdatErrorMethod(string key, T value);

        #endregion Public Methods

        #region Private Methods

        private void NotifyErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
        
        #endregion Private Methods

        #region Private Fields

        private int _intProperty = 0;
        private string _stringProperty = string.Empty;
        private bool _async = false;
        private object _lockObject = new object();
        private Dictionary<string, List<T>> _errors = null;

        #endregion Private Fields

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        // 
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            if (GetErrorsAlwaysNull)
                return null;

            if (GetErrorsThrows)
            {
                if (ThrowCritical)
                {
                    throw new System.Security.SecurityException("TEST!!! SecurityException thrown in INotifyDataErrorInfo.GetErrors."); 
                }
                else
                {
                    throw new Exception("TEST!!! Exception thrown in INotifyDataErrorInfo.GetErrors.");
                }
            }

            List<T> errorlist;

            if (!_errors.TryGetValue(propertyName, out errorlist))
            {
                errorlist = null;
            }

            return errorlist;
        }

        bool INotifyDataErrorInfo.HasErrors
        {
            get { return (_errors.Count > 0); }
        }

        #endregion INotifyDataErrorInfo

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion INotifyPropertyChanged
    }

    /// <summary>
    /// String used in error messages and test validations
    /// </summary>
    public static class ErrorStrings
    {
        public static string STRING_TOO_LONG = "String is too long.";
        public static string STRING_INVALID_CHARACTER = "String should note contain the # symbol.";
        public static string INT_CANNOT_BE_NEGATIVE = "Int value cannot be negative.";
        public static string INT_CANNOT_BE_ODD = "Int value cannot be odd.";
        public static string INT_TOO_LARGE = "Int value is too large.";
        public static string VALIDATION_RULE = "Custom validation rule error.";
        public static string EXCEPTION = "This is an expection message.";
    }
}
