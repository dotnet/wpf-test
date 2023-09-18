// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for DefaultValue (generated)
*
*   !!! This is a generated file. Do NOT edit it manually !!!
*
************************************************************/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes; using Microsoft.Test.Graphics;
using Microsoft.Test.Graphics.Factories;
using System.Windows.Controls;

//------------------------------------------------------------------

namespace                       Microsoft.Test.Graphics.Generated
{
    //--------------------------------------------------------------

    public partial class        DefaultValueTest : CoreGraphicsTest
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    Init( Variation v )
        {
            base.Init( v );


        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    RunTheTest()
        {
            TestAmbientLight();
            TestAxisAngleRotation3D();
            TestDiffuseMaterial();
            TestDirectionalLight();
            TestEmissiveMaterial();
            TestGeometryModel3D();
            TestMaterialGroup();
            TestMatrixCamera();
            TestMatrixTransform3D();
            TestMeshGeometry3D();
            TestModel3DGroup();
            TestModelVisual3D();
            TestOrthographicCamera();
            TestPerspectiveCamera();
            TestPointLight();
            TestQuaternionRotation3D();
            TestRotateTransform3D();
            TestScaleTransform3D();
            TestSpecularMaterial();
            TestSpotLight();
            TestTransform3DGroup();
            TestTranslateTransform3D();
            TestViewport3DVisual();
            TestViewport3D();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestAmbientLight()
        {
            AmbientLight ambientLight = new AmbientLight();

            if ( MathEx.NotEquals( ambientLight.Color, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for AmbientLight.Color should be Colors.White" );
                Log( "*** Actual: {0}", ambientLight.Color );
            }
            if ( MathEx.NotEquals( (Color)AmbientLight.ColorProperty.DefaultMetadata.DefaultValue, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for AmbientLight.ColorProperty should be Colors.White" );
                Log( "*** Actual: {0}", AmbientLight.ColorProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( ambientLight.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for AmbientLight.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", ambientLight.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( AmbientLight.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for AmbientLight.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", AmbientLight.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestAxisAngleRotation3D()
        {
            AxisAngleRotation3D axisAngleRotation3D = new AxisAngleRotation3D();

            if ( MathEx.NotEquals( axisAngleRotation3D.Angle, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for AxisAngleRotation3D.Angle should be 0" );
                Log( "*** Actual: {0}", axisAngleRotation3D.Angle );
            }
            if ( MathEx.NotEquals( (double)AxisAngleRotation3D.AngleProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for AxisAngleRotation3D.AngleProperty should be 0" );
                Log( "*** Actual: {0}", AxisAngleRotation3D.AngleProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( axisAngleRotation3D.Axis, Const.yAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for AxisAngleRotation3D.Axis should be Const.yAxis" );
                Log( "*** Actual: {0}", axisAngleRotation3D.Axis );
            }
            if ( MathEx.NotEquals( (Vector3D)AxisAngleRotation3D.AxisProperty.DefaultMetadata.DefaultValue, Const.yAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for AxisAngleRotation3D.AxisProperty should be Const.yAxis" );
                Log( "*** Actual: {0}", AxisAngleRotation3D.AxisProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestDiffuseMaterial()
        {
            DiffuseMaterial diffuseMaterial = new DiffuseMaterial();

            if ( MathEx.NotEquals( diffuseMaterial.AmbientColor, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for DiffuseMaterial.AmbientColor should be Colors.White" );
                Log( "*** Actual: {0}", diffuseMaterial.AmbientColor );
            }
            if ( MathEx.NotEquals( (Color)DiffuseMaterial.AmbientColorProperty.DefaultMetadata.DefaultValue, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for DiffuseMaterial.AmbientColorProperty should be Colors.White" );
                Log( "*** Actual: {0}", DiffuseMaterial.AmbientColorProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( diffuseMaterial.Brush, null ) || failOnPurpose )
            {
                AddFailure( "The default value for DiffuseMaterial.Brush should be null" );
                Log( "*** Actual: {0}", diffuseMaterial.Brush );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( DiffuseMaterial.BrushProperty.DefaultMetadata.DefaultValue, null ) || failOnPurpose )
            {
                AddFailure( "The default value for DiffuseMaterial.BrushProperty should be null" );
                Log( "*** Actual: {0}", DiffuseMaterial.BrushProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( diffuseMaterial.Color, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for DiffuseMaterial.Color should be Colors.White" );
                Log( "*** Actual: {0}", diffuseMaterial.Color );
            }
            if ( MathEx.NotEquals( (Color)DiffuseMaterial.ColorProperty.DefaultMetadata.DefaultValue, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for DiffuseMaterial.ColorProperty should be Colors.White" );
                Log( "*** Actual: {0}", DiffuseMaterial.ColorProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestDirectionalLight()
        {
            DirectionalLight directionalLight = new DirectionalLight();

            if ( MathEx.NotEquals( directionalLight.Color, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for DirectionalLight.Color should be Colors.White" );
                Log( "*** Actual: {0}", directionalLight.Color );
            }
            if ( MathEx.NotEquals( (Color)DirectionalLight.ColorProperty.DefaultMetadata.DefaultValue, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for DirectionalLight.ColorProperty should be Colors.White" );
                Log( "*** Actual: {0}", DirectionalLight.ColorProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( directionalLight.Direction, -Const.zAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for DirectionalLight.Direction should be -Const.zAxis" );
                Log( "*** Actual: {0}", directionalLight.Direction );
            }
            if ( MathEx.NotEquals( (Vector3D)DirectionalLight.DirectionProperty.DefaultMetadata.DefaultValue, -Const.zAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for DirectionalLight.DirectionProperty should be -Const.zAxis" );
                Log( "*** Actual: {0}", DirectionalLight.DirectionProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( directionalLight.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for DirectionalLight.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", directionalLight.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( DirectionalLight.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for DirectionalLight.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", DirectionalLight.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestEmissiveMaterial()
        {
            EmissiveMaterial emissiveMaterial = new EmissiveMaterial();

            if ( !ObjectUtils.DeepEqualsToAnimatable( emissiveMaterial.Brush, null ) || failOnPurpose )
            {
                AddFailure( "The default value for EmissiveMaterial.Brush should be null" );
                Log( "*** Actual: {0}", emissiveMaterial.Brush );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( EmissiveMaterial.BrushProperty.DefaultMetadata.DefaultValue, null ) || failOnPurpose )
            {
                AddFailure( "The default value for EmissiveMaterial.BrushProperty should be null" );
                Log( "*** Actual: {0}", EmissiveMaterial.BrushProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( emissiveMaterial.Color, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for EmissiveMaterial.Color should be Colors.White" );
                Log( "*** Actual: {0}", emissiveMaterial.Color );
            }
            if ( MathEx.NotEquals( (Color)EmissiveMaterial.ColorProperty.DefaultMetadata.DefaultValue, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for EmissiveMaterial.ColorProperty should be Colors.White" );
                Log( "*** Actual: {0}", EmissiveMaterial.ColorProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestGeometryModel3D()
        {
            GeometryModel3D geometryModel3D = new GeometryModel3D();

            if ( !ObjectUtils.DeepEqualsToAnimatable( geometryModel3D.BackMaterial, null ) || failOnPurpose )
            {
                AddFailure( "The default value for GeometryModel3D.BackMaterial should be null" );
                Log( "*** Actual: {0}", geometryModel3D.BackMaterial );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( GeometryModel3D.BackMaterialProperty.DefaultMetadata.DefaultValue, null ) || failOnPurpose )
            {
                AddFailure( "The default value for GeometryModel3D.BackMaterialProperty should be null" );
                Log( "*** Actual: {0}", GeometryModel3D.BackMaterialProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( geometryModel3D.Geometry, null ) || failOnPurpose )
            {
                AddFailure( "The default value for GeometryModel3D.Geometry should be null" );
                Log( "*** Actual: {0}", geometryModel3D.Geometry );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( GeometryModel3D.GeometryProperty.DefaultMetadata.DefaultValue, null ) || failOnPurpose )
            {
                AddFailure( "The default value for GeometryModel3D.GeometryProperty should be null" );
                Log( "*** Actual: {0}", GeometryModel3D.GeometryProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( geometryModel3D.Material, null ) || failOnPurpose )
            {
                AddFailure( "The default value for GeometryModel3D.Material should be null" );
                Log( "*** Actual: {0}", geometryModel3D.Material );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( GeometryModel3D.MaterialProperty.DefaultMetadata.DefaultValue, null ) || failOnPurpose )
            {
                AddFailure( "The default value for GeometryModel3D.MaterialProperty should be null" );
                Log( "*** Actual: {0}", GeometryModel3D.MaterialProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( geometryModel3D.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for GeometryModel3D.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", geometryModel3D.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( GeometryModel3D.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for GeometryModel3D.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", GeometryModel3D.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestMaterialGroup()
        {
            MaterialGroup materialGroup = new MaterialGroup();

            if ( !ObjectUtils.DeepEqualsToAnimatable( materialGroup.Children, new MaterialCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MaterialGroup.Children should be new MaterialCollection()" );
                Log( "*** Actual: {0}", materialGroup.Children );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( MaterialGroup.ChildrenProperty.DefaultMetadata.DefaultValue, new MaterialCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MaterialGroup.ChildrenProperty should be new MaterialCollection()" );
                Log( "*** Actual: {0}", MaterialGroup.ChildrenProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestMatrixCamera()
        {
            MatrixCamera matrixCamera = new MatrixCamera();

            if ( MathEx.NotEquals( matrixCamera.ProjectionMatrix, Matrix3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for MatrixCamera.ProjectionMatrix should be Matrix3D.Identity" );
                Log( "*** Actual: {0}", matrixCamera.ProjectionMatrix );
            }
            if ( MathEx.NotEquals( (Matrix3D)MatrixCamera.ProjectionMatrixProperty.DefaultMetadata.DefaultValue, Matrix3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for MatrixCamera.ProjectionMatrixProperty should be Matrix3D.Identity" );
                Log( "*** Actual: {0}", MatrixCamera.ProjectionMatrixProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( matrixCamera.ViewMatrix, Matrix3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for MatrixCamera.ViewMatrix should be Matrix3D.Identity" );
                Log( "*** Actual: {0}", matrixCamera.ViewMatrix );
            }
            if ( MathEx.NotEquals( (Matrix3D)MatrixCamera.ViewMatrixProperty.DefaultMetadata.DefaultValue, Matrix3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for MatrixCamera.ViewMatrixProperty should be Matrix3D.Identity" );
                Log( "*** Actual: {0}", MatrixCamera.ViewMatrixProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( matrixCamera.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for MatrixCamera.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", matrixCamera.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( MatrixCamera.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for MatrixCamera.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", MatrixCamera.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestMatrixTransform3D()
        {
            MatrixTransform3D matrixTransform3D = new MatrixTransform3D();

            if ( MathEx.NotEquals( matrixTransform3D.Matrix, Matrix3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for MatrixTransform3D.Matrix should be Matrix3D.Identity" );
                Log( "*** Actual: {0}", matrixTransform3D.Matrix );
            }
            if ( MathEx.NotEquals( (Matrix3D)MatrixTransform3D.MatrixProperty.DefaultMetadata.DefaultValue, Matrix3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for MatrixTransform3D.MatrixProperty should be Matrix3D.Identity" );
                Log( "*** Actual: {0}", MatrixTransform3D.MatrixProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestMeshGeometry3D()
        {
            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

            if ( !ObjectUtils.DeepEqualsToAnimatable( meshGeometry3D.Normals, new Vector3DCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MeshGeometry3D.Normals should be new Vector3DCollection()" );
                Log( "*** Actual: {0}", meshGeometry3D.Normals );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( MeshGeometry3D.NormalsProperty.DefaultMetadata.DefaultValue, new Vector3DCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MeshGeometry3D.NormalsProperty should be new Vector3DCollection()" );
                Log( "*** Actual: {0}", MeshGeometry3D.NormalsProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( meshGeometry3D.Positions, new Point3DCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MeshGeometry3D.Positions should be new Point3DCollection()" );
                Log( "*** Actual: {0}", meshGeometry3D.Positions );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( MeshGeometry3D.PositionsProperty.DefaultMetadata.DefaultValue, new Point3DCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MeshGeometry3D.PositionsProperty should be new Point3DCollection()" );
                Log( "*** Actual: {0}", MeshGeometry3D.PositionsProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( meshGeometry3D.TextureCoordinates, new PointCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MeshGeometry3D.TextureCoordinates should be new PointCollection()" );
                Log( "*** Actual: {0}", meshGeometry3D.TextureCoordinates );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( MeshGeometry3D.TextureCoordinatesProperty.DefaultMetadata.DefaultValue, new PointCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MeshGeometry3D.TextureCoordinatesProperty should be new PointCollection()" );
                Log( "*** Actual: {0}", MeshGeometry3D.TextureCoordinatesProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( meshGeometry3D.TriangleIndices, new Int32Collection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MeshGeometry3D.TriangleIndices should be new Int32Collection()" );
                Log( "*** Actual: {0}", meshGeometry3D.TriangleIndices );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( MeshGeometry3D.TriangleIndicesProperty.DefaultMetadata.DefaultValue, new Int32Collection() ) || failOnPurpose )
            {
                AddFailure( "The default value for MeshGeometry3D.TriangleIndicesProperty should be new Int32Collection()" );
                Log( "*** Actual: {0}", MeshGeometry3D.TriangleIndicesProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestModel3DGroup()
        {
            Model3DGroup model3DGroup = new Model3DGroup();

            if ( !ObjectUtils.DeepEqualsToAnimatable( model3DGroup.Children, new Model3DCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for Model3DGroup.Children should be new Model3DCollection()" );
                Log( "*** Actual: {0}", model3DGroup.Children );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( Model3DGroup.ChildrenProperty.DefaultMetadata.DefaultValue, new Model3DCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for Model3DGroup.ChildrenProperty should be new Model3DCollection()" );
                Log( "*** Actual: {0}", Model3DGroup.ChildrenProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( model3DGroup.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for Model3DGroup.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", model3DGroup.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( Model3DGroup.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for Model3DGroup.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", Model3DGroup.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestModelVisual3D()
        {
            ModelVisual3D modelVisual3D = new ModelVisual3D();

            if ( !ObjectUtils.DeepEqualsToAnimatable( modelVisual3D.Content, null ) || failOnPurpose )
            {
                AddFailure( "The default value for ModelVisual3D.Content should be null" );
                Log( "*** Actual: {0}", modelVisual3D.Content );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( ModelVisual3D.ContentProperty.DefaultMetadata.DefaultValue, null ) || failOnPurpose )
            {
                AddFailure( "The default value for ModelVisual3D.ContentProperty should be null" );
                Log( "*** Actual: {0}", ModelVisual3D.ContentProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( modelVisual3D.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for ModelVisual3D.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", modelVisual3D.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( ModelVisual3D.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for ModelVisual3D.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", ModelVisual3D.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestOrthographicCamera()
        {
            OrthographicCamera orthographicCamera = new OrthographicCamera();

            if ( MathEx.NotEquals( orthographicCamera.FarPlaneDistance, double.PositiveInfinity ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.FarPlaneDistance should be double.PositiveInfinity" );
                Log( "*** Actual: {0}", orthographicCamera.FarPlaneDistance );
            }
            if ( MathEx.NotEquals( (double)OrthographicCamera.FarPlaneDistanceProperty.DefaultMetadata.DefaultValue, double.PositiveInfinity ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.FarPlaneDistanceProperty should be double.PositiveInfinity" );
                Log( "*** Actual: {0}", OrthographicCamera.FarPlaneDistanceProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( orthographicCamera.LookDirection, -Const.zAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.LookDirection should be -Const.zAxis" );
                Log( "*** Actual: {0}", orthographicCamera.LookDirection );
            }
            if ( MathEx.NotEquals( (Vector3D)OrthographicCamera.LookDirectionProperty.DefaultMetadata.DefaultValue, -Const.zAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.LookDirectionProperty should be -Const.zAxis" );
                Log( "*** Actual: {0}", OrthographicCamera.LookDirectionProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( orthographicCamera.NearPlaneDistance, 0.125 ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.NearPlaneDistance should be 0.125" );
                Log( "*** Actual: {0}", orthographicCamera.NearPlaneDistance );
            }
            if ( MathEx.NotEquals( (double)OrthographicCamera.NearPlaneDistanceProperty.DefaultMetadata.DefaultValue, 0.125 ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.NearPlaneDistanceProperty should be 0.125" );
                Log( "*** Actual: {0}", OrthographicCamera.NearPlaneDistanceProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( orthographicCamera.Position, Const.p0 ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.Position should be Const.p0" );
                Log( "*** Actual: {0}", orthographicCamera.Position );
            }
            if ( MathEx.NotEquals( (Point3D)OrthographicCamera.PositionProperty.DefaultMetadata.DefaultValue, Const.p0 ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.PositionProperty should be Const.p0" );
                Log( "*** Actual: {0}", OrthographicCamera.PositionProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( orthographicCamera.UpDirection, Const.yAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.UpDirection should be Const.yAxis" );
                Log( "*** Actual: {0}", orthographicCamera.UpDirection );
            }
            if ( MathEx.NotEquals( (Vector3D)OrthographicCamera.UpDirectionProperty.DefaultMetadata.DefaultValue, Const.yAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.UpDirectionProperty should be Const.yAxis" );
                Log( "*** Actual: {0}", OrthographicCamera.UpDirectionProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( orthographicCamera.Width, 2 ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.Width should be 2" );
                Log( "*** Actual: {0}", orthographicCamera.Width );
            }
            if ( MathEx.NotEquals( (double)OrthographicCamera.WidthProperty.DefaultMetadata.DefaultValue, 2 ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.WidthProperty should be 2" );
                Log( "*** Actual: {0}", OrthographicCamera.WidthProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( orthographicCamera.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", orthographicCamera.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( OrthographicCamera.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for OrthographicCamera.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", OrthographicCamera.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestPerspectiveCamera()
        {
            PerspectiveCamera perspectiveCamera = new PerspectiveCamera();

            if ( MathEx.NotEquals( perspectiveCamera.FarPlaneDistance, double.PositiveInfinity ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.FarPlaneDistance should be double.PositiveInfinity" );
                Log( "*** Actual: {0}", perspectiveCamera.FarPlaneDistance );
            }
            if ( MathEx.NotEquals( (double)PerspectiveCamera.FarPlaneDistanceProperty.DefaultMetadata.DefaultValue, double.PositiveInfinity ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.FarPlaneDistanceProperty should be double.PositiveInfinity" );
                Log( "*** Actual: {0}", PerspectiveCamera.FarPlaneDistanceProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( perspectiveCamera.LookDirection, -Const.zAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.LookDirection should be -Const.zAxis" );
                Log( "*** Actual: {0}", perspectiveCamera.LookDirection );
            }
            if ( MathEx.NotEquals( (Vector3D)PerspectiveCamera.LookDirectionProperty.DefaultMetadata.DefaultValue, -Const.zAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.LookDirectionProperty should be -Const.zAxis" );
                Log( "*** Actual: {0}", PerspectiveCamera.LookDirectionProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( perspectiveCamera.NearPlaneDistance, 0.125 ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.NearPlaneDistance should be 0.125" );
                Log( "*** Actual: {0}", perspectiveCamera.NearPlaneDistance );
            }
            if ( MathEx.NotEquals( (double)PerspectiveCamera.NearPlaneDistanceProperty.DefaultMetadata.DefaultValue, 0.125 ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.NearPlaneDistanceProperty should be 0.125" );
                Log( "*** Actual: {0}", PerspectiveCamera.NearPlaneDistanceProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( perspectiveCamera.Position, Const.p0 ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.Position should be Const.p0" );
                Log( "*** Actual: {0}", perspectiveCamera.Position );
            }
            if ( MathEx.NotEquals( (Point3D)PerspectiveCamera.PositionProperty.DefaultMetadata.DefaultValue, Const.p0 ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.PositionProperty should be Const.p0" );
                Log( "*** Actual: {0}", PerspectiveCamera.PositionProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( perspectiveCamera.UpDirection, Const.yAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.UpDirection should be Const.yAxis" );
                Log( "*** Actual: {0}", perspectiveCamera.UpDirection );
            }
            if ( MathEx.NotEquals( (Vector3D)PerspectiveCamera.UpDirectionProperty.DefaultMetadata.DefaultValue, Const.yAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.UpDirectionProperty should be Const.yAxis" );
                Log( "*** Actual: {0}", PerspectiveCamera.UpDirectionProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( perspectiveCamera.FieldOfView, 45 ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.FieldOfView should be 45" );
                Log( "*** Actual: {0}", perspectiveCamera.FieldOfView );
            }
            if ( MathEx.NotEquals( (double)PerspectiveCamera.FieldOfViewProperty.DefaultMetadata.DefaultValue, 45 ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.FieldOfViewProperty should be 45" );
                Log( "*** Actual: {0}", PerspectiveCamera.FieldOfViewProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( perspectiveCamera.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", perspectiveCamera.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( PerspectiveCamera.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for PerspectiveCamera.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", PerspectiveCamera.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestPointLight()
        {
            PointLight pointLight = new PointLight();

            if ( MathEx.NotEquals( pointLight.Position, Const.p0 ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.Position should be Const.p0" );
                Log( "*** Actual: {0}", pointLight.Position );
            }
            if ( MathEx.NotEquals( (Point3D)PointLight.PositionProperty.DefaultMetadata.DefaultValue, Const.p0 ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.PositionProperty should be Const.p0" );
                Log( "*** Actual: {0}", PointLight.PositionProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( pointLight.Range, double.PositiveInfinity ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.Range should be double.PositiveInfinity" );
                Log( "*** Actual: {0}", pointLight.Range );
            }
            if ( MathEx.NotEquals( (double)PointLight.RangeProperty.DefaultMetadata.DefaultValue, double.PositiveInfinity ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.RangeProperty should be double.PositiveInfinity" );
                Log( "*** Actual: {0}", PointLight.RangeProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( pointLight.ConstantAttenuation, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.ConstantAttenuation should be 1" );
                Log( "*** Actual: {0}", pointLight.ConstantAttenuation );
            }
            if ( MathEx.NotEquals( (double)PointLight.ConstantAttenuationProperty.DefaultMetadata.DefaultValue, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.ConstantAttenuationProperty should be 1" );
                Log( "*** Actual: {0}", PointLight.ConstantAttenuationProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( pointLight.LinearAttenuation, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.LinearAttenuation should be 0" );
                Log( "*** Actual: {0}", pointLight.LinearAttenuation );
            }
            if ( MathEx.NotEquals( (double)PointLight.LinearAttenuationProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.LinearAttenuationProperty should be 0" );
                Log( "*** Actual: {0}", PointLight.LinearAttenuationProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( pointLight.QuadraticAttenuation, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.QuadraticAttenuation should be 0" );
                Log( "*** Actual: {0}", pointLight.QuadraticAttenuation );
            }
            if ( MathEx.NotEquals( (double)PointLight.QuadraticAttenuationProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.QuadraticAttenuationProperty should be 0" );
                Log( "*** Actual: {0}", PointLight.QuadraticAttenuationProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( pointLight.Color, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.Color should be Colors.White" );
                Log( "*** Actual: {0}", pointLight.Color );
            }
            if ( MathEx.NotEquals( (Color)PointLight.ColorProperty.DefaultMetadata.DefaultValue, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.ColorProperty should be Colors.White" );
                Log( "*** Actual: {0}", PointLight.ColorProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( pointLight.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", pointLight.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( PointLight.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for PointLight.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", PointLight.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestQuaternionRotation3D()
        {
            QuaternionRotation3D quaternionRotation3D = new QuaternionRotation3D();

            if ( MathEx.NotEquals( quaternionRotation3D.Quaternion, Quaternion.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for QuaternionRotation3D.Quaternion should be Quaternion.Identity" );
                Log( "*** Actual: {0}", quaternionRotation3D.Quaternion );
            }
            if ( MathEx.NotEquals( (Quaternion)QuaternionRotation3D.QuaternionProperty.DefaultMetadata.DefaultValue, Quaternion.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for QuaternionRotation3D.QuaternionProperty should be Quaternion.Identity" );
                Log( "*** Actual: {0}", QuaternionRotation3D.QuaternionProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestRotateTransform3D()
        {
            RotateTransform3D rotateTransform3D = new RotateTransform3D();

            if ( !ObjectUtils.DeepEqualsToAnimatable( rotateTransform3D.Rotation, Rotation3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for RotateTransform3D.Rotation should be Rotation3D.Identity" );
                Log( "*** Actual: {0}", rotateTransform3D.Rotation );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( RotateTransform3D.RotationProperty.DefaultMetadata.DefaultValue, Rotation3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for RotateTransform3D.RotationProperty should be Rotation3D.Identity" );
                Log( "*** Actual: {0}", RotateTransform3D.RotationProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( rotateTransform3D.CenterX, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for RotateTransform3D.CenterX should be 0" );
                Log( "*** Actual: {0}", rotateTransform3D.CenterX );
            }
            if ( MathEx.NotEquals( (double)RotateTransform3D.CenterXProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for RotateTransform3D.CenterXProperty should be 0" );
                Log( "*** Actual: {0}", RotateTransform3D.CenterXProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( rotateTransform3D.CenterY, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for RotateTransform3D.CenterY should be 0" );
                Log( "*** Actual: {0}", rotateTransform3D.CenterY );
            }
            if ( MathEx.NotEquals( (double)RotateTransform3D.CenterYProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for RotateTransform3D.CenterYProperty should be 0" );
                Log( "*** Actual: {0}", RotateTransform3D.CenterYProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( rotateTransform3D.CenterZ, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for RotateTransform3D.CenterZ should be 0" );
                Log( "*** Actual: {0}", rotateTransform3D.CenterZ );
            }
            if ( MathEx.NotEquals( (double)RotateTransform3D.CenterZProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for RotateTransform3D.CenterZProperty should be 0" );
                Log( "*** Actual: {0}", RotateTransform3D.CenterZProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestScaleTransform3D()
        {
            ScaleTransform3D scaleTransform3D = new ScaleTransform3D();

            if ( MathEx.NotEquals( scaleTransform3D.ScaleX, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.ScaleX should be 1" );
                Log( "*** Actual: {0}", scaleTransform3D.ScaleX );
            }
            if ( MathEx.NotEquals( (double)ScaleTransform3D.ScaleXProperty.DefaultMetadata.DefaultValue, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.ScaleXProperty should be 1" );
                Log( "*** Actual: {0}", ScaleTransform3D.ScaleXProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( scaleTransform3D.ScaleY, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.ScaleY should be 1" );
                Log( "*** Actual: {0}", scaleTransform3D.ScaleY );
            }
            if ( MathEx.NotEquals( (double)ScaleTransform3D.ScaleYProperty.DefaultMetadata.DefaultValue, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.ScaleYProperty should be 1" );
                Log( "*** Actual: {0}", ScaleTransform3D.ScaleYProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( scaleTransform3D.ScaleZ, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.ScaleZ should be 1" );
                Log( "*** Actual: {0}", scaleTransform3D.ScaleZ );
            }
            if ( MathEx.NotEquals( (double)ScaleTransform3D.ScaleZProperty.DefaultMetadata.DefaultValue, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.ScaleZProperty should be 1" );
                Log( "*** Actual: {0}", ScaleTransform3D.ScaleZProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( scaleTransform3D.CenterX, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.CenterX should be 0" );
                Log( "*** Actual: {0}", scaleTransform3D.CenterX );
            }
            if ( MathEx.NotEquals( (double)ScaleTransform3D.CenterXProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.CenterXProperty should be 0" );
                Log( "*** Actual: {0}", ScaleTransform3D.CenterXProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( scaleTransform3D.CenterY, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.CenterY should be 0" );
                Log( "*** Actual: {0}", scaleTransform3D.CenterY );
            }
            if ( MathEx.NotEquals( (double)ScaleTransform3D.CenterYProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.CenterYProperty should be 0" );
                Log( "*** Actual: {0}", ScaleTransform3D.CenterYProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( scaleTransform3D.CenterZ, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.CenterZ should be 0" );
                Log( "*** Actual: {0}", scaleTransform3D.CenterZ );
            }
            if ( MathEx.NotEquals( (double)ScaleTransform3D.CenterZProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for ScaleTransform3D.CenterZProperty should be 0" );
                Log( "*** Actual: {0}", ScaleTransform3D.CenterZProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestSpecularMaterial()
        {
            SpecularMaterial specularMaterial = new SpecularMaterial();

            if ( !ObjectUtils.DeepEqualsToAnimatable( specularMaterial.Brush, null ) || failOnPurpose )
            {
                AddFailure( "The default value for SpecularMaterial.Brush should be null" );
                Log( "*** Actual: {0}", specularMaterial.Brush );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( SpecularMaterial.BrushProperty.DefaultMetadata.DefaultValue, null ) || failOnPurpose )
            {
                AddFailure( "The default value for SpecularMaterial.BrushProperty should be null" );
                Log( "*** Actual: {0}", SpecularMaterial.BrushProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( specularMaterial.Color, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for SpecularMaterial.Color should be Colors.White" );
                Log( "*** Actual: {0}", specularMaterial.Color );
            }
            if ( MathEx.NotEquals( (Color)SpecularMaterial.ColorProperty.DefaultMetadata.DefaultValue, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for SpecularMaterial.ColorProperty should be Colors.White" );
                Log( "*** Actual: {0}", SpecularMaterial.ColorProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( specularMaterial.SpecularPower, 40 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpecularMaterial.SpecularPower should be 40" );
                Log( "*** Actual: {0}", specularMaterial.SpecularPower );
            }
            if ( MathEx.NotEquals( (double)SpecularMaterial.SpecularPowerProperty.DefaultMetadata.DefaultValue, 40 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpecularMaterial.SpecularPowerProperty should be 40" );
                Log( "*** Actual: {0}", SpecularMaterial.SpecularPowerProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestSpotLight()
        {
            SpotLight spotLight = new SpotLight();

            if ( MathEx.NotEquals( spotLight.Direction, -Const.zAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.Direction should be -Const.zAxis" );
                Log( "*** Actual: {0}", spotLight.Direction );
            }
            if ( MathEx.NotEquals( (Vector3D)SpotLight.DirectionProperty.DefaultMetadata.DefaultValue, -Const.zAxis ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.DirectionProperty should be -Const.zAxis" );
                Log( "*** Actual: {0}", SpotLight.DirectionProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( spotLight.InnerConeAngle, 180 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.InnerConeAngle should be 180" );
                Log( "*** Actual: {0}", spotLight.InnerConeAngle );
            }
            if ( MathEx.NotEquals( (double)SpotLight.InnerConeAngleProperty.DefaultMetadata.DefaultValue, 180 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.InnerConeAngleProperty should be 180" );
                Log( "*** Actual: {0}", SpotLight.InnerConeAngleProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( spotLight.OuterConeAngle, 90 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.OuterConeAngle should be 90" );
                Log( "*** Actual: {0}", spotLight.OuterConeAngle );
            }
            if ( MathEx.NotEquals( (double)SpotLight.OuterConeAngleProperty.DefaultMetadata.DefaultValue, 90 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.OuterConeAngleProperty should be 90" );
                Log( "*** Actual: {0}", SpotLight.OuterConeAngleProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( spotLight.Position, Const.p0 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.Position should be Const.p0" );
                Log( "*** Actual: {0}", spotLight.Position );
            }
            if ( MathEx.NotEquals( (Point3D)SpotLight.PositionProperty.DefaultMetadata.DefaultValue, Const.p0 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.PositionProperty should be Const.p0" );
                Log( "*** Actual: {0}", SpotLight.PositionProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( spotLight.Range, double.PositiveInfinity ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.Range should be double.PositiveInfinity" );
                Log( "*** Actual: {0}", spotLight.Range );
            }
            if ( MathEx.NotEquals( (double)SpotLight.RangeProperty.DefaultMetadata.DefaultValue, double.PositiveInfinity ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.RangeProperty should be double.PositiveInfinity" );
                Log( "*** Actual: {0}", SpotLight.RangeProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( spotLight.ConstantAttenuation, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.ConstantAttenuation should be 1" );
                Log( "*** Actual: {0}", spotLight.ConstantAttenuation );
            }
            if ( MathEx.NotEquals( (double)SpotLight.ConstantAttenuationProperty.DefaultMetadata.DefaultValue, 1 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.ConstantAttenuationProperty should be 1" );
                Log( "*** Actual: {0}", SpotLight.ConstantAttenuationProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( spotLight.LinearAttenuation, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.LinearAttenuation should be 0" );
                Log( "*** Actual: {0}", spotLight.LinearAttenuation );
            }
            if ( MathEx.NotEquals( (double)SpotLight.LinearAttenuationProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.LinearAttenuationProperty should be 0" );
                Log( "*** Actual: {0}", SpotLight.LinearAttenuationProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( spotLight.QuadraticAttenuation, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.QuadraticAttenuation should be 0" );
                Log( "*** Actual: {0}", spotLight.QuadraticAttenuation );
            }
            if ( MathEx.NotEquals( (double)SpotLight.QuadraticAttenuationProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.QuadraticAttenuationProperty should be 0" );
                Log( "*** Actual: {0}", SpotLight.QuadraticAttenuationProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( spotLight.Color, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.Color should be Colors.White" );
                Log( "*** Actual: {0}", spotLight.Color );
            }
            if ( MathEx.NotEquals( (Color)SpotLight.ColorProperty.DefaultMetadata.DefaultValue, Colors.White ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.ColorProperty should be Colors.White" );
                Log( "*** Actual: {0}", SpotLight.ColorProperty.DefaultMetadata.DefaultValue );
            }

            if ( !ObjectUtils.DeepEqualsToAnimatable( spotLight.Transform, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.Transform should be Transform3D.Identity" );
                Log( "*** Actual: {0}", spotLight.Transform );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( SpotLight.TransformProperty.DefaultMetadata.DefaultValue, Transform3D.Identity ) || failOnPurpose )
            {
                AddFailure( "The default value for SpotLight.TransformProperty should be Transform3D.Identity" );
                Log( "*** Actual: {0}", SpotLight.TransformProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestTransform3DGroup()
        {
            Transform3DGroup transform3DGroup = new Transform3DGroup();

            if ( !ObjectUtils.DeepEqualsToAnimatable( transform3DGroup.Children, new Transform3DCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for Transform3DGroup.Children should be new Transform3DCollection()" );
                Log( "*** Actual: {0}", transform3DGroup.Children );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( Transform3DGroup.ChildrenProperty.DefaultMetadata.DefaultValue, new Transform3DCollection() ) || failOnPurpose )
            {
                AddFailure( "The default value for Transform3DGroup.ChildrenProperty should be new Transform3DCollection()" );
                Log( "*** Actual: {0}", Transform3DGroup.ChildrenProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestTranslateTransform3D()
        {
            TranslateTransform3D translateTransform3D = new TranslateTransform3D();

            if ( MathEx.NotEquals( translateTransform3D.OffsetX, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for TranslateTransform3D.OffsetX should be 0" );
                Log( "*** Actual: {0}", translateTransform3D.OffsetX );
            }
            if ( MathEx.NotEquals( (double)TranslateTransform3D.OffsetXProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for TranslateTransform3D.OffsetXProperty should be 0" );
                Log( "*** Actual: {0}", TranslateTransform3D.OffsetXProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( translateTransform3D.OffsetY, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for TranslateTransform3D.OffsetY should be 0" );
                Log( "*** Actual: {0}", translateTransform3D.OffsetY );
            }
            if ( MathEx.NotEquals( (double)TranslateTransform3D.OffsetYProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for TranslateTransform3D.OffsetYProperty should be 0" );
                Log( "*** Actual: {0}", TranslateTransform3D.OffsetYProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( translateTransform3D.OffsetZ, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for TranslateTransform3D.OffsetZ should be 0" );
                Log( "*** Actual: {0}", translateTransform3D.OffsetZ );
            }
            if ( MathEx.NotEquals( (double)TranslateTransform3D.OffsetZProperty.DefaultMetadata.DefaultValue, 0 ) || failOnPurpose )
            {
                AddFailure( "The default value for TranslateTransform3D.OffsetZProperty should be 0" );
                Log( "*** Actual: {0}", TranslateTransform3D.OffsetZProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestViewport3DVisual()
        {
            Viewport3DVisual viewport3DVisual = new Viewport3DVisual();

            if ( !ObjectUtils.DeepEqualsToAnimatable( viewport3DVisual.Camera, new PerspectiveCamera() ) || failOnPurpose )
            {
                AddFailure( "The default value for Viewport3DVisual.Camera should be new PerspectiveCamera()" );
                Log( "*** Actual: {0}", viewport3DVisual.Camera );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( Viewport3DVisual.CameraProperty.DefaultMetadata.DefaultValue, new PerspectiveCamera() ) || failOnPurpose )
            {
                AddFailure( "The default value for Viewport3DVisual.CameraProperty should be new PerspectiveCamera()" );
                Log( "*** Actual: {0}", Viewport3DVisual.CameraProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( viewport3DVisual.Viewport, Rect.Empty ) || failOnPurpose )
            {
                AddFailure( "The default value for Viewport3DVisual.Viewport should be Rect.Empty" );
                Log( "*** Actual: {0}", viewport3DVisual.Viewport );
            }
            if ( MathEx.NotEquals( (Rect)Viewport3DVisual.ViewportProperty.DefaultMetadata.DefaultValue, Rect.Empty ) || failOnPurpose )
            {
                AddFailure( "The default value for Viewport3DVisual.ViewportProperty should be Rect.Empty" );
                Log( "*** Actual: {0}", Viewport3DVisual.ViewportProperty.DefaultMetadata.DefaultValue );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestViewport3D()
        {
            Viewport3D viewport3D = new Viewport3D();

            if ( !ObjectUtils.DeepEqualsToAnimatable( viewport3D.Camera, new PerspectiveCamera() ) || failOnPurpose )
            {
                AddFailure( "The default value for Viewport3D.Camera should be new PerspectiveCamera()" );
                Log( "*** Actual: {0}", viewport3D.Camera );
            }
            if ( !ObjectUtils.DeepEqualsToAnimatable( Viewport3D.CameraProperty.DefaultMetadata.DefaultValue, new PerspectiveCamera() ) || failOnPurpose )
            {
                AddFailure( "The default value for Viewport3D.CameraProperty should be new PerspectiveCamera()" );
                Log( "*** Actual: {0}", Viewport3D.CameraProperty.DefaultMetadata.DefaultValue );
            }

            if ( MathEx.NotEquals( viewport3D.ClipToBounds, true ) || failOnPurpose )
            {
                AddFailure( "The default value for Viewport3D.ClipToBounds should be true" );
                Log( "*** Actual: {0}", viewport3D.ClipToBounds );
            }
            if ( false /* Viewport3D.ClipToBounds overrides the default so this comparison would be wrong */ || failOnPurpose )
            {
                AddFailure( "The default value for Viewport3D.ClipToBoundsProperty should be true" );
                Log( "*** Actual: {0}", Viewport3D.ClipToBoundsProperty.DefaultMetadata.DefaultValue );
            }
        }

    }
}
