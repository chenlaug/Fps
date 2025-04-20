using UnityEngine;

public class BehaviourPlayer : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private LayerMask groundMask;
    
    private float _xRotation;
    private float _mouseSensibility = 100f;
    
    private float _groundDistance = 0.4f;
    private readonly float _gravity = -9.81f;
    private bool _isGrounded;
    private float _jumpHeight = 3.0f;
    private Vector3 _velocity;
    
    private readonly float _limitRotationX = 70.0f;
    private readonly float _moveSpeed = 10.0f;

    private void Update()
    {
        BehaviourPlayerLook();
        BehaviourPlayerMove();
        CheckGround();
        HandleJump();
    }
    private void BehaviourPlayerLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensibility * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensibility * Time.deltaTime;
        
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_limitRotationX, _limitRotationX);
        
        camera.transform.localRotation = Quaternion.Euler(_xRotation,0.0f,0.0f);
        gameObject.transform.Rotate(Vector3.up * mouseX);
    }
    private void BehaviourPlayerMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = gameObject.transform.right * moveX + gameObject.transform.forward * moveZ;
        characterController.Move(move * (_moveSpeed * Time.deltaTime));
    }

    private void CheckGround()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.transform.position,_groundDistance,groundMask);
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2.0f;
        }
        
        _velocity.y += _gravity * Time.deltaTime;
        characterController.Move(_velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
        }
    }
}
