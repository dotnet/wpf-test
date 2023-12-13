<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:myscripts='urn:yada-yada'>

<xsl:include href='scripts.xsl' />
<xsl:include href='guides.xsl' />
<xsl:include href='sections.xsl' />
<xsl:include href='top_files.xsl' />
<xsl:include href='type_page.xsl' />

<xsl:template match='/assembly'>

<multiple-files>

<!--
Write out the top-level files with frames to handle navigation.
-->
<file><xsl:attribute name='name'><xsl:value-of select='./@name' />Assembly.htm</xsl:attribute>
<xsl:call-template name='TopFileTemplate' />
</file>

<file><xsl:attribute name='name'><xsl:value-of select='./@name' />Content.htm</xsl:attribute>
<xsl:call-template name='ContentFileTemplate' />
</file>

<file><xsl:attribute name='name'><xsl:value-of select='./@name' />Left.htm</xsl:attribute>
<xsl:call-template name='LeftFileTemplate' />
</file>

<file><xsl:attribute name='name'>docstyles.css</xsl:attribute>
<xsl:call-template name='CssFile' />
</file>

<!--
Wite out the user guides.
-->
<xsl:call-template name='UserGuideFiles' />

<!--
The file with the assembly and namespaces suffix is used to hold the namespaces.
-->
<file><xsl:attribute name='name'><xsl:value-of select='./@name' />Ns.htm</xsl:attribute>
<html>
<head>
<title><xsl:value-of select='./@name' /> Namespaces</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head>
<body>
<p><a target="content" href='user_guides.htm'>User Guides</a></p>
<p><b>Namespaces</b></p>
<ul>
<xsl:for-each select='namespace'>
  <xsl:sort select='@name' />
  <li><a target="typesframe"><xsl:attribute name="href"><xsl:value-of select="@name"/>Types.htm</xsl:attribute><xsl:value-of select='@name' /></a></li>
</xsl:for-each>
</ul>
</body>
</html>
</file>

<!--
We write out all types for each namespace.
-->
<xsl:for-each select='namespace'>
  <xsl:variable name="namespace" select="@name" />
<file><xsl:attribute name='name'><xsl:value-of select='./@name' />Types.htm</xsl:attribute>
<html>
<head>
<title><xsl:value-of select='./@name' /> Types</title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head>
<body>
<b><xsl:value-of select='./@name' /> Types</b><br />

<xsl:if test='//assembly/type[@isClass="True" and @namespace=$namespace]'>
<h3>Classes</h3>
<p id='hintArea' class='hintAreaStyle'></p>
<p>
<xsl:for-each select='//assembly/type[@isClass="True" and @namespace=$namespace]'>
<xsl:sort select='@fullName' />
<a target="content">
<xsl:attribute name="href"><xsl:value-of select='myscripts:FullTypeToFileName(@fullName)'/>.htm</xsl:attribute>
<xsl:attribute name="onmouseover">hintArea.innerText='<xsl:apply-templates select='summary' />';</xsl:attribute>
<xsl:value-of select='@name' /></a><br />
</xsl:for-each>
</p>
</xsl:if>

</body>
</html>
</file>

</xsl:for-each>

<!-- Build the pages for the types -->
<xsl:for-each select='type'>
<file><xsl:attribute name='name'><xsl:value-of select='myscripts:FullTypeToFileName(@fullName)' />.htm</xsl:attribute>
<xsl:call-template name='TypePage' />
</file>
</xsl:for-each>

</multiple-files>

</xsl:template>

</xsl:stylesheet>
