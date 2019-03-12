using System;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //category_detector cd_obj = new category_detector();
        
        //SortedList<double, string> categories = new SortedList<double, string>();
        //cd_obj.detect_category(get_html_text_of_a_webpage("http://www.disooqi.com/document.html"), out categories);
        ////Response.Write("«·Õ„œ ··Â");
        ////Response.Write("Al7amdo Lelah");
        //thanking.Text = "Al7amdo Lelah";

        ////String rootPath = Server.MapPath("~");

        //thanking.Text = categories.Count.ToString();
        //foreach (KeyValuePair<double, string> cat in categories)
        //{
        //    Response.Write(cat.Value + " , " + cat.Key + "<br />");
        //}
    }
/*
    string get_html_text_of_a_webpage(string url)
    {
        //http://www.akhbarelyom.org.eg/
        //<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
        try
        {
            WebClient client = new WebClient();
            string html = client.DownloadString(url);
            string charSet = get_page_charset_value(html);

            if (Encoding.GetEncoding(charSet) != client.Encoding)
            {
                //Encoding page_encoding = Encoding.GetEncoding(charSet);

                if (string.IsNullOrEmpty(charSet))
                    client.Encoding = Encoding.UTF8;
                else
                    client.Encoding = Encoding.GetEncoding(charSet);

                html = client.DownloadString(url);
            }

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            //string html = (new StreamReader(resp.GetResponseStream(),target_encoding)).ReadToEnd();

            return html;
        }
        catch (Exception exc)
        {
            return "";
            throw new Exception(exc.Message, exc);
        }
    }

    string get_page_charset_value(string html)
    {
        MatchCollection mc = Regex.Matches(html, @"<meta.*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        for (int i = 0; i < mc.Count; i++)
        {
            if (mc[i].Value.Contains("charset="))
            {
                string str = mc[i].Value;

                return str.Substring(str.IndexOf("charset=") + 8, str.LastIndexOf("\"") - str.IndexOf("charset=") - 8);
            }
        }
        return null;
    }
    */

    public bool add_text_to_xml_file(string txt, string XML_path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(XML_path);

        XmlElement root = xmlDoc.DocumentElement;
        XmlElement interests_elem = (XmlElement)root.GetElementsByTagName("interests")[0];
        interests_elem.InnerText += txt;
        xmlDoc.Save(XML_path);
        return true;
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        string txt = "<category cat_id=\"5989\" cat_title=\"»–—… Ã€—«›Ì« „’—\" w=\"\" b=\"\" page_id=\"120930\" />";
        add_text_to_xml_file(txt, Server.MapPath(@"App_Data\profile.xml"));
    }
}
