package arabagent;

import java.util.*;

public class UserModel {
	
    private int numberOfConcepts;
    private HashMap<String, Integer> conceptIndex;
    private HashMap<String, Double> conceptWeights;
    private double[][] edgeWeightsMatrix;
    
    
    public UserModel(){
    	numberOfConcepts = 0;
    	conceptIndex = new HashMap<String, Integer>();
    	conceptWeights = new HashMap<String, Double>();
    	//SemanticNetwork = new UndirectedWeightedGraph();
    }
    
    void addConceptWithWeight(String conceptID, double conceptWeight)
    {    	
    	conceptWeights.put(conceptID, conceptWeight);
    	if(!conceptIndex.containsKey(conceptID))
    		conceptIndex.put(conceptID, numberOfConcepts++);
    }
    
    void createEdgeRepository()throws ConceptNotFoundException
    {
    	if(numberOfConcepts != 0)
    		edgeWeightsMatrix = new double[numberOfConcepts][numberOfConcepts];
    	else throw new ConceptNotFoundException();
    }
    
    void addEdgeWithWeight(String conceptID1, String conceptID2, double conceptWeight) 
    		throws ConceptNotFoundException
    {
    	if(edgeWeightsMatrix == null)
    		createEdgeRepository();
    	try{
    	edgeWeightsMatrix[conceptIndex.get(conceptID1)][conceptIndex.get(conceptID2)] = conceptWeight; 
    	edgeWeightsMatrix[conceptIndex.get(conceptID2)][conceptIndex.get(conceptID1)] = conceptWeight;
    	}catch(Exception e)
    	{throw new ConceptNotFoundException();}
    }
    
    List<String> getConceptsList()
    {
    	return Arrays.asList(conceptWeights.keySet().toArray(new String[conceptWeights.keySet().size()]));
    	}
    
    double getConceptWeight(String conceptID) throws ConceptNotFoundException
    {
    	if(conceptWeights.containsKey(conceptID))
    		return conceptWeights.get(conceptID);
    	else throw new ConceptNotFoundException();
    }
    
    double getEdgeWeight(String conceptID1, String conceptID2)throws ConceptNotFoundException
    {
    	try{
        	if(edgeWeightsMatrix == null)
    			createEdgeRepository();
        	
        	return edgeWeightsMatrix[conceptIndex.get(conceptID1)][conceptIndex.get(conceptID2)];
    	}catch(Exception e)
    	{throw new ConceptNotFoundException();}
    }
    
    void update(HashMap<String, Double> conceptImp)
    {}
    
    HashMap<String, Double> getAllConceptsWithTheirWeights()
    {
    	return conceptWeights;
    }
    
    boolean isConceptExist(String conceptID){
    	if(conceptWeights.containsKey(conceptID))
    		return true;
    	else return false;
    }
    
    boolean isConnectionExist(String conceptID1, String conceptID2){
    	if(isConceptExist(conceptID1) && isConceptExist(conceptID2))
	    	if(edgeWeightsMatrix[conceptIndex.get(conceptID1)][conceptIndex.get(conceptID2)] != 0.0)
	    		return true;
	    	else return false;
    	else return false;    		
    }
}