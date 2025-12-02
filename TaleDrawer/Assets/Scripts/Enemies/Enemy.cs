using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] protected int _health;
    [SerializeField] protected EnemyType _myType;
    [SerializeField] protected LayerMask _playerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void EnemyEvent()
    {

    }
    
    protected virtual void TakeDamage(int dmg)
    {
        _health -= dmg;

        if( _health <= 0)
        {
            Debug.Log("Me muero");
            Destroy(gameObject);
        }
    }
    
}

[System.Serializable]
public enum EnemyType
{
    EventEnemy,
    ActiveEnemy,
    MiniBoss,
    Boss
}