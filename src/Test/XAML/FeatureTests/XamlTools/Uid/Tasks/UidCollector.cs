// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using MS.Utility;                   // For SR
using MS.Internal.Tasks;

// Since we disable PreSharp warnings in this file, we first need to disable warnings about unknown message numbers and unknown pragmas.
#pragma warning disable 1634, 1691

namespace Microsoft.Build.Tasks.Windows.Demos
{

    // a class collects all the information about Uids per file
    internal sealed class UidCollector
    {
        public UidCollector(string fileName)
        {
            _uids = new List<Uid>(32);
            _namespacePrefixes = new List<string>(2);
            _uidTable = new Hashtable();
            _fileName = fileName;
            _sequenceMaxIds = new Hashtable();
        }

        // remembering all the namespace prefixes in the file
        // in case we need to add a new definition namespace declaration
        public void AddNamespacePrefix(string prefix)
        {
            _namespacePrefixes.Add(prefix);
        }

        // add the uid to the collector
        public void AddUid(Uid uid)
        {
            _uids.Add(uid);

            // set the uid status according to the raw data
            if (uid.Value == null)
            {
                uid.Status = UidStatus.Absent;
            }
            else if (_uidTable.Contains(uid.Value))
            {
                uid.Status = UidStatus.Duplicate;
            }
            else
            {
                // valid uid, store it
                StoreUid(uid.Value);
            }
        }

        public void ResolveUidErrors()
        {
            for (int i = 0; i < _uids.Count; i++)
            {
                Uid currentUid = _uids[i];

                if (currentUid.Status == UidStatus.Absent
                  && currentUid.NamespacePrefix == null
                  && _namespacePrefixForMissingUid == null)
                {
                    // there is Uid not in scope of any definition namespace
                    // we will need to generate a new namespace prefix for them
                    _namespacePrefixForMissingUid = GeneratePrefix();
                }

                if (currentUid.Status != UidStatus.Valid)
                {
                    // resolve invalid uids
                    currentUid.Value = GetAvailableUid(currentUid);
                }
            }
        }

        public int RootElementLineNumber
        {
            get { return _rootElementLineNumber; }
            set { _rootElementLineNumber = value; }
        }

        public int RootElementLinePosition
        {
            get { return _rootElementLinePosition; }
            set { _rootElementLinePosition = value; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public Uid this[int index]
        {
            get
            {
                return _uids[index];
            }
        }

        public int Count
        {
            get { return _uids.Count; }
        }

        public string NamespaceAddedForMissingUid
        {
            get { return _namespacePrefixForMissingUid; }
        }


        //-------------------------------------
        // Private methods
        //-------------------------------------
        private void StoreUid(string value)
        {
            // we just want to check for existence, so storing a null
            _uidTable[value] = null;

            string uidSequence;
            Int64 index;

            ParseUid(value, out uidSequence, out index);
            if (uidSequence != null)
            {
                if (_sequenceMaxIds.Contains(uidSequence))
                {
                    Int64 maxIndex = (Int64)_sequenceMaxIds[uidSequence];
                    if (maxIndex < index)
                    {
                        _sequenceMaxIds[uidSequence] = index;
                    }
                }
                else
                {
                    _sequenceMaxIds[uidSequence] = index;
                }
            }
        }

        private string GetAvailableUid(Uid uid)
        {
            string availableUid;

            // copy the ID if available
            if (uid.FrameworkElementName != null
             && (!_uidTable.Contains(uid.FrameworkElementName))
             )
            {
                availableUid = uid.FrameworkElementName;
            }
            else
            {
                // generate a new id
                string sequence = GetElementLocalName(uid.ElementName);
                Int64 index;

                if (_sequenceMaxIds.Contains(sequence))
                {
                    index = (Int64)_sequenceMaxIds[sequence];

                    if (index == Int64.MaxValue)
                    {
                        // this sequence reaches the max
                        // we fallback to create a new sequence
                        index = -1;
                        while (index < 0)
                        {
                            sequence = (_uidSequenceFallbackCount == 0) ?
                                UidFallbackSequence
                              : UidFallbackSequence + _uidSequenceFallbackCount;

                            if (_sequenceMaxIds.Contains(sequence))
                            {
                                index = (Int64)_sequenceMaxIds[sequence];
                                if (index < Int64.MaxValue)
                                {
                                    // found the fallback sequence with valid index
                                    index++;
                                    break;
                                }
                            }
                            else
                            {
                                // create a new sequence from 1
                                index = 1;
                                break;
                            }

                            _uidSequenceFallbackCount++;
                        }
                    }
                    else
                    {
                        index++;
                    }
                }
                else
                {
                    // a new sequence
                    index = 1;
                }

                availableUid = sequence + UidSeparator + index;
            }

            // store the uid so that it won't be used again
            StoreUid(availableUid);
            return availableUid;
        }

        private void ParseUid(string uid, out string prefix, out Int64 index)
        {
            // set prefix and index to invalid values
            prefix = null;
            index = -1;

            if (uid == null) return;

            int separatorIndex = uid.LastIndexOf(UidSeparator);
            if (separatorIndex > 0)
            {
                string suffix = uid.Substring(separatorIndex + 1);

                // Disable Presharp warning 6502 : catch block shouldn't have empty body
#pragma warning disable 6502
                try
                {
                    index = Int64.Parse(suffix, UidManager2.InvariantEnglishUS);
                    prefix = uid.Substring(0, separatorIndex);
                }
                catch (FormatException)
                {
                    // wrong format
                }
                catch (OverflowException)
                {
                    // not acceptable uid
                }
#pragma warning restore 6502
            }
        }

        private string GetElementLocalName(string typeFullName)
        {
            int index = typeFullName.LastIndexOf('.');
            if (index > 0)
            {
                return typeFullName.Substring(index + 1);
            }
            else
            {
                return typeFullName;
            }
        }

        private string GeneratePrefix()
        {
            Int64 ext = 1;
            string prefix = UidNamespaceAbbreviation.ToString(UidManager2.InvariantEnglishUS);

            // Disable Presharp warning 6502 : catch block shouldn't have empty body
#pragma warning disable 6502
            try
            {
                // find a prefix that is not used in the Xaml
                // from x1, x2, ... x[n]
                while (_namespacePrefixes.Contains(prefix))
                {
                    prefix = UidNamespaceAbbreviation + ext.ToString(UidManager2.InvariantEnglishUS);
                    ext++;
                }
                return prefix;
            }
            catch (OverflowException)
            {
            }
#pragma warning restore 6502

            // if overflows, (extreamly imposible), we will return a guid as the prefix
            return Guid.NewGuid().ToString();
        }

        private List<Uid> _uids;
        private Hashtable _uidTable;
        private string _fileName;
        private Hashtable _sequenceMaxIds;
        private List<string> _namespacePrefixes;
        private int _rootElementLineNumber = -1;
        private int _rootElementLinePosition = -1;
        private string _namespacePrefixForMissingUid = null;
        private int _uidSequenceFallbackCount = 0;

        private const char UidNamespaceAbbreviation = 'x';
        private const char UidSeparator = '_';
        private const string UidFallbackSequence = "_Uid";
    }
}
