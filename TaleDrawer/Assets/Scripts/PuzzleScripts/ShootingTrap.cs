using UnityEngine;

public class ShootingTrap : Trap
{
    [SerializeField] GameObject _projectile;
    [SerializeField] Transform _spawnPos;

    public override void Activation()
    {
        ShootProjectile();
    }
    public void ShootProjectile()
    {
        Instantiate(_projectile,_spawnPos.position, _projectile.transform.rotation);
    }
}
