package arabagent;

import java.util.*;

public class Wrapper {

	public Wrapper()
    {
		WikiData.load_wiki_files();
    }
	
    void get_term_concepts_list(String term, ArrayList<String> concepts_list)
    {
        try
        {
            //char[] sep = { ';' };

            String concepts_str = WikiData.get_concepts_for_a_term(term);
            if (!concepts_str.isEmpty())
            {
                String[] concepts = concepts_str.split(";");
                for(String concept : concepts)
                {
                    concepts_list.add(concept);
                }
            }
        }
        catch (Exception TIE)
        {
        	TIE.getMessage();
        }
    }
    
    private String get_term_concepts_list(String term)
    {
        return WikiData.get_concepts_for_a_term(term);
    }
    
    double get_inLinks_count(String concept)
    {
        return WikiData.get_inlinks_count(concept);
    }
    
    int retrieve_common_inlink_count(String c1, String c2)
    {
        //char[] separators = {';'};
        int counter = 0;
        
        //TreeSet<String> c1_inLinks = new TreeSet<String>(Arrays.asList((wd.what_links_here(c1).split(";"))));         
        ArrayList<String> c1_inLinks = new ArrayList<String>(Arrays.asList((WikiData.what_links_here(c1).split(";"))));
        String[] c2_inLinks = WikiData.what_links_here(c2).split(";");

        for (String str : c2_inLinks)
        {
            if (c1_inLinks.contains(str))
            {
                counter++;
            }
        }
        return counter;
    }
    
    void create_Feature_Vector(FeatureVector v1, FeatureVector v2, String c1_id_str, String c2_id_str)
    {
        //Feature_vector.Vector_Terms.RemoveRange(0, Feature_vector.Vector_Terms.Count);

        if (c1_id_str != null && c2_id_str != null)
        {
            //char[] separators = {';'};
            String[] c1_links = WikiData.get_concept_wikilinks(c1_id_str).split(";");
            String[] c2_links = WikiData.get_concept_wikilinks(c2_id_str).split(";");
            
            for(String target : c1_links)
            {            	
                v1.getVectorTerms().add(target.substring(0, target.indexOf(',')));
                v1.getVectorValues().add(Double.parseDouble(target.substring(target.indexOf(',')+1)));
                v2.getVectorValues().add(0.0);
            }

            for(String target : c2_links)
            {
                int term_index = v1.getVectorTerms().indexOf(target.substring(0, target.indexOf(',')));

                if (term_index < 0)
                {
                    v1.getVectorTerms().add(target.substring(0, target.indexOf(',')));
                    v1.getVectorValues().add(0.0);
                    v2.getVectorValues().add(Double.parseDouble(target.substring(target.indexOf(',') + 1)));
                }
                else {
                    v2.getVectorValues().add(term_index, Double.parseDouble(target.substring(target.indexOf(',') + 1)));
                }
            }       
        }

    }

    private double get_link_probability(String c_id)
    {
        return 0;
        //return Wiki_data_access.get_inlinks_count(c_id) 
        //    / Wiki_data_access.number_of_article_that_mentioned_the_phrase_at_all(c_id);
    }

}
