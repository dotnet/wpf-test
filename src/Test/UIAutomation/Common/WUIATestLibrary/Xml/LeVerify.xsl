<?xml version="1.0"?>

<!-- File Name: XsltDemo02.xsl -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <HTML>
      <head>
      </head>
      <BODY>
        <B>
          <font color="#008080">
            <U>
              Results
            </U>
          </font>
        </B>
        <table border="1" width="100%" id="table1">
          <tr>
            <td width="130" bgcolor="#C0C0C0">
              <B>Pass</B>
            </td>
            <td width="30" bgcolor="#EFEBDE">
              <xsl:value-of select="TESTRUN/RESULTS/@NumberPass"/>
            </td>
            <td rowspan="3" bgcolor="#EFEBDE">
              <ul>
                <li>
                  <font color="#008080" size="2">
                    <b>
                      <u>Documented Test Steps :</u>
                    </b>
                  </font>
                  <font size="2">Planned steps to be taken to test the object according to the test plan</font>
                </li>

                <li>
                  <font color="#008080" size="2">
                    <b>
                      <u>
                        Actual Trace :
                      </u>
                    </b>
                  </font>
                  <font size="2">
                    This is the actual output of the test run, equivalent to the Debug.Trace() statements.  Numbers correspond to the test step in <B>Documented Test Steps</B>
                  </font>
                </li>

                <li>
                  <font color="#008080" size="2">
                    <b>
                      <u>
                        Code to Run the Test :
                      </u>
                    </b>
                  </font>
                  <font size="2">Sample code that can be placed in a console application and ran to execute the test.  Required UIVerify.</font>
                </li>
              </ul>
            </td>
          </tr>
          <tr>
            <td width="130" bgcolor="#C0C0C0">
              <B>Fail</B>
            </td>
            <td width="30" bgcolor="#EFEBDE">
              <xsl:value-of select="TESTRUN/RESULTS/@NumberFail"/>
            </td>
          </tr>
          <tr>
            <td width="130" bgcolor="#C0C0C0">
              <B>Unexpected Fails</B>
            </td>
            <td width="30" bgcolor="#EFEBDE">
              <xsl:value-of select="TESTRUN/RESULTS/@NumberUnexpectedFails"/>
            </td>
          </tr>
        </table>

        <BR></BR>
        <B>
          <font color="#008080">
            <U>
              Tests that Failed
            </U>
          </font>
        </B>

        <xsl:for-each select="TESTRUN/TEST">
          <xsl:if test="RESULTS/@Status='ERROR'">
            <table border="1" width="100%" id="table1">
              <tr>
                <td width="130" bgcolor="#C0C0C0">
                  <B>Test name</B>
                </td>
                <td>
                  <xsl:value-of select="@TestName"/>
                </td>
              </tr>
              <tr>
                <td width="130" bgcolor="#C0C0C0">
                  <B>Summary</B>
                </td>
                <td>
                  <xsl:value-of select="@Summary"/>
                </td>
              </tr>
              <tr>
                <td width="130" bgcolor="#C0C0C0">
                  <B>Priority</B>
                </td>
                <td>
                  <xsl:value-of select="@TestPrority"/>
                </td>
              </tr>
              <tr>
                <td width="130" bgcolor="#C0C0C0">
                  <B>TestCaseType</B>
                </td>
                <td>
                  <xsl:value-of select="@TestCaseType"/>
                </td>
              </tr>
              <tr>
                <td width="130" bgcolor="#C0C0C0">
                  <B>Element</B>
                </td>
                <td>
                  <xsl:value-of select="@Element"/>
                </td>
              </tr>
              <tr>
                <td width="130" bgcolor="#C0C0C0">
                  <B>Failure</B>
                </td>
                <td>
                  <font color="#FF0000">
                    <xsl:value-of select="RESULTS/@Reason"/>
                  </font>
                </td>
              </tr>
            </table>
            <!--- Title code steps -->
            <BR></BR>
            <B>
              <font color="#008080">
                <U>
                  Documented Test Steps
                </U>
              </font>
            </B>
            <BR>
              <xsl:for-each select="STEPS/STEP">
                <p style="text-indent: -18px; margin-left: 18px; margin-top:0; margin-bottom:0">
                  <xsl:value-of select="."/>
                </p>
              </xsl:for-each>
            </BR>
            <BR></BR>
            <!--- Trace -->
            <B>
              <font color="#008080">
                <U>
                  Actual Trace
                </U>
              </font>
            </B>
            <xsl:for-each select="COMMENT">
              <p style="text-indent: -18px; margin-left: 18px; margin-top:0; margin-bottom:0">
                <xsl:value-of select="."/>
              </p>
            </xsl:for-each>
            <font color="#FF0000">
              <xsl:value-of select="RESULTS/@Reason"/>
            </font>

            <!--- Test steps -->
            <BR></BR>
            <BR></BR>
            <B>
              <font color="#008080">
                <U>
                  Code to Run the Test
                </U>
              </font>
            </B>
            <PRE>
using System;
using InternalHelper;
using System.Windows.Automation;
using Microsoft.Test.WindowsUIAutomation;
using Microsoft.Test.WindowsUIAutomation.Logging;

namespace TestBug
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      AutomationElement element;
      UIVerifyLogger.SetLoggerType(LogTypes.ConsoleLogger);
      <xsl:value-of select="PATH/."/><BR></BR>
      <![CDATA[      ]]><xsl:value-of select="CALL/."/>
      UIVerifyLogger.ReportResults();
    }
  }
}
            </PRE>
          </xsl:if>
        </xsl:for-each>
      </BODY>
    </HTML>
  </xsl:template>

</xsl:stylesheet>


