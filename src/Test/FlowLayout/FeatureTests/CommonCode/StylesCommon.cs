// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Resources;
using System.Reflection;

namespace Microsoft.Test.Layout
{
	/// <summary>
    /// Class that returns some styles that are theme neutral and are used in visual verification tests.
    /// </summary>
	public class GenericStyles
	{
        /// <summary>
        /// Load generic, theme neutral styles for cases that use visual verification
        /// </summary>
        /// <param name="filename">Name of file to load from resources.</param>
        /// <returns>A resource dictonary</returns>
        public static ResourceDictionary LoadStyle(string filename)
        {
            ResourceDictionary generic = null;

            Stream resourceStream = null;
            Assembly targetAssembly = typeof(GenericStyles).Assembly;

            ResourceManager resourceManager = new ResourceManager("FlowLayoutTest.g", targetAssembly);
            resourceManager.IgnoreCase = true;
            resourceStream = (Stream)resourceManager.GetObject(filename);

            if (resourceStream != null)
            {
                generic = XamlReader.Load(resourceStream) as ResourceDictionary;
            }

            return generic;
        }

        /// <summary>
        /// Load all generic, theme neutral styles.
        /// </summary>
        /// <returns>A resource dictionary</returns>
        public static ResourceDictionary LoadAllStyles()
        {
            ResourceDictionary generic = null;

            Stream resourceStream = null;
            Assembly targetAssembly = typeof(GenericStyles).Assembly;

            ResourceManager resourceManager = new ResourceManager("FlowLayoutTest.g", targetAssembly);
            resourceManager.IgnoreCase = true;
            resourceStream = (Stream)resourceManager.GetObject("GenericFlowLayoutStyles.xaml");

            if (resourceStream != null)
            {
                generic = XamlReader.Load(resourceStream) as ResourceDictionary;
            }

            return generic;
        }

        /// <summary>
        /// Load all generic, theme neutral styles with the related resource file.
        /// </summary>
        /// <returns>A resource dictionary</returns>
        public static ResourceDictionary LoadAllStyles(string resourceFileName)
        {
            ResourceDictionary generic = null;

            Stream resourceStream = null;
            Assembly targetAssembly = typeof(GenericStyles).Assembly;

            ResourceManager resourceManager = new ResourceManager(resourceFileName, targetAssembly);
            resourceManager.IgnoreCase = true;
            resourceStream = (Stream)resourceManager.GetObject("GenericFlowLayoutStyles.xaml");

            if (resourceStream != null)
            {
                generic = XamlReader.Load(resourceStream) as ResourceDictionary;
            }

            return generic;
        }
	}
}
