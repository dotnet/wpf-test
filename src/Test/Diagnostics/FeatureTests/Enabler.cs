using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Microsoft.Win32;

namespace Microsoft.Test.Diagnostics
{
    public abstract class Enabler : IDisposable
    {
        protected const string DisableDiagnosticsSwitchName = "Switch.System.Windows.Diagnostics.DisableDiagnostics";
        Action _enable, _disable;

        protected Enabler(Action enable, Action disable)
        {
            _enable = enable;
            _disable = disable;
        }

        public abstract void Dispose();

        protected void Enable()
        {
            if (_enable != null)    _enable();
        }

        protected void Disable()
        {
            if (_disable != null)   _disable();
        }

        // derived classes can set (in ctor) and restore (in Dispose)
        // external state of various kinds:

        //
        // registry key
        //
        string _keyName;
        string _valueName;
        bool _keyExisted;
        object _originalKeyValue;

        protected void SetRegistryKey(string keyName, string valueName, object value)
        {
            _keyName = keyName;
            _valueName = valueName;

            RegistryKey key = Registry.LocalMachine.OpenSubKey(_keyName, true);
            _keyExisted = (key != null);
            if (!_keyExisted)
            {
                key = Registry.LocalMachine.CreateSubKey(_keyName);
            }

            using (key)
            {
                _originalKeyValue = key.GetValue(_valueName);
                key.SetValue(_valueName, value);
            }
        }

        protected void RestoreRegistryKey()
        {
            if (_keyExisted)
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(_keyName, true);
                using (key)
                {
                    if (_originalKeyValue != null)
                    {
                        key.SetValue(_valueName, _originalKeyValue);
                    }
                    else
                    {
                        key.DeleteValue(_valueName);
                    }
                }
            }
            else
            {
                Registry.LocalMachine.DeleteSubKey(_keyName);
            }
        }

        //
        // private static field
        //
        FieldInfo _overrideField;
        object _originalFieldValue;

        protected void SetPrivateField(Type type, string name, object value)
        {
            _overrideField = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static);
            _originalFieldValue = _overrideField.GetValue(null);
            _overrideField.SetValue(null, value);
        }

        protected void RestorePrivateField()
        {
            _overrideField.SetValue(null, _originalFieldValue);
        }

        //
        // environment variable
        //
        string _varName;
        string _originalEnvironmentValue;

        protected void SetEnvironmentVariable(string varName, string value)
        {
            _varName = varName;
            _originalEnvironmentValue = Environment.GetEnvironmentVariable(_varName);
            Environment.SetEnvironmentVariable(_varName, value);
        }

        protected void RestoreEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable(_varName, _originalEnvironmentValue);
        }

        //
        // app-context switch
        //
        string _switchName;
        bool _originalSwitchValue;
        bool _wasSwitchSet;

        protected void SetSwitch(string switchName, bool value)
        {
            _switchName = switchName;
            _wasSwitchSet = AppContext.TryGetSwitch(_switchName, out _originalSwitchValue);
            AppContext.SetSwitch(_switchName, value);
        }

        protected void RestoreSwitch()
        {
            if (_wasSwitchSet)
            {
                AppContext.SetSwitch(_switchName, _originalSwitchValue);
            }
        }
    }


    public class RegistryEnabler : Enabler
    {
        public RegistryEnabler(string keyName, string valueName, object value, Action enable=null, Action disable=null)
            : base(enable, disable)
        {
            SetRegistryKey(keyName, valueName, value);
            Enable();
        }

        public override void Dispose()
        {
            Disable();
            RestoreRegistryKey();
        }
    }


    public class PrivateReflectionEnabler : Enabler
    {
        public PrivateReflectionEnabler(Type type, string name, object value, Action enable=null, Action disable=null)
            : base(enable, disable)
        {
            SetPrivateField(type, name, value);
            Enable();
        }

        public override void Dispose()
        {
            Disable();
            RestorePrivateField();
        }
    }


    public class EnvironmentEnabler : Enabler
    {
        public EnvironmentEnabler(string varName, string value, Action enable=null, Action disable=null)
            : base(enable, disable)
        {
            SetEnvironmentVariable(varName, value);
            Enable();
        }

        public override void Dispose()
        {
            Disable();
            RestoreEnvironmentVariable();
        }
    }


    public class SwitchEnabler : Enabler
    {
        public SwitchEnabler(string switchName, bool value, Action enable, Action disable)
            : base(enable, disable)
        {
            SetSwitch(switchName, value);
            Enable();
        }

        public override void Dispose()
        {
            Disable();
        }
    }


    public class NullEnabler : Enabler
    {
        public NullEnabler(Action enable=null, Action disable=null)
            : base(enable, disable)
        {
            Enable();
        }

        public override void Dispose()
        {
            Disable();
        }
    }

    public class DisableEnabler : SwitchEnabler
    {
        public DisableEnabler(Action enable=null, Action disable=null)
            : base(DisableDiagnosticsSwitchName, true, enable, disable)
        {
        }
    }

    public class DevModeEnabler : RegistryEnabler
    {
        public DevModeEnabler(Action enable=null, Action disable=null)
            : base( @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock",
                    "AllowDevelopmentWithoutDevLicense",
                    1,
                    enable, disable)
        {
        }
    }

    public class ResourceDictionaryTestEnabler : Enabler
    {
        string _mode;

        public ResourceDictionaryTestEnabler(string mode)
            : base(null, null)
        {
            _mode = mode;

            switch (_mode)
            {
                case "Enable":
                    SetEnvironmentVariable("ENABLE_XAML_DIAGNOSTICS_SOURCE_INFO", "1");
                    break;
                case "Null":
                    break;
                case "Disable":
                    SetSwitch(DisableDiagnosticsSwitchName, true);
                    SetEnvironmentVariable("ENABLE_XAML_DIAGNOSTICS_SOURCE_INFO", "1");
                    break;
                default:
                    throw new ArgumentException(String.Format("Unrecognized mode '{0}'", mode), nameof(mode));
            }

        }

        public override void Dispose()
        {
            switch (_mode)
            {
                case "Enable":
                    RestoreEnvironmentVariable();
                    break;
                case "Null":
                    break;
                case "Disable":
                    RestoreEnvironmentVariable();
                    RestoreSwitch();
                    break;
            }
        }

        public string Mode
        {
            get { return _mode; }
        }
    }
}
