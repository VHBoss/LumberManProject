using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TreeBalk : MonoBehaviour
{
    [SerializeField] Rigidbody[] m_Barks;
    [SerializeField] float forceX = 150;
    [SerializeField] float forceY = 20;
    [SerializeField] float breakPropability = 0.3f;
    [SerializeField] ParticleSystem poof;
    [SerializeField] Material burnMaterial;

    public void Split(float height)
    {
        float countf = height / 0.2f;
        int count = Mathf.FloorToInt(countf);

        gameObject.SetActive(true);

        for (int i = 0; i < m_Barks.Length; i++)
        {
            Rigidbody item = m_Barks[i];
            if (i < count)
            {
                item.AddForce(Random.Range(-forceX, forceX), forceY, forceX);

                bool destroy = Random.Range(0f, 1f) < breakPropability;
                if (destroy)
                {
                    StartCoroutine(WaitAndDestroy(item.gameObject));
                }
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }

        //Debug.Log(height + " | " + countf + " | " + count);
    }

    IEnumerator WaitAndDestroy(GameObject item)
    {
        yield return new WaitForSeconds(0.3f);

        if (item.GetComponent<BoxCollider>().enabled)//Если дерево ещё не подобрали
        {
            //poof.Play();
            GameObject.Instantiate(poof, item.transform.position, item.transform.rotation);
            //gameObject.SetActive(false);
            Destroy(item);
        }
    }

    public void Burn(Collider other)
    {
        other.tag = "Untagged";
        Transform root = other.transform.parent;
        if(root.GetComponent<MeshRenderer>().material == burnMaterial)
        {
            Debug.Log("Burn", other.gameObject);
        }
            root.GetComponent<MeshRenderer>().material = burnMaterial;
        DOTween.Sequence()
            .Append(root.DOScaleY(0, 0.3f))
            .Append(root.DOMoveY(-0.6f, 0.3f))
            .OnComplete(() => { if (root.gameObject != null) Destroy(root.gameObject); });
    }
}
