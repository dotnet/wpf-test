// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Flow DRT Base.
//

using System;                       // Type
using System.Reflection;            // Assembly
using System.Windows;               // FrameworkElement

namespace DRT
{
    /// <summary>
    /// Flow DRT Base.
    /// </summary>
    internal abstract class DrtFlowBase : DrtBase
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        protected DrtFlowBase()
        {
            _mode = ExecutionMode.Drt;
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static DrtFlowBase()
        {
            _pfAssembly = Assembly.GetAssembly(typeof(FrameworkElement));
            _pcAssembly = Assembly.GetAssembly(typeof(UIElement));

            // Disable Background Layout.
            Type typeBackgroundFormatInfo = _pfAssembly.GetType("MS.Internal.PtsHost.BackgroundFormatInfo");
            FieldInfo fieldInfo = typeBackgroundFormatInfo.GetField("_isBackgroundFormatEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, false);
        }

        #endregion Constructors

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Handle command-line arguments one-by-one.
        /// </summary>
        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
            {
                return true;
            }

            // Process arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            if (option)
            {
                switch (arg)
                {
                    case "app":
                        _mode = ExecutionMode.App;
                        this.AssertsAsExceptions = false;
                        this.KeepAlive = true;
                        this.Suites = new DrtTestSuite[] {
                            new ApplicationTestSuite(TestSuites),
                        };
                        break;
                    case "diff":
                        _mode = ExecutionMode.Diff;
                        break;
                    case "update":
                        _mode = ExecutionMode.Update;
                        break;
                    default: // Unknown option. Don't handle it.
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Print a description of command line arguments.
        /// </summary>
        protected override void PrintOptions()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("  -app : Run in standalone application mode.");
            Console.WriteLine("  -diff : Show diff for failing test cases.");
            Console.WriteLine("  -update : Update masters for failing test cases.");
            base.PrintOptions();
        }

        #endregion Protected Methods

        //-------------------------------------------------------------------
        //
        //  Internal Methods/Properties
        //
        //-------------------------------------------------------------------

        #region Internal Methods/Properties

        /// <summary>
        /// Test suites for this DRT.
        /// </summary>
        internal abstract DrtTestSuite[] TestSuites { get; }

        /// <summary>
        /// Location of all DRT related files.
        /// </summary>
        internal abstract string DrtFilesDirectory { get; }

        /// <summary>
        /// SD location of DRT files.
        /// </summary>
        internal virtual string DrtSDDirectory { get { return String.Empty; } }

        /// <summary>
        /// Execution mode for DrtFlow application.
        /// </summary>
        internal ExecutionMode Mode { get { return _mode; } }

        /// <summary>
        /// PresentationFramework assembly reference.
        /// </summary>
        internal static Assembly FrameworkAssembly { get { return _pfAssembly; } }

        /// <summary>
        /// PresentationCore assembly reference.
        /// </summary>
        internal static Assembly CoreAssembly { get { return _pcAssembly; } }

        #endregion Internal Methods/Properties

        //-------------------------------------------------------------------
        //
        //  Internal Types
        //
        //-------------------------------------------------------------------

        #region Internal Types

        /// <summary>
        /// Execution mode for DrtFlow application.
        /// </summary>
        internal enum ExecutionMode
        {
            Drt,            // DRT execution mode [default]
            App,            // standalone application mode
            Diff,           // shows diff for failing test cases
            Update,         // updates masters for failing test cases
        }

        #endregion Internal Types

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields

        /// <summary>
        /// Execution mode for DrtFlow application.
        /// </summary>
        private ExecutionMode _mode;

        /// <summary>
        /// PresentationFramework assembly reference.
        /// </summary>
        private static Assembly _pfAssembly;

        /// <summary>
        /// PresentationCore assembly reference.
        /// </summary>
        private static Assembly _pcAssembly;

        #endregion Private Fields
    }
}
