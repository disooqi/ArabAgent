using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Text;
using ArabAgent;

public partial class search : System.Web.UI.Page
{
    ArabAgent_Class personalAgent;

    protected void Page_Load(object sender, EventArgs e)
    {
        //Response.Write("Al7amdo lelah");

        if (Request.QueryString["q"] != null)
        {
            //customize the search part
            fb_form.InnerHtml += generate_result(Request.QueryString["q"]);
        }

        //The profile update part
        if (Request.QueryString["fb_rel"] != null)
        {
            personalAgent = new ArabAgent_Class("dos");

            if (Request.QueryString["fb_rel"] == "rel")
                personalAgent.updateAgent(Request.QueryString["fb_url"], true);
            else
                personalAgent.updateAgent(Request.QueryString["fb_url"], false);
        }
    }

    string generate_result(string original_keywords)
    {
        WebClient client = new WebClient();
        client.Encoding = Encoding.UTF8;
        string search_keywords = generate_customized_keywords(Request.QueryString["q"]);
        string html = client.DownloadString("http://www.google.com/search?q=" + search_keywords);

        // If result is not empty, Add feedback controls to the web page 
        if (html.Contains("<div id=ires>"))
            return add_feedback_controls_to_result(html);
        else return "<h2>No Result</h2>";
    }

    string add_feedback_controls_to_result(string html)
    {
        html = get_innerText("<div id=ires>", html);
        html = html.Replace("=\"/", "=\"http://www.google.com.eg/");
        string[] sep ={ "<li" };
        List<string> results = new List<string>(html.Split(sep, StringSplitOptions.RemoveEmptyEntries));
        html = results[0];
        for (int i = 1; i < results.Count; i++)
            if (i < results.Count - 1)
                html += addFeedbackScriptToSingleHit(results[i], i - 1) + "</li>";
            else html += addFeedbackScriptToSingleHit(results[i], i - 1) + "</li>";
        return html;
    }

    string get_innerText(string start_tag, string html)
    {
        html = html.Substring(html.IndexOf(start_tag));

        int pointer = 0;
        Stack<string> div_stack = new Stack<string>();
        while (pointer < html.Length)
        {
            if (html.Substring(pointer, 6) == "</div>")
            {
                if (div_stack.Count > 1)
                    div_stack.Pop();
                else return html.Substring(0, pointer + 6);
            }
            else if (html.Substring(pointer, 4) == "<div")
            {
                div_stack.Push("<div>");
            }
            pointer++;
        }
        return "";
    }

    string addFeedbackScriptToSingleHit(string result, int HitorderOfInPage)
    {
        //static int counter = 0;
        string feedback_control = "<span id=\"fb_ctl" + HitorderOfInPage + "\" style=\"color:red; font-weight:bold; visibility:visible;\">"
    + "<input id=\"rel" + HitorderOfInPage + "\" type=\"radio\" name=\"relevancy" + HitorderOfInPage + "\" onclick=\"send_feedback('" + HitorderOfInPage + "','rel');\" value=\"\"><label for=\"rel" + HitorderOfInPage + "\"> Relevant </label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
    + "<input id=\"irr" + HitorderOfInPage + "\" type=\"radio\" name=\"relevancy" + HitorderOfInPage + "\" onclick=\"send_feedback('" + HitorderOfInPage + "','irr');\" value=\"\"><label for=\"irr" + HitorderOfInPage + "\"> Irrelevant </label>"
    + "</span>";
        //+ "<span id=\"thank" + counter + "\" style=\"color:#43845a; font-weight:bold; visibility:hidden\">"
        //+ "<img src=\"http://mail.google.com/mail/help/images/check.gif\">Thank you for your feedback.</span>";


        result = result.Insert(result.IndexOf("</h3>") + 5, "&nbsp;&nbsp;&nbsp;" + feedback_control);
        result = "<li id=\"rs" + HitorderOfInPage + "\"" + result;
        return result;
    }

    string generate_customized_keywords(string q)
    {
        //q is the value of the q parameter in the search url of google or any search engine
        //this function should be in the customization component (dll)
        return q;
    }

    //string add_feedback_button_1()
    //{
    //    return "\n<div><input type=\"submit\" id=\"fb_button1\" value=\"Send Feedback\" onClick=\"Send_Feedback()\" class=\"lsb\" style=\"font-weight: bold; color: red; margin: 0 2px 0 5px\"></div>\n";
    //}

    //string add_feedback_button_2()
    //{
    //    return "\n<div><input type=\"submit\" id=\"fb_button2\" value=\"Send Feedback\" onClick=\"Send_Feedback()\" class=\"lsb\" style=\"font-weight: bold; color: red; margin: 0 2px 0 5px\"></div>\n";
    //}
}
