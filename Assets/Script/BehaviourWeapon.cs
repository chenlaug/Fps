using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BehaviourWeapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject cannonWeapon;
    [SerializeField] private BaseWeapon baseWeapon;
    [SerializeField] private GameObject pivotWeapon;
    [SerializeField] private TextMeshProUGUI numberBulletUI;
    private int _chamberSize;
    private int _chamberCurrent;
    [HideInInspector] public int NumberBulletLeft { get; private set; }

    private bool _bulletIsCreate;
    private bool _enemyIsAiming;

    private void Start()
    {
        _chamberSize = baseWeapon.chamberSize;
        _chamberCurrent = _chamberSize;
        NumberBulletLeft = baseWeapon.numberBulletLeft;
    }

    private void Update()
    {
        if (gameObject.layer == 8) // Player
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                ShootPlayer();
            if (Input.GetMouseButtonDown(1))
                ManualReload();
        }

        if (_chamberCurrent <= 0 && NumberBulletLeft > 0)
            AutoReload();
        ShowNumberBullet();
    }

    private void CreateBullet()
    {
        if (_chamberCurrent <= 0) return;
        GameObject bullet = Instantiate(bulletPrefab);
        Transform cannonTransform = cannonWeapon.transform;
        bullet.transform.position = cannonTransform.position;
        bullet.GetComponent<BehaviourBullet>().Direction = cannonTransform.up;
        _chamberCurrent--;
    }

    private void Reload()
    {
        if (_chamberCurrent >= _chamberSize || NumberBulletLeft <= 0)
            return;

        var neededAmmo = _chamberSize - _chamberCurrent;
        var ammoToReload = Mathf.Min(neededAmmo, NumberBulletLeft);

        _chamberCurrent += ammoToReload;
        NumberBulletLeft -= ammoToReload;
    }

    private void AutoReload()
    {
        if (_chamberCurrent <= 0 && NumberBulletLeft > 0) Reload();
    }

    public void ResetAmmo()
    {
        NumberBulletLeft = baseWeapon.numberBulletLeft;
    }

    private void ManualReload()
    {
        if (Input.GetMouseButtonDown(1)) Reload();
    }

    private void ShootPlayer()
    {
        if (gameObject.layer != 8 || GameManager.Instance.stateGame != GameManager.StateGame.OnGame)
            return; // 8 isn't the player and not in game
        if (Input.GetMouseButtonDown(0) && baseWeapon.weaponType == WeaponType.SimpleShoot)
        {
            GameManager.Instance.PlayAudioWanted(GameManager.AudioToPlay.FireSimple);
            CreateBullet();
        }
        else if (Input.GetMouseButton(0) && baseWeapon.weaponType == WeaponType.MultipleShoot && !_bulletIsCreate)
        {
            GameManager.Instance.PlayAudioWanted(GameManager.AudioToPlay.FireSimple);
            StartCoroutine(MultipleShootCoroutine());
        }
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
                GameManager.Instance.PlayAudioWanted(GameManager.AudioToPlay.FireSimple);
                StartCoroutine(SimpleShooterCoroutine());
                break;
            case WeaponType.MultipleShoot when !_bulletIsCreate:
                GameManager.Instance.PlayAudioWanted(GameManager.AudioToPlay.FireSimple);
                StartCoroutine(MultipleShootCoroutine());
                break;
            case WeaponType.None:
            default:
                break;
        }
    }

    private void ShowNumberBullet()
    {
        if (gameObject.layer == 8)
        {
            numberBulletUI.text = _chamberCurrent.ToString() + " / " + NumberBulletLeft.ToString();
        }
    }

    private IEnumerator MultipleShootCoroutine()
    {
        CreateBullet();
        _bulletIsCreate = true;
        yield return new WaitForSeconds(0.1f);
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