using UnityEngine;
using UnityEngine.Events;

public class TriggerCollector : MonoBehaviour
{
    public UnityEvent<Collider> onTriggerEnter;
    public UnityEvent<Collider> onTriggerExit;
    public UnityEvent<Collider> onTriggerStay;

    [SerializeField] bool debugLog;

    void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);

        if (debugLog) Debug.Log("OnTriggerEnter: " + other.name, other);
    }

    void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke(other);

        if (debugLog) Debug.Log("OnTriggerExit: " + other.name, other);
    }

    void OnTriggerStay(Collider other)
    {
        onTriggerStay.Invoke(other);
    }
}
