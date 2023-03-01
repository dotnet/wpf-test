// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Xml;
using System.Windows.Controls.Primitives;
using System.Windows.Annotations;

namespace Avalon.Test.Annotations
{
	enum ContentType
	{
		Fixed,
		Flow
	}

	/// <summary>
	/// A Navigation window that contains a DocumentViewer.
	/// </summary>
	class DocumentViewerTree
	{
		#region Public

		public DocumentViewerTree()
		{
			Initialize();
		}

		public DocumentViewerTree(ContentType contentType)
		{
			Initialize();
			SetContent(contentType);
		}

		public void SetDocumentViewerContent(IDocumentPaginatorSource content)
		{
			_documentViewer.Document = content;
		}

		public void SetContent(ContentType contentType)
		{
			if (contentType == ContentType.Flow)
				SetContentToFlow();
			else if (contentType == ContentType.Fixed)
				SetContentToFixed();
			else
				throw new NotImplementedException(contentType.ToString());
		}

		public void Show()
		{
			_navWindow.Show();
		}

		public DocumentViewerBase DocumentViewer
		{
			get { return _documentViewer; }
		}

		public ContentType ContentType
		{
			get { return _contentType; }
		}	

		#endregion Public

		#region Private

		private void Initialize()
		{
			_navWindow = new Window();
			_navWindow.Name = "window";
			_navWindow.Title = "DocumentViewer Tree";

			_mainContent = new DockPanel();
			_mainContent.Name = "mainContent";

			// Create PageViewer.
            _documentViewer = new FlowDocumentPageViewer();
			_documentViewer.Name = "documentViewer";

			// Set content but don't show it.
			_mainContent.Children.Add(_documentViewer);
			_navWindow.Content = (_mainContent);

			// Create AnnotationService for DocumentViewer.
			new AnnotationService(_documentViewer);
		}

		private void SetContentToFixed()
		{
			_contentType = ContentType.Fixed;
			throw new NotImplementedException();			
		}

		private void SetContentToFlow()
		{
            FlowDocument flow = new FlowDocument( new Paragraph(new Run("Most young people in the United States go to school for 12 or 13 years.  " +
				"They begin in Kindergarten and graduate from high school once they complete the twelfth grade.  " +
				"Some students may have to repeat some grades and others may have the opportunity to skip one or more grades.  " +
				"These decisions are usually based on performance or tests.  In the past a child's age would also be taken into account.  " +
				"There is a significant number of adults that take the G.D.E. exam.  Passing this exam is supposed to represent that the test taker has equivalent knowledge as a student who finished the normal number of years in school.  " +
				"Most young people in the United States go to school for 12 or 13 years.  They begin in Kindergarten and graduate from high school once they complete the twelfth grade." + 
			    
                "Most young people in the United States go to school for 12 or 13 years.  " +
				"They begin in Kindergarten and graduate from high school once they complete the twelfth grade.  " +
				"Some students may have to repeat some grades and others may have the opportunity to skip one or more grades.  " +
				"These decisions are usually based on performance or tests.  In the past a child's age would also be taken into account.  " +
				"There is a significant number of adults that take the G.D.E. exam.  Passing this exam is supposed to represent that the test taker has equivalent knowledge as a student who finished the normal number of years in school.  " +
				"Most young people in the United States go to school for 12 or 13 years.  They begin in Kindergarten and graduate from high school once they complete the twelfth grade." +
			
                "Most young people in the United States go to school for 12 or 13 years.  " +
				"They begin in Kindergarten and graduate from high school once they complete the twelfth grade.  " +
				"Some students may have to repeat some grades and others may have the opportunity to skip one or more grades.  " +
				"These decisions are usually based on performance or tests.  In the past a child's age would also be taken into account.  " +
				"There is a significant number of adults that take the G.D.E. exam.  Passing this exam is supposed to represent that the test taker has equivalent knowledge as a student who finished the normal number of years in school.  " +
				"Most young people in the United States go to school for 12 or 13 years.  They begin in Kindergarten and graduate from high school once they complete the twelfth grade." +
			
                "Most young people in the United States go to school for 12 or 13 years.  " +
				"They begin in Kindergarten and graduate from high school once they complete the twelfth grade.  " +
				"Some students may have to repeat some grades and others may have the opportunity to skip one or more grades.  " +
				"These decisions are usually based on performance or tests.  In the past a child's age would also be taken into account.  " +
				"There is a significant number of adults that take the G.D.E. exam.  Passing this exam is supposed to represent that the test taker has equivalent knowledge as a student who finished the normal number of years in school.  " +
				"Most young people in the United States go to school for 12 or 13 years.  They begin in Kindergarten and graduate from high school once they complete the twelfth grade." + 

			    "Most young people in the United States go to school for 12 or 13 years.  " +
				"They begin in Kindergarten and graduate from high school once they complete the twelfth grade.  " +
				"Some students may have to repeat some grades and others may have the opportunity to skip one or more grades.  " +
				"These decisions are usually based on performance or tests.  In the past a child's age would also be taken into account.  " +
				"There is a significant number of adults that take the G.D.E. exam.  Passing this exam is supposed to represent that the test taker has equivalent knowledge as a student who finished the normal number of years in school.  " +
				"Most young people in the United States go to school for 12 or 13 years.  They begin in Kindergarten and graduate from high school once they complete the twelfth grade." +

			    "Most young people in the United States go to school for 12 or 13 years.  " +
				"They begin in Kindergarten and graduate from high school once they complete the twelfth grade.  " +
				"Some students may have to repeat some grades and others may have the opportunity to skip one or more grades.  " +
				"These decisions are usually based on performance or tests.  In the past a child's age would also be taken into account.  " +
				"There is a significant number of adults that take the G.D.E. exam.  Passing this exam is supposed to represent that the test taker has equivalent knowledge as a student who finished the normal number of years in school.  " +
				"Most young people in the United States go to school for 12 or 13 years.  They begin in Kindergarten and graduate from high school once they complete the twelfth grade." + 

			    "Most young people in the United States go to school for 12 or 13 years.  " +
				"They begin in Kindergarten and graduate from high school once they complete the twelfth grade.  " +
				"Some students may have to repeat some grades and others may have the opportunity to skip one or more grades.  " +
				"These decisions are usually based on performance or tests.  In the past a child's age would also be taken into account.  " +
				"There is a significant number of adults that take the G.D.E. exam.  Passing this exam is supposed to represent that the test taker has equivalent knowledge as a student who finished the normal number of years in school.  " +
				"Most young people in the United States go to school for 12 or 13 years.  They begin in Kindergarten and graduate from high school once they complete the twelfth grade." + 

			    "Most young people in the United States go to school for 12 or 13 years.  " +
				"They begin in Kindergarten and graduate from high school once they complete the twelfth grade.  " +
				"Some students may have to repeat some grades and others may have the opportunity to skip one or more grades.  " +
				"These decisions are usually based on performance or tests.  In the past a child's age would also be taken into account.  " +
				"There is a significant number of adults that take the G.D.E. exam.  Passing this exam is supposed to represent that the test taker has equivalent knowledge as a student who finished the normal number of years in school.  " +
				"Most young people in the United States go to school for 12 or 13 years.  They begin in Kindergarten and graduate from high school once they complete the twelfth grade." +

			    "Most young people in the United States go to school for 12 or 13 years.  " +
				"They begin in Kindergarten and graduate from high school once they complete the twelfth grade.  " +
				"Some students may have to repeat some grades and others may have the opportunity to skip one or more grades.  " +
				"These decisions are usually based on performance or tests.  In the past a child's age would also be taken into account.  " +
				"There is a significant number of adults that take the G.D.E. exam.  Passing this exam is supposed to represent that the test taker has equivalent knowledge as a student who finished the normal number of years in school.  " +
				"Most young people in the United States go to school for 12 or 13 years.  They begin in Kindergarten and graduate from high school once they complete the twelfth grade." )));
           
            _documentViewer.Document = flow;
			_contentType = ContentType.Flow;
		}

		/// <summary>
		/// Method provided as sample by PageViewer folks (Source property has been removed)
		/// </summary>
		/// <param name="fileName">Uri name of file to load into PageViewer</param>
		/// <returns>XAML cast into IDocumentFormatter</returns>
		private IDocumentPaginatorSource LoadXaml(string fileName)
		{
			FileStream fs = new FileStream(fileName, FileMode.Open);
            ParserContext pc = new ParserContext();
            pc.BaseUri = new Uri("pack://siteoforigin:,,,/");
			object fsXaml = XamlReader.Load(fs, pc);

			if (fsXaml is IDocumentPaginatorSource)
				return (IDocumentPaginatorSource)fsXaml;
			else
				return null;
		}

		public Window _navWindow;
		public DockPanel _mainContent;
		public DocumentViewerBase _documentViewer;
		public ContentType _contentType;

		#endregion Private
	}
}

