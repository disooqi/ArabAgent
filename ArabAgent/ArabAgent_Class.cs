using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using nsPhrase_Detection;
using dosDataStructure;

namespace ArabAgent
{
    public class ArabAgent_Class
    {
        //int agentId;
        string inBehalfOf; //username
        UserModel AgentUserModel;
        PhrasesDetector PD_Obj;

        public ArabAgent_Class(string userName)
        {
            inBehalfOf = userName;
            AgentUserModel = new UserModel(userName);
            PD_Obj = new PhrasesDetector();
        }

        public ArabAgent_Class(FileInfo profileFile)
        {
            AgentUserModel = new UserModel(profileFile);
            PD_Obj = new PhrasesDetector();
        }


        public void updateAgent(String article)
        {
            //detect the concepts and their occurrence frequency
            Dictionary<int, n_gram> pos_struct_dic = new Dictionary<int, n_gram>();
            PD_Obj.wikify(article, ref pos_struct_dic);

            //send a list of concepts with their occurrence frequencies to User Model to update the profile
            AgentUserModel.updateUserModel(ref pos_struct_dic); 
        }

        public bool updateAgent(Uri url, bool relevancy_status)
        {
            if (relevancy_status)
            {
                //retrieveWebSite
                string WebPageKeywords = getHTMLofWebpage(url);
                //buid the semantic Graph («‰  „‘ „Õ «ÃÂ)

                //detect the concepts and their occurrence frequency
                Dictionary<int, n_gram> pos_struct_dic = new Dictionary<int, n_gram>();
                PD_Obj.wikify(WebPageKeywords, ref pos_struct_dic);
                                
                //send a list of concepts with their occurrence frequencies to User Model to update the profile
                AgentUserModel.updateUserModel(ref pos_struct_dic);               
            }
            return true;
        }

        //public bool updateAgent(ref Dictionary<int, n_gram> pos_struct_dic)
        //{
        //    if (relevancy_status)
        //    {
        //        //detect the concepts and their occurrence frequency
        //        Dictionary<int, n_gram> pos_struct_dic = new Dictionary<int, n_gram>();
        //        PhrasesDetector PD_Obj = new PhrasesDetector();
        //        PD_Obj.wikify(documentTxt, ref pos_struct_dic);

        //        //send a list of concepts with their occurrence frequencies to User Model to update the profile
        //        AgentUserModel.updateUserModel(ref pos_struct_dic);
        //    }
        //    return true;
        //}

        
        public bool updateAgent(string QueryStr, string url)
        { return true; }

        public double calculateInterestingValue(Uri url)
        {
            return 0;
        }

        public double calculateInterestingValue(string DocText)
        {
            Dictionary<int, n_gram> pos_struct_dic = new Dictionary<int, n_gram>();
            PD_Obj.wikify(DocText, ref pos_struct_dic);
            
            return AgentUserModel.calculateSimilarityToProfile(ref pos_struct_dic);
        }

        private string getHTMLofWebpage(Uri uri)
        {
            string url = uri.AbsolutePath;
            //http://www.akhbarelyom.org.eg/
            //<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
            try
            {
                WebClient client = new WebClient();

                //this method always download pages encoded in windows-1256
                string html = client.DownloadString(url);
                string charSet = get_page_charset_value(html);

                //the page iencoding is always utf-8 of no encoding is specified
                if (string.IsNullOrEmpty(charSet))
                {
                    client.Encoding = Encoding.UTF8;
                    html = client.DownloadString(url);
                }
                else if (Encoding.GetEncoding(charSet) != client.Encoding)
                {
                    client.Encoding = Encoding.GetEncoding(charSet);
                    html = client.DownloadString(url);
                }


                //Encoding page_encoding = Encoding.GetEncoding(charSet);
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
                //string html = (new StreamReader(resp.GetResponseStream(),target_encoding)).ReadToEnd();

                return html;
            }
            catch (Exception exc)
            {

                return "";
            }
        }
        
        private string get_page_charset_value(string html)
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

    }
}
