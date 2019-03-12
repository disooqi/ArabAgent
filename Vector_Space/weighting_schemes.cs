using System;
using System.Collections.Generic;
using System.Text;


namespace Vector_Space
{
    public partial class weighting_schemes
    {
        static int total_document_count;
        static int weight_scheme;

        public weighting_schemes(int doc_count)
        {
            weight_scheme = 1; //default
            total_document_count = doc_count;
        }

        public weighting_schemes(int doc_count, int scheme)
        {
            weight_scheme = scheme;
            total_document_count = doc_count;
        }

        public double get_weight(int term_count, int overall_term_counts, int document_frequency)
        {

            if (weight_scheme == 0)
            {
                //MessageBox.Show("Weight Scheme: Word Count");
                return term_count;
            }
            else if (weight_scheme == 1)
            {
                //MessageBox.Show("Weight Scheme: tf");
                return tf(term_count, overall_term_counts);
            }
            else if (weight_scheme == 2)
            {
                //MessageBox.Show("Weight Scheme: tf-idf");
                return tfidf(term_count, overall_term_counts, document_frequency);
            }
            else
                return 0;
        }
        public double get_weight(int term_count, int overall_term_counts)
        {
            if (weight_scheme == 0)
                return term_count;
            else if (weight_scheme == 1)
                return tf(term_count, overall_term_counts);
            else
                return 0;
        }
        public double get_weight(int term_count)
        {
            if (weight_scheme == 0)
                return term_count;
            else
                return 0;
        }
        
        public void word_count()
        {
        }

        public double tf(double term_count, double overall_term_counts)
        {
            return term_count/overall_term_counts;
        }

        public double tfidf(int term_count, int overall_term_counts, int document_frequency)
        {
            return tf(term_count, overall_term_counts) * Math.Log(total_document_count / (1+document_frequency));
        }

        public void binary()
        { }

    }
}
