using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.Serialization;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
   [SerializeField] private TMP_Text playerNameText;
   private Player _player;

   public void SetUp(Player player)
   {
      _player = player;
      playerNameText.text = player.NickName;
   }

   public override void OnPlayerLeftRoom(Player otherPlayer)
   {
      if (_player == otherPlayer)
      {
         Destroy(gameObject);
      }
   }

   public override void OnLeftRoom()
   {
      Destroy(gameObject);
   }
}
