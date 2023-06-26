<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:myscripts='urn:yada-yada'
  >
  
<xsl:template match='text()' mode='CopyTextMode'>
<xsl:value-of select='normalize-space(.)' />
</xsl:template>

<xsl:template match='text()'>
<xsl:value-of select='normalize-space(.)' />
</xsl:template>

<xsl:template match='declaringType'>
<xsl:if test='@isExternal="True"'>(declared in <b><xsl:value-of select='.' /></b>)</xsl:if>
<xsl:if test='(.!=../../@fullName) and @isExternal="False"'>(declared in <b><xsl:value-of select='.' /></b>)</xsl:if>
</xsl:template>

<xsl:template match='remarks'>
<xsl:apply-templates select='node() | text()' />
</xsl:template>

<xsl:template match='summary'>
<xsl:apply-templates select='text()' mode='CopyTextMode' />
</xsl:template>

<xsl:template match='code'>
<pre class='preCode'><xsl:value-of select='text()' /></pre>
</xsl:template>

<xsl:template match='c'>
<code><xsl:apply-templates select='node() | text()' /></code>
</xsl:template>

<xsl:template match='p'>
<p><xsl:apply-templates select='node() | text()' /></p>
</xsl:template>

</xsl:stylesheet>
