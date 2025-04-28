using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BehaviourPlayer : MonoBehaviour, IPunObservable
{
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

    [Header("Player")] [SerializeField] private Camera playerCamera;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();

    [FormerlySerializedAs("endText")] [Header("Ui")] [SerializeField]
    private TextMeshProUGUI messageEndText;

    [Header("Life")] [SerializeField] private int currentLife;
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject ammoInfoGameObject;

    [Header("Photon")] [SerializeField] public PhotonView playerView;

    private void Awake()
    {
        currentLife = BaseLife;
    }

    private void Start()
    {
        if (!InfoGame.Instance.isLocal)
        {
            if (!playerView.IsMine)
            {
                playerCamera.enabled = false;
                ammoInfoGameObject.SetActive(false);
                healthBar.gameObject.SetActive(false);
            }    
        }
    }

    private void Update()
    {
        if (!InfoGame.Instance.isLocal)
        {
            if (!playerView.IsMine || GameManager.Instance.stateGame == GameManager.StateGame.OnGameOver)
                return;     
        }
      
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

        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0.0f, 0.0f);
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
        if (weapons.Count <= 0 || !Input.GetKeyDown(KeyCode.Q)) return;
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

    public void RefillAmmo()
    {
        foreach (var go in weapons)
        {
            go.GetComponent<BehaviourWeapon>().ResetAmmo();
        }
    }

    public void TakeDamage(int damage)
    {
        currentLife -= damage;
        healthBar.fillAmount = currentLife / (float)BaseLife;
        if (!InfoGame.Instance.isLocal)
        {
            if (GameManager.Instance.stateGame == GameManager.StateGame.OnGame && currentLife <= 0)
            {
 
                playerView.RPC(nameof(RPC_ShowEndMessage), RpcTarget.Others);
                messageEndText.gameObject.SetActive(true);
                messageEndText.color = Color.red;
                messageEndText.text = "You are Dead";
                playerView.RPC(nameof(RPC_EndGame), RpcTarget.All);

            } 
        }
        else
        {
            GameManager.Instance.CheckGameOver(currentLife, gameObject);
        }
        GameManager.Instance.PlayAudioWanted(GameManager.AudioToPlay.HitMarker);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(weapons[0].activeSelf);
            stream.SendNext(weapons[1].activeSelf);
            stream.SendNext(currentLife);
            stream.SendNext(healthBar.fillAmount);
        }
        else
        {
            weapons[0].SetActive((bool)stream.ReceiveNext());
            weapons[1].SetActive((bool)stream.ReceiveNext());
            currentLife = (int)stream.ReceiveNext();
            healthBar.fillAmount = (float)stream.ReceiveNext();
        }
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
    
    #region RPC

    [PunRPC]
    private void RPC_EndGame()
    {
        GameManager.Instance.stateGame = GameManager.StateGame.OnGameOver;
        GameManager.Instance.HandleMultiEndGame();
    }
    
    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {
        if (!playerView.IsMine)
        {
            return;
        }

        TakeDamage(damage);
    }

    [PunRPC]
    public void RPC_ShowEndMessage()
    {
        messageEndText.color = Color.green;
        messageEndText.text = "You have Won";
        messageEndText.gameObject.SetActive(true);
    }
    #endregion
}