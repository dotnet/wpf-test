// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using Office = Microsoft.Office.InteropLateBound.Office;
using CoreInteropLateBound;

namespace Microsoft.Office.InteropLateBound.Word
{
	#region enums
	public enum WdAlertLevel
	{
		wdAlertsAll = -1,
		wdAlertsMessageBox = -2,
		wdAlertsNone = 0
	}

	public enum WdSaveOptions
	{
		wdDoNotSaveChanges = 0, 
		wdPromptToSaveChanges = -2,
		wdSaveChanges = -1
	}

	public enum WdSaveFormat
	{
        wdFormatDocument = 0,
        wdFormatDocument97 = 0,
        wdFormatDocumentDefault = 16,
        wdFormatDOSText = 4,
        wdFormatDOSTextLineBreaks = 5,
        wdFormatEncodedText = 7,
        wdFormatFilteredHTML = 10,
        wdFormatHTML = 8,
        wdFormatRTF = 6,
        wdFormatTemplate = 1,
        wdFormatTemplate97 = 1,
        wdFormatText = 2,
        wdFormatTextLineBreaks = 3,
        wdFormatUnicodeText = 7,
        wdFormatWebArchive = 9,
        wdFormatXML = 11,
        wdFormatXMLDocument = 12,
        wdFormatXMLDocumentMacroEnabled = 13,
        wdFormatXMLTemplate = 14,
        wdFormatXMLTemplateMacroEnabled = 15
	}
	#endregion
	
	/// <summary>
	/// Wrapper for Word
	/// </summary>
	public class Application : ApplicationCommon  
	{
		public Application()
		{
			StartOfficeApp(EOfficeAppName.Word);					
		}
		#region Methods

		public void Quit(WdSaveOptions SaveChanges)
		{
			InvokeMethod(m_OfficeApp, "Quit", new object[] {SaveChanges});				
		}
	
		#endregion
	
		#region Properties
		public object WordBasic
		{
			get
			{
				return GetProperty(m_OfficeApp, "WordBasic");
			}
		}

		public bool ScreenUpdating
		{
			set
			{
				SetProperty(m_OfficeApp, "ScreenUpdating", value);
			}

			get
			{
				return Convert.ToBoolean(GetProperty(m_OfficeApp, "ScreenUpdating"));
			}
		}	

		public WdAlertLevel DisplayAlerts
		{
			get
			{
				return (WdAlertLevel)Enum.Parse(typeof(WdAlertLevel), GetProperty(m_OfficeApp, "DisplayAlerts").ToString());
			}

			set
			{
				SetProperty(m_OfficeApp, "DisplayAlerts", new object[] {value});
			}
		}		

		public Documents Documents
		{
			get
			{
				return new Documents(this);
			}
		}

		public Document ActiveDocument
		{
			get
			{
				return new Document(GetProperty(m_OfficeApp, "ActiveDocument"), this);
			}
		}
		#endregion
	}
		
	#region Documents
	public class Documents : ApplicationChildObject
	{
		private object _oDocs;

		public override object GetUnderlyingObject()
		{
			return _oDocs;
		}

		public Documents(Application app) : base(app)
		{	
			_oDocs = GetProperty(app.GetUnderlyingObject(), "Documents");
		}

		public Document Add()
		{
			object oDoc = InvokeMethod(_oDocs, "Add", null);
			return new Document(oDoc, this.Application);
		}

		public int Count
		{
			get
			{
				return (int)GetProperty(_oDocs, "Count");
			}
		}

		/// <summary>
		/// Opens the specified document and adds it to the Documents collection. Returns a Document object.
		/// </summary>
		/// <param name="FileName">The name of the document (paths are accepted).</param>
		/// <param name="ConfirmConversions">True to display the Convert File dialog box if the file isn't in Microsoft Word format.</param>
		/// <param name="ReadOnly">True to open the document as read-only. Note: This argument doesn't override the read-only recommended setting on a saved document. For example, if a document has been saved with read-only recommended turned on, setting the ReadOnly argument to False will not cause the file to be opened as read/write.</param>
		/// <param name="AddToRecentFiles">True to add the file name to the list of recently used files at the bottom of the File menu.</param>
		/// <param name="PasswordDocument">The password for opening the document.</param>
		/// <param name="PasswordTemplate">The password for opening the template.</param>
		/// <param name="Revert">Controls what happens if FileName is the name of an open document. True to discard any unsaved changes to the open document and reopen the file. False to activate the open document.</param>
		/// <param name="WritePasswordDocument">The password for saving changes to the document.</param>
		/// <param name="WritePasswordTemplate">The password for saving changes to the template.</param>
		/// <param name="Format">The file converter to be used to open the document. A WdOpenFormat constant.</param>
		/// <param name="Encoding">The document encoding (code page or character set) to be used by Microsoft Word when you view the saved document. Can be any valid MsoEncoding constant.</param>
		/// <param name="Visible">True if the document is opened in a visible window. The default value is True.</param>
		public Document Open(object FileName, object ConfirmConversions, object ReadOnly, object AddToRecentFiles, object PasswordDocument, 
			object PasswordTemplate, object Revert, object WritePasswordDocument, object WritePasswordTemplate, object Format,
			object Encoding, object Visible)
		{							
			object oWdDoc = InvokeMethod(_oDocs, "Open", 
				new object[]{
								FileName, /* FileName */
								ConfirmConversions, /* [Confirm Conversions] */
								ReadOnly, /* [ReadOnly] */
								AddToRecentFiles, /* [AddToRecentFiles] */
								PasswordDocument, /* [PasswordDocument] */ 
								PasswordTemplate, /* [PasswordTemplate] */
								Revert, /* [Revert] */
								WritePasswordDocument, /* [WritePasswordDocument] */
								WritePasswordTemplate, /* [WritePasswordTemplate] */
								Format, /* [Format] */
								Encoding, /* [Encoding] */							
								Visible /* [Visible] */											
							}
				);

			return new Document(oWdDoc, this.Application);
		}

		public Document Open(string FileName, bool ConfirmConversions, bool ReadOnly, bool AddToRecentFiles, string PasswordDocument, string PasswordTemplate, bool Visible)
		{						
			return Open(FileName, ConfirmConversions, ReadOnly, AddToRecentFiles, PasswordDocument, PasswordTemplate, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Visible);
		}
		
		public Document Open(string FileName)
		{	
			return Open(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
		}
	
		public Document Item(int Index)
		{
			object oDoc = InvokeMethod(_oDocs, "Item", new object[]{Index});
			return new Document(oDoc, this.Application);
		}
	}

	#endregion

	#region Document
	public class Document : ApplicationChildObject
	{
		private object _oWdDoc;

		public override object GetUnderlyingObject()
		{
			return _oWdDoc;
		}

		public Document(object WordDocumentObj, Application app) : base(app)
		{				
			_oWdDoc = WordDocumentObj;			
		}

		public string Name
		{
			get { return (string)GetProperty(_oWdDoc, "Name"); }
		}

		public string FullName
		{
			get { return (string)GetProperty(_oWdDoc, "FullName"); }
		}

		public void SaveAs(string FileName, int FileFormat)
		{
			InvokeMethod(_oWdDoc, "SaveAs", new object[] {FileName, FileFormat});
		}

		public void SaveAs(string FileName)
		{
			InvokeMethod(_oWdDoc, "SaveAs", new object[] {FileName});			
		}

		public void Close(object SaveChanges, object OriginalFormat, object RouteDocument)
		{
			InvokeMethod(_oWdDoc, "Close", new object[] {SaveChanges, OriginalFormat, RouteDocument});
		}

		public void Close(Word.WdSaveOptions SaveChanges)
		{
			Close(SaveChanges, Type.Missing, Type.Missing);
		}
	}
	#endregion

	#region Base class for Application child objects
	public abstract class ApplicationChildObject : CoreInteropLateBound.CoreLateBound
	{
		private Application _application;

		public ApplicationChildObject(Application app)
		{
			_application = app;			
		}

		public Application Application
		{
			get
			{
				return _application;
			}			
		}

		abstract public object GetUnderlyingObject();
	}
	#endregion
}
