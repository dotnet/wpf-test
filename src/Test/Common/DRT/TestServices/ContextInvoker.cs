// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  Used in an invoker chain to provide and capture parameters for calls.

//  Mapping of key values is done simply by name and type, however as the name

//  is the key value, duplicate parameter names are not allowed. </summary>



using System;
using System.Reflection;
using System.Globalization;

namespace DRT
{
    /// <summary>
    /// Used in an invoker chain to provide and capture parameters for calls.
    /// Mapping of key values is done simply by name and type, however as the 
    /// name is the key value, duplicate parameter names are not allowed.
    /// </summary>
    public sealed class ContextInvoker : MethodInvoker
    {
        #region Constructors
        //----------------------------------------------------------------------
        //
        // Constructors
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// Will create an empty TestContext.
        /// </summary>
        public ContextInvoker() 
        {
            _context = new TestContext();
        }

        /// <summary>
        /// Will use the provided TestContext when invoking methods.
        /// </summary>
        /// <param name="context">A TestContext</param>
        public ContextInvoker(TestContext context)
        {
            _context = context;
        }
        #endregion

        #region Protected Methods - Overrides MethodInvoker
        //----------------------------------------------------------------------
        //
        // Protected Methods - Overrides MethodInvoker
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// This method is where the in parameters are fetched from the 
        /// TestContext and applied to the Call.
        /// </summary>
        /// <remarks>
        /// What should we do if there are specified by somone else in the 
        /// chain or caller?
        /// </remarks>
        /// <param name="currentCall">The Call to be invoked.</param>
        protected override void InternalPreInvoke(Call currentCall)
        {
            GetParametersFromContext(
                currentCall.Method, 
                _context, 
                currentCall.Parameters);
        }

        /// <summary>
        /// This method is where the out parameters are stored in to the 
        /// TestContext.
        /// </summary>
        /// <param name="currentCall">The Call that finished.</param>
        protected override void InternalPostInvoke(Call currentCall)
        {
            SetContextValues(
                currentCall.Method, 
                _context, 
                currentCall.Parameters);
        }
        #endregion

        #region Private Methods
        //----------------------------------------------------------------------
        //
        // Private Methods
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// Performs the match on the methods ParameterInfo and TestContext 
        /// and does substituion into the the argument array.
        /// </summary>
        /// <param name="methodInfo">Method that will be called.</param>
        /// <param name="context">TestContext to use as the source for data.
        /// </param>
        /// <param name="args">The Call's parameters.</param>
        private static void GetParametersFromContext(
            MethodInfo methodInfo, TestContext context, object[] args)
        {
            ParameterInfo[] pis = methodInfo.GetParameters();

            if ((args == null) || (pis.Length != args.Length))
            {
                throw new ArgumentException(
                    "Number of arguments provided does not match method.");
            }

            for (int i = 0; i < pis.Length; i++)
            {
                if (pis[i].ParameterType == typeof(TestContext))
                {
                    args[i] = context;
                }
                else
                {
                    object value = null;
                    if (!context.GetValue(pis[i].Name, out value))
                    {
                        throw new ArgumentOutOfRangeException(
                            pis[i].Name, 
                            "Could not find value.");
                    }
                    args[i] = GetTypedValue(pis[i], value);
                }
            }
        }

        /// <summary>
        /// Performs the match on the methods ParameterInfo and TestContext 
        /// and does substituion into the the argument array.
        /// </summary>
        /// <param name="methodInfo">Method that was called.</param>
        /// <param name="context">TestContext to use as the store for data.
        /// </param>
        /// <param name="args">The Call's parameters.</param>
        private static void SetContextValues(
            MethodInfo methodInfo, TestContext context, object[] args)
        {
            ParameterInfo[] pis = methodInfo.GetParameters();
            for (int i = 0; i < pis.Length; i++)
            {
                if (IsOutParameter(pis[i]))
                {
                    context.SetValue(pis[i].Name, args[i]);
                }
            }
        }

        /// <summary>
        /// Determines if the parameter is an out parameter.
        /// </summary>
        /// <param name="parameter">The parameter to inspect.</param>
        /// <returns>True if it is an out parameter.</returns>
        private static bool IsOutParameter(ParameterInfo parameter)
        {
            return (
                parameter.IsOut || 
                parameter.ParameterType.ToString().IndexOf('&') != -1);
        }

        /// <summary>
        /// Gets a typed value matching parameter from the provided value.
        /// </summary>
        /// <param name="pi">Target parameter.</param>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        // CA: Parse can throw up to 4 types of exceptions and the handling 
        // is always the same.
        private static object GetTypedValue(ParameterInfo pi, object value)
        {
            if (pi.ParameterType == typeof(int))
            {
                try
                {
                    value = int.Parse(
                        value.ToString(), CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    value = (int)0;
                }
            }
            else if (pi.ParameterType == typeof(bool))
            {
                try
                {
                    value = bool.Parse(value.ToString());
                }
                catch (Exception)
                {
                    value = false;
                }
            }
            else if (pi.ParameterType == typeof(string))
            {
                if (value != null)
                {
                    return (string)value;
                }
            }

            return value;
        }
        #endregion

        #region Private Fields
        //----------------------------------------------------------------------
        //
        // Private Fields
        //
        //----------------------------------------------------------------------

        private TestContext _context;
        #endregion
    }
}
