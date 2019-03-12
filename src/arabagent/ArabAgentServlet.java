package arabagent;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.Date;

import javax.servlet.http.*;

import com.google.appengine.api.datastore.DatastoreService;
import com.google.appengine.api.datastore.DatastoreServiceFactory;
import com.google.appengine.api.datastore.Entity;
import com.google.appengine.api.datastore.EntityNotFoundException;
import com.google.appengine.api.datastore.Key;
import com.google.appengine.api.datastore.KeyFactory;
import com.google.appengine.api.datastore.Transaction;
import com.google.appengine.api.users.User;
import com.google.appengine.api.users.UserService;
import com.google.appengine.api.users.UserServiceFactory;

@SuppressWarnings("serial")
public class ArabAgentServlet extends HttpServlet {
	DatastoreService aaDataStore;
	UserService aaUserService;
	User aaUser;
    ModelerAndProfiler mAP;
    Personalizer personalizer;
 
	public void doGet(HttpServletRequest req, HttpServletResponse resp)
			throws IOException {
		aaDataStore = DatastoreServiceFactory.getDatastoreService();
		aaUserService = UserServiceFactory.getUserService();
		aaUser = aaUserService.getCurrentUser();
		resp.setContentType("text/plain");
		
		if(aaUser != null)
		{

			
		}else{
			resp.sendRedirect(aaUserService.createLoginURL(req.getRequestURI()));
		}
	}
	
	public void doPost(HttpServletRequest req, HttpServletResponse resp) throws IOException 
    {
		if(mAP == null)
			loadUser();		
		
		String content = req.getParameter("content");
		String sim = req.getParameter("sim");
		String update = req.getParameter("update");
	    
	    if (content != null && !content.isEmpty()) {
	    	
	    	if(update != null)
	    	{
	    		updateAgent2(content);
	    		resp.getWriter().println("Your Profile has been updated, press \"back\".");
	    	}
	    	
	    	if(sim != null)
	    	{
	    		double similarity = personalizer.calculateSimilarity(content)*100.0;
	    		
	    		resp.getWriter().println("The Document interests you by "+similarity+"%");
	    		resp.getWriter().println("Press \"back\" and try again.");
	    	}
	    	
	    	}else{
	    		resp.getWriter().println("The Text box is Empty, press \"back\" and try again.");
	    	}


    //resp.sendRedirect("/arabagent.jsp");
    //resp.
    }
	
	private boolean isUserExist(String userName)
	{
		try{
			Key userKey = KeyFactory.createKey("User", userName);
			aaDataStore.get(userKey);
			return true;
		}catch(EntityNotFoundException e){
			return false;
				//aaDataStore.put(new Entity("User", aaUser.getNickname()));				
			}
	}
	
	private void createUser(String userName)
	{
		//creatUserProfile(userName);
		Transaction txn = aaDataStore.beginTransaction();
			try{
				Entity userEntity = new Entity("User", aaUser.getNickname());
				long now =  new Date().getTime();
				userEntity.setUnindexedProperty("creationDate",now);
				userEntity.setUnindexedProperty("lastUpdateTime",now);
				
				aaDataStore.put(userEntity);
			txn.commit();
			}
			finally{
			if(txn.isActive())
				txn.rollback();}	
	}
	
    void updateAgent(String urlString) throws MalformedURLException
    {  	
    	try{
    	URL url = new URL(urlString);
    	mAP.updateUserProfile(url);
    	}
    catch(MalformedURLException e)
    {throw e;}
    	
    }
    
    void updateAgent2(String docText) 
    {
    	mAP.updateUserProfile(docText);    	
    }
    
    void loadUser()
    {
    	try{
			aaDataStore = DatastoreServiceFactory.getDatastoreService();
			aaUserService = UserServiceFactory.getUserService();
			aaUser = aaUserService.getCurrentUser();
			
			if(isUserExist(aaUser.getNickname()))
			{	
					if(mAP == null)
					{
						TextProcessing tp = new TextProcessing();
						mAP = new ModelerAndProfiler(aaUser, tp);
						personalizer = new Personalizer(mAP, tp);						
					}
				//updateAgent("http://gate.ahram.org.eg/UI/Front/RSSContent.aspx?NewsPortalID=0");			
			}else{
				createUser(aaUser.getNickname());
				loadUser();
			}
	    }catch (Exception e)
	    {
	    	e.getStackTrace();
	    }
    }

}
