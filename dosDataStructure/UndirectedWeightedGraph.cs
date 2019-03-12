using System;
using System.Collections.Generic;
using System.Text;

namespace dosDataStructure
{
    public class UndirectedWeightedGraph
    {
        Dictionary<string, Node> vertices = new Dictionary<string, Node>();
        //List<Node> vertices;
        int _edgeCount;

        public UndirectedWeightedGraph() { vertices = new Dictionary<string, Node>(); }

        public void addNode(string nKey)
        {
            vertices.Add(nKey, new Node(nKey));
        }

        public void addNode(string nKey, double importance)
        {
            vertices.Add(nKey, new Node(nKey, importance));
        }

        public void addNode(string nKey, double importance, int docFreq)
        {
            vertices.Add(nKey, new Node(nKey, importance,docFreq));
        }

        public void addEdge(string fromKey, string toKey)
        {
            if (fromKey.Equals(toKey))
                return;
            //to check if both nodes exist
            if (vertices.ContainsKey(fromKey) && vertices.ContainsKey(toKey))
            {
                //check if added before
                if (!vertices[fromKey].isNeighbour(toKey) && !vertices[toKey].isNeighbour(fromKey))
                {
                    vertices[fromKey].AddNewNeigbour(toKey);
                    vertices[toKey].AddNewNeigbour(fromKey);

                    _edgeCount++;
                }
            }
        }

        public void addEdge(string fromKey, string toKey, double edgeImportance)
        {
            if (fromKey.Equals(toKey))
                return;
            //to check if both nodes exist
            if (vertices.ContainsKey(fromKey) && vertices.ContainsKey(toKey))
            {
                if (!vertices[fromKey].isNeighbour(toKey) && !vertices[toKey].isNeighbour(fromKey))
                {
                    vertices[fromKey].AddNewNeigbour(toKey, edgeImportance);
                    vertices[toKey].AddNewNeigbour(fromKey, edgeImportance);

                    _edgeCount++;
                }
            }
        }

        public int verticsCount
        {
            get { return vertices.Count; }
        }

        public int edgeCount
        {
            get { return -1; }
        }

        //private bool nodeExisted(int nKey)
        //{
            
        //    IEnumerator<Node> IEver = vertices.GetEnumerator();
        //    while (IEver.MoveNext())
        //        if (nKey.Equals(IEver.Current.NodeKey))
        //            return true;
                        
        //    return false; // reaching here means the node is not exist in the graph.
        //}

        //private bool nodeExisted(string nKey)
        //{
        //    if (nodeIndex(nKey) == -1)
        //        return false;
        //    else return true;
        //}

        //private int nodeIndex(string nKey)
        //{
        //    for (int i = 0; i < vertices.Count; i++)
        //    {
        //        if (vertices[i].NodeKey.Equals(nKey))
        //            return i;
        //    }
        //    return -1;
        //}

        //public Node getNodeWithValue(string nkey)
        //{
        //    IEnumerator<Node> IEver = vertices.GetEnumerator();
        //    while (IEver.MoveNext())
        //        if (nkey.Equals(IEver.Current.NodeKey))
        //            return IEver.Current;

        //    return null; // reaching here means the node is not exist in the graph.
        //}

        public Dictionary<string, Node> NodesDictionary
        {
            get { return vertices; }
        }
    }
}
