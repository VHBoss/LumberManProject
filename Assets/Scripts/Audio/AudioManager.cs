using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] AudioSO data;
    [SerializeField] AudioPool Pool;

    [Header("Ambient")]
    [SerializeField] AudioType sfxAmbient;
    //[SerializeField] AudioType sfxMusic;

    private AudioSource uiSource;
    private AudioSource ambientSource;
    //private AudioSource musicSource;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        uiSource = gameObject.AddComponent<AudioSource>();
        ambientSource = gameObject.AddComponent<AudioSource>();
        //musicSource = gameObject.AddComponent<AudioSource>();

        if (sfxAmbient != AudioType.None)
        {
            var audio = data.GetClip(sfxAmbient);
            ambientSource.volume = audio.mute ? 0f : audio.volume;
            ambientSource.pitch = audio.pitch;
            ambientSource.loop = true;
            ambientSource.clip = audio.GetClip();
            ambientSource.Play();
        }
        //if (sfxMusic != AudioType.None)
        //{
        //    var audio = data.GetClip(sfxMusic);
        //    musicSource.volume = audio.mute ? 0f : audio.volume;
        //    musicSource.pitch = audio.pitch;
        //    musicSource.loop = true;
        //    musicSource.clip = audio.GetClip();
        //    musicSource.Play();
        //}
    }

    public static AudioData GetAudioData(AudioType type) => instance.data.GetClip(type);

    public static PooledAudioSource Play(AudioType type) => instance.Pool.Play(instance.data.GetClip(type));
    public static PooledAudioSource PlayAt(AudioType type, Vector3 pos) => instance.Pool.PlayAt(instance.data.GetClip(type), pos);
    public static PooledAudioSource PlayAttached(AudioType type, Transform transform) => instance.Pool.PlayAttached(instance.data.GetClip(type), transform);
    //public void Stop(AudioType type) => Pool.Stop(data.GetClip(type));

    //public void Click() => uiSource.PlayOneShot(click);
}
