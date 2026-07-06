using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] PlayerController m_Player;
    [SerializeField] AudioType sfxMelee;
    [SerializeField] AudioType sfxChop;

    private PooledAudioSource sfxMeleeHandler;

    void Start()
    {
        Events.AxeMelee += Meele;
    }

    void OnDestroy()
    {
        Events.AxeMelee -= Meele;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("AXE: " + other.name, other);
        if (other.CompareTag("Tree"))
        {
            AudioManager.PlayAt(sfxChop, transform.position);
            if(sfxMeleeHandler != null) sfxMeleeHandler.Stop();
            var tree = other.GetComponent<TreeObject>();
            tree.Chop(transform, m_Player);
        }
    }

    void Meele()
    {
        sfxMeleeHandler = AudioManager.PlayAt(sfxMelee, transform.position);
    }
}
