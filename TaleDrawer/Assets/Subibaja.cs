using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Subibaja : MonoBehaviour, IInteractable
{
    public List<Transform> sides;
    public bool left = true;
    public Animator animator;
    public void Interact(SpawningObject interactor)
    {
        
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
            }
            else if (closest == sides[1] && !left)
            {
                Destroy(interactor);
            }
            else
            {

            }
        }
    }

    public void Interact(GameObject interactor)
    {
     
    }
}

