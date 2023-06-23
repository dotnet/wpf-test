using System.Reflection;
using System.Resources;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Markup;
using System.IO;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Provides access to resources in wpftoolkit.dll
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Get string from Exception string table
        /// </summary>
        public static string GetString(string resourceId)
        {
            Assembly assembly = typeof(Calendar).Assembly;

            if (assembly != null)
            {
                // Use a ResourceManager to locate the resource in the desired assembly.
                ResourceManager rm = new ResourceManager("ExceptionStringTable", assembly);
                return rm.GetString(resourceId);
            }
            else
            {
                return string.Empty;
            }
        }

        public static FrameworkElement LoadXamlResource(string file)
        {
            object xaml = null;
            try
            {
                Stream stream = null;
                ResourceManager manager = new ResourceManager("ControlsTest.g", Assembly.GetExecutingAssembly());
                manager.IgnoreCase = true;
                stream = (Stream)manager.GetObject(file);
                if (stream != null)
                    xaml = XamlReader.Load(stream);
            }
            catch (Exception e)
            {
                GlobalLog.LogEvidence(e);
            }

            if (xaml != null && xaml is FrameworkElement)
                return xaml as FrameworkElement;
            else
                return null;
        }
    }
}
