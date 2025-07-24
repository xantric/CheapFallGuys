// SoundManager.cs
using UnityEngine;
using Photon.Pun;

public class SoundManager : MonoBehaviourPun
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources & Clips")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip countdownTickClip;
    [SerializeField] private AudioClip raceStartClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>Play a single countdown tick.</summary>
    [PunRPC]
    public void PlayCountdownTick()
    {
        audioSource.PlayOneShot(countdownTickClip);
    }

    /// <summary>Play the race start sound.</summary>
    [PunRPC]
    public void PlayRaceStart()
    {
        audioSource.PlayOneShot(raceStartClip);
    }

    /// <summary>Play win sound locally.</summary>
    public void PlayWin()
    {
        audioSource.PlayOneShot(winClip);
    }

    /// <summary>Play lose sound locally.</summary>
    public void PlayLose()
    {
        audioSource.PlayOneShot(loseClip);
    }
}
