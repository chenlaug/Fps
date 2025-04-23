using UnityEngine;

[CreateAssetMenu(fileName = "BaseEnemy", menuName = "Enemy")]
public class BaseEnemy : ScriptableObject
{
    public int baseHealth;
    public int currentBaseHealth;
    public float baseSpeed;
    public float baseAcceleration;
    public float baseAngularSpeed;
    public WeaponType baseWeaponChoose;
}