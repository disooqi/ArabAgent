using System;
using System.Collections.Generic;
using System.Text;

namespace Vector_Space
{
    public partial class Feature_vector
    {
        List<string> attributes_terms = new List<string>();
        List<double> attributes_values = new List<double>();

        public int attribute_count
        {
            get
            { return attributes_terms.Count; }
        }

        public List<double> Vector_Values
        {
            get { return attributes_values; }
            set { attributes_values = value; }
        }

        public List<string> Vector_Terms
        {
            get { return attributes_terms; }
            set { attributes_terms = value; }
        }

        public int Term_Index(string term)
        {
            return attributes_terms.IndexOf(term);
        }

        public bool is_Term_Exist(string term)
        {
            return attributes_terms.Contains(term);
        }

        //public static Feature_vector add_two_vectors(Feature_vector v1, Feature_vector v2)
        //{
        //    Feature_vector temp = new Feature_vector();

        //    for (int i = 0; i < v1.attribute_count; i++)
        //    {
        //        temp.add_term(Feature_vector.terms[i], v1.values[i]);
        //    }

        //    for (int i = 0; i < v2.attribute_count; i++)
        //    {
        //        if (!temp.is_Term_Exist(Feature_vector.terms[i]))
        //            temp.add_term(Feature_vector.terms[i], v2.values[i]);
        //        else
        //            temp.values[temp.Term_Index(Feature_vector.terms[i])] += v2.Vector_Values[i];
        //    }
        //    return temp;
        //}

        //public static Feature_vector multiply_a_vector_by_a_number(Feature_vector v, double num)
        //{
        //    Feature_vector temp = new Feature_vector();
        //    for (int i = 0; i < v.attribute_count; i++)
        //    {
        //        temp.add_term(Feature_vector.terms[i], v.values[i] * num);
        //    }
        //    return temp;
        //}
    }
}
