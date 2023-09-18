<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" xmlns="http://www.w3.org/1999/xhtml">

<xsl:output method="xml" indent="yes"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" 
    doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" />


    <xsl:template match="/">

    <html lang="en" xml:lang="en">
     <head>
      <title>My Title</title>
     </head>
     <body>
       <table border="0">
        <tr><th>Path</th><th>Application</th><th>Type</th><th>Extension</th><th>Size</th></tr>

        <xsl:for-each select="ScanFiles/File">
        <tr>
        <td><xsl:value-of xmlns="" select="Path" /></td>
        <td><xsl:value-of xmlns="" select="CommonProperties/ApplicationName" /></td>
        <td><xsl:value-of xmlns="" select="CommonProperties/FileType" /></td>
        <td><xsl:value-of xmlns="" select="CommonProperties/FileExtension" /></td>
        <td><xsl:value-of xmlns="" select="CommonProperties/Size" /></td>
        </tr>
        </xsl:for-each>

       </table>
     </body>     
    </html>
     
   </xsl:template>

</xsl:stylesheet>