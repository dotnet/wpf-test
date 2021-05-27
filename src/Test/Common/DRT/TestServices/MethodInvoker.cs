// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <description>

//  This class will invoke a method.

//  It may be chained with other invokers in which case it behaves

//  as a linked list of decorators walking forward down the chain

//  for PreInvoke and Invoke and backwards for PostInvoke.

//  Specializing and chaining this class allows you to decorate the

//  call of a method; for example trace parameters or proxy targets

// </description>



using System;
using System.Reflection;
using System.Security;

namespace DRT
{
    /// <summary>
    /// This class will invoke a method.
    /// 
    /// It may be chained with other invokers in which case it behaves
    /// as a linked list of decorators walking forward down the chain
    /// for PreInvoke and Invoke and backwards for PostInvoke.
    /// 
    /// Specializing and chaining this class allows you to decorate the
    /// call of a method; for example trace parameters or proxy targets
    /// </summary>
    public class MethodInvoker
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Will construct the default method invoker.
        /// </summary>
        public MethodInvoker()
        {
            TestServices.Current.MessageSent += MessageSentHandler;
        }
        #endregion Constructors

        #region Public Methods
        //----------------------------------------------------------------------
        //   Public Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Will chain the invokers in the order provided.
        /// </summary>
        /// <param name="invokers">A list of invokers in order.</param>
        /// <returns>First invoker after being chained.</returns>
        public static MethodInvoker Chain(params MethodInvoker[] invokers)
        {
            for (int i = 0; i < invokers.Length - 1; i++)
            {
                invokers[i]._nextInvoker = invokers[i + 1];
            }

            return invokers[0];
        }

        /// <summary>
        /// Will invoke the call specified as describe in the class description.
        /// </summary>
        /// <param name="currentCall"></param>
        public void Invoke(Call currentCall)
        {
            ChainPreInvoke(currentCall);
            ChainInvoke(currentCall);
            ChainPostInvoke(currentCall);
        }
        #endregion Public Methods

        #region Public Properties
        //----------------------------------------------------------------------
        // Public Properties
        //----------------------------------------------------------------------

        /// <summary>
        /// Determines if the execution can continue with this invoker.
        /// </summary>
        public bool HadCriticalFailure
        {
            get { return _terminate; }
        }
        #endregion Public Properties

        #region Public Delegates
        //----------------------------------------------------------------------
        // Public Delegates
        //----------------------------------------------------------------------

        /// <summary>
        /// Delegate used to construct the MethodInvoker when needed.
        /// </summary>
        /// <returns>An instance of a MethodInvoker.</returns>
        public delegate MethodInvoker MethodInvokerFactoryDelegate();
        #endregion Public Delegates

        #region Protected Virtual Methods (Hooks for Derived Classes)
        //----------------------------------------------------------------------
        // Protected Virtual Methods (Hooks for Derived Classes)
        //----------------------------------------------------------------------

        /// <summary>
        /// This will be called before the Call is invoked begins.
        /// 
        /// No Implementation; no reason for derived classes to callback.
        /// </summary>
        /// <param name="currentCall">Call to be invoked.</param>
        protected virtual void InternalPreInvoke(Call currentCall)
        {
            return;
        }

        /// <summary>
        /// This will be called as part of the Call invokation.
        /// 
        /// No Implementation; no reason for derived classes to callback.
        /// </summary>
        /// <param name="currentCall">Call to be invoked.</param>
        protected virtual void InternalInvoke(Call currentCall)
        {
            return;
        }

        /// <summary>
        /// This will be called after the invokation occurs only
        /// if the invokation did not throw.
        /// 
        /// No Implementation; no reason for derived classes to callback.
        /// </summary>
        /// <param name="currentCall">Call to be invoked.</param>
        protected virtual void InternalPostInvoke(Call currentCall)
        {
            return;
        }
        #endregion Protected Virtual Methods (Hooks for Derived Classes)

        #region Private Methods
        //----------------------------------------------------------------------
        // Private Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Handler for message events from TestServices.
        /// We will abort the invoker when an assert is thrown.
        /// </summary>
        /// <param name="sender">Sender of the message.</param>
        /// <param name="args">Message arguments.</param>
        private void MessageSentHandler(object sender, MessageEventArgs args)
        {
            if (args.Category == MessageEventArgs.MessageCategory.Assert)
            {
                _terminate = true;
            }
        }

        /// <summary>
        /// Chains the calls
        /// </summary>
        private void ChainPreInvoke(Call currentCall)
        {
            TestServices.InternalTrace("PreInvoke for {0} using {1}.", 
                currentCall.Method.Name, this.GetType().Name);

            this.InternalPreInvoke(currentCall);

            if (_nextInvoker != null)
            {
                _nextInvoker.ChainPreInvoke(currentCall);
            }
        }

        /// <summary>
        /// Chains the calls
        /// </summary>
        private void ChainInvoke(Call currentCall)
        {
            TestServices.InternalTrace("Invoke for {0} using {1}.",
                currentCall.Method.Name, this.GetType().Name);

            this.InternalInvoke(currentCall);

            if (_nextInvoker != null)
            {
                _nextInvoker.ChainInvoke(currentCall);
            }
            else
            {
                // last in the chain or there is no chain do the real invoke.
                //try
                {
                    currentCall.Method.Invoke(
                        currentCall.Target, currentCall.Parameters);
                }
                //catch (TargetInvocationException e)
                //{
                //    Exception inner = e.InnerException;

                //    TestServices.Log(
                //        "{0} threw the following exception.\n{1}",
                //        currentCall.Method.Name,
                //        inner.ToString());

                //    DiagnosticTips.GenerateTip(inner);

                //    throw;
                //}
                return;
            }
        }
        /// <summary>
        /// Chains the calls
        /// </summary>
        private void ChainPostInvoke(Call currentCall)
        {
            // like an unwinding stack, going in reverse so we call the next
            // caller first
            if (_nextInvoker != null)
            {
                _nextInvoker.ChainPostInvoke(currentCall);
            }

            TestServices.InternalTrace("PostInvoke for {0} using {1}.", 
                currentCall.Method.Name, this.GetType().Name);
            
            this.InternalPostInvoke(currentCall);
        }
        #endregion Private Methods

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        /// <summary>
        /// The next invoker in the chain; null means we are the last.
        /// </summary>
        private MethodInvoker _nextInvoker;

        /// <summary>
        /// On true an error has occurred which requires the instance of this
        /// invoker to terminate.
        /// </summary>
        private bool _terminate;
        #endregion Private Fields
    }
}
