// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    sealed class PairwiseSettings
    {

        char aliasDelimiter = PictDefaults.DefaultAliasSeparator;

        bool cache = false;

        bool caseSensitive = false;

        static Regex containsWeight = new Regex(@".\(\d+\)");

        static readonly PairwiseSettings def = new PairwiseSettings(2, true);

        char delimParameter = PictDefaults.DefaultParameterValueSeparator;

        static string fullPath;

        static readonly object mutex = new object();

        char negativePrefix = PictDefaults.DefaultNegativeValuePrefix;

        string negativePrefixString = PictDefaults.DefaultNegativeValuePrefixString;

        int order = 2;

        static string pictDirectory;

        int randomSeed;

        bool randomSeedSpecified = false;

        string setValueSeparator = PictDefaults.SetValueSeparator;

        char[] specialCharacters;

        bool useRandom = false;

        PictWarningBehaviors warningBehavior = PictWarningBehaviors.DefaultBehavior;

        string seedFile = null;

        public event PictWarningEventHandler WarningBehaviorEvent;

        internal char AliasDelimiter
        {
            get
            {
                return aliasDelimiter;
            }
        }

        /// <summary>
        /// we can only cache if we're allowed to and we don't randomize OR (we randomize and we specify the seed)
        /// </summary>
        internal bool CanCache
        {
            get
            {
                if (!this.UsePairwiseCache)
                {
                    return false;
                }

                if (this.RandomizeGeneration)
                {
                    return this.RandomSeedSpecified;
                }
                else
                {
                    return true;
                }
            }
        }

        public static PairwiseSettings DefaultSettings
        {
            get
            {
                return def;
            }
        }

        public bool IsCaseSensitive
        {
            get
            {
                return this.caseSensitive;
            }
            set
            {
                this.caseSensitive = value;
            }
        }

        internal char NegativePrefix
        {
            get
            {
                return negativePrefix;
            }
        }

        internal string NegativePrefixString
        {
            get
            {
                return negativePrefixString;
            }
        }

        public int Order
        {
            get
            {
                return order;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "must be greater than or equal to 0");
                }

                this.order = value;
            }
        }

        public static string PictDirectory
        {
            get
            {
                lock (mutex)
                {
                    return pictDirectory;
                }
            }
            set
            {
                lock (mutex)
                {
                    fullPath = null;
                    pictDirectory = value;
                }
            }
        }

        public bool RandomizeGeneration
        {
            get
            {
                return useRandom;
            }
            set
            {
                useRandom = value;
            }
        }

        public int RandomSeed
        {
            get
            {
                return randomSeed;
            }
            set
            {
                randomSeed = value;
            }
        }

        public bool RandomSeedSpecified
        {
            get
            {
                return randomSeedSpecified;
            }
            set
            {
                randomSeedSpecified = value;
            }
        }

        internal string SetValueSeparator
        {
            get
            {
                return setValueSeparator;
            }
        }

        public bool UsePairwiseCache
        {
            get
            {
                return cache;
            }
            set
            {
                cache = value;
            }
        }

        internal char ValueDelimiter
        {
            get
            {
                return delimParameter;
            }
        }

        public PictWarningBehaviors WarningBehavior
        {
            get
            {
                return warningBehavior;
            }
            set
            {
                warningBehavior = value;
            }
        }

        public PairwiseSettings(): this(2)
        {
        }

        static PairwiseSettings()
        {
            string asm = typeof(PairwiseSettings).Assembly.Location;

            asm = Path.GetDirectoryName(asm);
            pictDirectory = asm;
        }

        public PairwiseSettings(int order)
        {
            this.Order = order;
        }

        public PairwiseSettings(int order, bool useCache): this(order)
        {
            this.cache = useCache;
        }

        public bool ContainsSpecialCharacters(string value)
        {
            if (specialCharacters == null)
            {
                char[] temp = new char[] {
                        // commas are bad
                    this.ValueDelimiter,
                        // pipes | usually
                        this.AliasDelimiter,
                        // tabs
                        PictConstants.OutputValueSeparator
                };

                specialCharacters = temp;
            }

            bool cont = value.IndexOfAny(specialCharacters) != -1;

            cont = cont || value.StartsWith(this.NegativePrefixString);
            cont = cont || containsWeight.IsMatch(value);
            return cont;
        }

        static char GetChar(string s)
        {
            if (s.Length != 1)
            {
                throw new ArgumentOutOfRangeException("Incorrect length: " + s.Length);
            }

            return s[0];
        }

        public string GetPictArgs()
        {
            string args = "";

            if (this.order != 2)
            {
                args += " /o:" + this.Order.ToString(PictConstants.Culture);
            }

            args += " /d:" + this.delimParameter;
            args += " /a:" + this.aliasDelimiter;
            args += " /n:" + this.negativePrefix;
            if (this.IsCaseSensitive)
            {
                args += " /c";
            }

            if (this.RandomizeGeneration)
            {
                args += " /r";
                if (this.RandomSeedSpecified)
                {
                    args += ":" + this.randomSeed.ToString(PictConstants.Culture);
                }
            }

            if (this.seedFile != null)
            {
                args += " /e:" + this.seedFile;
            }

            return args;
        }


        public string SeedFile
        {
            get { return this.seedFile; }
            set { this.seedFile = value; }
        }

        public static string GetPictExecutableFullPath()
        {
            if (fullPath == null)
            {
                lock (mutex)
                {
                    if (fullPath == null)
                    {
                        string dir = Environment.CurrentDirectory; //Environment.ExpandEnvironmentVariables(pictDirectory);

                        PictConstants.Trace("Directory = {0}", dir);

                        string arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToLower(PictConstants.Culture).Trim();
                        string id = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");

                        PictConstants.Trace("Arch = {0}, ID = {1}", arch, id);

                        string file;

                        if (arch == "ia64")
                        {
                            file = "pict.ia64.exe";
                        }
                        else if (arch == "amd64")
                        {
                            file = "pict.amd64.exe";
                        }
                        else if (arch == "x86")
                        {
                            file = "pict.exe";
                        }
                        else
                        {
                            PictConstants.Trace("Unrecognized arch: {0}, defaulting to pict.exe", arch);
                            file = "pict.exe";
                        }

                        string combined = Path.Combine(dir, file);

                        PictConstants.Trace("Combined path: {0}", combined);

                        string temp = Path.GetFullPath(combined);

                        PictConstants.Trace("Checking full path: {0}", temp);
                        if (!File.Exists(temp))
                        {
                            string msg = string.Format("Couldn't find {2} in {0} for processor {1}", dir, arch, file);

                            if (dir != pictDirectory)
                            {
                                msg += "(original directory = " + dir + ")";
                            }

                            throw new FileNotFoundException(msg, temp);
                        }

                        fullPath = temp;
                    }
                }
            }

            return fullPath;
        }

        static System.Diagnostics.FileVersionInfo versionInfo;
        public static System.Diagnostics.FileVersionInfo GetPictFileVersionInfo()
        {
            if (versionInfo == null)
            {
                lock (mutex)
                {
                    if (versionInfo == null)
                    {
                        versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(GetPictExecutableFullPath());
                    }
                } // end lock
            } // end outer if
            return versionInfo;
        }

        public void OnWarning(string warning)
        {
            PictConstants.Trace("Got warning with behavior {0}: {1}", warningBehavior, warning);
            if (this.warningBehavior == PictWarningBehaviors.Ignore)
            {
                PictConstants.Trace("Ignoring warning: {0}", warning);
                return;
            }

            if ((this.warningBehavior & PictWarningBehaviors.FireWarningBehaviorEvent) != 0)
            {
                if (this.WarningBehaviorEvent != null)
                {
                    this.WarningBehaviorEvent(this, new PictWarningEventArgs(warning));
                }
            }

            if ((this.warningBehavior & PictWarningBehaviors.WriteToConsoleError) != 0)
            {
                Console.Error.WriteLine(warning);
            }

            if ((this.warningBehavior & PictWarningBehaviors.ThrowException) != 0)
            {
                PictConstants.Trace("Throwing {0}", warning);
                throw new PairwiseException(warning);
            }
        }

        public static PairwiseSettings Parse(string argsForPict)
        {
            if (argsForPict == null)
            {
                throw new ArgumentNullException("argsForPict");
            }
            PairwiseSettings settings = new PairwiseSettings(2, false);

            foreach (string raw in argsForPict.Split())
            {
                string s = raw.Trim();

                if (s.Length == 0)
                {
                    continue;
                }

                if (s[0] != '/')
                {
                    throw new ArgumentOutOfRangeException("Unrecognized prefix: " + s);
                }

                char cmd = Char.ToLower(s[1], PictConstants.Culture);

                if (s.Length > 3 && s[2] != ':')
                {
                    throw new ArgumentOutOfRangeException("Unrecognized delimiter: " + s);
                }

                PictConstants.Trace(s + " " + s.Length);

                string extra = s.Length > 2 ? s.Substring(3) : "";

                switch (cmd)
                {
                    case 'o':
                        settings.Order = Int32.Parse(extra, PictConstants.Culture);
                        break;

                    case 'd':
                        settings.delimParameter = GetChar(extra);
                        settings.setValueSeparator = settings.delimParameter + " ";
                        break;

                    case 'a':
                        settings.aliasDelimiter = GetChar(extra);
                        break;

                    case 'n':
                        settings.negativePrefix = GetChar(extra);
                        settings.negativePrefixString = settings.negativePrefix.ToString(PictConstants.Culture);
                        break;

                    case 'c':
                        settings.IsCaseSensitive = true;
                        break;

                    case 'r':
                        settings.RandomizeGeneration = true;
                        if (extra.Length != 0)
                        {
                            settings.RandomSeedSpecified = true;
                            settings.RandomSeed = Int32.Parse(extra);
                        }

                        break;

                    case 'e':
                        settings.seedFile = extra;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("Unrecognized: " + raw);
                }
            }

            PictConstants.Trace("Parse: {0} -> {1}", argsForPict, settings.GetPictArgs());
            return settings;
        }

        /// <summary>
        /// this is just an alias for Trace.Listeners.Add(new TextWriterTraceListener(writer).
        /// </summary>
#if PICT_WRAPPER_TRACE
        public static void TraceTo(TextWriter writer)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(writer));
        }
#else
        public static void TraceTo(TextWriter writer)
        {
        }
#endif
    }
}
