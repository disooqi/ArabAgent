package arabagent;

import java.net.URL;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Map;

public class TextProcessing {
	TermDetection termDetector;
	
	TextProcessing(){
		termDetector = new TermDetection();
	}	
    
    void calculateConceptImportancesForADocument(URL url, HashMap<String, Double> conceptFreq)
    {
    	LinkedHashMap<Integer, nGram> posNgram = new LinkedHashMap<Integer, nGram>();
    	posNgram = getDocumentConcepts(url);
    	
        int totalConceptCount = 0;
        //calculate the frequency for each concept in "pos_ngram_dic"
        for(Map.Entry<Integer, nGram> ngram : posNgram.entrySet())
            if (ngram.getValue().concept != null)
            {
                if (conceptFreq.containsKey(ngram.getValue().concept))                    
                	conceptFreq.put(ngram.getValue().concept, conceptFreq.get(ngram.getValue().concept)+1);
                else conceptFreq.put(ngram.getValue().concept, 1.0);

                totalConceptCount++;
            }
        //then, using the freq. just calculated, we compute the concept Importance for every concept in the list 
        for (Map.Entry<String, Double> conceptFreqPair : conceptFreq.entrySet())
        {
        	//this (if) is to filter the concept and choose only those with freq. greater than two
        	//so that the profile would be more representative
            //if (conceptFreqPair.getValue() > 2)
        	conceptFreq.put(conceptFreqPair.getKey(), conceptFreqPair.getValue() / totalConceptCount);
        }
    }
    
    void calculateConceptImportancesForADocument(String text, HashMap<String, Double> conceptFreq)
    {
    	LinkedHashMap<Integer, nGram> posNgram = new LinkedHashMap<Integer, nGram>();
    	posNgram = getDocumentConcepts(text);
    	
        int totalConceptCount = 0;
        //calculate the frequency for each concept in "pos_ngram_dic"
        for(Map.Entry<Integer, nGram> ngram : posNgram.entrySet())
            if (ngram.getValue().concept != null)
            {
                if (conceptFreq.containsKey(ngram.getValue().concept))                    
                	conceptFreq.put(ngram.getValue().concept, conceptFreq.get(ngram.getValue().concept)+1);
                else conceptFreq.put(ngram.getValue().concept, 1.0);

                totalConceptCount++;
            }
        //then, using the freq. just calculated, we compute the concept Importance for every concept in the list 
        for (Map.Entry<String, Double> conceptFreqPair : conceptFreq.entrySet())
        {
        	//this (if) is to filter the concept and choose only those with freq. greater than two
        	//so that the profile would be more representative
            //if (conceptFreqPair.getValue() > 2)
        	conceptFreq.put(conceptFreqPair.getKey(), conceptFreqPair.getValue() / totalConceptCount);
        }
    }

    private LinkedHashMap<Integer, nGram> getDocumentConcepts(URL url)
    {		
		//retrieveWebSite
		String WebPageKeywords = getHTMLofWebpage(url);

		//detect the concepts and their occurrence frequency
		LinkedHashMap<Integer, nGram> pos_struct_dic = new LinkedHashMap<Integer, nGram>();
		termDetector.wikify(WebPageKeywords, pos_struct_dic);
                            
		//send a list of concepts with their occurrence frequencies to User Model to update the profile
		return pos_struct_dic;
    }
    
    private LinkedHashMap<Integer, nGram> getDocumentConcepts(String WebPageKeywords)
    {		
		//detect the concepts and their occurrence frequency
		LinkedHashMap<Integer, nGram> pos_struct_dic = new LinkedHashMap<Integer, nGram>();
		termDetector.wikify(WebPageKeywords, pos_struct_dic);
                            
		//send a list of concepts with their occurrence frequencies to User Model to update the profile
		return pos_struct_dic;
    }
    
    private String getHTMLofWebpage(URL url) 
    {
    	return "أبو الفتوح: استمرار \"العسكري\" في السلطة خطر على الاقتصاد والأمن القومي "
    			+ "أكد د. عبد المنعم أبو الفتوح، المرشح المحتمل لرئاسة الجمهورية، وجوب إعلاء مصلحة الوطن فوق الاختلافات السياسية والشخصية، والبعد عن الأساليب المستخدمة في تخوين بعضنا البعض، حتى نستطيع استكمال أهداف الثورة.  أوضح أبو الفتوح خلال لقائه مع فضيلة الإمام الأكبر أحمد الطيب شيخ الأزهر‪ ‬والبابا شنودة‪ ‬ورئيس الوزراء وفضيلة المرشد العام للإخوان الملسمين‪ ‬وبعض ممثلي القوى السياسية في مشيخة الأزهر بعنوان \"استكمال أهداف الثورة واستعادة الروح\"، أن الإختلاف بيننا من سنن الله في الكون، فيجب ألا يكون هذا الاختلاف عائقاً أمام الاتفاق على المصلحة العليا للوطن، قائلاً: \"إننا تحدثنا كثيراً حول شئون الوطن وخدمته، ويجب أن يتحوّل هذا الكلام إلى أفعال. وتابع قائلاً: \"إننا نفرق بين الجيش المصري الذي يحمي الوطن ويبدي مصلحته على أي مصلحة أخرى، وبين المجلس العسكري كمدير للمرحلة الإنتقالية والذي يمثل استمرار وجوده خطرا حقيقيا على الاقتصاد والأمن القومي، ويجب تسليمه السريع للسلطة من أجل صالح الوطن. " 
    			+ "أبدى أبو الفتوح سعادته باختيار شيخ الأزهر لعنوان اللقاء الذي جمع فيه كل أطياف القوى السياسية باستكمال أهداف الثورة والتي بدأت فعلياً من خلال ما شاهدناه من نزاهة في انتخابات مجلس الشعب، والتي عبرت عن رقي هذا الشعب، كما قام أفراد الجيش والقضاة بواجبهم لإنجاح هذه الانتخابات. "
    			+ "وأكمل أبو الفتوح أنه من أجل مصلحة الوطن علينا أن نحترم الدستور الجديد بإعطائه الوقت الكافي لإجراء حوار مجتمعي بشأنه حتى يعبر عن إرادة حقيقية للشعب المصري، بالإضافة لإعطاء الأحزاب المنتخبة حقها في إدارة البلاد لأنها جاءت بإرادة شعبية حرة.";
    	/*URL url;
	    try { 
		url = new URL(urltext);
		URLConnection urlConnection = url.openConnection();
		urlConnection.setConnectTimeout(60000);
		urlConnection.setReadTimeout(60000);
		//int timeOut = urlConnection.getConnectTimeout();
		urlConnection.setAllowUserInteraction(false);
		InputStream urlStream = url.openStream();
		InputStreamReader  fc = new InputStreamReader(urlStream, "utf8");
		BufferedReader br = new BufferedReader(fc);
		
    	String htmlStr = "";
    	String temp= "";
    	
    	while((temp = br.readLine())!= null)
    	{htmlStr += temp;}
    	
    	return htmlStr;

	    }catch (MalformedURLException e) {		
	    	e.getMessage();
	    	return "";
	    }catch( IOException e)
	    {
	    	e.getMessage();
	    	return "";
	    } 	
    	

    	/*
        //http://www.akhbarelyom.org.eg/
        //<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
        try
        {
            WebClient client = new WebClient();

            //this method always download pages encoded in windows-1256
            string html = client.DownloadString(url);
            string charSet = get_page_charset_value(html);

            //the page iencoding is always utf-8 of no encoding is specified
            if (string.IsNullOrEmpty(charSet))
            {
                client.Encoding = Encoding.UTF8;
                html = client.DownloadString(url);
            }
            else if (Encoding.GetEncoding(charSet) != client.Encoding)
            {
                client.Encoding = Encoding.GetEncoding(charSet);
                html = client.DownloadString(url);
            }


            //Encoding page_encoding = Encoding.GetEncoding(charSet);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            //string html = (new StreamReader(resp.GetResponseStream(),target_encoding)).ReadToEnd();

            return html;
        }
        catch (Exception exc)
        {

            return "";
        }
        */
    }

}
