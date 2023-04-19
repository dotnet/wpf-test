// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // [Serializable]
    /// <summary>
    /// This class keeps a list of NavigationStates
    /// and provides API to:
    ///    - add new states to the collection
    ///    - count number of states in the collection
    ///    - get and set individual states in the collection
    ///    - compare 2 different collections
    ///    - save a collection to a file
    ///    - load a collection from information saved in a file
    ///    - print out the contents of a collection
    /// </summary>
    public class NavigationStateCollection
    {
        /// <summary>
        /// List of saved states.
        /// </summary>
        /// <remarks>

        public List<NavigationState> states = new List<NavigationState>();

        /// <summary>
        /// Dummy constructor for NavigationStateCollection.
        /// </summary>
        public NavigationStateCollection()
        {
        }

        /// <summary>
        /// Gets number of states in the collection.
        /// </summary>
        public int Count
        {
            get { return states.Count; }
        }

        /// <summary>
        /// Gets and sets an individual state in the collection.
        /// </summary>
        /// <param name="index">Index of state.</param>
        /// <returns>NavigationState object.</returns>
        public NavigationState this[int index]
        {
            get
            {
                return states[index];
            }
            set
            {
                states[index] = value;
            }
        }

        /// <summary>
        /// Creates a new NavigationState for the current NavigationWindow/Frame
        /// after a navigation action is performed, and adds the state to the test's
        /// accumulated state list.
        /// </summary>
        /// <param name="navProvider">Usually a JournalHelper object that keeps track of journal state</param>
        public void RecordNewResult(IProvideJournalingState navProvider)
        {
            NavigationState b = new NavigationState();
            b.RecordNavigationState(navProvider);
            states.Add(b);
        }

        /// <summary>
        /// This creates a new NavigationState after a particular action is performed (such as 
        /// navigating to a URI) and adds the new state to the test's accumulated state list.
        /// </summary>
        /// <param name="navProvider">Usually a JournalHelper object that keeps track of journal state</param>
        /// <param name="stepDesc">String describing what action/state we're recording</param>
        public void RecordNewResult(IProvideJournalingState navProvider, String stepDesc)
        {
            NavigationState b = new NavigationState();
            b.RecordNavigationState(navProvider, stepDesc);
            states.Add(b);
        }

        /// <summary>
        /// This creates a new NavigationState after a particular action is performed (such as 
        /// navigating to a URI) and adds the new state to the test's accumulated state list.
        /// </summary>
        /// <param name="navProvider">Usually a JournalHelper object that keeps track of journal state</param>
        /// <param name="contentControl">Content (Frame) related to that journal is recorded</param>
        /// <param name="stepDesc">String describing what action/state we're recording</param>
        public void RecordNewResult(IProvideJournalingState navProvider, ContentControl contentControl, String stepDesc)
        {
            NavigationState navState = new NavigationState();
            navState.RecordNavigationState(navProvider, contentControl, stepDesc);
            states.Add(navState);
        }

        /// <summary>
        /// Log the NavigationStateCollection to an XML file
        /// </summary>
        /// <param name="filename">Name of file to write to</param>
        public void WriteResults(string filename)
        {
            if (!String.IsNullOrEmpty(filename))
            {
                FileStream output = File.Create(filename);
                WriteResults(output);
            }
        }

        /// <summary>
        /// Log the NavigationStateCollection to an XML file
        /// </summary>
        /// <param name="output">Stream to write results to</param>
        public void WriteResults(Stream output)
        {
            if (output != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(NavigationStateCollection));
                serializer.Serialize(output, this);
                output.Close();
            }
        }

        /// <summary>
        /// Loads a NavigationStateCollection object from information read from a file.
        /// </summary>
        /// <param name="input">Stream containing the expected results</param>
        /// <returns></returns>
        public static NavigationStateCollection GetResults(Stream input)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NavigationStateCollection));
            NavigationStateCollection expected = serializer.Deserialize(input) as NavigationStateCollection;
            return expected;
        }

        /// <summary>
        /// Set this value to true if you want to compare the description of the 
        /// actual states to expected states
        /// </summary>
        public static bool CompareDescription = false;

        /// <summary>
        /// Compares two NavigationStateCollection objects to see if they are equivalent.
        /// We compare the corresponding states at a particular index in the collection and
        /// a pass or fail depending on the outcome of the comparison.
        /// </summary>
        /// <param name="actual">NavigationStateCollection built by running a test case</param>
        /// <param name="expected">NavigationStateCollection built by loading expected results from file</param>
        /// <returns></returns>
        public static bool Compare(NavigationStateCollection actual, NavigationStateCollection expected)
        {
            if (actual.states.Count != expected.states.Count)
            {
                Log.Current.CurrentVariation.LogMessage("Comparison failed. Number of states do NOT match");
                Log.Current.CurrentVariation.LogMessage("EXPECTED # STATES: " + expected.states.Count + "; ACTUAL # STATES: " + actual.states.Count);
                NavigationHelper.CacheTestResult(Result.Fail);
                return false;
            }
            int nStates = actual.states.Count;
            bool match = true;
            for (int i = 0; i < nStates; i++)
            {
                NavigationState a = actual.states[i];
                NavigationState b = expected.states[i];

                // compare the description of the states
                if (CompareDescription == true)
                {
                    if (a.stateDescription.Equals(b.stateDescription) == false)
                    {
                        Log.Current.CurrentVariation.LogMessage("Actual and expected stateDescriptions did not match for state " + i);
                        Log.Current.CurrentVariation.LogMessage("\tActual: " + a.stateDescription.ToString());
                        Log.Current.CurrentVariation.LogMessage("\tExpected: " + b.stateDescription.ToString());
                        NavigationHelper.CacheTestResult(Result.Fail);
                        return false;
                    }
                }

                if (a.Equals(b))
                {
                    Log.Current.CurrentVariation.LogMessage("Actual and expected matched for state " + i);
                    NavigationHelper.CacheTestResult(Result.Pass);
                }
                else
                {
                    Log.Current.CurrentVariation.LogMessage("Actual and expected did not match for state " + i);
                    Log.Current.CurrentVariation.LogMessage("\tActual: " + a.ToString());
                    Log.Current.CurrentVariation.LogMessage("\tExpected: " + b.ToString());
                    NavigationHelper.CacheTestResult(Result.Fail);
                    match = false;
                }
            }
            return match;
        }
    }
}
