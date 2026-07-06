using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour
{
    [SerializeField] int poolSize = 20;
    [SerializeField] int minDistance = 10;
    [SerializeField] int maxDistance = 20;

    public bool debugLog;

    private readonly Queue<PooledAudioSource> free = new();
    private readonly List<PooledAudioSource> active = new();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = new($"Audio_{i}");
            go.transform.SetParent(transform);

            AudioSource source = go.AddComponent<AudioSource>();
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;

            PooledAudioSource pooled = go.AddComponent<PooledAudioSource>();

            pooled.Initialize(this);

            free.Enqueue(pooled);
        }
    }

    public PooledAudioSource Play(AudioData data)
    {
        return PlayAt(data, Vector3.zero);
    }

    public PooledAudioSource PlayAt(AudioData data, Vector3 position)
    {
#if UNITY_EDITOR
        if (debugLog)
            Debug.Log($"Playing audio {data.type}");
#endif

        PooledAudioSource source = GetFreeSource();

        active.Add(source);
        source.Play(data.GetClip(), position, data.sfx3D, data.mute ? 0f : data.volume);

        return source;
    }

    public PooledAudioSource PlayAttached(AudioData data, Transform target)
    {
#if UNITY_EDITOR
        if (debugLog)
            Debug.Log($"Playing audio {data.type}");
#endif

        var source = GetFreeSource();

        active.Add(source);

        source.transform.SetParent(target, false);
        source.transform.localPosition = Vector3.zero;

        source.Play(data.GetClip(), target.position, data.sfx3D, data.mute ? 0f : data.volume);

        return source;
    }

    PooledAudioSource GetFreeSource()
    {
        if (free.Count > 0)
            return free.Dequeue();

        // Если пул закончился — переиспользуем самый старый источник
        PooledAudioSource oldest = active[0];
        oldest.Stop();

        return free.Dequeue();
    }

    public void Release(PooledAudioSource source)
    {
        source.transform.SetParent(transform);
        active.Remove(source);
        free.Enqueue(source);
    }
}