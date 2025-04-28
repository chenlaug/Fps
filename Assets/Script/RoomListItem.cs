using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameRoom;
    private RoomInfo _roomInfo;

    public void SetUp(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        nameRoom.text = roomInfo.Name;
    }

    #region OnClick

    public void OnClick()
    {
        NetworkManager.Instance.JoinRoom(_roomInfo);
    }

    #endregion
}