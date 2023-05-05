// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel; //this is only used for _baseDictionary.Contains().

namespace Microsoft.Xaml.Tools.XamlDom
{
    internal class SealableNamespaceCollection : KeyedCollection<string, XamlDomNamespace>
    {
        public void Seal()
        {
            _isSealed = true;

            foreach (XamlDomNamespace xdns in this)
            {
                xdns.Seal();
            }
        }

        public bool IsSealed { get { return _isSealed; } }

        #region KeyedCollection Members

        protected override String GetKeyForItem(XamlDomNamespace item)
        {
            return item.NamespaceDeclaration.Prefix;
        }

        protected override void InsertItem(int index, XamlDomNamespace item)
        {
            CheckSealed();
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            CheckSealed();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, XamlDomNamespace item)
        {
            CheckSealed();
            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            CheckSealed();
            base.ClearItems();
        }

        #endregion



        private void CheckSealed()
        {
            if (IsSealed)
            {
                throw new NotSupportedException("The Dictionary has been sealed.");
            }
        }
        private bool _isSealed;
    }
}
