// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a simple engine to create different combination of settings for test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/Combinatorial.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Net;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Xml;

    using Test.Uis.Loggers;
    using Test.Uis.Management;

    #endregion Namespaces.

    /// <summary>
    /// Provides an engine to create all possible combinations for
    /// different values across different dimensions.
    /// </summary>
    /// <remarks>
    /// The following describes the configuration syntax.<code>
    /// [Combinations]
    ///   [Dimension Name='']
    ///     [Value (Filter='') (Text='')>(text)]</code>
    /// </remarks>
    public class CombinatorialEngine
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new CombinatorialEngine from the specified
        /// dimensions.
        /// </summary>
        /// <param name='dimensions'>Dimensions to use in engine.</param>
        /// <returns>A new CombinatorialEngine instance.</returns>
        private CombinatorialEngine(Dimension[] dimensions)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException("dimensions");
            }
            if (dimensions.Length == 0)
            {
                throw new ArgumentException("Dimensions cannot be empty.");
            }
            if (Array.IndexOf(dimensions, null) != -1)
            {
                throw new ArgumentException("Dimensions cannot have null elements.");
            }

            SetupForDimensions(dimensions);
        }

        /// <summary>Creates a new CombinatorialEngine instance.</summary>
        /// <param name='reader'>XmlReader over specification.</param>
        /// <param name='testName'>Test name scoping Combinations tag.</param>
        private CombinatorialEngine(XmlReader reader, string testName)
        {
            if (testName == null) testName = String.Empty;

            // Read in values.
            ArrayList dimensionsList = new ArrayList();
            ArrayList valuesList = new ArrayList();
            ArrayList filtersList = new ArrayList();
            string dimensionName = null;
            Stack elementNameStack = new Stack();
            string lastElementName = String.Empty;
            bool inCombinations = false;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        // Combinations elements under a testName node
                        if (reader.Name == "Combinations" &&
                            ((lastElementName == testName) || (testName == String.Empty)))
                        {
                            inCombinations = true;
                        }
                        else if (!inCombinations)
                        {
                            if (!reader.IsEmptyElement)
                            {
                                lastElementName = reader.Name;
                                elementNameStack.Push(lastElementName);
                            }
                            break;
                        }
                        else if (reader.Name == "Dimension")
                        {
                            dimensionName = reader.GetAttribute("Name");
                        }
                        else if (reader.Name == "Value")
                        {
                            string filter = reader.GetAttribute("Filter");
                            string text = reader.GetAttribute("Text");
                            if (text == null)
                            {
                                reader.Read();
                                if (reader.NodeType != XmlNodeType.Text ||
                                    reader.NodeType != XmlNodeType.CDATA)
                                    throw new Exception(
                                        "Text expected after value in dimension " +
                                        dimensionName + " with no Text attribute.");
                                text = reader.Value;
                            }
                            valuesList.Add(text);
                            filtersList.Add(filter);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (!inCombinations)
                        {
                            elementNameStack.Pop();
                            if (elementNameStack.Count == 0)
                                lastElementName = String.Empty;
                            else
                                lastElementName = (string) elementNameStack.Peek();
                            break;
                        }

                        // Finish reading when the Combinations element is closed.
                        if (reader.Name == "Combinations")
                            goto FinishedReader;

                        // Add dimension data when the Dimension element is closed.
                        if (reader.Name == "Dimension")
                        {
                            Dimension d = new Dimension(dimensionName,
                                (string[]) valuesList.ToArray(typeof(string)),
                                (string[]) filtersList.ToArray(typeof(string)));
                            dimensionsList.Add(d);
                            valuesList.Clear();
                            filtersList.Clear();
                            dimensionName = null;
                        }
                        break;
                }
            }

            // Label to break out of the reader loop when the closing element
            // is found.
            FinishedReader:

            SetupForDimensions((Dimension[])dimensionsList.ToArray(typeof(Dimension)));
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Initializes a new CombinatorialEngine from the specified
        /// dimensions.
        /// </summary>
        /// <param name='dimensions'>Dimensions to use in engine.</param>
        /// <returns>A new CombinatorialEngine instance.</returns>
        public static CombinatorialEngine FromDimensions(Dimension[] dimensions)
        {
            return new CombinatorialEngine(dimensions);
        }

        /// <summary>
        /// Creates a new instance from an existing file.
        /// </summary>
        /// <param name='fileName'>File with combination specification.</param>
        /// <param name='testName'>Test name scoping Combinations tag.</param>
        /// <returns>A new CombinatorialEngine instance.</returns>
        /// <example>The follownig sample shows how to use this method.<code>...
        /// private void MyMethod() {
        ///   CombinatorialEngine engine = CombinatorialEngine.FromFile(
        ///     "combinations.xml", "MyTest");
        ///   Hashtable cs = ConfigurationSettings.Current.CloneValues();
        ///   while (engine.Next(cs)) {
        ///     /* do something with combinations in cs */
        ///   }
        /// }</code></example>
        public static CombinatorialEngine FromFile(string fileName, string testName)
        {
            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            XmlTextReader reader = new XmlTextReader(fileName);
            try
            {
                return new CombinatorialEngine(reader, testName);
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Creates a new instance from a string specification.
        /// </summary>
        /// <param name='specification'>Combination specification.</param>
        /// <returns>A new CombinatorialEngine instance.</returns>
        public static CombinatorialEngine FromString(string specification)
        {
            return new CombinatorialEngine(
                new XmlTextReader(new StringReader(specification)),
                String.Empty);
        }

        /// <summary>
        /// Creates a new instance from an XmlReader over a specification.
        /// </summary>
        /// <param name='reader'>Reader over a combination specification.</param>
        /// <param name='testName'>Test name scoping Combinations tag.</param>
        /// <returns>A new CombinatorialEngine instance.</returns>
        public static CombinatorialEngine FromXmlReader(XmlReader reader, string testName)
        {
            return new CombinatorialEngine(reader, String.Empty);
        }

        /// <summary>
        /// Returns a description of the value for each dimension.
        /// </summary>
        /// <returns>
        /// A multiline description, [none] if the engine has not yet
        /// started to generate states.
        /// </returns>
        /// <example>The follownig sample shows how to use this method.<code>...
        /// private void RunAllStates(CombinatorialEngine engine, ConfigurationSettings cs) {
        ///   while (engine.Next(cs)) {
        ///     Logger.Current.Log("Combination settings: " + engine.DescribeState());
        ///     /* do something with combinations in cs */
        ///   }
        /// }</code></example>
        public string DescribeState()
        {
            StringBuilder sb;   // Result of method.

            if (_beforeStart)
                return "[none]";

            sb = new StringBuilder(_dimensions.Length * 64);
            for (int i = 0; i < _dimensions.Length; i++)
            {
                sb.Append(_dimensions[i].Name);
                sb.Append(": [");
                sb.Append(_dimensions[i].Values[_valueIndexes[i]]);
                sb.Append("]");
                sb.Append(System.Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>Whether there the engine has any content.</summary>
        public bool IsEmpty
        {
            get { return _dimensions.Length > 0; }
        }

        /// <summary>
        /// Sets the next set of values in the specified hash table.
        /// </summary>
        /// <param name='cs'>Hashtable with values to modify.</param>
        /// <returns>
        /// false if there are no more valid combinations, true otherwise.
        /// </returns>
        public bool Next(Hashtable cs)
        {
            int incrementingDimension;  // Index of dimension being incremented.
            bool validFound;            // Whether a valid combination was found.
            bool candidatesPending;     // Whether there are still combination
                                        // candidates to be evaluated.
            Dimension d;                // Dimension being evaluated.
            int valueIndex;             // Value being considered for dimension.

            if (_dimensions.Length == 0) return false;

            incrementingDimension = 0;
            validFound = false;
            do
            {
                d = _dimensions[incrementingDimension];
                valueIndex = _valueIndexes[incrementingDimension];

                // Consider the special case of 'before-start'.
                if (!_beforeStart)
                {
                    valueIndex++;
                }
                _beforeStart = false;

                // Move to the next dimension if this one has no more values
                // to consider, otherwise verify acceptability.
                if (valueIndex == d.Values.Length)
                {
                    for (int i = incrementingDimension; i >= 0; i--)
                    {
                        _valueIndexes[i] = 0;
                    }
                    incrementingDimension++;
                }
                else
                {
                    _valueIndexes[incrementingDimension] = valueIndex;
                    if (GetFiltersAreAcceptable())
                    {
                        validFound = true;
                    }
                    else
                    {
                        incrementingDimension = 0;
                    }
                }
                candidatesPending = incrementingDimension < _dimensions.Length;
            } while (candidatesPending && !validFound);

            if (validFound)
            {
                PopulateWithDimensionValues(cs);
                return true;
            }
            else
                return false;
        }

        #endregion Public methods.

        
        /// <summary>Filtering event fired when combination is evalated.</summary>
        public FilteringEventHandler Filtering;

        
        #region Private methods.

        /// <summary>Evaluates current filters.</summary>
        /// <returns>
        /// true if the current combination of filters is valid, false
        /// otherwise.
        /// </returns>
        private bool GetFiltersAreAcceptable()
        {
            int valueIndex;         // Index of value for dimension.
            EvaluationNode node;    // Evaluation node.
            bool result;            // Expected result;
            FilteringEventArgs e;   // Arguments for filtering event.
            Hashtable table;        // Table for filtering.

            result = true;
            for (int i = 0; i < _dimensions.Length; i++)
            {
                valueIndex = _valueIndexes[i];
                node = _dimensions[i].FilterEvaluations[valueIndex];
                if (node != null && !node.Evaluate(this))
                {
                    result = false;
                    break;
                }
            }

            if (Filtering != null)
            {
                table = new Hashtable();
                PopulateWithDimensionValues(table);

                e = new FilteringEventArgs(table, result);
                Filtering(this, e);

                result = e.IsAcceptable;
            }

            return result;
        }

        /// <summary>Gets the index of the specified dimension.</summary>
        /// <param name='dimensionName'>Name of dimension sought.</param>
        /// <returns>The index of the dimension, throws an exception otherwise.</returns>
        private int FindDimensionIndexByName(string dimensionName)
        {
            for (int i = 0; i < _dimensions.Length; i++)
            {
                if (_dimensions[i].Name == dimensionName)
                    return i;
            }
            throw new Exception("Dimension " + dimensionName + " not found.");
        }

        /// <summary>Populates the table with current dimension values.</summary>
        private void PopulateWithDimensionValues(Hashtable table)
        {
            int valueIndex; // Index of value for a given dimension.

            System.Diagnostics.Debug.Assert(table != null);
            for (int i = 0; i < _dimensions.Length; i++)
            {
                valueIndex = _valueIndexes[i];
                table[_dimensions[i].Name] = _dimensions[i].Values[valueIndex];
            }
        }

        /// <summary>
        /// Sets up the engine to use the specified dimensions in a
        /// combinatorial run.
        /// </summary>
        private void SetupForDimensions(Dimension[] dimensions)
        {
            System.Diagnostics.Debug.Assert(dimensions != null);
            this._dimensions = dimensions;
            _valueIndexes = new int[dimensions.Length];
            for (int i=0; i < dimensions.Length; i++)
            {
                dimensions[i].CreateEvaluationForFilters(this);
            }
            _beforeStart = true;
        }

        #endregion Private methods.
 

        #region Private fields.

        /// <summary>Dimensions to combine.</summary>
        private Dimension[] _dimensions;

        /// <summary>Index of the current value of each dimension.</summary>
        private int[] _valueIndexes;

        /// <summary>Before enumeration started flag.</summary>
        private bool _beforeStart;

        #endregion Private fields.


        #region Inner classes.

        /// <summary>A node in the filter evaluation tree.</summary>
        internal abstract class EvaluationNode
        {
            #region Private data.

            /// <summary>Opening parenthesis symbol in stack.</summary>
            private static object s_openingParenthesis = new object();

            /// <summary>Closing parenthesis symbol in stack.</summary>
            private static object s_closingParenthesis = new object();

            #endregion Private data.

            #region Public methods.

            /// <summary>Evaluates the node to true or false.</summary>
            public abstract bool Evaluate(CombinatorialEngine engine);

            /// <summary>
            /// Creates an evaluation tree for the specified filter expression.
            /// </summary>
            /// <param name='engine'>Engine creating the filter.</param>
            /// <param name='filter'>Filter to evaluate.</param>
            /// <returns>The root node of the evaluation tree.</returns>
            public static EvaluationNode CreateEvaluationForFilter(
                CombinatorialEngine engine, string filter)
            {
                if (engine == null)
                {
                    throw new ArgumentNullException("engine");
                }
                if ((filter == null) || (filter.Length == 0))
                {
                    return null;
                }

                EvaluationNode node;        // General node.
                Stack stack = new Stack();  // Stack of pending evaluations.
                string symbol = "";         // Symbol (usu. variable) being parsed.
                bool symbolClosed = false;  // Whether we have parsed this symbol already.
                int i = 0;                  // Character index.

                while (i < filter.Length)
                {
                    char c = filter[i];
                    switch (c)
                    {
                        case '(':
                            // We mark an opening parenthesis with a special
                            // flag on the stack, to add an additional
                            // level when merging nodes into the tree.
                            MergeOnStack(s_openingParenthesis, stack);
                            i++;
                            break;

                        case ')':
                            MergeOnStack(s_closingParenthesis, stack);
                            i++;
                            break;

                        case '!':
                        case '=':
                            // We create a new comparison (leaf) node, and
                            // possibly merge it into any pending items.
                            // Because we only support binary operations, if
                            // it was merged, then we remove this item from
                            // the stack context.
                            node = ParseComparisonEvaluation(engine, filter, symbol, ref i);
                            MergeOnStack(node, stack);

                            // Reset the symbol used for this comparison.
                            symbol = "";
                            symbolClosed = false;
                            break;

                        case ' ':
                            // Make sure we don't add any more characters
                            // to a pending symbol, otherwise characters
                            // are ignored.
                            if (symbol.Length > 0)
                            {
                                symbolClosed = true;
                            }
                            i++;
                            break;

                        default:
                            // Argh. Unfortunately, using && is a pain, so
                            // we reserve AND and OR as keywords.
                            bool isAnd = (symbol.Length == 0 && c == 'A' &&
                                i + 2 < filter.Length &&
                                filter[i+1] == 'N' && filter[i+2] == 'D');
                            bool isOr = (symbol.Length == 0 && c == 'O' &&
                                i + 1 < filter.Length &&
                                filter[i+1] == 'R');
                            System.Diagnostics.Debug.Assert(!(isAnd && isOr));
                            if (isAnd) i += 3;
                            if (isOr) i += 2;
                            if (isAnd || isOr)
                            {
                                // We create a new logical node and assign the
                                // top expression to the left-hand side, then
                                // we set up this node on the stack to prepare
                                // it for merging with the right-hand side
                                // expression later on.
                                if (stack.Count == 0)
                                {
                                    throw new Exception(
                                        "There is no left-hand side expression " +
                                        "for the logical operation at position " +
                                        i + " in filter: " + filter);
                                }
                                if (stack.Peek() == s_openingParenthesis ||
                                    stack.Peek() == s_closingParenthesis)
                                {
                                    throw new Exception(
                                        "Logical operator cannot follow " +
                                        "parenthesis at position " + i +
                                        " in filter: " + filter);
                                }

                                node = (EvaluationNode) stack.Pop();

                                LogicalEvaluationNode eval = new LogicalEvaluationNode();
                                eval.Operation = (isAnd)?
                                    LogicalEvaluationNode.LogicalOperation.And :
                                    LogicalEvaluationNode.LogicalOperation.Or;
                                eval.Left = node;
                                stack.Push(eval);
                            }
                            else
                            {
                                // This is a symbol character.
                                if (symbolClosed)
                                {
                                    throw new Exception("The symbol [" + symbol +
                                        "] has been closed. Typically this means " +
                                        "whitespace has been added to it or the " +
                                        "filter is malformed: " + filter);
                                }
                                symbol += c;
                                i++;
                            }
                            break;
                    }
                }

                // After parsing, any trailing symbols are an error.
                if (symbol.Length > 0)
                {
                    throw new Exception("Trailing symbol [" + symbol + "] in filter " +
                        filter + " is invalid.");
                }
                if (stack.Count == 0)
                {
                    throw new Exception("Filter has no expressions: " + filter);
                }
                if (stack.Count > 1)
                {
                    throw new Exception(
                        "Filter is prematurely terminated: " + filter);
                }
                return (EvaluationNode) stack.Pop();
            }

            #endregion Public methods.

            #region Private methods.

            /// <summary>
            /// Merges a node to a pending node in the stack as appropriate.
            /// </summary>
            /// <param name='node'>Node to be merged.</param>
            /// <param name='stack'>Stack with pending nodes.</param>
            private static void MergeOnStack(object node, Stack stack)
            {
                // Easiest case, just add a new level.
                if (node == s_openingParenthesis)
                {
                    stack.Push(node);
                    return;
                }

                // Another trivial case - an evaluation node on
                // an empty stack.
                if (node is EvaluationNode &&
                    (stack.Count == 0 || stack.Peek() == s_openingParenthesis))
                {
                    stack.Push(node);
                    return;
                }

                // Close a parenthesis by getting the top node,
                // removing an opening parenthesis that must have
                // been opened, and merging that top into any pending
                // nodes.
                if (node == s_closingParenthesis)
                {
                    if (stack.Count < 2)
                    {
                        throw new Exception("Closing paren does not " +
                            "match opening paren.");
                    }
                    object o = stack.Pop();
                    if (o == s_openingParenthesis)
                    {
                        throw new Exception("Empty parens not allowed.");
                    }
                    if (stack.Pop() != s_openingParenthesis)
                    {
                        throw new Exception("Closing paren does not " +
                            "match opening paren.");
                    }
                    MergeOnStack(o, stack);
                    return;
                }

                System.Diagnostics.Debug.Assert(stack.Count > 0);
                System.Diagnostics.Debug.Assert(node is EvaluationNode);

                EvaluationNode evalNode = (EvaluationNode)node;
                object top = stack.Peek();
                System.Diagnostics.Debug.Assert(top != null);
                if (top is LogicalEvaluationNode)
                {
                    LogicalEvaluationNode logicalNode =
                        (LogicalEvaluationNode) stack.Pop();
                    System.Diagnostics.Debug.Assert(logicalNode.Left != null);
                    if (logicalNode.Right != null)
                    {
                        throw new Exception("Logical evaluation " +
                            "already has two operands.");
                    }
                    logicalNode.Right = evalNode;
                    MergeOnStack(logicalNode, stack);
                }
                else
                {
                    throw new Exception("Consecutive expressions in filter.");
                }
            }

            /// <summary>
            /// Parses a comparison operation into a ComparisonEvaluationNode.
            /// </summary>
            /// <remarks>
            /// The filter must be invoked with the parsing index on a '=' or
            /// a '!' character, right after parsing a symbol.
            /// </remarks>
            private static ComparisonEvaluationNode ParseComparisonEvaluation(
                CombinatorialEngine engine, string filter, string symbol, ref int i)
            {
                System.Diagnostics.Debug.Assert(filter != null);
                System.Diagnostics.Debug.Assert(filter.Length > 0);
                System.Diagnostics.Debug.Assert(
                    filter[i] == '!' || filter[i] == '=');

                if (symbol.Length == 0)
                {
                    throw new Exception("Filter has '=' at position " + i +
                        " without a previous symbol.");
                }

                ComparisonEvaluationNode result = new ComparisonEvaluationNode();
                if (filter[i] == '=')
                {
                    result.Operation = ComparisonEvaluationNode.ComparisonOperation.Equal;
                }
                else
                {
                    result.Operation = ComparisonEvaluationNode.ComparisonOperation.NotEqual;
                }
                i++;
                if (filter[i] != '=')
                {
                    throw new Exception(
                        "'!' should be followed by '=' at position " + i +
                        " in filter " + filter);
                }
                result.NameDimensionIndex = engine.FindDimensionIndexByName(symbol);
                string indexString = "";
                i++;
                while (i < filter.Length && Char.IsDigit(filter[i]))
                {
                    indexString += filter[i];
                    i++;
                }
                result.FilterIndex = Int32.Parse(indexString,
                    System.Globalization.CultureInfo.InvariantCulture);
                return result;
            }

            #endregion Private methods.
        }

        /// <summary>Evaluates AND and OR nodes.</summary>
        class LogicalEvaluationNode: EvaluationNode
        {
            #region Public data.

            /// <summary>Left-hand evaluation node.</summary>
            public EvaluationNode Left;

            /// <summary>Right-hand evaluation node.</summary>
            public EvaluationNode Right;

            /// <summary>Logical operation.</summary>
            public LogicalOperation Operation;

            public enum LogicalOperation
            {
                And, Or
            }

            #endregion Public data.

            #region Public methods.

            /// <summary>Evaluates a logical operation.</summary>
            public override bool Evaluate(CombinatorialEngine engine)
            {
                if (Left == null)
                {
                    throw new InvalidOperationException("Left expression is null");
                }
                if (Right == null)
                {
                    throw new InvalidOperationException("Right expression is null");
                }
                bool result;
                switch (Operation)
                {
                    case LogicalOperation.And:
                        result = Left.Evaluate(engine);
                        if (result) result = Right.Evaluate(engine);
                        return result;
                    case LogicalOperation.Or:
                        result = Left.Evaluate(engine);
                        if (!result) result = Right.Evaluate(engine);
                        return result;
                    default:
                        throw new InvalidOperationException("Unknown logical operation.");
                }
            }

            #endregion Public methods.
        }

        /// <summary>Evaluates EQUAL and NOT EQUAL nodes.</summary>
        class ComparisonEvaluationNode: EvaluationNode
        {
            #region Public data.

            /// <summary>Expected filter index value.</summary>
            public int FilterIndex;

            /// <summary>Dimension to which index applies.</summary>
            public int NameDimensionIndex;

            /// <summary>Comparison operation to perform.</summary>
            public ComparisonOperation Operation;

            public enum ComparisonOperation
            {
                Equal, NotEqual
            }

            #endregion Public data.

            #region Public methods.

            /// <summary>Evaluates a comparison operation.</summary>
            public override bool Evaluate(CombinatorialEngine engine)
            {
                bool isEqual =
                    engine._valueIndexes[NameDimensionIndex] == FilterIndex;
                return
                    (Operation == ComparisonOperation.Equal && isEqual) ||
                    (Operation == ComparisonOperation.NotEqual && !isEqual);
            }

            #endregion Public methods.
        }

        #endregion Inner classes.
    }

    /// <summary>Event arguments for combination state filtering.</summary>
    public class FilteringEventArgs: EventArgs
    {
        #region Constructors.

        /// <summary>Initializes a new FilteringEventArgs instance.</summary>
        internal FilteringEventArgs(Hashtable values, bool isAcceptable)
        {
            System.Diagnostics.Debug.Assert(values != null);

            this._values = values;
            this._isAcceptable = isAcceptable;
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>Whether the combination evaluated is acceptable.</summary>
        public bool IsAcceptable
        {
            get { return this._isAcceptable; }
            set { this._isAcceptable = value; }
        }

        /// <summary>Values being considered, indexed by dimension name.</summary>
        public Hashtable Values
        {
            get { return this._values; }
        }

        #endregion Public properties.

        #region Private fields.

        private bool _isAcceptable;
        private Hashtable _values;

        #endregion Private fields.
    }

    /// <summary>Filtering event.</summary>
    /// <param name='sender'>Object firing the event.</param>
    /// <param name='e'>Event arguments.</param>
    public delegate void FilteringEventHandler(object sender, FilteringEventArgs e);

    /// <summary>
    /// A dimension defines a named set of values, only one
    /// of which is selected at a time for specific combination.
    /// It is usually generated for a configuration setting.
    /// </summary>
    public class Dimension
    {
        #region Private data.

        /// <summary>Name of the dimension.</summary>
        private string _name;

        /// <summary>Possible values in this dimension.</summary>
        private object[] _values;

        /// <summary>Filter expressions.</summary>
        private string[] _filters;
        private CombinatorialEngine.EvaluationNode[] _filterRoots;

        #endregion Private data.

        #region Constructors.

        /// <summary>Initializes a new Dimension instance.</summary>
        public Dimension(string name, object[] values)
            :this(name, values, null)
        {
        }

        /// <summary>Initializes a new Dimension instance.</summary>
        public Dimension(string name, object[] values, string[] filters)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentException(
                    "No name defined for the dimension.", "name");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException(
                    "No values have been defined for dimension " + name,
                    "values");
            }
            if (filters != null && values.Length != filters.Length)
            {
                throw new ArgumentException(
                    "Count of filters different from values.", "filters");
            }

            this._name = name;
            this._values = values;
            this._filters = filters;
        }

        #endregion Constructors.

        #region Internal methods.

        /// <summary>
        /// Creates the evaluation filters for the given combinatorial
        /// engine.
        /// </summary>
        /// <param name='engine'>Engine with variables to reference.</param>
        internal void CreateEvaluationForFilters(CombinatorialEngine engine)
        {
            // If filters == null, then all of our filter roots will
            // be null to indicate that there are no filters for the
            // value.
            if (_filters == null)
            {
                _filterRoots = new CombinatorialEngine.EvaluationNode[_values.Length];
                return;
            }

            _filterRoots = new CombinatorialEngine.EvaluationNode[_filters.Length];
            for (int i = 0; i < _filters.Length; i++)
            {
                _filterRoots[i] = CombinatorialEngine.EvaluationNode.
                    CreateEvaluationForFilter(engine, _filters[i]);
            }
        }

        /// <summary>Sets the Values property.</summary>
        /// <param name="values">New value for the Values property.</param>
        /// <remarks>
        /// This is a dangerous method to call, as most clients of Dimension
        /// expect that the Values property be immutable.
        /// </remarks>
        internal void SetValues(object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            this._values = values;
        }

        /// <summary>Given a value, returns a dimension value identifier.</summary>
        /// <param name="value">Object to convert.</param>
        /// <returns></returns>
        internal static string ValueToString(object value)
        {
            string result;

            if (value is string && value.ToString().Length == 0)
            {
                result = "EMPTY_STRING";
            }
            else if (value is Enum)
            {
                result = value.ToString();
            }
            else if (value is int)
            {
                result = ((int)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (value is double)
            {
                result = ((double)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (value is IStringIdentifierSupport)
            {
                result = ((IStringIdentifierSupport)value).StringIdentifier;
            }
            else if (value != null)
            {
                result = value.ToString();
            }
            else
            {
                result = null;
            }

            if (result == null)
            {
                result = "NULL";
            }

            return result.Replace(' ', '_').Replace('\\', '_');
        }

        #endregion Internal methods.

        #region Public properties.

        /// <summary>Dimension name (the attribute).</summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>Possible values for dimension.</summary>
        public object[] Values
        {
            get { return this._values; }
        }

        /// <summary>Filters for values. null indicates no filter.</summary>
        public string[] Filters
        {
            get { return this._filters; }
        }

        #endregion Public properties.

        #region Internal properties.

        /// <summary>
        /// Root evaluation nodes for filters. null indicates no filter.
        /// </summary>
        internal CombinatorialEngine.EvaluationNode[] FilterEvaluations
        {
            get { return this._filterRoots; }
        }

        #endregion Internal properties.
    }
}
