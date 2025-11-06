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
    public bool hasPlayer;

    [SerializeField] Subibaje_CustomNode _leftNode;
    [SerializeField] Subibaje_CustomNode _rightNode;
    [SerializeField] CustomNode _entryLeftLowerNode;
    [SerializeField] CustomNode _entryRightLowerNode;
    [SerializeField] CustomNode _entryLeftUpperNode;
    [SerializeField] CustomNode _entryRightUpperNode;
    public NewSerializableDictionary<SubibajaStates, float> weightState = new NewSerializableDictionary<SubibajaStates, float>();
    public SubibajaStates currentState;
    public float weightThreshold;
    [SerializeField] List<GameObject> currentObjects;
    public RelativeJoint2D myJoint;
    float currentZRot;
    [SerializeField] SpawnableObjectType _posibleObjects;
    private void Start()
    {
        //if (left)
        //{
        //    rightCollider.enabled = true;
        //    leftCollider.enabled = false;
        //}
        //else
        //{
        //    rightCollider.enabled = false;
        //    leftCollider.enabled = true;
        //}
        
    }
    private void Update()
    {
        currentZRot = transform.localEulerAngles.z;

   
        if(currentZRot < 17 && currentZRot > 9 && !left)
        {
            left = true;
            ChangeLeftNeightbours(0);
            ChangeRightNeightbours(1);
        }
        else if(currentZRot > 343 && currentZRot < 351 && left)
        {
            left = false;
            ChangeLeftNeightbours(1);
            ChangeRightNeightbours(0);
        }
    }
    public void Interact(SpawningObject spawningObject)
    {
        if (_posibleObjects.HasFlag(spawningObject.myType))
        {
            if(Vector2.Distance(spawningObject.transform.position, _leftNode.transform.position) < Vector2.Distance(spawningObject.transform.position, _rightNode.transform.position))
            {
                spawningObject.transform.position = _leftNode.transform.position;
            }
            else
            {
                spawningObject.transform.position += _rightNode.transform.position;
            }
            
        }
        else
        {
            spawningObject.CantInteract();
        }
    }

    public void OnMovingStateChange(bool sign)
    {
        if (myCharacter == null)
        {
            characterDetector.enabled = sign;
            isMoving = sign;

        }


    }

    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {

        interactor.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        //if (!interactor.TryGetComponent<SpawningObject>(out SpawningObject myObject) || currentObjects.Contains(interactor)) return;

        //float objectWeight = myObject.weight;

        //if (objectWeight < weightThreshold) return;
        //myObject.interactableDelegate += AddWeight;
        //currentObjects.Add(interactor);

        //var closest = sides.OrderBy(x => Vector2.Distance(x.position, interactor.transform.position)).First();
        //if (closest == sides[0])
        //{

        //    weightState[SubibajaStates.Left] = weightState[SubibajaStates.Left] + objectWeight;
        //    interactor.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        //    interactor.transform.parent = this.transform;



        //}
        //else if (closest == sides[1])
        //{
        //    weightState[SubibajaStates.Right] = weightState[SubibajaStates.Right] + objectWeight;
        //    interactor.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        //    interactor.transform.parent = this.transform;

        //}
        //CheckWeight();

    }
    private void FixedUpdate()
    {
        if (hasPlayer)
        {
            myCharacter.transform.rotation = transform.rotation;
        }
    }
    public void CheckWeight()
    {
        switch (currentState)
        {
            case SubibajaStates.Left:
                if (weightState[SubibajaStates.Left] > weightState[SubibajaStates.Right])
                {

                }
                else if (weightState[SubibajaStates.Left] == weightState[SubibajaStates.Right])
                {
                    currentState = SubibajaStates.Middle;
                    animator.SetTrigger("SwitchMiddle");
                }
                else
                {
                    currentState = SubibajaStates.Right;
                    animator.SetTrigger("SwitchRight");
                }

                break;
            case SubibajaStates.Right:

                if (weightState[SubibajaStates.Left] > weightState[SubibajaStates.Right])
                {
                    currentState = SubibajaStates.Left;
                    animator.SetTrigger("SwitchLeft");
                }
                else if (weightState[SubibajaStates.Left] == weightState[SubibajaStates.Right])
                {
                    currentState = SubibajaStates.Middle;
                    animator.SetTrigger("SwitchMiddle");
                }
                else
                {

                }
                break;
            case SubibajaStates.Middle:

                if (weightState[SubibajaStates.Left] > weightState[SubibajaStates.Right])
                {
                    currentState = SubibajaStates.Left;
                    animator.SetTrigger("SwitchLeft");
                }
                else if (weightState[SubibajaStates.Left] == weightState[SubibajaStates.Right])
                {

                }
                else
                {
                    currentState = SubibajaStates.Right;
                    animator.SetTrigger("SwitchRight");
                }

                break;
            default:
                break;
        }
    }
    public void Interact(GameObject interactor)
    {

    }

    public void InteractWithPlayer()
    {
        Debug.LogError("SDAD");
        Character character = Character.instance;
        if (hasPlayer)
        {
            if (Vector3.Distance(character.transform.position, _leftNode.transform.position) > (Vector3.Distance(character.transform.position, _rightNode.transform.position)))
            {
                character.GetPath(_leftNode);
                //character.characterModel.Flip(_leftNode.transform.position);
              //  Destroy(myJoint);
            }
            else
            {
                character.GetPath(_rightNode);
                //character.characterModel.Flip(_rightNode.transform.position);
               // Destroy(myJoint);
            }
        }
        else
        {
            if (Vector3.Distance(character.transform.position, _leftNode.transform.position) > (Vector3.Distance(character.transform.position, _rightNode.transform.position)))
            {
                character.GetPath(_rightNode);
            }
            else
            {
                character.GetPath(_leftNode);

            }
        }


        character.SendInputToFSM(CharacterStates.Moving);

    }

    public void AddWeight(float weightToAdd, GameObject myObject)
    {
        var closest = sides.OrderBy(x => Vector2.Distance(x.position, myObject.transform.position)).First();
        if (closest == sides[0])
        {
            Mathf.Clamp(weightState[SubibajaStates.Left] + weightToAdd, 0, Mathf.Infinity);
            weightState[SubibajaStates.Left] = weightState[SubibajaStates.Left] + weightToAdd;
        }
        else if (closest == sides[1])
        {
            weightState[SubibajaStates.Right] = weightState[SubibajaStates.Right] + weightToAdd;
        }
        currentObjects.Remove(myObject);

        CheckWeight();
    }

    public void CreateJoint()
    {
        Debug.LogError("EntroACACACACA");
        myJoint = gameObject.AddComponent<RelativeJoint2D>();
        myJoint.connectedBody = myCharacter.characterRigidbody;
        myJoint.autoConfigureOffset = true;

        // Ajustes recomendados
        myJoint.maxForce = 1000f;   // cuán fuerte lo “pega”
        myJoint.maxTorque = 10;
        myJoint.enableCollision = true;
        hasPlayer = true;
    }
    public void DestroyJoint()
    {
        Destroy(myJoint);
    }
    public void ChangeLeftNeightbours(int value)
    {
       
        if (value == 0)
        {
            _leftNode.SetNeghtboursBool(_entryLeftLowerNode, true);
            _leftNode.SetNeghtboursBool(_entryLeftUpperNode, false);
        }
        else
        {
            _leftNode.SetNeghtboursBool(_entryLeftLowerNode, false);
            _leftNode.SetNeghtboursBool(_entryLeftUpperNode, true);
        }
    

    }
    public void ChangeRightNeightbours(int value)
    {
        if (value == 0)
        {
            _rightNode.SetNeghtboursBool(_entryLeftLowerNode, true);
            _rightNode.SetNeghtboursBool(_entryLeftUpperNode, false);
        }
        else
        {
            _rightNode.SetNeghtboursBool(_entryLeftLowerNode, false);
            _rightNode.SetNeghtboursBool(_entryLeftUpperNode, true);
        }


    }
}

public enum SubibajaStates
{
    Left,
    Right,
    Middle,
}
