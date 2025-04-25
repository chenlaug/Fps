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

    [SerializeField] private int currentHealth;
    private Vector3 _walkPoint;
    private const float WalkPointRange = 100.0f;
    private bool _walkPointSet;

    private const float SightRange = 30.0f;
    private const float AttackRange = 8.0f;
    private bool _playerIsinSight;
    private bool _playerIsInAttackRange;
    private BehaviourWeapon _behavioursWeapon;
    private Vector3 _supplyAmmoPoints;

    private void Awake()
    {
        SetUpEnemy();
    }
    private void Update()
    {
        CheckSign();
        CheckStateEnemy();
    }
    private void SetUpEnemy()
    {
        agent.speed = baseEnemy.baseSpeed;
        agent.angularSpeed = baseEnemy.baseAngularSpeed;
        agent.acceleration = baseEnemy.baseAcceleration;
        currentHealth = baseEnemy.baseHealth;
        _supplyAmmoPoints = GameManager.Instance.RandomSupplyAmmoPoint();
        weaponSimple.SetActive(false);
        weaponMultiple.SetActive(false);
        _behavioursWeapon = null;

        switch (baseEnemy.baseWeaponChoose)
        {
            case WeaponType.SimpleShoot:
                weaponSimple.SetActive(true);
                weaponMultiple.SetActive(false);
                _behavioursWeapon = weaponSimple.GetComponent<BehaviourWeapon>();
                break;
            case WeaponType.MultipleShoot:
                weaponMultiple.SetActive(true);
                weaponSimple.SetActive(false);
                _behavioursWeapon = weaponMultiple.GetComponent<BehaviourWeapon>();
                break;
            case WeaponType.None:
            default:
                break;
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
    private void CheckSign()
    {
        _playerIsinSight = Physics.CheckSphere(gameObject.transform.position, SightRange, playerMask);
        _playerIsInAttackRange = Physics.CheckSphere(gameObject.transform.position, AttackRange, playerMask);
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        agent.SetDestination(gameObject.transform.position);
        gameObject.transform.LookAt(player.position);
        _behavioursWeapon.ShootEnemy();
    }
    private void SearchSupplyAmmo()
    {
        agent.SetDestination(_supplyAmmoPoints);
    }
    private void CheckStateEnemy()
    {
        if (_behavioursWeapon.NumberBulletLeft > 0)
        {
            if (!_playerIsinSight && !_playerIsInAttackRange) Patrolling();
            if (_playerIsinSight && !_playerIsInAttackRange) ChasePlayer();
            if (_playerIsinSight && _playerIsInAttackRange) AttackPlayer();
        }
        else
        {
            SearchSupplyAmmo();
        }
    }
    public void RefillAmmo()
    {
        _behavioursWeapon.ResetAmmo();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}