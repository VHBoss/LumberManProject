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
    [SerializeField] float maxDistance = 2.2f;
    [SerializeField] float maxDamagePerSecond = 50f;

    [Header("SFX")]
    [SerializeField] AudioType sfxBurn;
    [SerializeField] AudioType sfxDeath;

    private Transform furnace;
    private PlayerController playerController;
    private float currentHealth = 100;
    private bool isDead;
    private bool isCooldown;
    private float timeout;
    public float burnTimer;
    private float currentDamage;
    private PooledAudioSource sfxBurnHandler;
    private float sfxBurnVolume;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        furnace = GameObject.FindAnyObjectByType<Furnace>().transform;
    }

    void Update()
    {
        if(isDead) return;

        if (furnace != null)
        {
            float sqr_distance = furnace.position.x - transform.position.x;

            if (sqr_distance <= maxDistance)
            {
                if (sfxBurnHandler == null)
                {
                    sfxBurnHandler = AudioManager.PlayAt(sfxBurn, transform.position);
                    sfxBurnVolume = sfxBurnHandler.Source.volume;
                }
                burnTimer += Time.deltaTime;
                burnTimer = Mathf.Clamp01(burnTimer);
                sfxBurnHandler.Source.volume = sfxBurnVolume * burnTimer;

                // Нанесение урона пропорционально расстоянию
                float danger = 1f - Mathf.Clamp01(sqr_distance / maxDistance);
                currentDamage = maxDamagePerSecond * danger * Time.deltaTime;
                currentHealth -= currentDamage;
                UpdateUI();

                // Сбрасываем таймер восстановления при получении урона
                timeout = recoverTimeout;
                isCooldown = false;

                if (currentHealth <= 0)
                {
                    Burn();
                }
            }
            else if (!isCooldown && currentHealth < 100 && timeout <= 0)
            {
                // Начинаем восстановление если вышли из зоны
                isCooldown = true;
            }
        }

        // Восстановление здоровья
        if (!isDead && isCooldown)
        {
            currentHealth += Time.deltaTime * recoverSpeed;
            UpdateUI();

            if (currentHealth >= 100)
            {
                currentHealth = 100;
                isCooldown = false;
            }
        }

        // Таймер перед началом восстановления
        if (!isCooldown && timeout > 0)
        {
            timeout -= Time.deltaTime;
        }

        // Обрабатываем звук каждый кадр
        ProcessAudio();
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
        if (isDead) return;

        playerController.SetDead();
        isDead = true;

        AudioManager.PlayAt(sfxDeath, transform.position);

        currentHealth = 0;
        UpdateUI();

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

    void ProcessAudio()
    {
        if (sfxBurnHandler == null)
            return;

        // Если рядом с печью, звук уже увеличивается в Update
        float distance = furnace.position.x - transform.position.x;
        if (distance <= maxDistance)
            return;

        burnTimer -= Time.deltaTime;
        burnTimer = Mathf.Clamp01(burnTimer);

        sfxBurnHandler.Source.volume = sfxBurnVolume * burnTimer;

        if (burnTimer <= 0f)
        {
            sfxBurnHandler.Stop();
            sfxBurnHandler = null;
        }
    }
}
