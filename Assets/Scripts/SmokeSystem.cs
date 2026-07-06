using System.Collections;
using UnityEngine;

public class SmokeSystem : MonoBehaviour
{
    [SerializeField] ParticleSystem[] smoke;

    private TreeObject tree;
    private WaitForSeconds wait = new WaitForSeconds(3);
    private int prevCount;

    private void Start()
    {
        tree = GetComponent<TreeObject>();
        tree.OnDamageTaken += Damage;
        tree.OnDestroyed += Destroyed;
    }

    void OnDestroy()
    {
        tree.OnDamageTaken -= Damage;
        tree.OnDestroyed -= Destroyed;
    }

    void Damage(float damage)
    {
        // Рассчитываем сколько систем должно быть активно (от 0 до количества систем)
        int activeCount = Mathf.RoundToInt(damage * smoke.Length);
        activeCount = Mathf.Clamp(activeCount, 0, smoke.Length);

        if (prevCount != activeCount)
        {
            prevCount = activeCount;
            UpdateSmokeSystem(activeCount);
        }
    }

    void UpdateSmokeSystem(int activeCount)
    {
        for (int i = 0; i < smoke.Length; i++)
        {
            if (i < activeCount)
            {
                if (!smoke[i].isPlaying) smoke[i].Play();
            }
        }
    }

    void Destroyed()
    {
        foreach (var system in smoke)
        {
            system.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        StartCoroutine(WaitAndStop());
    }

    IEnumerator WaitAndStop()
    {
        smoke[0].transform.parent.SetParent(null);
        yield return wait;
        Destroy(smoke[0].transform.parent.gameObject);
    }
}