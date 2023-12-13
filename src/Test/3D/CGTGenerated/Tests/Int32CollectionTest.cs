// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for Int32Collection (generated)
*
*   !!! This is a generated file. Do NOT edit it manually !!!
*
************************************************************/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes; using Microsoft.Test.Graphics;
using Microsoft.Test.Graphics.Factories;
using System.Collections;
using System.Collections.Generic;

//------------------------------------------------------------------

namespace                       Microsoft.Test.Graphics.Generated
{
    //--------------------------------------------------------------

    public partial class        Int32CollectionTest : CoreGraphicsTest
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    Init( Variation v )
        {
            base.Init( v );

            _collection      = NewInt32Collection();
            _list            = NewInt32Collection();
            _objects         = new List<int>();
            _bogusCollection = new ArrayList();

            _objects.Add( 0 );
            _objects.Add( 1 );
            _objects.Add( 2 );
            _objects.Add( 3 );
            _objects.Add( 4 );
            _objects.Add( 5 );

            _bogusCollection.Add( 10 );
            _bogusCollection.Add( new TranslateTransform() );
            _bogusCollection.Add( new Point3D() );
            _bogusCollection.Add( null );
            _bogusCollection.Add( new Point() );
            _bogusCollection.Add( new Vector3D() );
            _bogusCollection.Add( new Matrix3D() );
            _bogusCollection.Add( true );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    RunTheTest()
        {
            if ( priority == 0 )
            {
                TestConstructor();
                TestAdd();
                TestChanged();
                TestClear();
                TestContains();
                TestCopyTo();
                TestCount();
                TestEnumerator();
                TestEquals();
                TestGetAsFrozen();
                TestGetCurrentValueAsFrozen();
                TestIndexOf();
                TestInsert();
                TestIsFixedSize();
                TestIsReadOnly();
                TestIsSynchronized();
                TestItem();
                TestRemove();
                TestRemoveAt();
                TestSyncRoot();
                TestToString();
            }
            else // priority > 0
            {
                TestConstructor2();
                TestAdd2();
                TestContains2();
                TestCopyTo2();
                TestEnumerator2();
                TestIndexOf2();
                TestInsert2();
                TestItem2();
                TestRemove2();
                TestRemoveAt2();
                TestToString2();
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Int32Collection NewInt32Collection()
        {
            return new Int32Collection();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Int32Collection NewInt32Collection( IEnumerable<int> collection )
        {
            return new Int32Collection( collection );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Int32Collection NewInt32Collection( int capacity )
        {
            return new Int32Collection( capacity );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            PopulateCollection()
        {
            _collection = NewInt32Collection();
            foreach ( int obj in _objects )
            {
                _collection.Add( obj );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private int[]           intArray
        {
            get
            {
                int[] array = new int[ _objects.Count ];
                _objects.CopyTo( array );
                return array;
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            PopulateList()
        {
            _list = NewInt32Collection();
            foreach ( int obj in _objects )
            {
                _list.Add( obj );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConstructor()
        {
            Log( "Testing Constructor..." );

            Int32Collection empty = NewInt32Collection();
            if ( empty.Count != 0 || failOnPurpose )
            {
                AddFailure( "Int32Collection.ctor() failed" );
                Log( "*** Expected: Default constructor creates empty collection" );
                Log( "*** Actual:   {0} objects in \"empty\" collection", empty.Count );
            }

            PopulateCollection();
            Int32Collection theirAnswer = NewInt32Collection( _collection );

            if ( theirAnswer.Count != _objects.Count || failOnPurpose )
            {
                AddFailure( "Int32Collection.ctor( IEnumerable<int> ) failed" );
                Log( "*** Expected: {0} objects in my collection", _objects.Count );
                Log( "*** Actual:   {0} objects in their collection", theirAnswer.Count );
            }
            else
            {
                for ( int i = 0; i < theirAnswer.Count; i++ )
                {
                    if ( !theirAnswer[ i ].Equals( _objects[ i ] ) || failOnPurpose )
                    {
                        AddFailure( "Int32Collection.ctor( IEnumerable<int> ) failed" );
                        Log( "*** Expected: Collection[{0}] = {1}", i, _collection[ i ] );
                        Log( "*** Actual:   Collection[{0}] = {1}", i, theirAnswer[ i ] );
                        break;
                    }
                }
            }

            TestConstructorWith( new List<int>() );
            TestConstructorWith( new FakeEnumerable() );
            TestConstructorWith( new NonCollectionEnumerable() );

            TestConstructorWith( 0 );
            TestConstructorWith( 5 );
            TestConstructorWith( 50 );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConstructorWith( IEnumerable<int> e )
        {
            Int32Collection theirAnswer = NewInt32Collection( e );

            if ( theirAnswer.Count != 0 || failOnPurpose )
            {
                AddFailure( "Int32Collection.ctor( IEnumerable<int> ) failed" );
                Log( "*** Expected: Count == 0" );
                Log( "*** Actual:   Count == {0}", theirAnswer.Count );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConstructorWith( int capacity )
        {
            Int32Collection theirAnswer = NewInt32Collection( capacity );

            if ( theirAnswer.Count != 0 || failOnPurpose )
            {
                AddFailure( "Int32Collection.ctor( int ) failed" );
                Log( "*** Expected: Count == 0" );
                Log( "*** Actual:   Count == {0}", theirAnswer.Count );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region FakeEnumerable

        private class           FakeEnumerable : IEnumerable<int>, ICollection<int>
        {
            public void         Add( int p )
            {
            }
            public void         Clear()
            {
            }
            public bool         Contains( int p )
            {
                return false;
            }
            public void         CopyTo( int[] points, int index )
            {
            }
            public bool         Remove( int p )
            {
                return false;
            }
            public int          Count
            {
                get { return 0; }
            }
            public bool         IsReadOnly
            {
                get { return true; }
            }
            public IEnumerator<int> GetEnumerator()
            {
                return new Enumerator();
            }
            IEnumerator         IEnumerable.GetEnumerator()
            {
                return new Enumerator();
            }

            private class       Enumerator : IEnumerator<int>
            {
                public int Current
                {
                    get { return new int(); }
                }
                public void     Dispose()
                {
                }
                public bool     MoveNext()
                {
                    return false;
                }
                public void     Reset()
                {
                }
                object IEnumerator.Current
                {
                    get { return new int(); }
                }
            }
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region NonCollectionEnumerable

        private class           NonCollectionEnumerable : IEnumerable<int>
        {
            public IEnumerator<int> GetEnumerator()
            {
                return new Enumerator();
            }
            IEnumerator         IEnumerable.GetEnumerator()
            {
                return new Enumerator();
            }

            private class       Enumerator : IEnumerator<int>
            {
                public int Current
                {
                    get { return new int(); }
                }
                public void     Dispose()
                {
                }
                public bool     MoveNext()
                {
                    return false;
                }
                public void     Reset()
                {
                }
                object IEnumerator.Current
                {
                    get { return new int(); }
                }
            }
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConstructor2()
        {
            Log( "Testing Constructor with bad parameters..." );

            Try( ConstructWithNull, typeof( ArgumentNullException ) );

        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region ExceptionThrowers for Constructor

        private void            ConstructWithNull()
        {
            Int32Collection c = NewInt32Collection( null );
        }


        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestAdd()
        {
            Log( "Testing Add..." );
            _list = NewInt32Collection();
            _collection = NewInt32Collection();

            foreach ( object o in _objects )
            {
                TestAddWith( o );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestAddWith( object o )
        {
            int oldCount = _list.Count;
            int position = _list.Add( o );

            if ( oldCount+1 != _list.Count || position != oldCount ||
                    !_list[ position ].Equals( o ) || failOnPurpose )
            {
                AddFailure( "IList.Add failed" );
                Log( "*** Expected: count = {0}, position = {1}, object added = {2}", oldCount+1, oldCount, o );
                Log( "***   Actual: count = {0}, position = {1}, object added = {2}", _list.Count, position, _list[ position ] );
            }

            _collection.Add( (int)o );

            if ( oldCount+1 != _collection.Count || position != oldCount ||
                    !_collection[ position ].Equals( (int)o ) || failOnPurpose )
            {
                AddFailure( "Int32Collection.Add failed" );
                Log( "*** Expected: count = {0}, position = {1}, int added = {2}", oldCount+1, oldCount, o );
                Log( "***   Actual: count = {0}, position = {1}, int added = {2}", _collection.Count, position, _collection[ position ] );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestAdd2()
        {
            Log( "Testing IList.Add( object ) with bad parameters..." );

            foreach ( object o in _bogusCollection )
            {
                Type itemType = ( o == null ) ? null : o.GetType();
                Type addType = typeof( int );
                try
                {
                    _list.Add( o );
                    if ( !itemType.IsSubclassOf( addType ) && !itemType.Equals( addType ) )
                    {
                        AddFailure( "Should not be able to add object of incorrect type to this collection" );
                    }
                }
                catch ( ArgumentException )
                {
                    if ( itemType == null || !itemType.IsSubclassOf( addType ) )
                    {
                        Log( "  Good! - Invalid cast detected when adding object of incorrect type" );
                    }
                    else
                    {
                        AddFailure( "Invalid cast detected when adding object of correct type" );
                    }
                }
            }

            Try( AddNullToList, typeof( ArgumentException ) );

            // Skipping null tests for Int32Collection.Add( int ) because it's a value-type
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region ExceptionThrowers for Add

        private void            AddNullToList()
        {
            _list = NewInt32Collection();
            _list.Add( null );
        }

        // Skipping null tests for Int32Collection.Add( int ) because it's a value-type

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestChanged()
        {
            Log( "Testing Int32Collection.Changed..." );

            TestChangedForAdd();
            TestChangedForClear();
            TestChangedForConstructor();
            TestChangedForInsert();
            TestChangedForItem();
            TestChangedForRemove();
            TestChangedForRemoveAt();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestChangedForAdd()
        {
            _collection = NewInt32Collection();
            _collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 0;
            _objectsExpected = 0;

            foreach ( int obj in _objects )
            {
                _callbacksExpected++;
                _objectsExpected++;
                _collection.Add( obj );
            }
            VerifyCallbacksProcessed();

            _collection.Changed -= ChangedHandler;

            foreach ( int obj in _objects )
            {
                // No more callbacks expected
                _objectsExpected++;
                _collection.Add( obj );
            }
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestChangedForClear()
        {
            PopulateCollection();
            _collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 1;
            _objectsExpected = 0;
            _collection.Clear();

            _callbacksExpected++;
            _collection.Clear();
            VerifyCallbacksProcessed();

            _collection.Changed -= ChangedHandler;
            // No more callbacks expected
            _collection.Clear();
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestChangedForConstructor()
        {
            PopulateCollection();
            _collection.Changed += ChangedHandler;

            // We don't expect a callback from this collection
            //  since we don't have access to the property until the constructor returns.
            //  But set the # of objects to reduce failure count just in case.
            _callbacksProcessed = 0;
            _callbacksExpected = 0;
            _objectsExpected = _objects.Count;

            // collection.Copy will call the private copy constructor.
            Int32Collection temp = _collection.Clone();
            temp = NewInt32Collection( _collection );
            VerifyCallbacksProcessed();

            _collection.Changed -= ChangedHandler;

            temp = _collection.Clone();
            temp = NewInt32Collection( _collection );
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestChangedForInsert()
        {
            _collection = NewInt32Collection();
            _collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 0;
            _objectsExpected = 0;

            foreach ( int obj in _objects )
            {
                _callbacksExpected++;
                _objectsExpected++;
                _collection.Insert( 0, obj );
            }
            VerifyCallbacksProcessed();

            _collection.Changed -= ChangedHandler;

            foreach ( int obj in _objects )
            {
                // No more callbacks expected
                _objectsExpected++;
                _collection.Insert( 0, obj );
            }
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestChangedForItem()
        {
            PopulateCollection();
            _collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 1;
            _objectsExpected = _objects.Count;

            _collection[ 0 ] = _objects[ _objects.Count-1 ];
            VerifyCallbacksProcessed();

            _collection.Changed -= ChangedHandler;

            // No more callbacks expected
            _collection[ 0 ] = _objects[ _objects.Count-1 ];
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestChangedForRemove()
        {
            PopulateCollection();
            _collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 0;
            _objectsExpected = _objects.Count;



            foreach ( int obj in _objects )
            {
                _callbacksExpected++;
                _objectsExpected--;
                _collection.Remove( obj );
            }


            VerifyCallbacksProcessed();

            _collection.Changed -= ChangedHandler;
            PopulateCollection();
            _objectsExpected = _objects.Count;



            foreach ( int obj in _objects )
            {
                // No more callbacks expected
                _objectsExpected--;
                _collection.Remove( obj );
            }


            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestChangedForRemoveAt()
        {
            PopulateCollection();
            _collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 0;
            _objectsExpected = _objects.Count;

            foreach ( int obj in _objects )
            {
                _callbacksExpected++;
                _objectsExpected--;
                _collection.RemoveAt( 0 );
            }
            VerifyCallbacksProcessed();

            _collection.Changed -= ChangedHandler;
            PopulateCollection();
            _objectsExpected = _objects.Count;

            foreach ( int obj in _objects )
            {
                // No more callbacks expected
                _objectsExpected--;
                _collection.RemoveAt( 0 );
            }
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private int             _callbacksProcessed;
        private int             _objectsExpected;
        private int             _callbacksExpected;

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            ChangedHandler( object sender, EventArgs args )
        {
            _callbacksProcessed++;

            // Do this partially because I want to verify "sender"
            //  and partially so I don't have to create a new collection variable
            //  and cast from "sender"
            if ( !object.ReferenceEquals( sender, _collection ) || failOnPurpose )
            {
                AddFailure( "It's not the same collection" );
                Log( "*** Expected: {0}", _collection );
                Log( "***   Actual: {0}", sender );
            }

            if ( _collection.Count != _objectsExpected || failOnPurpose )
            {
                AddFailure( "# objects in collection is not many as we expected" );
                Log( "*** Expected: {0}", _objectsExpected );
                Log( "***   Actual: {0}", _collection.Count );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            VerifyCallbacksProcessed()
        {
            if ( _callbacksProcessed != _callbacksExpected || failOnPurpose )
            {
                AddFailure( "Callbacks processed and callbacks expected do not match" );
                Log( "*** Expected: {0}", _callbacksExpected );
                Log( "***   Actual: {0}", _callbacksProcessed );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestClear()
        {
            Log( "Testing Clear..." );

            // Call Clear on a list with items in it
            PopulateList();
            _list.Clear();
            if ( _list.Count != 0 || failOnPurpose )
            {
                AddFailure( "Clear failed" );
                Log( "*** Expected: count = 0" );
                Log( "***   Actual: count = {0}", _list.Count );
            }
            foreach ( object o in _list )
            {
                AddFailure( "There shouldn't be any objects in the list despite what \"Count\" says" );
                break;
            }

            // Call clear on an empty list
            _list = NewInt32Collection();
            _list.Clear();
            if ( _list.Count != 0 || failOnPurpose )
            {
                AddFailure( "Clear failed" );
                Log( "*** Expected: count = 0" );
                Log( "***   Actual: count = {0}", _list.Count );
            }
            foreach ( object o in _list )
            {
                AddFailure( "There shouldn't be any objects in the list despite what \"Count\" says" );
                break;
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestContains()
        {
            Log( "Testing IList.Contains..." );
            PopulateList();

            foreach ( int obj in _objects )
            {
                if ( !_list.Contains( obj ) || failOnPurpose )
                {
                    AddFailure( "IList.Contains failed" );
                    Log( "Could not locate {0} in my collection", obj );
                }
            }

            _list.Remove( _objects[ 0 ] );
            if ( _list.Contains( _objects[ 0 ] ) || failOnPurpose )
            {
                AddFailure( "IList.Contains failed" );
                Log( "Should not have located {0} in my collection", _objects[ 0 ] );
            }

            Log( "Testing Int32Collection.Contains..." );
            PopulateCollection();

            foreach ( int obj in _objects )
            {
                if ( !_collection.Contains( obj ) || failOnPurpose )
                {
                    AddFailure( "Int32Collection.Contains failed" );
                    Log( "Could not locate {0} in my collection", obj );
                }
            }

            _collection.Remove( _objects[ 0 ] );
            if ( _collection.Contains( _objects[ 0 ] ) || failOnPurpose )
            {
                AddFailure( "Int32Collection.Contains failed" );
                Log( "Should not have located {0} in my collection", _objects[ 0 ] );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestContains2()
        {
            Log( "Testing IList.Contains( object ) with bad parameters..." );

            foreach ( object o in _bogusCollection )
            {
                Type itemType = ( o == null ) ? null : o.GetType();
                Type listType = typeof( int );
                if ( listType.Equals( itemType ) )
                {
                    // We're not looking for items of the correct type here
                    continue;
                }
                try
                {
                    bool theirAnswer = _list.Contains( o );
                    if ( theirAnswer != false || failOnPurpose )
                    {
                        AddFailure( "Found item in collection that should not be there" );
                        Log( "*** Found: " + o );
                    }
                }
                catch ( Exception )
                {
                    AddFailure( "Should just return false when asking if collection contains wrong type" );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCopyTo()
        {
            Log( "Testing ICollection.CopyTo..." );
            PopulateList();

            object[] array = new object[ _objects.Count ];
            _list.CopyTo( array, 0 );

            for ( int i = 0; i < _list.Count; i++ )
            {
                if ( !_list[ i ].Equals( array[ i ] ) || failOnPurpose )
                {
                    AddFailure( "ICollection.CopyTo( Array,int ) failed" );
                    Log( "*** Expected: {0}", _list[ i ] );
                    Log( "***   Actual: {0}", array[ i ] );
                    break;
                }
            }

            Log( "Testing Int32Collection.CopyTo..." );
            PopulateCollection();
            int[] typedArray = new int[ _objects.Count ];
            _collection.CopyTo( typedArray, 0 );

            for ( int i = 0; i < _collection.Count; i++ )
            {
                if ( !_collection[ i ].Equals( typedArray[ i ] ) || failOnPurpose )
                {
                    AddFailure( "CopyTo( int[],int ) failed" );
                    Log( "*** Expected: {0}", _collection[ i ] );
                    Log( "***   Actual: {0}", typedArray[ i ] );
                    break;
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCopyTo2()
        {
            Log( "Testing ICollection.CopyTo with bad parameters..." );

            Try( CopyToNullArray,               typeof( ArgumentException ) );
            Try( CopyToMultidimensionalArray,   typeof( ArgumentException ) );
            Try( CopyToWrongTypeArray,          typeof( ArgumentException ) );
            Try( CopyToSmallerArray,            typeof( ArgumentOutOfRangeException ) );
            Try( CopyToNegativeStartPosition,   typeof( ArgumentOutOfRangeException ) );
            Try( CopyToNotEnoughRoomPosition,   typeof( ArgumentOutOfRangeException ) );

            Log( "Testing Int32Collection.CopyTo with bad parameters..." );

            Try( TypedCopyToNullArray,              typeof( ArgumentException ) );
            Try( TypedCopyToMultidimensionalArray,  typeof( ArgumentException ) );
            Try( TypedCopyToSmallerArray,           typeof( ArgumentOutOfRangeException ) );
            Try( TypedCopyToNegativeStartPosition,  typeof( ArgumentOutOfRangeException ) );
            Try( TypedCopyToNotEnoughRoomPosition,  typeof( ArgumentOutOfRangeException ) );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region ExceptionThrowers for CopyTo

        private void            CopyToNullArray()
        {
            PopulateList();
            object[] array = null;
            _list.CopyTo( array, 0 );
        }

        private void            CopyToMultidimensionalArray()
        {
            PopulateList();
            object[,] array = new object[ _objects.Count,_objects.Count ];
            _list.CopyTo( array, 0 );
        }

        private void            CopyToWrongTypeArray()
        {
            PopulateList();

            // We're okay with this so long as we don't have BooleanCollections
            bool[] array = new bool[ _objects.Count ];
            _list.CopyTo( array, 0 );
        }

        private void            CopyToSmallerArray()
        {
            PopulateList();
            object[] array = new object[ _objects.Count-1 ];
            _list.CopyTo( array, 0 );
        }

        private void            CopyToNegativeStartPosition()
        {
            PopulateList();
            object[] array = new object[ _objects.Count ];
            _list.CopyTo( array, -2 );
        }

        private void            CopyToNotEnoughRoomPosition()
        {
            PopulateList();
            object[] array = new object[ _objects.Count ];
            _list.CopyTo( array, 1 );
        }

        private void            TypedCopyToNullArray()
        {
            PopulateCollection();
            int[] array = null;
            _collection.CopyTo( array, 0 );
        }

        private void            TypedCopyToMultidimensionalArray()
        {
            PopulateList();
            int[,] array = new int[ _objects.Count,_objects.Count ];
            _list.CopyTo( array, 0 );
        }

        private void            TypedCopyToSmallerArray()
        {
            PopulateCollection();
            int[] array = new int[ _objects.Count-1 ];
            _collection.CopyTo( array, 0 );
        }

        private void            TypedCopyToNegativeStartPosition()
        {
            PopulateCollection();
            int[] array = new int[ _objects.Count ];
            _collection.CopyTo( array, -2 );
        }

        private void            TypedCopyToNotEnoughRoomPosition()
        {
            PopulateCollection();
            int[] array = new int[ _objects.Count ];
            _collection.CopyTo( array, 1 );
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCount()
        {
            Log( "Testing Count..." );
            PopulateList();

            int count = 0;
            int theirAnswer = _list.Count;

            foreach ( object o in _list )
            {
                count++;
            }
            if ( count != theirAnswer || failOnPurpose )
            {
                AddFailure( "Count failed" );
                Log( "*** Expected: {0}", count );
                Log( "***   Actual: {0}", theirAnswer );
            }

            // Make sure list really is cleared and that Count is updated
            _list.Clear();
            count = 0;
            theirAnswer = _list.Count;

            foreach ( object o in _list )
            {
                count++;
            }
            if ( count != theirAnswer || failOnPurpose )
            {
                AddFailure( "Count failed" );
                Log( "*** Expected: {0}", count );
                Log( "***   Actual: {0}", theirAnswer );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestEnumerator()
        {
            Log( "Testing intEnumerator..." );
            PopulateCollection();

            TestEnumeratorWith( NewInt32Collection() );
            TestEnumeratorWith( _collection );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestEnumeratorWith( Int32Collection collection )
        {
            IEnumerator enumerator = ((IEnumerable)collection).GetEnumerator();

            Log( "Testing MoveNext and get_Current..." );
            TestMoveNextWith( enumerator, collection );

            Log( "Testing Reset..." );
            enumerator.Reset();
            int currentFailures = Failures;
            TestMoveNextWith( enumerator, collection );
            if ( currentFailures < Failures )
            {
                Log( "The above MoveNext failures were actually caused by a failing Reset function" );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestMoveNextWith( IEnumerator enumerator, Int32Collection collection )
        {
            int count = 0;
            while ( enumerator.MoveNext() )
            {
                int current = (int)enumerator.Current;
                if ( !collection[ count ].Equals( current ) || failOnPurpose )
                {
                    AddFailure( "Enumerator failed on object {0}", count );
                    Log( "*** Expected: Current = {0}", current );
                    Log( "***   Actual: Current = {0}", collection[ count ] );
                }
                count++;
            }

            if ( collection.Count != count || failOnPurpose )
            {
                AddFailure( "MoveNext failed" );
                Log( "*** Expected: should be called exactly {0} times", collection.Count + 1 );
                Log( "***   Actual: was called {0} times", count + 1 );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestEnumerator2()
        {
            Log( "Testing Enumerator with bad parameters..." );
            PopulateCollection();

            IEnumerator enumerator = ((IEnumerable)_collection).GetEnumerator();
            enumerator.Reset();

            try
            {
                object foo = enumerator.Current;
                AddFailure( "Current shouldn't be accessible until MoveNext is called" );
            }
            catch ( InvalidOperationException )
            {
                Log( "  Good!  Invalid operation exception caught for accessing Current before MoveNext is called" );
            }

            try
            {
                while ( enumerator.MoveNext() );

                object foo = enumerator.Current;
                AddFailure( "Current shouldn't be accessible after MoveNext returns false" );
            }
            catch ( InvalidOperationException )
            {
                Log( "  Good!  Invalid operation exception caught for accessing Current after MoveNext returns false" );
            }

            try
            {
                foreach ( int o in _collection )
                {
                    _collection.Remove( o );
                }
                AddFailure( "Should not be able to modify collection and enumerate at the same time" );
            }
            catch ( InvalidOperationException )
            {
                Log( "  Good!  Invalid Operation exception caught for modify + enumerate collection at the same time" );
            }

            try
            {
                PopulateCollection();
                enumerator = _collection.GetEnumerator();
                enumerator.MoveNext();
                _collection.Remove( _objects[ 0 ] );
                enumerator.Reset();
                AddFailure( "Should not be able to modify collection and reset enumerator" );
            }
            catch ( InvalidOperationException )
            {
                Log( "  Good!  Invalid Operation exception caught for modify + reset enumerator" );
            }

            try
            {
                PopulateCollection();
                enumerator = ((IEnumerable)NewInt32Collection()).GetEnumerator();
                object foo = enumerator.Current;
                AddFailure( "Empty collections cannot have Current elements" );
            }
            catch ( InvalidOperationException )
            {
                Log( "  Good!  Invalid operation exception caught for accessing Current from empty collection" );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestEquals()
        {
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestGetAsFrozen()
        {
            Log( "Testing GetAsFrozenCore..." );

            PopulateCollection();

            // We clone the collection first so that the test objects are not permanently frozen
            Int32Collection copy = _collection.Clone();

            if ( object.ReferenceEquals( copy, _collection ) ||
                 !ObjectUtils.DeepEqualsToAnimatable( copy, _collection ) ||
                 failOnPurpose )
            {
                AddFailure( "Clone should always return a deep copy" );
            }

            Int32Collection frozen = (Int32Collection)copy.GetAsFrozen();
            if ( object.ReferenceEquals( frozen, copy ) ||
                 !ObjectUtils.DeepEqualsToAnimatable( frozen, copy ) ||
                 failOnPurpose )
            {
                AddFailure( "GetAsFrozen should return a deep copy if the collection is not frozen" );
            }

            copy.Freeze();
            frozen = (Int32Collection)copy.GetAsFrozen();
            if ( !object.ReferenceEquals( frozen, copy ) || failOnPurpose )
            {
                AddFailure( "GetAsFrozen should return a shallow copy if the collection is frozen" );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestGetCurrentValueAsFrozen()
        {
            Log( "Testing GetCurrentValueAsFrozen..." );

            PopulateCollection();

            // We clone the collection first so that the test objects are not permanently frozen
            Int32Collection copy = _collection.Clone();

            if ( object.ReferenceEquals( copy, _collection ) ||
                 !ObjectUtils.DeepEqualsToAnimatable( copy, _collection ) ||
                 failOnPurpose )
            {
                AddFailure( "Clone should always return a deep copy" );
            }

            Int32Collection frozen = (Int32Collection)copy.GetCurrentValueAsFrozen();
            if ( object.ReferenceEquals( frozen, copy ) ||
                 !ObjectUtils.DeepEqualsToAnimatable( frozen, copy ) ||
                 failOnPurpose )
            {
                AddFailure( "GetCurrentValueAsFrozen should return a deep copy if the collection is not frozen" );
            }

            copy.Freeze();
            frozen = (Int32Collection)copy.GetCurrentValueAsFrozen();
            if ( !object.ReferenceEquals( frozen, copy ) || failOnPurpose )
            {
                AddFailure( "GetCurrentValueAsFrozen should return a shallow copy if the collection is frozen" );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestIndexOf()
        {
            Log( "Testing IList.IndexOf( object )..." );
            PopulateList();

            for ( int i = 0; i < _list.Count; i++ )
            {
                int position = _list.IndexOf( _objects[ i ] );
                if ( position != i || failOnPurpose )
                {
                    AddFailure( "IList.IndexOf failed" );
                    Log( "*** Expected: {0} at position {1}", _objects[ i ], i );
                    Log( "***   Actual: {0} at position {1}", _objects[ i ], position );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestIndexOf2()
        {
            Log( "Testing IList.IndexOf( object ) with bad parameters..." );

            SafeExecute( IndexOfNullItem );
            SafeExecute( IndexOfWrongType );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region SafeExecutionBlocks for IndexOf

        private void            IndexOfNullItem()
        {
            PopulateList();
            int index = _list.IndexOf( null );
            if ( index != -1 || failOnPurpose )
            {
                AddFailure( "IndexOf should return -1 when null is passed in" );
                Log( "*** Actual: " + index );
            }
        }

        private void            IndexOfWrongType()
        {
            PopulateList();

            // We're okay with this so long as we don't have BooleanCollections
            int index = _list.IndexOf( true );
            if ( index != -1 || failOnPurpose )
            {
                AddFailure( "IndexOf should return -1 when wrong type is passed in" );
                Log( "*** Actual: " + index );
            }
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestInsert()
        {
            Log( "Testing Insert..." );
            PopulateList();

            int count = _list.Count;
            _list.Insert( 0, _objects[ _objects.Count-1 ] );

            if ( count+1 != _list.Count ||
                !_list[ 0 ].Equals( _objects[ _objects.Count-1 ] ) || failOnPurpose )
            {
                AddFailure( "Insert failed" );
                Log( "*** Expected: count = {0}, first object = {1}", count+1, _objects[ _objects.Count-1 ] );
                Log( "***   Actual: count = {0}, first object = {1}", _list.Count, _list[ 0 ] );
            }

            PopulateList();
            _list.Insert( count-1, _objects[ 0 ] );

            if ( count+1 != _list.Count ||
                !_list[ _list.Count-2 ].Equals( _objects[ 0 ] ) || failOnPurpose )
            {
                AddFailure( "Insert failed" );
                Log( "*** Expected: count = {0}, 2nd to last object = {1}", count+1, _objects[ 0 ] );
                Log( "***   Actual: count = {0}, 2nd to last object = {1}", _list.Count, _list[ _list.Count-2 ] );
            }

            Log( "Testing Int32Collection.Insert..." );
            PopulateCollection();

            count = _collection.Count;
            _collection.Insert( 0, _objects[ _objects.Count-1 ] );

            if ( count+1 != _collection.Count ||
                !_collection[ 0 ].Equals( _objects[ _objects.Count-1 ] ) || failOnPurpose )
            {
                AddFailure( "Insert failed" );
                Log( "*** Expected: count = {0}, first object = {1}", count+1, _objects[ _objects.Count-1 ] );
                Log( "***   Actual: count = {0}, first object = {1}", _collection.Count, _collection[ 0 ] );
            }

            PopulateCollection();
            _collection.Insert( count-1, _objects[ 0 ] );

            if ( count+1 != _collection.Count ||
                !_collection[ _collection.Count-2 ].Equals( _objects[ 0 ] ) || failOnPurpose )
            {
                AddFailure( "Insert failed" );
                Log( "*** Expected: count = {0}, 2nd to last object = {1}", count+1, _objects[ 0 ] );
                Log( "***   Actual: count = {0}, 2nd to last object = {1}", _collection.Count, _collection[ _collection.Count-2 ] );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestInsert2()
        {
            Log( "Testing IList.Insert with bad parameters..." );
            PopulateList();

            Try( ListInsertNull, typeof( ArgumentException ) );
            Try( ListInsertNegativeIndex, typeof( ArgumentOutOfRangeException ) );
            Try( ListInsertLargeIndex, typeof( ArgumentOutOfRangeException ) );

            Log( "Testing Int32Collection.Insert with bad parameters..." );
            PopulateCollection();

            // Skipping null tests for Int32Collection.Insert because it's a value-type
            Try( CollectionInsertNegativeIndex, typeof( ArgumentOutOfRangeException ) );
            Try( CollectionInsertLargeIndex, typeof( ArgumentOutOfRangeException ) );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region ExceptionThrowers for Insert

        private void            ListInsertNull()
        {
            _list.Insert( 0, null );
        }

        private void            ListInsertNegativeIndex()
        {
            _list.Insert( -1, _objects[ 0 ] );
        }

        private void            ListInsertLargeIndex()
        {
            _list.Insert( _objects.Count+1, _objects[ 0 ] );
        }

        // Skipping null tests for Int32Collection.Insert because it's a value-type

        private void            CollectionInsertNegativeIndex()
        {
            _collection.Insert( -1, _objects[ 0 ] );
        }

        private void            CollectionInsertLargeIndex()
        {
            _collection.Insert( _objects.Count+1, _objects[ 0 ] );
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestIsFixedSize()
        {
            Log( "Testing IsFixedSize..." );
            PopulateCollection();

            bool theirAnswer = ((IList)_collection).IsFixedSize;
            if ( theirAnswer != false || failOnPurpose )
            {
                AddFailure( "A growing collection should not have a fixed size" );
            }

            _collection.Freeze();
            theirAnswer = ((IList)_collection).IsFixedSize;
            if ( theirAnswer != true || failOnPurpose )
            {
                AddFailure( "A frozen collection should be fixed in size" );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestIsReadOnly()
        {
            Log( "Testing IsReadOnly..." );
            PopulateCollection();

            bool theirAnswer = ((IList)_collection).IsReadOnly;
            if ( theirAnswer != false || failOnPurpose )
            {
                AddFailure( "A growing collection should not be read-only" );
            }

            _collection.Freeze();
            theirAnswer = ((IList)_collection).IsReadOnly;
            if ( theirAnswer != true || failOnPurpose )
            {
                AddFailure( "A frozen collection should be read-only" );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestIsSynchronized()
        {
            Log( "Testing IsSynchronized..." );
            PopulateList();

            // Collections should always be synchronized because they are only accessible
            //   from the thread they were created on.
            if ( !_list.IsSynchronized )
            {
                AddFailure( "IList.IsSynchronized failed" );
                Log( "*** Expected: true" );
                Log( "***   Actual: {0}", _list.IsSynchronized );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestItem()
        {
            Log( "Testing IList.get/set_Item..." );
            PopulateList();

            _list[ 0 ] = _objects[ 3 ];
            if ( !_list[ 0 ].Equals( _objects[ 3 ] ) || failOnPurpose )
            {
                AddFailure( "IList.get/set_Item failed" );
                Log( "*** Expected: item[0] = {0}", _objects[ 3 ] );
                Log( "***   Actual: item[0] = {0}", _list[ 0 ] );
            }

            Log( "Testing Int32Collection.get/set_Item..." );
            PopulateCollection();

            _collection[ 0 ] = _objects[ 3 ];
            if ( !_collection[ 0 ].Equals( _objects[ 3 ] ) || failOnPurpose )
            {
                AddFailure( "Int32Collection.get/set_Item failed" );
                Log( "*** Expected: item[0] = {0}", _objects[ 3 ] );
                Log( "***   Actual: item[0] = {0}", _collection[ 0 ] );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestItem2()
        {
            Log( "Testing IList.get/set_Item with bad parameters..." );
            PopulateList();

            Try( ListGetItemNegativeIndex,  typeof( ArgumentOutOfRangeException ) );
            Try( ListGetItemIndexTooLarge,  typeof( ArgumentOutOfRangeException ) );
            Try( ListSetItemNegativeIndex,  typeof( ArgumentOutOfRangeException ) );
            Try( ListSetItemIndexTooLarge,  typeof( ArgumentOutOfRangeException ) );
            Try( ListSetItemNull,           typeof( ArgumentException ) );
            Try( ListSetItemWrongType,      typeof( ArgumentException ) );

            Log( "Testing Int32Collection.get/set_Item with bad parameters..." );
            PopulateCollection();

            Try( CollectionGetItemNegativeIndex, typeof( ArgumentOutOfRangeException ) );
            Try( CollectionGetItemIndexTooLarge, typeof( ArgumentOutOfRangeException ) );
            Try( CollectionSetItemNegativeIndex, typeof( ArgumentOutOfRangeException ) );
            Try( CollectionSetItemIndexTooLarge, typeof( ArgumentOutOfRangeException ) );
            // Skipping null tests for Int32Collection.set_Item( int, int ) because it's a value-type
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region ExceptionThrowers for get/set_Item

        private void            ListGetItemNegativeIndex()
        {
            object item = _list[ -1 ];
        }

        private void            ListGetItemIndexTooLarge()
        {
            object item = _list[ _objects.Count ];
        }

        private void            ListSetItemNegativeIndex()
        {
            _list[ -1 ] = _objects[ 0 ];
        }

        private void            ListSetItemIndexTooLarge()
        {
            _list[ _objects.Count ] = _objects[ 0 ];
        }

        private void            ListSetItemNull()
        {
            _list[ 0 ] = null;
        }

        private void            ListSetItemWrongType()
        {
            // We're okay with this so long as we don't have BooleanCollections
            _list[ 0 ] = true;
        }

        private void            CollectionGetItemNegativeIndex()
        {
            int item = _collection[ -1 ];
        }

        private void            CollectionGetItemIndexTooLarge()
        {
            int item = _collection[ _objects.Count ];
        }

        private void            CollectionSetItemNegativeIndex()
        {
            _collection[ -1 ] = (int)_objects[ 0 ];
        }

        private void            CollectionSetItemIndexTooLarge()
        {
            _collection[ _objects.Count ] = (int)_objects[ 0 ];
        }

        // Skipping null tests for Int32Collection.set_Item( int, int ) because it's a value-type

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestRemove()
        {
            Log( "Testing IList.Remove..." );
            PopulateList();

            int count = _list.Count;
            _list.Remove( _objects[ 0 ] );
            if ( count-1 != _list.Count || _list.IndexOf( _objects[ 0 ] ) != -1 ||
                failOnPurpose )
            {
                AddFailure( "IList.Remove Failed" );
                Log( "*** Expected: count = {0} and object should not be found", count-1 );
                if ( count-1 == _list.Count )
                {
                    Log( "***   Actual: count = {0} and object was not removed", _list.Count );
                }
                else
                {
                    Log( "***   Actual: count = {0}", _list.Count );
                }
            }

            Log( "Testing Int32Collection.Remove..." );
            PopulateCollection();

            _collection.Remove( _objects[ 3 ] );

            if ( _collection.Count != _objects.Count - 1 || failOnPurpose )
            {
                AddFailure( "Remove failed" );
                Log( "*** Expected: Count = {0}", _objects.Count - 1 );
                Log( "***   Actual: Count = {0}", _collection.Count );
            }
            // I trust that ArrayList did the rest correctly (plus it was verified above)
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestRemove2()
        {
            Log( "Testing IList.Remove with bad parameters..." );

            SafeExecute( ListRemoveNull );
            SafeExecute( ListRemoveWrongType );
            SafeExecute( ListRemoveNonExisting );
            SafeExecute( RemoveFromEmptyList );
            Try( ListRemoveFrozen, typeof( InvalidOperationException ) );

            Log( "Testing Int32Collection.Remove with bad parameters..." );

            // Skipping null tests for Int32Collection.Remove because it's a value-type
            SafeExecute( CollectionRemoveNonExisting );
            SafeExecute( RemoveFromEmptyCollection );
            Try( CollectionRemoveFrozen, typeof( InvalidOperationException ) );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region SafeExecutionBlocks for Remove

        private void            ListRemoveNull()
        {
            PopulateList();
            _list.Remove( null );
        }

        private void            ListRemoveWrongType()
        {
            PopulateList();
            _list.Remove( true );
        }

        private void            ListRemoveNonExisting()
        {
            PopulateList();
            _list.Remove( _objects[ 0 ] );
            _list.Remove( _objects[ 0 ] );
        }

        private void            RemoveFromEmptyList()
        {
            _list = NewInt32Collection();
            _list.Remove( _objects[ 0 ] );
            if ( _list.Count != 0 )
            {
                AddFailure( "IList.Remove failed with empty list" );
                Log( "*** A list with {0} elements is not empty", _list.Count );
            }
        }

        // Skipping null tests for Int32Collection.Remove because it's a value-type

        private void            CollectionRemoveNonExisting()
        {
            PopulateCollection();
            _collection.Remove( (int)_objects[ 0 ] );
            _collection.Remove( (int)_objects[ 0 ] );
        }

        private void            RemoveFromEmptyCollection()
        {
            _collection = NewInt32Collection();
            _collection.Remove( (int)_objects[ 0 ] );
            if ( _collection.Count != 0 )
            {
                AddFailure( "Int32Collection.Remove failed with empty collection" );
                Log( "*** A collection with {0} elements is not empty", _collection.Count );
            }
        }

        #endregion
        #region ExceptionThrowers for Remove

        private void            ListRemoveFrozen()
        {
            PopulateList();
            ((Freezable)_list).Freeze();
            _list.Remove( _objects[ 0 ] );
        }

        private void            CollectionRemoveFrozen()
        {
            PopulateCollection();
            _collection.Freeze();
            _collection.Remove( _objects[ 0 ] );
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestRemoveAt()
        {
            Log( "Testing RemoveAt..." );
            PopulateList();

            object[] myList = new object[ _list.Count ];

            _list.CopyTo( myList, 0 );
            _list.RemoveAt( 0 );
            myList[ 0 ] = null;

            VerifyRemoveAt( myList );

            PopulateList();
            _list.CopyTo( myList, 0 );
            _list.RemoveAt( _list.Count-1 );
            myList[ myList.Length-1 ] = null;

            VerifyRemoveAt( myList );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            VerifyRemoveAt( object[] myList )
        {
            int myIterator = 0;
            int theirIterator = 0;
            while ( theirIterator < _list.Count )
            {
                if ( myList[ myIterator ] == null )
                {
                    myIterator++;
                }
                if ( !_list[ theirIterator ].Equals( myList[ myIterator ] ) || failOnPurpose )
                {
                    AddFailure( "RemoveAt Failed" );
                    Log( "*** Expected: object #{0} = {1}", theirIterator, myList[ myIterator ] );
                    Log( "***   Actual: object #{0} = {1}", theirIterator, _list[ theirIterator ] );
                    break;
                }
                myIterator++;
                theirIterator++;
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestRemoveAt2()
        {
            Log( "Testing Int32Collection.RemoveAt with bad parameters..." );

            Try( RemoveAtNegativeIndex, typeof( ArgumentOutOfRangeException ) );
            Try( RemoveAtLargeIndex, typeof( ArgumentOutOfRangeException ) );
            Try( RemoveAtZeroIndexEmpty, typeof( ArgumentOutOfRangeException ) );
        }

        #region ExceptionThrowers for RemoveAt

        private void            RemoveAtNegativeIndex()
        {
            PopulateCollection();
            _collection.RemoveAt( -1 );
        }

        private void            RemoveAtLargeIndex()
        {
            PopulateCollection();
            _collection.RemoveAt( _objects.Count );
        }

        private void            RemoveAtZeroIndexEmpty()
        {
            _collection = NewInt32Collection();
            _collection.RemoveAt( 0 );
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestSyncRoot()
        {
            Log( "Testing SyncRoot..." );
            PopulateList();

            if ( !object.ReferenceEquals( _list.SyncRoot, _list ) )
            {
                // Please reference the comments for the IsSynchronized test.
                AddFailure( "IList.SyncRoot should return a reference to the caller since it is always synchronized" );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestToString()
        {
            Log( "Testing ToString..." );
            PopulateCollection();

            TestToStringWith( _collection );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestToStringWith( Int32Collection collection )
        {
            // Test the default (forwards to a call passing in CurrentCulture)

            string theirAnswer = collection.ToString();
            string myAnswer = string.Empty;
            foreach ( int obj in collection )
            {
                // I don't like calling into another Avalon method, but since test coverage
                //  for ToString should already exist for int, this is okay.

                myAnswer += obj.ToString();
                myAnswer += " ";
            }
            if ( collection.Count > 0 )
            {
                // remove trailing space
                myAnswer = myAnswer.Substring( 0, myAnswer.Length-1 );
            }
            if ( theirAnswer != myAnswer || failOnPurpose )
            {
                AddFailure( "ToString failed" );
                Log( "*** Expected: {0}", myAnswer );
                Log( "***   Actual: {0}", theirAnswer );
            }

            // Test with IFormatProvider parameter

            theirAnswer = collection.ToString( CultureInfo.CurrentCulture );
            myAnswer = string.Empty;
            foreach ( int obj in collection )
            {
                myAnswer += obj.ToString( CultureInfo.CurrentCulture );
                myAnswer += " ";
            }
            if ( collection.Count > 0 )
            {
                // remove trailing space
                myAnswer = myAnswer.Substring( 0, myAnswer.Length-1 );
            }
            if ( theirAnswer != myAnswer || failOnPurpose )
            {
                AddFailure( "ToString( IFormatProvider ) failed" );
                Log( "*** Expected: {0}", myAnswer );
                Log( "***   Actual: {0}", theirAnswer );
            }

            // One last test, since collection is IFormattable
            // This will trim doubles to 2 decimal places

            theirAnswer = ((IFormattable)collection).ToString( "N", CultureInfo.CurrentCulture.NumberFormat );
            myAnswer = string.Empty;
            foreach ( int obj in collection )
            {
                myAnswer += ((IFormattable)obj).ToString( "N", CultureInfo.CurrentCulture.NumberFormat );
                myAnswer += " ";
            }
            if ( collection.Count > 0 )
            {
                // remove trailing space
                myAnswer = myAnswer.Substring( 0, myAnswer.Length-1 );
            }
            if ( theirAnswer != myAnswer || failOnPurpose )
            {
                AddFailure( "IFormattable.ToString( string,IFormatProvider ) failed" );
                Log( "*** Expected: {0}", myAnswer );
                Log( "***   Actual: {0}", theirAnswer );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestToString2()
        {
            Log( "Testing ToString with bad parameters..." );

            TestToStringWith( NewInt32Collection() );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Int32Collection _collection;
        private IList _list;
        private List<int> _objects;
        private ArrayList _bogusCollection;
    }
}
