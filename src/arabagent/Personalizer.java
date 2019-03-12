package arabagent;

import java.util.ArrayList;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

	public class Personalizer {
	private ModelerAndProfiler dataAccess;
	private TextProcessing textProcessing;
	
	Personalizer(ModelerAndProfiler dataAccess,	TextProcessing textProcessing){
		this.dataAccess = dataAccess;		
		this.textProcessing = textProcessing;
	}
	
	double calculateSimilarity(String docText){
		//Identify the concepts of the article to be concepts and 
		//initialize all nodes of the article network each to the corresponding concept importance
    	HashMap<String, Double> conceptImp = new HashMap<String, Double>();
    	
    	textProcessing.calculateConceptImportancesForADocument(docText, conceptImp);
    	
    	UserModel docGraph = new UserModel();
		for(Map.Entry<String, Double> conceptImpPair : conceptImp.entrySet())		
			docGraph.addConceptWithWeight(conceptImpPair.getKey(), conceptImpPair.getValue());
		
		//Set the activity level of all nodes that occur in both article graph and the user model 
		//to the weight of that same node in the user model multiplied by the article node weight
		List<String> docConcepts = docGraph.getConceptsList();
		
		//temporary to store only those common in both graphs
		List<String> commonConcepts = new ArrayList<String>();
		
		try{
		for(String concept : docConcepts)
		{
			if(dataAccess.isConceptExist(concept))
			{
				commonConcepts.add(concept);
				double st_interest = dataAccess.getShortTermUserInterestsForConcept(concept);
				if(st_interest!=0)			
					docGraph.addConceptWithWeight(concept, docGraph.getConceptWeight(concept)*st_interest);
			}
		}
		
		
		String[] conceptsArray = docConcepts.toArray(new String[docConcepts.size()]);
		//spread the weights out to the neighboring nodes in the article graph using the spreading 
		//activity mechanism.
		for(int i=0; i<conceptsArray.length; i++)
		{
			for(int j=i+1; j<conceptsArray.length; j++)
			{
				double st_interest = dataAccess.getShortTermUserInterestsForEdge(conceptsArray[i], conceptsArray [j]);
				
				docGraph.addConceptWithWeight(conceptsArray[i], 
						docGraph.getConceptWeight(conceptsArray[i])+(docGraph.getConceptWeight(conceptsArray[j])*st_interest));
				
				docGraph.addConceptWithWeight(conceptsArray[j], 
						docGraph.getConceptWeight(conceptsArray[j])+(docGraph.getConceptWeight(conceptsArray[i])*st_interest));				
			}			
		}

		
		//aggregate the Importance of the common concepts and divide by the sum of the importance of all the
		//concepts of the documents
		double sumOfCommonConceptWeights = 0;
		double sumOfAllConceptInArticle = 0;
		
		for(int i=0; i<conceptsArray.length; i++)
		{
			sumOfAllConceptInArticle += docGraph.getConceptWeight(conceptsArray[i]);
			if(commonConcepts.contains(conceptsArray[i]))
				sumOfCommonConceptWeights += docGraph.getConceptWeight(conceptsArray[i]);			
		}
		return sumOfCommonConceptWeights/sumOfAllConceptInArticle;

		}catch(ConceptNotFoundException e){
			return 0.0;
		}		
	}
	/*
    public double calculateDocSimilarityWithProfile(LinkedHashMap<Integer, nGram> pos_struct_dic)
    {
        UndirectedWeightedGraph textGraph = new UndirectedWeightedGraph();
        HashMap<String, Double> concept_frq_dic = new HashMap<String, Double>();
        textProcessing.calculateConceptImportanceForeachConceptInTheDocument(pos_struct_dic, concept_frq_dic);

        // first we create a graph for the text as the profile
        createGraphForText(textGraph, concept_frq_dic);
        
        String[] conceptIds = textGraph.getNodesDictionary().keySet().toArray(new String[textGraph.getNodesDictionary().size()]);

        //temporary to store only those common in both graphs
        ArrayList<String> commonCId = new ArrayList<String>();

        for (int i = 0; i < conceptIds.length; i++)
        {
            if (userSemanticNetwork.getNodesDictionary().containsKey(conceptIds[i]))
            {
                textGraph.getNodesDictionary().get(conceptIds[i]).setNodeWeight(userSemanticNetwork.getNodesDictionary().get(conceptIds[i]).getNodeWeight());
                commonCId.add(conceptIds[i]);
            }
        }

        for (int i = 0; i < commonCId.size(); i++)
            for (int j = i + 1; j < commonCId.size(); j++)
            {
                //commonCId[i]
                if (userSemanticNetwork.getNodesDictionary().get(commonCId.get(i)).isNeighbour(commonCId.get(i)))
                {
                    textGraph.getNodesDictionary().get(commonCId.get(i)).setNodeWeight( 
                        textGraph.getNodesDictionary().get(commonCId.get(i)).getNodeWeight() + 
                        (textGraph.getNodesDictionary().get(commonCId.get(i)).getNodeWeight() *
                    userSemanticNetwork.getNodesDictionary().get(commonCId.get(i)).relationWeight(commonCId.get(j))));

                    textGraph.getNodesDictionary().get(commonCId.get(j)).setNodeWeight( 
                        textGraph.getNodesDictionary().get(commonCId.get(j)).getNodeWeight() +
                        (textGraph.getNodesDictionary().get(commonCId.get(j)).getNodeWeight() *
                    userSemanticNetwork.getNodesDictionary().get(commonCId.get(j)).relationWeight(commonCId.get(i))));
                }
            }

        int numberOfNodesThatHaveWeightsAboveThreshold = 0;

        for (int i = 0; i < commonCId.size(); i++)
            if (textGraph.getNodesDictionary().get(commonCId.get(i)).getNodeWeight() > 0.2)
                numberOfNodesThatHaveWeightsAboveThreshold++;

        return (double)numberOfNodesThatHaveWeightsAboveThreshold / (double)textGraph.getVerticsCount();
       
            //compare it with profile graph and then return a double value that represent how much the user interest 
            //in the text the following function is created for evaluation perpose only
    }
 */
    public boolean isInteresting(String docText)
    {
        if (calculateSimilarity(docText) > 0.4)
            return true;
        else return false;
    }

}