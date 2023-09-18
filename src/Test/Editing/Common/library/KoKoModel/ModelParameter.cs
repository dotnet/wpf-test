// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;				
using System.Reflection;		//Reflection


namespace Microsoft.Test.KoKoMo
{
    /// <summary>
    /// ModelParameter (attribute)
    /// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple=true)]
	public class ModelParameterAttribute : ModelRangeAttribute
	{
		//Data
		int			_position	= -1;		//Not specified

        /// <summary>
        /// Constructor
        /// </summary>
		public ModelParameterAttribute()
		{
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="values"></param>
		public ModelParameterAttribute(params object[] values)
			: base(values)
		{
		}

		/// <summary>Get and set Position</summary>
		public int			Position
		{
			get { return _position;		}
			set { _position = value;	}
		}
	}

    /// <summary>
    /// ModelParameter
    /// </summary>
    public class ModelParameter : ModelRange
	{
		//Data
		ModelAction			_action;
		ParameterInfo		_paraminfo;
		
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="attr"></param>
        /// <param name="paraminfo"></param>
		public ModelParameter(ModelAction action, ModelParameterAttribute attr, ParameterInfo paraminfo)
			: base(action != null ? action.Model : null, attr)
		{
			_action		= action;
			_paraminfo	= paraminfo;

			//Infer values from the type, if not specified
			if(attr != null && attr.Type != null)
				this.AddValuesFromType(attr, null);

			//BitMask
			if(this.BitMask)
				this.AddBitCombinations();

			//Infer dynamic variable(s)
			this.InferDynamicVariables();
		}

        /// <summary>Get Action</summary>
		public virtual ModelAction			Action
		{
			get { return _action;				}
		}

        /// <summary>Get Position</summary>
		public virtual	int					Position
		{
			get { return _paraminfo.Position;						}
		}

        /// <summary>Get type</summary>
		public override Type				Type
		{
			get { return _paraminfo.ParameterType;					}
		}

        /// <summary>Get Name</summary>
		public override	string				Name
		{
			get { return _paraminfo.Name;							}
		}

        /// <summary>Get IsOptional</summary>
		public virtual bool                 IsOptional
		{
            get { return Attribute.IsDefined(_paraminfo, typeof(ParamArrayAttribute)); }
		}

        /// <summary>Get FullName</summary>
		public override string				FullName
		{
			get 
			{ 
				if(_fullname == null && _action != null)
				{
					_fullname = _action.FullName + "." + this.Name;
					return _fullname;
				}

				//Otherwise
				return this.Name;
			}
		}
	}

    ///<summary>This class represents a collection of model parameters, this is used by the action class to store its parameters</summary>
    public class ModelParameters : ModelRanges<ModelParameter>
    {
        ///<summary>
        ///Default constructor
        ///</summary>
        public ModelParameters()
        {
        }

        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public new virtual ModelParameters  Find(params string[] names)
        {
            return (ModelParameters)base.Find(names);
        }

        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public new virtual ModelParameters  FindExcept(params string[] names)
        {
            return (ModelParameters)base.FindExcept(names);
        }

        ///<summary>Return a collection of all the values of all the parameters in this collection</summary>
        public virtual ModelValues          Values
        {
            get
            {
                ModelValues values = new ModelValues();
                foreach (ModelRange range in this)
                    values.Add(range.Values);

                return values;
            }
        }

        ///<summary>A string concatenated version of the parameters to output as method signature</summary>
        public override string              ToString()
        {
            string output = null;
            foreach (ModelParameter parameter in this)
            {
                if (output != null)
                    output += ", ";
                output += parameter.Value;
            }

            return output;
        }
    }
}
