// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    
    ///<summary>
    ///         Creating a HwndSource with a Visual Root, later using mouseEnter Event swap the visual root.
    ///         For another visual root and validate that the new visual is there.
    ///</summary>
    ///<remarks>
    ///     <ol>Scenarios steps:
    ///         <li>Creating 1 context and Enter the context, mouse is on 0,0 coordinates</li>
    ///         <li>Create a VisualRoot that contains a White Text</li>
    ///         <li>When the visual is render, mouve the mouse over the visual </li>
    ///         <li>On mouse enter we move the mouse to  0,0 Coordinates and we post a background item to execute the next step</li>
    ///         <li>On the posted item we swap the visual root for a new visual (Red Text) and also this visual handles on mouse enter</li>
    ///         <li>To validate the new visual is there we move the mouse over the new visual this should fire onmouseenter</li>
    ///         <li>During on Mouse enter we set a flag that the test pass and we close the window and stop the dispatcher</li>
    ///         <li>The dispathcer stop and we validate that everything pass.</li>
    ///     </ol>
    ///     <filename>ChangingRootVisual.cs</filename>
    ///</remarks>
    [Test(0, @"Source\RootVisual\Simple", TestCaseSecurityLevel.FullTrust, "ChangingRootVisual", Area = "AppModel")]
    public class ChangingRootVisual : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public ChangingRootVisual() :base(TestCaseType.ContextSupport){}
        
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        override public void Run()
        {

            CoreLogger.BeginVariation();
            HelloElement Hello;
            HwndSource Source;            


			
            using (CoreLogger.AutoStatus("Creating a HelloElement to render"))
            {
                Hello = new HelloElement();
        
                Hello.RenderedSourcedHandlerEvent+=new Avalon.Test.CoreUI.Source.HelloElement.RenderHandler(FirstControl_RenderHandler);
            }

            using (CoreLogger.AutoStatus("Creating a HwndSource"))
            {
                
                Source = SourceHelper.CreateHwndSource( 400, 400,0,0);                
                Hello.Source = Source;
                Source.RootVisual = Hello;
                s_source = Source;
            }

            using (CoreLogger.AutoStatus("Dispather.Run"))
            {            
                Dispatcher.Run();

            }

            using (CoreLogger.AutoStatus("Validating Handle on the HwndSource"))
            {            
                if (!s_isTestPass)
                {
                    throw new Microsoft.Test.TestValidationException("The test didn't pass. Mouse Event was not receive on the new RootVisual");
                }
            }
            CoreLogger.EndVariation();
        }   
        

        /// <summary>
        /// This will be called when OnRender on First HelloElement is called.
        /// It will post a delegate that will Input a mouse move. The function called after this
        /// is IntroduceFirstInputHandler
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Source"></param>
        private void FirstControl_RenderHandler(UIElement target, HwndSource Source)
        {
            if (!_isCalled)
            {
                _isCalled = true;                
                using(CoreLogger.AutoStatus("Post a move to the position (100,100) First Mouse Move"))
                {
                    target.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(IntroduceInputHandler), Source);
                }
            }
        }
        bool _isCalled = false;

        /// <summary>
        /// Adding a delegate to the MouseEnter event on the First Control Render.
        /// Later it will move the move to the positon 100,100 of the Source Window, this
        /// will cause the delegate just added to be fired. The next function called will be
        /// FirstControl_MouseEnterHandler
        /// </summary>
        /// <param name="o">This is the Source window</param>
        /// <returns></returns>
        object IntroduceInputHandler (object o)
        {
            if (!_isCalledBar)
            {
                _isCalledBar = true;
                using(CoreLogger.AutoStatus("Move the mouse to (100,100)"))
                {
                    ((UIElement)s_source.RootVisual).MouseEnter += new MouseEventHandler(FirstControl_MouseEnterHandler);
                    MouseHelper.Move((UIElement)s_source.RootVisual); 
                }

                using (CoreLogger.AutoStatus("Move Mouse to 0,0 "))
                {
                    MouseHelper.MoveOnVirtualScreenMonitor();
                    Thread.Sleep(3000);
                    CoreLogger.LogStatus("aaaa");
                }
                
            }
            return null;
        }
        bool _isCalledBar = false;

        /// <summary>
        /// On MouseEnter: first move the mouse to (0,0) later we create a new Element and we swap the RootVisual from the Source
        /// also we add a delegate to fire an event when the Second HelloElement is rendered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstControl_MouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!_isCalledTwo)
            {
                _isCalledTwo = true;
               
                ((UIElement)sender).Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(SwapTree), null);
            }
        }

        bool _isCalledTwo = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        object SwapTree(object o)
        {
            HelloElement h;

            using(CoreLogger.AutoStatus("Create a new HelloElement with Red Font Color"))
            {
                h = new HelloElement();
                h.FontColor = Brushes.Red;
             
            }

            using(CoreLogger.AutoStatus("Adding a handler on the new HelloElement for Rendering"))
            {
                h.RenderedSourcedHandlerEvent += new Avalon.Test.CoreUI.Source.HelloElement.RenderHandler(SecondControl_RenderHandler);
            }

            using(CoreLogger.AutoStatus("Switch the new RootVisual"))
            {
                

                h.Source = s_source;
                s_source.RootVisual = h;
                                    
                                                       
            }

            return null;
        }

        /// <summary>
        /// When the second HelloElement is rendered a Second input is genereated and the SecondControlMouseEntherHandler will be file. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Source"></param>
        private void SecondControl_RenderHandler(UIElement target, HwndSource Source)
        {
            if (!_isCalledThree)
            {
                _isCalledThree = true;
                ((UIElement)target).Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(MoveMouseDelayActiveMouseEnterNewElement), target);                       
            }
        }


        bool _isCalledThree = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        object MoveMouseDelayActiveMouseEnterNewElement(object o)
        {
            UIElement target = (UIElement)s_source.RootVisual;
            using(CoreLogger.AutoStatus("Move the mouse to (100,100) Second Input"))
            {        
                target.MouseEnter +=new MouseEventHandler(SecondControl_MouseEnterHandler);
                MouseHelper.Move((UIElement)s_source.RootVisual);                 
            }
            return null;
        }
        /// <summary>
        /// MouseEnter of the second controls handler. Set the Global variabel IsTestPass to true and Close the window to 
        /// stop the test
        /// </summary>      
        private void SecondControl_MouseEnterHandler(object sender, MouseEventArgs e)
        {
            s_isTestPass = true;
            using(CoreLogger.AutoStatus("BeginInvoke on Background Priority to Close the Window"))
            {                
                s_source.RootVisual.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(CloseWindowASyncHandler), s_source);
            }
            
        }


        /// <summary>
        /// This handler will be posted to the Context to Close the windows and Stop the dispatcher
        /// </summary>
        /// <param name="o"></param>
        object CloseWindowASyncHandler(object o)
        {
            HwndSource Source = (HwndSource)o;
            using(CoreLogger.AutoStatus("Call Win32.DestroyWindow and Stop the dispatcher"))
            {
                Source.RootVisual = null;
                Microsoft.Test.Win32.NativeMethods.DestroyWindow( new HandleRef(null,Source.Handle) )  ;
                MainDispatcher.InvokeShutdown();
            }
            return null;
        }



        static HwndSource s_source;
        static bool s_isTestPass = false;


        
    }
}


