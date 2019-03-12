using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using nsText_Analysis;
using Data_Access_Wrapper;
using nsWord_Sense_Disambiguation;
using dosDataStructure;

namespace nsPhrase_Detection
{
    public partial class PhrasesDetector
    {
        PhraseDisambiguation CD_obj = new PhraseDisambiguation();
        Wrapper cDhObject = new Wrapper();
        text_analysis txt_analysis_obj = new text_analysis();
        string infile_path, outfile_path;
        //int n_gram_size;

        public PhrasesDetector(string infile, string outfile)
        {
            infile_path = infile;
            outfile_path = outfile;
        }

        public PhrasesDetector()
        { }

        //public void wikify(ref Dictionary<int, n_gram> pos_ngram_dic, ref Queue<string> tokens)
        //{
        //    detect_links(5, ref tokens, ref pos_ngram_dic);
        //    //removing_ngrams_below_certain_threshold(0.1, ref pos_struct_dic);
        //    CD_obj.disambiguate_terms(ref pos_ngram_dic);
        //}

        public void wikify(string docTxt, ref Dictionary<int, n_gram> pos_struct_dic)
        {
            //dont forget to make the default text analysis to web not trec as its now
            Queue<string> terms_queue = new Queue<string>();
            List<string> terms_list = new List<string>();

            txt_analysis_obj.Doc_type = 2;
            txt_analysis_obj.Analysis_type = 1;
            txt_analysis_obj.text_processing(docTxt, ref terms_list, false);
            foreach (string term in terms_list)
                terms_queue.Enqueue(term);

            detect_links(7, ref terms_queue, ref pos_struct_dic);

            remove_stop_words(ref pos_struct_dic);

            //removing_ngrams_below_certain_threshold(0.1, ref pos_struct_dic);
            CD_obj.disambiguate_terms(ref pos_struct_dic);
        }

        void detect_links(int n, ref Queue<string> tokens, ref Dictionary<int, n_gram> pos_struct_dic)
        {
            Wrapper cDhObject = new Wrapper();

            List<string> n_tokens_list = new List<string>();
            StringBuilder sb = new StringBuilder(1000);
            int count = 0;
            int position = 0;

            if (tokens.Count > 0)
            {
                while (count != n && tokens.Count > 0)
                {
                    n_tokens_list.Add(tokens.Dequeue());
                    count++;
                }
                int number_of_removed_terms_from_the_beginning_of_the_list = 0;
                while (n_tokens_list.Count > 0)
                {
                    number_of_removed_terms_from_the_beginning_of_the_list = n_tokens_list.Count;
                    string temp_str = "";

                    for (int x = 0; x < n_tokens_list.Count; x++)
                    {
                        for (int y = 0; y < n_tokens_list.Count - x; y++)
                        {
                            sb.Append(" " + n_tokens_list[y]);
                        }

                        temp_str = sb.ToString().Trim();
                        sb.Remove(0, sb.Length);


                        List<string> concepts = new List<string>();
                        cDhObject.get_term_concepts_list(temp_str, ref concepts);

                        // && number_of_removed_terms_from_the_beginning_of_the_list == 1
                        if (concepts.Count == 0 && !txt_analysis_obj.is_stopword(temp_str))
                        {
                            temp_str = txt_analysis_obj.light10_stemmer(temp_str);
                            cDhObject.get_term_concepts_list(temp_str, ref concepts);
                        }

                        //if the n-gram has a corresponding concepts add as candidate link
                        if (concepts.Count > 0)
                        {
                            //n_tokens_list.Count-x
                            n_gram ncf_struct = new n_gram();
                            ncf_struct.ngram = temp_str;
                            ncf_struct.concepts = concepts;
                            //if (concepts.Count == 1)
                            //{
                            //    ncf_struct.is_context_term = true;
                            //    ncf_struct.concept = concepts[0];
                            //}

                            pos_struct_dic.Add(position++, ncf_struct);

                            for (int z = 0; z < number_of_removed_terms_from_the_beginning_of_the_list; z++)
                            {
                                n_tokens_list.RemoveAt(0);
                                if (tokens.Count > 0)
                                    n_tokens_list.Add(tokens.Dequeue());
                            }

                            break;
                        }
                        //remove n_tokens_list.Count-x from the list
                        number_of_removed_terms_from_the_beginning_of_the_list--;
                    }

                    if (number_of_removed_terms_from_the_beginning_of_the_list == 0)
                    {
                        n_gram ncf_struct = new n_gram();
                        ncf_struct.ngram = temp_str;
                        //ncf_struct.ngram = txt_analysis_obj.light10_stemmer(n_tokens_list[0]);
                        //ncf_struct.ngram = n_tokens_list[0];
                        pos_struct_dic.Add(position++, ncf_struct);

                        n_tokens_list.RemoveAt(0);
                        if (tokens.Count > 0)
                            n_tokens_list.Add(tokens.Dequeue());
                    }
                }
            }
        }

        //string removing_ngrams_below_certain_threshold(string ngram_file_path, double threshold)
        //{
        //    string correct_ngram_file_path = @"c:\correct_ngram_file_path.txt";
        //    //text_analysis arab_stem = new text_analysis();
        //    using (StreamReader sr = new StreamReader(ngram_file_path, Encoding.GetEncoding("windows-1256")))
        //    {
        //        using (StreamWriter sw = new StreamWriter(correct_ngram_file_path, false, Encoding.GetEncoding("windows-1256")))
        //        {
        //            string term = string.Empty;
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
        void removing_ngrams_below_certain_threshold(double threshold, ref Dictionary<string, n_gram> ngram_struct_dic)
        {
            foreach (KeyValuePair<string, n_gram> kvp in ngram_struct_dic)
            {
                if (ngram_probability(kvp.Key) < threshold)
                    ngram_struct_dic.Remove(kvp.Key);
            }
        }

        void remove_stop_words(ref Dictionary<int, n_gram> pos_struct_dic)
        {
            List<int> pos_list = new List<int>(pos_struct_dic.Keys);

            for (int i = 0; i < pos_list.Count; i++)
            {
                if (txt_analysis_obj.is_stopword(pos_struct_dic[pos_list[i]].ngram))
                {
                    pos_struct_dic.Remove(pos_list[i]); 
                }
            }
        }

        double ngram_probability(string ngram)
        {
            return 0.8;
        }

    }
}
