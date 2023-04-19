// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
// Description: LocBaml command line parsing tool. 
//


using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Globalization;

namespace BamlLocalization 
{

    internal class Option
    {
        private String _strName;
        private String _strValue;

        public Option(String strName, String strValue)
        {
            _strName = strName;
            _strValue = strValue;
        }

        public String Name { get { return _strName; } }
        public String Value { get { return _strValue; } }
    }

    internal class Abbrevs
    {
        private String[] _aOptions;
        private bool[] _bRequiresValue;
        private bool[] _bCanHaveValue;

        public Abbrevs(String[] aOptions)
        {
            _aOptions = new String[aOptions.Length];
            _bRequiresValue = new bool[aOptions.Length];
            _bCanHaveValue = new bool[aOptions.Length];

            // Store option list in lower case for canonical comparison.
            for (int i = 0; i < aOptions.Length; i++)
            {
                String strOption = aOptions[i].ToLower(CultureInfo.InvariantCulture);

                // A leading '*' implies the option requires a value
                // (the '*' itself is not stored in the option name).
                if (strOption.StartsWith("*"))
                {
                    _bRequiresValue[i] = true;
                    _bCanHaveValue[i] = true;
                    strOption = strOption.Substring(1);
                }
                else if (strOption.StartsWith("+"))
                {
                    _bRequiresValue[i] = false;
                    _bCanHaveValue[i] = true;
                    strOption = strOption.Substring(1);
                }

                _aOptions[i] = strOption;
            }
        }

        public String Lookup(String strOpt, out bool bRequiresValue, out bool bCanHaveValue)
        {
            String strOptLower = strOpt.ToLower(CultureInfo.InvariantCulture);
            int i;
            bool bMatched = false;
            int iMatch = -1;

            // Compare option to stored list.
            for (i = 0; i < _aOptions.Length; i++)
            {
                // Exact matches always cause immediate termination of
                // the search
                if (strOptLower.Equals(_aOptions[i]))
                {
                    bRequiresValue = _bRequiresValue[i];
                    bCanHaveValue = _bCanHaveValue[i];
                    return _aOptions[i];
                }

                // Check for potential match (the input word is a prefix
                // of the current stored option).
                if (_aOptions[i].StartsWith(strOptLower))
                {
                    // If we've already seen a prefix match then the
                    // input word is ambiguous.
                    if (bMatched)
                        throw new ArgumentException(StringLoader.Get("Err_AmbigousOption", strOpt));

                    // Remember this partial match.
                    bMatched = true;
                    iMatch = i;
                }
            }

            // If we get here with bMatched set, we saw one and only one
            // partial match, so we've got a winner.
            if (bMatched)
            {
                bRequiresValue = _bRequiresValue[iMatch];
                bCanHaveValue = _bCanHaveValue[iMatch];
                return _aOptions[iMatch];
            }

            // Else the word doesn't match at all.
            throw new ArgumentException(StringLoader.Get("Err_UnknownOption", strOpt));
        }
    }

    internal class CommandLine
    {
        private String[] _aArgList;
        private Option[] _aOptList;
        private int _iArgCursor;
        private int _iOptCursor;
        private Abbrevs _sValidOptions;

        public CommandLine(String[] aArgs, String[] aValidOpts)
        {
            int i, iArg, iOpt;

            // Keep a list of valid option names.
            _sValidOptions = new Abbrevs(aValidOpts);

            // Temporary lists of raw arguments and options and their
            // associated values.
            String[] aArgList = new String[aArgs.Length];
            Option[] aOptList = new Option[aArgs.Length];

            // Reset counters of raw arguments and option/value pairs found
            // so far.
            iArg = 0;
            iOpt = 0;

            // Iterate through words of command line.
            for (i = 0; i < aArgs.Length; i++)
            {
                // Check for option or raw argument.
                if (aArgs[i].StartsWith("/") ||
                    aArgs[i].StartsWith("-"))
                {
                    String strOpt;
                    String strVal = null;
                    bool bRequiresValue;
                    bool bCanHaveValue;

                    // It's an option. Strip leading '/' or '-' and
                    // anything after a value separator (':' or
                    // '=').
                    int iColon = aArgs[i].IndexOfAny(new char[] {':', '='});
                    if (iColon == -1)
                            strOpt = aArgs[i].Substring(1);
                    else
                            strOpt = aArgs[i].Substring(1, iColon - 1);

                    // Look it up in the table of valid options (to
                    // check it exists, get the full option name and
                    // to see if an associated value is expected).
                    strOpt = _sValidOptions.Lookup(strOpt, out bRequiresValue, out bCanHaveValue);

                    // Check that the user hasn't specified a value separator for an option 
                    // that doesn't take a value.
                    if (!bCanHaveValue && (iColon != -1))
                        throw new ApplicationException(StringLoader.Get("Err_NoValueRequired", strOpt));

                    // Check that the user has put a colon if the option requires a value.
                    if (bRequiresValue && (iColon == -1))
                        throw new ApplicationException(StringLoader.Get("Err_ValueRequired", strOpt));
                    
                    // Go look for a value if there is one.
                    if (bCanHaveValue && iColon != -1)
                    {
                        if (iColon == (aArgs[i].Length - 1))
                        {
                            // No value separator, or
                            // separator is at end of
                            // option; look for value in
                            // next command line arg.
                            if (i + 1 == aArgs.Length)
                            {
                                throw new ApplicationException(StringLoader.Get("Err_ValueRequired", strOpt));
                            }
                            else
                            {
                                if ((aArgs[i + 1].StartsWith( "/" ) || aArgs[i + 1].StartsWith( "-" )))
                                    throw new ApplicationException(StringLoader.Get("Err_ValueRequired", strOpt));

                                strVal = aArgs[i+1];
                                i++;
                            }
                        }
                        else
                        {
                            // Value is in same command line
                            // arg as the option, substring
                            // it out.
                            strVal = aArgs[i].Substring(iColon + 1);
                        }
                    }

                    // Build the option value pair.
                    aOptList[iOpt++] = new Option(strOpt, strVal);
                }
                else
                {
                    // Command line word is a raw argument.
                    aArgList[iArg++] = aArgs[i];
                }
            }

            // Allocate the non-temporary arg and option lists at exactly
            // the right size.
            _aArgList = new String[iArg];
            _aOptList = new Option[iOpt];

            // Copy in the values we've calculated.
            Array.Copy(aArgList, _aArgList, iArg);
            Array.Copy(aOptList, _aOptList, iOpt);
        }

        public int NumArgs { get { return _aArgList.Length; } }

        public int NumOpts { get { return _aOptList.Length; } }

        public String GetNextArg()
        {
            if (_iArgCursor >= _aArgList.Length)
                return null;
            return _aArgList[_iArgCursor++];
        }

        public Option GetNextOption()
        {
            if (_iOptCursor >= _aOptList.Length)
                return null;
            return _aOptList[_iOptCursor++];
        }
    }

}
