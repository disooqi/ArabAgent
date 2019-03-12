package arabagent;

import java.util.*;

public class TermDetection {

	TermDisambiguation CD_obj = new TermDisambiguation();
    Wrapper cDhObject = new Wrapper();
    TextAnalysis txt_analysis_obj = new TextAnalysis();
    String infile_path, outfile_path;
    //int nGram_size;

    public TermDetection(String infile, String outfile)
    {
        infile_path = infile;
        outfile_path = outfile;
    }

    public TermDetection()
    { }

    //public void wikify( LinkedHashMap<Integer, nGram> pos_ngram_dic, ref Queue<String> tokens)
    //{
    //    detect_links(5, ref tokens, ref pos_ngram_dic);
    //    //removing_ngrams_below_certain_threshold(0.1, ref pos_struct_dic);
    //    CD_obj.disambiguate_terms(ref pos_ngram_dic);
    //}

    public void wikify(String docTxt, LinkedHashMap<Integer, nGram> pos_struct_dic)
    {
        //dont forget to make the default text analysis to web not trec as its now
        Queue<String> terms_queue = new LinkedList<String>();
        ArrayList<String> terms_list = new ArrayList<String>();

        txt_analysis_obj.setDocType(2);
        txt_analysis_obj.setAnalysisType(1);
        txt_analysis_obj.text_processing(docTxt, terms_list, false);
        for (String term : terms_list)
            terms_queue.add(term);

        detect_links(7, terms_queue, pos_struct_dic);

        remove_stop_words( pos_struct_dic);

        //removing_ngrams_below_certain_threshold(0.1, ref pos_struct_dic);
        CD_obj.disambiguate_terms(pos_struct_dic);
    }

    void detect_links(int n, Queue<String> tokens, LinkedHashMap<Integer, nGram> pos_struct_dic)
    {
        Wrapper cDhObject = new Wrapper();

        ArrayList<String> n_tokens_list = new ArrayList<String>();
        StringBuilder sb = new StringBuilder(1000);
        int count = 0;
        int position = 0;

        if (tokens.size() > 0)
        {
            while (count != n && tokens.size() > 0)
            {
                n_tokens_list.add(tokens.poll());
                count++;
            }
            int numberOfTokensToBeRemovedFromTheBeginning = 0;
            while (n_tokens_list.size() > 0)
            {
            	numberOfTokensToBeRemovedFromTheBeginning = n_tokens_list.size();
                String temp_str = "";

                for (int x = 0; x < n_tokens_list.size(); x++)
                {
                    for (int y = 0; y < n_tokens_list.size() - x; y++)
                    {
                        sb.append(" " + n_tokens_list.get(y));
                    }

                    temp_str = sb.toString().trim();
                    sb.delete(0, sb.length());


                    ArrayList<String> concepts = new ArrayList<String>();
                    cDhObject.get_term_concepts_list(temp_str, concepts);

                    if (concepts.size() == 0 && !txt_analysis_obj.is_stopword(temp_str) && numberOfTokensToBeRemovedFromTheBeginning ==1)
                    {
                        temp_str = txt_analysis_obj.light10_stemmer(temp_str);
                        cDhObject.get_term_concepts_list(temp_str, concepts);
                    }

                    //if the n-gram has a corresponding concepts add as candidate link
                    if (concepts.size() > 0)
                    {
                        //n_tokens_list.Count-x
                        nGram ncf_struct = new nGram();
                        ncf_struct.ngram = temp_str;
                        ncf_struct.concepts = concepts;
                        //if (concepts.Count == 1)
                        //{
                        //    ncf_struct.is_context_term = true;
                        //    ncf_struct.concept = concepts[0];
                        //}

                        pos_struct_dic.put(position++, ncf_struct);

                        for (int z = 0; z < numberOfTokensToBeRemovedFromTheBeginning; z++)
                        {
                            n_tokens_list.remove(0);
                            if (tokens.size() > 0)
                                n_tokens_list.add(tokens.poll());
                        }

                        break;
                    }
                    //remove n_tokens_list.Count-x from the list
                    numberOfTokensToBeRemovedFromTheBeginning--;
                }

                if (numberOfTokensToBeRemovedFromTheBeginning == 0)
                {
                    nGram ncf_struct = new nGram();
                    ncf_struct.ngram = temp_str;
                    //ncf_struct.ngram = txt_analysis_obj.light10_stemmer(n_tokens_list[0]);
                    //ncf_struct.ngram = n_tokens_list[0];
                    pos_struct_dic.put(position++, ncf_struct);

                    n_tokens_list.remove(0);
                    if (tokens.size() > 0)
                        n_tokens_list.add(tokens.poll());
                }
            }
        }
    }

    //String removing_ngrams_below_certain_threshold(String ngram_file_path, double threshold)
    //{
    //    String correct_ngram_file_path = @"c:\correct_ngram_file_path.txt";
    //    //text_analysis arab_stem = new text_analysis();
    //    using (StreamReader sr = new StreamReader(ngram_file_path, Encoding.GetEncoding("windows-1256")))
    //    {
    //        using (StreamWriter sw = new StreamWriter(correct_ngram_file_path, false, Encoding.GetEncoding("windows-1256")))
    //        {
    //            String term = String.Empty;
    //            while ((term = sr.ReadLine()) != null)
    //            {
    //                term = text_analysis.normalization(term, 0);
    //                if (!arab_stem.is_stopword(term) && ngram_probability(term) >= threshold)
    //                    sw.WriteLine(arab_stem.stemming(term, 0));
    //            }
    //        }
    //    }
    //    return correct_ngram_file_path;
    //}
    void removing_ngrams_below_certain_threshold(double threshold, LinkedHashMap<String, nGram> ngram_struct_dic)
    {
        for (Map.Entry<String, nGram> kvp : ngram_struct_dic.entrySet())
        {
            if (ngram_probability(kvp.getKey()) < threshold)
                ngram_struct_dic.remove(kvp.getKey());
        }
    }

    void remove_stop_words(LinkedHashMap<Integer, nGram> pos_struct_dic)
    {    	
    	Integer[] pos_list = pos_struct_dic.keySet().toArray(new Integer[pos_struct_dic.size()]);
    	
        for(int i=0; i< pos_list.length; i++)
        {
            if (txt_analysis_obj.is_stopword(pos_struct_dic.get(pos_list[i]).ngram))
            {
                pos_struct_dic.remove(pos_list[i]); 
            }
        }
    }

    double ngram_probability(String ngram)
    {
        return 0.8;
    }

}
