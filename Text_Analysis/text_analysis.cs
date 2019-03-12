using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;


namespace nsText_Analysis
{
    public partial class text_analysis
    {
        Dictionary<string, bool> stopwords_dic = new Dictionary<string, bool>();
        Dictionary<string, bool> wikipedia_terms_dic = new Dictionary<string, bool>();

        const char ALEF_MAKSOURA = (char)1609;
        const char YA2 = (char)1610;
        const char TA2_MARBOUTA = (char)1577;
        const char HA2 = (char)1607;
        const char ALEF_HAMZA_FOU2 = (char)1571;
        const char ALEF_HAMZA_TA7T = (char)1573;
        const char ALEF_MAD = (char)1570;
        const char ALEF_BEDON_HAMZA = (char)1575;
        const char KASHIDA = (char)1600;

        /* doc_type:
         * 0 Word
         * 1 Plain text  // the defualt
         * 2 web document
         * 3 Trec document
         * 4 PDF document

         */
        int doc_type = 1; //default
        int analysis_type = 1; //default if not set
        /* 
         * 0: no analysis; 
         * 1: normalization 
         * 2: simple stemming 
         * 3: Light10 stemming 
         * 4: Wikipedia stemming
         * 5: Stopword Removal
         */

        string[] Larkey_defarticles = { "ال", "وال", "بال", "كال", "فال", "لل" };
        string[] Larkey_suffixes = { "ها", "ان", "ات", "ون", "ين", "يه", "ية", "ه", "ة", "ي" };
        string[] stopwords_array = {
                "ان","بعد", "ضد", "يلي", "الى", "في", "من", "حتى", "وهو", "يكون",
                "به", "وليس", "أحد", "على", "وكان", "تلك", "كذلك", "التي", "وبين",
                "فيها", "عليها", "إن", "وعلى", "لكن", "عن", "مساء", "ليس", "منذ",
                "الذي", "أما", "حين", "ومن", "ليست", "وكانت", "أي", "ما", "عنه",
                "حول", "دون", "مع", "لكنه", "ولكن", "له", "هذا", "والتي","فقط", "ثم",
                "هذه", "أنه", "تكون", "قد", "بين", "جدا", "لن", "نحو", "كان", "لهم",
                "لأن", "اليوم", "لم", "هؤلاء", "فإن", "فيه", "ذلك", "لو", "عند",
                "اللذين", "كل", "بد", "لدى", "وثي", "أن", "ومع", "فقد", "بل", "هو",
                "عنها", "منه", "بها", "وفي", "فهو", "تحت", "لها", "أو", "إذ", "علي",
                "عليه", "كما", "كيف", "هنا", "وقد", "كانت", "لذلك", "أمام", "هناك",
                "قبل", "معه", "يوم", "منها", "إلى", "إذا", "هل", "حيث", "هي", "اذا",
                "او", "و", "لا", "الي", "إلي", "مازال", "لازال", "لايزال",
                "مايزال", "اصبح", "أصبح", "أمسى", "امسى", "أضحى", "اضحى", "ظل",
                "مابرح", "مافتئ", "ماانفك", "بات", "صار", "وليست", "إن", "كأن",
                "ليت", "لعل", "لاسيما", "ولايزال", "الحالي", "ضمن", "اول", "وله",
                "ذات", "اي", "بدلا", "اليها", "انه", "الذين", "فانه", "وان",
                "والذي", "وهذا", "لهذا", "الا", "فكان", "ستكون", "مما", "أبو",
                "بإن", "الذي", "اليه", "يمكن", "بهذا", "لدي", "وأن", "وهي", "وأبو",
                "آل", "الذي", "هن", "الذى", "وكذلك", "وما", "ولهذا", "وقبل", "اما", 
                "بتلك", "بهذه", "بذلك", "لماذا", "ماذا", "لما", "ولما", "ولماذا",
                "وماذا", "ولم", "مثل", "مثلا", "ابن", "بعض", "كلا", "قال", "قالت",
                "لقد", "ولقد", "ولعل", "انهم", "ايضا", "كل", "وكل", "بينهم", "بين",
                "بينما", "بينهن", "وبينما", "وبين", "بيننا", "وبينهم", "وايضا",
                "وان", "والى", "وفي", "واذا", "لذا", "ولذا", "بحيث", "وهل", "ومنها",
                "بما", "وبما", "سوف", "لسوف", "ولسوف", "وسوف", "أثناء", "وأثناء", 
                "اللتي", "واللتي", "باللتي", "فاللتي", "فيهم", "" };

        public text_analysis()
        {
            //load_stopwords_from_a_file();

            foreach (string stopword in stopwords_array)
                if (!stopwords_dic.ContainsKey(normalization(stopword,0)))
                    stopwords_dic.Add(normalization(stopword, 0), true);
        }
        /*
        public static void text_processing(ref List<string> terms)
        {
            Queue<string> temp_queue = new Queue<string>();
            Dictionary<int, n_gram> pos_ngram_dic = new Dictionary<int, n_gram>();
            Link_detector LD_obj = new Link_detector();

            for (int i = 0; i < terms.Count; i++)
            {
                terms[i] = normalization(terms[i], 0);
                if (!stopwords.Contains(terms[i]))
                    temp_queue.Enqueue(terms[i]);
            }

            terms.Clear();

            LD_obj.wikify(ref pos_ngram_dic, ref temp_queue);

            foreach (KeyValuePair<int, n_gram> ngram in pos_ngram_dic)
                if (ngram.Value.concept != null)
                    terms.Add(ngram.Value.concept);
                else terms.Add(ngram.Value.ngram);

        }
        */

        public int Doc_type
        {
            set { doc_type = value; }
            get { return doc_type; }
        }

        public int Analysis_type
        {
            set { analysis_type = value; }
            get { return analysis_type; }
        }

        public void text_processing(string text, ref List<string> terms, bool stopword_removal)
        {
            if (doc_type == 1) //Plain Text
                processing_plain_text(text, out terms, stopword_removal);
            else if (doc_type == 2) //Web page
                processing_web_page(text, out terms, stopword_removal);
            else if (doc_type == 3) //Trec document
                processing_TREC_document(text, out terms, stopword_removal);
            else if (doc_type == 4) //PDF
                return;

            while (terms.Remove("")) ;
        }

        void processing_plain_text(string text, out List<string> terms, bool stopword_removal)
        {
            List<string> temp = new List<string>();
            temp.AddRange(Tokenize(text));

            for(int i=0; i< temp.Count; i++)
                temp[i] = text_processing(temp[i], stopword_removal);

            terms = temp;
        }

        void processing_web_page(string html, out List<string> terms, bool stopword_removal)
        {
            //Extracting certain Informaion from tags (such as meta and title tags)
            html = add_meta_keywords_and_description(html);
            
            //remove tags arbitrary no information are kept about tags (no title inforamtion such as information
            //that kept by <h1> tags and <p> tags.
            //you may check "knoweldege extraction from web pages" topic it could help.
            html = remove_unwanted_tags(html);

            List<string> tokens = new List<string>();
            tokens.AddRange(Tokenize(html));

            for (int i = 0; i < tokens.Count; i++)
                tokens[i] = text_processing(tokens[i], stopword_removal);

            terms = tokens;
        }

        void processing_TREC_document(string text, out List<string> terms, bool stopword_removal)
        {
            List<string> temp = new List<string>();
            string[] sep = { "\r\n" };

            temp.AddRange(text.Split(sep, StringSplitOptions.RemoveEmptyEntries));

            for (int i = 0; i < temp.Count; i++)
                temp[i] = text_processing(temp[i], stopword_removal);

            terms = temp;
        }

        string add_meta_keywords_and_description(string html)
        {
            MatchCollection mc = Regex.Matches(html, @"<meta.*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            char[] sep = { '\"' };
            //char c = '\u0022';
            for (int i = 0; i < mc.Count; i++)
            {
                if (mc[i].Value.Contains("name=\"keywords\""))
                {
                    string str = mc[i].Value;
                    Match m = Regex.Match(str, "content=\"[^\"]*\"");
                    if (!string.IsNullOrEmpty(m.Value))
                        html = html.Replace(str, " " + m.Value.Split(sep)[1] + " ");
                    //return str.Substring(str.IndexOf("charset=") + 8, str.LastIndexOf("\"") - str.IndexOf("charset=") - 8);
                }
                else if (mc[i].Value.Contains("name=\"description\""))
                {
                    string str = mc[i].Value;
                    Match m = Regex.Match(str, "content=\"[^\"]*\"");
                    if (!string.IsNullOrEmpty(m.Value))
                        html = html.Replace(str, " " + m.Value.Split(sep)[1] + " ");
                }
            }
            return html;
        }

        string remove_unwanted_tags(string html)
        {
            //removing script tags including the text between start and end tags
            html = Regex.Replace(html, @"<script.*?</script>", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            //removing tyle tags including the text between start and end tags
            html = Regex.Replace(html, @"<style.*?</style>", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            //removing <noscript> including the text between start and end tags
            html = Regex.Replace(html, @"<noscript>.*?</noscript>", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            //code = Regex.Replace(code, @"<!--.*?-->", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            html = Regex.Replace(html, "<[^<>]+>", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            //removing character entities
            html = Regex.Replace(html, @"&\S+?;", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return html;
        }

        public string text_processing(string term, bool stopword_removal)
        {
            if (analysis_type == 1)
            {
                term = normalization(term, 0);
                if (!stopword_removal || !stopwords_dic.ContainsKey(term))
                    return term;
                else return string.Empty;
            }
            else if (analysis_type == 2)
            {
                term = normalization(term, 0);
                if (!stopword_removal || !stopwords_dic.ContainsKey(term))
                    return simple_stemmer(term);
                else return string.Empty;
            }
            else if (analysis_type == 3)
            {
                term = normalization(term, 0);
                if (!stopword_removal || !stopwords_dic.ContainsKey(term))
                    return light10_stemmer(term);
                else return string.Empty;
            }
            else if (analysis_type == 4)
            {
                term = normalization(term, 0);
                if (!stopword_removal || !stopwords_dic.ContainsKey(term))
                    return wikipedia_stemmer(term);
                else return string.Empty;
            }
                //for stopwords removal
            else if (analysis_type == 5)
            {
                if (!stopword_removal || !stopwords_dic.ContainsKey(term))
                    return term;
                else return string.Empty;
            }
            else if (analysis_type == 6)
            {
                term = normalization(term, 0);
                if (stopword_removal && stopwords_dic.ContainsKey(term))
                    return string.Empty;
                else {
                    StringBuilder temp = new StringBuilder(150);

                    string[] tokens = Tokenize(term);
                    foreach (string token in tokens)
                    {
                        temp.Append(light10_stemmer(token)+" ");
                    }
                    return temp.ToString().Trim();
                }
            }
            else
            {
                return term;
            }
        }

        public string[] Tokenize(string document)
        {
            return Regex.Split(document, @"[^\w]+",RegexOptions.Singleline);
            //Regex RE = new Regex(@"(\W)");
            //return (RE.Split(equation));
            //char[] sep = { ' ' };
            //Regex RE = new Regex(@"\W+");
            //textBox3.Lines = RE.Replace(textBox2.Text, " ").Split(sep, StringSplitOptions.RemoveEmptyEntries);
        }

        string normalization(string term, int normalizer_id)
        {
            if (normalizer_id == 0)
                return unified_normalization(term);
            else return term;
        }

        string unified_normalization(string word)
        {
            if (word.Length == 1)
                return "";
            string term = Regex.Replace(word, @"[\W-[\s]]", "", RegexOptions.Singleline);
            term = Regex.Replace(term, @"[\r\n]", "", RegexOptions.Singleline);
            term = term.Replace(ALEF_MAD, ALEF_BEDON_HAMZA);
            term = term.Replace(ALEF_HAMZA_FOU2, ALEF_BEDON_HAMZA);
            term = term.Replace(ALEF_HAMZA_TA7T, ALEF_BEDON_HAMZA);

            term = term.Replace(ALEF_MAKSOURA, YA2);
            term = term.Replace(TA2_MARBOUTA, HA2);

            term = term.Replace("ـ", "");

            return term;
        }

        public bool is_stopword(string word)
        {
            return stopwords_dic.ContainsKey(word);
        }

        string simple_stemmer(string term)
        {
            if (term.StartsWith("وال") && term.Length > 5)
                term = term.Remove(0, 3);
            if (term.StartsWith("بال") && term.Length > 5)
                term = term.Remove(0, 3);

            if (term.StartsWith("ال") && term.Length > 4)
                term = term.Remove(0, 2);
            if (term.StartsWith("لل") && term.Length > 4)
                term = term.Remove(0, 2);

            return term;
        }

        public string light10_stemmer(string word)
        {
            if (word.Length > 3 && word.StartsWith("و"))
            {
                word = word.Remove(0, 1);
            }

            int len = 0;
            int wordlen = word.Length;

            foreach (string article in Larkey_defarticles)
            {
                if (wordlen > (len = article.Length) + 1 && word.StartsWith(article))
                {
                    word = word.Remove(0, len);
                    break;
                }
            }

            if (word.Length > 2)
            {
                int suflen;
                wordlen = word.Length;

                if (wordlen == 0)
                    return null;

                foreach (string suffix in Larkey_suffixes)
                {
                    if (wordlen > (suflen = suffix.Length) + 1 && word.EndsWith(suffix))
                    {
                        word = word.Remove(wordlen - suflen);
                        wordlen = word.Length;
                    }
                }
            }
            return word;
        }

        string wikipedia_stemmer(string token)
        {
            string temp = token;
            //the word is not exist in wikipedia vocabulary
            if (!wikipedia_terms_dic.ContainsKey(temp))
            {
                if (temp.StartsWith("و") && temp.Length > 3)
                    temp = temp.Remove(0, 1);

                if (temp.StartsWith("لل") && temp.Length > 3)
                    temp = temp.Remove(0, 2);
                else if (temp.StartsWith("ال") && temp.Length > 3)
                    temp = temp.Remove(0, 2);
                else if (temp.StartsWith("بال") && temp.Length > 3)
                    temp = temp.Remove(0, 2);
                else if (temp.StartsWith("كال") && temp.Length > 3)
                    temp = temp.Remove(0, 2);
                else if (temp.StartsWith("فال") && temp.Length > 3)
                    temp = temp.Remove(0, 2);

                //term is not exist in wikipedia
                if (!wikipedia_terms_dic.ContainsKey(temp))
                {
                    if (temp.EndsWith("ون"))
                    {
                        temp = temp.Remove(temp.Length - 2);
                        if (!wikipedia_terms_dic.ContainsKey(temp))//not exist
                            return token;
                        else return temp;
                    }
                    else if (temp.EndsWith("ين"))
                    {
                        temp = temp.Remove(temp.Length - 2);
                        if (!wikipedia_terms_dic.ContainsKey(temp))//not exist
                            return token;
                        else return temp;
                    }
                    else if (temp.EndsWith("ات"))
                    {
                        temp = temp.Remove(temp.Length - 2);
                        if (!wikipedia_terms_dic.ContainsKey(temp))//not exist
                        {
                            temp = temp + "ه";
                            if (!wikipedia_terms_dic.ContainsKey(temp))//not exist
                                return token;
                            else return temp;
                        }
                        else return temp;
                    }
                    else return token;
                }
                else return temp;
            }
            else return token;
        }

        //static void populate_wikipedia_terms_K_mode()
        //{
        //    XmlDocument xmldoc = new XmlDocument();
        //    xmldoc.Load(system_parameters.Raw_concepts_path);
        //    XmlElement rootElem = xmldoc.DocumentElement;
        //    XmlNodeList synomymElements = rootElem.GetElementsByTagName("s");

        //    string temp;
        //    for (int i = 0; i < synomymElements.Count; i++)
        //    {
        //        temp = unified_normalization(synomymElements[i].Attributes[0].Value.Trim());
        //        if (!wikipedia_terms_dic.ContainsKey(temp))
        //            wikipedia_terms_dic.Add(temp, false);
        //    }
        //}

        public void tokenization(string filePath, ref Queue<string> tokens)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("windows-1256")))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                        if ((word = text_processing(word, true)) != string.Empty)
                            tokens.Enqueue(word);

                }
            }
            catch (Exception tokE)
            {
            }
        }

        public string generate_n_grams(string tokenized_file_path, int n)
        {
            string n_grams_file_path = @"C:\tempfilepath_ngrams.txt";
            //text_analysis arab_stem = new text_analysis();
            List<string> tokens_list = new List<string>();
            StringBuilder sb = new StringBuilder(1000);
            int count = n;

            using (StreamReader sr = new StreamReader(tokenized_file_path, Encoding.GetEncoding("windows-1256")))
            {
                using (StreamWriter sw = new StreamWriter(n_grams_file_path, false, Encoding.GetEncoding("windows-1256")))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null && count != 0)
                    {
                        count--;
                        tokens_list.Add(word);
                    }
                    while (tokens_list.Count > 0)
                    {
                        for (int x = 0; x < tokens_list.Count; x++)
                        {
                            sb.Append(" " + tokens_list[x]);
                            sw.WriteLine(sb.ToString().Trim());
                        }
                        sb.Remove(0, sb.Length);
                        if (word != null)
                            tokens_list.Add(word);
                        tokens_list.RemoveAt(0);
                        word = sr.ReadLine();
                    }
                }
            }
            return n_grams_file_path;
        }

        public void load_stopwords()
        {
            try
            {
                string word = string.Empty;

                using (StreamReader sr = new StreamReader("\\stopwords.txt", Encoding.GetEncoding("windows-1256")))
                    while ((word = sr.ReadLine()) != null)
                        stopwords_dic.Add(normalization(word, 0),true);
            }
            catch (Exception tokE)
            {
            }
        }
    }
}

