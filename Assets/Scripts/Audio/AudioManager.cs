using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] AudioSO data;
    [SerializeField] AudioPool Pool;

    private AudioSource uiSource;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        uiSource = gameObject.AddComponent<AudioSource>();
    }

    public static PooledAudioSource Play(AudioType type) => instance.Pool.Play(instance.data.GetClip(type));
    public static PooledAudioSource PlayAt(AudioType type, Vector3 pos) => instance.Pool.PlayAt(instance.data.GetClip(type), pos);
    public static PooledAudioSource PlayAttached(AudioType type, Transform transform) => instance.Pool.PlayAttached(instance.data.GetClip(type), transform);
    //public void Stop(AudioType type) => Pool.Stop(data.GetClip(type));

    //public void Click() => uiSource.PlayOneShot(click);
}
