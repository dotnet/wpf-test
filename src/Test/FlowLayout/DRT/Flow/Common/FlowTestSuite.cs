// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for Flow test suites.
//

using System;                       // string
using System.IO;                    // Stream
using System.Windows.Controls;      // Border
using System.Windows.Media;         // Brush
using System.Windows.Markup;        // XamlReader

namespace DRT
{
    /// <summary>
    /// Common functionality for Flow test suites.
    /// </summary>
    internal abstract class FlowTestSuite : DrtTestSuite
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
        /// <param name="suiteName">Name of the suite.</param>
        protected FlowTestSuite(string suiteName) : 
            base(suiteName)
        {
        }

        #endregion Constructors

        //-------------------------------------------------------------------
        //
        //  Protected Methods/Properties
        //
        //-------------------------------------------------------------------

        #region Protected Methods/Properties

        /// <summary>
        /// Initializes the suite and returns collection of tests.
        /// </summary>
        /// <returns>Returns collection of tests.</returns>
        public override DrtTest[] PrepareTests()
        {
            // Initialize the suite here. This includes loading the tree.
            _root = new Border();
            _root.Background = Brushes.White;
            _root.Width = 800;
            _root.Height = 600;

            // Do not show the root, if DRT is in Application mode.
            if (!(DRT is DrtFlowBase) || ((DrtFlowBase)DRT).Mode != DrtFlowBase.ExecutionMode.App)
            {
                DRT.Show(_root);
            }

            // Return the lists of tests to run against the tree
            return CreateTests();
        }

        /// <summary>
        /// Returns collection of tests.
        /// </summary>
        protected abstract DrtTest[] CreateTests();

        /// <summary>
        /// Loads content from xaml file.
        /// </summary>
        /// <param name="xamlFileName">Xaml file name.</param>
        /// <returns>Returns loaded element.</returns>
        protected object LoadFromXaml(string xamlFileName)
        {
            object content = null;
            Stream stream = null;
            string fileName = this.DrtFilesDirectory + xamlFileName;
            try
            {
                stream = File.OpenRead(fileName);
                content = XamlReader.Load(stream);
            }
            finally
            {
                // done with the stream
                if (stream != null) { stream.Close(); }
            }
            DRT.Assert(content != null, "Failed to load xaml file '{0}'", xamlFileName);
            return content;
        }

        /// <summary>
        /// Empty test callback.
        /// </summary>
        protected void Empty() { }

        /// <summary>
        /// Location of all DRT related files.
        /// </summary>
        protected string DrtFilesDirectory { get { return ((DrtFlowBase)DRT).DrtFilesDirectory;  } }

        /// <summary>
        /// Root UIElement of the test suite. Placeholder for content.
        /// </summary>
        internal Border Root { get { return _root; } }

        #endregion Protected Methods/Properties

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields

        /// <summary>
        /// Root UIElement of the test suite. Placeholder for content.
        /// </summary>
        private Border _root;

        #endregion Private Fields
    }
}
