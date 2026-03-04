// GameManager.cs
using StarterAssets;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Win/Lose UI")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endText;
    [SerializeField] private float freezeTimeScale = 0f;

    [Header("PauseUI")]
    [SerializeField] private GameObject pauseUIPanel;

    private bool _ended = false;
    private bool _paused = false;
    private bool _canPause = false;
    private bool _gameEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (endPanel != null)
            endPanel.SetActive(false);

        if (pauseUIPanel != null)
            pauseUIPanel.SetActive(false);
    }

    private void Update() {
        _canPause = !_gameEnded;
        if (_canPause && Input.GetKeyDown(KeyCode.Escape)) {
            if(!_paused) PauseGame();
            else ResumeGame();
        }
    }

    /// <summary>
    /// Show the win message.
    /// </summary>
    public void WinGame(string message)
    {
        _gameEnded = true;
        SoundManager.Instance.PlayWin();
        ShowEnd(message);
    }

    /// <summary>
    /// Show the lose message.
    /// </summary>
    public void LoseGame(string message)
    {
        _gameEnded = true;
        SoundManager.Instance.PlayLose();
        ShowEnd(message);
    }

    /// <summary>
    /// Displays end UI, unlocks cursor, disables player GameObject, and pauses time.
    /// </summary>
    private void ShowEnd(string message)
    {
        if (_ended) return;
        _ended = true;

        // Display the message
        if (endText != null)
            endText.text = message;
        if (endPanel != null)
            endPanel.SetActive(true);

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable the entire local player GameObject
        // Assumes your player has a ThirdPersonController component on its root
        var tpc = FindObjectOfType<ThirdPersonController>();
        if (tpc != null)
            tpc.gameObject.SetActive(false);

        // Pause the game
        Time.timeScale = freezeTimeScale;
    }

    /// <summary>
    /// Restart the current level.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame() {
        Time.timeScale = freezeTimeScale;
        pauseUIPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _paused = true;
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
        pauseUIPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        _paused = false;
    }
}
