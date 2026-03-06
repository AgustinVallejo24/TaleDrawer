using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] public Transform balloonPosition;
    public Balloon currentBalloon;
    public bool inWind;
    public Rigidbody2D entityRigidbody;
    public virtual void PutOnHelmet()
    {

    }

    public virtual void CatchBoleadoras()
    {

    }
    public virtual void LiftEntity()
    {

    }
    public virtual void ReleaseFromBalloon()
    {

    }

    public virtual void Death()
    {

    }
}
