using UnityEngine;

public class InfoGame : MonoBehaviour
{
    private static InfoGame _instance;
    public bool isLocal;

    public static InfoGame Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(InfoGame)) as InfoGame;
            }

            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnClickInfo(bool getInfoGame)
    {
        isLocal = getInfoGame;
    }
}
