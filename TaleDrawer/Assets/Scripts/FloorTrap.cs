using UnityEngine;
using System.Linq;
public class FloorTrap : Trap
{
    [SerializeField] Animator _floor;
    public bool open;
    [SerializeField] CustomNode[] innerNodes;
    [SerializeField] CustomNode[] outerNodes;
    [SerializeField] Transform[] jumpPositions;
    public override void Activation()
    {
        _floor.SetTrigger("Open");
        open = true;
 
    }

    public CustomNode GetClosestOuterNode()
    {
        Character character = Character.instance;
        return outerNodes.OrderBy(x => Vector3.Distance(x.transform.position, character.transform.position)).First();
    }
    public Transform GetClosestJumpPos()
    {
        Character character = Character.instance;
        return jumpPositions.OrderBy(x => Vector3.Distance(x.transform.position, character.transform.position)).First();
    }
    public override void Deactivation()
    {
        _floor.SetTrigger("Close");
        open = false;
    }


    public void ActivateNodes()
    {
        for (int i = 0; i < 2; i++)
        {
            outerNodes[i].SetCanDoEvent(innerNodes[i], true);
        }
    }

    public void DeactivateNodes()
    {

        for (int i = 0; i < 2; i++)
        {
            outerNodes[i].SetCanDoEvent(innerNodes[i], false);
        }
    }


}
