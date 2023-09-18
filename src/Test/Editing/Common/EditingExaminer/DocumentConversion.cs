// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 * Description: DocumentConversion.cs implement the immediate window with the following
 * features:
 *      1. Using winform Open dialog 
 *      2. using winform Save as dialog
 *      3. save/Open formats supported:
 *          *.Xaml, *.Doc, *.Xdoc, *.RTF, *.text, *HTML, XamlPackage(*.zip)
 *      4. Use MS word to Load/Save *.doc & html files.
 *
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Documents;
using System.Windows;
using System.Runtime.InteropServices;


namespace EditingExaminer
{
    class Documentconvertion
    {
        /// <summary>
        /// Load different type of files into RichTextBox. 
        /// </summary>
        /// <param name="richTextBox"></param>
        public static Exception LoadFileIntoRichTextBox(RichTextBox richTextBox, string fileName)
        {
            Exception error = null;
            //Get the full file name.
            if (fileName == null || fileName == string.Empty)
            {
                fileName = OpenFileDialog().ToLower();
            }
            
            if (!File.Exists(fileName))
            {
                return new Exception("File Does not exist!") ;      
            }

            try
            {
                if (fileName.EndsWith(".doc")
                    || fileName.EndsWith(".docx")
                    || fileName.EndsWith(".html")
                    || fileName.EndsWith(".htm")
                    || fileName.EndsWith(".mht"))
                {
                    //Load the word files and html files into a RichTextBox 
                    LoadDocIntoRichTextBox(richTextBox, fileName);
                }
                else if (fileName.EndsWith(".rtf"))
                {
                    //Load a RTF file int to a RichTextbox 
                    LoadRTFfileIntoRichTextBox(richTextBox, fileName);
                }
                else if (fileName.EndsWith(".zip"))
                {
                    //load xamlpackage into a RichTextBox
                    LoadXamlPackage(richTextBox, fileName);
                }
                else
                {
                    //Load a text file into RichTextBox
                    LoadTextIntoRichTextBox(richTextBox, fileName);
                }
            }
            catch (Exception e)
            {
                error = e;
            }

            return error; 
        }

        /// <summary>
        /// Save the RichTextBox content to a file.
        /// </summary>
        /// <param name="richTextBox"></param>
        public static Exception SaveRichTextBoxContentToFile(RichTextBox richTextBox, string fileName)
        {
            Exception error= null;

            if (fileName == null || fileName == string.Empty)
            {
                fileName = SaveFileDialog();
            }
            if (fileName == string.Empty || fileName == null)
            {
                return new Exception("File name can't be empty or null for save action!"); 
            }

            try
            {
                if (fileName.EndsWith(".doc")
                    || fileName.EndsWith(".docx")
                    || fileName.EndsWith(".html")
                    || fileName.EndsWith(".htm")
                    || fileName.EndsWith(".mht"))
                {
                    SaveRichTextBoxContentWithWord(richTextBox, fileName);
                }
                else if (fileName.EndsWith(".rtf"))
                {
                    //When we save RTF, we will not use word to do the convertion,
                    //instead, we will directly use the RTF converter. 
                    //So, we are still able to be successful in case word is not avaliable on a system.
                    SaveRTFFromRichTextBox(richTextBox, fileName);
                }
                else if (fileName.EndsWith(".txt"))
                {
                    //Save as plain text format
                    SaveTextFromRichTextBox(richTextBox, fileName);
                }
                else
                {
                    //Save as xamlpackage.
                    SaveXamlPackage(richTextBox, fileName);
                }
            }
            catch (Exception e)
            {
                error = e; 
            }

            return error; 
        }

        private static void SaveRichTextBoxContentWithWord(RichTextBox richTextBox, string fileName)
        {
            int format = -1000;
            SaveRTFFromRichTextBox(richTextBox, s_tempRtfFileName);
            object  wdoc = BootWordApp(s_tempRtfFileName);
            
            //We only save the word sensitive format through word.
            if(fileName.EndsWith(".doc"))
            {
                format = (int)WordFileFormats.wdFormatDocument;
            }
            else if (fileName.EndsWith(".html") || fileName.EndsWith(".htm") )
            {
                format = (int)WordFileFormats.wdFormatHTML;
            }
            else if(fileName.EndsWith(".mht"))
            {
                //Not sure which format if for singlepage html.
                format = (int)WordFileFormats.wdFormatHTML;
            }

            ReflectionUtils.InvokeInstanceMethod(wdoc, "SaveAs", new object[]{fileName, format});
            CloseWordDocument(wdoc);
        }

        
        private static void SaveXamlPackage(RichTextBox richTextBox, string fileName)
        {
            TextRange range;
            FileStream fStream;

            range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);                      

            fStream = new FileStream(fileName, FileMode.Create);
            range.Save(fStream, DataFormats.XamlPackage);
            fStream.Close();
        }

        private static void SaveRTFFromRichTextBox(RichTextBox richTextBox, string fileName)
        {
            string rtfContent;

            //Get the Rtf content from a RichTextBox 
            rtfContent = GetRtfContentFromRichTextBox(richTextBox);
            
            //Save the RTF content into a file.
            SaveStringToFile(fileName, rtfContent);
        }

        private static string GetRtfContentFromRichTextBox(RichTextBox richTextBox)
        {
            object[] objs, obj1;
            TextRange range; 
            MemoryStream stream;

            objs = new object[2];
            obj1 = new object[3];
            
            //we must create an object of the memorystream to pass in as ref.
            stream = new MemoryStream(); 

            //Create a range for WpfPayload.
            range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentStart);
    
            obj1[0] = range; 
            obj1[1] = stream; 
            obj1[2] = true;

            //Use reflection to get the xaml and xamlpackage for RTF converter
            Type type = ReflectionUtils.FindType("WpfPayload");
            string rtfContent = ReflectionUtils.InvokeStaticMethod(type, "SaveRange", obj1) as string;
            
            type = ReflectionUtils.FindType("TextEditorCopyPaste");
            objs[0] = rtfContent;

            //the steam must not be empty or null for image to be converted to Rtf
            objs[1] = stream; 

            //Perfrom Convertion.
            rtfContent = ReflectionUtils.InvokeStaticMethod(type, "ConvertXamlToRtf", objs) as string;

            return rtfContent; 
        }
        private static void SaveTextFromRichTextBox(RichTextBox richTextBox, string fileName)
        {
            TextRange range;

            range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            //Save the text of the range to a file. 
            SaveStringToFile(fileName, range.Text);
        }

        private static void SaveStringToFile(string fileName, string content)
        {
            FileStream fStream;
            StreamWriter sWriter;

            //Delete the existing file
            //Not the the winform save as dialog has already confirm the customer to over write the exsisting file.
            if(File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            //Open file
            fStream = new FileStream(fileName, FileMode.Create);
            
            //Create the streamwriter.
            sWriter = new StreamWriter(fStream);
            
            //Write the content into the file stream.
            sWriter.Write(content);

            //close the writer and the stream. 
            sWriter.Close();
            fStream.Close();
        }

        private static string SaveFileDialog()
        {
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();

            saveDialog.Filter = "RTF (*.rtf;)|*.rtf|Word Files(*.doc;*docx)|*.doc;*.docx|XAMLPackage (*.zip)|*.zip|HTML (*.html;*.htm)|*.html;*.htm|MHTML (*.mht)|*.mht|TextFile (*.txt)|*.txt|All files (*.*)|*.*";
            saveDialog.Title = "Save a file";

            System.Windows.Forms.DialogResult result = saveDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return saveDialog.FileName;
            }
            else
            {
                return string.Empty; 
            }
        }

        private static string OpenFileDialog()
        {
            System.Windows.Forms.OpenFileDialog openDialog = new System.Windows.Forms.OpenFileDialog();
            openDialog.Multiselect = false;
            //"Valid Files (*.rtf;*.Doc)|*.rtf;*.xaml|RTF files (*.rtf)|*.rtf|XAML files (*.xaml)|*.xaml|All files (*.*)|*.*";
            openDialog.Filter = "RTF (*.rtf;)|*.rtf|Word Files(*.doc;*docx)|*.doc;*.docx|XAMLPackage (*.zip)|*.zip|HTML (*.mht;*.html;*.htm)|*.mht;*.html;*.htm|TextFile (*.txt)|*.txt|All files (*.*)|*.*";
            openDialog.Title = "Open a file";

            System.Windows.Forms.DialogResult result = openDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return openDialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        private static void LoadTextIntoRichTextBox(RichTextBox richTextBox, string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                //how do we deal with the new lines?
                range.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        private static void LoadXamlPackage(RichTextBox richTextBox, string fileName)
        {
            TextRange range;
            FileStream fStream;

            if (File.Exists(fileName))
            {
                range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

                try
                {
                    fStream = new FileStream(fileName, FileMode.OpenOrCreate);
                    range.Load(fStream, DataFormats.XamlPackage);
                    fStream.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        private static void LoadDocIntoRichTextBox(RichTextBox richTextBox, string fileName)
        {

            if (File.Exists(s_tempRtfFileName))
            {
                File.Delete(s_tempRtfFileName);
            }

            object wdoc = BootWordApp(fileName);
            
            //6 is the enum value of RTF.
            //((Word.Document)wdoc).SaveAs(_tempRtfFileName, 6);
            object obj = ReflectionUtils.InvokeInstanceMethod(wdoc, "SaveAs", new object[] { s_tempRtfFileName, 6 });
            
            //We need to close the doc first before loading the RTR file to RichTextBox.
            CloseWordDocument(wdoc);
           
            LoadRTFfileIntoRichTextBox(richTextBox, s_tempRtfFileName);

            obj = null;
            //we don't need the temp file anymore, delete it.
            File.Delete(s_tempRtfFileName);

        }

        static void CloseWordDocument(object WordDoc)
        {
            object wordApp;
          
            MethodInfo mInfo = FindMethod(WordDoc, "Close", 1);
            //(int)Microsoft.Office.InteropLateBound.Word.WdSaveOptions.wdDoNotSaveChanges=;
          
            mInfo.Invoke(WordDoc, new object[] {0});

            wordApp = ReflectionUtils.GetProperty(WordDoc, "Application");
            mInfo=FindMethod(wordApp, "Quit", 1);
            mInfo.Invoke(wordApp, new object[] {0});
        }

        static MethodInfo FindMethod(object obj, string methodName, int pLength)
        {
            Type type = obj.GetType();

            MethodInfo[] mInfo = type.GetMethods();
            if (mInfo != null)
            {
                for (int i = 0; i < mInfo.Length; i++)
                {
                    if (mInfo[i].Name == methodName && mInfo[i].GetParameters().Length == pLength)
                    {
                        return mInfo[i];
                    }
                }
            }

            return null; 
        }
        private static string LoadRTFFile(string filename)
        {
            string fileContent = "";

            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    fileContent = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception)
            {
               
            }

            if (fileContent != "")
            {
                // first character of rtf file must be an open-brace
                if (fileContent[0] == '{')
                {

                    using (FileStream fileOpenStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        // read the file in as ANSI (otherwise StreamReader will mess up the conversion to Unicode)
                        BinaryReader br = new BinaryReader(fileOpenStream);
                        byte[] rtfBytesAnsi = br.ReadBytes((int)br.BaseStream.Length);
                        Encoding rtfEncoding = Encoding.GetEncoding(1252);
                        fileContent = rtfEncoding.GetString(rtfBytesAnsi);
                        br.Close();
                        fileOpenStream.Close();
                    }
               }
            }
            return fileContent; 
        }
        private static void LoadRTFfileIntoRichTextBox(RichTextBox richTextBox, string filePath)
        {
            string rtfContent = string.Empty;
            rtfContent = LoadRTFFile(filePath);
            object [] objs = new object[1];
            objs[0] = rtfContent; 
            Type type = ReflectionUtils.FindType("TextEditorCopyPaste");
            MemoryStream memoryStream = ReflectionUtils.InvokeStaticMethod(type, "ConvertRtfToXaml", objs) as MemoryStream;

            objs[0] = memoryStream;
            type = ReflectionUtils.FindType("WpfPayload");
            System.Windows.Documents.TextElement element = ReflectionUtils.InvokeStaticMethod(type, "LoadElement", objs) as TextElement;
            
            if (null != element)
            {
                richTextBox.Document.Blocks.Clear();
            }

            if (element is Block)
            {
                if (element is Section)
                {
                    Block block;
                    Section section = element as Section;
                    while (section.Blocks.Count > 0)
                    {
                        block = section.Blocks.FirstBlock;
                        section.Blocks.Remove(section.Blocks.FirstBlock);
                        richTextBox.Document.Blocks.Add(block);
                    }
                }
                else
                {
                    richTextBox.Document.Blocks.Add((Block)element);
                }
            }
            else if(element is Inline)
            {
                richTextBox.Document.Blocks.Add(new Paragraph((Inline)element));
            }
        }

        //private static  Word.Document BootWordAppold(string fullFileName)
        //{
        //    Word.Application wdApp; 
        //    wdApp = new Word.Application();
        //    wdApp.Visible = false;
        //    wdApp.ScreenUpdating = false;
        //    wdApp.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone;
        //    wdApp.AutomationSecurity = Microsoft.Office.InteropLateBound.Office.MsoAutomationSecurity.msoAutomationSecurityForceDisable;
            
        //    SetAutoMacros(wdApp, true);

        //    return wdApp.Documents.Open(fullFileName, false, true, false, "foo", "foo", true);
            
        //}

        private static object BootWordApp(string fullFileName)
        {
            object wdApp;
            object[] objs = new object[7];
            try
            {

                wdApp = ReflectionUtils.CreateInstanceOfType("Microsoft.Office.InteropLateBound.Word.Application", new object[] { });
            }
            catch (Exception e)
            {
                Exception ex = new Exception(e.Message + "\r\nIs OfficeLateBoundWrapper.dll copied to the same diretory as EditingExaminer?");
                throw ex; 
            }
            ReflectionUtils.SetProperty(wdApp, "Visible", false);
            ReflectionUtils.SetProperty(wdApp, "ScreenUpdating", false);
            
            //Word.DisplayAlerts = Microsoft.Office.InteropLateBound.Office.MsoAutomationSecurity==3;
            //ReflectionUtils.SetProperty(wdApp, "DisplayAlerts", 3);
            
            //SetAutoMacros(wdApp, true);
            
            object document = ReflectionUtils.GetProperty(wdApp, "Documents");
            objs[0] = fullFileName;
            objs[1] = false;
            objs[2] = true;
            objs[3] = false;
            objs[4] = "foo";
            objs[5] = "fool";
            objs[6] = true;

            return ReflectionUtils.InvokeInstanceMethod(document, "Open", objs);
        }

    //    private static void SetAutoMacros(object wapp, bool fDisable)
    //    {

    //        try
    //        {
    //            //object wb = wapp.WordBasic;
    //            object wb = ReflectionUtils.GetProperty(wapp, "WordBasic");
    //            int[] array = new int[1];
    //            System.Guid IID_NULL = System.Guid.Empty;
    //            IntPtr[] err = new IntPtr[2];
    //            System.Runtime.InteropServices.EXCEPINFO except = new System.Runtime.InteropServices.EXCEPINFO();
    //            System.Runtime.InteropServices.DISPPARAMS dispparams = new System.Runtime.InteropServices.DISPPARAMS();

    //            ((IDispatch)wb).GetIDsOfNames(ref IID_NULL, new string[] { "DisableAutoMacros" }, 1, 1033, array);

    //            dispparams.cNamedArgs = 0;
    //            dispparams.cArgs = 1;

    //            int size = 16 /*sizeof(VariantBool)*/;
    //            IntPtr varArray = Marshal.AllocCoTaskMem(size);

    //            VariantBool var = new VariantBool();

    //            if (fDisable)
    //            {
    //                var.boolVal = -1 /*TRUE*/;
    //            }
    //            else
    //            {
    //                var.boolVal = 0; /*FALSE*/
    //            }

    //            var.vt = 11 /*VT_BOOL*/;

    //            Marshal.StructureToPtr(var, varArray, false);
    //            dispparams.rgvarg = varArray;

    //            ((IDispatch)wb).Invoke(array[0], ref IID_NULL, 1033,
    //                1/*DISPATCH_METHOD*/, ref dispparams, null, ref except, err);

    //            Marshal.FreeCoTaskMem(varArray);
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.Fail("Failed to disable auto macros", ex.Message);
    //        }
    //    }
 
        private static string s_tempRtfFileName = System.Environment.CurrentDirectory + "/" + "EditingExaminerTempRtfFile.rtf"; 
    }
    
    //[Guid("00020400-0000-0000-c000-000000000046")]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface IDispatch
    //{
    //    [PreserveSig]
    //    int GetTypeInfoCount();
    //    System.Runtime.InteropServices.UCOMITypeInfo GetTypeInfo(
    //        [MarshalAs(UnmanagedType.U4)] int iTInfo,
    //        [MarshalAs(UnmanagedType.U4)] int lcid);
    //        [PreserveSig]
    //    int GetIDsOfNames(
    //    ref Guid riid,
    //    [MarshalAs(UnmanagedType.LPArray,
    //    ArraySubType = UnmanagedType.LPWStr)]
    //    string[] rgsNames,
    //    int cNames,
    //    int lcid,
    //    [MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);
    //    [PreserveSig]
    //    int Invoke(
    //        int dispIdMember,
    //        ref Guid riid,
    //    [MarshalAs(UnmanagedType.U4)] int lcid,
    //    [MarshalAs(UnmanagedType.U4)] int dwFlags,
    //    ref DISPPARAMS pDispParams,
    //    [MarshalAs(UnmanagedType.LPArray)][Out] object[]
    //    pVarResult,
    //    ref EXCEPINFO pExcepInfo,
    //    [MarshalAs(UnmanagedType.LPArray)][Out] IntPtr[]
    //    pArgErr);
    //}
    public struct VariantBool
    {
        public short vt /*VT_BOOL = 11*/;
        //short wReserved1;
        //short wReserved2;
        //short wReserved3;
        public short boolVal;
        public short dummy1;
        //IntPtr dummy2;
    }

    internal enum WordFileFormats
    {
        wdFormatAutosaveTemplateTest=17,
        wdFormatAutosaveTest=16,
        wdFormatDocument=0,
        wdFormatDocument97=14,
        wdFormatDocumentME=12,
        wdFormatDOSText=4,
        wdFormatDOSTextLineBreaks=5,
        wdFormatEncodedText=7,
        wdFormatFilteredHTML=10,
        wdFormatHTML=8,
        wdFormatRTF=6,
        wdFormatTemplate=1,
        wdFormatTemplate97=15,
        wdFormatTemplateME=13,
        wdFormatText=2,
        wdFormatTextLineBreaks=3,
        wdFormatUnicodeText=7,
        wdFormatWebArchive=9,
        wdFormatXML=1,
    }
}
