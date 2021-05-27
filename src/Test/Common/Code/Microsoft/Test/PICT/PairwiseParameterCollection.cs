// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Xml.Serialization;
    using System.Collections;
    //

    // PairwiseParameterCollection
    sealed class PairwiseParameterCollection : CollectionBase
    {

        public PairwiseParameter this[string name]
        {
            get
            {
                foreach (PairwiseParameter p in this.InnerList)
                {
                    if (p.Name == name)
                    {
                        return p;
                    }
                }

                throw new ArgumentOutOfRangeException("name", name, name + " not found.");
            }
        }

        public PairwiseParameter this[int index]
        {
            get
            {
                return (PairwiseParameter)InnerList[index];
            }
        }
        public PairwiseParameterCollection() : base()
        {
        }

        public PairwiseParameterCollection(params PairwiseParameter[] parameters)
        {
            this.InnerList.AddRange(parameters);
        }

        public PairwiseParameterCollection(PairwiseParameterCollection copy)
        {
            this.InnerList.AddRange(copy.InnerList);
        }

        public void Add(PairwiseParameter first, params PairwiseParameter[] parameters)
        {
            this.Add(first);
            this.InnerList.AddRange(parameters);
        }

        public int Add(PairwiseParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            return InnerList.Add(parameter);
        }

        public bool Contains(PairwiseParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            return InnerList.Contains(parameter);
        }

        public void CopyTo(PairwiseParameter[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        public int IndexOf(PairwiseParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            return InnerList.IndexOf(parameter);
        }

        public void Insert(int index, PairwiseParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            InnerList.Insert(index, parameter);
        }

        protected override void OnValidate(object value)
        {
            base.OnValidate(value);

            PairwiseParameter parameter = value as PairwiseParameter;

            if (parameter == null)
            {
                throw new ArgumentException("PairwiseParameter");
            }
        }

        public void Remove(PairwiseParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            InnerList.Remove(parameter);
        }
    }
    //

}
