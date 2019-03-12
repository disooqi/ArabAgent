package arabagent;

import java.util.*;

public class TermDisambiguation {

    Wrapper cDhObject;
    SemanticRelatedness SRCObj;
    //double context_quality;
    /*
     * most_common_sense            0
     * most_closely_related_pair    1
     */
    static int disambiguation_technique = 0; //default
    static double sensible_probability_threshold = 0.02; //default


    //LinkedHashMap<String, ArrayList<Double>> context_sense_relatedness_table;
    
    double[][] context_sense_relatedness_table;
    HashMap<String, Integer> concept_index_dic;

    public TermDisambiguation()
    {
        cDhObject = new Wrapper();
        SRCObj = new SemanticRelatedness();
        //context_sense_relatedness_table = new LinkedHashMap<String, ArrayList<Double>>();
        concept_index_dic = new HashMap<String, Integer>();
    }

    public void setDisambiguationTechnique(int disambiguationTechniqueCode)
    {disambiguation_technique = disambiguationTechniqueCode; }    
    public int getDisambiguationTechnique()
    {return disambiguation_technique;}
    
    public void setSensibleProbabilityThreshold(double sensibleProbabilityThreshold)
    {        
        sensible_probability_threshold = sensibleProbabilityThreshold;
    }
    public double getSensibleProbabilityThreshold()
    {
    	return sensible_probability_threshold;
    }
    
    public void disambiguate_concepts(String term1, String term2,  String c1,  String c2,int disambiguator_id)
    {
        if(disambiguator_id == 0)
            most_common_sense(term1, term2,  c1,  c2);
        else if (disambiguator_id == 1)
            most_closely_related_pair(term1, term2,  c1,  c2);
        //else if(disambiguator_id == 2)
        //    commonness_and_relatedness(term1, term2, out c1, out c2);
        //else if (disambiguator_id == 3)
        //    sequential_decision(term1, term2, out c1, out c2);
        else { c1 = "wrong disambiguator Index"; c2 = "wrong disambiguator Index"; }
    }

    public void disambiguate_concepts(LinkedHashMap<Integer, nGram> pos_ngram_dic, int disambiguator_id)
    {
        if (disambiguator_id == 0)
            most_common_sense(pos_ngram_dic);

        else if (disambiguator_id == 1)
            most_closely_related_pairs( pos_ngram_dic);
    }
    /*
     * PREV STEP: The Previous step before this is identifing the candidate articles (concepts)
    1- THIS STEP:  is to identify the concepts they relate to: (in Wikipedia’s case, the articles which discuss them.)
     * NEXT STEP: measuring the relatedness between two concepts
    */
    //there are several options:
    //**************************
    //1. one can make a snap decision by using the most common sense of each term.
    //    (HINT: The commonness of a sense is defined by the number of times the term is used to link to it)
    void most_common_sense(String term1, String term2, String c1, String c2)
    {
        ArrayList<String> concepts_list1 = new ArrayList<String>();
        ArrayList<String> concepts_list2 = new ArrayList<String>();

        cDhObject.get_term_concepts_list(term1,  concepts_list1);
        cDhObject.get_term_concepts_list(term2,  concepts_list2);

        double max_for_term1=0, max_for_term2=0;
        c1 = "";
        c2 = "";
        for(String concept : concepts_list1)
        {
            if (max_for_term1 < cDhObject.get_inLinks_count(concept))
            {
                c1 = concept;
                max_for_term1 = cDhObject.get_inLinks_count(concept);
            }
        }

        for(String concept : concepts_list2)
        {
            if (max_for_term2 < cDhObject.get_inLinks_count(concept))
            {
                c2 = concept;
                max_for_term2 = cDhObject.get_inLinks_count(concept);
            }
        }
    }
    void most_common_sense(LinkedHashMap<Integer, nGram> pos_ngram_dic)
    {
    	LinkedHashMap<Integer, nGram> temp_dic  = new LinkedHashMap<Integer, nGram>();
    	


        for (Map.Entry<Integer, nGram> kvp : pos_ngram_dic.entrySet())
        {
        	nGram temp_ngram = kvp.getValue();

            if (kvp.getValue().concepts != null)
            {
                double max_for_term = 0;
                double inlink_count = 0;
                for (String concept : temp_ngram.concepts)
                {
                    inlink_count = cDhObject.get_inLinks_count(concept);
                    if (max_for_term < inlink_count)
                    {
                        temp_ngram.concept = concept;
                        max_for_term = inlink_count;
                    }
                }
                if (temp_ngram.concept != null)
                    temp_dic.put(kvp.getKey(), temp_ngram);
                else { }
            }
            else temp_dic.put(kvp.getKey(), temp_ngram);
        }

        pos_ngram_dic = temp_dic;
    }
    //2. Another approach is to use the two terms involved to disambiguate each other (selecting most closely related pair)
    //    (HINT: but is marred by the number of obscure senses available: there may be hundreds for each term. Consequently, for efficiency and accuracy’s sake we only consider articles which receive at least 1% of the anchor’s links.)
    void most_closely_related_pair(String term1, String term2,  String c1, String c2)
    {
        ArrayList<String> concepts_list1 = new ArrayList<String>();
        ArrayList<String> concepts_list2 = new ArrayList<String>();

        cDhObject.get_term_concepts_list(term1,  concepts_list1);
        cDhObject.get_term_concepts_list(term2,  concepts_list2);

        double max_relatedness = 0, temp_relatedness;
        c1 = "";
        c2 = "";
        SRCObj.setConcept_1("");
        SRCObj.setConcept_2("");

        for(String concept1 : concepts_list1)
        {
            for(String concept2 : concepts_list2)
            {
            	SRCObj.setConcept_1(concept1);
            	SRCObj.setConcept_2(concept2);

                temp_relatedness = SRCObj.calculate_semantic_relatedness();
                if (max_relatedness < temp_relatedness)
                {
                    c1 = concept1;
                    c2 = concept2;
                    max_relatedness = temp_relatedness;
                }
            }
        }

       
    }

    void most_closely_related_pairs( LinkedHashMap<Integer, nGram> pos_ngram_dic)
    {
    	Iterator<Integer> positions = pos_ngram_dic.keySet().iterator();
    	//choosing the context term
    	while(positions.hasNext())
    	{
    		int curPos = positions.next();
    		if (pos_ngram_dic.get(curPos).concepts!=(null) && pos_ngram_dic.get(curPos).concepts.size() ==1)
    		{
    			nGram temp_ngram = pos_ngram_dic.get(curPos);
    			temp_ngram.is_context_term = true;
    			temp_ngram.concept = temp_ngram.concepts.get(0);
    			pos_ngram_dic.put(curPos, temp_ngram);
    		}
    	}
    	
        calculate_context_term_weights( pos_ngram_dic);
        
        positions = pos_ngram_dic.keySet().iterator();
        while(positions.hasNext())
    	{
        	int curPos = positions.next();
            if (pos_ngram_dic.get(curPos).concepts != null && pos_ngram_dic.get(curPos).concepts.size() > 1)
            {
                nGram temp_ngram = pos_ngram_dic.get(curPos);
                disambiguate_sense( temp_ngram,  pos_ngram_dic);

                if (temp_ngram.concept != null)
                	pos_ngram_dic.put(curPos, temp_ngram);                   
            }
        }
    }

    void disambiguate_sense( nGram ngram,  LinkedHashMap<Integer, nGram> pos_ngram_dic)
    {
        double inlinks_sum = get_ngram_inlinks_count(ngram.concepts);
        double max_average = 0, temp_average;

        for (String concept : ngram.concepts)
        {
            if (sensible_probability(concept, inlinks_sum) >= sensible_probability_threshold)
            {
                temp_average = weighted_average_relatedness(concept, pos_ngram_dic);
                if (max_average < temp_average)
                {
                    max_average = temp_average;
                    ngram.concept = concept;
                }
            }
        }
    }

    void calculate_context_term_weights( LinkedHashMap<Integer, nGram> pos_ngram_dic)
    {
    	Iterator<Integer> positions = pos_ngram_dic.keySet().iterator();
        
    	context_sense_relatedness_table = create_context_senses_relatedness_table(pos_ngram_dic);

        //choosing the context term
        while(positions.hasNext())
        {
        	int curPos = positions.next();
            if (pos_ngram_dic.get(curPos).is_context_term)
            {
                nGram temp_ngram = pos_ngram_dic.get(curPos);
                temp_ngram.weight = get_central_thread_relatedness( pos_ngram_dic, temp_ngram);
                pos_ngram_dic.put(curPos,temp_ngram);
            }
        }
    }

    double get_central_thread_relatedness( LinkedHashMap<Integer, nGram> context_ngrams, nGram target_ngram)
    {
        double sum = 0.0;
        int context_ngram_count = 0;
        
        for (Map.Entry<Integer, nGram> ngram : context_ngrams.entrySet())
        {
            if (ngram.getValue().is_context_term)
            {
                sum += get_relatedness_value_from_table(target_ngram.concept, ngram.getValue().concept);
                context_ngram_count++;
            }
        }

        return (sum - 1) / (context_ngram_count - 1);
    }

    double get_relatedness_value_from_table(String c_id_1, String c_id_2)
    {
        if (c_id_1 == c_id_2)
            return 1;
        else return context_sense_relatedness_table[concept_index_dic.get(c_id_1)][concept_index_dic.get(c_id_2)];
       
    }

    double[][] create_context_senses_relatedness_table(Map<Integer, nGram> ngram_dic)
    {        
        int index = 0;
        
        ArrayList<String> conceptsOfContextTerms = new ArrayList<String>();

        for (Map.Entry<Integer, nGram> ngram : ngram_dic.entrySet())
            //get all concepts for context n-grams
            if (ngram.getValue().is_context_term)
            	if(!conceptsOfContextTerms.contains(ngram.getValue().concept))
            		{
            			conceptsOfContextTerms.add(ngram.getValue().concept);
            			concept_index_dic.put(ngram.getValue().concept, index++);
            		}
        
        double[][] table = new double[index][index];

        String[] cArray = new String[conceptsOfContextTerms.size()];
        String[] concepts = conceptsOfContextTerms.toArray(cArray);
        
        //Map<String, ArrayList<Double>> context_concepts = new HashMap<String, ArrayList<Double>>();
        for (int x = 0; x < concepts.length; x++)
        {
            SRCObj.setConcept_1(concepts[x]);
 
            for (int y = x + 1; y < concepts.length; y++)
            {            	
                SRCObj.setConcept_2(concepts[y]);
                table[concept_index_dic.get(concepts[x])][concept_index_dic.get(concepts[y])] 
              = table[concept_index_dic.get(concepts[y])][concept_index_dic.get(concepts[x])] 
              = SRCObj.calculate_semantic_relatedness();
                                  
            }  
        }

        return table;
    }

    //3. The best results are obtained when both commonness and relatedness are considered. Evenly weighting the candidate senses by these variables and choosing the pair with the highest weight—highest (commonness + relatedness)—gives exactly the same results as with manual disambiguation
    //void commonness_and_relatedness(string term1, string term2, out string c1, out string c2)
    //{ }
    //4. Interestingly we can improve upon this by making a simple "sequential decision", which first groups the most related pairs of candidates (within 40% of the most related pair) and then chooses the most common pair. This makes subtly different choices from those made manually.
    //void sequential_decision(string term1, string term2, out string c1, out string c2)
    //{}
    //5. we can also consider the case where two words are closely related because they belong together in the same phrase: e.g. family planning is a well-known phrase, and consequently family and planning are given a high semantic relatedness by humans even though their respective concepts are relatively disjoint. To identify these cases we simply concatenate the terms and see whether this is used as an anchor. If so, the frequency with which the anchor is used is normalized and added to the relatedness score of the original terms.
    public void disambiguate_terms(LinkedHashMap<Integer, nGram> pos_struct_dic)
    {
        disambiguate_concepts( pos_struct_dic, disambiguation_technique);
    }

    double get_ngram_inlinks_count(ArrayList<String> ngram_senses)
    {
        double total_count = 0;

        for (String sense : ngram_senses)
        {
            total_count += cDhObject.get_inLinks_count(sense);
        }
        return total_count;
    }

    double sensible_probability(String candidate_sense, ArrayList<String> senses)
    {            
        double total_count = 0;
        
        for (String sense : senses)
        {
            total_count += cDhObject.get_inLinks_count(sense);
        }
        return cDhObject.get_inLinks_count(candidate_sense) / total_count;
    }

    double sensible_probability(String candidate_sense, double total_count)
    {
        return cDhObject.get_inLinks_count(candidate_sense) / total_count;
    }

    //Average of weighted Relatedness
    public double weighted_average_relatedness(String concept, Map<Integer, nGram> concepts_list)
    {       
        SRCObj.setConcept_1(concept); 
        double relatedness_aggregation = 0;
        int context_sense_count = 0;       
      
        for(Map.Entry<Integer, nGram> c : concepts_list.entrySet())
        {
            if (c.getValue().is_context_term)
            {
                context_sense_count++;
                SRCObj.setConcept_2(c.getValue().concept);

                relatedness_aggregation += c.getValue().weight * SRCObj.calculate_semantic_relatedness();
            }
        }
        return relatedness_aggregation / context_sense_count;
    }
}
