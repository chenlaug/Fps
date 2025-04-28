using UnityEngine;

public class BehaviourSupply : MonoBehaviour
{
    [SerializeField] private BehaviourEnemy behaviourEnemy;
    [SerializeField] private BehaviourPlayer behaviourPlayer;
    private const float RotationSpeed = 100f;
    private const float FloatAmplitude = 0.5f;
    private const float FloatFrequency = 1f;
    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    public void Update()
    {
        TurnOnSelf();
        FloatMovement();
    }

    private void TurnOnSelf()
    {
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
    }

    private void FloatMovement()
    {
        float newY = _startPosition.y + Mathf.Sin(Time.time * FloatFrequency) * FloatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            // enemy
            case 7:
                behaviourEnemy = other.GetComponentInParent<BehaviourEnemy>();
                behaviourEnemy?.RefillAmmo();
                break;
            // player
            case 8:
                behaviourPlayer = other.GetComponentInParent<BehaviourPlayer>();
                behaviourPlayer?.RefillAmmo();
                break;
        }
    }
}