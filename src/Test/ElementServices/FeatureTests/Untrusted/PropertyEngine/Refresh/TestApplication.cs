// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Documents;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.UtilityHelper;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// Basic Test Foundation
    /// </summary>
    public abstract class Test
    {
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

        private string _title;
    }

    /// <summary>
    /// Basic Test Foundation
    /// </summary>
    public abstract class TestApplication : PEApplication
    {
        /// <summary>
        /// Only ctor; a test title is required.
        /// </summary>
        public TestApplication() { }

        /// <summary>
        /// PEApplication override, forward to Setup.
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <param name="rootWindow"></param>
        protected override void TestSetupUI(StackPanel parentPanel, Window rootWindow)
        {
            Setup(parentPanel);
            TestTitle = Title;
            Utilities.PrintTitle(Title);
        }

        /// <summary>
        /// Setup the test environment.
        /// </summary>
        protected abstract void Setup(StackPanel spRoot);

        /// <summary>
        /// PEApplication override, forward to Validate
        /// </summary>
        /// <returns></returns>
        protected override bool TestCompleted()
        {
            Validate();
            return true; // If a problem occured an exception will be thrown
        }

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

        private string _title;
    }
}

