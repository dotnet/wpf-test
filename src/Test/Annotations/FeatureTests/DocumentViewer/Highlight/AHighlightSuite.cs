// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Base class for highlight tests.

using System;
using System.Windows;
using Annotations.Test.Framework;
using Annotations.Test;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Avalon.Test.Annotations
{
    [TestDimension("flow,fixed,fixed /fds=false")]
    [TestDimension("visible,nonvisible")]
	public abstract class AHighlightSuite : ADefaultContentSuite
	{
        #region Overrides

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);
            DetermineVisibleMode(args);            
        }

        protected override IList<string> UsageParameters()
        {
            IList<string> parameters = UsageParameters();
            parameters.Add("[nonvisible] - tests may run so that operations are tested on non-visible content.");
            return parameters;
        }

        protected override IList<string> UsageExamples()
        {
            IList<string> examples = base.UsageExamples();
            examples.Add("XXX.exe highlight1 flow nonvisible - run test case in non-visible mode.");
            return examples;
        }        

		/// <summary>
		/// Always returns in Highlight mode.
		/// </summary>
        protected override AnnotationMode DetermineAnnotationMode(string[] args)
        {
            return AnnotationMode.Highlight;
        }

        #endregion

        #region Protected Methods

        protected void CreateHighlight(ISelectionData selection, int colorIndex)
        {
            CreateHighlight(DocViewerWrapper, selection, colorIndex);
        }

        new protected void CreateAnnotation(AnnotationDefinition definition)
        {
            definition.Create(DocViewerWrapper);
        }

        #endregion

        #region Protected Properties

        protected bool NonVisibleMode
        {
            get
            {
                return _nonVisibleMode;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Look for argument 'nonvisible' to make NonVisibleMode == true.
        /// </summary>
        private void DetermineVisibleMode(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.Equals("nonvisible"))
                {
                    _nonVisibleMode= true;
                    break;
                }
            }

            printStatus("Visibility Mode = '" + ((NonVisibleMode) ? "Non-Visible" : "Visible") + "'.");
        }

        #endregion

        #region Private Fields

        private bool _nonVisibleMode = false;

        #endregion
    }
}	

