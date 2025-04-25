using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using System.Linq;

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
    }

    [SerializeField] private List<Transform> supplyAmmoPoints = new List<Transform>();
    [SerializeField] private List<Transform> spawnPointTransforms = new List<Transform>();
    private readonly List<GameObject> _destructibleObjects = new List<GameObject>();

    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private TextMeshProUGUI numberEnemyChosenText;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    private GameObject _playerSpawn;

    private static GameManager _gameManagerInstance;
    [HideInInspector] public StateGame stateGame = StateGame.OnStart;

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
                startMenu.SetActive(false);
                gameOverMenu.SetActive(false);
                pauseMenu.SetActive(false);
                optionMenu.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(menuChoose), menuChoose, null);
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
        ShowMenu(MenuChoose.CloseMenu);
        SpawnPlayerAndEnemy();
        stateGame = StateGame.OnGame;
    }

    private void HandleOption()
    {
        ShowMenu(MenuChoose.OptionMenu);
    }

    private void HandleQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        application.Quit();
#endif
    }

    private void HandleBackToMainMenu()
    {
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
        int numberChosenEnemy =
            numberEnemyChosenText.text.All(char.IsDigit) ? int.Parse(numberEnemyChosenText.text) : 1;
        if (numberChosenEnemy >= EnemyMax) return;
        numberChosenEnemy += 1;
        numberEnemyChosenText.text = numberChosenEnemy.ToString();
    }

    private void HandleRemoveEnemy()
    {
        int numberChosenEnemy =
            numberEnemyChosenText.text.All(char.IsDigit) ? int.Parse(numberEnemyChosenText.text) : 1;
        if (numberChosenEnemy <= EnemyMin) return;
        numberChosenEnemy -= 1;
        numberEnemyChosenText.text = numberChosenEnemy.ToString();
    }

    private void HandleMute()
    {
        AudioListener.volume = 0;
    }

    private void HandleUnmute()
    {
        AudioListener.volume = 1;
    }

    private void HandleContinue()
    {
        if (stateGame != StateGame.OnPause) return;
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