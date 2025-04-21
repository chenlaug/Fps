using UnityEngine;

public class BehaviourWeapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private GameObject cannonWeapon;

    private void Update()
    {
        InstantiateBullet();
    }

    private void InstantiateBullet()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = cannonWeapon.transform.position;
        }
    }
}