// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows;

namespace DRTFreezable
{
    /// <summary>
    /// TestFreezable is minimalistic implementation of the Freezable contract
    /// for use in the Freezable DRT.
    /// </summary>
    public class TestFreezable : Freezable
    {
        private SimpleFreezableProperty<int> _simpleProperty = new SimpleFreezableProperty<int>(0);
        private NestedFreezableProperty<TestFreezable> _nestedProperty = new NestedFreezableProperty<TestFreezable>(null);

        public static readonly DependencyProperty SimpleDPProperty = 
            DependencyProperty.Register(
                "SimpleDP", 
                typeof(bool), 
                typeof(TestFreezable));

        public static readonly DependencyProperty NestedDPProperty = 
            DependencyProperty.Register(
                "NestedDP", 
                typeof(TestFreezable), 
                typeof(TestFreezable));
                
        public new TestFreezable Clone()
        {
            return (TestFreezable) base.Clone();
        }

        /// <summary>
        /// Example of a "simple" property (return type is not a nested Freezable).
        /// </summary>
        public int SimpleProperty
        {
            get { return _simpleProperty.GetValue(this); }
            set { _simpleProperty.SetValue(this, value); }
        }

        /// <summary>
        /// Example of a non-simple property (return type is itself another Freezable.)
        /// </summary>
        public TestFreezable NestedProperty
        {
            get { return _nestedProperty.GetValue(this); }
            set { _nestedProperty.SetValue(this, value); }
        }

        // Not yet used
        public bool SimpleDP
        { 
            get { return (bool) GetValue(SimpleDPProperty); }
            set { SetValue(SimpleDPProperty, value); }
        }

        // Not yes used
        public TestFreezable NestedDP
        { 
            get { return (TestFreezable) GetValue(NestedDPProperty); }
            set { SetValue(NestedDPProperty, value); }
        }

        protected override void OnChanged()
        {
            ChangedCount++;

            base.OnChanged();
        }
        
        protected override Freezable CreateInstanceCore()
        {
            return new TestFreezable();
        }

        protected override void CloneCore(Freezable sourceFreezable)
        {
            TestFreezable sourceTestFreezable = (TestFreezable) sourceFreezable;
            base.CloneCore(sourceFreezable);

            _simpleProperty.ValueInternal = sourceTestFreezable._simpleProperty.ValueInternal;
            _nestedProperty.ValueInternal = sourceTestFreezable._nestedProperty.ValueInternal;
        }

        protected override bool FreezeCore(bool isChecking)
        {
            bool canFreeze = base.FreezeCore(isChecking);

            canFreeze &= Freezable.Freeze(_nestedProperty.ValueInternal, isChecking);

            return canFreeze;
        }

        // Generic implementation of the Freezable contract for a simple
        // (non-Freezable return type) property.
        private struct SimpleFreezableProperty<T>
        {
            public SimpleFreezableProperty(T defaultValue)
            {
                _value = defaultValue;
            }
        

            public T GetValue(TestFreezable owner)
            {
                owner.ReadPreamble();

                return ValueInternal;
            }
        
            public void SetValue(TestFreezable owner, T newValue)
            {
                owner.WritePreamble();

                ValueInternal = newValue;

                owner.WritePostscript();
            }

            internal T ValueInternal
            {
                get { return _value; }
                set { _value = value; }
            }

            private T _value;
        }

        // Generic implementation of the Freezable contract for a
        // non-simple (Freezable return type) property.
        private struct NestedFreezableProperty<T> where T: Freezable
        {
            public NestedFreezableProperty(T defaultValue)
            {
                _value = defaultValue;
            }

            public T GetValue(TestFreezable owner)
            {
                owner.ReadPreamble();
                
                return ValueInternal;
            }

            public void SetValue(TestFreezable owner, T newValue)
            {
                owner.WritePreamble();

                owner.OnFreezablePropertyChanged(ValueInternal, newValue);
                ValueInternal = newValue;
                
                owner.WritePostscript();
            }
            
            internal T ValueInternal
            {
                get { return _value; }
                set { _value = value; }
            }

            private T _value;
        }

        public int ChangedCount = 0;
    }

    public class MDFreezable : Freezable
    {
        private double _simpleProperty;

        public double SimpleProperty
        {
            get
            {
                ReadPreamble();

                return _simpleProperty;
            }

            set
            {
                WritePreamble();

                _simpleProperty = value;

                WritePostscript();
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new MDFreezable();
        }

        protected override void CloneCore(Freezable sourceFreezable)
        {
            MDFreezable mdFreezable = (MDFreezable) sourceFreezable;
            base.CloneCore(sourceFreezable);

            _simpleProperty = mdFreezable._simpleProperty;
        }
    }
}
