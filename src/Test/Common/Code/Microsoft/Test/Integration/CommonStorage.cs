// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Integration
{
      
    /// <summary>
    /// 
    /// </summary>
	public class CommonStorage
	{
        /// <summary>
        /// 
        /// </summary>
        public static void CleanAll()
        {
            CommonStorage cs = _commonStorage;
            _commonStorage = new CommonStorage();

            cs._storePerName.Clear();
            cs._storePerType.Clear();
        }

        /// <summary>
        ///  
        /// </summary>
        public static CommonStorage Current
        {
            get
            {
                return _commonStorage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public void Store(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            List<object> arrayList;
            Type t = o.GetType();

            arrayList = GetPerTypeList(t);
            arrayList.Add(o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="o"></param>
        public void Store(string name, object o)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name");
            }

            if (_storePerName.ContainsKey(name))
            {
                throw new InvalidOperationException("An object with the same name is already stored.");
            }

            _storePerName.Add(name, o);
            Store(o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Get(Type type)
        {
            if (!Contains(type))
            {
                return null;
            }

            List<object> arrayList = null;

            arrayList = GetPerTypeList(type);

            return arrayList[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Get(string name)
        {
            if (!Contains(name))
            {
                throw new InvalidOperationException("There is no object with that name stored.");
            }

            return _storePerName[name];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Contains(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return _storePerType.ContainsKey(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return _storePerName.ContainsKey(name);            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool ContainsItem(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            Type t = o.GetType();
            if (!Contains(t))
            {
                return false;
            }

            List<object> arrayList = null;

            arrayList = GetPerTypeList(t);

            return arrayList.Contains(o);
        }


        private List<object> GetPerTypeList(Type t)
        {
            List<object> arrayList;
            if (_storePerType.ContainsKey(t))
            {
                arrayList = _storePerType[t];
            }
            else
            {
                arrayList = new List<object>();
                _storePerType.Add(t, arrayList);
            }
            return arrayList;
        }

        Dictionary<string, object> _storePerName = new Dictionary<string, object>();
        Dictionary<Type, List<object>> _storePerType = new Dictionary<Type, List<object>>();

        static CommonStorage _commonStorage = new CommonStorage();

	}
}
