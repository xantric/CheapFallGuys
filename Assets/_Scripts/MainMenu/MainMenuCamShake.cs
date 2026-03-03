using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float positionShakeAmount = 0.05f;
    [SerializeField] private float rotationShakeAmount = 0.5f;
    [SerializeField] private float speed = 0.5f;

    private Vector3 initPos;
    private Quaternion initRot;

    private float seed;

    private void Start()
    {
        initPos = transform.localPosition;
        initRot = transform.localRotation;

        seed = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float time = Time.time * speed;

        float x = (Mathf.PerlinNoise(seed, time) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(seed + 1, time) - 0.5f) * 2f;

        Vector3 offset = new Vector3(x, y, 0f) * positionShakeAmount;
        transform.localPosition = initPos + offset;

        float rotZ = (Mathf.PerlinNoise(seed + 2, time) - 0.5f) * 2f;
        transform.localRotation = initRot * Quaternion.Euler(0f, 0f, rotZ * rotationShakeAmount);
    }
}
