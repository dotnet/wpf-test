// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#undef PICT_WRAPPER_TRACE
namespace Microsoft.Test.Pict
{
    #region using;
    using System;
    using System.Text.RegularExpressions;
    using System.Globalization;
    using System.Diagnostics;
    using System.Threading;
    using System.Security.Cryptography;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    #endregion

    sealed class PictDefaults
    {
        public const char DefaultAliasSeparator = '|';

        public const char DefaultNegativeValuePrefix = '~';

        public const string DefaultNegativeValuePrefixString = "~";

        // in general, this must be the same as SetValueSeparator?
        public const char DefaultParameterValueSeparator = ',';

        public const string SetValueSeparator = ", ";
        private PictDefaults()
        {
        }
    }

    sealed class PictConstants
    {
        public static readonly Regex isEasilyPrintable = new Regex(@"^[ @$\%&*\[\]\+\=\{\}\\/`'\""^#\-!?;_a-zA-Z0-9]+$", RegexOptions.None);
        public static string AggressivelyEscape(string s)
        {
            if (s.Length == 0)
            {
                return s;
            }
            StringBuilder sb = new StringBuilder(s.Length * 2);
            for (int i = 0; i < s.Length; ++i)
            {
                char c = s[i];
                string sc = c.ToString(PictConstants.Culture);
                if (isEasilyPrintable.IsMatch(sc))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.AppendFormat("_$0x{0:x}",((int) c));
                }
            }
            if (isEasilyPrintable.IsMatch(s) && sb.Length != s.Length)
            {
                throw new ApplicationException("Matched regex but did work anyway: " + s + " != "  +sb);
            }
            return sb.ToString();
        }

        public const string CacheFileName = "cachev32.bin";

        static readonly CultureInfo cult = CultureInfo.InvariantCulture;

        public const int DefaultWeight = 1;

        public const string NullObjectString = "Null";

        public static readonly Type[] NumericTypes = new Type[] {
            typeof(Int16), typeof(Int32), typeof(Int64), //
                typeof(UInt16), typeof(UInt32), typeof(UInt64), //
                typeof(Byte), typeof(SByte), //
                typeof(Single), typeof(Double), typeof(Decimal),
                                      typeof(char)
        };

        public const char OutputValueSeparator = '\t';

        // accompanying Pict version
        public static readonly Version Pict30Version = new Version(3, 0, 34, 0);

        public static readonly Version PictVersion = new Version(3, 2, 33, 0);
        public const string TempFileSuffix = ".pict";

        public const string TraceCategory = "Pairwise";

        public static readonly Type[] TreatLikeEnum = new Type[] {
            typeof(Guid), typeof(Boolean)
        };

        public const string UsedSeedMessage = "Used seed: ";

        public const string WarningMessage = "Warning:";

        public static CultureInfo Culture
        {
            get { return cult; }
        }

        PictConstants()
        {
        }

        [Conditional(PICT_WRAPPER_TRACE)]
        public static void Trace(string format, params object[] o)
        {
#if PICT_WRAPPER_TRACE
            string s;

            if (o.Length == 0)
            {
                s = format;
            }
            else
            {
                s = String.Format(format, o);
            }

            System.Diagnostics.Trace.WriteLine(s, TraceCategory);
#endif
        }

        public const string PICT_WRAPPER_TRACE  = "PICT_WRAPPER_TRACE";
    }
}
