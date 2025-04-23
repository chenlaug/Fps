using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BehaviourEnemy : MonoBehaviour
{
    [SerializeField] private BaseEnemy baseEnemy;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject weaponSimple;
    [SerializeField] private GameObject weaponMultiple;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask playerMask;

    private float _currentHealth;
    [SerializeField] private Vector3 _walkPoint;
    private const float WalkPointRange = 100.0f;
    private bool _walkPointSet;

    private void Awake()
    {
        SetUpEnemy();
    }

    private void Update()
    {
        Patrolling();
    }

    private void SetUpEnemy()
    {
        agent.speed = baseEnemy.baseSpeed;
        agent.angularSpeed = baseEnemy.baseAngularSpeed;
        agent.acceleration = baseEnemy.baseAcceleration;
        _currentHealth = baseEnemy.baseHealth;
        if (baseEnemy.baseWeaponChoose == WeaponType.SimpleShoot)
        {
            weaponSimple.SetActive(true);
            weaponMultiple.SetActive(false);
        }
        else if (baseEnemy.baseWeaponChoose == WeaponType.MultipleShoot)
        {
            weaponMultiple.SetActive(true);
            weaponSimple.SetActive(false);
        }
    }

    private void Patrolling()
    {
        if (!_walkPointSet) SearchWalkPoint();

        if (_walkPointSet) agent.SetDestination(_walkPoint);
        var distanceToWalkPoint = transform.position - _walkPoint;
        if (distanceToWalkPoint.magnitude < 2.0f) _walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        var randomZ = Random.Range(-WalkPointRange, WalkPointRange);
        var randomx = Random.Range(-WalkPointRange, WalkPointRange);

        _walkPoint = new Vector3(gameObject.transform.position.x + randomx,
            gameObject.transform.position.y,
            gameObject.transform.position.z + randomZ);
        if (Physics.Raycast(_walkPoint, -transform.up, 2f, ground)) _walkPointSet = true;
    }
}