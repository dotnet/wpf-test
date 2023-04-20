// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Test.Baml.Utilities
{
    public class BamlRecord
    {
        public BamlRecord(BamlRecordType recordId, bool isVariableSize, BamlField[] fields)
        {
            if(null == fields)
            {
                throw new ArgumentNullException("fields");
            }
            if(recordId <= BamlRecordType.Unknown || recordId >= BamlRecordType.LastRecordType)
            {
                throw new ArgumentOutOfRangeException("BamlRecordType recordId");
            }
            _recordId = recordId;
            _isVariableSize = isVariableSize;
            _fields = fields;
        }

        public BamlRecord Clone()
        {
            return new BamlRecord( _recordId, _isVariableSize, (BamlField[])_fields.Clone());
        }

        public BamlRecordType Id
        {
            get { return _recordId; }
        }

        public bool IsVariableSize
        {
            get { return _isVariableSize; }
        }

        public BamlField[] Fields
        {
            get { return _fields; }
        }

        private BamlRecordType _recordId;
        private bool _isVariableSize;
        private BamlField[] _fields;
    }

    
}

