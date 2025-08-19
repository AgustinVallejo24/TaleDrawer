using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbours;
    public int cost;    
    [SerializeField] NodeManager manager;



    [SerializeField] LayerMask _wallLayer;
    [SerializeField] [Range(0, 15)] float _viewRange;


    private void Start()
    {
        manager = NodeManager.instance;
        
        CallEvent();
        manager.onGraphUpdate += CallEvent;
    }

    public void UnsubscribeFromGraph()
    {
        manager.onGraphUpdate -= CallEvent;
    }


    void CallEvent()
    {
        Invoke(nameof(GetNeighbours), 0.03f);
    }

    public bool InLOS(Vector3 start, Vector3 desiredPos)
    {
        Vector3 dir = desiredPos - start;
        return !Physics.Raycast(start, dir, dir.magnitude, _wallLayer);
    }


    public void GetNeighbours()
    {
        List<Node> currentNeighbours = new List<Node>();
        foreach(Node node in manager.allNodes)
        {
            if(node == null) continue;
            if (node == this) continue;
            if (InLOS(transform.position, node.transform.position)) currentNeighbours.Add(node);
        }
        neighbours = currentNeighbours;
    }

    private void OnDestroy()
    {
        UnsubscribeFromGraph();
    }

    //private void OnDrawGizmos()
    //{
    //    //Gizmos.color = Color.white;
    //    //Gizmos.DrawWireSphere(transform.position, _viewRange);

    //    foreach(Node node in manager.allNodes)
    //    {
    //        if (node == this) continue;
    //        if (Vector3.Distance(transform.position, node.transform.position) > _viewRange) continue;
    //        if (InLOS(transform.position, node.transform.position))
    //        {
    //            Gizmos.color = Color.green;
    //            Gizmos.DrawLine(transform.position, node.transform.position);
    //        }
    //        else
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawLine(transform.position, node.transform.position);
    //        }
    //    }
    //}
}
