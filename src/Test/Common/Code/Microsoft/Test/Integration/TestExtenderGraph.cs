// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProperty("Generators")]
    public class TestExtenderGraph : TestContract, INameScope
    {

        ///<summary>
        ///</summary>        
        static public TestExtenderGraph Load(string txrGraph)
        {
            object o = null;
            
            using (FileStream fs = new FileStream(txrGraph, FileMode.Open, FileAccess.Read))
            {
                o = Load(fs);
            }

            return o as TestExtenderGraph;
        }
        
        ///<summary>
        ///</summary>        
        static public TestExtenderGraph Load(Stream txrGraph)
        {
            object o = XamlReader.Load(txrGraph);

            return o as TestExtenderGraph;
        }


        ///<summary>
        ///</summary>
        static public TestExtenderOutput Generate(string txrGraph)
        {
            return Load(txrGraph).Generate();
        }

        ///<summary>
        ///</summary>
        static public TestExtenderOutput Generate(Stream txrGraph)
        {
            object o = XamlReader.Load(txrGraph);

            return ((TestExtenderGraph)o).Generate() ;
        }

        ///<summary>
        ///</summary>
        public TestExtenderGraph()
        {
            _vgCollection = new IVariationGeneratorCollection();
            _vgCollection.CollectionChanged += new EventHandler(_vgCollection_CollectionChanged);
            _currentTXR = this;
        }

        ///<summary>
        ///</summary>
        public TestExtenderOutput Generate()
        {
            TestExtenderOutput txro = new TestExtenderOutput();

            Stopwatch timer = Stopwatch.StartNew();
            
            CalculateDependenciesAndRoots();
                       
            for (int i = 0; i < RootGenerators.Length; i++)
            {                
                TestExtenderOutput.MergeTEO(txro, Generate(i));                
            }

            timer.Stop();
            GlobalLog.LogDebug("Generation Total Time: " + timer.Elapsed.TotalSeconds.ToString());

            return txro;
        }

        ///<summary>
        ///</summary>
        public TestExtenderOutput Generate(int rootIndex)
        {
            TestExtenderOutput txro = new TestExtenderOutput();

            Stopwatch timer = Stopwatch.StartNew();

            CalculateDependenciesAndRoots();

            if (rootIndex >= this.Generators.Count || rootIndex < 0)
            {
                throw new ArgumentOutOfRangeException("rootIndex");
            }
            
            IVariationGenerator vg = this.RootGenerators[rootIndex];

            foreach (VariationItem item in vg.Generate())
            {
                item.IsRoot = true;
                txro.TestCases.Add(item);
            }

            timer.Stop();
            GlobalLog.LogDebug("Generation Time " + rootIndex.ToString() + " : " + timer.Elapsed.TotalSeconds.ToString());

            return txro;
        }


        ///<summary>
        ///</summary>
        internal static TestExtenderGraph Current
        {
            get
            {
                return _currentTXR;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IVariationGeneratorCollection Generators
        {
            get
            {
                return _vgCollection;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IVariationGenerator[] RootGenerators
        {
            get
            {
                CalculateDependenciesAndRoots();
                return _vgRoots.ToArray();
            }
        }

        private void CalculateDependenciesAndRoots()
        {
            if (!_invalidStateInputOutput)
            {
                return;
            }

            _invalidStateInputOutput = false;

            IVariationGeneratorCollection possibleRoots = new IVariationGeneratorCollection();

            foreach (IVariationGenerator vg in Generators)
            {
                if (possibleRoots.Contains(vg))
                {
                    throw new InvalidOperationException("Cannot have the same IVariationGenerator twice as child of the TXR.");
                }

                possibleRoots.Add(vg);

                FindAndRemoveFalseRoots(possibleRoots, vg);
            }

            _vgRoots = possibleRoots;
        }

        private static void FindAndRemoveFalseRoots(IVariationGeneratorCollection possibleRoots, IVariationGenerator vg)
        {
            foreach (IVariationGenerator childVG in vg.Dependencies)
            {
                if (possibleRoots.Contains(childVG))
                {
                    possibleRoots.Remove(childVG);
                }

                FindAndRemoveFalseRoots(possibleRoots, childVG);
            }
        }

        void _vgCollection_CollectionChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Invalidate()
        {            
            _invalidStateInputOutput = true;            
        }


        IVariationGeneratorCollection _vgCollection = null;
        IVariationGeneratorCollection _vgRoots = new IVariationGeneratorCollection();        
        static TestExtenderGraph _currentTXR;        
        bool _invalidStateInputOutput = true;

        #region INameScope Members

        ///<summary>
        ///</summary>
        public object FindName(string name)
        {
            return _hash[name];
        }

        ///<summary>
        ///</summary>
        public void RegisterName(string name, object scopedElement)
        {
            _hash[name] = scopedElement;
        }

        ///<summary>
        ///</summary>
        public void UnregisterName(string name)
        {
            _hash.Remove(name);
        }

        Dictionary<string, object> _hash = new Dictionary<string, object>();

        #endregion
    }
}
