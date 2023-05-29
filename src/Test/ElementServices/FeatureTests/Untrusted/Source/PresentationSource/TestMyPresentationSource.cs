// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;

using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using System.Collections;


namespace Avalon.Test.CoreUI.Source.Hwnd
{

    internal class MyPresentationSource : PresentationSource, IDisposable
    {

        public MyPresentationSource(){}

        public MyPresentationSource(bool addingSource)
        {
            if (addingSource)
                AddingSource();
        }
        
       public void AddingSource()
       {
            base.AddSource();
       }

        public void RemovingSource()
        {
            base.RemoveSource();
        }

        public void Dispose()
        {
            _isDisposed = true;
            RemovingSource();
        }

        internal void ChangeRoot(Visual oldRoot, Visual newRoot)
        {
            base.RootChanged( oldRoot,  newRoot);
        }
    
        public override bool IsDisposed 
        { 
            get
            {
                return _isDisposed;
            }
        }

        bool _isDisposed = false;

        Visual _root = null;

            /// <summary>
        ///     The root visual being presented in the source.
        /// </summary>
        override public  Visual RootVisual 
        {

            get
            {
                return _root;
            }
            set
            {

				Visual _oldVisual = _root;
				_root = value;
                ChangeRoot(_oldVisual, value);

            }
        }

        /// <summary>
        ///     The visual manager for the visuals being presented in the source.
        /// </summary>
        override protected CompositionTarget GetCompositionTargetCore()
        { 
            throw new NotImplementedException("");          
        }

    }
}




