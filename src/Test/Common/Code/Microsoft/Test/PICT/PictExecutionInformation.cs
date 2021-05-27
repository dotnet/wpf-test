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

    [Serializable]
    sealed class PictExecutionInformation
    {
        string filename;
        DateTime gen = DateTime.Now;
        string options;
        // NOTE: a MD array could be used here, but the gains (~25% mem?) may not be worth it
        string[][] parsed;
        int seed;
        bool spec;

        public string FileName
        {
            get { return filename; }
            set { filename = value;}
        }

        public DateTime Generated
        {
            get { return gen; }
            set { gen = value;}
        }

        public string Options
        {
            get { return options; }
            set { options = value;}
        }

        internal string[][] ParsedOutput
        {
            get { return this.parsed; }
            set { this.parsed = value;}
        }

        public int RandomSeed
        {
            get { return seed; }
            set { seed = value;}
        }

        public bool RandomSeedSpecified
        {
            get { return spec; }
            set { spec = value;}
        }

        public PictExecutionInformation()
        {
        }
    }
}
