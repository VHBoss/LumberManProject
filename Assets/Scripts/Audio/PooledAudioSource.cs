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

    public void Play(AudioData data, Vector3 position)
    {
        transform.position = position;

        Source.spatialBlend = data.sfx3D ? 1f : 0f;
        Source.volume = data.mute ? 0f : data.volume;
        Source.pitch = data.pitch;
        Source.loop = data.loop;
        Source.clip = data.GetClip();

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