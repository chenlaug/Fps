using UnityEngine;

[CreateAssetMenu(fileName = "BaseBullet", menuName = "Bullet")]
public class BaseBullet : ScriptableObject
{ 
    [SerializeField]
    public int damage;
    [SerializeField]
    public float speed;
}
