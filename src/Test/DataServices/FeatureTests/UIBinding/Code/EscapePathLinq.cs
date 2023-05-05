// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Text;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// This tests the escaping mechanism we implemented to accomodate support for Linq.
    /// The grammar for the XLinq property path syntax:
    /// [1] Path ::= Property ("." Property)*
    /// [2] Property ::= Axis ("[" NumericalIndex "]" | "[" ExpandedName "]")?
    /// [3] Axis ::= "Element" | "Elements" | "Descendant" | "Descendants" | "Attribute" | "Attributes" | "Nodes"
    /// [4] NumericalIndex ::= Digit+
    /// [5] Digit ::= see http://www.w3.org/TR/REC-xml/#NT-Digit <!--http://www.w3.org/TR/REC-xml/--> 
    /// [6] ExpandedName ::= "{" Uri "}" NCName | NCName 
    /// [7] Uri ::= LegalChar+ 
    /// [8] LegalChar ::= any legal Unicode character 
    /// [9] NCName ::= see http://www.w3.org/TR/REC-xml-names/#NT-NCName <!--http://www.w3.org/TR/REC-xml-names/--> 
    /// Sample document:
    /// <!--e1>
    ///    <e2 a1='A1' a2='A2'/>
    ///    <e2 a1='A1'/>
    ///    <ns2:e2 ns2:a2='A2' xmlns:ns2='NS2'/>
    ///    <e3 a2='A2' xmlns='NS3'/>
    /// </e1-->
    /// Sample queries use the sample document as context:
    /// A. Element[e1] => gets the 'e1' element
    /// B. Element[e1].Elements[e2] => get the 'e2' elements C. Element[e1].Element[e2].Attributes => gets the two attributes on the first 'e2' element D. Elements[e1].Element[{NS2}e2].Attribute[{NS2}a2] => gets the 'n2:a2' attribute on the 'ns2:e2' element E. Descendants[{NS3}e3].Attribute[a2] => gets the 'a2' attribute on the 'e3' element F. Elements[0].Nodes[0] => gets the first 'e2' element
	/// </description>
	/// <relatedBugs>

	/// </relatedBugs>
	/// </summary>
    [Test(3, "Binding", "EscapePathLinq")]
    public class EscapePathLinq : WindowTest
    {
        Random _random;
        FullPath _path;
        TextBlock _tb;
        MyClass _dataItem;
        const Char EscapeChar = '^';
        static readonly string s_specialChars = "^,[]()";
        int _n = 10000;
        int _failures = 0;

        public EscapePathLinq()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Verify);
        }

        TestResult Setup()
        {
            Status("Setup");

            _random = new Random();
            _path = new FullPath(_random);
            _tb = new TextBlock();
            _dataItem = new MyClass();

            return TestResult.Pass;
        }

        TestResult Verify()
        {
            for (int i = 0; i < _n; i++)
            {
                string s = _path.GetRandomString();
                string ss = null;
                bool success = false;

                try
                {
                    Binding b1 = new Binding();
                    b1.Path = new PropertyPath(String.Format("[{0}]", s));
                    b1.Source = _dataItem;
                    _tb.SetBinding(TextBlock.TextProperty, b1);

                    success = (s == _tb.Text);
                }
                catch (Exception)
                {
                    // In case of exception, we should retry with the path escaped.
                }

                // Success is false if the strings passed and returned to the indexer are different or
                // if there was an exception. Either way, we want to escape the path.
                if (!success)
                {
                    // retry with escaped string
                    ss = AddEscapes(s);

                    try
                    {
                        Binding b2 = new Binding();
                        b2.Path = new PropertyPath(String.Format("[{0}]", ss));
                        b2.Source = _dataItem;
                        _tb.SetBinding(TextBlock.TextProperty, b2);

                        success = (s == _tb.Text);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.GetType() + " - " + s);
                    }
                }

                if(!success)
                {
                    StringBuilder errorMessage = new StringBuilder("{0}.  Failed.", i);
                    errorMessage.AppendFormat("  string: {0}", s);
                    errorMessage.AppendFormat("  escape: {0}", ss);
                    errorMessage.AppendFormat("  result: {0}", _tb.Text);
                    LogComment(errorMessage.ToString());

                    ++_failures;
                }

                if (_failures > 0)
                {
                    LogComment(String.Format("Tested {0} strings, found {1} failures.", _n, _failures));
                    return TestResult.Fail;
                }
            }
            return TestResult.Pass;
        }

        string AddEscapes(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Char c in s)
            {
                if (s_specialChars.IndexOf(c) >= 0)
                {
                    sb.Append(EscapeChar);
                }
                sb.Append(c);
            }

            return sb.ToString();
        }

    }

    public class MyClass
    {
        public string this[string s]
        {
            get { return s; }
        }
    }

}
