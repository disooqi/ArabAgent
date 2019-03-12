using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using nsSemantic_Relatedness;
using Data_Access_Wrapper;
using dosDataStructure;

namespace nsWord_Sense_Disambiguation
{
    public partial class PhraseDisambiguation
    {
        Dictionary<string, double> relatedness_cache = new Dictionary<string, double>();

        Wrapper cDhObject;
        Semantic_relatedness SRCObj;
        //double context_quality;
        /*
         * most_common_sense            0
         * most_closely_related_pair    1
         */
        static int disambiguation_technique = 1; //default
        static double sensible_probability_threshold = 0.02; //default


        Dictionary<string, List<double>> context_sense_relatedness_table;
        Dictionary<string, int> concept_index_dic;

        public PhraseDisambiguation()
        {
            cDhObject = new Wrapper();
            SRCObj = new Semantic_relatedness();
        }

        public int Disambiguation_technique
        {
            set { disambiguation_technique = value; }
            get { return disambiguation_technique; }
        }

        public double Sensible_probability_threshold
        {
            set { sensible_probability_threshold = value; }
            get { return sensible_probability_threshold; }
        }

        public void disambiguate_concepts(string term1, string term2, out string c1, out string c2,int disambiguator_id)
        {
            if(disambiguator_id == 0)
                most_common_sense(term1, term2, out c1, out c2);
            else if (disambiguator_id == 1)
                most_closely_related_pair(term1, term2, out c1, out c2);
            //else if(disambiguator_id == 2)
            //    commonness_and_relatedness(term1, term2, out c1, out c2);
            //else if (disambiguator_id == 3)
            //    sequential_decision(term1, term2, out c1, out c2);
            else { c1 = "wrong disambiguator Index"; c2 = "wrong disambiguator Index"; }
        }

        public void disambiguate_concepts(ref Dictionary<int, n_gram> pos_ngram_dic, int disambiguator_id)
        {
            if (disambiguator_id == 0)
            {
                //MessageBox.Show("Weight Scheme: Commoness");
                most_common_sense(ref pos_ngram_dic);
            }
            else if (disambiguator_id == 1)
            {
                //MessageBox.Show("Weight Scheme: Relatedness");
                most_closely_related_pairs(ref pos_ngram_dic);
            }
            //else if(disambiguator_id == 2)
            //    commonness_and_relatedness(ref term_concepts);
            //else if (disambiguator_id == 3)
            //    sequential_decision(ref term_concepts);
            //else { c1 = "wrong disambiguator Index"; c2 = "wrong disambiguator Index"; }

        }
        /*
         * PREV STEP: The Previous step before this is identifing the candidate articles (concepts)
        1- THIS STEP:  is to identify the concepts they relate to: (in Wikipedia’s case, the articles which discuss them.)
         * NEXT STEP: measuring the relatedness between two concepts
        */
        //there are several options:
        //**************************
        //1. one can make a snap decision by using the most common sense of each term.
        //    (HINT: The commonness of a sense is defined by the number of times the term is used to link to it)
        void most_common_sense(string term1, string term2, out string c1, out string c2)
        {
            List<string> concepts_list1 = new List<string>();
            List<string> concepts_list2 = new List<string>();

            cDhObject.get_term_concepts_list(term1, ref concepts_list1);
            cDhObject.get_term_concepts_list(term2, ref concepts_list2);

            double max_for_term1=0, max_for_term2=0;
            c1 = "";
            c2 = "";
            foreach (string concept in concepts_list1)
            {
                if (max_for_term1 < cDhObject.get_inLinks_count(concept))
                {
                    c1 = concept;
                    max_for_term1 = cDhObject.get_inLinks_count(concept);
                }
            }

            foreach (string concept in concepts_list2)
            {
                if (max_for_term2 < cDhObject.get_inLinks_count(concept))
                {
                    c2 = concept;
                    max_for_term2 = cDhObject.get_inLinks_count(concept);
                }
            }
        }
        void most_common_sense(ref Dictionary<int, n_gram> pos_ngram_dic)
        {
            Dictionary<int, n_gram> temp_dic  = new Dictionary<int, n_gram>();

            foreach (KeyValuePair<int, n_gram> kvp in pos_ngram_dic)
            {
                n_gram temp_ngram = kvp.Value;

                if (kvp.Value.concepts != null)
                {
                    double max_for_term = 0;
                    double inlink_count = 0;
                    foreach (string concept in temp_ngram.concepts)
                    {
                        inlink_count = cDhObject.get_inLinks_count(concept);
                        if (max_for_term < inlink_count)
                        {
                            temp_ngram.concept = concept;
                            max_for_term = inlink_count;
                        }
                    }
                    if (temp_ngram.concept != null)
                        temp_dic.Add(kvp.Key, temp_ngram);
                    else { }
                }
                else temp_dic.Add(kvp.Key, temp_ngram);
            }

            pos_ngram_dic = temp_dic;
        }
        //2. Another approach is to use the two terms involved to disambiguate each other (selecting most closely related pair)
        //    (HINT: but is marred by the number of obscure senses available: there may be hundreds for each term. Consequently, for efficiency and accuracy’s sake we only consider articles which receive at least 1% of the anchor’s links.)
        void most_closely_related_pair(string term1, string term2, out string c1, out string c2)
        {
            List<string> concepts_list1 = new List<string>();
            List<string> concepts_list2 = new List<string>();

            cDhObject.get_term_concepts_list(term1, ref concepts_list1);
            cDhObject.get_term_concepts_list(term2, ref concepts_list2);

            double max_relatedness = 0, temp_relatedness;
            c1 = "";
            c2 = "";
            SRCObj.concept_1 = "";
            SRCObj.concept_2 = "";
            foreach (string concept1 in concepts_list1)
            {
                foreach (string concept2 in concepts_list2)
                {
                    SRCObj.concept_1 = concept1;
                    SRCObj.concept_2 = concept2;

                    temp_relatedness = SRCObj.calculate_semantic_relatedness();
                    if (max_relatedness < temp_relatedness)
                    {
                        c1 = concept1;
                        c2 = concept2;
                        max_relatedness = temp_relatedness;
                    }
                }
            }

           
        }

        void most_closely_related_pairs(ref Dictionary<int, n_gram> pos_ngram_dic)
        {
            List<int> positions = new List<int>(pos_ngram_dic.Keys);

            //choosing the context term
            for (int i = 0; i < positions.Count; i++)
            {
                if (pos_ngram_dic[positions[i]].concepts != null && pos_ngram_dic[positions[i]].concepts.Count == 1)
                {
                    n_gram temp_ngram = pos_ngram_dic[positions[i]];
                    temp_ngram.is_context_term = true;
                    temp_ngram.concept = temp_ngram.concepts[0];
                    pos_ngram_dic[positions[i]] = temp_ngram;
                }
            }

            calculate_context_term_weights(ref pos_ngram_dic);


            for (int i = 0; i < positions.Count; i++)
            {
                if (pos_ngram_dic[positions[i]].concepts != null && pos_ngram_dic[positions[i]].concepts.Count > 1)
                {
                    n_gram temp_ngram = pos_ngram_dic[positions[i]];
                    disambiguate_sense(ref temp_ngram, ref pos_ngram_dic);

                    if (temp_ngram.concept != null)
                        pos_ngram_dic[positions[i]] = temp_ngram;
                }
            }
        }

        void disambiguate_sense(ref n_gram ngram, ref Dictionary<int, n_gram> pos_ngram_dic)
        {
            double inlinks_sum = get_ngram_inlinks_count(ngram.concepts);
            double max_average = 0, temp_average;

            foreach (string concept in ngram.concepts)
            {
                if (sensible_probability(concept, inlinks_sum) >= sensible_probability_threshold)
                {
                    temp_average = weighted_average_relatedness(concept, pos_ngram_dic);
                    if (max_average < temp_average)
                    {
                        max_average = temp_average;
                        ngram.concept = concept;
                    }
                }
            }
        }

        void calculate_context_term_weights(ref Dictionary<int, n_gram> pos_ngram_dic)
        {
            List<int> positions = new List<int>(pos_ngram_dic.Keys);

            create_context_senses_relatedness_table(ref pos_ngram_dic, out context_sense_relatedness_table);

            //choosing the context term
            for (int i = 0; i < positions.Count; i++)
            {
                if (pos_ngram_dic[positions[i]].is_context_term)
                {
                    n_gram temp_ngram = pos_ngram_dic[positions[i]];
                    temp_ngram.weight = get_central_thread_relatedness( ref pos_ngram_dic, temp_ngram);
                    pos_ngram_dic[positions[i]] = temp_ngram;
                }
            }
        }

        double get_central_thread_relatedness( ref Dictionary<int, n_gram> context_ngrams, n_gram target_ngram)
        {
            double sum = 0.0;
            int context_ngram_count = 0;
            
            foreach (KeyValuePair<int, n_gram> ngram in context_ngrams)
            {
                if (ngram.Value.is_context_term)
                {
                    sum += get_relatedness_value_from_table(target_ngram.concept, ngram.Value.concept);
                    context_ngram_count++;
                }
            }

            return (sum - 1) / (context_ngram_count - 1);
        }

        double get_relatedness_value_from_table(string c_id_1, string c_id_2)
        {
            if (c_id_1 == c_id_2)
                return 1;
            else if (concept_index_dic[c_id_1] > concept_index_dic[c_id_2])
                return context_sense_relatedness_table[c_id_2][concept_index_dic[c_id_1] - 1 - concept_index_dic[c_id_2]];
            else
                return context_sense_relatedness_table[c_id_1][concept_index_dic[c_id_2] - 1 - concept_index_dic[c_id_1]];
        }

        void create_context_senses_relatedness_table(ref Dictionary<int, n_gram> ngram_dic, out Dictionary<string, List<double>> table)
        {
            Dictionary<string, List<double>> context_concepts = new Dictionary<string, List<double>>();
            concept_index_dic = new Dictionary<string, int>();
            int index = 0;

            foreach (KeyValuePair<int, n_gram> ngram in ngram_dic)
                //get all concepts for context n-grams
                if (ngram.Value.is_context_term)
                    if (!context_concepts.ContainsKey(ngram.Value.concept))
                    {
                        context_concepts.Add(ngram.Value.concept, new List<double>());
                        //
                        concept_index_dic.Add(ngram.Value.concept, index++);
                    }

            List<string> concepts = new List<string>(context_concepts.Keys);

            for (int x = 0; x < concepts.Count; x++)
            {
                SRCObj.concept_1 = concepts[x];
                List<double> temp_list = new List<double>();
                for (int y = x + 1; y < concepts.Count; y++)
                {
                    SRCObj.concept_2 = concepts[y];
                    temp_list.Add(SRCObj.calculate_semantic_relatedness());                  
                }
                context_concepts[concepts[x]] = temp_list;
            }

            table = context_concepts;
        }

        //3. The best results are obtained when both commonness and relatedness are considered. Evenly weighting the candidate senses by these variables and choosing the pair with the highest weight—highest (commonness + relatedness)—gives exactly the same results as with manual disambiguation
        //void commonness_and_relatedness(string term1, string term2, out string c1, out string c2)
        //{ }
        //4. Interestingly we can improve upon this by making a simple "sequential decision", which first groups the most related pairs of candidates (within 40% of the most related pair) and then chooses the most common pair. This makes subtly different choices from those made manually.
        //void sequential_decision(string term1, string term2, out string c1, out string c2)
        //{}
        //5. we can also consider the case where two words are closely related because they belong together in the same phrase: e.g. family planning is a well-known phrase, and consequently family and planning are given a high semantic relatedness by humans even though their respective concepts are relatively disjoint. To identify these cases we simply concatenate the terms and see whether this is used as an anchor. If so, the frequency with which the anchor is used is normalized and added to the relatedness score of the original terms.
        public void disambiguate_terms(ref Dictionary<int, n_gram> pos_struct_dic)
        {
            disambiguate_concepts(ref pos_struct_dic, disambiguation_technique);
        }

        double get_ngram_inlinks_count(List<string> ngram_senses)
        {
            double total_count = 0;

            foreach (string sense in ngram_senses)
            {
                total_count += cDhObject.get_inLinks_count(sense);
            }
            return total_count;
        }

        double sensible_probability(string candidate_sense, List<string> senses)
        {            
            double total_count = 0;
            
            foreach (string sense in senses)
            {
                total_count += cDhObject.get_inLinks_count(sense);
            }
            return cDhObject.get_inLinks_count(candidate_sense) / total_count;
        }

        double sensible_probability(string candidate_sense, double total_count)
        {
            return cDhObject.get_inLinks_count(candidate_sense) / total_count;
        }

        //Average of weighted Relatedness
        public double weighted_average_relatedness(string concept, Dictionary<int, n_gram> concepts_list)
        {
            SRCObj.concept_1 = concept;
            double relatedness_aggregation = 0;
            int context_sense_count = 0;
            StringBuilder cache_key = new StringBuilder(concept, 14);
            int key_len = cache_key.Length;
            foreach (KeyValuePair<int, n_gram> c in concepts_list)
            {
                if (c.Value.is_context_term)
                {
                    cache_key.Remove(key_len, cache_key.Length - key_len);

                    context_sense_count++;
                    SRCObj.concept_2 = c.Value.concept;

                    cache_key.Append(c.Value.concept);

                    if (!relatedness_cache.ContainsKey(cache_key.ToString()))
                        relatedness_cache.Add(cache_key.ToString(), SRCObj.calculate_semantic_relatedness());

                    relatedness_aggregation += c.Value.weight * relatedness_cache[cache_key.ToString()];
                    //relatedness_aggregation += c.Value.weight * calculate_semantic_relatedness();
                }
            }
            return relatedness_aggregation / context_sense_count;
        }
    }
}
