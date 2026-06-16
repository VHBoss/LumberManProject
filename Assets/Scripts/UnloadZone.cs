using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UnloadZone : MonoBehaviour
{
    [SerializeField] UIElement ui;
    [SerializeField] float loadTime = 0.1f;
    [SerializeField] int maxCount = 30;
    [SerializeField] Transform target;
    [SerializeField] Transform progress;
    [SerializeField] ParticleSystem poof;

    private BalkCollector balkCollector;
    private Furnace furnace;
    private int balkCount;
    private List<GameObject> balks = new List<GameObject>();
    private bool isUplolading;
    private bool isFull;

    void Start()
    {
        UpdateUI(0);
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("UnloadZone " + other.name + " | " + other.tag, other.gameObject);
        if (isUplolading || isFull) return;

        if (other.CompareTag("Player"))
        {
            balkCollector = other.GetComponent<BalkCollector>();
            InvokeRepeating("StartProcess", 0, loadTime);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Cancel();
    }

    void StartProcess()
    {
        if (balkCount >= maxCount)
        {
            isFull = true;
            GiveBonus();
            Cancel();
            return;
        }

        GameObject balk = balkCollector.GetBalk();
        if (balk != null)
        {
            balkCount++;
            ui.SetCount(balkCount, maxCount);
            balk.transform.SetParent(null);
            balk.SetActive(true);

            float delta = 0.4f/maxCount;

            DOTween.Sequence()
                .Append(balk.transform.DOMove(target.position, 0.3f))
                .Join(balk.transform.DOScale(0.8f, 0.3f))
                .OnComplete(() =>
                {
                    balks.Add(balk);
                    balk.SetActive(false);
                    //Destroy(balk.gameObject);
                    progress.localPosition = new Vector3(0, -0.4f + delta * balkCount, 0);
                    //progress.Translate(new Vector3(0, delta, 0));
                });
        }
        else
        {
            Cancel();
        }
    }

    [ContextMenu("GiveBonus")]
    void GiveBonus()
    {
        poof.Play();
        Events.GetCoins?.Invoke(transform.position);
    }

    void UpdateUI(int count)
    {
        ui.SetCount(count, maxCount);
    }

    void Cancel()
    {
        balkCollector = null;
        CancelInvoke("StartProcess");
    }

    public void Unload(Furnace furnace)
    {
        this.furnace = furnace;
        isUplolading = true;
        InvokeRepeating("UnloadProcess", 0, loadTime);
    }

    public void CancelUnload()
    {
        CancelInvoke();
        ui.gameObject.SetActive(false);
    }

    void UnloadProcess()
    {
        if (balkCount <= 0)
        {
            furnace.Continue();
            CancelUnload();
            return;
        }

        balkCount--;
        ui.SetCount(balkCount, maxCount);

        GameObject balk = GetBalk();
        if (balk != null)
        {
            float delta = 0.4f / maxCount;

            DOTween.Sequence()
                .Append(balk.transform.DOMove(furnace.transform.position, 0.3f))
                .Join(balk.transform.DOScale(0.8f, 0.3f))
                .OnComplete(() =>
                {
                    furnace.AddBalk(this);
                    Destroy(balk.gameObject);
                    progress.localPosition = new Vector3(0, -0.4f + delta * balkCount, 0);
                });
        }
    }

    public GameObject GetBalk()
    {
        if (balks.Count > 0)
        {
            GameObject balk = balks[0];
            balks.RemoveAt(0);
            balk.SetActive(true);
            return balk;
        }
        return null;
    }
}
