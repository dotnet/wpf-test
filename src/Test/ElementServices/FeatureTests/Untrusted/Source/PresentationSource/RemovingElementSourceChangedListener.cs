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
    public class RemovingElementSourceChangedListener : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public RemovingElementSourceChangedListener() :base(TestCaseType.None){}
        
        /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "RemoveElementSourceChangedListener", Area = "AppModel")]
        override public void Run()
        {
            CoreLogger.BeginVariation();

            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource( 500, 500, 0, 0);


            Button b = new Button();
            

            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (_exceptionMethod));

            PresentationSource.RemoveSourceChangedHandler(b, new SourceChangedEventHandler(_exceptionMethod));

            Source.RootVisual = b;

            CoreLogger.EndVariation();
        }


        bool _handlerFirst = false;

        //bool _handlerSecond = false.

        //int _count = 0.







        void _exceptionMethod(object o, SourceChangedEventArgs args)
        {
            throw new Microsoft.Test.TestValidationException("This handler should not be called");
        }


        /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "RemoveElementSourceChangedListenerAddRemoveAdd", Area = "AppModel")]
        public void AddRemoveAddListener()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource( 500, 500, 0, 0);


            Button b = new Button();
            

            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (_exceptionMethod));

            PresentationSource.RemoveSourceChangedHandler(b, new SourceChangedEventHandler(_exceptionMethod));

            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandlerTestOne));


            Source.RootVisual = b;
          



            if (!_handlerFirst)
                throw new Microsoft.Test.TestValidationException("The event was not called");
            CoreLogger.EndVariation();
        }



        void NotifyHandlerTestOne(object o, SourceChangedEventArgs args)
        {

            _handlerFirst = true;
        }


        /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "RemoveElementSourceChangedListenerAddRemove2x", Area = "AppModel")]
        public void AddRemoveTwiceAddListener()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource( 500, 500, 0, 0);


            Button b = new Button();
            

            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (_exceptionMethod));

            PresentationSource.RemoveSourceChangedHandler(b, new SourceChangedEventHandler(_exceptionMethod));

            PresentationSource.RemoveSourceChangedHandler(b, new SourceChangedEventHandler(_exceptionMethod));

            PresentationSource.RemoveSourceChangedHandler(b, new SourceChangedEventHandler(_exceptionMethod));

            PresentationSource.AddSourceChangedHandler(b, new SourceChangedEventHandler (NotifyHandlerTestOne));


            Source.RootVisual = b;
          


            if (!_handlerFirst)
                throw new Microsoft.Test.TestValidationException("The event was not called");
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
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "RemoveElementSourceChangedListenerRemove", Area = "AppModel")]
        public void RemoveWithoutAdding()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

             Source = SourceHelper.CreateHwndSource( 500, 500, 0, 0);


            Button b = new Button();
          

          
            PresentationSource.RemoveSourceChangedHandler(b, new SourceChangedEventHandler(_exceptionMethod));

           
            Source.RootVisual = b;

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
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "RemoveElementSourceChangedListenerInvalidParams", Area = "AppModel")]
         public void InvalidParameters()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;
            
            bool exceptionCalled = false;
           try
           {
                PresentationSource.RemoveSourceChangedHandler(null, new SourceChangedEventHandler (NotifyHandlerTestOne));
           }
           catch(ArgumentNullException)
           {
                exceptionCalled = true;
           }
          
          
            if (!exceptionCalled)
                throw new Microsoft.Test.TestValidationException("Expecting Argument Null Exception first Parameter");


            Button b = new Button();

            PresentationSource.RemoveSourceChangedHandler(b, null);

            // Before we were throwing a InvalidOperationException
            // if a Handler was not added. Now we do a NOP, this looks similar to += on events
            PresentationSource.RemoveSourceChangedHandler(b,new SourceChangedEventHandler (NotifyHandlerTestOne));

            CoreLogger.EndVariation();
        }


            Button _button ;


        /// <summary>
        ///     Creating a HwndSource and a Button. Calling AddSourceChangedHandler on the Button, Set the HwndSource.RootVisual to the button. Expecting an event.
        ///     Later Setting the HwndSource.RootVisual to null and expecting the Event again.  Validating Arguments
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestNotifyRootChange.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "RemoveElementSourceChangedListenerRemoveDiffThread", Area = "AppModel")]
        public void RemoveFromDiffThreadAndAdd()
        {

            CoreLogger.BeginVariation();
            Thread t1 = new Thread(new ThreadStart(_secondThread));

            MainDispatcher = Dispatcher.CurrentDispatcher;

            _button = new Button();

            Source = SourceHelper.CreateHwndSource( 500, 500, 0, 0);

            PresentationSource.AddSourceChangedHandler(_button, new SourceChangedEventHandler(_exceptionMethod));




            t1.Start();

            Dispatcher.Run();          

			if (!_handlerFirst || !_handlerSecond || !_handlerThird )
                throw new Microsoft.Test.TestValidationException("The event was not called");

            FinalReportFailure();
            CoreLogger.EndVariation();
        }


        bool _handlerSecond = false;
		bool _handlerThird = false;

		

        

        void _secondThread()
        {

            try
            {
                MainDispatcher.Invoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object o)
                    {
                    
                        PresentationSource.RemoveSourceChangedHandler(_button, new SourceChangedEventHandler(_exceptionMethod));            

                        PresentationSource.AddSourceChangedHandler(_button, new SourceChangedEventHandler(NotifyHandlerTestA));            

                        PresentationSource.AddSourceChangedHandler(_button, new SourceChangedEventHandler(NotifyHandlerTestB));

                        Source.RootVisual = _button;

                        MainDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
                        return null;

                    }, null);
            }
            catch(Exception e)
            {
                ExceptionList.Add(e);
                MainDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
            }
                

        }

		int _count = 0;

        void NotifyHandlerTestA(object o, SourceChangedEventArgs args)
        {
			if (_count == 0)
			{
				if (args.NewSource != Source || args.OldSource != null)
                    throw new Microsoft.Test.TestValidationException("Notify one; Source");

				if (o != _button)
                    throw new Microsoft.Test.TestValidationException("Notify one; Object argument");

				_handlerFirst = true;
			}
			if (_count == 2)
			{
				if (args.NewSource != null || args.OldSource != Source)
                    throw new Microsoft.Test.TestValidationException("Notify one; Source");

				if (o != _button)
                    throw new Microsoft.Test.TestValidationException("Notify one; Object argument");
				
				_handlerThird = true;

			}
			_count++;
        }

        void NotifyHandlerTestB(object o, SourceChangedEventArgs args)
        {


			if (_count == 1)
			{
				if (args.NewSource != Source || args.OldSource != null)
                    throw new Microsoft.Test.TestValidationException("Notify two; Source");

				if (o != _button)
                    throw new Microsoft.Test.TestValidationException("Notify two; Object argument");

				_handlerSecond = true;
			}
			

			_count++;
        }
    }
}


