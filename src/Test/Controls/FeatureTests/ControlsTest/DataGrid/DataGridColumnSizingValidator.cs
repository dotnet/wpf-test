using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Controls.Helpers;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid Column Sizing validator
    /// </summary>
    public enum HeaderGripper
    {
        Left,
        Right
    }

    public enum MouseMoveDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    public class DataGridColumnSizingValidator
    {
        private DataGrid datagrid;
        private int headerIndex;
        private int moveAmount;
        private HeaderGripper headerGripper;
        private MouseMoveDirection mouseMoveDirection;

        public DataGridColumnSizingValidator(DataGrid datagrid, int headerIndex, HeaderGripper headerGripper, int moveAmount, MouseMoveDirection mouseMoveDirection)
        {
            this.datagrid = datagrid;
            this.headerIndex = headerIndex;
            this.headerGripper = headerGripper;
            this.moveAmount = moveAmount;
            this.mouseMoveDirection = mouseMoveDirection;
        }

        public void Run()
        {
            // unchange
            int tolerance = 6;

            // Find headerGripper thumb
            Thumb thumb = null; 

            switch (headerGripper)
            {
                case HeaderGripper.Left:
                    thumb = DataGridHelper.GetColumnHeaderLeftGripper(datagrid, headerIndex);
                    break;
                case HeaderGripper.Right:
                    thumb = DataGridHelper.GetColumnHeaderGripper(datagrid, headerIndex);
                    break;
            }

            if (thumb == null)
            {
                throw new TestValidationException("Fail: thumb is null.");
            }

            // Compute the new location point
            System.Drawing.Point newPoint;

            int y = (int)(thumb.PointToScreen(new Point()).Y + thumb.ActualHeight / 2);
            int x = default(int);

            switch (mouseMoveDirection)
            {
                case MouseMoveDirection.Left:
                    x = (int)thumb.PointToScreen(new Point()).X - moveAmount;
                    break;
                case MouseMoveDirection.Right:
                    x = (int)thumb.PointToScreen(new Point()).X + moveAmount;
                    break;
            }

            newPoint = new System.Drawing.Point(x, y);

            // Action:
            //     Resize the datagrid column by dragging the header gripper
            InputHelper.MouseMoveToElementCenter(thumb);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            Microsoft.Test.Input.Mouse.Down(Microsoft.Test.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            Microsoft.Test.Input.Mouse.MoveTo(newPoint);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            Microsoft.Test.Input.Mouse.Up(Microsoft.Test.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            // Validation:
            //    Verify the thumb is in the right location
            System.Drawing.Point thumbPointToScreen = new System.Drawing.Point((int)thumb.PointToScreen(new Point()).X, (int)thumb.PointToScreen(new Point()).Y);

            int difference = newPoint.X - thumbPointToScreen.X;
            if (difference > tolerance)
            {
                throw new TestValidationException(String.Format("Fail: unable to move thumb. difference is {0}; tolerance is {1}", difference, tolerance));
            }
        }
    }
}
