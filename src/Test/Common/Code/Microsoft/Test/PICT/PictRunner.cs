// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    #region using;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Security.Cryptography;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Xml.Serialization;
    using System.Xml;
    using System.Text;
    #endregion
    
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    sealed class PictRunner : IDisposable
    {
        readonly PairwisePictCache cache;

        readonly string cacheFileName;

        bool disposed = false;

        static readonly Encoding EncodingForHashAlgorithm = Encoding.Default;

        static HashAlgorithm hashAlgLate;

        PictExecutionInformation last;

        static HashAlgorithm HashAlg
        {
            get
            {
                if (hashAlgLate == null)
                {
                    lock (EncodingForHashAlgorithm)
                    {
                        if (hashAlgLate == null)
                        {
                            hashAlgLate = new SHA512Managed();
                        }
                    }
                }

                return hashAlgLate;
            }
        }

        public PictExecutionInformation LastExecutionInformation
        {
            get
            {
                if (last == null)
                {
                    throw new InvalidOperationException("No Pict execution occurred yet.");
                }

                return last;
            }
        }

        /// <summary>
        /// creates a new runner w/o a cache file
        /// </summary>
        public PictRunner(): this(null)
        {
        }

        public PictRunner(string cacheFileName)
        {
            if (cacheFileName != null)
            {
                this.cacheFileName = cacheFileName;
                this.cache = PairwisePictCacheHelper.LoadOrCreate(cacheFileName);
            }
        }

        ~PictRunner()
        {
            DisposeWork();
        }

        [Obsolete("Please specify the PairwiseSettings() to get 'proper' warning behavior")]
        public string[][] AlwaysExecutePictOnFileName(string modelFileName, string argsForPict)
        {
            return AlwaysExecutePictOnFileNameCore(modelFileName, argsForPict, null);
        }

        public string[][] AlwaysExecutePictOnFileName(string modelFileName, PairwiseSettings settings)
        {
            return AlwaysExecutePictOnFileNameCore(modelFileName, settings.GetPictArgs(), settings);
        }

        string[][] AlwaysExecutePictOnFileNameCore(string modelFileName, string argsForPict, PairwiseSettings settings)
        {
            if (modelFileName == null)
            {
                throw new ArgumentNullException("modelFileName");
            }

            if (argsForPict == null)
            {
                argsForPict = "";
            }
            else
            {
                argsForPict = argsForPict.Trim();
            }

            if (!File.Exists(modelFileName))
            {
                throw new FileNotFoundException("Pict model not found: " + modelFileName);
            }

            ProcessStartInfo psi = new ProcessStartInfo();

            // this will throw if not found
            psi.FileName = PairwiseSettings.GetPictExecutableFullPath();
            PictConstants.Trace("Attempting to use {0}", psi.FileName);

            psi.Arguments = (MaybeQuote(modelFileName) + " " + argsForPict).Trim();
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;

            DateTime started = DateTime.Now;

            PictConstants.Trace("Starting " + psi.Arguments);

            using (Process process = Process.Start(psi))
            {
                PictConstants.Trace("Started successfully");

                string[][] parsed = Split(process.StandardOutput, PictConstants.OutputValueSeparator);

                process.StandardOutput.Close();

                int seed = 0;
                bool wasRandom = false;

                process.WaitForExit();
                PictConstants.Trace("Done waiting for exit");

                TimeSpan elapsed = DateTime.Now - started;

                PictConstants.Trace("Elapsed time: {0}", elapsed);

                // note: there's some better way to do this??
                string allStdErr = process.StandardError.ReadToEnd();

                process.StandardError.Close();
                if (allStdErr.Length != 0)
                {
                    PictConstants.Trace("Begin stderr/{0}/end stderr*", allStdErr);

                    ArrayList msgs = new ArrayList();
                    foreach (string s in allStdErr.Split('\r', '\n'))
                    {
                        if (s.Trim().Length != 0)
                        {
                            msgs.Add(s.TrimEnd());
                        }
                    }

                    for (int i = 0; i < msgs.Count; ++i)
                    {
                        string message = (string)msgs[i];

                        if (message.Trim().Length == 0)
                        {
                            continue;
                        }

                        if (message.StartsWith(PictConstants.UsedSeedMessage))
                        {
                            message = message.Replace(PictConstants.UsedSeedMessage, "").Trim();
                            wasRandom = true;
                            seed = int.Parse(message, PictConstants.Culture);
                            PictConstants.Trace("{0} {1}", wasRandom, seed);
                        }
                        else if (message.StartsWith(PictConstants.WarningMessage) || (message.IndexOf("arning") != -1))
                        {

                            while (i < msgs.Count - 1 && ((string)msgs[i + 1]).StartsWith(" "))
                            {
                                message += Environment.NewLine + ((string)msgs[++i]);
                            }

                            if (settings != null)
                            {
                                settings.OnWarning(message.Trim());
                            }
                        }
                        else
                        {
                            PictConstants.Trace("throwing /{0}/", message.Trim());
                            throw new PairwiseException(message.Trim());
                        }
                    }
                }

                // string stdout = sb.ToString();
                PictExecutionInformation pei = new PictExecutionInformation();

                pei.FileName = modelFileName;
                pei.Generated = DateTime.Now;
                pei.Options = argsForPict;
                pei.RandomSeedSpecified = wasRandom;

                // pei.RawOutput = stdout;
                pei.ParsedOutput = parsed;
                if (wasRandom)
                {
                    pei.RandomSeed = seed;
                }

                this.last = pei;
                return parsed;
            } // end using(Process)
        }

        void IDisposable.Dispose()
        {
            DisposeWork();
        }
        void DisposeWork()
        {
            if (!disposed)
            {
                if (cache != null && cache.Changed)
                {
                    PairwisePictCacheHelper.Save(cache, cacheFileName);
                }
                else if (cacheFileName != null)
                {
                    PictConstants.Trace("Didn't change, so skipping saving to {0}", cacheFileName);
                }

                GC.SuppressFinalize(this);

                // prevent multiple disposes?
                disposed = true;
            }

        }

        static string GetHash(PairwiseSettings settings, string input)
        {
            if (!settings.CanCache)
            {
                return Guid.NewGuid().ToString();
            }

            string both = settings.GetPictArgs() + input;

            return Convert.ToBase64String(HashAlg.ComputeHash(EncodingForHashAlgorithm.GetBytes(both)));
        }

        public string[][] MaybeExecutePict(PairwiseSettings settings, string inputContent)
        {
            string key = GetHash(settings, inputContent);

            if (cache != null && settings.CanCache && cache.Contains(key))
            {
                PictConstants.Trace("Found key, using cached value!");
                this.last = cache[key];
                return last.ParsedOutput;
            }

            string randomFileName = Guid.NewGuid().ToString() + PictConstants.TempFileSuffix;

            using (StreamWriter sw = new StreamWriter(randomFileName))
            {
                sw.WriteLine("# " + randomFileName + " " + settings.GetPictArgs());
                sw.WriteLine(inputContent);
            }

            try
            {
                string[][] ret = AlwaysExecutePictOnFileName(randomFileName, settings);

                if (cache != null && settings.CanCache)
                {
                    // always execute pict set last correctly
                    cache[key] = this.LastExecutionInformation;
                }

                return ret;
            }
            finally
            {
                File.Delete(randomFileName);
            }
        }

        static string[][] Split(TextReader tr, char fieldDelim)
        {
            string all = tr.ReadToEnd().Trim();
            string[] lines = all.Split('\n');
            string[][] ret = new string[lines.Length][];

            for (int i = 0; i < lines.Length; ++i)
            {
                ret[i] = lines[i].TrimEnd().Split(fieldDelim);
            }

            return ret;
        }

        static string MaybeQuote(string s)
        {
            if (s.IndexOf(" ") == -1)
            {
                return s;
            }

            return '"' + s.Trim() + '"';
        }
    }
}
