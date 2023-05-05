// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xaml.Utilities.XamlDom;

namespace Microsoft.Xaml.Utilities.XamlDom
{
    public abstract class NodeChange
    {
    }

    public class MemberNodeRemoval : NodeChange
    {
        public MemberNodeRemoval(MemberNode memberNode, int nodePosition)
        {
            MemberNode = memberNode;
            NodePosition = nodePosition;
        }
        public MemberNode MemberNode { get; protected set; }
        public int NodePosition { get; protected set; }
    }

    public class MemberValueSet : NodeChange
    {
        public MemberValueSet(ValueNode lastValueNode, ValueNode valueNode)
        {
            LastValueNode = lastValueNode;
            ValueNode = valueNode;
        }
        public ValueNode LastValueNode { get; protected set; }
        public ValueNode ValueNode { get; protected set; }
    }
}

/*
// Need to determine how to expose these...are they in the core dom, etc...
        public MemberValueSet SetMemberValue(XamlMember xamlMember, object value)
        {
            ValueNode lastValueNode = ValueNode.None;
            foreach (MemberNode memberNode in this.MemberNodes)
            {
                if (memberNode.Member == xamlMember)
                {
                    lastValueNode = memberNode.ItemNodes[0] as ValueNode;
                }
            }

            MemberNode newMemberNode = new MemberNode();
            newMemberNode.Member = xamlMember;
            newMemberNode.ParentObjectNode = this;
            ValueNode valueNode = new ValueNode();
            valueNode.Value = value;
            newMemberNode.ItemNodes.Clear();
            newMemberNode.ItemNodes.Add(valueNode);
            this.MemberNodes.Add(newMemberNode);

            MemberValueSet memberValueSet = new MemberValueSet(lastValueNode, valueNode);
            return memberValueSet;
        }

        public MemberNodeRemoval RemoveMember(XamlMember xamlMember)
        {
            int i = 0;
            MemberNodeRemoval memberNodeRemoval = null;
            foreach (MemberNode memberNode in this.MemberNodes)
            {
                if (memberNode.Member == xamlMember)
                {
                    memberNodeRemoval = new MemberNodeRemoval(memberNode, i);
                    break;
                }
                i++;
            }
            this.MemberNodes.Remove(memberNodeRemoval.MemberNode);
            return memberNodeRemoval;
        }


*/
