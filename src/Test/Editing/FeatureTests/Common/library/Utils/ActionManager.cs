// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a class to collect and dispatch action items.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/ActionManager.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using Test.Uis.Loggers;

    #endregion Namespaces.

    /// <summary>
    /// The type of invoke we support (or plan to support).
    /// </summary>
    public enum InvokeType
    {
        /// <summary>
        /// Invoke static method
        /// </summary>
        StaticMethod = 0,

        /// <summary>
        /// Invoke instance method (need this pointer to be passed as first parameter in InvokeStaticOrInstanceMethod)
        /// </summary>
        InstanceMethod,

        /// <summary>
        /// Invoke static get field
        /// </summary>
        GetStaticField,

        /// <summary>
        /// Invoke static set field
        /// </summary>
        SetStaticField,

        /// <summary>
        /// Invoke instance get field
        /// </summary>
        GetInstanceField,

        /// <summary>
        /// Invoke instance set field
        /// </summary>
        SetInstanceField,

        /// <summary>
        /// Invoke static get property
        /// </summary>
        GetStaticProperty,

        /// <summary>
        /// Invoke static set property
        /// </summary>
        SetStaticProperty,

        /// <summary>
        /// Invoke instance get property
        /// </summary>
        GetInstanceProperty,

        /// <summary>
        /// Invoke instance set property
        /// </summary>
        SetInstanceProperty
    }

    /// <summary>
    /// This is the public enum to specify type of parameter
    /// either Direct or RetrieveFromReturnValue
    /// </summary>
    public enum ParameterToMethodType : uint
    {
        /// <summary>
        /// Directly use the value from XML.
        /// </summary>
        Direct,

        /// <summary>
        /// Retrive the return value from a previously executed ActionItem.
        /// </summary>
        RetrieveFromReturnValue
    }

    /// <summary>
    /// This class defines an item to be placed in the ActionManager list
    /// to be invoked.
    /// </summary>
    /// <remarks>
    /// An action item can be identified by its id or name.
    /// Note that integers cannot be used as names. All positive integers
    /// are reserved as identifiers of Action items.
    /// </remarks>
    public class ActionItem
    {
        #region Constructors.

        /// <summary>
        /// Creates a new Test.Uis.Utils.ActionItem instance.
        /// </summary>
        /// <param name="name">Name of this item</param>
        /// <param name="invokeType">Type of the action. See InvokeType enum for details.</param>
        /// <param name="className">Declaring class name of the method</param>
        /// <param name="memberName">Member name of the invoked target.</param>
        /// <param name="useWorkerThread">true if the ActionItem should run in the worker thread, false otherwise</param>
        /// <param name="args">Arguments to the invoked method</param>
        public ActionItem(string name, InvokeType invokeType, string className,
            string memberName, bool useWorkerThread, object[] args)
        {
            if (String.IsNullOrEmpty(memberName))
            {
                throw new ArgumentException("memberName is null or empty", "memberName");
            }
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (ActionManager.IsInvokeTypeStatic(invokeType))
            {
                if (String.IsNullOrEmpty(className))
                {
                    throw new ArgumentException("Class name is null or empty", "className");
                }
            }
            else
            {
                if (args.Length == 0)
                {
                    throw new ArgumentException(
                        "Instance invocations cannot have empty arguments.", "args");
                }
                if (args[0] == null)
                {
                    throw new ArgumentException(
                        "Instance invocations require a non-null first argument.", "args");
                }
            }

            this._name = name;
            this._className = className;
            this._methodName = memberName;
            this._args = args;
            this._invokeType = invokeType;
            this._useWorkerThread = useWorkerThread;
            this._actionItemResult = new ActionItemResult(this);
            this._id = ActionManager.Current.NextActionItemId;
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Call InvokeAction to invoke calling on the method
        /// </summary>
        /// <returns></returns>
        public void InvokeAction()
        {
            object retObject = null;
            try
            {
                // we need to do conversion if the parameter type is to be retrieved from
                // other ActionItem
                ArrayList list = new ArrayList();

                for (int i = 0; i < this._args.Length; i++)
                {
                    ParameterToMethod param;
                    object parameterValue;
                    Type convertToType;
                    TypeConverter converter;

                    param = this._args[i] as ParameterToMethod;
                    if (param == null)
                    {
                        throw new InvalidOperationException("The element is not of type ParameterToMethod");
                    }

                    // Use the string value or get it from another result.
                    if (param.ParamType == ParameterToMethodType.Direct)
                    {
                        parameterValue = param.Parameter;
                    }
                    else
                    {
                        // The only other paramter type we know of.
                        System.Diagnostics.Debug.Assert(param.ParamType == ParameterToMethodType.RetrieveFromReturnValue);
                        parameterValue = ActionManager.Current.GetInvokedMethodReturnedValue(param.Parameter);
                    }

                    // Convert to a specific type if requested.
                    if (param.ConvertToTypeName != null && param.ConvertToTypeName.Length > 0)
                    {
                        convertToType = ReflectionUtils.FindType(param.ConvertToTypeName);
                        System.Diagnostics.Debug.Assert(convertToType != null);
                        converter = TypeDescriptor.GetConverter(convertToType);
                        if (converter == null)
                        {
                            throw new InvalidOperationException("There is no converter for type " + param.ConvertToTypeName);
                        }
                        parameterValue = converter.ConvertFrom(parameterValue);
                    }
                    list.Add(parameterValue);
                }

                retObject = ReflectionUtils.InvokePropertyOrMethod(
                    this._className,
                    this._methodName,
                    (object [])list.ToArray(),
                    this._invokeType);
            }
            catch(Exception)
            {
                Logger.Current.Log("Exception occurs when executing action " +
                    "(ID=[" + ID + "], Name=[" + Name + "]).");
                // Let the topmost handler report the exception.
                throw;
            }
            this._actionItemResult.ResultObject = retObject;
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>
        /// A unique identifier for the action item.
        /// </summary>
        public int ID
        {
            get
            {
                return this._id;
            }
        }

        /// <summary>
        /// The name of the action item, possibly null.
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        /// Retrieve ActionItem result object
        /// </summary>
        /// <value></value>
        public object Result
        {
            get
            {
                return this._actionItemResult.ResultObject;
            }
        }

        /// <summary>
        /// Returns true if the ActionItem runs in a worker thread, false otherwise
        /// </summary>
        /// <value></value>
        public bool UseWorkerThread
        {
            get { return this._useWorkerThread; }
        }

        #endregion Public properties.


        #region Private fields.

        private string _name;
        private string _className;
        private string _methodName;

        // very important!
        private object[] _args;
        private int _id;

        private ActionItemResult _actionItemResult;
        private InvokeType _invokeType;

        private bool _useWorkerThread;

        #endregion Private fields.
    }

    /// <summary>
    /// Use this class to hold a list of action items and the return values
    /// of each of the invoked method.
    /// </summary>
    /// <remarks>
    /// Warning: null elements in _invokeReturnValue can mean returning null
    /// value, or the method is void. We might need one more ArrayList
    /// for return types.
    ///
    /// This class has everything static. The reason for this is that I
    /// don't want more than 1 ActionManager existing in a test case,
    /// otherwise the order of ActionItem instances might be incorrect.
    /// </remarks>
    public class ActionManager
    {
        #region Private static members

        private ArrayList _actionItemList;
        private static ActionManager s_manager = null;
        private int    _actionItemDispatchId = 0;

        #endregion

        /// <summary>
        /// initialize those internal variables
        /// </summary>
        private ActionManager ()
        {
            _actionItemList = new ArrayList ();
        }

        #region Public methods.

        /// <summary>
        /// Determines whether an invocation is expected to be on a type.
        /// </summary>
        /// <param name='invokeType'>Type of invocation.</param>
        /// <returns>
        /// true if invokeType is any of the following: StaticMethod
        /// GetStaticField, SetStaticField, GetStaticProperty,
        /// SetStaticProperty; false otherwise.
        /// </returns>
        public static bool IsInvokeTypeStatic(InvokeType invokeType)
        {
            return (
                invokeType == InvokeType.StaticMethod ||
                invokeType == InvokeType.GetStaticField ||
                invokeType == InvokeType.SetStaticField ||
                invokeType == InvokeType.GetStaticProperty ||
                invokeType == InvokeType.SetStaticProperty);
        }

        /// <summary>
        /// Add ActionItem to the internal ArrayList
        /// </summary>
        /// <param name="actionItem"></param>
        public void AddActionItemToList(ActionItem actionItem)
        {
            _actionItemList.Add(actionItem);
        }

        /// <summary>
        /// returns the enumerator to go over the list
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _actionItemList.GetEnumerator();
        }

        #endregion Public methods.

        /// <summary>
        /// returns the current ticket (Id) to ActionItem
        /// it is made internal because I don't want outsider
        /// accidentally messes this up
        /// </summary>
        internal int NextActionItemId
        {
            get
            {
                return this._actionItemDispatchId++;
            }
        }

        /// <summary>
        /// retrieve return value of an invoked ActionItem
        /// Exception: IndexOutOfRangeException will be thrown if the index is greater than all the already invoked ActionItems' index
        /// </summary>
        /// <param name="name">Name (or maybe index) of the ActionItem</param>
        /// <returns>return value desired. It is up to the value consumer to cast it to valid type.</returns>
        public object GetInvokedMethodReturnedValue(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException("name cannot be empty.");
            }

            ActionItem actionItem = FindActionItemWithNameOrId(name);

            return actionItem.Result;
        }

        /// <summary>
        /// With this one we make ActionManager static.
        /// </summary>
        public static ActionManager Current
        {
            get
            {
                if (s_manager == null)
                {
                    s_manager = new ActionManager ();
                }

                return s_manager;
            }
        }

        /// <summary>
        /// this method is useful in debugging failure in xml with RetrieveFromReturnValue
        /// </summary>
        public void DumpReturnValueList()
        {
            IEnumerator enumerator = GetEnumerator();

            Logger.Current.Log("ID\t\tName\t\tValue");
            while(enumerator.MoveNext())
            {
                ActionItem actionItem = enumerator.Current as ActionItem;
                String output = String.Format("{0}\t\t{1}\t\t{2}",
                       actionItem.ID.ToString(),
                       actionItem.Name.ToString(),
                       actionItem.Result.ToString());

                Logger.Current.Log (output);
            }
        }

        #region Private methods

        /// <summary>
        /// Find ActionItem given the name. If no ActionItem with this name is found
        /// the parameter is treated as id. If there's no good match InvalidOperationException
        /// will be thrown to notify tester to fix their testxml
        /// </summary>
        /// <param name="name">name of the ActionItem, if no named ActionItem is found this is treated as Id</param>
        /// <returns>return ActionItem reference.</returns>
        private ActionItem FindActionItemWithNameOrId(string name)
        {
            System.Diagnostics.Debug.Assert(name != null);
            System.Diagnostics.Debug.Assert(name.Length > 0);

            ActionItem possibleActionItem = null;

            IEnumerator enumerator = GetEnumerator();

            while (enumerator.MoveNext())
            {
                ActionItem actionItem = enumerator.Current as ActionItem;

                if (actionItem == null)
                {
                    string output = String.Format("Current element is not an ActionItem");

                    throw new InvalidOperationException(output);
                }

                // the first one in the list with ActionItem.Name == name
                // is returned
                if (actionItem.Name == name)
                {
                    return actionItem;
                }

                // we store up the possibleActionItem so that if no ActionItem is found
                // to have the same name, this is treated as id
                if (actionItem.ID.ToString() == name && possibleActionItem == null)
                {
                    possibleActionItem = actionItem;
                }
            }

            // no ActionItem with the name specified is found, but a candidate
            // ActionItem with the specified ID is. we will retrun it right away
            if (possibleActionItem != null)
            {
                return possibleActionItem;
            }

            // neither ActionItem name nor id is the same as name param.
            // throw exception so that tester knows to fix the testxml
            string message = String.Format("Cannot find the return value from ActionItem named [{0}]. Check the testxml", name);

            throw new InvalidOperationException(message);
        }

        #endregion
    }

    /// <summary>
    /// ActionItemResult: associates a return value with ActionItem.ID and ActionItem.Name
    /// In the original design return values are stored as a raw list. The index on the
    /// arraylist corresponds to the ActionItem id. This is not flexible in the more
    /// complicated design (e.g. out of order item dispatching.)
    /// </summary>
    internal class ActionItemResult
    {
        /// <summary>
        /// Create an ActionItemResult
        /// </summary>
        /// <param name="actionItem">associate this ActionItemResult with an ActionItem</param>
        internal ActionItemResult(ActionItem actionItem)
        {
            this._actionItem = actionItem;
            this._retObject = null;
            this._validResultObject = false;
        }

        /// <summary>
        /// setter / getter of the return value object
        /// if validResultObject flag is false, the ActionItem
        /// is likely not run, and an InvalidOperationException will be
        /// thrown so that the tester knows what going on.
        /// </summary>
        /// <value></value>
        internal object ResultObject
        {
            get
            {
                if (!this._validResultObject)
                {
                    string message = String.Format("Retrieve return value from an invalid ActionItemResult. ActionItem name [{0}] ActionItem id [{1}]", this._actionItem.Name, this._actionItem.ID.ToString());

                    throw new InvalidOperationException(message);
                }
                return this._retObject;
            }
            set
            {
                this._retObject = value;
                this._validResultObject = true;
            }
        }

        /// <summary>
        /// return the flag for a valid return object
        /// </summary>
        /// <value></value>
        internal bool IsValidResultObject
        {
            get { return this._validResultObject; }
        }

        #region Private members
        private object _retObject = null;
        private bool _validResultObject = false;
        private ActionItem _actionItem;

        #endregion
    }

    /// <summary>
    /// Class to bind parameter and type together
    /// </summary>
    public class ParameterToMethod
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new ParameterToMethod instance.
        /// </summary>
        /// <param name="arg">
        /// The value of the argument or the name of the action to take it from.
        /// </param>
        /// <param name="type">The type of parameter retrieval to perform.</param>
        /// <param name="convertToTypeName">
        /// The name of the type to convert the value to. May be blank or
        /// null to specify that no convertion takes place.
        /// </param>
        public ParameterToMethod(string arg, ParameterToMethodType type,
            string convertToTypeName)
        {
            this._arg = arg;
            this._type = type;
            this._convertToTypeName = convertToTypeName;
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>
        /// The name of the type to convert the value to. May be blank or
        /// null to specify that no convertion takes place.
        /// </summary>
        public string ConvertToTypeName
        {
            get { return this._convertToTypeName; }
        }

        /// <summary>
        /// retrieve param value
        /// </summary>
        public string Parameter
        {
            get { return this._arg; }
        }

        /// <summary>
        /// retrieve param type. It can be either Direct or RetrieveFromReturnValue
        /// </summary>
        public ParameterToMethodType ParamType
        {
            get { return this._type; }
        }

        #endregion Public properties.

        #region Private fields.

        /// <summary>The value of the argument or the name of the action to take it from.</summary>
        private string _arg;

        /// <summary>The type of parameter retrieval to perform.</summary>
        private ParameterToMethodType _type;

        /// <summary>The name of the type to convert the value to.</summary>
        private string _convertToTypeName;

        #endregion Private fields.
    }
}
