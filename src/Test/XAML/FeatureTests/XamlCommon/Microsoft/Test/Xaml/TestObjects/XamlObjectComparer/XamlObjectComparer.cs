// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xaml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Utilities;

    /// <summary>
    /// Xaml object compare utility
    /// </summary>
    public static class XamlObjectComparer
    {
        /// <summary>
        /// readonly lablel
        /// </summary>
        public static readonly string Label = "XamlObjectComparer";

        /// <summary>
        /// Compare objects and attached properties
        /// </summary>
        /// <param name="source">source object instance</param>
        /// <param name="target">target object instance</param>
        public static void CompareObjectsAndAttachedProperties(object source, object target)
        {
            CompareObjects(source, target);

            Tracer.Trace("XamlObjectComparer", "Comparing attached properties");

            int sourcePropsCount = AttachablePropertyServices.GetAttachedPropertyCount(source);
            int targetPropsCount = AttachablePropertyServices.GetAttachedPropertyCount(target);

            if (sourcePropsCount != targetPropsCount)
            {
                throw new DataTestException("Attached properties collections are of different lengths.");
            }

            if (sourcePropsCount == 0)
            {
                return;
            }

            var sourceArray = new KeyValuePair<AttachableMemberIdentifier, object>[sourcePropsCount];
            var targetArray = new KeyValuePair<AttachableMemberIdentifier, object>[targetPropsCount];

            AttachablePropertyServices.CopyPropertiesTo(source, sourceArray, 0);
            AttachablePropertyServices.CopyPropertiesTo(target, targetArray, 0);

            foreach (var attachedProp in sourceArray)
            {
                if (attachedProp.Value is Hashtable)
                {
                    CompareHashtables(
                        (Hashtable)attachedProp.Value,
                        (Hashtable)targetArray.FirstOrDefault(elem => elem.Key == attachedProp.Key).Value);
                }
                else
                {
                    CompareObjects(
                        attachedProp.Value,
                        targetArray.FirstOrDefault(elem => elem.Key == attachedProp.Key).Value);
                }
            }
        }

        /// <summary>
        /// Compare objects
        /// </summary>
        /// <param name="source">source object instance</param>
        /// <param name="target">target object instance</param>
        public static void CompareObjects(object source, object target)
        {
            CompareResult result = TreeComparer.CompareLogical(source, target);
            if (result == CompareResult.Different)
            {
                string sourceString = new ObjectDumper().DumpToString(string.Empty, source);
                string targetString = new ObjectDumper().DumpToString(string.Empty, target);

                Tracer.Trace(XamlObjectComparer.Label, "Two objects are different.");
                Tracer.Trace(XamlObjectComparer.Label, "Source object dump:");
                Tracer.Trace(XamlObjectComparer.Label, sourceString);
                Tracer.Trace(XamlObjectComparer.Label, "Target object dump:");
                Tracer.Trace(XamlObjectComparer.Label, targetString);

                throw new DataTestException("Two Xaml Objects are different.");
            }
        }

        /// <summary>
        /// Compare two hashtables
        /// </summary>
        /// <param name="source">source hashtable</param>
        /// <param name="target">target hashtable</param>
        private static void CompareHashtables(Hashtable source, Hashtable target)
        {
            if (source.Count != target.Count)
            {
                throw new DataTestException("Hashtables are of different lengths");
            }

            foreach (object key in source.Keys)
            {
                CompareObjects(source[key], target[key]);
            }
        }
    }
}
