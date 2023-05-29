// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;


namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class ExeStubContainerFramework : ExeStubContainerCore
    {

        /// <summary>
        /// 
        /// </summary>
        public ExeStubContainerFramework() : base()
        {
            Initialize(Avalon.Test.CoreUI.Common.HostType.HwndSource);            
        }
        
        /// <summary>
        ///
        /// </summary>
        public ExeStubContainerFramework(Avalon.Test.CoreUI.Common.HostType hostType) : base()
        {
            Initialize(hostType);
        }

        /// <summary>
        /// Close the last modal window
        /// </summary>
        public override void CloseLastModal()
        {
            Surface modalWindow = null;
            
            lock(ModalStack)
            {
                if (ModalStack.Count > 0)
                {
                    modalWindow = ModalStack.Pop();
                }
            }

            if (modalWindow != null)
            {
                modalWindow.Close();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override Surface DisplayObject(object visual, int x, int y, int w, int h)
        {
            SurfaceFramework surface = new SurfaceFramework(_hostType.ToString(),x,y,w,h);

            surface.DisplayObject(visual);

            this.AddSurface(surface);

            return surface;
        }

        /// <summary>
        ///
        /// </summary>
        public override void DisplayObjectModal(object visual, int x, int y, int w, int h)
        {

            SurfaceFramework surface = new SurfaceFramework("Window",x,y,w,h,false);

            lock (ModalStack)
            {
                ModalStack.Push(surface);
            }
            surface.DisplayObject(visual); 
            surface.ShowModal();
            
        }

        /// <summary>
        /// </summary>
        public Avalon.Test.CoreUI.Common.HostType HostType
        {
            get
            {
                return _hostType;
            }
            set
            {
                _hostType = value;
            }
        }

        private void Initialize(Avalon.Test.CoreUI.Common.HostType hostType)
        {
            _hostType = hostType;
        }

        private Avalon.Test.CoreUI.Common.HostType _hostType  ;

    }
}


