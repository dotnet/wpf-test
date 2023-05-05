// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class Range
    {
        private char _begin;

        public char Begin
        {
            get { return _begin; }
            set { _begin = value; }
        }

        private char _end;

        public char End
        {
            get { return _end; }
            set { _end = value; }
        }

        public int Count
        {
            get { return (this._end - this._begin) + 1; }
        }

        public Range(char begin, char end)
        {
            this._begin = begin;
            this._end = end;
        }

        public char GetCharInIndex(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new InvalidOperationException("Invalid index for this range");
            }
            char c = _begin;
            for (int i = 0; i < index; i++)
            {
                c++;
            }

            return c;
        }
    }
}
