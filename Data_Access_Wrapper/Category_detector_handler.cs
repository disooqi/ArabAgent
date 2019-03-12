using System;
using System.Collections.Generic;
using System.Text;
using Vector_Space;
using DataAccess;

namespace Data_Access_Wrapper
{
    public partial class Wrapper
    {
        public Wrapper()
        {
            WikiData.load_wiki_files();
        }

        public void get_term_concepts_list(string term,ref List<string> concepts_list)
        {
            try
            {
                char[] sep = { ';' };

                string concepts_str = WikiData.get_concepts_for_a_term(term);
                if (concepts_str != null)
                {
                    string[] concepts = concepts_str.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string concept in concepts)
                    {
                        concepts_list.Add(concept);
                    }
                }
            }
            catch (Exception TIE)
            {
                //System.Windows.Forms.MessageBox.Show(TIE.Message);
            }
        }

        public string get_term_concepts_list(string term)
        {
            return WikiData.get_concepts_for_a_term(term);
        }

        public bool isLinkExist(string source, string target)
        {
            //return wfaObj.article_wikilinks.ContainsKey(source + "," + target);
            return true;
        }

        public double get_inLinks_count(string concept)
        {
            return WikiData.get_inlinks_count(concept);
        }
        
        public int retrieve_common_inlink_count(string c1, string c2)
        {
            char[] separators = {';'};
            int counter = 0;
            List<string> c1_inLinks = new List<string>(WikiData.what_links_here(c1).Split(separators, StringSplitOptions.RemoveEmptyEntries));
            string[] c2_inLinks = WikiData.what_links_here(c2).Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string str in c2_inLinks)
            {
                if (c1_inLinks.Contains(str))
                {
                    counter++;
                }
            }
            return counter;
        }

        public void create_Feature_Vector(ref Feature_vector v1, ref Feature_vector v2, string c1_id_str, string c2_id_str)
        {
            //Feature_vector.Vector_Terms.RemoveRange(0, Feature_vector.Vector_Terms.Count);

            if (c1_id_str != null && c2_id_str != null)
            {
                char[] separators = {';'};
                string[] c1_links = WikiData.get_concept_wikilinks(c1_id_str).Split(separators, StringSplitOptions.RemoveEmptyEntries);
                string[] c2_links = WikiData.get_concept_wikilinks(c2_id_str).Split(separators, StringSplitOptions.RemoveEmptyEntries);
                
                foreach(string target in c1_links)
                {
                    v1.Vector_Terms.Add(target.Substring(0, target.IndexOf(',')));
                    v1.Vector_Values.Add(double.Parse(target.Substring(target.IndexOf(',')+1)));
                    v2.Vector_Values.Add(0);
                }

                foreach (string target in c2_links)
                {
                    int term_index = v1.Vector_Terms.IndexOf(target.Substring(0, target.IndexOf(',')));

                    if (term_index < 0)
                    {
                        v1.Vector_Terms.Add(target.Substring(0, target.IndexOf(',')));
                        v1.Vector_Values.Add(0);
                        v2.Vector_Values.Add(double.Parse(target.Substring(target.IndexOf(',') + 1)));
                    }
                    else {
                        v2.Vector_Values[term_index] = double.Parse(target.Substring(target.IndexOf(',') + 1));
                    }
                }                  
            }

        }
        
        public double get_link_probability(string c_id)
        {
            return 0;
            //return Wiki_data_access.get_inlinks_count(c_id) 
            //    / Wiki_data_access.number_of_article_that_mentioned_the_phrase_at_all(c_id);
        }
    }
}
