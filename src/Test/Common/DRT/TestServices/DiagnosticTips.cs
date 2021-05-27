// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <description>

//  Provides tips on specific exceptions that can be thrown.

// </description>



using System;
using System.Security;
using System.Reflection;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Diagnostics;

namespace DRT
{
    /// <summary>
    /// Provides a diagnostic tips based on the exeception provided.
    /// </summary>
    internal static class DiagnosticTips
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Loads the dictionary with all known diagnostic tip handlers and the
        /// execeptions they can handle.
        /// </summary>
        /// <remarks>
        /// New TipHandlers must be added here; as with 'switch' the
        /// first type that satisfies the expression is used, thus order is
        /// relavent.
        /// Order should be with the last type in the inheritance tree, thus
        /// if registered Exception should be last.
        /// </remarks>
        static DiagnosticTips()
        {
            _tipHandlers = new Dictionary<Type, TipHandlerDelegate>();

            _tipHandlers.Add(
                typeof(TargetInvocationException),
                TargetInvocationExceptionTip);
            _tipHandlers.Add(
                typeof(RightsManagementException),
                RightsManagementExceptionTip);
        }
        #endregion Constructors

        #region Internal Methods
        //----------------------------------------------------------------------
        // Internal Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Will log a diagnostic tip for the exception provided if one exsits.
        /// </summary>
        /// <param name="exception"></param>
        internal static void GenerateTip(Exception exception)
        {
            string tip = GenerateTipCore(exception);

            if (!string.IsNullOrEmpty(tip))
            {
                TestServices.Log(
                    "DiagnosticTip: {0}", tip);
            }
        }
        #endregion Internal Methods

        #region Private Methods - Tip Handlers
        //----------------------------------------------------------------------
        // Private Methods - Tip Handlers
        //----------------------------------------------------------------------
        // NOTE: New TipHandlers should be defined here.

        /// <summary>
        /// Handles Rights MAnagement Exception tip.
        /// </summary>
        /// <param name="exception">An exception.</param>
        /// <returns>A tip message.</returns>
        private static string RightsManagementExceptionTip(Exception exception)
        {
            return "Is RM SDK is installed? Is msdrm.dll in application folder?";
        }

        /// <summary>
        /// TargetInvocationException is not interesting and may contain
        /// an interesting exception.
        /// </summary>
        /// <param name="exception">An exception.</param>
        /// <returns>A tip message.</returns>
        private static string TargetInvocationExceptionTip(Exception exception)
        {
            TargetInvocationException e = (TargetInvocationException)exception;
            return GenerateTipCore(e.InnerException);
        }
        #endregion Private Methods - Tip Handlers

        #region Private Methods
        //----------------------------------------------------------------------
        //  Private Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Will call the first tip delegate whose that registered a type that
        /// the exception can be cast to.
        /// 
        /// Example, if a delegate was the first to register and it registered
        /// with the type Exception, it would be the only delegate ever called.
        /// </summary>
        /// <param name="exception">An exception.</param>
        /// <returns>A tip message.</returns>
        private static string GenerateTipCore(Exception exception)
        {
            string result = string.Empty;

            Type e = exception.GetType();
            foreach (Type k in _tipHandlers.Keys)
            {
                if (k.IsAssignableFrom(e))
                {
                    result = _tipHandlers[k](exception);
                    break;
                }
            }

            return result;
        }
        #endregion Private Methods

        #region Private Delegates
        //----------------------------------------------------------------------
        //  Private Delegates
        //----------------------------------------------------------------------
        /// <summary>
        /// Delegate will be provided an exception for which to generate a
        /// tip message.
        /// </summary>
        /// <remarks>
        /// Implementor may call GenerateTipCore if there is an inner exception
        /// it would like to generate a tip for.
        /// </remarks>
        /// <param name="exception">An exception.</param>
        /// <returns>A tip message.</returns>
        private delegate string TipHandlerDelegate(Exception exception);
        #endregion Private Delegates

        #region Private Fields
        //----------------------------------------------------------------------
        //  Private Fields
        //----------------------------------------------------------------------
        
        /// <summary>
        /// List of tip handlers and the type they wish to handle.
        /// </summary>
        /// <remarks>
        /// Order should be with the last type in the inheritance tree, thus
        /// if registered Exception should be last.
        /// </remarks>
        private static Dictionary<Type, TipHandlerDelegate> _tipHandlers;
        #endregion Private Fields

    }
}
