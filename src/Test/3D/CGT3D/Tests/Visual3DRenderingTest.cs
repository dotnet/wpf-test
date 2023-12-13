// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Render and verify complex Visual3D trees
    /// </summary>
    public class Visual3DRenderingTest : VisualVerificationTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            parameters = new SceneTestObjects(v);
        }

        /// <summary/>
        public override Visual GetWindowContent()
        {
            return parameters.Content;
        }

        /// <summary/>
        public override void Verify()
        {
            VerifySerialization(parameters.Content);
            VerifyWithSceneRenderer(parameters.SceneRenderer);
        }

        /// <summary/>
        protected SceneTestObjects parameters;
    }
}