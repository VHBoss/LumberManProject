using DG.Tweening;
using UnityEngine;

public class Gates : MonoBehaviour
{
    [SerializeField] UIElement ui;
    [SerializeField] Transform gate;
    [SerializeField] Transform coinTarget;
    [SerializeField] Animator animator;
    [SerializeField] float loadTime = 0.2f;
    [SerializeField] int needCount = 10;

    private int currentCount;
    private LutCollector lutCollector;

    void Start()
    {
        UpdateUI(0);
    }

    void UpdateUI(int count)
    {
        ui.SetCount(count, needCount);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lutCollector = other.GetComponent<LutCollector>();
            InvokeRepeating("StartProcess", 0, loadTime);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke();
        }
    }

    void StartProcess()
    {
        if (currentCount >= needCount)
        {
            CancelInvoke();
            GetComponent<BoxCollider>().enabled = false;
            //gate.DOMoveY(-4, 2f);
            //animator.SetBool("Open", true);
            ui.gameObject.SetActive(false);
            return;
        }

        GameObject coin = lutCollector.GetCoin();
        if (coin != null)
        {
            coin.transform.SetParent(null);
            coin.SetActive(true);

            DOTween.Sequence()
                .Append(coin.transform.DOMove(coinTarget.position, 0.3f))
                .Join(coin.transform.DOScale(0.8f, 0.3f))
                .OnComplete(() =>
                {
                    currentCount++;
                    ui.SetCount(currentCount, needCount);

                    if (currentCount == needCount)
                    {
                        animator.SetBool("Open", true);
                    }
                    else
                    {
                        animator.SetTrigger("Take");
                    }
                    Destroy(coin.gameObject);
                });
        }
        else
        {
            CancelInvoke();
        }
    }

    public void OnExited(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Events.LevelExited?.Invoke();
        }
    }
}
