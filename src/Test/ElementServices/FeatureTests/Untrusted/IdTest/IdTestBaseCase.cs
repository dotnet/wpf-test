// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.IdTest
{
    /// <summary>
    /// A base class for class to test IdScoping. It provides 
    /// some function most other test case may use.
    /// </summary>
    public class IdTestBaseCase : WindowTest
    {
        /// <summary>
        /// A window
        /// </summary>
        protected Window _win = null;
        /// <summary>
        /// A Canvas
        /// </summary>
        protected Canvas _canvas = null;
        /// <summary>
        /// A FrameworkElement
        /// </summary>
        protected Button _fe = null;
        /// <summary>
        /// A FrameworkContentElement
        /// </summary>
        protected FrameworkContentElement _fce = null;
        /// <summary>
        /// A ControlTemplate
        /// </summary>
        protected ControlTemplate _controlTemplate = null;

        /// <summary>
        /// To create the object tree with a window, a canvas 
        /// and a button.
        /// </summary>
        /// <returns>root of the tree created</returns>
        protected object CreateTree()
        {
            SurfaceFramework surface = new SurfaceFramework("Window", 0,0,300,300);

            _canvas = new Canvas();
            
            Button button = new Button();
            _fe = button;
            _fe.Resources.Add("resource1", new object());

            Bold bold = new Bold();
            _fce = bold;
            button.Content = bold;

            _canvas.Children.Add(button);
            surface.DisplayObject( _canvas);

            _win = (Window)surface.SurfaceObject;

            NameScope.SetNameScope(_win, new NameScope());

            _controlTemplate = _win.Template;

            return _win;
        }

        /// <summary>
        /// Load object tree from xaml
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected object LoadFromXaml(string fileName)
        {
            return SerializationHelper.ParseXamlFile(fileName);
        }

        /// <summary>
        /// Find an object with certain Id begin from an element or a INameScope
        /// </summary>
        /// <param name="startElement"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static object FindElementWithId(object startElement, string id)
        {
            if (startElement is FrameworkElement)
                return ((FrameworkElement)startElement).FindName(id);
            else if (startElement is FrameworkContentElement)
                return ((FrameworkContentElement)startElement).FindName(id);
            else if(startElement is INameScope)
                return ((INameScope)startElement).FindName(id);
            return null;
        }

        /// <summary>
        /// Abstraction of RegisterName
        /// </summary>
        /// <param name="id"></param>
        /// <param name="elementToRegister"></param>
        /// <param name="whereToRegister"></param>
        protected void RegisterName(string id, object elementToRegister, object whereToRegister)
        {
            if(whereToRegister is INameScope)
            {
                INameScope scope = (INameScope)whereToRegister;
                scope.RegisterName(id, elementToRegister);
            }
            else if(whereToRegister is FrameworkElement)
            {
              ((FrameworkElement)whereToRegister).RegisterName(id, elementToRegister);
            }
            else if(whereToRegister is FrameworkContentElement)
            {
              ((FrameworkContentElement)whereToRegister).RegisterName(id, elementToRegister);
            }
            else
            {
               throw new Microsoft.Test.TestValidationException("WhereToRegister type is not allowed.");
            }

        }

        /// <summary>
        /// Abstraction of UnregisterName
        /// </summary>
        /// <param name="id"></param>
        /// <param name="whereToRegister"></param>
        protected void UnregisterName(string id, object whereToRegister)
        {
            if(whereToRegister is INameScope)
            {
                INameScope scope = (INameScope)whereToRegister;
                scope.UnregisterName(id);
            }
            else if(whereToRegister is FrameworkElement)
            {
              ((FrameworkElement)whereToRegister).UnregisterName(id);
            }
            else if(whereToRegister is FrameworkContentElement)
            {
              ((FrameworkContentElement)whereToRegister).UnregisterName(id);
            }
            else
            {
               throw new Microsoft.Test.TestValidationException("WhereToRegister type is not allowed.");
            }

        }

        /// <summary>
        /// Verify a field exists.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        //[ReflectionPermission(SecurityAction.Assert, Unrestricted = true)]
        public static FieldInfo VerifyFieldExist(object instance, string fieldName)
        {
            if (!IsCompilationCase()) return null;

            CoreLogger.LogStatus("Getting Type...");
            Type type = instance.GetType();
            CoreLogger.LogStatus("Getting the field ...");

            FieldInfo field =  null;

            field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if(null == field)
            {
                //For the case compile with VB, there is a 

                field = type.GetField("_" + fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            }              

            if (null == field)
            {
                throw new Microsoft.Test.TestValidationException("Field: " + fieldName + " not found.");
            }

            return field;
        }

        /// <summary>
        /// Return whether the current case is an application.
        /// </summary>
        /// <returns></returns>
        static bool IsCompilationCase()
        {
            return null != Application.Current;
        }

        /// <summary>
        /// Verify a field exist in an instance, and the value for that field is expected.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <param name="expectedValue"></param>
        public static void VerifyFieldValue(object instance, string fieldName, object expectedValue)
        {        
            if(!IsCompilationCase()) return;

            FieldInfo field = VerifyFieldExist(instance, fieldName);

            CoreLogger.LogStatus("Verifying the value for field...");
            object actualValue = field.GetValue(instance);
            if (actualValue != expectedValue)
            {
                throw new Microsoft.Test.TestValidationException("Value for field: " + fieldName + " is different.");
            }
        }
    }    
}
