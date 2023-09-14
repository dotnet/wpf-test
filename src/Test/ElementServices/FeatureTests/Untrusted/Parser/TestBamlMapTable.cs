// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: using reflection to cover the behavior of BamlMapTable.
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.IO;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Test.Serialization;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A test to cover methods in BamlMapTable with reflection.
    /// </summary>
    [Test(1,@"Serialization\BamlMapTable",TestCaseSecurityLevel.FullTrust,"A test to cover methods in BamlMapTable with reflection.",
       MethodName = "Run", SupportFiles = @"FeatureTests\ElementServices\StyleOverride07.xaml,FeatureTests\ElementServices\ButtonBVTComponentModel.xaml,FeatureTests\ElementServices\StyleSerializationCase20.xaml,FeatureTests\ElementServices\TestElementInGlobalNameSpace.xaml", Area = "XAML", Disabled=true)]
    public class TestBamlMapTable
    {
        /// <summary>
        /// Entrance
        /// </summary>
        public TestBamlMapTable()
        {
            _asmName = typeof(FrameworkElement).Assembly.GetName().FullName;
        }

        /// <summary>
        /// Entrance
        /// </summary>
        public void Run()
        {
            CoreLogger.BeginVariation();
            CoreLogger.LogStatus("Verifying BamlMapTable with reflection.");
            CoreLogger.LogStatus("Loading Assembly ...");
            Assembly assembly = Assembly.Load(_asmName);
            CoreLogger.LogStatus("Finding BamlMapTable Type ...");
            _BamlMapTableType = assembly.GetType("System.Windows.Markup.BamlMapTable", true, true);
            CoreLogger.LogStatus("Creating BamlMapTable Instance ...");
            string[] namespaces = {"PresentationCore", "PresentationFramework", "WindowsBase"};

            XamlTypeMapper mapper = new XamlTypeMapper(namespaces);
            object[] args = new object[1];
            args[0] = mapper;
            _BamlMapTableInstance = Activator.CreateInstance(_BamlMapTableType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, args, System.Globalization.CultureInfo.InvariantCulture);

            TestEachMethod();
            TestReusingParerContext();
            CoreLogger.EndVariation();
        }
        void TestReusingParerContext()
        {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(LoadAndDisplayTrees), null);
            Dispatcher.Run();
        }
        
        object LoadAndDisplayTrees(object obj)
        {
            string[] xamlfiles = { "StyleOverride07.xaml", "ButtonBVTComponentModel.xaml", "StyleSerializationCase20.xaml", "TestElementInGlobalNameSpace.xaml"};
            ParserContext pc = new ParserContext();
            pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
            SerializationHelper helper = new SerializationHelper();
            object treeRoot;
            foreach (string fileName in xamlfiles)
            {
                Stream stream = File.OpenRead(fileName);
                treeRoot = XamlReader.Load(stream, pc);
                helper.DisplayTree(treeRoot, "Display tree", true);
                stream.Close();
            }
            // Shut down the dispatcher
            Dispatcher.CurrentDispatcher.InvokeShutdown();
            return null;
        }
        private void TestEachMethod()
        {
            TestSeal();
            TestCreateKnownTypeFromId();
            TestGetKnownTypeFromId();
            TestGetKnownTypeIdFromName();
            TestGetKnownTypeIdFromType();
            TestShouldBypassCustomCheck();
        }
        private void TestSeal()
        {
            string methodName = "Seal";
            CoreLogger.LogStatus("Testing method : " + methodName + " ...");
            MethodInfo mInfo;
            mInfo = _BamlMapTableType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Debug.Assert((mInfo != null), "Can't find Method with name = " + methodName + ".");
            mInfo.Invoke(_BamlMapTableInstance, null);

            PropertyInfo pInfo = _BamlMapTableType.GetProperty("IsSealed", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            bool isSealed = (bool)pInfo.GetValue(_BamlMapTableInstance, null);
            Debug.Assert(isSealed, "Can't find Method with name = " + methodName + ".");

        }
        private void TestCreateKnownTypeFromId()
        {
            string methodName = "CreateKnownTypeFromId";
            CoreLogger.LogStatus("Testing method : " + methodName + " ...");
            MethodInfo mInfo;
            mInfo = _BamlMapTableType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert((mInfo != null), "Can't find Method with name = " + methodName + ".");
            object[] args = new object[1];
            short one = 1;
            args[0] = one;
            object returnvalue = mInfo.Invoke(_BamlMapTableInstance, args);
            Debug.Assert(returnvalue==null, "Return value for a positive id should be null.");
        }
        void TestGetKnownTypeFromId()
        {
            string methodName = "GetKnownTypeFromId";
            CoreLogger.LogStatus("Testing method : " + methodName + " ...");
            MethodInfo mInfo;
            mInfo = _BamlMapTableType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert((mInfo != null), "Can't find Method with name = " + methodName + ".");
            object[] args = new object[1];
            short one = 1;
            args[0] = one;

            object returnvalue = mInfo.Invoke(null, args);
            Debug.Assert(returnvalue == null, "Return value for a positive id should be null.");
        }
        void TestGetKnownTypeIdFromName()
        {
            string methodName = "GetKnownTypeIdFromName";
            CoreLogger.LogStatus("Testing method : " + methodName + " ...");
            MethodInfo mInfo;
            mInfo = _BamlMapTableType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert((mInfo != null), "Can't find Method with name = " + methodName + ".");
            object[] args = new object[2];
            
            args[0] = "";
            args[0] = "";
            short returnvalue = (short)mInfo.Invoke(null, args);
            Debug.Assert(returnvalue == 0, "Return value for an empty sting should be 0.");
        }
        void TestGetKnownTypeIdFromType()
        {
            string methodName = "GetKnownTypeIdFromType";
            CoreLogger.LogStatus("Testing method : " + methodName + " ...");
            MethodInfo mInfo;
            mInfo = _BamlMapTableType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert((mInfo != null), "Can't find Method with name = " + methodName + ".");
            object[] args = new object[1];

            args[0] = null;
            short returnvalue = (short)mInfo.Invoke(null, args);
            Debug.Assert(returnvalue == 0, "Return value for an empty sting should be 0.");
        }
        private void TestShouldBypassCustomCheck()
        {
            string methodName = "ShouldBypassCustomCheck";
            CoreLogger.LogStatus("Testing method : " + methodName + " ...");
            MethodInfo mInfo;
            mInfo = _BamlMapTableType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert((mInfo != null), "Can't find Method with name = " + methodName + ".");
            object[] args = new object[2];
            args[0] = null;
            args[1] = null;
            bool returnvalue = (bool)mInfo.Invoke(_BamlMapTableInstance, args);
            Debug.Assert(returnvalue, "Should return true value.");
            args[0] = typeof(Button);
            args[1] = null;
            returnvalue = (bool)mInfo.Invoke(_BamlMapTableInstance, args);
            Debug.Assert(returnvalue, "Should return true value.");
        }
        /// <summary>
        /// Verification for xaml TestElementInGlobalNameSpace.xaml
        /// </summary>
        /// <param name="root"></param>
        public static void VerifyTypesInGlobalNamespace(UIElement root)
        {
            FrameworkElement fe = root as FrameworkElement;
            Button control1 = fe.FindName("Control1") as Button;
            VerifyElement.VerifyBool(control1 != null, true);
            ControlTemplate template = control1.Template;
            VerifyElement.VerifyBool(template != null, true);
            ButtonInGlobalNameSpace buttonInTemplate = template.FindName("buttonInTemplate", control1) as ButtonInGlobalNameSpace;
            VerifyElement.VerifyBool(buttonInTemplate != null, true);

            ContentControl control2 = fe.FindName("Control2") as ContentControl;
            VerifyElement.VerifyBool(control2 != null, true);
            ButtonInGlobalNameSpace buttonContent = control2.Content as ButtonInGlobalNameSpace;
            VerifyElement.VerifyBool(buttonContent != null, true);

            ButtonInGlobalNameSpace control3 = fe.FindName("Control3") as ButtonInGlobalNameSpace;
            VerifyElement.VerifyBool(control3 != null, true);
            SolidColorBrush brush = control3.Background as SolidColorBrush;
            VerifyElement.VerifyBool(brush != null, true);
            VerifyElement.VerifyColor(brush.Color, Colors.Red);

            ContentControl control4 = fe.FindName("Control4") as ContentControl;
            VerifyElement.VerifyBool(control4 != null, true);
            BoldInGlobalNameSpace boldContent = control4.Content as BoldInGlobalNameSpace;
            VerifyElement.VerifyBool(boldContent != null, true);

            ContentControl control5 = fe.FindName("Control5") as ContentControl;
            VerifyElement.VerifyBool(control5 != null, true);
            boldContent = control4.Content as BoldInGlobalNameSpace;
            VerifyElement.VerifyBool(boldContent != null, true);
        }
        
        private object _BamlMapTableInstance;
        private Type _BamlMapTableType;
        private string _asmName = "";

    }
}
