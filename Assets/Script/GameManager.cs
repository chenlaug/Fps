using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using System.Linq;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private enum MenuChoose
    {
        StartMenu,
        GameOverMenu,
        PauseMenu,
        OptionMenu,
        CloseMenu,
    }

    private const int EnemyMax = 4;
    private const int EnemyMin = 1;

    public enum StateGame
    {
        OnStart,
        OnGame,
        OnPause,
        OnOption,
        OnGameOver,
    }

    public enum AudioToPlay
    {
        MainTheme,
        FireSimple,
        FireMultiple,
        HitMarker,
        Click,
    }

    [Header("Transform")] [SerializeField] private List<Transform> supplyAmmoPoints = new List<Transform>();
    [SerializeField] private List<Transform> spawnPointTransforms = new List<Transform>();
    private GameObject _playerSpawn;
    public GameObject PlayerSpawn => _playerSpawn;


    [Header("Audio")] [SerializeField] private AudioSource mainThemeAudio;
    [SerializeField] private AudioSource fireSimpleAudio;
    [SerializeField] private AudioSource fireMultipleAudio;
    [SerializeField] private AudioSource hitMarkerAudioAudio;
    [SerializeField] private AudioSource clickAudio;

    private readonly List<GameObject> _destructibleObjects = new List<GameObject>();

    [Header("Canvas Menu")] [SerializeField]
    private GameObject startMenu;

    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private TextMeshProUGUI numberEnemyChosenText;
    [Header("Prefab")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    private static GameManager _gameManagerInstance;
    [HideInInspector] public StateGame stateGame = StateGame.OnStart;

    private void Awake()
    {
        PlayAudioWanted(AudioToPlay.MainTheme);
    }

    private void Start()
    {
        ShowCursor();
    }

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

    private void Update()
    {
        HandlePause();
    }

    public Vector3 RandomSupplyAmmoPoint()
    {
        return supplyAmmoPoints[Random.Range(0, supplyAmmoPoints.Count)].position;
    }

    private void ShowMenu(MenuChoose menuChoose)
    {
        switch (menuChoose)
        {
            case MenuChoose.StartMenu:
                ShowCursor();
                startMenu.SetActive(true);
                gameOverMenu.SetActive(false);
                pauseMenu.SetActive(false);
                optionMenu.SetActive(false);
                break;
            case MenuChoose.GameOverMenu:
                startMenu.SetActive(false);
                gameOverMenu.SetActive(true);
                pauseMenu.SetActive(false);
                optionMenu.SetActive(false);
                break;
            case MenuChoose.PauseMenu:
                ShowCursor();
                startMenu.SetActive(false);
                gameOverMenu.SetActive(false);
                pauseMenu.SetActive(true);
                optionMenu.SetActive(false);
                break;
            case MenuChoose.OptionMenu:
                startMenu.SetActive(false);
                gameOverMenu.SetActive(false);
                pauseMenu.SetActive(false);
                optionMenu.SetActive(true);
                break;
            case MenuChoose.CloseMenu:
                HideCursor();
                startMenu.SetActive(false);
                gameOverMenu.SetActive(false);
                pauseMenu.SetActive(false);
                optionMenu.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(menuChoose), menuChoose, null);
        }
    }

    public void PlayAudioWanted(AudioToPlay audioToPlay)
    {
        switch (audioToPlay)
        {
            case AudioToPlay.MainTheme:
                mainThemeAudio.time = 0.0f;
                mainThemeAudio.Play();
                break;
            case AudioToPlay.FireSimple:
                fireSimpleAudio.time = 0.0f;
                fireSimpleAudio.Play();
                break;
            case AudioToPlay.FireMultiple:
                if (!fireMultipleAudio.isPlaying)
                    fireMultipleAudio.Play();
                break;
            case AudioToPlay.HitMarker:
                if (!hitMarkerAudioAudio.isPlaying)
                    hitMarkerAudioAudio.Play();
                break;
            case AudioToPlay.Click:
                clickAudio.time = 0.0f;
                clickAudio.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(audioToPlay), audioToPlay, null);
        }
    }

    private void SpawnPlayerAndEnemy()
    {
        List<Transform> spawnPointsTemps = spawnPointTransforms.ToList();

        _playerSpawn = Instantiate(playerPrefab);
        _destructibleObjects.Add(_playerSpawn);
        int randomPosition = Random.Range(0, spawnPointsTemps.Count);
        _playerSpawn.transform.position = spawnPointsTemps[randomPosition].position;
        spawnPointsTemps.RemoveAt(randomPosition);

        int numberChosenEnemy =
            numberEnemyChosenText.text.All(char.IsDigit) ? int.Parse(numberEnemyChosenText.text) : 1;
        for (int i = 0; i < numberChosenEnemy; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            _destructibleObjects.Add(enemy);
            randomPosition = Random.Range(0, spawnPointsTemps.Count);
            enemy.transform.position = spawnPointsTemps[randomPosition].position;
            spawnPointsTemps.RemoveAt(randomPosition);
        }
    }

    private void HandleStart()
    {
        PlayAudioWanted(AudioToPlay.Click);
        ShowMenu(MenuChoose.CloseMenu);
        SpawnPlayerAndEnemy();
        stateGame = StateGame.OnGame;
    }

    private void HandleOption()
    {
        PlayAudioWanted(AudioToPlay.Click);
        ShowMenu(MenuChoose.OptionMenu);
    }

    private void HandleQuitGame()
    {
        PlayAudioWanted(AudioToPlay.Click);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        application.Quit();
#endif
    }

    private void HandleBackToMainMenu()
    {
        PlayAudioWanted(AudioToPlay.Click);
        DestructibleAllPlayerAndEnemy();
        ShowMenu(MenuChoose.StartMenu);
        stateGame = StateGame.OnStart;
        Time.timeScale = 1;
    }

    private void DestructibleAllPlayerAndEnemy()
    {
        if (stateGame == StateGame.OnStart) return;
        foreach (GameObject go in _destructibleObjects)
        {
            Destroy(go);
        }

        _destructibleObjects.Clear();
    }

    private void HandleAddEnemy()
    {
        PlayAudioWanted(AudioToPlay.Click);
        int numberChosenEnemy =
            numberEnemyChosenText.text.All(char.IsDigit) ? int.Parse(numberEnemyChosenText.text) : 1;
        if (numberChosenEnemy >= EnemyMax) return;
        numberChosenEnemy += 1;
        numberEnemyChosenText.text = numberChosenEnemy.ToString();
    }

    private void HandleRemoveEnemy()
    {
        PlayAudioWanted(AudioToPlay.Click);
        int numberChosenEnemy =
            numberEnemyChosenText.text.All(char.IsDigit) ? int.Parse(numberEnemyChosenText.text) : 1;
        if (numberChosenEnemy <= EnemyMin) return;
        numberChosenEnemy -= 1;
        numberEnemyChosenText.text = numberChosenEnemy.ToString();
    }

    private void HandleMute()
    {
        PlayAudioWanted(AudioToPlay.Click);
        AudioListener.volume = 0;
    }

    private void HandleUnmute()
    {
        PlayAudioWanted(AudioToPlay.Click);
        AudioListener.volume = 1;
    }

    private void HandleContinue()
    {
        if (stateGame != StateGame.OnPause) return;
        PlayAudioWanted(AudioToPlay.Click);
        stateGame = StateGame.OnGame;
        ShowMenu(MenuChoose.CloseMenu);
        Time.timeScale = 1;
    }

    private void HandlePause()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) || stateGame != StateGame.OnGame) return;
        ShowMenu(MenuChoose.PauseMenu);
        stateGame = StateGame.OnPause;
        Time.timeScale = 0;
    }

    private void HandleGameOver()
    {
        ShowCursor();
        stateGame = StateGame.OnGameOver;
        ShowMenu(MenuChoose.GameOverMenu);
        DestructibleAllPlayerAndEnemy();
    }

    public void CheckGameOver(int currentLifeObject, GameObject myObject)
    {
        if (stateGame != StateGame.OnGame || currentLifeObject > 0) return;
        switch (myObject.layer)
        {
            case 8:
                HandleGameOver();
                Debug.Log("Player has lose");
                break;
            case 7:
            {
                _destructibleObjects.Remove(myObject);
                Destroy(myObject);
                if (_destructibleObjects.Count < 2)
                {
                    HandleGameOver();
                    Debug.Log("Player has win");
                }

                break;
            }
        }
    }

    private static void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private static void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #region OnClick Functions

    public void OnClickStart()
    {
        HandleStart();
    }

    public void OnClickOption()
    {
        HandleOption();
    }

    public void OnClickBackToMainMenu()
    {
        HandleBackToMainMenu();
    }

    public void OnClickQuitGame()
    {
        HandleQuitGame();
    }

    public void OnClickAddEnemy()
    {
        HandleAddEnemy();
    }

    public void OnClickRemoveEnemy()
    {
        HandleRemoveEnemy();
    }

    public void OnClickMute()
    {
        HandleMute();
    }

    public void OnClickUnmute()
    {
        HandleUnmute();
    }

    public void OnClickContinue()
    {
        HandleContinue();
    }

    #endregion
}