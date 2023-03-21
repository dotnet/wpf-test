// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using System.Drawing;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;

namespace Avalon.Test.Annotations
{
	public class AAnchoredBlockSuite : AFlowSuite
	{
        #region Overrides

        public override IDocumentPaginatorSource  FlowContent
        {
            get 
            {
                return LoadContent(ViewerTestConstants.AnchoredBlockTests.Filename);
            }
        }

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);

            if (CaseNumber.Contains("figure"))
            {
                TargetType = typeof(Figure);
                TargetName = ViewerTestConstants.AnchoredBlockTests.Figure1;
            }
            else
            {
                TargetType = typeof(Floater);
                TargetName = ViewerTestConstants.AnchoredBlockTests.Floater1;
            }
        }

        protected override AnnotationMode DetermineAnnotationMode(string[] args)
        {
            if (CaseNumber.Contains("highlight"))
                return AnnotationMode.Highlight;
            return base.DetermineAnnotationMode(args);
        }

        #endregion    

        #region Protected Methods

        /// <summary>
        /// Helper for creating and AnchoredBlockSelectionData object with a specific block as the target.
        /// </summary>
        protected ISelectionData AnchoredBlockSelection(PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
        {

            return new AnchoredBlockSelectionData(TargetType, TargetName, startPosition, startOffset, endPosition, endOffset);
        }

        #endregion

        #region Protected Fields

        protected Type TargetType;
        protected string TargetName;

        #endregion
    }
}	

