// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.AddIn
{
    /// <summary>
    /// Object to hold data about focus
    /// </summary>
    [Serializable]
    public sealed class FocusItem : IComparable
    {

        #region Public Static Methods

        //public static FocusItem ConvertFromString(string focusString)
        //{
        //    object hostContainerObject = XamlReader.Load(stream);

        //    FocusItem item = new FocusItem();
        //    item.Name = (string)dictionaryItem["Name"];
        //    item.DateTime = (DateTime)dictionaryItem["DateTime"];
        //    return item;
        //}

        //public static string ConvertToString(FocusItem focusItem)
        //{
        //    Dictionary<string, object> dictionary = new Dictionary<string, object>();
        //    dictionary.Add("Name", focusItem.Name);
        //    dictionary.Add("DateTime", focusItem.DateTime);
        //    return dictionary;
        //}

        #endregion
        
        #region Private Members

        private DateTime _dateTime;
        private string _name;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor. Initializes to Now and Empty string
        /// </summary>
        public FocusItem() : this(DateTime.Now, "") { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dateTime">DateTime that the UI received focus</param>
        /// <param name="name">Name of the UI element that recieved focus</param>
        public FocusItem(DateTime dateTime, string name)
        {
            this._dateTime = dateTime;
            this._name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Time that the UI element recieved focus
        /// </summary>
        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        /// <summary>
        /// Name of the UI element that had focus
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is FocusItem)
            {
                FocusItem item = (FocusItem)obj;
                return _dateTime.CompareTo(item.DateTime);
            }
            else
            {
                throw new ArgumentException("object is not a FocusItem");
            }
        }

        #endregion

    }
}
