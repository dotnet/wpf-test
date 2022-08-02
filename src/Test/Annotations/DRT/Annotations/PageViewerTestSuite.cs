// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

        
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Annotations;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Navigation;
    using System.Xml;
    
    using DRT;
    
    namespace DrtAnnotations
    {
        public class PageViewerTestSuite : DrtTestSuite
        {
            public PageViewerTestSuite() : base("PageViewer")
            {
                TeamContact = "WPF";
                Contact = "Microsoft";
        
                s_ITextViewType = s_serviceType.Assembly.GetType("System.Windows.Documents.ITextView");
                s_ITextPointerType = s_serviceType.Assembly.GetType("System.Windows.Documents.ITextPointer");
                Type TextEditorType = s_serviceType.Assembly.GetType("System.Windows.Documents.TextEditor");

    
                s_textSelectionMethod = TextEditorType.GetMethod("GetTextSelection", BindingFlags.NonPublic | BindingFlags.Static);
                s_getServiceMethod = s_serviceType.GetMethod("GetService", BindingFlags.NonPublic | BindingFlags.Static);
                s_getAttachedAnnotationsMethod = s_serviceType.GetMethod("GetAttachedAnnotations", BindingFlags.NonPublic | BindingFlags.Instance);
                s_textViewTextSegmentsProperty = s_ITextViewType.GetProperty("TextSegments");
                s_textSegmentStartProperty = s_serviceType.Assembly.GetType("System.Windows.Documents.TextSegment").GetProperty("Start", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ITextPointerCreatePointerMethod = s_serviceType.Assembly.GetType("System.Windows.Documents.ITextPointer").GetMethod("CreatePointer", new Type[] { typeof(int) });
                s_ITextRangeSelectMethod = s_serviceType.Assembly.GetType("System.Windows.Documents.ITextRange").GetMethod("Select", new Type[] { s_ITextPointerType, s_ITextPointerType });
            }


            public override DrtTest[] PrepareTests()
            {
                _window = new PageViewerApp();
                _window.Interactive = ((DrtAnnotationsDrt)DRT).Interactive;
                _window.InitializeComponent();
                _window.Visibility = Visibility.Visible;

                if (!((DrtAnnotationsDrt)DRT).UseExistingStores)
                {
                    if (File.Exists(PageViewerApp.FixedContent1Store))
                        File.Delete(PageViewerApp.FixedContent1Store);
                    if (File.Exists(PageViewerApp.FlowContent1Store))
                        File.Delete(PageViewerApp.FlowContent1Store);
                    if (File.Exists(PageViewerApp.FixedDocSequenceStore))
                        File.Delete(PageViewerApp.FixedDocSequenceStore);
                }

                if (((DrtAnnotationsDrt)DRT).Interactive)
                {
                    _window.LoadFlowDocumentPageViewer(null, null);

                    return new DrtTest[0];
                }
                else
                {
                    return GetTestArray();
                }
            }
  
            protected virtual DrtTest[] GetTestArray()
            {
                // Return the lists of tests to run against the tree
                return new DrtTest[]{
                            delegate() { LoadFixedDocument(); },          // 0 and 1 visible
                            delegate() { CreateStickyNoteAndVerify(); },  // create note on 0
                            delegate() { PageDown(); },                   // 1 and 2 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageUp(); },                     // 0 and 1 visible
                            delegate() { VerifyAnnotationsLoaded(); }, 
                            delegate() { PageDown(); },                   // 1 and 2 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 2 and 3 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 3 and 4 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { CreateStickyNoteAndVerify(); },  // create note on 3
                            delegate() { CreateHighlightAndVerify(); },   // create highlight on 3
                            delegate() { PageUp(); },                     // 2 and 3 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageUp(); },                     // 1 and 2 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageUp(); },                     // 0 and 1 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 1 and 2 visible
                            delegate() { CreateMultiPageStickyNoteAndVerify(); }, // create note on pages 1,2,3
                            delegate() { PageDown(); },                   // 2 and 3 visible 
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 3 and 4 visible 
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 4 and 5 visible 
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageUp(); },                     // 3 and 4 visible 
                            delegate() { PageUp(); },                     // 2 and 3 visible 
                            delegate() { PageUp(); },                     // 1 and 2 visible 
                            delegate() { PageUp(); },                     // 0 and 1 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 1 and 2 visible
                            delegate() { ClearContent(); },
                            delegate() { LoadFixedDocument(); },          // 0 and 1 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { CreateHighlightAndVerify(); },   // create highlight on 0
                            delegate() { PageDown(); },                   // 1 and 2 visible
                            delegate() { VerifyAnnotationsLoaded(); },  
                            delegate() { PageDown(); },                   // 2 and 3 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 3 and 4 visible
                            delegate() { VerifyAnnotationsLoaded(); },   
                            delegate() { PageDown(); },                   // 4 and 5 visible
                            delegate() { VerifyAnnotationsLoaded(); },  
                            delegate() { PageUp(); },                     // 3 and 4 visible
                            delegate() { PageUp(); },                     // 2 and 3 visible
                            delegate() { PageUp(); },                     // 1 and 2 visible
                            delegate() { PageUp(); },                     // 0 and 1 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { ClearContent(); },
                            delegate() { Reset(); },
                            delegate() { DeleteStore(); },

                            // FLOW CONTENT TESTS START HERE
                            delegate() { LoadFlowDocument(); },           // 0 and 1 visible            
                            delegate() { CreateStickyNoteAndVerify(); },  // create on 0
                            delegate() { PageDown(); },                   // 1 and 2 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageUp(); },                     // 0 and 1 visible
                            delegate() { VerifyAnnotationsLoaded(); }, 
                            delegate() { PageDown(); },                   // 1 and 2 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 2 and 3 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 3 and 4 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { CreateStickyNoteAndVerify(); },  // create on 3
                            delegate() { CreateHighlightAndVerify(); },   // create highlight on 3
                            delegate() { PageUp(); },                     // 2 and 3 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageUp(); },                     // 1 and 2 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageUp(); },                     // 0 and 1 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 1 and 2 visible
                            delegate() { CreateMultiPageStickyNoteAndVerify(); }, // create note on pages 1,2,3
                            delegate() { PageDown(); },                   // 2 and 3 visible 
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 3 and 4 visible 
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 4 and 5 visible 
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageUp(); },                     // 3 and 4 visible 
                            delegate() { PageUp(); },                     // 2 and 3 visible 
                            delegate() { PageUp(); },                     // 1 and 2 visible 
                            delegate() { PageUp(); },                     // 0 and 1 visible 
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { ClearContent(); },
                            delegate() { LoadFlowDocument(); },           // 0 and 1 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { CreateHighlightAndVerify(); },   // create highlight on 0
                            delegate() { PageDown(); },                   // 1 and 2 visible
                            delegate() { VerifyAnnotationsLoaded(); },  
                            delegate() { PageDown(); },                   // 2 and 3 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { PageDown(); },                   // 3 and 4 visible
                            delegate() { VerifyAnnotationsLoaded(); },   
                            delegate() { PageDown(); },                   // 4 and 5 visible
                            delegate() { VerifyAnnotationsLoaded(); },  
                            delegate() { PageUp(); },                     // 3 and 4 visible
                            delegate() { PageUp(); },                     // 2 and 3 visible
                            delegate() { PageUp(); },                     // 1 and 2 visible
                            delegate() { PageUp(); },                     // 0 and 1 visible
                            delegate() { VerifyAnnotationsLoaded(); },
                            delegate() { ClearContent(); },
                            delegate() { CloseWindow(); },
                            };
            }

            protected void CloseWindow()
            {
                _window.Close();
            }

            protected void LoadFlowDocument()
            {
                Console.WriteLine("Loading Flow Content in FlowDocumentPageViewer."); 
                _window.LoadFlowDocumentPageViewer(null, null);
                DRT.Pause(1000);
                _window.EnableAnnotations(null, null);
            }
   
            protected void LoadFixedDocument()
            {
                Console.WriteLine("Loading Fixed Content in DocumentViewer.");
                _window.LoadFixedDocument(null, null);
                _window.EnableAnnotations(null, null);
            }
    
            protected void ClearContent()
            {
                Console.WriteLine("Clearing Content."); 
                _window.ClearContent(null, null);
            }
    
            protected void Reset()
            {
                Console.WriteLine("Resetting counters"); 
                _pagesWithAnnotations = new int[20];
            }

            protected void DeleteStore()
            {
                Console.WriteLine("Deleting Store");                
                _window.DeleteStore();
            }
        
            protected void CreateStickyNoteAndVerify()
            {
                Console.WriteLine("Creating StickyNote on page: " + _window.Viewer.PageViews[0].PageNumber);
                SetSelectionOnPage(_window.Viewer.PageViews[0].PageNumber);
                _window.CreateTextStickyNote(null, null);
                _pagesWithAnnotations[_window.Viewer.PageViews[0].PageNumber]++;
                VerifyAnnotationsLoaded();
            }
    
            protected void CreateHighlightAndVerify()
            {
                Console.WriteLine("Creating Highlight on page: " + _window.Viewer.PageViews[0].PageNumber);
                SetSelectionOnPage(_window.Viewer.PageViews[0].PageNumber);
                _window.CreateHighlight(null, null);
                _pagesWithAnnotations[_window.Viewer.PageViews[0].PageNumber]++;
                VerifyAnnotationsLoaded();
            }

            protected void CreateMultiPageStickyNoteAndVerify()
            {
                Console.WriteLine("Creating three-page StickyNote starting on page: " + _window.Viewer.PageViews[0].PageNumber );
                SetThreePageSelectionOnPage(_window.Viewer.PageViews[0].PageNumber);
                _window.CreateTextStickyNote(null, null);
                _pagesWithAnnotations[_window.Viewer.PageViews[0].PageNumber]++;
                _pagesWithAnnotations[_window.Viewer.PageViews[0].PageNumber + 1]++;
                _pagesWithAnnotations[_window.Viewer.PageViews[0].PageNumber + 2]++;
                VerifyAnnotationsLoaded();
            }

            protected void PageDown()
            {
                Console.WriteLine("Paging down.");    
                _window.Viewer.NextPage();
            }
    
            protected void PageUp()
            {
                Console.WriteLine("Paging up.");    
                _window.Viewer.PreviousPage();
            }
    
            protected void VerifyAnnotationsLoaded()
            {
                int count = 0;
                int firstPage = _window.Viewer.PageViews[0].PageNumber;
                int lastPage = _window.Viewer.PageViews[_window.Viewer.PageViews.Count - 1].PageNumber;
                for(int i = firstPage; i <= lastPage; i++)
                {
                    count += _pagesWithAnnotations[i];
                }

                Console.WriteLine("Verifying " + count + " annotations on Pages " + firstPage + " through " + lastPage);
    
                AnnotationService service = s_getServiceMethod.Invoke(s_serviceType, new object[] { _window.Viewer }) as AnnotationService;
                IList annotations = s_getAttachedAnnotationsMethod.Invoke(service, new object[0]) as IList;
                if (annotations.Count != count)
                    throw new ApplicationException("Loaded annotation count does not match.  Expected " + count + " but found " + annotations.Count + ".");
            }
    
            private void SetSelectionOnPage(int pageNumber)
            {
                DocumentPage page = _window.Viewer.Document.DocumentPaginator.GetPage(pageNumber);
                object textView = ((IServiceProvider)page).GetService(s_ITextViewType); 
                TextSelection selection = s_textSelectionMethod.Invoke(null, new object[] { _window.Viewer }) as TextSelection;
    
                IList textSegmentCollection = s_textViewTextSegmentsProperty.GetValue(textView, new object[] {}) as IList;
                object pagePointer = s_textSegmentStartProperty.GetValue(textSegmentCollection[0], new object[] {});
                object newStartPointer = s_ITextPointerCreatePointerMethod.Invoke(pagePointer, new object[] { 20 });
                object newEndPointer = s_ITextPointerCreatePointerMethod.Invoke(newStartPointer, new object[] { 39 });            
                s_ITextRangeSelectMethod.Invoke(selection, new object[] { newStartPointer, newEndPointer});
            }

            private void SetThreePageSelectionOnPage(int pageNumber)
            {
                DocumentPage page = _window.Viewer.Document.DocumentPaginator.GetPage(pageNumber);                
                object textView = ((IServiceProvider)page).GetService(s_ITextViewType);
                TextSelection selection = s_textSelectionMethod.Invoke(null, new object[] { _window.Viewer }) as TextSelection;

                IList textSegmentCollection = s_textViewTextSegmentsProperty.GetValue(textView, new object[] { }) as IList;
                object pagePointer = s_textSegmentStartProperty.GetValue(textSegmentCollection[0], new object[] { });
                object newStartPointer = s_ITextPointerCreatePointerMethod.Invoke(pagePointer, new object[] { 45 });

                page = _window.Viewer.Document.DocumentPaginator.GetPage(pageNumber + 2);
                textView = ((IServiceProvider)page).GetService(s_ITextViewType);

                textSegmentCollection = s_textViewTextSegmentsProperty.GetValue(textView, new object[] { }) as IList;
                pagePointer = s_textSegmentStartProperty.GetValue(textSegmentCollection[0], new object[] { });
                object newEndPointer = s_ITextPointerCreatePointerMethod.Invoke(pagePointer, new object[] { 48 });

                s_ITextRangeSelectMethod.Invoke(selection, new object[] { newStartPointer, newEndPointer });
            }

            #region Private Fields
    
            static private Type s_serviceType = typeof(AnnotationService);
            static private Type s_ITextViewType;
            static private Type s_ITextPointerType;
            static private MethodInfo s_textSelectionMethod;
            static private MethodInfo s_getServiceMethod;
            static private MethodInfo s_getAttachedAnnotationsMethod;
            static private PropertyInfo s_textViewTextSegmentsProperty;
            static private PropertyInfo s_textSegmentStartProperty;
            static private MethodInfo s_ITextPointerCreatePointerMethod;
            static private MethodInfo s_ITextRangeSelectMethod;
    
            private int[] _pagesWithAnnotations = new int[20];
            private PageViewerApp _window;
    
            #endregion Private Fields
        }    
    }
