// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//

#region Using Statements

using System;
using System.IO;
using System.Reflection;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;

#endregion

namespace Avalon.Test.ComponentModel.Accessibility
{
    /// <summary>
    /// XamlReader loads a xaml file and put the root element inside a window.
    /// 
    /// Usage: XamlReader xamlfile
    /// </summary>
    public sealed class XamlReader : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            object element = null;

            // get element from xaml file
            if (e.Args.Length == 1)
            {
                string fileName = e.Args[0];
                element = LoadXaml(fileName);
            }

            // put element inside window
            MainWindow.Content = element;
            MainWindow.Show();
        }

        [STAThread]
        public static void Main(String[] args)
        {
            Application app = new XamlReader();
            Window window = new Window();
            window.Title = "Xaml Loader";
            app.Run(window);   
        }

        // load xaml file and return the root element
        private static object LoadXaml(string fileName)
        {
            string content;
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    content = sr.ReadToEnd();
                }
            }

            using (MemoryStream stream = new MemoryStream(content.Length))
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.Write(content);
                    sw.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    ParserContext pc = new ParserContext();
                    pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                    return System.Windows.Markup.XamlReader.Load(stream, pc);
                }
            }
        }
    }

    /// <summary>
    /// a control that is used to invoke a method on an object.
    /// another process can invoke the control to do method invoking in XamlReader process.
    /// </summary>
    public class MethodInvoker : Button
    {
        private static RoutedCommand s_invokeCommand;

        public static RoutedCommand Invoke
        {
            get
            {
                if (s_invokeCommand == null)
                {
                    s_invokeCommand = new RoutedCommand("Invoke", typeof(MethodInvoker));
                    CommandManager.RegisterClassCommandBinding(typeof(MethodInvoker), new CommandBinding(s_invokeCommand, DoInvoke, CanInvoke));
                }
                return s_invokeCommand;
            }
        }

        private static void DoInvoke(object sender, ExecutedRoutedEventArgs e)
        {
            MethodInvoker invoker = (MethodInvoker)sender;
            invoker.DoInvoke(e);
        }

        private static void CanInvoke(object sender, CanExecuteRoutedEventArgs e)
        {
            MethodInvoker invoker = (MethodInvoker)sender;
            invoker.CanInvoke(e);
        }

        private void CanInvoke(CanExecuteRoutedEventArgs e)
        {
            if (Target == null)
            {
                e.CanExecute = false;
                return;
            }

            if (string.IsNullOrEmpty(Method))
            {
                e.CanExecute = false;
                return;
            }

            MethodInfo mi = Target.GetType().GetMethod(Method);
            if (mi == null)
            {
                e.CanExecute = false;
                return;
            }

            ParameterInfo[] pis = mi.GetParameters();
            if (pis.Length != Parameters.Count)
            {
                e.CanExecute = false;
                return;
            }

            for (int i = 0; i < pis.Length; ++i)
            {
                if (!pis[i].ParameterType.IsInstanceOfType(Parameters[i]))
                {
                    e.CanExecute = false;
                    return;
                }
            }

            e.CanExecute = true;
        }

        private void DoInvoke(ExecutedRoutedEventArgs e)
        {
            MethodInfo mi = Target.GetType().GetMethod(Method);
            object[] paras = new object[Parameters.Count];
            Parameters.CopyTo(paras, 0);
            mi.Invoke(Target, paras);
        }

        /// <summary>
        /// the target of method invoking
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(object), typeof(MethodInvoker));

        public object Target
        {
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        private string _method;

        /// <summary>
        /// the name of the method
        /// </summary>
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        private ObservableCollection<object> _params = new ObservableCollection<object>();

        /// <summary>
        /// the parameter list
        /// </summary>
        public ObservableCollection<object> Parameters
        {
            get { return _params; }
        }
    }
}
