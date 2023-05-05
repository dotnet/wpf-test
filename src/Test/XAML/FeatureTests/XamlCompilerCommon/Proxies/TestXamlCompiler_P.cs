// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xaml;
using System.Collections;

namespace CodeGenInspect.Proxies
{
    public class TestXamlCompiler
    {
        static ProxyHelper s_testXamlCompilerType;

        static MethodInfo s_createCompilerDomRoot;
        static MethodInfo s_test_HarvestMethod;
        static MethodInfo s_test_CodeGenMethod;
        static PropertyInfo s_xamlTextReaderProperty;
        static PropertyInfo s_localAssemblyProperty;
        static PropertyInfo s_schemaContextProperty;
        static PropertyInfo s_validationErrorsProperty;
        static PropertyInfo s_classCodeInfoProperty;

        object _instance;

        static TestXamlCompiler()
        {
            Assembly asm = XamlClassCodeInfo.XamlCompilerCoreAssembly;
            s_testXamlCompilerType = new ProxyHelper(asm, "SampleTaskCode.TestXamlCompiler");

            s_createCompilerDomRoot = s_testXamlCompilerType.GetMethod("CreateCompilerDomRoot");
            s_test_HarvestMethod = s_testXamlCompilerType.GetMethod("Test_Harvest");
            s_test_CodeGenMethod = s_testXamlCompilerType.GetMethod("Test_CodeGen");

            s_xamlTextReaderProperty = s_testXamlCompilerType.GetProperty("XamlTextReader");
            s_localAssemblyProperty = s_testXamlCompilerType.GetProperty("LocalAssembly");
            s_schemaContextProperty = s_testXamlCompilerType.GetProperty("SchemaContext");
            s_validationErrorsProperty = s_testXamlCompilerType.GetProperty("ValidationErrors");
            s_classCodeInfoProperty = s_testXamlCompilerType.GetProperty("ClassCodeInfo");

        }

        public TestXamlCompiler(String localAssemblyPath, List<String> referenceAssemblyPaths)
        {
            object[] args = {localAssemblyPath, referenceAssemblyPaths};
            _instance = s_testXamlCompilerType.CreateInstance(args);
        }

        public CompilerDomRoot CreateCompilerDomRoot(XamlReader xamlReader, XamlSchemaContext schema)
        {
            object[] args = { xamlReader, schema };
            object ret = s_createCompilerDomRoot.Invoke(_instance, args);
            return new CompilerDomRoot(ret);
        }

        public void Test_Harvest(string xamlFileName)
        {
            object[] args = new object[] { xamlFileName };
            s_test_HarvestMethod.Invoke(_instance, args);
        }

        public List<KeyValuePair<String, String>> Test_CodeGen(string language)
        {
            object[] args = new object[] { language };
            var retValue = (List<KeyValuePair<String, String>>)s_test_CodeGenMethod.Invoke(_instance, args);
            return retValue;
        }

        public TextReader XamlTextReader
        {
            get { return (TextReader)s_xamlTextReaderProperty.GetValue(_instance, null); }
            set { s_xamlTextReaderProperty.SetValue(_instance, value, null); }
        }

        public Assembly LocalAssembly
        {
            get { return (Assembly)s_localAssemblyProperty.GetValue(_instance, null); }
        }

        public XamlSchemaContext SchemaContext
        {
            get { return (XamlSchemaContext)s_schemaContextProperty.GetValue(_instance, null); }
        }

        public List<XamlValidationError> ValidationErrors
        {
            get
            {
                IEnumerable objectList = (IEnumerable)s_validationErrorsProperty.GetValue(_instance, null);
                List<XamlValidationError> errorList = null;
                if (objectList != null)
                {
                    errorList = new List<XamlValidationError>();
                    foreach (Object obj in objectList)
                    {
                        XamlValidationError error = new XamlValidationError(obj);
                        errorList.Add(error);
                    }
                }
                return errorList;
            }
        }

        public XamlClassCodeInfo ClassCodeInfo
        {
            get
            {
                Object instance = s_classCodeInfoProperty.GetValue(_instance, null);
                XamlClassCodeInfo codeInfo = new XamlClassCodeInfo(instance);
                return codeInfo;
            }
        }
    }
}
