// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Test.Integration;
using Microsoft.Test.Serialization;
using System.Xml;
using Microsoft.Test.Logging;
using System.Windows.Markup;
using System.IO;
using System.ComponentModel;
using Microsoft.Test.Windows;
using System.Reflection;
using Microsoft.Test.Markup;
using Microsoft.Test;
using System.Windows;
using System.Globalization;
using System.Collections;
using Microsoft.Test.Integration.Windows;

namespace Avalon.Test.Xaml.Markup
{

    /// <summary>
    /// Represents an action that may be applied to types.
    /// </summary>
    public enum ActionForType
    {        
        /// <summary>
        /// 
        /// </summary>
        CompilePropertiesToo,
        
        /// <summary>
        /// 
        /// </summary>
        Compile,
            
            /// <summary>
        /// Parse xaml for each type using Load(Stream) API.
        /// </summary>
        ParsingOnly,

        /// <summary>
        /// Parse xaml for each type and its properties using Load(Stream) API.
        /// </summary>
        ParsingOnlyPropertiesToo,

        /// <summary>
        /// Parse xaml for each type using Load(Stream, ParserContext) and Load(XmlReader) APIs.
        /// </summary>
        MultipleParserOnly,

        /// <summary>
        /// Round-trips xaml for each type. 
        /// </summary>        
        RoundTrip,

        /// <summary>
        /// Round-trips xaml for each type and its properties. 
        /// </summary>        
        RoundTripPropertiesToo,

        /// <summary>
        /// Verifies the KnownTypes mapping for each type and its properties. 
        /// </summary>        
        CheckKnownTypes,
    }

    enum ParserOptions
    {
        stream,
        streamAndParserContext,
        xmlReader,
        roundTrip,
        compile
    }

    /// <summary>
    /// 
    /// </summary>
    public class MarkupActionsPerType
    {
        #region Constructor

        // Initialize the Avalon assemblies.
        static MarkupActionsPerType()
        {
            s_assemblies = new Assembly[]
            {
                typeof(FrameworkElement).Assembly,
                typeof(UIElement).Assembly,
                typeof(DependencyObject).Assembly
            };
        }
        #endregion


        private static XmlDocument s_xmlDoc = null;
        static object s_globalLock = new object();
        // Used for reading and caching ActionForTypeExclude.xml.
        private static Hashtable s_fullTrustHash = null;
        private static Hashtable s_exclusionHash = null;

        /// <summary>
        /// This callback happens a DiscoveryTime
        /// </summary>
        /// <returns></returns>
        public static bool GlobalFilterCallback(VariationItem vi, FilterCallback filterCallback)
        {
            if (s_xmlDoc == null)
            {
                lock (s_globalLock)
                {
                    if (s_xmlDoc == null)
                    {
                        s_xmlDoc = new XmlDocument();

                        s_xmlDoc.Load(Path.Combine(filterCallback.SupportInfo[0], "ActionForTypeExclude.xml"));
                    }
                }
            }

            TypeVariationItem tvi = null;
            CallbackVariationItem cgi = null;

            foreach (VariationItem viChild in vi.GetVIChildren())
            {
                if (viChild is TypeVariationItem)
                {
                    tvi = (TypeVariationItem)viChild;
                }

                if (viChild is CallbackVariationItem)
                {
                    cgi = (CallbackVariationItem)viChild;
                }
            }

            ActionForType actionType = (ActionForType)Enum.Parse(typeof(ActionForType), (string)cgi.Content.Content, true);
            Type type = tvi.TypeName.GetCurrentType();
            if (actionType == ActionForType.CheckKnownTypes && (
                type.IsSubclassOf(typeof(Attribute)) ||
                type.IsSubclassOf(typeof(System.IO.Stream)) ||
                type.IsSubclassOf(typeof(EventArgs)) ||
                type.IsSubclassOf(typeof(EventHandler)) ||
                type.IsSubclassOf(typeof(Delegate))))
            {
                return true;
            }

            List<string> exclusionList = _GetExclusionList(actionType);


            if (exclusionList != null && exclusionList.Contains(type.FullName))
            {
                return true;
            }

            if (_IsFullTrustType(actionType, type.FullName))
            {
                vi.SecurityLevel = TestCaseSecurityLevel.FullTrust;
            }
            return false;

        }

        private static bool _IsFullTrustType(ActionForType actionForType, string typeName)
        {
            //
            // Populate full trust list.
            //
            if (s_fullTrustHash == null)
            {
                s_fullTrustHash = new Hashtable();

                foreach (ActionForType currentActionForType in Enum.GetValues(typeof(ActionForType)))
                {
                    List<string> fullTrustList = new List<string>();
                    s_fullTrustHash.Add(currentActionForType, fullTrustList);

                    // Check section for the current action.
                    XmlNodeList nodeList = s_xmlDoc.SelectNodes("SetupActionForTypes/SetupActionForType[@Name='" + currentActionForType.ToString() + "']/FullTrustTypes/FullTrustType");

                    foreach (XmlNode node in nodeList)
                    {
                        fullTrustList.Add(node.Attributes["Name"].Value);
                    }

                    // Check section for all actions.
                    nodeList = s_xmlDoc.SelectNodes("SetupActionForTypes/SetupActionForType[@Name='All']/FullTrustTypes/FullTrustType");

                    foreach (XmlNode node in nodeList)
                    {
                        fullTrustList.Add(node.Attributes["Name"].Value);
                    }
                }
            }

            List<string> trustList = (List<string>)s_fullTrustHash[actionForType];

            return trustList.Contains(typeName);
        }


        private static List<string> _GetExclusionList(ActionForType actionForType)
        {
            //
            // Populate exclusion list.
            //
            if (s_exclusionHash == null)
            {
                s_exclusionHash = new Hashtable();

                foreach (ActionForType currentActionForType in Enum.GetValues(typeof(ActionForType)))
                {
                    List<string> exclusionList = new List<string>();
                    s_exclusionHash.Add(currentActionForType, exclusionList);

                    // Check section for the current action.
                    XmlNodeList nodeList = s_xmlDoc.SelectNodes("SetupActionForTypes/SetupActionForType[@Name='" + currentActionForType.ToString() + "']/ExcludeTypes/ExcludeType");

                    foreach (XmlNode node in nodeList)
                    {
                        exclusionList.Add(node.Attributes["Name"].Value);
                    }

                    // Check section for all actions.
                    nodeList = s_xmlDoc.SelectNodes("SetupActionForTypes/SetupActionForType[@Name='All']/ExcludeTypes/ExcludeType");

                    foreach (XmlNode node in nodeList)
                    {
                        exclusionList.Add(node.Attributes["Name"].Value);
                    }
                }
            }

            return (List<string>)s_exclusionHash[actionForType];
        }


        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">
        /// The possible values are: 
        ///     ParsingOnly; ParsingOnly; PropertiesToo; MultipleParserOnly; 
        ///     Roundtrip; RoundtripPropertiesToo; CheckKnownTypes
        /// </param>
        public void MarkupActionCallback(ContentItem item)
        {
            Type type = CommonStorage.Current.Get("Type") as Type;
            
            if (type == null)
            {
                throw new InvalidOperationException("Cannot reach the Type from CommonStorage.");
            }

            string typeName = type.FullName;

            if (String.IsNullOrEmpty(typeName))
            {
                throw new InvalidOperationException("The typeName cannot be null or empty.");
            }

            ActionForType actionType = (ActionForType)Enum.Parse(typeof(ActionForType), (string)item.Content,true);
            GlobalLog.LogStatus("Current ActionForType: "+ actionType);
            // Set culture to 'en-us' so round-tripping will work correctly.

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");


            switch (actionType)
            {
                case ActionForType.ParsingOnly:
                case ActionForType.ParsingOnlyPropertiesToo:

                    DoParseAction(typeName, actionType);
                    break;

                case ActionForType.MultipleParserOnly:

                    DoMultipleParsingAction(typeName, actionType);
                    break;


                case ActionForType.RoundTrip:
                case ActionForType.RoundTripPropertiesToo:

                    DoParseAction(typeName, actionType);
                    break;


                case ActionForType.CheckKnownTypes:

                    DoCheckKnownTypesAction(typeName, actionType);
                    break;

                case ActionForType.Compile:
                case ActionForType.CompilePropertiesToo:
                    DoCompileAction(typeName, actionType);
                    break;

                default:
                    throw new NotImplementedException(actionType.ToString());
            }


            if (TestLog.Current != null)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Generates xaml containing an instance declaration of the type,
        /// then verifies the xaml compiles correctly.  Optionally includes
        /// all properties set locally in xaml.
        /// </summary>
        public void DoCompileAction(string typeName, ActionForType actionForType)
        {
            Type type = GetType(typeName);

            // Create xaml.
            MarkupTestLog.LogStatus("Verifying xaml with correct default namespace...");
            _BuildAndSaveXaml(typeName, actionForType == ActionForType.CompilePropertiesToo, true);

            _DoXamlAction(ParserOptions.compile, type, false);
        }

        /// <summary>
        /// Verifies that have parseable type has an Id in KnownElements.
        /// Verifies that KnownTypes.Types[&lt;known element id&gt;] returns the correct type.
        /// Verifies that KnownTypes.CreateKnownElement() returns an element of the correct type.
        /// Verifies that KnownTypes.KnownTypeConverterId() returns an Id if the type has a TypeConverter.
        /// Verifies that KnownTypes.CreateKnownElement() returns an instance of the correct TypeConverter.
        /// </summary>        
        public void DoCheckKnownTypesAction(string typeName, ActionForType actionForType)
        {
            Type type = GetType(typeName);

            Type knownTypes = GetType("System.Windows.Markup.KnownTypes");
            Type knownElements = GetType("System.Windows.Markup.KnownElements");
            Type typeIndexer = GetType("System.Windows.Markup.TypeIndexer");

            //
            // KnownElements enum
            // Get the KnownElements ID for the type.
            // If this throws, it's a product 

            object elementId = null;
            try
            {
                elementId = Enum.Parse(knownElements, type.Name);
            }
            catch (ArgumentException ex)
            {
                if (!type.IsValueType)
                {
                    throw new Microsoft.Test.TestValidationException("The parseable (public with empty constructor) type does not have an Id in KnownElements.", ex);
                }
            }

            //
            // KnownTypes.Types[]
            // Verify that KnownTypes.Types[KnownElements.<typeName>] returns the correct type.
            //
            object obj = knownTypes.GetProperty("Types", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
            PropertyInfo item = typeIndexer.GetProperty("Item");
            object knownType = item.GetValue(obj, new object[] { elementId });

            if (knownType == null)
            {
                throw new Microsoft.Test.TestValidationException("KnownTypes.Types[KnownElements." + typeName + "] returned null.");
            }

            if (!knownType.Equals(type))
            {
                throw new Microsoft.Test.TestValidationException("KnownTypes.Types[KnownElements." + typeName + "] returned the wrong type. Expected: '" + typeName + "', Actual: '" + ((Type)knownType).Name + "'");
            }

            //
            // KnownTypes.CreateKnownElement()
            // Create an instance using the known type ID.
            // Verify the instance type returned is the correct type.
            //
            MethodInfo CreateKnownElement = knownTypes.GetMethod("CreateKnownElement", BindingFlags.NonPublic | BindingFlags.Static);

            object element = CreateKnownElement.Invoke(null, BindingFlags.Static | BindingFlags.NonPublic, null, new object[] { elementId }, System.Globalization.CultureInfo.InvariantCulture);
            if (element == null)
            {
                throw new Microsoft.Test.TestValidationException("KnownTypes.CreateKnownElement(KnownElements." + typeName + ") returned null.");
            }

            Type elementType = element.GetType();
            if (!elementType.Equals(type))
            {
                throw new Microsoft.Test.TestValidationException("KnownTypes.CreateKnownElement(KnownElements." + typeName + ") returned the wrong type of object: '" + elementType + "'.");
            }

            //
            // KnownTypes.GetKnownTypeConverterId()
            // Get TypeConverter for element via KnownTypes.
            // Get TypeConverter for element via TypeDescriptor.
            // Verify they are the same.
            //
            TypeConverter expectedConverter = TypeDescriptor.GetConverter(element, true);

            MethodInfo GetKnownTypeConverterId = knownTypes.GetMethod("GetKnownTypeConverterId", BindingFlags.NonPublic | BindingFlags.Static);

            object converterId = GetKnownTypeConverterId.Invoke(null, new object[] { elementId });

            TypeConverter knownConverter = CreateKnownElement.Invoke(null, new object[] { converterId }) as TypeConverter;

            Type expectedConverterType = expectedConverter.GetType();

            if (knownConverter != null)
            {
                if (expectedConverter == null || knownConverter.GetType() != expectedConverter.GetType())
                {
                    throw new Microsoft.Test.TestValidationException("Could not get the expected converter for '" + typeName + "' using KnownTypes. Expected: '" + expectedConverter.GetType().Name + "', Actual: '" + knownConverter.GetType().Name + "'");
                }
            }
            else
            {
                if (!_IsDefaultConverter(expectedConverter))
                {
                    throw new Microsoft.Test.TestValidationException("Could not get the expected converter for '" + typeName + "' using KnownTypes. Expected: '" + expectedConverter.GetType().Name + "', Actual: 'null'");
                }
            }

            // 
            // KnownTypes.GetKnownTypeConverterIdForProperty()
            // For each property, try to get a TypeConverter.
            // If we get one, compare it against the one that PropertyDescriptor returns.
            //
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(element, true);
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            MethodInfo GetKnownTypeConverterIdForProperty = knownTypes.GetMethod("GetKnownTypeConverterIdForProperty", BindingFlags.NonPublic | BindingFlags.Static);
            PropertyInfo info = null;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                info = propertyInfos[i];

                if (String.Compare(info.Name, "Item", true) == 0)
                {
                    continue;
                }

                // Get converter from KnownTypes.
                converterId = GetKnownTypeConverterIdForProperty.Invoke(null, new object[] { elementId, info.Name });

                knownConverter
                    = CreateKnownElement.Invoke(null, new object[] { converterId }) as TypeConverter;

                expectedConverter = properties[info.Name].Converter;



                // If converter was found, compare it against the one from PropertyDescriptor.
                if (knownConverter != null)
                {
                    if (expectedConverter == null || knownConverter.GetType() != expectedConverter.GetType())
                    {
                        throw new Microsoft.Test.TestValidationException("Could not get the expected converter for '" + typeName + "." + info.Name + "' using KnownTypes. Expected: '" + expectedConverter.GetType().Name + "', Actual: '" + knownConverter.GetType().Name + "'");
                    }
                }
                else
                {
                    if (!_IsDefaultConverter(expectedConverter))
                    {
                        //throw new Microsoft.Test.TestValidationException("Could not get the expected converter for '" + typeName + "." + info.Name + "' using KnownTypes. Expected: '" + expectedConverter.GetType().Name + "', Actual: 'null'");
                        if (knownConverter == null)
                        {
                            MarkupTestLog.LogStatus(info.Name.ToString() + " " + expectedConverter.ToString());
                        }
                    }

                }
            }

            //
            // 


            //
            // 


            //
            // 


            //
            // 


            //
            // 


            // 
            // 

        }

        private bool _IsDefaultConverter(TypeConverter converter)
        {
            Type type = converter.GetType();

            string assemblyName = type.Assembly.GetName().Name;

            if (type.FullName.IndexOf("System.ComponentModel") != -1 &&
                (assemblyName.IndexOf("mscorlib") != -1
                || assemblyName.IndexOf("System") != -1))
            {
                return true;
            }


            if (type.Equals(typeof(System.ComponentModel.CollectionConverter))
                || type.Equals(typeof(System.ComponentModel.StringConverter))
                || type.Equals(typeof(System.ComponentModel.TypeConverter))
                || type.Equals(typeof(System.ComponentModel.BaseNumberConverter))
                || type.Equals(typeof(System.ComponentModel.BooleanConverter))
                || type.Equals(typeof(System.ComponentModel.CharConverter))
                || type.Equals(typeof(System.ComponentModel.CultureInfoConverter))
                || type.Equals(typeof(System.ComponentModel.DateTimeConverter))
                || type.Equals(typeof(System.ComponentModel.EnumConverter))
                || type.Equals(typeof(System.ComponentModel.ExpandableObjectConverter))
                || type.Equals(typeof(System.ComponentModel.GuidConverter))
                || type.Equals(typeof(System.ComponentModel.ReferenceConverter))
                || type.Equals(typeof(System.ComponentModel.StringConverter))
                || type.Equals(typeof(System.ComponentModel.TimeSpanConverter))
                || type.Equals(typeof(System.ComponentModel.TypeListConverter))
                || type.Equals(typeof(System.Drawing.ColorConverter))
                || type.Equals(typeof(System.Drawing.FontConverter))
                || type.Equals(typeof(System.Drawing.ImageConverter))
                || type.Equals(typeof(System.Drawing.ImageFormatConverter))
                || type.Equals(typeof(System.Drawing.PointConverter))
                || type.Equals(typeof(System.Drawing.RectangleConverter))
                || type.Equals(typeof(System.Drawing.SizeConverter))
                || type.Equals(typeof(System.Resources.ResXFileRef.Converter))
                || type.Equals(typeof(System.Windows.Forms.CursorConverter))
                || type.Equals(typeof(System.Windows.Forms.KeysConverter))
                || type.Equals(typeof(System.Windows.Forms.ListBindingConverter))
                || type.Equals(typeof(System.Windows.Forms.OpacityConverter))
                || type.Equals(typeof(System.Windows.Forms.SelectionRangeConverter))
                || type.Equals(typeof(System.Windows.Forms.TreeNodeConverter))
            )
            {
                return true;
            }
            return false;
        }




        /// <summary>
        /// Generates xaml containing an instance declaration of the type,
        /// then verifies the xaml parses correctly.  Optionally includes
        /// all properties set locally in xaml.
        /// </summary>        
        public void DoMultipleParsingAction(string typeName, ActionForType actionForType)
        {
            Type type = GetType(typeName);

            // Create xaml.
            MarkupTestLog.LogStatus("Verifying xaml with correct default namespace...");
            Type rootType = _BuildAndSaveXaml(type, false, true);

            _DoXamlAction(ParserOptions.xmlReader, type, false);
            _DoXamlAction(ParserOptions.streamAndParserContext, type, false);
        }



        /// <summary>
        /// Generates xaml containing an instance declaration of the type,
        /// then verifies the xaml parses correctly.  Optionally includes
        /// all properties set locally in xaml.
        /// </summary>
        public void DoParseAction(string typeName, ActionForType actionForType)
        {
            Type type = GetType(typeName);

            // Create xaml.
            MarkupTestLog.LogStatus("Verifying xaml with correct default namespace...");
            _BuildAndSaveXaml(type, actionForType == ActionForType.ParsingOnlyPropertiesToo, true);

            _DoXamlAction(ParserOptions.stream, type, false);

            // Create xaml with invalid default namespace.
            MarkupTestLog.LogStatus("Verifying xaml with bad default namespace...");
            _BuildAndSaveXaml(type, actionForType == ActionForType.ParsingOnlyPropertiesToo, false);

            _DoXamlAction(ParserOptions.stream, type, true);
        }


        /// <summary>
        /// Generates xaml containing an instance declaration of the type,
        /// then verifies the xaml round-trips correctly.  Optionally includes
        /// all properties set locally in xaml.
        /// </summary>
        public void DoRoundTripAction(string typeName, ActionForType actionForType)
        {
            this.PropertiesToSkipFileName = "RoundTripActionForType_PropertiesToSkip.xml";

            Type type = GetType(typeName);

            // Create xaml.
            MarkupTestLog.LogStatus("Verifying xaml with correct default namespace...");
            _BuildAndSaveXaml(typeName, actionForType == ActionForType.RoundTripPropertiesToo, true);

            _DoXamlAction(ParserOptions.roundTrip, type, false);
        }



        private bool _IsInAvalonNamespace(Type type)
        {
            string ns = type.Namespace;

            object[] oArray = type.Assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute), false);

            bool expectingException = false;

            foreach (XmlnsDefinitionAttribute attribute in oArray)
            {
                if (String.Compare(attribute.XmlNamespace, s_avalonUri, true) == 0 &&
                    String.Compare(attribute.ClrNamespace, ns, true) == 0)
                {
                    expectingException = true;
                    break;
                }
            }

            return expectingException;
        }

        // Performs action with try/catch.
        // Verifies exception, including whether or not it was excepted.
        private void _DoXamlAction(ParserOptions parserOptions, Type type, bool isExceptionExpected)
        {
            GlobalLog.LogStatus("_DoXamlAction ParserOptions: " + parserOptions);
            Type expectedExceptionType = typeof(XamlParseException);

            if (isExceptionExpected)
            {
                MarkupTestLog.LogStatus("Exception is expected...");
            }

            using (FileStream fs = new FileStream(this.XamlFileName, FileMode.Open, FileAccess.Read))
            {
                //
                // Perform action and catch any exception that occurs.
                //
                Exception exceptionCaught = null;

                try
                {
                    if (parserOptions == ParserOptions.stream)
                    {
                        System.Windows.Markup.XamlReader.Load(fs);
                    }
                    else if (parserOptions == ParserOptions.streamAndParserContext)
                    {
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = s_baseUri;
                        System.Windows.Markup.XamlReader.Load(fs, pc);
                    }
                    else if (parserOptions == ParserOptions.xmlReader)
                    {
                        using (XmlTextReader xmlReader = new XmlTextReader(fs))
                        {
                            System.Windows.Markup.XamlReader.Load(xmlReader);
                        }
                    }
                    else if (parserOptions == ParserOptions.roundTrip)
                    {
                        SerializationHelper helper = new SerializationHelper();
                        helper.XamlSerialized += new XamlSerializedEventHandler(_XamlSerialized);

                        object root1 = null, root2 = null;

                        helper.PreFirstDisplay += (SerializationCustomerEventHandler)delegate(object treeRoot)
                            {
                                root1 = treeRoot;
                            };
                        helper.PreSecondDisplay += (SerializationCustomerEventHandler)delegate(object treeRoot)
                            {
                                root2 = treeRoot;
                            };

                        helper.DoTreeComparison = false;
                        helper.RoundTripTest(fs, false);

                        TreeCompareResult result = TreeComparer.CompareLogical(root1, root2, this.PropertiesToSkip);
                    }
                    //else if (parserOptions == ParserOptions.compile)
                    //{
                    //    expectedExceptionType = typeof(Microsoft.Test.TestSetupException);

                    //    // Compile xaml into Avalon app.
                    //    CompilerHelper compiler = new CompilerHelper();
                    //    compiler.AddClassAttribute = !type.IsSealed;
                    //    compiler.CleanUpCompilation();

                    //    compiler.CompileApp(this.XamlFileName, "Application", null, null, null, Languages.CSharp);
                    //}
                    else
                    {
                        throw new Microsoft.Test.TestSetupException("ParserOptions value not recognized.");
                    }
                }
                catch (Exception e)
                {
                    MarkupTestLog.LogStatus("Exception caught...");
                    exceptionCaught = e;
                }

                //
                // Verify that exception is as expected.
                //
                if (isExceptionExpected)
                {
                    if (exceptionCaught == null)
                    {
                        throw new Microsoft.Test.TestValidationException("An exception was expected.");
                    }

                    if (exceptionCaught.GetType() != expectedExceptionType)
                    {
                        throw new Microsoft.Test.TestValidationException("An unexpected Exception type was caught. Expected:" + expectedExceptionType.Name + ", Actual:" + exceptionCaught.GetType().Name);
                    }

                    XamlParseException parseEx = exceptionCaught as XamlParseException;
                    if (parseEx != null && parseEx.LineNumber != 1 && parseEx.LinePosition != 2)
                    {
                        throw new Microsoft.Test.TestValidationException("Wrong LineNumber or LinePosition for XamlParseException", parseEx);
                    }
                }
                else if (exceptionCaught != null)
                {
                    throw new Microsoft.Test.TestValidationException("Unexpected exception occurred.", exceptionCaught);
                }
            }
        }

        /// <summary>
        /// Finds and returns the Type for the given type name.
        /// Throws if type isn't found.
        /// </summary>
        protected static Type GetType(string typeName)
        {
            Type type = null;

            for (int i = 0; type == null && i < s_assemblies.Length; i++)
            {
                type = s_assemblies[i].GetType(typeName, false, false);
            }

            //type = Utility.FindType(typeName, true);
            if (type == null)
            {
                throw new Microsoft.Test.TestValidationException("Could not find type '" + typeName + "'.");
            }

            return type;
        }

        /// <summary>
        /// Name of snippets file used for xaml generation.
        /// </summary>
        protected string SnippetsFileName
        {
            set
            {
                if (!String.Equals(s_snippetsFileName, value, StringComparison.InvariantCultureIgnoreCase))
                {
                    s_snippetsFileName = value;
                    s_testDoc = null;
                }
            }
        }

        /// <summary>
        /// Name of template file used for xaml generation.
        /// </summary>
        protected string EmptyFileName
        {
            set
            {
                if (!String.Equals(s_emptyFileName, value, StringComparison.InvariantCultureIgnoreCase))
                {
                    s_emptyFileName = value;
                    s_testDoc = null;
                }
            }
        }

        /// <summary>
        /// Returns the XamlTestDocument for the xaml template and snippets.
        /// </summary>
        protected XamlTestDocument TestDocument
        {
            get
            {
                if (s_testDoc == null)
                {
                    s_testDoc = new XamlTestDocument(s_emptyFileName, s_snippetsFileName);
                }

                return s_testDoc;
            }
        }

        /// <summary>
        /// Returns a value to use for the property in xaml. 
        /// Custom values may be defined in a xaml snippets file. 
        /// The value returned is the first Value in xaml snippets file. 
        /// The property's default value will be returned if no custom value is available.
        /// </summary>
        protected static object GetPropertyValue(XamlTestDocument doc, string typeName, DependencyPropertyDescriptor dpd)
        {
            object value = GetPropertyValue(doc, typeName, dpd, 0);

            // Use default value if a match wasn't found in the snippets file.
            // Otherwise, when a match is found, return the xml snippet root if
            // it's a complex value.  Return the trimmed string if it's a simple value.
            if (value == null)
            {
                value = dpd.Metadata.DefaultValue;
            }

            return value;
        }

        /// <summary>
        /// Returns a value to use for the property in xaml. 
        /// Custom values may be defined in a xaml snippets file.
        /// Return null if the property has not been defined. 
        /// </summary>
        protected static object GetPropertyValue(XamlTestDocument doc, string typeName, DependencyPropertyDescriptor dpd, int index)
        {
            object value = null;
            DependencyProperty dp = dpd.DependencyProperty;
            string propName = dp.Name;

            // ValueType and ValueOwnerType and PropertyName
            XmlElement propertyTestValueElement = doc.GetSnippetByXPath("//av:PropertyTestValue[@ValueType='" + dp.PropertyType.Name + "' and @ValueOwnerType='" + typeName + "' and @PropertyName='" + propName + "']", false);

            // ValueType and PropertyName
            if (propertyTestValueElement == null)
                propertyTestValueElement = doc.GetSnippetByXPath("//av:PropertyTestValue[@ValueType='" + dp.PropertyType.Name + "' and not(@ValueOwnerType) and @PropertyName='" + propName + "']", false);

            // ValueType and ValueOwnerType
            if (propertyTestValueElement == null)
                propertyTestValueElement = doc.GetSnippetByXPath("//av:PropertyTestValue[@ValueType='" + dp.PropertyType.Name + "' and @ValueOwnerType='" + typeName + "' and not(@PropertyName)]", false);

            // ValueType
            if (propertyTestValueElement == null)
                propertyTestValueElement = doc.GetSnippetByXPath("//av:PropertyTestValue[@ValueType='" + dp.PropertyType.Name + "' and not(@ValueOwnerType) and not(@PropertyName)]", false);

            // Use default value if a match wasn't found in the snippets file.
            // Otherwise, when a match is found, return the xml snippet root if
            // it's a complex value.  Return the trimmed string if it's a simple value.
            if (propertyTestValueElement != null)
            {
                XmlNodeList valueNodes = propertyTestValueElement.ChildNodes;
                if (valueNodes.Count < index + 1) return null;
                XmlElement valueElement = valueNodes[index] as XmlElement;
                if (String.Equals(propertyTestValueElement.GetAttribute("ValueFormat"), "Simple", StringComparison.InvariantCulture))
                {
                    value = valueElement.InnerText.Trim();
                }
                else
                {
                    value = valueElement.FirstChild;
                }
            }
            return value;
        }

        /// <summary>
        /// Properties to ignore in xaml generation and verification.
        /// </summary>
        /// <returns>Dictionary of PropertyToIgnore items indexed by property name.</returns>
        protected Dictionary<string, PropertyToIgnore> PropertiesToSkip
        {
            get
            {
                if (_propertiesToSkip == null)
                {
                    _propertiesToSkip = TreeComparer.ReadSkipProperties(PropertiesToSkipFileName);
                }

                return _propertiesToSkip;
            }
        }

        /// <summary>
        /// File to use for directing properties to ignore in xaml generation and verification.
        /// </summary>
        protected string PropertiesToSkipFileName = "ActionForType_PropertiesToSkip.xml";

        /// <summary>
        /// Name of xaml file that is generated.
        /// </summary>
        protected string XamlFileName = "__ActionForType.xaml";

        // Gets the valid xml namespace for the given type.
        private string _GetNamespace(Type type)
        {
            string xmlns = s_avalonUri;

            if (!_IsInAvalonNamespace(type))
            {
                xmlns =
                    "clr-namespace:" +
                    type.Namespace +
                    ";assembly=" +
                    type.Assembly.GetName().Name;
            }

            return xmlns;
        }

        // Builds xaml containing a tag for the given type name.
        private Type _BuildAndSaveXaml(string typeName, bool includeProperties, bool useCorrectNamespace)
        {
            return _BuildAndSaveXaml(GetType(typeName), includeProperties, useCorrectNamespace);
        }

        // Builds xaml containing a tag for the given type name.
        // Optionally includes properties in the xaml.
        // Throws if the type cannot be created.
        private Type _BuildAndSaveXaml(Type type, bool includeProperties, bool useCorrectNamespace)
        {
            string xmlns = "WrongNamespace";
            if (useCorrectNamespace)
            {
                xmlns = _GetNamespace(type);
            }

            // Serialize object to xaml and save to file.
            using (FileStream fs = new FileStream(this.XamlFileName, FileMode.Create, FileAccess.Write))
            {
                if (includeProperties)
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
                    if (includeProperties)
                    {
                        PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        for (int i = 0; i < propertyInfos.Length; i++)
                        {
                            PropertyInfo info = propertyInfos[i];

                            // Include property only if it meets certain criteria.
                            if (info.CanRead &&
                                info.CanWrite &&
                                info.GetIndexParameters().Length == 0 &&
                                !ActionForTypeHelper.ShouldIgnoreProperty(info.Name, type, this.PropertiesToSkip))
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
                                    MarkupTestLog.LogStatus("Could not set '" + info.Name + "' property.");
                                }
                            }
                        }
                    }

                    string xaml = SerializationHelper.SerializeObjectTree(obj);

                    // Modify default namespace.
                    int startIndex = 7 + xaml.IndexOf("xmlns=\"", StringComparison.InvariantCultureIgnoreCase);
                    int endIndex = xaml.IndexOf("\"", startIndex, StringComparison.InvariantCultureIgnoreCase);
                    xaml = xaml.Remove(startIndex, endIndex - startIndex);
                    xaml = xaml.Insert(startIndex, xmlns);

                    // Save xaml to file.
                    StreamWriter writer = new StreamWriter(fs);
                    writer.Write(xaml);
                    writer.Close();
                }
                else
                {
                    string typeName = type.FullName.Substring(type.FullName.LastIndexOf(".") + 1);

                    XmlDocument xmlDoc = new XmlDocument();

                    XmlNode rootElement = xmlDoc.CreateElement(typeName, xmlns);
                    xmlDoc.AppendChild(rootElement);
                    xmlDoc.Save(fs);
                }

            }

            return type;
        }

        /// <summary>
        /// Returns the common xmlns manager used for xaml generation in action handlers.
        /// </summary>
        protected static XmlNamespaceManager NamespaceManager
        {
            get
            {
                if (s_nsmgr != null)
                {
                    return s_nsmgr;
                }

                lock (s_syncObject)
                {
                    if (s_nsmgr == null)
                    {
                        // Construct the XmlNamespaceManager used for xpath queries later.
                        NameTable ntable = new NameTable();

                        s_nsmgr = new XmlNamespaceManager(ntable);
                        s_nsmgr.AddNamespace("av", s_avalonUri);
                        s_nsmgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                        s_nsmgr.AddNamespace("cmn", "clr-namespace:Microsoft.Test.Serialization.CustomElements;assembly=TestRuntime");
                    }
                }

                return s_nsmgr;
            }
        }

        /// <summary>
        /// Checks whether or not a type is of a certain type or derives from it.
        /// Checks the non-qualified name, i.e. no namespace check.
        /// </summary>
        protected static bool DoesTypeMatch(Type ownerType, string typeName)
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

        private void _XamlSerialized(object sender, XamlSerializedEventArgs args)
        {
            IOHelper.SaveTextToFile(args.Xaml, _xamlFileName_serialized);
        }

        private static Uri s_baseUri = new Uri(Environment.CurrentDirectory + "\\");
        private static object s_syncObject = new object();
        private static Assembly[] s_assemblies = null;
        private static XmlNamespaceManager s_nsmgr = null;
        private Dictionary<string, PropertyToIgnore> _propertiesToSkip = null;
        private static XamlTestDocument s_testDoc = null;
        private static string s_snippetsFileName = "IntegrationXamlSnippets.xaml";
        private static string s_emptyFileName = null;
        private string _xamlFileName_serialized = "__ActionForType_Serialized.xaml";
        private static readonly string s_avalonUri = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

    }
}
