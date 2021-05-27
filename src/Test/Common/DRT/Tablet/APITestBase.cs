// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace DRT
{
    // The delegate of the event forwarding callback.
    public delegate object EventCallback(object arg);
    // The delegate of check the value of a property callback.
    public delegate void   ValueCheckingCallback(DrtBase drt, object value, string propertyName);

    /// <summary>
    /// This is an interface which can be used to assign a forwarding call when a test case is used for events tests.
    /// </summary>
    public interface IEventsForwading
    {
        // Set a callback when a proeperty's Changed event occurs
        void SetChangedCallback(EventCallback callback);
        // Set a callback when a proeperty's Changing event occurs
        void SetChangingCallback(EventCallback callback);
    }

    /// <summary>
    /// An abstract class which represents the basic test unit.
    /// </summary>
    internal abstract class TestUnit
    {
        #region Constructor

        protected TestUnit(DrtBase drt)
        {
            _drt = drt;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Retrieve an array of the test cases.
        /// </summary>
        /// <value></value>
        public DrtTest[] Tests
        {
            get
            {
                ArrayList tests = BuildTests();
                return tests == null ? new DrtTest[]{} : (DrtTest[])tests.ToArray(typeof(DrtTest));
            }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// An abstract method which builds the test cases.
        /// </summary>
        /// <returns></returns>
        protected abstract ArrayList BuildTests();

        #endregion Protected Methods

        #region Protected Properties

        /// <summary>
        /// Retrieve the corresponding DrtBase instance
        /// </summary>
        /// <value></value>
        protected DrtBase Drt
        {
            get
            {
                return _drt;
            }
        }

        #endregion Protected Properties

        #region Private Fields

        private DrtBase     _drt;

        #endregion Private Fields

    }

    /// <summary>
    /// The base class of the Property relevant test unit which uses Reflection to do the unit test on the properties of 
    /// an instance.
    /// </summary>
    internal abstract class PropertyTestUnit : TestUnit
    {
        #region Constructor

        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="drt">An instance of the DrtBase</param>
        /// <param name="instance">An instance of the tested object like InkCanvas</param>
        /// <param name="propertyName">The name of the tested property like EditingMode</param>
        /// <param name="callback">An instance of the IEventsForwading interface. If not null, the Changed/Changing Events will be forward to the test unit's handler.</param>
        protected PropertyTestUnit(DrtBase drt, object instance, string propertyName, IEventsForwading callback)
                : base(drt)
        {
            _callback = callback;
            _instance = instance;
            _propertyInfo = _instance.GetType().GetProperty(propertyName);
            _propertyName = propertyName;

            Drt.Assert(_propertyInfo != null, string.Format("The {0} type doesn't have the property - {1}.",  _instance.GetType(), propertyName));
        }

        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="drt">An instance of the DrtBase</param>
        /// <param name="instance">An instance of the tested object like InkCanvas</param>
        /// <param name="propertyName">The name of the tested property like EditingMode</param>
        protected PropertyTestUnit(DrtBase drt, object instance, string propertyName) 
            : this (drt, instance, propertyName, null)
        {
        }

        /// <summary>
        /// Mark the default constructor as private.
        /// </summary>
        private PropertyTestUnit() : base(null) { }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// A method which invokes the test.
        /// </summary>
        public virtual void OnInvoke()
        {
            if ( _callback != null )
            {
                _callback.SetChangingCallback(new EventCallback(ChangingCallback));
                _callback.SetChangedCallback(new EventCallback(ChangedCallback));
            }
        }

        /// <summary>
        /// A method which verifies the test.
        /// </summary>
        public virtual void OnVerify()
        {
            if ( _callback != null )
            {
                _callback.SetChangingCallback(null);
                _callback.SetChangedCallback(null);
            }
        }

        /// <summary>
        /// A method which is called when the property has been changed
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual object OnChanged(object args) { return null; }

        /// <summary>
        /// A method which is called when the property is changing.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual object OnChanging(object args) { return null; }

        #endregion Public Methods

        #region Protect Properties

        /// <summary>
        /// Gets/Sets the value of the tested property
        /// </summary>
        /// <value>The value</value>
        protected object Value
        {
            get
            {
                return _propertyInfo.GetValue(_instance, null);
            }
            set
            {
                _propertyInfo.SetValue(_instance, value, null);
            }
        }

        /// <summary>
        /// Gets the property name
        /// </summary>
        /// <value></value>
        protected string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }

        #endregion Protect Properties

        #region Private Methods

        /// <summary>
        /// The Changed callback which invokes OnChanged method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private object ChangedCallback(object args)
        {
            return OnChanged(args);
        }

        /// <summary>
        /// The Changing callback which invokes OnChanging method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private object ChangingCallback(object args)
        {
            return OnChanging(args);
        }

        #endregion Private Methods

        #region Private Fields

        private IEventsForwading _callback;


        private object          _instance;
        private PropertyInfo    _propertyInfo;
        private string          _propertyName;

        #endregion Private Fields

    }

    /// <summary>
    /// A test unit to verify accessing the property of an instance
    /// </summary>
    internal class AccessPropertyTest : PropertyTestUnit
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="drt">An instance of the DrtBase</param>
        /// <param name="instance">An instance of the tested object</param>
        /// <param name="propertyName">The property name of the tested property</param>
        /// <param name="newValue">The value which is set to the property.</param>
        /// <param name="callback">The interface which is used for forwarding events</param>
        /// <param name="changingCancelling">A flag which indicates whether cancels the changing event.</param>
        public AccessPropertyTest(DrtBase drt, object instance, string propertyName, object newValue, IEventsForwading callback, bool changingCancelling)
                : base (drt, instance, propertyName, callback)
        {
            _newValue = newValue;
            _changingCancelling = changingCancelling;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="drt">An instance of the DrtBase</param>
        /// <param name="instance">An instance of the tested object</param>
        /// <param name="propertyName">The property name of the tested property</param>
        /// <param name="newValue">The value which is set to the property.</param>
        public AccessPropertyTest(DrtBase drt, object instance, string propertyName, object value) 
                : this(drt, instance, propertyName, value, null, false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="drt">An instance of the DrtBase</param>
        /// <param name="instance">An instance of the tested object</param>
        /// <param name="propertyName">The property name of the tested property</param>
        /// <param name="newValue">An invalid value which is set to the property.</param>
        /// <param name="expectedExceptionType">The exception which the test unit expects</param>
        public AccessPropertyTest(DrtBase drt, object instance, string propertyName, object newValue, Type expectedExceptionType) 
                : this(drt, instance, propertyName, newValue, null, false)
        {
            _expectException = expectedExceptionType;
            _isInvalidValue = true;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// A method which sets the test value to the property
        /// </summary>
        public override void OnInvoke()
        {
            _oldValue = Value;

            base.OnInvoke();

            _isExceptionCaught = false;

            try
            {
                Value = _newValue;
            }
            catch ( Exception exp )
            {
                if ( _expectException != null && _expectException.IsInstanceOfType(exp.InnerException) )
                {
                    _isExceptionCaught = true;
                }
                else
                {
                    throw exp;
                }
            }
        }

        /// <summary>
        /// A method which verify the value of the property
        /// </summary>
        public override void OnVerify()
        {
            if ( !_isInvalidValue )
            {
                object value = Value;
                Drt.AssertEqual(_changingCancelling ? _oldValue : _newValue, value,
                                    string.Format("Set/Get the property ({0}) failed!", PropertyName));
            }
            else
            {
                Drt.AssertEqual(true, _isExceptionCaught,
                                    string.Format("Invalid value test of the property ({0}) failed!", PropertyName));
            }

            base.OnVerify();
        }

        /// <summary>
        /// The callback which is called when the Changing event occurs. The changing event can be cancelled by returning a boolean false.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override object OnChanging(object arg)
        {
            return _changingCancelling;
        }

        /// <summary>
        /// The callback which is called when the Changed event occurs. The method also verifies the new value has been set precisely
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override object OnChanged(object arg)
        {
            object value = Value;
            Drt.AssertEqual(_newValue, value, string.Format("Changed Event of the property ({0}) failed!", PropertyName));
            return null;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// An overriden method which builds the test case - Set the property, then Verify the property.
        /// </summary>
        /// <returns></returns>
        protected override ArrayList BuildTests()
        {
            ArrayList tests = new ArrayList();

            tests.Add(new DrtTest(OnInvoke));
            tests.Add(new DrtTest(OnVerify));

            return tests;
        }

        #endregion Protected Methods

        #region Private Fields

        private object          _newValue;
        private object          _oldValue;
        private bool            _changingCancelling;
        private bool            _isInvalidValue = false;
        private Type            _expectException = null;
        private bool            _isExceptionCaught;

        #endregion Private Fields
    }

    /// <summary>
    /// A test unit which is used to verify the default value of a tested property.
    /// </summary>
    internal class PropertyDefaultValueTest : PropertyTestUnit
    {
        public PropertyDefaultValueTest(DrtBase drt, object instance, string propertyName, object defaultValue)
                : base(drt, instance, propertyName, null)
        {
            _defaultValue = defaultValue;
        }

        public PropertyDefaultValueTest(DrtBase drt, object instance, string propertyName, ValueCheckingCallback callback)
                : base(drt, instance, propertyName, null)
        {
            _callback = callback;
        }

        public override void OnVerify()
        {
            object value = Value;

            if ( _callback == null )
            {
                Drt.AssertEqual(_defaultValue, value,
                                    string.Format("The default value of the property ({0}) failed!", PropertyName));
            }
            else
            {
                _callback(Drt, value, PropertyName);
            }
        }

        protected override ArrayList BuildTests()
        {
            ArrayList tests = new ArrayList();

            tests.Add(new DrtTest(OnVerify));

            return tests;
        }

        private object                  _defaultValue;
        private ValueCheckingCallback   _callback;
    }

    /// <summary>
    /// A test unit which is used to verify all values in the enum type of a property.
    /// </summary>
    internal class EnumTypePropertyTest : TestUnit
    {
        public EnumTypePropertyTest(DrtBase drt, object instance, string propertyName, Type type) : base(drt)
        {
            _tests = new ArrayList();

            TestUnit test;
            foreach ( int val in Enum.GetValues(type) )
            {
                object newValue = Enum.Parse(type, Enum.GetName(type, val));

                test = new AccessPropertyTest(drt, instance, propertyName, newValue);
                _tests.AddRange(test.Tests);
            }
        }

        protected override ArrayList BuildTests()
        {
            return _tests;
        }

        private ArrayList       _tests;

    }

    /// <summary>
    /// A test unit which is used to verify all values in the boolean type of a property.
    /// </summary>
    internal class BooleanTypePropertyTest : TestUnit
    {
        public BooleanTypePropertyTest(DrtBase drt, object instance, string propertyName) : base(drt)
        {
            _tests = new ArrayList();

            TestUnit test;

            test = new AccessPropertyTest(drt, instance, propertyName, true);
            _tests.AddRange(test.Tests);

            test = new AccessPropertyTest(drt, instance, propertyName, false);
            _tests.AddRange(test.Tests);
        }

        protected override ArrayList BuildTests()
        {
            return _tests;
        }

        private ArrayList _tests;

    }

}
