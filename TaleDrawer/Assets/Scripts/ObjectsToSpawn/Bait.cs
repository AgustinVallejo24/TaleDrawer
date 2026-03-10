using System;
using System.Collections.Generic;
using UnityEngine;

public class Bait : Balloon
{
    [SerializeField] Paint _paintsSc;    
    public Action<Transform> onReleaseTarget;
    [SerializeField] LayerMask _inGameLayer;
    public bool attacked;
    public Transform attacker;
    public override void OnSpawned()
    {
        base.OnSpawned();
        gameObject.layer = CustomTools.ToLayer(_inGameLayer);        
    }
    public void AddEnemy(Enemy enemy)
    {
        onReleaseTarget += enemy.HandleAggroRelease;        
    }

    public override void Delete()
    {
        GameManager.instance.RemoveSpawningObjectFromList(this);
        if (clouds != null)
        {
            Instantiate(clouds, transform.position, Quaternion.identity);
        }
        
        ReleaseTargets();      

        Destroy(gameObject);
    }

    public override void Paint()
    {
        StartCoroutine(_paintsSc.PaintSprite());
    }


    public void ReleaseTargets()
    {
        if(onReleaseTarget != null)
        {
            onReleaseTarget.Invoke(attacker);
            onReleaseTarget = null;
            attacker = null;
        }
    }

    public override void OnWind()
    {
        ReleaseTargets();        
    }

    protected override void Update()
    {
        
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
