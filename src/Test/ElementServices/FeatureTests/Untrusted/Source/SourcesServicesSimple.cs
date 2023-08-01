// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Win32;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    ///         Validating the SourceServices works correctly
    ///         
    /// </summary>
    /// <remarks>
    ///     <Owner>Microsoft</Owner>
 
    ///     <Area>Source\SimpleThreadSingleContext</Area>
    ///     <location>SourcesServicesSimple.cs</location>
    ///</remarks>
    [TestDefaults]
    public class SourcesServicesSimple : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public SourcesServicesSimple() :base(TestCaseType.HwndSourceSupport){}
        
        /// <summary>
        /// Retrieve the SourceFromFrameworkElement from a alement that it is no on the tree, later on the tree and after unplugged again
        /// </summary>
        /// <remarks>
        ///     <ol>Scenarios steps:
        ///         <li>Create 1 Source and a tree with Border-StackPanel->TextPanel->Button</li>
        ///         <li>QUery SourceFromFrameworkElement on the TP.  Validate Null</li>
        ///         <li>Connect the RootVisual to the Border. Post an item to validate Async</li>
        ///         <li>Run Disptacher</li>
        ///         <li>Validate the TP has source. and later Set the RootVisual to null and validate async again, later exit the dispatcher</li>
        ///         <li>Validate the Async call where done</li>
        ///         
        ///     </ol>
        ///     <Owner>Microsoft</Owner>
 
        ///     <Area>Source\SourceServices\Simple</Area>
        ///     <location>SourcesServicesSimple.cs</location>
        ///</remarks>        
        [Test(0, @"Source\PresentationSource\FromVisual", TestCaseSecurityLevel.FullTrust, "Retrieve the SourceFromFrameworkElement from a alement that it is no on the tree, later on the tree and after unplugged again", Area = "AppModel")]
        override public void Run()
        {
            CoreLogger.BeginVariation();
            FlowDocumentScrollViewer tp;
            Border border;

            using(CoreLogger.AutoStatus("Creating a tree"))
            {

                border = new Border();
                border.Background = Brushes.Cyan;

                Paragraph para= new Paragraph();
                StackPanel sp = new StackPanel();

                border.Child = sp;            

				tp = new FlowDocumentScrollViewer();
                tp.Document = new FlowDocument(para);

                sp.Children.Add(tp);

                Button b = new Button();

                b.Content = "My Button";
                b.Width = 200;
                b.Height = 200;

                InlineUIContainer uiElementContainer = new InlineUIContainer(b);
                para.Inlines.Add(uiElementContainer);
                //para.ContentEnd.InsertUIElement(b);

            }
            using(CoreLogger.AutoStatus("Validating from Unplugged tree"))
            {
                PresentationSource source  = PresentationSource.FromVisual(tp);

                if (source != null)
                   throw new Microsoft.Test.TestValidationException("The PresentationSource should be null");
            }

            Source.RootVisual = border;

            DispatcherOperation operation = MainDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(validateSource), tp);

            using(CoreLogger.AutoStatus("Dispatcher.Run"))
            {
                Dispatcher.Run();
            }
            
            using(CoreLogger.AutoStatus("Last Validation for Items be dispatched"))
            {
               if ( ! ((bool)operation.Result)  || !_isPassUnplugged)
                   throw new Microsoft.Test.TestValidationException("Expecting a true result");
            }
            CoreLogger.EndVariation();
        }


        object validateSource(object o)
        {
            FlowDocumentScrollViewer tp = o as FlowDocumentScrollViewer;
            
            using(CoreLogger.AutoStatus("Validating from Plugged tree"))
            {
                PresentationSource source  = PresentationSource.FromVisual(tp);

                if (source != (PresentationSource)Source)
                    throw new Microsoft.Test.TestValidationException("The PresentationSource should NOT be null");
            }

             using(CoreLogger.AutoStatus("RootVisual = Null"))
            {
                Source.RootVisual = null;
            }

                
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(validateSourceUnplugged), tp);

            return true;
        }

        object validateSourceUnplugged(object o)
        {
            FlowDocumentScrollViewer tp = o as FlowDocumentScrollViewer;

            using(CoreLogger.AutoStatus("Validating from UNPlugged tree Again"))
            {
                PresentationSource source  = PresentationSource.FromVisual(tp);

                if (source != null)
                    throw new Microsoft.Test.TestValidationException("The PresentationSource should be null. After Unplugged");

            }

        
            _isPassUnplugged = true;
                
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(exitDispatcher), null);
            return true;
        }





        object exitDispatcher(object o)
        {

            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            
            return false;
        }



        bool _isPassUnplugged = false;
        
    }
}



