using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAsMusicSource : MonoBehaviour
{
    private void Start()
    {
        MusicManager.Instance.RegisterAudioSource(
            GetComponent<AudioSource>()
        );
    }
}
