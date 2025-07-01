using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigameFlowController : MonoBehaviour
{
    [SerializeField] private List<MiniGameWrapper> miniGamesList;
    private Queue<MiniGameWrapper> _miniGamesQueue = new Queue<MiniGameWrapper>();
    private MiniGameWrapper _currentMiniGame;

    [Header("UI Buttons")]
    public Button startButton;
    public Button playAgainButton;
    public Button goToNextLevelButton;

    [SerializeField] private string _nextSceneName;

    private int _currentMiniGameIndex = 0;

    private void Start()
    {
        playAgainButton.onClick.AddListener(PlayAgain);
        goToNextLevelButton.onClick.AddListener(LoadNextScene);
        startButton.onClick.AddListener(StartGameFlow);
        playAgainButton.gameObject.SetActive(false);
        goToNextLevelButton.gameObject.SetActive(false);
    }

    public void StartGameFlow()
    {
        BuildMiniGameQueue();

        startButton.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
        goToNextLevelButton.gameObject.SetActive(false);

        StartMiniGame();
    }

    private void BuildMiniGameQueue()
    {
        _miniGamesQueue.Clear();
        _currentMiniGameIndex = 0;

        foreach (var miniGame in miniGamesList)
        {
            miniGame.miniGameObject.SetActive(false);
            _miniGamesQueue.Enqueue(miniGame);
        }
    }

    private void StartMiniGame()
    {
        if (_miniGamesQueue.Count == 0)
        {
            ShowEndButtons();
            return;
        }

        _currentMiniGame = _miniGamesQueue.Dequeue();
        _currentMiniGame.miniGameObject.SetActive(true);

        if (_currentMiniGame.miniGameScript is IMinigameIndex index)
        {
            index.SetupMinigame(_currentMiniGameIndex);
        }

        if (_currentMiniGame.miniGameScript is IStartable startable)
        {
            startable.OnMiniGameFinished += HandleMiniGameFinished;
            startable.StartMiniGame();
        }

        _currentMiniGameIndex++;
    }

    private void HandleMiniGameFinished()
    {
        if (_currentMiniGame.miniGameScript is IStartable startable)
        {
            startable.OnMiniGameFinished -= HandleMiniGameFinished;
        }

        _currentMiniGame.miniGameObject.SetActive(false);

        StartMiniGame();
    }

    private void ShowEndButtons()
    {
        playAgainButton.gameObject.SetActive(true);
        goToNextLevelButton.gameObject.SetActive(true);
    }

    private void PlayAgain()
    {
        playAgainButton.gameObject.SetActive(false);
        goToNextLevelButton.gameObject.SetActive(false);

        _currentMiniGameIndex = 0;
        BuildMiniGameQueue();
        StartMiniGame();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
}

[Serializable]
public class MiniGameWrapper
{
    public GameObject miniGameObject;
    public MonoBehaviour miniGameScript;
}

public interface IMinigameIndex
{
    void SetupMinigame(int minigameIndex);
}

