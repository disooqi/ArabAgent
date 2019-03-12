<%@ Page Language="C#" AutoEventWireup="true" CodeFile="search.aspx.cs" Inherits="search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Search Result</title>
    <link rel="stylesheet" type="text/css" href="feedback/google.css" />

    <script type="text/javascript" src="feedback/feedback_style.js"></script>
    <script type="text/javascript" src="feedback/xml_scripts.js"></script>

    <script language="javascript" type="text/javascript">
// <!CDATA[

function Submit1_onclick() 
{
    
}

// ]]>
    </script>

</head>
<body>
    <form id="tsf" method="GET" action="search.aspx">
        <table id="sft" class="ts" style="clear: both; margin: 19px 16px 20px 15px">
            <tr valign="top">
                <td style="width: 116px">
                    <h1>
                        <a id="logo" href="search.aspx?" title="Go to ArabAgent Home"><img
                             src="images/aa_201x40.png"
                            alt="" style="left: 4px; width: 96px; top: 2px; height: 34px" /></a>
                    </h1>
                </td>
                <td id="sff" style="padding: 1px 3px 7px; padding-left: 16px; width: 100%">
                    <table class="ts" style="margin: 12px 0 3px">
                        <tr>
                            <td nowrap="1">
                                <input type="hidden" name="hl" value="en" />
                                <input autocomplete="on" class="lst" type="text" name="q" size="41" maxlength="2048"
                                    value="ثورة يناير" title="Search" />
                                <input type="submit" name="btnG" class="lsb" style="margin: 0 2px 0 5px" value="Search" id="Submit1" onclick="return Submit1_onclick()" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
    <h2 class="hd">Search Results</h2>
    <div>
        <form id="fb_form" runat="server" method="post"></form>
    </div>
</body>
</html>
