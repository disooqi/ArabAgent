using System;
using System.Collections.Generic;
using System.Text;

namespace dosDataStructure
{
    public class Node
    {
        string key;
        double weight;

        int totalNumOfDocs, totalNumOfDays;

        Dictionary<string, double> neigbours;

        public Node(string con) { key = con; neigbours = new Dictionary<string, double>(); }
        public Node(string con, double imp) { key = con; weight = imp; neigbours = new Dictionary<string, double>(); }
        public Node(string con, double imp, int docNo) { key = con; weight = imp; neigbours = new Dictionary<string, double>(); totalNumOfDocs = docNo; }

        public void AddNewNeigbour(string nKey)
        {
            neigbours.Add(nKey, 0);
        }

        public void AddNewNeigbour(string nKey, double co_ocurr)
        {
            neigbours.Add(nKey, co_ocurr);
        }

        public bool isNeighbour(string nKey)
        {
            return neigbours.ContainsKey(nKey);
        }

        public double relationWeight(string nKey)
        {
            return neigbours[nKey];
        }

        //properties
        public string NodeKey
        {
            get { return key; }
        }
        public double NodeWeight
        {
            get { return weight; }
            set { weight = value; }
        }

        public int DocFreq
        {
            get { return totalNumOfDocs; }
            set { totalNumOfDocs = value; }
        }

        //public int DayFreq
        //{
        //    get { return totalNumOfDays; }
        //    set { totalNumOfDays = value; }
        //}
    }
}
