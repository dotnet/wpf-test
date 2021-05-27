// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Pict.ObjectArrays
{
    using System;
    using System.Collections;

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    struct AllPossibleObjectArrayEnumerator : IObjectArrayEnumerator, IDisposable
    {
        object[] cur;

        readonly IEnumerator[] enumerators;

        readonly int lenMinusOne;

        bool rebuildRequired;

        bool done;

        public AllPossibleObjectArrayEnumerator(params IEnumerable[] enumerables)
        {
            done = false;
            this.enumerators = new IEnumerator[enumerables.Length];
            for (int i = 0; i < enumerables.Length; ++i)
            {
                this.enumerators[i] = enumerables[i].GetEnumerator(); // disposed
            }

            this.cur = null;
            this.rebuildRequired = true;
            this.lenMinusOne = enumerables.Length - 1;
            this.Reset();
        }

        public object[] Current
        {
            get
            {
                return GetCurrent();
            }
        }

        object[] GetCurrent()
        {
            if (rebuildRequired)
            {
                cur = new object[enumerators.Length];
                for (int i = 0; i < enumerators.Length; ++i)
                {
                    cur[i] = enumerators[i].Current;
                }

                rebuildRequired = false;
            }

            return cur;
        }

        bool Inc(int location)
        {
            if (done)
            {
                return false;
            }

            if (location == -1)
            {
                done = true;
                return false;
            }

            IEnumerator whereE = enumerators[location];

            if (!whereE.MoveNext())
            {
                if (location == 0)
                {
                    done = true;
                    return false;
                }

                whereE.Reset();
                if (!whereE.MoveNext())
                {
                    done = true;
                    return false;
                }

                --location;
                return Inc(location);
            }

            return true;
        }

        public bool MoveNext()
        {
            rebuildRequired = true;
            return Inc(lenMinusOne);
        }

        public void Reset()
        {
            for (int i = 0; i < enumerators.Length - 1; ++i)
            {
                enumerators[i].Reset();
                if (!enumerators[i].MoveNext())
                {
                    done = true;
                }
            }

            if (lenMinusOne != -1)
            {
                enumerators[lenMinusOne].Reset();
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return GetCurrent();
            }
        }

        void IDisposable.Dispose()
        {
            for (int i = 0; i < enumerators.Length; ++i)
            {
                IDisposable d = enumerators[i] as IDisposable;

                if (d != null)
                {
                    d.Dispose();
                }
            }
        }
    }
}

