// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PomParser.Baml.Common
{
    internal class BamlFlowDirectionResource
    {
        internal BamlFlowDirectionResource(
            string              resourceId,
            string              resourceType,
            BamlFlowDirection   bamlFlowDirection,
            bool                isRoot
            )
        {
            if (resourceId == null)
            {
                throw new ArgumentNullException("resourceId");
            }

            if ( bamlFlowDirection != BamlFlowDirection.Inherited
              && bamlFlowDirection != BamlFlowDirection.LeftToRight
              && bamlFlowDirection != BamlFlowDirection.RightToLeft
                )
            {
                throw new InvalidEnumArgumentException(
                    "BamlFlowDirection", 
                    (int)bamlFlowDirection, 
                    typeof(BamlFlowDirection)
                    );
            }
            
            _resourceId    = resourceId;
            _resourceType  = resourceType;
            _bamlFlowDirection = bamlFlowDirection;
            _isRoot        = isRoot;            
        }

        internal string ResourceId
        {
            get { return _resourceId; }
        }

        internal string ResourceType
        {
            get { return _resourceType; }
        }

        internal BamlFlowDirection FlowDirection
        {
            get { return _bamlFlowDirection; }
        }

        internal bool IsRoot
        {
            get { return _isRoot; }
        }

        private string _resourceId;
        private string _resourceType;
        private BamlFlowDirection _bamlFlowDirection;
        private readonly  bool _isRoot;
    }
}
