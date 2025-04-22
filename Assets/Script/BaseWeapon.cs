using UnityEngine;

[CreateAssetMenu(fileName = "BaseWeapon", menuName = "Weapon")]
public class BaseWeapon : ScriptableObject
{
    public int ChamberSize;
    public int NumberBulletLeft;
}