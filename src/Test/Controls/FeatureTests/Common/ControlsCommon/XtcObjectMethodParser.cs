using System;
using System.Windows;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace Avalon.Test.ComponentModel.Utilities
{
    class XtcActionMethodParser
    {
        private XmlElement actionElement;

        public XtcActionMethodParser(XmlElement actionElement)
        {
            this.actionElement = actionElement;
        }

        // Properties
        public string ObjectName
        {
            get { return actionElement.Name.Substring(0, SeparationIndex(actionElement)); }
        }

        public string MethodName
        {
            get { return actionElement.Name.Substring(SeparationIndex(actionElement) + 1, actionElement.Name.Length - SeparationIndex(actionElement) - 1); }
        }

        public object ConvertAttributeToParameter(string parameterValue, Type parameterType, FrameworkElement scene)
        {
            if (parameterValue == "{null}")
            {
                return null;
            }
            if (parameterValue == "{Scene}")
            {
                //Special case scene lookup syntax
                return scene;
            }
            else if (parameterValue.StartsWith("{SceneTreeSearch:"))
            {
                if ((FrameworkElement)scene.FindName(SceneTreeSearchElement(parameterValue)) == null)
                {
                    throw new ArgumentException("Unable to find the FrameworkElement " + parameterValue + " in the scene tree for method " + MethodName, "actionElement");
                }
                else
                {
                    return (FrameworkElement)scene.FindName(SceneTreeSearchElement(parameterValue));
                }
            }
            else
            {
                //We use a type converter to convert the string into the requested type
                TypeConverter converter = TypeDescriptor.GetConverter(parameterType);
                if (!converter.CanConvertFrom(typeof(string)))
                {
                    throw new ArgumentException("The Parameter of the " + MethodName + " method which cannot be converted from a string", "actionElement");
                }
                return converter.ConvertFromInvariantString(parameterValue);
            }
        }

        public object ConvertElementToParameter(XmlElement childElement, ParameterInfo parameter, FrameworkElement scene)
        {
            XmlNodeList valueNodes = childElement.SelectNodes("*");
            //if the parameter is an array then we create a list of that type
            if (parameter.ParameterType.IsArray)
            {
                //create a list of values
                Type arrayItemType = parameter.ParameterType.GetElementType();
                ArrayList values = new ArrayList();

                foreach (XmlElement valueNode in valueNodes)
                {
                    values.Add(GetObjectFromNode(scene, valueNode));
                }

                return values.ToArray(arrayItemType);
            }
            else if (valueNodes.Count == 0)
            {
                //just use the inner text if the type is a string
                if (parameter.ParameterType == typeof(string) || parameter.ParameterType == typeof(object))
                {
                    return childElement.InnerText;
                }
                throw new ArgumentException("The parameter " + parameter.Name + " of the method " + MethodName + " of the Action class is not a string but the xml element", "actionElement");
            }
            else
            {
                //convert the single element into an object
                if (valueNodes.Count > 1)
                {
                    throw new ArgumentException("The parameter " + parameter.Name + " of the method " + MethodName + " has more than one child but the parameter is not a list or array", "actionElement");
                }

                return GetObjectFromNode(scene, (XmlElement)valueNodes[0]);
            }
        }

        /// <summary>
        /// We only support XmlElement syntax below.
        /// <ItemsControlActions.RefreshItemsControl TargetElement="{Scene}"/>
        /// Need to refactor when we want to support other scenario.
        /// </summary>
        /// <param name="testObjectElement"></param>
        /// <returns>Integer.</returns>
        private int SeparationIndex(XmlElement testObjectElement)
        {
            return testObjectElement.Name.IndexOf(".");
        }

        private object GetObjectFromNode(FrameworkElement scene, XmlElement xmlElement)
        {
            //If the xml namespace is set then use the xaml parser
            if (xmlElement.NamespaceURI != string.Empty)
            {
                return ObjectFactory.CreateObjectFromXaml(xmlElement.OuterXml);
            }
            else if (xmlElement.Name.Equals("Item"))
            {
                foreach (XmlAttribute xmlAttribute in xmlElement.Attributes)
                {
                    if (xmlAttribute.Value.StartsWith("{SceneTreeSearch:"))
                    {
                        return (FrameworkElement)scene.FindName(SceneTreeSearchElement(xmlAttribute.Value));
                    }
                }
                throw new ArgumentException("You need to specify item name that you want to remove.");
            }
            else
            {
                throw new ArgumentException("Only xaml is supported for element content for parameters.");
                //
            }
        }
 
        // The special syntax for looking up element in the logical tree of the scene {SceneTreeSearch: name}
        private string SceneTreeSearchElement(string targetName)
        {
            int index = targetName.IndexOf(":") + 1;
            return targetName.Substring(index, targetName.Length - index - 1);
        }
    }
}


