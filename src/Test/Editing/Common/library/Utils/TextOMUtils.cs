// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides methods to access Text object model

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 10 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Common/Library/Utils/TextOMUtils.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Runtime.InteropServices;
    using Test.Uis.IO;
    using System.IO;
    using Test.Uis.Utils;
    using Test.Uis.Management;

    #endregion Namespaces.

    /// <summary>
    /// Provides methods to access Text container
    /// </summary>
    public static class TextOMUtils
    {
        #region Public methods
        /// <summary>
        /// count how many same type embedded object in a range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int EmbeddedObjectCountInRange(TextRange range)
        {
            TextOMUtils.NullValidation(range, "range");
            return EmbeddedObjectsInRange(range).Count;
        }
        /// <summary>
        /// Count the specific embedded object in a range
        /// </summary>
        /// <param name="range"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static int EmbeddedObjectCountInRange(TextRange range, Type objType)
        {
            int result = 0; 
            TextOMUtils.NullValidation(range, "range");
            ArrayList aList = EmbeddedObjectsInRange(range);
            for (int i = 0; i < aList.Count; i++)
            {
                if(aList[i].GetType() == objType)
                {
                    result++;
                }
            }
            return result; 
        }
        /// <summary>
        /// Find the collection of Embedded object in a Range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static ArrayList EmbeddedObjectsInRange(TextRange range)
        {
            TextPointer pointer;
            DependencyObject uiContainer;
            ArrayList aList;
            UIElement element;

            element = null; 
            aList = new ArrayList();
            pointer = range.Start;

            while (pointer!=null && pointer.GetOffsetToPosition(range.End) >0)
            {
                uiContainer = pointer.GetAdjacentElement(LogicalDirection.Forward);
                if(uiContainer is InlineUIContainer)
                {
                    element= ((InlineUIContainer)uiContainer).Child;
                }
                else if(uiContainer is BlockUIContainer)
                {
                    element = ((BlockUIContainer)uiContainer).Child;
                }
                if (element != null && !aList.Contains(element))
                {
                    aList.Add(element);
                }

                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            return aList;

        }

        /// <summary>
        /// Check to see if an UIElement is in a range
        /// </summary>
        /// <param name="range"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsEmbeddedObjectInRange(TextRange range, UIElement element)
        {
            TextOMUtils.NullValidation(element, "element");

            return EmbeddedObjectsInRange(range).Contains(element);
        }

        #endregion Public methods

        /// <summary>
        /// Validate the null parameter.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="name"></param>
        static void NullValidation(object o, string name)
        {

            if (null == o)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}