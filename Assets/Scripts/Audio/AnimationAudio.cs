using UnityEngine;

public class AnimationAudio : MonoBehaviour
{
    [SerializeField] AudioType sfxOpen;
    [SerializeField] AudioType sfxTake;

    public void Open() => AudioManager.PlayAt(sfxOpen, transform.position);
    public void Take() => AudioManager.PlayAt(sfxTake, transform.position);
}
