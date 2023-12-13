// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;		    //Reflection
using System.Collections;	        //IEnumerable
using System.Collections.Generic;	//List<T>
using System.Threading;
using System.Windows.Threading;			    //Timeout

////////////////////////////////////////////////////////////////
// 'Static' State Variable Design
//		Maintain limited state variables, not explosive states
//		Very little code
//		Easy parameter passing (equivilent classes) 
//		Transitions well to Spec#
//		Finite
//		Known at compile time
//		Able to pre-determine paths (shortest, bee-line, etc)
//		Able to run until all values hit
//		All transitions easily determined
//
//	Example:
//	
//		[Model]
//		class
//		{
//			public enum Opened
//			{
//				No,
//				Yes,
//			}
//
//			[ModelVariable]
//			Opened _opened = Opened.No;
//		
//			[ModelAction(Weight = 10)
//			[ModelRequirement(Opened.No)]
//			void Open()
//			{
//				//perform the operation
//				//verify
//
//				//Behavior
//				_opened = Opened.Yes;
//			}
//		}
//
////////////////////////////////////////////////////////////////

namespace Microsoft.Test.KoKoMo
{
    /// <summary>
    /// Model Engine Flags
    /// </summary>
	public enum ModelEngineFlags
	{
        /// <summary>Include invalid actions </summary>
		InvalidActions = 0x00000001,
        
        /// <summary>Include invalid parameters</summary>
		InvalidParameters = 0x00000002,		
	}

	/// <summary>
	/// The centerpiece of the engine functionality, this class creates the 
	/// engine and user can call methods on this class to execute models.
	/// </summary>
	public class ModelEngine : ModelItem, IDisposable
	{
		/// <summary>Models</summary>
        protected Models                _models         = null;
        
        /// <summary>Caller</summary>
        protected object                _caller         = null;

        /// <summary>Engineoptions</summary>
        protected ModelEngineOptions    _engineoptions  = new ModelEngineOptions();

		//tracks the actions selected by the state machine
		//so it can spit it out in case of a exception or incase we want to get
		//the trace of the problem and use it in a repro.
        /// <summary>Action strace</summary>
		protected ModelActionInfos     _actionstrace	= new ModelActionInfos();

        //Constructor
		/// <summary>
		/// This method constructs the engine given the seed 
		/// and the calling object which is the model of type Model
		/// </summary>
		/// <param name="caller">An instance of type Model</param>
		/// <param name="seed">Seed for random generator</param>
		public ModelEngine(object caller, int seed)
			: base(null, null)
		{
			_engineoptions.Seed	= seed;
			_caller				= caller;
            Model model = caller as Model;
            if (model != null && model.Engine == null)
                model.Engine = this;
		}

		/// <summary>
		/// Constructor which just takes the model class.
		/// </summary>
		/// <param name="caller"></param>
		public ModelEngine(object caller)
			: base(null, null)
		{

			//Set the seed the same as the caller (if called from another model)
			Model model = caller as Model;
			if(model != null && model.Engine != null)
				_engineoptions.Seed = model.Engine.Options.Seed;
			_caller				= caller;
		}

		/// <summary>
		/// Constructor which takes multiple models.
		/// </summary>
		/// <param name="models">object of type Model</param>
		public ModelEngine(params Model[] models)
			: base(null, null)
		{
			_caller				= null;

			//Use passed in models, instead of finding them
			//Saves perf, and the user having to first clear, then add
			_models = new Models(this);
			_models.Add(models);
		}

		//Destructor
		void IDisposable.Dispose()
		{
			foreach(Model model in this.Models)
				model.Terminate();
		}

		//Accessors
		/// <summary>
		/// Property that controls whether InvalidActions be called or not.
		/// </summary>
		public virtual	bool				InvalidActions
		{
			get { return base.IsFlag((int)ModelEngineFlags.InvalidActions);		}
			set { SetFlag((int)ModelEngineFlags.InvalidActions, value);			}
		}

		/// <summary>
		/// Property that controls whether invalid parameter values be generated or not.
		/// </summary>
		public virtual	bool				InvalidParameters
		{
			get { return base.IsFlag((int)ModelEngineFlags.InvalidParameters);		}
			set { SetFlag((int)ModelEngineFlags.InvalidParameters, value);			}
		}

		/// <summary>
		/// List of ActionInfo items of the trace of actions that were executed by the state machine.
		/// </summary>
		public ModelActionInfos            ActionsTrace
		{
			get{ return _actionstrace; }
		}

        /// <summary>Get and set Options</summary>
		public ModelEngineOptions			Options
		{
			get { return _engineoptions; }
			set { _engineoptions = value; }
		}


		private long						DetermineTicks()
		{
			//Infinite
			if ( _engineoptions.Timeout == System.Threading.Timeout.Infinite)
				return long.MaxValue;

			//Update expected ticks
			return TimeSpan.FromSeconds(_engineoptions.Timeout ).Ticks;
		}


		/// <summary>
		/// Returns the calling model
		/// </summary>
		public virtual	object		Caller
		{
			get	{ return _caller;					}
		}

		/// <summary>
		/// Returns all the models in the scope for the engine.
		/// </summary>
		public virtual	Models		Models
		{
			get
			{
				if(_models == null)
				{
					_models = new Models(this);
					_models.AddFromAssembly(_caller);
				}
				return _models;
			}
		}
		
		/// <summary>
		/// This method forces the engine to reload the models.
		/// Useful to reload the weights and other properties.
		/// </summary>
		public override void		Reload()
		{
			//Clear, so they will be dynamically setup by reflection again
			_models		= null;
		}

		/// <summary>
		/// Clone method for engine.
		/// </summary>
		/// <returns></returns>
		public override object		Clone()
		{
			ModelEngine clone = (ModelEngine)this.MemberwiseClone();

			//Clone the collections, so add/remove is independent
			clone._models		= (Models)this.Models.Clone();
			return clone;
		}

		//Methods
        //FIXME: should this be public? It is difficult to extend the behavior of the engine without
        //hosting the extension in the same assembly, unless we do this.
        internal ModelActions		GetPossibleActions()
		{
			//Default - exclude: invalid, callbefore, callafter, calllast, which are handled seperatly.
			ModelActionFlags exclude = ModelActionFlags.CallBefore | ModelActionFlags.CallAfter | ModelActionFlags.CallLast;
			if(!this.InvalidActions)
				exclude |= (ModelActionFlags)ModelItemFlags.Invalid;

			//Loop through all models
			ModelActions totalactions = new ModelActions();
			foreach(Model model in this.Models)
			{
				//Ignore disabled models
				if(model.Weight == 0 || model.Disabled)
					continue;				

				totalactions.Add(model.Actions.FindFlag(exclude, false));
			}

			//Delegate
			return this.GetPossibleActions(totalactions);
		}

        /// <summary>
        /// GetPossibleActions
        /// </summary>
        /// <param name="totalactions"></param>
        /// <returns></returns>
		protected ModelActions		GetPossibleActions(ModelActions totalactions)
		{
			//Loop through all actions, specified
			ModelActions possibleactions = new ModelActions();
			foreach(ModelAction action in totalactions)
			{
				//Ignore Disabled actions
				if(action.Weight == 0 || action.Disabled)
					continue;				

				//Ignore CallLimit/CallOnce actions that have already been called (max times)
				if(action.Accessed >= action.CallLimit)
					continue;

				//Note: CallFirst and ClassLast imply CallOnce
				if(action.Accessed>0 && (action.CallFirst || action.CallLast))
					continue;

				//Ignore Actions, that return Models, when were over the limit of those models
				Type returntype = action.Method.ReturnType;
				if(!returntype.IsPrimitive && typeof(Model).IsAssignableFrom(returntype))
				{
				    Models found = (Models)this.Models.FindType(returntype).FindFlag((int)ModelItemFlags.Disabled, false);
				    if(found.Count >= found.MaxInstances)
				        continue;
				}
				
				//Determine if Requirements meet
				ModelRequirement failedrequirement = null;
				bool matched = MeetsRequirements(action.Requirements, out failedrequirement);
				if(matched)
				{
					//Requirements met, action can be called
					possibleactions.Add(action);
				}
				else
				{
					//Requirements not met, action can't be called
					//Unless the user wants this to be called, when the requirements aren't met
					if(this.InvalidActions && failedrequirement != null && failedrequirement.Throws)
					{
						//Note: We clone the action, and set the expected exception, just as if
						//it were an invalid action from the start.  We also set the weight of the 
						//invalid action, to the weight of the requirement, so it's not weighted
						//the same as the (positive) version that's specified at the action level
						ModelAction invalidaction	= (ModelAction)action.Clone();
						invalidaction.Exception		= failedrequirement.Exception;
						invalidaction.ExceptionId	= failedrequirement.ExceptionId;
						invalidaction.Weight		= failedrequirement.Weight;
						possibleactions.Add(invalidaction);
					}
				}
			}

			possibleactions.SortByWeightDesc();	//Sort all actions, across models
			return possibleactions;
		}
		
        /// <summary>
        /// MeetsRequirements
        /// </summary>
        /// <param name="requirements"></param>
        /// <returns></returns>
		protected bool				MeetsRequirements(ModelRequirements requirements)
		{
			//Delegate
			ModelRequirement failedrequirement = null;
			return this.MeetsRequirements(requirements, out failedrequirement);
		}

        /// <summary>
        /// MeetsRequirements
        /// </summary>
        /// <param name="requirements"></param>
        /// <param name="failedrequirement"></param>
        /// <returns></returns>
		protected bool				MeetsRequirements(ModelRequirements requirements, out ModelRequirement failedrequirement)
		{
			if(requirements != null)
			{
				//Loop over all requirements
				foreach(ModelRequirement requirement in requirements)
				{
					//Ignore disabled requirements
					if(requirement.Weight == 0 || requirement.Disabled)
						continue;
				
					//By default were just looking at this specific variable instance
					//However if 'Global' is enabled, consider this variable on any models of this type
					List<Object> currentValues = new List<Object>();
					if(requirement.Global)
					{
						//Note: We do this check everytime, since models could have been added dynamically
						if(requirement.Variable != null)
						{
						    foreach(Model model in this.Models.FindType(requirement.Variable.Model.GetType()))
						    {
							    ModelVariables variables = model.Variables.Find(requirement.Variable.Name);
							    foreach (ModelVariable variable in variables)
							    {
								    currentValues.Add(variable.CurrentValue);
							    }
						    }
						}
						else
						{
						    currentValues.Add(true);
						}
					}
					else
					{
					    if(requirement.Variable == null)
					        throw new ModelException(this, "Variable is null?");
					    currentValues.Add(requirement.Variable.CurrentValue);
					}
						
					//See if this requirement is met
					bool matched = false;
					foreach (Object value in currentValues)
					{
						if(requirement.Evaluate(value))
						{
							matched = true;
							break;
						}
					}

					//Stop on first mis-match, (ie: at least one requirement didn't match)
					if(!matched)
					{
						//First failed requirement
						//Note: This has to be order based, since production checks conditions in-order
						//and will fail (and throw) according to the first one not meet
						failedrequirement = requirement;
						return false;
					}
				}
			}
			
			failedrequirement = null;
			return true;
		}

        /// <summary>
        /// Determine Next Action
        /// </summary>
        /// <returns></returns>
		protected ModelAction DetermineNextAction()
		{
			return this.GetPossibleActions().Choose(this);
		}
		/// <summary>
        /// Determine Parameters
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		protected ModelParameters		DetermineParameters(ModelAction action)
		{
			try
			{
				ModelParameters allparameters = action.Parameters;
				ModelParameters choosen = new ModelParameters();

				//Loop through the method parameters
				foreach(ParameterInfo info in action.Method.GetParameters())
				{
					//Find the all parameters assoicated with this param
					ModelParameters parameters = allparameters.Find(info.Name);
					//Exclude invalid parameters (if not requested)
					if(!this.InvalidParameters)
						parameters = (ModelParameters)parameters.FindFlag((int)ModelItemFlags.Throws, false);
					if(parameters.Count <= 0)
						throw new ModelException(this, "Unable to find a ModelParameter for method parameter: '" + info.Name + "'");

					//Choose one of the parameters, based upon weight
					//Note: We cloning the param, since were choosing only one of the values to use this time.
					ModelParameter parameter = (ModelParameter)parameters.Choose(this).Clone();

					//Choose (or generate) one of the values
					ModelValue value = DetermineParameterValue(parameter);

					//Add it to the array
					parameter.Value = value;
					choosen.Add(parameter);
				}
				
				return choosen;
			}
			catch(Exception e)
			{
				//Make this easier to debug
				throw new ModelException(this, "DetermineParameters", e);
			}
		}
		
        /// <summary>
        /// Determine Parameter Value
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
		protected ModelValue			DetermineParameterValue(ModelParameter parameter)
		{
			ModelValues values = new ModelValues();
			values.Add(parameter.Values);

			//Parameter values, can be specified in numerous ways:
			//	1.  Value list - simply choose one of them
			//	2.  Expression - generate value that meets the criteria (ie: <, >, !=, etc)
			//	3.  Variable - simply obtain the value by calling a method/field

			//#3. Variable - simply obtain the value by calling a method/field
			if(parameter.Variable != null)
			{
                object current = parameter.Variable.CurrentValue;
				if(current is IEnumerable && !typeof(IEnumerable).IsAssignableFrom(parameter.Type))
				{
 					foreach(object v in (IEnumerable)current)
						values.Add(new ModelValue(v));
				}
				else
				{
					values.Add(new ModelValue(current));
				}
			}

			//First ensure we have a set of values/requirements to choose from
			if(values.Count <= 0)
				throw new ModelException(parameter, "No values specified to choose from");

			//Note: Since we allow the operator on the individual values, this is a little more complex.
			//Note: We allow multiple operators, not just one, (ie: x > 5, < 10, and != 7). 
			//This gives you great power and flexibility in expressions, but means we have to work a 
			//little hard in determing a value that meets the requirements

			//#1.  Value list - simply choose one of them
			//Note: Bitmask is already exploded into combinations
			ModelValues equalvalues = values.FindOperator(ModelValueOperator.Equal);

			//#2.  Expression - generate value that meets the criteria (ie: <, >, !=, etc)
			//Note: Since we allow the operator on the individual values, this is a little more complex.
			//Note: We allow multiple operators, not just one, (ie: x > 5, < 10, and != 7). 
			//This gives you great power and flexibility in expressions, but means we have to work a 
			//little hard in determing a value that meets the requirements.
			int min = Int32.MinValue;
			int max = Int32.MaxValue;

			//Adjust our parameter, simplier to loop over all of them
			foreach(ModelValue value in values.FindOperator(ModelValueOperator.Equal, false).FindOperator(ModelValueOperator.NotEqual, false))
			{
				//To keep this simple, we just support integers (for now).
				if(!(value.Value is int))
					throw new ModelException(parameter, "Generated value range must be specified in terms of integers, not '" + value.Type + "'");

				//Simplify our life
				int v = (int)value.Value;
				
				//Adjust Max (if there is one)
				switch(value.Operator)
				{
					case ModelValueOperator.LessThanOrEqual:
						if(v < max)
							max = v;
						break;
				
					case ModelValueOperator.GreaterThanOrEqual:
						if(v > min)
							min = v;
						break;

					case ModelValueOperator.LessThan:
						if(v-1 < max && v > Int32.MinValue/*prevent underflow*/)
							max = v-1;
						break;
				
					case ModelValueOperator.GreaterThan:
						if(v+1 > min && v < Int32.MaxValue/*prevent overflow*/)
							min = v+1;
						break;
				};
			}

			//Choose a new value, within the specified range
			//Note: We retry in case it equals one of the existing invalid values (ie: !=)
			while(true)
			{
				object choice = null;
				if(equalvalues.Count > 0)
				{
					//Simple: Choose one of the specified values
					choice = equalvalues.Choose(this).Value;
				}
				else
				{
					//Otherwise: Choose a value within in the range
					//Note: Random.Next = min <= x < max (so we have to add one)
					choice = _engineoptions.Random.Next(min, max < Int32.MaxValue ? max + 1 : max);	//Prevent overflow
				}

				//As soon as we find a value, within the range, (and not in the invalid list), were done
				bool valid = true;
				foreach(ModelValue invalid in values.FindOperator(ModelValueOperator.NotEqual))
				{
					if(invalid.Evaluate(choice, ModelValueOperator.Equal))
						valid = false;
				}

				if(valid)
					return new ModelValue(choice);
			}
		}

		/// <summary>
		/// This method does the following.
		/// It starts the engine and the stops it if 
		/// 1. Time out has occured OR
		/// 2. Exception has been thrown OR
		/// 3. Auto Restart is false and there are no more actions available to be called OR
		/// 4. Variable tracking was on and we have covered all values OR
		/// 5. State tracking was on and we have covered all the states OR
		/// 6. Maximum number of actions that we have specified have been invoked
		/// 7. and there
		/// </summary>
		public void		Run()
		{
			//No requirements, which means run until termination
			this.RunUntil((ModelRequirements)null);
		}

		/// <summary>
		/// This is similar to RunUntil( param object[] values ) except in this you can construct your own 
		/// requirements class and pass it in if those are complex.
		/// </summary>
		/// <param name="requirements"></param>
		/// <returns></returns>
		public bool		RunUntil(params ModelRequirement[] requirements)
		{
			//Delegate
			return this.RunUntil(new ModelRequirements(requirements));
		}

        /// <summary>
        /// RunUntil(custom delegate)
        /// Powerful version, where you own the code in the handler, and determine exactly when to stop execution
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool RunUntil(ModelFunction func)
        {
            //Build up a ModelRequirement around the delegate
            return this.RunUntil(new ModelExpression(func, new ModelValue(true)) );
        }

        delegate void SimpleExecutionDelegate(ModelAction action);

        void ExecutionDelegate(ModelAction action)
        {
            this.ExecuteAction(action);
        }

		/// <summary>
		/// This is similar to RunUntil( param object[] values ) except in this you can construct your own 
		/// requirements class and pass it in if those are complex.
		/// </summary>
		/// <param name="requirements"></param>
		/// <returns></returns>
		public bool		RunUntil(ModelRequirements requirements)
		{
			//Recalibrate any options passed on commandline since they should take precedence.
			//We cant do this anywhere else since user may set options which will override the
			//passed commandline options. Hence these options should be applied the last.
			this.Options.Load();

			//Run until the following requirements are met (ie: variable = values)
			//Or until the time is elapsed.
			long startingticks	= DateTime.Now.Ticks;
			long remainingticks = DetermineTicks();
			bool meetsrequirements	= false;
			ModelTrace.WriteLine("Seed: " + _engineoptions.Seed);

            //Continue until no more actions to execute
			ModelAction action = this.DetermineNextAction();
			while(action != null)
			{
				//Model
				Model model = action.Model;

				//Init
				if (model.Actions.Accessed == 0)
					model.Init();

				//CallFirst, actions
				foreach (ModelAction first in this.GetPossibleActions(model.Actions.FindFlag(ModelActionFlags.CallFirst)))
				{
                    if (first != action)
                    {
                        //	this.ExecuteAction(first);
			//This enables keyboard and mouse actions .. call synchronously
/*        [ModelAction]
        [ModelRequirement(Variable = "MaxSizeReached", Value = false)]
        public void WriteTextNode()
        {
            RichControl.Focus();
            AutomatedInput.TypeString("^{END}HELLO");
        }
*/
                        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle,new SimpleExecutionDelegate(ExecutionDelegate),first);
                    }
				}

				//Execute (choose the parameters as well)
				//this.ExecuteAction(action);
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new SimpleExecutionDelegate(ExecutionDelegate), action);

				//Determine if all the requirements were met
				if (requirements != null && requirements.Count > 0
					&& MeetsRequirements(requirements))
				{
					meetsrequirements = true;
					break;
				}

				//Check action count
				if (_actionstrace.Count >= _engineoptions.MaxActions)
				{
					ModelTrace.WriteLine("MaxActions: '" + _engineoptions.MaxActions + "' was reached.");
					break;
				}

				//Check Timeout
				long currentticks = DateTime.Now.Ticks;
				if (currentticks - startingticks > remainingticks)
				{
					ModelTrace.WriteLine("Timeout: '" + _engineoptions.Timeout + "' has elapsed.");
					break;
				}

				//Determine the next action
				action = this.DetermineNextAction();
			}

			//CallLast
			//Because requirements not met and model needs to shutdown correctly.
			if (!meetsrequirements)
			{
				//CallLast, actions
				foreach (Model model in this.Models)
				{
					if (model.Actions.Accessed > 0)	//Only if 'first' was called
					{

                        foreach (ModelAction last in this.GetPossibleActions(model.Actions.FindFlag(ModelActionFlags.CallLast)))
                        {
                            this.ExecuteAction(last);
                        }
					}
				}
			}

			return meetsrequirements;
		}

		/// <summary>
		/// This takes a set of ModelAction objects and runs them in the order specified.
		/// Useful for scenario playback.
		/// </summary>
		/// <param name="actions"></param>
		public void		RunScenario( params ModelAction[] actions )
		{
			//Delegate
			this.RunScenario(new ModelActions( actions ));
		}

		/// <summary>
		/// Another overload for RunActions which runs the actions specified.
		/// </summary>
		/// <param name="actions"></param>
		public void		RunScenario(ModelActions actions)
		{
			//Delegate
			foreach(ModelAction action in actions )
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new SimpleExecutionDelegate(ExecutionDelegate), action);
				//this.ExecuteAction(action);
		}

        delegate void SimpleExecutionInfoDelegate(ModelActionInfo actionInfo);
        void ExecutionInfoDelegate(ModelActionInfo actioninfo)
        {
            this.ExecuteActionInfo(actioninfo);
        }

		/// <summary>
		/// If you are also interested in the actual parameters and not just actions,
		/// you can use RunScenario which takes the ModelActionInfo object which also 
		/// contains the parameter values.
		/// </summary>
		/// <param name="actioninfos"></param>
		public void		RunScenario(params ModelActionInfo[] actioninfos)
		{
			//Delegate
            foreach (ModelActionInfo actioninfo in actioninfos)
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new SimpleExecutionInfoDelegate(ExecutionInfoDelegate), actioninfo);
                //this.ExecuteActionInfo(actioninfo);
            }
		}

		private void	ExecuteAction(ModelAction action)
		{
			//CallBefore, actions
			foreach(ModelAction before in this.GetPossibleActions(action.Model.Actions.FindFlag(ModelActionFlags.CallBefore)))
				this.ExecuteActionInfo(new ModelActionInfo(before, this.DetermineParameters(before), null));

			//Execute Action
			this.ExecuteActionInfo(new ModelActionInfo(action, this.DetermineParameters(action), null));

			//CallAfter, actions
			foreach(ModelAction after in this.GetPossibleActions(action.Model.Actions.FindFlag(ModelActionFlags.CallAfter)))
				this.ExecuteActionInfo(new ModelActionInfo(after, this.DetermineParameters(after), null));
		}

		private object		ExecuteActionInfo(ModelActionInfo actioninfo)
		{
			ModelAction action			= actioninfo.Action;
			ModelParameters parameters	= actioninfo.Parameters;
			Model model					= action.Model;

			//Pre-Execute, events
			//Note: If that returns false, we simply don't execute the method
			if(!model.OnCallBefore(action, parameters))
				return false;

			//Adding the selected action (and its param values) to the trace.
			_actionstrace.Add( actioninfo );

			//Execute the method (delegate)
    		actioninfo.RetVal = action.Execute(parameters);

			//Add the returned model to the system
			Model output = actioninfo.RetVal as Model;
			if(output != null)
			{
				//If it doesn't already exist, and the model type is part of the set
				if(this.Models.FindInstance(output) == null)
				{ 
					actioninfo.Created = true;
					Models found = this.Models.FindType(output.GetType());

                    //Either the model is part of the 'type' space, or the user wishes to add all returned models
                    //Note: We always obey the maxinstance count
                    if((found.Count > 0 || this.Options.AddReturnedModels) && found.Count < model.MaxInstances)
                    {
                        output.Disabled = false;
                        this.Models.Add(output);
                    }
				}
			}

			//Trace
			ModelTrace.WriteLine(ModelTrace.FormatMethod(actioninfo));

			//Post-Execute, events
			model.OnCallAfter(action, parameters, actioninfo.RetVal);

			//Return
			return actioninfo.RetVal;
		}
	}
}
