// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "Code Coverage - Section", MethodName = "Run")]
    public class SectionCodeCoverer : AvalonTest
    {
        private FlowDocument _fd;
        private Section _sec1;
        private Window _w1;

        public SectionCodeCoverer()
        {
            CreateLog = false;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);                                        
        }

        private TestResult Initialize()
        {            
            _w1 = new Window();
            UISetup(_w1, out _fd, out _sec1);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w1.Close();            
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            PassMethods(_sec1);
            _fd.Blocks.Add(_sec1);
            FailMethods(_sec1);      
            return TestResult.Pass;
        }

        private void UISetup(Window w1, out FlowDocument fd, out Section sec1)
        {
            fd = new FlowDocument();
            sec1 = new Section();
            sec1 = new Section(new Paragraph(new Run("Alternative Constructors called")));
            sec1.Blocks.Add(new Paragraph(new Run("Second line")));
            sec1.Blocks.Add(new Paragraph(new InlineUIContainer(new RichTextBox())));
            fd.Blocks.Add(sec1);
            w1.Content= fd;
            w1.Show();
        }

        private void PassMethods(Section sec1)
        {
            TestLog sectionPassMethods = new TestLog("List Pass Methods");
            
            Section sec2 = new Section();
            ReflectionHelper rh = ReflectionHelper.WrapObject(sec2);

            Paragraph p1 = new Paragraph();
            Paragraph p2 = new Paragraph();
            sec1.Blocks.Add(p1);
            sec1.Blocks.Add(p2);

            sectionPassMethods.Result = TestResult.Pass;
            sectionPassMethods.Close();
        }

        private void FailMethods(Section sec1)
        {
            TestLog sectionFailMethods = new TestLog("List Fail Methods");
            
            Section sec2 = new Section();
            Paragraph p1 = new Paragraph();
            Paragraph p2 = new Paragraph();
            sec2.Blocks.Add(p1);
            sec2.Blocks.Clear();

            sectionFailMethods.Result = TestResult.Pass;
            sectionFailMethods.Close();
        }

        private void GetProperties(DerivedFlowDocumentScrollViewer textflow1)
        {
            ReflectionHelper rh = ReflectionHelper.WrapObject(textflow1);
            rh.GetProperty("FormattedLinesCount");
            rh.GetProperty("StructuralCache");
        }

        private void SetProperties(DerivedFlowDocumentScrollViewer textflow1)
        {
            textflow1.Document.LineHeight = 15.0;
            textflow1.Document.TextAlignment = TextAlignment.Center;
        }
    }

}
