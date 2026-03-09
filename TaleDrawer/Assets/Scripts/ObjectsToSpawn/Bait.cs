using System;
using System.Collections.Generic;
using UnityEngine;

public class Bait : Balloon
{
    [SerializeField] Paint _paintsSc;    
    public Action<FSMStates> onReleaseTarget;
    [SerializeField] LayerMask _inGameLayer;
    public bool attacked;
    public override void OnSpawned()
    {
        base.OnSpawned();
        gameObject.layer = CustomTools.ToLayer(_inGameLayer);        
    }
    public void AddEnemy(Monkey enemy)
    {
        onReleaseTarget += enemy.StopChasingTarget;        
    }

    public override void Delete()
    {
        GameManager.instance.RemoveSpawningObjectFromList(this);
        if (clouds != null)
        {
            Instantiate(clouds, transform.position, Quaternion.identity);
        }
        
        if (onReleaseTarget != null)
        {
            if (!attacked)
            {
                onReleaseTarget.Invoke(FSMStates.IdleState);
                onReleaseTarget = null;
            }
            else
            {
                onReleaseTarget.Invoke(FSMStates.StunnedState);
                onReleaseTarget = null;
            }
            
        }

        Destroy(gameObject);
    }

    public override void Paint()
    {
        StartCoroutine(_paintsSc.PaintSprite());
    }

    public override void OnWind()
    {
        if(onReleaseTarget != null)
        {
            onReleaseTarget.Invoke(FSMStates.IdleState);
            onReleaseTarget = null;
        }
        
    }

    protected override void Update()
    {
        
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
