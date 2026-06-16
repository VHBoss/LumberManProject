using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Image damage;
    [SerializeField] UIElement hp;
    [SerializeField] float recoverTimeout = 5;
    [SerializeField] float recoverSpeed = 1;
    [SerializeField] float damageSpeed = 0.1f;

    private PlayerController playerController;
    private float currentHealth = 100;
    private bool isDead;
    private bool isHeating;
    private bool isCooldown;
    private float timeout;
    private float currentDamage;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if(isDead) return;

        if (isHeating)
        {
            currentDamage += Time.deltaTime * damageSpeed;
            currentHealth -= currentDamage;
            UpdateUI();

            if (currentHealth <= 0)
            {
                Burn();
            }
        }
        else
        {
            if (timeout > 0)
            {
                timeout -= Time.deltaTime;
                if (timeout <= 0) isCooldown = true;            
            }

            if (isCooldown)
            {
                currentHealth += Time.deltaTime * recoverSpeed;
                UpdateUI();
                if (currentHealth >= 100) isCooldown = false;
            }
        }
    }

    void UpdateUI()
    {
        hp.SetCountSimple(currentHealth, 100);
        Color c = damage.color;
        c.a = 1 - currentHealth / 100;
        damage.color = c;
    }

    public void Burn()
    {
        playerController.SetDead();
        isDead = true;

        DOTween.Sequence()
            .AppendInterval(2f)
            .Insert(0.5f, transform.DOScaleY(0, 0.3f))
            .Insert(0.5f, transform.DOMoveY(0.75f, 0.3f))
            .OnComplete(() =>
            {
                Destroy(gameObject);
                SceneManager.LoadScene(0);
            });
    }

    public void Heat(bool state)
    {
        isHeating = state;
        if (!state)
        {
            timeout = recoverTimeout;
            currentDamage = 0;
        }
    }
}
