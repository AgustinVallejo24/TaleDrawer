using UnityEditor.SceneManagement;
using UnityEngine;

public class Umbrella : SpawningObject
{
    [SerializeField] Paint _paintsSc;
    [SerializeField] Collider2D _collisionCollider;

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
            chara.ReleaseCurrentSpawningObject();
            chara.currentSpawningObject = this;
            transform.position = chara._umbrellasPos.position;
            _mySpriteRenderer.sortingOrder = 0;            
            transform.parent = _currentEntity.transform;
            _collisionCollider.enabled = false;
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
