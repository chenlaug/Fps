using UnityEngine;

public class BehaviourBullet : MonoBehaviour
{
    [SerializeField] private BaseBullet bullet;
    private GameObject _owner;

    private Vector3 _direction;

    public Vector3 Direction
    {
        get => _direction;
        set => _direction = value.normalized;
    }

    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        rb.velocity = _direction * (bullet.speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.root.gameObject == _owner) return;
        switch (other.gameObject.layer)
        {
            case 6: // Map
                Destroy(gameObject);
                break;
            case 7: // Enemy
                other.gameObject.GetComponentInParent<BehaviourEnemy>().TakeDamage(bullet.damage);
                Destroy(gameObject);
                break;
            case 8: // Player
                other.gameObject.GetComponentInParent<BehaviourPlayer>().TakeDamage(bullet.damage);
                Destroy(gameObject);
                break;
        }
    }
    
    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }
}