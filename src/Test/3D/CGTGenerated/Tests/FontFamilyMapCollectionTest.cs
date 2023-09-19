// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for FontFamilyMapCollection (generated)
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

    public partial class        FontFamilyMapCollectionTest : CoreGraphicsTest
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    Init( Variation v )
        {
            base.Init( v );

            _collection      = NewFontFamilyMapCollection();
            _list            = NewFontFamilyMapCollection();
            _objects         = new List<FontFamilyMap>();
            _bogusCollection = new ArrayList();

            _objects.Add( Const.FamilyMapLatin );
            _objects.Add( Const.FamilyMapHebrew );
            _objects.Add( Const.FamilyMapHindi );
            _objects.Add( Const.FamilyMapChinese );

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
                TestClear();
                TestContains();
                TestCopyTo();
                TestCount();
                TestEnumerator();
                TestEquals();
                TestInsert();
                TestItem();
                TestRemove();
                TestRemoveAt();
            }
            else // priority > 0
            {
                TestAdd2();
                TestContains2();
                TestEnumerator2();
                TestInsert2();
                TestItem2();
                TestRemoveAt2();
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private FontFamilyMapCollection NewFontFamilyMapCollection()
        {
            FontFamily ff = new FontFamily();
            return ff.FamilyMaps;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private FontFamilyMapCollection NewFontFamilyMapCollection( IEnumerable<FontFamilyMap> collection )
        {
            if ( collection is FontFamilyMapCollection )
            {
                return (FontFamilyMapCollection)collection;
            }
            else
            {
                FontFamily ff = new FontFamily();
                FontFamilyMapCollection col = ff.FamilyMaps;
                foreach ( FontFamilyMap map in collection )
                {
                    col.Add( map );
                }
                return col;
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private FontFamilyMapCollection NewFontFamilyMapCollection( int capacity )
        {
            FontFamily ff = new FontFamily();
            return ff.FamilyMaps;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            PopulateCollection()
        {
            _collection = NewFontFamilyMapCollection();
            foreach ( FontFamilyMap obj in _objects )
            {
                _collection.Add( obj );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private FontFamilyMap[] FontFamilyMapArray
        {
            get
            {
                FontFamilyMap[] array = new FontFamilyMap[ _objects.Count ];
                _objects.CopyTo( array );
                return array;
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            PopulateList()
        {
            _list = NewFontFamilyMapCollection();
            foreach ( FontFamilyMap obj in _objects )
            {
                _list.Add( obj );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConstructor()
        {
            Log( "Testing Constructor..." );

            FontFamilyMapCollection empty = NewFontFamilyMapCollection();
            if ( empty.Count != 0 || failOnPurpose )
            {
                AddFailure( "FontFamilyMapCollection.ctor() failed" );
                Log( "*** Expected: Default constructor creates empty collection" );
                Log( "*** Actual:   {0} objects in \"empty\" collection", empty.Count );
            }

            PopulateCollection();
            FontFamilyMapCollection theirAnswer = NewFontFamilyMapCollection( _collection );

            if ( theirAnswer.Count != _objects.Count || failOnPurpose )
            {
                AddFailure( "FontFamilyMapCollection.ctor( IEnumerable<FontFamilyMap> ) failed" );
                Log( "*** Expected: {0} objects in my collection", _objects.Count );
                Log( "*** Actual:   {0} objects in their collection", theirAnswer.Count );
            }
            else
            {
                for ( int i = 0; i < theirAnswer.Count; i++ )
                {
                    if ( !theirAnswer[ i ].Equals( _objects[ i ] ) || failOnPurpose )
                    {
                        AddFailure( "FontFamilyMapCollection.ctor( IEnumerable<FontFamilyMap> ) failed" );
                        Log( "*** Expected: Collection[{0}] = {1}", i, _collection[ i ] );
                        Log( "*** Actual:   Collection[{0}] = {1}", i, theirAnswer[ i ] );
                        break;
                    }
                }
            }

            TestConstructorWith( new List<FontFamilyMap>() );
            TestConstructorWith( new FakeEnumerable() );
            TestConstructorWith( new NonCollectionEnumerable() );

            TestConstructorWith( 0 );
            TestConstructorWith( 5 );
            TestConstructorWith( 50 );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConstructorWith( IEnumerable<FontFamilyMap> e )
        {
            FontFamilyMapCollection theirAnswer = NewFontFamilyMapCollection( e );

            if ( theirAnswer.Count != 0 || failOnPurpose )
            {
                AddFailure( "FontFamilyMapCollection.ctor( IEnumerable<FontFamilyMap> ) failed" );
                Log( "*** Expected: Count == 0" );
                Log( "*** Actual:   Count == {0}", theirAnswer.Count );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConstructorWith( int capacity )
        {
            FontFamilyMapCollection theirAnswer = NewFontFamilyMapCollection( capacity );

            if ( theirAnswer.Count != 0 || failOnPurpose )
            {
                AddFailure( "FontFamilyMapCollection.ctor( int ) failed" );
                Log( "*** Expected: Count == 0" );
                Log( "*** Actual:   Count == {0}", theirAnswer.Count );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region FakeEnumerable

        private class           FakeEnumerable : IEnumerable<FontFamilyMap>, ICollection<FontFamilyMap>
        {
            public void         Add( FontFamilyMap p )
            {
            }
            public void         Clear()
            {
            }
            public bool         Contains( FontFamilyMap p )
            {
                return false;
            }
            public void         CopyTo( FontFamilyMap[] points, int index )
            {
            }
            public bool         Remove( FontFamilyMap p )
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
            public IEnumerator<FontFamilyMap> GetEnumerator()
            {
                return new Enumerator();
            }
            IEnumerator         IEnumerable.GetEnumerator()
            {
                return new Enumerator();
            }

            private class       Enumerator : IEnumerator<FontFamilyMap>
            {
                public FontFamilyMap Current
                {
                    get { return null; }
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
                    get { return null; }
                }
            }
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region NonCollectionEnumerable

        private class           NonCollectionEnumerable : IEnumerable<FontFamilyMap>
        {
            public IEnumerator<FontFamilyMap> GetEnumerator()
            {
                return new Enumerator();
            }
            IEnumerator         IEnumerable.GetEnumerator()
            {
                return new Enumerator();
            }

            private class       Enumerator : IEnumerator<FontFamilyMap>
            {
                public FontFamilyMap Current
                {
                    get { return null; }
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
                    get { return null; }
                }
            }
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestAdd()
        {
            Log( "Testing Add..." );
            _list = NewFontFamilyMapCollection();
            _collection = NewFontFamilyMapCollection();

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

            _collection.Add( (FontFamilyMap)o );

            if ( oldCount+1 != _collection.Count || position != oldCount ||
                    !_collection[ position ].Equals( (FontFamilyMap)o ) || failOnPurpose )
            {
                AddFailure( "FontFamilyMapCollection.Add failed" );
                Log( "*** Expected: count = {0}, position = {1}, FontFamilyMap added = {2}", oldCount+1, oldCount, o );
                Log( "***   Actual: count = {0}, position = {1}, FontFamilyMap added = {2}", _collection.Count, position, _collection[ position ] );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestAdd2()
        {
            Log( "Testing IList.Add( object ) with bad parameters..." );

            foreach ( object o in _bogusCollection )
            {
                Type itemType = ( o == null ) ? null : o.GetType();
                Type addType = typeof( FontFamilyMap );
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

            Log( "Testing FontFamilyMapCollection.Add with bad parameters..." );

            Try( AddNullToCollection, typeof( ArgumentException ) );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region ExceptionThrowers for Add

        private void            AddNullToList()
        {
            _list = NewFontFamilyMapCollection();
            _list.Add( null );
        }


        private void            AddNullToCollection()
        {
            _collection = NewFontFamilyMapCollection();
            _collection.Add( null );
        }

        #endregion

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
            _list = NewFontFamilyMapCollection();
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

            foreach ( FontFamilyMap obj in _objects )
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

            Log( "Testing FontFamilyMapCollection.Contains..." );
            PopulateCollection();

            foreach ( FontFamilyMap obj in _objects )
            {
                if ( !_collection.Contains( obj ) || failOnPurpose )
                {
                    AddFailure( "FontFamilyMapCollection.Contains failed" );
                    Log( "Could not locate {0} in my collection", obj );
                }
            }

            _collection.Remove( _objects[ 0 ] );
            if ( _collection.Contains( _objects[ 0 ] ) || failOnPurpose )
            {
                AddFailure( "FontFamilyMapCollection.Contains failed" );
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
                Type listType = typeof( FontFamilyMap );
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

            Log( "Testing FontFamilyMapCollection.CopyTo..." );
            PopulateCollection();
            FontFamilyMap[] typedArray = new FontFamilyMap[ _objects.Count ];
            _collection.CopyTo( typedArray, 0 );

            for ( int i = 0; i < _collection.Count; i++ )
            {
                if ( !_collection[ i ].Equals( typedArray[ i ] ) || failOnPurpose )
                {
                    AddFailure( "CopyTo( FontFamilyMap[],int ) failed" );
                    Log( "*** Expected: {0}", _collection[ i ] );
                    Log( "***   Actual: {0}", typedArray[ i ] );
                    break;
                }
            }
        }

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
            Log( "Testing FontFamilyMapEnumerator..." );
            PopulateCollection();

            TestEnumeratorWith( NewFontFamilyMapCollection() );
            TestEnumeratorWith( _collection );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestEnumeratorWith( FontFamilyMapCollection collection )
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

        private void            TestMoveNextWith( IEnumerator enumerator, FontFamilyMapCollection collection )
        {
            int count = 0;
            while ( enumerator.MoveNext() )
            {
                FontFamilyMap current = (FontFamilyMap)enumerator.Current;
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
                PopulateCollection();
                enumerator = ((IEnumerable)NewFontFamilyMapCollection()).GetEnumerator();
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

            Log( "Testing FontFamilyMapCollection.Insert..." );
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

            Log( "Testing FontFamilyMapCollection.Insert with bad parameters..." );
            PopulateCollection();

            Try( CollectionInsertNull, typeof( ArgumentException ) );
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


        private void            CollectionInsertNull()
        {
            _collection.Insert( 0, null );
        }

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

            Log( "Testing FontFamilyMapCollection.get/set_Item..." );
            PopulateCollection();

            _collection[ 0 ] = _objects[ 3 ];
            if ( !_collection[ 0 ].Equals( _objects[ 3 ] ) || failOnPurpose )
            {
                AddFailure( "FontFamilyMapCollection.get/set_Item failed" );
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

            Log( "Testing FontFamilyMapCollection.get/set_Item with bad parameters..." );
            PopulateCollection();

            Try( CollectionGetItemNegativeIndex, typeof( ArgumentOutOfRangeException ) );
            Try( CollectionGetItemIndexTooLarge, typeof( ArgumentOutOfRangeException ) );
            Try( CollectionSetItemNegativeIndex, typeof( ArgumentOutOfRangeException ) );
            Try( CollectionSetItemIndexTooLarge, typeof( ArgumentOutOfRangeException ) );
            Try( CollectionSetItemNull, typeof( ArgumentException ) );
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
            FontFamilyMap item = _collection[ -1 ];
        }

        private void            CollectionGetItemIndexTooLarge()
        {
            FontFamilyMap item = _collection[ _objects.Count ];
        }

        private void            CollectionSetItemNegativeIndex()
        {
            _collection[ -1 ] = (FontFamilyMap)_objects[ 0 ];
        }

        private void            CollectionSetItemIndexTooLarge()
        {
            _collection[ _objects.Count ] = (FontFamilyMap)_objects[ 0 ];
        }


        private void            CollectionSetItemNull()
        {
            _collection[ 0 ] = (FontFamilyMap)null;
        }

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

            Log( "Testing FontFamilyMapCollection.Remove..." );
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
            Log( "Testing FontFamilyMapCollection.RemoveAt with bad parameters..." );

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
            _collection = NewFontFamilyMapCollection();
            _collection.RemoveAt( 0 );
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private FontFamilyMapCollection _collection;
        private IList _list;
        private List<FontFamilyMap> _objects;
        private ArrayList _bogusCollection;
    }
}