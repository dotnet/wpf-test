// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// Basic Test Foundation
    /// </summary>
    public abstract class TestApplication
    {
        private string _testTitle = "Derived Application Should Provide Better Test Title";
        private Window _mainWindow = null;
        private string _title;

        /// <summary>
        /// Derived class should set appropriate Test Title
        /// </summary>
        /// <value>Test Title String</value>
        protected string TestTitle
        {
          get
          {
            return _testTitle;
          }
          set
          {
            _testTitle = value;
            if (_mainWindow != null)
            {
                _mainWindow.Title = _testTitle;
            }
            else
            {
                throw new TestValidationException("ERROR: mainWindow returned null.");
            }
          }
        }

        /// <summary>
        /// Only ctor; a test title is required.
        /// </summary>
        public TestApplication() { }

        /// <summary>
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <param name="rootWindow"></param>
        protected void TestSetupUI(StackPanel parentPanel, Window rootWindow)
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
        /// </summary>
        /// <returns></returns>
        protected bool TestCompleted()
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
    }
}

