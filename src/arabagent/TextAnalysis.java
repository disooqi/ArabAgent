package arabagent;

import java.util.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class TextAnalysis {
	Map<String, Boolean> stopwords_dic = new HashMap<String, Boolean>();
	Map<String, Boolean> wikipedia_terms_dic = new HashMap<String, Boolean>();

    char ALEF_MAKSOURA = (char)1609;
    char YA2 = (char)1610;
    char TA2_MARBOUTA = (char)1577;
    char HA2 = (char)1607;
    char ALEF_HAMZA_FOU2 = (char)1571;
    char ALEF_HAMZA_TA7T = (char)1573;
    char ALEF_MAD = (char)1570;
    char ALEF_BEDON_HAMZA = (char)1575;
    char KASHIDA = (char)1600;
    

    /* doc_type:
     * 0 Word
     * 1 Plain text  // the defualt
     * 2 web document
     * 3 Trec document
     * 4 PDF document

     */
    int doc_type = 1; //default
    int analysis_type = 1; //default if not set
    /* 
     * 0: no analysis; 
     * 1: normalization 
     * 2: simple stemming 
     * 3: Light10 stemming 
     * 4: Wikipedia stemming
     * 5: Stopword Removal
     */

    String[] Larkey_defarticles = { "«·", "Ê«·", "»«·", "ﬂ«·", "›«·", "··" };
    String[] Larkey_suffixes = { "Â«", "«‰", "« ", "Ê‰", "Ì‰", "ÌÂ", "Ì…", "Â", "…", "Ì" };
    String[] stopwords_array = {
            "«‰","»⁄œ", "÷œ", "Ì·Ì", "«·Ï", "›Ì", "„‰", "Õ Ï", "ÊÂÊ", "ÌﬂÊ‰",
            "»Â", "Ê·Ì”", "√Õœ", "⁄·Ï", "Êﬂ«‰", " ·ﬂ", "ﬂ–·ﬂ", "«· Ì", "Ê»Ì‰",
            "›ÌÂ«", "⁄·ÌÂ«", "≈‰", "Ê⁄·Ï", "·ﬂ‰", "⁄‰", "„”«¡", "·Ì”", "„‰–",
            "«·–Ì", "√„«", "ÕÌ‰", "Ê„‰", "·Ì” ", "Êﬂ«‰ ", "√Ì", "„«", "⁄‰Â",
            "ÕÊ·", "œÊ‰", "„⁄", "·ﬂ‰Â", "Ê·ﬂ‰", "·Â", "Â–«", "Ê«· Ì","›ﬁÿ", "À„",
            "Â–Â", "√‰Â", " ﬂÊ‰", "ﬁœ", "»Ì‰", "Ãœ«", "·‰", "‰ÕÊ", "ﬂ«‰", "·Â„",
            "·√‰", "«·ÌÊ„", "·„", "Âƒ·«¡", "›≈‰", "›ÌÂ", "–·ﬂ", "·Ê", "⁄‰œ",
            "«··–Ì‰", "ﬂ·", "»œ", "·œÏ", "ÊÀÌ", "√‰", "Ê„⁄", "›ﬁœ", "»·", "ÂÊ",
            "⁄‰Â«", "„‰Â", "»Â«", "Ê›Ì", "›ÂÊ", " Õ ", "·Â«", "√Ê", "≈–", "⁄·Ì",
            "⁄·ÌÂ", "ﬂ„«", "ﬂÌ›", "Â‰«", "Êﬁœ", "ﬂ«‰ ", "·–·ﬂ", "√„«„", "Â‰«ﬂ",
            "ﬁ»·", "„⁄Â", "ÌÊ„", "„‰Â«", "≈·Ï", "≈–«", "Â·", "ÕÌÀ", "ÂÌ", "«–«",
            "«Ê", "Ê", "·«", "«·Ì", "≈·Ì", "„«“«·", "·«“«·", "·«Ì“«·",
            "„«Ì“«·", "«’»Õ", "√’»Õ", "√„”Ï", "«„”Ï", "√÷ÕÏ", "«÷ÕÏ", "Ÿ·",
            "„«»—Õ", "„«› ∆", "„««‰›ﬂ", "»« ", "’«—", "Ê·Ì” ", "≈‰", "ﬂ√‰",
            "·Ì ", "·⁄·", "·«”Ì„«", "Ê·«Ì“«·", "«·Õ«·Ì", "÷„‰", "«Ê·", "Ê·Â",
            "–« ", "«Ì", "»œ·«", "«·ÌÂ«", "«‰Â", "«·–Ì‰", "›«‰Â", "Ê«‰",
            "Ê«·–Ì", "ÊÂ–«", "·Â–«", "«·«", "›ﬂ«‰", "” ﬂÊ‰", "„„«", "√»Ê",
            "»≈‰", "«·–Ì", "«·ÌÂ", "Ì„ﬂ‰", "»Â–«", "·œÌ", "Ê√‰", "ÊÂÌ", "Ê√»Ê",
            "¬·", "«·–Ì", "Â‰", "«·–Ï", "Êﬂ–·ﬂ", "Ê„«", "Ê·Â–«", "Êﬁ»·", "«„«", 
            "» ·ﬂ", "»Â–Â", "»–·ﬂ", "·„«–«", "„«–«", "·„«", "Ê·„«", "Ê·„«–«",
            "Ê„«–«", "Ê·„", "„À·", "„À·«", "«»‰", "»⁄÷", "ﬂ·«", "ﬁ«·", "ﬁ«· ",
            "·ﬁœ", "Ê·ﬁœ", "Ê·⁄·", "«‰Â„", "«Ì÷«", "ﬂ·", "Êﬂ·", "»Ì‰Â„", "»Ì‰",
            "»Ì‰„«", "»Ì‰Â‰", "Ê»Ì‰„«", "Ê»Ì‰", "»Ì‰‰«", "Ê»Ì‰Â„", "Ê«Ì÷«",
            "Ê«‰", "Ê«·Ï", "Ê›Ì", "Ê«–«", "·–«", "Ê·–«", "»ÕÌÀ", "ÊÂ·", "Ê„‰Â«",
            "»„«", "Ê»„«", "”Ê›", "·”Ê›", "Ê·”Ê›", "Ê”Ê›", "√À‰«¡", "Ê√À‰«¡", 
            "«·· Ì", "Ê«·· Ì", "»«·· Ì", "›«·· Ì", "›ÌÂ„", "" };
    

    public TextAnalysis()
    {
        //load_stopwords_from_a_file();

        for(String stopword : stopwords_array)
            if (!stopwords_dic.containsKey(normalization(stopword,0)))
                stopwords_dic.put(normalization(stopword, 0), true);
    }

 
        public int getDocType()
        {return doc_type;}
        public void setDocType(int docType)
        {doc_type = docType; }

        public int getAnalysisType()
        {return analysis_type;}
        public void setAnalysisType(int analysisType)
        {analysis_type = analysisType;}
        
        public void text_processing(String text, ArrayList<String> terms, boolean stopword_removal)
        {
            if (doc_type == 1) //Plain Text
                processing_plain_text(text, terms, stopword_removal);
            else if (doc_type == 2) //Web page
                processing_web_page(text, terms, stopword_removal);
            else if (doc_type == 3) //Trec document
                processing_TREC_document(text, terms, stopword_removal);
            else if (doc_type == 4) //PDF
                return;

            while (terms.remove("")) ;
        }
        
        void processing_plain_text(String text, ArrayList<String> terms, boolean stopword_removal)
        {
        	ArrayList<String> temp = new ArrayList<String>();
            temp.addAll(Arrays.asList(tokenize(text)));

            for(int i=0; i< temp.size(); i++)
                temp.add(text_processing(temp.get(i),stopword_removal));

            terms = temp;
        }
       
        void processing_web_page(String html, ArrayList<String> terms, boolean stopword_removal)
        {
            //Extracting certain Informaion from tags (such as meta and title tags)
            html = add_meta_keywords_and_description(html);
            
            //remove tags arbitrary no information are kept about tags (no title inforamtion such as information
            //that kept by <h1> tags and <p> tags.
            //you may check "knoweldege extraction from web pages" topic it could help.
            html = remove_unwanted_tags(html);

            //ArrayList<String> tokens = new ArrayList<String>();
            terms.addAll(Arrays.asList(tokenize(html)));

            for (int i = 0; i < terms.size(); i++)
            	terms.set(i, text_processing(terms.get(i), stopword_removal));

            //terms = tokens;
        }
        
        void processing_TREC_document(String text, ArrayList<String> terms, boolean stopword_removal)
        {
        	ArrayList<String> temp = new ArrayList<String>();
            

            temp.addAll(Arrays.asList(text.split("\r\n")));

            for (int i = 0; i < temp.size(); i++)
                temp.add(text_processing(temp.get(i), stopword_removal));

            terms = temp;
        }

        String add_meta_keywords_and_description(String html)
        {
        	Pattern metaPattern = Pattern.compile("<meta.*?>", Pattern.CASE_INSENSITIVE);
        	Matcher metaMatcher = metaPattern.matcher(html);
           
            //char[] sep = { '\"' };
            //char c = '\u0022';
        	String content = "";
        	while(metaMatcher.find())
            {
        		String str = metaMatcher.group();        		
                if (str.matches(".*name=\"keywords\".*"))
                {                	
                    if (str.matches("(.*)(content=\")([^\"]*)(\".*)"))
                    {
                    	content = str.replaceAll("(.*)(content=\")([^\"]*)(\".*)", "$3");
                       	html = html.replace(str, content);
                    }
                    //return str.Substring(str.IndexOf("charset=") + 8, str.LastIndexOf("\"") - str.IndexOf("charset=") - 8);
                }
                else if (str.matches(".*name=\"description\".*"))
                {                    
                    if (str.matches("(.*)(content=\")([^\"]*)(\".*)"))
                    {
                    	content = str.replaceAll("(.*)(content=\")([^\"]*)(\".*)", "$3");
                    	html = html.replace(str, content);
                    }
                }
            }
            return html;
        }

        String remove_unwanted_tags(String html)
        {
            //removing script tags including the text between start and end tags
            html = html.replace("<script.*?</script>", " ");

            //removing tyle tags including the text between start and end tags
            html = html.replace("<style.*?</style>", " ");

            //removing <noscript> including the text between start and end tags
            html = html.replace("<noscript>.*?</noscript>", " ");

            //code = Regex.Replace(code, @"<!--.*?-->", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            html = html.replace("<[^<>]+>", " ");

            //removing character entities
            html = html.replace("&\\S+?;", " ");
            return html;
        }

        public String text_processing(String term, boolean stopword_removal)
        {
            if (analysis_type == 1)
            {
                term = normalization(term, 0);
                if (stopword_removal && stopwords_dic.containsKey(term))
                    return "";
                else return term;
            }
            else if (analysis_type == 2)
            {
                term = normalization(term, 0);
                if (stopword_removal && stopwords_dic.containsKey(term))
                    return "";
                else return simple_stemmer(term);
            }
            else if (analysis_type == 3)
            {
                term = normalization(term, 0);
                if (stopword_removal && stopwords_dic.containsKey(term))
                    return "";
                else return light10_stemmer(term);
            }
            else if (analysis_type == 4)
            {
                //term = normalization(term, 0);
                //if (stopword_removal && stopwords_dic.containsKey(term))
                 //   return wikipedia_stemmer(term);
                 return "";
            }
                //for stopwords removal
            else if (analysis_type == 5)
            {
                if (stopword_removal && stopwords_dic.containsKey(term))
                    return "";
                else return term;
            }
            else if (analysis_type == 6)
            {
                term = normalization(term, 0);
                if (stopword_removal && stopwords_dic.containsKey(term))
                    {
                    StringBuilder temp = new StringBuilder(150);

                    String[] tokens = tokenize(term);
                    for (String token : tokens)
                    {
                        temp.append(light10_stemmer(token)+" ");
                    }
                    return temp.toString().trim();
                }else return "";
            }
            else
            {
                return term;
            }
        }

        public String[] tokenize(String document)
        {
            //return document.split("[^\\w¯ÛıÒˆÚ˙]+");
        	return document.split("[^\\p{L}\\p{M}*]+");
        }

        String normalization(String term, int normalizer_id)
        {
            if (normalizer_id == 0)
                return unified_normalization(term);
            else return term;
        }
        
        String unified_normalization(String word)
        {
            if (word.length()== 1)
                return "";
            
            String term = word.replaceAll("\\p{M}", "");
            term = term.replace("[\r\n]+", "");
            term = term.replace(ALEF_MAD, ALEF_BEDON_HAMZA);
            term = term.replace(ALEF_HAMZA_FOU2, ALEF_BEDON_HAMZA);
            term = term.replace(ALEF_HAMZA_TA7T, ALEF_BEDON_HAMZA);

            term = term.replace(ALEF_MAKSOURA, YA2);
            term = term.replace(TA2_MARBOUTA, HA2);

            term = term.replace("‹", "");

            return term;
        }
       
        public boolean is_stopword(String word)
        {
            return stopwords_dic.containsKey(word);
        }
        
        String simple_stemmer(String term)
        {
            if (term.startsWith("Ê«·") && term.length() > 5)
                term = term.substring(3);
            if (term.startsWith("»«·") && term.length() > 5)
                term = term.substring(3);

            if (term.startsWith("«·") && term.length() > 4)
                term = term.substring(2);
            if (term.startsWith("··") && term.length() > 4)
                term = term.substring(2);

            return term;
        }
        
        public String light10_stemmer(String word)
        {
            if (word.length() > 3 && word.startsWith("Ê"))
            {
                word = word.substring(1);
            }

            int len = 0;
            int wordlen = word.length();

            for (String article : Larkey_defarticles)
            {
                if (wordlen > (len = article.length()) + 1 && word.startsWith(article))
                {
                    word = word.substring(len);
                    break;
                }
            }

            if (word.length() > 2)
            {
                int suflen;
                wordlen = word.length();

                if (wordlen == 0)
                    return null;

                for (String suffix : Larkey_suffixes)
                {
                    if (wordlen > (suflen = suffix.length()) + 1 && word.endsWith(suffix))
                    {
                        word = word.substring(0, wordlen - suflen-1);
                        wordlen = word.length();
                    }
                }
            }
            return word;
        }
   
}
