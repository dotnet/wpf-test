// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using System.Reflection;

namespace Microsoft.Test.DataServices
{
        public static class Helper
        {
            /// <summary>
            /// Compares current state of a collection view to a fresh collection view
            /// generated from the same data source.  Property name should be passed
            /// to be used for more detailed comparison if object to object comparison fails
            /// </summary>
            /// <param name="view">A collection view to be verified.</param>
            /// <param name="source">Collection source collection to be compared against.</param>
            /// <param name="propName">An existing property on a collection that comparison cares about.</param>
            /// <returns>TestResult</returns>
            public static TestResult CompareViewToSource(ICollectionView view, IEnumerable source, string propName)
            {
                TestResult result = TestResult.Pass;
                lock (source)
                {
                    DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                    if (ValidateCollections(view, source, propName))
                    {
                        result = TestResult.Pass;
                        GlobalLog.LogStatus("The view matches the collection");
                    }
                    else
                    {
                        result = TestResult.Fail;
                        GlobalLog.LogStatus("The view doesn't match the collection");
                    }
                    GlobalLog.LogStatus("Collection items count after validation is: " + source.Cast<object>().Count());
                }
                return result;
            }
            public static bool ValidateCollections(ICollectionView icv, IEnumerable ie, string propName)
            {
                // For validation, create a new CollectionViewSource, and tell it to do the same
                // things as our reference collection view is doing. We then give our reference
                // collection as it's source and it will now produce the appropriate view upon
                // the collection. Now the reference collection view and this new collection view
                // should match, if they don't it means the reference cv got confused.

                CollectionViewSource cvs = new CollectionViewSource();
                bool result = true;

                foreach (SortDescription sd in icv.SortDescriptions)
                {
                    cvs.SortDescriptions.Add(sd);
                }
                if (icv.CanFilter)
                {
                    cvs.Filter += delegate(object sender, FilterEventArgs e) { e.Accepted = icv.Filter != null ? icv.Filter(e.Item) : true; };
                }
                foreach (GroupDescription gd in icv.GroupDescriptions)
                {
                    cvs.GroupDescriptions.Add(gd);
                }
                cvs.Source = ie;

                // Attempting to compare entire view objects. If equal we'll stop here
                // if not, we'll look at the details in depth and make sure the passed
                // property is the same in both.
                if (icv.Cast<object>().SequenceEqual(cvs.View.Cast<object>()))
                {
                    GlobalLog.LogStatus("CollectionView has the same data as the Collection");
                    return result;
                }
                else
                {
                    IEnumerable<object> firstView = icv.Cast<object>();
                    IEnumerable<object> secondView = cvs.View.Cast<object>();
                    IEnumerator ie1 = firstView.GetEnumerator();
                    IEnumerator ie2 = secondView.GetEnumerator();
                    bool b1, b2;
                    int i = 0;
                    string viewType = icv.GetType().FullName;

                    for (b1 = ie1.MoveNext(), b2 = ie2.MoveNext(); b1 && b2; b1 = ie1.MoveNext(), b2 = ie2.MoveNext())
                    {
                        if (ie1.Current != ie2.Current)
                        {
                            object a = GetPropertyAsObject(icv.GetType(), ie1.Current, propName);
                            object b = GetPropertyAsObject(icv.GetType(), ie2.Current, propName);

                            if (a != null && b != null)
                            {
                                if (!a.Equals(b))
                                {
                                    GlobalLog.LogStatus(a + " is not the same as " + b + " at index " + i);
                                    result = false;
                                }
                            }
                            else
                            {
                                GlobalLog.LogStatus("Detailed validation is not supported for this CollectionView type: " + viewType);
                            }
                        }
                        else
                        {
                            GlobalLog.LogStatus("Values at index " + i + " match");
                        }
                        i++;
                    }
                    if (b1 || b2)
                    {
                        GlobalLog.LogStatus("The item count in view is: " + firstView.Count() + " but expecting: " + secondView.Count());
                        result = false;
                    }

                    //This is needed to workaround a bug in BindingListCollectionView
                    CollectionView cv = cvs.View as CollectionView;
                    if (cv != null) cv.DetachFromSourceCollection();
                    cvs.Source = null;

                    return result;
                }
            }
            /// <summary>
            /// Gets value of a property from an element as object
            /// This abstracts different collection view types and ways to get properties out
            /// </summary>
            /// <param name="viewType">A collection view to be verified.</param>
            /// <param name="o">Object - usually an element at a given index.</param>
            /// <param name="propName">An existing property in a view we are quering.</param>
            /// <returns>object</returns>
            public static object GetPropertyAsObject(Type viewType, object o, string propName)
            {
                return SetPropertyAsObject(viewType, o, propName,  null);
            }
            /// <summary>
            /// Similar to GetPropertyAsObject but first it sets the value to a given element
            /// and then get the value and returns it so that it can be verified as set.
            /// This abstracts different collection view types and ways to set properties.
            /// </summary>
            /// <param name="viewType">A collection view to be verified.</param>
            /// <param name="o">Object - usually an element at a given index.</param>
            /// <param name="propName">An existing property in a view we are quering.</param>
            /// <param name="value">Value to be set on a propName property.</param>
            /// <returns>object</returns>
            public static object SetPropertyAsObject(Type viewType, object o, string propName, object value)
            {
                switch (viewType.ToString())
                {
                    case "System.Windows.Data.ListCollectionView":
                    case "System.Windows.Controls.ItemCollection":
                        {
                            Type orderType = o.GetType();
                            PropertyInfo propInfo = orderType.GetProperty(propName);
                            if (propInfo == null)
                            {
                                throw new ApplicationException(string.Format("propInfo was null for {0}.{1}", orderType, propName));
                            }
                            if (value != null)
                            {
                                propInfo.SetValue(o,value,null);
                            }
                            return propInfo.GetValue(o, null);
                        }

                    case "System.Windows.Data.BindingListCollectionView":
                    {
                        if (value != null)
                        {
                            ((DataRowView)o)[propName] = value;
                        }
                        return ((DataRowView)o)[propName];
                    }

                    default:
                    {
                        GlobalLog.LogStatus("Unknown view type " + viewType.ToString());
                        return null;
                    }
                }
            }
        }
}


