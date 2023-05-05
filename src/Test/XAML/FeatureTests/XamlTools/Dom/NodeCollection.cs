// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public class NodeCollection<T> : IList<T> where T: DomNode
    {
        private DomNode _parentNode;
        private List<T> _nodes;
        private bool _isSealed;

        public NodeCollection(DomNode parent)
        {
            _parentNode = parent;
        }

        public int Count
        {
            get { return Nodes.Count; }
        }

        public void Seal()
        {
            _isSealed = true;
            foreach (DomNode node in _nodes)
            {
                node.Seal();
            }
        }

        public bool IsSealed { get { return _isSealed; } }

        #region IList<T> Members
        public void Add(T node)
        {
            CheckSealed();
            Nodes.Add(node);
            SetParent(node);
        }
        public int IndexOf(T item)
        {
            return Nodes.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            CheckSealed();
            Nodes.Insert(index, item);
            SetParent(item);
        }

        public void RemoveAt(int index)
        {
            CheckSealed();
            SetParentToNull(Nodes[index]);
            Nodes.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return Nodes[index];
            }
            set
            {
                CheckSealed();
                Nodes[index] = value;
                SetParent(value);
            }
        }

        #endregion

        #region ICollection<T> Members

        public bool IsReadOnly { get { return _isSealed; } }

        public void Clear()
        {
            CheckSealed();
            foreach (T node in Nodes)
            {
                SetParentToNull(node);
            }
            Nodes.Clear();
        }

        public bool Contains(T item)
        {
            return Nodes.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            CheckSealed();
            Nodes.CopyTo(array, arrayIndex);
            //
        }

        public bool Remove(T item)
        {
            CheckSealed();
            SetParentToNull(item);
            return Nodes.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        #endregion

        private List<T> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<T>();
                }
                return _nodes;
            }
        }

        private void SetParent(T node)
        {
            ItemNode itemNode = node as ItemNode;
            MemberNode propNode = node as MemberNode;

            if (itemNode != null)
            {
                itemNode.ParentMemberNode = (MemberNode)_parentNode;
            }
            if (propNode != null)
            {
                propNode.ParentObjectNode = (ObjectNode)_parentNode;
            }
        }
        private void SetParentToNull(T node)
        {
            ObjectNode objNode = node as ObjectNode;
            MemberNode propNode = node as MemberNode;

            if (objNode != null)
            {
                objNode.ParentMemberNode = null;
            }
            if (propNode != null)
            {
                propNode.ParentObjectNode = null;
            }
        }

        private void CheckSealed()
        {
            if (this.IsSealed)
            {
                throw new NotSupportedException("The MemberNode is read-only");
            }
        }
    }
}
