package arabagent;

import java.util.*;

public class FeatureVector {
	ArrayList<String> attributes_terms = new ArrayList<String>();
	ArrayList<Double> attributes_values = new ArrayList<Double>();

	
    public int getAttributeCount()
    {
    	return attributes_terms.size();    	
    }

    public ArrayList<Double> getVectorValues()
    {
    	return attributes_values;
    }
    
    public void setVectorValues(ArrayList<Double> vectorValues)
    {

        attributes_values = vectorValues;
    }

    public ArrayList<String> getVectorTerms()
    {
        return attributes_terms;    
    }
    
    public void setVectorTerms(ArrayList<String> vectorTerms)
    {     
        attributes_terms = vectorTerms;
    }

    public int Term_Index(String term)
    {
        return attributes_terms.indexOf(term);
    }

    public boolean is_Term_Exist(String term)
    {
        return attributes_terms.contains(term);
    }
}
