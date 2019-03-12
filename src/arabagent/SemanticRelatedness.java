package arabagent;

public class SemanticRelatedness {

    int concepts_count = 140430;
    String c1_id = "", c2_id = "";
    Wrapper cDhObject = new Wrapper();


    static int relatedness_measure = 1; //default

    public void setRelatednessMeasure(int measureCode)
    {
        relatedness_measure = measureCode;    
    }
    
    public int getRelatednessMeasure()
    {return relatedness_measure;}
    
    /*
    2- Judge the similarity between the two senses' representative articles. Two measures:
          a- One is based on the links extending out of each article,
              The first measure is defined by the angle between the vectors of the links found within the two 
              articles of interest. These are almost identical to the TF�IDF vectors used extensively within 
              information retrieval. The only difference is that we use link counts weighted by the probability 
              of each link occurring, instead of term counts weighted by the probability of the term occurring.
    */

    public void setConcept_1(String id)
    {  c1_id = id; }
    public String getConcept_1()
    {return c1_id;}
    
    public void setConcept_2(String id)
    {  c2_id = id; }
    public String getConcept_2()
    {return c2_id;}

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

    private double tf_idf_relatedness_measure(String c1_id, String c2_id)
    {
        /*
        The measure is defined by the angle between the vectors of the links found within the two 
        articles of interest. These are almost identical to the TF�IDF vectors used extensively within 
        information retrieval. The only difference is that we use link counts weighted by the probability 
        of each link occurring, instead of term counts weighted by the probability of the term occurring. 
        This probability is defined by the total number of links to the target article over the total number 
        of articles. Thus if s and t are the source and target articles, rispectively, then the weight w of 
        the link (s->t) is:
        */
        FeatureVector c1_v = new FeatureVector();
        FeatureVector c2_v = new FeatureVector();
        //c1_v.getVectorTerms(). = c2_v.VectorTerms;

        cDhObject.create_Feature_Vector(c1_v, c2_v, c1_id, c2_id);

        for (int i = 0; i < c1_v.getVectorTerms().size(); i++)
        {
            String term = c1_v.getVectorTerms().get(i);
            c1_v.getVectorValues().set(i, c1_v.getVectorValues().get(i) * link_weight_calculator(c1_id, term));
            c2_v.getVectorValues().set(i, c2_v.getVectorValues().get(i) * link_weight_calculator(c2_id, term));
        }

        return (new Similarity()).cosineSimilarity(c1_v, c2_v);
    }
    /*
          b- the other on the links made to them.
    */
    //Wikipedia Link-based Measure (WLM)
    private double google_distance_relatedness_Measure(String c1_id, String c2_id)
    {
        int common_count = cDhObject.retrieve_common_inlink_count(c1_id, c2_id);

        if (common_count != 0)
        {
            double A = cDhObject.get_inLinks_count(c1_id),
                   B = cDhObject.get_inLinks_count(c2_id);

            return 1 - ((Math.log(Math.max(A, B)) - Math.log(common_count)) /
                        (Math.log(concepts_count) - Math.log(Math.min(A, B))));

            //return (Math.Log10(common_count) / Math.Log10(A + B - common_count));
            //return common_count / (A + B - common_count);
        }
        else return 0;
    }

    private double tf_idf_and_google_distance_combination_relatedness_Measure(String c1_id, String c2_id)
    {
        return (tf_idf_relatedness_measure(c1_id, c2_id) + google_distance_relatedness_Measure(c1_id, c2_id)) / 2;
    }

    private double link_weight_calculator(String source, String target)
    {
        return Math.log10(concepts_count / cDhObject.get_inLinks_count(target));
    }
}
