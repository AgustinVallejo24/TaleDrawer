using UnityEngine;

public class Umbrella : SpawningObject
{
    [SerializeField] Paint paintsSc;

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
            chara.ReleaseCurrentSpawningObject();
            chara.currentSpawningObject = this;
            transform.position = chara._umbrellasPos.position;
            _mySpriteRenderer.sortingOrder = 0;            
            transform.parent = _currentEntity.transform;
            Robin.instance.SendInputToFSM(CharacterStates.Glide);
        }       
        
        //Destroy(gameObject);
    }

    public override void Delete()
    {
        base.Delete();
        if (_currentEntity.TryGetComponent(out Character chara))
        {
            chara.ReleaseCurrentSpawningObject();
            chara.SendInputToFSM(CharacterStates.Idle);
        }
        Destroy(gameObject);
    }

    public override void Paint()
    {
        StartCoroutine(paintsSc.PaintSprite());
    }
}
