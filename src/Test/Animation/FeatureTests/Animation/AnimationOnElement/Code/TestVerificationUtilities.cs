// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//  Summary:  This contains the means for checking a set of multiple runs
//      against a previous well known baseline.
//
// $Id:$ $Change:$
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Markup;

using MS.Internal;


namespace Microsoft.Test.Animation
{
    /// <summary>
    /// Provides a way to filter results by categories and
    /// compare regressions of multiple part tests.
    /// </summary>
    public class CategoryFilter
    {
        /// <summary>
        /// Creates a category filter
        /// </summary>
        public CategoryFilter()
        {
            // build hashtable
            _categories = new Hashtable();
        }

        /// <summary>
        /// Read-Only Error string
        /// </summary>
        public string LastError
        {
            get { return CategoryFilter.s_lastError; }
        }

        /// <summary>
        /// Creates a category
        /// <param name="category">A string used to identify the new category</param>
        /// </summary>
        public void RegisterCategory( string category )
        {
            // don't register twice ...
            if ( !_categories.ContainsKey( category ) )
            {
                _categories.Add( category, new ArrayList() );
            }
        }

        /// <summary>
        /// Adds an item to the appropriate category
        /// <param name="category">A string used to identify the new category</param>
        /// <param name="item">An item to include in this category</param>
        /// </summary>
        public void AddItemToCategory( string category, object item )
        {
            ((ArrayList)_categories[ category ]).Add( item );
        }

        /// <summary>
        /// Registers a category and populates it with a list of items
        /// <param name="category">A string used to identify the new category</param>
        /// <param name="items">An array of items to include in this new category</param>
        /// </summary>
        public void AddCompleteCategory( string category, object[] items )
        {
            // register category
            RegisterCategory( category );
            // populate it
            foreach ( object item in items )
            {
                AddItemToCategory( category, item );
            }
        }

        /// <summary>
        /// Formats the categories to a list of strings
        /// <param name="categorySeparator">A string used to differentiate the category form its memebers</param>
        /// <param name="itemSeparator">A string used to differentiate each category item</param>
        /// <returns>A list of strings representing the current categories</returns>
        /// </summary>
        public string[] FormatCategories( string categorySeparator, string itemSeparator)
        {
            // build result
            string[] result = new string[ _categories.Count ];
            ArrayList formatCat = new ArrayList();
            formatCat.AddRange( _categories.Keys );
            formatCat.Sort( new _CompareStr() );
            // populate result
            int index = 0;
            foreach ( object key in formatCat )
            {
                // build category header
                result[index] = (string)key;
                result[index] += " (count=" + ((ArrayList)_categories[ (string)key ]).Count + ") " + categorySeparator;
                // append all items for this category
                foreach ( object value in (ArrayList)_categories[ (string)key ] )
                {
                    result[index] += ((string)value + itemSeparator) ;
                }
                // next string item
                index++;
            }
            return result;
        }


        /// <summary>
        /// Generates a code friendly string the categories to a list of strings
        /// <param name="constantTranslator">A Hash table used to map current string values to named constants</param>
        /// <param name="objectName">A string used for the declared value</param>
        /// <returns>A list of strings representing the C# code used
        /// to generate an object equivalent to this one</returns>
        /// </summary>
        public string[] CodeGenerateCategories( Hashtable constantTranslator, string objectName )
        {
            // build result
            string[] result = new string[ _categories.Count ];
            // init constants
            int index = 0;

            // create helper formatters
            ArrayList formatCat = new ArrayList();
            formatCat.AddRange( _categories.Keys );
            formatCat.Sort( new _CompareStr() );

            // populate result
            foreach ( object key in formatCat )
            {
                string singleCat = objectName + ".AddCompleteCategory( ";
                singleCat += String.Format("{0,-40} /* count={1,-5} */, new string[] {{  ", constantTranslator[ key ], ((ArrayList)_categories[ (string)key ]).Count );
                // append all items for this category
                foreach ( object value in (ArrayList)_categories[ (string)key ] )
                {
                    singleCat += "\"" + (string)value + "\", " ;
                }
                result[index++] = singleCat.Substring( 0, singleCat.Length-2  ) + "} );" ;
            }
            return result;
        }

        /// <summary>
        /// Generates a code friendly string the categories to a list of strings
        /// <param name="constantTranslator">A Hash table used to map current string values to named constants</param>
        /// <returns>A list of strings representing the C# code used
        /// to generate an object equivalent to this one</returns>
        /// </summary>
        public string[] CodeGenerateCategories( Hashtable constantTranslator )
        {
            return CodeGenerateCategories( constantTranslator, "regressionCheck" );
        }



        /// <summary>
        /// Overloaded == operator
        /// </summary>
        public static bool operator == ( CategoryFilter lhs, CategoryFilter rhs)
        {
            s_lastError = "Error at OP == null validation." ;
            if ( ((object)lhs) == null && ((object)rhs) == null ) return true;
            if ( ((object)lhs) == null ) return false;
            if ( ((object)rhs) == null ) return false;
            s_lastError = "No Error." ;
            return lhs.Equals( rhs );
        }

        /// <summary>
        /// Overloaded != operator
        /// </summary>
        public static bool operator != ( CategoryFilter lhs, CategoryFilter rhs)
        {
            return !( lhs == rhs );
        }


        /// <summary>
        /// Overloaded Equals
        /// </summary>
        public override bool Equals( object o )
        {
            if( o == null )
            {
                s_lastError = "Compared object is null." ;
                return false;
            }

            CategoryFilter lhs = (CategoryFilter)this;
            CategoryFilter rhs = (CategoryFilter)o;
            // check size equality
            if ( lhs._categories.Count != rhs._categories.Count )
            {
                s_lastError = "Category count is different: lhs=" + lhs._categories.Count + " rhs=" +  rhs._categories.Count ;
                return false;
            }
            // check category equality
            if ( lhs._categories.Keys.Count != rhs._categories.Keys.Count )
            {
                s_lastError = "Category KEY count is different: lhs=" + lhs._categories.Keys.Count + " rhs=" +  rhs._categories.Keys.Count ;
                return false;
            }

            ArrayList catLHS = new ArrayList();
            ArrayList catRHS = new ArrayList();
            catLHS.AddRange( lhs._categories.Keys );
            catRHS.AddRange( rhs._categories.Keys );
            // Sort Categories
            catLHS.Sort( new _CompareStr() );
            catRHS.Sort( new _CompareStr() );
            for ( int i=0; i<catLHS.Count; i++ )
            {
                if ( catLHS[i] != catRHS[i] )
                {
                    s_lastError = "Sorted categories are different at index=" + i + " lhs=" + catLHS[i] + " rhs=" + catRHS[i];
                    return false;
                }
            }
            // check item equality
            ArrayList itemLHS;
            ArrayList itemRHS;
            for ( int j=0; j<catLHS.Count; j++ )
            {
                itemLHS = (ArrayList)lhs._categories[ catLHS[j] ] ;
                itemRHS = (ArrayList)rhs._categories[ catLHS[j] ] ;
                itemLHS.Sort( new _CompareStr() );
                itemRHS.Sort( new _CompareStr() );
                // check sizes
                if ( itemLHS.Count != itemRHS.Count )
                {
                    s_lastError = "Categories (" + catLHS[j] + ") at index=" + j + " have different count lhs=" + itemLHS.Count + " rhs=" + itemRHS.Count;
                    return false;
                }
                // check items
                for ( int k=0; k<itemLHS.Count; k++ )
                {
                    if ( itemLHS[k].ToString() != itemRHS[k].ToString() )
                    {
                        s_lastError = "Category (" + catLHS[j] + "), items at index=" + k + " differ. lhs=[" + itemLHS[k] + "] rhs=[" + itemRHS[k] + "]";
                        return false;
                    }
                }
            }
            // all is ok, they are equal
            s_lastError = "No Error." ;
            return true;
        }

        /// <summary>
        /// Overloaded GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return _categories.GetHashCode();
        }

        /// <summary>
        /// Reports test summary and generates regression checks
        /// <param name="cf">Category filter object to print</param>
        /// <param name="fileName">name of file to use</param>
        /// <param name="constantTranslator">A Hash table used to map current string values to named constants</param>
        /// <param name="objectName">name of the element to regress</param>
        /// </summary>
        public static void PrintRegresionCodeToFile( CategoryFilter cf, string fileName, Hashtable constantTranslator, string objectName )
        {
            // Only do this when a valid filenmae and CF object are given
            if ( cf != null && fileName != null )
            {
                TestLogFile fileLog = new TestLogFile(fileName);
                string indent = "                ";
                string[] code = cf.CodeGenerateCategories(constantTranslator,"cf");
                // Log generated code warning
                fileLog.Log2( indent + "// ### GENERATED CODE " + DateTime.Now.ToLongDateString()  + "  - " + DateTime.Now.ToLongTimeString() + " ### //" ) ;
                fileLog.Log2(indent + "case " + "\"" + objectName + "\":" );
                indent += "    " ;
                foreach( string cd in code )
                {
                    fileLog.Log2(indent + cd);
                }
                fileLog.Log2(indent + "break;" );
            }
        }

        /// <summary>
        /// Reports test summary and generates regression checks
        /// <param name="cf">Category filter object to print</param>
        /// <param name="fileName">name of file to use</param>
        /// <param name="constantTranslator">A Hash table used to map current string values to named constants</param>
        /// </summary>
        public static void PrintRegresionCodeToFile( CategoryFilter cf, string fileName, Hashtable constantTranslator )
        {
            PrintRegresionCodeToFile( cf, fileName, constantTranslator, "regressionCheck" );
        }

        /// <summary>
        /// Compares two category filters using a flexible model.
        /// </summary>
        /// <param name="lhs">lhs object to compare</param>
        /// <param name="rhs">rhs object to compare</param>
        /// <param name="lhsName">lhs object print name</param>
        /// <param name="rhsName">rhs object print name</param>
        /// <param name="criticalCategories">array of categories that
        /// MUST have a perfect match</param>
        /// <param name="detailedLog">Returend log with differences
        /// and warnings, critical or otherwise</param>
        /// <returns>true if they categories are equal in the critical items,
        /// false otherwise.</returns>
        public static bool BiasCompare(
            CategoryFilter lhs,
            CategoryFilter rhs,
            string lhsName,
            string rhsName,
            string[] criticalCategories,
            out string[] detailedLog )
        {
            // asume success
            bool finalResult = true;
            // warnings issued
            int warningCount = 0;
            // intialize log
            ArrayList log = new ArrayList();
            Hashtable criticals = new Hashtable();

            // Check for critical categories in both sides
            foreach ( string criticalCat in criticalCategories )
            {
                if ( !lhs._categories.ContainsKey(criticalCat) )
                {
                    log.Add( String.Format("{1}: Missing critical category key '{0}'", criticalCat, lhsName ) ) ;
                    finalResult &= false;
                }
                if ( !rhs._categories.ContainsKey(criticalCat) )
                {
                    log.Add( String.Format("{1}: Missing critical category key '{0}'", criticalCat, rhsName ) ) ;
                    finalResult &= false;
                }
                criticals.Add( criticalCat, criticalCat );
            }

            // check size equality
            if ( lhs._categories.Count != rhs._categories.Count )
            {
                log.Add( String.Format("Category count is different: {0}={1}  {2}={3}", lhsName, lhs._categories.Count, rhsName, rhs._categories.Count ) );
                warningCount++;
            }
            // check category size equality
            if ( lhs._categories.Keys.Count != rhs._categories.Keys.Count )
            {
                log.Add( String.Format("Category KEY count is different: {0}={1}  {2}={3}", lhsName, lhs._categories.Keys.Count, rhsName, rhs._categories.Keys.Count ) );
                warningCount++;
            }
            // check category equality
            foreach( string lcat in lhs._categories.Keys )
            {
                if ( !rhs._categories.ContainsKey(lcat) )
                {
                    log.Add( String.Format("{1}: Missing category key '{0}'", lcat, rhsName ) ) ;
                    warningCount++;
                }
            }
            foreach( string rcat in rhs._categories.Keys )
            {
                if ( !lhs._categories.ContainsKey(rcat) )
                {
                    log.Add( String.Format("{1}: Missing category key '{0}'", rcat, lhsName ) ) ;
                    warningCount++;
                }
            }

            // Get the largest subset that we can compare
            ICollection keys;
            if ( lhs._categories.Keys.Count > rhs._categories.Keys.Count )
            {
                keys = lhs._categories.Keys;
            }
            else
            {
                keys = rhs._categories.Keys;
            }
            // test items
            foreach ( string cat in keys )
            {
                if ( lhs._categories.ContainsKey(cat) && rhs._categories.ContainsKey(cat) )
                {
                    Hashtable catItems = new Hashtable();

                    // add all left items
                    foreach( string s in (ArrayList)lhs._categories[ cat ] )
                    {
                        catItems.Add( s, 1 );
                    }
                    // add right items, if not already there
                    foreach( string s in (ArrayList)rhs._categories[ cat ] )
                    {
                        if ( catItems.ContainsKey(s) )
                        {
                            catItems[s] = 0;
                        }
                        else
                        {
                            catItems.Add( s, -1 );
                        }
                    }
                    // report
                    foreach( string key in catItems.Keys )
                    {
                        int catValue = (int) catItems[key];
                        if ( catValue != 0 )
                        {
                            if ( catValue > 0)
                            {
                                log.Add( String.Format( "Category '{0}' has missing item for '{1}' --> {2} ", cat, rhsName, key )) ;
                            }
                            else
                            {
                                log.Add( String.Format( "Category '{0}' has missing item for '{1}' --> {2} ", cat, lhsName, key )) ;
                            }
                            // log
                            if ( criticals.ContainsKey( cat ) )
                            {
                                finalResult &= false;
                            }
                            else
                            {
                                warningCount++;
                            }
                        }
                    }
                }
                else
                {
                    log.Add( String.Format("ITEMS: Cannot compare category with key '{0}'", cat ) ) ;
                    warningCount++;
                }
            }
            if ( warningCount > 0 )
            {
                log.Add("TOTAL WARNINGS: " + warningCount.ToString() );
            }
            // fill output
            object[] logobj = log.ToArray();
            detailedLog = new string[logobj.Length];
            for ( int i=0; i<logobj.Length; i++ )
            {
                detailedLog[i] = logobj[i].ToString();
            }
            return finalResult;
        }

        /// <summary>
        /// Internal class used for array sorting based on string conversion
        /// </summary>
        private class _CompareStr : System.Collections.IComparer
        {
            /// <summary>
            /// ICompare basic implementation
            /// </summary>
            /// <param name="x">lhs object to compare</param>
            /// <param name="y">rhs object to compare</param>
            /// <returns>0, 1 or -1 depending on comparison result</returns>
            public int Compare( object x, object y )
            {
                return string.Compare(  x.ToString(), y.ToString() );
            }
        }

        /// <summary>
        /// Internal dictionary of categories
        /// </summary>
        private Hashtable _categories ;

        /// <summary>
        /// Internal extensive error description
        /// </summary>
        private static string s_lastError ;
    }


    /// <summary>
    /// Interface used for regression verification
    /// </summary>
    public interface IRegressionVerify
    {
        /// <summary>
        /// Category filter for comparing expected results
        /// </summary>
        CategoryFilter ExpectedResults { set; }
        /// <summary>
        /// file name for regresion verification file
        /// </summary>
        string RegressionVerificationFile { set; }
    }
}
