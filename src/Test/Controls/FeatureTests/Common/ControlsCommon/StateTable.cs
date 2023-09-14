//HashTable to hold values that is used to pass/share state information between Actions and Validations

using System;
using System.Collections;

namespace Avalon.Test.ComponentModel
{
    public static class StateTable
    {
        static StateTable()
        {
            stateTable = new Hashtable();
        }

        public static void Add(string key, object value)
        {
            stateTable.Add(key, value);
        }

        public static object Get(string key)
        {
            return stateTable[key];
        }

        public static void RemoveAll()
        {
            stateTable.Clear();
        }

        public static void Remove(string key)
        {
            stateTable.Remove(key);
        }

        public static bool Contains(string key)
        {
            return stateTable.Contains(key);
        }

        public static void SetValue(string key, object value)
        {
            if (stateTable.Contains(key))
            {
                stateTable[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }

        private static Hashtable stateTable;
    }
}
