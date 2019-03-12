package arabagent;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.ConcurrentModificationException;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Set;
import java.util.TreeSet;

import com.google.appengine.api.datastore.DatastoreService;
import com.google.appengine.api.datastore.DatastoreServiceFactory;
import com.google.appengine.api.datastore.Entity;
import com.google.appengine.api.datastore.EntityNotFoundException;
import com.google.appengine.api.datastore.FetchOptions;
import com.google.appengine.api.datastore.Key;
import com.google.appengine.api.datastore.KeyFactory;
import com.google.appengine.api.datastore.PreparedQuery;
import com.google.appengine.api.datastore.Query;
import com.google.appengine.api.datastore.Transaction;
import com.google.appengine.api.users.User;
import com.google.appengine.api.users.UserService;
import com.google.appengine.api.users.UserServiceFactory;
import com.sun.xml.internal.bind.v2.runtime.unmarshaller.XsiNilLoader.Array;

public class UserProfile{
	DatastoreService aaDataStore;
	UserService aaUserService;
	User aaUser;
	Entity userEntity;
	
	UserProfile(User user) throws EntityNotFoundException{
		aaDataStore = DatastoreServiceFactory.getDatastoreService();
		aaUserService = UserServiceFactory.getUserService();
		aaUser = user;
		Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
		try{
			userEntity = aaDataStore.get(userKey);
		}catch(EntityNotFoundException e){
			throw e;
		}
	}	
	/*
	private void addNewConceptAndNewOccur(String conceptKeyName, Double conceptImportance)
	{
		while(true)
		{
			int retries = 100;
			Transaction txn = aaDataStore.beginTransaction();
			try{
				Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
				Entity concept = new Entity("Concept", conceptKeyName, userKey);
				concept.setProperty("experience", conceptImportance);
				Entity conceptOccur = new Entity("ConceptOccur", conceptKeyName, userKey);
				conceptOccur.setProperty("date", new Date().getTime());
				conceptOccur.setUnindexedProperty("importance", conceptImportance);
				
				aaDataStore.put(Arrays.asList(concept, conceptOccur));
				
				txn.commit();
				break;
			}
			catch(ConcurrentModificationException e){
				if(retries == 0)
					throw e;
				retries--;
			}finally{
				if(txn.isActive())
				{txn.rollback();}
			}
		}
	}
	
	private void addNewOccurForTheConcept(String conceptKeyName, Double conceptImportance)
	{
		while(true)
		{
			int retries = 100;
			Key conceptKey = new KeyFactory.Builder("User", aaUser.getNickname()).addChild("Concept", conceptKeyName).getKey();
			Transaction txn = aaDataStore.beginTransaction();
			try{
				//update the Experience of the concept
				//Entity concept = new Entity(conceptKey);						
				//double newExperience = (Double)concept.getProperty("Experience")+ conceptImportance;
				//A best practice for working with counters is to use a technique known as " counter-sharding".
				//concept.setProperty("experience", newExperience);
				
				//make new occurrence 
				Entity conceptOccur = new Entity("ConceptOccur",conceptKey);
				conceptOccur.setProperty("date", new Date().getTime());
				conceptOccur.setUnindexedProperty("importance", conceptImportance);				
				
				aaDataStore.put(conceptOccur);
				
				txn.commit();
				break;
			}
			catch(ConcurrentModificationException e){
				if(retries == 0)
					throw e;
				retries--;
			}finally{
				if(txn.isActive())
				{txn.rollback();}
			}
		}
	}
	
	private void addNewConnection(String conceptKeyName1, String conceptKeyName2, Double importance)
	{
		while(true)
		{
			int retries = 100;
			//Key conceptKey1 = new KeyFactory.Builder("User", aaUser.getNickname()).addChild("Concept", conceptKeyName1).getKey();
			//Key conceptKey2 = new KeyFactory.Builder("User", aaUser.getNickname()).addChild("Concept", conceptKeyName2).getKey();
			Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
			Transaction txn = aaDataStore.beginTransaction();
			try{
				Entity connOccur = new Entity("ConnOccur",userKey);
				connOccur.setProperty("c1", conceptKeyName1);
				connOccur.setProperty("c2", conceptKeyName2);
				connOccur.setProperty("date", new Date().getTime());
				connOccur.setUnindexedProperty("importance", importance);
				
				aaDataStore.put(connOccur);
				
				txn.commit();
				break;
			}
			catch(ConcurrentModificationException e){
				if(retries == 0)
					throw e;
				retries--;
			}finally{
				if(txn.isActive())
				{txn.rollback();}
			}
		}
	}
	*/
	
	private boolean isEntityExist(String kind, String keyName, Key parentKey)
	{		 
		try{
			Key conceptKey = KeyFactory.createKey(parentKey, kind, keyName);
			aaDataStore.get(conceptKey);
			return true;
		}catch(EntityNotFoundException e)
		{return false;}
	}
	
	void update()
	{		
		long lastUpdateTime = (Long)userEntity.getProperty("lastUpdateTime");
		Long now = new Date().getTime();
		
		//update only if the last update is within more than 24 hours
		Long _24earlier = now - 24L*60L*60L*1000L;
		if(lastUpdateTime > _24earlier)
			return;		
		
		userEntity.setProperty("lastUpdateTime", now);
		
		Long _30days = 30L*24L*60L*60L*1000L;
		Long filterValue = now - _30days;
		
		Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
		
		Query conceptOccurQuery = new Query("ConceptOccur").setKeysOnly();
		conceptOccurQuery.setAncestor(userKey);		
		conceptOccurQuery.addFilter("date", Query.FilterOperator.LESS_THAN, filterValue);		
		//PreparedQuery conceptOccurResults1 = aaDataStore.prepare(conceptOccurQuery1);
		List<Entity> resultsKeys = aaDataStore.prepare(conceptOccurQuery).asList(FetchOptions.Builder.withChunkSize(100));
		
		List<Key> keys = new ArrayList<Key>();
		for (Entity result : resultsKeys) {
			keys.add(result.getKey());
		}
		
		Query connOccurQuery = new Query("ConnOccur").setKeysOnly();
		connOccurQuery.setAncestor(userKey);
		connOccurQuery.addFilter("date", Query.FilterOperator.LESS_THAN, filterValue);
		//PreparedQuery connOccurResults1 = aaDataStore.prepare(connOccurQuery1);
		resultsKeys = aaDataStore.prepare(connOccurQuery).asList(FetchOptions.Builder.withChunkSize(100));
		
		for (Entity result : resultsKeys) {
			keys.add(result.getKey());
		}
		
		Transaction txn = aaDataStore.beginTransaction();
		try{
		aaDataStore.delete(keys);
		txn.commit();
		}finally{
			if(txn.isActive())
				txn.rollback();
		}
	}
	
	/*	
	private double calculateDayImportance()
    {
      	//The date at the end of the last century 
        Date d1 = new GregorianCalendar(2012, 0, 12, 01, 16).getTime();

        //Today's date
        Date today = new Date();

        // Get msec from each, and subtract.
        long diff = today.getTime() - d1.getTime();
        diff = 30 - (diff / (1000 * 60 * 60 * 24));
        return diff/30.0;
    }  
	*/
    
	void update(HashMap<String, Double> conceptImp_dic)
    {        
    	Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
    	Long now = new Date().getTime();
        String[] conceptImp = conceptImp_dic.keySet().toArray(new String[conceptImp_dic.size()]);
        
        ArrayList<Entity> entities = new ArrayList<Entity>();
        
        Entity conceptOccur;
        Entity connOccur;
        
        double minmumValue = 0;
        
        for(int i=0; i<conceptImp.length; i++)
        {
			conceptOccur = new Entity("ConceptOccur", conceptImp[i], userKey);
			conceptOccur.setProperty("date", now);
			conceptOccur.setUnindexedProperty("importance", conceptImp_dic.get(conceptImp[i]));
			
			entities.add(conceptOccur);
			
        	for(int j=i+1; j<conceptImp.length; j++)
        	{
        		minmumValue = Math.min(conceptImp_dic.get(conceptImp[i]), 
        				conceptImp_dic.get(conceptImp[j]));
        		
        		connOccur = new Entity("ConnOccur",userKey);
				connOccur.setProperty("c1", conceptImp[i]);
				connOccur.setProperty("c2", conceptImp[j]);
				connOccur.setProperty("date", new Date().getTime());
				connOccur.setUnindexedProperty("importance", minmumValue);
				
				entities.add(connOccur);
        	}
        }
        
		Transaction txn = aaDataStore.beginTransaction();
		
		try{
			aaDataStore.put(entities);
			txn.commit();
		}catch(ConcurrentModificationException e)
		{
			if(txn.isActive())
				txn.rollback();
		}
    }
	
	private void putCollectionOfEntities(ArrayList<Entity> entities)
	{	
			Transaction txn = aaDataStore.beginTransaction();
			try{			
				aaDataStore.put(entities);
				
				txn.commit();
			}
			finally{
				if(txn.isActive())
				{txn.rollback();}
			}
	}

	/*
	 * 	void update(HashMap<String, Double> conceptImp_dic)
    {        
    	Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
    	Long now = new Date().getTime();
        String[] conceptImp = conceptImp_dic.keySet().toArray(new String[conceptImp_dic.size()]);
        for(int i=0; i<conceptImp.length; i++)
        {
			Entity conceptOccur = new Entity("ConceptOccur", conceptImp[i], userKey);
			conceptOccur.setProperty("date", now);
			conceptOccur.setUnindexedProperty("importance", conceptImp_dic.get(conceptImp[i]));
			
        	if(isConceptExist(conceptImp[i]))
        		addNewOccurForTheConcept(conceptImp[i], conceptImp_dic.get(conceptImp[i]));
        	else
        		addNewConceptAndNewOccur(conceptImp[i], conceptImp_dic.get(conceptImp[i]));
       
        	for(int j=i+1; j<conceptImp.length; j++)
        	{
        		double minmumValue = Math.min(conceptImp_dic.get(conceptImp[i]), 
        				conceptImp_dic.get(conceptImp[j]));
        		addNewConnection(conceptImp[i], conceptImp[j], minmumValue);
        	}
        }            
    }
	 * */
	
    private boolean isConceptExist(String conceptString)
    {    	
    	return isEntityExist("ConceptOccur", conceptString, userEntity.getKey());
    }
    
    List<Entity> getConceptOccurs(){    	
    	Query conceptOccursQuery = new Query("ConceptOccur").setKeysOnly();
    	Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
    	conceptOccursQuery.setAncestor(userKey);
    	//PreparedQuery copq = aaDataStore.prepare(conceptOccursQuery);    
    	
    	return aaDataStore.prepare(conceptOccursQuery).asList(FetchOptions.Builder.withChunkSize(100));   
    }
    
    /*
    private List<Entity> getConceptOccurEntites(String conceptID)
    {
    	Query query = new Query("ConceptOccur");
    	
    	Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
    	query.setAncestor(KeyFactory.createKey(userKey, "Concept", conceptID));
    	
    	return aaDataStore.prepare(query).asList(FetchOptions.Builder.withDefaults());
    }
    */
    
    private List<Entity> getConnectionOccurEntites(String conceptID1, String conceptID2)
    {
    	Query query = new Query("ConnOccur");
    	Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
    	query.setAncestor(userKey);
    	
    	Set<String> f= new TreeSet<String>();
    	f.add(conceptID1);
    	f.add(conceptID2);
    	query.addFilter("c1", Query.FilterOperator.IN, f);
    	query.addFilter("c2", Query.FilterOperator.IN, f);
    	return aaDataStore.prepare(query).asList(FetchOptions.Builder.withChunkSize(100));
    }
    
    List<Entity> getAllConnectionOccurEntites()
    {
    	Query query = new Query("ConnOccur");
    	Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
    	query.setAncestor(userKey);
    	
    	return aaDataStore.prepare(query).asList(FetchOptions.Builder.withChunkSize(100));
    }
    
    private void deleteAllContent()
    {
    	Key userKey = KeyFactory.createKey("User", aaUser.getNickname());
    	
    	Query q1 = new Query("ConceptOccur").setKeysOnly();
    	Query q2 = new Query("ConnOccur").setKeysOnly();
    	Query q3 = new Query("Concept").setKeysOnly();
    	
    	q1.setAncestor(userKey);
    	q2.setAncestor(userKey);
    	q3.setAncestor(userKey);
    	
    	List<Entity> q1List = aaDataStore.prepare(q1).asList(FetchOptions.Builder.withChunkSize(100));
    	List<Entity> q2List = aaDataStore.prepare(q2).asList(FetchOptions.Builder.withChunkSize(100));
    	List<Entity> q3List = aaDataStore.prepare(q3).asList(FetchOptions.Builder.withChunkSize(100));    	
    	
		List<Key> keys = new ArrayList<Key>();
		
		for (Entity result : q1List) {
			keys.add(result.getKey());
		}
		
		for (Entity result : q2List) {
			keys.add(result.getKey());
		}
		
		for (Entity result : q3List) {
			keys.add(result.getKey());
		}
		
		aaDataStore.delete(keys);
    }
    
}
