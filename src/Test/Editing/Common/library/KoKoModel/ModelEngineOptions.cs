// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;				

namespace Microsoft.Test.KoKoMo
{
	/// <summary>
	/// Global weighing scheme for the engine. Depending on this scheme
	/// the engine decides the probabilities.
	/// </summary>
	public enum WeightScheme
	{
        /// <summary>Default weighting (specified in the model) </summary>
		Custom,
        /// <summary>Equal weighing</summary>
		Equal,

        /// <summary>Adjusted to weight uncalled item higher</summary>
		AdaptiveEqual,

        /// <summary>Each item has twice the chance of the next one.</summary>
		Geometric 
	};
	
	/// <summary>
	/// This class stores all the options that are set on the Engine.
	/// </summary>
	public class ModelEngineOptions
	{
		//Data
		int				_seed			    = 0;
		Random			_rand			    = null;
		WeightScheme	_weightscheme	    = WeightScheme.Custom;	//Default - as defined in the model
		long            _timeout			= 30;				    //Default, seconds
		long            _maxactions			= long.MaxValue;	    //Default, no limit
        bool            _addreturnedmodels  = true;
		bool            _loaded				= false;

		//Constructor
		/// <summary>Default constructor</summary>
		public ModelEngineOptions()
		{
            _seed = unchecked((int)DateTime.Now.Ticks);
			//Default is already setup
		}

		//Accessors
		/// <summary>
		/// Weight Scheme to be used by the Engine. See WeightScheme Enum for various settings.
		/// </summary>
		public virtual	WeightScheme		WeightScheme
		{
			//Expose our random generator, in case tests need it, (ie: one seed for complete repro)
			get	{ return _weightscheme;				}
			set	{ _weightscheme = value;			}
		}

		/// <summary>
		/// Sets/Gets the unit value of the duration. Default value = 30 seconds
		/// </summary>
		public long							Timeout
		{
			get{ return _timeout;							}
			set{ _timeout = value;							}
		}

		/// <summary>
		/// Sets/Gets the maximum actions that must be executed by the state machine.
		/// After the number of actions set is hit, the machine stops and returns to user.
		/// Along with Seed, this is a powerful way to reproduce errors and add regressions.
		/// </summary>
		public long							MaxActions
		{
			get{ return _maxactions;					}
			set{ _maxactions = value;					}
		}

        /// <summary>
        /// This property allows you to automatically add returned models from actions
        /// </summary>
        public bool                         AddReturnedModels
        {
            get { return _addreturnedmodels;            }
            set { _addreturnedmodels = value;           }
        }
        
        /// <summary>
        /// Random
        /// </summary>
		public virtual	Random				Random
		{
			//Expose our random generator, in case tests need it, (ie: one seed for complete repro)
			get	
			{
                if (_rand == null)
                    _rand = new Random(_seed);
				return _rand;
			}
		}

		/// <summary>
		/// The seed that is used to set/get on the random sequence generator.
		/// </summary>
		public virtual	int					Seed
		{
			get	{ return _seed;						}
			set	
			{
				_seed = value;
				_rand = new Random(value);
			}
		}

        /// <summary>Load</summary>
		public void							Load()
		{
			if(!_loaded)
			{
				_loaded = true;
				string[] args = Environment.GetCommandLineArgs();
				int count = 0;
				while (count < args.Length)
				{
					switch (args[count].ToUpper(System.Globalization.CultureInfo.InvariantCulture))
					{
                        case "/ADDRETURNEDMODELS+":
                            this.AddReturnedModels = true;
                            break;
                        case "/ADDRETURNEDMODELS-":
                            this.AddReturnedModels = false;
                            break;
						case "/SEED":
							if (count + 1 < args.Length)
							{
								this.Seed = Int32.Parse(args[count + 1]);
								ModelTrace.WriteLine("setting seed from commandline: " + this.Seed);
								count++;
							}
							else
								throw new ModelException(null, "Command line argument Seed needs a parameter");
							break;
						case "/MAXACTIONS":
							if (count + 1 < args.Length)
							{
								this.MaxActions = Int32.Parse(args[count + 1]);
								count++;
							}
							else
								throw new ModelException(null, "Command line argument MaxAction needs a parameter");
							break;
						case "/TIMEOUT":
							if (count + 1 < args.Length)
							{
								this.Timeout = Int32.Parse(args[count + 1]);
								count++;
							}
							else
								throw new ModelException(null, "Command line argument Timeout needs a parameter");
							break;
						case "/WEIGHTING":
							if (count + 1 < args.Length)
							{
								this.WeightScheme = (WeightScheme)Enum.Parse(typeof(WeightScheme), args[count + 1], true /* Ignore case */ );
								count++;
							}
							else
								throw new ModelException(null, "Command line argument Weighting needs a parameter");
							break;
						default:
							break;
					}
					count++;
				}
			}
		}
	}
}
