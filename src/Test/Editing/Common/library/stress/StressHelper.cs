// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
* Description:
* Containing all common interfaces for stress components
*
\***************************************************************************/

using System;
using System.Windows;
using System.Reflection;
using System.Windows.Navigation;
using System.Collections;
using Test.Uis.Utils;

namespace Test.Uis.Stress
{
    /// <summary>
    /// Enum for the type of the tree being built.
    /// Currently we have the tree built from code or xaml
    /// </summary>
    public enum TreeBuildType : int
    {
        /// <summary>
        /// tree built from xaml
        /// </summary>
        Xaml = 1,

        /// <summary>
        /// tree built from code
        /// </summary>
        Code = 2
    }
    
    /// <summary>
    /// the interface for any stres operation to be carried out
    /// every stress operation class will have to implement this interface
    /// </summary>
    public interface IStressComponent
    {
        /// <summary>
        /// Stress operation to be done.
        /// </summary>
        /// <param name="stressSelector"></param>
        void DoStress(StressSelector stressSelector);
    }
    

    /// <summary>
    /// return true / false to specify that if the element is the stress target
    /// </summary>
    public interface IElementSelectionRule
    {
        /// <summary>
        /// implementation of this interface allows selection criteria of a particular element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool IsStressTarget(Element element);        
    }

    /// <summary>
    /// An abstract base class with a member Random instance
    /// Please note that if the StressComponent doesn't need
    /// a random it can inherit directly from IStressComponent
    /// </summary>
    public abstract class StressComponentBase : IStressComponent
    {
        /// <summary>
        /// protected ctor
        /// </summary>
        protected StressComponentBase()
        {
            this._rand = new Random();
        }
        
        /// <summary>
        /// Stress operation to be done.
        /// </summary>
        /// <param name="stressSelector"></param>
        public abstract void DoStress(StressSelector stressSelector);
        
        /// <summary>
        /// a protected property to expose the private Random instance
        /// </summary>
        protected Random Rand
        {
            get {return this._rand; }
        }
    
        /// <summary>
        /// the protected Random instance
        /// </summary>
        private Random _rand = null;    
    }

    /// <summary>
    /// the interface for the way of building tree
    /// currently we support building tree in code (which is still an abstract class)
    /// and in xaml
    /// </summary>
    public interface ITreeBuilder
    {
        /// <summary>
        /// build the tree in the method
        /// </summary>
        /// <param name="treeBuilderSelector">TreeBuilderSelector instance. This will tell you some parameters on tree building</param>
        /// <returns></returns>
        object BuildTree(TreeBuilderSelector treeBuilderSelector);
        
        /// <summary>
        /// specify how the tree is built.
        /// </summary>
        TreeBuildType BuildType
        { 
            get;
        }
    }
    
    /// <summary>
    /// The implementation of building tree in code.
    /// </summary>
    public abstract class CodeTreeBuilder : ITreeBuilder
    {
        /// <summary>
        /// Child classes will have to implement this method
        /// </summary>
        /// <param name="treeBuilderSelector"></param>
        /// <returns></returns>
        public abstract object BuildTree(TreeBuilderSelector treeBuilderSelector);

        /// <summary>
        /// Any child classes of this builds tree in code
        /// </summary>
        public TreeBuildType BuildType
        {
            get { return TreeBuildType.Code; }
        }
    }
    
    /// <summary>
    /// The implementation of building tree in xaml, and pass back the BindProductUri instance to
    /// the main class so that it can navigate
    /// </summary>
    public class XamlTreeBuilder : ITreeBuilder
    {
        /// <summary>
        /// This method reads in the xaml location and create the BindProductUri instance.
        /// </summary>
        /// <param name="treeBuilderSelector"></param>
        /// <returns></returns>
        public object BuildTree(TreeBuilderSelector treeBuilderSelector)
        {
            if (treeBuilderSelector == null)
            {
                throw new InvalidOperationException("XamlTreeBuilder.BuildTree. stressSelector is null");
            }
            
            if (treeBuilderSelector.XamlFile == null)
            {
                throw new InvalidOperationException("XamlTreeBuilder.BuildTree. Calling XamlTreeBuilder.BuildTree but it is not using xaml");
            }
            
            
            return (new BindProductUri(treeBuilderSelector.XamlFile));
        }
        
        /// <summary>
        /// We build the tree from xaml
        /// </summary>
        public TreeBuildType BuildType
        {
            get { return TreeBuildType.Xaml; }
        }
    }
    
    /// <summary>
    /// This class picks the correct tree building routine based on the xml settings.
    /// </summary>
    public sealed class TreeBuilderSelector
    {
        /// <summary>
        /// the ctor takes ConfigurationSettings instance and set the memeber variables.
        /// it calls SelectBuildTreeMethod and BuildTree
        /// </summary>
        /// <param name="configurationSettings">ConfigurationSettings instance created in the main class</param>
        public TreeBuilderSelector(ConfigurationSettings configurationSettings)
        {
            // Important: We should first initialize all variables here
            // before we create tree. BuildTree depends on, for instance,
            // xamlfile location to build the tree
            this._configurationSettings = configurationSettings;

            this._currentTreeBuildMethod = SelectBuildTreeMethod();

            this._objectToMavigateTo = this._currentTreeBuildMethod.BuildTree(this);
            
        }

        /// <summary>
        /// return xaml file location, if there exists one
        /// </summary>
        public string XamlFile
        {
            get { return this._xamlFile; }
        }

        /// <summary>
        /// forward the tree building type to the main class
        /// </summary>
        public TreeBuildType BuildType
        {
            get { return this._currentTreeBuildMethod.BuildType; }
        }
        
        /// <summary>
        /// This returns an object so that main class cna navigate. It
        /// is possible that the object returned is actually an instance
        /// of BindProductUri. The main class has the responsibility to
        /// differentiate the object type and call the correct NavigationWindow.Navigate
        /// </summary>
        public object ObjectToNavigate
        {
            get { return this._objectToMavigateTo; }
        }

        /// <summary>
        /// This method selects correct TreeBuilder and return ITreeBuilder to the caller
        /// </summary>
        /// <returns></returns>
        private ITreeBuilder SelectBuildTreeMethod()
        {
            ITreeBuilder treeBuilder = null;

            this._xamlFile = this._configurationSettings.GetArgument("XamlFile");
            
            // We need to create the tree from code
            if (this._xamlFile == null || this._xamlFile == "")
            {
                // Get the class that implements ITreeBuilder and instantiate it
                
                string treeBuilderName = this._configurationSettings.GetArgument("TreeBuilder");
                
                if (treeBuilderName == null || treeBuilderName == "")
                {
                    throw new InvalidOperationException("TreeBuilderSelector.SelectBuildTreeMethod Both XamlFile and TreeBulder are null");
                }
                
                Type type = FindType(treeBuilderName);
                
                if (type == null)
                {
                    throw new InvalidOperationException("TreeBuilderSelector.SelectBuildTreeMethod. Cannot instantiate type " + treeBuilderName);
                }
                
                // create the TreeBuilder instance
                treeBuilder = Activator.CreateInstance(type) as ITreeBuilder;

                if (treeBuilder == null)
                {
                    throw new InvalidOperationException("TreeBuilderSelector.SelectBuildTreeMethod Both XamlFile and TreeBulder are null");
                }
            }      
                  
            // We have the xaml file location, that means we need to build the tree from xaml
            // create an instance of XamlTreeBuilder and return that to caller
            else
            {
                treeBuilder = new XamlTreeBuilder();
            }
            
            return treeBuilder;
        }
        
        /// <summary>
        /// Return the instance of Type given a nam of the type. The name is not a full qualified name
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private Type FindType(string typeName)
        {
            if (typeName == null)
            {
                return null;
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(Assembly assembly in assemblies)
            {
                Type [] testTypes = assembly.GetTypes();
                foreach(Type type in testTypes)
                {
                    if (type.Name == typeName)
                    {
                        return type;
                    }
                }
            }
            
            return null;
        }

        /// <summary>
        /// forward to ConfigurationSettings.GetArgument
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public string GetArgument(string argument)
        {
            return this._configurationSettings.GetArgument(argument);
        }

        /// <summary>
        /// forward to ConfigurationSettings.GetArgumentAsBool
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public bool GetArgumentAsBool(string argument)
        {
            return this._configurationSettings.GetArgumentAsBool(argument);
        }

        /// <summary>
        /// forward to ConfigurationSettings.GetArgumentAsInt
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public int GetArgumentAsInt(string argument)
        {
            return this._configurationSettings.GetArgumentAsInt(argument);
        }

        private ITreeBuilder          _currentTreeBuildMethod  = null;
        private object                _objectToMavigateTo = null;
        private ConfigurationSettings _configurationSettings = null;
        private string                _xamlFile = null;
    }

    /// <summary>
    /// StressSelector selects randomly the stress operation
    /// </summary>
    public sealed class StressSelector
    {
        /// <summary>
        /// ctor
        /// </summary>
        public StressSelector(Element rootElement, ConfigurationSettings configurationSettings)
        {
            this._stressComponentArrayList = new ArrayList();

            EnumerateStressComponents();

            ArrayList elementSelectionRuleArrayList = new ArrayList();

            ArrayList elementArrayList = new ArrayList();

            EnumerateElementSelectionRule(elementSelectionRuleArrayList, configurationSettings);

            // something bad has happened. no element selection rule added
            if (elementSelectionRuleArrayList.Count == 0)
            {
                throw new InvalidOperationException("StressSelector ctor: stress can't proceed without element selection rule");
            }

            BuildElementArray(rootElement, elementArrayList, elementSelectionRuleArrayList);

            this._elementArray = new Element[elementArrayList.Count];

            for (int i = 0; i < elementArrayList.Count; i++)
            {
                this._elementArray[i] = elementArrayList[i] as Element;
            }
                        
            this._rand = new Random();
        }
        
        /// <summary>
        /// this method puts the intances of IStresComponent implementation into an ArrayList
        /// </summary>
        /// <param name="stressComponent"></param>
        /// <returns></returns>
        private bool RegisterSingleStressComponent(IStressComponent stressComponent)
        {
            if (stressComponent == null)
            {
                throw new ArgumentNullException("TextEditStress.RegisterSingleStressComponent. stressComponent is null");
            }
            
            this._stressComponentArrayList.Add(stressComponent);
            
            return true;
        }

        /// <summary>
        /// this method puts the intances of IElementSelectionRule implementation into an ArrayList
        /// </summary>
        /// <param name="elementSelectionRule"></param>
        /// <param name="elementSelectionRuleArrayList"></param>
        /// <param name="configurationSettings"></param>
        /// <returns></returns>
        private bool RegisterElementSelectionRule(IElementSelectionRule elementSelectionRule, 
            ArrayList elementSelectionRuleArrayList,
            ConfigurationSettings configurationSettings)
        {
            if (elementSelectionRule == null)
            {
                throw new ArgumentNullException("TextEditStress.RegisterElementSelectionRule. elementSelectionRule is null");
            }
            
            bool bAddRule = false;

            try
            {
                bAddRule = configurationSettings.GetArgumentAsBool(elementSelectionRule.GetType().Name, true);
            }
            catch
            {
                // if the value is missing we need to add that to the list
                // so rule activation is on by default
                bAddRule = true;
            }

            if (bAddRule)
            {
                elementSelectionRuleArrayList.Add(elementSelectionRule);
            }
            
            return true;
        }
        
        private void EnumerateElementSelectionRule(ArrayList elementSelectionRuleArrayList, ConfigurationSettings configurationSettings)
        {
            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(Assembly assembly in assemblies)
            {
                Type [] testTypes = assembly.GetTypes();
                foreach(Type type in testTypes)
                {
                    if (type != null
                        && type.GetInterface("IElementSelectionRule") == typeof(Test.Uis.Stress.IElementSelectionRule)
                        && !type.IsAbstract)
                    {
                        RegisterElementSelectionRule(Activator.CreateInstance(type, (new object[] {} ), null) as IElementSelectionRule, 
                            elementSelectionRuleArrayList, configurationSettings);
                    }
                }
            }
        }
         
        /// <summary>
        /// Find out all the classes that implement IStressComponent and register them.
        /// </summary>
        private void EnumerateStressComponents()
        {
            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(Assembly assembly in assemblies)
            {
                Type [] testTypes = assembly.GetTypes();
                foreach(Type type in testTypes)
                {
                    if (type != null
                        && type.GetInterface("IStressComponent") == typeof(Test.Uis.Stress.IStressComponent)
                        && !type.IsAbstract)
                    {
                        RegisterSingleStressComponent(Activator.CreateInstance(type, (new object[] {} ), null) as IStressComponent);
                    }
                }
            }
        }

        /// <summary>
        /// add elements to _elementArrayList if the elements are the stress target
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="elementArrayList"></param>
        /// <param name="elementSelectionRuleArrayList"></param>
        private void BuildElementArray(Element elem, 
            ArrayList elementArrayList, 
            ArrayList elementSelectionRuleArrayList)
        {            
            if (elem == null)
            {
                return;
            }

            bool bResult = false;
            
            foreach(IElementSelectionRule elementSelectionList in elementSelectionRuleArrayList)
            {
                bResult = elementSelectionList.IsStressTarget(elem);
                
                if (!bResult)
                {
                    break;
                }
            }   
         
            if (bResult)
            {
                elementArrayList.Add(elem);            
            }

            Element childElement = elem.FirstChildElement;

            while(childElement != null)
            {
                BuildElementArray(childElement, elementArrayList, elementSelectionRuleArrayList);
                childElement = childElement.NextElement;
            }
        }
        
        /// <summary>
        /// This method is called by the main class when stress interval occurs
        /// </summary>
        public void InvokeStressFunction()
        {            
            int index = this._rand.Next(0, this._stressComponentArrayList.Count);
            
            IStressComponent stressComponent = this._stressComponentArrayList[index] as IStressComponent;

            if (stressComponent == null)
            {
                return;
            }

            // Calling the corresponding DoStress in IStressComponent
            stressComponent.DoStress(this);
        }
        
        /// <summary>
        /// exposes the element target array
        /// </summary>
        public Element[] StressTargets
        {
            get { return this._elementArray; }
        }

        private ArrayList           _stressComponentArrayList = null;
        private Element             [] _elementArray = null;
        private Random              _rand = null;
    }
}