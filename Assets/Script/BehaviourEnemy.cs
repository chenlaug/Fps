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
    private Vector3 _walkPoint;
    private const float WalkPointRange = 100.0f;
    private bool _walkPointSet;

    private float _sightRange = 30.0f;
    private float _attackRange = 8.0f;
    private bool _playerIsinSight;
    private bool _playerIsInAttackRange;

    private void Awake()
    {
        SetUpEnemy();
    }

    private void Update()
    {
        CheckSignt();
        CheckStateEnemy();
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

    private void CheckSignt()
    {
        _playerIsinSight = Physics.CheckSphere(gameObject.transform.position, _sightRange, playerMask);
        _playerIsInAttackRange = Physics.CheckSphere(gameObject.transform.position, _attackRange, playerMask);
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(gameObject.transform.position);
        gameObject.transform.LookAt(player.position);
    }

    private void CheckStateEnemy()
    {
        if (!_playerIsinSight && !_playerIsInAttackRange) Patrolling();
        if (_playerIsinSight && !_playerIsInAttackRange) ChasePlayer();
        if (_playerIsinSight && _playerIsInAttackRange) AttackPlayer();
    }
}