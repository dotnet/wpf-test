// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xaml;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Globalization;
using System.Xaml.Schema;
using System.Windows.Markup;
using System.ComponentModel;

namespace Microsoft.Xaml.Tools
{

    public enum UISchemaContextMode
    {
        WPF,
        Silverlight,
    }

    // This SchemaContext will map between WPF and Silverlight types allowing you to write code against either platform and have it
    // automatically resolve against the correct framework.
    public class UISchemaContext : XamlSchemaContext
    {
        private UISchemaContextMode _mode;

        private List<Assembly> _silverlightAssemblies;
        private List<Assembly> _wpfAssemblies;
        private Dictionary<Type, XamlType> _typesDictionary;
        private Dictionary<XamlType, XamlType> _wrappedTypes;
        private Dictionary<XamlMember, XamlMember> _wrappedMembers;
        private Dictionary<XamlValueConverter<XamlDeferringLoader>, XamlValueConverter<XamlDeferringLoader>> _wrappedDeferringLoaders;
        private Dictionary<XamlValueConverter<ValueSerializer>, XamlValueConverter<ValueSerializer>> _wrappedValueSerializers;
        private Dictionary<XamlValueConverter<TypeConverter>, XamlValueConverter<TypeConverter>> _wrappedTypeConverters;
        private XamlSchemaContext _underlyingSchemaContext;
        private bool _isAssemblyResolverRegistered;

        public UISchemaContext(UISchemaContextMode mode)
        {
            _mode = mode;
            _typesDictionary = new Dictionary<Type, XamlType>();
            _wrappedTypes = new Dictionary<XamlType, XamlType>();
            _wrappedMembers = new Dictionary<XamlMember, XamlMember>();
            _wrappedDeferringLoaders = new Dictionary<XamlValueConverter<XamlDeferringLoader>, XamlValueConverter<XamlDeferringLoader>>();
            _wrappedValueSerializers = new Dictionary<XamlValueConverter<ValueSerializer>, XamlValueConverter<ValueSerializer>>();
            _wrappedTypeConverters = new Dictionary<XamlValueConverter<TypeConverter>, XamlValueConverter<TypeConverter>>();

            if (mode == UISchemaContextMode.Silverlight)
            {
                try
                {
                    // We don't want want to reference this assembly directly since we don't want to have a hard dependency on System.Windows.dll
                    Assembly asm = Assembly.Load("Microsoft.Xaml.Tools.Silverlight");
                    Type type = asm.GetType("Microsoft.Xaml.Tools.Silverlight.SilverlightSchemaContext");
                    _underlyingSchemaContext = Activator.CreateInstance(type) as XamlSchemaContext;
                }
                catch (Exception ex)
                {
                    throw new NotSupportedException("Please install the Silverlight XamlToolkit", ex);
                }
            }
            else
            {
                _underlyingSchemaContext = System.Windows.Markup.XamlReader.GetWpfSchemaContext();
            }
        }

        public UISchemaContextMode Mode { get { return _mode; } }

        public override XamlType GetXamlType(System.Type type)
        {
            XamlType xamlType;

            if (!_typesDictionary.TryGetValue(type, out xamlType))
            {
                Type newType = null;
                if (_mode == UISchemaContextMode.Silverlight)
                {
                    if (IsWpfType(type))
                    {
                        newType = FindEquivalentSilverlightType(type);
                    }
                    else
                    {
                        newType = type;
                    }
                }
                else
                {
                    if (IsSilverlightType(type))
                    {
                        newType = FindEquivalentWpfType(type);
                    }
                    else
                    {
                        newType = type;
                    }
                }

                xamlType = GetWrappedXamlType(_underlyingSchemaContext.GetXamlType(newType));
                _typesDictionary.Add(type, xamlType);
            }
            return xamlType;
        }

        internal XamlType GetWrappedXamlType(XamlType xamlType)
        {
            if (xamlType == null) return null;
            XamlType wrappedType;
            if (!_wrappedTypes.TryGetValue(xamlType, out wrappedType))
            {
                wrappedType = new WrapperXamlType(xamlType, this);
                _wrappedTypes[xamlType] = wrappedType;
            }
            return wrappedType;
        }

        internal List<XamlType> GetWrappedXamlTypes(IEnumerable<XamlType> types)
        {
            if (types == null) return null;
            List<XamlType> xamlTypes = new List<XamlType>();
            foreach (XamlType type in types)
            {
                xamlTypes.Add(GetWrappedXamlType(type));
            }
            return xamlTypes;
        }

        internal XamlMember GetWrappedXamlMember(XamlMember xamlMember)
        {
            if (xamlMember == null) return null;
            XamlMember member;
            if (!_wrappedMembers.TryGetValue(xamlMember, out member))
            {
                member = new WrapperXamlMember(xamlMember, this);
                _wrappedMembers[xamlMember] = member;
            }
            return member;
        }

        internal List<XamlMember> GetWrappedXamlMembers(IEnumerable<XamlMember> members)
        {
            if (members == null) return null;

            List<XamlMember> xamlMembers = new List<XamlMember>();
            foreach (XamlMember member in members)
            {
                xamlMembers.Add(GetWrappedXamlMember(member));
            }
            return xamlMembers;
        }

        internal XamlValueConverter<XamlDeferringLoader> GetWrappedDeferringLoader(XamlValueConverter<XamlDeferringLoader> converter)
        {
            if (converter == null) return null;
            XamlValueConverter<XamlDeferringLoader> valueConverter;
            if (!_wrappedDeferringLoaders.TryGetValue(converter, out valueConverter))
            {
                valueConverter = new WrapperXamlValueConverter<XamlDeferringLoader>(converter, this);
                _wrappedDeferringLoaders[converter] = valueConverter;
            }
            return valueConverter;
        }

        internal XamlValueConverter<ValueSerializer> GetWrappedValueSerializer(XamlValueConverter<ValueSerializer> converter)
        {
            if (converter == null) return null;
            XamlValueConverter<ValueSerializer> valueConverter;
            if (!_wrappedValueSerializers.TryGetValue(converter, out valueConverter))
            {
                valueConverter = new WrapperXamlValueConverter<ValueSerializer>(converter, this);
                _wrappedValueSerializers[converter] = valueConverter;
            }
            return valueConverter;
        }

        internal XamlValueConverter<TypeConverter> GetWrappedTypeConverter(XamlValueConverter<TypeConverter> converter)
        {
            if (converter == null) return null;
            XamlValueConverter<TypeConverter> valueConverter;
            if (!_wrappedTypeConverters.TryGetValue(converter, out valueConverter))
            {
                valueConverter = new WrapperXamlValueConverter<TypeConverter>(converter, this);
                _wrappedTypeConverters[converter] = valueConverter;
            }
            return valueConverter;
        }

        private Type FindEquivalentWpfType(System.Type originalType)
        {
            LoadWpfAssemblies();

            foreach (Assembly asm in _wpfAssemblies)
            {
                Type type = asm.GetType(originalType.FullName, false);
                if (type != null)
                {
                    return type;
                }
            }

            throw new NotSupportedException("Could not find " + originalType.FullName + " in Windows Presentation Foundation.");
        }

        private Type FindEquivalentSilverlightType(System.Type originalType)
        {
            if (new AssemblyName(originalType.Assembly.FullName).Name == "mscorlib")
            {
                return originalType;
            }
            LoadSilverlightAssemblies();

            foreach (Assembly asm in _silverlightAssemblies)
            {
                Type type = asm.GetType(originalType.FullName, false);
                if (type != null)
                {
                    return type;
                }
            }

            throw new NotSupportedException("Could not find " + originalType.FullName + " in Silverlight.");
        }

        private void LoadWpfAssemblies()
        {
            if (_wpfAssemblies == null)
            {
                _wpfAssemblies = new List<Assembly>();
                _wpfAssemblies.Add(typeof(System.Windows.DependencyObject).Assembly);
                _wpfAssemblies.Add(typeof(System.Windows.Media.Visual).Assembly);
                _wpfAssemblies.Add(typeof(System.Windows.FrameworkElement).Assembly);
            }
        }

        private void LoadSilverlightAssemblies()
        {
            if (_silverlightAssemblies == null)
            {
                _silverlightAssemblies = new List<Assembly>();

                foreach (string path in GetAllSilverlightPaths())
                {
                    AddAssembliesFromPath(path, _silverlightAssemblies);
                }
            }
        }

        private void AddAssembliesFromPath(string path, List<Assembly> assemblies)
        {
            foreach (String filePath in Directory.GetFiles(path, "*.dll"))
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(filePath);
                    if (asm != null)
                    {
                        assemblies.Add(asm);
                    }
                }
                catch
                { }
            }
            if (!_isAssemblyResolverRegistered)
            {
                _isAssemblyResolverRegistered = true;
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LoadResolveHander);
            }
        }

        static Assembly LoadResolveHander(object sender, ResolveEventArgs args)
        {
            string name = args.Name;
            Assembly[] loadedAsms = AppDomain.CurrentDomain.GetAssemblies();
            AssemblyName asmName = new AssemblyName(name);
            if (asmName.Name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(Object).Assembly;
            }

            foreach (Assembly asm in loadedAsms)
            {
                if (asm.FullName == name)
                    return asm;
            }

            return null;
        }



        private string[] GetAllSilverlightPaths()
        {
            List<String> list = new List<string>();
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\Silverlight\v3.0\ReferenceAssemblies"))
                {
                    if (key != null)
                    {
                        list.Add((string)key.GetValue("SLRuntimeInstallPath"));
                    }
                }

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\Silverlight\v3.0\AssemblyFoldersEx"))
                {
                    if (key != null)
                    {
                        foreach (string str2 in key.GetSubKeyNames())
                        {
                            using (RegistryKey key2 = key.OpenSubKey(str2))
                            {
                                if (key2 != null)
                                {
                                    string str3 = (string)key2.GetValue(null);
                                    if (!string.IsNullOrEmpty(str3) && Directory.Exists(str3))
                                    {
                                        list.Add(str3);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return list.ToArray();
        }


        private bool IsWpfType(System.Type type)
        {
            return !IsSilverlightType(type);
        }

        // This does the same check as Microsoft.VisualStudio.Silverlight.SLUtil.IsSilverlightAssembly()
        private bool IsSilverlightType(System.Type type)
        {
            foreach (AssemblyName asmName in type.Assembly.GetReferencedAssemblies())
            {
                if (asmName.Name.Equals("mscorlib", System.StringComparison.OrdinalIgnoreCase))
                {
                    if (asmName.Version.Major == 2 && asmName.Version.Minor == 0 && asmName.Version.Build == 5)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
    }  
}
