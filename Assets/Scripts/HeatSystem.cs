using System.Collections.Generic;
using UnityEngine;

public class HeatSystem : MonoBehaviour
{
    [SerializeField] BoxCollider heatZone;
    private Dictionary<Transform, IDamage> heatObjects = new Dictionary<Transform, IDamage>();

    void Update()
    {
        float zonePosition = heatZone.transform.position.x + heatZone.transform.localScale.x * 0.5f;

        foreach (var obj in heatObjects)
        {
            if(obj.Key == null) continue;

            float distance = zonePosition - obj.Key.position.x;
            float damage = 1 - Mathf.Clamp01(distance / heatZone.transform.localScale.x);
            obj.Value.Damage(damage);
        }
    }

    public void OnHeatZoneEnter(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    var player = other.GetComponent<PlayerHealth>();
        //    player.Heat(transform);
        //}
        //else 
        if (other.CompareTag("Tree"))
        {
            var obj = other.GetComponent<IDamage>();
            heatObjects.Add(other.transform, obj);
        }
        else if (other.CompareTag("BalkCollider"))
        {
            var obj = other.GetComponent<IDamage>();
            if(heatObjects.ContainsKey(other.transform)) return;
            heatObjects.Add(other.transform, obj);
        }
    }

    public void OnHeatZoneExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    var player = other.GetComponent<PlayerHealth>();
        //    player.Heat(null);
        //}
        //else 
        if (other.CompareTag("Tree"))
        {
            var obj = other.GetComponent<IDamage>();
            heatObjects.Remove(other.transform);
        }
        else if (other.CompareTag("BalkCollider"))
        {
            var obj = other.GetComponent<IDamage>();
            heatObjects.Remove(other.transform);
            //Destroy(other.gameObject);
        }
    }
}
