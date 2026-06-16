using UnityEngine;
using UnityEngine.Events;

public class CollisionCollector : MonoBehaviour
{
    public UnityEvent<Collision> onCollisionEnterEvent;

    [SerializeField] bool debugLog;

    void OnCollisionEnter(Collision collision)
    {
        onCollisionEnterEvent.Invoke(collision);

        if (debugLog) Debug.Log("OnCollisionEnter: " + collision.collider.name, collision.collider);
    }
}
