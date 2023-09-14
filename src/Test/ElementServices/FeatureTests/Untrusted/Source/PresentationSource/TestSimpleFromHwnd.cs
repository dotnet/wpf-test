// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;

using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    ///     
    ///</summary>
    [TestDefaults]
    public class TestSimpleFromHwnd : TestCase
    {
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public TestSimpleFromHwnd() :base(TestCaseType.None){}
        
        /// <summary>
        ///     Creating a HwndSource and calling FromHwnd. Basic Functionaly Test
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestSimpleFromHwnd.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "HwndSourceFromHwnd", Area = "AppModel")]
        override public void Run()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource( 500, 500, 0,0);

            PresentationSource isource = HwndSource.FromHwnd(Source.Handle);

                       
            if (isource != Source)
                ExceptionList.Add(new Microsoft.Test.TestValidationException("The expected source doesn't match"));
            
            Source.Dispose();

            FinalReportFailure();
            CoreLogger.EndVariation();
        }



        /// <summary>
        ///     Creating a HwndSource and calling Dispose(), after that call FromHwnd, this should return Null, basic Functionality
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestSimpleFromHwnd.cs</location>
        /// </remarks>
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "HwndSourceFromHwndDisposed", Area = "AppModel")]
         public void HwndDisposed()
        {
            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            Source = SourceHelper.CreateHwndSource( 500, 500, 0,0);
            
            IntPtr handler = Source.Handle;

            Source.Dispose();

            PresentationSource isource = HwndSource.FromHwnd(handler);
            
            if (isource != null)
                ExceptionList.Add(new Microsoft.Test.TestValidationException("The expected source doesn't match"));

            FinalReportFailure();
            CoreLogger.EndVariation();
        }




        /// <summary>
        ///     Creating 10 HwndSources and calling FromHwnd on all of them, should return the correct Ones
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestSimpleFromHwnd.cs</location>
        /// </remarks>
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "MultipleHwndSourceFromHwnd", Area = "AppModel")]
        public void MultipleHwndSimple()
        {

            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            HwndSource[] sourceArray = new HwndSource[10];
            HwndSource[] sourceArrayTwo = new HwndSource[10];


            for (int i=0;i<10;i++)
            {
                sourceArray[i] = SourceHelper.CreateHwndSource( 500, 500, 0,0);
                sourceArrayTwo[9-i] = sourceArray[i];
            }
        
            for (int j=0;j<10;j++)
            {

                PresentationSource isource = HwndSource.FromHwnd(sourceArray[j].Handle);
                if (isource != sourceArrayTwo[9-j])
                    ExceptionList.Add(new Microsoft.Test.TestValidationException("The Source is not the expected one; Source #" + j.ToString()));
            }

            FinalReportFailure();
            CoreLogger.EndVariation();
        }




        /// <summary>
        ///     Creating a HwndSource with IntPtr.Zero as paramenter. Expecting an exception
        ///</summary>
        /// <remarks>
        ///     <Owner>Microsoft</Owner>
 
        ///     <location>TestSimpleFromHwnd.cs</location>
        /// </remarks>        
        [TestAttribute(0, @"Source\PresentationSource", TestCaseSecurityLevel.FullTrust, "HwndSourceFromHwndInvalidParams", Area = "AppModel")]
        public void InvalidParameter()
        {

            CoreLogger.BeginVariation();
            MainDispatcher = Dispatcher.CurrentDispatcher;

            try
            {
                HwndSource.FromHwnd(IntPtr.Zero);
            }
            catch(ArgumentException)
            {
                TestCaseFailed = true;
            }

            

            if (!IsTestCaseFail)
                ExceptionList.Add(new Microsoft.Test.TestValidationException("Expecting ArgumentException"));


            FinalReportFailure();
            CoreLogger.EndVariation();
        } 

    }


}



