// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;				


namespace Microsoft.Test.KoKoMo
{
    /// <summary>
    /// ModelRequirement (attribute)
    /// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
	public class ModelRequirementAttribute : ModelRangeAttribute
	{
        /// <summary>
        /// Constructor 
        /// </summary>
		public ModelRequirementAttribute()
		{
		}
		
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="values"></param>
        public ModelRequirementAttribute(params object[] values)
			: base(values)
		{
		}
	}


    /// <summary>
    /// ModelRequirement
    /// </summary>
	public class ModelRequirement : ModelRange
	{
		ModelAction			_action;
		bool				_global = true;	//global requirement (ie: any)		

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public ModelRequirement(ModelVariable variable, Object value)
			: this(variable, new ModelValue(value))
		{
			//Delegate
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public ModelRequirement(ModelVariable variable, ModelValue value)
			: this(null, null, variable, value)
		{
			//Delegate
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="variable"></param>
        /// <param name="value"></param>
		public ModelRequirement(ModelAction action, ModelVariable variable, ModelValue value)
			: this(action, null, variable, value)
		{
			//Delegate
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="attr"></param>
        /// <param name="variable"></param>
        /// <param name="value"></param>
		public ModelRequirement(ModelAction action, ModelRequirementAttribute attr, ModelVariable variable, ModelValue value)
			: base(action != null ? action.Model : null, attr)
		{
			//Action
			_action = action;

			//Variable
			if (variable != null)
			{
				_variable = variable;
				if (variable.BitMask)
					this.BitMask = true;
			}
			//if(_variable == null)
			//    throw new ModelException(this, "An empty variable is not a valid requirement", null);

			//Only override the attr, if values are really specified
			if(value != null)
				_values = new ModelValues(value);

			//BitMask
			if(this.BitMask)
				this.AddBitCombinations();

			//Infer dynamic variable(s)
			this.InferDynamicVariables();

			//Requirements assoicated with actions, are not global.  They are tied to that particular
			//instance of the model, and it's instance of state variables.  However if not assoicated
			//with actions, the default behavior is that their global, they apply to all models
			//that contain that state variable, unless explicitly indicated otherwise.
			_global = (action == null);
		}

        /// <summary>Get Name</summary>
		public override	string					Name
		{
			get { return _variable != null ? _variable.Name : null;		}
		}

        /// <summary>Get Action</summary>
		public virtual ModelAction				Action
		{
			get { return _action;				}
		}
        /// <summary>Get Type</summary>
		public override Type					Type
		{
			get 
			{ 
				if(_variable != null)
					return _variable.Type;	
				return null;
			}
		}

        /// <summary>Get Global</summary>
		public virtual bool						Global
		{
			get { return _global;				}
			set { _global = value;				}
		}
	}

    ///<summary>
    ///This class represents a collection of requirements, this is used by the ModelAction class.
    ///</summary>
    public class ModelRequirements : ModelRanges<ModelRequirement>
    {
        ///<summary>Default constructor</summary>
        public ModelRequirements()
        {
        }

        ///<summary>Overload that takes an array of requirements</summary>
        public ModelRequirements(params ModelRequirement[] requirements)
            : base(requirements)
        {
        }

        ///<summary>Add a requirement to this collection from the variable and the value specified</summary>
        public virtual ModelRequirement     Add(ModelVariable variable, object value)
        {
            return this.Add(new ModelRequirement(variable, new ModelValue(value)));
        }

        ///<summary>Find a requirement from the variable specified</summary>
        public virtual ModelRequirement     Find(ModelVariable variable)
        {
            //Find a matching variable, of the specified type
            foreach (ModelRequirement requirement in this)
            {
                if (requirement.Variable == variable)
                    return requirement;
            }

            //Otherwise
            return null;
        }
    }
}
