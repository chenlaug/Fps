using UnityEngine;

public enum WeaponType
{
    SimpleShoot,
    MultipleShoot
}

[CreateAssetMenu(fileName = "BaseWeapon", menuName = "Weapon")]
public class BaseWeapon : ScriptableObject
{
    public int chamberSize;
    public int numberBulletLeft;
    public WeaponType weaponType;
}