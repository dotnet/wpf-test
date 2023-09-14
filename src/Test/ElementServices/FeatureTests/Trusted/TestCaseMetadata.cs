// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Exception that is thrown the the metadata is not valid.
    /// </summary>
    public class SetupMetadataException : Exception 
    {
        /// <summary>
        /// This is the main contructor for instantiating the type.
        /// </summary>
        /// <param name="msg"></param>
        public SetupMetadataException(string msg) : base(msg)
        {
        }
        /// <summary>
        /// This constructor is required for deserializing this type across AppDomains.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SetupMetadataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Attribute that represents all the values for all others metadata attributes.
    /// </summary>
    /// <remarks>
    /// The class has to be decorated with CoreTestsLoaderAttribute.MethodBase so we can find this attribute
    /// on the methods.  This attribute can only be applied on methods. Each method can have multiple attributes.
    /// </remarks>
    [AttributeUsage (AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class TestCaseAttribute : Attribute 
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseAttribute(string priority, string area, TestCaseSecurityLevel securityLevel, string title) 
            : this (priority, area, "", "0", securityLevel, title)  {}


        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseAttribute(string priority, string area, string title) 
            : this (priority, area, "", "0", TestCaseSecurityLevel.PartialTrust, title)  {}

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseAttribute(string priority, string area, string paramsstr, string title)
            : this(priority, area, paramsstr, "0", TestCaseSecurityLevel.PartialTrust, title) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseAttribute(string priority, string area, string paramsstr,TestCaseSecurityLevel securityLevel,  string title)
			: this(priority, area, paramsstr, "0", securityLevel, title) { }


		/// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseAttribute(string priority, string area, string paramsstr, string disabled, string title)
			: this(priority, area, paramsstr, disabled, TestCaseSecurityLevel.PartialTrust, title) { }

		/// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public TestCaseAttribute(string priority, string area, string paramsstr, string disabled,
            TestCaseSecurityLevel securityLevel, string title)
        {
            if (area == String.Empty)
            {
                throw new SetupMetadataException("The area on the TestCaseAttribute cannot be empty");
            }
            
            if (priority == "0" || priority == "1" || priority == "2" || priority == "3" || priority == "4")
            {
                this._priority = priority;
            }
            else
            {
                throw new SetupMetadataException("Invalid priority on the TestCaseAttribute. Valid [0-4]");
            }

            if (disabled == "0" || disabled == "1")
            {
                this._disabled = disabled;
            }
            else
            {
                throw new SetupMetadataException("Invalid disabled on the TestCaseAttribute. Only [0 | 1]");
            }
            
            this._securityLevel = securityLevel;        
            this._area = area;
            this._title = title;
            this._params = paramsstr;
        }
        
        /// <summary>
        /// Get the value for this Area.
        /// </summary>
        public string Area 
        {
            get {return this._area;}
        }

        /// <summary>
        /// Get the value for this Priority.
        /// </summary>
        public string Priority 
        {
            get {return this._priority;}
        }

        /// <summary>
        /// Get the value is disabled.
        /// </summary>
        public string Disabled 
        {
            get {return this._disabled;}
        }

        /// <summary>
        /// Get the value for title.
        /// </summary>
        public string Title
        {
            get {return this._title;}
        }

        /// <summary>
        /// Get the value for Params.
        /// </summary>
        public string Params
        {
            get {return this._params;}
        }

        /// <summary>
        /// Get if the test case will run under partial trust.
        /// </summary>
		public TestCaseSecurityLevel SecurityLevel
		{
            get {return this._securityLevel;}
        }

        /// <summary>
        /// Our title.
        /// </summary>
        private string _title = String.Empty;

        /// <summary>
        /// Hold disabled value.
        /// </summary>
        private string _disabled = "0";

        /// <summary>
        /// Hold the Area.
        /// </summary>
        private string _area;
        /// <summary>
        /// Hold the Priority.
        /// </summary>
        private string _priority;

        /// <summary>
        /// Hold the Params.
        /// </summary>
        private string _params;

        /// <summary>
        /// Hold the SEE.
        /// </summary>
		private TestCaseSecurityLevel _securityLevel = TestCaseSecurityLevel.PartialTrust;

	}



    /// <summary>
    /// This attibrute is to support TestContainer type of cases. Where 
    /// the Presentation is not tied to the test case.
    /// </summary>
    [AttributeUsage (AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class TestCaseContainerAttribute : TestCaseAttribute 
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseContainerAttribute(string containerName, string containerHostType, string priority,
            string area, string title)
			: this(containerName, containerHostType, priority, area, "", "0", TestCaseSecurityLevel.PartialTrust, title) { }


        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseContainerAttribute(string containerName, string containerHostType, string priority,
            string area, TestCaseSecurityLevel securityLevel, string title)
			: this(containerName, containerHostType, priority, area, "", "0", securityLevel, title) { }


		/// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseContainerAttribute(string containerName, string containerHostType, string priority,
            string area, string paramsstr, string title)
			: this(containerName, containerHostType, priority, area, paramsstr, "0", TestCaseSecurityLevel.PartialTrust, title) { }


		/// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseContainerAttribute(string containerName, string containerHostType, string priority,
            string area, string paramsstr, TestCaseSecurityLevel securityLevel, string title)
			: this(containerName, containerHostType, priority, area, paramsstr, "0", securityLevel, title) { }

		/// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseContainerAttribute(string containerName, string containerHostType, string priority,
            string area, string paramsstr, string disabled, string title)
			: this(containerName, containerHostType, priority, area, paramsstr, disabled, TestCaseSecurityLevel.PartialTrust, title) { }

		/// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public TestCaseContainerAttribute(string containerName, string containerHostType, string priority, string area,
			string paramsstr, string disabled, TestCaseSecurityLevel securityLevel, string title)
			: base(priority, area, paramsstr, disabled, securityLevel, title)
		{
            if (containerName == "")
            {
                throw new SetupMetadataException("The containerName cannot be empty.");
            }
            if (containerHostType == "")
            {
                throw new SetupMetadataException("ContainerHostType cannot be empty.");
            }

            _containerName = containerName;
            _containerHostType = containerHostType;
        }

        /// <summary>
        /// Get the value for TestContainer type.
        /// </summary>
        public string ContainerName
        {
            get {return this._containerName;}
        }

        /// <summary>
        /// Get the value of the type of Presentation that will be displayed.
        /// </summary>
        public string ContainerHostType
        {
            get {return this._containerHostType;}
        }

        /// <summary>
        /// Hold disabled value.
        /// </summary>
        private string _containerName = "";

        /// <summary>
        /// Hold the Area.
        /// </summary>
        private string _containerHostType = "";

    }


    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class TestCaseSeedBasedAttribute : TestCaseAttribute 
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseSeedBasedAttribute(int seedStart, int seedEnd, string priority,
            string area, string title)
			: this(seedStart, seedEnd, priority, area, "", "0", TestCaseSecurityLevel.PartialTrust, title) { }


        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseSeedBasedAttribute(int seedStart, int seedEnd, string priority,
            string area, TestCaseSecurityLevel securityLevel, string title)
			: this(seedStart, seedEnd, priority, area, "", "0", securityLevel, title) { }


		/// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseSeedBasedAttribute(int seedStart, int seedEnd, string priority,
            string area, string paramsstr, string title)
			: this(seedStart, seedEnd, priority, area, paramsstr, "0", TestCaseSecurityLevel.PartialTrust, title) { }


		/// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseSeedBasedAttribute(int seedStart, int seedEnd, string priority,
            string area, string paramsstr, TestCaseSecurityLevel securityLevel, string title)
			: this(seedStart, seedEnd, priority, area, paramsstr, "0", securityLevel, title) { }

		/// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseSeedBasedAttribute(int seedStart, int seedEnd, string priority,
            string area, string paramsstr, string disabled, string title)
			: this(seedStart, seedEnd, priority, area, paramsstr, disabled, TestCaseSecurityLevel.PartialTrust, title) { }

		/// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public TestCaseSeedBasedAttribute(int seedStart, int seedEnd, string priority, string area,
			string paramsstr, string disabled, TestCaseSecurityLevel securityLevel, string title)
			: base(priority, area, paramsstr, disabled, securityLevel, title)
		{
            if (seedStart < 0)
            {
                throw new SetupMetadataException("The seed start cannot be lower than 0.");
            }
            if (seedEnd < 0 || seedStart > seedEnd)
            {
                throw new SetupMetadataException("The seed end cannot be higher than 0 or lower than the seed start.");
            }
            
            _seedStart= seedStart;
            _seedEnd = seedEnd;
        }

        /// <summary>
        /// Get the value for Params.
        /// </summary>
        public int SeedStart
        {
            get {return this._seedStart;}
        }

        /// <summary>
        /// Get if the test case will run under SEE.
        /// </summary>
        public int SeedEnd
        {
            get {return this._seedEnd;}
        }

        /// <summary>
        /// Hold disabled value.
        /// </summary>
        private int _seedStart = 0;

        /// <summary>
        /// Hold the Area.
        /// </summary>
        private int _seedEnd = 0;

    }
    
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [AttributeUsage (AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class TestCaseModelAttribute : TestCaseAttribute 
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseModelAttribute(string xtcFileName, string priority, string area)
            : this(xtcFileName, priority, area, "") { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseModelAttribute(string xtcFileName, string priority, string area, string title)
            : base(priority, area, "", "0", TestCaseSecurityLevel.PartialTrust, title)
        {
            _xtcFileName = xtcFileName;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseModelAttribute(string xtcFileName, string priority, string area, TestCaseSecurityLevel securityLevel, string title)
            : base(priority, area, "", "0", securityLevel, title)
        {
            _xtcFileName = xtcFileName;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseModelAttribute(string xtcFileName, string priority, string area, string paramsString, string disabled, TestCaseSecurityLevel securityLevel, string title)
            : base(priority, area, paramsString, disabled, securityLevel, title)
        {
            _xtcFileName = xtcFileName;
        }
        

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseModelAttribute(string xtcFileName, int modelStart, int modelEnd, string priority,
            string area, string title)
			: this(xtcFileName, modelStart, modelEnd, priority, area, "", "0", TestCaseSecurityLevel.PartialTrust, title) { }


        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseModelAttribute(string xtcFileName, int modelStart, int modelEnd, string priority,
            string area, TestCaseSecurityLevel securityLevel, string title)
			: this(xtcFileName, modelStart, modelEnd, priority, area, "", "0", securityLevel, title) { }


		/// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseModelAttribute(string xtcFileName, int modelStart, int modelEnd, string priority,
            string area, string paramsstr, string title)
			: this(xtcFileName, modelStart, modelEnd, priority, area, paramsstr, "0", TestCaseSecurityLevel.PartialTrust, title) { }

		/// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseModelAttribute(string xtcFileName, int modelStart, int modelEnd, string priority,
            string area, string paramsstr, string disabled, string title)
			: this(xtcFileName, modelStart, modelEnd, priority, area, paramsstr, disabled, TestCaseSecurityLevel.PartialTrust, title) { }

		/// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public TestCaseModelAttribute(string xtcFileName, int modelStart, int modelEnd, string priority, string area,
			string paramsstr, string disabled, TestCaseSecurityLevel securityLevel, string title)
			: base(priority, area, paramsstr, disabled, securityLevel, title)
		{
            if (modelStart != -10000)
            {
                if (modelStart < 1)
                {
                    throw new SetupMetadataException("The model start cannot be lower than 1.");
                }
                if (modelEnd < 1 || modelStart > modelEnd)
                {
                    throw new SetupMetadataException("The model end cannot be higher than 1 or lower than the model start.");
                }
            }
            
            _xtcFileName = xtcFileName;
            _modelStart= modelStart;
            _modelEnd = modelEnd;
        }

        /// <summary>
        /// Get the value for title.
        /// </summary>
        public string XtcFileName
        {
            get {return this._xtcFileName;}
        }

        /// <summary>
        /// Get the value for Params.
        /// </summary>
        public int ModelStart
        {
            get {return this._modelStart;}
        }

        /// <summary>
        /// Get if the test case will run under SEE.
        /// </summary>
        public int ModelEnd
        {
            get {return this._modelEnd;}
        }

        /// <summary>
        /// Our title.
        /// </summary>
        private string _xtcFileName = String.Empty;

        /// <summary>
        /// Hold disabled value.
        /// </summary>
        private int _modelStart = 0;

        /// <summary>
        /// Hold the Area.
        /// </summary>
        private int _modelEnd = 0;

    }


    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [AttributeUsage (AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class MultipleThreadTestCaseModelAttribute : TestCaseModelAttribute 
    {

		/// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public MultipleThreadTestCaseModelAttribute(string xtcFileName, int numOfThreads, string priority, string area,
			string title) : this(
			xtcFileName,numOfThreads, -10000, 0, priority, area,"","0",TestCaseSecurityLevel.PartialTrust,title)
		{
		}

		/// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public MultipleThreadTestCaseModelAttribute(string xtcFileName, int numOfThreads, string priority, string area,
			TestCaseSecurityLevel securityLevel, string title) : this(
			xtcFileName,numOfThreads, -10000, 0, priority, area,"","0",securityLevel,title)
		{
		}



		/// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public MultipleThreadTestCaseModelAttribute(string xtcFileName, int numOfThreads, string priority, string area,
			string paramsstr, TestCaseSecurityLevel securityLevel, string title) : this(
			xtcFileName,numOfThreads, -10000, 0, priority, area,paramsstr,"0",securityLevel,title)
		{
		}



		/// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public MultipleThreadTestCaseModelAttribute(string xtcFileName, int numOfThreads, string priority, string area,
			string paramsstr, string disabled, TestCaseSecurityLevel securityLevel, string title) : this(
			xtcFileName,numOfThreads, -10000, 0, priority, area,paramsstr,disabled,securityLevel,title)
		{

		}

		/// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public MultipleThreadTestCaseModelAttribute(string xtcFileName, int numOfThreads, int modelStart, int modelEnd, 
            string priority, string area, string paramsstr, string disabled, TestCaseSecurityLevel securityLevel, string title)
			: base(xtcFileName, modelStart, modelEnd, priority, area, paramsstr, disabled, securityLevel, title)
		{
            _numOfThreads = numOfThreads;
		}


        /// <summary>
        /// Hold the Number of Threads for this test case.
        /// </summary>
        public int NumOfThreads
        {
            get
            {
                return _numOfThreads;
            }
        }

        /// <summary>
        /// Hold the Number of Threads for this test case.
        /// </summary>
        private int _numOfThreads = 0;

    }




    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class TestCaseHostedModelAttribute : TestCaseAttribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseHostedModelAttribute(string containerName, string containerHostType, string xtcFileName, string priority, string area)
            : this(containerName, containerHostType, xtcFileName, priority, area, "") { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseHostedModelAttribute(string containerName, string containerHostType, string xtcFileName, string priority, string area, string title)
            : base(priority, area, "", "0", TestCaseSecurityLevel.PartialTrust, title)
        {
            _xtcFileName = xtcFileName;
            _containerName = containerName;
            _containerHostType = containerHostType;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseHostedModelAttribute(string containerName, string containerHostType, string xtcFileName, string priority, string area, TestCaseSecurityLevel securityLevel, string title)
            : base(priority, area, "", "0", securityLevel, title)
        {
            _xtcFileName = xtcFileName;
            _containerName = containerName;
            _containerHostType = containerHostType;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseHostedModelAttribute(string containerName, string containerHostType, string xtcFileName, string priority, string area, string paramsString, string disabled, TestCaseSecurityLevel securityLevel, string title)
            : base(priority, area, paramsString, disabled, securityLevel, title)
        {
            _xtcFileName = xtcFileName;
            _containerName = containerName;
            _containerHostType = containerHostType;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseHostedModelAttribute(string containerName, string containerHostType, string xtcFileName, int modelStart, int modelEnd, string priority,
            string area, string title)
            : this(containerName, containerHostType, xtcFileName, modelStart, modelEnd, priority, area, "", "0", TestCaseSecurityLevel.PartialTrust, title) { }


        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseHostedModelAttribute(string containerName, string containerHostType, string xtcFileName, int modelStart, int modelEnd, string priority,
            string area, TestCaseSecurityLevel securityLevel, string title)
            : this(containerName, containerHostType, xtcFileName, modelStart, modelEnd, priority, area, "", "0", securityLevel, title) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseHostedModelAttribute(string containerName, string containerHostType, string xtcFileName, int modelStart, int modelEnd, string priority,
            string area, string paramsstr, string title)
            : this(containerName, containerHostType, xtcFileName, modelStart, modelEnd, priority, area, paramsstr, "0", TestCaseSecurityLevel.PartialTrust, title) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseHostedModelAttribute(string containerName, string containerHostType, string xtcFileName, int modelStart, int modelEnd, string priority,
            string area, string paramsstr, string disabled, string title)
            : this(containerName, containerHostType, xtcFileName, modelStart, modelEnd, priority, area, paramsstr, disabled, TestCaseSecurityLevel.PartialTrust, title) { }

        /// <summary>
        /// Constructor that takes everything.
        /// </summary>
        public TestCaseHostedModelAttribute(string containerName, string containerHostType, string xtcFileName, int modelStart, int modelEnd, string priority, string area,
            string paramsstr, string disabled, TestCaseSecurityLevel securityLevel, string title)
            : base(priority, area, paramsstr, disabled, securityLevel, title)
        {
            if (modelStart < 1)
            {
                throw new SetupMetadataException("The model start cannot be lower than 1.");
            }
            if (modelEnd < 1 || modelStart > modelEnd)
            {
                throw new SetupMetadataException("The model end cannot be higher than 1 or lower than the model start.");
            }

            _xtcFileName = xtcFileName;
            _modelStart = modelStart;
            _modelEnd = modelEnd;
            _containerName = containerName;
            _containerHostType = containerHostType;
        }

        /// <summary>
        /// Get the value for TestContainer type.
        /// </summary>
        public string ContainerName
        {
            get { return this._containerName; }
        }

        /// <summary>
        /// Get the value of the type of Presentation that will be displayed.
        /// </summary>
        public string ContainerHostType
        {
            get { return this._containerHostType; }
        }

        /// <summary>
        /// Get the value for title.
        /// </summary>
        public string XtcFileName
        {
            get { return this._xtcFileName; }
        }

        /// <summary>
        /// Get the value for Params.
        /// </summary>
        public int ModelStart
        {
            get { return this._modelStart; }
        }

        /// <summary>
        /// Get if the test case will run under SEE.
        /// </summary>
        public int ModelEnd
        {
            get { return this._modelEnd; }
        }

        /// <summary>
        /// Our title.
        /// </summary>
        private string _xtcFileName = String.Empty;

        /// <summary>
        /// Hold disabled value.
        /// </summary>
        private int _modelStart = 0;

        /// <summary>
        /// Hold the Area.
        /// </summary>
        private int _modelEnd = 0;

        /// <summary>
        /// Hold disabled value.
        /// </summary>
        private string _containerName = "";

        /// <summary>
        /// Hold the Area.
        /// </summary>
        private string _containerHostType = "";
    }

    /// <summary>
    /// Attribute that specifies the Area for the test case. No Inherited and not AllowMultiple
    /// </summary>
    /// <remarks>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestCaseAreaAttribute : Attribute 
    {
        /// <summary>
        /// Constructor that takes the area
        /// </summary>
        /// <param name="Area"></param>
        public TestCaseAreaAttribute(string Area)
        {
            if (Area == String.Empty)
            {
                throw new SetupMetadataException("The area on the TestCaseAreaAttribute cannot be empty");
            }
            this._area = Area;
        }
        
        /// <summary>
        /// Get the value for this Area
        /// </summary>
        public string Area 
        {
            get {return this._area;}
        }

        /// <summary>
        /// Hold the Area
        /// </summary>
        private string _area;
    }

	/// <summary>
	/// 
	/// </summary>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestCasePriorityAttribute : Attribute 
    {
        /// <summary>
        /// Constructor that takes the priority
        /// </summary>
        /// <param name="Priority">Valida priorities [0-4]</param>
        public TestCasePriorityAttribute(string Priority)
        {
            if (Priority == "1" || Priority == "2" || Priority == "3"||Priority == "4" || Priority == "0")
            {
                this._priority = Priority;
            }
            else
            {
                throw new SetupMetadataException("Invalid priority on the TestCasePriorityAttribute. Valid [0-4]");
            }
        }
        
        /// <summary>
        /// Get the Priority for the test case
        /// </summary>
        public string Priority 
        {
            get {return this._priority;}
        }

        /// <summary>
        /// 
        /// </summary>
        private string _priority;
    }

    /// <summary>
    /// Attribute that specifies if the test case is disable for the test case. No Inherited and not AllowMultiple
    /// </summary>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestCaseDisabledAttribute : Attribute 
    {
        /// <summary>
        /// Constructor that takes the Disabled
        /// </summary>
        /// <param name="Disable">"1" value means disabled. "0" values means enabled</param>
        public TestCaseDisabledAttribute(string Disable)
        {
            if (Disable == "0" || Disable == "1")
            {
                this._disable = Disable;
            }
            else
            {
                throw new SetupMetadataException("Invalid value. Only [0 | 1]");
            }
        }
        
        /// <summary>
        /// Get fi the test case is disabled
        /// </summary>
        public string Disabled 
        {
            get {return this._disable;}
        }

        /// <summary>
        /// 
        /// </summary>
        private string _disable = "0";
    }

    /// <summary>
    /// Attribute that specifies the extra parameters for the test case. No Inherited and not AllowMultiple.
    /// You can think on this a command line parameters
    /// </summary>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestCaseParamsAttribute : Attribute 
    {
        /// <summary>
        /// Constructor that takes the Parameters for the Test case
        /// </summary>
        /// <param name="Params"></param>
        public TestCaseParamsAttribute(string Params)
        {
            this._params = Params;
        }
        
        /// <summary>
        /// Get the params for the test case
        /// </summary>
        public string Params
        {
            get {return this._params;}
        }

        /// <summary>
        /// 
        /// </summary>
        private string _params = String.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestCaseSecurityLevelAttribute : Attribute 
    {
        /// <summary>
        /// Constructor that takes the security level. 
        /// </summary>
        public TestCaseSecurityLevelAttribute(TestCaseSecurityLevel securityLevel)
		{
			_securityLevel = securityLevel;
		}
        
        /// <summary>
        /// Get if the test case will run under SEE
        /// </summary>
        public TestCaseSecurityLevel SecurityLevel
        {
            get {return _securityLevel;}
        }

        /// <summary>
        /// 
        /// </summary>
		private TestCaseSecurityLevel _securityLevel = TestCaseSecurityLevel.PartialTrust;
	}

    /// <summary>
    /// Attribute that specifies the title of the test case or method. No Inherited and not AllowMultiple.
    /// You can use this for output to the screen or a log file.
    /// </summary>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestCaseTitleAttribute : Attribute 
    {
        /// <summary>
        /// Construct a title attribute.
        /// </summary>
        /// <param name="title">Title string</param>
        public TestCaseTitleAttribute(string title)
        {
            this._title = title;
        }
        
        /// <summary>
        /// Test case title.
        /// </summary>
        public string Title
        {
            get {return this._title;}
        }

        /// <summary>
        /// Our title.
        /// </summary>
        private string _title = String.Empty;
    }

    /// <summary>
    /// Type of the Test case
    /// </summary>
    public enum CoreTestsTestType 
    {
        /// <summary>
        /// Means that the Test case is base on a Class. this mean that is 1 test case per class. with one method
        /// as entry point. Only requiere on teh Class
        /// </summary>
        ClassBase,
        
        /// <summary>
        /// Means that the Test case is base on a Method. this mean that is multiple test cases per class. Each test case
        /// it will contain a method as entry point for each test case. It is really importnat to set this attribute variable value on the
        /// class and each method.
        /// </summary>
        MethodBase

    }

    /// <summary>
    /// This Attribute it design for Class and Methods. 
    /// </summary>
    /// <remarks>
    /// When this attribute takes ClassBase parameter this attribute should be set only on the Class. <para/>
    /// But when this attribute has MethodBase, you <b>MUST</b> set the attribute on the class and each method.
    /// </remarks>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class CoreTestsLoaderAttribute : Attribute 
    {
        /// <summary>
        /// Constructor that takes how is the Base type of the test case
        /// </summary>
        /// <param name="typeoftest"></param>
        public CoreTestsLoaderAttribute(CoreTestsTestType typeoftest)
        {
            this._typeoftest = typeoftest;
        }
        
        /// <summary>
        /// Get the type of Test Case
        /// </summary>
        public CoreTestsTestType TypeOfTest 
        {
            get {return this._typeoftest;}
        }

        /// <summary>
        /// 
        /// </summary>
        private CoreTestsTestType _typeoftest;
    }

    /// <summary>
    /// This Attribute it design for Class and Methods. 
    /// <para />This will be the Entry Point method to execute
    /// </summary>
    /// <remarks>
    /// This only is usefull of ClassBase test case.  
    /// </remarks>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestCaseMethodAttribute : Attribute 
    {
        /// <summary>
        /// Constructor that takes the Method.
        /// </summary>
        /// <param name="MethodName"></param>
        public TestCaseMethodAttribute(string MethodName)
        {
            this._methodName = MethodName;
        }
        
        /// <summary>
        /// Get the method name
        /// </summary>
        public string MethodName 
        {
            get {return this._methodName;}
        }

        /// <summary>
        /// 
        /// </summary>
        private string _methodName = "RunTest";
    }

    /// <summary>
    /// This Attribute it design for Class and Methods.
    /// <para />
    /// This set the Timeout requiere for the specific test case.
    /// </summary>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestCaseTimeoutAttribute : Attribute 
    {
        /// <summary>
        /// Constructor that takes the area
        /// </summary>
        /// <param name="TimeOut">On seconds. By default 180 seconds</param> 
        public TestCaseTimeoutAttribute(string TimeOut)
        {
            this._timeOut = TimeOut;
        }
        
        /// <summary>
        /// Get the Timeout
        /// </summary>
        public string TimeOut 
        {
            get {return this._timeOut;}
        }

        /// <summary>
        /// 
        /// </summary>
        private string _timeOut = "180";
    }

    /// <summary>
    /// This Attribute it design for Class and Methods.
    /// <para />
    /// For Support Files
    /// </summary>
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class TestCaseSupportFileAttribute : Attribute 
    {
        /// <summary>
        /// Constructor that takes the fileName
        /// </summary>
        /// <param name="fileName"></param> 
        public TestCaseSupportFileAttribute(string fileName)
        {
            this._fileName = fileName;
        }
        
        /// <summary>
        /// Get the support file
        /// </summary>
        public string FileName 
        {
            get {return this._fileName;}
        }

        /// <summary>
        /// 
        /// </summary>
        private string _fileName = "";
    }
}
