// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Proxies.System.Windows.Annotations;
using PSWC = Proxies.System.Windows.Controls;

namespace Avalon.Test.Annotations
{
    [TestDimension("flow,fixed")]
    public abstract class AStickyNoteWithAnchorSuite : ADefaultContentSuite
    {
        #region Protected Methods

        /// <summary>
        /// VScan files will be located in different locations depending upon if we are running in Piper or not.
        /// In Piper: vscan files will be in the local directory.
        /// Standalon: vscan files will bin in VScan\Fixed and VScan\Flow subdirectories.
        /// </summary>
        protected string VScanPath
        {
            get
            {
                if (!Directory.Exists(MODEL_BASE_PATH))
                    return "";
                return (ContentMode == TestMode.Fixed) ? FIXED_MODEL_PATH : FLOW_MODEL_PATH;
            }
        }

        /// <summary>
        /// Compare screenshot of DocumentViewer contents to given Master.  Tries to locate a tolerance file
        /// based on the name of the master.
        /// </summary>
        /// <param name="masterFile">Relative path to master.</param>
        protected void CompareToMaster(string masterFile)
        {
            string[] segments = masterFile.Split('.');
            base.CompareToMaster(VScanPath + masterFile, null);
        }

        /// <summary>
        /// Compare screenshot of DocumentViewer contents to given Master.
        /// </summary>
        /// <param name="masterFile">Relative path to master.</param>
        protected override void CompareToMaster(string masterFile, string toleranceFile)
        {
            string path = VScanPath;
            base.CompareToMaster(path + masterFile, path + toleranceFile);
        }

        /// <summary>
        /// Create a new StickyNoteWithAnchor for the current selection of the DocumentViewer.  Will
        /// walk the Visual tree and find the new StickyNoteControl and return a wrapper for it.
        /// </summary>
        /// <remarks>If StickyNotes are created without going through this method than the wrapper
        /// returned is not garuanteed to be for the most recently created StickyNote.</remarks>
        /// <returns>Wrapper for the newly created StickyNote.</returns>
        protected StickyNoteWrapper CreateStickyNoteWithAnchor()
        {
            return CreateStickyNoteWithAnchor(null, null);
        }

        /// <summary>
        /// Create a new StickyNoteWithAnchor for the current selection of the DocumentViewer.  Will
        /// walk the Visual tree and find the new StickyNoteControl and return a wrapper for it.
        /// </summary>
        /// <remarks>
        /// 1. If StickyNotes are created without going through this method than the wrapper
        /// returned is not garuanteed to be for the most recently created StickyNote.
        /// 2. If create is called for a non-visible page then no StickyNote component will actually
        /// be created and therefore this method will return null.
        /// </remarks>
        /// <returns>Wrapper for the newly created StickyNote, or null if created on non-visible page.</returns>
        /// <param name="selection">Selection to create SNwA on.  If null, then use current selection.</param>
        /// <param name="title">String to set SN title to.  If null, leave as default.</param>
        /// <exception cref="TestFailedException">If CreateAnnotation returns false.</exception>
        protected StickyNoteWrapper CreateStickyNoteWithAnchor(ISelectionData selection, string author)
        {
            DoCreate(selection, AnnotationMode.StickyNote, true, author);			

            StickyNoteWrapper wrapper = null;
            IList<StickyNoteControl> visibleSNs = AnnotationComponentFinder.GetVisibleStickyNotes(ViewerBase);
            foreach (Control sn in visibleSNs)
            {
                if (string.IsNullOrEmpty(sn.Name) || !sn.Name.Contains(_snidentifier))
                {
                    wrapper = new StickyNoteWrapper(sn, _snidentifier + _currentId.ToString());
                    _currentId++;
                    break;
                }
            }

            return wrapper;
        }

        /// <summary>
        /// Trivial wrapper for creating a Highlight annotation.
        /// </summary>
        protected void CreateHighlight(ISelectionData selection)
        {
            DoCreate(selection, AnnotationMode.Highlight, true, String.Empty);
        }

		protected void CreateHighlight(ISelectionData selection, bool verifyCreated)
		{
			DoCreate(selection, AnnotationMode.Highlight, verifyCreated, String.Empty);
		}

        /// <summary>
        /// Trivial wrapper for creating a Highlight annotation.
        /// </summary>
        protected void CreateHighlight()
        {
            DoCreate(null, AnnotationMode.Highlight, true, String.Empty);
        }		

        protected void DeleteStickyNoteWithAnchor(ISelectionData selection)
        {
            DoDelete(selection, AnnotationMode.StickyNote);
        }

        protected void DeleteHighlight(ISelectionData selection)
        {
            DoDelete(selection, AnnotationMode.Highlight);
        }

        #endregion

        #region Protected Variables

        protected static string MODEL_BASE_PATH = @"VScan\";
        protected static string FIXED_MODEL_PATH = MODEL_BASE_PATH + @"Fixed\";
        protected static string FLOW_MODEL_PATH = MODEL_BASE_PATH + @"Flow\";

        #endregion

        #region Private Methods

        private void DoCreate(ISelectionData selection, AnnotationMode type, bool verifyCreated, string author)
        {
            if (selection != null)
                selection.SetSelection(DocViewerWrapper.SelectionModule);
			bool result = CreateAnnotation(type, author);
			if (verifyCreated)
				Assert("Verify annotation was created.", result);						
        }

        private void DoDelete(ISelectionData selection, AnnotationMode type)
        {
            if (selection != null)
                selection.SetSelection(DocViewerWrapper.SelectionModule);
            DeleteAnnotation(type);
        }

        #endregion

        #region Private Variables

        private string _snidentifier = "snid";
        private int _currentId = 0;

        #endregion
    }
}   

