using System;
using System.Collections.Generic;
using System.Text;

namespace Vector_Space
{
    public partial class Similarity
    {
        public double Jaccard_similarity_coefficient() //in the case of binary attributes
        {
            return 0;
        }
        public double Cosine_Similarity(Feature_vector v1, Feature_vector v2)
        {
            return Calculate_Dot_Product_of_two_Vector(v1, v2) /
            (Calculate_Magnitude_of_Vector(v1) *
            Calculate_Magnitude_of_Vector(v2));
        }

        double Calculate_Dot_Product_of_two_Vector(Feature_vector v1, Feature_vector v2)
        {
            double count = 0.0;
            for (int i = 0; i < v1.attribute_count; i++)
            {
                count += v1.Vector_Values[i] * v2.Vector_Values[i];
            }
            return count;
        }
        double Calculate_Magnitude_of_Vector(Feature_vector v)
        {
            double count = 0.0;
            for (int i = 0; i < v.attribute_count; i++)
            {
                count += Math.Pow(v.Vector_Values[i], 2);
            }
            return Math.Sqrt(count);
        }
    }
}
