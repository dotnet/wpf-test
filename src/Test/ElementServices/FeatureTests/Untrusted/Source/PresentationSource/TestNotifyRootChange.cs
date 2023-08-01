// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using Avalon.Test.CoreUI.Common;

using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using System.Collections;
using System.Windows.Controls;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    ///     
    ///</summary>
    [TestDefaults]
    public class TestNotifyRootChange : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public TestNotifyRootChange() :base(TestCaseType.None){}
        
        /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChange", Area = "AppModel")]
        override public void Run()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500,500,0,0);

            Button b = new Button();
            
            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandler));

            Source.RootVisual = b;
         
            MainDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(_AsyncHandler), MainDispatcher);
          
            Dispatcher.Run();


            if (!_handlerFirst || !_handlerSecond)
                throw new Microsoft.Test.TestValidationException("The event was not fired");

            FinalReportFailure();
            CoreLogger.EndVariation();
        }

        object _AsyncHandler(object o)
        {

            Border border  = new Border();
            Source.RootVisual = null;

            MainDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

            return null;
        }

        bool _handlerFirst = false;

        bool _handlerSecond = false;

        int _count = 0;

        void NotifyHandler(object o, SourceChangedEventArgs args)
        {
                if (_count ==0)
                {
                    if (args.NewSource != Source || args.OldSource != null)
                        throw new Microsoft.Test.TestValidationException("The First Adding Notification didn't work as expected");

                    _handlerFirst = true;
                }

                if (_count ==1)
                {
                    if (args.NewSource != null || args.OldSource != Source)
                        throw new Microsoft.Test.TestValidationException("The Second Adding Notification didn't work as expected");

                    _handlerSecond = true;
                }
                if (_count > 1)
                    throw new Microsoft.Test.TestValidationException("This should not be called");
                
                _count++;
        }







         /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChangeTwoNewSource", Area = "AppModel")]
         public void TwoNewSource()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500,500,0,0);

            _sourceTwo = SourceHelper.CreateHwndSource(500,500,0,0);



            Button b = new Button();
            

            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandlerTwo));

            Source.RootVisual = b;

            Source.RootVisual = null;
            
            _sourceTwo.RootVisual = b;
          

            if (!_handlerFirst || !_handlerSecond || !_handlerThird)
                throw new Microsoft.Test.TestValidationException("The event was not fired correctly");

            FinalReportFailure();
            CoreLogger.EndVariation();
        }

        bool _handlerThird = false;

        void NotifyHandlerTwo(object o, SourceChangedEventArgs args)
        {
                if (_count == 0)
                {
                    if (args.NewSource != Source || args.OldSource != null)
                        throw new Microsoft.Test.TestValidationException("The First Adding Notification didn't work as expected");

                    _handlerFirst = true;
                }

                if (_count == 1)
                {
                    if (args.NewSource != null || args.OldSource != Source)
                        throw new Microsoft.Test.TestValidationException("The Second Adding Notification didn't work as expected");

                    _handlerSecond = true;
                }

                if (_count == 2)
                {
                    if (args.NewSource != _sourceTwo || args.OldSource != null)
                        throw new Microsoft.Test.TestValidationException("The Third Adding Notification didn't work as expected");

                    _handlerThird = true;
                }
                
                _count++;
        }


        HwndSource _sourceTwo;


         /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChangeInvalidParams", Area = "AppModel")]
         public void InvalidParameters()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;
            
            bool exceptionCalled = false;
           try
           {
                PresentationSource.AddSourceChangedHandler(null, new SourceChangedEventHandler (NotifyHandlerTwo));
           }
           catch(ArgumentNullException)
           {
                exceptionCalled = true;
           }
          
          
            if (!exceptionCalled)
                throw new Microsoft.Test.TestValidationException("Expecting Argument Null Exception first Parameter");

            exceptionCalled = false;

            Button b = new Button();

            PresentationSource.AddSourceChangedHandler(b, null);
           
            PresentationSource.AddSourceChangedHandler(b,new SourceChangedEventHandler (NotifyHandlerTwo));
            CoreLogger.EndVariation();
        }




         /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChangeMovingUIElementLower", Area = "AppModel")]
         public void MovingtheUIElem2Lower()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            // we want to record the shutdown event because we do not want 
            // _ExceptionHandler to evaluate Source.RootVisual when the application is exiting. 
            MainDispatcher.ShutdownStarted += new EventHandler(Dispatcher_ShutdownStarted);

            Source = SourceHelper.CreateHwndSource(500,500,0,0);

            Button b = new Button();
            
            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (_ExceptionHandler));

            Source.RootVisual = b;

            StackPanel rootStackPanel = null;

            StackPanel stackPanel = new StackPanel();

            rootStackPanel = stackPanel;
           
            for (int i=0;i<10;i++)
            {
                StackPanel s = new StackPanel();
                stackPanel.Children.Add(s);
                stackPanel = s;
            }

            Source.RootVisual = rootStackPanel;
          
             stackPanel.Children.Add(b);
             CoreLogger.EndVariation();
        }

        bool _afterDisposed = false;
        void _ExceptionHandler(object o, SourceChangedEventArgs args)
        {
            if (_count == 0)
            {
                if (args.OldSource != null || args.NewSource != Source)
                {
                    throw new Microsoft.Test.TestValidationException("The SourceChangedEventArgs values are not the expected");
                }
            }
            else if (_count == 1)
		    {
                if (args.OldSource != Source || args.NewSource != null)
                {
                    throw new Microsoft.Test.TestValidationException("The SourceChangedEventArgs values are not the expected");
                }
		    }
            else if (_count == 2)
            {
                if (args.OldSource != null || args.NewSource != Source)
                {
                    throw new Microsoft.Test.TestValidationException("The SourceChangedEventArgs values are not the expected");
                }
            }
            else
            {
                // the handler should not called only once - during disposal. If it is called at any other time  - Throw
                if (!_afterDisposed) 
                { 
                    throw new Microsoft.Test.TestValidationException(
                        string.Format("No. of times _ExceptionHandler got called: (exptd:obsrvd) ({0}:{1})", 3, _count)
                        );
                }
            }
            _count++;
         }

         void Dispatcher_ShutdownStarted(object sender, EventArgs e)
         {
             _afterDisposed = true;
         }



        /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChangeMultipleSimpleAddSameTree", Area = "AppModel")]
         public void MultipleSimpleAddSameTree()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500,500,0,0);

            Border border = new Border();
            StackPanel stackPanel = new StackPanel();
            border.Child = stackPanel;
            Button b = new Button();
            stackPanel.Children.Add(b);

            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandlerA));
            PresentationSource.AddSourceChangedHandler(stackPanel, new SourceChangedEventHandler (NotifyHandlerB));
            PresentationSource.AddSourceChangedHandler(border, new SourceChangedEventHandler (NotifyHandlerC));

            Source.RootVisual = border;

            if (_count != 3)
                throw new Microsoft.Test.TestValidationException("Not all the handler are called");

            if (!_handlerFirst || !_handlerSecond || !_handlerThird)
                throw new Microsoft.Test.TestValidationException("The event was not fired");

			MainDispatcher.InvokeShutdown();

			if (_count != 6)
                throw new Microsoft.Test.TestValidationException("Not all the handler are called. After Dispose");
            CoreLogger.EndVariation();
        }

				

         void NotifyHandlerA(object o, SourceChangedEventArgs args)
         {
			 if (_count == 0)
			 {
				 if (!(o is Button))
                     throw new Microsoft.Test.TestValidationException("Expecting a Button");

				 if (args.NewSource != Source || args.OldSource != null)
                     throw new Microsoft.Test.TestValidationException("Button SourceChangeEventArgs are incorrect");

				 _handlerFirst = true;
			 }
            _count++;
         }

        void NotifyHandlerB(object o, SourceChangedEventArgs args)
         {
			 if (_count == 1)
			 {
				 if (!(o is StackPanel))
                     throw new Microsoft.Test.TestValidationException("Expecting a StackPanel");

				 if (args.NewSource != Source || args.OldSource != null)
                     throw new Microsoft.Test.TestValidationException("StackPanel SourceChangeEventArgs are incorrect");

				 _handlerSecond = true;
			 }
            _count++;
         }
        

        void NotifyHandlerC(object o, SourceChangedEventArgs args)
         {
			 if (_count == 2)
			 {
				 if (args.NewSource != Source || args.OldSource != null)
                     throw new Microsoft.Test.TestValidationException("Border SourceChangeEventArgs are incorrect");

				 if (!(o is Border))
                     throw new Microsoft.Test.TestValidationException("Expecting a Border");

				 _handlerThird = true;
			 }
            _count++;
         }

        MyPresentationSource _cSource,_cSourceTwo;

        /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChangeAddSimpleCustomPS", Area = "AppModel")]
         public void AddSimpleCustomPS()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;


            _cSource = new MyPresentationSource(true);
            _cSourceTwo = new MyPresentationSource(true);

            Button b = new Button();

            StackPanel stackPanel = new StackPanel();
            
            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandlerCustomA));
            
            _cSource.RootVisual = b;

             _cSource.RootVisual = stackPanel;

            if (!_handlerFirst || !_handlerSecond)
                throw new Microsoft.Test.TestValidationException("The event was not fired");

        
            _cSource.Dispose();
            _cSourceTwo.Dispose();
            CoreLogger.EndVariation();
        }

        void NotifyHandlerCustomA(object o, SourceChangedEventArgs args)
        {
            if(_count == 0)
            {
                if (args.OldSource != null || args.NewSource != _cSource)
                    throw new Microsoft.Test.TestValidationException("");
                _handlerFirst = true;
            }

            if(_count == 1)
            {
                if (args.OldSource != _cSource || args.NewSource != null)
                    throw new Microsoft.Test.TestValidationException("");

                _handlerSecond = true;
            }

            if (_count > 1)
                throw new Microsoft.Test.TestValidationException("This should not be called");
            _count ++;

        }

        /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChangeSettingSameValueCustomPS", Area = "AppModel")]
         public void SettingSameValueCustomPS()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;


            _cSource = new MyPresentationSource(true);
            _cSourceTwo = new MyPresentationSource(true);

            Button b = new Button();

            
            _cSource.RootVisual = b;
            
            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (_exceptionHandler));
            

            _cSource.RootVisual = b;
        
            _cSource.Dispose();
            CoreLogger.EndVariation();

        }

        void _exceptionHandler(object o, SourceChangedEventArgs args)
        {
            throw new Microsoft.Test.TestValidationException("This should not be called");
        }



        /// <summary>
        /// Create an HwndSource and Button. AddSourceChangedHandler should be called when the button is assigned to 
        /// HwndSource.RootVisual. Removing the button from the tree should call the event handler again.
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChangeAddAndRemove", Area = "AppModel")]
		public void AddSourceChangedAndRemoveFromTree()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;


            _cSource = new MyPresentationSource(true);

            StackPanel fpOne = new StackPanel();
            StackPanel fpTwo = new StackPanel();
            StackPanel fpThree = new StackPanel();

            Button b = new Button();
            
            fpOne.Children.Add(fpTwo);
            fpTwo.Children.Add(fpThree);
            fpThree.Children.Add(b);
            
            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifySourceChangedButton));

            CoreLogger.LogStatus("Assigning test tree to PresentationSource.");
            _cSource.RootVisual = fpOne;

            CoreLogger.LogStatus("Removing button from test tree.");
            fpThree.Children.Remove(b);

            MainDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);

            Dispatcher.Run();
            
            _cSource.Dispose();

            if (!_handlerFirst)
            {
                throw new Microsoft.Test.TestValidationException("Button's SourceChangedEventHandler was not called twice.");
            }
            CoreLogger.EndVariation();
        }

        private void NotifySourceChangedButton(object o, SourceChangedEventArgs args)
        {
            CoreLogger.LogStatus("Test button's SourceChangedEventHandler called.");

            if (_count == 0)
            {
                if (args.OldSource != null || args.NewSource != _cSource)
                    throw new Microsoft.Test.TestValidationException("Old source is null or new source is not the expected PresentationSource");
            }

            if (_count == 1)
            {
                 _handlerFirst = true;
            }

            if (_count > 2)
                throw new Microsoft.Test.TestValidationException("NotifySourceChanged should only be called twice, it was called " + _count + " times.");

            _count++;
        }



        /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChangeMulipleAddSourceOnSame", Area = "AppModel")]
         public void MultipleAddSourceCallontheSame()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500,500,0,0);


            Button b = new Button();
            

            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandlerCC));
            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandlerBB));
            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandlerAA));

                Source.RootVisual = b;
        
            MainDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
          
            Dispatcher.Run();


            if (!_handlerFirst || !_handlerSecond || !_handlerThird || _count != 3)
                throw new Microsoft.Test.TestValidationException("The event was not fired");

            CoreLogger.EndVariation();
        }


         void NotifyHandlerCC(object o, SourceChangedEventArgs args)
         {
             if (_count < 3)
             {
                 if (!(o is Button))
                     throw new Microsoft.Test.TestValidationException("Expecting a Button");

                 if (args.NewSource != Source || args.OldSource != null)
                     throw new Microsoft.Test.TestValidationException("Button SourceChangeEventArgs are incorrect");

                 _handlerFirst = true;
                 _count++;
             }
         }

        void NotifyHandlerBB(object o, SourceChangedEventArgs args)
         {
             if (_count < 3)
             {
                 if (!(o is Button))
                     throw new Microsoft.Test.TestValidationException("Expecting a Button");

                 if (args.NewSource != Source || args.OldSource != null)
                     throw new Microsoft.Test.TestValidationException("Button SourceChangeEventArgs are incorrect");

                 _handlerSecond = true;
                 _count++;
             }
         }
        

        void NotifyHandlerAA(object o, SourceChangedEventArgs args)
         {
             if (_count < 3)
             {
                 if (!(o is Button))
                     throw new Microsoft.Test.TestValidationException("Expecting a Button");

                 if (args.NewSource != Source || args.OldSource != null)
                     throw new Microsoft.Test.TestValidationException("Button SourceChangeEventArgs are incorrect");

                 _handlerThird = true;
                 _count++;
             }
         }




         /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [Test(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "TestNotifyRootChangeEventonBuiltTree", Area = "AppModel")]
         public void AddSourceChangedEventonbuiltTree()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500,500,0,0);

            Button b = new Button();
            
            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandlerATwo));

            StackPanel rootStackPanel = null;

            StackPanel stackPanel = new StackPanel();

            rootStackPanel = stackPanel;
           
            for (int i=0;i<10;i++)
            {
                StackPanel s = new StackPanel();
                stackPanel.Children.Add(s);
                stackPanel = s;
            }


             Source.RootVisual = rootStackPanel;

             stackPanel.Children.Add(b);


            if (!_handlerFirst)
                throw new Microsoft.Test.TestValidationException("THe event was not fired");
            CoreLogger.EndVariation();
        }

         void NotifyHandlerATwo(object o, SourceChangedEventArgs args)
         {
            if (!(o is Button))
            {
                throw new Microsoft.Test.TestValidationException("Expecting a Button");
            }
            if (false == _handlerFirst)
            {
                if (args.NewSource != Source || args.OldSource != null)
                {
                    throw new Microsoft.Test.TestValidationException("The sources are not the expected one");
                }
            }
            else
            {
                if (args.NewSource != null || args.OldSource != Source)
                {
                    throw new Microsoft.Test.TestValidationException("The sources are not the expected one");
                }
            }

            if (args.Source != o || args.OriginalSource != o)
                throw new Microsoft.Test.TestValidationException("");
            _handlerFirst=true;
         }
  }
}





