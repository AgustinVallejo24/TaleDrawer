using UnityEngine;
using System.Collections.Generic;
public class Polea : MonoBehaviour, IInteractableSP
{
    public Transform jumpPos;
    public Animator anim;
    public CustomNode platformNode;
    public bool hasPlayer;
    public float platformWeight;
    public float netWeight;
    public List<NodesAndSections> nodeList;
    InteractableType _interactableType;
    public void InsideInteraction()
    {
        
    }

    public InteractableType MyInteractableType()
    {
        return _interactableType;
    }
    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {
        
    }

    public void Interact(SpawningObject spawningObject)
    {
        
    }

    public void Interact(GameObject interactor)
    {
       
    }

    public void InteractWithPlayer()
    {
        Character character = Character.instance;
        if (!hasPlayer)
        {
            character.SendInputToFSM(CharacterStates.Moving);
        }
    }

    public void CheckWeight()
    {
        float weightDifference = netWeight - platformWeight;
        if (weightDifference > 4)
        {
            anim.SetTrigger("High");
            NodeActivation("High");
        }
        else if (weightDifference <= 4 && weightDifference > 2)
        {
            anim.SetTrigger("Middle");
            NodeActivation("Middle");
        }
        else
        {
            anim.SetTrigger("Low");
            NodeActivation("Low");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Character character))
        {
            ConfigurePlayer(character,true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Character character))
        {
            ConfigurePlayer(character,false);
        }
    }
    public void ConfigurePlayer(Character _myCharacter, bool state)
    {
        if (state)
        {
            _myCharacter.transform.parent = transform;
            hasPlayer = true;
            platformWeight = 2;

        }
        else
        {
            _myCharacter.transform.parent = null;
            hasPlayer = false;
            platformWeight = 0;
        }



    }

    public void NodeActivation(string section)
    {
        //foreach (var item in nodeList)
        //{
        //    if(section == "Low")
        //    {
        //        if (item.section != "Low")
        //        {
        //            foreach (var node in item.nodes)
        //            {
        //                platformNode.SetCanDoEvent(node, false);

        //            }
        //        }
        //        else
        //        {
        //            foreach (var node in item.nodes)
        //            {
        //                node.SetCanDoEvent(platformNode, true);
        //            }
        //        }
        //    }
        //    else if(section == "Middle")
        //    {
        //        if (item.section == "Middle")
        //        {
        //            foreach (var node in item.nodes)
        //            {
        //                platformNode.SetCanDoEvent(node, true);
        //                node.SetCanDoEvent(platformNode, true);
        //            }
        //        }
        //        else if(item.section == "High")
        //        {
        //            foreach (var node in item.nodes)
        //            {
        //                platformNode.SetCanDoEvent(node, false);
        //            }
        //        }
        //        else if (item.section == "Low")
        //        {
        //            foreach (var node in item.nodes)
        //            {
        //                node.SetCanDoEvent(platformNode, false);
        //            }
        //        }
        //    }
        //    else if (section == "High")
        //    {
        //        if (item.section == "High")
        //        {
        //            foreach (var node in item.nodes)
        //            {
        //                platformNode.SetCanDoEvent(node, true);
        //                node.SetCanDoEvent(platformNode, true);
        //            }
        //        }
        //        else if (item.section == "Middle")
        //        {
        //            foreach (var node in item.nodes)
        //            {
        //                platformNode.SetCanDoEvent(node, true);
        //                node.SetCanDoEvent(platformNode, false);
        //            }
        //        }
        //        else if (item.section == "Low")
        //        {
        //            foreach (var node in item.nodes)
        //            {
        //                node.SetCanDoEvent(platformNode, false);
        //            }
        //        }

        //    }

       // }

    }
    
}


[System.Serializable]
public struct NodesAndSections
{
    public string section;
    public List<CustomNode> nodes;

}