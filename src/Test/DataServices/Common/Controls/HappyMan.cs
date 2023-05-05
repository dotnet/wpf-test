// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
	using System.Collections;

    /// <summary>
    /// Test element to verify databinding
    /// </summary>
    public class HappyMan : FrameworkElement
    {


		bool _updatingLinkedProperties;
		PropertyCache _cache;

 		/// <summary>
        /// Creates a new HappyMan element
        /// </summary>
        public HappyMan() {
			_cache = new PropertyCache(this);
            this.MouseLeftButtonUp += new MouseButtonEventHandler (OnMouseUp);
        }
        


		#region Public Members

        /// <summary>
        /// Gets or sets the HappyName of the HappyMan
        /// </summary>
        public string HappyName {
            get{ return (string)_cache.GetValue(HappyNameProperty); }
            set{ SetValue(HappyNameProperty, value); }
        }

		/// <summary>
		/// Gets or sets point where the eyes are looking
		/// </summary>
		public Point LookAt {
			get { return (Point)_cache.GetValue(LookAtProperty); }
			set { SetValue(LookAtProperty, value); }
		}

        /// <summary>
        /// Gets or sets the Position of the HappyMan on the Canvas
        /// </summary>
		public Point Position {
			get { return (Point)_cache.GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}

        /// <summary>
		/// Gets or sets the color of the eyes of the Happyman
        /// </summary>
		public Color EyeColor {
			get { return (Color)_cache.GetValue(EyeColorProperty); }
			set { SetValue(EyeColorProperty, value); }
		}

		/// <summary>
		/// Gets or sets the color of the skin of the Happyman
		/// </summary>
		public Color SkinColor {
			get { return (Color)_cache.GetValue(SkinColorProperty); }
			set { SetValue(SkinColorProperty, value); }
		}

		#endregion


		#region Dependency Property and Metadata Registration

		public static readonly DependencyProperty HappyNameProperty = DependencyProperty.RegisterAttached ("HappyName", typeof(string), typeof(HappyMan));
        public static readonly DependencyProperty EyeColorProperty = DependencyProperty.RegisterAttached ("EyeColor", typeof(Color), typeof(HappyMan));
        public static readonly DependencyProperty LookAtProperty = DependencyProperty.RegisterAttached ("LookAt",typeof(Point), typeof(HappyMan));
		public static readonly DependencyProperty SkinColorProperty = DependencyProperty.RegisterAttached("SkinColor", typeof(Color), typeof(HappyMan));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached ("Position",  typeof(Point), typeof(HappyMan));

		//Static constructor for metadata registration
		static HappyMan() {
            FrameworkPropertyMetadata metadata;
            //HappyName
            metadata = new FrameworkPropertyMetadata("");
			metadata.AffectsArrange = true;
			metadata.AffectsParentMeasure = true;

            HappyNameProperty.OverrideMetadata(typeof(HappyMan), metadata);

			//LookAt
            metadata = new FrameworkPropertyMetadata(new Point(50, 50));
            metadata.AffectsRender = true;
            LookAtProperty.OverrideMetadata(typeof(HappyMan), metadata);

			//SkinColor
			metadata = new FrameworkPropertyMetadata(Colors.Yellow);
            metadata.AffectsRender = true;
            SkinColorProperty.OverrideMetadata(typeof(HappyMan), metadata);
			
			//EyeColor
            metadata = new FrameworkPropertyMetadata(Colors.White);
            metadata.AffectsRender = true;
	    EyeColorProperty.OverrideMetadata( typeof(HappyMan), metadata);

			//Position
			metadata = new FrameworkPropertyMetadata(new Point(50, 50));
			metadata.AffectsArrange = true;
			metadata.AffectsRender = true;
			metadata.PropertyChangedCallback += new PropertyChangedCallback(OnPositionChange);
			PositionProperty.OverrideMetadata( typeof(HappyMan), metadata);

			/***  linked properties  ***/
			//Left
			metadata = new FrameworkPropertyMetadata(0.0);
			metadata.AffectsParentArrange = true;
			metadata.PropertyChangedCallback += new PropertyChangedCallback(OnLocationChange);
			Canvas.LeftProperty.OverrideMetadata( typeof(HappyMan), metadata);

			//Top
			metadata = new FrameworkPropertyMetadata(0.0);
			metadata.AffectsParentArrange = true;
			metadata.PropertyChangedCallback += new PropertyChangedCallback(OnLocationChange);
			Canvas.TopProperty.OverrideMetadata( typeof(HappyMan), metadata);

			//Width
			metadata = new FrameworkPropertyMetadata(100.0);
			metadata.AffectsParentMeasure = true;
			metadata.PropertyChangedCallback += new PropertyChangedCallback(OnWidthChange);
			Canvas.WidthProperty.OverrideMetadata( typeof(HappyMan), metadata);

			//Height
			metadata = new FrameworkPropertyMetadata(100.0);
			metadata.AffectsParentMeasure = true;
			metadata.PropertyChangedCallback += new PropertyChangedCallback(OnHeightChange);
			Canvas.HeightProperty.OverrideMetadata( typeof(HappyMan), metadata);

			/***  properties that affect rendering  ***/
			metadata = new FrameworkPropertyMetadata(false);
			metadata.AffectsRender = true;
			//HappyMan.IsKeyboardFocusedProperty.OverrideMetadata( typeof(HappyMan), metadata);

			FocusableProperty.OverrideMetadata(typeof(HappyMan), new FrameworkPropertyMetadata(true));

		}


		static void OnPositionChange(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			HappyMan hm = obj as HappyMan;

			if (!hm._updatingLinkedProperties) {
				//WORKAROUND: Width & Height do not return the value set by the user value
                //Length width = (Length)hm.GetValue(Canvas.WidthProperty);
                //Length height = (Length)hm.GetValue(Canvas.HeightProperty);
                double width = (double)hm.GetValue(Canvas.WidthProperty);
                double height = (double)hm.GetValue(Canvas.HeightProperty);
                
                hm._updatingLinkedProperties = true;
                //hm.SetValue(Canvas.LeftProperty, new Length(hm.Position.X - width / 2));
                //hm.SetValue(Canvas.TopProperty, new Length(hm.Position.Y - height / 2));
                double cLeft = (hm.Position.X - width / 2);
                double cTop = (hm.Position.Y - height / 2);
                hm.SetValue(Canvas.LeftProperty, cLeft);
                hm.SetValue(Canvas.TopProperty, cTop);
                
                hm._updatingLinkedProperties = false;
			}
		}


		static void OnLocationChange(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			HappyMan hm = obj as HappyMan;

			if (!hm._updatingLinkedProperties) {
				//WORKAROUND: Width & Height do not return the value set by the user value
                //Length width = (Length)hm.GetValue(Canvas.WidthProperty);
                //Length height = (Length)hm.GetValue(Canvas.HeightProperty);
                double width = (double)hm.GetValue(Canvas.WidthProperty);
                double height = (double)hm.GetValue(Canvas.HeightProperty);

                hm._updatingLinkedProperties = true;
                //Length top = (Length)hm.GetValue(Canvas.TopProperty);
                //Length left = (Length)hm.GetValue(Canvas.LeftProperty);
                double top = (double)hm.GetValue(Canvas.TopProperty);
                double left = (double)hm.GetValue(Canvas.LeftProperty);

                hm.Position = new Point(left + width / 2, top + height / 2);
				hm._updatingLinkedProperties = false;
			}
		}


		static void OnWidthChange(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			HappyMan hm = obj as HappyMan;

			if (!hm._updatingLinkedProperties) {
				//WORKAROUND: Width & Height do not return the value set by the user value
                //Length width = (Length)hm.GetValue(Canvas.WidthProperty);
                //Length height = (Length)hm.GetValue(Canvas.HeightProperty);
                double width = (double)hm.GetValue(Canvas.WidthProperty);
                double height = (double)hm.GetValue(Canvas.HeightProperty);

                hm._updatingLinkedProperties = true;
				//hm.SetValue(Canvas.LeftProperty, new Length(hm.Position.X - (width * 0.5)));
                double cLeft = (hm.Position.X - (width * 0.5));
                hm.SetValue(Canvas.LeftProperty, cLeft);
                hm._updatingLinkedProperties = false;
			}
		}

		static void OnHeightChange(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			HappyMan hm = obj as HappyMan;

			if (!hm._updatingLinkedProperties) {
				//WORKAROUND: Width & Height do not return the value set by the user value
                //Length width = (Length)hm.GetValue(Canvas.WidthProperty);
                //Length height = (Length)hm.GetValue(Canvas.HeightProperty);
                double width = (double)hm.GetValue(Canvas.WidthProperty);
                double height = (double)hm.GetValue(Canvas.HeightProperty);

                hm._updatingLinkedProperties = true;
                double cTop = (hm.Position.Y - (height * 0.5));
				hm.SetValue(Canvas.TopProperty, cTop);
				hm._updatingLinkedProperties = false;
			}
		}

		
		#endregion

	
		#region Rendering

        protected override void OnRender(DrawingContext ctx) {
            double eyeSize = Width / 7 + 4;
            Color borderColor = Colors.Black;

            if (this.IsKeyboardFocused)
                borderColor = Colors.Red;

            //Head
            ctx.DrawGeometry(new SolidColorBrush(SkinColor), new Pen (new SolidColorBrush (borderColor), 2), new EllipseGeometry (new Point (Width / 2, Height / 2), Width / 2 - 1, Height / 2 - 1));

            //Left Eye
            DrawEye(ctx, new Point(Width / 3, Height / 3), eyeSize);

            //Right eye
            DrawEye(ctx, new Point(2 * Width / 3, Height / 3), eyeSize);

            //Nose
            ctx.DrawLine (new Pen (new SolidColorBrush (Colors.Black), 3), new Point (Width / 2, Height / 2 - eyeSize / 3), new Point (Width / 2, Height / 2 + eyeSize / 3));









            //Draw HappyName
            FormattedText txt = new FormattedText(HappyName, Language.GetSpecificCulture(), FlowDirection.LeftToRight, new Typeface("Arial"), 20, Brushes.Tomato);

            ctx.DrawText (txt, new Point (Width / 2 - txt.Width / 2, Height + 2));
        }

        void DrawEye (DrawingContext ctx, Point location, double size) {
            //eye
            ctx.DrawGeometry (new SolidColorBrush (EyeColor), new Pen (new SolidColorBrush (Colors.Black), 1), new EllipseGeometry (location, size / 2, size / 2));

            //Length left = (Length)GetValue (Canvas.LeftProperty);
            //Length top = (Length)GetValue (Canvas.TopProperty);
            double left = (double)GetValue(Canvas.LeftProperty);
            double top = (double)GetValue(Canvas.TopProperty);

            //get the angle
            double rads = Math.Atan2 (LookAt.Y - (Position.Y + location.Y - Height / 2), LookAt.X - (Position.X + location.X - Width / 2));
            double pupSize = size / 2;
            Point ploc = new Point (location.X + Math.Cos (rads) * (size - pupSize) / 2, location.Y + Math.Sin (rads) * (size - pupSize) / 2);

            //pupil
            ctx.DrawGeometry (Brushes.Black, new Pen (new SolidColorBrush (Colors.Black), 1), new EllipseGeometry (ploc, pupSize / 2, pupSize / 2));
        }

		#endregion

		
		//Focus on click
		void OnMouseUp(object sender, MouseButtonEventArgs e) {
			if (!IsKeyboardFocused)
				Focus();
		}
 
    }


	#region Property Cache

	//Generic Property Cache for elements
	internal class PropertyCache {
		
		DependencyObject _dependencyObject;
		Hashtable _isValid = new Hashtable();
		Hashtable _cache = new Hashtable();

		public PropertyCache(DependencyObject dependencyObject){
			this._dependencyObject = dependencyObject;
		}

		public void InvalidateProperty(DependencyProperty dp) {
			if (_isValid.ContainsKey(dp)) {
				_isValid[dp] = false;
			}
		}

		public object GetValue(DependencyProperty dp) {
			//Can't use Cache because there is no way to determin which property changed
			//if (!isValid.ContainsKey(dp) || !(bool)isValid[dp]) {
				_cache[dp] = _dependencyObject.GetValue(dp);
				_isValid[dp] = true;
			//}
			return _cache[dp];
		}

	}

	#endregion

}

