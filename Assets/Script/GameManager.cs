using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Transform> supplyAmmoPoints = new List<Transform>();
    private static GameManager _gameManagerInstance;

    public static GameManager Instance
    {
        get
        {
            if (_gameManagerInstance == null)
            {
                _gameManagerInstance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return _gameManagerInstance;
        }
    }
    public Vector3 RandomSupplyAmmoPoint()
    {
        return supplyAmmoPoints[Random.Range(0, supplyAmmoPoints.Count)].position;
    }
}