using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomNode : MonoBehaviour
{
    ///public List<CustomNode> neighbours;    
    public List<NeighbouringNodesAndActions> neighbours;
    [SerializeField] GameObject _jumpPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Jump()
    {
        Debug.Log("Llego al salto");
        Vector2 jumpPos = new Vector2(_jumpPosition.transform.position.x, _jumpPosition.transform.position.y);
        Character.instance.characterModel.Jump(jumpPos, 1);
    }
}

[System.Serializable]
public struct NeighbouringNodesAndActions
{
    public CustomNode node;
    public UnityEvent nodeEvent;
    public bool canDoEvent;
    public int cost;
}
