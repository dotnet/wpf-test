// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;	
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Reflection;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    /// <summary>    
    /// Testing basic boundary conditions    
    /// </summary>
    [Test(3, "PropertyTests", "BoundaryConditionTest", MethodName = "Run", Timeout = 120)]
    public class BoundaryConditionTest : AvalonTest
    {
        private Window _w;
        private string _contentFile;
        private string _test;       
        
        [Variation("Paginated", "CE")]
        [Variation("Bottomless", "CE")]
        [Variation("Paginated", "UIE")]
        [Variation("Bottomless", "UIE")]
        public BoundaryConditionTest(string content, string elementType)
        {
            CreateLog = false;
            if (content == "Paginated")
            {
                _contentFile = "BoundaryConditionContent.xaml";
            }
            else
            {
                _contentFile = "BoundaryConditionContent_FlowDocumentScrollViewer.xaml";
            }
           
            _test = elementType.ToLowerInvariant();
            
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);                                  
        }

        private TestResult Initialize()
        {           
            object content;

            _w = new Window();
            using (Stream fs = File.OpenRead(_contentFile))
            {
                content = XamlReader.Load(fs);
                _w.Content = content;
                _w.Show();
                fs.Close();
            }            
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            State state = new State();
            if (_test == "ce")
            {
                List<ContentElement> CEresults = new List<ContentElement>();
                GetContentElementsRecursively(_w.Content as DependencyObject, CEresults);
                state.PropertyChanger = ChangeContent;
                state.elements = CEresults.GetEnumerator();
            }
            else if (_test == "uie")
            {
                List<UIElement> UIEresults = new List<UIElement>();
                GetUIElementsRecursively(_w.Content as DependencyObject, UIEresults);
                state.PropertyChanger = ChangeContent;
                state.elements = UIEresults.GetEnumerator();
            }
            state.Start();
            Dispatcher.Run();
           
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private void GetContentElementsRecursively(DependencyObject dObj, List<ContentElement> CEresults)
        {
            if (dObj == null)
            {
                return;
            }

            ContentElement contentElement = dObj as ContentElement;
            if (contentElement != null)
            {
                CEresults.Add(contentElement);
            }

            foreach (object o in LogicalTreeHelper.GetChildren(dObj))
            {
                GetContentElementsRecursively(o as DependencyObject, CEresults);
            }
        }

        private void GetUIElementsRecursively(DependencyObject dObj, List<UIElement> UIresults)
        {
            if (dObj == null)
            {
                return;
            }

            UIElement uiElement = dObj as UIElement;
            if (uiElement != null)
            {
                UIresults.Add(uiElement);
            }

            foreach (object o in LogicalTreeHelper.GetChildren(dObj))
            {
                GetUIElementsRecursively(o as DependencyObject, UIresults);
            }
        }

        private IEnumerable<object> ChangeContent(DependencyObject element)
        {            
            bool testElement = true;

            //if we are testing UIElements, we only care about testing TextBlock
            UIElement ui = element as UIElement;
            if (ui != null)
            {
                testElement = false;
                
                TextBlock tb = element as TextBlock;

                if (tb != null)
                {
                    testElement = true;
                }
            }

            if (testElement)
            {
                ObservableCollection<DependencyProperty> dpList = new ObservableCollection<DependencyProperty>();
                
                InsertInheritedDPs(element, typeof(ContentElement), dpList);
                InsertDeclaredDPs(element, dpList);

                foreach (DependencyProperty prop in dpList)
                {                   
                    object originalValue = element.GetValue(prop);
                    object[] values = GetValues(element, prop);

                    if (values != null)
                    {
                        foreach (object value in values)
                        {
                            try
                            {
                                GlobalLog.LogStatus("Setting Element {0} Property {1} Value of {2}", element.ToString(), prop, value); 
                                element.SetValue(prop, value);
                            }
                            catch (System.ArgumentException)
                            {
                                //Exception caught in Set.  This is OK.
                            }
                            yield return null;
                        }
                        
                        element.SetValue(prop, originalValue);
                        yield return null;
                    }
                }
            }       
        }

        private object[] GetValues(DependencyObject element, DependencyProperty prop)
        {
            try
            {
                if (prop.PropertyType == typeof(int))
                {
                    return new object[] { Int32.MaxValue, Int32.MinValue };
                }
                else if (prop.PropertyType == typeof(double))
                {
                    return new object[] { Double.MaxValue, Double.MinValue, Double.Epsilon };
                }
                else if (IsOfType(prop.PropertyType, typeof(Thickness)))
                {
                    return new object[] { new Thickness(Double.MaxValue), new Thickness(Double.MinValue), new Thickness(Double.Epsilon) };
                }
                else if (prop.PropertyType == typeof(FigureLength))
                {
                    return new object[] { new FigureLength(Double.MaxValue, FigureUnitType.Pixel), new FigureLength(Double.MaxValue, FigureUnitType.Column), new FigureLength(Double.MaxValue, FigureUnitType.Page), new FigureLength(Double.MaxValue, FigureUnitType.Content), new FigureLength(Double.MinValue, FigureUnitType.Pixel), new FigureLength(Double.MinValue, FigureUnitType.Column), new FigureLength(Double.MinValue, FigureUnitType.Page), new FigureLength(Double.MinValue, FigureUnitType.Content), new FigureLength(Double.Epsilon, FigureUnitType.Pixel), new FigureLength(Double.Epsilon, FigureUnitType.Column), new FigureLength(Double.Epsilon, FigureUnitType.Page), new FigureLength(Double.Epsilon, FigureUnitType.Content) };
                }
                else if (prop.PropertyType == typeof(GridLength))
                {
                    return new object[] { new GridLength(Double.MaxValue, GridUnitType.Pixel), new GridLength(Double.MaxValue, GridUnitType.Star), new GridLength(Double.MinValue, GridUnitType.Pixel), new GridLength(Double.MinValue, GridUnitType.Star), new GridLength(Double.Epsilon, GridUnitType.Pixel), new GridLength(Double.Epsilon, GridUnitType.Star) };
                }
                else if (IsOfType(prop.PropertyType, typeof(Enum)))
                {
                    return new object[] { 1001 };
                }
            }
            catch (System.ArgumentException)
            {
                //Exception caught in Set.  This is OK.
            }
            return null;
        }        

        private void InsertInheritedDPs(DependencyObject element, Type lastBaseClass, ObservableCollection<DependencyProperty> list)
        {
            Type type = element.GetType();
            while (type.IsSubclassOf(lastBaseClass.BaseType))
            {
                InsertDeclaredDPs(element, type, list);
                type = type.BaseType;
            }
        }

        private void InsertDeclaredDPs(DependencyObject element, ObservableCollection<DependencyProperty> list)
        {
            InsertDeclaredDPs(element, element.GetType(), list);
        }

        private void InsertDeclaredDPs(DependencyObject element, Type type, ObservableCollection<DependencyProperty> list)
        {
            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static))
            {
                if (fieldInfo.FieldType == typeof(DependencyProperty))
                {
                    DependencyProperty dp = fieldInfo.GetValue(null) as DependencyProperty;
                    if (!dp.ReadOnly)
                    {
                        list.Add(dp);
                    }
                }
            }
        }

        private bool IsOfType(Type type1, Type type2)
        {
            return type1.IsSubclassOf(type2) || type1 == type2;
        }
    }

    public class State
    {
        public delegate IEnumerable PropertyChangerDelegate(DependencyObject o);
        public PropertyChangerDelegate PropertyChanger;
        public IEnumerator elements;
        IEnumerator _actions;
        private TestLog _log;        

        public void Start()
        {           
            elements.MoveNext();
            PostNext();
        }

        private void PostNext()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(doNext), null);
        }

        private object doNext(object o)
        {
            if (_actions == null)
            {
                _actions = PropertyChanger(elements.Current as DependencyObject).GetEnumerator();
                
                GlobalLog.LogStatus("ON ELEMENT " + elements.Current);
                _log = new TestLog("Element: " + elements.Current);                
            }

            if (_actions.MoveNext())
            {
                PostNext();
            }
            else
            {
                _log.Result = TestResult.Pass;
                _log.Close();
                if (elements.MoveNext())
                {
                    _actions = null;
                    PostNext();
                }
                else
                {
                    GlobalLog.LogStatus("Done!");                   
                    Dispatcher.ExitAllFrames(); 
                    
                }
            }
            return null;
        }
    }
}
