using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TreeLog : MonoBehaviour, IDamage
{
    [SerializeField] ParticleSystem[] smoke;

    [Header("SFX")]
    [SerializeField] AudioType sfxBurn;

    private readonly int HeatProperty = Shader.PropertyToID("_Heat");
    private readonly int DissolveProperty = Shader.PropertyToID("_Dissolve");

    private WaitForSeconds wait = new WaitForSeconds(3);
    private MeshRenderer mr;
    private int prevCount;
    private float prevDamage;
    private bool isBurn;

    internal void Init(float burnAmount, float forceX, float forceY)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Random.Range(-forceX, forceX), forceY, forceX);
        mr = GetComponent<MeshRenderer>();
        Damage(burnAmount);
    }

    public void Damage(float damage)
    {
        if (damage < prevDamage) return;

        prevDamage = damage;

        UpdateSmokeSystem(damage);

        if(mr == null) mr = GetComponent<MeshRenderer>();
        mr.material.SetFloat(HeatProperty, damage);
    }

    void UpdateSmokeSystem(float damage)
    {
        int activeCount = Mathf.RoundToInt(damage * smoke.Length);
        activeCount = Mathf.Clamp(activeCount, 0, smoke.Length);

        if (prevCount != activeCount)
        {
            prevCount = activeCount;

            for (int i = 0; i < smoke.Length; i++)
            {
                if (i < activeCount && !smoke[i].isPlaying)
                {
                    smoke[i].Play();
                    float randomX = Random.Range(-0.1f, 0.1f);
                    float randomY = Random.Range(-0.1f, 0.1f);
                    float randomZ = Random.Range(-0.23f, 0.23f);
                    smoke[i].transform.localPosition = new Vector3(randomX, randomY, randomZ);
                }
            }
        }
    }

    public void Burn(Collider other)
    {
        if (!isBurn)
        {
            isBurn = true;
            AudioManager.PlayAt(sfxBurn, transform.position);
        }

        other.tag = "Untagged";
        Destroyed();
        MeshRenderer mr = other.GetComponent<MeshRenderer>();
        mr.material.DOFloat(0, DissolveProperty, 1.2f).OnComplete(() =>
        {
            other.gameObject.SetActive(false);
        });
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
