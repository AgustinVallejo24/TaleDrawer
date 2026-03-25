
using UnityEngine;

public class Umbrella : SpawningObject
{
    [SerializeField] Paint _paintsSc;
    [SerializeField] Collider2D _collisionCollider;
    [SerializeField] LayerMask _newLayer;
    public bool HasEntity()
    {
        return _currentEntity != null;
    }

    public Entity GetCurrentEntity()
    {
        return _currentEntity;
    }

    public override void InteractionWithEntity()
    {
        if(_currentEntity.TryGetComponent(out Robin chara))
        {
            if(_myrb != null)
            {
                Destroy(_myrb);
                _myrb = null;

            }
            _currentEntity = chara;
            chara.ReleaseCurrentSpawningObject();
            chara.currentSpawningObject = this;
            chara.hasObject = true;
            transform.position = chara._umbrellasPos.position;            
            _mySpriteRenderer.enabled = false;
            transform.parent = _currentEntity.transform;
            _collisionCollider.enabled = false;
            gameObject.layer = CustomTools.ToLayer(_newLayer);
            Robin.instance.SendInputToFSM(CharacterStates.Glide);
        }       
        
        //Destroy(gameObject);
    }

    public override void Delete()
    {
        base.Delete();
        if (_currentEntity != null && _currentEntity.TryGetComponent(out Character chara))
        {
            chara.currentSpawningObject = null;            
            chara.SendInputToFSM(CharacterStates.Idle);
        }
        Destroy(gameObject);
    }

    public override void Paint()
    {
        StartCoroutine(_paintsSc.PaintSprite());
    }
}
