using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TreeObject : MonoBehaviour
{
    [SerializeField] int m_Strength = 3;
    [SerializeField] ParticleSystem leafs;
    [SerializeField] ParticleSystem poof;
    [SerializeField] ParticleSystem hitFx;
    [SerializeField] Material burnMaterial;
    [SerializeField] MeshRenderer treeRenderer;
    [SerializeField] MeshRenderer penRenderer;
    [SerializeField] Transform m_Tree;
    [SerializeField] Transform m_TreeTop;
    [SerializeField] TreeBalk m_BalkPrefab;

    private bool OnFloor;
    private bool isBurned;
    private Vector3 prevPosition;
    private bool isDestroyed;

    public void Chop(Transform axe, PlayerController player)
    {
        if (isBurned) return;

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
            Fall(direction);
        }
    }

    public void CollisionEnter(Collision collision)
    {
        if(isDestroyed) return;

        //int layer = collision.collider.gameObject.layer;
        //if(layer == 0 || layer == 6)
        //{
        //    Destroy();
        //    return;
        //}
        //Debug.Log(gameObject.name + " collided with " + collision.collider.tag, collision.collider);
        if (collision.collider.CompareTag("Finish") && m_Strength > 0)
        {
            //Debug.Log("<color=yellow>COLLIDE</color>");
            m_Strength = 0;
            Vector3 direction = transform.position - collision.transform.position;
            Fall(direction);
        }
        if (collision.collider.CompareTag("Respawn") && !isDestroyed)
        {
            //Debug.Log("<color=orange>OnFloor</color>");
            Destroy();
        }
    }

    void Destroy()
    {
        if (isDestroyed)  return;

        isDestroyed = true;

        poof.transform.position = m_Tree.position;
        poof.transform.rotation = m_Tree.rotation;
        poof.Play();

        m_Tree.gameObject.SetActive(false);

        StopAllCoroutines();
        CreateBalk();
    }

    public void Fall(Vector3 direction)
    {
        var collider = GetComponent<Collider>();
        collider.enabled = false;

        var rb = transform.GetChild(1).GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddTorque(direction, ForceMode.Impulse);

        prevPosition = m_TreeTop.position;

        //StartCoroutine(CheckSpeed());
    }

    IEnumerator CheckSpeed()
    {
        while (true)
        {
            Vector3 direction = m_TreeTop.position - prevPosition;
            treeRenderer.material.SetVector("_Direction", direction);
            //m_Renderer.material.SetFloat("_Bend", direction.magnitude * 0.2f);
            yield return null;
        }
    }

    void Shake(Vector3 pos)
    {
        var rend = m_Tree.GetComponent<MeshRenderer>();
        rend.material.SetVector("_HitPosition", pos);
        rend.material.SetFloat("_ShakeStrength", 2);
        //rend.material.DOFloat(0, "_ShakeStrength", 0.5f);
    }

    void CreateBalk()
    {
        TreeBalk balk = Instantiate(m_BalkPrefab, m_Tree.position, m_Tree.rotation);
        balk.transform.localScale = transform.localScale;
        balk.Split(treeRenderer.transform.localScale.y);

        //TreeQuad tq = GetComponentInParent<TreeQuad>();
        //tq.DestroyTree(this);
    }

    public void Burn()
    {
        if(isBurned) return;
        isBurned = true;

        treeRenderer.material = burnMaterial;
        penRenderer.material = burnMaterial;

        transform.DOKill();
        //transform.DOScaleY(0, 0.5f).OnComplete(() => Destroy(gameObject));
        transform.DOMoveY(-4, 1f).OnComplete(() => Destroy(gameObject));
    }
}
