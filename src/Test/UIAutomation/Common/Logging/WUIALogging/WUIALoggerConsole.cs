// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Test.WindowsUIAutomation.Logging;

namespace Microsoft.Test.WindowsUIAutomation.Logging
{
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class ConsoleLogger : BaseLogger
    {
        #region Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        string _LINE1 = "===============================================================================";

        #endregion Variables

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        public ConsoleLogger()
        {
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        override public void CacheCodeExampleVariables(string elementPath, string exampleCall)
        {
            WriteOutComment("Path         : " + elementPath);
            WriteOutComment("Example Call : " + exampleCall);
            elementPath = "InternalHelper.Helpers.GetAutomationElementFromXmlPath(\"" + elementPath + "\" );";
            base.CacheCodeExampleVariables(elementPath, exampleCall);
        }

        #region Overrides

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        override protected void WriteOutComment(object comment)
        {
            Type type = comment.GetType();
            if (type == typeof(string))
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(comment.ToString());
                Console.ForegroundColor = consoleColor;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        override protected void WriteOutFail(object comment)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(comment.ToString());
            Console.ForegroundColor = consoleColor;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        override protected void WriteStartTest(object testName)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(_LINE1);
            Console.WriteLine("Test : " + testName);
            Console.WriteLine(_LINE1);
            Console.ForegroundColor = consoleColor;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        override protected void WriteOutEndTest()
        {
            ConsoleColor consoleColor = Console.ForegroundColor;

            if (_testResult == TestResult.Pass)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("*************************** Pass : Completed ****************************");
                Console.WriteLine("* " + _runningTest + "Passed");
                Console.WriteLine("*************************************************************************");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("*************************** Failed : Completed **************************");
                Console.WriteLine("* " + _runningTest + "Failed");
                Console.WriteLine("*************************************************************************");
            }

            Console.ForegroundColor = consoleColor;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------

        #endregion Overrides

    }
}
