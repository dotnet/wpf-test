<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:myscripts='urn:yada-yada'
  >

<!-- Build the table row for a type member. -->
<xsl:template name='MemberSummaryRow'>
<tr>
  <td><a><xsl:attribute name='href'>
    <xsl:value-of select='myscripts:GetLinkToMember(.)' />
  </xsl:attribute><xsl:value-of select='@name' /></a></td>
  <td><xsl:apply-templates select='summary' /><xsl:apply-templates select="declaringType" /></td>
</tr>
</xsl:template>

<xsl:template name="TypePage">
<html>
<head>
<title><xsl:value-of select='./@name' /></title>
<LINK REL="stylesheet" TYPE="text/css" HREF="docstyles.css" />
</head>
<body>
<h2>
  <xsl:value-of select='./@name' />
  <xsl:text> </xsl:text>
  <xsl:apply-templates select="." mode="TypeKindTemplate"/>
</h2>
<p><xsl:apply-templates select='summary' /></p>

<div class='hierarchyBlock'>
<xsl:call-template name='WriteHierarchy'>
  <xsl:with-param name='mostSpecificType' select='.' />
</xsl:call-template>
</div>

<p class='signature'><code>
<xsl:if test='@isPublic="True"'>public</xsl:if>
<xsl:if test='@isProtected="True"'>protected</xsl:if>
<xsl:text> </xsl:text>
<xsl:if test='@isClass="True"'>class</xsl:if>
<xsl:if test='@isInterface="True"'>interface</xsl:if>
<xsl:if test='@isValueType="True" and @isEnum="False"'>struct</xsl:if>
<xsl:if test='@isEnum="True"'>enum</xsl:if>
<xsl:if test='@isDelegate="True"'>delegate</xsl:if>
<xsl:if test='@isEvent="True"'>event</xsl:if>
<xsl:text> </xsl:text>
<xsl:value-of select='@name' /> <xsl:if test='baseType'> : 
<a><xsl:attribute name='href'>
  <xsl:if test='baseType[@isExternal="True"]'>http://msdn.microsoft.com</xsl:if>
  <xsl:if test='baseType[@isExternal="False"]'><xsl:value-of select='baseType/@fullName' /></xsl:if>
  </xsl:attribute><xsl:value-of select='baseType/@name' /></a>
<xsl:for-each select='interface'>
, <a><xsl:attribute name='href'>
    <xsl:if test='@isExternal="True"'>http://msdn.microsoft.com</xsl:if>
    <xsl:if test='@isExternal="False"'><xsl:value-of select='@fullName' />.htm</xsl:if>
    </xsl:attribute><xsl:value-of select='@name' /></a>
</xsl:for-each>
</xsl:if>
</code></p>

<xsl:if test='remarks'>
<div class='remarksTitle'>Remarks</div>
<xsl:apply-templates select='remarks' />
</xsl:if>

<xsl:if test='example'>
<div class='exampleTitle'>Example</div>
<xsl:apply-templates select='example' />
</xsl:if>

<!-- Build each type members page -->
<xsl:if test='member[@isConstructor="True" and @isPublic="True"]'>
<div class='typeMemberSectionTitle'>Public Constructors</div>
<table>
<xsl:for-each select='member[@isConstructor="True" and @isPublic="True"]'>
<tr>
  <td><a><xsl:attribute name='href'>#<xsl:value-of select='@tag'/></xsl:attribute><xsl:value-of select="../@name"/></a></td>
  <td><xsl:apply-templates select="summary" /></td>
</tr>
</xsl:for-each>
</table>
</xsl:if>

<xsl:if test='member[@isMethod="True" and @isSpecialName="False" and @isPublic="True"]'>
<div class='typeMemberSectionTitle'>Public Methods</div>
<table>
<xsl:for-each select='member[@isMethod="True" and @isSpecialName="False" and @isPublic="True"]'>
<xsl:sort select='@name' />
<xsl:call-template name='MemberSummaryRow' />
<!--
<tr>
  <td><a><xsl:attribute name='href'>
    <xsl:if test='declaringType[@isExternal="True"]'>ms-help://MS.NETFrameworkSDK/cpref/html/cpref_start.htm</xsl:if>
    <xsl:if test='declaringType[@isExternal!="True"]'>#<xsl:value-of select='@tag' /></xsl:if>
    </xsl:attribute><xsl:value-of select="@name" /></a></td> 
  <td><xsl:apply-templates select="summary" /><xsl:apply-templates select="declaringType" /></td>
</tr>
-->
</xsl:for-each>
</table>
</xsl:if>

<xsl:if test='member[@isProperty="True" and @isSpecialName="False" and @isPublic="True"]'>
<div class='typeMemberSectionTitle'>Public Properties</div>
<table>
<xsl:for-each select='member[@isProperty="True" and @isSpecialName="False" and @isPublic="True"]'>
<xsl:sort select='@name' />
<tr>
  <td><a><xsl:attribute name='href'><xsl:if test='declaringType[@isExternal="True"]'>ms-help://MS.NETFrameworkSDK/cpref/html/cpref_start.htm</xsl:if><xsl:if test='declaringType[@isExternal!="True"]'>#<xsl:value-of select='@tag' /></xsl:if></xsl:attribute><xsl:value-of select="@name" /></a></td>
  <td><xsl:apply-templates select="summary" /><xsl:apply-templates select="declaringType" /></td>
</tr>
</xsl:for-each>
</table>
</xsl:if>

<xsl:if test='member[@isMethod="True" and @isSpecialName="False" and @isProtected="True"]'>
<div class='typeMemberSectionTitle'>Protected Methods</div>
<table>
<xsl:for-each select='member[@isMethod="True" and @isSpecialName="False" and @isProtected="True"]'>
<xsl:sort select='@name' />
<tr>
  <td><a><xsl:attribute name='href'><xsl:if test='declaringType[@isExternal="True"]'>ms-help://MS.NETFrameworkSDK/cpref/html/cpref_start.htm</xsl:if><xsl:if test='declaringType[@isExternal!="True"]'>#<xsl:value-of select='@tag' /></xsl:if></xsl:attribute><xsl:value-of select="@name" /></a></td> 
  <td><xsl:apply-templates select="summary" /><xsl:apply-templates select="declaringType" /></td>
</tr>
</xsl:for-each>
</table>
</xsl:if>

<xsl:if test='member[@isProperty="True" and @isSpecialName="False" and @isProtected="True"]'>
<div class='typeMemberSectionTitle'>Protected Properties</div>
<table>
<xsl:for-each select='member[@isProperty="True" and @isSpecialName="False" and @isProtected="True"]'>
<xsl:sort select='@name' />
<tr>
  <td><a><xsl:attribute name='href'><xsl:if test='declaringType[@isExternal="True"]'>ms-help://MS.NETFrameworkSDK/cpref/html/cpref_start.htm</xsl:if><xsl:if test='declaringType[@isExternal!="True"]'>#<xsl:value-of select='@tag' /></xsl:if></xsl:attribute><xsl:value-of select="@name" /></a></td> 
  <td><xsl:apply-templates select="summary" /><xsl:apply-templates select="declaringType" /></td>
</tr>
</xsl:for-each>
</table>
</xsl:if>

<hr />

<!-- Build each type member details page -->
<xsl:for-each select='member[(@isSpecialName="False") and (@isPublic="True" or @isFamily="True") and (declaringType=../@fullName)]'>
<xsl:variable name='memberName'>
<xsl:if test='@isConstructor="True"'><xsl:value-of select="../@name" /></xsl:if>
<xsl:if test='@isConstructor != "True"'><xsl:value-of select="@name" /></xsl:if>
</xsl:variable>

<a><xsl:attribute name='name'><xsl:value-of select='@tag' /></xsl:attribute>

<div class='typeMemberTitle'><xsl:value-of select="../@name" />.<xsl:value-of select='$memberName' /></div></a>
<p class='signature'><code>
<xsl:if test='@isPublic="True"'>public</xsl:if>
<xsl:if test='@isProtected="True"'>protected</xsl:if>
<xsl:if test='@isFamily="True"'>protected</xsl:if>
<xsl:if test='@isPrivate="True"'>private</xsl:if>
<xsl:text> </xsl:text>
<xsl:if test='@isStatic="True"'>static</xsl:if>
<xsl:text> </xsl:text>
<a><xsl:attribute name='href'>
  <xsl:if test='@memtypeisExternal="True"'><xsl:value-of select='@memtypeexternalLink' /></xsl:if>
  <xsl:if test='@memtypeisExternal="False"'><xsl:value-of select='@memtypefullName' />.htm</xsl:if>
  </xsl:attribute>
<xsl:value-of select="@cspropertyType" />
<xsl:value-of select="@csfieldType" />
<xsl:value-of select='@csreturnType' /></a>
<xsl:text> </xsl:text>
<xsl:value-of select='$memberName' /><xsl:if test='@isMethod="True" or @isConstructor="True"'
>(<xsl:for-each select='param'>
  <xsl:sort select='@position' />
  <xsl:if test='@position &gt; 0'>, </xsl:if>
  <xsl:if test='@isOut="True" and @isIn="False"'>out </xsl:if>
  <xsl:if test='@isOut="True" and @isIn="True"'>ref </xsl:if>
  <a><xsl:attribute name='href'>
    <xsl:if test='@paramisExternal="True"'><xsl:value-of select='@paramexternalLink' /></xsl:if>
    <xsl:if test='@paramisExternal="False"'><xsl:value-of select='@paramfullName' />.htm</xsl:if>
    </xsl:attribute>
  <xsl:value-of select='@cstype' /></a>
  <xsl:text> </xsl:text>
  <xsl:value-of select='@name' />
</xsl:for-each
>)
</xsl:if><xsl:if test='@isProperty="True"'> { <xsl:if test='@canRead="True"'>get; </xsl:if>
<xsl:if test='@canWrite="True"'>set; </xsl:if
>}</xsl:if>
</code></p>

<div class='typeMemberBody'>

<xsl:apply-templates select='summary' />

<xsl:if test='param'>
<div class='parametersTitle'>Parameters</div>
<ul>
<xsl:for-each select='param'>
  <li><i><xsl:value-of select='@name' /></i>: <xsl:value-of select='.' /></li>
</xsl:for-each>
</ul>
</xsl:if>

<xsl:if test='@isMethod="True"'>
<div class='returnValueTitle'>Return Value</div>
<xsl:if test='@csreturnType!="void"'>
<xsl:apply-templates select='returns' />
</xsl:if>
<xsl:if test='@csreturnType="void"'>
<p>This member does not return a value.</p>
</xsl:if>
</xsl:if>

<xsl:if test='remarks'>
<div class='remarksTitle'>Remarks</div>
<xsl:apply-templates select='remarks' />
</xsl:if>

<xsl:if test='example'>
<div class='exampleTitle'>Example</div>
<xsl:apply-templates select='example' />
</xsl:if>

</div>

</xsl:for-each> <!-- Build each type member details page stops here -->

</body>
</html>

</xsl:template>

<xsl:template match="type" mode="TypeKindTemplate">
<xsl:if test='@isClass="True"'>Class</xsl:if>
<xsl:if test='@isInterface="True"'>Interface</xsl:if>
<xsl:if test='@isValueType="True" and @isEnum="False"'>Value Type</xsl:if>
<xsl:if test='@isEnum="True"'>Enumeration</xsl:if>
</xsl:template>

<!-- Writes an indented type name for hierarchies spaces -->
<xsl:template name='WriteBaseTypeSpaces'>
  <xsl:param name='baseTypesAbove' />
  <xsl:param name='typeNode' />
  <xsl:param name='isAtBottom' />

  <xsl:if test='$typeNode/baseType'>
  <xsl:call-template name='WriteBaseTypeSpaces'>
    <xsl:with-param name='baseTypesAbove' select='$baseTypesAbove - 1' />
    <xsl:with-param name='typeNode' select='$typeNode/baseType' />
    <xsl:with-param name='isAtBottom' select='"False"' />
  </xsl:call-template>
  </xsl:if>
  
  <xsl:value-of select='myscripts:RepeatSpaces($baseTypesAbove*2)' />

  <xsl:if test='$isAtBottom="False"'>
  <a><xsl:attribute name='href'>
    <xsl:if test='$typeNode/@isExternal="True"'><xsl:value-of select='$typeNode/@externalLink' /></xsl:if>
    <xsl:if test='$typeNode/@isExternal="False"'><xsl:value-of select='$typeNode/@fullName' />.htm</xsl:if>
    </xsl:attribute>
  <xsl:value-of select='$typeNode/@fullName' /></a><br />
  </xsl:if>

  <xsl:if test='$isAtBottom="True"'>
  <b><xsl:value-of select='$typeNode/@fullName' /></b>
  </xsl:if>
</xsl:template>

<!-- Matches the first baseType in a hierarchy -->
<xsl:template name='WriteHierarchy'>
  <xsl:param name='mostSpecificType' />
  <xsl:call-template name='WriteBaseTypeSpaces'>
    <xsl:with-param name='baseTypesAbove' select='count($mostSpecificType//baseType)' />
    <xsl:with-param name='typeNode' select='$mostSpecificType' />
    <xsl:with-param name='isAtBottom' select='"True"' />
  </xsl:call-template>
</xsl:template>

</xsl:stylesheet>
