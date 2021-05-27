// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    #region using;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Security.Cryptography;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Serialization;
    using System.Xml;
    using System.Text.RegularExpressions;
    using System.Text;
    using System.Runtime.Serialization.Formatters.Binary;
    #endregion
    // NOTE: we're just using binary serialization here
    [Serializable]
    internal sealed class PairwisePictCache
    {
        [NonSerialized]
        volatile bool changed = false;

        readonly Hashtable dict;
        public bool Changed
        {
            get { return changed; }
        }

        public int Count { get { return dict.Count; } }

        public PictExecutionInformation this[string key]
        {
            get
            {
                if (!Contains(key))
                {
                    throw new IndexOutOfRangeException(key + " not found");
                }
                return (PictExecutionInformation)dict[key];
            }

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (dict.ContainsKey(key))
                {
                    string[][] before = ((PictExecutionInformation)dict[key]).ParsedOutput;
                    changed = !IsSame(before, value.ParsedOutput);
                    if (!changed)
                    {
                        return;
                    }
                }
                else
                {
                    changed = true; // added something: the key wasn't there
                }
                this.dict[key] = value;
            }
        }

        public PairwisePictCache()
        {
            this.dict = Hashtable.Synchronized(new Hashtable());
        }

        public bool Contains(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return dict.ContainsKey(key);
        }

        static bool IsSame(string[][] a, string[][] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            if (a == b || (a.Length == 0 && b.Length == 0))
            {
                return true;
            }

            for (int i = 0; i < a.Length; ++i)
            {
                string[] sa = a[i];
                string[] sb = b[i];

                if (sa.Length != sb.Length)
                {
                    return false;
                }

                for (int j = 0; j < sa.Length; ++j)
                {
                    if (sa[j] != sb[j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal void ResetChanged()
        {
            this.changed = false;
        }
    }

    internal sealed class PairwisePictCacheHelper
    {

        static BinaryFormatter formatter;
        static readonly Hashtable loaded = Hashtable.Synchronized(new Hashtable());
        static readonly object mutex = new object();

        PairwisePictCacheHelper()
        {
        }

        public static PairwisePictCache LoadOrCreate(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException();
            }
            lock (mutex)
            {
                PairwisePictCache ppc;
                if (loaded.ContainsKey(filename))
                {
                    ppc = (PairwisePictCache)loaded[filename];
                    PictConstants.Trace("Using cached object instead of loading from {0}", filename);
                }
                else
                {
                    if (formatter == null)
                    {
                        formatter = new BinaryFormatter();
                    }
                    try
                    {
                        using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                        {
                            ppc = (PairwisePictCache)formatter.Deserialize(fs);
                            Pict.PictConstants.Trace("Loaded cache from {0}", filename);
                        }
                    }
                    catch (Exception e)
                    {
                        Pict.PictConstants.Trace("Error loading cache from {0}: {1}", filename, e.Message);
                        ppc = new PairwisePictCache();
                    }
                    loaded[filename] = ppc;
                }
                return ppc;
            }
        }

        public static void Save(PairwisePictCache cache, string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            lock (mutex)
            {
                try
                {
                    if (cache.Changed)
                    {
                        if (formatter == null)
                        {
                            formatter = new BinaryFormatter();
                        }
                        using (FileStream fs = new FileStream(filename, FileMode.Create))
                        {
                            formatter.Serialize(fs, cache);
                        }

                        PictConstants.Trace("Saved to {0}, entries = {1}", filename, cache.Count);
                        cache.ResetChanged();
                    }
                    else
                    {
                        PictConstants.Trace("Didn't change!");
                    }
                }
                catch (Exception e)
                {
                    PictConstants.Trace("Error saving cache to {0}: {1}", filename, e.Message);
                }
            }
        }
    }
}
