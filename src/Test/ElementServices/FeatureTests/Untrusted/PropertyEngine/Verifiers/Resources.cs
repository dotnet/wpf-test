// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.CoreInput.Common;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// All property engine verifiers in this class
    /// </summary>
    public static partial class Verifiers
    {
        /// <summary>
        /// Used to verify a series of PropertyTrigger-related test cases
        /// </summary>
        /// <param name="root">Root element of the VisualTree</param>
        /// <returns>true when verification completes, false otherwise.</returns>
        public static bool ResourcesVerifier(StackPanel root)
        {
            string tag = (string)root.Tag;
            PrintTitle(tag);

            switch (tag)
            {
                case "Resources0001":
                    ResourcesVerifierScenario1(root, null);
                    break;
                case "Resources0002":
                    ResourcesVerifierScenario1(root, true);
                    break;
                case "Resources0003":
                    ResourcesVerifierScenario1(root, false);
                    break;
                case "Resources0004":
                    ResourcesVerifierScenario2(root);
                    break;
                case "Resources0005":
                    ResourcesVerifierScenario1(root, null);
                    break;
                case "Resources0101":
                case "Resources0102":
                case "Resources0103":
                case "Resources0104":
                    ResourcesVerifierScenario3(root, s_brushIndigo);
                    break;
                case "Resources0105":
                    ResourcesVerifierScenario3(root, s_brushLightGreen);
                    break;
                case "Resources2000":
                case "Resources2001":
                case "Resources2002":
                case "Resources2003":
                case "Resources2004":
                case "Resources2005":
                case "Resources2006":
                case "Resources2007":
                case "Resources2008":
                case "Resources2009":
                    ResourcesVerifierScenarioSelfCheck1(root);
                    break;

            }
            return true;
        }

        /// <summary>
        /// Code used for Resources0001 through Resources0003
        /// </summary>
        /// <param name="root">Root element</param>
        /// <param name="xSharedTag">null when x:Shared takes default value, true when x:Shared is true, false when x:Shared is false </param>
        private static void ResourcesVerifierScenario1(StackPanel root, Nullable<bool> xSharedTag)
        {
            object[] allKeys, allKeysDefaultShared, allKeysDefaultNotShared;
            ResourcesVerifierSetKeyArrays(out allKeys, out allKeysDefaultShared, out allKeysDefaultNotShared);

            ResourceDictionary rd = root.Resources;
            Debug.Assert(rd != null, "ResourceDictionary is non-null.");

            CoreLogger.LogStatus("At startup, all Resources are defer loaded");
            ResourcesVerifierCheckDeferred(root, rd, allKeys, true);
            CoreLogger.LogStatus("Ask for resource for typeof(Button)");
            Button testBtn = new Button();
            root.Children.Add(testBtn);
            if (!xSharedTag.HasValue || xSharedTag == true)
            {
                CoreLogger.LogStatus("typeof(Button) is no longer defer-loaded");
                ResourcesVerifierCheckDeferred(root, rd, typeof(Button), false);
            }
            else
            {
                CoreLogger.LogStatus("typeof(Button) is still defer-loaded because x:Shared is False");
                ResourcesVerifierCheckDeferred(root, rd, typeof(Button), true);  //Blocked here due to 
            }
            CoreLogger.LogStatus("Other are still defer-loaded");
            ResourcesVerifierCheckDeferred(root, rd, new object[] { "StringInfo", "LightBlueBrush", "ConfirmButton", "TestFCE", "TestTemplate", "TestRD", "TestStyle", "TestUIElement", "TestContentElement" }, true);

            CoreLogger.LogStatus("Now load 'TestStyle', which in turn loads 'LightBlueBrush'");
            testBtn.Style = (Style)root.FindResource("TestStyle");
            if (!xSharedTag.HasValue || xSharedTag == true)
            {
                CoreLogger.LogStatus("three resources are no longer defer-loaded");
                ResourcesVerifierCheckDeferred(root, rd, new object[] { typeof(Button), "TestStyle", "LightBlueBrush" }, false);
            }
            else
            {
                CoreLogger.LogStatus("these three resources are still defer-loaded because x:Shared is False");
                ResourcesVerifierCheckDeferred(root, rd, new object[] { typeof(Button), "TestStyle", "LightBlueBrush" }, true);
            }
            //Other are still defer-loaded
            ResourcesVerifierCheckDeferred(root, rd, new object[] { "StringInfo", "ConfirmButton", "TestFCE", "TestTemplate", "TestRD", "TestUIElement", "TestContentElement" }, true);

            CoreLogger.LogStatus("Now load 'ConfirmButton', 'TestUIElement', and 'TestContentElement'");
            Button confirmButton = (Button)root.Resources["ConfirmButton"];
            UIElement testUIElement = (UIElement)root.FindResource("TestUIElement");
            ContentElement testContentElement = (ContentElement)rd["TestContentElement"];
            //By default, or x:Shared is false, they are still defer-loaded (Differentiate 3 test cases)
            if (!xSharedTag.HasValue || xSharedTag == false)
            {
                ResourcesVerifierCheckDeferred(root, rd, new object[] { "StringInfo", "ConfirmButton", "TestFCE", "TestTemplate", "TestRD", "TestUIElement", "TestContentElement" }, true);
            }
            else  //Otherwise, they are no longer defer-loaded
            {
                ResourcesVerifierCheckDeferred(root, rd, new object[] { typeof(Button), "TestStyle", "LightBlueBrush", "ConfirmButton", "TestContentElement", "TestUIElement" }, false);
                ResourcesVerifierCheckDeferred(root, rd, new object[] { "StringInfo", "TestTemplate", "TestRD", "TestFCE" }, true);
            }
            //Using SetResourceReference
            confirmButton.SetResourceReference(Button.NameProperty, "StringInfo");
            root.Children.Add(confirmButton);
            //Only SetResourceReference() does not load the Resource
            if (!xSharedTag.HasValue || xSharedTag == false)
            {
                ResourcesVerifierCheckDeferred(root, rd, new object[] { "StringInfo", "ConfirmButton", "TestFCE", "TestTemplate", "TestRD", "TestUIElement", "TestContentElement" }, true);
            }
            else
            {
                ResourcesVerifierCheckDeferred(root, rd, new object[] { typeof(Button), "TestStyle", "LightBlueBrush", "ConfirmButton", "TestContentElement", "TestUIElement" }, false);
                ResourcesVerifierCheckDeferred(root, rd, new object[] { "StringInfo", "TestTemplate", "TestRD", "TestFCE" }, true);
            }
            //Accessing it does load the Resource
            string name = confirmButton.Name;
            Debug.Assert("Test XAML" == name, "Content is 'Test XAML'");
            //Now "StringInfo" are no longer defer-loaded for default, or x:Shared is true
            if (!xSharedTag.HasValue || xSharedTag == true)
            {
                ResourcesVerifierCheckDeferred(root, rd, "StringInfo", false);
            }
            else
            {
                ResourcesVerifierCheckDeferred(root, rd, "StringInfo", true);
            }

            CoreLogger.LogStatus("Now, verify x:Shared default behaviour (All resources will be accessed)");
            ResourcesVerifierCheckXShared(root, rd, allKeys, xSharedTag);    //Differentiate 3 different cases

            //Final check for Deferred
            ResourcesVerifierCheckDeferred(root, rd, allKeys, ResourcesVerifierFlipNullableBool(xSharedTag));   //Differentiate 3 different cases
        }

        /// <summary>
        /// Code used by Resources0004
        /// </summary>
        /// <param name="root">root element</param>
        private static void ResourcesVerifierScenario2(StackPanel root)
        {
            object[] allKeys, allKeysDefaultShared, allKeysDefaultNotShared;
            ResourcesVerifierSetKeyArrays(out allKeys, out allKeysDefaultShared, out allKeysDefaultNotShared);

            ResourceDictionary rd = root.Resources;
            Debug.Assert(rd != null, "ResourceDictionary is non-null.");

            ResourcesVerifierCheckDeferred(root, rd, new object[] { typeof(Button), "TestStyle", "LightBlueBrush", "StringInfo" }, false);
            ResourcesVerifierCheckDeferred(root, rd, new object[] { "ConfirmButton", "TestFCE", "TestTemplate", "TestRD", "TestUIElement", "TestContentElement" }, true);

            CoreLogger.LogStatus("Now, verify x:Shared default behaviour (All resources will be accessed)");
            ResourcesVerifierCheckXShared(root, rd, allKeys, null);    

            //Final check for Deferred
            ResourcesVerifierCheckDeferred(root, rd, allKeys, ResourcesVerifierFlipNullableBool(null));   
        }


        /// <summary>
        /// Code used by Resources0101 through 0105
        /// </summary>
        /// <param name="root"></param>
        /// <param name="expectedBackground"></param>
        private static void ResourcesVerifierScenario3(StackPanel root, SolidColorBrush expectedBackground)
        {
            Button testButton = (Button)root.FindName("TestButton");
            ValidateButtonBackgroundAssert(testButton, expectedBackground);
        }

        /// <summary>
        /// Each Button's Tag indicated its background. The name ending with s is for {StaticResource}. 
        /// and the name ending with d is for {DynamicResource}
        /// </summary>
        /// <param name="root">root element. It only contains Buttons</param>
        private static void ResourcesVerifierScenarioSelfCheck1(StackPanel root)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Brush));
            foreach (UIElement element in root.Children)
            {
                //This root only contains Button
                System.Diagnostics.Debug.Assert(element is Button, "For ResourcesVerifierScenarioSelfCheck1 to work. Root can only contain Button");
                Button btn = (Button)element;
                System.Diagnostics.Debug.Assert(btn.Tag is string, "For ResourcesVerifierScenarioSelfCheck1 to work. Each Button must have a tag of type String");
                string btnTag = (string)btn.Tag;
                //The Tag format should be: color.index.d|s. For example: Blue.2.d
                string[] tagParts = btnTag.Split('.');
                System.Diagnostics.Debug.Assert(tagParts.Length == 3, "Invalid Tag Format!");
                string colorPart = tagParts[0];
                object result = tc.ConvertFrom(colorPart);
                System.Diagnostics.Debug.Assert(result is SolidColorBrush, "Invalid Tag Value");
                SolidColorBrush expectedBackground = (SolidColorBrush)result;
                ValidateButtonBackgroundAssert(btn, expectedBackground);
            }
        }

        /// <summary>
        ///Helper method to set three commonly used key arrays
        /// </summary>
        /// <param name="allKeys">all keys array</param>
        /// <param name="allKeysDefaultShared">all keys that has default value of true for x:Shared </param>
        /// <param name="allKeysDefaultNotShared">all keys that has default value of false for x:Shared</param>
        private static void ResourcesVerifierSetKeyArrays(out object[] allKeys, out object[] allKeysDefaultShared, out object[] allKeysDefaultNotShared)
        {
            //All keys
            allKeys = new object[]{ "StringInfo", "LightBlueBrush", "ConfirmButton", "TestFCE", "TestTemplate", "TestRD", typeof(Button), "TestStyle", "TestUIElement", "TestContentElement" };
            //Default shared: Deferred Resource Loading only for first access
            allKeysDefaultShared = new object[] { "StringInfo", "LightBlueBrush", "TestFCE", "TestTemplate", "TestRD", typeof(Button), "TestStyle" };
            //Default not shared: Deferred Resource Loading for first and subsequent access
            allKeysDefaultNotShared = new object[]{ "ConfirmButton", "TestUIElement", "TestContentElement" };
        }

        private static Nullable<bool> ResourcesVerifierFlipNullableBool(Nullable<bool> b)
        {
            if (b.HasValue)
            {
                if (b == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return b;
            }
        }

        /// <summary>
        /// Using Reflection to find the internal Hashtable for ResourceDictionary
        /// </summary>
        /// <param name="rd">ResourceDictionary to reflect upon</param>
        /// <returns></returns>
        private static Hashtable GetResourceDictionaryPrivateBaseDictionary(ResourceDictionary rd)
        {
            FieldInfo fi = typeof(ResourceDictionary).GetField("_baseDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
            object privateBaseDictionary = fi.GetValue(rd);
            Debug.Assert(privateBaseDictionary != null && privateBaseDictionary is Hashtable, "Fix GetResourceDictionaryPrivateBaseDictionary. Private hashtable may be renamed.");
            return (Hashtable)privateBaseDictionary;
        }


        /// <summary>
        /// Check if the ResourceDictionary for a number of keys is currently deferred
        /// </summary>
        /// <param name="root">The root element</param>
        /// <param name="rd">The ResourceDictionary</param>
        /// <param name="keys">an array of keys to check</param>
        /// <param name="isDeferredExpected">true if the resource should be deferred, false if the resource should not be deferred, null to use default after the resource has been accessed.</param>
        private static void ResourcesVerifierCheckDeferred(FrameworkElement root, ResourceDictionary rd, object[] keys, Nullable<bool> isDeferredExpected)
        {
            Debug.Assert(keys != null, "keys cannot be null");
            foreach (object key in keys)
            {
                ResourcesVerifierCheckDeferred(root, rd, key, isDeferredExpected);
            }
        }

        /// <summary>
        /// Check if the ResourceDictionary for key is currently deferred
        /// </summary>
        /// <param name="root">The root element</param>
        /// <param name="rd">The ResourceDictionary</param>
        /// <param name="key">The key to check</param>
        /// <param name="isDeferredExpected">true if the resource should be deferred, false if the resource should not be deferred, null to use default after the resource has been accessed.</param>
        private static void ResourcesVerifierCheckDeferred(FrameworkElement root, ResourceDictionary rd, object key, Nullable<bool> isDeferredExpected)
        {
            Hashtable privateBaseDictionary = GetResourceDictionaryPrivateBaseDictionary(rd);
            object privateValue = privateBaseDictionary[key];
            string privateValueTypeName = privateValue.GetType().FullName;
            //Console.WriteLine("Debug : " + privateValueTypeName);

            bool isDeferredActual = false;

            if (key is Type)
            {
                isDeferredActual = privateValueTypeName == "System.Windows.Markup.BamlDefAttributeKeyTypeRecord";
            }
            else
            {
                isDeferredActual = privateValueTypeName == "System.Windows.Markup.BamlDefAttributeKeyStringRecord";
            }
            if (isDeferredExpected.HasValue)
            {
                Debug.Assert(isDeferredExpected == isDeferredActual, "Resource " + key + " is " + ((bool)isDeferredExpected ? "" : "NOT ") + "deferred.");
            }
            else
            {//Compare with the default (only after the resource has been accessed)
                //Make sure that we access it
                object o = root.FindResource(key);
                //Check deferred
                Debug.Assert(isDeferredActual == !ResourcesVerifierGetXSharedDefault(o), "Resource " + key + " is " + (!ResourcesVerifierGetXSharedDefault(o) ? "" : "NOT ") + "deferred.");
            }
        }

        /// <summary>
        /// Check x:Shared behaviour for a number of keys
        /// </summary>
        /// <param name="root">The root element</param>
        /// <param name="rd">The ResourceDictionary</param>
        /// <param name="keys">An array of keys</param>
        /// <param name="isSharedExpected">true if the expectation is shared, false if not shared, null to take the default</param>
        private static void ResourcesVerifierCheckXShared(FrameworkElement root, ResourceDictionary rd, object[] keys, Nullable<bool> isSharedExpected)
        {
            Debug.Assert(keys != null, "keys cannot be null");
            foreach (object key in keys)
            {
                ResourcesVerifierCheckXShared(root, rd, key, isSharedExpected);
            }
        }

        /// <summary>
        /// Check x:Shared behaviour for a single key
        /// </summary>
        /// <param name="root">The root element</param>
        /// <param name="rd">The ResourceDictionary</param>
        /// <param name="key">The key to check</param>
        /// <param name="isSharedExpected">true if the expectation is shared, false if not shared, null to take the default</param>
        private static void ResourcesVerifierCheckXShared(FrameworkElement root, ResourceDictionary rd, object key, Nullable<bool> isSharedExpected)
        {
            Debug.Assert(rd.Contains(key), "Calling CheckXShared with invalid key"); //TOCHECK: Simply checking contains should not load resource. (elsewhere)
            //Get the resource twice
            object o1 = rd[key];
            object o2 = root.FindResource(key);

            bool isSharedActual = object.ReferenceEquals(o1, o2);
            if (isSharedExpected.HasValue)
            {
                Debug.Assert(isSharedExpected == isSharedActual, "Resource " + key + " is " + ((bool)isSharedExpected ? "" : "NOT ") + "shared.");
            }
            else
            {
                //Compare with the dafault
                Debug.Assert(isSharedActual == ResourcesVerifierGetXSharedDefault(o1), "Resource " + key + " is " + (ResourcesVerifierGetXSharedDefault(o1) ? "" : "NOT ") + "shared.");
            }
        }

        /// <summary>
        /// Get default value for x:Shared.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private static bool ResourcesVerifierGetXSharedDefault(object resource)
        {
            if (resource is UIElement || resource is ContentElement)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
