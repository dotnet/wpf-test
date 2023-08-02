using System;
using System.Windows;
using System.Windows.Media;
using System.Reflection;

using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// Class to invoke a Method on a Control. A sample xtc example:
    ///   <Action Name="ControlMethodInvokeAction">
    ///   	<Parameter Value="sv1" />
    ///   	<Parameter Value="PageRight" />
    ///   </Action>
    /// Parameters can also be added.  Provide both the type and values.  For example
    ///      <Parameter Value="System.Boolean" />
    ///      <Parameter Value="True" />
    /// </summary>
    public class ControlMethodInvokeAction : IAction
    {
        /// <summary>
        /// invoke a method by reflection. Parameter type and values can be provided if needed.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string[] args = actionParams as string[];
            int count = args.Length - 2;
            if (args == null || (count % 2) != 0)
            {
                throw new ArgumentException("Must provide type and value for each param used by the method! ");
            }
            string controlName = args[0];
            string methodName = args[1];

            TestLog.Current.LogStatus("Control.Name: [" + controlName + "]");
            TestLog.Current.LogStatus("Control Method: [" + methodName + "]");

            Visual tempResult = LogicalTreeHelper.FindLogicalNode(frmElement, controlName) as Visual;
            if (tempResult == null)
            {
                throw new NullReferenceException("No Control " + controlName + " Found!");
            }

            Type controlType = tempResult.GetType();

            TestLog.Current.LogStatus("Control.Type: [" + controlType.ToString() + "]");
	    
            
            MethodInfo mInfo = controlType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase); // Get MethodInfo by reflection.
            if (mInfo == null)
            {
                throw new ArgumentException("Method " + methodName + " does not exists in type " + controlType.ToString() );
            }

            object[] parameters = new object[count / 2];
            for (int i = 0; i < count / 2; i++)    // Parse method parmaters
            {
                Type targetType = Type.GetType(args[2 + i * 2],true);
                parameters[i] = XmlHelper.ConvertToType(targetType, args[3 + i * 2]);
            }
            mInfo.Invoke(tempResult, parameters);                       
        }
    }

}
