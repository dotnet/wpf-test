// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;				
using System.Reflection;		//MemberInfo


namespace Microsoft.Test.KoKoMo
{

    /// <summary>
    /// ModelVariable (attribute)
    /// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ModelVariableAttribute : ModelRangeAttribute
	{
    }

    /// <summary>
    /// ModelVariable
    /// </summary>
	public class ModelVariable : ModelRange
	{
		//Note: We allow model variables to be simple fields, or properties.  Fields for simplicity
		//in value comparison, and properties for custom complex expressions (ie: any code).
		MemberInfo				_info;
		object					_originalvalue;
        Object                  _instance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="info"></param>
        /// <param name="attr"></param>
		public ModelVariable(Model model, MemberInfo info, ModelVariableAttribute attr)
			: this(model, model, info, attr)
		{
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="instance"></param>
        /// <param name="info"></param>
        /// <param name="attr"></param>
		public ModelVariable(Model model, Object instance, MemberInfo info, ModelVariableAttribute attr)
			: base(model, attr)
		{
			_info			= info;
            _instance       = instance;

			if(!model.Disabled)
				_originalvalue	= this.CurrentValue;

            if(attr != null)
            {
                //Infer values from the type, if not specified
			    if(_values.Count == 0 || attr.Type != null)
				    this.AddValuesFromType(attr, this.Type);
            }
			
			//BitMask
			if(this.BitMask)
				this.AddBitCombinations();

			//Add the current value to the possible value list, if not already present
			ModelValue v = _values.FindValue(_originalvalue);
			if(v == null)
				_values.Add(new ModelValue(_originalvalue));
        }

        /// <summary>Get Name</summary>
		public override	string			Name
		{
			get { return _info.Name;				}
		}

        /// <summary>Get Type</summary>
		public override	Type			Type
		{
			get 
			{	
				if(_info is FieldInfo)
					return ((FieldInfo)_info).FieldType;			//Field
                if(_info is MethodInfo)
                    return ((MethodInfo)_info).ReturnType;			//Method
                return ((PropertyInfo)_info).PropertyType;			//Property
			}
		}

        /// <summary>Get IsCalculated</summary>
        public virtual  bool            IsCalculated
        {
            get
            {
                if (_info is PropertyInfo && !((PropertyInfo)_info).CanWrite)
                    return true;
                if(_info is MethodInfo)
                    return true;

                return false;
            }
        }

        /// <summary>Get CurrentValue</summary>
        public virtual	object			CurrentValue
		{
			get 
			{ 
				try
				{
					if(_info is FieldInfo)
						return ((FieldInfo)_info).GetValue(_instance);			//Field
                    if(_info is MethodInfo)
                        return ((MethodInfo)_info).Invoke(_instance, null);		//Method
                    return ((PropertyInfo)_info).GetValue(_instance, null);	    //Property
				}
				catch(Exception e)
				{
					//Make this easier to debug
					throw new ModelException(this, null, e);
				}
			}
		}

        /// <summary>
        /// Reload method
        /// </summary>
		public override void			Reload()
		{
		}

        /// <summary>
        /// Clone Method
        /// </summary>
        /// <returns></returns>
		public override object			Clone()
		{
			return this.MemberwiseClone();
		}
	}

    ///<summary>This class represents a collection of model variables</summary>
    public class ModelVariables : ModelRanges<ModelVariable>
    {
        ///<summary>Default constructor</summary>
        public ModelVariables()
        {
        }

        /// <summary>
        /// Find method
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public new virtual ModelVariables   Find(params string[] names)
        {
            return (ModelVariables)base.Find(names);
        }

        /// <summary>
        /// FindException method
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public new virtual ModelVariables   FindExcept(params string[] names)
        {
            return (ModelVariables)base.FindExcept(names);
        }

        /// <summary>
        /// This method
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual ModelVariable        this[Type type]
        {
            //Find first, fail if not found, although make it easier to debug
            get
            {
                ModelVariables found = this.FindType(type);
                if (found == null || found.Count <= 0)
                    throw new IndexOutOfRangeException(this.Name + "['" + type + "'] is not found");
                return found.First;
            }
        }

        ///<summary>Finds a model variable in this collection of the specified type</summary>
        public virtual ModelVariables       FindType(Type type)
        {
            //Find a matching variable, of the specified type
            ModelVariables found = new ModelVariables();
            foreach (ModelVariable variable in this)
            {
                if (variable.Type == type)
                    found.Add(variable);
            }

            return found;
        }
    }
}
