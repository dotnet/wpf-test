<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  >

<!--
..............................................................................

User Guide File Descriptions

Provides the file elements to invoke content (implemented as templates),
and the Table Of Contents.

..............................................................................
-->

<xsl:template name='UserGuideFiles'>

<file name='user_guides.htm'>
<html>
<head>
<title>User Guides</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head>
<body>
<h1>User Guides</h1>
<ul>
  <li><a href='about_infrastructure.htm'>About the infrastructure</a>.</li>
  <li><a href='tutorials.htm'>Tutorials</a>.</li>
  <li><a href='command_line_switches.htm'>Command-line Switches</a>.</li>
  <li><a href='data_driven_cases.htm'>Data-Driven Test Cases</a>.</li>
  <li><a href='action_driven_cases.htm'>Action-Driven Test Cases</a>.</li>
  <li><a href='automating_test_matrixes.htm'>Automating Test Matrixes</a>.</li>
  <li><a href='stress_cases.htm'>Stress Test Cases</a>.</li>
  <li><a href='tracking_file_versions.htm'>Tracking File Versions</a>.</li>
</ul>
</body>
</html>
</file>

<file name='about_infrastructure.htm'>
<html><head>
<title>Command-line Switches</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head><body>
<xsl:call-template name='AboutInfrastructure' />
</body></html>
</file>

<file name='tutorials.htm'>
<html><head>
<title>Command-line Switches</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head><body>
<xsl:call-template name='Tutorials' />
</body></html>
</file>

<file name='command_line_switches.htm'>
<html><head>
<title>Command-line Switches</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head><body>
<xsl:call-template name='CommandLineSwitches' />
</body></html>
</file>

<file name='data_driven_cases.htm'>
<html><head>
<title>Data-Driven Test Cases</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head><body>
<xsl:call-template name='DataDrivenTestCases' />
</body></html>
</file>

<file name='action_driven_cases.htm'>
<html><head>
<title>Action-Driven Test Cases</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head><body>
<xsl:call-template name='ActionDrivenTestCases' />
</body></html>
</file>

<file name='automating_test_matrixes.htm'>
<html><head>
<title>Automating Test Matrixes</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head><body>
<xsl:call-template name='AutomatingTestMatrixes' />
</body></html>
</file>

<file name='stress_cases.htm'>
<html><head>
<title>Stress Test Cases</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head><body>
<xsl:call-template name='StressTestCases' />
</body></html>
</file>

<file name='tracking_file_versions.htm'>
<html><head>
<title>Tracking File Versions</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head><body>
<xsl:call-template name='TrackingFileVersions' />
</body></html>
</file>

</xsl:template>

<!--
..............................................................................

User Guide Contents

Provides the actual contents that go into each user guide.

..............................................................................
-->

<xsl:template name='AboutInfrastructure'>
<h1>About the infrastructure</h1>
<p>
Avalon Editing test cases run as part of the Avalon test environment.
There is a wealth of information in the
<a href='http://avalon/test/mentor/handbook/'>Avalon Tester's Handbook</a>
about the pieces that are particular to testing, and in the
<a href='http://avalon/build/'>Avalon Build</a> site about Avalon
setup in general.
</p><p>
Note that much of the reference information is obsolete, but the
<a href='http://avalon/test/mentor/handbook/Setting%20Up/Tool%20Ramping%20Up%20Guide.doc'>
Tool Ramping Up Guide</a> should be fairly up to date.
</p>

<h2>Running outside the Avalon environment</h2>
<p>
If you have an enlistment, you can run test cases without 
installing any supporting infrastructure. To do this, please
refer to 
<a href='http://team/sites/Avalon/editing/Shared%20Documents/Joint%20Test%20Projects/Standalone%20Setup%20For%20Editing%20Testing.mht'>Standalone Setup For Editing Testing</a>.
</p>

<h2>Editing Test Binaries</h2>
<p>
The following paths and binaries are used by Editing test cases.
'$' is used to represent %sdxroot%\testsrc\windowstest\client.
</p>

<table>
  <thead>
    <tr>
      <th>Binary</th>
      <th>Path</th>
      <th>Purpose</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>WTC.Uis.TextBvt.exe</td>
      <td>$\wcptests\uis\text\bvt\ExeTarget</td>
      <td>Main test binary, with entry point. Uses no
        unsafe code and can run in partial trust.</td>
    </tr><tr>
      <td>WTC.Uis.TestLib.dll</td>
      <td>$\wcptests\uis\common\library</td>
      <td>Helper library for tests. Includes bootstrapping for
        test cases, test case selection and execution, product models, 
        various utility classes for comparisons, logging and verification,
        and other shared goodness.</td>
    </tr><tr>
      <td>AutomationFramework.dll</td>
      <td>$\tools\runtime\legacy\AutomationFramework</td>
      <td>Provides the communication between the test cases
        and the reporting infrastructure. Fails silently if infrastructure
        support is not installed. Most test cases speak to 
        WTC.Uis.TestLib.dll directly.</td>
    </tr><tr>
      <td>ClientTestLibrary.dll</td>
      <td>$\wcptests\common\library</td>
      <td>Provides utilities available to all Avalon test teams.</td>
    </tr><tr>
      <td>ClientTestRuntime.dll</td>
      <td>$\tools\runtime\ClientTestRuntime</td>
      <td>Provides basic runtime services to tests. Most test cases
        speak to WTC.Uis.TestLib.dll directly.</td>
    </tr><tr>
      <td>AutoData.dll</td>
      <td>$\tools\runtime\legacy\AutoData</td>
      <td>Provides support for international text and data samples.</td>
    </tr>
  </tbody>
</table>
</xsl:template>

<xsl:template name='Tutorials'>
<h1>Tutorials</h1>
<p>
This section provides a step-by-step guide to creating a test
case for Avalon Editing.
</p><p>
Let's say that we want to write a straightforward test
case, to verify that a text box updates its rendering
when its Text property is modified. We first create a class
that is of <a href='Test.Uis.TestTypes.CustomTestCase'>CustomTestCase</a>
type, and add some attributes to indicate who has written this
case, and a '123' value to flag that we need to add this to the Tactics
database.
</p><p>
Let's append this code to 
%sdxroot%\testsrc\windowstest\client\wcptests\uis\Text\BVT\TextBoxOM\Text.cs.
</p>
<pre class='preCode'>
/// &lt;summary&gt;
/// Verifies that modifying the Text property updates the rendering.
/// &lt;/summary&gt;
[TestOwner("Microsoft"), TestTactics("123")]
public class TextPropertyRenders: CustomTestCase
{
// More code goes here.
}
</pre>
<p>
Now, we can add the code to create a TextBox, capture its default state image,
then change the text and verify that the capture has changed.
</p>
<pre class='preCode'>
public class TextPropertyRenders: <a href='Test.Uis.TestTypes.CustomTestCase.htm'>CustomTestCase</a>
{
    #region Main flow.

    /// &lt;summary&gt;Runs the test case.&lt;/summary&gt;
    public override void RunTestCase()
    {
        // Create a TextBox in the window, and wait until it has rendered.
        _control = new TextBox();
        MainWindow.Content = _control;
        QueueDelegate(AfterFirstRender);
    }
    
    private void AfterFirstRender()
    {
        const string CustomText = "some text"; // Text to display.

        Log("Capturing initial bitmap...");
        _initialCapture = <a href='Test.Uis.Imaging.BitmapCapture.htm'>BitmapCapture</a>.CreateBitmapFromElement(_control);
        
        Log("Modifying text: " + CustomText);
        _control.Text = CustomText;
        QueueDelegate(AfterTextChange);
    }
    
    private void AfterTextChange()
    {
        Bitmap changedCapture;
        Bitmap differences;

        Log("Verifying that bitmaps match...");
        changedCapture = <a href='Test.Uis.Imaging.BitmapCapture.htm'>BitmapCapture</a>.CreateBitmapFromElement(_control);
        <a href='Test.Uis.Loggers.Verifier.htm'>Verifier</a>.Verify(
            !<a href='Test.Uis.Imaging.ComparisonOperationUtils.htm'>ComparisonOperationUtils</a>.AreBitmapsEqual(_initialCapture, changedCapture, out differences),
            "Rendered image changes after Text change.", true);
        
        <a href='Test.Uis.Loggers.Logger.htm'>Logger</a>.Current.ReportSuccess();
    }

    #endregion Main flow.
    
    #region Private fields.
    
    private Bitmap _initialCapture;
    private TextBox _control;
    
    #endregion Private fields.
}
</pre>

<p>
Now we can rebuild WTC.Uis.TextBvt.exe, and copy it again to the work
directory. From there, running WTC.Uis.TextBvt.exe /TestCaseType=TextPropertyRenders
should execute the test case. If the test case manager is not installed, all
logging will be redirected to the console.
</p>

<h2>Combinatorial Cases</h2>
<p>
Now, let's try all controls, with all text scripts, to provide
more thorough coverage for this test. We will change the
test case to be a <a href='Test.Uis.TestTypes.ManagedCombinatorialTestCase.htm'>ManagedCombinatorialTestCase</a>.
</p><p>
There are a number of changes we should make to our test case to make it
combinatorial-driven. First, we should change the base class for our control.
</p>
<pre class='preCode'>
public class TextPropertyRenders: <a href='Test.Uis.TestTypes.ManagedCombinatorialTestCase.htm'>ManagedCombinatorialTestCase</a>
...
</pre>
<p>
Now, because this builds as part of WTC.Uis.TextBvt.exe, we should define
the dimensions that we want to combine. The test library has support for
these in the form of arrays for useful concepts for testing. We declare
these as an entry in %sdxroot%\testsrc\windowstest\client\wcptests\uis\Text\BVT\AssemblyData\AssemblyTestCaseData.cs
</p>
<pre class='preCode'>
public static class AssemblyTestCaseData
{
    ...
    public static TestCaseData[] Data = new TestCaseData[] {
        ...
        new <a href='Test.Uis.TestTypes.TestCaseData.htm'>TestCaseData</a>(typeof(TextPropertyRenders), "",
            new <a href='Test.Uis.Utils.Dimension.htm'>Dimension</a>("EditableType", <a href='Test.Uis.Data.TextEditableType.htm'>TextEditableType</a>.Values),
            new <a href='Test.Uis.Utils.Dimension.htm'>Dimension</a>("CustomText", <a href='Test.Uis.Data.TextScript.htm'>TextScript</a>.Values),
        ...
    }
}
</pre>
<p>
The ManagedCombinatorialTestCase class will read these values into fields, 
so we need to have them declared with the dimension niame in our test
class.
</p>
<pre class='preCode'>
public class TextPropertyRenders: <a href='Test.Uis.TestTypes.ManagedCombinatorialTestCase.htm'>ManagedCombinatorialTestCase</a>
{
    ...
    #region Private fields.
    
    private Bitmap _initialCapture;

    /// &lt;summary&gt;Control being tested.&lt;/summary&gt;
    private FrameworkElement _control;
    
    /// &lt;summary&gt;Type of editable control to test.&lt;/summary&gt;
    private TextEditableType EditableType;
    
    /// &lt;summary&gt;Script with custom text to check for updates.&lt;/summary&gt;
    private TextScript CustomText;
    
    #endregion Private fields.
}
</pre>
<p>
The entry point for these test cases changes to DoRunCombination, and
the final call should be to NextCombination, to indicate that the
next combination should run. The test case will log success when
all combinations have passed.
</p><p>
Another thing to change in our main method is MainWindow.Content.
Instead, we will use TestElement, which allows us to keep a piece
of UI displaying the currently executing combination values.
</p><p>
Finally, we will create a FrameworkElement from the editable type,
rather than a hard-coded TextBox.
</p>
<pre class='preCode'>
public class TextPropertyRenders: <a href='Test.Uis.TestTypes.CustomTestCase.htm'>CustomTestCase</a>
{
    #region Main flow.

    /// &lt;summary&gt;Runs the current combination.&lt;/summary&gt;
    public override void DoRunCombination()
    {
        // Create a control in the window, and wait until it has rendered.
        _control = <a href='Test.Uis.Data.TextEditableType.htm'>EditableType</a>.CreateInstance();
        TestElement = _control;
        QueueDelegate(AfterFirstRender);
    }
...
</pre>
<p>
To set the text, we will use a helper wrapper, that abstracts the differences
between TextBox, RichTextBox, PasswordBox and others. We will also
use the CustomText value rather than a hard-coded string.
</p>
<pre class='preCode'>
private void AfterFirstRender()
{
    Log("Capturing initial bitmap...");
    _initialCapture = <a href='Test.Uis.Imaging.BitmapCapture.htm'>BitmapCapture</a>.CreateBitmapFromElement(_control);

    Log("Modifying text: " + CustomText.Sample);
    (new <a href='Test.Uis.Wrappers.UIElementWrapper.htm'>UIElementWrapper</a>(_control)).Text = <a href='Test.Uis.Data.TextScript.htm'>CustomText</a>.Sample;
    QueueDelegate(AfterTextChange);
}
</pre>
<p>
In the last method, the only change required (not shown) is Logger.Current.ReportSuccess()
to NextCombination().
</p><p>
Now, try building and running this test, and you will see how all controls
are exercised with all text scripts.
</p>

</xsl:template>

<xsl:template name='DataDrivenTestCases'>
<h1>Data-Driven Test Cases</h1>
<p>
Test case development provides many opportunities for
code reuse. Suppose we have the following test case,
that verifies that text can be selected programmatically
and the APIs agree on the values.
</p>

<pre class='preCode'>...
TextBox textbox = /* some textbox */ ;
textbox.Text = "123";
textbox.SelectionStart = 0;
textbox.SelectionLength = 2;
Verifier.Verify(textbox.SelectedText == "12");
</pre>

<p>
By removing the hard-coded values, we can re-use this test
case and test how the code will behave with different
languages, surrogate pairs, and test the boundary conditions
for the first and last characters. We can also verify
that exceptions are throw if that is what we expect.
</p><p>
To do this, the ConfigurationSettings class gives the test
case access to settings defined someplace else (more on
this later).
</p>

<pre class='preCode'>...
string text = ConfigurationSettings.Current.GetArgument("Text", true);
int start = ConfigurationSettings.Current.GetArgumentAsInt("Start", true);
int length = ConfigurationSettings.Current.GetArgumentAsInt("Length", true);
string expectedSelectedText = 
    ConfigurationSettings.Current.GetArgumentAsInt("ExpectedSelectedText");
string expectedException = 
    ConfigurationSettings.Current.GetArgumentAsInt("ExpectedException");

TextBox textbox = /* some textbox */ ;
textbox.Text = text;
try
{
  textbox.SelectionStart = start;
  textbox.SelectionLength = length;
  Verifier.Verify(textbox.SelectedText == expectedSelectedText);
}
catch(Exception exception)
{
  if (expectedException != String.Empty)
  {
      Verifier.Verify(exception.GetType().Name == expectedException);
  }
  else
  {
    throw;
  }
}
</pre>

<p>
Note the following about the code above.
</p>
<ul>
  <li>The test case doesn't know where the settings are coming from.
    This makes it easier to give it input from different sources,
    such as environment variables, command-line arguments, or
    a configuration file.</li>
  <li>The first arguments are required for the test case to run, and
    therefore the second argument is true (throw if missing).</li>
  <li>If we expect an exception, we can use the ExpectedException
    setting to match its type. If we don't have this set, we re-throw
    the exception with the throw keyword (rather than <code>throw
    exception</code>, which would cause the stack to be re-captured
    and the original stack to be lost).</li>
</ul>

<p>
To make the code more readable and easier to change, sometimes
it's a good idea to include all arguments in a separate region,
and access them as properties on the test case class.
</p>

<pre class='preCode'>...
#region Settings.

public string Text
{
  get { return ConfigurationSettings.Current.GetArgument("Text", true); }
}

public int Start
{
  get { return ConfigurationSettings.Current.GetArgumentAsInt("Start", true); }
}

public int Length
{
  get { return ConfigurationSettings.Current.GetArgumentAsInt("Length", true); }
}

public string ExpectedSelectedText
{
  get { return ConfigurationSettings.Current.GetArgument("ExpectedSelectedText"); }
}

public string ExpectedException
{
  get { return ConfigurationSettings.Current.GetArgument("ExpectedException"); }
}

#endregion Settings.

...

TextBox textbox = /* some textbox */ ;
textbox.Text = Text;
try
{
  textbox.SelectionStart = Start;
  textbox.SelectionLength = Length;
  Verifier.Verify(textbox.SelectedText == ExpectedSelectedText);
}
catch(Exception exception)
{
  if (ExpectedException != String.Empty)
  {
      Verifier.Verify(exception.GetType().Name == ExpectedException);
  }
  else
  {
    throw;
  }
}
</pre>

<p>
Finally, if you are subclassing CustomTestCase, you can use the
<code>Settings</code> property to refer to
<code>ConfigurationSettings.Current</code> directly.
</p><p>
Also, you can use the <code>TestArgument</code> attribute to
document your usage of settings. Simply add one attribute with
name and description for each setting your test case uses, and
they will be logged together with your test case.
</p>

</xsl:template>

<xsl:template name='ActionDrivenTestCases'>
<h1>Action-Driven Test Cases</h1>
<p>
Action-driven test cases are the logical step after
data-driven test cases.
</p><p>
Just like data-driven test cases are useful when your
code is a template and data items are interchangeable
building blocks, action-driven test cases are useful
when it's the code itself that plays the role of
a building block. This makes action-driven test cases
especially useful when the actions and verification operations
are well defined and well encapsulated, eg in test
cases based around input emulation.
</p><p>
To write action-driven test cases, you can write an XML
description of the sequence of actions to take. These
actions are then carried out by a test case class, typically
the same for all action-driven test cases. For cases written
for the <b>WTC.Uis.TextBvt.exe</b> assembly, the test
case type is <code>ActionDrivenTest</code>.
</p><p>
There are many static methods ready to be used in the
<a href='Test.Uis.Wrappers.ActionItemWrapper.htm'>ActionItemWrapper</a>
class.
</p><p>
Action-driven test cases always have their steps declared in
an XML file, in element tags called Action. Actions elements 
can have attributes and sub-elements. The following
attributes can be specified on an Action tag.
</p>
<ul>
  <li><b>Name</b>: not required, but allows the action to be
    easily referred to from elsewhere.</li>
  <li><b>Type</b>: required for all actions, 
    type of action to take (eg: StaticMethod).</li>
  <li><b>MethodName</b>: required for most actions,
    name of method to invoke (eg: GetArgument).</li>
  <li><b>ClassName</b>: required for most actions,
    class on which to take an action (eg: Test.Uis.Wrappers.ActionItemWrapper).</li>
  <li><b>UseWorkerThread</b>: optional. Default is false.
    If this value is true this action item runs in a worker thread. By 
    default the action item runs in the main dispatcher thread.</li>
</ul>
<p>
Every Action tag can have Param tags inside of it. These parameters
are typically argument values for a method call. For instance methods,
the first parameter is the instance on which to invoke the method,
the second the first argument, and so on.
For static methods, the first parameter is the first argument.
One of the following attributes must be found on each Param tag.
</p>
<ul>
  <li><b>Value</b>: used to specify a string value.</li>
  <li><b>RetrieveFromReturnValue</b>: used to specify an
    object value that is the return value of another
    action item; may be an index number of the action's Name value.</li>
</ul>
<p>
Actions that have return values store these values for the duration
of the test case, and can be referred to from other action parameters
using the RetrieveFromReturnValue attribute.
</p><p>
Actions are always executed in declaration order, allowing the application
to go idle after each one, to ensure that all side-effects (like pending
layout or rerendering) have already taken place by the time the
next action is executed.
</p>

</xsl:template>

<xsl:template name='CommandLineSwitches'>
<h1>Command-line Switches</h1>
<p>
The following command-line switches are currently in use by the common
library. Please do not use these for test case argument values.
</p>
<ul>
  <li><b>xml</b>: this switch is used to specify the name of the file with
    XML data for test cases. If omitted, it defaults to <i>testxml.xml</i>.</li>
  <li><b>CombFile</b>: this switch is used to specify the name of the file
    with XML data for combinatorial test case. It may be <i>testxml.xml</i>,
    but this makes the startup code run in combinatorial mode.</li>
  <li><b>RedirectOutput</b>: this switch is used by the startup code to
    redirect the default logger to the specified file name, rather than
    logging to AutomationFramework. Also used by the combinatorial engine
    to coordinate feedback.</li>
  <li><b>TestCaseType</b>: this switch is used by the startup code to
    find the specified type and run it. The method with TestCaseEntryPoint
    will be run on an instance of this type.</li>
  <li><b>TestName</b>: this switch is used by the configuration settings
    object to retrieve a specific block of settings from the XML data file.</li>
  <li><b>NoExit</b>: this boolean switch is used by action-based test cases to
    avoid closing the test case. <b>Note</b>: this should only be used
    for interactive investigation.</li>
  <li><b>WaitForDebuggerAttach</b>: The lanched process with /WaitForDebuggerAttach:true
    will show a dialog, waiting for user to attach a debugger to the process. Process id
    is shown on the dialog. The reason is that Rascal / VS.Net doesn't allow you to launch
    a new debuggee process on a remote machine so attach to process is the only option you 
    have. This switch buys you some time to attach if the debuggee only exists for a second
    or two.</li>
  <li><b>CreateVersionReport</b>: Writes out an HTML document with information
    about all the versioned files used to build the test binaries.</li>
<!--
  <li><b></b>:.</li>
-->
</ul>
</xsl:template>

<xsl:template name='AutomatingTestMatrixes'>
<h1>Automating Test Matrixes</h1>
<p>
Test matrixes are simply combinations of two or more variables. The
simplest case is two variables, and the test matrix is a table.
</p><p>
For example, the following test matrix shows what formatting 
commands are available and what controls we can use them on.
</p>
<table>
	<tbody>
		<tr><td></td>
			<td>TextBox</td><td>RichTextBox</td><td>PasswordBox</td></tr>
	  <tr><td>Bold</td></tr>
	  <tr><td>Italic</td></tr>
	  <tr><td>Underline</td></tr>
	</tbody>
</table>
<p>
What values do we want to include in each column? Well, only values
that are different enough amongs themselves to make a difference.
This can be likened to the general idea of 'equivalence partitioning'
for a test input (consider valid and invalid values!).
</p><p>
So, if we
wanted to try formatting commands on different spans of text, we
would consider 'all text', 'empty text', 'all text but no selection',
'first character', 'last character', 'no selection on first character',
'no selection on last character', 'whole paragraph', 'whole line', etc.
This could be a long list, if there are many interesting cases, but
is certainly more manageable than every possible text that can
be held in a text box.
</p><p>
Classic places to look for interesting values are regular valid values,
invalid values, boundary cases, and right before and after boundary
cases.
</p><p>
The following list enumerates some values defined in the common library.
The more values we have in this list, the more we can all leverage
each other's work and test new interesting cases easily.
</p>
<ul>
	<li><a href='Test.Uis.Utils.TextEditableType.htm'>Text Editable Types</a>.
		This class is a classic example of how we can populate and leverage
		interesting values for our test cases to use in a more comprehensive
		fashion. The class exposes information about types and what they
		can be used for (eg: Text supports plain text, TextPanel supports
		rich text), and so they make for excellent inclusion in
		matrix dimensions.</li>
	<li><a href='Test.Uis.Utils.TextElementType.htm'>Text Element Types</a>.
		This class is also another classic example of a list of interesting
		things that can be used directly in a test matrix. In this case,
		it's a simple array with the types of known TextElement types.</li>
	<li><a href='Test.Uis.Utils.TextScript.htm'>Text Script</a>.
		Like the text editable type, this is also a more table-like
		approach to providing test cases with information about how
		they can test multiple text scripts.</li>
</ul>
<p>
The following list points to some helper classes that can be used to
leverage the values defined. Especially useful is the combinatorial
engine, as it can be used to drive your test case through all
interesting combinations of values.
</p>
<ul>
	<li><a href='Test.Uis.Utils.CombinatorialEngine.htm'>Combinatorial Engine</a>.
		This class generates combinations of values. To use this class effectively,
		you need to write your test case in such a way so as to clean-up after
		running with a given combination of values, and then re-run with
		a new combination. The most difficult part is coming up with the
		code that will understand a given combination and do the right
		validation.</li>
  <li><a href='Test.Uis.TestTypes.CustomCombinatorialTestCase.htm'>Custom Combinatorial Test Case</a>.
    This class provides a supertype for test cases that are driven
    by a combinatorial engine. The class has methods that need
    to be overriden to define dimensions, filter values, and
    execute tests for combinations.</li>
  <li><a href='Test.Uis.TestTypes.ManagedCombinatorialTestCase.htm'>Managed Combinatorial Test Case</a>.
    This class provides a supertype for combinatorial test cases that 
    have their dimension definition in an in-memory table, and
    reads values directly into fields before combination execution.</li>
	<li><a href='Test.Uis.Utils.ReflectionUtils.htm'>Reflection Utilities</a>.
		This class can be used to refer to members to call or set with
		less coupling. One of the advantages of the combinatorial engine
		is that your values can be strongly typed, so try to work around
		using this class. However, this class can help you out in situations
		where this is not enough.</li>
	<li><a href='Test.Uis.Utils.XamlUtils.htm'>XAML Utilites</a>.
		This class allows you to manipulate text and XAML, and makes
		it very easy to create Avalon trees from a string and to
		validate text ranges in text controls.</li>
	<li><a href='Test.Uis.Utils.XPathNavigatorUtils.htm'>XPath Navigator Utilities</a>.
		This class also helps to keep your test cases simple and data-driven
		by allowing you to discover elements in the visual tree, the
		automation tree, or a text container 'tree', from a simple
		string.</li>
</ul>
</xsl:template>

<xsl:template name='StressTestCases'>
<h1>Stress Test Cases</h1>
<p>
Help for the stress support classes comes here.
</p>
</xsl:template>

<xsl:template name='TrackingFileVersions'>
<h2>How to see a report of file versions tracked</h2>
<p>
Run the following commands with your files setup as if to run a test case.
</p><p><code>
WTC.Uis.TextBvt.exe /CreateVersionReport=True &gt; htm.htm<br />
start htm.htm
</code></p>

<h2>How to add a file to the versioning report system</h2>
<p>
Edit the file with the following command.
</p><p><code>
sd edit -t text+k myfile.cs
</code></p><p>
This tells Source Depot that you will be editing the file, and that the 
type of the file is now a text file with keyword expansion.
</p><p>
Add the following line at the top of the file (look for 
<b>TextRangeSerializationTest.cs</b> for an example).
</p><p><code>
[assembly: Test.Uis.Management.VersionInformation("$Author$ $Change$ $Date$ $Revision$ $Source$")]
</code></p><p>
When you checkin the file, the keywords will be expanded, and so 
examining the file will show you the new values. Building will 
include all the information in the assembly.
</p>
</xsl:template>

</xsl:stylesheet>
