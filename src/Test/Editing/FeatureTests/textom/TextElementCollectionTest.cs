// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Testing TOM.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Markup;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using System.Windows.Threading;

    #endregion Namespaces.

    /// <summary>Helper test class for TextElementCollection</summary>
    public abstract class TextElementCollectionTest<T> : CustomTestCase
        where T : TextElement
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {            
            _wrapper = new UIElementWrapper(new RichTextBox());
            _panel = new StackPanel();

            _panel.Children.Add(_wrapper.Element);
            MainWindow.Content = _panel;

            BasicContent = typeof(T).Name + " ";

            // Remove the first default block from the document.
            RichTextBoxContainer.Document.Blocks.Remove(RichTextBoxContainer.Document.Blocks.FirstBlock); 

            CreateAndHookUpParent();
            System.Diagnostics.Debug.Assert(this.Parent != null);
            System.Diagnostics.Debug.Assert(this.Collection != null);

            AddChildren();
           QueueDelegate(VerifyAPIsInBaseClass);
           QueueDelegate(VerifyInheritedClass);

        }
        /// <summary>Verify the test scenarios, need to be override by the inherited class</summary>
        protected virtual void VerifyInheritedClass()
        {
            Logger.Current.ReportSuccess(); 
        }

        /// <summary>
        /// We want to test each API one by one in Detail
        /// This following ReadOnly property will be used to verify the other APIs. no individual input test is needed.
        ///         Count
        ///         IsReadOnly
        ///         IList.IsFixedSize
        ///         IList.IsReadOnly
        ///         ICollection.Count
        ///         ICollection.IsSynchronized
        ///         ICollection.SyncRoot
        /// </summary>
        protected void VerifyAPIsInBaseClass()
        {
            //Add(TextElementType item)
            InputTestAdd();
            
            //void Clear()
            InputTestClear(); 

            //Contains(TextElementType item)
            InputTestContains(); 

            //CopyTo(TextElementType[] array, int arrayIndex)
            InputTestCopyTo();
            
            //Remove(TextElementType item)
            InputTestRemove();

            //InsertAfter(TextElementType previousSibling, TextElementType newItem)
            InputTestInsertAfter();
            
            //InsertBefore(TextElementType nextSibling, TextElementType newItem)
            InputTestInsertBefore();

            //AddRange(IEnumerable range)
            InputTestAddRange();
            
            //IList.Clear()
            InputTestIList_Clear();
            
            //IList.Add(object value)
            InputTestIList_Add();
            
            //IList.Contains(object value)
            InputTestIList_Contains();
            
            //IList.IndexOf(object value)
            InputTestIList_IndexOf();
            
            //IList.Insert(int index, object value)
            InputTestIList_Insert();

            //IList.Remove(object value)
            
            InputTestIList_Remove();
            //IList.RemoveAt(int index)
            
            InputTestIlist_RemoveAt();
            
            //IList.this[int index]
            InputTestIList_This();
            
            //ICollection.CopyTo(Array array, int arrayIndex)
            InputTest_ICollection_CopyTo();

            VerifyITextElementParent();

            VerifyOtherProperties();
        }
        #endregion Main flow.

        #region Verfication
        /// <summary>Input test for the TextElementCollection.Add() method</summary>
        void InputTestAdd()
        {
            Log("Verifying TextElementCollection.Add() ...");
            try
            {
                Collection.Add(null);
                throw new Exception("TextElementCollection.Add() won't accepts null values!");
            }
            catch(ArgumentNullException e)
            {
                VerifyException(e,_className,  "Add");
            }
            //Make sure that the collection is emtpy.
            Collection.Clear();
            Verifier.Verify(Collection.Count == 0, 
                "After Clear() is called, Count should be[0], Actual[" + Collection.Count + "]");
            
            Collection.Add(CreateInstance());
            Verifier.Verify(Collection.Count == 1, 
                "After add() is called, Count should be[1], Actual[" + Collection.Count + "]");

            AddChildren();
            Verifier.Verify(Collection.Count == 1 + ChildLimit,
                "After AddChildren() is called, Count should be[" + 1 + ChildLimit + "], Actual[" + Collection.Count + "]");
        }

        /// <summary>Input Test for TextElementCollection.Clear() method</summary>
        void InputTestClear()
        {
            //Make sure that the collection is emtpy.
            Log("Verifying TextElementCollection.Clear() ...");
            Collection.Clear();
            Verifier.Verify(Collection.Count == 0, 
                "After Clear() is called, Count should be[0], Actual[" + Collection.Count + "]");
           
            //Clear when the Collection is empty, no side effect is expected.
            Collection.Clear();
            Verifier.Verify(Collection.Count == 0,
              "After Clear() is called, Count should be[0], Actual[" + Collection.Count + "]");
            if (typeof(T) == typeof(Inline))
            {
                Verifier.Verify(_wrapper.Text == "\r\n", "Expected newline in the control, Actual[" + _wrapper.Text + "]");
            }
            else
            {
                Verifier.Verify(_wrapper.Text == "", "Expected no content in the control, Actual[" + _wrapper.Text + "]");
            }

            //add to an empty collection
            Collection.Add(CreateInstance());
            Verifier.Verify(Occurency(_wrapper.Text, BasicContent) == 1,
                "Expected [1] copy of " +  BasicContent + ", Actual[" + Occurency(_wrapper.Text, BasicContent) + "] of " + BasicContent);

            //Make sure that the collection is emtpy.
            Collection.Clear();
            Verifier.Verify(Collection.Count == 0,
                "After Clear() is called, Count should be[0], Actual[" + Collection.Count + "]");
            
            //Make sure that the textcontainer is cleared
            if (typeof(T) == typeof(Inline))
            {
                Verifier.Verify(_wrapper.Text == "\r\n", "Expected newline in the control, Actual[" + _wrapper.Text + "]");
            }
            else
            {
                Verifier.Verify(_wrapper.Text == "", "Expected no content in the control, Actual[" + _wrapper.Text + "]");
            }
        }

        /// <summary>Input Test for TextElementCollection.Contains() method</summary>
        void InputTestContains()
        {
            Log("Verifying TextElementCollection.Contains() ...");
            AddChildren();
          
            try
            {
                #pragma warning disable 0458
                Collection.Contains(null as T);
                #pragma warning restore 0458
                throw new Exception("TextElementCollection.Contains() won't accepts null values!");
            }
            catch (Exception e)
            {
                VerifyException(e, "TextElementCollectionTest", "InputTestContains");
            }

            for (int i = 0; i < Collection.Count; i++)
            {
                Verifier.Verify(Collection.Contains(((IList)Collection)[i] as T), 
                    "Contanins() should return true when passing one of its item in!");
            }
            Verifier.Verify(!Collection.Contains(CreateInstance()), 
                "Contains() shoul return false when passing any un-added item in");

            //Test when the collection is empty.
            Collection.Clear();
            Verifier.Verify(!Collection.Contains(CreateInstance()),
                "Contains() should return false when passing any un-added item in"); 
        }

        /// <summary>Input Test for TextElementCollection.CopyTo() method</summary>
        void InputTestCopyTo()
        {
            Log("Verifying TextElementCollection.CopyTo() ...");
            T[] tarray =null;

            try
            {
                Collection.CopyTo(tarray, 0);
                throw new Exception("TextElementCollection.CopyTo won't accepts null value!");
            }
            catch(ArgumentNullException e)
            {
                VerifyException(e, _className, "CopyTo");
            }
            
            //invalid, Array is not big enough.
            tarray = new T[ChildLimit - 1];
            Collection.Clear();
            AddChildren();
            try
            {
                Collection.CopyTo(tarray, 0);
                throw new Exception("TextElementCollection.CopyTo should throw if the supplied array is not large enough!");
            }
            catch (ArgumentException e)
            {
                VerifyException(e,_className, "CopyTo");
            }

            //copy the items to an array. Note our array can hold 20 values, and the collection has 10 value
            //we will copy the collection to any 10 consecutive slots.
            for(int i =0; i< 20 - ChildLimit; i++)
            {
                tarray = new T[20];
                Collection.CopyTo(tarray, i);
                for (int j = 0; j < 20; j++)
                {
                    if (j < i)
                    {
                        Verifier.Verify(tarray[j] == null, "Should not damiage the slot before the arrayIndex!");
                    }
                    else if (j >= i && j < i + ChildLimit)
                    {
                        Verifier.Verify(tarray[j] is T, "coppied items should be type of" + typeof(T).ToString() + "!");
                    }
                    else
                    {
                        Verifier.Verify(tarray[j] == null, "Should not damiage the slot after the coppied items!");
                    }
                }
            }
        }

        /// <summary>Input Test for TextElement.Remove() method</summary>
        void InputTestRemove()
        {
            Log("Verifying TextElementCollection.Remove() ...");
            bool result; 
            
            //Invalid values.  Gets compiler warning in Part1, so suppress...
            #pragma warning disable 0458
            Collection.Remove(null as T);
            #pragma warning restore 0458
            
            Collection.Remove(CreateInstance());
            
            //initialize values in the collection.
            Collection.Clear();
            AddChildren();
            
            T item = ((IList)Collection)[3] as T; 
            
            //remove the Middle one
            result = Collection.Remove(item);
            Verifier.Verify(Collection.Count == ChildLimit - 1,
                "After a is removed, child count sould be[" + (ChildLimit - 1).ToString() + "], Actual[" + Collection.Count + "]");
            Verifier.Verify(!Collection.Contains(item), "After a item is removed, the Contains() method should retrun false!");

            Verifier.Verify(result, "A succesfully remove will return true!");
            
            //Remove the last one.
            item = ((IList)Collection)[Collection.Count -1 ] as T;
            result = Collection.Remove(item);
            Verifier.Verify(Collection.Count == ChildLimit - 2,
                "After the last one is removed, child count sould be[" + (ChildLimit - 2).ToString() + "], Actual[" + Collection.Count + "]");
            Verifier.Verify(!Collection.Contains(item), "After a item is removed, the Contains() method should retrun false!");
            Verifier.Verify(result, "A succesfully remove will return true!");
            
            for (int i = 0; i < Collection.Count; )
            {
                item = ((IList)Collection)[0] as T;
                Verifier.Verify(Collection.Contains(item), "The Item should be in the collection before it is moved!");
                result = Collection.Remove(item);
                Verifier.Verify(!Collection.Contains(item), "After a item is removed, the Contains() method should retrun false!");
                Verifier.Verify(result, "A succesfully remove will return true!");
            }
            Verifier.Verify(!Collection.Remove(null), "Remove null should return false!");
            AddChildren();
            item = ((IList)Collection)[3] as T;
            Collection.Remove(item);
            Verifier.Verify(!Collection.Remove(item), "Remove uncontained object return false!");
        }

        /// <summary>Input Test for TextElement.InsertAfter() method</summary>
        void InputTestInsertAfter()
        {

            T item = CreateInstance();  
            Collection.Clear();
            AddChildren();
            object[] objs = new object[3];
            objs[0] = null;
            objs[1] = ((IList)Collection)[0] as T;
            objs[2] = item;

            Log("Verifying TextElementCollection.InsertAfter() ...");

            //Invalid input test
            for (int m = 0; m < 2; m++)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    for (int j = 0; j < objs.Length; j++)
                    {
                        try
                        {
                            if (m == 0)
                            {
                                Collection.InsertAfter(objs[i] as T, objs[j] as T);
                            }
                            else
                            {
                                Collection.InsertBefore(objs[i] as T, objs[j] as T);
                            }

                            throw new Exception("Invalid value(s) should not be accepted!");
                        }
                        catch (Exception e)
                        {
                            if (e.Message == "Invalid value(s) should not be accepted!")
                            {
                                //if i== 1 && j == 1 is a valid scenario
                                if (!(i == 1 && j == 2))
                                    Verifier.Verify(false, "Invalid value(s) should not be accepted!");
                            }
                            else
                            {
                                if (m == 0)
                                {
                                    VerifyException(e, _className, "InsertAfter");
                                }
                                else
                                {
                                    VerifyException(e,_className, "InsertBefore");
                                }
                            }
                        }
                    }
                }
            }
            
            //valid input test
            Collection.Remove(item);
            for (int i = 0; i < Collection.Count; i++)
            {

                Collection.InsertAfter(((IList)Collection)[i] as T, item);
                Verifier.Verify(((IList)Collection)[i + 1] == item, "Item is not insert to the right location!");
                ((RichTextBox)_wrapper.Element).Undo();
            }
        }

        /// <summary>Input Test for TextElement.InsertBefore() method</summary>
        void InputTestInsertBefore()
        {
            //Invalid test is done in InputTestInsertAfter() method
            T item = CreateInstance();
            Collection.Clear();
            AddChildren();

            Log("Verifying TextElementCollection.InsertBefor() ...");
            for (int i = 0; i < Collection.Count; i++)
            {

                Collection.InsertBefore(((IList)Collection)[i] as T, item);
                Verifier.Verify(((IList)Collection)[i] == item, "Item is not insert to the right location!");
                ((RichTextBox)_wrapper.Element).Undo();
            }
        }

        /// <summary>Input Test for TextElement.AddRange() method</summary>
        void InputTestAddRange()
        {
            TestRange range = new TestRange(CreateInstance());
            TestRange rangenull = new TestRange(CreateInstance());
            rangenull.SetNullEnumerator();
            ArrayList inputList = new ArrayList();
            inputList.Add(null);
            inputList.Add(rangenull);

            Log("Verifying TextElementCollection.AddRange() ...");
           
            //Invalid values
            for (int i = 0; i < inputList.Count; i++)
            {
                try
                {
                    Collection.AddRange(inputList[i] as IEnumerable);
                    throw new Exception("TextElementCollection.AddRange won't accept invalid value!");
                }
                catch (Exception e)
                {
                    VerifyException(e,_className, "AddRange");
                }
            }
            //Add null value in a IENumerable
            range.Add(null);
            try
            {
                Collection.AddRange(range  as IEnumerable);
                throw new Exception("TextElementCollection.AddRange won't accept invalid value!");
            }
            catch (Exception e)
            {
                VerifyException(e,_className, "AddRange");
            }

            //Valid values
            range.clear();
            range.Add(CreateInstance());
            range.Add(CreateInstance());
            Collection.Clear();
            Collection.AddRange(range as IEnumerable);
            Verifier.Verify(Collection.Count == 2, "Failed to add a valid item form an IEnumerable!");
        }

        /// <summary>Input Test for ILIst.Clear() method</summary>
        void InputTestIList_Clear()
        {
            IList list = Collection as IList;
            Log("Verifying IList.Clear() ...");
            //Make sure that the collection is emtpy.
            list.Clear();
            Verifier.Verify(list.Count == 0,
                "After Clear() is called, Count should be[0], Actual[" + list.Count + "]");
            
            Verifier.Verify(list.Count == ((ICollection)Collection).Count, "IList.Count does not equal TextElementCollection.Count!");

            //Clear when the Collection is empty, no side effect is expected.
            list.Clear();
            Verifier.Verify(list.Count == 0,
              "After Clear() is called, Count should be[0], Actual[" + Collection.Count + "]");
            if (typeof(T) == typeof(Inline))
            {
                Verifier.Verify(_wrapper.Text == "\r\n", "Expected a newline in the control, Actual[" + _wrapper.Text + "]");
            }
            else
            {
                Verifier.Verify(_wrapper.Text == "", "Expected no content in the control, Actual[" + _wrapper.Text + "]");
            }

            Verifier.Verify(list.Count == Collection.Count, "IList.Count does not equal TextElementCollection.Count!");
           
            //add to an empty collection
            list.Add(CreateInstance());
            Verifier.Verify(Occurency(_wrapper.Text, BasicContent) == 1,
                "Expected [1] copy of " + BasicContent + ", Actual[" + Occurency(_wrapper.Text, BasicContent) + "] of " + BasicContent);
            Verifier.Verify(list.Count == Collection.Count, "IList.Count does not equal TextElementCollection.Count!");
           
            //Make sure that the collection is emtpy.
            list.Clear();
            Verifier.Verify(list.Count == 0,
                "After Clear() is called, Count should be[0], Actual[" + list.Count + "]");
            Verifier.Verify(list.Count == Collection.Count, "IList.Count does not equal TextElementCollection.Count!");

            //Make sure that the textcontainer is cleared
            if (typeof(T) == typeof(Inline))
            {
                Verifier.Verify(_wrapper.Text == "\r\n", "Expected a newline in the control, Actual[" + _wrapper.Text + "]");
            }
            else
            {
                Verifier.Verify(_wrapper.Text == "", "Expected no content in the control, Actual[" + _wrapper.Text + "]");
            }

        }

        /// <summary>Input Test for ILIst.Add() method</summary>
        void InputTestIList_Add()
        {
            IList list = Collection as IList;
            
            Log("Verifying IList.Add() ...");
            //Null value expected a ArgumentNullException
            try
            {
                list.Add(null);
                throw new Exception("IList.Add won't accepted null value!");
            }
            catch(ArgumentNullException e)
            {
                VerifyException(e, _className, "OnAdd");              
            }
            try
            {
                list.Add(12345 as object);
                throw new Exception("IList.Add won't accepted invalid object value!");
            }
            catch (Exception e)
            {
                VerifyException(e, _className, "OnAdd");
            }
            list.Clear();
            list.Add(CreateInstance());
            Verifier.Verify(list.Count == 1, 
                "Failed, after an items is added, Ilist.Count should be[1], Actual[" + list.Count + "]");

        }

        /// <summary>Input Test for ILIst.Contains() method</summary>
        void InputTestIList_Contains()
        {
            object[] invalidObjs = new object[2];
            invalidObjs[0] = null;
            invalidObjs[1] = "abc"; //string 
            IList list = Collection as IList;
            T item;
            Log("Verifying IList.Contains() ...");
            //Invalid values
            for (int i = 0; i < invalidObjs.Length; i++)
            {

                   Verifier.Verify(!list.Contains(invalidObjs[i]), "Failed, Invalid input should return false!") ;
 
            }
            //valid values
            for (int i = 0; i < Collection.Count; i++)
            {
                Verifier.Verify(list.Contains(list[i] ),
                    "Contanins() should return true when passing one of its item in!");
            }
            AddChildren();
            item = list[0] as T; 
            Verifier.Verify(!list.Contains(CreateInstance()),
                "IList.Contains() shoul return false when passing any un-added item in");

            //Test when the collection is empty.
            list.Clear();
            Verifier.Verify(!list.Contains(item),
                "ILiset.Contains() shoul return false when passing any un-added item in");
        }

        /// <summary>Input Test for ILIst.IndexOf() method</summary>
        void InputTestIList_IndexOf()
        {
            object[] invalidObjs = new object[2];
            invalidObjs[0] = null;
            invalidObjs[1] = "abc"; //string 
            IList list = Collection as IList;
            T item;
            int index;

            Log("Verifying IList.IndexOf() ...");

            //Invalid Input Test
            for (int i = 0; i < invalidObjs.Length; i++)
            {
                Verifier.Verify(-1 == list.IndexOf(invalidObjs[i]), "Failed: IndexOf() should return -1 for invalid value!" );                
            }

            AddChildren();
            
            //valid index.
            for (int j = 0; j < list.Count; j++)
            {
                item = list[j] as T;
                index = list.IndexOf(item);

                Verifier.Verify(index == j, 
                    "Index return from IndexOf is incorect! expected[" + j.ToString() + "], Actual[" + index.ToString() + "]");
            }

            //Item is not in the collection.
            index = list.IndexOf(CreateInstance());
            Verifier.Verify(index == -1,
                   "Index return from IndexOf is incorect! expected[-1], Actual[" + index.ToString() + "]");
        }

        /// <summary>Input Test for ILIst.Insert() method</summary>
        void InputTestIList_Insert()
        {
            object[] para1 = new object[3];
            object[] para2 = new object[3];
            para1[0] = -1; 
            para1[1] = 5; 
            para1[2] = 10000;
            para2[0] = null;
            para2[1] = CreateInstance();
            IList list = Collection as IList;
            AddChildren();
            para2[2] = list[3];

            T item = CreateInstance();
            int index;

            Log("Verifying IList.Insert() ...");
            
            //Invalid input test
            for (int i = 0; i < para1.Length; i++)
            {
                for (int j = 0; j < para2.Length; j++)
                {
                    try
                    {
                        if (!(i == 1 && j == 1))
                        {
                            //Regression_Bug586, When this bug is fixed, we will remove the if statement.
                            if(!(i==1 && j==2))
                            {
                                list.Insert((int)para1[i], (object)para2[j]);
                                throw new Exception("Invalid values should not be accepted!");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        VerifyException(e,_className, "IList.Insert");
                    }
                }
            }
            
            //Valid input test
            for (int k = 0; k < Collection.Count; k++)
            {
                list.Insert(k, item);
                index = list.IndexOf(item);
                Verifier.Verify(k == index, 
                    "Item is inderted in the wrong location! expected[" + k.ToString() + "], Actual[" + index.ToString() + "]");
                ((RichTextBox)_wrapper.Element).Undo();
            }
        }

        /// <summary>Input Test for ILIst.Remove() method</summary>
        void InputTestIList_Remove()
        {
            object[] para1 = new object[2];
            para1[0] = null;
            para1[1] = "abc";
            IList list = Collection as IList;
            T item;

            Log("Verifying IList.Remove() ...");
            
            //Invalid input test. Nothing should hypen.
            for (int i = 0; i < para1.Length; i++)
            {              
                list.Remove(para1[i]);
            }
            
            //Valid input test
            for (int j = 0; j < Collection.Count; j++)
            {
                item = ((IList)Collection)[j] as T;
                Verifier.Verify(Collection.Contains(item), "The Item should be in the collection before it is moved!");
                list.Remove(item);
                Verifier.Verify(!Collection.Contains(item), "After a item is removed, the Contains() method should retrun false!");
                ((RichTextBox)_wrapper.Element).Undo();
            }
        }
        /// <summary>Input Test for ILIst.RemoveAt() method</summary>
        void InputTestIlist_RemoveAt()
        {
            object[] para1;
            IList list;
            T item; 
            para1 = new object[3];
            para1[0] = -1;
            para1[1] = 1000;
            //when the colleciton is empty, remove at index 0 is invalid.
            para1[2] = 0; 
            list = Collection as IList; 
            Collection.Clear();

            Log("Verifying IList.RemoveAt() ...");
            
            //Invalid input test
            for (int i = 0; i < para1.Length; i++)
            {
                try
                {
                    list.RemoveAt((int)para1[i]);
                    throw new Exception("Invalid parameter should not be accepted!");
                }
                catch (Exception e)
                {
                    VerifyException(e, _className, "RemoveAtInternal");
                }
            }

            AddChildren();

            //Valid input test
            for (int j = 0; j < Collection.Count; j++)
            {
                item = list[j] as T;
                list.RemoveAt(j);
                Verifier.Verify(!list.Contains(item), 
                    "When an item is removed at an index, it should not be in the collection!");
                ((RichTextBox)_wrapper.Element).Undo();
            }
        }

        /// <summary>Input Test for ILIst.This() method</summary>
        void InputTestIList_This()
        {
            object[] para1;
            IList list;
            ArrayList aList; 
            T item; 
            
            para1 = new object[3];
            para1[0] = -1;
            para1[1] = 1000;
            //when the collection is empty. 0 is an invalid value too.
            para1[2] = 0;
            list = Collection as IList;
            aList = new ArrayList();

            Log("Verifying IList.This() ...");

            Collection.Clear();

            //Test the invalid values.
            for (int m = 0; m < 2; m++)
            {
                for (int i = 0; i < para1.Length; i++)
                {
                    try
                    {
                        if (m == 0)
                        {
                           object o = list[(int)para1[i]];
                        }
                        else
                        {
                            list[(int)para1[i]] = CreateInstance();
                        }
                        throw new Exception("Invalid parameter should not be accepted!");
                    }
                    catch (Exception e)
                    {
                        if (m == 0)
                        {
                            VerifyException(e, _className, "IList.get_Item");
                        }
                        else
                        {
                            VerifyException(e, _className, "RemoveAtInternal");
                        }
                    }
                }
            }

            //Regression_Bug587 coverage
            try
            {
                list[0] = null;
                throw new Exception("NULL parameter should not be accepted!");
            }
            catch (Exception e)
            {
                VerifyException(e, _className, "IList.set_Item");

            }
            
            //illegal type.
            try
            {
                list[0] = new Button();
                throw new Exception("Invalid parameter type should not be accepted!");
            }
            catch (Exception e)
            {
                VerifyException(e,_className, "IList.set_Item");
            }

            //set up values.
            for(int j = 0; j<ChildLimit;  j++ )
            {
                item = CreateInstance();
                aList.Add(item);

                //Regression_Bug588 is by design. list[index] is to replace the existing item with new item at index.
                //the index must be valid.
                //Pre set a item for replacement.
                list.Add(CreateInstance());
                list[j] = item;
            }

            //Get the Get property. the index shold match
            for (int k = 0; k < ChildLimit; k++)
            {
                Verifier.Verify((T)aList[k] == (T)list[k], "The order of the collection is incorrect!");
            }

            item = CreateInstance();
            
            for (int m = 0; m < Collection.Count - 1; m++)
            {
                list[m] = item;
                Verifier.Verify(list[m] == item, "Item inserted at a location can't be retrieved using the same index!");
                list.RemoveAt(m);
            }

            try
            {
                list[list.Count] = CreateInstance();
                throw new Exception("Should get exception shen insert item at end!");
            }
            catch(Exception ex)
            {
                VerifyException(ex, _className, "RemoveAtInternal");
            }
        }

        /// <summary>Input Test for ICollection.CopyTo() method</summary>
        void InputTest_ICollection_CopyTo()
        {

            Array tarray = null;
            int size = 20;
            ICollection iCollection = Collection as ICollection;
            
            Log("Verifying ICollection.CopyTo() ...");

            //Invalid input test
            try
            {
                iCollection.CopyTo(tarray, 0);
                throw new Exception("ICollection.CopyTo won't accepts null value!");
            }
            catch (ArgumentNullException e)
            {
                VerifyException(e, _className, "ICollection.CopyTo");
            }

            tarray = new T[ChildLimit - 1];
            Collection.Clear();
            AddChildren();
            try
            {
                iCollection.CopyTo(tarray, 0);
                throw new Exception("ICollection.CopyTo should throw if the supplied array is not large enough!");
            }
            catch (ArgumentException e)
            {
                VerifyException(e,  _className, "ICollection.CopyTo");
            }

            //when Regression_Bug589 is fixed, We will enable the following code
            try
            {
                iCollection.CopyTo(new Array[size], 0);
                throw new Exception("ICollection.CopyTo should throw if the supplied array is the right type!");
            }
            catch (ArgumentException e)
            {
                VerifyException(e, _className, "ICollection.CopyTo");
            }

            //copy the items to an array. Note our array can hold 20 values, and the collection has 10 value
            //we will copy the collection to any 10 consecutive slots.
           
            //Valid input test
            for (int i = 0; i < size - ChildLimit; i++)
            {
                tarray = new T[size];
                iCollection.CopyTo(tarray, i);
                for (int j = 0; j < size; j++)
                {
                    if (j < i)
                    {
                        Verifier.Verify(tarray.GetValue(j) == null, "Should not damiage the slot before the arrayIndex!");
                    }
                    else if (j >= i && j < i + ChildLimit)
                    {
                        Verifier.Verify(tarray.GetValue(j) is T, "coppied items should be type of" + typeof(T).ToString() + "!");
                    }
                    else
                    {
                        Verifier.Verify(tarray.GetValue(j) == null, "Should not damiage the slot after the coppied items!");
                    }
                }
            }
        }

        /// <summary>
        /// Verify the Exceptions
        /// </summary>
        /// <param name="e"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
 
        protected void VerifyException(Exception e, string className, string methodName)
        {
            string topExceptionFrame = e.StackTrace;
            int index = topExceptionFrame.IndexOf("(", 0);
            index = index > 200? topExceptionFrame.Length - 1 : index;
            index = index < 0 ? topExceptionFrame.Length - 1 : index;
            topExceptionFrame = topExceptionFrame.Substring(0, index);

            //Make sure that the Exception is thrown by the Add method.
            Verifier.Verify(topExceptionFrame.Contains(className),
                "The exception is not thrown by TextElementCollection class!");
            Verifier.Verify(topExceptionFrame.Contains(methodName),
                "The exception is not thrown by" +  methodName +"() Method!");
        }


        /// <summary>
        /// Create a new ITextElementParent on which test is performed.
        /// </summary>
        /// <remarks>This method creates the Parent and initializes the Collection property.</remarks>
        public abstract void CreateAndHookUpParent();

        /// <summary>Adds children.</summary>
        public void AddChildren()
        {
            for (int i = 0; i < ChildLimit; i++)
            {
                Collection.Add(CreateInstance());
            }
        }

        /// <summary>Verifies properties/methods on ITextElementParent.</summary>
        void VerifyITextElementParent()
        {
            AddChildren();
            Verifier.Verify(_parent.ContentStart.GetOffsetToPosition(GetFirstChild().ElementStart) == 0,
                "Verifying that distance between ContentStart of parent and ElementStart of firstChild is 0", true);

            Verifier.Verify(_parent.ContentEnd.GetOffsetToPosition(GetLastChild().ElementEnd) == 0,
                "Verifying that distance between ContentEnd of parent and ElementEnd of firstChild is 0", true);

            _parent.ContentStart.InsertTextInRun("Sample Text Begin");
            
            TextRange range = new TextRange( _parent.ContentStart,  _parent.ContentEnd);
            Verifier.Verify(range.Text.Contains("Sample Text Begin"), "Failed to insert text at the beginning of a TextElement");
            _parent.ContentEnd.InsertTextInRun("Sample Text End");
           
            range = new TextRange(_parent.ContentStart, _parent.ContentEnd);
            Verifier.Verify(range.Text.Contains("Sample Text End"), "Failed to insert text at the end of a TextElement");
        }

        /// <summary>Verify the properties that are not covered</summary>
        private void VerifyOtherProperties()
        {
            System.Collections.IList list;
            T child, otherChild;
            TextRange tr; 

            child = CreateInstance();
            otherChild = CreateInstance();

            child.Tag = "Child";
            otherChild.Tag = "OtherChild";

            list = (System.Collections.IList)Collection;
            Log("Verifying IList interface implementation...");

            Log("Verifying that the list is not read-only...");
            Verifier.Verify(list.IsReadOnly == false, "List is not read-only.", true);

            Log("Verifying that the list is not fixed in size...");
            Verifier.Verify(list.IsFixedSize == false, "List is not fixed in size.", true);

            Log("Verifying that the list is thread-safe...");
            Verifier.Verify(list.IsSynchronized == true, "List is synchronized.", true);

            Log("Verifying the IsReadOnly property...");
            Verifier.Verify(!Collection.IsReadOnly,
               "Verifying IsReadOnly property", true);

            //FirstChild
            Log("Verifying the FirstChild property...");
            tr = new TextRange(GetFirstChild().ContentStart, GetFirstChild().ContentEnd);
            Verifier.Verify(tr.Text.Contains(BasicContent),
                 "Verifying the contents of FirstChild", true);

            //LastChild
            Log("Verifying the LastChild property...");
            tr = new TextRange(GetLastChild().ContentStart, GetLastChild().ContentEnd);
            Verifier.Verify(tr.Text.Contains(BasicContent),
                "Verifying the contents of LastChild", true);
        }
        #endregion Invalid testing
        
        #region Helper functions


        /// <summary>Returns the first child in the collection.</summary>
        public abstract T GetFirstChild();

        /// <summary>Returns the last child in the collection.</summary>
        public abstract T GetLastChild();

        /// <summary>Creates a new instance of type T.</summary>
        /// <remarks>
        /// This cannot be made generic because Block() doesn't have an 
        /// empty constructor.
        /// </remarks>
        public abstract T CreateInstance();

        #endregion Helper functions

        #region Public Properties

        /// <summary>Basic content to be used in the test</summary>
        public string BasicContent
        {
            get { return _basicContent; }
            set { _basicContent = value; }
        }

        /// <summary>ChildCount</summary>
        public int ChildCount
        {
            get { return _childCount++; }
        }

        /// <summary>Number of children to be added</summary>
        public int ChildLimit
        {
            get { return _childLimit; }
        }

        /// <summary>Container for children of type T.</summary>
        public virtual TextElementCollection<T> Collection
        {
            get { return _collection; }
            set { _collection = value; }
        }

        /// <summary>Panel</summary>
        public StackPanel Panel
        {
            get { return _panel; }
        }

        /// <summary>Parent instance</summary>
        public new TextElement Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// RichTextBox which acts as a container in this test.
        /// </summary>
        public RichTextBox RichTextBoxContainer
        {
            get { return _wrapper.Element as RichTextBox; }
        }

        /// <summary>
        /// Find the freuqency of a string
        /// </summary>
        /// <param name="MainString"></param>
        /// <param name="SubString"></param>
        /// <returns></returns>
        protected int Occurency(string MainString, string SubString)
        {
            return MainString.Contains(SubString) ? Occurency(MainString.Substring(MainString.IndexOf(SubString) + SubString.Length), SubString) + 1 : 0;
        }
        #endregion Public Properties

        #region Private data.

        private TextElement _parent;
        private TextElementCollection<T> _collection;

        private StackPanel _panel;         
        private const int _childLimit = 10;
        private int _childCount = 0;
        private UIElementWrapper _wrapper; 
        private string _basicContent = "";
        private string _className = "TextElementCollection";

        #endregion Private data.
    }    

    /// <summary>
    /// Test for TextElementCollection[BlockItem]
    /// </summary>
    [Test(0, "TextOM", "BlockItemCollectionTest", MethodParameters = "/TestCaseType=BlockItemCollectionTest")]
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("518"), TestBugs("590, 589, 588, 727, 728, 586, 729, 730, 731, 729"), TestLastUpdatedOn("Jan 25, 2007")]
    public class BlockItemCollectionTest : TextElementCollectionTest<Block>
    {
        /// <summary>
        /// Create a new ITextElementParent on which test is performed.
        /// </summary>        
        public override void CreateAndHookUpParent()
        {
            Section section;

            section = new Section();
            Parent = section;
            Collection = section.Blocks;
            RichTextBoxContainer.Document.Blocks.Add(section);
        }

        /// <summary>Creates a new instance of type T.</summary>
        public override Block CreateInstance() { return new Paragraph(new Run(BasicContent)); }

        /// <summary>Gets the first child in the block collection.</summary>
        public override Block GetFirstChild() { return ((BlockCollection)Collection).FirstBlock; }
        
        /// <summary>Gets the last child in the block collection.</summary>
        public override Block GetLastChild() { return ((BlockCollection)Collection).LastBlock; }

        // Work around Regression_Bug590.
        /// <summary>Container for children of type T.</summary>
        public override TextElementCollection<Block> Collection
        {
            get { return ((Section)Parent).Blocks; }
            set { }
        }
    }

    /// <summary>
    /// Test for TextElementCollection[ListItem]
    /// </summary>
    [Test(0, "TextOM", "ListItemCollectionTest", MethodParameters = "/TestCaseType=ListItemCollectionTest")]
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("517"), TestBugs("587"), TestLastUpdatedOn("Jan 25, 2007")]
    public class ListItemCollectionTest : TextElementCollectionTest<ListItem> 
    {
        /// <summary>
        /// Create a new ITextElementParent on which test is performed.
        /// </summary>        
        public override void CreateAndHookUpParent()
        {
            List list;

            list = new List();
            Parent = list;
            Collection = list.ListItems;

            RichTextBoxContainer.Document.Blocks.Add(list);
        }

        /// <summary>Creates a new instance of type ListItem.</summary>
        public override ListItem CreateInstance() { return new ListItem(new Paragraph(new Run(BasicContent))); }

        /// <summary>Gets the first child in the list item collection.</summary>
        public override ListItem GetFirstChild() { return ((ListItemCollection)Collection).FirstListItem; }

        /// <summary>Gets the last child in the list item collection.</summary>
        public override ListItem GetLastChild() { return ((ListItemCollection)Collection).LastListItem; }
    }
    
    /// <summary>
    /// Test for TextElementCollection[ListItem]
    /// </summary>
    [Test(0, "TextOM", "InlineCollectionTest", MethodParameters = "/TestCaseType=InlineCollectionTest")]
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("516"), TestBugs("591, 732"),TestLastUpdatedOn("Jan 25, 2007")]
    public class InlineCollectionTest : TextElementCollectionTest<Inline>
    {
        /// <summary>
        /// Create a new ITextElementParent on which test is performed.
        /// </summary>        
        public override void CreateAndHookUpParent()
        {
            if (!(RichTextBoxContainer.Document.Blocks.FirstBlock is Paragraph))
            {
                RichTextBoxContainer.Document.Blocks.Clear();
                RichTextBoxContainer.Document.Blocks.Add(new Paragraph());
            }
            Parent = RichTextBoxContainer.Document.Blocks.FirstBlock;
            Collection = ((Paragraph)Parent).Inlines; 
        }

        /// <summary>Creates a new instance of type ListItem.</summary>
        public override Inline CreateInstance() { return new Run(BasicContent); }

        /// <summary>Gets the first child in the list item collection.</summary>
        public override Inline GetFirstChild() { return ((InlineCollection)Collection).FirstInline; }
        /// <summary>Gets the last child in the list item collection.</summary>
        public override Inline GetLastChild() { return ((InlineCollection)Collection).LastInline; }

        /// <summary> Call base class</summary>
        protected override void VerifyInheritedClass ()
        {
            InlineCollecitonAddString();
            Logger.Current.ReportSuccess();  
        }
        void InlineCollecitonAddString()
        {
            string str ;
            IList list = Collection as IList;
            InlineUIContainer container; 
            InlineCollection inlines = Collection as InlineCollection;

            Log("Function: InlineCollecitonAddString, Testing null value...");
            try
            {
                inlines.Add((string) null);
                throw new Exception("InlineCollection.Add(string) should not accept null!");
            }
            catch (Exception e)
            {
                VerifyException(e,"InlineCollection", "Add");
            }
            try
            {

                inlines.Add((UIElement) null);
                throw new Exception("InlineCollection.Add(UIElement) should not accept null!");
            }
            catch (Exception e)
            {
                VerifyException(e, "InlineCollection", "Add");
            }

            //Add a string.
            Log("Function: InlineCollecitonAddString, Testing InlineCollection.add(string)...");
            inlines.Clear();
            inlines.Add("abc");
            Verifier.Verify(inlines.Count == 1, "Failed: InlineCollection should contain only one item after [abc] is added!");
            str = ((Run)inlines.FirstInline).Text;
            Verifier.Verify(str == "abc", "Failed, the collection should ocntains[abc], Actual[" + str + "]");

            //Add an UIelement.
            Log("Function: InlineCollecitonAddString, Testing InlineCollection.add(UIelement)...");
            Button button = new Button();
            button.Content = "myButton";
            inlines.Clear();
            inlines.Add(button);
            Verifier.Verify(inlines.Count == 1, "Failed: InlineCollection should contain only one item after a button is added!");
            container = inlines.FirstInline as InlineUIContainer ;

            Verifier.Verify(((Button)container.Child).Content.ToString() == "myButton", 
                "Failed, the content of the button should be[myButton], Actual[" + ((Button)container.Child).Content.ToString() + "]");

            //Test IList.Add()
            Log("Function: InlineCollecitonAddString, Testing IList.add(string)...");
            list.Clear();
            list.Add("abcd");
            Verifier.Verify(inlines.Count == 1, "Failed: InlineCollection should contain only one item after [abcd] is added!");
            str = ((Run)inlines.FirstInline).Text;
            Verifier.Verify(str == "abcd", "Failed, the collection should ocntains[abc], Actual[" + str + "]");

            //for UIelement through IList.
            Log("Function: InlineCollecitonAddString, Testing IList.add(UIelement)...");
            inlines.Clear();
            
            //Regression_Bug591 is by design. 
            button = new Button();
            button.Content = "myButton";
            list.Add(button);
            Verifier.Verify(inlines.Count == 1, "Failed: InlineCollection should contain only one item after a button is added through IList.Add!");
            container = inlines.FirstInline as InlineUIContainer;

            Verifier.Verify(((Button)container.Child).Content.ToString() == "myButton",
                "Failed, the content of the button should be[myButton], Actual[" + ((Button)container.Child).Content.ToString() + "]");
        }
    }

    /// <summary>dummy range for test of AddRange</summary>
    internal class TestRange : IEnumerable<TextElement>
    {
        ArrayList _list; 
        bool _nullEnumerator=false;
        /// <summary>do nothing</summary>
        public TestRange(TextElement element)
        {
            _list = new ArrayList();
        }
        /// <summary>return null Enumerator</summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_nullEnumerator)
            {
                return null;
            }
            else
            {
                return _list.GetEnumerator();
            }
        }

        /// <summary>Add a value in </summary>
        /// <param name="o"></param>
        public void Add(object o)
        {
            _list.Add(o);
        }

        /// <summary>Clear the list</summary>
        public void clear()
        {
            _list.Clear();
        }
        
        ///// <returns></returns>
        /// <summary>
        /// return null Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TextElement> GetEnumerator()
        {
            return null ;
        }
        public void SetNullEnumerator()
        {
            _nullEnumerator = true;
        }
    }

    /// <summary>
    /// Tests functions of TextElementEnumerator
    /// </summary>
    [Test(0, "TextOM", "TextElementEnumeratorTest", MethodParameters = "/TestCaseType=TextElementEnumeratorTest")]
    [TestOwner("Microsoft"), TestTactics("515"), TestWorkItem("81"), TestBugs("595, 593, 592"),TestLastUpdatedOn("Jan 25, 2007")]
    public class TextElementEnumeratorTest : CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            StackPanel stackPanel = new StackPanel();
            _rtb = new RichTextBox();
            InitializeRichTextBox(_rtb);
            UIElementWrapper _rtbWrapper = new UIElementWrapper(_rtb);
            _rtbWrapper = new UIElementWrapper(_rtb);
            stackPanel.Children.Add(_rtb);
            MainWindow.Content = stackPanel;
            QueueDelegate(DoFocus);
        }

        private void InitializeRichTextBox(RichTextBox rtb)
        {
            rtb.FontSize = 14;
            rtb.Width = 300;
            rtb.Height = 400;
            ((Paragraph)(rtb.Document.Blocks.FirstBlock)).Inlines.Add(new Bold(new Run(_firstRun)));
            ((Paragraph)(rtb.Document.Blocks.FirstBlock)).Inlines.Add(new InlineUIContainer(new Button()));
            ((Paragraph)(rtb.Document.Blocks.FirstBlock)).Inlines.Add(new Run(_secRun));
            rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        /// <summary>Focuses on element</summary>
        private void DoFocus()
        {
            _rtb.Focus();
            QueueDelegate(VerifyEnumeratorMovement);
        }

        /// <summary>verify correct working of IEnumerator</summary>
        private void VerifyEnumeratorMovement()
        {
            _collection = ((Paragraph)(_rtb.Document.Blocks.FirstBlock)).Inlines;
            int count = _collection.Count;
            _inlineCollection = _collection.GetEnumerator();
            
            _inlineCollection.MoveNext();
            while (count > 0)
            {
                Inline _inline = (Inline)(_inlineCollection.Current);

                // Regression_Bug592 
                _inline = (Inline)(_inlineCollection.Current);

                if (_inline.GetType().ToString() == "System.Windows.Documents.Bold")
                {
                    Run rnInside = (Run)(((Bold)_inline).Inlines.FirstInline);
                    Verifier.Verify(rnInside.Text == _firstRun, "Bold element Expected [" +
                        _firstRun + "] Actual [" + rnInside.Text + "]", true);
                }
                else
                if (_inline.GetType().ToString() == "System.Windows.Documents.Run")
                {
                    Verifier.Verify(((Run)(_inline)).Text == _secRun, "Run element Expected [" +
                        _secRun + "] Actual [" + ((Run)(_inline)).Text + "]", true);
                }
                else
                if (_inline.GetType().ToString() == "System.Windows.Documents.InlineUIContainer")
                {
                    Verifier.Verify(((InlineUIContainer)_inline).Child.GetType().ToString() == "System.Windows.Controls.Button",
                        "InlineUIContainer Expected[Button] Actual[" + ((InlineUIContainer)_inline).Child.GetType().ToString() + "]", true);
                }
                else
                {
                    throw new ApplicationException("Unexpected type of inline element" + _inline.GetType().ToString());
                }
                count--;

                if (count >0) 
                { 
                    _inlineCollection.MoveNext(); 
                }
            }
            VerifyResettingofEnumerator();
        }

        /// <summary>Getting current after resetting throws exception. Also verifies reset works</summary>
        private void VerifyResettingofEnumerator()
        {
            _inlineCollection.Reset();

            try
            {
                Inline _inline = (Inline)(_inlineCollection.Current);
                throw new ApplicationException("EXPECTED: Getting Current after Resetting throws exception");
            }
            catch (InvalidOperationException)
            {
                Log("THROWN AS EXPECTED: Getting Current after Resetting throws exception");
            }
            _inlineCollection.MoveNext();
            Run rnInside = (Run)((Bold)(Inline)(_inlineCollection.Current)).Inlines.FirstInline;
            Verifier.Verify(rnInside.Text == _firstRun, "First Run Expected[" +
                _firstRun + "] Actual [" + rnInside.Text + "]", true);
            int count = _collection.Count;

            //Make COUNT >0 WHEN Regression_Bug593 IS RESOLVED
            while (count > 1)
            {
                _inlineCollection.MoveNext();
                count--;
            }
            TestEnumeratorAfterModifyingRtbStructure();
        }

        /// <summary>Verify resetting throws exception after modifying the structure</summary>
        private void TestEnumeratorAfterModifyingRtbStructure()
        {
            ((Paragraph)(_rtb.Document.Blocks.FirstBlock)).Inlines.Remove((Inline)(_inlineCollection.Current));
                
            //REMOVE ONCE Regression_Bug595 IS RESOLVED
            try
            {
                Log("Getting Current after Modifying RTB structure Gets the cached current value");
                Inline _inline = (Inline)(_inlineCollection.Current);
                Verifier.Verify(((Run)(Inline)(_inlineCollection.Current)).Text == _secRun, "Sec Run Expected[" +
                _secRun + "] Actual [" + ((Run)(Inline)(_inlineCollection.Current)).Text + "]", true);
            //    throw new ApplicationException("EXPECTED: Getting Current after Modifying RTB structure throws exception");
            }
            catch (InvalidOperationException)
            {
                Log("THROWN AS EXPECTED: Getting Current after Modifying RTB structure throws exception");
            }

            try
            {
                _inlineCollection.Reset();
                throw new ApplicationException("EXPECTED: Reset after Modifying RTB structure throws exception");
            }
            catch (InvalidOperationException)
            {
                Log("THROWN AS EXPECTED: Reset after Modifying RTB structure throws exception");
            }

            InvalidTesting();
        }

        /// <summary>Verify move next throws exception after deleting the block.</summary>
        private void InvalidTesting()
        {
            _rtb.Document.Blocks.Remove(_rtb.Document.Blocks.FirstBlock);

            try
            {
                _inlineCollection.MoveNext();
                throw new ApplicationException("EXPECTED: MoveNext after deleting block throws exception");
            }
            catch (InvalidOperationException)
            {
                Log("THROWN AS EXPECTED: MoveNext after deleting block throws exception");
            }
            Logger.Current.ReportSuccess();
        }

        /// <summary>Verify current throws exception after Dispose//Why isnt dispose available?.</summary>
        private void TestEnumeratorAfterDispose()
        {
            _rtb1 = new RichTextBox();
            InitializeRichTextBox(_rtb1);

            _collection = ((Paragraph)(_rtb1.Document.Blocks.FirstBlock)).Inlines;
            int count = _collection.Count;
            _inlineCollection = _collection.GetEnumerator();

            while (count-- > 0)
            {
                _inlineCollection.MoveNext();
            }
          //  _inlineCollection.Dispose();

            try
            {
                Inline _inline = (Inline)(_inlineCollection.Current);
                throw new ApplicationException("EXPECTED: Getting Current after Dispose throws exception");
            }
            catch (InvalidOperationException)
            {
                Log("THROWN AS EXPECTED: Getting Current after Dispose throws exception");
            }
        }

        #region data.

        private RichTextBox _rtb = null;
        private RichTextBox _rtb1 = null;
        private string _firstRun = "hello";
        private string _secRun = "world";
        private TextElementCollection<Inline> _collection;
        private IEnumerator _inlineCollection;

        #endregion data.
    }

    /// <summary>
    /// Tests reading content in diff threads
    /// This test scenario is not supported in 4.0. Refer bug Part1 Regression_Bug594
    /// </summary>
    [Test(0, "TextOM", "RichTextBoxReadInDiffThreads", MethodParameters = "/TestCaseType=RichTextBoxReadInDiffThreads", Versions="3.0,3.0SP1,3.0SP2,AH")]
    [TestOwner("Microsoft"), TestTactics("514"), TestWorkItem("80"), TestLastUpdatedOn("Jan 25, 2007")]
    public class RichTextBoxReadInDiffThreads : CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            StackPanel stackPanel = new StackPanel();
            _rtb = new RichTextBox();
            InitializeRichTextBox(_rtb);
            UIElementWrapper _rtbWrapper = new UIElementWrapper(_rtb);
            _rtbWrapper = new UIElementWrapper(_rtb);
            stackPanel.Children.Add(_rtb);
            _autoThread = null;
            _autoThread1 = null;
            MainWindow.Content = stackPanel;
            QueueDelegate(DoFocus);
        }

        private void InitializeRichTextBox(RichTextBox rtb)
        {
            rtb.FontSize = 14;
            rtb.Width = 300;
            rtb.Height = 400;
            ((Paragraph)(rtb.Document.Blocks.FirstBlock)).Inlines.Add(new Bold(new Run(_firstRun)));
            ((Paragraph)(rtb.Document.Blocks.FirstBlock)).Inlines.Add(new Run(_secRun));
            rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        /// <summary>Focuses on element</summary>
        private void DoFocus()
        {
            _rtb.Focus();
            QueueDelegate(CreateThreads);
        }

        private void CreateThreads()
        {
            _autoThread = new Thread(new ThreadStart(ReadUsingEnumerator));
            _autoThread.SetApartmentState(System.Threading.ApartmentState.STA);
            _autoThread.Name = "Thread 1";

            _autoThread1 = new Thread(new ThreadStart(ReadUsingEnumerator));
            _autoThread1.SetApartmentState(System.Threading.ApartmentState.STA);
            _autoThread1.Name = "Thread 2";

            _autoThread.Start();
            
            bool threadReturned = _autoThread.Join(4000);
            _autoThread1.Start();
            threadReturned = threadReturned & _autoThread1.Join(4000);
            if (_threadCompleteCount == 2)
            {
                ReportSuccess();
            }
            else
            {
                Logger.Current.ReportResult(false, "TEST CASE FAILED ");
            }
        }

        /// <summary>verify correct working of IEnumerator</summary>
        private void ReadUsingEnumerator()
        {
             TextElementCollection<Inline> _collection;
             IEnumerator _inlineCollection;
            _collection = ((Paragraph)(_rtb.Document.Blocks.FirstBlock)).Inlines;
            int count = _collection.Count;
            _inlineCollection = _collection.GetEnumerator();

            _inlineCollection.MoveNext();
            while (count > 0)
            {
                Inline _inline = (Inline)(_inlineCollection.Current);
                _inline = (Inline)(_inlineCollection.Current);

                if (_inline.GetType().ToString() == "System.Windows.Documents.Bold")
                {
                    Log("\r\n----" +Thread.CurrentThread.Name +" : Inside Bold ----");
                    
                    Run rnInside = (Run)(((Bold)_inline).Inlines.FirstInline);
                    Verifier.Verify(rnInside.Text == _firstRun, "Bold element Expected [" +
                        _firstRun + "] Actual [" + rnInside.Text + "]", true);
                }
                else
                    if (_inline.GetType().ToString() == "System.Windows.Documents.Run")
                    {
                        Log("\r\n----" + Thread.CurrentThread.Name + " : Inside Run ----");
                        Verifier.Verify(((Run)(_inline)).Text == _secRun, "Run element Expected [" +
                            _secRun + "] Actual [" + ((Run)(_inline)).Text + "]", true);
                    }
                   
                count--;
                if (count > 0)
                {
                    _inlineCollection.MoveNext();
                }
            }
            _threadCompleteCount++;
        }

        private static void ReportSuccess()
        {
            Logger.Current.ReportSuccess();
        }

        #region data.

        private RichTextBox _rtb = null;
        private string _firstRun = "hello";
        private string _secRun = "world";
        private int _threadCompleteCount = 0;
        private Thread _autoThread = null;
        private Thread _autoThread1 = null;

        #endregion data.
    }
}
