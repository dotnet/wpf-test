// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;

namespace DRT
{
    public class DrtThemeDictionaryExtension : DrtTestSuite
    {
        public DrtThemeDictionaryExtension()
            : base("ThemeDictionaryExtension")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.LoadXamlFile("DrtThemeDictionaryExtension.xaml");

            if (!DRT.KeepAlive)
            {
                return new DrtTest[]
                    {
                        new DrtTest(BasicTest),
                        new DrtTest(VerifyProperties),
                    };
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private void BasicTest()
        {
            ThemeDictionaryExtension tde = new ThemeDictionaryExtension();
            DRT.Assert(tde.AssemblyName == null, "AssemblyName should be null by default");
            
            ThemeDictionaryExtension tde1 = new ThemeDictionaryExtension("foo");
            DRT.Assert(tde1.AssemblyName == "foo", "AssemblyName should be set to foo");
            
            ResourceDictionary rd = new ResourceDictionary();
            PropertyInfo sourceProperty = typeof(ResourceDictionary).GetProperty("Source");

            DrtThemeMEServiceProvider serviceProvider = new DrtThemeMEServiceProvider( rd, sourceProperty );
            object uriObject = tde1.ProvideValue(serviceProvider);
            
            DRT.Assert(uriObject is Uri, "tde1 Returned an object that is not a Uri");

            string uri = uriObject.ToString().ToLower();
            DRT.Assert(uri.StartsWith("/foo;component/themes/"), "{0} does not start with /foo;component/themes/", uri);
            DRT.Assert(uri.EndsWith(".xaml"), "{0} does not end with .baml", uri);

            // using null property should work
            serviceProvider = new DrtThemeMEServiceProvider( rd, null );
            uri = tde1.ProvideValue(serviceProvider).ToString().ToLower() ;
            DRT.Assert(uri.StartsWith("/foo;component/themes/"), "{0} does not start with /foo;component/themes/", uri);
            DRT.Assert(uri.EndsWith(".xaml"), "{0} does not end with .baml", uri);


            // using other property/dp should not work
            Button button = new Button();
            
            try
            {
                serviceProvider = new DrtThemeMEServiceProvider( button, Button.BackgroundProperty );
                tde1.ProvideValue(serviceProvider);
                DRT.Fail("ProvideValue did not throw when assigning to Button.BackgroundProperty");
            }
            catch (InvalidOperationException)
            {
                /* good */
            }

        }

        private void VerifyProperties()
        {
            Button button1 = (Button)DRT.FindElementByID("Button1");
            Button button2 = (Button)DRT.FindElementByID("Button2");
            Button button3 = (Button)DRT.FindElementByID("Button3");
            
            DRT.Assert(button1 != null, "Button1 is null");
            DRT.Assert(button2 != null, "Button2 is null");

            Brush systemBrush = button1.Background;
            SolidColorBrush customBrush = button2.Background as SolidColorBrush;
            Brush restoredSystemBrush = button3.Background;

            DRT.Assert(customBrush != null, "Button1 should have solid color background");

            if (systemBrush is SolidColorBrush)
            {
                DRT.Assert(restoredSystemBrush is SolidColorBrush, "Button2 should be a solid color");
                DRT.Assert(((SolidColorBrush)systemBrush).Color == ((SolidColorBrush)restoredSystemBrush).Color, "Button3 should have the same color as the system brush");
            }
            else if (systemBrush is LinearGradientBrush)
            {
                LinearGradientBrush b1 = systemBrush as LinearGradientBrush;
                LinearGradientBrush b2 = restoredSystemBrush as LinearGradientBrush;
                DRT.Assert(b2 != null, "Button3 should be a linear gradient brush");

                DRT.Assert(b1.GradientStops.Count == b2.GradientStops.Count, "Button3 should have the same number of gradient stops as Button1");

                for (int i = 0; i < b1.GradientStops.Count; i++)
                {
                    GradientStop g1 = b1.GradientStops[i];
                    GradientStop g2 = b2.GradientStops[i];

                    DRT.Assert(g1.Color == g2.Color, "Button3 gradient stop " + i + " should have the same color as the system brush");
                    DRT.Assert(g1.Offset == g2.Offset, "Button3 gradient stop " + i + " should have the same offset as the system brush");
                }
            }
            else
            {
                DRT.Fail("Brush type is unknown");
            }
        }
    }


    internal class DrtThemeMEServiceProvider : IServiceProvider, IProvideValueTarget
    {
        /// <summary>
        /// </summary>
        internal DrtThemeMEServiceProvider(object targetObject, object targetProperty)
        {
            _targetObject = targetObject;
            _targetProperty = targetProperty;
        }


        object IProvideValueTarget.TargetObject
        {
            get { return _targetObject; }
        }
        
        object IProvideValueTarget.TargetProperty
        {
            get { return _targetProperty; }
        }

        private object _targetObject = null;
        private object _targetProperty = null;

        /// <summary>
        /// </summary>
        /// <param name="service">Service that is being requested.</param>
        /// <returns>
        ///  An object that can provide the requested service.
        /// </returns>
        public object GetService(Type service)
        {
            if( service == typeof(IProvideValueTarget))
            {
                return this as IProvideValueTarget;
            }

            else
            {
                return null;
            }
                
        }

    }
    
}
