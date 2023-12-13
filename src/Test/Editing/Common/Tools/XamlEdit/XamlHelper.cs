// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security;
using System.Text;
using System.Xml;

using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;

namespace XamlPadEdit
{
    public static class XamlHelper
    {
        #region Private Fields

        private const string savedXamlFileName = "XamlPad_Saved.xaml";

        private static Uri s_baseUri = null;
        private static string[] s_commandLineArgs = null;
        private static object s_lastParsedContent = null;
        private static bool s_isSavedToMyDocuments;
        private static bool s_isSavedToCurrentDir;
        private static bool s_isSavedToIsolatedStore;

        #endregion Private Fields

        #region Public Fields

        public const string InitialXaml = "<Page xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:sys=\"clr-namespace:System;assembly=mscorlib\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" >\r\n  <Grid>\r\n\r\n  </Grid>\r\n</Page>\r\n";

        #endregion Public Fields

        #region Public Properties

        public static string[] CommandLineArgs
        {
            get
            {
                return s_commandLineArgs;
            }

            set
            {
                s_commandLineArgs = value;
            }
        }

        public static object LastParsedContent
        {
            get
            {
                return s_lastParsedContent;
            }
        }

        public static string SavedXamlLocation
        {
            get
            {
                string ret = null;

                if (s_isSavedToIsolatedStore)
                    return "IsolatedStorageFile";
                
                if (s_isSavedToCurrentDir)
                    ret = "\"" + SavedXamlFileInCurrentWorkingDir + "\"";

                if (s_isSavedToMyDocuments)
                    ret =  "\"" + SavedXamlFileInMyDocuments + "\"";

                return ret;
            }
        }

        /// <summary>
        /// Full path to the saved XAML file in the MyDocument folder
        /// </summary>
        static string s_savedXamlFileInMyDocuments = null;
        public static string SavedXamlFileInMyDocuments
        {
            get
            {
                if (s_savedXamlFileInMyDocuments == null)
                {
                    s_savedXamlFileInMyDocuments = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), savedXamlFileName);
                }
                return s_savedXamlFileInMyDocuments;
            }
        }

        /// <summary>
        /// Full path to the saved XAML file in the current working dir
        /// </summary>
        static string s_savedXamlFileInCurrentWorkingDir = null;
        public static string SavedXamlFileInCurrentWorkingDir
        {
            get
            {
                if (s_savedXamlFileInCurrentWorkingDir == null)
                {
                    s_savedXamlFileInCurrentWorkingDir = System.IO.Path.Combine(Environment.CurrentDirectory, savedXamlFileName);
                }
                return s_savedXamlFileInCurrentWorkingDir;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public static string IndentXaml(string xaml)
        {
            //open the string as an XML node
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xaml);
            XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc);

            //write it back onto a stringWriter
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);
            xmlWriter.Formatting = System.Xml.Formatting.Indented;
            xmlWriter.Indentation = 4;
            xmlWriter.IndentChar = ' ';
            xmlWriter.WriteNode(nodeReader, false);

            string result = stringWriter.ToString();
            xmlWriter.Close();

            return result;
        }

        // WRITING the saved XAML file behavior
        //
        // 1. If this is a partial trust express app then write the XAML to the IsolatedStorage
        //
        // 2. If the app is deployed with ClickOnce and is not an partial trust express app
        // then save the XAML in the root of the current user's "My Documents" folder
        // 
        // 3. If the app is not ClickOnce deployed then it implies running from the command line. The
        // the saved XAML is written on app load to the current working directory. If that fails then save to
        // the "My Documents" folder. Writing to the CWD may fail because of access problems.
        // See Regression_Bug2
        public static void SaveXamlContent(string content)
        {
            string          ErrorMessage = null;
            bool            IsSuccessfullySaved = false;
            StreamWriter    SaveStream = null;

            s_isSavedToMyDocuments    = false;
            s_isSavedToCurrentDir     = false;
            s_isSavedToIsolatedStore  = false;

#if XamlPadExpressApp
            // XBAPs run in partial trust so can't write locally so attempt to save to IsolatedStoreage
            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    SaveStream = new StreamWriter(GetIsolatedStoreStream(FileMode.Create));
                    SaveStream.Write(content);
                    IsSuccessfullySaved = true;
                    IsSavedToIsolatedStore = true;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            finally
            {
                if (SaveStream != null)
                    SaveStream.Close();
            }
#else
            // First attempt to save to the CWD if not network deployed
            // This implies full trust when running from command prompt.
            try
            {
                if (!System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    SaveStream = new StreamWriter(new FileStream(SavedXamlFileInCurrentWorkingDir, FileMode.Create));
                    SaveStream.Write(content);
                    IsSuccessfullySaved = true;
                    s_isSavedToCurrentDir = true;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            finally
            {
                if (SaveStream != null)
                {
                    SaveStream.Close();
                }
            }

            // Attempt to save to MyDocuments if current user cannot write to the CWD
            if (!IsSuccessfullySaved && !s_isSavedToCurrentDir)
            {
                try
                {
                    SaveStream = new StreamWriter(new FileStream(SavedXamlFileInMyDocuments, FileMode.Create));
                    SaveStream.Write(content);
                    IsSuccessfullySaved = true;
                    s_isSavedToMyDocuments = true;
                }
                catch (Exception e)
                {
                    if (!String.IsNullOrEmpty(ErrorMessage))
                    {
                        ErrorMessage += "\n";
                    }
                    ErrorMessage += e.Message;
                }
                finally
                {
                    if (SaveStream != null)
                        SaveStream.Close();
                }
            }
#endif
            if (!IsSuccessfullySaved)
            {
                throw new ApplicationException("An error occurred saving " + savedXamlFileName + ": " + ErrorMessage);
            }
        }

        // READING the saved XAML file behavior
        //
        // 1. If this is a partial trust express app then read the XAML from the IsolatedStorage
        //
        // 2. If the app is deployed with ClickOnce and is not an partial trust express app
        // then read the XAML from the root of the current user's "My Documents" folder
        // 
        // 3. If the app is not ClickOnce deployed then it is run from the command line. The
        // the saved XAML is read the the most recent file from either the current working directory
        // or the "My Documents" folder. Load the file with the latest modified timestamp since
        // we try write to the saved file on startup and the user may not have access to CWD.
        // See SaveXamlContent(...) for details. This is to handle Regression_Bug2.
        public static string LoadSavedXamlContent()
        {
            bool    gotText     = false;
            string  loadedText  = null;

#if XamlPadExpressApp
            // try to read file from the IsolatedStore if this is a partial trust XBAP
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(GetIsolatedStoreStream(FileMode.Open)))
                    {
                        loadedText = sr.ReadToEnd();
                    }
                    gotText = true;
                }
                catch (Exception) { }
            }
#else
            // try to read file using the command line params first
            if (!gotText && !System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed &&
                null != s_commandLineArgs && s_commandLineArgs.Length > 0)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(s_commandLineArgs[0]))
                    {
                        loadedText = sr.ReadToEnd();
                    }
                    gotText = true;

                    // Set the base directory for this file.  commandLineArgs may be an absolute or relative file path
                    string dir = System.IO.Path.GetDirectoryName(s_commandLineArgs[0]);
                    if (dir == "")
                    {
                        dir = ".";
                    }

                    Uri uri = new Uri(dir + "/", UriKind.RelativeOrAbsolute);
                    if (!uri.IsAbsoluteUri)
                    {
                        Uri currentDir = new Uri(System.Environment.CurrentDirectory + "/");
                        uri = new Uri(currentDir, uri);
                    }
                    s_baseUri = uri;
                }
                catch (Exception) { }
            }

            // try to read from MyDocuments or CWD. The most recently updated file's contents is used.
            if (!gotText)
            {
                try
                {
                    string NewestSavedFile;
                    int DateDiff = DateTime.Compare(File.GetLastWriteTime(SavedXamlFileInCurrentWorkingDir), File.GetLastWriteTime(SavedXamlFileInMyDocuments));

                    if (DateDiff >= 0)
                    {
                        NewestSavedFile = SavedXamlFileInCurrentWorkingDir;
                    }
                    else
                    {
                        NewestSavedFile = SavedXamlFileInMyDocuments;
                    }

                    using (StreamReader sr = new StreamReader(File.Open(NewestSavedFile, FileMode.Open)))
                    {
                        loadedText = sr.ReadToEnd();
                    }
                    gotText = true;
                }
                catch (Exception) { }
            }
#endif

            if (!gotText)
            {
                loadedText = InitialXaml;
            }

            return loadedText;
        }

        public static ParseResult ParseXaml(string str)
        {
            return ParseXaml(str, false);
        }

        public static ParseResult ParseXaml(string str, bool dontSave)
        {
            if (!dontSave)
            {
                s_lastParsedContent = null;
            }

            ParseResult r = new ParseResult();
            MemoryStream ms = null;
            StreamWriter sw = null;
            try
            {
                ms = new MemoryStream(str.Length);
                sw = new StreamWriter(ms);
                sw.Write(str);
                sw.Flush();

                ms.Seek(0, SeekOrigin.Begin);

                ParserContext pc = new ParserContext();

                if (s_baseUri != null)
                {
                    // If we loaded a file from the command line then baseUri will be set to 
                    // the directory that contains that file.
                    pc.BaseUri = s_baseUri;
                }
                else
                {
                    try
                    {
                        // If we were launched without a file name use the current working directory
                        pc.BaseUri = new Uri(System.Environment.CurrentDirectory + "/");
                    }
                    catch (System.Security.SecurityException)
                    {
                        // If we are in partial trust we may not have access to the current directory
                    }
                }

                // XamlReader will close the MemoryStream.  Closing the MemoryStream
                //  (or the StreamWriter that's been built on top of it) risk closing
                //  the stream before the parser is done parsing.  (Such as when
                //  handling XAML with x:SynchronousMode="Async" set on the root tag.)
                object obj = XamlReader.Load(ms, pc);

                r.Root = obj;

                if (!dontSave)
                {
                    s_lastParsedContent = obj;
                }
            }
            catch (Exception e)
            {
                r.Root = null;
                r.ErrorMessage = e.Message;
                if (e is XamlParseException)
                {
                    XamlParseException xe = (XamlParseException)e;
                    r.ErrorLineNumber = xe.LineNumber;
                    r.ErrorPosition = xe.LinePosition;
                }
            }

            return r;
        }

        /// <summary>
        /// Get xaml from TextRange.Xml property
        /// </summary>
        /// <param name="range">TextRange</param>
        /// <returns>return a string serialized from the TextRange</returns>
        public static string TextRange_GetXaml(TextRange range)
        {
            MemoryStream mstream;

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            mstream = new MemoryStream();
            range.Save(mstream, DataFormats.Xaml);

            //must move the stream pointer to the beginning since range.save() will move it to the end.
            mstream.Seek(0, SeekOrigin.Begin);

            //Create a stream reader to read the xaml.
            StreamReader stringReader = new StreamReader(mstream);

            return stringReader.ReadToEnd();
        }

        /// <summary>
        /// Set xml to TextRange.Xml property.
        /// </summary>
        /// <param name="range">TextRange</param>
        /// <param name="xaml">Xaml to be set</param>
        public static void TextRange_SetXml(TextRange range, string xaml)
        {
            MemoryStream mstream;
            if (null == xaml)
            {
                throw new ArgumentNullException("xaml");
            }
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            mstream = new MemoryStream();
            StreamWriter sWriter = new StreamWriter(mstream);

            mstream.Seek(0, SeekOrigin.Begin); //this line may not be needed.
            sWriter.Write(xaml);
            sWriter.Flush();

            //move the stream pointer to the beginning. 
            mstream.Seek(0, SeekOrigin.Begin);

            range.Load(mstream, DataFormats.Xaml);
        }

        public static string RemoveIndentation(string xaml)
        {
            if (xaml.Contains("\r\n    "))
            {
                return RemoveIndentation(xaml.Replace("\r\n    ", "\r\n"));
            }
            else
            {
                return xaml.Replace("\r\n", "");
            }
        }

        #endregion Public Methods

        #region Private Methods
        private static FileStream GetIsolatedStoreStream(FileMode mode)
        {
            try
            {
                // Get a store from Isolated Storage and see if xamlpad_saved.xaml exists
                // for this user/application.
                return new IsolatedStorageFileStream(savedXamlFileName, mode, IsolatedStorageFile.GetUserStoreForApplication());
            }
            catch (Exception)
            {
                // Just return null as if the saved file wasn't there.
                return null;
            }
        }
        #endregion Private Methods
    }
}