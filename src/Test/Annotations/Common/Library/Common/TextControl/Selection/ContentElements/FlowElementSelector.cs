// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Base class for making selections relative to any flow element.
using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Collections.Generic;

namespace Avalon.Test.Annotations
{
    public abstract class FlowElementSelector
    {
        #region Constructor

        public FlowElementSelector(SelectionModule selectionMod)
        {            
            selectionModule = selectionMod;
        }

        #endregion

        #region Public Methods

        public object FindElement(Type elementType, string name)
        {
            TextPointer startPointer = StartOfDocument;
            TextPointer endPointer = EndOfDocument;

            //
            // As we walk through the text we will see each element twice, once when it begins and
            // once when it ends.  Therefore, keep track of which opening elements we've seen so that
            // we can distiguish between end elements and new opening ones.
            //
            IDictionary<object, bool> openElements = new Dictionary<object, bool>();
            while (startPointer.CompareTo(endPointer) != 0)
            {
                object element = startPointer.GetAdjacentElement(LogicalDirection.Forward);
                if (element != null && Type.Equals(elementType, element.GetType()))
                {                    
                    PropertyInfo nameProperty = element.GetType().GetProperty("Name");
                    if (nameProperty != null && string.Equals(nameProperty.GetValue(element, null), name))
                        return element;
                }
                startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            return null;
        }

        #endregion

        #region protected Methods

        protected void PrintStatus(string str)
        {
            // Only log when we are actually changing a selection.
            if (selectionModule.WriteThrough)
                TestSuite.Current.printStatus(str);
        }

        /// <summary>
        /// Find table with the given name.
        /// </summary>
        /// <param name="name">Name of Table element to find.</param>
        /// <returns>TextPointer just before the start of the Table with the given name.</returns>
        protected ElementPosition FindElementWithName(Type elementType, string name)
        {
            TextPointer startPointer = StartOfDocument;
            TextPointer endPointer = EndOfDocument;

            ElementPosition position = new ElementPosition();

            //
            // As we walk through the text we will see each element twice, once when it begins and
            // once when it ends.  Therefore, keep track of which opening elements we've seen so that
            // we can distiguish between end elements and new opening ones.
            //
            IDictionary<object, bool> openElements = new Dictionary<object, bool>();
            //int foundCount = 0;
            object match = null;
            while (startPointer.CompareTo(endPointer) != 0)
            {
                object element = startPointer.GetAdjacentElement(LogicalDirection.Forward);
                if (element != null && Type.Equals(elementType, element.GetType()))
                {
                    // If at the end of of target element, store end position and break.
                    if (match != null && element == match)
                    {
                        position.End = startPointer;
                        break;
                    }
                    // If at end of other element, pop element.
                    else if (openElements.ContainsKey(element))
                    {
                        openElements.Remove(element);
                    }
                    // otherwise, check to see if we are at the start of target element.
                    else
                    {
                        PropertyInfo nameProperty = element.GetType().GetProperty("Name");
                        if (nameProperty != null && string.Equals(nameProperty.GetValue(element, null), name))
                        {
                            match = element;
                            position.Start = startPointer;
                        }
                        openElements.Add(element, true);
                    }
                }
                startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            if (position.Start == null || position.End == null)
                throw new ArgumentException("Did not find element of type '" + elementType + "' with Name '" + name + "' in current document.");
            return position;
        }

        /// <summary>
        /// Returns the next element of type 'elementType' within the given range.
        /// </summary>        
        protected ElementPosition FindNextElement(Type elementType, TextPointer startPointer, TextPointer endPointer) 
        {
           return FindNthElement(elementType, startPointer, endPointer, 1);
        }

        /// <summary>
        /// Find the Nth element of the given type starting a given TextPointer.
        /// </summary>
        /// <param name="elementType">Type of element to find.</param>
        /// <param name="startPointer">Position in text to begin search.</param>
        /// <param name="endPointer">Position in text to end search.</param>
        /// <param name="occurance">Index of element to return, e.g. if 2, returns the 2nd element of specific type.</param>
        /// <returns>The Nth element of specified type from the given start point or null if </returns>
        protected ElementPosition FindNthElement(Type elementType, TextPointer startPointer, TextPointer endPointer, int occurance) 
        {
            if (occurance <= 0)
                throw new ArgumentException("Argument '{0}' must be >= 0.", "occurance");

            ElementPosition position = new ElementPosition();

            //
            // As we walk through the text we will see each element twice, once when it begins and
            // once when it ends.  Therefore, keep track of which opening elements we've seen so that
            // we can distiguish between end elements and new opening ones.
            //
            IDictionary<object, bool> openElements = new Dictionary<object, bool>();
            int foundCount = 0;
            object match = null;
            while (startPointer.CompareTo(endPointer) != 0)
            {
                object element = startPointer.GetAdjacentElement(LogicalDirection.Forward);
                if (element != null && Type.Equals(elementType, element.GetType()))
                {
                    // If at the end of of target element, store end position and break.
                    if (match != null && element == match)
                    {
                        position.End = startPointer;
                        break;
                    }
                    // If at end of other element, pop element.
                    else if (openElements.ContainsKey(element))
                    {
                        openElements.Remove(element);
                    }
                    // otherwise, check to see if we are at the start of target element.
                    else
                    {
                        foundCount++;
                        if (foundCount == occurance)
                        {
                            match = element;
                            position.Start = startPointer;
                        }
                        openElements.Add(element, true);
                    }
                }
                startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            if (position.Start == null || position.End == null)
                throw new ArgumentException("Could not find '" + occurance + "'th element of type '" + elementType + "' in specified range.");
            return position;
        }        

        /// <summary>
        /// Verify that the document we are about to act upon is valid.
        /// </summary>
        protected void VerifyDocument()
        {
            if (!(selectionModule.Document is FlowDocument))
                throw new NotSupportedException("TableSelector is only compatible with FlowDocument.");
        }

        protected TextPointer StartOfDocument
        {
            get
            {
                return (TextPointer)selectionModule.StartOfDocument;
            }
        }

        protected TextPointer EndOfDocument 
        {
            get 
            {
                return (TextPointer)selectionModule.EndOfDocument;
            }
        }

        protected TextPointer CreatePointer(ElementPosition element, PagePosition relativePosition)
        {
            TextPointer pointer = null;

            switch (relativePosition)
            {
                case PagePosition.Beginning:
                    pointer = element.Start;
                    break;
                case PagePosition.End:
                    pointer = element.End;
                    break;
                case PagePosition.Middle:
                    int midpoint = element.Start.GetOffsetToPosition(element.End) / 2;
                    pointer = (TextPointer)selectionModule.CreatePointer(element.Start, midpoint);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return pointer;
        }

        /// <summary>
        /// Create selection relative to given ElementPosition.
        /// </summary>
        protected TextRange Select(ElementPosition element, PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
        {
            object startPointer = CreatePointer(element, startPosition);
            object endPointer = CreatePointer(element, endPosition);
            startPointer = selectionModule.CreatePointer(startPointer, startOffset);
            endPointer = selectionModule.CreatePointer(endPointer, endOffset);
            return selectionModule.Select(startPointer, endPointer);
        }

        #endregion

        #region Fields
        
        protected SelectionModule selectionModule;

        #endregion
    }

    public class ElementPosition
    {
        public TextPointer Start;
        public TextPointer End;
    }
}	
