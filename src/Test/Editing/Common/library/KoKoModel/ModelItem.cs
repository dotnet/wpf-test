// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;				
using System.Collections;	        //Hashtable
using System.Collections.Generic;	//List<T>


namespace Microsoft.Test.KoKoMo
{
	/// <summary>
	/// This Enum stores the values to track if a particular property on the custom attribute was set.
	/// It is used for boolean flags such as Invalid, Disabled, Tracked, etc.
	/// </summary>
	public enum ModelItemFlags
	{
        /// <summary>Invalid</summary>
		Invalid		= 0x01000000,
        /// <summary>Throws</summary>
		Throws		= 0x02000000,
        /// <summary>Disabled</summary>
		Disabled	= 0x10000000,
        /// <summary>Tracked</summary>
		Tracked		= 0x20000000
	};

	///<summary>
	///This class is the base class for all custom attributes in KoKoMo
	///</summary>
	public abstract class ModelItemAttribute : Attribute
	{
		/// <summary></summary>
		protected int		    _weight		= 1;		//Weighting
        /// <summary></summary>
        
        protected String	    _name		= null;		//Name
        /// <summary></summary>
        
        protected String	    _category	= null;		//Category
        /// <summary></summary>
        
        protected Object[]      _labels	    = null;		//Labels
        /// <summary></summary>
        protected int	        _flags		= (int)ModelItemFlags.Tracked;
		
        Type			        _exception;				//If the action ALWAYS throws an exception
		string			        _exceptionid;			//Advanced verification

		///<summary>
		///Returns the value of the Name property on the model item.
		///</summary>
        public virtual String           Name
		{
			get { return _name;			}
			set { _name = value;		}
		}

		///<summary>
		///Returns the value of the Desc property on the model item.
		///</summary>
        public virtual String           Category
		{
			get { return _category;		}
			set { _category = value;	}
		}

		///<summary>
		///Returns the value of the Weight property on the model item.
		///</summary>
        public virtual int              Weight
		{
			get { return _weight;		}
			set { _weight = value;		}
		}

		///<summary>
		///Returns the value of the Disabled property on the model item.
		///</summary>
        public virtual bool             Disabled
		{
			get { return IsFlag((int)ModelItemFlags.Disabled);		}
			set { SetFlag((int)ModelItemFlags.Disabled, value);		}
		}

		///<summary>
		///Returns the value of the Tracked property on the model item.
		///Default is true.
		///</summary>
        public virtual bool             Tracked
		{
			get { return IsFlag((int)ModelItemFlags.Tracked);		}
			set { SetFlag((int)ModelItemFlags.Tracked, value);		}
		}

		///<summary>
		///Returns the value of the Invalid property on the model item.
		///</summary>
        public virtual bool             Invalid
		{
			get { return IsFlag((int)ModelItemFlags.Invalid);		}
			set { SetFlag((int)ModelItemFlags.Invalid, value);		}
		}

		///<summary>
		///This property returns the exception type specified on the model item.
		///</summary>
        public virtual Type             Exception
		{
			get { return _exception;		}
			set 
			{
				_exception = value;		
				SetFlag((int)ModelItemFlags.Throws, true);
			}
		}

		///<summary>
		///This property returns the Exception Id specified on the model item.
		///</summary>
        public virtual string           ExceptionId
		{
			get { return _exceptionid;		}
			set 
			{ 
				_exceptionid = value;		
				SetFlag((int)ModelItemFlags.Throws, true);
			}
		}

		///<summary>
		///Returns if the model item has a Throws= property set.
		///</summary>
        public virtual bool             Throws
		{
			get { return IsFlag((int)ModelItemFlags.Throws);		}
		}

		///<summary>
		///This property returns all the properties set on the custom attribute.
		///</summary>
        public virtual int              Flags
		{
			get { return _flags;		}
			set { _flags = value;		}
		}

		/// <summary>
		/// This method returns true if the required property is set on the custom attribute.
		/// </summary>
		/// <param name="flag">Flag to set specified from the enum</param>
		/// <returns>true if Flag is set to true</returns>
        public virtual bool             IsFlag(int flag)
		{
			return (_flags & flag) == flag;	
		}
		
		/// <summary>
		/// This method Sets the flags to track all the properties set on the custom attribute of the item.
		/// </summary>
		/// <param name="flag">Flag to set specified from the enum</param>
		/// <param name="value">true/false for the flag</param>
        public virtual void             SetFlag(int flag, bool value)
		{
			if(value)
				_flags |= flag;
			else
				_flags &= ~flag;
		}

        /// <summary>
        /// label
        /// </summary>
        public virtual object           Label
		{
			get 
			{
				if(_labels != null )
					return _labels[0]; 
				return null;
			}
			set
			{
				if(_labels == null) 
					_labels = new Object[1];  
				_labels[0] = value;
			}
		}

        /// <summary>
        /// Labels 
        /// </summary>
        public virtual object[]         Labels
		{
			get	{ return _labels;	    }
			set	{ _labels = value;		}
		}
	}


	/// <summary>
	/// The base class for any item in the KoKoMo framework.
	/// This is the base class for any Action, Parameter, Variable, Requirement, Range etc.
	/// </summary>
	public abstract class ModelItem
	{
		/// <summary> Weight of the item. </summary>
		protected int	    _weight		= 1;	//Default
		
        /// <summary>item name </summary>
        protected String    _name		= null;
        
        /// <summary>item name </summary>
		protected String    _fullname	= null;
        
        /// <summary>item name </summary>
        protected String    _category	= null;
        /// <summary>item name </summary>
        private   Model	    _model      = null;
        
        /// <summary>item name </summary>
        protected object[]  _labels	    = null;
		
		/// <summary>Flags set on the item.</summary>
		protected int	_flags		    = 0;
		
        internal int	_accessed	    = 0;

		private int	_id				    = 0;
		static Hashtable s_idhash	    = new Hashtable(100);
		
		Type			_exception		= null;	//Invalid Value - throws exception <type>
		string			_exceptionid	= null;	//Invalid Value - throws exception <id>
 
        /// <summary>
        /// Constructor to create an item.
        /// </summary>
        /// <param name="model">Custom model</param>
        /// <param name="attr">Custom attribute specified on the item.</param>
		public ModelItem(Model model, ModelItemAttribute attr)
		{
			_model = model;
			this.SetAttributeValues(attr);
		}

		//Accessors
		/// <summary>
		/// Property that returns the name of this item.
		/// </summary>
		public virtual string		Name
		{
			get 
			{ 
				if(_name != null)
					return _name;
				return this.GetType().Name;	
			}
			set { _name = value;				}
		}

		//Accessors
		/// <summary>
		/// Property that returns the full name of this item.
		/// </summary>
		public virtual string		FullName
		{
			get 
			{ 
				if(_fullname == null && _model != null)
				{
					_fullname = _model.Name + "." + this.Name;
					return _fullname;
				}

				//Otherwise
				return this.Name;
			}
		}

        /// <summary>
        /// Model
        /// </summary>
		public virtual Model		Model
		{
			get { return _model;				}
		}

		//Accessors
		/// <summary>
		/// Property that returns the category of this item.
		/// </summary>
		public virtual string		Category
		{
			get { return _category;				}
			set { _category = value;			}
		}

		/// <summary>
		/// Gets the label declared on the item.
		/// </summary>
		public virtual object		Label
		{
			get 
			{
				if(_labels != null && _labels.Length > 0 )
					return _labels[0];
				return null;
			}
		}
		/// <summary>
		/// Gets the labels declared on the item.
		/// </summary>
		public virtual object[]		Labels
		{
			get { return _labels; }
		}

		/// <summary>
		/// Returns the ID of the item.
		/// </summary>
		public virtual	int			Id
		{
			get 
			{ 
				if(_id == 0)
					_id = NextId(this.GetType());
				return _id;
			}
			set 
			{ 
				_id = value;
			}
		}
		
        /// <summary>
        /// Next ID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		public static int			NextId(Type type)
		{
			//Retrieve it from the hash
			object id = s_idhash[type];
			if(id == null)
				id = 1;
			else
				id = (int)(id) + 1;

			//Add it to the hash
			s_idhash[type] = id;
			return (int)id;
		}

		/// <summary>
		/// Returns the Weight of the item.
		/// </summary>
		public  virtual int			Weight
		{
			get { return _weight;				}
			set { _weight = value;				}
		}

		/// <summary>
		/// Returns if the item is disabled or not.
		/// </summary>
		public virtual bool			Disabled
		{
			get { return IsFlag((int)ModelItemFlags.Disabled);		}
			set { SetFlag((int)ModelItemFlags.Disabled, value);		}
		}

		/// <summary>
		/// Returns if the item is tracked or not.
		/// </summary>
		public virtual bool			Tracked
		{
			get { return IsFlag((int)ModelItemFlags.Tracked);		}
			set { SetFlag((int)ModelItemFlags.Tracked, value);		}
		}

		/// <summary>
		/// Returns if the item is invalid or not.
		/// </summary>
		public virtual bool			Invalid
		{
			get { return IsFlag((int)ModelItemFlags.Invalid);		}
			set { SetFlag((int)ModelItemFlags.Invalid, value);		}
		}

		/// <summary>
		/// Returns the type of exception thrown by the item.
		/// </summary>
        public virtual Type         Exception
		{
			get { return _exception;		}
			set 
			{
				_exception = value;
                SetFlag((int)ModelItemFlags.Throws, (value == null && String.IsNullOrEmpty(ExceptionId))? false : true);
			}
		}

		/// <summary>
		/// Returns the ExceptionID for this item.
		/// </summary>
        public virtual string       ExceptionId
		{
			get { return _exceptionid;		}
			set 
			{ 
				_exceptionid = value;		
				SetFlag((int)ModelItemFlags.Throws, (String.IsNullOrEmpty(value) && Exception==null) ? false : true);
			}
		}

		/// <summary>
		/// Returns if the item is throws an exception or not.
		/// </summary>
        public virtual bool         Throws
		{
			get { return IsFlag((int)ModelItemFlags.Throws);		}
		}

		/// <summary>
		/// Virtual method to set the item state depending on the custom attribute definition.
		/// </summary>
		public virtual void		    SetAttributeValues(ModelItemAttribute attr)
		{
			if(attr != null)			
			{	
				_weight		= attr.Weight;
				_flags		= attr.Flags;
				_exception	= attr.Exception;
				_exceptionid= attr.ExceptionId;
				_name		= attr.Name;
				_category	= attr.Category;
				_labels		= attr.Labels;
			}
		}

		/// <summary>
		/// Returns the current set of flags on this Item.
		/// </summary>
        public virtual int          Flags
		{
			get { return _flags;				}
			set { _flags = value;				}
		}

		/// <summary>
		/// This method returns true if the required property is set on the custom attribute.
		/// </summary>
		/// <param name="flag">Flag to set specified from the enum</param>
		/// <returns>true if Flag is set to true</returns>
        public virtual bool         IsFlag(int flag)
		{
			return (_flags & flag) == flag;	
		}
		
		/// <summary>
		/// This method returns true if the required property is set on the custom attribute.
		/// </summary>
		/// <param name="flag">Flag to set specified from the enum</param>
		/// <returns>false if Flag is set to true</returns>
        public virtual bool         IsFlagExcept(int flag)
		{
			return (_flags & flag) == 0;
		}

		/// <summary>
		/// This method Sets the flags to track all the properties set on the custom attribute of the item.
		/// </summary>
		/// <param name="flag">Flag to set specified from the enum</param>
		/// <param name="value">true/false for the flag</param>
        public virtual void         SetFlag(int flag, bool value)
		{
			if(value)
				_flags |= flag;
			else
				_flags &= ~flag;
		}

		/// <summary>
		/// Returns the number of times this item was accessed.
		/// </summary>
        public virtual int          Accessed
		{
			get { return _accessed;			}
			set { _accessed = value;		}
		}
		
		/// <summary>
		/// Method to reload this item and reset its values.
		/// </summary>
		public abstract void		Reload();
		/// <summary>
		/// Method to copy this item.
		/// </summary>
		/// <returns></returns>
		public abstract object		Clone();
		/// <summary>
		/// Overriden ToString()
		/// </summary>
		/// <returns>Item.Name</returns>
		public override string		ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Checks if a specified label is defined on the item.
		/// </summary>
		/// <param name="label">Label to check</param>
		/// <returns>True if label is defined, false otherwise</returns>
		public virtual int          CompareLabel( object label )
		{
			if ( this._labels != null )
			{
				foreach( object lbl in _labels )
				{
					IComparable icomp = lbl as IComparable;
					if ( icomp != null )
					{
						return icomp.CompareTo(label);
					}
					else
					{
						if ( lbl.Equals(label) )
							return 0;
					}
				}
			}
			return 1;
		}
	}
}
