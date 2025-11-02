using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class CustomNode : MonoBehaviour
{
    ///public List<CustomNode> neighbours;    
    public List<NeighbouringNodesAndActions> neighbours;
    [SerializeField] public GameObject _jumpPosition;
    public bool shouldWait;
    public Action goalDelegate;
    public bool isClickable = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Jump( Transform jumpDestination)
    {
        Character.instance.Jump(jumpDestination);
    }

    public void SetCanDoEvent(CustomNode customNode, bool value)
    {
        int index = neighbours.FindIndex(x => x.node == customNode);
        if (index == -1) return;

        var neighbour = neighbours[index];
        neighbour.canDoEvent = value;
        neighbours[index] = neighbour;
    }
}

[System.Serializable]
public struct NeighbouringNodesAndActions
{
    public CustomNode node;
    public UnityEvent nodeEvent;
    public bool canDoEvent;
    public int cost;
    public NeighbouringNodesAndActions(CustomNode newNode, UnityEvent newEvent, bool newCanDoEvent, int newCost)
    {
        node = newNode;
        nodeEvent = newEvent;
        canDoEvent = newCanDoEvent;
        cost = newCost;
    }
}
