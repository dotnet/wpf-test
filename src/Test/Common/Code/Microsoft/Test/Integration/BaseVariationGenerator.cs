// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.ComponentModel;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// This class is not a IVariationGenerator, but gives the 
    /// the common implementation for all the built-in IVariationGenerators
    /// It is not a requiere to inherit from this class, it is just a helper.
    /// </summary>
    [RuntimeNamePropertyAttribute("Name")]
    [ContentProperty("Dependencies")]
    abstract public class BaseVariationGenerator : TestContract
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseVariationGenerator() 
        {            
            Dependencies.CollectionChanged += new EventHandler(Dependencies_CollectionChanged);
        }

        /// <summary>
        /// This should be called at the end.
        /// </summary>
        /// <returns></returns>
        public abstract List<VariationItem> Generate();


        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IVariationGeneratorCollection Dependencies
        {
            get 
            {
                return DependencyList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetPriorityLimit(int priority)
        {
            if (priority < -1)
            {
                throw new ArgumentException("Priority need to be >= -1");
            }

            _priorityLimit = priority;
        }

        ///<summary>
        ///</summary>
        public int PriorityLimit
        {
            get
            {
                return _priorityLimit;
            }
        }

        private ITestContract _defaultTestContract = new TestContract();
        
        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITestContract DefaultTestContract
        {
            get { return _defaultTestContract; }
            internal set { _defaultTestContract = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected List<List<VariationItem>> GenerateIVGChildrenSequential( )
        {            

            if (_listListVariationItemCache == null)
            {
                List<List<VariationItem>> variationListList = new List<List<VariationItem>>();

                foreach (IVariationGenerator v in DependencyList)
                {
                    if (v.PriorityLimit == -1 || v.PriorityLimit <= this.PriorityLimit)
                    {
                        variationListList.Add(v.Generate());
                    }
                }
                _listListVariationItemCache = variationListList;
            }

            return _listListVariationItemCache;
        }

        

        /// <summary>
        /// 
        /// </summary>
        protected IVariationGeneratorCollection DependencyList = new IVariationGeneratorCollection();


        /// <summary>
        /// 
        /// </summary>
        protected virtual void InvalidateCache()
        {
            _listListVariationItemCache = null;
            
        }

        void Dependencies_CollectionChanged(object sender, EventArgs e)
        {
            InvalidateCache();
        }
        
        private List<List<VariationItem>> _listListVariationItemCache = null;
        private string _name = "";
        private int _priorityLimit = -1;
    }
}
