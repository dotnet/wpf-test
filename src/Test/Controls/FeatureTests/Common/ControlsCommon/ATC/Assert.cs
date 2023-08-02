//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.Text.RegularExpressions;

using Microsoft.Test.Logging;

#endregion

namespace Avalon.Test.ComponentModel.Utilities
{
    /// <summary>
    /// helper class for wrint unit test cases
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// an exception type specially for AbstractUnitTest
        /// make it private in Assert class to prevent any access from outside.
        /// the reason for this is that this exception is mainly used to indicate a failure,
        /// while other kind of exceptions will indicate errors. and only Assert can 
        /// create this kind of exceptions to indicate failures.
        /// </summary>
        private class AssertFailureException : Exception
        {
            internal AssertFailureException(string msg)
                : base(msg)
            {
            }
        }
        /// <summary>
        /// helper function to make assertion more simple. 
        /// </summary>
        /// <param name="msg">error message</param>
        /// <param name="expression"></param>
        public static void AssertTrue(string msg, bool expression)
        {
            if (!expression)
                Fail(msg);
        }

        /// <summary>
        /// helper function to make assertion more simple. 
        /// </summary>
        /// <param name="msg">error message</param>
        /// <param name="expression"></param>
        public static void AssertFalse(string msg, bool expression)
        {
            if (expression)
                Fail(msg);
        }

        /// <summary>
        /// helper function to make assertion more simple. 
        /// </summary>
        /// <param name="msg">error message</param>
        /// <param name="expected">expected value</param>
        /// <param name="actual">actual value</param>
        public static void AssertEqual(string msg, object expected, object actual)
        {
            if (expected == actual)
                return;
            if (expected != null && expected.Equals(actual))
                return;
            else
                Fail(msg + "\t{expected=[" + expected + "], actual=[" + actual + "]}");
        }

        /// <summary>
        /// helper function to make assertion more simple. 
        /// </summary>
        /// <param name="msg">error message</param>
        public static void Fail(string msg)
        {
            throw new AssertFailureException(msg);
        }

        public delegate void Run();

        /// <summary>
        /// filter out the exact line of code that goes wrong
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <returns>line info or empty string if not found</returns>
        private static string FindCodeLineInfo(string stackTrace)
        {
            Regex reg = new Regex(@"Avalon\.Test\.ComponentModel\.Utilities\.Assert.*");
            string[] trace = stackTrace.Split(System.Environment.NewLine.ToCharArray());
            foreach (string s in trace)
            {
                if (s != string.Empty)
                {
                    if (reg.IsMatch(s))
                        continue;
                    return Regex.Replace(s.Trim(), @".* in (.*):line (.*)", "$1($2)");
                }
            }
            return string.Empty;
        }

        public static bool RunTest(Run run)
        {
            try
            {
                run();
                return true;
            }
            catch (AssertFailureException e)
            {
                // test failed
                GlobalLog.LogEvidence(FindCodeLineInfo(e.StackTrace) + ": " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                // unexpected exception occured, it should be an error
                GlobalLog.LogEvidence("Exception Thrown: type=[" + e.GetType().Name 
                    + "], message=[" + e.Message + "]" + "\n" + e.StackTrace);
                return false;
            }
        }
    }
}
