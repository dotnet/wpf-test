// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Reflection;
    using System.Xaml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Driver;

    public class ItemType
    {
        private string _itemName;
        private double _price;

        public ItemType()
        {
        }

        public ItemType(string itemName, double price)
        {
            this._itemName = itemName;
            this._price = price;
        }

        public string ItemName
        {
            get
            {
                return _itemName;
            }
            set
            {
                _itemName = value;
            }
        }
        public double Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
            }
        }
    }

    public class CollectionContainerType1
    {
        private ArrayList _arrayItems = new ArrayList();
        private Hashtable _hashTable = new Hashtable();
        private ItemType[] _items = new ItemType[3];
        private Dictionary<string, ItemType> _dictionary = new Dictionary<string, ItemType>();
        private GenericType1<string, int>[] _genericArray = new GenericType1<string, int>[3];

        public CollectionContainerType1()
        {
        }

        public ArrayList ArrayList
        {
            get
            {
                return _arrayItems;
            }
            set
            {
                this._arrayItems = value;
            }
        }

        public Hashtable HashTable
        {
            get
            {
                return _hashTable;
            }
            set
            {
                _hashTable = value;
            }
        }

        public ItemType[] Items
        {
            get
            {
                return this._items;
            }
            set
            {
                this._items = value;
            }
        }

        public Dictionary<string, ItemType> Dictionary
        {
            get
            {
                return this._dictionary;
            }
            set
            {
                this._dictionary = value;
            }
        }

        public GenericType1<string, int>[] GenericArray
        {
            get
            {
                return this._genericArray;
            }
            set
            {
                this._genericArray = value;
            }
        }

        #region Test Implementation

        // collection initialized in type
        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // basic sanity
            CollectionContainerType1 instance1 = new CollectionContainerType1();
            instance1.ArrayList.Add(new ItemType("Name1", 12));
            instance1.Items[0] = new ItemType("Name2", 13);
            instance1.HashTable.Add("Name4", "aaaaaa");
            instance1.Dictionary.Add("aaa", new ItemType("Name3", 14.11));
            instance1.GenericArray[0] = new GenericType1<string, int>
                                            {
                                                Info1 = "blah",
                                                Info2 = 3
                                            };
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            // empty collections
            CollectionContainerType1 instance2 = new CollectionContainerType1();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  TestID = instanceIDPrefix + 2
                              });

            // null in collections
            CollectionContainerType1 instance3 = new CollectionContainerType1();
            instance3.Items[0] = null;
            instance3.Items[1] = new ItemType("Name1", 0);
            instance3.Items[2] = null;
            instance3.HashTable.Add("Name2", null);
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.Dictionary.Add("aaa", null);
            instance3.Dictionary.Add("bbb", new ItemType
                                                {
                                                    ItemName = null,
                                                    Price = -123
                                                });
            instance1.GenericArray[0] = new GenericType1<string, int>
                                            {
                                                Info1 = "blah",
                                                Info2 = 3
                                            };
            instance1.GenericArray[1] = null;
            instance1.GenericArray[2] = new GenericType1<string, int>
                                            {
                                                Info1 = null,
                                                Info2 = 3
                                            };
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance3,
                                  TestID = instanceIDPrefix + 3,
                              });

            return testCases;
        }

        #endregion
    }

    // Collection not initialized in type
    public class CollectionContainerType2
    {
        private ArrayList _arrayList;
        private Hashtable _hashTable;
        private ItemType[] _items;
        private Dictionary<string, ItemType> _dictionary;

        public CollectionContainerType2()
        {
        }

        public ArrayList ArrayList
        {
            get
            {
                return _arrayList;
            }
            set
            {
                this._arrayList = value;
            }
        }

        public Hashtable HashTable
        {
            get
            {
                if (this._hashTable == null)
                    this._hashTable = new Hashtable();
                return _hashTable;
            }
            set
            {
                _hashTable = value;
            }
        }

        public ItemType[] Items
        {
            get
            {
                return this._items;
            }
            set
            {
                this._items = value;
            }
        }

        public Dictionary<string, ItemType> Dictionary
        {
            get
            {
                if (this._dictionary == null)
                {
                    this._dictionary = new Dictionary<string, ItemType>();
                }
                return this._dictionary;
            }
            set
            {
                this._dictionary = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // basic sanity
            CollectionContainerType2 instance1 = new CollectionContainerType2();
            instance1._items = new ItemType[3];
            instance1._arrayList = new ArrayList();
            instance1._dictionary = new Dictionary<string, ItemType>();
            instance1._hashTable = new Hashtable();
            instance1.ArrayList.Add(new ItemType("Name1", 12));
            instance1.Items[0] = new ItemType("Name2", 13);
            instance1.HashTable.Add("Name4", "aaaaaa");
            instance1.Dictionary.Add("aaa", new ItemType("Name3", 14.11));
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1,
                              });

            // empty collections
            CollectionContainerType2 instance2 = new CollectionContainerType2();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  TestID = instanceIDPrefix + 2
                              });

            // null in collections
            CollectionContainerType2 instance3 = new CollectionContainerType2();
            instance3._items = new ItemType[3];
            instance3._arrayList = new ArrayList();
            instance3._dictionary = new Dictionary<string, ItemType>();
            instance3._hashTable = new Hashtable();
            instance3.Items[0] = null;
            instance3.Items[1] = new ItemType("Name1", 0);
            instance3.Items[2] = null;
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.HashTable.Add("Name2", null);
            instance3.Dictionary.Add("aaa", null);
            instance3.Dictionary.Add("bbb", new ItemType
                                                {
                                                    ItemName = null,
                                                    Price = -123
                                                });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance3,
                                  TestID = instanceIDPrefix + 3,
                              });

            return testCases;
        }

        #endregion
    }

    // RO collection initialized in type
    public class CollectionContainerType3
    {
        private readonly ArrayList _arrayList = new ArrayList();
        private readonly Hashtable _hashTable = new Hashtable();
        private readonly Dictionary<string, ItemType> _dictionary = new Dictionary<string, ItemType>();

        public CollectionContainerType3()
        {
        }

        public ArrayList ArrayList
        {
            get
            {
                return _arrayList;
            }
        }

        public Hashtable HashTable
        {
            get
            {
                return _hashTable;
            }
        }

        public Dictionary<string, ItemType> Dictionary
        {
            get
            {
                return this._dictionary;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // basic sanity
            CollectionContainerType3 instance1 = new CollectionContainerType3();
            instance1.ArrayList.Add(new ItemType("Name1", 12));
            instance1.HashTable.Add("Name4", "aaaaaa");
            instance1.Dictionary.Add("aaa", new ItemType("Name3", 14.11));
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            // empty collections
            // 
            CollectionContainerType3 instance2 = new CollectionContainerType3();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  TestID = instanceIDPrefix + 2
                              });

            // null in collections
            // 9060: general null roundtripping failure
            CollectionContainerType3 instance3 = new CollectionContainerType3();
            instance3.HashTable.Add("Name2", null);
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.Dictionary.Add("aaa", null);
            instance3.Dictionary.Add("bbb", new ItemType
                                                {
                                                    ItemName = null,
                                                    Price = -123
                                                });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance3,
                                  TestID = instanceIDPrefix + 3
                              });

            return testCases;
        }

        #endregion
    }

    // RO collection not initialized in type - should failed.
    // unless it is empty collection - not called during deserialziation
    public class CollectionContainerType4
    {
        private ArrayList _arrayList;
        private Hashtable _hashTable;
        private ItemType[] _items;
        private Dictionary<string, ItemType> _dictionary;

        public CollectionContainerType4()
        {
        }

        public ArrayList ArrayList
        {
            get
            {
                return _arrayList;
            }
        }

        public Hashtable HashTable
        {
            get
            {
                return _hashTable;
            }
        }

        public ItemType[] Items
        {
            get
            {
                return this._items;
            }
        }

        public Dictionary<string, ItemType> Dictionary
        {
            get
            {
                return this._dictionary;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // basic sanity
            CollectionContainerType4 instance1 = new CollectionContainerType4();
            instance1._items = new ItemType[3];
            instance1._arrayList = new ArrayList();
            instance1._hashTable = new Hashtable();
            instance1._dictionary = new Dictionary<string, ItemType>();
            instance1.ArrayList.Add(new ItemType("Name1", 12));
            instance1.Items[0] = new ItemType("Name2", 13);
            instance1.HashTable.Add("Name4", "aaaaaa");
            instance1.Dictionary.Add("aaa", new ItemType("Name3", 14.11));
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  ExpectedMessage = Exceptions.GetMessage("GetObjectNull", WpfBinaries.SystemXaml),
                                  ExpectedResult = false,
                                  TestID = instanceIDPrefix + 1
                              });

            // empty collections
            CollectionContainerType4 instance2 = new CollectionContainerType4();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 2
                              });

            // null in collections
            CollectionContainerType4 instance3 = new CollectionContainerType4();
            instance3._items = new ItemType[3];
            instance3._arrayList = new ArrayList();
            instance3._hashTable = new Hashtable();
            instance3._dictionary = new Dictionary<string, ItemType>();
            instance3.Items[0] = null;
            instance3.Items[1] = new ItemType("Name1", 0);
            instance3.Items[2] = null;
            instance3.HashTable.Add("Name2", null);
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.Dictionary.Add("aaa", null);
            instance3.Dictionary.Add("bbb", new ItemType
                                                {
                                                    ItemName = null,
                                                    Price = -123
                                                });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance3,
                                  ExpectedResult = false,
                                  ExpectedMessage = Exceptions.GetMessage("GetObjectNull", WpfBinaries.SystemXaml),
                                  TestID = instanceIDPrefix + 3
                              });

            return testCases;
        }

        #endregion
    }

    // multiple kinds of collection like IList<int> and IList<string>
    public abstract class CollectionContainerType5 : IList<int>, IList<string>
    {
        protected IList<int> intList = new List<int>();
        protected IList<string> stringList = new List<string>();

        #region IList<string> Members

        public int IndexOf(string item)
        {
            return stringList.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            stringList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            stringList.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return stringList[index];
            }
            set
            {
                stringList[index] = value;
            }
        }

        #endregion

        #region ICollection<string> Members

        public void Add(string item)
        {
            stringList.Add(item);
        }

        public void Clear()
        {
            stringList.Clear();
        }

        public bool Contains(string item)
        {
            return stringList.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            stringList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return stringList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return stringList.IsReadOnly;
            }
        }

        public bool Remove(string item)
        {
            return stringList.Remove(item);
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return stringList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalGetEnumerator();
        }

        #endregion

        #region IList<int> Members

        public int IndexOf(int item)
        {
            return intList.IndexOf(item);
        }

        public void Insert(int index, int item)
        {
            intList.Insert(index, item);
        }

        int IList<int>.this[int index]
        {
            get
            {
                return intList[index];
            }
            set
            {
                intList[index] = value;
            }
        }

        #endregion

        #region ICollection<int> Members

        public void Add(int item)
        {
            intList.Add(item);
        }

        public bool Contains(int item)
        {
            return intList.Contains(item);
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            intList.CopyTo(array, arrayIndex);
        }

        public bool Remove(int item)
        {
            return intList.Remove(item);
        }

        #endregion

        #region IEnumerable<int> Members

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            return intList.GetEnumerator();
        }

        #endregion

        protected abstract IEnumerator InternalGetEnumerator();
    }

    // 

    public class CollectionContainerType5GoodEnumerator : CollectionContainerType5
    {
        // non generic enumerator includes both strings and ints
        protected override IEnumerator InternalGetEnumerator()
        {
            ArrayList combinedList = new ArrayList();
            int[] ints = new int[intList.Count];
            intList.CopyTo(ints, 0);
            string[] strings = new string[stringList.Count];
            stringList.CopyTo(strings, 0);
            combinedList.AddRange(ints);
            combinedList.AddRange(strings);
            return combinedList.GetEnumerator();
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            CollectionContainerType5GoodEnumerator instance1 = new CollectionContainerType5GoodEnumerator();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 1,
                              }
                );

            CollectionContainerType5GoodEnumerator instance2 = new CollectionContainerType5GoodEnumerator
                                                                   {
                                                                       Int16.MaxValue,
                                                                       "blah"
                                                                   };

            // [DISABLED]
            // testCases.Add(new TestCaseInfo
            //                   {
            //                       Target = instance2,
            //                       ExpectedResult = true,
            //                       TestID = instanceIDPrefix + 2,
            //                   });
            return testCases;
        }

        #endregion
    }

    public class CollectionContainerType5BadEnumerator : CollectionContainerType5
    {
        // non-generic enumerator only contains ints
        protected override IEnumerator InternalGetEnumerator()
        {
            return ((IEnumerable)intList).GetEnumerator();
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            CollectionContainerType5BadEnumerator instance1 = new CollectionContainerType5BadEnumerator();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 1,
                              });

            CollectionContainerType5BadEnumerator instance2 = new CollectionContainerType5BadEnumerator()
                                                                  {
                                                                      Int32.MinValue,
                                                                      "blah"
                                                                  };
            // [DISABLED] : Known Bug / Test Case Hang ?
            // testCases.Add(new TestCaseInfo
            //                   {
            //                       Target = instance2,
            //                       ExpectedResult = false,
            //                       TestID = instanceIDPrefix + 2,
            //                       ExpectedMessage = "Two Xaml Ojects are different.",
            //                   });
            return testCases;
        }

        #endregion
    }

    // collection composition
    // jagged array
    public class CollectionContainerType6
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();
            ItemType item = new ItemType("My item", double.Epsilon);

            CollectionContainerType6 instance1 = new CollectionContainerType6();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 1
                              });

            CollectionContainerType6 instance2 = new CollectionContainerType6();
            instance2.ArrayOfArrayOfItems = new ItemType[5][];

            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 2
                              });

            CollectionContainerType6 instance3 = new CollectionContainerType6();
            instance3.ArrayOfArrayOfItems = new ItemType[2][]
            {
                new ItemType[] { new ItemType { ItemName = "name", Price = 3.14 } },
                new ItemType[] { new ItemType { ItemName = "name", Price = 3.14 }, new ItemType { ItemName = "name", Price = 3.14 } },
            };

            testCases.Add(new TestCaseInfo
            {
                Target = instance3,
                ExpectedResult = true,
                TestID = instanceIDPrefix + 3
            });

            ItemType[,] multidimensionalArray = new ItemType[2, 2]
            {
                { new ItemType { ItemName = "name", Price = 3.14 }, new ItemType { ItemName = "name", Price = 3.14 } },
                { new ItemType { ItemName = "name", Price = 3.14 }, new ItemType { ItemName = "name", Price = 3.14 } },
            };

            testCases.Add(new TestCaseInfo
            {
                Target = multidimensionalArray,
                ExpectedResult = false,
                TestID = instanceIDPrefix + 4,
                ExpectedMessage = Exceptions.GetMessage("ObjectReaderMultidimensionalArrayNotSupported", WpfBinaries.SystemXaml),
            });

            var arrayTypes = new List<Type>()
            {
                typeof(string[]),
                typeof(string[][]),
                typeof(string[,]),
                typeof(string[][,]),
                typeof(string[,][])
            };

            // [DISABLED] : [DISABLED] Known Failure / Test Case Hang ?
            // testCases.Add(new TestCaseInfo
            //{
            //   Target = arrayTypes,
            //   ExpectedResult = true,
            //   TestID = instanceIDPrefix + 5,
            //});

            return testCases;
        }

        #endregion

        public ItemType[][] ArrayOfArrayOfItems { get; set; }
    }

    // collection composition
    // generic list of generic lists
    public class CollectionContainerType7
    {
        private List<List<ItemType>> _itemListList = new List<List<ItemType>>();
        public List<List<ItemType>> ItemListList
        {
            get
            {
                return _itemListList;
            }
            set
            {
                _itemListList = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();
            ItemType item = new ItemType("My item", double.Epsilon);

            CollectionContainerType7 instance1 = new CollectionContainerType7();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 1
                              });

            CollectionContainerType7 instance2 = new CollectionContainerType7();

            for (int i = 0; i < 5; i++)
            {
                List<ItemType> items = new List<ItemType>();
                for (int j = 0; j < 5; j++)
                {
                    item.Price += 1.0;

                    items.Add(new ItemType(item.ItemName, item.Price));
                }

                instance2.ItemListList.Add(items);
            }

            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 2
                              });

            CollectionContainerType7 instance3 = new CollectionContainerType7();

            for (int i = 0; i < 5; i++)
            {
                List<ItemType> items = new List<ItemType>();

                for (int j = 0; j < 5; j++)
                {
                    item.Price += 1.0;
                    items.Add((i + j) % 2 == 0 ? null : new ItemType(item.ItemName, item.Price));
                }
                instance3.ItemListList.Add(items);
            }

            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance3,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 3
                              });

            return testCases;
        }

        #endregion
    }

    // most common generic collections
    // Dictionary<K,V>, List<T>, Collection<T>
    public class CollectionContainerType8
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            Dictionary<string, ItemType> emptyDict = new Dictionary<string, ItemType>();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = emptyDict,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 1, 
                              });

            Dictionary<string, ItemType> dictionary = new Dictionary<string, ItemType>
                                                      {
                                                          {
                                                              "Item1", new ItemType("Item1", 3.14)
                                                              },
                                                          {
                                                              "Item2", null
                                                              },
                                                          {
                                                              "Item3", new ItemType(null, 6.28)
                                                              }
                                                      };
            testCases.Add(new TestCaseInfo
                              {
                                  Target = dictionary,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 2, 
                              });

            List<ItemType> emptyList = new List<ItemType>();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = emptyList,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 3, 
                              });

            List<ItemType> list = new List<ItemType>();
            list.AddRange(dictionary.Values);

            testCases.Add(new TestCaseInfo
                              {
                                  Target = list,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 4, 
                              });

            Collection<ItemType> emptyColl = new Collection<ItemType>();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = emptyColl,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 5, 
                              });

            Collection<ItemType> collection = new Collection<ItemType>();
            foreach (ItemType item in dictionary.Values)
            {
                collection.Add(item);
            }
            testCases.Add(new TestCaseInfo
                              {
                                  Target = collection,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 6, 
                              });

            // verify an array works as part of a collection
            List<ItemType[]> listOfArray = new List<ItemType[]>
            {
                new ItemType[] { new ItemType { ItemName = "Name", Price = 3.14 } },
                new ItemType[] {},
                null
            };
            testCases.Add(new TestCaseInfo
            {
                Target = listOfArray,
                ExpectedResult = true,
                TestID = instanceIDPrefix + 7,
            });

            return testCases;
        }

        #endregion
    }

    // arrays of primitives
    public class CollectionContainerType9
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            List<Array> arrays = new List<Array>
                                     {
                                         new bool[0],
                                         new bool[]
                                             {
                                                 true, false
                                             },
                                         new sbyte[0],
                                         new sbyte[]
                                             {
                                                 sbyte.MaxValue, sbyte.MinValue, 0, -1, 1
                                             },
                                         new short[0],
                                         new short[]
                                             {
                                                 short.MaxValue, short.MinValue, 0, -1, 1
                                             },
                                         new int[0],
                                         new int[]
                                             {
                                                 int.MaxValue, int.MinValue, 0, -1, 1
                                             },
                                         new long[0],
                                         new long[]
                                             {
                                                 long.MaxValue, long.MinValue, 0, -1, 1
                                             },
                                         new uint[0],
                                         new uint[]
                                             {
                                                 uint.MinValue, uint.MaxValue, 1
                                             },
                                         new ushort[0],
                                         new ushort[]
                                             {
                                                 ushort.MinValue, ushort.MaxValue, 1
                                             },
                                         new byte[0],
                                         new byte[]
                                             {
                                                 byte.MinValue, byte.MaxValue, 1
                                             },
                                         new ulong[0],
                                         new ulong[]
                                             {
                                                 ulong.MaxValue, ulong.MinValue, 1
                                             },
                                         new float[0],
                                         new float[]
                                             {
                                                 float.MinValue, float.MaxValue, float.NaN, float.NegativeInfinity, float.PositiveInfinity, float.Epsilon
                                             },
                                         new double[0],
                                         new double[]
                                             {
                                                 double.MinValue, double.MaxValue, double.Epsilon, double.NaN, double.NegativeInfinity, double.PositiveInfinity
                                             },
                                         new decimal[0],
                                         new decimal[]
                                             {
                                                 decimal.MaxValue, decimal.MinValue, decimal.MinusOne, decimal.One, decimal.Zero
                                             },
                                         new string[0],
                                         new string[]
                                             {
                                                 string.Empty, null, "some other string"
                                             },
                                         new DateTime[0],
                                         new DateTime[]
                                             {
                                                 DateTime.Now, 
                                                 DateTime.Today, 
                                                 CultureInfo.InvariantCulture.DateTimeFormat.Calendar.MaxSupportedDateTime,
                                                 CultureInfo.InvariantCulture.DateTimeFormat.Calendar.MinSupportedDateTime,
                                             },
                                         new char[0],
                                         new char[]
                                             {
                                                 'a', 'b', 'c'
                                             },
                                         new TimeSpan[0],
                                         new TimeSpan[]
                                             {
                                                 TimeSpan.MaxValue, TimeSpan.MinValue, TimeSpan.Zero, new TimeSpan(TimeSpan.TicksPerDay)
                                             },
                                         new Guid[0],
                                         new Guid[]
                                             {
                                                 Guid.Empty, Guid.NewGuid()
                                             },
                                         new object[0],
                                         new object[]
                                             {
                                                 new object(), "a string", 3, 3.14, Guid.NewGuid(), DateTime.Now, TimeSpan.TicksPerHour
                                             }
                                     };

            foreach (Array array in arrays)
            {
                TestCaseInfo testInfo = new TestCaseInfo
                                            {
                                                Target = array,
                                                ExpectedResult = true,
                                                TestID = instanceIDPrefix + array.GetType().GetElementType().Name + array.Length,
                                            };

                testCases.Add(testInfo);
            }

            return testCases;
        }

        #endregion
    }

    // dictionary with a value that has a type converter
    public class CollectionContainerType10
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            Dictionary<string, AnimalList> instance1 = new Dictionary<string, AnimalList>
                                                           {
                                                               {
                                                                   "Key1", new AnimalList
                                                                               {
                                                                                   new Animal
                                                                                       {
                                                                                           Name = "Monkey"
                                                                                       }, new Animal
                                                                                              {
                                                                                                  Name = "Emu"
                                                                                              }
                                                                               }
                                                                   },
                                                               {
                                                                   "Key2", new AnimalList()
                                                                   },
                                                               {
                                                                   "Key3", null
                                                                   }
                                                           };
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 1
                              });

            Dictionary<string, AnimalList> instance2 = new Dictionary<string, AnimalList>();

            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  ExpectedResult = true,
                                  TestID = instanceIDPrefix + 2
                              });

            return testCases;
        }

        #endregion
    }

    // case insensitive hashtable and dictionary
    public class CollectionContainerType11
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            Hashtable instance1 = new Hashtable(StringComparer.CurrentCultureIgnoreCase);
            Dictionary<string, Animal> instance2 = new Dictionary<string, Animal>(StringComparer.CurrentCultureIgnoreCase);

            instance1.Add("KEY1", new Animal
                                      {
                                          Name = "Monkey"
                                      });
            instance1.Add("key2", new Animal
                                      {
                                          Name = "Emu"
                                      });
            instance1.Add("Key3", new Animal
                                      {
                                          Name = "Bear"
                                      });

            instance2.Add("KEY1", new Animal
                                      {
                                          Name = "Monkey"
                                      });
            instance2.Add("key2", new Animal
                                      {
                                          Name = "Emu"
                                      });
            instance2.Add("Key3", new Animal
                                      {
                                          Name = "Bear"
                                      });

            return new List<TestCaseInfo>
                       {
                        // [DISABLED]
                        //    new TestCaseInfo
                        //        {
                        //            Target = instance1,
                        //            TestID = instanceIDPrefix + 1,
                        //        },
                        // [DISABLED]
                        //    new TestCaseInfo
                        //        {
                        //            Target = instance2,
                        //            TestID = instanceIDPrefix + 2,
                        //        }
                       };
        }

        #endregion
    }

    public class CollectionContainerType12
    {
        private IList<ItemType> _items = new List<ItemType>();
        public IList<ItemType> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            CollectionContainerType12 instance1 = new CollectionContainerType12();
            instance1.Items.Add(new ItemType
                                    {
                                        ItemName = "Item1",
                                        Price = 3.14
                                    });
            instance1.Items.Add(new ItemType
                                    {
                                        ItemName = "Item2",
                                        Price = 6.23
                                    });

            CollectionContainerType12 instance2 = new CollectionContainerType12();

            CollectionContainerType12 instance3 = new CollectionContainerType12();
            instance3.Items = null;

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = instance1, TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = instance2, TestID = instanceIDPrefix + 2
                               },
                           new TestCaseInfo
                               {
                                   Target = instance3, TestID = instanceIDPrefix + 3
                               },
                       };
        }

        #endregion
    }

    public class CollectionContainerType13
    {
        private IList<ItemType> _items;
        public IList<ItemType> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            CollectionContainerType13 instance1 = new CollectionContainerType13();
            instance1.Items = new List<ItemType>();
            instance1.Items.Add(new ItemType
                                    {
                                        ItemName = "Item1",
                                        Price = 3.14
                                    });
            instance1.Items.Add(new ItemType
                                    {
                                        ItemName = "Item2",
                                        Price = 6.23
                                    });

            CollectionContainerType13 instance2 = new CollectionContainerType13();

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = instance1, TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = instance2, TestID = instanceIDPrefix + 2
                               },
                       };
        }

        #endregion
    }

    public class CollectionContainerType14 : IList<ItemType>
    {
        private readonly IList<ItemType> _items = new List<ItemType>();

        #region IList<Item> Members

        public int IndexOf(ItemType item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, ItemType item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public ItemType this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }

        #endregion

        #region ICollection<Item> Members

        public void Add(ItemType item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(ItemType item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(ItemType[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _items.IsReadOnly;
            }
        }

        public bool Remove(ItemType item)
        {
            return _items.Remove(item);
        }

        #endregion

        #region IEnumerable<Item> Members

        public IEnumerator<ItemType> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

        #endregion

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new CollectionContainerType14(), TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new CollectionContainerType14
                                                {
                                                    new ItemType
                                                        {
                                                            ItemName = "Item1", Price = 3.14
                                                        },
                                                    new ItemType
                                                        {
                                                            ItemName = "Item2", Price = 6.28
                                                        },
                                                    null
                                                },
                                   TestID = instanceIDPrefix + 2,
                               }
                       };
        }

        #endregion
    }

    public class CollectionContainerType15 : Dictionary<string, ItemType>
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new CollectionContainerType15(), TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new CollectionContainerType15
                                                {
                                                    {
                                                        "Key1", new ItemType
                                                                    {
                                                                        ItemName = "Item1", Price = 3.14
                                                                    }
                                                        },
                                                    {
                                                        "Key2", new ItemType
                                                                    {
                                                                        ItemName = "Item2", Price = 6.28
                                                                    }
                                                        },
                                                    {
                                                        "Key3", null
                                                        }
                                                },
                                   TestID = instanceIDPrefix + 2
                               }
                       };
        }

        #endregion
    }

    public class CollectionContainerType16 : List<ItemType>
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new CollectionContainerType16(), TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new CollectionContainerType16
                                                {
                                                    new ItemType
                                                        {
                                                            ItemName = "Item1", Price = 3.14
                                                        },
                                                    new ItemType
                                                        {
                                                            ItemName = "Item2", Price = 6.28
                                                        },
                                                    null
                                                },
                                   TestID = instanceIDPrefix + 2
                               }
                       };
        }

        #endregion
    }

    public class CollectionContainerType17
    {
        private readonly List<ItemType> _items;
        public List<ItemType> Items
        {
            get
            {
                return _items;
            }
        }

        private readonly Dictionary<string, ItemType> _map;
        public Dictionary<string, ItemType> Map
        {
            get
            {
                return _map;
            }
        }

        public CollectionContainerType17(List<ItemType> iTeMs, Dictionary<string, ItemType> MaP)
        {
            _items = iTeMs;
            _map = MaP;
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            List<ItemType> items = new List<ItemType>
                                   {
                                       new ItemType
                                           {
                                               ItemName = "Item1", Price = 3.14
                                           },
                                       new ItemType
                                           {
                                               ItemName = null, Price = 6.28
                                           }
                                   };

            Dictionary<string, ItemType> map = new Dictionary<string, ItemType>
                                               {
                                                   {
                                                       "Key1", new ItemType
                                                                   {
                                                                       ItemName = "Item1", Price = 3.14
                                                                   }
                                                       },
                                                   {
                                                       "Key2", null
                                                       }
                                               };

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new CollectionContainerType17(items, map),
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedResult = false,
                                   ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoDefaultConstructor", WpfBinaries.SystemXaml),
                               },
                       };
        }

        #endregion
    }

    public class CollectionContainerType18
    {
        private readonly List<ItemType> _items = new List<ItemType>();
        public IList Items
        {
            get
            {
                return ArrayList.ReadOnly(_items);
            }
        }

        public void AddItem(ItemType item)
        {
            _items.Add(item);
        }
    }

    public class CollectionContainerType19
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            ItemType item = new ItemType
                            {
                                ItemName = "item",
                                Price = double.MaxValue
                            };

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "BitArray",
                                   Target = new BitArray(new bool[]
                                                             {
                                                                 true, false, false, true, false
                                                             }),
                                   ExpectedResult = false,
                                   ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoDefaultConstructor", WpfBinaries.SystemXaml),
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "SortedList",
                                   Target = new SortedList
                                                {
                                                    {
                                                        "item", item
                                                        }
                                                },
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "HashSetT",
                                   Target = new HashSet<ItemType>
                                                {
                                                    item
                                                },
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "LinkedList",
                                   Target = new LinkedList<ItemType>(new ItemType[]
                                                                     {
                                                                         item
                                                                     }),
                               },
                            // [DISABLED]
                        //    new TestCaseInfo
                        //        {
                        //            TestID = instanceIDPrefix + "QueueT",
                        //            Target = new Queue<ItemType>(new ItemType[]
                        //                                         {
                        //                                             item
                        //                                         }),
                        //        },
                            // [DISABLED]
                        //    new TestCaseInfo
                        //        {
                        //            TestID = instanceIDPrefix + "StackT",
                        //            Target = new Stack<ItemType>(new ItemType[]
                        //                                         {
                        //                                             item
                        //                                         }),
                        //        },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "SortedDictionary",
                                   Target = new SortedDictionary<string, ItemType>
                                                {
                                                    {
                                                        "item", item
                                                        }
                                                },
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "SortedListT",
                                   Target = new SortedList<string, ItemType>
                                                {
                                                    {
                                                        "item", item
                                                        }
                                                },
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "CollectionT",
                                   Target = new Collection<ItemType>
                                                {
                                                    item
                                                },
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "ObservableCollection",
                                   Target = new ObservableCollection<ItemType>
                                                {
                                                    item
                                                },
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "HybridDictionary",
                                   Target = new HybridDictionary
                                                {
                                                    {
                                                        "item", item
                                                        }
                                                },
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "ListDictionary",
                                   Target = new ListDictionary
                                                {
                                                    {
                                                        "item", item
                                                        }
                                                },
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "OrderedDictionary",
                                   Target = new OrderedDictionary
                                                {
                                                    {
                                                        "item", item
                                                        }
                                                },
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "StringCollection",
                                   Target = new StringCollection
                                                {
                                                    "some value"
                                                }
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + "StringDictionary",
                                   Target = new StringDictionary
                                                {
                                                    {
                                                        "key", "value"
                                                        }
                                                },
                               }
                       };
        }

        #endregion
    }

    public class CollectionContainerType20 : ICollection<ItemType>
    {
        private Collection<ItemType> _collection = new Collection<ItemType>();
        #region ICollection<Item> Members

        public void Add(ItemType item)
        {
            _collection.Add(item);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(ItemType item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(ItemType[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ItemType item)
        {
            return _collection.Remove(item);
        }

        #endregion

        #region IEnumerable<Item> Members

        public IEnumerator<ItemType> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            ItemType item = new ItemType
            {
                ItemName = "Some item",
                Price = double.MaxValue
            };

            return new List<TestCaseInfo>
                   {
                       new TestCaseInfo
                       {
                           Target = new CollectionContainerType20()
                                    {
                                    },
                           TestID = instanceIDPrefix + 1,
                       },
                       new TestCaseInfo
                       {
                           Target = new CollectionContainerType20()
                                    {
                                        new ItemType { ItemName = "Item1", Price = 3.14 },
                                    },
                           TestID = instanceIDPrefix + 2,
                       },
                       new TestCaseInfo
                       {
                           Target = new CollectionContainerType20()
                                    {
                                        item,
                                        new ItemType { ItemName = "Item2", Price = double.MinValue },
                                        item
                                    },
                           TestID = instanceIDPrefix + 3,
                       },

                       new TestCaseInfo
                       {
                           Target = new CollectionContainerType20()
                                    {
                                        new ItemType { ItemName = "Item1", Price = 3.14 },
                                        null,
                                        new ItemType { ItemName = "Item2", Price = double.MinValue },
                                    },
                           TestID = instanceIDPrefix + 4,
                       },
                   };
        }

        #endregion
    }

    // collection with protected setters
    public class CollectionContainerType21
    {
        private ArrayList _arrayList = new ArrayList();
        private Hashtable _hashTable = new Hashtable();
        private Dictionary<string, ItemType> _dictionary = new Dictionary<string, ItemType>();

        public CollectionContainerType21()
        {
        }

        public ArrayList ArrayList
        {
            get
            {
                return _arrayList;
            }
            private set
            {
                _arrayList = value;
            }
        }

        public Hashtable HashTable
        {
            get
            {
                return _hashTable;
            }
            protected set
            {
                _hashTable = value;
            }
        }

        public Dictionary<string, ItemType> Dictionary
        {
            get
            {
                return this._dictionary;
            }
            internal set
            {
                _dictionary = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // basic sanity
            var instance1 = new CollectionContainerType21();
            instance1.ArrayList.Add(new ItemType("Name1", 12));
            instance1.HashTable.Add("Name4", "aaaaaa");
            instance1.Dictionary.Add("aaa", new ItemType("Name3", 14.11));
            testCases.Add(new TestCaseInfo
            {
                Target = instance1,
                TestID = instanceIDPrefix + 1
            });

            // empty collections
            // 
            var instance2 = new CollectionContainerType21();
            testCases.Add(new TestCaseInfo
            {
                Target = instance2,
                TestID = instanceIDPrefix + 2
            });

            // null in collections
            // 9060: general null roundtripping failure
            var instance3 = new CollectionContainerType21();
            instance3.HashTable.Add("Name2", null);
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.ArrayList.Add(null);
            instance3.Dictionary.Add("aaa", null);
            instance3.Dictionary.Add("bbb", new ItemType
            {
                ItemName = null,
                Price = -123
            });
            testCases.Add(new TestCaseInfo
            {
                Target = instance3,
                TestID = instanceIDPrefix + 3
            });

            return testCases;
        }

        #endregion
    }

    // test x:Type is serialized instead of RuntimeType
    public class CollectionContainerType22
    {
        Collection<Type> _data = new Collection<Type>();
        public Collection<Type> Data { get { return _data; } }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new CollectionContainerType22(), TestID = instanceIDPrefix + 1
                               },

                        // [DISABLED] Known Failure / Test Case Hang ?
                        //    new TestCaseInfo
                        //        {
                        //            Target = new CollectionContainerType22
                        //                         {
                        //                             Data = 
                        //                             {
                        //                                 typeof(string),
                        //                                 typeof(int),
                        //                                 typeof(CollectionContainerType22),
                        //                                 null
                        //                             }
                        //                         },
                        //            TestID = instanceIDPrefix + 2,
                        //            XPathExpresions = 
                        //             {
                        //                 "/mtxt:CollectionContainerType22/mtxt:CollectionContainerType22.Data/x:Type[@Type='x:String']",
                        //                 "/mtxt:CollectionContainerType22/mtxt:CollectionContainerType22.Data/x:Type[@Type='x:Int32']",
                        //                 "/mtxt:CollectionContainerType22/mtxt:CollectionContainerType22.Data/x:Type[@Type='CollectionContainerType22']",
                        //             },
                        //        },
                       };
        }

        #endregion
    }

    // verify x:Reference composes with dictionaries
    public class CollectionContainerType23
    {
        public Point Stuff { get; set; }

        IDictionary<string, Point> _container = new Dictionary<string, Point>();
        public IDictionary<string, Point> Container { get { return _container; } }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            Point p = new Point { X = 3, Y = 42 };

            CollectionContainerType23 target1 = new CollectionContainerType23
            {
                Stuff = p,
                Container =
                {
                    { "blah", p },
                }
            };

            CollectionContainerType23 target2 = new CollectionContainerType23
            {
                Stuff = p,
                Container =
                {
                    { "blah", p },
                    { "foo", p },
                }
            };

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = target1, TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = target2.Container,
                                   TestID = instanceIDPrefix + 2,
                                   
                               },
                       };
        }

        #endregion
    }

    // repro for bugs 619377 and 681904
    // must implement IDictionary<K,V> but not IDictionary
    // then add derived types to it
    public class CollectionContainerType24<K, V> : IDictionary<K, V>
    {
        Dictionary<K, V> _dictionary = new Dictionary<K, V>();

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            var target = new CollectionContainerType24<object, object>
            {
                { "blah", "blah" }
            };

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = target, TestID = instanceIDPrefix + 1
                               },
                       };
        }

        #endregion

        #region IDictionary implementation
        public void Add(K key, V value)
        {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(K key)
        {
            return _dictionary.ContainsKey(key);
        }

        public ICollection<K> Keys
        {
            get { return _dictionary.Keys; }
        }

        public bool Remove(K key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(K key, out V value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<V> Values
        {
            get { return _dictionary.Values; }
        }

        public V this[K key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                _dictionary[key] = value;
            }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return _dictionary.ContainsKey(item.Key) && _dictionary.ContainsValue(item.Value);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
        #endregion
    }

    // collection that isn't a dictionary but has an add that looks a dictionary add 
    // should be treated like a collection
    public class CollectionContainerType25 : List<string>
    {
        public void Add(string key, object value) { }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            var target = new CollectionContainerType24<object, object>
            {
                { "blah", "blah" }
            };

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new CollectionContainerType25
                                   {
                                   },
                                   TestID = instanceIDPrefix + 1
                               },
                            new TestCaseInfo
                               {
                                   Target = new CollectionContainerType25
                                   {
                                       "foo",
                                       "bar",
                                       "baz",
                                       string.Empty,
                                       null
                                   },
                                   TestID = instanceIDPrefix + 2
                               },
                       };
        }

        #endregion
    }

    public class GenericDictionary<K, V>
    {
        public IDictionary IDictionary { get; set; }

        public GenericDictionary()
        {
            IDictionary = new Dictionary<K, V>();
        }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            string xaml = new NodeList("Key_IDictionary_TC")
            {
                new NamespaceNode("x2", XamlLanguage.Xaml2006Namespace),
                new StartObject(typeof(GenericDictionary<int, string>)),
                    new StartMember(typeof(GenericDictionary<int, string>), "IDictionary"),
                        new StartObject(XamlLanguage.String),
                            new StartMember(XamlLanguage.Key),
                                new ValueNode('1'),
                            new EndMember(),
                            new StartMember(XamlLanguage.Initialization),
                                new ValueNode(2),
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            }.NodeListToXaml();
            
            return new List<TestCaseInfo>
                       {
                           new XamlFirstTestCaseInfo
                               {
                                   // Regression test
                                   Target = xaml,
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedResult = false,
                                   ExpectedMessage = Exceptions.GetMessage("AddDictionary", WpfBinaries.SystemXaml),
                               },
                       };
        }
    }


}
