// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Render a 3D scene then perform an action on
    ///the scene that should cause the scene to re-render.
    /// </summary>
    public partial class Visual3DActionTest : XamlTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            v.AssertExistenceOf("Action");

            string a = v["Action"];
            switch (a)
            {
                case "RemoveVisual":
                    // Visual="<path>"
                    //      <path> is the dot-down path from the Viewport3D to the Visual3D to be removed
                    v.AssertExistenceOf("Visual");
                    _action = new RemoveVisualAction(v["Visual"]);
                    break;

                case "ClearVisual":
                    // Visual="<path>"
                    //      <path> is the dot-down path from the Viewport3D to a leaf Visual3d of the collection to be cleared
                    v.AssertExistenceOf("Visual");
                    _action = new ClearVisualAction(v["Visual"]);
                    break;

                case "InsertVisual":
                    v.AssertExistenceOf("Name", "Location");
                    // Name="<model>"
                    //      <model> is something that ModelFactory can parse
                    // Location="<path>"
                    //      <path> is the dot-down path from the Viewport3D to the location where the Visual3D will be inserted
                    _action = new InsertVisualAction(v["Name"], v["Location"]);
                    break;

                case "MoveVisual":
                    v.AssertExistenceOf("From", "To");
                    // From="<path>"
                    //      <path> is the dot-down path from the Viewport3D to the Visual3D that will be moved
                    // To="<path>"
                    //      <path> is the dot-down path from the Viewport3D to the location where the Visual3D will be placed
                    _action = new MoveVisualAction(v["From"], v["To"]);
                    break;

                case "CopyVisual":
                    v.AssertExistenceOf("From", "To");
                    // From="<path>"
                    //      <path> is the dot-down path from the Viewport3D to the Visual3D that will be copied
                    // To="<path>"
                    //      <path> is the dot-down path from the Viewport3D to the location where the copied Visual3D will be placed
                    _action = new CopyVisualAction(v["From"], v["To"]);
                    break;

                case "SwapVisuals":
                    v.AssertExistenceOf("Visual1", "Visual2");
                    // Visual1="<path>"
                    //      <path> is the dot-down path from the Viewport3D to the first Visual3D to be swapped
                    // Visual2="<path>"
                    //      <path> is the dot-down path from the Viewport3D to the second Visual3D to be swapped
                    _action = new SwapVisualsAction(v["Visual1"], v["Visual2"]);
                    break;

                default:
                    throw new ApplicationException("Unrecognized action: " + a);
            }
        }

        /// <summary/>
        public override void RunTheTest()
        {
            // Render and verify the initial scene
            RenderWindowContent();
            VerifyWithinContext();

            // Modify the scene and verify again
            ModifyWindowContent();
            VerifyWithinContext();
        }

        /// <summary/>
        public override Visual GetWindowContent()
        {
            // "Thaw" the frozen objects because loading Xaml creates frozen objects.

            Log("Rendering baseline prior to actions...");
            Log("");
            parameters.ThawViewport();
            return parameters.Viewport;
        }

        /// <summary/>
        public override void ModifyWindowContent(Visual v)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(parameters.Viewport, v), "The stored viewport should be the one being rendered");

            Log("");
            Log("{0}", _action);
            Log("");

            _action.Perform(parameters.Viewport);
        }

        private Action _action;

        #region Actions

        /// <summary/>
        private abstract class Action
        {
            /// <summary/>
            public abstract void Perform(Viewport3D viewport);

            /// <summary/>
            public sealed override string ToString()
            {
                return TestDescription;
            }

            /// <summary/>
            public abstract string TestDescription
            {
                get;
            }

            /// <summary/>
            protected Visual3DCollection GetCollectionAndIndex(string path, Viewport3D viewport, out int index)
            {
                // path is expected to be in this format:
                //
                // "Children[0]"
                // "Children[0].Children[0]"
                // ...
                // If path does not end with an indexed Visual3DCollection, an exception will be thrown.

                int i = path.LastIndexOf(']');
                if (i != path.Length - 1)
                {
                    index = -1;
                }
                else
                {
                    while (path[i] != '[')
                    {
                        i--;
                    }

                    // "Children[0]" has index = 8 and Length = 11.  We want substring from index 9 of length 1.
                    index = StringConverter.ToInt(path.Substring(i + 1, path.Length - i - 2).Trim());
                }

                Visual3DCollection collection = ObjectUtils.GetPropertyOwner(path, viewport) as Visual3DCollection;
                if (collection == null || index < 0)
                {
                    throw new ArgumentException("Path: " + path + " does not lead to a Visual3D");
                }
                return collection;
            }
        }

        private class RemoveVisualAction : Action
        {
            public RemoveVisualAction(string visualPath)
            {
                this._visualPath = visualPath;
            }

            public override void Perform(Viewport3D viewport)
            {
                int index;
                Visual3DCollection collection = GetCollectionAndIndex(_visualPath, viewport, out index);

                collection.RemoveAt(index);
            }

            public override string TestDescription
            {
                get { return "Remove Visual3D at: " + _visualPath; }
            }

            private string _visualPath;
        }

        private class ClearVisualAction : Action
        {
            public ClearVisualAction(string visualPath)
            {
                this._visualPath = visualPath;
            }

            public override void Perform(Viewport3D viewport)
            {
                int index;
                //This test ignores the index value as it is clearing the entire collection
                Visual3DCollection collection = GetCollectionAndIndex(_visualPath, viewport, out index);
                collection.Clear();
            }

            public override string TestDescription
            {
                get { return "Clearing Visual3DCollection at level of: " + _visualPath; }
            }

            private string _visualPath;
        }

        private class InsertVisualAction : Action
        {
            public InsertVisualAction(string name, string location)
            {
                this._name = name;
                this._location = location;
            }

            public override void Perform(Viewport3D viewport)
            {
                int index;
                Visual3DCollection collection = GetCollectionAndIndex(_location, viewport, out index);

                ModelVisual3D visual = new ModelVisual3D();
                visual.Content = ModelFactory.MakeModel(_name);

                collection.Insert(index, visual);
            }

            public override string TestDescription
            {
                get { return "Insert Visual3D " + _name + " at: " + _location; }
            }

            private string _name;
            private string _location;
        }

        private class MoveVisualAction : Action
        {
            public MoveVisualAction(string from, string to)
            {
                this._from = from;
                this._to = to;
            }

            public override void Perform(Viewport3D viewport)
            {
                int fromIndex;
                int toIndex;
                Visual3DCollection fromCollection = GetCollectionAndIndex(_from, viewport, out fromIndex);
                Visual3DCollection toCollection = GetCollectionAndIndex(_to, viewport, out toIndex);

                Visual3D mover = fromCollection[fromIndex];
                fromCollection.RemoveAt(fromIndex);
                toCollection.Insert(toIndex, mover);
            }

            public override string TestDescription
            {
                get { return "Move Visual3D from: " + _from + " to: " + _to; }
            }

            private string _from;
            private string _to;
        }

        private class CopyVisualAction : Action
        {
            public CopyVisualAction(string from, string to)
            {
                this._from = from;
                this._to = to;
            }

            public override void Perform(Viewport3D viewport)
            {
                int fromIndex;
                int toIndex;
                Visual3DCollection fromCollection = GetCollectionAndIndex(_from, viewport, out fromIndex);
                Visual3DCollection toCollection = GetCollectionAndIndex(_to, viewport, out toIndex);

                Visual3D copy = Copy(fromCollection[fromIndex]);
                if (object.ReferenceEquals(fromCollection, toCollection) && copy is ModelVisual3D)
                {
                    // Nuke the top-level transform so that it doesn't render at the same place
                    ((ModelVisual3D)copy).Transform = null;
                }
                toCollection.Insert(toIndex, copy);
            }

            /// <summary/>
            protected Visual3D Copy(Visual3D visual)
            {
                if (visual is ModelVisual3D)
                {
                    ModelVisual3D mv = visual as ModelVisual3D;
                    ModelVisual3D result = new ModelVisual3D();
                    if (mv.Content != null)
                    {
                        result.Content = mv.Content.Clone();
                    }
                    if (mv.Transform != null)
                    {
                        result.Transform = mv.Transform.Clone();
                    }
                    foreach (Visual3D child in mv.Children)
                    {
                        result.Children.Add(Copy(child));
                    }
                    return result;
                }
                throw new NotSupportedException("Only ModelVisual3D is supported at this time");
            }

            public override string TestDescription
            {
                get { return "Copy Visual3D from: " + _from + " to: " + _to; }
            }

            private string _from;
            private string _to;
        }

        private class SwapVisualsAction : Action
        {
            public SwapVisualsAction(string visual1, string visual2)
            {
                this._visual1 = visual1;
                this._visual2 = visual2;
            }

            public override void Perform(Viewport3D viewport)
            {
                int index1;
                int index2;
                Visual3DCollection collection1 = GetCollectionAndIndex(_visual1, viewport, out index1);
                Visual3DCollection collection2 = GetCollectionAndIndex(_visual2, viewport, out index2);
                Visual3D v1 = collection1[index1];
                Visual3D v2 = collection2[index2];

                collection1.Remove(v1);
                collection2.Remove(v2);

                // Order matters if collection1 and collection2 are the same collection.
                //   If index1 < index2, then index2 is invalid until a Visual3D is inserted at index1.
                //   Likewise for the converse.

                // But if collection1 != collection2, it doesn't matter which order this happens.
                if (index1 < index2)
                {
                    collection1.Insert(index1, v2);
                    collection2.Insert(index2, v1);
                }
                else
                {
                    collection2.Insert(index2, v1);
                    collection1.Insert(index1, v2);
                }
            }

            public override string TestDescription
            {
                get { return "Swap Visual3Ds: " + _visual1 + " and: " + _visual2; }
            }

            private string _visual1;
            private string _visual2;
        }

        #endregion
    }
}