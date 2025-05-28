using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    [Header("흔들림 세기")]
    [SerializeField] private float positionShake = 0.1f;

    [Header("흔들림 속도")]
    [SerializeField] private float frequency = 1f;

    private Vector3 initialPosition;
    private float seed;

    void OnEnable()
    {
        initialPosition = transform.position;
        seed = Random.Range(0f, 100f);
    }

    void Update()
    {
        float time = Time.time * frequency;

        float offsetX = (Mathf.PerlinNoise(seed, time) - 0.5f) * 2f * positionShake;
        float offsetY = (Mathf.PerlinNoise(seed + 1, time) - 0.5f) * 2f * positionShake;
        float offsetZ = (Mathf.PerlinNoise(seed + 2, time) - 0.5f) * 2f * positionShake;

        transform.position = initialPosition + new Vector3(offsetX, offsetY, offsetZ);
    }
}
