// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    ///     
    ///</summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class TestFromVisual : TestCase
    {

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public TestFromVisual() : base(TestCaseType.None) { }

        /// <summary>
        ///    Calling FromVisual very simple scenario. I pass the root as parameter and validates correct Source 
        ///    
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestFromVisual.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource\FromVisual", TestCaseSecurityLevel.FullTrust, "TestFromVisual", Area = "AppModel")]
        override public void Run()
        {
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500, 500, 0, 0);

            Button b = new Button();

            Source.RootVisual = b;

            PresentationSource sourceGet = PresentationSource.FromVisual(b);

            if (sourceGet != Source)
                throw new Microsoft.Test.TestValidationException("The source is not the expecting");
        }


        /// <summary>
        ///     
        ///    
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestFromVisual.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource\FromVisaul", TestCaseSecurityLevel.FullTrust, "TestFromVisualNotOnTree", Area = "AppModel")]
        public void NotOnTheTree()
        {
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500, 500, 0, 0);


            Button b = new Button();
            StackPanel stackPanel = new StackPanel();


            Source.RootVisual = stackPanel;

            PresentationSource sourceGet = PresentationSource.FromVisual(b);


            if (sourceGet != null)
                throw new Microsoft.Test.TestValidationException("Expecting Null from calling FromVisual");


        }


        /// <summary>
        ///     
        ///    
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestFromVisual.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource\FromVisaul", TestCaseSecurityLevel.FullTrust, "TestFromVisualOnDeepTree", Area = "AppModel")]
        public void LookingOnDeepTree()
        {
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500, 500, 0, 0);


            Button b = new Button();
            StackPanel stackPanel = new StackPanel();
            StackPanel root = stackPanel;
            for (int i = 0; i < 100; i++)
            {

                StackPanel fpAux = new StackPanel();

                stackPanel.Children.Add(fpAux);

                stackPanel = fpAux;

            }

            stackPanel.Children.Add(b);

            PresentationSource sourceGet = PresentationSource.FromVisual(b);


            if (sourceGet != null)
                throw new Microsoft.Test.TestValidationException("Expecting Null from calling FromVisual");

            Source.RootVisual = root;

            sourceGet = PresentationSource.FromVisual(b);


            if (sourceGet != Source)
                throw new Microsoft.Test.TestValidationException("Expecting Source.");


        }



        /// <summary>
        ///     
        ///    
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestFromVisual.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource\FromVisaul", TestCaseSecurityLevel.FullTrust, "TestFromVisualInvalidParam", Area = "AppModel")]
        public void InvalidParam()
        {
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500, 500, 0, 0);


            try
            {
                PresentationSource sourceGet = PresentationSource.FromVisual(null);
            }
            catch (ArgumentNullException)
            {
                _handlerFirst = true;
            }

            if (!_handlerFirst)
                throw new Microsoft.Test.TestValidationException("Expecting an exception");


        }

        bool _handlerFirst = false;



        HwndSource _sourceTwo;


        /// <summary>
        ///     
        ///    
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestFromVisual.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource\FromVisaul", TestCaseSecurityLevel.FullTrust, "TestFromVisualMovingRoot", Area = "AppModel")]
        public void MovingRootFromS1toS2()
        {
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource(500, 500, 0, 0);

            _sourceTwo = SourceHelper.CreateHwndSource(500, 500, 0, 0);

            Button b = new Button();
            StackPanel stackPanel = new StackPanel();
            StackPanel rootOne = stackPanel;
            for (int i = 0; i < 100; i++)
            {
                StackPanel fpAux = new StackPanel();

                stackPanel.Children.Add(fpAux);
                stackPanel = fpAux;
            }

            StackPanel lastOne = stackPanel;
            Source.RootVisual = rootOne;

            stackPanel = new StackPanel();
            StackPanel rootTwo = stackPanel;

            for (int i = 0; i < 100; i++)
            {
                StackPanel fpAux = new StackPanel();

                stackPanel.Children.Add(fpAux);
                stackPanel = fpAux;
            }

            _sourceTwo.RootVisual = rootTwo;

            lastOne.Children.Add(b);
            lastOne.Children.Remove(b);
            stackPanel.Children.Add(b);

            PresentationSource sourceGet = PresentationSource.FromVisual(b);

            if (sourceGet != _sourceTwo)
                throw new Microsoft.Test.TestValidationException("Expecting the Second Source");
        }
    }
}






