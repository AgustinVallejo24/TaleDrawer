using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] GameObject _projectile;
    [SerializeField] Transform _spawnPos;
 
    public void ShootProjectile()
    {
        Instantiate(_projectile,_spawnPos.position, _projectile.transform.rotation);
    }
}
