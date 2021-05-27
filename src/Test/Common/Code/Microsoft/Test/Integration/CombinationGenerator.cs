// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Pict.ObjectArrays;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    public enum CombinationDegree
    {
        ///<summary>
        ///</summary>
        All,

        ///<summary>
        ///</summary>
        Tuple
    }

    /// <summary>
    /// Return true if you want to remove an item.
    /// </summary>
    /// <returns></returns>
    public delegate bool CombinationGeneratorFilterEventHandler(VariationItem vi, FilterCallback filterCallback);


    /// <summary>
    /// 
    /// </summary>
    public class FilterCallback
    {

        /// <summary>
        /// The method in this property will be call
        /// </summary>
        public StringCollection SupportInfo
        {
            get
            {
                return _supportInfo;
            }
            set
            {
                _supportInfo = value;
            }
        }

        /// <summary>
        /// The method in this property will be call
        /// </summary>
        [DefaultValue(null)]
        public MethodDesc Callback
        {
            get { return _callback; }
            set
            {
                if (value != null)
                {
                    value.ValidateState();
                }

                _callback = value;
            }
        }

        MethodDesc _callback = null;
        StringCollection _supportInfo = new StringCollection();
    }

    /// <summary>
    /// 
    /// </summary>
    public class CombinationGenerator : BaseVariationGenerator, IVariationGenerator
    {
        ///<summary>
        ///</summary>
        public CombinationDegree CombinerDegree
        {
            get { return _combinerDegree; }
            set { _combinerDegree = value; }
        }

        /// <summary>
        /// The method in this property will be call
        /// </summary>
        [DefaultValue(null)]
        public FilterCallback FilterCallback
        {
            get { return _filterMethod; }
            set
            {
                _filterMethod = value;

                // We are passed down a relative path from the XAML parser.  Convert to an absolute path.
                _filterMethod.SupportInfo[0] = TestExtenderHelper.QualifyPath(_filterMethod.SupportInfo[0]);
            }
        }

        #region IVariationGenerator Members

        ///<summary>
        ///</summary>
        public override List<VariationItem> Generate()
        {
            List<List<VariationItem>> variationListList = base.GenerateIVGChildrenSequential();
           
            if (CombinerDegree == CombinationDegree.Tuple && this.Dependencies.Count > 2)
            {
                bool pictFound = true;
                if (!File.Exists("pict.exe"))
                {
                    // HACK FOR VS Deployment.
                    if (File.Exists(@"Data\pict.exe"))
                    {
                        File.Copy(@"Data\pict.exe", "pict.exe", true);
                    }
                    if (File.Exists(@"Common\pict.exe"))
                    {
                        File.Copy(@"Common\pict.exe", "pict.exe", true);
                    }
                    else
                    {
                        pictFound = false;
                    }
                }

                if (!pictFound)
                {
                    throw new InvalidOperationException("The file pict.exe is not found in the current directory. " + Directory.GetCurrentDirectory());
                }

                int countVariationList = variationListList.Count;

                object o = typeof(ObjectArrayGenerator).InvokeMember("GeneratePairwiseObjectArrays", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                   null, typeof(ObjectArrayGenerator), variationListList.ToArray());

                IObjectArrayCollection oCollection = (IObjectArrayCollection)o;

                List<VariationItem> list = new List<VariationItem>();

                foreach (object[] x in oCollection)
                {
                    VariationItemGroup vGroup = new VariationItemGroup();
                    vGroup.Creator = typeof(CombinationGenerator).Name;
                    vGroup.Merge(this);

                    for (int i = 0; i < countVariationList; i++)
                    {
                        VariationItem item = (VariationItem)x[i];
                        if (!FilterVariationItem(item))
                        {
                            vGroup.Merge(item);
                            vGroup.Children.Add(item);
                        }
                    }

                    list.Add(vGroup);
                }

                return list;
            }
            else
            {                                
                if (variationListList.Count == 1)
                {
                    return variationListList[0];
                }

                if (variationListList.Count > 1)
                {
                    List<VariationItem> variationList = variationListList[0];

                    for (int i = 1; i < variationListList.Count; i++)
                    {
                        variationList = Combine(this, variationList, variationListList[i]);
                    }

                    return variationList;
                }                   
            }

            return new List<VariationItem>() ;
        }


        bool FilterVariationItem(VariationItem vi)
        {
            bool filter = false;

            if (FilterCallback != null)
            {
                object cache = null;
                Delegate callback = FilterCallback.Callback.GetDelegate(typeof(CombinationGeneratorFilterEventHandler), ref cache, true);

                if (callback != null)
                {
                    filter = (bool)callback.DynamicInvoke(vi,FilterCallback);
                }

            }

            return filter;
        }

        private List<VariationItem> Combine(CombinationGenerator cg, List<VariationItem> viList1, List<VariationItem> viList2)
        {
            List<VariationItem> vList = new List<VariationItem>();

            foreach (VariationItem v1 in viList1)
            {
                foreach (VariationItem v2 in viList2)
                {
                    VariationItemGroup vi = new VariationItemGroup();
                    vi.Creator = typeof(CombinationGenerator).Name;
                    vi.Merge(this);

                    vi.Merge(v1);
                    vi.Children.Add(v1);

                    vi.Merge(v2);
                    vi.Children.Add(v2);

                    if (!cg.FilterVariationItem(vi))
                    {
                        vList.Add(vi);
                    }
                }
            }

            return vList;
        }

        #endregion

        private FilterCallback _filterMethod = null;
        private CombinationDegree _combinerDegree = CombinationDegree.Tuple;
    }
}
