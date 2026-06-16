using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BalkCollector : MonoBehaviour
{
    [SerializeField] UIElement playerBag;
    [SerializeField] private Transform m_Target;
    [SerializeField] private Transform m_Bag;
    [SerializeField] int maxCount = 100;

    public int BalkCount => balks.Count;

    private List<GameObject> balks = new List<GameObject>();

    void Start()
    {
        UpdateUI(0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Balk"))
        {
            if (balks.Count < maxCount)
            {
                balks.Add(other.gameObject);
                UpdateUI(balks.Count);
                UpdateBag();

                other.enabled = false;
                Collect(other.transform.parent);
            }
        }
    }

    void Collect(Transform balk)
    {
        balk.GetComponent<BoxCollider>().enabled = false;
        balk.SetParent(transform);        

        DOTween.Sequence()
            .Append(balk.DOLocalMove(m_Target.localPosition, 0.3f))
            .Join(balk.DOScale(0.2f, 0.3f))
            .OnComplete(() =>
            {
                DOTween.Kill(balk, true);                
                balk.gameObject.SetActive(false);
                //Destroy(balk.gameObject);
            });
    }

    void UpdateBag()
    {
        float percent = (float)balks.Count / maxCount;
        m_Bag.localScale = Vector3.one * (1 + percent);
    }

    void UpdateUI(int count)
    {
        playerBag.SetCount(count, maxCount);
    }

    public GameObject GetBalk()
    {
        if (balks.Count > 0)
        {
            GameObject balk = balks[0];
            balks.RemoveAt(0);
            UpdateUI(balks.Count);
            UpdateBag();
            return balk;
        }
        return null;
    }
}
