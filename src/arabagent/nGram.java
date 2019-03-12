package arabagent;

import java.util.*;

public class nGram {
	public String ngram;
    public ArrayList<String> concepts;
    public String concept;
    //public int freq;
    public boolean is_context_term;
    
    //(only for context terms) this weight of contribution of the context term in the document (the extent of concept closeness to 
    //the central thread of the document)  
    public double weight;
}
