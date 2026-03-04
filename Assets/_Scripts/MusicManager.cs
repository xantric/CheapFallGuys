using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public AudioSource audioSource;

    [SerializeField] private AudioClip musicClip_mainMenu;
    [SerializeField] private AudioClip musicClip_game;

    private AudioClip currentClip;
    private float currentTime;
    private bool wasPlaying;
    public enum MusicType
    {
        MainMenu,
        Game
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        currentTime = audioSource.time;
    }

    public void BeginMusic(MusicType musicType, float volume = 1f)
    {
        AudioClip musicClip = (musicType == MusicType.MainMenu) ? musicClip_mainMenu : musicClip_game;

        currentClip = musicClip;

        if (audioSource == null) return;

        if (audioSource.clip == musicClip && audioSource.isPlaying)
            return;

        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void RegisterAudioSource(AudioSource newSource)
    {
        if (audioSource != null)
        {
            currentTime = audioSource.time;
            wasPlaying = audioSource.isPlaying;
        }

        audioSource = newSource;

        if (currentClip != null)
        {
            audioSource.clip = currentClip;
            audioSource.loop = true;
            audioSource.time = currentTime;

            if (wasPlaying)
                audioSource.Play();
        }
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
    }


}
