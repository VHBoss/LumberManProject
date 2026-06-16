using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LutCollector : MonoBehaviour
{
    [SerializeField] UIElement ui;
    [SerializeField] Transform m_Target;

    private int currentCount = 0;
    private List<GameObject> coins = new List<GameObject>();

    void Start()
    {
        UpdateUI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lut"))
        {
            other.enabled = false;
            other.transform.SetParent(transform);

            currentCount++;
            UpdateUI();
            DOTween.Sequence()
                .Append(other.transform.DOLocalMove(m_Target.localPosition, 0.3f))
                .Join(other.transform.DOScale(0.2f, 0.3f))
                .OnComplete(() =>
                {
                    DOTween.Kill(other, true);
                    coins.Add(other.gameObject);
                    other.gameObject.SetActive(false);
                    //Destroy(other.gameObject);
                });
        }
    }

    void UpdateUI()
    {
        ui.SetCount(currentCount);
    }

    public GameObject GetCoin()
    {
        if (coins.Count > 0)
        {
            GameObject coin = coins[0];
            coins.RemoveAt(0);
            currentCount = coins.Count;
            UpdateUI();
            return coin;
        }
        return null;
    }
}
