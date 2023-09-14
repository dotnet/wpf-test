// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Modeling;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using System.Text;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Resources
{    
    /// <summary>
    /// ResourceModel State capture
    /// </summary>        
    [Serializable]
    class ResourcesModelState : CoreState
    {
        /// <summary>
        /// Set default value, useful for unit testing
        /// </summary>                
        public ResourcesModelState()
        {
            //Kind = "XamlCompile";
            //ResourceUseLocation = "TemplateTriggerConditionValue_ControlTemplate_MultiTrigger";
            //InlineOwnerAndResourceType = "N/A";
            //ResourceDefinitionLocation = "PresentRD";
            //ResolutionSteps = 3;
            //ReferenceType = "AllStatic";
            //MarkupSyntax = "PropertyElement";
            //SameRDPreferred = "false";
            //ApplicationResourcePreferred = "false";
        }

        public ResourcesModelState(CoreState state)
        {
            this.Dictionary = (PropertyBag)state.Dictionary.Clone();
        }        

        public void ParseState(PropertyBag state)
        {           
        }

        public ResourcesModelState(State state) : base (state)
        {            
        }

                
        public override void LogState()
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append(" Kind: ");
            sb1.Append(Kind);
            sb1.Append(System.Environment.NewLine);

            sb1.Append(" ResourceUseLocation: ");
            sb1.Append(ResourceUseLocation);
            sb1.Append(System.Environment.NewLine);

            sb1.Append(" InlineOwnerAndResourceType: ");
            sb1.Append(InlineOwnerAndResourceType);
            sb1.Append(System.Environment.NewLine);

            sb1.Append(" ResourceDefinitionLocation: ");
            sb1.Append(ResourceDefinitionLocation);
            sb1.Append(System.Environment.NewLine);

            sb1.Append(" ResolutionSteps: ");
            sb1.Append(ResolutionSteps);
            sb1.Append(System.Environment.NewLine);

            sb1.Append(" ReferenceType: ");
            sb1.Append(ReferenceType);
            sb1.Append(System.Environment.NewLine);

            sb1.Append(" MarkupSyntax: ");
            sb1.Append(MarkupSyntax);
            sb1.Append(System.Environment.NewLine);

            sb1.Append(" SameRDPreferred: ");
            sb1.Append(SameRDPreferred);
            sb1.Append(System.Environment.NewLine);

            sb1.Append(" ApplicationResourcePreferred: ");
            sb1.Append(ApplicationResourcePreferred);

            CoreLogger.LogStatus(sb1.ToString());
        }

             
        public string Kind { get { return Dictionary["Kind"]; } }
        public string ResourceUseLocation { get { return Dictionary["ResourceUseLocation"]; } }
        public string InlineOwnerAndResourceType { get { return Dictionary["InlineOwnerAndResourceType"]; } }
        public string ResourceDefinitionLocation { get { return Dictionary["ResourceDefinitionLocation"]; } }
        public int ResolutionSteps { get { return Int32.Parse(Dictionary["ResolutionSteps"]); } }
        public string ReferenceType { get { return Dictionary["ReferenceType"]; } }
        public string MarkupSyntax { get { return Dictionary["MarkupSyntax"]; } }
        public string SameRDPreferred { get { return Dictionary["SameRDPreferred"]; } }
        public string ApplicationResourcePreferred { get { return Dictionary["ApplicationResourcePreferred"]; } }
    }
}
