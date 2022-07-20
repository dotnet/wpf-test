// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Resources;
using System.Windows.Markup;
using System.Xml;
using System.Reflection;
using System.Runtime.InteropServices;
using MS.Win32;

namespace DRT
{
    /// <summary>
    /// This is a test suite to test the visual style supports in StickyNoteControl
    /// 1. Style a StickyNoteControl in xaml.
    /// </summary>
    public class DrtStickyNoteControlTestSuite : DrtTabletTestSuite
    {
        // The values for typing test
        private bool                _isTextNote;
        private FileStream          _stream;
        private AnnotationService   _service;
        private FlowDocumentPageViewer    _docViewer;
        private StickyNoteControl   _snc;
        private StickyNoteControlProxy _sncProxy;

        static private Type ServiceType;
        static private Type ITextViewType;
        static private Type ITextPointerType;
        static private MethodInfo TextSelectionMethod;
        static private MethodInfo GetServiceMethod;
        static private PropertyInfo TextViewTextSegmentsProperty;
        static private MethodInfo GetAttachedAnnotationsMethod;
        static private PropertyInfo TextSegmentStartProperty;
        static private MethodInfo ITextPointerCreatePointerMethod;
        static private MethodInfo ITextRangeSelectMethod;


        private static string ContentStore = @"DrtFiles\\drtstickynotecontrol_store.xml";
        //private static string FixedContent = @"drtannotations_fixedcontent1.xaml";
        private const string FixedContent = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph FontSize=\"32pt\">DrtStickyNoteControl</Paragraph></FlowDocument>";

        static DrtStickyNoteControlTestSuite()
        {
            ServiceType = typeof(AnnotationService);

            ITextViewType = ServiceType.Assembly.GetType("System.Windows.Documents.ITextView");
            ITextPointerType = ServiceType.Assembly.GetType("System.Windows.Documents.ITextPointer");
            Type TextEditorType = ServiceType.Assembly.GetType("System.Windows.Documents.TextEditor");

            TextSelectionMethod = TextEditorType.GetMethod("GetTextSelection", BindingFlags.NonPublic | BindingFlags.Static);
            GetServiceMethod = ServiceType.GetMethod("GetService", BindingFlags.NonPublic | BindingFlags.Static);
            GetAttachedAnnotationsMethod = ServiceType.GetMethod("GetAttachedAnnotations", BindingFlags.NonPublic | BindingFlags.Instance);
            TextViewTextSegmentsProperty = ITextViewType.GetProperty("TextSegments");
            TextSegmentStartProperty = ServiceType.Assembly.GetType("System.Windows.Documents.TextSegment").GetProperty("Start", BindingFlags.NonPublic | BindingFlags.Instance);
            ITextPointerCreatePointerMethod = ServiceType.Assembly.GetType("System.Windows.Documents.ITextPointer").GetMethod("CreatePointer", new Type[] { typeof(int) });
            ITextRangeSelectMethod = ServiceType.Assembly.GetType("System.Windows.Documents.ITextRange").GetMethod("Select", new Type[] { ITextPointerType, ITextPointerType });
        }

        public DrtStickyNoteControlTestSuite(string drtName)
            : this(drtName, true)
        {
        }

        public DrtStickyNoteControlTestSuite(string drtName, bool isTextNote)
            : base(drtName)
        {
            _isTextNote = isTextNote;
        }

        public override DrtTest[] PrepareTests()
        {
            CreateTestPage();

            return new DrtTest[] { 
                        new DrtTest(CreateStickyNoteControl),
                        new DrtTest(IdentifyStickyNoteControl) };
        }

        public override void ReleaseResources()
        {
            _stream.Close();
            File.Delete(ContentStore);
        }

        protected StickyNoteControlProxy StickyNoteProxy
        {
            get
            {
                return _sncProxy;
            }
        }
            
        protected StickyNoteControl StickyNote
        {
            get
            {
                return _snc;
            }
        }

        #region Tests

        private void CreateTestPage()
        {
            // setup DocumentViewer
            _docViewer = new FlowDocumentPageViewer();

            // setup AnnotationService

            String filePath = Path.GetDirectoryName(ContentStore);
            if (!Directory.Exists(filePath)) 
            {
                // Create the directory it does not exist.
                Directory.CreateDirectory(filePath);
            }

            _stream = new FileStream(ContentStore, FileMode.OpenOrCreate, FileAccess.ReadWrite); 
            _service = new AnnotationService(_docViewer);
            _service.Enable(new XmlStreamStore(_stream));

            // Load fixed content into the Viewer
            // Set content on viewer
            using ( StringReader xamlReader = new StringReader(FixedContent) )
            {
                _docViewer.Document = (IDocumentPaginatorSource)XamlReader.Load(new XmlTextReader(xamlReader));
            }

            DRT.Show(_docViewer);
        }

        private void CreateStickyNoteControl()
        {
            DocumentPage page = _docViewer.Document.DocumentPaginator.GetPage(0);
            object textView = ( (IServiceProvider)page ).GetService(ITextViewType);
            TextSelection selection = TextSelectionMethod.Invoke(null, new object[] { _docViewer }) as TextSelection;

            IList textSegmentCollection = TextViewTextSegmentsProperty.GetValue(textView, new object[] { }) as IList;
            object startPointer = TextSegmentStartProperty.GetValue(textSegmentCollection[0], new object[] { });
            object newStartPointer = ITextPointerCreatePointerMethod.Invoke(startPointer, new object[] { 10 });
            object newEndPointer = ITextPointerCreatePointerMethod.Invoke(newStartPointer, new object[] { 10 });
            ITextRangeSelectMethod.Invoke(selection, new object[] { newStartPointer, newEndPointer });

            if ( _isTextNote )
            {
                AnnotationHelper.CreateTextStickyNoteForSelection(_service, null);
            }
            else
            {
                AnnotationHelper.CreateInkStickyNoteForSelection(_service, null);
            }
        }

        private void IdentifyStickyNoteControl()
        {
            _snc = (StickyNoteControl)DRT.FindVisualByType(typeof(StickyNoteControl), DRT.RootElement);
            _sncProxy = new StickyNoteControlProxy(_snc);
        }

        #endregion Tests

    }
}
