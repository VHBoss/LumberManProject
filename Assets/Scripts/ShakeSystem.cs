using UnityEngine;

public class ShakeSystem : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform furnace;
    public Transform targetCamera;

    [Header("Shake Settings")]
    public float shakeDistance = 3f;
    public float maxAmplitude = 0.1f;
    public float frequency = 8f;

    private Vector3 startPos;
    private float seedX;
    private float seedY;

    void Start()
    {
        startPos = targetCamera.localPosition;

        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(0f, 100f);
    }

    void Update()
    {
        float distanceX = furnace.position.x - player.position.x;

        // Если печь далеко или уже впереди игрока
        if (distanceX > shakeDistance || distanceX < 0f)
        {
            targetCamera.localPosition = Vector3.Lerp(
                targetCamera.localPosition,
                startPos,
                Time.deltaTime * 8f);

            return;
        }

        // Интенсивность 0..1
        float intensity = Mathf.InverseLerp(shakeDistance, 0f, distanceX);

        float x = (Mathf.PerlinNoise(seedX, Time.time * frequency) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(seedY, Time.time * frequency) - 0.5f) * 2f;

        Vector3 offset = new Vector3(x, y, 0f) * maxAmplitude * intensity;

        targetCamera.localPosition = startPos + offset;
    }
}
