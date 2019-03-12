package arabagent;

public class Similarity {
    public double jaccardSimilarityCoefficient() //in the case of binary attributes
    {
        return 0;
    }
    public double cosineSimilarity(FeatureVector v1, FeatureVector v2)
    {
        return Calculate_Dot_Product_of_two_Vector(v1, v2) /
        (Calculate_Magnitude_of_Vector(v1) *
        Calculate_Magnitude_of_Vector(v2));
    }

    double Calculate_Dot_Product_of_two_Vector(FeatureVector v1, FeatureVector v2)
    {
        double count = 0.0;
        for (int i = 0; i < v1.getAttributeCount(); i++)
        {
            count += v1.getVectorValues().indexOf(i) * v2.getVectorValues().indexOf(i);
        }
        return count;
    }
    double Calculate_Magnitude_of_Vector(FeatureVector v)
    {
        double count = 0.0;
        for (int i = 0; i < v.getAttributeCount(); i++)
        {
            count += Math.pow(v.getVectorValues().indexOf(i), 2);
        }
        return Math.sqrt(count);
    }
}
