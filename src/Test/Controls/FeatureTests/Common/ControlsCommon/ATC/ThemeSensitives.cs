//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;

using Avalon.Test.ComponentModel.Utilities;

#endregion

namespace Avalon.Test.ComponentModel.Validations
{
    /// <summary>
    /// ThemeSensitive is a test object that is theme sensitive.
    /// <code>
    /// <ThemeSensitive>
    ///     <ThemeSensitive.Classic>
    ///         <!--Test in Classic theme-->
    ///     </ThemeSensitive.Classic>
    ///     <ThemeSensitive.Aero>
    ///         <!--Test in Aero theme-->
    ///     </ThemeSensitive.Aero>
    /// </ThemeSensitive>
    /// </code>
    /// </summary>
    public sealed class ThemeSensitive : TestObject
    {
        protected sealed override void DoCore()
        {
            string theme = DisplayConfiguration.GetTheme();
            switch (theme.ToLower())
            {
                case "windows classic":
                    DoInClassic();
                    break;
                case "luna":
                    DoInLuna();
                    break;
                case "aero":
                    DoInAero();
                    break;
                default:
                    Assert.Fail("No supported theme: " + theme);
                    break;
            }
        }

        /// <summary>
        /// Executed on Classic theme.
        /// </summary>
        private void DoInClassic()
        {
            DoSteps(Classic);
        }

        private void DoSteps(IList<ITestStep> steps)
        {
            foreach (ITestStep step in steps)
            {
                step.Do(Target);
            }
        }

        /// <summary>
        /// Executed on Luna theme
        /// </summary>
        private void DoInLuna()
        {
            if (Luna.Count == 0)
            {
                DoInClassic();
                return;
            }
            DoSteps(Luna);
        }

        /// <summary>
        /// Executed on Aero theme
        /// </summary>
        private void DoInAero()
        {
            if (Aero.Count == 0)
            {
                DoInClassic();
                return;
            }
            DoSteps(Aero);
        }

        public ObservableCollection<ITestStep> Classic
        {
            get { return _classic; }
        }

        private ObservableCollection<ITestStep> _classic = new ObservableCollection<ITestStep>();

        private ObservableCollection<ITestStep> _luna = new ObservableCollection<ITestStep>();

        public ObservableCollection<ITestStep> Luna
        {
            get { return _luna; }
        }

        private ObservableCollection<ITestStep> _aero = new ObservableCollection<ITestStep>();

        public ObservableCollection<ITestStep> Aero
        {
            get { return _aero; }
        }
    }
}
