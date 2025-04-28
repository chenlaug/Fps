using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BehaviourWeapon : MonoBehaviour
{
    [Header("GameObject")] [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField] private GameObject cannonWeapon;
    [SerializeField] private GameObject pivotWeapon;
    [SerializeField] private GameObject owner;

    [SerializeField] private BaseWeapon baseWeapon;
    [SerializeField] private TextMeshProUGUI numberBulletUI;

    [Header("Photon")] [SerializeField] private PhotonView viewWeapon;
    private int _chamberSize;
    private int _chamberCurrent;
    public int NumberBulletLeft { get; private set; }

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
        if (!InfoGame.Instance.isLocal)
        {
            if (!viewWeapon.IsMine)
                return;   
        }
        
        HandleShoot();
    }

    private void HandleShoot()
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

        BehaviourBullet behaviourBullet = bullet.GetComponent<BehaviourBullet>();
        behaviourBullet.Direction = cannonTransform.up;
        behaviourBullet.SetOwner(owner);

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
        if (_chamberCurrent <= 0 && NumberBulletLeft > 0)
            if (InfoGame.Instance.isLocal)
            {
                Reload();
            }
            else
            {
                viewWeapon.RPC(nameof(RPC_Reload), RpcTarget.All);
            }
    }

    public void ResetAmmo()
    {
        NumberBulletLeft = baseWeapon.numberBulletLeft;
    }

    private void ManualReload()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (InfoGame.Instance.isLocal)
            {
                Reload();
            }
            else
            {
                viewWeapon.RPC(nameof(RPC_Reload), RpcTarget.All);
            }
        }
    }

    private void ShootPlayer()
    {
        if (gameObject.layer != 8 || GameManager.Instance.stateGame != GameManager.StateGame.OnGame)
            return; // 8 isn't the player and not in game
        if (Input.GetMouseButtonDown(0) && baseWeapon.weaponType == WeaponType.SimpleShoot)
        {
            GameManager.Instance.PlayAudioWanted(GameManager.AudioToPlay.FireSimple);
            if (!InfoGame.Instance.isLocal)
            {
                viewWeapon.RPC(nameof(RPC_CreateBullet), RpcTarget.All);
            }
            else
            {
                CreateBullet();
            }
        }
        else if (Input.GetMouseButton(0) && baseWeapon.weaponType == WeaponType.MultipleShoot && !_bulletIsCreate)
        {
            GameManager.Instance.PlayAudioWanted(GameManager.AudioToPlay.FireSimple);
            if (!InfoGame.Instance.isLocal)
            {
                viewWeapon.RPC(nameof(RPC_MultipleShootCoroutine), RpcTarget.All);
            }
            else
            {
                StartCoroutine(MultipleShootCoroutine());
            }
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

    #region RPC

    [PunRPC]
    private void RPC_CreateBullet()
    {
        CreateBullet();
    }

    [PunRPC]
    private void RPC_Reload()
    {
        Reload();
    }

    [PunRPC]
    private void RPC_MultipleShootCoroutine()
    {
        StartCoroutine(MultipleShootCoroutine());
    }

    #endregion

    #region IEnumerator

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

    #endregion
}