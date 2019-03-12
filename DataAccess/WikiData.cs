using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace DataAccess
{
    public static partial class WikiData
    {
        static string files_path = @"D:\Dropbox\_workspace\work\KSA research group\ArabAgent\data\";
        static string is_loaded = "unloaded";

        //static string inlink_counts_path = "_cc_inlink_count(sorted).sql";
        //static string concept_outconcepts_list_index = "_concept_outconcepts_list_index(sorted).sql";
        //static string concept_inconcepts_list_index = "_concept_inconcepts_list_index(sorted).sql";
        //static string term_concepts_path = "_term_concepts(norm_no_stopwords).sql";

        static string inlink_counts_path = "_cc_inconcepts_count.sql";
        static string concept_outconcepts_list_index = "_concept_outconcepts_list_index.sql";
        static string concept_inconcepts_list_index = "_concept_inconcepts_list_index.sql";
        static string term_concepts_path = "term_concepts_list_index.sql";


        static Dictionary<string, string> concept_outconcepts_index = new Dictionary<string, string>();
        static Dictionary<string, string> concept_inconcepts_index = new Dictionary<string, string>();
        static Dictionary<string, double> inlink_counts_cache = new Dictionary<string, double>();
        static Dictionary<string, string> term_concepts_dic = new Dictionary<string, string>();

        //public static string Files_path
        //{
        //    set { files_path = value; }
        //    get { return files_path; }
        //}

        static WikiData() {  }

        public static void load_wiki_files()
        {
            switch (is_loaded)
            {
                case "unloaded":
                    is_loaded = "loading";
                    break;
                case "loading":
                    while (is_loaded == "loading") ;
                    return;
                case "loaded": return;
            }

            try
            {
                using (StreamReader sr = new StreamReader(files_path+inlink_counts_path, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        inlink_counts_cache.Add(word.Substring(0, word.IndexOf(',')), double.Parse(word.Substring(word.IndexOf(',') + 1)));
                    }
                }
                //to create the veture vector of the category (41M)
                using (StreamReader sr = new StreamReader(files_path + concept_outconcepts_list_index, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        concept_outconcepts_index.Add(word.Substring(0, word.IndexOf(':')), word.Substring(word.IndexOf(':') + 1));
                    }
                }
                //for google similarity distance (33M)
                using (StreamReader sr = new StreamReader(files_path + concept_inconcepts_list_index, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        concept_inconcepts_index.Add(word.Substring(0, word.IndexOf(':')), word.Substring(word.IndexOf(':') + 1));
                    }
                }

                //15M
                using (StreamReader sr = new StreamReader(files_path + term_concepts_path, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        term_concepts_dic.Add(word.Substring(0, word.IndexOf(':')), word.Substring(word.IndexOf(':') + 1));
                    }
                }

                is_loaded = "loaded";
            }
            catch (Exception load_exp)
            {
                throw new Exception("Ya Disooqi: " + load_exp.Message, load_exp);
            }
        }
        
        public static string get_concept_wikilinks(string concept_id)
        {
            return concept_outconcepts_index[concept_id];
        }

        public static string what_links_here(string concept_id)
        {
            return concept_inconcepts_index[concept_id];
        }
        
        public static string get_concepts_for_a_term(string term)
        {
            string temp = string.Empty;
            term_concepts_dic.TryGetValue(term, out temp);
            return temp;
        }

        public static double get_inlinks_count(string c_id)
        {
            return inlink_counts_cache[c_id];
        }

        //public static int number_of_article_that_mentioned_the_phrase_at_all(string c_id)
        //{
        //    return context_term_appearance[c_id];
        //}

        //public static void reload_term_concepts_index()
        //{
        //    using (StreamReader sr = new StreamReader(term_concepts_path, Encoding.UTF8))
        //    {
        //        string word = string.Empty;
        //        while ((word = sr.ReadLine()) != null)
        //        {
        //            term_concepts_dic.Add(word.Substring(0, word.IndexOf(':')), word.Substring(word.IndexOf(':') + 1));
        //        }
        //    }
        //}
    }
}
