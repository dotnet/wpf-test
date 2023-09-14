//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Windows.Markup;
using System.Collections.Generic;
using Microsoft.Test.Logging;

#endregion

namespace Avalon.Test.ComponentModel.Utilities
{
    /// <summary>
    /// helper class for xtc test
    /// </summary>
    public static class XtcTestHelper
    {
        /// <summary>
        /// do actions specified by xml element against a framework element
        /// </summary>
        /// <param name="element">the framework element</param>
        /// <param name="actionsXml">xml element for actions</param>
        public static void DoActions(FrameworkElement element, XmlElement actionsXml)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (actionsXml == null)
                throw new ArgumentNullException("actionsXml");

            foreach (XmlNode node in actionsXml.ChildNodes)
            {
                if (!(node is XmlElement))
                    continue;
                XmlElement actionXml = (XmlElement)node;
                IAction action = GetActionFromXml(actionXml);
                if (action == null)
                    throw new Exception("Action not found: [" + actionsXml.InnerXml + "]");
                DoAction(action, element, actionXml);
            }
        }

        internal static void DoAction(IAction action, FrameworkElement element, XmlElement actionXml)
        {
            if (IsExtension(action))
            {
                action.Do(element, actionXml);
            }
            else
            {
                string[] values = PMEUtils.Current.ValuesFromXml(actionXml);
                action.Do(element, values);
            }
        }

        /// <!--summary>
        /// test whether an action is an extension.
        /// you should specify an action or validation as an extension by means of IsExtensionAttribute
        /// </summary>
        /// <param name="type">the type of an action or validation</param>
        /// <returns>true if the type is an extension</returns-->
        private static bool IsExtension(object obj)
        {
            if (obj == null)
                return false;
            return obj.GetType().GetCustomAttributes(typeof(IsExtensionAttribute), true).Length != 0;
        }

        /// <summary>
        /// get action object from an xml element
        /// </summary>
        /// <param name="actionXml">xml element for action</param>
        /// <returns>action object or null if not found</returns>
        public static IAction GetActionFromXml(XmlElement actionXml)
        {
            return GetObjectFromXml(actionXml) as IAction;
        }

        /// <!--summary>
        /// get object from an xml element
        /// </summary>
        /// <param name="xml">xml element</param>
        /// <param name="namespaces">namespace the class of object residents in</param>
        /// <returns>object or exception if not found</returns-->
        public static object GetObjectFromXml(XmlElement xml)
        {
            if (xml == null)
                throw new ArgumentNullException("xml");

            string name = xml.Name;
            if ("Action" == name || "Validation" == name)
                name = XmlHelper.GetAttribute<string>(xml, "Name");
            return ObjectFactory.CreateObjectFromTypeName(name);
        }

        /// <summary>
        /// do validations specified by an xml element against a framework element
        /// </summary>
        /// <param name="element">framework element</param>
        /// <param name="validationsXml">xml element of validations</param>
        /// <returns>true if all validations succeed; false if errors occur</returns>
        public static string DoValidations(object element, XmlElement validationsXml)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (validationsXml == null)
                throw new ArgumentNullException("validationsXml");

            foreach (XmlNode node in validationsXml.ChildNodes)
            {
                if (!(node is XmlElement))
                    continue;
                XmlElement validationXml = (XmlElement)node;
                IValidation validation = GetValidationFromXml(validationXml);
                if (validation == null)
                    throw new ArgumentException("Validation not found: [" + validationXml.InnerXml + "]");
                if (!DoValidation(validation, element, validationXml))
                    return validation.GetType().Name;
            }
            return string.Empty;
        }

        internal static bool DoValidation(IValidation validation, object element, XmlElement validationXml)
        {
            if (IsExtension(validation))
            {
                return validation.Validate(element, validationXml);
            }
            else
            {
                string[] values = PMEUtils.Current.ValuesFromXml(validationXml);
                return validation.Validate(element, values);
            }
        }

        /// <summary>
        /// get validation object from an xml elemetn
        /// </summary>
        /// <param name="validationXml">xml element of validaton</param>
        /// <returns>validation object or null if not found</returns>
        public static IValidation GetValidationFromXml(XmlElement validationXml)
        {
            return GetObjectFromXml(validationXml) as IValidation;
        }

        /// <summary>
        /// load xaml string to an object
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static object LoadXml(string objectName)
        {
            return ObjectFactory.CreateObjectFromXaml(objectName);
        }

        /// <summary>
        /// create a event tester from a fragment of xml element
        /// </summary>
        /// <param name="xmlElement"></param>
        /// <returns></returns>
        public static IEventTester GetEventTesterFromXml(XmlElement xmlElement)
        {
            if (xmlElement.Name == "Events")
                return new EventCollection(xmlElement);
            else if (xmlElement.Name == "Event")
                return new EventTester(xmlElement);
            else
                return null;
        }
    }

    public interface IEventTester
    {
        void Hook(object eventRaiser);
        void Unhook();
        string Verify();
        string Name
        { get;}
    }

    public class EventTester : IEventTester
    {
        public EventTester(XmlElement element)
        {
            this.element = element;
            ev = new EventVerifier<EventArgs>();
        }
        
        public void Hook(object eventRaiser)
        {
            if (eventRaiser == null)
                throw new ArgumentNullException("event raiser");
            string eventName = XmlHelper.GetAttribute<string>(element, "Name");
            this.eventRaiser = eventRaiser;
            ei = eventRaiser.GetType().GetEvent(eventName);
            MethodInfo mi = ev.GetType().GetMethod("OnEvent");
            del = Delegate.CreateDelegate(ei.EventHandlerType, ev, mi);
            ei.AddEventHandler(eventRaiser, del);
        }

        public string Verify()
        {
            int times = XmlHelper.GetAttribute(element, "Raised", true) ? 1 : 0;
            if (!ev.Verify(times))
            {
                return Name + " event not raised " + times + " time(s)";
            }

            if (times == 0)
                return null;

            foreach (EventArgInfo<EventArgs> eai in ev.Events)
            {
                if (eai.Source != eventRaiser)
                    return Name;
                for (XmlNode node = element.FirstChild; node != null; node = node.NextSibling)
                {
                    if (node.Name != "ArgsProperty" || node.GetType() != typeof(XmlElement))
                        continue;
                    string proName = XmlHelper.GetAttribute((XmlElement)node, "Name", "");
                    PropertyInfo proi = eai.Args.GetType().GetProperty(proName);
                    object expectedValue = XmlHelper.GetAttribute(proi.PropertyType,
                        (XmlElement)node, "ExpectedValue");
                    object actualValue = proi.GetValue(eai.Args, null);
                    if (!(expectedValue == actualValue || (expectedValue != null && expectedValue.Equals(actualValue))))
                        return Name + "." + proName 
                            + ": expected [" + expectedValue + "]" + " actual [" + actualValue + "]";
                }
            }

            return null;
        }

        public void Unhook()
        {
            if (ei != null)
                ei.RemoveEventHandler(eventRaiser, del);
        }

        public string Name
        {
            get { return XmlHelper.GetAttribute<string>(element, "Name", ""); }
        }
        
        private XmlElement element;
        private EventVerifier<EventArgs> ev;
        private Delegate del;
        private EventInfo ei;
        private object eventRaiser;
    }

    public class EventCollection : IEventTester
    {
        public EventCollection(XmlElement element)
        {
            this.element = element;
            testers = new List<IEventTester>();
            foreach (XmlNode node in element.ChildNodes)
            {
                if (!(node is XmlElement) || node.Name != "Event")
                    continue;
                testers.Add(XtcTestHelper.GetEventTesterFromXml((XmlElement)node));
            }
        }
        public void Hook(object eventRaiser)
        {
            foreach (EventTester tester in testers)
            {
                tester.Hook(eventRaiser);
            }
        }

        public string Verify()
        {
            foreach (EventTester tester in testers)
            {
                string failed = tester.Verify();
                if (failed != null)
                {
                    return failed;
                }
            }
            return null;
        }

        public void Unhook()
        {
            foreach (EventTester tester in testers)
            {
                tester.Unhook();
            }
        }

        public string Name
        {
            get { return "EventActions"; }
        }

        private List<IEventTester> testers;
        private XmlElement element;
    }

    /// <summary>
    /// a mark that indicates that the action or validation is using the extension interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IsExtensionAttribute : Attribute
    {
    }
}
