package arabagent;

import java.util.*;
import java.io.*;
import java.net.URL;


public class WikiData {
	//static String files_path = "C:/Documents and Settings/disooqi/workspace/ArabAgent/war/wikiData/";

    /*
     * 0 = unloaded
     * 1 = loading
     * 2 = loaded
     * */
    static int is_loaded = 0;
    
    //classLoader.
    //ClassLoader().;//.getResource("_cc_inconcepts_count.txt");
    //static URL inlink_counts_path = ClassLoader().getResource("_cc_inconcepts_count.txt");
    //static URL term_concepts_path = WikiData.class.getClassLoader().getResource("term_concepts_list_index.txt");

    static String inlink_counts_path = "_cc_inconcepts_count.txt";
    static String concept_outconcepts_list_index = "_concept_outconcepts_list_index.txt";
    static String concept_inconcepts_list_index = "_concept_inconcepts_list_index.txt";
    static String term_concepts_path = "term_concepts_list_index.txt";
    
    static Map<String, String> concept_outconcepts_index = new HashMap<String, String>();
    static Map<String, String> concept_inconcepts_index = new HashMap<String, String>();
    static Map<String, Double> inlink_counts_cache = new HashMap<String, Double>();
    static Map<String, String> term_concepts_dic = new HashMap<String, String>();
    
    public static void load_wiki_files()
    {
        switch (is_loaded)
        {
            case 0:
                is_loaded = 1;
                break;
            case 1:
                while (is_loaded == 1) ;
                return;
            case 2: return;
        }
        
        try
        {
        	InputStream is = WikiData.class.getResourceAsStream(inlink_counts_path);
        	//InputStream is = WikiData.class.getClassLoader().getResourceAsStream(inlink_counts_path);        	        	
        	BufferedReader br = new BufferedReader(new InputStreamReader(is, "UTF8"));

            String word = "";
            while ((word = br.readLine()) != null)            
              inlink_counts_cache.put(word.substring(0, word.indexOf(',')), Double.parseDouble(word.substring(word.indexOf(',') + 1)));
            is.close();
            br.close();
        	/*  
            //to create the feature vector of the category (41M)
            fis = new FileInputStream(concept_outconcepts_list_index);        	        	
        	br = new BufferedReader(new InputStreamReader(fis, "UTF8"));

            word = "";
            while ((word = br.readLine()) != null)                
                concept_outconcepts_index.put(word.substring(0, word.indexOf(':')), word.substring(word.indexOf(':') + 1));
            fis.close();  
            br.close();
                            
            //for google similarity distance (33M)
            fis = new FileInputStream(concept_inconcepts_list_index);        	        	
            br = new BufferedReader(new InputStreamReader(fis, "UTF8"));

            word = "";          
            while ((word = br.readLine()) != null)            
                concept_inconcepts_index.put(word.substring(0, word.indexOf(':')), word.substring(word.indexOf(':') + 1));
            fis.close(); 
            br.close();*/ 
           
            //15M
            is = WikiData.class.getResourceAsStream(term_concepts_path);        	        	
            br = new BufferedReader(new InputStreamReader(is, "UTF8"));

            word = "";
            while ((word = br.readLine()) != null)     
                term_concepts_dic.put(word.substring(0, word.indexOf(':')), word.substring(word.indexOf(':') + 1));
            is.close(); 
            br.close();
         
            is_loaded = 2;
        }
        catch (FileNotFoundException e) { e.getMessage(); }
        catch (UnsupportedEncodingException e){e.getMessage();}
        catch (IOException e){e.getMessage();}
    }

    public static String get_concept_wikilinks(String concept_id)
    {
        return concept_outconcepts_index.get(concept_id);
    }

    public static String what_links_here(String concept_id)
    {
        return concept_inconcepts_index.get(concept_id);
    }
    
    public static String get_concepts_for_a_term(String term)
    {        
        if(term_concepts_dic.containsKey(term))
        	return term_concepts_dic.get(term);
		return "";
    }

    public static double get_inlinks_count(String c_id)
    {
        return inlink_counts_cache.get(c_id);
    }


}
