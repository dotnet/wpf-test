// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;				
using System.Reflection;		    //MethodInfo
using System.Collections;	        //Hashtable
using System.Collections.Generic;	//List<T>

namespace Microsoft.Test.KoKoMo
{
	/// <summary>enum Model Action Flags</summary>
	public enum ModelActionFlags	//: ModelItemFlags - would be nice to inherit
	{
        /// <summary>Call First</summary>
		CallFirst	= 0x00000010,
        /// <summary>Call Last</summary>
		CallLast	= 0x00000020,
        /// <summary>Call Before</summary>
		CallBefore	= 0x00000040,
        /// <summary>Call After</summary>
		CallAfter	= 0x00000080,
	}

	/// <summary>
    /// Model Action Attribute class
    /// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
	public class ModelActionAttribute : ModelItemAttribute
	{
		int	_callLimit	= Int32.MaxValue;	//Default (unlimited)

        /// <summary>Constructor</summary>
		public ModelActionAttribute()
		{
		}
				
        /// <summary>Call limit </summary>
		public int							CallLimit
		{
			get { return _callLimit;									}
			set { _callLimit = value;									}
		}

        /// <summary>CallO nce</summary>
		public bool							CallOnce
		{
			//Delegate (helper over CallLimit)
			get { return _callLimit == 1;								}
			set { _callLimit = (value ? 1 : Int32.MaxValue);			}
		}

        /// <summary>Call First </summary>
		public bool							CallFirst
		{
			get { return base.IsFlag((int)ModelActionFlags.CallFirst);	}
			set { SetFlag((int)ModelActionFlags.CallFirst, value);		}
		}

        /// <summary>CallLast</summary>
		public bool							CallLast
		{
			get { return base.IsFlag((int)ModelActionFlags.CallLast);	}
			set { SetFlag((int)ModelActionFlags.CallLast, value);		}
		}

        /// <summary>Call Before</summary>
		public bool							CallBefore
		{
			get { return base.IsFlag((int)ModelActionFlags.CallBefore);	}
			set { SetFlag((int)ModelActionFlags.CallBefore, value);		}
		}

        /// <summary>Call After</summary>
		public bool							CallAfter
		{
			get { return base.IsFlag((int)ModelActionFlags.CallAfter);	}
			set { SetFlag((int)ModelActionFlags.CallAfter, value);		}
		}
	}

	/// <summary>
    /// Class of Model Action
    /// </summary>
	public class ModelAction : ModelItem
	{
		//Data
		MethodInfo			_method;
		ModelRequirements	_requirements;
		ModelParameters		_parameters;
		int					_callLimit		= Int32.MaxValue;	//Default (unlimited)

		/// <summary>
        /// Constructor
		/// </summary>
		/// <param name="model"></param>
		/// <param name="method"></param>
		/// <param name="attr"></param>
		public ModelAction(Model model, MethodInfo method, ModelActionAttribute attr)
			: base(model, attr)
		{
			_method = method;
			if(attr!= null)
				_callLimit = attr.CallLimit;
		}

        /// <summary>Name Property</summary>
		public override	string				Name
		{
			get { return _method.Name;					}
		}

        /// <summary>Weight</summary>
		public override int					Weight
		{
			get { return this.Model.Weight * _weight;	}
		}
        /// <summary>Disabled</summary>
		public override bool				Disabled
		{
			get { return this.Model.Disabled || base.Disabled;			}
			set { base.Disabled = value;								}
		}

        /// <summary>CallLimit</summary>
		public int							CallLimit
		{
			get { return _callLimit;									}
			set { _callLimit = value;									}
		}

        /// <summary>CallOnce</summary>
		public bool							CallOnce
		{
			//Delegate (helper over CallLimit)
			get { return _callLimit == 1;								}
			set { _callLimit = (value ? 1 : Int32.MaxValue);			}
		}

        /// <summary>CallFirst</summary>
		public bool							CallFirst
		{
			get { return base.IsFlag((int)ModelActionFlags.CallFirst);	}
			set { SetFlag((int)ModelActionFlags.CallFirst, value);		}
		}

        /// <summary>Call Last</summary>
		public bool							CallLast
		{
			get { return base.IsFlag((int)ModelActionFlags.CallLast);	}
			set { SetFlag((int)ModelActionFlags.CallLast, value);		}
		}

        /// <summary>Call Before</summary>
		public bool							CallBefore
		{
			get { return base.IsFlag((int)ModelActionFlags.CallBefore);	}
			set { SetFlag((int)ModelActionFlags.CallBefore, value);		}
		}
        /// <summary>Call After</summary>
		public bool							CallAfter
		{
			get { return base.IsFlag((int)ModelActionFlags.CallAfter);	}
			set { SetFlag((int)ModelActionFlags.CallAfter, value);		}
		}

        /// <summary>
        /// Execute method
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
		public virtual	object				Execute(ModelParameters parameters)
		{
			object output = null;
			this.Accessed++;

			try
			{
				//Arguments
				object[] args = parameters.Values.ToArray();

				//Invoke the method (with arguments)
				output = _method.Invoke(this.Model, args);
			}
			catch(Exception e)
			{
				//Since were using reflection, if an error occurs within the method
				//make this easier to debug (for the method writer) so they see their exception
				//instead of the reflection based one.
				if(e.InnerException != null && e is TargetInvocationException)
					e = e.InnerException;

				//Find what should have thrown the exception, action or parameters
				Type exception		= this.Exception;
				string exceptionid	= this.ExceptionId;
				if(exception == null && exceptionid == null)
				{
					//Otherwise maybe one of the parameters was supposed to throw
					//Find the parameter that expects an error
					ModelParameters found = (ModelParameters)parameters.FindFlag((int)ModelItemFlags.Throws);
					if(found.Count > 0)
					{
						//Note: We find the 'highest' weighted parameter (ie: order of errors processed)
						found.SortByWeightDesc();					
						exception	= found.First.Exception;
						exceptionid = found.First.ExceptionId;
					}
				}

				//Exception not expected
				if(exception == null && exceptionid == null)
					throw new ModelException(this, "Threw an exception: " + e.Message,  e);

				//Expected: Simple verification, type based
				if(exception != null && (exception != e.GetType()))
					throw new ModelException(this, "Threw the wrong exception: " + e.Message,  e);

				//Expected: Advanced verification, user implemented function
				if(exceptionid != null)
					this.Model.VerifyException(this, parameters, e, exceptionid);	//Throws if not verified

				//Otherwise, it was verified correctly
				output = "Exception Verified";
				return output;
			}
			
			//Should it have thrown, and it didn't?
			if(this.Throws)	
				throw new ModelException(this, "Requirements were not met, and was expected to throw Exception: '" + this.Exception + "'" + " ID: '" + this.ExceptionId + "''");
			return output;
		}

        /// <summary>Return the requirments </summary>
		public virtual ModelRequirements	Requirements
		{
			get
			{
				//Dynamically find all actions (of this model)
				//Note: Override if you had a different way to determine actions
				if(_requirements == null)
				{
					//Find all [ModelRequirement] attributes
					_requirements = new ModelRequirements();
					foreach(ModelRequirementAttribute attr in _method.GetCustomAttributes(typeof(ModelRequirementAttribute), false/*inhert*/))
					{
						if(attr._values.Count == 0)
							throw new ModelException(this, "Missing requirement values.");
				
						//Loop through the values
						foreach(ModelValue value in attr._values)
						{
							//Find the assoicated model variable, by name or type if not specified
							ModelVariables found = null;

							//Note: We only look within the current model for the variable
							//Looking across other models would be error prone, since we don't necessarily
							//know how thie models are related (ie: if a variable on another model 
							//applies, just becuase it has the same name/type).

							//Find by name
							if(attr.Variable != null)
							{
								found = this.Model.Variables.Find(attr.Variable);
                                if(found.Count == 0)
                                {
                                    //Is is a dynamic variable (ie: property/field/method)
                                    ModelRequirement requirement = new ModelRequirement(this, attr, null, value);
                                    found.Add(requirement.Variable);
                                }
                                if(found.Count == 0)
                                    throw new ModelException(this, "Unable to find variable: '" + attr.Variable + "'");
                            }
							else
							{
								//Find by Type
								found = this.Model.Variables.FindType(value.Type);
                                if(found.Count == 0)
                                    throw new ModelException(this, "Unable to find variable matching type: '" + value.Type.ToString() + "'");
                            }

                            //Note: Create a seperate ModelRequirement class for each modelvariable
							//This keeps it simple, and is only an issue with OR clauses
							if(found.Count == 1)
							{
                                ModelVariable variable = found.First;
                                ModelRequirement requirement = _requirements.Find(variable);
								if (requirement != null && (requirement.Throws == attr.Throws))
								{
									//Add the value
									requirement.Values.Add(value);
									requirement.InferDynamicVariables(); //For the new value
								}
								else
								{
									//Add the requirement
									requirement = new ModelRequirement(this, attr, variable, value);
									_requirements.Add(requirement);
								}
							}
							else
							{
								throw new ModelException(this, "Ambigous variable " + (attr.Variable != null ? ("'" + attr.Variable + "' ") : "") + "multiple ModelVariables exists with type: '" + value.Type + "', you will need to explicitly specify the variable and/or model your refering to in the requirement.  The recommended design is to use unique enumerations for model variables instead.");
							}
						}
					}

					//Sort
					_requirements.SortByWeightDesc();
				}
				return _requirements;
			}
		}

        /// <summary>Return Parameters</summary>
		public virtual ModelParameters		Parameters
		{
			get
			{
				//Dynamically find all actions (of this model)
				//Note: Override if you had a different way to determine actions
				if(_parameters == null)
				{
					_parameters = new ModelParameters();

					//Obtain the method parameter info
					ParameterInfo[] infos = _method.GetParameters();

					//Find all [ModelParameter] attributes
					foreach(ModelParameterAttribute attr in _method.GetCustomAttributes(typeof(ModelParameterAttribute), false/*inhert*/))
					{
						//Find the assoicated ParameterInfo (from the method)
						ParameterInfo info = null;

						//Find by index, if specified
						if(attr.Position >= 0)
						{
							if(attr.Position >= infos.Length)
								throw new ModelException(this, "Parameter position '" + attr.Position + "' is greater than the number of actual method parameters");
							info = infos[attr.Position];
						}

						//Find by name, if specified
						if(attr.Name != null)	
						{
							foreach(ParameterInfo p in infos)
							{
								if(String.Compare(p.Name, attr.Name, true/*ignore case*/)==0)
								{
									info = p;
									break;
								}
							}
							if(info == null)
								throw new ModelException(this, "Unable to find method parameter: " + attr.Name);
						}

						//Still not specified
						if(info == null)
						{
							if(infos.Length == 1)		//If only one parameter, no need to specify, no ambiguity
								info = infos[0];
							else if(infos.Length == 0)	//No parameters, but params specified
								throw new ModelException(this, "MethodParameter attribute is specified, but this action doesn't take parameters?");
							else						//More than one parameter, ambigous
								throw new ModelException(this, "When there are numerous method parameters, ModelParameter.Position or .Name is required in order to distinguish.");
						}

						//Add the parameter
						ModelParameter parameter = new ModelParameter(this, attr, info);
						_parameters.Add(parameter);
					}

					//Loop over the physical parameters
					//	1.  Looking for additional attributes
					//  2.  Ensuring we have a [parameter] specified for every actual parameter
					foreach(ParameterInfo info in infos)
					{
						//Add the parameters attributes, if any are specified on the actual parameter
						foreach(ModelParameterAttribute attr in info.GetCustomAttributes(typeof(ModelParameterAttribute), false/*inhert*/))
							_parameters.Add(new ModelParameter(this, attr, info));

						//Ensure all parameters have attributes
                        if (_parameters.Find(info.Name).Count <= 0)
                                throw new ModelException(this, "Unable to find a ModelParameter for parameter: '" + info.Name + "'");
					}

					//Sort
					_parameters.SortByWeightDesc();
				}
				return _parameters;
			}
		}

        /// <summary>Return Method info</summary>
		public virtual MethodInfo			Method
		{
			get { return _method;		}
		}

        /// <summary>
        /// Reload method
        /// </summary>
		public override void				Reload()
		{
			//Clear, so they will be dynamically setup by reflection again
			_requirements	= null;
			_parameters		= null;
		}

        /// <summary>
        /// Clone method
        /// </summary>
        /// <returns></returns>
		public override object				Clone()
		{
			ModelAction clone = (ModelAction)this.MemberwiseClone();

			//Clone the collections, so add/remove is independent
			clone._requirements	= (ModelRequirements)this.Requirements.Clone();
			clone._parameters	= (ModelParameters)this.Parameters.Clone();
			return clone;
		}
	}

    ///<summary>This class represents a collection of ModelAction</summary>
    public class ModelActions : ModelItems<ModelAction>
    {
        //Constructors
        ///<summary>Default Constructor</summary>
        public ModelActions()
        {
        }

        ///<summary>Constructor with multiple actions</summary>
        public ModelActions(params ModelAction[] actions)
            : base(actions)
        {
        }

        ///<summary>Return a collection of all parameters defined in all the actions in this collection</summary>
        public virtual ModelParameters      Parameters
        {
            get
            {
                //Helper to return the flat list of all variables
                ModelParameters found = new ModelParameters();
                foreach(ModelAction action in this)
                    found.Add(action.Parameters);
                return found;
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public new virtual ModelActions     Find(params string[] names)
        {
            return (ModelActions)base.Find(names);
        }

        /// <summary>
        /// FindExcept
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public new virtual ModelActions     FindExcept(params string[] names)
        {
            return (ModelActions)base.FindExcept(names);
        }

        ///<summary>Find all actions that match the give flag</summary>
        public virtual ModelActions         FindFlag(ModelActionFlags flag)
        {
            //Delegate
            return this.FindFlag(flag, true);
        }

        ///<summary>Find all actions that dont match the given flag</summary>
        public virtual ModelActions         FindFlag(ModelActionFlags flag, bool include)
        {
            //Delegate
            return (ModelActions)base.FindFlag((int)flag, include);
        }
    }

    /// <summary>
    /// Class ModelActionInfo
    /// </summary>
	public class ModelActionInfo
	{
		//Data
		ModelAction		_action;
		ModelParameters	_parameters;
		object			_retval;
		bool			_created;
			
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="action"></param>
		/// <param name="parameters"></param>
		/// <param name="retval"></param>
		public ModelActionInfo(ModelAction action, ModelParameters parameters, object retval )
		{
			_action		= action;
			_parameters	= parameters;
			_retval		= retval;
		}

		/// <summary>Get and set Action</summary>
		public ModelAction		Action
		{
			get {return _action;			}
			set {_action = value;			}
		}

		/// <summary>Get and Set Parameters</summary>
		public ModelParameters Parameters
		{
			get { return _parameters;		}
			set { _parameters = value;		}
		}
        /// <summary>Get and SEt RetVal </summary>
		public object			RetVal
		{
			get { return _retval;			}
			set { _retval = value;			}
		}

        /// <summary>Get and Set Created</summary>
		public bool				Created
		{
			get { return _created;			}
			set { _created = value;			}
		}
	}

	/// <summary>
    /// Class ModelActionInfos
    /// </summary>
	public class ModelActionInfos : List<ModelActionInfo>
	{
	}
}