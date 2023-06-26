<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:myscripts='urn:yada-yada'>

<msxsl:script language="C#" implements-prefix='myscripts'>
<![CDATA[
public string RepeatSpaces(int count)
{
    return new System.String(' ', count);
}

public string FullTypeToFileName(string typeName)
{
    return typeName.Replace("+", "_");
}

public string GetTypeKind(XPathNavigator typeNode)
{
    if (typeNode.GetAttribute("isInterface", "") == "True") return "interface";
    if (typeNode.GetAttribute("isEnum", "") == "True") return "enum";
    if (typeNode.GetAttribute("isValueType", "") == "True") return "struct"; // needs to come after enums
    if (typeNode.GetAttribute("isDelegate", "") == "True") return "delegate";
    if (typeNode.GetAttribute("isClass", "") == "True") return "class";
    if (typeNode.GetAttribute("isEvent", "") == "True") return "event";
    return "[unknown]";
}

public string GetLinkToMember(XPathNodeIterator memberNodeIterator)
{
    if (!memberNodeIterator.MoveNext())
        return "";

    XPathNavigator memberNode = memberNodeIterator.Current;
    const string namespaceUri = "";

    if (memberNode == null)
    {
        throw new ArgumentNullException("memberNode");
    }
    if (memberNode.LocalName != "member")
    {
        throw new ArgumentException(
            "GetLinkToMember must be called with a member node, not a " +
            memberNode.Name + " node.", "memberNode");
    }
    
    // Get the tag for the current member.
    string tag = memberNode.GetAttribute("tag", namespaceUri);
    if (tag == "")
    {
        throw new Exception("member has not tag attribute.");
    }
    
    // Get the declaring type name.
    string declaringTypeName;
    XPathNodeIterator iter = memberNode.SelectChildren("declaringType", namespaceUri);
    if (!iter.MoveNext())
    {
        throw new Exception("There are no declaringType nodes under member.");
    }
    string isExternalValue = iter.Current.GetAttribute("isExternal", namespaceUri);
    if (isExternalValue == "True")
    {
        return "http://wcpsdk/";
    }
    else
    {
        declaringTypeName = iter.Current.Value;
    }
    
    //
    // Get the parent to determine whether it's local or not, and return
    // a link to the current page or to some other page.
    //
    memberNode.MoveToParent();
    if (memberNode.LocalName != "type")
    {
        throw new Exception("member elemetns should be children of type " +
          "tags, not " + memberNode.LocalName);
    }
    string currentTypeName = memberNode.GetAttribute("fullName", namespaceUri);
    if (currentTypeName == declaringTypeName)
    {
        return "#" + tag;
    }
    else
    {
        return GetLinkToTypeMember(declaringTypeName, tag);
    }
}

private string GetLinkToTypeMember(string declaringTypeName, string memberTag)
{
    if (declaringTypeName == null)
    {
        throw new ArgumentNullException("declaringTypeName");
    }
    if (memberTag == null)
    {
        throw new ArgumentNullException("memberTag");
    }
    return FullTypeToFileName(declaringTypeName) + ".htm#" + memberTag;
}
]]>
</msxsl:script>

</xsl:stylesheet>
