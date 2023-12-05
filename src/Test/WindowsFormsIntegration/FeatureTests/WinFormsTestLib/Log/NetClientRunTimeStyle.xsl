<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <html>
            <head>
                <title></title>
                <meta name="save" content="history" />
                <style type="text/css">
.save{
   behavior:url(#default#savehistory);}
a.dsphead{
   text-decoration:none;
   margin-left:1.5em;}
a.dsphead:hover{
   text-decoration:underline;}
a.dsphead span.dspchar{
   font-family:monospace;
   font-weight:normal;}
.dspcont{
   display:none;
   margin-left:1.5em;}
.dspblock{
   display:block;
   margin-left:1.5em;}
</style>
                <script type="text/javascript">	
	d = document;
	
	function toggleSection(sectionId)
	{
	   if (d.getElementById)
	   {
	      currSection = d.getElementById(sectionId);
	   
	      if (currSection.style.display == "block")
	      {
	         currSection.style.display = "none";
	      }
	      else
	      {
	         currSection.style.display = "block";
	      }
	   }
	}
</script>
                <script type="text/javascript">

function Scenariodsp(loc){
   if(document.getElementById){
      var foc=loc.firstChild;
      var styleNode = loc.parentNode.parentNode.nextSibling.nextSibling.nextSibling;	
      foc.innerHTML=foc.innerHTML=='+'?'-':'+';
      styleNode.style.display=styleNode.style.display=='block'?'none':'block';}}  

function ScenarioGroupdsp(loc){
   if(document.getElementById){
      var foc=loc.firstChild;
      var styleNode = loc.parentNode.parentNode.nextSibling.nextSibling.nextSibling;	
      foc.innerHTML=foc.innerHTML=='+'?'-':'+';
      styleNode.style.display=styleNode.style.display=='none'?'block':'none';}} 

</script>
                <noscript>
                    <style type="text/css">
.dspcont{display:block;}
</style>
                </noscript>
            </head>
            <body bgcolor="fcfcfc">
                <div class="save">
                    <span style="font-family: verdana; font-size: 8pt">
                        <xsl:apply-templates/>
                    </span>
                </div>
            </body>
        </html>
    </xsl:template>
    <xsl:template match="Testcase">
        <table bgcolor="eeeeff" width="600">
            <tr>
                <td>
                    <h4>Testcase: <xsl:value-of select="@name"/></h4>
                </td>
            </tr>
            <tr>
                <td>
                    <xsl:choose>
                        <xsl:when test="FinalResults/@type = 'Fail'">
                            <span style="color:#bb0000;background-color:#ffbbbb">
                                <span style="font-family: verdana; font-size: 9pt">
Failed: <xsl:value-of select="FinalResults/@fail"/>/<xsl:value-of select="FinalResults/@total"/>
</span>
                            </span>
                        </xsl:when>
                        <xsl:otherwise>
                            <span style="color:#004400;background-color:#bbffbb">
                                <span style="font-family: verdana; font-size: 9pt">
Passed: <xsl:value-of select="FinalResults/@total"/>
</span>
                            </span>
                        </xsl:otherwise>
                    </xsl:choose>
                </td>
            </tr>
            <tr>
                <td>
                    <a href="javascript:toggleSection(1)">
                        <span style="font-family: verdana; font-size: 9pt">Testcase Initialize Information</span>
                    </a>
                </td>
            </tr>
        </table>
        <table bgcolor="eeeeff" width="600" id="1" style="display=none">
            <xsl:apply-templates select="CommandLineParameter"/>
            <xsl:apply-templates select="TestInitialize"/>
        </table>
        <xsl:for-each select="child::node()">
            <xsl:choose>
                <xsl:when test="starts-with(name(), 'CommandLineParameter')"></xsl:when>
                <xsl:when test="starts-with(name(), 'TestInitialize')"></xsl:when>
                <xsl:when test="starts-with(name(), 'ScenarioGroup')">
                    <xsl:apply-templates select="."/>
                </xsl:when>
                <xsl:when test="starts-with(name(), 'FinalResults')"></xsl:when>
                <xsl:otherwise>
                    <xsl:call-template name="line-breaks">
                        <xsl:with-param name="text" select="."/>
                    </xsl:call-template>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:for-each>
    </xsl:template>
    <xsl:template match="CommandLineParameter">
        <tr>
            <td>
                <span style="font-family: verdana; font-size: 8pt">
CommandLineParamter: <xsl:value-of select="@name"/>/<xsl:value-of select="@value"/>
</span>
            </td>
        </tr>
    </xsl:template>
    <xsl:template match="TestInitialize">
        <tr>
            <td>
                <span style="font-family: verdana; font-size: 8pt">
                    <xsl:call-template name="line-breaks">
                        <xsl:with-param name="text" select="."/>
                    </xsl:call-template>
                </span>
            </td>
        </tr>
    </xsl:template>
    <xsl:template match="ScenarioGroup">
        <p>
            <hr/>
            <xsl:choose>
                <xsl:when test="ClassResults/@type = 'Fail'">
                    <span style="color:#bb0000;background-color:#ffbbbb">
                        <h5 align="center">
                            <a href="javascript:void(0)" class="dsphead" onclick="ScenarioGroupdsp(this)">
                                <span class="dspchar">-</span> Scenario Group <xsl:value-of select="@name"/>
  </a>
Failed: <xsl:value-of select="ClassResults/@fail"/>/<xsl:value-of select="ClassResults/@total"/>
</h5>
                    </span>
                </xsl:when>
                <xsl:otherwise>
                    <span style="color:#004400;background-color:#bbffbb">
                        <h5 align="center">
                            <a href="javascript:void(0)" class="dsphead" onclick="ScenarioGroupdsp(this)">
                                <span class="dspchar">-</span> Scenario Group <xsl:value-of select="@name"/>
  </a>
Passed: <xsl:value-of select="ClassResults/@total"/>
</h5>
                    </span>
                </xsl:otherwise>
            </xsl:choose>
            <br/>
            <br/>
            <div class="dspblock">
                <xsl:for-each select="child::node()">
                    <xsl:choose>
                        <xsl:when test="starts-with(name(), 'Scenario')">
                            <xsl:apply-templates select="."/>
                        </xsl:when>
                        <xsl:when test="starts-with(name(), 'ExcludedScenario')">
                            <xsl:apply-templates select="."/>
                        </xsl:when>
                        <xsl:when test="starts-with(name(), 'ClassResults')">
                            <xsl:apply-templates select="."/>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:call-template name="line-breaks">
                                <xsl:with-param name="text" select="."/>
                            </xsl:call-template>
                        </xsl:otherwise>
                    </xsl:choose>
                </xsl:for-each>
            </div>
        </p>
    </xsl:template>
    <xsl:template match="FinalResults"></xsl:template>
    <xsl:template match="Scenario">
        <xsl:choose>
            <xsl:when test="Result/@type = 'Fail'">
                <span style="color:#bb0000;background-color:#ffbbbb">
                    <span style="font-family: verdana; font-size: 8pt">
                        <a href="javascript:void(0)" class="dsphead" onclick="Scenariodsp(this)">
                            <span class="dspchar">+</span> Scenario <xsl:value-of select="@method"/> - <xsl:value-of select="@name"/>
  </a>
Failed: <xsl:value-of select="Result/@fail"/>/<xsl:value-of select="Result/@total"/>
</span>
                </span>
                <br/>
                <br/>
            </xsl:when>
            <xsl:otherwise>
                <span style="color:#004400;background-color:#bbffbb">
                    <span style="font-family: verdana; font-size: 8pt">
                        <a href="javascript:void(0)" class="dsphead" onclick="Scenariodsp(this)">
                            <span class="dspchar">+</span> Scenario <xsl:value-of select="@method"/> - <xsl:value-of select="@name"/>
  </a>
Passed: <xsl:value-of select="Result/@total"/>
</span>
                </span>
                <br/>
                <br/>
            </xsl:otherwise>
        </xsl:choose>
        <div class="dspcont">
            <xsl:for-each select="child::node()">
                <xsl:choose>
                    <xsl:when test="starts-with(name(), 'ExpectedActual')">
                        <xsl:apply-templates select="."/>
                    </xsl:when>
                    <xsl:when test="starts-with(name(), 'KnownBug')">
                        <xsl:apply-templates select="."/>
                    </xsl:when>
                    <xsl:when test="starts-with(name(), 'Exception')">
                        <xsl:apply-templates select="."/>
                    </xsl:when>
                    <xsl:when test="starts-with(name(), 'InnerException')">
                        <xsl:apply-templates select="."/>
                    </xsl:when>
                    <xsl:when test="starts-with(name(), 'Result')"></xsl:when>
                    <xsl:when test="starts-with(name(), 'ResultComments')"></xsl:when>
                    <xsl:otherwise>
                        <xsl:call-template name="line-breaks">
                            <xsl:with-param name="text" select="."/>
                        </xsl:call-template>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:for-each>
            <br/>
        </div>
    </xsl:template>
    <xsl:template match="ExpectedActual">
        <br/>
        <b>Expected Value:</b>
        <xsl:value-of select="Expected"/>
        <br/>
        <b>Expected Type:</b>
        <xsl:value-of select="Expected/@type"/>
        <br/>
        <b>Actual Value:</b>
        <xsl:value-of select="Actual"/>
        <br/>
        <b>Actual Type:</b>
        <xsl:value-of select="Actual/@type"/>
        <br/>
    </xsl:template>
    <xsl:template match="KnownBug">
        <pre>
            <span style="font-family: verdana; font-size: 8pt">
                <b>Known Bug: DB:</b>
                <xsl:value-of select="@db"/>
                <b> ID:</b>
                <xsl:value-of select="@id"/>
                <b> URL:</b>
                <xsl:value-of select="@url"/>
            </span>
        </pre>
        <xsl:call-template name="line-breaks">
            <xsl:with-param name="text" select="."/>
        </xsl:call-template>
    </xsl:template>
    <xsl:template match="Exception">
        <br/>
        <b>
            <xsl:value-of select="@type"/>
        </b>
        <br/>
        <xsl:apply-templates />
        <br/>
    </xsl:template>
    <xsl:template match="InnerException">
        <br/>
        <b>
            <xsl:value-of select="@type"/>
        </b>
        <br/>
        <xsl:apply-templates />
        <br/> 
******** End of inner exception ********
<br/>
</xsl:template>
    <xsl:template match="Message">
        <xsl:call-template name="line-breaks">
            <xsl:with-param name="text" select="."/>
        </xsl:call-template>
    </xsl:template>
    <xsl:template match="StackTrace">
        <xsl:apply-templates select="Frame" />
    </xsl:template>
    <xsl:template match="Frame">
at <xsl:value-of select="@function"/> in <xsl:value-of select="@file"/> line <xsl:value-of select="@line"/>
<br/>
</xsl:template>
    <xsl:template match="Text"></xsl:template>
    <xsl:template match="ClassResults">
        <xsl:for-each select="child::node()">
            <xsl:choose>
                <xsl:when test="starts-with(name(), 'ExtraScenario')">
                    <xsl:apply-templates select="."/>
                </xsl:when>
                <xsl:when test="starts-with(name(), 'MissingScenario')">
                    <xsl:apply-templates select="."/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:call-template name="line-breaks">
                        <xsl:with-param name="text" select="."/>
                    </xsl:call-template>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:for-each>
    </xsl:template>
    <xsl:template match="ExtraScenario">
        <span style="color:#000077;background-color:#bbbbff">
            <b>Extra Scenario: </b>
            <xsl:value-of select="@name"/>
        </span>
        <br/>
    </xsl:template>
    <xsl:template match="ExcludedScenario">
        <span style="color:#bb0000;background-color:#ffbbbb">
            <b>Excluded Scenario: </b>
            <xsl:value-of select="@name"/>
        </span>
        <br/>
    </xsl:template>
    <xsl:template match="MissingScenario">
        <span style="color:#bb0000;background-color:#ffbbbb">
            <b>Missing Scenario: </b>
            <xsl:value-of select="@name"/>
        </span>
        <br/>
    </xsl:template>
    <xsl:template name="line-breaks">
        <xsl:param name="text"/>
        <xsl:choose>
            <xsl:when test="contains($text,'&#xA;')">
                <xsl:value-of select="substring-before($text,'&#xA;')"/>
                <br/>
                <xsl:call-template name="line-breaks">
                    <xsl:with-param name="text" select="substring-after($text,'&#xA;')"/>
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$text"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
</xsl:stylesheet>
