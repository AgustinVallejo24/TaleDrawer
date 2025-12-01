using UnityEngine;

public class Polea : MonoBehaviour, IInteractable
{
    public Transform jumpPos;
    public Animator anim;
    public CustomNode platformNode;
    public bool hasPlayer;
    public float platformWeight;
    public float netWeight;
    public void InsideInteraction()
    {
        
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
            character.GetPath(platformNode);
            character.SendInputToFSM(CharacterStates.Moving);
        }
    }

    public void CheckWeight()
    {
        float weightDifference = netWeight - platformWeight;
        if (weightDifference > 4)
        {
            anim.SetTrigger("High");
        }
        else if (weightDifference <= 4 && weightDifference > 2)
        {
            anim.SetTrigger("Middle");
        }
        else
        {
            anim.SetTrigger("Low");
        }
    }

    
}
