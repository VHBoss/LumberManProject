using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform gate;
    [SerializeField] Vector3 offset;
    [SerializeField] float gateOffset;
    [SerializeField] bool useCurrentCameraOffset;

    private Vector3 pos;

    void Start()
    {
        if(useCurrentCameraOffset) offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if(target == null) return;

        pos = offset;
        pos.x = target.position.x + offset.x;
        pos.x = Mathf.Max(pos.x, gate.position.x - gateOffset);
        transform.position = pos;
    }
}
