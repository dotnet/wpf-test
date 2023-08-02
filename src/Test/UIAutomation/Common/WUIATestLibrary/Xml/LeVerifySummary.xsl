<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

   <xsl:template match="/">
	<B><font color="#FF0000">Unexpected Errors:</font></B>
		<xsl:for-each select="TEST/SHORTSUMMARY/UnexpectedError">
			<![CDATA[    ]]><xsl:value-of select="@COUNT"/>
		</xsl:for-each>
	<BR></BR>
	<B><font color="#FF0000">Verificational Errors:</font></B>
		<xsl:for-each select="TEST/SHORTSUMMARY/VerificationError">
			<![CDATA[    ]]><xsl:value-of select="@COUNT"/>
		</xsl:for-each>

	<BR></BR>
   <B>Pass:</B>
		<xsl:for-each select="TEST/SHORTSUMMARY/PASS">
			<![CDATA[    ]]><xsl:value-of select="@COUNT"/>
		</xsl:for-each>
	<BR></BR>
	<B>Precondition Errors:</B>
		<xsl:for-each select="TEST/SHORTSUMMARY/CONFIGURATIONERROR">
			<![CDATA[    ]]><xsl:value-of select="@COUNT"/>
		</xsl:for-each>
		
	<BR></BR>
	<B>Informational:</B>
		<xsl:for-each select="TEST/SHORTSUMMARY/INFORMATIONAL">
			<![CDATA[    ]]><xsl:value-of select="@COUNT"/>
		</xsl:for-each>
		
</xsl:template>
</xsl:stylesheet>

