// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.Collections.Generic;
using System.Text;
using Avalon.Test.Annotations;
using System.Windows.Annotations;
using System.Windows.Controls;


using Proxy_AnnotationPublicNamespace = Proxies.System.Windows.Annotations;
using Proxy_AnnotationInternalNamespace = Proxies.MS.Internal.Annotations;
using System.Xml;
using System.Collections.ObjectModel;
using Annotations.Test.Framework;
using System.Collections;
using Microsoft.Test.Logging;
using System.Windows.Annotations.Storage;
using System.IO;
using System.Windows.Media;
using System.Windows.Documents;
using Microsoft.Test.Discovery;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Xml.Schema;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Annotations.FeatureTests.DocumentViewer
{
    public class StickyNoteIAnchorInfoSuite : AStickyNoteControlSuite
    {
        #region Test Case

        /// <summary>
        /// Single StickyNote.
        /// Get IAnchorInfo from StickyNote.IAnchorInfo
        /// </summary>
        [Priority(0)]
        protected void ianchor1_01()
        {
            //create annotation, verify that it is createtd.
            CreateAnnotation(new SimpleSelectionData(0, 100, 125));
            VerifyAnnotation(GetText(new SimpleSelectionData(0, 100, 125)));

            //get the AnchorInfo of the current StickyNote object
            IAnchorInfo anchorinfo = this.Note.AnchorInfo;

            //verify that the IAnchorInfo is not null
            AssertNotNull("IAnchorInfo is null.", anchorinfo);

            //Compare IAnchorInfo to IAttachedAnnotation
            IAnchorInfoHelper.VerifyAnnotationProxies(GetText(new SimpleSelectionData(0, 100, 400)), anchorinfo, Service);

            //if it makes it this far, test passess
            passTest("StickyNote IAnchorInfo test 1 passed.");
        }

        /// <summary>
        /// Two StickyNotes.
        /// Get IAnchorInfo from StickyNote.IAnchorInfo
        /// </summary>
        [Priority(1)]
        protected void ianchor1_02()
        {
            GoToPage(2);
            //create two annotations
            CreateAnnotation(new SimpleSelectionData(2, PagePosition.Beginning, 300));
            CreateAnnotation(new SimpleSelectionData(2, PagePosition.End, -200));

            //verify both are there.
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, PagePosition.Beginning, 300)),
                GetText(new SimpleSelectionData(2, PagePosition.End, -200))
            });

            //Compare IAnchorInfo to IAttachedAnnotation
            IAnchorInfoHelper.VerifyAnnotationProxies(new string[] {
                GetText(new SimpleSelectionData(2, PagePosition.Beginning, 300)),
                GetText(new SimpleSelectionData(2, PagePosition.End, -200))},
                IAnchorInfoHelper.GetIAnchorInfos(GetStickyNoteWrappers()),
                2,
                Service
            );

            //if it makes it this far, test passess
            passTest("StickyNote IAnchorInfo test 2 passed.");
        }

        /// <summary>
        /// Four overlapping StickyNotes.
        /// Get IAnchorInfo from StickyNote.IAnchorInfo
        /// </summary>
        [Priority(1)]
        protected void ianchor1_03()
        {
            GoToPage(2);
            CreateAnnotation(new SimpleSelectionData(2, 0, 100));
            CreateAnnotation(new SimpleSelectionData(2, 75, 100));
            CreateAnnotation(new SimpleSelectionData(2, 150, 100));
            CreateAnnotation(new SimpleSelectionData(2, 225, 100));

            //verify all are there.
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 0, 100)),
                GetText(new SimpleSelectionData(2, 75, 100)),
                GetText(new SimpleSelectionData(2, 150, 100)),
                GetText(new SimpleSelectionData(2, 225, 100))
            });

            //Compare IAnchorInfo to IAttachedAnnotation
            IAnchorInfoHelper.VerifyAnnotationProxies(new string[] {
                GetText(new SimpleSelectionData(2, 0, 100)),
                GetText(new SimpleSelectionData(2, 75, 100)),
                GetText(new SimpleSelectionData(2, 150, 100)),
                GetText(new SimpleSelectionData(2, 225, 100))},
                IAnchorInfoHelper.GetIAnchorInfos(GetStickyNoteWrappers()),
                4,
                Service
            );

            passTest("StickyNote IAnchorInfo test 3 passed.");
        }

        /// <summary>
        /// Add annotation to the store, get anchorinfo
        /// </summary>
        [Priority(1)]
        protected void ianchor1_04()
        {
            //Set Selection.
            SetSelection(new SimpleSelectionData(0, 100, 500));

            //Start Annotation Service.
            AnnotationService service = (AnnotationService)Service.Delegate;

            //Load Annotation
            Annotation annotation = IAnchorInfoHelper.CreateNewAnnotation();//.MakeNewAnnotation();

            //Add Annotation to store
            service.Store.AddAnnotation(annotation);
            
            AnnotationHelper.CreateTextStickyNoteForSelection(service, "otis");

            IList<Annotation> annotations = service.Store.GetAnnotations();

            //verify both are there.
            VerifyAnnotations(new string[]{
                GetText(new SimpleSelectionData(0, 100, 500))
            });

            IAnchorInfo ianchorinfo = this.Note.AnchorInfo;
                     
            //verify that the IAnchorInfo is not null
            AssertNotNull("IAnchorInfo is null.", ianchorinfo);


            //Compare IAnchorInfo to IAttachedAnnotation
            IAnchorInfoHelper.VerifyAnnotationProxies(
                GetText(new SimpleSelectionData(0, 100, 500)),
                ianchorinfo,
                Service
            );

            passTest("StickyNote IAnchorInfo test 4 passed.");
        }

        /// <summary>
        /// Get IAnchorInfo of non visible StickyNote
        /// </summary>
        [Priority(1)]
        protected void ianchor1_05()
        {
            AnnotationService service = (AnnotationService)Service.Delegate;

            GoToPage(2);
            CreateAnnotation(new SimpleSelectionData(2, 105, 35));
            VerifyAnnotation(GetText(new SimpleSelectionData(2, 105, 35)));
            GoToPage(0);

            IList<Annotation> annotations = service.Store.GetAnnotations();
            AssertEquals("GetAnnotations returned incorrect amount.", 1, annotations.Count);

            foreach (Annotation annotation in annotations)
            {
                IAnchorInfo anchorinfo = AnnotationHelper.GetAnchorInfo(service, annotation);
                AssertNotNull("IAnchorInfo is null.", anchorinfo);
            }

            passTest("No error.");
        }
  
        /// <summary>
        /// Get IAnchorInfo of non visible StickyNote
        /// </summary>
        [Priority(0)]
        protected void ianchor1_06()
        {
            AnnotationService service = (AnnotationService)Service.Delegate;

            CreateAnnotation(new SimpleSelectionData(2, 105, 35));
            
            IList<Annotation> annotations = service.Store.GetAnnotations();
            AssertEquals("GetAnnotations returned incorrect amount.", 1, annotations.Count);

            foreach (Annotation annotation in annotations)
            {
                IAnchorInfo anchorinfo = AnnotationHelper.GetAnchorInfo(service, annotation);
                AssertNotNull("IAnchorInfo is null.", anchorinfo);
            }

            GoToPage(2);

            IAnchorInfoHelper.VerifyAnnotationProxies(
                GetText(new SimpleSelectionData(2, 105, 35)),
                IAnchorInfoHelper.GetIAnchorInfos(GetStickyNoteWrappers()),
                Service
            );

            passTest("No error.");
        }

        /// <summary>
        /// Load XML Store.
        /// Get IAnchorInfo for each Annotation in the store.
        /// </summary>
        [Priority(0)]
        protected void ianchor1_07()
        {
            AnnotationService service = (AnnotationService)Service.Delegate;

            if (ContentMode == TestMode.Fixed)
            {
                IAnchorInfoHelper.ImportAnnotationsStore("Fixed_Annotations.xml", service);
            }
            else
            {
                IAnchorInfoHelper.ImportAnnotationsStore("Flow_Annotations.xml", service);
            }

            IList<Annotation> annotations = service.Store.GetAnnotations();

            foreach (Annotation annotation in annotations)
            {
                IAnchorInfo anchorinfo = AnnotationHelper.GetAnchorInfo(service, annotation);
                AssertNotNull("IAnchorInfo is null.", anchorinfo);
            }

            passTest("No Errors");
        }

        /// <summary>
        /// Load XML Store.
        /// Get IAnchorInfo for each Annotation in the store.
        /// Navigate to a TextRange, Validate the Anchor is now attached.
        /// </summary>
        [Priority(0)]
        protected void ianchor1_08()
        {
            IDocumentPaginatorSource source = this.DocViewerWrapper.Document;

            //Start AnnotationService
            AnnotationService service = (AnnotationService)Service.Delegate;

            //Load list of Annotations
            if (ContentMode == TestMode.Fixed)
            {
                IAnchorInfoHelper.ImportAnnotationsStore("Fixed_Annotations.xml", service);
            }
            else
            {
                IAnchorInfoHelper.ImportAnnotationsStore("Flow_Annotations.xml", service);
            }

            IList<Annotation> annotations = service.Store.GetAnnotations();
            List<IAnchorInfo> anchorinfos = new List<IAnchorInfo>(); 

            //Get IAnchorInfos and ensure that they are not null
            foreach (Annotation annotation in annotations)
            {
                IAnchorInfo anchorinfo = AnnotationHelper.GetAnchorInfo(service, annotation);
                AssertNotNull("IAnchorInfo is null.", anchorinfo);
                anchorinfos.Add(anchorinfo);
            }

            //Ensure ResolvedAnchor's for each IAnchorInfo is a TextAnchor
            //Go to page with annotation
            //verify page has attached annotation.
            foreach (IAnchorInfo ainfo  in anchorinfos)
            {
                Assert("ResolvedAnchor was not a TextAnchor.", AnnotationTestHelper.IsTextAnchor(ainfo.ResolvedAnchor));
                TextAnchor anchor = ainfo.ResolvedAnchor as TextAnchor;
                int page = ((DynamicDocumentPaginator)source.DocumentPaginator).GetPageNumber(anchor.BoundingStart);
                GoToPage(page);
                DispatcherHelper.DoEvents();
                VerifyNumberOfAttachedAnnotations(1);
            }

            passTest("No Errors");
        }

        /// <summary>
        /// Load XML Store.
        /// Get IAnchorInfo for each Annotation in the store.
        /// Navigate to a TextRange [NavigationCommands.GoToPage], Validate the Anchor is now attached.
        /// </summary>
        [Priority(1)]
        protected void ianchor1_09()
        {
            IDocumentPaginatorSource source = this.DocViewerWrapper.Document;

            //Start AnnotationService
            AnnotationService service = (AnnotationService)Service.Delegate;

            //Load list of Annotations
            if (ContentMode == TestMode.Fixed)
            {
                IAnchorInfoHelper.ImportAnnotationsStore("Fixed_Annotations.xml", service);
            }
            else
            {
                IAnchorInfoHelper.ImportAnnotationsStore("Flow_Annotations.xml", service);
            }

            IList<Annotation> annotations = service.Store.GetAnnotations();
            List<IAnchorInfo> anchorinfos = new List<IAnchorInfo>();

            //Get IAnchorInfos and ensure that they are not null
            foreach (Annotation annotation in annotations)
            {
                IAnchorInfo anchorinfo = AnnotationHelper.GetAnchorInfo(service, annotation);
                AssertNotNull("IAnchorInfo is null.", anchorinfo);
                anchorinfos.Add(anchorinfo);
            }

            //Ensure ResolvedAnchor's for each IAnchorInfo is a TextAnchor
            //Go to page with annotation
            //verify page has attached annotation.
            foreach (IAnchorInfo ainfo in anchorinfos)
            {
                
                Assert("ResolvedAnchor was not a TextAnchor.", AnnotationTestHelper.IsTextAnchor(ainfo.ResolvedAnchor));
                TextAnchor anchor = ainfo.ResolvedAnchor as TextAnchor;
                int page = ((DynamicDocumentPaginator)source.DocumentPaginator).GetPageNumber(anchor.BoundingStart);
                // Go to that page
                NavigationCommands.GoToPage.Execute(page + 1, this.ViewerBase);
                DispatcherHelper.DoEvents();
                VerifyNumberOfAttachedAnnotations(1);
            }

            passTest("No Errors");
        }

        #endregion
    }
}

