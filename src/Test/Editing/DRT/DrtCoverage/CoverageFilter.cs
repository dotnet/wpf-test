// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;

namespace CovFilter
{
    using Dictionary = Dictionary<string, string>;

    class Program
    {
        static CmdLine s_cmdLine;

        static void Main(string[] args)
        {
            s_cmdLine = new CmdLine(args);
            new Program().Run();
        }

        XmlReader _src;
        XmlWriter _dst;

        Dictionary _funcBsl = new Dictionary();        // function -> baseline
        Dictionary _srcOwner = new Dictionary();        // sourceId -> owner
        Dictionary _funcSrc = new Dictionary();        // function -> sourceId
        SortedDictionary<string, int> _funcDecor = new SortedDictionary<string, int>();  // decorateName -> Percent

        OwnerLookup _ownerLookup = new OwnerLookup();

        void Run()
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            {
                readerSettings.IgnoreWhitespace = true;
                readerSettings.IgnoreProcessingInstructions = true;
                readerSettings.IgnoreComments = true;
            }

            if (s_cmdLine.BslFile != null)
            {
                using (_src = XmlReader.Create(s_cmdLine.BslFile, readerSettings))
                {
                    _src.Read();
                    _src.ReadStartElement("funcs");
                    while (IsStartElement("func"))
                    {
                        string decName = _src.GetAttribute("name");
                        _funcBsl[decName] = _src.GetAttribute("percent");
                        _src.Skip();
                    }
                    _src.ReadEndElement();
                }
            }

            using (_src = XmlReader.Create(s_cmdLine.CovFile, readerSettings))
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                {
                    writerSettings.Indent = true;
                    writerSettings.OmitXmlDeclaration = true;
                }
                using (_dst =
                    s_cmdLine.CovOut != null
                    ? XmlWriter.Create(s_cmdLine.CovOut, writerSettings)
                    : XmlWriter.Create(Console.Out, writerSettings)
                )
                {
                    _src.Read();
                    _dst.WriteComment(string.Format("Filtered results generated using command line arguments '{0}'", s_cmdLine.UnparsedArgs));

                    CopyStartElement("magellanviews");
                    CopyStartElement("views");
                    CopyStartElement("view");
                    CopyElement("keys");
                    CopyElement("headerlines");

                    CopySourceFiles();
                    CopyFuncs();
                    CopyClasses();

                    CopyEndElement("view");
                    CopyEndElement("views");
                    CopyEndElement("magellanviews");
                }
            }

            if (s_cmdLine.BslOut != null)
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                {
                    writerSettings.Indent = true;
                    writerSettings.OmitXmlDeclaration = true;
                }
                using (_dst = XmlWriter.Create(s_cmdLine.BslOut, writerSettings))
                {
                    _dst.WriteStartElement("funcs");

                    foreach (KeyValuePair<string, int> func in _funcDecor)
                    {
                        _dst.WriteStartElement("func");
                        _dst.WriteAttributeString("name", func.Key);
                        _dst.WriteAttributeString("percent", func.Value.ToString());
                        _dst.WriteEndElement(/*func*/);
                    }
                    _dst.WriteEndElement(/*funcs*/);
                }
            }

            if (s_cmdLine.BslFile != null)
            {
                Console.WriteLine("{0} uncovered functions in BSL", _funcBsl.Count);
                if (_funcSrc.Count == 0)
                {
                    Console.WriteLine("No new uncovered functions");
                }
                else
                {
                    Console.WriteLine("{0} NEW uncovered functions", _funcSrc.Count);
                    Console.WriteLine("Diff is saved to {0}", s_cmdLine.CovOut);
                }
            }
            else
            {
                Console.WriteLine("{0} uncovered functions", _funcSrc.Count);
                Console.WriteLine("Filtered data are saved to {0}", s_cmdLine.CovOut);
            }
        }

        void CopySourceFiles()
        {
            CopyStartElement("sourcefiles");
            while (IsStartElement("sourcefile"))
            {
                string sourceId = _src.GetAttribute("sourceid");
                _src.Read();
                string filePath = ReadText();
                CheckEndElement("sourcefile");
                _src.Read();
                string owner = _ownerLookup.GetFileOwner(filePath);

                if (s_cmdLine.OwnerName != null && !owner.Equals(s_cmdLine.OwnerName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (s_cmdLine.IncludeFiles.Count > 0 && !EndsWithAny(filePath, s_cmdLine.IncludeFiles))
                {
                    continue;
                }

                _srcOwner[sourceId] = owner;
                _dst.WriteStartElement("sourcefile");
                _dst.WriteAttributeString("sourceid", sourceId);
                _dst.WriteAttributeString("owner", owner);
                _dst.WriteString(filePath);
                _dst.WriteEndElement();
            }
            CopyEndElement("sourcefiles");
        }

        void CopyFuncs()
        {
            CopyStartElement("funcs");
            while (IsStartElement("func"))
            {
                string sourceId = _src.GetAttribute("sourceid");
                string name = _src.GetAttribute("name");
                string decor = _src.GetAttribute("decoratedname");
                int blockHitCountPct = int.Parse(_src.GetAttribute("blockhitcountpct"));
                string fullName = GetFullName(name, decor);

                if (
                    s_cmdLine.Percent < blockHitCountPct ||
                    !_srcOwner.ContainsKey(sourceId) ||
                    StartsWithAny(name, s_cmdLine.Excludes)
                )
                {
                    _src.Skip();
                    continue;
                }

                if (s_cmdLine.BslOut != null)
                {
                    _funcDecor[fullName] = blockHitCountPct;
                }

                if (_funcBsl.ContainsKey(fullName))
                {
                    _src.Skip();
                    continue;
                }

                _funcSrc[_src.GetAttribute("functionid")] = sourceId;
                CopyElement("func");
            }
            CopyEndElement("funcs");
        }

        string GetFullName(string name, string decor)
        {
            int offset = decor.IndexOf(name);
            if (offset < 0)
            {
                return name;
            }
            else
            {
                return decor.Substring(offset);
            }
        }

        void CopyClasses()
        {
            CopyStartElement("classes");
            while (IsStartElement("class"))
            {
                string[] funcIds = _src.GetAttribute("functionids").Split(',');
                int idx = 0;

                foreach (string id in funcIds)
                {
                    if (_funcSrc.ContainsKey(id))
                    {
                        funcIds[idx++] = id;
                    }
                }

                if (idx > 0)
                {
                    _dst.WriteStartElement("class");
                    _dst.WriteAttributeString("name", _src.GetAttribute("name"));
                    _dst.WriteAttributeString("functionids", string.Join(",", funcIds, 0, idx));
                    _dst.WriteEndElement();
                }
                _src.Read();
            }
            CopyEndElement("classes");
        }

        void CopyStartElement(string name)
        {
            CheckStartElement(name);
            _dst.WriteStartElement(name);
            while (_src.MoveToNextAttribute())
            {
                _dst.WriteAttributeString(_src.LocalName, _src.NamespaceURI, _src.Value);
            }
            _src.MoveToElement();
            _src.Read();
        }

        void CopyElement(string name)
        {
            CheckStartElement(name);
            _dst.WriteNode(_src, /*defattr*/true);
        }

        void CopyEndElement(string name)
        {
            CheckEndElement(name);
            _dst.WriteEndElement();
            _src.Read();
        }

        void SkipElement(string name)
        {
            CheckStartElement(name);
            _src.Skip();
        }

        #region Helper methods

        // Helper methods
        
        bool IsStartElement(string name)
        {
            return _src.NodeType == XmlNodeType.Element && _src.LocalName == name && _src.NamespaceURI.Length == 0;
        }

        bool IsEndElement(string name)
        {
            return _src.NodeType == XmlNodeType.EndElement && _src.LocalName == name && _src.NamespaceURI.Length == 0;
        }

        void CheckStartElement(string name)
        {
            if (!IsStartElement(name))
            {
                throw new ApplicationException("Unexpected element name '" + _src.Name + "'");
            }
        }

        void CheckEndElement(string name)
        {
            if (!IsEndElement(name))
            {
                throw new ApplicationException("Unexpected end element name '" + _src.Name + "'");
            }
        }

        StringBuilder _sb = new StringBuilder();

        string ReadText()
        {
            _sb.Length = 0;
            while (_src.NodeType == XmlNodeType.Text)
            {
                _sb.Append(_src.Value);
                _src.Read();
            }
            return _sb.ToString();
        }

        bool StartsWithAny(string str, List<string> prefixes)
        {
            foreach (string prefix in prefixes)
            {
                if (str.StartsWith(prefix))
                {
                    return true;
                }
            }
            return false;
        }

        bool EndsWithAny(string str, List<string> suffixes)
        {
            foreach (string suffix in suffixes)
            {
                if (str.EndsWith(suffix))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

    internal class CmdLine
    {
        public int Percent = 100;
        public string CovFile = null;
        public string BslFile = null;
        public string CovOut = null;
        public string BslOut = null;
        public string OwnerName = null;
        public bool UseOwner = false;
        public List<string> Excludes = new List<string>();
        public List<string> IncludeFiles = new List<string>();
        public string UnparsedArgs = null;

        public CmdLine(string[] args)
        {
            UnparsedArgs = string.Join(" ", args);
            foreach (String arg in args)
            {
                if ('-' == arg[0] || '/' == arg[0])
                {
                    string par = arg.Substring(1);
                    string tmp = null;

                    if (GetOption(par, "percent:", "p:", ref tmp))
                    {
                        Percent = int.Parse(tmp);
                    }
                    else if (GetOption(par, "user:", "u:", ref OwnerName))
                    {
                    }
                    else if (GetOption(par, "base:", "b:", ref BslFile))
                    {
                    }
                    else if (GetOption(par, "outputCov:", "oc:", ref CovOut))
                    {
                    }
                    else if (GetOption(par, "outputBsl:", "ob:", ref BslOut))
                    {
                    }
                    else if (GetOption(par, "exclude:", "x:", ref tmp))
                    {
                        Excludes.Add(tmp);
                    }
                    else if (GetOption(par, "includeFile:", "f:", ref tmp))
                    {
                        if (tmp.StartsWith("@"))
                        {
                            using (StreamReader sr = File.OpenText(tmp.Substring(1)))
                            {
                                string fileName;
                                while ((fileName = sr.ReadLine()) != null)
                                {
                                    if (fileName.EndsWith(".cs", /*ignoreCase:*/true, System.Globalization.CultureInfo.InvariantCulture))
                                    {
                                        IncludeFiles.Add(fileName);
                                        Console.WriteLine("Including file: " + fileName);
                                    }
                                }
                            }
                        }
                        else
                        {
                            IncludeFiles.Add(tmp);
                        }
                    }
                    else if (par == "?")
                    {
                        PrintUsageAndExit();
                    }
                    else
                    {
                        Console.Error.WriteLine("Unrecognized argument: '{0}'", arg);
                        PrintUsageAndExit();
                    }
                }
                else
                {
                    if (CovFile == null)
                    {
                        CovFile = arg;
                    }
                    else
                    {
                        Console.Error.WriteLine("File name was already specified");
                        Console.Error.WriteLine("Unrecognized argument: '{0}'", arg);
                        PrintUsageAndExit();
                    }
                }
            }

            if (CovFile == null)
            {
                Console.Error.WriteLine("Input file is not specified");
                PrintUsageAndExit();
            }
        }

        private void PrintUsageAndExit()
        {
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("    Uncover <filename> [-p:<number>] [-u:alias] [-b:<filename>] [-x:<namespace>]* [-oc:<filename>] [-ob:<filename>]");
            Console.Error.WriteLine("    -p:N  -percent:N      Search functions with coverage percent less than N");
            Console.Error.WriteLine("    -u:A  -user:A         Search only functions owned by user A");
            Console.Error.WriteLine("    -b:F  -base:F         Base file F will be used to exclude functions whose coverage has not changed");
            Console.Error.WriteLine("    -x:N  -exclude:N      Exclude functions with names starting with N");
            Console.Error.WriteLine("    -f:F  -include:F      Include file with names ending with F");
            Console.Error.WriteLine("    -oc:F -outputCov:F    Output filtered coverage data to file F");
            Console.Error.WriteLine("    -ob:F -outputBsl:F    Output new bsl to file F");
            Environment.Exit(1);
        }

        #region Helper methods
        
        // Helper methods
        
        private static bool GetOption(string arg, string optionName, ref string optionValue)
        {
            int optLen = optionName.Length;

            if (string.Compare(arg, 0, optionName, 0, optLen) == 0)
            {
                optionValue = arg.Substring(optLen);
                return true;
            }
            return false;
        }

        private static bool GetOption(string arg, string fullForm, string shortForm, ref string optionValue)
        {
            return GetOption(arg, fullForm, ref optionValue) || GetOption(arg, shortForm, ref optionValue);
        }
        #endregion
    }

    class OwnerLookup
    {
        // Max number of head lines to search for ownership
        const int HeadLinesToSearch = 20;

        private string _sdRoot;
        private string _srcLocation;
        private Regex _ownerRegex;

        /// <summary>
        /// Use this constructor if source files are available at their original file paths.
        /// </summary>
        public OwnerLookup() { }

        /// <summary>
        /// Use this constructor if source files must be searched at a different root than the one specified in file paths.
        /// Example: new OwnerLookup(@"ndp\fx\src\", @"\\cpvsbuild\Drops\whidbey\lab23dev\raw\current\sources\")
        /// </summary>
        public OwnerLookup(string sdRoot, string srcLocation)
        {
            this._sdRoot = sdRoot;
            this._srcLocation = srcLocation;
        }

        public string GetFileOwner(string filePath)
        {
            if (_ownerRegex == null)
            {
                _ownerRegex = new Regex(@"<owner\s+current\s*=\s*(['""])true\1\s+primary\s*=\s*(['""])true\2\s*>\s*([^<]*?)\s*</owner\s*>", RegexOptions.Compiled);
            }

            if (_sdRoot != null && _srcLocation != null)
            {
                int idx = filePath.IndexOf(_sdRoot);
                if (idx < 0)
                {
                    throw new ApplicationException(string.Format("File {0} is not under {1}", filePath, _sdRoot));
                }
                filePath = _srcLocation + filePath.Remove(0, idx);
            }

            try
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    for (int i = 0; i < HeadLinesToSearch; i++)
                    {
                        string str = sr.ReadLine();
                        Match match = _ownerRegex.Match(str);
                        if (match.Success)
                        {
                            return match.Groups[3].ToString().ToLower();
                        }
                    }
                    // Either ownership element is not found in the file, or there is no current primary owner
                    return "NULL";
                }
            }
            catch
            {
                // DirectoryNotFoundException, FileNotFoundException, UnauthorizedAccessException, etc.
                return "UNKNOWN";
            }
        }
    }
}