using System;
using System.Collections.Generic;
using System.Text;

namespace dosDataStructure
{
    public struct n_gram
    {
        public string ngram;
        public List<string> concepts;
        public string concept;
        //public int freq;
        public bool is_context_term;
        public double weight;
    }
}
