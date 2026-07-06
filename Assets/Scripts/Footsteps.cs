using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] AudioType sfxStep;

    public void PlayFootstep()
    {
        AudioManager.PlayAt(sfxStep, transform.position);
    }
}
