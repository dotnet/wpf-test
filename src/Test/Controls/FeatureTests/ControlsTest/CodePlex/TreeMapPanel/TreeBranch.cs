using System;
using System.Collections;
using System.Windows;

using WpfControlToolkit;

namespace Avalon.Test.ComponentModel.Actions
{
    namespace TreeMapTest
    {
        /// <summary>
        /// Represents a TreeMapPanel branch-head -- the most senior child of a branch.
        /// Initially all TreeMapPanel children are each considered a one-child branch.
        /// This data structure builds up state that supports the discovery of branches
        /// in the TreeMapPanel.
        /// </summary>
        internal struct TreeBranch
        {
            public enum Direction
            {
                Undefined = 0,
                Vertical,
                Horizontal
            }
            public Direction direction;

            // The number of other branches coalesced into this one, or zero if this not a branch head
            public Int32 count;
            // TreeMapPanel.AreaProperty of first element in this branch
            public Double largestWeight;
            // TreeMapPanel.AreaProperty of last element in this branch
            public Double smallestWeight;
            // Total of TreeMapPanel.AreaProperty values of elements coalesced to this branch.
            public Double totalWeight;
            // Top left location of this branch relative to TreeMapPanel parent -- (when FlowDirection.LeftToRight)
            public Point origin;
            // The width of this branch if it is Vertical, or still Undefined
            public Double width;
            // The height of this branch if it is Horizontal, or still Undefined
            public Double height;

            public bool SameBranch(ref TreeBranch b)//, out TreeBranch.Direction direction)
            {
                if (this.origin.X == b.origin.X)
                {
                    direction = Direction.Vertical;
                    return (Math.Abs(this.width - b.width) < TreeMapPanelTestActions.LayoutTolerance);
                }
                else if (this.origin.Y == b.origin.Y)
                {
                    return (Math.Abs(this.height - b.height) < TreeMapPanelTestActions.LayoutTolerance);
                }
                direction = TreeBranch.Direction.Undefined;
                return false;
            }
        }

        /// <summary>
        /// An error reporting class that could be re-written as an exception later
        /// </summary>
        internal class TreeBranchError
        {
            private String _message = String.Empty;
            public TreeBranchError()
            {
            }

            public TreeBranchError(String message)
            {
                _message = message;
            }
            public virtual String Message
            {
                get { return _message; }
            }
        }
    }
}
