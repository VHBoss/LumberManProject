using System.Collections;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    [Header("Speed")]
    public float baseSpeed = 1f;
    public float currentSpeed;
    public int speedCount = 3;
    public float speedMultiplier = 0.5f;

    [Header("Hunger")]
    public float hunger = 0f;
    public float maxHunger = 100f;
    public float hungerPerSecond = 5;
    public float hungerZeroTimeout = 3;
    public float balkPrice = 5f;

    [Header("Sleep")]
    public float sleepDuration = 10f;

    [Header("UI")]
    public UIElement ui;

    public Animator animator;

    [Header("FX")]
    public ParticleSystem[] dust;

    [Header("SFX")]
    public AudioType sfxMove;
    public AudioType sfxEating;
    public AudioType sfxNextLevel;

    private PooledAudioSource sfxMoveHandle;
    private PooledAudioSource sfxEatingHandle;
    private int currentSpeedNum = 0;
    private int prevSpeedNum;
    private bool isEating;
    private float hungerTimeout;
    private bool exitLevel;

    void Start()
    {
        currentSpeed = baseSpeed;
        ui.SetCount(hunger, maxHunger);
        sfxMoveHandle = AudioManager.PlayAttached(sfxMove, transform);

        Events.LevelExited += ExitLevel;
    }

    void OnDestroy()
    {
        Events.LevelExited -= ExitLevel;
    }

    void Update()
    {
        if (isEating || exitLevel) return;

        transform.position -= Vector3.right * currentSpeed * Time.deltaTime;

        if (hungerTimeout > 0)
        {
            hungerTimeout -= Time.deltaTime;
        }
        else
        {
            hunger += hungerPerSecond * Time.deltaTime;
            hunger = hunger > maxHunger ? maxHunger : hunger;
        }

        ui.SetCount(hunger, maxHunger);

        float step = maxHunger / (float)speedCount;
        currentSpeedNum = Mathf.FloorToInt(hunger / step);

        if(prevSpeedNum != currentSpeedNum)
        {
            if(prevSpeedNum < currentSpeedNum) AudioManager.PlayAt(sfxNextLevel, transform.position);
            prevSpeedNum = currentSpeedNum;
        }

        currentSpeed = baseSpeed + baseSpeed * speedMultiplier * currentSpeedNum;
    }

    public void OnDeadZoneEnter(Collider other)
    {
        //Debug.Log("Furnace " + other.name + " | " + other.tag);
        if (other.CompareTag("Tree"))
        {
            var tree = other.GetComponent<TreeObject>();
            //tree.Burn();
            tree.Fall(transform.forward);
        }
        else if (other.CompareTag("BalkCollider"))
        {
            var balk = other.GetComponent<TreeLog>();
            balk.Burn(other);
        }
        else if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerHealth>();
            player.Burn();
        }
        //else if (other.CompareTag("Hub"))
        //{
        //    var hub = other.GetComponent<UnloadZone>();
        //    //hub.Unload(this);
        //    StartCoroutine(Eat(hub));
        //}
    }

    public void OnBurnZoneEnter(Collider other)
    {
        //Debug.Log("Furnace " + other.name + " | " + other.tag);
        if (other.CompareTag("Hub"))
        {
            var hub = other.GetComponent<UnloadZone>();
            StartCoroutine(Eat(hub));
        }
    }

    IEnumerator Eat(UnloadZone hub)
    {
        if (sfxMoveHandle != null)
        {
            sfxMoveHandle.Stop();
            sfxMoveHandle = null;
        }

        isEating = true;
        yield return new WaitForSeconds(1);
        sfxEatingHandle = AudioManager.PlayAttached(sfxEating, transform);
        hub.Unload(this);
        animator.SetBool("Eat", true);
        StopFX();
    }

    public void AddBalk(UnloadZone unloadZone)
    {
        hunger -= balkPrice;
        if (hunger <= 0)
        {
            hunger = 0;
            unloadZone.CancelUnload();
            Continue();
        }

        ui.SetCount(hunger, maxHunger);
    }

    public void Continue()
    {
        isEating = false;
        hungerTimeout = hungerZeroTimeout;
        animator.SetBool("Eat", false);
        StartFX();
        if (sfxEatingHandle != null)
        {
            sfxEatingHandle.Stop();
            sfxEatingHandle = null;
        }
        sfxMoveHandle = AudioManager.PlayAttached(AudioType.TreeBurn, transform);
    }

    void StartFX()
    {
        for (int i = 0; i < dust.Length; i++)
        {
            dust[i].Play();
        }
    }

    void StopFX()
    {
        for (int i = 0; i < dust.Length; i++)
        {
            dust[i].Stop();
        }
    }

    void ExitLevel()
    {
        exitLevel = true;
    }
}