// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Navigation;
using MS.Internal;

namespace DRT
{
    class CacheSuite : DrtTestSuite
    {
        const string GENERIC_DELIMITER = "`"; 

        public CacheSuite() : base("Cache")
        {
            TeamContact = "WPF";
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]{
                            new DrtTest(RunParameterTests),
                            new DrtTest(RunFunctionalityTests),
                        };
        }

        /// <summary>
        /// Finds appropriate objects and methods to use in the drt (via reflection)
        /// </summary>
        private void FindMethods()
        {
            if (_foundMethods)
            {
                return;
            }
            
            Assembly assembly = Assembly.GetAssembly(typeof(GlyphRun)); // to get PresentationCore.dll
            Type type = assembly.GetType("MS.Internal.SizeLimitedCache" + GENERIC_DELIMITER + "2");
            type = type.MakeGenericType(new Type[] { typeof(String), typeof(String) });
            foreach (MethodInfo method in type.GetMethods())
            {
                switch (method.Name)
                {
                    case "Add":
                        if (method.GetParameters().Length == 3)
                        {
                        _add = method;
                        }
                        break;
                    case "Remove":
                        _remove = method;
                        break;
                    case "Get":
                        _get = method;
                        break;
                }
            }

            _maximumItems = type.GetProperty("MaximumItems");

            _cache = Activator.CreateInstance(type, new object[] { _capacity });

            DRT.Assert(_add != null,
            "FindMethods: Wasn't able to find Add method");

            DRT.Assert(_remove != null,
            "FindMethods: Wasn't able to find Remove method");

            DRT.Assert(_get != null,
            "FindMethods: Wasn't able to find Get method");

            DRT.Assert(_maximumItems != null,
            "FindMethods: Wasn't able to find MaximumItems property");

            DRT.Assert(_cache != null,
            "FindMethods: Wasn't able to create instance of ObjectCache");

            _foundMethods = true;
        }

        /// <summary>
        /// Performs input parameter tests (makes sure exceptions are thrown).
        /// </summary>
        private void RunParameterTests()
        {
            FindMethods();
            bool success = false;

            try
            {
                success = false;
                Assembly assembly = Assembly.GetAssembly(typeof(GlyphRun)); // to get PresentationCore.dll
                Type type = assembly.GetType("MS.Internal.SizeLimitedCache" + GENERIC_DELIMITER + "2");
                type = type.MakeGenericType(new Type[] { typeof(Uri), typeof(String) });
                Activator.CreateInstance(type, new object[] { 0 });
            }
            catch (ApplicationException e)
            {
                if (e.InnerException is ArgumentOutOfRangeException)
                {
                    success = true;
                }
            }
            DRT.Assert(success, "ArgumentOutOfRangeException should have been thrown (maximumItems can't be <= 0)");

            try
            {
                success = false;
                _get.Invoke(_cache, new object[] { null });
            }
            catch (ApplicationException e)
            {
                if (e.InnerException is ArgumentNullException)
                {
                    success = true;
                }
            }
            DRT.Assert(success, "Get: ArgumentNullException should have been thrown");

            try
            {
                success = false;
                _remove.Invoke(_cache, new object[] { null });
            }
            catch (ApplicationException e)
            {
                if (e.InnerException is ArgumentNullException)
                {
                    success = true;
                }
            }
            DRT.Assert(success, "Remove: ArgumentNullException should have been thrown");

            try
            {
                success = false;
                _add.Invoke(_cache, new object[] { null, "test", false });
            }
            catch (ApplicationException e)
            {
                if (e.InnerException is ArgumentNullException)
                {
                    success = true;
                }
            }
            DRT.Assert(success,"Add (1): ArgumentNullException should have been thrown");

            try
            {
                success = false;
                _add.Invoke(_cache, new object[] { "test", null, false });
            }
            catch (ApplicationException e)
            {
                if (e.InnerException is ArgumentNullException)
                {
                    success = true;
                }
            }
            DRT.Assert(success, "Add (2): ArgumentNullException should have been thrown");
        }

        /// <summary>
        /// Runs a series of functionality tests for the ResourceCache.
        /// Makes sure it does what it should do.
        /// </summary>
        private void RunFunctionalityTests()
        {
            FindMethods();

            // Check MaximumItems.
            int capacity = (int)_maximumItems.GetValue(_cache, null);
            DRT.Assert(capacity == _capacity, "ERROR: MaximumItems: Expected {0}. Got {1}", _capacity, capacity);

            // cache state: empty
            VerifyCache("Getting something not in the cache.", new string[] { "A" }, new string[] { null });

            // cache state: empty
            AddToCache(new string[] { "A" }, new string[] { "1" }, new bool[] { false });
            VerifyCache("Simple Get.", new string[] { "A" }, new string[] { "1" });

            // cache state: keys: A. values: 1.
            /*
            AddToCache(new string[] { "B", "B", "B" },
            new string[] { "2", "3", "4" },
            new bool[] { true, true, false });
            VerifyCache("Permanent items should not be mutable.", new string[] { "B" }, new string[] { "2" });
            */

            // cache state: keys: A. values: 1
            AddToCache(new string[] { "A", "A", "B", "B", "C", "D", "D", "E" },
            new string[] { "junk", "1", "2", "2", "3", "4", "4", "5" },
            new bool[] { false, false, true, true, false, false, false, true });
            VerifyCache("Testing overwritting and capacity.",
            new string[] { "A", "B", "C", "D", "E" }, new string[] { "1", "2", "3", "4", "5" });

            // cache state: keys: A, B(perm), C, D, E(perm). values: 1, 2, 3, 4, 5
            _remove.Invoke(_cache, new object[] { "A" });
            _remove.Invoke(_cache, new object[] { "B" });
            VerifyCache("Verifying removal.", new string[] { "A", "B" }, new string[] { null, null });

            // cache state: keys: C, D, E(perm). values: 2, 3, 4, 5
            AddToCache(new string[] { "E", "F", "G" }, new string[] { "5", "6", "7" }, new bool[] { false, false, false });
            VerifyCache("Removal policy.",
            new string[] { "C", "D", "E", "F", "G" }, new string[] { null, null, "5", "6", "7" });
        }

        private void VerifyCache(string message, string[] keys, string[] values)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                string currentValue = (string)_get.Invoke(_cache, new object[] { keys[i] });
                if (currentValue != values[i])
                {
                    String error = "VerifyCache: " + message + " Lookup: " + keys[i] + " Expected: ";
                    if (values[i] == null)
                    {
                        error += "null";
                    }
                    else
                    {
                        error += values[i];
                    }
                    error += " Found: ";
                    if (currentValue == null)
                    {
                        error += "null";
                    }
                    else
                    {
                        error += currentValue;
                    }
                    DRT.Assert(false, error);
                }
            }
        }

        private void AddToCache(string[] keys, string[] values, bool[] permanence)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                _add.Invoke(_cache, new object[] { keys[i], values[i], permanence[i] });
            }
        }

        private MethodInfo _add;
        private MethodInfo _remove;
        private MethodInfo _get;
        private PropertyInfo _maximumItems;
        private int _capacity = 3;
        private Object _cache;
        private bool _foundMethods;
    }
}
