package arabagent;

@SuppressWarnings("serial")
public class ConceptNotFoundException extends Exception {
	String errorMessage;
	ConceptNotFoundException(){
		super();
		errorMessage = "Concept is not Exist";
	}	

	String errorMessage()
	{
		return errorMessage;
	}

}
