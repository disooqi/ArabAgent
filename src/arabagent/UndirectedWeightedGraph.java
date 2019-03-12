package arabagent;

import java.util.*;

public class UndirectedWeightedGraph {
    HashMap<String, Node> vertices;
    //List<Node> vertices;
    int _edgeCount;

    UndirectedWeightedGraph() { vertices = new HashMap<String, Node>(); }

    private void addNode(String nKey)
    {
        vertices.put(nKey, new Node(nKey));
    }

    private void addNode(String nKey, double importance)
    {
        vertices.put(nKey, new Node(nKey, importance));
    }

    private void addNode(String nKey, double importance, int docFreq)
    {
        vertices.put(nKey, new Node(nKey, importance,docFreq));
    }

    private void addEdge(String fromKey, String toKey)
    {
        if (fromKey.equals(toKey))
            return;
        //to check if both nodes exist
        if (vertices.containsKey(fromKey) && vertices.containsKey(toKey))
        {
            //check if added before
            if (!vertices.get(fromKey).isNeighbour(toKey) && !vertices.get(toKey).isNeighbour(fromKey))
            {
                vertices.get(fromKey).addNewNeigbour(toKey);
                vertices.get(toKey).addNewNeigbour(fromKey);

                _edgeCount++;
            }
        }
    }

    private void addEdge(String fromKey, String toKey, double edgeImportance)
    {
        if (fromKey.equals(toKey))
            return;
        //to check if both nodes exist
        if (vertices.containsKey(fromKey) && vertices.containsKey(toKey))
        {
            if (!vertices.get(fromKey).isNeighbour(toKey) && !vertices.get(toKey).isNeighbour(fromKey))
            {
                vertices.get(fromKey).addNewNeigbour(toKey, edgeImportance);
                vertices.get(toKey).addNewNeigbour(fromKey, edgeImportance);

                _edgeCount++;
            }
        }
    }

    private int getVerticsCount(){
        return vertices.size();
    }

    private int getEdgeCount(){
        return -1;
    }

    private HashMap<String, Node> getNodesDictionary()
    {
        return vertices;
    }
}
