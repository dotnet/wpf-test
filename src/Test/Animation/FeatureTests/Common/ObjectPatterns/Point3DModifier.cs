// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

//------------------------------------------------------------------

namespace                       Microsoft.Test.Animation.ObjectPatterns
{
    //--------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    public class                Point3DModifier              : System.Windows.Media.Animation.Point3DAnimationBase
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public                  Point3DModifier ( ModifierController c, double dx, double dy, double dz )
        {
            _controller = c;
            _deltaX = dx;
            _deltaY = dy;
            _deltaZ = dz;
        }

        /// <summary>
        /// 
        /// </summary>
        protected Point3DModifier()
        {
        }
       
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            Point3DModifier point3DModifier = (Point3DModifier)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _controller = point3DModifier._controller;
            _deltaX = point3DModifier._deltaX;
            _deltaY = point3DModifier._deltaY;
            _deltaZ = point3DModifier._deltaZ;
  
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Point3DModifier point3DModifier = (Point3DModifier)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _controller = point3DModifier._controller;
            _deltaX = point3DModifier._deltaX;
            _deltaY = point3DModifier._deltaY;
            _deltaZ = point3DModifier._deltaZ;

        }
        /// <summary>
        /// 
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            Point3DModifier point3DModifier = (Point3DModifier)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _controller = point3DModifier._controller;
            _deltaX = point3DModifier._deltaX;
            _deltaY = point3DModifier._deltaY;
            _deltaZ = point3DModifier._deltaZ;

        }
        /// <summary>
        /// 
        /// </summary>
        public new Point3DModifier GetAsFrozen()
        {
            return (Point3DModifier)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new Point3DModifier();
        }
     

        /// <summary>
        /// 
        /// </summary>
        protected override System.Windows.Media.Media3D.Point3D
                                GetCurrentValueCore (System.Windows.Media.Media3D.Point3D defaultOriginValue, System.Windows.Media.Media3D.Point3D baseValue , System.Windows.Media.Animation.AnimationClock clock )
        {
            if ( !_controller.UsesBaseValue )
            {
                return new System.Windows.Media.Media3D.Point3D ( _deltaX, _deltaY, _deltaZ );
            }
            else
            {
                return new System.Windows.Media.Media3D.Point3D ( baseValue.X + _deltaX, baseValue.Y + _deltaY, baseValue.Z + _deltaZ );
            }
        }

        //----------------------------------------------------------

        private ModifierController  _controller;
        private double              _deltaX;
        private double              _deltaY;
        private double              _deltaZ;
    }
}
