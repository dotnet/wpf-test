// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;

using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;


namespace Avalon.Test.Framework.Dispatchers.Registration
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>ScheduleAbortSimple.cs</filename>
    ///</remarks>
    [TestDefaults]
    public class Win32DispatcherMultipleContext : TestCase
    {
        
        /// <summary>
        /// This is not used.
        /// </summary>
        public override void Run()
        {
            string Parameter = TestCaseInfo.GetCurrentInfo().Params;

             if (Parameter == "MultipleContextRegistration")
                MultipleContextRegistration();
             if (Parameter == "PassingNull")
                PassingNull();
                                       
        }

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public Win32DispatcherMultipleContext() :base(TestCaseType.None)
        {

        }
        
        /// <summary>
        /// Passing Null to RegisterContext and UnRegisterContext and validating the ArgumentNullException
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///  <li></li>
        ///  </ol>
        ///     <filename>Win32DispatcherMultipleContext.cs</filename>
        /// </remarks>
        //[CoreTestsLoader (CoreTestsTestType.MethodBase)]
        //[TestCasePriority ("1")]
        //[TestCaseArea (@"Dispatcher\Registration\Simple")]
        //[TestCaseMethod ("RunTest")]
        //[TestCaseParams ("PassingNull")]
        [TestAttribute(1, @"Dispatcher\Registration", "PassingNull", TestParameters="PassingNull", Area="ElementServices")]
        public void PassingNull ()
        {
                    using ()
                    {
                        _uiDispatcher = new Win32Dispatcher ();
                    }

                    Exception Exp = null;

                    try
                    {
                        _uiDispatcher.RegisterContext(null);
                    }
                    catch(ArgumentNullException e)
                    {
                        Exp = e;
                    }
                    catch(Exception){}
                    

                    if (Exp == null)
                        throw new Microsoft.Test.TestValidationException("Expecting an ArgumentNullException from RegisterContext");


                    try
                    {
                        _uiDispatcher.UnregisterContext(null);
                    }
                    catch(ArgumentNullException e)
                    {
                        Exp = e;
                    }
                    catch(Exception){}
                    

                    if (Exp == null)
                        throw new Microsoft.Test.TestValidationException("Expecting an ArgumentNullException from UnRegisterContext");


                    try
                    {
                        _uiDispatcher.IsDispatcherThread(null,new Dispatcher());
                    }
                    catch(ArgumentNullException e)
                    {
                        Exp = e;
                    }
                    catch(Exception){}
                    

                    if (Exp == null)
                        throw new Microsoft.Test.TestValidationException("Expecting an ArgumentNullException from IsDispatcherThread (thread parameter null)");



                    try
                    {
                        _uiDispatcher.IsDispatcherThread(Thread.CurrentThread,null);
                    }
                    catch(ArgumentNullException e)
                    {
                        Exp = e;
                    }
                    catch(Exception){}
                    

                    if (Exp == null)
                        throw new Microsoft.Test.TestValidationException("Expecting an ArgumentNullException from IsDispatcherThread (Context parameter null)");

                    
        }



        /// <summary>
        /// Register 10 Dispatcher on Win32Dispatcher and Unregister the 10 of them. Validating the Property Dispatcher on the Context
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///  <li></li>
        ///  </ol>
        ///     <filename>Win32DispatcherMultipleContext.cs</filename>
        /// </remarks>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"Dispatcher\Registration\Simple")]
        [TestCaseMethod("RunTest")]
        [TestCaseParams("MultipleContextRegistration")]
        public void MultipleContextRegistration()
        {
     

            using ()
            {
                _uiDispatcher = new Win32Dispatcher();
            }

            UIDispatcher[] arrayDispatcher = {_uiDispatcher};

            using ()
            {
            
                RegisterContextOnDispatcher(arrayDispatcher, 10);
            }

            using ()
            {
            
                ValidateDispatchers(_uiDispatcher,true);
            }


            using ()
            {
            
                UnRegisterContextOnDispatcher(_uiDispatcher);
            }


            using ()
            {
            
                ValidateDispatchers(_uiDispatcher,false);
            }

        }


        void RegisterContextOnDispatcher(UIDispatcher[] Dispatchers, int NoContext)
        {

            using ()
            {
                for (int i=1; i<NoContext; i++)
                    ContextList.Add(new Dispatcher());
            }        

            using ()
            {
                for (int j=0; j < Dispatchers.Length; j++)
                {
                    for (int i=0; i<ContextList.Count; i++)          
                        Dispatchers[j].RegisterContext((Dispatcher)ContextList[i]);
                }

            }
        }

        void UnRegisterContextOnDispatcher(UIDispatcher Dispatcher)
        {
            using ()
            {
                for (int i=0; i<ContextList.Count; i++)          
                    Dispatcher.UnregisterContext((Dispatcher)ContextList[i]);
            }

        }


        void ValidateDispatchers(UIDispatcher Dispatcher, bool ValidateTrue)
        {
            for (int i=0; i<ContextList.Count; i++)
            {
                Dispatcher tempContext = ContextList[i] as Dispatcher;

                if (tempContext.Dispatcher != Dispatcher && ValidateTrue)
                    throw new Microsoft.Test.TestValidationException("The dispatcher doesn't match");
                else  if (tempContext.Dispatcher == Dispatcher && !ValidateTrue)
                        throw new Microsoft.Test.TestValidationException("The dispatcher doesn't match");
                
            }
        }
            
        private UIDispatcher _uiDispatcher = null;

    }
        
 }




