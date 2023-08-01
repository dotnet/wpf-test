// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Serialization;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.IdTest
{
    /// <summary>
    /// A class to test IdScope
    /// </summary>
    [Test(0, "IdTest", "StyleIdTestCase1")]
    public class StyleIdTestCase1 : IdTestBaseCase
    {
        #region
        private Style _style = null;
        private FrameworkElementFactory _textFactory = null;
        #endregion


        #region Constructor
        public StyleIdTestCase1()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(BasicAction);
        }
        #endregion


        #region Test Steps
        /// <summary>
        /// Initialize.
        /// </summary>
        /// <returns>A TestResul.Pass</returns>
        TestResult Initialize()
        {
            CreateTree();
            _style = CreateStyle();

            return TestResult.Pass;
        }

        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult BasicAction()
        {
            GlobalLog.LogStatus("basic action for Id scope on Style.");

            //register a Id for textFactory
            RegisterName("element", _textFactory, _style);

            //find the element from style
            FrameworkElementFactory foundElement = FindElementWithId(_style, "element") as FrameworkElementFactory;

            GlobalLog.LogStatus("Verifying element found from style is correct...:" + foundElement.GetType().ToString());
            VerifyElement.VerifyBool(foundElement == _textFactory, true);

            //unregiser id
            UnregisterName("element", _style);

            //should not find it.
            try
            {
                foundElement = FindElementWithId(_style, "element") as FrameworkElementFactory;
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Should be no exception: " + e.Message);
            }

            if (null != foundElement)
            {
                throw new Microsoft.Test.TestValidationException("Should not have found button because it has been removed from Id Scope");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Methods
        private Style CreateStyle()
        {
            Style s = new Style(typeof(Button));
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Border));

            _textFactory = new FrameworkElementFactory(typeof(TextBlock));
            //register the text
            _textFactory.Name = "element";
            RegisterName("element", _textFactory, s);
            factory.AppendChild(_textFactory);
            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = factory;
            s.Setters.Add(new Setter(Button.TemplateProperty, template));

            return s;
        }
        #endregion
    }
}
