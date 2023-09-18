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
    public partial class MaterialGroupTest : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nParameters:" +
                    "\n   /Deep=        (Int)  Number of nested MaterialGroup." +
                    "\n   /Wide=        (Int)  Number of materials per MaterialGroup." +
                    "\n   /Material=    (String) One of DiffuseMaterial, SpecularMaterial, EmissiveMaterial. (default:DiffuseMaterial)" +
                    "\n   /ColorScale   (Flag) Set to scale color in wide trees." +
                    "\n   /Color=       (Color) ARGB color value. (Default=255,63,127,175)" +
                    "";
            }
        }

        public void OnLoaded(object sender, EventArgs args)
        {
            ParseParameters();
            BuildScene();
        }

        int _deep = 1;
        int _wide = 1;
        string _material = "DiffuseMaterial";
        bool _colorScale = false;
        Color _color = Color.FromArgb(0xFF, 0x3F, 0x7F, 0xAF);

        public void ParseParameters()
        {
            Application application = Application.Current as Application;
            if (application.Properties["Deep"] != null)
            {
                _deep = StringConverter.ToInt((string)application.Properties["Deep"]);
            }
            if (application.Properties["Wide"] != null)
            {
                _wide = StringConverter.ToInt((string)application.Properties["Wide"]);
            }
            if (application.Properties["Material"] != null)
            {
                _material = application.Properties["Material"] as string;
            }
            if (application.Properties["ColorScale"] != null)
            {
                _colorScale = StringConverter.ToBool((string)application.Properties["ColorScale"]);
            }
            if (application.Properties["Color"] != null)
            {
                _color = StringConverter.ToColor((string)application.Properties["Color"]);
            }
        }

        public void BuildScene()
        {
            AmbientLight light = new AmbientLight();
            MeshGeometry3D mesh = MeshFactory.FullScreenMesh;

            Material mat = BuildMaterial(_color);
            if (_wide > 1)
            {
                Material matWide = BuildWideMaterial(_wide, _color);
                mat = matWide;
            }
            if (_deep > 1)
            {
                Material matDeep = BuildMaterialChain(mat, _deep);
                mat = matDeep;
            }
            GeometryModel3D primitive = new GeometryModel3D(mesh, mat);

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add(light);
            mg.Children.Add(primitive);
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add(visual);
            VIEWPORT.Camera = CameraFactory.PerspectiveDefault;
        }

        public Material BuildMaterial(Color color)
        {
            Brush b = new SolidColorBrush(color);
            Material materialOut = null;
            switch (_material)
            {
                case "SpecularMaterial":
                    materialOut = new SpecularMaterial(b, 10);
                    break;
                case "EmissiveMaterial":
                    materialOut = new EmissiveMaterial(b);
                    break;
                case "DiffuseMaterial":
                    materialOut = new DiffuseMaterial(b);
                    break;
                default:
                    throw new ApplicationException("Unexpected material: " + _material);
            }
            return materialOut;
        }

        public MaterialGroup BuildWideMaterial(int count, Color baseColor)
        {
            MaterialGroup group = new MaterialGroup();
            for (int i = 0; i < count; i++)
            {
                Color current = baseColor;
                if (_colorScale)
                {
                    current = ColorOperations.Blend(baseColor, Colors.Black, 1.0 / (double)count);
                }
                Material mat = BuildMaterial(current);
                group.Children.Add(mat);
            }
            return group;
        }

        public MaterialGroup BuildMaterialChain(Material material, int count)
        {
            MaterialGroup group = new MaterialGroup();

            MaterialGroup last = group;
            for (int i = 0; i < count; i++)
            {
                MaterialGroup current = new MaterialGroup();
                // add child
                current.Children.Add(material);
                // add to parent            
                last.Children.Add(current);
                // update chain
                last = current;
            }
            return group;
        }


    }
}
