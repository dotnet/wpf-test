using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Collections.Specialized;

namespace WpfControlToolkit
{
    public class Graph : FrameworkElement
    {
        static Graph()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(Graph), new FrameworkPropertyMetadata(true));
        }

        public Graph()
        {
            this.Loaded += new RoutedEventHandler(Graph_Loaded);
            this.Unloaded += new RoutedEventHandler(Graph_Unloaded);
            _compositionTarget_RenderingCallback = new EventHandler(this.compositionTarget_rendering);

            _fadingGCPs = new Dictionary<int, GraphContentPresenter>();
            _fadingGCPsNextKey = int.MinValue;

            _nodeTemplateBinding = new Binding(NodeTemplateProperty.Name);
            _nodeTemplateBinding.Source = this;

            _nodeTemplateSelectorBinding = new Binding(NodeTemplateSelectorProperty.Name);
            _nodeTemplateSelectorBinding.Source = this;

            _nodesChangedHandler = new NotifyCollectionChangedEventHandler(this.NodesCollectionChanged);

            _frameTickWired = false;

            _nodePresenters = new List<GraphContentPresenter>();
        }

        #region overrides
        protected override Size MeasureOverride(Size availableSize)
        {
            handleChanges();
            _measureInvalidated = true;
            wireFrameTick();

            for (int i = 0; i < _needsMeasure.Count; i++)
            {
                _needsMeasure[i].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            _needsMeasure.Clear();

            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _controlCenter.X = finalSize.Width / 2;
            _controlCenter.Y = finalSize.Height / 2;

            for (int i = 0; i < _needsArrange.Count; i++)
            {
                _needsArrange[i].Arrange(EmptyRect);
            }
            _needsArrange.Clear();

            return finalSize;
        }
        protected override int VisualChildrenCount
        {
            get
            {
                if (!_isChildCountValid)
                {
                    _childCount = 0;
                    if (_centerObjectPresenter != null)
                    {
                        _childCount++;
                    }
                    if (_nodePresenters != null)
                    {
                        _childCount += _nodePresenters.Count;
                    }
                    if (_fadingGCPs != null)
                    {
                        if (!_fadingGCPListValid)
                        {
                            _fadingGCPList.Clear();
                            Dictionary<int, GraphContentPresenter>.ValueCollection values = _fadingGCPs.Values;
                            foreach (GraphContentPresenter gcp in values)
                            {
                                _fadingGCPList.Add(gcp);
                            }
                            _fadingGCPListValid = true;
                        }
                        Debug.Assert(_fadingGCPList.Count == _fadingGCPs.Count);
                        _childCount += _fadingGCPList.Count;
                    }
                    _isChildCountValid = true;
                }

                return _childCount;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < _fadingGCPs.Count)
            {
                Debug.Assert(_fadingGCPListValid);
                return _fadingGCPList[index];
            }
            else
            {
                index -= _fadingGCPs.Count;
            }

            if (_nodePresenters != null)
            {
                if (index < _nodePresenters.Count)
                {
                    return _nodePresenters[index];
                }
                else
                {
                    index -= _nodePresenters.Count;
                }
            }

            if (index == 0)
            {
                return _centerObjectPresenter;
            }
            else
            {
                throw new Exception("not a valid index");
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (LinePen != null)
            {
                Pen p = LinePen;
                if (_nodePresenters != null && _centerObjectPresenter != null)
                {
                    for (int i = 0; i < _nodePresenters.Count; i++)
                    {
                        drawingContext.DrawLine(p, _centerObjectPresenter.ActualLocation, _nodePresenters[i].ActualLocation);
                    }
                }
            }
        }
        #endregion

        #region properties

        #region CenterObject
        public object CenterObject
        {
            get { return GetValue(CenterObjectProperty); }
            set { SetValue(CenterObjectProperty, value); }
        }
        public static readonly DependencyProperty CenterObjectProperty = DependencyProperty.Register(
            "CenterObject", typeof(object), typeof(Graph), getCenterObjectPropertyMetadata());

        #region CenterObject Impl

        private static PropertyMetadata getCenterObjectPropertyMetadata()
        {
            FrameworkPropertyMetadata fpm = new FrameworkPropertyMetadata();
            fpm.AffectsMeasure = true;
            fpm.PropertyChangedCallback = new PropertyChangedCallback(CenterObjectPropertyChanged);
            return fpm;
        }

        private static void CenterObjectPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            ((Graph)element).CenterObjectPropertyChanged(args);
        }

        private void CenterObjectPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            _centerChanged = true;
            resetNodesBinding();
        }

        #endregion

        #endregion

        #region NodesBindingPath
        public string NodesBindingPath
        {
            get
            {
                return (string)GetValue(NodesBindingPathProperty);
            }
            set
            {
                SetValue(NodesBindingPathProperty, value);
            }
        }
        public static readonly DependencyProperty NodesBindingPathProperty =
            DependencyProperty.Register("NodesBindingPath",
            typeof(string), typeof(Graph),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(NodesBindingPathPropertyChanged)));

        private static void NodesBindingPathPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            Graph g = (Graph)element;
            g.resetNodesBinding();
        }
        #endregion

        #region NodeTemplate
        public DataTemplate NodeTemplate
        {
            get
            {
                return (DataTemplate)GetValue(NodeTemplateProperty);
            }
            set
            {
                SetValue(NodeTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register(
            "NodeTemplate", typeof(DataTemplate), typeof(Graph), new FrameworkPropertyMetadata(null));
        #endregion

        #region NodeTemplateSelector
        public DataTemplateSelector NodeTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(NodeTemplateSelectorProperty);
            }
            set
            {
                SetValue(NodeTemplateSelectorProperty, value);
            }
        }

        public static readonly DependencyProperty NodeTemplateSelectorProperty = DependencyProperty.Register(
            "NodeTemplateSelector", typeof(DataTemplateSelector), typeof(Graph), new FrameworkPropertyMetadata(null));
        #endregion

        #region CoefficientOfDampening
        public double CoefficientOfDampening
        {
            get
            {
                return (double)GetValue(CoefficientOfDampeningProperty);
            }
            set
            {
                SetValue(CoefficientOfDampeningProperty, value);
            }
        }

        public static readonly DependencyProperty CoefficientOfDampeningProperty = DependencyProperty.Register("CoefficientOfDampening",
            typeof(double), typeof(Graph), new FrameworkPropertyMetadata(.9, null, new CoerceValueCallback(CoerceCoefficientOfDampeningPropertyCallback)));

        private static object CoerceCoefficientOfDampeningPropertyCallback(DependencyObject element, object baseValue)
        {
            return CoerceCoefficientOfDampeningPropertyCallback((double)baseValue);
        }

        private static double CoerceCoefficientOfDampeningPropertyCallback(double baseValue)
        {
            if (baseValue <= MinCOD)
            {
                return MinCOD;
            }
            else if (baseValue >= MaxCOD)
            {
                return MaxCOD;
            }
            else
            {
                return baseValue;
            }
        }
        #endregion

        #region FrameRate
        public double FrameRate
        {
            get { return (double)GetValue(FrameRateProperty); }
            set
            {
                SetValue(FrameRateProperty, value);
            }
        }

        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register("FrameRate",
    typeof(double), typeof(Graph), new FrameworkPropertyMetadata(.4, null, new CoerceValueCallback(CoerceFrameRatePropertyCallback)));

        private static object CoerceFrameRatePropertyCallback(DependencyObject element, object baseValue)
        {
            return CoerceFrameRatePropertyCallback((double)baseValue);
        }

        private static double CoerceFrameRatePropertyCallback(double baseValue)
        {
            if (baseValue <= MinCOD)
            {
                return MinCOD;
            }
            else if (baseValue >= MaxCOD)
            {
                return MaxCOD;
            }
            else
            {
                return baseValue;
            }
        }
        #endregion

        #region Line brush

        public Pen LinePen
        {
            get { return (Pen)GetValue(LinePenProperty); }
            set { SetValue(LinePenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinePen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register("LinePen", typeof(Pen), typeof(Graph), new PropertyMetadata(GetPen()));

        private static Pen GetPen()
        {
            if (DefaultPen == null)
            {
                DefaultPen = new Pen(Brushes.Gray, 1);
                DefaultPen.Freeze();
            }
            return DefaultPen;
        }

        private static Pen DefaultPen;

        #endregion


        #endregion

        #region implementation

        private void resetNodesBinding()
        {
            if (NodesBindingPath == null)
            {
                BindingOperations.ClearBinding(this, NodesProperty);
            }
            else
            {
                Binding theBinding = GetBinding(NodesBindingPath, this.CenterObject);
                if (theBinding == null)
                {
                    BindingOperations.ClearBinding(this, NodesProperty);
                }
                else
                {
                    BindingOperations.SetBinding(this, NodesProperty, theBinding);
                }
            }
        }
        private void wireFrameTick()
        {
            if (!_frameTickWired)
            {
                Debug.Assert(CheckAccess());
                CompositionTarget.Rendering += _compositionTarget_RenderingCallback;
                _frameTickWired = true;
            }
        }
        private void unwireFrameTick()
        {
            if (_frameTickWired)
            {
                Debug.Assert(CheckAccess());
                CompositionTarget.Rendering -= _compositionTarget_RenderingCallback;
                _frameTickWired = false;
            }
        }

        private void Graph_Unloaded(object sender, RoutedEventArgs e)
        {
            unwireFrameTick();
        }

        private void Graph_Loaded(object sender, RoutedEventArgs e)
        {
            wireFrameTick();
        }

        private readonly EventHandler _compositionTarget_RenderingCallback;

        private void compositionTarget_rendering(object sender, EventArgs args)
        {
            Debug.Assert(_nodePresenters != null);

            if (_springForces == null)
            {
                _springForces = SetupForceVertors(_nodePresenters.Count);
            }
            else if (_springForces.GetLowerBound(0) != _nodePresenters.Count)
            {
                _springForces = SetupForceVertors(_nodePresenters.Count);
            }

            bool _somethingInvalid = false;
            if (_measureInvalidated || _stillMoving)
            {
                if (_measureInvalidated)
                {
                    _ticksOfLastMeasureUpdate = Environment.TickCount;
                }

                #region CenterObject
                if (_centerObjectPresenter != null)
                {
                    if (_centerObjectPresenter.New)
                    {
                        _centerObjectPresenter.ParentCenter = _controlCenter;
                        _centerObjectPresenter.New = false;
                        _somethingInvalid = true;
                    }
                    else
                    {
                        Vector forceVector = GetAttractionForce(
                            ensureNonzeroVector((Vector)_centerObjectPresenter.Location));

                        if (updateGraphCP(_centerObjectPresenter, forceVector, CoefficientOfDampening, FrameRate, _controlCenter))
                        {
                            _somethingInvalid = true;
                        }
                    }
                }
                #endregion

                GraphContentPresenter gcp;
                for (int i = 0; i < _nodePresenters.Count; i++)
                {
                    gcp = _nodePresenters[i];

                    if (gcp.New)
                    {
                        gcp.New = false;
                        _somethingInvalid = true;
                    }

                    for (int j = (i + 1); j < _nodePresenters.Count; j++)
                    {
                        Vector distance = ensureNonzeroVector(gcp.Location - _nodePresenters[j].Location);

                        Vector repulsiveForce = GetRepulsiveForce(distance);//GetSpringForce(distance, gcp.Velocity - _nodePresenters[j].Velocity);
                        _springForces[i, j] = repulsiveForce;
                    }
                }

                Point centerLocationToUse = (_centerObjectPresenter != null) ? _centerObjectPresenter.Location : new Point();

                for (int i = 0; i < _nodePresenters.Count; i++)
                {
                    Vector forceVector = new Vector();
                    forceVector += GetVectorSum(i, _nodePresenters.Count, _springForces);
                    forceVector += GetSpringForce(ensureNonzeroVector(_nodePresenters[i].Location - centerLocationToUse));
                    forceVector += GetWallForce(this.RenderSize, _nodePresenters[i].Location);

                    if (updateGraphCP(_nodePresenters[i], forceVector, CoefficientOfDampening, FrameRate, _controlCenter))
                    {
                        _somethingInvalid = true;
                    }
                }

                #region animate all of the fading ones away
                for (int i = 0; i < _fadingGCPList.Count; i++)
                {
                    if (!_fadingGCPList[i].WasCenter)
                    {
                        Vector centerDiff = ensureNonzeroVector(_fadingGCPList[i].Location - centerLocationToUse);
                        centerDiff.Normalize();
                        centerDiff *= 20;
                        if (updateGraphCP(_fadingGCPList[i], centerDiff, CoefficientOfDampening, FrameRate, _controlCenter))
                        {
                            _somethingInvalid = true;
                        }
                    }
                }

                #endregion


                if (_somethingInvalid && belowMaxSettleTime())
                {
                    _stillMoving = true;
                    InvalidateVisual();
                }
                else
                {
                    _stillMoving = false;
                    unwireFrameTick();
                }
                _measureInvalidated = false;

            }
        }

        private bool belowMaxSettleTime()
        {
            Debug.Assert(_ticksOfLastMeasureUpdate != int.MinValue);
            return MaxSettleTime > TimeSpan.FromMilliseconds(Environment.TickCount - _ticksOfLastMeasureUpdate);
        }

        private static Vector ensureNonzeroVector(Vector vector)
        {
            if (vector.Length > 0)
            {
                return vector;
            }
            else
            {
                return new Vector(Rnd.NextDouble() - .5, Rnd.NextDouble() - .5);
            }
        }

        private static bool updateGraphCP(GraphContentPresenter graphContentPresenter, Vector forceVector,
                            double coefficientOfDampening, double frameRate, Point parentCenter)
        {
            bool parentCenterChanged = (graphContentPresenter.ParentCenter != parentCenter);
            if (parentCenterChanged)
            {
                graphContentPresenter.ParentCenter = parentCenter;
            }

            //add system drag
            Debug.Assert(coefficientOfDampening > 0);
            Debug.Assert(coefficientOfDampening < 1);
            graphContentPresenter.Velocity *= (1 - coefficientOfDampening * frameRate);

            //add force
            graphContentPresenter.Velocity += (forceVector * frameRate);

            //apply terminalVelocity
            if (graphContentPresenter.Velocity.Length > TerminalVelocity)
            {
                graphContentPresenter.Velocity *= (TerminalVelocity / graphContentPresenter.Velocity.Length);
            }

            if (graphContentPresenter.Velocity.Length > MinVelocity && forceVector.Length > MinVelocity)
            {
                graphContentPresenter.Location += (graphContentPresenter.Velocity * frameRate);
                return true;
            }
            else
            {
                graphContentPresenter.Velocity = new Vector();
                return false || parentCenterChanged;
            }
        }

        private static Vector[,] SetupForceVertors(int count)
        {
            return new Vector[count, count];
        }

        private void KillGCP(GraphContentPresenter gcp, bool isCenter)
        {
            Debug.Assert(VisualTreeHelper.GetParent(gcp) == this);

            this.InvalidateVisual();

            _fadingGCPs.Add(_fadingGCPsNextKey, gcp);
            _fadingGCPListValid = false;
            _isChildCountValid = false;

            int theKey = _fadingGCPsNextKey;

            gcp.IsHitTestVisible = false;
            if (isCenter)
            {
                gcp.WasCenter = true;
            }

            ScaleTransform st = gcp.ScaleTransform;

            DoubleAnimation da = GetNewHideAnimation(gcp, this, theKey);
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da);
            gcp.BeginAnimation(OpacityProperty, da);


            if (_fadingGCPsNextKey == int.MaxValue)
            {
                _fadingGCPsNextKey = int.MinValue;
            }
            else
            {
                _fadingGCPsNextKey++;
            }
        }
        private void CleanUpGCP(int key)
        {
            Debug.Assert(CheckAccess());
            if (_fadingGCPs.ContainsKey(key))
            {
                GraphContentPresenter gcp = _fadingGCPs[key];
                Debug.Assert(gcp != null);
                Debug.Assert(VisualTreeHelper.GetParent(gcp) == this);
                this.RemoveVisualChild(gcp);
                _fadingGCPListValid = false;
                _isChildCountValid = false;
                _fadingGCPs.Remove(key);
            }
        }
        private static DoubleAnimation GetNewHideAnimation(GraphContentPresenter element, Graph owner, int key)
        {
            DoubleAnimation da = new DoubleAnimation(0, HideDuration);
            da.FillBehavior = FillBehavior.Stop;
            //da.SetValue(Timeline.DesiredFrameRateProperty, HideDesiredFrameRate);
            HideAnimationManager ham = new HideAnimationManager(owner, key);
            da.Completed += new EventHandler(ham.CompletedHandler);
            da.Freeze();
            return da;
        }

        private void handleChanges()
        {
            handleNodesChangedWiring();

            if (_centerChanged && _nodeCollectionChanged &&
                CenterObject != null &&
                _centerObjectPresenter != null
                )
            {
                Debug.Assert(!CenterObject.Equals(_centerDataInUse));
                Debug.Assert(_centerObjectPresenter.Content == null || _centerObjectPresenter.Content.Equals(_centerDataInUse));

                _centerDataInUse = CenterObject;

                //figure out if we can re-cycle one of the existing children as the center Node
                //if we can, newCenter != null
                GraphContentPresenter newCenterPresenter = null;
                for (int i = 0; i < _nodePresenters.Count; i++)
                {
                    if (_nodePresenters[i].Content.Equals(CenterObject))
                    {
                        //we should re-use this 
                        newCenterPresenter = _nodePresenters[i];
                        _nodePresenters[i] = null;
                        break;
                    }
                }

                //figure out if we can re-cycle the exsting center as one of the new child nodes
                //if we can, newChild != null && newChildIndex == indexOf(data in Nodes)
                int newChildIndex = -1;
                GraphContentPresenter newChildPresenter = null;
                for (int i = 0; i < _nodesInUse.Count; i++)
                {
                    if (_nodesInUse[i] != null && _centerObjectPresenter.Content != null && _nodesInUse[i].Equals(_centerObjectPresenter.Content))
                    {
                        newChildIndex = i;
                        newChildPresenter = _centerObjectPresenter;
                        _centerObjectPresenter = null;
                        break;
                    }
                }

                //now we potentially have a center (or not) and one edge(or not)
                GraphContentPresenter[] newChildren = new GraphContentPresenter[_nodesInUse.Count];

                //we did all the work to see if the current cernter can be reused.
                //if it can, use it
                if (newChildPresenter != null)
                {
                    newChildren[newChildIndex] = newChildPresenter;
                }

                //now go through all the existing children and place them in newChildren
                //if they match
                for (int i = 0; i < _nodesInUse.Count; i++)
                {
                    if (newChildren[i] == null)
                    {
                        for (int j = 0; j < _nodePresenters.Count; j++)
                        {
                            if (_nodePresenters[j] != null)
                            {
                                if (_nodesInUse[i].Equals(_nodePresenters[j].Content))
                                {
                                    Debug.Assert(newChildren[i] == null);
                                    newChildren[i] = _nodePresenters[j];
                                    _nodePresenters[j] = null;
                                    break;
                                }
                            }
                        }
                    }
                }

                //we've now reused everything we can
                if (_centerObjectPresenter == null)
                {
                    //we didn't find anything to recycle
                    //create a new one
                    if (newCenterPresenter == null)
                    {
                        _centerObjectPresenter = GetGraphContentPresenter(CenterObject,
                            _nodeTemplateBinding, _nodeTemplateSelectorBinding, false
                            );
                        this.AddVisualChild(_centerObjectPresenter);
                    }
                    else //we did find something to recycle. Use it.
                    {
                        _centerObjectPresenter = newCenterPresenter;
                        Debug.Assert(VisualTreeHelper.GetParent(newCenterPresenter) == this);
                    }
                }
                else
                {
                    if (newCenterPresenter == null)
                    {
                        _centerObjectPresenter.Content = CenterObject;
                    }
                    else
                    {
                        KillGCP(_centerObjectPresenter, true);
                        _centerObjectPresenter = newCenterPresenter;
                        Debug.Assert(VisualTreeHelper.GetParent(newCenterPresenter) == this);
                    }
                }

                //go through all of the old CPs that are not being used and remove them
                for (int i = 0; i < _nodePresenters.Count; i++)
                {
                    if (_nodePresenters[i] != null)
                    {
                        KillGCP(_nodePresenters[i], false);
                    }
                }

                //go through and "fill in" all the new CPs
                for (int i = 0; i < _nodesInUse.Count; i++)
                {
                    if (newChildren[i] == null)
                    {
                        GraphContentPresenter gcp = GetGraphContentPresenter(_nodesInUse[i],
                            _nodeTemplateBinding, _nodeTemplateSelectorBinding, true);
                        this.AddVisualChild(gcp);
                        newChildren[i] = gcp;
                    }
                }

                _nodePresenters.Clear();
                _nodePresenters.AddRange(newChildren);

                _isChildCountValid = false;

                _centerChanged = false;
                _nodeCollectionChanged = false;
            }
            else
            {
                if (_centerChanged)
                {
                    _centerDataInUse = CenterObject;
                    if (_centerObjectPresenter != null)
                    {
                        Debug.Assert(_centerDataInUse == null);
                        KillGCP(_centerObjectPresenter, true);
                        _centerObjectPresenter = null;
                    }
                    if (_centerDataInUse != null)
                    {
                        SetUpCleanCenter(_centerDataInUse);
                    }
                    _centerChanged = false;
                }

                if (_nodeCollectionChanged)
                {
                    SetupNodes(Nodes);

                    _nodesInUse = Nodes;

                    _nodeCollectionChanged = false;
                }
            }

#if DEBUG
            if (CenterObject != null)
            {
                CenterObject.Equals(_centerDataInUse);
                Debug.Assert(_centerObjectPresenter != null);
            }
            else
            {
                Debug.Assert(_centerDataInUse == null);
            }
            if (Nodes != null)
            {
                Debug.Assert(_nodePresenters != null);
                Debug.Assert(Nodes.Count == _nodePresenters.Count);
                Debug.Assert(_nodesInUse == Nodes);
            }
            else
            {
                Debug.Assert(_nodesInUse == null);
                if (_nodePresenters != null)
                {
                    Debug.Assert(_nodePresenters.Count == 0);
                }
            }
#endif

        }

        private void handleNodesChangedWiring()
        {
            if (_nodesChanged)
            {
                INotifyCollectionChanged oldList = _nodesInUse as INotifyCollectionChanged;
                if (oldList != null)
                {
                    oldList.CollectionChanged -= _nodesChangedHandler;
                }

                INotifyCollectionChanged newList = Nodes as INotifyCollectionChanged;
                if (newList != null)
                {
                    newList.CollectionChanged += _nodesChangedHandler;
                }

                _nodesInUse = Nodes;
                _nodesChanged = false;
            }
        }

        private void SetupNodes(IList nodes)
        {
#if DEBUG
            for (int i = 0; i < _nodePresenters.Count; i++)
            {
                Debug.Assert(_nodePresenters[i] != null);
                Debug.Assert(VisualTreeHelper.GetParent(_nodePresenters[i]) == this);
            }
#endif

            int nodesCount = (nodes == null) ? 0 : nodes.Count;

            GraphContentPresenter[] newNodes = new GraphContentPresenter[nodesCount];
            for (int i = 0; i < nodesCount; i++)
            {
                for (int j = 0; j < _nodePresenters.Count; j++)
                {
                    if (_nodePresenters[j] != null)
                    {
                        if (nodes[i] == _nodePresenters[j].Content)
                        {
                            newNodes[i] = _nodePresenters[j];
                            _nodePresenters[j] = null;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < _nodePresenters.Count; i++)
            {
                if (_nodePresenters[i] != null)
                {
                    KillGCP(_nodePresenters[i], false);
                    _nodePresenters[i] = null;
                }
            }

            for (int i = 0; i < newNodes.Length; i++)
            {
                if (newNodes[i] == null)
                {
                    newNodes[i] = GetGraphContentPresenter(nodes[i],
                        _nodeTemplateBinding, _nodeTemplateSelectorBinding, true);
                    this.AddVisualChild(newNodes[i]);
                }
            }

#if DEBUG
            for (int i = 0; i < _nodePresenters.Count; i++)
            {
                Debug.Assert(_nodePresenters[i] == null);
            }
            for (int i = 0; i < newNodes.Length; i++)
            {
                Debug.Assert(newNodes[i] != null);
                Debug.Assert(VisualTreeHelper.GetParent(newNodes[i]) == this);
                Debug.Assert(newNodes[i].Content == nodes[i]);
            }
#endif

            _nodePresenters.Clear();
            _nodePresenters.AddRange(newNodes);
            _isChildCountValid = false;
        }

        private void SetUpCleanCenter(object newCenter)
        {
            Debug.Assert(_centerObjectPresenter == null);

            _centerObjectPresenter = GetGraphContentPresenter(newCenter, _nodeTemplateBinding, _nodeTemplateSelectorBinding, false);
            this.AddVisualChild(_centerObjectPresenter);

            _isChildCountValid = false;
        }

        #region private Nodes property
        private IList Nodes
        {
            get
            {
                return (IList)GetValue(NodesProperty);
            }
        }
        private static readonly DependencyProperty NodesProperty = DependencyProperty.Register(
            "Nodes", typeof(IList) , typeof(Graph) , getNodesPropertyMetadata());

        private static PropertyMetadata getNodesPropertyMetadata()
        {
            FrameworkPropertyMetadata fpm = new FrameworkPropertyMetadata();
            fpm.AffectsMeasure = true;
            fpm.PropertyChangedCallback = new PropertyChangedCallback(NodesPropertyChanged);
            return fpm;
        }

        private static void NodesPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            ((Graph)element).NodesPropertyChanged(args);
        }
        private void NodesPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            _nodeCollectionChanged = true;
            _nodesChanged = true;
        }
        #endregion

        private void NodesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            VerifyAccess();
            InvalidateMeasure();
            _nodeCollectionChanged = true;
        }

        private GraphContentPresenter GetGraphContentPresenter(object content, BindingBase nodeTemplateBinding,
            BindingBase nodeTemplateSelectorBinding, bool offsetCenter)
        {
            GraphContentPresenter gcp = new GraphContentPresenter(content, nodeTemplateBinding, nodeTemplateSelectorBinding, offsetCenter);

            _needsMeasure.Add(gcp);
            _needsArrange.Add(gcp);

            return gcp;
        }

        private readonly NotifyCollectionChangedEventHandler _nodesChangedHandler;

        private int _childCount;
        private bool _isChildCountValid;

        private object _centerDataInUse;
        private IList _nodesInUse;

        private bool _centerChanged;
        private bool _nodesChanged;
        private bool _nodeCollectionChanged;

        private GraphContentPresenter _centerObjectPresenter;
        private readonly List<GraphContentPresenter> _nodePresenters;

        private readonly Dictionary<int, GraphContentPresenter> _fadingGCPs;
        private int _fadingGCPsNextKey;
        private bool _fadingGCPListValid = false;
        private readonly List<GraphContentPresenter> _fadingGCPList = new List<GraphContentPresenter>();


        private bool _measureInvalidated = false;
        private bool _stillMoving = false;
        private Vector[,] _springForces;
        private Point _controlCenter;

        private readonly Binding _nodeTemplateBinding;
        private readonly Binding _nodeTemplateSelectorBinding;

        private int _ticksOfLastMeasureUpdate = int.MinValue;

        private bool _frameTickWired;

        private readonly List<GraphContentPresenter> _needsMeasure = new List<GraphContentPresenter>();
        private readonly List<GraphContentPresenter> _needsArrange = new List<GraphContentPresenter>();

        #endregion

        private class HideAnimationManager
        {
            public HideAnimationManager(Graph owner, int key)
            {
                _owner = owner;
                _key = key;
            }
            public void CompletedHandler(object sender, EventArgs args)
            {
#if DEBUG
                _owner.VerifyAccess();
#endif
                _owner.CleanUpGCP(_key);
            }

            private Graph _owner;
            private int _key;
        }

        private class GraphContentPresenter : ContentPresenter
        {

            public GraphContentPresenter(object content,
            BindingBase nodeTemplateBinding, BindingBase nodeTemplateSelectorBinding, bool offsetCenter)
                : base()
            {
                base.Content = content;


                base.SetBinding(ContentPresenter.ContentTemplateProperty, nodeTemplateBinding);
                base.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, nodeTemplateSelectorBinding);


                ScaleTransform = new ScaleTransform();
                if (offsetCenter)
                {
                    _translateTransform = new TranslateTransform(Rnd.NextDouble() - .5, Rnd.NextDouble() - .5);
                }
                else
                {
                    _translateTransform = new TranslateTransform();
                }

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(ScaleTransform);
                tg.Children.Add(_translateTransform);

                this.RenderTransform = tg;

                DoubleAnimation da = new DoubleAnimation(.5, 1, ShowDuration);
                this.BeginAnimation(OpacityProperty, da);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);
            }

            protected override Size MeasureOverride(Size constraint)
            {
                _actualDesiredSize = base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
                return new Size();
            }

            protected override Size ArrangeOverride(Size arrangeSize)
            {
                _actualRenderSize = base.ArrangeOverride(_actualDesiredSize);

                ScaleTransform.CenterX = this._actualRenderSize.Width / 2;
                ScaleTransform.CenterY = this._actualRenderSize.Height / 2;

                _centerVector.X = -this._actualRenderSize.Width / 2;
                _centerVector.Y = -this._actualRenderSize.Height / 2;

                updateTransform();

                return new Size();
            }

            public bool New = true;
            public Point Location
            {
                get { return _location; }
                set
                {
                    if (_location != value)
                    {
                        _location = value;
                        updateTransform();
                    }
                }
            }

            public Point ParentCenter
            {
                get
                {
                    return _parentCenter;
                }
                set
                {
                    if (_parentCenter != value)
                    {
                        _parentCenter = value;
                        updateTransform();
                    }
                }
            }

            public Point ActualLocation
            {
                get
                {
                    return new Point(_location.X + _parentCenter.X, _location.Y + _parentCenter.Y);
                }
            }

            public Vector Velocity;
            public bool WasCenter = false;
            public ScaleTransform ScaleTransform;

            private void updateTransform()
            {
                _translateTransform.X = _centerVector.X + _location.X + _parentCenter.X;
                _translateTransform.Y = _centerVector.Y + _location.Y + _parentCenter.Y;
            }

            private Point _location;

            private Size _actualDesiredSize;
            private Size _actualRenderSize;

            private Vector _centerVector;
            private Point _parentCenter;

            private TranslateTransform _translateTransform;
        }

        #region Static Stuff

        private static Binding GetBinding(string bindingPath, object source)
        {
            Binding newBinding = null;
            try
            {
                newBinding = new Binding(bindingPath);
                newBinding.Source = source;
                newBinding.Mode = BindingMode.OneWay;
            }
            catch (InvalidOperationException) { }
            return newBinding;
        }

        private static PropertyPath ClonePropertyPath(PropertyPath path)
        {
            return new PropertyPath(path.Path, path.PathParameters);
        }

        #region static helpers

        private static Vector GetVectorSum(int itemIndex, int itemCount, Vector[,] vectors)
        {
            Debug.Assert(itemIndex >= 0);
            Debug.Assert(itemIndex < itemCount);

            Vector vector = new Vector();

            for (int i = 0; i < itemCount; i++)
            {
                if (i != itemIndex)
                {
                    vector += GetVector(itemIndex, i, itemCount, vectors);
                }
            }

            return vector;
        }

        private static Vector GetVector(int a, int b, int count, Vector[,] vectors)
        {
            Debug.Assert(a != b);
            if (a < b)
            {
                return vectors[a, b];
            }
            else
            {
                return -vectors[b, a];
            }
        }

        private static Point GetRandomPoint(Size range)
        {
            return new Point(Rnd.NextDouble() * range.Width, Rnd.NextDouble() * range.Height);
        }

        private static readonly Random Rnd = new Random();

        private static Rect GetCenteredRect(Size elementSize, Point center)
        {
            double x = center.X - elementSize.Width / 2;
            double y = center.Y - elementSize.Height / 2;

            return new Rect(x, y, elementSize.Width, elementSize.Height);
        }

        private static Vector GetSpringForce(Vector x)
        {
            Vector force = new Vector();
            //negative is attraction
            force += GetAttractionForce(x);
            //positive is repulsion
            force += GetRepulsiveForce(x);

            Debug.Assert(IsGoodVector(force));

            return force;
        }

        private static Vector GetAttractionForce(Vector x)
        {
            Vector force = -.2 * Normalize(x) * x.Length;
            Debug.Assert(IsGoodVector(force));
            return force;
        }

        private static Vector GetRepulsiveForce(Vector x)
        {
            Vector force = .1 * Normalize(x) / Math.Pow(x.Length / 1000, 2);
            Debug.Assert(IsGoodVector(force));
            return force;
        }

        private static Vector Normalize(Vector v)
        {
            v.Normalize();
            Debug.Assert(IsGoodVector(v));
            return v;
        }

        private static Vector GetWallForce(Size area, Point location)
        {
            Vector force = new Vector();
            force += (VerticalVector * GetForce(-location.Y - area.Height / 2));
            force += (-VerticalVector * GetForce(location.Y - area.Height / 2));

            force += (HorizontalVector * GetForce(-location.X - area.Width / 2));
            force += (-HorizontalVector * GetForce(location.X - area.Width / 2));

            force *= 1000;
            return force;
        }
        private static double GetForce(double x)
        {
            return GetSCurve((x + 100) / 200);
        }

        private static bool IsGoodDouble(double d)
        {
            return !double.IsNaN(d) && !double.IsInfinity(d);
        }

        private static bool IsGoodVector(Vector v)
        {
            return IsGoodDouble(v.X) && IsGoodDouble(v.Y);
        }

        #region math
        private static double GetSCurve(double x)
        {
            return 0.5 + Math.Sin(Math.Abs(x * (Math.PI / 2)) - Math.Abs((x * (Math.PI / 2)) - (Math.PI / 2))) / 2;
        }
        #endregion

        #endregion

        private static readonly Vector VerticalVector = new Vector(0, 1);
        private static readonly Vector HorizontalVector = new Vector(1, 0);
        private static readonly Duration HideDuration = new Duration(new TimeSpan(0, 0 , 1));
        private static readonly Duration ShowDuration = new Duration(new TimeSpan(0, 0 , 0 , 0 , 500));

        private static readonly TimeSpan MaxSettleTime = new TimeSpan(0, 0 , 8);

        private const double TerminalVelocity = 150;
        private const double MinVelocity = .05;

        private const double MinCOD = .001, MaxCOD = .999;

        private static readonly Rect EmptyRect = new Rect();

        #endregion
    }
}
