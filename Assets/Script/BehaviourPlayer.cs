using System.Collections.Generic;
using UnityEngine;

public class BehaviourPlayer : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();

    private float _xRotation;
    private const float MouseSensibility = 100f;

    private const float GroundDistance = 0.4f;
    private const float Gravity = -9.81f;
    private bool _isGrounded;
    private const float JumpHeight = 3.0f;
    private Vector3 _velocity;

    private const float LimitRotationX = 70.0f;
    private const float MoveSpeed = 10.0f;
    private const int BaseLife = 1000;
    [SerializeField]private int _currentLife;

    private void Awake()
    {
        _currentLife = BaseLife;
    }

    private void Update()
    {
        BehaviourPlayerLook();
        BehaviourPlayerMove();
        CheckGround();
        HandleJump();
        SwitchWeapon();
    }

    private void BehaviourPlayerLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensibility * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensibility * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -LimitRotationX, LimitRotationX);

        camera.transform.localRotation = Quaternion.Euler(_xRotation, 0.0f, 0.0f);
        gameObject.transform.Rotate(Vector3.up * mouseX);
    }

    private void BehaviourPlayerMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = gameObject.transform.right * moveX + gameObject.transform.forward * moveZ;
        characterController.Move(move * (MoveSpeed * Time.deltaTime));
    }

    private void CheckGround()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.transform.position, GroundDistance, groundMask);
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2.0f;
        }

        _velocity.y += Gravity * Time.deltaTime;
        characterController.Move(_velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2.0f * Gravity);
        }
    }

    private void SwitchWeapon()
    {
        if (weapons.Count > 0 && Input.GetKeyDown(KeyCode.Q))
        {
            if (weapons[0].activeSelf)
            {
                weapons[0].SetActive(false);
                weapons[1].SetActive(true);
            }
            else if (weapons[1].activeSelf)
            {
                weapons[1].SetActive(false);
                weapons[0].SetActive(true);
            }
        }
    }

    public void RefillAmmo()
    {
        foreach (var go in weapons)
        {
            go.GetComponent<BehaviourWeapon>().ResetAmmo();
        }
    }

    public void TakeDamage(int damage)
    {
        _currentLife -= damage;
    }
}