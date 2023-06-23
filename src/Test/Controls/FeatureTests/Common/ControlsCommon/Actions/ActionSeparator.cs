using System;
using System.Windows;
using System.Windows.Media;
using System.Reflection;

using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// Separating parsing from normal logic
    /// Any class deriving from this class can only implement the Do(object) method.
    /// 
    /// Sample:
    /// How to use the class to create a SetPropertyAction:
    /// 1. define a class named SetPropertyAction deriving from this class.
    /// 2. define properties: Property and Value.
    /// 3. attach Parser attribute with properties' name.
    ///     the parser will use each property to parser each action parameter.
    /// 4. implement the Do(object) function.
    /// <code>
    ///     [Parser("Property", "Value")]
    ///     public class SetPropertyAction : ActionSeparator
    ///     {
    ///         public override void Do(object testObject)
    ///         {
    ///             // code here
    ///         }
    ///         public string Property
    ///         {
    ///             // ...
    ///         }
    ///         public object Value
    ///         {
    ///             // ...
    ///         }
    ///     }
    /// </code>
    /// Now you can use it in xtc
    /// <code>
    ///     <Action Name='SetPropertyAction'>
    ///         <Parameter Value='Margin'/>
    ///         <Parameter Value='0,0,2,1'/>
    ///     </Action>
    /// </code>
    /// ActionSeparator will use the parser to parse action parameters.
    /// After parsing is done, SetPropertyAction.Property will be set to "Margin",
    /// and Value will be set to "0,0,2,1".
    /// And then ActionSeparator will call SetPropertyAction.Do(object) to do the action.
    /// </summary>
    public abstract class ActionSeparator : IAction, ITestStep
    {
        #region IAction Members

        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            object[] parsers = GetType().GetCustomAttributes(typeof(ParserAttribute), true);
            foreach (object parserObject in parsers)
            {
                ParserAttribute parser = (ParserAttribute)parserObject;
                parser.Parse(this, actionParams);
            }

            // convert it to object to call Do(object) instead of self.
            Do((object)frmElement);
        }

        #endregion

        #region ITestStep Members

        public abstract void Do(object testObject);

        #endregion
    }

    /// <summary>
    /// parser for action properties.
    /// </summary>
    public class ParserAttribute : Attribute
    {
        #region Constructor

        public ParserAttribute(params string[] properties)
        {
            if (properties == null || properties.Length == 0)
                throw new ArgumentException("properties should be specified");

            _properties = properties;
        }

        #endregion

        #region Parse

        public void Parse(object parseObject, params object[] parseParams)
        {
            // verify parameters are correct
            if (parseObject == null)
                throw new ArgumentNullException("target");
            if (parseParams == null || parseParams.Length == 0)
                return;

            // get the value string array
            object[] values = parseParams;
            if (values.Length > Properties.Length)
                throw new ArgumentException("Too many values. Max value number is " + Properties.Length);

            // parse each string against the corresponding property name
            Type type = parseObject.GetType();
            for (int i = 0; i < values.Length; ++i)
            {
                string property = Properties[i];
                PropertyInfo pi = type.GetProperty(property);
                if (pi == null)
                    throw new ArgumentException("Property " + property + " not exist in " + type);

                object value = XmlHelper.ConvertToType(pi.PropertyType, values[i]);
                pi.SetValue(parseObject, value, new object[0]);
            }
        }

        #endregion

        #region Properties

        public string[] Properties
        {
            get { return _properties; }
        }

        private string[] _properties;

        #endregion
    }
}
