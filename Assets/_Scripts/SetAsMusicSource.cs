using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAsMusicSource : MonoBehaviour
{
    private void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        if (source == null) return;
        if (MusicManager.Instance == null) return;
        MusicManager.Instance.RegisterAudioSource(source);
    }
}
