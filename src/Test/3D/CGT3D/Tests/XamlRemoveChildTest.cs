// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows.Media;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Render a 3D scene then remove children from a specified group.
    /// </summary>
    public class XamlRemoveChildTest : XamlTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            v.AssertExistenceOf("GroupPath", "Index");

            // Right now we have nothing to validate this path against.
            // We'll have to defer until later...
            collectionPath = v["GroupPath"] + ".Children";
            index = StringConverter.ToInt(v["Index"]);
        }

        /// <summary/>
        public override void RunTheTest()
        {
            // Render and verify the initial scene
            RenderWindowContent();
            VerifyWithinContext();

            // Remove a child from one of the collections and verify again
            ModifyWindowContent();
            VerifyWithinContext();
        }

        /// <summary/>
        public override Visual GetWindowContent()
        {
            // "Thaw" the frozen objects because loading Xaml creates frozen objects.

            Log("'Thawing' frozen 3D objects...");
            Log("");
            parameters.ThawViewport();
            return parameters.Viewport;
        }

        /// <summary/>
        public override void ModifyWindowContent(Visual v)
        {
            IList collection = ObjectUtils.GetDependencyObject(collectionPath, v) as IList;

            if (collection == null)
            {
                AddFailure("'{0}' does not refer to a valid collection", collectionPath);
                Log("Test is broken");
            }

            Log("");
            Log("Removing object {0} from {1}", index, collectionPath);
            Log("");
            collection.RemoveAt(index);
        }

        /// <summary/>
        protected int index;
        /// <summary/>
        protected string collectionPath;
    }
}