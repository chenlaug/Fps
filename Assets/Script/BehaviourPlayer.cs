using UnityEngine;

public class BehaviourPlayer : MonoBehaviour
{
    [SerializeField] private new Camera camera;

    private float _xRotation;
    private float _mouseSensibility = 100f;
    private readonly float _limiteRotationX = 70.0f;

    private void Update()
    {
        BehaviourPlayerLook();
    }

    private void BehaviourPlayerLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensibility * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensibility * Time.deltaTime;
        
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_limiteRotationX, _limiteRotationX);
        
        camera.transform.localRotation = Quaternion.Euler(_xRotation,0.0f,0.0f);
        gameObject.transform.Rotate(Vector3.up * mouseX);
    }
}
