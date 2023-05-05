// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using sx = System.Xaml;

namespace EventsFromME
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Test case passed");
            Application.Current.Shutdown(0);
        }

        private void ClickHandler(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Click event handler!!");
        }
    }

    public class EventExtension : MarkupExtension
    {
        public EventExtension()
        {

        }

        public EventExtension(string eventHandlerName)
        {
            MethodName = eventHandlerName;
        }

        public string MethodName { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget ipvt = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            sx.IRootObjectProvider irop = (sx.IRootObjectProvider)serviceProvider.GetService(typeof(sx.IRootObjectProvider));
            
            if (ipvt.TargetProperty is EventInfo)
            {
                EventInfo targetProperty = (EventInfo)ipvt.TargetProperty;
                //MethodInfo eventHandlerMethodInfo = irop.RootObject.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
                Delegate del = Delegate.CreateDelegate(targetProperty.EventHandlerType, irop.RootObject, MethodName);
                if (del == null)
                {
                    throw new InvalidOperationException("Failed to create delegate");
                }
                return del;
            }
            else
            {
                return null;
            }
        }
    }
}
