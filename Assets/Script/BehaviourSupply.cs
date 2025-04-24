using UnityEngine;

public class BehaviourSupply : MonoBehaviour
{
    [SerializeField] private BehaviourEnemy behaviourEnemy;
    [SerializeField] private BehaviourPlayer behaviourPlayer;

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