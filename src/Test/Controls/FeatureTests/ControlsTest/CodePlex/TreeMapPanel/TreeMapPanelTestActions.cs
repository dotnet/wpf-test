using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Text;
using System.Diagnostics;

using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using System.Windows.Controls.Primitives;

using WpfControlToolkit;

namespace Avalon.Test.ComponentModel.Actions
{
    using TreeMapTest;

    public class TreeMapPanelTestActions
    {
        #region private constants

        /// <summary>
        /// The resolution of LayoutInformation.GetLayOutSlot( element) is less exact than
        /// full "Double" precision with some variability in the least significant digits
        /// from call to call. LayoutTolerance is used when comparing LayoutSlot
        /// Widths or Heights for eqality
        /// Note: This value delta is a heuristic--the underlying tolerances are not known.
        ///       The comparisons where it is used compares two values obtained directly from WPF.
        /// </summary>
        internal const Double LayoutTolerance = 1E-14;
        /// <summary>
        /// Used for equality comparison of: ratios of Layout areas with ratios of AreaProperty
        /// values. 
        /// Note: This value delta is a heuristic--the comparisons where it is used involve values
        ///       from multiple WPF API calls and contain accumulated errors, so the tolerance is
        ///       larger than LayoutTolerance.
        /// </summary>
        internal const Double RatioTolerance = 1E-13;
        /// <summary>
        /// Used for eqality comparisons of: "Sum of discovered-branch  weights" to 
        ///                                  "Sum of treeMap.Children.AreaProperty values"
        /// Note: This value delta is a heuristic--the comparisons where it is used involve values
        ///       from two different domains: AreaProperty attached property values, and WPF
        ///       UI LayoutSlot geometry values. Furthermore both compared values contain accumulated
        ///       errors, so the tolerance is larger than LayoutTolerance, as well.
        /// </summary>
        internal const Double AreaTotalsDifferenceTolerance = 1E-11;

        /// <summary>
        /// addChild_MinWeight & addChild_IncrementQuantum
        /// Specify limits on how parameters passed to test method AddModelChildren are used.
        /// The intent is to avoid having to design for comparisons of arbitrary small values.
        /// </summary>
        private const Double addChild_MinWeight = 1E-3;
        private const Double addChild_IncrementQuantum = 1E-3;
        #endregion

        #region private classes and structs

        // ----------------------------------------------
        // Class to support Array.Sort ordering of an Int32[], based
        // on comparisons of "weights" attached to TreeMapPanel child elements
        // ----------------------------------------------
        private class Comparer : System.Collections.Generic.IComparer<Int32>
        {
            UIElementCollection _reference;
            public Comparer(UIElementCollection reference)
            {
                _reference = reference;
            }

            public Int32 Compare(Int32 i, Int32 j) //inverse comparison
            {
                return           (TreeMapElementExtensions.Weight((FrameworkElement)_reference[j]))
                       .CompareTo(TreeMapElementExtensions.Weight((FrameworkElement)_reference[i]));
            }
        }
        #endregion

        #region private methods

        // This is intended only for local test development, not to be called when
        // TestLog.Current is instantiated in the environment.
        static private void PostLocalTestErrorMessage(String errorString)
        {
            if( TestLog.Current != null){
                TestLog.Current.LogEvidence(errorString);
            }
            else
            {
                MessageBox.Show(errorString);
            }
        }

        // <summary>
        // Initialize an array of TreeBranch structs, one from each TreeMapPanel child, extracting the child-element's
        // layout slot and weight, ordered by decreasing child-element weight as specified by elementSortMap.
        // </summary>
        static private TreeBranch[] BranchArrayFromTreeMapChildren(UIElementCollection elements, Int32[] elementSortMap)
        {
            Debug.Assert(elements.Count == elementSortMap.Length);

            TreeBranch[] branches = new TreeBranch[elementSortMap.Length];

            // Initialize a branch for each element in elementSortMap order 
            for (var i = 0; i < elements.Count; ++i)
            {
                FrameworkElement element = (FrameworkElement)elements[elementSortMap[i]];

                branches[i].direction = TreeBranch.Direction.Undefined;
                branches[i].count = 1;
                //branches[i].largestWeight = element.Weight();
                //branches[i].smallestWeight = element.Weight();
                //branches[i].totalWeight = element.Weight();
                //branches[i].origin = element.SlotLocation();
                //branches[i].width = element.SlotSize().Width;
                //branches[i].height = element.SlotSize().Height;
                branches[i].largestWeight = TreeMapElementExtensions.Weight(element);
                branches[i].smallestWeight = TreeMapElementExtensions.Weight(element);
                branches[i].totalWeight = TreeMapElementExtensions.Weight(element);
                branches[i].origin = TreeMapElementExtensions.SlotLocation(element);
                branches[i].width = (TreeMapElementExtensions.SlotSize(element)).Width;
                branches[i].height = (TreeMapElementExtensions.SlotSize(element)).Height;
            }
            return branches;
        }

        static private List<TreeBranch> FindAndCoalesceBranches(TreeBranch[] branches)
        {
            // Coalesce branches:
            // j is index of current branch-head candidate
            for (Int32 j = 0; j < (branches.Length - 1); ++j)
            {
                // if not already coalesced into an earlier branch.
                if (branches[j].count != 0) 
                {
                    // branches[j] is now a branch-head
                    // -------------------------------------------------
                    // As long as an iteration coalesces a branch, try to coalesce more branches.
                    // End iteration k once an iteration does not coalesce an additional branch.
                    // (Any remaining branches cannot possibly extend branches[j])
                    bool tryNextBranch = true;
                    for (Int32 k = j + 1; tryNextBranch && (k < branches.Length); ++k)
                    {
                        tryNextBranch = false;

                        // Because treeMap's sort may not coincide with ours for equal-weighted children,
                        // evaluate all branch candidates with the same weight as branches[k]
                        for (Int32 m = k;    (m < branches.Length)
                                          && (branches[m].count == 1)
                                          && (branches[m].smallestWeight == branches[k].smallestWeight);
                             ++m)
                        {
                            if (branches[j].SameBranch(ref branches[m]))
                            {
                                branches[m].count = 0; // remove branch[m] from consideration as a branch head.
                                branches[j].smallestWeight = branches[m].smallestWeight;
                                branches[j].count++;
                                branches[j].totalWeight += branches[m].smallestWeight;
                                tryNextBranch = true;
                            }
                        }
                    }
                }
            }

            List<TreeBranch> branchList = new List<TreeBranch>(branches.Length);
            foreach(TreeBranch branch in branches)
            {
                // Add only head=branches to branchList
                if (branch.count > 0)
                {
                    branchList.Add(branch);
                }
            }
            return branchList;
        }

        // <summary>
        //  Verify discovered TreeMapPanel layout constraints, given that 'branchList' was
        //  built by private static method FindAndCoalesceBranches()
        // </summary>
        // <returns></returns>
        static private bool BranchListMembersAreInIncreasingDistanceOrderFromPanelOrigin(
                                                           List<TreeBranch> branchList,
                                                           out Double branchListWeightSum,
                                                           out Int32 branchListCountSum,
                                                           out TreeBranchError treeBranchError)
        {
            treeBranchError = null;

            branchListWeightSum = branchList[0].totalWeight;
            branchListCountSum = branchList[0].count;
            Double prevDistance = 0.0; // The value of branchList[0] distance--by definition
            Double prevWeight = branchList[0].totalWeight;

            // ---------------------------------------------------------
            // Is heaviest (1st) branch origin located at Panel origin?
            // ---------------------------------------------------------
            if (branchList[0].origin != new Point(0d, 0d))
            {
                treeBranchError =
                    new TreeBranchError(String.Format("!!!Error: Branch[0] origin should be {0}; Actually is {1}",
                                                           new Point().ToString(), branchList[0].origin.ToString()));
                return false;
            }

            for (Int32 i = 1; i < branchList.Count; ++i)
            {
                branchListWeightSum += branchList[i].totalWeight;
                branchListCountSum += branchList[i].count;

                // -----------------------------------------------------------------------------------------------
                // Are branch heads ordered by increasing distance from Panel origin?
                // -----------------------------------------------------------------------------------------------
                Double distance = Math.Sqrt(Math.Pow(branchList[i].origin.X, 2) + Math.Pow(branchList[i].origin.Y, 2));
                if (distance < prevDistance)
                {
                    StringBuilder strErr =
                        new StringBuilder("!!!Error: 'Later' branches must be further from Panel origin than 'Earlier' branches.");

                    strErr.AppendFormat("..Detail: Branch[{0}] distance= {1},  Branch[{2}] distance= {3}",
                                         i, distance, i - 1, prevDistance);

                    treeBranchError = new TreeBranchError(strErr.ToString());
                    return false;
                }

                prevWeight = branchList[i].totalWeight;
            }
            return true;
        }

        // <summary>
        // Validate that the TreeMapPanel children are arranged into 'branches' in hierarchical order:
        //   I. Highest-order branch origin is located at Panel origin.
        //  II. Successive-order branch origins are at increasing distances from Panel origin.
        // III. Sum of branches' member's counts == treeMap.Children.Count.
        //  IV. Sum of branches' member's weights == Sum of treeMap.Children.AreaProperty.
        //  NOTE: An additional constraint "Successive branches contain only elements of equal or
        //        lesser weights" is tested imlicitly as a consequence of:
        //        a) Fact that FindAndCoalesceBranches() examines elements by decreasing weight.
        //        b) Fact that VerifyRatioInvariant()checks that all elemenets are accounted-for.
        // </summary>
        // <param name="branchList">The list of branches discovered by FindBranches()</param>
        // <param name="treeMap"></param>
        // <param name="treeBranchError"></param>
        // <returns></returns>
        static private bool VerifyTreeMapBranchConstraints( 
              List<TreeBranch> branchList, TreeMapPanel treeMap, out TreeBranchError treeBranchError)
        {
            // ..................................................................................................

            treeBranchError = null;
            if (branchList.Count == 0)
            {
                return true;
            }

            Double branchListWeightSum;
            Int32 branchListCountSum;
            // ----Constraints I & II -------------------------------------------------------------------------
            //  I. Verify highest-order branch origin is located at Panel origin.
            // II. verify successive-order branch origins are at increasing distances from Panel origin
            // -----------------------------------------------------------------------------------------------
            if (!BranchListMembersAreInIncreasingDistanceOrderFromPanelOrigin(
                branchList, out branchListWeightSum, out branchListCountSum, out treeBranchError))
            {
                return false;
            }

            // ----Constraint III ----------------------------------------------------------------------------
            // Verify Sum of branchList member's counts == treeMap.Children.Count
            // -----------------------------------------------------------------------------------------------
            if (branchListCountSum != treeMap.Children.Count)
            {
                treeBranchError = new TreeBranchError(
                    String.Format("!!!Error: Sum of BranchList child counts == {0}, does not match actual Panel.Children.Count == {1}",
                           branchListCountSum, treeMap.Children.Count));
                return false;
            }

            // ----Constraint IV -----------------------------------------------------------------------------
            // Verify Sum of branchList member's weights == Sum of treeMap.Children.AreaProperty
            // -----------------------------------------------------------------------------------------------
            Double panelChildrenWeightSum = 0.0;
            foreach (FrameworkElement element in treeMap.Children)
            {
                panelChildrenWeightSum += TreeMapElementExtensions.Weight(element);
            }

            if (Math.Abs(branchListWeightSum - panelChildrenWeightSum) > AreaTotalsDifferenceTolerance)
            {
                StringBuilder errStr = new StringBuilder(
                    "!!!Error: Branch AreaProperty sum is not equal to Panel.Children AreaProperty sum. Difference exceeds tolerance.");

                errStr.AppendFormat(
                    "..Detail: Branch AreaProperty sum= {0}, Panel.Children AreaProperty sum= {1}; Diff delta= {2}; Diff tolerance ={3}",
                       branchListWeightSum, panelChildrenWeightSum,
                       Math.Abs(branchListWeightSum - panelChildrenWeightSum), AreaTotalsDifferenceTolerance);
                errStr.AppendLine();
                errStr.AppendFormat(
                    "....Note: Panel.Children.Count and Sum of BranchList child counts agree == {0}",
                       branchListCountSum);

                treeBranchError = new TreeBranchError(errStr.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// A higher-level method that calls lower level, functionally decomposed, methods to
        /// find and verify valid TreeMapPanel arrangement of child elements into geometric
        /// subareas we call branches.
        /// </summary>
        static private bool VerifyArrangementConstraints(TreeMapPanel treeMap)
        {
            if (treeMap.Children.Count == 0)
            {
                return true;
            }

            Int32[] elementSortMap = new Int32[treeMap.Children.Count];
            Double totalChildWeight = 0.0;
            for (var i = 0; i < treeMap.Children.Count; ++i)
            {
                elementSortMap[i] = i;
                totalChildWeight += TreeMapElementExtensions.Weight((FrameworkElement)treeMap.Children[i]);
            }

            // The elementSortMap values become indices into TreeMapPanel's child
            // collection ordered by decreasing child area-weight.
            Array.Sort<Int32>(elementSortMap, new Comparer( treeMap.Children) );

            TreeBranch[] branches= BranchArrayFromTreeMapChildren(treeMap.Children, elementSortMap);

            List<TreeBranch> branchList = FindAndCoalesceBranches(branches);

            TreeBranchError treeBranchError;
            if (false == VerifyTreeMapBranchConstraints(branchList, treeMap, out treeBranchError))
            {
                if (TestLog.Current != null)
                {
                    TestLog.Current.LogEvidence(treeBranchError.Message);
                }
                else
                {
                    PostLocalTestErrorMessage(treeBranchError.Message);
                } 
                return false;
            }
            return true;
        }

        // <summary>
        // Verify that the ratio of every element's attached AreaProperty to the sum of all elements' AreaProperties,
        // corresponds to the ratio of LayOutSlot Rect allocated for that element to the total LayOutSlot Rects of the Panel.
        // </summary>
        // <param name="treeMap"></param>
        // <returns></returns>
        static private bool VerifyAreaRatiosConstraint(TreeMapPanel treeMap)
        {
            Double SumOfChildAreaProperties = 0.0;
            Double SumOfChildLayoutSpace = 0.0;

            if (treeMap.Children.Count == 0)
            {
                return true;
            }

            foreach (var child in treeMap.Children)
            {
                SumOfChildAreaProperties += (Double)((FrameworkElement)child).GetValue(TreeMapPanel.AreaProperty);
                SumOfChildLayoutSpace += (  LayoutInformation.GetLayoutSlot((FrameworkElement)child).Width
                                          * LayoutInformation.GetLayoutSlot((FrameworkElement)child).Height);
            }

            if ( (SumOfChildLayoutSpace == 0.0) || (SumOfChildAreaProperties == 0.0) )
            {
                StringBuilder strErr = new StringBuilder();
                strErr.AppendLine("!!!Error: Found Child 'Layout' and/or 'Area' sums equal to zero.");
                strErr.AppendFormat("..Detail: Sum of Child 'Layout' 2D space (Rect area)= {0}, Sum of Child 'Areas'(AreaProperty)= {1}",
                                       SumOfChildLayoutSpace, SumOfChildAreaProperties);
                return false;
            }

            foreach (var child in treeMap.Children)
            {
                if (   Math.Abs(  (  LayoutInformation.GetLayoutSlot((FrameworkElement)child).Width
                                   * LayoutInformation.GetLayoutSlot((FrameworkElement)child).Height)
                                   / SumOfChildLayoutSpace)
                                - (  (Double)((FrameworkElement)child).GetValue(TreeMapPanel.AreaProperty)
                                   / SumOfChildAreaProperties)
                       > RatioTolerance)
                {
                    StringBuilder errStr = new StringBuilder();
                    errStr.AppendLine("!!!Error: Comparison of Child 'Layout' ratio to Child 'Area' ratio exceeds test tolerance.");
                    errStr.AppendFormat("..Detail: Child position X, Y = ({0}, {1}); Child 'area' = {2}; Ratio delta= {3}; Ratio tolerance= {4}",
                                     LayoutInformation.GetLayoutSlot((FrameworkElement)child).X,
                                     LayoutInformation.GetLayoutSlot((FrameworkElement)child).Y,
                                     ((Double)((FrameworkElement)child).GetValue(TreeMapPanel.AreaProperty)),
                                     (Math.Abs((  LayoutInformation.GetLayoutSlot((FrameworkElement)child).Width
                                                * LayoutInformation.GetLayoutSlot((FrameworkElement)child).Height)
                                                / SumOfChildLayoutSpace)
                                             - (  (Double)((FrameworkElement)child).GetValue(TreeMapPanel.AreaProperty)
                                                / SumOfChildAreaProperties)));

                    if (TestLog.Current != null)
                    {
                        TestLog.Current.LogEvidence(errStr.ToString());
                    }
                    else
                    {
                        PostLocalTestErrorMessage(errStr.ToString());
                    } 
                    return false;
                }
            }
            return true;
        }
 
        #endregion

        #region public methods
        /// <summary>
        /// Public method to validate TreeMap constraints -- the TreeMap children must derive from FrameworkElement. 
        /// </summary>
        /// <param name="TreeMap"></param>
        /// <returns></returns>
        static public bool VerifyTreeMapConstraints(TreeMapPanel TreeMap)
        {
            return VerifyArrangementConstraints(TreeMap) && VerifyAreaRatiosConstraint(TreeMap);
        }

        /// <summary>
        /// Dynamically add any number of children to a TreeMapPanel
        /// </summary>
        /// <param name="TreeMap">TreeMapPanel for new children</param>
        /// <param name="Model">
        ///    A Control-derived FrameworkElement whose Type specifies the child type to create.
        /// </param>
        /// <param name="Count">Number of cheldren to create</param>
        /// <param name="SetTag">
        ///   The FrameworkElement.Tag property to set on all the children added. The Tag value is
        ///   then available when calling the SetTaggedChildrenWeight() & RemoveTaggedChildren() methods.
        /// </param>
        /// <param name="FirstWeight">The TreeMapPanel.AreaProperty value to assign to the first child added</param>
        /// <param name="WeightIncrement">
        ///   The increment for determining each successive child's TreeMapPanel.AreaProperty assinged value.
        ///   This allows for adding a set of children with a range of AreaProperty values.
        /// </param>
        /// <returns></returns>
        static public bool AddModelChildren(TreeMapPanel TreeMap, Control Model, Int32 Count, String SetTag, Double FirstWeight, Double WeightIncrement)
        {
            if (Count < 1)
            {
                throw new ArgumentOutOfRangeException("Count", "Value cannot be less than one");
            }
            if (SetTag == null || SetTag == String.Empty)
            {
                throw new ArgumentException("SetTag argument string cannot be null or empty.");
            }
            if (FirstWeight < addChild_MinWeight)
            {
                throw new ArgumentOutOfRangeException("FirstWeight", String.Format("Test value should be greater than: {0:E}", addChild_MinWeight));
            }
            if (WeightIncrement < 0.0)
            {
                throw new ArgumentOutOfRangeException("WeightIncrement", "Value must be non-negative");
            }
            WeightIncrement = (Math.Floor(WeightIncrement / addChild_IncrementQuantum) * addChild_IncrementQuantum);

            Int32 idx = 0;
            Type controlType = Model.GetType();
            PropertyInfo textProperty = controlType.GetProperty("Text", BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo contentProperty = controlType.GetProperty("Content", BindingFlags.Instance | BindingFlags.Public);
            foreach(Brush brush in BrushCycler.GetBrushes(Count))
            {
                Control newChild = (Control)System.Activator.CreateInstance(controlType);
                newChild.SetValue(TreeMapPanel.AreaProperty, (FirstWeight + (++idx * WeightIncrement)));
                newChild.Background = brush;
                newChild.Tag = SetTag;
                if (textProperty != null)
                {
                    textProperty.SetValue(newChild, idx.ToString(), null);
                }
                else if (contentProperty != null)
                {
                    contentProperty.SetValue(newChild, idx.ToString(), null);
                }
                TreeMap.Children.Add(newChild);
            }
            QueueHelper.WaitTillQueueItemsProcessed();
            return true;
        }

        /// <summary>
        ///  Dynamically change the AreaProperty value of some children in the TreeMapPanel.
        /// </summary>
        /// <param name="TreeMap">TreeMapPanel whose children will be modified</param>
        /// <param name="HavingTag">String-valued Tag to filter children by</param>
        /// <param name="Weight">New AreaProperty value to be assigned</param>
        /// <param name="LimitCount">The maximum number of children to affect</param>
        /// <returns></returns>
        static public bool SetTaggedChildrenWeight(TreeMapPanel TreeMap, String HavingTag, Double Weight, Int32 LimitCount)
        {
            if( HavingTag == null || HavingTag == String.Empty)
            {
                throw new ArgumentException("HavingTag argument string cannot be null or empty.");
            }

            if (LimitCount < 1) LimitCount = Int32.MaxValue; 
            Int32 numberAffected = 0;
            foreach (UIElement child in TreeMap.Children)
            {
                if (HavingTag == ((FrameworkElement)child).Tag as String)
                {
                    ((FrameworkElement)child).SetValue(TreeMapPanel.AreaProperty, Weight);
                    if (++numberAffected == LimitCount) break;
                }
            }
            QueueHelper.WaitTillQueueItemsProcessed();
            return true;
        }

        /// <summary>
        ///  Dynamically remove children from the TreeMapPanel.
        /// </summary>
        /// <param name="TreeMap">TreeMapPanel to remove children from.</param>
        /// <param name="ChildTag">String-valued Tag to filter children by.</param>
        /// <param name="LimitCount">The maximum number of children to affect</param>
        /// <returns></returns>
        static public bool RemoveTaggedChildren(TreeMapPanel TreeMap, String ChildTag, Int32 LimitCount)
        {
            if (ChildTag == null || ChildTag == String.Empty)
            {
                throw new ArgumentException("ChildTag argument string cannot be null or empty.");
            }

            if (LimitCount < 1) LimitCount = Int32.MaxValue;
            Int32 numberAffected = 0;

            // Using for-loop instead of Collection enumerator to avoid
            // an invalidated enumerator exeption after the first removal.
            for (Int32 i = TreeMap.Children.Count - 1; i >= 0; --i)
            {
                if (ChildTag == ((FrameworkElement)TreeMap.Children[i]).Tag as String)
                {
                    TreeMap.Children.RemoveAt(i);
                    if (++numberAffected == LimitCount) break;
                }
            }
            QueueHelper.WaitTillQueueItemsProcessed();
            return true;
        }

        /// <summary>
        /// Dynamically change the TreeMapPanel FlowDirection property.
        /// </summary>
        /// <param name="TreeMap"></param>
        /// <returns></returns>
        static public bool ToggleFlowDirection(TreeMapPanel TreeMap)
        {
            TreeMap.FlowDirection = (TreeMap.FlowDirection == FlowDirection.LeftToRight)
                ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            return true;
        }

        /// <summary>
        /// Dynamically change the TreeMapPanel's desired size.
        /// </summary>
        /// <param name="TreeMap"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        static public bool SetPanelSize(TreeMapPanel TreeMap, Int32 Width, Int32 Height)
        {
            TreeMap.Width = Width;
            TreeMap.Height = Height;
            return true;
        }
        #endregion
    }
}
