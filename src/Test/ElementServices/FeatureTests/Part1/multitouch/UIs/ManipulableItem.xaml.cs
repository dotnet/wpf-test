// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Input.Manipulations;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// A simple item that can be translated, rotated, and scaled by dragging
    /// with the mouse. It has checkboxes to allow enabling/disabling the
    /// various types of manipulations, and a pivot indicator (which can be toggled
    /// on and off by clicking it).
    /// </summary>
    public partial class ManipulableItem : UserControl
    {
        #region Fields

        private const float minLinearFlickVelocity = 0.01F;
        private const float minScaleRotateRadius = 15F;

        private readonly ManipulationProcessor2D _manipulationProcessor;
        private readonly InertiaProcessor2D _inertiaProcessor;
        private readonly ManipulationPivot2D _pivot;
        private readonly DispatcherTimer _inertiaTimer;
        private UIElement _container;
        private Point _dragCenter = new Point(double.NaN, double.NaN);

        #endregion

        #region Event

        /// <summary>
        /// Event that fires when manipulation starts.
        /// </summary>
        public event EventHandler<Manipulation2DStartedEventArgs> Started
        {
            add { this._manipulationProcessor.Started += value; }
            remove { this._manipulationProcessor.Started -= value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ManipulableItem()
        {
            InitializeComponent();

            CanTranslate.Tag = Manipulations2D.Translate;
            CanRotate.Tag = Manipulations2D.Rotate;
            CanScale.Tag = Manipulations2D.Scale;
            CanTranslate.Checked += OnCheckedChanged;
            CanTranslate.Unchecked += OnCheckedChanged;
            CanRotate.Checked += OnCheckedChanged;
            CanRotate.Unchecked += OnCheckedChanged;
            CanScale.Checked += OnCheckedChanged;
            CanScale.Unchecked += OnCheckedChanged;
            PivotButton.Click += OnPivotClick;

            // The DeadZone is a little red ring that shows the area inside which
            // no rotation or scaling will happen if you drag the mouse. We set
            // it to four times the size of the manipulation processor's MininumScaleRotateRadius.
            // Reason for the number:
            // - x2, because diameter = 2 * radius
            // - x2, because the number on the manipulation processor is radius from
            //   the center of mass of the manipulators being used, which in the case
            //   of this test app will be the midpoint between the mouse and the hub.
            DeadZone.Width = 4 * minScaleRotateRadius;
            DeadZone.Height = 4 * minScaleRotateRadius;

            this._manipulationProcessor = new ManipulationProcessor2D(SupportedManipulations);
            this._manipulationProcessor.MinimumScaleRotateRadius = minScaleRotateRadius;
            this._manipulationProcessor.Started += OnManipulationStarted;
            this._manipulationProcessor.Delta += OnManipulationDelta;
            this._manipulationProcessor.Completed += OnManipulationCompleted;

            this._inertiaProcessor = new InertiaProcessor2D();
            this._inertiaProcessor.TranslationBehavior.DesiredDeceleration = 0.0001F;
            this._inertiaProcessor.RotationBehavior.DesiredDeceleration = 1e-6F;
            this._inertiaProcessor.ExpansionBehavior.DesiredDeceleration = 0.0001F;
            this._inertiaProcessor.Delta += OnManipulationDelta;
            this._inertiaProcessor.Completed += OnInertiaCompleted;

            this._inertiaTimer = new DispatcherTimer(DispatcherPriority.Input,Dispatcher.CurrentDispatcher);
            this._inertiaTimer.IsEnabled = false;
            this._inertiaTimer.Interval = TimeSpan.FromMilliseconds(15);
            this._inertiaTimer.Tick += OnTimerTick;

            this._pivot = new ManipulationPivot2D();

            RenderTransformOrigin = new Point(0.5, 0.5);

            Radius = 100;
            Center = new Point(0, 0);
            IsPivotActive = true;
            Move(Radius, Radius, 0, 1);
        }

        #endregion 

        #region Public Properties

        /// <summary>
        /// Gets or sets the container used for manipulations.
        /// </summary>
        public UIElement Container
        {
            get { return this._container; }
            set { this._container = value; }
        }

        #endregion

        #region Protected Event handlers

        /// <summary>
        /// mouse goes down on the item.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            e.MouseDevice.Capture(this);
        }

        /// <summary>
        /// mouse goes up.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.MouseDevice.Captured == this)
            {
                e.MouseDevice.Capture(null);
            }
        }

        /// <summary>
        /// capture the mouse.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            base.OnGotMouseCapture(e);
            ProcessMouse(e.MouseDevice);
        }

        /// <summary>
        /// lost mouse capture.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            this._manipulationProcessor.ProcessManipulators(Timestamp, null);
        }

        /// <summary>
        /// mouse moves.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ProcessMouse(e.MouseDevice);
        }

        #endregion

        #region Private Members - Methods, Properties, Event Handlers

        /// <summary>
        /// Process a mouse event.
        /// </summary>
        /// <param name="mouse"></param>
        private void ProcessMouse(MouseDevice mouse)
        {
            if ((mouse.Captured == this) && (this._container != null))
            {
                Point position = mouse.GetPosition(this._container);
                List<Manipulator2D> manipulators = new List<Manipulator2D>();
                manipulators.Add(new Manipulator2D(
                    0,
                    (float)(position.X),
                    (float)(position.Y)));

                // If translation is turned off and the pivot is turned on,
                // make it act like there's a manipulator on the pivot point,
                // to allow us to do scaling
                if (((SupportedManipulations & Manipulations2D.Translate) == Manipulations2D.None)
                    && IsPivotActive)
                {
                    manipulators.Add(new Manipulator2D(
                        1,
                        (float)(Center.X),
                        (float)(Center.Y)));
                }

                const Manipulations2D translateAndRotate = Manipulations2D.Translate | Manipulations2D.Rotate;
                if ((manipulators.Count == 1)
                    && ((this._manipulationProcessor.SupportedManipulations & translateAndRotate) == translateAndRotate)
                    && IsPivotActive)
                {
                    this._dragCenter = position;
                }
                else
                {
                    this._dragCenter.X = double.NaN;
                    this._dragCenter.Y = double.NaN;
                }

                this._manipulationProcessor.ProcessManipulators(
                    Timestamp,
                    manipulators);
            }
        }

        /// <summary>
        /// Get the manipulations we should currently be supporting.
        /// </summary>
        private Manipulations2D SupportedManipulations
        {
            get
            {
                return GetManipulations(CanTranslate)
                        | GetManipulations(CanRotate)
                        | GetManipulations(CanScale);
            }
        }

        /// <summary>
        /// Gets the center of the item, in container coordinates.
        /// </summary>
        private Point Center
        {
            get 
            { 
                return new Point(this._pivot.X, this._pivot.Y); 
            }
            set
            {
                this._pivot.X = (float)value.X;
                this._pivot.Y = (float)value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the object, in degrees.
        /// </summary>
        private double Orientation { get; set; }

        /// <summary>
        /// Gets or sets the radius of the object, in pixels.
        /// </summary>
        private double Radius
        {
            get 
            { 
                return this._pivot.Radius; 
            }
            set
            {
                this._pivot.Radius = (float)value;
                Width = 2 * value;
                Height = 2 * value;
            }
        }

        /// <summary>
        /// Gets or sets whether the pivot is active.
        /// </summary>
        private bool IsPivotActive
        {
            get 
            { 
                return this._manipulationProcessor.Pivot != null; 
            }
            set
            {
                this._manipulationProcessor.Pivot = value ? this._pivot : null;
                PivotButton.Opacity = value ? 1.0 : 0.3;
            }
        }

        /// <summary>
        /// Gets the current timestamp.
        /// </summary>
        private static long Timestamp
        {
            get
            {
                // The question of what tick source to use is a difficult
                // one in general, but for purposes of this test app,
                // DateTime ticks are good enough.
                return DateTime.UtcNow.Ticks;
            }
        }

        /// <summary>
        /// when the state of a checkbox changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            this._manipulationProcessor.SupportedManipulations = SupportedManipulations;
            if (this._inertiaProcessor.IsRunning)
            {
                this._inertiaProcessor.Complete(Timestamp);
            }
        }

        /// <summary>
        /// when the pivot button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPivotClick(object sender, RoutedEventArgs e)
        {
            IsPivotActive = !IsPivotActive;
        }

        /// <summary>
        /// when manipulation starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnManipulationStarted(object sender, Manipulation2DStartedEventArgs e)
        {
            if (this._inertiaProcessor.IsRunning)
            {
                this._inertiaProcessor.Complete(Timestamp);
            }
        }

        /// <summary>
        /// when manipulation gives a delta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnManipulationDelta(object sender, Manipulation2DDeltaEventArgs e)
        {
            Move(
                e.Delta.TranslationX,
                e.Delta.TranslationY,
                e.Delta.Rotation,
                e.Delta.ScaleX);
        }

        /// <summary>
        /// when manipulation completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnManipulationCompleted(object sender, Manipulation2DCompletedEventArgs e)
        {
            this._dragCenter = new Point(double.NaN, double.NaN);

            float velocityX = e.Velocities.LinearVelocityX;
            float velocityY = e.Velocities.LinearVelocityY;
            float speedSquared = velocityX * velocityX + velocityY * velocityY;
            if (speedSquared < minLinearFlickVelocity * minLinearFlickVelocity)
            {
                velocityX = velocityY = 0;
            }
            this._inertiaProcessor.TranslationBehavior.InitialVelocityX = velocityX;
            this._inertiaProcessor.TranslationBehavior.InitialVelocityY = velocityY;
            this._inertiaProcessor.RotationBehavior.InitialVelocity = e.Velocities.AngularVelocity;
            this._inertiaProcessor.ExpansionBehavior.InitialVelocityX = e.Velocities.ExpansionVelocityX;
            this._inertiaProcessor.ExpansionBehavior.InitialVelocityY = e.Velocities.ExpansionVelocityY;
            //this.inertiaProcessor.InitialRadius = (float)Radius; // this.inertiaProcessor.InitialRadius = (float)Radius;
            this._inertiaTimer.Start();
        }

        /// <summary>
        /// when manipulation completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInertiaCompleted(object sender, Manipulation2DCompletedEventArgs e)
        {
            this._inertiaTimer.Stop();
        }

        /// <summary>
        /// when the inertia timer ticks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            this._inertiaProcessor.Process(Timestamp);
        }

        /// <summary>
        /// Move the item as specified.
        /// </summary>
        /// <param name="deltaX">Distance to translate along X axis.</param>
        /// <param name="deltaY">Distance to translate along Y axis.</param>
        /// <param name="rotation">Amount to rotate, in radians.</param>
        /// <param name="scale">Scale factor to apply.</param>
        private void Move(double deltaX, double deltaY, double rotation, double scale)
        {
            AdjustForSingleManipulatorDragRotation(ref deltaX, ref deltaY, rotation);

            MatrixTransform transform = RenderTransform as MatrixTransform;
            if ((transform == null) || transform.IsFrozen)
            {
                transform = new MatrixTransform();
                RenderTransform = transform;
            }

            double newX = Center.X + deltaX;
            double newY = Center.Y + deltaY;
            if (this._container != null)
            {
                newX = Math.Max(0, Math.Min(newX, this._container.RenderSize.Width));
                newY = Math.Max(0, Math.Min(newY, this._container.RenderSize.Height));
            }

            Center = new Point(newX, newY);
            Orientation += rotation * 180.0 / Math.PI;
            Radius = Math.Max(40, Math.Min(350, scale * Radius));

            Matrix matrix = Matrix.Identity;
            matrix.Rotate(Orientation);
            matrix.Translate(Center.X - Radius, Center.Y - Radius);

            transform.Matrix = matrix;
        }

        /// <summary>
        /// If you're dragging with a single manipulator, *and* rotation and translation
        /// are both enabled, *and* the pivot is turned on, then we want the object to
        /// swing into line behind the drag point. This adjusts for that.
        /// </summary>
        /// <param name="deltaX">Distance to translate along X axis.</param>
        /// <param name="deltaY">Distance to translate along Y axis.</param>
        /// <param name="rotation">Amount to rotate, in radians.</param>
        private void AdjustForSingleManipulatorDragRotation(ref double deltaX, ref double deltaY, double rotation)
        {
            if (double.IsNaN(this._dragCenter.X) || double.IsNaN(this._dragCenter.Y))
            {
                // we're not in single-manipulator-drag-rotate mode, do nothing
                return;
            }

            Vector toCenter = Center - this._dragCenter;
            double sin = Math.Sin(rotation);
            double cos = Math.Cos(rotation);
            Vector rotatedToCenter = new Vector(
                toCenter.X * cos - toCenter.Y * sin,
                toCenter.X * sin + toCenter.Y * cos);
            Vector shift = rotatedToCenter - toCenter;
            deltaX += shift.X;
            deltaY += shift.Y;
        }

        /// <summary>
        /// Get the manipulations that a checkbox specifies.
        /// </summary>
        /// <param name="checkbox"></param>
        /// <returns></returns>
        private static Manipulations2D GetManipulations(CheckBox checkbox)
        {
            return (checkbox.IsChecked.Value) ? (Manipulations2D)checkbox.Tag : Manipulations2D.None;
        }

        #endregion
    }
}
