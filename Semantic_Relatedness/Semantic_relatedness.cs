using System;
using System.Collections.Generic;
using System.Text;
using Vector_Space;
using Data_Access_Wrapper;

namespace nsSemantic_Relatedness
{
    public partial class Semantic_relatedness
    {
        const double concepts_count = 140430;
        string c1_id = string.Empty, c2_id = string.Empty;
        Wrapper cDhObject = new Wrapper();


        static int relatedness_measure = 1; //default

        public int Relatedness_measure
        {
            set { relatedness_measure = value; }
            get { return relatedness_measure; }
        }
        /*
        2- Judge the similarity between the two senses' representative articles. Two measures:
              a- One is based on the links extending out of each article,
                  The first measure is defined by the angle between the vectors of the links found within the two 
                  articles of interest. These are almost identical to the TF×IDF vectors used extensively within 
                  information retrieval. The only difference is that we use link counts weighted by the probability 
                  of each link occurring, instead of term counts weighted by the probability of the term occurring.
        */

        public string concept_1
        {
            get { return c1_id; }
            set { c1_id = value; }
        }

        public string concept_2
        {
            get { return c2_id; }
            set { c2_id = value; }
        }

        public double calculate_semantic_relatedness()
        {
            if (c1_id == c2_id)
                return 1;

            if (c1_id == null || c2_id == null)
                return 0;

            if (relatedness_measure == 0)
                return tf_idf_relatedness_measure(c1_id, c2_id);
            else if (relatedness_measure == 1)
                return google_distance_relatedness_Measure(c1_id, c2_id);
            else if (relatedness_measure == 2)
                return tf_idf_and_google_distance_combination_relatedness_Measure(c1_id, c2_id);
            else return -1;
        }

        private double tf_idf_relatedness_measure(string c1_id, string c2_id)
        {
            /*
            The measure is defined by the angle between the vectors of the links found within the two 
            articles of interest. These are almost identical to the TF×IDF vectors used extensively within 
            information retrieval. The only difference is that we use link counts weighted by the probability 
            of each link occurring, instead of term counts weighted by the probability of the term occurring. 
            This probability is defined by the total number of links to the target article over the total number 
            of articles. Thus if s and t are the source and target articles, rispectively, then the weight w of 
            the link (s->t) is:
            */
            Feature_vector c1_v = new Feature_vector();
            Feature_vector c2_v = new Feature_vector();
            c1_v.Vector_Terms = c2_v.Vector_Terms;

            cDhObject.create_Feature_Vector(ref c1_v, ref c2_v, c1_id, c2_id);

            for (int i = 0; i < c1_v.Vector_Terms.Count; i++)
            {
                string term = c1_v.Vector_Terms[i];
                c1_v.Vector_Values[i] = c1_v.Vector_Values[i] * link_weight_calculator(c1_id, term);
                c2_v.Vector_Values[i] = c2_v.Vector_Values[i] * link_weight_calculator(c2_id, term);
            }

            return (new Similarity()).Cosine_Similarity(c1_v, c2_v);
        }
        /*
              b- the other on the links made to them.
        */
        //Wikipedia Link-based Measure (WLM)
        private double google_distance_relatedness_Measure(string c1_id, string c2_id)
        {
            int common_count = cDhObject.retrieve_common_inlink_count(c1_id, c2_id);

            if (common_count != 0)
            {
                double A = cDhObject.get_inLinks_count(c1_id),
                       B = cDhObject.get_inLinks_count(c2_id);

                return 1 - ((Math.Log(Math.Max(A, B)) - Math.Log(common_count)) /
                            (Math.Log(concepts_count) - Math.Log(Math.Min(A, B))));

                //return (Math.Log10(common_count) / Math.Log10(A + B - common_count));
                //return common_count / (A + B - common_count);
            }
            else return 0;
        }

        private double tf_idf_and_google_distance_combination_relatedness_Measure(string c1_id, string c2_id)
        {
            return (tf_idf_relatedness_measure(c1_id, c2_id) + google_distance_relatedness_Measure(c1_id, c2_id)) / 2;
        }

        private double link_weight_calculator(string source, string target)
        {
            //if (cDhObject.isLinkExist(source, target))
            //{
            return Math.Log10(concepts_count / cDhObject.get_inLinks_count(target));
            //}
            //else
            //{
            //    return 0;
            //}
        }
    }
}
