package arabagent;

import java.util.*;

public class Node {

    String key;
    double weight;

    int totalNumOfDocs, totalNumOfDays;

    HashMap<String, Double> neigbours;

    public Node(String con) { key = con; neigbours = new HashMap<String, Double>(); }
    public Node(String con, double imp) { key = con; weight = imp; neigbours = new HashMap<String, Double>(); }
    public Node(String con, double imp, int docNo) { key = con; weight = imp; neigbours = new HashMap<String, Double>(); totalNumOfDocs = docNo; }

    public void addNewNeigbour(String nKey)
    {
        neigbours.put(nKey, 0.0);
    }

    public void addNewNeigbour(String nKey, double co_ocurr)
    {
        neigbours.put(nKey, co_ocurr);
    }

    public boolean isNeighbour(String nKey)
    {
        return neigbours.containsKey(nKey);
    }

    public double relationWeight(String nKey)
    {
        return neigbours.get(nKey);
    }

    //properties
    public String getNodeKey(){
        return key;
    }
    
    public void setNodeWeight(double weight){
        this.weight = weight;
    }
    
    public double getNodeWeight(){
        return weight;
    }

    public void setDocFreq(int totalNumOfDocs){
        this.totalNumOfDocs = totalNumOfDocs; 
    }
    
    public int getDocFreq(){
        return totalNumOfDocs;
    }
}
