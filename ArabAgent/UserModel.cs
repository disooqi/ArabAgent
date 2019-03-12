using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using dosDataStructure;
using System.IO;

namespace ArabAgent
{
    class UserModel
    {
        DateTime _updateTime;
        UndirectedWeightedGraph userSemanticNetwork;
        string _userName;
        string profilePathStr;

        public UserModel(string userName)
        {
            _userName = userName;
            profilePathStr = @"D:\Dropbox\_workspace\work\KSA research group\ArabAgent\ArabAgent\" + userName + ".xml";
            //profilePathStr = Environment.CurrentDirectory;
            //DirectoryInfo dirInfo = new DirectoryInfo(profilePathStr);
            //DirectoryInfo rootDirInfo = dirInfo.Parent;
            //profilePathStr = rootDirInfo.FullName;
            //profilePathStr = profilePathStr + @"\" + _userName + "SN.xml"; ;
            
            userSemanticNetwork = new UndirectedWeightedGraph();
            loadUserModel();
        }
        public UserModel(FileInfo profilefile)
        {
            profilePathStr = profilefile.FullName;

            userSemanticNetwork = new UndirectedWeightedGraph();
            loadUserModel();
        }
        
        public bool createNewUserModel()
        { return true; }

        public void loadUserModel()
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(profilePathStr);
                //UPDATE Profile Here
                XmlElement rootElem = xmldoc.DocumentElement;

                //loading nodes
                XmlNodeList nodeElements = rootElem.GetElementsByTagName("C");
                for (int i = 0; i < nodeElements.Count; i++)
                {
                    IEnumerator daysEnum = nodeElements[i].FirstChild.GetEnumerator();
                    double conceptWeight = 0;
                    int docFreq = 0;
                    while (daysEnum.MoveNext())
                    {
                        conceptWeight += calculateDayImportance(((XmlNode)daysEnum.Current).Attributes[0].Value) *
                            double.Parse(((XmlNode)daysEnum.Current).Attributes[2].Value);

                        docFreq += int.Parse(((XmlNode)daysEnum.Current).Attributes[1].Value);
                    }

                    userSemanticNetwork.addNode(nodeElements[i].Attributes[0].Value, conceptWeight, docFreq);
                }

                //loading edges
                XmlNodeList edgeElements = rootElem.GetElementsByTagName("E");
                for (int i = 0; i < edgeElements.Count; i++)
                {
                    string fromNodeKey = edgeElements[i].Attributes[0].Value;
                    string toNodeKey = edgeElements[i].Attributes[1].Value;

                    IEnumerator daysEnum = edgeElements[i].FirstChild.GetEnumerator();
                    double edgeWeight = 0;
                    int edgeDocFreq = 0;

                    while (daysEnum.MoveNext())
                    {

                        edgeWeight += calculateDayImportance(((XmlNode)daysEnum.Current).Attributes[0].Value) *
                            (double.Parse(((XmlNode)daysEnum.Current).Attributes[2].Value) / 1);
                        edgeDocFreq += int.Parse(((XmlNode)daysEnum.Current).Attributes[1].Value);
                    }
                    edgeWeight = (double)edgeDocFreq / (userSemanticNetwork.NodesDictionary[fromNodeKey].DocFreq + userSemanticNetwork.NodesDictionary[toNodeKey].DocFreq - edgeDocFreq);
                    userSemanticNetwork.addEdge(fromNodeKey, toNodeKey, edgeWeight);
                }
            }
            catch (FileNotFoundException e)
            {
                string dir = Path.GetDirectoryName(profilePathStr);
                string xmlStr = "<SemanticNetwork createdBy=\"" + _userName + "\" createdOn=\"" + DateTime.Now.ToString() + "\"><Nodes></Nodes><Edges></Edges></SemanticNetwork>";
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xmlStr);
                xmldoc.Save(profilePathStr);
                loadUserModel();
                //File.Create(profilePathStr);
            }
        }

        public bool updateUserModel(ref Dictionary<int, n_gram> pos_struct_dic)
        {
            //calculate concept importance for the document
            Dictionary<string, double> conceptImp_dic = new Dictionary<string, double>();
            calculateConceptImportanceForeachConceptInTheDocument(ref pos_struct_dic, ref conceptImp_dic);

            //store the edges that have been updated so it does not added again
            Dictionary<string, List<string>> edgesDic = new Dictionary<string, List<string>>();

            Dictionary<string, double> conceptImp_dic_forEdgeUpdate = new Dictionary<string, double>();

            foreach (KeyValuePair<string, double> tempPair in conceptImp_dic)
            {
                conceptImp_dic_forEdgeUpdate.Add(tempPair.Key, tempPair.Value);
                //edgesDic.Add(tempPair.Key, new List<string>()); //for later use for updating edges
            }

            //for each concept check if it exist in the profile
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(profilePathStr);
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList nodeElements = rootElem.GetElementsByTagName("C");
            for (int i = 0; i < nodeElements.Count; i++)
            {
                //check if the concept is in the profile exist 
                if (conceptImp_dic.ContainsKey(nodeElements[i].Attributes[0].Value))
                {
                    // check if today exist
                    if (DateTime.Today.Day - DateTime.Parse(nodeElements[i].ChildNodes[0].FirstChild.Attributes[0].Value).Day == 0)
                    {
                        //  if today exist increase the DocCount by 1 and add the ConceptImp value to the sum of importance
                        int docCount = int.Parse(nodeElements[i].ChildNodes[0].FirstChild.Attributes[1].Value) + 1;
                        nodeElements[i].ChildNodes[0].FirstChild.Attributes[1].Value = docCount.ToString();

                        double sumOfAllConceptImportances = double.Parse(nodeElements[i].ChildNodes[0].FirstChild.Attributes[2].Value) +
                                                        conceptImp_dic[nodeElements[i].Attributes[0].Value];
                        nodeElements[i].ChildNodes[0].FirstChild.Attributes[2].Value = sumOfAllConceptImportances.ToString();
                    }
                    else {
                        //   else (today not exist) add a new day (today) make the value of DocCount =1 and the value of  sum of importance equals ConceptImp value
                        XmlElement todayNode = xmldoc.CreateElement("Day");
                        todayNode.SetAttribute("date", DateTime.Today.ToString());
                        todayNode.SetAttribute("docCount", "1");
                        todayNode.SetAttribute("SCISD",conceptImp_dic[nodeElements[i].Attributes[0].Value].ToString());
                        nodeElements[i].ChildNodes[0].InsertBefore(todayNode, nodeElements[i].ChildNodes[0].FirstChild);
                    }
                        conceptImp_dic.Remove(nodeElements[i].Attributes[0].Value);
                } 
            }

            //if the concept is compoletely new
            // else(concept not exist) add new concept with id = conceptId and add a new day also
            //the remaining concept means they aren't in the profile.
            foreach (KeyValuePair<string, double> conceptImp_Pair in conceptImp_dic)
            {
                XmlElement conceptElement = xmldoc.CreateElement("C");
                conceptElement.SetAttribute("cid", conceptImp_Pair.Key);
                conceptElement.SetAttribute("lt_interest", "");

                XmlElement daysElement = xmldoc.CreateElement("Days");
                
                XmlElement dayElement = xmldoc.CreateElement("Day");
                dayElement.SetAttribute("date", DateTime.Today.ToString());
                dayElement.SetAttribute("docCount", "1");
                dayElement.SetAttribute("SCISD", conceptImp_Pair.Value.ToString());

                daysElement.AppendChild(dayElement);
                conceptElement.AppendChild(daysElement);
                
                rootElem.GetElementsByTagName("Nodes")[0].AppendChild(conceptElement); 
            }

            //update edges
            //for old edges just update
            XmlNodeList edgeElements1 = rootElem.GetElementsByTagName("E");

            for (int i = 0; i < edgeElements1.Count; i++)
            {
                //if both exist that means this edge is going to be updated
                if (conceptImp_dic_forEdgeUpdate.ContainsKey(edgeElements1[i].Attributes[0].Value) &&
                    conceptImp_dic_forEdgeUpdate.ContainsKey(edgeElements1[i].Attributes[1].Value))
                {
                    //adding "from" attribute
                    if (!edgesDic.ContainsKey(edgeElements1[i].Attributes[0].Value))
                        edgesDic.Add(edgeElements1[i].Attributes[0].Value, new List<string>());
                    edgesDic[edgeElements1[i].Attributes[0].Value].Add(edgeElements1[i].Attributes[1].Value);

                    //adding "to" attribute
                    if (!edgesDic.ContainsKey(edgeElements1[i].Attributes[1].Value))
                        edgesDic.Add(edgeElements1[i].Attributes[1].Value, new List<string>());
                    edgesDic[edgeElements1[i].Attributes[1].Value].Add(edgeElements1[i].Attributes[0].Value);

                    //  if today exist increase the DocCount by 1 and add the ConceptImp value to the sum of importance
                    if (DateTime.Today.Day - DateTime.Parse(edgeElements1[i].ChildNodes[0].FirstChild.Attributes[0].Value).Day == 0)
                    {
                        int docCount = int.Parse(edgeElements1[i].ChildNodes[0].FirstChild.Attributes[1].Value) + 1;
                        edgeElements1[i].ChildNodes[0].FirstChild.Attributes[1].Value = docCount.ToString();



                        double sumOfMinConceptImportances = double.Parse(edgeElements1[i].ChildNodes[0].FirstChild.Attributes[2].Value) +
                                                        Math.Min(conceptImp_dic_forEdgeUpdate[edgeElements1[i].Attributes[0].Value],
                                                        conceptImp_dic_forEdgeUpdate[edgeElements1[i].Attributes[1].Value]);

                        edgeElements1[i].ChildNodes[0].FirstChild.Attributes[2].Value = sumOfMinConceptImportances.ToString();
                    }
                    //else (today not exist) add a new day (today) make the value of DocCount =1 and the value of  sum of min importance equals minimum ConceptImp value
                    else {
                        XmlElement todayNode = xmldoc.CreateElement("Day");
                        todayNode.SetAttribute("date", DateTime.Today.ToString());
                        todayNode.SetAttribute("docCount", "1");
                        double minmumValue = Math.Min(conceptImp_dic_forEdgeUpdate[edgeElements1[i].Attributes[0].Value],
                                                        conceptImp_dic_forEdgeUpdate[edgeElements1[i].Attributes[1].Value]);
                        todayNode.SetAttribute("SMP", minmumValue.ToString());

                        edgeElements1[i].ChildNodes[0].InsertBefore(todayNode, edgeElements1[i].ChildNodes[0].FirstChild);
                    }
                }
            }

            // for new edge add new edges
            List<string> conceptKeys = new List<string>(conceptImp_dic_forEdgeUpdate.Keys);

            for (int i = 0; i < conceptImp_dic_forEdgeUpdate.Count; i++ )
                for (int j = i + 1; j < conceptImp_dic_forEdgeUpdate.Count; j++)
                {
                    //if not exist in edgesDic
                    if (!edgesDic.ContainsKey(conceptKeys[i]) || !edgesDic[conceptKeys[i]].Contains(conceptKeys[j]))
                    {
                        XmlElement edgeElement1 = xmldoc.CreateElement("E");
                        edgeElement1.SetAttribute("from", conceptKeys[i]);
                        edgeElement1.SetAttribute("to", conceptKeys[j]);

                        XmlElement edgeDaysElement1 = xmldoc.CreateElement("Days");

                        XmlElement edgeDay1 = xmldoc.CreateElement("Day");
                        edgeDay1.SetAttribute("date", DateTime.Today.ToString());
                        edgeDay1.SetAttribute("docCount", "1");

                        double minmumValue = Math.Min(conceptImp_dic_forEdgeUpdate[conceptKeys[i]],
                                                        conceptImp_dic_forEdgeUpdate[conceptKeys[j]]);

                        edgeDay1.SetAttribute("SMP", minmumValue.ToString());
                        edgeDaysElement1.AppendChild(edgeDay1);
                        edgeElement1.AppendChild(edgeDaysElement1);
                        rootElem.GetElementsByTagName("Edges")[0].AppendChild(edgeElement1);
                    }
                }
            //IOException
        readfile: 
            try
            {
                xmldoc.Save(@"D:\Dropbox\_workspace\work\KSA research group\ArabAgent\ArabAgent\" + _userName + ".xml");
            }
            catch (IOException e)
            { goto readfile; }

            //remove the old days > 30 from the profile
            //load user model again to reflect changes
            return true;
        }
        
        private double calculateConceptImportance()
        { return 0; }

        private double calculateDayImportance(string dateStr)
        {
            DateTime date = DateTime.Parse(dateStr);
            TimeSpan dayDif = DateTime.Today - date;
            if (dayDif.Days > 30)
                return 0;
            else return (30 - dayDif.TotalDays) / 30;
        }

        public bool isInteresting(ref Dictionary<int, n_gram> pos_struct_dic)
        {
            if (calculateSimilarityToProfile(ref pos_struct_dic) > 0.4)
                return true;
            else return false;
        }

        public double calculateSimilarityToProfile(ref Dictionary<int, n_gram> pos_struct_dic)
        {
            UndirectedWeightedGraph textGraph = new UndirectedWeightedGraph();
            Dictionary<string, double> concept_frq_dic = new Dictionary<string, double>();
            calculateConceptImportanceForeachConceptInTheDocument(ref pos_struct_dic, ref concept_frq_dic);

            // first we create a graph for the text as the profile
            createGraphForText(ref textGraph, ref concept_frq_dic);

            List<string> conceptIds = new List<string>(textGraph.NodesDictionary.Keys);

            //temporary to store only those common in both graphs
            List<string> commonCId = new List<string>();

            for (int i = 0; i < conceptIds.Count; i++)
            {
                if (userSemanticNetwork.NodesDictionary.ContainsKey(conceptIds[i]))
                {
                    textGraph.NodesDictionary[conceptIds[i]].NodeWeight = userSemanticNetwork.NodesDictionary[conceptIds[i]].NodeWeight;
                    commonCId.Add(conceptIds[i]);
                }
            }

            for (int i = 0; i < commonCId.Count; i++)
                for (int j = i + 1; j < commonCId.Count; j++)
                {
                    //commonCId[i]
                    if (userSemanticNetwork.NodesDictionary[commonCId[i]].isNeighbour(commonCId[j]))
                    {
                        textGraph.NodesDictionary[commonCId[i]].NodeWeight = 
                            textGraph.NodesDictionary[commonCId[i]].NodeWeight + 
                            (textGraph.NodesDictionary[commonCId[i]].NodeWeight *
                        userSemanticNetwork.NodesDictionary[commonCId[i]].relationWeight(commonCId[j]));

                        textGraph.NodesDictionary[commonCId[j]].NodeWeight = 
                            textGraph.NodesDictionary[commonCId[j]].NodeWeight +
                            (textGraph.NodesDictionary[commonCId[j]].NodeWeight *
                        userSemanticNetwork.NodesDictionary[commonCId[j]].relationWeight(commonCId[i]));
                    }
                }

            int numberOfNodesThatHaveWeightsAboveThreshold = 0;

            for (int i = 0; i < commonCId.Count; i++)
                if (textGraph.NodesDictionary[commonCId[i]].NodeWeight > 0.2)
                    numberOfNodesThatHaveWeightsAboveThreshold++;

            return (double)numberOfNodesThatHaveWeightsAboveThreshold / (double)textGraph.verticsCount;
           
                //compare it with profile graph and then return a double value that represent how much the user interest 
                //in the text the following function is created for evaluation perpose only
        }

        private void createGraphForText(ref UndirectedWeightedGraph textGraph, ref Dictionary<string, double> concept_frq_dic)
        {
            List<string> poss = new List<string>(concept_frq_dic.Keys);

            for (int i = 0; i < poss.Count; i++)
                    textGraph.addNode(poss[i], 0);

            for (int i = 0; i < poss.Count; i++)
                for (int j = i + 1; j < poss.Count; j++)
                    textGraph.addEdge(poss[i], poss[j], 0);
        }

        public bool deleteUserModel()
        { return true; }

        public double compareUserSemanticNetworkWith()
        { return 0; }

        private void calculateConceptImportanceForeachConceptInTheDocument(ref Dictionary<int, n_gram> pos_ngram_dic, ref Dictionary<string, double> concept_freq_dic)
        {
            int totalConceptCount = 0;
            foreach (KeyValuePair<int, n_gram> ngram in pos_ngram_dic)
                if (ngram.Value.concept != null)
                {
                    if (concept_freq_dic.ContainsKey(ngram.Value.concept))
                        concept_freq_dic[ngram.Value.concept]++;
                    else concept_freq_dic.Add(ngram.Value.concept, 1);

                    totalConceptCount++;
                }

            Dictionary<string, double> tempDic = new Dictionary<string, double>();

            foreach (KeyValuePair<string, double> conceptFreqPair in concept_freq_dic)
            {
                if (conceptFreqPair.Value > 2)
                    tempDic.Add(conceptFreqPair.Key, conceptFreqPair.Value / totalConceptCount);
            }

            concept_freq_dic.Clear();
            concept_freq_dic = tempDic;
        }

        public DateTime updateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }

        public string TemporalProfilesPath // this was created for evaluation. the class shouldn't
        //provide such properties
        {
            set { profilePathStr = value; }
            get { return profilePathStr; }
        }

    }
}
