// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//  Description: A single test case variation.
//  Creator: derekme
//  Date Created: 9/8/05
//---------------------------------------------------------------------
using System;
using System.Text;

namespace Annotations.Test.Framework.Internal
{
    internal enum VariationResult
    {
        Unresolved,
        Passed,
        Failed
    }

    internal enum VariationStage
    {
        Setup,
        Action,
        Cleanup
    }

    internal class TestVariation
    {
        public TestVariation(TestCase test, string[] args)
        {
            TestCase = test;
            Parameters = args;
        }

        public string Message = string.Empty;
        public VariationResult Result = VariationResult.Unresolved;
        public string[] Parameters = new string[0];
        public TestCase TestCase;

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            if (Parameters.Length == 0 || !Parameters[0].Equals(TestCase.Id))
                str.Append(TestCase.Id + " ");

            foreach (string param in Parameters)
                str.Append(param + " ");

            int[] bugs = TestCase.Bugs;
            for (int i = 0; i < bugs.Length; i++)
                str.Append("Bug#" + bugs[i] + " ");

            return str.ToString();
        }
    }
}
