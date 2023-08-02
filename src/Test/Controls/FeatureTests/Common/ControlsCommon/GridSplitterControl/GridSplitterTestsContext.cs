using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls.Helpers
{
    /// <summary>
    /// This delegate signature is the published interface for
    /// all "Action" methods in the test-case specification data
    /// </summary>
    public delegate void GridSplitterActionDelegate(
         out GridDefinitionSnapshot snapBefore,
         out GridDefinitionSnapshot snapAfter,
         Grid grid,
         GridSplitter splitter,
         Object[] actionArgs);
    
    /// <summary>
    /// This delegate signature is the published interface for
    /// all "Verify" methods in the test-case specification data
    /// </summary>
    public delegate TestResult GridSplitterVerifyDelegate(
         Int32 variationIndex,
         Grid grid,
         GridSplitter splitter,
         GridSplitterActionDelegate action,
         Object[] actionArgs,
         Object[] verifyArgs);

    /// <summary>
    /// Instances of this class represent a single test-case.
    ///  ----------------------------------------------------------------------
    /// This class is a shallow adaptor over a "Dictionary<String, Object>"
    /// which represents a test-case instance specification.
    /// This class exposes methods that better serve the needs of GridSplitter
    /// test architecture, and embody knowledge of special values like "Action"
    ///  and "Verify" that reference delegate instances.
    /// </summary>
    public class GridSplitterVariationContext
    {
        private Dictionary<String, Object> variation;

        public GridSplitterVariationContext(Dictionary<String, Object> variationData)
        {
            variation = variationData;
        }

        public GridSplitterActionDelegate Action
        {
            get
            { 
                Object obj;
                return (variation.TryGetValue("Action",out obj))
                       ? obj as GridSplitterActionDelegate : null;
            }
        }

        public Object[] ActionArgs
        {
            get
            {
                Object obj;
                return (variation.TryGetValue("ActionArgs", out obj))
                        ? obj as Object[]: new Object[0]; 
            }
        }

        public GridSplitterVerifyDelegate Verify
        {
            get
            {
                Object obj;
                return (variation.TryGetValue("Verify", out obj))
                    ? obj as GridSplitterVerifyDelegate : null;
            }
        }

        public Object[] VerifyArgs
        {
            get
            {
                Object obj;
                return (variation.TryGetValue("VerifyArgs", out obj))
                    ? obj as Object[] : new Object[0]; ; 
            }
        }

        public Object this[String key]
        {
            get
            {
                if ( String.IsNullOrEmpty(key)) return null;
                Object obj;
                return variation.TryGetValue(key, out obj) ? obj : null;
            }
        }

        public String[] Keys
        {
            get
            {
                String[] keys = new String[variation.Keys.Count];
                variation.Keys.CopyTo(keys, 0);
                return keys;
            }
        }
    }
}
