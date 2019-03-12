package arabagent;

import java.net.URL;
import java.util.Date;
import java.util.HashMap;
import java.util.List;

import com.google.appengine.api.datastore.DatastoreService;
import com.google.appengine.api.datastore.DatastoreServiceFactory;
import com.google.appengine.api.datastore.Entity;
import com.google.appengine.api.datastore.EntityNotFoundException;
import com.google.appengine.api.users.User;

public class ModelerAndProfiler {
	private UserProfile curUserProfile;
	private UserModel curUserModel;
	private TextProcessing textProcessing;
	private User curUser;
	
	ModelerAndProfiler(User user, TextProcessing textProcessing) throws EntityNotFoundException{
		curUser = user;
		
		try{
		curUserProfile = new UserProfile(curUser);
		}catch(EntityNotFoundException e){throw e;}
		
		loadUserModel();
		this.textProcessing = textProcessing;
	}
	
    void createNewUserProfile(String userName){
    	DatastoreService aaDataStore = DatastoreServiceFactory.getDatastoreService();
    	Entity user = new Entity("User", userName);
		aaDataStore.put(user);		
    	}
    
	void loadUserModel()
	{
		UserModel userModel = new UserModel();
		try{
		//get concepts from the user profile
		/*String[] concepts = curUserProfile.getConcepts();
		
		for(int i=0; i<concepts.length; i++)
		{
			List<Entity> conceptOccurs = curUserProfile.getConceptOccurEntites(concepts[i]);
			double test = calculateConceptImportance(conceptOccurs);
			userModel.addConceptWithWeight(concepts[i], test);
		}*/
			List<Entity> conceptOccurs = curUserProfile.getConceptOccurs();
			double newWeight = 0.0;
			
			for(Entity conceptOccur : conceptOccurs)
			{
				System.out.println(conceptOccur.getKey().getName());
				if(userModel.isConceptExist(conceptOccur.getKey().getName()))
				{
					newWeight = userModel.getConceptWeight(conceptOccur.getKey().getName()) + 
									calculateConceptImportance(conceptOccur);
					userModel.addConceptWithWeight(conceptOccur.getKey().getName(), newWeight);
				}else{
					userModel.addConceptWithWeight(conceptOccur.getKey().getName(), 
									calculateConceptImportance(conceptOccur));
				}
			}
		
		List<Entity> connectionOccurs = curUserProfile.getAllConnectionOccurEntites();
		String c1 = "";
		String c2 = "";
		
		for(Entity connOccur : connectionOccurs)
		{
			try {
				c1 = (String)connOccur.getProperty("c1");
				c2 = (String)connOccur.getProperty("c2");
				newWeight = calculateEdgeImportance(connOccur) + userModel.getEdgeWeight(c1, c2);
				
			userModel.addEdgeWithWeight(c1, c2,	newWeight);
			}
			catch (ConceptNotFoundException e2) {				
				e2.printStackTrace();
			}
		}
		}catch(Exception e)
		{e.getStackTrace();}
		
		curUserModel = userModel;
	}
	
	private double calculateConceptImportance(List<Entity> conceptOccurs)
	{		
		double weight = 0.0;
		
		for(Entity occur : conceptOccurs)
		{		
			weight += calculateDayImportance((Long)occur.getProperty("date"))* 
					(Double)occur.getProperty("importance");					
		}

		return weight;
	}
	
	private double calculateConceptImportance(Entity conceptOccur)
	{
		System.out.println("date: "+(Long)conceptOccur.getProperty("date"));
		System.out.println("importance: "+(Double)conceptOccur.getProperty("importance"));
		
		return calculateDayImportance((Long)conceptOccur.getProperty("date"))* 
				(Double)conceptOccur.getProperty("importance");
	}
	
	private double calculateEdgeImportance(Entity occur)
	{	
		return calculateDayImportance((Long)occur.getProperty("date"))*(Double)occur.getProperty("importance");
	}
	
    private double calculateDayImportance(long date)
    {
      	///** The date at the end of the last century */
        //Date d1 = new GregorianCalendar(2012, 0, 12, 01, 16).getTime();

        /** Today's date */
        long now = new Date().getTime();

        long diff = now - date;
        diff = 30 - (diff / (1000 * 60 * 60 * 24));
        return diff/30.0;
    }

    void updateUserProfile(URL url)
    {
        //calculate concept importance for the document
    	HashMap<String, Double> conceptImp = new HashMap<String, Double>();
    	textProcessing.calculateConceptImportancesForADocument(url, conceptImp);
    	
    	//this happen only when the user provide feedback
        curUserProfile.update(conceptImp);
        
        //to reflect the user profile
        curUserModel.update(conceptImp);
        
        //a peridical update to delete occurrence exceed more than 30 days
        curUserProfile.update();        
    }
    
    void updateUserProfile(String docText)
    {
        //calculate concept importance for the document
    	HashMap<String, Double> conceptImp = new HashMap<String, Double>();
    	textProcessing.calculateConceptImportancesForADocument(docText, conceptImp);
    	
    	//this happen only when the user provide feedback
        curUserProfile.update(conceptImp);
        
        //to reflect the user profile
        //curUserModel.update(conceptImp);
        loadUserModel();
        
        //a peridical update to delete occurrence exceed more than 30 days
        curUserProfile.update();        
    }
    
    HashMap<String, Double> getConceptsWithTheirWeights()
    {
    	return curUserModel.getAllConceptsWithTheirWeights();
    }
    
    double getShortTermUserInterestsForConcept(String conceptID) 
    {
    	try{
    		return curUserModel.getConceptWeight(conceptID);
    	}catch(ConceptNotFoundException e){
    		return 0;
    		}
    }
    
    double getShortTermUserInterestsForEdge(String conceptID1, String conceptID2)
    {
    	try{
    	return curUserModel.getEdgeWeight(conceptID1, conceptID2);
    	}catch(ConceptNotFoundException e)
    	{ return 0;}
    }
    
    boolean isConceptExist(String conceptID)
    {
    	return curUserModel.isConceptExist(conceptID);
    }
}
