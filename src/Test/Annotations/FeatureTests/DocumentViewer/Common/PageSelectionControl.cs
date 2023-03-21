// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Reflection;
using System.Collections;
using System.Windows.Annotations;
using Annotations.Test.Reflection;
using System.Windows.Controls.Primitives;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Control bar that allows you to set the selection of a DocumentViewer using page, start offset, and length.
	/// The control shows you what the selection would be and allows you to test making annotations (if AnnotationService
	/// is enabled).
	/// </summary>
	class PageSelectionControl
	{
		public PageSelectionControl(ADocumentViewerBaseWrapper wrapper)
		{
			_dvController = wrapper;
			_dv = wrapper.ViewerBase;
		}

        private ColumnDefinition CreateColumn(int star)
        {
            ColumnDefinition col = new ColumnDefinition();
            col.Width = new GridLength(star, GridUnitType.Star);
            return col;
        }

        public UIElement Create()
		{
			Grid grid = new Grid();
            grid.ColumnDefinitions.Add(CreateColumn(1));
            grid.ColumnDefinitions.Add(CreateColumn(1));
            grid.ColumnDefinitions.Add(CreateColumn(1));
            grid.ColumnDefinitions.Add(CreateColumn(5));
            grid.ColumnDefinitions.Add(CreateColumn(1));
            grid.ColumnDefinitions.Add(CreateColumn(1));
            grid.ColumnDefinitions.Add(CreateColumn(1));
            grid.ColumnDefinitions.Add(CreateColumn(1));

            pageBox = new TextBox();
			pageBox.Text = "0";
			Grid.SetColumn(pageBox, 0);
			pageBox.TextChanged += MakeSelection;

			startBox = new TextBox();
			startBox.Text = "10";
			Grid.SetColumn(startBox, 1);
			startBox.TextChanged += MakeSelection;

			lengthBox = new TextBox();
			lengthBox.Text = "10";
			Grid.SetColumn(lengthBox, 2);
			lengthBox.TextChanged += MakeSelection;

			resultBox = new TextBox();
			resultBox.IsReadOnly = true;
			Grid.SetColumn(resultBox, 3);

			Button snButton = new Button();
			snButton.Content = "TextSN";
			snButton.Click += DoSN;
			Grid.SetColumn(snButton, 4);
			grid.Children.Add(snButton);

			Button inkSnButton = new Button();
			inkSnButton .Content = "InkSN";
			inkSnButton .Click += DoInkSN;
			Grid.SetColumn(inkSnButton , 5);
			grid.Children.Add(inkSnButton);

            Button highlightButton = new Button();
            highlightButton.Content = "Highlight";
            highlightButton.Click += DoHighlight;
            Grid.SetColumn(highlightButton, 6);
            grid.Children.Add(highlightButton);

            Button deleteButton = new Button();
            deleteButton.Content = "Delete";
            deleteButton.Click += DoDelete;
            Grid.SetColumn(deleteButton, 7);
            grid.Children.Add(deleteButton);

            grid.Children.Add(pageBox);
			grid.Children.Add(startBox);
			grid.Children.Add(lengthBox);
			grid.Children.Add(resultBox);

			DockPanel panel = new DockPanel();
			panel.Children.Add(grid);
			DockPanel.SetDock(panel, Dock.Top);

			return panel;
		}

		private void DoSN(object sender, EventArgs args)
		{
			if (Service.IsEnabled)
				AnnotationHelper.CreateTextStickyNoteForSelection(Service, "Text S N Author");
			else
				resultBox.Text = "Sorry, you need to setup the AnnotationService first.";
		}

		private void DoInkSN(object sender, EventArgs args)
		{
			if (Service.IsEnabled)
				AnnotationHelper.CreateInkStickyNoteForSelection(Service, "Ink S N Author");
			else
				resultBox.Text = "Sorry, you need to setup the AnnotationService first.";
		}

        private void DoHighlight(object sender, EventArgs args)
        {
			if (Service.IsEnabled)
				// 
                AnnotationHelper.CreateHighlightForSelection(Service, "H I Light Author", null); // Brush is null for the moment
            else
                resultBox.Text = "Sorry, you need to setup the AnnotationService first.";
        }

        private void DoDelete(object sender, EventArgs args)
        {
			if (Service.IsEnabled)
			{
				AnnotationHelper.ClearHighlightsForSelection(Service);
				AnnotationHelper.DeleteTextStickyNotesForSelection(Service);
				AnnotationHelper.DeleteInkStickyNotesForSelection(Service);
			}
			else
				resultBox.Text = "Sorry, you need to setup the AnnotationService first.";
        }

        public void MakeSelection(object sender, RoutedEventArgs args)
		{
			string text = "";
			try
			{							
				int page = Int32.Parse(pageBox.Text);
				int length = Int32.Parse(lengthBox.Text);
				if (page < 0 || page > _dv.PageCount)
					throw new ArgumentException("page");

				else if (startBox.Text.Equals("start"))
				{
					text = _dvController.SetSelection(page, PagePosition.Beginning, length).Text;
				}
				else if (startBox.Text.Equals("end"))
				{
					text = _dvController.SetSelection(page, PagePosition.End, length).Text;
				}
				else
				{
					int startIdx = Int32.Parse(startBox.Text);
					text = _dvController.SetSelection(page, startIdx, length).Text;
				}

				resultBox.Text = text.Replace("\r\n", "\\r\\n");
			}
			catch (Exception e)
			{
				resultBox.Text = e.ToString();
			}
		}

		// Property to find and then cache the AnnotationService so that we don't rely on its creation before this class
		private AnnotationService _service = null;

		protected AnnotationService Service
		{
			get
			{
				if (_service == null)
				{
					_service = AnnotationService.GetService(_dv);
				}
				return _service;
			}
		}

		TextBox pageBox;
		TextBox startBox;
		TextBox lengthBox;
		TextBox resultBox;
		DocumentViewerBase _dv;
        ADocumentViewerBaseWrapper _dvController;
	}
}

