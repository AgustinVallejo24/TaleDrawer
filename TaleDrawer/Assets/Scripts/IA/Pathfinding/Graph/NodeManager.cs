using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class NodeManager : MonoBehaviour
{
    public List<Node> allNodes;

    public event Action onGraphUpdate = delegate { };

    [SerializeField] LayerMask wallLayer;
    public static NodeManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    private void Start()
    {
        GetAllNodes();
    }

    public void UpdateGraph()
    {
        onGraphUpdate();
    }

    public void GetAllNodes()
    {
        var everyNode = new FList<Node>();
        
        allNodes = everyNode.ToList();
    }
    


    public Node GetNearestNode(Vector3 position)
    {
        Node nearestNode = default;
        float _currentDist = Mathf.Infinity;

        foreach (Node node in allNodes)
        {

            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < _currentDist)
            {
                _currentDist = dist;
                nearestNode = node;
            }
        }

        return nearestNode;
    }

    public Node GetNearestNodeInLOS(Vector3 position)
    {
        Node nearestNode = default;
        float _currentDist = Mathf.Infinity;

        foreach (Node node in allNodes)
        {
            if (!InLOSTool.InLOS(position, node.transform.position, wallLayer)) continue;
                float dist = Vector3.Distance(position, node.transform.position);
            if (dist < _currentDist)
            {
                _currentDist = dist;
                nearestNode = node;
            }
        }

        return nearestNode;
    }
}
