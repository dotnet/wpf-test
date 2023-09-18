// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;				
using System.Reflection;		    //BindingFlags
using System.Collections.Generic;	//List<T>

namespace Microsoft.Test.KoKoMo
{

	/// <summary>
    /// Model (attribute)
	/// </summary>
    [AttributeUsage(AttributeTargets.Class)]
	public class ModelAttribute : ModelItemAttribute
	{
		/// <summary>
		/// Max instane variable.
		/// </summary>
		protected int	_maxinstances   = Int32.MaxValue;	//Default (unlimited)

		/// <summary>
		/// Constructor of the model Attribute
		/// </summary>
		public ModelAttribute()
		{
		}
				
		/// <summary>
		/// Max Instances.
		/// </summary>
		public virtual int	                MaxInstances
		{
			get { return _maxinstances;		        }
			set { _maxinstances = value;		    }
		}
	}

    /// <summary>
    /// CallBeforeHandler Delegates
    /// </summary>
    /// <param name="action"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public delegate bool CallBeforeHandler(ModelAction action, ModelParameters parameters);

    /// <summary>
    /// CallAfterHandler delegates
    /// </summary>
    /// <param name="action">a model action</param>
    /// <param name="parameters">a Model prameters</param>
    /// <param name="result">result</param>
	public delegate void CallAfterHandler(ModelAction action, ModelParameters parameters, object result);

    /// <summary>
    /// The abstract class Model
    /// </summary>
	public abstract class Model : ModelItem
	{
		//Data
		ModelActions		    _actions;
		ModelVariables		    _variables;
		ModelEngine			    _engine;
        bool                    _isExecuting    = true;
        List<CallBeforeHandler> _callbefore     = new List<CallBeforeHandler>();
        List<CallAfterHandler>  _callafter      = new List<CallAfterHandler>();
        Model                   _parentmodel    = null;
        int                     _maxinstances   = Int32.MaxValue;	//Default (unlimited)

        /// <summary>
        /// Constructor
        /// </summary>
		public Model()
			: base(null, null)
		{
			//Obtain the attribute.
			//Note: We don't want to require a constructor that takes the attribute, since this
			//is an inherited class by the user, we'll simply obtain the attribute for default info.
			foreach(ModelAttribute attr in this.GetType().GetCustomAttributes(typeof(ModelAttribute), false/*inherit*/))
				this.SetAttributeValues(attr);

			//By default, constructing the object, it becomes enabled.
			//This is so, you can automatically construct (disabled) models, and add them to the system
			//However if called in the context of the [attr] caller, it will get reset to the attribute
			this.Disabled = false;
		}

        /// <summary>
        /// initialization
        /// </summary>
		public virtual	void				Init()
		{
			//Override, if you have code that should be executed first (before any methods are called)
		}

        /// <summary>
        /// Terminate of the model
        /// </summary>
		public virtual	void				Terminate()
		{
		}

        /// <summary>
        /// Set the attribute values.
        /// </summary>
        /// <param name="attr"></param>
        public override void                SetAttributeValues(ModelItemAttribute attr)
        {
            base.SetAttributeValues(attr);
            if(attr is ModelAttribute)
                _maxinstances = ((ModelAttribute)attr).MaxInstances;
        }
        /// <summary>
        /// The Engine that create the model
        /// </summary>
		public virtual	ModelEngine			Engine
		{
			get { return _engine;			}
			set { _engine = value;			}
		}

		//Accessors

        /// <summary>
        /// Indicates if the model is executing IUT code or just exploring the state space.
        /// </summary>
        public virtual bool                 IsExecuting
        {
            get { return _isExecuting;          }
            set { _isExecuting = value;         }
        }
        /// <summary>
        /// Max value
        /// </summary>
        public virtual int                  MaxInstances
        {
            get { return _maxinstances;         }
            set { _maxinstances = value;        }
        }

        /// <summary>
        /// Actions. 
        /// </summary>
        public virtual	ModelActions		Actions
		{
			get
			{
				//Dynamically find all actions (of this model)
				//Note: Override if you had a different way to determine actions
				if(_actions == null)
				{
					_actions = new ModelActions();

					//Looking for *ALL* methods and properties
					BindingFlags bindingflags = BindingFlags.Public | BindingFlags.NonPublic | 
												BindingFlags.Instance | BindingFlags.Static | 
												BindingFlags.GetProperty | BindingFlags.SetProperty | 
												BindingFlags.InvokeMethod;

					//Loop over them
					foreach(MethodInfo info in this.GetType().GetMethods(bindingflags))
					{
						foreach(ModelActionAttribute attr in info.GetCustomAttributes(typeof(ModelActionAttribute), false/*inhert*/))
						{
							_actions.Add(new ModelAction(this, info, attr));
						}
					}

					//Sort
					_actions.SortByWeightDesc();
				}			
				return _actions;
			}
		}

        /// <summary>
        /// Variables of the model
        /// </summary>
		public virtual	ModelVariables		Variables
		{
			get
			{
				//Dynamically find all model variables
				//Note: Override if you had a different way to determine variables
				if(_variables == null)
				{
					//Find all [ModelAction] attributes
					_variables = new ModelVariables();

					//Looking for *ALL* fields and properties
					BindingFlags bindingflags = BindingFlags.Public | BindingFlags.NonPublic | 
												BindingFlags.Instance | BindingFlags.Static | 
												BindingFlags.GetField | BindingFlags.GetProperty;

					//Loop over them
					foreach(MemberInfo info in this.GetType().GetMembers(bindingflags))
					{
						foreach(ModelVariableAttribute attr in info.GetCustomAttributes(typeof(ModelVariableAttribute), false/*inhert*/))
						{
							_variables.Add(new ModelVariable(this, info, attr));
						}
					}

					//Sort
					_variables.SortByWeightDesc();
				}			
				return _variables;
			}
		}

        /// <summary>
        /// Call Before event
        /// </summary>
		public event CallBeforeHandler      CallBefore
		{
			add { _callbefore.Add(value);               }
			remove { _callbefore.Remove(value);         }
		}

        /// <summary>
        /// Call After event
        /// </summary>
		public event CallAfterHandler       CallAfter
		{
			add { _callafter.Add(value);                }
			remove { _callafter.Remove(value);          }
		}

        /// <summary>
        /// On CallBefore 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
		public virtual bool				    OnCallBefore(ModelAction action, ModelParameters parameters)
		{
			//override if you want to do something, or call other methods BEFORE execution of the action
			bool ret = true; //true, indicates continue to call the action
			foreach(CallBeforeHandler h in _callbefore)
				ret &= h(action, parameters);	

			return ret;	
		}

        /// <summary>
        /// On CAllAfter
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
		public virtual void				    OnCallAfter(ModelAction action, ModelParameters parameters, object result)
		{
            //override if you want to do something, or call other methods AFTER execution of the action
            foreach(CallAfterHandler h in _callafter)
                h(action, parameters, result);
		}

        /// <summary>
        /// re-throw the exception.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <param name="e"></param>
        /// <param name="id"></param>
		public virtual void				    VerifyException(ModelAction action, ModelParameters parameters, Exception e, string id)
		{
			//Override this method, and verify the ExceptionId specified in the model
			throw new ModelException(this, "ExceptionId was specified and not verified.  Override Model.VerifyException, and verify the ExceptionId as was specified in the model", e);
		}

        /// <summary>
        /// Reload
        /// </summary>
		public override void			    Reload()
		{
			//Clear, so they will be dynamically setup by reflection again
			_actions	= null;
			_variables	= null;
		}

        /// <summary>
        /// Clone the model
        /// </summary>
        /// <returns></returns>
		public override object			    Clone()
		{
			Model clone = (Model)this.MemberwiseClone();

			//Clone the collections, so add/remove is independent
			clone._actions	= (ModelActions)this.Actions.Clone();
			clone._variables= (ModelVariables)this.Variables.Clone();
			return clone;
		}

        /// <summary>
        /// Parent model
        /// </summary>
        public virtual Model                Parent
        {
            get { return _parentmodel; }
            set { _parentmodel = value; }
        }

        /// <summary>
        /// Children model
        /// </summary>
        public virtual Models               Children
        {
            get
            {
                //We have to recompute this everytime since 
                //new children could have been added.
                //Can change this later.
                Models found = new Models(this.Engine);
                if(this.Engine != null)
                {
                    foreach(Model model in this.Engine.Models)
                    {
                        if(model.Parent == this)
                            found.Add(model);                        
                    }
            }
                return found;
            }
        }
	}


    /// <summary>
    /// Models
    /// </summary>
    public class Models : ModelItems<Model>
    {
        //Data
        ModelEngine _engine;

        //Constructors

        ///<summary>Constructor</summary>
        ///<param name="engine">The engine context for this collection.</param>
        public Models(ModelEngine engine)
        {
            _engine = engine;
        }

        ///<summary>Type Indexer</summary>
        public virtual Model        this[Type type]
        {
            get
            {
                //Find first, fail if not found, although make it easier to debug
                Models found = this.FindType(type);
                if (found == null || found.Count <= 0)
                    throw new IndexOutOfRangeException(this.Name + "['" + type + "'] is not found");
                return found.First;
            }
        }

        ///<summary>Find models of type "type"</summary>
        public virtual Models       FindType(Type type)
        {
            //Find a matching variable, of the specified type
            Models found = new Models(_engine);
            foreach (Model model in this)
            {
                if (model.GetType() == type || model.GetType().IsSubclassOf(type))
                    found.Add(model);
            }

            //Otherwise
            return found;
        }

        ///<summary>Add a Model to collection</summary>
        public override Model       Add(Model model)
        {
            base.Add(model);
            model.Engine = _engine;
            return model;
        }

        ///<summary>Given the assembly or the calling object, find models in the assembly and add them to the collection</summary>
        public virtual void         AddFromAssembly(object caller)
        {
            //Delegate
            this.AddFromAssembly(Assembly.GetAssembly(caller.GetType()));
        }

        ///<summary>Add the models found in the specified assembly</summary>
        public virtual void         AddFromAssembly(Assembly assembly)
        {
            //Find all [Model] attributes
            foreach (Type type in assembly.GetTypes())
            {
                //Models can be inherited from common bases
                if (type.IsAbstract)
                    continue;

                //Loop over all attributes
                foreach (ModelAttribute attr in type.GetCustomAttributes(typeof(ModelAttribute), false))
                {
                    //Ensure the class if of type Model
                    //Instead of throwing a hard to debug, constructor error
                    if (!type.IsSubclassOf(typeof(Model)))
                        throw new ModelException(_engine, "[Model] class: '" + type.Name + "' must inherit from Model");

                    //Ensure the class has a default constructor (required)
                    ConstructorInfo ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                null, Type.EmptyTypes, null);
                    if (ctor == null)
                        throw new ModelException(_engine, "[Model] class: '" + type.Name + "' must implement a default constructor");

                    //Construct the model
                    Model model = (Model)ctor.Invoke(null);
                    model.SetAttributeValues(attr);
                    this.Add(model);
                }
            }
        }

        /// <summary>
        /// Set the CallBefore event on all models in the engine.
        /// </summary>
        public virtual event CallBeforeHandler CallBefore
        {
            add { foreach (Model model in this) model.CallBefore += value; }
            remove { foreach (Model model in this) model.CallBefore -= value; }
        }

        /// <summary>
        /// Set the CallAfter event on all models in the engine.
        /// </summary>
        public virtual event CallAfterHandler CallAfter
        {
            add { foreach (Model model in this) model.CallAfter += value; }
            remove { foreach (Model model in this) model.CallAfter -= value; }
        }

        /// <summary>
        /// Initialize all models in the collection.
        /// </summary>
        public virtual void         Init()
        {
            foreach (Model model in this)
                model.Init();
        }

        /// <summary>
        /// Get or Set the 'IsExecuting' property for all models in the collection. This flag allows the 
        /// model developer to indicate which portions of the model should be run only during execution.
        /// </summary>
        public virtual bool         IsExecuting
        {
            get
            {
                if (this.Count == 0)
                    throw new InvalidOperationException("Unable to read aggregate property 'IsExecuting' because the model collection is empty.");

                bool val = this.First.IsExecuting;

                for (int i = 1; i < this.Count; i++)
                {
                    if (val != this[i].IsExecuting)
                        throw new ModelException(_engine, "Unable to read aggregate property 'IsExecuting' because not all models have the same value.");

                    val = this[i].IsExecuting;
                }

                return val;
            }

            //note: less error handling is needed here because "set" of a non-nullable type is better defined
            //than "get" over an empty or varied set of objects.
            set
            {
                foreach (Model model in this)
                    model.IsExecuting = value;
            }
        }

        ///<summary>Return a collection of ModelAction for all the actions that are defined on all models in this collection</summary>
        public virtual ModelActions     Actions
        {
            get
            {
                //Helper to return the flat list of all actions
                ModelActions actions = new ModelActions();
                foreach (Model model in this)
                    actions.Add(model.Actions);
                return actions;
            }
        }

        ///<summary>Return a collection of all variables defined in all the models in this collection</summary>
        public virtual ModelVariables   Variables
        {
            get
            {
                //Helper to return the flat list of all variables
                ModelVariables variables = new ModelVariables();
                foreach (Model model in this)
                    variables.Add(model.Variables);
                return variables;
            }
        }
        /// <summary>
        /// MaxInstances
        /// </summary>
        public virtual int              MaxInstances
        {
            get 
            {
                //Return the smaller of the values
                int maxinstances = Int32.MaxValue;
                foreach(Model model in this)
                    maxinstances = Math.Min(maxinstances, model.MaxInstances);
                return maxinstances;
            }
            set 
            { 
                foreach(Model model in this)
                    model.MaxInstances = value; 
            }
        }
    }
}
