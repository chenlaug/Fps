using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BehaviourWeapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject cannonWeapon;
    [SerializeField] private BaseWeapon baseWeapon;
    [SerializeField] private GameObject pivotWeapon;


    private int _chamberSize;
    private int _chamberCurrent;
    private int _numberBulletLeft;

    private bool _bulletIsCreate;
    private bool _enemyIsAiming;

    private void Start()
    {
        _chamberSize = baseWeapon.chamberSize;
        _chamberCurrent = _chamberSize;
        _numberBulletLeft = baseWeapon.numberBulletLeft;
    }

    private void Update()
    {
        ShootPlayer();
        AutoReload();
        ManuelReload();
    }

    private void CreateBullet()
    {
        if (_chamberCurrent <= 0) return;
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = cannonWeapon.transform.position;
        bullet.GetComponent<BehaviourBullet>().Direction = cannonWeapon.transform.up;
        _chamberCurrent--;
    }

    private void Reload()
    {
        if (_chamberCurrent >= _chamberSize || _numberBulletLeft <= 0)
            return;

        var neededAmmo = _chamberSize - _chamberCurrent;
        var ammoToReload = Mathf.Min(neededAmmo, _numberBulletLeft);

        _chamberCurrent += ammoToReload;
        _numberBulletLeft -= ammoToReload;
    }

    private void AutoReload()
    {
        if (_chamberCurrent <= 0 && _numberBulletLeft > 0) Reload();
    }

    private void ManuelReload()
    {
        if (Input.GetMouseButtonDown(1)) Reload();
    }

    private void ShootPlayer()
    {
        if (gameObject.layer != 8) return; // 8 isn't the player 
        if (Input.GetMouseButtonDown(0) && baseWeapon.weaponType == WeaponType.SimpleShoot)
            CreateBullet();
        else if (Input.GetMouseButton(0) && baseWeapon.weaponType == WeaponType.MultipleShoot && !_bulletIsCreate)
            StartCoroutine(MultipleShootCoroutine());
    }

    public void ShootEnemy()
    {
        if (gameObject.layer != 7) return; // 7 isn't the Enemy 
        if (!_enemyIsAiming)
        {
            StartCoroutine(RadomAim());
        }
        switch (baseWeapon.weaponType)
        {
            case WeaponType.SimpleShoot when !_bulletIsCreate:
                StartCoroutine(SimpleShooterCoroutine());
                break;
            case WeaponType.MultipleShoot when !_bulletIsCreate:
                StartCoroutine(MultipleShootCoroutine());
                break;
            case WeaponType.None:
            default:
                break;
        }
    }

    private IEnumerator MultipleShootCoroutine()
    {
        CreateBullet();
        _bulletIsCreate = true;
        yield return new WaitForFixedUpdate();
        _bulletIsCreate = false;
    }
    
    private IEnumerator SimpleShooterCoroutine()
    {
        CreateBullet();
        _bulletIsCreate = true;
        yield return new WaitForSeconds(1.0f);
        _bulletIsCreate = false;
    }

    private IEnumerator RadomAim()
    {
        if (pivotWeapon != null)
        {
            pivotWeapon.transform.localRotation = Quaternion.Euler(0.0f, Random.Range(-15, 5), 0.0f);
            _enemyIsAiming = true;
            yield return new WaitForSeconds(1f);
            _enemyIsAiming = false;
        }
    }
}