using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PooledAudioSource : MonoBehaviour
{
    public AudioSource Source { get; private set; }

    private AudioPool pool;

    public void Initialize(AudioPool pool)
    {
        this.pool = pool;
        Source = GetComponent<AudioSource>();
        Source.playOnAwake = false;
    }

    public void Play(AudioClip clip, Vector3 position, bool spatial, float volume = 1f, float pitch = 1f)
    {
        transform.position = position;

        Source.spatialBlend = spatial ? 1f : 0f;
        Source.volume = volume;
        Source.pitch = pitch;
        Source.clip = clip;

        Source.Play();
    }

    public void Stop()
    {
        Source.Stop();
        Source.clip = null;
        pool.Release(this);
    }

    private void Update()
    {
        if (Source.clip != null && !Source.isPlaying)
        {
            Source.clip = null;
            pool.Release(this);
        }
    }
}