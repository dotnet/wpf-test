// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using Microsoft.Test.Integration;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

namespace Microsoft.Test.Integration.Windows
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public delegate bool TypeFilterEventHandler(Type type);

    /// <summary>
    /// Generate VariationItems with all the types specified on the TypeName property.
    /// </summary>
    public class TypeVariationGenerator : BaseVariationGenerator, IVariationGenerator, ISupportInitialize 
	{
        /// <summary>
        /// 
        /// </summary>
        public TypeVariationGenerator()
        {
            DefaultTestContract.Description = "Generate VariationItems with all the types specified on the TypeName property.";

            StorageItem si = new StorageItem();
            si.Name = "Type";

            TypeDesc typeDesc = new TypeDesc();
            si.Type = typeDesc;
            typeDesc.TypeName = "System.Type";
            typeDesc.AssemblyName = "mscorlib.dll";

            DefaultTestContract.Output.Add(si);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<VariationItem> Generate()
        {
            ProcessDefults();

            List<VariationItem> viList = new List<VariationItem>();
            Type baseType = null;

            baseType = TypeToSearch.GetCurrentType();

            if (baseType == null)
            {
                throw new InvalidOperationException("The type: " + TypeToSearch.ToString() + " cannot be found.");
            }

            foreach (AssemblyDesc assemblyDesc in _assembliesToSearch)
            {
                Assembly assembly = assemblyDesc.GetAssembly();
                string assemblyName = assembly.GetName().FullName;
                Type[] allTypes = assembly.GetTypes();

                foreach (Type currentType in allTypes)
                {
                    if (IsMatchingType(baseType, currentType))
                    {
                        TypeVariationItem tvi = new TypeVariationItem();
                        tvi.TypeName = new TypeDesc();
                        tvi.TypeName.UpdateInfo(currentType);
                        tvi.Creator = this.GetType().Name;
                        tvi.Title = currentType.Name;
                        tvi.Merge(DefaultTestContract);
                        tvi.Merge(this);
                        viList.Add(tvi);                       
                    }
                }
            }

            return viList;
        }

        /// <summary>
        /// 
        /// </summary>
        public AssemblyDescCollection AssembliesToSearch
        {
            get { return _assembliesToSearch; }            
        }
       
        /// <summary>
        /// 
        /// </summary>
        public TypeDesc TypeToSearch
        {
            get { return _typeToSearch; }
            set { _typeToSearch = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IncludeWPF3InSearch
        {
            get { return _includeWPF; }
            set { _includeWPF = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool IncludeSubClasses
        {
            get { return _includeSubClasses = true; }
            set { _includeSubClasses = value; }
        }

        /// <summary>
        /// The method in this property will be call
        /// </summary>
        [DefaultValue(null)]
        public MethodDesc FilterMethod
        {
            get { return _filterMethod; }
            set
            {
                if (value != null)
                {
                    value.ValidateState();
                }

                _filterMethod = value;
            }
        }


        private bool IsMatchingType(Type baseType, Type currentType)
        {
            Type[] emptyArray = new Type[0];

            bool isMatching =
                (currentType.IsValueType || currentType.GetConstructor(emptyArray) != null) // Value types always have empty ctor.
                && currentType.IsPublic
                && !currentType.IsAbstract
                && !currentType.IsGenericType
                && !currentType.IsEnum
                && (currentType == baseType || currentType.IsSubclassOf(baseType));


            // Give a chance for a user to filter during design time (or 
            // trxo generation).

            if (isMatching && FilterMethod != null)
            {
                object cache = null;
                Delegate callback = FilterMethod.GetDelegate(typeof(TypeFilterEventHandler), ref cache, true);

                if (callback != null)
                {
                    isMatching = (bool)callback.DynamicInvoke(currentType);
                }
            }
           
            return isMatching;
        }


        #region ISupportInitialize Members

        void ISupportInitialize.BeginInit() {}

        void ISupportInitialize.EndInit()
        {
            if (!_endInit)
            {
                _endInit = true;

                ProcessDefults();
            }
        }

        #endregion



        private void ProcessDefults()
        {
            if (!_processedDefaults)
            {
                _processedDefaults = true;

                if (IncludeWPF3InSearch)
                {
                    foreach (Assembly assembly in WPF3Helper.Assemblies)
                    {
                        AssemblyDesc assemblyDesc = new AssemblyDesc();
                        assemblyDesc.AssemblyName = assembly.GetName().FullName;
                        _assembliesToSearch.Add(assemblyDesc);
                    }
                }
            }
        }


        private MethodDesc _filterMethod = null;
        private TypeDesc _typeToSearch = null;
        private bool _includeWPF = true;
        private bool _includeSubClasses = true;        
        private AssemblyDescCollection _assembliesToSearch = new AssemblyDescCollection();
        bool _endInit = false;
        bool _processedDefaults = false;
    }
}
