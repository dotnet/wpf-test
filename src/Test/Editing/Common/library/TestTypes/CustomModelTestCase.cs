// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TestTypes
{
    #region Namespaces.

    using System;
    using System.Windows;
    using System.Collections;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.TestTypes;
    using Test.Uis.Wrappers;
    using Microsoft.Test.KoKoMo;

    #endregion Namespaces.

    /// <summary>
    /// Manage to run a collection of combined test cases.
    /// example of using this class: GroupedCustomTestCase
    /// </summary>
    public abstract class CustomModelTestCase : CustomTestCase
    {
        private ModelEngine _engine;

        protected override void DoMainWindowShown()
        {
            base.DoMainWindowShown();
            _engine = new ModelEngine(this);
            // Add event handler for post action processing
            this.CallAfter += new CallAfterHandler(AfterCallHandler);
        }

        /// <summary>
        /// This abstract method is the event handler after each call is completed. 
        /// This method intend for user to do verfication.
        /// Note: if we do UI test, this will make not make sure that Avalon is at idle stage.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        protected abstract void AfterCallHandler(ModelAction action, ModelParameters parameters, object result);

        /// <summary>
        /// 
        /// </summary>
        public ModelEngine TestModeEngine
        {
            get
            {
                return _engine;
            }
        }
    }
}
