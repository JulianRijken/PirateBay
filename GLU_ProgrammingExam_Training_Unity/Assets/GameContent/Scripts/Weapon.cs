using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public GameObject Owner;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _weaponMuzzle;
    [SerializeField] private float _damage;
    
    public virtual void Fire()
    {
        var projectile = Instantiate(_projectilePrefab, _weaponMuzzle.position,_weaponMuzzle.rotation);
        projectile.Instigator = Owner;
        projectile.Damage = _damage;
    }
}
