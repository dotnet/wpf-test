// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// Basic Test Foundation
    /// </summary>
    public abstract class Test
    {
        private string _title;

        /// <summary>
        /// Only ctor; a test title is required.
        /// </summary>
        public Test() { }

        /// <summary>
        /// Run this test.
        /// </summary>
        public void Run()
        {
            Setup();
            Utilities.PrintTitle(Title);
            Validate();
        }

        /// <summary>
        /// Setup the test environment.
        /// </summary>
        protected abstract void Setup();

        /// <summary>
        /// Validate test results.
        /// </summary>
        protected abstract void Validate();

        /// <summary>
        /// Test title.
        /// </summary>
        /// <value></value>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }
    }
}

