using Microsoft.Test.Logging;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Threading;

// Added for automation input
// Shouldn't need to live here, currently for testing
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel.Utilities;
using System.Threading;
using Microsoft.Test.Input;

using Avalon.Test.ComponentModel.Utilities.VScanTools;

// Daniel's List of Things To Do:
// - Feature Creep
//     - Overload event test and allow testing of series of events. (Allows for testing sequence such as during UI interaction)
//     - Allow testing for non-triggering of events.
//     - Create methodInvoke action
// - Refactor
//     - Consider move some generic object property code that is not CM specific out into a different file. Or perhaps this file
//       should contain that sort of thing?
//     - Decide on whether PMEUtils and Scenarios using them should coexist in same file or split.
//     - Get rid of ControlPropertyTest util and scenario - the are subsumed by the ActionValidation scenario.
//     - Get rid of testing temp code such as some stub actions and validations.
//     *** KEEP EVERYTHING IN HERE FOR NOW, REFACTOR LATER ***
// - Error Handling
//     - Null checks
//     - Valid XML which does not follow my informal format.

namespace Avalon.Test.ComponentModel.UnitTests
{
    // These UnitTests do not contain attribute information because we don't want reflection based mechanism to try and run them,
    // since they need specific variation information to understand what to do.

    /// <summary>
    /// The ControlActionValidationUnitTest does a sequence of actions and validations. The test makes no sense without
    /// variation information, so the unit test reports an unknown result if it is called without a variation node.
    /// </summary>
    public class ControlTestActionValidationUnitTest : IUnitTest
    {
        /// <summary>
        /// Implementation of IUnitTest interface. This method is called to perform a series of actions and validations.
        /// </summary>
        /// <param name="passedTestElement">The control to test.</param>
        /// <param name="variation">Variation node containing information about the actions and validations to perform.</param>
        public TestResult Perform(object passedTestElement, XmlElement variation)
        {
            if (variation == null)
            {
                TestLog.Current.LogEvidence("No variation given, test doesn't make sense without");
                return TestResult.Unknown;
            }

            QueueHelper.WaitTillQueueItemsProcessed();

            XmlElement validationselement = variation["Validations"];
            string[] validations = PMEUtils.Current.NamesFromXml(validationselement);

            ArrayList arrayList = new ArrayList();

            XmlElement actionElement = null;

            object[] actionsParameters = null;

            XmlElement actionselement = variation["Actions"];
            string[] actions = null;


            if (actionselement != null)
            {
                actions = PMEUtils.Current.NamesFromXml(actionselement);

                actionElement = actionselement.FirstChild as XmlElement;

                while (actionElement != null)
                {
                    arrayList.Add(PMEUtils.Current.ValuesFromXml(actionElement));
                    actionElement = actionElement.NextSibling as XmlElement;
                }
                actionsParameters = arrayList.ToArray(typeof(object[])) as object[];

                arrayList.Clear();
            }
            else
            {
                TestLog.Current.LogEvidence("No actions specified");
            }

            XmlElement validationElement = validationselement.FirstChild as XmlElement;
            while (validationElement != null)
            {
                arrayList.Add(PMEUtils.Current.ValuesFromXml(validationElement));
                validationElement = validationElement.NextSibling as XmlElement;
            }
            object[] validationsParameters = arrayList.ToArray(typeof(object[])) as object[];

            if (PMEUtils.Current.TestControlActionValidation(passedTestElement, actions, actionsParameters, validations, validationsParameters))
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
    }

    /// <summary>
    /// The ControlTestEventUnitTest tests the triggering of an event for a control. The test makes no sense without variation information, so the
    /// unit test reports an unknown result if it is called without a variation node.
    /// </summary>
    /// A sample variation node is:
    /// <VARIATION ID="1">
    ///     <Control Name="Thumb" />
    ///     <Event Name="DragDelta" />
    ///     <Actions>
    ///         <Action Name="ClickAction" />
    ///         <Action Name="MoveAction" />
    ///     </Actions>
    ///     <Validations>
    ///         <Validation Name="DragDeltaArgsValidation" />
    ///     </Validations>
    /// </VARIATION>
    /// 
    /// 
    public class ControlTestEventUnitTest : IUnitTest
    {
        /// <summary>
        /// Implementation of IScenario interface. This method is called to perform testing of the event.
        /// </summary>
        /// <param name="passedTestElement">The control to test.</param>
        /// <param name="variation">Variation node containing information about the event to test.</param>
        public TestResult Perform(object passedTestElement, XmlElement variation)
        {
            if (variation == null)
            {
                TestLog.Current.LogEvidence("No variation given, test doesn't make sense without");
                return TestResult.Unknown;
            }

            XmlElement eventElement = variation["Event"];
            string eventName = eventElement.GetAttribute("Name");

            QueueHelper.WaitTillQueueItemsProcessed();

            XmlElement actionselement = variation["Actions"];
            string[] actions = PMEUtils.Current.NamesFromXml(actionselement);

            XmlElement validationselement = variation["Validations"];
            string[] validations = PMEUtils.Current.NamesFromXml(validationselement);

            ArrayList arrayList = new ArrayList();

            XmlElement actionElement = actionselement.FirstChild as XmlElement;
            while (actionElement != null)
            {
                arrayList.Add(PMEUtils.Current.ValuesFromXml(actionElement));
                actionElement = actionElement.NextSibling as XmlElement;
            }
            object[] actionsParameters = arrayList.ToArray(typeof(object[])) as object[];

            arrayList.Clear();

            XmlElement validationElement = validationselement.FirstChild as XmlElement;
            while (validationElement != null)
            {
                arrayList.Add(PMEUtils.Current.ValuesFromXml(validationElement));
                validationElement = validationElement.NextSibling as XmlElement;
            }
            object[] validationsParameters = arrayList.ToArray(typeof(object[])) as object[];

            if (PMEUtils.Current.TestControlEvent(passedTestElement, eventName, actions, actionsParameters, validations, validationsParameters))
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
    }

    public class ControlTestMethodUnitTest : IUnitTest
    {

        public TestResult Perform(object passedTestElement, XmlElement variation)
        {
            if (variation == null)
            {
                TestLog.Current.LogEvidence("No variation given, test doesn't make sense without");
                return TestResult.Unknown;
            }

            XmlElement methodElement = variation["Method"];

            ArrayList arrayList = new ArrayList();

            string methodName = methodElement.GetAttribute("Name");

            XmlElement methodParam = methodElement.FirstChild as XmlElement;
            while (methodParam != null)
            {
                arrayList.Add(PMEUtils.Current.ValuesFromXml(methodParam));
                methodParam = methodParam.NextSibling as XmlElement;
            }
            object[] methodParams = arrayList.ToArray(typeof(object[])) as object[];

            arrayList.Clear();

            QueueHelper.WaitTillQueueItemsProcessed();

            XmlElement actionselement = variation["Actions"];
            string[] actions = PMEUtils.Current.NamesFromXml(actionselement);

            XmlElement validationselement = variation["Validations"];
            string[] validations = PMEUtils.Current.NamesFromXml(validationselement);

            XmlElement actionElement = actionselement.FirstChild as XmlElement;
            while (actionElement != null)
            {
                arrayList.Add(PMEUtils.Current.ValuesFromXml(actionElement));
                actionElement = actionElement.NextSibling as XmlElement;
            }
            object[] actionsParameters = arrayList.ToArray(typeof(object[])) as object[];

            arrayList.Clear();

            XmlElement validationElement = validationselement.FirstChild as XmlElement;
            while (validationElement != null)
            {
                arrayList.Add(PMEUtils.Current.ValuesFromXml(validationElement));
                validationElement = validationElement.NextSibling as XmlElement;
            }
            object[] validationsParameters = arrayList.ToArray(typeof(object[])) as object[];

            if (PMEUtils.Current.TestControlMethod(passedTestElement, methodName, methodParams, actions, actionsParameters, validations, validationsParameters))
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
    }

}

namespace Avalon.Test.ComponentModel.Utilities
{
    /// <summary>
    ///  Component Model Utilities for generalized Property, Method, and Event testing.
    /// </summary>
    /// You might notice that there is not a method for method testing specifically. It seemed like an overweight wrapper. The suggested
    /// solution is to create an action which invokes the method, and the ActionValidation method can be used. (It seems that a simple wrapper
    /// could have been writted for this) However, if part of the validation includes looking for an event to have been fired, the Event method
    /// can be used, again using the method invoke action in the actions list to trigger. 
    public class PMEUtils
    {
        #region CLASS_VARIABLES
        private object eventArgs = null;
        private bool eventTriggered = false;
        private object eventSender = null;
        private static PMEUtils _current = null;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public PMEUtils()
        {
            _current = this;
        }

        /// <summary>
        /// Get a handle on the current PMEUtils object.
        /// </summary>
        /// <value>The current PMEUtils object.</value>
        public static PMEUtils Current
        {
            get
            {
                if (_current == null)
                    new PMEUtils();
                return _current;
            }
        }

        #region HELPERS

        #region XML_HELPERS

        /// <summary>
        /// Takes an XML element of a certain format and extracts a string array of names.
        /// </summary>
        /// <param name="stringsElement">XML Element to use.</param>
        /// <returns>String array of names.</returns>
        /// Format:
        /// <Actions>
        ///     <Action Name="ClickAction" />
        ///     <Action Name="MoveAction" />
        /// </Actions>
        public string[] NamesFromXml(XmlElement stringsElement)
        {
            ArrayList arrayList = new ArrayList();
            XmlElement stringElement = stringsElement.FirstChild as XmlElement;
            while (stringElement != null)
            {
                arrayList.Add(stringElement.GetAttribute("Name"));
                stringElement = stringElement.NextSibling as XmlElement;
            }
            return arrayList.ToArray(typeof(string)) as string[];
        }

        /// <summary>
        /// Takes an XML element where it has a series of children each of which have a single attribute,
        /// and extracts a string array of those values.
        /// </summary>
        /// <param name="stringsElement">XML Element.</param>
        /// <returns>String array of values.</returns>
        /// Format:
        /// <Action Name="MoveAction">
        ///     <Parameter Value="5" />
        ///     <Parameter Vaule="10" />
        /// </Action>
        /// Would return { "5", "10" }
        public string[] ValuesFromXml(XmlElement stringsElement)
        {
            ArrayList arrayList = new ArrayList();
            
            // Ignore comments. Don't try to extract attributes from the comments node
            foreach(XmlNode xmlNode in stringsElement.ChildNodes)
            {
                if(xmlNode.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                XmlElement stringElement = xmlNode as XmlElement;
                if(stringElement != null)
                {
                    XmlAttributeCollection attributes = stringElement.Attributes;
                    if (attributes.Count != 1)
                    {
                        throw new Exception("Parameters are only supposed to have one attribute per!");
                    }

                    arrayList.Add(attributes[0].Value);
                }
            }
            return arrayList.ToArray(typeof(string)) as string[];
        }
        #endregion

        /// <summary>
        /// Currently a placeholder for a more robust cleanup utility.
        /// </summary>
        /// <param name="control">Control to mouseup over.</param>
        private static void Cleanup(object control)
        {
            // To avoid exception, we do not perform UserInput.MouseLeftUp on not yet rendered ContextMenu
            if (!(control is ContextMenu))
            {
                UserInput.MouseLeftUp(control as FrameworkElement);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }

        /// <summary>
        /// Executes a series of actions.
        /// </summary>
        /// <param name="control">Control to act upon.</param>
        /// <param name="actions">Names of actions.</param>
        /// <param name="actionParameters">Parameters to pass to actions.</param>
        public void DoActions(object control, string[] actions, object[] actionParameters)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            // To avoid exception, we do not perform UserInput.MouseLeftUp on not yet rendered ContextMenu
            if (!(control is ContextMenu))
            {
                // Right now I always do this manual cleanup
                UserInput.MouseLeftUp(control as FrameworkElement);
            }

            
            int i = 0;

            foreach (string actionName in actions)
            {
                TestLog.Current.LogStatus("---------------------------");
                TestLog.Current.LogStatus("ActionName - "+actionName);
                Execute(actionName, control as FrameworkElement, actionParameters[i]);
                QueueHelper.WaitTillQueueItemsProcessed();
                i++;
            }

            QueueHelper.WaitTillQueueItemsProcessed();
        }

        /// <summary>
        /// Execute an action using BeginInvoke.
        /// </summary>
        /// <param name="actionName">Action name.</param>
        /// <param name="frameworkElement">Element to act upon.</param>
        /// <param name="actionParams">Parameters for the action.</param>
        public void Execute(string actionName, FrameworkElement frameworkElement, object actionParams)
        {
            object[] obj = new object[] { actionName, frameworkElement, actionParams };
            
            ExecuteAction(obj);
        
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        /// <summary>
        /// Execute a series of actions using BeginInvoke.
        /// </summary>
        /// <param name="actionName">Action names.</param>
        /// <param name="frameworkElement">Element to act upon.</param>
        /// <param name="actionParams">Series of parameters for actions.</param>
        private void Execute(string[] actionName, FrameworkElement frameworkElement, object[] actionParams)
        {
            for (int i = 0; i<actionName.Length; i++)
            {
                object[] obj = new object[] { actionName[i], frameworkElement, actionParams[i] };

                ExecuteAction(obj);

                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }

        /// <summary>
        /// Execute a single action.
        /// </summary>
        /// <param name="args">Arguments in a special format.</param>
        /// <returns>The arguments.</returns>
        private object ExecuteAction(object args)
        {
            object[] obj = args as object[];
            string actionName = obj[0] as string;
            FrameworkElement frameworkElement = obj[1] as FrameworkElement;
            object[] actionParams = obj[2] as object[];
            IAction action = ObjectFactory.CreateObjectFromTypeName(actionName) as IAction;
            if (action == null)
            {
                throw new Exception("Action was not found.");
            }
            action.Do(frameworkElement, actionParams);
            return args;
        }

        /// <summary>
        /// Create an object of the appropriate type from a given string.
        /// </summary>
        /// <param name="value">String representation of the object to create.</param>
        /// <param name="type">Type of the object to create.</param>
        /// <returns>The created object, null if was unable to create it.</returns>
        public object StringToType(string value, Type type)
        {
	    // To support Nullable type null value, we use Empty string as null for Nullable type.
            if (String.IsNullOrEmpty(value) || type == null)
            {
                return null;
            }

            // If the target type is just object then the type is compatible.
            if (type == typeof(object))
            {
                return value;
            }

            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);

            if (typeConverter.CanConvertFrom(typeof(string)))
            {
                return typeConverter.ConvertFromInvariantString(value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Verify the value of a given property.
        /// This method works only for CLR properties.  Attached DPs does not have CLR correspondent property.
        /// We should use static GetX() methods.
        /// Example:
        /// XTC validation:
        /// <PropertyValition PropertyName="Selector.IsSelected" Value="true"/>
        /// Code to get the value:
        /// m = GetMethosInfo("Selector", "Get"+"IsSelected", static | public )
        /// m.Invoke(button);
        /// </summary>
        /// <param name="obj">The object to look at.</param>
        /// <param name="propertyName">The name of the property to look at.</param>
        /// <param name="expectedValue">The expected value for the property.</param>
        /// <returns>True if value matched expectation, false otherwise.</returns>
        public bool VerifyProperty(object obj, string propertyName, object expectedValue)
        {
            if (String.IsNullOrEmpty(propertyName) || obj == null)
                return false;

            PropertyInfo pInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (null == pInfo)
            {
                // Can't find specified property on the resource object.
                throw new Exception("Could not find the property " + propertyName + " in the control");
            }

            object curValue = pInfo.GetValue(obj, new object[0]);

            if (curValue == null)
            {
                TestLog.Current.LogStatus("Actual value: null");
                return expectedValue == null;
            }
            else
            {
                TestLog.Current.LogStatus("Actual value: " + curValue.ToString());
            }

            return ObjectFactory.CompareObjects(curValue, expectedValue);
        }

        public static bool ValidateProperty(FrameworkElement targetElement, string propertyName, string propertyValue)
        {
            if (targetElement == null)
            {
                throw new ArgumentNullException("targetElement");
            }

            if (String.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            if (String.IsNullOrEmpty(propertyValue))
            {
                throw new ArgumentNullException("propertyValue");
            }


            PropertyInfo pInfo = targetElement.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Type valueType = pInfo.PropertyType;

            return PMEUtils.Current.VerifyProperty(targetElement, propertyName, PMEUtils.Current.StringToType((string)propertyValue, valueType));
        }

        #endregion

        #region PME_TEST_METHODS
        /// <summary>
        /// Generic event handler we can hook up to any event which sends arguments deriving from EventArgs. Saves off information
        /// about the sender and the arguments.
        /// </summary>
        /// <param name="sender">Object which raised the event.</param>
        /// <param name="args">The event arguments.</param>
        /// I don't think this will work with RoutedEventArgs because they don't derive from EventArgs? (Not sure about this)
        private static void GenericEventHandler(object sender, EventArgs args)
        {
            TestLog.Current.LogEvidence("Generic event handler triggered by " + sender.ToString() + ".");

            TestLog.Current.LogEvidence("EventArgs.ToString(): " + args.ToString());

            if (args is RoutedEventArgs)
            {
                RoutedEventArgs routedEventArgs = args as RoutedEventArgs;

                TestLog.Current.LogEvidence("RoutedEventArgs.Name: " + routedEventArgs.RoutedEvent.Name);
            }

            // Want to move validation back to caller, just have log and such in here
            _current.eventArgs = args;
            _current.eventTriggered = true;
            _current.eventSender = sender;
        }

        /// <summary>
        /// Perform a series of actions and validations upon a control.
        /// </summary>
        /// <param name="control">Control to test.</param>
        /// <param name="actions">Actions to commit.</param>
        /// <param name="actionParams">Parameters for each of the actions.</param>
        /// <param name="validations">Validatons to make.</param>
        /// <param name="validationParams">Parameters for each of the validatons.</param>
        /// <returns>True if passed validations, false otherwise.</returns>
        public bool TestControlActionValidation(object control, string[] actions, object[] actionParams, string[] validations, object[] validationParams)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            object validationValue = ((FrameworkElement)control).FindName("ValidationValue");

            if (actions != null)
            {

                DoActions(control, actions, actionParams);

                QueueHelper.WaitTillQueueItemsProcessed();

            }

            int i = 0;

            foreach (string validationName in validations)
            {
                IValidation validation = ObjectFactory.CreateObjectFromTypeName(validationName) as IValidation;
                if (!validation.Validate(control, validationParams[i], validationValue))
                {
                    Cleanup(control);
                    return false;
                }
                i++;
            }

            Cleanup(control);
            return true;
        }

        /// <summary>
        /// Tests the event on the given control. Hooks up a handler to the event, does the actions to trigger the event, and runs the validations.
        /// </summary>
        /// <param name="control">The control to test.</param>
        /// <param name="eventName">Name of the event to hook the handler up to.</param>
        /// <param name="actions">Actions to perform, must implement IAction.</param>
        /// <param name="actionParams"></param>
        /// <param name="validations">Validations to perform, must implement IValidation.</param>
        /// <param name="validationParams"></param>
        /// <returns>True if event behaves as expected, false otherwise.</returns>
        /// 


        public bool TestControlEvent(object control, string eventName, string[] actions, object[] actionParams, string[] validations, object[] validationParams)
        {
            // Make sure properly reset.
            eventArgs = null;
            eventTriggered = false;
            eventSender = null;

            object eventTarget = FindTarget(control, "EventTarget");
            object validationValue = ((FrameworkElement)control).FindName("ValidationValue");

            // attach handler
            // Creates a bitmask based on BindingFlags.
            BindingFlags myBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            Type myTypeBindingFlags = eventTarget.GetType();
            EventInfo myEventBindingFlags = myTypeBindingFlags.GetEvent(eventName, myBindingFlags);
            Type myEventHandlerType = myEventBindingFlags.EventHandlerType;
            MethodInfo myMethodInfo = this.GetType().GetMethod("GenericEventHandler", myBindingFlags);

            Delegate myDelegate = Delegate.CreateDelegate(myEventHandlerType, myMethodInfo);
            myEventBindingFlags.AddEventHandler(eventTarget, myDelegate);

            object eventActionTarget = FindTarget(control, "EventActionTarget");

            DoActions(eventActionTarget, actions, actionParams);

            // Remove the event handler... why do I need to do this. INVESTIGATE!
            myEventBindingFlags.RemoveEventHandler(eventTarget, myDelegate);

            if (!eventTriggered)
            {
                TestLog.Current.LogEvidence("Event handler was not triggered by actions.");
                return false;
            }

            if (eventTarget != eventSender)
            {
                TestLog.Current.LogEvidence("Event was sent by a source other than the control under test.");
                return false;
            }

            int i = 0;
            foreach (string validationName in validations)
            {
                IValidation validation = ObjectFactory.CreateObjectFromTypeName(validationName) as IValidation;
                // 

                if (!validation.Validate(control, validationParams[i], validationValue, eventArgs))
                {
                    Cleanup(eventActionTarget);
                    return false;
                }
                i++;
            }

            Cleanup(control);

            TestLog.Current.LogStatus("Repeat actions after removing the event handler and verify event not fired");
            eventTriggered = false;

            DoActions(eventActionTarget, actions, actionParams);

            if (eventTriggered)
            {
                TestLog.Current.LogEvidence("Event was triggered by actions, after removing the event handler.");
                return false;
            }

            TestLog.Current.LogStatus("Verified event not fired after event handler removed");

            return true;
        }

        /// <summary>
        /// Find element with given name
        /// Search both Logical/Visual tree as well as ContextMenu/ToolTip
        /// properties if the given object is FrameworkElement
        /// 
        /// We are using FindName to find validationValue because it returns null when target is not found.
        /// Warning: FindName does not find ContextMenu and ToolTip because they are properties.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="targetName"></param>
        /// <returns></returns>
        private object FindTarget(object control, string targetName)
        {
            object target = null;

            target = LogicalTreeHelper.FindLogicalNode(control as DependencyObject, targetName);

            if (target == null)
            {
                target = VisualTreeUtils.FindPartByName(control as FrameworkElement, targetName) as FrameworkElement;
            }

            if (control is FrameworkElement)
            {
                if ((control as FrameworkElement).ContextMenu != null)
                {
                    if ((control as FrameworkElement).ContextMenu.Name == targetName)
                        target = (control as FrameworkElement).ContextMenu;
                }

                if ((control as FrameworkElement).ToolTip != null)
                {
                    if (((control as FrameworkElement).ToolTip as ToolTip).Name == targetName)
                        target = (control as FrameworkElement).ToolTip;
                }
            }


            if (target == null)
                target = control;

            return target;
        }

        /// <summary>
        /// Tests the method on the given control. Does a series of actions, calls the method, and runs the validations.
        /// </summary>
        /// <param name="control">The control to test.</param>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="methodParams">Parameters for the method.</param>
        /// <param name="actions">Names of actions to perform.</param>
        /// <param name="actionParams">Parameters for each action.</param>
        /// <param name="validations">Names of validations to perform.</param>
        /// <param name="validationParams">Parameters for each validation.</param>
        /// <returns>True if validations pass, false otherwise.</returns>
        public bool TestControlMethod(object control, string methodName, object[] methodParams, string[] actions, object[] actionParams, string[] validations, object[] validationParams)
        {
            object methodReturn = null;

            DoActions(control, actions, actionParams);

            // Build array of types
            ArrayList arrayList = new ArrayList();

            foreach (object methodParam in methodParams)
            {
                arrayList.Add(methodParam.GetType());
            }
            Type[] typeParams = (Type[])arrayList.ToArray(typeof(Type));

            MethodInfo myMethodInfo = control.GetType().GetMethod(methodName, typeParams);

            // If empty params, need to make null in accordance with invoke format
            if ((methodParams != null) && (methodParams.Length == 0))
            {
                methodParams = null;
            }

            myMethodInfo.Invoke(control, methodParams);

            QueueHelper.WaitTillQueueItemsProcessed();

            int i = 0;
            foreach (string validationName in validations)
            {
                IValidation validation = ObjectFactory.CreateObjectFromTypeName(validationName) as IValidation;
                // 

                if (!validation.Validate(control, validationParams[i], methodReturn))
                {
                    Cleanup(control);
                    return false;
                }
                i++;
            }

            Cleanup(control);
            return true;
        }

        #endregion
    }
}

// These are just for testing... they shouldn't live here
namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// Perform a mouseclick on the control.
    /// </summary>
    public class ClickAction : IAction
    {
        /// <summary>
        /// Perform the action.
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">Parameters for the action. In this case irrelevant.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.MouseLeftDown(frmElement as FrameworkElement);
        }
    }

    /// <summary>
    /// Perform a mousemove over the control.
    /// </summary>
    public class MoveAction : IAction
    {
        /// <summary>
        /// Perform the action.
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">Parameters for the action. In this case irrelevant.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.MouseMove(frmElement as FrameworkElement, 10, 10);
        }
    }

    /// <summary>
    /// Set a property upon the control.
    /// </summary>
    public class ControlPropertyAction : IAction
    {
        /// <summary>
        /// Perform the action.
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">Parameters for the action. First is the name of the property to set, second is the object to set it to.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string propertyName = (string)actionParams[0];
            object setValue = actionParams[1];
            PropertyInfo pInfo = frmElement.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Type valueType = pInfo.PropertyType;
            if (valueType == setValue.GetType())
            {
                ObjectFactory.SetObjectPropertyToObject(frmElement, propertyName, setValue);
            }
            else if (setValue.GetType() == typeof(string))
            {
                ObjectFactory.SetObjectPropertyToObject(frmElement, propertyName, PMEUtils.Current.StringToType((string)setValue, valueType));
            }
            else
            {
                throw new Exception("Invalid set of arguments");
            }
        }
    }

    /// <summary>
    /// Set a property upon the control.
    /// </summary>
    public class ControlPropertyActionById : IAction
    {
        /// <summary>
        /// Gets the FrameworkElement with a given Id and sets a property on it.
        /// The XTC will look something like this:
        /// <Action Name="ControlPropertyActionById">
        ///     <Parameter Value="toolbar1" />
        ///     <Parameter Value="IsOverflowOpen" />
        ///     <Parameter Value="false" />
        /// </Action>
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">Parameters for the action. First is the Id of the Frameworkelement to set,
        /// second is the name of the property to set, third is the object to set it to.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string elementId = actionParams[0] as String;
            if (elementId != "")
            {
                frmElement = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as FrameworkElement;
            }
            
            string propertyName = (string)actionParams[1];
            object setValue = actionParams[2];
            PropertyInfo pInfo = frmElement.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Type valueType = pInfo.PropertyType;
            if (valueType == setValue.GetType())
            {
                ObjectFactory.SetObjectPropertyToObject(frmElement, propertyName, setValue);
            }
            else if (setValue.GetType() == typeof(string))
            {
                ObjectFactory.SetObjectPropertyToObject(frmElement, propertyName, PMEUtils.Current.StringToType((string)setValue, valueType));
            }
            else
            {
                throw new Exception("Invalid set of arguments");
            }
        }
    }
}

// PME Util Validations need to take params of form (control, params) where params is an array of arguments. Limitation to current code, there may be
// a more complicated way to take as constant list, but is it really a big deal either way? Summary, though, is that we have to promise we call
// validations in a certain way, so you have to make sure you take that signature. (Advance solution involves changing xml representation of validation
// to allow arbitrary construction to match validation format requirement)
namespace Avalon.Test.ComponentModel.Validations
{
    /// <summary>
    /// Validate the value of a control's property.
    /// </summary>
    public class ControlPropertyValidation : IValidation
    {
        /// <summary>
        /// Perform the validation.
        /// </summary>
        /// <param name="validateParams">First is the control, second is an array within which is the name of the property and the expected value.</param>
        /// <returns>True if validation passed, false otherwise.</returns>
        /// assumption is that the second element of validateParams is array with first element name and second value.
        public bool Validate(params object[] validateParams)
        {
            TestLog.Current.LogStatus("ValidationName - ControlPropertyValidation");
            object control = validateParams[0];
            string propertyName = (string)(validateParams[1] as Array).GetValue(0);
            object expectedValue = (validateParams[1] as Array).GetValue(1);
            object validationValue = validateParams.Length < 3 ? null : validateParams[2]; // Read the third param if exist

            if (validationValue != null)
            {
                expectedValue = validationValue;
            }

            PropertyInfo pInfo = control.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Type valueType = pInfo.PropertyType;
            if (valueType == expectedValue.GetType() || validationValue != null)
            {
                return PMEUtils.Current.VerifyProperty(control, propertyName, expectedValue);
            }
            else if (expectedValue.GetType() == typeof(string))
            {
                return PMEUtils.Current.VerifyProperty(control, propertyName, PMEUtils.Current.StringToType((string)expectedValue, valueType));
            }
            else
            {
                throw new Exception("Invalid set of arguments");
            }
        }
    }

    /// <summary>
    /// Validate the value of a control's property by Id.
    /// </summary>
    public class ControlPropertyValidationById : IValidation
    {
        /// <summary>
        /// Perform the validation.
        /// The XTC would look something like the following:
        /// <Validation Name="ControlPropertyValidationById">
        ///     <Parameter Value="tb1" />
        ///     <Parameter Value="BandIndex" />
        ///     <Parameter Value="1" />
        /// </Validation>
        /// </summary>
        /// <param name="validateParams">First is the control, second is an array within which is the name of the property and the expected value.</param>
        /// <returns>True if validation passed, false otherwise.</returns>
        /// assumption is that the second element of validateParams is array with first element name and second value.
        public bool Validate(params object[] validateParams)
        {
            object control = validateParams[0];

            string elementId = (string)(validateParams[1] as Array).GetValue(0);
            if (elementId != "")
            {
                TestLog.Current.LogStatus("ControlId: [" + elementId + "]");
                control = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)control, elementId) as FrameworkElement;
            }
            string propertyName = (string)(validateParams[1] as Array).GetValue(1);
            object expectedValue = (validateParams[1] as Array).GetValue(2);
            PropertyInfo pInfo = control.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Type valueType = pInfo.PropertyType;

            TestLog.Current.LogStatus("ControlType: [" + control.GetType().ToString() + "]");
            TestLog.Current.LogStatus("PropertyName: [" + propertyName + "]");
            TestLog.Current.LogStatus("ExpectedValue: [" + expectedValue.ToString() + "]");

            if (valueType == expectedValue.GetType())
            {
                return PMEUtils.Current.VerifyProperty(control, propertyName, expectedValue);
            }
            else if (expectedValue.GetType() == typeof(string))
            {
                return PMEUtils.Current.VerifyProperty(control, propertyName, PMEUtils.Current.StringToType((string)expectedValue, valueType));
            }
            else
            {
                throw new Exception("Invalid set of arguments");
            }
        }
    }

}
