// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Windows;
using Microsoft.Test.Xaml.Framework;
using Microsoft.Test.Xaml.Serialization;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Misc
{
    public enum SkipAt
    {
        Start,
        CreateObject,
        Serialize,
    }

    public class RoundTripPublicTypesTest
    {
        private Dictionary<string, PropertyToIgnore> _propertiesToSkip = null;
        private string _propertiesToSkipFileName = "ActionForType_PropertiesToSkip.xml";
        private string[] _assembliesToSearch = { "PresentationFramework", "PresentationCore", "WindowsBase" };
        private string _typesToExcludeFileName = "ActionForType_TypesToSkipForSerializing.xml";
        private bool _shouldSerializeWithProps = true;
        private bool _useLightVerification = false;
        private string _xamlFile;
        private string _newSerializedFile;
        private object _originalRoot = null;
        private object _reloadRoot = null;
        IXamlTestSerializer _serializer;
        Dictionary<string, SkipAt> _typesToExclude = new Dictionary<string, SkipAt>();

        public void Run()
        {
            // This causes the WPF assemblies to load, this is a hack that will be removed later
            FrameworkElement frameworkElement = new FrameworkElement();
            frameworkElement = null;

            LoadTypesToExcludeFromXML();

            _serializer = XamlTestSerializerFactory.Create();
            _propertiesToSkip = TreeComparer.ReadSkipProperties(_propertiesToSkipFileName);

            Collection<Type> types = LoadTypes(_assembliesToSearch);

            foreach (Type type in types)
            {
                if ((type.GetConstructor(new Type[0] { }) != null) &&
                    (type.ContainsGenericParameters == false) &&
                    (type.IsNestedPublic == false) &&
                    (type.IsGenericType == false) &&
                    (type.IsPublic == true))
                {
                    if (_typesToExclude.ContainsKey(type.FullName) && (_typesToExclude[type.FullName] == SkipAt.Start))
                    { }
                    else
                    {
                        TestLog.Current.LogStatus("----------------------------------------------------");
                        TestLog.Current.LogStatus("---------------" + type.FullName + "------------------");
                        _shouldSerializeWithProps = true;
                        if (CreateXamlFileForRoundTrip(type))
                        {
                            if (RoundTripAndXamlCompare(type))
                            {
                                ReloadAndObjectCompare(type);
                            }
                        }
                    }
                }
            }
        }

        private bool CreateXamlFileForRoundTrip(Type type)
        {
            _xamlFile = type.ToString() + ".xaml";
            _newSerializedFile = type.ToString() + ".Serialized.xaml";

            try
            {
                object createdObject = CreateObject(type);

                if (_typesToExclude.ContainsKey(type.FullName) && (_typesToExclude[type.FullName] == SkipAt.CreateObject))
                {
                    TestLog.Current.LogStatus("Skipping serializing Type:" + type.FullName);
                    return false;
                }

                SerializeObject(_xamlFile, createdObject, _serializer);

                _useLightVerification = !_shouldSerializeWithProps;
                _shouldSerializeWithProps = true;
            }
            catch (Exception e)
            {
                TestLog.Current.LogEvidence("Xaml creation failed for type: " + type.ToString());
                TestLog.Current.LogStatus(e.ToString());
                TestLog.Current.Result = TestResult.Fail;
                return false;
            }

            return true;
        }

        private bool RoundTripAndXamlCompare(Type type)
        {
            try
            {
                // Load created xaml file     
                _originalRoot = LoadStreamContext(_xamlFile); // load with default settings

                object tempObject = LoadStreamContext(_xamlFile); // load with default settings
                // Serialize object tree
                SerializeObject(_newSerializedFile, tempObject, _serializer);

                if (_typesToExclude.ContainsKey(type.FullName) && (_typesToExclude[type.FullName] == SkipAt.Serialize))
                {
                    // We skip here because on serializing some type insert properties - so the xaml compare fails
                    TestLog.Current.LogStatus("Skipping after Serializing Type:" + type.FullName);
                    return false;
                }

                // Compare newly serialized xaml file with preserialized file
                if (!ComparisonServices.CompareXamlFiles(_xamlFile, _newSerializedFile))
                {
                    if (_useLightVerification)
                    {
                        TestLog.Current.LogStatus(type.Name + " was created using XmlDoc. In some types serialization forces additional props causing the compare to fail");
                    }
                    else
                    {
                        LogFailure(type, _xamlFile, _newSerializedFile, "Xaml file comparison failed for Type:");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                TestLog.Current.LogEvidence(e);
                TestLog.Current.Result = TestResult.Fail;
                return false;
            }

            return true;
        }

        private bool ReloadAndObjectCompare(Type type)
        {
            try
            {
                // Reload serialized xaml and compare object trees
                _reloadRoot = LoadStreamContext(_newSerializedFile);

                if (!ComparisonServices.CompareObjectTrees(_originalRoot, _reloadRoot))
                {
                    if (_useLightVerification)
                    {
                        if (_originalRoot.GetType() != _reloadRoot.GetType())
                        {
                            LogFailure(type, _xamlFile, _newSerializedFile, "Object root type comparison failed for Type:");
                            return false;
                        }
                    }
                    else
                    {
                        LogFailure(type, _xamlFile, _newSerializedFile, "Object tree comparison failed for Type:");
                        return false;
                    }
                }

                TestLog.Current.Result = TestResult.Pass;
            }
            catch (Exception e)
            {
                TestLog.Current.LogEvidence(e);
                TestLog.Current.Result = TestResult.Fail;
                return false;
            }

            return true;
        }

        #region Helpers

        /// <summary>
        /// LogFailure helper
        /// </summary>
        /// <param name="type">type examined</param>
        /// <param name="xamlFile">original serialized file</param>
        /// <param name="newSerializedFile">newly serialized file</param>
        /// <param name="message">message to be printed out</param>
        private static void LogFailure(Type type, string xamlFile, string newSerializedFile, string message)
        {
            TestLog.Current.Result = TestResult.Fail;
            TestLog.Current.LogEvidence(message + type.ToString() + "Pass1 File:" + xamlFile + "Pass2 File:" + newSerializedFile);
            TestLog.Current.LogFile(xamlFile);
            TestLog.Current.LogFile(newSerializedFile);
        }

        // Builds xaml containing a tag for the given type name.
        // Optionally includes properties in the xaml.
        // Throws if the type cannot be created.
        private object CreateObject(Type type)
        {
            // Create an instance of the type.
            object obj = Activator.CreateInstance(
                type,
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new object[0],
                null
            );

            // Explicitly reset properties using the same value they already have.
            // This will set DPs locally so they will be serialized.
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo info = propertyInfos[i];

                // Include property only if it meets certain criteria.
                if (info.CanRead &&
                    info.CanWrite &&
                    info.GetIndexParameters().Length == 0 &&
                    !ShouldIgnoreProperty(info.Name, type, this._propertiesToSkip))
                {
                    object val;

                    //Name value should following certain format. 
                    if (string.Equals(info.Name, "Name", StringComparison.InvariantCulture))
                    {
                        val = "Name_" + i.ToString();
                    }
                    else
                    {
                        val = info.GetValue(obj, null);
                    }
                    try
                    {
                        info.SetValue(obj, val, null);
                    }
                    catch (Exception)
                    {
                        TestLog.Current.LogStatus("Could not set '" + info.Name + "' property. Hence creating through XmlDoc");
                        _shouldSerializeWithProps = false;
                    }
                }
            }

            if (_shouldSerializeWithProps == false)
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlNode rootElement = xmlDoc.CreateElement(type.Name, GetNamespace(type));
                xmlDoc.AppendChild(rootElement);
                obj = xmlDoc;
            }

            return obj;
        }

        // Builds xaml containing a tag for the given type name.
        // Optionally includes properties in the xaml.
        // Throws if the type cannot be created.
        private void SerializeObject(string xamlFileName, object obj, IXamlTestSerializer serializer)
        {
            if (_shouldSerializeWithProps)
            {
                serializer.SerializeObjectTree(xamlFileName, obj);
            }
            else
            {
                ((XmlDocument)obj).Save(xamlFileName);
            }
        }

        private Collection<Type> LoadTypes(string[] assembliesToSearch)
        {
            Collection<Type> types = new Collection<Type>();
            foreach (string assemblyName in assembliesToSearch)
            {
                Assembly asm = XamlTestHelper.FindLoadedAssembly(assemblyName);
                Type[] asmTypes = asm.GetTypes();
                foreach (Type type in asmTypes)
                {
                    if (type.IsPublic)
                    {
                        types.Add(type);
                    }
                }
            }

            return types;
        }

        private bool ShouldIgnoreProperty(string propertyName, Type type, Dictionary<string, PropertyToIgnore> skipProps)
        {
            PropertyToIgnore property = null;
            foreach (string key in skipProps.Keys)
            {
                if (String.Equals(key, propertyName, StringComparison.InvariantCulture)
                    || key.StartsWith(propertyName + "___owner___"))
                {
                    property = skipProps[key];
                    if ((null == property.Owner) || DoesTypeMatch(type, property.Owner))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether or not a type is of a certain type or derives from it.
        /// Checks the non-qualified name, i.e. no namespace check.
        /// </summary>
        private bool DoesTypeMatch(Type ownerType, string typeName)
        {
            Type type = ownerType;
            bool isMatch = false;

            while (type != null && !isMatch)
            {
                if (0 == String.Compare(type.Name, typeName))
                {
                    isMatch = true;
                }

                type = type.BaseType;
            }

            return isMatch;
        }

        // Gets the valid xml namespace for the given type.
        private string GetNamespace(Type type)
        {
            string xmlns =
                "clr-namespace:" +
                type.Namespace +
                ";assembly=" +
                type.Assembly.GetName().Name;

            return xmlns;
        }

        private void LoadTypesToExcludeFromXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Environment.CurrentDirectory + "//" + _typesToExcludeFileName);
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Type");
            foreach (XmlNode node in nodeList)
            {
                _typesToExclude.Add(node.Attributes["TypeName"].Value, ((SkipAt)Enum.Parse(typeof(SkipAt), node.Attributes["SkipOn"].Value)));
            }
        }

        /// <summary>
        /// Loads xaml using the XamlReader.Load(Stream, ParserContext) method
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>object value</returns>
        private object LoadStreamContext(string xamlFileName)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            ParserContext context = new ParserContext();
            context.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Load(file);
            }
            finally
            {
                file.Close();
            }

            return treeRoot;
        }

        #endregion
    }
}
