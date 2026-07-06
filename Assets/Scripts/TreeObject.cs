using DG.Tweening;
using System;
using UnityEngine;

public class TreeObject : MonoBehaviour, IDamage
{
    public Action<float> OnDamageTaken;
    public Action OnDestroyed;

    [SerializeField] int m_Strength = 3;
    [SerializeField] ParticleSystem leafs;
    [SerializeField] ParticleSystem poof;
    [SerializeField] ParticleSystem hitFx;
    [SerializeField] Material burnMaterial;
    [SerializeField] MeshRenderer treeRenderer;
    [SerializeField] MeshRenderer penRenderer;
    [SerializeField] Transform m_Tree;
    [SerializeField] TreeBalk m_BalkPrefab;

    [Header("Audio")]
    [SerializeField] AudioType sfxStartFall;
    [SerializeField] AudioType sfxFall;
    [SerializeField] AudioType sfxBurn;

    private bool isDestroyed;
    private float burned;
    private bool isBurned;
    private PooledAudioSource burnSFXHandle;
    private readonly int HeatProperty = Shader.PropertyToID("_Heat");

    public void Chop(Transform axe, PlayerController player)
    {
        Vector3 direction = transform.position - player.transform.position;

        transform.DOKill();
        transform.DOPunchRotation(direction * 10, 0.3f, 3, 0.5f);

        leafs.Play();

        float angle = -Vector3.SignedAngle(-direction, transform.forward, Vector3.up) - 90;
        angle = angle < 0 ? angle += 360 : angle;

        hitFx.transform.localEulerAngles = new Vector3(0, angle, 0);
        hitFx.Play();

        m_Strength--;

        if (m_Strength == 0)
        {
            int count = Utils.GetClosestTrees(transform.position, out Collider[] trees);
            if (count > 0)
            {
                Collider closest = trees[0];
                float closestSqrDistance = 9999;

                for (int i = 0; i < count; i++)
                {
                    float sqrDistance = (transform.position - trees[i].transform.position).sqrMagnitude;
                    if (sqrDistance < 0.3f) continue; //skip self

                    if (sqrDistance < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistance;
                        closest = trees[i];
                    }
                    //Debug.Log(trees[i].name + " | " + sqrDistance, trees[i]);
                }
                //Debug.Log(closest.name + " | " + closestSqrDistance, closest);
                direction = closest.transform.position - transform.position;
                //Debug.DrawRay(transform.position, direction, Color.red, 5f);
            }
            Debug.DrawRay(transform.position, direction, Color.red, 5f);
            Fall(direction);
        }
    }

    public void CollisionEnter(Collision collision)
    {
        if(isDestroyed) return;

        int layer = collision.gameObject.layer;

        //Debug.Log(gameObject.name + " collided with " + collision.collider.tag, collision.collider);
        if (layer == 7 && m_Strength > 0)
        {
            //Debug.Log("<color=yellow>COLLIDE</color>");
            m_Strength = 0;
            Vector3 direction = transform.position - collision.transform.position;
            Fall(direction.normalized);
        }
        if (layer == 6 || layer == 4)//Ground or Water
        {
            if(collision.thisCollider is BoxCollider)
                Destroy();
        }
        //print("CollisionEnter " + collision.thisCollider.GetType());
    }

    void Destroy()
    {
        if (isDestroyed)  return;

        isDestroyed = true;

        poof.transform.position = m_Tree.position;
        poof.transform.rotation = m_Tree.rotation;
        poof.Play();

        if(burnSFXHandle != null) burnSFXHandle.Stop();

        OnDestroyed?.Invoke();
        m_Tree.gameObject.SetActive(false);

        StopAllCoroutines();
        CreateBalk();
    }

    public float torqueForce = 1;

    public void Fall(Vector3 direction)
    {
        PlaySFX(sfxStartFall);

        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
        collider.excludeLayers = 1 << 3;//Ignore Player Layer

        var rb = transform.GetChild(1).GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Vector3 topPoint = transform.position + Vector3.up * 3f;
        rb.AddForceAtPosition(direction * 2, topPoint, ForceMode.Impulse);

        //Vector3 torque = Vector3.Cross(Vector3.up, direction.normalized);
        //rb.AddTorque(torque * 5f, ForceMode.Impulse);
    }

    void CreateBalk()
    {
        PlaySFX(sfxFall);

        TreeBalk balk = Instantiate(m_BalkPrefab, m_Tree.position, m_Tree.rotation);
        balk.transform.localScale = transform.localScale;
        balk.Split(m_Tree.localScale.y, burned);
    }

    public void Damage(float amount)
    {
        if (!isBurned)
        {
            burnSFXHandle = PlaySFX(sfxBurn);
            isBurned = true;
        }
        treeRenderer.material.SetFloat(HeatProperty, amount);
        penRenderer.material.SetFloat(HeatProperty, amount);
        burned = amount;

        if(!isDestroyed) OnDamageTaken?.Invoke(amount);
    }

    PooledAudioSource PlaySFX(AudioType type) => AudioManager.PlayAt(type, transform.position);
}
