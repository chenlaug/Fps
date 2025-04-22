using System.Collections;
using UnityEngine;

public class BehaviourWeapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject cannonWeapon;
    [SerializeField] private BaseWeapon baseWeapon;

    private int _chamberSize;
    private int _chamberCurrent;
    private int _numberBulletLeft;

    private bool _bulletIsCreate;

    private void Start()
    {
        _chamberSize = baseWeapon.chamberSize;
        _chamberCurrent = _chamberSize;
        _numberBulletLeft = baseWeapon.numberBulletLeft;
    }

    private void Update()
    {
        Tire();
        AutoReload();
        ManuelReload();
        Debug.Log("Le nombre de balles restantes dans le chargeur est : " + _chamberCurrent);
        Debug.Log("le nombre de balles restantes : " + _numberBulletLeft);
    }

    private void CreateBullet()
    {
        if (_chamberCurrent > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = cannonWeapon.transform.position;
            bullet.GetComponent<BehaviourBullet>().Direction = cannonWeapon.transform.up;
            _chamberCurrent--;
        }
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

    private void Tire()
    {
        if (Input.GetMouseButtonDown(0) && baseWeapon.weaponType == WeaponType.SimpleShoot)
            CreateBullet();
        else if (Input.GetMouseButton(0) && baseWeapon.weaponType == WeaponType.MultipleShoot && !_bulletIsCreate)
            StartCoroutine(MultipleShootCoroutine());
    }

    private IEnumerator MultipleShootCoroutine()
    {
        CreateBullet();
        _bulletIsCreate = true;
        yield return new WaitForFixedUpdate();
        _bulletIsCreate = false;
    }
}