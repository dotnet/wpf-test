// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Test; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Utilities for trying various combinations of valid and invalid values
    /// </summary>
    /// <remarks>
    /// The various parameter functions provided as arguments will typically be
    /// taken from methods on the Values class.
    /// </remarks>
    public static class ValueTests
    {
        /// <summary>
        /// Try a 1-argument action with a variety of valid and invalid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="parameters"></param>
        /// <param name="paramName"></param>
        public static void Try<T>(
            System.Action<T> tryAction,
            System.Func<bool, IEnumerable<T>> parameters,
            string paramName)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(parameters != null);
            Utils.Assert(paramName != null);

            TryInvalid(tryAction, parameters, paramName);
            TryValid(tryAction, parameters);
        }

        /// <summary>
        /// Try a 1-argument action with a variety of invalid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="parameters"></param>
        /// <param name="paramName"></param>
        public static void TryInvalid<T>(
            System.Action<T> tryAction,
            System.Func<bool, IEnumerable<T>> parameters,
            string paramName)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(parameters != null);
            Utils.Assert(paramName != null);

            // Verify invalid values in parameter
            foreach (T invalidParam in parameters(false))
            {
                //



            }
        }

        /// <summary>
        /// Try a 1-argument action with a variety of valid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="parameters"></param>
        public static void TryValid<T>(
            System.Action<T> tryAction,
            System.Func<bool, IEnumerable<T>> parameters)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(parameters != null);

            foreach (T validParam in parameters(true))
            {
                tryAction(validParam);
            }
        }

        /// <summary>
        /// Try a 2-argument action with a variety of valid and invalid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        public static void Try2<T1,T2>(
            System.Action<T1, T2> tryAction,
            System.Func<bool, IEnumerable<T1>> params1,
            System.Func<bool, IEnumerable<T2>> params2,
            string paramName1,
            string paramName2)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(params1 != null);
            Utils.Assert(params2 != null);
            Utils.Assert(paramName1 != null);
            Utils.Assert(paramName2 != null);

            TryInvalid2(
                tryAction,
                params1,
                params2,
                paramName1,
                paramName2);

            TryValid2(
                tryAction,
                params1,
                params2);
        }

        /// <summary>
        /// Try a 2-argument action with a variety of invalid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        public static void TryInvalid2<T1,T2>(
            System.Action<T1, T2> tryAction,
            System.Func<bool, IEnumerable<T1>> params1,
            System.Func<bool, IEnumerable<T2>> params2,
            string paramName1,
            string paramName2)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(params1 != null);
            Utils.Assert(params2 != null);
            Utils.Assert(paramName1 != null);
            Utils.Assert(paramName2 != null);

            // Verify invalid values in parameter 1
            foreach (T2 validParam2 in params2(true))
            {
                TryInvalid(
                    (x) => tryAction(x, validParam2),
                    params1,
                    paramName1);
            }

            // If both parameter 1 & 2 are invalid, it's parameter 1 that complains
            foreach (T2 invalidParam2 in params2(false))
            {
                TryInvalid(
                    (x) => tryAction(x, invalidParam2),
                    params1,
                    paramName1);
            }

            // Verify invalid values in parameter 2
            foreach (T2 invalidParam2 in params2(false))
            {
                //



            }
        }

        /// <summary>
        /// Try a 2-argument action with a variety of valid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        public static void TryValid2<T1,T2>(
            System.Action<T1, T2> tryAction,
            System.Func<bool, IEnumerable<T1>> params1,
            System.Func<bool, IEnumerable<T2>> params2)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(params1 != null);
            Utils.Assert(params2 != null);

            foreach (T2 validParam2 in params2(true))
            {
                TryValid(
                    (x) => tryAction(x, validParam2),
                    params1);
            }
        }

        /// <summary>
        /// Try a 3-argument action with a variety of valid and invalid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        /// <param name="params3"></param>
        /// <param name="paramName1"></param>
        /// <param name="paramName2"></param>
        /// <param name="paramName3"></param>
        public static void Try3<T1,T2,T3>(
            System.Action<T1, T2, T3> tryAction,
            System.Func<bool, IEnumerable<T1>> params1,
            System.Func<bool, IEnumerable<T2>> params2,
            System.Func<bool, IEnumerable<T3>> params3,
            string paramName1,
            string paramName2,
            string paramName3)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(params1 != null);
            Utils.Assert(params2 != null);
            Utils.Assert(params3 != null);
            Utils.Assert(paramName1 != null);
            Utils.Assert(paramName2 != null);
            Utils.Assert(paramName3 != null);

            TryInvalid3(
                tryAction,
                params1,
                params2,
                params3,
                paramName1,
                paramName2,
                paramName3);

            TryValid3(
                tryAction,
                params1,
                params2,
                params3);
        }

        /// <summary>
        /// Try a 3-argument action with a variety of invalid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        /// <param name="params3"></param>
        /// <param name="paramName1"></param>
        /// <param name="paramName2"></param>
        /// <param name="paramName3"></param>
        public static void TryInvalid3<T1,T2,T3>(
            System.Action<T1, T2, T3> tryAction,
            System.Func<bool, IEnumerable<T1>> params1,
            System.Func<bool, IEnumerable<T2>> params2,
            System.Func<bool, IEnumerable<T3>> params3,
            string paramName1,
            string paramName2,
            string paramName3)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(params1 != null);
            Utils.Assert(params2 != null);
            Utils.Assert(params3 != null);
            Utils.Assert(paramName1 != null);
            Utils.Assert(paramName2 != null);
            Utils.Assert(paramName3 != null);

            foreach (T3 validParam3 in params3(true))
            {
                TryInvalid2(
                    (a, b) => tryAction(a, b, validParam3),
                    params1,
                    params2,
                    paramName1,
                    paramName2);
            }

            foreach (T3 invalidParam3 in params3(false))
            {
                //



            }
        }

        /// <summary>
        /// Try a 3-argument action with a variety of valid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        /// <param name="params3"></param>
        public static void TryValid3<T1,T2,T3>(
            System.Action<T1, T2, T3> tryAction,
            System.Func<bool, IEnumerable<T1>> params1,
            System.Func<bool, IEnumerable<T2>> params2,
            System.Func<bool, IEnumerable<T3>> params3)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(params1 != null);
            Utils.Assert(params2 != null);
            Utils.Assert(params3 != null);

            foreach (T3 param3 in params3(true))
            {
                TryValid2(
                    (a, b) => tryAction(a, b, param3), 
                    params1,
                    params2);
            }
        }


        /// <summary>
        /// Try a 4-argument action with a variety of valid and invalid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        /// <param name="params3"></param>
        /// <param name="params4"></param>
        /// <param name="paramName1"></param>
        /// <param name="paramName2"></param>
        /// <param name="paramName3"></param>
        /// <param name="paramName4"></param>
        public static void Try4<T1,T2,T3,T4>(
            System.Action<T1, T2, T3, T4> tryAction,
            System.Func<bool, IEnumerable<T1>> params1,
            System.Func<bool, IEnumerable<T2>> params2,
            System.Func<bool, IEnumerable<T3>> params3,
            System.Func<bool, IEnumerable<T4>> params4,
            string paramName1,
            string paramName2,
            string paramName3,
            string paramName4)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(params1 != null);
            Utils.Assert(params2 != null);
            Utils.Assert(params3 != null);
            Utils.Assert(params4 != null);
            Utils.Assert(paramName1 != null);
            Utils.Assert(paramName2 != null);
            Utils.Assert(paramName3 != null);
            Utils.Assert(paramName4 != null);

            TryInvalid4(
                tryAction,
                params1,
                params2,
                params3,
                params4,
                paramName1,
                paramName2,
                paramName3,
                paramName4);

            TryValid4(
                tryAction,
                params1,
                params2,
                params3,
                params4);
        }

        /// <summary>
        /// Try a 4-argument action with a variety of invalid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        /// <param name="params3"></param>
        /// <param name="params4"></param>
        /// <param name="paramName1"></param>
        /// <param name="paramName2"></param>
        /// <param name="paramName3"></param>
        /// <param name="paramName4"></param>
        public static void TryInvalid4<T1,T2,T3,T4>(
            System.Action<T1, T2, T3, T4> tryAction,
            System.Func<bool, IEnumerable<T1>> params1,
            System.Func<bool, IEnumerable<T2>> params2,
            System.Func<bool, IEnumerable<T3>> params3,
            System.Func<bool, IEnumerable<T4>> params4,
            string paramName1,
            string paramName2,
            string paramName3,
            string paramName4)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(params1 != null);
            Utils.Assert(params2 != null);
            Utils.Assert(params3 != null);
            Utils.Assert(params4 != null);
            Utils.Assert(paramName1 != null);
            Utils.Assert(paramName2 != null);
            Utils.Assert(paramName3 != null);
            Utils.Assert(paramName4 != null);

            foreach (T4 validParam4 in params4(true))
            {
                TryInvalid3(
                    (a, b, c) => tryAction(a, b, c, validParam4),
                    params1,
                    params2,
                    params3,
                    paramName1,
                    paramName2,
                    paramName3);
            }

            foreach (T4 invalidParam4 in params4(false))
            {
                //



            }
        }

        /// <summary>
        /// Try a 4-argument action with a variety of valid parameter values.
        /// </summary>
        /// <param name="tryAction"></param>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        /// <param name="params3"></param>
        /// <param name="params4"></param>
        public static void TryValid4<T1,T2,T3,T4>(
            System.Action<T1, T2, T3, T4> tryAction,
            System.Func<bool, IEnumerable<T1>> params1,
            System.Func<bool, IEnumerable<T2>> params2,
            System.Func<bool, IEnumerable<T3>> params3,
            System.Func<bool, IEnumerable<T4>> params4)
        {
            Utils.Assert(tryAction != null);
            Utils.Assert(params1 != null);
            Utils.Assert(params2 != null);
            Utils.Assert(params3 != null);
            Utils.Assert(params4 != null);

            foreach (T4 param4 in params4(true))
            {
                TryValid3(
                    (a, b, c) => tryAction(a, b, c, param4),
                    params1,
                    params2,
                    params3);
            }
        }
    }
}
