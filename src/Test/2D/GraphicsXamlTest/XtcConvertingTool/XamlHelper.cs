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

namespace Microsoft.Test.Graphics
{
    public static class XamlHelper
    {
        #region Private Fields

        private const string savedXamlFileName = "XamlPad_Saved.xaml";

        private static Uri s_baseUri = null;
        private static string[] s_commandLineArgs = null;
        private static string s_savedDataPath = null;
        private static string s_savedDataFriendlyName = null;
        private static object s_lastParsedContent = null;

        #endregion Private Fields

        #region Public Fields

        public const string InitialXaml = "<Page xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" > \n  <Grid>\n\n  </Grid>\n</Page>\n";

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
                if (s_savedDataFriendlyName == null)
                {
                    s_savedDataFriendlyName = Path.Combine(Directory.GetCurrentDirectory(), savedXamlFileName);
                }

                return s_savedDataFriendlyName;
            }
        }


        #endregion Public Properties

        #region Public Methods

        public static string LoadSavedXamlContent()
        {
            bool gotText = false;
            string loadedText = null;
            if ((null != s_commandLineArgs) && (s_commandLineArgs.Length > 0))
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
            if (!gotText)
            {
                try
                {

                    using (StreamReader sr = new StreamReader(GetStreamForSavedXamlFile(FileMode.Open)))
                    {
                        loadedText = sr.ReadToEnd();
                    }
                    gotText = true;
                }
                catch (Exception) { }
            }
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
            try
            {
                MemoryStream ms = new MemoryStream(str.Length);
                StreamWriter sw = new StreamWriter(ms);
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

                object obj = XamlReader.Load(ms, pc);

                if (obj is FixedDocument)
                {
                    FixedDocument panel = obj as FixedDocument;

                    obj = panel.Pages[0].GetPageRoot(false);
                }

                r.Root = obj;

                if (!dontSave)
                {
                    s_lastParsedContent = obj;
                }

                return r;
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
                return r;
            }
        }

        public static void SaveXamlContent(string content)
        {
            try
            {
                StreamWriter saveFile = new StreamWriter(GetStreamForSavedXamlFile(FileMode.Create));
                saveFile.Write(content);
                saveFile.Close();
            }
            catch (SecurityException)
            {
                throw new ApplicationException("Security restrictions prevent access to " + savedXamlFileName + ".");
            }
            catch (ArgumentNullException)
            {
                throw new ApplicationException("An error occurred while trying to save " + savedXamlFileName + ".");
            }
        }

        #endregion Public Methods

        #region Private Properties

#if !XamlPadExpressApp
        private static string SavedDataPath
        {
            get
            {
                if (s_savedDataPath == null)
                {
                    s_savedDataPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), savedXamlFileName);
                }

                return s_savedDataPath;
            }
        }
#endif

        #endregion Private Properties

        #region Private Methods

        private static FileStream GetStreamForSavedXamlFile(FileMode mode)
        {
#if XamlPadExpressApp
            // Get a store from Isolated Storage and see if xamlpad_saved.xaml exists
            // for this user/application.
            IsolatedStorageFile isoStore;
            try
            {
                isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            }

            catch (SecurityException)
            {
                // Just return null as if the saved file wasn't there.
                return null;
            }

            // Get a stream for this file
            try
            {
                IsolatedStorageFileStream stream = new IsolatedStorageFileStream(
                    savedXamlFileName,
                    mode,
                    isoStore);

                savedDataFriendlyName = "Isolated Storage";
                return stream;
            }

            catch (FileNotFoundException)
            {
                // We are trying to open an existing file but it's not there.
                // Just return null and we'll default to the initial content.
                return null;
            }

            catch (IsolatedStorageException e)
            {
                // Isolated Storage permissions may not be granted to this user.
                return null;
            }

#else
            // Use the rules for reading/writing the saved file -- if the app was deployed
            // with ClickOnce, it goes to/from the user's desktop, otherwise the current 
            // directory is used.
            try
            {
                FileStream stream = new FileStream(SavedDataPath, mode);
                return stream;
            }

            catch (UnauthorizedAccessException)
            {
                return null;
            }

            catch (IOException)
            {
                return null;
            }

            catch (SecurityException)
            {
                return null;
            }

#endif
        }

        #endregion Private Methods


    }
}