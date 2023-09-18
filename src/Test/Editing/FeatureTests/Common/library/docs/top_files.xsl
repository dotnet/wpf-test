<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  >
  
<xsl:template name='TopFileTemplate'>

<html>
<head>
<title><xsl:value-of select='./@name' /></title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head>
<frameset cols="30%,*">
<frame name="left"><xsl:attribute name="SRC"><xsl:value-of select='./@name' />Left.htm</xsl:attribute></frame>
<frame name="content"><xsl:attribute name="SRC"><xsl:value-of select='./@name' />Content.htm</xsl:attribute></frame>
</frameset>
</html>

</xsl:template>

<xsl:template name='LeftFileTemplate'>

<html>
<head>
<title><xsl:value-of select='./@name' /></title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head>
<frameset rows='40%,60%'>
<frame name="nsframe"><xsl:attribute name="SRC"><xsl:value-of select='./@name' />Ns.htm</xsl:attribute></frame>
<frame name="typesframe"><xsl:attribute name="SRC"><xsl:value-of select='./@name' />Ns.htm</xsl:attribute></frame>
</frameset>
</html>

</xsl:template>

<xsl:template name='ContentFileTemplate'>

<html>
<head>
<title><xsl:value-of select='./@name' /></title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head>
<body>
<h1><xsl:value-of select='./@name' /></h1>
<p>
Use the frames on the side to navigate the documentation
for the <xsl:value-of select='./@name' /> assembly.
</p>
</body>
</html>

</xsl:template>

<xsl:template name='CssFile'>
body {
  background: lightyellow;
  font-family: Verdana;
  font-size: 9pt;
}
h1 {
  font-size: 200%;
}

h2 {
  font-size; 120%;
  font-weight: normal;
  font-style: italic;
  border-bottom: 1px solid black;
}

h3 {
  font-size: 100%;
  font-weight: bold;
}

th {
  font-size: 9pt;
  background: #CCCCCC;
  border: 1px solid #999999;
}

td {
  font-size: 9pt;
  vertical-align: top;
}

p {
  margin: 8px 0px;
}

.signature {
  font-family: Lucida Console, Courier, Courier New;
  border: 1px solid #666666;
  background: #eeeeee;
  padding: 0px 8px;
  margin-top: 0px;
}

.parametersTitle {
  font-weight: bold;
  margin: 6px 0px;
}

.returnValueTitle {
  font-weight: bold;
  margin: 6px 0px;
}

.threadSafetyTitle {
  font-weight: bold;
  margin: 6px 0px;
}

.exampleTitle {
  font-weight: bold;
  margin: 6px 0px;
}

.remarksTitle {
  font-weight: bold;
  margin: 6px 0px;
}

.hierarchy {
  padding-left: 8px;
}

.hierarchyBlock {
  font-family: Verdana;
  border: 1px solid #666666;
  background: #EEEEEE;
  padding-left: 8px;
  margin-bottom: 4px;
}

.typeMemberBody {
  // filter:progid:DXImageTransform.Microsoft.Gradient(startColorstr='khaki',endColorstr='chocolate');
  filter:progid:DXImageTransform.Microsoft.Gradient(startColorstr='lightyellow',endColorstr='tan');
  border: 1px solid #999999;
  padding: 2px;
  width: 100%;
}

.typeMemberTitle {
  background: #eeeeff;
  border: 1px solid #999999;
  font-weight: bold;
  margin-top: 8px;
  padding-left: 4px;
}

.typeMemberSectionTitle {
  font-weight: bold;
  margin-top: 6px;
}

.preCode {
  background: #eeeeee;
  border: 1px solid #999999;
}

.hintAreaStyle {
  background: khaki;
  border: 1px solid goldenrod;
  height: 38px;
  width: 100%;
}
</xsl:template>

</xsl:stylesheet>
