// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Win32;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public partial class Lights : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nParameters:" +
                    "\n   /LightCount=           (Int)  Number of lights. (default=5)" +
                    "\n   /TranslateLights       (Flag) Set to add a translate transform to the lights." +
                    "\n   /RotateLights          (Flag) Set to add a rotate transform to the lights." +
                    "\n   /AnimateTransforms     (Flag) Set to animate all transforms." +
                    "\n   /SpecularPower=        (Double) If other than 0.0 ( default ) then SpecularMaterial is used with that power." +
                    "\n   /AddEmissive           (Flag) Set to add a dim EmissiveMaterial layer." +
                    "\n   /UseSpotLight          (Flag) Set to use SpotLights instead of PointLights." +
                    "\n   /UseDirectionalLight   (Flag) Set to use DirectionalLight instead of PointLight." +
                    "\n   /AddAmbientLight       (Flag) Set to add an AmbientLight." +
                    "\n   /AmbientColor=         (Color) Color Knob for Ambient." +
                    "\n   /DiffuseColor=         (Color) Color Knob for Diffuse." +
                    "\n   /SpecularColor=        (Color) Color Knob for Specular." +
                    "\n   /EmissiveColor=        (Color) Color Knob for Emissive." +
                    "";
            }
        }

        public void OnLoaded(object sender, EventArgs args)
        {
            ParseParameters();
            BuildScene();
        }

        int _lightCount = 5;
        bool _translateLights = false;
        bool _rotateLights = false;
        bool _animateTransforms = false;
        bool _addEmissive = false;
        bool _useSpotLight = false;
        bool _useDirectionalLight = false;
        bool _addAmbientLight = false;
        double _specularPower = 0.0;

        Color _ambientColor = Colors.White;
        Color _diffuseColor = Colors.White;
        Color _specularColor = Colors.White;
        Color _emissiveColor = Colors.White;

        public void ParseParameters()
        {
            Application application = Application.Current as Application;
            if (application.Properties["LightCount"] != null)
            {
                _lightCount = StringConverter.ToInt((string)application.Properties["LightCount"]);
            }
            if (application.Properties["TranslateLights"] != null)
            {
                _translateLights = StringConverter.ToBool((string)application.Properties["TranslateLights"]);
            }
            if (application.Properties["RotateLights"] != null)
            {
                _rotateLights = StringConverter.ToBool((string)application.Properties["RotateLights"]);
            }
            if (application.Properties["AnimateTransforms"] != null)
            {
                _animateTransforms = StringConverter.ToBool((string)application.Properties["AnimateTransforms"]);
            }
            if (application.Properties["SpecularPower"] != null)
            {
                _specularPower = StringConverter.ToDouble((string)application.Properties["SpecularPower"]);
            }
            if (application.Properties["AddEmissive"] != null)
            {
                _addEmissive = StringConverter.ToBool((string)application.Properties["AddEmissive"]);
            }
            if (application.Properties["UseSpotLight"] != null)
            {
                _useSpotLight = StringConverter.ToBool((string)application.Properties["UseSpotLight"]);
            }
            if (application.Properties["UseDirectionalLight"] != null)
            {
                _useDirectionalLight = StringConverter.ToBool((string)application.Properties["UseDirectionalLight"]);
            }
            if (application.Properties["AddAmbientLight"] != null)
            {
                _addAmbientLight = StringConverter.ToBool((string)application.Properties["AddAmbientLight"]);
            }
            if (application.Properties["AmbientColor"] != null)
            {
                _ambientColor = StringConverter.ToColor((string)application.Properties["AmbientColor"]);
            }
            if (application.Properties["DiffuseColor"] != null)
            {
                _diffuseColor = StringConverter.ToColor((string)application.Properties["DiffuseColor"]);
            }
            if (application.Properties["SpecularColor"] != null)
            {
                _specularColor = StringConverter.ToColor((string)application.Properties["SpecularColor"]);
            }
            if (application.Properties["EmissiveColor"] != null)
            {
                _emissiveColor = StringConverter.ToColor((string)application.Properties["EmissiveColor"]);
            }


        }

        public void BuildScene()
        {
            MeshGeometry3D mesh = MeshFactory.Sphere(100, 100, 1.2);

            MaterialGroup mat = new MaterialGroup();
            if (_specularPower > 0.0)
            {
                SpecularMaterial specular = new SpecularMaterial(Brushes.White, _specularPower);
                specular.Color = _specularColor;
                mat.Children.Add(specular);
            }
            else
            {
                DiffuseMaterial diffuse = new DiffuseMaterial(Brushes.White);
                diffuse.Color = _diffuseColor;
                diffuse.AmbientColor = _ambientColor;
                mat.Children.Add(diffuse);
            }
            if (_addEmissive)
            {
                EmissiveMaterial emissive = new EmissiveMaterial(new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20)));
                mat.Children.Add(emissive);
            }

            GeometryModel3D primitive = new GeometryModel3D(mesh, mat);

            Model3DGroup mg = new Model3DGroup();
            Model3DGroup lights = CreateRing(1.5, _lightCount, Colors.Gray);
            SplitLightGroupRGB(lights);
            AddLightTransforms(lights);
            AddSpinTransforms(lights);

            mg.Children.Add(lights);
            if (_addAmbientLight)
            {
                mg.Children.Add(new AmbientLight()); // Default white ambient
            }

            mg.Children.Add(primitive);
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add(visual);
            VIEWPORT.Camera = CameraFactory.PerspectiveDefault;
        }

        public Model3DGroup CreateRing(double radius, int count, Color color)
        {
            double angleDelta = Math.PI * 2 / (double)count;
            Model3DGroup lights = new Model3DGroup();

            double angle = 0;
            for (int i = 0; i < count; i++, angle += angleDelta)
            {
                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);

                if (_useDirectionalLight)
                {
                    // Directional only cares about direction
                    DirectionalLight d = new DirectionalLight(color, new Point3D() - new Point3D(x, y, 0));
                    lights.Children.Add(d);
                }
                else
                {
                    // Point or Spot - common parameters 
                    PointLightBase p;
                    if (_useSpotLight)
                    {
                        p = new SpotLight(color, new Point3D(x, y, .5), new Point3D() - new Point3D(x, y, .5), 45, 30);
                    }
                    else
                    {
                        p = new PointLight(color, new Point3D(x, y, .5));
                    }
                    p.LinearAttenuation = 0;
                    p.QuadraticAttenuation = 0;
                    p.ConstantAttenuation = 1;
                    p.Range = 100;
                    lights.Children.Add(p);
                }
            }
            return lights;
        }

        public void SplitLightGroupRGB(Model3DGroup lights)
        {
            for (int i = 0; i < lights.Children.Count; i++)
            {
                if (lights.Children[i] is Light)
                {
                    Color lightColor = (lights.Children[i] as Light).Color;
                    switch (i % 3)
                    {
                        case 0:
                            lightColor = Color.FromRgb(lightColor.R, 0x00, 0x00);
                            break;

                        case 1:
                            lightColor = Color.FromRgb(0x00, lightColor.G, 0x00);
                            break;

                        case 2:
                            lightColor = Color.FromRgb(0x00, 0x00, lightColor.R);
                            break;
                    }
                    (lights.Children[i] as Light).Color = lightColor;
                }
            }
        }

        public void AddLightTransforms(Model3D model)
        {
            Transform3DGroup tc = new Transform3DGroup();
            tc.Children = new Transform3DCollection(new Transform3D[] { model.Transform });

            if (_translateLights)
            {
                TranslateTransform3D translate = new TranslateTransform3D(new Vector3D(0, 0, 0.75));
                if (_animateTransforms)
                {
                    DoubleAnimation da = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(1500)));
                    da.AutoReverse = true;
                    da.RepeatBehavior = RepeatBehavior.Forever;
                    translate.BeginAnimation(TranslateTransform3D.OffsetZProperty, da);
                }
                tc.Children.Add(translate);
            }
            if (_rotateLights)
            {
                AxisAngleRotation3D axisAngle = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 1);
                RotateTransform3D rotation = new RotateTransform3D(axisAngle, new Point3D(0, 0, 0));
                if (_animateTransforms)
                {
                    DoubleAnimation da = new DoubleAnimation(
                            0,
                            360,
                            new Duration(TimeSpan.FromMilliseconds(1500)));
                    da.AutoReverse = false;
                    da.RepeatBehavior = RepeatBehavior.Forever;
                    rotation.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
                }
                tc.Children.Add(rotation);
            }

            model.Transform = tc;
        }

        public void AddSpinTransforms(Model3D model)
        {
            Transform3DGroup tc = new Transform3DGroup();
            tc.Children = new Transform3DCollection(new Transform3D[] { model.Transform });

            if (_rotateLights)
            {
                AxisAngleRotation3D axisAngle = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 1);
                RotateTransform3D rotation = new RotateTransform3D(axisAngle, new Point3D(0, 0, 0));
                if (_animateTransforms)
                {
                    DoubleAnimation da = new DoubleAnimation(
                            0,
                            360,
                            new Duration(TimeSpan.FromMilliseconds(3700)));
                    da.AutoReverse = false;
                    da.RepeatBehavior = RepeatBehavior.Forever;
                    rotation.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
                }
                tc.Children.Add(rotation);
            }

            model.Transform = tc;
        }

    }

}
