using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Subibaja : MonoBehaviour, IInteractable
{
    public List<Transform> sides;
    public bool left = true;
    public Animator animator;
    public Collider2D leftCollider;
    public Collider2D rightCollider;

    public Character myCharacter;

    public SubibajaEn_InterestPoint interestPoint;
    public Transform tpPoint;

    public Collider2D characterDetector;
    public bool isMoving;

    [SerializeField] Subibaje_CustomNode _leftNode;
    [SerializeField] Subibaje_CustomNode _rightNode;
    private void Start()
    {
        if (left)
        {
            rightCollider.enabled = true;
            leftCollider.enabled = false;
        }
        else
        {
            rightCollider.enabled = false;
            leftCollider.enabled = true;
        }
    }
    public void Interact(SpawningObject interactor)
    {
        
    }

    public void OnMovingStateChange(bool sign)
    {
        if(myCharacter == null)
        {
            characterDetector.enabled = sign;
            isMoving = sign;

        }


    }
    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {
       if(objectType == SpawnableObjectType.Caja)
        {

            var closest = sides.OrderBy(x => Vector2.Distance(x.position, interactor.transform.position)).First();
            if (closest == sides[0] && left)
            {
                Debug.LogError("aaaaaaaa");
                Destroy(interactor);
            }
            else if (closest == sides[1] && left)
            {
                //interactor.transform.position = sides[1].position;
                
                animator.SetTrigger("Switch");
                interactor.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                interactor.transform.parent = this.transform;
                rightCollider.enabled = true;
                leftCollider.enabled = false;
                left = false;
            }
            else if (closest == sides[1] && !left)
            {
                Destroy(interactor);
            }
            else
            {
                rightCollider.enabled = false;
                leftCollider.enabled = true;
                animator.SetTrigger("Switch");
                left = true;
            }
        }
    }

    public void Interact(GameObject interactor)
    {
     
    }

    public void InteractWithPlayer()
    {
        Debug.LogError("SDAD");
        Character character = Character.instance;
        character.GetPath(_leftNode);
        character.SendInputToFSM(CharacterStates.Moving);
        
    }


    public void ChangeLeftNeightbours(int value)
    {
        _leftNode.SetNeghtboursBool(value);
    }
    public void ChangeRightNeightbours(int value)
    {
        _rightNode.SetNeghtboursBool(value);
    }
}

