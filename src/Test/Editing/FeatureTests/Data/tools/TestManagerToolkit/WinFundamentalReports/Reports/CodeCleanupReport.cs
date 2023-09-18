// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinFundamentalReports.Reports
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    using WinFundamentalReports;

    #endregion Namespaces.

    class CodeCleanupReport : ReportBase
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new CodeCleanupReport instance.
        /// </summary>
        /// <param name="excelWriter">Writer to an SpreadsheetML document.</param>
        /// <param name="wordWriter">Writer to an WordML document.</param>
        /// <param name="options">Report options.</param>
        public CodeCleanupReport(XmlWriter excelWriter, XmlWriter wordWriter,
            FundamentalReportOptions options): base(excelWriter, wordWriter, options)
        {

        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Writes the report to the configured outputs.</summary>
        public override void WriteReport()
        {
            base.WriteReport();

            WriteStartSection("Code Cleanup");
            WriteParagraph("Code cleanup information for the following paths:");
            foreach (string path in Options.CodeCleanupPaths)
            {
                if (path == null || path.Length == 0)
                {
                    continue;
                }
                WriteParagraph(path);
            }

            foreach (string path in Options.CodeCleanupPaths)
            {
                if (path == null || path.Length == 0)
                {
                    continue;
                }

                WriteCleanupForPath(path);
            }

            WriteEndSection();
        }

        #endregion Public methods.

        #region Private methods.

        private void WriteCleanupComments(string file, List<string> comments)
        {
            if (file == null)
            {
                throw new ArgumentException("file");
            }
            if (comments == null)
            {
                throw new ArgumentException("file");
            }

            WriteEmptyParagraph();
            WriteStyledParagraph("File: " + file, true, false, null);
            foreach (string comment in comments)
            {
                WriteParagraph(comment);
            }
        }

        private void WriteCleanupForFile(string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            using (StreamReader reader = new StreamReader(file))
            {
                string line;
                List<string> comments;

                comments = null;
                while ((line = reader.ReadLine()) != null)
                {
                    bool lineIsComment;

                    lineIsComment = false;
                    if (line.IndexOf("TODO") != -1)
                    {
                        lineIsComment = true;
                    }
                    else if (line.IndexOf("TestBugs(\"123\"") != -1)
                    {
                        lineIsComment = true;
                    }
                    else if (line.IndexOf("TestTactics(\"123\"") != -1)
                    {
                        lineIsComment = true;
                    }

                    if (lineIsComment)
                    {
                        if (comments == null)
                        {
                            comments = new List<string>();
                        }
                        comments.Add(line);
                    }
                }

                if (comments != null)
                {
                    WriteCleanupComments(file, comments);
                }
            }
        }

        private void WriteCleanupForPath(string path)
        {
            string[] files;
            string[] directories;

            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            files = Directory.GetFiles(path, "*.cs");
            foreach (string file in files)
            {
                WriteCleanupForFile(file);
            }

            directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                WriteCleanupForPath(directory);
            }
        }

        #endregion Private methods.
    }
}
