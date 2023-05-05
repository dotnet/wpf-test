// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.InstanceReference
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Markup;
    using System.Xaml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;

    #region simple forward, backward reference

    public class SimpleRefFoo
    {
        public SimpleRefBar bar { get; set; }
        public SimpleRefBar bar2 { get; set; }
    }

    public class SimpleRefBar
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    #endregion

    #region multiple references

    public class MultipleRefFoo
    {
        public MultipleRefBar bar { get; set; }
        public MultipleRefBar barForward { get; set; }
        public MultipleRefBar barBackward { get; set; }
    }

    public class MultipleRefBar
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    #endregion

    #region tree references

    public class TreeFoo
    {
        public TreeFoo child0 { get; set; }
        public TreeFoo child1 { get; set; }

        public int IntProperty { get; set; }
        public string StringProperty { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            // instance reference leaf backward //
            TreeFoo target = new TreeFoo()
                                 {
                                     IntProperty = 100,
                                     StringProperty = "root"
                                 };

            TreeFoo child0 = new TreeFoo()
                                 {
                                     IntProperty = 101,
                                     StringProperty = "child0"
                                 };
            TreeFoo child1 = new TreeFoo()
                                 {
                                     IntProperty = 102,
                                     StringProperty = "child1"
                                 };

            TreeFoo child00 = new TreeFoo()
                                  {
                                      IntProperty = 103,
                                      StringProperty = "child00"
                                  };
            TreeFoo child01 = new TreeFoo()
                                  {
                                      IntProperty = 104,
                                      StringProperty = "child01"
                                  };

            TreeFoo child10 = new TreeFoo()
                                  {
                                      IntProperty = 105,
                                      StringProperty = "child10"
                                  };
            // child11 points to  child01
            TreeFoo child11 = child01;

            target.child0 = child0;
            target.child1 = child1;
            child0.child0 = child00;
            child0.child1 = child01;
            child1.child0 = child10;
            child1.child1 = child11;

            // TreeReferencesLeafRefsNonLeafBackward//
            TreeFoo target1 = new TreeFoo()
                                  {
                                      IntProperty = 100,
                                      StringProperty = "root"
                                  };

            TreeFoo target1child0 = new TreeFoo()
                                        {
                                            IntProperty = 101,
                                            StringProperty = "child0"
                                        };
            TreeFoo target1child1 = new TreeFoo()
                                        {
                                            IntProperty = 102,
                                            StringProperty = "child1"
                                        };

            TreeFoo target1child00 = new TreeFoo()
                                         {
                                             IntProperty = 103,
                                             StringProperty = "child00"
                                         };
            TreeFoo target1child01 = new TreeFoo()
                                         {
                                             IntProperty = 104,
                                             StringProperty = "child01"
                                         };

            TreeFoo target1child10 = new TreeFoo()
                                         {
                                             IntProperty = 105,
                                             StringProperty = "child10"
                                         };
            // child11 points to  child1 (non leaf)
            TreeFoo target1child11 = target1child1;

            target1.child0 = target1child0;
            target1.child1 = target1child1;
            target1child0.child0 = target1child00;
            target1child0.child1 = target1child01;
            target1child1.child0 = target1child10;
            target1child1.child1 = target1child11;

            List<TestCaseInfo> testCases = new List<TestCaseInfo>
                                               {
                                                   new TestCaseInfo
                                                       {
                                                           Target = target,
                                                           TestID = "InstanceReferenceLeafBackward" + 0,
                                                       },
                                                   new TestCaseInfo
                                                       {
                                                           Target = target1,
                                                           TestID = "TreeReferencesLeafRefsNonLeafBackward" + 0,
                                                       }
                                               };

            return testCases;
        }

        #endregion
    }

    #endregion

    #region Implicit naming

    public class ImplicitNamingFoo
    {
        public ImplicitNamingBar bar { get; set; }
        public ImplicitNamingBar bar2 { get; set; }
    }

    [RuntimeNamePropertyAttribute("MyImplicitName")]
    public class ImplicitNamingBar
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }

        public string MyImplicitName { get; set; }
    }

    #endregion

    #region Multiple references extended

    public class B
    {
        public object D { get; set; }
    }

    public class A : B
    {
        public object C { get; set; }
    }

    #endregion

    #region Namescopes

    public class UnScopedFoo
    {
        public ScopedBar bar { get; set; }
        public ScopedBar bar2 { get; set; }

        public UnScopedBar bar3 { get; set; }
    }

    public class UnScopedBar
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }

        public UnScopedBar bar4 { get; set; }
    }

    public class ScopedBar : NameScopeImpl
    {
        public UnScopedBar ubar { get; set; }
        public UnScopedBar ubar2 { get; set; }
    }

    public class SingleScopedBar : NameScopeImpl
    {
        public UnScopedBar ubar { get; set; }
    }

    public class SingleScopedFoo
    {
        public SingleScopedBar bar { get; set; }
        public UnScopedBar ubar { get; set; }
    }

    #endregion

    #region Nested Namescopes

    public class UnscopedFoo1
    {
        public UnscopedBar2 ubar2 { get; set; }
        public NestedScope nestedScope { get; set; }
    }

    public class UnscopedBar2
    {
        public string StringProperty { get; set; }
    }

    public class NestedScope : NameScopeImpl
    {
        public UnscopedBar2 unscopedBar { get; set; }
        public NestedScope scope1 { get; set; }
    }

    #endregion

    #region DesignerSerializationVisibility

    public class SimpleDSVFoo
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public SimpleDSVBar barVisible { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int IntPropertyVisible { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<int> IntListVisible { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleDSVBar barContent { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public int IntPropertyContent { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<int> IntListContent { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SimpleDSVBar barHidden { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IntPropertyHidden { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<int> IntListHidden { get; set; }
    }

    public class SimpleDSVBar
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    public class FooWithHiddenBar
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SimpleDSVBar HiddenBar { get; set; }

        public SimpleDSVBar VisibleBar { get; set; }
    }

    #endregion

    #region circular references

    public class Node
    {
        public SimpleRefBar Bar { get; set; }
        public Node Next { get; set; }
    }

    public class circle
    {
        public string StringProperty { get; set; }
    }

    #endregion

    [ContentProperty("Name")]
    [RuntimeNameProperty("Name")]
    public class Persona
    {
        public string Name { get; set; }
        public IList<Persona> Friends { get; set; }
    }

    [RuntimeNameProperty("Name")]
    public class MyNode
    {
        public string Name { get; set; }
        public MyNode NextNode { get; set; }
    }

    public class _scoped : NameScopeImpl
    {
        public object MyProperty { get; set; }

        public object MyProperty1 { get; set; }
    }

    [RuntimeNameProperty("Name")]
    public class RTNPClass
    {
        public string Name { get; set; }
        public object MySelf { get; set; }
    }

    [RuntimeNameProperty("Name")]
    public class NamedClass
    {
        public string Name { get; set; }
        public string StringProperty { get; set; }
    }

    public class UnNamedNode
    {
        public string Id { get; set; }
        public UnNamedNode Next { get; set; }
    }

    public class XamlNameResolverExtension : MarkupExtension
    {
        private bool _called = false;

        public XamlNameResolverExtension()
        {
        }

        public XamlNameResolverExtension(string data)
        {
            Data = data;
        }

        public const string ReferenceName1 = "__Reference_ID_0";
        public const string ReferenceName2 = "__Reference_ID_1";
        public const string ReferenceName3 = "__Reference_ID_2";
        public const string AltReferenceName1 = "__Reference_ID_0Alt";
        public const string AltReferenceName2 = "__Reference_ID_1Alt";
        public const string AltReferenceName3 = "__Reference_ID_2Alt";
        public string Data { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var ixnr = (IXamlNameResolver)serviceProvider.GetService(typeof(IXamlNameResolver));
            if (ixnr == null)
            {
                throw new DataTestException("Unable to get IXamlNameResolver");
            }

            if (String.IsNullOrEmpty(Data))
            {
                throw new DataTestException("Data must be set.");
            }

            if (Data == "differentTC" && _called)
            {
                throw new DataTestException("Same instance called again.");
            }

            try
            {
                object stuff;
                switch (Data)
                {
                    case "requestDifferentRef":
                        if (_called)
                        {
                            stuff = null;
                        }
                        else
                        {
                            stuff = ixnr.Resolve(ReferenceName1);
                        }
                        break;
                    case "multipleNotPresent":
                        stuff = ixnr.Resolve(ReferenceName3);
                        break;
                    default:
                        stuff = ixnr.Resolve(ReferenceName1);
                        break;
                }

                if (stuff != null)
                {
                    return stuff;
                }

                if (!ixnr.IsFixupTokenAvailable)
                {
                    throw new InvalidOperationException("Forward reference failed to resolve");
                }
                switch (Data)
                {
                    case "null":
                        return ixnr.GetFixupToken(new string[]
                                                            {
                                                                ReferenceName1
                                                            }, true);
                    case "nullnonames":
                        return ixnr.GetFixupToken(new string[]
                                                            {
                                                            }, true);
                    case "nullnullnames":
                        return ixnr.GetFixupToken(null, true);
                    case "this":
                        return ixnr.GetFixupToken(new string[]
                                                            {
                                                                ReferenceName1
                                                            });
                    case "thisnonames":
                        return ixnr.GetFixupToken(new string[]
                                                            {
                                                            });
                    case "thisnullnames":
                        return ixnr.GetFixupToken(null);
                    case "differentME":
                        return ixnr.GetFixupToken(new string[]
                                                           {
                                                               ReferenceName1
                                                           });
                    case "differentTC":
                        return ixnr.GetFixupToken(new string[]
                                                             {
                                                                 ReferenceName1
                                                             });
                    case "multiplePresent":
                        return ixnr.GetFixupToken(new string[]
                                                            {
                                                                ReferenceName1, ReferenceName2
                                                            });
                    case "multipleNotPresent":
                        return ixnr.GetFixupToken(new string[]
                                                            {
                                                                ReferenceName1, ReferenceName3
                                                            });
                    case "requestDifferentRef":
                        return ixnr.GetFixupToken(new string[]
                                                            {
                                                                !_called ? ReferenceName1 : AltReferenceName1
                                                            });
                    case "other":
                        return ixnr.GetFixupToken(new string[]
                                                            {
                                                                ReferenceName1
                                                            });
                    default:
                        throw new DataTestException("Invalid value for Data: " + Data);
                }
            }
            finally
            {
                _called = true;
            }
        }
    }

    public class MultipleRefFooWTypeConverter
    {
        [TypeConverter(typeof(XamlNameResolverTypeConverter))]
        public MultipleRefBar bar { get; set; }

        [TypeConverter(typeof(XamlNameResolverTypeConverter))]
        public MultipleRefBar barForward { get; set; }

        [TypeConverter(typeof(XamlNameResolverTypeConverter))]
        public MultipleRefBar barBackward { get; set; }
    }

    public class XamlNameResolverTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        private bool _called = false;
        public string Data { get; set; }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string && !String.IsNullOrEmpty((string)value))
            {
                var ixnr = (IXamlNameResolver)context.GetService(typeof(IXamlNameResolver));
                if (ixnr == null)
                {
                    throw new DataTestException("Unable to get IXamlNameResolver");
                }

                if ((Data == "differentME" || Data == "differentTC") && _called)
                {
                    throw new DataTestException("Same instance called again.");
                }

                try
                {
                    object stuff;
                    switch (Data)
                    {
                        case "requestDifferentRef":
                            if (_called)
                            {
                                stuff = null;
                            }
                            else
                            {
                                stuff = ixnr.Resolve(XamlNameResolverExtension.ReferenceName1);
                            }
                            break;
                        default:
                            if ((string)value == "multipleNotPresent")
                            {
                                stuff = ixnr.Resolve(XamlNameResolverExtension.ReferenceName3);
                            }
                            else
                            {
                                stuff = ixnr.Resolve(XamlNameResolverExtension.ReferenceName1);
                            }
                            break;
                    }
                    if (stuff != null)
                    {
                        return stuff;
                    }

                    if (!ixnr.IsFixupTokenAvailable)
                    {
                        throw new InvalidOperationException("Forward reference failed to resolve");
                    }
                    switch ((string)value)
                    {
                        case "null":
                            return ixnr.GetFixupToken(new string[]
                                                                {
                                                                    XamlNameResolverExtension.ReferenceName1
                                                                }, true);
                        case "nullnonames":
                            return ixnr.GetFixupToken(new string[]
                                                                {
                                                                }, true);
                        case "nullnullnames":
                            return ixnr.GetFixupToken(null, true);
                        case "this":
                            return ixnr.GetFixupToken(new string[]
                                                                {
                                                                    XamlNameResolverExtension.ReferenceName1
                                                                });
                        case "thisnonames":
                            return ixnr.GetFixupToken(new string[]
                                                                {
                                                                });
                        case "thisnullnames":
                            return ixnr.GetFixupToken(null);
                        case "other":
                            return ixnr.GetFixupToken(new string[]
                                                                        {
                                                                            XamlNameResolverExtension.ReferenceName1
                                                                        });
                        case "differentME":
                            return ixnr.GetFixupToken(new string[]
                                                                                               {
                                                                                                   XamlNameResolverExtension.ReferenceName1
                                                                                               });
                        case "differentTC":
                            return ixnr.GetFixupToken(new string[]
                                                                 {
                                                                     XamlNameResolverExtension.ReferenceName1
                                                                 });
                        case "multiplePresent":
                            return ixnr.GetFixupToken(new string[]
                                                                {
                                                                    XamlNameResolverExtension.ReferenceName1, XamlNameResolverExtension.ReferenceName2
                                                                });
                        case "multipleNotPresent":
                            return ixnr.GetFixupToken(new string[]
                                                                {
                                                                    XamlNameResolverExtension.ReferenceName1, XamlNameResolverExtension.ReferenceName3
                                                                });
                        case "requestDifferentRef":
                            return ixnr.GetFixupToken(new string[]
                                                                {
                                                                    !_called ? XamlNameResolverExtension.ReferenceName1 : XamlNameResolverExtension.AltReferenceName1
                                                                });
                        default:
                            throw new DataTestException("Invalid value for Data: " + value);
                    }
                }
                finally
                {
                    _called = true;
                }
            }
            else
            {
                throw new ArgumentException("In ConvertFrom: can not convert to MultipleRefFooWTypeConverter.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            throw new DataTestException("ConvertTo doesn't work with this converter");
        }
    }

    [NameScopeProperty("NameScope")]
    public class NameScopePropOnType
    {
        public INameScope NameScope { get; set; }

        public static INameScope GetNameScope(object instance)
        {
            return ((NameScopePropOnType)instance).NameScope;
        }

        public MultipleRefBar bar { get; set; }
        public MultipleRefBar barForward { get; set; }
        public MultipleRefBar barBackward { get; set; }
    }

    [NameScopeProperty("NameScopeProp", typeof(CustomAttachableNameScope))]
    public class CustomAttachedNameScope
    {
        public MultipleRefBar bar { get; set; }
        public MultipleRefBar barForward { get; set; }
        public MultipleRefBar barBackward { get; set; }
    }

    public class CustomAttachableNameScope
    {
        private static readonly AttachableMemberIdentifier s_nameScopePropName =
            new AttachableMemberIdentifier(typeof(CustomAttachableNameScope), "NameScopeProp");

        public static INameScope GetNameScopeProp(object target)
        {
            INameScope nameScope = null;
            AttachablePropertyServices.TryGetProperty(target, s_nameScopePropName, out nameScope);
            return nameScope;
        }

        public static void SetNameScopeProp(object target, INameScope nameScopeProp)
        {
            AttachablePropertyServices.SetProperty(target, s_nameScopePropName, nameScopeProp);
        }
    }

    public class IntCompositeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string text = (string)value;
            if (text == null)
            {
                throw new ArgumentException("Missing String Value", "value");
            }
            
            var nameResolver = (IXamlNameResolver)context.GetService(typeof(IXamlNameResolver));
            if (nameResolver == null)
            {
                throw new ArgumentException("Missing IXamlNameResolver", "context");
            }

            var unknownNames = new List<string>();
            foreach (string token in text.Split(' '))
            {
                object namedValue = nameResolver.Resolve(token);
                if (!RecognizedValue(token) && namedValue == null)
                {
                    unknownNames.Add(token);
                }
            }

            if (unknownNames.Count > 0)
            {
                return nameResolver.GetFixupToken(unknownNames);
            }

            return Parse(text, nameResolver);
        }

        bool RecognizedValue(string value)
        {
            int temp;
            return int.TryParse(value, out temp) || value == "(" || value == ")";
        }

        public IntComposite Parse(string value, IXamlNameResolver resolver)
        {
            string[] tokens = value.Split(' ');

            if (tokens.Length == 1)
            {
                return new IntComposite
                {
                    Value = new List<IntCompositeElement>
                    {
                        new IntCompositeLeaf { Value = int.Parse(tokens[0])}
                    }
                };
            }

            int temp;
            return ParseInternal(tokens, 0, out temp, resolver);
        }

        IntComposite ParseInternal(string[] tokens, int startToken, out int lastToken, IXamlNameResolver resolver)
        {
            if (tokens[startToken] != "(")
            {
                throw new FormatException();
            }

            IntComposite root = new IntComposite { Value = new List<IntCompositeElement>() };
            lastToken = startToken;
            for (int currentToken = startToken + 1; currentToken < tokens.Length; currentToken++)
            {
                if (tokens[currentToken] == "(")
                {
                    root.Add(ParseInternal(tokens, currentToken, out currentToken, resolver));
                    continue;
                }

                int value;
                if (int.TryParse(tokens[currentToken], out value))
                {
                    root.Add(new IntCompositeLeaf { Value = value });
                    continue;
                }
                else
                {
                    if (tokens[currentToken] == ")")
                    {
                        lastToken = currentToken;
                        return root;
                    }

                    root.Add((IntCompositeElement)resolver.Resolve(tokens[currentToken]));
                }
            }

            return root;
        }
    }

    public abstract class IntCompositeElement
    {
        public abstract object Value { get; set; }
    }

    public class IntCompositeLeaf : IntCompositeElement
    {
        int _value;
        public override object Value
        {
            get
            {
                return _value;
            }
            set
            {
                this._value = (int)value;
            }
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }

    [TypeConverter(typeof(IntCompositeConverter))]
    public class IntComposite : IntCompositeElement
    {
        List<IntCompositeElement> _children;

        public override object Value
        {
            get
            {
                return _children;
            }
            set
            {
                _children = (List<IntCompositeElement>)value;
            }
        }

        public void Add(IntCompositeElement value)
        {
            _children.Add(value);
        }

        public override string ToString()
        {
            if (_children == null || _children.Count == 0)
            {
                return "empty";
            }

            if (_children.Count == 1 && _children[0] is IntCompositeLeaf)
            {
                return _children[0].ToString();
            }

            StringBuilder finalValue = new StringBuilder("( ");
            foreach (var value in _children)
            {
                finalValue.Append(value.ToString() + " ");
            }

            finalValue.Append(")");
            return finalValue.ToString();
        }

        
    }

    [DictionaryKeyProperty("KeyProperty")]
    public class TypeWithDictionaryKeyProperty
    {
        public object KeyProperty { get; set; }

        public object ItemProperty { get; set; }
    }

    [DictionaryKeyProperty("KeyProperty")]
    public class ItemWithKeyPropertyAndHash
    {
        public object KeyProperty { get; set; }

        public object ItemProperty { get; set; }

        public override int GetHashCode()
        {
            return ItemProperty.GetHashCode();
        }
    }

    public class ValidatingCollection : Collection<TypeWithDictionaryKeyProperty>
    {
        protected override void InsertItem(int index, TypeWithDictionaryKeyProperty item)
        {
            if (item.KeyProperty == null)
            {
                throw new ArgumentNullException("item.KeyProperty");
            }
            base.InsertItem(index, item);
        }
    }

    public class SimpleNode
    {
        public SimpleNode Next { get; set; }
    }
}
