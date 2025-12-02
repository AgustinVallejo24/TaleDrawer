using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Monkey : Enemy
{
    [Header("References")]
    [SerializeField] bool _hasHelmet;
    [SerializeField] GameObject _helmet;
    [SerializeField] Vector3 _areaSize;
    [SerializeField] Character _character;
    [SerializeField] SpawnableObjectType _dangerousObjects;

    private IEnumerator Start()
    {        
        if( _hasHelmet)
        {
            _helmet.SetActive(true);
        }

        while (true)
        {
            if(Physics2D.OverlapArea(transform.position - _areaSize, transform.position + _areaSize, _playerMask))
            {
                EnemyEvent();
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    protected override void EnemyEvent()
    {
        GameManager.instance.StateChanger(SceneStates.GameOver);
        _character.SendInputToFSM(CharacterStates.Stop);
        _character._currentPath = null;
        _character.characterRigidbody.linearVelocity = Vector3.zero;
        if(_character.transform.position.x >=  transform.position.x)
        {
            transform.DOMoveX(_character.transform.position.x - 1, 1).OnComplete(() => { Debug.LogError("GameOver"); });
        }
        else
        {
            transform.DOMoveX(_character.transform.position.x + 1, 1).OnComplete(() => { Debug.LogError("GameOver"); });
        }
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out SpawningObject obj) && _dangerousObjects.HasFlag(obj.myType))
        {
            Destroy(obj.gameObject);

            if (_hasHelmet)
            {
                Debug.Log("No me hace nada");
            }
            else
            {
                TakeDamage(2);
                
            }
                
        }
        
    }

}
