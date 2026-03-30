using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Bait : Balloon
{
    [SerializeField] Paint _paintsSc;
    [SerializeField] Sprite[] _sprites;
    public Action<Transform> onReleaseTarget;
    [SerializeField] LayerMask _inGameLayer;
    public bool attacked;
    public Transform attacker;
    [SerializeField] Transform _basePosition;
    [SerializeField] LayerMask _baitExcludeLayer;
    [SerializeField] float _floorDetetctionDist;
    public bool aboveFloor;
    [SerializeField] float _speedMultiplier;

    public override void Start()
    {
        base.Start();
        int random = UnityEngine.Random.Range(0, _sprites.Length);

        _mySpriteRenderer.sprite = _sprites[random];

    }

    public override void OnSpawned()
    {
        base.OnSpawned();
        gameObject.layer = CustomTools.ToLayer(_inGameLayer);

        if (!IsGrounded())
        {
            MoveTowardsFloor();
        }
        else
        {
            aboveFloor = true;
        }
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

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(_basePosition.position, Vector2.one * .8f, 0, -transform.up, 0, _baitExcludeLayer);

        if (hit.collider == null || hit.collider.isTrigger)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void MoveTowardsFloor()
    {
        RaycastHit2D hit = Physics2D.BoxCast(_basePosition.position, Vector2.one * .8f, 0, -transform.up, _floorDetetctionDist, _baitExcludeLayer);

        if (hit.collider != null && !hit.collider.isTrigger)
        {
            float speed = Math.Abs(_basePosition.position.y - hit.point.y);

            if(speed < 1)
            {
                speed = 1;
            }

            transform.DOMoveY(hit.point.y + MathF.Abs(_basePosition.localPosition.y),
                speed * _speedMultiplier).OnComplete(() => aboveFloor = true);
        }
        
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
