using UnityEngine;

[CreateAssetMenu(fileName = "BaseBullet", menuName = "Bullet")]
public class BaseBullet : ScriptableObject
{ 
    [SerializeField]
    protected int damage;
    [SerializeField]
    protected float speed;
}
