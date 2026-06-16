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

    public int currentSpeedNum = 0;
    private bool isEating;
    private float hungerTimeout;

    void Start()
    {
        currentSpeed = baseSpeed;
        ui.SetCount(hunger, maxHunger);
    }

    void Update()
    {
        if (isEating) return;

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
        else if (other.CompareTag("Balk"))
        {
            var balk = other.GetComponentInParent<TreeBalk>();
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

    public void OnHeatZoneEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerHealth>();
            player.Heat(true);
        }
    }

    public void OnHeatZoneExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerHealth>();
            player.Heat(false);
        }
    }

    IEnumerator Eat(UnloadZone hub)
    {
        isEating = true;
        yield return new WaitForSeconds(1);
        hub.Unload(this);
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
    }
}