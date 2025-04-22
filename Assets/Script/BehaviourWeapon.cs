using UnityEngine;

public class BehaviourWeapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject cannonWeapon;
    [SerializeField] private BaseWeapon baseWeapon;

    private int _chamberSize;
    private int _chamberCurrent;
    private int _numberBulletLeft;

    private void Start()
    {
        _chamberSize = baseWeapon.ChamberSize;
        _chamberCurrent = _chamberSize;
        _numberBulletLeft = baseWeapon.NumberBulletLeft;
    }

    private void Update()
    {
        InstantiateBullet();
        Reload();
        Debug.Log("Le nombre de balles restantes dans le chargeur est : " + _chamberCurrent);
        Debug.Log("le nombre de balles restantes : " + _numberBulletLeft);
    }

    private void InstantiateBullet()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var bullet = Instantiate(bulletPrefab);
            bullet.transform.position = cannonWeapon.transform.position;
            bullet.GetComponent<BehaviourBullet>().Direction = cannonWeapon.transform.up;
            _chamberCurrent--;
        }
    }

    private void Reload()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int neededAmmo = _chamberSize - _chamberCurrent;
            int ammoToReload = Mathf.Min(neededAmmo, _numberBulletLeft);

            _chamberCurrent += ammoToReload;
            _numberBulletLeft -= ammoToReload;
        }
    }
}