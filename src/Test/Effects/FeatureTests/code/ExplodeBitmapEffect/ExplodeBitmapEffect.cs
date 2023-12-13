// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2005
 *
 *   Program:   TestEffects: ExplodeBitmapEffect
 *   Author:    Microsoft
 *   Note:      File copied over from wpf\Test\Common\Legacy\Gto\consolidated, so we won't have
 *              dependency on GTO. 
 ************************************************************/

using System;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using sysEff = System.Windows.Media.Effects;

namespace System.Windows.Media.Effects
{
    internal class COMSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal COMSafeHandle()
            : base(true)
        {
        }

        /// <summary>
        /// Release handle
        /// </summary>
        protected override bool ReleaseHandle()
        {
            Marshal.Release(handle);
            return true;
        }
    }

    public class Ole32Methods
    {
        [DllImport("ole32.dll")]
        internal static extern uint /* HRESULT */ CoCreateInstance(
            ref Guid clsid,
            IntPtr inner,
            uint context,
            ref Guid uuid,
            out COMSafeHandle ppEffect);
    }

    public class ExplodeBitmapEffect : BitmapEffect
    {
        public ExplodeBitmapEffect()
        {
        }

        /// <summary>
        /// UNSAFE. Create the unmanaged COM explode effect
        /// The corresponding dll must be registered, else we will fail.
        /// </summary>
//Only obsolete in Part1
#if TESTBUILD_CLR40
        [Obsolete(ExplodeBitmapEffect.BitmapEffectObsoleteMessage)]
#endif
        unsafe protected override SafeHandle CreateUnmanagedEffect()
        {
            const uint CLSCTX_INPROC_SERVER = 1;
            Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
            Guid guidEffectCLSID = new Guid("C9308B90-2011-47E6-BC3E-872B1AEF7258");
#pragma warning disable 0618
            SafeHandle wrapper = BitmapEffect.CreateBitmapEffectOuter();
#pragma warning restore 0618
            COMSafeHandle unmanagedEffect;
            uint hresult = Ole32Methods.CoCreateInstance(ref guidEffectCLSID, wrapper.DangerousGetHandle(),
                           CLSCTX_INPROC_SERVER, ref IID_IUnknown, out unmanagedEffect);
            if (0 != hresult)
            {
                throw new Exception("Cannot co-create the effect. HRESULT = " + hresult.ToString());
            }
            BitmapEffect.InitializeBitmapEffect(wrapper, unmanagedEffect);
            if (0 != hresult)
            {
                throw new Exception("Cannot instantiate effect. HRESULT = " + hresult.ToString());
            }
            return wrapper;
        }

        /// <summary>
        /// Updates the unamanged properties based on the managed changes.
        /// </summary>
        /// <param name="unmanagedEffect"> The unmanaged effect handle</param>
//Only obsolete in Part1
#if TESTBUILD_CLR40
        [Obsolete(ExplodeBitmapEffect.BitmapEffectObsoleteMessage)]
#endif
        protected override void UpdateUnmanagedPropertyState(SafeHandle unmanagedEffect)
        {
#pragma warning disable 0618
            BitmapEffect.SetValue(unmanagedEffect, "Radius", this.Radius);
            BitmapEffect.SetValue(unmanagedEffect, "SeedNumber", this.SeedNumber);
#pragma warning restore 0618
        }


        #region Public Methods

        /// <summary>
        /// Clone
        /// </summary>
        public new ExplodeBitmapEffect Clone()
        {
            return (ExplodeBitmapEffect)base.Clone();
        }

        /// <summary>
        /// Clone current state
        /// </summary>
        public new ExplodeBitmapEffect CloneCurrentValue()
        {
            return (ExplodeBitmapEffect)base.CloneCurrentValue();
        }

        #endregion Public Methods


        #region Protected Methods

        protected override Freezable CreateInstanceCore()
        {
            return new ExplodeBitmapEffect();
        }
       #endregion ProtectedMethods

        /// <summary>
        /// Size of explosion. Higher value expands it.
        /// </summary>
        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }
            set
            {
                SetValue(RadiusProperty, value);
            }
        }

        /// <summary>
        /// Radius DependencyProperty
        /// </summary>
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
                "Radius",
            typeof(double),
            typeof(ExplodeBitmapEffect),
            new PropertyMetadata(cDefaultRadius,
                          OnRadiusPropertyInvalidated));

        private static void OnRadiusPropertyInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExplodeBitmapEffect)d).OnChanged();
        }

        /// <summary>
        /// Seed number property. For the same radius, have different explosions. Seed controls the randomization
        /// </summary>
        [CLSCompliant(false)]
        public uint SeedNumber
        {
            get
            {
                return (uint)GetValue(SeedNumberProperty);
            }
            set
            {
                SetValue(SeedNumberProperty, value);
            }
        }

        /// <summary>
        /// Seed number DependencyProperty. 
        /// </summary>
        public static readonly DependencyProperty SeedNumberProperty =
            DependencyProperty.Register(
                "SeedNumber",
            typeof(uint),
            typeof(ExplodeBitmapEffect),
            new PropertyMetadata(cDefaultSeedNumber,
                          new PropertyChangedCallback(OnSeedNumberPropertyInvalidated)));

        private static void OnSeedNumberPropertyInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExplodeBitmapEffect)d).OnChanged();
        }

        #region Internal Fields
        internal const double cDefaultRadius = 1.25;
        internal const uint cDefaultSeedNumber = 0x29A;
        internal const string BitmapEffectObsoleteMessage = "BitmapEffect is Obsolete.";
        #endregion Internal Fields
    }
}