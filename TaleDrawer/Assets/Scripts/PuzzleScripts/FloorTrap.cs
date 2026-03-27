using UnityEngine;
using System.Linq;
public class FloorTrap : Trap
{
    [SerializeField] Animator _floor;
    public bool open;

    public override void Activation()
    {
        _floor.SetTrigger("Open");
        open = true;
 
    }

  
    public override void Deactivation()
    {
        _floor.SetTrigger("Close");
        open = false;
    }


  


}
